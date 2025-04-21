// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ILevelTitleCard
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    public interface ILevelTitleCard : IDisposable
    {
      bool AllowLevelToStart { get; }

      bool AllowCharacterControl { get; }

      bool Finished { get; }

      bool Seamless { get; set; }

      bool UnlockCamera { get; set; }

      Task LoadAsync(ResourceTree resourceTree, CancellationToken ct = default (CancellationToken));

      void Update();

      void Draw(Renderer renderer);

      void Start();
    }
}
