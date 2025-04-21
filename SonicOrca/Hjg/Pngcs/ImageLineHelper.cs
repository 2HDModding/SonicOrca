// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.ImageLineHelper
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Hjg.Pngcs.Chunks;
using System;

namespace Hjg.Pngcs
{

    public class ImageLineHelper
    {
      public static int[] Palette2rgb(ImageLine line, PngChunkPLTE pal, PngChunkTRNS trns, int[] buf)
      {
        bool flag1 = trns != null;
        int num1 = flag1 ? 4 : 3;
        int length1 = line.ImgInfo.Cols * num1;
        if (buf == null || buf.Length < length1)
          buf = new int[length1];
        if (!line.SamplesUnpacked)
          line = line.unpackToNewImageLine();
        bool flag2 = line.SampleType == ImageLine.ESampleType.BYTE;
        int length2 = trns != null ? trns.GetPalletteAlpha().Length : 0;
        for (int index1 = 0; index1 < line.ImgInfo.Cols; ++index1)
        {
          int index2 = flag2 ? (int) line.ScanlineB[index1] & (int) byte.MaxValue : line.Scanline[index1];
          pal.GetEntryRgb(index2, buf, index1 * num1);
          if (flag1)
          {
            int num2 = index2 < length2 ? trns.GetPalletteAlpha()[index2] : (int) byte.MaxValue;
            buf[index1 * num1 + 3] = num2;
          }
        }
        return buf;
      }

      public static int[] Palette2rgb(ImageLine line, PngChunkPLTE pal, int[] buf)
      {
        return ImageLineHelper.Palette2rgb(line, pal, (PngChunkTRNS) null, buf);
      }

      public static int ToARGB8(int r, int g, int b)
      {
        return -16777216 /*0xFF000000*/ | r << 16 /*0x10*/ | g << 8 | b;
      }

      public static int ToARGB8(int r, int g, int b, int a) => a << 24 | r << 16 /*0x10*/ | g << 8 | b;

      public static int ToARGB8(int[] buff, int offset, bool alpha)
      {
        return !alpha ? ImageLineHelper.ToARGB8(buff[offset++], buff[offset++], buff[offset]) : ImageLineHelper.ToARGB8(buff[offset++], buff[offset++], buff[offset++], buff[offset]);
      }

      public static int ToARGB8(byte[] buff, int offset, bool alpha)
      {
        return !alpha ? ImageLineHelper.ToARGB8((int) buff[offset++], (int) buff[offset++], (int) buff[offset]) : ImageLineHelper.ToARGB8((int) buff[offset++], (int) buff[offset++], (int) buff[offset++], (int) buff[offset]);
      }

      public static void FromARGB8(int val, int[] buff, int offset, bool alpha)
      {
        buff[offset++] = val >> 16 /*0x10*/ & (int) byte.MaxValue;
        buff[offset++] = val >> 8 & (int) byte.MaxValue;
        buff[offset] = val & (int) byte.MaxValue;
        if (!alpha)
          return;
        buff[offset + 1] = val >> 24 & (int) byte.MaxValue;
      }

      public static void FromARGB8(int val, byte[] buff, int offset, bool alpha)
      {
        buff[offset++] = (byte) (val >> 16 /*0x10*/ & (int) byte.MaxValue);
        buff[offset++] = (byte) (val >> 8 & (int) byte.MaxValue);
        buff[offset] = (byte) (val & (int) byte.MaxValue);
        if (!alpha)
          return;
        buff[offset + 1] = (byte) (val >> 24 & (int) byte.MaxValue);
      }

      public static int GetPixelToARGB8(ImageLine line, int column)
      {
        return line.IsInt() ? ImageLineHelper.ToARGB8(line.Scanline, column * line.channels, line.ImgInfo.Alpha) : ImageLineHelper.ToARGB8(line.ScanlineB, column * line.channels, line.ImgInfo.Alpha);
      }

      public static void SetPixelFromARGB8(ImageLine line, int column, int argb)
      {
        if (line.IsInt())
          ImageLineHelper.FromARGB8(argb, line.Scanline, column * line.channels, line.ImgInfo.Alpha);
        else
          ImageLineHelper.FromARGB8(argb, line.ScanlineB, column * line.channels, line.ImgInfo.Alpha);
      }

