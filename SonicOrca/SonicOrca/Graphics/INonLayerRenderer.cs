// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.INonLayerRenderer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections.Generic;

namespace SonicOrca.Graphics
{

    public interface INonLayerRenderer : IRenderer, IDisposable
    {
      void Render(IEnumerable<Rectanglei> rects);
    }
}
