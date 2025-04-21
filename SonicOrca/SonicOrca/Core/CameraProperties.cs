// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.CameraProperties
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Core
{

    public class CameraProperties
    {
      public Rectangle Box { get; set; }

      public Vector2i Delay { get; set; }

      public Vector2 MaxVelocity { get; set; }

      public Vector2 Offset { get; set; }
    }
}
