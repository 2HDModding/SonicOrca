// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.VertexAttributeTypeAttribute
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Graphics
{

    public class VertexAttributeTypeAttribute : Attribute
    {
      private readonly VertexAttributePointerType _type;
      private readonly int _size;

      public VertexAttributePointerType Type => this._type;

      public int Size => this._size;

      public VertexAttributeTypeAttribute(VertexAttributePointerType type, int size)
      {
        this._type = type;
        this._size = size;
      }
    }
}
