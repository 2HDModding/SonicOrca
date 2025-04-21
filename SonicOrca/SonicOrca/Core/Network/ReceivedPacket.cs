// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.ReceivedPacket
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Net;

namespace SonicOrca.Core.Network
{

    internal class ReceivedPacket
    {
      private readonly IPEndPoint _sender;
      private readonly Packet _packet;

      public IPEndPoint Sender => this._sender;

      public Packet Packet => this._packet;

      public ReceivedPacket(IPEndPoint sender, Packet packet)
      {
        this._sender = sender;
        this._packet = packet;
      }

      public override string ToString() => $"{this._sender} sent {this._packet}";
    }
}
