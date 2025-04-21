// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.ChunkHelper
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Hjg.Pngcs.Zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hjg.Pngcs.Chunks
{

    public class ChunkHelper
    {
      internal const string IHDR = "IHDR";
      internal const string PLTE = "PLTE";
      internal const string IDAT = "IDAT";
      internal const string IEND = "IEND";
      internal const string cHRM = "cHRM";
      internal const string gAMA = "gAMA";
      internal const string iCCP = "iCCP";
      internal const string sBIT = "sBIT";
      internal const string sRGB = "sRGB";
      internal const string bKGD = "bKGD";
      internal const string hIST = "hIST";
      internal const string tRNS = "tRNS";
      internal const string pHYs = "pHYs";
      internal const string sPLT = "sPLT";
      internal const string tIME = "tIME";
      internal const string iTXt = "iTXt";
      internal const string tEXt = "tEXt";
      internal const string zTXt = "zTXt";
      internal static readonly byte[] b_IHDR = ChunkHelper.ToBytes(nameof (IHDR));
      internal static readonly byte[] b_PLTE = ChunkHelper.ToBytes(nameof (PLTE));
      internal static readonly byte[] b_IDAT = ChunkHelper.ToBytes(nameof (IDAT));
      internal static readonly byte[] b_IEND = ChunkHelper.ToBytes(nameof (IEND));

      public static byte[] ToBytes(string x) => PngHelperInternal.charsetLatin1.GetBytes(x);

      public static string ToString(byte[] x) => PngHelperInternal.charsetLatin1.GetString(x);

      public static string ToString(byte[] x, int offset, int len)
      {
        return PngHelperInternal.charsetLatin1.GetString(x, offset, len);
      }

      public static byte[] ToBytesUTF8(string x) => PngHelperInternal.charsetUtf8.GetBytes(x);

      public static string ToStringUTF8(byte[] x) => PngHelperInternal.charsetUtf8.GetString(x);

      public static string ToStringUTF8(byte[] x, int offset, int len)
      {
        return PngHelperInternal.charsetUtf8.GetString(x, offset, len);
      }

      public static void WriteBytesToStream(Stream stream, byte[] bytes)
      {
        stream.Write(bytes, 0, bytes.Length);
      }

      public static bool IsCritical(string id) => char.IsUpper(id[0]);

      public static bool IsPublic(string id) => char.IsUpper(id[1]);

      public static bool IsSafeToCopy(string id) => !char.IsUpper(id[3]);

      public static bool IsUnknown(PngChunk chunk) => chunk is PngChunkUNKNOWN;

      public static int PosNullByte(byte[] bytes)
      {
        for (int index = 0; index < bytes.Length; ++index)
        {
          if (bytes[index] == (byte) 0)
            return index;
        }
        return -1;
      }

      public static bool ShouldLoad(string id, ChunkLoadBehaviour behav)
      {
        if (ChunkHelper.IsCritical(id))
          return true;
        bool flag = PngChunk.isKnown(id);
        switch (behav)
        {
          case ChunkLoadBehaviour.LOAD_CHUNK_NEVER:
            return false;
          case ChunkLoadBehaviour.LOAD_CHUNK_KNOWN:
            return flag;
          case ChunkLoadBehaviour.LOAD_CHUNK_IF_SAFE:
            return flag || ChunkHelper.IsSafeToCopy(id);
          case ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS:
            return true;
          default:
            return false;
        }
      }

      internal static byte[] compressBytes(byte[] ori, bool compress)
      {
        return ChunkHelper.compressBytes(ori, 0, ori.Length, compress);
      }

      internal static byte[] compressBytes(byte[] ori, int offset, int len, bool compress)
      {
        try
        {
          MemoryStream st1 = new MemoryStream(ori, offset, len);
          Stream inx = (Stream) st1;
          if (!compress)
            inx = (Stream) ZlibStreamFactory.createZlibInputStream((Stream) st1);
          MemoryStream st2 = new MemoryStream();
          Stream outx = (Stream) st2;
          if (compress)
            outx = (Stream) ZlibStreamFactory.createZlibOutputStream((Stream) st2);
          ChunkHelper.shovelInToOut(inx, outx);
          inx.Close();
          outx.Close();
          return st2.ToArray();
        }
        catch (Exception ex)
        {
          throw new PngjException(ex);
        }
      }

      private static void shovelInToOut(Stream inx, Stream outx)
      {
        byte[] buffer = new byte[1024 /*0x0400*/];
        int count;
        while ((count = inx.Read(buffer, 0, 1024 /*0x0400*/)) > 0)
          outx.Write(buffer, 0, count);
      }

      internal static bool maskMatch(int v, int mask) => (v & mask) != 0;

      public static List<PngChunk> FilterList(List<PngChunk> list, ChunkPredicate predicateKeep)
      {
        List<PngChunk> pngChunkList = new List<PngChunk>();
        foreach (PngChunk chunk in list)
        {
          if (predicateKeep.Matches(chunk))
            pngChunkList.Add(chunk);
        }
        return pngChunkList;
      }

      public static int TrimList(List<PngChunk> list, ChunkPredicate predicateRemove)
      {
        int num = 0;
        for (int index = list.Count - 1; index >= 0; --index)
        {
          if (predicateRemove.Matches(list[index]))
          {
            list.RemoveAt(index);
            ++num;
          }
        }
        return num;
      }

      public static bool Equivalent(PngChunk c1, PngChunk c2)
      {
        if (c1 == c2)
          return true;
        if (c1 == null || c2 == null || !c1.Id.Equals(c2.Id) || c1.GetType() != c2.GetType())
          return false;
        if (!c2.AllowsMultiple())
          return true;
        switch (c1)
        {
          case PngChunkTextVar _:
            return ((PngChunkTextVar) c1).GetKey().Equals(((PngChunkTextVar) c2).GetKey());
          case PngChunkSPLT _:
            return ((PngChunkSPLT) c1).PalName.Equals(((PngChunkSPLT) c2).PalName);
          default:
            return false;
        }
      }

      public static bool IsText(PngChunk c) => c is PngChunkTextVar;
    }
}
