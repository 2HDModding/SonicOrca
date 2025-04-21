// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.SampleGenerator
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Audio
{

    public abstract class SampleGenerator : IDisposable, ISampleProvider
    {
      private readonly AudioContext _audioAdapter;

      public double Volume { get; set; }

      public double Pan { get; set; }

      public SampleInstanceClassification Classification { get; set; }

      public bool Playing { get; private set; }

      public double CalculatedVolume
      {
        get
        {
          double volume = this.Volume;
          if (this.Classification == SampleInstanceClassification.Sound)
            volume *= this._audioAdapter.SoundVolume;
          else if (this.Classification == SampleInstanceClassification.Music)
            volume *= this._audioAdapter.MusicVolume;
          return volume * this._audioAdapter.Volume;
        }
      }

      public SampleGenerator(AudioContext audioAdapter)
      {
        this._audioAdapter = audioAdapter;
        this.Volume = 1.0;
        this._audioAdapter.RegisterSampleProvider((ISampleProvider) this);
      }

      public void Dispose() => this._audioAdapter.UnregisterSampleProvider((ISampleProvider) this);

      public void Play() => this.Playing = true;

      public void Stop() => this.Playing = false;

      public int Read(byte[] buffer, int offset, int count)
      {
        for (int index = 0; index < count; index += 4)
        {
          byte[] bytes = BitConverter.GetBytes((short) (this.GetNextSample() * (double) short.MaxValue));
          Array.Copy((Array) bytes, 0, (Array) buffer, offset + index, 2);
          Array.Copy((Array) bytes, 0, (Array) buffer, offset + index + 2, 2);
        }
        return count;
      }

      protected abstract double GetNextSample();
    }
}
