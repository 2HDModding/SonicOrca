// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ObjectEditorProperty
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Core
{

    public class ObjectEditorProperty
    {
      private readonly string _name;
      private readonly string _key;
      private readonly Type _type;
      private readonly object _defaultValue;
      private readonly string _description;

      public string Name => this._name;

      public string Key => this._key;

      public Type Type => this._type;

      public object DefaultValue => this._defaultValue;

      public string Description => this._description;

      public ObjectEditorProperty(
        string name,
        string key,
        Type type,
        object defaultValue,
        string description = null)
      {
        this._name = name;
        this._key = key;
        this._type = type;
        this._defaultValue = defaultValue;
        this._description = description;
      }

      public virtual bool Validate(ref object value) => true;

      public override string ToString() => $"{this._name} [{this._key} : {this._type}]";
    }
}
