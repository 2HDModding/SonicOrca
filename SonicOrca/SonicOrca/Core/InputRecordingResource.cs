// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.InputRecordingResource
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.IO;

namespace SonicOrca.Core
{

    public class InputRecordingResource : ILoadedResource, IDisposable
    {
      public byte[] Data { get; }

      public Resource Resource { get; set; }

      public InputRecordingResource(byte[] data) => this.Data = data;

      public void Dispose()
      {
      }

      public void OnLoaded()
      {
      }

      public System.IO.Stream GetStream() => (System.IO.Stream) new MemoryStream(this.Data);
    }
}
