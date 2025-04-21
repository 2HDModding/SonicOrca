// Decompiled with JetBrains decompiler
// Type: csvorbis.Drft
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace csvorbis
{

    internal class Drft
    {
      private int n;
      private float[] trigcache;
      private int[] splitcache;
      private static int[] ntryh = new int[4]{ 4, 2, 3, 5 };
      private static float tpi = 6.28318548f;
      private static float hsqt2 = 0.707106769f;
      private static float taui = 0.8660254f;
      private static float taur = -0.5f;
      private static float sqrt2 = 1.41421354f;

      internal void backward(float[] data)
      {
        if (this.n == 1)
          return;
        Drft.drftb1(this.n, data, this.trigcache, this.trigcache, this.n, this.splitcache);
      }

      internal void init(int n)
      {
        this.n = n;
        this.trigcache = new float[3 * n];
        this.splitcache = new int[32 /*0x20*/];
        Drft.fdrffti(n, this.trigcache, this.splitcache);
      }

      internal void clear()
      {
        if (this.trigcache != null)
          this.trigcache = (float[]) null;
        if (this.splitcache == null)
          return;
        this.splitcache = (int[]) null;
      }

      private static void drfti1(int n, float[] wa, int index, int[] ifac)
      {
        int num1 = 0;
        int index1 = -1;
        int num2 = n;
        int num3 = 0;
    label_1:
        ++index1;
        if (index1 < 4)
          num1 = Drft.ntryh[index1];
        else
          num1 += 2;
        do
        {
          int num4 = num2 / num1;
          if (num2 - num1 * num4 == 0)
          {
            ++num3;
            ifac[num3 + 1] = num1;
            num2 = num4;
            if (num1 == 2 && num3 != 1)
            {
              for (int index2 = 1; index2 < num3; ++index2)
              {
                int index3 = num3 - index2 + 1;
                ifac[index3 + 1] = ifac[index3];
              }
              ifac[2] = 2;
            }
          }
          else
            goto label_1;
        }
        while (num2 != 1);
        ifac[0] = n;
        ifac[1] = num3;
        float num5 = Drft.tpi / (float) n;
        int num6 = 0;
        int num7 = num3 - 1;
        int num8 = 1;
        if (num7 == 0)
          return;
        for (int index4 = 0; index4 < num7; ++index4)
        {
          int num9 = ifac[index4 + 2];
          int num10 = 0;
          int num11 = num8 * num9;
          int num12 = n / num11;
          int num13 = num9 - 1;
          for (int index5 = 0; index5 < num13; ++index5)
          {
            num10 += num8;
            int num14 = num6;
            float num15 = (float) num10 * num5;
            float num16 = 0.0f;
            for (int index6 = 2; index6 < num12; index6 += 2)
            {
              ++num16;
              float num17 = num16 * num15;
              float[] numArray1 = wa;
              int num18 = index;
              int num19 = num14;
              int num20 = num19 + 1;
              int index7 = num18 + num19;
              double num21 = Math.Cos((double) num17);
              numArray1[index7] = (float) num21;
              float[] numArray2 = wa;
              int num22 = index;
              int num23 = num20;
              num14 = num23 + 1;
              int index8 = num22 + num23;
              double num24 = Math.Sin((double) num17);
              numArray2[index8] = (float) num24;
            }
            num6 += num12;
          }
          num8 = num11;
        }
      }

      private static void fdrffti(int n, float[] wsave, int[] ifac)
      {
        if (n == 1)
          return;
        Drft.drfti1(n, wsave, n, ifac);
      }

      private static void dradf2(int ido, int l1, float[] cc, float[] ch, float[] wa1, int index)
      {
        int index1 = 0;
        int index2;
        int num1 = index2 = l1 * ido;
        int num2 = ido << 1;
        for (int index3 = 0; index3 < l1; ++index3)
        {
          ch[index1 << 1] = cc[index1] + cc[index2];
          ch[(index1 << 1) + num2 - 1] = cc[index1] - cc[index2];
          index1 += ido;
          index2 += ido;
        }
        if (ido < 2)
          return;
        if (ido != 2)
        {
          int num3 = 0;
          int num4 = num1;
          for (int index4 = 0; index4 < l1; ++index4)
          {
            int index5 = num4;
            int index6 = (num3 << 1) + (ido << 1);
            int index7 = num3;
            int index8 = num3 + num3;
            for (int index9 = 2; index9 < ido; index9 += 2)
            {
              index5 += 2;
              index6 -= 2;
              index7 += 2;
              index8 += 2;
              float num5 = (float) ((double) wa1[index + index9 - 2] * (double) cc[index5 - 1] + (double) wa1[index + index9 - 1] * (double) cc[index5]);
              float num6 = (float) ((double) wa1[index + index9 - 2] * (double) cc[index5] - (double) wa1[index + index9 - 1] * (double) cc[index5 - 1]);
              ch[index8] = cc[index7] + num6;
              ch[index6] = num6 - cc[index7];
              ch[index8 - 1] = cc[index7 - 1] + num5;
              ch[index6 - 1] = cc[index7 - 1] - num5;
            }
            num3 += ido;
            num4 += ido;
          }
          if (ido % 2 == 1)
            return;
        }
        int index10;
        int index11;
        int index12 = (index11 = (index10 = ido) - 1) + num1;
        for (int index13 = 0; index13 < l1; ++index13)
        {
          ch[index10] = -cc[index12];
          ch[index10 - 1] = cc[index11];
          index10 += ido << 1;
          index12 += ido;
          index11 += ido;
        }
      }

      private static void dradf4(
        int ido,
        int l1,
        float[] cc,
        float[] ch,
        float[] wa1,
        int index1,
        float[] wa2,
        int index2,
        float[] wa3,
        int index3)
      {
        int num1 = l1 * ido;
        int index4 = num1;
        int index5 = index4 << 1;
        int index6 = index4 + (index4 << 1);
        int index7 = 0;
        for (int index8 = 0; index8 < l1; ++index8)
        {
          float num2 = cc[index4] + cc[index6];
          float num3 = cc[index7] + cc[index5];
          int num4;
          ch[num4 = index7 << 2] = num2 + num3;
          ch[(ido << 2) + num4 - 1] = num3 - num2;
          int index9;
          ch[(index9 = num4 + (ido << 1)) - 1] = cc[index7] - cc[index5];
          ch[index9] = cc[index6] - cc[index4];
          index4 += ido;
          index6 += ido;
          index7 += ido;
          index5 += ido;
        }
        if (ido < 2)
          return;
        if (ido != 2)
        {
          int num5 = 0;
          for (int index10 = 0; index10 < l1; ++index10)
          {
            int index11 = num5;
            int index12 = num5 << 2;
            int num6;
            int index13 = (num6 = ido << 1) + index12;
            for (int index14 = 2; index14 < ido; index14 += 2)
            {
              int num7 = index11 += 2;
              index12 += 2;
              index13 -= 2;
              int index15 = num7 + num1;
              float num8 = (float) ((double) wa1[index1 + index14 - 2] * (double) cc[index15 - 1] + (double) wa1[index1 + index14 - 1] * (double) cc[index15]);
              double num9 = (double) wa1[index1 + index14 - 2] * (double) cc[index15] - (double) wa1[index1 + index14 - 1] * (double) cc[index15 - 1];
              int index16 = index15 + num1;
              float num10 = (float) ((double) wa2[index2 + index14 - 2] * (double) cc[index16 - 1] + (double) wa2[index2 + index14 - 1] * (double) cc[index16]);
              float num11 = (float) ((double) wa2[index2 + index14 - 2] * (double) cc[index16] - (double) wa2[index2 + index14 - 1] * (double) cc[index16 - 1]);
              int index17 = index16 + num1;
              float num12 = (float) ((double) wa3[index3 + index14 - 2] * (double) cc[index17 - 1] + (double) wa3[index3 + index14 - 1] * (double) cc[index17]);
              float num13 = (float) ((double) wa3[index3 + index14 - 2] * (double) cc[index17] - (double) wa3[index3 + index14 - 1] * (double) cc[index17 - 1]);
              float num14 = num8 + num12;
              float num15 = num12 - num8;
              float num16 = (float) num9 + num13;
              float num17 = (float) num9 - num13;
              float num18 = cc[index11] + num11;
              float num19 = cc[index11] - num11;
              float num20 = cc[index11 - 1] + num10;
              float num21 = cc[index11 - 1] - num10;
              ch[index12 - 1] = num14 + num20;
              ch[index12] = num16 + num18;
              ch[index13 - 1] = num21 - num17;
              ch[index13] = num15 - num19;
              ch[index12 + num6 - 1] = num17 + num21;
              ch[index12 + num6] = num15 + num19;
              ch[index13 + num6 - 1] = num20 - num14;
              ch[index13 + num6] = num16 - num18;
            }
            num5 += ido;
          }
          if ((ido & 1) != 0)
            return;
        }
        int index18;
        int index19 = (index18 = num1 + ido - 1) + (num1 << 1);
        int num22 = ido << 2;
        int index20 = ido;
        int num23 = ido << 1;
        int num24 = ido;
        for (int index21 = 0; index21 < l1; ++index21)
        {
          float num25 = (float) (-(double) Drft.hsqt2 * ((double) cc[index18] + (double) cc[index19]));
          float num26 = Drft.hsqt2 * (cc[index18] - cc[index19]);
          ch[index20 - 1] = num26 + cc[num24 - 1];
          ch[index20 + num23 - 1] = cc[num24 - 1] - num26;
          ch[index20] = num25 - cc[index18 + num1];
          ch[index20 + num23] = num25 + cc[index18 + num1];
          index18 += ido;
          index19 += ido;
          index20 += num22;
          num24 += ido;
        }
      }

      private static void dradfg(
        int ido,
        int ip,
        int l1,
        int idl1,
        float[] cc,
        float[] c1,
        float[] c2,
        float[] ch,
        float[] ch2,
        float[] wa,
        int index)
      {
        double a;
        float num1 = (float) Math.Cos(a = (double) Drft.tpi / (double) ip);
        float num2 = (float) Math.Sin(a);
        int num3 = ip + 1 >> 1;
        int num4 = ip;
        int num5 = ido;
        int num6 = ido - 1 >> 1;
        int num7 = l1 * ido;
        int num8 = ip * ido;
        if (ido != 1)
        {
          for (int index1 = 0; index1 < idl1; ++index1)
            ch2[index1] = c2[index1];
          int num9 = 0;
          for (int index2 = 1; index2 < ip; ++index2)
          {
            num9 += num7;
            int index3 = num9;
            for (int index4 = 0; index4 < l1; ++index4)
            {
              ch[index3] = c1[index3];
              index3 += ido;
            }
          }
          int num10 = -ido;
          int num11 = 0;
          if (num6 > l1)
          {
            for (int index5 = 1; index5 < ip; ++index5)
            {
              num11 += num7;
              num10 += ido;
              int num12 = -ido + num11;
              for (int index6 = 0; index6 < l1; ++index6)
              {
                int num13 = num10 - 1;
                num12 += ido;
                int index7 = num12;
                for (int index8 = 2; index8 < ido; index8 += 2)
                {
                  num13 += 2;
                  index7 += 2;
                  ch[index7 - 1] = (float) ((double) wa[index + num13 - 1] * (double) c1[index7 - 1] + (double) wa[index + num13] * (double) c1[index7]);
                  ch[index7] = (float) ((double) wa[index + num13 - 1] * (double) c1[index7] - (double) wa[index + num13] * (double) c1[index7 - 1]);
                }
              }
            }
          }
          else
          {
            for (int index9 = 1; index9 < ip; ++index9)
            {
              num10 += ido;
              int num14 = num10 - 1;
              num11 += num7;
              int num15 = num11;
              for (int index10 = 2; index10 < ido; index10 += 2)
              {
                num14 += 2;
                num15 += 2;
                int index11 = num15;
                for (int index12 = 0; index12 < l1; ++index12)
                {
                  ch[index11 - 1] = (float) ((double) wa[index + num14 - 1] * (double) c1[index11 - 1] + (double) wa[index + num14] * (double) c1[index11]);
                  ch[index11] = (float) ((double) wa[index + num14 - 1] * (double) c1[index11] - (double) wa[index + num14] * (double) c1[index11 - 1]);
                  index11 += ido;
                }
              }
            }
          }
          int num16 = 0;
          int num17 = num4 * num7;
          if (num6 < l1)
          {
            for (int index13 = 1; index13 < num3; ++index13)
            {
              num16 += num7;
              num17 -= num7;
              int num18 = num16;
              int num19 = num17;
              for (int index14 = 2; index14 < ido; index14 += 2)
              {
                num18 += 2;
                num19 += 2;
                int index15 = num18 - ido;
                int index16 = num19 - ido;
                for (int index17 = 0; index17 < l1; ++index17)
                {
                  index15 += ido;
                  index16 += ido;
                  c1[index15 - 1] = ch[index15 - 1] + ch[index16 - 1];
                  c1[index16 - 1] = ch[index15] - ch[index16];
                  c1[index15] = ch[index15] + ch[index16];
                  c1[index16] = ch[index16 - 1] - ch[index15 - 1];
                }
              }
            }
          }
          else
          {
            for (int index18 = 1; index18 < num3; ++index18)
            {
              num16 += num7;
              num17 -= num7;
              int num20 = num16;
              int num21 = num17;
              for (int index19 = 0; index19 < l1; ++index19)
              {
                int index20 = num20;
                int index21 = num21;
                for (int index22 = 2; index22 < ido; index22 += 2)
                {
                  index20 += 2;
                  index21 += 2;
                  c1[index20 - 1] = ch[index20 - 1] + ch[index21 - 1];
                  c1[index21 - 1] = ch[index20] - ch[index21];
                  c1[index20] = ch[index20] + ch[index21];
                  c1[index21] = ch[index21 - 1] - ch[index20 - 1];
                }
                num20 += ido;
                num21 += ido;
              }
            }
          }
        }
        for (int index23 = 0; index23 < idl1; ++index23)
          c2[index23] = ch2[index23];
        int num22 = 0;
        int num23 = num4 * idl1;
        for (int index24 = 1; index24 < num3; ++index24)
        {
          num22 += num7;
          num23 -= num7;
          int index25 = num22 - ido;
          int index26 = num23 - ido;
          for (int index27 = 0; index27 < l1; ++index27)
          {
            index25 += ido;
            index26 += ido;
            c1[index25] = ch[index25] + ch[index26];
            c1[index26] = ch[index26] - ch[index25];
          }
        }
        float num24 = 1f;
        float num25 = 0.0f;
        int num26 = 0;
        int num27 = num4 * idl1;
        int num28 = (ip - 1) * idl1;
        for (int index28 = 1; index28 < num3; ++index28)
        {
          num26 += idl1;
          num27 -= idl1;
          float num29 = (float) ((double) num1 * (double) num24 - (double) num2 * (double) num25);
          num25 = (float) ((double) num1 * (double) num25 + (double) num2 * (double) num24);
          num24 = num29;
          int num30 = num26;
          int num31 = num27;
          int num32 = num28;
          int num33 = idl1;
          for (int index29 = 0; index29 < idl1; ++index29)
          {
            ch2[num30++] = c2[index29] + num24 * c2[num33++];
            ch2[num31++] = num25 * c2[num32++];
          }
          float num34 = num24;
          float num35 = num25;
          float num36 = num24;
          float num37 = num25;
          int num38 = idl1;
          int num39 = (num4 - 1) * idl1;
          for (int index30 = 2; index30 < num3; ++index30)
          {
            num38 += idl1;
            num39 -= idl1;
            float num40 = (float) ((double) num34 * (double) num36 - (double) num35 * (double) num37);
            num37 = (float) ((double) num34 * (double) num37 + (double) num35 * (double) num36);
            num36 = num40;
            int index31 = num26;
            int index32 = num27;
            int num41 = num38;
            int num42 = num39;
            for (int index33 = 0; index33 < idl1; ++index33)
            {
              ch2[index31] += num36 * c2[num41++];
              ++index31;
              ch2[index32] += num37 * c2[num42++];
              ++index32;
            }
          }
        }
        int num43 = 0;
        for (int index34 = 1; index34 < num3; ++index34)
        {
          num43 += idl1;
          int num44 = num43;
          for (int index35 = 0; index35 < idl1; ++index35)
            ch2[index35] += c2[num44++];
        }
        if (ido >= l1)
        {
          int num45 = 0;
          int num46 = 0;
          for (int index36 = 0; index36 < l1; ++index36)
          {
            int num47 = num45;
            int num48 = num46;
            for (int index37 = 0; index37 < ido; ++index37)
              cc[num48++] = ch[num47++];
            num45 += ido;
            num46 += num8;
          }
        }
        else
        {
          for (int index38 = 0; index38 < ido; ++index38)
          {
            int index39 = index38;
            int index40 = index38;
            for (int index41 = 0; index41 < l1; ++index41)
            {
              cc[index40] = ch[index39];
              index39 += ido;
              index40 += num8;
            }
          }
        }
        int num49 = 0;
        int num50 = ido << 1;
        int num51 = 0;
        int num52 = num4 * num7;
        for (int index42 = 1; index42 < num3; ++index42)
        {
          num49 += num50;
          num51 += num7;
          num52 -= num7;
          int index43 = num49;
          int index44 = num51;
          int index45 = num52;
          for (int index46 = 0; index46 < l1; ++index46)
          {
            cc[index43 - 1] = ch[index44];
            cc[index43] = ch[index45];
            index43 += num8;
            index44 += ido;
            index45 += ido;
          }
        }
        if (ido == 1)
          return;
        if (num6 >= l1)
        {
          int num53 = -ido;
          int num54 = 0;
          int num55 = 0;
          int num56 = num4 * num7;
          for (int index47 = 1; index47 < num3; ++index47)
          {
            num53 += num50;
            num54 += num50;
            num55 += num7;
            num56 -= num7;
            int num57 = num53;
            int num58 = num54;
            int num59 = num55;
            int num60 = num56;
            for (int index48 = 0; index48 < l1; ++index48)
            {
              for (int index49 = 2; index49 < ido; index49 += 2)
              {
                int num61 = num5 - index49;
                cc[index49 + num58 - 1] = ch[index49 + num59 - 1] + ch[index49 + num60 - 1];
                cc[num61 + num57 - 1] = ch[index49 + num59 - 1] - ch[index49 + num60 - 1];
                cc[index49 + num58] = ch[index49 + num59] + ch[index49 + num60];
                cc[num61 + num57] = ch[index49 + num60] - ch[index49 + num59];
              }
              num57 += num8;
              num58 += num8;
              num59 += ido;
              num60 += ido;
            }
          }
        }
        else
        {
          int num62 = -ido;
          int num63 = 0;
          int num64 = 0;
          int num65 = num4 * num7;
          for (int index50 = 1; index50 < num3; ++index50)
          {
            num62 += num50;
            num63 += num50;
            num64 += num7;
            num65 -= num7;
            for (int index51 = 2; index51 < ido; index51 += 2)
            {
              int index52 = num5 + num62 - index51;
              int index53 = index51 + num63;
              int index54 = index51 + num64;
              int index55 = index51 + num65;
              for (int index56 = 0; index56 < l1; ++index56)
              {
                cc[index53 - 1] = ch[index54 - 1] + ch[index55 - 1];
                cc[index52 - 1] = ch[index54 - 1] - ch[index55 - 1];
                cc[index53] = ch[index54] + ch[index55];
                cc[index52] = ch[index55] - ch[index54];
                index52 += num8;
                index53 += num8;
                index54 += ido;
                index55 += ido;
              }
            }
          }
        }
      }

      private static void drftf1(int n, float[] c, float[] ch, float[] wa, int[] ifac)
      {
        int num1 = ifac[1];
        int num2 = 1;
        int num3 = n;
        int num4 = n;
        for (int index = 0; index < num1; ++index)
        {
          int num5 = num1 - index;
          int ip = ifac[num5 + 1];
          int l1 = num3 / ip;
          int ido = n / num3;
          int idl1 = ido * l1;
          num4 -= (ip - 1) * ido;
          num2 = 1 - num2;
          switch (ip)
          {
            case 2:
              if (num2 == 0)
              {
                Drft.dradf2(ido, l1, c, ch, wa, num4 - 1);
                break;
              }
              Drft.dradf2(ido, l1, ch, c, wa, num4 - 1);
              break;
            case 4:
              int num6 = num4 + ido;
              int num7 = num6 + ido;
              if (num2 != 0)
              {
                Drft.dradf4(ido, l1, ch, c, wa, num4 - 1, wa, num6 - 1, wa, num7 - 1);
                break;
              }
              Drft.dradf4(ido, l1, c, ch, wa, num4 - 1, wa, num6 - 1, wa, num7 - 1);
              break;
            default:
              if (ido == 1)
                num2 = 1 - num2;
              if (num2 == 0)
              {
                Drft.dradfg(ido, ip, l1, idl1, c, c, c, ch, ch, wa, num4 - 1);
                num2 = 1;
                break;
              }
              Drft.dradfg(ido, ip, l1, idl1, ch, ch, ch, c, c, wa, num4 - 1);
              num2 = 0;
              goto label_15;
          }
          num3 = l1;
        }
    label_15:
        if (num2 == 1)
          return;
        for (int index = 0; index < n; ++index)
          c[index] = ch[index];
      }

      private static void dradb2(int ido, int l1, float[] cc, float[] ch, float[] wa1, int index)
      {
        int num1 = l1 * ido;
        int index1 = 0;
        int index2 = 0;
        int num2 = (ido << 1) - 1;
        for (int index3 = 0; index3 < l1; ++index3)
        {
          ch[index1] = cc[index2] + cc[num2 + index2];
          ch[index1 + num1] = cc[index2] - cc[num2 + index2];
          index2 = (index1 += ido) << 1;
        }
        if (ido < 2)
          return;
        if (ido != 2)
        {
          int num3 = 0;
          int num4 = 0;
          for (int index4 = 0; index4 < l1; ++index4)
          {
            int index5 = num3;
            int index6;
            int index7 = (index6 = num4) + (ido << 1);
            int index8 = num1 + num3;
            for (int index9 = 2; index9 < ido; index9 += 2)
            {
              index5 += 2;
              index6 += 2;
              index7 -= 2;
              index8 += 2;
              ch[index5 - 1] = cc[index6 - 1] + cc[index7 - 1];
              float num5 = cc[index6 - 1] - cc[index7 - 1];
              ch[index5] = cc[index6] - cc[index7];
              float num6 = cc[index6] + cc[index7];
              ch[index8 - 1] = (float) ((double) wa1[index + index9 - 2] * (double) num5 - (double) wa1[index + index9 - 1] * (double) num6);
              ch[index8] = (float) ((double) wa1[index + index9 - 2] * (double) num6 + (double) wa1[index + index9 - 1] * (double) num5);
            }
            num4 = (num3 += ido) << 1;
          }
          if (ido % 2 == 1)
            return;
        }
        int index10 = ido - 1;
        int index11 = ido - 1;
        for (int index12 = 0; index12 < l1; ++index12)
        {
          ch[index10] = cc[index11] + cc[index11];
          ch[index10 + num1] = (float) -((double) cc[index11 + 1] + (double) cc[index11 + 1]);
          index10 += ido;
          index11 += ido << 1;
        }
      }

      private static void dradb3(
        int ido,
        int l1,
        float[] cc,
        float[] ch,
        float[] wa1,
        int index1,
        float[] wa2,
        int index2)
      {
        int num1 = l1 * ido;
        int index3 = 0;
        int num2 = num1 << 1;
        int index4 = ido << 1;
        int num3 = ido + (ido << 1);
        int index5 = 0;
        for (int index6 = 0; index6 < l1; ++index6)
        {
          float num4 = cc[index4 - 1] + cc[index4 - 1];
          float num5 = cc[index5] + Drft.taur * num4;
          ch[index3] = cc[index5] + num4;
          float num6 = Drft.taui * (cc[index4] + cc[index4]);
          ch[index3 + num1] = num5 - num6;
          ch[index3 + num2] = num5 + num6;
          index3 += ido;
          index4 += num3;
          index5 += num3;
        }
        if (ido == 1)
          return;
        int num7 = 0;
        int num8 = ido << 1;
        for (int index7 = 0; index7 < l1; ++index7)
        {
          int index8 = num7 + (num7 << 1);
          int index9;
          int index10 = index9 = index8 + num8;
          int index11 = num7;
          int index12;
          int index13 = (index12 = num7 + num1) + num1;
          for (int index14 = 2; index14 < ido; index14 += 2)
          {
            index9 += 2;
            index10 -= 2;
            index8 += 2;
            index11 += 2;
            index12 += 2;
            index13 += 2;
            float num9 = cc[index9 - 1] + cc[index10 - 1];
            float num10 = cc[index8 - 1] + Drft.taur * num9;
            ch[index11 - 1] = cc[index8 - 1] + num9;
            float num11 = cc[index9] - cc[index10];
            double num12 = (double) cc[index8] + (double) Drft.taur * (double) num11;
            ch[index11] = cc[index8] + num11;
            float num13 = Drft.taui * (cc[index9 - 1] - cc[index10 - 1]);
            float num14 = Drft.taui * (cc[index9] + cc[index10]);
            float num15 = num10 - num14;
            float num16 = num10 + num14;
            float num17 = (float) num12 + num13;
            float num18 = (float) num12 - num13;
            ch[index12 - 1] = (float) ((double) wa1[index1 + index14 - 2] * (double) num15 - (double) wa1[index1 + index14 - 1] * (double) num17);
            ch[index12] = (float) ((double) wa1[index1 + index14 - 2] * (double) num17 + (double) wa1[index1 + index14 - 1] * (double) num15);
            ch[index13 - 1] = (float) ((double) wa2[index2 + index14 - 2] * (double) num16 - (double) wa2[index2 + index14 - 1] * (double) num18);
            ch[index13] = (float) ((double) wa2[index2 + index14 - 2] * (double) num18 + (double) wa2[index2 + index14 - 1] * (double) num16);
          }
          num7 += ido;
        }
      }

      private static void dradb4(
        int ido,
        int l1,
        float[] cc,
        float[] ch,
        float[] wa1,
        int index1,
        float[] wa2,
        int index2,
        float[] wa3,
        int index3)
      {
        int num1 = l1 * ido;
        int num2 = 0;
        int num3 = ido << 2;
        int index4 = 0;
        int num4 = ido << 1;
        int num5;
        for (int index5 = 0; index5 < l1; ++index5)
        {
          int index6 = index4 + num4;
          int index7 = num2;
          float num6 = cc[index6 - 1] + cc[index6 - 1];
          float num7 = cc[index6] + cc[index6];
          int num8;
          float num9 = cc[index4] - cc[(num8 = index6 + num4) - 1];
          float num10 = cc[index4] + cc[num8 - 1];
          ch[index7] = num10 + num6;
          int num11;
          ch[num11 = index7 + num1] = num9 - num7;
          int num12;
          ch[num12 = num11 + num1] = num10 - num6;
          ch[num5 = num12 + num1] = num9 + num7;
          num2 += ido;
          index4 += num3;
        }
        if (ido < 2)
          return;
        if (ido != 2)
        {
          int num13 = 0;
          for (int index8 = 0; index8 < l1; ++index8)
          {
            int index9;
            int num14;
            int index10 = num14 = (index9 = num13 << 2) + num4;
            int index11 = num14;
            int index12 = num14 + num4;
            int index13 = num13;
            for (int index14 = 2; index14 < ido; index14 += 2)
            {
              index9 += 2;
              index10 += 2;
              index11 -= 2;
              index12 -= 2;
              index13 += 2;
              float num15 = cc[index9] + cc[index12];
              float num16 = cc[index9] - cc[index12];
              float num17 = cc[index10] - cc[index11];
              float num18 = cc[index10] + cc[index11];
              float num19 = cc[index9 - 1] - cc[index12 - 1];
              float num20 = cc[index9 - 1] + cc[index12 - 1];
              float num21 = cc[index10 - 1] - cc[index11 - 1];
              float num22 = cc[index10 - 1] + cc[index11 - 1];
              ch[index13 - 1] = num20 + num22;
              float num23 = num20 - num22;
              ch[index13] = num16 + num17;
              float num24 = num16 - num17;
              float num25 = num19 - num18;
              float num26 = num19 + num18;
              float num27 = num15 + num21;
              float num28 = num15 - num21;
              int index15;
              ch[(index15 = index13 + num1) - 1] = (float) ((double) wa1[index1 + index14 - 2] * (double) num25 - (double) wa1[index1 + index14 - 1] * (double) num27);
              ch[index15] = (float) ((double) wa1[index1 + index14 - 2] * (double) num27 + (double) wa1[index1 + index14 - 1] * (double) num25);
              int index16;
              ch[(index16 = index15 + num1) - 1] = (float) ((double) wa2[index2 + index14 - 2] * (double) num23 - (double) wa2[index2 + index14 - 1] * (double) num24);
              ch[index16] = (float) ((double) wa2[index2 + index14 - 2] * (double) num24 + (double) wa2[index2 + index14 - 1] * (double) num23);
              int index17;
              ch[(index17 = index16 + num1) - 1] = (float) ((double) wa3[index3 + index14 - 2] * (double) num26 - (double) wa3[index3 + index14 - 1] * (double) num28);
              ch[index17] = (float) ((double) wa3[index3 + index14 - 2] * (double) num28 + (double) wa3[index3 + index14 - 1] * (double) num26);
            }
            num13 += ido;
          }
          if (ido % 2 == 1)
            return;
        }
        int index18 = ido;
        int num29 = ido << 2;
        int num30 = ido - 1;
        int index19 = ido + (ido << 1);
        for (int index20 = 0; index20 < l1; ++index20)
        {
          int index21 = num30;
          float num31 = cc[index18] + cc[index19];
          float num32 = cc[index19] - cc[index18];
          float num33 = cc[index18 - 1] - cc[index19 - 1];
          float num34 = cc[index18 - 1] + cc[index19 - 1];
          ch[index21] = num34 + num34;
          int num35;
          ch[num35 = index21 + num1] = Drft.sqrt2 * (num33 - num31);
          int num36;
          ch[num36 = num35 + num1] = num32 + num32;
          ch[num5 = num36 + num1] = (float) (-(double) Drft.sqrt2 * ((double) num33 + (double) num31));
          num30 += ido;
          index18 += num29;
          index19 += num29;
        }
      }

      private static void dradbg(
        int ido,
        int ip,
        int l1,
        int idl1,
        float[] cc,
        float[] c1,
        float[] c2,
        float[] ch,
        float[] ch2,
        float[] wa,
        int index)
      {
        int num1 = ip * ido;
        int num2 = l1 * ido;
        double a;
        float num3 = (float) Math.Cos(a = (double) Drft.tpi / (double) ip);
        float num4 = (float) Math.Sin(a);
        int num5 = ido - 1 >>> 1;
        int num6 = ip;
        int num7 = ip + 1 >>> 1;
        if (ido >= l1)
        {
          int num8 = 0;
          int num9 = 0;
          for (int index1 = 0; index1 < l1; ++index1)
          {
            int index2 = num8;
            int index3 = num9;
            for (int index4 = 0; index4 < ido; ++index4)
            {
              ch[index2] = cc[index3];
              ++index2;
              ++index3;
            }
            num8 += ido;
            num9 += num1;
          }
        }
        else
        {
          int num10 = 0;
          for (int index5 = 0; index5 < ido; ++index5)
          {
            int index6 = num10;
            int index7 = num10;
            for (int index8 = 0; index8 < l1; ++index8)
            {
              ch[index6] = cc[index7];
              index6 += ido;
              index7 += num1;
            }
            ++num10;
          }
        }
        int num11 = 0;
        int num12 = num6 * num2;
        int num13;
        int num14 = num13 = ido << 1;
        for (int index9 = 1; index9 < num7; ++index9)
        {
          num11 += num2;
          num12 -= num2;
          int index10 = num11;
          int index11 = num12;
          int index12 = num13;
          for (int index13 = 0; index13 < l1; ++index13)
          {
            ch[index10] = cc[index12 - 1] + cc[index12 - 1];
            ch[index11] = cc[index12] + cc[index12];
            index10 += ido;
            index11 += ido;
            index12 += num1;
          }
          num13 += num14;
        }
        if (ido != 1)
        {
          if (num5 >= l1)
          {
            int num15 = 0;
            int num16 = num6 * num2;
            int num17 = 0;
            for (int index14 = 1; index14 < num7; ++index14)
            {
              num15 += num2;
              num16 -= num2;
              int num18 = num15;
              int num19 = num16;
              num17 += ido << 1;
              int num20 = num17;
              for (int index15 = 0; index15 < l1; ++index15)
              {
                int index16 = num18;
                int index17 = num19;
                int index18 = num20;
                int index19 = num20;
                for (int index20 = 2; index20 < ido; index20 += 2)
                {
                  index16 += 2;
                  index17 += 2;
                  index18 += 2;
                  index19 -= 2;
                  ch[index16 - 1] = cc[index18 - 1] + cc[index19 - 1];
                  ch[index17 - 1] = cc[index18 - 1] - cc[index19 - 1];
                  ch[index16] = cc[index18] - cc[index19];
                  ch[index17] = cc[index18] + cc[index19];
                }
                num18 += ido;
                num19 += ido;
                num20 += num1;
              }
            }
          }
          else
          {
            int num21 = 0;
            int num22 = num6 * num2;
            int num23 = 0;
            for (int index21 = 1; index21 < num7; ++index21)
            {
              num21 += num2;
              num22 -= num2;
              int num24 = num21;
              int num25 = num22;
              num23 += ido << 1;
              int num26 = num23;
              int num27 = num23;
              for (int index22 = 2; index22 < ido; index22 += 2)
              {
                num24 += 2;
                num25 += 2;
                num26 += 2;
                num27 -= 2;
                int index23 = num24;
                int index24 = num25;
                int index25 = num26;
                int index26 = num27;
                for (int index27 = 0; index27 < l1; ++index27)
                {
                  ch[index23 - 1] = cc[index25 - 1] + cc[index26 - 1];
                  ch[index24 - 1] = cc[index25 - 1] - cc[index26 - 1];
                  ch[index23] = cc[index25] - cc[index26];
                  ch[index24] = cc[index25] + cc[index26];
                  index23 += ido;
                  index24 += ido;
                  index25 += num1;
                  index26 += num1;
                }
              }
            }
          }
        }
        float num28 = 1f;
        float num29 = 0.0f;
        int num30 = 0;
        int num31;
        int num32 = num31 = num6 * idl1;
        int num33 = (ip - 1) * idl1;
        for (int index28 = 1; index28 < num7; ++index28)
        {
          num30 += idl1;
          num31 -= idl1;
          float num34 = (float) ((double) num3 * (double) num28 - (double) num4 * (double) num29);
          num29 = (float) ((double) num3 * (double) num29 + (double) num4 * (double) num28);
          num28 = num34;
          int num35 = num30;
          int num36 = num31;
          int num37 = 0;
          int num38 = idl1;
          int num39 = num33;
          for (int index29 = 0; index29 < idl1; ++index29)
          {
            c2[num35++] = ch2[num37++] + num28 * ch2[num38++];
            c2[num36++] = num29 * ch2[num39++];
          }
          float num40 = num28;
          float num41 = num29;
          float num42 = num28;
          float num43 = num29;
          int num44 = idl1;
          int num45 = num32 - idl1;
          for (int index30 = 2; index30 < num7; ++index30)
          {
            num44 += idl1;
            num45 -= idl1;
            float num46 = (float) ((double) num40 * (double) num42 - (double) num41 * (double) num43);
            num43 = (float) ((double) num40 * (double) num43 + (double) num41 * (double) num42);
            num42 = num46;
            int index31 = num30;
            int index32 = num31;
            int num47 = num44;
            int num48 = num45;
            for (int index33 = 0; index33 < idl1; ++index33)
            {
              c2[index31] += num42 * ch2[num47++];
              ++index31;
              c2[index32] += num43 * ch2[num48++];
              ++index32;
            }
          }
        }
        int num49 = 0;
        for (int index34 = 1; index34 < num7; ++index34)
        {
          num49 += idl1;
          int num50 = num49;
          for (int index35 = 0; index35 < idl1; ++index35)
            ch2[index35] += ch2[num50++];
        }
        int num51 = 0;
        int num52 = num6 * num2;
        for (int index36 = 1; index36 < num7; ++index36)
        {
          num51 += num2;
          num52 -= num2;
          int index37 = num51;
          int index38 = num52;
          for (int index39 = 0; index39 < l1; ++index39)
          {
            ch[index37] = c1[index37] - c1[index38];
            ch[index38] = c1[index37] + c1[index38];
            index37 += ido;
            index38 += ido;
          }
        }
        if (ido != 1)
        {
          if (num5 >= l1)
          {
            int num53 = 0;
            int num54 = num6 * num2;
            for (int index40 = 1; index40 < num7; ++index40)
            {
              num53 += num2;
              num54 -= num2;
              int num55 = num53;
              int num56 = num54;
              for (int index41 = 0; index41 < l1; ++index41)
              {
                int index42 = num55;
                int index43 = num56;
                for (int index44 = 2; index44 < ido; index44 += 2)
                {
                  index42 += 2;
                  index43 += 2;
                  ch[index42 - 1] = c1[index42 - 1] - c1[index43];
                  ch[index43 - 1] = c1[index42 - 1] + c1[index43];
                  ch[index42] = c1[index42] + c1[index43 - 1];
                  ch[index43] = c1[index42] - c1[index43 - 1];
                }
                num55 += ido;
                num56 += ido;
              }
            }
          }
          else
          {
            int num57 = 0;
            int num58 = num6 * num2;
            for (int index45 = 1; index45 < num7; ++index45)
            {
              num57 += num2;
              num58 -= num2;
              int num59 = num57;
              int num60 = num58;
              for (int index46 = 2; index46 < ido; index46 += 2)
              {
                num59 += 2;
                num60 += 2;
                int index47 = num59;
                int index48 = num60;
                for (int index49 = 0; index49 < l1; ++index49)
                {
                  ch[index47 - 1] = c1[index47 - 1] - c1[index48];
                  ch[index48 - 1] = c1[index47 - 1] + c1[index48];
                  ch[index47] = c1[index47] + c1[index48 - 1];
                  ch[index48] = c1[index47] - c1[index48 - 1];
                  index47 += ido;
                  index48 += ido;
                }
              }
            }
          }
        }
        if (ido == 1)
          return;
        for (int index50 = 0; index50 < idl1; ++index50)
          c2[index50] = ch2[index50];
        int num61 = 0;
        for (int index51 = 1; index51 < ip; ++index51)
        {
          int index52 = num61 += num2;
          for (int index53 = 0; index53 < l1; ++index53)
          {
            c1[index52] = ch[index52];
            index52 += ido;
          }
        }
        if (num5 <= l1)
        {
          int num62 = -ido - 1;
          int num63 = 0;
          for (int index54 = 1; index54 < ip; ++index54)
          {
            num62 += ido;
            num63 += num2;
            int num64 = num62;
            int num65 = num63;
            for (int index55 = 2; index55 < ido; index55 += 2)
            {
              num65 += 2;
              num64 += 2;
              int index56 = num65;
              for (int index57 = 0; index57 < l1; ++index57)
              {
                c1[index56 - 1] = (float) ((double) wa[index + num64 - 1] * (double) ch[index56 - 1] - (double) wa[index + num64] * (double) ch[index56]);
                c1[index56] = (float) ((double) wa[index + num64 - 1] * (double) ch[index56] + (double) wa[index + num64] * (double) ch[index56 - 1]);
                index56 += ido;
              }
            }
          }
        }
        else
        {
          int num66 = -ido - 1;
          int num67 = 0;
          for (int index58 = 1; index58 < ip; ++index58)
          {
            num66 += ido;
            num67 += num2;
            int num68 = num67;
            for (int index59 = 0; index59 < l1; ++index59)
            {
              int num69 = num66;
              int index60 = num68;
              for (int index61 = 2; index61 < ido; index61 += 2)
              {
                num69 += 2;
                index60 += 2;
                c1[index60 - 1] = (float) ((double) wa[index + num69 - 1] * (double) ch[index60 - 1] - (double) wa[index + num69] * (double) ch[index60]);
                c1[index60] = (float) ((double) wa[index + num69 - 1] * (double) ch[index60] + (double) wa[index + num69] * (double) ch[index60 - 1]);
              }
              num68 += ido;
            }
          }
        }
      }

      private static void drftb1(int n, float[] c, float[] ch, float[] wa, int index, int[] ifac)
      {
        int num1 = ifac[1];
        int num2 = 0;
        int l1 = 1;
        int num3 = 1;
        for (int index1 = 0; index1 < num1; ++index1)
        {
          int ip = ifac[index1 + 2];
          int num4 = ip * l1;
          int ido = n / num4;
          int idl1 = ido * l1;
          switch (ip)
          {
            case 2:
              if (num2 != 0)
                Drft.dradb2(ido, l1, ch, c, wa, index + num3 - 1);
              else
                Drft.dradb2(ido, l1, c, ch, wa, index + num3 - 1);
              num2 = 1 - num2;
              break;
            case 3:
              int num5 = num3 + ido;
              if (num2 != 0)
                Drft.dradb3(ido, l1, ch, c, wa, index + num3 - 1, wa, index + num5 - 1);
              else
                Drft.dradb3(ido, l1, c, ch, wa, index + num3 - 1, wa, index + num5 - 1);
              num2 = 1 - num2;
              break;
            case 4:
              int num6 = num3 + ido;
              int num7 = num6 + ido;
              if (num2 != 0)
                Drft.dradb4(ido, l1, ch, c, wa, index + num3 - 1, wa, index + num6 - 1, wa, index + num7 - 1);
              else
                Drft.dradb4(ido, l1, c, ch, wa, index + num3 - 1, wa, index + num6 - 1, wa, index + num7 - 1);
              num2 = 1 - num2;
              break;
            default:
              if (num2 != 0)
                Drft.dradbg(ido, ip, l1, idl1, ch, ch, ch, c, c, wa, index + num3 - 1);
              else
                Drft.dradbg(ido, ip, l1, idl1, c, c, c, ch, ch, wa, index + num3 - 1);
              if (ido == 1)
                num2 = 1 - num2;
              num3 += (ip - 1) * ido;
              break;
          }
          l1 = num4;
          num3 += (ip - 1) * ido;
        }
        if (num2 == 0)
          return;
        for (int index2 = 0; index2 < n; ++index2)
          c[index2] = ch[index2];
      }
    }
}
