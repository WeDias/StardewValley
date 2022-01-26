// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.FarmHouse
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley.Locations
{
  public class FarmHouse : DecoratableLocation
  {
    public int farmerNumberOfOwner;
    [XmlElement("fireplaceOn")]
    public readonly NetBool fireplaceOn = new NetBool();
    [XmlElement("fridge")]
    public readonly NetRef<Chest> fridge = new NetRef<Chest>(new Chest(true));
    [XmlIgnore]
    public readonly NetInt synchronizedDisplayedLevel = new NetInt(-1);
    public Point fridgePosition;
    [XmlIgnore]
    public Point spouseRoomSpot;
    [XmlIgnore]
    private LocalizedContentManager mapLoader;
    public List<Warp> cellarWarps;
    [XmlElement("cribStyle")]
    public readonly NetInt cribStyle = new NetInt(1);
    [XmlIgnore]
    public int previousUpgradeLevel = -1;
    private int currentlyDisplayedUpgradeLevel;
    private bool displayingSpouseRoom;

    [XmlIgnore]
    public virtual Farmer owner => Game1.MasterPlayer;

    [XmlIgnore]
    public virtual int upgradeLevel
    {
      get => this.owner == null ? 0 : (int) (NetFieldBase<int, NetInt>) this.owner.houseUpgradeLevel;
      set
      {
        if (this.owner == null)
          return;
        this.owner.houseUpgradeLevel.Value = value;
      }
    }

    public FarmHouse()
    {
    }

    public FarmHouse(int ownerNumber = 1) => this.farmerNumberOfOwner = ownerNumber;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.fireplaceOn, (INetSerializable) this.fridge, (INetSerializable) this.cribStyle, (INetSerializable) this.synchronizedDisplayedLevel);
      this.fireplaceOn.fieldChangeVisibleEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((field, oldValue, newValue) =>
      {
        Point fireplacePoint = this.getFireplacePoint();
        this.setFireplace(newValue, fireplacePoint.X, fireplacePoint.Y);
      });
      this.cribStyle.InterpolationEnabled = false;
      this.cribStyle.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((field, old_value, new_value) =>
      {
        if (this.map == null)
          return;
        if (this._appliedMapOverrides != null && this._appliedMapOverrides.Contains("crib"))
          this._appliedMapOverrides.Remove("crib");
        this.UpdateChildRoom();
        this.ReadWallpaperAndFloorTileData();
        this.setWallpapers();
        this.setFloors();
      });
    }

    public List<Child> getChildren() => this.characters.Where<NPC>((Func<NPC, bool>) (n => n is Child)).Select<NPC, Child>((Func<NPC, Child>) (n => n as Child)).ToList<Child>();

    public int getChildrenCount()
    {
      int childrenCount = 0;
      foreach (NPC character in this.characters)
      {
        if (character is Child)
          ++childrenCount;
      }
      return childrenCount;
    }

    public override bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character,
      bool pathfinding,
      bool projectile = false,
      bool ignoreCharacterRequirement = false)
    {
      return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding);
    }

    public override bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character)
    {
      return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
    }

    public override bool isTileLocationTotallyClearAndPlaceable(Vector2 v) => base.isTileLocationTotallyClearAndPlaceable(v);

    public override void performTenMinuteUpdate(int timeOfDay)
    {
      base.performTenMinuteUpdate(timeOfDay);
      foreach (NPC character in this.characters)
      {
        if (character.isMarried())
        {
          if (character.getSpouse() == Game1.player)
            character.checkForMarriageDialogue(timeOfDay, (GameLocation) this);
          if (Game1.IsMasterGame && Game1.timeOfDay >= 2200 && Game1.IsMasterGame && character.getTileLocationPoint() != this.getSpouseBedSpot(character.Name) && (timeOfDay == 2200 || character.controller == null && timeOfDay % 100 % 30 == 0))
          {
            Point spouseBedSpot = this.getSpouseBedSpot(character.Name);
            character.controller = (PathFindController) null;
            PathFindController.endBehavior endBehaviorFunction = (PathFindController.endBehavior) null;
            bool flag = this.GetSpouseBed() != null;
            if (flag)
              endBehaviorFunction = new PathFindController.endBehavior(FarmHouse.spouseSleepEndFunction);
            character.controller = new PathFindController((Character) character, (GameLocation) this, spouseBedSpot, 0, endBehaviorFunction);
            if (character.controller.pathToEndPoint == null || !this.isTileOnMap(character.controller.pathToEndPoint.Last<Point>().X, character.controller.pathToEndPoint.Last<Point>().Y))
              character.controller = (PathFindController) null;
            else if (flag)
            {
              foreach (Furniture furniture in this.furniture)
              {
                if (furniture is BedFurniture && furniture.getBoundingBox(furniture.TileLocation).Intersects(new Microsoft.Xna.Framework.Rectangle(spouseBedSpot.X * 64, spouseBedSpot.Y * 64, 64, 64)))
                {
                  (furniture as BedFurniture).ReserveForNPC();
                  break;
                }
              }
            }
          }
        }
        if (character is Child)
          (character as Child).tenMinuteUpdate();
      }
    }

    public static void spouseSleepEndFunction(Character c, GameLocation location)
    {
      if (c == null || !(c is NPC))
        return;
      if (Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions").ContainsKey(c.name.Value.ToLower() + "_sleep"))
        (c as NPC).playSleepingAnimation();
      foreach (Furniture furniture in location.furniture)
      {
        if (furniture is BedFurniture && furniture.getBoundingBox(furniture.TileLocation).Intersects(c.GetBoundingBox()))
        {
          (furniture as BedFurniture).ReserveForNPC();
          break;
        }
      }
    }

    public virtual Point getFrontDoorSpot()
    {
      foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) this.warps)
      {
        if (warp.TargetName == "Farm")
        {
          if (this is Cabin)
            return new Point(warp.TargetX, warp.TargetY);
          return warp.TargetX == 64 && warp.TargetY == 15 ? Game1.getFarm().GetMainFarmHouseEntry() : new Point(warp.TargetX, warp.TargetY);
        }
      }
      return Game1.getFarm().GetMainFarmHouseEntry();
    }

    public virtual Point getPorchStandingSpot()
    {
      switch (this.farmerNumberOfOwner)
      {
        case 0:
        case 1:
          Point mainFarmHouseEntry = Game1.getFarm().GetMainFarmHouseEntry();
          mainFarmHouseEntry.X += 2;
          return mainFarmHouseEntry;
        default:
          return new Point(-1000, -1000);
      }
    }

    public Point getKitchenStandingSpot()
    {
      switch (this.upgradeLevel)
      {
        case 1:
          return this.GetMapPropertyPosition("KitchenStandingLocation", 4, 5);
        case 2:
        case 3:
          return this.GetMapPropertyPosition("KitchenStandingLocation", 7, 14);
        default:
          return this.GetMapPropertyPosition("KitchenStandingLocation", -1000, -1000);
      }
    }

    public virtual BedFurniture GetSpouseBed()
    {
      if (this.owner.getSpouse() != null && this.owner.getSpouse().Name == "Krobus")
        return (BedFurniture) null;
      return this.owner != null && this.owner.hasCurrentOrPendingRoommate() && this.GetBed(BedFurniture.BedType.Single) != null ? this.GetBed(BedFurniture.BedType.Single) : this.GetBed(BedFurniture.BedType.Double);
    }

    public Point getSpouseBedSpot(string spouseName)
    {
      if (spouseName == "Krobus" && SocialPage.isRoommateOfAnyone(spouseName) || this.GetSpouseBed() == null)
        return this.GetSpouseRoomSpot();
      BedFurniture spouseBed = this.GetSpouseBed();
      Point bedSpot = this.GetSpouseBed().GetBedSpot();
      if (spouseBed.bedType == BedFurniture.BedType.Double)
        ++bedSpot.X;
      return bedSpot;
    }

    public Point GetSpouseRoomSpot() => this.upgradeLevel == 0 ? new Point(-1000, -1000) : this.spouseRoomSpot;

    public BedFurniture GetBed(BedFurniture.BedType bed_type = BedFurniture.BedType.Any, int index = 0)
    {
      foreach (Furniture furniture in this.furniture)
      {
        if (furniture is BedFurniture)
        {
          BedFurniture bed = furniture as BedFurniture;
          if (bed_type == BedFurniture.BedType.Any || bed.bedType == bed_type)
          {
            if (index == 0)
              return bed;
            --index;
          }
        }
      }
      return (BedFurniture) null;
    }

    public Point GetPlayerBedSpot()
    {
      BedFurniture playerBed = this.GetPlayerBed();
      return playerBed != null ? playerBed.GetBedSpot() : this.getEntryLocation();
    }

    public BedFurniture GetPlayerBed() => this.upgradeLevel == 0 ? this.GetBed(BedFurniture.BedType.Single) : this.GetBed(BedFurniture.BedType.Double);

    public Point getBedSpot(BedFurniture.BedType bed_type = BedFurniture.BedType.Any)
    {
      BedFurniture bed = this.GetBed(bed_type);
      return bed != null ? bed.GetBedSpot() : new Point(-1000, -1000);
    }

    public Point getEntryLocation()
    {
      switch (this.upgradeLevel)
      {
        case 0:
          return this.GetMapPropertyPosition("EntryLocation", 3, 11);
        case 1:
          return this.GetMapPropertyPosition("EntryLocation", 9, 11);
        case 2:
        case 3:
          return this.GetMapPropertyPosition("EntryLocation", 12, 20);
        default:
          return new Point(-1000, -1000);
      }
    }

    public BedFurniture GetChildBed(int index) => this.GetBed(BedFurniture.BedType.Child, index);

    public Point GetChildBedSpot(int index)
    {
      BedFurniture childBed = this.GetChildBed(index);
      return childBed != null ? childBed.GetBedSpot() : Point.Zero;
    }

    public override bool isTilePlaceable(Vector2 v, Item item = null) => (!this.isTileOnMap(v) || this.getTileIndexAt((int) v.X, (int) v.Y, "Back") != 0 || !(this.getTileSheetIDAt((int) v.X, (int) v.Y, "Back") == "indoor")) && base.isTilePlaceable(v, item);

    public Point getRandomOpenPointInHouse(Random r, int buffer = 0, int tries = 30)
    {
      Point openPointInHouse = Point.Zero;
      for (int index = 0; index < tries; ++index)
      {
        openPointInHouse = new Point(r.Next(this.map.Layers[0].LayerWidth), r.Next(this.map.Layers[0].LayerHeight));
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(openPointInHouse.X - buffer, openPointInHouse.Y - buffer, 1 + buffer * 2, 1 + buffer * 2);
        bool flag = false;
        for (int x = rectangle.X; x < rectangle.Right; ++x)
        {
          for (int y = rectangle.Y; y < rectangle.Bottom; ++y)
          {
            flag = this.getTileIndexAt(x, y, "Back") == -1 || !this.isTileLocationTotallyClearAndPlaceable(x, y) || this.isTileOnWall(x, y);
            if (this.getTileIndexAt(x, y, "Back") == 0 && this.getTileSheetIDAt(x, y, "Back") == "indoor")
              flag = true;
            if (flag)
              break;
          }
          if (flag)
            break;
        }
        if (!flag)
          return openPointInHouse;
      }
      return Point.Zero;
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (action == null || !who.IsLocalPlayer || !(action.Split(' ')[0] == "kitchen"))
        return base.performAction(action, who, tileLocation);
      this.ActivateKitchen(this.fridge);
      return true;
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.map.GetLayer("Buildings").Tiles[tileLocation] != null)
      {
        switch (this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex)
        {
          case 173:
            this.fridge.Value.fridge.Value = true;
            this.fridge.Value.checkForAction(who, false);
            return true;
          case 794:
          case 795:
          case 796:
          case 797:
            this.fireplaceOn.Value = !(bool) (NetFieldBase<bool, NetBool>) this.fireplaceOn;
            return true;
          case 2173:
            if (Game1.player.eventsSeen.Contains(463391) && Game1.player.spouse != null && Game1.player.spouse.Equals("Emily"))
            {
              TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(5858585);
              if (temporarySpriteById != null && temporarySpriteById is EmilysParrot)
                (temporarySpriteById as EmilysParrot).doAction();
            }
            return true;
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public FarmHouse(string m, string name)
      : base(m, name)
    {
      this.ReadWallpaperAndFloorTileData();
      this.furniture.Add((Furniture) new BedFurniture(BedFurniture.DEFAULT_BED_INDEX, new Vector2(9f, 8f)));
      if (Game1.getFarm().getMapProperty("FarmHouseFurniture") != "")
      {
        int result1;
        if (!int.TryParse(Game1.getFarm().getMapProperty("FarmHouseWallpaper"), out result1))
          result1 = -1;
        if (result1 >= 0)
          this.setWallpaper(result1, persist: true);
        if (!int.TryParse(Game1.getFarm().getMapProperty("FarmHouseFlooring"), out result1))
          result1 = -1;
        if (result1 >= 0)
          this.setFloor(result1, persist: true);
        if (Game1.getFarm().getMapProperty("FarmHouseStarterSeedsPosition") != "")
        {
          int result2 = 3;
          int result3 = 7;
          string[] strArray = Game1.getFarm().getMapProperty("FarmHouseStarterSeedsPosition").Split(' ');
          if (strArray.Length == 2)
          {
            if (!int.TryParse(strArray[0], out result2))
              result2 = 3;
            if (!int.TryParse(strArray[1], out result3))
              result3 = 7;
          }
          this.objects.Add(new Vector2((float) result2, (float) result3), (StardewValley.Object) new Chest(0, new List<Item>()
          {
            (Item) new StardewValley.Object(472, 15)
          }, new Vector2((float) result2, (float) result3), true));
        }
        else
          this.objects.Add(new Vector2(3f, 7f), (StardewValley.Object) new Chest(0, new List<Item>()
          {
            (Item) new StardewValley.Object(472, 15)
          }, new Vector2(3f, 7f), true));
        string[] strArray1 = Game1.getFarm().getMapProperty("FarmHouseFurniture").Split(' ');
        for (int index = 0; index < strArray1.Length; index += 4)
        {
          int result4 = -1;
          int result5 = 0;
          if (!int.TryParse(strArray1[index], out result4))
            result4 = -1;
          int result6;
          if (!int.TryParse(strArray1[index + 1], out result6))
            result4 = -1;
          int result7;
          if (!int.TryParse(strArray1[index + 2], out result7))
            result4 = -1;
          if (!int.TryParse(strArray1[index + 3], out result5))
            result4 = -1;
          if (result4 >= 0)
          {
            Furniture furnitureAt = this.GetFurnitureAt(new Vector2((float) result6, (float) result7));
            if (furnitureAt != null)
              furnitureAt.heldObject.Value = (StardewValley.Object) new Furniture(result4, new Vector2((float) result6, (float) result7), result5);
            else
              this.furniture.Add(new Furniture(result4, new Vector2((float) result6, (float) result7), result5));
          }
        }
      }
      else
      {
        switch (Game1.whichFarm)
        {
          case 0:
            this.furniture.Add(new Furniture(1120, new Vector2(5f, 4f)));
            this.furniture.Last<Furniture>().heldObject.Value = (StardewValley.Object) new Furniture(1364, new Vector2(5f, 4f));
            this.furniture.Add(new Furniture(1376, new Vector2(1f, 10f)));
            this.furniture.Add(new Furniture(0, new Vector2(4f, 4f)));
            this.furniture.Add((Furniture) new TV(1466, new Vector2(1f, 4f)));
            this.furniture.Add(new Furniture(1614, new Vector2(3f, 1f)));
            this.furniture.Add(new Furniture(1618, new Vector2(6f, 8f)));
            this.furniture.Add(new Furniture(1602, new Vector2(5f, 1f)));
            this.furniture.Add(new Furniture(1792, Utility.PointToVector2(this.getFireplacePoint())));
            this.objects.Add(new Vector2(3f, 7f), (StardewValley.Object) new Chest(0, new List<Item>()
            {
              (Item) new StardewValley.Object(472, 15)
            }, new Vector2(3f, 7f), true));
            break;
          case 1:
            this.setWallpaper(11, persist: true);
            this.setFloor(1, persist: true);
            this.furniture.Add(new Furniture(1122, new Vector2(1f, 6f)));
            this.furniture.Last<Furniture>().heldObject.Value = (StardewValley.Object) new Furniture(1367, new Vector2(1f, 6f));
            this.furniture.Add(new Furniture(3, new Vector2(1f, 5f)));
            this.furniture.Add((Furniture) new TV(1680, new Vector2(5f, 4f)));
            this.furniture.Add(new Furniture(1673, new Vector2(1f, 1f)));
            this.furniture.Add(new Furniture(1673, new Vector2(3f, 1f)));
            this.furniture.Add(new Furniture(1676, new Vector2(5f, 1f)));
            this.furniture.Add(new Furniture(1737, new Vector2(6f, 8f)));
            this.furniture.Add(new Furniture(1742, new Vector2(5f, 5f)));
            this.furniture.Add(new Furniture(1792, Utility.PointToVector2(this.getFireplacePoint())));
            this.furniture.Add(new Furniture(1675, new Vector2(10f, 1f)));
            this.objects.Add(new Vector2(4f, 7f), (StardewValley.Object) new Chest(0, new List<Item>()
            {
              (Item) new StardewValley.Object(472, 15)
            }, new Vector2(4f, 7f), true));
            break;
          case 2:
            this.setWallpaper(92, persist: true);
            this.setFloor(34, persist: true);
            this.furniture.Add(new Furniture(1134, new Vector2(1f, 7f)));
            this.furniture.Last<Furniture>().heldObject.Value = (StardewValley.Object) new Furniture(1748, new Vector2(1f, 7f));
            this.furniture.Add(new Furniture(3, new Vector2(1f, 6f)));
            this.furniture.Add((Furniture) new TV(1680, new Vector2(6f, 4f)));
            this.furniture.Add(new Furniture(1296, new Vector2(1f, 4f)));
            this.furniture.Add(new Furniture(1682, new Vector2(3f, 1f)));
            this.furniture.Add(new Furniture(1777, new Vector2(6f, 5f)));
            this.furniture.Add(new Furniture(1745, new Vector2(6f, 1f)));
            this.furniture.Add(new Furniture(1792, Utility.PointToVector2(this.getFireplacePoint())));
            this.furniture.Add(new Furniture(1747, new Vector2(5f, 4f)));
            this.furniture.Add(new Furniture(1296, new Vector2(10f, 4f)));
            this.objects.Add(new Vector2(4f, 7f), (StardewValley.Object) new Chest(0, new List<Item>()
            {
              (Item) new StardewValley.Object(472, 15)
            }, new Vector2(4f, 7f), true));
            break;
          case 3:
            this.setWallpaper(12, persist: true);
            this.setFloor(18, persist: true);
            this.furniture.Add(new Furniture(1218, new Vector2(1f, 6f)));
            this.furniture.Last<Furniture>().heldObject.Value = (StardewValley.Object) new Furniture(1368, new Vector2(1f, 6f));
            this.furniture.Add(new Furniture(1755, new Vector2(1f, 5f)));
            this.furniture.Add(new Furniture(1755, new Vector2(3f, 6f), 1));
            this.furniture.Add((Furniture) new TV(1680, new Vector2(5f, 4f)));
            this.furniture.Add(new Furniture(1751, new Vector2(5f, 10f)));
            this.furniture.Add(new Furniture(1749, new Vector2(3f, 1f)));
            this.furniture.Add(new Furniture(1753, new Vector2(5f, 1f)));
            this.furniture.Add(new Furniture(1742, new Vector2(5f, 5f)));
            this.objects.Add(new Vector2(2f, 9f), (StardewValley.Object) new Chest(0, new List<Item>()
            {
              (Item) new StardewValley.Object(472, 15)
            }, new Vector2(2f, 9f), true));
            this.furniture.Add(new Furniture(1794, Utility.PointToVector2(this.getFireplacePoint())));
            break;
          case 4:
            this.setWallpaper(95, persist: true);
            this.setFloor(4, persist: true);
            this.furniture.Add((Furniture) new TV(1680, new Vector2(1f, 4f)));
            this.furniture.Add(new Furniture(1628, new Vector2(1f, 5f)));
            this.furniture.Add(new Furniture(1393, new Vector2(3f, 4f)));
            this.furniture.Last<Furniture>().heldObject.Value = (StardewValley.Object) new Furniture(1369, new Vector2(3f, 4f));
            this.furniture.Add(new Furniture(1678, new Vector2(10f, 1f)));
            this.furniture.Add(new Furniture(1812, new Vector2(3f, 1f)));
            this.furniture.Add(new Furniture(1630, new Vector2(1f, 1f)));
            this.furniture.Add(new Furniture(1794, Utility.PointToVector2(this.getFireplacePoint())));
            this.furniture.Add(new Furniture(1811, new Vector2(6f, 1f)));
            this.furniture.Add(new Furniture(1389, new Vector2(10f, 4f)));
            this.objects.Add(new Vector2(4f, 7f), (StardewValley.Object) new Chest(0, new List<Item>()
            {
              (Item) new StardewValley.Object(472, 15)
            }, new Vector2(4f, 7f), true));
            this.furniture.Add(new Furniture(1758, new Vector2(1f, 10f)));
            break;
          case 5:
            this.setWallpaper(65, persist: true);
            this.setFloor(5, persist: true);
            this.furniture.Add((Furniture) new TV(1466, new Vector2(1f, 4f)));
            this.furniture.Add(new Furniture(1792, Utility.PointToVector2(this.getFireplacePoint())));
            this.furniture.Add(new Furniture(1614, new Vector2(3f, 1f)));
            this.furniture.Add(new Furniture(1614, new Vector2(6f, 1f)));
            this.furniture.Add(new Furniture(1601, new Vector2(10f, 1f)));
            this.furniture.Add(new Furniture(202, new Vector2(3f, 4f), 1));
            this.furniture.Add(new Furniture(1124, new Vector2(4f, 4f), 1));
            this.furniture.Last<Furniture>().heldObject.Value = (StardewValley.Object) new Furniture(1379, new Vector2(5f, 4f));
            this.furniture.Add(new Furniture(202, new Vector2(6f, 4f), 3));
            this.furniture.Add(new Furniture(1378, new Vector2(10f, 4f)));
            this.furniture.Add(new Furniture(1377, new Vector2(1f, 9f)));
            this.furniture.Add(new Furniture(1445, new Vector2(1f, 10f)));
            this.furniture.Add(new Furniture(1618, new Vector2(2f, 9f)));
            this.objects.Add(new Vector2(3f, 7f), (StardewValley.Object) new Chest(0, new List<Item>()
            {
              (Item) new StardewValley.Object(472, 15)
            }, new Vector2(3f, 7f), true));
            break;
          case 6:
            this.setWallpaper(106, persist: true);
            this.setFloor(35, persist: true);
            this.furniture.Add((Furniture) new TV(1680, new Vector2(4f, 4f)));
            this.furniture.Add(new Furniture(1614, new Vector2(7f, 1f)));
            this.furniture.Add(new Furniture(1294, new Vector2(3f, 4f)));
            this.furniture.Add(new Furniture(1283, new Vector2(1f, 4f)));
            this.furniture.Add(new Furniture(1614, new Vector2(8f, 1f)));
            this.furniture.Add(new Furniture(202, new Vector2(7f, 4f)));
            this.furniture.Add(new Furniture(1294, new Vector2(10f, 4f)));
            this.furniture.Add(new Furniture(6, new Vector2(2f, 6f), 1));
            this.furniture.Add(new Furniture(6, new Vector2(5f, 7f), 3));
            this.furniture.Add(new Furniture(1124, new Vector2(3f, 6f)));
            this.furniture.Last<Furniture>().heldObject.Value = (StardewValley.Object) new Furniture(1362, new Vector2(4f, 6f));
            this.objects.Add(new Vector2(8f, 6f), (StardewValley.Object) new Chest(0, new List<Item>()
            {
              (Item) new StardewValley.Object(472, 15)
            }, new Vector2(8f, 6f), true));
            this.furniture.Add(new Furniture(1228, new Vector2(2f, 9f)));
            break;
        }
      }
    }

    public bool hasActiveFireplace()
    {
      for (int index = 0; index < this.furniture.Count<Furniture>(); ++index)
      {
        if ((int) (NetFieldBase<int, NetInt>) this.furniture[index].furniture_type == 14 && (bool) (NetFieldBase<bool, NetBool>) this.furniture[index].isOn)
          return true;
      }
      return false;
    }

    public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
    {
      base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
      if (!Game1.IsMasterGame)
        return;
      foreach (NPC character in this.characters)
      {
        Farmer spouse = character.getSpouse();
        if (spouse != null && spouse == this.owner)
        {
          NPC c = character;
          if (c != null && Game1.timeOfDay < 1500 && Game1.random.NextDouble() < 0.0006 && c.controller == null && c.Schedule == null && !c.getTileLocation().Equals(Utility.PointToVector2(this.getSpouseBedSpot(Game1.player.spouse))) && this.furniture.Count > 0)
          {
            Furniture furniture = this.furniture[Game1.random.Next(this.furniture.Count)];
            Microsoft.Xna.Framework.Rectangle boundingBox = (Microsoft.Xna.Framework.Rectangle) (NetFieldBase<Microsoft.Xna.Framework.Rectangle, NetRectangle>) furniture.boundingBox;
            Vector2 v = new Vector2((float) (boundingBox.X / 64), (float) (boundingBox.Y / 64));
            if (furniture.furniture_type.Value != 15 && furniture.furniture_type.Value != 12)
            {
              int num1 = 0;
              int finalFacingDirection = -3;
              for (; num1 < 3; ++num1)
              {
                int num2 = Game1.random.Next(-1, 2);
                int num3 = Game1.random.Next(-1, 2);
                v.X += (float) num2;
                if (num2 == 0)
                  v.Y += (float) num3;
                if (num2 == -1)
                  finalFacingDirection = 1;
                else if (num2 == 1)
                {
                  finalFacingDirection = 3;
                }
                else
                {
                  switch (num3)
                  {
                    case -1:
                      finalFacingDirection = 2;
                      break;
                    case 1:
                      finalFacingDirection = 0;
                      break;
                  }
                }
                if (this.isTileLocationTotallyClearAndPlaceable(v))
                  break;
              }
              if (num1 < 3)
                c.controller = new PathFindController((Character) c, (GameLocation) this, new Point((int) v.X, (int) v.Y), finalFacingDirection, false, false);
            }
          }
        }
      }
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      if (this.wasUpdated)
        return;
      base.UpdateWhenCurrentLocation(time);
      this.fridge.Value.updateWhenCurrentLocation(time, (GameLocation) this);
      if (!Game1.player.isMarried() || Game1.player.spouse == null)
        return;
      NPC characterFromName = this.getCharacterFromName(Game1.player.spouse);
      if (characterFromName == null || characterFromName.isEmoting)
        return;
      Vector2 tileLocation1 = characterFromName.getTileLocation();
      foreach (Vector2 adjacentTilesOffset in Character.AdjacentTilesOffsets)
      {
        Vector2 tileLocation2 = tileLocation1 + adjacentTilesOffset;
        NPC npc = this.isCharacterAtTile(tileLocation2);
        if (npc != null && npc.IsMonster && !npc.Name.Equals("Cat"))
        {
          characterFromName.faceGeneralDirection(tileLocation2 * new Vector2(64f, 64f));
          Game1.showSwordswipeAnimation(characterFromName.FacingDirection, characterFromName.Position, 60f, false);
          this.localSound("swordswipe");
          characterFromName.shake(500);
          characterFromName.showTextAboveHead(Game1.content.LoadString("Strings\\Locations:FarmHouse_SpouseAttacked" + (Game1.random.Next(12) + 1).ToString()));
          ((Monster) npc).takeDamage(50, (int) Utility.getAwayFromPositionTrajectory(npc.GetBoundingBox(), characterFromName.Position).X, (int) Utility.getAwayFromPositionTrajectory(npc.GetBoundingBox(), characterFromName.Position).Y, false, 1.0, Game1.player);
          if (((Monster) npc).Health <= 0)
          {
            this.debris.Add(new Debris((string) (NetFieldBase<string, NetString>) npc.Sprite.textureName, Game1.random.Next(6, 16), new Vector2((float) npc.getStandingX(), (float) npc.getStandingY())));
            this.monsterDrop((Monster) npc, npc.getStandingX(), npc.getStandingY(), this.owner);
            this.characters.Remove(npc);
            ++Game1.stats.MonstersKilled;
            Game1.player.changeFriendship(-10, characterFromName);
          }
          else
            ((Monster) npc).shedChunks(4);
          characterFromName.CurrentDialogue.Clear();
          characterFromName.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Data\\ExtraDialogue:Spouse_MonstersInHouse"), characterFromName));
        }
      }
    }

    public Point getFireplacePoint()
    {
      switch (this.upgradeLevel)
      {
        case 0:
          return new Point(8, 4);
        case 1:
          return new Point(26, 4);
        case 2:
        case 3:
          return new Point(2, 13);
        default:
          return new Point(-50, -50);
      }
    }

    public bool shouldShowSpouseRoom() => this.owner.isMarried();

    public virtual void showSpouseRoom()
    {
      bool flag = this.owner.isMarried() && this.owner.spouse != null;
      int num = this.displayingSpouseRoom ? 1 : 0;
      this.displayingSpouseRoom = flag;
      this.updateMap();
      if (num != 0 && !this.displayingSpouseRoom)
      {
        Point spouseRoomCorner = this.GetSpouseRoomCorner();
        Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(spouseRoomCorner.X, spouseRoomCorner.Y, this.GetSpouseRoomWidth(), this.GetSpouseRoomHeight());
        --rectangle1.X;
        List<Item> overflow_items = new List<Item>();
        Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(rectangle1.X * 64, rectangle1.Y * 64, rectangle1.Width * 64, rectangle1.Height * 64);
        foreach (Furniture furniture in new List<Furniture>((IEnumerable<Furniture>) this.furniture))
        {
          if (furniture.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture.tileLocation).Intersects(rectangle2))
          {
            if (furniture is StorageFurniture)
            {
              StorageFurniture storageFurniture = furniture as StorageFurniture;
              overflow_items.AddRange((IEnumerable<Item>) storageFurniture.heldItems);
              storageFurniture.heldItems.Clear();
            }
            if (furniture.heldObject.Value != null)
            {
              overflow_items.Add((Item) (StardewValley.Object) (NetFieldBase<StardewValley.Object, NetRef<StardewValley.Object>>) furniture.heldObject);
              furniture.heldObject.Value = (StardewValley.Object) null;
            }
            overflow_items.Add((Item) furniture);
            this.furniture.Remove(furniture);
          }
        }
        for (int x = rectangle1.X; x <= rectangle1.Right; ++x)
        {
          for (int y = rectangle1.Y; y <= rectangle1.Bottom; ++y)
          {
            StardewValley.Object @object = this.getObjectAtTile(x, y);
            switch (@object)
            {
              case null:
              case Furniture _:
                continue;
              default:
                @object.performRemoveAction(new Vector2((float) x, (float) y), (GameLocation) this);
                if (@object is Fence)
                  @object = new StardewValley.Object(Vector2.Zero, (@object as Fence).GetItemParentSheetIndex(), 1);
                if (@object is IndoorPot)
                {
                  IndoorPot indoorPot = @object as IndoorPot;
                  if (indoorPot.hoeDirt.Value != null && indoorPot.hoeDirt.Value.crop != null)
                    indoorPot.hoeDirt.Value.destroyCrop((Vector2) (NetFieldBase<Vector2, NetVector2>) indoorPot.tileLocation, false, (GameLocation) this);
                }
                else if (@object is Chest)
                {
                  Chest chest = @object as Chest;
                  overflow_items.AddRange((IEnumerable<Item>) chest.items);
                  chest.items.Clear();
                }
                if ((NetFieldBase<StardewValley.Object, NetRef<StardewValley.Object>>) @object.heldObject != (NetRef<StardewValley.Object>) null)
                  @object.heldObject.Value = (StardewValley.Object) null;
                @object.minutesUntilReady.Value = -1;
                if (@object.readyForHarvest.Value)
                  @object.readyForHarvest.Value = false;
                overflow_items.Add((Item) @object);
                this.objects.Remove(new Vector2((float) x, (float) y));
                continue;
            }
          }
        }
        if (this.upgradeLevel >= 2)
          Utility.createOverflowChest((GameLocation) this, new Vector2(24f, 22f), overflow_items);
        else
          Utility.createOverflowChest((GameLocation) this, new Vector2(21f, 10f), overflow_items);
      }
      this.loadObjects();
      if (this.upgradeLevel == 3)
      {
        this.AddCellarTiles();
        this.createCellarWarps();
        if (!Game1.player.craftingRecipes.ContainsKey("Cask"))
          Game1.player.craftingRecipes.Add("Cask", 0);
      }
      if (!flag)
        return;
      this.loadSpouseRoom();
    }

    public virtual void AddCellarTiles()
    {
      if (this._appliedMapOverrides.Contains("cellar"))
        this._appliedMapOverrides.Remove("cellar");
      this.ApplyMapOverride("FarmHouse_Cellar", "cellar");
    }

    public string GetCellarName()
    {
      int num = -1;
      if (this.owner != null)
      {
        foreach (int key in Game1.player.team.cellarAssignments.Keys)
        {
          if (Game1.player.team.cellarAssignments[key] == this.owner.UniqueMultiplayerID)
            num = key;
        }
      }
      return num >= 0 && num <= 1 ? "Cellar" : "Cellar" + num.ToString();
    }

    protected override void resetSharedState()
    {
      base.resetSharedState();
      if (Game1.timeOfDay >= 2200 && this.owner.spouse != null && this.getCharacterFromName(this.owner.spouse) != null && !this.owner.isEngaged())
        Game1.player.team.requestSpouseSleepEvent.Fire(this.owner.UniqueMultiplayerID);
      if (Game1.timeOfDay >= 2000 && this.owner.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID && Game1.getFarm().farmers.Count <= 1)
        Game1.player.team.requestPetWarpHomeEvent.Fire(this.owner.UniqueMultiplayerID);
      if (!Game1.IsMasterGame)
        return;
      Farm farm = Game1.getFarm();
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        if (this.characters[index] is Pet && (!this.isTileOnMap(this.characters[index].getTileX(), this.characters[index].getTileY()) || this.getTileIndexAt(this.characters[index].GetBoundingBox().Left / 64, this.characters[index].getTileY(), "Buildings") != -1 || this.getTileIndexAt(this.characters[index].GetBoundingBox().Right / 64, this.characters[index].getTileY(), "Buildings") != -1))
        {
          this.characters[index].faceDirection(2);
          Game1.warpCharacter(this.characters[index], "Farm", farm.GetPetStartLocation());
          break;
        }
      }
      for (int index1 = this.characters.Count - 1; index1 >= 0; --index1)
      {
        for (int index2 = index1 - 1; index2 >= 0; --index2)
        {
          if (index1 < this.characters.Count && index2 < this.characters.Count && (this.characters[index2].Equals((object) this.characters[index1]) || this.characters[index2].Name.Equals(this.characters[index1].Name) && this.characters[index2].isVillager() && this.characters[index1].isVillager()) && index2 != index1)
            this.characters.RemoveAt(index2);
        }
        for (int index3 = farm.characters.Count - 1; index3 >= 0; --index3)
        {
          if (index1 < this.characters.Count && index3 < this.characters.Count && farm.characters[index3].Equals((object) this.characters[index1]))
            farm.characters.RemoveAt(index3);
        }
      }
    }

    public void UpdateForRenovation()
    {
      this.updateFarmLayout();
      this.setWallpapers();
      this.setFloors();
    }

    public void updateFarmLayout()
    {
      if (this.currentlyDisplayedUpgradeLevel != this.upgradeLevel)
        this.setMapForUpgradeLevel(this.upgradeLevel);
      this._ApplyRenovations();
      if (!this.displayingSpouseRoom && this.shouldShowSpouseRoom() || this.displayingSpouseRoom && !this.shouldShowSpouseRoom())
        this.showSpouseRoom();
      this.UpdateChildRoom();
      this.ReadWallpaperAndFloorTileData();
    }

    protected virtual void _ApplyRenovations()
    {
      if (this.upgradeLevel >= 2)
      {
        if (this._appliedMapOverrides.Contains("bedroom_open"))
          this._appliedMapOverrides.Remove("bedroom_open");
        if (this.owner.mailReceived.Contains("renovation_bedroom_open"))
          this.ApplyMapOverride("FarmHouse_Bedroom_Open", "bedroom_open");
        else
          this.ApplyMapOverride("FarmHouse_Bedroom_Normal", "bedroom_open");
        if (this._appliedMapOverrides.Contains("southernroom_open"))
          this._appliedMapOverrides.Remove("southernroom_open");
        if (this.owner.mailReceived.Contains("renovation_southern_open"))
          this.ApplyMapOverride("FarmHouse_SouthernRoom_Add", "southernroom_open");
        else
          this.ApplyMapOverride("FarmHouse_SouthernRoom_Remove", "southernroom_open");
        if (this._appliedMapOverrides.Contains("cornerroom_open"))
          this._appliedMapOverrides.Remove("cornerroom_open");
        if (this.owner.mailReceived.Contains("renovation_corner_open"))
        {
          this.ApplyMapOverride("FarmHouse_CornerRoom_Add", "cornerroom_open");
          if (this.displayingSpouseRoom)
            this.setMapTile(34, 9, 229, "Front", (string) null, 2);
        }
        else
        {
          this.ApplyMapOverride("FarmHouse_CornerRoom_Remove", "cornerroom_open");
          if (this.displayingSpouseRoom)
            this.setMapTile(34, 9, 87, "Front", (string) null, 2);
        }
      }
      if (!this.map.Properties.ContainsKey("AdditionalRenovations"))
        return;
      foreach (string str1 in this.map.Properties["AdditionalRenovations"].ToString().Split(','))
      {
        string[] strArray = str1.Trim().Split(' ');
        if (strArray.Length >= 4)
        {
          string override_key_name = strArray[0];
          string str2 = strArray[1];
          string map_name1 = strArray[2];
          string map_name2 = strArray[3];
          Microsoft.Xna.Framework.Rectangle? destination_rect = new Microsoft.Xna.Framework.Rectangle?();
          if (strArray.Length >= 8)
          {
            try
            {
              destination_rect = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle()
              {
                X = int.Parse(strArray[4]),
                Y = int.Parse(strArray[5]),
                Width = int.Parse(strArray[6]),
                Height = int.Parse(strArray[7])
              });
            }
            catch (Exception ex)
            {
              destination_rect = new Microsoft.Xna.Framework.Rectangle?();
            }
          }
          if (this._appliedMapOverrides.Contains(override_key_name))
            this._appliedMapOverrides.Remove(override_key_name);
          if (this.owner.mailReceived.Contains(str2))
            this.ApplyMapOverride(map_name1, override_key_name, destination_rect: destination_rect);
          else
            this.ApplyMapOverride(map_name2, override_key_name, destination_rect: destination_rect);
        }
      }
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      this.updateFarmLayout();
      this.setWallpapers();
      this.setFloors();
      if (this.owner.getSpouse() == null || !this.owner.getSpouse().name.Equals((object) "Sebastian") || !Game1.netWorldState.Value.hasWorldStateID("sebastianFrog"))
        return;
      Point spouseRoomCorner = this.GetSpouseRoomCorner();
      ++spouseRoomCorner.X;
      spouseRoomCorner.Y += 6;
      Vector2 vector2 = Utility.PointToVector2(spouseRoomCorner);
      this.removeTile((int) vector2.X, (int) vector2.Y - 1, "Front");
      this.removeTile((int) vector2.X + 1, (int) vector2.Y - 1, "Front");
      this.removeTile((int) vector2.X + 2, (int) vector2.Y - 1, "Front");
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (this.owner.isMarried() && this.owner.spouse != null && this.owner.spouse.Equals("Emily") && Game1.player.eventsSeen.Contains(463391))
      {
        Vector2 location = new Vector2(2064f, 160f);
        switch (this.upgradeLevel)
        {
          case 2:
          case 3:
            location = new Vector2(2448f, 736f);
            break;
        }
        this.temporarySprites.Add((TemporaryAnimatedSprite) new EmilysParrot(location));
      }
      if (Game1.player.currentLocation == null || !Game1.player.currentLocation.Equals((GameLocation) this) && !Game1.player.currentLocation.name.Value.StartsWith("Cellar"))
      {
        Game1.player.Position = Utility.PointToVector2(this.getEntryLocation()) * 64f;
        Game1.xLocationAfterWarp = Game1.player.getTileX();
        Game1.yLocationAfterWarp = Game1.player.getTileY();
        Game1.player.currentLocation = (GameLocation) this;
      }
      foreach (NPC character in this.characters)
      {
        if (character is Child)
          (character as Child).resetForPlayerEntry((GameLocation) this);
        if (Game1.IsMasterGame && Game1.timeOfDay >= 2000 && !(character is Pet))
        {
          character.controller = (PathFindController) null;
          character.Halt();
        }
      }
      if (this.owner == Game1.player && Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).HasValue && Game1.player.team.IsMarried(Game1.player.UniqueMultiplayerID) && !Game1.player.mailReceived.Contains("CF_Spouse"))
      {
        Vector2 vector2 = Utility.PointToVector2(this.getEntryLocation()) + new Vector2(0.0f, -1f);
        Chest chest = new Chest(0, new List<Item>()
        {
          (Item) new StardewValley.Object(434, 1)
        }, vector2, true, 1);
        this.overlayObjects[vector2] = (StardewValley.Object) chest;
      }
      if (this.owner != null && !this.owner.activeDialogueEvents.ContainsKey("pennyRedecorating") && !this.owner.mailReceived.Contains("pennyQuilt0") && !this.owner.mailReceived.Contains("pennyQuilt1"))
        this.owner.mailReceived.Contains("pennyQuilt2");
      if (this.owner.Equals((object) Game1.player) && !Game1.player.activeDialogueEvents.ContainsKey("pennyRedecorating"))
      {
        int whichStyle = -1;
        if (Game1.player.mailReceived.Contains("pennyQuilt0"))
          whichStyle = 0;
        else if (Game1.player.mailReceived.Contains("pennyQuilt1"))
          whichStyle = 1;
        else if (Game1.player.mailReceived.Contains("pennyQuilt2"))
          whichStyle = 2;
        if (whichStyle != -1 && !Game1.player.mailReceived.Contains("pennyRefurbished"))
        {
          List<StardewValley.Object> objectsToStoreInChests = new List<StardewValley.Object>();
          foreach (Furniture furniture in this.furniture)
          {
            if (furniture is BedFurniture)
            {
              BedFurniture bedFurniture = furniture as BedFurniture;
              if (bedFurniture.bedType == BedFurniture.BedType.Double)
              {
                int which = -1;
                if (this.owner.mailReceived.Contains("pennyQuilt0"))
                  which = 2058;
                if (this.owner.mailReceived.Contains("pennyQuilt1"))
                  which = 2064;
                if (this.owner.mailReceived.Contains("pennyQuilt2"))
                  which = 2070;
                if (which != -1)
                {
                  Vector2 tileLocation = bedFurniture.TileLocation;
                  bedFurniture.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) bedFurniture.tileLocation, (GameLocation) this);
                  objectsToStoreInChests.Add((StardewValley.Object) bedFurniture);
                  this.furniture.Remove(this.furniture.GuidOf((Furniture) bedFurniture));
                  this.furniture.Add((Furniture) new BedFurniture(which, new Vector2(tileLocation.X, tileLocation.Y)));
                  break;
                }
                break;
              }
            }
          }
          Game1.player.mailReceived.Add("pennyRefurbished");
          Microsoft.Xna.Framework.Rectangle rectangle = Microsoft.Xna.Framework.Rectangle.Empty;
          rectangle = this.upgradeLevel < 2 ? new Microsoft.Xna.Framework.Rectangle(20, 1, 8, 10) : new Microsoft.Xna.Framework.Rectangle(23, 10, 11, 13);
          for (int x = rectangle.X; x <= rectangle.Right; ++x)
          {
            for (int y = rectangle.Y; y <= rectangle.Bottom; ++y)
            {
              if (this.getObjectAtTile(x, y) != null)
              {
                StardewValley.Object object1 = this.getObjectAtTile(x, y);
                switch (object1)
                {
                  case null:
                  case Chest _:
                  case StorageFurniture _:
                  case IndoorPot _:
                  case BedFurniture _:
                    continue;
                  default:
                    if (object1.Name != null && object1.Name.Contains("Table") && object1.heldObject.Value != null)
                    {
                      StardewValley.Object object2 = object1.heldObject.Value;
                      object1.heldObject.Value = (StardewValley.Object) null;
                      objectsToStoreInChests.Add(object2);
                    }
                    object1.performRemoveAction(new Vector2((float) x, (float) y), (GameLocation) this);
                    if (object1 is Fence)
                      object1 = new StardewValley.Object(Vector2.Zero, (object1 as Fence).GetItemParentSheetIndex(), 1);
                    objectsToStoreInChests.Add(object1);
                    this.objects.Remove(new Vector2((float) x, (float) y));
                    if (object1 is Furniture)
                    {
                      this.furniture.Remove(object1 as Furniture);
                      continue;
                    }
                    continue;
                }
              }
            }
          }
          this.decoratePennyRoom(whichStyle, objectsToStoreInChests);
        }
      }
      if (this.owner.getSpouse() == null || !this.owner.getSpouse().name.Equals((object) "Sebastian") || !Game1.netWorldState.Value.hasWorldStateID("sebastianFrog"))
        return;
      Point spouseRoomCorner = this.GetSpouseRoomCorner();
      ++spouseRoomCorner.X;
      spouseRoomCorner.Y += 6;
      Vector2 vector2_1 = Utility.PointToVector2(spouseRoomCorner);
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = Game1.mouseCursors,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(641, 1534, 48, 37),
        animationLength = 1,
        sourceRectStartingPos = new Vector2(641f, 1534f),
        interval = 5000f,
        totalNumberOfLoops = 9999,
        position = vector2_1 * 64f + new Vector2(0.0f, -5f) * 4f,
        scale = 4f,
        layerDepth = (float) (((double) vector2_1.Y + 2.0 + 0.100000001490116) * 64.0 / 10000.0)
      });
      if (Game1.random.NextDouble() < 0.85)
      {
        Texture2D texture2D = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters");
        List<TemporaryAnimatedSprite> temporarySprites = this.TemporarySprites;
        SebsFrogs sebsFrogs = new SebsFrogs();
        sebsFrogs.texture = texture2D;
        sebsFrogs.sourceRect = new Microsoft.Xna.Framework.Rectangle(64, 224, 16, 16);
        sebsFrogs.animationLength = 1;
        sebsFrogs.sourceRectStartingPos = new Vector2(64f, 224f);
        sebsFrogs.interval = 100f;
        sebsFrogs.totalNumberOfLoops = 9999;
        sebsFrogs.position = vector2_1 * 64f + new Vector2(Game1.random.NextDouble() < 0.5 ? 22f : 25f, Game1.random.NextDouble() < 0.5 ? 2f : 1f) * 4f;
        sebsFrogs.scale = 4f;
        sebsFrogs.flipped = Game1.random.NextDouble() < 0.5;
        sebsFrogs.layerDepth = (float) (((double) vector2_1.Y + 2.0 + 0.109999999403954) * 64.0 / 10000.0);
        sebsFrogs.Parent = (GameLocation) this;
        temporarySprites.Add((TemporaryAnimatedSprite) sebsFrogs);
      }
      if (Game1.player.activeDialogueEvents.ContainsKey("sebastianFrog2") || Game1.random.NextDouble() >= 0.5)
        return;
      Texture2D texture2D1 = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters");
      List<TemporaryAnimatedSprite> temporarySprites1 = this.TemporarySprites;
      SebsFrogs sebsFrogs1 = new SebsFrogs();
      sebsFrogs1.texture = texture2D1;
      sebsFrogs1.sourceRect = new Microsoft.Xna.Framework.Rectangle(64, 240, 16, 16);
      sebsFrogs1.animationLength = 1;
      sebsFrogs1.sourceRectStartingPos = new Vector2(64f, 240f);
      sebsFrogs1.interval = 150f;
      sebsFrogs1.totalNumberOfLoops = 9999;
      sebsFrogs1.position = vector2_1 * 64f + new Vector2(8f, 3f) * 4f;
      sebsFrogs1.scale = 4f;
      sebsFrogs1.layerDepth = (float) (((double) vector2_1.Y + 2.0 + 0.109999999403954) * 64.0 / 10000.0);
      sebsFrogs1.flipped = Game1.random.NextDouble() < 0.5;
      sebsFrogs1.pingPong = false;
      sebsFrogs1.Parent = (GameLocation) this;
      temporarySprites1.Add((TemporaryAnimatedSprite) sebsFrogs1);
      if (Game1.random.NextDouble() >= 0.1 || Game1.timeOfDay <= 610)
        return;
      DelayedAction.playSoundAfterDelay("croak", 1000);
    }

    private void addFurnitureIfSpaceIsFreePenny(
      List<StardewValley.Object> objectsToStoreInChests,
      Furniture f,
      Furniture heldObject = null)
    {
      bool flag = false;
      foreach (Furniture furniture in this.furniture)
      {
        if (f.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) f.tileLocation).Intersects(furniture.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture.tileLocation)))
        {
          flag = true;
          break;
        }
      }
      if (this.objects.ContainsKey(f.TileLocation))
        flag = true;
      if (!flag)
      {
        this.furniture.Add(f);
        if (heldObject == null)
          return;
        this.furniture.Last<Furniture>().heldObject.Value = (StardewValley.Object) heldObject;
      }
      else
      {
        objectsToStoreInChests.Add((StardewValley.Object) f);
        if (heldObject == null)
          return;
        objectsToStoreInChests.Add((StardewValley.Object) heldObject);
      }
    }

    private void addFurnitureIfSpaceIsFree(Furniture f, Furniture heldObject = null)
    {
      if (this.objects.ContainsKey(f.TileLocation))
        return;
      this.furniture.Add(f);
      if (heldObject == null)
        return;
      this.furniture.Last<Furniture>().heldObject.Value = (StardewValley.Object) heldObject;
    }

    private void decoratePennyRoom(int whichStyle, List<StardewValley.Object> objectsToStoreInChests)
    {
      List<Chest> chestList = new List<Chest>();
      List<Vector2> vector2List = new List<Vector2>();
      Color color = new Color();
      switch (whichStyle)
      {
        case 0:
          if (this.upgradeLevel == 1)
          {
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1916, new Vector2(20f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1914, new Vector2(21f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1915, new Vector2(22f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1914, new Vector2(23f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1916, new Vector2(24f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1682, new Vector2(26f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1747, new Vector2(25f, 4f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1395, new Vector2(26f, 4f)), new Furniture(1363, Vector2.Zero));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1443, new Vector2(27f, 4f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1664, new Vector2(27f, 5f), 1));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1978, new Vector2(21f, 6f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1124, new Vector2(26f, 9f)), new Furniture(1368, Vector2.Zero));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(6, new Vector2(25f, 10f), 1));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1296, new Vector2(28f, 10f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1747, new Vector2(24f, 10f)));
            this.SetWallpaper("107", "Bedroom");
            this.SetFloor("2", "Bedroom");
            color = new Color(85, 85, (int) byte.MaxValue);
            vector2List.Add(new Vector2(21f, 10f));
            vector2List.Add(new Vector2(22f, 10f));
            break;
          }
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1916, new Vector2(23f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1914, new Vector2(24f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1604, new Vector2(26f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1915, new Vector2(28f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1916, new Vector2(30f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1914, new Vector2(32f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1916, new Vector2(33f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1443, new Vector2(23f, 13f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1747, new Vector2(24f, 13f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1395, new Vector2(25f, 13f)), new Furniture(1363, Vector2.Zero));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(714, new Vector2(31f, 13f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1443, new Vector2(33f, 13f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1978, new Vector2(27f, 15f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1664, new Vector2(32f, 15f), 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1664, new Vector2(23f, 17f), 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1124, new Vector2(31f, 21f)), new Furniture(1368, Vector2.Zero));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(416, new Vector2(25f, 22f), 2));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1296, new Vector2(23f, 22f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(6, new Vector2(30f, 22f), 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1296, new Vector2(33f, 22f)));
          this.SetWallpaper("107", "Bedroom");
          this.SetFloor("2", "Bedroom");
          color = new Color(85, 85, (int) byte.MaxValue);
          vector2List.Add(new Vector2(23f, 14f));
          vector2List.Add(new Vector2(24f, 14f));
          break;
        case 1:
          if (this.upgradeLevel == 1)
          {
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1678, new Vector2(20f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1814, new Vector2(21f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1814, new Vector2(22f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1814, new Vector2(23f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1907, new Vector2(24f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1400, new Vector2(25f, 4f)), new Furniture(1365, Vector2.Zero));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1866, new Vector2(26f, 4f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1909, new Vector2(27f, 6f), 1));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1451, new Vector2(21f, 6f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1138, new Vector2(27f, 9f)), new Furniture(1378, Vector2.Zero));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(12, new Vector2(26f, 10f), 1));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1758, new Vector2(24f, 10f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1618, new Vector2(21f, 9f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1390, new Vector2(22f, 10f)));
            this.SetWallpaper("84", "Bedroom");
            this.SetFloor("35", "Bedroom");
            color = new Color((int) byte.MaxValue, 85, 85);
            vector2List.Add(new Vector2(21f, 10f));
            vector2List.Add(new Vector2(23f, 10f));
            break;
          }
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1678, new Vector2(24f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1907, new Vector2(25f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1814, new Vector2(27f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1814, new Vector2(28f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1814, new Vector2(29f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1907, new Vector2(30f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1916, new Vector2(33f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1758, new Vector2(23f, 13f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1400, new Vector2(25f, 13f)), new Furniture(1365, Vector2.Zero));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1390, new Vector2(31f, 13f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1866, new Vector2(32f, 13f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1387, new Vector2(23f, 14f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1909, new Vector2(32f, 14f), 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(719, new Vector2(23f, 15f), 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1451, new Vector2(27f, 15f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1909, new Vector2(23f, 17f), 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1389, new Vector2(32f, 19f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1377, new Vector2(33f, 19f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1758, new Vector2(26f, 20f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(424, new Vector2(27f, 20f), 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1618, new Vector2(29f, 20f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(536, new Vector2(32f, 20f), 3));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1138, new Vector2(23f, 21f)), new Furniture(1378, Vector2.Zero));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1383, new Vector2(26f, 21f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1449, new Vector2(33f, 22f)));
          this.SetWallpaper("84", "Bedroom");
          this.SetFloor("35", "Bedroom");
          color = new Color((int) byte.MaxValue, 85, 85);
          vector2List.Add(new Vector2(24f, 13f));
          vector2List.Add(new Vector2(28f, 15f));
          break;
        case 2:
          if (this.upgradeLevel == 1)
          {
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1673, new Vector2(20f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1547, new Vector2(21f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1675, new Vector2(24f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1900, new Vector2(25f, 1f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1393, new Vector2(25f, 4f)), new Furniture(1367, Vector2.Zero));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1798, new Vector2(26f, 4f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1902, new Vector2(25f, 5f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1751, new Vector2(22f, 6f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1122, new Vector2(26f, 9f)), new Furniture(1378, Vector2.Zero));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(197, new Vector2(28f, 9f), 3));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(3, new Vector2(25f, 10f), 1));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1294, new Vector2(20f, 10f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1294, new Vector2(24f, 10f)));
            this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1964, new Vector2(21f, 8f)));
            this.SetWallpaper("95", "Bedroom");
            this.SetFloor("1", "Bedroom");
            color = new Color(85, 85, 85);
            vector2List.Add(new Vector2(22f, 10f));
            vector2List.Add(new Vector2(23f, 10f));
            break;
          }
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1673, new Vector2(23f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1675, new Vector2(25f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1547, new Vector2(27f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1900, new Vector2(30f, 10f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1751, new Vector2(23f, 13f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1393, new Vector2(25f, 13f)), new Furniture(1367, Vector2.Zero));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1798, new Vector2(32f, 13f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1902, new Vector2(31f, 14f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1964, new Vector2(27f, 15f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1294, new Vector2(23f, 16f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(3, new Vector2(31f, 19f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1294, new Vector2(23f, 20f)));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(1122, new Vector2(31f, 20f)), new Furniture(1369, Vector2.Zero));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(197, new Vector2(33f, 20f), 3));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(709, new Vector2(23f, 21f), 1));
          this.addFurnitureIfSpaceIsFreePenny(objectsToStoreInChests, new Furniture(3, new Vector2(32f, 22f), 2));
          this.SetWallpaper("95", "Bedroom");
          this.SetFloor("1", "Bedroom");
          color = new Color(85, 85, 85);
          vector2List.Add(new Vector2(24f, 13f));
          vector2List.Add(new Vector2(31f, 13f));
          break;
      }
      if (objectsToStoreInChests != null)
      {
        foreach (StardewValley.Object objectsToStoreInChest in objectsToStoreInChests)
        {
          if (chestList.Count == 0)
            chestList.Add(new Chest(true));
          bool flag = false;
          foreach (Chest chest in chestList)
          {
            if (chest.addItem((Item) objectsToStoreInChest) == null)
              flag = true;
          }
          if (!flag)
          {
            Chest chest = new Chest(true);
            chestList.Add(chest);
            chest.addItem((Item) objectsToStoreInChest);
          }
        }
      }
      for (int index = 0; index < chestList.Count; ++index)
      {
        Chest o = chestList[index];
        o.playerChoiceColor.Value = color;
        this.PlaceInNearbySpace(vector2List[Math.Min(index, vector2List.Count - 1)], (StardewValley.Object) o);
      }
    }

    public void PlaceInNearbySpace(Vector2 tileLocation, StardewValley.Object o)
    {
      if (o == null || tileLocation.Equals(Vector2.Zero))
        return;
      int num = 0;
      Queue<Vector2> vector2Queue = new Queue<Vector2>();
      HashSet<Vector2> vector2Set = new HashSet<Vector2>();
      vector2Queue.Enqueue(tileLocation);
      Vector2 vector2 = Vector2.Zero;
      for (; num < 100; ++num)
      {
        vector2 = vector2Queue.Dequeue();
        if (this.isTileOccupiedForPlacement(vector2) || !this.isTileLocationTotallyClearAndPlaceable(vector2) || this.isOpenWater((int) vector2.X, (int) vector2.Y))
        {
          vector2Set.Add(vector2);
          foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(vector2))
          {
            if (!vector2Set.Contains(adjacentTileLocation))
              vector2Queue.Enqueue(adjacentTileLocation);
          }
        }
        else
          break;
      }
      if (vector2.Equals(Vector2.Zero) || this.isTileOccupiedForPlacement(vector2) || this.isOpenWater((int) vector2.X, (int) vector2.Y) || !this.isTileLocationTotallyClearAndPlaceable(vector2))
        return;
      o.tileLocation.Value = vector2;
      this.objects.Add(vector2, o);
    }

    public virtual void RefreshFloorObjectNeighbors()
    {
      foreach (Vector2 key in this.terrainFeatures.Keys)
      {
        TerrainFeature terrainFeature = this.terrainFeatures[key];
        if (terrainFeature is Flooring)
          (terrainFeature as Flooring).OnAdded((GameLocation) this, key);
      }
    }

    public void moveObjectsForHouseUpgrade(int whichUpgrade)
    {
      this.previousUpgradeLevel = this.upgradeLevel;
      this.overlayObjects.Clear();
      switch (whichUpgrade)
      {
        case 0:
          if (this.upgradeLevel != 1)
            break;
          this.shiftObjects(-6, 0);
          break;
        case 1:
          if (this.upgradeLevel == 0)
            this.shiftObjects(6, 0);
          if (this.upgradeLevel != 2)
            break;
          this.shiftObjects(-3, 0);
          break;
        case 2:
        case 3:
          if (this.upgradeLevel == 1)
          {
            this.shiftObjects(3, 9);
            foreach (Furniture furniture in this.furniture)
            {
              if ((double) furniture.tileLocation.X >= 10.0 && (double) furniture.tileLocation.X <= 13.0 && (double) furniture.tileLocation.Y >= 10.0 && (double) furniture.tileLocation.Y <= 11.0)
              {
                furniture.tileLocation.X -= 3f;
                furniture.boundingBox.X -= 192;
                furniture.tileLocation.Y -= 9f;
                furniture.boundingBox.Y -= 576;
                furniture.updateDrawPosition();
              }
            }
            this.moveFurniture(27, 13, 1, 4);
            this.moveFurniture(28, 13, 2, 4);
            this.moveFurniture(29, 13, 3, 4);
            this.moveFurniture(28, 14, 7, 4);
            this.moveFurniture(29, 14, 8, 4);
            this.moveFurniture(27, 14, 4, 4);
            this.moveFurniture(28, 15, 5, 4);
            this.moveFurniture(29, 16, 6, 4);
          }
          if (this.upgradeLevel != 0)
            break;
          this.shiftObjects(9, 9);
          break;
      }
    }

    protected override LocalizedContentManager getMapLoader()
    {
      if (this.mapLoader == null)
        this.mapLoader = Game1.game1.xTileContent.CreateTemporary();
      return this.mapLoader;
    }

    public override void drawAboveFrontLayer(SpriteBatch b)
    {
      base.drawAboveFrontLayer(b);
      if (!this.fridge.Value.mutex.IsLocked())
        return;
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) this.fridgePosition.X, (float) (this.fridgePosition.Y - 1)) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 192, 16, 32)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((this.fridgePosition.Y + 1) * 64 + 1) / 10000f);
    }

    public override void updateMap()
    {
      bool flag = this.owner.spouse != null && this.owner.isMarried();
      this.mapPath.Value = "Maps\\FarmHouse" + (this.upgradeLevel == 0 ? "" : (this.upgradeLevel == 3 ? "2" : this.upgradeLevel.ToString() ?? "")) + (flag ? "_marriage" : "");
      base.updateMap();
    }

    public virtual void setMapForUpgradeLevel(int level)
    {
      this.upgradeLevel = level;
      int num = this.synchronizedDisplayedLevel.Value;
      this.currentlyDisplayedUpgradeLevel = level;
      this.synchronizedDisplayedLevel.Value = level;
      bool flag1 = this.owner.isMarried() && this.owner.spouse != null;
      if (this.displayingSpouseRoom && !flag1)
        this.displayingSpouseRoom = false;
      this.updateMap();
      this.RefreshFloorObjectNeighbors();
      if (flag1)
        this.showSpouseRoom();
      this.loadObjects();
      if (level == 3)
      {
        this.AddCellarTiles();
        this.createCellarWarps();
        if (!Game1.player.craftingRecipes.ContainsKey("Cask"))
          Game1.player.craftingRecipes.Add("Cask", 0);
      }
      bool flag2 = false;
      if (this.previousUpgradeLevel == 0 && this.upgradeLevel >= 0)
        flag2 = true;
      if (this.previousUpgradeLevel >= 0)
      {
        if (this.previousUpgradeLevel < 2 && this.upgradeLevel >= 2)
        {
          for (int index1 = 0; index1 < this.map.Layers[0].TileWidth; ++index1)
          {
            for (int index2 = 0; index2 < this.map.Layers[0].TileHeight; ++index2)
            {
              if (this.doesTileHaveProperty(index1, index2, "DefaultChildBedPosition", "Back") != null)
              {
                this.furniture.Add((Furniture) new BedFurniture(BedFurniture.CHILD_BED_INDEX, new Vector2((float) index1, (float) index2)));
                break;
              }
            }
          }
        }
        Furniture furniture1 = (Furniture) null;
        if (this.previousUpgradeLevel == 0)
        {
          foreach (Furniture furniture2 in this.furniture)
          {
            if (furniture2 is BedFurniture && (furniture2 as BedFurniture).bedType == BedFurniture.BedType.Single)
            {
              furniture1 = furniture2;
              break;
            }
          }
        }
        else
        {
          foreach (Furniture furniture3 in this.furniture)
          {
            if (furniture3 is BedFurniture && (furniture3 as BedFurniture).bedType == BedFurniture.BedType.Double)
            {
              furniture1 = furniture3;
              break;
            }
          }
        }
        if (this.upgradeLevel != 3 || flag2)
        {
          for (int index3 = 0; index3 < this.map.Layers[0].TileWidth; ++index3)
          {
            for (int index4 = 0; index4 < this.map.Layers[0].TileHeight; ++index4)
            {
              if (this.doesTileHaveProperty(index3, index4, "DefaultBedPosition", "Back") != null)
              {
                int bed_index = BedFurniture.DEFAULT_BED_INDEX;
                if (this.previousUpgradeLevel != 1 || furniture1 == null || (double) furniture1.tileLocation.X == 24.0 && (double) furniture1.tileLocation.Y == 12.0)
                {
                  if (furniture1 != null)
                    bed_index = furniture1.ParentSheetIndex;
                  if (this.previousUpgradeLevel == 0 && furniture1 != null)
                  {
                    furniture1.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture1.tileLocation, (GameLocation) this);
                    this.furniture.Remove(this.furniture.GuidOf(furniture1));
                    this.furniture.Add((Furniture) new BedFurniture(Utility.GetDoubleWideVersionOfBed(bed_index), new Vector2((float) index3, (float) index4)));
                    break;
                  }
                  if (furniture1 != null)
                  {
                    furniture1.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture1.tileLocation, (GameLocation) this);
                    this.furniture.Remove(this.furniture.GuidOf(furniture1));
                    this.furniture.Add((Furniture) new BedFurniture(furniture1.ParentSheetIndex, new Vector2((float) index3, (float) index4)));
                    break;
                  }
                  break;
                }
                break;
              }
            }
          }
        }
        this.previousUpgradeLevel = -1;
      }
      if (num != level)
        this.lightGlows.Clear();
      this.fridgePosition = new Point();
      bool flag3 = false;
      for (int x = 0; x < this.map.GetLayer("Buildings").LayerWidth; ++x)
      {
        for (int y = 0; y < this.map.GetLayer("Buildings").LayerHeight; ++y)
        {
          if (this.map.GetLayer("Buildings").Tiles[x, y] != null && this.map.GetLayer("Buildings").Tiles[x, y].TileIndex == 173)
          {
            this.fridgePosition = new Point(x, y);
            flag3 = true;
            break;
          }
        }
        if (flag3)
          break;
      }
    }

    public void createCellarWarps() => this.updateCellarWarps();

    public void updateCellarWarps()
    {
      Layer layer = this.map.GetLayer("Back");
      for (int index1 = 0; index1 < layer.LayerWidth; ++index1)
      {
        for (int index2 = 0; index2 < layer.LayerHeight; ++index2)
        {
          string str = this.doesTileHaveProperty(index1, index2, "TouchAction", "Back");
          if (str != null && str.StartsWith("Warp "))
          {
            string[] strArray = str.Split(' ');
            if (strArray.Length >= 2 && strArray[1].StartsWith("Cellar"))
            {
              strArray[1] = this.GetCellarName();
              this.setTileProperty(index1, index2, "Back", "TouchAction", string.Join(" ", strArray));
            }
          }
        }
      }
      if (this.cellarWarps == null)
        return;
      foreach (Warp cellarWarp in this.cellarWarps)
      {
        if (!this.warps.Contains(cellarWarp))
          this.warps.Add(cellarWarp);
        cellarWarp.TargetName = this.GetCellarName();
      }
    }

    public virtual int GetSpouseRoomWidth() => 6;

    public virtual int GetSpouseRoomHeight() => 9;

    public virtual Point GetSpouseRoomCorner() => this.upgradeLevel == 1 ? new Point(29, 1) : new Point(35, 10);

    public virtual void loadSpouseRoom()
    {
      NPC spouse = this.owner.getSpouse();
      this.spouseRoomSpot = this.GetSpouseRoomCorner();
      this.spouseRoomSpot.X += 3;
      this.spouseRoomSpot.Y += 4;
      if (spouse == null)
        return;
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\SpouseRooms");
      int num1 = -1;
      string map_name = "spouseRooms";
      if (dictionary != null)
      {
        if (dictionary.ContainsKey(spouse.Name))
        {
          try
          {
            string[] strArray = dictionary[spouse.Name].Split('/');
            map_name = strArray[0];
            num1 = int.Parse(strArray[1]);
          }
          catch (Exception ex)
          {
          }
        }
      }
      if (num1 == -1)
      {
        switch (spouse.Name)
        {
          case "Abigail":
            num1 = 0;
            break;
          case "Alex":
            num1 = 6;
            break;
          case "Elliott":
            num1 = 8;
            break;
          case "Emily":
            num1 = 11;
            break;
          case "Haley":
            num1 = 3;
            break;
          case "Harvey":
            num1 = 7;
            break;
          case "Krobus":
            num1 = 12;
            break;
          case "Leah":
            num1 = 2;
            break;
          case "Maru":
            num1 = 4;
            break;
          case "Penny":
            num1 = 1;
            break;
          case "Sam":
            num1 = 9;
            break;
          case "Sebastian":
            num1 = 5;
            break;
          case "Shane":
            num1 = 10;
            break;
        }
      }
      int spouseRoomWidth = this.GetSpouseRoomWidth();
      int spouseRoomHeight = this.GetSpouseRoomHeight();
      Point spouseRoomCorner = this.GetSpouseRoomCorner();
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(spouseRoomCorner.X, spouseRoomCorner.Y, spouseRoomWidth, spouseRoomHeight);
      Map map = Game1.game1.xTileContent.Load<Map>("Maps\\" + map_name);
      int num2 = map.Layers[0].LayerWidth / spouseRoomWidth;
      int num3 = map.Layers[0].LayerHeight / spouseRoomHeight;
      Point point = new Point(num1 % num2 * spouseRoomWidth, num1 / num2 * spouseRoomHeight);
      ((IDictionary<string, PropertyValue>) this.map.Properties).Remove("DayTiles");
      ((IDictionary<string, PropertyValue>) this.map.Properties).Remove("NightTiles");
      List<KeyValuePair<Point, Tile>> keyValuePairList = new List<KeyValuePair<Point, Tile>>();
      Layer layer = this.map.GetLayer("Front");
      for (int left = rectangle.Left; left < rectangle.Right; ++left)
      {
        Point key = new Point(left, rectangle.Bottom - 1);
        Tile tile = layer.Tiles[key.X, key.Y];
        if (tile != null)
          keyValuePairList.Add(new KeyValuePair<Point, Tile>(key, tile));
      }
      if (this._appliedMapOverrides.Contains("spouse_room"))
        this._appliedMapOverrides.Remove("spouse_room");
      this.ApplyMapOverride(map_name, "spouse_room", new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(point.X, point.Y, rectangle.Width, rectangle.Height)), new Microsoft.Xna.Framework.Rectangle?(rectangle));
      for (int index1 = 0; index1 < rectangle.Width; ++index1)
      {
        for (int index2 = 0; index2 < rectangle.Height; ++index2)
        {
          if (map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2] != null)
            this.adjustMapLightPropertiesForLamp(map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2].TileIndex, rectangle.X + index1, rectangle.Y + index2, "Buildings");
          if (index2 < rectangle.Height - 1 && map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2] != null)
            this.adjustMapLightPropertiesForLamp(map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2].TileIndex, rectangle.X + index1, rectangle.Y + index2, "Front");
        }
      }
      bool flag = false;
      for (int left = rectangle.Left; left < rectangle.Right; ++left)
      {
        for (int top = rectangle.Top; top < rectangle.Bottom; ++top)
        {
          if (this.getTileIndexAt(new Point(left, top), "Paths") == 7)
          {
            flag = true;
            this.spouseRoomSpot = new Point(left, top);
            break;
          }
        }
        if (flag)
          break;
      }
      Point spouseRoomSpot = this.GetSpouseRoomSpot();
      this.setTileProperty(spouseRoomSpot.X, spouseRoomSpot.Y, "Back", "NoFurniture", "T");
      foreach (KeyValuePair<Point, Tile> keyValuePair in keyValuePairList)
        layer.Tiles[keyValuePair.Key.X, keyValuePair.Key.Y] = keyValuePair.Value;
    }

    public virtual Microsoft.Xna.Framework.Rectangle? GetCribBounds() => this.upgradeLevel < 2 ? new Microsoft.Xna.Framework.Rectangle?() : new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(15, 2, 3, 4));

    public virtual Microsoft.Xna.Framework.Rectangle? GetBedBounds(int child_index = 0)
    {
      if (this.upgradeLevel < 2)
        return new Microsoft.Xna.Framework.Rectangle?();
      if (child_index == 0)
        return new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(22, 3, 2, 4));
      return child_index == 1 ? new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(26, 3, 2, 4)) : new Microsoft.Xna.Framework.Rectangle?();
    }

    public virtual Microsoft.Xna.Framework.Rectangle? GetChildBedBounds(int child_index = 0)
    {
      if (this.upgradeLevel < 2)
        return new Microsoft.Xna.Framework.Rectangle?();
      if (child_index == 0)
        return new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(22, 3, 2, 4));
      return child_index == 1 ? new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(26, 3, 2, 4)) : new Microsoft.Xna.Framework.Rectangle?();
    }

    public virtual void UpdateChildRoom()
    {
      Microsoft.Xna.Framework.Rectangle? cribBounds = this.GetCribBounds();
      if (!cribBounds.HasValue)
        return;
      if (this._appliedMapOverrides.Contains("crib"))
        this._appliedMapOverrides.Remove("crib");
      this.ApplyMapOverride("FarmHouse_Crib_" + this.cribStyle.Value.ToString(), "crib", destination_rect: cribBounds);
    }

    public void playerDivorced() => this.displayingSpouseRoom = false;

    public virtual List<Microsoft.Xna.Framework.Rectangle> getForbiddenPetWarpTiles()
    {
      List<Microsoft.Xna.Framework.Rectangle> forbiddenPetWarpTiles = new List<Microsoft.Xna.Framework.Rectangle>();
      switch (this.upgradeLevel)
      {
        case 0:
          forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(2, 8, 3, 4));
          break;
        case 1:
          forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(8, 8, 3, 4));
          forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(17, 8, 4, 3));
          break;
        case 2:
        case 3:
          forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(11, 17, 3, 4));
          forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(20, 17, 4, 3));
          forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(12, 5, 4, 3));
          forbiddenPetWarpTiles.Add(new Microsoft.Xna.Framework.Rectangle(11, 7, 2, 6));
          break;
      }
      return forbiddenPetWarpTiles;
    }

    public bool canPetWarpHere(Vector2 tile_position)
    {
      foreach (Microsoft.Xna.Framework.Rectangle forbiddenPetWarpTile in this.getForbiddenPetWarpTiles())
      {
        if (forbiddenPetWarpTile.Contains((int) tile_position.X, (int) tile_position.Y))
          return false;
      }
      return true;
    }

    public override List<Microsoft.Xna.Framework.Rectangle> getWalls()
    {
      List<Microsoft.Xna.Framework.Rectangle> walls = new List<Microsoft.Xna.Framework.Rectangle>();
      switch (this.upgradeLevel)
      {
        case 0:
          walls.Add(new Microsoft.Xna.Framework.Rectangle(1, 1, 10, 3));
          break;
        case 1:
          walls.Add(new Microsoft.Xna.Framework.Rectangle(1, 1, 17, 3));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(18, 6, 2, 2));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(20, 1, 9, 3));
          break;
        case 2:
        case 3:
          walls.Add(new Microsoft.Xna.Framework.Rectangle(1, 1, 12, 3));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(15, 1, 13, 3));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(13, 3, 2, 2));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(1, 10, 10, 3));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(13, 10, 8, 3));
          int num = this.owner.hasOrWillReceiveMail("renovation_corner_open") ? -3 : 0;
          if (this.owner.hasOrWillReceiveMail("renovation_bedroom_open"))
          {
            walls.Add(new Microsoft.Xna.Framework.Rectangle(21, 15, 0, 2));
            walls.Add(new Microsoft.Xna.Framework.Rectangle(21, 10, 13 + num, 3));
          }
          else
          {
            walls.Add(new Microsoft.Xna.Framework.Rectangle(21, 15, 2, 2));
            walls.Add(new Microsoft.Xna.Framework.Rectangle(23, 10, 11 + num, 3));
          }
          if (this.owner.hasOrWillReceiveMail("renovation_southern_open"))
          {
            walls.Add(new Microsoft.Xna.Framework.Rectangle(23, 24, 3, 3));
            walls.Add(new Microsoft.Xna.Framework.Rectangle(31, 24, 3, 3));
          }
          else
          {
            walls.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
            walls.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
          }
          if (this.owner.hasOrWillReceiveMail("renovation_corner_open"))
          {
            walls.Add(new Microsoft.Xna.Framework.Rectangle(30, 1, 9, 3));
            walls.Add(new Microsoft.Xna.Framework.Rectangle(28, 3, 2, 2));
            break;
          }
          walls.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
          walls.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
          break;
      }
      return walls;
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      if (l is FarmHouse)
        this.cribStyle.Value = (l as FarmHouse).cribStyle.Value;
      base.TransferDataFromSavedLocation(l);
    }

    public override List<Microsoft.Xna.Framework.Rectangle> getFloors()
    {
      List<Microsoft.Xna.Framework.Rectangle> floors = new List<Microsoft.Xna.Framework.Rectangle>();
      switch (this.upgradeLevel)
      {
        case 0:
          floors.Add(new Microsoft.Xna.Framework.Rectangle(1, 3, 10, 9));
          break;
        case 1:
          floors.Add(new Microsoft.Xna.Framework.Rectangle(1, 3, 6, 9));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(7, 3, 11, 9));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(18, 8, 2, 2));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(20, 3, 9, 8));
          break;
        case 2:
        case 3:
          floors.Add(new Microsoft.Xna.Framework.Rectangle(1, 3, 12, 6));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(15, 3, 13, 6));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(13, 5, 2, 2));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(0, 12, 10, 11));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(10, 12, 11, 9));
          if (this.owner.mailReceived.Contains("renovation_bedroom_open"))
          {
            floors.Add(new Microsoft.Xna.Framework.Rectangle(21, 17, 0, 2));
            floors.Add(new Microsoft.Xna.Framework.Rectangle(21, 12, 14, 11));
          }
          else
          {
            floors.Add(new Microsoft.Xna.Framework.Rectangle(21, 17, 2, 2));
            floors.Add(new Microsoft.Xna.Framework.Rectangle(23, 12, 12, 11));
          }
          if (this.owner.hasOrWillReceiveMail("renovation_southern_open"))
            floors.Add(new Microsoft.Xna.Framework.Rectangle(23, 26, 11, 8));
          else
            floors.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
          if (this.owner.hasOrWillReceiveMail("renovation_corner_open"))
          {
            floors.Add(new Microsoft.Xna.Framework.Rectangle(28, 5, 2, 3));
            floors.Add(new Microsoft.Xna.Framework.Rectangle(30, 3, 9, 6));
            break;
          }
          floors.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
          floors.Add(new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
          break;
      }
      return floors;
    }

    public virtual bool CanModifyCrib()
    {
      if (this.owner == null || this.owner.isMarried() && this.owner.GetSpouseFriendship().DaysUntilBirthing != -1)
        return false;
      foreach (NPC child in this.owner.getChildren())
      {
        if (child.Age < 3)
          return false;
      }
      return true;
    }
  }
}
