// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.PngjBadCrcException
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace Hjg.Pngcs
{

    [Serializable]
    public class PngjBadCrcException : PngjException
    {
      private const long serialVersionUID = 1;

      public PngjBadCrcException(string message, Exception cause)
        : base(message, cause)
      {
      }

      public PngjBadCrcException(string message)
        : base(message)
      {
      }

      public PngjBadCrcException(Exception cause)
        : base(cause)
      {
      }
    }
}
