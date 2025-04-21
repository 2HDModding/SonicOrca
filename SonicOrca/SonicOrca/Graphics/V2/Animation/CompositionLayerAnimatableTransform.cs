// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Animation.CompositionLayerAnimatableTransform
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Graphics.V2.Animation
{

    public class CompositionLayerAnimatableTransform
    {
      private uint _ellapsedFrames;
      private List<CompositionLayerTween> _tweens = new List<CompositionLayerTween>();

      public List<CompositionLayerTween> Tweens => this._tweens;

      private CompositionLayerTween GetCurrentTween(CompositionLayerTween.Type tweenType)
      {
        return this._tweens.Select<CompositionLayerTween, CompositionLayerTween>((Func<CompositionLayerTween, CompositionLayerTween>) (t => t)).Where<CompositionLayerTween>((Func<CompositionLayerTween, bool>) (t => t.TweenType == tweenType && this._ellapsedFrames >= t.StartFrame && this._ellapsedFrames <= t.EndFrame)).LastOrDefault<CompositionLayerTween>();
      }

      public CompositionLayerTween GetFirstTween()
      {
        return this._tweens.Select<CompositionLayerTween, CompositionLayerTween>((Func<CompositionLayerTween, CompositionLayerTween>) (t => t)).OrderBy<CompositionLayerTween, uint>((Func<CompositionLayerTween, uint>) (t => t.StartFrame)).First<CompositionLayerTween>();
      }

      private CompositionLayerTween GetFirstTween(CompositionLayerTween.Type tweenType)
      {
        return this._tweens.Select<CompositionLayerTween, CompositionLayerTween>((Func<CompositionLayerTween, CompositionLayerTween>) (t => t)).Where<CompositionLayerTween>((Func<CompositionLayerTween, bool>) (t => t.TweenType == tweenType && this._ellapsedFrames <= t.StartFrame)).OrderBy<CompositionLayerTween, uint>((Func<CompositionLayerTween, uint>) (t => t.StartFrame)).FirstOrDefault<CompositionLayerTween>();
      }

      private CompositionLayerTween GetLastTween(CompositionLayerTween.Type tweenType)
      {
        return this._tweens.Select<CompositionLayerTween, CompositionLayerTween>((Func<CompositionLayerTween, CompositionLayerTween>) (t => t)).Where<CompositionLayerTween>((Func<CompositionLayerTween, bool>) (t => t.TweenType == tweenType)).OrderBy<CompositionLayerTween, uint>((Func<CompositionLayerTween, uint>) (t => t.EndFrame)).Last<CompositionLayerTween>();
      }

      public double Opacity
      {
        get
        {
          return (this.GetCurrentTween(CompositionLayerTween.Type.OPACITY) ?? this.GetFirstTween(CompositionLayerTween.Type.OPACITY) ?? this.GetLastTween(CompositionLayerTween.Type.OPACITY)).GetValueSet()[nameof (Opacity)];
        }
      }

      public Vector2 Position
      {
        get
        {
          Dictionary<string, double> valueSet = (this.GetCurrentTween(CompositionLayerTween.Type.POSITION) ?? this.GetFirstTween(CompositionLayerTween.Type.POSITION) ?? this.GetLastTween(CompositionLayerTween.Type.POSITION)).GetValueSet();
          return new Vector2(valueSet["X"], valueSet["Y"]);
        }
      }

      public double Rotation
      {
        get
        {
          return MathX.ToRadians((this.GetCurrentTween(CompositionLayerTween.Type.ROTATION) ?? this.GetFirstTween(CompositionLayerTween.Type.ROTATION) ?? this.GetLastTween(CompositionLayerTween.Type.ROTATION)).GetValueSet()[nameof (Rotation)]);
        }
      }

      public Vector2 Scale
      {
        get
        {
          Dictionary<string, double> valueSet = (this.GetCurrentTween(CompositionLayerTween.Type.SCALE) ?? this.GetFirstTween(CompositionLayerTween.Type.SCALE) ?? this.GetLastTween(CompositionLayerTween.Type.SCALE)).GetValueSet();
          return new Vector2(valueSet["X"], valueSet["Y"]);
        }
      }

      public void ResetFrame()
      {
        this._ellapsedFrames = 0U;
        foreach (CompositionLayerTween tween in this._tweens)
          tween.ResetFrame();
      }

      public void Animate()
      {
        uint num1 = 0;
        uint num2 = 0;
        foreach (CompositionLayerTween tween in this._tweens)
        {
          if (this._ellapsedFrames <= tween.EndFrame)
            tween.Animate();
          if (tween.StartFrame <= num1)
            num1 = tween.StartFrame;
          if (tween.EndFrame >= num2)
            num2 = tween.EndFrame;
        }
        if (this._ellapsedFrames > num2)
          return;
        ++this._ellapsedFrames;
      }

      public void AddKeyFrameTween(CompositionLayerTween tween) => this._tweens.Add(tween);
    }
}
