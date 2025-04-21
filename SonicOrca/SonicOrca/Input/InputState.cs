// Decompiled with JetBrains decompiler
// Type: SonicOrca.Input.InputState
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Input
{

    public class InputState
    {
      private readonly MouseState _mouseState;
      private readonly KeyboardState _keyboardState;
      private readonly GamePadInputState[] _gamePadInputState;

      public MouseState Mouse => this._mouseState;

      public KeyboardState Keyboard => this._keyboardState;

      public IReadOnlyList<GamePadInputState> GamePad
      {
        get => (IReadOnlyList<GamePadInputState>) this._gamePadInputState;
      }

      public InputState()
      {
        this._mouseState = new MouseState();
        this._keyboardState = new KeyboardState();
        this._gamePadInputState = Enumerable.Range(0, 4).Select<int, GamePadInputState>((Func<int, GamePadInputState>) (x => new GamePadInputState())).ToArray<GamePadInputState>();
      }

      public InputState(
        MouseState mouseState,
        KeyboardState keyboardState,
        GamePadInputState[] gamepadInputState)
      {
        this._mouseState = mouseState;
        this._keyboardState = keyboardState;
        this._gamePadInputState = gamepadInputState;
      }

      public static InputState GetPressed(InputState previousState, InputState nextState)
      {
        return new InputState(MouseState.GetPressed(previousState.Mouse, nextState.Mouse), KeyboardState.GetPressed(previousState.Keyboard, nextState.Keyboard), Enumerable.Range(0, 4).Select<int, GamePadInputState>((Func<int, GamePadInputState>) (x => GamePadInputState.GetPressed(previousState.GamePad[x], nextState.GamePad[x]))).ToArray<GamePadInputState>());
      }

      public static InputState GetReleased(InputState previousState, InputState nextState)
      {
        return new InputState(MouseState.GetReleased(previousState.Mouse, nextState.Mouse), KeyboardState.GetReleased(previousState.Keyboard, nextState.Keyboard), Enumerable.Range(0, 4).Select<int, GamePadInputState>((Func<int, GamePadInputState>) (x => GamePadInputState.GetReleased(previousState.GamePad[x], nextState.GamePad[x]))).ToArray<GamePadInputState>());
      }
    }
}
