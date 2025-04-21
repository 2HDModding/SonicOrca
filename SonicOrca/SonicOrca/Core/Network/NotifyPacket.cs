// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.NotifyPacket
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace SonicOrca.Core.Network
{

    internal class NotifyPacket(PacketType packetType) : Packet(packetType)
    {
      protected override byte[] SerialiseData() => new byte[0];

      public override string ToString() => $"Notify packet = {this.Type}";
    }
}
