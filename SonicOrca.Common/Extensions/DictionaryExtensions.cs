// Decompiled with JetBrains decompiler
// Type: SonicOrca.Extensions.DictionaryExtensions
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System.Collections.Generic;

namespace SonicOrca.Extensions
{

    public static class DictionaryExtensions
    {
      public static Tvalue GetValueOrDefault<Tkey, Tvalue>(
        this IDictionary<Tkey, Tvalue> dictionary,
        Tkey key)
      {
        Tvalue obj;
        return !dictionary.TryGetValue(key, out obj) ? default (Tvalue) : obj;
      }

      public static Tvalue GetValueOrDefault<Tkey, Tvalue>(
        this IDictionary<Tkey, Tvalue> dictionary,
        Tkey key,
        Tvalue defaultValue)
      {
        Tvalue obj;
        return !dictionary.TryGetValue(key, out obj) ? defaultValue : obj;
      }
    }
}
