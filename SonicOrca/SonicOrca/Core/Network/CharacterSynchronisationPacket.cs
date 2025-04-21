// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Network.CharacterSynchronisationPacket
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Objects;
using SonicOrca.Core.Objects.Base;
using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SonicOrca.Core.Network
{

    internal class CharacterSynchronisationPacket : Packet
    {
      private readonly IReadOnlyList<CharacterSynchronisationPacket.CharacterStateInfo> _characterStates;

      public IReadOnlyList<CharacterSynchronisationPacket.CharacterStateInfo> CharacterStates
      {
        get => this._characterStates;
      }

      public CharacterSynchronisationPacket(IEnumerable<ICharacter> characters)
        : base(PacketType.CharacterSynchronisation)
      {
        this._characterStates = (IReadOnlyList<CharacterSynchronisationPacket.CharacterStateInfo>) characters.Select<ICharacter, CharacterSynchronisationPacket.CharacterStateInfo>((Func<ICharacter, CharacterSynchronisationPacket.CharacterStateInfo>) (x => new CharacterSynchronisationPacket.CharacterStateInfo(x.StateFlags, x.PositionPrecise, x.Velocity))).ToArray<CharacterSynchronisationPacket.CharacterStateInfo>();
      }

      public CharacterSynchronisationPacket(byte[] data)
        : base(PacketType.CharacterSynchronisation)
      {
        using (MemoryStream input = new MemoryStream(data))
        {
          BinaryReader binaryReader = new BinaryReader((Stream) input);
          int length = (int) binaryReader.ReadByte();
          CharacterSynchronisationPacket.CharacterStateInfo[] characterStateInfoArray = new CharacterSynchronisationPacket.CharacterStateInfo[length];
          for (int index = 0; index < length; ++index)
            characterStateInfoArray[index] = new CharacterSynchronisationPacket.CharacterStateInfo((CharacterState) binaryReader.ReadInt32(), new Vector2(binaryReader.ReadDouble(), binaryReader.ReadDouble()), new Vector2(binaryReader.ReadDouble(), binaryReader.ReadDouble()));
          this._characterStates = (IReadOnlyList<CharacterSynchronisationPacket.CharacterStateInfo>) characterStateInfoArray;
        }
      }

      protected override byte[] SerialiseData()
      {
        using (MemoryStream output = new MemoryStream())
        {
          BinaryWriter binaryWriter1 = new BinaryWriter((Stream) output);
          binaryWriter1.Write((byte) ((IReadOnlyCollection<CharacterSynchronisationPacket.CharacterStateInfo>) this._characterStates).Count);
          foreach (CharacterSynchronisationPacket.CharacterStateInfo characterState in (IEnumerable<CharacterSynchronisationPacket.CharacterStateInfo>) this._characterStates)
          {
            binaryWriter1.Write((int) characterState.StateFlags);
            BinaryWriter binaryWriter2 = binaryWriter1;
            Vector2 vector2 = characterState.PositionPrecise;
            double x1 = vector2.X;
            binaryWriter2.Write(x1);
            BinaryWriter binaryWriter3 = binaryWriter1;
            vector2 = characterState.PositionPrecise;
            double y1 = vector2.Y;
            binaryWriter3.Write(y1);
            BinaryWriter binaryWriter4 = binaryWriter1;
            vector2 = characterState.Velocity;
            double x2 = vector2.X;
            binaryWriter4.Write(x2);
            BinaryWriter binaryWriter5 = binaryWriter1;
            vector2 = characterState.Velocity;
            double y2 = vector2.Y;
            binaryWriter5.Write(y2);
          }
          return output.ToArray();
        }
      }

      public void Apply(IReadOnlyList<ICharacter> characters, int roundTripTime)
      {
        int num1 = 0;
        foreach (CharacterSynchronisationPacket.CharacterStateInfo characterState in (IEnumerable<CharacterSynchronisationPacket.CharacterStateInfo>) this._characterStates)
        {
          if (((IReadOnlyCollection<ICharacter>) characters).Count > num1)
          {
            characters[num1].StateFlags = characterState.StateFlags;
            characters[num1].PositionPrecise = characterState.PositionPrecise;
            characters[num1].Velocity = characterState.Velocity;
            double num2 = (double) roundTripTime / (50.0 / 3.0);
            ICharacter character = characters[num1];
            character.PositionPrecise = character.PositionPrecise + characterState.Velocity * (num2 / 2.0);
          }
          ++num1;
        }
      }

      public override string ToString()
      {
        return $"Packet = CharacterSynchronisation Characters = {((IReadOnlyCollection<CharacterSynchronisationPacket.CharacterStateInfo>) this._characterStates).Count}";
      }

      public class CharacterStateInfo
      {
        private readonly CharacterState _state;
        private readonly Vector2 _position;
        private readonly Vector2 _velocity;

        public CharacterState StateFlags => this._state;

        public Vector2 PositionPrecise => this._position;

        public Vector2 Velocity => this._velocity;

        public CharacterStateInfo(CharacterState stateFlags, Vector2 position, Vector2 velocity)
        {
          this._state = stateFlags;
          this._position = position;
          this._velocity = velocity;
        }
      }
    }
}
