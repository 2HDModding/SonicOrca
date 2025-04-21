// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.LevelSynchronisationPacket
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.IO;

namespace SonicOrca.Core.Network
{

    internal class LevelSynchronisationPacket : Packet
    {
      private int _ticks;
      private int _time;

      public LevelSynchronisationPacket(Level level)
        : base(PacketType.LevelSynchronisation)
      {
        this._ticks = level.Ticks;
        this._time = level.Time;
      }

      public LevelSynchronisationPacket(byte[] data)
        : base(PacketType.LevelSynchronisation)
      {
        using (MemoryStream input = new MemoryStream(data))
        {
          BinaryReader binaryReader = new BinaryReader((Stream) input);
          this._ticks = binaryReader.ReadInt32();
          this._time = binaryReader.ReadInt32();
        }
      }

      protected override byte[] SerialiseData()
      {
        using (MemoryStream output = new MemoryStream())
        {
          BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
          binaryWriter.Write(this._ticks);
          binaryWriter.Write(this._time);
          return output.ToArray();
        }
      }

      public void Apply(Level level)
      {
        level.Ticks = this._ticks;
        level.Time = this._time;
      }

      public override string ToString() => string.Format("Packet = LevelSynchronisationPacket");
    }
}
