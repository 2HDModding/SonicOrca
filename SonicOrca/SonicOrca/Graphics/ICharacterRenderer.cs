// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.ICharacterRenderer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;

namespace SonicOrca.Graphics
{

    public interface ICharacterRenderer : IRenderer, IDisposable
    {
      Rectangle ClipRectangle { get; set; }

      Matrix4 ModelMatrix { get; set; }

      int Filter { get; set; }

      double FilterAmount { get; set; }

      float Brightness { get; set; }

      void RenderTexture(
        ITexture skinTexture,
        ITexture bodyTexture,
        Rectangle source,
        Rectangle destination,
        bool flipX = false,
        bool flipY = false);

      void RenderTexture(
        ITexture skinTexture,
        ITexture bodyTexture,
        double hueShift,
        double satuationShift,
        double luminosityShift,
        Rectangle source,
        Rectangle destination,
        bool flipX = false,
        bool flipY = false);

      void RenderTextureGhost(
        ITexture skinTexture,
        ITexture bodyTexture,
        Rectangle source,
        Rectangle destination,
        bool flipX = false,
        bool flipY = false);
    }
}
