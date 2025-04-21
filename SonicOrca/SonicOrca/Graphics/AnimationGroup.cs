// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.AnimationGroup
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Graphics
{

    public class AnimationGroup : 
      ILoadedResource,
      IDisposable,
      IReadOnlyList<Animation>,
      IReadOnlyCollection<Animation>,
      IEnumerable<Animation>,
      IEnumerable
    {
      private readonly ResourceTree _resourceTree;
      private readonly IReadOnlyList<string> _textureResourceKeys;
      private readonly IReadOnlyList<Animation> _animations;

      public Resource Resource { get; set; }

      public IReadOnlyList<ITexture> Textures { get; private set; }

      public AnimationGroup(IEnumerable<ITexture> textures, IEnumerable<Animation> animations)
      {
        this.Textures = (IReadOnlyList<ITexture>) textures.ToArray<ITexture>();
        this._animations = (IReadOnlyList<Animation>) animations.ToArray<Animation>();
      }

      public AnimationGroup(
        ResourceTree resourceTree,
        IEnumerable<string> textureResourceKeys,
        IEnumerable<Animation> animations)
      {
        this._resourceTree = resourceTree;
        this._textureResourceKeys = (IReadOnlyList<string>) textureResourceKeys.ToArray<string>();
        this._animations = (IReadOnlyList<Animation>) animations.ToArray<Animation>();
      }

      public void OnLoaded()
      {
        this.Textures = (IReadOnlyList<ITexture>) ((IEnumerable<string>) this._textureResourceKeys).Select<string, ITexture>((Func<string, ITexture>) (x => this._resourceTree.GetLoadedResource<ITexture>(x))).ToArray<ITexture>();
      }

      public void Dispose()
      {
      }

      public Animation this[int index] => this._animations[index];

      public int Count => ((IReadOnlyCollection<Animation>) this._animations).Count;

      public IEnumerator<Animation> GetEnumerator()
      {
        return ((IEnumerable<Animation>) this._animations).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
    }
}
