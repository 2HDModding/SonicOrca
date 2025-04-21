// Decompiled with JetBrains decompiler
// Type: csvorbis.CodeBook
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;
using System.Runtime.CompilerServices;

namespace csvorbis
{

    internal class CodeBook
    {
      internal int dim;
      internal int entries;
      internal StaticCodeBook c = new StaticCodeBook();
      internal float[] valuelist;
      internal int[] codelist;
      internal DecodeAux decode_tree;
      internal int[] t = new int[15];

      internal int encode(int a, csBuffer b)
      {
        b.write(this.codelist[a], this.c.lengthlist[a]);
        return this.c.lengthlist[a];
      }

      internal int errorv(float[] a)
      {
        int num = this.best(a, 1);
        for (int index = 0; index < this.dim; ++index)
          a[index] = this.valuelist[num * this.dim + index];
        return num;
      }

      internal int encodev(int best, float[] a, csBuffer b)
      {
        for (int index = 0; index < this.dim; ++index)
          a[index] = this.valuelist[best * this.dim + index];
        return this.encode(best, b);
      }

      internal int encodevs(float[] a, csBuffer b, int step, int addmul)
      {
        return this.encode(this.besterror(a, step, addmul), b);
      }

      [MethodImpl(MethodImplOptions.Synchronized)]
      internal int decodevs_add(float[] a, int offset, csBuffer b, int n)
      {
        int length = n / this.dim;
        if (this.t.Length < length)
          this.t = new int[length];
        for (int index = 0; index < length; ++index)
        {
          int num = this.decode(b);
          if (num == -1)
            return -1;
          this.t[index] = num * this.dim;
        }
        int num1 = 0;
        int num2 = 0;
        while (num1 < this.dim)
        {
          for (int index = 0; index < length; ++index)
            a[offset + num2 + index] += this.valuelist[this.t[index] + num1];
          ++num1;
          num2 += length;
        }
        return 0;
      }

      internal int decodev_add(float[] a, int offset, csBuffer b, int n)
      {
        if (this.dim > 8)
        {
          int num1 = 0;
          while (num1 < n)
          {
            int num2 = this.decode(b);
            if (num2 == -1)
              return -1;
            int num3 = num2 * this.dim;
            int num4 = 0;
            while (num4 < this.dim)
              a[offset + num1++] += this.valuelist[num3 + num4++];
          }
        }
        else
        {
          int num5 = 0;
          while (num5 < n)
          {
            int num6 = this.decode(b);
            if (num6 == -1)
              return -1;
            int num7 = num6 * this.dim;
            int num8 = 0;
            for (int index = 0; index < this.dim; ++index)
              a[offset + num5++] += this.valuelist[num7 + num8++];
          }
        }
        return 0;
      }

      internal int decodev_set(float[] a, int offset, csBuffer b, int n)
      {
        int num1 = 0;
        while (num1 < n)
        {
          int num2 = this.decode(b);
          if (num2 == -1)
            return -1;
          int num3 = num2 * this.dim;
          int num4 = 0;
          while (num4 < this.dim)
            a[offset + num1++] = this.valuelist[num3 + num4++];
        }
        return 0;
      }

      internal int decodevv_add(float[][] a, int offset, int ch, csBuffer b, int n)
      {
        int index1 = 0;
        int index2 = offset / ch;
        while (index2 < (offset + n) / ch)
        {
          int num1 = this.decode(b);
          if (num1 == -1)
            return -1;
          int num2 = num1 * this.dim;
          for (int index3 = 0; index3 < this.dim; ++index3)
          {
            a[index1][index2] += this.valuelist[num2 + index3];
            ++index1;
            if (index1 == ch)
            {
              index1 = 0;
              ++index2;
            }
          }
        }
        return 0;
      }

      internal int decode(csBuffer b)
      {
        int index1 = 0;
        DecodeAux decodeTree = this.decode_tree;
        int index2 = b.look(decodeTree.tabn);
        if (index2 >= 0)
        {
          index1 = decodeTree.tab[index2];
          b.adv(decodeTree.tabl[index2]);
          if (index1 <= 0)
            return -index1;
        }
        do
        {
          switch (b.read1())
          {
            case 0:
              index1 = decodeTree.ptr0[index1];
              break;
            case 1:
              index1 = decodeTree.ptr1[index1];
              break;
            default:
              return -1;
          }
        }
        while (index1 > 0);
        return -index1;
      }

