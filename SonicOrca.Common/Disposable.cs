// Decompiled with JetBrains decompiler
// Type: SonicOrca.Disposable
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System;

namespace SonicOrca
{

    public class Disposable : IDisposable
    {
      private readonly Action _action;

      private Disposable(Action action) => this._action = action;

      public void Dispose()
      {
        if (this._action == null)
          return;
        this._action();
      }

      public static IDisposable FromAction(Action action) => (IDisposable) new Disposable(action);
    }
}
