// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Collision.CollisionEvent
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace SonicOrca.Core.Collision
{

    public class CollisionEvent
    {
      private readonly ActiveObject _object;
      private CollisionInfo _collisionInfo;
      private readonly int _id;

      public ActiveObject ActiveObject => this._object;

      public CollisionInfo CollisionInfo
      {
        get => this._collisionInfo;
        set => this._collisionInfo = value;
      }

      public int Id => this._id;

      public bool IgnoreCollision { get; set; }

      public bool MaintainVelocity { get; set; }

      public CollisionEvent(ActiveObject obj, int id)
      {
        this._object = obj;
        this._id = id;
      }

      public CollisionEvent(ActiveObject obj, CollisionInfo collisionInfo)
      {
        this._object = obj;
        this._collisionInfo = collisionInfo;
        this._id = this._collisionInfo.Vector.Id;
      }

      public override string ToString() => $"CollisionEvent ({this._object} for #{this._id})";
    }
}
