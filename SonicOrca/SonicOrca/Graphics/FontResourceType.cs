// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.FontResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Extensions;
using SonicOrca.Geometry;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SonicOrca.Graphics;

internal class FontResourceType : ResourceType
{
  public override string Name => "font, xml";

  public override string DefaultExtension => ".font.xml";

  public override bool CompressByDefault => true;

  public FontResourceType()
    : base(ResourceTypeIdentifier.Font)
  {
  }

  public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
  {
    FontResourceType fontResourceType = this;
    XmlDocument xmlDocument = new XmlDocument();
    await Task.Run((Action) (() => xmlDocument.Load(e.InputStream)));
    XmlNode node = xmlDocument.SelectSingleNode("font");
    string absolutePath = e.GetAbsolutePath(node.SelectSingleNode("shape").InnerText);
    string[] array = node.SelectNodes("overlay").OfType<XmlNode>().Select<XmlNode, string>((Func<XmlNode, string>) (x => e.GetAbsolutePath(x.InnerText))).ToArray<string>();
    int defaultWidth = int.Parse(node.GetNodeInnerText("width", "0"));
    int height = int.Parse(node.GetNodeInnerText("height", "0"));
    int tracking = int.Parse(node.GetNodeInnerText("tracking", "0"));
    Vector2i? shadow = new Vector2i?();
    XmlNode xmlNode = node.SelectSingleNode("shadow");
    if (xmlNode != null)
      shadow = new Vector2i?(new Vector2i(int.Parse(xmlNode.Attributes["x"].Value), int.Parse(xmlNode.Attributes["y"].Value)));
    // ISSUE: reference to a compiler-generated method
    IEnumerable<Font.CharacterDefinition> characterDefinitions = node.SelectNodes("chardefs/chardef").OfType<XmlNode>().Select<XmlNode, Font.CharacterDefinition>(new Func<XmlNode, Font.CharacterDefinition>(fontResourceType.\u003CLoadAsync\u003Eb__7_2));
    e.PushDependency(absolutePath);
    e.PushDependencies(array);
    return (ILoadedResource) new Font(e.ResourceTree, absolutePath, (IEnumerable<string>) array, defaultWidth, height, tracking, shadow, characterDefinitions)
    {
      Resource = e.Resource
    };
  }

  private Font.CharacterDefinition ParseCharacterDefinition(XmlNode chardefNode)
  {
    int key = (int) chardefNode.Attributes["char"].Value.First<char>();
    XmlNode xmlNode1 = chardefNode.SelectSingleNode("rect");
    Rectanglei rectanglei = new Rectanglei(int.Parse(xmlNode1.Attributes["x"].Value), int.Parse(xmlNode1.Attributes["y"].Value), int.Parse(xmlNode1.Attributes["w"].Value), int.Parse(xmlNode1.Attributes["h"].Value));
    XmlNode xmlNode2 = chardefNode.SelectSingleNode("offset");
    Vector2i vector2i = xmlNode2 == null ? new Vector2i() : new Vector2i(int.Parse(xmlNode2.Attributes["x"].Value), int.Parse(xmlNode2.Attributes["y"].Value));
    string s;
    int num = chardefNode.TryGetNodeInnerText("width", out s) ? int.Parse(s) : rectanglei.Width;
    Rectanglei sourceRectangle = rectanglei;
    Vector2i offset = vector2i;
    int width = num;
    return new Font.CharacterDefinition((char) key, sourceRectangle, offset, width);
  }
}
