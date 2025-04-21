// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkIHDR
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.IO;

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkIHDR(ImageInfo info) : PngChunkSingle("IHDR", info)
    {
      public const string ID = "IHDR";

      public int Cols { get; set; }

      public int Rows { get; set; }

      public int Bitspc { get; set; }

      public int Colormodel { get; set; }

      public int Compmeth { get; set; }

      public int Filmeth { get; set; }

      public int Interlaced { get; set; }

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.NA;
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw rawChunk = new ChunkRaw(13, ChunkHelper.b_IHDR, true);
        int offset1 = 0;
        PngHelperInternal.WriteInt4tobytes(this.Cols, rawChunk.Data, offset1);
        int offset2 = offset1 + 4;
        PngHelperInternal.WriteInt4tobytes(this.Rows, rawChunk.Data, offset2);
        int num1 = offset2 + 4;
        byte[] data1 = rawChunk.Data;
        int index1 = num1;
        int num2 = index1 + 1;
        int bitspc = (int) (byte) this.Bitspc;
        data1[index1] = (byte) bitspc;
        byte[] data2 = rawChunk.Data;
        int index2 = num2;
        int num3 = index2 + 1;
        int colormodel = (int) (byte) this.Colormodel;
        data2[index2] = (byte) colormodel;
        byte[] data3 = rawChunk.Data;
        int index3 = num3;
        int num4 = index3 + 1;
        int compmeth = (int) (byte) this.Compmeth;
        data3[index3] = (byte) compmeth;
        byte[] data4 = rawChunk.Data;
        int index4 = num4;
        int num5 = index4 + 1;
        int filmeth = (int) (byte) this.Filmeth;
        data4[index4] = (byte) filmeth;
        byte[] data5 = rawChunk.Data;
        int index5 = num5;
        int num6 = index5 + 1;
        int interlaced = (int) (byte) this.Interlaced;
        data5[index5] = (byte) interlaced;
        return rawChunk;
      }

      public override void ParseFromRaw(ChunkRaw c)
      {
        MemoryStream mask0 = c.Length == 13 ? c.GetAsByteStream() : throw new PngjException("Bad IDHR len " + (object) c.Length);
        this.Cols = PngHelperInternal.ReadInt4((Stream) mask0);
        this.Rows = PngHelperInternal.ReadInt4((Stream) mask0);
        this.Bitspc = PngHelperInternal.ReadByte((Stream) mask0);
        this.Colormodel = PngHelperInternal.ReadByte((Stream) mask0);
        this.Compmeth = PngHelperInternal.ReadByte((Stream) mask0);
        this.Filmeth = PngHelperInternal.ReadByte((Stream) mask0);
        this.Interlaced = PngHelperInternal.ReadByte((Stream) mask0);
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkIHDR pngChunkIhdr = (PngChunkIHDR) other;
        this.Cols = pngChunkIhdr.Cols;
        this.Rows = pngChunkIhdr.Rows;
        this.Bitspc = pngChunkIhdr.Bitspc;
        this.Colormodel = pngChunkIhdr.Colormodel;
        this.Compmeth = pngChunkIhdr.Compmeth;
        this.Filmeth = pngChunkIhdr.Filmeth;
        this.Interlaced = pngChunkIhdr.Interlaced;
      }
    }
}
