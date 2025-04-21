// Decompiled with JetBrains decompiler
// Type: SonicOrca.Original.Mappings
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core;
using System.IO;

namespace SonicOrca.Original
{

    public static class Mappings
    {
      public static void Export(
        string tilesFilename,
        string chunksFilename,
        string layoutFilename,
        string outputFilename)
      {
        byte[] chunks = Kosinski.Decompress(File.ReadAllBytes(chunksFilename));
        int[][,] numArray1 = new int[256 /*0x0100*/][,];
        for (int index = 0; index < 256 /*0x0100*/; ++index)
          numArray1[index] = Mappings.GetChunk(chunks, index * 128 /*0x80*/);
        byte[] layout = Kosinski.Decompress(File.ReadAllBytes(layoutFilename));
        byte[,] backgroundLayout = Mappings.GetBackgroundLayout(layout);
        byte[,] foregroundLayout = Mappings.GetForegroundLayout(layout);
        int[,] numArray2 = new int[1024 /*0x0400*/, 128 /*0x80*/];
        int[,] numArray3 = new int[1024 /*0x0400*/, 128 /*0x80*/];
        for (int index1 = 0; index1 < 128 /*0x80*/; ++index1)
        {
          for (int index2 = 0; index2 < 1024 /*0x0400*/; ++index2)
          {
            int[,] numArray4 = numArray1[(int) backgroundLayout[index2 / 8, index1 / 8]];
            numArray2[index2, index1] = numArray4[index2 % 8, index1 % 8];
            int[,] numArray5 = numArray1[(int) foregroundLayout[index2 / 8, index1 / 8]];
            numArray3[index2, index1] = numArray5[index2 % 8, index1 % 8];
          }
        }
        LevelMap map = new LevelMap();
        map.Layers.Add(new LevelLayer(map));
        map.Layers.Add(new LevelLayer(map));
        map.Layers[0].Resize(1024 /*0x0400*/, 128 /*0x80*/);
        for (int index3 = 0; index3 < map.Layers[0].Rows; ++index3)
        {
          for (int index4 = 0; index4 < map.Layers[0].Columns; ++index4)
            map.Layers[0].Tiles[index4, index3] = numArray2[index4, index3];
        }
        map.Layers[1].Resize(1024 /*0x0400*/, 128 /*0x80*/);
        for (int index5 = 0; index5 < map.Layers[1].Rows; ++index5)
        {
          for (int index6 = 0; index6 < map.Layers[1].Columns; ++index6)
            map.Layers[1].Tiles[index6, index5] = numArray3[index6, index5] + 2048 /*0x0800*/;
        }
        new LevelMapWriter(map).Save(outputFilename);
      }

      private static int[,] GetChunk(byte[] chunks, int index)
      {
        int[,] chunk = new int[8, 8];
        for (int index1 = 0; index1 < 8; ++index1)
        {
          for (int index2 = 0; index2 < 8; ++index2)
          {
            int num = (int) chunks[index + index1 * 16 /*0x10*/ + index2 * 2] << 8 | (int) chunks[index + index1 * 16 /*0x10*/ + index2 * 2 + 1];
            chunk[index2, index1] = num & 1023 /*0x03FF*/;
            if ((num & 1024 /*0x0400*/) != 0)
              chunk[index2, index1] |= 16384 /*0x4000*/;
            if ((num & 2048 /*0x0800*/) != 0)
              chunk[index2, index1] |= 32768 /*0x8000*/;
          }
        }
        return chunk;
      }

      private static byte[,] GetBackgroundLayout(byte[] layout)
      {
        byte[,] backgroundLayout = new byte[128 /*0x80*/, 16 /*0x10*/];
        for (int index1 = 0; index1 < 16 /*0x10*/; ++index1)
        {
          for (int index2 = 0; index2 < 128 /*0x80*/; ++index2)
            backgroundLayout[index2, index1] = layout[256 /*0x0100*/ * index1 + index2 + 128 /*0x80*/];
        }
        return backgroundLayout;
      }

      private static byte[,] GetForegroundLayout(byte[] layout)
      {
        byte[,] foregroundLayout = new byte[128 /*0x80*/, 16 /*0x10*/];
        for (int index1 = 0; index1 < 16 /*0x10*/; ++index1)
        {
          for (int index2 = 0; index2 < 128 /*0x80*/; ++index2)
            foregroundLayout[index2, index1] = layout[256 /*0x0100*/ * index1 + index2];
        }
        return foregroundLayout;
      }
    }
}
