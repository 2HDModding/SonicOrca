// Decompiled with JetBrains decompiler
// Type: SonicOrca.Controller
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Input;

namespace SonicOrca
{

    public class Controller
    {
      private readonly int _index;
      private readonly SonicOrcaGameContext _gameContext;
      private readonly InputStateEventType _eventType;

      public static bool IsDebug { get; set; }

      public Vector2 DirectionLeft { get; private set; }

      public Vector2 DirectionRight { get; private set; }

      public bool Start { get; private set; }

      public bool Action1 { get; private set; }

      public bool Action2 { get; private set; }

      public bool Action3 { get; private set; }

      public double LeftTrigger { get; private set; }

      public double RightTrigger { get; private set; }

      public int Index => this._index;

      public Controller(SonicOrcaGameContext gameContext, int index, InputStateEventType eventType)
      {
        this._gameContext = gameContext;
        this._index = index;
        this._eventType = eventType;
      }

      public void Update()
      {
        GamePadInputState gamePadInputState = this._gameContext.Input.GetInputState(this._eventType).GamePad[this._index];
        if (this._index == 0)
        {
          KeyboardState keyboard = this._gameContext.Input.GetInputState(this._eventType).Keyboard;
          Vector2 vector2 = new Vector2();
          if (keyboard.Keys[80 /*0x50*/])
            vector2.X = -1.0;
          else if (keyboard.Keys[79])
            vector2.X = 1.0;
          if (keyboard.Keys[82])
            vector2.Y = -1.0;
          else if (keyboard.Keys[81])
            vector2.Y = 1.0;
          if (vector2 == new Vector2())
            vector2 = !(gamePadInputState.LeftAxis == new Vector2()) ? gamePadInputState.LeftAxis : (Vector2) gamePadInputState.POV;
          this.DirectionLeft = vector2;
          this.Action1 = gamePadInputState.West || gamePadInputState.North;
          this.Action2 = gamePadInputState.South;
          this.Action3 = gamePadInputState.East;
          this.Action1 = this.Action1 || keyboard.Keys[4];
          this.Action2 = this.Action2 || keyboard.Keys[22];
          this.Action3 = this.Action3 || keyboard.Keys[7];
          this.Start = gamePadInputState.Start;
          this.Start = this.Start || keyboard.Keys[40];
          this.LeftTrigger = gamePadInputState.LeftTrigger;
          this.RightTrigger = gamePadInputState.RightTrigger;
          this.DirectionRight = gamePadInputState.RightAxis;
        }
        else if (this._index == 1)
        {
          KeyboardState keyboard = this._gameContext.Input.GetInputState(this._eventType).Keyboard;
          Vector2 vector2 = new Vector2();
          if (keyboard.Keys[13])
            vector2.X = -1.0;
          else if (keyboard.Keys[15])
            vector2.X = 1.0;
          if (keyboard.Keys[12])
            vector2.Y = -1.0;
          else if (keyboard.Keys[14])
            vector2.Y = 1.0;
          if (vector2 == new Vector2())
            vector2 = !(gamePadInputState.LeftAxis == new Vector2()) ? gamePadInputState.LeftAxis : (Vector2) gamePadInputState.POV;
          this.DirectionLeft = vector2;
          this.Action1 = gamePadInputState.West || gamePadInputState.North;
          this.Action2 = gamePadInputState.South;
          this.Action3 = gamePadInputState.East;
          this.DirectionLeft = vector2;
          this.Action1 = gamePadInputState.West || gamePadInputState.North;
          this.Action2 = gamePadInputState.South;
          this.Action3 = gamePadInputState.East;
          this.Action1 = this.Action1 || keyboard.Keys[20];
          this.Action2 = this.Action2 || keyboard.Keys[26];
          this.Action3 = this.Action3 || keyboard.Keys[8];
          this.Start = gamePadInputState.Start;
          this.Start = this.Start || keyboard.Keys[40];
          this.LeftTrigger = gamePadInputState.LeftTrigger;
          this.RightTrigger = gamePadInputState.RightTrigger;
          this.DirectionRight = gamePadInputState.RightAxis;
        }
        if (Controller.IsDebug)
          return;
        this.Action1 = this.Action1 || this.Action2 || this.Action3;
      }
    }
}
