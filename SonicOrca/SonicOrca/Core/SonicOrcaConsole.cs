// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.SonicOrcaConsole
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Objects;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Input;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    public class SonicOrcaConsole : IDisposable
    {
      private const string DefaultFontResourceKey = "SONICORCA/FONTS/SEGOEUI";
      private readonly SonicOrcaGameContext _context;
      private ResourceSession _resourceSession;
      private Font _font;
      private bool _loadingResources;
      private bool _resourcesLoaded;
      private float _opacity;
      private readonly StringBuilder _currentLine = new StringBuilder();
      private readonly List<string> _buffer = new List<string>();
      private readonly List<string> _history = new List<string>();
      private int _historyScrollIndex;

      public bool IsOpen { get; set; }

      public LevelGameState LevelGameState { get; set; }

      public SonicOrcaConsole(SonicOrcaGameContext context) => this._context = context;

      public void LoadResources()
      {
        if (this._loadingResources)
          return;
        this._loadingResources = true;
        Task.Run((Func<Task>) (() => this.LoadResourcesAsync()));
      }

      public async Task LoadResourcesAsync()
      {
        ResourceTree resourceTree = this._context.ResourceTree;
        this._resourceSession = new ResourceSession(resourceTree);
        this._resourceSession.PushDependency("SONICORCA/FONTS/SEGOEUI");
        await this._resourceSession.LoadAsync();
        this._font = resourceTree.GetLoadedResource<Font>("SONICORCA/FONTS/SEGOEUI");
        this._loadingResources = false;
        this._resourcesLoaded = true;
      }

      public void Dispose()
      {
        if (this._resourceSession == null)
          return;
        this._resourceSession.Dispose();
      }

      public void Update()
      {
        if (!this._resourcesLoaded)
        {
          this.LoadResources();
        }
        else
        {
          InputContext input = this._context.Input;
          KeyboardState keyboard = input.Pressed.Keyboard;
          if (this.IsOpen)
          {
            this._opacity = Math.Min(this._opacity + 0.06666667f, 1f);
            if (keyboard[42])
            {
              if (this._currentLine.Length <= 0)
                return;
              this._currentLine.Remove(this._currentLine.Length - 1, 1);
            }
            else if (keyboard[40])
            {
              if (this._currentLine.Length <= 0)
                return;
              string line = this._currentLine.ToString();
              this._currentLine.Clear();
              if (this._history.Count >= 128 /*0x80*/)
                this._history.RemoveAt(0);
              this._history.Add(line);
              this._historyScrollIndex = this._history.Count;
              this.WriteLine("> " + line);
              this.ProcessLine(line);
            }
            else if (keyboard[82])
            {
              this._currentLine.Clear();
              this._historyScrollIndex = Math.Max(0, this._historyScrollIndex - 1);
              if (this._historyScrollIndex >= this._history.Count)
                return;
              this._currentLine.Append(this._history[this._historyScrollIndex]);
            }
            else if (keyboard[81])
            {
              this._currentLine.Clear();
              this._historyScrollIndex = Math.Min(this._history.Count, this._historyScrollIndex + 1);
              if (this._historyScrollIndex >= this._history.Count)
                return;
              this._currentLine.Append(this._history[this._historyScrollIndex]);
            }
            else
            {
              if (input.TextInput == null)
                return;
              this._currentLine.Append(input.TextInput);
            }
          }
          else
            this._opacity = Math.Max(this._opacity - 0.06666667f, 0.0f);
        }
      }

      public void Draw(Renderer renderer)
      {
        if (!this._resourcesLoaded || (double) this._opacity == 0.0)
          return;
        int height = this._font.Height;
        Colour colour1 = new Colour(0.8 * (double) this._opacity, 0.1, 0.1, 0.1);
        Colour colour2 = new Colour(1.0 * (double) this._opacity, 1.0, 1.0, 1.0);
        Vector2i clientSize = renderer.Window.ClientSize;
        Rectanglei destination1 = new Rectanglei(8, 8, clientSize.X - 16 /*0x10*/, 6 * height);
        Rectanglei destination2 = (Rectanglei) new Rectangle(8.0, (double) (destination1.Bottom + 8), (double) (clientSize.X - 16 /*0x10*/), (double) height);
        I2dRenderer obj = renderer.Get2dRenderer();
        obj.RenderQuad(colour1, (Rectangle) destination1);
        obj.RenderQuad(colour1, (Rectangle) destination2);
        IFontRenderer fontRenderer = renderer.GetFontRenderer();
        int bottom = destination1.Bottom;
        for (int index = 0; index < Math.Min(this._buffer.Count, 6); ++index)
        {
          string text = this._buffer[this._buffer.Count - 1 - index];
          bottom -= height;
          fontRenderer.RenderString(text, new Rectangle((double) (destination1.X + 4), (double) bottom, (double) destination1.Width, (double) height), FontAlignment.MiddleY, this._font, colour2);
        }
        fontRenderer.RenderString("> " + this._currentLine.ToString(), (Rectangle) destination2.Inflate(new Vector2i(-4, 0)), FontAlignment.MiddleY, this._font, colour2);
      }

      public void WriteLine(string line)
      {
        if (this._buffer.Count >= 128 /*0x80*/)
          this._buffer.RemoveAt(0);
        this._buffer.Add(line);
      }

      private void ProcessLine(string line)
      {
        string[] tokens = SonicOrcaConsole.GetTokens(line);
        if (tokens.Length == 0)
          return;
        string[] array = ((IEnumerable<string>) tokens).Skip<string>(1).ToArray<string>();
        switch (tokens[0].ToLower())
        {
          case "barrier":
            this.ProcessCommandBarrier(array);
            break;
          case "debug":
            this.ProcessCommandDebug(array);
            break;
          case "spawn":
            this.ProcessCommandSpawn(array);
            break;
          case "teleport":
            this.ProcessCommandTeleport(array);
            break;
          case "restart":
            this.ProcessCommandRestart(array);
            break;
        }
      }

      private void ProcessCommandBarrier(string[] args)
      {
        if (args.Length < 1)
        {
          this.WriteLine("usage: barrier <none|classic|bubble|fire|lightning>");
        }
        else
        {
          BarrierType barrierType;
          switch (args[0].ToLower())
          {
            case "none":
              barrierType = BarrierType.None;
              break;
            case "classic":
              barrierType = BarrierType.Classic;
              break;
            case "bubble":
              barrierType = BarrierType.Bubble;
              break;
            case "fire":
              barrierType = BarrierType.Fire;
              break;
            case "lightning":
              barrierType = BarrierType.Lightning;
              break;
            default:
              this.WriteLine("usage: barrier <classic|bubble|fire|lightning>");
              return;
          }
          LevelGameState levelGameState = (LevelGameState) null;
          if (levelGameState == null || levelGameState.Level == null)
            return;
          ICharacter protagonist = levelGameState.Player.Protagonist;
          if (protagonist == null)
            return;
          protagonist.Barrier = barrierType;
        }
      }

      private void ProcessCommandDebug(string[] args)
      {
        if (this.LevelGameState == null || this.LevelGameState.Level == null)
          return;
        Level level = this.LevelGameState.Level;
        if (args.Length == 0)
        {
          level.ClassicDebugMode = !level.ClassicDebugMode;
        }
        else
        {
          switch (args[0].ToLower())
          {
            case "collision":
              if (level.LayerViewOptions.ShowLandscapeCollision)
              {
                level.LayerViewOptions.ShowLandscape = true;
                level.LayerViewOptions.ShowLandscapeCollision = false;
                level.LayerViewOptions.ShowObjectCollision = false;
                level.ShowCharacterInfo = false;
                break;
              }
              level.LayerViewOptions.ShowLandscape = false;
              level.LayerViewOptions.ShowLandscapeCollision = true;
              level.LayerViewOptions.ShowObjectCollision = true;
              level.ShowCharacterInfo = true;
              break;
            case "invulnerable":
              level.Player.Protagonist.IsInvincible = !level.Player.Protagonist.IsInvincible;
              break;
          }
        }
      }

      private void ProcessCommandSpawn(string[] args)
      {
        if (this.LevelGameState == null || this.LevelGameState.Level == null)
          return;
        Level level = this.LevelGameState.Level;
        if (args.Length < 1)
          return;
        string objectType = "SONICORCA/OBJECTS/" + args[0].ToUpper();
        if (!((IEnumerable<ObjectType>) level.ObjectManager.RegisteredTypes).Any<ObjectType>((Func<ObjectType, bool>) (x => x.ResourceKey == objectType)))
          return;
        Dictionary<string, object> state = new Dictionary<string, object>();
        for (int index = 1; index < args.Length; ++index)
        {
          if (args[index].StartsWith("-") || index >= args.Length - 2)
          {
            string key = args[index].Substring(1);
            object behaviourValue = SonicOrcaConsole.ParseBehaviourValue(args[index + 1]);
            state.Add(key, behaviourValue);
            ++index;
          }
        }
        Vector2i position = level.Player.Protagonist.Position;
        LevelLayer layer = level.Map.Layers[level.Map.CollisionPathLayers.Last<int>()];
        ObjectPlacement objectPlacement = new ObjectPlacement(objectType, level.Map.Layers.IndexOf(layer), position, (object) state);
        level.ObjectManager.AddObject(objectPlacement);
      }

      private static object ParseBehaviourValue(string value)
      {
        int result1;
        if (int.TryParse(value, out result1))
          return (object) result1;
        double result2;
        if (double.TryParse(value, out result2))
          return (object) result2;
        bool result3;
        return bool.TryParse(value, out result3) ? (object) result3 : (object) value;
      }

      private void ProcessCommandTeleport(string[] args)
      {
        if (this.LevelGameState == null || this.LevelGameState.Level == null)
          return;
        Level level = this.LevelGameState.Level;
        int result1;
        int result2;
        if (args.Length < 2 || !int.TryParse(args[0], out result1) || !int.TryParse(args[1], out result2))
          return;
        level.Player.Protagonist.Position = new Vector2i(result1, result2);
      }

      private void ProcessCommandRestart(string[] args)
      {
        if (this.LevelGameState == null || this.LevelGameState.Level == null)
          return;
        Level level = this.LevelGameState.Level;
        string str = string.Empty;
        if (args.Length >= 1)
          str = args[0].ToLower();
        if (!(str == "checkpoint") && !(str == "starpost"))
          level.FadeOut(LevelState.Restart);
        else
          level.FadeOut(LevelState.RestartCheckpoint);
      }

      private static string[] GetTokens(string line)
      {
        List<string> stringList = new List<string>();
        StringBuilder stringBuilder = new StringBuilder();
        bool flag = false;
        foreach (char c in line)
        {
          if (c == '"')
            flag = !flag;
          else if (char.IsWhiteSpace(c) && !flag)
          {
            if (stringBuilder.Length > 0)
            {
              stringList.Add(stringBuilder.ToString());
              stringBuilder.Clear();
            }
          }
          else
            stringBuilder.Append(c);
        }
        if (stringBuilder.Length > 0)
          stringList.Add(stringBuilder.ToString());
        return stringList.ToArray();
      }
    }
}
