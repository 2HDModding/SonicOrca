// Decompiled with JetBrains decompiler
// Type: SonicOrca.Menu.Screen
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Graphics;
using System.Threading;
using System.Threading.Tasks;

namespace SonicOrca.Menu
{

    public abstract class Screen
    {
      public ScreenState State { get; set; }

      public Screen SwitchScreen { get; set; }

      public bool ManagerHasResponsibility { get; set; }

      public Screen()
      {
        this.State = ScreenState.Constructed;
        this.ManagerHasResponsibility = true;
      }

      public virtual void Initialise()
      {
      }

      public virtual Task LoadAsync(ScreenLoadingProgress progress, CancellationToken ct = default (CancellationToken))
      {
        return (Task) Task.FromResult<bool>(false);
      }

      public virtual void Update()
      {
      }

      public virtual void Draw(Renderer renderer)
      {
      }

      public virtual void Unload()
      {
      }

      public virtual void Deinitialise()
      {
      }

      protected void Finish() => this.State = ScreenState.Finished;

      protected void SwitchEfficient(Screen screen)
      {
        this.State = ScreenState.SwitchedEfficiently;
        this.SwitchScreen = screen;
      }

      protected void Switch(Screen screen)
      {
        this.State = ScreenState.Switched;
        this.SwitchScreen = screen;
      }
    }
}
