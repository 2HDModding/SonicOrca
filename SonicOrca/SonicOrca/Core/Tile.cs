// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Tile
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core
{

    public class Tile : ITile
    {
      public const int Size = 64 /*0x40*/;
      public const int IndexMask = 4095 /*0x0FFF*/;
      public const int TileSetMask = 12288 /*0x3000*/;
      public const int FlipXMask = 16384 /*0x4000*/;
      public const int FlipYMask = 32768 /*0x8000*/;
      private readonly TileSet _tileSet;
      private readonly int _id;
      private readonly Tile.Frame[] _frames;
      private readonly TileBlendMode _blend;
      private int _currentFrameIndex;
      private int _frameTime;
      private float _opacity;
      private float _opacityChange;

      public int Id => this._id;

      public bool Animated => this._frames.Length > 1;

      public IReadOnlyList<Tile.Frame> Frames => (IReadOnlyList<Tile.Frame>) this._frames;

      public TileBlendMode Blend => this._blend;

      public Tile(TileSet tileSet, int id, IEnumerable<Tile.Frame> frames, TileBlendMode blend = TileBlendMode.Alpha)
      {
        this._tileSet = tileSet;
        this._id = id;
        this._frames = frames.ToArray<Tile.Frame>();
        this._blend = blend;
        if (this._frames.Length < 1)
          return;
        this._opacity = this._frames[0].Opacity;
        if (this._frames.Length < 2)
          return;
        this._opacityChange = (this._frames[1].Opacity - this._opacity) / (float) (this._frames[0].Delay + 1);
      }

      public void Animate()
      {
        if (this._frames.Length <= 1)
          return;
        Tile.Frame frame1 = this._frames[this._currentFrameIndex];
        if (this._frameTime >= frame1.Delay)
        {
          this._currentFrameIndex = (this._currentFrameIndex + 1) % this._frames.Length;
          this._frameTime = 0;
          frame1 = this._frames[this._currentFrameIndex];
          this._opacity = frame1.Opacity;
          int index = this._currentFrameIndex + 1;
          if (index >= this._frames.Length)
            index = 0;
          Tile.Frame frame2 = this._frames[index];
          if ((double) frame2.Opacity != (double) this._opacity && frame1.Delay > 0)
            this._opacityChange = (frame2.Opacity - this._opacity) / (float) (frame1.Delay + 1);
          else
            this._opacityChange = 0.0f;
        }
        else
        {
          ++this._frameTime;
          this._opacity += this._opacityChange;
        }
      }

      public void Draw(Renderer renderer, int flags, int x, int y)
      {
        ref Tile.Frame local = ref this._frames[this._currentFrameIndex];
        this.Draw(renderer, flags, (Rectanglei) new Rectangle(0.0, 0.0, 64.0, 64.0), (Rectanglei) new Rectangle((double) x, (double) y, 64.0, 64.0));
      }

      public void Draw(Renderer renderer, int flags, Rectanglei source, Rectanglei destination)
      {
        ITileRenderer tileRenderer = renderer.GetTileRenderer();
        if (tileRenderer.Rendering)
          this.Draw(tileRenderer, flags, source, destination);
        else
          this.Draw(renderer.Get2dRenderer(), flags, source, destination);
      }

      public void Draw(I2dRenderer g, int flags, Rectanglei source, Rectanglei destination)
      {
        Tile.Frame frame = this._frames[this._currentFrameIndex];
        source.X = (flags & 16384 /*0x4000*/) == 0 ? frame.X + source.X : frame.X + 64 /*0x40*/ - source.Width - source.X;
        source.Y = (flags & 32768 /*0x8000*/) == 0 ? frame.Y + source.Y : frame.Y + 64 /*0x40*/ - source.Height - source.Y;
        ITexture texture = this._tileSet.Textures[frame.TextureId];
        g.BlendMode = this._blend == TileBlendMode.Additive ? BlendMode.Additive : BlendMode.Alpha;
        g.Colour = new Colour((double) this._opacity, 1.0, 1.0, 1.0);
        g.RenderTexture(texture, (Rectangle) source, (Rectangle) destination, (flags & 16384 /*0x4000*/) != 0, (flags & 32768 /*0x8000*/) != 0);
      }

      public void Draw(
        ITileRenderer tileRenderer,
        int flags,
        Rectanglei source,
        Rectanglei destination)
      {
        Tile.Frame frame = this._frames[this._currentFrameIndex];
        source.X = (flags & 16384 /*0x4000*/) == 0 ? frame.X + source.X : frame.X + 64 /*0x40*/ - source.Width - source.X;
        source.Y = (flags & 32768 /*0x8000*/) == 0 ? frame.Y + source.Y : frame.Y + 64 /*0x40*/ - source.Height - source.Y;
        tileRenderer.AddTile(source, destination, frame.TextureId, (flags & 16384 /*0x4000*/) != 0, (flags & 32768 /*0x8000*/) != 0, this._opacity, this._blend);
      }

      public override string ToString()
      {
        return this._frames.Length != 1 ? string.Format("Id = {0} Frames = {1}", (object) this._frames.Length) : string.Format("Id = {0} TextureId = {1} X = {2} Y = {3}", (object) this._frames[0].TextureId, (object) this._frames[0].X, (object) this._frames[0].Y);
      }

      public struct Frame
      {
        public int TextureId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public float Opacity { get; set; }

        public int Delay { get; set; }

        public override string ToString()
        {
          return $"TextureId = {this.TextureId} X = {this.X} Y = {this.Y} Opacity = {this.Opacity} Delay = {this.Delay}";
        }
      }
    }
}
