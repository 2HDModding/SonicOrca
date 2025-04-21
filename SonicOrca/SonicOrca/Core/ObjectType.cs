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
        public Level Level { get; private set; }

        public string ResourceKey => Resource.FullKeyPath;
        public string Name => _name;
        public ObjectClassification Classification => _classification;
        public IReadOnlyCollection<string> Dependencies => _dependencies;
        public IReadOnlyCollection<ObjectEditorProperty> EditorProperties => _editorProperties;

        public static IReadOnlyList<ObjectType> LoadedTypes
        {
            get
            {
                lock (LoadedTypeList.Sync)
                    return LoadedTypeList.Instance.ToArray();
            }
        }

        public static void ClearLoadedTypes()
        {
            lock (LoadedTypeList.Sync)
                LoadedTypeList.Instance.Clear();
        }

        public ObjectType()
        {
            _editorProperties = StateVariableAttribute.GetEditorProperties(this);
            _name = NameAttribute.FromObject(this)?.Name;
            _description = DescriptionAttribute.FromObject(this)?.Description;
            _classification = ClassificationAttribute.FromObject(this)?.Classification ?? default;
            _dependencies = SonicOrca.Core.Objects.Metadata.DependencyAttribute.GetDependencies(this).ToArray();
        }

        public void OnLoaded()
        {
            lock (LoadedTypeList.Sync)
            {
                if (!LoadedTypeList.Instance.Exists(ot => ot.GetType() == GetType()))
                    LoadedTypeList.Instance.Add(this);
            }
        }

        public void Dispose()
        {
            lock (LoadedTypeList.Sync)
            {
                LoadedTypeList.Instance.RemoveAll(ot => ot.GetType() == GetType());
            }
        }

        public void Register(Level level) => Level = level;
        public void Unregister() => Level = null;

        public void Start() => OnStart();
        public void Update() => OnUpdate();
        public void Animate() => OnAnimate();
        public void Stop() => OnStop();

        public virtual ActiveObject CreateInstance()
        {
            var attr = ObjectInstanceAttribute.FromObject(this) ?? throw new Exception("Missing ObjectInstanceAttribute.");
            return (ActiveObject)Activator.CreateInstance(attr.ObjectInstanceType);
        }

        public Vector2 GetLifeRadius(IActiveObject state)
        {
            if (state.GetType() == typeof(IActiveObject))
                throw new InvalidOperationException();

            var instanceType = GetType();
            var method = instanceType.GetMethod("GetLifeRadius", new[] { state.GetType() });
            if (method == null)
                throw new MissingMethodException($"GetLifeRadius not implemented for {state.GetType().Name}.");

            return (Vector2)method.Invoke(this, new object[] { state });
        }

        public virtual Vector2 GetLifeRadius(ActiveObject state) => new Vector2(0.0, 0.0);

        protected virtual void OnStart() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnAnimate() { }
        protected virtual void OnStop() { }
    }
}
