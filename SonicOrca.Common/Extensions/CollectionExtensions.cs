// Decompiled with JetBrains decompiler
// Type: SonicOrca.Extensions.CollectionExtensions
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Extensions
{

    public static class CollectionExtensions
    {
      public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
      {
        foreach (T obj in items)
          collection.Add(obj);
      }

      public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
      {
        foreach (T obj in items)
          collection.Remove(obj);
      }

      public static int RemoveAll<T>(this ICollection<T> collection, Predicate<T> match)
      {
        T[] array = collection.Where<T>((Func<T, bool>) (x => match(x))).ToArray<T>();
        foreach (T obj in array)
          collection.Remove(obj);
        return array.Length;
      }
    }
}
