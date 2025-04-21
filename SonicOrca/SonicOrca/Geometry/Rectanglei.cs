// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.Rectanglei
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Geometry
{

    public struct Rectanglei : IEquatable<Rectanglei>
    {
      public static Rectanglei Empty => new Rectanglei();

      public int X { get; set; }

      public int Y { get; set; }

      public int Width { get; set; }

      public int Height { get; set; }

      public long Area => (long) this.Width * (long) this.Height;

      public Vector2i TopLeft => new Vector2i(this.Left, this.Top);

      public Vector2i TopRight => new Vector2i(this.Right, this.Top);

      public Vector2i BottomLeft => new Vector2i(this.Left, this.Bottom);

      public Vector2i BottomRight => new Vector2i(this.Right, this.Bottom);

      public Vector2i Centre => new Vector2i(this.Left + this.Width / 2, this.Top + this.Height / 2);

      public Sizei Size => new Sizei(this.Width, this.Height);

      public static Rectangle FromLTRB(float left, float top, float right, float bottom)
      {
        return new Rectangle((double) left, (double) top, (double) right - (double) left, (double) bottom - (double) top);
      }

      public static Rectanglei FromLTRB(int left, int top, int right, int bottom)
      {
        return new Rectanglei(left, top, right - left, bottom - top);
      }

      public Rectanglei(int x, int y, int width, int height)
        : this()
      {
        this.X = x;
        this.Y = y;
        this.Width = width;
        this.Height = height;
      }

      public bool Contains(Vector2i p)
      {
        return p.X >= this.X && p.Y >= this.Y && p.X < this.Right && p.Y < this.Bottom;
      }

      public bool Contains(Rectanglei rect)
      {
        return rect.X >= this.X && rect.Y >= this.Y && rect.Right <= this.Right && rect.Bottom <= this.Bottom;
      }

      public bool IntersectsWith(Rectanglei rect)
      {
        return rect.X < this.Right && rect.Right > this.X && rect.Y < this.Bottom && rect.Bottom > this.Y;
      }

      public Rectanglei Inflate(Vector2i amount)
      {
        return new Rectanglei(this.X - amount.X, this.Y - amount.Y, this.Width + amount.X * 2, this.Height + amount.Y * 2);
      }

      public Rectanglei OffsetBy(Vector2i amount)
      {
        return new Rectanglei(this.X + amount.X, this.Y + amount.Y, this.Width, this.Height);
      }

      public Rectanglei Union(Rectanglei rect)
      {
        return new Rectanglei()
        {
          Left = Math.Min(this.Left, rect.Left),
          Top = Math.Min(this.Top, rect.Top),
          Right = Math.Max(this.Right, rect.Right),
          Bottom = Math.Max(this.Bottom, rect.Bottom)
        };
      }

      public override bool Equals(object obj)
      {
        Rectanglei rectanglei = (Rectanglei) obj;
        return rectanglei.X == this.X && rectanglei.Y == this.Y && rectanglei.Width == this.Width && rectanglei.Height == this.Height;
      }

      public bool Equals(Rectanglei other)
      {
        return other.X == this.X && other.Y == this.Y && other.Width == this.Width && other.Height == this.Height;
      }

      public override int GetHashCode()
      {
        return (((13 * 7 + this.X.GetHashCode()) * 7 + this.Y.GetHashCode()) * 7 + this.Width.GetHashCode()) * 7 + this.Height.GetHashCode();
      }

      public override string ToString()
      {
        return $"X = {this.X} Y = {this.Y} Width = {this.Width} Height = {this.Height}";
      }

      public Vector2i Location
      {
        get => new Vector2i(this.X, this.Y);
        set
        {
          this.X = value.X;
          this.Y = value.Y;
        }
      }

      public int Left
      {
        get => this.X;
        set
        {
          int right = this.Right;
          this.X = value;
          this.Right = right;
        }
      }

      public int Top
      {
        get => this.Y;
        set
        {
          int bottom = this.Bottom;
          this.Y = value;
          this.Bottom = bottom;
        }
      }

      public int Right
      {
        get => this.X + this.Width;
        set => this.Width = value - this.X;
      }

      public int Bottom
      {
        get => this.Y + this.Height;
        set => this.Height = value - this.Y;
      }

      public static Rectanglei Union(Rectanglei a, Rectanglei b)
      {
        return Rectanglei.FromLTRB(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));
      }

      public static Rectanglei Intersect(Rectanglei a, Rectanglei b)
      {
        int x = Math.Max(a.X, b.X);
        int num1 = Math.Min(a.Right, b.Right);
        int y = Math.Max(a.Y, b.Y);
        int num2 = Math.Min(a.Bottom, b.Bottom);
        return num1 >= x && num2 >= y ? new Rectanglei(x, y, num1 - x, num2 - y) : Rectanglei.Empty;
      }

      public static implicit operator Rectangle(Rectanglei r)
      {
        return new Rectangle((double) r.X, (double) r.Y, (double) r.Width, (double) r.Height);
      }

      public static implicit operator Rectanglei(Rectangle r)
      {
        return new Rectanglei((int) r.X, (int) r.Y, (int) r.Width, (int) r.Height);
      }

      public static bool operator ==(Rectanglei a, Rectanglei b) => a.Equals(b);

      public static bool operator !=(Rectanglei a, Rectanglei b) => !a.Equals(b);
    }
}
