// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.ParticleObject
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Extensions;
using SonicOrca.Geometry;
using SonicOrca.Graphics;

namespace SonicOrca.Core.Objects.Base
{

    public class ParticleObject : ActiveObject
    {
      private readonly string _animationGroupResourceKey;
      private readonly int _animationIndex;
      private readonly int _animationCycles;
      protected AnimationInstance _animationInstance;

      protected bool AdditiveBlending { get; set; }

      protected double FilterMultiplier { get; set; }

      public bool FlipX { get; set; }

      public bool FlipY { get; set; }

      public ParticleObject(string animationGroupResourceKey, int animationIndex = 0, int animationCycles = 1)
      {
        this.FilterMultiplier = 1.0;
        this._animationGroupResourceKey = animationGroupResourceKey;
        this._animationIndex = animationIndex;
        this._animationCycles = animationCycles;
      }

      protected override void OnStart()
      {
        this._animationInstance = new AnimationInstance(this.ResourceTree, this.Type.GetAbsolutePath(this._animationGroupResourceKey), this._animationIndex);
        this.Priority = 2048 /*0x0800*/;
      }

      protected override void OnStop() => this.FinishForever();

      protected override void OnUpdate()
      {
        if (this._animationCycles == 0 || this._animationInstance.Cycles < this._animationCycles)
          return;
        this.FinishForever();
      }

      protected override void OnAnimate() => this._animationInstance.Animate();

      protected bool CanDraw()
      {
        return this._animationCycles == 0 || this._animationInstance.Cycles < this._animationCycles;
      }

      protected override void OnDraw(Renderer renderer, LayerViewOptions viewOptions)
      {
        if (!this.CanDraw())
          return;
        IObjectRenderer objectRenderer = renderer.GetObjectRenderer();
        using (objectRenderer.BeginMatixState())
        {
          if (this.FlipX)
            objectRenderer.ModelMatrix *= Matrix4.CreateScale(-1.0, 1.0);
          if (this.FlipY)
            objectRenderer.ModelMatrix *= Matrix4.CreateScale(1.0, -1.0);
          if (this.FilterMultiplier == 0.0)
            objectRenderer.Filter = 0;
          else
            objectRenderer.FilterAmount *= this.FilterMultiplier;
          objectRenderer.BlendMode = this.AdditiveBlending ? BlendMode.Additive : BlendMode.Alpha;
          objectRenderer.Render(this._animationInstance);
        }
      }
    }
}
