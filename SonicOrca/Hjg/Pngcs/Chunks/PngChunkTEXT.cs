// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkTEXT
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkTEXT(ImageInfo info) : PngChunkTextVar("tEXt", info)
    {
      public const string ID = "tEXt";

      public override ChunkRaw CreateRawChunk()
      {
        byte[] sourceArray = this.key.Length != 0 ? PngHelperInternal.charsetLatin1.GetBytes(this.key) : throw new PngjException("Text chunk key must be non empty");
        byte[] bytes = PngHelperInternal.charsetLatin1.GetBytes(this.val);
        ChunkRaw emptyChunk = this.createEmptyChunk(sourceArray.Length + bytes.Length + 1, true);
        Array.Copy((Array) sourceArray, 0, (Array) emptyChunk.Data, 0, sourceArray.Length);
        emptyChunk.Data[sourceArray.Length] = (byte) 0;
        Array.Copy((Array) bytes, 0, (Array) emptyChunk.Data, sourceArray.Length + 1, bytes.Length);
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw c)
      {
        int count = 0;
        while (count < c.Data.Length && c.Data[count] != (byte) 0)
          ++count;
        this.key = PngHelperInternal.charsetLatin1.GetString(c.Data, 0, count);
        int index = count + 1;
        this.val = index < c.Data.Length ? PngHelperInternal.charsetLatin1.GetString(c.Data, index, c.Data.Length - index) : "";
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkTEXT pngChunkText = (PngChunkTEXT) other;
        this.key = pngChunkText.key;
        this.val = pngChunkText.val;
      }
    }
}
