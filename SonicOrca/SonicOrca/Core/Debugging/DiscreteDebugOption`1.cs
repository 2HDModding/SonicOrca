// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Debugging.DiscreteDebugOption`1
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core.Debugging
{

    internal class DiscreteDebugOption<T> : DebugOption
    {
      private readonly string _name;
      private readonly KeyValuePair<string, T>[] _values;
      private int _selectedIndex;

      public T SelectedValue
      {
        get => this._values[this._selectedIndex].Value;
        set
        {
          for (int index = 0; index < this._values.Length; ++index)
          {
            if (this._values[index].Value.Equals((object) value))
            {
              this._selectedIndex = index;
              break;
            }
          }
        }
      }

      public DiscreteDebugOption(
        DebugContext context,
        string page,
        string category,
        string name,
        IEnumerable<KeyValuePair<string, T>> values)
        : base(context, page, category)
      {
        this._name = name;
        this._values = values.ToArray<KeyValuePair<string, T>>();
      }

      public override void OnPressLeft()
      {
        if (this._selectedIndex <= 0)
          return;
        --this._selectedIndex;
        this.OnChange();
        this.Context.PlayFocusSound();
      }

      public override void OnPressRight()
      {
        if (this._selectedIndex >= this._values.Length - 1)
          return;
        ++this._selectedIndex;
        this.OnChange();
        this.Context.PlayFocusSound();
      }

      public virtual void OnChange()
      {
      }

      public override int Draw(Renderer renderer)
      {
        this.Context.DrawText(renderer, this._name, FontAlignment.Left, 0.0, 0.0, 0.5, new int?(this.Context.CurrentOption == this ? 1 : 0));
        int x = 1888;
        int num = this._values.Length - 1;
        foreach (KeyValuePair<string, T> keyValuePair in ((IEnumerable<KeyValuePair<string, T>>) this._values).Reverse<KeyValuePair<string, T>>())
        {
          bool flag = num == this._selectedIndex;
          this.Context.DrawText(renderer, keyValuePair.Key, FontAlignment.Right, (double) x, 0.0, 0.5, new int?(flag ? 1 : 0));
          x += -(int) (this.Context.Font.MeasureString(keyValuePair.Key).Width * 0.5) - 16 /*0x10*/;
          --num;
        }
        return (int) ((double) this.Context.Font.Height * 0.5);
      }
    }
}
