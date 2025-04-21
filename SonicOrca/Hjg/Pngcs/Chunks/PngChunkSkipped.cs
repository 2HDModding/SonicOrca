// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkSkipped
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    internal class PngChunkSkipped : PngChunk
    {
      internal PngChunkSkipped(string id, ImageInfo imgInfo, int clen)
        : base(id, imgInfo)
      {
        this.Length = clen;
      }

      public sealed override bool AllowsMultiple() => true;

      public sealed override ChunkRaw CreateRawChunk()
      {
        throw new PngjException("Non supported for a skipped chunk");
      }

      public sealed override void ParseFromRaw(ChunkRaw c)
      {
        throw new PngjException("Non supported for a skipped chunk");
      }

      public sealed override void CloneDataFromRead(PngChunk other)
      {
        throw new PngjException("Non supported for a skipped chunk");
      }

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.NONE;
      }
    }
}
