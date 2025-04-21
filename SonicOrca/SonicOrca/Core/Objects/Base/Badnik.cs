// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.Badnik
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using SonicOrca.Geometry;
using System;

namespace SonicOrca.Core.Objects.Base
{

    public class Badnik : ActiveObject
    {
      public bool CanBeDestoryed { get; set; } = true;

      protected override void OnStart() => this.Priority = 128 /*0x80*/;

      protected override void OnCollision(CollisionEvent e)
      {
        if (e.ActiveObject.Type.Classification != ObjectClassification.Character)
          return;
        ICharacter activeObject = (ICharacter) e.ActiveObject;
        if (activeObject.IsDeadly && this.CanBeDestoryed)
        {
          this.Destroy(activeObject);
        }
        else
        {
          ICharacter character = activeObject;
          Vector2i position = e.ActiveObject.Position;
          int x1 = position.X;
          position = this.Position;
          int x2 = position.X;
          int direction = Math.Sign(x1 - x2);
          character.Hurt(direction);
        }
      }

      protected void Destroy(ICharacter character)
      {
        if (character.IsAirborne)
          this.Rebound(character);
        this.FinishForever();
        int points = character.Player.AwardChainedScore();
        this.CreateExplosionObject();
        this.Level.CreateRandomAnimalObject(this.Level.Map.Layers.IndexOf(this.Layer), this.Position);
        this.Level.CreateScoreObject(points, this.Position);
      }

      protected virtual void Rebound(ICharacter character)
      {
        Vector2 velocity = character.Velocity;
        if (velocity.Y < 0.0)
          velocity.Y += 4.0;
        else if (character.Position.Y >= this.Position.Y)
          velocity.Y -= 4.0;
        else
          velocity.Y *= -1.0;
        character.Velocity = velocity;
      }

      protected void CreateExplosionObject()
      {
        this.Level.ObjectManager.AddObject(new ObjectPlacement(this.Level.CommonResources.GetResourcePath("badnikexplosionobject"), this.Level.Map.Layers.IndexOf(this.Layer), this.Position));
      }
    }
}
