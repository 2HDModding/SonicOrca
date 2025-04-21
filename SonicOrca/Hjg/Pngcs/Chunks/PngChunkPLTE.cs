// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkPLTE
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkPLTE : PngChunkSingle
    {
      public const string ID = "PLTE";
      private int nentries;
      private int[] entries;

      public PngChunkPLTE(ImageInfo info)
        : base("PLTE", info)
      {
        this.nentries = 0;
      }

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.NA;
      }

      public override ChunkRaw CreateRawChunk()
      {
        int len = 3 * this.nentries;
        int[] rgb = new int[3];
        ChunkRaw emptyChunk = this.createEmptyChunk(len, true);
        int n = 0;
        int num1 = 0;
        for (; n < this.nentries; ++n)
        {
          this.GetEntryRgb(n, rgb);
          byte[] data1 = emptyChunk.Data;
          int index1 = num1;
          int num2 = index1 + 1;
          int num3 = (int) (byte) rgb[0];
          data1[index1] = (byte) num3;
          byte[] data2 = emptyChunk.Data;
          int index2 = num2;
          int num4 = index2 + 1;
          int num5 = (int) (byte) rgb[1];
          data2[index2] = (byte) num5;
          byte[] data3 = emptyChunk.Data;
          int index3 = num4;
          num1 = index3 + 1;
          int num6 = (int) (byte) rgb[2];
          data3[index3] = (byte) num6;
        }
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw chunk)
      {
        this.SetNentries(chunk.Length / 3);
        int num1 = 0;
        int num2 = 0;
        for (; num1 < this.nentries; ++num1)
        {
          int n = num1;
          byte[] data1 = chunk.Data;
          int index1 = num2;
          int num3 = index1 + 1;
          int r = (int) data1[index1] & (int) byte.MaxValue;
          byte[] data2 = chunk.Data;
          int index2 = num3;
          int num4 = index2 + 1;
          int g = (int) data2[index2] & (int) byte.MaxValue;
          byte[] data3 = chunk.Data;
          int index3 = num4;
          num2 = index3 + 1;
          int b = (int) data3[index3] & (int) byte.MaxValue;
          this.SetEntry(n, r, g, b);
        }
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkPLTE pngChunkPlte = (PngChunkPLTE) other;
        this.SetNentries(pngChunkPlte.GetNentries());
        Array.Copy((Array) pngChunkPlte.entries, 0, (Array) this.entries, 0, this.nentries);
      }

      public void SetNentries(int nentries)
      {
        this.nentries = nentries;
        if (nentries < 1 || nentries > 256 /*0x0100*/)
          throw new PngjException("invalid pallette - nentries=" + (object) nentries);
        if (this.entries != null && this.entries.Length == nentries)
          return;
        this.entries = new int[nentries];
      }

      public int GetNentries() => this.nentries;

      public void SetEntry(int n, int r, int g, int b)
      {
        this.entries[n] = r << 16 /*0x10*/ | g << 8 | b;
      }

      public int GetEntry(int n) => this.entries[n];

      public void GetEntryRgb(int index, int[] rgb, int offset)
      {
        int entry = this.entries[index];
        rgb[offset] = (entry & 16711680 /*0xFF0000*/) >> 16 /*0x10*/;
        rgb[offset + 1] = (entry & 65280) >> 8;
        rgb[offset + 2] = entry & (int) byte.MaxValue;
      }

      public void GetEntryRgb(int n, int[] rgb) => this.GetEntryRgb(n, rgb, 0);

      public int MinBitDepth()
      {
        if (this.nentries <= 2)
          return 1;
        if (this.nentries <= 4)
          return 2;
        return this.nentries <= 16 /*0x10*/ ? 4 : 8;
      }
    }
}
