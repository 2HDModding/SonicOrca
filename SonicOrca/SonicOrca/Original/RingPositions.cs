// Decompiled with JetBrains decompiler
// Type: SonicOrca.Original.RingPositions
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Geometry;
using System.Collections.Generic;
using System.IO;

namespace SonicOrca.Original
{

    public static class RingPositions
    {
      private const int Horizontal = 0;
      private const int Vertical = 1;

      public static IReadOnlyCollection<Vector2i> FromFile(string filename)
      {
        using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
          return RingPositions.FromStream((Stream) fileStream);
      }

      public static IReadOnlyCollection<Vector2i> FromStream(Stream stream)
      {
        List<Vector2i> vector2iList = new List<Vector2i>();
        byte[] buffer = new byte[4];
        while (stream.Read(buffer, 0, 2) == 2 && (buffer[0] != byte.MaxValue || buffer[1] != byte.MaxValue) && stream.Read(buffer, 2, 2) == 2)
        {
          int x = (int) buffer[0] << 8 | (int) buffer[1];
          int y = ((int) buffer[2] & 15) << 8 | (int) buffer[3];
          int num1 = ((int) buffer[2] >> 4 & 7) + 1;
          int num2 = (int) buffer[2] >> 7;
          for (int index = 0; index < num1; ++index)
          {
            vector2iList.Add(new Vector2i(x, y));
            switch (num2)
            {
              case 0:
                x += 24;
                break;
              case 1:
                y += 24;
                break;
            }
          }
        }
        return (IReadOnlyCollection<Vector2i>) vector2iList.ToArray();
      }
    }
}
