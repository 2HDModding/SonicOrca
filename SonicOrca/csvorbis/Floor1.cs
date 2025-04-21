// Decompiled with JetBrains decompiler
// Type: csvorbis.Floor1
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;
using System;

namespace csvorbis
{

    internal class Floor1 : FuncFloor
    {
      private static int floor1_rangedb = 140;
      private static int VIF_POSIT = 63 /*0x3F*/;
      private static float[] FLOOR_fromdB_LOOKUP = new float[256 /*0x0100*/]
      {
        1.06498632E-07f,
        1.1341951E-07f,
        1.20790148E-07f,
        1.28639783E-07f,
        1.369995E-07f,
        1.459025E-07f,
        1.55384086E-07f,
        1.65481808E-07f,
        1.76235744E-07f,
        1.87688556E-07f,
        1.998856E-07f,
        2.128753E-07f,
        2.26709133E-07f,
        2.41441967E-07f,
        2.57132228E-07f,
        2.73842119E-07f,
        2.91637917E-07f,
        3.10590224E-07f,
        3.307741E-07f,
        3.52269666E-07f,
        3.75162131E-07f,
        3.995423E-07f,
        4.255068E-07f,
        4.53158634E-07f,
        4.82607447E-07f,
        5.1397E-07f,
        5.47370632E-07f,
        5.829419E-07f,
        6.208247E-07f,
        6.611694E-07f,
        7.041359E-07f,
        7.49894639E-07f,
        7.98627E-07f,
        8.505263E-07f,
        9.057983E-07f,
        9.646621E-07f,
        1.02735135E-06f,
        1.0941144E-06f,
        1.16521608E-06f,
        1.24093845E-06f,
        1.32158164E-06f,
        1.40746545E-06f,
        1.49893049E-06f,
        1.59633942E-06f,
        1.70007854E-06f,
        1.81055918E-06f,
        1.92821949E-06f,
        2.053526E-06f,
        2.18697573E-06f,
        2.3290977E-06f,
        2.48045581E-06f,
        2.64164964E-06f,
        2.813319E-06f,
        2.9961443E-06f,
        3.19085052E-06f,
        3.39821E-06f,
        3.619045E-06f,
        3.85423073E-06f,
        4.10470057E-06f,
        4.371447E-06f,
        4.6555283E-06f,
        4.958071E-06f,
        5.280274E-06f,
        5.623416E-06f,
        5.988857E-06f,
        6.37804669E-06f,
        6.79252844E-06f,
        7.23394533E-06f,
        7.704048E-06f,
        8.2047E-06f,
        8.737888E-06f,
        9.305725E-06f,
        9.910464E-06f,
        1.05545014E-05f,
        1.12403923E-05f,
        1.19708557E-05f,
        1.27487892E-05f,
        1.3577278E-05f,
        1.44596061E-05f,
        1.53992714E-05f,
        1.64000048E-05f,
        1.74657689E-05f,
        1.86007928E-05f,
        1.98095768E-05f,
        2.10969138E-05f,
        2.24679115E-05f,
        2.39280016E-05f,
        2.54829774E-05f,
        2.71390054E-05f,
        2.890265E-05f,
        3.078091E-05f,
        3.27812268E-05f,
        3.49115326E-05f,
        3.718028E-05f,
        3.95964671E-05f,
        4.21696677E-05f,
        4.491009E-05f,
        4.7828602E-05f,
        5.09367746E-05f,
        5.424693E-05f,
        5.77722021E-05f,
        6.152657E-05f,
        6.552491E-05f,
        6.97830837E-05f,
        7.43179844E-05f,
        7.914758E-05f,
        8.429104E-05f,
        8.976875E-05f,
        9.560242E-05f,
        0.000101815211f,
        0.000108431741f,
        0.000115478237f,
        0.000122982674f,
        0.000130974775f,
        0.000139486248f,
        0.000148550855f,
        0.000158204537f,
        0.000168485552f,
        0.00017943469f,
        0.000191095358f,
        0.000203513817f,
        0.0002167393f,
        0.000230824226f,
        0.000245824485f,
        0.000261799549f,
        0.000278812746f,
        0.000296931568f,
        0.000316227874f,
        0.000336778146f,
        0.000358663878f,
        0.000381971884f,
        0.00040679457f,
        0.000433230365f,
        0.0004613841f,
        0.0004913675f,
        1f / (703f * (float)Math.E),
        0.0005573062f,
        0.0005935231f,
        0.0006320936f,
        0.0006731706f,
        0.000716917f,
        0.0007635063f,
        0.000813123246f,
        0.000865964568f,
        0.000922239851f,
        0.0009821722f,
        0.00104599923f,
        0.00111397426f,
        0.00118636654f,
        0.00126346329f,
        0.0013455702f,
        0.00143301289f,
        0.00152613816f,
        0.00162531529f,
        0.00173093739f,
        0.00184342347f,
        0.00196321961f,
        0.00209080055f,
        0.0022266726f,
        0.00237137428f,
        0.00252547953f,
        0.00268959929f,
        0.00286438479f,
        0.0030505287f,
        0.003248769f,
        0.00345989247f,
        0.00368473586f,
        0.00392419053f,
        0.00417920668f,
        0.004450795f,
        0.004740033f,
        0.005048067f,
        0.0053761187f,
        0.005725489f,
        0.00609756354f,
        0.00649381755f,
        0.00691582263f,
        0.00736525143f,
        0.007843887f,
        0.008353627f,
        0.008896492f,
        0.009474637f,
        0.010090352f,
        0.01074608f,
        0.0114444206f,
        0.012188144f,
        0.0129801976f,
        0.0138237253f,
        0.0147220679f,
        0.0156787913f,
        0.0166976862f,
        0.0177827962f,
        0.0189384222f,
        0.0201691482f,
        0.0214798544f,
        0.0228757355f,
        0.02436233f,
        0.0259455312f,
        0.0276316181f,
        0.0294272769f,
        0.0313396268f,
        0.03337625f,
        0.0355452262f,
        0.0378551558f,
        0.0403152f,
        0.0429351069f,
        0.0457252748f,
        0.0486967564f,
        0.05186135f,
        0.05523159f,
        0.05882085f,
        0.0626433641f,
        0.06671428f,
        0.07104975f,
        0.0756669641f,
        0.08058423f,
        0.08582105f,
        0.09139818f,
        0.0973377451f,
        0.1036633f,
        0.110399932f,
        0.117574342f,
        0.125214979f,
        0.133352146f,
        0.142018124f,
        0.151247263f,
        0.161076173f,
        0.1715438f,
        0.182691678f,
        0.194564015f,
        0.207207873f,
        0.220673427f,
        0.235014021f,
        0.250286549f,
        0.266551584f,
        0.283873618f,
        0.3023213f,
        0.32196787f,
        69f * (float) Math.E / 547f,
        0.365174145f,
        0.3889052f,
        0.414178461f,
        0.44109413f,
        0.4697589f,
        0.50028646f,
        0.532797933f,
        0.5674221f,
        0.6042964f,
        0.643566966f,
        0.6853896f,
        0.729930043f,
        0.777365f,
        0.8278826f,
        0.881683052f,
        0.9389798f,
        1f
      };

