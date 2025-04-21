// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.IActiveObject
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Core
{

    public interface IActiveObject
    {
      Level Level { get; }

      ObjectType Type { get; }

      ObjectEntry Entry { get; }

      LevelLayer Layer { get; set; }

      int Priority { get; set; }

      Vector2i Position { get; set; }

      Vector2 PositionPrecise { get; set; }

      Vector2i LastPosition { get; }

      Vector2 LastPositionPrecise { get; }

      float Brightness { get; set; }

      void Finish();

      void FinishForever();
    }
}
