// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkOFFS
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkOFFS(ImageInfo info) : PngChunkSingle("oFFs", info)
    {
      public const string ID = "oFFs";
      private long posX;
      private long posY;
      private int units;

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.BEFORE_IDAT;
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw emptyChunk = this.createEmptyChunk(9, true);
        PngHelperInternal.WriteInt4tobytes((int) this.posX, emptyChunk.Data, 0);
        PngHelperInternal.WriteInt4tobytes((int) this.posY, emptyChunk.Data, 4);
        emptyChunk.Data[8] = (byte) this.units;
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw chunk)
      {
        this.posX = chunk.Length == 9 ? (long) PngHelperInternal.ReadInt4fromBytes(chunk.Data, 0) : throw new PngjException("bad chunk length " + (object) chunk);
        if (this.posX < 0L)
          this.posX += 4294967296L /*0x0100000000*/;
        this.posY = (long) PngHelperInternal.ReadInt4fromBytes(chunk.Data, 4);
        if (this.posY < 0L)
          this.posY += 4294967296L /*0x0100000000*/;
        this.units = PngHelperInternal.ReadInt1fromByte(chunk.Data, 8);
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkOFFS pngChunkOffs = (PngChunkOFFS) other;
        this.posX = pngChunkOffs.posX;
        this.posY = pngChunkOffs.posY;
        this.units = pngChunkOffs.units;
      }

      public int GetUnits() => this.units;

      public void SetUnits(int units) => this.units = units;

      public long GetPosX() => this.posX;

      public void SetPosX(long posX) => this.posX = posX;

      public long GetPosY() => this.posY;

      public void SetPosY(long posY) => this.posY = posY;
    }
}
