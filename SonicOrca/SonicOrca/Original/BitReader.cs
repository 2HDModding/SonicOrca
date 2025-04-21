// Decompiled with JetBrains decompiler
// Type: SonicOrca.Original.BitReader
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.IO;

namespace SonicOrca.Original
{

    internal class BitReader
    {
      private readonly Stream _stream;
      private readonly int _blockSize;
      private int _data;
      private int _bitIndex;

      public BitReader(Stream stream, int blockSize)
      {
        this._stream = stream;
        this._blockSize = blockSize;
        this.ReadBlock();
      }

      public int ReadBits(int count)
      {
        int num = 0;
        while (count-- > 0)
        {
          if (this.ReadBit())
            num = num << 1 | 1;
        }
        return num;
      }

      public bool ReadBit()
      {
        int num = (this._data & 1 << this._bitIndex++) != 0 ? 1 : 0;
        if (this._bitIndex < this._blockSize * 8)
          return num != 0;
        this.ReadBlock();
        return num != 0;
      }

      private void ReadBlock()
      {
        this._bitIndex = 0;
        switch (this._blockSize)
        {
          case 1:
            this._data = this._stream.ReadByte();
            break;
          case 2:
            this._data = this._stream.ReadByte() | this._stream.ReadByte() << 8;
            break;
          case 3:
            this._data = this._stream.ReadByte() | this._stream.ReadByte() << 8 | this._stream.ReadByte() << 16 /*0x10*/;
            break;
          case 4:
            this._data = this._stream.ReadByte() | this._stream.ReadByte() << 8 | this._stream.ReadByte() << 16 /*0x10*/ | this._stream.ReadByte() << 24;
            break;
          default:
            throw new InvalidOperationException();
        }
      }
    }
}
