// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.Vector2
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Globalization;

namespace SonicOrca.Geometry
{

    public struct Vector2 : IEquatable<Vector2>
    {
      public double X { get; set; }

      public double Y { get; set; }

      public Vector2 Normalised => this / this.Length;

      public Vector2(double xy)
        : this(xy, xy)
      {
      }

      public Vector2(double x, double y)
        : this()
      {
        this.X = x;
        this.Y = y;
      }

      public override bool Equals(object obj) => this.Equals((Vector2) obj);

      public bool Equals(Vector2 p) => p.X == this.X && p.Y == this.Y;

      public override int GetHashCode() => (13 * 7 + this.X.GetHashCode()) * 7 + this.Y.GetHashCode();

      public override string ToString()
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "X = {0}, Y = {1}", (object) this.X, (object) this.Y);
      }

      public Vector2 Rotate(double radians)
      {
        double num1 = Math.Cos(radians);
        double num2 = Math.Sin(radians);
        return new Vector2(this.X * num1 - this.Y * num2, this.X * num2 + this.Y * num1);
      }

      public Vector2 Rotate(double radians, Vector2 origin) => (this + origin).Rotate(radians) - origin;

      public Vector2 Reflect(Vector2 normal) => this - 2.0 * this.Dot(normal) * normal;

      public double Dot(Vector2 v) => this.X * v.X + this.Y * v.Y;

      public double Cross(Vector2 v) => this.X * v.Y - this.Y * v.X;

      public double Length => Math.Sqrt(this.X * this.X + this.Y * this.Y);

      public double LengthSquared => Math.Pow(this.X * this.X + this.Y * this.Y, 2.0);

      public double Angle => Math.Atan2(this.Y, this.X);

      public static Vector2 GetPointRotatedFromRelative(Vector2 relative, Vector2 point, double theta)
      {
        double x1 = point.X;
        double y1 = point.Y;
        double x2 = relative.X;
        double y2 = relative.Y;
        return new Vector2(Math.Cos(theta) * (x1 - x2) - Math.Sin(theta) * (y1 - y2) + x2, Math.Sin(theta) * (x1 - x2) + Math.Cos(theta) * (y1 - y2) + y2);
      }

      public static Vector2 FromAngle(double radians)
      {
        return new Vector2(Math.Cos(radians), Math.Sin(radians));
      }

      public static double GetDistance(Vector2 pointA, Vector2 pointB)
      {
        return Math.Sqrt(Math.Pow(pointB.X - pointA.X, 2.0) + Math.Pow(pointB.Y - pointA.Y, 2.0));
      }

      public static bool Intersects(
        Vector2 line0point0,
        Vector2 line0point1,
        Vector2 line1point0,
        Vector2 line1point1)
      {
        Vector2 vector2_1 = new Vector2(line0point1.X - line0point0.X, line0point1.Y - line0point0.Y);
        Vector2 vector2_2 = new Vector2(line1point1.X - line1point0.X, line1point1.Y - line1point0.Y);
        double num1 = (-vector2_1.Y * (line0point0.X - line1point0.X) + vector2_1.X * (line0point0.Y - line1point0.Y)) / (-vector2_2.X * vector2_1.Y + vector2_1.X * vector2_2.Y);
        double num2 = (vector2_2.X * (line0point0.Y - line1point0.Y) - vector2_2.Y * (line0point0.X - line1point0.X)) / (-vector2_2.X * vector2_1.Y + vector2_1.X * vector2_2.Y);
        return num1 >= 0.0 && num1 <= 1.0 && num2 >= 0.0 && num2 <= 1.0;
      }

      public static bool GetLineIntersection(
        Vector2 A,
        Vector2 B,
        Vector2 C,
        Vector2 D,
        out Vector2 intersection)
      {
        double num1 = B.Y - A.Y;
        double num2 = A.X - B.X;
        double num3 = num1 * A.X + num2 * A.Y;
        double num4 = D.Y - C.Y;
        double num5 = C.X - D.X;
        double num6 = num4 * C.X + num5 * C.Y;
        double num7 = num1 * num5 - num4 * num2;
        if (num7 == 0.0)
        {
          intersection = new Vector2(double.MaxValue, double.MaxValue);
          return false;
        }
        double x = (num5 * num3 - num2 * num6) / num7;
        double y = (num1 * num6 - num4 * num3) / num7;
        intersection = new Vector2(x, y);
        return true;
      }

      public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);

      public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);

      public static Vector2 operator -(Vector2 a) => new Vector2(-a.X, -a.Y);

      public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.X * b.X, a.Y * b.Y);

      public static double Multiply(Vector2 v, Vector2 w) => v.X * w.X + v.Y * w.Y;

      public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.X / b.X, a.Y / b.Y);

      public static Vector2 operator *(Vector2 v, double x) => new Vector2(v.X * x, v.Y * x);

      public static Vector2 operator *(double x, Vector2 v) => new Vector2(v.X * x, v.Y * x);

      public static Vector2 operator /(Vector2 v, double x) => new Vector2(v.X / x, v.Y / x);

      public static Vector2 operator /(double x, Vector2 v) => new Vector2(v.X / x, v.Y / x);

      public static bool operator ==(Vector2 a, Vector2 b) => a.Equals(b);

      public static bool operator !=(Vector2 a, Vector2 b) => !a.Equals(b);
    }
}
