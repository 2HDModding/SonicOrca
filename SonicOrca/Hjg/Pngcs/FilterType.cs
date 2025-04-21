// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.FilterType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs
{

    public enum FilterType
    {
      FILTER_UNKNOWN = -100, // 0xFFFFFF9C
      FILTER_CYCLIC = -50, // 0xFFFFFFCE
      FILTER_VERYAGGRESSIVE = -3, // 0xFFFFFFFD
      FILTER_AGGRESSIVE = -2, // 0xFFFFFFFE
      FILTER_DEFAULT = -1, // 0xFFFFFFFF
      FILTER_NONE = 0,
      FILTER_SUB = 1,
      FILTER_UP = 2,
      FILTER_AVERAGE = 3,
      FILTER_PAETH = 4,
    }
}
