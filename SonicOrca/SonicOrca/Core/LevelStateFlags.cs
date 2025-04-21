// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelStateFlags
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Core
{

    [Flags]
    public enum LevelStateFlags
    {
      Animating = 2,
      Updating = 4,
      AllowCharacterControl = 8,
      FadingIn = 16, // 0x00000010
      FadingOut = 32, // 0x00000020
      CompletingStage = 64, // 0x00000040
      Restarting = 128, // 0x00000080
      UpdateTime = 256, // 0x00000100
      TitleCardActive = 512, // 0x00000200
      Editing = 1024, // 0x00000400
      Dead = 2048, // 0x00000800
      StageCompleted = 4096, // 0x00001000
      TimeOver = 8192, // 0x00002000
      GameOver = 16384, // 0x00004000
      TimeUp = 32768, // 0x00008000
      Paused = 65536, // 0x00010000
      WaitingForCharacterToWin = 131072, // 0x00020000
    }
}
