// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Zlib.ZlibStreamFactory
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.IO;

namespace Hjg.Pngcs.Zlib
{

    public class ZlibStreamFactory
    {
      public static AZlibInputStream createZlibInputStream(Stream st, bool leaveOpen)
      {
        return (AZlibInputStream) new ZlibInputStreamMs(st, leaveOpen);
      }

      public static AZlibInputStream createZlibInputStream(Stream st)
      {
        return ZlibStreamFactory.createZlibInputStream(st, false);
      }

      public static AZlibOutputStream createZlibOutputStream(
        Stream st,
        int compressLevel,
        EDeflateCompressStrategy strat,
        bool leaveOpen)
      {
        return (AZlibOutputStream) new ZlibOutputStreamMs(st, compressLevel, strat, leaveOpen);
      }

      public static AZlibOutputStream createZlibOutputStream(Stream st)
      {
        return ZlibStreamFactory.createZlibOutputStream(st, false);
      }

      public static AZlibOutputStream createZlibOutputStream(Stream st, bool leaveOpen)
      {
        return ZlibStreamFactory.createZlibOutputStream(st, 6, EDeflateCompressStrategy.Default, leaveOpen);
      }
    }
}
