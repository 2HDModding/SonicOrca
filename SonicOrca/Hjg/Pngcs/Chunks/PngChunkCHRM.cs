// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunkCHRM
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public class PngChunkCHRM(ImageInfo info) : PngChunkSingle("cHRM", info)
    {
      public const string ID = "cHRM";
      private double whitex;
      private double whitey;
      private double redx;
      private double redy;
      private double greenx;
      private double greeny;
      private double bluex;
      private double bluey;

      public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
      {
        return PngChunk.ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
      }

      public override ChunkRaw CreateRawChunk()
      {
        ChunkRaw emptyChunk = this.createEmptyChunk(32 /*0x20*/, true);
        PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.whitex), emptyChunk.Data, 0);
        PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.whitey), emptyChunk.Data, 4);
        PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.redx), emptyChunk.Data, 8);
        PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.redy), emptyChunk.Data, 12);
        PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.greenx), emptyChunk.Data, 16 /*0x10*/);
        PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.greeny), emptyChunk.Data, 20);
        PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.bluex), emptyChunk.Data, 24);
        PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(this.bluey), emptyChunk.Data, 28);
        return emptyChunk;
      }

      public override void ParseFromRaw(ChunkRaw c)
      {
        this.whitex = c.Length == 32 /*0x20*/ ? PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 0)) : throw new PngjException("bad chunk " + (object) c);
        this.whitey = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 4));
        this.redx = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 8));
        this.redy = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 12));
        this.greenx = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 16 /*0x10*/));
        this.greeny = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 20));
        this.bluex = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 24));
        this.bluey = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 28));
      }

      public override void CloneDataFromRead(PngChunk other)
      {
        PngChunkCHRM pngChunkChrm = (PngChunkCHRM) other;
        this.whitex = pngChunkChrm.whitex;
        this.whitey = pngChunkChrm.whitex;
        this.redx = pngChunkChrm.redx;
        this.redy = pngChunkChrm.redy;
        this.greenx = pngChunkChrm.greenx;
        this.greeny = pngChunkChrm.greeny;
        this.bluex = pngChunkChrm.bluex;
        this.bluey = pngChunkChrm.bluey;
      }

      public void SetChromaticities(
        double whitex,
        double whitey,
        double redx,
        double redy,
        double greenx,
        double greeny,
        double bluex,
        double bluey)
      {
        this.whitex = whitex;
        this.redx = redx;
        this.greenx = greenx;
        this.bluex = bluex;
        this.whitey = whitey;
        this.redy = redy;
        this.greeny = greeny;
        this.bluey = bluey;
      }

      public double[] GetChromaticities()
      {
        return new double[8]
        {
          this.whitex,
          this.whitey,
          this.redx,
          this.redy,
          this.greenx,
          this.greeny,
          this.bluex,
          this.bluey
        };
      }
    }
}
