// Decompiled with JetBrains decompiler
// Type: csvorbis.Time0
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;

namespace csvorbis
{

    internal class Time0 : FuncTime
    {
      public override void pack(object i, csBuffer opb)
      {
      }

      public override object unpack(Info vi, csBuffer opb) => (object) "";

      public override object look(DspState vd, InfoMode mi, object i) => (object) "";

      public override void free_info(object i)
      {
      }

      public override void free_look(object i)
      {
      }

      public override int forward(Block vb, object i) => 0;

      public override int inverse(Block vb, object i, float[] fin, float[] fout) => 0;
    }
}
