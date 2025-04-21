// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.WaveSampleGenerator
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Audio
{

    public class WaveSampleGenerator : SampleGenerator
    {
      private double _angle;

      public WaveSampleGenerator.WaveFunction Function { get; set; }

      public double Amplitude { get; set; }

      public double Frequency { get; set; }

      public WaveSampleGenerator(AudioContext audioAdapter)
        : base(audioAdapter)
      {
        this.Function = WaveSampleGenerator.WaveFunction.Sine;
        this.Amplitude = 1.0;
        this.Frequency = 256.0;
      }

      protected override double GetNextSample()
      {
        this._angle = MathX.WrapRadians(this._angle + 0.00014247585730565955 * this.Frequency);
        switch (this.Function)
        {
          case WaveSampleGenerator.WaveFunction.Sine:
            return Math.Sin(this._angle) * this.Amplitude;
          case WaveSampleGenerator.WaveFunction.Square:
            return (double) Math.Sign(this._angle / Math.PI) * this.Amplitude;
          case WaveSampleGenerator.WaveFunction.Triangle:
            return 2.0 * this.Amplitude / Math.PI * Math.Asin(Math.Sin(this._angle));
          case WaveSampleGenerator.WaveFunction.Sawtooth:
            return 2.0 * this.Amplitude / Math.PI * Math.Atan(Math.Cos(this._angle) / Math.Sin(this._angle));
          default:
            return 0.0;
        }
      }

      public enum WaveFunction
      {
        Sine,
        Square,
        Triangle,
        Sawtooth,
      }
    }
}
