// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.AnimationInstance
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Resources;
using System.Collections.Generic;

namespace SonicOrca.Graphics
{

    public class AnimationInstance
    {
      private readonly AnimationGroup _animationGroup;
      private int _animationIndex;
      private int _frameTime;

      public int CurrentFrameIndex { get; set; }

      public int Cycles { get; set; }

      public int? OverrideDelay { get; set; }

      public int? OverrideTextureIndex { get; set; }

      public bool AdditiveBlending { get; set; }

      public Animation Animation => this._animationGroup[this._animationIndex];

      public AnimationGroup AnimationGroup => this._animationGroup;

      public Animation.Frame CurrentFrame
      {
        get => this._animationGroup[this._animationIndex].Frames[this.CurrentFrameIndex];
      }

      public ITexture CurrentTexture
      {
        get
        {
          return this._animationGroup.Textures[this.OverrideTextureIndex ?? this.CurrentFrame.TextureIndex];
        }
      }

      public int Index
      {
        get => this._animationIndex;
        set
        {
          if (this._animationIndex == value)
            return;
          this._animationIndex = value;
          this.CurrentFrameIndex = 0;
          this._frameTime = 0;
        }
      }

      public AnimationInstance(ResourceTree resourceTree, string resourceKey, int index = 0)
      {
        this._animationGroup = resourceTree.GetLoadedResource<AnimationGroup>(resourceKey);
        this._animationIndex = index;
      }

      public AnimationInstance(AnimationGroup animationGroup, int index = 0)
      {
        this._animationGroup = animationGroup;
        this._animationIndex = index;
      }

      public void ResetFrame()
      {
        this.CurrentFrameIndex = 0;
        this._frameTime = 0;
      }

      public void Animate()
      {
        Animation animation = this._animationGroup[this._animationIndex];
        if (this._frameTime >= (this.OverrideDelay ?? animation.Frames[this.CurrentFrameIndex].Delay))
        {
          ++this.CurrentFrameIndex;
          if (this.CurrentFrameIndex >= ((IReadOnlyCollection<Animation.Frame>) animation.Frames).Count)
          {
            if (animation.LoopFrameIndex.HasValue)
              this.CurrentFrameIndex = animation.LoopFrameIndex.Value;
            else if (animation.NextFrameIndex.HasValue)
              this.Index = animation.NextFrameIndex.Value;
            else
              this.CurrentFrameIndex = 0;
            ++this.Cycles;
          }
          this._frameTime = 0;
        }
        else
          ++this._frameTime;
      }

      public void Seek(int ticks)
      {
        for (int index = 0; index < ticks; ++index)
          this.Animate();
      }

      public void Draw(I2dRenderer renderer, Vector2 position = default (Vector2), bool flipX = false, bool flipY = false)
      {
        this.Draw(renderer, Colours.White, position, flipX, flipY);
      }

      public void Draw(I2dRenderer renderer, Colour colour, Vector2 position = default (Vector2), bool flipX = false, bool flipY = false)
      {
        Animation.Frame currentFrame = this.CurrentFrame;
        Rectangle destination;
        ref Rectangle local = ref destination;
        double x = position.X - (double) (currentFrame.Source.Width / 2);
        double y = position.Y - (double) (currentFrame.Source.Height / 2);
        Rectanglei source = currentFrame.Source;
        double width = (double) source.Width;
        source = currentFrame.Source;
        double height = (double) source.Height;
        local = new Rectangle(x, y, width, height);
        this.Draw(renderer, colour, destination, flipX, flipY);
      }

      public void Draw(
        I2dRenderer renderer,
        Colour colour,
        Rectangle destination,
        bool flipX = false,
        bool flipY = false)
      {
        this.Draw(renderer, colour, (Rectangle) this.CurrentFrame.Source, destination, flipX, flipY);
      }

      public void Draw(
        I2dRenderer renderer,
        Colour colour,
        Rectangle source,
        Rectangle destination,
        bool flipX = false,
        bool flipY = false)
      {
        Animation.Frame currentFrame = this.CurrentFrame;
        ITexture texture = this._animationGroup.Textures[this.OverrideTextureIndex ?? currentFrame.TextureIndex];
        destination = destination.OffsetBy((Vector2) currentFrame.Offset);
        renderer.Colour = colour;
        renderer.BlendMode = this.AdditiveBlending ? BlendMode.Additive : BlendMode.Alpha;
        renderer.RenderTexture(texture, source, destination, flipX, flipY);
        renderer.BlendMode = BlendMode.Alpha;
      }
    }
}