      public override void pack(object i, csBuffer opb)
      {
        InfoFloor1 infoFloor1 = (InfoFloor1) i;
        int num1 = 0;
        int v = infoFloor1.postlist[1];
        int num2 = -1;
        opb.write(infoFloor1.partitions, 5);
        for (int index = 0; index < infoFloor1.partitions; ++index)
        {
          opb.write(infoFloor1.partitionclass[index], 4);
          if (num2 < infoFloor1.partitionclass[index])
            num2 = infoFloor1.partitionclass[index];
        }
        for (int index1 = 0; index1 < num2 + 1; ++index1)
        {
          opb.write(infoFloor1.class_dim[index1] - 1, 3);
          opb.write(infoFloor1.class_subs[index1], 2);
          if (infoFloor1.class_subs[index1] != 0)
            opb.write(infoFloor1.class_book[index1], 8);
          for (int index2 = 0; index2 < 1 << infoFloor1.class_subs[index1]; ++index2)
            opb.write(infoFloor1.class_subbook[index1][index2] + 1, 8);
        }
        opb.write(infoFloor1.mult - 1, 2);
        opb.write(Floor1.ilog2(v), 4);
        int bits = Floor1.ilog2(v);
        int index3 = 0;
        int num3 = 0;
        for (; index3 < infoFloor1.partitions; ++index3)
        {
          for (num1 += infoFloor1.class_dim[infoFloor1.partitionclass[index3]]; num3 < num1; ++num3)
            opb.write(infoFloor1.postlist[num3 + 2], bits);
        }
      }

