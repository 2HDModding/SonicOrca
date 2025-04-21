// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.IWaterRenderer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;

namespace SonicOrca.Graphics
{

    public interface IWaterRenderer : IRenderer, IDisposable
    {
      double HueTarget { get; set; }

      double HueAmount { get; set; }

      double SaturationChange { get; set; }

      double LuminosityChange { get; set; }

      double WavePhase { get; set; }

      double NumWaves { get; set; }

      double WaveSize { get; set; }

      float Time { get; set; }

      void Render(Rectanglei bounds);
    }
}
