// Decompiled with JetBrains decompiler
// Type: SonicOrca.Original.BitWriter
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.IO;

namespace SonicOrca.Original
{

    internal class BitWriter
    {
      private readonly Stream _stream;
      private readonly int _blockSize;
      private int _data;
      private int _bitIndex;

      public BitWriter(Stream stream, int blockSize)
      {
        this._stream = stream;
        this._blockSize = blockSize;
      }

      public void WriteBits(int value, int count)
      {
        while (count-- > 0)
          this.WriteBit(value >> count & 1);
      }

      public void WriteBit(int bit)
      {
        int num = 1 << this._blockSize * 8 - 1 - this._bitIndex++;
        if (bit != 0)
          this._data |= num;
        else
          this._data &= ~num;
        if (this._bitIndex < this._blockSize * 8)
          return;
        this.WriteBlock();
      }

      private void WriteBlock()
      {
        switch (this._blockSize)
        {
          case 1:
            this._stream.WriteByte((byte) (this._data & (int) byte.MaxValue));
            break;
          case 2:
            this._stream.WriteByte((byte) (this._data & (int) byte.MaxValue));
            this._stream.WriteByte((byte) (this._data >> 8 & (int) byte.MaxValue));
            break;
          case 3:
            this._stream.WriteByte((byte) (this._data & (int) byte.MaxValue));
            this._stream.WriteByte((byte) (this._data >> 8 & (int) byte.MaxValue));
            this._stream.WriteByte((byte) (this._data >> 16 /*0x10*/ & (int) byte.MaxValue));
            break;
          case 4:
            this._stream.WriteByte((byte) (this._data & (int) byte.MaxValue));
            this._stream.WriteByte((byte) (this._data >> 8 & (int) byte.MaxValue));
            this._stream.WriteByte((byte) (this._data >> 16 /*0x10*/ & (int) byte.MaxValue));
            this._stream.WriteByte((byte) (this._data >> 24 & (int) byte.MaxValue));
            break;
          default:
            throw new InvalidOperationException();
        }
        this._bitIndex = 0;
        this._data = 0;
      }
    }
}
