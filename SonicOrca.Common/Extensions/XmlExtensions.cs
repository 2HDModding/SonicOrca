// Decompiled with JetBrains decompiler
// Type: SonicOrca.Extensions.XmlExtensions
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System.Xml;

#nullable disable
namespace SonicOrca.Extensions;

public static class XmlExtensions
{
  public static string GetNodeInnerText(this XmlNode node, string xpath, string defaultValue)
  {
    string nodeInnerText;
    if (!node.TryGetNodeInnerText(xpath, out nodeInnerText))
      nodeInnerText = defaultValue;
    return nodeInnerText;
  }

  public static bool TryGetNodeInnerText(this XmlNode node, string xpath, out string value)
  {
    XmlNode xmlNode = node.SelectSingleNode(xpath);
    if (xmlNode == null)
    {
      value = (string) null;
      return false;
    }
    value = xmlNode.InnerText;
    return true;
  }

  public static bool TryGetAttributeValue(this XmlNode node, string name, out string value)
  {
    XmlAttribute attribute = node.Attributes[name];
    if (attribute == null)
    {
      value = (string) null;
      return false;
    }
    value = attribute.Value;
    return true;
  }
}