      public override object unpack(Info vi, csBuffer opb)
      {
        int num1 = 0;
        int num2 = -1;
        InfoFloor1 infoFloor1 = new InfoFloor1();
        infoFloor1.partitions = opb.read(5);
        for (int index = 0; index < infoFloor1.partitions; ++index)
        {
          infoFloor1.partitionclass[index] = opb.read(4);
          if (num2 < infoFloor1.partitionclass[index])
            num2 = infoFloor1.partitionclass[index];
        }
        for (int index1 = 0; index1 < num2 + 1; ++index1)
        {
          infoFloor1.class_dim[index1] = opb.read(3) + 1;
          infoFloor1.class_subs[index1] = opb.read(2);
          if (infoFloor1.class_subs[index1] < 0)
          {
            infoFloor1.free();
            return (object) null;
          }
          if (infoFloor1.class_subs[index1] != 0)
            infoFloor1.class_book[index1] = opb.read(8);
          if (infoFloor1.class_book[index1] < 0 || infoFloor1.class_book[index1] >= vi.books)
          {
            infoFloor1.free();
            return (object) null;
          }
          for (int index2 = 0; index2 < 1 << infoFloor1.class_subs[index1]; ++index2)
          {
            infoFloor1.class_subbook[index1][index2] = opb.read(8) - 1;
            if (infoFloor1.class_subbook[index1][index2] < -1 || infoFloor1.class_subbook[index1][index2] >= vi.books)
            {
              infoFloor1.free();
              return (object) null;
            }
          }
        }
        infoFloor1.mult = opb.read(2) + 1;
        int bits = opb.read(4);
        int index3 = 0;
        int num3 = 0;
        for (; index3 < infoFloor1.partitions; ++index3)
        {
          for (num1 += infoFloor1.class_dim[infoFloor1.partitionclass[index3]]; num3 < num1; ++num3)
          {
            int num4 = infoFloor1.postlist[num3 + 2] = opb.read(bits);
            if (num4 < 0 || num4 >= 1 << bits)
            {
              infoFloor1.free();
              return (object) null;
            }
          }
        }
        infoFloor1.postlist[0] = 0;
        infoFloor1.postlist[1] = 1 << bits;
        return (object) infoFloor1;
      }

