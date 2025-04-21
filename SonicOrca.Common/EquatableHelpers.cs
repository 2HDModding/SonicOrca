// Decompiled with JetBrains decompiler
// Type: SonicOrca.EquatableHelpers
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System;

#nullable disable
namespace SonicOrca;

public static class EquatableHelpers
{
  public static bool ReferenceThenValueEquals<T>(T a, T b) where T : IEquatable<T>
  {
    if ((object) a == (object) b)
      return true;
    return (object) a != null && (object) b != null && a.Equals(b);
  }
}
