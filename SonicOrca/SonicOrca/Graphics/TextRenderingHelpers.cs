// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.TextRenderingHelpers
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System.Collections.Generic;

#nullable disable
namespace SonicOrca.Graphics;

public static class TextRenderingHelpers
{
  public static Rectangle RenderWith2d(I2dRenderer g, TextRenderInfo textRenderInfo)
  {
    if (string.IsNullOrEmpty(textRenderInfo.Text) || textRenderInfo.Colour.Alpha == (byte) 0)
      return new Rectangle();
    if (textRenderInfo.Shadow != new Vector2())
    {
      TextRenderInfo textRenderInfo1 = new TextRenderInfo()
      {
        Font = textRenderInfo.Font,
        Bounds = textRenderInfo.Bounds.OffsetBy(textRenderInfo.Shadow),
        Alignment = textRenderInfo.Alignment,
        Colour = textRenderInfo.ShadowColour,
        Overlay = textRenderInfo.ShadowOverlay,
        Text = textRenderInfo.Text
      };
    }
    return TextRenderingHelpers.RenderWith2dInternal(g, textRenderInfo);
  }

  private static Rectangle RenderWith2dInternal(I2dRenderer g, TextRenderInfo textRenderInfo)
  {
    Font font = textRenderInfo.Font;
    string text = textRenderInfo.Text;
    Colour colour = textRenderInfo.Colour;
    int? overlay = textRenderInfo.Overlay;
    Rectangle rectangle = font.MeasureString(text, textRenderInfo.Bounds, textRenderInfo.Alignment);
    switch (textRenderInfo.Alignment & FontAlignment.HorizontalMask)
    {
      case FontAlignment.MiddleX:
        rectangle.X += rectangle.Width * (double) textRenderInfo.SizeMultiplier / 2.0;
        break;
      case FontAlignment.Right:
        rectangle.X += rectangle.Width * (double) textRenderInfo.SizeMultiplier;
        break;
    }
    switch (textRenderInfo.Alignment & FontAlignment.VerticalMask)
    {
      case FontAlignment.MiddleY:
        double num = (double) font.Height * (double) textRenderInfo.SizeMultiplier;
        rectangle.Y += (rectangle.Height - num) / 2.0;
        break;
      case FontAlignment.Bottom:
        rectangle.X += rectangle.Height * (double) textRenderInfo.SizeMultiplier;
        break;
    }
    Vector2 destination = new Vector2(rectangle.X, rectangle.Y);
    foreach (char key in text)
    {
      Font.CharacterDefinition characterDefinition = font[key];
      if (characterDefinition == null)
      {
        destination.X += (double) font.DefaultWidth * (double) textRenderInfo.SizeMultiplier;
      }
      else
      {
        TextRenderingHelpers.RenderCharacter(g, font, characterDefinition, destination, colour, overlay, textRenderInfo.SizeMultiplier);
        destination.X += (double) characterDefinition.Width * (double) textRenderInfo.SizeMultiplier;
      }
      destination.X += (double) font.Tracking * (double) textRenderInfo.SizeMultiplier;
    }
    return rectangle;
  }

  private static void RenderCharacter(
    I2dRenderer g,
    Font font,
    Font.CharacterDefinition characterDefinition,
    Vector2 destination,
    Colour colour,
    int? overlay,
    float sizeMultiplier)
  {
    ITexture[] texture = new ITexture[2]
    {
      font.ShapeTexture,
      null
    };
    if (overlay.HasValue)
      texture[1] = font.OverlayTextures[overlay.Value];
    Rectangle sourceRectangle = (Rectangle) characterDefinition.SourceRectangle;
    Rectangle destination1;
    ref Rectangle local = ref destination1;
    double x1 = destination.X;
    Vector2i offset = characterDefinition.Offset;
    double num1 = (double) offset.X * (double) sizeMultiplier;
    double x2 = x1 + num1;
    double y1 = destination.Y;
    offset = characterDefinition.Offset;
    double num2 = (double) offset.Y * (double) sizeMultiplier;
    double y2 = y1 + num2;
    double width = sourceRectangle.Width * (double) sizeMultiplier;
    double height = sourceRectangle.Height * (double) sizeMultiplier;
    local = new Rectangle(x2, y2, width, height);
    g.BlendMode = BlendMode.Alpha;
    g.Colour = colour;
    if (overlay.HasValue)
      g.RenderTexture((IEnumerable<ITexture>) texture, sourceRectangle, destination1);
    else
      g.RenderTexture(texture[0], sourceRectangle, destination1);
  }

  public static Rectangle MeasureWith2d(I2dRenderer g, TextRenderInfo textRenderInfo)
  {
    Font font = textRenderInfo.Font;
    string text = textRenderInfo.Text;
    Colour colour = textRenderInfo.Colour;
    int? overlay = textRenderInfo.Overlay;
    Rectangle rectangle = font.MeasureString(text, textRenderInfo.Bounds, textRenderInfo.Alignment);
    switch (textRenderInfo.Alignment & FontAlignment.HorizontalMask)
    {
      case FontAlignment.MiddleX:
        rectangle.X += rectangle.Width * (double) textRenderInfo.SizeMultiplier / 2.0;
        break;
      case FontAlignment.Right:
        rectangle.X += rectangle.Width * (double) textRenderInfo.SizeMultiplier;
        break;
    }
    switch (textRenderInfo.Alignment & FontAlignment.VerticalMask)
    {
      case FontAlignment.Right:
        rectangle.X += rectangle.Height * (double) textRenderInfo.SizeMultiplier;
        break;
      case FontAlignment.MiddleY:
        rectangle.Y += rectangle.Height * (double) textRenderInfo.SizeMultiplier / 2.0;
        break;
    }
    Vector2 vector2 = new Vector2(rectangle.X, rectangle.Y);
    foreach (char key in text)
    {
      Font.CharacterDefinition characterDefinition = font[key];
      if (characterDefinition == null)
        vector2.X += (double) font.DefaultWidth * (double) textRenderInfo.SizeMultiplier;
      else
        vector2.X += (double) characterDefinition.Width * (double) textRenderInfo.SizeMultiplier;
      vector2.X += (double) font.Tracking * (double) textRenderInfo.SizeMultiplier;
    }
    return rectangle;
  }
}
