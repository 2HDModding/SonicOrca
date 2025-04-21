// Decompiled with JetBrains decompiler
// Type: csvorbis.Residue1
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace csvorbis
{

    internal class Residue1 : Residue0
    {
      private new int forward(Block vb, object vl, float[][] fin, int ch) => 0;

      public override int inverse(Block vb, object vl, float[][] fin, int[] nonzero, int ch)
      {
        int ch1 = 0;
        for (int index = 0; index < ch; ++index)
        {
          if (nonzero[index] != 0)
            fin[ch1++] = fin[index];
        }
        return ch1 != 0 ? Residue0._01inverse(vb, vl, fin, ch1, 1) : 0;
      }
    }
}
