// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkMultiple
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public abstract class PngChunkMultiple : PngChunk
    {
      internal PngChunkMultiple(string id, ImageInfo imgInfo)
        : base(id, imgInfo)
      {
      }

      public sealed override bool AllowsMultiple() => true;
    }
}
