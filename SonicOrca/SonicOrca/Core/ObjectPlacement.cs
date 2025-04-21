// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ObjectPlacement
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Extensions;
using SonicOrca.Core.Objects.Metadata;
using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SonicOrca.Core
{

    public class ObjectPlacement
    {
      private string _key;
      private Guid _uid;
      private string _name;
      private int _layer;
      private Vector2i _position;
      private IReadOnlyCollection<KeyValuePair<string, object>> _entry = (IReadOnlyCollection<KeyValuePair<string, object>>) new List<KeyValuePair<string, object>>();
      private IReadOnlyCollection<KeyValuePair<string, object>> _behaviour = (IReadOnlyCollection<KeyValuePair<string, object>>) new List<KeyValuePair<string, object>>();
      private IReadOnlyCollection<KeyValuePair<string, object>> _mappings = (IReadOnlyCollection<KeyValuePair<string, object>>) new List<KeyValuePair<string, object>>();

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

      public IReadOnlyCollection<KeyValuePair<string, object>> Entry => this._entry;

      public IReadOnlyCollection<KeyValuePair<string, object>> Behaviour => this._behaviour;

      public IReadOnlyCollection<KeyValuePair<string, object>> Mappings
      {
        get => this._mappings;
        set => this._mappings = value;
      }

      public ObjectPlacement(string key, int layer, Vector2i position)
      {
        var entry = new
        {
          Key = key,
          Uid = Guid.NewGuid(),
          Name = "",
          Layer = layer,
          Position = new{ X = position.X, Y = position.Y }
        };
        var state = new{  };
        this._key = key;
        this._uid = entry.Uid;
        this._name = "";
        this._layer = layer;
        this._position = position;
        this.ParseProperties((object) entry, (object) state);
      }

      public ObjectPlacement(string key, int layer, Vector2i position, object state)
      {
        var entry = new
        {
          Key = key,
          Uid = Guid.NewGuid(),
          Name = "",
          Layer = layer,
          Position = new{ X = position.X, Y = position.Y }
        };
        this._key = key;
        this._uid = entry.Uid;
        this._name = "";
        this._layer = layer;
        this._position = position;
        this.ParseProperties((object) entry, state);
      }

      public ObjectPlacement(
        string key,
        Guid uid,
        string name,
        int layer,
        Vector2i position,
        object state)
      {
        var entry = new
        {
          Key = key,
          Uid = uid,
          Name = name,
          Layer = layer,
          Position = new{ X = position.X, Y = position.Y }
        };
        this._key = key;
        this._uid = uid;
        this._name = name;
        this._layer = layer;
        this._position = position;
        this.ParseProperties((object) entry, state);
      }

      private void ParseProperties(object entry, object state)
      {
        this._entry = (IReadOnlyCollection<KeyValuePair<string, object>>) ObjectPlacement.BehaviourToKeyPairs(entry).ToArray<KeyValuePair<string, object>>();
        if (state == null)
          throw new Exception();
        if (state.GetType().IsAnonymous() || state.GetType() == typeof (ExpandoObject))
          this._behaviour = (IReadOnlyCollection<KeyValuePair<string, object>>) ObjectPlacement.BehaviourToKeyPairs(state).ToArray<KeyValuePair<string, object>>();
        else
          this._behaviour = (IReadOnlyCollection<KeyValuePair<string, object>>) ObjectPlacement.StateToKeyPairs(state).ToArray<KeyValuePair<string, object>>();
      }

      public override string ToString()
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Key = {0} X = {1} Y = {2}", (object) this._key, (object) this._position.X, (object) this._position.Y);
        if (this._behaviour.Count > 0)
        {
          sb.Append(" Entry = ");
          ObjectPlacement.WriteBehaviourString(sb, (IEnumerable<KeyValuePair<string, object>>) this._entry);
          sb.Append(" Behaviour = ");
          ObjectPlacement.WriteBehaviourString(sb, (IEnumerable<KeyValuePair<string, object>>) this._behaviour);
        }
        return sb.ToString();
      }

      private static void WriteBehaviourString(
        StringBuilder sb,
        IEnumerable<KeyValuePair<string, object>> behaviourObject)
      {
        sb.Append("{ ");
        foreach (KeyValuePair<string, object> keyValuePair in behaviourObject)
        {
          sb.Append(keyValuePair.Key);
          sb.Append(" = ");
          if (keyValuePair.Value is IEnumerable<KeyValuePair<string, object>>)
            ObjectPlacement.WriteBehaviourString(sb, behaviourObject);
          else
            sb.Append(keyValuePair.Value);
          sb.Append(" ");
        }
        sb.Append("}");
      }

      private static IEnumerable<KeyValuePair<string, object>> StateToKeyPairs(object obj)
      {
        IEnumerable<Tuple<MemberInfo, StateVariableAttribute>> stateVariables = StateVariableAttribute.GetStateVariables(obj.GetType());
        object defaultState = (object) (Activator.CreateInstance(obj.GetType()) as ActiveObject);
        Type defaultType = defaultState.GetType();
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (Tuple<MemberInfo, StateVariableAttribute> tuple in stateVariables)
        {
          MemberInfo member1 = tuple.Item1;
          Type underlyingType = member1.GetUnderlyingType();
          string name = member1.Name;
          object stateValue = ObjectPlacement.ParseStateValue(member1.GetUnderlyingValue(obj), underlyingType);
          MemberInfo member2 = ((IEnumerable<MemberInfo>) defaultType.GetMember(member1.Name, bindingFlags)).First<MemberInfo>();
          member2.GetUnderlyingType();
          if (!ObjectPlacement.StateValueEquals(member1.GetUnderlyingValue(obj), member2.GetUnderlyingValue(defaultState), underlyingType))
            yield return new KeyValuePair<string, object>(name, stateValue);
        }
      }

      private static object ParseStateValue(object value, Type type)
      {
        if (value == null)
          return value;
        if (type.IsSubclassOf(typeof (ActiveObject)) || type == typeof (ActiveObject))
          return (object) new KeyValuePair<string, object>("Target", (object) (value as ActiveObject).Uid.ToString());
        if (type == typeof (Vector2))
        {
          List<KeyValuePair<string, object>> stateValue = new List<KeyValuePair<string, object>>();
          KeyValuePair<string, object> keyValuePair1 = new KeyValuePair<string, object>("X", (object) ((Vector2) value).X);
          KeyValuePair<string, object> keyValuePair2 = new KeyValuePair<string, object>("Y", (object) ((Vector2) value).Y);
          stateValue.Add(keyValuePair1);
          stateValue.Add(keyValuePair2);
          return (object) stateValue;
        }
        if (!(type == typeof (Vector2i)))
          return (object) value.ToString();
        List<KeyValuePair<string, object>> stateValue1 = new List<KeyValuePair<string, object>>();
        KeyValuePair<string, object> keyValuePair3 = new KeyValuePair<string, object>("X", (object) ((Vector2i) value).X);
        KeyValuePair<string, object> keyValuePair4 = new KeyValuePair<string, object>("Y", (object) ((Vector2i) value).Y);
        stateValue1.Add(keyValuePair3);
        stateValue1.Add(keyValuePair4);
        return (object) stateValue1;
      }

      private static bool StateValueEquals(object value1, object value2, Type type)
      {
        if (value1 == null || value2 == null)
          return value1 == null && value2 == null;
        if (type.IsSubclassOf(typeof (ActiveObject)) || type == typeof (ActiveObject))
          return (value1 as ActiveObject).Uid.ToString() == (value2 as ActiveObject).Uid.ToString();
        if (type == typeof (Vector2))
          return ((Vector2) value1).Equals((Vector2) value2);
        return type == typeof (Vector2i) ? ((Vector2i) value1).Equals((Vector2i) value2) : value1.ToString() == value2.ToString();
      }

      private static IEnumerable<KeyValuePair<string, object>> BehaviourToKeyPairs(object behaviour)
      {
        if (behaviour.GetType() == typeof (ExpandoObject))
          return (IEnumerable<KeyValuePair<string, object>>) (behaviour as IDictionary<string, object>);
        Dictionary<string, object> keyPairs = new Dictionary<string, object>();
        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(behaviour))
        {
          object obj = property.GetValue(behaviour);
          keyPairs.Add(property.Name, obj);
        }
        return (IEnumerable<KeyValuePair<string, object>>) keyPairs;
      }
    }
}
