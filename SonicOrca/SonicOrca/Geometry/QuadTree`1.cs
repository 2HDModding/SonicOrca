// Decompiled with JetBrains decompiler
// Type: SonicOrca.Geometry.QuadTree`1
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SonicOrca.Geometry
{

    public class QuadTree<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : IBounds
    {
      private QuadTree<T>.Node _root;
      private Rectanglei _bounds;
      private readonly int _minNodeSize = 4096 /*0x1000*/;
      private readonly int _subdivideTargetCount = 4;

      public QuadTree(Rectanglei bounds, int minNodeSize = 4096 /*0x1000*/, int subdivideTargetCount = 4)
      {
        this._minNodeSize = minNodeSize;
        this._subdivideTargetCount = subdivideTargetCount;
        this._bounds = bounds;
        this._root = new QuadTree<T>.Node(this, this._bounds);
      }

      public QuadTree(IEnumerable<T> items, int minNodeSize = 4096 /*0x1000*/, int subdivideTargetCount = 4)
      {
        this._minNodeSize = minNodeSize;
        this._subdivideTargetCount = subdivideTargetCount;
        T[] array = items.ToArray<T>();
        if (array.Length == 0)
        {
          this._bounds = new Rectanglei(0, 0, minNodeSize, minNodeSize);
          this._root = new QuadTree<T>.Node(this, this._bounds);
        }
        else
        {
          int x = ((IEnumerable<T>) array).Min<T>((Func<T, int>) (i => i.Bounds.Left));
          int y = ((IEnumerable<T>) array).Min<T>((Func<T, int>) (i => i.Bounds.Top));
          int num1 = ((IEnumerable<T>) array).Max<T>((Func<T, int>) (i => i.Bounds.Right));
          int num2 = ((IEnumerable<T>) array).Max<T>((Func<T, int>) (i => i.Bounds.Bottom));
          this._bounds = new Rectanglei(x, y, num1 - x, num2 - y);
          this._root = new QuadTree<T>.Node(this, this._bounds);
          Trace.WriteLine("QuadTree bounds: " + (object) this._bounds);
          foreach (T obj in array)
            this.Add(obj);
        }
      }

      public IEnumerable<T> Query(Rectanglei queryBounds) => this._root.Query(queryBounds);

      public void AddRange(IEnumerable<T> items)
      {
        T[] array1 = items.ToArray<T>();
        if (((IEnumerable<T>) array1).Any<T>((Func<T, bool>) (x => !this._bounds.Contains(x.Bounds))))
        {
          Trace.WriteLine("Resizing quad tree");
          foreach (T obj in array1)
            this._bounds = this._bounds.Union(obj.Bounds);
          T[] array2 = this._root.Items.ToArray<T>();
          this._root = new QuadTree<T>.Node(this, this._bounds);
          foreach (T obj in array2)
            this._root.Add(obj);
        }
        foreach (T obj in array1)
          this.Add(obj);
      }

      public int GetDepth() => this._root.GetDepth();

      public void Add(T item)
      {
        if (this._root.Add(item))
          return;
        Trace.WriteLine("Resizing quad tree");
        this._bounds = this._bounds.Union(item.Bounds);
        T[] array = this._root.Items.ToArray<T>();
        this._root = new QuadTree<T>.Node(this, this._bounds);
        foreach (T obj in array)
          this._root.Add(obj);
        this._root.Add(item);
      }

      public void Clear() => this._root = new QuadTree<T>.Node(this, this._bounds);

      public bool Contains(T item) => this._root.Items.Contains<T>(item);

      public void CopyTo(T[] array, int arrayIndex)
      {
        this._root.Items.ToArray<T>().CopyTo((Array) array, arrayIndex);
      }

      public int Count => this._root.Count;

      public bool IsReadOnly => false;

      public bool Remove(T item) => this._root.Remove(item);

      public IEnumerator<T> GetEnumerator() => this._root.Items.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._root.Items.GetEnumerator();

      private class Node
      {
        private readonly QuadTree<T> _quadTree;
        private readonly Rectanglei _bounds;
        private readonly List<T> _items = new List<T>();
        private QuadTree<T>.Node[] _children = new QuadTree<T>.Node[0];

        public Rectanglei Bounds => this._bounds;

        public int Count
        {
          get
          {
            int count = this._items.Count;
            foreach (QuadTree<T>.Node child in this._children)
              count += child.Count;
            return count;
          }
        }

        public IEnumerable<T> Items
        {
          get
          {
            foreach (T obj in this._items)
              yield return obj;
            QuadTree<T>.Node[] nodeArray = this._children;
            for (int index = 0; index < nodeArray.Length; ++index)
            {
              foreach (T obj in nodeArray[index].Items)
                yield return obj;
            }
            nodeArray = (QuadTree<T>.Node[]) null;
          }
        }

        public Node(QuadTree<T> quadTree, Rectanglei bounds)
        {
          this._quadTree = quadTree;
          this._bounds = bounds;
        }

        public bool Add(T item)
        {
          if (!this._bounds.Contains(item.Bounds))
            return false;
          if (this._children.Length == 0)
          {
            if (this._items.Count > this._quadTree._subdivideTargetCount)
            {
              this.ReStructure();
            }
            else
            {
              this._items.Add(item);
              return true;
            }
          }
          foreach (QuadTree<T>.Node child in this._children)
          {
            if (child._bounds.Contains(item.Bounds))
            {
              Trace.Indent();
              int num = child.Add(item) ? 1 : 0;
              Trace.Unindent();
              return num != 0;
            }
          }
          this._items.Add(item);
          return true;
        }

        public bool Remove(T item)
        {
          if (this._items.Remove(item))
            return true;
          foreach (QuadTree<T>.Node child in this._children)
          {
            if (child.Remove(item))
              return true;
          }
          return false;
        }

        public IEnumerable<T> Query(Rectanglei queryBounds)
        {
          foreach (T obj in this._items)
          {
            Rectanglei bounds = obj.Bounds;
            if (queryBounds.IntersectsWith(obj.Bounds))
              yield return obj;
          }
          QuadTree<T>.Node[] nodeArray = this._children;
          for (int index = 0; index < nodeArray.Length; ++index)
          {
            QuadTree<T>.Node child = nodeArray[index];
            if (child._items.Count != 0 || child._children.Length != 0)
            {
              if (child._bounds.Contains(queryBounds))
              {
                foreach (T obj in child.Query(queryBounds))
                  yield return obj;
                break;
              }
              if (queryBounds.Contains(child._bounds))
              {
                foreach (T obj in child.Items)
                  yield return obj;
              }
              else
              {
                if (child._bounds.IntersectsWith(queryBounds))
                {
                  foreach (T obj in child.Query(queryBounds))
                    yield return obj;
                }
                child = (QuadTree<T>.Node) null;
              }
            }
          }
          nodeArray = (QuadTree<T>.Node[]) null;
        }

        private void ReStructure()
        {
          T[] array = this._items.ToArray();
          this._items.Clear();
          this.CreateChildren();
          foreach (T obj in array)
            this.Add(obj);
        }

        private void CreateChildren()
        {
          if (this._bounds.Area <= (long) this._quadTree._minNodeSize)
            return;
          int width1 = this._bounds.Width / 2;
          int height1 = this._bounds.Height / 2;
          this._children = new QuadTree<T>.Node[4];
          QuadTree<T>.Node[] children1 = this._children;
          QuadTree<T> quadTree1 = this._quadTree;
          int x1 = this._bounds.X;
          Rectanglei bounds1 = this._bounds;
          int y1 = bounds1.Y;
          int width2 = width1;
          int height2 = height1;
          Rectanglei bounds2 = new Rectanglei(x1, y1, width2, height2);
          QuadTree<T>.Node node1 = new QuadTree<T>.Node(quadTree1, bounds2);
          children1[0] = node1;
          QuadTree<T>.Node[] children2 = this._children;
          QuadTree<T> quadTree2 = this._quadTree;
          bounds1 = this._bounds;
          int x2 = bounds1.X;
          Rectanglei bounds3 = this._bounds;
          int y2 = bounds3.Y + height1;
          int width3 = width1;
          int height3 = height1;
          Rectanglei bounds4 = new Rectanglei(x2, y2, width3, height3);
          QuadTree<T>.Node node2 = new QuadTree<T>.Node(quadTree2, bounds4);
          children2[1] = node2;
          QuadTree<T>.Node[] children3 = this._children;
          QuadTree<T> quadTree3 = this._quadTree;
          bounds3 = this._bounds;
          int x3 = bounds3.X + width1;
          Rectanglei bounds5 = this._bounds;
          int y3 = bounds5.Y;
          int width4 = width1;
          int height4 = height1;
          Rectanglei bounds6 = new Rectanglei(x3, y3, width4, height4);
          QuadTree<T>.Node node3 = new QuadTree<T>.Node(quadTree3, bounds6);
          children3[2] = node3;
          QuadTree<T>.Node[] children4 = this._children;
          QuadTree<T> quadTree4 = this._quadTree;
          bounds5 = this._bounds;
          Rectanglei bounds7 = new Rectanglei(bounds5.X + width1, this._bounds.Y + height1, width1, height1);
          QuadTree<T>.Node node4 = new QuadTree<T>.Node(quadTree4, bounds7);
          children4[3] = node4;
        }

        public int GetDepth()
        {
          return this._children.Length == 0 ? 1 : ((IEnumerable<QuadTree<T>.Node>) this._children).Select<QuadTree<T>.Node, int>((Func<QuadTree<T>.Node, int>) (x => x.GetDepth())).Max();
        }

        public override string ToString()
        {
          return $"{this._items.Count} items, {this._children.Length} nodes";
        }
      }
    }
}
