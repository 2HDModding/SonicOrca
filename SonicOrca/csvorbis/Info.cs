// Decompiled with JetBrains decompiler
// Type: csvorbis.Info
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;
using System.Text;

namespace csvorbis
{

    public class Info
    {
      private static int OV_EBADPACKET = -136;
      private static int OV_ENOTAUDIO = -135;
      private static string _vorbis = "vorbis";
      private static int VI_TIMEB = 1;
      private static int VI_FLOORB = 2;
      private static int VI_RESB = 3;
      private static int VI_MAPB = 1;
      private static int VI_WINDOWB = 1;
      public int version;
      public int channels;
      public int rate;
      internal int bitrate_upper;
      internal int bitrate_nominal;
      internal int bitrate_lower;
      internal int[] blocksizes = new int[2];
      internal int modes;
      internal int maps;
      internal int times;
      internal int floors;
      internal int residues;
      internal int books;
      internal int psys;
      internal InfoMode[] mode_param;
      internal int[] map_type;
      internal object[] map_param;
      internal int[] time_type;
      internal object[] time_param;
      internal int[] floor_type;
      internal object[] floor_param;
      internal int[] residue_type;
      internal object[] residue_param;
      internal StaticCodeBook[] book_param;
      internal PsyInfo[] psy_param = new PsyInfo[64 /*0x40*/];

      public void init() => this.rate = 0;

      public void clear()
      {
        for (int index = 0; index < this.modes; ++index)
          this.mode_param[index] = (InfoMode) null;
        this.mode_param = (InfoMode[]) null;
        for (int index = 0; index < this.maps; ++index)
          FuncMapping.mapping_P[this.map_type[index]].free_info(this.map_param[index]);
        this.map_param = (object[]) null;
        for (int index = 0; index < this.times; ++index)
          FuncTime.time_P[this.time_type[index]].free_info(this.time_param[index]);
        this.time_param = (object[]) null;
        for (int index = 0; index < this.floors; ++index)
          FuncFloor.floor_P[this.floor_type[index]].free_info(this.floor_param[index]);
        this.floor_param = (object[]) null;
        for (int index = 0; index < this.residues; ++index)
          FuncResidue.residue_P[this.residue_type[index]].free_info(this.residue_param[index]);
        this.residue_param = (object[]) null;
        for (int index = 0; index < this.books; ++index)
        {
          if (this.book_param[index] != null)
          {
            this.book_param[index].clear();
            this.book_param[index] = (StaticCodeBook) null;
          }
        }
        this.book_param = (StaticCodeBook[]) null;
        for (int index = 0; index < this.psys; ++index)
          this.psy_param[index].free();
      }

      private int unpack_info(csBuffer opb)
      {
        this.version = opb.read(32 /*0x20*/);
        if (this.version != 0)
          return -1;
        this.channels = opb.read(8);
        this.rate = opb.read(32 /*0x20*/);
        this.bitrate_upper = opb.read(32 /*0x20*/);
        this.bitrate_nominal = opb.read(32 /*0x20*/);
        this.bitrate_lower = opb.read(32 /*0x20*/);
        this.blocksizes[0] = 1 << opb.read(4);
        this.blocksizes[1] = 1 << opb.read(4);
        if (this.rate >= 1 && this.channels >= 1 && this.blocksizes[0] >= 8 && this.blocksizes[1] >= this.blocksizes[0] && opb.read(1) == 1)
          return 0;
        this.clear();
        return -1;
      }

