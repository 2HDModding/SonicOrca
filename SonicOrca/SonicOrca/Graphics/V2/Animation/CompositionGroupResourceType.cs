// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.V2.Animation.CompositionGroupResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Extensions;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SonicOrca.Graphics.V2.Animation;

public class CompositionGroupResourceType : ResourceType
{
  public CompositionGroupResourceType()
    : base(ResourceTypeIdentifier.CompositionGroup)
  {
  }

  public override string Name => "composition, xml";

  public override string DefaultExtension => ".composition.xml";

  public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
  {
    List<string> textureResourceKeys = new List<string>();
    List<CompositionAsset> assets = new List<CompositionAsset>();
    List<CompositionLayer> layers = new List<CompositionLayer>();
    List<Composition> compositions = new List<Composition>();
    XmlDocument xmlDocument = new XmlDocument();
    await Task.Run((Action) (() => xmlDocument.Load(e.InputStream)));
    XmlNode node1 = xmlDocument.SelectSingleNode("root");
    string version = "";
    string s1 = "";
    string s2 = "";
    string s3 = "";
    string s4 = "";
    string s5 = "";
    string name1 = "";
    node1.TryGetNodeInnerText("v", out version);
    node1.TryGetNodeInnerText("fr", out s1);
    node1.TryGetNodeInnerText("ip", out s2);
    node1.TryGetNodeInnerText("op", out s3);
    node1.TryGetNodeInnerText("w", out s4);
    node1.TryGetNodeInnerText("h", out s5);
    node1.TryGetNodeInnerText("nm", out name1);
    foreach (XmlNode node2 in node1.SelectNodes("assets").OfType<XmlNode>())
    {
      string id = "";
      string s6 = "";
      string s7 = "";
      string path = "";
      string fileName = "";
      node2.TryGetNodeInnerText("id", out id);
      node2.TryGetNodeInnerText("w", out s6);
      node2.TryGetNodeInnerText("h", out s7);
      node2.TryGetNodeInnerText("u", out path);
      node2.TryGetNodeInnerText("p", out fileName);
      assets.Add(new CompositionAsset(id, int.Parse(s6), int.Parse(s7), path, fileName));
    }
    foreach (XmlNode node3 in node1.SelectNodes("layers").OfType<XmlNode>())
    {
      string s8 = "";
      string s9 = "";
      string s10 = "";
      string str1 = "";
      string str2 = "";
      string str3 = "";
      string str4 = "";
      string s11 = "";
      string s12 = "";
      string s13 = "";
      node3.TryGetNodeInnerText("ind", out s8);
      node3.TryGetNodeInnerText("ty", out s9);
      node3.TryGetNodeInnerText("td", out s10);
      node3.TryGetNodeInnerText("nm", out str1);
      node3.TryGetNodeInnerText("bm", out s13);
      node3.TryGetNodeInnerText("cl", out str2);
      node3.TryGetNodeInnerText("refId", out str3);
      node3.TryGetNodeInnerText("sr", out str4);
      node3.TryGetNodeInnerText("ip", out s11);
      node3.TryGetNodeInnerText("op", out s12);
      uint index = uint.Parse(s8);
      uint layerType = uint.Parse(s9);
      uint result1 = 0;
      uint.TryParse(s10, out result1);
      uint result2 = 0;
      uint.TryParse(s13, out result2);
      string name2 = str1;
      string fileExtension = str2;
      string textureReference = str3;
      uint num1 = uint.Parse(s11);
      uint num2 = uint.Parse(s12);
      XmlNode xmlNode = node3.SelectSingleNode("ks");
      XmlNode transformNode1 = xmlNode.SelectSingleNode("o");
      XmlNode transformNode2 = xmlNode.SelectSingleNode("r");
      XmlNode transformNode3 = xmlNode.SelectSingleNode("p");
      XmlNode transformNode4 = xmlNode.SelectSingleNode("s");
      transformNode2.SelectNodes("k").OfType<XmlNode>();
      transformNode3.SelectNodes("k").OfType<XmlNode>();
      transformNode4.SelectNodes("k").OfType<XmlNode>();
      List<CompositionLayerTween> compositionLayerTweenList = new List<CompositionLayerTween>();
      compositionLayerTweenList.AddRange(this.ParseScalarTransform(transformNode1, CompositionLayerTween.Type.OPACITY, num1, num2));
      compositionLayerTweenList.AddRange(this.ParseScalarTransform(transformNode2, CompositionLayerTween.Type.ROTATION, num1, num2));
      compositionLayerTweenList.AddRange(this.ParseVectorTransform(transformNode3, CompositionLayerTween.Type.POSITION, num1, num2));
      compositionLayerTweenList.AddRange(this.ParseVectorTransform(transformNode4, CompositionLayerTween.Type.SCALE, num1, num2));
      CompositionLayerAnimatableTransform transform = new CompositionLayerAnimatableTransform();
      foreach (CompositionLayerTween tween in compositionLayerTweenList)
        transform.AddKeyFrameTween(tween);
      layers.Add(new CompositionLayer(index, layerType, result1, result2, name2, fileExtension, textureReference, num1, num2, transform));
    }
    layers = layers.OrderBy<CompositionLayer, uint>((Func<CompositionLayer, uint>) (l => l.Index)).ToList<CompositionLayer>();
    compositions.Add(new Composition(version, uint.Parse(s1), uint.Parse(s2), uint.Parse(s3), uint.Parse(s4), uint.Parse(s5), name1, (IEnumerable<CompositionLayer>) layers));
    string fullKeyPath = e.Resource.FullKeyPath;
    string str5 = fullKeyPath.Remove(fullKeyPath.LastIndexOf("/"));
    foreach (CompositionAsset compositionAsset in assets)
    {
      string str6 = compositionAsset.FileName.Remove(compositionAsset.FileName.LastIndexOf(".")).Replace('_', '/');
      textureResourceKeys.Add($"{str5}/{compositionAsset.Path.ToUpper()}{str6.ToUpper()}");
    }
    e.PushDependencies((IEnumerable<string>) textureResourceKeys);
    return (ILoadedResource) new CompositionGroup(e.ResourceTree, (IEnumerable<string>) textureResourceKeys, (IEnumerable<CompositionAsset>) assets, (IEnumerable<Composition>) compositions)
    {
      Resource = e.Resource
    };
  }

