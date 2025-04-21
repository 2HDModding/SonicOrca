// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.PingPacket
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.IO;

namespace SonicOrca.Core.Network
{

    internal class PingPacket : Packet
    {
      private readonly int _id;
      private readonly int _roundTripTime;

      public int Id => this._id;

      public int RoundTripTime => this._roundTripTime;

      public PingPacket(int id, int roundTripTime)
        : base(PacketType.Ping)
      {
        this._id = id;
        this._roundTripTime = roundTripTime;
      }

      public PingPacket(byte[] data)
        : base(PacketType.Ping)
      {
        using (MemoryStream input = new MemoryStream(data))
        {
          BinaryReader binaryReader = new BinaryReader((Stream) input);
          this._id = binaryReader.ReadInt32();
          this._roundTripTime = binaryReader.ReadInt32();
        }
      }

      protected override byte[] SerialiseData()
      {
        using (MemoryStream output = new MemoryStream())
        {
          BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
          binaryWriter.Write(this._id);
          binaryWriter.Write(this._roundTripTime);
          return output.ToArray();
        }
      }

      public override string ToString() => $"Packet = Ping, RTT = {this._roundTripTime}";
    }
}
