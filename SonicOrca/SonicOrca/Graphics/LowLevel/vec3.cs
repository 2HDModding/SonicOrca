﻿// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.LowLevel.vec3
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Runtime.InteropServices;

#nullable disable
namespace SonicOrca.Graphics.LowLevel;

[VertexAttributeType(VertexAttributePointerType.Float, 3)]
[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct vec3
{
  [FieldOffset(0)]
  public float x;
  [FieldOffset(4)]
  public float y;
  [FieldOffset(8)]
  public float z;
  [FieldOffset(0)]
  public float s;
  [FieldOffset(4)]
  public float t;
  [FieldOffset(8)]
  public float p;
}
