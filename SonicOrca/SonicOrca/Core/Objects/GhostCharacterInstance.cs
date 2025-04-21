// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.GhostCharacterInstance
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Objects.Base;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System;

namespace SonicOrca.Core.Objects
{

    internal class GhostCharacterInstance : ActiveObject
    {
      private AnimationInstance _animation;

      public double Angle { get; set; }

      public CharacterState State { get; set; }

      protected override void OnStart() => this.Priority = 1022;

      public void Initialise(string animationResourceKey)
      {
        this.Initialise(this.Level.GameContext.ResourceTree.GetLoadedResource<AnimationGroup>(animationResourceKey));
      }

      public void Initialise(AnimationGroup animationGroup)
      {
        this._animation = new AnimationInstance(animationGroup);
      }

      public void Set(PlayRecorder.Entry entry)
      {
        this.Position = entry.Position;
        if (this.Level.Map.Layers.Count > entry.LayerIndex)
          this.Layer = this.Level.Map.Layers[entry.LayerIndex];
        this.State = (CharacterState) entry.State;
        if (this._animation != null)
        {
          this._animation.Index = entry.AnimationIndex;
          this._animation.CurrentFrameIndex = entry.AnimationFrameIndex;
        }
        this.Angle = (double) entry.Angle;
      }

      protected override void OnDraw(Renderer renderer, LayerViewOptions viewOptions)
      {
        if (viewOptions.Shadows || this._animation == null)
          return;
        I2dRenderer obj = renderer.Get2dRenderer();
        ICharacterRenderer characterRenderer = renderer.GetCharacterRenderer();
        characterRenderer.ModelMatrix = obj.ModelMatrix;
        if (this.Angle != 0.0)
          characterRenderer.ModelMatrix *= Matrix4.CreateRotationZ(this.Angle * Math.PI);
        Animation.Frame currentFrame = this._animation.CurrentFrame;
        Vector2 offset = (Vector2) currentFrame.Offset;
        Rectangle destination = new Rectangle(offset.X - (double) (currentFrame.Source.Width / 2), offset.Y - (double) (currentFrame.Source.Height / 2), (double) currentFrame.Source.Width, (double) currentFrame.Source.Height);
        characterRenderer.RenderTextureGhost(this._animation.AnimationGroup.Textures[1], this._animation.AnimationGroup.Textures[0], (Rectangle) currentFrame.Source, destination, !this.State.HasFlag((Enum) CharacterState.Left), this.State.HasFlag((Enum) CharacterState.Left) && this._animation.Index == 16 /*0x10*/);
      }
    }
}
