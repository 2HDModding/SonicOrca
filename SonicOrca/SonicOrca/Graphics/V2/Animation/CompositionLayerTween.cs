// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Animation.CompositionLayerTween
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Collections.Generic;

namespace SonicOrca.Graphics.V2.Animation
{

    public class CompositionLayerTween
    {
      private bool _hasKeyframes = true;
      private uint _duration;
      private uint _startFrame;
      private uint _endFrame;
      private uint _currentFrame;
      private uint _ellapsedFrames;
      private Dictionary<string, double> _startValues = new Dictionary<string, double>();
      private Dictionary<string, double> _endValues = new Dictionary<string, double>();
      protected List<string> _valueKeys = new List<string>();

      public CompositionLayerTween.Type TweenType { get; set; }

      public Dictionary<string, double> StartValues => this._startValues;

      public Dictionary<string, double> EndValues => this._endValues;

      public List<string> ValueKeys => this._valueKeys;

      public uint Duration => this._duration;

      public uint CurrentFrame
      {
        get => this._currentFrame;
        protected set => this._currentFrame = value;
      }

      public uint StartFrame => this._startFrame;

      public uint EndFrame => this._endFrame;

      public bool HasKeyFrames
      {
        get => this._hasKeyframes;
        set => this._hasKeyframes = value;
      }

      public CompositionLayerTween(uint startFrame, uint endFrame)
      {
        this._startFrame = startFrame;
        this._endFrame = endFrame;
        this._currentFrame = 0U;
        this._duration = (uint) (1 + ((int) this._endFrame - (int) this._startFrame));
      }

      public void ResetFrame()
      {
        this._ellapsedFrames = 0U;
        this._currentFrame = 0U;
      }

      public void Animate()
      {
        if (this._ellapsedFrames < this._endFrame)
          ++this._ellapsedFrames;
        if (this._ellapsedFrames < this._startFrame || this._currentFrame >= this._duration)
          return;
        ++this._currentFrame;
      }

      public Dictionary<string, double> GetValueSet()
      {
        Dictionary<string, double> valueSet = new Dictionary<string, double>();
        foreach (string valueKey in this.ValueKeys)
        {
          double num = this.EndValues[valueKey] - this.StartValues[valueKey];
          valueSet[valueKey] = num == 0.0 ? this.StartValues[valueKey] : this.StartValues[valueKey] + num / (double) this._duration * (double) this._currentFrame;
        }
        return valueSet;
      }

      public enum Type
      {
        OPACITY,
        POSITION,
        ROTATION,
        SCALE,
      }
    }
}