  private IEnumerable<CompositionLayerTween> ParseScalarTransform(
    XmlNode transformNode,
    CompositionLayerTween.Type tweenType,
    uint layerStartFrame,
    uint layerEndFrame)
  {
    List<CompositionLayerTween> scalarTransform = new List<CompositionLayerTween>();
    if (this.TransformPropertyHasKeyFrames(transformNode))
      scalarTransform.AddRange(this.ParseScalarTweenKeySet(transformNode.SelectNodes("k").OfType<XmlNode>(), tweenType));
    else
      scalarTransform.Add(this.ParseSimpleTweenTransform(transformNode, tweenType, layerStartFrame, layerEndFrame));
    return (IEnumerable<CompositionLayerTween>) scalarTransform;
  }

  private IEnumerable<CompositionLayerTween> ParseVectorTransform(
    XmlNode transformNode,
    CompositionLayerTween.Type tweenType,
    uint layerStartFrame,
    uint layerEndFrame)
  {
    List<CompositionLayerTween> vectorTransform = new List<CompositionLayerTween>();
    if (this.TransformPropertyHasKeyFrames(transformNode))
      vectorTransform.AddRange(this.ParseVectorTweenKeySet(transformNode.SelectNodes("k").OfType<XmlNode>(), tweenType));
    else
      vectorTransform.Add(this.ParseSimpleTweenTransform(transformNode, tweenType, layerStartFrame, layerEndFrame));
    return (IEnumerable<CompositionLayerTween>) vectorTransform;
  }

