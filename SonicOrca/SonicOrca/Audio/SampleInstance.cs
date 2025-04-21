// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.SampleInstance
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace SonicOrca.Audio
{

    public class SampleInstance : IDisposable, ISampleProvider
    {
      private readonly AudioContext _audioAdapter;
      private readonly Sample _sample;
      private Stream _sampleInputStream;
      private byte[] _lastReadBytes;
      private int? _loopSampleIndex;

      public double Volume { get; set; }

      public double Pan { get; set; }

      public SampleInstanceClassification Classification { get; set; } = SampleInstanceClassification.Sound;

      public Sample Sample => this._sample;

      public bool Playing { get; private set; }

      public int SampleIndex { get; private set; }

      public double Position => (double) this.SampleIndex / (double) this._sample.SampleRate;

      public IReadOnlyList<byte> LastReadBytes => (IReadOnlyList<byte>) this._lastReadBytes;

      private int? LoopSampleIndex
      {
        get => this._loopSampleIndex;
        set
        {
          int? loopSampleIndex = this._loopSampleIndex;
          int? nullable = value;
          if ((loopSampleIndex.GetValueOrDefault() == nullable.GetValueOrDefault() ? (loopSampleIndex.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
            return;
          this._loopSampleIndex = value;
          this.CreateSampleInputStream();
        }
      }

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

      public SampleInstance(SonicOrcaGameContext context, SampleInfo sampleInfo)
        : this(context.Audio, sampleInfo.Sample, sampleInfo.HasLoopPoint ? new int?(sampleInfo.LoopSampleIndex) : new int?())
      {
      }

      public SampleInstance(AudioContext audioAdapter, SampleInfo sampleInfo)
        : this(audioAdapter, sampleInfo.Sample, sampleInfo.HasLoopPoint ? new int?(sampleInfo.LoopSampleIndex) : new int?())
      {
      }

      public SampleInstance(SonicOrcaGameContext context, Sample sample, int? loopSampleIndex = null)
        : this(context.Audio, sample, loopSampleIndex)
      {
      }

      public SampleInstance(AudioContext audioAdapter, Sample sample, int? loopSampleIndex = null)
      {
        this._audioAdapter = audioAdapter;
        this._sample = sample;
        this._loopSampleIndex = loopSampleIndex;
        this.CreateSampleInputStream();
        this.Volume = 1.0;
        this._audioAdapter.RegisterSampleProvider((ISampleProvider) this);
      }

      private void CreateSampleInputStream()
      {
        this._sampleInputStream = (Stream) new SampleStream(this._sample, this._loopSampleIndex);
        if (this._sample.SampleRate == 44100)
          return;
        this._sampleInputStream = (Stream) new ResamplerStream(this._sampleInputStream, this._sample.SampleRate, 44100);
      }

      public void Dispose()
      {
        this._audioAdapter.UnregisterSampleProvider((ISampleProvider) this);
        this._sampleInputStream.Dispose();
      }

      public void Play() => this.Playing = true;

      public void Stop() => this.Playing = false;

      public void SeekToStart() => this.SeekTo(0);

      public void SeekToLoopPoint() => this.SeekTo(this._loopSampleIndex ?? 0);

      public void SeekTo(int sampleIndex)
      {
        this._sampleInputStream.Position = this._sample.GetPcmDataOffset(sampleIndex);
        this.SampleIndex = sampleIndex;
      }

      public void SeekTo(double time) => this.SeekTo((int) (time * (double) this._sample.SampleRate));

      public int Read(byte[] buffer, int offset, int count)
      {
        this.SampleIndex = this._sample.GetSampleIndex(this._sampleInputStream.Position);
        int length = this._sampleInputStream.Read(buffer, offset, count);
        if (length == 0)
          this.Playing = false;
        this._lastReadBytes = buffer.GetRange<byte>(0, length);
        return length;
      }
    }
}
