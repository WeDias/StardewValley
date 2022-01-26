// Decompiled with JetBrains decompiler
// Type: Force.DeepCloner.Helpers.ClonerToExprGenerator
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Force.DeepCloner.Helpers
{
  internal static class ClonerToExprGenerator
  {
    internal static object GenerateClonerInternal(Type realType, bool isDeepClone) => !realType.IsValueType() ? ClonerToExprGenerator.GenerateProcessMethod(realType, isDeepClone) : throw new InvalidOperationException("Operation is valid only for reference types");

    private static object GenerateProcessMethod(Type type, bool isDeepClone)
    {
      if (type.IsArray)
        return ClonerToExprGenerator.GenerateProcessArrayMethod(type, isDeepClone);
      Type type1 = typeof (object);
      List<Expression> expressionList = new List<Expression>();
      ParameterExpression parameterExpression1 = Expression.Parameter(type1);
      ParameterExpression parameterExpression2 = Expression.Parameter(type1);
      ParameterExpression instance = Expression.Parameter(typeof (DeepCloneState));
      ParameterExpression left1 = Expression.Variable(type);
      ParameterExpression left2 = Expression.Variable(type);
      expressionList.Add((Expression) Expression.Assign((Expression) left1, (Expression) Expression.Convert((Expression) parameterExpression1, type)));
      expressionList.Add((Expression) Expression.Assign((Expression) left2, (Expression) Expression.Convert((Expression) parameterExpression2, type)));
      if (isDeepClone)
        expressionList.Add((Expression) Expression.Call((Expression) instance, typeof (DeepCloneState).GetMethod("AddKnownRef"), (Expression) parameterExpression1, (Expression) parameterExpression2));
      List<FieldInfo> fieldInfoList = new List<FieldInfo>();
      Type t = type;
      while (!(t.Name == "ContextBoundObject"))
      {
        fieldInfoList.AddRange((IEnumerable<FieldInfo>) t.GetDeclaredFields());
        t = t.BaseType();
        if (!(t != (Type) null))
          break;
      }
      foreach (FieldInfo field in fieldInfoList)
      {
        if (isDeepClone && !DeepClonerSafeTypes.CanReturnSameObject(field.FieldType))
        {
          MethodInfo method;
          if (!field.FieldType.IsValueType())
            method = typeof (DeepClonerGenerator).GetPrivateStaticMethod("CloneClassInternal");
          else
            method = typeof (DeepClonerGenerator).GetPrivateStaticMethod("CloneStructInternal").MakeGenericMethod(field.FieldType);
          MemberExpression memberExpression = Expression.Field((Expression) left1, field);
          ParameterExpression parameterExpression3 = instance;
          Expression expression = (Expression) Expression.Call(method, (Expression) memberExpression, (Expression) parameterExpression3);
          if (!field.FieldType.IsValueType())
            expression = (Expression) Expression.Convert(expression, field.FieldType);
          if (field.IsInitOnly)
          {
            MethodInfo privateStaticMethod = typeof (DeepClonerExprGenerator).GetPrivateStaticMethod("ForceSetField");
            expressionList.Add((Expression) Expression.Call(privateStaticMethod, (Expression) Expression.Constant((object) field), (Expression) Expression.Convert((Expression) left2, typeof (object)), (Expression) Expression.Convert(expression, typeof (object))));
          }
          else
            expressionList.Add((Expression) Expression.Assign((Expression) Expression.Field((Expression) left2, field), expression));
        }
        else
          expressionList.Add((Expression) Expression.Assign((Expression) Expression.Field((Expression) left2, field), (Expression) Expression.Field((Expression) left1, field)));
      }
      expressionList.Add((Expression) Expression.Convert((Expression) left2, type1));
      Type delegateType = typeof (Func<,,,>).MakeGenericType(type1, type1, typeof (DeepCloneState), type1);
      List<ParameterExpression> variables = new List<ParameterExpression>();
      if (parameterExpression1 != left1)
        variables.Add(left1);
      if (parameterExpression2 != left2)
        variables.Add(left2);
      BlockExpression body = Expression.Block((IEnumerable<ParameterExpression>) variables, (IEnumerable<Expression>) expressionList);
      ParameterExpression[] parameterExpressionArray = new ParameterExpression[3]
      {
        parameterExpression1,
        parameterExpression2,
        instance
      };
      return (object) Expression.Lambda(delegateType, (Expression) body, parameterExpressionArray).Compile();
    }

    private static object GenerateProcessArrayMethod(Type type, bool isDeep)
    {
      Type elementType = type.GetElementType();
      int arrayRank = type.GetArrayRank();
      ParameterExpression parameterExpression1 = Expression.Parameter(typeof (object));
      ParameterExpression parameterExpression2 = Expression.Parameter(typeof (object));
      ParameterExpression parameterExpression3 = Expression.Parameter(typeof (DeepCloneState));
      Type delegateType = typeof (Func<,,,>).MakeGenericType(typeof (object), typeof (object), typeof (DeepCloneState), typeof (object));
      if (arrayRank == 1 && type == elementType.MakeArrayType())
      {
        if (!isDeep)
        {
          MethodCallExpression body = Expression.Call(typeof (ClonerToExprGenerator).GetPrivateStaticMethod("ShallowClone1DimArraySafeInternal").MakeGenericMethod(elementType), (Expression) Expression.Convert((Expression) parameterExpression1, type), (Expression) Expression.Convert((Expression) parameterExpression2, type));
          return (object) Expression.Lambda(delegateType, (Expression) body, parameterExpression1, parameterExpression2, parameterExpression3).Compile();
        }
        string methodName = "Clone1DimArrayClassInternal";
        if (DeepClonerSafeTypes.CanReturnSameObject(elementType))
          methodName = "Clone1DimArraySafeInternal";
        else if (elementType.IsValueType())
          methodName = "Clone1DimArrayStructInternal";
        MethodCallExpression body1 = Expression.Call(typeof (ClonerToExprGenerator).GetPrivateStaticMethod(methodName).MakeGenericMethod(elementType), (Expression) Expression.Convert((Expression) parameterExpression1, type), (Expression) Expression.Convert((Expression) parameterExpression2, type), (Expression) parameterExpression3);
        return (object) Expression.Lambda(delegateType, (Expression) body1, parameterExpression1, parameterExpression2, parameterExpression3).Compile();
      }
      MethodCallExpression body2 = Expression.Call(typeof (ClonerToExprGenerator).GetPrivateStaticMethod(arrayRank != 2 || !(type == elementType.MakeArrayType()) ? "CloneAbstractArrayInternal" : "Clone2DimArrayInternal"), (Expression) Expression.Convert((Expression) parameterExpression1, type), (Expression) Expression.Convert((Expression) parameterExpression2, type), (Expression) parameterExpression3, (Expression) Expression.Constant((object) isDeep));
      return (object) Expression.Lambda(delegateType, (Expression) body2, parameterExpression1, parameterExpression2, parameterExpression3).Compile();
    }

    internal static T[] ShallowClone1DimArraySafeInternal<T>(T[] objFrom, T[] objTo)
    {
      int length = Math.Min(objFrom.Length, objTo.Length);
      Array.Copy((Array) objFrom, (Array) objTo, length);
      return objTo;
    }

    internal static T[] Clone1DimArraySafeInternal<T>(T[] objFrom, T[] objTo, DeepCloneState state)
    {
      int length = Math.Min(objFrom.Length, objTo.Length);
      state.AddKnownRef((object) objFrom, (object) objTo);
      Array.Copy((Array) objFrom, (Array) objTo, length);
      return objTo;
    }

    internal static T[] Clone1DimArrayStructInternal<T>(
      T[] objFrom,
      T[] objTo,
      DeepCloneState state)
    {
      if (objFrom == null || objTo == null)
        return (T[]) null;
      int num = Math.Min(objFrom.Length, objTo.Length);
      state.AddKnownRef((object) objFrom, (object) objTo);
      Func<T, DeepCloneState, T> clonerForValueType = DeepClonerGenerator.GetClonerForValueType<T>();
      for (int index = 0; index < num; ++index)
        objTo[index] = clonerForValueType(objTo[index], state);
      return objTo;
    }

    internal static T[] Clone1DimArrayClassInternal<T>(
      T[] objFrom,
      T[] objTo,
      DeepCloneState state)
    {
      if (objFrom == null || objTo == null)
        return (T[]) null;
      int num = Math.Min(objFrom.Length, objTo.Length);
      state.AddKnownRef((object) objFrom, (object) objTo);
      for (int index = 0; index < num; ++index)
        objTo[index] = (T) DeepClonerGenerator.CloneClassInternal((object) objFrom[index], state);
      return objTo;
    }

    internal static T[,] Clone2DimArrayInternal<T>(
      T[,] objFrom,
      T[,] objTo,
      DeepCloneState state,
      bool isDeep)
    {
      if (objFrom == null || objTo == null)
        return (T[,]) null;
      int num1 = Math.Min(objFrom.GetLength(0), objTo.GetLength(0));
      int num2 = Math.Min(objFrom.GetLength(1), objTo.GetLength(1));
      state.AddKnownRef((object) objFrom, (object) objTo);
      if ((!isDeep || DeepClonerSafeTypes.CanReturnSameObject(typeof (T))) && objFrom.GetLength(0) == objTo.GetLength(0) && objFrom.GetLength(1) == objTo.GetLength(1))
      {
        Array.Copy((Array) objFrom, (Array) objTo, objFrom.Length);
        return objTo;
      }
      if (!isDeep)
      {
        for (int index1 = 0; index1 < num1; ++index1)
        {
          for (int index2 = 0; index2 < num2; ++index2)
            objTo[index1, index2] = objFrom[index1, index2];
        }
        return objTo;
      }
      if (typeof (T).IsValueType())
      {
        Func<T, DeepCloneState, T> clonerForValueType = DeepClonerGenerator.GetClonerForValueType<T>();
        for (int index3 = 0; index3 < num1; ++index3)
        {
          for (int index4 = 0; index4 < num2; ++index4)
            objTo[index3, index4] = clonerForValueType(objFrom[index3, index4], state);
        }
      }
      else
      {
        for (int index5 = 0; index5 < num1; ++index5)
        {
          for (int index6 = 0; index6 < num2; ++index6)
            objTo[index5, index6] = (T) DeepClonerGenerator.CloneClassInternal((object) objFrom[index5, index6], state);
        }
      }
      return objTo;
    }

    internal static Array CloneAbstractArrayInternal(
      Array objFrom,
      Array objTo,
      DeepCloneState state,
      bool isDeep)
    {
      if (objFrom == null || objTo == null)
        return (Array) null;
      int rank = objFrom.Rank;
      if (objTo.Rank != rank)
        throw new InvalidOperationException("Invalid rank of target array");
      int[] array1 = Enumerable.Range(0, rank).Select<int, int>(new Func<int, int>(objFrom.GetLowerBound)).ToArray<int>();
      int[] array2 = Enumerable.Range(0, rank).Select<int, int>(new Func<int, int>(objTo.GetLowerBound)).ToArray<int>();
      int[] array3 = Enumerable.Range(0, rank).Select<int, int>((Func<int, int>) (x => Math.Min(objFrom.GetLength(x), objTo.GetLength(x)))).ToArray<int>();
      int[] array4 = Enumerable.Range(0, rank).Select<int, int>(new Func<int, int>(objFrom.GetLowerBound)).ToArray<int>();
      int[] array5 = Enumerable.Range(0, rank).Select<int, int>(new Func<int, int>(objTo.GetLowerBound)).ToArray<int>();
      state.AddKnownRef((object) objFrom, (object) objTo);
label_5:
      if (isDeep)
        objTo.SetValue(DeepClonerGenerator.CloneClassInternal(objFrom.GetValue(array4), state), array5);
      else
        objTo.SetValue(objFrom.GetValue(array4), array5);
      int index = rank - 1;
      do
      {
        ++array4[index];
        ++array5[index];
        if (array4[index] >= array1[index] + array3[index])
        {
          array4[index] = array1[index];
          array5[index] = array2[index];
          --index;
        }
        else
          goto label_5;
      }
      while (index >= 0);
      return objTo;
    }
  }
}
