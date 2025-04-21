// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.ObjectEntryTable
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core.Objects
{

    public class ObjectEntryTable : ICollection<ObjectEntry>, IEnumerable<ObjectEntry>, IEnumerable
    {
      private readonly List<ObjectEntry> _entries = new List<ObjectEntry>();
      private readonly Level _level;

      public ObjectEntryTable(Level level) => this._level = level;

      public void Initialise(LevelBinding binding)
      {
        this._entries.Clear();
        foreach (ObjectPlacement objectPlacement in (IEnumerable<ObjectPlacement>) binding.ObjectPlacements)
          this.Add(objectPlacement);
      }

      public void RemoveFinishedEntries()
      {
        this._entries.RemoveAll((Predicate<ObjectEntry>) (x => x.FinishedForever));
      }

      public void Add(ObjectPlacement item) => this.Add(new ObjectEntry(this._level, item));

      public IEnumerable<ObjectEntry> GetAllInRegion(Rectanglei region)
      {
        return this._entries.Where<ObjectEntry>((Func<ObjectEntry, bool>) (x => !x.FinishedForever && region.IntersectsWith(x.LifetimeArea)));
      }

      public IEnumerable<ObjectEntry> GetAll() => (IEnumerable<ObjectEntry>) this._entries;

      public int GetRingCount()
      {
        return this._entries.Count<ObjectEntry>((Func<ObjectEntry, bool>) (x => x.Type.Classification == ObjectClassification.Ring));
      }

      public int GetRingCountInRegion(Rectanglei region)
      {
        return this.GetAllInRegion(region).Count<ObjectEntry>((Func<ObjectEntry, bool>) (x => x.Type.Classification == ObjectClassification.Ring));
      }

      public override string ToString()
      {
        return $"{this._entries.Count} entries, {this._entries.Count<ObjectEntry>((Func<ObjectEntry, bool>) (x => x.Active != null))} loaded.";
      }

      public void Add(ObjectEntry item) => this._entries.Add(item);

      public void Clear() => this._entries.Clear();

      public bool Contains(ObjectEntry item) => this._entries.Contains(item);

      public void CopyTo(ObjectEntry[] array, int arrayIndex)
      {
        this._entries.CopyTo(array, arrayIndex);
      }

      public int Count => this._entries.Count;

      public bool IsReadOnly => false;

      public bool Remove(ObjectEntry item) => this._entries.Remove(item);

      public IEnumerator<ObjectEntry> GetEnumerator()
      {
        return (IEnumerator<ObjectEntry>) this._entries.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._entries.GetEnumerator();
    }
}
