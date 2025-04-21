// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.Packet
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.IO;
using System.Threading.Tasks;

namespace SonicOrca.Core.Network
{

    internal abstract class Packet
    {
      private readonly PacketType _type;

      public PacketType Type => this._type;

      protected Packet(PacketType type) => this._type = type;

      protected abstract byte[] SerialiseData();

      public byte[] Serialise()
      {
        byte[] buffer = this.SerialiseData();
        using (MemoryStream output = new MemoryStream())
        {
          BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
          binaryWriter.Write((byte) this._type);
          binaryWriter.Write(buffer.Length);
          binaryWriter.Write(buffer);
          return output.ToArray();
        }
      }

      public static async Task<Packet> FromStreamAsync(Stream stream)
      {
        Packet specialisedPacket;
        try
        {
          byte[] header = new byte[5];
          int num1 = await stream.ReadAsync(header, 0, header.Length);
          PacketType packetType = (PacketType) header[0];
          int int32 = BitConverter.ToInt32(header, 1);
          byte[] data = new byte[int32];
          if (int32 > 0)
          {
            int num2 = await stream.ReadAsync(data, 0, int32);
          }
          specialisedPacket = Packet.CreateSpecialisedPacket(packetType, data);
        }
        catch (NetworkException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          throw new NetworkException("Error reading packet from network stream.", ex);
        }
        return specialisedPacket;
      }

      public static Packet FromBuffer(byte[] buffer)
      {
        try
        {
          byte[] destinationArray1 = new byte[5];
          Array.Copy((Array) buffer, (Array) destinationArray1, destinationArray1.Length);
          int num = (int) destinationArray1[0];
          int int32 = BitConverter.ToInt32(destinationArray1, 1);
          byte[] destinationArray2 = new byte[int32];
          if (int32 > 0)
            Array.Copy((Array) buffer, destinationArray1.Length, (Array) destinationArray2, 0, int32);
          byte[] data = destinationArray2;
          return Packet.CreateSpecialisedPacket((PacketType) num, data);
        }
        catch (NetworkException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          throw new NetworkException("Error reading packet from buffer.", ex);
        }
      }

      private static Packet CreateSpecialisedPacket(PacketType packetType, byte[] data)
      {
        switch (packetType)
        {
          case PacketType.Ping:
            return (Packet) new PingPacket(data);
          case PacketType.Pong:
            return (Packet) new PongPacket(data);
          case PacketType.ChatMessage:
            return (Packet) new ChatMessagePacket(data);
          case PacketType.PlayInput:
            return (Packet) new PlayInputPacket(data);
          case PacketType.CharacterSynchronisation:
            return (Packet) new CharacterSynchronisationPacket(data);
          case PacketType.LevelSynchronisation:
            return (Packet) new LevelSynchronisationPacket(data);
          default:
            return (Packet) new NotifyPacket(packetType);
        }
      }

      public override string ToString() => $"Packet = {this.Type} Length = {this.Serialise().Length}";
    }
}
