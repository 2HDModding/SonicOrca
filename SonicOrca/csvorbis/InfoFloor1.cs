// Decompiled with JetBrains decompiler
// Type: csvorbis.InfoFloor1
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace csvorbis
{

    internal class InfoFloor1
    {
      private const int VIF_POSIT = 63 /*0x3F*/;
      private const int VIF_CLASS = 16 /*0x10*/;
      private const int VIF_PARTS = 31 /*0x1F*/;
      internal int partitions;
      internal int[] partitionclass = new int[31 /*0x1F*/];
      internal int[] class_dim = new int[16 /*0x10*/];
      internal int[] class_subs = new int[16 /*0x10*/];
      internal int[] class_book = new int[16 /*0x10*/];
      internal int[][] class_subbook = new int[16 /*0x10*/][];
      internal int mult;
      internal int[] postlist = new int[65];
      internal float maxover;
      internal float maxunder;
      internal float maxerr;
      internal int twofitminsize;
      internal int twofitminused;
      internal int twofitweight;
      internal float twofitatten;
      internal int unusedminsize;
      internal int unusedmin_n;
      internal int n;

      internal InfoFloor1()
      {
        for (int index = 0; index < this.class_subbook.Length; ++index)
          this.class_subbook[index] = new int[8];
      }

      internal void free()
      {
        this.partitionclass = (int[]) null;
        this.class_dim = (int[]) null;
        this.class_subs = (int[]) null;
        this.class_book = (int[]) null;
        this.class_subbook = (int[][]) null;
        this.postlist = (int[]) null;
      }

      internal object copy_info()
      {
        InfoFloor1 infoFloor1_1 = this;
        InfoFloor1 infoFloor1_2 = new InfoFloor1();
        infoFloor1_2.partitions = infoFloor1_1.partitions;
        Array.Copy((Array) infoFloor1_1.partitionclass, 0, (Array) infoFloor1_2.partitionclass, 0, 31 /*0x1F*/);
        Array.Copy((Array) infoFloor1_1.class_dim, 0, (Array) infoFloor1_2.class_dim, 0, 16 /*0x10*/);
        Array.Copy((Array) infoFloor1_1.class_subs, 0, (Array) infoFloor1_2.class_subs, 0, 16 /*0x10*/);
        Array.Copy((Array) infoFloor1_1.class_book, 0, (Array) infoFloor1_2.class_book, 0, 16 /*0x10*/);
        for (int index = 0; index < 16 /*0x10*/; ++index)
          Array.Copy((Array) infoFloor1_1.class_subbook[index], 0, (Array) infoFloor1_2.class_subbook[index], 0, 8);
        infoFloor1_2.mult = infoFloor1_1.mult;
        Array.Copy((Array) infoFloor1_1.postlist, 0, (Array) infoFloor1_2.postlist, 0, 65);
        infoFloor1_2.maxover = infoFloor1_1.maxover;
        infoFloor1_2.maxunder = infoFloor1_1.maxunder;
        infoFloor1_2.maxerr = infoFloor1_1.maxerr;
        infoFloor1_2.twofitminsize = infoFloor1_1.twofitminsize;
        infoFloor1_2.twofitminused = infoFloor1_1.twofitminused;
        infoFloor1_2.twofitweight = infoFloor1_1.twofitweight;
        infoFloor1_2.twofitatten = infoFloor1_1.twofitatten;
        infoFloor1_2.unusedminsize = infoFloor1_1.unusedminsize;
        infoFloor1_2.unusedmin_n = infoFloor1_1.unusedmin_n;
        infoFloor1_2.n = infoFloor1_1.n;
        return (object) infoFloor1_2;
      }
    }
}
