// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.PngChunk
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Hjg.Pngcs.Chunks
{

    public abstract class PngChunk
    {
      public readonly string Id;
      public readonly bool Crit;
      public readonly bool Pub;
      public readonly bool Safe;
      protected readonly ImageInfo ImgInfo;
      private static Dictionary<string, Type> factoryMap = PngChunk.initFactory();

      public bool Priority { get; set; }

      public int ChunkGroup { get; set; }

      public int Length { get; set; }

      public long Offset { get; set; }

      protected PngChunk(string id, ImageInfo imgInfo)
      {
        this.Id = id;
        this.ImgInfo = imgInfo;
        this.Crit = ChunkHelper.IsCritical(id);
        this.Pub = ChunkHelper.IsPublic(id);
        this.Safe = ChunkHelper.IsSafeToCopy(id);
        this.Priority = false;
        this.ChunkGroup = -1;
        this.Length = -1;
        this.Offset = 0L;
      }

      private static Dictionary<string, Type> initFactory()
      {
        return new Dictionary<string, Type>()
        {
          {
            "IDAT",
            typeof (PngChunkIDAT)
          },
          {
            "IHDR",
            typeof (PngChunkIHDR)
          },
          {
            "PLTE",
            typeof (PngChunkPLTE)
          },
          {
            "IEND",
            typeof (PngChunkIEND)
          },
          {
            "tEXt",
            typeof (PngChunkTEXT)
          },
          {
            "iTXt",
            typeof (PngChunkITXT)
          },
          {
            "zTXt",
            typeof (PngChunkZTXT)
          },
          {
            "bKGD",
            typeof (PngChunkBKGD)
          },
          {
            "gAMA",
            typeof (PngChunkGAMA)
          },
          {
            "pHYs",
            typeof (PngChunkPHYS)
          },
          {
            "iCCP",
            typeof (PngChunkICCP)
          },
          {
            "tIME",
            typeof (PngChunkTIME)
          },
          {
            "tRNS",
            typeof (PngChunkTRNS)
          },
          {
            "cHRM",
            typeof (PngChunkCHRM)
          },
          {
            "sBIT",
            typeof (PngChunkSBIT)
          },
          {
            "sRGB",
            typeof (PngChunkSRGB)
          },
          {
            "hIST",
            typeof (PngChunkHIST)
          },
          {
            "sPLT",
            typeof (PngChunkSPLT)
          },
          {
            "oFFs",
            typeof (PngChunkOFFS)
          },
          {
            "sTER",
            typeof (PngChunkSTER)
          }
        };
      }

      public static void FactoryRegister(string chunkId, Type type)
      {
        PngChunk.factoryMap.Add(chunkId, type);
      }

      internal static bool isKnown(string id) => PngChunk.factoryMap.ContainsKey(id);

      internal bool mustGoBeforePLTE()
      {
        return this.GetOrderingConstraint() == PngChunk.ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
      }

      internal bool mustGoBeforeIDAT()
      {
        PngChunk.ChunkOrderingConstraint orderingConstraint = this.GetOrderingConstraint();
        switch (orderingConstraint)
        {
          case PngChunk.ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT:
          case PngChunk.ChunkOrderingConstraint.BEFORE_IDAT:
            return true;
          default:
            return orderingConstraint == PngChunk.ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
        }
      }

      internal bool mustGoAfterPLTE()
      {
        return this.GetOrderingConstraint() == PngChunk.ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
      }

      internal static PngChunk Factory(ChunkRaw chunk, ImageInfo info)
      {
        PngChunk pngChunk = PngChunk.FactoryFromId(ChunkHelper.ToString(chunk.IdBytes), info);
        pngChunk.Length = chunk.Length;
        pngChunk.ParseFromRaw(chunk);
        return pngChunk;
      }

      internal static PngChunk FactoryFromId(string cid, ImageInfo info)
      {
        PngChunk pngChunk = (PngChunk) null;
        if (PngChunk.factoryMap == null)
          PngChunk.initFactory();
        if (PngChunk.isKnown(cid))
        {
          Type factory = PngChunk.factoryMap[cid];
          if (factory == (Type) null)
            Console.Error.WriteLine("What?? " + cid);
          pngChunk = (PngChunk) factory.GetConstructor(new Type[1]
          {
            typeof (ImageInfo)
          }).Invoke(new object[1]{ (object) info });
        }
        if (pngChunk == null)
          pngChunk = (PngChunk) new PngChunkUNKNOWN(cid, info);
        return pngChunk;
      }

      public ChunkRaw createEmptyChunk(int len, bool alloc)
      {
        return new ChunkRaw(len, ChunkHelper.ToBytes(this.Id), alloc);
      }

      public static T CloneChunk<T>(T chunk, ImageInfo info) where T : PngChunk
      {
        PngChunk pngChunk = PngChunk.FactoryFromId(chunk.Id, info);
        if ((object) pngChunk.GetType() != (object) chunk.GetType())
          throw new PngjException($"bad class cloning chunk: {(object) pngChunk.GetType()} {(object) chunk.GetType()}");
        pngChunk.CloneDataFromRead((PngChunk) chunk);
        return (T) pngChunk;
      }

      internal void write(Stream os)
      {
        (this.CreateRawChunk() ?? throw new PngjException("null chunk ! creation failed for " + (object) this)).WriteChunk(os);
      }

      public override string ToString()
      {
        return $"chunk id= {this.Id} (len={(object) this.Length} off={(object) this.Offset}) c={this.GetType().Name}";
      }

      public abstract ChunkRaw CreateRawChunk();

      public abstract void ParseFromRaw(ChunkRaw c);

      public abstract void CloneDataFromRead(PngChunk other);

      public abstract bool AllowsMultiple();

      public abstract PngChunk.ChunkOrderingConstraint GetOrderingConstraint();

      public enum ChunkOrderingConstraint
      {
        NONE,
        BEFORE_PLTE_AND_IDAT,
        AFTER_PLTE_BEFORE_IDAT,
        BEFORE_IDAT,
        NA,
      }
    }
}
