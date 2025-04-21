// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelHud
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Objects;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    internal class LevelHud : IDisposable
    {
      private const string HudFontResourceKey = "SONICORCA/FONTS/HUD";
      private const string HudItalicFontResourceKey = "SONICORCA/FONTS/HUDITALIC";
      private const string SonicCheckeredResourceKey = "SONICORCA/HUD/CHECKERED";
      private const string TailsCheckeredResourceKey = "SONICORCA/HUD/CHECKERED/TAILS";
      private const string SonicTriangleResourceKey = "SONICORCA/HUD/TRIANGLE";
      private const string TailsTriangleResourceKey = "SONICORCA/HUD/TRIANGLE/TAILS";
      private const string KnucklesTriangleResourceKey = "SONICORCA/HUD/TRIANGLE/KNUCKLES";
      private const string SonicLifeResourceKey = "SONICORCA/HUD/LIFE/SONIC";
      private const string TailsLifeResourceKey = "SONICORCA/HUD/LIFE/TAILS";
      private const string szScore = "SCORE";
      private const string szTime = "TIME";
      private const string szRings = "RINGS";
      private readonly Level _level;
      private ResourceSession _resourceSession;
      private string _fontResourceKey;
      private Font _font;
      private ITexture _checkeredTextureSonic;
      private ITexture _checkeredTextureTails;
      private ITexture _triangleTextureSonic;
      private ITexture _triangleTextureTails;
      private ITexture _triangleTextureKnuckles;
      private ITexture _lifeTextureSonic;
      private ITexture _lifeTextureTails;
      private double _redAnimation;

      public bool ItalicFont { get; set; }

      public bool ShowMiliseconds { get; set; }

      public LevelHud(Level level)
      {
        this._level = level;
        this.ItalicFont = true;
      }

      public async Task LoadAsync(CancellationToken ct = default (CancellationToken))
      {
        ResourceTree resourceTree = this._level.GameContext.ResourceTree;
        this._fontResourceKey = this.ItalicFont ? "SONICORCA/FONTS/HUDITALIC" : "SONICORCA/FONTS/HUD";
        this._resourceSession = new ResourceSession(resourceTree);
        this._resourceSession.PushDependencies(this._fontResourceKey, "SONICORCA/HUD/CHECKERED", "SONICORCA/HUD/CHECKERED/TAILS", "SONICORCA/HUD/TRIANGLE", "SONICORCA/HUD/TRIANGLE/TAILS", "SONICORCA/HUD/TRIANGLE/KNUCKLES", "SONICORCA/HUD/LIFE/SONIC", "SONICORCA/HUD/LIFE/TAILS");
        await this._resourceSession.LoadAsync(ct);
        this._font = resourceTree.GetLoadedResource<Font>(this._fontResourceKey);
        this._checkeredTextureSonic = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/CHECKERED");
        this._checkeredTextureTails = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/CHECKERED/TAILS");
        this._triangleTextureSonic = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TRIANGLE");
        this._triangleTextureTails = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TRIANGLE/TAILS");
        this._triangleTextureKnuckles = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TRIANGLE/KNUCKLES");
        this._lifeTextureSonic = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/LIFE/SONIC");
        this._lifeTextureTails = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/LIFE/TAILS");
      }

      public void Dispose()
      {
        if (this._resourceSession == null)
          return;
        this._resourceSession.Dispose();
      }

      public void Animate() => this._redAnimation = (this._redAnimation + 0.05) % 2.0;

      public void Draw(Renderer renderer)
      {
        ICharacter protagonist = this._level.Player.Protagonist;
        if (protagonist != null && protagonist.IsDebug)
          this.DrawDebug(renderer);
        else
          this.DrawNormal(renderer);
      }

      private void DrawNormal(Renderer renderer)
      {
        this.DrawTLInfo(renderer, 226.0, 610.0, 98.0, "SCORE", this._level.Player.Score.ToString());
        int num1 = this._level.Time / 60 / 60;
        int num2 = this._level.Time / 60 % 60;
        int num3 = this._level.Time % 60 * 100 / 60;
        if (this.ShowMiliseconds)
          this.DrawTLInfo(renderer, 226.0, 400.0, 162.0, "TIME", $"{num1}'{num2:00}\"{num3:00}", num1 >= 9, false);
        else
          this.DrawTLInfo(renderer, 226.0, 400.0, 162.0, "TIME", $"{num1}:{num2:00}", num1 >= 9, false);
        this.DrawTLInfo(renderer, 226.0, 516.0, 226.0, "RINGS", this._level.Player.CurrentRings.ToString(), this._level.Player.CurrentRings == 0);
        if (this._level.Player.Lives < 0)
          return;
        this.DrawLives(renderer);
      }

      private void DrawDebug(Renderer renderer)
      {
        ICharacter protagonist = this._level.Player.Protagonist;
        this.DrawTLInfoHexCoordinate(renderer, 226.0, 98.0, "PLAYER", protagonist.Position);
        this.DrawTLInfoHexCoordinate(renderer, 226.0, 162.0, "SCROLL", (Vector2i) this._level.Camera.Bounds.TopLeft);
        this.DrawBottomText(renderer, "DEBUG MODE");
      }

      private void DrawTLInfoHexCoordinate(
        Renderer renderer,
        double captionLeft,
        double captionTop,
        string caption,
        Vector2i coordinate)
      {
        this.DrawTLInfo(renderer, captionLeft, captionLeft + 630.0, captionTop, caption, $"{coordinate.X & 1048575 /*0x0FFFFF*/:X5} {coordinate.Y & 1048575 /*0x0FFFFF*/:X5}");
      }

      private void DrawTLInfo(
        Renderer renderer,
        double captionLeft,
        double valueRight,
        double top,
        string caption,
        string value,
        bool redAnimate = false,
        bool rightAligned = true)
      {
        I2dRenderer obj = renderer.Get2dRenderer();
        IFontRenderer fontRenderer = renderer.GetFontRenderer();
        obj.BlendMode = BlendMode.Alpha;
        obj.Colour = new Colour((byte) 96 /*0x60*/, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        if (this._level.Player.ProtagonistCharacterType == CharacterType.Sonic)
        {
          obj.RenderTexture(this._checkeredTextureSonic, new Vector2(captionLeft - (double) (this._checkeredTextureSonic.Width / 2) - 8.0, top + (double) this._font.Height / 2.0));
        }
        else
        {
          if (this._level.Player.ProtagonistCharacterType != CharacterType.Tails)
            throw new NotImplementedException();
          obj.RenderTexture(this._checkeredTextureTails, new Vector2(captionLeft - (double) (this._checkeredTextureTails.Width / 2) - 8.0, top + (double) this._font.Height / 2.0));
        }
        if (rightAligned)
          fontRenderer.RenderStringWithShadow(value, new Rectangle(0.0, top, valueRight, 0.0), FontAlignment.Right, this._font, 0);
        else
          fontRenderer.RenderStringWithShadow(value, new Rectangle(valueRight, top, 0.0, 0.0), FontAlignment.Left, this._font, 0);
        Rectangle boundary = new Rectangle(captionLeft, top, 0.0, 0.0);
        FontAlignment fontAlignment = FontAlignment.Left;
        Rectangle rectangle = this._font.MeasureString(caption, boundary, fontAlignment);
        obj.Colour = new Colour((byte) 155, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        if (this._level.Player.ProtagonistCharacterType == CharacterType.Sonic)
          obj.RenderTexture(this._triangleTextureSonic, new Vector2(rectangle.Right - 8.0, rectangle.Bottom - 4.0));
        else if (this._level.Player.ProtagonistCharacterType == CharacterType.Tails)
          obj.RenderTexture(this._triangleTextureTails, new Vector2(rectangle.Right - 8.0, rectangle.Bottom - 4.0));
        else
          obj.RenderTexture(this._triangleTextureKnuckles, new Vector2(rectangle.Right - 8.0, rectangle.Bottom - 4.0));
        Colour colour = Colours.White;
        if (redAnimate)
        {
          double num = this._redAnimation > 1.0 ? 2.0 - this._redAnimation : this._redAnimation;
          colour = new Colour(1.0, 1.0 - num, 1.0 - num);
        }
        fontRenderer.RenderStringWithShadow(caption, new Rectangle(captionLeft, top, 0.0, 0.0), fontAlignment, this._font, colour, new int?(1));
      }

      private void DrawLives(Renderer renderer)
      {
        I2dRenderer obj = renderer.Get2dRenderer();
        IFontRenderer fontRenderer = renderer.GetFontRenderer();
        obj.Colour = Colours.White;
        if (this._level.Player.ProtagonistCharacterType == CharacterType.Sonic)
        {
          obj.RenderTexture(this._lifeTextureSonic, new Vector2(264.0, 958.0));
        }
        else
        {
          if (this._level.Player.ProtagonistCharacterType != CharacterType.Tails)
            throw new NotImplementedException();
          obj.RenderTexture(this._lifeTextureTails, new Vector2(264.0, 958.0));
        }
        fontRenderer.RenderStringWithShadow("×", new Rectangle(300.0, 934.0, 0.0, 0.0), FontAlignment.Left, this._font, 1);
        fontRenderer.RenderStringWithShadow(this._level.Player.Lives.ToString(), new Rectangle(370.0, 934.0, 0.0, 0.0), FontAlignment.Left, this._font, 0);
      }

      private void DrawBottomText(Renderer renderer, string text)
      {
        I2dRenderer obj = renderer.Get2dRenderer();
        IFontRenderer fontRenderer = renderer.GetFontRenderer();
        obj.Colour = Colours.White;
        if (this._level.Player.ProtagonistCharacterType == CharacterType.Sonic)
        {
          obj.RenderTexture(this._lifeTextureSonic, new Vector2(264.0, 958.0));
        }
        else
        {
          if (this._level.Player.ProtagonistCharacterType != CharacterType.Tails)
            throw new NotImplementedException();
          obj.RenderTexture(this._lifeTextureTails, new Vector2(264.0, 958.0));
        }
        fontRenderer.RenderStringWithShadow(text, new Rectangle(310.0, 934.0, 0.0, 0.0), FontAlignment.Left, this._font, 1);
      }
    }
}
