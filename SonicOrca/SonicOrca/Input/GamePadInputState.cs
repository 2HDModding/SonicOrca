// Decompiled with JetBrains decompiler
// Type: SonicOrca.Input.GamePadInputState
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Input
{

    public struct GamePadInputState
    {
      public Vector2i POV { get; set; }

      public Vector2 LeftAxis { get; set; }

      public Vector2 RightAxis { get; set; }

      public bool LeftAxisButton { get; set; }

      public bool RightAxisButton { get; set; }

      public bool North { get; set; }

      public bool East { get; set; }

      public bool South { get; set; }

      public bool West { get; set; }

      public bool Start { get; set; }

      public bool Select { get; set; }

      public bool LeftBumper { get; set; }

      public bool RightBumper { get; set; }

      public double LeftTrigger { get; set; }

      public double RightTrigger { get; set; }

      public static GamePadInputState GetPressed(GamePadInputState prev, GamePadInputState next)
      {
        return next with
        {
          POV = GamePadInputState.GetAxisStateChanged(prev.POV, next.POV),
          LeftAxis = GamePadInputState.GetAxisStateChanged(prev.LeftAxis, next.LeftAxis),
          RightAxis = GamePadInputState.GetAxisStateChanged(prev.RightAxis, next.RightAxis),
          LeftAxisButton = !prev.LeftAxisButton && next.LeftAxisButton,
          RightAxisButton = !prev.RightAxisButton && next.RightAxisButton,
          North = !prev.North && next.North,
          East = !prev.East && next.East,
          South = !prev.South && next.South,
          West = !prev.West && next.West,
          Start = !prev.Start && next.Start,
          Select = !prev.Select && next.Select,
          LeftBumper = !prev.LeftBumper && next.LeftBumper,
          RightBumper = !prev.RightBumper && next.RightBumper
        };
      }

      public static GamePadInputState GetReleased(GamePadInputState prev, GamePadInputState next)
      {
        return next with
        {
          POV = GamePadInputState.GetAxisStateChanged(prev.POV, next.POV),
          LeftAxis = GamePadInputState.GetAxisStateChanged(prev.LeftAxis, next.LeftAxis),
          RightAxis = GamePadInputState.GetAxisStateChanged(prev.RightAxis, next.RightAxis),
          LeftAxisButton = prev.LeftAxisButton && !next.LeftAxisButton,
          RightAxisButton = prev.RightAxisButton && !next.RightAxisButton,
          North = prev.North && !next.North,
          East = prev.East && !next.East,
          West = prev.West && !next.West,
          Start = prev.Start && !next.Start,
          Select = prev.Select && !next.Select,
          LeftBumper = prev.LeftBumper && !next.LeftBumper,
          RightBumper = prev.RightBumper && !next.RightBumper
        };
      }

      private static Vector2i GetAxisStateChanged(Vector2i prev, Vector2i next)
      {
        Vector2i axisStateChanged = next;
        if (prev.X == next.X)
          axisStateChanged.X = 0;
        if (prev.Y == next.Y)
          axisStateChanged.Y = 0;
        return axisStateChanged;
      }

      private static Vector2 GetAxisStateChanged(Vector2 prev, Vector2 next)
      {
        int direction1 = GamePadInputState.GetDirection(prev.X);
        int direction2 = GamePadInputState.GetDirection(prev.Y);
        int direction3 = GamePadInputState.GetDirection(next.X);
        int direction4 = GamePadInputState.GetDirection(next.Y);
        Vector2 axisStateChanged = next;
        int num = direction3;
        if (direction1 == num)
          axisStateChanged.X = 0.0;
        if (direction2 == direction4)
          axisStateChanged.Y = 0.0;
        return axisStateChanged;
      }

      private static int GetDirection(double value)
      {
        if (value <= -0.2)
          return -1;
        return value >= 0.2 ? 1 : 0;
      }
    }
}
