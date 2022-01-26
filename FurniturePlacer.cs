// Decompiled with JetBrains decompiler
// Type: StardewValley.FurniturePlacer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System.Collections.Generic;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley
{
  internal class FurniturePlacer
  {
    public static void addAllFurnitureOwnedByFarmer()
    {
      foreach (string furnitureName in Game1.player.furnitureOwned)
        FurniturePlacer.addFurniture(furnitureName);
    }

    public static void addFurniture(string furnitureName)
    {
      if (furnitureName.Equals("Television"))
      {
        GameLocation locationFromName = Game1.getLocationFromName("FarmHouse");
        locationFromName.Map.GetLayer("Buildings").Tiles[6, 3] = (Tile) new StaticTile(locationFromName.Map.GetLayer("Buildings"), locationFromName.Map.GetTileSheet("Farmhouse"), BlendMode.Alpha, 12);
        locationFromName.Map.GetLayer("Buildings").Tiles[6, 3].Properties.Add("Action", new PropertyValue("TV"));
        locationFromName.Map.GetLayer("Buildings").Tiles[7, 3] = (Tile) new StaticTile(locationFromName.Map.GetLayer("Buildings"), locationFromName.Map.GetTileSheet("Farmhouse"), BlendMode.Alpha, 13);
        locationFromName.Map.GetLayer("Buildings").Tiles[7, 3].Properties.Add("Action", new PropertyValue("TV"));
        locationFromName.Map.GetLayer("Buildings").Tiles[6, 2] = (Tile) new StaticTile(locationFromName.Map.GetLayer("Buildings"), locationFromName.Map.GetTileSheet("Farmhouse"), BlendMode.Alpha, 4);
        locationFromName.Map.GetLayer("Buildings").Tiles[7, 2] = (Tile) new StaticTile(locationFromName.Map.GetLayer("Buildings"), locationFromName.Map.GetTileSheet("Farmhouse"), BlendMode.Alpha, 5);
      }
      else if (furnitureName.Equals("Incubator"))
      {
        GameLocation locationFromName = Game1.getLocationFromName("Coop");
        locationFromName.map.GetLayer("Buildings").Tiles[1, 3] = (Tile) new StaticTile(locationFromName.map.GetLayer("Buildings"), locationFromName.map.TileSheets[0], BlendMode.Alpha, 44);
        locationFromName.map.GetLayer("Buildings").Tiles[1, 3].Properties.Add(new KeyValuePair<string, PropertyValue>("Action", new PropertyValue("Incubator")));
        locationFromName.map.GetLayer("Front").Tiles[1, 2] = (Tile) new StaticTile(locationFromName.map.GetLayer("Front"), locationFromName.map.TileSheets[0], BlendMode.Alpha, 45);
      }
      if (Game1.player.furnitureOwned.Contains(furnitureName))
        return;
      Game1.player.furnitureOwned.Add(furnitureName);
    }
  }
}
