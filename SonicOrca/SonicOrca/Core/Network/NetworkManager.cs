// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.NetworkManager
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonicOrca.Core.Network
{

    public class NetworkManager
    {
      public const int DefaultPort = 7237;
      private Task _joiningServer;

      internal NetworkGameServer Server { get; private set; }

      internal NetworkGameClient Client { get; private set; }

      public bool NetworkPlay { get; private set; }

      public bool Hosting { get; private set; }

      public bool AllConnected { get; private set; }

      public void Host(int port = 7237)
      {
        this.Server = new NetworkGameServer((Level) null, 7237);
        this.NetworkPlay = true;
        this.Hosting = true;
      }

      public void Join(string serverHost, int port = 7237)
      {
        this.Client = new NetworkGameClient((Level) null);
        this._joiningServer = this.Client.InitiateHandshake(serverHost, port);
        this.NetworkPlay = true;
      }

      public void Update()
      {
        if (!this.NetworkPlay)
          return;
        if (this.AllConnected)
        {
          if (this.Hosting)
            this.Server.Update();
          else
            this.Client.Update();
        }
        else if (this.Hosting)
        {
          if (((IReadOnlyCollection<NetworkPlayer>) this.Server.NetworkPlayers).Count <= 0)
            return;
          this.Server.AllowClientsToConnect = false;
          this.AllConnected = true;
        }
        else
        {
          if (!this._joiningServer.IsCompleted)
            return;
          if (this._joiningServer.IsFaulted)
            throw this._joiningServer.Exception.InnerException;
          this.AllConnected = true;
        }
      }
    }
}
