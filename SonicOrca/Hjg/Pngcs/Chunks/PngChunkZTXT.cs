// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkZTXT
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.IO;

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkZTXT(ImageInfo info) : PngChunkTextVar("zTXt", info)
    {
      public const string ID = "zTXt";

      public override ChunkRaw CreateRawChunk()
      {
        if (this.key.Length == 0)
          throw new PngjException("Text chunk key must be non empty");
        MemoryStream memoryStream = new MemoryStream();
        ChunkHelper.WriteBytesToStream((Stream) memoryStream, ChunkHelper.ToBytes(this.key));
        memoryStream.WriteByte((byte) 0);
        memoryStream.WriteByte((byte) 0);
        ChunkHelper.WriteBytesToStream((Stream) memoryStream, ChunkHelper.compressBytes(ChunkHelper.ToBytes(this.val), true));
        byte[] array = memoryStream.ToArray();
        ChunkRaw emptyChunk = this.createEmptyChunk(array.Length, false);
        emptyChunk.Data = array;
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw c)
      {
        int len = -1;
        for (int index = 0; index < c.Data.Length; ++index)
        {
          if (c.Data[index] == (byte) 0)
          {
            len = index;
            break;
          }
        }
        if (len < 0 || len > c.Data.Length - 2)
          throw new PngjException("bad zTXt chunk: no separator found");
        this.key = ChunkHelper.ToString(c.Data, 0, len);
        if (c.Data[len + 1] != (byte) 0)
          throw new PngjException("bad zTXt chunk: unknown compression method");
        this.val = ChunkHelper.ToString(ChunkHelper.compressBytes(c.Data, len + 2, c.Data.Length - len - 2, false));
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkZTXT pngChunkZtxt = (PngChunkZTXT) other;
        this.key = pngChunkZtxt.key;
        this.val = pngChunkZtxt.val;
      }
    }
}
