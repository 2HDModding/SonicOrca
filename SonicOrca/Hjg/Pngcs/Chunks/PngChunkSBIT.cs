// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkSBIT
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkSBIT(ImageInfo info) : PngChunkSingle("sBIT", info)
    {
      public const string ID = "sBIT";

      public int Graysb { get; set; }

      public int Alphasb { get; set; }

      public int Redsb { get; set; }

      public int Greensb { get; set; }

      public int Bluesb { get; set; }

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
      }

      public override void ParseFromRaw(ChunkRaw c)
      {
        if (c.Length != this.GetLen())
          throw new PngjException("bad chunk length " + (object) c);
        if (this.ImgInfo.Greyscale)
        {
          this.Graysb = PngHelperInternal.ReadInt1fromByte(c.Data, 0);
          if (!this.ImgInfo.Alpha)
            return;
          this.Alphasb = PngHelperInternal.ReadInt1fromByte(c.Data, 1);
        }
        else
        {
          this.Redsb = PngHelperInternal.ReadInt1fromByte(c.Data, 0);
          this.Greensb = PngHelperInternal.ReadInt1fromByte(c.Data, 1);
          this.Bluesb = PngHelperInternal.ReadInt1fromByte(c.Data, 2);
          if (!this.ImgInfo.Alpha)
            return;
          this.Alphasb = PngHelperInternal.ReadInt1fromByte(c.Data, 3);
        }
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw emptyChunk = this.createEmptyChunk(this.GetLen(), true);
        if (this.ImgInfo.Greyscale)
        {
          emptyChunk.Data[0] = (byte) this.Graysb;
          if (this.ImgInfo.Alpha)
            emptyChunk.Data[1] = (byte) this.Alphasb;
        }
        else
        {
          emptyChunk.Data[0] = (byte) this.Redsb;
          emptyChunk.Data[1] = (byte) this.Greensb;
          emptyChunk.Data[2] = (byte) this.Bluesb;
          if (this.ImgInfo.Alpha)
            emptyChunk.Data[3] = (byte) this.Alphasb;
        }
        return emptyChunk;
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkSBIT pngChunkSbit = (PngChunkSBIT) other;
        this.Graysb = pngChunkSbit.Graysb;
        this.Redsb = pngChunkSbit.Redsb;
        this.Greensb = pngChunkSbit.Greensb;
        this.Bluesb = pngChunkSbit.Bluesb;
        this.Alphasb = pngChunkSbit.Alphasb;
      }

      private int GetLen()
      {
        int len = this.ImgInfo.Greyscale ? 1 : 3;
        if (this.ImgInfo.Alpha)
          ++len;
        return len;
      }
    }
}
