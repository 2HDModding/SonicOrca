// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelDependenciesResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SonicOrca.Core
{

    public class LevelDependenciesResourceType : ResourceType
    {
      public override string Name => "dependencies, xml";

      public override string DefaultExtension => ".dependencies.xml";

      public override bool CompressByDefault => true;

      public LevelDependenciesResourceType()
        : base(ResourceTypeIdentifier.LevelDependencies)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        XmlDocument xmlDocument = new XmlDocument();
        await Task.Run((Action) (() => xmlDocument.Load(e.InputStream)));
        LevelDependencies levelDependencies = new LevelDependencies();
        levelDependencies.Resource = e.Resource;
        e.PushDependencies(xmlDocument.SelectSingleNode("Dependencies").SelectNodes("Dependency").OfType<XmlNode>().Select<XmlNode, string>((Func<XmlNode, string>) (x => x.SelectSingleNode("Key").InnerText)).Distinct<string>());
        return (ILoadedResource) levelDependencies;
      }
    }
}
