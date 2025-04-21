// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.ChunkRaw
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Hjg.Pngcs.Zlib;
using System;
using System.IO;

namespace Hjg.Pngcs.Chunks
{

    public class ChunkRaw
    {
      public readonly int Length;
      public readonly byte[] IdBytes;
      public byte[] Data;
      private int crcval;

      internal ChunkRaw(int length, byte[] idbytes, bool alloc)
      {
        this.IdBytes = new byte[4];
        this.Data = (byte[]) null;
        this.crcval = 0;
        this.Length = length;
        Array.Copy((Array) idbytes, 0, (Array) this.IdBytes, 0, 4);
        if (!alloc)
          return;
        this.AllocData();
      }

      private int ComputeCrc()
      {
        CRC32 crc = PngHelperInternal.GetCRC();
        crc.Reset();
        crc.Update(this.IdBytes, 0, 4);
        if (this.Length > 0)
          crc.Update(this.Data, 0, this.Length);
        return (int) crc.GetValue();
      }

      internal void WriteChunk(Stream os)
      {
        if (this.IdBytes.Length != 4)
          throw new PngjOutputException($"bad chunkid [{ChunkHelper.ToString(this.IdBytes)}]");
        this.crcval = this.ComputeCrc();
        PngHelperInternal.WriteInt4(os, this.Length);
        PngHelperInternal.WriteBytes(os, this.IdBytes);
        if (this.Length > 0)
          PngHelperInternal.WriteBytes(os, this.Data, 0, this.Length);
        PngHelperInternal.WriteInt4(os, this.crcval);
      }

      internal int ReadChunkData(Stream stream, bool checkCrc)
      {
        PngHelperInternal.ReadBytes(stream, this.Data, 0, this.Length);
        this.crcval = PngHelperInternal.ReadInt4(stream);
        if (checkCrc)
        {
          int crc = this.ComputeCrc();
          if (crc != this.crcval)
            throw new PngjBadCrcException($"crc invalid for chunk {this.ToString()} calc={(object) crc} read={(object) this.crcval}");
        }
        return this.Length + 4;
      }

      internal MemoryStream GetAsByteStream() => new MemoryStream(this.Data);

      private void AllocData()
      {
        if (this.Data != null && this.Data.Length >= this.Length)
          return;
        this.Data = new byte[this.Length];
      }

      public override string ToString()
      {
        return $"chunkid={ChunkHelper.ToString(this.IdBytes)} len={(object) this.Length}";
      }
    }
}
