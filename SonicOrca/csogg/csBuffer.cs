// Decompiled with JetBrains decompiler
// Type: csogg.csBuffer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace csogg
{

    public class csBuffer
    {
      private static int BUFFER_INCREMENT = 256 /*0x0100*/;
      private static uint[] mask = new uint[33]
      {
        0U,
        1U,
        3U,
        7U,
        15U,
        31U /*0x1F*/,
        63U /*0x3F*/,
        (uint) sbyte.MaxValue,
        (uint) byte.MaxValue,
        511U /*0x01FF*/,
        1023U /*0x03FF*/,
        2047U /*0x07FF*/,
        4095U /*0x0FFF*/,
        8191U /*0x1FFF*/,
        16383U /*0x3FFF*/,
        (uint) short.MaxValue,
        (uint) ushort.MaxValue,
        131071U /*0x01FFFF*/,
        262143U /*0x03FFFF*/,
        524287U /*0x07FFFF*/,
        1048575U /*0x0FFFFF*/,
        2097151U /*0x1FFFFF*/,
        4194303U /*0x3FFFFF*/,
        8388607U /*0x7FFFFF*/,
        16777215U /*0xFFFFFF*/,
        33554431U /*0x01FFFFFF*/,
        67108863U /*0x03FFFFFF*/,
        134217727U /*0x07FFFFFF*/,
        268435455U /*0x0FFFFFFF*/,
        536870911U /*0x1FFFFFFF*/,
        1073741823U /*0x3FFFFFFF*/,
        (uint) int.MaxValue,
        uint.MaxValue
      };
      private int ptr;
      private byte[] buffer;
      private int endbit;
      private int endbyte;
      private int storage;

      public void writeinit()
      {
        this.buffer = new byte[csBuffer.BUFFER_INCREMENT];
        this.ptr = 0;
        this.buffer[0] = (byte) 0;
        this.storage = csBuffer.BUFFER_INCREMENT;
      }

      public void write(byte[] s)
      {
        for (int index = 0; index < s.Length && s[index] != (byte) 0; ++index)
          this.write((int) s[index], 8);
      }

      public void read(byte[] s, int bytes)
      {
        int num = 0;
        while (bytes-- != 0)
          s[num++] = (byte) this.read(8);
      }

      private void reset()
      {
        this.ptr = 0;
        this.buffer[0] = (byte) 0;
        this.endbit = this.endbyte = 0;
      }

      public void writeclear() => this.buffer = (byte[]) null;

      public void readinit(byte[] buf, int start, int bytes)
      {
        this.ptr = start;
        this.buffer = buf;
        this.endbit = this.endbyte = 0;
        this.storage = bytes;
      }

      public void write(int vvalue, int bits)
      {
        if (this.endbyte + 4 >= this.storage)
        {
          byte[] destinationArray = new byte[this.storage + csBuffer.BUFFER_INCREMENT];
          Array.Copy((Array) this.buffer, 0, (Array) destinationArray, 0, this.storage);
          this.buffer = destinationArray;
          this.storage += csBuffer.BUFFER_INCREMENT;
        }
        vvalue &= (int) csBuffer.mask[bits];
        bits += this.endbit;
        this.buffer[this.ptr] |= (byte) (vvalue << this.endbit);
        if (bits >= 8)
        {
          this.buffer[this.ptr + 1] = (byte) (vvalue >>> 8 - this.endbit);
          if (bits >= 16 /*0x10*/)
          {
            this.buffer[this.ptr + 2] = (byte) (vvalue >>> 16 /*0x10*/ - this.endbit);
            if (bits >= 24)
            {
              this.buffer[this.ptr + 3] = (byte) (vvalue >>> 24 - this.endbit);
              if (bits >= 32 /*0x20*/)
                this.buffer[this.ptr + 4] = this.endbit <= 0 ? (byte) 0 : (byte) (vvalue >>> 32 /*0x20*/ - this.endbit);
            }
          }
        }
        this.endbyte += bits / 8;
        this.ptr += bits / 8;
        this.endbit = bits & 7;
      }

      public int look(int bits)
      {
        uint num1 = csBuffer.mask[bits];
        bits += this.endbit;
        if (this.endbyte + 4 >= this.storage && this.endbyte + (bits - 1) / 8 >= this.storage)
          return -1;
        int num2 = ((int) this.buffer[this.ptr] & (int) byte.MaxValue) >> this.endbit;
        if (bits > 8)
        {
          num2 |= ((int) this.buffer[this.ptr + 1] & (int) byte.MaxValue) << 8 - this.endbit;
          if (bits > 16 /*0x10*/)
          {
            num2 |= ((int) this.buffer[this.ptr + 2] & (int) byte.MaxValue) << 16 /*0x10*/ - this.endbit;
            if (bits > 24)
            {
              num2 |= ((int) this.buffer[this.ptr + 3] & (int) byte.MaxValue) << 24 - this.endbit;
              if (bits > 32 /*0x20*/ && this.endbit != 0)
                num2 |= ((int) this.buffer[this.ptr + 4] & (int) byte.MaxValue) << 32 /*0x20*/ - this.endbit;
            }
          }
        }
        return (int) ((long) num1 & (long) num2);
      }

      public int look1()
      {
        return this.endbyte >= this.storage ? -1 : (int) this.buffer[this.ptr] >> this.endbit & 1;
      }

      public void adv(int bits)
      {
        bits += this.endbit;
        this.ptr += bits / 8;
        this.endbyte += bits / 8;
        this.endbit = bits & 7;
      }

      public void adv1()
      {
        ++this.endbit;
        if (this.endbit <= 7)
          return;
        this.endbit = 0;
        ++this.ptr;
        ++this.endbyte;
      }

      public int read(int bits)
      {
        uint num1 = csBuffer.mask[bits];
        bits += this.endbit;
        if (this.endbyte + 4 >= this.storage)
        {
          int num2 = -1;
          if (this.endbyte + (bits - 1) / 8 >= this.storage)
          {
            this.ptr += bits / 8;
            this.endbyte += bits / 8;
            this.endbit = bits & 7;
            return num2;
          }
        }
        int num3 = ((int) this.buffer[this.ptr] & (int) byte.MaxValue) >> this.endbit;
        if (bits > 8)
        {
          num3 |= ((int) this.buffer[this.ptr + 1] & (int) byte.MaxValue) << 8 - this.endbit;
          if (bits > 16 /*0x10*/)
          {
            num3 |= ((int) this.buffer[this.ptr + 2] & (int) byte.MaxValue) << 16 /*0x10*/ - this.endbit;
            if (bits > 24)
            {
              num3 |= ((int) this.buffer[this.ptr + 3] & (int) byte.MaxValue) << 24 - this.endbit;
              if (bits > 32 /*0x20*/ && this.endbit != 0)
                num3 |= ((int) this.buffer[this.ptr + 4] & (int) byte.MaxValue) << 32 /*0x20*/ - this.endbit;
            }
          }
        }
        int num4 = num3 & (int) num1;
        this.ptr += bits / 8;
        this.endbyte += bits / 8;
        this.endbit = bits & 7;
        return num4;
      }

      public int read1()
      {
        if (this.endbyte >= this.storage)
        {
          int num = -1;
          ++this.endbit;
          if (this.endbit <= 7)
            return num;
          this.endbit = 0;
          ++this.ptr;
          ++this.endbyte;
          return num;
        }
        int num1 = (int) this.buffer[this.ptr] >> this.endbit & 1;
        ++this.endbit;
        if (this.endbit <= 7)
          return num1;
        this.endbit = 0;
        ++this.ptr;
        ++this.endbyte;
        return num1;
      }

      public int bytes() => this.endbyte + (this.endbit + 7) / 8;

      public int bits() => this.endbyte * 8 + this.endbit;

      public static int ilog(int v)
      {
        int num = 0;
        for (; v > 0; v >>= 1)
          ++num;
        return num;
      }

      public byte[] buf() => this.buffer;
    }
}
