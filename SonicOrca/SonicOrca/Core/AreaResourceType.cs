// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.AreaResourceType
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Core
{

    internal class AreaResourceType : ResourceType
    {
      public override string Name => "area, cs";

      public override string DefaultExtension => ".area.cs";

      public override bool CompressByDefault => true;

      public AreaResourceType()
        : base(ResourceTypeIdentifier.Area)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        Type type = ((IEnumerable<Type>) ScriptImport.Compile(await new StreamReader(e.InputStream).ReadToEndAsync())).FirstOrDefault<Type>((Func<Type, bool>) (x => typeof (Area).IsAssignableFrom(x)));
        Area area = !(type == (Type) null) ? (Area) Activator.CreateInstance(type) : throw new ResourceException("No class inheriting Area found.");
        area.Resource = e.Resource;
        e.PushDependencies(((IEnumerable<string>) area.Dependencies).Select<string, string>((Func<string, string>) (x => e.GetAbsolutePath(x))));
        return (ILoadedResource) area;
      }
    }
}
