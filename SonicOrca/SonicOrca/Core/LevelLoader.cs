// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelLoader
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    public class LevelLoader
    {
      private readonly SonicOrcaGameContext _gameContext;
      private Task _loadTask;
      private ResourceSession _resourceSession;
      private Area _area;
      private readonly Level _level;
      private bool _loadingArea;
      private bool _loadingLevel;
      private bool _levelLoaded;

      public Area Area => this._area;

      public Level Level => this._level;

      public LevelLoader(SonicOrcaGameContext gameContext)
      {
        this._gameContext = gameContext;
        this._resourceSession = new ResourceSession(gameContext.ResourceTree);
        this._level = new Level(gameContext);
      }

      public void LoadArea(LevelPrepareSettings prepareSettings)
      {
        if (this._area != null)
          throw new InvalidOperationException("Area is already loaded.");
        if (this._loadingArea)
          throw new InvalidOperationException("Area is already loading.");
        this._loadTask = this.LoadAreaAsync(prepareSettings);
        this._loadingArea = true;
      }

      public void LoadLevel()
      {
        if (this._area == null)
          throw new InvalidOperationException("Area is not loaded.");
        if (this._loadingArea)
          throw new InvalidOperationException("Area is currently loading.");
        if (this._loadingLevel)
          throw new InvalidOperationException("Level is already loading.");
        this._loadTask = this.LoadLevelAsync();
        this._loadingLevel = true;
      }

      public void UnloadLevel()
      {
        if (!this._levelLoaded)
          throw new InvalidOperationException("Level is not loaded.");
        if (this._loadingArea || this._loadingLevel)
          throw new InvalidOperationException("Can't unload while an area or level is being loaded.");
        this._level.Stop();
        this._level.Unload();
        this._levelLoaded = false;
      }

      public void UnloadArea()
      {
        if (this._area == null)
          throw new InvalidOperationException("Area is not loaded.");
        if (this._levelLoaded)
          throw new InvalidOperationException("Can't unload area until level is unloaded.");
        if (this._loadingArea || this._loadingLevel)
          throw new InvalidOperationException("Can't unload while an area or level is being loaded.");
        this._area.Dispose();
        this._area = (Area) null;
        this._resourceSession.Unload();
      }

      private async Task LoadAreaAsync(LevelPrepareSettings prepareSettings, CancellationToken ct = default (CancellationToken))
      {
        this._resourceSession.PushDependency(prepareSettings.AreaResourceKey);
        await this._resourceSession.LoadAsync(ct);
        this._area = this._gameContext.ResourceTree.GetLoadedResource<Area>(prepareSettings.AreaResourceKey);
        this._level.PrepareSettings = prepareSettings;
        await this._level.LoadCommonAsync(ct);
      }

      private async Task LoadLevelAsync(CancellationToken ct = default (CancellationToken))
      {
        this._area.Prepare(this._level, this._level.PrepareSettings);
        await this._level.LoadAsync(this._area, ct);
        this._levelLoaded = true;
      }

      public bool HasLoadedArea
      {
        get
        {
          if (!this._loadingArea)
            return this._area != null;
          if (!this._loadTask.IsCompleted)
            return false;
          if (this._loadTask.IsFaulted)
            throw this._loadTask.Exception.InnerException;
          this._loadingArea = false;
          return false;
        }
      }

      public bool HasLoadedLevel
      {
        get
        {
          if (!this._loadingLevel)
            return this._levelLoaded;
          if (!this._loadTask.IsCompleted)
            return false;
          if (this._loadTask.IsFaulted)
            throw this._loadTask.Exception.InnerException;
          this._loadingLevel = false;
          return false;
        }
      }
    }
}