      public override object look(DspState vd, InfoMode mi, object i)
      {
        int num1 = 0;
        int[] numArray = new int[Floor1.VIF_POSIT + 2];
        InfoFloor1 infoFloor1 = (InfoFloor1) i;
        LookFloor1 lookFloor1 = new LookFloor1();
        lookFloor1.vi = infoFloor1;
        lookFloor1.n = infoFloor1.postlist[1];
        for (int index = 0; index < infoFloor1.partitions; ++index)
          num1 += infoFloor1.class_dim[infoFloor1.partitionclass[index]];
        int num2 = num1 + 2;
        lookFloor1.posts = num2;
        for (int index = 0; index < num2; ++index)
          numArray[index] = index;
        for (int index1 = 0; index1 < num2 - 1; ++index1)
        {
          for (int index2 = index1; index2 < num2; ++index2)
          {
            if (infoFloor1.postlist[numArray[index1]] > infoFloor1.postlist[numArray[index2]])
            {
              int num3 = numArray[index2];
              numArray[index2] = numArray[index1];
              numArray[index1] = num3;
            }
          }
        }
        for (int index = 0; index < num2; ++index)
          lookFloor1.forward_index[index] = numArray[index];
        for (int index = 0; index < num2; ++index)
          lookFloor1.reverse_index[lookFloor1.forward_index[index]] = index;
        for (int index = 0; index < num2; ++index)
          lookFloor1.sorted_index[index] = infoFloor1.postlist[lookFloor1.forward_index[index]];
        switch (infoFloor1.mult)
        {
          case 1:
            lookFloor1.quant_q = 256 /*0x0100*/;
            break;
          case 2:
            lookFloor1.quant_q = 128 /*0x80*/;
            break;
          case 3:
            lookFloor1.quant_q = 86;
            break;
          case 4:
            lookFloor1.quant_q = 64 /*0x40*/;
            break;
          default:
            lookFloor1.quant_q = -1;
            break;
        }
        for (int index3 = 0; index3 < num2 - 2; ++index3)
        {
          int num4 = 0;
          int num5 = 1;
          int num6 = 0;
          int num7 = lookFloor1.n;
          int num8 = infoFloor1.postlist[index3 + 2];
          for (int index4 = 0; index4 < index3 + 2; ++index4)
          {
            int num9 = infoFloor1.postlist[index4];
            if (num9 > num6 && num9 < num8)
            {
              num4 = index4;
              num6 = num9;
            }
            if (num9 < num7 && num9 > num8)
            {
              num5 = index4;
              num7 = num9;
            }
          }
          lookFloor1.loneighbor[index3] = num4;
          lookFloor1.hineighbor[index3] = num5;
        }
        return (object) lookFloor1;
      }

      public override void free_info(object i)
      {
      }

      public override void free_look(object i)
      {
      }

      public override void free_state(object vs)
      {
      }

      public override int forward(Block vb, object i, float[] fin, float[] fout, object vs) => 0;

      public override object inverse1(Block vb, object ii, object memo)
      {
        LookFloor1 lookFloor1 = (LookFloor1) ii;
        InfoFloor1 vi = lookFloor1.vi;
        CodeBook[] fullbooks = vb.vd.fullbooks;
        if (vb.opb.read(1) != 1)
          return (object) null;
        int[] numArray = (int[]) null;
        if (memo is int[])
          numArray = (int[]) memo;
        if (numArray == null || numArray.Length < lookFloor1.posts)
        {
          numArray = new int[lookFloor1.posts];
        }
        else
        {
          for (int index = 0; index < numArray.Length; ++index)
            numArray[index] = 0;
        }
        numArray[0] = vb.opb.read(Floor1.ilog(lookFloor1.quant_q - 1));
        numArray[1] = vb.opb.read(Floor1.ilog(lookFloor1.quant_q - 1));
        int index1 = 0;
        int num1 = 2;
        for (; index1 < vi.partitions; ++index1)
        {
          int index2 = vi.partitionclass[index1];
          int num2 = vi.class_dim[index2];
          int classSub = vi.class_subs[index2];
          int num3 = 1 << classSub;
          int num4 = 0;
          if (classSub != 0)
          {
            num4 = fullbooks[vi.class_book[index2]].decode(vb.opb);
            if (num4 == -1)
              return (object) null;
          }
          for (int index3 = 0; index3 < num2; ++index3)
          {
            int index4 = vi.class_subbook[index2][num4 & num3 - 1];
            num4 >>>= classSub;
            if (index4 >= 0)
            {
              if ((numArray[num1 + index3] = fullbooks[index4].decode(vb.opb)) == -1)
                return (object) null;
            }
            else
              numArray[num1 + index3] = 0;
          }
          num1 += num2;
        }
        for (int index5 = 2; index5 < lookFloor1.posts; ++index5)
        {
          int num5 = Floor1.render_point(vi.postlist[lookFloor1.loneighbor[index5 - 2]], vi.postlist[lookFloor1.hineighbor[index5 - 2]], numArray[lookFloor1.loneighbor[index5 - 2]], numArray[lookFloor1.hineighbor[index5 - 2]], vi.postlist[index5]);
          int num6 = lookFloor1.quant_q - num5;
          int num7 = num5;
          int num8 = (num6 < num7 ? num6 : num7) << 1;
          int num9 = numArray[index5];
          if (num9 != 0)
          {
            int num10 = num9 < num8 ? ((num9 & 1) == 0 ? num9 >> 1 : (int) -(uint) (num9 + 1 >>> 1)) : (num6 <= num7 ? -1 - (num9 - num6) : num9 - num7);
            numArray[index5] = num10 + num5;
            numArray[lookFloor1.loneighbor[index5 - 2]] &= (int) short.MaxValue;
            numArray[lookFloor1.hineighbor[index5 - 2]] &= (int) short.MaxValue;
          }
          else
            numArray[index5] = num5 | 32768 /*0x8000*/;
        }
        return (object) numArray;
      }

