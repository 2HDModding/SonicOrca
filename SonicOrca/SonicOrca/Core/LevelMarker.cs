// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelMarker
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;

namespace SonicOrca.Core
{

    public class LevelMarker
    {
      private string _tag = "";

      public string Name { get; set; }

      public string Tag
      {
        get => this._tag;
        set => this._tag = value == null ? string.Empty : value;
      }

      public Rectanglei Bounds { get; set; }

      public LevelLayer Layer { get; set; }

      public Vector2i Position
      {
        get => this.Bounds.TopLeft;
        set => this.Bounds = new Rectanglei(value.X, value.Y, 0, 0);
      }

      public LevelMarker(string name, string tag, Vector2i position, LevelLayer layer)
      {
        this.Name = name;
        this.Tag = tag;
        this.Bounds = new Rectanglei(position.X, position.Y, 0, 0);
        this.Layer = layer;
      }

      public LevelMarker(string name, string tag, Rectanglei bounds, LevelLayer layer)
      {
        this.Name = name;
        this.Tag = tag;
        this.Bounds = bounds;
        this.Layer = layer;
      }

      public override string ToString() => this.Name;
    }
}
