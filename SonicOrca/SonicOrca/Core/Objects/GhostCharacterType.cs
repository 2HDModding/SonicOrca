// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.GhostCharacterType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Objects.Metadata;

namespace SonicOrca.Core.Objects
{

    [SonicOrca.Core.Objects.Metadata.Name("Ghost")]
    [Description("Ghost character simulating playback.")]
    [ObjectInstance(typeof (GhostCharacterInstance))]
    internal class GhostCharacterType : ObjectType
    {
      public const string VirtualResourceKey = "SONICORCA/OBJECTS/GHOST";
    }
}
