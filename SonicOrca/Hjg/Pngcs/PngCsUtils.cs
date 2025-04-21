// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.PngCsUtils
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs
{

    internal class PngCsUtils
    {
      internal static bool arraysEqual4(byte[] ar1, byte[] ar2)
      {
        return (int) ar1[0] == (int) ar2[0] && (int) ar1[1] == (int) ar2[1] && (int) ar1[2] == (int) ar2[2] && (int) ar1[3] == (int) ar2[3];
      }

      internal static bool arraysEqual(byte[] a1, byte[] a2)
      {
        if (a1.Length != a2.Length)
          return false;
        for (int index = 0; index < a1.Length; ++index)
        {
          if ((int) a1[index] != (int) a2[index])
            return false;
        }
        return true;
      }
    }
}
