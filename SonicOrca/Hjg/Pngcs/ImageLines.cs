// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.ImageLines
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs
{

    public class ImageLines
    {
      internal readonly int channels;
      internal readonly int bitDepth;
      internal readonly int elementsPerRow;

      public ImageInfo ImgInfo { get; private set; }

      public ImageLine.ESampleType sampleType { get; private set; }

      public bool SamplesUnpacked { get; private set; }

      public int RowOffset { get; private set; }

      public int Nrows { get; private set; }

      public int RowStep { get; private set; }

      public int[][] Scanlines { get; private set; }

      public byte[][] ScanlinesB { get; private set; }

      public ImageLines(
        ImageInfo ImgInfo,
        ImageLine.ESampleType sampleType,
        bool unpackedMode,
        int rowOffset,
        int nRows,
        int rowStep)
      {
        this.ImgInfo = ImgInfo;
        this.channels = ImgInfo.Channels;
        this.bitDepth = ImgInfo.BitDepth;
        this.sampleType = sampleType;
        this.SamplesUnpacked = unpackedMode || !ImgInfo.Packed;
        this.RowOffset = rowOffset;
        this.Nrows = nRows;
        this.RowStep = rowStep;
        this.elementsPerRow = unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked;
        if (sampleType == ImageLine.ESampleType.INT)
        {
          this.Scanlines = new int[nRows][];
          for (int index = 0; index < nRows; ++index)
            this.Scanlines[index] = new int[this.elementsPerRow];
          this.ScanlinesB = (byte[][]) null;
        }
        else
        {
          if (sampleType != ImageLine.ESampleType.BYTE)
            throw new PngjExceptionInternal("bad ImageLine initialization");
          this.ScanlinesB = new byte[nRows][];
          for (int index = 0; index < nRows; ++index)
            this.ScanlinesB[index] = new byte[this.elementsPerRow];
          this.Scanlines = (int[][]) null;
        }
      }

      public int ImageRowToMatrixRow(int imrow)
      {
        int num = (imrow - this.RowOffset) / this.RowStep;
        if (num < 0)
          return 0;
        return num >= this.Nrows ? this.Nrows - 1 : num;
      }

      public int ImageRowToMatrixRowStrict(int imrow)
      {
        imrow -= this.RowOffset;
        int num = imrow < 0 || imrow % this.RowStep != 0 ? -1 : imrow / this.RowStep;
        return num >= this.Nrows ? -1 : num;
      }

      public int MatrixRowToImageRow(int mrow) => mrow * this.RowStep + this.RowOffset;

      public ImageLine GetImageLineAtMatrixRow(int mrow)
      {
        if (mrow < 0 || mrow > this.Nrows)
          throw new PngjException($"Bad row {(object) mrow}. Should be positive and less than {(object) this.Nrows}");
        ImageLine imageLineAtMatrixRow = this.sampleType == ImageLine.ESampleType.INT ? new ImageLine(this.ImgInfo, this.sampleType, this.SamplesUnpacked, this.Scanlines[mrow], (byte[]) null) : new ImageLine(this.ImgInfo, this.sampleType, this.SamplesUnpacked, (int[]) null, this.ScanlinesB[mrow]);
        imageLineAtMatrixRow.Rown = this.MatrixRowToImageRow(mrow);
        return imageLineAtMatrixRow;
      }
    }
}
