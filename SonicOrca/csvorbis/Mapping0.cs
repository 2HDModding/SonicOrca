// Decompiled with JetBrains decompiler
// Type: csvorbis.Mapping0
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;

namespace csvorbis
{

    internal class Mapping0 : FuncMapping
    {
      private float[][] pcmbundle;
      private int[] zerobundle;
      private int[] nonzero;
      private object[] floormemo;

      public override void free_info(object imap)
      {
      }

      public override void free_look(object imap)
      {
      }

      public override object look(DspState vd, InfoMode vm, object m)
      {
        Info vi = vd.vi;
        LookMapping0 lookMapping0 = new LookMapping0();
        InfoMapping0 infoMapping0 = lookMapping0.map = (InfoMapping0) m;
        lookMapping0.mode = vm;
        lookMapping0.time_look = new object[infoMapping0.submaps];
        lookMapping0.floor_look = new object[infoMapping0.submaps];
        lookMapping0.residue_look = new object[infoMapping0.submaps];
        lookMapping0.time_func = new FuncTime[infoMapping0.submaps];
        lookMapping0.floor_func = new FuncFloor[infoMapping0.submaps];
        lookMapping0.residue_func = new FuncResidue[infoMapping0.submaps];
        for (int index1 = 0; index1 < infoMapping0.submaps; ++index1)
        {
          int index2 = infoMapping0.timesubmap[index1];
          int index3 = infoMapping0.floorsubmap[index1];
          int index4 = infoMapping0.residuesubmap[index1];
          lookMapping0.time_func[index1] = FuncTime.time_P[vi.time_type[index2]];
          lookMapping0.time_look[index1] = lookMapping0.time_func[index1].look(vd, vm, vi.time_param[index2]);
          lookMapping0.floor_func[index1] = FuncFloor.floor_P[vi.floor_type[index3]];
          lookMapping0.floor_look[index1] = lookMapping0.floor_func[index1].look(vd, vm, vi.floor_param[index3]);
          lookMapping0.residue_func[index1] = FuncResidue.residue_P[vi.residue_type[index4]];
          lookMapping0.residue_look[index1] = lookMapping0.residue_func[index1].look(vd, vm, vi.residue_param[index4]);
        }
        if (vi.psys != 0)
        {
          int analysisp = vd.analysisp;
        }
        lookMapping0.ch = vi.channels;
        return (object) lookMapping0;
      }

      public override void pack(Info vi, object imap, csBuffer opb)
      {
        InfoMapping0 infoMapping0 = (InfoMapping0) imap;
        if (infoMapping0.submaps > 1)
        {
          opb.write(1, 1);
          opb.write(infoMapping0.submaps - 1, 4);
        }
        else
          opb.write(0, 1);
        if (infoMapping0.coupling_steps > 0)
        {
          opb.write(1, 1);
          opb.write(infoMapping0.coupling_steps - 1, 8);
          for (int index = 0; index < infoMapping0.coupling_steps; ++index)
          {
            opb.write(infoMapping0.coupling_mag[index], Mapping0.ilog2(vi.channels));
            opb.write(infoMapping0.coupling_ang[index], Mapping0.ilog2(vi.channels));
          }
        }
        else
          opb.write(0, 1);
        opb.write(0, 2);
        if (infoMapping0.submaps > 1)
        {
          for (int index = 0; index < vi.channels; ++index)
            opb.write(infoMapping0.chmuxlist[index], 4);
        }
        for (int index = 0; index < infoMapping0.submaps; ++index)
        {
          opb.write(infoMapping0.timesubmap[index], 8);
          opb.write(infoMapping0.floorsubmap[index], 8);
          opb.write(infoMapping0.residuesubmap[index], 8);
        }
      }

      public override object unpack(Info vi, csBuffer opb)
      {
        InfoMapping0 infoMapping0 = new InfoMapping0();
        infoMapping0.submaps = opb.read(1) == 0 ? 1 : opb.read(4) + 1;
        if (opb.read(1) != 0)
        {
          infoMapping0.coupling_steps = opb.read(8) + 1;
          for (int index = 0; index < infoMapping0.coupling_steps; ++index)
          {
            int num1 = infoMapping0.coupling_mag[index] = opb.read(Mapping0.ilog2(vi.channels));
            int num2 = infoMapping0.coupling_ang[index] = opb.read(Mapping0.ilog2(vi.channels));
            if (num1 < 0 || num2 < 0 || num1 == num2 || num1 >= vi.channels || num2 >= vi.channels)
            {
              infoMapping0.free();
              return (object) null;
            }
          }
        }
        if (opb.read(2) > 0)
        {
          infoMapping0.free();
          return (object) null;
        }
        if (infoMapping0.submaps > 1)
        {
          for (int index = 0; index < vi.channels; ++index)
          {
            infoMapping0.chmuxlist[index] = opb.read(4);
            if (infoMapping0.chmuxlist[index] >= infoMapping0.submaps)
            {
              infoMapping0.free();
              return (object) null;
            }
          }
        }
        for (int index = 0; index < infoMapping0.submaps; ++index)
        {
          infoMapping0.timesubmap[index] = opb.read(8);
          if (infoMapping0.timesubmap[index] >= vi.times)
          {
            infoMapping0.free();
            return (object) null;
          }
          infoMapping0.floorsubmap[index] = opb.read(8);
          if (infoMapping0.floorsubmap[index] >= vi.floors)
          {
            infoMapping0.free();
            return (object) null;
          }
          infoMapping0.residuesubmap[index] = opb.read(8);
          if (infoMapping0.residuesubmap[index] >= vi.residues)
          {
            infoMapping0.free();
            return (object) null;
          }
        }
        return (object) infoMapping0;
      }

