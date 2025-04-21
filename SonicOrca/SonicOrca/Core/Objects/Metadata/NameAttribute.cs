// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Metadata.NameAttribute
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Core.Objects.Metadata
{

    [AttributeUsage(AttributeTargets.Class)]
    public class NameAttribute : Attribute
    {
      private readonly string _name;

      public string Name => this._name;

      public NameAttribute(string name) => this._name = name;

      public static NameAttribute FromObject(object obj)
      {
        return AttributeHelpers.GetAttribute<NameAttribute>((object) obj.GetType());
      }
    }
}
