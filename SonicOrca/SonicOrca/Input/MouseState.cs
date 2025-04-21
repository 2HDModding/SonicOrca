// Decompiled with JetBrains decompiler
// Type: SonicOrca.Input.MouseState
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Input
{

    public struct MouseState
    {
      public int X { get; set; }

      public int Y { get; set; }

      public double Wheel { get; set; }

      public bool Left { get; set; }

      public bool Middle { get; set; }

      public bool Right { get; set; }

      public Vector2i Position => new Vector2i(this.X, this.Y);

      public static MouseState GetPressed(MouseState previousState, MouseState nextState)
      {
        return nextState with
        {
          Left = !previousState.Left && nextState.Left,
          Middle = !previousState.Middle && nextState.Middle,
          Right = !previousState.Right && nextState.Right
        };
      }

      public static MouseState GetReleased(MouseState previousState, MouseState nextState)
      {
        return nextState with
        {
          Left = previousState.Left && !nextState.Left,
          Middle = previousState.Middle && !nextState.Middle,
          Right = previousState.Right && !nextState.Right
        };
      }
    }
}
