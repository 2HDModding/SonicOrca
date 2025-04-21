// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Base.Enemy
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Collision;
using System;

namespace SonicOrca.Core.Objects.Base
{

    public class Enemy : ActiveObject
    {
      protected override void OnCollision(CollisionEvent e)
      {
        if (e.ActiveObject.Type.Classification != ObjectClassification.Character)
          return;
        ICharacter activeObject = (ICharacter) e.ActiveObject;
        if (!activeObject.CanBeHurt)
          return;
        activeObject.Hurt(Math.Sign(activeObject.Position.X - this.Position.X));
        this.OnHurtCharacter(activeObject);
      }

      protected virtual void OnHurtCharacter(ICharacter character)
      {
      }
    }
}
