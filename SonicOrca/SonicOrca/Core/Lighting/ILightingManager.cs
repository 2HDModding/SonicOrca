// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Lighting.ILightingManager
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Core.Lighting
{

    public interface ILightingManager
    {
      void RegisterLightSource(ILightSource lightSource);

      void UnregisterLightSource(ILightSource lightSource);

      Vector2i GetShadowOffset(Vector2i forPosition, IShadowInfo shadowInfo);
    }
}
