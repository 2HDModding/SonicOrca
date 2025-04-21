// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.Animal
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using SonicOrca.Core.Objects.Metadata;
using SonicOrca.Extensions;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System;

namespace SonicOrca.Core.Objects.Base
{

    public abstract class Animal : ActiveObject
    {
      private readonly string _animationGroupResourceKey;
      private AnimationInstance _animationInstance;
      private Vector2 _velocity;
      private bool _jumping;
      private int _delay;
      private int _direction = -1;

      [StateVariable]
      public int Delay
      {
        get => this._delay;
        set => this._delay = value;
      }

      [StateVariable]
      public int Direction
      {
        get => this._direction;
        set => this._direction = value;
      }

      protected Vector2 JumpVelocity { get; set; }

      protected double JumpGravity { get; set; }

      protected AnimationInstance AnimationInstance => this._animationInstance;

      protected Vector2 Velocity => this._velocity;

      public Animal(string animationGroupResourceKey)
      {
        this._animationGroupResourceKey = animationGroupResourceKey;
        this.JumpGravity = 0.375;
      }

      protected override void OnStart()
      {
        this._animationInstance = new AnimationInstance(this.ResourceTree, this.Type.GetAbsolutePath(this._animationGroupResourceKey));
        Vector2 jumpVelocity = this.JumpVelocity;
        double x = Math.Abs(jumpVelocity.X) * (double) Math.Sign(this._direction);
        jumpVelocity = this.JumpVelocity;
        double y = jumpVelocity.Y;
        this.JumpVelocity = new Vector2(x, y);
        this.Priority = this._delay > 0 ? -256 : 1280 /*0x0500*/;
      }

      protected override void OnUpdate()
      {
        if (this._delay > 0)
        {
          this.Priority = -256;
          --this._delay;
          if (this._delay != 0)
            return;
          this._velocity = new Vector2(0.0, -16.0);
        }
        else
        {
          this.Priority = 1280 /*0x0500*/;
          this.PositionPrecise = this.PositionPrecise + this._velocity;
          bool flag = false;
          foreach (CollisionInfo collision in CollisionVector.GetCollisions((ActiveObject) this, 32 /*0x20*/))
          {
            if (!collision.Vector.IsWall && collision.Vector.Mode != CollisionMode.Bottom)
            {
              ActiveObject owner = collision.Vector.Owner;
              if (owner == null || owner.Type.Classification != ObjectClassification.Capsule)
              {
                this.PositionPrecise = new Vector2(this.PositionPrecise.X, this.PositionPrecise.Y + collision.Shift);
                flag = true;
              }
            }
          }
          if (flag)
          {
            this._jumping = true;
            this._velocity = this.JumpVelocity;
            this._animationInstance.Index = 1;
          }
          else
            this._velocity.Y += this._jumping ? this.JumpGravity : 0.875;
        }
      }

      protected override void OnAnimate()
      {
        if (this._delay > 0)
          return;
        this._animationInstance.Animate();
      }

      protected override void OnDraw(Renderer renderer, LayerViewOptions viewOptions)
      {
        renderer.GetObjectRenderer().Render(this._animationInstance, this._velocity.X > 0.0);
      }

      protected override void OnStop() => this.FinishForever();
    }
}
