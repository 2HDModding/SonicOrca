// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelPrepareSettings
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Core
{

    public class LevelPrepareSettings
    {
      public bool TimeTrial { get; set; }

      public CharacterType ProtagonistCharacter { get; set; }

      public CharacterType SidekickCharacter { get; set; }

      public string RecordedPlayReadPath { get; set; }

      public string RecordedPlayWritePath { get; set; }

      public string RecordedPlayGhostReadPath { get; set; }

      public byte[] RecordedPlayReadData { get; set; }

      public int LevelNumber { get; set; }

      public string AreaResourceKey { get; set; }

      public int Act { get; set; }

      public Vector2i? StartPosition { get; set; }

      public int StartPath { get; set; }

      public bool Seamless { get; set; }

      public int Lives { get; set; }

      public int Score { get; set; }

      public bool Debugging { get; set; }

      public double NightMode { get; set; }

      public LevelPrepareSettings() => this.Lives = 3;
    }
}
