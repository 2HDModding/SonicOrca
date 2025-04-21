// Decompiled with JetBrains decompiler
// Type: csvorbis.VorbisFile
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;
using System;
using System.IO;

namespace csvorbis
{

    public class VorbisFile
    {
      private static int CHUNKSIZE = 8500;
      private static int SEEK_SET = 0;
      private static int SEEK_CUR = 1;
      private static int SEEK_END = 2;
      private static int OV_FALSE = -1;
      private static int OV_EOF = -2;
      private static int OV_HOLE = -3;
      private static int OV_EREAD = (int) sbyte.MinValue;
      private static int OV_EFAULT = -129;
      private static int OV_EIMPL = -130;
      private static int OV_EINVAL = -131;
      private static int OV_ENOTVORBIS = -132;
      private static int OV_EBADHEADER = -133;
      private static int OV_EVERSION = -134;
      private static int OV_ENOTAUDIO = -135;
      private static int OV_EBADPACKET = -136;
      private static int OV_EBADLINK = -137;
      private static int OV_ENOSEEK = -138;
      private FileStream datasource;
      private bool skable;
      private long offset;
      private long end;
      private SyncState oy = new SyncState();
      private int links;
      private long[] offsets;
      private long[] dataoffsets;
      private int[] serialnos;
      private long[] pcmlengths;
      private Info[] vi;
      private Comment[] vc;
      private long pcm_offset;
      private bool decode_ready;
      private int current_serialno;
      private int current_link;
      private float bittrack;
      private float samptrack;
      private StreamState os;
      private DspState vd;
      private Block vb;

      private VorbisFile()
      {
        this.os = new StreamState();
        this.vd = new DspState();
        this.vb = new Block(this.vd);
      }

