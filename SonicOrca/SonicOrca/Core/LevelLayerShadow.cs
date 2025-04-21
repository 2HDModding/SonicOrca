// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelLayerShadow
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;

namespace SonicOrca.Core
{

    public class LevelLayerShadow
    {
      public static readonly Colour DefaultShadowColour = new Colour((byte) 128 /*0x80*/, (byte) 0, (byte) 0, (byte) 0);

      public bool Tiles { get; set; }

      public bool Objects { get; set; }

      public int LayerIndexOffset { get; set; }

      public Vector2i Displacement { get; set; }

      public int Softness { get; set; }

      public Colour Colour { get; set; }

      public LevelLayerShadow()
      {
        this.Tiles = true;
        this.Objects = true;
        this.Colour = LevelLayerShadow.DefaultShadowColour;
      }

      public override string ToString()
      {
        object[] objArray = new object[7]
        {
          (object) (char) (this.Tiles ? 84 : 32 /*0x20*/),
          (object) (char) (this.Objects ? 79 : 32 /*0x20*/),
          (object) this.LayerIndexOffset,
          null,
          null,
          null,
          null
        };
        Vector2i displacement = this.Displacement;
        objArray[3] = (object) displacement.X;
        displacement = this.Displacement;
        objArray[4] = (object) displacement.Y;
        objArray[5] = (object) this.Colour.ToHexString();
        objArray[6] = (object) this.Softness;
        return string.Format("[{0}{1}] {2:+#;-#;0} {3:+#;-#;0},{4:+#;-#;0} {5} @{6}", objArray);
      }
    }
}
