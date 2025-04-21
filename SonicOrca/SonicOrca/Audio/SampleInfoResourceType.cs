// Decompiled with JetBrains decompiler
// Type: SonicOrca.Audio.SampleInfoResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Extensions;
using SonicOrca.Resources;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SonicOrca.Audio
{

    internal class SampleInfoResourceType : ResourceType
    {
      public override string Name => "sample info";

      public override string DefaultExtension => ".sampleinfo.xml";

      public override bool CompressByDefault => true;

      public SampleInfoResourceType()
        : base(ResourceTypeIdentifier.SampleInfo)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        return (ILoadedResource) await Task.Run<SampleInfo>((Func<SampleInfo>) (() =>
        {
          XmlDocument node = new XmlDocument();
          node.Load(e.InputStream);
          string absolutePath = e.GetAbsolutePath(node.SelectSingleNode("sampleinfo/sample").InnerText);
          e.PushDependency(absolutePath);
          int? loopSampleIndex = new int?();
          string s;
          if (node.TryGetNodeInnerText("sampleinfo/loop", out s))
            loopSampleIndex = new int?(int.Parse(s));
          return new SampleInfo(e.ResourceTree, absolutePath, loopSampleIndex)
          {
            Resource = e.Resource
          };
        }));
      }
    }
}
