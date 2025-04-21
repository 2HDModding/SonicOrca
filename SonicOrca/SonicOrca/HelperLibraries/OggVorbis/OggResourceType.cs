// Decompiled with JetBrains decompiler
// Type: SonicOrca.HelperLibraries.OggVorbis.OggResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.HelperLibraries.OggVorbis
{

    public class OggResourceType : ResourceType
    {
      public override string Name => "ogg";

      public override string DefaultExtension => ".ogg";

      public override bool CompressByDefault => false;

      public OggResourceType()
        : base(ResourceTypeIdentifier.SampleOGG)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        System.IO.Stream slowStream = await this.GetSlowStream(e.InputStream);
        return await ResourceType.FromIdentifier(ResourceTypeIdentifier.SampleWAV).LoadAsync(new ResourceLoadArgs(e.ResourceTree, e.Resource, slowStream), ct);
      }

      private Task<System.IO.Stream> GetSlowStream(System.IO.Stream inputStream)
      {
        return Task.Run<System.IO.Stream>((Func<System.IO.Stream>) (() => (System.IO.Stream) new OggDecodeStream(inputStream, false)));
      }

      [DllImport("SonicOrcaFastUtil.dll")]
      private static extern int BeginReadVorbis(
        IntPtr input,
        int inputSize,
        out IntPtr output,
        out int outputLength);

      [DllImport("SonicOrcaFastUtil.dll")]
      private static extern void EndReadVorbis(IntPtr output);

      private async Task<System.IO.Stream> GetFastStream(System.IO.Stream inputStream)
      {
        byte[] array;
        using (MemoryStream ms = new MemoryStream())
        {
          await inputStream.CopyToAsync((System.IO.Stream) ms);
          array = ms.ToArray();
        }
        IntPtr output;
        int outputLength;
        if (OggResourceType.BeginReadVorbis(Marshal.UnsafeAddrOfPinnedArrayElement((Array) array, 0), array.Length, out output, out outputLength) == 0)
          throw new ResourceException("Unable to read OGG Vorbis data");
        byte[] numArray = new byte[outputLength];
        Marshal.Copy(output, numArray, 0, outputLength);
        OggResourceType.EndReadVorbis(output);
        return (System.IO.Stream) new MemoryStream(numArray);
      }
    }
}
