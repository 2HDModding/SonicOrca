// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkTRNS
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkTRNS(ImageInfo info) : PngChunkSingle("tRNS", info)
    {
      public const string ID = "tRNS";
      private int gray;
      private int red;
      private int green;
      private int blue;
      private int[] paletteAlpha;

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
          emptyChunk = this.createEmptyChunk(this.paletteAlpha.Length, true);
          for (int index = 0; index < emptyChunk.Length; ++index)
            emptyChunk.Data[index] = (byte) this.paletteAlpha[index];
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
          int length = c.Data.Length;
          this.paletteAlpha = new int[length];
          for (int index = 0; index < length; ++index)
            this.paletteAlpha[index] = (int) c.Data[index] & (int) byte.MaxValue;
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
        PngChunkTRNS pngChunkTrns = (PngChunkTRNS) other;
        this.gray = pngChunkTrns.gray;
        this.red = pngChunkTrns.red;
        this.green = pngChunkTrns.green;
        this.blue = pngChunkTrns.blue;
        if (pngChunkTrns.paletteAlpha == null)
          return;
        this.paletteAlpha = new int[pngChunkTrns.paletteAlpha.Length];
        Array.Copy((Array) pngChunkTrns.paletteAlpha, 0, (Array) this.paletteAlpha, 0, this.paletteAlpha.Length);
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

      public void SetGray(int g)
      {
        if (!this.ImgInfo.Greyscale)
          throw new PngjException("only grayscale images support this");
        this.gray = g;
      }

      public int GetGray()
      {
        if (!this.ImgInfo.Greyscale)
          throw new PngjException("only grayscale images support this");
        return this.gray;
      }

      public void SetPalletteAlpha(int[] palAlpha)
      {
        if (!this.ImgInfo.Indexed)
          throw new PngjException("only indexed images support this");
        this.paletteAlpha = palAlpha;
      }

      public void setIndexEntryAsTransparent(int palAlphaIndex)
      {
        if (!this.ImgInfo.Indexed)
          throw new PngjException("only indexed images support this");
        this.paletteAlpha = new int[1]{ palAlphaIndex + 1 };
        for (int index = 0; index < palAlphaIndex; ++index)
          this.paletteAlpha[index] = (int) byte.MaxValue;
        this.paletteAlpha[palAlphaIndex] = 0;
      }

      public int[] GetPalletteAlpha()
      {
        if (!this.ImgInfo.Indexed)
          throw new PngjException("only indexed images support this");
        return this.paletteAlpha;
      }
    }
}
