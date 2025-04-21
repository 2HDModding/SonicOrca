// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.Size
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Geometry
{

    public struct Size : IComparable<Size>, IEquatable<Size>
    {
      public double Width { get; set; }

      public double Height { get; set; }

      public double Area => this.Width * this.Height;

      public Size(double width, double height)
        : this()
      {
        this.Width = width;
        this.Height = height;
      }

      public override bool Equals(object obj) => this.Equals((Size) obj);

      public bool Equals(Size other) => this.Width == other.Width && this.Height == other.Height;

      public int CompareTo(Size other) => this.Area.CompareTo(other.Area);

      public override int GetHashCode()
      {
        return (13 * 7 + this.Width.GetHashCode()) * 7 + this.Height.GetHashCode();
      }

      public override string ToString() => $"Width = {this.Width} Height = {this.Height}";

      public static Size operator +(Size a, Size b) => new Size(a.Width + b.Width, a.Height + b.Height);

      public static Size operator -(Size a, Size b) => new Size(a.Width - b.Width, a.Height - b.Height);

      public static Size operator *(Size a, Size b) => new Size(a.Width * b.Width, a.Height * b.Height);

      public static Size operator /(Size a, Size b) => new Size(a.Width / b.Width, a.Height / b.Height);

      public static Size operator *(Size v, int x)
      {
        return new Size(v.Width * (double) x, v.Height * (double) x);
      }

      public static Size operator /(Size v, int x)
      {
        return new Size(v.Width / (double) x, v.Height / (double) x);
      }

      public static bool operator ==(Size a, Size b) => a.Equals(b);

      public static bool operator !=(Size a, Size b) => !a.Equals(b);
    }
}
