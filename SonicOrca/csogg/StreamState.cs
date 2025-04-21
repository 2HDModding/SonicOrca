// Decompiled with JetBrains decompiler
// Type: csogg.StreamState
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Text;

namespace csogg
{

    public class StreamState
    {
      private byte[] body_data;
      private int body_storage;
      private int body_fill;
      private int body_returned;
      private int[] lacing_vals;
      private long[] granule_vals;
      private int lacing_storage;
      private int lacing_fill;
      private int lacing_packet;
      private int lacing_returned;
      private byte[] header = new byte[282];
      private int header_fill;
      public int e_o_s;
      private int b_o_s;
      private int serialno;
      private int pageno;
      private long packetno;
      private long granulepos;

      private StreamState(int serialno)
        : this()
      {
        this.init(serialno);
      }

      public StreamState() => this.init();

      private void init()
      {
        this.body_storage = 16384 /*0x4000*/;
        this.body_data = new byte[this.body_storage];
        this.lacing_storage = 1024 /*0x0400*/;
        this.lacing_vals = new int[this.lacing_storage];
        this.granule_vals = new long[this.lacing_storage];
      }

      public void init(int serialno)
      {
        if (this.body_data == null)
        {
          this.init();
        }
        else
        {
          for (int index = 0; index < this.body_data.Length; ++index)
            this.body_data[index] = (byte) 0;
          for (int index = 0; index < this.lacing_vals.Length; ++index)
            this.lacing_vals[index] = 0;
          for (int index = 0; index < this.granule_vals.Length; ++index)
            this.granule_vals[index] = 0L;
        }
        this.serialno = serialno;
      }

      public void clear()
      {
        this.body_data = (byte[]) null;
        this.lacing_vals = (int[]) null;
        this.granule_vals = (long[]) null;
      }

      private void destroy() => this.clear();

      private void body_expand(int needed)
      {
        if (this.body_storage > this.body_fill + needed)
          return;
        this.body_storage += needed + 1024 /*0x0400*/;
        byte[] destinationArray = new byte[this.body_storage];
        Array.Copy((Array) this.body_data, 0, (Array) destinationArray, 0, this.body_data.Length);
        this.body_data = destinationArray;
      }

      private void lacing_expand(int needed)
      {
        if (this.lacing_storage > this.lacing_fill + needed)
          return;
        this.lacing_storage += needed + 32 /*0x20*/;
        int[] destinationArray1 = new int[this.lacing_storage];
        Array.Copy((Array) this.lacing_vals, 0, (Array) destinationArray1, 0, this.lacing_vals.Length);
        this.lacing_vals = destinationArray1;
        long[] destinationArray2 = new long[this.lacing_storage];
        Array.Copy((Array) this.granule_vals, 0, (Array) destinationArray2, 0, this.granule_vals.Length);
        this.granule_vals = destinationArray2;
      }

      public int packetin(Packet op)
      {
        int needed = op.bytes / (int) byte.MaxValue + 1;
        if (this.body_returned != 0)
        {
          this.body_fill -= this.body_returned;
          if (this.body_fill != 0)
            Array.Copy((Array) this.body_data, this.body_returned, (Array) this.body_data, 0, this.body_fill);
          this.body_returned = 0;
        }
        this.body_expand(op.bytes);
        this.lacing_expand(needed);
        Array.Copy((Array) op.packet_base, op.packet, (Array) this.body_data, this.body_fill, op.bytes);
        this.body_fill += op.bytes;
        int num;
        for (num = 0; num < needed - 1; ++num)
        {
          this.lacing_vals[this.lacing_fill + num] = (int) byte.MaxValue;
          this.granule_vals[this.lacing_fill + num] = this.granulepos;
        }
        this.lacing_vals[this.lacing_fill + num] = op.bytes % (int) byte.MaxValue;
        this.granulepos = this.granule_vals[this.lacing_fill + num] = op.granulepos;
        this.lacing_vals[this.lacing_fill] |= 256 /*0x0100*/;
        this.lacing_fill += needed;
        ++this.packetno;
        if (op.e_o_s != 0)
          this.e_o_s = 1;
        return 0;
      }

