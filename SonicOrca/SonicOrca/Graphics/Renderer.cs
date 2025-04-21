// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.Renderer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections.Generic;

namespace SonicOrca.Graphics
{

    public abstract class Renderer : IDisposable
    {
      private readonly HashSet<IRenderer> _registeredRenderers = new HashSet<IRenderer>();
      private readonly WindowContext _window;
      private IRenderer _currentRenderer;

      public WindowContext Window => this._window;

      public IRenderer CurrentRenderer => this._currentRenderer;

      public Renderer(WindowContext window) => this._window = window;

      public void Dispose()
      {
        foreach (IDisposable registeredRenderer in this._registeredRenderers)
          registeredRenderer.Dispose();
      }

      public void RegisterRenderer(IRenderer renderer) => this._registeredRenderers.Add(renderer);

      public void ActivateRenderer(IRenderer renderer)
      {
        if (this._currentRenderer != null && this._currentRenderer != renderer)
          this._currentRenderer.Deactivate();
        this._currentRenderer = renderer;
      }

      public void DeativateRenderer() => this.ActivateRenderer((IRenderer) null);

      public static void GetVertices(Rectangle destination, Vector2[] vertices)
      {
        vertices[0].X = destination.Left;
        vertices[0].Y = destination.Top;
        vertices[1].X = destination.Left;
        vertices[2].Y = vertices[1].Y = vertices[0].Y + destination.Height;
        vertices[3].X = vertices[2].X = vertices[0].X + destination.Width;
        vertices[3].Y = destination.Top;
      }

      public static void GetTextureMappings(
        ITexture texture,
        Rectanglei source,
        Vector2[] textureMappings,
        bool flipX = false,
        bool flipY = false)
      {
        double num1 = flipX ? (double) source.Right / (double) texture.Width : (double) source.Left / (double) texture.Width;
        double num2 = flipX ? (double) source.Left / (double) texture.Width : (double) source.Right / (double) texture.Width;
        double num3 = flipY ? (double) source.Bottom / (double) texture.Height : (double) source.Top / (double) texture.Height;
        double num4 = flipY ? (double) source.Top / (double) texture.Height : (double) source.Bottom / (double) texture.Height;
        textureMappings[0].X = num1;
        textureMappings[0].Y = num3;
        textureMappings[1].X = num1;
        textureMappings[1].Y = num4;
        textureMappings[2].X = num2;
        textureMappings[2].Y = num4;
        textureMappings[3].X = num2;
        textureMappings[3].Y = num3;
      }

      public static void GetTextureMappingsHalfIn(
        ITexture texture,
        Rectanglei source,
        ref Vector2[] textureMappings,
        bool flipX = false,
        bool flipY = false)
      {
        double num1 = flipX ? ((double) source.Right - 0.5) / (double) texture.Width : ((double) source.Left + 0.5) / (double) texture.Width;
        double num2 = flipX ? ((double) source.Left + 0.5) / (double) texture.Width : ((double) source.Right - 0.5) / (double) texture.Width;
        double num3 = flipY ? ((double) source.Bottom - 0.5) / (double) texture.Height : ((double) source.Top + 0.5) / (double) texture.Height;
        double num4 = flipY ? ((double) source.Top + 0.5) / (double) texture.Height : ((double) source.Bottom - 0.5) / (double) texture.Height;
        textureMappings[0].X = num1;
        textureMappings[0].Y = num3;
        textureMappings[1].X = num1;
        textureMappings[1].Y = num4;
        textureMappings[2].X = num2;
        textureMappings[2].Y = num4;
        textureMappings[3].X = num2;
        textureMappings[3].Y = num3;
      }

      public abstract I2dRenderer Get2dRenderer();

      public abstract IFontRenderer GetFontRenderer();

      public abstract ITileRenderer GetTileRenderer();

      public abstract IObjectRenderer GetObjectRenderer();

      public abstract ICharacterRenderer GetCharacterRenderer();

      public abstract IWaterRenderer GetWaterRenderer();

      public abstract IHeatRenderer GetHeatRenderer();

      public abstract INonLayerRenderer GetNonLayerRenderer();

      public abstract IMaskRenderer GetMaskRenderer();

      public abstract IFadeTransitionRenderer CreateFadeTransitionRenderer();
    }
}
