// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.Colour
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Text.RegularExpressions;

namespace SonicOrca.Graphics
{

    public struct Colour : IEquatable<Colour>
    {
      public uint Argb { get; set; }

      public Colour(double rgb)
        : this(rgb, rgb, rgb)
      {
      }

      public Colour(double r, double g, double b)
        : this(1.0, r, g, b)
      {
      }

      public Colour(double a, double r, double g, double b)
        : this((byte) (a * (double) byte.MaxValue), (byte) (r * (double) byte.MaxValue), (byte) (g * (double) byte.MaxValue), (byte) (b * (double) byte.MaxValue))
      {
      }

      public Colour(byte r, byte g, byte b)
        : this(byte.MaxValue, r, g, b)
      {
      }

      public Colour(byte a, byte r, byte g, byte b)
        : this()
      {
        this.Alpha = a;
        this.Red = r;
        this.Green = g;
        this.Blue = b;
      }

      public Colour(uint argb)
        : this()
      {
        this.Argb = argb;
      }

      public byte Alpha
      {
        get => (byte) (this.Argb >> 24);
        set => this.Argb = (uint) ((int) this.Argb & 16777215 /*0xFFFFFF*/ | (int) value << 24);
      }

      public byte Red
      {
        get => (byte) (this.Argb >> 16 /*0x10*/ & (uint) byte.MaxValue);
        set => this.Argb = (uint) ((int) this.Argb & -16711681 | (int) value << 16 /*0x10*/);
      }

      public byte Green
      {
        get => (byte) (this.Argb >> 8 & (uint) byte.MaxValue);
        set => this.Argb = (uint) ((int) this.Argb & -65281 | (int) value << 8);
      }

      public byte Blue
      {
        get => (byte) (this.Argb & (uint) byte.MaxValue);
        set => this.Argb = this.Argb & 4294967040U | (uint) value;
      }

      public double Hue
      {
        get
        {
          double val1 = (double) this.Red / (double) byte.MaxValue;
          double val2_1 = (double) this.Green / (double) byte.MaxValue;
          double val2_2 = (double) this.Blue / (double) byte.MaxValue;
          double num1 = Math.Max(Math.Max(val1, val2_1), val2_2);
          double num2 = Math.Min(Math.Min(val1, val2_1), val2_2);
          if ((num2 + num1) / 2.0 <= 0.0)
            return 0.0;
          double num3 = num1 - num2;
          if (num3 <= 0.0)
            return 0.0;
          double num4 = (num1 - val1) / num3;
          double num5 = (num1 - val2_1) / num3;
          double num6 = (num1 - val2_2) / num3;
          return (val1 != num1 ? (val2_1 != num1 ? (val1 == num2 ? 3.0 + num5 : 5.0 - num4) : (val2_2 == num2 ? 1.0 + num4 : 3.0 - num6)) : (val2_1 == num2 ? 5.0 + num6 : 1.0 - num5)) / 6.0;
        }
        set => this.SetFromHSL(value, this.Saturation, this.Luminosity);
      }

      public double Saturation
      {
        get
        {
          double val1 = (double) this.Red / (double) byte.MaxValue;
          double val2_1 = (double) this.Green / (double) byte.MaxValue;
          double val2_2 = (double) this.Blue / (double) byte.MaxValue;
          double num1 = Math.Max(Math.Max(val1, val2_1), val2_2);
          double num2 = Math.Min(Math.Min(val1, val2_1), val2_2);
          double num3 = (num2 + num1) / 2.0;
          if (num3 <= 0.0)
            return 0.0;
          double num4 = num1 - num2;
          return num4 <= 0.0 ? num4 : num4 / (num3 <= 0.5 ? num1 + num2 : 2.0 - num1 - num2);
        }
        set => this.SetFromHSL(this.Hue, value, this.Luminosity);
      }

      public double Luminosity
      {
        get
        {
          double val1 = (double) this.Red / (double) byte.MaxValue;
          double val2_1 = (double) this.Green / (double) byte.MaxValue;
          double val2_2 = (double) this.Blue / (double) byte.MaxValue;
          double num = Math.Max(Math.Max(val1, val2_1), val2_2);
          return (Math.Min(Math.Min(val1, val2_1), val2_2) + num) / 2.0;
        }
        set => this.SetFromHSL(this.Hue, this.Saturation, value);
      }

      public override int GetHashCode() => this.Argb.GetHashCode();

      public override bool Equals(object obj) => this.Equals((Colour) obj);

      public bool Equals(Colour other) => (int) this.Argb == (int) other.Argb;

      public override string ToString() => $"({this.Red}, {this.Green}, {this.Blue}, {this.Alpha})";

      public static bool operator ==(Colour lhs, Colour rhs) => lhs.Equals(rhs);

      public static bool operator !=(Colour lhs, Colour rhs) => !lhs.Equals(rhs);

      public Colour GetDarker(double amount)
      {
        Colour darker = this;
        darker.Luminosity -= amount;
        return darker;
      }

      public Colour GetLighter(double amount)
      {
        Colour lighter = this;
        lighter.Luminosity += amount;
        return lighter;
      }

      public static Colour FromHSL(double hue, double sat, double lum)
      {
        Colour colour = new Colour();
        colour.SetFromHSL(hue, sat, lum);
        return colour;
      }

