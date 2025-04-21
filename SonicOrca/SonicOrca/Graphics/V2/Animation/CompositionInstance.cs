// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Animation.CompositionInstance
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Graphics.V2.Animation
{

    public class CompositionInstance
    {
      private readonly CompositionGroup _compositionGroup;
      private int _ellapsedFrames;
      private int _currentRenderLayerIndex;
      private List<CompositionLayer> _activeLayers = new List<CompositionLayer>();
      private Composition _composition;
      private bool _playing;
      private bool _finished;
      private List<CompositionLayer> _maskedLayers = new List<CompositionLayer>();
      private List<CompositionLayer> _masks = new List<CompositionLayer>();

      public CompositionGroup CompositionGroup => this._compositionGroup;

      public bool AdditiveBlending { get; set; }

      public CompositionInstance(ResourceTree resourceTree, string resourceKey)
      {
        this._compositionGroup = resourceTree.GetLoadedResource<CompositionGroup>(resourceKey);
        this._composition = this._compositionGroup.First<Composition>();
      }

      public CompositionInstance(CompositionGroup compositionGroup)
      {
        this._compositionGroup = compositionGroup;
        this._composition = this._compositionGroup.First<Composition>();
      }

      public void ResetFrame()
      {
        this._ellapsedFrames = 0;
        this._playing = false;
        this._finished = false;
        foreach (CompositionLayer layer in (IEnumerable<CompositionLayer>) this._composition.Layers)
          layer.ResetFrame();
      }

      public bool Playing => this._playing;

      public bool Finished => this._finished;

      public void Animate()
      {
        if ((long) this._ellapsedFrames >= (long) this._composition.StartFrame && (long) this._ellapsedFrames <= (long) this._composition.EndFrame)
        {
          foreach (CompositionLayer layer in (IEnumerable<CompositionLayer>) this._composition.Layers)
            layer.Animate();
        }
        if ((long) this._ellapsedFrames > (long) this._composition.EndFrame)
        {
          this._playing = false;
          this._finished = true;
        }
        else
        {
          this._playing = true;
          ++this._ellapsedFrames;
        }
      }

      public void Seek(int ticks)
      {
        this.ResetFrame();
        for (int index = 0; index < ticks; ++index)
          this.Animate();
      }

      public void Draw(Renderer renderer, Vector2 offset = default (Vector2), bool flipX = false, bool flipY = false)
      {
        this.Draw(renderer, Colours.White, offset, flipX, flipY);
      }

      private Composition.Frame CurrentFrame
      {
        get
        {
          Composition.Frame currentFrame = new Composition.Frame();
          CompositionLayer currentLayer = this._activeLayers.ElementAt<CompositionLayer>(this._currentRenderLayerIndex);
          CompositionLayerAnimatableTransform transform = currentLayer.Transform;
          currentFrame.TextureIndex = ((IEnumerable<CompositionAsset>) this._compositionGroup.Assets).TakeWhile<CompositionAsset>((Func<CompositionAsset, bool>) (x => x.ID != currentLayer.TextureReference)).Count<CompositionAsset>();
          currentFrame.Source = new Rectanglei(0, 0, ((IEnumerable<CompositionAsset>) this._compositionGroup.Assets).ElementAt<CompositionAsset>(currentFrame.TextureIndex).Width, ((IEnumerable<CompositionAsset>) this._compositionGroup.Assets).ElementAt<CompositionAsset>(currentFrame.TextureIndex).Height);
          currentFrame.Opacity = transform.Opacity;
          currentFrame.Position = transform.Position;
          currentFrame.Rotation = transform.Rotation;
          currentFrame.Scale = transform.Scale;
          return currentFrame;
        }
      }

      private Composition.Frame GetMaskFrameForLayer(CompositionLayer layer)
      {
        Composition.Frame maskFrameForLayer = new Composition.Frame();
        CompositionLayer currentMask = this._masks.Select<CompositionLayer, CompositionLayer>((Func<CompositionLayer, CompositionLayer>) (m => m)).Where<CompositionLayer>((Func<CompositionLayer, bool>) (m => (int) m.Index == (int) layer.Index - 1 && m.SubKind == CompositionLayer.LayerSubKind.Mask)).First<CompositionLayer>();
        CompositionLayerAnimatableTransform transform = currentMask.Transform;
        maskFrameForLayer.TextureIndex = ((IEnumerable<CompositionAsset>) this._compositionGroup.Assets).TakeWhile<CompositionAsset>((Func<CompositionAsset, bool>) (x => x.ID != currentMask.TextureReference)).Count<CompositionAsset>();
        maskFrameForLayer.Source = new Rectanglei(0, 0, ((IEnumerable<CompositionAsset>) this._compositionGroup.Assets).ElementAt<CompositionAsset>(maskFrameForLayer.TextureIndex).Width, ((IEnumerable<CompositionAsset>) this._compositionGroup.Assets).ElementAt<CompositionAsset>(maskFrameForLayer.TextureIndex).Height);
        maskFrameForLayer.Opacity = transform.Opacity;
        maskFrameForLayer.Position = transform.Position;
        maskFrameForLayer.Rotation = transform.Rotation;
        maskFrameForLayer.Scale = transform.Scale;
        return maskFrameForLayer;
      }

      private List<CompositionLayer> GetActiveLayers()
      {
        IEnumerable<CompositionLayer> activeLayers = ((IEnumerable<CompositionLayer>) this._composition.Layers).Select<CompositionLayer, CompositionLayer>((Func<CompositionLayer, CompositionLayer>) (l => l)).Where<CompositionLayer>((Func<CompositionLayer, bool>) (l =>
        {
          if (((long) this._ellapsedFrames < (long) l.StartFrame || l.Transform.Tweens.Count <= 0 ? 0 : (l.Transform.Tweens.Any<CompositionLayerTween>((Func<CompositionLayerTween, bool>) (t => t.HasKeyFrames && (long) this._ellapsedFrames >= (long) t.StartFrame)) ? 1 : 0)) != 0)
            return true;
          return (long) this._ellapsedFrames >= (long) l.StartFrame && l.Transform.Tweens.All<CompositionLayerTween>((Func<CompositionLayerTween, bool>) (t => !t.HasKeyFrames));
        }));
        this._masks.AddRange(activeLayers.Select<CompositionLayer, CompositionLayer>((Func<CompositionLayer, CompositionLayer>) (m => m)).Where<CompositionLayer>((Func<CompositionLayer, bool>) (m => m.SubKind == CompositionLayer.LayerSubKind.Mask)));
        this._maskedLayers.AddRange(activeLayers.Select<CompositionLayer, CompositionLayer>((Func<CompositionLayer, CompositionLayer>) (l => l)).Where<CompositionLayer>((Func<CompositionLayer, bool>) (l => activeLayers.Any<CompositionLayer>((Func<CompositionLayer, bool>) (m => m.SubKind == CompositionLayer.LayerSubKind.Mask && (int) m.Index == (int) l.Index - 1)))));
        this._activeLayers.AddRange(activeLayers.Select<CompositionLayer, CompositionLayer>((Func<CompositionLayer, CompositionLayer>) (l => l)).Where<CompositionLayer>((Func<CompositionLayer, bool>) (l => l.SubKind == CompositionLayer.LayerSubKind.None && !((IEnumerable<CompositionLayer>) this._composition.Layers).Any<CompositionLayer>((Func<CompositionLayer, bool>) (m => m.SubKind == CompositionLayer.LayerSubKind.Mask && (int) m.Index == (int) l.Index - 1)))));
        this._activeLayers.AddRange((IEnumerable<CompositionLayer>) this._maskedLayers);
        this._activeLayers = this._activeLayers.OrderByDescending<CompositionLayer, uint>((Func<CompositionLayer, uint>) (l => l.Index)).ToList<CompositionLayer>();
        return this._activeLayers;
      }

      public void Draw(Renderer renderer, Colour colour, Vector2 offset = default (Vector2), bool flipX = false, bool flipY = false)
      {
        this._masks.Clear();
        this._maskedLayers.Clear();
        this._activeLayers.Clear();
        this._activeLayers = this.GetActiveLayers();
        for (int index = 0; index < this._activeLayers.Count<CompositionLayer>(); ++index)
        {
          this._currentRenderLayerIndex = index;
          Composition.Frame currentFrame = this.CurrentFrame;
          Rectangle targetDestination;
          ref Rectangle local1 = ref targetDestination;
          double x1 = offset.X;
          Rectanglei source = currentFrame.Source;
          double num1 = (double) source.Width * 0.5;
          double x2 = x1 - num1;
          double y1 = offset.Y;
          source = currentFrame.Source;
          double num2 = (double) source.Height * 0.5;
          double y2 = y1 - num2;
          source = currentFrame.Source;
          double width1 = (double) source.Width;
          source = currentFrame.Source;
          double height1 = (double) source.Height;
          local1 = new Rectangle(x2, y2, width1, height1);
          this.AdditiveBlending = this._activeLayers.ElementAt<CompositionLayer>(index).BlendMode == BlendMode.Additive;
          if (!this._maskedLayers.Contains(this._activeLayers.ElementAt<CompositionLayer>(index)))
          {
            this.Draw(renderer.GetMaskRenderer(), colour, (Rectangle) currentFrame.Source, targetDestination, flipX, flipY);
          }
          else
          {
            IMaskRenderer maskRenderer = renderer.GetMaskRenderer();
            Composition.Frame maskFrameForLayer = this.GetMaskFrameForLayer(this._activeLayers.ElementAt<CompositionLayer>(index));
            Rectangle maskDestination;
            ref Rectangle local2 = ref maskDestination;
            double x3 = offset.X;
            source = maskFrameForLayer.Source;
            double num3 = (double) source.Width * 0.5;
            Vector2 scale = maskFrameForLayer.Scale;
            double num4 = scale.X * 0.01;
            double num5 = num3 * num4;
            double x4 = x3 - num5;
            double y3 = offset.Y;
            source = maskFrameForLayer.Source;
            double num6 = (double) source.Height * 0.5;
            scale = maskFrameForLayer.Scale;
            double num7 = scale.Y * 0.01;
            double num8 = num6 * num7;
            double y4 = y3 - num8;
            source = maskFrameForLayer.Source;
            double width2 = (double) source.Width;
            scale = maskFrameForLayer.Scale;
            double x5 = scale.X;
            double width3 = width2 * x5 * 0.01;
            source = maskFrameForLayer.Source;
            double height2 = (double) source.Height * maskFrameForLayer.Scale.Y * 0.01;
            local2 = new Rectangle(x4, y4, width3, height2);
            this.Draw(maskRenderer, maskFrameForLayer, colour, (Rectangle) maskFrameForLayer.Source, maskDestination, (Rectangle) currentFrame.Source, targetDestination, flipX, flipY);
          }
        }
      }

      public void Draw(
        I2dRenderer renderer,
        Colour colour,
        Rectangle source,
        Rectangle destination,
        bool flipX = false,
        bool flipY = false)
      {
        Composition.Frame currentFrame = this.CurrentFrame;
        ITexture texture = this._compositionGroup.Textures[currentFrame.TextureIndex];
        using (renderer.BeginMatixState())
        {
          Matrix4 matrix4_1 = Matrix4.Identity * Matrix4.CreateTranslation(currentFrame.Position);
          Vector2 scale1 = currentFrame.Scale;
          double x = scale1.X * 0.01;
          scale1 = currentFrame.Scale;
          double y = scale1.Y * 0.01;
          Matrix4 scale2 = Matrix4.CreateScale(x, y);
          Matrix4 matrix4_2 = (matrix4_1 * scale2).RotateZ(currentFrame.Rotation);
          colour.Alpha = (byte) (currentFrame.Opacity * 0.01 * 256.0);
          renderer.ModelMatrix = matrix4_2;
          renderer.Colour = colour;
          renderer.BlendMode = this.AdditiveBlending ? BlendMode.Additive : BlendMode.Alpha;
          renderer.RenderTexture(texture, source, destination, flipX, flipY);
        }
      }

      public void Draw(
        IMaskRenderer renderer,
        Colour colour,
        Rectangle targetSource,
        Rectangle targetDestination,
        bool flipX = false,
        bool flipY = false)
      {
        Composition.Frame currentFrame = this.CurrentFrame;
        ITexture texture = this._compositionGroup.Textures[currentFrame.TextureIndex];
        using (renderer.BeginMatixState())
        {
          double num1 = 1.0;
          double num2 = 1.0;
          if (flipX)
            num1 = -1.0;
          if (flipY)
            num2 = -1.0;
          Matrix4 matrix4_1 = Matrix4.Identity * Matrix4.CreateTranslation(currentFrame.Position);
          Vector2 scale1 = currentFrame.Scale;
          double x = scale1.X * 0.01 * num1;
          scale1 = currentFrame.Scale;
          double y = scale1.Y * 0.01 * num2;
          Matrix4 scale2 = Matrix4.CreateScale(x, y);
          Matrix4 matrix4_2 = (matrix4_1 * scale2).RotateZ(currentFrame.Rotation);
          renderer.BlendMode = this.AdditiveBlending ? BlendMode.Additive : BlendMode.Alpha;
          renderer.Colour = colour;
          renderer.TargetModelMatrix = matrix4_2;
          renderer.Source = (Rectanglei) targetSource;
          renderer.Destination = (Rectanglei) targetDestination;
          renderer.Texture = texture;
          renderer.MaskTexture = (ITexture) null;
          renderer.Render();
        }
      }

      public void Draw(
        IMaskRenderer renderer,
        Composition.Frame maskFrame,
        Colour colour,
        Rectangle maskSource,
        Rectangle maskDestination,
        Rectangle targetSource,
        Rectangle targetDestination,
        bool flipX = false,
        bool flipY = false)
      {
        Composition.Frame currentFrame = this.CurrentFrame;
        ITexture texture1 = this._compositionGroup.Textures[currentFrame.TextureIndex];
        ITexture texture2 = this._compositionGroup.Textures[maskFrame.TextureIndex];
        using (renderer.BeginMatixState())
        {
          Matrix4 matrix4_1 = Matrix4.Identity * Matrix4.CreateTranslation(maskFrame.Position);
          Matrix4 matrix4_2 = Matrix4.Identity * Matrix4.CreateTranslation(new Vector2(0.5));
          Vector2 scale1 = currentFrame.Scale;
          double x = 1.0 / (scale1.X * 0.01);
          scale1 = currentFrame.Scale;
          double y = 1.0 / (scale1.Y * 0.01);
          Matrix4 scale2 = Matrix4.CreateScale(x, y);
          Matrix4 matrix4_3 = matrix4_2 * scale2 * Matrix4.CreateRotationZ(0.0) * Matrix4.CreateTranslation(new Vector2(-0.5));
          Matrix4 matrix4_4 = Matrix4.Identity * Matrix4.CreateTranslation(new Vector2(0.5)) * Matrix4.CreateRotationZ(maskFrame.Rotation) * Matrix4.CreateTranslation(new Vector2(-0.5));
          renderer.BlendMode = this.AdditiveBlending ? BlendMode.Additive : BlendMode.Alpha;
          renderer.Colour = colour;
          renderer.IntersectionModelMatrix = matrix4_1;
          renderer.MaskModelMatrix = matrix4_4;
          renderer.MaskSource = (Rectanglei) maskSource;
          renderer.MaskDestination = (Rectanglei) maskDestination;
          renderer.MaskTexture = texture2;
          renderer.TargetModelMatrix = matrix4_3;
          renderer.Source = (Rectanglei) targetSource;
          renderer.Destination = (Rectanglei) targetDestination;
          renderer.Texture = texture1;
          renderer.Render();
        }
      }
    }
}
