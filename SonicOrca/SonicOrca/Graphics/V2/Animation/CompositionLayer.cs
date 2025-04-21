// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Animation.CompositionLayer
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Graphics.V2.Animation
{

    public class CompositionLayer
    {
      private const uint _ddd = 0;
      private uint _index;
      private CompositionLayer.LayerKind _layerType;
      private CompositionLayer.LayerSubKind _layerSubType;
      private BlendMode _layerBlendMode;
      private string _name;
      private string _fileExtension;
      private string _textureReference;
      private const double _sampleRate = 0.0;
      private uint _startFrame;
      private uint _endFrame;
      private const uint _bm = 0;
      private uint _ellapsedFrames;
      private CompositionLayerAnimatableTransform _transform;

      public string TextureReference => this._textureReference;

      public uint StartFrame => this._startFrame;

      public uint EndFrame => this._endFrame;

      public uint Index => this._index;

      public CompositionLayer.LayerKind Kind => this._layerType;

      public CompositionLayer.LayerSubKind SubKind => this._layerSubType;

      public BlendMode BlendMode => this._layerBlendMode;

      public CompositionLayerAnimatableTransform Transform => this._transform;

      public CompositionLayer(
        uint index,
        uint layerType,
        uint layerSubType,
        uint layerBlendMode,
        string name,
        string fileExtension,
        string textureReference,
        uint startFrame,
        uint endFrame,
        CompositionLayerAnimatableTransform transform)
      {
        this._index = index;
        if (layerType != 2U)
          throw new NotImplementedException();
        this._layerType = CompositionLayer.LayerKind.Image;
        if (layerSubType != 0U)
        {
          if (layerSubType != 1U)
            throw new NotImplementedException();
          this._layerSubType = CompositionLayer.LayerSubKind.Mask;
        }
        else
          this._layerSubType = CompositionLayer.LayerSubKind.None;
        switch (layerBlendMode)
        {
          case 0:
            this._layerBlendMode = BlendMode.Alpha;
            break;
          case 1:
            this._layerBlendMode = BlendMode.Additive;
            break;
          case 2:
            this._layerBlendMode = BlendMode.Opaque;
            break;
        }
        this._name = name;
        this._fileExtension = fileExtension;
        this._textureReference = textureReference;
        this._startFrame = startFrame;
        this._endFrame = endFrame;
        this._transform = transform;
      }

      public void ResetFrame()
      {
        this._ellapsedFrames = 0U;
        this._transform.ResetFrame();
      }

      public void Animate()
      {
        if (this._ellapsedFrames <= this._endFrame)
          this._transform.Animate();
        if (this._ellapsedFrames > this._endFrame)
          return;
        ++this._ellapsedFrames;
      }

      public enum LayerKind
      {
        Image,
      }

      public enum LayerSubKind
      {
        None,
        Mask,
      }
    }
}
