// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelGameState
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Network;
using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core
{

    public class LevelGameState : IGameState, IDisposable
    {
      private readonly SonicOrcaGameContext _gameContext;
      private readonly LevelLoader _levelLoader;
      private Area _area;
      private Level _level;
      private Player _player;
      private bool _draw;
      private bool _finished;
      private bool _areaPaused;
      private int _lastNotificationTickCount;
      private Vector2 _inputDirection;
      private bool _inputAction;

      public LevelPrepareSettings PrepareSettings { get; set; }

      public bool Completed { get; set; }

      public Player Player => this._player;

      public Level Level => this._level;

      public LevelGameState(SonicOrcaGameContext gameContext)
      {
        this._gameContext = gameContext;
        this._levelLoader = new LevelLoader(gameContext);
      }

      public void Dispose()
      {
        if (this._levelLoader.HasLoadedLevel)
          this._levelLoader.UnloadLevel();
        if (!this._levelLoader.HasLoadedArea)
          return;
        this._levelLoader.UnloadArea();
      }

      public IEnumerable<UpdateResult> Update()
      {
        if (this.PrepareSettings == null)
          throw new InvalidOperationException("Prepare settings was not specified.");
        this._levelLoader.LoadArea(this.PrepareSettings);
        while (!this._levelLoader.HasLoadedArea)
          yield return UpdateResult.Next();
        this._area = this._levelLoader.Area;
        this._level = this._levelLoader.Level;
        this._player = this._level.Player;
        this._player.Lives = this.PrepareSettings.Lives;
        this._player.Score = this.PrepareSettings.Score;
        this._levelLoader.LoadLevel();
        do
        {
          while (!this._levelLoader.HasLoadedLevel)
            yield return UpdateResult.Next();
          switch (this._level.State)
          {
            case LevelState.Null:
              if (!this.IsWaitingForNetworkPlayers())
              {
                this.StartLevel();
                break;
              }
              break;
            case LevelState.Playing:
              this.OnLevelUpdate();
              break;
            case LevelState.Dead:
              this.OnPlayerDeath();
              break;
            case LevelState.StageCompleted:
              this.OnStageComplete();
              break;
            case LevelState.Restart:
            case LevelState.RestartCheckpoint:
              this.OnPlayerRestart();
              break;
            case LevelState.Quit:
              this._finished = true;
              break;
          }
          yield return UpdateResult.Next();
        }
        while (!this._finished);
      }

      public void HandleInput()
      {
        if (this._level == null || this._level.State != LevelState.Playing)
          return;
        this._level.HandleInput();
      }

      public void Draw()
      {
        if (!this._draw)
          return;
        this._level.Draw(this._gameContext.Renderer);
      }

      private void StartLevel()
      {
        this._level.SeamlessAct = this._level.CurrentAct == 1 && !this.PrepareSettings.TimeTrial;
        if (this.PrepareSettings.TimeTrial)
          this._level.Player.Lives = -1;
        this._level.Start();
        this._draw = true;
      }

      private void OnLevelUpdate()
      {
        this.NetworkPlayUpdate();
        if (this._level.StateFlags.HasFlag((Enum) LevelStateFlags.Paused))
        {
          if (!this._areaPaused)
          {
            this._areaPaused = true;
            this._area.OnPause();
          }
        }
        else
        {
          if (this._areaPaused)
          {
            this._areaPaused = false;
            this._area.OnUnpause();
          }
          this._area.OnUpdate();
        }
        this._level.Update();
        this._level.Animate();
      }

      private void OnPlayerDeath()
      {
        if (this._level.PlayRecorder != null)
        {
          if (this._level.PlayRecorder.Recording)
            this.PrepareSettings.RecordedPlayWritePath = (string) null;
          if (this._level.PlayRecorder.Playing)
          {
            if (!string.IsNullOrEmpty(this.PrepareSettings.RecordedPlayReadPath))
            {
              this._finished = true;
              return;
            }
            if (!this.PrepareSettings.TimeTrial && this._level.Player.StarpostIndex != -1)
              this.PrepareSettings.RecordedPlayGhostReadPath = (string) null;
          }
        }
        this._draw = false;
        this._level.State = LevelState.Null;
        this._levelLoader.UnloadLevel();
        this.PrepareSettings.Seamless = false;
        if (this.PrepareSettings.TimeTrial)
        {
          this._level.Time = 0;
          this._level.RingsCollected = 0;
          this._level.Player.StarpostIndex = -1;
        }
        else if (this._level.Player.Lives == 0)
        {
          this._finished = true;
          return;
        }
        this._levelLoader.LoadLevel();
      }

      private void OnPlayerRestart()
      {
        bool flag = this._level.State == LevelState.RestartCheckpoint;
        if (this._level.PlayRecorder != null)
        {
          if (this._level.PlayRecorder.Recording)
            this.PrepareSettings.RecordedPlayWritePath = (string) null;
          if (this._level.PlayRecorder.Playing)
          {
            if (!string.IsNullOrEmpty(this.PrepareSettings.RecordedPlayReadPath))
            {
              this._finished = true;
              return;
            }
            if (!this.PrepareSettings.TimeTrial && this._level.Player.StarpostIndex != -1)
              this.PrepareSettings.RecordedPlayGhostReadPath = (string) null;
          }
        }
        this._draw = false;
        this._level.State = LevelState.Null;
        this._levelLoader.UnloadLevel();
        this.PrepareSettings.Seamless = false;
        if (this.PrepareSettings.TimeTrial || !flag)
        {
          this._level.Time = 0;
          this._level.RingsCollected = 0;
          this._level.Player.StarpostIndex = -1;
        }
        else if (this._level.Player.Lives == 0)
        {
          this._finished = true;
          return;
        }
        this._levelLoader.LoadLevel();
      }

      private void OnStageComplete()
      {
        this._level.State = LevelState.Null;
        if (this.PrepareSettings.TimeTrial)
        {
          this._draw = false;
          this.Completed = true;
          this._finished = true;
        }
        else if (this._level.CurrentAct == 1)
        {
          this._level.Time = 0;
          this._level.RingsCollected = 0;
          this._level.Player.StarpostIndex = -1;
          ++this.PrepareSettings.Act;
          this.PrepareSettings.Seamless = true;
          this._levelLoader.LoadLevel();
        }
        else
        {
          this._draw = false;
          this._finished = true;
          this.Completed = true;
        }
      }

      private bool IsWaitingForNetworkPlayers()
      {
        bool flag = true;
        NetworkManager networkManager = this._gameContext.NetworkManager;
        if (networkManager.NetworkPlay)
        {
          if (networkManager.Hosting)
          {
            networkManager.Server.Level = this._level;
            if (((IEnumerable<NetworkPlayer>) networkManager.Server.NetworkPlayers).All<NetworkPlayer>((Func<NetworkPlayer, bool>) (x => x.Ready)))
            {
              networkManager.Server.SendPacketToAllClients((Packet) new NotifyPacket(PacketType.ReadyToStartLevel));
              flag = false;
            }
          }
          else
          {
            networkManager.Client.Level = this._level;
            if (networkManager.Client.Ready)
              flag = false;
            else if (Environment.TickCount > this._lastNotificationTickCount + 3000)
            {
              networkManager.Client.SendPacket((Packet) new NotifyPacket(PacketType.ReadyToStartLevel));
              this._lastNotificationTickCount = Environment.TickCount;
            }
          }
        }
        else
          flag = false;
        return flag;
      }

      private void NetworkPlayUpdate()
      {
        NetworkManager networkManager = this._gameContext.NetworkManager;
        if (!networkManager.Hosting)
          return;
        int characterId = 1;
        if (this._inputDirection == this._gameContext.Current[0].DirectionLeft && this._inputAction == this._gameContext.Current[0].Action1)
          return;
        this._inputDirection = this._gameContext.Current[0].DirectionLeft;
        this._inputAction = this._gameContext.Current[0].Action1;
        PlayInputPacket playInputPacket = new PlayInputPacket(characterId, this._inputDirection, this._inputAction);
        networkManager.Server.SendPacketToAllClients((Packet) playInputPacket);
      }
    }
}
