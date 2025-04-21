// Decompiled with JetBrains decompiler
// Type: SonicOrca.EnumHelpers
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System;

namespace SonicOrca
{

    public static class EnumHelpers
    {
      public static int GetEnumCount(Type enumeration) => Enum.GetNames(enumeration).Length;
    }
}