      public VorbisFile(string file)
        : this()
      {
        FileStream iis;
        try
        {
          iis = new FileStream(file, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
          throw new csorbisException("VorbisFile: " + ex.Message);
        }
        if (this.open(iis, (byte[]) null, 0) == -1)
          throw new csorbisException("VorbisFile: open return -1");
      }

      public VorbisFile(FileStream inst, byte[] initial, int ibytes)
        : this()
      {
        this.open(inst, initial, ibytes);
      }

      private int get_data()
      {
        int offset = this.oy.buffer(VorbisFile.CHUNKSIZE);
        byte[] data = this.oy.data;
        int bytes;
        try
        {
          bytes = this.datasource.Read(data, offset, VorbisFile.CHUNKSIZE);
        }
        catch (Exception ex)
        {
          Console.Error.WriteLine(ex.Message);
          return VorbisFile.OV_EREAD;
        }
        this.oy.wrote(bytes);
        if (bytes == -1)
          bytes = 0;
        return bytes;
      }

      private void seek_helper(long offst)
      {
        VorbisFile.fseek(this.datasource, offst, VorbisFile.SEEK_SET);
        this.offset = offst;
        this.oy.reset();
      }

      private int get_next_page(Page page, long boundary)
      {
        if (boundary > 0L)
          boundary += this.offset;
        while (boundary <= 0L || this.offset < boundary)
        {
          int num = this.oy.pageseek(page);
          if (num < 0)
            this.offset -= (long) num;
          else if (num == 0)
          {
            if (boundary == 0L)
              return VorbisFile.OV_FALSE;
            int data = this.get_data();
            if (data == 0)
              return VorbisFile.OV_EOF;
            if (data < 0)
              return VorbisFile.OV_EREAD;
          }
          else
          {
            int offset = (int) this.offset;
            this.offset += (long) num;
            return offset;
          }
        }
        return VorbisFile.OV_FALSE;
      }

      private int get_prev_page(Page page)
      {
        long offst1 = this.offset;
        int offst2 = -1;
        while (offst2 == -1)
        {
          offst1 -= (long) VorbisFile.CHUNKSIZE;
          if (offst1 < 0L)
            offst1 = 0L;
          this.seek_helper(offst1);
          while (this.offset < offst1 + (long) VorbisFile.CHUNKSIZE)
          {
            int nextPage = this.get_next_page(page, offst1 + (long) VorbisFile.CHUNKSIZE - this.offset);
            if (nextPage == VorbisFile.OV_EREAD)
              return VorbisFile.OV_EREAD;
            if (nextPage >= 0)
              offst2 = nextPage;
            else
              break;
          }
        }
        this.seek_helper((long) offst2);
        return this.get_next_page(page, (long) VorbisFile.CHUNKSIZE) < 0 ? VorbisFile.OV_EFAULT : offst2;
      }

      private int bisect_forward_serialno(long begin, long searched, long end, int currentno, int m)
      {
        long num1 = end;
        long num2 = end;
        Page page = new Page();
        while (searched < num1)
        {
          long offst = num1 - searched >= (long) VorbisFile.CHUNKSIZE ? (searched + num1) / 2L : searched;
          this.seek_helper(offst);
          int nextPage = this.get_next_page(page, -1L);
          if (nextPage == VorbisFile.OV_EREAD)
            return VorbisFile.OV_EREAD;
          if (nextPage < 0 || page.serialno() != currentno)
          {
            num1 = offst;
            if (nextPage >= 0)
              num2 = (long) nextPage;
          }
          else
            searched = (long) (nextPage + page.header_len + page.body_len);
        }
        this.seek_helper(num2);
        int nextPage1 = this.get_next_page(page, -1L);
        if (nextPage1 == VorbisFile.OV_EREAD)
          return VorbisFile.OV_EREAD;
        if (searched >= end || nextPage1 == -1)
        {
          this.links = m + 1;
          this.offsets = new long[m + 2];
          this.offsets[m + 1] = searched;
        }
        else if (this.bisect_forward_serialno(num2, this.offset, end, page.serialno(), m + 1) == VorbisFile.OV_EREAD)
          return VorbisFile.OV_EREAD;
        this.offsets[m] = begin;
        return 0;
      }

      private int fetch_headers(Info vi, Comment vc, int[] serialno, Page og_ptr)
      {
        Page page = new Page();
        Packet op = new Packet();
        if (og_ptr == null)
        {
          int nextPage = this.get_next_page(page, (long) VorbisFile.CHUNKSIZE);
          if (nextPage == VorbisFile.OV_EREAD)
            return VorbisFile.OV_EREAD;
          if (nextPage < 0)
            return VorbisFile.OV_ENOTVORBIS;
          og_ptr = page;
        }
        if (serialno != null)
          serialno[0] = og_ptr.serialno();
        this.os.init(og_ptr.serialno());
        vi.init();
        vc.init();
        int num = 0;
        while (num < 3)
        {
          this.os.pagein(og_ptr);
          for (; num < 3; ++num)
          {
            switch (this.os.packetout(op))
            {
              case -1:
                Console.Error.WriteLine("Corrupt header in logical bitstream.");
                vi.clear();
                vc.clear();
                this.os.clear();
                return -1;
              case 0:
                goto label_16;
              default:
                if (vi.synthesis_headerin(vc, op) != 0)
                {
                  Console.Error.WriteLine("Illegal header in logical bitstream.");
                  vi.clear();
                  vc.clear();
                  this.os.clear();
                  return -1;
                }
                continue;
            }
          }
    label_16:
          if (num < 3 && this.get_next_page(og_ptr, 1L) < 0)
          {
            Console.Error.WriteLine("Missing header in logical bitstream.");
            vi.clear();
            vc.clear();
            this.os.clear();
            return -1;
          }
        }
        return 0;
      }

      private void prefetch_all_headers(Info first_i, Comment first_c, int dataoffset)
      {
        Page page = new Page();
        this.vi = new Info[this.links];
        this.vc = new Comment[this.links];
        this.dataoffsets = new long[this.links];
        this.pcmlengths = new long[this.links];
        this.serialnos = new int[this.links];
    label_12:
        for (int index = 0; index < this.links; ++index)
        {
          if (first_i != null && first_c != null && index == 0)
          {
            this.vi[index] = first_i;
            this.vc[index] = first_c;
            this.dataoffsets[index] = (long) dataoffset;
          }
          else
          {
            this.seek_helper(this.offsets[index]);
            if (this.fetch_headers(this.vi[index], this.vc[index], (int[]) null, (Page) null) == -1)
            {
              Console.Error.WriteLine($"Error opening logical bitstream #{(object) (index + 1)}\n");
              this.dataoffsets[index] = -1L;
            }
            else
            {
              this.dataoffsets[index] = this.offset;
              this.os.clear();
            }
          }
          this.seek_helper(this.offsets[index + 1]);
          while (this.get_prev_page(page) != -1)
          {
            if (page.granulepos() != -1L)
            {
              this.serialnos[index] = page.serialno();
              this.pcmlengths[index] = page.granulepos();
              goto label_12;
            }
          }
          Console.Error.WriteLine($"Could not find last page of logical bitstream #{(object) index}\n");
          this.vi[index].clear();
          this.vc[index].clear();
        }
      }

      private int make_decode_ready()
      {
        if (this.decode_ready)
          Environment.Exit(1);
        this.vd.synthesis_init(this.vi[0]);
        this.vb.init(this.vd);
        this.decode_ready = true;
        return 0;
      }

      private int open_seekable()
      {
        Info info = new Info();
        Comment comment = new Comment();
        Page page = new Page();
        int[] serialno = new int[1];
        int num = this.fetch_headers(info, comment, serialno, (Page) null);
        int currentno = serialno[0];
        int offset1 = (int) this.offset;
        this.os.clear();
        if (num == -1)
          return -1;
        this.skable = true;
        VorbisFile.fseek(this.datasource, 0L, VorbisFile.SEEK_END);
        this.offset = VorbisFile.ftell(this.datasource);
        long offset2 = this.offset;
        long prevPage = (long) this.get_prev_page(page);
        if (page.serialno() != currentno)
        {
          if (this.bisect_forward_serialno(0L, 0L, prevPage + 1L, currentno, 0) < 0)
          {
            this.clear();
            return VorbisFile.OV_EREAD;
          }
        }
        else if (this.bisect_forward_serialno(0L, prevPage, prevPage + 1L, currentno, 0) < 0)
        {
          this.clear();
          return VorbisFile.OV_EREAD;
        }
        this.prefetch_all_headers(info, comment, offset1);
        return this.raw_seek(0);
      }

      private int open_nonseekable()
      {
        this.links = 1;
        this.vi = new Info[this.links];
        this.vi[0] = new Info();
        this.vc = new Comment[this.links];
        this.vc[0] = new Comment();
        int[] serialno = new int[1];
        if (this.fetch_headers(this.vi[0], this.vc[0], serialno, (Page) null) == -1)
          return -1;
        this.current_serialno = serialno[0];
        this.make_decode_ready();
        return 0;
      }

      private void decode_clear()
      {
        this.os.clear();
        this.vd.clear();
        this.vb.clear();
        this.decode_ready = false;
        this.bittrack = 0.0f;
        this.samptrack = 0.0f;
      }

      private int process_packet(int readp)
      {
        Page page = new Page();
        Packet op;
        long granulepos;
        int num1;
        while (true)
        {
          if (this.decode_ready)
          {
            op = new Packet();
            if (this.os.packetout(op) > 0)
            {
              granulepos = op.granulepos;
              if (this.vb.synthesis(op) == 0)
                break;
            }
          }
          if (readp != 0 && this.get_next_page(page, -1L) >= 0)
          {
            this.bittrack += (float) (page.header_len * 8);
            if (this.decode_ready && this.current_serialno != page.serialno())
              this.decode_clear();
            if (!this.decode_ready)
            {
              if (this.skable)
              {
                this.current_serialno = page.serialno();
                int index = 0;
                while (index < this.links && this.serialnos[index] != this.current_serialno)
                  ++index;
                if (index != this.links)
                {
                  this.current_link = index;
                  this.os.init(this.current_serialno);
                  this.os.reset();
                }
                else
                  goto label_20;
              }
              else
              {
                int[] serialno = new int[1];
                num1 = this.fetch_headers(this.vi[0], this.vc[0], serialno, page);
                this.current_serialno = serialno[0];
                if (num1 == 0)
                  ++this.current_link;
                else
                  goto label_23;
              }
              this.make_decode_ready();
            }
            this.os.pagein(page);
          }
          else
            goto label_11;
        }
        int num2 = this.vd.synthesis_pcmout((float[][][]) null, (int[]) null);
        this.vd.synthesis_blockin(this.vb);
        this.samptrack += (float) (this.vd.synthesis_pcmout((float[][][]) null, (int[]) null) - num2);
        this.bittrack += (float) (op.bytes * 8);
        if (granulepos != -1L && op.e_o_s == 0)
        {
          int currentLink = this.skable ? this.current_link : 0;
          int num3 = this.vd.synthesis_pcmout((float[][][]) null, (int[]) null);
          long num4 = granulepos - (long) num3;
          for (int index = 0; index < currentLink; ++index)
            num4 += this.pcmlengths[index];
          this.pcm_offset = num4;
        }
        return 1;
    label_11:
        return 0;
    label_20:
        return -1;
    label_23:
        return num1;
      }

      private int clear()
      {
        this.vb.clear();
        this.vd.clear();
        this.os.clear();
        if (this.vi != null && this.links != 0)
        {
          for (int index = 0; index < this.links; ++index)
          {
            this.vi[index].clear();
            this.vc[index].clear();
          }
          this.vi = (Info[]) null;
          this.vc = (Comment[]) null;
        }
        if (this.dataoffsets != null)
          this.dataoffsets = (long[]) null;
        if (this.pcmlengths != null)
          this.pcmlengths = (long[]) null;
        if (this.serialnos != null)
          this.serialnos = (int[]) null;
        if (this.offsets != null)
          this.offsets = (long[]) null;
        this.oy.clear();
        return 0;
      }

      private static int fseek(FileStream fis, long off, int whence)
      {
        if (fis.CanSeek)
        {
          try
          {
            if (whence == VorbisFile.SEEK_SET)
              fis.Seek(off, SeekOrigin.Begin);
            else if (whence == VorbisFile.SEEK_END)
              fis.Seek(fis.Length - off, SeekOrigin.Begin);
            else
              Console.Error.WriteLine($"seek: {(object) whence} is not supported");
          }
          catch (Exception ex)
          {
            Console.Error.WriteLine(ex.Message);
          }
          return 0;
        }
        try
        {
          if (whence == 0)
            fis.Seek(0L, SeekOrigin.Begin);
          fis.Seek(off, SeekOrigin.Begin);
        }
        catch (Exception ex)
        {
          Console.Error.WriteLine(ex.Message);
          return -1;
        }
        return 0;
      }

      private static long ftell(FileStream fis)
      {
        try
        {
          if (fis.CanSeek)
            return fis.Position;
        }
        catch (Exception ex)
        {
          Console.Error.WriteLine(ex.Message);
        }
        return 0;
      }

      private int open(FileStream iis, byte[] initial, int ibytes)
      {
        return this.open_callbacks(iis, initial, ibytes);
      }

      private int open_callbacks(FileStream iis, byte[] initial, int ibytes)
      {
        this.datasource = iis;
        this.oy.init();
        if (initial != null)
        {
          int destinationIndex = this.oy.buffer(ibytes);
          Array.Copy((Array) initial, 0, (Array) this.oy.data, destinationIndex, ibytes);
          this.oy.wrote(ibytes);
        }
        int num = !iis.CanSeek ? this.open_nonseekable() : this.open_seekable();
        if (num != 0)
        {
          this.datasource = (FileStream) null;
          this.clear();
        }
        return num;
      }

      public int streams() => this.links;

      public bool seekable() => this.skable;

      public int bitrate(int i)
      {
        if (i >= this.links)
          return -1;
        if (!this.skable && i != 0)
          return this.bitrate(0);
        if (i < 0)
        {
          long num = 0;
          for (int index = 0; index < this.links; ++index)
            num += (this.offsets[index + 1] - this.dataoffsets[index]) * 8L;
          return (int) Math.Round((double) num / (double) this.time_total(-1));
        }
        if (this.skable)
          return (int) Math.Round((double) ((this.offsets[i + 1] - this.dataoffsets[i]) * 8L) / (double) this.time_total(i));
        if (this.vi[i].bitrate_nominal > 0)
          return this.vi[i].bitrate_nominal;
        if (this.vi[i].bitrate_upper <= 0)
          return -1;
        return this.vi[i].bitrate_lower > 0 ? (this.vi[i].bitrate_upper + this.vi[i].bitrate_lower) / 2 : this.vi[i].bitrate_upper;
      }

      public int bitrate_instant()
      {
        int currentLink = this.skable ? this.current_link : 0;
        if ((double) this.samptrack == 0.0)
          return -1;
        int num = (int) ((double) this.bittrack / (double) this.samptrack * (double) this.vi[currentLink].rate + 0.5);
        this.bittrack = 0.0f;
        this.samptrack = 0.0f;
        return num;
      }

      public int serialnumber(int i)
      {
        if (i >= this.links)
          return -1;
        if (!this.skable && i >= 0)
          return this.serialnumber(-1);
        return i < 0 ? this.current_serialno : this.serialnos[i];
      }

      public long raw_total(int i)
      {
        if (!this.skable || i >= this.links)
          return -1;
        if (i >= 0)
          return this.offsets[i + 1] - this.offsets[i];
        long num = 0;
        for (int i1 = 0; i1 < this.links; ++i1)
          num += this.raw_total(i1);
        return num;
      }

      public long pcm_total(int i)
      {
        if (!this.skable || i >= this.links)
          return -1;
        if (i >= 0)
          return this.pcmlengths[i];
        long num = 0;
        for (int i1 = 0; i1 < this.links; ++i1)
          num += this.pcm_total(i1);
        return num;
      }

      public float time_total(int i)
      {
        if (!this.skable || i >= this.links)
          return -1f;
        if (i >= 0)
          return (float) this.pcmlengths[i] / (float) this.vi[i].rate;
        float num = 0.0f;
        for (int i1 = 0; i1 < this.links; ++i1)
          num += this.time_total(i1);
        return num;
      }

      public int raw_seek(int pos)
      {
        if (!this.skable)
          return -1;
        if (pos < 0 || (long) pos > this.offsets[this.links])
        {
          this.pcm_offset = -1L;
          this.decode_clear();
          return -1;
        }
        this.pcm_offset = -1L;
        this.decode_clear();
        this.seek_helper((long) pos);
        switch (this.process_packet(1))
        {
          case -1:
            this.pcm_offset = -1L;
            this.decode_clear();
            return -1;
          case 0:
            this.pcm_offset = this.pcm_total(-1);
            return 0;
          default:
            int num;
            do
            {
              num = this.process_packet(0);
              if (num == -1)
                goto label_10;
            }
            while (num != 0);
            return 0;
    label_10:
            this.pcm_offset = -1L;
            this.decode_clear();
            return -1;
        }
      }

      public int pcm_seek(long pos)
      {
        long num1 = this.pcm_total(-1);
        if (!this.skable)
          return -1;
        if (pos < 0L || pos > num1)
        {
          this.pcm_offset = -1L;
          this.decode_clear();
          return -1;
        }
        int index1;
        for (index1 = this.links - 1; index1 >= 0; --index1)
        {
          num1 -= this.pcmlengths[index1];
          if (pos >= num1)
            break;
        }
        long num2 = pos - num1;
        long num3 = this.offsets[index1 + 1];
        long offset = this.offsets[index1];
        int pos1 = (int) offset;
        Page page = new Page();
        while (offset < num3)
        {
          long offst = num3 - offset >= (long) VorbisFile.CHUNKSIZE ? (num3 + offset) / 2L : offset;
          this.seek_helper(offst);
          int nextPage = this.get_next_page(page, num3 - offst);
          if (nextPage == -1)
            num3 = offst;
          else if (page.granulepos() < num2)
          {
            pos1 = nextPage;
            offset = this.offset;
          }
          else
            num3 = offst;
        }
        if (this.raw_seek(pos1) != 0)
        {
          this.pcm_offset = -1L;
          this.decode_clear();
          return -1;
        }
        if (this.pcm_offset >= pos)
        {
          this.pcm_offset = -1L;
          this.decode_clear();
          return -1;
        }
        if (pos > this.pcm_total(-1))
        {
          this.pcm_offset = -1L;
          this.decode_clear();
          return -1;
        }
        while (this.pcm_offset < pos)
        {
          int num4 = (int) (pos - this.pcm_offset);
          float[][][] _pcm = new float[1][][];
          int[] index2 = new int[this.getInfo(-1).channels];
          int bytes = this.vd.synthesis_pcmout(_pcm, index2);
          float[][] numArray = _pcm[0];
          if (bytes > num4)
            bytes = num4;
          this.vd.synthesis_read(bytes);
          this.pcm_offset += (long) bytes;
          if (bytes < num4 && this.process_packet(1) == 0)
            this.pcm_offset = this.pcm_total(-1);
        }
        return 0;
      }

      public int time_seek(float seconds)
      {
        long num1 = this.pcm_total(-1);
        float num2 = this.time_total(-1);
        if (!this.skable)
          return -1;
        if ((double) seconds < 0.0 || (double) seconds > (double) num2)
        {
          this.pcm_offset = -1L;
          this.decode_clear();
          return -1;
        }
        int i;
        for (i = this.links - 1; i >= 0; --i)
        {
          num1 -= this.pcmlengths[i];
          num2 -= this.time_total(i);
          if ((double) seconds >= (double) num2)
            break;
        }
        return this.pcm_seek((long) ((double) num1 + ((double) seconds - (double) num2) * (double) this.vi[i].rate));
      }

      public long raw_tell() => this.offset;

      public long pcm_tell() => this.pcm_offset;

      public float time_tell()
      {
        int i = -1;
        long num1 = 0;
        float num2 = 0.0f;
        if (this.skable)
        {
          num1 = this.pcm_total(-1);
          num2 = this.time_total(-1);
          for (i = this.links - 1; i >= 0; --i)
          {
            num1 -= this.pcmlengths[i];
            num2 -= this.time_total(i);
            if (this.pcm_offset >= num1)
              break;
          }
        }
        return num2 + (float) (this.pcm_offset - num1) / (float) this.vi[i].rate;
      }

      public Info getInfo(int link)
      {
        return this.skable ? (link < 0 ? (this.decode_ready ? this.vi[this.current_link] : (Info) null) : (link >= this.links ? (Info) null : this.vi[link])) : (this.decode_ready ? this.vi[0] : (Info) null);
      }

      public Comment getComment(int link)
      {
        return this.skable ? (link < 0 ? (this.decode_ready ? this.vc[this.current_link] : (Comment) null) : (link >= this.links ? (Comment) null : this.vc[link])) : (this.decode_ready ? this.vc[0] : (Comment) null);
      }

      private int host_is_big_endian() => 0;

      public int read(
        byte[] buffer,
        int length,
        int bigendianp,
        int word,
        int sgned,
        int[] bitstream)
      {
        int num1 = this.host_is_big_endian();
        int num2 = 0;
    label_1:
        if (this.decode_ready)
        {
          float[][][] _pcm = new float[1][][];
          int[] index1 = new int[this.getInfo(-1).channels];
          int bytes = this.vd.synthesis_pcmout(_pcm, index1);
          float[][] numArray1 = _pcm[0];
          if (bytes != 0)
          {
            int channels = this.getInfo(-1).channels;
            int num3 = word * channels;
            if (bytes > length / num3)
              bytes = length / num3;
            if (word == 1)
            {
              int num4 = sgned != 0 ? 0 : 128 /*0x80*/;
              for (int index2 = 0; index2 < bytes; ++index2)
              {
                for (int index3 = 0; index3 < channels; ++index3)
                {
                  int num5 = (int) ((double) numArray1[index3][index1[index3] + index2] * 128.0 + 0.5);
                  if (num5 > (int) sbyte.MaxValue)
                    num5 = (int) sbyte.MaxValue;
                  else if (num5 < (int) sbyte.MinValue)
                    num5 = (int) sbyte.MinValue;
                  buffer[num2++] = (byte) (num5 + num4);
                }
              }
            }
            else
            {
              int num6 = sgned != 0 ? 0 : 32768 /*0x8000*/;
              if (num1 == bigendianp)
              {
                if (sgned != 0)
                {
                  for (int index4 = 0; index4 < channels; ++index4)
                  {
                    int num7 = index1[index4];
                    int index5 = index4 * 2;
                    for (int index6 = 0; index6 < bytes; ++index6)
                    {
                      int num8 = (int) ((double) numArray1[index4][num7 + index6] * (double) short.MaxValue);
                      if (num8 > (int) short.MaxValue)
                        num8 = (int) short.MaxValue;
                      else if (num8 < (int) short.MinValue)
                        num8 = (int) short.MinValue;
                      buffer[index5] = (byte) num8;
                      buffer[index5 + 1] = (byte) (num8 >>> 8);
                      index5 += num3;
                    }
                  }
                }
                else
                {
                  for (int index7 = 0; index7 < channels; ++index7)
                  {
                    float[] numArray2 = numArray1[index7];
                    int index8 = index7;
                    for (int index9 = 0; index9 < bytes; ++index9)
                    {
                      int num9 = (int) ((double) numArray2[index9] * 32768.0 + 0.5);
                      if (num9 > (int) short.MaxValue)
                        num9 = (int) short.MaxValue;
                      else if (num9 < (int) short.MinValue)
                        num9 = (int) short.MinValue;
                      buffer[index8] = (byte) (num9 + num6 >>> 8);
                      buffer[index8 + 1] = (byte) (num9 + num6);
                      index8 += channels * 2;
                    }
                  }
                }
              }
              else if (bigendianp != 0)
              {
                for (int index10 = 0; index10 < bytes; ++index10)
                {
                  for (int index11 = 0; index11 < channels; ++index11)
                  {
                    int num10 = (int) ((double) numArray1[index11][index10] * 32768.0 + 0.5);
                    if (num10 > (int) short.MaxValue)
                      num10 = (int) short.MaxValue;
                    else if (num10 < (int) short.MinValue)
                      num10 = (int) short.MinValue;
                    int num11 = num10 + num6;
                    byte[] numArray3 = buffer;
                    int index12 = num2;
                    int num12 = index12 + 1;
                    int num13 = (int) (byte) (num11 >>> 8);
                    numArray3[index12] = (byte) num13;
                    byte[] numArray4 = buffer;
                    int index13 = num12;
                    num2 = index13 + 1;
                    int num14 = (int) (byte) num11;
                    numArray4[index13] = (byte) num14;
                  }
                }
              }
              else
              {
                for (int index14 = 0; index14 < bytes; ++index14)
                {
                  for (int index15 = 0; index15 < channels; ++index15)
                  {
                    int num15 = (int) ((double) numArray1[index15][index14] * 32768.0 + 0.5);
                    if (num15 > (int) short.MaxValue)
                      num15 = (int) short.MaxValue;
                    else if (num15 < (int) short.MinValue)
                      num15 = (int) short.MinValue;
                    int num16 = num15 + num6;
                    byte[] numArray5 = buffer;
                    int index16 = num2;
                    int num17 = index16 + 1;
                    int num18 = (int) (byte) num16;
                    numArray5[index16] = (byte) num18;
                    byte[] numArray6 = buffer;
                    int index17 = num17;
                    num2 = index17 + 1;
                    int num19 = (int) (byte) (num16 >>> 8);
                    numArray6[index17] = (byte) num19;
                  }
                }
              }
            }
            this.vd.synthesis_read(bytes);
            this.pcm_offset += (long) bytes;
            if (bitstream != null)
              bitstream[0] = this.current_link;
            return bytes * num3;
          }
        }
        switch (this.process_packet(1))
        {
          case -1:
            return -1;
          case 0:
            return 0;
          default:
            goto label_1;
        }
      }

      public Info[] getInfo() => this.vi;

      public Comment[] getComment() => this.vc;
    }
}
