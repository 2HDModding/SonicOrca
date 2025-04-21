// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Debugging.DebugContext
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Audio;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Input;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core.Debugging
{

    public class DebugContext : IDisposable
    {
      private const string FontResourceKey = "SONICORCA/FONTS/HUD";
      private const string FocusResourceKey = "SONICORCA/SOUND/TALLY/SWITCH";
      private readonly SonicOrcaGameContext _gameContext;
      private readonly Level _level;
      private readonly List<DebugPage> _pages = new List<DebugPage>();
      private DebugPage _currentPage;
      private DebugOption _currentOption;
      private ResourceSession _resourceSession;
      private Sample _focusSample;
      public const int DebugTextSilver = 0;
      public const int DebugTextGold = 1;
      public const double DebugTextNormal = 1.0;
      public const double DebugTextSmall = 0.75;
      public const double DebugTextXSmall = 0.5;
      public const double DebugTextXXSmall = 0.25;

      public Font Font { get; private set; }

      public bool Visible { get; private set; }

      public Level Level => this._level;

      public DebugOption CurrentOption => this._currentOption;

      public DebugContext(Level level)
      {
        this._gameContext = level.GameContext;
        this._level = level;
        this._pages.AddRange(DebugOptionDefinitions.CreateOptionsInOrder(this).GroupBy<DebugOption, string>((Func<DebugOption, string>) (x => x.Page)).Select<IGrouping<string, DebugOption>, DebugPage>((Func<IGrouping<string, DebugOption>, DebugPage>) (x => new DebugPage(this, x.Key, (IEnumerable<DebugOption>) x))));
        if (this._pages.Count <= 0)
          return;
        this.SelectPage(this._pages.First<DebugPage>());
      }

      public async Task LoadAsync(CancellationToken ct = default (CancellationToken))
      {
        ResourceTree resourceTree = this._gameContext.ResourceTree;
        this._resourceSession = new ResourceSession(resourceTree);
        this._resourceSession.PushDependencies("SONICORCA/FONTS/HUD", "SONICORCA/SOUND/TALLY/SWITCH");
        await this._resourceSession.LoadAsync(ct);
        this.Font = resourceTree.GetLoadedResource<Font>("SONICORCA/FONTS/HUD");
        this._focusSample = resourceTree.GetLoadedResource<Sample>("SONICORCA/SOUND/TALLY/SWITCH");
      }

      public void Dispose()
      {
        if (this._resourceSession == null)
          return;
        this._resourceSession.Dispose();
      }

      public void Update()
      {
        if (this._gameContext.Console.IsOpen)
          return;
        this.HandleInput();
      }

      private void SelectPage(DebugPage page)
      {
        this._currentPage = page;
        this._currentOption = this._currentPage.Options.FirstOrDefault<DebugOption>((Func<DebugOption, bool>) (x => x.Selectable));
      }

      private void HandleInput()
      {
        KeyboardState keyboard = this._gameContext.Input.Pressed.Keyboard;
        if (!this.Visible)
          return;
        if (keyboard[80 /*0x50*/])
          this.OnPressLeft();
        else if (keyboard[79])
          this.OnPressRight();
        if (keyboard[82])
        {
          this.OnPressUp();
        }
        else
        {
          if (!keyboard[81])
            return;
          this.OnPressDown();
        }
      }

      private void OnPressLeft()
      {
        if (this._gameContext.Input.CurrentState.Keyboard[224 /*0xE0*/])
        {
          int num = this._pages.IndexOf(this._currentPage);
          if (num <= 0)
            return;
          this.SelectPage(this._pages[num - 1]);
          this.PlayFocusSound();
        }
        else
        {
          if (this._currentOption == null)
            return;
          this._currentOption.OnPressLeft();
        }
      }

      private void OnPressRight()
      {
        if (this._gameContext.Input.CurrentState.Keyboard[224 /*0xE0*/])
        {
          int num = this._pages.IndexOf(this._currentPage);
          if (num >= this._pages.Count - 1)
            return;
          this.SelectPage(this._pages[num + 1]);
          this.PlayFocusSound();
        }
        else
        {
          if (this._currentOption == null)
            return;
          this._currentOption.OnPressRight();
        }
      }

      private void OnPressUp()
      {
        if (this._currentPage.Options.Count == 0)
          return;
        int num = this._currentPage.Options.IndexOf(this._currentOption);
        for (int index = num - 1; index >= 0; --index)
        {
          DebugOption option = this._currentPage.Options[num - 1];
          if (option.Selectable)
          {
            this._currentOption = option;
            this.PlayFocusSound();
          }
        }
      }

      private void OnPressDown()
      {
        if (this._currentPage.Options.Count == 0)
          return;
        int num = this._currentPage.Options.IndexOf(this._currentOption);
        for (int index = num + 1; index < this._currentPage.Options.Count; ++index)
        {
          DebugOption option = this._currentPage.Options[num + 1];
          if (option.Selectable)
          {
            this._currentOption = option;
            this.PlayFocusSound();
          }
        }
      }

      public void Draw(Renderer renderer)
      {
        if (!this.Visible)
          return;
        I2dRenderer obj = renderer.Get2dRenderer();
        obj.BlendMode = BlendMode.Alpha;
        obj.RenderQuad(new Colour((byte) 192 /*0xC0*/, (byte) 0, (byte) 0, (byte) 0), new Rectangle(8.0, 8.0, 1904.0, 1064.0));
        using (obj.BeginMatixState())
        {
          obj.ModelMatrix = obj.ModelMatrix.Translate(16.0, 16.0);
          obj.ClipRectangle = new Rectangle(16.0, 16.0, 1888.0, 1048.0);
          this.DrawPageTabs(renderer);
          obj.ModelMatrix = obj.ModelMatrix.Translate(0.0, (double) (this.Font.Height + 32 /*0x20*/));
          if (this._currentPage == null)
            return;
          this._currentPage.Draw(renderer);
        }
      }

      private void DrawPageTabs(Renderer renderer)
      {
        int x = 0;
        foreach (DebugPage page in this._pages)
        {
          this.DrawText(renderer, page.Name, FontAlignment.Left, (double) x, 0.0, 1.0, new int?(this._currentPage == page ? 1 : 0));
          x += (int) this.Font.MeasureString(page.Name).Width + 64 /*0x40*/;
        }
      }

      public double DrawText(
        Renderer renderer,
        string text,
        FontAlignment alignment,
        double x,
        double y,
        double scale,
        int? overlay)
      {
        return this.DrawText(renderer, text, alignment, x, y, scale, overlay, Colours.White);
      }

      public double DrawText(
        Renderer renderer,
        string text,
        FontAlignment alignment,
        double x,
        double y,
        double scale,
        Colour colour)
      {
        return this.DrawText(renderer, text, alignment, x, y, scale, new int?(), colour);
      }

      public double DrawText(
        Renderer renderer,
        string text,
        FontAlignment alignment,
        double x,
        double y,
        double scale,
        int? overlay,
        Colour colour)
      {
        I2dRenderer obj = renderer.Get2dRenderer();
        using (obj.BeginMatixState())
        {
          obj.ModelMatrix = (obj.ModelMatrix * Matrix4.CreateScale(scale, scale)).Translate(x, y);
          renderer.GetFontRenderer().RenderStringWithShadow(text, new Rectangle(), alignment, this.Font, colour, overlay);
        }
        return (double) this.Font.Height * scale + 8.0;
      }

      public void PlayFocusSound() => this._gameContext.Audio.PlaySound(this._focusSample);
    }
}
