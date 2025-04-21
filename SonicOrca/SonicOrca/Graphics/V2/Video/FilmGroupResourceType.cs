// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Video.FilmGroupResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SonicOrca.Graphics.V2.Video
{

    public class FilmGroupResourceType : ResourceType
    {
      public FilmGroupResourceType()
        : base(ResourceTypeIdentifier.FilmGroup)
      {
      }

      public override string Name => "film, xml";

      public override string DefaultExtension => ".film.xml";

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        List<string> filmResourceKeys = new List<string>();
        List<Film> films = new List<Film>();
        XmlDocument xmlDocument = new XmlDocument();
        await Task.Run((Action) (() => xmlDocument.Load(e.InputStream)));
        IEnumerable<XmlNode> xmlNodes = xmlDocument.SelectSingleNode("root").SelectNodes("video").OfType<XmlNode>();
        List<string> stringList = new List<string>();
        foreach (XmlNode xmlNode in xmlNodes)
        {
          string innerText = xmlNode.InnerText;
          stringList.Add(innerText);
        }
        string fullKeyPath = e.Resource.FullKeyPath;
        string str1 = fullKeyPath.Remove(fullKeyPath.LastIndexOf("/"));
        string str2 = str1.Remove(str1.LastIndexOf("/"));
        foreach (string str3 in stringList)
        {
          string path = $"{str2}/{str3.ToUpper()}";
          filmResourceKeys.Add(path);
          films.Add(new Film(path));
        }
        e.PushDependencies((IEnumerable<string>) filmResourceKeys);
        return (ILoadedResource) new FilmGroup(e.ResourceTree, (IEnumerable<string>) filmResourceKeys, (IEnumerable<Film>) films)
        {
          Resource = e.Resource
        };
      }
    }
}
