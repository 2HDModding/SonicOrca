// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Zlib.CRC32
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Zlib
{

    public class CRC32
    {
      private const uint defaultPolynomial = 3988292384;
      private const uint defaultSeed = 4294967295 /*0xFFFFFFFF*/;
      private static uint[] defaultTable;
      private uint hash;
      private uint seed;
      private uint[] table;

      public CRC32()
        : this(3988292384U, uint.MaxValue)
      {
      }

      public CRC32(uint polynomial, uint seed)
      {
        this.table = CRC32.InitializeTable(polynomial);
        this.seed = seed;
        this.hash = seed;
      }

      public void Update(byte[] buffer) => this.Update(buffer, 0, buffer.Length);

      public void Update(byte[] buffer, int start, int length)
      {
        int num = 0;
        int index = start;
        while (num < length)
        {
          this.hash = this.hash >> 8 ^ this.table[(int) buffer[index] ^ (int) this.hash & (int) byte.MaxValue];
          ++num;
          ++index;
        }
      }

      public uint GetValue() => ~this.hash;

      public void Reset() => this.hash = this.seed;

      private static uint[] InitializeTable(uint polynomial)
      {
        if (polynomial == 3988292384U && CRC32.defaultTable != null)
          return CRC32.defaultTable;
        uint[] numArray = new uint[256 /*0x0100*/];
        for (int index1 = 0; index1 < 256 /*0x0100*/; ++index1)
        {
          uint num = (uint) index1;
          for (int index2 = 0; index2 < 8; ++index2)
          {
            if (((int) num & 1) == 1)
              num = num >> 1 ^ polynomial;
            else
              num >>= 1;
          }
          numArray[index1] = num;
        }
        if (polynomial == 3988292384U)
          CRC32.defaultTable = numArray;
        return numArray;
      }
    }
}