      private static int render_point(int x0, int x1, int y0, int y1, int x)
      {
        y0 &= (int) short.MaxValue;
        y1 &= (int) short.MaxValue;
        int num1 = y1 - y0;
        int num2 = x1 - x0;
        int num3 = Math.Abs(num1) * (x - x0) / num2;
        return num1 < 0 ? y0 - num3 : y0 + num3;
      }

      public override int inverse2(Block vb, object i, object memo, float[] fout)
      {
        LookFloor1 lookFloor1 = (LookFloor1) i;
        InfoFloor1 vi = lookFloor1.vi;
        int num1 = vb.vd.vi.blocksizes[vb.mode] / 2;
        if (memo != null)
        {
          int[] numArray = (int[]) memo;
          int x1 = 0;
          int x0 = 0;
          int y0 = numArray[0] * vi.mult;
          for (int index1 = 1; index1 < lookFloor1.posts; ++index1)
          {
            int index2 = lookFloor1.forward_index[index1];
            int num2 = numArray[index2] & (int) short.MaxValue;
            if (num2 == numArray[index2])
            {
              int y1 = num2 * vi.mult;
              x1 = vi.postlist[index2];
              Floor1.render_line(x0, x1, y0, y1, fout);
              x0 = x1;
              y0 = y1;
            }
          }
          for (int index = x1; index < num1; ++index)
            fout[index] *= fout[index - 1];
          return 1;
        }
        for (int index = 0; index < num1; ++index)
          fout[index] = 0.0f;
        return 0;
      }

      private static void render_line(int x0, int x1, int y0, int y1, float[] d)
      {
        int num1 = y1 - y0;
        int num2 = x1 - x0;
        int num3 = Math.Abs(num1);
        int num4 = num1 / num2;
        int num5 = num1 < 0 ? num4 - 1 : num4 + 1;
        int index1 = x0;
        int index2 = y0;
        int num6 = 0;
        int num7 = num3 - Math.Abs(num4 * num2);
        d[index1] *= Floor1.FLOOR_fromdB_LOOKUP[index2];
        while (++index1 < x1)
        {
          num6 += num7;
          if (num6 >= num2)
          {
            num6 -= num2;
            index2 += num5;
          }
          else
            index2 += num4;
          d[index1] *= Floor1.FLOOR_fromdB_LOOKUP[index2];
        }
      }

      private static int ilog(int v)
      {
        int num = 0;
        for (; v != 0; v >>>= 1)
          ++num;
        return num;
      }

      private static int ilog2(int v)
      {
        int num = 0;
        for (; v > 1; v >>>= 1)
          ++num;
        return num;
      }
    }
}
