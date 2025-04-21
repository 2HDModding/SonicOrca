// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Video.FilmGroup
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Graphics.V2.Video
{

    public class FilmGroup : 
      ILoadedResource,
      IDisposable,
      IReadOnlyList<Film>,
      IReadOnlyCollection<Film>,
      IEnumerable<Film>,
      IEnumerable
    {
      private readonly ResourceTree _resourceTree;
      private readonly IReadOnlyList<string> _filmResourceKeys;
      private readonly IReadOnlyList<Film> _films;

      public Resource Resource { get; set; }

      public IReadOnlyList<IFilmBuffer> FilmBuffers { get; private set; }

      public IReadOnlyList<string> FilmResourceKeys { get; private set; }

      public FilmGroup(
        ResourceTree resourceTree,
        IEnumerable<string> filmResourceKeys,
        IEnumerable<Film> films)
      {
        this._resourceTree = resourceTree;
        this._filmResourceKeys = (IReadOnlyList<string>) filmResourceKeys.ToArray<string>();
        this._films = (IReadOnlyList<Film>) films.ToArray<Film>();
      }

      public void OnLoaded()
      {
        this.FilmResourceKeys = this._filmResourceKeys;
        this.FilmBuffers = (IReadOnlyList<IFilmBuffer>) ((IEnumerable<string>) this._filmResourceKeys).Select<string, IFilmBuffer>((Func<string, IFilmBuffer>) (x => this._resourceTree.GetLoadedResource<IFilmBuffer>(x))).ToArray<IFilmBuffer>();
      }

      public void Dispose()
      {
      }

      public Film this[int index] => this._films[index];

      public int Count => ((IReadOnlyCollection<Film>) this._films).Count;

      public IEnumerator<Film> GetEnumerator() => ((IEnumerable<Film>) this._films).GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
    }
}
