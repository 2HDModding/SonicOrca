// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.Rectangle
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Geometry
{

    public struct Rectangle : IEquatable<Rectangle>
    {
      public static Rectangle Empty => new Rectangle();

      public double X { get; set; }

      public double Y { get; set; }

      public double Width { get; set; }

      public double Height { get; set; }

      public double Area => this.Width * this.Height;

      public Vector2 TopLeft => new Vector2(this.Left, this.Top);

      public Vector2 TopRight => new Vector2(this.Right, this.Top);

      public Vector2 BottomLeft => new Vector2(this.Left, this.Bottom);

      public Vector2 BottomRight => new Vector2(this.Right, this.Bottom);

      public Vector2 Centre => new Vector2(this.Left + this.Width / 2.0, this.Top + this.Height / 2.0);

      public Size Size => new Size(this.Width, this.Height);

      public static Rectangle FromLTRB(double left, double top, double right, double bottom)
      {
        return new Rectangle(left, top, right - left, bottom - top);
      }

      public Rectangle(double x, double y, double width, double height)
        : this()
      {
        this.X = x;
        this.Y = y;
        this.Width = width;
        this.Height = height;
      }

      public bool Contains(Vector2 p)
      {
        return p.X > this.X && p.Y > this.Y && p.X < this.Right && p.Y < this.Bottom;
      }

      public bool ContainsOrOverlaps(Vector2 p)
      {
        return p.X >= this.Top && p.Y >= this.Left && p.X <= this.Right && p.Y <= this.Bottom;
      }

      public bool Contains(Rectangle rect)
      {
        return rect.X >= this.X && rect.Y >= this.Y && rect.Right <= this.Right && rect.Bottom <= this.Bottom;
      }

      public bool IntersectsWith(Rectangle rect)
      {
        return rect.X <= this.Right && rect.Right >= this.X && rect.Y <= this.Bottom && rect.Bottom >= this.Y;
      }

      public Rectangle Inflate(Vector2 amount)
      {
        return new Rectangle(this.X - amount.X, this.Y - amount.Y, this.Width + amount.X * 2.0, this.Height + amount.Y * 2.0);
      }

      public Rectangle OffsetBy(Vector2 amount)
      {
        return new Rectangle(this.X + amount.X, this.Y + amount.Y, this.Width, this.Height);
      }

      public override bool Equals(object obj) => this.Equals((Rectangle) obj);

      public bool Equals(Rectangle other)
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

      public static Rectangle operator +(Rectangle rectangle, Vector2 offset)
      {
        rectangle.X += offset.X;
        rectangle.Y += offset.Y;
        return rectangle;
      }

      public static Rectangle operator -(Rectangle rectangle, Vector2 offset)
      {
        rectangle.X -= offset.X;
        rectangle.Y -= offset.Y;
        return rectangle;
      }

      public static bool operator ==(Rectangle a, Rectangle b) => a.Equals(b);

      public static bool operator !=(Rectangle a, Rectangle b) => !a.Equals(b);

      public Vector2 Location
      {
        get => new Vector2(this.X, this.Y);
        set
        {
          this.X = value.X;
          this.Y = value.Y;
        }
      }

      public double Left
      {
        get => this.X;
        set
        {
          double right = this.Right;
          this.X = value;
          this.Right = right;
        }
      }

      public double Top
      {
        get => this.Y;
        set
        {
          double bottom = this.Bottom;
          this.Y = value;
          this.Bottom = bottom;
        }
      }

      public double Right
      {
        get => this.X + this.Width;
        set => this.Width = value - this.X;
      }

      public double Bottom
      {
        get => this.Y + this.Height;
        set => this.Height = value - this.Y;
      }

      public double CentreX => this.X + this.Width / 2.0;

      public double CentreY => this.Y + this.Height / 2.0;

      public static Rectangle Intersect(Rectangle a, Rectangle b)
      {
        double x = Math.Max(a.X, b.X);
        double num1 = Math.Min(a.Right, b.Right);
        double y = Math.Max(a.Y, b.Y);
        double num2 = Math.Min(a.Bottom, b.Bottom);
        return num1 >= x && num2 >= y ? new Rectangle(x, y, num1 - x, num2 - y) : Rectangle.Empty;
      }
    }
}
