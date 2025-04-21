// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ActiveObject
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using SonicOrca.Core.Lighting;
using SonicOrca.Core.Objects.Metadata;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.Linq;

namespace SonicOrca.Core
{

    public abstract class ActiveObject : IActiveObject
    {
      private Vector2i _position;
      private Vector2 _positionPrecise;
      private Vector2i _lastPosition;
      private Vector2 _lastPositionPrecise;
      private bool[] _activeLayers;

      public string Name { get; set; }

      public Guid Uid { get; set; }

      public Level Level { get; private set; }

      public ObjectType Type { get; private set; }

      public ObjectEntry Entry { get; private set; }

      public LevelLayer Layer { get; set; }

      public int Priority { get; set; }

      public ActiveObject ParentObject { get; set; }

      public CollisionVector[] CollisionVectors { get; set; }

      public CollisionRectangle[] CollisionRectangles { get; set; }

      public CameraProperties CameraProperties { get; set; }

      public bool LockLifetime { get; set; }

      public bool Finished { get; private set; }

      public Rectanglei DesignBounds { get; protected set; }

      public float Brightness { get; set; }

      public ShadowInfo ShadowInfo { get; set; }

      public Vector2i Position
      {
        get => this._position;
        set
        {
          this._position = value;
          this._positionPrecise = (Vector2) value;
        }
      }

      public Vector2 PositionPrecise
      {
        get => this._positionPrecise;
        set
        {
          this._position = (Vector2i) value;
          this._positionPrecise = value;
        }
      }

      public Vector2i LastPosition => this._lastPosition;

      public Vector2 LastPositionPrecise => this._lastPositionPrecise;

      public Rectanglei LifetimeArea
      {
        get
        {
		    Vector2 lifeRadius = this.Type.GetLifeRadius(this.Entry.State);
			Vector2 vector = new Vector2((double)this.Position.X - lifeRadius.X, (double)this.Position.Y - lifeRadius.Y);
			return new Rectangle(vector.X, vector.Y, lifeRadius.X * 2.0, lifeRadius.Y * 2.0);
        }
      }

      public bool IsSubObject => this.ParentObject != null;

      public ResourceTree ResourceTree => this.Level.GameContext.ResourceTree;

      public ActiveObject()
      {
        this.CollisionVectors = new CollisionVector[0];
        this.CollisionRectangles = new CollisionRectangle[0];
        this.DesignBounds = (Rectanglei) new Rectangle(-32.0, -32.0, 64.0, 64.0);
      }

      public void Initialise(ObjectEntry entry)
      {
        this.Entry = entry;
        this.Level = entry.Level;
        this.Type = entry.Type;
        this.Layer = this.Level.Map.Layers[entry.Layer];
        this.Position = entry.Position;
        this.Name = entry.Name;
        this.Uid = entry.Uid;
        this._activeLayers = Enumerable.Range(0, this.Level.Map.Layers.Count).Select<int, bool>((Func<int, bool>) (x => true)).ToArray<bool>();
        this.Priority = 256 /*0x0100*/;
        this.CameraProperties = new CameraProperties();
        this.CameraProperties.Box = new Rectangle(-64.0, -192.0, 64.0, 256.0);
        this.CameraProperties.Delay = new Vector2i(0, 0);
        this.CameraProperties.MaxVelocity = new Vector2(64.0, 64.0);
        this.CameraProperties.Offset = new Vector2(0.0, 0.0);
        StateVariableAttribute.SetObjectState((IActiveObject) this, (IActiveObject) this.Entry.State);
        this.ShadowInfo = new ShadowInfo()
        {
          OcclusionSize = new Vector2i(16 /*0x10*/),
          MaxShadowOffset = new Vector2i(int.MaxValue)
        };
      }

      public void Finish() => this.Finished = true;

      public void FinishForever()
      {
        this.Finish();
        this.Entry.FinishForever();
      }

      public void Start()
      {
        this.OnStart();
        this.RegisterCollisionUpdate();
      }

      public void UpdateEditor() => this.OnUpdateEditor();

      public void UpdatePrepare()
      {
        this._lastPosition = this._position;
        this._lastPositionPrecise = this._positionPrecise;
        this.OnUpdatePrepare();
      }

      public void Update() => this.OnUpdate();

      public void UpdateCollision() => this.OnUpdateCollision();

      public void Collision(CollisionEvent e) => this.OnCollision(e);

      public void Animate() => this.OnAnimate();

