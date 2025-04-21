// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.CharacterIntelligence
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Objects;
using SonicOrca.Core.Objects.Base;

namespace SonicOrca.Core
{

    public static class CharacterIntelligence
    {
      public static void SimpleMoveRightJumpObsticals(ICharacter character)
      {
        CharacterInputState characterInputState = new CharacterInputState();
        characterInputState.HorizontalDirection = 1;
        if (character.IsAirborne)
          characterInputState.A = CharacterInputButtonState.Down;
        else if (!character.IsAirborne && character.IsPushing)
          characterInputState.A = CharacterInputButtonState.Pressed;
        character.Input = characterInputState;
      }
    }
}
