// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.LowLevel.LowLevelExtensions
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Graphics.LowLevel
{

    public static class LowLevelExtensions
    {
      public static vec2 ToVec2(this Vector2 v)
      {
        return new vec2() { x = (float) v.X, y = (float) v.Y };
      }
    }
}
