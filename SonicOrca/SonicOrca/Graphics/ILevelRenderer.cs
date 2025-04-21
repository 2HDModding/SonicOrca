// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.ILevelRenderer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core;
using SonicOrca.Geometry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Graphics
{

    public interface ILevelRenderer : IDisposable
    {
      string[] LastDebugLog { get; }

      Task LoadAsync(CancellationToken ct = default (CancellationToken));

      void Initialise();

      void Render(Renderer renderer, Viewport viewport, LayerViewOptions layerViewOptions);

      void RenderToClipboard(Viewport viewport, LayerViewOptions layerViewOptions);
    }
}
