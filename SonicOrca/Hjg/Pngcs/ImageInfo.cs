// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.ImageInfo
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs
{

    public class ImageInfo
    {
      private const int MAX_COLS_ROWS_VAL = 400000;
      public readonly int Cols;
      public readonly int Rows;
      public readonly int BitDepth;
      public readonly int Channels;
      public readonly int BitspPixel;
      public readonly int BytesPixel;
      public readonly int BytesPerRow;
      public readonly int SamplesPerRow;
      public readonly int SamplesPerRowPacked;
      public readonly bool Alpha;
      public readonly bool Greyscale;
      public readonly bool Indexed;
      public readonly bool Packed;

      public ImageInfo(int cols, int rows, int bitdepth, bool alpha)
        : this(cols, rows, bitdepth, alpha, false, false)
      {
      }

      public ImageInfo(int cols, int rows, int bitdepth, bool alpha, bool grayscale, bool palette)
      {
        this.Cols = cols;
        this.Rows = rows;
        this.Alpha = alpha;
        this.Indexed = palette;
        this.Greyscale = grayscale;
        if (this.Greyscale & palette)
          throw new PngjException("palette and greyscale are exclusive");
        this.Channels = grayscale | palette ? (alpha ? 2 : 1) : (alpha ? 4 : 3);
        this.BitDepth = bitdepth;
        this.Packed = bitdepth < 8;
        this.BitspPixel = this.Channels * this.BitDepth;
        this.BytesPixel = (this.BitspPixel + 7) / 8;
        this.BytesPerRow = (this.BitspPixel * cols + 7) / 8;
        this.SamplesPerRow = this.Channels * this.Cols;
        this.SamplesPerRowPacked = this.Packed ? this.BytesPerRow : this.SamplesPerRow;
        switch (this.BitDepth)
        {
          case 1:
          case 2:
          case 4:
            if (!this.Indexed && !this.Greyscale)
              throw new PngjException("only indexed or grayscale can have bitdepth=" + (object) this.BitDepth);
            goto case 8;
          case 8:
            if (cols < 1 || cols > 400000)
              throw new PngjException($"invalid cols={(object) cols} ???");
            if (rows >= 1 && rows <= 400000)
              break;
            throw new PngjException($"invalid rows={(object) rows} ???");
          case 16 /*0x10*/:
            if (this.Indexed)
              throw new PngjException("indexed can't have bitdepth=" + (object) this.BitDepth);
            goto case 8;
          default:
            throw new PngjException("invalid bitdepth=" + (object) this.BitDepth);
        }
      }

      public override string ToString()
      {
        return $"ImageInfo [cols={(object) this.Cols}, rows={(object) this.Rows}, bitDepth={(object) this.BitDepth}, channels={(object) this.Channels}, bitspPixel={(object) this.BitspPixel}, bytesPixel={(object) this.BytesPixel}, bytesPerRow={(object) this.BytesPerRow}, samplesPerRow={(object) this.SamplesPerRow}, samplesPerRowP={(object) this.SamplesPerRowPacked}, alpha={this.Alpha.ToString()}, greyscale={this.Greyscale.ToString()}, indexed={this.Indexed.ToString()}, packed={this.Packed.ToString()}]";
      }

      public override int GetHashCode()
      {
        int num = 31 /*0x1F*/;
        return num * (num * (num * (num * (num * (num * (num * 1 + (this.Alpha ? 1231 : 1237)) + this.BitDepth) + this.Channels) + this.Cols) + (this.Greyscale ? 1231 : 1237)) + (this.Indexed ? 1231 : 1237)) + this.Rows;
      }

      public override bool Equals(object obj)
      {
        if (this == obj)
          return true;
        if (obj == null || (object) this.GetType() != (object) obj.GetType())
          return false;
        ImageInfo imageInfo = (ImageInfo) obj;
        return this.Alpha == imageInfo.Alpha && this.BitDepth == imageInfo.BitDepth && this.Channels == imageInfo.Channels && this.Cols == imageInfo.Cols && this.Greyscale == imageInfo.Greyscale && this.Indexed == imageInfo.Indexed && this.Rows == imageInfo.Rows;
      }
    }
}
