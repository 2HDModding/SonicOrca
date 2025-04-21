// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.NetworkPlayer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace SonicOrca.Core.Network
{

    internal class NetworkPlayer
    {
      private readonly NetworkGameServer _networkGameServer;
      private readonly IPEndPoint _remoteEndPoint;
      private readonly string _name;
      private readonly ConcurrentQueue<Packet> _receivedPackets = new ConcurrentQueue<Packet>();
      private volatile int _roundTripTime;
      private int _lastPingId;
      private int _lastPingTickCount;
      private int _lastPacketReceivedTickCount;

      public IPEndPoint RemoteEndPoint => this._remoteEndPoint;

      public string Name => this._name;

      public Player Player { get; set; }

      public int CharacterId { get; set; }

      public Vector2 InputDirection { get; set; }

      public bool InputAction { get; set; }

      public bool InputDirty { get; set; }

      public int LastPacketReceivedTickCount => this._lastPacketReceivedTickCount;

      public bool Ready { get; set; }

      public NetworkPlayer(NetworkGameServer networkGameServer, IPEndPoint remoteEndPoint, string name)
      {
        this._networkGameServer = networkGameServer;
        this._remoteEndPoint = remoteEndPoint;
        this._name = name;
      }

      public void SendPacket(Packet packet) => this._networkGameServer.SendPacket(this, packet);

      public void ReceivePacket(Packet packet)
      {
        this._lastPacketReceivedTickCount = Environment.TickCount;
        if (packet.Type == PacketType.Pong)
          this.ReceivePong(((PongPacket) packet).Id);
        else
          this._receivedPackets.Enqueue(packet);
      }

      public bool TryGetNextPacket(out Packet packet) => this._receivedPackets.TryDequeue(out packet);

      public void Update()
      {
        if (Environment.TickCount > this._lastPingTickCount + 5000)
          this.Ping();
        Packet packet;
        while (this.TryGetNextPacket(out packet))
          this.ProcessPacket(packet);
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
        }
      }

      private void Ping()
      {
        ++this._lastPingId;
        this._lastPingTickCount = Environment.TickCount;
        Task.Run((Action) (() => this.SendPacket((Packet) new PingPacket(this._lastPingId, this._roundTripTime))));
      }

      private void ReceivePong(int id)
      {
        if (id != this._lastPingId)
          return;
        this._roundTripTime = Environment.TickCount - this._lastPingTickCount;
      }

      private void ProcessPlayInput(PlayInputPacket playInputPacket)
      {
        int characterId = playInputPacket.CharacterId;
        this.InputDirection = playInputPacket.Direction;
        this.InputAction = playInputPacket.Action;
        this.InputDirty = true;
      }
    }
}
