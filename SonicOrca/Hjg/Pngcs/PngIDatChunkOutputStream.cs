// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.PngIDatChunkOutputStream
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Hjg.Pngcs.Chunks;
using System.IO;

namespace Hjg.Pngcs
{

    internal class PngIDatChunkOutputStream : ProgressiveOutputStream
    {
      private const int SIZE_DEFAULT = 32768 /*0x8000*/;
      private readonly Stream outputStream;

      public PngIDatChunkOutputStream(Stream outputStream_0)
        : this(outputStream_0, 32768 /*0x8000*/)
      {
      }

      public PngIDatChunkOutputStream(Stream outputStream_0, int size)
        : base(size > 8 ? size : 32768 /*0x8000*/)
      {
        this.outputStream = outputStream_0;
      }

      protected override void FlushBuffer(byte[] b, int len)
      {
        new ChunkRaw(len, ChunkHelper.b_IDAT, false)
        {
          Data = b
        }.WriteChunk(this.outputStream);
      }

      public override void Close() => this.Flush();
    }
}
