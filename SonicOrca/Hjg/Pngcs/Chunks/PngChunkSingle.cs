// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkSingle
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public abstract class PngChunkSingle : PngChunk
    {
      public PngChunkSingle(string id, ImageInfo imgInfo)
        : base(id, imgInfo)
      {
      }

      public sealed override bool AllowsMultiple() => false;

      public override int GetHashCode()
      {
        return 31 /*0x1F*/ * 1 + (this.Id == null ? 0 : this.Id.GetHashCode());
      }

      public override bool Equals(object obj)
      {
        return obj is PngChunkSingle && this.Id != null && this.Id.Equals(((PngChunk) obj).Id);
      }
    }
}
