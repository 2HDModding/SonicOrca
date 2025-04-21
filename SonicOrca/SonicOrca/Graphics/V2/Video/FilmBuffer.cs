// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Video.FilmBuffer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Accord.Video.FFMPEG;
using SonicOrca.Resources;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SonicOrca.Graphics.V2.Video
{

    public class FilmBuffer : IFilmBuffer, IDisposable, ILoadedResource
    {
      private VideoFileReader _reader = new VideoFileReader();
      private int _width;
      private int _height;
      private double _currentFrame;
      private double _numFrames;
      private ImageConverter converter = new ImageConverter();
      private byte[] _bytes;
      private string _path;

      public int Width => this._width;

      public int Height => this._height;

      public double CurrentTime => this._currentFrame / this._reader.FrameRate.Value;

      public double Duration => this._numFrames / this._reader.FrameRate.Value;

      public Resource Resource { get; set; }

      public void Dispose() => this._reader.Close();

      public void OnLoaded()
      {
        try
        {
          this._reader.Open(this._path);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
        this._width = this._reader.Width;
        this._height = this._reader.Height;
        this._numFrames = (double) this._reader.FrameCount;
        this._currentFrame = 0.0;
      }

      public FilmBuffer(string path) => this._path = path;

      public void Decode()
      {
        Bitmap bitmap = this._reader.ReadVideoFrame();
        if (bitmap != null)
        {
          ++this._currentFrame;
          Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
          BitmapData bitmapdata = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
          IntPtr scan0 = bitmapdata.Scan0;
          int length1 = Math.Abs(bitmapdata.Stride) * bitmap.Height;
          if (this._bytes == null)
            this._bytes = new byte[length1];
          else if (length1 != this._bytes.Length)
            this._bytes = new byte[length1];
          byte[] bytes = this._bytes;
          int length2 = length1;
          Marshal.Copy(scan0, bytes, 0, length2);
          bitmap.UnlockBits(bitmapdata);
        }
        bitmap.Dispose();
      }

      public byte[] GetArgbData() => this._bytes;
    }
}
