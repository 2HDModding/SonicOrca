// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Debugging.DebugOptionDefinitions
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core.Debugging;

internal static class DebugOptionDefinitions
{
  public static IEnumerable<DebugOption> CreateOptionsInOrder(DebugContext context)
  {
    return (IEnumerable<DebugOption>) new DebugOption[22]
    {
      (DebugOption) new DebugOptionDefinitions.PlatformStats(context),
      (DebugOption) new DebugOptionDefinitions.LevelInfo(context),
      (DebugOption) new DebugOptionDefinitions.PlayerInfo(context),
      (DebugOption) new DebugOptionDefinitions.ShowHud(context),
      (DebugOption) new DebugOptionDefinitions.ClassicDebugMode(context),
      (DebugOption) new DebugOptionDefinitions.CameraInfo(context),
      (DebugOption) new DebugOptionDefinitions.CameraShowInfo(context),
      (DebugOption) new DebugOptionDefinitions.CameraMode(context),
      (DebugOption) new DebugOptionDefinitions.CameraZoom(context),
      (DebugOption) new DebugOptionDefinitions.LandscapeInfo(context),
      (DebugOption) new DebugOptionDefinitions.LandscapeVisible(context),
      (DebugOption) new DebugOptionDefinitions.LandscapeAnimate(context),
      (DebugOption) new DebugOptionDefinitions.CollisionShowLandscape(context),
      (DebugOption) new DebugOptionDefinitions.CollisionShowObjects(context),
      (DebugOption) new DebugOptionDefinitions.CollisionStats(context),
      (DebugOption) new DebugOptionDefinitions.ObjectsVisible(context),
      (DebugOption) new DebugOptionDefinitions.ObjectsAnimate(context),
      (DebugOption) new DebugOptionDefinitions.ObjectsShowCharacterInfo(context),
      (DebugOption) new DebugOptionDefinitions.ObjectsShowSidekickIntel(context),
      (DebugOption) new DebugOptionDefinitions.ObjectsStats(context),
      (DebugOption) new DebugOptionDefinitions.WaterInfo(context),
      (DebugOption) new DebugOptionDefinitions.WaterVisible(context)
    };
  }

