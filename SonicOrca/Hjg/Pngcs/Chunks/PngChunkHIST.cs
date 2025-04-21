// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkHIST
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkHIST(ImageInfo info) : PngChunkSingle(PngChunkHIST.ID, info)
    {
      public static readonly string ID = "hIST";
      private int[] hist = new int[0];

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
      }

      public override ChunkRaw CreateRawChunk()
      {
        if (!this.ImgInfo.Indexed)
          throw new PngjException("only indexed images accept a HIST chunk");
        ChunkRaw emptyChunk = this.createEmptyChunk(this.hist.Length * 2, true);
        for (int index = 0; index < this.hist.Length; ++index)
          PngHelperInternal.WriteInt2tobytes(this.hist[index], emptyChunk.Data, index * 2);
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw c)
      {
        if (!this.ImgInfo.Indexed)
          throw new PngjException("only indexed images accept a HIST chunk");
        this.hist = new int[c.Data.Length / 2];
        for (int index = 0; index < this.hist.Length; ++index)
          this.hist[index] = PngHelperInternal.ReadInt2fromBytes(c.Data, index * 2);
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkHIST pngChunkHist = (PngChunkHIST) other;
        this.hist = new int[pngChunkHist.hist.Length];
        Array.Copy((Array) pngChunkHist.hist, 0, (Array) this.hist, 0, pngChunkHist.hist.Length);
      }

      public int[] GetHist() => this.hist;

      public void SetHist(int[] hist) => this.hist = hist;
    }
}
