// Decompiled with JetBrains decompiler
// Type: Force.DeepCloner.Helpers.DeepClonerExprGenerator
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Force.DeepCloner.Helpers
{
  internal static class DeepClonerExprGenerator
  {
    private static readonly ConcurrentDictionary<FieldInfo, bool> _readonlyFields = new ConcurrentDictionary<FieldInfo, bool>();
    private static FieldInfo _attributesFieldInfo = typeof (FieldInfo).GetPrivateField("m_fieldAttributes");

    internal static object GenerateClonerInternal(Type realType, bool asObject) => DeepClonerExprGenerator.GenerateProcessMethod(realType, asObject && realType.IsValueType());

    internal static void ForceSetField(FieldInfo field, object obj, object value)
    {
      FieldInfo privateField = field.GetType().GetPrivateField("m_fieldAttributes");
      if (privateField == (FieldInfo) null || !(privateField.GetValue((object) field) is FieldAttributes fieldAttributes))
        return;
      lock (privateField)
      {
        privateField.SetValue((object) field, (object) (fieldAttributes & ~FieldAttributes.InitOnly));
        field.SetValue(obj, value);
        privateField.SetValue((object) field, (object) (fieldAttributes | FieldAttributes.InitOnly));
      }
    }

    private static object GenerateProcessMethod(Type type, bool unboxStruct)
    {
      if (type.IsArray)
        return DeepClonerExprGenerator.GenerateProcessArrayMethod(type);
      if (type.FullName != null && type.FullName.StartsWith("System.Tuple`"))
      {
        Type[] source = type.GenericArguments();
        if (source.Length < 10 && ((IEnumerable<Type>) source).All<Type>(new Func<Type, bool>(DeepClonerSafeTypes.CanReturnSameObject)))
          return DeepClonerExprGenerator.GenerateProcessTupleMethod(type);
      }
      Type type1 = unboxStruct || type.IsClass() ? typeof (object) : type;
      List<Expression> expressionList = new List<Expression>();
      ParameterExpression parameterExpression1 = Expression.Parameter(type1);
      ParameterExpression left = parameterExpression1;
      ParameterExpression parameterExpression2 = Expression.Variable(type);
      ParameterExpression instance = Expression.Parameter(typeof (DeepCloneState));
      if (!type.IsValueType())
      {
        MethodInfo privateMethod = typeof (object).GetPrivateMethod("MemberwiseClone");
        expressionList.Add((Expression) Expression.Assign((Expression) parameterExpression2, (Expression) Expression.Convert((Expression) Expression.Call((Expression) parameterExpression1, privateMethod), type)));
        left = Expression.Variable(type);
        expressionList.Add((Expression) Expression.Assign((Expression) left, (Expression) Expression.Convert((Expression) parameterExpression1, type)));
        expressionList.Add((Expression) Expression.Call((Expression) instance, typeof (DeepCloneState).GetMethod("AddKnownRef"), (Expression) parameterExpression1, (Expression) parameterExpression2));
      }
      else if (unboxStruct)
      {
        expressionList.Add((Expression) Expression.Assign((Expression) parameterExpression2, (Expression) Expression.Unbox((Expression) parameterExpression1, type)));
        left = Expression.Variable(type);
        expressionList.Add((Expression) Expression.Assign((Expression) left, (Expression) parameterExpression2));
      }
      else
        expressionList.Add((Expression) Expression.Assign((Expression) parameterExpression2, (Expression) parameterExpression1));
      List<FieldInfo> fieldInfoList = new List<FieldInfo>();
      Type t = type;
      while (!(t.Name == "ContextBoundObject"))
      {
        fieldInfoList.AddRange((IEnumerable<FieldInfo>) t.GetDeclaredFields());
        t = t.BaseType();
        if (!(t != (Type) null))
          break;
      }
      foreach (FieldInfo fieldInfo in fieldInfoList)
      {
        if (!DeepClonerSafeTypes.CanReturnSameObject(fieldInfo.FieldType))
        {
          MethodInfo method;
          if (!fieldInfo.FieldType.IsValueType())
            method = typeof (DeepClonerGenerator).GetPrivateStaticMethod("CloneClassInternal");
          else
            method = typeof (DeepClonerGenerator).GetPrivateStaticMethod("CloneStructInternal").MakeGenericMethod(fieldInfo.FieldType);
          MemberExpression memberExpression = Expression.Field((Expression) left, fieldInfo);
          ParameterExpression parameterExpression3 = instance;
          Expression expression = (Expression) Expression.Call(method, (Expression) memberExpression, (Expression) parameterExpression3);
          if (!fieldInfo.FieldType.IsValueType())
            expression = (Expression) Expression.Convert(expression, fieldInfo.FieldType);
          if (DeepClonerExprGenerator._readonlyFields.GetOrAdd(fieldInfo, (Func<FieldInfo, bool>) (f => f.IsInitOnly)))
          {
            MethodInfo privateStaticMethod = typeof (DeepClonerExprGenerator).GetPrivateStaticMethod("ForceSetField");
            expressionList.Add((Expression) Expression.Call(privateStaticMethod, (Expression) Expression.Constant((object) fieldInfo), (Expression) Expression.Convert((Expression) parameterExpression2, typeof (object)), (Expression) Expression.Convert(expression, typeof (object))));
          }
          else
            expressionList.Add((Expression) Expression.Assign((Expression) Expression.Field((Expression) parameterExpression2, fieldInfo), expression));
        }
      }
      expressionList.Add((Expression) Expression.Convert((Expression) parameterExpression2, type1));
      Type delegateType = typeof (Func<,,>).MakeGenericType(type1, typeof (DeepCloneState), type1);
      List<ParameterExpression> variables = new List<ParameterExpression>();
      if (parameterExpression1 != left)
        variables.Add(left);
      variables.Add(parameterExpression2);
      BlockExpression body = Expression.Block((IEnumerable<ParameterExpression>) variables, (IEnumerable<Expression>) expressionList);
      ParameterExpression[] parameterExpressionArray = new ParameterExpression[2]
      {
        parameterExpression1,
        instance
      };
      return (object) Expression.Lambda(delegateType, (Expression) body, parameterExpressionArray).Compile();
    }

    private static object GenerateProcessArrayMethod(Type type)
    {
      Type elementType = type.GetElementType();
      int arrayRank = type.GetArrayRank();
      MethodInfo method;
      if (arrayRank != 1 || type != elementType.MakeArrayType())
      {
        if (arrayRank == 2 && type == elementType.MakeArrayType())
          method = typeof (DeepClonerGenerator).GetPrivateStaticMethod("Clone2DimArrayInternal").MakeGenericMethod(elementType);
        else
          method = typeof (DeepClonerGenerator).GetPrivateStaticMethod("CloneAbstractArrayInternal");
      }
      else
      {
        string methodName = "Clone1DimArrayClassInternal";
        if (DeepClonerSafeTypes.CanReturnSameObject(elementType))
          methodName = "Clone1DimArraySafeInternal";
        else if (elementType.IsValueType())
          methodName = "Clone1DimArrayStructInternal";
        method = typeof (DeepClonerGenerator).GetPrivateStaticMethod(methodName).MakeGenericMethod(elementType);
      }
      ParameterExpression parameterExpression1 = Expression.Parameter(typeof (object));
      ParameterExpression parameterExpression2 = Expression.Parameter(typeof (DeepCloneState));
      MethodCallExpression body = Expression.Call(method, (Expression) Expression.Convert((Expression) parameterExpression1, type), (Expression) parameterExpression2);
      return (object) Expression.Lambda(typeof (Func<,,>).MakeGenericType(typeof (object), typeof (DeepCloneState), typeof (object)), (Expression) body, parameterExpression1, parameterExpression2).Compile();
    }

    private static object GenerateProcessTupleMethod(Type type)
    {
      ParameterExpression parameterExpression = Expression.Parameter(typeof (object));
      ParameterExpression instance = Expression.Parameter(typeof (DeepCloneState));
      ParameterExpression local = Expression.Variable(type);
      BinaryExpression binaryExpression1 = Expression.Assign((Expression) local, (Expression) Expression.Convert((Expression) parameterExpression, type));
      Type delegateType = typeof (Func<object, DeepCloneState, object>);
      int tupleLength = type.GenericArguments().Length;
      BinaryExpression binaryExpression2 = Expression.Assign((Expression) local, (Expression) Expression.New(((IEnumerable<ConstructorInfo>) type.GetPublicConstructors()).First<ConstructorInfo>((Func<ConstructorInfo, bool>) (x => x.GetParameters().Length == tupleLength)), (IEnumerable<Expression>) ((IEnumerable<PropertyInfo>) type.GetPublicProperties()).OrderBy<PropertyInfo, string>((Func<PropertyInfo, string>) (x => x.Name)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (x => x.CanRead && x.Name.StartsWith("Item") && char.IsDigit(x.Name[4]))).Select<PropertyInfo, MemberExpression>((Func<PropertyInfo, MemberExpression>) (x => Expression.Property((Expression) local, x.Name)))));
      BlockExpression body = Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        local
      }, (Expression) binaryExpression1, (Expression) binaryExpression2, (Expression) Expression.Call((Expression) instance, typeof (DeepCloneState).GetMethod("AddKnownRef"), (Expression) parameterExpression, (Expression) local), (Expression) parameterExpression);
      ParameterExpression[] parameterExpressionArray = new ParameterExpression[2]
      {
        parameterExpression,
        instance
      };
      return (object) Expression.Lambda(delegateType, (Expression) body, parameterExpressionArray).Compile();
    }
  }
}
