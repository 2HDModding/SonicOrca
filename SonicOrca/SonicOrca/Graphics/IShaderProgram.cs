// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.IShaderProgram
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;

namespace SonicOrca.Graphics
{

    public interface IShaderProgram : IDisposable
    {
      void Activate();

      int GetAttributeLocation(string name);

      void SetUniform(string name, int value);

      void SetUniform(string name, float value);

      void SetUniform(string name, double value);

      void SetUniform(string name, Vector2 value);

      void SetUniform(string name, Vector3 value);

      void SetUniform(string name, Vector4 value);

      void SetUniform(string name, Matrix4 value);

      void SetUniform(string name, Colour value);
    }
}
