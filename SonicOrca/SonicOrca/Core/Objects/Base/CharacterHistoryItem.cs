// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.CharacterHistoryItem
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Core.Objects.Base
{

    public class CharacterHistoryItem
    {
      private readonly Vector2 _positionPrecise;
      private readonly CharacterState _state;
      private readonly CharacterInputState _input;

      public Vector2i Position => (Vector2i) this._positionPrecise;

      public Vector2 PositionPrecise => this._positionPrecise;

      public CharacterState State => this._state;

      public CharacterInputState Input => this._input;

      public CharacterHistoryItem(
        Vector2 positionPrecise,
        CharacterState state,
        CharacterInputState input)
      {
        this._positionPrecise = positionPrecise;
        this._state = state;
        this._input = input;
      }
    }
}