      private int unpack_books(csBuffer opb)
      {
        this.books = opb.read(8) + 1;
        if (this.book_param == null || this.book_param.Length != this.books)
          this.book_param = new StaticCodeBook[this.books];
        for (int index = 0; index < this.books; ++index)
        {
          this.book_param[index] = new StaticCodeBook();
          if (this.book_param[index].unpack(opb) != 0)
          {
            this.clear();
            return -1;
          }
        }
        this.times = opb.read(6) + 1;
        if (this.time_type == null || this.time_type.Length != this.times)
          this.time_type = new int[this.times];
        if (this.time_param == null || this.time_param.Length != this.times)
          this.time_param = new object[this.times];
        for (int index = 0; index < this.times; ++index)
        {
          this.time_type[index] = opb.read(16 /*0x10*/);
          if (this.time_type[index] < 0 || this.time_type[index] >= Info.VI_TIMEB)
          {
            this.clear();
            return -1;
          }
          this.time_param[index] = FuncTime.time_P[this.time_type[index]].unpack(this, opb);
          if (this.time_param[index] == null)
          {
            this.clear();
            return -1;
          }
        }
        this.floors = opb.read(6) + 1;
        if (this.floor_type == null || this.floor_type.Length != this.floors)
          this.floor_type = new int[this.floors];
        if (this.floor_param == null || this.floor_param.Length != this.floors)
          this.floor_param = new object[this.floors];
        for (int index = 0; index < this.floors; ++index)
        {
          this.floor_type[index] = opb.read(16 /*0x10*/);
          if (this.floor_type[index] < 0 || this.floor_type[index] >= Info.VI_FLOORB)
          {
            this.clear();
            return -1;
          }
          this.floor_param[index] = FuncFloor.floor_P[this.floor_type[index]].unpack(this, opb);
          if (this.floor_param[index] == null)
          {
            this.clear();
            return -1;
          }
        }
        this.residues = opb.read(6) + 1;
        if (this.residue_type == null || this.residue_type.Length != this.residues)
          this.residue_type = new int[this.residues];
        if (this.residue_param == null || this.residue_param.Length != this.residues)
          this.residue_param = new object[this.residues];
        for (int index = 0; index < this.residues; ++index)
        {
          this.residue_type[index] = opb.read(16 /*0x10*/);
          if (this.residue_type[index] < 0 || this.residue_type[index] >= Info.VI_RESB)
          {
            this.clear();
            return -1;
          }
          this.residue_param[index] = FuncResidue.residue_P[this.residue_type[index]].unpack(this, opb);
          if (this.residue_param[index] == null)
          {
            this.clear();
            return -1;
          }
        }
        this.maps = opb.read(6) + 1;
        if (this.map_type == null || this.map_type.Length != this.maps)
          this.map_type = new int[this.maps];
        if (this.map_param == null || this.map_param.Length != this.maps)
          this.map_param = new object[this.maps];
        for (int index = 0; index < this.maps; ++index)
        {
          this.map_type[index] = opb.read(16 /*0x10*/);
          if (this.map_type[index] < 0 || this.map_type[index] >= Info.VI_MAPB)
          {
            this.clear();
            return -1;
          }
          this.map_param[index] = FuncMapping.mapping_P[this.map_type[index]].unpack(this, opb);
          if (this.map_param[index] == null)
          {
            this.clear();
            return -1;
          }
        }
        this.modes = opb.read(6) + 1;
        if (this.mode_param == null || this.mode_param.Length != this.modes)
          this.mode_param = new InfoMode[this.modes];
        for (int index = 0; index < this.modes; ++index)
        {
          this.mode_param[index] = new InfoMode();
          this.mode_param[index].blockflag = opb.read(1);
          this.mode_param[index].windowtype = opb.read(16 /*0x10*/);
          this.mode_param[index].transformtype = opb.read(16 /*0x10*/);
          this.mode_param[index].mapping = opb.read(8);
          if (this.mode_param[index].windowtype >= Info.VI_WINDOWB || this.mode_param[index].transformtype >= Info.VI_WINDOWB || this.mode_param[index].mapping >= this.maps)
          {
            this.clear();
            return -1;
          }
        }
        if (opb.read(1) == 1)
          return 0;
        this.clear();
        return -1;
      }

      public int synthesis_headerin(Comment vc, Packet op)
      {
        csBuffer opb = new csBuffer();
        if (op != null)
        {
          opb.readinit(op.packet_base, op.packet, op.bytes);
          byte[] s = new byte[6];
          int num = opb.read(8);
          opb.read(s, 6);
          if (s[0] != (byte) 118 || s[1] != (byte) 111 || s[2] != (byte) 114 || s[3] != (byte) 98 || s[4] != (byte) 105 || s[5] != (byte) 115)
            return -1;
          switch (num)
          {
            case 1:
              return op.b_o_s == 0 || this.rate != 0 ? -1 : this.unpack_info(opb);
            case 3:
              return this.rate == 0 ? -1 : vc.unpack(opb);
            case 5:
              return this.rate == 0 || vc.vendor == null ? -1 : this.unpack_books(opb);
          }
        }
        return -1;
      }

