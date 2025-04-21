// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.Vector3
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Geometry
{

    public struct Vector3 : IEquatable<Vector3>
    {
      public double X { get; set; }

      public double Y { get; set; }

      public double Z { get; set; }

      public Vector2 XY => new Vector2(this.X, this.Y);

      public Vector3 Normalised => this / this.Length;

      public double Length => Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);

      public Vector3(double xyz)
        : this(xyz, xyz, xyz)
      {
      }

      public Vector3(double x, double y, double z)
        : this()
      {
        this.X = x;
        this.Y = y;
        this.Z = z;
      }

      public override bool Equals(object obj) => this.Equals((Vector3) obj);

      public bool Equals(Vector3 p) => p.X == this.X && p.Y == this.Y && p.Z == this.Z;

      public override int GetHashCode()
      {
        return ((13 * 7 + this.X.GetHashCode()) * 7 + this.Y.GetHashCode()) * 7 + this.Z.GetHashCode();
      }

      public override string ToString() => $"X = {this.X} Y = {this.Y} Z = {this.Z}";

      public Vector3 Cross(Vector3 other)
      {
        return new Vector3(this.Y * other.Z - this.Z * other.Y, this.Z * other.X - this.X * other.Z, this.X * other.Y - this.Y * other.X);
      }

      public static Vector3 operator +(Vector3 a, Vector3 b)
      {
        return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
      }

      public static Vector3 operator -(Vector3 a, Vector3 b)
      {
        return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
      }

      public static Vector3 operator -(Vector3 v) => new Vector3(-v.X, -v.Y, -v.Z);

      public static Vector3 operator *(Vector3 a, Vector3 b)
      {
        return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
      }

      public static Vector3 operator *(Vector3 v, double s) => new Vector3(v.X * s, v.Y * s, v.Z * s);

      public static Vector3 operator *(double s, Vector3 v) => new Vector3(v.X * s, v.Y * s, v.Z * s);

      public static Vector3 operator /(Vector3 a, Vector3 b)
      {
        return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
      }

      public static Vector3 operator /(Vector3 v, double s) => new Vector3(v.X / s, v.Y / s, v.Z / s);

      public static Vector3 operator /(double s, Vector3 v) => new Vector3(v.X / s, v.Y / s, v.Z / s);
    }
}
