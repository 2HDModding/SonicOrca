// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.GameOverHud
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Audio;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    public class GameOverHud : IDisposable
    {
      private const string FontResourceKey = "SONICORCA/FONTS/METALIC";
      private const string JingleResourceKey = "SONICORCA/MUSIC/JINGLE/GAMEOVER/S1";
      private const int TotalTime = 900;
      private const int FlyTime = 15;
      private static readonly EaseTimeline LeftFly = new EaseTimeline(new EaseTimeline.Entry[4]
      {
        new EaseTimeline.Entry(0, 0.0),
        new EaseTimeline.Entry(15, 960.0),
        new EaseTimeline.Entry(885, 960.0),
        new EaseTimeline.Entry(900, 0.0)
      });
      private static readonly EaseTimeline RightFly = new EaseTimeline(new EaseTimeline.Entry[4]
      {
        new EaseTimeline.Entry(0, 1920.0),
        new EaseTimeline.Entry(15, 960.0),
        new EaseTimeline.Entry(885, 960.0),
        new EaseTimeline.Entry(900, 1920.0)
      });
      private readonly Level _level;
      private readonly ResourceSession _resourceSession;
      private Font _font;
      private Sample _jingle;
      private GameOverHud.Reason _reason;
      private int _ticks;

      public bool Finished { get; private set; }

      public GameOverHud(Level level)
      {
        this._level = level;
        this._resourceSession = new ResourceSession(level.GameContext.ResourceTree);
      }

      public async Task LoadAsync(CancellationToken ct = default (CancellationToken))
      {
        ResourceTree resourceTree = this._resourceSession.ResourceTree;
        this._resourceSession.PushDependencies("SONICORCA/FONTS/METALIC", "SONICORCA/MUSIC/JINGLE/GAMEOVER/S1");
        await this._resourceSession.LoadAsync(ct);
        this._font = resourceTree.GetLoadedResource<Font>("SONICORCA/FONTS/METALIC");
        this._jingle = resourceTree.GetLoadedResource<Sample>("SONICORCA/MUSIC/JINGLE/GAMEOVER/S1");
      }

      public void Dispose() => this._resourceSession.Dispose();

      public void Start(GameOverHud.Reason reason)
      {
        this._ticks = 0;
        this._reason = reason;
        this.Finished = false;
        this._level.SoundManager.StopAll();
        this._level.SoundManager.PlayJingleOnce(this._jingle);
      }

      public void Update()
      {
        ++this._ticks;
        if (this._ticks < 900)
          return;
        this.Finished = true;
      }

      public void Draw(Renderer renderer)
      {
        IFontRenderer fontRenderer = renderer.GetFontRenderer();
        int num = this._font.DefaultWidth / 2;
        fontRenderer.RenderStringWithShadow(this._reason == GameOverHud.Reason.GameOver ? "GAME" : "TIME", new Rectangle(GameOverHud.LeftFly.GetValueAt(this._ticks) - (double) num, 0.0, 0.0, 1080.0), FontAlignment.Right | FontAlignment.MiddleY, this._font, Colours.White);
        fontRenderer.RenderStringWithShadow("OVER", new Rectangle(GameOverHud.RightFly.GetValueAt(this._ticks) + (double) num, 0.0, 0.0, 1080.0), FontAlignment.MiddleY, this._font, Colours.White);
      }

      public enum Reason
      {
        GameOver,
        TimeOver,
      }
    }
}
