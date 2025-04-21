// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngMetadata
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Collections.Generic;

namespace Hjg.Pngcs.Chunks
{

    public class PngMetadata
    {
      private readonly ChunksList chunkList;
      private readonly bool ReadOnly;

      internal PngMetadata(ChunksList chunks)
      {
        this.chunkList = chunks;
        if (chunks is ChunksListForWrite)
          this.ReadOnly = false;
        else
          this.ReadOnly = true;
      }

      public void QueueChunk(PngChunk chunk, bool lazyOverwrite)
      {
        ChunksListForWrite chunkListW = this.getChunkListW();
        if (this.ReadOnly)
          throw new PngjException("cannot set chunk : readonly metadata");
        if (lazyOverwrite)
          ChunkHelper.TrimList(chunkListW.GetQueuedChunks(), (ChunkPredicate) new ChunkPredicateEquiv(chunk));
        chunkListW.Queue(chunk);
      }

      public void QueueChunk(PngChunk chunk) => this.QueueChunk(chunk, true);

      private ChunksListForWrite getChunkListW() => (ChunksListForWrite) this.chunkList;

      public double[] GetDpi()
      {
        PngChunk byId1 = this.chunkList.GetById1("pHYs", true);
        if (byId1 != null)
          return ((PngChunkPHYS) byId1).GetAsDpi2();
        return new double[2]{ -1.0, -1.0 };
      }

      public void SetDpi(double dpix, double dpiy)
      {
        PngChunkPHYS chunk = new PngChunkPHYS(this.chunkList.imageInfo);
        chunk.SetAsDpi2(dpix, dpiy);
        this.QueueChunk((PngChunk) chunk);
      }

      public void SetDpi(double dpi) => this.SetDpi(dpi, dpi);

      public PngChunkTIME SetTimeNow(int nsecs)
      {
        PngChunkTIME chunk = new PngChunkTIME(this.chunkList.imageInfo);
        chunk.SetNow(nsecs);
        this.QueueChunk((PngChunk) chunk);
        return chunk;
      }

      public PngChunkTIME SetTimeNow() => this.SetTimeNow(0);

      public PngChunkTIME SetTimeYMDHMS(int year, int mon, int day, int hour, int min, int sec)
      {
        PngChunkTIME chunk = new PngChunkTIME(this.chunkList.imageInfo);
        chunk.SetYMDHMS(year, mon, day, hour, min, sec);
        this.QueueChunk((PngChunk) chunk, true);
        return chunk;
      }

      public PngChunkTIME GetTime() => (PngChunkTIME) this.chunkList.GetById1("tIME");

      public string GetTimeAsString()
      {
        PngChunkTIME time = this.GetTime();
        return time != null ? time.GetAsString() : "";
      }

      public PngChunkTextVar SetText(string key, string val, bool useLatin1, bool compress)
      {
        if (compress && !useLatin1)
          throw new PngjException("cannot compress non latin text");
        PngChunkTextVar chunk;
        if (useLatin1)
        {
          chunk = !compress ? (PngChunkTextVar) new PngChunkTEXT(this.chunkList.imageInfo) : (PngChunkTextVar) new PngChunkZTXT(this.chunkList.imageInfo);
        }
        else
        {
          chunk = (PngChunkTextVar) new PngChunkITXT(this.chunkList.imageInfo);
          ((PngChunkITXT) chunk).SetLangtag(key);
        }
        chunk.SetKeyVal(key, val);
        this.QueueChunk((PngChunk) chunk, true);
        return chunk;
      }

      public PngChunkTextVar SetText(string key, string val) => this.SetText(key, val, false, false);

      public List<PngChunkTextVar> GetTxtsForKey(string key)
      {
        List<PngChunkTextVar> txtsForKey = new List<PngChunkTextVar>();
        foreach (PngChunk pngChunk in this.chunkList.GetById("tEXt", key))
          txtsForKey.Add((PngChunkTextVar) pngChunk);
        foreach (PngChunk pngChunk in this.chunkList.GetById("zTXt", key))
          txtsForKey.Add((PngChunkTextVar) pngChunk);
        foreach (PngChunk pngChunk in this.chunkList.GetById("iTXt", key))
          txtsForKey.Add((PngChunkTextVar) pngChunk);
        return txtsForKey;
      }

      public string GetTxtForKey(string key)
      {
        string txtForKey = "";
        List<PngChunkTextVar> txtsForKey = this.GetTxtsForKey(key);
        if (txtsForKey.Count == 0)
          return txtForKey;
        foreach (PngChunkTextVar pngChunkTextVar in txtsForKey)
          txtForKey = $"{txtForKey}{pngChunkTextVar.GetVal()}\n";
        return txtForKey.Trim();
      }

      public PngChunkPLTE GetPLTE() => (PngChunkPLTE) this.chunkList.GetById1("PLTE");

      public PngChunkPLTE CreatePLTEChunk()
      {
        PngChunkPLTE chunk = new PngChunkPLTE(this.chunkList.imageInfo);
        this.QueueChunk((PngChunk) chunk);
        return chunk;
      }

      public PngChunkTRNS GetTRNS() => (PngChunkTRNS) this.chunkList.GetById1("tRNS");

      public PngChunkTRNS CreateTRNSChunk()
      {
        PngChunkTRNS chunk = new PngChunkTRNS(this.chunkList.imageInfo);
        this.QueueChunk((PngChunk) chunk);
        return chunk;
      }
    }
}
