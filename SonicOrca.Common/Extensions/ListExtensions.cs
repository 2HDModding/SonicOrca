// Decompiled with JetBrains decompiler
// Type: SonicOrca.Extensions.ListExtensions
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System;
using System.Collections.Generic;

namespace SonicOrca.Extensions
{

    public static class ListExtensions
    {
      public static int IndexOf<T>(this IReadOnlyList<T> list, Predicate<T> match)
      {
        return list.IndexOf<T>(0, match);
      }

      public static int IndexOf<T>(this IReadOnlyList<T> list, int startIndex, Predicate<T> match)
      {
        for (int index = startIndex; index < ((IReadOnlyCollection<T>) list).Count; ++index)
        {
          if (match(list[index]))
            return index;
        }
        return -1;
      }
    }
}
