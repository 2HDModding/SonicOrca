// Decompiled with JetBrains decompiler
// Type: SonicOrca.Extensions.ArrayExtensions
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace SonicOrca.Extensions;

public static class ArrayExtensions
{
  public static IEnumerator<T> GetEnumeratorGeneric<T>(this T[] array)
  {
    return ((IEnumerable<T>) array).GetEnumerator();
  }

  public static T[] GetRange<T>(this T[] data, int index, int length)
  {
    T[] destinationArray = new T[length];
    Array.Copy((Array) data, index, (Array) destinationArray, 0, length);
    return destinationArray;
  }
}
