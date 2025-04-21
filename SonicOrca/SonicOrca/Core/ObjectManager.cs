// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ObjectManager
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using SonicOrca.Core.Extensions;
using SonicOrca.Core.Objects;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SonicOrca.Core
{

    public class ObjectManager
    {
      private readonly List<ObjectType> _registeredTypes = new List<ObjectType>();
      private readonly ObjectEntryTable _objectEntryTable;
      private readonly List<ActiveObject> _activeObjects = new List<ActiveObject>();
      private readonly List<ActiveObject> _newActiveObjects = new List<ActiveObject>();
      private readonly HashSet<ObjectEntry> _respawnPrevention = new HashSet<ObjectEntry>();
      private readonly List<Rectanglei> _lifeTimeAreas = new List<Rectanglei>();
      private readonly Level _level;

      public IReadOnlyCollection<ObjectType> RegisteredTypes
      {
        get => (IReadOnlyCollection<ObjectType>) this._registeredTypes;
      }

      public ICollection<ActiveObject> ActiveObjects => (ICollection<ActiveObject>) this._activeObjects;

      public ObjectEntryTable ObjectEntryTable => this._objectEntryTable;

      public IEnumerable<ICharacter> Characters
      {
        get
        {
          return this._activeObjects.Where<ActiveObject>((Func<ActiveObject, bool>) (x => x.Type.Classification == ObjectClassification.Character)).Cast<ICharacter>();
        }
      }

      public ObjectManager(Level level)
      {
        this._level = level;
        this._objectEntryTable = new ObjectEntryTable(level);
      }

      public void Setup(LevelMap map)
      {
        Trace.WriteLine("Setting up object manager");
        Trace.Indent();
        Trace.WriteLine("Registering object types");
        this.RegisterObjectTypes();
        Trace.Unindent();
      }

      public void Bind(LevelBinding binding)
      {
        Trace.Indent();
        Trace.WriteLine("Clearing objects and object entries");
        this._activeObjects.Clear();
        this._objectEntryTable.Clear();
        Trace.WriteLine("Initialising object entry table");
        this._objectEntryTable.Initialise(binding);
        Trace.Unindent();
      }

      public void ResetLifetimeArea() => this._lifeTimeAreas.Clear();

      public void AddLifeArea(Rectanglei area) => this._lifeTimeAreas.Add(area);

      public void Update()
      {
        this.ManageObjects();
        this.UpdateObjectTypes();
        this.UpdatePrepareActiveObjects();
        this.UpdateActiveObjects();
        this.UpdateCollisionActiveObjects();
        this.RemoveFinishedActiveObjects();
        this._activeObjects.AddRange((IEnumerable<ActiveObject>) this._newActiveObjects);
        this._newActiveObjects.Clear();
        this._objectEntryTable.RemoveFinishedEntries();
      }

      public void UpdateEditor()
      {
        this.ManageObjects();
        this.UpdateEditorActiveObjects();
        this.RemoveFinishedActiveObjects();
        this._respawnPrevention.Clear();
        this._activeObjects.AddRange((IEnumerable<ActiveObject>) this._newActiveObjects);
        this._newActiveObjects.Clear();
        this._objectEntryTable.RemoveFinishedEntries();
      }

      public void Animate()
      {
        this.AnimateObjectTypes();
        this.AnimateActiveObjects();
      }

      public void Draw(
        Renderer renderer,
        Viewport viewport,
        LevelLayer layer,
        LayerViewOptions viewOptions,
        bool priority)
      {
        foreach (ActiveObject activeObject in (IEnumerable<ActiveObject>) this._activeObjects.Where<ActiveObject>((Func<ActiveObject, bool>) (activeObject => activeObject.Layer == layer && activeObject.Priority != 0 && !(activeObject.Priority < 0 & priority) && (activeObject.Priority <= 0 || priority))).OrderBy<ActiveObject, int>((Func<ActiveObject, int>) (x => x.Priority)))
          activeObject.Draw(renderer, viewport, viewOptions);
      }

      public void DrawDebugInfo(Renderer renderer)
      {
        double y1 = 500.0;
        double y2 = y1 + this._level.DebugContext.DrawText(renderer, $"TOTAL OBJECTS: {this._objectEntryTable.Count}", FontAlignment.Left, 8.0, y1, 0.75, new int?(0));
        double y3 = y2 + this._level.DebugContext.DrawText(renderer, $"ACTIVE OBJECTS: {this._activeObjects.Count}", FontAlignment.Left, 8.0, y2, 0.75, new int?(0));
        double num = y3 + this._level.DebugContext.DrawText(renderer, $"RESPAWN PREVENTED OBJECTS: {this._respawnPrevention.Count}", FontAlignment.Left, 8.0, y3, 0.75, new int?(0));
      }

      public void Start()
      {
        Trace.WriteLine("Registering more object types");
        this.RegisterObjectTypes();
        this.StartObjectTypes();
      }

      public void Stop()
      {
        this._objectEntryTable.Clear();
        foreach (ActiveObject activeObject in this._activeObjects.ToArray())
          this.DeactivateObject(activeObject);
        this.StopObjectTypes();
        this.UnregisterObjectTypes();
      }

      public ActiveObject AddObject(ObjectPlacement objectPlacement)
      {
        return this.ActivateObjectEntry(new ObjectEntry(this._level, objectPlacement));
      }

      public void AddObjectEntry(ObjectEntry objectEntry)
      {
        if (string.IsNullOrEmpty(objectEntry.Name))
        {
          int num = this._objectEntryTable.Where<ObjectEntry>((Func<ObjectEntry, bool>) (e => e.Type == objectEntry.Type)).Count<ObjectEntry>();
          objectEntry.Name = $"{objectEntry.Type.Name} {(object) num}";
        }
        this._objectEntryTable.Add(objectEntry);
      }

      public T AddSubObject<T>(ActiveObject parentObject) where T : ActiveObject
      {
        return this.ActiveSubObject<T>(new ObjectEntry(this._level, new ObjectPlacement(parentObject.Type.ResourceKey, this._level.Map.Layers.IndexOf(parentObject.Layer), parentObject.Position)), parentObject);
      }

      private void MapInstancesOf(ActiveObject activeObject)
      {
        ObjectEntry entry = this._objectEntryTable.Select<ObjectEntry, ObjectEntry>((Func<ObjectEntry, ObjectEntry>) (e => e)).Where<ObjectEntry>((Func<ObjectEntry, bool>) (e => e.Uid == activeObject.Uid)).FirstOrDefault<ObjectEntry>();
        IEnumerable<ActiveObject> source = this._activeObjects.Concat<ActiveObject>((IEnumerable<ActiveObject>) this._newActiveObjects);
        if (entry == null)
          return;
        foreach (ObjectMapping mapping1 in (IEnumerable<ObjectMapping>) entry.Mappings)
        {
          ObjectMapping mapping = mapping1;
          ActiveObject activeObject1 = source.Select<ActiveObject, ActiveObject>((Func<ActiveObject, ActiveObject>) (e => e)).Where<ActiveObject>((Func<ActiveObject, bool>) (e => e.Uid == mapping.Target)).FirstOrDefault<ActiveObject>();
          if (activeObject1 != null)
          {
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MemberInfo member = ((IEnumerable<MemberInfo>) activeObject.GetType().GetMember(mapping.Field, bindingAttr)).First<MemberInfo>();
            if (member != (MemberInfo) null)
              member.SetUnderlyingValue((object) activeObject, (object) activeObject1);
          }
        }
        foreach (ActiveObject instance in source.Select<ActiveObject, ActiveObject>((Func<ActiveObject, ActiveObject>) (ao => ao)).Where<ActiveObject>((Func<ActiveObject, bool>) (ao => ao.Entry.Mappings.FirstOrDefault<ObjectMapping>((Func<ObjectMapping, bool>) (m => m.Target == entry.Uid)) != null)))
        {
          foreach (ObjectMapping mapping in (IEnumerable<ObjectMapping>) instance.Entry.Mappings)
          {
            if (mapping.Target == activeObject.Uid)
            {
              BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
              MemberInfo member = ((IEnumerable<MemberInfo>) instance.GetType().GetMember(mapping.Field, bindingAttr)).First<MemberInfo>();
              if (member != (MemberInfo) null)
                member.SetUnderlyingValue((object) instance, (object) activeObject);
            }
          }
        }
      }

      private void UnMapInstancesOf(ActiveObject activeObject)
      {
        ObjectEntry entry = this._objectEntryTable.Select<ObjectEntry, ObjectEntry>((Func<ObjectEntry, ObjectEntry>) (e => e)).Where<ObjectEntry>((Func<ObjectEntry, bool>) (e => e.Uid == activeObject.Uid)).FirstOrDefault<ObjectEntry>();
        if (entry == null)
          return;
        if (entry != null)
        {
          foreach (ObjectMapping mapping in (IEnumerable<ObjectMapping>) entry.Mappings)
          {
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MemberInfo member = ((IEnumerable<MemberInfo>) activeObject.GetType().GetMember(mapping.Field, bindingAttr)).First<MemberInfo>();
            if (member != (MemberInfo) null)
              member.SetUnderlyingValue((object) activeObject, (object) null);
          }
        }
        foreach (ActiveObject instance in this._activeObjects.Select<ActiveObject, ActiveObject>((Func<ActiveObject, ActiveObject>) (ao => ao)).Where<ActiveObject>((Func<ActiveObject, bool>) (ao => ao.Entry.Mappings.FirstOrDefault<ObjectMapping>((Func<ObjectMapping, bool>) (m => m.Target == entry.Uid)) != null)))
        {
          foreach (ObjectMapping mapping in (IEnumerable<ObjectMapping>) instance.Entry.Mappings)
          {
            if (mapping.Target == activeObject.Uid)
            {
              BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
              MemberInfo member = ((IEnumerable<MemberInfo>) instance.GetType().GetMember(mapping.Field, bindingAttr)).First<MemberInfo>();
              if (member != (MemberInfo) null)
                member.SetUnderlyingValue((object) instance, (object) null);
            }
          }
        }
      }

      public ActiveObject ActivateObjectEntry(ObjectEntry objectEntry)
      {
        ActiveObject activeObject = objectEntry.CreateActiveObject();
        this._newActiveObjects.Add(activeObject);
        this.MapInstancesOf(activeObject);
        activeObject.Start();
        return activeObject;
      }

      public T ActiveSubObject<T>(ObjectEntry objectEntry, ActiveObject parentObject) where T : ActiveObject
      {
        T subObject = objectEntry.CreateSubObject<T>();
        this._newActiveObjects.Add((ActiveObject) subObject);
        subObject.ParentObject = parentObject;
        subObject.Start();
        return subObject;
      }

      public void DeactivateObject(ActiveObject activeObject)
      {
        activeObject.Finish();
        activeObject.Stop();
        ObjectEntry entry = activeObject.Entry;
        entry.Active = (ActiveObject) null;
        if (entry.FinishedForever)
          this._objectEntryTable.Remove(entry);
        else
          this._respawnPrevention.Add(entry);
        this.UnMapInstancesOf(activeObject);
        this._activeObjects.Remove(activeObject);
      }

      public bool IsInLifetimeArea(Rectanglei rect)
      {
        return this._lifeTimeAreas.Any<Rectanglei>((Func<Rectanglei, bool>) (r => rect.IntersectsWith(r)));
      }

      private void ManageObjects()
      {
        this._respawnPrevention.ExceptWith((IEnumerable<ObjectEntry>) this._respawnPrevention.Where<ObjectEntry>((Func<ObjectEntry, bool>) (x => !this.IsInLifetimeArea(x.LifetimeArea))).ToArray<ObjectEntry>());
        foreach (ActiveObject activeObject in (IEnumerable<ActiveObject>) this._activeObjects.Where<ActiveObject>((Func<ActiveObject, bool>) (x =>
        {
          if (x.LockLifetime || this.IsInLifetimeArea(x.LifetimeArea))
            return false;
          return !this.IsInLifetimeArea(x.Entry.LifetimeArea) || x.IsSubObject;
        })).ToArray<ActiveObject>())
        {
          this.DeactivateObject(activeObject);
          this._respawnPrevention.Remove(activeObject.Entry);
        }
        List<ObjectEntry> objectEntryList = new List<ObjectEntry>();
        foreach (Rectanglei lifeTimeArea in this._lifeTimeAreas)
          objectEntryList.AddRange(this._objectEntryTable.GetAllInRegion(lifeTimeArea));
        foreach (ObjectEntry objectEntry in objectEntryList)
        {
          if (objectEntry.Active == null && !this._respawnPrevention.Contains(objectEntry))
            this.ActivateObjectEntry(objectEntry);
        }
      }

      public void RegisterObjectTypes()
      {
        foreach (ObjectType objectType in ((IEnumerable<ObjectType>) ObjectType.LoadedTypes).Except<ObjectType>((IEnumerable<ObjectType>) this._registeredTypes).ToArray<ObjectType>())
        {
          this._registeredTypes.Add(objectType);
          objectType.Register(this._level);
        }
      }

      public void UnregisterObjectTypes()
      {
        foreach (ObjectType registeredType in this._registeredTypes)
          registeredType.Unregister();
        this._registeredTypes.Clear();
      }

      private void StartObjectTypes()
      {
        foreach (ObjectType registeredType in this._registeredTypes)
          registeredType.Start();
      }

      private void StopObjectTypes()
      {
        foreach (ObjectType registeredType in this._registeredTypes)
          registeredType.Stop();
      }

      private void UpdateObjectTypes()
      {
        foreach (ObjectType registeredType in this._registeredTypes)
          registeredType.Update();
      }

      private void AnimateObjectTypes()
      {
        foreach (ObjectType registeredType in this._registeredTypes)
          registeredType.Animate();
      }

      private void UpdateEditorActiveObjects()
      {
        foreach (ActiveObject activeObject in this._activeObjects)
          activeObject.UpdateEditor();
      }

      private void UpdatePrepareActiveObjects()
      {
        foreach (ActiveObject activeObject in this._activeObjects)
          activeObject.UpdatePrepare();
      }

      private void UpdateActiveObjects()
      {
        foreach (ActiveObject activeObject in this._activeObjects)
          activeObject.Update();
      }

      private void UpdateCollisionActiveObjects()
      {
        foreach (ActiveObject activeObject in this._activeObjects)
          activeObject.UpdateCollision();
      }

      private void AnimateActiveObjects()
      {
        foreach (ActiveObject activeObject in this._activeObjects)
          activeObject.Animate();
      }

      public void RemoveFinishedActiveObjects()
      {
        foreach (ActiveObject activeObject in this._activeObjects.Where<ActiveObject>((Func<ActiveObject, bool>) (x => x.Finished)).ToArray<ActiveObject>())
          this.DeactivateObject(activeObject);
      }

      public void RemoveFinishedEntries() => this._objectEntryTable.RemoveFinishedEntries();

      public ICharacter GetClosestCharacterTo(Vector2 position)
      {
        IEnumerable<ICharacter> characters = this._activeObjects.Where<ActiveObject>((Func<ActiveObject, bool>) (x => x.Type.Classification == ObjectClassification.Character)).Select<ActiveObject, ICharacter>((Func<ActiveObject, ICharacter>) (x => (ICharacter) x)).Where<ICharacter>((Func<ICharacter, bool>) (x => !x.IsDead && !x.IsDebug && !x.IsDying));
        ICharacter closestCharacterTo = (ICharacter) null;
        double num = double.NaN;
        foreach (ICharacter character in characters)
        {
          double length = (position - (Vector2) character.Position).Length;
          if (closestCharacterTo == null || length < num)
          {
            closestCharacterTo = character;
            num = length;
          }
        }
        return closestCharacterTo;
      }

      public void FinishSubObjects(ActiveObject parent)
      {
        foreach (ActiveObject activeObject in this._activeObjects.Where<ActiveObject>((Func<ActiveObject, bool>) (x => x.ParentObject == parent)))
          activeObject.Finish();
      }

      public bool IsCharacterStandingOn(CollisionVector v)
      {
        foreach (ICharacter character in this.Characters)
        {
          if (character.GroundVector == v)
            return true;
        }
        return false;
      }
    }
}
