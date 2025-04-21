// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.PlayInputPacket
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System.IO;

namespace SonicOrca.Core.Network
{

    internal class PlayInputPacket : Packet
    {
      private readonly int _characterId;
      private readonly Vector2 _direction;
      private readonly bool _action;

      public int CharacterId => this._characterId;

      public Vector2 Direction => this._direction;

      public bool Action => this._action;

      public PlayInputPacket(int characterId, Vector2 direction, bool action)
        : base(PacketType.PlayInput)
      {
        this._characterId = characterId;
        this._direction = direction;
        this._action = action;
      }

      public PlayInputPacket(byte[] data)
        : base(PacketType.PlayInput)
      {
        using (MemoryStream input = new MemoryStream(data))
        {
          BinaryReader binaryReader = new BinaryReader((Stream) input);
          this._characterId = binaryReader.ReadInt32();
          this._direction = new Vector2(binaryReader.ReadDouble(), binaryReader.ReadDouble());
          this._action = binaryReader.ReadBoolean();
        }
      }

      protected override byte[] SerialiseData()
      {
        using (MemoryStream output = new MemoryStream())
        {
          BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
          binaryWriter.Write(this._characterId);
          binaryWriter.Write(this._direction.X);
          binaryWriter.Write(this._direction.Y);
          binaryWriter.Write(this._action);
          return output.ToArray();
        }
      }

      public override string ToString()
      {
        return $"Packet = PlayInput CharacterId = {this._characterId} Direction = {this._direction} Action = {this._action}";
      }
    }
}
