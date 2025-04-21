// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Collision.CollisionTable
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SonicOrca.Core.Collision
{

    public class CollisionTable
    {
      private readonly Level _level;
      private QuadTree<CollisionVector> _newCollisionQuadTree;

      public QuadTree<CollisionVector> InternalTree => this._newCollisionQuadTree;

      public int Count => this._newCollisionQuadTree.Count;

      public CollisionTable(Level level) => this._level = level;

      public void Initialise(LevelMap map)
      {
        Trace.WriteLine("Initialising collision table");
        IEnumerable<CollisionVector> collisionVectors = (IEnumerable<CollisionVector>) map.CollisionVectors;
        this._newCollisionQuadTree = new QuadTree<CollisionVector>(collisionVectors);
        foreach (CollisionVector collisionVector in collisionVectors)
          collisionVector.UpdateDerrivedFields();
        this.UpdateAllConnections();
      }

      public IEnumerable<CollisionVector> GetVectorConnections(CollisionVector v)
      {
        HashSet<CollisionVector> results = new HashSet<CollisionVector>();
        int numPaths = this._level.Map.CollisionPathLayers.Count;
        for (int i = 0; i < numPaths; ++i)
        {
          CollisionVector connectionA = v.GetConnectionA(i);
          CollisionVector connectionB = v.GetConnectionB(i);
          if (connectionA != null && results.Add(connectionA))
            yield return connectionA;
          if (connectionB != null && results.Add(connectionB))
            yield return connectionB;
          connectionB = (CollisionVector) null;
        }
      }

      public void UpdateConnectionsFast(IEnumerable<CollisionVector> vectors)
      {
        int count = this._level.Map.CollisionPathLayers.Count;
        Queue<CollisionVector> collisionVectorQueue = new Queue<CollisionVector>(vectors);
        HashSet<CollisionVector> source = new HashSet<CollisionVector>();
        while (collisionVectorQueue.Count > 0)
        {
          CollisionVector v = collisionVectorQueue.Dequeue();
          if (source.Add(v))
          {
            foreach (CollisionVector vectorConnection in this.GetVectorConnections(v))
              collisionVectorQueue.Enqueue(vectorConnection);
            foreach (CollisionVector collisionIntersection in this.GetPossibleCollisionIntersections(v.Bounds.Inflate(new Vector2i(8, 8)), objects: false))
            {
              if (collisionIntersection.AbsoluteA == v.AbsoluteA || collisionIntersection.AbsoluteA == v.AbsoluteB || collisionIntersection.AbsoluteB == v.AbsoluteA || collisionIntersection.AbsoluteB == v.AbsoluteB)
                collisionVectorQueue.Enqueue(collisionIntersection);
            }
          }
        }
        this.UpdateConnections((IEnumerable<CollisionVector>) source.ToArray<CollisionVector>());
      }

      public void UpdateAllConnections()
      {
        this.UpdateConnections((IEnumerable<CollisionVector>) this._level.Map.CollisionVectors);
      }

      public void UpdateConnections(IEnumerable<CollisionVector> vectors)
      {
        int count = this._level.Map.CollisionPathLayers.Count;
        foreach (CollisionVector vector in vectors)
          vector.ResetConnections();
        foreach (CollisionVector vector1 in vectors)
        {
          foreach (CollisionVector vector2 in vectors)
          {
            if (vector1 != vector2)
            {
              if (vector1.AbsoluteA == vector2.AbsoluteB && CollisionVector.TestConnection(vector1, vector2))
              {
                for (int path = 0; path < count; ++path)
                {
                  if (vector2.HasPath(path))
                    vector1.SetConnectionA(path, vector2);
                  if (vector1.HasPath(path))
                    vector2.SetConnectionB(path, vector1);
                }
              }
              if (vector1.AbsoluteB == vector2.AbsoluteA && CollisionVector.TestConnection(vector1, vector2))
              {
                for (int path = 0; path < count; ++path)
                {
                  if (vector2.HasPath(path))
                    vector1.SetConnectionB(path, vector2);
                  if (vector1.HasPath(path))
                    vector2.SetConnectionA(path, vector1);
                }
              }
            }
          }
        }
      }

      public void UpdateConnectionsForVector(CollisionVector v1)
      {
        IList<CollisionVector> collisionVectors = this._level.Map.CollisionVectors;
        int count = this._level.Map.CollisionPathLayers.Count;
        foreach (CollisionVector collisionVector in (IEnumerable<CollisionVector>) collisionVectors)
        {
          if (v1 != collisionVector)
          {
            if (v1.AbsoluteA == collisionVector.AbsoluteB && CollisionVector.TestConnection(v1, collisionVector))
            {
              for (int path = 0; path < count; ++path)
              {
                if (collisionVector.HasPath(path))
                  v1.SetConnectionA(path, collisionVector);
                if (v1.HasPath(path))
                  collisionVector.SetConnectionB(path, v1);
              }
            }
            if (v1.AbsoluteB == collisionVector.AbsoluteA && CollisionVector.TestConnection(v1, collisionVector))
            {
              for (int path = 0; path < count; ++path)
              {
                if (collisionVector.HasPath(path))
                  v1.SetConnectionB(path, collisionVector);
                if (v1.HasPath(path))
                  collisionVector.SetConnectionA(path, v1);
              }
            }
          }
        }
      }

      public IEnumerable<CollisionVector> GetPossibleCollisionIntersections(
        Rectanglei bounds,
        bool landscape = true,
        bool objects = true)
      {
        if (objects)
        {
          foreach (CollisionVector collisionIntersection in this._level.ObjectManager.ActiveObjects.SelectMany<ActiveObject, CollisionVector>((Func<ActiveObject, IEnumerable<CollisionVector>>) (x => (IEnumerable<CollisionVector>) x.CollisionVectors)))
            yield return collisionIntersection;
        }
        if (landscape)
        {
          foreach (CollisionVector collisionIntersection in this._newCollisionQuadTree.Query(bounds))
            yield return collisionIntersection;
        }
      }

      public override string ToString() => $"{this._newCollisionQuadTree.Count} collision vectors";
    }
}
