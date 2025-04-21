// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.BasicSampleMixer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Audio
{

    public class BasicSampleMixer : ISampleMixer
    {
      public void Mix(byte[] buffer, int offset, int length, IEnumerable<ISampleProvider> channels)
      {
        byte[] buffer1 = new byte[length];
        IEnumerable<ISampleProvider> second = (IEnumerable<ISampleProvider>) channels.OfType<SampleInstance>().Where<SampleInstance>((Func<SampleInstance, bool>) (x => x.Classification == SampleInstanceClassification.Music));
        foreach (ISampleProvider sampleProvider in (IEnumerable<ISampleProvider>) channels.Except<ISampleProvider>(second).Concat<ISampleProvider>(second).Where<ISampleProvider>((Func<ISampleProvider, bool>) (x => x.Playing)).ToArray<ISampleProvider>())
        {
          double calculatedVolume = sampleProvider.CalculatedVolume;
          if (calculatedVolume > 0.0)
          {
            int num1 = sampleProvider.Read(buffer1, offset, length);
            for (int startIndex = 0; startIndex < num1; startIndex += 2)
            {
              short num2 = this.MixSample(BitConverter.ToInt16(buffer, startIndex), (short) ((double) BitConverter.ToInt16(buffer1, startIndex) * calculatedVolume));
              buffer[startIndex] = (byte) ((uint) num2 & (uint) byte.MaxValue);
              buffer[startIndex + 1] = (byte) ((int) num2 >> 8 & (int) byte.MaxValue);
            }
          }
        }
      }

      private short MixSample(short a, short b)
      {
        int num1 = (int) a;
        int num2 = (int) b;
        return num1 >= 0 || num2 >= 0 ? (num1 <= 0 || num2 <= 0 ? (short) (num1 + num2) : (short) (num1 + num2 - num1 * num2 / (int) short.MaxValue)) : (short) (num1 + num2 - num1 * num2 / (int) short.MinValue);
      }
    }
}
