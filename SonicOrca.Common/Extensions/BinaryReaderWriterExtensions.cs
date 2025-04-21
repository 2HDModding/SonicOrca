// Decompiled with JetBrains decompiler
// Type: SonicOrca.Extensions.BinaryReaderWriterExtensions
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System.IO;
using System.Text;

namespace SonicOrca.Extensions
{

    public static class BinaryReaderWriterExtensions
    {
      public static string ReadNullTerminatedString(this BinaryReader br)
      {
        StringBuilder stringBuilder = new StringBuilder();
        byte num;
        while ((num = br.ReadByte()) != (byte) 0)
          stringBuilder.Append((char) num);
        return stringBuilder.ToString();
      }

      public static void WriteNullTerminatedString(this BinaryWriter bw, string s)
      {
        if (!string.IsNullOrEmpty(s))
          bw.Write(Encoding.ASCII.GetBytes(s));
        bw.Write((byte) 0);
      }
    }
}
