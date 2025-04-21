// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.ResamplerStream
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.IO;

namespace SonicOrca.Audio
{

    public class ResamplerStream : Stream
    {
      private Stream _baseStream;

      public int InputSampleRate { get; set; }

      public int OutputSampleRate { get; set; }

      public ResamplerStream(Stream stream, int inputSampleRate, int outputSampleRate)
      {
        this._baseStream = stream;
        this.InputSampleRate = inputSampleRate;
        this.OutputSampleRate = outputSampleRate;
      }

      public override bool CanRead => this._baseStream.CanRead;

      public override bool CanSeek => this._baseStream.CanSeek;

      public override bool CanWrite => false;

      public override long Length => this._baseStream.Length;

      public override long Position
      {
        get => this._baseStream.Position;
        set => this._baseStream.Position = value;
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        double num = (double) this.InputSampleRate / (double) this.OutputSampleRate;
        int count1 = (int) ((double) count * num);
        byte[] numArray = new byte[count1];
        this._baseStream.Read(numArray, 0, count1);
        float[] leftSamples;
        float[] rightSamples;
        Sample.PCMToSamples(numArray, out leftSamples, out rightSamples);
        byte[] pcm = Sample.SamplesToPCM(ResamplerStream.LerpSamples(leftSamples, count / 4), ResamplerStream.LerpSamples(rightSamples, count / 4));
        Array.Copy((Array) pcm, 0, (Array) buffer, offset, pcm.Length);
        return pcm.Length;
      }

      private static float[] LerpSamples(float[] source, int desiredLength)
      {
        float[] numArray = new float[desiredLength];
        for (int index1 = 0; index1 < desiredLength; ++index1)
        {
          float num1 = (float) index1 / (float) (desiredLength - 1) * (float) (source.Length - 1);
          int index2 = (int) Math.Floor((double) num1);
          int index3 = (int) Math.Ceiling((double) num1);
          float num2 = source[index2];
          float num3 = source[index3];
          numArray[index1] = (double) num3 - (double) num2 == 0.0 ? num2 : num2 + (float) (((double) num3 - (double) num2) * ((double) num1 - (double) index2));
        }
        return numArray;
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        return this._baseStream.Seek(offset, origin);
      }

      public override void Flush() => throw new InvalidOperationException();

      public override void SetLength(long value) => throw new InvalidOperationException();

      public override void Write(byte[] buffer, int offset, int count)
      {
        throw new InvalidOperationException();
      }
    }
}
