// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.Sizei
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Geometry
{

    public struct Sizei : IComparable<Sizei>, IEquatable<Sizei>
    {
      public int Width { get; set; }

      public int Height { get; set; }

      public long Area => (long) this.Width * (long) this.Height;

      public Sizei(int width, int height)
        : this()
      {
        this.Width = width;
        this.Height = height;
      }

      public override bool Equals(object obj) => this.Equals((Sizei) obj);

      public bool Equals(Sizei other) => this.Width == other.Width && this.Height == other.Height;

      public int CompareTo(Sizei other) => this.Area.CompareTo(other.Area);

      public override int GetHashCode()
      {
        return (13 * 7 + this.Width.GetHashCode()) * 7 + this.Height.GetHashCode();
      }

      public override string ToString() => $"Width = {this.Width} Height = {this.Height}";

      public static Sizei operator +(Sizei a, Sizei b)
      {
        return new Sizei(a.Width + b.Width, a.Height + b.Height);
      }

      public static Sizei operator -(Sizei a, Sizei b)
      {
        return new Sizei(a.Width - b.Width, a.Height - b.Height);
      }

      public static Sizei operator *(Sizei a, Sizei b)
      {
        return new Sizei(a.Width * b.Width, a.Height * b.Height);
      }

      public static Sizei operator /(Sizei a, Sizei b)
      {
        return new Sizei(a.Width / b.Width, a.Height / b.Height);
      }

      public static Sizei operator *(Sizei v, int x) => new Sizei(v.Width * x, v.Height * x);

      public static Sizei operator /(Sizei v, int x) => new Sizei(v.Width / x, v.Height / x);

      public static bool operator ==(Sizei a, Sizei b) => a.Equals(b);

      public static bool operator !=(Sizei a, Sizei b) => !a.Equals(b);

      public static implicit operator Size(Sizei size)
      {
        return new Size((double) size.Width, (double) size.Height);
      }

      public static explicit operator Sizei(Size size)
      {
        return new Sizei((int) size.Width, (int) size.Height);
      }
    }
}
