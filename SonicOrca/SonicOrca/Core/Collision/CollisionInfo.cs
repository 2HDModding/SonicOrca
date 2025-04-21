// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Collision.CollisionInfo
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Core.Collision
{

    public class CollisionInfo
    {
      private readonly CollisionVector _vector;
      private readonly Vector2 _touch;
      private readonly double _shift;
      private readonly double _angle;

      public CollisionVector Vector => this._vector;

      public Vector2 Touch => this._touch;

      public double Shift => this._shift;

      public double Angle => this._angle;

      public CollisionInfo(CollisionVector vector, Vector2 touch, double shift, double angle)
      {
        this._vector = vector;
        this._touch = touch;
        this._shift = shift;
        this._angle = angle;
      }
    }
}
