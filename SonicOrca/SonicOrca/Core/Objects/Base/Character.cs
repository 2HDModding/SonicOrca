// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.Character
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using SonicOrca.Core.Debugging;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core.Objects.Base
{

    public abstract class Character : ActiveObject, ICharacter, IActiveObject
    {
      private BarrierType _barrierTypeAnimation;
      public const double TumbleSpeed = 0.0234375;
      private const int KeepSpinballAirborneTolerance = 11;
      private readonly bool[] _collisionDetected = new bool[4];
      private readonly CollisionEvent[] _collisionEvents = new CollisionEvent[4];
      private readonly bool[] _collisionDetectedTiles = new bool[4];
      private readonly CollisionEvent[] _collisionEventsTiles = new CollisionEvent[4];
      private readonly List<KeyValuePair<ActiveObject, CollisionEvent>> _collidedVectors = new List<KeyValuePair<ActiveObject, CollisionEvent>>();
      private CollisionVector _originalGroundVector;
      private CollisionVector _overlappingGroundVector;
      private KeyValuePair<ActiveObject, CollisionEvent> _overlappingCollision;
      private Vector2 _lastPositionAdjusted;
      private CollisionVector _lastLeftVector;
      private CollisionVector _lastRightVector;
      private const string DefaultBarrierClassicResourceKey = "SONICORCA/OBJECTS/BARRIER/ANIGROUP";
      private const string DefaultBarrierBubbleResourceKey = "SONICORCA/OBJECTS/BARRIER/BUBBLE/ANIGROUP";
      private const string DefaultBarrierFireResourceKey = "SONICORCA/OBJECTS/BARRIER/FIRE/ANIGROUP";
      private const string DefaultBarrierLightningResourceKey = "SONICORCA/OBJECTS/BARRIER/LIGHTNING/ANIGROUP";
      private const string DefaultSpindashDustResourceKey = "SONICORCA/OBJECTS/SPINDASH/ANIGROUP";
      private const string DefaultBrakeDustResourceKey = "SONICORCA/OBJECTS/DUST";
      private const string DefaultInvincibilityResourceKey = "SONICORCA/OBJECTS/INVINCIBILITY/ANIGROUP";
      private const int MaximumVelocity = 96 /*0x60*/;
      private readonly CharacterHistoryItem[] _history = new CharacterHistoryItem[32 /*0x20*/];
      private CharacterBalanceDirection _balanceDirection;
      private int _brakeDustDelay;
      private int _brakeTicks;
      private int _pushDirection;
      private int _pushTicks;
      private double _spindashExtraSpeed;
      private Vector2 _intersection;
      private int _dyingTicks;
      private PlayerDeathCause _deathCause;
      private static readonly IReadOnlyList<Vector2> ScatterRingOffsets = (IReadOnlyList<Vector2>) new Vector2[32 /*0x20*/]
      {
        new Vector2(-3.12, -15.69),
        new Vector2(3.12, -15.69),
        new Vector2(-8.89, -13.3),
        new Vector2(8.89, -13.3),
        new Vector2(-13.3, -8.89),
        new Vector2(13.3, -8.89),
        new Vector2(-15.69, -3.12),
        new Vector2(15.69, -3.12),
        new Vector2(-15.69, 3.12),
        new Vector2(15.69, 3.12),
        new Vector2(-13.3, 8.89),
        new Vector2(13.3, 8.89),
        new Vector2(-8.89, 13.3),
        new Vector2(8.89, 13.3),
        new Vector2(-3.12, 15.69),
        new Vector2(3.12, 15.69),
        new Vector2(-1.56, -7.85),
        new Vector2(1.56, -7.85),
        new Vector2(-4.44, -6.65),
        new Vector2(4.44, -6.65),
        new Vector2(-6.65, -4.44),
        new Vector2(6.65, -4.44),
        new Vector2(-7.85, -1.56),
        new Vector2(7.85, -1.56),
        new Vector2(-7.85, 1.56),
        new Vector2(7.85, 1.56),
        new Vector2(-6.65, 4.44),
        new Vector2(6.65, 4.44),
        new Vector2(-4.44, 6.65),
        new Vector2(4.44, 6.65),
        new Vector2(-1.56, 7.85),
        new Vector2(1.56, 7.85)
      };
      private readonly List<Character.InvincibilityParticle> _invincibilityParticles = new List<Character.InvincibilityParticle>();
      private readonly List<Vector2> _invincibilityCharacterPositionHistory = new List<Vector2>();
      private const int InvincibilityParticlesPerPositionHistory = 3;
      private const int InvincibilityPositionHistoryChunks = 4;
      private const int InvincibilityPositionHistoryDelay = 2;
      public const int NoHumanInputTime = 600;
      public const int SidekickOffscreenTime = 300;
      private bool _humanControlled;
      private int _humanInputTicksRemaining;
      private Character.AutoSidekickState _autoSidekickState;
      private int _sidekickOffscreenTicks;
      private Vector2i _sidekickTargetPosition;
      private bool _autoSidekickJumping;
      private const string DrowningCountDownResourceKey = "SONICORCA/HUD/DROWNING/ANIGROUP";
      private const string InhaleSoundResourceKey = "SONICORCA/SOUND/INHALE";
      private const string SplashSoundResourceKey = "SONICORCA/SOUND/SPLASH";
      private const string DrownWarningSoundResourceKey = "SONICORCA/SOUND/DROWNWARNING";
      private const string DrownSoundResourceKey = "SONICORCA/SOUND/DROWN";
      private static readonly IReadOnlyCollection<int> BreathLeftWarnings = (IReadOnlyCollection<int>) new int[3]
      {
        1500,
        1200,
        900
      };
      private const int BreathLeftDrowningMusic = 720;
      private const int MaximumBreath = 1800;
      private AnimationInstance _drowningAnimation;
      private int _nextBubbleTime;
      private bool _drowningClimax;
      private int _inhalingBubble;
      private Vector2i _drownCountdownPosition;
      private int _drownCountdownValue;

      protected string AnimationGroupResourceKey { get; set; }

      protected string BrakeDustResourceKey { get; set; }

      protected string SpindashDustGroupResourceKey { get; set; }

      public AnimationInstance Animation { get; set; }

      protected AnimationInstance BarrierAnimation { get; set; }

      protected AnimationInstance SpindashDustAnimation { get; set; }

      public double ShowAngle { get; set; }

      public double TumbleAngle { get; set; }

      public int TumbleTurns { get; set; }

      protected bool DrawBodyRotated { get; set; }

      protected override void OnAnimate()
      {
        this.DrawBodyRotated = false;
        this.Animation.OverrideDelay = new int?();
        if (this.IsDead)
          return;
        if (this.SpecialState == CharacterSpecialState.Grabbed)
        {
          this.Animation.Index = 24;
          this.Animation.Animate();
        }
        else
          this.AnimationNormalState();
        if (!this._drowningClimax)
          return;
        this._drowningAnimation.Animate();
      }

      private void AnimationNormalState()
      {
        if (this.IsDying)
        {
          this.Animation.Index = this._deathCause != PlayerDeathCause.Drown ? 15 : 20;
        }
        else
        {
          if (this.IsDebug)
          {
            this.Animation.Index = 10;
            return;
          }
          if (this.IsHurt)
            this.Animation.Index = 14;
          else if (this.IsWinning)
            this.Animation.Index = 19;
          else if (this.IsFlying)
            this.Animation.Index = 17;
          else if (this.IsCharging)
            this.Animation.Index = 11;
          else if (this.IsSpinball)
          {
            this.Animation.Index = 10;
            this.Animation.OverrideDelay = new int?((int) (Math.Max(0.0, 20.0 - Math.Abs(this.GroundVelocity)) / 5.0));
          }
          else if (this.Animation.Index != 12 && this.Animation.Index != 13 || !this.IsAirborne)
          {
            if (this.LookDirection == CharacterLookDirection.Up)
              this.Animation.Index = 1;
            else if (this.LookDirection == CharacterLookDirection.Ducking)
              this.Animation.Index = 2;
            else if (this.IsPushing)
            {
              this.Animation.Index = 6;
              this.Animation.OverrideDelay = new int?(8);
              this.DrawBodyRotated = true;
            }
            else if (this.IsBraking)
            {
              this.Animation.Index = 9;
              this.DrawBodyRotated = true;
            }
            else if (this.GroundVelocity == 0.0)
            {
              switch (Math.Abs((int) this._balanceDirection))
              {
                case 1:
                  this.Animation.Index = this.IsFacingLeft && this._balanceDirection > CharacterBalanceDirection.None || this.IsFacingRight && this._balanceDirection < CharacterBalanceDirection.None ? 5 : 4;
                  break;
                case 2:
                  this.Animation.Index = 3;
                  break;
                default:
                  if (this.IsAirborne)
                  {
                    this.Animation.Index = 13;
                    break;
                  }
                  if (this.Animation.Index == 19)
                  {
                    this.Animation.Index = 26;
                    break;
                  }
                  if (this.Animation.Index != 26)
                  {
                    this.Animation.Index = 0;
                    break;
                  }
                  break;
              }
            }
            else if (this._inhalingBubble != 0)
            {
              if (this._inhalingBubble == 1)
              {
                this._inhalingBubble = 2;
                this.Animation.Index = 18;
              }
              else if (this.Animation.Index != 18)
                this._inhalingBubble = 0;
            }
            else
            {
              bool flag1 = true;
              bool flag2 = false;
              bool flag3 = false;
              if (this.Player.ProtagonistCharacterType == CharacterType.Sonic && this == this.Player.Protagonist)
              {
                flag2 = true;
                flag3 = true;
              }
              if (Math.Abs(this.GroundVelocity) < 24.0)
              {
                if (flag3)
                {
                  if (this.Animation.Index == 0 && this.Animation.CurrentFrameIndex >= 248)
                    this.Animation.Index = 27;
                  else if (this.Animation.Index == 27)
                    this.GroundVelocity = 0.0;
                  else
                    this.Animation.Index = 7;
                }
                else
                  this.Animation.Index = 7;
              }
              else if (Math.Abs(this.GroundVelocity) >= 48.0)
              {
                this.Animation.Index = !flag2 ? 8 : 28;
                flag1 = false;
              }
              else
                this.Animation.Index = !flag2 ? 8 : (this.Animation.Index != 28 ? 8 : 29);
              if (flag1)
                this.Animation.OverrideDelay = new int?((int) (Math.Max(0.0, 32.0 - Math.Abs(this.GroundVelocity)) / 8.0));
              this.DrawBodyRotated = true;
            }
          }
        }
        this.UpdateRotation();
        if (!this.IsSpinball && this.TumbleAngle != 0.0 && !this.IsHurt && !this.IsDying)
        {
          this.Animation.Index = 16 /*0x10*/;
          double num1 = MathX.Clamp(this.TumbleAngle, 1.0);
          double num2 = num1 >= 0.0 ? num1 / 2.0 : (1.0 - Math.Abs(num1)) / 2.0 + 0.5;
          if (this.IsFacingLeft)
            num2 = MathX.Wrap(1.0 - num2 + 0.5, 1.0);
          this.Animation.CurrentFrameIndex = (int) ((double) (((IReadOnlyCollection<SonicOrca.Graphics.Animation.Frame>) this.Animation.AnimationGroup[this.Animation.Index].Frames).Count - 1) * num2);
          this.Animation.CurrentFrameIndex = MathX.Clamp(0, this.Animation.CurrentFrameIndex, ((IReadOnlyCollection<SonicOrca.Graphics.Animation.Frame>) this.Animation.AnimationGroup[this.Animation.Index].Frames).Count - 1);
        }
        else
          this.Animation.Animate();
        if (this.HasBarrier)
        {
          if (this._barrierTypeAnimation != this.Barrier)
          {
            this._barrierTypeAnimation = this.Barrier;
            this.BarrierAnimation = new AnimationInstance(this.ResourceTree, Character.GetBarrierResourceKey(this._barrierTypeAnimation));
          }
          this.BarrierAnimation.Animate();
        }
        this.SpindashDustAnimation.Animate();
        if (!this.IsInvincible)
          return;
        this.AnimateInvincibility();
      }

      private void UpdateRotation()
      {
        if (this.Mode == CollisionMode.Air || this.GroundVector == null)
        {
          this.ShowAngle = MathX.GoTowardsWrap(this.ShowAngle, 0.0, 0.05, 0.0, 2.0 * Math.PI);
          if (this.TumbleTurns == 0)
            this.TumbleAngle = MathX.ChangeSpeed(this.TumbleAngle, -3.0 / 128.0);
          else if (this.TumbleTurns < 0)
          {
            this.TumbleAngle -= 3.0 / 128.0;
            if (this.TumbleAngle >= 0.0)
              return;
            ++this.TumbleAngle;
            ++this.TumbleTurns;
          }
          else
          {
            if (this.TumbleTurns <= 0)
              return;
            this.TumbleAngle += 3.0 / 128.0;
            if (this.TumbleAngle <= 1.0)
              return;
            this.TumbleAngle -= 2.0;
            --this.TumbleTurns;
          }
        }
        else
        {
          this.ShowAngle = MathX.LerpWrap(this.ShowAngle, this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.Rotate) ? this.GroundVector.Angle : (double) this.Mode * (-1.0 * Math.PI / 2.0), MathX.Clamp(0.25, Math.Abs(this.GroundVelocity) / 64.0, 1.0), 0.0, 2.0 * Math.PI);
          this.TumbleAngle = 0.0;
        }
      }

      private static string GetBarrierResourceKey(BarrierType type)
      {
        switch (type)
        {
          case BarrierType.Classic:
            return "SONICORCA/OBJECTS/BARRIER/ANIGROUP";
          case BarrierType.Bubble:
            return "SONICORCA/OBJECTS/BARRIER/BUBBLE/ANIGROUP";
          case BarrierType.Fire:
            return "SONICORCA/OBJECTS/BARRIER/FIRE/ANIGROUP";
          case BarrierType.Lightning:
            return "SONICORCA/OBJECTS/BARRIER/LIGHTNING/ANIGROUP";
          default:
            return (string) null;
        }
      }

      public int LedgeSensorRadius { get; set; }

      public Vector2i NormalCollisionRadius { get; set; }

      public Vector2i SpinballCollisionRadius { get; set; }

      public Vector2i RectangleCollisionRadius { get; set; }

      public Vector2i CollisionRadius { get; set; }

      public int FloorSensorRadius { get; set; }

      public int CollisionSensorSize { get; set; }

      public bool CheckCollision { get; set; }

      public bool CheckLandscapeCollision { get; set; }

      public bool CheckObjectCollision { get; set; }

      public int Path { get; set; }

      public int LastPath { get; set; }

      public CollisionMode Mode { get; set; }

      public CollisionVector GroundVector { get; set; }

      private double GroundAngle => this.GroundVector != null ? this.GroundVector.Angle : 0.0;

      public double DistanceFromLedge { get; set; }

      public ActiveObject ObjectLink { get; set; }

      public object ObjectTag { get; set; }

      public SonicOrca.Core.Objects.IPlatform CurrentPlatform => this.ObjectLink as SonicOrca.Core.Objects.IPlatform;

      public int LastTickOnGround { get; set; }

      private void ProcessCollision()
      {
        this.ResetCollisionEvents();
        if (this.GroundVector != null && this.GroundVector.Owner != null)
        {
          CollisionVector groundVector = this.GroundVector;
          if (!((IEnumerable<CollisionVector>) this.GroundVector.Owner.CollisionVectors).Contains<CollisionVector>(this.GroundVector))
            this.LeaveGround();
        }
        if (this.CheckCollision)
        {
          this._originalGroundVector = this.Mode == CollisionMode.Air ? (CollisionVector) null : this.GroundVector;
          this._collidedVectors.Clear();
          if (this.GroundVector != null)
          {
            switch (this.Mode)
            {
              case CollisionMode.Top:
              case CollisionMode.Left:
              case CollisionMode.Bottom:
              case CollisionMode.Right:
                this.ConstrainToPath();
                break;
            }
          }
          this.Bump((Vector2) this.CollisionRadius);
          this.CheckOverlappingVector();
          if (this.PlayerShouldLand())
            this.Land();
          this.UpdateCollisionsWithObjects();
          if (this.Mode != CollisionMode.Air)
          {
            this._collisionDetected[(int) this.Mode] = true;
            CollisionInfo collisionInfo = new CollisionInfo(this.GroundVector, new Vector2(), 0.0, this.GroundAngle);
            CollisionEvent collisionEvent;
            if (this.GroundVector != null)
            {
              collisionEvent = new CollisionEvent((ActiveObject) this, collisionInfo);
            }
            else
            {
              collisionEvent = new CollisionEvent((ActiveObject) this, 0);
              collisionEvent.CollisionInfo = collisionInfo;
            }
            this._collisionEvents[(int) this.Mode] = collisionEvent;
          }
          this.CheckCrushed();
        }
        this.UpdateDistanceFromLedge();
        this.LastPath = this.Path;
      }

      private void ResetCollisionEvents()
      {
        for (int index = 0; index < 4; ++index)
        {
          this._collisionEvents[index] = (CollisionEvent) null;
          this._collisionDetected[index] = false;
          this._collisionEventsTiles[index] = (CollisionEvent) null;
          this._collisionDetectedTiles[index] = false;
          this._overlappingGroundVector = (CollisionVector) null;
          this._overlappingCollision = new KeyValuePair<ActiveObject, CollisionEvent>((ActiveObject) null, (CollisionEvent) null);
        }
      }

      private bool CheckIgnoreFlag(CollisionVector t) => t.Flags.HasFlag((Enum) CollisionFlags.Ignore);

      private bool CheckSolidAngle(CollisionVector t, Vector2 intersection, Vector2 collisionRadius)
      {
        if (t.Flags.HasFlag((Enum) CollisionFlags.Solid))
          return false;
        Vector2 vector2 = new Vector2();
        double x = this.PositionPrecise.X;
        int num = 1;
        if (t.Mode == CollisionMode.Right)
          vector2 = this.GetPointRotatedFromRelative(intersection, new Vector2(intersection.X, intersection.Y - collisionRadius.X), Math.PI / 2.0 + this.GroundAngle);
        else if (t.Mode == CollisionMode.Left)
          vector2 = this.GetPointRotatedFromRelative(intersection, new Vector2(intersection.X, intersection.Y + collisionRadius.X), Math.PI / 2.0 + this.GroundAngle);
        else if (t.Mode == CollisionMode.Top)
          vector2 = this.GetPointRotatedFromRelative(intersection, new Vector2(intersection.X - collisionRadius.Y, intersection.Y), Math.PI / 2.0 + this.GroundAngle);
        else if (t.Mode == CollisionMode.Bottom)
          vector2 = this.GetPointRotatedFromRelative(intersection, new Vector2(intersection.X + collisionRadius.Y, intersection.Y), Math.PI / 2.0 + this.GroundAngle);
        return MathX.DifferenceRadians((this.PositionPrecise - vector2).Angle, t.Angle) * (double) num > 0.0;
      }

      private Vector2 GetPointRotatedFromOrigin(Vector2 center, Vector2 point, double angleInRadians)
      {
        double x1 = point.X;
        double y1 = point.Y;
        double x2 = center.X;
        double y2 = center.Y;
        return new Vector2((x1 - x2) * Math.Cos(angleInRadians) - (y1 - y2) * Math.Sin(angleInRadians) + x2, (x1 - x2) * Math.Sin(angleInRadians) + (y1 - y2) * Math.Cos(angleInRadians) + y2);
      }

      private Vector2 GetPointRotatedFromRelative(Vector2 relative, Vector2 point, double theta)
      {
        double x1 = point.X;
        double y1 = point.Y;
        double x2 = relative.X;
        double y2 = relative.Y;
        return new Vector2(Math.Cos(theta) * (x1 - x2) - Math.Sin(theta) * (y1 - y2) + x2, Math.Sin(theta) * (x1 - x2) + Math.Cos(theta) * (y1 - y2) + y2);
      }

      public Vector2[][] GetCollisionBox(Vector2 collisionRadius, bool rotate)
      {
        double num = 0.0;
        if (rotate)
          num = this.GroundAngle;
        Vector2 positionPrecise1 = this.PositionPrecise;
        Vector2 positionPrecise2 = this.PositionPrecise;
        double x1 = positionPrecise2.X - collisionRadius.X;
        positionPrecise2 = this.PositionPrecise;
        double y1 = positionPrecise2.Y - collisionRadius.Y;
        Vector2 point1 = new Vector2(x1, y1);
        double theta1 = num;
        Vector2 rotatedFromRelative1 = this.GetPointRotatedFromRelative(positionPrecise1, point1, theta1);
        Vector2 positionPrecise3 = this.PositionPrecise;
        positionPrecise2 = this.PositionPrecise;
        double x2 = positionPrecise2.X + collisionRadius.X;
        positionPrecise2 = this.PositionPrecise;
        double y2 = positionPrecise2.Y - collisionRadius.Y;
        Vector2 point2 = new Vector2(x2, y2);
        double theta2 = num;
        Vector2 rotatedFromRelative2 = this.GetPointRotatedFromRelative(positionPrecise3, point2, theta2);
        Vector2 positionPrecise4 = this.PositionPrecise;
        positionPrecise2 = this.PositionPrecise;
        double x3 = positionPrecise2.X + collisionRadius.X;
        positionPrecise2 = this.PositionPrecise;
        double y3 = positionPrecise2.Y + collisionRadius.Y;
        Vector2 point3 = new Vector2(x3, y3);
        double theta3 = num;
        Vector2 rotatedFromRelative3 = this.GetPointRotatedFromRelative(positionPrecise4, point3, theta3);
        Vector2 positionPrecise5 = this.PositionPrecise;
        positionPrecise2 = this.PositionPrecise;
        double x4 = positionPrecise2.X - collisionRadius.X;
        positionPrecise2 = this.PositionPrecise;
        double y4 = positionPrecise2.Y + collisionRadius.Y;
        Vector2 point4 = new Vector2(x4, y4);
        double theta4 = num;
        Vector2 rotatedFromRelative4 = this.GetPointRotatedFromRelative(positionPrecise5, point4, theta4);
        return new Vector2[4][]
        {
          new Vector2[2]{ rotatedFromRelative1, rotatedFromRelative2 },
          new Vector2[2]{ rotatedFromRelative3, rotatedFromRelative2 },
          new Vector2[2]{ rotatedFromRelative4, rotatedFromRelative3 },
          new Vector2[2]{ rotatedFromRelative4, rotatedFromRelative1 }
        };
      }

      private bool IsBeyondPathEnd(bool useLedgeSensor)
      {
        if (this.LastPositionPrecise == this.PositionPrecise)
          return false;
        int num1 = useLedgeSensor ? this.CollisionRadius.X - this.LedgeSensorRadius : 0;
        Vector2 positionPrecise1 = this.PositionPrecise;
        Vector2 positionPrecise2 = this.PositionPrecise;
        double x1 = positionPrecise2.X - (double) this.CollisionRadius.Y;
        positionPrecise2 = this.PositionPrecise;
        double y1 = positionPrecise2.Y;
        Vector2 point1 = new Vector2(x1, y1);
        double theta1 = this.GroundAngle + Math.PI / 2.0;
        Vector2 rotatedFromRelative1 = this.GetPointRotatedFromRelative(positionPrecise1, point1, theta1);
        Vector2 positionPrecise3 = this.PositionPrecise;
        positionPrecise2 = this.PositionPrecise;
        double x2 = positionPrecise2.X + (double) this.CollisionRadius.Y;
        positionPrecise2 = this.PositionPrecise;
        double y2 = positionPrecise2.Y;
        Vector2 point2 = new Vector2(x2, y2);
        double theta2 = this.GroundAngle + Math.PI / 2.0;
        Vector2 rotatedFromRelative2 = this.GetPointRotatedFromRelative(positionPrecise3, point2, theta2);
        Vector2i absoluteA = this.GroundVector.AbsoluteA;
        Vector2i absoluteB = this.GroundVector.AbsoluteB;
        if (Vector2.Intersects(rotatedFromRelative1, rotatedFromRelative2, (Vector2) absoluteA, (Vector2) absoluteB))
          return false;
        Vector2 intersection = new Vector2();
        Vector2.GetLineIntersection(rotatedFromRelative1, rotatedFromRelative2, (Vector2) absoluteA, (Vector2) absoluteB, out intersection);
        double distance1 = Vector2.GetDistance(intersection, (Vector2) this.GroundVector.Bounds.Centre);
        double distance2 = Vector2.GetDistance(intersection, (Vector2) absoluteA);
        double distance3 = Vector2.GetDistance(intersection, (Vector2) absoluteB);
        double distance4 = Vector2.GetDistance((Vector2) this.GroundVector.Bounds.Centre, (Vector2) absoluteB);
        double groundAngle = this.GroundAngle;
        double angle = this.GroundVector.Angle;
        double num2 = distance2;
        return distance3 < num2 && distance1 - (double) num1 > distance4;
      }

      private bool IsBeyondPathStart(bool useLedgeSensor)
      {
        Vector2i collisionRadius;
        int num1;
        if (!useLedgeSensor)
        {
          num1 = 0;
        }
        else
        {
          collisionRadius = this.CollisionRadius;
          num1 = collisionRadius.X - this.LedgeSensorRadius;
        }
        int num2 = num1;
        Vector2 positionPrecise1 = this.PositionPrecise;
        double x1 = this.PositionPrecise.X;
        collisionRadius = this.CollisionRadius;
        double y1 = (double) collisionRadius.Y;
        double x2 = x1 - y1;
        Vector2 positionPrecise2 = this.PositionPrecise;
        double y2 = positionPrecise2.Y;
        Vector2 point1 = new Vector2(x2, y2);
        double theta1 = this.GroundAngle + Math.PI / 2.0;
        Vector2 rotatedFromRelative1 = this.GetPointRotatedFromRelative(positionPrecise1, point1, theta1);
        Vector2 positionPrecise3 = this.PositionPrecise;
        positionPrecise2 = this.PositionPrecise;
        double x3 = positionPrecise2.X;
        collisionRadius = this.CollisionRadius;
        double y3 = (double) collisionRadius.Y;
        Vector2 point2 = new Vector2(x3 + y3, this.PositionPrecise.Y);
        double theta2 = this.GroundAngle + Math.PI / 2.0;
        Vector2 rotatedFromRelative2 = this.GetPointRotatedFromRelative(positionPrecise3, point2, theta2);
        Vector2i absoluteA = this.GroundVector.AbsoluteA;
        Vector2i absoluteB = this.GroundVector.AbsoluteB;
        if (Vector2.Intersects(rotatedFromRelative1, rotatedFromRelative2, (Vector2) absoluteA, (Vector2) absoluteB))
          return false;
        Vector2 intersection = new Vector2();
        Vector2.GetLineIntersection(rotatedFromRelative1, rotatedFromRelative2, (Vector2) absoluteA, (Vector2) absoluteB, out intersection);
        double distance1 = Vector2.GetDistance(intersection, (Vector2) this.GroundVector.Bounds.Centre);
        double distance2 = Vector2.GetDistance(intersection, (Vector2) this.GroundVector.AbsoluteA);
        double distance3 = Vector2.GetDistance(intersection, (Vector2) this.GroundVector.AbsoluteB);
        double distance4 = Vector2.GetDistance((Vector2) this.GroundVector.Bounds.Centre, (Vector2) absoluteA);
        double num3 = distance2;
        return distance3 > num3 && distance1 - (double) num2 > distance4;
      }

      private double GenesisHexToRadians(double angle)
      {
        return MathX.ToRadians((angle - 256.0) * (45.0 / 32.0));
      }

      private double GenesisHexToDegrees(double angle) => 360.0 + -(256.0 - angle) * (45.0 / 32.0);

      private bool IsLandingViable(CollisionVector vector)
      {
        if (vector.Flags.HasFlag((Enum) CollisionFlags.NoLanding))
        {
          this.Velocity = new Vector2();
          return false;
        }
        switch (vector.Mode)
        {
          case CollisionMode.Top:
            return this.Velocity.Y >= 0.0;
          case CollisionMode.Left:
          case CollisionMode.Right:
            if (vector.FlipX != 0 && Math.Abs(this.Velocity.X) >= 0.0)
              return true;
            if (this.Velocity.X * (double) vector.FlipY < 0.0)
            {
              this.Velocity = vector == null ? new Vector2(0.0, this.Velocity.Y) : new Vector2(0.0, this.Velocity.Y);
              if (this.Mode != CollisionMode.Air)
                this.GroundVelocity = 0.0;
            }
            else if (vector.FlipY == 0)
              this.Velocity = new Vector2(0.0, this.Velocity.Y);
            return false;
          case CollisionMode.Bottom:
            if (vector.FlipY != 0 && this.Velocity.Y < 0.0)
              return true;
            if (this.Velocity.Y < 0.0)
            {
              this.Velocity = new Vector2(this.Velocity.X, 0.0);
              if (this.Mode != CollisionMode.Air)
                this.GroundVelocity = 0.0;
            }
            return false;
          default:
            return false;
        }
      }

      private bool PlayerShouldLand()
      {
        bool flag1 = false;
        CollisionMode index1 = CollisionMode.Air;
        for (int index2 = 0; index2 < 4; ++index2)
        {
          if (this._collisionEvents[index2] != null && !this._collisionEvents[index2].MaintainVelocity && this.IsLandingViable(this._collisionEvents[index2].CollisionInfo.Vector))
            index1 = (CollisionMode) index2;
        }
        if (index1 != CollisionMode.Air && !this.CharacterEvents.HasFlag((Enum) CharacterEvent.Hurt))
        {
          bool flag2 = this.Mode == CollisionMode.Air;
          CollisionVector vector = this._collisionEvents[(int) index1].CollisionInfo.Vector;
          if (vector != this._originalGroundVector)
          {
            this.PositionPrecise = this.GetAlignmentForVector2(vector, this.PositionPrecise, this.GroundAngle);
            this.AcquireVector(vector);
            if (flag2)
              flag1 = true;
            this.Bump((Vector2) this.CollisionRadius);
          }
        }
        return flag1;
      }

      private void Land(bool updateSpeed = true)
      {
        if (this.Barrier == BarrierType.Bubble && this.HasPerformedBarrierAttack)
        {
          this.Mode = CollisionMode.Air;
          this.GroundVector = (CollisionVector) null;
          this.IsJumping = true;
          this.IsRollJumping = false;
          this.HasPerformedBarrierAttack = false;
          this.Velocity = new Vector2(this.Velocity.X, -30.0);
        }
        else
        {
          this.IsAirborne = false;
          this.IsJumping = false;
          if (this.Level.Ticks - this.LastTickOnGround > 11)
            this.IsSpinball = false;
          this.IsRollJumping = false;
          this.IsHurt = false;
          this.IsFlying = false;
          this.HasPerformedBarrierAttack = false;
          this.ShowAngle = this.GroundAngle;
          this.Jumped = false;
          if (updateSpeed)
          {
            if (this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.NoAngle))
            {
              this.GroundVelocity = this.Velocity.X;
            }
            else
            {
              double num1;
              if (Math.Abs(this.Velocity.X) <= Math.Abs(this.Velocity.Y))
              {
                Vector2 velocity = this.Velocity;
                double num2 = velocity.X * Math.Cos(this.GroundAngle);
                velocity = this.Velocity;
                double num3 = velocity.Y * Math.Sin(this.GroundAngle);
                num1 = num2 + num3;
              }
              else
                num1 = this.Velocity.X * (double) this.GroundVector.FlipX;
              this.GroundVelocity = num1;
              this.Velocity = new Vector2(this.GroundVelocity * Math.Cos(this.GroundAngle), this.GroundVelocity * Math.Sin(this.GroundAngle));
            }
          }
        }
        this.Player.ResetScoreChain();
      }

      public void LeaveGround()
      {
        if (this.GroundVector != null)
        {
          this.RemovePlatformVelocity();
          if (this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.Snap))
          {
            Math.Sign(this.GroundVelocity);
            double num = MathX.Snap(this.GroundAngle, Math.PI / 2.0);
            this.Velocity = new Vector2(this.GroundVelocity * Math.Cos(num), this.GroundVelocity * Math.Sin(num));
          }
          this.GroundVector = (CollisionVector) null;
          this.ObjectLink = (ActiveObject) null;
        }
        this.Mode = CollisionMode.Air;
        this.IsAirborne = true;
        this.IsBraking = false;
        this.IsCharging = false;
        this.IsHurt = false;
      }

      private void ConstrainToPath()
      {
        CollisionVector collisionVector1 = (CollisionVector) null;
        bool flag1 = true;
        if (this.IsBeyondPathEnd(false))
        {
          CollisionVector t = this.GroundVector;
          bool flag2 = true;
          double num = double.MaxValue;
          CollisionVector collisionVector2 = (CollisionVector) null;
          while (flag2)
          {
            if ((t = t.GetConnectionB(this.Path)) != null)
            {
              double distanceToVector = this.GetDistanceToVector(t);
              if (distanceToVector < num)
              {
                num = distanceToVector;
                if (this._lastLeftVector != t)
                {
                  collisionVector2 = t;
                  this._lastLeftVector = collisionVector2;
                }
              }
              else
                flag2 = false;
            }
            else
              flag2 = false;
          }
          collisionVector1 = collisionVector2;
        }
        else if (this.IsBeyondPathStart(false))
        {
          CollisionVector t = this.GroundVector;
          bool flag3 = true;
          double num = double.MaxValue;
          CollisionVector collisionVector3 = (CollisionVector) null;
          while (flag3)
          {
            if ((t = t.GetConnectionA(this.Path)) != null)
            {
              double distanceToVector = this.GetDistanceToVector(t);
              if (distanceToVector < num)
              {
                num = distanceToVector;
                if (this._lastRightVector != t)
                {
                  collisionVector3 = t;
                  this._lastRightVector = collisionVector3;
                }
              }
              else
                flag3 = false;
            }
            else
              flag3 = false;
          }
          collisionVector1 = collisionVector3;
        }
        else
        {
          this._lastRightVector = (CollisionVector) null;
          this._lastLeftVector = (CollisionVector) null;
          flag1 = false;
        }
        if (collisionVector1 != null)
        {
          if (!collisionVector1.Flags.HasFlag((Enum) CollisionFlags.NoPathFollowing))
          {
            this.PositionPrecise = this.GetAlignmentForVector2(collisionVector1, this.PositionPrecise, this.GroundAngle);
            this.AcquireVector(collisionVector1);
          }
          else
          {
            if (!flag1)
              return;
            this.LeaveGround();
          }
        }
        else if (flag1)
        {
          if (!this.IsBeyondPathEnd(true) && !this.IsBeyondPathStart(true))
            return;
          this.LeaveGround();
        }
        else
        {
          this.PlayerShouldLand();
          if (!this.CheckIgnoreFlag(this.GroundVector))
            return;
          this.LeaveGround();
        }
      }

      private IEnumerable<CollisionVector> FixPosition(
        Vector2 collisionRadius,
        IEnumerable<CollisionVector> vectorsToCheck)
      {
        List<CollisionVector> collisionVectorList = new List<CollisionVector>();
        double num1 = 0.0;
        double num2 = 0.0;
        double num3 = 0.0;
        double num4 = 0.0;
        bool flag1 = false;
        foreach (CollisionVector collisionVector in vectorsToCheck)
        {
          Vector2i collisionRadius1;
          double x1;
          double y1;
          if (this.Mode == CollisionMode.Left || this.Mode == CollisionMode.Right)
          {
            flag1 = true;
            collisionRadius1 = this.CollisionRadius;
            x1 = (double) collisionRadius1.Y;
            collisionRadius1 = this.CollisionRadius;
            y1 = (double) collisionRadius1.X;
          }
          else
          {
            collisionRadius1 = this.CollisionRadius;
            x1 = (double) collisionRadius1.X;
            collisionRadius1 = this.CollisionRadius;
            y1 = (double) collisionRadius1.Y;
          }
          Vector2 positionPrecise1 = this.PositionPrecise;
          Vector2 vector2_1 = this.PositionPrecise;
          double x2 = vector2_1.X;
          vector2_1 = this.PositionPrecise;
          double y2 = vector2_1.Y + x1;
          Vector2 point1 = new Vector2(x2, y2);
          double theta1 = Math.PI / 2.0 + this.GroundAngle;
          Vector2 rotatedFromRelative1 = this.GetPointRotatedFromRelative(positionPrecise1, point1, theta1);
          Vector2 positionPrecise2 = this.PositionPrecise;
          vector2_1 = this.PositionPrecise;
          double x3 = vector2_1.X;
          vector2_1 = this.PositionPrecise;
          double y3 = vector2_1.Y - x1;
          Vector2 point2 = new Vector2(x3, y3);
          double theta2 = Math.PI / 2.0 + this.GroundAngle;
          Vector2 rotatedFromRelative2 = this.GetPointRotatedFromRelative(positionPrecise2, point2, theta2);
          Vector2 positionPrecise3 = this.PositionPrecise;
          vector2_1 = this.PositionPrecise;
          double x4 = vector2_1.X + y1;
          vector2_1 = this.PositionPrecise;
          double y4 = vector2_1.Y;
          Vector2 point3 = new Vector2(x4, y4);
          double theta3 = Math.PI / 2.0 + this.GroundAngle;
          Vector2 rotatedFromRelative3 = this.GetPointRotatedFromRelative(positionPrecise3, point3, theta3);
          Vector2 positionPrecise4 = this.PositionPrecise;
          vector2_1 = this.PositionPrecise;
          double x5 = vector2_1.X - y1;
          vector2_1 = this.PositionPrecise;
          double y5 = vector2_1.Y;
          Vector2 point4 = new Vector2(x5, y5);
          double theta4 = Math.PI / 2.0 + this.GroundAngle;
          Vector2 rotatedFromRelative4 = this.GetPointRotatedFromRelative(positionPrecise4, point4, theta4);
          Vector2[][] collisionBox1 = this.GetCollisionBox(new Vector2(x1, y1 - (double) this.FloorSensorRadius + 20.0), false);
          Vector2[][] collisionBox2 = this.GetCollisionBox(new Vector2(x1 - (double) this.LedgeSensorRadius, y1), false);
          Vector2[][] vector2Array1 = new Vector2[0][];
          Vector2[][] vector2Array2 = new Vector2[0][];
          Vector2 vector2_2 = new Vector2();
          Vector2 vector2_3 = new Vector2();
          Vector2 vector2_4 = new Vector2();
          Vector2 vector2_5 = new Vector2();
          Vector2[][] collection1;
          Vector2[][] collection2;
          Vector2 vector2_6;
          Vector2 vector2_7;
          Vector2 vector2_8;
          Vector2 vector2_9;
          if (flag1)
          {
            collection1 = collisionBox2;
            collection2 = collisionBox1;
            vector2_6 = rotatedFromRelative3;
            vector2_7 = rotatedFromRelative4;
            vector2_8 = rotatedFromRelative1;
            vector2_9 = rotatedFromRelative2;
          }
          else
          {
            collection1 = collisionBox1;
            collection2 = collisionBox2;
            vector2_6 = rotatedFromRelative1;
            vector2_7 = rotatedFromRelative2;
            vector2_8 = rotatedFromRelative3;
            vector2_9 = rotatedFromRelative4;
          }
          if (this.CanCollideWithVector(collisionVector))
          {
            Vector2[] vector2Array3 = new Vector2[2]
            {
              (Vector2) collisionVector.AbsoluteA,
              (Vector2) collisionVector.AbsoluteB
            };
            bool flag2 = false;
            if (!this.CheckIgnoreFlag(collisionVector))
            {
              bool flag3 = false;
              if (collisionVector.IsWall)
              {
                List<Vector2[]> vector2ArrayList = new List<Vector2[]>()
                {
                  new Vector2[2]{ vector2_6, vector2_7 }
                };
                vector2ArrayList.AddRange((IEnumerable<Vector2[]>) collection1);
                List<Vector2> source = new List<Vector2>();
                for (int index = 0; index < vector2ArrayList.Count; ++index)
                {
                  if (Vector2.Intersects(vector2ArrayList[index][0], vector2ArrayList[index][1], vector2Array3[0], vector2Array3[1]))
                  {
                    flag2 = true;
                    Vector2 intersection = new Vector2();
                    Vector2.GetLineIntersection(vector2ArrayList[index][0], vector2ArrayList[index][1], vector2Array3[0], vector2Array3[1], out intersection);
                    source.Add(intersection);
                  }
                  bool flag4 = index >= 1;
                }
                if (!flag2)
                {
                  Rectangle rectangle = Rectangle.FromLTRB(collection1[0][0].X, collection1[0][0].Y, collection1[1][0].X, collection1[1][0].Y);
                  if (rectangle.ContainsOrOverlaps(vector2Array3[0]))
                  {
                    Vector2 intersection = new Vector2();
                    Vector2 rotatedFromRelative5 = this.GetPointRotatedFromRelative(vector2Array3[0], new Vector2(vector2Array3[0].X - 5.0, vector2Array3[0].Y), Math.PI / 2.0);
                    Vector2.GetLineIntersection(vector2Array3[0], rotatedFromRelative5, vector2ArrayList[0][0], vector2ArrayList[0][1], out intersection);
                    source.Add(intersection);
                    flag2 = true;
                  }
                  if (rectangle.ContainsOrOverlaps(vector2Array3[1]))
                  {
                    Vector2 intersection = new Vector2();
                    Vector2 rotatedFromRelative6 = this.GetPointRotatedFromRelative(vector2Array3[1], new Vector2(vector2Array3[1].X - 5.0, vector2Array3[1].Y), Math.PI / 2.0);
                    Vector2.GetLineIntersection(vector2Array3[1], rotatedFromRelative6, vector2ArrayList[0][0], vector2ArrayList[0][1], out intersection);
                    source.Add(intersection);
                    flag2 = true;
                  }
                }
                if (flag2)
                {
                  Vector2 vector2_10 = new Vector2();
                  vector2_1 = this.PositionPrecise;
                  double x6 = vector2_1.X;
                  double num5 = num2;
                  double num6 = num1;
                  Vector2 vector2_11 = new Vector2(-1.0, -1.0);
                  if (collisionVector.Mode == CollisionMode.Right)
                  {
                    Vector2 vector2_12 = source.First<Vector2>();
                    foreach (Vector2 vector2_13 in source.Skip<Vector2>(1))
                    {
                      if (vector2_13.X < vector2_12.X)
                        vector2_12 = vector2_13;
                    }
                    vector2_11 = vector2_12;
                    Vector2 rotatedFromRelative7 = this.GetPointRotatedFromRelative(vector2_11, new Vector2(vector2_11.X, vector2_11.Y - x1), Math.PI / 2.0 + this.GroundAngle);
                    if (rotatedFromRelative7.X - x6 >= num2)
                      num2 = rotatedFromRelative7.X - x6;
                  }
                  else if (collisionVector.Mode == CollisionMode.Left)
                  {
                    Vector2 vector2_14 = source.First<Vector2>();
                    foreach (Vector2 vector2_15 in source.Skip<Vector2>(1))
                    {
                      if (vector2_15.X > vector2_14.X)
                        vector2_14 = vector2_15;
                    }
                    vector2_11 = vector2_14;
                    Vector2 rotatedFromRelative8 = this.GetPointRotatedFromRelative(vector2_11, new Vector2(vector2_11.X, vector2_11.Y + x1), Math.PI / 2.0 + this.GroundAngle);
                    if (rotatedFromRelative8.X - x6 < num1)
                      num1 = rotatedFromRelative8.X - x6;
                  }
                  if (!this.CheckSolidAngle(collisionVector, vector2_11, new Vector2(x1, y1)))
                  {
                    double distance = Vector2.GetDistance(rotatedFromRelative2, vector2_11);
                    CollisionEvent e = new CollisionEvent((ActiveObject) this, new CollisionInfo(collisionVector, vector2_11, distance, collisionVector.Angle));
                    if (collisionVector.Owner != null)
                      collisionVector.Owner.Collision(e);
                    if (!e.IgnoreCollision)
                    {
                      this._collisionDetected[(int) collisionVector.Mode] = true;
                      this._collisionEvents[(int) collisionVector.Mode] = e;
                    }
                    Vector2 positionPrecise5 = this.PositionPrecise;
                    double num7 = 0.0;
                    double num8 = 0.0;
                    if (num5 != num2)
                      num7 = num2 - num5;
                    if (num6 != num1)
                      num8 = num1 - num6;
                    if (num7 != 0.0 || num8 != 0.0)
                    {
                      positionPrecise5.X += num7;
                      positionPrecise5.X += num8;
                      this.PositionPrecise = positionPrecise5;
                    }
                  }
                }
              }
              else
              {
                List<Vector2[]> vector2ArrayList = new List<Vector2[]>()
                {
                  new Vector2[2]{ vector2_8, vector2_9 }
                };
                vector2ArrayList.AddRange((IEnumerable<Vector2[]>) collection2);
                List<Vector2> source = new List<Vector2>();
                for (int index = 0; index < vector2ArrayList.Count; ++index)
                {
                  if (Vector2.Intersects(vector2ArrayList[index][0], vector2ArrayList[index][1], vector2Array3[0], vector2Array3[1]))
                  {
                    flag2 = true;
                    Vector2 intersection = new Vector2();
                    Vector2.GetLineIntersection(vector2ArrayList[index][0], vector2ArrayList[index][1], vector2Array3[0], vector2Array3[1], out intersection);
                    source.Add(intersection);
                  }
                  flag3 = index >= 1;
                }
                if (!flag2)
                {
                  Rectangle rectangle = Rectangle.FromLTRB(collection2[0][0].X, collection2[0][0].Y, collection2[1][0].X, collection2[1][0].Y);
                  if (rectangle.ContainsOrOverlaps(vector2Array3[0]))
                  {
                    Vector2 intersection = new Vector2();
                    Vector2.GetLineIntersection(this.GetPointRotatedFromRelative(vector2Array3[0], new Vector2(vector2Array3[0].X, vector2Array3[0].Y + 5.0), Math.PI / 2.0), vector2Array3[0], vector2ArrayList[0][0], vector2ArrayList[0][1], out intersection);
                    source.Add(intersection);
                    flag2 = true;
                  }
                  if (rectangle.ContainsOrOverlaps(vector2Array3[1]))
                  {
                    Vector2 intersection = new Vector2();
                    Vector2.GetLineIntersection(this.GetPointRotatedFromRelative(vector2Array3[1], new Vector2(vector2Array3[1].X, vector2Array3[1].Y + 5.0), Math.PI / 2.0), vector2Array3[1], vector2ArrayList[0][0], vector2ArrayList[0][1], out intersection);
                    source.Add(intersection);
                    flag2 = true;
                  }
                }
                if (flag2)
                {
                  Vector2 vector2_16 = new Vector2(-1.0, -1.0);
                  if (!this.ShouldDisregardAsTooHighStep(collisionVector))
                  {
                    Vector2 vector2_17 = new Vector2();
                    Math.Sign(this.GroundVelocity);
                    Math.Min(Math.Abs(this.GroundVelocity), 96.0);
                    Vector2 vector2_18 = new Vector2(MathX.Clamp(-64.0, this.GroundVelocity * Math.Cos(0.0), 64.0), MathX.Clamp(-64.0, this.GroundVelocity * Math.Sin(0.0), 64.0));
                    double y6;
                    if (this.IsAirborne)
                    {
                      vector2_1 = this.Velocity;
                      y6 = vector2_1.Y;
                    }
                    else
                      y6 = vector2_18.Y;
                    vector2_1 = this.PositionPrecise;
                    double y7 = vector2_1.Y;
                    double num9 = num3;
                    double num10 = num4;
                    if (collisionVector.Mode == CollisionMode.Top)
                    {
                      Vector2 vector2_19 = source.First<Vector2>();
                      foreach (Vector2 vector2_20 in source.Skip<Vector2>(1))
                      {
                        if (vector2_20.Y > vector2_19.Y)
                          vector2_19 = vector2_20;
                      }
                      vector2_16 = vector2_19;
                      if (y6 >= 0.0)
                      {
                        vector2_17 = this.GetPointRotatedFromRelative(vector2_16, new Vector2(vector2_16.X - y1, vector2_16.Y), Math.PI / 2.0 + this.GroundAngle);
                        if (vector2_17.Y - y7 < num3)
                        {
                          num3 = vector2_17.Y - y7;
                          if (collisionVector != this.GroundVector && this.GroundVector != null)
                          {
                            if (collisionVector.Owner != null)
                            {
                              if (collisionVector.Owner.PositionPrecise != collisionVector.Owner.LastPositionPrecise)
                                this._overlappingGroundVector = collisionVector;
                            }
                            else if (this.GroundVector.Owner != null)
                              this._overlappingGroundVector = collisionVector;
                            else if (collisionVector.Mode == this.Mode)
                              this._overlappingGroundVector = collisionVector;
                          }
                        }
                      }
                    }
                    else if (collisionVector.Mode == CollisionMode.Bottom)
                    {
                      Vector2 vector2_21 = source.First<Vector2>();
                      foreach (Vector2 vector2_22 in source.Skip<Vector2>(1))
                      {
                        if (vector2_22.Y > vector2_21.Y)
                          vector2_21 = vector2_22;
                      }
                      vector2_16 = vector2_21;
                      if (y6 < 0.0)
                      {
                        vector2_17 = this.GetPointRotatedFromRelative(vector2_16, new Vector2(vector2_16.X + y1, vector2_16.Y), Math.PI / 2.0 + this.GroundAngle);
                        if (vector2_17.Y - y7 > num4)
                          num4 = vector2_17.Y - y7;
                      }
                    }
                    if (!this.CheckSolidAngle(collisionVector, vector2_16, new Vector2(x1, y1)))
                    {
                      double distance = Vector2.GetDistance(rotatedFromRelative4, vector2_16);
                      CollisionEvent collisionEvent = new CollisionEvent((ActiveObject) this, new CollisionInfo(collisionVector, vector2_16, distance, collisionVector.Angle));
                      if (flag3 && this.GroundVector != null && (collisionVector.Mode == CollisionMode.Top || collisionVector.Mode == CollisionMode.Bottom) && collisionVector.Owner == null && !this.IsFlying)
                      {
                        this._collisionDetectedTiles[(int) collisionVector.Mode] = true;
                        this._collisionEventsTiles[(int) collisionVector.Mode] = collisionEvent;
                      }
                      else
                      {
                        if (collisionVector.Owner != null)
                        {
                          KeyValuePair<ActiveObject, CollisionEvent> keyValuePair = new KeyValuePair<ActiveObject, CollisionEvent>(collisionVector.Owner, collisionEvent);
                          this._collidedVectors.Add(keyValuePair);
                          if (this._overlappingGroundVector != null)
                          {
                            if (collisionVector.Owner == this._overlappingGroundVector.Owner)
                              this._overlappingCollision = keyValuePair;
                            else
                              keyValuePair.Key.Collision(keyValuePair.Value);
                          }
                          else
                            keyValuePair.Key.Collision(keyValuePair.Value);
                        }
                        if (!collisionEvent.IgnoreCollision)
                        {
                          this._collisionDetected[(int) collisionVector.Mode] = true;
                          this._collisionEvents[(int) collisionVector.Mode] = collisionEvent;
                        }
                        Vector2 positionPrecise6 = this.PositionPrecise;
                        double num11 = 0.0;
                        double num12 = 0.0;
                        if (num9 != num3)
                          num11 = num3 - num9;
                        if (num10 != num4)
                          num12 = num4 - num10;
                        if (num11 != 0.0 || num12 != 0.0)
                        {
                          positionPrecise6.Y += num11;
                          positionPrecise6.Y += num12;
                          this.PositionPrecise = positionPrecise6;
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        return (IEnumerable<CollisionVector>) collisionVectorList;
      }

      private void CheckOverlappingVector()
      {
        if (this._overlappingGroundVector == null || this._overlappingGroundVector == this.GroundVector || this.GroundVector == null || this._overlappingGroundVector.Mode != this.GroundVector.Mode || this._overlappingGroundVector == this.GroundVector.GetConnectionA(this.Path) || this._overlappingGroundVector == this.GroundVector.GetConnectionB(this.Path))
          return;
        bool flag = true;
        if (this._overlappingGroundVector.Owner != null)
          flag = ((IEnumerable<CollisionVector>) this._overlappingGroundVector.Owner.CollisionVectors).Contains<CollisionVector>(this._overlappingGroundVector);
        this.PositionPrecise = this.GetAlignmentForVector2(this._overlappingGroundVector, this.PositionPrecise, this._overlappingGroundVector.Angle);
        if (!flag)
          return;
        this.AcquireVector(this._overlappingGroundVector);
        if (this._overlappingGroundVector.Owner != null)
          this.Land(false);
        else
          this.Land();
        if (this._overlappingCollision.Key == null || this._overlappingCollision.Value == null)
          return;
        this._overlappingCollision.Key.Collision(this._overlappingCollision.Value);
      }

      private bool IsAttachedToPlatform()
      {
        Vector2 vector2 = new Vector2();
        if (this.GroundVector != null && this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.Conveyor))
          vector2 = !(this.GroundVector.Owner is SonicOrca.Core.Objects.IPlatform) ? this.GroundVector.Owner.PositionPrecise - this.GroundVector.Owner.LastPositionPrecise : (this.GroundVector.Owner as SonicOrca.Core.Objects.IPlatform).Velocity;
        return vector2 != new Vector2();
      }

      private void Bump(Vector2 collisionRadius)
      {
        Rectangle bounds;
        ref Rectangle local = ref bounds;
        Vector2 positionPrecise = this.PositionPrecise;
        double x = positionPrecise.X - 128.0;
        positionPrecise = this.PositionPrecise;
        double y = positionPrecise.Y - 128.0;
        local = new Rectangle(x, y, 256.0, 256.0);
        IEnumerable<CollisionVector> collisionIntersections = this.Level.CollisionTable.GetPossibleCollisionIntersections((Rectanglei) bounds, this.CheckLandscapeCollision, this.CheckObjectCollision);
        bool platform = this.IsAttachedToPlatform();
        Vector2 velocity = this.Velocity;
        double num1 = Math.Abs(velocity.X);
        velocity = this.Velocity;
        double num2 = Math.Abs(velocity.Y);
        IEnumerable<CollisionVector> vectorsToCheck = !(num1 >= num2 | platform) ? (IEnumerable<CollisionVector>) collisionIntersections.OrderBy<CollisionVector, int>((Func<CollisionVector, int>) (t =>
        {
          if (t.IsWall)
            return 1;
          return t.Mode != CollisionMode.Top && t.Owner != null ? 2 : 0;
        })) : (IEnumerable<CollisionVector>) collisionIntersections.OrderBy<CollisionVector, int>((Func<CollisionVector, int>) (t =>
        {
          if (t.IsWall)
            return 0;
          return t.Mode != CollisionMode.Top && t.Owner != null ? 2 : 1;
        }));
        this.FixPosition(collisionRadius, vectorsToCheck);
      }

      private bool ShouldDisregardAsTooHighStep(CollisionVector t)
      {
        bool flag = false;
        if (this.Mode == t.Mode && Math.Abs(t.Bottom - (this.Position.Y + this.CollisionRadius.Y)) >= this.FloorSensorRadius)
        {
          flag = true;
          if (t.Owner != null && t.Owner.PositionPrecise.Y - t.Owner.LastPositionPrecise.Y < 0.0)
            flag = false;
        }
        return flag;
      }

      private bool CanCollideWithVector(CollisionVector vector) => vector.HasPath(this.Path);

      private double GetDistanceToVector(CollisionVector t)
      {
        return Vector2.GetDistance(this.PositionPrecise, (Vector2) t.Bounds.Centre);
      }

      public Vector2 GetProjectedPointOnLine(Vector2 v1, Vector2 v2, Vector2 point)
      {
        Vector2 projectedPointOnLine = point;
        Vector2 vector2 = new Vector2(v2.X - v1.X, v2.Y - v1.Y);
        Vector2 v = new Vector2(projectedPointOnLine.X - v1.X, projectedPointOnLine.Y - v1.Y);
        double num1 = vector2.Dot(v);
        double num2 = Math.Sqrt(vector2.X * vector2.X + vector2.Y * vector2.Y);
        double num3 = Math.Sqrt(v.X * v.X + v.Y * v.Y);
        double num4 = num2 * num3;
        double num5 = num1 / num4 * num3;
        projectedPointOnLine = new Vector2(v1.X + num5 * vector2.X / num2, v1.Y + num5 * vector2.Y / num2);
        return projectedPointOnLine;
      }

      public Vector2 GetProjectedPointOnLine2(Vector2 v1, Vector2 v2, Vector2 point)
      {
        Vector2 projectedPointOnLine2 = point;
        Vector2 vector2 = new Vector2(v2.X - v1.X, v2.Y - v1.Y);
        Vector2 v = new Vector2(projectedPointOnLine2.X - v1.X, projectedPointOnLine2.Y - v1.Y);
        double num1 = vector2.Dot(v);
        double num2 = Math.Sqrt(vector2.X * vector2.X + vector2.Y * vector2.Y);
        double num3 = Math.Sqrt(v.X * v.X + v.Y * v.Y);
        double num4 = num2 * num3;
        double num5 = num1 / num4 * num3;
        projectedPointOnLine2 = new Vector2(v1.X + num5 * vector2.X / num2, v1.Y + num5 * vector2.Y / num2);
        return projectedPointOnLine2;
      }

      private Vector2 GetAlignmentForVector(
        CollisionVector t,
        Vector2 alignmentPosition,
        double alignmentAngle)
      {
        int y = this.CollisionRadius.Y;
        this.GetPointRotatedFromRelative(alignmentPosition, new Vector2(alignmentPosition.X - (double) y, alignmentPosition.Y), Math.PI / 2.0 + alignmentAngle);
        Vector2 rotatedFromRelative1 = this.GetPointRotatedFromRelative(alignmentPosition, new Vector2(alignmentPosition.X + (double) y, alignmentPosition.Y), Math.PI / 2.0 + alignmentAngle);
        Vector2 rotatedFromRelative2 = this.GetPointRotatedFromRelative(rotatedFromRelative1, new Vector2(rotatedFromRelative1.X - (double) y, rotatedFromRelative1.Y), Math.PI / 2.0 + t.Angle);
        Vector2 rotatedFromRelative3 = this.GetPointRotatedFromRelative(rotatedFromRelative1, new Vector2(rotatedFromRelative1.X + (double) y, rotatedFromRelative1.Y), Math.PI / 2.0 + t.Angle);
        Vector2 intersection = ((IEnumerable<Vector2>) Character.Intersector.Intersection((Vector2) t.AbsoluteB, (Vector2) t.AbsoluteA, rotatedFromRelative3, rotatedFromRelative2)).FirstOrDefault<Vector2>();
        Vector2.GetLineIntersection((Vector2) t.AbsoluteB, (Vector2) t.AbsoluteA, rotatedFromRelative3, rotatedFromRelative2, out intersection);
        return this.GetPointRotatedFromRelative(intersection, new Vector2(intersection.X - (double) y, intersection.Y), Math.PI / 2.0 + t.Angle);
      }

      public Vector2 GetAlignmentForVector2(
        CollisionVector t,
        Vector2 alignmentPosition,
        double alignmentAngle)
      {
        double y = (double) this.CollisionRadius.Y;
        Vector2 rotatedFromRelative1 = this.GetPointRotatedFromRelative(alignmentPosition, new Vector2(alignmentPosition.X - y, alignmentPosition.Y), Math.PI / 2.0 + t.Angle);
        Vector2 rotatedFromRelative2 = this.GetPointRotatedFromRelative(alignmentPosition, new Vector2(alignmentPosition.X + y, alignmentPosition.Y), Math.PI / 2.0 + t.Angle);
        Vector2 intersection = ((IEnumerable<Vector2>) Character.Intersector.Intersection((Vector2) t.AbsoluteA, (Vector2) t.AbsoluteB, rotatedFromRelative1, rotatedFromRelative2)).FirstOrDefault<Vector2>();
        Vector2.GetLineIntersection((Vector2) t.AbsoluteA, (Vector2) t.AbsoluteB, rotatedFromRelative1, rotatedFromRelative2, out intersection);
        return this.GetPointRotatedFromRelative(intersection, new Vector2(intersection.X - y, intersection.Y), Math.PI / 2.0 + t.Angle);
      }

      private void AcquireVector(CollisionVector vector)
      {
        this.GroundVector = vector;
        this.Mode = vector.Mode;
        this.ObjectLink = vector.Owner;
      }

      private void UpdateCollisionsWithObjects()
      {
        Vector2i vector2i = this.RectangleCollisionRadius;
        int y1 = vector2i.Y;
        vector2i = this.NormalCollisionRadius;
        int y2 = vector2i.Y;
        int num = y1 - y2;
        Vector2 positionPrecise = this.PositionPrecise;
        double left = positionPrecise.X - (double) this.RectangleCollisionRadius.X;
        positionPrecise = this.PositionPrecise;
        double top = positionPrecise.Y - (double) this.RectangleCollisionRadius.Y - (double) num;
        positionPrecise = this.PositionPrecise;
        double right = positionPrecise.X + (double) this.RectangleCollisionRadius.X;
        positionPrecise = this.PositionPrecise;
        double bottom = positionPrecise.Y + (double) this.RectangleCollisionRadius.Y - (double) num;
        Rectangle rect = Rectangle.FromLTRB(left, top, right, bottom);
        foreach (ActiveObject activeObject in (IEnumerable<ActiveObject>) this.Level.ObjectManager.ActiveObjects)
        {
          foreach (CollisionRectangle collisionRectangle in activeObject.CollisionRectangles)
          {
            if (collisionRectangle.AbsoluteBounds.IntersectsWith((Rectanglei) rect))
              activeObject.Collision(new CollisionEvent((ActiveObject) this, collisionRectangle.Id));
          }
        }
      }

      private void UpdateDistanceFromLedge()
      {
        if (this.Mode == CollisionMode.Air || this.GroundVector == null)
        {
          this.DistanceFromLedge = 0.0;
        }
        else
        {
          if (this.Mode == CollisionMode.Left || this.Mode == CollisionMode.Right)
          {
            Vector2 positionPrecise;
            Vector2i vector2i;
            if (this.GroundVector.GetConnectionB(this.Path) == null)
            {
              positionPrecise = this.PositionPrecise;
              double y1 = positionPrecise.Y;
              vector2i = this.GroundVector.AbsoluteB;
              double y2 = (double) vector2i.Y;
              double num = (y1 - y2) * (double) this.GroundVector.FlipY;
              if (num > 0.0)
              {
                this.DistanceFromLedge = num;
                return;
              }
            }
            if (this.GroundVector.GetConnectionA(this.Path) == null)
            {
              positionPrecise = this.PositionPrecise;
              double y3 = positionPrecise.Y;
              vector2i = this.GroundVector.AbsoluteA;
              double y4 = (double) vector2i.Y;
              double num = (y3 - y4) * (double) this.GroundVector.FlipY;
              if (num < 0.0)
              {
                this.DistanceFromLedge = num;
                return;
              }
            }
          }
          else
          {
            Vector2 positionPrecise;
            Vector2i vector2i;
            if (this.GroundVector.GetConnectionB(this.Path) == null)
            {
              positionPrecise = this.PositionPrecise;
              double x1 = positionPrecise.X;
              vector2i = this.GroundVector.AbsoluteB;
              double x2 = (double) vector2i.X;
              double num = (x1 - x2) * (double) this.GroundVector.FlipX;
              if (num > 0.0)
              {
                this.DistanceFromLedge = num;
                return;
              }
            }
            if (this.GroundVector.GetConnectionA(this.Path) == null)
            {
              positionPrecise = this.PositionPrecise;
              double x3 = positionPrecise.X;
              vector2i = this.GroundVector.AbsoluteA;
              double x4 = (double) vector2i.X;
              double num = (x3 - x4) * (double) this.GroundVector.FlipX;
              if (num < 0.0)
              {
                this.DistanceFromLedge = num;
                return;
              }
            }
          }
          this.DistanceFromLedge = 0.0;
        }
      }

      private void AdjustForPlatform(CollisionVector vector)
      {
        this._lastPositionAdjusted = this.PositionPrecise;
        if (vector == null || vector == this.GroundVector)
          return;
        if (vector.Owner is SonicOrca.Core.Objects.IPlatform owner)
          this._lastPositionAdjusted += owner.Velocity;
        else if (vector.Owner != null)
          this._lastPositionAdjusted += vector.Owner.PositionPrecise - vector.Owner.LastPositionPrecise;
        if (this.GroundVector == null || this.IsAirborne)
          return;
        this.PositionPrecise = this._lastPositionAdjusted;
        this.PositionPrecise = this.GetAlignmentForVector2(this.GroundVector, this.PositionPrecise, vector.Angle);
      }

      private void AddPlatformVelocity()
      {
        if (this.GroundVector == null || !this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.Conveyor))
          return;
        Vector2 vector2 = new Vector2();
        bool flag = false;
        if (this.GroundVector.Owner is SonicOrca.Core.Objects.IPlatform)
        {
          vector2 = (this.GroundVector.Owner as SonicOrca.Core.Objects.IPlatform).Velocity;
        }
        else
        {
          Vector2.GetDistance(this.PositionPrecise, (Vector2) this.GroundVector.Bounds.Centre);
          flag = !this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.NoAutoAlignment);
        }
        this.PositionPrecise = (Vector2) this.Position + (this.GroundVector.Owner.PositionPrecise - (Vector2) this.GroundVector.Owner.Position);
        this.Velocity += vector2;
        if (!flag)
          return;
        this.PositionPrecise = this.GetAlignmentForVector2(this.GroundVector, this.PositionPrecise, this.GroundAngle);
      }

      private void RemovePlatformVelocity()
      {
        if (this.GroundVector == null || !this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.Conveyor))
          return;
        Vector2 vector2 = new Vector2();
        this.Velocity -= !(this.GroundVector.Owner is SonicOrca.Core.Objects.IPlatform) ? this.GroundVector.Owner.PositionPrecise - this.GroundVector.Owner.LastPositionPrecise : (this.GroundVector.Owner as SonicOrca.Core.Objects.IPlatform).Velocity;
      }

      private bool IsCrushedBetweenFloors(
        Vector2[][] collisionBox,
        CollisionEvent top,
        CollisionEvent bottom)
      {
        bool flag = false;
        Rectangle rectangle = Rectangle.FromLTRB(collisionBox[0][0].X, collisionBox[0][0].Y, collisionBox[1][0].X, collisionBox[1][0].Y);
        if (top != null)
        {
          Vector2 touch = top.CollisionInfo.Touch;
          if (rectangle.Contains(touch))
            flag = true;
        }
        if (bottom != null)
        {
          Vector2 touch = bottom.CollisionInfo.Touch;
          if (rectangle.Contains(touch))
            flag = true;
        }
        return flag;
      }

      private void CheckCrushed()
      {
        int num = !this._collisionDetected[0] ? 0 : (this._collisionDetected[2] ? 1 : 0);
        bool flag1 = this._collisionDetected[0] && this._collisionDetectedTiles[2];
        bool flag2 = this._collisionDetectedTiles[0] && this._collisionDetected[2];
        bool flag3 = this._collisionDetected[1] && this._collisionDetected[3];
        bool flag4 = false;
        Vector2[][] collisionBox = this.GetCollisionBox(new Vector2((double) (this.CollisionRadius.X - this.LedgeSensorRadius), (double) this.CollisionRadius.Y), false);
        if (num != 0)
        {
          CollisionEvent collisionEvent1 = this._collisionEvents[0];
          CollisionEvent collisionEvent2 = this._collisionEvents[2];
          if (collisionEvent1.CollisionInfo.Vector != null && collisionEvent2.CollisionInfo.Vector != null && (collisionEvent1.CollisionInfo.Vector.Owner != null && collisionEvent2.CollisionInfo.Vector.Owner != null || collisionEvent1.CollisionInfo.Vector.Owner != null && collisionEvent2.CollisionInfo.Vector.Owner == null || collisionEvent1.CollisionInfo.Vector.Owner == null && collisionEvent2.CollisionInfo.Vector.Owner != null) && collisionEvent1.CollisionInfo.Vector.Owner != collisionEvent2.CollisionInfo.Vector.Owner)
          {
            bool flag5 = true;
            if (collisionEvent1.CollisionInfo.Vector.Owner != null)
              flag5 = collisionEvent1.CollisionInfo.Vector.Owner.PositionPrecise != collisionEvent1.CollisionInfo.Vector.Owner.LastPositionPrecise;
            if (collisionEvent2.CollisionInfo.Vector.Owner != null)
              flag5 = collisionEvent2.CollisionInfo.Vector.Owner.PositionPrecise != collisionEvent2.CollisionInfo.Vector.Owner.LastPositionPrecise | flag5;
            if (flag5)
              flag4 = this.IsCrushedBetweenFloors(collisionBox, collisionEvent1, collisionEvent2);
          }
        }
        else if (flag1)
        {
          CollisionEvent collisionEvent = this._collisionEvents[0];
          CollisionEvent collisionEventsTile = this._collisionEventsTiles[2];
          if (collisionEvent.CollisionInfo.Vector != null && collisionEvent.CollisionInfo.Vector.Owner != null && collisionEvent.CollisionInfo.Vector.Owner.PositionPrecise != collisionEvent.CollisionInfo.Vector.Owner.LastPositionPrecise)
            flag4 = this.IsCrushedBetweenFloors(collisionBox, collisionEvent, collisionEventsTile);
        }
        else if (flag2)
        {
          CollisionEvent collisionEventsTile = this._collisionEventsTiles[0];
          CollisionEvent collisionEvent = this._collisionEvents[2];
          if (collisionEvent.CollisionInfo.Vector != null && collisionEvent.CollisionInfo.Vector.Owner != null && collisionEvent.CollisionInfo.Vector.Owner.PositionPrecise != collisionEvent.CollisionInfo.Vector.Owner.LastPositionPrecise)
            flag4 = this.IsCrushedBetweenFloors(collisionBox, collisionEventsTile, collisionEvent);
        }
        if (flag3)
        {
          Rectangle rectangle = Rectangle.FromLTRB(collisionBox[0][0].X, collisionBox[0][0].Y, collisionBox[1][0].X, collisionBox[1][0].Y);
          CollisionEvent collisionEvent3 = this._collisionEvents[1];
          CollisionEvent collisionEvent4 = this._collisionEvents[3];
          if (collisionEvent3 != null && collisionEvent3.CollisionInfo.Vector != null && collisionEvent3.CollisionInfo.Vector.Owner != null && collisionEvent3.CollisionInfo.Vector.Owner.PositionPrecise != collisionEvent3.CollisionInfo.Vector.Owner.LastPositionPrecise)
          {
            Vector2 touch = this._collisionEvents[1].CollisionInfo.Touch;
            if (rectangle.Contains(touch))
              flag4 = true;
          }
          if (collisionEvent4 != null && collisionEvent4.CollisionInfo.Vector != null && collisionEvent4.CollisionInfo.Vector.Owner != null && collisionEvent4.CollisionInfo.Vector.Owner.PositionPrecise != collisionEvent4.CollisionInfo.Vector.Owner.LastPositionPrecise)
          {
            Vector2 touch = this._collisionEvents[3].CollisionInfo.Touch;
            if (rectangle.Contains(touch))
              flag4 = true;
          }
        }
        if (!flag4)
          return;
        this.Kill(PlayerDeathCause.Crushed);
      }

      private static bool IsObjectMovingTowardsCollisionMode(IActiveObject obj, CollisionMode mode)
      {
        Vector2 vector2 = obj.PositionPrecise - obj.LastPositionPrecise;
        switch (mode)
        {
          case CollisionMode.Top:
            return vector2.Y < 0.0;
          case CollisionMode.Left:
            return vector2.X < 0.0;
          case CollisionMode.Bottom:
            return vector2.Y > 0.0;
          case CollisionMode.Right:
            return vector2.X > 0.0;
          default:
            return false;
        }
      }

      public bool CanFly { get; set; }

      public Player Player { get; set; }

      public CharacterInputState Input { get; set; }

      public int MovingDirection => Math.Sign(this.Input.HorizontalDirection);

      public CharacterLookDirection LookDirection { get; set; }

      public int LookDelay { get; set; }

      public double GroundVelocity { get; set; }

      public Vector2 Velocity { get; set; }

      public double Acceleration { get; set; }

      private double Deceleration { get; set; }

      private double Friction { get; set; }

      private double TopSpeed { get; set; }

      private double JumpForce { get; set; }

      private double JumpReleaseForce { get; set; }

      private double Gravity { get; set; }

      public int SlopeLockTicks { get; set; }

      public CharacterEvent CharacterEvents { get; set; }

      public BarrierType Barrier { get; set; }

      public bool HasPerformedBarrierAttack { get; set; }

      public bool HasSuperAbility { get; set; }

      public bool HasHyperAbility { get; set; }

      public bool Jumped { get; set; }

      public CharacterSpecialState SpecialState { get; set; }

      public bool HasBarrier => this.Barrier != 0;

      public IReadOnlyList<CharacterHistoryItem> History
      {
        get => (IReadOnlyList<CharacterHistoryItem>) this._history;
      }

      public bool IsDeadly => this.IsSpinball || this.IsCharging || this.IsInvincible;

      public Character()
      {
        this.Input = new CharacterInputState();
        this.SetDefaultConstants();
      }

      protected override void OnStart()
      {
        ResourceTree resourceTree = this.ResourceTree;
        this.Animation = new AnimationInstance(resourceTree.GetLoadedResource<AnimationGroup>(this.AnimationGroupResourceKey));
        this.SpindashDustGroupResourceKey = "SONICORCA/OBJECTS/SPINDASH/ANIGROUP";
        this.SpindashDustAnimation = new AnimationInstance(resourceTree.GetLoadedResource<AnimationGroup>(this.SpindashDustGroupResourceKey));
        this.BrakeDustResourceKey = "SONICORCA/OBJECTS/DUST";
        this.InvincibilityGroupResourceKey = "SONICORCA/OBJECTS/INVINCIBILITY/ANIGROUP";
        this._drowningAnimation = new AnimationInstance(resourceTree, "SONICORCA/HUD/DROWNING/ANIGROUP");
        this.Priority = 1024 /*0x0400*/;
        this.StateFlags = (CharacterState) 0;
        this.ShowAngle = 0.0;
        this.GroundVelocity = 0.0;
        this.Velocity = new Vector2(0.0, 0.0);
        this.SlopeLockTicks = 0;
        this.LookDirection = CharacterLookDirection.Normal;
        this.BreathTicks = 1800;
        this.CollisionRadius = this.NormalCollisionRadius;
        this.CheckCollision = true;
        this.CheckLandscapeCollision = true;
        this.CheckObjectCollision = true;
        this.InvulnerabilityTicks = 0;
        this.InitialiseInvincibility();
        this.Mode = CollisionMode.Air;
        this._autoSidekickState = Character.AutoSidekickState.Normal;
        this.ShouldReactToLevel = true;
        this.SpecialState = CharacterSpecialState.Normal;
      }

      protected override void OnStop()
      {
      }

      protected override void OnUpdate()
      {
        this.RecordHistory();
        this.HandleInput();
        if (this.IsDebug)
        {
          this.ShouldReactToLevel = false;
          this.UpdateDebugState();
        }
        else if (this.IsDying)
        {
          this.ShouldReactToLevel = false;
          this.UpdateDying();
          this.UpdateWaterLogic();
        }
        else if (this.IsSidekick)
        {
          this.UpdateSidekick();
        }
        else
        {
          this.ShouldReactToLevel = true;
          this.UpdateNormal();
        }
      }

      private void SetDefaultConstants()
      {
        this.LedgeSensorRadius = 9;
        this.NormalCollisionRadius = new Vector2i(40, 60);
        this.RectangleCollisionRadius = new Vector2i(44, 76);
        this.SpinballCollisionRadius = new Vector2i(34, 56);
        this.FloorSensorRadius = 32 /*0x20*/;
        this.CollisionSensorSize = 1;
        this.Path = 1;
      }

      public void HandleInput()
      {
        if (this.IsWinning || this.IsHurt || this.IsObjectControlled)
        {
          this.Input.Clear();
          this.LookDirection = CharacterLookDirection.Normal;
        }
        else
        {
          this.LookDirection = CharacterLookDirection.Normal;
          if (!this.IsAirborne && !this.IsSpinball && this._balanceDirection == CharacterBalanceDirection.None)
          {
            double throttle = this.Input.Throttle;
            double verticalDirection = (double) this.Input.VerticalDirection;
            if (Math.Abs(verticalDirection) > Math.Abs(throttle))
            {
              if (verticalDirection <= -0.5)
                this.LookDirection = CharacterLookDirection.Up;
              else if (verticalDirection >= 0.5)
                this.LookDirection = CharacterLookDirection.Ducking;
              this.Input.Throttle = 0.0;
            }
          }
          if (!this.Level.ClassicDebugMode)
            return;
          if (this.Input.B == CharacterInputButtonState.Pressed)
            this.GroundVelocity += (double) (64 /*0x40*/ * this.Facing);
          if (this.Input.C == CharacterInputButtonState.Pressed)
          {
            this.IsDebug = !this.IsDebug;
            this.LeaveGround();
          }
          this.Input.B = CharacterInputButtonState.Up;
          this.Input.C = CharacterInputButtonState.Up;
        }
      }

      private void RecordHistory()
      {
        Array.Copy((Array) this._history, 0, (Array) this._history, 1, this._history.Length - 1);
        this._history[0] = new CharacterHistoryItem(this.PositionPrecise, this.StateFlags, this.Input);
      }

      private void UpdateNormal()
      {
        if (this.IsWinning)
          this.GroundVelocity = 0.0;
        this.UpdateVariables();
        this.UpdateMovement();
        this.UpdateSpindash();
        this.UpdateSpinning();
        this.UpdateJumping();
        this.UpdateFalling();
        this.UpdatePosition();
        this.UpdateBalance();
        this.UpdateLooking();
        this.UpdatePushing();
        this.UpdateWaterLogic();
        if (this.InvulnerabilityTicks <= 0)
          return;
        --this.InvulnerabilityTicks;
      }

      protected override void OnUpdateCollision()
      {
        if (this.IsSidekick && (this._autoSidekickState == Character.AutoSidekickState.Spawning || this._autoSidekickState == Character.AutoSidekickState.Flying))
          return;
        if (!this.IsDebug && !this.IsDying && !this.IsDead)
          this.ProcessCollision();
        if (!this.IsDying && !this.IsDead)
          this.ApplyLevelBounds();
        this.SetCameraTracking();
        this.CharacterEvents &= ~CharacterEvent.Hurt;
      }

      private void UpdateVariables()
      {
        this.Acceleration = 3.0 / 16.0;
        this.Deceleration = 2.0;
        this.Friction = this.Acceleration;
        this.TopSpeed = 24.0;
        this.JumpForce = 26.0;
        this.JumpReleaseForce = 16.0;
        this.Gravity = 0.875;
        if (this.HasSpeedShoes)
        {
          this.Acceleration = 0.375;
          this.Friction = 0.375;
          this.TopSpeed = 48.0;
        }
        if (this.Mode == CollisionMode.Air)
        {
          if (this.IsRollJumping)
            this.Acceleration = 0.0;
          else
            this.Acceleration *= 2.0;
        }
        else if (this.IsSpinball)
        {
          this.Acceleration = 0.0;
          this.Deceleration = 0.5;
          this.Friction /= 2.0;
        }
        else if (this.MovingDirection != 0)
          this.Friction = 0.0;
        if (this.IsUnderwater)
        {
          this.Acceleration *= 0.5;
          this.Deceleration *= 0.5;
          this.Friction *= 0.5;
          this.TopSpeed *= 0.5;
          this.JumpForce *= 7.0 / 13.0;
          this.JumpReleaseForce *= 0.5;
          this.Gravity = 0.25;
        }
        if (this.IsHurt)
        {
          this.Gravity = 0.75;
          this.Acceleration = 0.0;
          this.Deceleration = 0.0;
        }
        else if (this.IsCharging)
        {
          this.Acceleration = 0.0;
          this.Deceleration = 0.0;
        }
        if (this.SlopeLockTicks == 0)
          return;
        if (this.Mode == CollisionMode.Air)
        {
          this.SlopeLockTicks = 0;
        }
        else
        {
          --this.SlopeLockTicks;
          this.Deceleration = 0.0;
        }
      }

      private void UpdateMovement()
      {
        if (this.Mode == CollisionMode.Air)
        {
          Vector2 velocity = this.Velocity;
          if (this.MovingDirection != 0)
          {
            velocity.X = Character.ChangeSpeed(velocity.X, this.MovingDirection, this.TopSpeed, this.Acceleration, this.Acceleration, 0.0);
            this.Facing = this.MovingDirection;
          }
          velocity.Y = Character.ChangeSpeed(velocity.Y, 1, 64.0, this.Gravity, this.Gravity, 0.0);
          if (velocity.Y < 0.0 && velocity.Y > -this.JumpReleaseForce)
            velocity.X *= 31.0 / 32.0;
          this.Velocity = velocity;
        }
        else
        {
          if (!this.IsCharging)
            this.ApplySlopeFriction();
          this.GroundVelocity = Character.ChangeSpeed(this.GroundVelocity, this.MovingDirection, this.TopSpeed, this.Acceleration, this.Deceleration, this.Friction);
          if ((double) this.MovingDirection * this.GroundVelocity > 0.0)
            this.Facing = this.MovingDirection;
          this.UpdateBraking();
        }
      }

      private void UpdateBraking()
      {
        if (this.MovingDirection == Math.Sign(this.GroundVelocity) || this.GroundVelocity == 0.0 || this.Acceleration == 0.0 || this.IsSpinball || this.SlopeLockTicks > 0)
        {
          this.IsBraking = false;
        }
        else
        {
          if (!this.IsBraking && this.MovingDirection != 0 && Math.Abs(this.GroundVelocity) >= 16.0)
          {
            this.IsBraking = true;
            this._brakeDustDelay = 0;
            this._brakeTicks = 33;
            this.Level.SoundManager.PlaySound((IActiveObject) this, "SONICORCA/SOUND/BRAKE");
          }
          if (!this.IsBraking)
            return;
          --this._brakeTicks;
          if (this._brakeTicks == 0)
          {
            this.IsBraking = false;
          }
          else
          {
            --this._brakeDustDelay;
            if (this._brakeDustDelay <= 0)
            {
              this._brakeDustDelay = 6;
              ObjectManager objectManager = this.Level.ObjectManager;
              string brakeDustResourceKey = this.BrakeDustResourceKey;
              int layer = this.Level.Map.Layers.IndexOf(this.Layer);
              int x = this.Position.X;
              Vector2i vector2i = this.Position;
              int y1 = vector2i.Y;
              vector2i = this.CollisionRadius;
              int y2 = vector2i.Y;
              int y3 = y1 + y2;
              Vector2i position = new Vector2i(x, y3);
              ObjectPlacement objectPlacement = new ObjectPlacement(brakeDustResourceKey, layer, position);
              objectManager.AddObject(objectPlacement);
            }
            if (Math.Abs(this.GroundVelocity) >= 4.0)
              return;
            this.IsBraking = false;
          }
        }
      }

      private void ApplySlopeFriction()
      {
        if (this.GroundVector == null || this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.NoAngle) || this.GroundAngle == 0.0)
          return;
        double num1 = -Math.Sin(this.GroundAngle);
        double num2;
        if (this.IsSpinball)
        {
          num2 = this.GroundVelocity * num1 <= 0.0 ? num1 * 1.25 : num1 * 0.2873;
        }
        else
        {
          num2 = num1 * 0.5;
          if (this.GroundVelocity == 0.0 && Math.Abs(num2) <= 0.05)
            return;
        }
        double groundVelocity = this.GroundVelocity;
        this.GroundVelocity -= num2;
      }

      private void UpdateSpindash()
      {
        if (this.IsCharging)
        {
          this.LookDelay = 120;
          this._spindashExtraSpeed *= 31.0 / 32.0;
          if (this.Input.ABC == CharacterInputButtonState.Pressed)
          {
            this._spindashExtraSpeed = Math.Min(this._spindashExtraSpeed + 4.0, 16.0);
            this.Level.SoundManager.PlaySound((IActiveObject) this, "SONICORCA/SOUND/SPINDASH/CHARGE");
          }
          if (this.LookDirection == CharacterLookDirection.Ducking)
            return;
          this.IsCharging = false;
          this.GroundVelocity = (32.0 + this._spindashExtraSpeed) * (double) this.Facing;
          this.CharacterEvents |= CharacterEvent.SpindashLaunch;
          this.CameraProperties.Delay = new Vector2i(16 /*0x10*/, this.CameraProperties.Delay.Y);
          this.Level.SoundManager.PlaySound((IActiveObject) this, "SONICORCA/SOUND/SPINDASH/RELEASE");
        }
        else
        {
          if (this.IsSpinball || this.Mode == CollisionMode.Air || this.LookDirection != CharacterLookDirection.Ducking || this.Input.ABC != CharacterInputButtonState.Pressed)
            return;
          this.IsCharging = true;
          this._spindashExtraSpeed = 0.0;
          this.Level.SoundManager.PlaySound((IActiveObject) this, "SONICORCA/SOUND/SPINDASH/CHARGE");
        }
      }

      private void UpdateSpinning()
      {
        if (this.Mode == CollisionMode.Air)
          return;
        if (this.GroundVector != null && this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.ForceRoll))
          this.UpdateSpinningForced();
        else if (this.IsSpinball)
        {
          if (Math.Abs(this.GroundVelocity) >= 2.0)
            return;
          this.IsSpinball = false;
        }
        else
        {
          if (this.LookDirection != CharacterLookDirection.Ducking && !this.CharacterEvents.HasFlag((Enum) CharacterEvent.SpindashLaunch) || Math.Abs(this.GroundVelocity) <= 4.0)
            return;
          this.IsSpinball = true;
          if (this.CharacterEvents.HasFlag((Enum) CharacterEvent.SpindashLaunch))
            this.CharacterEvents &= ~CharacterEvent.SpindashLaunch;
          else
            this.Level.SoundManager.PlaySound((IActiveObject) this, "SONICORCA/SOUND/SPINBALL");
        }
      }

      private void UpdateSpinningForced()
      {
        if (!this.IsSpinball)
        {
          this.IsSpinball = true;
          this.Level.SoundManager.PlaySound((IActiveObject) this, "SONICORCA/SOUND/SPINBALL");
        }
        if (Math.Abs(this.GroundVelocity) >= 2.0 || this.ShouldFallOffGround())
          return;
        int num = Math.Sign(this.GroundVelocity);
        if (num == 0)
          num = this.Facing;
        this.GroundVelocity = (double) (16 /*0x10*/ * num);
      }

      private void UpdateJumping()
      {
        if (this.IsHurt)
          return;
        if (this.IsJumping)
        {
          if (this.Input.ABC.HasFlag((Enum) CharacterInputButtonState.Down))
            return;
          Vector2 velocity = this.Velocity;
          if (velocity.Y < -this.JumpReleaseForce)
          {
            velocity = this.Velocity;
            this.Velocity = new Vector2(velocity.X, -this.JumpReleaseForce);
          }
          this.IsJumping = false;
        }
        else
        {
          if (this.Input.ABC != CharacterInputButtonState.Pressed || this.IsCharging)
            return;
          if (this.Mode == CollisionMode.Air)
          {
            if (!this.Jumped)
              return;
            if (this.CanFly && this._humanControlled)
              this.CharacterEvents |= CharacterEvent.Fly;
            else if (this.Barrier != BarrierType.None && this.Barrier != BarrierType.Classic)
            {
              if (this.HasPerformedBarrierAttack)
                return;
              this.HasPerformedBarrierAttack = true;
              this.CharacterEvents |= CharacterEvent.BarrierAttack;
            }
            else
            {
              if (!this.Level.ClassicDebugMode || !this.IsSpinball)
                return;
              this.IsJumping = true;
              this.CharacterEvents |= CharacterEvent.DoubleJump;
              this.Level.SoundManager.PlaySound((IActiveObject) this, "SONICORCA/SOUND/JUMP");
            }
          }
          else
          {
            if (this.GroundVector == null || this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.PreventJump))
              return;
            this.Jumped = true;
            this.IsJumping = true;
            if (!this.IsAirborne && this.IsSpinball)
              this.IsRollJumping = true;
            this.IsSpinball = true;
            this.CharacterEvents |= CharacterEvent.Fall;
            this.CharacterEvents |= CharacterEvent.Jump;
            this.Level.SoundManager.PlaySound((IActiveObject) this, "SONICORCA/SOUND/JUMP");
          }
        }
      }

      private void UpdateFalling()
      {
        if (this.Mode == CollisionMode.Top || this.Mode == CollisionMode.Air || Math.Abs(this.GroundVelocity) >= 10.0)
          return;
        if ((this.GroundVector == null ? 0 : (this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.DontFall) ? 1 : 0)) == 0 && this.ShouldFallOffGround())
        {
          this.CharacterEvents |= CharacterEvent.Fall;
        }
        else
        {
          if (this.SlopeLockTicks != 0)
            return;
          this.SlopeLockTicks = 32 /*0x20*/;
        }
      }

      private bool ShouldFallOffGround() => Math.Cos(this.GroundAngle) <= 0.0001;

      private void UpdatePosition()
      {
        if (this.Mode != CollisionMode.Air)
        {
          this.GroundVelocity = (double) Math.Sign(this.GroundVelocity) * Math.Min(Math.Abs(this.GroundVelocity), 96.0);
          this.Velocity = new Vector2(MathX.Clamp(-64.0, this.GroundVelocity * Math.Cos(this.GroundAngle), 64.0), MathX.Clamp(-64.0, this.GroundVelocity * Math.Sin(this.GroundAngle), 64.0));
          this.AddPlatformVelocity();
          if (this.CharacterEvents.HasFlag((Enum) CharacterEvent.Jump))
          {
            double num = this.GroundAngle;
            if (this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.NoAngle))
              num = 0.0;
            this.Velocity += new Vector2(this.JumpForce * Math.Sin(num), this.JumpForce * -Math.Cos(num));
            this.CharacterEvents &= ~CharacterEvent.Jump;
          }
          if (this.CharacterEvents.HasFlag((Enum) CharacterEvent.Fall))
          {
            this.LeaveGround();
            this.CharacterEvents &= ~CharacterEvent.Fall;
          }
        }
        this.CharacterEvents.HasFlag((Enum) CharacterEvent.Fly);
        Vector2 velocity;
        if (this.CharacterEvents.HasFlag((Enum) CharacterEvent.BarrierAttack))
        {
          switch (this.Barrier)
          {
            case BarrierType.Bubble:
              this.Velocity = new Vector2(0.0, 32.0);
              break;
            case BarrierType.Fire:
              this.Velocity = new Vector2((double) (32 /*0x20*/ * this.Facing), 0.0);
              break;
            case BarrierType.Lightning:
              velocity = this.Velocity;
              this.Velocity = new Vector2(velocity.X, -22.0);
              break;
          }
          this.CharacterEvents &= ~CharacterEvent.BarrierAttack;
        }
        if (this.CharacterEvents.HasFlag((Enum) CharacterEvent.DoubleJump))
        {
          velocity = this.Velocity;
          double x = velocity.X;
          velocity = this.Velocity;
          double y = Math.Min(velocity.Y, -this.JumpForce);
          this.Velocity = new Vector2(x, y);
          this.CharacterEvents &= ~CharacterEvent.DoubleJump;
        }
        velocity = this.Velocity;
        double num1 = (double) Math.Sign(velocity.X);
        velocity = this.Velocity;
        double num2 = Math.Min(Math.Abs(velocity.X), 96.0);
        double x1 = num1 * num2;
        velocity = this.Velocity;
        double num3 = (double) Math.Sign(velocity.Y);
        velocity = this.Velocity;
        double num4 = Math.Min(Math.Abs(velocity.Y), 96.0);
        double y1 = num3 * num4;
        this.Velocity = new Vector2(x1, y1);
        bool flag1 = this._collisionEvents[3] != null;
        bool flag2 = this._collisionEvents[1] != null;
        double y2 = 0.0;
        bool platform = this.IsAttachedToPlatform();
        if (this.IsPushing && !platform)
        {
          if (flag1 && this._collisionEvents[3].CollisionInfo.Vector.Flags.HasFlag((Enum) CollisionFlags.Mobile))
          {
            velocity = this.Velocity;
            y2 = velocity.Y;
          }
          if (flag2 && this._collisionEvents[1].CollisionInfo.Vector.Flags.HasFlag((Enum) CollisionFlags.Mobile))
          {
            velocity = this.Velocity;
            y2 = velocity.Y;
          }
        }
        else
        {
          velocity = this.Velocity;
          y2 = velocity.Y;
        }
        Vector2 positionPrecise = this.PositionPrecise;
        velocity = this.Velocity;
        Vector2 vector2 = new Vector2(velocity.X, y2);
        this.PositionPrecise = positionPrecise + vector2;
      }

      private void UpdateBalance()
      {
        if (this.Mode == CollisionMode.Air || this.GroundVelocity != 0.0)
          this._balanceDirection = CharacterBalanceDirection.None;
        else if (this.GroundVector.Flags.HasFlag((Enum) CollisionFlags.NoBalance))
        {
          this._balanceDirection = CharacterBalanceDirection.None;
        }
        else
        {
          int num1 = Math.Sign(this.DistanceFromLedge);
          double num2 = Math.Abs(this.DistanceFromLedge);
          if (num2 > (double) this.LedgeSensorRadius)
          {
            this._balanceDirection = (CharacterBalanceDirection) (2 * num1);
            this.Facing = num1;
          }
          else if (num2 > 0.0)
            this._balanceDirection = (CharacterBalanceDirection) num1;
          else
            this._balanceDirection = CharacterBalanceDirection.None;
        }
      }

      private void UpdateLooking()
      {
        if ((this._balanceDirection != CharacterBalanceDirection.None || this.GroundVelocity != 0.0) && this.LookDirection == CharacterLookDirection.Up)
          this.LookDirection = CharacterLookDirection.Normal;
        Vector2 offset = this.CameraProperties.Offset;
        switch (this.LookDirection)
        {
          case CharacterLookDirection.Up:
            this.LookDelay = Math.Max(0, this.LookDelay - 1);
            offset.Y = this.LookDelay == 0 ? Math.Max(-344.0, offset.Y - 8.0) : MathX.ChangeSpeed(offset.Y, -8.0);
            break;
          case CharacterLookDirection.Normal:
            this.LookDelay = 120;
            offset.Y = MathX.ChangeSpeed(offset.Y, -8.0);
            break;
          case CharacterLookDirection.Ducking:
            this.LookDelay = Math.Max(0, this.LookDelay - 1);
            offset.Y = this.LookDelay == 0 ? Math.Min(352.0, offset.Y + 8.0) : MathX.ChangeSpeed(offset.Y, -8.0);
            break;
        }
        this.CameraProperties.Offset = offset;
      }

      private void UpdatePushing()
      {
        if (this.Mode == CollisionMode.Air || this.MovingDirection == 0 || this.SlopeLockTicks > 0)
        {
          this.IsPushing = false;
          this._pushTicks = 0;
        }
        else if (this.Mode != CollisionMode.Top)
        {
          this.IsPushing = false;
          this._pushTicks = 0;
          this._pushDirection = 0;
        }
        else
        {
          bool flag1 = this._collisionEvents[3] != null;
          bool flag2 = this._collisionEvents[1] != null;
          if (this.MovingDirection == -1 & flag1 || this.MovingDirection == 1 & flag2)
          {
            this.IsPushing = true;
            this._pushDirection = this.MovingDirection;
            this._pushTicks = 12;
          }
          else if (this.MovingDirection != this._pushDirection)
          {
            this.IsPushing = false;
            this._pushTicks = 0;
            this._pushDirection = 0;
          }
          else
          {
            --this._pushTicks;
            if (this._pushTicks != 0)
              return;
            this.IsPushing = false;
            this._pushTicks = 0;
            this._pushDirection = 0;
          }
        }
      }

      private static double ChangeSpeed(
        double speed,
        int direction,
        double limit,
        double acceleration,
        double deceleration,
        double friction)
      {
        if (direction != 0)
        {
          double num = limit - speed * (double) direction;
          if (num > limit)
            speed += deceleration * (double) direction;
          else if (num > acceleration)
            speed += acceleration * (double) direction;
          else if (num > 0.0)
            speed = limit * (double) direction;
        }
        if (friction > 0.0)
        {
          if (speed > friction)
            speed -= friction;
          else if (speed < -friction)
            speed += friction;
          else
            speed = 0.0;
        }
        return speed;
      }

      private void ApplyLevelBounds()
      {
        Vector2 vector2;
        if (this.Position.X - 36 < this.Level.Bounds.X)
        {
          double x = (double) (this.Level.Bounds.X + 36);
          vector2 = this.PositionPrecise;
          double y = vector2.Y;
          this.PositionPrecise = new Vector2(x, y);
          vector2 = this.Velocity;
          this.Velocity = new Vector2(0.0, vector2.Y);
          this.GroundVelocity = MathX.Clamp(-1.0, this.GroundVelocity, 0.0);
        }
        else if (this.Position.X + 36 > this.Level.Bounds.Right - 1)
        {
          this.PositionPrecise = new Vector2((double) (this.Level.Bounds.Right - 1 - 36), this.PositionPrecise.Y);
          this.Velocity = new Vector2(0.0, this.Velocity.Y);
          this.GroundVelocity = MathX.Clamp(0.0, this.GroundVelocity, 1.0);
        }
        if (this.IsDebug)
        {
          Vector2i position = this.Position;
          if (position.Y >= this.Level.Bounds.Bottom)
          {
            vector2 = this.PositionPrecise;
            this.PositionPrecise = new Vector2(vector2.X, (double) this.Level.Bounds.Bottom);
            vector2 = this.Velocity;
            this.Velocity = new Vector2(vector2.X, 0.0);
          }
          position = this.Position;
          if (position.Y >= this.Level.Bounds.Top)
            return;
          vector2 = this.PositionPrecise;
          this.PositionPrecise = new Vector2(vector2.X, (double) this.Level.Bounds.Top);
          vector2 = this.Velocity;
          this.Velocity = new Vector2(vector2.X, 0.0);
        }
        else
        {
          if (this.Position.Y < this.Level.Bounds.Bottom)
            return;
          this.Kill(PlayerDeathCause.BottomlessPit);
        }
      }

      private void SetCameraTracking()
      {
        CameraProperties cameraProperties1 = this.CameraProperties;
        cameraProperties1.MaxVelocity = new Vector2(64.0, cameraProperties1.MaxVelocity.Y);
        Vector2 vector2_1;
        if (this.IsAirborne)
        {
          cameraProperties1.Box = new Rectangle(-64.0, -192.0, 64.0, 256.0);
          CameraProperties cameraProperties2 = cameraProperties1;
          vector2_1 = cameraProperties1.MaxVelocity;
          Vector2 vector2_2 = new Vector2(vector2_1.X, 64.0);
          cameraProperties2.MaxVelocity = vector2_2;
        }
        else
        {
          Rectangle rectangle = new Rectangle(-64.0, -16.0, 64.0, 0.0);
          if (this.IsSpinball)
            rectangle.Y += (double) (this.NormalCollisionRadius.Y - this.SpinballCollisionRadius.Y);
          cameraProperties1.Box = rectangle;
          CameraProperties cameraProperties3 = cameraProperties1;
          Vector2 vector2_3 = cameraProperties1.MaxVelocity;
          double x = vector2_3.X;
          vector2_3 = this.Velocity;
          double y = vector2_3.Y <= 6.0 ? 24.0 : 64.0;
          Vector2 vector2_4 = new Vector2(x, y);
          cameraProperties3.MaxVelocity = vector2_4;
        }
        if (!this.IsDebug)
          return;
        CameraProperties cameraProperties4 = cameraProperties1;
        vector2_1 = this.Velocity;
        double x1 = Math.Max(64.0, Math.Abs(vector2_1.X));
        vector2_1 = this.Velocity;
        double y1 = Math.Max(64.0, Math.Abs(vector2_1.Y));
        Vector2 vector2_5 = new Vector2(x1, y1);
        cameraProperties4.MaxVelocity = vector2_5;
      }

      private void UpdateDebugState()
      {
        Controller controller = this.Level.GameContext.Current[this.Player.ProtagonistGamepadIndex];
        Vector2 velocity = this.Velocity;
        Vector2 vector2;
        if (controller.DirectionLeft.X == 0.0)
        {
          ref Vector2 local = ref velocity;
          vector2 = this.Velocity;
          double num = MathX.ChangeSpeed(vector2.X, -3.0 / 16.0);
          local.X = num;
        }
        else
          velocity.X += controller.DirectionLeft.X * (3.0 / 16.0) * 2.0;
        vector2 = controller.DirectionLeft;
        if (vector2.Y == 0.0)
        {
          ref Vector2 local = ref velocity;
          vector2 = this.Velocity;
          double num = MathX.ChangeSpeed(vector2.Y, -3.0 / 16.0);
          local.Y = num;
        }
        else
        {
          ref Vector2 local = ref velocity;
          double y = local.Y;
          vector2 = controller.DirectionLeft;
          double num = vector2.Y * (3.0 / 16.0) * 2.0;
          local.Y = y + num;
        }
        this.Velocity = velocity;
        this.UpdatePosition();
        this.ApplyLevelBounds();
        this.LookDelay = 120;
        this.CameraProperties.Delay = new Vector2i(0, 0);
        this.BreathTicks = 1800;
        if (!this.IsUnderwater)
          return;
        this.LeaveWater();
      }

      public void DrawDebugInfo(Renderer renderer)
      {
        double y1 = 512.0;
        this.Level.DebugContext.DrawText(renderer, string.Join("  ", (object) (char) (this.IsFacingLeft ? 76 : 82), (object) (char) (this.IsBraking ? 66 : 32 /*0x20*/), (object) (char) (this.IsPushing ? 80 /*0x50*/ : 32 /*0x20*/), (object) (char) (this.IsAirborne ? 65 : 32 /*0x20*/), (object) (char) (this.IsSpinball ? 83 : 32 /*0x20*/), (object) (char) (this.IsRollJumping ? 82 : 32 /*0x20*/), (object) (char) (this.IsDying ? 68 : 32 /*0x20*/), (object) (char) (this.IsFlying ? 70 : 32 /*0x20*/), (object) (char) (this.ExhibitsVirtualPlatform ? 86 : 32 /*0x20*/), this.HasSpeedShoes ? (object) "SS" : (object) "  ", (object) (char) (this.IsInvincible ? 73 : 32 /*0x20*/)), FontAlignment.MiddleX, 960.0, 16.0, 0.75, new int?(0));
        double y2 = y1 + this.Level.DebugContext.DrawText(renderer, $"LAYER: {this.Level.Map.Layers.IndexOf(this.Layer)}", FontAlignment.Left, 8.0, y1, 0.75, new int?(0));
        double y3 = y2 + this.Level.DebugContext.DrawText(renderer, $"ANGLE: {this.GroundAngle:0.00} TUMBLE: {this.TumbleAngle:0.00}", FontAlignment.Left, 8.0, y2, 0.75, new int?(0));
        double y4 = y3 + this.Level.DebugContext.DrawText(renderer, $"POSITION: {this.PositionPrecise.X:0.00}, {this.PositionPrecise.Y:0.00}", FontAlignment.Left, 8.0, y3, 0.75, new int?(0));
        double y5 = y4 + this.Level.DebugContext.DrawText(renderer, $"VELOCITY: {this.Velocity.X:0.00}, {this.Velocity.Y:0.00}", FontAlignment.Left, 8.0, y4, 0.75, new int?(0));
        if (!this.IsAirborne)
          y5 += this.Level.DebugContext.DrawText(renderer, $"GROUND VELOCITY: {this.GroundVelocity:0.00}", FontAlignment.Left, 8.0, y5, 0.75, new int?(0));
        if (this.SlopeLockTicks > 0)
          y5 += this.Level.DebugContext.DrawText(renderer, $"SLOPE LOCK: {this.SlopeLockTicks}", FontAlignment.Left, 8.0, y5, 0.75, new int?(0));
        if (this.LookDelay < 120)
          y5 += this.Level.DebugContext.DrawText(renderer, $"LOOK DELAY: {this.LookDelay}", FontAlignment.Left, 8.0, y5, 0.75, new int?(0));
        if (this.IsCharging)
          y5 += this.Level.DebugContext.DrawText(renderer, $"SPINDASH: {32.0 + this._spindashExtraSpeed}", FontAlignment.Left, 8.0, y5, 0.75, new int?(0));
        if (this.InvulnerabilityTicks > 0)
          y5 += this.Level.DebugContext.DrawText(renderer, $"INVULNERABILITY: {this.InvulnerabilityTicks}", FontAlignment.Left, 8.0, y5, 0.75, new int?(0));
        if (this.BreathTicks >= 1800)
          return;
        double num = y5 + this.Level.DebugContext.DrawText(renderer, $"DROWNING: {this.BreathTicks}", FontAlignment.Left, 8.0, y5, 0.75, new int?(0));
      }

      protected internal void DrawNewCollisionDebug(Renderer renderer)
      {
        if (this.IsSidekick)
          return;
        string[] strArray = new string[7]
        {
          "MODE: " + (object) this.Mode,
          $"POSITION: {(object) this.Position.X}, {(object) this.Position.Y}",
          $"PRECISE: {(object) (this.PositionPrecise.X - (double) this.Position.X)}, {(object) (this.PositionPrecise.Y - (double) this.Position.Y)}",
          $"VELOCITY: {(object) (int) this.Velocity.X}, {(object) (int) this.Velocity.Y}",
          "GROUND VELOCITY: " + (object) (int) this.GroundVelocity,
          "GROUND ID: " + (object) this.Level.Map.CollisionVectors.IndexOf(this.GroundVector),
          "PATH: " + (object) this.Path
        };
        I2dRenderer obj1 = renderer.Get2dRenderer();
        using (obj1.BeginMatixState())
        {
          obj1.ModelMatrix = Matrix4.Identity;
          double y1 = 16.0;
          foreach (string str in strArray)
            y1 += this.Level.DebugContext.DrawText(renderer, str.ToUpper(), FontAlignment.Right, 1904.0, y1, 0.5, new int?(0));
          obj1.ModelMatrix = obj1.ModelMatrix.Translate(-this.Level.Camera.Bounds.X, -this.Level.Camera.Bounds.Y);
          Vector2i vector2i;
          if ((this.Mode == CollisionMode.Top || this.Mode == CollisionMode.Bottom) && this.GroundVector != null)
          {
            I2dRenderer obj2 = obj1;
            Colour blue = Colours.Blue;
            Vector2 a1 = new Vector2((double) this.GroundVector.AbsoluteA.X, 0.0);
            Vector2i absoluteA1 = this.GroundVector.AbsoluteA;
            Vector2 b1 = new Vector2((double) absoluteA1.X, 10000.0);
            obj2.RenderLine(blue, a1, b1, 8.0);
            I2dRenderer obj3 = obj1;
            Colour yellow = Colours.Yellow;
            absoluteA1 = this.GroundVector.AbsoluteA;
            Vector2 a2 = new Vector2((double) absoluteA1.X, 0.0);
            absoluteA1 = this.GroundVector.AbsoluteA;
            Vector2 b2 = new Vector2((double) absoluteA1.X, 10000.0);
            obj3.RenderLine(yellow, a2, b2, 4.0);
            obj1.RenderLine(Colours.Blue, new Vector2((double) this.GroundVector.AbsoluteB.X, 0.0), new Vector2((double) this.GroundVector.AbsoluteB.X, 10000.0), 8.0);
            obj1.RenderLine(Colours.Yellow, new Vector2((double) this.GroundVector.AbsoluteB.X, 0.0), new Vector2((double) this.GroundVector.AbsoluteB.X, 10000.0), 4.0);
            string str1 = $"{{X: {(object) this.GroundVector.AbsoluteA.X}, Y: {(object) this.GroundVector.AbsoluteA.Y}}}";
            DebugContext debugContext1 = this.Level.DebugContext;
            Renderer renderer1 = renderer;
            string upper1 = str1.ToUpper();
            Vector2i absoluteA2 = this.GroundVector.AbsoluteA;
            double x1 = (double) (absoluteA2.X - 128 /*0x80*/);
            absoluteA2 = this.GroundVector.AbsoluteA;
            double y2 = (double) (absoluteA2.Y - 128 /*0x80*/);
            int? overlay1 = new int?(0);
            debugContext1.DrawText(renderer1, upper1, FontAlignment.Right, x1, y2, 0.5, overlay1);
            string str2 = $"{{X: {(object) this.GroundVector.AbsoluteB.X}, Y: {(object) this.GroundVector.AbsoluteB.Y}}}";
            DebugContext debugContext2 = this.Level.DebugContext;
            Renderer renderer2 = renderer;
            string upper2 = str2.ToUpper();
            vector2i = this.GroundVector.AbsoluteB;
            double x2 = (double) (vector2i.X + 256 /*0x0100*/);
            vector2i = this.GroundVector.AbsoluteB;
            double y3 = (double) (vector2i.Y - 128 /*0x80*/);
            int? overlay2 = new int?(0);
            debugContext2.DrawText(renderer2, upper2, FontAlignment.Right, x2, y3, 0.5, overlay2);
          }
          else if ((this.Mode == CollisionMode.Left || this.Mode == CollisionMode.Right) && this.GroundVector != null)
          {
            I2dRenderer obj4 = obj1;
            Colour blue = Colours.Blue;
            Vector2 a3 = new Vector2(0.0, (double) this.GroundVector.AbsoluteA.Y);
            Vector2i absoluteA = this.GroundVector.AbsoluteA;
            Vector2 b3 = new Vector2(100000.0, (double) absoluteA.Y);
            obj4.RenderLine(blue, a3, b3, 8.0);
            I2dRenderer obj5 = obj1;
            Colour yellow1 = Colours.Yellow;
            absoluteA = this.GroundVector.AbsoluteA;
            Vector2 a4 = new Vector2(0.0, (double) absoluteA.Y);
            absoluteA = this.GroundVector.AbsoluteA;
            Vector2 b4 = new Vector2(100000.0, (double) absoluteA.Y);
            obj5.RenderLine(yellow1, a4, b4, 4.0);
            obj1.RenderLine(Colours.Blue, new Vector2(0.0, (double) this.GroundVector.AbsoluteB.Y), new Vector2(100000.0, (double) this.GroundVector.AbsoluteB.Y), 8.0);
            I2dRenderer obj6 = obj1;
            Colour yellow2 = Colours.Yellow;
            vector2i = this.GroundVector.AbsoluteB;
            Vector2 a5 = new Vector2(0.0, (double) vector2i.Y);
            vector2i = this.GroundVector.AbsoluteB;
            Vector2 b5 = new Vector2(100000.0, (double) vector2i.Y);
            obj6.RenderLine(yellow2, a5, b5, 4.0);
          }
          foreach (Vector2[] vector2Array in this.GetCollisionBox((Vector2) this.CollisionRadius, true))
            ;
          Vector2 positionPrecise1 = this.PositionPrecise;
          double x3 = positionPrecise1.X;
          vector2i = this.CollisionRadius;
          double x4 = (double) vector2i.X;
          double x5 = x3 - x4;
          positionPrecise1 = this.PositionPrecise;
          double y4 = positionPrecise1.Y;
          vector2i = this.CollisionRadius;
          double y5 = (double) vector2i.Y;
          double y6 = y4 - y5;
          vector2i = this.CollisionRadius;
          double width = (double) (vector2i.X * 2);
          vector2i = this.CollisionRadius;
          double height = (double) (vector2i.Y * 2);
          Rectangle rectangle = new Rectangle(x5, y6, width, height);
          bool flag = false;
          double num1;
          double y7;
          if (this.Mode == CollisionMode.Left || this.Mode == CollisionMode.Right)
          {
            vector2i = this.CollisionRadius;
            num1 = (double) vector2i.Y;
            vector2i = this.CollisionRadius;
            y7 = (double) vector2i.X;
            flag = true;
          }
          else
          {
            vector2i = this.CollisionRadius;
            num1 = (double) vector2i.X;
            vector2i = this.CollisionRadius;
            y7 = (double) vector2i.Y;
          }
          Vector2 positionPrecise2 = this.PositionPrecise;
          positionPrecise1 = this.PositionPrecise;
          double x6 = positionPrecise1.X;
          positionPrecise1 = this.PositionPrecise;
          double y8 = positionPrecise1.Y + num1;
          Vector2 point1 = new Vector2(x6, y8);
          double theta1 = Math.PI / 2.0 + this.GroundAngle;
          Vector2 rotatedFromRelative1 = this.GetPointRotatedFromRelative(positionPrecise2, point1, theta1);
          Vector2 positionPrecise3 = this.PositionPrecise;
          positionPrecise1 = this.PositionPrecise;
          double x7 = positionPrecise1.X;
          positionPrecise1 = this.PositionPrecise;
          double y9 = positionPrecise1.Y - num1;
          Vector2 point2 = new Vector2(x7, y9);
          double theta2 = Math.PI / 2.0 + this.GroundAngle;
          Vector2 rotatedFromRelative2 = this.GetPointRotatedFromRelative(positionPrecise3, point2, theta2);
          Vector2 positionPrecise4 = this.PositionPrecise;
          positionPrecise1 = this.PositionPrecise;
          double x8 = positionPrecise1.X - y7;
          positionPrecise1 = this.PositionPrecise;
          double y10 = positionPrecise1.Y;
          Vector2 point3 = new Vector2(x8, y10);
          double theta3 = Math.PI / 2.0 + this.GroundAngle;
          Vector2 rotatedFromRelative3 = this.GetPointRotatedFromRelative(positionPrecise4, point3, theta3);
          Vector2 positionPrecise5 = this.PositionPrecise;
          positionPrecise1 = this.PositionPrecise;
          double x9 = positionPrecise1.X + y7;
          positionPrecise1 = this.PositionPrecise;
          double y11 = positionPrecise1.Y;
          Vector2 point4 = new Vector2(x9, y11);
          double theta4 = Math.PI / 2.0 + this.GroundAngle;
          Vector2 rotatedFromRelative4 = this.GetPointRotatedFromRelative(positionPrecise5, point4, theta4);
          Vector2[][] collisionBox1 = this.GetCollisionBox(new Vector2(num1 - (double) this.LedgeSensorRadius, y7 - (double) this.FloorSensorRadius + 20.0), false);
          Vector2[][] collisionBox2 = this.GetCollisionBox(new Vector2(num1 - (double) this.LedgeSensorRadius, y7), false);
          Vector2[][] vector2Array1 = new Vector2[0][];
          Vector2[][] vector2Array2 = new Vector2[0][];
          Vector2 vector2_1 = new Vector2();
          Vector2 vector2_2 = new Vector2();
          Vector2 vector2_3 = new Vector2();
          Vector2 vector2_4 = new Vector2();
          Vector2[][] collection1;
          Vector2[][] collection2;
          Vector2 a6;
          Vector2 b6;
          Vector2 a7;
          Vector2 b7;
          if (!flag)
          {
            collection1 = collisionBox1;
            collection2 = collisionBox2;
            a6 = rotatedFromRelative1;
            b6 = rotatedFromRelative2;
            a7 = rotatedFromRelative3;
            b7 = rotatedFromRelative4;
          }
          else
          {
            collection1 = collisionBox2;
            collection2 = collisionBox1;
            a6 = rotatedFromRelative3;
            b6 = rotatedFromRelative4;
            a7 = rotatedFromRelative1;
            b7 = rotatedFromRelative2;
          }
          obj1.RenderLine(Colour.ParseHex("00FF00"), a6, b6, 5.0);
          obj1.RenderLine(Colour.ParseHex("00FF00"), a7, b7, 5.0);
          foreach (CollisionVector collisionVector in (IEnumerable<CollisionVector>) this.Level.Map.CollisionVectors)
          {
            if (this.Level.Camera.Bounds.IntersectsWith((Rectangle) collisionVector.Bounds))
            {
              int num2 = 6;
              obj1.RenderLine(Colour.ParseHex("FF0000"), (Vector2) collisionVector.AbsoluteA, (Vector2) collisionVector.AbsoluteB, 5.0);
              I2dRenderer obj7 = obj1;
              Colour hex1 = Colour.ParseHex("FA35FF");
              vector2i = collisionVector.AbsoluteA;
              double left1 = (double) (vector2i.X - num2);
              vector2i = collisionVector.AbsoluteA;
              double top1 = (double) (vector2i.Y - num2);
              vector2i = collisionVector.AbsoluteA;
              double right1 = (double) (vector2i.X + num2);
              vector2i = collisionVector.AbsoluteA;
              double bottom1 = (double) (vector2i.Y + num2);
              Rectangle destination1 = Rectangle.FromLTRB(left1, top1, right1, bottom1);
              double tickness1 = (double) (num2 * 4);
              obj7.RenderRectangle(hex1, destination1, tickness1);
              I2dRenderer obj8 = obj1;
              Colour hex2 = Colour.ParseHex("FA35FF");
              vector2i = collisionVector.AbsoluteB;
              double left2 = (double) (vector2i.X - num2);
              vector2i = collisionVector.AbsoluteB;
              double top2 = (double) (vector2i.Y - num2);
              vector2i = collisionVector.AbsoluteB;
              double right2 = (double) (vector2i.X + num2);
              vector2i = collisionVector.AbsoluteB;
              double bottom2 = (double) (vector2i.Y + num2);
              Rectangle destination2 = Rectangle.FromLTRB(left2, top2, right2, bottom2);
              double tickness2 = (double) (num2 * 4);
              obj8.RenderRectangle(hex2, destination2, tickness2);
            }
          }
          foreach (ActiveObject activeObject in (IEnumerable<ActiveObject>) this.Level.ObjectManager.ActiveObjects)
          {
            foreach (CollisionVector collisionVector in activeObject.CollisionVectors)
              obj1.RenderLine(Colour.ParseHex("FF00FF"), (Vector2) collisionVector.AbsoluteA, (Vector2) collisionVector.AbsoluteB, 5.0);
            foreach (CollisionRectangle collisionRectangle in activeObject.CollisionRectangles)
              obj1.RenderRectangle(Colour.ParseHex("FF00FF"), (Rectangle) collisionRectangle.AbsoluteBounds, 5.0);
          }
          if (this.GroundVector != null)
          {
            obj1.RenderLine(Colour.ParseHex("FFFFFF"), (Vector2) this.GroundVector.AbsoluteA, (Vector2) this.GroundVector.AbsoluteB, 5.0);
            if (this.GroundVector.GetConnectionA(this.Path) != null)
            {
              CollisionVector connectionA = this.GroundVector.GetConnectionA(this.Path);
              obj1.RenderLine(Colour.ParseHex("00FFFF"), (Vector2) connectionA.AbsoluteA, (Vector2) connectionA.AbsoluteB, 5.0);
            }
            if (this.GroundVector.GetConnectionB(this.Path) != null)
            {
              CollisionVector connectionB = this.GroundVector.GetConnectionB(this.Path);
              obj1.RenderLine(Colour.ParseHex("FFFF00"), (Vector2) connectionB.AbsoluteA, (Vector2) connectionB.AbsoluteB, 5.0);
            }
          }
          List<Vector2[]> vector2ArrayList1 = new List<Vector2[]>()
          {
            new Vector2[2]{ a6, b6 }
          };
          vector2ArrayList1.AddRange((IEnumerable<Vector2[]>) collection1);
          for (int index = 0; index < vector2ArrayList1.Count; ++index)
          {
            Vector2[] vector2Array3 = vector2ArrayList1[index];
            obj1.RenderLine(Colour.ParseHex("00FF00"), vector2Array3[0], vector2Array3[1], 5.0);
          }
          List<Vector2[]> vector2ArrayList2 = new List<Vector2[]>()
          {
            new Vector2[2]{ a7, b7 }
          };
          vector2ArrayList2.AddRange((IEnumerable<Vector2[]>) collection2);
          for (int index = 0; index < vector2ArrayList2.Count; ++index)
          {
            Vector2[] vector2Array4 = vector2ArrayList2[index];
            obj1.RenderLine(Colour.ParseHex("0000FF"), vector2Array4[0], vector2Array4[1], 5.0);
          }
        }
      }

      protected override void OnDraw(Renderer renderer, LayerViewOptions viewOptions)
      {
        if (this.IsSidekick && this._autoSidekickState == Character.AutoSidekickState.Spawning)
          return;
        if (this.IsHurt || this.InvulnerabilityTicks <= 0 || this.InvulnerabilityTicks % 8 >= 4)
          this.DrawBody(renderer, viewOptions);
        this.DrawSpindashDust(renderer);
        if (!viewOptions.Shadows)
        {
          this.DrawProtection(renderer);
          this.DrawBreathTimeRemaining(renderer);
        }
        this._intersection -= this.PositionPrecise;
        renderer.Get2dRenderer().RenderRectangle(Colour.ParseHex("FF00FF"), Rectangle.FromLTRB(this._intersection.X - 10.0, this._intersection.Y - 10.0, this._intersection.X + 10.0, this._intersection.Y + 10.0), 40.0);
      }

      protected virtual void DrawBody(Renderer renderer, LayerViewOptions viewOptions)
      {
        I2dRenderer obj = renderer.Get2dRenderer();
        ICharacterRenderer characterRenderer = renderer.GetCharacterRenderer();
        characterRenderer.ModelMatrix = obj.ModelMatrix;
        double propertyDouble1 = this.Level.GameContext.Configuration.GetPropertyDouble("debug", "sonic_hue_shift");
        double propertyDouble2 = this.Level.GameContext.Configuration.GetPropertyDouble("debug", "sonic_sat_shift");
        double propertyDouble3 = this.Level.GameContext.Configuration.GetPropertyDouble("debug", "sonic_lum_shift");
        if (this.DrawBodyRotated)
          characterRenderer.ModelMatrix *= Matrix4.CreateRotationZ(this.ShowAngle);
        SonicOrca.Graphics.Animation.Frame currentFrame = this.Animation.CurrentFrame;
        Vector2 offset = (Vector2) currentFrame.Offset;
        Rectangle destination = new Rectangle(offset.X - (double) (currentFrame.Source.Width / 2), offset.Y - (double) (currentFrame.Source.Height / 2), (double) currentFrame.Source.Width, (double) currentFrame.Source.Height);
        characterRenderer.Filter = viewOptions.Filter;
        characterRenderer.FilterAmount = viewOptions.FilterAmount;
        characterRenderer.Brightness = viewOptions.Shadows ? 0.0f : this.Brightness;
        characterRenderer.RenderTexture(this.Animation.AnimationGroup.Textures[1], this.Animation.AnimationGroup.Textures[0], propertyDouble1, propertyDouble2, propertyDouble3, (Rectangle) currentFrame.Source, destination, this.IsFacingRight, this.IsFacingLeft && this.Animation.Index == 16 /*0x10*/);
      }

      protected void DrawSpindashDust(Renderer renderer)
      {
        if (!this.IsCharging)
          return;
        Vector2 destination;
        ref Vector2 local = ref destination;
        Rectanglei source = this.SpindashDustAnimation.CurrentFrame.Source;
        double x = (double) (source.Width / 2);
        double num1 = (double) this.CollisionRadius.Y + 8.0;
        source = this.SpindashDustAnimation.CurrentFrame.Source;
        double num2 = (double) (source.Height / 2);
        double y = num1 - num2;
        local = new Vector2(x, y);
        if (this.IsFacingRight)
          destination.X *= -1.0;
        renderer.GetObjectRenderer().Render(this.SpindashDustAnimation, destination, this.IsFacingRight);
      }

      protected void DrawProtection(Renderer renderer)
      {
        if (this.IsInvincible)
          this.DrawInvincibility(renderer);
        else
          this.DrawBarrier(renderer);
      }

      protected void DrawBarrier(Renderer renderer)
      {
        if (!this.HasBarrier || this.BarrierAnimation == null)
          return;
        IObjectRenderer objectRenderer = renderer.GetObjectRenderer();
        objectRenderer.BlendMode = BlendMode.Additive;
        objectRenderer.FilterAmount *= 0.25;
        objectRenderer.Render(this.BarrierAnimation);
      }

      protected void DrawBreathTimeRemaining(Renderer renderer)
      {
        if (!this._drowningClimax || this._drowningAnimation.Cycles != 0)
          return;
        this._drowningAnimation.Draw(renderer.Get2dRenderer(), (Vector2) (this._drownCountdownPosition - this.Position));
      }

      protected int InvulnerabilityTicks { get; set; }

      private void ScatterRings(int ringCount, int facing)
      {
        this.Level.SoundManager.PlaySound((IActiveObject) this, "SONICORCA/SOUND/RINGSCATTER");
        ringCount = Math.Min(ringCount, 32 /*0x20*/);
        for (int index = 0; index < ringCount; ++index)
        {
          Vector2 scatterRingOffset = Character.ScatterRingOffsets[index];
          scatterRingOffset.X *= (double) facing;
          this.CreateRing(scatterRingOffset);
        }
      }

      private void CreateRing(Vector2 velocity)
      {
        this.Level.ObjectManager.AddObject(new ObjectPlacement(this.Level.CommonResources.GetResourcePath("ringobject"), this.Level.Map.Layers.IndexOf(this.Layer), this.Position, (object) new
        {
          Scatter = true,
          Velocity = new Vector2(velocity.X, velocity.Y)
        }));
      }

      public bool CanBeHurt => !this.IsHurt && this.InvulnerabilityTicks <= 0 && !this.IsInvincible;

      public void Hurt(int direction, PlayerDeathCause cause = PlayerDeathCause.Hurt)
      {
        if (!this.CanBeHurt)
          return;
        this.LeaveGround();
        if (this.IsSidekick)
        {
          Vector2 vector2 = new Vector2(-8.0, -16.0);
          if (this.IsUnderwater)
            vector2 = new Vector2(vector2.X / 2.0, vector2.Y / 2.0);
          if (direction >= 0)
            vector2.X *= -1.0;
          this.Velocity = vector2;
          this.Facing = -direction;
          this.IsAirborne = true;
          this.IsBraking = false;
          this.IsCharging = false;
          this.IsSpinball = false;
          this.IsFlying = false;
          this.IsHurt = true;
          this.ShowAngle = 0.0;
          this.TumbleAngle = 0.0;
          this.InvulnerabilityTicks = 120;
        }
        else if (this.Player.CurrentRings > 0 || this.HasBarrier)
        {
          Vector2 vector2 = new Vector2(-8.0, -16.0);
          if (this.IsUnderwater)
            vector2 = new Vector2(vector2.X / 2.0, vector2.Y / 2.0);
          if (direction >= 0)
            vector2.X *= -1.0;
          this.Velocity = vector2;
          this.Facing = -direction;
          this.IsAirborne = true;
          this.IsBraking = false;
          this.IsCharging = false;
          this.IsSpinball = false;
          this.IsFlying = false;
          this.IsHurt = true;
          this.ShowAngle = 0.0;
          this.TumbleAngle = 0.0;
          this.InvulnerabilityTicks = 120;
          if (this.HasBarrier)
          {
            this.Barrier = BarrierType.None;
            this.Level.SoundManager.PlaySound((IActiveObject) this, cause == PlayerDeathCause.Spikes ? "SONICORCA/SOUND/HURT/SPIKES" : "SONICORCA/SOUND/HURT");
          }
          else
          {
            this.ScatterRings(this.Player.CurrentRings, -direction);
            this.Player.LoseAllRings();
          }
        }
        else
          this.Kill(cause);
        this.CharacterEvents |= CharacterEvent.Hurt;
      }

      public void Kill(PlayerDeathCause cause)
      {
        this._deathCause = cause;
        this.Velocity = new Vector2(0.0, -28.0);
        this._dyingTicks = 120;
        this.CameraProperties.Delay = new Vector2i(3600, 3600);
        this.StateFlags = (CharacterState) 0;
        this.IsAirborne = true;
        this.IsBraking = false;
        this.IsCharging = false;
        this.IsFlying = false;
        this.IsHurt = false;
        this.IsDying = true;
        this.ShouldReactToLevel = false;
        this.ShowAngle = 0.0;
        this.Level.SoundManager.PlaySound((IActiveObject) this, cause == PlayerDeathCause.Spikes ? "SONICORCA/SOUND/HURT/SPIKES" : "SONICORCA/SOUND/HURT");
        this.Priority = 4096 /*0x1000*/;
      }

      private void UpdateDying()
      {
        if (this._dyingTicks == 0)
        {
          this.StateFlags = (CharacterState) 0;
          this.IsDead = true;
        }
        else
        {
          double num = this._deathCause == PlayerDeathCause.Drown ? 0.25 : 0.875;
          Vector2 velocity = this.Velocity;
          double x = velocity.X;
          velocity = this.Velocity;
          double y = Math.Min(velocity.Y + num, 96.0);
          this.Velocity = new Vector2(x, y);
          this.PositionPrecise = this.PositionPrecise + this.Velocity;
          --this._dyingTicks;
        }
      }

      protected string InvincibilityGroupResourceKey { get; set; }

      private void InitialiseInvincibility()
      {
        AnimationGroup loadedResource = this.Level.GameContext.ResourceTree.GetLoadedResource<AnimationGroup>(this.InvincibilityGroupResourceKey);
        this._invincibilityParticles.Clear();
        for (int animationIndex = 0; animationIndex < 4; ++animationIndex)
        {
          for (int index = 0; index < 3; ++index)
            this._invincibilityParticles.Add(new Character.InvincibilityParticle(this, (double) (index * 4 + animationIndex) / 12.0 * (2.0 * Math.PI), loadedResource, animationIndex));
        }
      }

      private void StartInvincibility()
      {
        this._invincibilityCharacterPositionHistory.Clear();
        for (int index = 0; index < 8; ++index)
          this._invincibilityCharacterPositionHistory.Add((Vector2) this.Position);
        this._invincibilityCharacterPositionHistory.TrimExcess();
      }

      private void AnimateInvincibility()
      {
        if (this._invincibilityCharacterPositionHistory.Count > 0)
          this._invincibilityCharacterPositionHistory.RemoveAt(this._invincibilityCharacterPositionHistory.Count - 1);
        this._invincibilityCharacterPositionHistory.Insert(0, (Vector2) this.Position);
        for (int index1 = 0; index1 < 4; ++index1)
        {
          int index2 = index1 * 2;
          if (index2 < this._invincibilityCharacterPositionHistory.Count)
          {
            Vector2 vector2 = this._invincibilityCharacterPositionHistory[index2];
            for (int index3 = 0; index3 < 3; ++index3)
              this._invincibilityParticles[index1 * 3 + index3].Origin = vector2;
          }
        }
        foreach (Character.InvincibilityParticle invincibilityParticle in this._invincibilityParticles)
          invincibilityParticle.Animate();
      }

      private void DrawInvincibility(Renderer renderer)
      {
        foreach (Character.InvincibilityParticle invincibilityParticle in this._invincibilityParticles)
          invincibilityParticle.Draw(renderer);
      }

      public bool IsSidekick { get; set; }

      public bool Respawning { get; private set; }

      private bool IsLeftBehind
      {
        get
        {
          if (!this.IsSidekick)
            return false;
          double num1 = (double) Math.Abs(this.Position.X - this.Player.Protagonist.Position.X);
          Vector2i position = this.Position;
          int y1 = position.Y;
          position = this.Player.Protagonist.Position;
          int y2 = position.Y;
          double num2 = (double) Math.Abs(y1 - y2);
          return num1 > 2069.0 || num2 > 1152.0;
        }
      }

      public bool IsOnScreen()
      {
        Rectangle rect;
        ref Rectangle local = ref rect;
        Vector2i position = this.Position;
        double x = (double) (position.X - 128 /*0x80*/);
        position = this.Position;
        double y = (double) (position.Y - 128 /*0x80*/);
        local = new Rectangle(x, y, 256.0, 256.0);
        return this.Level.Camera.Bounds.IntersectsWith(rect);
      }

      public bool IsHumanInput
      {
        get
        {
          return this.Input.HorizontalDirection != 0 || this.Input.VerticalDirection != 0 || this.Input.ABC.HasFlag((Enum) CharacterInputButtonState.Down);
        }
      }

      private void UpdateSidekick()
      {
        if (!this.Level.StateFlags.HasFlag((Enum) LevelStateFlags.AllowCharacterControl))
        {
          this.UpdateNormal();
        }
        else
        {
          if (this.IsHumanInput)
          {
            this._humanControlled = true;
            this._humanInputTicksRemaining = 600;
          }
          else
          {
            --this._humanInputTicksRemaining;
            if (this._humanInputTicksRemaining <= 0)
              this._humanControlled = false;
          }
          ICharacter protagonist = this.Player.Protagonist;
          CharacterHistoryItem protagonistHistory = protagonist.History[17];
          switch (this._autoSidekickState)
          {
            case Character.AutoSidekickState.Spawning:
              if (protagonist.IsDebug || !this._humanControlled && this.Level.Ticks % 64 /*0x40*/ != 0)
                break;
              this._autoSidekickState = Character.AutoSidekickState.Flying;
              this.StateFlags = (CharacterState) 0;
              this.CollisionRadius = this.NormalCollisionRadius;
              this.Position = new Vector2i(protagonist.Position.X, protagonist.Position.Y - 1152);
              this.Priority = 8192 /*0x2000*/;
              this._sidekickOffscreenTicks = 300;
              break;
            case Character.AutoSidekickState.Flying:
              if (this.IsOnScreen())
              {
                this._sidekickOffscreenTicks = 300;
              }
              else
              {
                --this._sidekickOffscreenTicks;
                if (this._sidekickOffscreenTicks <= 0)
                {
                  this._autoSidekickState = Character.AutoSidekickState.Spawning;
                  this.Position = new Vector2i();
                  break;
                }
              }
              this._sidekickTargetPosition = protagonistHistory.Position;
              if (this.CanFly)
              {
                this.IsFlying = true;
                this.IsSpinball = false;
              }
              else
                this.IsSpinball = true;
              this.IsAirborne = true;
              this.IsHurt = false;
              this.IsDying = false;
              this.IsDead = false;
              this.ShouldReactToLevel = false;
              this.Mode = CollisionMode.Air;
              Vector2i offset = new Vector2i();
              int x1 = this._sidekickTargetPosition.X;
              Vector2i vector2i = this.Position;
              int x2 = vector2i.X;
              int num1 = x1 - x2;
              if (num1 != 0)
              {
                offset.X = Math.Sign(num1) * Math.Min(48 /*0x30*/, Math.Abs(num1) / 4);
                offset.X += (int) protagonist.Velocity.X + 4;
                this.Facing = Math.Sign(offset.X);
              }
              vector2i = protagonist.NormalCollisionRadius;
              int y1 = vector2i.Y;
              vector2i = this.NormalCollisionRadius;
              int y2 = vector2i.Y;
              int num2 = y1 - y2;
              int y3 = this._sidekickTargetPosition.Y;
              vector2i = this.Position;
              int y4 = vector2i.Y;
              int num3 = y3 - y4 + num2;
              if (num3 != 0)
                offset.Y = Math.Sign(num3) * Math.Min(Math.Abs(num3), this.CanFly ? 4 : 24);
              CharacterState state = protagonistHistory.State;
              if ((state & (CharacterState.Dying | CharacterState.Dead)) == (CharacterState) 0)
                this.Move(offset);
              if (Math.Abs(offset.X) > 48 /*0x30*/ || offset.Y != 0 || (state & (CharacterState.Rolling | CharacterState.Airborne | CharacterState.Dying | CharacterState.Dead | CharacterState.Debug)) != (CharacterState) 0)
                break;
              this.Velocity = new Vector2();
              this.GroundVelocity = 0.0;
              this.Path = protagonist.Path;
              this.Layer = protagonist.Layer;
              this.Mode = CollisionMode.Air;
              this._autoSidekickState = Character.AutoSidekickState.Normal;
              goto case Character.AutoSidekickState.Normal;
            case Character.AutoSidekickState.Normal:
              if (protagonist.IsDying || protagonist.IsDead)
              {
                this.UpdateNormal();
                break;
              }
              this.ShouldReactToLevel = true;
              this.Priority = 1020;
              if (this.IsUnableToCatchUp())
                break;
              if (this._humanControlled || this.IsObjectControlled || this.IsWinning || protagonistHistory == null)
              {
                this.UpdateNormal();
                break;
              }
              if (this.SlopeLockTicks > 0)
              {
                this._autoSidekickState = Character.AutoSidekickState.Spindash;
                goto case Character.AutoSidekickState.Spindash;
              }
              this.ApplyAutoInput(this.GetAutoInputNormal(protagonist, protagonistHistory));
              this.UpdateNormal();
              break;
            case Character.AutoSidekickState.Spindash:
              if (this.IsUnableToCatchUp())
                break;
              if (this.SlopeLockTicks > 0)
              {
                this.UpdateNormal();
                break;
              }
              this.ApplyAutoInput(this.GetAutoInputSpindash(protagonist));
              this.UpdateNormal();
              break;
          }
        }
      }

      private CharacterInputState GetAutoInputNormal(
        ICharacter protagonist,
        CharacterHistoryItem protagonistHistory)
      {
        CharacterInputState autoInputNormal = new CharacterInputState(protagonistHistory.Input);
        this.IsFacingLeft = protagonistHistory.State.HasFlag((Enum) CharacterState.Left);
        Vector2i position1 = protagonistHistory.Position;
        int x1 = position1.X;
        position1 = this.Position;
        int x2 = position1.X;
        int num1 = x1 - x2;
        if (num1 == 0)
          this.IsFacingLeft = protagonistHistory.State.HasFlag((Enum) CharacterState.Left);
        else if (num1 > 192 /*0xC0*/)
        {
          if (num1 >= 64 /*0x40*/)
            autoInputNormal.HorizontalDirection = 1;
          if (this.GroundVelocity != 0.0 && this.IsFacingRight)
            this.Move(4, 0);
        }
        else if (num1 < 128 /*0x80*/)
        {
          if (num1 <= -64)
            autoInputNormal.HorizontalDirection = -1;
          if (this.GroundVelocity != 0.0 && !this.IsFacingRight)
            this.Move(-4, 0);
        }
        if (this._autoSidekickJumping)
        {
          autoInputNormal.A = CharacterInputButtonState.Down;
          if (this.IsAirborne)
            return autoInputNormal;
          this._autoSidekickJumping = false;
        }
        if (this.Level.Ticks % 256 /*0x0100*/ == 0 && num1 >= 256 /*0x0100*/)
          return autoInputNormal;
        Vector2i position2 = protagonistHistory.Position;
        int y1 = position2.Y;
        position2 = this.Position;
        int y2 = position2.Y;
        int num2 = y1 - y2;
        if (num2 >= 0)
        {
          if (protagonist.IsPushing)
            autoInputNormal.HorizontalDirection = Math.Sign(num1);
          return autoInputNormal;
        }
        if (num2 > (int) sbyte.MinValue)
        {
          if (!protagonist.IsPushing || !this.IsPushing || this._pushDirection == protagonist.Facing || this.LookDirection == CharacterLookDirection.Ducking)
            return autoInputNormal;
          autoInputNormal.A = CharacterInputButtonState.Pressed;
          this._autoSidekickJumping = true;
          return autoInputNormal;
        }
        if (this.Level.Ticks % 64 /*0x40*/ == 0 && this.LookDirection != CharacterLookDirection.Ducking)
        {
          autoInputNormal.A = CharacterInputButtonState.Pressed;
          this._autoSidekickJumping = true;
        }
        return autoInputNormal;
      }

      private CharacterInputState GetAutoInputSpindash(ICharacter protagonist)
      {
        CharacterInputState autoInputSpindash = new CharacterInputState();
        if (!this.IsCharging)
        {
          if (this.GroundVelocity != 0.0)
            return autoInputSpindash;
          Vector2i position = protagonist.Position;
          int x1 = position.X;
          position = this.Position;
          int x2 = position.X;
          this.Facing = Math.Sign(x1 - x2);
          autoInputSpindash.VerticalDirection = 1;
          autoInputSpindash.A = CharacterInputButtonState.Pressed;
        }
        else
        {
          autoInputSpindash.VerticalDirection = 1;
          if (this.Level.Ticks % 128 /*0x80*/ != 0)
          {
            if (this.Level.Ticks % 32 /*0x20*/ == 0)
              autoInputSpindash.A = CharacterInputButtonState.Pressed;
          }
          else
            this._autoSidekickState = Character.AutoSidekickState.Normal;
        }
        return autoInputSpindash;
      }

      private void ApplyAutoInput(CharacterInputState input)
      {
        this.LookDirection = (CharacterLookDirection) input.VerticalDirection;
        this.Input = input;
      }

      private bool IsUnableToCatchUp()
      {
        if (this.IsOnScreen())
          this._sidekickOffscreenTicks = 300;
        else
          --this._sidekickOffscreenTicks;
        if (!this.IsDead && this._sidekickOffscreenTicks > 0)
          return false;
        this._autoSidekickState = Character.AutoSidekickState.Spawning;
        this.Position = new Vector2i(0, 0);
        return true;
      }

      private void DoInstaShield()
      {
      }

      private void DoFireBarrierMove()
      {
        this.GroundVelocity = (double) (32 /*0x20*/ * this.Facing);
        this.Velocity = new Vector2(this.GroundVelocity, 0.0);
      }

      private void DoLightningBarrierMove()
      {
        this.Velocity = new Vector2(this.Velocity.X, -22.0);
        this.IsJumping = false;
      }

      private void DoBubbleBarrierMove()
      {
        this.GroundVelocity = 0.0;
        this.Velocity = new Vector2(0.0, 32.0);
      }

      private void DoSuperHyperTransform()
      {
      }

      private void DoHyperDash()
      {
      }

      public CharacterState StateFlags { get; set; }

      public bool IsPushing
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Pushing);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.Pushing : this.StateFlags & ~CharacterState.Pushing;
        }
      }

      public bool IsBraking
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Braking);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.Braking : this.StateFlags & ~CharacterState.Braking;
        }
      }

      public bool IsSpinball
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Spinball);
        set
        {
          if (this.IsSpinball == value)
            return;
          if (value)
          {
            this.StateFlags |= CharacterState.Spinball;
            this.CollisionRadius = this.SpinballCollisionRadius;
            Vector2 positionPrecise = this.PositionPrecise;
            Vector2i vector2i = this.NormalCollisionRadius;
            int y1 = vector2i.Y;
            vector2i = this.SpinballCollisionRadius;
            int y2 = vector2i.Y;
            Vector2 vector2 = new Vector2(0.0, (double) (y1 - y2));
            this.PositionPrecise = positionPrecise + vector2;
          }
          else
          {
            this.StateFlags &= ~CharacterState.Spinball;
            this.CollisionRadius = this.NormalCollisionRadius;
            Vector2 positionPrecise = this.PositionPrecise;
            Vector2i vector2i = this.NormalCollisionRadius;
            int y3 = vector2i.Y;
            vector2i = this.SpinballCollisionRadius;
            int y4 = vector2i.Y;
            Vector2 vector2 = new Vector2(0.0, (double) -(y3 - y4));
            this.PositionPrecise = positionPrecise + vector2;
          }
          if (value)
            return;
          this.IsRollJumping = false;
        }
      }

      public bool IsRollJumping
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Rolling);
        set
        {
          if (value)
            this.StateFlags |= CharacterState.Rolling;
          else
            this.StateFlags &= ~CharacterState.Rolling;
        }
      }

      public bool IsAirborne
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Airborne);
        set
        {
          if (value)
          {
            if (this.StateFlags != CharacterState.Airborne)
            {
              this.StateFlags |= CharacterState.Airborne;
              this.LastTickOnGround = this.Level.Ticks;
            }
            this.Mode = CollisionMode.Air;
            this.IsBraking = false;
            this.IsPushing = false;
          }
          else
          {
            this.StateFlags &= ~CharacterState.Airborne;
            this.IsFlying = false;
            this.IsJumping = false;
          }
        }
      }

      public bool IsJumping
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Jumping);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.Jumping : this.StateFlags & ~CharacterState.Jumping;
        }
      }

      public int Facing
      {
        get => !this.StateFlags.HasFlag((Enum) CharacterState.Left) ? 1 : -1;
        set
        {
          if (value < 0)
          {
            this.StateFlags |= CharacterState.Left;
          }
          else
          {
            if (value <= 0)
              return;
            this.StateFlags &= ~CharacterState.Left;
          }
        }
      }

      public bool IsFacingLeft
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Left);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.Left : (this.StateFlags &= ~CharacterState.Left);
        }
      }

      public bool IsFacingRight
      {
        get => !this.StateFlags.HasFlag((Enum) CharacterState.Left);
        set
        {
          this.StateFlags = value ? (this.StateFlags &= ~CharacterState.Left) : this.StateFlags | CharacterState.Left;
        }
      }

      public bool IsUnderwater
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Underwater);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.Underwater : this.StateFlags & ~CharacterState.Underwater;
        }
      }

      public bool IsHurt
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Hurt);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.Hurt : (this.StateFlags &= ~CharacterState.Hurt);
        }
      }

      public bool IsDying
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Dying);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.Dying : this.StateFlags & ~CharacterState.Dying;
        }
      }

      public bool IsDead
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Dead);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.Dead : this.StateFlags & ~CharacterState.Dead;
        }
      }

      public bool IsDebug
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Debug);
        set
        {
          if (value)
          {
            this.StateFlags = CharacterState.Debug | this.StateFlags & CharacterState.Left;
            this.Velocity = new Vector2(0.0, 0.0);
          }
          else
          {
            this.StateFlags = CharacterState.Airborne | this.StateFlags & CharacterState.Left;
            if (this.Player.InvincibillityTicks > 0 && !this.IsSidekick)
              this.StateFlags |= CharacterState.Invincible;
          }
          this.IsSpinball = true;
          this.Mode = CollisionMode.Air;
        }
      }

      public bool IsFlying
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Flying);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.Flying : this.StateFlags & ~CharacterState.Flying;
        }
      }

      public bool ExhibitsVirtualPlatform
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.VirtualPlatform);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.VirtualPlatform : this.StateFlags & ~CharacterState.VirtualPlatform;
        }
      }

      public bool HasSpeedShoes
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.SpeedShoes);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.SpeedShoes : this.StateFlags & ~CharacterState.SpeedShoes;
        }
      }

      public bool IsInvincible
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Invincible);
        set
        {
          if (value)
          {
            if (this.IsInvincible)
              return;
            this.StateFlags |= CharacterState.Invincible;
            this.StartInvincibility();
          }
          else
            this.StateFlags &= ~CharacterState.Invincible;
        }
      }

      public bool ForceSpinball
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.ForceSpinball);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.ForceSpinball : this.StateFlags & ~CharacterState.ForceSpinball;
        }
      }

      public bool IsWinning
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Winning);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.Winning : (this.StateFlags &= ~CharacterState.Winning);
          if (!value)
            return;
          this.GroundVelocity = 0.0;
          this.Velocity = new Vector2();
        }
      }

      public bool IsSuper
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.Super);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.Super : this.StateFlags & ~CharacterState.Super;
        }
      }

      public bool IsCharging
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.ChargingSpindash);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.ChargingSpindash : this.StateFlags & ~CharacterState.ChargingSpindash;
        }
      }

      public bool ShouldReactToLevel
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.ShouldReactToLevel);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.ShouldReactToLevel : this.StateFlags & ~CharacterState.ShouldReactToLevel;
        }
      }

      public bool IsObjectControlled
      {
        get => this.StateFlags.HasFlag((Enum) CharacterState.ObjectControlled);
        set
        {
          this.StateFlags = value ? this.StateFlags | CharacterState.ObjectControlled : this.StateFlags & ~CharacterState.ObjectControlled;
        }
      }

      public int BreathTicks { get; set; }

      private void UpdateWaterLogic()
      {
        if (this.IsDying && this._deathCause == PlayerDeathCause.Drown)
        {
          this.IsUnderwater = true;
          if (this._nextBubbleTime-- >= 0)
            return;
          this._nextBubbleTime = this.Level.Random.Next(2, 8);
          this.CreateBubble(1);
        }
        else
        {
          if (this.IsDying || this.IsDead)
            return;
          bool isUnderwater = this.IsUnderwater;
          bool flag = this.IsUnderwater = this.Level.WaterManager.IsUnderwater(this.Position);
          if (!isUnderwater & flag)
            this.EnterWater();
          else if (isUnderwater && !flag)
            this.LeaveWater();
          if (flag)
          {
            if (this._nextBubbleTime-- < 0)
            {
              this._nextBubbleTime = this.Level.Random.Next(Math.Abs(this.Velocity.X) > 4.0 ? 8 : 32 /*0x20*/, Math.Abs(this.Velocity.X) > 4.0 ? 32 /*0x20*/ : 128 /*0x80*/);
              this.CreateBubble(0);
            }
            --this.BreathTicks;
            this.CheckForDrowning();
            if (this.Barrier != BarrierType.Fire && this.Barrier != BarrierType.Lightning)
              return;
            this.Barrier = BarrierType.None;
          }
          else
            this.BreathTicks = 1800;
        }
      }

      private void EnterWater()
      {
        Vector2 velocity = this.Velocity;
        double x = velocity.X / 2.0;
        velocity = this.Velocity;
        double y = velocity.Y / 4.0;
        this.Velocity = new Vector2(x, y);
        if (!this.IsAirborne || this.IsObjectControlled)
          return;
        this.Level.WaterManager.CreateSplash(this.Layer, SplashType.Enter, this.Position);
        this.Level.SoundManager.PlaySound("SONICORCA/SOUND/SPLASH");
      }

      private void LeaveWater()
      {
        if (this.IsAirborne && this.Velocity.Y < 0.0)
          this.Velocity = new Vector2(this.Velocity.X, Math.Max(this.Velocity.Y * 2.0, -48.0));
        if (this.IsAirborne && !this.IsObjectControlled && !this.IsDying)
        {
          this.Level.WaterManager.CreateSplash(this.Layer, SplashType.Exit, this.Position);
          this.Level.SoundManager.PlaySound("SONICORCA/SOUND/SPLASH");
        }
        this.BreathTicks = 1800;
        if (!this._drowningClimax || this.IsSidekick)
          return;
        this.Level.SoundManager.StopJingle(JingleType.Drowning);
        this.Level.SoundManager.PlayMusic();
      }

      private void CheckForDrowning()
      {
        if (this.BreathTicks > 720)
        {
          if (((IEnumerable<int>) Character.BreathLeftWarnings).Contains<int>(this.BreathTicks) && !this.IsSidekick)
            this.Level.SoundManager.PlaySound("SONICORCA/SOUND/DROWNWARNING");
          this._drowningClimax = false;
        }
        else
        {
          if (!this._drowningClimax)
          {
            if (!this.IsSidekick)
              this.Level.SoundManager.PlayJingle(JingleType.Drowning);
            this._drowningClimax = true;
          }
          if (this.BreathTicks <= 300 && this.BreathTicks % 60 == 0)
            this.ShowBreathTimeRemaining(this.BreathTicks / 60);
          if (this.BreathTicks > -60)
            return;
          this.Drown();
        }
      }

      private void Drown()
      {
        this.Kill(PlayerDeathCause.Drown);
        this.Velocity = new Vector2();
        if (!this.IsSidekick)
          this.Level.SoundManager.StopJingle(JingleType.Drowning);
        this.Level.SoundManager.PlaySound((IActiveObject) this, "SONICORCA/SOUND/DROWN");
      }

      private void ShowBreathTimeRemaining(int secondsLeft)
      {
        this._drownCountdownValue = secondsLeft;
        this._drownCountdownPosition = this.Position + new Vector2i(96 /*0x60*/, -96);
        if (secondsLeft < 0 || secondsLeft > 5)
          return;
        this._drowningAnimation.Index = 5 - secondsLeft;
        this._drowningAnimation.Cycles = 0;
      }

      private void CreateBubble(int size)
      {
        this.Level.WaterManager.CreateBubble(this.Level.Map.Layers.IndexOf(this.Layer), this.Position + new Vector2i(8, -8), size);
      }

      public void InhaleOxygen()
      {
        this.BreathTicks = 1800;
        if (this._drowningClimax && !this.IsSidekick)
        {
          this.Level.SoundManager.StopJingle(JingleType.Drowning);
          this.Level.SoundManager.PlayMusic();
        }
        this.IsSpinball = false;
        this._inhalingBubble = 1;
        this.Level.SoundManager.PlaySound((IActiveObject) this, "SONICORCA/SOUND/INHALE");
      }

      private enum CollisionBox
      {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
      }

      public class Intersector
      {
        private static double MyEpsilon = 1E-05;

        private static double[] OverlapIntervals(double ub1, double ub2)
        {
          double val2_1 = Math.Min(ub1, ub2);
          double val2_2 = Math.Max(ub1, ub2);
          double num1 = Math.Max(0.0, val2_1);
          double num2 = Math.Min(1.0, val2_2);
          if (num1 > num2)
            return new double[0];
          return num1 == num2 ? new double[1]{ num1 } : new double[2]
          {
            num1,
            num2
          };
        }

        private static Vector2[] OneD_Intersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
          double num1 = a2.X - a1.X;
          double num2 = a2.Y - a1.Y;
          double ub1;
          double ub2;
          if (Math.Abs(num1) > Math.Abs(num2))
          {
            ub1 = (b1.X - a1.X) / num1;
            ub2 = (b2.X - a1.X) / num1;
          }
          else
          {
            ub1 = (b1.Y - a1.Y) / num2;
            ub2 = (b2.Y - a1.Y) / num2;
          }
          List<Vector2> vector2List = new List<Vector2>();
          foreach (float overlapInterval in Character.Intersector.OverlapIntervals(ub1, ub2))
          {
            Vector2 vector2 = new Vector2(a2.X * (double) overlapInterval + a1.X * (1.0 - (double) overlapInterval), a2.Y * (double) overlapInterval + a1.Y * (1.0 - (double) overlapInterval));
            vector2List.Add(vector2);
          }
          return vector2List.ToArray();
        }

        private static bool PointOnLine(Vector2 p, Vector2 a1, Vector2 a2)
        {
          double u = 0.0;
          return Character.Intersector.DistFromSeg(p, a1, a2, Character.Intersector.MyEpsilon, ref u) < Character.Intersector.MyEpsilon;
        }

        private static double DistFromSeg(
          Vector2 p,
          Vector2 q0,
          Vector2 q1,
          double radius,
          ref double u)
        {
          double num1 = q1.X - q0.X;
          double num2 = q1.Y - q0.Y;
          double num3 = q0.X - p.X;
          double num4 = q0.Y - p.Y;
          double num5 = Math.Sqrt(num1 * num1 + num2 * num2);
          if (num5 < Character.Intersector.MyEpsilon)
            throw new Exception("Expected line segment, not point.");
          return Math.Abs(num1 * num4 - num3 * num2) / num5;
        }

        public static Vector2[] Intersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
          if (a1.Equals(a2) && b1.Equals(b2))
          {
            if (!a1.Equals(b1))
              return new Vector2[0];
            return new Vector2[1]{ a1 };
          }
          if (b1.Equals(b2))
          {
            if (!Character.Intersector.PointOnLine(b1, a1, a2))
              return new Vector2[0];
            return new Vector2[1]{ b1 };
          }
          if (a1.Equals(a2))
          {
            if (!Character.Intersector.PointOnLine(a1, b1, b2))
              return new Vector2[0];
            return new Vector2[1]{ a1 };
          }
          double num1 = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
          double num2 = (a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X);
          double num3 = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);
          if (-Character.Intersector.MyEpsilon >= num3 || num3 >= Character.Intersector.MyEpsilon)
          {
            double num4 = num1 / num3;
            double num5 = num2 / num3;
            if (0.0 > num4 || num4 > 1.0 || 0.0 > num5 || num5 > 1.0)
              return new Vector2[0];
            return new Vector2[1]
            {
              new Vector2(a1.X + num4 * (a2.X - a1.X), a1.Y + num4 * (a2.Y - a1.Y))
            };
          }
          if ((-Character.Intersector.MyEpsilon >= num1 || num1 >= Character.Intersector.MyEpsilon) && (-Character.Intersector.MyEpsilon >= num2 || num2 >= Character.Intersector.MyEpsilon))
            return new Vector2[0];
          return a1.Equals(a2) ? Character.Intersector.OneD_Intersection(b1, b2, a1, a2) : Character.Intersector.OneD_Intersection(a1, a2, b1, b2);
        }
      }

      private class InvincibilityParticle
      {
        private readonly Character _character;
        private readonly AnimationInstance _animation;
        private double _angle;
        private Vector2 _position;

        public Vector2 Origin { get; set; }

        public InvincibilityParticle(
          Character character,
          double initialAngle,
          AnimationGroup animationGroup,
          int animationIndex)
        {
          this._character = character;
          this._angle = initialAngle;
          this._animation = new AnimationInstance(animationGroup);
          this._animation.Index = animationIndex;
          this._animation.CurrentFrameIndex = this._character.Level.Random.Next(((IReadOnlyCollection<SonicOrca.Graphics.Animation.Frame>) animationGroup[animationIndex].Frames).Count);
        }

        public void Animate()
        {
          this._angle = MathX.WrapRadians(this._angle + Math.PI / 16.0);
          this._position.X = Math.Cos(this._angle) * 46.0 + (double) this._character.Level.Random.Next(-8, 8);
          this._position.Y = Math.Sin(this._angle) * 64.0 + (double) this._character.Level.Random.Next(-8, 8);
          this._animation.Animate();
        }

        public void Draw(Renderer renderer)
        {
          IObjectRenderer objectRenderer = renderer.GetObjectRenderer();
          objectRenderer.BlendMode = BlendMode.Additive;
          int filter = objectRenderer.Filter;
          objectRenderer.EmitsLight = true;
          objectRenderer.Render(this._animation, this.Origin - (Vector2) this._character.Position + this._position);
          objectRenderer.EmitsLight = false;
        }
      }

      private enum AutoSidekickState
      {
        Spawning,
        Flying,
        Normal,
        Spindash,
      }
    }
}
