// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.FontResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

// Decompiled and fixed for clarity and maintainability
using SonicOrca.Extensions;
using SonicOrca.Geometry;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SonicOrca.Graphics
{
    internal class FontResourceType : ResourceType
    {
        public override string Name => "font, xml";
        public override string DefaultExtension => ".font.xml";
        public override bool CompressByDefault => true;

        public FontResourceType() : base(ResourceTypeIdentifier.Font) { }

        public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default)
        {
            var xmlDocument = new XmlDocument();
            await Task.Run(() => xmlDocument.Load(e.InputStream), ct);

            XmlNode root = xmlDocument.SelectSingleNode("font");
            if (root == null)
                throw new XmlException("Missing <font> root node.");

            string shapePath = e.GetAbsolutePath(root.SelectSingleNode("shape")?.InnerText);

            var overlays = root.SelectNodes("overlay")
                               .OfType<XmlNode>()
                               .Select(x => e.GetAbsolutePath(x.InnerText))
                               .ToArray();

            int defaultWidth = int.Parse(root.GetNodeInnerText("width", "0"));
            int height = int.Parse(root.GetNodeInnerText("height", "0"));
            int tracking = int.Parse(root.GetNodeInnerText("tracking", "0"));

            Vector2i? shadow = null;
            var shadowNode = root.SelectSingleNode("shadow");
            if (shadowNode != null)
            {
                shadow = new Vector2i(
                    int.Parse(shadowNode.Attributes["x"].Value),
                    int.Parse(shadowNode.Attributes["y"].Value)
                );
            }

            var characterDefinitions = root.SelectNodes("chardefs/chardef")
                                           .OfType<XmlNode>()
                                           .Select(ParseCharacterDefinition)
                                           .ToList();

            e.PushDependency(shapePath);
            e.PushDependencies(overlays);

            return new Font(e.ResourceTree, shapePath, overlays, defaultWidth, height, tracking, shadow, characterDefinitions)
            {
                Resource = e.Resource
            };
        }

        private static Font.CharacterDefinition ParseCharacterDefinition(XmlNode chardefNode)
        {
            char character = chardefNode.Attributes["char"].Value.First();

            var rectNode = chardefNode.SelectSingleNode("rect");
            var rect = new Rectanglei(
                int.Parse(rectNode.Attributes["x"].Value),
                int.Parse(rectNode.Attributes["y"].Value),
                int.Parse(rectNode.Attributes["w"].Value),
                int.Parse(rectNode.Attributes["h"].Value));

            var offsetNode = chardefNode.SelectSingleNode("offset");
            var offset = offsetNode != null
                ? new Vector2i(
                    int.Parse(offsetNode.Attributes["x"].Value),
                    int.Parse(offsetNode.Attributes["y"].Value))
                : new Vector2i();

            int width = chardefNode.TryGetNodeInnerText("width", out string widthStr)
                ? int.Parse(widthStr)
                : rect.Width;

            return new Font.CharacterDefinition(character, rect, offset, width);
        }
    }
}
