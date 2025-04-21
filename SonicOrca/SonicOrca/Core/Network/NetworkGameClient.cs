// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.NetworkGameClient
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Objects;
using SonicOrca.Core.Objects.Base;
using SonicOrca.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SonicOrca.Core.Network
{

    internal class NetworkGameClient : IDisposable, IObserver<ReceivedPacket>
    {
      private readonly ConcurrentQueue<Packet> _receivedPackets = new ConcurrentQueue<Packet>();
      private readonly Queue<Packet> _outgoingPackets = new Queue<Packet>();
      private IPacketRadio _radio;
      private NetworkGameClient.ConnectionState _connectionState;
      private int _roundTripTime;
      private int _characterId;
      private Vector2[] _characterInputDirection = new Vector2[2];
      private bool[] _characterInputAction = new bool[2];
      private TaskCompletionSource<bool> _handshakeAcknoledged;

      public bool Ready { get; set; }

      public bool Connected => this._connectionState == NetworkGameClient.ConnectionState.Connected;

      public Level Level { get; set; }

      public NetworkGameClient(Level level)
      {
        this._connectionState = NetworkGameClient.ConnectionState.NotConnected;
        this.Level = level;
      }

      public void Dispose()
      {
        if (this._radio == null)
          return;
        this._radio.Dispose();
      }

      public async Task InitiateHandshake(string serverHost, int port, int timeout = 5000)
      {
        NetworkGameClient networkGameClient = this;
        networkGameClient._radio = (IPacketRadio) new UdpPacketRadio(serverHost, port);
        networkGameClient._radio.Subscribe((IObserver<ReceivedPacket>) networkGameClient);
        networkGameClient._connectionState = NetworkGameClient.ConnectionState.ConnectingToServer;
        networkGameClient._handshakeAcknoledged = new TaskCompletionSource<bool>();
        await networkGameClient._radio.SendPacketAsync((Packet) new NotifyPacket(PacketType.Connect));
        Task task = await Task.WhenAny(new Task[2]
        {
          Task.Delay(timeout),
          (Task) networkGameClient._handshakeAcknoledged.Task
        });
        if (!networkGameClient._handshakeAcknoledged.Task.IsCompleted)
          throw new TimeoutException("Connecting to server timed out.");
        if (!networkGameClient._handshakeAcknoledged.Task.Result)
          throw new NetworkException("Server refused connection.");
        networkGameClient._connectionState = NetworkGameClient.ConnectionState.Connected;
      }

      private void OnReceiveHandshakeAcknoledgement(Packet packet)
      {
        if (this._connectionState != NetworkGameClient.ConnectionState.ConnectingToServer)
          return;
        this._handshakeAcknoledged.SetResult(true);
      }

      public void Pong(PingPacket packet)
      {
        Task.Run((Func<Task>) (() => this._radio.SendPacketAsync((Packet) new PongPacket(packet.Id))));
      }

      public void OnCompleted() => throw new NotImplementedException();

      public void OnError(Exception error) => throw new NotImplementedException();

      public void OnNext(ReceivedPacket receivedPacket)
      {
        switch (receivedPacket.Packet.Type)
        {
          case PacketType.ConnectAck:
            this.OnReceiveHandshakeAcknoledgement(receivedPacket.Packet);
            break;
          case PacketType.Ping:
            PingPacket packet = (PingPacket) receivedPacket.Packet;
            this.Pong(packet);
            this._roundTripTime = packet.RoundTripTime;
            break;
          default:
            this._receivedPackets.Enqueue(receivedPacket.Packet);
            break;
        }
      }

      public void SendPacket(Packet packet)
      {
        Task.Run((Func<Task>) (() => this._radio.SendPacketAsync(packet)));
      }

      public void Update()
      {
        this._outgoingPackets.Clear();
        Packet result;
        while (this._receivedPackets.TryDequeue(out result))
          this.ProcessPacket(result);
        if (this.Level != null)
        {
          this.PerformCharacterInputs();
          this.ConstructPackets();
        }
        IEnumerable<Packet> packetsToSend = (IEnumerable<Packet>) this._outgoingPackets.ToArray();
        Task.Run((Func<Task>) (async () =>
        {
          foreach (Packet packet in packetsToSend)
            await this._radio.SendPacketAsync(packet);
        }));
      }

      private void ProcessPacket(Packet packet)
      {
        switch (packet.Type)
        {
          case PacketType.ReadyToStartLevel:
            this.Ready = true;
            break;
          case PacketType.PlayInput:
            this.ProcessPlayInput((PlayInputPacket) packet);
            break;
          case PacketType.CharacterSynchronisation:
            this.ProcessCharacterSynchronisation((CharacterSynchronisationPacket) packet);
            break;
        }
      }

      private void ConstructPackets() => this.SendPlayInput();

      private void ProcessPlayInput(PlayInputPacket playInputPacket)
      {
        int characterId = playInputPacket.CharacterId;
        if (characterId == this._characterId)
          return;
        this._characterInputDirection[characterId] = playInputPacket.Direction;
        this._characterInputAction[characterId] = playInputPacket.Action;
      }

      private void ProcessCharacterSynchronisation(
        CharacterSynchronisationPacket characterSynchronisation)
      {
        if (characterSynchronisation == null)
          return;
        characterSynchronisation.Apply((IReadOnlyList<ICharacter>) this.Level.ObjectManager.Characters.ToArray<ICharacter>(), this._roundTripTime);
        characterSynchronisation = (CharacterSynchronisationPacket) null;
      }

      private void SendPlayInput()
      {
        SonicOrcaGameContext gameContext = this.Level.GameContext;
        if (this._characterInputDirection[this._characterId] == gameContext.Current[0].DirectionLeft && this._characterInputAction[this._characterId] == gameContext.Current[0].Action1)
          return;
        this._characterInputDirection[this._characterId] = gameContext.Current[0].DirectionLeft;
        this._characterInputAction[this._characterId] = gameContext.Current[0].Action1;
        this._outgoingPackets.Enqueue((Packet) new PlayInputPacket(this._characterId, this._characterInputDirection[this._characterId], this._characterInputAction[this._characterId]));
      }

      private void PerformCharacterInputs()
      {
        ICharacter[] array = this.Level.ObjectManager.Characters.ToArray<ICharacter>();
        for (int index = 0; index < 2; ++index)
        {
          if (array.Length > index)
            array[index].Input = new CharacterInputState()
            {
              Throttle = this._characterInputDirection[index].X,
              VerticalDirection = Math.Sign(this._characterInputDirection[index].Y),
              A = this._characterInputAction[index] ? CharacterInputButtonState.Down : CharacterInputButtonState.Up
            };
        }
      }

      private enum ConnectionState
      {
        NotConnected,
        ConnectingToServer,
        Connected,
      }
    }
}
