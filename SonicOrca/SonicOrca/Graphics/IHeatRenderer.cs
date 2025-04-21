// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.IHeatRenderer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;

namespace SonicOrca.Graphics
{

    public interface IHeatRenderer : IRenderer, IDisposable
    {
      ITexture DistortionTexture { get; set; }

      double DistortionAmount { get; set; }

      void Render(ITexture texture, Rectanglei destination, bool flipX = false, bool flipY = false);

      void Render(
        ITexture texture,
        Rectanglei source,
        Rectanglei destination,
        bool flipX = false,
        bool flipY = false);
    }
}
