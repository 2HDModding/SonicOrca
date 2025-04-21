// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Zlib.Adler32
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Zlib
{

    public class Adler32
    {
      private uint a = 1;
      private uint b;
      private const int _base = 65521;
      private const int _nmax = 5550;
      private int pend;

      public void Update(byte data)
      {
        if (this.pend >= 5550)
          this.updateModulus();
        this.a += (uint) data;
        this.b += this.a;
        ++this.pend;
      }

      public void Update(byte[] data) => this.Update(data, 0, data.Length);

      public void Update(byte[] data, int offset, int length)
      {
        int num = 5550 - this.pend;
        for (int index = 0; index < length; ++index)
        {
          if (index == num)
          {
            this.updateModulus();
            num = index + 5550;
          }
          this.a += (uint) data[index + offset];
          this.b += this.a;
          ++this.pend;
        }
      }

      public void Reset()
      {
        this.a = 1U;
        this.b = 0U;
        this.pend = 0;
      }

      private void updateModulus()
      {
        this.a %= 65521U;
        this.b %= 65521U;
        this.pend = 0;
      }

      public uint GetValue()
      {
        if (this.pend > 0)
          this.updateModulus();
        return this.b << 16 /*0x10*/ | this.a;
      }
    }
}
