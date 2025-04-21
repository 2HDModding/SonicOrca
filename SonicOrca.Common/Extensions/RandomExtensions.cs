// Decompiled with JetBrains decompiler
// Type: SonicOrca.Extensions.RandomExtensions
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System;

#nullable disable
namespace SonicOrca.Extensions;

public static class RandomExtensions
{
  public static double NextSignedDouble(this Random random) => random.NextDouble() * 2.0 - 1.0;

  public static double NextRadians(this Random random) => random.NextSignedDouble() * Math.PI;

  public static double NextDouble(this Random random, double maxValue)
  {
    return random.NextDouble() * maxValue;
  }

  public static double NextDouble(this Random random, double minValue, double maxValue)
  {
    return minValue + random.NextDouble() * (maxValue - minValue);
  }

  public static bool NextBoolean(this Random random) => (random.Next() & 1) == 0;

  public static int NextSign(this Random random) => !random.NextBoolean() ? 1 : -1;

  public static int NextSign(this Random random, int value)
  {
    return !random.NextBoolean() ? value : -value;
  }
}
