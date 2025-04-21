// Decompiled with JetBrains decompiler
// Type: csvorbis.Lsp
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Runtime.InteropServices;

namespace csvorbis
{

    internal class Lsp
    {
      private static float M_PI = 3.14159274f;

      internal static void lsp_to_curve(
        float[] curve,
        int[] map,
        int n,
        int ln,
        float[] lsp,
        int m,
        float amp,
        float ampoffset)
      {
        float num1 = Lsp.M_PI / (float) ln;
        for (int index = 0; index < m; ++index)
          lsp[index] = Lookup.coslook(lsp[index]);
        int num2 = m / 2 * 2;
        int index1 = 0;
        while (index1 < n)
        {
          Lsp.FloatHack floatHack = new Lsp.FloatHack();
          int num3 = map[index1];
          float num4 = 0.707106769f;
          float num5 = 0.707106769f;
          float num6 = Lookup.coslook(num1 * (float) num3);
          for (int index2 = 0; index2 < num2; index2 += 2)
          {
            num5 *= lsp[index2] - num6;
            num4 *= lsp[index2 + 1] - num6;
          }
          float num7;
          float num8;
          if ((m & 1) != 0)
          {
            float num9 = num5 * (lsp[m - 1] - num6);
            num7 = num9 * num9;
            num8 = num4 * (num4 * (float) (1.0 - (double) num6 * (double) num6));
          }
          else
          {
            num7 = num5 * (num5 * (1f + num6));
            num8 = num4 * (num4 * (1f - num6));
          }
          float a = num8 + num7;
          floatHack.fh_float = a;
          int fhInt = floatHack.fh_int;
          int num10 = int.MaxValue & fhInt;
          int num11 = 0;
          if (num10 < 2139095040 /*0x7F800000*/ && num10 != 0)
          {
            if (num10 < 8388608 /*0x800000*/)
            {
              float num12 = a * 33554432f;
              floatHack.fh_float = num12;
              fhInt = floatHack.fh_int;
              num10 = int.MaxValue & fhInt;
              num11 = -25;
            }
            num11 += (num10 >>> 23) - 126;
            int num13 = (int) ((long) fhInt & 2155872255L | 1056964608L /*0x3F000000*/);
            floatHack.fh_int = num13;
            a = floatHack.fh_float;
          }
          float num14 = Lookup.fromdBlook(amp * Lookup.invsqlook(a) * Lookup.invsq2explook(num11 + m) - ampoffset);
    label_15:
          curve[index1] *= num14;
          ++index1;
          if (index1 < n && map[index1] == num3)
            goto label_15;
        }
      }

      [StructLayout(LayoutKind.Explicit, Size = 32 /*0x20*/)]
      private struct FloatHack
      {
        [FieldOffset(0)]
        public float fh_float;
        [FieldOffset(0)]
        public int fh_int;
      }
    }
}
