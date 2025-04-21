// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.PngReader
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

    public class PngReader
    {
      protected readonly string filename;
      private Dictionary<string, int> skipChunkIdsSet;
      private readonly PngMetadata metadata;
      private readonly ChunksList chunksList;
      protected ImageLine imgLine;
      protected byte[] rowb;
      protected byte[] rowbprev;
      protected byte[] rowbfilter;
      public readonly bool interlaced;
      private readonly PngDeinterlacer deinterlacer;
      private bool crcEnabled = true;
      private bool unpackedMode;
      protected int rowNum = -1;
      private long offset;
      private int bytesChunksLoaded;
      private readonly Stream inputStream;
      internal AZlibInputStream idatIstream;
      internal PngIDatChunkInputStream iIdatCstream;
      protected Adler32 crctest;

      public ImageInfo ImgInfo { get; private set; }

      public ChunkLoadBehaviour ChunkLoadBehaviour { get; set; }

      public bool ShouldCloseStream { get; set; }

      public long MaxBytesMetadata { get; set; }

      public long MaxTotalBytesRead { get; set; }

      public int SkipChunkMaxSize { get; set; }

      public string[] SkipChunkIds { get; set; }

      public int CurrentChunkGroup { get; private set; }

      public PngReader(Stream inputStream)
        : this(inputStream, "[NO FILENAME AVAILABLE]")
      {
      }

      public PngReader(Stream inputStream, string filename)
      {
        this.filename = filename == null ? "" : filename;
        this.inputStream = inputStream;
        this.chunksList = new ChunksList((ImageInfo) null);
        this.metadata = new PngMetadata(this.chunksList);
        this.offset = 0L;
        this.CurrentChunkGroup = -1;
        this.ShouldCloseStream = true;
        this.MaxBytesMetadata = 5242880L /*0x500000*/;
        this.MaxTotalBytesRead = 209715200L /*0x0C800000*/;
        this.SkipChunkMaxSize = 2097152 /*0x200000*/;
        this.SkipChunkIds = new string[1]{ "fdAT" };
        this.ChunkLoadBehaviour = ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS;
        byte[] numArray1 = new byte[8];
        PngHelperInternal.ReadBytes(inputStream, numArray1, 0, numArray1.Length);
        this.offset += (long) numArray1.Length;
        if (!PngCsUtils.arraysEqual(numArray1, PngHelperInternal.PNG_ID_SIGNATURE))
          throw new PngjInputException("Bad PNG signature");
        this.CurrentChunkGroup = 0;
        int clen = PngHelperInternal.ReadInt4(inputStream);
        this.offset += 4L;
        if (clen != 13)
          throw new Exception("IDHR chunk len != 13 ?? " + (object) clen);
        byte[] numArray2 = new byte[4];
        PngHelperInternal.ReadBytes(inputStream, numArray2, 0, 4);
        if (!PngCsUtils.arraysEqual4(numArray2, ChunkHelper.b_IHDR))
          throw new PngjInputException($"IHDR not found as first chunk??? [{ChunkHelper.ToString(numArray2)}]");
        this.offset += 4L;
        PngChunkIHDR pngChunkIhdr = (PngChunkIHDR) this.ReadChunk(numArray2, clen, false);
        bool alpha = (pngChunkIhdr.Colormodel & 4) != 0;
        bool palette = (pngChunkIhdr.Colormodel & 1) != 0;
        bool grayscale = pngChunkIhdr.Colormodel == 0 || pngChunkIhdr.Colormodel == 4;
        this.ImgInfo = new ImageInfo(pngChunkIhdr.Cols, pngChunkIhdr.Rows, pngChunkIhdr.Bitspc, alpha, grayscale, palette);
        this.rowb = new byte[this.ImgInfo.BytesPerRow + 1];
        this.rowbprev = new byte[this.rowb.Length];
        this.rowbfilter = new byte[this.rowb.Length];
        this.interlaced = pngChunkIhdr.Interlaced == 1;
        this.deinterlacer = this.interlaced ? new PngDeinterlacer(this.ImgInfo) : (PngDeinterlacer) null;
        if (pngChunkIhdr.Filmeth != 0 || pngChunkIhdr.Compmeth != 0 || (pngChunkIhdr.Interlaced & 65534) != 0)
          throw new PngjInputException("compmethod or filtermethod or interlaced unrecognized");
        if (pngChunkIhdr.Colormodel < 0 || pngChunkIhdr.Colormodel > 6 || pngChunkIhdr.Colormodel == 1 || pngChunkIhdr.Colormodel == 5)
          throw new PngjInputException("Invalid colormodel " + (object) pngChunkIhdr.Colormodel);
        if (pngChunkIhdr.Bitspc != 1 && pngChunkIhdr.Bitspc != 2 && pngChunkIhdr.Bitspc != 4 && pngChunkIhdr.Bitspc != 8 && pngChunkIhdr.Bitspc != 16 /*0x10*/)
          throw new PngjInputException("Invalid bit depth " + (object) pngChunkIhdr.Bitspc);
      }

      private bool FirstChunksNotYetRead() => this.CurrentChunkGroup < 1;

      private void ReadLastAndClose()
      {
        if (this.CurrentChunkGroup < 5)
        {
          try
          {
            this.idatIstream.Close();
          }
          catch (Exception ex)
          {
          }
          this.ReadLastChunks();
        }
        this.Close();
      }

      private void Close()
      {
        if (this.CurrentChunkGroup < 6)
        {
          try
          {
            this.idatIstream.Close();
          }
          catch (Exception ex)
          {
          }
          this.CurrentChunkGroup = 6;
        }
        if (!this.ShouldCloseStream)
          return;
        this.inputStream.Close();
      }

      private void UnfilterRow(int nbytes)
      {
        int num = (int) this.rowbfilter[0];
        switch (num)
        {
          case 0:
            this.UnfilterRowNone(nbytes);
            break;
          case 1:
            this.UnfilterRowSub(nbytes);
            break;
          case 2:
            this.UnfilterRowUp(nbytes);
            break;
          case 3:
            this.UnfilterRowAverage(nbytes);
            break;
          case 4:
            this.UnfilterRowPaeth(nbytes);
            break;
          default:
            throw new PngjInputException($"Filter type {(object) num} not implemented");
        }
        if (this.crctest == null)
          return;
        this.crctest.Update(this.rowb, 1, nbytes);
      }

      private void UnfilterRowAverage(int nbytes)
      {
        int index1 = 1 - this.ImgInfo.BytesPixel;
        int index2 = 1;
        while (index2 <= nbytes)
        {
          int num = index1 > 0 ? (int) this.rowb[index1] : 0;
          this.rowb[index2] = (byte) ((uint) this.rowbfilter[index2] + (uint) ((num + ((int) this.rowbprev[index2] & (int) byte.MaxValue)) / 2));
          ++index2;
          ++index1;
        }
      }

      private void UnfilterRowNone(int nbytes)
      {
        for (int index = 1; index <= nbytes; ++index)
          this.rowb[index] = this.rowbfilter[index];
      }

      private void UnfilterRowPaeth(int nbytes)
      {
        int index1 = 1 - this.ImgInfo.BytesPixel;
        int index2 = 1;
        while (index2 <= nbytes)
        {
          int a = index1 > 0 ? (int) this.rowb[index1] : 0;
          int c = index1 > 0 ? (int) this.rowbprev[index1] : 0;
          this.rowb[index2] = (byte) ((uint) this.rowbfilter[index2] + (uint) PngHelperInternal.FilterPaethPredictor(a, (int) this.rowbprev[index2], c));
          ++index2;
          ++index1;
        }
      }

      private void UnfilterRowSub(int nbytes)
      {
        for (int index = 1; index <= this.ImgInfo.BytesPixel; ++index)
          this.rowb[index] = this.rowbfilter[index];
        int index1 = 1;
        int index2 = this.ImgInfo.BytesPixel + 1;
        while (index2 <= nbytes)
        {
          this.rowb[index2] = (byte) ((uint) this.rowbfilter[index2] + (uint) this.rowb[index1]);
          ++index2;
          ++index1;
        }
      }

      private void UnfilterRowUp(int nbytes)
      {
        for (int index = 1; index <= nbytes; ++index)
          this.rowb[index] = (byte) ((uint) this.rowbfilter[index] + (uint) this.rowbprev[index]);
      }

      private void ReadFirstChunks()
      {
        if (!this.FirstChunksNotYetRead())
          return;
        int num = 0;
        bool flag = false;
        byte[] numArray = new byte[4];
        this.CurrentChunkGroup = 1;
        while (!flag)
        {
          num = PngHelperInternal.ReadInt4(this.inputStream);
          this.offset += 4L;
          if (num >= 0)
          {
            PngHelperInternal.ReadBytes(this.inputStream, numArray, 0, 4);
            this.offset += 4L;
            if (PngCsUtils.arraysEqual4(numArray, ChunkHelper.b_IDAT))
            {
              flag = true;
              this.CurrentChunkGroup = 4;
              this.chunksList.AppendReadChunk((PngChunk) new PngChunkIDAT(this.ImgInfo, num, this.offset - 8L), this.CurrentChunkGroup);
              break;
            }
            string str = !PngCsUtils.arraysEqual4(numArray, ChunkHelper.b_IEND) ? ChunkHelper.ToString(numArray) : throw new PngjInputException("END chunk found before image data (IDAT) at offset=" + (object) this.offset);
            if (str.Equals("PLTE"))
              this.CurrentChunkGroup = 2;
            this.ReadChunk(numArray, num, false);
            if (str.Equals("PLTE"))
              this.CurrentChunkGroup = 3;
          }
          else
            break;
        }
        int lenFirstChunk = flag ? num : -1;
        if (lenFirstChunk < 0)
          throw new PngjInputException("first idat chunk not found!");
        this.iIdatCstream = new PngIDatChunkInputStream(this.inputStream, lenFirstChunk, this.offset);
        this.idatIstream = ZlibStreamFactory.createZlibInputStream((Stream) this.iIdatCstream, true);
        if (this.crcEnabled)
          return;
        this.iIdatCstream.DisableCrcCheck();
      }

      private void ReadLastChunks()
      {
        this.CurrentChunkGroup = 5;
        if (!this.iIdatCstream.IsEnded())
          this.iIdatCstream.ForceChunkEnd();
        int clen = this.iIdatCstream.GetLenLastChunk();
        byte[] idLastChunk = this.iIdatCstream.GetIdLastChunk();
        bool flag1 = false;
        bool flag2 = true;
        while (!flag1)
        {
          bool skipforced = false;
          if (!flag2)
          {
            clen = PngHelperInternal.ReadInt4(this.inputStream);
            this.offset += 4L;
            if (clen < 0)
              throw new PngjInputException("bad len " + (object) clen);
            PngHelperInternal.ReadBytes(this.inputStream, idLastChunk, 0, 4);
            this.offset += 4L;
          }
          flag2 = false;
          if (PngCsUtils.arraysEqual4(idLastChunk, ChunkHelper.b_IDAT))
            skipforced = true;
          else if (PngCsUtils.arraysEqual4(idLastChunk, ChunkHelper.b_IEND))
          {
            this.CurrentChunkGroup = 6;
            flag1 = true;
          }
          this.ReadChunk(idLastChunk, clen, skipforced);
        }
        if (!flag1)
          throw new PngjInputException("end chunk not found - offset=" + (object) this.offset);
      }

      private PngChunk ReadChunk(byte[] chunkid, int clen, bool skipforced)
      {
        if (clen < 0)
          throw new PngjInputException("invalid chunk lenght: " + (object) clen);
        if (this.skipChunkIdsSet == null && this.CurrentChunkGroup > 0)
        {
          this.skipChunkIdsSet = new Dictionary<string, int>();
          if (this.SkipChunkIds != null)
          {
            foreach (string skipChunkId in this.SkipChunkIds)
              this.skipChunkIdsSet.Add(skipChunkId, 1);
          }
        }
        string str = ChunkHelper.ToString(chunkid);
        bool flag1 = ChunkHelper.IsCritical(str);
        bool flag2 = skipforced;
        if (this.MaxTotalBytesRead > 0L && (long) clen + this.offset > this.MaxTotalBytesRead)
          throw new PngjInputException($"Maximum total bytes to read exceeeded: {(object) this.MaxTotalBytesRead} offset:{(object) this.offset} clen={(object) clen}");
        if (this.CurrentChunkGroup > 0 && !ChunkHelper.IsCritical(str))
          flag2 = flag2 || this.SkipChunkMaxSize > 0 && clen >= this.SkipChunkMaxSize || this.skipChunkIdsSet.ContainsKey(str) || this.MaxBytesMetadata > 0L && (long) clen > this.MaxBytesMetadata - (long) this.bytesChunksLoaded || !ChunkHelper.ShouldLoad(str, this.ChunkLoadBehaviour);
        PngChunk chunk1;
        if (flag2)
        {
          PngHelperInternal.SkipBytes(this.inputStream, clen);
          PngHelperInternal.ReadInt4(this.inputStream);
          chunk1 = (PngChunk) new PngChunkSkipped(str, this.ImgInfo, clen);
        }
        else
        {
          ChunkRaw chunk2 = new ChunkRaw(clen, chunkid, true);
          chunk2.ReadChunkData(this.inputStream, this.crcEnabled | flag1);
          chunk1 = PngChunk.Factory(chunk2, this.ImgInfo);
          if (!chunk1.Crit)
            this.bytesChunksLoaded += chunk2.Length;
        }
        chunk1.Offset = this.offset - 8L;
        this.chunksList.AppendReadChunk(chunk1, this.CurrentChunkGroup);
        this.offset += (long) clen + 4L;
        return chunk1;
      }

      internal void logWarn(string warn) => Console.Error.WriteLine(warn);

      public ChunksList GetChunksList()
      {
        if (this.FirstChunksNotYetRead())
          this.ReadFirstChunks();
        return this.chunksList;
      }

      public PngMetadata GetMetadata()
      {
        if (this.FirstChunksNotYetRead())
          this.ReadFirstChunks();
        return this.metadata;
      }

      public ImageLine ReadRow(int nrow)
      {
        return this.imgLine != null && this.imgLine.SampleType == ImageLine.ESampleType.BYTE ? this.ReadRowByte(nrow) : this.ReadRowInt(nrow);
      }

      public ImageLine ReadRowInt(int nrow)
      {
        if (this.imgLine == null)
          this.imgLine = new ImageLine(this.ImgInfo, ImageLine.ESampleType.INT, this.unpackedMode);
        if (this.imgLine.Rown == nrow)
          return this.imgLine;
        this.ReadRowInt(this.imgLine.Scanline, nrow);
        this.imgLine.FilterUsed = (FilterType) this.rowbfilter[0];
        this.imgLine.Rown = nrow;
        return this.imgLine;
      }

      public ImageLine ReadRowByte(int nrow)
      {
        if (this.imgLine == null)
          this.imgLine = new ImageLine(this.ImgInfo, ImageLine.ESampleType.BYTE, this.unpackedMode);
        if (this.imgLine.Rown == nrow)
          return this.imgLine;
        this.ReadRowByte(this.imgLine.ScanlineB, nrow);
        this.imgLine.FilterUsed = (FilterType) this.rowbfilter[0];
        this.imgLine.Rown = nrow;
        return this.imgLine;
      }

      public int[] ReadRow(int[] buffer, int nrow) => this.ReadRowInt(buffer, nrow);

      public int[] ReadRowInt(int[] buffer, int nrow)
      {
        if (buffer == null)
          buffer = new int[this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked];
        if (!this.interlaced)
        {
          if (nrow <= this.rowNum)
            throw new PngjInputException("rows must be read in increasing order: " + (object) nrow);
          int bytesRead = 0;
          while (this.rowNum < nrow)
            bytesRead = this.ReadRowRaw(this.rowNum + 1);
          this.decodeLastReadRowToInt(buffer, bytesRead);
        }
        else
        {
          if (this.deinterlacer.getImageInt() == null)
            this.deinterlacer.setImageInt(this.ReadRowsInt().Scanlines);
          Array.Copy((Array) this.deinterlacer.getImageInt()[nrow], 0, (Array) buffer, 0, this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked);
        }
        return buffer;
      }

      public byte[] ReadRowByte(byte[] buffer, int nrow)
      {
        if (buffer == null)
          buffer = new byte[this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked];
        if (!this.interlaced)
        {
          if (nrow <= this.rowNum)
            throw new PngjInputException("rows must be read in increasing order: " + (object) nrow);
          int bytesRead = 0;
          while (this.rowNum < nrow)
            bytesRead = this.ReadRowRaw(this.rowNum + 1);
          this.decodeLastReadRowToByte(buffer, bytesRead);
        }
        else
        {
          if (this.deinterlacer.getImageByte() == null)
            this.deinterlacer.setImageByte(this.ReadRowsByte().ScanlinesB);
          Array.Copy((Array) this.deinterlacer.getImageByte()[nrow], 0, (Array) buffer, 0, this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked);
        }
        return buffer;
      }

      [Obsolete("GetRow is deprecated,  use ReadRow/ReadRowInt/ReadRowByte instead.")]
      public ImageLine GetRow(int nrow) => this.ReadRow(nrow);

      private void decodeLastReadRowToInt(int[] buffer, int bytesRead)
      {
        if (this.ImgInfo.BitDepth <= 8)
        {
          int index = 0;
          int num = 1;
          for (; index < bytesRead; ++index)
            buffer[index] = (int) this.rowb[num++];
        }
        else
        {
          int num1 = 0;
          int num2 = 1;
          while (num2 < bytesRead)
          {
            int[] numArray = buffer;
            int index1 = num1;
            byte[] rowb1 = this.rowb;
            int index2 = num2;
            int num3 = index2 + 1;
            int num4 = (int) rowb1[index2] << 8;
            byte[] rowb2 = this.rowb;
            int index3 = num3;
            num2 = index3 + 1;
            int num5 = (int) rowb2[index3];
            int num6 = num4 + num5;
            numArray[index1] = num6;
            ++num1;
          }
        }
        if (!this.ImgInfo.Packed || !this.unpackedMode)
          return;
        ImageLine.unpackInplaceInt(this.ImgInfo, buffer, buffer, false);
      }

      private void decodeLastReadRowToByte(byte[] buffer, int bytesRead)
      {
        if (this.ImgInfo.BitDepth <= 8)
        {
          Array.Copy((Array) this.rowb, 1, (Array) buffer, 0, bytesRead);
        }
        else
        {
          int index1 = 0;
          for (int index2 = 1; index2 < bytesRead; index2 += 2)
          {
            buffer[index1] = this.rowb[index2];
            ++index1;
          }
        }
        if (!this.ImgInfo.Packed || !this.unpackedMode)
          return;
        ImageLine.unpackInplaceByte(this.ImgInfo, buffer, buffer, false);
      }

      public ImageLines ReadRowsInt(int rowOffset, int nRows, int rowStep)
      {
        if (nRows < 0)
          nRows = (this.ImgInfo.Rows - rowOffset) / rowStep;
        if (rowStep < 1 || rowOffset < 0 || nRows * rowStep + rowOffset > this.ImgInfo.Rows)
          throw new PngjInputException("bad args");
        ImageLines imageLines = new ImageLines(this.ImgInfo, ImageLine.ESampleType.INT, this.unpackedMode, rowOffset, nRows, rowStep);
        if (!this.interlaced)
        {
          for (int index = 0; index < this.ImgInfo.Rows; ++index)
          {
            int bytesRead = this.ReadRowRaw(index);
            int matrixRowStrict = imageLines.ImageRowToMatrixRowStrict(index);
            if (matrixRowStrict >= 0)
              this.decodeLastReadRowToInt(imageLines.Scanlines[matrixRowStrict], bytesRead);
          }
        }
        else
        {
          int[] numArray = new int[this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked];
          for (int p = 1; p <= 7; ++p)
          {
            this.deinterlacer.setPass(p);
            for (int nrow = 0; nrow < this.deinterlacer.getRows(); ++nrow)
            {
              int bytesRead = this.ReadRowRaw(nrow);
              int currRowReal = this.deinterlacer.getCurrRowReal();
              int matrixRowStrict = imageLines.ImageRowToMatrixRowStrict(currRowReal);
              if (matrixRowStrict >= 0)
              {
                this.decodeLastReadRowToInt(numArray, bytesRead);
                this.deinterlacer.deinterlaceInt(numArray, imageLines.Scanlines[matrixRowStrict], !this.unpackedMode);
              }
            }
          }
        }
        this.End();
        return imageLines;
      }

      public ImageLines ReadRowsInt() => this.ReadRowsInt(0, this.ImgInfo.Rows, 1);

      public ImageLines ReadRowsByte(int rowOffset, int nRows, int rowStep)
      {
        if (nRows < 0)
          nRows = (this.ImgInfo.Rows - rowOffset) / rowStep;
        if (rowStep < 1 || rowOffset < 0 || nRows * rowStep + rowOffset > this.ImgInfo.Rows)
          throw new PngjInputException("bad args");
        ImageLines imageLines = new ImageLines(this.ImgInfo, ImageLine.ESampleType.BYTE, this.unpackedMode, rowOffset, nRows, rowStep);
        if (!this.interlaced)
        {
          for (int index = 0; index < this.ImgInfo.Rows; ++index)
          {
            int bytesRead = this.ReadRowRaw(index);
            int matrixRowStrict = imageLines.ImageRowToMatrixRowStrict(index);
            if (matrixRowStrict >= 0)
              this.decodeLastReadRowToByte(imageLines.ScanlinesB[matrixRowStrict], bytesRead);
          }
        }
        else
        {
          byte[] numArray = new byte[this.unpackedMode ? this.ImgInfo.SamplesPerRow : this.ImgInfo.SamplesPerRowPacked];
          for (int p = 1; p <= 7; ++p)
          {
            this.deinterlacer.setPass(p);
            for (int nrow = 0; nrow < this.deinterlacer.getRows(); ++nrow)
            {
              int bytesRead = this.ReadRowRaw(nrow);
              int currRowReal = this.deinterlacer.getCurrRowReal();
              int matrixRowStrict = imageLines.ImageRowToMatrixRowStrict(currRowReal);
              if (matrixRowStrict >= 0)
              {
                this.decodeLastReadRowToByte(numArray, bytesRead);
                this.deinterlacer.deinterlaceByte(numArray, imageLines.ScanlinesB[matrixRowStrict], !this.unpackedMode);
              }
            }
          }
        }
        this.End();
        return imageLines;
      }

      public ImageLines ReadRowsByte() => this.ReadRowsByte(0, this.ImgInfo.Rows, 1);

      private int ReadRowRaw(int nrow)
      {
        if (nrow == 0 && this.FirstChunksNotYetRead())
          this.ReadFirstChunks();
        if (nrow == 0 && this.interlaced)
          Array.Clear((Array) this.rowb, 0, this.rowb.Length);
        int nbytes = this.ImgInfo.BytesPerRow;
        if (this.interlaced)
        {
          if (nrow < 0 || nrow > this.deinterlacer.getRows() || nrow != 0 && nrow != this.deinterlacer.getCurrRowSubimg() + 1)
            throw new PngjInputException("invalid row in interlaced mode: " + (object) nrow);
          this.deinterlacer.setRow(nrow);
          nbytes = (this.ImgInfo.BitspPixel * this.deinterlacer.getPixelsToRead() + 7) / 8;
          if (nbytes < 1)
            throw new PngjExceptionInternal("wtf??");
        }
        else if (nrow < 0 || nrow >= this.ImgInfo.Rows || nrow != this.rowNum + 1)
          throw new PngjInputException("invalid row: " + (object) nrow);
        this.rowNum = nrow;
        byte[] rowb = this.rowb;
        this.rowb = this.rowbprev;
        this.rowbprev = rowb;
        PngHelperInternal.ReadBytes((Stream) this.idatIstream, this.rowbfilter, 0, nbytes + 1);
        this.offset = this.iIdatCstream.GetOffset();
        if (this.offset < 0L)
          throw new PngjExceptionInternal("bad offset ??" + (object) this.offset);
        if (this.MaxTotalBytesRead > 0L && this.offset >= this.MaxTotalBytesRead)
          throw new PngjInputException($"Reading IDAT: Maximum total bytes to read exceeeded: {(object) this.MaxTotalBytesRead} offset:{(object) this.offset}");
        this.rowb[0] = (byte) 0;
        this.UnfilterRow(nbytes);
        this.rowb[0] = this.rowbfilter[0];
        if (this.rowNum == this.ImgInfo.Rows - 1 && !this.interlaced || this.interlaced && this.deinterlacer.isAtLastRow())
          this.ReadLastAndClose();
        return nbytes;
      }

      public void ReadSkippingAllRows()
      {
        if (this.FirstChunksNotYetRead())
          this.ReadFirstChunks();
        this.iIdatCstream.DisableCrcCheck();
        try
        {
          do
            ;
          while (this.iIdatCstream.Read(this.rowbfilter, 0, this.rowbfilter.Length) >= 0);
        }
        catch (IOException ex)
        {
          throw new PngjInputException("error in raw read of IDAT", (Exception) ex);
        }
        this.offset = this.iIdatCstream.GetOffset();
        if (this.offset < 0L)
          throw new PngjExceptionInternal("bad offset ??" + (object) this.offset);
        if (this.MaxTotalBytesRead > 0L && this.offset >= this.MaxTotalBytesRead)
          throw new PngjInputException($"Reading IDAT: Maximum total bytes to read exceeeded: {(object) this.MaxTotalBytesRead} offset:{(object) this.offset}");
        this.ReadLastAndClose();
      }

      public override string ToString() => $"filename={this.filename} {this.ImgInfo.ToString()}";

      public void End()
      {
        if (this.CurrentChunkGroup >= 6)
          return;
        this.Close();
      }

      public bool IsInterlaced() => this.interlaced;

      public void SetUnpackedMode(bool unPackedMode) => this.unpackedMode = unPackedMode;

      public bool IsUnpackedMode() => this.unpackedMode;

      public void SetCrcCheckDisabled() => this.crcEnabled = false;

      internal long GetCrctestVal() => (long) this.crctest.GetValue();

      internal void InitCrctest() => this.crctest = new Adler32();
    }
}
