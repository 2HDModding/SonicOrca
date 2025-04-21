// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkUNKNOWN
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkUNKNOWN : PngChunkMultiple
    {
      private byte[] data;

      public PngChunkUNKNOWN(string id, ImageInfo info)
        : base(id, info)
      {
      }

      private PngChunkUNKNOWN(PngChunkUNKNOWN c, ImageInfo info)
        : base(c.Id, info)
      {
        Array.Copy((Array) c.data, 0, (Array) this.data, 0, c.data.Length);
      }

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.NONE;
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw emptyChunk = this.createEmptyChunk(this.data.Length, false);
        emptyChunk.Data = this.data;
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw c) => this.data = c.Data;

      public byte[] GetData() => this.data;

      public void SetData(byte[] data_0) => this.data = data_0;

      public override void CloneDataFromRead(PngChunk other)
      {
        this.data = ((PngChunkUNKNOWN) other).data;
      }
    }
}
