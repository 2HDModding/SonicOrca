// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelBinding
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.Collections.Generic;

namespace SonicOrca.Core
{

    public class LevelBinding : ILoadedResource, IDisposable
    {
      private readonly List<ObjectPlacement> _objectPlacements = new List<ObjectPlacement>();

      public Resource Resource { get; set; }

      public IList<ObjectPlacement> ObjectPlacements => (IList<ObjectPlacement>) this._objectPlacements;

      public Level Level { get; set; }

      public void OnLoaded()
      {
      }

      public void Dispose()
      {
      }
    }
}
