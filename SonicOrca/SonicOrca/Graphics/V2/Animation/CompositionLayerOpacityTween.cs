// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Animation.CompositionLayerOpacityTween
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Collections.Generic;

namespace SonicOrca.Graphics.V2.Animation
{

    public class CompositionLayerOpacityTween : CompositionLayerTween
    {
      public CompositionLayerOpacityTween(
        uint startFrame,
        uint endFrame,
        KeyValuePair<double, double> startEndValues)
        : base(startFrame, endFrame)
      {
        this.StartValues["Opacity"] = startEndValues.Key;
        this.EndValues["Opacity"] = startEndValues.Value;
        this.TweenType = CompositionLayerTween.Type.OPACITY;
        this.ValueKeys.Add("Opacity");
      }
    }
}
