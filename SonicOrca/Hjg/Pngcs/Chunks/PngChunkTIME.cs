// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkTIME
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkTIME(ImageInfo info) : PngChunkSingle("tIME", info)
    {
      public const string ID = "tIME";
      private int year;
      private int mon;
      private int day;
      private int hour;
      private int min;
      private int sec;

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.NONE;
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw emptyChunk = this.createEmptyChunk(7, true);
        PngHelperInternal.WriteInt2tobytes(this.year, emptyChunk.Data, 0);
        emptyChunk.Data[2] = (byte) this.mon;
        emptyChunk.Data[3] = (byte) this.day;
        emptyChunk.Data[4] = (byte) this.hour;
        emptyChunk.Data[5] = (byte) this.min;
        emptyChunk.Data[6] = (byte) this.sec;
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw chunk)
      {
        this.year = chunk.Length == 7 ? PngHelperInternal.ReadInt2fromBytes(chunk.Data, 0) : throw new PngjException("bad chunk " + (object) chunk);
        this.mon = PngHelperInternal.ReadInt1fromByte(chunk.Data, 2);
        this.day = PngHelperInternal.ReadInt1fromByte(chunk.Data, 3);
        this.hour = PngHelperInternal.ReadInt1fromByte(chunk.Data, 4);
        this.min = PngHelperInternal.ReadInt1fromByte(chunk.Data, 5);
        this.sec = PngHelperInternal.ReadInt1fromByte(chunk.Data, 6);
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkTIME pngChunkTime = (PngChunkTIME) other;
        this.year = pngChunkTime.year;
        this.mon = pngChunkTime.mon;
        this.day = pngChunkTime.day;
        this.hour = pngChunkTime.hour;
        this.min = pngChunkTime.min;
        this.sec = pngChunkTime.sec;
      }

      public void SetNow(int secsAgo)
      {
        DateTime now = DateTime.Now;
        this.year = now.Year;
        this.mon = now.Month;
        this.day = now.Day;
        this.hour = now.Hour;
        this.min = now.Minute;
        this.sec = now.Second;
      }

      internal void SetYMDHMS(int yearx, int monx, int dayx, int hourx, int minx, int secx)
      {
        this.year = yearx;
        this.mon = monx;
        this.day = dayx;
        this.hour = hourx;
        this.min = minx;
        this.sec = secx;
      }

      public int[] GetYMDHMS()
      {
        return new int[6]
        {
          this.year,
          this.mon,
          this.day,
          this.hour,
          this.min,
          this.sec
        };
      }

      public string GetAsString()
      {
        return string.Format("%04d/%02d/%02d %02d:%02d:%02d", (object) this.year, (object) this.mon, (object) this.day, (object) this.hour, (object) this.min, (object) this.sec);
      }
    }
}
