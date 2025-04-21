// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.TileSet
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core
{

    public class TileSet : 
      IReadOnlyDictionary<int, ITile>,
      IReadOnlyCollection<KeyValuePair<int, ITile>>,
      IEnumerable<KeyValuePair<int, ITile>>,
      IEnumerable,
      ILoadedResource,
      IDisposable
    {
      public const int InitialTileCapacity = 4096 /*0x1000*/;
      private ITile[] _tiles = new ITile[4096 /*0x1000*/];
      private readonly HashSet<int> _tileIds = new HashSet<int>();
      private readonly ResourceTree _resourceTree;
      private readonly IReadOnlyList<string> _textureKeys;

      public Resource Resource { get; set; }

      public IReadOnlyList<ITexture> Textures { get; private set; }

      public TileSet()
      {
      }

      public TileSet(ResourceTree resourceTree, IEnumerable<string> textureKeys)
      {
        this._resourceTree = resourceTree;
        this._textureKeys = (IReadOnlyList<string>) textureKeys.ToArray<string>();
      }

      public ITile this[int id]
      {
        get => id >= this._tiles.Length ? (ITile) null : this._tiles[id];
        set
        {
          if (this._tiles.Length <= id)
          {
            int length = this._tiles.Length;
            do
            {
              length *= 2;
            }
            while (length <= id);
            Array.Resize<ITile>(ref this._tiles, length);
          }
          this._tiles[id] = value;
          this._tileIds.Add(id);
        }
      }

      public void Animate()
      {
        foreach (int tileId in this._tileIds)
          this._tiles[tileId].Animate();
      }

      public void DrawTile(Renderer renderer, int index, int x, int y)
      {
        this.DrawTile(renderer, index, new Rectanglei(x, y, 64 /*0x40*/, 64 /*0x40*/));
      }

      public void DrawTile(Renderer renderer, int index, Rectanglei destination)
      {
        this.DrawTile(renderer, index, new Rectanglei(0, 0, destination.Width, destination.Height), destination);
      }

      public void DrawTile(Renderer renderer, int index, Rectanglei source, Rectanglei destination)
      {
        ITile tile;
        if (!this.TryGetValue(index, out tile))
          return;
        tile.Draw(renderer, index, source, destination);
      }

      public void OnLoaded()
      {
        this.Textures = (IReadOnlyList<ITexture>) ((IEnumerable<string>) this._textureKeys).Select<string, ITexture>((Func<string, ITexture>) (x => this._resourceTree.GetLoadedResource<ITexture>(x))).ToArray<ITexture>();
      }

      public void Dispose()
      {
      }

      public bool ContainsKey(int key) => this[key] != null;

      public IEnumerable<int> Keys => (IEnumerable<int>) this._tileIds;

      public bool TryGetValue(int tile, out ITile value)
      {
        value = this[tile & 4095 /*0x0FFF*/];
        return value != null;
      }

      public IEnumerable<ITile> Values
      {
        get => this._tileIds.Select<int, ITile>((Func<int, ITile>) (i => this._tiles[i]));
      }

      public int Count => this._tileIds.Count;

      public IEnumerator<KeyValuePair<int, ITile>> GetEnumerator()
      {
        return this._tileIds.Select<int, KeyValuePair<int, ITile>>((Func<int, KeyValuePair<int, ITile>>) (i => new KeyValuePair<int, ITile>(i, this._tiles[i]))).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
    }
}
