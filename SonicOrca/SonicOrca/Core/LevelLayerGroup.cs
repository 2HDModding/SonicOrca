// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.LevelLayerGroup
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Collections.Generic;

namespace SonicOrca.Core
{

    public class LevelLayerGroup : ILevelLayerTreeNode
    {
      private readonly List<ILevelLayerTreeNode> _children = new List<ILevelLayerTreeNode>();

      public string Name { get; set; }

      public bool Editing { get; set; }

      public bool Visible { get; set; }

      public IList<ILevelLayerTreeNode> Children => (IList<ILevelLayerTreeNode>) this._children;

      public LevelLayerGroup(string name)
      {
        this.Name = name;
        this.Visible = true;
      }

      public override string ToString() => $"[{this.Name}]";
    }
}
