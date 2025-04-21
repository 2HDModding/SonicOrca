// Decompiled with JetBrains decompiler
// Type: csvorbis.Mdct
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Runtime.CompilerServices;

namespace csvorbis
{

    internal class Mdct
    {
      private int n;
      private int log2n;
      private float[] trig;
      private int[] bitrev;
      private float scale;
      private float[] _x = new float[1024 /*0x0400*/];
      private float[] _w = new float[1024 /*0x0400*/];

      internal void init(int n)
      {
        this.bitrev = new int[n / 4];
        this.trig = new float[n + n / 4];
        this.log2n = (int) Math.Round(Math.Log((double) n) / Math.Log(2.0));
        this.n = n;
        int num1 = 0;
        int num2 = 1;
        int num3 = num1 + n / 2;
        int num4 = num3 + 1;
        int num5 = num3 + n / 2;
        int num6 = num5 + 1;
        for (int index = 0; index < n / 4; ++index)
        {
          this.trig[num1 + index * 2] = (float) Math.Cos(Math.PI / (double) n * (double) (4 * index));
          this.trig[num2 + index * 2] = (float) -Math.Sin(Math.PI / (double) n * (double) (4 * index));
          this.trig[num3 + index * 2] = (float) Math.Cos(Math.PI / (double) (2 * n) * (double) (2 * index + 1));
          this.trig[num4 + index * 2] = (float) Math.Sin(Math.PI / (double) (2 * n) * (double) (2 * index + 1));
        }
        for (int index = 0; index < n / 8; ++index)
        {
          this.trig[num5 + index * 2] = (float) Math.Cos(Math.PI / (double) n * (double) (4 * index + 2));
          this.trig[num6 + index * 2] = (float) -Math.Sin(Math.PI / (double) n * (double) (4 * index + 2));
        }
        int num7 = (1 << this.log2n - 1) - 1;
        int num8 = 1 << this.log2n - 2;
        for (int index1 = 0; index1 < n / 8; ++index1)
        {
          int num9 = 0;
          for (int index2 = 0; num8 >>> index2 != 0; ++index2)
          {
            if (((long) (uint) (num8 >>> index2) & (long) index1) != 0L)
              num9 |= 1 << index2;
          }
          this.bitrev[index1 * 2] = ~num9 & num7;
          this.bitrev[index1 * 2 + 1] = num9;
        }
        this.scale = 4f / (float) n;
      }

      internal void clear()
      {
      }

      internal void forward(float[] fin, float[] fout)
      {
      }

      [MethodImpl(MethodImplOptions.Synchronized)]
      internal void backward(float[] fin, float[] fout)
      {
        if (this._x.Length < this.n / 2)
          this._x = new float[this.n / 2];
        if (this._w.Length < this.n / 2)
          this._w = new float[this.n / 2];
        float[] x = this._x;
        float[] w = this._w;
        int n2 = this.n >>> 1;
        int n4 = this.n >>> 2;
        int n8 = this.n >>> 3;
        int index1 = 1;
        int num1 = 0;
        int index2 = n2;
        for (int index3 = 0; index3 < n8; ++index3)
        {
          index2 -= 2;
          float[] numArray1 = x;
          int index4 = num1;
          int num2 = index4 + 1;
          double num3 = -(double) fin[index1 + 2] * (double) this.trig[index2 + 1] - (double) fin[index1] * (double) this.trig[index2];
          numArray1[index4] = (float) num3;
          float[] numArray2 = x;
          int index5 = num2;
          num1 = index5 + 1;
          double num4 = (double) fin[index1] * (double) this.trig[index2 + 1] - (double) fin[index1 + 2] * (double) this.trig[index2];
          numArray2[index5] = (float) num4;
          index1 += 4;
        }
        int index6 = n2 - 4;
        for (int index7 = 0; index7 < n8; ++index7)
        {
          index2 -= 2;
          float[] numArray3 = x;
          int index8 = num1;
          int num5 = index8 + 1;
          double num6 = (double) fin[index6] * (double) this.trig[index2 + 1] + (double) fin[index6 + 2] * (double) this.trig[index2];
          numArray3[index8] = (float) num6;
          float[] numArray4 = x;
          int index9 = num5;
          num1 = index9 + 1;
          double num7 = (double) fin[index6] * (double) this.trig[index2] - (double) fin[index6 + 2] * (double) this.trig[index2 + 1];
          numArray4[index9] = (float) num7;
          index6 -= 4;
        }
        float[] numArray = this.mdct_kernel(x, w, this.n, n2, n4, n8);
        int index10 = 0;
        int index11 = n2;
        int index12 = n4;
        int index13 = index12 - 1;
        int index14 = n4 + n2;
        int index15 = index14 - 1;
        for (int index16 = 0; index16 < n4; ++index16)
        {
          float num8 = (float) ((double) numArray[index10] * (double) this.trig[index11 + 1] - (double) numArray[index10 + 1] * (double) this.trig[index11]);
          float num9 = (float) -((double) numArray[index10] * (double) this.trig[index11] + (double) numArray[index10 + 1] * (double) this.trig[index11 + 1]);
          fout[index12] = -num8;
          fout[index13] = num8;
          fout[index14] = num9;
          fout[index15] = num9;
          ++index12;
          --index13;
          ++index14;
          --index15;
          index10 += 2;
          index11 += 2;
        }
      }

