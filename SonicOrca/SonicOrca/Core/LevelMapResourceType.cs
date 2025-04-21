// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelMapResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using SonicOrca.Core.Extensions;
using SonicOrca.Extensions;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SonicOrca.Core
{

    internal class LevelMapResourceType : ResourceType
    {
      public override string Name => "map, xml";

      public override string DefaultExtension => ".map.xml";

      public override bool CompressByDefault => true;

      public LevelMapResourceType()
        : base(ResourceTypeIdentifier.Map)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        LevelMapResourceType levelMapResourceType1 = this;
        LevelMapResourceType levelMapResourceType = levelMapResourceType1;
        ResourceLoadArgs e1 = e;
        XmlDocument xmlDocument = new XmlDocument();
        await Task.Run((Action) (() => xmlDocument.Load(e1.InputStream)));
        LevelMap map = new LevelMap();
        map.Resource = e1.Resource;
        XmlNode parent = xmlDocument.SelectSingleNode("map/tiles");
        ILevelLayerTreeNode[] array1 = levelMapResourceType1.GetLevelLayersFromXmlNode(parent, map).ToArray<ILevelLayerTreeNode>();
        map.LayerTree.Children.Clear();
        map.LayerTree.Children.AddRange<ILevelLayerTreeNode>((IEnumerable<ILevelLayerTreeNode>) array1);
        LevelLayer[] array2 = map.LayerTree.GetDescendantsOrdered().Select<ILevelLayerTreeNode, LevelLayer>((Func<ILevelLayerTreeNode, LevelLayer>) (x => x as LevelLayer)).Where<LevelLayer>((Func<LevelLayer, bool>) (x => x != null)).ToArray<LevelLayer>();
        map.Layers.Clear();
        map.Layers.AddRange<LevelLayer>((IEnumerable<LevelLayer>) array2);
        XmlNode xmlNode1 = xmlDocument.SelectSingleNode("map/collision");
        if (xmlNode1 != null)
        {
          map.CollisionPathLayers.AddRange<int>(levelMapResourceType1.GetCollisionPathsFromXmlNode(xmlNode1.SelectSingleNode("paths")));
          map.CollisionVectors.AddRange<CollisionVector>(levelMapResourceType1.GetCollisionVectorsFromXmlNode(xmlNode1.SelectSingleNode("vectors")));
        }
        XmlNode xmlNode2 = xmlDocument.SelectSingleNode("map/markers");
        if (xmlNode2 != null)
          map.Markers.AddRange<LevelMarker>(xmlNode2.SelectNodes("marker").OfType<XmlNode>().Select<XmlNode, LevelMarker>((Func<XmlNode, LevelMarker>) (x => levelMapResourceType.GetMarkerFromXmlNode(map, x))));
        return (ILoadedResource) map;
      }

      private IEnumerable<ILevelLayerTreeNode> GetLevelLayersFromXmlNode(XmlNode parent, LevelMap map)
      {
        foreach (XmlNode node in parent.ChildNodes)
        {
          if (node.Name == "layergroup")
          {
            string empty;
            if (!node.TryGetAttributeValue("name", out empty))
              empty = string.Empty;
            LevelLayerGroup levelLayerGroup = new LevelLayerGroup(empty);
            foreach (ILevelLayerTreeNode levelLayerTreeNode in this.GetLevelLayersFromXmlNode(node, map))
              levelLayerGroup.Children.Add(levelLayerTreeNode);
            yield return (ILevelLayerTreeNode) levelLayerGroup;
          }
          else if (node.Name == "layer")
            yield return (ILevelLayerTreeNode) this.GetLevelLayerFromXmlNode(node, map);
        }
      }

      private LevelLayer GetLevelLayerFromXmlNode(XmlNode node, LevelMap map)
      {
        LevelLayer layerFromXmlNode = new LevelLayer(map);
        string s;
        if (node.TryGetAttributeValue("name", out s))
          layerFromXmlNode.Name = s;
        Colour result1;
        if (node.TryGetAttributeValue("minimap_colour", out s) && Colour.TryParseHex(s, out result1))
          layerFromXmlNode.MiniMapColour = result1;
        layerFromXmlNode.LayerRowDefinitions.AddRange<LayerRowDefinition>(node.SelectNodes("rowdefinitions/rowdefinition").OfType<XmlNode>().Select<XmlNode, LayerRowDefinition>((Func<XmlNode, LayerRowDefinition>) (x => this.GetRowDefinitionFromXmlNode(x))));
        XmlNode node1 = node.SelectSingleNode("lighting");
        if (node1 != null)
        {
          LevelLayerLighting levelLayerLighting = new LevelLayerLighting();
          LevelLayerLightingType result2;
          if (node1.TryGetAttributeValue("type", out s) && Enum.TryParse<LevelLayerLightingType>(s, true, out result2))
            levelLayerLighting.Type = result2;
          if (node1.TryGetAttributeValue("light", out s))
            levelLayerLighting.Light = double.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
          layerFromXmlNode.Lighting = levelLayerLighting;
        }
        layerFromXmlNode.Shadows.AddRange<LevelLayerShadow>(node.SelectNodes("shadows/shadow").OfType<XmlNode>().Select<XmlNode, LevelLayerShadow>((Func<XmlNode, LevelLayerShadow>) (x => this.GetShadowFromXmlNode(x))));
        XmlNode node2 = node.SelectSingleNode("tiles");
        if (node2 != null)
        {
          if (node2.TryGetAttributeValue("offset_y", out s))
            layerFromXmlNode.OffsetY = int.Parse(s);
          if (node2.TryGetAttributeValue("parallax_y", out s))
          {
            if (s.Equals("auto", StringComparison.OrdinalIgnoreCase))
              layerFromXmlNode.AutomaticYParallax = true;
            else
              layerFromXmlNode.ParallaxY = double.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
          }
          if (node2.TryGetAttributeValue("wrap_x", out s))
            layerFromXmlNode.WrapX = s.Equals("true", StringComparison.OrdinalIgnoreCase);
          if (node2.TryGetAttributeValue("wrap_y", out s))
            layerFromXmlNode.WrapY = s.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
        int[][] array = node.SelectNodes("tiles/row").OfType<XmlNode>().Select<XmlNode, int[]>((Func<XmlNode, int[]>) (x => this.ReadTileValues(x.InnerText))).ToArray<int[]>();
        if (array.Length != 0)
        {
          layerFromXmlNode.Resize(((IEnumerable<int[]>) array).Max<int[]>((Func<int[], int>) (x => x.Length)), array.Length);
          for (int index1 = 0; index1 < array.Length; ++index1)
          {
            for (int index2 = 0; index2 < Math.Min(layerFromXmlNode.Columns, array[index1].Length); ++index2)
              layerFromXmlNode.Tiles[index2, index1] = array[index1][index2];
          }
        }
        return layerFromXmlNode;
      }

      private LayerRowDefinition GetRowDefinitionFromXmlNode(XmlNode node)
      {
        LayerRowDefinition definitionFromXmlNode = new LayerRowDefinition();
        string s;
        if (node.TryGetAttributeValue("width", out s))
          definitionFromXmlNode.Width = int.Parse(s);
        if (node.TryGetAttributeValue("height", out s))
          definitionFromXmlNode.Height = int.Parse(s);
        if (node.TryGetAttributeValue("parallax_x", out s))
          definitionFromXmlNode.Parallax = double.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
        if (node.TryGetAttributeValue("velocity_x", out s))
          definitionFromXmlNode.Velocity = double.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
        if (node.TryGetAttributeValue("offset_x", out s))
          definitionFromXmlNode.InitialOffset = int.Parse(s);
        return definitionFromXmlNode;
      }

      private LevelLayerShadow GetShadowFromXmlNode(XmlNode node)
      {
        LevelLayerShadow shadowFromXmlNode = new LevelLayerShadow();
        string s;
        if (node.TryGetAttributeValue("tiles", out s))
          shadowFromXmlNode.Tiles = bool.Parse(s);
        if (node.TryGetAttributeValue("objects", out s))
          shadowFromXmlNode.Objects = bool.Parse(s);
        if (node.TryGetAttributeValue("layerIndexOffset", out s))
          shadowFromXmlNode.LayerIndexOffset = int.Parse(s);
        if (node.TryGetAttributeValue("dx", out s))
        {
          Vector2i displacement = shadowFromXmlNode.Displacement with
          {
            X = int.Parse(s)
          };
          shadowFromXmlNode.Displacement = displacement;
        }
        if (node.TryGetAttributeValue("dy", out s))
        {
          Vector2i displacement = shadowFromXmlNode.Displacement with
          {
            Y = int.Parse(s)
          };
          shadowFromXmlNode.Displacement = displacement;
        }
        if (node.TryGetAttributeValue("softness", out s))
          shadowFromXmlNode.Softness = int.Parse(s);
        if (node.TryGetAttributeValue("colour", out s))
          shadowFromXmlNode.Colour = Colour.ParseHex(s);
        return shadowFromXmlNode;
      }

      private void ReadXYAttribute(string attributeValue, out bool x, out bool y)
      {
        string[] strArray = attributeValue.Split(',');
        x = strArray[0] == "true";
        y = strArray[1] == "true";
      }

      private void ReadXYAttribute(string attributeValue, out Vector2 result)
      {
        string[] strArray = attributeValue.Split(',');
        result = new Vector2(double.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture));
      }

      private int[] ReadTileValues(string text)
      {
        StringBuilder stringBuilder = new StringBuilder();
        List<int> intList = new List<int>();
        StringReader reader = new StringReader(text);
        while (reader.CanRead())
        {
          int num1 = 0;
          char c = (char) reader.Peek();
          if (char.ToUpper(c) == 'H')
          {
            num1 |= 16384 /*0x4000*/;
            reader.Read();
            c = (char) reader.Peek();
          }
          if (char.ToUpper(c) == 'V')
          {
            num1 |= 32768 /*0x8000*/;
            reader.Read();
            c = (char) reader.Peek();
          }
          stringBuilder.Clear();
          for (; char.IsNumber(c); c = (char) reader.Peek())
          {
            reader.Read();
            stringBuilder.Append(c);
          }
          int num2 = num1 | int.Parse(stringBuilder.ToString());
          intList.Add(num2);
          if (c != ',' && c != char.MaxValue)
            throw new Exception();
          reader.Read();
        }
        return intList.ToArray();
      }

      private LevelMarker GetMarkerFromXmlNode(LevelMap map, XmlNode node)
      {
        Rectanglei bounds = new Rectanglei();
        bounds.X = int.Parse(node.Attributes["x"].Value);
        bounds.Y = int.Parse(node.Attributes["y"].Value);
        string s;
        if (node.TryGetAttributeValue("width", out s))
          bounds.Width = int.Parse(s);
        if (node.TryGetAttributeValue("height", out s))
          bounds.Height = int.Parse(s);
        string name = (string) null;
        if (node.TryGetAttributeValue("name", out s))
          name = s;
        string tag = (string) null;
        if (node.TryGetAttributeValue("tag", out s))
          tag = s;
        LevelLayer layer = (LevelLayer) null;
        if (node.TryGetAttributeValue("layer", out s))
          layer = map.Layers[int.Parse(s)];
        return new LevelMarker(name, tag, bounds, layer);
      }

      private IEnumerable<CollisionVector> GetCollisionVectorsFromXmlNode(XmlNode parent)
      {
        foreach (XmlNode childNode in parent.ChildNodes)
        {
          Vector2i xy1;
          Vector2i xy2;
          string s;
          uint result1;
          int result2;
          if (!(childNode.Name != "vector") && this.TryGetAttributeXY(childNode, "a", out xy1) && this.TryGetAttributeXY(childNode, "b", out xy2) && childNode.TryGetAttributeValue("paths", out s) && uint.TryParse(s, out result1) && childNode.TryGetAttributeValue("flags", out s) && int.TryParse(s, out result2))
            yield return new CollisionVector(xy1, xy2, result1, (CollisionFlags) result2);
        }
      }

      private IEnumerable<int> GetCollisionPathsFromXmlNode(XmlNode parent)
      {
        foreach (XmlNode childNode in parent.ChildNodes)
        {
          if (!(childNode.Name != "path"))
          {
            string s;
            if (!childNode.TryGetAttributeValue("layer", out s))
              throw new Exception();
            int result;
            if (!int.TryParse(s, out result))
              throw new Exception();
            yield return result;
          }
        }
      }

      private bool TryGetAttributeXY(XmlNode node, string name, out Vector2i xy)
      {
        xy = new Vector2i();
        string str;
        if (!node.TryGetAttributeValue(name, out str))
          return false;
        string[] strArray = str.Split(',');
        int result1;
        int result2;
        if (strArray.Length != 2 || !int.TryParse(strArray[0], out result1) || !int.TryParse(strArray[1], out result2))
          return false;
        xy = new Vector2i(result1, result2);
        return true;
      }
    }
}
