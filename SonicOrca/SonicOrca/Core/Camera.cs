// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Camera
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Debugging;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System;

namespace SonicOrca.Core
{

    public class Camera
    {
      private readonly Level _level;
      private Rectangle _bounds;
      private Rectangle _trackBounds;
      private Vector2 _velocity;

      public Rectangle ScreenBounds { get; set; }

      public ActiveObject ObjectToTrack { get; set; }

      public Vector2 Acceleration { get; set; }

      public Vector2 Deceleration { get; set; }

      public Vector2 MaxVelocity { get; set; }

      public Rectanglei Limits { get; set; }

      public Rectangle Bounds
      {
        get => this._bounds;
        set => this._bounds = value;
      }

      public Vector2 Velocity => this._velocity;

      public Vector2 Scale
      {
        get
        {
          Rectangle rectangle = this.ScreenBounds;
          double width1 = rectangle.Width;
          rectangle = this.Bounds;
          double width2 = rectangle.Width;
          double x = width1 / width2;
          rectangle = this.ScreenBounds;
          double height1 = rectangle.Height;
          rectangle = this.Bounds;
          double height2 = rectangle.Height;
          double y = height1 / height2;
          return new Vector2(x, y);
        }
      }

      public bool ShowDebugInformation { get; set; }

      public bool SpyMode { get; set; }

      public Camera(Level level)
      {
        this._level = level;
        this._trackBounds = new Rectangle(0.0, 0.0, 1920.0, 1080.0);
        this._bounds = new Rectangle(0.0, 0.0, 1920.0, 1080.0);
        this.ScreenBounds = new Rectangle(0.0, 0.0, 1920.0, 1080.0);
        this.Acceleration = new Vector2(0.0, 0.0);
        this.Deceleration = new Vector2(0.5, 0.5);
        this._velocity = new Vector2(0.0, 0.0);
        this.MaxVelocity = new Vector2(64.0, 64.0);
        this.SetScale(1.0);
      }

      public virtual void Update()
      {
        if (this.SpyMode)
          this.UpdateSpyMode();
        else
          this.UpdateTrackMode();
      }

      public void SetScale(double value)
      {
        this._bounds.Width = this.ScreenBounds.Width / value;
        this._bounds.Height = this.ScreenBounds.Height / value;
        this._trackBounds.Width = this.Bounds.Width;
        this._trackBounds.Height = this.Bounds.Height;
      }

      public void CentreObjectToTrack()
      {
        if (this.ObjectToTrack == null)
          return;
        CameraProperties cameraProperties = this.ObjectToTrack.CameraProperties;
        Vector2 position = (Vector2) this.ObjectToTrack.Position;
        this._trackBounds.X = position.X - this._trackBounds.Width / 2.0;
        this._trackBounds.Y = position.Y - this._trackBounds.Height / 2.0;
        this._bounds.X = this._trackBounds.X + cameraProperties.Offset.X;
        this._bounds.Y = this._trackBounds.Y + cameraProperties.Offset.Y;
        this.ApplyBounds();
      }

      public void Shift(int x, int y)
      {
        this._bounds.X += (double) x;
        this._bounds.Y += (double) y;
        this.ApplyBounds();
      }

      protected void ApplyMovement()
      {
        Vector2 vector2;
        if (this.Acceleration.X == 0.0)
        {
          ref Vector2 local = ref this._velocity;
          double x = this._velocity.X;
          vector2 = this.Deceleration;
          double amount = -vector2.X;
          double num = MathX.ChangeSpeed(x, amount);
          local.X = num;
        }
        vector2 = this.Acceleration;
        if (vector2.Y == 0.0)
        {
          ref Vector2 local = ref this._velocity;
          double y = this._velocity.Y;
          vector2 = this.Deceleration;
          double amount = -vector2.Y;
          double num = MathX.ChangeSpeed(y, amount);
          local.Y = num;
        }
        this._velocity += this.Acceleration;
        ref Vector2 local1 = ref this._velocity;
        double x1 = this._velocity.X;
        vector2 = this.MaxVelocity;
        double x2 = vector2.X;
        double num1 = MathX.Clamp(x1, x2);
        local1.X = num1;
        ref Vector2 local2 = ref this._velocity;
        double y1 = this._velocity.Y;
        vector2 = this.MaxVelocity;
        double y2 = vector2.Y;
        double num2 = MathX.Clamp(y1, y2);
        local2.Y = num2;
        this._bounds.X += this._velocity.X;
        this._bounds.Y += this._velocity.Y;
      }