      public static void SetPixel(ImageLine line, int col, int r, int g, int b, int a)
      {
        int num1 = col * line.channels;
        if (line.IsInt())
        {
          int[] scanline1 = line.Scanline;
          int index1 = num1;
          int num2 = index1 + 1;
          int num3 = r;
          scanline1[index1] = num3;
          int[] scanline2 = line.Scanline;
          int index2 = num2;
          int index3 = index2 + 1;
          int num4 = g;
          scanline2[index2] = num4;
          line.Scanline[index3] = b;
          if (!line.ImgInfo.Alpha)
            return;
          line.Scanline[index3 + 1] = a;
        }
        else
        {
          byte[] scanlineB1 = line.ScanlineB;
          int index4 = num1;
          int num5 = index4 + 1;
          int num6 = (int) (byte) r;
          scanlineB1[index4] = (byte) num6;
          byte[] scanlineB2 = line.ScanlineB;
          int index5 = num5;
          int index6 = index5 + 1;
          int num7 = (int) (byte) g;
          scanlineB2[index5] = (byte) num7;
          line.ScanlineB[index6] = (byte) b;
          if (!line.ImgInfo.Alpha)
            return;
          line.ScanlineB[index6 + 1] = (byte) a;
        }
      }

      public static void SetPixel(ImageLine line, int col, int r, int g, int b)
      {
        ImageLineHelper.SetPixel(line, col, r, g, b, line.maxSampleVal);
      }

      public static double ReadDouble(ImageLine line, int pos)
      {
        return line.IsInt() ? (double) line.Scanline[pos] / ((double) line.maxSampleVal + 0.9) : (double) line.ScanlineB[pos] / ((double) line.maxSampleVal + 0.9);
      }

      public static void WriteDouble(ImageLine line, double d, int pos)
      {
        if (line.IsInt())
          line.Scanline[pos] = (int) (d * ((double) line.maxSampleVal + 0.99));
        else
          line.ScanlineB[pos] = (byte) (d * ((double) line.maxSampleVal + 0.99));
      }

      public static int Interpol(int a, int b, int c, int d, double dx, double dy)
      {
        double num1 = (double) a * (1.0 - dx) + (double) b * dx;
        double num2 = (double) c * (1.0 - dx) + (double) d * dx;
        double num3 = 1.0 - dy;
        return (int) (num1 * num3 + num2 * dy + 0.5);
      }

      public static int ClampTo_0_255(int i)
      {
        if (i > (int) byte.MaxValue)
          return (int) byte.MaxValue;
        return i >= 0 ? i : 0;
      }

      public static double ClampDouble(double i)
      {
        if (i < 0.0)
          return 0.0;
        return i < 1.0 ? i : 0.999999;
      }

      public static int ClampTo_0_65535(int i)
      {
        if (i > (int) ushort.MaxValue)
          return (int) ushort.MaxValue;
        return i >= 0 ? i : 0;
      }

      public static int ClampTo_128_127(int x)
      {
        if (x > (int) sbyte.MaxValue)
          return (int) sbyte.MaxValue;
        return x >= (int) sbyte.MinValue ? x : (int) sbyte.MinValue;
      }

      public static int[] Unpack(ImageInfo imgInfo, int[] src, int[] dst, bool scale)
      {
        int samplesPerRow = imgInfo.SamplesPerRow;
        int samplesPerRowPacked = imgInfo.SamplesPerRowPacked;
        if (dst == null || dst.Length < samplesPerRow)
          dst = new int[samplesPerRow];
        if (imgInfo.Packed)
          ImageLine.unpackInplaceInt(imgInfo, src, dst, scale);
        else
          Array.Copy((Array) src, 0, (Array) dst, 0, samplesPerRowPacked);
        return dst;
      }

      public static byte[] Unpack(ImageInfo imgInfo, byte[] src, byte[] dst, bool scale)
      {
        int samplesPerRow = imgInfo.SamplesPerRow;
        int samplesPerRowPacked = imgInfo.SamplesPerRowPacked;
        if (dst == null || dst.Length < samplesPerRow)
          dst = new byte[samplesPerRow];
        if (imgInfo.Packed)
          ImageLine.unpackInplaceByte(imgInfo, src, dst, scale);
        else
          Array.Copy((Array) src, 0, (Array) dst, 0, samplesPerRowPacked);
        return dst;
      }

      public static int[] Pack(ImageInfo imgInfo, int[] src, int[] dst, bool scale)
      {
        int samplesPerRowPacked = imgInfo.SamplesPerRowPacked;
        if (dst == null || dst.Length < samplesPerRowPacked)
          dst = new int[samplesPerRowPacked];
        if (imgInfo.Packed)
          ImageLine.packInplaceInt(imgInfo, src, dst, scale);
        else
          Array.Copy((Array) src, 0, (Array) dst, 0, samplesPerRowPacked);
        return dst;
      }

      public static byte[] Pack(ImageInfo imgInfo, byte[] src, byte[] dst, bool scale)
      {
        int samplesPerRowPacked = imgInfo.SamplesPerRowPacked;
        if (dst == null || dst.Length < samplesPerRowPacked)
          dst = new byte[samplesPerRowPacked];
        if (imgInfo.Packed)
          ImageLine.packInplaceByte(imgInfo, src, dst, scale);
        else
          Array.Copy((Array) src, 0, (Array) dst, 0, samplesPerRowPacked);
        return dst;
      }
    }
}
