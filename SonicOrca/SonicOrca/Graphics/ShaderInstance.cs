// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.ShaderInstance
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;

namespace SonicOrca.Graphics
{

    public class ShaderInstance : IDisposable
    {
      private readonly IGraphicsContext _graphicsContext;
      private readonly ManagedShaderProgram _shaderProgram;
      private readonly VertexBuffer _vertexBuffer;

      public IGraphicsContext GraphicsContext => this._graphicsContext;

      public IShaderProgram ShaderProgram => this._shaderProgram.Program;

      public VertexBuffer VertexBuffer => this._vertexBuffer;

      public ShaderInstance(
        IGraphicsContext graphicsContext,
        string vertex,
        string fragment,
        IEnumerable<int> vertexCounts)
      {
        this._graphicsContext = graphicsContext;
        this._shaderProgram = new ManagedShaderProgram(graphicsContext, vertex, fragment);
        this._vertexBuffer = this._graphicsContext.CreateVertexBuffer(vertexCounts);
      }

      public ShaderInstance(
        IGraphicsContext graphicsContext,
        string orcaGLSL,
        IEnumerable<int> vertexCounts)
      {
        this._graphicsContext = graphicsContext;
        this._shaderProgram = OrcaShader.Create(graphicsContext, orcaGLSL);
        this._vertexBuffer = this._graphicsContext.CreateVertexBuffer(vertexCounts);
      }

      public void Dispose()
      {
        this._vertexBuffer.Dispose();
        this._shaderProgram.Dispose();
      }
    }
}
