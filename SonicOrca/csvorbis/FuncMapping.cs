// Decompiled with JetBrains decompiler
// Type: csvorbis.FuncMapping
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using csogg;

namespace csvorbis
{

    internal abstract class FuncMapping
    {
      public static FuncMapping[] mapping_P = new FuncMapping[1]
      {
        (FuncMapping) new Mapping0()
      };

      public abstract void pack(Info info, object imap, csBuffer buffer);

      public abstract object unpack(Info info, csBuffer buffer);

      public abstract object look(DspState vd, InfoMode vm, object m);

      public abstract void free_info(object imap);

      public abstract void free_look(object imap);

      public abstract int inverse(Block vd, object lm);
    }
}
