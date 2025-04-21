// Decompiled with JetBrains decompiler
// Type: csvorbis.FuncTime
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;

namespace csvorbis
{

    internal abstract class FuncTime
    {
      public static FuncTime[] time_P = new FuncTime[1]
      {
        (FuncTime) new Time0()
      };

      public abstract void pack(object i, csBuffer opb);

      public abstract object unpack(Info vi, csBuffer opb);

      public abstract object look(DspState vd, InfoMode vm, object i);

      public abstract void free_info(object i);

      public abstract void free_look(object i);

      public abstract int forward(Block vb, object i);

      public abstract int inverse(Block vb, object i, float[] fin, float[] fout);
    }
}
