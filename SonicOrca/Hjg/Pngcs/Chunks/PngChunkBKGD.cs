// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkBKGD
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkBKGD(ImageInfo info) : PngChunkSingle("bKGD", info)
    {
      public const string ID = "bKGD";
      private int gray;
      private int red;
      private int green;
      private int blue;
      private int paletteIndex;

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw emptyChunk;
        if (this.ImgInfo.Greyscale)
        {
          emptyChunk = this.createEmptyChunk(2, true);
          PngHelperInternal.WriteInt2tobytes(this.gray, emptyChunk.Data, 0);
        }
        else if (this.ImgInfo.Indexed)
        {
          emptyChunk = this.createEmptyChunk(1, true);
          emptyChunk.Data[0] = (byte) this.paletteIndex;
        }
        else
        {
          emptyChunk = this.createEmptyChunk(6, true);
          PngHelperInternal.WriteInt2tobytes(this.red, emptyChunk.Data, 0);
          PngHelperInternal.WriteInt2tobytes(this.green, emptyChunk.Data, 0);
          PngHelperInternal.WriteInt2tobytes(this.blue, emptyChunk.Data, 0);
        }
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw c)
      {
        if (this.ImgInfo.Greyscale)
          this.gray = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
        else if (this.ImgInfo.Indexed)
        {
          this.paletteIndex = (int) c.Data[0] & (int) byte.MaxValue;
        }
        else
        {
          this.red = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
          this.green = PngHelperInternal.ReadInt2fromBytes(c.Data, 2);
          this.blue = PngHelperInternal.ReadInt2fromBytes(c.Data, 4);
        }
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkBKGD pngChunkBkgd = (PngChunkBKGD) other;
        this.gray = pngChunkBkgd.gray;
        this.red = pngChunkBkgd.red;
        this.green = pngChunkBkgd.red;
        this.blue = pngChunkBkgd.red;
        this.paletteIndex = pngChunkBkgd.paletteIndex;
      }

      public void SetGray(int gray)
      {
        if (!this.ImgInfo.Greyscale)
          throw new PngjException("only gray images support this");
        this.gray = gray;
      }

      public int GetGray()
      {
        if (!this.ImgInfo.Greyscale)
          throw new PngjException("only gray images support this");
        return this.gray;
      }

      public void SetPaletteIndex(int index)
      {
        if (!this.ImgInfo.Indexed)
          throw new PngjException("only indexed (pallete) images support this");
        this.paletteIndex = index;
      }

      public int GetPaletteIndex()
      {
        if (!this.ImgInfo.Indexed)
          throw new PngjException("only indexed (pallete) images support this");
        return this.paletteIndex;
      }

      public void SetRGB(int r, int g, int b)
      {
        if (this.ImgInfo.Greyscale || this.ImgInfo.Indexed)
          throw new PngjException("only rgb or rgba images support this");
        this.red = r;
        this.green = g;
        this.blue = b;
      }

      public int[] GetRGB()
      {
        if (this.ImgInfo.Greyscale || this.ImgInfo.Indexed)
          throw new PngjException("only rgb or rgba images support this");
        return new int[3]{ this.red, this.green, this.blue };
      }
    }
}
