// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.IMaskRenderer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;

namespace SonicOrca.Graphics
{

    public interface IMaskRenderer : IDisposable
    {
      ITexture Texture { get; set; }

      Rectanglei Source { get; set; }

      Rectanglei Destination { get; set; }

      ITexture MaskTexture { get; set; }

      Rectanglei MaskSource { get; set; }

      Rectanglei MaskDestination { get; set; }

      BlendMode BlendMode { get; set; }

      Colour Colour { get; set; }

      Matrix4 IntersectionModelMatrix { get; set; }

      Matrix4 TargetModelMatrix { get; set; }

      Matrix4 MaskModelMatrix { get; set; }

      IDisposable BeginMatixState();

      void Render(bool maskColorMultiply = false);
    }
}
