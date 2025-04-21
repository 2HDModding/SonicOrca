// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Sonic2LevelTitleCard
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    internal class Sonic2LevelTitleCard : ILevelTitleCard, IDisposable
    {
      private const string LogoResourceKey = "SONICORCA/HUD/TITLECARD/LOGO";
      private const string LogoDarkResourceKey = "SONICORCA/HUD/TITLECARD/LOGODARK";
      private const string TriangleResourceKey = "SONICORCA/HUD/TITLECARD/TRIANGLE";
      private const string PatternAResourceKey = "SONICORCA/HUD/TITLECARD/PATTERNA";
      private const string PatternBResourceKey = "SONICORCA/HUD/TITLECARD/PATTERNB";
      private const string GameResourceKey = "SONICORCA/HUD/TITLECARD/GAME";
      private const string LevelNameFontResourceKey = "SONICORCA/FONTS/TITLE/S2/NAME";
      private const string ActFontResourceKey = "SONICORCA/FONTS/TITLE/S2/ACT";
      private readonly Level _level;
      private ResourceSession _resourceSession;
      private ITexture _logoTexture;
      private ITexture _logoDarkTexture;
      private ITexture _triangleTexture;
      private ITexture _patternATexture;
      private ITexture _patternBTexture;
      private ITexture _gameTexture;
      private Font _levelNameFont;
      private Font _levelActFont;
      private bool _disposed;
      private int _ticks;
      private static readonly EaseTimeline BlueBlockY = new EaseTimeline(new EaseTimeline.Entry[4]
      {
        new EaseTimeline.Entry(0, 0.0),
        new EaseTimeline.Entry(18, 1080.0),
        new EaseTimeline.Entry(146, 1080.0),
        new EaseTimeline.Entry(156, 0.0)
      });
      private static readonly EaseTimeline YellowBlockX = new EaseTimeline(new EaseTimeline.Entry[4]
      {
        new EaseTimeline.Entry(12, 1920.0),
        new EaseTimeline.Entry(42, 0.0),
        new EaseTimeline.Entry(134, 0.0),
        new EaseTimeline.Entry(150, 1920.0)
      });
      private static readonly EaseTimeline RedBlockX = new EaseTimeline(new EaseTimeline.Entry[4]
      {
        new EaseTimeline.Entry(36, 0.0),
        new EaseTimeline.Entry(50, 808.0),
        new EaseTimeline.Entry(132, 808.0),
        new EaseTimeline.Entry(140, 0.0)
      });
      private static readonly EaseTimeline LevelNameX = new EaseTimeline(new EaseTimeline.Entry[4]
      {
        new EaseTimeline.Entry(32 /*0x20*/, 3840.0),
        new EaseTimeline.Entry(72, 1680.0),
        new EaseTimeline.Entry(190, 1440.0),
        new EaseTimeline.Entry(210, -16.0)
      });
      private static readonly EaseTimeline ZoneActX = new EaseTimeline(new EaseTimeline.Entry[4]
      {
        new EaseTimeline.Entry(42, 0.0),
        new EaseTimeline.Entry(72, 1440.0),
        new EaseTimeline.Entry(190, 1680.0),
        new EaseTimeline.Entry(210, 3840.0)
      });
      private static readonly EaseTimeline LevelNameHighlight = new EaseTimeline(new EaseTimeline.Entry[3]
      {
        new EaseTimeline.Entry(90, 0.0),
        new EaseTimeline.Entry(100, (double) byte.MaxValue),
        new EaseTimeline.Entry(110, 0.0)
      });
      private static readonly EaseTimeline ZoneActHighlight = new EaseTimeline(new EaseTimeline.Entry[3]
      {
        new EaseTimeline.Entry(110, 0.0),
        new EaseTimeline.Entry(120, (double) byte.MaxValue),
        new EaseTimeline.Entry(130, 0.0)
      });

      public bool AllowLevelToStart { get; private set; }

      public bool AllowCharacterControl { get; private set; }

      public bool Finished { get; private set; }

      public bool Seamless { get; set; }

      public bool UnlockCamera { get; set; }

      public Sonic2LevelTitleCard(Level level) => this._level = level;

      public async Task LoadAsync(ResourceTree resourceTree, CancellationToken ct = default (CancellationToken))
      {
        if (this._disposed)
          throw new ObjectDisposedException(nameof (Sonic2LevelTitleCard));
        this._resourceSession = new ResourceSession(resourceTree);
        this._resourceSession.PushDependencies("SONICORCA/HUD/TITLECARD/LOGO", "SONICORCA/HUD/TITLECARD/LOGODARK", "SONICORCA/HUD/TITLECARD/TRIANGLE", "SONICORCA/HUD/TITLECARD/PATTERNA", "SONICORCA/HUD/TITLECARD/PATTERNB", "SONICORCA/HUD/TITLECARD/GAME", "SONICORCA/FONTS/TITLE/S2/NAME", "SONICORCA/FONTS/TITLE/S2/ACT");
        await this._resourceSession.LoadAsync(ct);
        this._logoTexture = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TITLECARD/LOGO");
        this._logoDarkTexture = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TITLECARD/LOGODARK");
        this._triangleTexture = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TITLECARD/TRIANGLE");
        this._patternATexture = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TITLECARD/PATTERNA");
        this._patternBTexture = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TITLECARD/PATTERNB");
        this._gameTexture = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TITLECARD/GAME");
        this._levelNameFont = resourceTree.GetLoadedResource<Font>("SONICORCA/FONTS/TITLE/S2/NAME");
        this._levelActFont = resourceTree.GetLoadedResource<Font>("SONICORCA/FONTS/TITLE/S2/ACT");
      }

      public void Dispose()
      {
        if (this._resourceSession != null)
          this._resourceSession.Unload();
        this._disposed = true;
      }

      public void Start()
      {
        this._ticks = 0;
        this.AllowLevelToStart = false;
        this.AllowCharacterControl = false;
        this.UnlockCamera = false;
        this.Finished = false;
      }

      public void Update()
      {
        if (this.Finished)
          return;
        if (this.Seamless)
        {
          this.AllowLevelToStart = true;
          if (this._ticks >= 90)
            this.UnlockCamera = true;
        }
        if (this._ticks >= 120)
          this.AllowLevelToStart = true;
        if (this._ticks >= 145)
          this.AllowCharacterControl = true;
        if (this._ticks >= 250)
          this.Finished = true;
        ++this._ticks;
      }

      public void Draw(Renderer renderer)
      {
        if (this.Finished)
          return;
        if (this._ticks < 60 && !this.Seamless)
          renderer.Get2dRenderer().RenderQuad(Colours.Black, new Rectangle(0.0, 0.0, 1920.0, 1080.0));
        if (!this.Seamless)
        {
          this.DrawBlueBlock(renderer);
          this.DrawYellowBlock(renderer);
          this.DrawRedBlock(renderer);
        }
        else
        {
          this.DrawBlueBlockSeamless(renderer);
          this.DrawYellowBlockSeamless(renderer);
          this.DrawRedBlockSeamless(renderer);
        }
        this.DrawLevelName(renderer);
        this.DrawZoneAct(renderer);
      }

      private void DrawBlueBlock(Renderer renderer)
      {
        int valueAt = (int) Sonic2LevelTitleCard.BlueBlockY.GetValueAt(this._ticks);
        int num = 63 /*0x3F*/ - this._ticks % 64 /*0x40*/;
        I2dRenderer obj = renderer.Get2dRenderer();
        obj.RenderQuad(new Colour((byte) 0, (byte) 61, (byte) 155), new Rectangle(1502.0, (double) (valueAt - 1080), 418.0, 1080.0));
        obj.RenderQuad(new Colour((byte) 0, (byte) 80 /*0x50*/, (byte) 170), new Rectangle(0.0, (double) (valueAt - 1080), 1502.0, 1080.0));
        obj.Colour = new Colour((byte) 0, (byte) 80 /*0x50*/, (byte) 170);
        using (obj.BeginMatixState())
        {
          obj.ClipRectangle = new Rectangle(0.0, 0.0, 1920.0, (double) valueAt);
          for (int index = -1; index < 17; ++index)
            obj.RenderTexture(this._triangleTexture, new Rectangle(1502.0, (double) (valueAt - 1080 + index * 64 /*0x40*/ + num), 32.0, 64.0));
        }
        obj.Colour = Colours.White;
        obj.RenderTexture(this._logoTexture, new Vector2(1466.0, (double) (valueAt - 734)));
      }

      private void DrawYellowBlock(Renderer renderer)
      {
        int valueAt = (int) Sonic2LevelTitleCard.YellowBlockX.GetValueAt(this._ticks);
        int num1 = (int) sbyte.MaxValue - this._ticks % 128 /*0x80*/;
        int num2 = this._ticks % 128 /*0x80*/;
        I2dRenderer obj = renderer.Get2dRenderer();
        obj.RenderQuad(new Colour(byte.MaxValue, byte.MaxValue, (byte) 0), new Rectangle((double) valueAt, 736.0, 1920.0, 344.0));
        using (obj.BeginMatixState())
        {
          obj.ClipRectangle = new Rectangle((double) valueAt, 736.0, (double) (1920 - valueAt), 344.0);
          obj.Colour = Colours.White;
          for (int index = -1; index < 16 /*0x10*/; ++index)
            obj.RenderTexture(this._patternATexture, new Rectangle((double) (valueAt + index * 128 /*0x80*/ + num1), 736.0, 128.0, 128.0));
          obj.Colour = new Colour(byte.MaxValue, (byte) 234, (byte) 0);
          for (int index = -1; index < 16 /*0x10*/; ++index)
            obj.RenderTexture(this._patternBTexture, new Rectangle((double) (valueAt + index * 128 /*0x80*/ + num2), 892.0, 128.0, 128.0));
          obj.Colour = Colours.White;
          obj.RenderTexture(this._gameTexture, new Vector2((double) (valueAt + 1347), 800.0));
        }
      }

      private void DrawRedBlock(Renderer renderer)
      {
        int valueAt = (int) Sonic2LevelTitleCard.RedBlockX.GetValueAt(this._ticks);
        int[] numArray = new int[3]
        {
          this._ticks * 4 % 256 /*0x0100*/,
          this._ticks * 2 % 128 /*0x80*/,
          this._ticks % 64 /*0x40*/
        };
        int x = valueAt - 808;
        I2dRenderer obj = renderer.Get2dRenderer();
        obj.RenderQuad(new Colour((byte) 224 /*0xE0*/, (byte) 0, (byte) 0), new Rectangle((double) (x + 454), 0.0, 320.0, 1080.0));
        obj.RenderQuad(new Colour((byte) 191, (byte) 0, (byte) 0), new Rectangle((double) (x + 126), 0.0, 328.0, 1080.0));
        obj.RenderQuad(new Colour((byte) 168, (byte) 0, (byte) 0), new Rectangle((double) x, 0.0, 126.0, 1080.0));
        for (int index = -1; index < 17; ++index)
        {
          obj.Colour = Colours.Black;
          obj.RenderTexture(this._triangleTexture, new Rectangle((double) (x + 774), (double) (index * 64 /*0x40*/ + 16 /*0x10*/ + numArray[2]), 32.0, 64.0));
          obj.Colour = new Colour((byte) 224 /*0xE0*/, (byte) 0, (byte) 0);
          obj.RenderTexture(this._triangleTexture, new Rectangle((double) (x + 774), (double) (index * 64 /*0x40*/ + numArray[2]), 32.0, 64.0));
        }
        obj.Colour = new Colour((byte) 191, (byte) 0, (byte) 0);
        for (int index = -1; index < 9; ++index)
          obj.RenderTexture(this._triangleTexture, new Rectangle((double) (x + 454), (double) (index * 128 /*0x80*/ + numArray[1]), 64.0, 128.0));
        obj.Colour = new Colour((byte) 168, (byte) 0, (byte) 0);
        for (int index = -1; index < 5; ++index)
          obj.RenderTexture(this._triangleTexture, new Rectangle((double) (x + 126), (double) (index * 256 /*0x0100*/ + numArray[0]), 128.0, 256.0));
      }

      private void DrawBlueBlockSeamless(Renderer renderer)
      {
        int valueAt = (int) Sonic2LevelTitleCard.BlueBlockY.GetValueAt(this._ticks);
        I2dRenderer obj = renderer.Get2dRenderer();
        obj.BlendMode = BlendMode.Additive;
        obj.Colour = Colours.White;
        obj.RenderTexture(this._logoDarkTexture, new Vector2(1466.0, (double) (valueAt - 734)));
        obj.BlendMode = BlendMode.Alpha;
      }

      private void DrawYellowBlockSeamless(Renderer renderer)
      {
        int x = (int) Sonic2LevelTitleCard.YellowBlockX.GetValueAt(this._ticks) + 600;
        int num1 = (int) sbyte.MaxValue - this._ticks % 128 /*0x80*/;
        int num2 = this._ticks % 128 /*0x80*/;
        I2dRenderer obj = renderer.Get2dRenderer();
        obj.RenderQuad(Colours.Black, new Rectangle((double) x, 816.0, 1920.0, 16.0));
        obj.RenderQuad(Colours.Yellow, new Rectangle((double) x, 736.0, 1920.0, 80.0));
        using (obj.BeginMatixState())
        {
          obj.ClipRectangle = new Rectangle((double) x, 736.0, (double) (1920 - x), 128.0);
          obj.Colour = Colours.White;
          for (int index = -1; index < 16 /*0x10*/; ++index)
            obj.RenderTexture(this._patternATexture, new Rectangle((double) (x + index * 128 /*0x80*/ + num1), 718.0, 128.0, 128.0));
          obj.Colour = new Colour(byte.MaxValue, (byte) 234, (byte) 0);
          for (int index = -1; index < 16 /*0x10*/; ++index)
            obj.RenderTexture(this._patternBTexture, new Rectangle((double) (x + index * 128 /*0x80*/ + num2), 874.0, 128.0, 128.0));
          obj.Colour = Colours.White;
          obj.RenderTexture(this._gameTexture, new Vector2((double) (x + 770), 782.0));
        }
      }

      private void DrawRedBlockSeamless(Renderer renderer)
      {
        int valueAt = (int) Sonic2LevelTitleCard.RedBlockX.GetValueAt(this._ticks);
        int[] numArray = new int[3]
        {
          63 /*0x3F*/ - this._ticks % 64 /*0x40*/,
          this._ticks * 2 % 128 /*0x80*/,
          this._ticks % 64 /*0x40*/
        };
        int num = valueAt - 808;
        I2dRenderer obj = renderer.Get2dRenderer();
        obj.RenderQuad(new Colour((byte) 224 /*0xE0*/, (byte) 0, (byte) 0), new Rectangle((double) (num + 454), 0.0, 320.0, 1080.0));
        obj.RenderQuad(new Colour((byte) 168, (byte) 0, (byte) 0), new Rectangle((double) (num + 432), 0.0, 168.0, 1080.0));
        for (int index = -1; index < 17; ++index)
        {
          obj.Colour = Colours.Black;
          obj.RenderTexture(this._triangleTexture, new Rectangle((double) (num + 774), (double) (index * 64 /*0x40*/ + 16 /*0x10*/ + numArray[2]), 32.0, 64.0));
          obj.Colour = new Colour((byte) 224 /*0xE0*/, (byte) 0, (byte) 0);
          obj.RenderTexture(this._triangleTexture, new Rectangle((double) (num + 774), (double) (index * 64 /*0x40*/ + numArray[2]), 32.0, 64.0));
        }
        obj.Colour = new Colour((byte) 168, (byte) 0, (byte) 0);
        for (int index = -1; index < 9; ++index)
          obj.RenderTexture(this._triangleTexture, new Rectangle((double) (num + 600), (double) (index * 128 /*0x80*/ + numArray[1]), 64.0, 128.0));
        for (int index = -1; index < 17; ++index)
        {
          obj.Colour = Colours.Black;
          obj.RenderTexture(this._triangleTexture, new Rectangle((double) (num + 400), (double) (index * 64 /*0x40*/ + 16 /*0x10*/ + numArray[0]), 32.0, 64.0), true);
          obj.Colour = new Colour((byte) 168, (byte) 0, (byte) 0);
          obj.RenderTexture(this._triangleTexture, new Rectangle((double) (num + 400), (double) (index * 64 /*0x40*/ + numArray[0]), 32.0, 64.0), true);
        }
      }

      private void DrawLevelName(Renderer renderer)
      {
        int valueAt = (int) Sonic2LevelTitleCard.LevelNameX.GetValueAt(this._ticks);
        byte num = (byte) MathX.Clamp(0.0, Sonic2LevelTitleCard.LevelNameHighlight.GetValueAt(this._ticks), (double) byte.MaxValue);
        IFontRenderer fontRenderer = renderer.GetFontRenderer();
        int x = valueAt - (int) this._levelNameFont.MeasureString(this._level.Name.ToUpper(), new Rectangle((double) valueAt, 342.0, 0.0, 0.0), FontAlignment.Left).Width;
        string upper = this._level.Name.ToUpper();
        Rectangle boundary = new Rectangle((double) x, 342.0, 0.0, 0.0);
        Font levelNameFont = this._levelNameFont;
        Colour white = Colours.White;
        int? overlay = num > (byte) 0 ? new int?() : new int?(0);
        Vector2i? defaultShadow = this._levelNameFont.DefaultShadow;
        Colour shadowColour = new Colour(num, num, num);
        int? shadowOverlay = new int?();
        fontRenderer.RenderStringWithShadow(upper, boundary, FontAlignment.Left, levelNameFont, white, overlay, defaultShadow, shadowColour, shadowOverlay);
      }

      private void DrawZoneAct(Renderer renderer)
      {
        Rectangle boundary = new Rectangle(Sonic2LevelTitleCard.ZoneActX.GetValueAt(this._ticks) - 16.0, 486.0, 256.0, 256.0);
        byte num = (byte) MathX.Clamp(0.0, Sonic2LevelTitleCard.ZoneActHighlight.GetValueAt(this._ticks), (double) byte.MaxValue);
        IFontRenderer fontRenderer = renderer.GetFontRenderer();
        if (this._level.ShowAsAct)
        {
          boundary.X -= this._levelActFont.MeasureString(this._level.CurrentAct.ToString(), boundary, FontAlignment.Left).Width;
          fontRenderer.RenderStringWithShadow(this._level.CurrentAct.ToString(), boundary, FontAlignment.Left, this._levelActFont, Colours.White, new int?(0), this._levelActFont.DefaultShadow, new Colour(num, num, num));
          if (num > (byte) 0)
            fontRenderer.RenderString(this._level.CurrentAct.ToString(), boundary, FontAlignment.Left, this._levelActFont, new Colour(num, byte.MaxValue, byte.MaxValue, byte.MaxValue));
        }
        if (!this._level.ShowAsZone)
          return;
        boundary.X -= this._levelNameFont.MeasureString("ZONE", boundary, FontAlignment.Left).Width;
        fontRenderer.RenderStringWithShadow("ZONE", boundary, FontAlignment.Left, this._levelNameFont, Colours.White, num > (byte) 0 ? new int?() : new int?(0), this._levelNameFont.DefaultShadow, new Colour(num, num, num));
      }
    }
}
