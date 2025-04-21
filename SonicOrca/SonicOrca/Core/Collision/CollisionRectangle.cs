// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Collision.CollisionRectangle
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;

namespace SonicOrca.Core.Collision
{

    public class CollisionRectangle : IBounds
    {
      private readonly ActiveObject _owner;

      public ActiveObject Owner => this._owner;

      public int Id { get; set; }

      public Rectanglei Bounds { get; set; }

      public Rectanglei AbsoluteBounds
      {
        get => this._owner != null ? this.Bounds.OffsetBy(this.Owner.Position) : this.Bounds;
      }

      public CollisionRectangle(ActiveObject owner, int id, int x, int y, int width, int height)
      {
        this._owner = owner;
        this.Id = id;
        this.Bounds = (Rectanglei) new Rectangle((double) x, (double) y, (double) width, (double) height);
      }

      public void OffsetBy(Vector2i offset)
      {
        Rectangle bounds = (Rectangle) this.Bounds;
        bounds.Location += (Vector2) offset;
        this.Bounds = (Rectanglei) bounds;
      }

      public bool IntersectsWith(CollisionRectangle other)
      {
        return this.AbsoluteBounds.IntersectsWith(other.AbsoluteBounds);
      }

      public void Draw(Renderer renderer, Viewport viewport)
      {
        if (!viewport.Bounds.IntersectsWith(this.AbsoluteBounds))
          return;
        Rectangle destination = (Rectangle) this.AbsoluteBounds.OffsetBy(viewport.Bounds.Location * -1);
        destination.X *= viewport.Scale.X;
        destination.Y *= viewport.Scale.Y;
        destination.Width *= viewport.Scale.X;
        destination.Height *= viewport.Scale.Y;
        I2dRenderer obj = renderer.Get2dRenderer();
        Colour colour = new Colour(byte.MaxValue, (byte) 0, (byte) 0);
        obj.RenderRectangle(colour, destination, 1.0);
        obj.RenderLine(colour, new Vector2(destination.X + destination.Width / 2.0, destination.Y), new Vector2(destination.X + destination.Width / 2.0, destination.Y + destination.Height), 1.0);
        obj.RenderLine(colour, new Vector2(destination.X, destination.Y + destination.Height / 2.0), new Vector2(destination.X + destination.Width, destination.Y + destination.Height / 2.0), 1.0);
      }
    }
}
