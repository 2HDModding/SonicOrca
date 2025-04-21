// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.Platform
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Objects.Metadata;
using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core.Objects.Base
{

    public class Platform : ActiveObject, SonicOrca.Core.Objects.IPlatform, IActiveObject
    {
      private readonly List<ICharacter> _charactersOnPlatform = new List<ICharacter>();
      private Vector2i _initialPosition;
      private int _sagOffset;
      private bool _fallWhenStoodOn;
      private bool _falling;
      private int _fallingDelay;
      private Vector2 _fallingVelocity;
      protected Vector2 _velocityBasedOnNextPosition;

      protected Vector2 _nextPositionPrecise { get; set; }

      [StateVariable]
      public Vector2 MovementRadius { get; protected set; }

      [StateVariable]
      protected int TimePeriod { get; set; }

      [StateVariable]
      protected int TimeOffset { get; set; }

      [StateVariable]
      protected bool FallWhenStoodOn
      {
        get => this._fallWhenStoodOn;
        set => this._fallWhenStoodOn = value;
      }

      protected bool SagWhenStoodOn { get; set; }

      protected bool Linear { get; set; }

      public int CurrentTime { get; set; }

      public double CurrentT { get; set; }

      public Vector2 Velocity => this._velocityBasedOnNextPosition;

      public Ellipse Area { get; set; }

      protected override void OnStart()
      {
        this._initialPosition = this.Position;
        this._fallingDelay = 30;
      }

      protected override void OnUpdatePrepare()
      {
        Vector2 positionPrecise = this.PositionPrecise;
        this.UpdateMovement();
        this._nextPositionPrecise = this.PositionPrecise;
        this._velocityBasedOnNextPosition = this._nextPositionPrecise - positionPrecise;
        this.PositionPrecise = positionPrecise;
      }

      protected override void OnUpdate() => this.PositionPrecise = this._nextPositionPrecise;

      protected virtual void UpdateMovement()
      {
        this.UpdateTime();
        if (this._falling)
        {
          if (this._fallingDelay > 0)
          {
            --this._fallingDelay;
          }
          else
          {
            this._fallingVelocity.Y += 0.875;
            this.PositionPrecise = this.PositionPrecise + this._fallingVelocity;
          }
        }
        else
        {
          this.UpdateOscillationPosition();
          if (!this.IsCharacterInteractingWithPlatform())
          {
            if (this.MovementRadius.Y == 0.0 && !this._fallWhenStoodOn && this.SagWhenStoodOn)
              this._sagOffset = Math.Max(0, this._sagOffset - 2);
          }
          else
          {
            if (this.MovementRadius.Y == 0.0 && !this._fallWhenStoodOn && this.SagWhenStoodOn)
              this._sagOffset = Math.Min(16 /*0x10*/, this._sagOffset + 2);
            if (this._fallWhenStoodOn)
              this._falling = true;
          }
          this.PositionPrecise = this.PositionPrecise + new Vector2(0.0, (double) this._sagOffset);
        }
      }

      private void UpdateTime()
      {
        if (this.TimePeriod > 0)
        {
          this.CurrentTime = (this.Level.Ticks + this.TimeOffset) % this.TimePeriod;
          this.CurrentT = (double) this.CurrentTime / (double) this.TimePeriod;
        }
        else
        {
          this.CurrentTime = 0;
          this.CurrentT = 0.0;
        }
      }

      private void UpdateOscillationPosition()
      {
        if (this.TimePeriod == 0)
          return;
        double currentT = this.CurrentT;
        double num1;
        double num2;
        Vector2 movementRadius;
        if (this.Linear)
        {
          double num3 = currentT > 0.25 ? (currentT > 0.5 ? (currentT > 0.75 ? (currentT - 0.75) / 0.25 - 1.0 : -((currentT - 0.5) / 0.25)) : 1.0 - (currentT - 0.25) / 0.25) : currentT / 0.25;
          num1 = num3;
          num2 = num3;
        }
        else
        {
          double num4 = currentT * (2.0 * Math.PI);
          if (this.MovementRadius.X != 0.0)
          {
            movementRadius = this.MovementRadius;
            if (movementRadius.Y != 0.0)
            {
              num1 = Math.Cos(num4);
              num2 = Math.Sin(num4);
              goto label_7;
            }
          }
          num1 = Math.Sin(num4);
          num2 = Math.Sin(num4);
        }
    label_7:
        double x1 = (double) this._initialPosition.X;
        double num5 = num1;
        movementRadius = this.MovementRadius;
        double x2 = movementRadius.X;
        double num6 = num5 * x2;
        double x3 = x1 + num6;
        double y1 = (double) this._initialPosition.Y;
        double num7 = num2;
        movementRadius = this.MovementRadius;
        double y2 = movementRadius.Y;
        double num8 = num7 * y2;
        double y3 = y1 + num8 + (double) this._sagOffset;
        this.PositionPrecise = new Vector2(x3, y3);
      }

      protected bool IsCharacterInteractingWithPlatform()
      {
        return this.Level.ObjectManager.Characters.Any<ICharacter>((Func<ICharacter, bool>) (x => x.ObjectLink == this));
      }
    }
}