      protected void ApplyBounds()
      {
        double x = this.Bounds.X;
        double y = this.Bounds.Y;
        Rectanglei rectanglei = this.Limits == Rectanglei.Empty ? this._level.Bounds : this.Limits;
        this._bounds.X = Math.Max(this.Bounds.X, (double) rectanglei.X);
        ref Rectangle local1 = ref this._bounds;
        Rectangle bounds = this.Bounds;
        double num1 = Math.Max(bounds.Y, (double) rectanglei.Y);
        local1.Y = num1;
        ref Rectangle local2 = ref this._bounds;
        bounds = this.Bounds;
        double num2 = Math.Min(bounds.Right, (double) rectanglei.Right);
        bounds = this.Bounds;
        double width = bounds.Width;
        double num3 = num2 - width;
        local2.X = num3;
        ref Rectangle local3 = ref this._bounds;
        bounds = this.Bounds;
        double num4 = Math.Min(bounds.Bottom, (double) rectanglei.Bottom);
        bounds = this.Bounds;
        double height = bounds.Height;
        double num5 = num4 - height;
        local3.Y = num5;
        Vector2 velocity = this.Velocity;
        bounds = this.Bounds;
        if (bounds.X != x)
          velocity.X = 0.0;
        bounds = this.Bounds;
        if (bounds.Y == y)
          return;
        velocity.Y = 0.0;
      }

      private void UpdateSpyMode()
      {
        this.Acceleration = new Vector2(0.0, 0.0);
        if (!this._level.DebugContext.Visible)
          this.Acceleration = new Vector2(this._level.GameContext.Current[0].DirectionLeft.X * 0.5, this._level.GameContext.Current[0].DirectionLeft.Y * 0.5);
        this.ApplyMovement();
        this.ApplyBounds();
      }

      private void UpdateTrackMode()
      {
        this._velocity = new Vector2(0.0, 0.0);
        if (this.ObjectToTrack == null)
          return;
        CameraProperties cameraProperties1 = this.ObjectToTrack.CameraProperties;
        Rectangle box = cameraProperties1.Box;
        box.X += this._trackBounds.Width / 2.0;
        box.Y += this._trackBounds.Height / 2.0;
        Vector2 position = (Vector2) this.ObjectToTrack.Position;
        position.X -= this._trackBounds.X;
        position.Y -= this._trackBounds.Y;
        Vector2i vector2i1 = cameraProperties1.Delay;
        Vector2 vector2;
        if (vector2i1.X == 0)
        {
          if (position.X < box.X)
            this._velocity.X = position.X - box.X;
          else if (position.X > box.Right)
            this._velocity.X = position.X - box.Right;
          ref Vector2 local = ref this._velocity;
          double x1 = this._velocity.X;
          vector2 = cameraProperties1.MaxVelocity;
          double x2 = vector2.X;
          double num = MathX.Clamp(x1, x2);
          local.X = num;
        }
        else
        {
          CameraProperties cameraProperties2 = cameraProperties1;
          vector2i1 = cameraProperties1.Delay;
          int x = vector2i1.X - 1;
          vector2i1 = cameraProperties1.Delay;
          int y = vector2i1.Y;
          Vector2i vector2i2 = new Vector2i(x, y);
          cameraProperties2.Delay = vector2i2;
        }
        vector2i1 = cameraProperties1.Delay;
        if (vector2i1.Y == 0)
        {
          if (position.Y < box.Y)
            this._velocity.Y = position.Y - box.Y;
          else if (position.Y > box.Bottom)
            this._velocity.Y = position.Y - box.Bottom;
          ref Vector2 local = ref this._velocity;
          double y1 = this._velocity.Y;
          vector2 = cameraProperties1.MaxVelocity;
          double y2 = vector2.Y;
          double num = MathX.Clamp(y1, y2);
          local.Y = num;
        }
        else
        {
          CameraProperties cameraProperties3 = cameraProperties1;
          vector2i1 = cameraProperties1.Delay;
          int x = vector2i1.X;
          vector2i1 = cameraProperties1.Delay;
          int y = vector2i1.Y - 1;
          Vector2i vector2i3 = new Vector2i(x, y);
          cameraProperties3.Delay = vector2i3;
        }
        Vector2 maxVelocity = cameraProperties1.MaxVelocity;
        ref Vector2 local1 = ref maxVelocity;
        double x3 = maxVelocity.X;
        vector2 = this.MaxVelocity;
        double x4 = vector2.X;
        double num1 = Math.Min(x3, x4);
        local1.X = num1;
        ref Vector2 local2 = ref maxVelocity;
        double y3 = maxVelocity.Y;
        vector2 = this.MaxVelocity;
        double y4 = vector2.Y;
        double num2 = Math.Min(y3, y4);
        local2.Y = num2;
        this._trackBounds.X += this._velocity.X;
        this._trackBounds.Y += this._velocity.Y;
        vector2 = cameraProperties1.MaxVelocity;
        if (vector2.X == 0.0)
        {
          ref Rectangle local3 = ref this._bounds;
          vector2i1 = this.ObjectToTrack.Position;
          double num3 = (double) vector2i1.X - this._trackBounds.Width / 2.0;
          local3.X = num3;
        }
        else
        {
          ref Rectangle local4 = ref this._bounds;
          double x5 = this._bounds.X;
          double x6 = this._trackBounds.X;
          vector2 = cameraProperties1.Offset;
          double x7 = vector2.X;
          double dest = x6 + x7;
          double x8 = maxVelocity.X;
          double num4 = MathX.GoTowards(x5, dest, x8);
          local4.X = num4;
        }
        vector2 = cameraProperties1.MaxVelocity;
        if (vector2.Y == 0.0)
        {
          ref Rectangle local5 = ref this._bounds;
          vector2i1 = this.ObjectToTrack.Position;
          double num5 = (double) vector2i1.Y - this._trackBounds.Height / 2.0;
          local5.Y = num5;
        }
        else
        {
          ref Rectangle local6 = ref this._bounds;
          double y5 = this._bounds.Y;
          double y6 = this._trackBounds.Y;
          vector2 = cameraProperties1.Offset;
          double y7 = vector2.Y;
          double dest = y6 + y7;
          double y8 = maxVelocity.Y;
          double num6 = MathX.GoTowards(y5, dest, y8);
          local6.Y = num6;
        }
        this.ApplyBounds();
      }

