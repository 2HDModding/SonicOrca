// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelSound
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Audio;
using SonicOrca.Geometry;
using System;

namespace SonicOrca.Core
{

    public class LevelSound : IDisposable
    {
      private readonly Level _level;
      private readonly SampleInstance _instance;
      private readonly bool _autoFinish;
      private bool _paused;

      public Vector2i Position { get; set; }

      public bool Finished { get; private set; }

      public int DistanceAudible { get; set; } = 1000;

      public LevelSound(Level level, Sample sample, Vector2i position = default (Vector2i), bool autoFinish = true)
      {
        this._level = level;
        this._instance = new SampleInstance(this._level.GameContext, sample);
        this._instance.Classification = SampleInstanceClassification.Sound;
        this._autoFinish = autoFinish;
        this.Position = position;
      }

      public LevelSound(Level level, SampleInfo sampleInfo, Vector2i position = default (Vector2i), bool autoFinish = true)
      {
        this._level = level;
        this._instance = new SampleInstance(this._level.GameContext, sampleInfo);
        this._instance.Classification = SampleInstanceClassification.Sound;
        this._autoFinish = autoFinish;
        this.Position = position;
      }

      public void Dispose()
      {
        this._instance.Dispose();
        this.Finished = true;
      }

      public void Update()
      {
        this.UpdatePanAndVolume();
        if (!this._autoFinish || this._instance.Playing)
          return;
        this.Finished = true;
      }

      private void UpdatePanAndVolume()
      {
        Camera camera = this._level.Camera;
        Rectangle bounds1 = camera.Bounds;
        double x1 = bounds1.X;
        bounds1 = camera.Bounds;
        double num1 = bounds1.Width / 2.0;
        double num2 = (double) this.Position.X - (x1 + num1);
        Rectangle bounds2;
        if (Math.Abs(num2) < 512.0)
        {
          this._instance.Pan = 0.0;
        }
        else
        {
          SampleInstance instance = this._instance;
          double num3 = num2;
          bounds2 = camera.Bounds;
          double width = bounds2.Width;
          double num4 = MathX.Clamp(-1.0, num3 / width, 1.0);
          instance.Pan = num4;
        }
        double num5 = 0.0;
        double x2 = (double) this.Position.X;
        bounds2 = camera.Bounds;
        double x3 = bounds2.X;
        if (x2 < x3)
        {
          bounds2 = camera.Bounds;
          num5 = bounds2.X - (double) this.Position.X;
        }
        else
        {
          Vector2i position = this.Position;
          double x4 = (double) position.X;
          bounds2 = camera.Bounds;
          double right1 = bounds2.Right;
          if (x4 > right1)
          {
            position = this.Position;
            double x5 = (double) position.X;
            bounds2 = camera.Bounds;
            double right2 = bounds2.Right;
            num5 = x5 - right2;
          }
        }
        this._instance.Volume = MathX.Clamp(0.0, 1.0 - num5 / (double) this.DistanceAudible, 1.0);
      }

      public void Play() => this._instance.Play();

      public void Stop() => this._instance.Stop();

      public void Pause()
      {
        if (!this._instance.Playing)
          return;
        this._instance.Stop();
        this._paused = true;
      }

      public void Resume()
      {
        if (!this._paused)
          return;
        this._paused = false;
        this._instance.Play();
      }
    }
}
