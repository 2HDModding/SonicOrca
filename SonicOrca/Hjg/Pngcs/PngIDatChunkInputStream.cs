// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.PngIDatChunkInputStream
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Hjg.Pngcs.Chunks;
using Hjg.Pngcs.Zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hjg.Pngcs
{

    internal class PngIDatChunkInputStream : Stream
    {
      private readonly Stream inputStream;
      private readonly CRC32 crcEngine;
      private bool checkCrc;
      private int lenLastChunk;
      private byte[] idLastChunk;
      private int toReadThisChunk;
      private bool ended;
      private long offset;
      public IList<PngIDatChunkInputStream.IdatChunkInfo> foundChunksInfo;

      public override void Write(byte[] buffer, int offset, int count)
      {
      }

      public override void SetLength(long value)
      {
      }

      public override long Seek(long offset, SeekOrigin origin) => -1;

      public override void Flush()
      {
      }

      public override long Position { get; set; }

      public override long Length => 0;

      public override bool CanWrite => false;

      public override bool CanRead => true;

      public override bool CanSeek => false;

      public PngIDatChunkInputStream(Stream iStream, int lenFirstChunk, long offset_0)
      {
        this.idLastChunk = new byte[4];
        this.toReadThisChunk = 0;
        this.ended = false;
        this.foundChunksInfo = (IList<PngIDatChunkInputStream.IdatChunkInfo>) new List<PngIDatChunkInputStream.IdatChunkInfo>();
        this.offset = offset_0;
        this.checkCrc = true;
        this.inputStream = iStream;
        this.crcEngine = new CRC32();
        this.lenLastChunk = lenFirstChunk;
        this.toReadThisChunk = lenFirstChunk;
        Array.Copy((Array) ChunkHelper.b_IDAT, 0, (Array) this.idLastChunk, 0, 4);
        this.crcEngine.Update(this.idLastChunk, 0, 4);
        this.foundChunksInfo.Add(new PngIDatChunkInputStream.IdatChunkInfo(this.lenLastChunk, offset_0 - 8L));
        if (this.lenLastChunk != 0)
          return;
        this.EndChunkGoForNext();
      }

      public override void Close() => base.Close();

      private void EndChunkGoForNext()
      {
        do
        {
          int num1 = PngHelperInternal.ReadInt4(this.inputStream);
          this.offset += 4L;
          if (this.checkCrc)
          {
            int num2 = (int) this.crcEngine.GetValue();
            if (this.lenLastChunk > 0 && num1 != num2)
              throw new PngjBadCrcException("error reading idat; offset: " + (object) this.offset);
            this.crcEngine.Reset();
          }
          this.lenLastChunk = PngHelperInternal.ReadInt4(this.inputStream);
          this.toReadThisChunk = this.lenLastChunk >= 0 ? this.lenLastChunk : throw new PngjInputException("invalid len for chunk: " + (object) this.lenLastChunk);
          PngHelperInternal.ReadBytes(this.inputStream, this.idLastChunk, 0, 4);
          this.offset += 8L;
          this.ended = !PngCsUtils.arraysEqual4(this.idLastChunk, ChunkHelper.b_IDAT);
          if (!this.ended)
          {
            this.foundChunksInfo.Add(new PngIDatChunkInputStream.IdatChunkInfo(this.lenLastChunk, this.offset - 8L));
            if (this.checkCrc)
              this.crcEngine.Update(this.idLastChunk, 0, 4);
          }
        }
        while (this.lenLastChunk == 0 && !this.ended);
      }

      public void ForceChunkEnd()
      {
        if (this.ended)
          return;
        byte[] numArray = new byte[this.toReadThisChunk];
        PngHelperInternal.ReadBytes(this.inputStream, numArray, 0, this.toReadThisChunk);
        if (this.checkCrc)
          this.crcEngine.Update(numArray, 0, this.toReadThisChunk);
        this.EndChunkGoForNext();
      }

      public override int Read(byte[] b, int off, int len_0)
      {
        if (this.ended)
          return -1;
        if (this.toReadThisChunk == 0)
          throw new Exception("this should not happen");
        int length = this.inputStream.Read(b, off, len_0 >= this.toReadThisChunk ? this.toReadThisChunk : len_0);
        if (length == -1)
          length = -2;
        if (length > 0)
        {
          if (this.checkCrc)
            this.crcEngine.Update(b, off, length);
          this.offset += (long) length;
          this.toReadThisChunk -= length;
        }
        if (length >= 0 && this.toReadThisChunk == 0)
          this.EndChunkGoForNext();
        return length;
      }

      public int Read(byte[] b) => this.Read(b, 0, b.Length);

      public override int ReadByte()
      {
        byte[] buffer = new byte[1];
        return this.Read(buffer, 0, 1) >= 0 ? (int) buffer[0] : -1;
      }

      public int GetLenLastChunk() => this.lenLastChunk;

      public byte[] GetIdLastChunk() => this.idLastChunk;

      public long GetOffset() => this.offset;

      public bool IsEnded() => this.ended;

      internal void DisableCrcCheck() => this.checkCrc = false;

      public class IdatChunkInfo
      {
        public readonly int len;
        public readonly long offset;

        public IdatChunkInfo(int len_0, long offset_1)
        {
          this.len = len_0;
          this.offset = offset_1;
        }
      }
    }
}
