// Decompiled with JetBrains decompiler
// Type: csvorbis.StaticCodeBook
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;
using System;

namespace csvorbis
{

    internal class StaticCodeBook
    {
      internal int dim;
      internal int entries;
      internal int[] lengthlist;
      internal int maptype;
      internal int q_min;
      internal int q_delta;
      internal int q_quant;
      internal int q_sequencep;
      internal int[] quantlist;
      internal EncodeAuxNearestMatch nearest_tree;
      internal EncodeAuxThreshMatch thresh_tree;
      private static int VQ_FMAN = 21;
      private static int VQ_FEXP_BIAS = 768 /*0x0300*/;

      internal StaticCodeBook()
      {
      }

      internal StaticCodeBook(
        int dim,
        int entries,
        int[] lengthlist,
        int maptype,
        int q_min,
        int q_delta,
        int q_quant,
        int q_sequencep,
        int[] quantlist,
        object nearest_tree,
        object thresh_tree)
        : this()
      {
        this.dim = dim;
        this.entries = entries;
        this.lengthlist = lengthlist;
        this.maptype = maptype;
        this.q_min = q_min;
        this.q_delta = q_delta;
        this.q_quant = q_quant;
        this.q_sequencep = q_sequencep;
        this.quantlist = quantlist;
      }

      internal int pack(csBuffer opb)
      {
        bool flag = false;
        opb.write(5653314, 24);
        opb.write(this.dim, 16 /*0x10*/);
        opb.write(this.entries, 24);
        int index1 = 1;
        while (index1 < this.entries && this.lengthlist[index1] >= this.lengthlist[index1 - 1])
          ++index1;
        if (index1 == this.entries)
          flag = true;
        if (flag)
        {
          int num1 = 0;
          opb.write(1, 1);
          opb.write(this.lengthlist[0] - 1, 5);
          int index2;
          for (index2 = 1; index2 < this.entries; ++index2)
          {
            int num2 = this.lengthlist[index2];
            int num3 = this.lengthlist[index2 - 1];
            if (num2 > num3)
            {
              for (int index3 = num3; index3 < num2; ++index3)
              {
                opb.write(index2 - num1, StaticCodeBook.ilog(this.entries - num1));
                num1 = index2;
              }
            }
          }
          opb.write(index2 - num1, StaticCodeBook.ilog(this.entries - num1));
        }
        else
        {
          opb.write(0, 1);
          int index4 = 0;
          while (index4 < this.entries && this.lengthlist[index4] != 0)
            ++index4;
          if (index4 == this.entries)
          {
            opb.write(0, 1);
            for (int index5 = 0; index5 < this.entries; ++index5)
              opb.write(this.lengthlist[index5] - 1, 5);
          }
          else
          {
            opb.write(1, 1);
            for (int index6 = 0; index6 < this.entries; ++index6)
            {
              if (this.lengthlist[index6] == 0)
              {
                opb.write(0, 1);
              }
              else
              {
                opb.write(1, 1);
                opb.write(this.lengthlist[index6] - 1, 5);
              }
            }
          }
        }
        opb.write(this.maptype, 4);
        switch (this.maptype)
        {
          case 0:
            return 0;
          case 1:
          case 2:
            if (this.quantlist == null)
              return -1;
            opb.write(this.q_min, 32 /*0x20*/);
            opb.write(this.q_delta, 32 /*0x20*/);
            opb.write(this.q_quant - 1, 4);
            opb.write(this.q_sequencep, 1);
            int num = 0;
            switch (this.maptype)
            {
              case 1:
                num = this.maptype1_quantvals();
                break;
              case 2:
                num = this.entries * this.dim;
                break;
            }
            for (int index7 = 0; index7 < num; ++index7)
              opb.write(Math.Abs(this.quantlist[index7]), this.q_quant);
            goto case 0;
          default:
            return -1;
        }
      }

