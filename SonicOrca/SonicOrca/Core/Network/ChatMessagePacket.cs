// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.ChatMessagePacket
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.IO;

namespace SonicOrca.Core.Network
{

    internal class ChatMessagePacket : Packet
    {
      private readonly string _sender;
      private readonly string _message;

      public string Sender => this._sender;

      public string Message => this._message;

      public ChatMessagePacket(string sender, string message)
        : base(PacketType.ChatMessage)
      {
        this._sender = sender;
        this._message = message;
      }

      public ChatMessagePacket(byte[] data)
        : base(PacketType.ChatMessage)
      {
        using (MemoryStream input = new MemoryStream(data))
        {
          BinaryReader binaryReader = new BinaryReader((Stream) input);
          this._sender = binaryReader.ReadString();
          this._message = binaryReader.ReadString();
        }
      }

      protected override byte[] SerialiseData()
      {
        using (MemoryStream output = new MemoryStream())
        {
          BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
          binaryWriter.Write(this._sender);
          binaryWriter.Write(this._message);
          return output.ToArray();
        }
      }

      public override string ToString() => $"{this._sender}: {this._message}";
    }
}
