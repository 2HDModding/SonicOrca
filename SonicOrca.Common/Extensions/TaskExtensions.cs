// Decompiled with JetBrains decompiler
// Type: SonicOrca.Extensions.TaskExtensions
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System.Threading.Tasks;

#nullable disable
namespace SonicOrca.Extensions;

public static class TaskExtensions
{
  public static void RunSync(this Task task) => task.Wait();

  public static T RunSync<T>(this Task<T> task)
  {
    task.Wait();
    return task.Result;
  }
}
