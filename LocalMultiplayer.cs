// Decompiled with JetBrains decompiler
// Type: StardewValley.LocalMultiplayer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace StardewValley
{
  public class LocalMultiplayer
  {
    internal static List<FieldInfo> staticFields;
    internal static List<object> staticDefaults;
    public static Type StaticVarHolderType;
    private static DynamicMethod staticDefaultMethod;
    private static DynamicMethod staticSaveMethod;
    private static DynamicMethod staticLoadMethod;
    public static LocalMultiplayer.StaticInstanceMethod StaticSetDefault;
    public static LocalMultiplayer.StaticInstanceMethod StaticSave;
    public static LocalMultiplayer.StaticInstanceMethod StaticLoad;

    public static bool IsLocalMultiplayer(bool is_local_only = false) => is_local_only ? Game1.hasLocalClientsOnly : GameRunner.instance.gameInstances.Count > 1;

    public static void Initialize()
    {
      LocalMultiplayer.GetStaticFieldsAndDefaults();
      LocalMultiplayer.GenerateDynamicMethodsForStatics();
    }

    private static void GetStaticFieldsAndDefaults()
    {
      LocalMultiplayer.staticFields = new List<FieldInfo>();
      LocalMultiplayer.staticDefaults = new List<object>();
      List<Type> typeList1 = new List<Type>();
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        "Microsoft",
        "MonoGame",
        "mscorlib",
        "NetCode",
        "System",
        "xTile",
        "FAudio-CS"
      };
      List<Type> typeList2 = new List<Type>();
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (!stringSet.Contains(assembly.GetName().Name.Split('.')[0]))
        {
          foreach (Type type in assembly.GetTypes())
            typeList2.Add(type);
        }
      }
      foreach (Type type in typeList2)
      {
        if (type.GetCustomAttributes(typeof (CompilerGeneratedAttribute), true).Length == 0)
        {
          bool flag = false;
          if (type.GetCustomAttributes(typeof (InstanceStatics), true).Length != 0)
            flag = true;
          foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
          {
            if (!field.IsInitOnly && field.IsStatic && !field.IsLiteral && (flag || field.GetCustomAttributes(typeof (InstancedStatic), true).Length != 0) && field.GetCustomAttributes(typeof (NonInstancedStatic), true).Length == 0)
            {
              RuntimeHelpers.RunClassConstructor(field.DeclaringType.TypeHandle);
              LocalMultiplayer.staticFields.Add(field);
              LocalMultiplayer.staticDefaults.Add(field.GetValue((object) null));
            }
          }
        }
      }
    }

    private static void GenerateDynamicMethodsForStatics()
    {
      TypeBuilder typeBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("StardewValley.StaticInstanceVars"), AssemblyBuilderAccess.RunAndCollect).DefineDynamicModule("MainModule").DefineType("StardewValley.StaticInstanceVars", TypeAttributes.Public | TypeAttributes.AutoClass);
      foreach (FieldInfo staticField in LocalMultiplayer.staticFields)
        typeBuilder.DefineField(staticField.DeclaringType.Name + "_" + staticField.Name, staticField.FieldType, FieldAttributes.Public);
      LocalMultiplayer.StaticVarHolderType = typeBuilder.CreateType();
      LocalMultiplayer.staticDefaultMethod = new DynamicMethod("SetStaticVarsToDefault", (Type) null, new Type[1]
      {
        typeof (object)
      }, typeof (Game1).Module, true);
      ILGenerator ilGenerator1 = LocalMultiplayer.staticDefaultMethod.GetILGenerator();
      LocalBuilder localBuilder1 = ilGenerator1.DeclareLocal(LocalMultiplayer.StaticVarHolderType);
      ilGenerator1.Emit(OpCodes.Ldarg_0);
      ilGenerator1.Emit(OpCodes.Castclass, LocalMultiplayer.StaticVarHolderType);
      ilGenerator1.Emit(OpCodes.Stloc_0);
      FieldInfo field = typeof (LocalMultiplayer).GetField("staticDefaults", BindingFlags.Static | BindingFlags.NonPublic);
      MethodInfo method = typeof (List<object>).GetMethod("get_Item");
      for (int index = 0; index < LocalMultiplayer.staticFields.Count; ++index)
      {
        FieldInfo staticField = LocalMultiplayer.staticFields[index];
        ilGenerator1.Emit(OpCodes.Ldloc, localBuilder1.LocalIndex);
        ilGenerator1.Emit(OpCodes.Ldsfld, field);
        ilGenerator1.Emit(OpCodes.Ldc_I4, index);
        ilGenerator1.Emit(OpCodes.Callvirt, method);
        if (staticField.FieldType.IsValueType)
          ilGenerator1.Emit(OpCodes.Unbox_Any, staticField.FieldType);
        else
          ilGenerator1.Emit(OpCodes.Castclass, staticField.FieldType);
        ilGenerator1.Emit(OpCodes.Stfld, LocalMultiplayer.StaticVarHolderType.GetField(staticField.DeclaringType.Name + "_" + staticField.Name));
      }
      ilGenerator1.Emit(OpCodes.Ret);
      LocalMultiplayer.StaticSetDefault = (LocalMultiplayer.StaticInstanceMethod) LocalMultiplayer.staticDefaultMethod.CreateDelegate(typeof (LocalMultiplayer.StaticInstanceMethod));
      LocalMultiplayer.staticSaveMethod = new DynamicMethod("SaveStaticVars", (Type) null, new Type[1]
      {
        typeof (object)
      }, typeof (Game1).Module, true);
      ILGenerator ilGenerator2 = LocalMultiplayer.staticSaveMethod.GetILGenerator();
      LocalBuilder localBuilder2 = ilGenerator2.DeclareLocal(LocalMultiplayer.StaticVarHolderType);
      ilGenerator2.Emit(OpCodes.Ldarg_0);
      ilGenerator2.Emit(OpCodes.Castclass, LocalMultiplayer.StaticVarHolderType);
      ilGenerator2.Emit(OpCodes.Stloc_0);
      foreach (FieldInfo staticField in LocalMultiplayer.staticFields)
      {
        ilGenerator2.Emit(OpCodes.Ldloc, localBuilder2.LocalIndex);
        ilGenerator2.Emit(OpCodes.Ldsfld, staticField);
        ilGenerator2.Emit(OpCodes.Stfld, LocalMultiplayer.StaticVarHolderType.GetField(staticField.DeclaringType.Name + "_" + staticField.Name));
      }
      ilGenerator2.Emit(OpCodes.Ret);
      LocalMultiplayer.StaticSave = (LocalMultiplayer.StaticInstanceMethod) LocalMultiplayer.staticSaveMethod.CreateDelegate(typeof (LocalMultiplayer.StaticInstanceMethod));
      LocalMultiplayer.staticLoadMethod = new DynamicMethod("LoadStaticVars", (Type) null, new Type[1]
      {
        typeof (object)
      }, typeof (Game1).Module, true);
      ILGenerator ilGenerator3 = LocalMultiplayer.staticLoadMethod.GetILGenerator();
      LocalBuilder localBuilder3 = ilGenerator3.DeclareLocal(LocalMultiplayer.StaticVarHolderType);
      ilGenerator3.Emit(OpCodes.Ldarg_0);
      ilGenerator3.Emit(OpCodes.Castclass, LocalMultiplayer.StaticVarHolderType);
      ilGenerator3.Emit(OpCodes.Stloc_0);
      foreach (FieldInfo staticField in LocalMultiplayer.staticFields)
      {
        ilGenerator3.Emit(OpCodes.Ldloc, localBuilder3.LocalIndex);
        ilGenerator3.Emit(OpCodes.Ldfld, LocalMultiplayer.StaticVarHolderType.GetField(staticField.DeclaringType.Name + "_" + staticField.Name));
        ilGenerator3.Emit(OpCodes.Stsfld, staticField);
      }
      ilGenerator3.Emit(OpCodes.Ret);
      LocalMultiplayer.StaticLoad = (LocalMultiplayer.StaticInstanceMethod) LocalMultiplayer.staticLoadMethod.CreateDelegate(typeof (LocalMultiplayer.StaticInstanceMethod));
    }

    public static void SaveOptions()
    {
      if (Game1.player == null || !(bool) (NetFieldBase<bool, NetBool>) Game1.player.isCustomized)
        return;
      if (!Game1.splitscreenOptions.ContainsKey(Game1.player.UniqueMultiplayerID))
        Game1.splitscreenOptions.Add(Game1.player.UniqueMultiplayerID, Game1.options);
      else
        Game1.splitscreenOptions[(long) Game1.player.uniqueMultiplayerID] = Game1.options;
    }

    public delegate void StaticInstanceMethod(object staticVarsHolder);
  }
}
