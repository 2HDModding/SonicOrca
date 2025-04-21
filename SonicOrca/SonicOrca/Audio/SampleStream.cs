// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.SampleStream
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.IO;

namespace SonicOrca.Audio
{

    internal class SampleStream : Stream
    {
      private readonly Sample _sample;
      private readonly byte[] _pcmData;
      private readonly long? _loopPoint;
      private readonly long _length;
      private readonly long _lengthFromLoopPoint;

      public SampleStream(Sample sample, int? loopSampleIndex = null)
      {
        this._sample = sample;
        this._pcmData = sample.PcmData;
        this._length = (long) this._sample.PcmData.Length;
        if (!loopSampleIndex.HasValue)
          return;
        this._loopPoint = new long?(sample.GetPcmDataOffset(loopSampleIndex.Value));
        this._lengthFromLoopPoint = this._length - this._loopPoint.Value;
      }

      public override bool CanRead => throw new NotImplementedException();

      public override bool CanSeek => true;

      public override bool CanWrite => false;

      public override long Length => this._length;

      public override long Position { get; set; }

      public override long Seek(long offset, SeekOrigin origin)
      {
        if (origin == SeekOrigin.End)
          throw new NotImplementedException();
        if (origin == SeekOrigin.Current)
          offset += this.Position;
        long num1 = offset - (long) this._sample.PcmData.Length;
        if (num1 > 0L)
        {
          long? loopPoint = this._loopPoint;
          long num2;
          if (!loopPoint.HasValue)
          {
            num2 = this._length;
          }
          else
          {
            loopPoint = this._loopPoint;
            num2 = loopPoint.Value + num1 % this._lengthFromLoopPoint;
          }
          this.Position = num2;
        }
        else
          this.Position = offset;
        return this.Position;
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        long val2 = this._length - this.Position;
        return (long) count > val2 && this._loopPoint.HasValue ? this.ReadWrapped(buffer, offset, count) : this.ReadChunk(buffer, offset, (int) Math.Min((long) count, val2));
      }

      private int ReadWrapped(byte[] buffer, int offset, int count)
      {
        int num = 0;
        while (num < count)
        {
          long val1 = this._length - this.Position;
          long val2 = (long) (count - num);
          int count1 = (int) Math.Min(val1, val2);
          this.ReadChunk(buffer, offset, count1);
          offset += count1;
          num += count1;
          if (val1 < val2)
          {
            long? loopPoint = this._loopPoint;
            long length;
            if (!loopPoint.HasValue)
            {
              length = this._length;
            }
            else
            {
              loopPoint = this._loopPoint;
              length = loopPoint.Value;
            }
            this.Position = length;
          }
        }
        return num;
      }

      private int ReadChunk(byte[] buffer, int offset, int count)
      {
        Array.Copy((Array) this._pcmData, this.Position, (Array) buffer, (long) offset, (long) count);
        this.Position += (long) count;
        return count;
      }

      public override void Flush() => throw new InvalidOperationException();

      public override void SetLength(long value) => throw new InvalidOperationException();

      public override void Write(byte[] buffer, int offset, int count)
      {
        throw new InvalidOperationException();
      }
    }
}
