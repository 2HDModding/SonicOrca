// Decompiled with JetBrains decompiler
// Type: SonicOrca.HelperLibraries.Png.PngResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Hjg.Pngcs;
using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.HelperLibraries.Png
{

    public class PngResourceType : ResourceType
    {
      public override string Name => "png";

      public override string DefaultExtension => ".png";

      public override bool CompressByDefault => false;

      public PngResourceType()
        : base(ResourceTypeIdentifier.TexturePNG)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        PngResourceType.PixelData slowStream = await this.GetSlowStream(e.InputStream);
        ITexture texture = SonicOrcaGameContext.Singleton.Window.GraphicsContext.CreateTexture(slowStream.Width, slowStream.Height, slowStream.Channels, slowStream.Data);
        texture.Resource = e.Resource;
        return (ILoadedResource) texture;
      }

      private async Task<PngResourceType.PixelData> GetSlowStream(System.IO.Stream inputStream)
      {
        PngReader reader = (PngReader) null;
        PngResourceType.PixelData slowStream;
        try
        {
          reader = new PngReader(inputStream);
          int width = reader.ImgInfo.Cols;
          int height = reader.ImgInfo.Rows;
          int channels = reader.ImgInfo.Channels;
          byte[] argb = new byte[width * height * 4];
          await Task.Run((Action) (() =>
          {
            for (int nrow = 0; nrow < height; ++nrow)
            {
              ImageLine imageLine = reader.ReadRowByte(nrow);
              Buffer.BlockCopy((Array) imageLine.ScanlineB, 0, (Array) argb, nrow * width * imageLine.channels, width * imageLine.channels);
            }
          }));
          reader.End();
          slowStream = new PngResourceType.PixelData(width, height, channels, argb);
        }
        finally
        {
          if (reader != null)
            reader.End();
        }
        return slowStream;
      }

      [DllImport("SonicOrcaFastUtil.dll")]
      private static extern int BeginReadPNG(
        IntPtr input,
        int inputSize,
        out int width,
        out int height,
        out IntPtr output);

      [DllImport("SonicOrcaFastUtil.dll")]
      private static extern void EndReadPNG(IntPtr output);

      private async Task<PngResourceType.PixelData> GetFastStream(System.IO.Stream inputStream)
      {
        byte[] array;
        using (MemoryStream ms = new MemoryStream())
        {
          await inputStream.CopyToAsync((System.IO.Stream) ms);
          array = ms.ToArray();
        }
        int width;
        int height;
        IntPtr output;
        if (PngResourceType.BeginReadPNG(Marshal.UnsafeAddrOfPinnedArrayElement((Array) array, 0), array.Length, out width, out height, out output) == 0)
          throw new ResourceException("Unable to load PNG.");
        byte[] numArray = new byte[width * height * 4];
        Marshal.Copy(output, numArray, 0, numArray.Length);
        PngResourceType.EndReadPNG(output);
        return new PngResourceType.PixelData(width, height, 4, numArray);
      }

      private struct PixelData
      {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public byte[] Data { get; private set; }

        public int Channels { get; private set; }

        public PixelData(int width, int height, int channels, byte[] data)
          : this()
        {
          this.Width = width;
          this.Height = height;
          this.Channels = channels;
          this.Data = data;
        }
      }
    }
}
