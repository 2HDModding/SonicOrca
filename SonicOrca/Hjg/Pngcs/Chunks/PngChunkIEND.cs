// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkIEND
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkIEND(ImageInfo info) : PngChunkSingle("IEND", info)
    {
      public const string ID = "IEND";

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.NA;
      }

      public override ChunkRaw CreateRawChunk() => new ChunkRaw(0, ChunkHelper.b_IEND, false);

      public override void ParseFromRaw(ChunkRaw c)
      {
      }

      public override void CloneDataFromRead(PngChunk other)
      {
      }
    }
}
