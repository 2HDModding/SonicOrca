// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkSTER
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkSTER(ImageInfo info) : PngChunkSingle("sTER", info)
    {
      public const string ID = "sTER";

      public byte Mode { get; set; }

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.BEFORE_IDAT;
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw emptyChunk = this.createEmptyChunk(1, true);
        emptyChunk.Data[0] = this.Mode;
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw chunk)
      {
        this.Mode = chunk.Length == 1 ? chunk.Data[0] : throw new PngjException("bad chunk length " + (object) chunk);
      }

      public override void CloneDataFromRead(PngChunk other) => this.Mode = ((PngChunkSTER) other).Mode;
    }
}
