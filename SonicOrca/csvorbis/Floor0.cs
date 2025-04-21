// Decompiled with JetBrains decompiler
// Type: csvorbis.Floor0
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;
using System;

namespace csvorbis
{

    internal class Floor0 : FuncFloor
    {
      private float[] lsp;

      public override void pack(object i, csBuffer opb)
      {
        InfoFloor0 infoFloor0 = (InfoFloor0) i;
        opb.write(infoFloor0.order, 8);
        opb.write(infoFloor0.rate, 16 /*0x10*/);
        opb.write(infoFloor0.barkmap, 16 /*0x10*/);
        opb.write(infoFloor0.ampbits, 6);
        opb.write(infoFloor0.ampdB, 8);
        opb.write(infoFloor0.numbooks - 1, 4);
        for (int index = 0; index < infoFloor0.numbooks; ++index)
          opb.write(infoFloor0.books[index], 8);
      }

      public override object unpack(Info vi, csBuffer opb)
      {
        InfoFloor0 infoFloor0 = new InfoFloor0();
        infoFloor0.order = opb.read(8);
        infoFloor0.rate = opb.read(16 /*0x10*/);
        infoFloor0.barkmap = opb.read(16 /*0x10*/);
        infoFloor0.ampbits = opb.read(6);
        infoFloor0.ampdB = opb.read(8);
        infoFloor0.numbooks = opb.read(4) + 1;
        if (infoFloor0.order < 1 || infoFloor0.rate < 1 || infoFloor0.barkmap < 1 || infoFloor0.numbooks < 1)
          return (object) null;
        for (int index = 0; index < infoFloor0.numbooks; ++index)
        {
          infoFloor0.books[index] = opb.read(8);
          if (infoFloor0.books[index] < 0 || infoFloor0.books[index] >= vi.books)
            return (object) null;
        }
        return (object) infoFloor0;
      }

      public override object look(DspState vd, InfoMode mi, object i)
      {
        Info vi = vd.vi;
        InfoFloor0 infoFloor0 = (InfoFloor0) i;
        LookFloor0 lookFloor0 = new LookFloor0();
        lookFloor0.m = infoFloor0.order;
        lookFloor0.n = vi.blocksizes[mi.blockflag] / 2;
        lookFloor0.ln = infoFloor0.barkmap;
        lookFloor0.vi = infoFloor0;
        lookFloor0.lpclook.init(lookFloor0.ln, lookFloor0.m);
        float num1 = (float) lookFloor0.ln / (float) Floor0.toBARK((float) infoFloor0.rate / 2f);
        lookFloor0.linearmap = new int[lookFloor0.n];
        for (int index = 0; index < lookFloor0.n; ++index)
        {
          int num2 = (int) Math.Floor(Floor0.toBARK((float) infoFloor0.rate / 2f / (float) lookFloor0.n * (float) index) * (double) num1);
          if (num2 >= lookFloor0.ln)
            num2 = lookFloor0.ln;
          lookFloor0.linearmap[index] = num2;
        }
        return (object) lookFloor0;
      }

      private static double toBARK(float f)
      {
        double num1 = 13.1 * Math.Atan(0.00074 * (double) f);
        double num2 = 2.24 * Math.Atan((double) f * (double) f * 1.85E-08);
        double num3 = 0.0001 * (double) f;
        double num4 = num2;
        return num1 + num4 + num3;
      }

      private object state(object i)
      {
        EchstateFloor0 echstateFloor0 = new EchstateFloor0();
        InfoFloor0 infoFloor0 = (InfoFloor0) i;
        echstateFloor0.codewords = new int[infoFloor0.order];
        echstateFloor0.curve = new float[infoFloor0.barkmap];
        echstateFloor0.frameno = -1L;
        return (object) echstateFloor0;
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

      private int inverse(Block vb, object i, float[] fout)
      {
        LookFloor0 lookFloor0 = (LookFloor0) i;
        InfoFloor0 vi = lookFloor0.vi;
        int num1 = vb.opb.read(vi.ampbits);
        if (num1 > 0)
        {
          int num2 = (1 << vi.ampbits) - 1;
          float amp = (float) num1 / (float) num2 * (float) vi.ampdB;
          int index1 = vb.opb.read(Floor0.ilog(vi.numbooks));
          if (index1 != -1 && index1 < vi.numbooks)
          {
            lock (this)
            {
              if (this.lsp == null || this.lsp.Length < lookFloor0.m)
              {
                this.lsp = new float[lookFloor0.m];
              }
              else
              {
                for (int index2 = 0; index2 < lookFloor0.m; ++index2)
                  this.lsp[index2] = 0.0f;
              }
              CodeBook fullbook = vb.vd.fullbooks[vi.books[index1]];
              float num3 = 0.0f;
              for (int index3 = 0; index3 < lookFloor0.m; ++index3)
                fout[index3] = 0.0f;
              for (int index4 = 0; index4 < lookFloor0.m; index4 += fullbook.dim)
              {
                if (fullbook.decodevs(this.lsp, index4, vb.opb, 1, -1) == -1)
                {
                  for (int index5 = 0; index5 < lookFloor0.n; ++index5)
                    fout[index5] = 0.0f;
                  return 0;
                }
              }
              int index6 = 0;
              while (index6 < lookFloor0.m)
              {
                int num4 = 0;
                while (num4 < fullbook.dim)
                {
                  this.lsp[index6] += num3;
                  ++num4;
                  ++index6;
                }
                num3 = this.lsp[index6 - 1];
              }
              Lsp.lsp_to_curve(fout, lookFloor0.linearmap, lookFloor0.n, lookFloor0.ln, this.lsp, lookFloor0.m, amp, (float) vi.ampdB);
              return 1;
            }
          }
        }
        return 0;
      }

