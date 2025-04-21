// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.Sample
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;

namespace SonicOrca.Audio
{

    public class Sample : ILoadedResource, IDisposable
    {
      private readonly byte[] _pcmData;
      private readonly int _bitsPerSample;
      private readonly int _sampleRate;
      private readonly int _channels;

      public Resource Resource { get; set; }

      public byte[] PcmData => this._pcmData;

      public int BitsPerSample => this._bitsPerSample;

      public int SampleRate => this._sampleRate;

      public int Channels => this._channels;

      public int SampleCount => this._pcmData.Length / (this._bitsPerSample / 8 * this._channels);

      public Sample(byte[] pcmData, int bitsPerSample, int sampleRate, int channels)
      {
        this._pcmData = pcmData;
        this._bitsPerSample = bitsPerSample;
        this._sampleRate = sampleRate;
        this._channels = channels;
      }

      public void OnLoaded()
      {
      }

      public void Dispose()
      {
      }

      public long GetPcmDataOffset(int sampleIndex)
      {
        return (long) (this._bitsPerSample / 8 * this._channels) * (long) sampleIndex;
      }

      public int GetSampleIndex(long pcmDataOffset)
      {
        return (int) (pcmDataOffset / (long) (this._bitsPerSample / 8 * this._channels));
      }

      public static int PCMToSamples(byte[] source, out float[] leftSamples, out float[] rightSamples)
      {
        leftSamples = new float[source.Length / 4];
        rightSamples = new float[source.Length / 4];
        int samples = 0;
        for (int startIndex = 0; startIndex < source.Length; startIndex += 4)
        {
          leftSamples[samples] = (float) BitConverter.ToInt16(source, startIndex) / (float) short.MaxValue;
          rightSamples[samples] = (float) BitConverter.ToInt16(source, startIndex + 2) / (float) short.MaxValue;
          ++samples;
        }
        return samples;
      }

      public static byte[] SamplesToPCM(float[] leftSamples, float[] rightSamples)
      {
        byte[] pcm = new byte[leftSamples.Length * 4];
        int num1 = 0;
        for (int index1 = 0; index1 < leftSamples.Length; ++index1)
        {
          short num2 = (short) ((double) leftSamples[index1] * (double) short.MaxValue);
          short num3 = (short) ((double) rightSamples[index1] * (double) short.MaxValue);
          byte[] numArray1 = pcm;
          int index2 = num1;
          int num4 = index2 + 1;
          int num5 = (int) (byte) ((uint) num2 & (uint) byte.MaxValue);
          numArray1[index2] = (byte) num5;
          byte[] numArray2 = pcm;
          int index3 = num4;
          int num6 = index3 + 1;
          int num7 = (int) (byte) ((uint) num2 >> 8);
          numArray2[index3] = (byte) num7;
          byte[] numArray3 = pcm;
          int index4 = num6;
          int num8 = index4 + 1;
          int num9 = (int) (byte) ((uint) num3 & (uint) byte.MaxValue);
          numArray3[index4] = (byte) num9;
          byte[] numArray4 = pcm;
          int index5 = num8;
          num1 = index5 + 1;
          int num10 = (int) (byte) ((uint) num3 >> 8);
          numArray4[index5] = (byte) num10;
        }
        return pcm;
      }
    }
}
