// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Level
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Audio;
using SonicOrca.Core.Collision;
using SonicOrca.Core.Debugging;
using SonicOrca.Core.Lighting;
using SonicOrca.Core.Objects;
using SonicOrca.Core.Objects.Base;
using SonicOrca.Extensions;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Input;
using SonicOrca.Menu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    public class Level : IDisposable
    {
      private int TimeBeforeDeath = 36000;
      private readonly SonicOrcaGameContext _gameContext;
      private readonly Player _player;
      private readonly Random _random = new Random();
      private readonly FadeTransition _fadeTransition = new FadeTransition(60);
      private readonly ObjectManager _objectManager;
      private readonly CollisionTable _collisionTable;
      private readonly ParticleManager _particleManager;
      private readonly WaterManager _waterManager;
      private readonly SoundManager _soundManager;
      private readonly SonicOrca.Core.Lighting.LightingManager _lightingManager;
      private readonly Dictionary<string, LevelMarker> _markers = new Dictionary<string, LevelMarker>();
      private readonly DebugContext _debugContext;
      private readonly LevelHud _hud;
      private readonly LevelCompleteHud _completeHud;
      private readonly GameOverHud _gameOverHud;
      private ILevelTitleCard _titleCard;
      private readonly Camera _camera;
      private readonly LayerViewOptions _layerViewOptions = new LayerViewOptions();
      private readonly List<Tuple<int, Action>> _intervals = new List<Tuple<int, Action>>();
      private GhostCharacterInstance _ghostCharacter;
      private PlayRecorder _playRecorder;
      private Rectanglei _targetBounds;
      private int _boundsScrollSpeed;
      private bool _cameraLockedSeamless;
      private LevelState _fadeOutToState;
      private readonly CommonResources _commonResources;
      private readonly IFadeTransitionRenderer _fadeTransitionRenderer;
      private readonly ILevelRenderer _levelRenderer;
      private readonly IReadOnlyList<string> CharacterTypeResourceKeys = (IReadOnlyList<string>) new string[4]
      {
        null,
        "SONICORCA/OBJECTS/SONIC",
        "SONICORCA/OBJECTS/TAILS",
        "SONICORCA/OBJECTS/KNUCKLES"
      };
      private const double EarthquakeFadeAmount = 0.011111111111111112;
      private bool _earthquakeIsCurrentlyActive;
      private double _earthquakeFade;
      private SampleInstance _earthquakeSampleInstance;

      public SonicOrcaGameContext GameContext => this._gameContext;

      public Player Player => this._player;

      public Area Area { get; private set; }

      public LevelPrepareSettings PrepareSettings { get; set; }

      public string Name { get; set; }

      public bool ShowAsZone { get; set; }

      public bool ShowAsAct { get; set; }

      public int CurrentAct { get; set; }

      public LevelScheme Scheme { get; set; }

      public IReadOnlyCollection<string> AnimalResourceKeys { get; set; }

      public string LevelMusic { get; set; }

      public Vector2i StartPosition { get; set; }

      public int StartLayerIndex { get; set; }

      public Rectanglei Bounds { get; set; }

      public Rectanglei SeamlessNextBounds { get; set; }

      public bool SeamlessAct { get; set; }

      public double NightMode { get; set; }

      public bool FinishOnCompleteLevel { get; set; }

      public TileSet TileSet { get; set; }

      public LevelBinding Binding { get; set; }

      public LevelMap Map { get; set; }

      internal PlayRecorder PlayRecorder => this._playRecorder;

      public LevelState State { get; set; }

      public LevelStateFlags StateFlags { get; set; }

      public int Ticks { get; set; }

      public int Time { get; set; }

      public Camera Camera => this._camera;

      public bool IsScrollingBounds => this._boundsScrollSpeed != 0;

      public LayerViewOptions LayerViewOptions => this._layerViewOptions;

      public Random Random => this._random;

      public ObjectManager ObjectManager => this._objectManager;

      public CollisionTable CollisionTable => this._collisionTable;

      public ParticleManager ParticleManager => this._particleManager;

      public WaterManager WaterManager => this._waterManager;

      public SoundManager SoundManager => this._soundManager;

      public ILightingManager LightingManager => (ILightingManager) this._lightingManager;

      public DebugContext DebugContext => this._debugContext;

      public bool LandscapeAnimating { get; set; }

      public bool ObjectsAnimating { get; set; }

      public bool ShowCharacterInfo { get; set; }

      public bool ShowSidekickIntelligence { get; set; }

      public bool ClassicDebugMode { get; set; }

      public bool ShowHUD { get; set; }

      public int RingsCollected { get; set; }

      public int RingsPerfectTarget { get; set; }

      public CommonResources CommonResources => this._commonResources;

      public ILevelRenderer LevelRenderer => this._levelRenderer;

      public Level(SonicOrcaGameContext gameContext)
      {
        this._gameContext = gameContext;
        this._fadeTransitionRenderer = gameContext.Renderer.CreateFadeTransitionRenderer();
        this._levelRenderer = gameContext.CreateLevelRenderer(this);
        this._player = new Player(this);
        this._collisionTable = new CollisionTable(this);
        this._objectManager = new ObjectManager(this);
        this._particleManager = new ParticleManager(this);
        this._waterManager = new WaterManager(this);
        this._soundManager = new SoundManager(this);
        this._lightingManager = new SonicOrca.Core.Lighting.LightingManager();
        this._debugContext = new DebugContext(this);
        this._hud = new LevelHud(this);
        this._completeHud = new LevelCompleteHud(this);
        this._gameOverHud = new GameOverHud(this);
        this._camera = new Camera(this);
        this._commonResources = new CommonResources(this._gameContext);
        this._playRecorder = new PlayRecorder();
        this.TileSet = new TileSet();
        this.Map = new LevelMap();
        this.LandscapeAnimating = true;
        this.ObjectsAnimating = true;
        this.ShowHUD = true;
        this.AnimalResourceKeys = (IReadOnlyCollection<string>) new string[0];
      }

      public void Dispose()
      {
        if (this._earthquakeSampleInstance != null)
          this._earthquakeSampleInstance.Dispose();
        this.Unload();
        this._playRecorder.Dispose();
        this._commonResources.Dispose();
        this._particleManager.Dispose();
        this._gameOverHud.Dispose();
        this._completeHud.Dispose();
        this._hud.Dispose();
        this._debugContext.Dispose();
        this._fadeTransitionRenderer.Dispose();
      }

      public async Task LoadCommonAsync(CancellationToken ct = default (CancellationToken))
      {
        Trace.WriteLine("Loading common level components");
        await this._debugContext.LoadAsync(ct);
        await this._hud.LoadAsync(ct);
        await this._completeHud.LoadAsync(ct);
        await this._gameOverHud.LoadAsync(ct);
        await this._particleManager.LoadAsync(ct);
        await this._levelRenderer.LoadAsync(ct);
        await this._commonResources.LoadEntriesAsync(ct);
      }

      public async Task LoadAsync(Area area, CancellationToken ct = default (CancellationToken))
      {
        Level level = this;
        Trace.WriteLine("Loading level " + level.Name);
        level.Area = area;
        await level._commonResources.LoadSchemeAsync(level.Scheme, ct);
        level._titleCard = (ILevelTitleCard) new Sonic2LevelTitleCard(level);
        await level._titleCard.LoadAsync(level._gameContext.ResourceTree, ct);
        level._soundManager.SetJingle(JingleType.Life, level._commonResources.GetResourcePath("1upjingle"));
        level._soundManager.SetJingle(JingleType.Invincibility, level._commonResources.GetResourcePath("invincibilityjingle"));
        level._soundManager.SetJingle(JingleType.Drowning, level._commonResources.GetResourcePath("drowningjingle"));
      }

      public void LoadMap(LevelMap map)
      {
        Trace.WriteLine("Loading level map");
        this.Map = map;
        this.Map.Level = this;
        this._objectManager.Setup(this.Map);
        this._collisionTable.Initialise(this.Map);
        this._markers.Clear();
        foreach (LevelMarker marker in (IEnumerable<LevelMarker>) map.Markers)
        {
          if (!string.IsNullOrEmpty(marker.Name))
            this._markers[marker.Name.ToLower()] = marker;
        }
        this._levelRenderer.Initialise();
        Trace.WriteLine("Map loading complete");
      }

      public void LoadBinding(LevelBinding binding)
      {
        Trace.WriteLine("Loading level binding");
        this.Binding = binding;
        this.Binding.Level = this;
        this._objectManager.Bind(this.Binding);
        Trace.WriteLine("Map binding complete");
      }

      public void UnloadCommon()
      {
        Trace.WriteLine("Unloading common level components");
        this._debugContext.Dispose();
        this._hud.Dispose();
        this._completeHud.Dispose();
        this._gameOverHud.Dispose();
        this._particleManager.Dispose();
        this._levelRenderer.Dispose();
        this._commonResources.Dispose();
      }

      public void Unload()
      {
        Trace.WriteLine("Unloading level " + this.Name);
        this.Stop();
        if (this._titleCard == null)
          return;
        this._titleCard.Dispose();
      }

      public void Start()
      {
        Trace.WriteLine("Starting level " + this.Name);
        this._waterManager.Load();
        this._fadeTransition.Clear();
        this.StateFlags = (LevelStateFlags) 0;
        this.Ticks = 0;
        this._intervals.Clear();
        this.InitialiseEarthquake();
        this.FinishOnCompleteLevel = true;
        this.NightMode = this.PrepareSettings.NightMode;
        if (!this.PrepareSettings.Seamless)
          this._objectManager.Start();
        if (this.State == LevelState.Editing)
          return;
        this._player.ResetScoreChain();
        this._player.ResetRings();
        this._player.RemovePowerups();
        if (this.PrepareSettings.Seamless)
        {
          this._player.StarpostIndex = -1;
          this._player.Protagonist.IsWinning = false;
          if (this._player.Sidekick != null)
            this._player.Sidekick.IsWinning = false;
          this.StateFlags |= LevelStateFlags.AllowCharacterControl;
          this.StateFlags |= LevelStateFlags.UpdateTime;
          this._cameraLockedSeamless = true;
        }
        else
        {
          this.SetupCharacters();
          this._cameraLockedSeamless = false;
        }
        this.StartTitleCard(this.PrepareSettings.Seamless);
        this._soundManager.PlayMusic(this.LevelMusic);
        this.Time = this._player.StarpostIndex == -1 ? 0 : this._player.StarpostTime;
        this._hud.ShowMiliseconds = this.PrepareSettings.TimeTrial;
        this.ClassicDebugMode = this.PrepareSettings.Debugging;
        Controller.IsDebug = this.PrepareSettings.Debugging;
        if (!this.PrepareSettings.Seamless)
        {
          if (!string.IsNullOrEmpty(this.PrepareSettings.RecordedPlayReadPath))
            this._playRecorder.BeginPlaying(this.PrepareSettings.RecordedPlayReadPath);
          else if (this.PrepareSettings.RecordedPlayReadData != null)
            this._playRecorder.BeginPlaying((Stream) new MemoryStream(this.PrepareSettings.RecordedPlayReadData));
          else if (!string.IsNullOrEmpty(this.PrepareSettings.RecordedPlayGhostReadPath))
            this._playRecorder.BeginPlaying(this.PrepareSettings.RecordedPlayGhostReadPath);
          if (!string.IsNullOrEmpty(this.PrepareSettings.RecordedPlayWritePath))
            this._playRecorder.BeginRecording(this.PrepareSettings.RecordedPlayWritePath);
        }
        this.State = LevelState.Playing;
        this.RingsCollected = 0;
        this.Area.OnStart();
      }

      public void Stop()
      {
        Trace.WriteLine("Stopping level " + this.Name);
        if (this._playRecorder != null)
        {
          this._playRecorder.EndPlaying();
          this._playRecorder.EndRecording();
        }
        this.StopEarthquake();
        this._soundManager.StopAll();
        this._objectManager.Stop();
      }

      private void SetupCharacters()
      {
        Vector2i playerStartPosition = this.GetPlayerStartPosition();
        Vector2i position = playerStartPosition + new Vector2i(-64, 0);
        CharacterType characterType1 = this.PrepareSettings.ProtagonistCharacter;
        CharacterType characterType2 = this.PrepareSettings.SidekickCharacter;
        if (characterType1 == CharacterType.Null)
        {
          characterType1 = CharacterType.Sonic;
          characterType2 = CharacterType.Tails;
        }
        string characterTypeResourceKey1 = this.CharacterTypeResourceKeys[(int) characterType1];
        string characterTypeResourceKey2 = this.CharacterTypeResourceKeys[(int) characterType2];
        ICharacter character1 = characterType2 != CharacterType.Null ? this.CreateCharacter(characterTypeResourceKey2, position) : (ICharacter) null;
        ICharacter character2 = this.CreateCharacter(characterTypeResourceKey1, playerStartPosition);
        this.SetupCharacters(character2, character1);
        this.Player.ProtagonistCharacterType = characterType1;
        this.Player.SidekickCharacterType = characterType2;
        if (string.IsNullOrEmpty(this.PrepareSettings.RecordedPlayGhostReadPath))
          return;
        this._ghostCharacter = this.CreateGhostCharacter(character2, playerStartPosition);
      }

      private void SetupCharacters(ICharacter protagonist, ICharacter sidekick)
      {
        this._camera.ObjectToTrack = (ActiveObject) protagonist;
        this._camera.CentreObjectToTrack();
        protagonist.Player = this.Player;
        protagonist.Path = this.PrepareSettings.StartPath;
        this.Player.Protagonist = protagonist;
        this.Player.ProtagonistGamepadIndex = 0;
        if (sidekick == null)
          return;
        sidekick.Player = this.Player;
        sidekick.IsSidekick = true;
        sidekick.Path = this.PrepareSettings.StartPath;
        this.Player.Sidekick = sidekick;
        this.Player.SidekickGamepadIndex = 1;
      }

      private ICharacter CreateCharacter(string resourceKey, Vector2i position)
      {
        ObjectEntry objectEntry = new ObjectEntry(this, new ObjectPlacement(resourceKey, this.Map.Layers.IndexOf(this.Map.Layers[this.StartLayerIndex]), position));
        this._objectManager.AddObjectEntry(objectEntry);
        ActiveObject character = this._objectManager.ActivateObjectEntry(objectEntry);
        character.LockLifetime = true;
        return (ICharacter) character;
      }

      private GhostCharacterInstance CreateGhostCharacter(ICharacter character, Vector2i position)
      {
        GhostCharacterInstance ghostCharacter = (GhostCharacterInstance) this._objectManager.ActivateObjectEntry(new ObjectEntry(this, (ObjectType) new GhostCharacterType(), this.Map.Layers[this.StartLayerIndex], position));
        ghostCharacter.LockLifetime = true;
        ghostCharacter.Initialise(character.Animation.AnimationGroup);
        return ghostCharacter;
      }

      public Vector2i GetPlayerStartPosition()
      {
        Vector2i playerStartPosition = this.PrepareSettings.StartPosition ?? this.StartPosition;
        if (this._player.StarpostIndex != -1)
          playerStartPosition = this._player.StarpostPosition;
        return playerStartPosition;
      }

      public void Update()
      {
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.Paused))
          return;
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.TitleCardActive))
        {
          this._titleCard.Update();
          if (this._titleCard.AllowLevelToStart)
          {
            this.StateFlags |= LevelStateFlags.Updating;
            this.StateFlags |= LevelStateFlags.UpdateTime;
            this.StateFlags |= LevelStateFlags.Animating;
          }
          if (this._titleCard.AllowCharacterControl)
            this.StateFlags |= LevelStateFlags.AllowCharacterControl;
          if (this._titleCard.Finished)
            this.StateFlags &= ~LevelStateFlags.TitleCardActive;
          if (this._titleCard.Seamless && this._titleCard.UnlockCamera && this._cameraLockedSeamless)
          {
            this._cameraLockedSeamless = false;
            this.Bounds = this.SeamlessNextBounds;
            this.Camera.MaxVelocity = new Vector2(0.0, 0.0);
          }
        }
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.CompletingStage))
        {
          if (this._player.Sidekick != null && !this._player.Sidekick.IsAirborne)
            this._player.Sidekick.IsWinning = true;
          this._completeHud.Update();
          if (this._completeHud.Finished)
          {
            this.StateFlags &= ~LevelStateFlags.CompletingStage;
            this.StateFlags |= LevelStateFlags.StageCompleted;
            if (this.FinishOnCompleteLevel || this.PrepareSettings.TimeTrial)
            {
              if (!this.SeamlessAct)
                this.FadeOut(LevelState.StageCompleted);
              else
                this.State = LevelState.StageCompleted;
            }
            else
            {
              this._player.Protagonist.IsWinning = false;
              if (this._player.Sidekick != null)
                this._player.Sidekick.IsWinning = false;
            }
          }
        }
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.TimeOver) || this.StateFlags.HasFlag((Enum) LevelStateFlags.GameOver))
        {
          this._gameOverHud.Update();
          if (this._gameOverHud.Finished)
          {
            this.StateFlags &= ~(LevelStateFlags.TimeOver | LevelStateFlags.GameOver);
            this.StateFlags |= LevelStateFlags.Dead;
            this.FadeOut(LevelState.Dead);
          }
        }
        PlayRecorder.Entry entry = (PlayRecorder.Entry) null;
        if (this._playRecorder.Playing)
          entry = this._playRecorder.GetNextEntry();
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.Updating))
        {
          if (entry != null)
          {
            if (string.IsNullOrEmpty(this.PrepareSettings.RecordedPlayGhostReadPath))
            {
              CharacterInputState input = this._player.Protagonist.Input;
              input.Throttle = entry.Direction.X;
              input.VerticalDirection = Math.Sign(entry.Direction.Y);
              input.A = !entry.Action ? (input.A == CharacterInputButtonState.Down || input.A == CharacterInputButtonState.Pressed ? CharacterInputButtonState.Released : CharacterInputButtonState.Up) : (input.A == CharacterInputButtonState.Up || input.A == CharacterInputButtonState.Released ? CharacterInputButtonState.Pressed : CharacterInputButtonState.Down);
            }
            else
            {
              this._ghostCharacter.Set(entry);
              this.ReadPlayerInput();
            }
          }
          else
            this.ReadPlayerInput();
          this.SetObjectLifetimeArea();
          this._objectManager.Update();
          this._particleManager.Update();
          this._waterManager.Update();
          this.UpdateScrollBounds();
          this._camera.Update();
          this.UpdateInverals();
          this.Player.Update();
          if (this.Player.Protagonist.IsDead)
          {
            if (this._player.Lives > 0)
              --this._player.Lives;
            this.StateFlags &= ~LevelStateFlags.Animating;
            this.StateFlags &= ~LevelStateFlags.Updating;
            this.StateFlags &= ~LevelStateFlags.UpdateTime;
            this.StateFlags |= LevelStateFlags.Dead;
            if (this.StateFlags.HasFlag((Enum) LevelStateFlags.TimeUp))
              this.StartTimeOverSequence();
            else if (this.Player.Lives == 0)
              this.StartGameOverSequence();
            else
              this.FadeOut(LevelState.Dead);
          }
          if (this.StateFlags.HasFlag((Enum) LevelStateFlags.WaitingForCharacterToWin))
            this.CompleteLevel();
          this.UpdateEarthquake();
        }
        else if (this._earthquakeSampleInstance != null)
          this._earthquakeSampleInstance.Stop();
        this._soundManager.Update();
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.UpdateTime))
        {
          if (this.Time >= this.TimeBeforeDeath - 1)
          {
            this._player.StarpostTime = 0;
            this.StateFlags &= ~LevelStateFlags.UpdateTime;
            this.StateFlags |= LevelStateFlags.TimeUp;
            this.Player.Protagonist.Kill(PlayerDeathCause.TimeOver);
          }
          else
            ++this.Time;
        }
        this._debugContext.Update();
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.FadingOut))
        {
          this._fadeTransition.Update();
          if (this._fadeTransition.Finished)
          {
            this.StateFlags &= ~LevelStateFlags.FadingOut;
            this.State = this._fadeOutToState;
          }
        }
        if (this._playRecorder.Recording)
        {
          CharacterInputState input = this._player.Protagonist.Input;
          this._playRecorder.WriteEntry(new PlayRecorder.Entry()
          {
            Direction = new Vector2(input.Throttle, (double) input.VerticalDirection),
            Action = input.ABC.HasFlag((Enum) CharacterInputButtonState.Down),
            Position = this._player.Protagonist.Position,
            LayerIndex = this.Map.Layers.IndexOf(this._player.Protagonist.Layer),
            State = (int) this._player.Protagonist.StateFlags,
            AnimationIndex = this._player.Protagonist.Animation.Index,
            AnimationFrameIndex = this._player.Protagonist.Animation.CurrentFrameIndex,
            Angle = (float) this._player.Protagonist.ShowAngle
          });
        }
        ++this.Ticks;
      }

      public void HandleInput()
      {
        if (this._gameContext.Console.IsOpen || !this._gameContext.Pressed[0].Start)
          return;
        this.TogglePause();
      }

      public void Animate()
      {
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.Paused) || !this.StateFlags.HasFlag((Enum) LevelStateFlags.Animating))
          return;
        if (this.LandscapeAnimating)
          this.TileSet.Animate();
        this.Map.Animate();
        if (this.ObjectsAnimating)
          this._objectManager.Animate();
        this._waterManager.Animate();
        this._hud.Animate();
      }

      public void TogglePause()
      {
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.Paused))
          this.Unpause();
        else
          this.Pause();
      }

      public void Pause()
      {
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.Paused) || (this.StateFlags & (LevelStateFlags.FadingIn | LevelStateFlags.FadingOut | LevelStateFlags.CompletingStage | LevelStateFlags.Restarting | LevelStateFlags.TitleCardActive | LevelStateFlags.Dead | LevelStateFlags.StageCompleted | LevelStateFlags.TimeOver | LevelStateFlags.GameOver | LevelStateFlags.TimeUp)) != (LevelStateFlags) 0)
          return;
        this.StateFlags |= LevelStateFlags.Paused;
        this.SoundManager.PauseAll();
        if (this._earthquakeSampleInstance == null)
          return;
        this._earthquakeSampleInstance.Stop();
      }

      public void Unpause()
      {
        if (!this.StateFlags.HasFlag((Enum) LevelStateFlags.Paused) && !this.StateFlags.HasFlag((Enum) LevelStateFlags.GameOver))
          return;
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.Paused))
          this.SoundManager.ResumeAll();
        this.StateFlags &= ~LevelStateFlags.Paused;
        if (this._earthquakeSampleInstance == null || !this._earthquakeIsCurrentlyActive)
          return;
        this._earthquakeSampleInstance.Play();
      }

      private void ReadPlayerInput()
      {
        if (this._gameContext.Console.IsOpen || !this.StateFlags.HasFlag((Enum) LevelStateFlags.AllowCharacterControl))
          return;
        Controller controller1 = this._gameContext.Current[this._player.ProtagonistGamepadIndex];
        Controller controller2 = this._gameContext.Pressed[this._player.ProtagonistGamepadIndex];
        Controller controller3 = this._gameContext.Released[this._player.ProtagonistGamepadIndex];
        this._player.Protagonist.Input = new CharacterInputState()
        {
          Throttle = controller1.DirectionLeft.X,
          VerticalDirection = Math.Sign(controller1.DirectionLeft.Y),
          A = Level.GetButtonState(controller1.Action1, controller2.Action1, controller3.Action1),
          B = Level.GetButtonState(controller1.Action2, controller2.Action2, controller3.Action2),
          C = Level.GetButtonState(controller1.Action3, controller2.Action3, controller3.Action3)
        };
        Controller controller4 = this._gameContext.Current[this._player.SidekickGamepadIndex];
        Controller controller5 = this._gameContext.Pressed[this._player.SidekickGamepadIndex];
        Controller controller6 = this._gameContext.Released[this._player.SidekickGamepadIndex];
        if (this._player.Sidekick == null)
          return;
        this._player.Sidekick.Input = new CharacterInputState()
        {
          Throttle = controller4.DirectionLeft.X,
          VerticalDirection = Math.Sign(controller4.DirectionLeft.Y),
          A = Level.GetButtonState(controller4.Action1, controller5.Action1, controller6.Action1),
          B = Level.GetButtonState(controller4.Action2, controller5.Action2, controller6.Action2),
          C = Level.GetButtonState(controller4.Action3, controller5.Action3, controller6.Action3)
        };
      }

      private static CharacterInputButtonState GetButtonState(bool down, bool pressed, bool released)
      {
        return !down ? (!released ? CharacterInputButtonState.Up : CharacterInputButtonState.Released) : (!pressed ? CharacterInputButtonState.Down : CharacterInputButtonState.Pressed);
      }

      public void Draw(Renderer renderer)
      {
        this._levelRenderer.Render(renderer, new Viewport((Rectanglei) new Rectangle(0.0, 0.0, 1920.0, 1080.0))
        {
          Bounds = (Rectanglei) this._camera.Bounds
        }, this._layerViewOptions);
        this._camera.Draw(renderer);
        if (this.ShowHUD)
          this._hud.Draw(renderer);
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.CompletingStage))
          this._completeHud.Draw(renderer);
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.TimeOver) || this.StateFlags.HasFlag((Enum) LevelStateFlags.GameOver))
          this._gameOverHud.Draw(renderer);
        if (this.StateFlags.HasFlag((Enum) LevelStateFlags.TitleCardActive))
          this._titleCard.Draw(renderer);
        this.DrawDebugThings(renderer);
        renderer.DeativateRenderer();
        this._fadeTransitionRenderer.Opacity = (float) -this._fadeTransition.Opacity;
        this._fadeTransitionRenderer.Render();
      }

      private void DrawMusicFrequency(Renderer renderer)
      {
        if (this._soundManager == null || this._soundManager.MusicInstance == null || this._soundManager.MusicInstance.LastReadBytes == null || ((IReadOnlyCollection<byte>) this._soundManager.MusicInstance.LastReadBytes).Count == 0)
          return;
        float[] leftSamples;
        int samples = Sample.PCMToSamples(((IEnumerable<byte>) this._soundManager.MusicInstance.LastReadBytes).ToArray<byte>(), out leftSamples, out float[] _);
        ComplexNumber[] data = new ComplexNumber[samples];
        for (int index = 0; index < samples; ++index)
          data[index] = new ComplexNumber((double) leftSamples[index], 0.0);
        int length = 256 /*0x0100*/;
        ComplexNumber[] range = data.GetRange<ComplexNumber>(0, length);
        FastFourierTransform.TimeToFrequency((int) Math.Log((double) range.Length, 2.0), range);
        int width = 1024 /*0x0400*/ / (length / 2);
        I2dRenderer obj = renderer.Get2dRenderer();
        obj.RenderQuad(new Colour((byte) 76, (byte) 0, (byte) 0, (byte) 0), new Rectangle(0.0, 0.0, 1024.0, 1024.0));
        for (int index = 0; index < range.Length / 2; ++index)
        {
          double num1 = 10.0 * Math.Log10(range[index].Magnitude);
          double num2 = -90.0;
          if (num1 < num2)
            num1 = num2;
          double height = 1024.0 - num1 / num2 * 1024.0;
          obj.RenderQuad(Colours.White, new Rectangle((double) (index * width), 1024.0 - height, (double) width, height));
        }
      }

      private void DrawDebugThings(Renderer renderer)
      {
        if (this.ShowCharacterInfo)
          (this.Player.Protagonist as Character).DrawNewCollisionDebug(renderer);
        this._debugContext.Draw(renderer);
      }

      private void StartTitleCard(bool seamless = false)
      {
        this.StateFlags |= LevelStateFlags.TitleCardActive;
        this._titleCard.Seamless = seamless;
        this._titleCard.Start();
      }

      private void StartCompleteStageHud()
      {
        this.StateFlags |= LevelStateFlags.CompletingStage;
        this._completeHud.Start();
      }

      private void StartTimeOverSequence()
      {
        this.StateFlags &= ~(LevelStateFlags.Animating | LevelStateFlags.Updating);
        this.StateFlags |= LevelStateFlags.TimeOver;
        this._gameOverHud.Start(GameOverHud.Reason.TimeOver);
      }

      private void StartGameOverSequence()
      {
        this.StateFlags &= ~(LevelStateFlags.Animating | LevelStateFlags.Updating);
        this.StateFlags |= LevelStateFlags.GameOver;
        this._gameOverHud.Start(GameOverHud.Reason.GameOver);
      }

      public void FadeOut(LevelState finishingState)
      {
        this._soundManager.FadeOutMusic(60);
        this._fadeTransition.BeginFadeOut();
        this.StateFlags |= LevelStateFlags.FadingOut;
        this._fadeOutToState = finishingState;
      }

      public void JustAboutToCompleteLevel()
      {
        this.StateFlags &= ~LevelStateFlags.UpdateTime;
        this.Camera.CentreObjectToTrack();
        this.Bounds = (Rectanglei) ((Rectangle) this.Bounds with
        {
          Left = this.Camera.Bounds.Left,
          Right = this.Camera.Bounds.Right
        });
        this._player.RemovePowerups();
      }

      public void CompleteLevel()
      {
        ICharacter protagonist = this._player.Protagonist;
        if (protagonist.IsAirborne)
        {
          this.StateFlags |= LevelStateFlags.WaitingForCharacterToWin;
        }
        else
        {
          this.StateFlags &= ~LevelStateFlags.WaitingForCharacterToWin;
          protagonist.IsWinning = true;
          this.StartCompleteStageHud();
        }
      }

      public void SetInterval(int ticks, Action callback)
      {
        this._intervals.Add(new Tuple<int, Action>(this.Ticks + ticks, callback));
      }

      private void UpdateInverals()
      {
        foreach (Tuple<int, Action> tuple in this._intervals.Where<Tuple<int, Action>>((Func<Tuple<int, Action>, bool>) (x => x.Item1 <= this.Ticks)))
          tuple.Item2();
        this._intervals.RemoveAll((Predicate<Tuple<int, Action>>) (x => x.Item1 == this.Ticks));
      }

      public void ScrollBoundsTo(Rectanglei newBounds, int speed)
      {
        this._targetBounds = newBounds;
        this._boundsScrollSpeed = speed;
        this.Bounds = Rectanglei.Union((Rectanglei) this._camera.Bounds, newBounds);
      }

      private void UpdateScrollBounds()
      {
        if (!this._cameraLockedSeamless)
        {
          Vector2 maxVelocity = this.Camera.MaxVelocity;
          if (maxVelocity.X < 64.0)
          {
            maxVelocity.X = Math.Min(maxVelocity.X + 0.5, 64.0);
            maxVelocity.Y = maxVelocity.X;
            this.Camera.MaxVelocity = maxVelocity;
          }
        }
        if (this._boundsScrollSpeed == 0)
          return;
        Rectanglei bounds = this.Bounds;
        bounds.Left = MathX.GoTowards(bounds.Left, this._targetBounds.Left, this._boundsScrollSpeed);
        bounds.Top = MathX.GoTowards(bounds.Top, this._targetBounds.Top, this._boundsScrollSpeed);
        bounds.Right = MathX.GoTowards(bounds.Right, this._targetBounds.Right, this._boundsScrollSpeed);
        bounds.Bottom = MathX.GoTowards(bounds.Bottom, this._targetBounds.Bottom, this._boundsScrollSpeed);
        this.Bounds = bounds;
        if (!(bounds == this._targetBounds))
          return;
        this._boundsScrollSpeed = 0;
      }

      private void SetObjectLifetimeArea()
      {
        this._objectManager.ResetLifetimeArea();
        int num = 1024 /*0x0400*/;
        foreach (ICharacter character in this._objectManager.Characters)
        {
          ObjectManager objectManager = this._objectManager;
          Vector2i position = character.Position;
          int x = position.X - num;
          position = character.Position;
          int y = position.Y - num;
          int width = num * 2;
          int height = num * 2;
          Rectanglei area = new Rectanglei(x, y, width, height);
          objectManager.AddLifeArea(area);
        }
        this._objectManager.AddLifeArea(((Rectanglei) this._camera.Bounds).Inflate(new Vector2i(num, num)));
      }

      public void CreateScoreObject(int points, Vector2i position)
      {
        if (this.Map.Layers.Count == 0)
          return;
        this._objectManager.AddObject(new ObjectPlacement(this._commonResources.GetResourcePath("pointsobject"), this.Map.Layers.IndexOf(this.Map.Layers.Last<LevelLayer>()), position, (object) new
        {
          Value = points
        }));
      }

      public void CreateRandomAnimalObject(int layer, Vector2i position, int direction = -1, int delay = 0)
      {
        if (this.Map.Layers.Count == 0)
          return;
        string animalResourceKey = this.GetRandomAnimalResourceKey();
        if (string.IsNullOrEmpty(animalResourceKey))
          return;
        this._objectManager.AddObject(new ObjectPlacement(animalResourceKey, layer, position, (object) new
        {
          Delay = delay,
          Direction = direction
        }));
      }

      private string GetRandomAnimalResourceKey()
      {
        string[] array = ((IEnumerable<string>) this.AnimalResourceKeys).ToArray<string>();
        return array.Length != 0 ? array[this._random.Next(array.Length)] : (string) null;
      }

      public LevelMarker GetMarker(string name)
      {
        LevelMarker marker;
        if (this.TryGetMarker(name, out marker))
          return marker;
        throw new ArgumentException("No marker named " + name);
      }

      public bool TryGetMarker(string name, out LevelMarker marker)
      {
        return this._markers.TryGetValue(name, out marker);
      }

      public void SetStartPosition(string markerName)
      {
        this.SetStartPosition(this.GetMarker(markerName));
      }

      public void SetStartPosition(LevelMarker marker)
      {
        this.StartPosition = marker.Position;
        this.StartLayerIndex = this.Map.Layers.IndexOf(marker.Layer);
      }

      public LevelMarker[] GetMarkersWithTag(string tag)
      {
        List<LevelMarker> levelMarkerList = new List<LevelMarker>();
        foreach (LevelMarker marker in (IEnumerable<LevelMarker>) this.Map.Markers)
        {
          if (string.Compare(tag, marker.Tag, true) == 0)
            levelMarkerList.Add(marker);
        }
        return levelMarkerList.ToArray();
      }

      public bool EarthquakeActive { get; set; }

      private void InitialiseEarthquake()
      {
        this._earthquakeIsCurrentlyActive = false;
        this._earthquakeFade = 0.0;
        this.EarthquakeActive = false;
      }

      private void StopEarthquake()
      {
        this._earthquakeIsCurrentlyActive = false;
        this._earthquakeFade = 0.0;
        this.EarthquakeActive = false;
        if (this._earthquakeSampleInstance == null)
          return;
        this._earthquakeSampleInstance.Stop();
      }

      private void UpdateEarthquake()
      {
        this.GameContext.Output[0] = new GamePadOutputState();
        if (!this.EarthquakeActive && this._earthquakeFade == 0.0)
          return;
        if (this.EarthquakeActive && this._earthquakeFade < 1.0)
        {
          if (this._earthquakeSampleInstance == null)
            this._earthquakeSampleInstance = new SampleInstance(this._gameContext, this._gameContext.ResourceTree.GetLoadedResource<SampleInfo>(this._commonResources.GetResourcePath("earthquakesample")));
          if (!this._earthquakeSampleInstance.Playing)
          {
            this._earthquakeSampleInstance.SeekToStart();
            this._earthquakeSampleInstance.Play();
          }
          this._earthquakeIsCurrentlyActive = true;
          this._earthquakeFade = Math.Min(this._earthquakeFade + 1.0 / 90.0, 1.0);
        }
        else if (this._earthquakeIsCurrentlyActive && this._earthquakeFade > 0.0)
        {
          this._earthquakeFade -= 1.0 / 90.0;
          if (this._earthquakeFade <= 0.0)
          {
            this._earthquakeFade = 0.0;
            this._earthquakeIsCurrentlyActive = false;
            if (this._earthquakeSampleInstance == null)
              return;
            this._earthquakeSampleInstance.Stop();
            return;
          }
        }
        this._earthquakeSampleInstance.Volume = this._earthquakeFade;
        int maxValue = (int) (this._earthquakeFade * 16.0);
        this._camera.Shift(this._random.Next(-maxValue, maxValue), this._random.Next(-maxValue, maxValue));
        double val1 = this._earthquakeFade * 1.0;
        this.GameContext.Output[0] = new GamePadOutputState()
        {
          LeftVibration = Math.Min(val1, 0.5 + this._random.NextDouble() * 0.5),
          RightVibration = Math.Min(val1, 0.5 + this._random.NextDouble() * 0.5)
        };
      }
    }
}
