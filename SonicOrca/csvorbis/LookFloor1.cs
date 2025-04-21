// Decompiled with JetBrains decompiler
// Type: csvorbis.LookFloor1
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace csvorbis
{

    internal class LookFloor1
    {
      private static int VIF_POSIT = 63 /*0x3F*/;
      internal int[] sorted_index = new int[LookFloor1.VIF_POSIT + 2];
      internal int[] forward_index = new int[LookFloor1.VIF_POSIT + 2];
      internal int[] reverse_index = new int[LookFloor1.VIF_POSIT + 2];
      internal int[] hineighbor = new int[LookFloor1.VIF_POSIT];
      internal int[] loneighbor = new int[LookFloor1.VIF_POSIT];
      internal int posts;
      internal int n;
      internal int quant_q;
      internal InfoFloor1 vi;

      private void free()
      {
        this.sorted_index = (int[]) null;
        this.forward_index = (int[]) null;
        this.reverse_index = (int[]) null;
        this.hineighbor = (int[]) null;
        this.loneighbor = (int[]) null;
      }
    }
}
