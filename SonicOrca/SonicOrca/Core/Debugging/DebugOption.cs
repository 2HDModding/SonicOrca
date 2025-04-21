// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Debugging.DebugOption
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Graphics;

namespace SonicOrca.Core.Debugging
{

    public class DebugOption
    {
      private readonly DebugContext _context;
      private readonly string _page;
      private readonly string _category;
      private readonly bool _selectable;

      public DebugContext Context => this._context;

      public string Page => this._page;

      public string Category => this._category;

      public bool Selectable => this._selectable;

      public DebugOption(DebugContext context, string page, string category, bool selectable = true)
      {
        this._context = context;
        this._page = page;
        this._category = category;
        this._selectable = selectable;
      }

      public virtual void OnPressLeft()
      {
      }

      public virtual void OnPressRight()
      {
      }

      public virtual int Draw(Renderer renderer) => 32 /*0x20*/;
    }
}
