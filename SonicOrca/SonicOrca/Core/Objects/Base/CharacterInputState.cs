// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.CharacterInputState
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Core.Objects.Base
{

    public class CharacterInputState
    {
      private int _verticalDirection;

      private double _throttle { get; set; }

      public double Throttle
      {
        get => this._throttle;
        set => this._throttle = MathX.Clamp(-1.0, value, 1.0);
      }

      public int HorizontalDirection
      {
        get => Math.Sign(this.Throttle);
        set => this.Throttle = (double) Math.Sign(value);
      }

      public int VerticalDirection
      {
        get => this._verticalDirection;
        set => this._verticalDirection = Math.Sign(value);
      }

      public CharacterInputButtonState A { get; set; }

      public CharacterInputButtonState B { get; set; }

      public CharacterInputButtonState C { get; set; }

      public CharacterInputButtonState ABC
      {
        get
        {
          return this.A == CharacterInputButtonState.Pressed || this.B == CharacterInputButtonState.Pressed || this.C == CharacterInputButtonState.Pressed ? CharacterInputButtonState.Pressed : (this.A | this.B | this.C) & CharacterInputButtonState.Down;
        }
      }

      public CharacterInputState()
      {
      }

      public CharacterInputState(CharacterInputState state)
      {
        this._throttle = state._throttle;
        this._verticalDirection = state._verticalDirection;
        this.A = state.A;
        this.B = state.B;
        this.C = state.C;
      }

      public void Clear()
      {
        this._throttle = 0.0;
        this._verticalDirection = 0;
        this.A = CharacterInputButtonState.Up;
        this.B = CharacterInputButtonState.Up;
        this.C = CharacterInputButtonState.Up;
      }
    }
}
