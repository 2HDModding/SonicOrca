// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Animation.SemiNumericComparer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;

namespace SonicOrca.Graphics.V2.Animation
{

    public class SemiNumericComparer : IComparer<string>
    {
      public int Compare(string s1, string s2)
      {
        if (SemiNumericComparer.IsNumeric((object) s1) && SemiNumericComparer.IsNumeric((object) s2))
        {
          if (Convert.ToInt32(s1) > Convert.ToInt32(s2))
            return 1;
          if (Convert.ToInt32(s1) < Convert.ToInt32(s2))
            return -1;
          if (Convert.ToInt32(s1) == Convert.ToInt32(s2))
            return 0;
        }
        if (SemiNumericComparer.IsNumeric((object) s1) && !SemiNumericComparer.IsNumeric((object) s2))
          return -1;
        return !SemiNumericComparer.IsNumeric((object) s1) && SemiNumericComparer.IsNumeric((object) s2) ? 1 : string.Compare(s1, s2, true);
      }

      public static bool IsNumeric(object value)
      {
        try
        {
          Convert.ToInt32(value.ToString());
          return true;
        }
        catch (FormatException ex)
        {
          return false;
        }
      }
    }
}
