// Decompiled with JetBrains decompiler
// Type: csvorbis.InfoResidue0
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace csvorbis
{

    internal class InfoResidue0
    {
      internal int begin;
      internal int end;
      internal int grouping;
      internal int partitions;
      internal int groupbook;
      internal int[] secondstages = new int[64 /*0x40*/];
      internal int[] booklist = new int[256 /*0x0100*/];
      internal float[] entmax = new float[64 /*0x40*/];
      internal float[] ampmax = new float[64 /*0x40*/];
      internal int[] subgrp = new int[64 /*0x40*/];
      internal int[] blimit = new int[64 /*0x40*/];
    }
}