      internal int unpack(csBuffer opb)
      {
        if (opb.read(24) != 5653314)
        {
          this.clear();
          return -1;
        }
        this.dim = opb.read(16 /*0x10*/);
        this.entries = opb.read(24);
        if (this.entries == -1)
        {
          this.clear();
          return -1;
        }
        switch (opb.read(1))
        {
          case 0:
            this.lengthlist = new int[this.entries];
            if (opb.read(1) != 0)
            {
              for (int index = 0; index < this.entries; ++index)
              {
                if (opb.read(1) != 0)
                {
                  int num = opb.read(5);
                  if (num == -1)
                  {
                    this.clear();
                    return -1;
                  }
                  this.lengthlist[index] = num + 1;
                }
                else
                  this.lengthlist[index] = 0;
              }
              break;
            }
            for (int index = 0; index < this.entries; ++index)
            {
              int num = opb.read(5);
              if (num == -1)
              {
                this.clear();
                return -1;
              }
              this.lengthlist[index] = num + 1;
            }
            break;
          case 1:
            int num1 = opb.read(5) + 1;
            this.lengthlist = new int[this.entries];
            int index1 = 0;
            while (index1 < this.entries)
            {
              int num2 = opb.read(StaticCodeBook.ilog(this.entries - index1));
              if (num2 == -1)
              {
                this.clear();
                return -1;
              }
              int num3 = 0;
              while (num3 < num2)
              {
                this.lengthlist[index1] = num1;
                ++num3;
                ++index1;
              }
              ++num1;
            }
            break;
          default:
            return -1;
        }
        switch (this.maptype = opb.read(4))
        {
          case 0:
            return 0;
          case 1:
          case 2:
            this.q_min = opb.read(32 /*0x20*/);
            this.q_delta = opb.read(32 /*0x20*/);
            this.q_quant = opb.read(4) + 1;
            this.q_sequencep = opb.read(1);
            int length = 0;
            switch (this.maptype)
            {
              case 1:
                length = this.maptype1_quantvals();
                break;
              case 2:
                length = this.entries * this.dim;
                break;
            }
            this.quantlist = new int[length];
            for (int index2 = 0; index2 < length; ++index2)
              this.quantlist[index2] = opb.read(this.q_quant);
            if (this.quantlist[length - 1] == -1)
            {
              this.clear();
              return -1;
            }
            goto case 0;
          default:
            this.clear();
            return -1;
        }
      }

      internal int maptype1_quantvals()
      {
        int num1 = (int) Math.Floor(Math.Pow((double) this.entries, 1.0 / (double) this.dim));
        while (true)
        {
          int num2 = 1;
          int num3 = 1;
          for (int index = 0; index < this.dim; ++index)
          {
            num2 *= num1;
            num3 *= num1 + 1;
          }
          if (num2 > this.entries || num3 <= this.entries)
          {
            if (num2 > this.entries)
              --num1;
            else
              ++num1;
          }
          else
            break;
        }
        return num1;
      }

      internal void clear()
      {
      }

      internal float[] unquantize()
      {
        if (this.maptype != 1 && this.maptype != 2)
          return (float[]) null;
        float num1 = StaticCodeBook.float32_unpack(this.q_min);
        float num2 = StaticCodeBook.float32_unpack(this.q_delta);
        float[] numArray = new float[this.entries * this.dim];
        switch (this.maptype)
        {
          case 1:
            int num3 = this.maptype1_quantvals();
            for (int index1 = 0; index1 < this.entries; ++index1)
            {
              float num4 = 0.0f;
              int num5 = 1;
              for (int index2 = 0; index2 < this.dim; ++index2)
              {
                float num6 = Math.Abs((float) this.quantlist[index1 / num5 % num3]) * num2 + num1 + num4;
                if (this.q_sequencep != 0)
                  num4 = num6;
                numArray[index1 * this.dim + index2] = num6;
                num5 *= num3;
              }
            }
            break;
          case 2:
            for (int index3 = 0; index3 < this.entries; ++index3)
            {
              float num7 = 0.0f;
              for (int index4 = 0; index4 < this.dim; ++index4)
              {
                float num8 = Math.Abs((float) this.quantlist[index3 * this.dim + index4]) * num2 + num1 + num7;
                if (this.q_sequencep != 0)
                  num7 = num8;
                numArray[index3 * this.dim + index4] = num8;
              }
            }
            break;
        }
        return numArray;
      }

      internal static int ilog(int v)
      {
        int num = 0;
        for (; v != 0; v >>>= 1)
          ++num;
        return num;
      }

      internal static long float32_pack(float val)
      {
        uint num1 = 0;
        if ((double) val < 0.0)
        {
          num1 = 2147483648U /*0x80000000*/;
          val = -val;
        }
        int num2 = (int) Math.Floor(Math.Log((double) val) / Math.Log(2.0));
        int num3 = (int) Math.Round(Math.Pow((double) val, (double) (StaticCodeBook.VQ_FMAN - 1 - num2)));
        int num4 = num2 + StaticCodeBook.VQ_FEXP_BIAS << StaticCodeBook.VQ_FMAN;
        return (long) ((int) num1 | num4 | num3);
      }

      internal static float float32_unpack(int val)
      {
        float foo = (float) (val & 2097151 /*0x1FFFFF*/);
        float num = (float) (uint) ((val & 2145386496 /*0x7FE00000*/) >>> StaticCodeBook.VQ_FMAN);
        if (((long) val & 2147483648L /*0x80000000*/) != 0L)
          foo = -foo;
        return StaticCodeBook.ldexp(foo, (int) num - (StaticCodeBook.VQ_FMAN - 1) - StaticCodeBook.VQ_FEXP_BIAS);
      }

      internal static float ldexp(float foo, int e) => foo * (float) Math.Pow(2.0, (double) e);
    }
}
