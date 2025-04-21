// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.Objects.Metadata.StateVariableAttribute
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SonicOrca.Core.Objects.Metadata
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class StateVariableAttribute : Attribute
    {
      private object _defaultValue;
      private Type _type;
      private string _name = "";

      public string Name
      {
        get
        {
          if (this._type == (Type) null)
            throw new TypeAccessException();
          return this._name;
        }
        private set
        {
          if (this._type == (Type) null)
            throw new TypeAccessException();
          this._name = value;
        }
      }

      public object DefaultValue
      {
        get
        {
          if (this._type == (Type) null)
            throw new TypeAccessException();
          return this._defaultValue;
        }
        private set
        {
          if (this._type == (Type) null)
            throw new TypeAccessException();
          this._defaultValue = value;
        }
      }

      public Type Type
      {
        get => !(this._type == (Type) null) ? this._type : throw new TypeAccessException();
        private set => this._type = value;
      }

      public static IEnumerable<Tuple<MemberInfo, StateVariableAttribute>> GetStateVariables(Type type)
      {
        object instance = Activator.CreateInstance(type);
        IEnumerable<MemberInfo> publicDeclaredMembers = type.GetUnderlyingMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        foreach (MemberInfo member in publicDeclaredMembers)
        {
          if (CustomAttributeExtensions.GetCustomAttribute<HideInEditorAttribute>(member) == null)
            yield return new Tuple<MemberInfo, StateVariableAttribute>(member, new StateVariableAttribute()
            {
              Type = member.GetUnderlyingType(),
              Name = member.Name,
              DefaultValue = member.GetUnderlyingValue(instance)
            });
        }
        foreach (MemberInfo member in type.GetUnderlyingMembers(BindingFlags.Instance | BindingFlags.Public).Except<MemberInfo>(publicDeclaredMembers))
        {
          StateVariableAttribute customAttribute = CustomAttributeExtensions.GetCustomAttribute<StateVariableAttribute>(member);
          if (customAttribute != null)
          {
            customAttribute.Type = member.GetUnderlyingType();
            customAttribute.Name = member.Name;
            customAttribute.DefaultValue = member.GetUnderlyingValue(instance);
            yield return new Tuple<MemberInfo, StateVariableAttribute>(member, customAttribute);
          }
        }
        foreach (MemberInfo underlyingMember in type.GetUnderlyingMembers(BindingFlags.Instance | BindingFlags.NonPublic))
        {
          StateVariableAttribute customAttribute = CustomAttributeExtensions.GetCustomAttribute<StateVariableAttribute>(underlyingMember);
          if (customAttribute != null)
          {
            customAttribute.Type = underlyingMember.GetUnderlyingType();
            customAttribute.Name = underlyingMember.Name;
            customAttribute.DefaultValue = underlyingMember.GetUnderlyingValue(instance);
            yield return new Tuple<MemberInfo, StateVariableAttribute>(underlyingMember, customAttribute);
          }
        }
      }

      public static void SetObjectState(IActiveObject instance, IActiveObject state)
      {
        BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (Tuple<MemberInfo, StateVariableAttribute> stateVariable in StateVariableAttribute.GetStateVariables(instance.GetType()))
        {
          MemberInfo member1 = stateVariable.Item1;
          MemberInfo member2 = ((IEnumerable<MemberInfo>) state.GetType().GetMember(member1.Name, bindingAttr)).FirstOrDefault<MemberInfo>();
          if (member2 != (MemberInfo) null)
          {
            object underlyingValue = member2.GetUnderlyingValue((object) state);
            member1.SetUnderlyingValue((object) instance, underlyingValue);
          }
        }
      }

      private static bool EnumTryParse(
        Type enumType,
        string name,
        out object enumValue,
        bool ignoreCase = false)
      {
        enumValue = (object) null;
        try
        {
          enumValue = Enum.Parse(enumType, name, ignoreCase);
          return true;
        }
        catch
        {
        }
        return false;
      }

      public static ObjectEditorProperty[] GetEditorProperties(ObjectType objType)
      {
        ObjectInstanceAttribute instanceAttribute = ObjectInstanceAttribute.FromObject((object) objType);
        if (instanceAttribute == null)
          return new ObjectEditorProperty[0];
        Type objectInstanceType = instanceAttribute.ObjectInstanceType;
        List<ObjectEditorProperty> objectEditorPropertyList = new List<ObjectEditorProperty>();
        foreach (Tuple<MemberInfo, StateVariableAttribute> stateVariable in StateVariableAttribute.GetStateVariables(objectInstanceType))
        {
          MemberInfo memberInfo = stateVariable.Item1;
          StateVariableAttribute variableAttribute = stateVariable.Item2;
          objectEditorPropertyList.Add(new ObjectEditorProperty(variableAttribute.Name, variableAttribute.Name, variableAttribute.Type, variableAttribute.DefaultValue));
        }
        return objectEditorPropertyList.ToArray();
      }
    }
}