  public class PlatformStats(DebugContext context) : InformationDebugOption(context, "PLATFORM", "")
  {
    protected override IEnumerable<IEnumerable<KeyValuePair<string, object>>> GetInformation()
    {
      IPlatform platform = this.Context.Level.GameContext.Platform;
      IGraphicsContext graphicsContext = platform.Window.GraphicsContext;
      double totalMemory = (double) platform.TotalMemory;
      double num1 = (double) Environment.WorkingSet / 1048576.0;
      int num2 = (int) Math.Ceiling(num1 / totalMemory * 100.0);
      return (IEnumerable<IEnumerable<KeyValuePair<string, object>>>) new KeyValuePair<string, object>[10][]
      {
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("NAME", (object) "SONICORCA"),
          new KeyValuePair<string, object>("VERSION", (object) SonicOrcaInfo.Version)
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("ARCHITECTURE", Environment.Is64BitOperatingSystem ? (object) "x64" : (object) "x86"),
          new KeyValuePair<string, object>("OS ARCHITECTURE", Environment.Is64BitOperatingSystem ? (object) "x64" : (object) "x86")
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("OS", (object) Environment.OSVersion),
          new KeyValuePair<string, object>("MACHINE NAME", (object) Environment.MachineName)
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("LOGICAL PROCESSORS", (object) Environment.ProcessorCount),
          new KeyValuePair<string, object>("TOTAL MEMORY", (object) $"{totalMemory / 1024.0:0.0} GB")
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("USER", (object) Environment.UserName),
          new KeyValuePair<string, object>("MEMORY USAGE", (object) $"{num1:0.0} MB ({num2}%)")
        },
        new KeyValuePair<string, object>[0],
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("WINDOW SIZE", (object) $"{platform.Window.ClientSize.X} X {platform.Window.ClientSize.Y}"),
          new KeyValuePair<string, object>("FULLSCREEN", (object) platform.Window.FullScreen)
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("GRAPHICS API", (object) platform.GraphicsAPI),
          new KeyValuePair<string, object>("VENDOR", (object) platform.GraphicsVendor)
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("TEXTURES", (object) graphicsContext.Textures.Count),
          new KeyValuePair<string, object>("SHADERS", (object) graphicsContext.ShaderPrograms.Count)
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("RENDER TARGETS", (object) graphicsContext.RenderTargets.Count),
          new KeyValuePair<string, object>("VERTEX BUFFERS", (object) graphicsContext.VertexBuffers.Count)
        }
      };
    }
  }

  public class LevelInfo(DebugContext context) : InformationDebugOption(context, "LEVEL", "")
  {
    protected override IEnumerable<IEnumerable<KeyValuePair<string, object>>> GetInformation()
    {
      Level level = this.Context.Level;
      KeyValuePair<string, object>[] array = level.Area.StateVariables.ToArray<KeyValuePair<string, object>>();
      KeyValuePair<string, object>[] destinationArray1 = new KeyValuePair<string, object>[(array.Length + 1) / 2];
      KeyValuePair<string, object>[] destinationArray2 = new KeyValuePair<string, object>[array.Length - destinationArray1.Length];
      if (destinationArray1.Length != 0)
        Array.Copy((Array) array, 0, (Array) destinationArray1, 0, destinationArray1.Length);
      if (destinationArray2.Length != 0)
        Array.Copy((Array) array, destinationArray1.Length, (Array) destinationArray2, 0, destinationArray2.Length);
      List<IEnumerable<KeyValuePair<string, object>>> second = new List<IEnumerable<KeyValuePair<string, object>>>();
      for (int index = 0; index < destinationArray1.Length; ++index)
        second.Add((IEnumerable<KeyValuePair<string, object>>) new KeyValuePair<string, object>[2]
        {
          destinationArray1[index],
          destinationArray2.Length > index ? destinationArray2[index] : new KeyValuePair<string, object>("", (object) "")
        });
      KeyValuePair<string, object>[][] first = new KeyValuePair<string, object>[5][]
      {
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("NAME", (object) (level.Name + (level.ShowAsAct ? " Zone" : string.Empty))),
          new KeyValuePair<string, object>("ACT", (object) level.CurrentAct)
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("TICKS", (object) level.Ticks),
          new KeyValuePair<string, object>("TIME", (object) string.Format("{0}, {1:mm':'ss':'fff}", (object) level.Time, (object) TimeSpan.FromSeconds((double) level.Time / 60.0)))
        },
        null,
        null,
        null
      };
      KeyValuePair<string, object>[] keyValuePairArray = new KeyValuePair<string, object>[2];
      object[] objArray = new object[4];
      Rectanglei bounds1 = level.Bounds;
      objArray[0] = (object) bounds1.Left;
      bounds1 = level.Bounds;
      objArray[1] = (object) bounds1.Top;
      bounds1 = level.Bounds;
      objArray[2] = (object) bounds1.Right;
      Rectanglei bounds2 = level.Bounds;
      objArray[3] = (object) bounds2.Bottom;
      keyValuePairArray[0] = new KeyValuePair<string, object>("BOUNDS", (object) string.Format("{0}, {1} - {2}, {3}", objArray));
      bounds2 = level.Bounds;
      // ISSUE: variable of a boxed type
      __Boxed<int> width = (ValueType) bounds2.Width;
      bounds2 = level.Bounds;
      // ISSUE: variable of a boxed type
      __Boxed<int> height = (ValueType) bounds2.Height;
      keyValuePairArray[1] = new KeyValuePair<string, object>("BOUNDS SIZE", (object) $"{width} X {height}");
      first[2] = keyValuePairArray;
      first[3] = new KeyValuePair<string, object>[2]
      {
        new KeyValuePair<string, object>("LAYERS", (object) level.Map.Layers.Count),
        new KeyValuePair<string, object>("", (object) "")
      };
      first[4] = new KeyValuePair<string, object>[0];
      return ((IEnumerable<IEnumerable<KeyValuePair<string, object>>>) first).Concat<IEnumerable<KeyValuePair<string, object>>>((IEnumerable<IEnumerable<KeyValuePair<string, object>>>) second);
    }
  }

  public class PlayerInfo(DebugContext context) : InformationDebugOption(context, "LEVEL", "PLAYER")
  {
    protected override IEnumerable<IEnumerable<KeyValuePair<string, object>>> GetInformation()
    {
      Level level = this.Context.Level;
      Player player = this.Context.Level.Player;
      return (IEnumerable<IEnumerable<KeyValuePair<string, object>>>) new KeyValuePair<string, object>[7][]
      {
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("PROTAGONIST", (object) "SONIC"),
          new KeyValuePair<string, object>("SIDEKICK", (object) "TAILS")
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("SCORE", (object) player.Score),
          new KeyValuePair<string, object>("LIFE TARGET SCORE", (object) player.TargetScoreForNextLife)
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("CURRENT RINGS", (object) player.CurrentRings),
          new KeyValuePair<string, object>("LIFE TARGET RINGS", (object) player.TargetRingCountForNextLife)
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("TOTAL COLLECTED RINGS", (object) player.TotalRings),
          new KeyValuePair<string, object>("PERFECT BONUS", (object) (player.TotalRings + level.ObjectManager.ObjectEntryTable.GetRingCount()))
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("LIVES", (object) player.Lives),
          new KeyValuePair<string, object>("", (object) "")
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("STARPOST ID", (object) player.StarpostIndex),
          new KeyValuePair<string, object>("STARPOST TIME", (object) string.Format("{0}, {1:mm':'ss':'fff}", (object) player.StarpostTime, (object) TimeSpan.FromSeconds((double) player.StarpostTime / 60.0)))
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("INVINCIBILITY, TICKS LEFT", (object) player.InvincibillityTicks),
          new KeyValuePair<string, object>("SPEED SHOES, TICKS LEFT", (object) player.SpeedShoesTicks)
        }
      };
    }
  }

  public class ShowHud : DiscreteDebugOption<bool>
  {
    public ShowHud(DebugContext context)
      : base(context, "LEVEL", "", "SHOW HUD", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
      {
        new KeyValuePair<string, bool>("YES", true),
        new KeyValuePair<string, bool>("NO", false)
      })
    {
      this.SelectedValue = true;
    }

    public override void OnChange() => this.Context.Level.ShowHUD = this.SelectedValue;
  }

  public class ClassicDebugMode : DiscreteDebugOption<bool>
  {
    public ClassicDebugMode(DebugContext context)
      : base(context, "LEVEL", "", "CLASSIC DEBUG MODE", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
      {
        new KeyValuePair<string, bool>("YES", true),
        new KeyValuePair<string, bool>("NO", false)
      })
    {
      this.SelectedValue = false;
    }

    public override void OnChange() => this.Context.Level.ClassicDebugMode = this.SelectedValue;
  }

  public class CameraInfo(DebugContext context) : InformationDebugOption(context, "CAMERA", "")
  {
    protected override IEnumerable<IEnumerable<KeyValuePair<string, object>>> GetInformation()
    {
      Camera camera = this.Context.Level.Camera;
      KeyValuePair<string, object>[][] information = new KeyValuePair<string, object>[2][];
      KeyValuePair<string, object>[] keyValuePairArray = new KeyValuePair<string, object>[2];
      keyValuePairArray[0] = new KeyValuePair<string, object>("BOUNDS", (object) $"{camera.Bounds.Left}, {camera.Bounds.Top} - {camera.Bounds.Right}, {camera.Bounds.Bottom}");
      Rectangle bounds = camera.Bounds;
      // ISSUE: variable of a boxed type
      __Boxed<double> width = (ValueType) bounds.Width;
      bounds = camera.Bounds;
      // ISSUE: variable of a boxed type
      __Boxed<double> height = (ValueType) bounds.Height;
      keyValuePairArray[1] = new KeyValuePair<string, object>("SIZE", (object) $"{width} X {height}");
      information[0] = keyValuePairArray;
      information[1] = new KeyValuePair<string, object>[2]
      {
        new KeyValuePair<string, object>("VELOCITY", (object) camera.Velocity),
        new KeyValuePair<string, object>("ACCELERATION", (object) camera.Acceleration)
      };
      return (IEnumerable<IEnumerable<KeyValuePair<string, object>>>) information;
    }
  }

  public class CameraShowInfo(DebugContext context) : DiscreteDebugOption<bool>(context, "CAMERA", "", "SHOW INFO", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
  {
    new KeyValuePair<string, bool>("YES", true),
    new KeyValuePair<string, bool>("NO", false)
  })
  {
    public override void OnChange()
    {
      this.Context.Level.Camera.ShowDebugInformation = this.SelectedValue;
    }
  }

  public class CameraMode(DebugContext context) : DiscreteDebugOption<int>(context, "CAMERA", "", "MODE", (IEnumerable<KeyValuePair<string, int>>) new KeyValuePair<string, int>[2]
  {
    new KeyValuePair<string, int>("TRACK", 0),
    new KeyValuePair<string, int>("FREECAM", 1)
  })
  {
    public override void OnChange() => this.Context.Level.Camera.SpyMode = this.SelectedValue == 1;
  }

  public class CameraZoom : DiscreteDebugOption<double>
  {
    public CameraZoom(DebugContext context)
      : base(context, "CAMERA", "", "ZOOM", (IEnumerable<KeyValuePair<string, double>>) new KeyValuePair<string, double>[5]
      {
        new KeyValuePair<string, double>("0.25", 0.25),
        new KeyValuePair<string, double>("0.50", 0.5),
        new KeyValuePair<string, double>("1.00", 1.0),
        new KeyValuePair<string, double>("2.00", 2.0),
        new KeyValuePair<string, double>("4.00", 4.0)
      })
    {
      this.SelectedValue = 1.0;
    }

    public override void OnChange() => this.Context.Level.Camera.SetScale(this.SelectedValue);
  }

  public class LandscapeInfo(DebugContext context) : InformationDebugOption(context, "LANDSCAPE", "")
  {
    protected override IEnumerable<IEnumerable<KeyValuePair<string, object>>> GetInformation()
    {
      TileSet tileSet = this.Context.Level.TileSet;
      return (IEnumerable<IEnumerable<KeyValuePair<string, object>>>) new KeyValuePair<string, object>[2][]
      {
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("TOTAL TILES", (object) tileSet.Count),
          new KeyValuePair<string, object>("ANIMATED TILES", (object) tileSet.Values.Count<ITile>((Func<ITile, bool>) (x => x.Animated)))
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("TILESET TEXTURES", (object) ((IReadOnlyCollection<ITexture>) tileSet.Textures).Count),
          new KeyValuePair<string, object>("", (object) "")
        }
      };
    }
  }

  public class LandscapeVisible(DebugContext context) : DiscreteDebugOption<bool>(context, "LANDSCAPE", "", "VISIBLE", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
  {
    new KeyValuePair<string, bool>("YES", true),
    new KeyValuePair<string, bool>("NO", false)
  })
  {
    public override void OnChange()
    {
      this.Context.Level.LayerViewOptions.ShowLandscape = this.SelectedValue;
    }
  }

  public class LandscapeAnimate(DebugContext context) : DiscreteDebugOption<bool>(context, "LANDSCAPE", "", "ANIMATE", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
  {
    new KeyValuePair<string, bool>("YES", true),
    new KeyValuePair<string, bool>("NO", false)
  })
  {
    public override void OnChange() => this.Context.Level.LandscapeAnimating = this.SelectedValue;
  }

  public class CollisionStats(DebugContext context) : InformationDebugOption(context, "COLLISION", "STATISTICS")
  {
    protected override IEnumerable<IEnumerable<KeyValuePair<string, object>>> GetInformation()
    {
      Level level = this.Context.Level;
      CollisionTable collisionTable = this.Context.Level.CollisionTable;
      QuadTree<CollisionVector> internalTree = collisionTable.InternalTree;
      return (IEnumerable<IEnumerable<KeyValuePair<string, object>>>) new KeyValuePair<string, object>[1][]
      {
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("TOTAL VECTORS", (object) collisionTable.Count),
          new KeyValuePair<string, object>("TOTAL QUADTREE NODES", (object) internalTree.GetDepth())
        }
      };
    }
  }

  public class CollisionShowLandscape : DiscreteDebugOption<bool>
  {
    public CollisionShowLandscape(DebugContext context)
      : base(context, "COLLISION", "", "SHOW LANDSCAPE", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
      {
        new KeyValuePair<string, bool>("YES", true),
        new KeyValuePair<string, bool>("NO", false)
      })
    {
      this.SelectedValue = false;
    }

    public override void OnChange()
    {
      this.Context.Level.LayerViewOptions.ShowLandscapeCollision = this.SelectedValue;
    }
  }

  public class CollisionShowObjects : DiscreteDebugOption<bool>
  {
    public CollisionShowObjects(DebugContext context)
      : base(context, "COLLISION", "", "SHOW OBJECTS", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
      {
        new KeyValuePair<string, bool>("YES", true),
        new KeyValuePair<string, bool>("NO", false)
      })
    {
      this.SelectedValue = false;
    }

    public override void OnChange()
    {
      this.Context.Level.LayerViewOptions.ShowObjectCollision = this.SelectedValue;
    }
  }

  public class ObjectsVisible(DebugContext context) : DiscreteDebugOption<bool>(context, "OBJECTS", "", "VISIBLE", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
  {
    new KeyValuePair<string, bool>("YES", true),
    new KeyValuePair<string, bool>("NO", false)
  })
  {
    public override void OnChange()
    {
      this.Context.Level.LayerViewOptions.ShowObjects = this.SelectedValue;
    }
  }

  public class ObjectsAnimate(DebugContext context) : DiscreteDebugOption<bool>(context, "OBJECTS", "", "ANIMATE", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
  {
    new KeyValuePair<string, bool>("YES", true),
    new KeyValuePair<string, bool>("NO", false)
  })
  {
    public override void OnChange() => this.Context.Level.ObjectsAnimating = this.SelectedValue;
  }

  public class ObjectsShowCharacterInfo : DiscreteDebugOption<bool>
  {
    public ObjectsShowCharacterInfo(DebugContext context)
      : base(context, "OBJECTS", "", "SHOW CHARACTER INFO", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
      {
        new KeyValuePair<string, bool>("YES", true),
        new KeyValuePair<string, bool>("NO", false)
      })
    {
      this.SelectedValue = false;
    }

    public override void OnChange() => this.Context.Level.ShowCharacterInfo = this.SelectedValue;
  }

  public class ObjectsShowSidekickIntel : DiscreteDebugOption<bool>
  {
    public ObjectsShowSidekickIntel(DebugContext context)
      : base(context, "OBJECTS", "", "SHOW SIDEKICK INTELLIGENCE", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
      {
        new KeyValuePair<string, bool>("YES", true),
        new KeyValuePair<string, bool>("NO", false)
      })
    {
      this.SelectedValue = false;
    }

    public override void OnChange()
    {
      this.Context.Level.ShowSidekickIntelligence = this.SelectedValue;
    }
  }

  public class ObjectsStats(DebugContext context) : InformationDebugOption(context, "OBJECTS", "STATISTICS")
  {
    protected override IEnumerable<IEnumerable<KeyValuePair<string, object>>> GetInformation()
    {
      Level level = this.Context.Level;
      return (IEnumerable<IEnumerable<KeyValuePair<string, object>>>) new KeyValuePair<string, object>[2][]
      {
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("TOTAL OBJECTS", (object) level.ObjectManager.ObjectEntryTable.Count),
          new KeyValuePair<string, object>("ACTIVE OBJECTS", (object) level.ObjectManager.ActiveObjects.Count)
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("SUB OBJECTS", (object) level.ObjectManager.ActiveObjects.Count<ActiveObject>((Func<ActiveObject, bool>) (x => x.IsSubObject))),
          new KeyValuePair<string, object>("RESPAWN PREVENTED OBJECTS", (object) "")
        }
      };
    }
  }

  public class WaterInfo(DebugContext context) : InformationDebugOption(context, "WATER", "")
  {
    protected override IEnumerable<IEnumerable<KeyValuePair<string, object>>> GetInformation()
    {
      WaterManager waterManager = this.Context.Level.WaterManager;
      return (IEnumerable<IEnumerable<KeyValuePair<string, object>>>) new KeyValuePair<string, object>[4][]
      {
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("TOTAL AREAS", (object) waterManager.WaterAreas.Count),
          new KeyValuePair<string, object>("", (object) "")
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("HUE", (object) $"{waterManager.HueTarget * 360.0:0} DEG"),
          new KeyValuePair<string, object>("HUE CONCENTRATION", (object) waterManager.HueAmount)
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("SATURATION ADJUSTMENT", (object) waterManager.SaturationChange),
          new KeyValuePair<string, object>("LUMINOSITY ADJUSTMENT", (object) waterManager.LuminosityChange)
        },
        new KeyValuePair<string, object>[2]
        {
          new KeyValuePair<string, object>("RIPPLE SIZE", (object) waterManager.WaveSize),
          new KeyValuePair<string, object>("RIPPLE PHASE", (object) $"{MathX.ToDegrees(waterManager.WavePhase):0} DEG")
        }
      };
    }
  }

  public class WaterVisible(DebugContext context) : DiscreteDebugOption<bool>(context, "WATER", "", "VISIBLE", (IEnumerable<KeyValuePair<string, bool>>) new KeyValuePair<string, bool>[2]
  {
    new KeyValuePair<string, bool>("YES", true),
    new KeyValuePair<string, bool>("NO", false)
  })
  {
    public override void OnChange()
    {
      this.Context.Level.LayerViewOptions.ShowWater = this.SelectedValue;
    }
  }
}
