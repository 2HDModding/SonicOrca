﻿// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelLayerLighting
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

#nullable disable
namespace SonicOrca.Core;

public class LevelLayerLighting
{
  public LevelLayerLightingType Type { get; set; }

  public double Light { get; set; }

  public LevelLayerLighting() => this.Light = 1.0;
}