      public int packetout(Packet op)
      {
        int lacingReturned = this.lacing_returned;
        if (this.lacing_packet <= lacingReturned)
          return 0;
        if ((this.lacing_vals[lacingReturned] & 1024 /*0x0400*/) != 0)
        {
          ++this.lacing_returned;
          ++this.packetno;
          return -1;
        }
        int num1 = this.lacing_vals[lacingReturned] & (int) byte.MaxValue;
        int num2 = 0;
        op.packet_base = this.body_data;
        op.packet = this.body_returned;
        op.e_o_s = this.lacing_vals[lacingReturned] & 512 /*0x0200*/;
        op.b_o_s = this.lacing_vals[lacingReturned] & 256 /*0x0100*/;
        int num3 = num2 + num1;
        while (num1 == (int) byte.MaxValue)
        {
          int lacingVal = this.lacing_vals[++lacingReturned];
          num1 = lacingVal & (int) byte.MaxValue;
          if ((lacingVal & 512 /*0x0200*/) != 0)
            op.e_o_s = 512 /*0x0200*/;
          num3 += num1;
        }
        op.packetno = this.packetno;
        op.granulepos = this.granule_vals[lacingReturned];
        op.bytes = num3;
        this.body_returned += num3;
        this.lacing_returned = lacingReturned + 1;
        ++this.packetno;
        return 1;
      }

      public int pagein(Page og)
      {
        byte[] headerBase = og.header_base;
        int header = og.header;
        byte[] bodyBase = og.body_base;
        int body = og.body;
        int bodyLen = og.body_len;
        int num1 = 0;
        int num2 = og.version();
        int num3 = og.continued();
        int num4 = og.bos();
        int num5 = og.eos();
        long num6 = og.granulepos();
        int num7 = og.serialno();
        int num8 = og.pageno();
        int num9 = (int) headerBase[header + 26] & (int) byte.MaxValue;
        int lacingReturned = this.lacing_returned;
        int bodyReturned = this.body_returned;
        if (bodyReturned != 0)
        {
          this.body_fill -= bodyReturned;
          if (this.body_fill != 0)
            Array.Copy((Array) this.body_data, bodyReturned, (Array) this.body_data, 0, this.body_fill);
          this.body_returned = 0;
        }
        if (lacingReturned != 0)
        {
          if (this.lacing_fill - lacingReturned != 0)
          {
            Array.Copy((Array) this.lacing_vals, lacingReturned, (Array) this.lacing_vals, 0, this.lacing_fill - lacingReturned);
            Array.Copy((Array) this.granule_vals, lacingReturned, (Array) this.granule_vals, 0, this.lacing_fill - lacingReturned);
          }
          this.lacing_fill -= lacingReturned;
          this.lacing_packet -= lacingReturned;
          this.lacing_returned = 0;
        }
        if (num7 != this.serialno || num2 > 0)
          return -1;
        this.lacing_expand(num9 + 1);
        if (num8 != this.pageno)
        {
          for (int lacingPacket = this.lacing_packet; lacingPacket < this.lacing_fill; ++lacingPacket)
            this.body_fill -= this.lacing_vals[lacingPacket] & (int) byte.MaxValue;
          this.lacing_fill = this.lacing_packet;
          if (this.pageno != -1)
          {
            this.lacing_vals[this.lacing_fill++] = 1024 /*0x0400*/;
            ++this.lacing_packet;
          }
          if (num3 != 0)
          {
            num4 = 0;
            for (; num1 < num9; ++num1)
            {
              int num10 = (int) headerBase[header + 27 + num1] & (int) byte.MaxValue;
              body += num10;
              bodyLen -= num10;
              if (num10 < (int) byte.MaxValue)
              {
                ++num1;
                break;
              }
            }
          }
        }
        if (bodyLen != 0)
        {
          this.body_expand(bodyLen);
          Array.Copy((Array) bodyBase, body, (Array) this.body_data, this.body_fill, bodyLen);
          this.body_fill += bodyLen;
        }
        int index = -1;
        while (num1 < num9)
        {
          int num11 = (int) headerBase[header + 27 + num1] & (int) byte.MaxValue;
          this.lacing_vals[this.lacing_fill] = num11;
          this.granule_vals[this.lacing_fill] = -1L;
          if (num4 != 0)
          {
            this.lacing_vals[this.lacing_fill] |= 256 /*0x0100*/;
            num4 = 0;
          }
          if (num11 < (int) byte.MaxValue)
            index = this.lacing_fill;
          ++this.lacing_fill;
          ++num1;
          if (num11 < (int) byte.MaxValue)
            this.lacing_packet = this.lacing_fill;
        }
        if (index != -1)
          this.granule_vals[index] = num6;
        if (num5 != 0)
        {
          this.e_o_s = 1;
          if (this.lacing_fill > 0)
            this.lacing_vals[this.lacing_fill - 1] |= 512 /*0x0200*/;
        }
        this.pageno = num8 + 1;
        return 0;
      }

