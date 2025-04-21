// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Animation.CompositionGroup
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Graphics.V2.Animation
{

    public class CompositionGroup : 
      ILoadedResource,
      IDisposable,
      IReadOnlyList<Composition>,
      IReadOnlyCollection<Composition>,
      IEnumerable<Composition>,
      IEnumerable
    {
      private readonly ResourceTree _resourceTree;
      private readonly IReadOnlyList<string> _textureResourceKeys;
      private readonly IReadOnlyList<CompositionAsset> _assets;
      private readonly IReadOnlyList<Composition> _compositions;

      public Resource Resource { get; set; }

      public IReadOnlyList<ITexture> Textures { get; private set; }

      public IReadOnlyList<string> TextureResourceKeys { get; private set; }

      public IReadOnlyList<CompositionAsset> Assets { get; private set; }

      public CompositionGroup(
        ResourceTree resourceTree,
        IEnumerable<string> textureResourceKeys,
        IEnumerable<CompositionAsset> assets,
        IEnumerable<Composition> compositions)
      {
        this._resourceTree = resourceTree;
        this._textureResourceKeys = (IReadOnlyList<string>) textureResourceKeys.ToArray<string>();
        this._assets = (IReadOnlyList<CompositionAsset>) assets.ToArray<CompositionAsset>();
        this._compositions = (IReadOnlyList<Composition>) compositions.ToArray<Composition>();
      }

      public void OnLoaded()
      {
        this.TextureResourceKeys = this._textureResourceKeys;
        this.Textures = (IReadOnlyList<ITexture>) ((IEnumerable<string>) this._textureResourceKeys).Select<string, ITexture>((Func<string, ITexture>) (x => this._resourceTree.GetLoadedResource<ITexture>(x))).ToArray<ITexture>();
        this.Assets = this._assets;
      }

      public void Dispose()
      {
      }

      public Composition this[int index] => this._compositions[index];

      public int Count => ((IReadOnlyCollection<Composition>) this._compositions).Count;

      public IEnumerator<Composition> GetEnumerator()
      {
        return ((IEnumerable<Composition>) this._compositions).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
    }
}