      internal int decodevs(float[] a, int index, csBuffer b, int step, int addmul)
      {
        int num1 = this.decode(b);
        if (num1 == -1)
          return -1;
        switch (addmul)
        {
          case -1:
            int num2 = 0;
            int num3 = 0;
            while (num2 < this.dim)
            {
              a[index + num3] = this.valuelist[num1 * this.dim + num2];
              ++num2;
              num3 += step;
            }
            break;
          case 0:
            int num4 = 0;
            int num5 = 0;
            while (num4 < this.dim)
            {
              a[index + num5] += this.valuelist[num1 * this.dim + num4];
              ++num4;
              num5 += step;
            }
            break;
          case 1:
            int num6 = 0;
            int num7 = 0;
            while (num6 < this.dim)
            {
              a[index + num7] *= this.valuelist[num1 * this.dim + num6];
              ++num6;
              num7 += step;
            }
            break;
        }
        return num1;
      }

      internal int best(float[] a, int step)
      {
        EncodeAuxNearestMatch nearestTree = this.c.nearest_tree;
        EncodeAuxThreshMatch threshTree = this.c.thresh_tree;
        int index1 = 0;
        if (threshTree != null)
        {
          int index2 = 0;
          int num = 0;
          int index3 = step * (this.dim - 1);
          while (num < this.dim)
          {
            int index4 = 0;
            while (index4 < threshTree.threshvals - 1 && (double) a[index3] >= (double) threshTree.quantthresh[index4])
              ++index4;
            index2 = index2 * threshTree.quantvals + threshTree.quantmap[index4];
            ++num;
            index3 -= step;
          }
          if (this.c.lengthlist[index2] > 0)
            return index2;
        }
        if (nearestTree != null)
        {
          do
          {
            double num1 = 0.0;
            int num2 = nearestTree.p[index1];
            int num3 = nearestTree.q[index1];
            int num4 = 0;
            int index5 = 0;
            while (num4 < this.dim)
            {
              num1 += ((double) this.valuelist[num2 + num4] - (double) this.valuelist[num3 + num4]) * ((double) a[index5] - ((double) this.valuelist[num2 + num4] + (double) this.valuelist[num3 + num4]) * 0.5);
              ++num4;
              index5 += step;
            }
            index1 = num1 <= 0.0 ? -nearestTree.ptr1[index1] : -nearestTree.ptr0[index1];
          }
          while (index1 > 0);
          return -index1;
        }
        int num5 = -1;
        float num6 = 0.0f;
        int index6 = 0;
        for (int index7 = 0; index7 < this.entries; ++index7)
        {
          if (this.c.lengthlist[index7] > 0)
          {
            float num7 = CodeBook.dist(this.dim, this.valuelist, index6, a, step);
            if (num5 == -1 || (double) num7 < (double) num6)
            {
              num6 = num7;
              num5 = index7;
            }
          }
          index6 += this.dim;
        }
        return num5;
      }

      internal int besterror(float[] a, int step, int addmul)
      {
        int num1 = this.best(a, step);
        switch (addmul)
        {
          case 0:
            int num2 = 0;
            int index1 = 0;
            while (num2 < this.dim)
            {
              a[index1] -= this.valuelist[num1 * this.dim + num2];
              ++num2;
              index1 += step;
            }
            break;
          case 1:
            int num3 = 0;
            int index2 = 0;
            while (num3 < this.dim)
            {
              float num4 = this.valuelist[num1 * this.dim + num3];
              if ((double) num4 == 0.0)
                a[index2] = 0.0f;
              else
                a[index2] /= num4;
              ++num3;
              index2 += step;
            }
            break;
        }
        return num1;
      }

      internal void clear()
      {
      }

