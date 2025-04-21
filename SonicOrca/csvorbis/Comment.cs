// Decompiled with JetBrains decompiler
// Type: csvorbis.Comment
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;
using System;
using System.Text;

namespace csvorbis
{

    public class Comment
    {
      private static string _vorbis = "vorbis";
      private static int OV_EIMPL = -130;
      public byte[][] user_comments;
      public int[] comment_lengths;
      public int comments;
      public byte[] vendor;

      public void init()
      {
        this.user_comments = (byte[][]) null;
        this.comments = 0;
        this.vendor = (byte[]) null;
      }

      public void add(string comment) => this.add(Encoding.UTF8.GetBytes(comment));

      private void add(byte[] comment)
      {
        byte[][] destinationArray1 = new byte[this.comments + 2][];
        if (this.user_comments != null)
          Array.Copy((Array) this.user_comments, 0, (Array) destinationArray1, 0, this.comments);
        this.user_comments = destinationArray1;
        int[] destinationArray2 = new int[this.comments + 2];
        if (this.comment_lengths != null)
          Array.Copy((Array) this.comment_lengths, 0, (Array) destinationArray2, 0, this.comments);
        this.comment_lengths = destinationArray2;
        byte[] destinationArray3 = new byte[comment.Length + 1];
        Array.Copy((Array) comment, 0, (Array) destinationArray3, 0, comment.Length);
        this.user_comments[this.comments] = destinationArray3;
        this.comment_lengths[this.comments] = comment.Length;
        ++this.comments;
        this.user_comments[this.comments] = (byte[]) null;
      }

      public void add_tag(string tag, string contents)
      {
        if (contents == null)
          contents = "";
        this.add($"{tag}={contents}");
      }

      private static bool tagcompare(byte[] s1, byte[] s2, int n)
      {
        for (int index = 0; index < n; ++index)
        {
          byte num1 = s1[index];
          byte num2 = s2[index];
          if (num1 >= (byte) 65)
            num1 = (byte) ((int) num1 - 65 + 97);
          if (num2 >= (byte) 65)
            num2 = (byte) ((int) num2 - 65 + 97);
          if ((int) num1 != (int) num2)
            return false;
        }
        return true;
      }

      public string query(string tag) => this.query(tag, 0);

      public string query(string tag, int count)
      {
        Encoding utF8 = Encoding.UTF8;
        int index1 = this.query(utF8.GetBytes(tag), count);
        if (index1 == -1)
          return (string) null;
        byte[] userComment = this.user_comments[index1];
        for (int index2 = 0; index2 < this.comment_lengths[index1]; ++index2)
        {
          if (userComment[index2] == (byte) 61)
            return new string(utF8.GetChars(userComment), index2 + 1, this.comment_lengths[index1] - (index2 + 1));
        }
        return (string) null;
      }

      private int query(byte[] tag, int count)
      {
        int num = 0;
        int length = tag.Length;
        byte[] numArray = new byte[length + 2];
        Array.Copy((Array) tag, 0, (Array) numArray, 0, tag.Length);
        numArray[tag.Length] = (byte) 61;
        for (int index = 0; index < this.comments; ++index)
        {
          if (Comment.tagcompare(this.user_comments[index], numArray, length))
          {
            if (count == num)
              return index;
            ++num;
          }
        }
        return -1;
      }

      internal int unpack(csBuffer opb)
      {
        int bytes1 = opb.read(32 /*0x20*/);
        if (bytes1 < 0)
        {
          this.clear();
          return -1;
        }
        this.vendor = new byte[bytes1 + 1];
        opb.read(this.vendor, bytes1);
        this.comments = opb.read(32 /*0x20*/);
        if (this.comments < 0)
        {
          this.clear();
          return -1;
        }
        this.user_comments = new byte[this.comments + 1][];
        this.comment_lengths = new int[this.comments + 1];
        for (int index = 0; index < this.comments; ++index)
        {
          int bytes2 = opb.read(32 /*0x20*/);
          if (bytes2 < 0)
          {
            this.clear();
            return -1;
          }
          this.comment_lengths[index] = bytes2;
          this.user_comments[index] = new byte[bytes2 + 1];
          opb.read(this.user_comments[index], bytes2);
        }
        if (opb.read(1) == 1)
          return 0;
        this.clear();
        return -1;
      }

      private int pack(csBuffer opb)
      {
        string s = "Xiphophorus libVorbis I 20000508";
        Encoding utF8 = Encoding.UTF8;
        byte[] bytes1 = utF8.GetBytes(s);
        byte[] bytes2 = utF8.GetBytes(Comment._vorbis);
        opb.write(3, 8);
        opb.write(bytes2);
        opb.write(s.Length, 32 /*0x20*/);
        opb.write(bytes1);
        opb.write(this.comments, 32 /*0x20*/);
        if (this.comments != 0)
        {
          for (int index = 0; index < this.comments; ++index)
          {
            if (this.user_comments[index] != null)
            {
              opb.write(this.comment_lengths[index], 32 /*0x20*/);
              opb.write(this.user_comments[index]);
            }
            else
              opb.write(0, 32 /*0x20*/);
          }
        }
        opb.write(1, 1);
        return 0;
      }

      public int header_out(Packet op)
      {
        csBuffer opb = new csBuffer();
        opb.writeinit();
        if (this.pack(opb) != 0)
          return Comment.OV_EIMPL;
        op.packet_base = new byte[opb.bytes()];
        op.packet = 0;
        op.bytes = opb.bytes();
        Array.Copy((Array) opb.buf(), 0, (Array) op.packet_base, 0, op.bytes);
        op.b_o_s = 0;
        op.e_o_s = 0;
        op.granulepos = 0L;
        return 0;
      }

      internal void clear()
      {
        for (int index = 0; index < this.comments; ++index)
          this.user_comments[index] = (byte[]) null;
        this.user_comments = (byte[][]) null;
        this.vendor = (byte[]) null;
      }

      public string getVendor() => new string(Encoding.UTF8.GetChars(this.vendor));

      public string getComment(int i)
      {
        Encoding utF8 = Encoding.UTF8;
        return this.comments <= i ? (string) null : new string(utF8.GetChars(this.user_comments[i]));
      }

      public string toString()
      {
        Encoding utF8 = Encoding.UTF8;
        string str = "Vendor: " + new string(utF8.GetChars(this.vendor));
        for (int index = 0; index < this.comments; ++index)
          str = $"{str}\nComment: {new string(utF8.GetChars(this.user_comments[index]))}";
        return str + "\n";
      }
    }
}
