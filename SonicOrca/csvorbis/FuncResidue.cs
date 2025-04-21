// Decompiled with JetBrains decompiler
// Type: csvorbis.FuncResidue
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;

namespace csvorbis
{

    internal abstract class FuncResidue
    {
      public static FuncResidue[] residue_P = new FuncResidue[3]
      {
        (FuncResidue) new Residue0(),
        (FuncResidue) new Residue1(),
        (FuncResidue) new Residue2()
      };

      public abstract void pack(object vr, csBuffer opb);

      public abstract object unpack(Info vi, csBuffer opb);

      public abstract object look(DspState vd, InfoMode vm, object vr);

      public abstract void free_info(object i);

      public abstract void free_look(object i);

      public abstract int forward(Block vb, object vl, float[][] fin, int ch);

      public abstract int inverse(Block vb, object vl, float[][] fin, int[] nonzero, int ch);
    }
}
