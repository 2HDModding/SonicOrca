// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.I2dRenderer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections.Generic;

namespace SonicOrca.Graphics
{

    public interface I2dRenderer : IDisposable, IRenderer
    {
      BlendMode BlendMode { get; set; }

      Rectangle ClipRectangle { get; set; }

      Colour Colour { get; set; }

      Colour AdditiveColour { get; set; }

      Matrix4 ModelMatrix { get; set; }

      IDisposable BeginMatixState();

      void RenderQuad(Colour colour, Rectangle destination);

      void RenderEllipse(
        Colour colour,
        Vector2 centre,
        double innerRadius,
        double outerRadius,
        int sectors);

      void RenderRectangle(Colour colour, Rectangle destination, double tickness);

      void RenderLine(Colour colour, Vector2 a, Vector2 b, double thickness);

      void RenderTexture(ITexture texture, Rectangle destination, bool flipx = false, bool flipy = false);

      void RenderTexture(ITexture texture, Vector2 destination, bool flipx = false, bool flipy = false);

      void RenderTexture(
        ITexture texture,
        Rectangle source,
        Rectangle destination,
        bool flipx = false,
        bool flipy = false);

      void RenderTexture(
        IEnumerable<ITexture> texture,
        Rectangle source,
        Rectangle destination,
        bool flipx = false,
        bool flipy = false);

      Rectangle RenderText(TextRenderInfo textRenderInfo);

      Rectangle MeasureText(TextRenderInfo textRenderInfo);
    }
}
