// Decompiled with JetBrains decompiler
// Type: csvorbis.Residue0
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;
using System;
using System.Runtime.CompilerServices;

namespace csvorbis
{

    internal class Residue0 : FuncResidue
    {
      private static int[][][] partword = new int[2][][];

      public override void pack(object vr, csBuffer opb)
      {
        InfoResidue0 infoResidue0 = (InfoResidue0) vr;
        int num = 0;
        opb.write(infoResidue0.begin, 24);
        opb.write(infoResidue0.end, 24);
        opb.write(infoResidue0.grouping - 1, 24);
        opb.write(infoResidue0.partitions - 1, 6);
        opb.write(infoResidue0.groupbook, 8);
        for (int index = 0; index < infoResidue0.partitions; ++index)
        {
          if (Residue0.ilog(infoResidue0.secondstages[index]) > 3)
          {
            opb.write(infoResidue0.secondstages[index], 3);
            opb.write(1, 1);
            opb.write(infoResidue0.secondstages[index] >> 3, 5);
          }
          else
            opb.write(infoResidue0.secondstages[index], 4);
          num += Residue0.icount(infoResidue0.secondstages[index]);
        }
        for (int index = 0; index < num; ++index)
          opb.write(infoResidue0.booklist[index], 8);
      }

      public override object unpack(Info vi, csBuffer opb)
      {
        int num = 0;
        InfoResidue0 i = new InfoResidue0();
        i.begin = opb.read(24);
        i.end = opb.read(24);
        i.grouping = opb.read(24) + 1;
        i.partitions = opb.read(6) + 1;
        i.groupbook = opb.read(8);
        for (int index = 0; index < i.partitions; ++index)
        {
          int v = opb.read(3);
          if (opb.read(1) != 0)
            v |= opb.read(5) << 3;
          i.secondstages[index] = v;
          num += Residue0.icount(v);
        }
        for (int index = 0; index < num; ++index)
          i.booklist[index] = opb.read(8);
        if (i.groupbook >= vi.books)
        {
          this.free_info((object) i);
          return (object) null;
        }
        for (int index = 0; index < num; ++index)
        {
          if (i.booklist[index] >= vi.books)
          {
            this.free_info((object) i);
            return (object) null;
          }
        }
        return (object) i;
      }

      public override object look(DspState vd, InfoMode vm, object vr)
      {
        InfoResidue0 infoResidue0 = (InfoResidue0) vr;
        LookResidue0 lookResidue0 = new LookResidue0();
        int num1 = 0;
        int num2 = 0;
        lookResidue0.info = infoResidue0;
        lookResidue0.map = vm.mapping;
        lookResidue0.parts = infoResidue0.partitions;
        lookResidue0.fullbooks = vd.fullbooks;
        lookResidue0.phrasebook = vd.fullbooks[infoResidue0.groupbook];
        int dim = lookResidue0.phrasebook.dim;
        lookResidue0.partbooks = new int[lookResidue0.parts][];
        for (int index1 = 0; index1 < lookResidue0.parts; ++index1)
        {
          int length = Residue0.ilog(infoResidue0.secondstages[index1]);
          if (length != 0)
          {
            if (length > num2)
              num2 = length;
            lookResidue0.partbooks[index1] = new int[length];
            for (int index2 = 0; index2 < length; ++index2)
            {
              if ((infoResidue0.secondstages[index1] & 1 << index2) != 0)
                lookResidue0.partbooks[index1][index2] = infoResidue0.booklist[num1++];
            }
          }
        }
        lookResidue0.partvals = (int) Math.Round(Math.Pow((double) lookResidue0.parts, (double) dim));
        lookResidue0.stages = num2;
        lookResidue0.decodemap = new int[lookResidue0.partvals][];
        for (int index3 = 0; index3 < lookResidue0.partvals; ++index3)
        {
          int num3 = index3;
          int num4 = lookResidue0.partvals / lookResidue0.parts;
          lookResidue0.decodemap[index3] = new int[dim];
          for (int index4 = 0; index4 < dim; ++index4)
          {
            int num5 = num3 / num4;
            num3 -= num5 * num4;
            num4 /= lookResidue0.parts;
            lookResidue0.decodemap[index3][index4] = num5;
          }
        }
        return (object) lookResidue0;
      }

      public override void free_info(object i)
      {
      }

      public override void free_look(object i)
      {
      }

      public override int forward(Block vb, object vl, float[][] fin, int ch) => 0;

