// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.Vector4
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Geometry
{

    public struct Vector4 : IEquatable<Vector4>
    {
      public double X { get; set; }

      public double Y { get; set; }

      public double Z { get; set; }

      public double W { get; set; }

      public Vector2 XY => new Vector2(this.X, this.Y);

      public Vector3 XYZ => new Vector3(this.X, this.Y, this.Z);

      public Vector4 Normalised => this / this.Length;

      public double Length
      {
        get
        {
          return Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W * this.W);
        }
      }

      public static Vector4 UnitX => new Vector4(1.0, 0.0, 0.0, 0.0);

      public static Vector4 UnitY => new Vector4(0.0, 1.0, 0.0, 0.0);

      public static Vector4 UnitZ => new Vector4(0.0, 0.0, 1.0, 0.0);

      public static Vector4 UnitW => new Vector4(0.0, 0.0, 0.0, 1.0);

      public Vector4(double xyzw)
        : this(xyzw, xyzw, xyzw, xyzw)
      {
      }

      public Vector4(double x, double y, double z, double w)
        : this()
      {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.W = w;
      }

      public override bool Equals(object obj) => this.Equals((Vector4) obj);

      public bool Equals(Vector4 p) => p.X == this.X && p.Y == this.Y && p.Z == this.Z && p.W == this.W;

      public override int GetHashCode()
      {
        return (((13 * 7 + this.X.GetHashCode()) * 7 + this.Y.GetHashCode()) * 7 + this.Z.GetHashCode()) * 7 + this.W.GetHashCode();
      }

      public override string ToString() => $"X = {this.X} Y = {this.Y} Z = {this.Z} W = {this.W}";

      public static Vector4 operator +(Vector4 a, Vector4 b)
      {
        return new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
      }

      public static Vector4 operator -(Vector4 a, Vector4 b)
      {
        return new Vector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
      }

      public static Vector4 operator -(Vector4 v) => new Vector4(-v.X, -v.Y, -v.Z, -v.W);

      public static Vector4 operator *(Vector4 a, Vector4 b)
      {
        return new Vector4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
      }

      public static Vector4 operator *(Vector4 v, double s)
      {
        return new Vector4(v.X * s, v.Y * s, v.Z * s, v.W * s);
      }

      public static Vector4 operator *(double s, Vector4 v)
      {
        return new Vector4(v.X * s, v.Y * s, v.Z * s, v.W * s);
      }

      public static Vector4 operator /(Vector4 a, Vector4 b)
      {
        return new Vector4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
      }

      public static Vector4 operator /(Vector4 v, double s)
      {
        return new Vector4(v.X / s, v.Y / s, v.Z / s, v.W / s);
      }

      public static Vector4 operator /(double s, Vector4 v)
      {
        return new Vector4(v.X / s, v.Y / s, v.Z / s, v.W / s);
      }

      public static bool operator ==(Vector4 a, Vector4 b) => a.Equals(b);

      public static bool operator !=(Vector4 a, Vector4 b) => !a.Equals(b);
    }
}
