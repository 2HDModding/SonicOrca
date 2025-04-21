// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Collision.CollisionFlags
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Core.Collision
{

    [Flags]
    public enum CollisionFlags
    {
      Mobile = 1,
      Conveyor = 2,
      Reacts = 4,
      Rotate = 8,
      ForceRoll = 16, // 0x00000010
      PreventJump = 32, // 0x00000020
      DontFall = 64, // 0x00000040
      NoAngle = 128, // 0x00000080
      NoLanding = 256, // 0x00000100
      Snap = 512, // 0x00000200
      NoBalance = 1024, // 0x00000400
      Solid = 2048, // 0x00000800
      NoPathFollowing = 4096, // 0x00001000
      Ignore = 8192, // 0x00002000
      NoAutoAlignment = 16384, // 0x00004000
    }
}
