// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelBindingWriter
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace SonicOrca.Core
{

    public class LevelBindingWriter
    {
      private readonly LevelBinding _binding;

      public LevelBindingWriter(LevelBinding binding) => this._binding = binding;

      public void Save(string path)
      {
        XmlWriterSettings settings = new XmlWriterSettings()
        {
          Indent = true
        };
        using (XmlWriter writer = XmlWriter.Create(path, settings))
        {
          writer.WriteStartDocument();
          writer.WriteStartElement("Binding");
          int key = this._binding.ObjectPlacements.GroupBy<ObjectPlacement, int>((Func<ObjectPlacement, int>) (x => x.Layer)).OrderByDescending<IGrouping<int, ObjectPlacement>, int>((Func<IGrouping<int, ObjectPlacement>, int>) (x => x.Count<ObjectPlacement>())).First<IGrouping<int, ObjectPlacement>>().Key;
          writer.WriteStartElement("Definitions");
          writer.WriteAttributeString("DefaultLayer", key.ToString());
          foreach (ObjectPlacement objectPlacement in (IEnumerable<ObjectPlacement>) this._binding.ObjectPlacements)
          {
            writer.WriteStartElement("Definition");
            writer.WriteStartElement("Common");
            writer.WriteStartElement("Key");
            writer.WriteString(objectPlacement.Key.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Uid");
            writer.WriteString(objectPlacement.Uid.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Name");
            writer.WriteString(objectPlacement.Name.ToString());
            writer.WriteEndElement();
            if (objectPlacement.Layer != key)
            {
              writer.WriteStartElement("Layer");
              writer.WriteString(objectPlacement.Layer.ToString());
              writer.WriteEndElement();
            }
            writer.WriteStartElement("Position");
            XmlWriter xmlWriter1 = writer;
            Vector2i position = objectPlacement.Position;
            string str1 = position.X.ToString();
            xmlWriter1.WriteAttributeString("X", str1);
            XmlWriter xmlWriter2 = writer;
            position = objectPlacement.Position;
            string str2 = position.Y.ToString();
            xmlWriter2.WriteAttributeString("Y", str2);
            writer.WriteEndElement();
            writer.WriteEndElement();
            if (objectPlacement.Behaviour.Count > 0)
            {
              writer.WriteStartElement("Behaviour");
              this.WriteObjectBehaviour(writer, (IEnumerable<KeyValuePair<string, object>>) objectPlacement.Behaviour);
              writer.WriteEndElement();
            }
            if (objectPlacement.Mappings.Count > 0)
            {
              writer.WriteStartElement("Mappings");
              this.WriteObjectBehaviour(writer, (IEnumerable<KeyValuePair<string, object>>) objectPlacement.Mappings);
              writer.WriteEndElement();
            }
            writer.WriteEndElement();
          }
          writer.WriteEndElement();
          writer.WriteEndElement();
          writer.WriteEndDocument();
        }
      }

      private void WriteObjectBehaviour(
        XmlWriter writer,
        IEnumerable<KeyValuePair<string, object>> behaviour)
      {
        foreach (KeyValuePair<string, object> keyValuePair in behaviour)
        {
          writer.WriteStartElement(keyValuePair.Key);
          if (keyValuePair.Value is IEnumerable<KeyValuePair<string, object>>)
            this.WriteObjectBehaviour(writer, (IEnumerable<KeyValuePair<string, object>>) keyValuePair.Value);
          else
            writer.WriteString(Convert.ToString(keyValuePair.Value, (IFormatProvider) CultureInfo.InvariantCulture));
          writer.WriteEndElement();
        }
      }

      public void ConvertObjectEntriesToPlacements()
      {
        this._binding.ObjectPlacements.Clear();
        foreach (ObjectEntry objectEntry in this._binding.Level.ObjectManager.ObjectEntryTable)
        {
          ObjectPlacement objectPlacement = new ObjectPlacement(objectEntry.Type.ResourceKey, objectEntry.Uid, objectEntry.Name, objectEntry.Layer, objectEntry.Position, (object) objectEntry.State);
          foreach (ObjectMapping mapping in (IEnumerable<ObjectMapping>) objectEntry.Mappings)
            (objectPlacement.Mappings as List<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>(mapping.Field, (object) mapping.Target));
          this._binding.ObjectPlacements.Add(objectPlacement);
        }
      }
    }
}