      private int pack_info(csBuffer opb)
      {
        byte[] bytes = Encoding.UTF8.GetBytes(Info._vorbis);
        opb.write(1, 8);
        opb.write(bytes);
        opb.write(0, 32 /*0x20*/);
        opb.write(this.channels, 8);
        opb.write(this.rate, 32 /*0x20*/);
        opb.write(this.bitrate_upper, 32 /*0x20*/);
        opb.write(this.bitrate_nominal, 32 /*0x20*/);
        opb.write(this.bitrate_lower, 32 /*0x20*/);
        opb.write(Info.ilog2(this.blocksizes[0]), 4);
        opb.write(Info.ilog2(this.blocksizes[1]), 4);
        opb.write(1, 1);
        return 0;
      }

      private int pack_books(csBuffer opb)
      {
        byte[] bytes = Encoding.UTF8.GetBytes(Info._vorbis);
        opb.write(5, 8);
        opb.write(bytes);
        opb.write(this.books - 1, 8);
        for (int index = 0; index < this.books; ++index)
        {
          if (this.book_param[index].pack(opb) != 0)
            return -1;
        }
        opb.write(this.times - 1, 6);
        for (int index = 0; index < this.times; ++index)
        {
          opb.write(this.time_type[index], 16 /*0x10*/);
          FuncTime.time_P[this.time_type[index]].pack(this.time_param[index], opb);
        }
        opb.write(this.floors - 1, 6);
        for (int index = 0; index < this.floors; ++index)
        {
          opb.write(this.floor_type[index], 16 /*0x10*/);
          FuncFloor.floor_P[this.floor_type[index]].pack(this.floor_param[index], opb);
        }
        opb.write(this.residues - 1, 6);
        for (int index = 0; index < this.residues; ++index)
        {
          opb.write(this.residue_type[index], 16 /*0x10*/);
          FuncResidue.residue_P[this.residue_type[index]].pack(this.residue_param[index], opb);
        }
        opb.write(this.maps - 1, 6);
        for (int index = 0; index < this.maps; ++index)
        {
          opb.write(this.map_type[index], 16 /*0x10*/);
          FuncMapping.mapping_P[this.map_type[index]].pack(this, this.map_param[index], opb);
        }
        opb.write(this.modes - 1, 6);
        for (int index = 0; index < this.modes; ++index)
        {
          opb.write(this.mode_param[index].blockflag, 1);
          opb.write(this.mode_param[index].windowtype, 16 /*0x10*/);
          opb.write(this.mode_param[index].transformtype, 16 /*0x10*/);
          opb.write(this.mode_param[index].mapping, 8);
        }
        opb.write(1, 1);
        return 0;
      }

      public int blocksize(Packet op)
      {
        csBuffer csBuffer = new csBuffer();
        csBuffer.readinit(op.packet_base, op.packet, op.bytes);
        if (csBuffer.read(1) != 0)
          return Info.OV_ENOTAUDIO;
        int bits = 0;
        for (int modes = this.modes; modes > 1; modes >>>= 1)
          ++bits;
        int index = csBuffer.read(bits);
        return index == -1 ? Info.OV_EBADPACKET : this.blocksizes[this.mode_param[index].blockflag];
      }

      private static int ilog2(int v)
      {
        int num = 0;
        for (; v > 1; v >>>= 1)
          ++num;
        return num;
      }

      public string toString()
      {
        return $"version:{this.version.ToString()}, channels:{this.channels.ToString()}, rate:{this.rate.ToString()}, bitrate:{this.bitrate_upper.ToString()},{this.bitrate_nominal.ToString()},{this.bitrate_lower.ToString()}";
      }
    }
}
