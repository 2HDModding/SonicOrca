// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Collision.CollisionVector
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicOrca.Core.Collision
{

    public class CollisionVector : IBounds
    {
      public const uint AllPaths = 4294967295 /*0xFFFFFFFF*/;
      private CollisionVector[] _connectionA = new CollisionVector[2];
      private CollisionVector[] _connectionB = new CollisionVector[2];
      private int _left;
      private int _top;
      private int _right;
      private int _bottom;

      public ActiveObject Owner { get; set; }

      public Vector2i RelativeA { get; set; }

      public Vector2i RelativeB { get; set; }

      public uint Paths { get; set; }

      public CollisionFlags Flags { get; set; }

      public int Id { get; set; }

      public double Angle { get; private set; }

      public int FlipX { get; private set; }

      public int FlipY { get; private set; }

      public CollisionMode Mode { get; private set; }

      public int Left => this.Owner != null ? this._left + this.Owner.Position.X : this._left;

      public int Top => this.Owner != null ? this._top + this.Owner.Position.Y : this._top;

      public int Right => this.Owner != null ? this._right + this.Owner.Position.X : this._right;

      public int Bottom => this.Owner != null ? this._bottom + this.Owner.Position.Y : this._bottom;

      public int Width { get; private set; }

      public int Height { get; private set; }

      public double Ratio { get; private set; }

      public Vector2i AbsoluteA
      {
        get => this.Owner != null ? this.RelativeA + this.Owner.Position : this.RelativeA;
      }

      public Vector2i AbsoluteB
      {
        get => this.Owner != null ? this.RelativeB + this.Owner.Position : this.RelativeB;
      }

      public Vector2i AB => this.RelativeB - this.RelativeA;

      public double Length => (double) this.AB.Length;

      public double CollisionAngle => this.Angle + Math.PI;

      public CollisionVector()
      {
      }

      public CollisionVector(Vector2i a, Vector2i b, uint paths = 4294967295 /*0xFFFFFFFF*/, CollisionFlags flags = (CollisionFlags) 0)
        : this((ActiveObject) null, a, b, paths, flags)
      {
      }

      public CollisionVector(
        ActiveObject owner,
        Vector2i a,
        Vector2i b,
        uint paths = 4294967295 /*0xFFFFFFFF*/,
        CollisionFlags flags = (CollisionFlags) 0)
      {
        this.Owner = owner;
        this.RelativeA = a;
        this.RelativeB = b;
        this.Paths = paths;
        this.Flags = flags;
      }

      public static CollisionVector[] FromRectangle(Rectanglei rect)
      {
        return CollisionVector.FromRectangle((ActiveObject) null, rect);
      }

      public static CollisionVector[] FromRectangle(
        ActiveObject owner,
        Rectanglei rect,
        uint paths = 4294967295 /*0xFFFFFFFF*/,
        CollisionFlags flags = (CollisionFlags) 0)
      {
        return new CollisionVector[4]
        {
          new CollisionVector(owner, new Vector2i(rect.X, rect.Y + rect.Height), new Vector2i(rect.X, rect.Y), paths),
          new CollisionVector(owner, new Vector2i(rect.X, rect.Y), new Vector2i(rect.X + rect.Width, rect.Y), paths),
          new CollisionVector(owner, new Vector2i(rect.X + rect.Width, rect.Y), new Vector2i(rect.X + rect.Width, rect.Y + rect.Height), paths),
          new CollisionVector(owner, new Vector2i(rect.X + rect.Width, rect.Y + rect.Height), new Vector2i(rect.X, rect.Y + rect.Height), paths)
        };
      }

      public static void UpdateRectangle(CollisionVector[] vectors, Rectanglei rect)
      {
        vectors[0].RelativeA = new Vector2i(rect.X, rect.Y + rect.Height);
        vectors[0].RelativeB = new Vector2i(rect.X, rect.Y);
        vectors[1].RelativeA = new Vector2i(rect.X, rect.Y);
        vectors[1].RelativeB = new Vector2i(rect.X + rect.Width, rect.Y);
        vectors[2].RelativeA = new Vector2i(rect.X + rect.Width, rect.Y);
        vectors[2].RelativeB = new Vector2i(rect.X + rect.Width, rect.Y + rect.Height);
        vectors[3].RelativeA = new Vector2i(rect.X + rect.Width, rect.Y + rect.Height);
        vectors[3].RelativeB = new Vector2i(rect.X, rect.Y + rect.Height);
      }

      public Rectanglei Bounds
      {
        get
        {
          Vector2i vector2i = this.AbsoluteA;
          int x1 = vector2i.X;
          vector2i = this.AbsoluteB;
          int x2 = vector2i.X;
          int left = Math.Min(x1, x2);
          vector2i = this.AbsoluteA;
          int y1 = vector2i.Y;
          vector2i = this.AbsoluteB;
          int y2 = vector2i.Y;
          int top = Math.Min(y1, y2);
          vector2i = this.AbsoluteA;
          int x3 = vector2i.X;
          vector2i = this.AbsoluteB;
          int x4 = vector2i.X;
          int right = Math.Max(x3, x4);
          vector2i = this.AbsoluteA;
          int y3 = vector2i.Y;
          vector2i = this.AbsoluteB;
          int y4 = vector2i.Y;
          int bottom = Math.Max(y3, y4);
          return Rectanglei.FromLTRB(left, top, right, bottom);
        }
      }

      public bool IsWall => this.Mode == CollisionMode.Left || this.Mode == CollisionMode.Right;

      public bool IsFloor => !this.IsWall;

      public bool IsThereConnectionA
      {
        get
        {
          return ((IEnumerable<CollisionVector>) this._connectionA).Any<CollisionVector>((Func<CollisionVector, bool>) (x => x != null));
        }
      }

      public bool IsThereConnectionB
      {
        get
        {
          return ((IEnumerable<CollisionVector>) this._connectionB).Any<CollisionVector>((Func<CollisionVector, bool>) (x => x != null));
        }
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        bool thereConnectionA = this.IsThereConnectionA;
        bool thereConnectionB = this.IsThereConnectionB;
        stringBuilder.AppendFormat("A = {0}, B = {1}", (object) this.AbsoluteA, (object) this.AbsoluteB);
        if (thereConnectionA & thereConnectionB)
          stringBuilder.Append(" Connected (A, B)");
        else if (thereConnectionA)
          stringBuilder.Append(" Connected (A)");
        else if (thereConnectionB)
          stringBuilder.Append(" Connected (B)");
        return stringBuilder.ToString();
      }

      public CollisionVector GetConnectionA(int path)
      {
        return this._connectionA.Length <= path ? (CollisionVector) null : this._connectionA[path];
      }

      public CollisionVector GetConnectionB(int path)
      {
        return this._connectionB.Length <= path ? (CollisionVector) null : this._connectionB[path];
      }

      public void SetConnectionA(int path, CollisionVector v)
      {
        if (this._connectionA.Length <= path)
          Array.Resize<CollisionVector>(ref this._connectionA, path);
        this._connectionA[path] = v;
      }

      public void SetConnectionB(int path, CollisionVector v)
      {
        if (this._connectionB.Length <= path)
          Array.Resize<CollisionVector>(ref this._connectionB, path);
        this._connectionB[path] = v;
      }

      public void UpdateDerrivedFields()
      {
        Vector2i vector2i1 = this.RelativeA;
        int x1 = vector2i1.X;
        vector2i1 = this.RelativeB;
        int x2 = vector2i1.X;
        if (x1 < x2)
        {
          Vector2i vector2i2 = this.RelativeA;
          this._left = vector2i2.X;
          vector2i2 = this.RelativeB;
          this._right = vector2i2.X;
          this.FlipX = 1;
        }
        else
        {
          Vector2i vector2i3 = this.RelativeB;
          this._left = vector2i3.X;
          vector2i3 = this.RelativeA;
          this._right = vector2i3.X;
          this.FlipX = this._left == this._right ? 0 : -1;
        }
        Vector2i vector2i4 = this.AbsoluteA;
        int y1 = vector2i4.Y;
        vector2i4 = this.AbsoluteB;
        int y2 = vector2i4.Y;
        if (y1 < y2)
        {
          Vector2i vector2i5 = this.RelativeA;
          this._top = vector2i5.Y;
          vector2i5 = this.RelativeB;
          this._bottom = vector2i5.Y;
          this.FlipY = 1;
        }
        else
        {
          Vector2i vector2i6 = this.RelativeB;
          this._top = vector2i6.Y;
          vector2i6 = this.RelativeA;
          this._bottom = vector2i6.Y;
          this.FlipY = this._top == this._bottom ? 0 : -1;
        }
        this.Width = this.Right - this.Left;
        this.Height = this.Bottom - this.Top;
        this.Mode = this.Width < this.Height ? (this.FlipY > 0 ? CollisionMode.Right : CollisionMode.Left) : (this.FlipX > 0 ? CollisionMode.Top : CollisionMode.Bottom);
        Vector2i vector2i7 = this.AbsoluteB;
        int x3 = vector2i7.X;
        vector2i7 = this.AbsoluteA;
        int x4 = vector2i7.X;
        double num1 = (double) (x3 - x4);
        double num2 = (double) (this.AbsoluteB.Y - this.AbsoluteA.Y);
        this.Ratio = num2 == 0.0 ? 0.0 : num1 / num2;
        this.Angle = (this.AbsoluteB - this.AbsoluteA).Angle;
      }

      public void ResetConnections()
      {
        Array.Clear((Array) this._connectionA, 0, this._connectionA.Length);
        Array.Clear((Array) this._connectionB, 0, this._connectionB.Length);
      }

      public bool HasPath(int path) => ((ulong) this.Paths & (ulong) (1 << path)) > 0UL;

      public bool FindFloor(
        Vector2 sensorPosition,
        double sensorSize,
        int path,
        out CollisionInfo info)
      {
        bool flag = false;
        double num1 = this.Angle;
        double x1 = sensorPosition.X - Math.Sin(num1) * sensorSize;
        double y = (double) this.AbsoluteA.Y;
        Vector2i vector2i;
        if (this.GetConnectionB(path) != null)
        {
          double num2 = x1;
          vector2i = this.AbsoluteB;
          double x2 = (double) vector2i.X;
          if ((num2 - x2) * (double) this.FlipX > 0.0)
          {
            vector2i = this.AbsoluteB;
            x1 = (double) vector2i.X;
            flag = true;
            goto label_6;
          }
        }
        if (this.GetConnectionA(path) != null)
        {
          double num3 = x1;
          vector2i = this.AbsoluteA;
          double x3 = (double) vector2i.X;
          if ((num3 - x3) * (double) this.FlipX < 0.0)
          {
            vector2i = this.AbsoluteA;
            x1 = (double) vector2i.X;
            flag = true;
          }
        }
    label_6:
        if (flag)
        {
          num1 = Math.Asin(MathX.Clamp(-1.0, (x1 - sensorPosition.X) / sensorSize, 1.0)) * (double) this.FlipX;
          if (this.FlipX < 0)
            num1 += Math.PI;
        }
        if (this.Height > 0)
        {
          double num4 = y;
          double num5 = x1;
          vector2i = this.AbsoluteA;
          double x4 = (double) vector2i.X;
          double num6 = (num5 - x4) / this.Ratio;
          y = num4 + num6;
        }
        double shift = y - Math.Cos(num1) * sensorSize - sensorPosition.Y;
        info = new CollisionInfo(this, new Vector2(x1, y), shift, num1);
        return shift * (double) this.FlipX < 0.0;
      }

      public bool FindWall(
        Vector2 sensorPosition,
        double sensorSize,
        int path,
        out CollisionInfo info)
      {
        bool flag = false;
        double num1 = this.Angle;
        double x = (double) this.AbsoluteA.X;
        double y1 = sensorPosition.Y + Math.Cos(num1) * sensorSize;
        Vector2i vector2i;
        if (this.GetConnectionB(path) != null)
        {
          double num2 = y1;
          vector2i = this.AbsoluteB;
          double y2 = (double) vector2i.Y;
          if ((num2 - y2) * (double) this.FlipY > 0.0)
          {
            vector2i = this.AbsoluteB;
            y1 = (double) vector2i.Y;
            flag = true;
            goto label_6;
          }
        }
        if (this.GetConnectionA(path) != null)
        {
          double num3 = y1;
          vector2i = this.AbsoluteA;
          double y3 = (double) vector2i.Y;
          if ((num3 - y3) * (double) this.FlipY < 0.0)
          {
            vector2i = this.AbsoluteA;
            y1 = (double) vector2i.Y;
            flag = true;
          }
        }
    label_6:
        if (flag)
        {
          double num4 = Math.Asin(MathX.Clamp(-1.0, (y1 - sensorPosition.Y) / sensorSize, 1.0)) * (double) this.FlipY;
          if (this.FlipY > 0)
            num4 += Math.PI;
          num1 = num4 - Math.PI / 2.0;
        }
        if (this.Width > 0)
        {
          double num5 = x;
          double num6 = y1;
          vector2i = this.AbsoluteA;
          double y4 = (double) vector2i.Y;
          double num7 = (num6 - y4) * this.Ratio;
          x = num5 + num7;
        }
        double shift = x + Math.Sin(num1) * sensorSize - sensorPosition.X;
        info = new CollisionInfo(this, new Vector2(x, y1), shift, num1);
        return shift * (double) this.FlipY > 0.0;
      }

      public static bool TestConnection(CollisionVector a, CollisionVector b)
      {
        return a.Mode == b.Mode || Math.Abs(MathX.DifferenceRadians(a.Angle, b.Angle)) <= Math.PI / 3.0;
      }

      public static bool OnSamePath(CollisionVector a, CollisionVector b, int path)
      {
        if (a == b)
          return true;
        if (a.Mode != b.Mode)
          return false;
        if ((a.IsWall ? ((a.AbsoluteA.Y - b.AbsoluteA.Y) * a.FlipY > 0 ? 1 : 0) : ((a.AbsoluteA.X - b.AbsoluteA.X) * a.FlipX > 0 ? 1 : 0)) != 0)
        {
          for (CollisionVector connectionB = b.GetConnectionB(path); connectionB != null; connectionB = connectionB.GetConnectionB(path))
          {
            if (connectionB == a)
              return true;
            if (connectionB.Mode != b.Mode)
              return false;
          }
        }
        else
        {
          for (CollisionVector connectionA = b.GetConnectionA(path); connectionA != null; connectionA = connectionA.GetConnectionA(path))
          {
            if (connectionA == a)
              return true;
            if (connectionA.Mode != b.Mode)
              return false;
          }
        }
        return false;
      }

      public void Draw(Renderer renderer, Viewport viewport)
      {
        Vector2i absoluteA = this.AbsoluteA;
        Vector2i absoluteB = this.AbsoluteB;
        Rectanglei bounds;
        if (absoluteA.X < viewport.Bounds.X)
        {
          int x1 = absoluteB.X;
          bounds = viewport.Bounds;
          int x2 = bounds.X;
          if (x1 < x2)
            return;
        }
        int x3 = absoluteA.X;
        bounds = viewport.Bounds;
        int right1 = bounds.Right;
        if (x3 >= right1)
        {
          int x4 = absoluteB.X;
          bounds = viewport.Bounds;
          int right2 = bounds.Right;
          if (x4 >= right2)
            return;
        }
        int y1 = absoluteA.Y;
        bounds = viewport.Bounds;
        int y2 = bounds.Y;
        if (y1 < y2)
        {
          int y3 = absoluteB.Y;
          bounds = viewport.Bounds;
          int y4 = bounds.Y;
          if (y3 < y4)
            return;
        }
        int y5 = absoluteA.Y;
        bounds = viewport.Bounds;
        int bottom1 = bounds.Bottom;
        if (y5 >= bottom1)
        {
          int y6 = absoluteB.Y;
          bounds = viewport.Bounds;
          int bottom2 = bounds.Bottom;
          if (y6 >= bottom2)
            return;
        }
        ref Vector2i local1 = ref absoluteA;
        int x5 = local1.X;
        bounds = viewport.Bounds;
        int x6 = bounds.X;
        local1.X = x5 - x6;
        ref Vector2i local2 = ref absoluteA;
        int y7 = local2.Y;
        bounds = viewport.Bounds;
        int y8 = bounds.Y;
        local2.Y = y7 - y8;
        ref Vector2i local3 = ref absoluteB;
        int x7 = local3.X;
        bounds = viewport.Bounds;
        int x8 = bounds.X;
        local3.X = x7 - x8;
        ref Vector2i local4 = ref absoluteB;
        int y9 = local4.Y;
        bounds = viewport.Bounds;
        int y10 = bounds.Y;
        local4.Y = y9 - y10;
        this.Draw(renderer, (Vector2) absoluteA, (Vector2) absoluteB, viewport.Scale);
      }

      private void Draw(Renderer renderer, Vector2 pa, Vector2 pb, Vector2 scale)
      {
        pa = (Vector2) (Vector2i) (pa * scale);
        pb = (Vector2) (Vector2i) (pb * scale);
        I2dRenderer obj = renderer.Get2dRenderer();
        Colour colour = this.Owner == null ? Colours.White : Colours.Yellow;
        if (this.Owner == null && this.Paths == 1U)
          colour = Colours.Aqua;
        else if (this.Owner == null && this.Paths == 2U)
          colour = Colours.Fuchsia;
        obj.RenderLine(colour, pa, pb, 1.0);
        double num1 = 16.0 * scale.X;
        if (num1 > 2.0)
        {
          double num2 = this.Angle - Math.PI / 2.0;
          Vector2 a = new Vector2((pb.X - pa.X) / 2.0 + pa.X, (pb.Y - pa.Y) / 2.0 + pa.Y);
          Vector2 b = a + new Vector2(Math.Cos(num2) * num1, Math.Sin(num2) * num1);
          obj.RenderLine(colour, a, b, 1.0);
        }
        double num3 = 8.0 * scale.X;
        int sectors = (int) (8.0 * scale.X);
        if (num3 <= 1.0)
          return;
        if (this.IsThereConnectionA)
          obj.RenderEllipse(colour, pa, 0.0, num3 + 0.5, sectors);
        else
          obj.RenderRectangle(colour, new Rectangle(pa.X - num3, pa.Y - num3, num3 * 2.0, num3 * 2.0), 1.0);
        if (this.IsThereConnectionB)
          obj.RenderEllipse(colour, pb, 0.0, num3 + 0.5, sectors);
        else
          obj.RenderRectangle(colour, new Rectangle(pb.X - num3, pb.Y - num3, num3 * 2.0, num3 * 2.0), 1.0);
      }

      public static bool GetIntersection(
        CollisionVector a,
        CollisionVector b,
        out Vector2 intersection)
      {
        Vector2 absoluteA1 = (Vector2) a.AbsoluteA;
        Vector2 absoluteB1 = (Vector2) a.AbsoluteB;
        Vector2 absoluteA2 = (Vector2) b.AbsoluteA;
        Vector2 absoluteB2 = (Vector2) b.AbsoluteB;
        Vector2 vector2_1 = new Vector2(absoluteB1.X - absoluteA1.X, absoluteB1.Y - absoluteA1.Y);
        Vector2 vector2_2 = new Vector2(absoluteB2.X - absoluteA2.X, absoluteB2.Y - absoluteA2.Y);
        double num1 = (-vector2_1.Y * (absoluteA1.X - absoluteA2.X) + vector2_1.X * (absoluteA1.Y - absoluteA2.Y)) / (-vector2_2.X * vector2_1.Y + vector2_1.X * vector2_2.Y);
        double num2 = (vector2_2.X * (absoluteA1.Y - absoluteA2.Y) - vector2_2.Y * (absoluteA1.X - absoluteA2.X)) / (-vector2_2.X * vector2_1.Y + vector2_1.X * vector2_2.Y);
        if (num1 >= 0.0 && num1 <= 1.0 && num2 >= 0.0 && num2 <= 1.0)
        {
          intersection = new Vector2(absoluteA1.X + num2 * vector2_1.X, absoluteA1.Y + num2 * vector2_1.Y);
          return true;
        }
        intersection = new Vector2();
        return false;
      }

      public static bool Intersects(CollisionVector a, CollisionVector b)
      {
        return CollisionVector.GetIntersection(a, b, out Vector2 _);
      }

      public static IEnumerable<CollisionInfo> GetCollisions(
        ActiveObject obj,
        int radialSensorSize,
        uint paths = 4294967295 /*0xFFFFFFFF*/)
      {
        return CollisionVector.GetCollisions(obj, radialSensorSize, radialSensorSize, paths);
      }

      public static IEnumerable<CollisionInfo> GetCollisions(
        ActiveObject obj,
        int floorSensorSize,
        int wallSensorSize,
        uint paths = 4294967295 /*0xFFFFFFFF*/)
      {
        Vector2i positionPrecise = (Vector2i) obj.PositionPrecise;
        Rectanglei floorRect = Rectanglei.FromLTRB(positionPrecise.X - wallSensorSize, positionPrecise.Y - floorSensorSize, positionPrecise.X + wallSensorSize, positionPrecise.Y + floorSensorSize);
        Rectanglei wallRect = Rectanglei.FromLTRB(positionPrecise.X - wallSensorSize, positionPrecise.Y - wallSensorSize, positionPrecise.X + wallSensorSize, positionPrecise.Y + wallSensorSize);
        return CollisionVector.GetCollisions(obj, floorSensorSize, wallSensorSize, floorRect, wallRect, paths, CollisionMode.Air, (CollisionVector) null);
      }

      public static IEnumerable<CollisionInfo> GetCollisions(
        ActiveObject obj,
        int floorSensorSize,
        int wallSensorSize,
        Rectanglei floorRect,
        Rectanglei wallRect,
        uint paths,
        CollisionMode mode,
        CollisionVector groundVector)
      {
        int numPaths = obj.Level.Map.CollisionPathLayers.Count;
        foreach (CollisionVector collisionIntersection in obj.Level.CollisionTable.GetPossibleCollisionIntersections(new Rectanglei(obj.Position.X - wallSensorSize * 2, obj.Position.Y - floorSensorSize * 2, wallSensorSize * 4, floorSensorSize * 4)))
        {
          CollisionVector t = collisionIntersection;
          if (((int) t.Paths & (int) paths) != 0)
          {
            for (int i = 0; i < numPaths; ++i)
            {
              if (((long) t.Paths & (long) (1 << i)) != 0L)
              {
                if (t.IsWall)
                {
                  CollisionInfo info;
                  if (CollisionVector.RectangleCheckCollision(wallRect, t.Bounds) && t.FindWall(obj.PositionPrecise, (double) wallSensorSize, i, out info) && !CollisionVector.CheckSolidAngle(t, obj.LastPositionPrecise, info.Touch) && !CollisionVector.CheckSolidAngle(t, obj.LastPositionPrecise, obj.PositionPrecise))
                  {
                    yield return info;
                    break;
                  }
                }
                else
                {
                  CollisionInfo info;
                  if (CollisionVector.RectangleCheckCollision(floorRect, t.Bounds) && t.FindFloor(obj.PositionPrecise, (double) floorSensorSize, i, out info) && !CollisionVector.CheckSolidAngle(t, obj.LastPositionPrecise, info.Touch) && !CollisionVector.CheckSolidAngle(t, obj.LastPositionPrecise, obj.PositionPrecise))
                  {
                    yield return info;
                    break;
                  }
                }
              }
            }
            t = (CollisionVector) null;
          }
        }
      }

      private static bool CheckSolidAngle(CollisionVector t, Vector2 previousPosition, Vector2 point)
      {
        return point == previousPosition || MathX.DifferenceRadians((point - previousPosition).Angle, t.Angle) > 0.0;
      }

      private static bool RectangleCheckCollision(Rectanglei a, Rectanglei b)
      {
        return a.X < b.X + b.Width && b.X < a.X + a.Width && a.Y < b.Y + b.Height && b.Y < a.Y + a.Height;
      }
    }
}
