// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Metadata.ClassificationAttribute
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Core.Objects.Metadata
{

    [AttributeUsage(AttributeTargets.Class)]
    public class ClassificationAttribute : Attribute
    {
      private readonly ObjectClassification _classification;

      public ObjectClassification Classification => this._classification;

      public ClassificationAttribute(ObjectClassification classification)
      {
        this._classification = classification;
      }

      public static ClassificationAttribute FromObject(object obj)
      {
        return AttributeHelpers.GetAttribute<ClassificationAttribute>((object) obj.GetType());
      }
    }
}
