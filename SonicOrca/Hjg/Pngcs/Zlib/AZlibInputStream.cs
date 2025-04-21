// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Zlib.AZlibInputStream
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.IO;

namespace Hjg.Pngcs.Zlib
{

    public abstract class AZlibInputStream : Stream
    {
      protected readonly Stream rawStream;
      protected readonly bool leaveOpen;

      public AZlibInputStream(Stream st, bool leaveOpen)
      {
        this.rawStream = st;
        this.leaveOpen = leaveOpen;
      }

      public override bool CanRead => true;

      public override bool CanWrite => false;

      public override void SetLength(long value) => throw new NotImplementedException();

      public override bool CanSeek => false;

      public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

      public override long Position
      {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
      }

      public override long Length => throw new NotImplementedException();

      public override void Write(byte[] buffer, int offset, int count)
      {
        throw new NotImplementedException();
      }

      public override bool CanTimeout => false;

      public abstract string getImplementationId();
    }
}
