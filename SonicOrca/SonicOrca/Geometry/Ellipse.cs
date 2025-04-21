// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.Ellipse
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Geometry
{

    public struct Ellipse : IEquatable<Ellipse>
    {
      public static Ellipse Empty => new Ellipse();

      public Vector2 Position { get; set; }

      public Vector2 Radius { get; set; }

      public double Area => Math.PI * this.Radius.X * this.Radius.Y;

      public Ellipse(double x, double y, double radiusX, double radiusY)
        : this()
      {
        this.Position = new Vector2(x, y);
        this.Radius = new Vector2(radiusX, radiusY);
      }

      public bool Contains(Vector2 p)
      {
        if (this.Radius.X <= 0.0 || this.Radius.Y <= 0.0)
          return false;
        Vector2 vector2_1;
        ref Vector2 local = ref vector2_1;
        double x1 = p.X;
        Vector2 vector2_2 = this.Position;
        double x2 = vector2_2.X;
        double x3 = x1 - x2;
        double y1 = p.Y;
        vector2_2 = this.Position;
        double y2 = vector2_2.Y;
        double y3 = y1 - y2;
        local = new Vector2(x3, y3);
        double num1 = vector2_1.X * vector2_1.X;
        vector2_2 = this.Radius;
        double x4 = vector2_2.X;
        vector2_2 = this.Radius;
        double x5 = vector2_2.X;
        double num2 = x4 * x5;
        double num3 = num1 / num2;
        double num4 = vector2_1.Y * vector2_1.Y;
        vector2_2 = this.Radius;
        double y4 = vector2_2.Y;
        vector2_2 = this.Radius;
        double y5 = vector2_2.Y;
        double num5 = y4 * y5;
        double num6 = num4 / num5;
        return num3 + num6 <= 1.0;
      }

      public override bool Equals(object obj) => this.Equals((Ellipse) obj);

      public bool Equals(Ellipse other)
      {
        Vector2 position1 = other.Position;
        double x1 = position1.X;
        position1 = this.Position;
        double x2 = position1.X;
        if (x1 == x2)
        {
          Vector2 position2 = other.Position;
          double y1 = position2.Y;
          position2 = this.Position;
          double y2 = position2.Y;
          if (y1 == y2)
          {
            Vector2 radius1 = other.Radius;
            double x3 = radius1.X;
            radius1 = this.Radius;
            double x4 = radius1.X;
            if (x3 == x4)
            {
              Vector2 radius2 = other.Radius;
              double y3 = radius2.Y;
              radius2 = this.Radius;
              double y4 = radius2.Y;
              return y3 == y4;
            }
          }
        }
        return false;
      }

      public override int GetHashCode()
      {
        return (((13 * 7 + this.Position.X.GetHashCode()) * 7 + this.Position.Y.GetHashCode()) * 7 + this.Radius.X.GetHashCode()) * 7 + this.Radius.Y.GetHashCode();
      }

      public override string ToString()
      {
        return $"X = {this.Position.X} Y = {this.Position.Y} RadiusX = {this.Radius.X} RadiusY = {this.Radius.Y}";
      }
    }
}
