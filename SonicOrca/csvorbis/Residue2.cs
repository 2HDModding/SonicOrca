// Decompiled with JetBrains decompiler
// Type: csvorbis.Residue2
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace csvorbis
{

    internal class Residue2 : Residue0
    {
      public override int forward(Block vb, object vl, float[][] fin, int ch) => 0;

      public override int inverse(Block vb, object vl, float[][] fin, int[] nonzero, int ch)
      {
        int index = 0;
        while (index < ch && nonzero[index] == 0)
          ++index;
        return index == ch ? 0 : Residue0._2inverse(vb, vl, fin, ch);
      }
    }
}
