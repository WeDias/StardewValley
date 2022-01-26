// Decompiled with JetBrains decompiler
// Type: Force.DeepCloner.Helpers.DeepClonerCache
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Collections.Concurrent;

namespace Force.DeepCloner.Helpers
{
  internal static class DeepClonerCache
  {
    private static readonly ConcurrentDictionary<Type, object> _typeCache = new ConcurrentDictionary<Type, object>();
    private static readonly ConcurrentDictionary<Type, object> _typeCacheDeepTo = new ConcurrentDictionary<Type, object>();
    private static readonly ConcurrentDictionary<Type, object> _typeCacheShallowTo = new ConcurrentDictionary<Type, object>();
    private static readonly ConcurrentDictionary<Type, object> _structAsObjectCache = new ConcurrentDictionary<Type, object>();
    private static readonly ConcurrentDictionary<Tuple<Type, Type>, object> _typeConvertCache = new ConcurrentDictionary<Tuple<Type, Type>, object>();

    public static object GetOrAddClass<T>(Type type, Func<Type, T> adder)
    {
      object orAddClass;
      if (DeepClonerCache._typeCache.TryGetValue(type, out orAddClass))
        return orAddClass;
      lock (type)
        return DeepClonerCache._typeCache.GetOrAdd(type, (Func<Type, object>) (t => (object) adder(t)));
    }

    public static object GetOrAddDeepClassTo<T>(Type type, Func<Type, T> adder)
    {
      object orAddDeepClassTo;
      if (DeepClonerCache._typeCacheDeepTo.TryGetValue(type, out orAddDeepClassTo))
        return orAddDeepClassTo;
      lock (type)
        return DeepClonerCache._typeCacheDeepTo.GetOrAdd(type, (Func<Type, object>) (t => (object) adder(t)));
    }

    public static object GetOrAddShallowClassTo<T>(Type type, Func<Type, T> adder)
    {
      object addShallowClassTo;
      if (DeepClonerCache._typeCacheShallowTo.TryGetValue(type, out addShallowClassTo))
        return addShallowClassTo;
      lock (type)
        return DeepClonerCache._typeCacheShallowTo.GetOrAdd(type, (Func<Type, object>) (t => (object) adder(t)));
    }

    public static object GetOrAddStructAsObject<T>(Type type, Func<Type, T> adder)
    {
      object addStructAsObject;
      if (DeepClonerCache._structAsObjectCache.TryGetValue(type, out addStructAsObject))
        return addStructAsObject;
      lock (type)
        return DeepClonerCache._structAsObjectCache.GetOrAdd(type, (Func<Type, object>) (t => (object) adder(t)));
    }

    public static T GetOrAddConvertor<T>(Type from, Type to, Func<Type, Type, T> adder) => (T) DeepClonerCache._typeConvertCache.GetOrAdd(new Tuple<Type, Type>(from, to), (Func<Tuple<Type, Type>, object>) (tuple => (object) adder(tuple.Item1, tuple.Item2)));

    /// <summary>
    /// This method can be used when we switch between safe / unsafe variants (for testing)
    /// </summary>
    public static void ClearCache()
    {
      DeepClonerCache._typeCache.Clear();
      DeepClonerCache._typeCacheDeepTo.Clear();
      DeepClonerCache._typeCacheShallowTo.Clear();
      DeepClonerCache._structAsObjectCache.Clear();
      DeepClonerCache._typeConvertCache.Clear();
    }
  }
}
