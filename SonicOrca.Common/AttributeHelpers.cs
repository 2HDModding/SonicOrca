// Decompiled with JetBrains decompiler
// Type: SonicOrca.AttributeHelpers
// Assembly: SonicOrca.Common, Version=2.0.1012.10517, Culture=neutral, PublicKeyToken=null
// MVID: DBB68121-E3AF-4423-9C2F-110CAC67FEBB
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

#nullable disable
namespace SonicOrca;

public static class AttributeHelpers
{
  public static string GetDescription(object value)
  {
    return AttributeHelpers.GetAttribute<DescriptionAttribute>(value)?.Description;
  }

  public static T GetAttribute<T>(object value) where T : Attribute
  {
    Type type1 = value as Type;
    if (!(type1 == (Type) null))
      return CustomAttributeExtensions.GetCustomAttribute<T>((MemberInfo) type1);
    Type type2 = value.GetType();
    return type2.IsEnum ? CustomAttributeExtensions.GetCustomAttribute<T>(((IEnumerable<MemberInfo>) type2.GetMember(value.ToString())).First<MemberInfo>()) : throw new ArgumentException("Value must be a Type or Enum value.", nameof (value));
  }
}
