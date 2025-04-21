// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.Chunks.ChunkCopyBehaviour
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace Hjg.Pngcs.Chunks
{

    public class ChunkCopyBehaviour
    {
      public static readonly int COPY_NONE = 0;
      public static readonly int COPY_PALETTE = 1;
      public static readonly int COPY_ALL_SAFE = 4;
      public static readonly int COPY_ALL = 8;
      public static readonly int COPY_PHYS = 16 /*0x10*/;
      public static readonly int COPY_TEXTUAL = 32 /*0x20*/;
      public static readonly int COPY_TRANSPARENCY = 64 /*0x40*/;
      public static readonly int COPY_UNKNOWN = 128 /*0x80*/;
      public static readonly int COPY_ALMOSTALL = 256 /*0x0100*/;
    }
}
