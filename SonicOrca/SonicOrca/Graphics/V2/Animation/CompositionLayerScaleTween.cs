// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Animation.CompositionLayerScaleTween
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Collections.Generic;

namespace SonicOrca.Graphics.V2.Animation
{

    public class CompositionLayerScaleTween : CompositionLayerTween
    {
      public CompositionLayerScaleTween(
        uint startFrame,
        uint endFrame,
        KeyValuePair<double, double> startEndValuesX,
        KeyValuePair<double, double> startEndValuesY,
        KeyValuePair<double, double> startEndValuesZ)
        : base(startFrame, endFrame)
      {
        this.StartValues["X"] = startEndValuesX.Key;
        this.StartValues["Y"] = startEndValuesY.Key;
        this.StartValues["Z"] = startEndValuesZ.Key;
        this.EndValues["X"] = startEndValuesX.Value;
        this.EndValues["Y"] = startEndValuesY.Value;
        this.EndValues["Z"] = startEndValuesZ.Value;
        this.TweenType = CompositionLayerTween.Type.SCALE;
        this.ValueKeys.Add("X");
        this.ValueKeys.Add("Y");
        this.ValueKeys.Add("Z");
      }

      public CompositionLayerScaleTween(
        uint startFrame,
        uint endFrame,
        KeyValuePair<double, double> startEndValuesX,
        KeyValuePair<double, double> startEndValuesY)
        : base(startFrame, endFrame)
      {
        this.StartValues["X"] = startEndValuesX.Key;
        this.StartValues["Y"] = startEndValuesY.Key;
        this.EndValues["X"] = startEndValuesX.Value;
        this.EndValues["Y"] = startEndValuesY.Value;
        this.TweenType = CompositionLayerTween.Type.SCALE;
        this.ValueKeys.Add("X");
        this.ValueKeys.Add("Y");
      }
    }
}
