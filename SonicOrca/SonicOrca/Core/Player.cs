// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Player
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Objects;
using SonicOrca.Geometry;
using System;

namespace SonicOrca.Core
{

    public class Player
    {
      private const int RingsForLife = 100;
      private const int ScoreForLife = 50000;
      private const int MaxLives = 999;
      private const int MaxRings = 999;
      private const int BigChainSize = 16 /*0x10*/;
      private const int BigChainScore = 10000;
      private static readonly int[] ChainScores = new int[4]
      {
        100,
        200,
        500,
        1000
      };
      public const int UndefinedStarpostIndex = -1;
      private readonly Level _level;
      private int _scoreChainIndex;

      public int SpeedShoesTicks { get; private set; }

      public int InvincibillityTicks { get; private set; }

      public int TargetRingCountForNextLife { get; private set; }

      public int TargetScoreForNextLife { get; private set; }

      public int StarpostIndex { get; set; }

      public Vector2i StarpostPosition { get; set; }

      public int StarpostTime { get; set; }

      public ICharacter Protagonist { get; set; }

      public CharacterType ProtagonistCharacterType { get; set; }

      public int ProtagonistGamepadIndex { get; set; }

      public ICharacter Sidekick { get; set; }

      public CharacterType SidekickCharacterType { get; set; }

      public int SidekickGamepadIndex { get; set; }

      public int Score { get; set; }

      public int Lives { get; set; }

      public int TotalRings { get; set; }

      public int CurrentRings { get; set; }

      public Player(Level level)
      {
        this._level = level;
        this.TargetRingCountForNextLife = 100;
        this.TargetScoreForNextLife = 50000;
        this.StarpostIndex = -1;
        this.Lives = 3;
      }

      public void Update()
      {
        if (this.SpeedShoesTicks > 0)
        {
          --this.SpeedShoesTicks;
          if (this.SpeedShoesTicks == 0)
            this.RemoveSpeedShoes();
        }
        if (this.InvincibillityTicks <= 0)
          return;
        --this.InvincibillityTicks;
        if (this.InvincibillityTicks != 0)
          return;
        this.RemoveInvincibility();
      }

      public void GainScore(int points)
      {
        this.Score += points;
        if (this.Score < this.TargetScoreForNextLife)
          return;
        this.TargetScoreForNextLife += 50000;
        this.GainLives();
      }

      public void GainRings(int count = 1)
      {
        this.TotalRings += count;
        this.CurrentRings = Math.Min(this.CurrentRings + count, 999);
        if (this.CurrentRings < this.TargetRingCountForNextLife)
          return;
        this.TargetRingCountForNextLife += 100;
        this.GainLives();
      }

      public void GainLives(int count = 1)
      {
        if (this.Lives >= 0)
          this.Lives = Math.Min(this.Lives + count, 999);
        this._level.SoundManager.PlayJingle(JingleType.Life);
      }

      public void LoseAllRings()
      {
        this.TargetRingCountForNextLife = 100;
        this.CurrentRings = 0;
      }

      public void ResetRings()
      {
        this.TargetRingCountForNextLife = 100;
        this.CurrentRings = 0;
        this.TotalRings = 0;
      }

      public int AwardChainedScore()
      {
        int points = this._scoreChainIndex >= 16 /*0x10*/ ? 10000 : Player.ChainScores[Math.Min(this._scoreChainIndex, Player.ChainScores.Length - 1)];
        ++this._scoreChainIndex;
        this.GainScore(points);
        return points;
      }

      public void ResetScoreChain() => this._scoreChainIndex = 0;

      public void GiveBarrier(BarrierType type) => this.Protagonist.Barrier = type;

      public void GiveSpeedShoes()
      {
        this.SpeedShoesTicks = 1200;
        this.Protagonist.HasSpeedShoes = true;
        this._level.SoundManager.PlayJingle(JingleType.SpeedShoes);
      }

      public void RemoveSpeedShoes()
      {
        if (this.Protagonist != null)
          this.Protagonist.HasSpeedShoes = false;
        this._level.SoundManager.StopJingle(JingleType.SpeedShoes);
      }

      public void GiveInvincibility()
      {
        this.InvincibillityTicks = 1200;
        this.Protagonist.IsInvincible = true;
        this._level.SoundManager.PlayJingle(JingleType.Invincibility);
      }

      public void RemoveInvincibility()
      {
        if (this.Protagonist != null)
          this.Protagonist.IsInvincible = false;
        this._level.SoundManager.StopJingle(JingleType.Invincibility);
      }

      public void RemovePowerups()
      {
        this.RemoveInvincibility();
        this.RemoveSpeedShoes();
      }

      public bool IsStarpostActivated(int index) => index <= this.StarpostIndex;

      public void ActivateStarpost(int index, Vector2i position)
      {
        this.StarpostIndex = index;
        this.StarpostPosition = position;
        this.StarpostTime = this._level.Time;
      }
    }
}
