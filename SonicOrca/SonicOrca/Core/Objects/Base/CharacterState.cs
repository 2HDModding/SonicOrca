// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.CharacterState
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Core.Objects.Base
{

    [Flags]
    public enum CharacterState
    {
      Balancing = 2,
      Pushing = 4,
      Braking = 8,
      Spinball = 16, // 0x00000010
      Rolling = 32, // 0x00000020
      Airborne = 64, // 0x00000040
      Jumping = 128, // 0x00000080
      Left = 256, // 0x00000100
      Underwater = 512, // 0x00000200
      Hurt = 1024, // 0x00000400
      Dying = 2048, // 0x00000800
      Drowning = 4096, // 0x00001000
      Dead = 8192, // 0x00002000
      Debug = 16384, // 0x00004000
      Flying = 32768, // 0x00008000
      VirtualPlatform = 65536, // 0x00010000
      SpeedShoes = 131072, // 0x00020000
      Invincible = 262144, // 0x00040000
      ForceSpinball = 524288, // 0x00080000
      Winning = 1048576, // 0x00100000
      Super = 2097152, // 0x00200000
      ChargingSpindash = 4194304, // 0x00400000
      ShouldReactToLevel = 8388608, // 0x00800000
      ObjectControlled = 16777216, // 0x01000000
    }
}
