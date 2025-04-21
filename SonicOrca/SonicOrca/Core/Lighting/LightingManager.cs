// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Lighting.LightingManager
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections.Generic;

namespace SonicOrca.Core.Lighting
{

    internal class LightingManager : ILightingManager
    {
      private readonly LinkedList<ILightSource> _lightSources = new LinkedList<ILightSource>();

      public void RegisterLightSource(ILightSource lightSource)
      {
        this._lightSources.AddLast(lightSource);
      }

      public void UnregisterLightSource(ILightSource lightSource)
      {
        this._lightSources.Remove(lightSource);
      }

      public Vector2i GetShadowOffset(Vector2i forPosition, IShadowInfo shadowInfo)
      {
        Vector2i vector2i1 = new Vector2i(int.MaxValue, int.MaxValue);
        Vector2i vector2i2 = new Vector2i(int.MinValue, int.MinValue);
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = false;
        foreach (ILightSource lightSource in this._lightSources)
        {
          Vector2i shadowOffset = lightSource.GetShadowOffset(forPosition, shadowInfo);
          if (shadowOffset.X < 0)
          {
            vector2i1.X = Math.Min(vector2i1.X, shadowOffset.X);
            flag1 = true;
          }
          else if (shadowOffset.X > 0)
          {
            vector2i2.X = Math.Max(vector2i2.X, shadowOffset.X);
            flag3 = true;
          }
          if (shadowOffset.Y < 0)
          {
            vector2i1.Y = Math.Min(vector2i1.Y, shadowOffset.Y);
            flag2 = true;
          }
          else if (shadowOffset.Y > 0)
          {
            vector2i2.Y = Math.Max(vector2i2.Y, shadowOffset.Y);
            flag4 = true;
          }
        }
        Vector2i shadowOffset1 = new Vector2i();
        if (flag1)
          shadowOffset1.X += vector2i1.X;
        if (flag3)
          shadowOffset1.X += vector2i2.X;
        if (flag1 & flag3)
          shadowOffset1.X /= 2;
        if (flag2)
          shadowOffset1.Y += vector2i1.Y;
        if (flag4)
          shadowOffset1.Y += vector2i2.Y;
        if (flag2 & flag3)
          shadowOffset1.Y /= 2;
        Vector2i maxShadowOffset = shadowInfo.MaxShadowOffset;
        shadowOffset1.X = MathX.Clamp(-maxShadowOffset.X, shadowOffset1.X, maxShadowOffset.X);
        shadowOffset1.Y = MathX.Clamp(-maxShadowOffset.Y, shadowOffset1.Y, maxShadowOffset.Y);
        return shadowOffset1;
      }

      private static int UpdateShadowOffset(int globalOffset, int localOffset)
      {
        int val1 = globalOffset;
        return val1 >= 0 ? Math.Max(Math.Max(val1, localOffset), 0) : Math.Min(Math.Min(val1, localOffset), 0);
      }
    }
}
