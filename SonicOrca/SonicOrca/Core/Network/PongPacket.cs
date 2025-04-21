// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.PongPacket
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.IO;

namespace SonicOrca.Core.Network
{

    internal class PongPacket : Packet
    {
      private readonly int _id;

      public int Id => this._id;

      public PongPacket(int id)
        : base(PacketType.Pong)
      {
        this._id = id;
      }

      public PongPacket(byte[] data)
        : base(PacketType.Pong)
      {
        using (MemoryStream input = new MemoryStream(data))
          this._id = new BinaryReader((Stream) input).ReadInt32();
      }

      protected override byte[] SerialiseData()
      {
        using (MemoryStream output = new MemoryStream())
        {
          new BinaryWriter((Stream) output).Write(this._id);
          return output.ToArray();
        }
      }

      public override string ToString() => string.Format("Packet = Pong");
    }
}
