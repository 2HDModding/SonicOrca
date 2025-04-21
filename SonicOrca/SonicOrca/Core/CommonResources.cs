// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.CommonResources
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SonicOrca.Core
{

    public class CommonResources : IDisposable
    {
      private const string EntriesResourcePath = "SONICORCA/LEVELS/COMMONRESOURCES";
      private readonly Dictionary<string, IEnumerable<CommonResources.CommonResourceEntry>> _entries = new Dictionary<string, IEnumerable<CommonResources.CommonResourceEntry>>();
      private readonly SonicOrcaGameContext _gameContext;
      private ResourceSession _resourceSession;
      private string _loadedScheme;

      public string LoadedScheme => this._loadedScheme;

      public CommonResources(SonicOrcaGameContext gameContext) => this._gameContext = gameContext;

      public void Dispose()
      {
        if (this._resourceSession == null)
          return;
        this._resourceSession.Dispose();
      }

      public async Task LoadEntriesAsync(CancellationToken ct = default (CancellationToken))
      {
        using (ResourceSession resourceSession = new ResourceSession(this._gameContext.ResourceTree))
        {
          resourceSession.PushDependency("SONICORCA/LEVELS/COMMONRESOURCES");
          await resourceSession.LoadAsync(ct);
          foreach (IGrouping<string, CommonResources.CommonResourceEntry> source in this.ParseEntriesFromXmlNode(this._gameContext.ResourceTree.GetLoadedResource<XmlLoadedResource>("SONICORCA/LEVELS/COMMONRESOURCES").XmlDocument.SelectSingleNode("resources")).GroupBy<CommonResources.CommonResourceEntry, string>((Func<CommonResources.CommonResourceEntry, string>) (x => x.Scheme)))
            this._entries[source.Key] = (IEnumerable<CommonResources.CommonResourceEntry>) source.ToArray<CommonResources.CommonResourceEntry>();
        }
      }

      private IEnumerable<CommonResources.CommonResourceEntry> ParseEntriesFromXmlNode(
        XmlNode resourcesNode)
      {
        foreach (XmlNode selectNode in resourcesNode.SelectNodes("resource"))
        {
          string empty = string.Empty;
          XmlAttribute attribute = selectNode.Attributes["scheme"];
          if (attribute != null)
            empty = attribute.Value;
          string key = selectNode.Attributes["key"].Value;
          string path = selectNode.Attributes["path"].Value;
          yield return new CommonResources.CommonResourceEntry(empty, key, path);
        }
      }

      public Task LoadSchemeAsync(LevelScheme scheme, CancellationToken ct = default (CancellationToken))
      {
        return this.LoadSchemeAsync(scheme.ToString().ToLower(), ct);
      }

      public async Task LoadSchemeAsync(string scheme, CancellationToken ct = default (CancellationToken))
      {
        if (this._loadedScheme == scheme)
          return;
        if (this._resourceSession != null)
        {
          Trace.WriteLine("Unloading scheme " + this._loadedScheme);
          this._resourceSession.Unload();
        }
        Trace.WriteLine("Loading scheme " + scheme);
        ResourceSession newResourceSession = new ResourceSession(this._gameContext.ResourceTree);
        newResourceSession.PushDependencies(this.GetResourcePaths(scheme));
        await newResourceSession.LoadAsync(ct);
        this._resourceSession = newResourceSession;
        this._loadedScheme = scheme;
      }

      public void UnloadScheme()
      {
        if (this._resourceSession == null)
          return;
        Trace.WriteLine("Unloading scheme " + this._loadedScheme);
        this._resourceSession.Unload();
        this._resourceSession = (ResourceSession) null;
        this._loadedScheme = (string) null;
      }

      private IEnumerable<string> GetResourcePaths(string scheme)
      {
        IEnumerable<CommonResources.CommonResourceEntry> commonResourceEntries;
        if (this._entries.TryGetValue(string.Empty, out commonResourceEntries))
        {
          foreach (CommonResources.CommonResourceEntry commonResourceEntry in commonResourceEntries)
            yield return commonResourceEntry.Path;
        }
        if (this._entries.TryGetValue(scheme, out commonResourceEntries))
        {
          foreach (CommonResources.CommonResourceEntry commonResourceEntry in commonResourceEntries)
            yield return commonResourceEntry.Path;
        }
      }

      public string GetResourcePath(string key) => this.GetResourcePath(this._loadedScheme, key);

      public string GetResourcePath(string scheme, string key)
      {
        IEnumerable<CommonResources.CommonResourceEntry> source;
        CommonResources.CommonResourceEntry commonResourceEntry1;
        if (this._entries.TryGetValue(scheme, out source) && (commonResourceEntry1 = source.FirstOrDefault<CommonResources.CommonResourceEntry>((Func<CommonResources.CommonResourceEntry, bool>) (x => x.Key == key))) != null)
          return commonResourceEntry1.Path;
        CommonResources.CommonResourceEntry commonResourceEntry2;
        if (this._entries.TryGetValue(string.Empty, out source) && (commonResourceEntry2 = source.FirstOrDefault<CommonResources.CommonResourceEntry>((Func<CommonResources.CommonResourceEntry, bool>) (x => x.Key == key))) != null)
          return commonResourceEntry2.Path;
        throw new ResourceException(key + " not found in the level common resources.");
      }

      private class CommonResourceEntry
      {
        private readonly string _scheme;
        private readonly string _key;
        private readonly string _path;

        public string Scheme => this._scheme;

        public string Key => this._key;

        public string Path => this._path;

        public CommonResourceEntry(string scheme, string key, string path)
        {
          this._scheme = scheme.ToLower();
          this._key = key.ToLower();
          this._path = path.ToUpper();
        }

        public override string ToString() => $"{this._scheme}.{this._key} = {this._path}";
      }
    }
}
