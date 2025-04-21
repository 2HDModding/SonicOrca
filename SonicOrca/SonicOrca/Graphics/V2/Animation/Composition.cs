// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Animation.Composition
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Graphics.V2.Animation
{

    public class Composition
    {
      private string _version;
      private uint _frameRate;
      private uint _startFrame;
      private uint _endFrame;
      private uint _width;
      private uint _height;
      private string _name;
      private const uint _ddd = 0;
      private CompositionLayer[] _layers = new CompositionLayer[0];

      public uint StartFrame => this._startFrame;

      public uint EndFrame => this._endFrame;

      public IReadOnlyList<CompositionLayer> Layers
      {
        get => (IReadOnlyList<CompositionLayer>) this._layers;
        set => this._layers = ((IEnumerable<CompositionLayer>) value).ToArray<CompositionLayer>();
      }

      public Composition(
        string version,
        uint frameRate,
        uint startFrame,
        uint endFrame,
        uint width,
        uint height,
        string name,
        IEnumerable<CompositionLayer> layers)
      {
        this._version = version;
        this._frameRate = frameRate;
        this._startFrame = startFrame;
        this._endFrame = endFrame;
        this._width = width;
        this._height = height;
        this._name = name;
        this._layers = layers.ToArray<CompositionLayer>();
      }

      public struct Frame
      {
        public int TextureIndex { get; set; }

        public Rectanglei Source { get; set; }

        public double Opacity { get; set; }

        public Vector2 Position { get; set; }

        public double Rotation { get; set; }

        public Vector2 Scale { get; set; }
      }
    }
}
