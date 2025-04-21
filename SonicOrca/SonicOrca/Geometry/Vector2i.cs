// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.Vector2i
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Geometry
{

    public struct Vector2i : IEquatable<Vector2i>
    {
      public int X { get; set; }

      public int Y { get; set; }

      public Vector2i(int xy)
        : this(xy, xy)
      {
      }

      public Vector2i(int x, int y)
        : this()
      {
        this.X = x;
        this.Y = y;
      }

      public override bool Equals(object obj) => this.Equals((Vector2i) obj);

      public bool Equals(Vector2i p) => p.X == this.X && p.Y == this.Y;

      public override int GetHashCode() => (13 * 7 + this.X.GetHashCode()) * 7 + this.Y.GetHashCode();

      public override string ToString() => $"X = {this.X}, Y = {this.Y}";

      public int Length
      {
        get
        {
          return (int) Math.Sqrt((double) ((long) this.X * (long) this.X + (long) this.Y * (long) this.Y));
        }
      }

      public double Angle => Math.Atan2((double) this.Y, (double) this.X);

      public static Vector2i operator +(Vector2i a, Vector2i b) => new Vector2i(a.X + b.X, a.Y + b.Y);

      public static Vector2i operator -(Vector2i a, Vector2i b) => new Vector2i(a.X - b.X, a.Y - b.Y);

      public static Vector2i operator *(Vector2i a, Vector2i b) => new Vector2i(a.X * b.X, a.Y * b.Y);

      public static Vector2i operator /(Vector2i a, Vector2i b) => new Vector2i(a.X / b.X, a.Y / b.Y);

      public static Vector2i operator *(Vector2i v, int x) => new Vector2i(v.X * x, v.Y * x);

      public static Vector2i operator /(Vector2i v, int x) => new Vector2i(v.X / x, v.Y / x);

      public static bool operator ==(Vector2i a, Vector2i b) => a.Equals(b);

      public static bool operator !=(Vector2i a, Vector2i b) => !a.Equals(b);

      public static implicit operator Vector2(Vector2i v) => new Vector2((double) v.X, (double) v.Y);

      public static explicit operator Vector2i(Vector2 v) => new Vector2i((int) v.X, (int) v.Y);
    }
}
