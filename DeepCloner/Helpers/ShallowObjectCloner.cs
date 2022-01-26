// Decompiled with JetBrains decompiler
// Type: Force.DeepCloner.Helpers.ShallowObjectCloner
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Force.DeepCloner.Helpers
{
  /// <summary>
  /// Internal class but due implementation restriction should be public
  /// </summary>
  public abstract class ShallowObjectCloner
  {
    private static readonly ShallowObjectCloner _unsafeInstance;
    private static ShallowObjectCloner _instance = (ShallowObjectCloner) new ShallowObjectCloner.ShallowSafeObjectCloner();

    /// <summary>Abstract method for real object cloning</summary>
    protected abstract object DoCloneObject(object obj);

    /// <summary>Performs real shallow object clone</summary>
    public static object CloneObject(object obj) => ShallowObjectCloner._instance.DoCloneObject(obj);

    internal static bool IsSafeVariant() => ShallowObjectCloner._instance is ShallowObjectCloner.ShallowSafeObjectCloner;

    static ShallowObjectCloner() => ShallowObjectCloner._unsafeInstance = ShallowObjectCloner._instance;

    /// <summary>Purpose of this method is testing variants</summary>
    internal static void SwitchTo(bool isSafe)
    {
      DeepClonerCache.ClearCache();
      if (isSafe)
        ShallowObjectCloner._instance = (ShallowObjectCloner) new ShallowObjectCloner.ShallowSafeObjectCloner();
      else
        ShallowObjectCloner._instance = ShallowObjectCloner._unsafeInstance;
    }

    private class ShallowSafeObjectCloner : ShallowObjectCloner
    {
      private static readonly Func<object, object> _cloneFunc;

      static ShallowSafeObjectCloner()
      {
        MethodInfo privateMethod = typeof (object).GetPrivateMethod("MemberwiseClone");
        ShallowObjectCloner.ShallowSafeObjectCloner._cloneFunc = ((Expression<Func<object, object>>) (obj => Expression.Call(obj, privateMethod))).Compile();
      }

      protected override object DoCloneObject(object obj) => ShallowObjectCloner.ShallowSafeObjectCloner._cloneFunc(obj);
    }
  }
}
