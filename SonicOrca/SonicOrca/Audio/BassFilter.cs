// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.BassFilter
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Audio
{

    internal class BassFilter
    {
      public static void Apply(byte[] buffer)
      {
        float[] leftSamples;
        float[] rightSamples;
        Sample.PCMToSamples(buffer, out leftSamples, out rightSamples);
        int val2 = 512 /*0x0200*/;
        for (int index = 0; index < leftSamples.Length; index += val2)
        {
          float[] range1 = leftSamples.GetRange<float>(index, Math.Min(leftSamples.Length - index, val2));
          float[] range2 = rightSamples.GetRange<float>(index, Math.Min(rightSamples.Length - index, val2));
          BassFilter.Apply(range1);
          BassFilter.Apply(range2);
          Array.Copy((Array) range1, 0, (Array) leftSamples, index, range1.Length);
          Array.Copy((Array) range2, 0, (Array) rightSamples, index, range2.Length);
        }
        Array.Copy((Array) Sample.SamplesToPCM(leftSamples, leftSamples), (Array) buffer, buffer.Length);
      }

      private static void Apply(float[] samples)
      {
        int m = (int) Math.Log((double) samples.Length, 2.0);
        ComplexNumber[] frequency = FastFourierTransform.TimeToFrequency(m, ((IEnumerable<float>) samples).Select<float, double>((Func<float, double>) (x => (double) x)).ToArray<double>());
        for (int index = 32 /*0x20*/; index < samples.Length; ++index)
        {
          frequency[index].Real = 0.0;
          frequency[index].Imaginary = 0.0;
        }
        FastFourierTransform.FrequencyToTime(m, frequency);
        for (int index = 0; index < samples.Length; ++index)
          samples[index] = (float) frequency[index].Real;
      }
    }
}
