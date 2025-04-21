// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ParticleManager
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    public class ParticleManager : IDisposable
    {
      private const string HeatParticleTextureResourceKey = "SONICORCA/PARTICLE/HEAT";
      private const string SmokeParticleTextureResourceKey = "SONICORCA/PARTICLE/SMOKE";
      private readonly SonicOrcaGameContext _gameContext;
      private readonly Level _level;
      private readonly Random _random = new Random();
      private readonly List<Particle> _particles = new List<Particle>();
      private readonly ResourceSession _resourceSession;
      private ITexture _heatParticleTexture;
      private ITexture _smokeParticleTexture;
      private IFramebuffer _workCanvas;
      private bool _enabled;

      public Random Random => this._random;

      public ParticleManager(Level level)
      {
        this._gameContext = level.GameContext;
        this._level = level;
        this._resourceSession = new ResourceSession(this._gameContext.ResourceTree);
        this._workCanvas = this._gameContext.Window.GraphicsContext.CreateFrameBuffer(128 /*0x80*/, 128 /*0x80*/);
        this._enabled = this._gameContext.Configuration.GetPropertyBoolean("graphics", "particles", true);
      }

      public async Task LoadAsync(CancellationToken ct = default (CancellationToken))
      {
        ResourceTree resourceTree = this._gameContext.ResourceTree;
        this._resourceSession.PushDependencies("SONICORCA/PARTICLE/HEAT", "SONICORCA/PARTICLE/SMOKE");
        await this._resourceSession.LoadAsync();
        this._heatParticleTexture = resourceTree.GetLoadedResource<ITexture>("SONICORCA/PARTICLE/HEAT");
        this._smokeParticleTexture = resourceTree.GetLoadedResource<ITexture>("SONICORCA/PARTICLE/SMOKE");
      }

      public void Dispose()
      {
        this._resourceSession.Dispose();
        if (this._workCanvas == null)
          return;
        this._workCanvas.Dispose();
      }

      public int GetNumParticlesOnLayer(LevelLayer layer)
      {
        return this._particles.Count<Particle>((Func<Particle, bool>) (x => x.Layer == layer));
      }

      public void Add(Particle p)
      {
        if (!this._enabled)
          return;
        this._particles.Add(p);
      }

      public void Clear() => this._particles.Clear();

      public void Update()
      {
        if (!this._enabled)
          return;
        foreach (Particle particle in this._particles)
          particle.Update();
        this._particles.RemoveAll((Predicate<Particle>) (x => x.Time <= 0));
      }

      public void Draw(
        Renderer renderer,
        Viewport viewport,
        LevelLayer layer,
        IVideoSettings videoSettings)
      {
        if (!this._enabled || this._particles.Count == 0)
          return;
        IFramebuffer currentFramebuffer = renderer.Window.GraphicsContext.CurrentFramebuffer;
        I2dRenderer obj = renderer.Get2dRenderer();
        IHeatRenderer heatRenderer = renderer.GetHeatRenderer();
        heatRenderer.DistortionTexture = this._heatParticleTexture;
        Matrix4 modelMatrix = obj.ModelMatrix;
        Rectanglei destination;
        foreach (Particle particle in this._particles)
        {
          if (particle.Layer == layer)
          {
            switch (particle.Type)
            {
              case ParticleType.Heat:
                if (videoSettings.EnableHeatEffects)
                {
                  renderer.DeativateRenderer();
                  destination = (Rectanglei) new Rectangle(particle.Position.X - (double) (this._heatParticleTexture.Width / 2), particle.Position.Y - (double) (this._heatParticleTexture.Height / 2), (double) this._heatParticleTexture.Width, (double) this._heatParticleTexture.Height);
                  destination = destination.OffsetBy(viewport.Bounds.TopLeft * -1);
                  if (destination.IntersectsWith(new Rectanglei(0, 0, currentFramebuffer.Width, currentFramebuffer.Height)))
                  {
                    heatRenderer.DistortionAmount = 1.0 / (double) this._heatParticleTexture.Width * 8.0;
                    if (particle.Time < 60)
                      heatRenderer.DistortionAmount *= (double) particle.Time / 60.0;
                    this._workCanvas.Activate();
                    obj.BlendMode = BlendMode.Opaque;
                    obj.Colour = Colours.White;
                    obj.ClipRectangle = new Rectangle(0.0, 0.0, 1920.0, 1080.0);
                    obj.ModelMatrix = Matrix4.Identity;
                    obj.RenderTexture(currentFramebuffer.Textures[0], (Rectangle) new Rectanglei(destination.X, 1080 - destination.Bottom, destination.Width, destination.Height), (Rectangle) new Rectanglei(0, 0, destination.Width, destination.Height));
                    obj.Deactivate();
                    currentFramebuffer.Activate();
                    heatRenderer.Render(this._workCanvas.Textures[0], new Rectanglei(0, 0, destination.Width, destination.Height), destination);
                    continue;
                  }
                  continue;
                }
                continue;
              case ParticleType.Smoke:
                Vector2 vector2_1 = particle.Position - (Vector2) viewport.Bounds.TopLeft;
                destination = (Rectanglei) new Rectangle(vector2_1.X - (double) (this._smokeParticleTexture.Width / 2), vector2_1.Y - (double) (this._smokeParticleTexture.Height / 2), (double) this._smokeParticleTexture.Width, (double) this._smokeParticleTexture.Height);
                if (destination.IntersectsWith(new Rectanglei(0, 0, currentFramebuffer.Width, currentFramebuffer.Height)))
                {
                  double a = (particle.Time < 120 ? (double) particle.Time / 120.0 : 1.0) * 0.2;
                  currentFramebuffer.Activate();
                  obj.BlendMode = BlendMode.Alpha;
                  obj.Colour = new Colour(a, 1.0, 1.0, 1.0);
                  obj.ClipRectangle = new Rectangle(0.0, 0.0, 1920.0, 1080.0);
                  obj.ModelMatrix = Matrix4.CreateTranslation(vector2_1.X, vector2_1.Y) * Matrix4.CreateRotationZ(particle.Angle);
                  obj.RenderTexture(this._smokeParticleTexture, new Vector2());
                  continue;
                }
                continue;
              case ParticleType.Custom:
                ITexture customTexture = particle.CustomTexture;
                if (customTexture != null)
                {
                  Vector2 vector2_2 = particle.Position - (Vector2) viewport.Bounds.TopLeft;
                  destination = (Rectanglei) new Rectangle(vector2_2.X - (double) (customTexture.Width / 2), vector2_2.Y - (double) (customTexture.Height / 2), (double) customTexture.Width, (double) customTexture.Height);
                  if (destination.IntersectsWith(new Rectanglei(0, 0, currentFramebuffer.Width, currentFramebuffer.Height)))
                  {
                    currentFramebuffer.Activate();
                    obj.BlendMode = BlendMode.Alpha;
                    obj.Colour = new Colour(1.0, 1.0, 1.0, 1.0);
                    obj.ClipRectangle = new Rectangle(0.0, 0.0, 1920.0, 1080.0);
                    obj.ModelMatrix = Matrix4.CreateTranslation(vector2_2.X, vector2_2.Y) * Matrix4.CreateRotationZ(particle.Angle) * Matrix4.CreateScale(particle.Size, particle.Size);
                    obj.RenderTexture(customTexture, new Vector2());
                    continue;
                  }
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
        renderer.DeativateRenderer();
        obj.ModelMatrix = modelMatrix;
      }
    }
}
