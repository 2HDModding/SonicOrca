// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkIDAT
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkIDAT : PngChunkMultiple
    {
      public const string ID = "IDAT";

      public PngChunkIDAT(ImageInfo i, int len, long offset)
        : base("IDAT", i)
      {
        this.Length = len;
        this.Offset = offset;
      }

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.NA;
      }

      public override ChunkRaw CreateRawChunk() => (ChunkRaw) null;

      public override void ParseFromRaw(ChunkRaw c)
      {
      }

      public override void CloneDataFromRead(PngChunk other)
      {
      }
    }
}
