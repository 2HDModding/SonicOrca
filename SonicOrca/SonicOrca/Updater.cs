// Decompiled with JetBrains decompiler
// Type: SonicOrca.Updater
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Collections.Generic;

namespace SonicOrca
{

    public class Updater
    {
      private readonly IEnumerator<UpdateResult> _updateStateEnumerator;
      private int _updateStateWaitTicks;

      public Updater(IEnumerable<UpdateResult> updateMethod)
      {
        this._updateStateEnumerator = updateMethod.GetEnumerator();
      }

      public bool Update()
      {
        if (this._updateStateWaitTicks > 0)
        {
          --this._updateStateWaitTicks;
          return true;
        }
        if (!this._updateStateEnumerator.MoveNext())
          return false;
        UpdateResult current = this._updateStateEnumerator.Current;
        switch (current.Type)
        {
          case UpdateResultType.Wait:
            this._updateStateWaitTicks = current.WaitTicks;
            return true;
          default:
            return true;
        }
      }
    }
}
