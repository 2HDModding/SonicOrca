// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelMapWriter
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace SonicOrca.Core
{

    public class LevelMapWriter
    {
      private readonly LevelMap _levelMap;

      public LevelMapWriter(LevelMap map) => this._levelMap = map;

      public void Save(string path)
      {
        XmlWriterSettings settings = new XmlWriterSettings()
        {
          Indent = true
        };
        using (XmlWriter xmlWriter1 = XmlWriter.Create(path, settings))
        {
          xmlWriter1.WriteStartDocument();
          xmlWriter1.WriteStartElement("map");
          xmlWriter1.WriteStartElement("tiles");
          foreach (ILevelLayerTreeNode child in (IEnumerable<ILevelLayerTreeNode>) this._levelMap.LayerTree.Children)
            this.WriteLevelLayerTreeNode(xmlWriter1, child);
          xmlWriter1.WriteEndElement();
          if (this._levelMap.CollisionVectors.Count > 0)
          {
            xmlWriter1.WriteStartElement("collision");
            xmlWriter1.WriteStartElement("paths");
            foreach (int collisionPathLayer in (IEnumerable<int>) this._levelMap.CollisionPathLayers)
            {
              xmlWriter1.WriteStartElement(nameof (path));
              xmlWriter1.WriteAttributeString("layer", collisionPathLayer.ToString());
              xmlWriter1.WriteEndElement();
            }
            xmlWriter1.WriteEndElement();
            xmlWriter1.WriteStartElement("vectors");
            foreach (CollisionVector collisionVector in (IEnumerable<CollisionVector>) this._levelMap.CollisionVectors)
            {
              xmlWriter1.WriteStartElement("vector");
              xmlWriter1.WriteAttributeString("a", $"{(object) collisionVector.RelativeA.X},{(object) collisionVector.RelativeA.Y}");
              xmlWriter1.WriteAttributeString("b", $"{(object) collisionVector.RelativeB.X},{(object) collisionVector.RelativeB.Y}");
              xmlWriter1.WriteAttributeString("paths", collisionVector.Paths.ToString());
              xmlWriter1.WriteAttributeString("flags", ((int) collisionVector.Flags).ToString());
              xmlWriter1.WriteEndElement();
            }
            xmlWriter1.WriteEndElement();
            xmlWriter1.WriteEndElement();
          }
          if (this._levelMap.Markers.Count > 0)
          {
            xmlWriter1.WriteStartElement("markers");
            foreach (LevelMarker marker in (IEnumerable<LevelMarker>) this._levelMap.Markers)
            {
              xmlWriter1.WriteStartElement("marker");
              if (!string.IsNullOrEmpty(marker.Name))
                xmlWriter1.WriteAttributeString("name", marker.Name);
              if (!string.IsNullOrEmpty(marker.Tag))
                xmlWriter1.WriteAttributeString("tag", marker.Tag);
              XmlWriter xmlWriter2 = xmlWriter1;
              Rectanglei bounds = marker.Bounds;
              int num = bounds.X;
              string str1 = num.ToString();
              xmlWriter2.WriteAttributeString("x", str1);
              XmlWriter xmlWriter3 = xmlWriter1;
              bounds = marker.Bounds;
              num = bounds.Y;
              string str2 = num.ToString();
              xmlWriter3.WriteAttributeString("y", str2);
              bounds = marker.Bounds;
              if (bounds.Width > 0)
              {
                XmlWriter xmlWriter4 = xmlWriter1;
                bounds = marker.Bounds;
                num = bounds.Width;
                string str3 = num.ToString();
                xmlWriter4.WriteAttributeString("width", str3);
              }
              bounds = marker.Bounds;
              if (bounds.Height > 0)
              {
                XmlWriter xmlWriter5 = xmlWriter1;
                bounds = marker.Bounds;
                num = bounds.Height;
                string str4 = num.ToString();
                xmlWriter5.WriteAttributeString("height", str4);
              }
              if (marker.Layer != null)
              {
                XmlWriter xmlWriter6 = xmlWriter1;
                num = this._levelMap.Layers.IndexOf(marker.Layer);
                string str5 = num.ToString();
                xmlWriter6.WriteAttributeString("layer", str5);
              }
              xmlWriter1.WriteEndElement();
            }
            xmlWriter1.WriteEndElement();
          }
          xmlWriter1.WriteEndElement();
          xmlWriter1.WriteEndDocument();
        }
      }

      private void WriteLevelLayerTreeNode(XmlWriter xmlWriter, ILevelLayerTreeNode node)
      {
        if (node is LevelLayerGroup)
        {
          xmlWriter.WriteStartElement("layergroup");
          xmlWriter.WriteAttributeString("name", node.Name);
          foreach (ILevelLayerTreeNode child in (IEnumerable<ILevelLayerTreeNode>) node.Children)
            this.WriteLevelLayerTreeNode(xmlWriter, child);
          xmlWriter.WriteEndElement();
        }
        else
        {
          LevelLayer layer = (LevelLayer) node;
          xmlWriter.WriteStartElement("layer");
          if (!string.IsNullOrEmpty(layer.Name))
            xmlWriter.WriteAttributeString("name", layer.Name);
          Colour colour;
          if (layer.MiniMapColour != new Colour())
          {
            XmlWriter xmlWriter1 = xmlWriter;
            colour = layer.MiniMapColour;
            string hexString = colour.ToHexString();
            xmlWriter1.WriteAttributeString("minimap_colour", hexString);
          }
          if (layer.LayerRowDefinitions.Count > 0)
          {
            xmlWriter.WriteStartElement("rowdefinitions");
            foreach (LayerRowDefinition layerRowDefinition in (IEnumerable<LayerRowDefinition>) layer.LayerRowDefinitions)
            {
              xmlWriter.WriteStartElement("rowdefinition");
              int num1;
              if (layerRowDefinition.Width != 0)
              {
                XmlWriter xmlWriter2 = xmlWriter;
                num1 = layerRowDefinition.Width;
                string str = num1.ToString();
                xmlWriter2.WriteAttributeString("width", str);
              }
              XmlWriter xmlWriter3 = xmlWriter;
              num1 = layerRowDefinition.Height;
              string str1 = num1.ToString();
              xmlWriter3.WriteAttributeString("height", str1);
              double num2;
              if (layerRowDefinition.Parallax != 1.0)
              {
                XmlWriter xmlWriter4 = xmlWriter;
                num2 = layerRowDefinition.Parallax;
                string str2 = num2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                xmlWriter4.WriteAttributeString("parallax_x", str2);
              }
              if (layerRowDefinition.Velocity != 0.0)
              {
                XmlWriter xmlWriter5 = xmlWriter;
                num2 = layerRowDefinition.Velocity;
                string str3 = num2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                xmlWriter5.WriteAttributeString("velocity_x", str3);
              }
              if (layerRowDefinition.InitialOffset != 0)
              {
                XmlWriter xmlWriter6 = xmlWriter;
                num1 = layerRowDefinition.InitialOffset;
                string str4 = num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                xmlWriter6.WriteAttributeString("offset_x", str4);
              }
              xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
          }
          double num3;
          if (layer.Lighting.Type != LevelLayerLightingType.Outside || layer.Lighting.Light != 1.0)
          {
            xmlWriter.WriteStartElement("lighting");
            xmlWriter.WriteAttributeString("type", layer.Lighting.Type.ToString().ToLower());
            XmlWriter xmlWriter7 = xmlWriter;
            num3 = layer.Lighting.Light;
            string str = num3.ToString();
            xmlWriter7.WriteAttributeString("light", str);
            xmlWriter.WriteEndElement();
          }
          if (layer.Shadows.Count > 0)
          {
            xmlWriter.WriteStartElement("shadows");
            foreach (LevelLayerShadow shadow in (IEnumerable<LevelLayerShadow>) layer.Shadows)
            {
              xmlWriter.WriteStartElement("shadow");
              if (!shadow.Tiles)
                xmlWriter.WriteAttributeString("tiles", "false");
              if (!shadow.Objects)
                xmlWriter.WriteAttributeString("tiles", "false");
              XmlWriter xmlWriter8 = xmlWriter;
              int num4 = shadow.LayerIndexOffset;
              string str5 = num4.ToString();
              xmlWriter8.WriteAttributeString("layerIndexOffset", str5);
              XmlWriter xmlWriter9 = xmlWriter;
              num4 = shadow.Displacement.X;
              string str6 = num4.ToString();
              xmlWriter9.WriteAttributeString("dx", str6);
              XmlWriter xmlWriter10 = xmlWriter;
              num4 = shadow.Displacement.Y;
              string str7 = num4.ToString();
              xmlWriter10.WriteAttributeString("dy", str7);
              if (shadow.Softness != 0)
              {
                XmlWriter xmlWriter11 = xmlWriter;
                num4 = shadow.Softness;
                string str8 = num4.ToString();
                xmlWriter11.WriteAttributeString("softness", str8);
              }
              if (shadow.Colour != LevelLayerShadow.DefaultShadowColour)
              {
                XmlWriter xmlWriter12 = xmlWriter;
                colour = shadow.Colour;
                string hexString = colour.ToHexString();
                xmlWriter12.WriteAttributeString("colour", hexString);
              }
              xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
          }
          if (layer.Rows > 0)
          {
            xmlWriter.WriteStartElement("tiles");
            int num5;
            if (layer.OffsetY != 0)
            {
              XmlWriter xmlWriter13 = xmlWriter;
              num5 = layer.OffsetY;
              string str = num5.ToString();
              xmlWriter13.WriteAttributeString("offset_y", str);
            }
            if (layer.AutomaticYParallax)
              xmlWriter.WriteAttributeString("parallax_y", "auto");
            else if (layer.ParallaxY != 1.0)
            {
              XmlWriter xmlWriter14 = xmlWriter;
              num3 = layer.ParallaxY;
              string str = num3.ToString((IFormatProvider) CultureInfo.InvariantCulture);
              xmlWriter14.WriteAttributeString("parallax_y", str);
            }
            if (layer.WrapX)
              xmlWriter.WriteAttributeString("wrap_x", "true");
            if (layer.WrapY)
              xmlWriter.WriteAttributeString("wrap_y", "true");
            for (int y = 0; y < layer.Rows; num5 = y++)
            {
              xmlWriter.WriteStartElement("row");
              IEnumerable<int> source = Enumerable.Range(0, layer.Columns).Select<int, int>((Func<int, int>) (x => layer.Tiles[x, y]));
              xmlWriter.WriteString(string.Join(",", source.Select<int, string>((Func<int, string>) (tileIndex => ((tileIndex & 16384 /*0x4000*/) != 0 ? (object) "h" : (object) string.Empty).ToString() + ((tileIndex & 32768 /*0x8000*/) != 0 ? (object) "v" : (object) string.Empty) + (object) (tileIndex & 4095 /*0x0FFF*/)))));
              xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
          }
          xmlWriter.WriteEndElement();
        }
      }
    }
}
