// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.PngDeinterlacer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs
{

    internal class PngDeinterlacer
    {
      private readonly ImageInfo imi;
      private int pass;
      private int rows;
      private int cols;
      private int dY;
      private int dX;
      private int oY;
      private int oX;
      private int oXsamples;
      private int dXsamples;
      private int currRowSubimg = -1;
      private int currRowReal = -1;
      private readonly int packedValsPerPixel;
      private readonly int packedMask;
      private readonly int packedShift;
      private int[][] imageInt;
      private byte[][] imageByte;

      internal PngDeinterlacer(ImageInfo iminfo)
      {
        this.imi = iminfo;
        this.pass = 0;
        if (this.imi.Packed)
        {
          this.packedValsPerPixel = 8 / this.imi.BitDepth;
          this.packedShift = this.imi.BitDepth;
          this.packedMask = this.imi.BitDepth != 1 ? (this.imi.BitDepth != 2 ? 240 /*0xF0*/ : 192 /*0xC0*/) : 128 /*0x80*/;
        }
        else
          this.packedMask = this.packedShift = this.packedValsPerPixel = 1;
        this.setPass(1);
        this.setRow(0);
      }

      internal void setRow(int n)
      {
        this.currRowSubimg = n;
        this.currRowReal = n * this.dY + this.oY;
        if (this.currRowReal < 0 || this.currRowReal >= this.imi.Rows)
          throw new PngjExceptionInternal("bad row - this should not happen");
      }

      internal void setPass(int p)
      {
        if (this.pass == p)
          return;
        this.pass = p;
        switch (this.pass)
        {
          case 1:
            this.dY = this.dX = 8;
            this.oX = this.oY = 0;
            break;
          case 2:
            this.dY = this.dX = 8;
            this.oX = 4;
            this.oY = 0;
            break;
          case 3:
            this.dX = 4;
            this.dY = 8;
            this.oX = 0;
            this.oY = 4;
            break;
          case 4:
            this.dX = this.dY = 4;
            this.oX = 2;
            this.oY = 0;
            break;
          case 5:
            this.dX = 2;
            this.dY = 4;
            this.oX = 0;
            this.oY = 2;
            break;
          case 6:
            this.dX = this.dY = 2;
            this.oX = 1;
            this.oY = 0;
            break;
          case 7:
            this.dX = 1;
            this.dY = 2;
            this.oX = 0;
            this.oY = 1;
            break;
          default:
            throw new PngjExceptionInternal("bad interlace pass" + (object) this.pass);
        }
        this.rows = (this.imi.Rows - this.oY) / this.dY + 1;
        if ((this.rows - 1) * this.dY + this.oY >= this.imi.Rows)
          --this.rows;
        this.cols = (this.imi.Cols - this.oX) / this.dX + 1;
        if ((this.cols - 1) * this.dX + this.oX >= this.imi.Cols)
          --this.cols;
        if (this.cols == 0)
          this.rows = 0;
        this.dXsamples = this.dX * this.imi.Channels;
        this.oXsamples = this.oX * this.imi.Channels;
      }

      internal void deinterlaceInt(int[] src, int[] dst, bool readInPackedFormat)
      {
        if (!(this.imi.Packed & readInPackedFormat))
        {
          int num = 0;
          int oXsamples = this.oXsamples;
          while (num < this.cols * this.imi.Channels)
          {
            for (int index = 0; index < this.imi.Channels; ++index)
              dst[oXsamples + index] = src[num + index];
            num += this.imi.Channels;
            oXsamples += this.dXsamples;
          }
        }
        else
          this.deinterlaceIntPacked(src, dst);
      }

      private void deinterlaceIntPacked(int[] src, int[] dst)
      {
        int packedMask = this.packedMask;
        int num1 = -1;
        int num2 = 0;
        int oX = this.oX;
        while (num2 < this.cols)
        {
          int index1 = num2 / this.packedValsPerPixel;
          ++num1;
          if (num1 >= this.packedValsPerPixel)
            num1 = 0;
          packedMask >>= this.packedShift;
          if (num1 == 0)
            packedMask = this.packedMask;
          int index2 = oX / this.packedValsPerPixel;
          int num3 = oX % this.packedValsPerPixel;
          int num4 = src[index1] & packedMask;
          int num5 = num1;
          int num6 = num3 - num5;
          if (num6 > 0)
            num4 >>= num6 * this.packedShift;
          else if (num6 < 0)
            num4 <<= -num6 * this.packedShift;
          dst[index2] |= num4;
          ++num2;
          oX += this.dX;
        }
      }

      internal void deinterlaceByte(byte[] src, byte[] dst, bool readInPackedFormat)
      {
        if (!(this.imi.Packed & readInPackedFormat))
        {
          int num = 0;
          int oXsamples = this.oXsamples;
          while (num < this.cols * this.imi.Channels)
          {
            for (int index = 0; index < this.imi.Channels; ++index)
              dst[oXsamples + index] = src[num + index];
            num += this.imi.Channels;
            oXsamples += this.dXsamples;
          }
        }
        else
          this.deinterlacePackedByte(src, dst);
      }

      private void deinterlacePackedByte(byte[] src, byte[] dst)
      {
        int packedMask = this.packedMask;
        int num1 = -1;
        int num2 = 0;
        int oX = this.oX;
        while (num2 < this.cols)
        {
          int index1 = num2 / this.packedValsPerPixel;
          ++num1;
          if (num1 >= this.packedValsPerPixel)
            num1 = 0;
          packedMask >>= this.packedShift;
          if (num1 == 0)
            packedMask = this.packedMask;
          int index2 = oX / this.packedValsPerPixel;
          int num3 = oX % this.packedValsPerPixel;
          int num4 = (int) src[index1] & packedMask;
          int num5 = num1;
          int num6 = num3 - num5;
          if (num6 > 0)
            num4 >>= num6 * this.packedShift;
          else if (num6 < 0)
            num4 <<= -num6 * this.packedShift;
          dst[index2] |= (byte) num4;
          ++num2;
          oX += this.dX;
        }
      }

      internal bool isAtLastRow() => this.pass == 7 && this.currRowSubimg == this.rows - 1;

      internal int getCurrRowSubimg() => this.currRowSubimg;

      internal int getCurrRowReal() => this.currRowReal;

      internal int getPass() => this.pass;

      internal int getRows() => this.rows;

      internal int getCols() => this.cols;

      internal int getPixelsToRead() => this.getCols();

      internal int[][] getImageInt() => this.imageInt;

      internal void setImageInt(int[][] imageInt) => this.imageInt = imageInt;

      internal byte[][] getImageByte() => this.imageByte;

      internal void setImageByte(byte[][] imageByte) => this.imageByte = imageByte;
    }
}
