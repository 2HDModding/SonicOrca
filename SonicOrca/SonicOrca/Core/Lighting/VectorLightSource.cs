// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Lighting.VectorLightSource
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;

namespace SonicOrca.Core.Lighting
{

    public class VectorLightSource : ILightSource
    {
      public int Intensity { get; set; }

      public Vector2i A { get; set; }

      public Vector2i B { get; set; }

      public VectorLightSource(int intensity, Vector2i a, Vector2i b)
      {
        this.Intensity = intensity;
        this.A = a;
        this.B = b;
      }

      public Vector2i GetShadowOffset(Vector2i occlusionPosition, IShadowInfo shadowInfo)
      {
        Vector2i occlusionSize = shadowInfo.OcclusionSize;
        Rectanglei rectanglei = new Rectanglei(occlusionPosition.X - occlusionSize.X, occlusionPosition.Y - occlusionSize.Y, occlusionSize.X * 2, occlusionSize.Y * 2);
        if (rectanglei.Top > this.A.Y)
          return new Vector2i();
        int num1 = 0;
        Vector2i vector2i;
        if (rectanglei.Right < this.A.X)
        {
          int num2 = num1;
          vector2i = this.A;
          int num3 = vector2i.X - rectanglei.Right;
          num1 = num2 + num3;
        }
        int left1 = rectanglei.Left;
        vector2i = this.B;
        int x1 = vector2i.X;
        if (left1 > x1)
        {
          int num4 = num1;
          int left2 = rectanglei.Left;
          vector2i = this.B;
          int x2 = vector2i.X;
          int num5 = left2 - x2;
          num1 = num4 + num5;
        }
        int num6 = num1 / 4;
        vector2i = this.A;
        return new Vector2i(0, Math.Min(0, -Math.Max(0, 48 /*0x30*/ - Math.Max(0, vector2i.Y - occlusionPosition.Y) / 16 /*0x10*/) + num6));
      }
    }
}
