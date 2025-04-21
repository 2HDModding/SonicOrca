// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Extensions.ReflectionTypeExtensions
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SonicOrca.Core.Extensions
{

    public static class ReflectionTypeExtensions
    {
      public static Type GetUnderlyingType(this MemberInfo member)
      {
        switch (member.MemberType)
        {
          case MemberTypes.Event:
            return ((EventInfo) member).EventHandlerType;
          case MemberTypes.Field:
            return ((FieldInfo) member).FieldType;
          case MemberTypes.Method:
            return ((MethodInfo) member).ReturnType;
          case MemberTypes.Property:
            return ((PropertyInfo) member).PropertyType;
          default:
            throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
        }
      }

      public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
      {
        foreach (Type type in givenType.GetInterfaces())
        {
          if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
            return true;
        }
        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
          return true;
        Type baseType = givenType.BaseType;
        return !(baseType == (Type) null) && baseType.IsAssignableToGenericType(genericType);
      }

      public static object GetUnderlyingValue(this MemberInfo member, object instance)
      {
        switch (member.MemberType)
        {
          case MemberTypes.Field:
            return ((FieldInfo) member).GetValue(instance);
          case MemberTypes.Property:
            return ((PropertyInfo) member).GetValue(instance);
          default:
            throw new ArgumentException("Input MemberInfo must be type FieldInfo, or PropertyInfo");
        }
      }

      public static void SetUnderlyingValue(this MemberInfo member, object instance, object value)
      {
        switch (member.MemberType)
        {
          case MemberTypes.Field:
            ((FieldInfo) member).SetValue(instance, value);
            break;
          case MemberTypes.Property:
            ((PropertyInfo) member).SetValue(instance, value);
            break;
          default:
            throw new ArgumentException("Input MemberInfo must be type FieldInfo, or PropertyInfo");
        }
      }

      public static IEnumerable<MemberInfo> GetUnderlyingMembers(
        this Type type,
        BindingFlags bindingFlags)
      {
        return type.GetFields(bindingFlags).Cast<MemberInfo>().Concat<MemberInfo>(type.GetProperties(bindingFlags).Cast<MemberInfo>());
      }

      public static bool IsAnonymous(this Type type)
      {
        if (type == (Type) null)
          throw new ArgumentNullException(nameof (type));
        return Attribute.IsDefined((MemberInfo) type, typeof (CompilerGeneratedAttribute), false) && type.Name.Contains("AnonymousType") && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$")) && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
      }
    }
}
