// Decompiled with JetBrains decompiler
// Type: SonicOrca.EaseTimeline
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca
{

    public class EaseTimeline
    {
      private readonly List<EaseTimeline.Entry> _entries = new List<EaseTimeline.Entry>();

      public IList<EaseTimeline.Entry> Entries => (IList<EaseTimeline.Entry>) this._entries;

      public EaseTimeline()
      {
      }

      public EaseTimeline(params EaseTimeline.Entry[] entries)
        : this((IEnumerable<EaseTimeline.Entry>) entries)
      {
      }

      public EaseTimeline(IEnumerable<EaseTimeline.Entry> entries) => this._entries.AddRange(entries);

      public int Length
      {
        get
        {
          return this._entries.Count <= 0 ? 0 : this._entries.Max<EaseTimeline.Entry>((Func<EaseTimeline.Entry, int>) (x => x.Time));
        }
      }

      public double GetValueAt(int time)
      {
        EaseTimeline.Entry[] array = this._entries.OrderBy<EaseTimeline.Entry, int>((Func<EaseTimeline.Entry, int>) (x => x.Time)).ToArray<EaseTimeline.Entry>();
        EaseTimeline.Entry entry1 = ((IEnumerable<EaseTimeline.Entry>) array).LastOrDefault<EaseTimeline.Entry>((Func<EaseTimeline.Entry, bool>) (x => x.Time <= time));
        EaseTimeline.Entry entry2 = ((IEnumerable<EaseTimeline.Entry>) array).FirstOrDefault<EaseTimeline.Entry>((Func<EaseTimeline.Entry, bool>) (x => x.Time >= time));
        if (entry1 == null)
          entry1 = ((IEnumerable<EaseTimeline.Entry>) array).First<EaseTimeline.Entry>();
        if (entry2 == null)
          entry2 = ((IEnumerable<EaseTimeline.Entry>) array).Last<EaseTimeline.Entry>();
        if (entry1 == entry2)
          return entry1.Value;
        double num = (entry2.Value - entry1.Value) / (double) (entry2.Time - entry1.Time);
        return entry1.Value + num * (double) (time - entry1.Time);
      }

      public class Entry
      {
        private readonly int _time;
        private readonly double _value;

        public int Time => this._time;

        public double Value => this._value;

        public Entry(int time, double value)
        {
          this._time = time;
          this._value = value;
        }

        public override string ToString() => $"Time = {this._time} Value = {this._value}";
      }
    }
}
