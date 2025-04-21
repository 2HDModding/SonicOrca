// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.ScriptImport
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SonicOrca.Core
{

    internal static class ScriptImport
    {
      public static Type[] Compile(string sourceCode)
      {
        if (sourceCode.StartsWith("#use "))
        {
          string useNamespace = ScriptImport.ReadToWhitespace(sourceCode.Substring(5));
          return ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).SelectMany<Assembly, Type>((Func<Assembly, IEnumerable<Type>>) (x => (IEnumerable<Type>) x.GetTypes())).Where<Type>((Func<Type, bool>) (x => x.Namespace == useNamespace)).ToArray<Type>();
        }
        IReadOnlyList<string> referenceAssemblyNames = (IReadOnlyList<string>) new string[1]
        {
          "SonicOrca"
        };
        CompilerResults compilerResults = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).Where<Assembly>((Func<Assembly, bool>) (x => ((IEnumerable<string>) referenceAssemblyNames).Contains<string>(x.GetName().Name))).Select<Assembly, string>((Func<Assembly, string>) (x => x.Location)).ToArray<string>())
        {
          GenerateInMemory = true
        }, sourceCode);
        if (compilerResults.Errors.Count > 0)
          throw new Exception("Compile errors:\n" + string.Join(Environment.NewLine, compilerResults.Errors.OfType<CompilerError>().Select<CompilerError, string>((Func<CompilerError, string>) (x => $"  Line {x.Line}, {x.ErrorText}"))));
        return compilerResults.CompiledAssembly.GetTypes();
      }

      private static string ReadToWhitespace(string input)
      {
        for (int index = 0; index < input.Length; ++index)
        {
          if (char.IsWhiteSpace(input[index]))
            return input.Substring(0, index);
        }
        return input;
      }
    }
}
