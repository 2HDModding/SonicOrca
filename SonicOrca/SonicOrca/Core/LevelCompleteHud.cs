// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelCompleteHud
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Audio;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    internal class LevelCompleteHud : IDisposable
    {
      private const string CaptionFontResourceKey = "SONICORCA/FONTS/TITLE/S2/NAME";
      private const string CaptionActFontResourceKey = "SONICORCA/FONTS/TITLE/S2/ACT";
      private const string StatisticsFontResourceKey = "SONICORCA/FONTS/HUD";
      private const string SonicTriangleResourceKey = "SONICORCA/HUD/TRIANGLE";
      private const string TailsTriangleResourceKey = "SONICORCA/HUD/TRIANGLE/TAILS";
      private const string KnucklesTriangleResourceKey = "SONICORCA/HUD/TRIANGLE/KNUCKLES";
      private const string JingleResourceKey = "SONICORCA/MUSIC/JINGLE/STAGECLEAR/S1";
      private const string TallySwitchResourceKey = "SONICORCA/SOUND/TALLY/SWITCH";
      private const string TalleEndResourceKey = "SONICORCA/SOUND/TALLY/END";
      private static IReadOnlyList<int> TimeBonusScores = (IReadOnlyList<int>) new int[21]
      {
        50000,
        50000,
        10000,
        5000,
        4000,
        4000,
        3000,
        3000,
        2000,
        2000,
        2000,
        2000,
        1000,
        1000,
        1000,
        1000,
        500,
        500,
        500,
        500,
        0
      };
      private const int BankFrequency = 120;
      private const int BankAmount = 100;
      private readonly Level _level;
      private ResourceSession _resourceSession;
      private Font _captionFont;
      private Font _captionActFont;
      private Font _statisticsFont;
      private ITexture _triangleTextureSonic;
      private ITexture _triangleTextureTails;
      private ITexture _triangleTextureKnuckles;
      private Sample _jingleSample;
      private Sample _tallySwitchSample;
      private Sample _tallyEndSample;
      private LevelCompleteHud.State _state;
      private int _ticks;
      private int _flyTicks;
      private int _shineTicks;
      private int _bankPointsDelay;
      private bool _achievedPerfect;
      private int _total;
      private int _tallySwitchDelay;
      private EaseTimeline _captionAFly;
      private EaseTimeline _captionBFly;
      private EaseTimeline _timeBonusFly;
      private EaseTimeline _ringBonusFly;
      private EaseTimeline _perfectBonusFly;
      private EaseTimeline _totalFly;
      private static readonly EaseTimeline CaptionAFlyIn = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(0, -1920.0),
        new EaseTimeline.Entry(40, 0.0)
      });
      private static readonly EaseTimeline CaptionBFlyIn = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(0, 1920.0),
        new EaseTimeline.Entry(40, 0.0)
      });
      private static readonly EaseTimeline TimeBonusFlyIn = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(40, 1920.0),
        new EaseTimeline.Entry(80 /*0x50*/, 0.0)
      });
      private static readonly EaseTimeline RingBonusFlyIn = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(45, 1920.0),
        new EaseTimeline.Entry(85, 0.0)
      });
      private static readonly EaseTimeline PerfectBonusFlyIn = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(50, 1920.0),
        new EaseTimeline.Entry(90, 0.0)
      });
      private static readonly EaseTimeline TotalFlyIn = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(55, 1920.0),
        new EaseTimeline.Entry(95, 0.0)
      });
      private static readonly EaseTimeline TotalFlyOut = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(0, 0.0),
        new EaseTimeline.Entry(40, 1920.0)
      });
      private static readonly EaseTimeline PerfectBonusFlyOut = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(5, 0.0),
        new EaseTimeline.Entry(45, 1920.0)
      });
      private static readonly EaseTimeline RingBonusFlyOut = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(10, 0.0),
        new EaseTimeline.Entry(50, 1920.0)
      });
      private static readonly EaseTimeline TimeBonusFlyOut = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(15, 0.0),
        new EaseTimeline.Entry(55, 1920.0)
      });
      private static readonly EaseTimeline CaptionBFlyOut = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(0, 0.0),
        new EaseTimeline.Entry(40, 1920.0)
      });
      private static readonly EaseTimeline CaptionAFlyOut = new EaseTimeline(new EaseTimeline.Entry[2]
      {
        new EaseTimeline.Entry(0, 0.0),
        new EaseTimeline.Entry(40, -1920.0)
      });
      private static readonly EaseTimeline TotalShine = new EaseTimeline(new EaseTimeline.Entry[3]
      {
        new EaseTimeline.Entry(0, 0.0),
        new EaseTimeline.Entry(10, (double) byte.MaxValue),
        new EaseTimeline.Entry(20, 0.0)
      });

      public int TimeBonus { get; set; }

      public int RingBonus { get; set; }

      public int PerfectBonus { get; set; }

      public bool Finished => this._state == LevelCompleteHud.State.Finished;

      public LevelCompleteHud(Level level) => this._level = level;

      public async Task LoadAsync(CancellationToken ct = default (CancellationToken))
      {
        ResourceTree resourceTree = this._level.GameContext.ResourceTree;
        this._resourceSession = new ResourceSession(resourceTree);
        this._resourceSession.PushDependencies("SONICORCA/FONTS/TITLE/S2/NAME", "SONICORCA/FONTS/TITLE/S2/ACT", "SONICORCA/FONTS/HUD", "SONICORCA/HUD/TRIANGLE", "SONICORCA/HUD/TRIANGLE/TAILS", "SONICORCA/HUD/TRIANGLE/KNUCKLES", "SONICORCA/MUSIC/JINGLE/STAGECLEAR/S1", "SONICORCA/SOUND/TALLY/SWITCH", "SONICORCA/SOUND/TALLY/END");
        await this._resourceSession.LoadAsync(ct);
        this._captionFont = resourceTree.GetLoadedResource<Font>("SONICORCA/FONTS/TITLE/S2/NAME");
        this._captionActFont = resourceTree.GetLoadedResource<Font>("SONICORCA/FONTS/TITLE/S2/ACT");
        this._statisticsFont = resourceTree.GetLoadedResource<Font>("SONICORCA/FONTS/HUD");
        this._triangleTextureSonic = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TRIANGLE");
        this._triangleTextureTails = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TRIANGLE/TAILS");
        this._triangleTextureKnuckles = resourceTree.GetLoadedResource<ITexture>("SONICORCA/HUD/TRIANGLE/KNUCKLES");
        this._jingleSample = resourceTree.GetLoadedResource<Sample>("SONICORCA/MUSIC/JINGLE/STAGECLEAR/S1");
        this._tallySwitchSample = resourceTree.GetLoadedResource<Sample>("SONICORCA/SOUND/TALLY/SWITCH");
        this._tallyEndSample = resourceTree.GetLoadedResource<Sample>("SONICORCA/SOUND/TALLY/END");
      }

      public void Dispose()
      {
        if (this._resourceSession == null)
          return;
        this._resourceSession.Dispose();
      }

      public void Start()
      {
        this.TimeBonus = LevelCompleteHud.TimeBonusScores[Math.Min(this._level.Time / 60 / 15, ((IReadOnlyCollection<int>) LevelCompleteHud.TimeBonusScores).Count - 1)];
        this.RingBonus = this._level.Player.CurrentRings * 100;
        if (this._level.RingsCollected < this._level.RingsPerfectTarget)
        {
          this._achievedPerfect = false;
          this.PerfectBonus = 0;
        }
        else
        {
          this._achievedPerfect = true;
          this.PerfectBonus = 50000;
        }
        this._total = 0;
        this._state = LevelCompleteHud.State.PreDelay;
        this._level.SoundManager.FadeOutMusic(60);
      }

      public void Update()
      {
        switch (this._state)
        {
          case LevelCompleteHud.State.PreDelay:
            ++this._ticks;
            if (this._ticks < 90)
              break;
            this._state = LevelCompleteHud.State.FlyIn;
            this._flyTicks = 0;
            this.SetFlyInEasings();
            this._level.SoundManager.PlayJingleOnce(this._jingleSample);
            break;
          case LevelCompleteHud.State.FlyIn:
            ++this._flyTicks;
            if (this._flyTicks >= 340)
            {
              this._state = LevelCompleteHud.State.Banking;
              this._bankPointsDelay = 0;
            }
            ++this._ticks;
            break;
          case LevelCompleteHud.State.Banking:
            --this._bankPointsDelay;
            if (this._bankPointsDelay <= 0)
            {
              if (this.BankPoints())
              {
                this._bankPointsDelay = 1;
              }
              else
              {
                this._state = LevelCompleteHud.State.PostBankWait;
                this._bankPointsDelay = 120;
                this._shineTicks = 0;
              }
            }
            ++this._ticks;
            break;
          case LevelCompleteHud.State.PostBankWait:
            --this._bankPointsDelay;
            ++this._shineTicks;
            if (this._bankPointsDelay <= 0)
            {
              this._state = LevelCompleteHud.State.FlyOut;
              this._flyTicks = 0;
              this.SetFlyOutEasings();
            }
            ++this._ticks;
            break;
          case LevelCompleteHud.State.FlyOut:
            ++this._flyTicks;
            if (this._flyTicks >= 75)
            {
              this._level.SoundManager.StopAll();
              this._state = LevelCompleteHud.State.Finished;
            }
            ++this._ticks;
            break;
        }
      }

      public void Animate()
      {
      }

      private void SetFlyInEasings()
      {
        this._captionAFly = LevelCompleteHud.CaptionAFlyIn;
        this._captionBFly = LevelCompleteHud.CaptionBFlyIn;
        this._timeBonusFly = LevelCompleteHud.TimeBonusFlyIn;
        this._ringBonusFly = LevelCompleteHud.RingBonusFlyIn;
        this._perfectBonusFly = LevelCompleteHud.PerfectBonusFlyIn;
        this._totalFly = LevelCompleteHud.TotalFlyIn;
      }

      private void SetFlyOutEasings()
      {
        this._captionAFly = LevelCompleteHud.CaptionAFlyOut;
        this._captionBFly = LevelCompleteHud.CaptionBFlyOut;
        this._timeBonusFly = LevelCompleteHud.TimeBonusFlyOut;
        this._ringBonusFly = LevelCompleteHud.RingBonusFlyOut;
        this._perfectBonusFly = LevelCompleteHud.PerfectBonusFlyOut;
        this._totalFly = LevelCompleteHud.TotalFlyOut;
      }

      private bool BankPoints()
      {
        int num1 = Math.Min(this.TimeBonus, 100);
        int num2 = Math.Min(this.RingBonus, 100);
        int num3 = Math.Min(this.PerfectBonus, 100);
        int num4 = num1 + num2 + num3;
        if (num4 == 0)
        {
          this._level.SoundManager.PlaySound(this._tallyEndSample);
          return false;
        }
        this.TimeBonus -= num1;
        this.RingBonus -= num2;
        this.PerfectBonus -= num3;
        this._total += num4;
        this._level.Player.Score += num4;
        this._tallySwitchDelay = (this._tallySwitchDelay + 1) % 2;
        if (this._tallySwitchDelay == 0)
          this._level.SoundManager.PlaySound(this._tallySwitchSample);
        return true;
      }

      public void Draw(Renderer renderer)
      {
        if (this._state == LevelCompleteHud.State.PreDelay)
          return;
        this.DrawCaption(renderer);
        this.DrawScoreLabels(renderer);
      }

      private void DrawCaption(Renderer renderer)
      {
        double valueAt1 = this._captionAFly.GetValueAt(this._flyTicks);
        double valueAt2 = this._captionBFly.GetValueAt(this._flyTicks);
        string str = "SONIC";
        switch (this._level.Player.ProtagonistCharacterType)
        {
          case CharacterType.Tails:
            str = "TAILS";
            break;
          case CharacterType.Knuckles:
            str = "KNUCKLES";
            break;
        }
        IFontRenderer fontRenderer = renderer.GetFontRenderer();
        fontRenderer.RenderStringWithShadow(str + " GOT", new Rectangle(valueAt1, 270.0, 1920.0, 0.0), FontAlignment.MiddleX, this._captionFont, 0);
        Rectangle rectangle = this._captionFont.MeasureString("THROUGH ACT", new Rectangle(), FontAlignment.Left);
        double num1 = rectangle.Width + (double) (this._captionFont.DefaultWidth / 2);
        double num2 = num1;
        Font captionActFont = this._captionActFont;
        string text = this._level.CurrentAct.ToString();
        rectangle = new Rectangle();
        Rectangle boundary = rectangle;
        rectangle = captionActFont.MeasureString(text, boundary, FontAlignment.Left);
        double width = rectangle.Width;
        double num3 = num2 + width;
        double x = valueAt2 + (1920.0 - num3) / 2.0;
        fontRenderer.RenderStringWithShadow("THROUGH ACT", new Rectangle(x, (double) (270 + this._captionFont.Height + 16 /*0x10*/), 1920.0, 0.0), FontAlignment.Left, this._captionFont, 0);
        fontRenderer.RenderStringWithShadow(this._level.CurrentAct.ToString(), new Rectangle(x + num1, (double) (270 + this._captionFont.Height + 16 /*0x10*/), 0.0, 0.0), FontAlignment.Left, this._captionActFont, 0);
      }

      private void DrawScoreLabels(Renderer renderer)
      {
        this.DrawScoreLabel(renderer, "TIME BONUS", this.TimeBonus, this._timeBonusFly.GetValueAt(this._flyTicks), 604.0);
        this.DrawScoreLabel(renderer, "RING BONUS", this.RingBonus, this._ringBonusFly.GetValueAt(this._flyTicks), 668.0);
        if (this._achievedPerfect)
          this.DrawScoreLabel(renderer, "PERFECT BONUS", this.PerfectBonus, this._perfectBonusFly.GetValueAt(this._flyTicks), 732.0);
        byte highlight = (byte) MathX.Clamp(0.0, LevelCompleteHud.TotalShine.GetValueAt(this._shineTicks), (double) byte.MaxValue);
        this.DrawScoreLabel(renderer, "TOTAL", this._total, this._totalFly.GetValueAt(this._flyTicks), 796.0, highlight);
      }

      private void DrawScoreLabel(
        Renderer renderer,
        string caption,
        int value,
        double offsetX,
        double y,
        byte highlight = 0)
      {
        this.DrawCaption(renderer, caption, new Rectangle(480.0 + offsetX, y, 480.0, 0.0), FontAlignment.MiddleX);
        IFontRenderer fontRenderer = renderer.GetFontRenderer();
        FontAlignment fontAlignment = FontAlignment.Right;
        Rectangle boundary = new Rectangle(1380.0 + offsetX, y, 0.0, 0.0);
        if (highlight > (byte) 0)
          fontRenderer.RenderStringWithShadow(value.ToString(), boundary, fontAlignment, this._statisticsFont, Colours.White, new int?(), this._statisticsFont.DefaultShadow, new Colour(highlight, highlight, highlight));
        else
          fontRenderer.RenderStringWithShadow(value.ToString(), boundary, fontAlignment, this._statisticsFont, 0);
      }

      private void DrawCaption(
        Renderer renderer,
        string text,
        Rectangle boundary,
        FontAlignment textAlignment)
      {
        Rectangle rectangle = this._statisticsFont.MeasureString(text, boundary, textAlignment);
        ITexture texture = this._triangleTextureSonic;
        switch (this._level.Player.ProtagonistCharacterType)
        {
          case CharacterType.Tails:
            texture = this._triangleTextureTails;
            break;
          case CharacterType.Knuckles:
            texture = this._triangleTextureKnuckles;
            break;
        }
        renderer.Get2dRenderer().RenderTexture(texture, new Vector2(rectangle.Right - 8.0, rectangle.Bottom - 7.0));
        renderer.GetFontRenderer().RenderStringWithShadow(text, boundary, textAlignment, this._statisticsFont, 1);
      }

      private enum State
      {
        None,
        PreDelay,
        FlyIn,
        Banking,
        PostBankWait,
        FlyOut,
        Finished,
      }
    }
}
