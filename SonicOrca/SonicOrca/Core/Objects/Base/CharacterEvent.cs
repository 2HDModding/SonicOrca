// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.CharacterEvent
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Core.Objects.Base
{

    [Flags]
    public enum CharacterEvent
    {
      Fall = 1,
      Jump = 2,
      DoubleJump = 4,
      SpindashLaunch = 8,
      Fly = 16, // 0x00000010
      BarrierAttack = 32, // 0x00000020
      Hurt = 64, // 0x00000040
    }
}
