// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.TileSequence
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core
{

    public class TileSequence : ITile
    {
      private readonly TileSet _tileSet;
      private readonly int _id;
      private readonly int[] _tileIds;

      public int Id => this._id;

      public bool Animated => false;

      public TileSequence(TileSet tileSet, int id, IEnumerable<int> tileIds)
      {
        this._tileSet = tileSet;
        this._id = id;
        this._tileIds = tileIds.ToArray<int>();
      }

      public void Animate()
      {
      }

      public void Draw(Renderer renderer, int flags, int x, int y)
      {
        foreach (int tileId in this._tileIds)
        {
          ITile tile;
          if (this._tileSet.TryGetValue(tileId, out tile))
            tile.Draw(renderer, flags, x, y);
        }
      }

      public void Draw(Renderer renderer, int flags, Rectanglei source, Rectanglei destination)
      {
        ITileRenderer tileRenderer = renderer.GetTileRenderer();
        if (tileRenderer.Rendering)
          this.Draw(tileRenderer, flags, source, destination);
        else
          this.Draw(renderer.Get2dRenderer(), flags, source, destination);
      }

      public void Draw(I2dRenderer g, int flags, Rectanglei source, Rectanglei destination)
      {
        foreach (int tileId in this._tileIds)
        {
          ITile tile;
          if (this._tileSet.TryGetValue(tileId, out tile))
            tile.Draw(g, flags, source, destination);
        }
      }

      public void Draw(
        ITileRenderer tileRenderer,
        int flags,
        Rectanglei source,
        Rectanglei destination)
      {
        foreach (int tileId in this._tileIds)
        {
          ITile tile;
          if (this._tileSet.TryGetValue(tileId, out tile))
            tile.Draw(tileRenderer, flags, source, destination);
        }
      }
    }
}
