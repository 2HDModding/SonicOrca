// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.Animation
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Graphics
{

    public class Animation
    {
      private Animation.Frame[] _frames = new Animation.Frame[0];

      public IReadOnlyList<Animation.Frame> Frames
      {
        get => (IReadOnlyList<Animation.Frame>) this._frames;
        set => this._frames = ((IEnumerable<Animation.Frame>) value).ToArray<Animation.Frame>();
      }

      public int? NextFrameIndex { get; set; }

      public int? LoopFrameIndex { get; set; }

      public int Duration
      {
        get
        {
          return ((IEnumerable<Animation.Frame>) this._frames).Sum<Animation.Frame>((Func<Animation.Frame, int>) (x => x.Delay + 1));
        }
      }

      public Animation()
      {
      }

      public Animation(IEnumerable<Animation.Frame> frames)
      {
        this._frames = frames.ToArray<Animation.Frame>();
      }

      public Animation(IEnumerable<Animation.Frame> frames, int? nextFrameIndex, int? loopFrameIndex)
      {
        this._frames = frames.ToArray<Animation.Frame>();
        this.NextFrameIndex = nextFrameIndex;
        this.LoopFrameIndex = loopFrameIndex;
      }

      public struct Frame
      {
        public int TextureIndex { get; set; }

        public Rectanglei Source { get; set; }

        public Vector2i Offset { get; set; }

        public int Delay { get; set; }
      }
    }
}
