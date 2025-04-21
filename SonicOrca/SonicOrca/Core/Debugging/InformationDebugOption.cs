// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Debugging.InformationDebugOption
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using SonicOrca.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core.Debugging
{

    internal class InformationDebugOption(DebugContext context, string page, string category) : 
      DebugOption(context, page, category, false)
    {
      protected virtual IEnumerable<IEnumerable<KeyValuePair<string, object>>> GetInformation()
      {
        yield return (IEnumerable<KeyValuePair<string, object>>) new KeyValuePair<string, object>[1]
        {
          new KeyValuePair<string, object>("UNDEFINED", (object) "INFORMATION")
        };
      }

      public override int Draw(Renderer renderer)
      {
        int height = (int) ((double) this.Context.Font.Height * 0.5);
        Rectangle bounds = new Rectangle(0.0, 0.0, 1888.0, (double) height);
        foreach (IEnumerable<KeyValuePair<string, object>> source in this.GetInformation())
        {
          this.Draw(renderer, (Rectanglei) bounds, source.ToArray<KeyValuePair<string, object>>());
          bounds.Y += (double) (height + 16 /*0x10*/);
        }
        return (int) bounds.Y - 16 /*0x10*/;
      }

      private void Draw(
        Renderer renderer,
        Rectanglei bounds,
        KeyValuePair<string, object>[] horizontalInfos)
      {
        if (horizontalInfos.Length == 0)
          return;
        int num1 = bounds.Width / horizontalInfos.Length;
        for (int index = 0; index < horizontalInfos.Length; ++index)
        {
          int num2 = index != horizontalInfos.Length - 1 ? 32 /*0x20*/ : 0;
          int num3 = index != 0 ? 32 /*0x20*/ : 0;
          Rectanglei bounds1 = new Rectanglei(bounds.X + num1 * index + num3, bounds.Y, num1 - num3 - num2, bounds.Height);
          this.Draw(renderer, bounds1, horizontalInfos[index]);
        }
      }

      private void Draw(Renderer renderer, Rectanglei bounds, KeyValuePair<string, object> kvp)
      {
        if (!string.IsNullOrEmpty(kvp.Key))
          this.Context.DrawText(renderer, kvp.Key.ToUpper() + ":", FontAlignment.Left, (double) bounds.Left, (double) bounds.Top, 0.5, new int?(0));
        this.Context.DrawText(renderer, kvp.Value.ToString().ToUpper(), FontAlignment.Right, (double) bounds.Right, (double) bounds.Top, 0.5, new int?(0));
      }
    }
}
