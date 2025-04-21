// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.WaterManager
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core
{

    public class WaterManager
    {
      private readonly SonicOrcaGameContext _gameContext;
      private readonly Level _level;
      private readonly List<Rectanglei> _waterAreas = new List<Rectanglei>();
      public static float offsetX;
      public static float offsetY;
      public static float viewportWaterLevel;
      public static ITexture waveTexture;
      private AnimationGroup wavesAnimationGroup;
      private int _waveFrame;
      private float _waterTime;
      private bool _loaded;
      private readonly List<Splash> _splashes = new List<Splash>();

      public Level Level => this._level;

      public bool Enabled { get; set; }

      public IList<Rectanglei> WaterAreas => (IList<Rectanglei>) this._waterAreas;

      public double HueTarget { get; set; }

      public double HueAmount { get; set; }

      public double SaturationChange { get; set; }

      public double LuminosityChange { get; set; }

      public double WavePhase { get; set; }

      public double NumWaves { get; set; }

      public double WaveSize { get; set; }

      public string SurfaceResourceKey { get; set; }

      public double SurfaceOffsetY { get; set; }

      internal AnimationGroup SpashEnterAnimationGroup { get; set; }

      internal AnimationGroup SpashExitAnimationGroup { get; set; }

      public WaterManager(Level level)
      {
        this._gameContext = level.GameContext;
        this._level = level;
        this.HueTarget = 0.45;
        this.HueAmount = 0.4;
        this.SaturationChange = -0.5;
        this.LuminosityChange = 0.0;
        this.NumWaves = 1.5;
        this.WaveSize = 4.0;
      }

      public void Load()
      {
        if (!this.Enabled)
          return;
        ResourceTree resourceTree = this._level.GameContext.ResourceTree;
        this.SpashEnterAnimationGroup = resourceTree.GetLoadedResource<AnimationGroup>("SONICORCA/PARTICLE/SPLASH/ENTER");
        this.SpashExitAnimationGroup = resourceTree.GetLoadedResource<AnimationGroup>("SONICORCA/PARTICLE/SPLASH/EXIT");
        WaterManager.waveTexture = resourceTree.GetLoadedResource<ITexture>("SONICORCA/OBJECTS/WAVE/TEXTURE");
        this.wavesAnimationGroup = resourceTree.GetLoadedResource<AnimationGroup>("SONICORCA/OBJECTS/WAVES/ANIGROUP");
        this._gameContext.Renderer.GetWaterRenderer();
        this._loaded = true;
      }

      public void Update()
      {
        if (!this.Enabled || !this._loaded)
          return;
        foreach (Splash splash in this._splashes)
          splash.Animate();
        this._splashes.RemoveAll((Predicate<Splash>) (x => x.Finished));
      }

      public void Animate()
      {
        if (!this.Enabled || !this._loaded)
          return;
        ++this._waveFrame;
        this._waterTime += 0.0166f;
        this.WavePhase = 0.0 * MathX.WrapRadians(this.WavePhase + 1.0 / 32.0);
      }

      public bool IsUnderwater(Vector2i position)
      {
        return this._waterAreas.Any<Rectanglei>((Func<Rectanglei, bool>) (x => x.Contains(position)));
      }

      public void CreateSplash(LevelLayer layer, SplashType splashType, Vector2i position)
      {
        this._splashes.Add(new Splash(this, splashType, position));
      }

      public void CreateBubble(int layer, Vector2i position, int size)
      {
        this._level.ObjectManager.AddObject(new ObjectPlacement(this._level.CommonResources.GetResourcePath("bubbleobject"), layer, position, (object) new
        {
          Size = size
        }));
      }

      public void Draw(Renderer renderer, Viewport viewport)
      {
        if (!this.Enabled || !this._loaded)
          return;
        this.DrawSplashes(renderer, viewport);
        foreach (Rectanglei waterArea1 in this._waterAreas)
        {
          Rectangle waterArea2 = (Rectangle) waterArea1;
          this.Draw(renderer, viewport, (Rectanglei) waterArea2);
        }
      }

      public void DrawSplashes(Renderer renderer, Viewport viewport)
      {
        if (this._splashes.Count <= 0)
          return;
        using (viewport.ApplyRendererState(renderer))
        {
          I2dRenderer renderer1 = renderer.Get2dRenderer();
          I2dRenderer obj = renderer1;
          Matrix4 modelMatrix = renderer1.ModelMatrix;
          ref Matrix4 local = ref modelMatrix;
          Rectanglei bounds = viewport.Bounds;
          double x = (double) -bounds.X;
          bounds = viewport.Bounds;
          double y = (double) -bounds.Y;
          Matrix4 matrix4 = local.Translate(x, y);
          obj.ModelMatrix = matrix4;
          foreach (Splash splash in this._splashes)
            splash.Draw(renderer1);
        }
        renderer.DeativateRenderer();
      }

      public void Draw(Renderer renderer, Viewport viewport, Rectanglei waterArea)
      {
        WaterManager.waveTexture = this.wavesAnimationGroup.Textures[this._waveFrame / 2 % 32 /*0x20*/];
        Rectanglei bounds1 = viewport.Bounds;
        bounds1.Left = Math.Max(bounds1.Left, waterArea.Left);
        bounds1.Top = Math.Max(bounds1.Top, waterArea.Top);
        bounds1.Right = Math.Min(bounds1.Right, waterArea.Right);
        bounds1.Bottom = Math.Min(bounds1.Bottom, waterArea.Bottom);
        bounds1.X -= viewport.Bounds.X;
        bounds1.Y -= viewport.Bounds.Y;
        Vector2 scale = viewport.Scale;
        bounds1.X = (int) ((double) bounds1.X * scale.X);
        bounds1.Y = (int) ((double) bounds1.Y * scale.Y);
        bounds1.Width = (int) ((double) bounds1.Width * scale.X);
        bounds1.Height = (int) ((double) bounds1.Height * scale.Y);
        if (bounds1.Width < 0 || (double) bounds1.Height < -(this.WavePhase + (double) waterArea.Y / 1080.0 * (2.0 * Math.PI)))
          return;
        IWaterRenderer waterRenderer = renderer.GetWaterRenderer();
        waterRenderer.HueTarget = this.HueTarget;
        waterRenderer.HueAmount = this.HueAmount;
        waterRenderer.SaturationChange = this.SaturationChange;
        waterRenderer.LuminosityChange = this.LuminosityChange;
        waterRenderer.WavePhase = 0.0;
        waterRenderer.NumWaves = this.NumWaves / scale.Y;
        waterRenderer.WaveSize = this.WaveSize * scale.X;
        waterRenderer.Time = this._waterTime;
        Rectanglei bounds2 = viewport.Bounds;
        WaterManager.offsetX = (float) bounds2.Left;
        bounds2 = viewport.Bounds;
        WaterManager.offsetY = (float) bounds2.Top;
        WaterManager.viewportWaterLevel = (float) bounds1.Y;
        waterRenderer.Render(bounds1);
      }
    }
}
