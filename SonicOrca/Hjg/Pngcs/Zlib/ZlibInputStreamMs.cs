// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Zlib.ZlibInputStreamMs
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.IO;
using System.IO.Compression;

namespace Hjg.Pngcs.Zlib
{

    internal class ZlibInputStreamMs(Stream st, bool leaveOpen) : AZlibInputStream(st, leaveOpen)
    {
      private DeflateStream deflateStream;
      private bool initdone;
      private bool closed;
      private bool fdict;
      private int cmdinfo;
      private byte[] dictid;
      private byte[] crcread;

      public override int Read(byte[] array, int offset, int count)
      {
        if (!this.initdone)
          this.doInit();
        if (this.deflateStream == null && count > 0)
          this.initStream();
        int num = this.deflateStream.Read(array, offset, count);
        if (num < 1 && this.crcread == null)
        {
          this.crcread = new byte[4];
          for (int index = 0; index < 4; ++index)
            this.crcread[index] = (byte) this.rawStream.ReadByte();
        }
        return num;
      }

      public override void Close()
      {
        if (!this.initdone)
          this.doInit();
        if (this.closed)
          return;
        this.closed = true;
        if (this.deflateStream != null)
          this.deflateStream.Close();
        if (this.crcread == null)
        {
          this.crcread = new byte[4];
          for (int index = 0; index < 4; ++index)
            this.crcread[index] = (byte) this.rawStream.ReadByte();
        }
        if (this.leaveOpen)
          return;
        this.rawStream.Close();
      }

      private void initStream()
      {
        if (this.deflateStream != null)
          return;
        this.deflateStream = new DeflateStream(this.rawStream, CompressionMode.Decompress, true);
      }

      private void doInit()
      {
        if (this.initdone)
          return;
        this.initdone = true;
        int num1 = this.rawStream.ReadByte();
        int num2 = this.rawStream.ReadByte();
        if (num1 == -1 || num2 == -1)
          return;
        if ((num1 & 15) != 8)
          throw new Exception("Bad compression method for ZLIB header: cmf=" + (object) num1);
        this.cmdinfo = (num1 & 240 /*0xF0*/) >> 8;
        this.fdict = (num2 & 32 /*0x20*/) != 0;
        if (!this.fdict)
          return;
        this.dictid = new byte[4];
        for (int index = 0; index < 4; ++index)
          this.dictid[index] = (byte) this.rawStream.ReadByte();
      }

      public override void Flush()
      {
        if (this.deflateStream == null)
          return;
        this.deflateStream.Flush();
      }

      public override string getImplementationId() => "Zlib inflater: .Net CLR 4.5";
    }
}