      public override object inverse1(Block vb, object i, object memo)
      {
        LookFloor0 lookFloor0 = (LookFloor0) i;
        InfoFloor0 vi = lookFloor0.vi;
        float[] a = (float[]) null;
        if (memo is float[])
          a = (float[]) memo;
        int num1 = vb.opb.read(vi.ampbits);
        if (num1 > 0)
        {
          int num2 = (1 << vi.ampbits) - 1;
          float num3 = (float) num1 / (float) num2 * (float) vi.ampdB;
          int index1 = vb.opb.read(Floor0.ilog(vi.numbooks));
          if (index1 != -1 && index1 < vi.numbooks)
          {
            CodeBook fullbook = vb.vd.fullbooks[vi.books[index1]];
            float num4 = 0.0f;
            if (a == null || a.Length < lookFloor0.m + 1)
            {
              a = new float[lookFloor0.m + 1];
            }
            else
            {
              for (int index2 = 0; index2 < a.Length; ++index2)
                a[index2] = 0.0f;
            }
            for (int offset = 0; offset < lookFloor0.m; offset += fullbook.dim)
            {
              if (fullbook.decodev_set(a, offset, vb.opb, fullbook.dim) == -1)
                return (object) null;
            }
            int index3 = 0;
            while (index3 < lookFloor0.m)
            {
              int num5 = 0;
              while (num5 < fullbook.dim)
              {
                a[index3] += num4;
                ++num5;
                ++index3;
              }
              num4 = a[index3 - 1];
            }
            a[lookFloor0.m] = num3;
            return (object) a;
          }
        }
        return (object) null;
      }

      public override int inverse2(Block vb, object i, object memo, float[] fout)
      {
        LookFloor0 lookFloor0 = (LookFloor0) i;
        InfoFloor0 vi = lookFloor0.vi;
        if (memo != null)
        {
          float[] lsp = (float[]) memo;
          float amp = lsp[lookFloor0.m];
          Lsp.lsp_to_curve(fout, lookFloor0.linearmap, lookFloor0.n, lookFloor0.ln, lsp, lookFloor0.m, amp, (float) vi.ampdB);
          return 1;
        }
        for (int index = 0; index < lookFloor0.n; ++index)
          fout[index] = 0.0f;
        return 0;
      }

      private static float fromdB(float x) => (float) Math.Exp((double) x * 0.11512925);

      private static int ilog(int v)
      {
        int num = 0;
        for (; v != 0; v >>>= 1)
          ++num;
        return num;
      }

      private static void lsp_to_lpc(float[] lsp, float[] lpc, int m)
      {
        int length = m / 2;
        float[] numArray1 = new float[length];
        float[] numArray2 = new float[length];
        float[] numArray3 = new float[length + 1];
        float[] numArray4 = new float[length + 1];
        float[] numArray5 = new float[length];
        float[] numArray6 = new float[length];
        for (int index = 0; index < length; ++index)
        {
          numArray1[index] = (float) (-2.0 * Math.Cos((double) lsp[index * 2]));
          numArray2[index] = (float) (-2.0 * Math.Cos((double) lsp[index * 2 + 1]));
        }
        int index1;
        for (index1 = 0; index1 < length; ++index1)
        {
          numArray3[index1] = 0.0f;
          numArray4[index1] = 1f;
          numArray5[index1] = 0.0f;
          numArray6[index1] = 1f;
        }
        numArray4[index1] = 1f;
        numArray3[index1] = 1f;
        for (int index2 = 1; index2 < m + 1; ++index2)
        {
          float num1;
          float num2 = num1 = 0.0f;
          int index3;
          for (index3 = 0; index3 < length; ++index3)
          {
            float num3 = numArray1[index3] * numArray4[index3] + numArray3[index3];
            numArray3[index3] = numArray4[index3];
            numArray4[index3] = num2;
            num2 += num3;
            float num4 = numArray2[index3] * numArray6[index3] + numArray5[index3];
            numArray5[index3] = numArray6[index3];
            numArray6[index3] = num1;
            num1 += num4;
          }
          lpc[index2 - 1] = (float) (((double) num2 + (double) numArray4[index3] + (double) num1 - (double) numArray3[index3]) / 2.0);
          numArray4[index3] = num2;
          numArray3[index3] = num1;
        }
      }

      private static void lpc_to_curve(
        float[] curve,
        float[] lpc,
        float amp,
        LookFloor0 l,
        string name,
        int frameno)
      {
        float[] curve1 = new float[Math.Max(l.ln * 2, l.m * 2 + 2)];
        if ((double) amp == 0.0)
        {
          for (int index = 0; index < l.n; ++index)
            curve[index] = 0.0f;
        }
        else
        {
          l.lpclook.lpc_to_curve(curve1, lpc, amp);
          for (int index = 0; index < l.n; ++index)
            curve[index] = curve1[l.linearmap[index]];
        }
      }
    }
}
