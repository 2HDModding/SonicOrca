﻿// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Lighting.ILightSource
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

#nullable disable
namespace SonicOrca.Core.Lighting;

public interface ILightSource
{
  Vector2i GetShadowOffset(Vector2i position, IShadowInfo shadowInfo);
}
