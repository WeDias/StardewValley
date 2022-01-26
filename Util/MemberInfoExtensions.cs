// Decompiled with JetBrains decompiler
// Type: Sickhead.Engine.Util.MemberInfoExtensions
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Reflection;

namespace Sickhead.Engine.Util
{
  /// <summary>
  /// Allows Set/GetValue of MemberInfo(s) so that code does not need to
  /// be written to work specifically on PropertyInfo or FieldInfo.
  /// </summary>
  public static class MemberInfoExtensions
  {
    public static Type GetDataType(this MemberInfo info)
    {
      if ((object) (info as PropertyInfo) != null)
        return (info as PropertyInfo).PropertyType;
      return (object) (info as FieldInfo) != null ? (info as FieldInfo).FieldType : throw new InvalidOperationException(string.Format("MemberInfo.GetDataType is not possible for type={0}", (object) info.GetType()));
    }

    public static object GetValue(this MemberInfo info, object obj) => info.GetValue(obj, (object[]) null);

    public static void SetValue(this MemberInfo info, object obj, object value) => info.SetValue(obj, value, (object[]) null);

    public static object GetValue(this MemberInfo info, object obj, object[] index)
    {
      if ((object) (info as PropertyInfo) != null)
        return (info as PropertyInfo).GetValue(obj, index);
      return (object) (info as FieldInfo) != null ? (info as FieldInfo).GetValue(obj) : throw new InvalidOperationException(string.Format("MemberInfo.GetValue is not possible for type={0}", (object) info.GetType()));
    }

    public static void SetValue(this MemberInfo info, object obj, object value, object[] index)
    {
      if ((object) (info as PropertyInfo) != null)
        (info as PropertyInfo).SetValue(obj, value, index);
      else if ((object) (info as FieldInfo) != null)
      {
        (info as FieldInfo).SetValue(obj, value);
      }
      else
      {
        MethodInfo methodInfo = info as MethodInfo;
        throw new InvalidOperationException(string.Format("MemberInfo.SetValue is not possible for type={0}", (object) info.GetType()));
      }
    }

    public static bool IsStatic(this MemberInfo info)
    {
      if ((object) (info as PropertyInfo) != null)
        return (info as PropertyInfo).GetGetMethod(true).IsStatic;
      if ((object) (info as FieldInfo) != null)
        return (info as FieldInfo).IsStatic;
      return (object) (info as MethodInfo) != null ? (info as MethodInfo).IsStatic : throw new InvalidOperationException(string.Format("MemberInfo.IsStatic is not possible for type={0}", (object) info.GetType()));
    }

    /// <summary>
    /// Returns true if this is a property or field that is accessible to be set via reflection
    /// on all platforms. Note: windows phone can only set public or internal scope members.
    /// </summary>
    public static bool CanBeSet(this MemberInfo info)
    {
      if ((object) (info as PropertyInfo) != null)
      {
        PropertyInfo propertyInfo = info as PropertyInfo;
        MethodAttributes attributes = propertyInfo.GetSetMethod().Attributes;
        if (!propertyInfo.CanWrite)
          return true;
        return (attributes & MethodAttributes.Public) != MethodAttributes.Public && (attributes & MethodAttributes.Assembly) != MethodAttributes.Assembly;
      }
      FieldInfo fieldInfo = (object) (info as FieldInfo) != null ? info as FieldInfo : throw new InvalidOperationException(string.Format("MemberInfo.CanSet is not possible for type={0}", (object) info.GetType()));
      return !fieldInfo.IsPrivate && !fieldInfo.IsFamily;
    }

    /// <summary>
    /// In Win8 the static Delegate.Create was removed and added
    /// instead as an instance method on MethodInfo. Therefore it
    /// is most portable if the new api is used and this extension
    /// translates it to the older API on those platforms.
    /// </summary>
    public static Delegate CreateDelegate(this MethodInfo method, Type type, object target) => Delegate.CreateDelegate(type, target, method);

    public static Delegate CreateDelegate(this MethodInfo method, Type type) => Delegate.CreateDelegate(type, method);
  }
}
