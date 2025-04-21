// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.WavResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Audio
{

    public class WavResourceType : ResourceType
    {
      public override string Name => "wav";

      public override string DefaultExtension => ".wav";

      public override bool CompressByDefault => false;

      public WavResourceType()
        : base(ResourceTypeIdentifier.SampleWAV)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        return await Task.Run<ILoadedResource>((Func<ILoadedResource>) (() =>
        {
          BinaryReader binaryReader = new BinaryReader(e.InputStream);
          if (binaryReader.ReadInt32() != 1179011410)
            throw new ResourceException("Invalid wav signature.");
          binaryReader.ReadInt32();
          if (binaryReader.ReadInt32() != 1163280727)
            throw new ResourceException("Invalid wav signature.");
          if (binaryReader.ReadInt32() != 544501094)
            throw new ResourceException("Invalid wav signature.");
          binaryReader.ReadInt32();
          short channels = binaryReader.ReadInt16() == (short) 1 ? binaryReader.ReadInt16() : throw new ResourceException("Non PCM wav signature.");
          int sampleRate = binaryReader.ReadInt32();
          binaryReader.ReadInt32();
          int num = (int) binaryReader.ReadInt16();
          short bitsPerSample = binaryReader.ReadInt16();
          if (binaryReader.ReadInt32() != 1635017060)
            throw new ResourceException("Invalid wav format.");
          return (ILoadedResource) new Sample(binaryReader.ReadBytes(binaryReader.ReadInt32()), (int) bitsPerSample, sampleRate, (int) channels)
          {
            Resource = e.Resource
          };
        }));
      }
    }
}