      public virtual void Draw(Renderer renderer)
      {
        this.DrawBounds(renderer, this.ScreenBounds, this.Bounds);
        if (this.ShowDebugInformation)
        {
          this.DrawSafeAreaGuides(renderer);
          if (this.ObjectToTrack != null && !this.SpyMode)
            this.DrawTracking(renderer);
          else
            this.DrawCrossHair(renderer);
          this.DrawPosition(renderer);
        }
        if (!this.SpyMode)
          return;
        this.DrawSpyCameraModeLabel(renderer);
      }

      protected void DrawBounds(Renderer renderer, Rectangle screenBounds, Rectangle bounds)
      {
      }

      protected void DrawSafeAreaGuides(Renderer renderer)
      {
			I2dRenderer i2dRenderer = renderer.Get2dRenderer();
			double num = this.ScreenBounds.Width * 0.019999999552965164;
			double num2 = Math.Min(this.ScreenBounds.Width, this.ScreenBounds.Height) / 3.0;
			Rectangle rectangle = new Rectangle(this.ScreenBounds.X + num, this.ScreenBounds.Y + num, this.ScreenBounds.Width - num * 2.0, this.ScreenBounds.Height - num * 2.0);
			i2dRenderer.RenderLine(Colours.White, new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X + num2, rectangle.Y), 1.0);
			i2dRenderer.RenderLine(Colours.White, new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X, rectangle.Y + num2), 1.0);
			i2dRenderer.RenderLine(Colours.White, new Vector2(rectangle.Right, rectangle.Y), new Vector2(rectangle.Right - num2, rectangle.Y), 1.0);
			i2dRenderer.RenderLine(Colours.White, new Vector2(rectangle.Right, rectangle.Y), new Vector2(rectangle.Right, rectangle.Y + num2), 1.0);
			i2dRenderer.RenderLine(Colours.White, new Vector2(rectangle.X, rectangle.Bottom), new Vector2(rectangle.X, rectangle.Bottom - num2), 1.0);
			i2dRenderer.RenderLine(Colours.White, new Vector2(rectangle.X, rectangle.Bottom), new Vector2(rectangle.X + num2, rectangle.Bottom), 1.0);
			i2dRenderer.RenderLine(Colours.White, new Vector2(rectangle.Right, rectangle.Bottom), new Vector2(rectangle.Right - num2, rectangle.Bottom), 1.0);
			i2dRenderer.RenderLine(Colours.White, new Vector2(rectangle.Right, rectangle.Bottom), new Vector2(rectangle.Right, rectangle.Bottom - num2), 1.0);
      }

      protected void DrawCrossHair(Renderer renderer)
      {
        I2dRenderer obj = renderer.Get2dRenderer();
        Rectangle screenBounds1 = this.ScreenBounds;
        double width = screenBounds1.Width;
        screenBounds1 = this.ScreenBounds;
        double height = screenBounds1.Height;
        double num = Math.Min(width, height) / 3.0;
        Colour white1 = Colours.White;
        Rectangle screenBounds2 = this.ScreenBounds;
        double centreX1 = screenBounds2.CentreX;
        screenBounds2 = this.ScreenBounds;
        double y1 = screenBounds2.CentreY - num;
        Vector2 a1 = new Vector2(centreX1, y1);
        screenBounds2 = this.ScreenBounds;
        double centreX2 = screenBounds2.CentreX;
        screenBounds2 = this.ScreenBounds;
        double y2 = screenBounds2.CentreY + num;
        Vector2 b1 = new Vector2(centreX2, y2);
        obj.RenderLine(white1, a1, b1, 1.0);
        Colour white2 = Colours.White;
        Rectangle screenBounds3 = this.ScreenBounds;
        double x1 = screenBounds3.CentreX - num;
        screenBounds3 = this.ScreenBounds;
        double centreY1 = screenBounds3.CentreY;
        Vector2 a2 = new Vector2(x1, centreY1);
        screenBounds3 = this.ScreenBounds;
        double x2 = screenBounds3.CentreX + num;
        screenBounds3 = this.ScreenBounds;
        double centreY2 = screenBounds3.CentreY;
        Vector2 b2 = new Vector2(x2, centreY2);
        obj.RenderLine(white2, a2, b2, 1.0);
      }

      protected void DrawTracking(Renderer renderer)
      {
        if (this.ObjectToTrack == null)
          return;
        Rectangle box = this.ObjectToTrack.CameraProperties.Box;
        box.X += this._trackBounds.X - this._bounds.X + this._trackBounds.Width / 2.0;
        box.Y += this._trackBounds.Y - this._bounds.Y + this._trackBounds.Height / 2.0;
        Vector2 position = (Vector2) this.ObjectToTrack.Position;
        position.X -= this._bounds.X;
        position.Y -= this._bounds.Y;
        I2dRenderer obj = renderer.Get2dRenderer();
        obj.RenderRectangle(Colours.White, box, 1.0);
        obj.RenderLine(Colours.White, new Vector2(position.X, position.Y - 16.0), new Vector2(position.X, position.Y + 16.0), 1.0);
        obj.RenderLine(Colours.White, new Vector2(position.X - 16.0, position.Y), new Vector2(position.X + 16.0, position.Y), 1.0);
      }

      protected void DrawPosition(Renderer renderer)
      {
        DebugContext debugContext1 = this._level.DebugContext;
        Renderer renderer1 = renderer;
        string text1 = $"{this._bounds.Left:0.0}, {this._bounds.Top:0.0}";
        Rectangle screenBounds1 = this.ScreenBounds;
        double x1 = screenBounds1.Left + 8.0;
        screenBounds1 = this.ScreenBounds;
        double y1 = screenBounds1.Top + 8.0;
        int? overlay1 = new int?(0);
        debugContext1.DrawText(renderer1, text1, FontAlignment.Left, x1, y1, 0.75, overlay1);
        DebugContext debugContext2 = this._level.DebugContext;
        Renderer renderer2 = renderer;
        string text2 = $"{this._bounds.Right:0.0}, {this._bounds.Bottom:0.0}";
        Rectangle screenBounds2 = this.ScreenBounds;
        double x2 = screenBounds2.Right - 8.0;
        screenBounds2 = this.ScreenBounds;
        double y2 = screenBounds2.Bottom - 8.0;
        int? overlay2 = new int?(0);
        debugContext2.DrawText(renderer2, text2, FontAlignment.Right | FontAlignment.Bottom, x2, y2, 0.75, overlay2);
      }

      protected void DrawSpyCameraModeLabel(Renderer renderer)
      {
        DebugContext debugContext = this._level.DebugContext;
        Renderer renderer1 = renderer;
        Rectangle screenBounds = this.ScreenBounds;
        double x = screenBounds.Right - 8.0;
        screenBounds = this.ScreenBounds;
        double y = screenBounds.Top + 8.0;
        int? overlay = new int?(0);
        debugContext.DrawText(renderer1, "SPY CAMERA", FontAlignment.Right, x, y, 0.75, overlay);
      }
    }
}
