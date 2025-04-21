// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.WindowContext
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;

namespace SonicOrca.Graphics
{

    public abstract class WindowContext : IDisposable
    {
      public virtual bool FullScreen { get; set; }

      public virtual VideoMode Mode { get; set; }

      public virtual string WindowTitle { get; set; }

      public virtual Vector2i ClientSize { get; set; }

      public virtual Vector2i AspectRatio { get; set; }

      public virtual bool HideCursorIfIdle { get; set; }

      public bool Finished { get; protected set; }

      public virtual IGraphicsContext GraphicsContext => (IGraphicsContext) null;

      public Rectanglei ClientBounds
      {
        get
        {
          Vector2i clientSize = this.ClientSize;
          return new Rectanglei(0, 0, clientSize.X, clientSize.Y);
        }
      }

      public abstract void Dispose();

      public virtual void Update()
      {
      }

      public virtual void BeginRender()
      {
      }

      public virtual void EndRender()
      {
      }
    }
}
