// Decompiled with JetBrains decompiler
// Type: SonicOrca.Drawing.TheRenderer
// Assembly: SonicOrca.Drawing, Version=2.0.1012.10520, Culture=neutral, PublicKeyToken=null
// MVID: 31C48419-27DE-46EA-9D16-61FB91FF0FE1
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Drawing.dll

using SonicOrca.Drawing.Renderers;
using SonicOrca.Graphics;

namespace SonicOrca.Drawing
{

    public class TheRenderer(WindowContext windowContext) : Renderer(windowContext)
    {
      public override I2dRenderer Get2dRenderer()
      {
        return (I2dRenderer) SimpleRenderer.FromRenderer((Renderer) this);
      }

      public override IFontRenderer GetFontRenderer()
      {
        return (IFontRenderer) FontRenderer.FromRenderer((Renderer) this);
      }

      public override ITileRenderer GetTileRenderer()
      {
        return (ITileRenderer) TileRenderer.FromRenderer((Renderer) this);
      }

      public override IObjectRenderer GetObjectRenderer()
      {
        return (IObjectRenderer) ObjectRenderer.FromRenderer((Renderer) this);
      }

      public override ICharacterRenderer GetCharacterRenderer()
      {
        return (ICharacterRenderer) CharacterRenderer.FromRenderer((Renderer) this);
      }

      public override IWaterRenderer GetWaterRenderer()
      {
        return (IWaterRenderer) WaterRenderer.FromRenderer((Renderer) this);
      }

      public override IHeatRenderer GetHeatRenderer()
      {
        return (IHeatRenderer) HeatRenderer.FromRenderer((Renderer) this);
      }

      public override INonLayerRenderer GetNonLayerRenderer()
      {
        return (INonLayerRenderer) NonLayerRenderer.FromRenderer((Renderer) this);
      }

      public override IMaskRenderer GetMaskRenderer()
      {
        return (IMaskRenderer) MaskRenderer.FromRenderer((Renderer) this);
      }

      public override IFadeTransitionRenderer CreateFadeTransitionRenderer()
      {
        return (IFadeTransitionRenderer) new ClassicFadeTransitionRenderer(this.Window.GraphicsContext);
      }
    }
}
