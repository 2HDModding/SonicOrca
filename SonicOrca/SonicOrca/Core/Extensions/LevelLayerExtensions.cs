// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Extensions.LevelLayerExtensions
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Collections.Generic;

namespace SonicOrca.Core.Extensions
{

    public static class LevelLayerExtensions
    {
      public static IEnumerable<ILevelLayerTreeNode> GetDescendantsOrdered(this ILevelLayerTreeNode node)
      {
        IList<ILevelLayerTreeNode> children = node.Children;
        if (children != null)
        {
          foreach (ILevelLayerTreeNode child in (IEnumerable<ILevelLayerTreeNode>) children)
          {
            yield return child;
            foreach (ILevelLayerTreeNode levelLayerTreeNode in child.GetDescendantsOrdered())
              yield return levelLayerTreeNode;
          }
        }
      }
    }
}
