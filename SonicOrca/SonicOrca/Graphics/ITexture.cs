// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.ITexture
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;

namespace SonicOrca.Graphics
{

    public interface ITexture : IDisposable, ILoadedResource
    {
      int Width { get; }

      int Height { get; }

      int Id { get; }

      TextureFiltering Filtering { get; set; }

      TextureWrapping Wrapping { get; set; }

      byte[] GetArgbData();

      void SetArgbData(int width, int height, byte[] data);
    }
}
