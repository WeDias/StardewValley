// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.MagnifyingGlass
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Menus;
using StardewValley.Network;
using System.Collections.Generic;

namespace StardewValley.Tools
{
  public class MagnifyingGlass : Tool
  {
    public MagnifyingGlass()
      : base("Magnifying Glass", -1, 5, 5, false)
    {
      this.InstantUse = true;
    }

    public override Item getOne()
    {
      MagnifyingGlass one = new MagnifyingGlass();
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:MagnifyingGlass.cs.14119");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:MagnifyingGlass.cs.14120");

    public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
    {
      who.Halt();
      who.canMove = true;
      who.UsingTool = false;
      this.DoFunction(location, Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y, 0, who);
      return true;
    }

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      base.DoFunction(location, x, y, power, who);
      this.CurrentParentTileIndex = 5;
      this.IndexOfMenuItemView = 5;
      Rectangle rectangle = new Rectangle(x / 64 * 64, y / 64 * 64, 64, 64);
      switch (location)
      {
        case Farm _:
          using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.PairsCollection.Enumerator enumerator = (location as Farm).animals.Pairs.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<long, FarmAnimal> current = enumerator.Current;
              if (current.Value.GetBoundingBox().Intersects(rectangle))
              {
                Game1.activeClickableMenu = (IClickableMenu) new AnimalQueryMenu(current.Value);
                break;
              }
            }
            break;
          }
        case AnimalHouse _:
          using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.PairsCollection.Enumerator enumerator = (location as AnimalHouse).animals.Pairs.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<long, FarmAnimal> current = enumerator.Current;
              if (current.Value.GetBoundingBox().Intersects(rectangle))
              {
                Game1.activeClickableMenu = (IClickableMenu) new AnimalQueryMenu(current.Value);
                break;
              }
            }
            break;
          }
      }
    }
  }
}
