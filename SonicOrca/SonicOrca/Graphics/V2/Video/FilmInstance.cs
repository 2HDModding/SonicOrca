// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Video.FilmInstance
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Resources;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Graphics.V2.Video
{

    public class FilmInstance
    {
      private readonly FilmGroup _filmGroup;
      private Film _film;
      private bool _playing;
      private bool _finished;
      private ITexture _renderTarget;

      public FilmGroup FilmGroup => this._filmGroup;

      public bool Playing => this._playing;

      public bool Finished => this._finished;

      public double CurrentTime
      {
        get
        {
          IFilmBuffer filmBuffer = ((IEnumerable<IFilmBuffer>) this._filmGroup.FilmBuffers).FirstOrDefault<IFilmBuffer>();
          return filmBuffer != null ? filmBuffer.CurrentTime : 0.0;
        }
      }

      public double Duration
      {
        get
        {
          IFilmBuffer filmBuffer = ((IEnumerable<IFilmBuffer>) this._filmGroup.FilmBuffers).FirstOrDefault<IFilmBuffer>();
          return filmBuffer != null ? filmBuffer.Duration : 0.0;
        }
      }

      public FilmInstance(ResourceTree resourceTree, string resourceKey)
      {
        this._filmGroup = resourceTree.GetLoadedResource<FilmGroup>(resourceKey);
        this._film = this._filmGroup.First<Film>();
        this.SetupRenderTarget();
      }

      public FilmInstance(FilmGroup filmGroup)
      {
        this._filmGroup = filmGroup;
        this._film = this._filmGroup.First<Film>();
        this.SetupRenderTarget();
      }

      private void SetupRenderTarget()
      {
        IFilmBuffer filmBuffer = ((IEnumerable<IFilmBuffer>) this._filmGroup.FilmBuffers).First<IFilmBuffer>();
        this._renderTarget = SonicOrcaGameContext.Singleton.Window.GraphicsContext.CreateTexture(filmBuffer.Width, filmBuffer.Height, 4, new byte[filmBuffer.Width * filmBuffer.Height * 4]);
      }

      public void Animate()
      {
        IFilmBuffer filmBuffer = ((IEnumerable<IFilmBuffer>) this._filmGroup.FilmBuffers).First<IFilmBuffer>();
        filmBuffer.Decode();
        if (filmBuffer.CurrentTime < filmBuffer.Duration)
          return;
        this._finished = true;
      }

      public void Seek(int ticks)
      {
      }

      public void Draw(Renderer renderer, Vector2 offset = default (Vector2), bool flipX = false, bool flipY = false)
      {
        this.Draw(renderer, Colours.White, offset, flipX, flipY);
      }

      public void Draw(Renderer renderer, Colour colour, Vector2 offset = default (Vector2), bool flipX = false, bool flipY = false)
      {
        IFilmBuffer filmBuffer = ((IEnumerable<IFilmBuffer>) this._filmGroup.FilmBuffers).First<IFilmBuffer>();
        Rectanglei source = new Rectanglei(0, 0, filmBuffer.Width, filmBuffer.Height);
        Rectanglei destination = new Rectanglei(0, 0, filmBuffer.Width, filmBuffer.Height);
        this.Draw(renderer.Get2dRenderer(), colour, (Rectangle) source, (Rectangle) destination, flipX, flipY);
      }

      public void Draw(
        I2dRenderer renderer,
        Colour colour,
        Rectangle source,
        Rectangle destination,
        bool flipX = false,
        bool flipY = false)
      {
        IFilmBuffer filmBuffer = ((IEnumerable<IFilmBuffer>) this._filmGroup.FilmBuffers).First<IFilmBuffer>();
        byte[] argbData = filmBuffer.GetArgbData();
        this._renderTarget.SetArgbData(filmBuffer.Width, filmBuffer.Height, argbData);
        using (renderer.BeginMatixState())
        {
          Matrix4 matrix4 = Matrix4.Identity * Matrix4.CreateScale(flipX ? -1.0 : 1.0, flipY ? -1.0 : 1.0);
          renderer.ModelMatrix = matrix4;
          renderer.Colour = Colours.White;
          renderer.BlendMode = BlendMode.Opaque;
          renderer.RenderTexture(this._renderTarget, source, destination, flipX, flipY);
        }
      }
    }
}