      public void Draw(Renderer renderer, Viewport viewport, LayerViewOptions viewOptions)
      {
        Vector2i vector2i = new Vector2i();
        if (viewOptions.Shadows)
        {
          IShadowInfo shadowInfo = (IShadowInfo) this.ShadowInfo;
          if (shadowInfo == null)
            return;
          vector2i = this.Level.LightingManager.GetShadowOffset(this.Position, shadowInfo);
          if (vector2i == new Vector2i())
            return;
        }
        I2dRenderer obj1 = renderer.Get2dRenderer();
        using (obj1.BeginMatixState())
        {
          I2dRenderer obj2 = obj1;
          Matrix4 modelMatrix = obj1.ModelMatrix;
          int x1 = this._position.X;
          Rectanglei bounds = viewport.Bounds;
          int x2 = bounds.X;
          double x3 = (double) (x1 - x2 + vector2i.X) * viewport.Scale.X;
          int y1 = this._position.Y;
          bounds = viewport.Bounds;
          int y2 = bounds.Y;
          double y3 = (double) (y1 - y2 + vector2i.Y) * viewport.Scale.Y;
          Matrix4 translation = Matrix4.CreateTranslation(x3, y3);
          Matrix4 matrix4 = modelMatrix * translation;
          obj2.ModelMatrix = matrix4;
          IObjectRenderer objectRenderer = renderer.GetObjectRenderer();
          objectRenderer.ClipRectangle = (Rectangle) viewport.Destination;
          objectRenderer.ModelMatrix = obj1.ModelMatrix;
          objectRenderer.SetDefault();
          objectRenderer.Shadow = viewOptions.Shadows;
          objectRenderer.Filter = viewOptions.Filter;
          objectRenderer.FilterAmount = viewOptions.FilterAmount;
          objectRenderer.Scale = viewport.Scale;
          if (!viewOptions.ShowObjects)
            return;
          this.OnDraw(renderer, viewOptions);
        }
      }

      public void DrawCollision(Renderer renderer, Viewport viewport)
      {
        foreach (CollisionVector collisionVector in this.CollisionVectors)
          collisionVector.Draw(renderer, viewport);
        foreach (CollisionRectangle collisionRectangle in this.CollisionRectangles)
          collisionRectangle.Draw(renderer, viewport);
      }

      public void Stop() => this.OnStop();

      protected void RegisterCollisionUpdate()
      {
        foreach (CollisionVector collisionVector in this.CollisionVectors)
          collisionVector.UpdateDerrivedFields();
        int count = this.Level.Map.CollisionPathLayers.Count;
        foreach (CollisionVector collisionVector1 in this.CollisionVectors)
        {
          foreach (CollisionVector collisionVector2 in this.CollisionVectors)
          {
            if (collisionVector1 != collisionVector2)
            {
              if (collisionVector1.AbsoluteA == collisionVector2.AbsoluteB && CollisionVector.TestConnection(collisionVector1, collisionVector2))
              {
                for (int path = 0; path < count; ++path)
                {
                  if (collisionVector2.HasPath(path))
                    collisionVector1.SetConnectionA(path, collisionVector2);
                  if (collisionVector1.HasPath(path))
                    collisionVector2.SetConnectionB(path, collisionVector1);
                }
              }
              if (collisionVector1.AbsoluteB == collisionVector2.AbsoluteA && CollisionVector.TestConnection(collisionVector1, collisionVector2))
              {
                for (int path = 0; path < count; ++path)
                {
                  if (collisionVector2.HasPath(path))
                    collisionVector1.SetConnectionB(path, collisionVector2);
                  if (collisionVector1.HasPath(path))
                    collisionVector2.SetConnectionA(path, collisionVector1);
                }
              }
            }
          }
        }
      }

      public void Move(int x, int y) => this.Position += new Vector2i(x, y);

      public void Move(Vector2i offset) => this.Position += offset;

      public void MovePrecise(double x, double y) => this.PositionPrecise += new Vector2(x, y);

      public void MovePrecise(Vector2 offset) => this.PositionPrecise += offset;

      protected virtual void OnStart()
      {
      }

      protected virtual void OnUpdateEditor()
      {
      }

      protected virtual void OnUpdatePrepare()
      {
      }

      protected virtual void OnUpdate()
      {
      }

      protected virtual void OnUpdateCollision()
      {
      }

      protected virtual void OnCollision(CollisionEvent e)
      {
      }

      protected virtual void OnAnimate()
      {
      }

      protected virtual void OnDraw(Renderer renderer, LayerViewOptions viewOptions)
      {
      }

      protected virtual void OnStop()
      {
      }
    }
}
