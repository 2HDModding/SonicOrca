// Decompiled with JetBrains decompiler
// Type: SonicOrca.Menu.FadeTransition
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;

namespace SonicOrca.Menu
{

    public class FadeTransition
    {
      private bool _decreasingOpacity;

      public double Opacity { get; private set; }

      public Colour Colour { get; set; }

      public int Duration { get; set; }

      public int UpdatesRemaining { get; set; }

      public bool Finished => this.UpdatesRemaining <= 0;

      public FadeTransition(int duration)
        : this(Colours.Black, duration)
      {
      }

      public FadeTransition(Colour colour, int duration)
      {
        this.Colour = colour;
        this.Duration = duration;
      }

      public void Clear()
      {
        this.Opacity = 0.0;
        this.UpdatesRemaining = 0;
      }

      public void Set()
      {
        this.Opacity = 1.0;
        this.UpdatesRemaining = 0;
      }

      public void BeginFadeIn()
      {
        this._decreasingOpacity = true;
        this.UpdatesRemaining = this.Duration;
        this.Update();
      }

      public void BeginFadeOut()
      {
        this._decreasingOpacity = false;
        this.UpdatesRemaining = this.Duration;
        this.Update();
      }

      public void Update()
      {
        if (this.UpdatesRemaining <= 0)
          return;
        this.Opacity = (double) this.UpdatesRemaining / (double) this.Duration;
        if (!this._decreasingOpacity)
          this.Opacity = 1.0 - this.Opacity;
        --this.UpdatesRemaining;
      }

      public void Draw(Renderer renderer)
      {
        I2dRenderer obj = renderer.Get2dRenderer();
        Colour colour = this.Colour with
        {
          Alpha = (byte) (this.Opacity * (double) byte.MaxValue)
        };
        obj.BlendMode = BlendMode.Alpha;
        obj.RenderQuad(colour, new Rectangle(0.0, 0.0, 1920.0, 1080.0));
      }
    }
}
