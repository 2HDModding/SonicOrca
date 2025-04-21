// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.ITileRenderer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core;
using SonicOrca.Geometry;
using System;

namespace SonicOrca.Graphics
{

    public interface ITileRenderer : IRenderer, IDisposable
    {
      Rectangle ClipRectangle { get; set; }

      Matrix4 ModelMatrix { get; set; }

      ITexture[] Textures { get; set; }

      int Filter { get; set; }

      double FilterAmount { get; set; }

      bool Rendering { get; }

      int NumTiles { get; }

      void BeginRender();

      void AddTile(
        Rectanglei source,
        Rectanglei destination,
        int textureIndex,
        bool flipX = false,
        bool flipY = false,
        float opacity = 1f,
        TileBlendMode blend = TileBlendMode.Alpha);

      void EndRender();
    }
}
