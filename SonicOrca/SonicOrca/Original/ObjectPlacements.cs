// Decompiled with JetBrains decompiler
// Type: SonicOrca.Original.ObjectPlacements
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core;
using SonicOrca.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SonicOrca.Original
{

    public static class ObjectPlacements
    {
      public static IReadOnlyCollection<ObjectPlacements.OriginalObjectPlacement> FromFile(
        string filename)
      {
        using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
          return ObjectPlacements.FromStream((Stream) fileStream);
      }

      public static IReadOnlyCollection<ObjectPlacements.OriginalObjectPlacement> FromStream(
        Stream stream)
      {
        List<ObjectPlacements.OriginalObjectPlacement> originalObjectPlacementList = new List<ObjectPlacements.OriginalObjectPlacement>();
        byte[] numArray = new byte[6];
        while (stream.Read(numArray, 0, 2) == 2 && (numArray[0] != byte.MaxValue || numArray[1] != byte.MaxValue) && stream.Read(numArray, 2, 4) == 4)
          originalObjectPlacementList.Add(new ObjectPlacements.OriginalObjectPlacement(numArray));
        return (IReadOnlyCollection<ObjectPlacements.OriginalObjectPlacement>) originalObjectPlacementList.ToArray();
      }

      public static IEnumerable<ObjectPlacement> ConvertOriginalObjectPlacement(
        IEnumerable<ObjectPlacements.OriginalObjectPlacement> originalPlacements)
      {
        return originalPlacements.Select<ObjectPlacements.OriginalObjectPlacement, ObjectPlacement>((Func<ObjectPlacements.OriginalObjectPlacement, ObjectPlacement>) (x => ObjectPlacements.ConvertOriginalObjectPlacement(x))).Where<ObjectPlacement>((Func<ObjectPlacement, bool>) (x => x != null));
      }

      public static ObjectPlacement ConvertOriginalObjectPlacement(
        ObjectPlacements.OriginalObjectPlacement originalPlacement)
      {
        string key = (string) null;
        object state = (object) null;
        Vector2i position = originalPlacement.Position * 4;
        switch (originalPlacement.Type)
        {
          case 3:
            key = "SONICORCA/OBJECTS/LAYERSWITCH";
            int[] numArray = new int[4]
            {
              256 /*0x0100*/,
              512 /*0x0200*/,
              1024 /*0x0400*/,
              2048 /*0x0800*/
            };
            state = ((int) originalPlacement.SubType & 4) == 0 ? (object) new
            {
              AllowAirborne = (((int) originalPlacement.SubType & 128 /*0x80*/) == 0),
              Height = numArray[(int) originalPlacement.SubType & 3],
              Left = (((int) originalPlacement.SubType & 16 /*0x10*/) == 0 ? 0 : 1),
              Right = (((int) originalPlacement.SubType & 8) == 0 ? 0 : 1)
            } : (object) new
            {
              AllowAirborne = (((int) originalPlacement.SubType & 128 /*0x80*/) == 0),
              Width = numArray[(int) originalPlacement.SubType & 3],
              Above = (((int) originalPlacement.SubType & 16 /*0x10*/) == 0 ? 0 : 1),
              Below = (((int) originalPlacement.SubType & 8) == 0 ? 0 : 1)
            };
            break;
          case 11:
            key = "SONICORCA/OBJECTS/CPZTRAPDOOR";
            state = (object) new
            {
              TimeOffset = (((int) originalPlacement.SubType & 15) * 16 /*0x10*/)
            };
            break;
          case 13:
            key = "SONICORCA/OBJECTS/SIGNPOST";
            break;
          case 20:
            key = "SONICORCA/OBJECTS/HTZSEESAW";
            break;
          case 22:
            key = "SONICORCA/OBJECTS/HTZVINELIFT";
            state = (object) new
            {
              Duration = ((int) originalPlacement.SubType * 8),
              Direction = (originalPlacement.FlipX ? "left" : "right")
            };
            position.Y += 152;
            break;
          case 24:
            key = "SONICORCA/OBJECTS/EHZPLATFORM";
            if (((int) originalPlacement.SubType >> 4 & 1) != 0)
              key = "SONICORCA/OBJECTS/EHZBLOCK";
            switch ((int) originalPlacement.SubType & 3)
            {
              case 1:
                state = (object) new
                {
                  RadiusX = 256 /*0x0100*/,
                  TimePeriod = 256 /*0x0100*/
                };
                break;
              case 2:
                state = (object) new
                {
                  RadiusY = 256 /*0x0100*/,
                  TimePeriod = 256 /*0x0100*/
                };
                break;
              case 3:
                state = (object) new{ Falling = true };
                break;
            }
            break;
          case 25:
            key = "SONICORCA/OBJECTS/CPZPLATFORM";
            bool flag = ((int) originalPlacement.SubType >> 4 & 1) == 0;
            switch ((int) originalPlacement.SubType & 7)
            {
              case 0:
                state = (object) new
                {
                  Size = (flag ? "large" : "small"),
                  RadiusX = 256 /*0x0100*/,
                  TimePeriod = 256 /*0x0100*/
                };
                break;
              case 1:
                position += new Vector2i(-192, 0);
                state = (object) new
                {
                  Size = (flag ? "large" : "small"),
                  RadiusX = 192 /*0xC0*/,
                  TimePeriod = 440,
                  TimeOffset = 110
                };
                break;
              case 2:
                position += new Vector2i(0, -256);
                state = (object) new
                {
                  Size = (flag ? "large" : "small"),
                  RadiusY = 256 /*0x0100*/,
                  TimePeriod = 360,
                  TimeOffset = 90
                };
                break;
              case 6:
                position += new Vector2i(0, -384);
                state = (object) new
                {
                  Size = (flag ? "large" : "small"),
                  RadiusY = 384,
                  TimePeriod = 312,
                  TimeOffset = 78
                };
                break;
              case 7:
                position += new Vector2i(0, -392);
                state = (object) new
                {
                  Size = (flag ? "large" : "small"),
                  RadiusY = 392,
                  TimePeriod = 316,
                  TimeOffset = 79
                };
                break;
            }
            break;
          case 27:
            key = "SONICORCA/OBJECTS/CPZSPEEDBOOSTER";
            state = (object) new
            {
              Strength = (new int[2]{ 64 /*0x40*/, 40 }[((int) originalPlacement.SubType & 2) >> 1] * (originalPlacement.FlipX ? -1 : 1))
            };
            break;
          case 28:
            switch (originalPlacement.SubType)
            {
              case 2:
                key = "SONICORCA/OBJECTS/EHZBRIDGE/STAKE";
                break;
              case 4:
                key = "SONICORCA/OBJECTS/HTZVINELIFT/PILLAR";
                break;
              case 5:
                key = "SONICORCA/OBJECTS/HTZVINELIFT/PILLAR";
                state = (object) new{ Right = true };
                break;
            }
            break;
          case 29:
            key = "SONICORCA/OBJECTS/CPZBLOBS";
            state = (object) new
            {
              Count = ((int) originalPlacement.SubType & 15),
              Type = (((int) originalPlacement.SubType & 240 /*0xF0*/) != 0 ? "straight" : "arc")
            };
            break;
          case 30:
            key = "SONICORCA/OBJECTS/CPZTUBEENTRANCE";
            state = (object) new
            {
              Subtype = originalPlacement.SubType
            };
            break;
          case 38:
            key = "SONICORCA/OBJECTS/MONITOR";
            state = (object) new
            {
              Contents = new string[10]
              {
                "none",
                "life",
                "life",
                "robotnik",
                "ring",
                "speedshoes",
                "barrier",
                "invincibility",
                "swapplaces",
                "random"
              }[(int) originalPlacement.SubType]
            };
            break;
          case 45:
            key = "SONICORCA/OBJECTS/HTZDOOR";
            break;
          case 50:
            key = "SONICORCA/OBJECTS/HTZROCK";
            break;
          case 54:
            key = "SONICORCA/OBJECTS/SPIKES";
            break;
          case 62:
            key = "SONICORCA/OBJECTS/CAPSULE";
            break;
          case 64 /*0x40*/:
            key = "SONICORCA/OBJECTS/SPRINGBOARD";
            if (originalPlacement.FlipX)
            {
              state = (object) new{ flipX = true };
              break;
            }
            break;
          case 65:
            key = "SONICORCA/OBJECTS/SPRING";
            int num = 0;
            switch ((int) originalPlacement.SubType >> 4 & 7)
            {
              case 1:
                num = originalPlacement.FlipX ? 6 : 2;
                break;
              case 2:
                num = 4;
                break;
              case 3:
                num = originalPlacement.FlipX ? 7 : 1;
                break;
              case 4:
                num = originalPlacement.FlipX ? 5 : 3;
                break;
            }
            state = (object) new
            {
              Strength = (((int) originalPlacement.SubType & 2) != 0 ? 40 : 64 /*0x40*/),
              Direction = num
            };
            break;
          case 107:
            key = "SONICORCA/OBJECTS/CPZBLOCK";
            state = (object) new
            {
              Type = originalPlacement.SubType
            };
            break;
          case 120:
            key = "SONICORCA/OBJECTS/CPZBLOCK";
            state = (object) new
            {
              Type = ("stairs " + (object) originalPlacement.SubType)
            };
            break;
          case 121:
            key = "SONICORCA/OBJECTS/STARPOST";
            state = (object) new
            {
              Index = originalPlacement.SubType
            };
            position.Y -= 32 /*0x20*/;
            break;
          case 123:
            key = "SONICORCA/OBJECTS/CPZTUBESPRING";
            state = (object) new
            {
              Type = originalPlacement.SubType
            };
            break;
          case 132:
            key = "SONICORCA/OBJECTS/FORCESPINBALL";
            break;
          case 146:
            key = "SONICORCA/OBJECTS/SPIKER";
            if (originalPlacement.FlipY)
              position.Y += 64 /*0x40*/;
            else
              position.Y -= 64 /*0x40*/;
            state = (object) new
            {
              UpsideDown = originalPlacement.FlipY
            };
            break;
          case 148:
          case 150:
            key = "SONICORCA/OBJECTS/REXON";
            break;
          case 149:
            key = "SONICORCA/OBJECTS/SOL";
            break;
          case 165:
            key = "SONICORCA/OBJECTS/SPINY";
            break;
          case 166:
            key = "SONICORCA/OBJECTS/SPINY";
            state = (object) new{ Wall = true };
            break;
          case 167:
            key = "SONICORCA/OBJECTS/GRABBER";
            break;
        }
        if (key == null)
          return (ObjectPlacement) null;
        return state == null ? new ObjectPlacement(key, -1, position) : new ObjectPlacement(key, -1, position, state);
      }

      public class OriginalObjectPlacement
      {
        private readonly Vector2i _position;
        private readonly bool _respawn;
        private readonly bool _flipX;
        private readonly bool _flipY;
        private readonly byte _type;
        private readonly byte _subtype;

        public Vector2i Position => this._position;

        public bool Respawn => this._respawn;

        public bool FlipX => this._flipX;

        public bool FlipY => this._flipY;

        public byte Type => this._type;

        public byte SubType => this._subtype;

        public OriginalObjectPlacement(byte[] data)
        {
          this._position = new Vector2i((int) data[0] << 8 | (int) data[1], ((int) data[2] & 15) << 8 | (int) data[3]);
          this._respawn = ((int) data[2] & 128 /*0x80*/) == 0;
          this._flipX = ((uint) data[2] & 32U /*0x20*/) > 0U;
          this._flipY = ((uint) data[2] & 64U /*0x40*/) > 0U;
          this._type = data[4];
          this._subtype = data[5];
        }
      }
    }
}
