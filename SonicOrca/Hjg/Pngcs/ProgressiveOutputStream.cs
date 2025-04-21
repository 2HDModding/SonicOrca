// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.ProgressiveOutputStream
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.IO;

namespace Hjg.Pngcs
{

    internal abstract class ProgressiveOutputStream : MemoryStream
    {
      private readonly int size;
      private long countFlushed;

      public ProgressiveOutputStream(int size_0)
      {
        this.size = size_0;
        if (this.size < 8)
          throw new PngjException("bad size for ProgressiveOutputStream: " + (object) this.size);
      }

      public override void Close()
      {
        this.Flush();
        base.Close();
      }

      public override void Flush()
      {
        base.Flush();
        this.CheckFlushBuffer(true);
      }

      public override void Write(byte[] b, int off, int len)
      {
        base.Write(b, off, len);
        this.CheckFlushBuffer(false);
      }

      public void Write(byte[] b)
      {
        this.Write(b, 0, b.Length);
        this.CheckFlushBuffer(false);
      }

      private void CheckFlushBuffer(bool forced)
      {
        int num1 = (int) this.Position;
        byte[] buffer = this.GetBuffer();
        while (forced || num1 >= this.size)
        {
          int num2 = this.size;
          if (num2 > num1)
            num2 = num1;
          if (num2 == 0)
            break;
          this.FlushBuffer(buffer, num2);
          this.countFlushed += (long) num2;
          int length = num1 - num2;
          num1 = length;
          this.Position = (long) num1;
          if (length > 0)
            Array.Copy((Array) buffer, num2, (Array) buffer, 0, length);
        }
      }

      protected abstract void FlushBuffer(byte[] b, int n);

      public long GetCountFlushed() => this.countFlushed;
    }
}
