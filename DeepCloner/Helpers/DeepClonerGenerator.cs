// Decompiled with JetBrains decompiler
// Type: Force.DeepCloner.Helpers.DeepClonerGenerator
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Linq;

namespace Force.DeepCloner.Helpers
{
  internal static class DeepClonerGenerator
  {
    public static T CloneObject<T>(T obj)
    {
      if ((object) obj is ValueType)
      {
        Type type = obj.GetType();
        if (typeof (T) == type)
          return DeepClonerSafeTypes.CanReturnSameObject(type) ? obj : DeepClonerGenerator.CloneStructInternal<T>(obj, new DeepCloneState());
      }
      return (T) DeepClonerGenerator.CloneClassRoot((object) obj);
    }

    private static object CloneClassRoot(object obj)
    {
      if (obj == null)
        return (object) null;
      Func<object, DeepCloneState, object> orAddClass = (Func<object, DeepCloneState, object>) DeepClonerCache.GetOrAddClass<object>(obj.GetType(), (Func<Type, object>) (t => DeepClonerGenerator.GenerateCloner(t, true)));
      return orAddClass == null ? obj : orAddClass(obj, new DeepCloneState());
    }

    internal static object CloneClassInternal(object obj, DeepCloneState state)
    {
      if (obj == null)
        return (object) null;
      Func<object, DeepCloneState, object> orAddClass = (Func<object, DeepCloneState, object>) DeepClonerCache.GetOrAddClass<object>(obj.GetType(), (Func<Type, object>) (t => DeepClonerGenerator.GenerateCloner(t, true)));
      return orAddClass == null ? obj : state.GetKnownRef(obj) ?? orAddClass(obj, state);
    }

    private static T CloneStructInternal<T>(T obj, DeepCloneState state)
    {
      Func<T, DeepCloneState, T> clonerForValueType = DeepClonerGenerator.GetClonerForValueType<T>();
      return clonerForValueType == null ? obj : clonerForValueType(obj, state);
    }

    internal static T[] Clone1DimArraySafeInternal<T>(T[] obj, DeepCloneState state)
    {
      T[] objArray = new T[obj.Length];
      state.AddKnownRef((object) obj, (object) objArray);
      Array.Copy((Array) obj, (Array) objArray, obj.Length);
      return objArray;
    }

    internal static T[] Clone1DimArrayStructInternal<T>(T[] obj, DeepCloneState state)
    {
      if (obj == null)
        return (T[]) null;
      int length = obj.Length;
      T[] to = new T[length];
      state.AddKnownRef((object) obj, (object) to);
      Func<T, DeepCloneState, T> clonerForValueType = DeepClonerGenerator.GetClonerForValueType<T>();
      for (int index = 0; index < length; ++index)
        to[index] = clonerForValueType(obj[index], state);
      return to;
    }

    internal static T[] Clone1DimArrayClassInternal<T>(T[] obj, DeepCloneState state)
    {
      if (obj == null)
        return (T[]) null;
      int length = obj.Length;
      T[] to = new T[length];
      state.AddKnownRef((object) obj, (object) to);
      for (int index = 0; index < length; ++index)
        to[index] = (T) DeepClonerGenerator.CloneClassInternal((object) obj[index], state);
      return to;
    }

    internal static T[,] Clone2DimArrayInternal<T>(T[,] obj, DeepCloneState state)
    {
      if (obj == null)
        return (T[,]) null;
      int length1 = obj.GetLength(0);
      int length2 = obj.GetLength(1);
      T[,] objArray = new T[length1, length2];
      state.AddKnownRef((object) obj, (object) objArray);
      if (DeepClonerSafeTypes.CanReturnSameObject(typeof (T)))
      {
        Array.Copy((Array) obj, (Array) objArray, obj.Length);
        return objArray;
      }
      if (typeof (T).IsValueType())
      {
        Func<T, DeepCloneState, T> clonerForValueType = DeepClonerGenerator.GetClonerForValueType<T>();
        for (int index1 = 0; index1 < length1; ++index1)
        {
          for (int index2 = 0; index2 < length2; ++index2)
            objArray[index1, index2] = clonerForValueType(obj[index1, index2], state);
        }
      }
      else
      {
        for (int index3 = 0; index3 < length1; ++index3)
        {
          for (int index4 = 0; index4 < length2; ++index4)
            objArray[index3, index4] = (T) DeepClonerGenerator.CloneClassInternal((object) obj[index3, index4], state);
        }
      }
      return objArray;
    }

    internal static Array CloneAbstractArrayInternal(Array obj, DeepCloneState state)
    {
      if (obj == null)
        return (Array) null;
      int rank = obj.Rank;
      int[] array1 = Enumerable.Range(0, rank).Select<int, int>(new Func<int, int>(obj.GetLowerBound)).ToArray<int>();
      int[] array2 = Enumerable.Range(0, rank).Select<int, int>(new Func<int, int>(obj.GetLength)).ToArray<int>();
      int[] array3 = Enumerable.Range(0, rank).Select<int, int>(new Func<int, int>(obj.GetLowerBound)).ToArray<int>();
      Array instance = Array.CreateInstance(obj.GetType().GetElementType(), array2, array1);
      state.AddKnownRef((object) obj, (object) instance);
label_3:
      instance.SetValue(DeepClonerGenerator.CloneClassInternal(obj.GetValue(array3), state), array3);
      int index = rank - 1;
      do
      {
        ++array3[index];
        if (array3[index] >= array1[index] + array2[index])
        {
          array3[index] = array1[index];
          --index;
        }
        else
          goto label_3;
      }
      while (index >= 0);
      return instance;
    }

    internal static Func<T, DeepCloneState, T> GetClonerForValueType<T>() => (Func<T, DeepCloneState, T>) DeepClonerCache.GetOrAddStructAsObject<object>(typeof (T), (Func<Type, object>) (t => DeepClonerGenerator.GenerateCloner(t, false)));

    private static object GenerateCloner(Type t, bool asObject) => DeepClonerSafeTypes.CanReturnSameObject(t) && asObject && !t.IsValueType() ? (object) null : DeepClonerExprGenerator.GenerateClonerInternal(t, asObject);

    public static object CloneObjectTo(object objFrom, object objTo, bool isDeep)
    {
      if (objTo == null)
        return (object) null;
      Type type = objFrom != null ? objFrom.GetType() : throw new ArgumentNullException(nameof (objFrom), "Cannot copy null object to another");
      if (!type.IsInstanceOfType(objTo))
        throw new InvalidOperationException("From object should be derived from From object, but From object has type " + objFrom.GetType().FullName + " and to " + objTo.GetType().FullName);
      if (objFrom is string)
        throw new InvalidOperationException("It is forbidden to clone strings");
      Func<object, object, DeepCloneState, object> func = isDeep ? (Func<object, object, DeepCloneState, object>) DeepClonerCache.GetOrAddDeepClassTo<object>(type, (Func<Type, object>) (t => ClonerToExprGenerator.GenerateClonerInternal(t, true))) : (Func<object, object, DeepCloneState, object>) DeepClonerCache.GetOrAddShallowClassTo<object>(type, (Func<Type, object>) (t => ClonerToExprGenerator.GenerateClonerInternal(t, false)));
      return func == null ? objTo : func(objFrom, objTo, new DeepCloneState());
    }
  }
}
