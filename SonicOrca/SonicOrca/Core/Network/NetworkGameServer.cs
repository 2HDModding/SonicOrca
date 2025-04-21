// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.NetworkGameServer
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
using System.Net;
using System.Threading.Tasks;

namespace SonicOrca.Core.Network
{

    internal class NetworkGameServer : IDisposable, IObserver<ReceivedPacket>
    {
      private readonly ConcurrentQueue<Packet> _receivedPackets = new ConcurrentQueue<Packet>();
      private readonly Lockable<Queue<Tuple<IPEndPoint, Packet>>> _outgoingPackets = new Lockable<Queue<Tuple<IPEndPoint, Packet>>>(new Queue<Tuple<IPEndPoint, Packet>>());
      private IPacketRadio _radio;
      private readonly Lockable<List<NetworkPlayer>> _networkPlayers = new Lockable<List<NetworkPlayer>>(new List<NetworkPlayer>());
      private Vector2[] _characterInputDirection = new Vector2[2];
      private bool[] _characterInputAction = new bool[2];
      private int _lastCharacterSynchronisationTickCount;

      public bool AllowClientsToConnect { get; set; }

      public IReadOnlyList<NetworkPlayer> NetworkPlayers
      {
        get
        {
          lock (this._networkPlayers.Sync)
            return (IReadOnlyList<NetworkPlayer>) this._networkPlayers.Instance.ToArray();
        }
      }

      public Level Level { get; set; }

      public NetworkGameServer(Level level, int port)
      {
        this.Level = level;
        this._radio = (IPacketRadio) new UdpPacketRadio(port);
        this._radio.Subscribe((IObserver<ReceivedPacket>) this);
        this.AllowClientsToConnect = true;
      }

      public void Dispose() => this._radio.Dispose();

      private NetworkPlayer GetNetworkPlayer(IPEndPoint remoteEndPoint)
      {
        lock (this._networkPlayers.Sync)
          return this._networkPlayers.Instance.FirstOrDefault<NetworkPlayer>((Func<NetworkPlayer, bool>) (x => x.RemoteEndPoint.Equals((object) remoteEndPoint)));
      }

      private void OnReceiveHandshake(ReceivedPacket receivedPacket)
      {
        if (!this.AllowClientsToConnect)
          return;
        NetworkPlayer newNetworkPlayer = new NetworkPlayer(this, receivedPacket.Sender, "No name");
        lock (this._networkPlayers.Sync)
          this._networkPlayers.Instance.Add(newNetworkPlayer);
        Task.Run((Func<Task>) (() => this._radio.SendPacketAsync(newNetworkPlayer.RemoteEndPoint, (Packet) new NotifyPacket(PacketType.ConnectAck))));
      }

      private void MaintainConnections()
      {
        List<NetworkPlayer> networkPlayerList = new List<NetworkPlayer>();
        lock (this._networkPlayers.Sync)
        {
          foreach (NetworkPlayer networkPlayer in this._networkPlayers.Instance)
          {
            if (Environment.TickCount >= networkPlayer.LastPacketReceivedTickCount + 30000)
            {
              this._networkPlayers.Instance.Remove(networkPlayer);
              networkPlayerList.Add(networkPlayer);
            }
          }
        }
        foreach (NetworkPlayer networkPlayer in networkPlayerList)
          this.OnDisconnect(networkPlayer);
      }

      private void OnDisconnect(NetworkPlayer networkPlayer)
      {
      }

      public void OnCompleted() => throw new NotImplementedException();

      public void OnError(Exception error) => throw new NotImplementedException();

      public void OnNext(ReceivedPacket receivedPacket)
      {
        if (receivedPacket.Packet.Type == PacketType.Connect)
          this.OnReceiveHandshake(receivedPacket);
        else
          this.GetNetworkPlayer(receivedPacket.Sender)?.ReceivePacket(receivedPacket.Packet);
      }

      public void SendPacket(NetworkPlayer networkPlayer, Packet packet)
      {
        lock (this._outgoingPackets.Sync)
          this._outgoingPackets.Instance.Enqueue(new Tuple<IPEndPoint, Packet>(networkPlayer.RemoteEndPoint, packet));
      }

      public void SendPacket(IEnumerable<NetworkPlayer> networkPlayers, Packet packet)
      {
        foreach (NetworkPlayer networkPlayer in networkPlayers)
          this.SendPacket(networkPlayer, packet);
      }

      public void SendPacketToAllClients(Packet packet)
      {
        lock (this._networkPlayers.Sync)
        {
          foreach (NetworkPlayer networkPlayer in this._networkPlayers.Instance)
            this.SendPacket(networkPlayer, packet);
        }
      }

      public void Update()
      {
        lock (this._networkPlayers.Sync)
        {
          foreach (NetworkPlayer networkPlayer in this._networkPlayers.Instance)
            networkPlayer.Update();
        }
        if (this.Level != null)
        {
          this.PerformCharacterInputs();
          this.ConstructPackets();
        }
        IEnumerable<Tuple<IPEndPoint, Packet>> packetsToSend;
        lock (this._outgoingPackets.Sync)
        {
          packetsToSend = (IEnumerable<Tuple<IPEndPoint, Packet>>) this._outgoingPackets.Instance.ToArray();
          this._outgoingPackets.Instance.Clear();
        }
        Task.Run((Func<Task>) (async () =>
        {
          foreach (Tuple<IPEndPoint, Packet> tuple in packetsToSend)
            await this._radio.SendPacketAsync(tuple.Item1, tuple.Item2);
        }));
      }

      private void ConstructPackets()
      {
        if (this._lastCharacterSynchronisationTickCount + 200 >= Environment.TickCount)
          return;
        this._lastCharacterSynchronisationTickCount = Environment.TickCount;
        this.SendCharacterSynchronisation();
      }

      private void PerformCharacterInputs()
      {
        ICharacter[] array = this.Level.ObjectManager.Characters.ToArray<ICharacter>();
        lock (this._networkPlayers.Sync)
        {
          foreach (NetworkPlayer networkPlayer1 in this._networkPlayers.Instance)
          {
            NetworkPlayer networkPlayer = networkPlayer1;
            int characterId = networkPlayer.CharacterId;
            if (characterId >= 0 && characterId < array.Length)
            {
              ICharacter character = array[characterId];
              CharacterInputState characterInputState = new CharacterInputState();
              Vector2 inputDirection = networkPlayer.InputDirection;
              characterInputState.Throttle = inputDirection.X;
              inputDirection = networkPlayer.InputDirection;
              characterInputState.VerticalDirection = Math.Sign(inputDirection.Y);
              characterInputState.A = networkPlayer.InputAction ? CharacterInputButtonState.Down : CharacterInputButtonState.Up;
              character.Input = characterInputState;
              if (networkPlayer.InputDirty)
              {
                foreach (NetworkPlayer networkPlayer2 in this._networkPlayers.Instance.Where<NetworkPlayer>((Func<NetworkPlayer, bool>) (x => x != networkPlayer)))
                  networkPlayer2.SendPacket((Packet) new PlayInputPacket(characterId, networkPlayer.InputDirection, networkPlayer.InputAction));
                networkPlayer.InputDirty = false;
              }
            }
          }
        }
      }

      private void SendCharacterSynchronisation()
      {
        ICharacter[] array = this.Level.ObjectManager.Characters.ToArray<ICharacter>();
        if (array.Length == 0)
          return;
        this.SendPacketToAllClients((Packet) new CharacterSynchronisationPacket((IEnumerable<ICharacter>) array));
      }
    }
}
