// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Lighting.PointLightSource
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;

#nullable disable
namespace SonicOrca.Core.Lighting;

public class PointLightSource : ILightSource
{
  public int Intensity { get; set; }

  public Vector2i Position { get; set; }

  public PointLightSource(int intensity, Vector2i position)
  {
    this.Intensity = intensity;
    this.Position = position;
  }

  public Vector2i GetShadowOffset(Vector2i occlusionPosition, IShadowInfo shadowInfo)
  {
    int num1 = 25;
    int num2 = 16 /*0x10*/;
    int num3 = num1;
    int num4 = num1 * num2;
    Vector2i vector2i = occlusionPosition - this.Position;
    int length = vector2i.Length;
    Vector2i shadowOffset = new Vector2i();
    if (length != 0)
    {
      if (length <= num3)
      {
        int num5 = num1 * length / num3;
        shadowOffset.X = vector2i.X * num5 / length;
        shadowOffset.Y = vector2i.Y * num5 / length;
      }
      else if (length < num4)
      {
        int num6 = (num4 - length) / num2;
        shadowOffset.X = vector2i.X * num6 / length;
        shadowOffset.Y = vector2i.Y * num6 / length;
      }
    }
    return shadowOffset;
  }

  private static int ScaleOffset(int distance, int delta, int max)
  {
    if (delta > 0)
    {
      delta = Math.Max(delta, distance);
      int num = delta / 8;
      return Math.Max(max - num, 0);
    }
    delta = Math.Min(delta, -distance);
    int num1 = delta / 8;
    return Math.Min(-max - num1, 0);
  }

  public override string ToString()
  {
    Vector2i position = this.Position;
    // ISSUE: variable of a boxed type
    __Boxed<int> x = (ValueType) position.X;
    position = this.Position;
    // ISSUE: variable of a boxed type
    __Boxed<int> y = (ValueType) position.Y;
    return $"PointLight ({x}, {y})";
  }
}
