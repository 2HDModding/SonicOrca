// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ObjectDefinition
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core
{

    public class ObjectDefinition
    {
      private string _key;
      private Guid _uid;
      private string _name;
      private int _layer;
      private Vector2i _position;
      private readonly IReadOnlyCollection<KeyValuePair<string, object>> _behaviour;

      public string Key
      {
        get => this._key;
        set => this._key = value;
      }

      public Guid Uid
      {
        get => this._uid;
        set => this._uid = value;
      }

      public string Name
      {
        get => this._name;
        set => this._name = value;
      }

      public int Layer
      {
        get => this._layer;
        set => this._layer = value;
      }

      public Vector2i Position
      {
        get => this._position;
        set => this._position = value;
      }

      public IReadOnlyCollection<KeyValuePair<string, object>> Behaviour => this._behaviour;

      public ObjectDefinition(
        string key,
        Guid uid,
        string name,
        int layer,
        Vector2i position,
        IEnumerable<KeyValuePair<string, object>> behaviour)
      {
        this._key = key;
        this._uid = uid;
        this._name = name;
        this._layer = layer;
        this._position = position;
        this._behaviour = (IReadOnlyCollection<KeyValuePair<string, object>>) behaviour.ToArray<KeyValuePair<string, object>>();
      }
    }
}