      private void SetFromHSL(double hue, double sat, double lum)
      {
        hue = MathX.Clamp(0.0, hue, 1.0);
        sat = MathX.Clamp(0.0, sat, 1.0);
        lum = MathX.Clamp(0.0, lum, 1.0);
        double num1 = lum;
        double num2 = lum;
        double num3 = lum;
        double num4 = lum <= 0.5 ? lum * (1.0 + sat) : lum + sat - lum * sat;
        if (num4 > 0.0)
        {
          double num5 = lum + lum - num4;
          double num6 = (num4 - num5) / num4;
          hue *= 6.0;
          int num7 = (int) hue;
          double num8 = hue - (double) num7;
          double num9 = num4 * num6 * num8;
          double num10 = num5 + num9;
          double num11 = num4 - num9;
          switch (num7)
          {
            case 0:
              num1 = num4;
              num2 = num10;
              num3 = num5;
              break;
            case 1:
              num1 = num11;
              num2 = num4;
              num3 = num5;
              break;
            case 2:
              num1 = num5;
              num2 = num4;
              num3 = num10;
              break;
            case 3:
              num1 = num5;
              num2 = num11;
              num3 = num4;
              break;
            case 4:
              num1 = num10;
              num2 = num5;
              num3 = num4;
              break;
            case 5:
              num1 = num4;
              num2 = num5;
              num3 = num11;
              break;
          }
        }
        this.Red = (byte) MathX.Clamp(0.0, num1 * (double) byte.MaxValue, (double) byte.MaxValue);
        this.Green = (byte) MathX.Clamp(0.0, num2 * (double) byte.MaxValue, (double) byte.MaxValue);
        this.Blue = (byte) MathX.Clamp(0.0, num3 * (double) byte.MaxValue, (double) byte.MaxValue);
      }

      public string ToHexString()
      {
        if (Colour.IsFourBitColour(this.Alpha) && Colour.IsFourBitColour(this.Red) && Colour.IsFourBitColour(this.Green) && Colour.IsFourBitColour(this.Blue))
        {
          if (this.Alpha == byte.MaxValue)
            return $"#{(int) this.Red & 15:X}{(int) this.Green & 15:X}{(int) this.Blue & 15:X}";
          return $"#{(int) this.Alpha & 15:X}{(int) this.Red & 15:X}{(int) this.Green & 15:X}{(int) this.Blue & 15:X}";
        }
        if (this.Alpha == byte.MaxValue)
          return $"#{this.Red:X2}{this.Green:X2}{this.Blue:X2}";
        return $"#{this.Alpha:X2}{this.Red:X2}{this.Green:X2}{this.Blue:X2}";
      }

      public static Colour ParseHex(string s)
      {
        Colour result;
        if (!Colour.TryParseHex(s, out result))
          throw new FormatException("Invalid colour hex string.");
        return result;
      }

      public static bool TryParseHex(string s, out Colour result)
      {
        s = s.Trim();
        if (s.Length == 0)
        {
          result = Colours.Black;
          return false;
        }
        if (s[0] == '#')
          s = s.Substring(1);
        s = s.ToUpper();
        if (!Regex.IsMatch(s, "^([0-9A-F]{3}|[0-9A-F]{4}|[0-9A-F]{6}|[0-9A-F]{8})$"))
        {
          result = Colours.Black;
          return false;
        }
        switch (s.Length)
        {
          case 3:
            result = new Colour(Colour.Convert4to8BitColour(Convert.ToByte(s.Substring(0, 1), 16 /*0x10*/)), Colour.Convert4to8BitColour(Convert.ToByte(s.Substring(1, 1), 16 /*0x10*/)), Colour.Convert4to8BitColour(Convert.ToByte(s.Substring(2, 1), 16 /*0x10*/)));
            return true;
          case 4:
            result = new Colour(Colour.Convert4to8BitColour(Convert.ToByte(s.Substring(0, 1), 16 /*0x10*/)), Colour.Convert4to8BitColour(Convert.ToByte(s.Substring(1, 1), 16 /*0x10*/)), Colour.Convert4to8BitColour(Convert.ToByte(s.Substring(2, 1), 16 /*0x10*/)), Colour.Convert4to8BitColour(Convert.ToByte(s.Substring(3, 1), 16 /*0x10*/)));
            return true;
          case 6:
            result = new Colour(Convert.ToByte(s.Substring(0, 2), 16 /*0x10*/), Convert.ToByte(s.Substring(2, 2), 16 /*0x10*/), Convert.ToByte(s.Substring(4, 2), 16 /*0x10*/));
            return true;
          case 8:
            result = new Colour(Convert.ToByte(s.Substring(0, 2), 16 /*0x10*/), Convert.ToByte(s.Substring(2, 2), 16 /*0x10*/), Convert.ToByte(s.Substring(4, 2), 16 /*0x10*/), Convert.ToByte(s.Substring(6, 2), 16 /*0x10*/));
            return true;
          default:
            result = Colours.Black;
            return false;
        }
      }

      private static byte Convert4to8BitColour(byte colour)
      {
        return (byte) ((uint) colour << 4 | (uint) colour);
      }

      private static bool IsFourBitColour(byte colour) => (int) colour >> 4 == ((int) colour & 15);

      public static Colour FromOpacity(double opacity) => new Colour(opacity, 1.0, 1.0, 1.0);
    }
}
