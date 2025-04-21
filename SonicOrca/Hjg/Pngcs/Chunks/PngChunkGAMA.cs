// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkGAMA
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkGAMA(ImageInfo info) : PngChunkSingle("gAMA", info)
    {
      public const string ID = "gAMA";
      private double gamma;

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw emptyChunk = this.createEmptyChunk(4, true);
        PngHelperInternal.WriteInt4tobytes((int) (this.gamma * 100000.0 + 0.5), emptyChunk.Data, 0);
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw chunk)
      {
        if (chunk.Length != 4)
          throw new PngjException("bad chunk " + (object) chunk);
        this.gamma = (double) PngHelperInternal.ReadInt4fromBytes(chunk.Data, 0) / 100000.0;
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        this.gamma = ((PngChunkGAMA) other).gamma;
      }

      public double GetGamma() => this.gamma;

      public void SetGamma(double gamma) => this.gamma = gamma;
    }
}