      public int flush(Page og)
      {
        int num1 = this.lacing_fill > (int) byte.MaxValue ? (int) byte.MaxValue : this.lacing_fill;
        int num2 = 0;
        int num3 = 0;
        long num4 = this.granule_vals[0];
        if (num1 == 0)
          return 0;
        int sourceIndex;
        if (this.b_o_s == 0)
        {
          num4 = 0L;
          for (sourceIndex = 0; sourceIndex < num1; ++sourceIndex)
          {
            if ((this.lacing_vals[sourceIndex] & (int) byte.MaxValue) < (int) byte.MaxValue)
            {
              ++sourceIndex;
              break;
            }
          }
        }
        else
        {
          for (sourceIndex = 0; sourceIndex < num1 && num3 <= 4096 /*0x1000*/; ++sourceIndex)
          {
            num3 += this.lacing_vals[sourceIndex] & (int) byte.MaxValue;
            num4 = this.granule_vals[sourceIndex];
          }
        }
        byte[] bytes = Encoding.UTF8.GetBytes("OggS");
        Array.Copy((Array) bytes, 0, (Array) this.header, 0, bytes.Length);
        this.header[4] = (byte) 0;
        this.header[5] = (byte) 0;
        if ((this.lacing_vals[0] & 256 /*0x0100*/) == 0)
          this.header[5] |= (byte) 1;
        if (this.b_o_s == 0)
          this.header[5] |= (byte) 2;
        if (this.e_o_s != 0 && this.lacing_fill == sourceIndex)
          this.header[5] |= (byte) 4;
        this.b_o_s = 1;
        for (int index = 6; index < 14; ++index)
        {
          this.header[index] = (byte) num4;
          num4 >>= 8;
        }
        int serialno = this.serialno;
        for (int index = 14; index < 18; ++index)
        {
          this.header[index] = (byte) serialno;
          serialno >>= 8;
        }
        if (this.pageno == -1)
          this.pageno = 0;
        int num5 = this.pageno++;
        for (int index = 18; index < 22; ++index)
        {
          this.header[index] = (byte) num5;
          num5 >>= 8;
        }
        this.header[22] = (byte) 0;
        this.header[23] = (byte) 0;
        this.header[24] = (byte) 0;
        this.header[25] = (byte) 0;
        this.header[26] = (byte) sourceIndex;
        for (int index = 0; index < sourceIndex; ++index)
        {
          this.header[index + 27] = (byte) this.lacing_vals[index];
          num2 += (int) this.header[index + 27] & (int) byte.MaxValue;
        }
        og.header_base = this.header;
        og.header = 0;
        og.header_len = this.header_fill = sourceIndex + 27;
        og.body_base = this.body_data;
        og.body = this.body_returned;
        og.body_len = num2;
        this.lacing_fill -= sourceIndex;
        Array.Copy((Array) this.lacing_vals, sourceIndex, (Array) this.lacing_vals, 0, this.lacing_fill * 4);
        Array.Copy((Array) this.granule_vals, sourceIndex, (Array) this.granule_vals, 0, this.lacing_fill * 8);
        this.body_returned += num2;
        og.checksum();
        return 1;
      }

      public int pageout(Page og)
      {
        return this.e_o_s != 0 && this.lacing_fill != 0 || this.body_fill - this.body_returned > 4096 /*0x1000*/ || this.lacing_fill >= (int) byte.MaxValue || this.lacing_fill != 0 && this.b_o_s == 0 ? this.flush(og) : 0;
      }

      public int eof() => this.e_o_s;

      public int reset()
      {
        this.body_fill = 0;
        this.body_returned = 0;
        this.lacing_fill = 0;
        this.lacing_packet = 0;
        this.lacing_returned = 0;
        this.header_fill = 0;
        this.e_o_s = 0;
        this.b_o_s = 0;
        this.pageno = -1;
        this.packetno = 0L;
        this.granulepos = 0L;
        return 0;
      }
    }
}
