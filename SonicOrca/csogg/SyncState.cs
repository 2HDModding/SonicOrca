// Decompiled with JetBrains decompiler
// Type: csogg.SyncState
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace csogg
{

    public class SyncState
    {
      public byte[] data;
      private int storage;
      private int fill;
      private int returned;
      private int unsynced;
      private int headerbytes;
      private int bodybytes;
      private Page pageseek_p = new Page();
      private byte[] chksum = new byte[4];

      public int clear()
      {
        this.data = (byte[]) null;
        return 0;
      }

      public int buffer(int size)
      {
        if (this.returned != 0)
        {
          this.fill -= this.returned;
          if (this.fill > 0)
            Array.Copy((Array) this.data, this.returned, (Array) this.data, 0, this.fill);
          this.returned = 0;
        }
        if (size > this.storage - this.fill)
        {
          int length = size + this.fill + 4096 /*0x1000*/;
          if (this.data != null)
          {
            byte[] destinationArray = new byte[length];
            Array.Copy((Array) this.data, 0, (Array) destinationArray, 0, this.data.Length);
            this.data = destinationArray;
          }
          else
            this.data = new byte[length];
          this.storage = length;
        }
        return this.fill;
      }

      public int wrote(int bytes)
      {
        if (this.fill + bytes > this.storage)
          return -1;
        this.fill += bytes;
        return 0;
      }

      public int pageseek(Page og)
      {
        int returned1 = this.returned;
        int num1 = this.fill - this.returned;
        if (this.headerbytes == 0)
        {
          if (num1 < 27)
            return 0;
          if (this.data[returned1] != (byte) 79 || this.data[returned1 + 1] != (byte) 103 || this.data[returned1 + 2] != (byte) 103 || this.data[returned1 + 3] != (byte) 83)
          {
            this.headerbytes = 0;
            this.bodybytes = 0;
            int num2 = 0;
            for (int index = 0; index < num1 - 1; ++index)
            {
              if (this.data[returned1 + 1 + index] == (byte) 79)
              {
                num2 = returned1 + 1 + index;
                break;
              }
            }
            if (num2 == 0)
              num2 = this.fill;
            this.returned = num2;
            return -(num2 - returned1);
          }
          int num3 = ((int) this.data[returned1 + 26] & (int) byte.MaxValue) + 27;
          if (num1 < num3)
            return 0;
          for (int index = 0; index < ((int) this.data[returned1 + 26] & (int) byte.MaxValue); ++index)
            this.bodybytes += (int) this.data[returned1 + 27 + index] & (int) byte.MaxValue;
          this.headerbytes = num3;
        }
        if (this.bodybytes + this.headerbytes > num1)
          return 0;
        lock (this.chksum)
        {
          Array.Copy((Array) this.data, returned1 + 22, (Array) this.chksum, 0, 4);
          this.data[returned1 + 22] = (byte) 0;
          this.data[returned1 + 23] = (byte) 0;
          this.data[returned1 + 24] = (byte) 0;
          this.data[returned1 + 25] = (byte) 0;
          Page pageseekP = this.pageseek_p;
          pageseekP.header_base = this.data;
          pageseekP.header = returned1;
          pageseekP.header_len = this.headerbytes;
          pageseekP.body_base = this.data;
          pageseekP.body = returned1 + this.headerbytes;
          pageseekP.body_len = this.bodybytes;
          pageseekP.checksum();
          if ((int) this.chksum[0] == (int) this.data[returned1 + 22] && (int) this.chksum[1] == (int) this.data[returned1 + 23] && (int) this.chksum[2] == (int) this.data[returned1 + 24])
          {
            if ((int) this.chksum[3] == (int) this.data[returned1 + 25])
              goto label_34;
          }
          Array.Copy((Array) this.chksum, 0, (Array) this.data, returned1 + 22, 4);
          this.headerbytes = 0;
          this.bodybytes = 0;
          int num4 = 0;
          for (int index = 0; index < num1 - 1; ++index)
          {
            if (this.data[returned1 + 1 + index] == (byte) 79)
            {
              num4 = returned1 + 1 + index;
              break;
            }
          }
          if (num4 == 0)
            num4 = this.fill;
          this.returned = num4;
          return -(num4 - returned1);
        }
    label_34:
        int returned2 = this.returned;
        if (og != null)
        {
          og.header_base = this.data;
          og.header = returned2;
          og.header_len = this.headerbytes;
          og.body_base = this.data;
          og.body = returned2 + this.headerbytes;
          og.body_len = this.bodybytes;
        }
        this.unsynced = 0;
        int num5;
        this.returned += num5 = this.headerbytes + this.bodybytes;
        this.headerbytes = 0;
        this.bodybytes = 0;
        return num5;
      }

      public int pageout(Page og)
      {
        do
        {
          int num = this.pageseek(og);
          if (num > 0)
            return 1;
          if (num == 0)
            return 0;
        }
        while (this.unsynced != 0);
        this.unsynced = 1;
        return -1;
      }

      public int reset()
      {
        this.fill = 0;
        this.returned = 0;
        this.unsynced = 0;
        this.headerbytes = 0;
        this.bodybytes = 0;
        return 0;
      }

      public void init()
      {
      }
    }
}
