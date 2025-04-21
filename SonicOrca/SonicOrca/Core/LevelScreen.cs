// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelScreen
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Network;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Menu;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    public class LevelScreen : Screen
    {
      public const int DefaultPort = 7237;
      private const string FontResourceKey = "SONICORCA/FONTS/HUD";
      private readonly SonicOrcaGameContext _gameContext;
      private readonly ResourceSession _resourceSession;
      private Player _player;
      private Area _area;
      private Level _level;
      private LevelScreen.LevelScreenState _state;
      private LevelScreen.LevelScreenState _nextState;
      private int _currentAct = 1;
      private bool _seamless;
      private string _areaResourceKey = "SONICORCA/S2/LEVELS/EHZ/AREA";
      private Font _font;
      private Task _loadingAreaTask;
      private Task _preparingLevelTask;
      private bool _networkPlay;
      private bool _hosting;
      private NetworkGameServer _server;
      private NetworkGameClient _client;
      private Task _joiningServer;
      private int _lastNotificationTickCount;
      private Vector2 _inputDirection;
      private bool _inputAction;

      public Vector2i? OverrideStartPosition { get; set; }

      public Area Area => this._area;

      public Player Player => this._player;

      public Level Level => this._level;

      public LevelScreen(SonicOrcaGameContext gameContext)
      {
        this._gameContext = gameContext;
        this._resourceSession = new ResourceSession(gameContext.ResourceTree);
        this._level = new Level(this._gameContext);
        this._currentAct = int.Parse(this._gameContext.Configuration.GetProperty("debug", "startact", "1"));
        string property = this._gameContext.Configuration.GetProperty("debug", "startpos");
        if (property != null)
        {
          string[] strArray = property.Split(',');
          this.OverrideStartPosition = new Vector2i?(new Vector2i(int.Parse(strArray[0]), int.Parse(strArray[1])));
        }
        int num = int.Parse(this._gameContext.Configuration.GetProperty("debug", "zone", "1"));
        if (num == 2)
          this._areaResourceKey = "SONICORCA/S2/LEVELS/CPZ/AREA";
        if (num == 3)
        {
          this._areaResourceKey = "SONICORCA/S2/LEVELS/ARZ/AREA";
        }
        else
        {
          if (num != 5)
            return;
          this._areaResourceKey = "SONICORCA/S2/LEVELS/HTZ/AREA";
        }
      }

      public LevelScreen(SonicOrcaGameContext gameContext, string areaResourceKey, int act)
      {
        this._gameContext = gameContext;
        this._resourceSession = new ResourceSession(gameContext.ResourceTree);
        this._level = new Level(this._gameContext);
        this._areaResourceKey = areaResourceKey;
        this._currentAct = act;
      }

      public override async Task LoadAsync(ScreenLoadingProgress progress, CancellationToken ct = default (CancellationToken))
      {
        this._resourceSession.PushDependency("SONICORCA/FONTS/HUD");
        await this._resourceSession.LoadAsync(ct);
        this._font = this._gameContext.ResourceTree.GetLoadedResource<Font>("SONICORCA/FONTS/HUD");
      }

      public override void Unload()
      {
        if (this._level != null)
          this._level.Dispose();
        this._resourceSession.Unload();
      }

      public override void Update()
      {
        switch (this._state)
        {
          case LevelScreen.LevelScreenState.Initialise:
            if (this.NetworkSetupCheck())
            {
              this._state = LevelScreen.LevelScreenState.EstablishingNetworkPlay;
              break;
            }
            this._state = LevelScreen.LevelScreenState.Loading;
            break;
          case LevelScreen.LevelScreenState.Loading:
            this.LoadingUpdate();
            break;
          case LevelScreen.LevelScreenState.ReadyToQuit:
            this.UnloadLevel();
            this.Finish();
            break;
          case LevelScreen.LevelScreenState.ReadyToPlay:
          case LevelScreen.LevelScreenState.Playing:
            this.PlayingUpdate();
            break;
          case LevelScreen.LevelScreenState.EstablishingNetworkPlay:
            this.EstablishNetworkUpdate();
            break;
          case LevelScreen.LevelScreenState.NetworkWaitingForStart:
            this.NetworkWaitingUpdate();
            break;
          case LevelScreen.LevelScreenState.FadingOut:
            this.PlayingUpdate();
            if (this._level.StateFlags.HasFlag((Enum) LevelStateFlags.FadingOut))
              break;
            this._state = this._nextState;
            break;
          case LevelScreen.LevelScreenState.Dead:
            if (this._level.StateFlags.HasFlag((Enum) LevelStateFlags.TimeUp))
              this._player.StarpostTime = 0;
            this._level.Time = this._player.StarpostTime;
            this.UnloadLevel();
            --this._player.Lives;
            if (this._player.Lives <= 0)
            {
              this.Finish();
              break;
            }
            this._state = LevelScreen.LevelScreenState.Loading;
            break;
        }
      }

      public override void Draw(Renderer renderer)
      {
        switch (this._state)
        {
          case LevelScreen.LevelScreenState.Loading:
            if (this._seamless)
            {
              this.PlayingDraw(renderer);
              break;
            }
            this.LoadingDraw(renderer);
            break;
          case LevelScreen.LevelScreenState.ReadyToPlay:
            if (!this._seamless)
              break;
            this.PlayingDraw(renderer);
            break;
          case LevelScreen.LevelScreenState.Playing:
          case LevelScreen.LevelScreenState.FadingOut:
            this.PlayingDraw(renderer);
            break;
          case LevelScreen.LevelScreenState.EstablishingNetworkPlay:
            this.EstablishNetworkDraw(renderer);
            break;
          case LevelScreen.LevelScreenState.NetworkWaitingForStart:
            this.NetworkWaitingDraw(renderer);
            break;
        }
      }

      private async Task LoadArea(CancellationToken ct = default (CancellationToken))
      {
        this._resourceSession.PushDependency(this._areaResourceKey);
        await this._resourceSession.LoadAsync(ct);
        this._area = this._gameContext.ResourceTree.GetLoadedResource<Area>(this._areaResourceKey);
        this._player = this._level.Player;
        await this._level.LoadCommonAsync(ct);
      }

      private Task PrepareLevel(bool seamless)
      {
        Trace.WriteLine(string.Join(" ", "Preparing", seamless ? nameof (seamless) : string.Empty, "level"));
        this._area.Prepare(this._level, new LevelPrepareSettings()
        {
          Act = this._currentAct,
          Seamless = seamless
        });
        Vector2i? nullable = this.OverrideStartPosition;
        if (nullable.HasValue)
        {
          Level level = this._level;
          nullable = this.OverrideStartPosition;
          Vector2i vector2i = nullable.Value;
          level.StartPosition = vector2i;
          nullable = new Vector2i?();
          this.OverrideStartPosition = nullable;
        }
        return this._level.LoadAsync(this._area);
      }

      private void LoadingUpdate()
      {
        if (this._area == null)
        {
          if (this._loadingAreaTask != null)
            return;
          this._loadingAreaTask = this.LoadArea();
        }
        else
        {
          this._loadingAreaTask = (Task) null;
          if (this._preparingLevelTask == null)
          {
            this._preparingLevelTask = this.PrepareLevel(this._seamless);
          }
          else
          {
            if (!this._preparingLevelTask.IsCompleted)
              return;
            this._preparingLevelTask = (Task) null;
            this._state = this._networkPlay ? LevelScreen.LevelScreenState.NetworkWaitingForStart : LevelScreen.LevelScreenState.ReadyToPlay;
          }
        }
      }

      private void LoadingDraw(Renderer renderer)
      {
        Area area = this._area;
        renderer.GetFontRenderer();
      }

      private void UnloadLevel() => this._level.Unload();

      private void PlayingUpdate()
      {
        if (this._state == LevelScreen.LevelScreenState.ReadyToPlay)
        {
          if (!this._seamless)
          {
            this._level.Start();
          }
          else
          {
            this._seamless = false;
            this._level.Start();
          }
          this._state = LevelScreen.LevelScreenState.Playing;
        }
        if (this._state == LevelScreen.LevelScreenState.Playing)
        {
          switch (this._level.State)
          {
            case LevelState.Dead:
              this._level.FadeOut(LevelState.Dead);
              this._state = LevelScreen.LevelScreenState.FadingOut;
              this._nextState = LevelScreen.LevelScreenState.Dead;
              break;
            case LevelState.StageCompleted:
              if (this._level.CurrentAct == 1)
              {
                this._currentAct = 2;
                this._level.Time = 0;
                this._player.StarpostIndex = -1;
                this._seamless = true;
                this._state = LevelScreen.LevelScreenState.Loading;
                break;
              }
              this._level.FadeOut(LevelState.StageCompleted);
              this._state = LevelScreen.LevelScreenState.FadingOut;
              this._nextState = LevelScreen.LevelScreenState.ReadyToQuit;
              break;
          }
        }
        if (this._networkPlay)
          this.NetworkPlayUpdate();
        if (!this._level.StateFlags.HasFlag((Enum) LevelStateFlags.Editing))
          this._area.OnUpdate();
        this._level.Update();
        this._level.Animate();
      }

      private void PlayingDraw(Renderer renderer) => this._level.Draw(renderer);

      private void EstablishNetworkUpdate()
      {
        if (this._hosting)
        {
          if (((IReadOnlyCollection<NetworkPlayer>) this._server.NetworkPlayers).Count <= 0)
            return;
          this._server.AllowClientsToConnect = false;
          this._state = LevelScreen.LevelScreenState.Loading;
        }
        else
        {
          if (!this._joiningServer.IsCompleted)
            return;
          if (this._joiningServer.IsFaulted)
            throw this._joiningServer.Exception.InnerException;
          if (!this._client.Connected)
            return;
          this._state = LevelScreen.LevelScreenState.Loading;
        }
      }

      private void EstablishNetworkDraw(Renderer renderer)
      {
        string text = this._hosting ? "WAITING FOR CLIENT TO CONNECT..." : "CONNECTING TO SERVER...";
        renderer.GetFontRenderer().RenderStringWithShadow(text, new Rectangle(8.0, 8.0, 0.0, 0.0), FontAlignment.Left, this._font, 0);
      }

      private void NetworkWaitingUpdate()
      {
        if (this._hosting)
        {
          this._server.Update();
          if (!((IEnumerable<NetworkPlayer>) this._server.NetworkPlayers).All<NetworkPlayer>((Func<NetworkPlayer, bool>) (x => x.Ready)))
            return;
          this._server.SendPacketToAllClients((Packet) new NotifyPacket(PacketType.ReadyToStartLevel));
          this._state = LevelScreen.LevelScreenState.ReadyToPlay;
        }
        else
        {
          this._client.Update();
          if (this._client.Ready)
          {
            this._state = LevelScreen.LevelScreenState.ReadyToPlay;
          }
          else
          {
            if (Environment.TickCount <= this._lastNotificationTickCount + 3000)
              return;
            this._client.SendPacket((Packet) new NotifyPacket(PacketType.ReadyToStartLevel));
            this._lastNotificationTickCount = Environment.TickCount;
          }
        }
      }

      private void NetworkWaitingDraw(Renderer renderer)
      {
        renderer.GetFontRenderer().RenderStringWithShadow("WAITING FOR OTHER PLAYERS TO LOAD GAME...", new Rectangle(8.0, 8.0, 0.0, 0.0), FontAlignment.Left, this._font, 0);
      }

      private bool NetworkSetupCheck()
      {
        IReadOnlyList<string> commandLineArgs = (IReadOnlyList<string>) Environment.GetCommandLineArgs();
        if (((IReadOnlyCollection<string>) commandLineArgs).Count > 1)
        {
          switch (commandLineArgs[0].ToLower())
          {
            case "host":
              this._server = new NetworkGameServer(this._level, 7237);
              this._networkPlay = true;
              this._hosting = true;
              return true;
            case "join":
              this._client = new NetworkGameClient(this._level);
              this._joiningServer = this._client.InitiateHandshake(commandLineArgs[2], 7237);
              this._networkPlay = true;
              return true;
          }
        }
        return false;
      }

      private void NetworkPlayUpdate()
      {
        if (this._hosting)
        {
          this._server.Update();
          int characterId = 1;
          if (this._inputDirection == this._gameContext.Current[0].DirectionLeft && this._inputAction == this._gameContext.Current[0].Action1)
            return;
          this._inputDirection = this._gameContext.Current[0].DirectionLeft;
          this._inputAction = this._gameContext.Current[0].Action1;
          this._server.SendPacketToAllClients((Packet) new PlayInputPacket(characterId, this._inputDirection, this._inputAction));
        }
        else
          this._client.Update();
      }

      private enum LevelScreenState
      {
        Initialise,
        Loading,
        ReadyToQuit,
        ReadyToPlay,
        Playing,
        EstablishingNetworkPlay,
        NetworkWaitingForStart,
        FadingOut,
        Dead,
      }
    }
}
