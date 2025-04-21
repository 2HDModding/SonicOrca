// Decompiled with JetBrains decompiler
// Type: csvorbis.InfoMapping0
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace csvorbis
{

    internal class InfoMapping0
    {
      internal int submaps;
      internal int[] chmuxlist = new int[256 /*0x0100*/];
      internal int[] timesubmap = new int[16 /*0x10*/];
      internal int[] floorsubmap = new int[16 /*0x10*/];
      internal int[] residuesubmap = new int[16 /*0x10*/];
      internal int[] psysubmap = new int[16 /*0x10*/];
      internal int coupling_steps;
      internal int[] coupling_mag = new int[256 /*0x0100*/];
      internal int[] coupling_ang = new int[256 /*0x0100*/];

      internal void free()
      {
        this.chmuxlist = (int[]) null;
        this.timesubmap = (int[]) null;
        this.floorsubmap = (int[]) null;
        this.residuesubmap = (int[]) null;
        this.psysubmap = (int[]) null;
        this.coupling_mag = (int[]) null;
        this.coupling_ang = (int[]) null;
      }
    }
}
