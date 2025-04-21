// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.AnimationGroupResourceType
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
    internal class AnimationGroupResourceType : ResourceType
    {
        public AnimationGroupResourceType() : base(ResourceTypeIdentifier.AnimationGroup) { }

        public override string Name => "animationgroup, xml";
        public override string DefaultExtension => ".anigroup.xml";
        public override bool CompressByDefault => true;

        public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default)
        {
            var xmlDoc = new XmlDocument();
            await Task.Run(() => xmlDoc.Load(e.InputStream), ct);

            XmlNode root = xmlDoc.SelectSingleNode("anigroup");
            if (root == null)
                throw new XmlException("Missing <anigroup> root node.");

            var texturePaths = root.SelectNodes("textures/texture")
                                   .OfType<XmlNode>()
                                   .Select(x => e.GetAbsolutePath(x.InnerText))
                                   .ToList();

            var animations = root.SelectNodes("animations/animation")
                                 .OfType<XmlNode>()
                                 .Select(GetAnimationFromXmlNode)
                                 .ToList();

            e.PushDependencies(texturePaths);

            return new AnimationGroup(e.ResourceTree, texturePaths, animations)
            {
                Resource = e.Resource
            };
        }

        private static Animation GetAnimationFromXmlNode(XmlNode node)
        {
            int? nextFrameIndex = node.TryGetAttributeValue("next", out string s) ? int.Parse(s) : (int?)null;
            int? loopFrameIndex = node.TryGetAttributeValue("loop", out s) ? int.Parse(s) : (int?)null;

            int defaultTexture = node.TryGetAttributeValue("texture", out s) ? int.Parse(s) : 0;
            int defaultWidth = node.TryGetAttributeValue("w", out s) ? int.Parse(s) : 0;
            int defaultHeight = node.TryGetAttributeValue("h", out s) ? int.Parse(s) : 0;
            int defaultOffsetX = node.TryGetAttributeValue("offset_x", out s) ? int.Parse(s) : 0;
            int defaultOffsetY = node.TryGetAttributeValue("offset_y", out s) ? int.Parse(s) : 0;
            int defaultDelay = node.TryGetAttributeValue("delay", out s) ? int.Parse(s) : 0;

            var frames = node.SelectNodes("frame")
                             .OfType<XmlNode>()
                             .Select(frame => GetFrameFromXmlNode(frame, defaultTexture, defaultWidth, defaultHeight, defaultOffsetX, defaultOffsetY, defaultDelay))
                             .ToList();

            return new Animation(frames, nextFrameIndex, loopFrameIndex);
        }

        private static Animation.Frame GetFrameFromXmlNode(
            XmlNode node,
            int defaultTexture,
            int defaultWidth,
            int defaultHeight,
            int defaultOffsetX,
            int defaultOffsetY,
            int defaultDelay)
        {
            int x = int.Parse(node.Attributes["x"].Value);
            int y = int.Parse(node.Attributes["y"].Value);

            int width = node.TryGetAttributeValue("w", out string s) ? int.Parse(s) : defaultWidth;
            int height = node.TryGetAttributeValue("h", out s) ? int.Parse(s) : defaultHeight;
            int texture = node.TryGetAttributeValue("texture", out s) ? int.Parse(s) : defaultTexture;
            int offsetX = node.TryGetAttributeValue("offset_x", out s) ? int.Parse(s) : defaultOffsetX;
            int offsetY = node.TryGetAttributeValue("offset_y", out s) ? int.Parse(s) : defaultOffsetY;
            int delay = node.TryGetAttributeValue("delay", out s) ? int.Parse(s) : defaultDelay;

            return new Animation.Frame
            {
                TextureIndex = texture,
                Offset = new Vector2i(offsetX, offsetY),
                Source = new Rectanglei(x, y, width, height),
                Delay = delay
            };
        }
    }
}
