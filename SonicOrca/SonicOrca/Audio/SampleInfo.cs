// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.SampleInfo
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;

namespace SonicOrca.Audio
{

    public class SampleInfo : ILoadedResource, IDisposable
    {
      private readonly ResourceTree _resourceTree;
      private readonly string _sampleResourceKeyPath;
      private readonly int? _loopSampleIndex;

      public Resource Resource { get; set; }

      public Sample Sample { get; private set; }

      public bool HasLoopPoint => this._loopSampleIndex.HasValue;

      public int LoopSampleIndex => this._loopSampleIndex.Value;

      public SampleInfo(Sample sample, int? loopSampleIndex = null)
      {
        this.Sample = sample;
        this._loopSampleIndex = loopSampleIndex;
      }

      public SampleInfo(ResourceTree resourceTree, string sampleResourceKeyPath, int? loopSampleIndex = null)
      {
        this._resourceTree = resourceTree;
        this._sampleResourceKeyPath = sampleResourceKeyPath;
        this._loopSampleIndex = loopSampleIndex;
      }

      public void OnLoaded()
      {
        if (this._sampleResourceKeyPath == null)
          return;
        this.Sample = this._resourceTree.GetLoadedResource<Sample>(this._sampleResourceKeyPath);
      }

      public void Dispose()
      {
      }
    }
}
