// Decompiled with JetBrains decompiler
// Type: SonicOrca.Input.InputContext
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Input
{

    public abstract class InputContext
    {
      public InputState LastState { get; protected set; }

      public InputState CurrentState { get; protected set; }

      public InputState Pressed { get; private set; }

      public InputState Released { get; private set; }

      public OutputState OutputState { get; set; }

      public bool IsVibrationEnabled { get; set; }

      public string TextInput { get; protected set; }

      protected InputContext()
      {
        this.LastState = new InputState();
        this.CurrentState = new InputState();
        this.Pressed = new InputState();
        this.Released = new InputState();
        this.OutputState = new OutputState();
      }

      public virtual void UpdateCurrentState()
      {
      }

      public void Update()
      {
        this.CurrentState = new InputState();
        this.UpdateCurrentState();
        this.OutputState = new OutputState();
      }

      public void UpdatePressedReleased()
      {
        this.Pressed = InputState.GetPressed(this.LastState, this.CurrentState);
        this.Released = InputState.GetReleased(this.LastState, this.CurrentState);
        this.LastState = this.CurrentState;
      }

      public InputState GetInputState(InputStateEventType eventType)
      {
        switch (eventType)
        {
          case InputStateEventType.Current:
            return this.CurrentState;
          case InputStateEventType.Pressed:
            return this.Pressed;
          case InputStateEventType.Released:
            return this.Released;
          default:
            throw new ArgumentException("Invalid event type", nameof (eventType));
        }
      }

      public abstract char GetKeyCode(int scancode);
    }
}
