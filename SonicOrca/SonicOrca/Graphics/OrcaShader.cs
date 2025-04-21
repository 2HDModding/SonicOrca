// Decompiled with JetBrains decompiler
// Type: SonicOrca.Graphics.OrcaShader
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SonicOrca.Graphics
{

    public static class OrcaShader
    {
      public static ManagedShaderProgram CreateFromFile(IGraphicsContext context, string path)
      {
        string vertexOuput;
        string fragmentOutput;
        OrcaShader.Parse(OrcaShader.ParseShaderFileWithIncludes(path), out vertexOuput, out fragmentOutput);
        return new ManagedShaderProgram(context, vertexOuput, fragmentOutput);
      }

      public static ManagedShaderProgram Create(IGraphicsContext context, string input)
      {
        string vertexOuput;
        string fragmentOutput;
        OrcaShader.Parse(input, out vertexOuput, out fragmentOutput);
        return new ManagedShaderProgram(context, vertexOuput, fragmentOutput);
      }

      private static string ParseShaderFileWithIncludes(string path)
      {
        if (!File.Exists(path))
          path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);
        StringBuilder stringBuilder = new StringBuilder();
        StreamReader streamReader = new StreamReader(path);
        string str1;
        while ((str1 = streamReader.ReadLine()) != null)
        {
          string str2 = str1.TrimStart();
          if (str2.StartsWith("@include"))
          {
            string str3 = str2.Substring(8);
            if (str3.Length > 0 && char.IsWhiteSpace(str3[0]))
            {
              string str4 = str3.Trim();
              string path2 = str4.StartsWith("\"") && str4.EndsWith("\"") ? str4.Substring(1, str4.Length - 2) : throw new InvalidDataException("include syntax error");
              string str5 = Path.Combine(Path.GetDirectoryName(path), path2);
              if (!File.Exists(str5))
                throw new FileNotFoundException(str5 + " not found.", str5);
              stringBuilder.AppendLine(OrcaShader.ParseShaderFileWithIncludes(str5));
              continue;
            }
          }
          stringBuilder.AppendLine(str1);
        }
        return stringBuilder.ToString();
      }

      public static void Parse(string input, out string vertexOuput, out string fragmentOutput)
      {
        IDictionary<string, string> dictionary = (IDictionary<string, string>) ((IEnumerable<KeyValuePair<string, string>>) OrcaShader.GetBlocks(input)).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key), (Func<KeyValuePair<string, string>, string>) (x => x.Value));
        vertexOuput = OrcaShader.ConcatenateBlocks(dictionary, (IEnumerable<string>) new string[5]
        {
          string.Empty,
          "uniform",
          "vertex_input",
          "vertex_output",
          "vertex"
        });
        fragmentOutput = OrcaShader.ConcatenateBlocks(dictionary, (IEnumerable<string>) new string[5]
        {
          string.Empty,
          "uniform",
          "fragment_input",
          "fragment_output",
          "fragment"
        });
      }

      private static string ConcatenateBlocks(
        IDictionary<string, string> blocks,
        IEnumerable<string> blockNames)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string blockName in blockNames)
        {
          string str;
          if (blocks.TryGetValue(blockName, out str))
          {
            if (!string.IsNullOrEmpty(blockName))
              stringBuilder.AppendLine($"//{blockName}:");
            stringBuilder.Append(str);
          }
        }
        return stringBuilder.ToString();
      }

      private static IReadOnlyCollection<KeyValuePair<string, string>> GetBlocks(string input)
      {
        string key = string.Empty;
        StringBuilder stringBuilder = new StringBuilder();
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        StringReader stringReader = new StringReader(input);
        string str1;
        while ((str1 = stringReader.ReadLine()) != null)
        {
          if (str1.TrimStart().StartsWith("@"))
          {
            string str2 = str1.Substring(str1.IndexOf("@") + 1);
            int startIndex = str2.IndexOf(":");
            if (startIndex != -1)
            {
              keyValuePairList.Add(new KeyValuePair<string, string>(key, stringBuilder.ToString()));
              key = str2.Remove(startIndex);
              stringBuilder.Clear();
              continue;
            }
          }
          stringBuilder.AppendLine(str1);
        }
        keyValuePairList.Add(new KeyValuePair<string, string>(key, stringBuilder.ToString()));
        return (IReadOnlyCollection<KeyValuePair<string, string>>) keyValuePairList.ToArray();
      }
    }
}
