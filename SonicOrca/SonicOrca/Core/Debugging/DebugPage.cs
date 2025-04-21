// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Debugging.DebugPage
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System.Collections.Generic;

namespace SonicOrca.Core.Debugging
{

    internal class DebugPage
    {
      private readonly DebugContext _context;
      private readonly string _name;
      private readonly List<DebugOption> _options = new List<DebugOption>();

      public string Name => this._name;

      public IList<DebugOption> Options => (IList<DebugOption>) this._options;

      public DebugPage(DebugContext context, string name)
      {
        this._context = context;
        this._name = name;
      }

      public DebugPage(DebugContext context, string name, IEnumerable<DebugOption> options)
      {
        this._context = context;
        this._name = name;
        this._options.AddRange(options);
      }

      public void Draw(Renderer renderer)
      {
        string text = string.Empty;
        I2dRenderer obj1 = renderer.Get2dRenderer();
        using (obj1.BeginMatixState())
        {
          foreach (DebugOption option in this._options)
          {
            Matrix4 modelMatrix;
            if (option.Category != text)
            {
              text = option.Category;
              I2dRenderer obj2 = obj1;
              modelMatrix = obj1.ModelMatrix;
              Matrix4 matrix4_1 = modelMatrix.Translate(0.0, 32.0);
              obj2.ModelMatrix = matrix4_1;
              int num = (int) this._context.DrawText(renderer, text, FontAlignment.Left, 0.0, 0.0, 0.75, new int?(0));
              I2dRenderer obj3 = obj1;
              modelMatrix = obj1.ModelMatrix;
              Matrix4 matrix4_2 = modelMatrix.Translate(0.0, (double) (num + 16 /*0x10*/));
              obj3.ModelMatrix = matrix4_2;
            }
            I2dRenderer obj4 = obj1;
            modelMatrix = obj1.ModelMatrix;
            Matrix4 matrix4 = modelMatrix.Translate(0.0, (double) (option.Draw(renderer) + 16 /*0x10*/));
            obj4.ModelMatrix = matrix4;
          }
        }
      }
    }
}
