// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.ICharacter
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using SonicOrca.Core.Objects.Base;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System.Collections.Generic;

namespace SonicOrca.Core.Objects
{

    public interface ICharacter : IActiveObject
    {
      CharacterSpecialState SpecialState { get; set; }

      bool Jumped { get; set; }

      bool IsJumping { get; set; }

      bool IsObjectControlled { get; set; }

      bool ShouldReactToLevel { get; }

      IReadOnlyList<CharacterHistoryItem> History { get; }

      ActiveObject ObjectLink { get; set; }

      object ObjectTag { get; set; }

      int Path { get; set; }

      CharacterState StateFlags { get; set; }

      AnimationInstance Animation { get; }

      Player Player { get; set; }

      bool IsSidekick { get; set; }

      Vector2 Velocity { get; set; }

      double GroundVelocity { get; set; }

      CharacterLookDirection LookDirection { get; set; }

      double ShowAngle { get; set; }

      int LookDelay { get; set; }

      CharacterInputState Input { get; set; }

      int Facing { get; set; }

      bool IsAirborne { get; set; }

      bool IsDeadly { get; }

      bool IsSpinball { get; set; }

      bool IsUnderwater { get; set; }

      bool IsRollJumping { get; set; }

      bool IsHurt { get; set; }

      int BreathTicks { get; set; }

      BarrierType Barrier { get; set; }

      bool HasBarrier { get; }

      bool ForceSpinball { get; set; }

      void Hurt(int direction, PlayerDeathCause cause = PlayerDeathCause.Hurt);

      bool IsCharging { get; }

      bool Respawning { get; }

      bool ExhibitsVirtualPlatform { get; set; }

      bool IsDebug { get; set; }

      double TumbleAngle { get; set; }

      int TumbleTurns { get; set; }

      bool HasSpeedShoes { get; set; }

      bool IsInvincible { get; set; }

      int SlopeLockTicks { get; set; }

      bool IsPushing { get; set; }

      bool IsDead { get; set; }

      bool IsDying { get; set; }

      void Kill(PlayerDeathCause cause);

      void InhaleOxygen();

      bool IsWinning { get; set; }

      CameraProperties CameraProperties { get; set; }

      bool CheckCollision { get; set; }

      bool CheckLandscapeCollision { get; set; }

      bool CheckObjectCollision { get; set; }

      int LedgeSensorRadius { get; }

      Vector2i NormalCollisionRadius { get; }

      Vector2i SpinballCollisionRadius { get; }

      Vector2i CollisionRadius { get; }

      void LeaveGround();

      CollisionVector GroundVector { get; }

      CollisionMode Mode { get; set; }

      bool CanBeHurt { get; }
    }
}
