// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.UdpPacketRadio
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core.Network
{

    internal class UdpPacketRadio : IPacketRadio, IDisposable, IObservable<ReceivedPacket>
    {
      private readonly Lockable<List<IObserver<ReceivedPacket>>> _subscribers = new Lockable<List<IObserver<ReceivedPacket>>>(new List<IObserver<ReceivedPacket>>());
      private readonly UdpClient _udpClient;
      private int _packetsSent;
      private int _packetsReceived;

      public UdpPacketRadio(int port)
      {
        this._udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        Trace.WriteLine($"Listening for UDP packets on port {(object) port}.");
        this.RunReceiveThread();
      }

      public UdpPacketRadio(IPAddress serverIpAddress, int port)
      {
        this._udpClient = new UdpClient();
        this._udpClient.Connect(serverIpAddress, port);
        this.RunReceiveThread();
      }

      public UdpPacketRadio(string serverHost, int port)
      {
        this._udpClient = new UdpClient();
        IPAddress address;
        if (IPAddress.TryParse(serverHost, out address))
          this._udpClient.Connect(address, port);
        else
          this._udpClient.Connect(serverHost, port);
        this.RunReceiveThread();
      }

      public void Dispose() => this._udpClient.Close();

      private void RunReceiveThread()
      {
        Task.Run((Func<Task>) (async () =>
        {
          while (true)
          {
            UdpReceiveResult async = await this._udpClient.ReceiveAsync();
            this.OnReceivePacket(new ReceivedPacket(async.RemoteEndPoint, Packet.FromBuffer(async.Buffer)));
          }
        }));
      }

      private void OnReceivePacket(ReceivedPacket receivedPacket)
      {
        Console.WriteLine("RECEIVE " + (object) receivedPacket.Packet);
        Interlocked.Increment(ref this._packetsReceived);
        lock (this._subscribers.Sync)
        {
          foreach (IObserver<ReceivedPacket> observer in this._subscribers.Instance)
            observer.OnNext(receivedPacket);
        }
      }

      public async Task SendPacketAsync(Packet packet)
      {
        byte[] numArray = packet.Serialise();
        int num = await this._udpClient.SendAsync(numArray, numArray.Length);
        Interlocked.Increment(ref this._packetsSent);
        Console.WriteLine("SEND " + (object) packet);
      }

      public async Task SendPacketAsync(IPEndPoint destination, Packet packet)
      {
        byte[] numArray = packet.Serialise();
        int num = await this._udpClient.SendAsync(numArray, numArray.Length, destination);
        Interlocked.Increment(ref this._packetsSent);
        Console.WriteLine("SEND " + (object) packet);
      }

      public IDisposable Subscribe(IObserver<ReceivedPacket> observer)
      {
        lock (this._subscribers.Sync)
          this._subscribers.Instance.Add(observer);
        return Disposable.FromAction((Action) (() =>
        {
          lock (this._subscribers.Sync)
            this._subscribers.Instance.Remove(observer);
        }));
      }

      public override string ToString()
      {
        return string.Format("UdpPacketRadio Sent = {0} Received {1}", (object) this._packetsSent, (object) this._packetsReceived, (object) this._udpClient);
      }
    }
}
