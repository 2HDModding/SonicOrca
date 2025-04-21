// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.TextRenderInfo
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Graphics
{

    public class TextRenderInfo
    {
      public Font Font { get; set; }

      public Rectangle Bounds { get; set; }

      public FontAlignment Alignment { get; set; }

      public Colour Colour { get; set; }

      public int? Overlay { get; set; }

      public string Text { get; set; }

      public Vector2 Shadow { get; set; }

      public Colour ShadowColour { get; set; }

      public int? ShadowOverlay { get; set; }

      public float SizeMultiplier { get; set; }

      public TextRenderInfo() => this.Colour = Colours.White;
    }
}
