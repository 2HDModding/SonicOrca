// Decompiled with JetBrains decompiler
// Type: SonicOrca.Original.Kosinski
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Collections.Generic;
using System.IO;

namespace SonicOrca.Original
{

    public static class Kosinski
    {
      public static byte[] Decompress(byte[] input)
      {
        using (MemoryStream input1 = new MemoryStream(input))
          return Kosinski.Decompress((Stream) input1);
      }

      public static byte[] Decompress(Stream input)
      {
        using (MemoryStream output = new MemoryStream())
        {
          Kosinski.Decompress(input, (Stream) output);
          return output.ToArray();
        }
      }

      public static int Decompress(Stream input, Stream output)
      {
        BitReader bitReader = new BitReader(input, 2);
        BinaryReader binaryReader = new BinaryReader(input);
        List<byte> byteList = new List<byte>();
        while (true)
        {
          while (!bitReader.ReadBit())
          {
            int num1;
            int num2;
            if (bitReader.ReadBit())
            {
              int num3 = (int) binaryReader.ReadByte();
              int num4 = (int) binaryReader.ReadByte();
              int num5 = num4 & 7;
              if (num5 == 0)
              {
                num1 = (int) binaryReader.ReadByte();
                switch (num1)
                {
                  case 0:
                    return byteList.Count;
                  case 1:
                    continue;
                }
              }
              else
                num1 = num5 + 1;
              num2 = (int) (short) (57344 /*0xE000*/ | (num4 & 248) << 5 | num3);
            }
            else
            {
              num1 = ((bitReader.ReadBit() ? 1 : 0) << 1 | (bitReader.ReadBit() ? 1 : 0)) + 1;
              num2 = (int) (short) (65280 | (int) binaryReader.ReadByte());
            }
            int num6 = num1 + 1;
            int index1 = byteList.Count + num2;
            for (int index2 = 0; index2 < num6; ++index2)
            {
              output.WriteByte(byteList[index1]);
              byteList.Add(byteList[index1]);
              ++index1;
            }
          }
          byte num = binaryReader.ReadByte();
          output.WriteByte(num);
          byteList.Add(num);
        }
      }
    }
}
