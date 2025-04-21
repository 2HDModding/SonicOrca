// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelDependenciesWriter
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Linq;
using System.Xml;

namespace SonicOrca.Core
{

    public class LevelDependenciesWriter
    {
      private readonly LevelBinding _binding;

      public LevelDependenciesWriter(LevelBinding binding) => this._binding = binding;

      public void Save(string path)
      {
        XmlWriterSettings settings = new XmlWriterSettings()
        {
          Indent = true
        };
        using (XmlWriter xmlWriter = XmlWriter.Create(path, settings))
        {
          xmlWriter.WriteStartDocument();
          xmlWriter.WriteStartElement("Dependencies");
          foreach (string str in this._binding.ObjectPlacements.Select<ObjectPlacement, string>((Func<ObjectPlacement, string>) (p => p.Key)).Distinct<string>())
          {
            xmlWriter.WriteStartElement("Dependency");
            xmlWriter.WriteStartElement("Key");
            xmlWriter.WriteString(str.ToString());
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
          }
          xmlWriter.WriteEndElement();
          xmlWriter.WriteEndDocument();
        }
      }
    }
}