      public override int inverse(Block vb, object l)
      {
        lock (this)
        {
          DspState vd = vb.vd;
          Info vi = vd.vi;
          LookMapping0 lookMapping0 = (LookMapping0) l;
          InfoMapping0 map = lookMapping0.map;
          InfoMode mode = lookMapping0.mode;
          int num1 = vb.pcmend = vi.blocksizes[vb.W];
          float[] numArray1 = vd.wnd[vb.W][vb.lW][vb.nW][mode.windowtype];
          if (this.pcmbundle == null || this.pcmbundle.Length < vi.channels)
          {
            this.pcmbundle = new float[vi.channels][];
            this.nonzero = new int[vi.channels];
            this.zerobundle = new int[vi.channels];
            this.floormemo = new object[vi.channels];
          }
          for (int index1 = 0; index1 < vi.channels; ++index1)
          {
            float[] numArray2 = vb.pcm[index1];
            int index2 = map.chmuxlist[index1];
            this.floormemo[index1] = lookMapping0.floor_func[index2].inverse1(vb, lookMapping0.floor_look[index2], this.floormemo[index1]);
            this.nonzero[index1] = this.floormemo[index1] == null ? 0 : 1;
            for (int index3 = 0; index3 < num1 / 2; ++index3)
              numArray2[index3] = 0.0f;
          }
          for (int index = 0; index < map.coupling_steps; ++index)
          {
            if (this.nonzero[map.coupling_mag[index]] != 0 || this.nonzero[map.coupling_ang[index]] != 0)
            {
              this.nonzero[map.coupling_mag[index]] = 1;
              this.nonzero[map.coupling_ang[index]] = 1;
            }
          }
          for (int index4 = 0; index4 < map.submaps; ++index4)
          {
            int ch = 0;
            for (int index5 = 0; index5 < vi.channels; ++index5)
            {
              if (map.chmuxlist[index5] == index4)
              {
                this.zerobundle[ch] = this.nonzero[index5] == 0 ? 0 : 1;
                this.pcmbundle[ch++] = vb.pcm[index5];
              }
            }
            lookMapping0.residue_func[index4].inverse(vb, lookMapping0.residue_look[index4], this.pcmbundle, this.zerobundle, ch);
          }
          for (int index6 = map.coupling_steps - 1; index6 >= 0; --index6)
          {
            float[] numArray3 = vb.pcm[map.coupling_mag[index6]];
            float[] numArray4 = vb.pcm[map.coupling_ang[index6]];
            for (int index7 = 0; index7 < num1 / 2; ++index7)
            {
              float num2 = numArray3[index7];
              float num3 = numArray4[index7];
              if ((double) num2 > 0.0)
              {
                if ((double) num3 > 0.0)
                {
                  numArray3[index7] = num2;
                  numArray4[index7] = num2 - num3;
                }
                else
                {
                  numArray4[index7] = num2;
                  numArray3[index7] = num2 + num3;
                }
              }
              else if ((double) num3 > 0.0)
              {
                numArray3[index7] = num2;
                numArray4[index7] = num2 + num3;
              }
              else
              {
                numArray4[index7] = num2;
                numArray3[index7] = num2 - num3;
              }
            }
          }
          for (int index8 = 0; index8 < vi.channels; ++index8)
          {
            float[] fout = vb.pcm[index8];
            int index9 = map.chmuxlist[index8];
            lookMapping0.floor_func[index9].inverse2(vb, lookMapping0.floor_look[index9], this.floormemo[index8], fout);
          }
          for (int index = 0; index < vi.channels; ++index)
          {
            float[] numArray5 = vb.pcm[index];
            ((Mdct) vd.transform[vb.W][0]).backward(numArray5, numArray5);
          }
          for (int index10 = 0; index10 < vi.channels; ++index10)
          {
            float[] numArray6 = vb.pcm[index10];
            if (this.nonzero[index10] != 0)
            {
              for (int index11 = 0; index11 < num1; ++index11)
                numArray6[index11] *= numArray1[index11];
            }
            else
            {
              for (int index12 = 0; index12 < num1; ++index12)
                numArray6[index12] = 0.0f;
            }
          }
          return 0;
        }
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
