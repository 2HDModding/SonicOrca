// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.IFontRenderer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Graphics
{

    public interface IFontRenderer
    {
      FontAlignment Alignment { get; set; }

      Rectangle Boundary { get; set; }

      Colour Colour { get; set; }

      Font Font { get; set; }

      Rectangle Measure();

      int Overlay { get; set; }

      void Render();

      void RenderString(
        string text,
        Rectangle boundary,
        FontAlignment fontAlignment,
        Font font,
        Colour colour,
        int? overlay = null);

      void RenderString(
        string text,
        Rectangle boundary,
        FontAlignment fontAlignment,
        Font font,
        int overlay);

      void RenderStringWithShadow(
        string text,
        Rectangle boundary,
        FontAlignment fontAlignment,
        Font font,
        Colour colour,
        int? overlay = null);

      void RenderStringWithShadow(
        string text,
        Rectangle boundary,
        FontAlignment fontAlignment,
        Font font,
        Colour colour,
        int? overlay,
        Vector2i? shadow,
        Colour shadowColour,
        int? shadowOverlay = null);

      void RenderStringWithShadow(
        string text,
        Rectangle boundary,
        FontAlignment fontAlignment,
        Font font,
        int overlay);

      Vector2 Shadow { get; set; }

      string Text { get; set; }
    }
}