      internal static float dist(int el, float[] rref, int index, float[] b, int step)
      {
        float num1 = 0.0f;
        for (int index1 = 0; index1 < el; ++index1)
        {
          float num2 = rref[index + index1] - b[index1 * step];
          num1 += num2 * num2;
        }
        return num1;
      }

      internal int init_decode(StaticCodeBook s)
      {
        this.c = s;
        this.entries = s.entries;
        this.dim = s.dim;
        this.valuelist = s.unquantize();
        this.decode_tree = this.make_decode_tree();
        if (this.decode_tree != null)
          return 0;
        this.clear();
        return -1;
      }

      internal static int[] make_words(int[] l, int n)
      {
        int[] numArray1 = new int[33];
        int[] numArray2 = new int[n];
        for (int index1 = 0; index1 < n; ++index1)
        {
          int index2 = l[index1];
          if (index2 > 0)
          {
            int num = numArray1[index2];
            if (index2 < 32 /*0x20*/ && num >>> index2 != 0)
              return (int[]) null;
            numArray2[index1] = num;
            for (int index3 = index2; index3 > 0; --index3)
            {
              if ((numArray1[index3] & 1) != 0)
              {
                if (index3 == 1)
                {
                  ++numArray1[1];
                  break;
                }
                numArray1[index3] = numArray1[index3 - 1] << 1;
                break;
              }
              ++numArray1[index3];
            }
            for (int index4 = index2 + 1; index4 < 33 && (long) (uint) (numArray1[index4] >>> 1) == (long) num; ++index4)
            {
              num = numArray1[index4];
              numArray1[index4] = numArray1[index4 - 1] << 1;
            }
          }
        }
        for (int index5 = 0; index5 < n; ++index5)
        {
          int num = 0;
          for (int index6 = 0; index6 < l[index5]; ++index6)
            num = num << 1 | numArray2[index5] >>> index6 & 1;
          numArray2[index5] = num;
        }
        return numArray2;
      }

      internal DecodeAux make_decode_tree()
      {
        int num1 = 0;
        DecodeAux decodeAux = new DecodeAux();
        int[] numArray1 = decodeAux.ptr0 = new int[this.entries * 2];
        int[] numArray2 = decodeAux.ptr1 = new int[this.entries * 2];
        int[] numArray3 = CodeBook.make_words(this.c.lengthlist, this.c.entries);
        if (numArray3 == null)
          return (DecodeAux) null;
        decodeAux.aux = this.entries * 2;
        for (int index1 = 0; index1 < this.entries; ++index1)
        {
          if (this.c.lengthlist[index1] > 0)
          {
            int index2 = 0;
            int num2;
            for (num2 = 0; num2 < this.c.lengthlist[index1] - 1; ++num2)
            {
              if ((numArray3[index1] >>> num2 & 1) == 0)
              {
                if (numArray1[index2] == 0)
                  numArray1[index2] = ++num1;
                index2 = numArray1[index2];
              }
              else
              {
                if (numArray2[index2] == 0)
                  numArray2[index2] = ++num1;
                index2 = numArray2[index2];
              }
            }
            if ((numArray3[index1] >>> num2 & 1) == 0)
              numArray1[index2] = -index1;
            else
              numArray2[index2] = -index1;
          }
        }
        decodeAux.tabn = CodeBook.ilog(this.entries) - 4;
        if (decodeAux.tabn < 5)
          decodeAux.tabn = 5;
        int length = 1 << decodeAux.tabn;
        decodeAux.tab = new int[length];
        decodeAux.tabl = new int[length];
        for (int index3 = 0; index3 < length; ++index3)
        {
          int index4 = 0;
          int num3;
          for (num3 = 0; num3 < decodeAux.tabn && (index4 > 0 || num3 == 0); ++num3)
            index4 = (index3 & 1 << num3) == 0 ? numArray1[index4] : numArray2[index4];
          decodeAux.tab[index3] = index4;
          decodeAux.tabl[index3] = num3;
        }
        return decodeAux;
      }

      internal static int ilog(int v)
      {
        int num = 0;
        for (; v != 0; v >>>= 1)
          ++num;
        return num;
      }
    }
}
