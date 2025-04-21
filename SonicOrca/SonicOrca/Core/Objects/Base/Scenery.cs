// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.Scenery
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Extensions;
using SonicOrca.Graphics;

namespace SonicOrca.Core.Objects.Base
{

    public class Scenery : ActiveObject
    {
      private readonly string _animationGroupResourceKey;
      private AnimationInstance _animationInstance;

      protected bool AdditiveBlending { get; set; }

      public Scenery(string animationGroupResourceKey)
      {
        this._animationGroupResourceKey = animationGroupResourceKey;
      }

      protected override void OnStart()
      {
        this._animationInstance = new AnimationInstance(this.ResourceTree, this.Type.GetAbsolutePath(this._animationGroupResourceKey));
      }

      protected override void OnAnimate() => this._animationInstance.Animate();

      protected override void OnDraw(Renderer renderer, LayerViewOptions viewOptions)
      {
        IObjectRenderer objectRenderer = renderer.GetObjectRenderer();
        objectRenderer.BlendMode = this.AdditiveBlending ? BlendMode.Additive : BlendMode.Alpha;
        objectRenderer.Render(this._animationInstance);
      }
    }
}
