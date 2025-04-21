// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.PngWriter
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Hjg.Pngcs.Chunks;
using Hjg.Pngcs.Zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hjg.Pngcs
{

    public class PngWriter
    {
      public readonly ImageInfo ImgInfo;
      protected readonly string filename;
      private FilterWriteStrategy filterStrat;
      private readonly PngMetadata metadata;
      private readonly ChunksListForWrite chunksList;
      protected byte[] rowb;
      protected byte[] rowbprev;
      protected byte[] rowbfilter;
      private int rowNum = -1;
      private readonly Stream outputStream;
      private PngIDatChunkOutputStream datStream;
      private AZlibOutputStream datStreamDeflated;
      private int[] histox = new int[256 /*0x0100*/];
      private bool unpackedMode;
      private bool needsPack;

      public EDeflateCompressStrategy CompressionStrategy { get; set; }

      public int CompLevel { get; set; }

      public bool ShouldCloseStream { get; set; }

      public int IdatMaxSize { get; set; }

      public int CurrentChunkGroup { get; private set; }

      public PngWriter(Stream outputStream, ImageInfo imgInfo)
        : this(outputStream, imgInfo, "[NO FILENAME AVAILABLE]")
      {
      }

      public PngWriter(Stream outputStream, ImageInfo imgInfo, string filename)
      {
        this.filename = filename == null ? "" : filename;
        this.outputStream = outputStream;
        this.ImgInfo = imgInfo;
        this.CompLevel = 6;
        this.ShouldCloseStream = true;
        this.IdatMaxSize = 0;
        this.CompressionStrategy = EDeflateCompressStrategy.Filtered;
        this.rowb = new byte[imgInfo.BytesPerRow + 1];
        this.rowbprev = new byte[this.rowb.Length];
        this.rowbfilter = new byte[this.rowb.Length];
        this.chunksList = new ChunksListForWrite(this.ImgInfo);
        this.metadata = new PngMetadata((ChunksList) this.chunksList);
        this.filterStrat = new FilterWriteStrategy(this.ImgInfo, FilterType.FILTER_DEFAULT);
        this.unpackedMode = false;
        this.needsPack = this.unpackedMode && imgInfo.Packed;
      }

      private void init()
      {
        this.datStream = new PngIDatChunkOutputStream(this.outputStream, this.IdatMaxSize);
        this.datStreamDeflated = ZlibStreamFactory.createZlibOutputStream((Stream) this.datStream, this.CompLevel, this.CompressionStrategy, true);
        this.WriteSignatureAndIHDR();
        this.WriteFirstChunks();
      }

      private void reportResultsForFilter(int rown, FilterType type, bool tentative)
      {
        for (int index = 0; index < this.histox.Length; ++index)
          this.histox[index] = 0;
        int sum = 0;
        for (int index = 1; index <= this.ImgInfo.BytesPerRow; ++index)
        {
          int num = (int) this.rowbfilter[index];
          if (num < 0)
            sum -= num;
          else
            sum += num;
          ++this.histox[num & (int) byte.MaxValue];
        }
        this.filterStrat.fillResultsForFilter(rown, type, (double) sum, this.histox, tentative);
      }

      private void WriteEndChunk()
      {
        new PngChunkIEND(this.ImgInfo).CreateRawChunk().WriteChunk(this.outputStream);
      }

      private void WriteFirstChunks()
      {
        this.CurrentChunkGroup = 1;
        this.chunksList.writeChunks(this.outputStream, this.CurrentChunkGroup);
        this.CurrentChunkGroup = 2;
        int num = this.chunksList.writeChunks(this.outputStream, this.CurrentChunkGroup);
        if (num > 0 && this.ImgInfo.Greyscale)
          throw new PngjOutputException("cannot write palette for this format");
        if (num == 0 && this.ImgInfo.Indexed)
          throw new PngjOutputException("missing palette");
        this.CurrentChunkGroup = 3;
        this.chunksList.writeChunks(this.outputStream, this.CurrentChunkGroup);
        this.CurrentChunkGroup = 4;
      }

      private void WriteLastChunks()
      {
        this.CurrentChunkGroup = 5;
        this.chunksList.writeChunks(this.outputStream, this.CurrentChunkGroup);
        List<PngChunk> queuedChunks = this.chunksList.GetQueuedChunks();
        if (queuedChunks.Count > 0)
          throw new PngjOutputException($"{(object) queuedChunks.Count} chunks were not written! Eg: {queuedChunks[0].ToString()}");
        this.CurrentChunkGroup = 6;
      }

      private void WriteSignatureAndIHDR()
      {
        this.CurrentChunkGroup = 0;
        PngHelperInternal.WriteBytes(this.outputStream, PngHelperInternal.PNG_ID_SIGNATURE);
        PngChunkIHDR pngChunkIhdr = new PngChunkIHDR(this.ImgInfo);
        pngChunkIhdr.Cols = this.ImgInfo.Cols;
        pngChunkIhdr.Rows = this.ImgInfo.Rows;
        pngChunkIhdr.Bitspc = this.ImgInfo.BitDepth;
        int num = 0;
        if (this.ImgInfo.Alpha)
          num += 4;
        if (this.ImgInfo.Indexed)
          ++num;
        if (!this.ImgInfo.Greyscale)
          num += 2;
        pngChunkIhdr.Colormodel = num;
        pngChunkIhdr.Compmeth = 0;
        pngChunkIhdr.Filmeth = 0;
        pngChunkIhdr.Interlaced = 0;
        pngChunkIhdr.CreateRawChunk().WriteChunk(this.outputStream);
      }

      protected void encodeRowFromByte(byte[] row)
      {
        if (row.Length == this.ImgInfo.SamplesPerRowPacked && !this.needsPack)
        {
          int index = 1;
          if (this.ImgInfo.BitDepth <= 8)
          {
            foreach (byte num in row)
              this.rowb[index++] = num;
          }
          else
          {
            foreach (byte num in row)
            {
              this.rowb[index] = num;
              index += 2;
            }
          }
        }
        else
        {
          if (row.Length >= this.ImgInfo.SamplesPerRow && this.needsPack)
            ImageLine.packInplaceByte(this.ImgInfo, row, row, false);
          if (this.ImgInfo.BitDepth <= 8)
          {
            int index = 0;
            int num = 1;
            for (; index < this.ImgInfo.SamplesPerRowPacked; ++index)
              this.rowb[num++] = row[index];
          }
          else
          {
            int index1 = 0;
            int num1 = 1;
            for (; index1 < this.ImgInfo.SamplesPerRowPacked; ++index1)
            {
              byte[] rowb1 = this.rowb;
              int index2 = num1;
              int num2 = index2 + 1;
              int num3 = (int) row[index1];
              rowb1[index2] = (byte) num3;
              byte[] rowb2 = this.rowb;
              int index3 = num2;
              num1 = index3 + 1;
              rowb2[index3] = (byte) 0;
            }
          }
        }
      }

      protected void encodeRowFromInt(int[] row)
      {
        if (row.Length == this.ImgInfo.SamplesPerRowPacked && !this.needsPack)
        {
          int num1 = 1;
          if (this.ImgInfo.BitDepth <= 8)
          {
            foreach (int num2 in row)
              this.rowb[num1++] = (byte) num2;
          }
          else
          {
            foreach (int num3 in row)
            {
              byte[] rowb1 = this.rowb;
              int index1 = num1;
              int num4 = index1 + 1;
              int num5 = (int) (byte) (num3 >> 8);
              rowb1[index1] = (byte) num5;
              byte[] rowb2 = this.rowb;
              int index2 = num4;
              num1 = index2 + 1;
              int num6 = (int) (byte) num3;
              rowb2[index2] = (byte) num6;
            }
          }
        }
        else
        {
          if (row.Length >= this.ImgInfo.SamplesPerRow && this.needsPack)
            ImageLine.packInplaceInt(this.ImgInfo, row, row, false);
          if (this.ImgInfo.BitDepth <= 8)
          {
            int index = 0;
            int num = 1;
            for (; index < this.ImgInfo.SamplesPerRowPacked; ++index)
              this.rowb[num++] = (byte) row[index];
          }
          else
          {
            int index3 = 0;
            int num7 = 1;
            for (; index3 < this.ImgInfo.SamplesPerRowPacked; ++index3)
            {
              byte[] rowb3 = this.rowb;
              int index4 = num7;
              int num8 = index4 + 1;
              int num9 = (int) (byte) (row[index3] >> 8);
              rowb3[index4] = (byte) num9;
              byte[] rowb4 = this.rowb;
              int index5 = num8;
              num7 = index5 + 1;
              int num10 = (int) (byte) row[index3];
              rowb4[index5] = (byte) num10;
            }
          }
        }
      }

      private void FilterRow(int rown)
      {
        if (this.filterStrat.shouldTestAll(rown))
        {
          this.FilterRowNone();
          this.reportResultsForFilter(rown, FilterType.FILTER_NONE, true);
          this.FilterRowSub();
          this.reportResultsForFilter(rown, FilterType.FILTER_SUB, true);
          this.FilterRowUp();
          this.reportResultsForFilter(rown, FilterType.FILTER_UP, true);
          this.FilterRowAverage();
          this.reportResultsForFilter(rown, FilterType.FILTER_AVERAGE, true);
          this.FilterRowPaeth();
          this.reportResultsForFilter(rown, FilterType.FILTER_PAETH, true);
        }
        FilterType type = this.filterStrat.gimmeFilterType(rown, true);
        this.rowbfilter[0] = (byte) type;
        switch (type)
        {
          case FilterType.FILTER_NONE:
            this.FilterRowNone();
            break;
          case FilterType.FILTER_SUB:
            this.FilterRowSub();
            break;
          case FilterType.FILTER_UP:
            this.FilterRowUp();
            break;
          case FilterType.FILTER_AVERAGE:
            this.FilterRowAverage();
            break;
          case FilterType.FILTER_PAETH:
            this.FilterRowPaeth();
            break;
          default:
            throw new PngjOutputException($"Filter type {(object) type} not implemented");
        }
        this.reportResultsForFilter(rown, type, false);
      }

      private void prepareEncodeRow(int rown)
      {
        if (this.datStream == null)
          this.init();
        ++this.rowNum;
        if (rown >= 0 && this.rowNum != rown)
          throw new PngjOutputException($"rows must be written in order: expected:{(object) this.rowNum} passed:{(object) rown}");
        byte[] rowb = this.rowb;
        this.rowb = this.rowbprev;
        this.rowbprev = rowb;
      }

      private void filterAndSend(int rown)
      {
        this.FilterRow(rown);
        this.datStreamDeflated.Write(this.rowbfilter, 0, this.ImgInfo.BytesPerRow + 1);
      }

      private void FilterRowAverage()
      {
        int bytesPerRow = this.ImgInfo.BytesPerRow;
        int index1 = 1 - this.ImgInfo.BytesPixel;
        int index2 = 1;
        while (index2 <= bytesPerRow)
        {
          this.rowbfilter[index2] = (byte) ((int) this.rowb[index2] - ((int) this.rowbprev[index2] + (index1 > 0 ? (int) this.rowb[index1] : 0)) / 2);
          ++index2;
          ++index1;
        }
      }

      private void FilterRowNone()
      {
        for (int index = 1; index <= this.ImgInfo.BytesPerRow; ++index)
          this.rowbfilter[index] = this.rowb[index];
      }

      private void FilterRowPaeth()
      {
        int bytesPerRow = this.ImgInfo.BytesPerRow;
        int index1 = 1 - this.ImgInfo.BytesPixel;
        int index2 = 1;
        while (index2 <= bytesPerRow)
        {
          this.rowbfilter[index2] = (byte) ((int) this.rowb[index2] - PngHelperInternal.FilterPaethPredictor(index1 > 0 ? (int) this.rowb[index1] : 0, (int) this.rowbprev[index2], index1 > 0 ? (int) this.rowbprev[index1] : 0));
          ++index2;
          ++index1;
        }
      }

      private void FilterRowSub()
      {
        for (int index = 1; index <= this.ImgInfo.BytesPixel; ++index)
          this.rowbfilter[index] = this.rowb[index];
        int index1 = 1;
        int index2 = this.ImgInfo.BytesPixel + 1;
        while (index2 <= this.ImgInfo.BytesPerRow)
        {
          this.rowbfilter[index2] = (byte) ((uint) this.rowb[index2] - (uint) this.rowb[index1]);
          ++index2;
          ++index1;
        }
      }

      private void FilterRowUp()
      {
        for (int index = 1; index <= this.ImgInfo.BytesPerRow; ++index)
          this.rowbfilter[index] = (byte) ((uint) this.rowb[index] - (uint) this.rowbprev[index]);
      }

      private long SumRowbfilter()
      {
        long num = 0;
        for (int index = 1; index <= this.ImgInfo.BytesPerRow; ++index)
        {
          if (this.rowbfilter[index] < (byte) 0)
            num -= (long) this.rowbfilter[index];
          else
            num += (long) this.rowbfilter[index];
        }
        return num;
      }

      private void CopyChunks(PngReader reader, int copy_mask, bool onlyAfterIdat)
      {
        bool flag1 = this.CurrentChunkGroup >= 4;
        if (onlyAfterIdat && reader.CurrentChunkGroup < 6)
          throw new PngjException("tried to copy last chunks but reader has not ended");
        foreach (PngChunk chunk in reader.GetChunksList().GetChunks())
        {
          if (!(chunk.ChunkGroup < 4 & flag1))
          {
            bool flag2 = false;
            if (chunk.Crit)
            {
              if (chunk.Id.Equals("PLTE"))
              {
                if (this.ImgInfo.Indexed && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_PALETTE))
                  flag2 = true;
                if (!this.ImgInfo.Greyscale && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALL))
                  flag2 = true;
              }
            }
            else
            {
              bool flag3 = chunk is PngChunkTextVar;
              int num = chunk.Safe ? 1 : 0;
              if (ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALL))
                flag2 = true;
              if (num != 0 && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALL_SAFE))
                flag2 = true;
              if (chunk.Id.Equals("tRNS") && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_TRANSPARENCY))
                flag2 = true;
              if (chunk.Id.Equals("pHYs") && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_PHYS))
                flag2 = true;
              if (flag3 && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_TEXTUAL))
                flag2 = true;
              if (ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALMOSTALL) && !(ChunkHelper.IsUnknown(chunk) | flag3) && !chunk.Id.Equals("hIST") && !chunk.Id.Equals("tIME"))
                flag2 = true;
              if (chunk is PngChunkSkipped)
                flag2 = false;
            }
            if (flag2)
              this.chunksList.Queue(PngChunk.CloneChunk<PngChunk>(chunk, this.ImgInfo));
          }
        }
      }

      public void CopyChunksFirst(PngReader reader, int copy_mask)
      {
        this.CopyChunks(reader, copy_mask, false);
      }

      public void CopyChunksLast(PngReader reader, int copy_mask)
      {
        this.CopyChunks(reader, copy_mask, true);
      }

      public double ComputeCompressionRatio()
      {
        if (this.CurrentChunkGroup < 6)
          throw new PngjException("must be called after End()");
        return (double) this.datStream.GetCountFlushed() / (double) ((this.ImgInfo.BytesPerRow + 1) * this.ImgInfo.Rows);
      }

      public void End()
      {
        if (this.rowNum != this.ImgInfo.Rows - 1)
          throw new PngjOutputException("all rows have not been written");
        try
        {
          this.datStreamDeflated.Close();
          this.datStream.Close();
          this.WriteLastChunks();
          this.WriteEndChunk();
          if (!this.ShouldCloseStream)
            return;
          this.outputStream.Close();
        }
        catch (IOException ex)
        {
          throw new PngjOutputException((Exception) ex);
        }
      }

      public string GetFilename() => this.filename;

      public void WriteRow(ImageLine imgline, int rownumber)
      {
        this.SetUseUnPackedMode(imgline.SamplesUnpacked);
        if (imgline.SampleType == ImageLine.ESampleType.INT)
          this.WriteRowInt(imgline.Scanline, rownumber);
        else
          this.WriteRowByte(imgline.ScanlineB, rownumber);
      }

      public void WriteRow(int[] newrow) => this.WriteRow(newrow, -1);

      public void WriteRow(int[] newrow, int rown) => this.WriteRowInt(newrow, rown);

      public void WriteRowInt(int[] newrow, int rown)
      {
        this.prepareEncodeRow(rown);
        this.encodeRowFromInt(newrow);
        this.filterAndSend(rown);
      }

      public void WriteRowByte(byte[] newrow, int rown)
      {
        this.prepareEncodeRow(rown);
        this.encodeRowFromByte(newrow);
        this.filterAndSend(rown);
      }

      public void WriteRowsInt(int[][] image)
      {
        for (int rown = 0; rown < this.ImgInfo.Rows; ++rown)
          this.WriteRowInt(image[rown], rown);
      }

      public void WriteRowsByte(byte[][] image)
      {
        for (int rown = 0; rown < this.ImgInfo.Rows; ++rown)
          this.WriteRowByte(image[rown], rown);
      }

      public PngMetadata GetMetadata() => this.metadata;

      public ChunksListForWrite GetChunksList() => this.chunksList;

      public void SetFilterType(FilterType filterType)
      {
        this.filterStrat = new FilterWriteStrategy(this.ImgInfo, filterType);
      }

      public bool IsUnpackedMode() => this.unpackedMode;

      public void SetUseUnPackedMode(bool useUnpackedMode)
      {
        this.unpackedMode = useUnpackedMode;
        this.needsPack = this.unpackedMode && this.ImgInfo.Packed;
      }
    }
}
