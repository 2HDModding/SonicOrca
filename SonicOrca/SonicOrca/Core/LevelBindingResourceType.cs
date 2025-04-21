// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelBindingResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Microsoft.CSharp.RuntimeBinder;
using SonicOrca.Extensions;
using SonicOrca.Geometry;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SonicOrca.Core
{

    public class LevelBindingResourceType : ResourceType
    {
      public override string Name => "binding, xml";

      public override string DefaultExtension => ".binding.xml";

      public override bool CompressByDefault => true;

      public LevelBindingResourceType()
        : base(ResourceTypeIdentifier.Binding)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        LevelBindingResourceType bindingResourceType = this;
        ResourceLoadArgs e1 = e;
        XmlDocument xmlDocument = new XmlDocument();
        await Task.Run((Action) (() => xmlDocument.Load(e1.InputStream)));
        LevelBinding levelBinding = new LevelBinding();
        levelBinding.Resource = e1.Resource;
        XmlNode node = xmlDocument.SelectSingleNode("Binding/Definitions");
        string s;
        int defaultLayerIndex;
        if (!node.TryGetAttributeValue("DefaultLayer", out s) || !int.TryParse(s, out defaultLayerIndex))
          defaultLayerIndex = 0;
        levelBinding.ObjectPlacements.AddRange<ObjectPlacement>(node.SelectNodes("Definition").OfType<XmlNode>().Select<XmlNode, ObjectPlacement>((Func<XmlNode, ObjectPlacement>) (x => bindingResourceType.GetObjectPlacementFromXmlNode(x, defaultLayerIndex))));
        e1.PushDependencies(levelBinding.ObjectPlacements.Select<ObjectPlacement, string>((Func<ObjectPlacement, string>) (x => x.Key)).Distinct<string>());
        return (ILoadedResource) levelBinding;
      }

        private ObjectPlacement GetObjectPlacementFromXmlNode(XmlNode node, int defaultLayerIndex)
        {
            var commonNode = node.SelectSingleNode("Common");
            var behaviourNode = node.SelectSingleNode("Behaviour");
            var mappingsNode = node.SelectSingleNode("Mappings");

            var keyNode = commonNode?.SelectSingleNode("Key");
            var uidNode = commonNode?.SelectSingleNode("Uid");
            var nameNode = commonNode?.SelectSingleNode("Name");
            var layerNode = commonNode?.SelectSingleNode("Layer");
            var posNode = commonNode?.SelectSingleNode("Position");

            int layerIndex = defaultLayerIndex;
            if (layerNode != null && commonNode.TryGetNodeInnerText("Layer", out string layerStr))
            {
                int.TryParse(layerStr, out layerIndex);
            }

            Guid uid = Guid.Empty;
            if (uidNode != null && !string.IsNullOrEmpty(uidNode.InnerText))
            {
                Guid.TryParse(uidNode.InnerText, out uid);
            }

            Vector2i position = new Vector2i(0, 0);
            if (posNode?.Attributes != null &&
                int.TryParse(posNode.Attributes["X"]?.Value, out int x) &&
                int.TryParse(posNode.Attributes["Y"]?.Value, out int y))
            {
                position = new Vector2i(x, y);
            }

            object behaviour = new { };
            if (behaviourNode?.HasChildNodes == true)
            {
                var behaviourPairs = this.ParseBehaviour(behaviourNode);
                var expando = new ExpandoObject() as IDictionary<string, object>;
                foreach (var kvp in behaviourPairs)
                {
                    expando[kvp.Key] = kvp.Value;
                }
                behaviour = expando;
            }

            var objectPlacement = new ObjectPlacement(
                keyNode?.InnerText ?? "",
                uid,
                nameNode?.InnerText ?? "",
                layerIndex,
                position,
                behaviour
            );

            if (mappingsNode?.HasChildNodes == true)
            {
                objectPlacement.Mappings = new List<KeyValuePair<string, object>>(this.ParseBehaviour(mappingsNode));
            }

            return objectPlacement;
        }


      private IEnumerable<KeyValuePair<string, object>> ParseBehaviour(XmlNode entryNode)
      {
        foreach (XmlAttribute attribute in (XmlNamedNodeMap) entryNode.Attributes)
          yield return new KeyValuePair<string, object>(attribute.Name, LevelBindingResourceType.ParseBehaviourValue(attribute.Value));
        foreach (XmlNode entryNode1 in entryNode)
        {
          if (entryNode1.NodeType == XmlNodeType.Element)
          {
            string name = entryNode1.Name;
            entryNode1.ChildNodes.OfType<XmlNode>().Where<XmlNode>((Func<XmlNode, bool>) (x => x.NodeType == XmlNodeType.Element)).ToArray<XmlNode>();
            KeyValuePair<string, object>[] array = this.ParseBehaviour(entryNode1).ToArray<KeyValuePair<string, object>>();
            object obj = array.Length == 0 ? LevelBindingResourceType.ParseBehaviourValue(entryNode1.InnerText) : (object) array;
            yield return new KeyValuePair<string, object>(name, obj);
          }
        }
      }

      public static object ParseBehaviourValue(string value)
      {
        int result1;
        if (int.TryParse(value, out result1))
          return (object) result1;
        double result2;
        if (double.TryParse(value, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
          return (object) result2;
        bool result3;
        if (bool.TryParse(value, out result3))
          return (object) result3;
        Guid result4;
        return Guid.TryParse(value, out result4) ? (object) result4 : (object) value;
      }

      public static object ParseBehaviourValue(string value, Type type)
      {
        if (type == typeof (int))
        {
          int result;
          return int.TryParse(value, out result) ? (object) result : (object) 0;
        }
        if (type == typeof (uint))
        {
          uint result;
          return uint.TryParse(value, out result) ? (object) result : (object) 0U;
        }
        if (type == typeof (double))
        {
          double result;
          return double.TryParse(value, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? (object) result : (object) 0.0;
        }
        if (type == typeof (bool))
        {
          bool result;
          return bool.TryParse(value, out result) ? (object) result : (object) false;
        }
        if (type == typeof (Vector2))
        {
          string s1 = value.Trim('{', '}', ' ').Replace(" ", "").Split(',')[0].Split('=')[1];
          string s2 = value.Trim('{', '}', ' ').Replace(" ", "").Split(',')[1].Split('=')[1];
          double result;
          double.TryParse(s1, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result);
          CultureInfo invariantCulture = CultureInfo.InvariantCulture;
          double y = 0.0f;
          ref double local = ref y;
          double.TryParse(s2, NumberStyles.Float, (IFormatProvider) invariantCulture, out local);
          return (object) new Vector2(result, y);
        }
        if (type == typeof (Vector2i))
        {
          string s3 = value.Trim('{', '}', ' ').Replace(" ", "").Split(',')[0].Split('=')[1];
          string s4 = value.Trim('{', '}', ' ').Replace(" ", "").Split(',')[1].Split('=')[1];
          int result;
          int.TryParse(s3, out result);
          int y = 0;
          ref int local = ref y;
          int.TryParse(s4, out local);
          return (object) new Vector2i(result, y);
        }
        Guid result1;
        return type == typeof (Guid) ? (Guid.TryParse(value, out result1) ? (object) result1 : (object) new Guid()) : (type.IsEnum ? Enum.Parse(type, value, true) : (object) value);
      }
    }
}
