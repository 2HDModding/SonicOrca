// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkSRGB
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkSRGB(ImageInfo info) : PngChunkSingle("sRGB", info)
    {
      public const string ID = "sRGB";
      public const int RENDER_INTENT_Perceptual = 0;
      public const int RENDER_INTENT_Relative_colorimetric = 1;
      public const int RENDER_INTENT_Saturation = 2;
      public const int RENDER_INTENT_Absolute_colorimetric = 3;

      public int Intent { get; set; }

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw emptyChunk = this.createEmptyChunk(1, true);
        emptyChunk.Data[0] = (byte) this.Intent;
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw c)
      {
        this.Intent = c.Length == 1 ? PngHelperInternal.ReadInt1fromByte(c.Data, 0) : throw new PngjException("bad chunk length " + (object) c);
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        this.Intent = ((PngChunkSRGB) other).Intent;
      }
    }
}
