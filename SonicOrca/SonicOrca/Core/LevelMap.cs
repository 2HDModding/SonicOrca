// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelMap
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using SonicOrca.Geometry;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core
{

    public class LevelMap : ILoadedResource, IDisposable
    {
      private readonly ILevelLayerTreeNode _layerTree = (ILevelLayerTreeNode) new LevelLayerGroup((string) null);
      private readonly List<LevelLayer> _layers = new List<LevelLayer>();
      private readonly List<CollisionVector> _collisionVectors = new List<CollisionVector>();
      private readonly List<int> _collisionPathLayers = new List<int>();
      private readonly List<ObjectPlacement> _objectPlacements = new List<ObjectPlacement>();
      private readonly List<LevelMarker> _markers = new List<LevelMarker>();

      public Resource Resource { get; set; }

      public ILevelLayerTreeNode LayerTree => this._layerTree;

      public IList<LevelLayer> Layers => (IList<LevelLayer>) this._layers;

      public IList<CollisionVector> CollisionVectors => (IList<CollisionVector>) this._collisionVectors;

      public IList<int> CollisionPathLayers => (IList<int>) this._collisionPathLayers;

      public IList<LevelMarker> Markers => (IList<LevelMarker>) this._markers;

      public Level Level { get; set; }

      public int MaxColumns
      {
        get
        {
          return this.Layers.Count == 0 ? 0 : this.Layers.Max<LevelLayer>((Func<LevelLayer, int>) (x => x.Columns));
        }
      }

      public int MaxRows
      {
        get
        {
          return this.Layers.Count == 0 ? 0 : this.Layers.Max<LevelLayer>((Func<LevelLayer, int>) (x => x.Rows));
        }
      }

      public Rectanglei Bounds
      {
        get => new Rectanglei(0, 0, this.MaxColumns * 64 /*0x40*/, this.MaxRows * 64 /*0x40*/);
      }

      public void Update()
      {
        foreach (LevelLayer layer in this._layers)
          layer.Update();
      }

      public void Animate()
      {
        foreach (LevelLayer layer in this._layers)
          layer.Animate();
      }

      public void OnLoaded()
      {
      }

      public void Dispose()
      {
      }

      public LevelLayer FindLayer(string name)
      {
        return this._layers.FirstOrDefault<LevelLayer>((Func<LevelLayer, bool>) (x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
      }
    }
}
