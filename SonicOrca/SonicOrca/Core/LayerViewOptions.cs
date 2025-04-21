// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LayerViewOptions
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace SonicOrca.Core
{

    public class LayerViewOptions
    {
      public bool ShowLandscape { get; set; }

      public bool ShowObjects { get; set; }

      public bool ShowMarkers { get; set; }

      public bool ShowWater { get; set; }

      public bool ShowLandscapeCollision { get; set; }

      public bool ShowObjectCollision { get; set; }

      public bool Shadows { get; set; }

      public int Filter { get; set; }

      public double FilterAmount { get; set; }

      public LayerViewOptions()
      {
        this.ShowLandscape = true;
        this.ShowObjects = true;
        this.ShowWater = true;
      }
    }
}
