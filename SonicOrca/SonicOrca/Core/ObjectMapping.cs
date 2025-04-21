// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ObjectMapping
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Core
{

    public class ObjectMapping
    {
      private readonly string _field;
      private Guid _target;

      public string Field => this._field;

      public Guid Target
      {
        get => this._target;
        set => this._target = value;
      }

      public ObjectMapping(string field, Guid target)
      {
        this._field = field;
        this._target = target;
      }
    }
}
