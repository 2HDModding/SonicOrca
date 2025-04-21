// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Area
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core
{

    public abstract class Area : IDisposable, ILoadedResource
    {
      private readonly string[] _dependencies;

      public Resource Resource { get; set; }

      public IReadOnlyCollection<string> Dependencies
      {
        get => (IReadOnlyCollection<string>) this._dependencies;
      }

      public virtual IEnumerable<KeyValuePair<string, object>> StateVariables
      {
        get => Enumerable.Empty<KeyValuePair<string, object>>();
      }

      public Area(IEnumerable<string> dependencies)
      {
        this._dependencies = dependencies.ToArray<string>();
      }

      public abstract void Prepare(Level level, LevelPrepareSettings settings);

      public virtual void OnStart()
      {
      }

      public virtual void OnUpdate()
      {
      }

      public virtual void OnComplete()
      {
      }

      public virtual void OnPause()
      {
      }

      public virtual void OnUnpause()
      {
      }

      public void OnLoaded()
      {
      }

      public virtual void Dispose()
      {
      }

      protected void ExtendSeamlessLevelBounds(Level level, Rectanglei newRegion)
      {
        newRegion.Left = Math.Min(newRegion.Left, level.Bounds.Left);
        newRegion.Top = Math.Min(newRegion.Top, level.Bounds.Top);
        newRegion.Right = Math.Max(newRegion.Right, level.Bounds.Right);
        newRegion.Bottom = Math.Max(newRegion.Bottom, level.Bounds.Bottom);
        level.SeamlessNextBounds = newRegion;
      }
    }
}
