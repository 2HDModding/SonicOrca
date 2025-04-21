// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.AnimationGroupResourceType
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

namespace SonicOrca.Graphics
{

    internal class AnimationGroupResourceType : ResourceType
    {
      public AnimationGroupResourceType()
        : base(ResourceTypeIdentifier.AnimationGroup)
      {
      }

      public override string Name => "animationgroup, xml";

      public override string DefaultExtension => ".anigroup.xml";

      public override bool CompressByDefault => true;

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        AnimationGroupResourceType groupResourceType = this;
        XmlDocument xmlDocument = new XmlDocument();
        await Task.Run((Action) (() => xmlDocument.Load(e.InputStream)));
        XmlNode xmlNode = xmlDocument.SelectSingleNode("anigroup");
        IEnumerable<string> strings = xmlNode.SelectNodes("textures/texture").OfType<XmlNode>().Select<XmlNode, string>((Func<XmlNode, string>) (x => e.GetAbsolutePath(x.InnerText)));
        // ISSUE: reference to a compiler-generated method
        IEnumerable<Animation> animations = xmlNode.SelectNodes("animations/animation").OfType<XmlNode>().Select<XmlNode, Animation>(new Func<XmlNode, Animation>(groupResourceType.\u003CLoadAsync\u003Eb__7_2));
        e.PushDependencies(strings);
        return (ILoadedResource) new AnimationGroup(e.ResourceTree, strings, animations)
        {
          Resource = e.Resource
        };
      }

      private Animation GetAnimationFromXmlNode(XmlNode node)
      {
        int? nextFrameIndex = new int?();
        int? loopFrameIndex = new int?();
        int defaultTexture = 0;
        int defaultWidth = 0;
        int defaultHeight = 0;
        int defaultOffsetX = 0;
        int defaultOffsetY = 0;
        int defaultDelay = 0;
        string s;
        if (node.TryGetAttributeValue("texture", out s))
          defaultTexture = int.Parse(s);
        if (node.TryGetAttributeValue("w", out s))
          defaultWidth = int.Parse(s);
        if (node.TryGetAttributeValue("h", out s))
          defaultHeight = int.Parse(s);
        if (node.TryGetAttributeValue("offset_x", out s))
          defaultOffsetX = int.Parse(s);
        if (node.TryGetAttributeValue("offset_y", out s))
          defaultOffsetY = int.Parse(s);
        if (node.TryGetAttributeValue("delay", out s))
          defaultDelay = int.Parse(s);
        if (node.TryGetAttributeValue("next", out s))
          nextFrameIndex = new int?(int.Parse(s));
        if (node.TryGetAttributeValue("loop", out s))
          loopFrameIndex = new int?(int.Parse(s));
        return new Animation(node.SelectNodes("frame").OfType<XmlNode>().Select<XmlNode, Animation.Frame>((Func<XmlNode, Animation.Frame>) (x => this.GetFrameFromXmlNode(x, defaultTexture, defaultWidth, defaultHeight, defaultOffsetX, defaultOffsetY, defaultDelay))), nextFrameIndex, loopFrameIndex);
      }

      private Animation.Frame GetFrameFromXmlNode(
        XmlNode node,
        int defaultTexture,
        int defaultWidth,
        int defaultHeight,
        int defaultOffsetX,
        int defaultOffsetY,
        int defaultDelay)
      {
        Animation.Frame frameFromXmlNode = new Animation.Frame();
        int x1 = int.Parse(node.Attributes["x"].Value);
        int y1 = int.Parse(node.Attributes["y"].Value);
        string s;
        int width = node.TryGetAttributeValue("w", out s) ? int.Parse(s) : defaultWidth;
        int height = node.TryGetAttributeValue("h", out s) ? int.Parse(s) : defaultHeight;
        frameFromXmlNode.TextureIndex = node.TryGetAttributeValue("texture", out s) ? int.Parse(s) : defaultTexture;
        int x2 = node.TryGetAttributeValue("offset_x", out s) ? int.Parse(s) : defaultOffsetX;
        int y2 = node.TryGetAttributeValue("offset_y", out s) ? int.Parse(s) : defaultOffsetY;
        frameFromXmlNode.Delay = node.TryGetAttributeValue("delay", out s) ? int.Parse(s) : defaultDelay;
        frameFromXmlNode.Offset = new Vector2i(x2, y2);
        frameFromXmlNode.Source = new Rectanglei(x1, y1, width, height);
        return frameFromXmlNode;
      }
    }
}
