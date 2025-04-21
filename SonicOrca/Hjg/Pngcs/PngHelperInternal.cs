// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.PngHelperInternal
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Hjg.Pngcs.Zlib;
using System;
using System.IO;
using System.Text;

namespace Hjg.Pngcs
{

    public class PngHelperInternal
    {
      [ThreadStatic]
      private static CRC32 crc32Engine = (CRC32) null;
      public static readonly byte[] PNG_ID_SIGNATURE = new byte[8]
      {
        (byte) 137,
        (byte) 80 /*0x50*/,
        (byte) 78,
        (byte) 71,
        (byte) 13,
        (byte) 10,
        (byte) 26,
        (byte) 10
      };
      public static Encoding charsetLatin1 = Encoding.GetEncoding("ISO-8859-1");
      public static Encoding charsetUtf8 = Encoding.GetEncoding("UTF-8");
      public static bool DEBUG = false;

      public static CRC32 GetCRC()
      {
        if (PngHelperInternal.crc32Engine == null)
          PngHelperInternal.crc32Engine = new CRC32();
        return PngHelperInternal.crc32Engine;
      }

      public static int DoubleToInt100000(double d) => (int) (d * 100000.0 + 0.5);

      public static double IntToDouble100000(int i) => (double) i / 100000.0;

      public static void WriteInt2(Stream os, int n)
      {
        byte[] b = new byte[2]
        {
          (byte) (n >> 8 & (int) byte.MaxValue),
          (byte) (n & (int) byte.MaxValue)
        };
        PngHelperInternal.WriteBytes(os, b);
      }

      public static int ReadInt2(Stream mask0)
      {
        try
        {
          int num1 = mask0.ReadByte();
          int num2 = mask0.ReadByte();
          return num1 == -1 || num2 == -1 ? -1 : (num1 << 8) + num2;
        }
        catch (IOException ex)
        {
          throw new PngjInputException("error reading readInt2", (Exception) ex);
        }
      }

      public static int ReadInt4(Stream mask0)
      {
        try
        {
          int num1 = mask0.ReadByte();
          int num2 = mask0.ReadByte();
          int num3 = mask0.ReadByte();
          int num4 = mask0.ReadByte();
          return num1 == -1 || num2 == -1 || num3 == -1 || num4 == -1 ? -1 : (num1 << 24) + (num2 << 16 /*0x10*/) + (num3 << 8) + num4;
        }
        catch (IOException ex)
        {
          throw new PngjInputException("error reading readInt4", (Exception) ex);
        }
      }

      public static int ReadInt1fromByte(byte[] b, int offset) => (int) b[offset] & (int) byte.MaxValue;

      public static int ReadInt2fromBytes(byte[] b, int offset)
      {
        return ((int) b[offset] & (int) byte.MaxValue) << 16 /*0x10*/ | (int) b[offset + 1] & (int) byte.MaxValue;
      }

      public static int ReadInt4fromBytes(byte[] b, int offset)
      {
        return ((int) b[offset] & (int) byte.MaxValue) << 24 | ((int) b[offset + 1] & (int) byte.MaxValue) << 16 /*0x10*/ | ((int) b[offset + 2] & (int) byte.MaxValue) << 8 | (int) b[offset + 3] & (int) byte.MaxValue;
      }

      public static void WriteInt2tobytes(int n, byte[] b, int offset)
      {
        b[offset] = (byte) (n >> 8 & (int) byte.MaxValue);
        b[offset + 1] = (byte) (n & (int) byte.MaxValue);
      }

      public static void WriteInt4tobytes(int n, byte[] b, int offset)
      {
        b[offset] = (byte) (n >> 24 & (int) byte.MaxValue);
        b[offset + 1] = (byte) (n >> 16 /*0x10*/ & (int) byte.MaxValue);
        b[offset + 2] = (byte) (n >> 8 & (int) byte.MaxValue);
        b[offset + 3] = (byte) (n & (int) byte.MaxValue);
      }

      public static void WriteInt4(Stream os, int n)
      {
        byte[] b = new byte[4];
        PngHelperInternal.WriteInt4tobytes(n, b, 0);
        PngHelperInternal.WriteBytes(os, b);
      }

      public static void ReadBytes(Stream mask0, byte[] b, int offset, int len)
      {
        if (len == 0)
          return;
        try
        {
          int num;
          for (int index = 0; index < len; index += num)
          {
            num = mask0.Read(b, offset + index, len - index);
            if (num < 1)
              throw new Exception($"error reading, {(object) num} !={(object) len}");
          }
        }
        catch (IOException ex)
        {
          throw new PngjInputException("error reading", (Exception) ex);
        }
      }

      public static void SkipBytes(Stream ist, int len)
      {
        byte[] buffer = new byte[32768 /*0x8000*/];
        int num1 = len;
        try
        {
          int num2;
          for (; num1 > 0; num1 -= num2)
          {
            num2 = ist.Read(buffer, 0, num1 > buffer.Length ? buffer.Length : num1);
            if (num2 < 0)
              throw new PngjInputException("error reading (skipping) : EOF");
          }
        }
        catch (IOException ex)
        {
          throw new PngjInputException("error reading (skipping)", (Exception) ex);
        }
      }

      public static void WriteBytes(Stream os, byte[] b)
      {
        try
        {
          os.Write(b, 0, b.Length);
        }
        catch (IOException ex)
        {
          throw new PngjOutputException((Exception) ex);
        }
      }

      public static void WriteBytes(Stream os, byte[] b, int offset, int n)
      {
        try
        {
          os.Write(b, offset, n);
        }
        catch (IOException ex)
        {
          throw new PngjOutputException((Exception) ex);
        }
      }

      public static int ReadByte(Stream mask0)
      {
        try
        {
          return mask0.ReadByte();
        }
        catch (IOException ex)
        {
          throw new PngjOutputException((Exception) ex);
        }
      }

      public static void WriteByte(Stream os, byte b)
      {
        try
        {
          os.WriteByte(b);
        }
        catch (IOException ex)
        {
          throw new PngjOutputException((Exception) ex);
        }
      }

      public static int UnfilterRowPaeth(int r, int a, int b, int c)
      {
        return r + PngHelperInternal.FilterPaethPredictor(a, b, c) & (int) byte.MaxValue;
      }

      public static int FilterPaethPredictor(int a, int b, int c)
      {
        int num1 = a + b - c;
        int num2 = num1 >= a ? num1 - a : a - num1;
        int num3 = num1 >= b ? num1 - b : b - num1;
        int num4 = num1 >= c ? num1 - c : c - num1;
        if (num2 <= num3 && num2 <= num4)
          return a;
        return num3 <= num4 ? b : c;
      }

      public static void Logdebug(string msg)
      {
        if (!PngHelperInternal.DEBUG)
          return;
        Console.Out.WriteLine(msg);
      }

      public static void InitCrcForTests(PngReader pngr) => pngr.InitCrctest();

      public static long GetCrctestVal(PngReader pngr) => pngr.GetCrctestVal();
    }
}
