// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.IVertexArray
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Graphics
{

    public interface IVertexArray : IDisposable
    {
      void Bind();

      void DefineAttribute(
        int attributeLocation,
        VertexAttributePointerType type,
        int size,
        int stride,
        int offset);

      void Render(PrimitiveType type, int index, int count);
    }
}
