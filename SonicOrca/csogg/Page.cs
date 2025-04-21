// Decompiled with JetBrains decompiler
// Type: csogg.Page
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace csogg
{

    public class Page
    {
      private static uint[] crc_lookup = new uint[256 /*0x0100*/];
      public byte[] header_base;
      public int header;
      public int header_len;
      public byte[] body_base;
      public int body;
      public int body_len;

      private static uint crc_entry(uint index)
      {
        uint num = index << 24;
        for (int index1 = 0; index1 < 8; ++index1)
        {
          if (((int) num & int.MinValue) != 0)
            num = (uint) ((int) num << 1 ^ 79764919);
          else
            num <<= 1;
        }
        return num & uint.MaxValue;
      }

      internal int version() => (int) this.header_base[this.header + 4] & (int) byte.MaxValue;

      internal int continued() => (int) this.header_base[this.header + 5] & 1;

      public int bos() => (int) this.header_base[this.header + 5] & 2;

      public int eos() => (int) this.header_base[this.header + 5] & 4;

      public long granulepos()
      {
        return (((((((long) ((int) this.header_base[this.header + 13] & (int) byte.MaxValue) << 8 | (long) ((uint) this.header_base[this.header + 12] & (uint) byte.MaxValue)) << 8 | (long) ((uint) this.header_base[this.header + 11] & (uint) byte.MaxValue)) << 8 | (long) ((uint) this.header_base[this.header + 10] & (uint) byte.MaxValue)) << 8 | (long) ((uint) this.header_base[this.header + 9] & (uint) byte.MaxValue)) << 8 | (long) ((uint) this.header_base[this.header + 8] & (uint) byte.MaxValue)) << 8 | (long) ((uint) this.header_base[this.header + 7] & (uint) byte.MaxValue)) << 8 | (long) ((uint) this.header_base[this.header + 6] & (uint) byte.MaxValue);
      }

      public int serialno()
      {
        return (int) this.header_base[this.header + 14] & (int) byte.MaxValue | ((int) this.header_base[this.header + 15] & (int) byte.MaxValue) << 8 | ((int) this.header_base[this.header + 16 /*0x10*/] & (int) byte.MaxValue) << 16 /*0x10*/ | ((int) this.header_base[this.header + 17] & (int) byte.MaxValue) << 24;
      }

      internal int pageno()
      {
        return (int) this.header_base[this.header + 18] & (int) byte.MaxValue | ((int) this.header_base[this.header + 19] & (int) byte.MaxValue) << 8 | ((int) this.header_base[this.header + 20] & (int) byte.MaxValue) << 16 /*0x10*/ | ((int) this.header_base[this.header + 21] & (int) byte.MaxValue) << 24;
      }

      internal void checksum()
      {
        uint num1 = 0;
        for (int index = 0; index < this.header_len; ++index)
        {
          uint num2 = (uint) this.header_base[this.header + index] & (uint) byte.MaxValue;
          uint num3 = num1 >> 24 & (uint) byte.MaxValue;
          num1 = num1 << 8 ^ Page.crc_lookup[(int) num2 ^ (int) num3];
        }
        for (int index = 0; index < this.body_len; ++index)
        {
          uint num4 = (uint) this.body_base[this.body + index] & (uint) byte.MaxValue;
          uint num5 = num1 >> 24 & (uint) byte.MaxValue;
          num1 = num1 << 8 ^ Page.crc_lookup[(int) num4 ^ (int) num5];
        }
        this.header_base[this.header + 22] = (byte) num1;
        this.header_base[this.header + 23] = (byte) (num1 >> 8);
        this.header_base[this.header + 24] = (byte) (num1 >> 16 /*0x10*/);
        this.header_base[this.header + 25] = (byte) (num1 >> 24);
      }

      public Page()
      {
        for (uint index = 0; (long) index < (long) Page.crc_lookup.Length; ++index)
          Page.crc_lookup[(int) index] = Page.crc_entry(index);
      }
    }
}
