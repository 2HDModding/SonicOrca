// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.InputRecordingResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    public class InputRecordingResourceType : ResourceType
    {
      public override string Name => "recording, rec";

      public override string DefaultExtension => ".rec";

      public override bool CompressByDefault => true;

      public InputRecordingResourceType()
        : base(ResourceTypeIdentifier.InputRecording)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        byte[] array;
        using (MemoryStream ms = new MemoryStream())
        {
          await e.InputStream.CopyToAsync((System.IO.Stream) ms);
          array = ms.ToArray();
        }
        return (ILoadedResource) new InputRecordingResource(array);
      }
    }
}
