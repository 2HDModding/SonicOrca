// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Metadata.DependencyAttribute
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SonicOrca.Core.Objects.Metadata
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
    public class DependencyAttribute : Attribute
    {
      private readonly string _resourceKey;

      public string ResourceKey => this._resourceKey;

      public DependencyAttribute()
      {
      }

      public DependencyAttribute(string resourceKey) => this._resourceKey = resourceKey;

      public static IEnumerable<string> GetDependencies(object obj)
      {
        Type objType = obj.GetType();
        foreach (DependencyAttribute customAttribute in CustomAttributeExtensions.GetCustomAttributes<DependencyAttribute>((MemberInfo) objType))
          yield return customAttribute.ResourceKey;
        MemberInfo[] memberInfoArray = objType.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        for (int index = 0; index < memberInfoArray.Length; ++index)
        {
          MemberInfo memberInfo = memberInfoArray[index];
          if (CustomAttributeExtensions.GetCustomAttribute<DependencyAttribute>(memberInfo) != null)
          {
            string dependency = (string) null;
            if (memberInfo.MemberType == MemberTypes.Field)
            {
              FieldInfo fieldInfo = (FieldInfo) memberInfo;
              if (fieldInfo.FieldType == typeof (string))
                dependency = (string) fieldInfo.GetValue(obj);
            }
            else if (memberInfo.MemberType == MemberTypes.Property)
            {
              PropertyInfo propertyInfo = (PropertyInfo) memberInfo;
              if (propertyInfo.PropertyType == typeof (string) && propertyInfo.CanRead)
                dependency = (string) propertyInfo.GetValue(obj);
            }
            yield return dependency;
          }
        }
        memberInfoArray = (MemberInfo[]) null;
      }
    }
}
