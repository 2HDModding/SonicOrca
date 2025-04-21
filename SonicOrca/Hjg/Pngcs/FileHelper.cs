// Decompiled with JetBrains decompiler
// Type: Hjg.Pngcs.FileHelper
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System.IO;

namespace Hjg.Pngcs
{

    public class FileHelper
    {
      public static Stream OpenFileForReading(string file)
      {
        return file != null && File.Exists(file) ? (Stream) new FileStream(file, FileMode.Open) : throw new PngjInputException($"Cannot open file for reading ({file})");
      }

      public static Stream OpenFileForWriting(string file, bool allowOverwrite)
      {
        return !File.Exists(file) || allowOverwrite ? (Stream) new FileStream(file, FileMode.Create) : throw new PngjOutputException($"File already exists ({file}) and overwrite=false");
      }

      public static PngWriter CreatePngWriter(string fileName, ImageInfo imgInfo, bool allowOverwrite)
      {
        return new PngWriter(FileHelper.OpenFileForWriting(fileName, allowOverwrite), imgInfo, fileName);
      }

      public static PngReader CreatePngReader(string fileName)
      {
        return new PngReader(FileHelper.OpenFileForReading(fileName), fileName);
      }
    }
}
