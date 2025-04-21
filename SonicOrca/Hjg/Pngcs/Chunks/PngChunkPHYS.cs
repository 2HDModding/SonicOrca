// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkPHYS
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkPHYS(ImageInfo info) : PngChunkSingle("pHYs", info)
    {
      public const string ID = "pHYs";

      public long PixelsxUnitX { get; set; }

      public long PixelsxUnitY { get; set; }

      public int Units { get; set; }

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.BEFORE_IDAT;
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw emptyChunk = this.createEmptyChunk(9, true);
        PngHelperInternal.WriteInt4tobytes((int) this.PixelsxUnitX, emptyChunk.Data, 0);
        PngHelperInternal.WriteInt4tobytes((int) this.PixelsxUnitY, emptyChunk.Data, 4);
        emptyChunk.Data[8] = (byte) this.Units;
        return emptyChunk;
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkPHYS pngChunkPhys = (PngChunkPHYS) other;
        this.PixelsxUnitX = pngChunkPhys.PixelsxUnitX;
        this.PixelsxUnitY = pngChunkPhys.PixelsxUnitY;
        this.Units = pngChunkPhys.Units;
      }

      public override void ParseFromRaw(ChunkRaw chunk)
      {
        this.PixelsxUnitX = chunk.Length == 9 ? (long) PngHelperInternal.ReadInt4fromBytes(chunk.Data, 0) : throw new PngjException("bad chunk length " + (object) chunk);
        if (this.PixelsxUnitX < 0L)
          this.PixelsxUnitX += 4294967296L /*0x0100000000*/;
        this.PixelsxUnitY = (long) PngHelperInternal.ReadInt4fromBytes(chunk.Data, 4);
        if (this.PixelsxUnitY < 0L)
          this.PixelsxUnitY += 4294967296L /*0x0100000000*/;
        this.Units = PngHelperInternal.ReadInt1fromByte(chunk.Data, 8);
      }

      public double GetAsDpi()
      {
        return this.Units != 1 || this.PixelsxUnitX != this.PixelsxUnitY ? -1.0 : (double) this.PixelsxUnitX * 0.0254;
      }

      public double[] GetAsDpi2()
      {
        return this.Units != 1 ? new double[2]{ -1.0, -1.0 } : new double[2]
        {
          (double) this.PixelsxUnitX * 0.0254,
          (double) this.PixelsxUnitY * 0.0254
        };
      }

      public void SetAsDpi(double dpi)
      {
        this.Units = 1;
        this.PixelsxUnitX = (long) (dpi / 0.0254 + 0.5);
        this.PixelsxUnitY = this.PixelsxUnitX;
      }

      public void SetAsDpi2(double dpix, double dpiy)
      {
        this.Units = 1;
        this.PixelsxUnitX = (long) (dpix / 0.0254 + 0.5);
        this.PixelsxUnitY = (long) (dpiy / 0.0254 + 0.5);
      }
    }
}
