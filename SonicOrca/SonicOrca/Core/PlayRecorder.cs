// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.PlayRecorder
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.IO;

namespace SonicOrca.Core
{

    internal class PlayRecorder : IDisposable
    {
      private Stream _inputStream;
      private Stream _outputStream;
      private int _gameTick;

      public bool Recording => this._outputStream != null;

      public bool Playing => this._inputStream != null;

      public void Dispose()
      {
        if (this._outputStream == null)
          return;
        this._outputStream.Dispose();
      }

      public void BeginRecording(string path)
      {
        if (this._outputStream != null)
          throw new InvalidOperationException("Already recording.");
        this.BeginRecording((Stream) new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read));
      }

      public void BeginRecording(Stream stream)
      {
        this._outputStream = this._outputStream == null ? stream : throw new InvalidOperationException("Already recording.");
        this._gameTick = 0;
      }

      public void WriteEntry(PlayRecorder.Entry entry)
      {
        BinaryWriter binaryWriter = new BinaryWriter(this._outputStream);
        binaryWriter.Write(entry.Direction.X);
        binaryWriter.Write(entry.Direction.Y);
        binaryWriter.Write(entry.Action);
        binaryWriter.Write((byte) 0);
        binaryWriter.Write(entry.Position.X);
        binaryWriter.Write(entry.Position.Y);
        binaryWriter.Write((short) entry.LayerIndex);
        binaryWriter.Write(entry.State);
        binaryWriter.Write((short) entry.AnimationIndex);
        binaryWriter.Write((short) entry.AnimationFrameIndex);
        binaryWriter.Write(entry.Angle);
        binaryWriter.Flush();
        ++this._gameTick;
      }

      public void EndRecording()
      {
        if (this._outputStream == null)
          return;
        this._outputStream.Dispose();
        this._outputStream = (Stream) null;
      }

      public void BeginPlaying(string path)
      {
        if (this._inputStream != null)
          throw new InvalidOperationException("Already playing.");
        this.BeginPlaying((Stream) new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
      }

      public void BeginPlaying(Stream stream)
      {
        this._inputStream = this._inputStream == null ? stream : throw new InvalidOperationException("Already playing.");
        this._gameTick = 0;
      }

      public PlayRecorder.Entry GetNextEntry()
      {
        PlayRecorder.Entry nextEntry = new PlayRecorder.Entry();
        BinaryReader binaryReader = new BinaryReader(this._inputStream);
        try
        {
          nextEntry.Direction = new Vector2(binaryReader.ReadDouble(), binaryReader.ReadDouble());
          nextEntry.Action = ((uint) binaryReader.ReadByte() & 1U) > 0U;
          int num = (int) binaryReader.ReadByte();
          nextEntry.Position = new Vector2i(binaryReader.ReadInt32(), binaryReader.ReadInt32());
          nextEntry.LayerIndex = (int) binaryReader.ReadInt16();
          nextEntry.State = binaryReader.ReadInt32();
          nextEntry.AnimationIndex = (int) binaryReader.ReadInt16();
          nextEntry.AnimationFrameIndex = (int) binaryReader.ReadInt16();
          nextEntry.Angle = binaryReader.ReadSingle();
        }
        catch (EndOfStreamException ex)
        {
          this.EndPlaying();
          return (PlayRecorder.Entry) null;
        }
        ++this._gameTick;
        return nextEntry;
      }

      public void EndPlaying()
      {
        if (this._inputStream == null)
          return;
        this._inputStream.Dispose();
        this._inputStream = (Stream) null;
      }

      public class Entry
      {
        public Vector2 Direction { get; set; }

        public bool Action { get; set; }

        public Vector2i Position { get; set; }

        public int LayerIndex { get; set; }

        public int State { get; set; }

        public int AnimationIndex { get; set; }

        public int AnimationFrameIndex { get; set; }

        public float Angle { get; set; }
      }
    }
}
