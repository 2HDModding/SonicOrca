// Decompiled with JetBrains decompiler
// Type: SonicOrca.Menu.ScreenLoadingProgress
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace SonicOrca.Menu
{

    public class ScreenLoadingProgress
    {
      private readonly Screen _screen;

      public Screen Screen => this._screen;

      public int MinimumValue { get; set; }

      public int MaximumValue { get; set; }

      public int CurrentValue { get; set; }

      public string Status { get; set; }

      public ScreenLoadingProgress(Screen screen) => this._screen = screen;
    }
}
