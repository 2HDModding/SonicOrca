// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.IRendererFactory
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

namespace SonicOrca.Graphics
{

    public interface IRendererFactory
    {
      I2dRenderer Create2dRenderer();

      IFontRenderer CreateFontRenderer();

      IFadeTransitionRenderer CreateFadeTransitionRenderer();

      ITileRenderer CreateTileRenderer();

      IObjectRenderer CreateObjectRenderer();

      ICharacterRenderer CreateCharacterRenderer();

      IWaterRenderer CreateWaterRenderer();

      IHeatRenderer CreateHeatRenderer();

      INonLayerRenderer CreateNonLayerRenderer();

      IMaskRenderer CreateMaskRenderer();
    }
}
