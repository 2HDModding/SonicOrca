// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.ObjectTypeResourceType
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

namespace SonicOrca.Core.Objects
{

    internal class ObjectTypeResourceType : ResourceType
    {
      public override string Name => "object, cs";

      public override string DefaultExtension => ".object.cs";

      public ObjectTypeResourceType()
        : base(ResourceTypeIdentifier.Object)
      {
      }

      public override async Task<ILoadedResource> LoadAsync(ResourceLoadArgs e, CancellationToken ct = default (CancellationToken))
      {
        Type[] source = ScriptImport.Compile(await new StreamReader(e.InputStream).ReadToEndAsync());
        Type type1 = ((IEnumerable<Type>) source).FirstOrDefault<Type>((Func<Type, bool>) (x => typeof (ObjectType).IsAssignableFrom(x)));
        Type type2 = ((IEnumerable<Type>) source).FirstOrDefault<Type>((Func<Type, bool>) (x => typeof (ActiveObject).IsAssignableFrom(x)));
        if (type1 == (Type) null)
          throw new ResourceException("No class inheriting ObjectType found.");
        if (type2 == (Type) null)
          throw new ResourceException("No class inheriting ActiveObject found.");
        ObjectType instance = (ObjectType) Activator.CreateInstance(type1);
        instance.Resource = e.Resource;
        e.PushDependencies(((IEnumerable<string>) instance.Dependencies).Select<string, string>((Func<string, string>) (x => e.GetAbsolutePath(x))));
        return (ILoadedResource) instance;
      }
    }
}
