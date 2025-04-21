// Decompiled with JetBrains decompiler
// Type: SonicOrca.MathX
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca
{

    public static class MathX
    {
      public const double TWOPI = 6.2831853071795862;
      public const double PI_2 = 1.5707963267948966;
      public const double PI_4 = 0.78539816339744828;

      public static double Lerp(double a, double b, double t) => (1.0 - t) * a + t * b;

      public static double Lerp(double from, double to, double amount, double minChange)
      {
        double num = (to - from) * amount;
        if (to > from)
        {
          from += num < minChange ? minChange : num;
          return from <= to ? from : to;
        }
        from += num > -minChange ? -minChange : num;
        return from >= to ? from : to;
      }

      public static double LerpWrap(
        double from,
        double to,
        double amount,
        double minValue,
        double maxValue,
        double minChange = 0.0)
      {
        double num1 = maxValue - minValue;
        double num2 = num1 / 2.0;
        from %= num1;
        to %= num1;
        while (to < from - num2)
          to += num1;
        while (to > from + num2)
          to -= num1;
        from = MathX.Lerp(from, to, amount, minChange);
        if (from < minValue)
          return from + num1;
        return from > maxValue ? from - num1 : from;
      }

      public static double Snap(double value, double precision)
      {
        return Math.Round(value / precision) * precision;
      }

      public static bool IsBetween(double a, double b, double c) => a <= b && b <= c;

      public static int Clamp(int low, int value, int high)
      {
        if (value < low)
          return low;
        return value > high ? high : value;
      }

      public static int Clamp(int value, int high) => MathX.Clamp(-high, value, high);

      public static double Clamp(double low, double value, double high)
      {
        if (value < low)
          return low;
        return value > high ? high : value;
      }

      public static double Clamp(double value, double high) => MathX.Clamp(-high, value, high);

      public static double ChangeSpeed(double x, double amount)
      {
        if (x < 0.0)
        {
          x -= amount;
          if (x > 0.0)
            x = 0.0;
        }
        else
        {
          x += amount;
          if (x < 0.0)
            x = 0.0;
        }
        return x;
      }

      public static double ChangeSpeedNonZero(double x, double amount)
      {
        if (x < 0.0)
          x -= amount;
        else
          x += amount;
        return x;
      }

      public static int Wrap(int x, int max, int min = 0)
      {
        int num = max - min;
        while (x < min)
          x += num;
        while (x > max)
          x -= num;
        return x;
      }

      public static double Wrap(double x, double max, double min = 0.0)
      {
        double num = max - min;
        while (x < min)
          x += num;
        while (x > max)
          x -= num;
        return x;
      }

      public static double WrapRadians(double radians)
      {
        if (radians < Math.PI)
          radians += 2.0 * Math.PI;
        if (radians > Math.PI)
          radians -= 2.0 * Math.PI;
        return radians;
      }

      public static int GoTowards(int x, int dest, int maxChange)
      {
        if (x < dest)
          return Math.Min(x + maxChange, dest);
        return x > dest ? Math.Max(x - maxChange, dest) : x;
      }

      public static double GoTowards(double x, double dest, double maxChange)
      {
        if (x < dest)
          return Math.Min(x + maxChange, dest);
        return x > dest ? Math.Max(x - maxChange, dest) : x;
      }

      public static double GoTowardsWrap(
        double x,
        double dest,
        double maxChange,
        double minValue,
        double maxValue)
      {
        x = MathX.Wrap(x, maxValue, minValue);
        dest = MathX.Wrap(dest, maxValue, minValue);
        if (x == dest)
          return x;
        if (x < dest)
        {
          double val1 = x - minValue + (maxValue - dest);
          double num = dest - x;
          return val1 < num ? MathX.Wrap(x - Math.Min(val1, maxChange), maxValue, minValue) : Math.Min(x + maxChange, dest);
        }
        double num1 = x - dest;
        double val1_1 = maxValue - x + (dest - minValue);
        double num2 = val1_1;
        return num1 < num2 ? Math.Max(x - maxChange, dest) : MathX.Wrap(x + Math.Min(val1_1, maxChange), maxValue, minValue);
      }

      public static double WrapDifference(double a, double b, double minValue, double maxValue)
      {
        if (a == b)
          return 0.0;
        if (a < b)
        {
          double num1 = a - minValue + (maxValue - b);
          double num2 = b - a;
          return num1 < num2 ? -num1 : num2;
        }
        double num3 = a - b;
        double num4 = maxValue - a + (b - minValue);
        return num3 < num4 ? -num3 : num4;
      }

      public static double ToRadians(double degrees) => degrees * (Math.PI / 180.0);

      public static double ToDegrees(double radians) => radians * (180.0 / Math.PI);

      public static double DifferenceRadians(double a, double b)
      {
        return MathX.WrapDifference(a, b, -1.0 * Math.PI, Math.PI);
      }
    }
}
