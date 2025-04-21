// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.ChunksList
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Collections.Generic;
using System.Text;

namespace Hjg.Pngcs.Chunks
{

    public class ChunksList
    {
      internal const int CHUNK_GROUP_0_IDHR = 0;
      internal const int CHUNK_GROUP_1_AFTERIDHR = 1;
      internal const int CHUNK_GROUP_2_PLTE = 2;
      internal const int CHUNK_GROUP_3_AFTERPLTE = 3;
      internal const int CHUNK_GROUP_4_IDAT = 4;
      internal const int CHUNK_GROUP_5_AFTERIDAT = 5;
      internal const int CHUNK_GROUP_6_END = 6;
      protected List<PngChunk> chunks;
      internal readonly ImageInfo imageInfo;

      internal ChunksList(ImageInfo imfinfo)
      {
        this.chunks = new List<PngChunk>();
        this.imageInfo = imfinfo;
      }

      public Dictionary<string, int> GetChunksKeys()
      {
        Dictionary<string, int> chunksKeys = new Dictionary<string, int>();
        foreach (PngChunk chunk in this.chunks)
          chunksKeys[chunk.Id] = chunksKeys.ContainsKey(chunk.Id) ? chunksKeys[chunk.Id] + 1 : 1;
        return chunksKeys;
      }

      public List<PngChunk> GetChunks() => new List<PngChunk>((IEnumerable<PngChunk>) this.chunks);

      internal static List<PngChunk> GetXById(List<PngChunk> list, string id, string innerid)
      {
        return innerid == null ? ChunkHelper.FilterList(list, (ChunkPredicate) new ChunkPredicateId(id)) : ChunkHelper.FilterList(list, (ChunkPredicate) new ChunkPredicateId2(id, innerid));
      }

      public void AppendReadChunk(PngChunk chunk, int chunkGroup)
      {
        chunk.ChunkGroup = chunkGroup;
        this.chunks.Add(chunk);
      }

      public List<PngChunk> GetById(string id) => this.GetById(id, (string) null);

      public List<PngChunk> GetById(string id, string innerid)
      {
        return ChunksList.GetXById(this.chunks, id, innerid);
      }

      public PngChunk GetById1(string id) => this.GetById1(id, false);

      public PngChunk GetById1(string id, bool failIfMultiple)
      {
        return this.GetById1(id, (string) null, failIfMultiple);
      }

      public PngChunk GetById1(string id, string innerid, bool failIfMultiple)
      {
        List<PngChunk> byId = this.GetById(id, innerid);
        if (byId.Count == 0)
          return (PngChunk) null;
        if (byId.Count > 1 && (failIfMultiple || !byId[0].AllowsMultiple()))
          throw new PngjException("unexpected multiple chunks id=" + id);
        return byId[byId.Count - 1];
      }

      public List<PngChunk> GetEquivalent(PngChunk chunk)
      {
        return ChunkHelper.FilterList(this.chunks, (ChunkPredicate) new ChunkPredicateEquiv(chunk));
      }

      public override string ToString() => "ChunkList: read: " + (object) this.chunks.Count;

      public string ToStringFull()
      {
        StringBuilder stringBuilder = new StringBuilder(this.ToString());
        stringBuilder.Append("\n Read:\n");
        foreach (PngChunk chunk in this.chunks)
          stringBuilder.Append((object) chunk).Append($" G={(object) chunk.ChunkGroup}\n");
        return stringBuilder.ToString();
      }
    }
}