      internal float[] mdct_kernel(float[] x, float[] w, int n, int n2, int n4, int n8)
      {
        int index1 = n4;
        int index2 = 0;
        int num1 = n4;
        int index3 = n2;
        int index4;
        for (int index5 = 0; index5 < n4; index5 = index4 + 1)
        {
          float num2 = x[index1] - x[index2];
          float[] numArray1 = w;
          int index6 = num1 + index5;
          float[] numArray2 = x;
          int index7 = index1;
          int index8 = index7 + 1;
          double num3 = (double) numArray2[index7];
          float[] numArray3 = x;
          int index9 = index2;
          int index10 = index9 + 1;
          double num4 = (double) numArray3[index9];
          double num5 = num3 + num4;
          numArray1[index6] = (float) num5;
          float num6 = x[index8] - x[index10];
          index3 -= 4;
          float[] numArray4 = w;
          int index11 = index5;
          index4 = index11 + 1;
          double num7 = (double) num2 * (double) this.trig[index3] + (double) num6 * (double) this.trig[index3 + 1];
          numArray4[index11] = (float) num7;
          w[index4] = (float) ((double) num6 * (double) this.trig[index3] - (double) num2 * (double) this.trig[index3 + 1]);
          float[] numArray5 = w;
          int index12 = num1 + index4;
          float[] numArray6 = x;
          int index13 = index8;
          index1 = index13 + 1;
          double num8 = (double) numArray6[index13];
          float[] numArray7 = x;
          int index14 = index10;
          index2 = index14 + 1;
          double num9 = (double) numArray7[index14];
          double num10 = num8 + num9;
          numArray5[index12] = (float) num10;
        }
        for (int index15 = 0; index15 < this.log2n - 3; ++index15)
        {
          int num11 = n >>> index15 + 2;
          int num12 = 1 << index15 + 3;
          int num13 = n2 - 2;
          int index16 = 0;
          for (int index17 = 0; (long) index17 < (long) (uint) (num11 >>> 2); ++index17)
          {
            int index18 = num13;
            int index19 = index18 - (num11 >> 1);
            float num14 = this.trig[index16];
            float num15 = this.trig[index16 + 1];
            num13 -= 2;
            int num16 = num11 + 1;
            for (int index20 = 0; index20 < 2 << index15; ++index20)
            {
              float num17 = w[index18] - w[index19];
              x[index18] = w[index18] + w[index19];
              int index21;
              int index22;
              float num18 = w[index21 = index18 + 1] - w[index22 = index19 + 1];
              x[index21] = w[index21] + w[index22];
              x[index22] = (float) ((double) num18 * (double) num14 - (double) num17 * (double) num15);
              x[index22 - 1] = (float) ((double) num17 * (double) num14 + (double) num18 * (double) num15);
              index18 = index21 - num16;
              index19 = index22 - num16;
            }
            num11 = num16 - 1;
            index16 += num12;
          }
          float[] numArray = w;
          w = x;
          x = numArray;
        }
        int index23 = n;
        int num19 = 0;
        int num20 = 0;
        int num21 = n2 - 1;
        for (int index24 = 0; index24 < n8; ++index24)
        {
          int[] bitrev1 = this.bitrev;
          int index25 = num19;
          int num22 = index25 + 1;
          int index26 = bitrev1[index25];
          int[] bitrev2 = this.bitrev;
          int index27 = num22;
          num19 = index27 + 1;
          int index28 = bitrev2[index27];
          double num23 = (double) w[index26] - (double) w[index28 + 1];
          float num24 = w[index26 - 1] + w[index28];
          float num25 = w[index26] + w[index28 + 1];
          float num26 = w[index26 - 1] - w[index28];
          float num27 = (float) num23 * this.trig[index23];
          double num28 = (double) num24;
          float[] trig1 = this.trig;
          int index29 = index23;
          int index30 = index29 + 1;
          double num29 = (double) trig1[index29];
          float num30 = (float) (num28 * num29);
          float num31 = (float) num23 * this.trig[index30];
          double num32 = (double) num24;
          float[] trig2 = this.trig;
          int index31 = index30;
          index23 = index31 + 1;
          double num33 = (double) trig2[index31];
          float num34 = (float) (num32 * num33);
          float[] numArray8 = x;
          int index32 = num20;
          int num35 = index32 + 1;
          double num36 = ((double) num25 + (double) num31 + (double) num30) * 0.5;
          numArray8[index32] = (float) num36;
          float[] numArray9 = x;
          int index33 = num21;
          int num37 = index33 - 1;
          double num38 = (-(double) num26 + (double) num34 - (double) num27) * 0.5;
          numArray9[index33] = (float) num38;
          float[] numArray10 = x;
          int index34 = num35;
          num20 = index34 + 1;
          double num39 = ((double) num26 + (double) num34 - (double) num27) * 0.5;
          numArray10[index34] = (float) num39;
          float[] numArray11 = x;
          int index35 = num37;
          num21 = index35 - 1;
          double num40 = ((double) num25 - (double) num31 - (double) num30) * 0.5;
          numArray11[index35] = (float) num40;
        }
        return x;
      }
    }
}
