// Decompiled with JetBrains decompiler
// Type: SonicOrca.UpdateResult
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace SonicOrca
{

    public struct UpdateResult
    {
      private readonly UpdateResultType _type;
      private readonly int _waitTicks;

      public UpdateResultType Type => this._type;

      public int WaitTicks => this._waitTicks;

      private UpdateResult(UpdateResultType type, int waitTicks = 0)
      {
        this._type = type;
        this._waitTicks = waitTicks;
      }

      public static UpdateResult Next() => new UpdateResult(UpdateResultType.Next);

      public static UpdateResult Wait(int ticks) => new UpdateResult(UpdateResultType.Wait, ticks);
    }
}
