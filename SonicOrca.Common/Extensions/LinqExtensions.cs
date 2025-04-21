// Decompiled with JetBrains decompiler
// Type: SonicOrca.Extensions.LinqExtensions
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Extensions
{

    public static class LinqExtensions
    {
      public static T[] AsArray<T>(this IEnumerable<T> items)
      {
        if (!(items is T[] objArray))
          objArray = items.ToArray<T>();
        return objArray;
      }

      public static int GetCollectionHashCode<T>(this IEnumerable<T> e)
      {
        IEnumerator<T> enumerator = e.GetEnumerator();
        int collectionHashCode = 27;
        int num1;
        if (enumerator.MoveNext())
        {
          int num2 = collectionHashCode;
          int num3;
          if ((object) enumerator.Current != null)
          {
            num3 = enumerator.Current.GetHashCode();
          }
          else
          {
            num1 = 1;
            num3 = -num1.GetHashCode();
          }
          collectionHashCode = num2 * num3;
        }
        while (enumerator.MoveNext())
        {
          int num4 = 13 * collectionHashCode;
          int num5;
          if ((object) enumerator.Current != null)
          {
            num5 = enumerator.Current.GetHashCode();
          }
          else
          {
            num1 = 1;
            num5 = -num1.GetHashCode();
          }
          collectionHashCode = num4 + num5;
        }
        return collectionHashCode;
      }

      public static int GetCollectionHashCode(params object[] items)
      {
        return ((IEnumerable<object>) items).GetCollectionHashCode<object>();
      }

      public static bool HasSameElementsAs<T>(this IEnumerable<T> first, IEnumerable<T> second)
      {
        Dictionary<T, int> firstMap = first.GroupBy<T, T>((Func<T, T>) (x => x)).ToDictionary<IGrouping<T, T>, T, int>((Func<IGrouping<T, T>, T>) (x => x.Key), (Func<IGrouping<T, T>, int>) (x => x.Count<T>()));
        Dictionary<T, int> secondMap = second.GroupBy<T, T>((Func<T, T>) (x => x)).ToDictionary<IGrouping<T, T>, T, int>((Func<IGrouping<T, T>, T>) (x => x.Key), (Func<IGrouping<T, T>, int>) (x => x.Count<T>()));
        return firstMap.Keys.All<T>((Func<T, bool>) (x => secondMap.Keys.Contains<T>(x) && firstMap[x] == secondMap[x])) && secondMap.Keys.All<T>((Func<T, bool>) (x => firstMap.Keys.Contains<T>(x) && secondMap[x] == firstMap[x]));
      }

      public static string CommaAndConcatenation<T>(this IEnumerable<T> strings)
      {
        T[] array = strings.ToArray<T>();
        if (array.Length == 0)
          return string.Empty;
        if (array.Length == 1)
          return array[0].ToString();
        return array.Length == 2 ? $"{array[0]} and {array[1]}" : $"{string.Join<T>(", ", ((IEnumerable<T>) array).Take<T>(array.Length - 1))} and {((IEnumerable<T>) array).Last<T>()}";
      }

      public static IEnumerable<T> TakeAllButLast<T>(this IEnumerable<T> source)
      {
        IEnumerator<T> it = source.GetEnumerator();
        bool hasRemainingItems = false;
        bool flag = true;
        T obj = default (T);
        do
        {
          hasRemainingItems = it.MoveNext();
          if (hasRemainingItems)
          {
            if (!flag)
              yield return obj;
            obj = it.Current;
            flag = false;
          }
        }
        while (hasRemainingItems);
      }

      public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> source, T separator)
      {
        bool flag = true;
        foreach (T obj in source)
        {
          T value = obj;
          if (!flag)
            yield return separator;
          yield return value;
          flag = false;
          value = default (T);
        }
      }

      public static IEnumerable<IEnumerable<T>> SplitWhen<T>(
        this IEnumerable<T> source,
        Func<T, bool> shouldSplit)
      {
        List<T> currentGroup = new List<T>();
        foreach (T obj in source)
        {
          T item = obj;
          if (shouldSplit(item) && currentGroup.Count > 0)
          {
            yield return (IEnumerable<T>) currentGroup.ToArray();
            currentGroup.Clear();
          }
          currentGroup.Add(item);
          item = default (T);
        }
        if (currentGroup.Count > 0)
          yield return (IEnumerable<T>) currentGroup.ToArray();
      }

      public static bool Contains(this IEnumerable<string> items, string query, bool ignoreCase = false)
      {
        return items.Any<string>((Func<string, bool>) (x => string.Compare(x, query, ignoreCase) == 0));
      }

      public static string FirstOrDefault(
        this IEnumerable<string> items,
        string query,
        bool ignoreCase = false)
      {
        return items.FirstOrDefault<string>((Func<string, bool>) (x => string.Compare(x, query, ignoreCase) == 0));
      }

      public static IEnumerable<string> Surround(this IEnumerable<string> strings, string surrounder)
      {
        return strings.Select<string, string>((Func<string, string>) (x => surrounder + x + surrounder));
      }
    }
}
