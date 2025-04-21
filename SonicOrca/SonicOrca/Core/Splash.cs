// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Splash
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System.Linq;

namespace SonicOrca.Core
{

    internal class Splash
    {
      private readonly WaterManager _waterManager;
      private readonly AnimationInstance _animation;
      private Vector2i _position;

      public bool Finished { get; private set; }

      public Splash(WaterManager waterManager, SplashType type, Vector2i position)
      {
        this._waterManager = waterManager;
        this._animation = type != SplashType.Enter ? new AnimationInstance(waterManager.SpashExitAnimationGroup) : new AnimationInstance(waterManager.SpashEnterAnimationGroup);
        this._position = position;
      }

      public void Animate()
      {
        if (this._waterManager.WaterAreas.Count == 0)
        {
          this.Finished = true;
        }
        else
        {
          this._position.Y = this._waterManager.WaterAreas.First<Rectanglei>().Top;
          this._animation.Animate();
          if (this._animation.Cycles <= 0)
            return;
          this.Finished = true;
        }
      }

      public void Draw(I2dRenderer renderer)
      {
        Vector2i position = this._position + new Vector2i(0, -this._animation.CurrentFrame.Source.Height / 2);
        this._animation.Draw(renderer, (Vector2) position);
      }
    }
}
