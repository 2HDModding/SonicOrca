// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.BossObject
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using SonicOrca.Geometry;
using SonicOrca.Graphics;

namespace SonicOrca.Core.Objects.Base
{

    public class BossObject : Enemy
    {
      public static readonly Colour FlashAdditiveColour = new Colour((byte) 64 /*0x40*/, (byte) 64 /*0x40*/, (byte) 64 /*0x40*/);
      private int _explosionTimer;

      protected string ExplosionResourceKey { get; set; }

      protected string HitSoundResourceKey { get; set; }

      protected int Health { get; set; }

      protected int InvincibilityTimer { get; set; }

      public bool Defeated { get; set; }

      public bool Fleeing { get; set; }

      protected bool IsInvincibleFlashing
      {
        get => this.InvincibilityTimer > 0 && this.InvincibilityTimer % 2 == 0;
      }

      public BossObject()
      {
        this.ExplosionResourceKey = "SONICORCA/OBJECTS/BOSS/EXPLOSION";
        this.HitSoundResourceKey = "SONICORCA/SOUND/BOSSHIT";
        this.Health = 8;
      }

      protected override void OnUpdate()
      {
        base.OnUpdate();
        if (this.InvincibilityTimer <= 0)
          return;
        --this.InvincibilityTimer;
      }

      protected override void OnCollision(CollisionEvent e)
      {
        if (e.ActiveObject.Type.Classification == ObjectClassification.Character)
        {
          ICharacter activeObject = (ICharacter) e.ActiveObject;
          if (activeObject.IsDeadly)
          {
            if (this.InvincibilityTimer > 0)
              return;
            this.Hit(activeObject);
            return;
          }
        }
        base.OnCollision(e);
      }

      protected virtual void Hit(ICharacter character)
      {
        if (character.IsAirborne)
          character.Velocity *= -1.0;
        else
          character.GroundVelocity *= -1.0;
        --this.Health;
        this.InvincibilityTimer = 32 /*0x20*/;
        if (this.Health > 0)
          this.Level.SoundManager.PlaySound((IActiveObject) this, this.HitSoundResourceKey);
        else
          this.Defeat();
      }

      protected virtual void Defeat()
      {
      }

      protected void UpdateExplosions(int radius)
      {
        if (this._explosionTimer-- > 0)
          return;
        this._explosionTimer = 8;
        this.ExplodeAt(this.Position + new Vector2i(this.Level.Random.Next(-radius, radius), this.Level.Random.Next(-radius, radius)));
      }

      protected void ExplodeAt(Vector2i position)
      {
        this.Level.ObjectManager.AddObject(new ObjectPlacement(this.ExplosionResourceKey, this.Level.Map.Layers.IndexOf(this.Layer), position));
      }
    }
}
