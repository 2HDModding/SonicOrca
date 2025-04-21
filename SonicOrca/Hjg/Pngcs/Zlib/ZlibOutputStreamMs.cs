// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Zlib.ZlibOutputStreamMs
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.IO;
using System.IO.Compression;

namespace Hjg.Pngcs.Zlib
{

    internal class ZlibOutputStreamMs(
      Stream st,
      int compressLevel,
      EDeflateCompressStrategy strat,
      bool leaveOpen) : AZlibOutputStream(st, compressLevel, strat, leaveOpen)
    {
      private DeflateStream deflateStream;
      private Adler32 adler32 = new Adler32();
      private bool initdone;
      private bool closed;

      public override void WriteByte(byte value)
      {
        if (!this.initdone)
          this.doInit();
        if (this.deflateStream == null)
          this.initStream();
        base.WriteByte(value);
        this.adler32.Update(value);
      }

      public override void Write(byte[] array, int offset, int count)
      {
        if (count == 0)
          return;
        if (!this.initdone)
          this.doInit();
        if (this.deflateStream == null)
          this.initStream();
        this.deflateStream.Write(array, offset, count);
        this.adler32.Update(array, offset, count);
      }

      public override void Close()
      {
        if (!this.initdone)
          this.doInit();
        if (this.closed)
          return;
        this.closed = true;
        if (this.deflateStream != null)
        {
          this.deflateStream.Close();
        }
        else
        {
          this.rawStream.WriteByte((byte) 3);
          this.rawStream.WriteByte((byte) 0);
        }
        uint num = this.adler32.GetValue();
        this.rawStream.WriteByte((byte) (num >> 24 & (uint) byte.MaxValue));
        this.rawStream.WriteByte((byte) (num >> 16 /*0x10*/ & (uint) byte.MaxValue));
        this.rawStream.WriteByte((byte) (num >> 8 & (uint) byte.MaxValue));
        this.rawStream.WriteByte((byte) (num & (uint) byte.MaxValue));
        if (this.leaveOpen)
          return;
        this.rawStream.Close();
      }

      private void initStream()
      {
        if (this.deflateStream != null)
          return;
        CompressionLevel compressionLevel = (CompressionLevel) 0;
        if (this.compressLevel >= 1 && this.compressLevel <= 5)
          compressionLevel = (CompressionLevel) 1;
        else if (this.compressLevel == 0)
          compressionLevel = (CompressionLevel) 2;
        this.deflateStream = new DeflateStream(this.rawStream, compressionLevel, true);
      }

      private void doInit()
      {
        if (this.initdone)
          return;
        this.initdone = true;
        int num1 = 120;
        int num2 = 218;
        if (this.compressLevel >= 5 && this.compressLevel <= 6)
          num2 = 156;
        else if (this.compressLevel >= 3 && this.compressLevel <= 4)
          num2 = 94;
        else if (this.compressLevel <= 2)
          num2 = 1;
        int num3 = num2 - (num1 * 256 /*0x0100*/ + num2) % 31 /*0x1F*/;
        if (num3 < 0)
          num3 += 31 /*0x1F*/;
        this.rawStream.WriteByte((byte) num1);
        this.rawStream.WriteByte((byte) num3);
      }

      public override void Flush()
      {
        if (this.deflateStream == null)
          return;
        this.deflateStream.Flush();
      }

      public override string getImplementationId() => "Zlib deflater: .Net CLR 4.5";
    }
}
