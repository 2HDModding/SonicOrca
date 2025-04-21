// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.ImageLine
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace Hjg.Pngcs
{

    public class ImageLine
    {
      internal readonly int channels;
      internal readonly int bitDepth;

      public ImageInfo ImgInfo { get; private set; }

      public int[] Scanline { get; private set; }

      public byte[] ScanlineB { get; private set; }

      public int Rown { get; set; }

      public int ElementsPerRow { get; private set; }

      public int maxSampleVal { get; private set; }

      public ImageLine.ESampleType SampleType { get; private set; }

      public bool SamplesUnpacked { get; private set; }

      public FilterType FilterUsed { get; set; }

      public ImageLine(ImageInfo imgInfo)
        : this(imgInfo, ImageLine.ESampleType.INT, false)
      {
      }

      public ImageLine(ImageInfo imgInfo, ImageLine.ESampleType stype)
        : this(imgInfo, stype, false)
      {
      }

      public ImageLine(ImageInfo imgInfo, ImageLine.ESampleType stype, bool unpackedMode)
        : this(imgInfo, stype, unpackedMode, (int[]) null, (byte[]) null)
      {
      }

      internal ImageLine(
        ImageInfo imgInfo,
        ImageLine.ESampleType stype,
        bool unpackedMode,
        int[] sci,
        byte[] scb)
      {
        this.ImgInfo = imgInfo;
        this.channels = imgInfo.Channels;
        this.bitDepth = imgInfo.BitDepth;
        this.FilterUsed = FilterType.FILTER_UNKNOWN;
        this.SampleType = stype;
        this.SamplesUnpacked = unpackedMode || !imgInfo.Packed;
        this.ElementsPerRow = this.SamplesUnpacked ? imgInfo.SamplesPerRow : imgInfo.SamplesPerRowPacked;
        if (stype == ImageLine.ESampleType.INT)
        {
          this.Scanline = sci != null ? sci : new int[this.ElementsPerRow];
          this.ScanlineB = (byte[]) null;
          this.maxSampleVal = this.bitDepth == 16 /*0x10*/ ? (int) ushort.MaxValue : ImageLine.GetMaskForPackedFormatsLs(this.bitDepth);
        }
        else
        {
          if (stype != ImageLine.ESampleType.BYTE)
            throw new PngjExceptionInternal("bad ImageLine initialization");
          this.ScanlineB = scb != null ? scb : new byte[this.ElementsPerRow];
          this.Scanline = (int[]) null;
          this.maxSampleVal = this.bitDepth == 16 /*0x10*/ ? (int) byte.MaxValue : ImageLine.GetMaskForPackedFormatsLs(this.bitDepth);
        }
        this.Rown = -1;
      }

      internal static void unpackInplaceInt(ImageInfo iminfo, int[] src, int[] dst, bool Scale)
      {
        int bitDepth = iminfo.BitDepth;
        if (bitDepth >= 8)
          return;
        int forPackedFormatsLs = ImageLine.GetMaskForPackedFormatsLs(bitDepth);
        int num1 = 8 - bitDepth;
        int num2 = 8 * iminfo.SamplesPerRowPacked - bitDepth * iminfo.SamplesPerRow;
        int num3;
        int num4;
        if (num2 != 8)
        {
          num3 = forPackedFormatsLs << num2;
          num4 = num2;
        }
        else
        {
          num3 = forPackedFormatsLs;
          num4 = 0;
        }
        int index1 = iminfo.SamplesPerRow - 1;
        int index2 = iminfo.SamplesPerRowPacked - 1;
        for (; index1 >= 0; --index1)
        {
          int num5 = (src[index2] & num3) >> num4;
          if (Scale)
            num5 <<= num1;
          dst[index1] = num5;
          num3 <<= bitDepth;
          num4 += bitDepth;
          if (num4 == 8)
          {
            num3 = forPackedFormatsLs;
            num4 = 0;
            --index2;
          }
        }
      }

      internal static void packInplaceInt(ImageInfo iminfo, int[] src, int[] dst, bool scaled)
      {
        int bitDepth = iminfo.BitDepth;
        if (bitDepth >= 8)
          return;
        int forPackedFormatsLs = ImageLine.GetMaskForPackedFormatsLs(bitDepth);
        int num1 = 8 - bitDepth;
        int num2 = 8 - bitDepth;
        int num3 = 8 - bitDepth;
        int num4 = src[0];
        dst[0] = 0;
        if (scaled)
          num4 >>= num1;
        int num5 = (num4 & forPackedFormatsLs) << num3;
        int index1 = 0;
        for (int index2 = 0; index2 < iminfo.SamplesPerRow; ++index2)
        {
          int num6 = src[index2];
          if (scaled)
            num6 >>= num1;
          dst[index1] |= (num6 & forPackedFormatsLs) << num3;
          num3 -= bitDepth;
          if (num3 < 0)
          {
            num3 = num2;
            ++index1;
            dst[index1] = 0;
          }
        }
        dst[0] |= num5;
      }

      internal static void unpackInplaceByte(ImageInfo iminfo, byte[] src, byte[] dst, bool scale)
      {
        int bitDepth = iminfo.BitDepth;
        if (bitDepth >= 8)
          return;
        int forPackedFormatsLs = ImageLine.GetMaskForPackedFormatsLs(bitDepth);
        int num1 = 8 - bitDepth;
        int num2 = 8 * iminfo.SamplesPerRowPacked - bitDepth * iminfo.SamplesPerRow;
        int num3;
        int num4;
        if (num2 != 8)
        {
          num3 = forPackedFormatsLs << num2;
          num4 = num2;
        }
        else
        {
          num3 = forPackedFormatsLs;
          num4 = 0;
        }
        int index1 = iminfo.SamplesPerRow - 1;
        int index2 = iminfo.SamplesPerRowPacked - 1;
        for (; index1 >= 0; --index1)
        {
          int num5 = ((int) src[index2] & num3) >> num4;
          if (scale)
            num5 <<= num1;
          dst[index1] = (byte) num5;
          num3 <<= bitDepth;
          num4 += bitDepth;
          if (num4 == 8)
          {
            num3 = forPackedFormatsLs;
            num4 = 0;
            --index2;
          }
        }
      }

      internal static void packInplaceByte(ImageInfo iminfo, byte[] src, byte[] dst, bool scaled)
      {
        int bitDepth = iminfo.BitDepth;
        if (bitDepth >= 8)
          return;
        byte forPackedFormatsLs = (byte) ImageLine.GetMaskForPackedFormatsLs(bitDepth);
        byte num1 = (byte) (8 - bitDepth);
        byte num2 = (byte) (8 - bitDepth);
        int num3 = 8 - bitDepth;
        byte num4 = src[0];
        dst[0] = (byte) 0;
        if (scaled)
          num4 >>= (int) num1;
        byte num5 = (byte) (((int) num4 & (int) forPackedFormatsLs) << num3);
        int index1 = 0;
        for (int index2 = 0; index2 < iminfo.SamplesPerRow; ++index2)
        {
          byte num6 = src[index2];
          if (scaled)
            num6 >>= (int) num1;
          dst[index1] |= (byte) (((int) num6 & (int) forPackedFormatsLs) << num3);
          num3 -= bitDepth;
          if (num3 < 0)
          {
            num3 = (int) num2;
            ++index1;
            dst[index1] = (byte) 0;
          }
        }
        dst[0] |= num5;
      }

      internal void SetScanLine(int[] b)
      {
        Array.Copy((Array) b, 0, (Array) this.Scanline, 0, this.Scanline.Length);
      }

      internal int[] GetScanLineCopy(int[] b)
      {
        if (b == null || b.Length < this.Scanline.Length)
          b = new int[this.Scanline.Length];
        Array.Copy((Array) this.Scanline, 0, (Array) b, 0, this.Scanline.Length);
        return b;
      }

      public ImageLine unpackToNewImageLine()
      {
        ImageLine newImageLine = new ImageLine(this.ImgInfo, this.SampleType, true);
        if (this.SampleType == ImageLine.ESampleType.INT)
          ImageLine.unpackInplaceInt(this.ImgInfo, this.Scanline, newImageLine.Scanline, false);
        else
          ImageLine.unpackInplaceByte(this.ImgInfo, this.ScanlineB, newImageLine.ScanlineB, false);
        return newImageLine;
      }

      public ImageLine packToNewImageLine()
      {
        ImageLine newImageLine = new ImageLine(this.ImgInfo, this.SampleType, false);
        if (this.SampleType == ImageLine.ESampleType.INT)
          ImageLine.packInplaceInt(this.ImgInfo, this.Scanline, newImageLine.Scanline, false);
        else
          ImageLine.packInplaceByte(this.ImgInfo, this.ScanlineB, newImageLine.ScanlineB, false);
        return newImageLine;
      }

      public int[] GetScanlineInt() => this.Scanline;

      public byte[] GetScanlineByte() => this.ScanlineB;

      public bool IsInt() => this.SampleType == ImageLine.ESampleType.INT;

      public bool IsByte() => this.SampleType == ImageLine.ESampleType.BYTE;

      public override string ToString()
      {
        return $"row={(object) this.Rown} cols={(object) this.ImgInfo.Cols} bpc={(object) this.ImgInfo.BitDepth} size={(object) this.Scanline.Length}";
      }

      internal static int GetMaskForPackedFormats(int bitDepth)
      {
        switch (bitDepth)
        {
          case 1:
            return 128 /*0x80*/;
          case 2:
            return 192 /*0xC0*/;
          case 4:
            return 240 /*0xF0*/;
          default:
            return (int) byte.MaxValue;
        }
      }

      internal static int GetMaskForPackedFormatsLs(int bitDepth)
      {
        switch (bitDepth)
        {
          case 1:
            return 1;
          case 2:
            return 3;
          case 4:
            return 15;
          default:
            return (int) byte.MaxValue;
        }
      }

      public enum ESampleType
      {
        INT,
        BYTE,
      }
    }
}
