// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.Fragment
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;

namespace SonicOrca.Core.Objects.Base
{

    public class Fragment : ActiveObject
    {
      private AnimationInstance _animationInstance;

      public AnimationGroup AnimationGroup { get; set; }

      public string AnimationGroupResourceKey { get; set; }

      public int AnimationIndex { get; set; }

      public int AnimationCycles { get; set; }

      protected bool AdditiveBlending { get; set; }

      protected double FilterMultiplier { get; set; }

      public bool FlipX { get; set; }

      public bool FlipY { get; set; }

      public double Angle { get; set; }

      public double Scale { get; set; }

      public double AngularVelocity { get; set; }

      public Vector2 Velocity { get; set; }

      public double Gravity { get; set; }

      public Fragment()
      {
        this.FilterMultiplier = 1.0;
        this.Gravity = 0.875;
        this.Scale = 1.0;
      }

      protected override void OnStart() => this.Priority = 2048 /*0x0800*/;

      protected override void OnStop() => this.FinishForever();

      public void Initialise()
      {
        if (this.AnimationGroup == null)
          this.AnimationGroup = this.ResourceTree.GetLoadedResource<AnimationGroup>(this.AnimationGroupResourceKey);
        this._animationInstance = new AnimationInstance(this.AnimationGroup, this.AnimationIndex);
      }

      protected override void OnUpdate()
      {
        if (this._animationInstance == null)
          this.Initialise();
        if (this.AnimationCycles != 0 && this._animationInstance.Cycles >= this.AnimationCycles)
        {
          this.FinishForever();
        }
        else
        {
          this.MovePrecise(this.Velocity);
          this.Velocity += new Vector2(0.0, this.Gravity);
          this.Angle += this.AngularVelocity;
        }
      }

      protected override void OnAnimate()
      {
        if (this._animationInstance == null)
          return;
        this._animationInstance.Animate();
      }

      protected override void OnDraw(Renderer renderer, LayerViewOptions viewOptions)
      {
        if (this._animationInstance == null || this.AnimationCycles != 0 && this._animationInstance.Cycles >= this.AnimationCycles)
          return;
        IObjectRenderer objectRenderer = renderer.GetObjectRenderer();
        if (this.FilterMultiplier == 0.0)
          objectRenderer.Filter = 0;
        else
          objectRenderer.FilterAmount *= this.FilterMultiplier;
        objectRenderer.BlendMode = this.AdditiveBlending ? BlendMode.Additive : BlendMode.Alpha;
        objectRenderer.ModelMatrix = objectRenderer.ModelMatrix.RotateZ(this.Angle);
        Rectanglei source = this._animationInstance.CurrentFrame.Source;
        int width = (int) ((double) source.Width * this.Scale);
        int height = (int) ((double) source.Height * this.Scale);
        Rectanglei destination = new Rectanglei(-width / 2, -height / 2, width, height);
        objectRenderer.Texture = this._animationInstance.CurrentTexture;
        objectRenderer.Render((Rectangle) source, (Rectangle) destination, this.FlipX, this.FlipY);
      }
    }
}
