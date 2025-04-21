// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.ManagedShaderProgram
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;

namespace SonicOrca.Graphics
{

    public class ManagedShaderProgram : IDisposable
    {
      private readonly IShader _vertexShader;
      private readonly IShader _fragmentShader;
      private readonly IShaderProgram _shaderProgram;

      public IShader Vertex => this._vertexShader;

      public IShader Fragment => this._fragmentShader;

      public IShaderProgram Program => this._shaderProgram;

      public ManagedShaderProgram(
        IGraphicsContext context,
        string vertexSourceCode,
        string fragmentSourceCode)
      {
        this._vertexShader = context.CreateShader(ShaderType.Vertex, vertexSourceCode);
        this._fragmentShader = context.CreateShader(ShaderType.Fragment, fragmentSourceCode);
        this._shaderProgram = context.CreateShaderProgram(this._vertexShader, this._fragmentShader);
      }

      public void Dispose()
      {
        this._shaderProgram.Dispose();
        this._vertexShader.Dispose();
        this._fragmentShader.Dispose();
      }
    }
}
