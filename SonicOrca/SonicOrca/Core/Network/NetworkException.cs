// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.NetworkException
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Runtime.Serialization;

namespace SonicOrca.Core.Network
{

    [Serializable]
    internal class NetworkException : Exception
    {
      public NetworkException()
      {
      }

      public NetworkException(string message)
        : base(message)
      {
      }

      public NetworkException(string message, Exception inner)
        : base(message, inner)
      {
      }

      protected NetworkException(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
      }
    }
}
