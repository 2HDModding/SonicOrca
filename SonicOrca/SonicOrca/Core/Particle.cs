// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Particle
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;

namespace SonicOrca.Core
{

    public class Particle
    {
      public ParticleType Type { get; set; }

      public LevelLayer Layer { get; set; }

      public Vector2 Position { get; set; }

      public Vector2 Velocity { get; set; }

      public int Time { get; set; }

      public double Angle { get; set; }

      public double AngularVelocity { get; set; }

      public double Size { get; set; }

      public ITexture CustomTexture { get; set; }

      public Particle() => this.Size = 1.0;

      public void Update()
      {
        this.Position += this.Velocity;
        this.Angle = MathX.WrapRadians(this.Angle + this.AngularVelocity);
        --this.Time;
      }

      public override string ToString()
      {
        return $"{this.Time} particle, Time = {this.Type} Position = {this.Position} Velocity = {this.Velocity}";
      }
    }
}
