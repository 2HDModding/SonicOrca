// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Tiles.TileSetResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Extensions;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SonicOrca.Core.Tiles
{

    internal class TileSetResourceType : ResourceType
    {
      public override string Name => "tileset, xml";

      public override string DefaultExtension => ".tileset.xml";

      public override bool CompressByDefault => true;

      public TileSetResourceType()
        : base(ResourceTypeIdentifier.TileSet)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        XmlDocument xmlDocument = new XmlDocument();
        await Task.Run((Action) (() => xmlDocument.Load(e.InputStream)));
        XmlNode xmlNode = xmlDocument.SelectSingleNode("tileset");
        IEnumerable<string> strings = xmlNode.SelectNodes("textures/texture").OfType<XmlNode>().Select<XmlNode, string>((Func<XmlNode, string>) (x => e.GetAbsolutePath(x.InnerText)));
        e.PushDependencies(strings);
        TileSet tileSet = new TileSet(e.ResourceTree, strings);
        tileSet.Resource = e.Resource;
        IEnumerable<Tile> source1 = xmlNode.SelectNodes("tiles/tile").OfType<XmlNode>().Select<XmlNode, Tile>((Func<XmlNode, Tile>) (x => TileSetResourceType.ParseXmlTile(x, tileSet)));
        IEnumerable<TileSequence> source2 = xmlNode.SelectNodes("tiles/tileseq").OfType<XmlNode>().Select<XmlNode, TileSequence>((Func<XmlNode, TileSequence>) (x => TileSetResourceType.ParseXmlTileSequence(x, tileSet)));
        foreach (ITile tile in source1.Cast<ITile>().Concat<ITile>(source2.Cast<ITile>()))
          tileSet[tile.Id] = tile;
        return (ILoadedResource) tileSet;
      }

      private static Tile ParseXmlTile(XmlNode node, TileSet tileSet)
      {
        int id = int.Parse(node.Attributes["id"].Value);
        string s;
        int defaultTextureId = node.TryGetAttributeValue("texture", out s) ? int.Parse(s) : 0;
        int defaultX = node.TryGetAttributeValue("x", out s) ? int.Parse(s) : 0;
        int defaultY = node.TryGetAttributeValue("y", out s) ? int.Parse(s) : 0;
        int defaultDelay = node.TryGetAttributeValue("delay", out s) ? int.Parse(s) : 0;
        float defaultOpacity = node.TryGetAttributeValue("opacity", out s) ? float.Parse(s) : 1f;
        TileBlendMode blend = TileBlendMode.Alpha;
        TileBlendMode result;
        if (node.TryGetAttributeValue("blend", out s) && Enum.TryParse<TileBlendMode>(s, true, out result))
          blend = result;
        XmlNode[] array = node.SelectNodes("frame").OfType<XmlNode>().ToArray<XmlNode>();
        IEnumerable<Tile.Frame> frames1;
        if (array.Length == 0)
          frames1 = (IEnumerable<Tile.Frame>) new Tile.Frame[1]
          {
            new Tile.Frame()
            {
              TextureId = defaultTextureId,
              X = defaultX,
              Y = defaultY,
              Delay = defaultDelay,
              Opacity = defaultOpacity
            }
          };
        else
          frames1 = ((IEnumerable<XmlNode>) array).Select<XmlNode, Tile.Frame>((Func<XmlNode, Tile.Frame>) (x => TileSetResourceType.ParseXmlTileFrame(x, defaultX, defaultY, defaultTextureId, defaultDelay, defaultOpacity)));
        IEnumerable<Tile.Frame> frames2 = frames1;
        return new Tile(tileSet, id, frames2, blend);
      }

      private static Tile.Frame ParseXmlTileFrame(
        XmlNode node,
        int defaultX,
        int defaultY,
        int defaultTextureId,
        int defaultDelay,
        float defaultOpacity)
      {
        string s;
        return new Tile.Frame()
        {
          TextureId = node.TryGetAttributeValue("texture", out s) ? int.Parse(s) : defaultTextureId,
          X = node.TryGetAttributeValue("x", out s) ? int.Parse(s) : defaultX,
          Y = node.TryGetAttributeValue("y", out s) ? int.Parse(s) : defaultY,
          Delay = node.TryGetAttributeValue("delay", out s) ? int.Parse(s) : defaultDelay,
          Opacity = node.TryGetAttributeValue("opacity", out s) ? float.Parse(s) : defaultOpacity
        };
      }

      private static TileSequence ParseXmlTileSequence(XmlNode node, TileSet tileSet)
      {
        int id = int.Parse(node.Attributes["id"].Value);
        List<int> tileIds = new List<int>();
        foreach (XmlNode node1 in node.SelectNodes("tile").OfType<XmlNode>().ToArray<XmlNode>())
        {
          string s;
          if (node1.TryGetAttributeValue("id", out s))
            tileIds.Add(int.Parse(s));
        }
        return new TileSequence(tileSet, id, (IEnumerable<int>) tileIds);
      }
    }
}
