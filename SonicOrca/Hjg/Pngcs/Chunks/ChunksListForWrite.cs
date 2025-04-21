// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.ChunksListForWrite
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Collections.Generic;
using System.IO;

namespace Hjg.Pngcs.Chunks
{

    public class ChunksListForWrite : ChunksList
    {
      private List<PngChunk> queuedChunks;
      private Dictionary<string, int> alreadyWrittenKeys;

      internal ChunksListForWrite(ImageInfo info)
        : base(info)
      {
        this.queuedChunks = new List<PngChunk>();
        this.alreadyWrittenKeys = new Dictionary<string, int>();
      }

      public List<PngChunk> GetQueuedById(string id) => this.GetQueuedById(id, (string) null);

      public List<PngChunk> GetQueuedById(string id, string innerid)
      {
        return ChunksList.GetXById(this.queuedChunks, id, innerid);
      }

      public PngChunk GetQueuedById1(string id, string innerid, bool failIfMultiple)
      {
        List<PngChunk> queuedById = this.GetQueuedById(id, innerid);
        if (queuedById.Count == 0)
          return (PngChunk) null;
        if (queuedById.Count > 1 && (failIfMultiple || !queuedById[0].AllowsMultiple()))
          throw new PngjException("unexpected multiple chunks id=" + id);
        return queuedById[queuedById.Count - 1];
      }

      public PngChunk GetQueuedById1(string id, bool failIfMultiple)
      {
        return this.GetQueuedById1(id, (string) null, failIfMultiple);
      }

      public PngChunk GetQueuedById1(string id) => this.GetQueuedById1(id, false);

      public bool RemoveChunk(PngChunk c) => this.queuedChunks.Remove(c);

      public bool Queue(PngChunk chunk)
      {
        this.queuedChunks.Add(chunk);
        return true;
      }

      private static bool shouldWrite(PngChunk c, int currentGroup)
      {
        if (currentGroup == 2)
          return c.Id.Equals("PLTE");
        if (currentGroup % 2 == 0)
          throw new PngjOutputException("bad chunk group?");
        int num1;
        int num2;
        if (c.mustGoBeforePLTE())
          num2 = num1 = 1;
        else if (c.mustGoBeforeIDAT())
        {
          num1 = 3;
          num2 = c.mustGoAfterPLTE() ? 3 : 1;
        }
        else
        {
          num1 = 5;
          num2 = 1;
        }
        int num3 = num1;
        if (c.Priority)
          num3 = num2;
        if (ChunkHelper.IsUnknown(c) && c.ChunkGroup > 0)
          num3 = c.ChunkGroup;
        return currentGroup == num3 || currentGroup > num3 && currentGroup <= num1;
      }

      internal int writeChunks(Stream os, int currentGroup)
      {
        List<int> intList = new List<int>();
        for (int index = 0; index < this.queuedChunks.Count; ++index)
        {
          PngChunk queuedChunk = this.queuedChunks[index];
          if (ChunksListForWrite.shouldWrite(queuedChunk, currentGroup))
          {
            if (ChunkHelper.IsCritical(queuedChunk.Id) && !queuedChunk.Id.Equals("PLTE"))
              throw new PngjOutputException("bad chunk queued: " + (object) queuedChunk);
            if (this.alreadyWrittenKeys.ContainsKey(queuedChunk.Id) && !queuedChunk.AllowsMultiple())
              throw new PngjOutputException("duplicated chunk does not allow multiple: " + (object) queuedChunk);
            queuedChunk.write(os);
            this.chunks.Add(queuedChunk);
            this.alreadyWrittenKeys[queuedChunk.Id] = this.alreadyWrittenKeys.ContainsKey(queuedChunk.Id) ? this.alreadyWrittenKeys[queuedChunk.Id] + 1 : 1;
            intList.Add(index);
            queuedChunk.ChunkGroup = currentGroup;
          }
        }
        for (int index = intList.Count - 1; index >= 0; --index)
          this.queuedChunks.RemoveAt(intList[index]);
        return intList.Count;
      }

      internal List<PngChunk> GetQueuedChunks() => this.queuedChunks;
    }
}
