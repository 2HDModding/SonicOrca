// Decompiled with JetBrains decompiler
// Type: SonicOrca.Lockable`1
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

#nullable disable
namespace SonicOrca;

public sealed class Lockable<T> where T : class
{
  private readonly T _instance;
  private readonly object _sync;

  public Lockable(T instance)
  {
    this._instance = instance;
    this._sync = new object();
  }

  public T Instance => this._instance;

  public object Sync => this._sync;

  public static implicit operator T(Lockable<T> lockable) => lockable.Instance;
}
