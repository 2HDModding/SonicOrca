// Decompiled with JetBrains decompiler
// Type: csvorbis.Lpc
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace csvorbis
{

    internal class Lpc
    {
      private Drft fft = new Drft();
      private int ln;
      private int m;

      private static float lpc_from_data(float[] data, float[] lpc, int n, int m)
      {
        float[] numArray = new float[m + 1];
        int index1 = m + 1;
        while (index1-- != 0)
        {
          float num = 0.0f;
          for (int index2 = index1; index2 < n; ++index2)
            num += data[index2] * data[index2 - index1];
          numArray[index1] = num;
        }
        float num1 = numArray[0];
        for (int index3 = 0; index3 < m; ++index3)
        {
          float num2 = -numArray[index3 + 1];
          if ((double) num1 == 0.0)
          {
            for (int index4 = 0; index4 < m; ++index4)
              lpc[index4] = 0.0f;
            return 0.0f;
          }
          for (int index5 = 0; index5 < index3; ++index5)
            num2 -= lpc[index5] * numArray[index3 - index5];
          float num3 = num2 / num1;
          lpc[index3] = num3;
          int index6;
          for (index6 = 0; index6 < index3 / 2; ++index6)
          {
            float num4 = lpc[index6];
            lpc[index6] += num3 * lpc[index3 - 1 - index6];
            lpc[index3 - 1 - index6] += num3 * num4;
          }
          if (index3 % 2 != 0)
            lpc[index6] += lpc[index6] * num3;
          num1 *= (float) (1.0 - (double) num3 * (double) num3);
        }
        return num1;
      }

      private float lpc_from_curve(float[] curve, float[] lpc)
      {
        int ln = this.ln;
        float[] data = new float[ln + ln];
        float num1 = 0.5f / (float) ln;
        for (int index = 0; index < ln; ++index)
        {
          data[index * 2] = curve[index] * num1;
          data[index * 2 + 1] = 0.0f;
        }
        data[ln * 2 - 1] = curve[ln - 1] * num1;
        int n = ln * 2;
        this.fft.backward(data);
        int index1 = 0;
        int index2 = n / 2;
        while (index1 < n / 2)
        {
          float num2 = data[index1];
          data[index1++] = data[index2];
          data[index2++] = num2;
        }
        return Lpc.lpc_from_data(data, lpc, n, this.m);
      }

      internal void init(int mapped, int m)
      {
        this.ln = mapped;
        this.m = m;
        this.fft.init(mapped * 2);
      }

      private void clear() => this.fft.clear();

      private static float FAST_HYPOT(float a, float b)
      {
        return (float) Math.Sqrt((double) a * (double) a + (double) b * (double) b);
      }

      internal void lpc_to_curve(float[] curve, float[] lpc, float amp)
      {
        for (int index = 0; index < this.ln * 2; ++index)
          curve[index] = 0.0f;
        if ((double) amp == 0.0)
          return;
        for (int index = 0; index < this.m; ++index)
        {
          curve[index * 2 + 1] = lpc[index] / (4f * amp);
          curve[index * 2 + 2] = (float) (-(double) lpc[index] / (4.0 * (double) amp));
        }
        this.fft.backward(curve);
        int num1 = this.ln * 2;
        float num2 = 1f / amp;
        curve[0] = (float) (1.0 / ((double) curve[0] * 2.0 + (double) num2));
        for (int index = 1; index < this.ln; ++index)
        {
          double num3 = (double) curve[index] + (double) curve[num1 - index];
          float b = curve[index] - curve[num1 - index];
          double num4 = (double) num2;
          float a = (float) (num3 + num4);
          curve[index] = 1f / Lpc.FAST_HYPOT(a, b);
        }
      }
    }
}
