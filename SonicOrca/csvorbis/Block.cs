// Decompiled with JetBrains decompiler
// Type: csvorbis.Block
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;

namespace csvorbis
{

    public class Block
    {
      internal float[][] pcm = new float[0][];
      internal csBuffer opb = new csBuffer();
      internal int lW;
      internal int W;
      internal int nW;
      internal int pcmend;
      internal int mode;
      internal int eofflag;
      internal long granulepos;
      internal long sequence;
      internal DspState vd;
      internal int glue_bits;
      internal int time_bits;
      internal int floor_bits;
      internal int res_bits;

      public Block(DspState vd)
      {
        this.vd = vd;
        if (vd.analysisp == 0)
          return;
        this.opb.writeinit();
      }

      public void init(DspState vd) => this.vd = vd;

      public int clear()
      {
        if (this.vd != null && this.vd.analysisp != 0)
          this.opb.writeclear();
        return 0;
      }

      public int synthesis(Packet op)
      {
        Info vi = this.vd.vi;
        this.opb.readinit(op.packet_base, op.packet, op.bytes);
        if (this.opb.read(1) != 0)
          return -1;
        int num = this.opb.read(this.vd.modebits);
        if (num == -1)
          return -1;
        this.mode = num;
        this.W = vi.mode_param[this.mode].blockflag;
        if (this.W != 0)
        {
          this.lW = this.opb.read(1);
          this.nW = this.opb.read(1);
          if (this.nW == -1)
            return -1;
        }
        else
        {
          this.lW = 0;
          this.nW = 0;
        }
        this.granulepos = op.granulepos;
        this.sequence = op.packetno - 3L;
        this.eofflag = op.e_o_s;
        this.pcmend = vi.blocksizes[this.W];
        if (this.pcm.Length < vi.channels)
          this.pcm = new float[vi.channels][];
        for (int index1 = 0; index1 < vi.channels; ++index1)
        {
          if (this.pcm[index1] == null || this.pcm[index1].Length < this.pcmend)
          {
            this.pcm[index1] = new float[this.pcmend];
          }
          else
          {
            for (int index2 = 0; index2 < this.pcmend; ++index2)
              this.pcm[index1][index2] = 0.0f;
          }
        }
        int index = vi.map_type[vi.mode_param[this.mode].mapping];
        return FuncMapping.mapping_P[index].inverse(this, this.vd.mode[this.mode]);
      }
    }
}
