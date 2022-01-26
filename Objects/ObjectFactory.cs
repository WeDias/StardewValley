// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.ObjectFactory
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Tools;
using System;

namespace StardewValley.Objects
{
  public class ObjectFactory
  {
    public const byte regularObject = 0;
    public const byte bigCraftable = 1;
    public const byte weapon = 2;
    public const byte specialItem = 3;
    public const byte regularObjectRecipe = 4;
    public const byte bigCraftableRecipe = 5;

    public static ItemDescription getDescriptionFromItem(Item i)
    {
      switch (i)
      {
        case StardewValley.Object _ when (bool) (NetFieldBase<bool, NetBool>) (i as StardewValley.Object).bigCraftable:
          return new ItemDescription((byte) 1, (i as StardewValley.Object).ParentSheetIndex, i.Stack);
        case StardewValley.Object _:
          return new ItemDescription((byte) 0, (i as StardewValley.Object).ParentSheetIndex, i.Stack);
        case MeleeWeapon _:
          return new ItemDescription((byte) 2, (i as MeleeWeapon).CurrentParentTileIndex, i.Stack);
        default:
          throw new Exception("ItemFactory trying to create item description from unknown item");
      }
    }

    public static Item getItemFromDescription(byte type, int index, int stack)
    {
      switch (type)
      {
        case 0:
          return (Item) new StardewValley.Object(Vector2.Zero, index, stack);
        case 1:
          return (Item) new StardewValley.Object(Vector2.Zero, index);
        case 2:
          return (Item) new MeleeWeapon(index);
        case 4:
          return (Item) new StardewValley.Object(index, stack, true);
        case 5:
          return (Item) new StardewValley.Object(Vector2.Zero, index, true);
        default:
          throw new Exception("ItemFactory trying to create item from unknown description");
      }
    }
  }
}
