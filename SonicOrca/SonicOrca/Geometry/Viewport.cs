// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.Viewport
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Graphics;
using System;

namespace SonicOrca.Geometry
{

    public class Viewport
    {
      private Rectanglei _bounds;
      private Rectanglei _destination;
      private Vector2 _scale;

      public Viewport(Rectanglei destination)
      {
        this.Bounds = (Rectanglei) new Rectangle(0.0, 0.0, (double) destination.Width, (double) destination.Height);
        this.Destination = destination;
      }

      public Viewport(Rectanglei destination, Rectanglei bounds)
      {
        this.Destination = destination;
        this.Bounds = bounds;
      }

      public Rectanglei Bounds
      {
        get => this._bounds;
        set
        {
          this._bounds = value;
          this.CalculateScale();
        }
      }

      public Rectanglei Destination
      {
        get => this._destination;
        set
        {
          this._destination = value;
          this.CalculateScale();
        }
      }

      public Vector2 Scale => this._scale;

      private void CalculateScale()
      {
        Rectanglei rectanglei = this.Destination;
        double width1 = (double) rectanglei.Width;
        rectanglei = this.Bounds;
        double width2 = (double) rectanglei.Width;
        double x = width1 / width2;
        rectanglei = this.Destination;
        double height1 = (double) rectanglei.Height;
        rectanglei = this.Bounds;
        double height2 = (double) rectanglei.Height;
        double y = height1 / height2;
        this._scale = new Vector2(x, y);
      }

      public Vector2i GetAbsolutePosition(Vector2i position)
      {
        return this.GetAbsolutePosition(position.X, position.Y);
      }

      public Vector2i GetAbsolutePosition(int x, int y)
      {
        Vector2 scale = this.Scale;
        Rectanglei bounds = this.Bounds;
        int x1 = bounds.X + (int) ((double) x * scale.X);
        bounds = this.Bounds;
        int y1 = bounds.Y + (int) ((double) y * scale.Y);
        return new Vector2i(x1, y1);
      }

      public Vector2i GetRelativePosition(Vector2i position)
      {
        return this.GetRelativePosition(position.X, position.Y);
      }

      public Vector2i GetRelativePosition(int x, int y)
      {
        Vector2 scale = this.Scale;
        return new Vector2i((int) ((double) (x - this.Bounds.X) / scale.X), (int) ((double) (y - this.Bounds.Y) / scale.Y));
      }

      public IDisposable ApplyRendererState(Renderer renderer)
      {
        I2dRenderer obj = renderer.Get2dRenderer();
        IDisposable disposable = obj.BeginMatixState();
        obj.ClipRectangle = (Rectangle) this.Destination;
        Vector2 scale = this.Scale;
        Rectanglei destination = this.Destination;
        double x = (double) destination.X;
        destination = this.Destination;
        double y = (double) destination.Y;
        obj.ModelMatrix = Matrix4.CreateTranslation(x, y) * Matrix4.CreateScale(scale.X, scale.Y);
        return disposable;
      }
    }
}