  private IEnumerable<CompositionLayerTween> ParseScalarTweenKeySet(
    IEnumerable<XmlNode> tweenKeySet,
    CompositionLayerTween.Type tweenType)
  {
    if (tweenType != CompositionLayerTween.Type.OPACITY && tweenType != CompositionLayerTween.Type.ROTATION)
      throw new NotImplementedException();
    System.Type type = tweenType == CompositionLayerTween.Type.OPACITY ? typeof (CompositionLayerOpacityTween) : typeof (CompositionLayerRotationTween);
    List<CompositionLayerTween> scalarTweenKeySet = new List<CompositionLayerTween>();
    XmlNode xmlNode = tweenKeySet.First<XmlNode>();
    XmlNode node1 = tweenKeySet.First<XmlNode>();
    string s1 = "";
    string s2 = "";
    string s3 = "";
    node1.TryGetNodeInnerText("t", out s1);
    node1.TryGetNodeInnerText("s", out s2);
    node1.TryGetNodeInnerText("e", out s3);
    KeyValuePair<double, double> keyValuePair1;
    if (!string.IsNullOrEmpty(s2) && !string.IsNullOrEmpty(s3))
      keyValuePair1 = new KeyValuePair<double, double>(double.Parse(s2, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(s3, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture));
    else if (tweenType == CompositionLayerTween.Type.OPACITY)
    {
      keyValuePair1 = new KeyValuePair<double, double>(100.0, 100.0);
    }
    else
    {
      if (tweenType != CompositionLayerTween.Type.ROTATION)
        throw new NotImplementedException();
      keyValuePair1 = new KeyValuePair<double, double>(0.0, 0.0);
    }
    uint num1 = (uint) Math.Round(double.Parse(s1, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture));
    CompositionLayerTween instance = (CompositionLayerTween) Activator.CreateInstance(type, (object) num1, (object) uint.MaxValue, (object) keyValuePair1);
    foreach (XmlNode node2 in tweenKeySet.Skip<XmlNode>(1))
    {
      xmlNode = node2;
      string s4 = "";
      string s5 = "";
      string s6 = "";
      node2.TryGetNodeInnerText("t", out s4);
      node2.TryGetNodeInnerText("s", out s5);
      node2.TryGetNodeInnerText("e", out s6);
      KeyValuePair<double, double> keyValuePair2;
      KeyValuePair<string, double> keyValuePair3;
      if (!string.IsNullOrEmpty(s5) && !string.IsNullOrEmpty(s6))
      {
        keyValuePair2 = new KeyValuePair<double, double>(double.Parse(s5, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(s6, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture));
      }
      else
      {
        if (tweenType != CompositionLayerTween.Type.OPACITY && tweenType != CompositionLayerTween.Type.ROTATION)
          throw new NotImplementedException();
        ref KeyValuePair<double, double> local = ref keyValuePair2;
        keyValuePair3 = instance.EndValues.First<KeyValuePair<string, double>>();
        double key = keyValuePair3.Value;
        keyValuePair3 = instance.EndValues.First<KeyValuePair<string, double>>();
        double num2 = keyValuePair3.Value;
        local = new KeyValuePair<double, double>(key, num2);
      }
      KeyValuePair<double, double> keyValuePair4;
      ref KeyValuePair<double, double> local1 = ref keyValuePair4;
      keyValuePair3 = instance.StartValues.First<KeyValuePair<string, double>>();
      double key1 = keyValuePair3.Value;
      keyValuePair3 = instance.EndValues.First<KeyValuePair<string, double>>();
      double num3 = keyValuePair3.Value;
      local1 = new KeyValuePair<double, double>(key1, num3);
      uint num4 = (uint) Math.Round(double.Parse(s4, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture));
      instance = (CompositionLayerTween) Activator.CreateInstance(type, (object) instance.StartFrame, (object) num4, (object) keyValuePair4);
      scalarTweenKeySet.Add(instance);
      if (node2 != tweenKeySet.Last<XmlNode>())
        instance = (CompositionLayerTween) Activator.CreateInstance(type, (object) num4, (object) uint.MaxValue, (object) keyValuePair2);
    }
    if (xmlNode == tweenKeySet.First<XmlNode>())
      throw new NotSupportedException();
    return (IEnumerable<CompositionLayerTween>) scalarTweenKeySet;
  }

  private IEnumerable<CompositionLayerTween> ParseVectorTweenKeySet(
    IEnumerable<XmlNode> tweenKeySet,
    CompositionLayerTween.Type tweenType)
  {
    if (tweenType != CompositionLayerTween.Type.POSITION && tweenType != CompositionLayerTween.Type.SCALE)
      throw new NotImplementedException();
    System.Type type = tweenType == CompositionLayerTween.Type.POSITION ? typeof (CompositionLayerPositionTween) : typeof (CompositionLayerScaleTween);
    List<CompositionLayerTween> vectorTweenKeySet = new List<CompositionLayerTween>();
    XmlNode xmlNode = tweenKeySet.First<XmlNode>();
    XmlNode node1 = tweenKeySet.First<XmlNode>();
    string s1 = "";
    List<string> source1 = new List<string>();
    List<string> source2 = new List<string>();
    node1.TryGetNodeInnerText("t", out s1);
    IEnumerable<XmlNode> source3 = node1.SelectNodes("s").OfType<XmlNode>();
    IEnumerable<XmlNode> source4 = node1.SelectNodes("e").OfType<XmlNode>();
    if (source3.Count<XmlNode>() == 3 && source4.Count<XmlNode>() == 3)
    {
      source1.Add(source3.ElementAt<XmlNode>(0).InnerText);
      source1.Add(source3.ElementAt<XmlNode>(1).InnerText);
      source1.Add(source3.ElementAt<XmlNode>(2).InnerText);
      source2.Add(source4.ElementAt<XmlNode>(0).InnerText);
      source2.Add(source4.ElementAt<XmlNode>(1).InnerText);
      source2.Add(source4.ElementAt<XmlNode>(2).InnerText);
    }
    else if (source3.Count<XmlNode>() != 0 && source4.Count<XmlNode>() != 0)
      throw new NotImplementedException();
    List<KeyValuePair<double, double>> keyValuePairList1 = new List<KeyValuePair<double, double>>();
    if (source1.All<string>((Func<string, bool>) (v => !string.IsNullOrEmpty(v))) && source2.All<string>((Func<string, bool>) (v => !string.IsNullOrEmpty(v))) && source1.Count > 0 && source2.Count > 0)
    {
      for (int index = 0; index < 3; ++index)
        keyValuePairList1.Add(new KeyValuePair<double, double>(double.Parse(source1[index], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(source2[index], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture)));
    }
    else if (tweenType == CompositionLayerTween.Type.SCALE)
    {
      for (int index = 0; index < 3; ++index)
        keyValuePairList1.Add(new KeyValuePair<double, double>(100.0, 100.0));
    }
    else
    {
      if (tweenType != CompositionLayerTween.Type.POSITION)
        throw new NotImplementedException();
      for (int index = 0; index < 3; ++index)
        keyValuePairList1.Add(new KeyValuePair<double, double>(0.0, 0.0));
    }
    uint num1 = (uint) Math.Round(double.Parse(s1, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture));
    CompositionLayerTween instance = (CompositionLayerTween) Activator.CreateInstance(type, (object) num1, (object) uint.MaxValue, (object) keyValuePairList1[0], (object) keyValuePairList1[1], (object) keyValuePairList1[2]);
    foreach (XmlNode node2 in tweenKeySet.Skip<XmlNode>(1))
    {
      xmlNode = node2;
      string s2 = "";
      List<string> source5 = new List<string>();
      List<string> source6 = new List<string>();
      node2.TryGetNodeInnerText("t", out s2);
      IEnumerable<XmlNode> source7 = node2.SelectNodes("s").OfType<XmlNode>();
      IEnumerable<XmlNode> source8 = node2.SelectNodes("e").OfType<XmlNode>();
      if (source7.Count<XmlNode>() == 3 && source8.Count<XmlNode>() == 3)
      {
        source5.Add(source7.ElementAt<XmlNode>(0).InnerText);
        source5.Add(source7.ElementAt<XmlNode>(1).InnerText);
        source5.Add(source7.ElementAt<XmlNode>(2).InnerText);
        source6.Add(source8.ElementAt<XmlNode>(0).InnerText);
        source6.Add(source8.ElementAt<XmlNode>(1).InnerText);
        source6.Add(source8.ElementAt<XmlNode>(2).InnerText);
      }
      else if (source7.Count<XmlNode>() != 0 && source8.Count<XmlNode>() != 0)
        throw new NotImplementedException();
      List<KeyValuePair<double, double>> keyValuePairList2 = new List<KeyValuePair<double, double>>();
      KeyValuePair<string, double> keyValuePair1;
      if (source5.All<string>((Func<string, bool>) (v => !string.IsNullOrEmpty(v))) && source6.All<string>((Func<string, bool>) (v => !string.IsNullOrEmpty(v))) && source5.Count > 0 && source6.Count > 0)
      {
        for (int index = 0; index < 3; ++index)
          keyValuePairList2.Add(new KeyValuePair<double, double>(double.Parse(source5[index], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(source6[index], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture)));
      }
      else
      {
        if (tweenType != CompositionLayerTween.Type.POSITION && tweenType != CompositionLayerTween.Type.SCALE)
          throw new NotImplementedException();
        for (int index = 0; index < 3; ++index)
        {
          List<KeyValuePair<double, double>> keyValuePairList3 = keyValuePairList2;
          keyValuePair1 = instance.EndValues.ElementAt<KeyValuePair<string, double>>(index);
          double key = keyValuePair1.Value;
          keyValuePair1 = instance.EndValues.ElementAt<KeyValuePair<string, double>>(index);
          double num2 = keyValuePair1.Value;
          KeyValuePair<double, double> keyValuePair2 = new KeyValuePair<double, double>(key, num2);
          keyValuePairList3.Add(keyValuePair2);
        }
      }
      List<KeyValuePair<double, double>> keyValuePairList4 = new List<KeyValuePair<double, double>>();
      for (int index = 0; index < 3; ++index)
      {
        KeyValuePair<double, double> keyValuePair3;
        ref KeyValuePair<double, double> local = ref keyValuePair3;
        keyValuePair1 = instance.StartValues.ElementAt<KeyValuePair<string, double>>(index);
        double key = keyValuePair1.Value;
        keyValuePair1 = instance.EndValues.ElementAt<KeyValuePair<string, double>>(index);
        double num3 = keyValuePair1.Value;
        local = new KeyValuePair<double, double>(key, num3);
        keyValuePairList4.Add(keyValuePair3);
      }
      uint num4 = (uint) Math.Round(double.Parse(s2, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture));
      instance = (CompositionLayerTween) Activator.CreateInstance(type, (object) instance.StartFrame, (object) num4, (object) keyValuePairList4[0], (object) keyValuePairList4[1], (object) keyValuePairList4[2]);
      vectorTweenKeySet.Add(instance);
      if (node2 != tweenKeySet.Last<XmlNode>())
        instance = (CompositionLayerTween) Activator.CreateInstance(type, (object) num4, (object) uint.MaxValue, (object) keyValuePairList2[0], (object) keyValuePairList2[1], (object) keyValuePairList2[2]);
    }
    if (xmlNode == tweenKeySet.First<XmlNode>())
      throw new NotSupportedException();
    return (IEnumerable<CompositionLayerTween>) vectorTweenKeySet;
  }

  private CompositionLayerTween ParseSimpleTweenTransform(
    XmlNode tweenKey,
    CompositionLayerTween.Type tweenType,
    uint layerStartFrame,
    uint layerEndFrame)
  {
    System.Type type = (System.Type) null;
    switch (tweenType)
    {
      case CompositionLayerTween.Type.OPACITY:
        type = typeof (CompositionLayerOpacityTween);
        break;
      case CompositionLayerTween.Type.POSITION:
        type = typeof (CompositionLayerPositionTween);
        break;
      case CompositionLayerTween.Type.ROTATION:
        type = typeof (CompositionLayerRotationTween);
        break;
      case CompositionLayerTween.Type.SCALE:
        type = typeof (CompositionLayerScaleTween);
        break;
    }
    CompositionLayerTween instance;
    if (tweenType == CompositionLayerTween.Type.OPACITY || tweenType == CompositionLayerTween.Type.ROTATION)
    {
      double key = double.Parse(tweenKey.SelectSingleNode("k").InnerText, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture);
      KeyValuePair<double, double> keyValuePair = new KeyValuePair<double, double>(key, key);
      instance = (CompositionLayerTween) Activator.CreateInstance(type, (object) layerStartFrame, (object) layerEndFrame, (object) keyValuePair);
    }
    else
    {
      IEnumerable<XmlNode> source = tweenKey.SelectNodes("k").OfType<XmlNode>();
      string s = source.Count<XmlNode>() == 3 ? source.ElementAt<XmlNode>(0).InnerText : throw new NotSupportedException();
      string innerText = source.ElementAt<XmlNode>(1).InnerText;
      double key1 = double.Parse(s, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture);
      double key2 = double.Parse(innerText, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture);
      KeyValuePair<double, double> keyValuePair1 = new KeyValuePair<double, double>(key1, key1);
      KeyValuePair<double, double> keyValuePair2 = new KeyValuePair<double, double>(key2, key2);
      instance = (CompositionLayerTween) Activator.CreateInstance(type, (object) layerStartFrame, (object) layerEndFrame, (object) keyValuePair1, (object) keyValuePair2);
    }
    instance.HasKeyFrames = false;
    return instance;
  }

  private bool TransformPropertyHasKeyFrames(XmlNode node)
  {
    return node.SelectSingleNode("k").SelectSingleNode("t") != null;
  }
}
