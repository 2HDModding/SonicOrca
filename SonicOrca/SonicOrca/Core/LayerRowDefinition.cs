// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LayerRowDefinition
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace SonicOrca.Core
{

    public class LayerRowDefinition
    {
      public int Width { get; set; }

      public int Height { get; set; }

      public int InitialOffset { get; set; }

      public double CurrentOffset { get; set; }

      public double Parallax { get; set; }

      public double Velocity { get; set; }

      public LayerRowDefinition() => this.Parallax = 1.0;

      public void Animate() => this.CurrentOffset += this.Velocity;
    }
}
