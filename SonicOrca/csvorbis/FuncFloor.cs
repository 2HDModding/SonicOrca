// Decompiled with JetBrains decompiler
// Type: csvorbis.FuncFloor
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;

namespace csvorbis
{

    internal abstract class FuncFloor
    {
      public static FuncFloor[] floor_P = new FuncFloor[2]
      {
        (FuncFloor) new Floor0(),
        (FuncFloor) new Floor1()
      };

      public abstract void pack(object i, csBuffer opb);

      public abstract object unpack(Info vi, csBuffer opb);

      public abstract object look(DspState vd, InfoMode mi, object i);

      public abstract void free_info(object i);

      public abstract void free_look(object i);

      public abstract void free_state(object vs);

      public abstract int forward(Block vb, object i, float[] fin, float[] fout, object vs);

      public abstract object inverse1(Block vb, object i, object memo);

      public abstract int inverse2(Block vb, object i, object memo, float[] fout);
    }
}
