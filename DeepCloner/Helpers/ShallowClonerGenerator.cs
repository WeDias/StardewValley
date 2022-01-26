// Decompiled with JetBrains decompiler
// Type: Force.DeepCloner.Helpers.ShallowClonerGenerator
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;

namespace Force.DeepCloner.Helpers
{
  internal static class ShallowClonerGenerator
  {
    public static T CloneObject<T>(T obj)
    {
      if ((object) obj is ValueType)
        return typeof (T) == obj.GetType() ? obj : (T) ShallowObjectCloner.CloneObject((object) obj);
      if ((object) obj == null)
        return (T) null;
      return DeepClonerSafeTypes.CanReturnSameObject(obj.GetType()) ? obj : (T) ShallowObjectCloner.CloneObject((object) obj);
    }
  }
}