      [MethodImpl(MethodImplOptions.Synchronized)]
      internal static int _01inverse(Block vb, object vl, float[][] fin, int ch, int decodepart)
      {
        LookResidue0 lookResidue0 = (LookResidue0) vl;
        InfoResidue0 info = lookResidue0.info;
        int grouping = info.grouping;
        int dim = lookResidue0.phrasebook.dim;
        int num1 = (info.end - info.begin) / grouping;
        int length = (num1 + dim - 1) / dim;
        if (Residue0.partword.Length < ch)
        {
          Residue0.partword = new int[ch][][];
          for (int index = 0; index < ch; ++index)
            Residue0.partword[index] = new int[length][];
        }
        else
        {
          for (int index = 0; index < ch; ++index)
          {
            if (Residue0.partword[index] == null || Residue0.partword[index].Length < length)
              Residue0.partword[index] = new int[length][];
          }
        }
        for (int index1 = 0; index1 < lookResidue0.stages; ++index1)
        {
          int num2 = 0;
          int index2 = 0;
          while (num2 < num1)
          {
            if (index1 == 0)
            {
              for (int index3 = 0; index3 < ch; ++index3)
              {
                int index4 = lookResidue0.phrasebook.decode(vb.opb);
                if (index4 == -1)
                  return 0;
                Residue0.partword[index3][index2] = lookResidue0.decodemap[index4];
                if (Residue0.partword[index3][index2] == null)
                  return 0;
              }
            }
            for (int index5 = 0; index5 < dim && num2 < num1; ++num2)
            {
              for (int index6 = 0; index6 < ch; ++index6)
              {
                int offset = info.begin + num2 * grouping;
                if ((info.secondstages[Residue0.partword[index6][index2][index5]] & 1 << index1) != 0)
                {
                  CodeBook fullbook = lookResidue0.fullbooks[lookResidue0.partbooks[Residue0.partword[index6][index2][index5]][index1]];
                  if (fullbook != null)
                  {
                    switch (decodepart)
                    {
                      case 0:
                        if (fullbook.decodevs_add(fin[index6], offset, vb.opb, grouping) == -1)
                          return 0;
                        continue;
                      case 1:
                        if (fullbook.decodev_add(fin[index6], offset, vb.opb, grouping) == -1)
                          return 0;
                        continue;
                      default:
                        continue;
                    }
                  }
                }
              }
              ++index5;
            }
            ++index2;
          }
        }
        return 0;
      }

      internal static int _2inverse(Block vb, object vl, float[][] fin, int ch)
      {
        LookResidue0 lookResidue0 = (LookResidue0) vl;
        InfoResidue0 info = lookResidue0.info;
        int grouping = info.grouping;
        int dim = lookResidue0.phrasebook.dim;
        int num1 = (info.end - info.begin) / grouping;
        int[][] numArray = new int[(num1 + dim - 1) / dim][];
        for (int index1 = 0; index1 < lookResidue0.stages; ++index1)
        {
          int num2 = 0;
          int index2 = 0;
          while (num2 < num1)
          {
            if (index1 == 0)
            {
              int index3 = lookResidue0.phrasebook.decode(vb.opb);
              if (index3 == -1)
                return 0;
              numArray[index2] = lookResidue0.decodemap[index3];
              if (numArray[index2] == null)
                return 0;
            }
            for (int index4 = 0; index4 < dim && num2 < num1; ++num2)
            {
              int offset = info.begin + num2 * grouping;
              if ((info.secondstages[numArray[index2][index4]] & 1 << index1) != 0)
              {
                CodeBook fullbook = lookResidue0.fullbooks[lookResidue0.partbooks[numArray[index2][index4]][index1]];
                if (fullbook != null && fullbook.decodevv_add(fin, offset, ch, vb.opb, grouping) == -1)
                  return 0;
              }
              ++index4;
            }
            ++index2;
          }
        }
        return 0;
      }

      public override int inverse(Block vb, object vl, float[][] fin, int[] nonzero, int ch)
      {
        int ch1 = 0;
        for (int index = 0; index < ch; ++index)
        {
          if (nonzero[index] != 0)
            fin[ch1++] = fin[index];
        }
        return ch1 != 0 ? Residue0._01inverse(vb, vl, fin, ch1, 0) : 0;
      }

      internal static int ilog(int v)
      {
        int num = 0;
        for (; v != 0; v >>>= 1)
          ++num;
        return num;
      }

      internal static int icount(int v)
      {
        int num = 0;
        for (; v != 0; v >>>= 1)
          num += v & 1;
        return num;
      }
    }
}
