// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkITXT
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.IO;

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkITXT(ImageInfo info) : PngChunkTextVar("iTXt", info)
    {
      public const string ID = "iTXt";
      private bool compressed;
      private string langTag = "";
      private string translatedTag = "";

      public override ChunkRaw CreateRawChunk()
      {
        if (this.key.Length == 0)
          throw new PngjException("Text chunk key must be non empty");
        MemoryStream memoryStream = new MemoryStream();
        ChunkHelper.WriteBytesToStream((Stream) memoryStream, ChunkHelper.ToBytes(this.key));
        memoryStream.WriteByte((byte) 0);
        memoryStream.WriteByte(this.compressed ? (byte) 1 : (byte) 0);
        memoryStream.WriteByte((byte) 0);
        ChunkHelper.WriteBytesToStream((Stream) memoryStream, ChunkHelper.ToBytes(this.langTag));
        memoryStream.WriteByte((byte) 0);
        ChunkHelper.WriteBytesToStream((Stream) memoryStream, ChunkHelper.ToBytesUTF8(this.translatedTag));
        memoryStream.WriteByte((byte) 0);
        byte[] numArray = ChunkHelper.ToBytesUTF8(this.val);
        if (this.compressed)
          numArray = ChunkHelper.compressBytes(numArray, true);
        ChunkHelper.WriteBytesToStream((Stream) memoryStream, numArray);
        byte[] array = memoryStream.ToArray();
        ChunkRaw emptyChunk = this.createEmptyChunk(array.Length, false);
        emptyChunk.Data = array;
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw c)
      {
        int index1 = 0;
        int[] numArray = new int[3];
        for (int index2 = 0; index2 < c.Data.Length; ++index2)
        {
          if (c.Data[index2] == (byte) 0)
          {
            numArray[index1] = index2;
            ++index1;
            if (index1 == 1)
              index2 += 2;
            if (index1 == 3)
              break;
          }
        }
        if (index1 != 3)
          throw new PngjException("Bad formed PngChunkITXT chunk");
        this.key = ChunkHelper.ToString(c.Data, 0, numArray[0]);
        int index3 = numArray[0] + 1;
        this.compressed = c.Data[index3] != (byte) 0;
        int offset1 = index3 + 1;
        if (this.compressed && c.Data[offset1] != (byte) 0)
          throw new PngjException("Bad formed PngChunkITXT chunk - bad compression method ");
        this.langTag = ChunkHelper.ToString(c.Data, offset1, numArray[1] - offset1);
        this.translatedTag = ChunkHelper.ToStringUTF8(c.Data, numArray[1] + 1, numArray[2] - numArray[1] - 1);
        int offset2 = numArray[2] + 1;
        if (this.compressed)
          this.val = ChunkHelper.ToStringUTF8(ChunkHelper.compressBytes(c.Data, offset2, c.Data.Length - offset2, false));
        else
          this.val = ChunkHelper.ToStringUTF8(c.Data, offset2, c.Data.Length - offset2);
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkITXT pngChunkItxt = (PngChunkITXT) other;
        this.key = pngChunkItxt.key;
        this.val = pngChunkItxt.val;
        this.compressed = pngChunkItxt.compressed;
        this.langTag = pngChunkItxt.langTag;
        this.translatedTag = pngChunkItxt.translatedTag;
      }

      public bool IsCompressed() => this.compressed;

      public void SetCompressed(bool compressed) => this.compressed = compressed;

      public string GetLangtag() => this.langTag;

      public void SetLangtag(string langtag) => this.langTag = langtag;

      public string GetTranslatedTag() => this.translatedTag;

      public void SetTranslatedTag(string translatedTag) => this.translatedTag = translatedTag;
    }
}
