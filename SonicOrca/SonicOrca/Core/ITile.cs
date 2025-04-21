// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ITile
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;

namespace SonicOrca.Core
{

    public interface ITile
    {
      int Id { get; }

      bool Animated { get; }

      void Animate();

      void Draw(Renderer renderer, int flags, int x, int y);

      void Draw(Renderer renderer, int flags, Rectanglei source, Rectanglei destination);

      void Draw(I2dRenderer g, int flags, Rectanglei source, Rectanglei destination);

      void Draw(ITileRenderer tileRenderer, int flags, Rectanglei source, Rectanglei destination);
    }
}
