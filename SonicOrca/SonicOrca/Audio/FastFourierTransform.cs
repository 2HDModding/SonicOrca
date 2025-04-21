// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.FastFourierTransform
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Audio
{

    public static class FastFourierTransform
    {
      public static double HammingWindow(int n, int frameSize)
      {
        return 0.54 - 0.46 * Math.Cos(2.0 * Math.PI * (double) n / (double) (frameSize - 1));
      }

      public static ComplexNumber[] TimeToFrequency(int m, double[] samples)
      {
        ComplexNumber[] array = ((IEnumerable<double>) samples).Select<double, ComplexNumber>((Func<double, ComplexNumber>) (x => new ComplexNumber(x, 0.0))).ToArray<ComplexNumber>();
        FastFourierTransform.Apply(true, m, array);
        return array;
      }

      public static void TimeToFrequency(int m, ComplexNumber[] data)
      {
        FastFourierTransform.Apply(true, m, data);
      }

      public static void FrequencyToTime(int m, ComplexNumber[] data)
      {
        FastFourierTransform.Apply(false, m, data);
      }

      private static void Apply(bool forward, int m, ComplexNumber[] data)
      {
        int num1 = 1;
        int index1 = 0;
        int num2 = 1;
        double num3 = -1.0;
        double num4 = 0.0;
        for (int index2 = 0; index2 < m; ++index2)
          num1 *= 2;
        int num5 = num1 >> 1;
        for (int index3 = 0; index3 < num1 - 1; ++index3)
        {
          if (index3 < index1)
          {
            double real = data[index3].Real;
            double imaginary = data[index3].Imaginary;
            data[index3].Real = data[index1].Real;
            data[index3].Imaginary = data[index1].Imaginary;
            data[index1].Real = real;
            data[index1].Imaginary = imaginary;
          }
          int num6;
          for (num6 = num5; num6 <= index1; num6 >>= 1)
            index1 -= num6;
          index1 += num6;
        }
        for (int index4 = 0; index4 < m; ++index4)
        {
          int num7 = num2;
          num2 <<= 1;
          double num8 = 1.0;
          double num9 = 0.0;
          for (int index5 = 0; index5 < num7; ++index5)
          {
            for (int index6 = index5; index6 < num1; index6 += num2)
            {
              int index7 = index6 + num7;
              double num10 = num8 * data[index7].Real - num9 * data[index7].Imaginary;
              double num11 = num8 * data[index7].Imaginary + num9 * data[index7].Real;
              data[index7].Real = data[index6].Real - num10;
              data[index7].Imaginary = data[index6].Imaginary - num11;
              data[index6].Real += num10;
              data[index6].Imaginary += num11;
            }
            double num12 = num8 * num3 - num9 * num4;
            num9 = num8 * num4 + num9 * num3;
            num8 = num12;
          }
          num4 = Math.Sqrt((1.0 - num3) / 2.0);
          if (forward)
            num4 = -num4;
          num3 = Math.Sqrt((1.0 + num3) / 2.0);
        }
        if (!forward)
          return;
        for (int index8 = 0; index8 < num1; ++index8)
        {
          data[index8].Real /= (double) num1;
          data[index8].Imaginary /= (double) num1;
        }
      }
    }
}
