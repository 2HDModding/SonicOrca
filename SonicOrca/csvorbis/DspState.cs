// Decompiled with JetBrains decompiler
// Type: csvorbis.DspState
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace csvorbis
{

    public class DspState
    {
      private static float M_PI = 3.14159274f;
      private static int VI_TRANSFORMB = 1;
      private static int VI_WINDOWB = 1;
      internal int analysisp;
      internal Info vi;
      internal int modebits;
      private float[][] pcm;
      private int pcm_storage;
      private int pcm_current;
      private int pcm_returned;
      private float[] multipliers;
      private int envelope_storage;
      private int envelope_current;
      private int eofflag;
      private int lW;
      private int W;
      private int nW;
      private int centerW;
      private long granulepos;
      public long sequence;
      private long glue_bits;
      private long time_bits;
      private long floor_bits;
      private long res_bits;
      internal float[][][][][] wnd;
      internal object[][] transform;
      internal CodeBook[] fullbooks;
      internal object[] mode;
      private byte[] header;
      private byte[] header1;
      private byte[] header2;

      public DspState()
      {
        this.transform = new object[2][];
        this.wnd = new float[2][][][][];
        this.wnd[0] = new float[2][][][];
        this.wnd[0][0] = new float[2][][];
        this.wnd[0][1] = new float[2][][];
        this.wnd[0][0][0] = new float[2][];
        this.wnd[0][0][1] = new float[2][];
        this.wnd[0][1][0] = new float[2][];
        this.wnd[0][1][1] = new float[2][];
        this.wnd[1] = new float[2][][][];
        this.wnd[1][0] = new float[2][][];
        this.wnd[1][1] = new float[2][][];
        this.wnd[1][0][0] = new float[2][];
        this.wnd[1][0][1] = new float[2][];
        this.wnd[1][1][0] = new float[2][];
        this.wnd[1][1][1] = new float[2][];
      }

      private static int ilog2(int v)
      {
        int num = 0;
        for (; v > 1; v >>>= 1)
          ++num;
        return num;
      }

      internal static float[] window(int type, int wnd, int left, int right)
      {
        float[] numArray = new float[wnd];
        if (type != 0)
          return (float[]) null;
        int num1 = wnd / 4 - left / 2;
        int num2 = wnd - wnd / 4 - right / 2;
        for (int index = 0; index < left; ++index)
        {
          float num3 = (float) Math.Sin(((double) index + 0.5) / (double) left * (double) DspState.M_PI / 2.0);
          float num4 = (float) Math.Sin((double) (num3 * num3 * (DspState.M_PI / 2f)));
          numArray[index + num1] = num4;
        }
        for (int index = num1 + left; index < num2; ++index)
          numArray[index] = 1f;
        for (int index = 0; index < right; ++index)
        {
          float num5 = (float) Math.Sin(((double) (right - index) - 0.5) / (double) right * (double) DspState.M_PI / 2.0);
          float num6 = (float) Math.Sin((double) (num5 * num5 * (DspState.M_PI / 2f)));
          numArray[index + num2] = num6;
        }
        return numArray;
      }

      private int init(Info vi, bool encp)
      {
        this.vi = vi;
        this.modebits = DspState.ilog2(vi.modes);
        this.transform[0] = new object[DspState.VI_TRANSFORMB];
        this.transform[1] = new object[DspState.VI_TRANSFORMB];
        this.transform[0][0] = (object) new Mdct();
        this.transform[1][0] = (object) new Mdct();
        ((Mdct) this.transform[0][0]).init(vi.blocksizes[0]);
        ((Mdct) this.transform[1][0]).init(vi.blocksizes[1]);
        this.wnd[0][0][0] = new float[DspState.VI_WINDOWB][];
        this.wnd[0][0][1] = this.wnd[0][0][0];
        this.wnd[0][1][0] = this.wnd[0][0][0];
        this.wnd[0][1][1] = this.wnd[0][0][0];
        this.wnd[1][0][0] = new float[DspState.VI_WINDOWB][];
        this.wnd[1][0][1] = new float[DspState.VI_WINDOWB][];
        this.wnd[1][1][0] = new float[DspState.VI_WINDOWB][];
        this.wnd[1][1][1] = new float[DspState.VI_WINDOWB][];
        for (int type = 0; type < DspState.VI_WINDOWB; ++type)
        {
          this.wnd[0][0][0][type] = DspState.window(type, vi.blocksizes[0], vi.blocksizes[0] / 2, vi.blocksizes[0] / 2);
          this.wnd[1][0][0][type] = DspState.window(type, vi.blocksizes[1], vi.blocksizes[0] / 2, vi.blocksizes[0] / 2);
          this.wnd[1][0][1][type] = DspState.window(type, vi.blocksizes[1], vi.blocksizes[0] / 2, vi.blocksizes[1] / 2);
          this.wnd[1][1][0][type] = DspState.window(type, vi.blocksizes[1], vi.blocksizes[1] / 2, vi.blocksizes[0] / 2);
          this.wnd[1][1][1][type] = DspState.window(type, vi.blocksizes[1], vi.blocksizes[1] / 2, vi.blocksizes[1] / 2);
        }
        this.fullbooks = new CodeBook[vi.books];
        for (int index = 0; index < vi.books; ++index)
        {
          this.fullbooks[index] = new CodeBook();
          this.fullbooks[index].init_decode(vi.book_param[index]);
        }
        this.pcm_storage = 8192 /*0x2000*/;
        this.pcm = new float[vi.channels][];
        for (int index = 0; index < vi.channels; ++index)
          this.pcm[index] = new float[this.pcm_storage];
        this.lW = 0;
        this.W = 0;
        this.centerW = vi.blocksizes[1] / 2;
        this.pcm_current = this.centerW;
        this.mode = new object[vi.modes];
        for (int index1 = 0; index1 < vi.modes; ++index1)
        {
          int mapping = vi.mode_param[index1].mapping;
          int index2 = vi.map_type[mapping];
          this.mode[index1] = FuncMapping.mapping_P[index2].look(this, vi.mode_param[index1], vi.map_param[mapping]);
        }
        return 0;
      }

      public int synthesis_init(Info vi)
      {
        this.init(vi, false);
        this.pcm_returned = this.centerW;
        this.centerW -= vi.blocksizes[this.W] / 4 + vi.blocksizes[this.lW] / 4;
        this.granulepos = -1L;
        this.sequence = -1L;
        return 0;
      }

      private DspState(Info vi)
        : this()
      {
        this.init(vi, false);
        this.pcm_returned = this.centerW;
        this.centerW -= vi.blocksizes[this.W] / 4 + vi.blocksizes[this.lW] / 4;
        this.granulepos = -1L;
        this.sequence = -1L;
      }

      public int synthesis_blockin(Block vb)
      {
        if (this.centerW > this.vi.blocksizes[1] / 2 && this.pcm_returned > 8192 /*0x2000*/)
        {
          int num = this.centerW - this.vi.blocksizes[1] / 2;
          int sourceIndex = this.pcm_returned < num ? this.pcm_returned : num;
          this.pcm_current -= sourceIndex;
          this.centerW -= sourceIndex;
          this.pcm_returned -= sourceIndex;
          if (sourceIndex != 0)
          {
            for (int index = 0; index < this.vi.channels; ++index)
              Array.Copy((Array) this.pcm[index], sourceIndex, (Array) this.pcm[index], 0, this.pcm_current);
          }
        }
        this.lW = this.W;
        this.W = vb.W;
        this.nW = -1;
        this.glue_bits += (long) vb.glue_bits;
        this.time_bits += (long) vb.time_bits;
        this.floor_bits += (long) vb.floor_bits;
        this.res_bits += (long) vb.res_bits;
        if (this.sequence + 1L != vb.sequence)
          this.granulepos = -1L;
        this.sequence = vb.sequence;
        int blocksiz = this.vi.blocksizes[this.W];
        int num1 = this.centerW + this.vi.blocksizes[this.lW] / 4 + blocksiz / 4;
        int num2 = num1 - blocksiz / 2;
        int num3 = num2 + blocksiz;
        int num4 = 0;
        int num5 = 0;
        if (num3 > this.pcm_storage)
        {
          this.pcm_storage = num3 + this.vi.blocksizes[1];
          for (int index = 0; index < this.vi.channels; ++index)
          {
            float[] destinationArray = new float[this.pcm_storage];
            Array.Copy((Array) this.pcm[index], 0, (Array) destinationArray, 0, this.pcm[index].Length);
            this.pcm[index] = destinationArray;
          }
        }
        switch (this.W)
        {
          case 0:
            num4 = 0;
            num5 = this.vi.blocksizes[0] / 2;
            break;
          case 1:
            num4 = this.vi.blocksizes[1] / 4 - this.vi.blocksizes[this.lW] / 4;
            num5 = num4 + this.vi.blocksizes[this.lW] / 2;
            break;
        }
        for (int index1 = 0; index1 < this.vi.channels; ++index1)
        {
          int num6 = num2;
          int index2;
          for (index2 = num4; index2 < num5; ++index2)
            this.pcm[index1][num6 + index2] += vb.pcm[index1][index2];
          for (; index2 < blocksiz; ++index2)
            this.pcm[index1][num6 + index2] = vb.pcm[index1][index2];
        }
        if (this.granulepos == -1L)
        {
          this.granulepos = vb.granulepos;
        }
        else
        {
          this.granulepos += (long) (num1 - this.centerW);
          if (vb.granulepos != -1L && this.granulepos != vb.granulepos)
          {
            if (this.granulepos > vb.granulepos && vb.eofflag != 0)
              num1 -= (int) (this.granulepos - vb.granulepos);
            this.granulepos = vb.granulepos;
          }
        }
        this.centerW = num1;
        this.pcm_current = num3;
        if (vb.eofflag != 0)
          this.eofflag = 1;
        return 0;
      }

      public int synthesis_pcmout(float[][][] _pcm, int[] index)
      {
        if (this.pcm_returned >= this.centerW)
          return 0;
        if (_pcm != null)
        {
          for (int index1 = 0; index1 < this.vi.channels; ++index1)
            index[index1] = this.pcm_returned;
          _pcm[0] = this.pcm;
        }
        return this.centerW - this.pcm_returned;
      }

      public int synthesis_read(int bytes)
      {
        if (bytes != 0 && this.pcm_returned + bytes > this.centerW)
          return -1;
        this.pcm_returned += bytes;
        return 0;
      }

      public void clear()
      {
      }
    }
}
