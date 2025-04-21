// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.AudioContext
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;

namespace SonicOrca.Audio
{

    public abstract class AudioContext
    {
      private readonly List<SampleInstance> _fireAndForgetSoundInstances = new List<SampleInstance>();

      public ISampleMixer Mixer { get; set; }

      public double Volume { get; set; }

      public double MusicVolume { get; set; }

      public double SoundVolume { get; set; }

      protected AudioContext()
      {
        this.Volume = 1.0;
        this.MusicVolume = 0.3;
        this.SoundVolume = 1.0;
        this.Mixer = (ISampleMixer) new BasicSampleMixer();
      }

      public virtual void RegisterSampleProvider(ISampleProvider sampleProvider)
      {
      }

      public virtual void UnregisterSampleProvider(ISampleProvider sampleProvider)
      {
      }

      public virtual void Update()
      {
        this._fireAndForgetSoundInstances.RemoveAll((Predicate<SampleInstance>) (x => !x.Playing));
      }

      public void PlaySound(Sample sample)
      {
        SampleInstance sampleInstance = new SampleInstance(this, sample);
        sampleInstance.Play();
        this._fireAndForgetSoundInstances.Add(sampleInstance);
      }

      public void StopAll()
      {
        foreach (SampleInstance forgetSoundInstance in this._fireAndForgetSoundInstances)
          forgetSoundInstance.Stop();
      }
    }
}
