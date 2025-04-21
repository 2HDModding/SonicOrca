// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ObjectEntry
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Extensions;
using SonicOrca.Core.Objects.Metadata;
using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

#nullable disable
namespace SonicOrca.Core;

public class ObjectEntry
{
  private readonly Level _level;
  private readonly ObjectType _type;
  private string _key;
  private ActiveObject _state;
  private Guid _uid;
  private string _name;
  private List<ObjectMapping> _mappings = new List<ObjectMapping>();

  public int Layer { get; set; }

  public Vector2i Position { get; set; }

  public bool FinishedForever { get; private set; }

  public ActiveObject Active { get; set; }

  public Level Level => this._level;

  public ObjectType Type => this._type;

  public ActiveObject State => this._state;

  public string Key
  {
    get => this._key;
    private set => this._key = value;
  }

  public string Name
  {
    get => this._name;
    set => this._name = value;
  }

  public Guid Uid
  {
    get => this._uid;
    set => this._uid = value;
  }

  public IList<ObjectMapping> Mappings => (IList<ObjectMapping>) this._mappings;

  public ObjectEntry(Level level, ObjectType type, LevelLayer layer, Vector2i position, Guid uid = default (Guid))
  {
    this._level = level;
    this.Layer = this.Level.Map.Layers.IndexOf(layer);
    this.Position = position;
    this._type = type;
    this._uid = uid;
    throw new NotImplementedException();
  }

  public ObjectEntry(Level level, ObjectPlacement placement)
  {
    if (placement.Entry.Count > 0)
      this.SetKeyValuePairReflection((IEnumerable<KeyValuePair<string, object>>) placement.Entry, (object) this);
    else
      this._uid = Guid.NewGuid();
    this._level = level;
    this._type = level.GameContext.ResourceTree.GetLoadedResource<ObjectType>(this._key);
    this.CreateState();
    if (placement.Behaviour.Count > 0)
      this.SetKeyValuePairReflection((IEnumerable<KeyValuePair<string, object>>) placement.Behaviour, (object) this._state);
    if (placement.Mappings.Count <= 0)
      return;
    foreach (KeyValuePair<string, object> mapping in (IEnumerable<KeyValuePair<string, object>>) placement.Mappings)
      this.Mappings.Add(new ObjectMapping(mapping.Key, Guid.Parse(mapping.Value.ToString())));
  }

  private void CreateState()
  {
    this._state = Activator.CreateInstance((ObjectInstanceAttribute.FromObject((object) this._type) ?? throw new Exception()).ObjectInstanceType) as ActiveObject;
  }

  private void RebindState()
  {
  }

  public void Finish()
  {
    if (this.Active == null)
      return;
    this.Active.Finish();
  }

  public void FinishForever() => this.FinishedForever = true;

  public ActiveObject CreateActiveObject()
  {
    this.Active = this.Type.CreateInstance();
    this.Active.Initialise(this);
    return this.Active;
  }

  public T CreateSubObject<T>() where T : ActiveObject
  {
    this.Active = (ActiveObject) Activator.CreateInstance<T>();
    this.Active.Initialise(this);
    return (T) this.Active;
  }

  public Rectanglei LifetimeArea
  {
    get
    {
      Vector2 lifeRadius = this._type.GetLifeRadius((IActiveObject) this._state);
      Vector2 vector2;
      ref Vector2 local = ref vector2;
      Vector2i position = this.Position;
      double x = (double) position.X - lifeRadius.X;
      position = this.Position;
      double y = (double) position.Y - lifeRadius.Y;
      local = new Vector2(x, y);
      return (Rectanglei) new Rectangle(vector2.X, vector2.Y, lifeRadius.X * 2.0, lifeRadius.Y * 2.0);
    }
  }

  private object SetKeyValuePairReflection(
    IEnumerable<KeyValuePair<string, object>> behaviour,
    object targetObject)
  {
    BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    foreach (KeyValuePair<string, object> keyValuePair in behaviour)
    {
      KeyValuePair<string, object> kvp = keyValuePair;
      if (kvp.Value is IEnumerable<KeyValuePair<string, object>>)
      {
        MemberInfo member = ((IEnumerable<MemberInfo>) targetObject.GetType().GetMembers(bindingAttr)).Select<MemberInfo, MemberInfo>((Func<MemberInfo, MemberInfo>) (m => m)).Where<MemberInfo>((Func<MemberInfo, bool>) (m => m.Name == kvp.Key)).FirstOrDefault<MemberInfo>();
        object targetObject1 = member != (MemberInfo) null ? member.GetUnderlyingValue(targetObject) : throw new Exception();
        if (!member.GetUnderlyingType().IsSubclassOf(typeof (ActiveObject)) && !(member.GetUnderlyingType() == typeof (IActiveObject)))
        {
          object obj = this.SetKeyValuePairReflection((IEnumerable<KeyValuePair<string, object>>) kvp.Value, targetObject1);
          member.SetUnderlyingValue(targetObject, obj);
        }
      }
      else
      {
        MemberInfo member = ((IEnumerable<MemberInfo>) targetObject.GetType().GetMembers(bindingAttr)).Select<MemberInfo, MemberInfo>((Func<MemberInfo, MemberInfo>) (m => m)).Where<MemberInfo>((Func<MemberInfo, bool>) (m => m.Name == kvp.Key)).FirstOrDefault<MemberInfo>();
        if (!(member != (MemberInfo) null))
          throw new Exception();
        string str = Convert.ToString(kvp.Value, (IFormatProvider) CultureInfo.InvariantCulture);
        member.SetUnderlyingValue(targetObject, LevelBindingResourceType.ParseBehaviourValue(str, member.GetUnderlyingType()));
      }
    }
    return targetObject;
  }

  public override string ToString() => this.Name;
}
