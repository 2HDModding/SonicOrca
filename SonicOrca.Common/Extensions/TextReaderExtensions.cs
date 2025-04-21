// Decompiled with JetBrains decompiler
// Type: SonicOrca.Extensions.TextReaderExtensions
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SonicOrca.Extensions
{

    public static class TextReaderExtensions
    {
      public static bool TryPeek(this TextReader reader, out char c)
      {
        int num = reader.Peek();
        if (num == -1)
        {
          c = char.MinValue;
          return false;
        }
        c = (char) num;
        return true;
      }

      public static bool TryRead(this TextReader reader, out char c)
      {
        int num = reader.Read();
        if (num == -1)
        {
          c = char.MinValue;
          return false;
        }
        c = (char) num;
        return true;
      }

      public static string ReadToWhitespace(this TextReader reader)
      {
        StringBuilder stringBuilder = new StringBuilder();
        char c;
        while (reader.TryPeek(out c) && !char.IsWhiteSpace(c))
        {
          reader.Read();
          stringBuilder.Append(c);
        }
        return stringBuilder.ToString();
      }

      public static void SkipWhitespace(this TextReader reader)
      {
        char c;
        while (reader.TryPeek(out c) && char.IsWhiteSpace(c))
          reader.Read();
      }

      public static string ReadRegex(this TextReader reader, string regex)
      {
        return reader.ReadRegex(new Regex(regex));
      }

      public static string ReadRegex(this TextReader reader, Regex regex)
      {
        StringBuilder stringBuilder = new StringBuilder();
        char c;
        while (reader.TryPeek(out c))
        {
          stringBuilder.Append(c);
          string input = stringBuilder.ToString();
          if (!regex.IsMatch(input))
            return input.Remove(input.Length - 1);
          reader.Read();
        }
        return stringBuilder.ToString();
      }

      public static bool CanRead(this TextReader reader) => reader.Peek() != -1;
    }
}
