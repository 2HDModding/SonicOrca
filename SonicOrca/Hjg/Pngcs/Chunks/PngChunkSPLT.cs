// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkSPLT
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.IO;

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkSPLT : PngChunkMultiple
    {
      public const string ID = "sPLT";

      public string PalName { get; set; }

      public int SampleDepth { get; set; }

      public int[] Palette { get; set; }

      public PngChunkSPLT(ImageInfo info)
        : base("sPLT", info)
      {
        this.PalName = "";
      }

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.BEFORE_IDAT;
      }

      public override ChunkRaw CreateRawChunk()
      {
        MemoryStream os = new MemoryStream();
        ChunkHelper.WriteBytesToStream((Stream) os, ChunkHelper.ToBytes(this.PalName));
        os.WriteByte((byte) 0);
        os.WriteByte((byte) this.SampleDepth);
        int nentries = this.GetNentries();
        for (int index1 = 0; index1 < nentries; ++index1)
        {
          for (int index2 = 0; index2 < 4; ++index2)
          {
            if (this.SampleDepth == 8)
              PngHelperInternal.WriteByte((Stream) os, (byte) this.Palette[index1 * 5 + index2]);
            else
              PngHelperInternal.WriteInt2((Stream) os, this.Palette[index1 * 5 + index2]);
          }
          PngHelperInternal.WriteInt2((Stream) os, this.Palette[index1 * 5 + 4]);
        }
        byte[] array = os.ToArray();
        ChunkRaw emptyChunk = this.createEmptyChunk(array.Length, false);
        emptyChunk.Data = array;
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw c)
      {
        int len = -1;
        for (int index = 0; index < c.Data.Length; ++index)
        {
          if (c.Data[index] == (byte) 0)
          {
            len = index;
            break;
          }
        }
        if (len <= 0 || len > c.Data.Length - 2)
          throw new PngjException("bad sPLT chunk: no separator found");
        this.PalName = ChunkHelper.ToString(c.Data, 0, len);
        this.SampleDepth = PngHelperInternal.ReadInt1fromByte(c.Data, len + 1);
        int offset1 = len + 2;
        int num1 = (c.Data.Length - offset1) / (this.SampleDepth == 8 ? 6 : 10);
        this.Palette = new int[num1 * 5];
        int num2 = 0;
        for (int index1 = 0; index1 < num1; ++index1)
        {
          int num3;
          int num4;
          int num5;
          int offset2;
          int num6;
          if (this.SampleDepth == 8)
          {
            byte[] data1 = c.Data;
            int offset3 = offset1;
            int num7 = offset3 + 1;
            num3 = PngHelperInternal.ReadInt1fromByte(data1, offset3);
            byte[] data2 = c.Data;
            int offset4 = num7;
            int num8 = offset4 + 1;
            num4 = PngHelperInternal.ReadInt1fromByte(data2, offset4);
            byte[] data3 = c.Data;
            int offset5 = num8;
            int num9 = offset5 + 1;
            num5 = PngHelperInternal.ReadInt1fromByte(data3, offset5);
            byte[] data4 = c.Data;
            int offset6 = num9;
            offset2 = offset6 + 1;
            num6 = PngHelperInternal.ReadInt1fromByte(data4, offset6);
          }
          else
          {
            num3 = PngHelperInternal.ReadInt2fromBytes(c.Data, offset1);
            int offset7 = offset1 + 2;
            num4 = PngHelperInternal.ReadInt2fromBytes(c.Data, offset7);
            int offset8 = offset7 + 2;
            num5 = PngHelperInternal.ReadInt2fromBytes(c.Data, offset8);
            int offset9 = offset8 + 2;
            num6 = PngHelperInternal.ReadInt2fromBytes(c.Data, offset9);
            offset2 = offset9 + 2;
          }
          int num10 = PngHelperInternal.ReadInt2fromBytes(c.Data, offset2);
          offset1 = offset2 + 2;
          int[] palette1 = this.Palette;
          int index2 = num2;
          int num11 = index2 + 1;
          int num12 = num3;
          palette1[index2] = num12;
          int[] palette2 = this.Palette;
          int index3 = num11;
          int num13 = index3 + 1;
          int num14 = num4;
          palette2[index3] = num14;
          int[] palette3 = this.Palette;
          int index4 = num13;
          int num15 = index4 + 1;
          int num16 = num5;
          palette3[index4] = num16;
          int[] palette4 = this.Palette;
          int index5 = num15;
          int num17 = index5 + 1;
          int num18 = num6;
          palette4[index5] = num18;
          int[] palette5 = this.Palette;
          int index6 = num17;
          num2 = index6 + 1;
          int num19 = num10;
          palette5[index6] = num19;
        }
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkSPLT pngChunkSplt = (PngChunkSPLT) other;
        this.PalName = pngChunkSplt.PalName;
        this.SampleDepth = pngChunkSplt.SampleDepth;
        this.Palette = new int[pngChunkSplt.Palette.Length];
        Array.Copy((Array) pngChunkSplt.Palette, 0, (Array) this.Palette, 0, this.Palette.Length);
      }

      public int GetNentries() => this.Palette.Length / 5;
    }
}
