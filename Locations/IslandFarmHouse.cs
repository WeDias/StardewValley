// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandFarmHouse
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class IslandFarmHouse : DecoratableLocation
  {
    [XmlElement("fridge")]
    public readonly NetRef<Chest> fridge = new NetRef<Chest>(new Chest(true));
    public Point fridgePosition;
    public NetBool visited = new NetBool(false);

    public IslandFarmHouse()
    {
    }

    public IslandFarmHouse(string map, string name)
      : base(map, name)
    {
      this.furniture.Add(new Furniture(1798, new Vector2(12f, 8f)));
      this.furniture.Add(new Furniture(1614, new Vector2(3f, 1f)));
      this.furniture.Add(new Furniture(1614, new Vector2(8f, 1f)));
      this.furniture.Add(new Furniture(1614, new Vector2(20f, 1f)));
      this.furniture.Add(new Furniture(1614, new Vector2(25f, 1f)));
      this.furniture.Add(new Furniture(1294, new Vector2(1f, 4f)));
      this.furniture.Add(new Furniture(1294, new Vector2(10f, 4f)));
      this.furniture.Add(new Furniture(1294, new Vector2(18f, 4f)));
      this.furniture.Add(new Furniture(1294, new Vector2(28f, 4f)));
      this.furniture.Add(new Furniture(1742, new Vector2(20f, 4f)));
      this.furniture.Add(new Furniture(1755, new Vector2(14f, 9f)));
      this.ReadWallpaperAndFloorTileData();
      this.setWallpaper(88, 0, true);
      this.setFloor(23, 0, true);
      this.setWallpaper(88, 1, true);
      this.setFloor(48, 1, true);
      this.setWallpaper(87, 2, true);
      this.setFloor(52, 2, true);
      this.setWallpaper(87, 3, true);
      this.setFloor(23, 3, true);
      this.setWallpaper(87, 4, true);
      this.fridgePosition = new Point();
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      this.fridge.Value = (l as IslandFarmHouse).fridge.Value;
      this.visited.Value = (l as IslandFarmHouse).visited.Value;
      base.TransferDataFromSavedLocation(l);
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (action != null && who.IsLocalPlayer)
      {
        string str = action.Split(' ')[0];
        if (!(str == "kitchen"))
        {
          if (str == "drawer")
            return this.performAction("kitchen", who, tileLocation);
        }
        else
        {
          this.ActivateKitchen(this.fridge);
          return true;
        }
      }
      return base.performAction(action, who, tileLocation);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this.fridge.Value.updateWhenCurrentLocation(time, (GameLocation) this);
    }

    public override void DayUpdate(int dayOfMonth) => base.DayUpdate(dayOfMonth);

    public override List<Microsoft.Xna.Framework.Rectangle> getWalls() => new List<Microsoft.Xna.Framework.Rectangle>()
    {
      new Microsoft.Xna.Framework.Rectangle(1, 1, 10, 3),
      new Microsoft.Xna.Framework.Rectangle(18, 1, 11, 3),
      new Microsoft.Xna.Framework.Rectangle(12, 5, 5, 2),
      new Microsoft.Xna.Framework.Rectangle(17, 9, 2, 2),
      new Microsoft.Xna.Framework.Rectangle(21, 9, 8, 2)
    };

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (!this.visited.Value)
        this.visited.Value = true;
      bool flag = false;
      for (int x = 0; x < this.map.GetLayer("Buildings").LayerWidth; ++x)
      {
        for (int y = 0; y < this.map.GetLayer("Buildings").LayerHeight; ++y)
        {
          if (this.map.GetLayer("Buildings").Tiles[x, y] != null && this.map.GetLayer("Buildings").Tiles[x, y].TileIndex == 258)
          {
            this.fridgePosition = new Point(x, y);
            flag = true;
            break;
          }
        }
        if (flag)
          break;
      }
    }

    public override List<Microsoft.Xna.Framework.Rectangle> getFloors() => new List<Microsoft.Xna.Framework.Rectangle>()
    {
      new Microsoft.Xna.Framework.Rectangle(1, 3, 11, 12),
      new Microsoft.Xna.Framework.Rectangle(11, 7, 6, 9),
      new Microsoft.Xna.Framework.Rectangle(18, 3, 11, 6),
      new Microsoft.Xna.Framework.Rectangle(17, 11, 12, 6)
    };

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.fridge, (INetSerializable) this.visited);
      this.visited.InterpolationEnabled = false;
      this.visited.fieldChangeVisibleEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((a, b, c) => this.InitializeBeds());
    }

    public virtual void InitializeBeds()
    {
      if (!Game1.IsMasterGame || Game1.gameMode == (byte) 6 || !this.visited.Value)
        return;
      int num1 = 0;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
        ++num1;
      int which = 2176;
      this.furniture.Add((Furniture) new BedFurniture(which, new Vector2(22f, 3f)));
      int val2 = num1 - 1;
      if (val2 > 0)
      {
        this.furniture.Add((Furniture) new BedFurniture(which, new Vector2(26f, 3f)));
        --val2;
      }
      for (int index = 0; index < Math.Min(6, val2); ++index)
      {
        int x = 3;
        int num2 = 3;
        if (index % 2 == 0)
          x += 4;
        int y = num2 + index / 2 * 4;
        this.furniture.Add((Furniture) new BedFurniture(which, new Vector2((float) x, (float) y)));
      }
    }

    public override void drawAboveFrontLayer(SpriteBatch b)
    {
      base.drawAboveFrontLayer(b);
      if (!this.fridge.Value.mutex.IsLocked())
        return;
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) this.fridgePosition.X, (float) (this.fridgePosition.Y - 1)) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 192, 16, 32)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((this.fridgePosition.Y + 1) * 64 + 1) / 10000f);
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.map.GetLayer("Buildings").Tiles[tileLocation] == null || this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex != 258)
        return base.checkAction(tileLocation, viewport, who);
      this.fridge.Value.fridge.Value = true;
      this.fridge.Value.checkForAction(who, false);
      return true;
    }
  }
}
