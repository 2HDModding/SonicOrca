// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ObjectType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Microsoft.CSharp.RuntimeBinder;
using SonicOrca.Core.Objects.Metadata;
using SonicOrca.Geometry;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SonicOrca.Core
{

    public abstract class ObjectType : ILoadedResource, IDisposable
    {
      public const string AnimalClass = "animal";
      public const string CharacterClass = "character";
      public const string ParticleClass = "particle";
      public const string RingClass = "ring";
      private static readonly Lockable<List<ObjectType>> LoadedTypeList = new Lockable<List<ObjectType>>(new List<ObjectType>());
      private readonly string _name;
      private readonly string _description;
      private readonly ObjectClassification _classification;
      private readonly string[] _dependencies;
      private readonly IReadOnlyCollection<ObjectEditorProperty> _editorProperties;

      public Resource Resource { get; set; }

      public static IReadOnlyList<ObjectType> LoadedTypes
      {
        get
        {
          lock (ObjectType.LoadedTypeList.Sync)
            return (IReadOnlyList<ObjectType>) ObjectType.LoadedTypeList.Instance.ToArray();
        }
      }

      public static void ClearLoadedTypes()
      {
        lock (ObjectType.LoadedTypeList.Sync)
          ObjectType.LoadedTypeList.Instance.Clear();
      }

      public Level Level { get; private set; }

      public string ResourceKey => this.Resource.FullKeyPath;

      public string Name => this._name;

      public ObjectClassification Classification => this._classification;

      public IReadOnlyCollection<string> Dependencies
      {
        get => (IReadOnlyCollection<string>) this._dependencies;
      }

      public IReadOnlyCollection<ObjectEditorProperty> EditorProperties => this._editorProperties;

      public ObjectType()
      {
        this._editorProperties = (IReadOnlyCollection<ObjectEditorProperty>) StateVariableAttribute.GetEditorProperties(this);
        NameAttribute nameAttribute = NameAttribute.FromObject((object) this);
        if (nameAttribute != null)
          this._name = nameAttribute.Name;
        DescriptionAttribute descriptionAttribute = DescriptionAttribute.FromObject((object) this);
        if (descriptionAttribute != null)
          this._description = descriptionAttribute.Description;
        ClassificationAttribute classificationAttribute = ClassificationAttribute.FromObject((object) this);
        if (classificationAttribute != null)
          this._classification = classificationAttribute.Classification;
        this._dependencies = SonicOrca.Core.Objects.Metadata.DependencyAttribute.GetDependencies((object) this).ToArray<string>();
      }

      public void OnLoaded()
      {
        lock (ObjectType.LoadedTypeList.Sync)
        {
          if (ObjectType.LoadedTypeList.Instance.Exists((Predicate<ObjectType>) (ot => ot.GetType() == this.GetType())))
            return;
          ObjectType.LoadedTypeList.Instance.Add(this);
        }
      }

      public void Dispose()
      {
        lock (ObjectType.LoadedTypeList.Sync)
        {
          if (!ObjectType.LoadedTypeList.Instance.Exists((Predicate<ObjectType>) (ot => ot.GetType() == this.GetType())))
            return;
          ObjectType.LoadedTypeList.Instance.Remove(this);
        }
      }

      public void Register(Level level) => this.Level = level;

      public void Unregister() => this.Level = (Level) null;

      public void Start() => this.OnStart();

      public void Update() => this.OnUpdate();

      public void Animate() => this.OnAnimate();

      public void Stop() => this.OnStop();

      public virtual ActiveObject CreateInstance()
      {
        return (ActiveObject) Activator.CreateInstance((ObjectInstanceAttribute.FromObject((object) this) ?? throw new Exception()).ObjectInstanceType);
      }

      public Vector2 GetLifeRadius(IActiveObject state)
      {
        if (state.GetType() == typeof (IActiveObject))
          throw new InvalidOperationException();
        object obj1 = (object) this;
        // ISSUE: reference to a compiler-generated field
        if (ObjectType.\u003C\u003Eo__41.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          ObjectType.\u003C\u003Eo__41.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, Vector2>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (Vector2), typeof (ObjectType)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, Vector2> target = ObjectType.\u003C\u003Eo__41.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, Vector2>> p1 = ObjectType.\u003C\u003Eo__41.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (ObjectType.\u003C\u003Eo__41.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          ObjectType.\u003C\u003Eo__41.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, nameof (GetLifeRadius), (IEnumerable<Type>) null, typeof (ObjectType), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj2 = ObjectType.\u003C\u003Eo__41.\u003C\u003Ep__0.Target((CallSite) ObjectType.\u003C\u003Eo__41.\u003C\u003Ep__0, obj1, (object) state);
        return target((CallSite) p1, obj2);
      }

      public Vector2 GetLifeRadius(ActiveObject state) => new Vector2(0.0, 0.0);

      protected virtual void OnStart()
      {
      }

      protected virtual void OnUpdate()
      {
      }

      protected virtual void OnAnimate()
      {
      }

      protected virtual void OnStop()
      {
      }
    }
}
