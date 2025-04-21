// Decompiled with JetBrains decompiler
// Type: SonicOrca.HelperLibraries.H264.H264ResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Graphics.V2.Video;
using SonicOrca.Resources;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.HelperLibraries.H264
{

    public class H264ResourceType : ResourceType
    {
      public override string Name => "h265";

      public override string DefaultExtension => ".mp4";

      public override bool CompressByDefault => false;

      public H264ResourceType()
        : base(ResourceTypeIdentifier.VideoH264)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        return await Task.Run<ILoadedResource>((Func<ILoadedResource>) (() =>
        {
          return (ILoadedResource) new FilmBuffer(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\credits.film")
          {
            Resource = e.Resource
          };
        }));
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
