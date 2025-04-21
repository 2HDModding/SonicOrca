// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.ChunkPredicateId2
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    internal class ChunkPredicateId2 : ChunkPredicate
    {
      private readonly string id;
      private readonly string innerid;

      public ChunkPredicateId2(string id, string inner)
      {
        this.id = id;
        this.innerid = inner;
      }

      public bool Matches(PngChunk c)
      {
        return c.Id.Equals(this.id) && (!(c is PngChunkTextVar) || ((PngChunkTextVar) c).GetKey().Equals(this.innerid)) && (!(c is PngChunkSPLT) || ((PngChunkSPLT) c).PalName.Equals(this.innerid));
      }
    }
}
