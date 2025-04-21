// Decompiled with JetBrains decompiler
// Type: SonicOrca.SonicOrcaGameContext
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Audio;
using SonicOrca.Core;
using SonicOrca.Core.Network;
using SonicOrca.Graphics;
using SonicOrca.Input;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SonicOrca
{

    public abstract class SonicOrcaGameContext : IDisposable
    {
      private readonly IPlatform _platform;
      private readonly ResourceTree _resourceTree;
      private bool _catchup;
      private bool _dirty;
      private int _catchupTick;
      private int _lastUpdateTick;
      private int _lastFrameTick;
      private int _fpsMonitorLastTick;
      private int _fpsMonitorLastUps;
      private int _fpsMonitorLastFps;
      private int _fpsMonitorUpdateCount;
      private int _fpsMonitorDrawCount;
      private Renderer _renderer;
      private Stopwatch _stopwatch;
      private readonly NetworkManager _networkManager = new NetworkManager();
      protected IFramebuffer _canvas;
      private Controller[] _controllersCurrent = new Controller[4];
      private Controller[] _controllersPressed = new Controller[4];
      private Controller[] _controllersReleased = new Controller[4];
      private GamePadOutputState[] _controllersOutput = new GamePadOutputState[4];

      public static SonicOrcaGameContext Singleton { get; private set; }

      public static bool IsMaxPerformance { get; set; }

      public Renderer Renderer => this._renderer;

      public IPlatform Platform => this._platform;

      public AudioContext Audio => this._platform.Audio;

      public InputContext Input => this._platform.Input;

      public WindowContext Window => this._platform.Window;

      public ResourceTree ResourceTree => this._resourceTree;

      public int UpdateCount { get; private set; }

      public int DrawCount { get; private set; }

      public bool Finish { get; set; }

      public int TargetFrameRate { get; set; }

      public int LastUps => this._fpsMonitorLastUps;

      public int LastFps => this._fpsMonitorLastFps;

      public bool Catchup
      {
        get => this._catchup;
        set
        {
          this._catchup = value;
          this.ResetCatchup();
        }
      }

      public SonicOrcaConsole Console { get; private set; }

      public NetworkManager NetworkManager => this._networkManager;

      public bool ForceHD { get; set; }

      public IniConfiguration Configuration { get; protected set; }

      public IReadOnlyList<Controller> Current => (IReadOnlyList<Controller>) this._controllersCurrent;

      public IReadOnlyList<Controller> Pressed => (IReadOnlyList<Controller>) this._controllersPressed;

      public IReadOnlyList<Controller> Released
      {
        get => (IReadOnlyList<Controller>) this._controllersReleased;
      }

      public IList<GamePadOutputState> Output => (IList<GamePadOutputState>) this._controllersOutput;

      public string UserDataDirectory { get; protected set; }

      public IEnumerable<Controller> Controllers
      {
        get
        {
          return ((IEnumerable<Controller>) this._controllersCurrent).Concat<Controller>((IEnumerable<Controller>) this._controllersPressed).Concat<Controller>((IEnumerable<Controller>) this._controllersReleased);
        }
      }

      public abstract IAudioSettings AudioSettings { get; }

      public abstract IVideoSettings VideoSettings { get; }

      public SonicOrcaGameContext(IPlatform platform)
      {
        SonicOrcaGameContext.Singleton = this;
        this._platform = platform;
        this._resourceTree = new ResourceTree();
        this.TargetFrameRate = 60;
        this.ForceHD = true;
        for (int index = 0; index < 4; ++index)
        {
          this._controllersCurrent[index] = new Controller(this, index, InputStateEventType.Current);
          this._controllersPressed[index] = new Controller(this, index, InputStateEventType.Pressed);
          this._controllersReleased[index] = new Controller(this, index, InputStateEventType.Released);
        }
        this.Console = new SonicOrcaConsole(this);
      }

      public virtual void Initialise()
      {
        this.Window.HideCursorIfIdle = true;
        this._renderer = this.CreateRenderer();
      }

      public virtual void Dispose()
      {
      }

      public void Run()
      {
        this.ResetCatchup();
        this.Finish = false;
        while (!this.Finish)
          this.MainLoopIteration();
      }

      private void MainLoopIteration()
      {
        int tickCount = Environment.TickCount;
        if (this._stopwatch == null)
          this._stopwatch = new Stopwatch();
        this._stopwatch.Start();
        if (this._catchup)
          this.UpdateCatchup();
        else
          this.Update();
        if (this._dirty)
          this.Draw(this._renderer);
        this.MeasureFrameRate();
        if (!SonicOrcaGameContext.IsMaxPerformance)
        {
          while (this._stopwatch.ElapsedTicks <= Stopwatch.Frequency / 60L)
          {
            if (this._stopwatch.ElapsedTicks < Stopwatch.Frequency / 70L)
              Thread.Sleep(1);
          }
        }
        this._stopwatch.Reset();
      }

      private void MeasureFrameRate()
      {
        if (Environment.TickCount - this._fpsMonitorLastTick <= 1000)
          return;
        this._fpsMonitorLastUps = this.UpdateCount - this._fpsMonitorUpdateCount;
        this._fpsMonitorLastFps = this.DrawCount - this._fpsMonitorDrawCount;
        this._fpsMonitorLastTick = Environment.TickCount;
        this._fpsMonitorUpdateCount = this.UpdateCount;
        this._fpsMonitorDrawCount = this.DrawCount;
      }

      public void ResetCatchup() => this._catchupTick = Environment.TickCount;

      private void UpdateCatchup()
      {
        int num = (Environment.TickCount - this._catchupTick) * this.TargetFrameRate / 1000;
        if (num > this._lastFrameTick + this.TargetFrameRate * 2)
        {
          this._lastUpdateTick = 0;
          this._catchupTick = Environment.TickCount;
          this.Update();
        }
        else
        {
          for (; num > this._lastUpdateTick; ++this._lastUpdateTick)
            this.Update();
        }
      }

      public void Update()
      {
        this.Window.Update();
        if (this.Window.Finished)
          this.Finish = true;
        this.Audio.Update();
        this.Input.Update();
        this.OnUpdate();
        this.Input.UpdatePressedReleased();
        this.OnUpdateStep();
        this._dirty = true;
        ++this.UpdateCount;
      }

      protected virtual void OnUpdate()
      {
      }

      protected virtual void OnUpdateStep()
      {
      }

      public void Draw(Renderer renderer)
      {
        this.Window.BeginRender();
        this.OnDraw();
        renderer.DeativateRenderer();
        this.Window.EndRender();
        ++this.DrawCount;
      }

      protected virtual void OnDraw()
      {
      }

      protected abstract Renderer CreateRenderer();

      protected internal abstract ILevelRenderer CreateLevelRenderer(Level level);
    }
}
