// Decompiled with JetBrains decompiler
// Type: StardewValley.Farm
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewValley
{
  public class Farm : BuildableGameLocation, IAnimalLocation
  {
    [XmlIgnore]
    [NonInstancedStatic]
    public static Texture2D houseTextures = Game1.content.Load<Texture2D>("Buildings\\houses");
    [XmlIgnore]
    public Texture2D paintedHouseTexture;
    public Color? frameHouseColor;
    public NetRef<BuildingPaintColor> housePaintColor = new NetRef<BuildingPaintColor>();
    public const int default_layout = 0;
    public const int riverlands_layout = 1;
    public const int forest_layout = 2;
    public const int mountains_layout = 3;
    public const int combat_layout = 4;
    public const int fourCorners_layout = 5;
    public const int beach_layout = 6;
    public const int mod_layout = 7;
    public const int layout_max = 7;
    [XmlElement("animals")]
    public readonly NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> animals = new NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>();
    [XmlElement("piecesOfHay")]
    public readonly NetInt piecesOfHay = new NetInt(0);
    [XmlElement("grandpaScore")]
    public NetInt grandpaScore = new NetInt(0);
    [XmlElement("farmCaveReady")]
    public NetBool farmCaveReady = new NetBool(false);
    private TemporaryAnimatedSprite shippingBinLid;
    private Microsoft.Xna.Framework.Rectangle shippingBinLidOpenArea = new Microsoft.Xna.Framework.Rectangle(4480, 832, 256, 192);
    [XmlIgnore]
    private readonly NetCollection<Item> sharedShippingBin = new NetCollection<Item>();
    [XmlIgnore]
    public Item lastItemShipped;
    public bool hasSeenGrandpaNote;
    protected Dictionary<string, Dictionary<Point, Tile>> _baseSpouseAreaTiles = new Dictionary<string, Dictionary<Point, Tile>>();
    [XmlElement("houseSource")]
    public readonly NetRectangle houseSource = new NetRectangle();
    [XmlElement("greenhouseUnlocked")]
    public readonly NetBool greenhouseUnlocked = new NetBool();
    [XmlElement("greenhouseMoved")]
    public readonly NetBool greenhouseMoved = new NetBool();
    private readonly NetEvent1Field<Vector2, NetVector2> spawnCrowEvent = new NetEvent1Field<Vector2, NetVector2>();
    public readonly NetEvent1<Farm.LightningStrikeEvent> lightningStrikeEvent = new NetEvent1<Farm.LightningStrikeEvent>();
    private readonly List<KeyValuePair<long, FarmAnimal>> _tempAnimals = new List<KeyValuePair<long, FarmAnimal>>();
    public readonly NetBool petBowlWatered = new NetBool(false);
    [XmlIgnore]
    public readonly NetPoint petBowlPosition = new NetPoint();
    [XmlIgnore]
    public Point? mapGrandpaShrinePosition;
    [XmlIgnore]
    public Point? mapMainMailboxPosition;
    [XmlIgnore]
    public Point? mainFarmhouseEntry;
    [XmlIgnore]
    public Vector2? mapSpouseAreaCorner;
    [XmlIgnore]
    public Vector2? mapShippingBinPosition;
    private int chimneyTimer = 500;
    protected Microsoft.Xna.Framework.Rectangle? _mountainForageRectangle;
    protected bool? _shouldSpawnForestFarmForage;
    protected bool? _shouldSpawnBeachFarmForage;
    protected bool? _oceanCrabPotOverride;
    protected string _fishLocationOverride;
    protected float _fishChanceOverride;
    public Point spousePatioSpot;
    public const int numCropsForCrow = 16;

    public NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> Animals => this.animals;

    public Farm()
    {
    }

    public Farm(string mapPath, string name)
      : base(mapPath, name)
    {
      if (Game1.IsMasterGame)
      {
        Layer layer = this.map.GetLayer("Buildings");
        for (int x = 0; x < layer.LayerWidth; ++x)
        {
          for (int y = 0; y < layer.LayerHeight; ++y)
          {
            if (layer.Tiles[x, y] != null && layer.Tiles[x, y].TileIndex == 1938)
              this.petBowlPosition.Set(x, y);
          }
        }
      }
      this.AddModularShippingBin();
    }

    public virtual void AddModularShippingBin()
    {
      Building building1 = (Building) new ShippingBin(new BluePrint("Shipping Bin"), this.GetStarterShippingBinLocation());
      this.buildings.Add(building1);
      building1.load();
      Building building2 = (Building) new GreenhouseBuilding(new BluePrint("Greenhouse"), this.GetGreenhouseStartLocation());
      this.buildings.Add(building2);
      building2.load();
    }

    public virtual Microsoft.Xna.Framework.Rectangle GetHouseRect()
    {
      Point mainFarmHouseEntry = this.GetMainFarmHouseEntry();
      return new Microsoft.Xna.Framework.Rectangle(mainFarmHouseEntry.X - 5, mainFarmHouseEntry.Y - 4, 9, 6);
    }

    public virtual Vector2 GetStarterShippingBinLocation()
    {
      if (!this.mapShippingBinPosition.HasValue)
        this.mapShippingBinPosition = new Vector2?(Utility.PointToVector2(this.GetMapPropertyPosition("ShippingBinLocation", 71, 14)));
      return this.mapShippingBinPosition.Value;
    }

    public virtual Vector2 GetGreenhouseStartLocation()
    {
      if (this.map.Properties.ContainsKey("GreenhouseLocation"))
      {
        int result1 = -1;
        int result2 = -1;
        string[] strArray = this.map.Properties["GreenhouseLocation"].ToString().Split(' ');
        if (strArray.Length >= 2 && int.TryParse(strArray[0], out result1) && int.TryParse(strArray[1], out result2))
          return new Vector2((float) result1, (float) result2);
      }
      if (Game1.whichFarm == 5)
        return new Vector2(36f, 29f);
      return Game1.whichFarm == 6 ? new Vector2(14f, 14f) : new Vector2(25f, 10f);
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.animals, (INetSerializable) this.piecesOfHay, (INetSerializable) this.sharedShippingBin, (INetSerializable) this.houseSource, (INetSerializable) this.spawnCrowEvent, (INetSerializable) this.petBowlWatered, (INetSerializable) this.petBowlPosition, (INetSerializable) this.lightningStrikeEvent, (INetSerializable) this.grandpaScore, (INetSerializable) this.greenhouseUnlocked, (INetSerializable) this.greenhouseMoved, (INetSerializable) this.housePaintColor, (INetSerializable) this.farmCaveReady);
      this.spawnCrowEvent.onEvent += new AbstractNetEvent1<Vector2>.Event(this.doSpawnCrow);
      this.lightningStrikeEvent.onEvent += new AbstractNetEvent1<Farm.LightningStrikeEvent>.Event(this.doLightningStrike);
      this.greenhouseMoved.fieldChangeVisibleEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((field, old_value, new_value) => this.ClearGreenhouseGrassTiles());
      this.petBowlWatered.fieldChangeVisibleEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((field, old_value, new_value) => this._UpdateWaterBowl());
      if (this.housePaintColor.Value != null)
        return;
      this.housePaintColor.Value = new BuildingPaintColor();
    }

    public virtual void ClearGreenhouseGrassTiles()
    {
      if (this.map == null || Game1.gameMode == (byte) 6 || !this.greenhouseMoved.Value)
        return;
      switch (Game1.whichFarm)
      {
        case 0:
        case 3:
        case 4:
          this.ApplyMapOverride("Farm_Greenhouse_Dirt", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int) this.GetGreenhouseStartLocation().X, (int) this.GetGreenhouseStartLocation().Y, 9, 6)));
          break;
        case 5:
          this.ApplyMapOverride("Farm_Greenhouse_Dirt_FourCorners", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int) this.GetGreenhouseStartLocation().X, (int) this.GetGreenhouseStartLocation().Y, 9, 6)));
          break;
      }
    }

    protected void _UpdateWaterBowl()
    {
      if (this.petBowlWatered.Value)
        this.setMapTileIndex(this.petBowlPosition.X, this.petBowlPosition.Y, 1939, "Buildings");
      else
        this.setMapTileIndex(this.petBowlPosition.X, this.petBowlPosition.Y, 1938, "Buildings");
    }

    public static string getMapNameFromTypeInt(int type)
    {
      switch (type)
      {
        case 0:
          return nameof (Farm);
        case 1:
          return "Farm_Fishing";
        case 2:
          return "Farm_Foraging";
        case 3:
          return "Farm_Mining";
        case 4:
          return "Farm_Combat";
        case 5:
          return "Farm_FourCorners";
        case 6:
          return "Farm_Island";
        case 7:
          if (Game1.whichModFarm != null)
            return Game1.whichModFarm.MapName;
          break;
      }
      return nameof (Farm);
    }

    public Point GetPetStartLocation() => new Point(this.petBowlPosition.X - 1, this.petBowlPosition.Y + 1);

    public override void DayUpdate(int dayOfMonth)
    {
      for (int index = this.animals.Count() - 1; index >= 0; --index)
        this.animals.Pairs.ElementAt(index).Value.dayUpdate((GameLocation) this);
      base.DayUpdate(dayOfMonth);
      this.UpdatePatio();
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        if (this.characters[index] is Pet && (this.getTileIndexAt(this.characters[index].getTileLocationPoint(), "Buildings") != -1 || this.getTileIndexAt(this.characters[index].getTileX() + 1, this.characters[index].getTileY(), "Buildings") != -1 || !this.isTileLocationTotallyClearAndPlaceable(this.characters[index].getTileLocation()) || !this.isTileLocationTotallyClearAndPlaceable(new Vector2((float) (this.characters[index].getTileX() + 1), (float) this.characters[index].getTileY()))))
          this.characters[index].setTilePosition(this.GetPetStartLocation());
      }
      this.lastItemShipped = (Item) null;
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        if (this.characters[index] is JunimoHarvester)
          this.characters.RemoveAt(index);
      }
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        if (this.characters[index] is Monster && (this.characters[index] as Monster).wildernessFarmMonster)
          this.characters.RemoveAt(index);
      }
      if (this.characters.Count > 5)
      {
        int num = 0;
        for (int index = this.characters.Count - 1; index >= 0; --index)
        {
          if (this.characters[index] is GreenSlime && Game1.random.NextDouble() < 0.035)
          {
            this.characters.RemoveAt(index);
            ++num;
          }
        }
        if (num > 0)
          Game1.multiplayer.broadcastGlobalMessage(num == 1 ? "Strings\\Locations:Farm_1SlimeEscaped" : "Strings\\Locations:Farm_NSlimesEscaped", false, num.ToString() ?? "");
      }
      if (Game1.whichFarm == 5)
      {
        if (this.isTileLocationTotallyClearAndPlaceable(5, 32) && this.isTileLocationTotallyClearAndPlaceable(6, 32) && this.isTileLocationTotallyClearAndPlaceable(6, 33) && this.isTileLocationTotallyClearAndPlaceable(5, 33))
          this.resourceClumps.Add(new ResourceClump(600, 2, 2, new Vector2(5f, 32f)));
        if (this.objects.Count() > 0)
        {
          for (int index = 0; index < 6; ++index)
          {
            Object @object = this.objects.Pairs.ElementAt(Game1.random.Next(this.objects.Count())).Value;
            if (@object.name.Equals("Weeds") && (double) @object.tileLocation.X < 36.0 && (double) @object.tileLocation.Y < 34.0)
              @object.ParentSheetIndex = 792 + Utility.getSeasonNumber(Game1.currentSeason);
          }
        }
      }
      if (this.ShouldSpawnBeachFarmForage())
      {
        while (Game1.random.NextDouble() < 0.9)
        {
          Vector2 randomTile = this.getRandomTile();
          if (this.isTileLocationTotallyClearAndPlaceable(randomTile) && this.getTileIndexAt((int) randomTile.X, (int) randomTile.Y, "AlwaysFront") == -1)
          {
            int parentSheetIndex1 = -1;
            if (this.doesTileHavePropertyNoNull((int) randomTile.X, (int) randomTile.Y, "BeachSpawn", "Back") != "")
            {
              parentSheetIndex1 = 372;
              Game1.stats.incrementStat("beachFarmSpawns", 1);
              switch (Game1.random.Next(6))
              {
                case 0:
                  parentSheetIndex1 = 393;
                  break;
                case 1:
                  parentSheetIndex1 = 719;
                  break;
                case 2:
                  parentSheetIndex1 = 718;
                  break;
                case 3:
                  parentSheetIndex1 = 723;
                  break;
                case 4:
                case 5:
                  parentSheetIndex1 = 152;
                  break;
              }
              if (Game1.stats.DaysPlayed > 1U)
              {
                if (Game1.random.NextDouble() < 0.15 || Game1.stats.getStat("beachFarmSpawns") % 4U == 0U)
                {
                  int parentSheetIndex2 = Game1.random.Next(922, 925);
                  this.objects.Add(randomTile, new Object(randomTile, parentSheetIndex2, 1)
                  {
                    Fragility = 2,
                    MinutesUntilReady = 3
                  });
                  parentSheetIndex1 = -1;
                }
                else if (Game1.random.NextDouble() < 0.1)
                  parentSheetIndex1 = 397;
                else if (Game1.random.NextDouble() < 0.05)
                  parentSheetIndex1 = 392;
                else if (Game1.random.NextDouble() < 0.02)
                  parentSheetIndex1 = 394;
              }
            }
            else if (Game1.currentSeason != "winter" && new Microsoft.Xna.Framework.Rectangle(20, 66, 33, 18).Contains((int) randomTile.X, (int) randomTile.Y) && this.doesTileHavePropertyNoNull((int) randomTile.X, (int) randomTile.Y, "Type", "Back") == "Grass")
              parentSheetIndex1 = Utility.getRandomBasicSeasonalForageItem(Game1.currentSeason, (int) Game1.stats.DaysPlayed);
            if (parentSheetIndex1 != -1)
              this.dropObject(new Object(randomTile, parentSheetIndex1, (string) null, false, true, false, true), randomTile * 64f, Game1.viewport, true);
          }
        }
      }
      if (Game1.whichFarm == 2)
      {
        for (int x = 0; x < 20; ++x)
        {
          for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
          {
            if (this.map.GetLayer("Paths").Tiles[x, y] != null && this.map.GetLayer("Paths").Tiles[x, y].TileIndex == 21 && this.isTileLocationTotallyClearAndPlaceable(x, y) && this.isTileLocationTotallyClearAndPlaceable(x + 1, y) && this.isTileLocationTotallyClearAndPlaceable(x + 1, y + 1) && this.isTileLocationTotallyClearAndPlaceable(x, y + 1))
              this.resourceClumps.Add(new ResourceClump(600, 2, 2, new Vector2((float) x, (float) y)));
          }
        }
      }
      if (this.ShouldSpawnForestFarmForage() && !Game1.IsWinter)
      {
        while (Game1.random.NextDouble() < 0.75)
        {
          Vector2 vector2 = new Vector2((float) Game1.random.Next(18), (float) Game1.random.Next(this.map.Layers[0].LayerHeight));
          if (Game1.random.NextDouble() < 0.5 || Game1.whichFarm != 2)
            vector2 = this.getRandomTile();
          if (this.isTileLocationTotallyClearAndPlaceable(vector2) && this.getTileIndexAt((int) vector2.X, (int) vector2.Y, "AlwaysFront") == -1 && (Game1.whichFarm == 2 && (double) vector2.X < 18.0 || this.doesTileHavePropertyNoNull((int) vector2.X, (int) vector2.Y, "Type", "Back").Equals("Grass")))
          {
            int parentSheetIndex = 792;
            string currentSeason = Game1.currentSeason;
            if (!(currentSeason == "spring"))
            {
              if (!(currentSeason == "summer"))
              {
                if (currentSeason == "fall")
                {
                  switch (Game1.random.Next(4))
                  {
                    case 0:
                      parentSheetIndex = 281;
                      break;
                    case 1:
                      parentSheetIndex = 420;
                      break;
                    case 2:
                      parentSheetIndex = 422;
                      break;
                    case 3:
                      parentSheetIndex = 404;
                      break;
                  }
                }
              }
              else
              {
                switch (Game1.random.Next(4))
                {
                  case 0:
                    parentSheetIndex = 402;
                    break;
                  case 1:
                    parentSheetIndex = 396;
                    break;
                  case 2:
                    parentSheetIndex = 398;
                    break;
                  case 3:
                    parentSheetIndex = 404;
                    break;
                }
              }
            }
            else
            {
              switch (Game1.random.Next(4))
              {
                case 0:
                  parentSheetIndex = 16;
                  break;
                case 1:
                  parentSheetIndex = 22;
                  break;
                case 2:
                  parentSheetIndex = 20;
                  break;
                case 3:
                  parentSheetIndex = 257;
                  break;
              }
            }
            this.dropObject(new Object(vector2, parentSheetIndex, (string) null, false, true, false, true), vector2 * 64f, Game1.viewport, true);
          }
        }
        if (this.objects.Count() > 0)
        {
          for (int index = 0; index < 6; ++index)
          {
            Object @object = this.objects.Pairs.ElementAt(Game1.random.Next(this.objects.Count())).Value;
            if (@object.name.Equals("Weeds"))
              @object.ParentSheetIndex = 792 + Utility.getSeasonNumber(Game1.currentSeason);
          }
        }
      }
      if (Game1.whichFarm == 3 || Game1.whichFarm == 5 || this.ShouldSpawnMountainOres())
        this.doDailyMountainFarmUpdate();
      ICollection<Vector2> source = (ICollection<Vector2>) new List<Vector2>((IEnumerable<Vector2>) this.terrainFeatures.Keys);
      for (int index = source.Count - 1; index >= 0; --index)
      {
        Vector2 key = source.ElementAt<Vector2>(index);
        if (this.terrainFeatures[key] is HoeDirt && (this.terrainFeatures[key] as HoeDirt).crop == null)
        {
          if (this.objects.ContainsKey(key))
          {
            Object @object = this.objects[key];
            if (@object != null && @object.IsSpawnedObject && @object.isForage((GameLocation) this))
              continue;
          }
          if (Game1.random.NextDouble() <= 0.1)
            this.terrainFeatures.Remove(key);
        }
      }
      if (this.terrainFeatures.Count() > 0 && Game1.currentSeason.Equals("fall") && Game1.dayOfMonth > 1 && Game1.random.NextDouble() < 0.05)
      {
        for (int index = 0; index < 10; ++index)
        {
          TerrainFeature terrainFeature = this.terrainFeatures.Pairs.ElementAt(Game1.random.Next(this.terrainFeatures.Count())).Value;
          if (terrainFeature is Tree && (int) (NetFieldBase<int, NetInt>) (terrainFeature as Tree).growthStage >= 5 && !(bool) (NetFieldBase<bool, NetBool>) (terrainFeature as Tree).tapped)
          {
            (terrainFeature as Tree).treeType.Value = 7;
            (terrainFeature as Tree).loadSprite();
            break;
          }
        }
      }
      this.addCrows();
      if (!Game1.currentSeason.Equals("winter"))
        this.spawnWeedsAndStones(Game1.currentSeason.Equals("summer") ? 30 : 20);
      this.spawnWeeds(false);
      this.HandleGrassGrowth(dayOfMonth);
    }

    public void doDailyMountainFarmUpdate()
    {
      double num1 = 1.0;
      while (Game1.random.NextDouble() < num1)
      {
        Vector2 zero = Vector2.Zero;
        Vector2 vector2 = !this.ShouldSpawnMountainOres() ? (Game1.whichFarm == 5 ? Utility.getRandomPositionInThisRectangle(new Microsoft.Xna.Framework.Rectangle(51, 67, 11, 3), Game1.random) : Utility.getRandomPositionInThisRectangle(new Microsoft.Xna.Framework.Rectangle(5, 37, 22, 8), Game1.random)) : Utility.getRandomPositionInThisRectangle(this._mountainForageRectangle.Value, Game1.random);
        if (this.doesTileHavePropertyNoNull((int) vector2.X, (int) vector2.Y, "Type", "Back").Equals("Dirt") && this.isTileLocationTotallyClearAndPlaceable(vector2))
        {
          int parentSheetIndex = 668;
          int num2 = 2;
          if (Game1.random.NextDouble() < 0.15)
          {
            this.objects.Add(vector2, new Object(vector2, 590, 1));
            continue;
          }
          if (Game1.random.NextDouble() < 0.5)
            parentSheetIndex = 670;
          if (Game1.random.NextDouble() < 0.1)
          {
            if (Game1.player.MiningLevel >= 8 && Game1.random.NextDouble() < 0.33)
            {
              parentSheetIndex = 77;
              num2 = 7;
            }
            else if (Game1.player.MiningLevel >= 5 && Game1.random.NextDouble() < 0.5)
            {
              parentSheetIndex = 76;
              num2 = 5;
            }
            else
            {
              parentSheetIndex = 75;
              num2 = 3;
            }
          }
          if (Game1.random.NextDouble() < 0.21)
          {
            parentSheetIndex = 751;
            num2 = 3;
          }
          if (Game1.player.MiningLevel >= 4 && Game1.random.NextDouble() < 0.15)
          {
            parentSheetIndex = 290;
            num2 = 4;
          }
          if (Game1.player.MiningLevel >= 7 && Game1.random.NextDouble() < 0.1)
          {
            parentSheetIndex = 764;
            num2 = 8;
          }
          if (Game1.player.MiningLevel >= 10 && Game1.random.NextDouble() < 0.01)
          {
            parentSheetIndex = 765;
            num2 = 16;
          }
          this.objects.Add(vector2, new Object(vector2, parentSheetIndex, 10)
          {
            MinutesUntilReady = num2
          });
        }
        num1 *= 0.75;
      }
    }

    public override bool catchOceanCrabPotFishFromThisSpot(int x, int y)
    {
      if (this.map != null)
      {
        if (!this._oceanCrabPotOverride.HasValue)
          this._oceanCrabPotOverride = new bool?(this.map.Properties.ContainsKey("FarmOceanCrabPotOverride"));
        if (this._oceanCrabPotOverride.Value)
          return true;
      }
      if (Game1.whichFarm != 6)
        return base.catchOceanCrabPotFishFromThisSpot(x, y);
      return x <= 28 || x >= 57 || y <= 46 || y >= 82;
    }

    public override float getExtraTrashChanceForCrabPot(int x, int y)
    {
      if (Game1.whichFarm != 6)
        return base.getExtraTrashChanceForCrabPot(x, y);
      return x > 28 && x < 57 && y > 46 && y < 82 ? 0.25f : 0.0f;
    }

    public void addCrows()
    {
      int num1 = 0;
      foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.terrainFeatures.Pairs)
      {
        if (pair.Value is HoeDirt && (pair.Value as HoeDirt).crop != null)
          ++num1;
      }
      List<Vector2> vector2List = new List<Vector2>();
      foreach (KeyValuePair<Vector2, Object> pair in this.objects.Pairs)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) pair.Value.bigCraftable && pair.Value.IsScarecrow())
          vector2List.Add(pair.Key);
      }
      int num2 = Math.Min(4, num1 / 16);
      for (int index1 = 0; index1 < num2; ++index1)
      {
        if (Game1.random.NextDouble() < 0.3)
        {
          for (int index2 = 0; index2 < 10; ++index2)
          {
            Vector2 key1 = this.terrainFeatures.Pairs.ElementAt(Game1.random.Next(this.terrainFeatures.Count())).Key;
            if (this.terrainFeatures[key1] is HoeDirt && (this.terrainFeatures[key1] as HoeDirt).crop != null && (int) (NetFieldBase<int, NetInt>) (this.terrainFeatures[key1] as HoeDirt).crop.currentPhase > 1)
            {
              bool flag = false;
              foreach (Vector2 key2 in vector2List)
              {
                int radiusForScarecrow = this.objects[key2].GetRadiusForScarecrow();
                if ((double) Vector2.Distance(key2, key1) < (double) radiusForScarecrow)
                {
                  flag = true;
                  ++this.objects[key2].SpecialVariable;
                  break;
                }
              }
              if (!flag)
              {
                (this.terrainFeatures[key1] as HoeDirt).destroyCrop(key1, false, (GameLocation) this);
                this.spawnCrowEvent.Fire(key1);
                break;
              }
              break;
            }
          }
        }
      }
    }

    private void doSpawnCrow(Vector2 v)
    {
      if (this.critters == null && (bool) (NetFieldBase<bool, NetBool>) this.isOutdoors)
        this.critters = new List<Critter>();
      this.critters.Add((Critter) new Crow((int) v.X, (int) v.Y));
    }

    public static Point getFrontDoorPositionForFarmer(Farmer who)
    {
      Point mainFarmHouseEntry = Game1.getFarm().GetMainFarmHouseEntry();
      --mainFarmHouseEntry.Y;
      return mainFarmHouseEntry;
    }

    public override void performTenMinuteUpdate(int timeOfDay)
    {
      base.performTenMinuteUpdate(timeOfDay);
      if (timeOfDay >= 1300 && Game1.IsMasterGame)
      {
        foreach (NPC npc in new List<Character>((IEnumerable<Character>) this.characters))
        {
          if (npc.isMarried())
            npc.returnHomeFromFarmPosition(this);
        }
      }
      foreach (NPC character in this.characters)
      {
        if (character.getSpouse() == Game1.player)
          character.checkForMarriageDialogue(timeOfDay, (GameLocation) this);
        if (character is Child)
          (character as Child).tenMinuteUpdate();
      }
      if (!Game1.spawnMonstersAtNight || Game1.farmEvent != null || Game1.timeOfDay < 1900 || Game1.random.NextDouble() >= 0.25 - Game1.player.team.AverageDailyLuck() / 2.0)
        return;
      if (Game1.random.NextDouble() < 0.25)
      {
        if (!this.Equals(Game1.currentLocation))
          return;
        this.spawnFlyingMonstersOffScreen();
      }
      else
        this.spawnGroundMonsterOffScreen();
    }

    public void spawnGroundMonsterOffScreen()
    {
      for (int index = 0; index < 15; ++index)
      {
        Vector2 zero = Vector2.Zero;
        Vector2 randomTile = this.getRandomTile();
        if (Utility.isOnScreen(Utility.Vector2ToPoint(randomTile), 64, (GameLocation) this))
          randomTile.X -= (float) (Game1.viewport.Width / 64);
        if (this.isTileLocationTotallyClearAndPlaceable(randomTile))
        {
          bool flag;
          if (Game1.player.CombatLevel >= 8 && Game1.random.NextDouble() < 0.15)
          {
            NetCollection<NPC> characters = this.characters;
            ShadowBrute shadowBrute = new ShadowBrute(randomTile * 64f);
            shadowBrute.focusedOnFarmers = true;
            shadowBrute.wildernessFarmMonster = true;
            characters.Add((NPC) shadowBrute);
            flag = true;
          }
          else if (Game1.random.NextDouble() < (Game1.whichFarm == 4 ? 0.66 : 0.33) && this.isTileLocationTotallyClearAndPlaceable(randomTile))
          {
            NetCollection<NPC> characters = this.characters;
            RockGolem rockGolem = new RockGolem(randomTile * 64f, Game1.player.CombatLevel);
            rockGolem.wildernessFarmMonster = true;
            characters.Add((NPC) rockGolem);
            flag = true;
          }
          else
          {
            int mineLevel = 1;
            if (Game1.player.CombatLevel >= 10)
              mineLevel = 140;
            else if (Game1.player.CombatLevel >= 8)
              mineLevel = 100;
            else if (Game1.player.CombatLevel >= 4)
              mineLevel = 41;
            NetCollection<NPC> characters = this.characters;
            GreenSlime greenSlime = new GreenSlime(randomTile * 64f, mineLevel);
            greenSlime.wildernessFarmMonster = true;
            characters.Add((NPC) greenSlime);
            flag = true;
          }
          if (!flag || !Game1.currentLocation.Equals((GameLocation) this))
            break;
          using (OverlaidDictionary.PairsCollection.Enumerator enumerator = this.objects.Pairs.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<Vector2, Object> current = enumerator.Current;
              if (current.Value != null && (bool) (NetFieldBase<bool, NetBool>) current.Value.bigCraftable && (int) (NetFieldBase<int, NetInt>) current.Value.parentSheetIndex == 83)
              {
                current.Value.shakeTimer = 1000;
                current.Value.showNextIndex.Value = true;
                Game1.currentLightSources.Add(new LightSource(4, current.Key * 64f + new Vector2(32f, 0.0f), 1f, Color.Cyan * 0.75f, (int) ((double) current.Key.X * 797.0 + (double) current.Key.Y * 13.0 + 666.0)));
              }
            }
            break;
          }
        }
      }
    }

    public void spawnFlyingMonstersOffScreen()
    {
      Vector2 zero = Vector2.Zero;
      switch (Game1.random.Next(4))
      {
        case 0:
          zero.X = (float) Game1.random.Next(this.map.Layers[0].LayerWidth);
          break;
        case 1:
          zero.X = (float) (this.map.Layers[0].LayerWidth - 1);
          zero.Y = (float) Game1.random.Next(this.map.Layers[0].LayerHeight);
          break;
        case 2:
          zero.Y = (float) (this.map.Layers[0].LayerHeight - 1);
          zero.X = (float) Game1.random.Next(this.map.Layers[0].LayerWidth);
          break;
        case 3:
          zero.Y = (float) Game1.random.Next(this.map.Layers[0].LayerHeight);
          break;
      }
      if (Utility.isOnScreen(zero * 64f, 64))
        zero.X -= (float) Game1.viewport.Width;
      bool flag;
      if (Game1.player.CombatLevel >= 10 && Game1.random.NextDouble() < 0.01 && Game1.player.hasItemInInventoryNamed("Galaxy Sword"))
      {
        NetCollection<NPC> characters = this.characters;
        Bat bat = new Bat(zero * 64f, 9999);
        bat.focusedOnFarmers = true;
        bat.wildernessFarmMonster = true;
        characters.Add((NPC) bat);
        flag = true;
      }
      else if (Game1.player.CombatLevel >= 10 && Game1.random.NextDouble() < 0.25)
      {
        NetCollection<NPC> characters = this.characters;
        Bat bat = new Bat(zero * 64f, 172);
        bat.focusedOnFarmers = true;
        bat.wildernessFarmMonster = true;
        characters.Add((NPC) bat);
        flag = true;
      }
      else if (Game1.player.CombatLevel >= 10 && Game1.random.NextDouble() < 0.25)
      {
        NetCollection<NPC> characters = this.characters;
        Serpent serpent = new Serpent(zero * 64f);
        serpent.focusedOnFarmers = true;
        serpent.wildernessFarmMonster = true;
        characters.Add((NPC) serpent);
        flag = true;
      }
      else if (Game1.player.CombatLevel >= 8 && Game1.random.NextDouble() < 0.5)
      {
        NetCollection<NPC> characters = this.characters;
        Bat bat = new Bat(zero * 64f, 81);
        bat.focusedOnFarmers = true;
        bat.wildernessFarmMonster = true;
        characters.Add((NPC) bat);
        flag = true;
      }
      else if (Game1.player.CombatLevel >= 5 && Game1.random.NextDouble() < 0.5)
      {
        NetCollection<NPC> characters = this.characters;
        Bat bat = new Bat(zero * 64f, 41);
        bat.focusedOnFarmers = true;
        bat.wildernessFarmMonster = true;
        characters.Add((NPC) bat);
        flag = true;
      }
      else
      {
        NetCollection<NPC> characters = this.characters;
        Bat bat = new Bat(zero * 64f, 1);
        bat.focusedOnFarmers = true;
        bat.wildernessFarmMonster = true;
        characters.Add((NPC) bat);
        flag = true;
      }
      if (!flag || !Game1.currentLocation.Equals((GameLocation) this))
        return;
      foreach (KeyValuePair<Vector2, Object> pair in this.objects.Pairs)
      {
        if (pair.Value != null && (bool) (NetFieldBase<bool, NetBool>) pair.Value.bigCraftable && (int) (NetFieldBase<int, NetInt>) pair.Value.parentSheetIndex == 83)
        {
          pair.Value.shakeTimer = 1000;
          pair.Value.showNextIndex.Value = true;
          Game1.currentLightSources.Add(new LightSource(4, pair.Key * 64f + new Vector2(32f, 0.0f), 1f, Color.Cyan * 0.75f, (int) ((double) pair.Key.X * 797.0 + (double) pair.Key.Y * 13.0 + 666.0)));
        }
      }
    }

    public override bool performToolAction(Tool t, int tileX, int tileY)
    {
      Point point = new Point(tileX * 64 + 32, tileY * 64 + 32);
      if (t is MeleeWeapon)
      {
        foreach (FarmAnimal farmAnimal in this.animals.Values)
        {
          if (farmAnimal.GetBoundingBox().Intersects((t as MeleeWeapon).mostRecentArea))
            farmAnimal.hitWithWeapon(t as MeleeWeapon);
        }
      }
      if (t is WateringCan && (t as WateringCan).WaterLeft > 0 && this.getTileIndexAt(tileX, tileY, "Buildings") == 1938 && !this.petBowlWatered.Value)
      {
        this.petBowlWatered.Set(true);
        this._UpdateWaterBowl();
      }
      return base.performToolAction(t, tileX, tileY);
    }

    public override void timeUpdate(int timeElapsed)
    {
      base.timeUpdate(timeElapsed);
      if (Game1.IsMasterGame)
      {
        foreach (FarmAnimal farmAnimal in this.animals.Values)
          farmAnimal.updatePerTenMinutes(Game1.timeOfDay, (GameLocation) this);
      }
      foreach (Building building in this.buildings)
      {
        if ((int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0)
        {
          building.performTenMinuteAction(timeElapsed);
          if (building.indoors.Value != null && !Game1.locations.Contains(building.indoors.Value) && timeElapsed >= 10)
          {
            building.indoors.Value.performTenMinuteUpdate(Game1.timeOfDay);
            if (timeElapsed > 10)
              building.indoors.Value.passTimeForObjects(timeElapsed - 10);
          }
        }
      }
    }

    public bool placeAnimal(
      BluePrint blueprint,
      Vector2 tileLocation,
      bool serverCommand,
      long ownerID)
    {
      for (int index1 = 0; index1 < blueprint.tilesHeight; ++index1)
      {
        for (int index2 = 0; index2 < blueprint.tilesWidth; ++index2)
        {
          Vector2 vector2 = new Vector2(tileLocation.X + (float) index2, tileLocation.Y + (float) index1);
          if (Game1.player.getTileLocation().Equals(vector2) || this.isTileOccupied(vector2, "", false) || !this.isTilePassable(new Location((int) vector2.X, (int) vector2.Y), Game1.viewport))
            return false;
        }
      }
      long newId = Game1.multiplayer.getNewID();
      FarmAnimal farmAnimal = new FarmAnimal(blueprint.name, newId, ownerID);
      farmAnimal.Position = new Vector2((float) ((double) tileLocation.X * 64.0 + 4.0), (float) ((double) tileLocation.Y * 64.0 + 64.0 - (double) farmAnimal.Sprite.getHeight() - 4.0));
      this.animals.Add(newId, farmAnimal);
      if (farmAnimal.sound.Value != null && !farmAnimal.sound.Value.Equals(""))
        this.localSound((string) (NetFieldBase<string, NetString>) farmAnimal.sound);
      return true;
    }

    /// <summary>
    /// returns the leftover hay that didn't make it into the silos.
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public int tryToAddHay(int num)
    {
      int num1 = Math.Min(Utility.numSilos() * 240 - (int) (NetFieldBase<int, NetInt>) this.piecesOfHay, num);
      this.piecesOfHay.Value += num1;
      return num - num1;
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
      if (!glider)
      {
        if (this.resourceClumps.Count > 0)
        {
          Microsoft.Xna.Framework.Rectangle rectangle = character != null ? character.GetBoundingBox() : Microsoft.Xna.Framework.Rectangle.Empty;
          foreach (ResourceClump resourceClump in this.resourceClumps)
          {
            Microsoft.Xna.Framework.Rectangle boundingBox = resourceClump.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) resourceClump.tile);
            if (boundingBox.Intersects(position) && (!isFarmer || character == null || !boundingBox.Intersects(rectangle)))
              return true;
          }
        }
        switch (character)
        {
          case null:
          case FarmAnimal _:
            break;
          default:
            Microsoft.Xna.Framework.Rectangle boundingBox1 = Game1.player.GetBoundingBox();
            Farmer farmer = isFarmer ? character as Farmer : (Farmer) null;
            using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.ValuesCollection.Enumerator enumerator = this.animals.Values.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                FarmAnimal current = enumerator.Current;
                if (position.Intersects(current.GetBoundingBox()) && (!isFarmer || !boundingBox1.Intersects(current.GetBoundingBox())))
                {
                  if (farmer != null)
                  {
                    if (farmer.TemporaryPassableTiles.Intersects(position))
                      break;
                  }
                  current.farmerPushing();
                  return true;
                }
              }
              break;
            }
        }
      }
      return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement);
    }

    public bool CheckPetAnimal(Vector2 position, Farmer who)
    {
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) pair.Value.wasPet && pair.Value.GetCursorPetBoundingBox().Contains((int) position.X, (int) position.Y))
        {
          pair.Value.pet(who);
          return true;
        }
      }
      return false;
    }

    public bool CheckPetAnimal(Microsoft.Xna.Framework.Rectangle rect, Farmer who)
    {
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) pair.Value.wasPet && pair.Value.GetBoundingBox().Intersects(rect))
        {
          pair.Value.pet(who);
          return true;
        }
      }
      return false;
    }

    public bool CheckInspectAnimal(Vector2 position, Farmer who)
    {
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) pair.Value.wasPet && pair.Value.GetCursorPetBoundingBox().Contains((int) position.X, (int) position.Y))
        {
          pair.Value.pet(who);
          return true;
        }
      }
      return false;
    }

    public bool CheckInspectAnimal(Microsoft.Xna.Framework.Rectangle rect, Farmer who)
    {
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) pair.Value.wasPet && pair.Value.GetBoundingBox().Intersects(rect))
        {
          pair.Value.pet(who);
          return true;
        }
      }
      return false;
    }

    public virtual void requestGrandpaReevaluation()
    {
      this.grandpaScore.Value = 0;
      if (Game1.IsMasterGame)
      {
        Game1.player.eventsSeen.Remove(558292);
        Game1.player.eventsSeen.Add(321777);
      }
      this.removeTemporarySpritesWithID(6666);
    }

    public override void OnMapLoad(Map map)
    {
      this.CacheOffBasePatioArea();
      base.OnMapLoad(map);
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64);
      if (!this.objects.ContainsKey(new Vector2((float) tileLocation.X, (float) tileLocation.Y)) && this.CheckPetAnimal(rect, who))
        return true;
      Point grandpaShrinePosition = this.GetGrandpaShrinePosition();
      if (tileLocation.X >= grandpaShrinePosition.X - 1 && tileLocation.X <= grandpaShrinePosition.X + 1 && tileLocation.Y == grandpaShrinePosition.Y)
      {
        if (!this.hasSeenGrandpaNote)
        {
          Game1.addMail("hasSeenGrandpaNote", true);
          this.hasSeenGrandpaNote = true;
          Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(Game1.content.LoadString("Strings\\Locations:Farm_GrandpaNote", (object) Game1.player.Name).Replace('\n', '^'));
          return true;
        }
        if (Game1.year >= 3 && (int) (NetFieldBase<int, NetInt>) this.grandpaScore > 0 && (int) (NetFieldBase<int, NetInt>) this.grandpaScore < 4)
        {
          if (who.ActiveObject != null && (int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex == 72 && (int) (NetFieldBase<int, NetInt>) this.grandpaScore < 4)
          {
            who.reduceActiveItemByOne();
            this.playSound("stoneStep");
            this.playSound("fireball");
            DelayedAction.playSoundAfterDelay("yoba", 800, (GameLocation) this);
            DelayedAction.showDialogueAfterDelay(Game1.content.LoadString("Strings\\Locations:Farm_GrandpaShrine_PlaceDiamond"), 1200);
            Game1.multiplayer.broadcastGrandpaReevaluation();
            Game1.player.freezePause = 1200;
            return true;
          }
          if (who.ActiveObject == null || (int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex != 72)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Farm_GrandpaShrine_DiamondSlot"));
            return true;
          }
        }
        else
        {
          if ((int) (NetFieldBase<int, NetInt>) this.grandpaScore >= 4 && !Utility.doesItemWithThisIndexExistAnywhere(160, true))
          {
            who.addItemByMenuIfNecessaryElseHoldUp((Item) new Object(Vector2.Zero, 160), new ItemGrabMenu.behaviorOnItemSelect(this.grandpaStatueCallback));
            return true;
          }
          if ((int) (NetFieldBase<int, NetInt>) this.grandpaScore == 0 && Game1.year >= 3)
          {
            Game1.player.eventsSeen.Remove(558292);
            if (!Game1.player.eventsSeen.Contains(321777))
              Game1.player.eventsSeen.Add(321777);
          }
        }
      }
      return base.checkAction(tileLocation, viewport, who) || Game1.didPlayerJustRightClick(true) && this.CheckInspectAnimal(rect, who);
    }

    public void grandpaStatueCallback(Item item, Farmer who)
    {
      if (item == null || !(item is Object) || !(bool) (NetFieldBase<bool, NetBool>) (item as Object).bigCraftable || (int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex != 160 || who == null)
        return;
      who.mailReceived.Add("grandpaPerfect");
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      base.TransferDataFromSavedLocation(l);
      this.housePaintColor.Value = (l as Farm).housePaintColor.Value;
      this.farmCaveReady.Value = (l as Farm).farmCaveReady.Value;
      if (!(l as Farm).hasSeenGrandpaNote)
        return;
      Game1.addMail("hasSeenGrandpaNote", true);
    }

    public NetCollection<Item> getShippingBin(Farmer who) => (bool) (NetFieldBase<bool, NetBool>) Game1.player.team.useSeparateWallets ? who.personalShippingBin : this.sharedShippingBin;

    public void shipItem(Item i, Farmer who)
    {
      if (i == null)
        return;
      who.removeItemFromInventory(i);
      this.getShippingBin(who).Add(i);
      if (i is Object)
        this.showShipment(i as Object, false);
      this.lastItemShipped = i;
      if (Game1.player.ActiveObject != null)
        return;
      Game1.player.showNotCarrying();
      Game1.player.Halt();
    }

    public override bool leftClick(int x, int y, Farmer who) => base.leftClick(x, y, who);

    public void showShipment(Object o, bool playThrowSound = true)
    {
      if (playThrowSound)
        this.localSound("backpackIN");
      DelayedAction.playSoundAfterDelay("Ship", playThrowSound ? 250 : 0);
      int num = Game1.random.Next();
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(524, 218, 34, 22), new Vector2(71f, 13f) * 64f + new Vector2(0.0f, 5f) * 4f, false, 0.0f, Color.White)
      {
        interval = 100f,
        totalNumberOfLoops = 1,
        animationLength = 3,
        pingPong = true,
        scale = 4f,
        layerDepth = 0.09601f,
        id = (float) num,
        extraInfoForEndBehavior = num,
        endFunction = new TemporaryAnimatedSprite.endBehavior(((GameLocation) this).removeTemporarySpritesWithID)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(524, 230, 34, 10), new Vector2(71f, 13f) * 64f + new Vector2(0.0f, 17f) * 4f, false, 0.0f, Color.White)
      {
        interval = 100f,
        totalNumberOfLoops = 1,
        animationLength = 3,
        pingPong = true,
        scale = 4f,
        layerDepth = 0.0963f,
        id = (float) num,
        extraInfoForEndBehavior = num
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) o.parentSheetIndex, 16, 16), new Vector2(71f, 13f) * 64f + new Vector2((float) (8 + Game1.random.Next(6)), 2f) * 4f, false, 0.0f, Color.White)
      {
        interval = 9999f,
        scale = 4f,
        alphaFade = 0.045f,
        layerDepth = 0.096225f,
        motion = new Vector2(0.0f, 0.3f),
        acceleration = new Vector2(0.0f, 0.2f),
        scaleChange = -0.05f
      });
    }

    public override int getFishingLocation(Vector2 tile)
    {
      switch (Game1.whichFarm)
      {
        case 1:
        case 2:
        case 5:
          return 1;
        case 3:
          return 0;
        default:
          return -1;
      }
    }

    public override bool doesTileSinkDebris(int tileX, int tileY, Debris.DebrisType type) => this.isTileBuildingFishable(tileX, tileY) || base.doesTileSinkDebris(tileX, tileY, type);

    public override bool CanRefillWateringCanOnTile(int tileX, int tileY)
    {
      Building buildingAt = this.getBuildingAt(new Vector2((float) tileX, (float) tileY));
      return buildingAt != null && buildingAt.CanRefillWateringCan() || base.CanRefillWateringCanOnTile(tileX, tileY);
    }

    public override bool isTileBuildingFishable(int tileX, int tileY)
    {
      Vector2 tile = new Vector2((float) tileX, (float) tileY);
      foreach (Building building in this.buildings)
      {
        if (building.isTileFishable(tile))
          return true;
      }
      return base.isTileBuildingFishable(tileX, tileY);
    }

    public override Object getFish(
      float millisecondsAfterNibble,
      int bait,
      int waterDepth,
      Farmer who,
      double baitPotency,
      Vector2 bobberTile,
      string location = null)
    {
      if (location != null && location != this.Name)
        return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, location);
      if (bobberTile != Vector2.Zero)
      {
        foreach (Building building in this.buildings)
        {
          if (building is FishPond && building.isTileFishable(bobberTile))
            return (building as FishPond).CatchFish();
        }
      }
      if (this._fishLocationOverride == null)
      {
        string mapProperty = this.getMapProperty("FarmFishLocationOverride");
        if (mapProperty == "")
        {
          this._fishLocationOverride = "";
          this._fishChanceOverride = 0.0f;
        }
        else
        {
          string[] strArray = mapProperty.Split(' ');
          try
          {
            this._fishLocationOverride = strArray[0];
            this._fishChanceOverride = float.Parse(strArray[1]);
          }
          catch (Exception ex)
          {
            this._fishLocationOverride = "";
            this._fishChanceOverride = 0.0f;
          }
        }
      }
      if ((double) this._fishChanceOverride > 0.0)
      {
        if (Game1.random.NextDouble() < (double) this._fishChanceOverride)
          return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, this._fishLocationOverride);
      }
      else
      {
        switch (Game1.whichFarm)
        {
          case 1:
            return Game1.random.NextDouble() < 0.3 ? base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "Forest") : base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "Town");
          case 2:
            if (Game1.random.NextDouble() < 0.05 + Game1.player.DailyLuck)
              return new Object(734, 1);
            if (Game1.random.NextDouble() < 0.45)
              return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "Forest");
            break;
          case 3:
            if (Game1.random.NextDouble() < 0.5)
              return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "Forest");
            break;
          case 4:
            if (Game1.random.NextDouble() <= 0.35)
              return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "Mountain");
            break;
          case 5:
            if (who != null && who.getTileX() < 40 && who.getTileY() > 54 && Game1.random.NextDouble() <= 0.5)
            {
              if (!who.mailReceived.Contains("cursed_doll") || who.mailReceived.Contains("eric's_prank_1"))
                return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "Forest");
              who.mailReceived.Add("eric's_prank_1");
              return new Object(103, 1);
            }
            break;
          case 6:
            if (who != null && who.getTileLocation().Equals(new Vector2(23f, 98f)) && !who.mailReceived.Contains("gotBoatPainting"))
            {
              who.mailReceived.Add("gotBoatPainting");
              return (Object) new Furniture(2421, Vector2.Zero);
            }
            if (!new Microsoft.Xna.Framework.Rectangle(26, 45, 31, 39).Contains((int) bobberTile.X, (int) bobberTile.Y))
            {
              if (Game1.random.NextDouble() < 0.15)
                return new Object(152, 1);
              if (Game1.random.NextDouble() < 0.06)
              {
                int parentSheetIndex = -1;
                switch (Game1.random.Next(4))
                {
                  case 0:
                    parentSheetIndex = 723;
                    break;
                  case 1:
                    parentSheetIndex = 393;
                    break;
                  case 2:
                    parentSheetIndex = 719;
                    break;
                  case 3:
                    parentSheetIndex = 718;
                    break;
                }
                return new Object(parentSheetIndex, 1);
              }
              if (Game1.random.NextDouble() < 0.66)
                return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "Beach");
              break;
            }
            break;
        }
      }
      return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile);
    }

    public List<FarmAnimal> getAllFarmAnimals()
    {
      List<FarmAnimal> list = this.animals.Values.ToList<FarmAnimal>();
      foreach (Building building in this.buildings)
      {
        if (building.indoors.Value != null && building.indoors.Value is AnimalHouse)
          list.AddRange((IEnumerable<FarmAnimal>) ((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors).animals.Values.ToList<FarmAnimal>());
      }
      return list;
    }

    public override bool isTileOccupied(
      Vector2 tileLocation,
      string characterToIgnore = "",
      bool ignoreAllCharacters = false)
    {
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
      {
        if (pair.Value.getTileLocation().Equals(tileLocation))
          return true;
      }
      return base.isTileOccupied(tileLocation, characterToIgnore, ignoreAllCharacters);
    }

    protected override void resetSharedState()
    {
      base.resetSharedState();
      if (!this.greenhouseUnlocked.Value && Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccPantry"))
        this.greenhouseUnlocked.Value = true;
      this.houseSource.Value = new Microsoft.Xna.Framework.Rectangle(0, 144 * ((int) (NetFieldBase<int, NetInt>) Game1.MasterPlayer.houseUpgradeLevel == 3 ? 2 : (int) (NetFieldBase<int, NetInt>) Game1.MasterPlayer.houseUpgradeLevel), 160, 144);
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        if (Game1.timeOfDay >= 1300 && this.characters[index].isMarried() && this.characters[index].controller == null)
        {
          this.characters[index].Halt();
          this.characters[index].drawOffset.Value = Vector2.Zero;
          this.characters[index].Sprite.StopAnimation();
          FarmHouse locationFromName = Game1.getLocationFromName(this.characters[index].getSpouse().homeLocation.Value) as FarmHouse;
          Game1.warpCharacter(this.characters[index], this.characters[index].getSpouse().homeLocation.Value, locationFromName.getKitchenStandingSpot());
          break;
        }
      }
    }

    public virtual void UpdatePatio()
    {
      if (Game1.MasterPlayer.isMarried() && Game1.MasterPlayer.spouse != null)
        this.addSpouseOutdoorArea(Game1.MasterPlayer.spouse);
      else
        this.addSpouseOutdoorArea("");
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      this.ClearGreenhouseGrassTiles();
      this.UpdatePatio();
      this._UpdateWaterBowl();
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.hasSeenGrandpaNote = Game1.player.hasOrWillReceiveMail("hasSeenGrandpaNote");
      this.frameHouseColor = new Color?();
      if (!Game1.player.mailReceived.Contains("button_tut_2"))
      {
        Game1.player.mailReceived.Add("button_tut_2");
        Game1.onScreenMenus.Add((IClickableMenu) new ButtonTutorialMenu(1));
      }
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        if (this.characters[index] is Child)
          (this.characters[index] as Child).resetForPlayerEntry((GameLocation) this);
        if (this.characters[index].isVillager() && this.characters[index].name.Equals((object) Game1.player.spouse))
          this.petBowlWatered.Set(true);
      }
      if (Game1.timeOfDay >= 1830)
      {
        for (int index = this.animals.Count() - 1; index >= 0; --index)
          this.animals.Pairs.ElementAt(index).Value.warpHome(this, this.animals.Pairs.ElementAt(index).Value);
      }
      if (this.isThereABuildingUnderConstruction() && (int) (NetFieldBase<int, NetInt>) this.getBuildingUnderConstruction().daysOfConstructionLeft > 0 && Game1.getCharacterFromName("Robin").currentLocation.Equals((GameLocation) this))
      {
        Building underConstruction = this.getBuildingUnderConstruction();
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(399, 262, (int) (NetFieldBase<int, NetInt>) underConstruction.daysOfConstructionLeft == 1 ? 29 : 9, 43), new Vector2((float) ((int) (NetFieldBase<int, NetInt>) underConstruction.tileX + (int) (NetFieldBase<int, NetInt>) underConstruction.tilesWide / 2), (float) ((int) (NetFieldBase<int, NetInt>) underConstruction.tileY + (int) (NetFieldBase<int, NetInt>) underConstruction.tilesHigh / 2)) * 64f + new Vector2(-16f, -144f), false, 0.0f, Color.White)
        {
          id = 16846f,
          scale = 4f,
          interval = 999999f,
          animationLength = 1,
          totalNumberOfLoops = 99999,
          layerDepth = (float) (((int) (NetFieldBase<int, NetInt>) underConstruction.tileY + (int) (NetFieldBase<int, NetInt>) underConstruction.tilesHigh / 2) * 64 + 32) / 10000f
        });
      }
      else
        this.removeTemporarySpritesWithIDLocal(16846f);
      this.addGrandpaCandles();
      if (!Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") || Game1.player.mailReceived.Contains("Farm_Eternal_Parrots") || Game1.IsRainingHere((GameLocation) this))
        return;
      for (int index = 0; index < 20; ++index)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Microsoft.Xna.Framework.Rectangle(49, 24 * Game1.random.Next(4), 24, 24), new Vector2((float) Game1.viewport.MaxCorner.X, (float) (Game1.viewport.Location.Y + Game1.random.Next(64, Game1.viewport.Height / 2))), false, 0.0f, Color.White)
        {
          scale = 4f,
          motion = new Vector2((float) ((double) Game1.random.Next(-10, 11) / 10.0 - 5.0), (float) (4.0 + (double) Game1.random.Next(-10, 11) / 10.0)),
          acceleration = new Vector2(0.0f, -0.02f),
          animationLength = 3,
          interval = 100f,
          pingPong = true,
          totalNumberOfLoops = 999,
          delayBeforeAnimationStart = index * 250,
          drawAboveAlwaysFront = true,
          startSound = "batFlap"
        });
      DelayedAction.playSoundAfterDelay("parrot_squawk", 1000);
      DelayedAction.playSoundAfterDelay("parrot_squawk", 4000);
      DelayedAction.playSoundAfterDelay("parrot", 3000);
      DelayedAction.playSoundAfterDelay("parrot", 5500);
      DelayedAction.playSoundAfterDelay("parrot_squawk", 7000);
      for (int index = 0; index < 20; ++index)
        DelayedAction.playSoundAfterDelay("batFlap", 5000 + index * 250);
      Game1.player.mailReceived.Add("Farm_Eternal_Parrots");
    }

    public virtual Vector2 GetSpouseOutdoorAreaCorner()
    {
      if (!this.mapSpouseAreaCorner.HasValue)
      {
        int default_x = 69;
        int default_y = 6;
        if (Game1.whichFarm == 6)
        {
          default_x = 79;
          default_y = 2;
        }
        this.mapSpouseAreaCorner = new Vector2?(Utility.PointToVector2(this.GetMapPropertyPosition("SpouseAreaLocation", default_x, default_y)));
      }
      return this.mapSpouseAreaCorner.Value;
    }

    public virtual int GetSpouseOutdoorAreaSpritesheetIndex() => 1;

    public virtual void CacheOffBasePatioArea()
    {
      this._baseSpouseAreaTiles = new Dictionary<string, Dictionary<Point, Tile>>();
      List<string> stringList = new List<string>();
      foreach (Layer layer in this.map.Layers)
        stringList.Add(layer.Id);
      foreach (string str in stringList)
      {
        Layer layer = this.map.GetLayer(str);
        Dictionary<Point, Tile> dictionary = new Dictionary<Point, Tile>();
        this._baseSpouseAreaTiles[str] = dictionary;
        Vector2 outdoorAreaCorner = this.GetSpouseOutdoorAreaCorner();
        for (int x = (int) outdoorAreaCorner.X; x < (int) outdoorAreaCorner.X + 4; ++x)
        {
          for (int y = (int) outdoorAreaCorner.Y; y < (int) outdoorAreaCorner.Y + 4; ++y)
          {
            if (layer == null)
              dictionary[new Point(x, y)] = (Tile) null;
            else
              dictionary[new Point(x, y)] = layer.Tiles[x, y];
          }
        }
      }
    }

    public virtual void ReapplyBasePatioArea()
    {
      foreach (string key1 in this._baseSpouseAreaTiles.Keys)
      {
        Layer layer = this.map.GetLayer(key1);
        foreach (Point key2 in this._baseSpouseAreaTiles[key1].Keys)
        {
          Tile tile = this._baseSpouseAreaTiles[key1][key2];
          if (layer != null)
            layer.Tiles[key2.X, key2.Y] = tile;
        }
      }
    }

    public void addSpouseOutdoorArea(string spouseName)
    {
      this.ReapplyBasePatioArea();
      Point point1 = Utility.Vector2ToPoint(this.GetSpouseOutdoorAreaCorner());
      this.GetSpouseOutdoorAreaSpritesheetIndex();
      this.spousePatioSpot = new Point(point1.X + 2, point1.Y + 3);
      if (spouseName == null)
        return;
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\SpousePatios");
      int num1 = -1;
      string map_name = "spousePatios";
      if (dictionary != null)
      {
        if (dictionary.ContainsKey(spouseName))
        {
          try
          {
            string[] strArray = dictionary[spouseName].Split('/');
            map_name = strArray[0];
            num1 = int.Parse(strArray[1]);
          }
          catch (Exception ex)
          {
          }
        }
      }
      if (num1 < 0)
        return;
      int width = 4;
      int height = 4;
      Point point2 = point1;
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(point2.X, point2.Y, width, height);
      Map map = Game1.game1.xTileContent.Load<Map>("Maps\\" + map_name);
      int num2 = map.Layers[0].LayerWidth / width;
      int num3 = map.Layers[0].LayerHeight / height;
      Point point3 = new Point(num1 % num2 * width, num1 / num2 * height);
      if (this._appliedMapOverrides.Contains("spouse_patio"))
        this._appliedMapOverrides.Remove("spouse_patio");
      this.ApplyMapOverride(map_name, "spouse_patio", new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(point3.X, point3.Y, rectangle.Width, rectangle.Height)), new Microsoft.Xna.Framework.Rectangle?(rectangle));
      bool flag = false;
      for (int left = rectangle.Left; left < rectangle.Right; ++left)
      {
        for (int top = rectangle.Top; top < rectangle.Bottom; ++top)
        {
          if (this.getTileIndexAt(new Point(left, top), "Paths") == 7)
          {
            this.spousePatioSpot = new Point(left, top);
            flag = true;
            break;
          }
        }
        if (flag)
          break;
      }
    }

    public void addGrandpaCandles()
    {
      Point grandpaShrinePosition = this.GetGrandpaShrinePosition();
      if ((int) (NetFieldBase<int, NetInt>) this.grandpaScore > 0)
      {
        Microsoft.Xna.Framework.Rectangle sourceRect = new Microsoft.Xna.Framework.Rectangle(577, 1985, 2, 5);
        this.removeTemporarySpritesWithIDLocal(6666f);
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 99999f, 1, 9999, new Vector2((float) ((grandpaShrinePosition.X - 1) * 64 + 20), (float) ((grandpaShrinePosition.Y - 1) * 64 + 20)), false, false, (float) ((grandpaShrinePosition.Y - 1) * 64) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float) ((grandpaShrinePosition.X - 1) * 64 + 12), (float) ((grandpaShrinePosition.Y - 1) * 64 - 4)), false, 0.0f, Color.White)
        {
          interval = 50f,
          totalNumberOfLoops = 99999,
          animationLength = 7,
          light = true,
          id = 6666f,
          lightRadius = 1f,
          scale = 3f,
          layerDepth = 0.0385f,
          delayBeforeAnimationStart = 0
        });
        if ((int) (NetFieldBase<int, NetInt>) this.grandpaScore > 1)
        {
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 99999f, 1, 9999, new Vector2((float) ((grandpaShrinePosition.X - 1) * 64 + 40), (float) ((grandpaShrinePosition.Y - 2) * 64 + 24)), false, false, (float) ((grandpaShrinePosition.Y - 1) * 64) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float) ((grandpaShrinePosition.X - 1) * 64 + 36), (float) ((grandpaShrinePosition.Y - 2) * 64)), false, 0.0f, Color.White)
          {
            interval = 50f,
            totalNumberOfLoops = 99999,
            animationLength = 7,
            light = true,
            id = 6666f,
            lightRadius = 1f,
            scale = 3f,
            layerDepth = 0.0385f,
            delayBeforeAnimationStart = 50
          });
        }
        if ((int) (NetFieldBase<int, NetInt>) this.grandpaScore > 2)
        {
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 99999f, 1, 9999, new Vector2((float) ((grandpaShrinePosition.X + 1) * 64 + 20), (float) ((grandpaShrinePosition.Y - 2) * 64 + 24)), false, false, (float) ((grandpaShrinePosition.Y - 1) * 64) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float) ((grandpaShrinePosition.X + 1) * 64 + 16), (float) ((grandpaShrinePosition.Y - 2) * 64)), false, 0.0f, Color.White)
          {
            interval = 50f,
            totalNumberOfLoops = 99999,
            animationLength = 7,
            light = true,
            id = 6666f,
            lightRadius = 1f,
            scale = 3f,
            layerDepth = 0.0385f,
            delayBeforeAnimationStart = 100
          });
        }
        if ((int) (NetFieldBase<int, NetInt>) this.grandpaScore > 3)
        {
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 99999f, 1, 9999, new Vector2((float) ((grandpaShrinePosition.X + 1) * 64 + 40), (float) ((grandpaShrinePosition.Y - 1) * 64 + 20)), false, false, (float) ((grandpaShrinePosition.Y - 1) * 64) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float) ((grandpaShrinePosition.X + 1) * 64 + 36), (float) ((grandpaShrinePosition.Y - 1) * 64 - 4)), false, 0.0f, Color.White)
          {
            interval = 50f,
            totalNumberOfLoops = 99999,
            animationLength = 7,
            light = true,
            id = 6666f,
            lightRadius = 1f,
            scale = 3f,
            layerDepth = 0.0385f,
            delayBeforeAnimationStart = 150
          });
        }
      }
      if (!Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(176, 157, 15, 16), 99999f, 1, 9999, new Vector2((float) (grandpaShrinePosition.X * 64 + 4), (float) ((grandpaShrinePosition.Y - 2) * 64 - 24)), false, false, (float) ((grandpaShrinePosition.Y - 1) * 64) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
    }

    private void openShippingBinLid()
    {
      if (this.shippingBinLid == null)
        return;
      if (this.shippingBinLid.pingPongMotion != 1 && Game1.currentLocation == this)
        this.localSound("doorCreak");
      this.shippingBinLid.pingPongMotion = 1;
      this.shippingBinLid.paused = false;
    }

    private void closeShippingBinLid()
    {
      if (this.shippingBinLid == null || this.shippingBinLid.currentParentTileIndex <= 0)
        return;
      if (this.shippingBinLid.pingPongMotion != -1 && Game1.currentLocation == this)
        this.localSound("doorCreakReverse");
      this.shippingBinLid.pingPongMotion = -1;
      this.shippingBinLid.paused = false;
    }

    private void updateShippingBinLid(GameTime time)
    {
      if (this.isShippingBinLidOpen(true) && this.shippingBinLid.pingPongMotion == 1)
        this.shippingBinLid.paused = true;
      else if (this.shippingBinLid.currentParentTileIndex == 0 && this.shippingBinLid.pingPongMotion == -1)
      {
        if (!this.shippingBinLid.paused && Game1.currentLocation == this)
          this.localSound("woodyStep");
        this.shippingBinLid.paused = true;
      }
      this.shippingBinLid.update(time);
    }

    private bool isShippingBinLidOpen(bool requiredToBeFullyOpen = false) => this.shippingBinLid != null && this.shippingBinLid.currentParentTileIndex >= (requiredToBeFullyOpen ? this.shippingBinLid.animationLength - 1 : 1);

    public override void pokeTileForConstruction(Vector2 tile)
    {
      base.pokeTileForConstruction(tile);
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
      {
        if (pair.Value.getTileLocation().Equals(tile))
          pair.Value.Poke();
      }
      foreach (NPC character in this.characters)
      {
        if (character is Pet && character.getTileLocation() == tile)
        {
          Pet pet = character as Pet;
          pet.FacingDirection = Game1.random.Next(0, 4);
          pet.faceDirection(pet.FacingDirection);
          pet.CurrentBehavior = 0;
          pet.forceUpdateTimer = 2000;
          pet.setMovingInFacingDirection();
        }
      }
    }

    public override bool isTileOccupiedForPlacement(Vector2 tileLocation, Object toPlace = null)
    {
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
      {
        if (pair.Value.getTileLocation().Equals(tileLocation))
          return true;
      }
      return base.isTileOccupiedForPlacement(tileLocation, toPlace);
    }

    public override bool shouldShadowBeDrawnAboveBuildingsLayer(Vector2 p)
    {
      if (this.doesTileHaveProperty((int) p.X, (int) p.Y, "NoSpawn", "Back") == "All" && this.doesTileHaveProperty((int) p.X, (int) p.Y, "Type", "Back") == "Wood")
        return true;
      foreach (Building building in this.buildings)
      {
        if (building.occupiesTile(p) && building.isTilePassable(p))
          return true;
      }
      return base.shouldShadowBeDrawnAboveBuildingsLayer(p);
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
        pair.Value.draw(b);
      Point mainFarmHouseEntry = this.GetMainFarmHouseEntry();
      Vector2 vector2 = Utility.PointToVector2(mainFarmHouseEntry) * 64f;
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (mainFarmHouseEntry.X - 5), (float) (mainFarmHouseEntry.Y + 2)) * 64f), new Microsoft.Xna.Framework.Rectangle?(Building.leftShadow), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
      for (int index = 1; index < 8; ++index)
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (mainFarmHouseEntry.X - 5 + index), (float) (mainFarmHouseEntry.Y + 2)) * 64f), new Microsoft.Xna.Framework.Rectangle?(Building.middleShadow), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (mainFarmHouseEntry.X + 3), (float) (mainFarmHouseEntry.Y + 2)) * 64f), new Microsoft.Xna.Framework.Rectangle?(Building.rightShadow), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
      Texture2D texture = Farm.houseTextures;
      if (this.paintedHouseTexture != null)
        texture = this.paintedHouseTexture;
      Color white = Color.White;
      if (this.frameHouseColor.HasValue)
      {
        white = this.frameHouseColor.Value;
        this.frameHouseColor = new Color?();
      }
      Vector2 globalPosition = new Vector2(vector2.X - 384f, vector2.Y - 440f);
      b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Microsoft.Xna.Framework.Rectangle?((Microsoft.Xna.Framework.Rectangle) (NetFieldBase<Microsoft.Xna.Framework.Rectangle, NetRectangle>) this.houseSource), white, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 230.0) / 10000.0));
      if (Game1.mailbox.Count > 0)
      {
        float num1 = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
        Point mailboxPosition = Game1.player.getMailboxPosition();
        float num2 = (float) ((double) ((mailboxPosition.X + 1) * 64) / 10000.0 + (double) (mailboxPosition.Y * 64) / 10000.0);
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (mailboxPosition.X * 64), (float) (mailboxPosition.Y * 64 - 96 - 48) + num1)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num2 + 1E-06f);
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (mailboxPosition.X * 64 + 32 + 4), (float) (mailboxPosition.Y * 64 - 64 - 24 - 8) + num1)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(189, 423, 15, 13)), Color.White, 0.0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, num2 + 1E-05f);
      }
      if (this.shippingBinLid != null)
        this.shippingBinLid.draw(b);
      if (this.hasSeenGrandpaNote)
        return;
      Point grandpaShrinePosition = this.GetGrandpaShrinePosition();
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((grandpaShrinePosition.X + 1) * 64), (float) (grandpaShrinePosition.Y * 64))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(575, 1972, 11, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.044801f);
    }

    public virtual Point GetMainMailboxPosition()
    {
      if (!this.mapMainMailboxPosition.HasValue)
        this.mapMainMailboxPosition = new Point?(this.GetMapPropertyPosition("MailboxLocation", 68, 16));
      return this.mapMainMailboxPosition.Value;
    }

    public virtual Point GetGrandpaShrinePosition()
    {
      if (!this.mapGrandpaShrinePosition.HasValue)
        this.mapGrandpaShrinePosition = new Point?(this.GetMapPropertyPosition("GrandpaShrineLocation", 8, 7));
      return this.mapGrandpaShrinePosition.Value;
    }

    public virtual Point GetMainFarmHouseEntry()
    {
      if (!this.mainFarmhouseEntry.HasValue)
        this.mainFarmhouseEntry = new Point?(this.GetMapPropertyPosition("FarmHouseEntry", 64, 15));
      return this.mainFarmhouseEntry.Value;
    }

    public override void startEvent(Event evt)
    {
      if (evt.id != -2)
      {
        Point mainFarmHouseEntry = this.GetMainFarmHouseEntry();
        int x = mainFarmHouseEntry.X - 64;
        int y = mainFarmHouseEntry.Y - 15;
        evt.eventPositionTileOffset = new Vector2((float) x, (float) y);
      }
      base.startEvent(evt);
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b) => base.drawAboveAlwaysFrontLayer(b);

    public virtual void ApplyHousePaint()
    {
      if (this.paintedHouseTexture != null)
      {
        this.paintedHouseTexture.Dispose();
        this.paintedHouseTexture = (Texture2D) null;
      }
      this.paintedHouseTexture = BuildingPainter.Apply(Farm.houseTextures, "Buildings\\houses_PaintMask", (BuildingPaintColor) (NetFieldBase<BuildingPaintColor, NetRef<BuildingPaintColor>>) this.housePaintColor);
    }

    public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
    {
      this.spawnCrowEvent.Poll();
      this.lightningStrikeEvent.Poll();
      this.housePaintColor.Value.Poll(new Action(this.ApplyHousePaint));
      base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
      if (Game1.currentLocation.Equals((GameLocation) this))
        return;
      NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.PairsCollection pairs = this.animals.Pairs;
      for (int index = pairs.Count() - 1; index >= 0; --index)
        pairs.ElementAt(index).Value.updateWhenNotCurrentLocation((Building) null, time, (GameLocation) this);
    }

    public bool isTileOpenBesidesTerrainFeatures(Vector2 tile)
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = new Microsoft.Xna.Framework.Rectangle((int) tile.X * 64, (int) tile.Y * 64, 64, 64);
      foreach (Building building in this.buildings)
      {
        if (building.intersects(boundingBox))
          return false;
      }
      foreach (ResourceClump resourceClump in this.resourceClumps)
      {
        if (resourceClump.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) resourceClump.tile).Intersects(boundingBox))
          return false;
      }
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
      {
        if (pair.Value.getTileLocation().Equals(tile))
          return true;
      }
      return !this.objects.ContainsKey(tile) && this.isTilePassable(new Location((int) tile.X, (int) tile.Y), Game1.viewport);
    }

    private void doLightningStrike(Farm.LightningStrikeEvent lightning)
    {
      if (lightning.smallFlash)
      {
        if (Game1.currentLocation.IsOutdoors && !(Game1.currentLocation is Desert) && !Game1.newDay && Game1.netWorldState.Value.GetWeatherForLocation(Game1.currentLocation.GetLocationContext()).isLightning.Value)
        {
          Game1.flashAlpha = (float) (0.5 + Game1.random.NextDouble());
          if (Game1.random.NextDouble() < 0.5)
            DelayedAction.screenFlashAfterDelay((float) (0.3 + Game1.random.NextDouble()), Game1.random.Next(500, 1000));
          DelayedAction.playSoundAfterDelay("thunder_small", Game1.random.Next(500, 1500));
        }
      }
      else if (lightning.bigFlash && Game1.currentLocation.IsOutdoors && !(Game1.currentLocation is Desert) && Game1.netWorldState.Value.GetWeatherForLocation(Game1.currentLocation.GetLocationContext()).isLightning.Value && !Game1.newDay)
      {
        Game1.flashAlpha = (float) (0.5 + Game1.random.NextDouble());
        Game1.playSound("thunder");
      }
      if (!lightning.createBolt || !Game1.currentLocation.name.Equals((object) nameof (Farm)))
        return;
      if (lightning.destroyedTerrainFeature)
        this.temporarySprites.Add(new TemporaryAnimatedSprite(362, 75f, 6, 1, lightning.boltPosition, false, false));
      Utility.drawLightningBolt(lightning.boltPosition, (GameLocation) this);
    }

    public override bool CanBeRemotedlyViewed() => true;

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      if (this.wasUpdated && Game1.gameMode != (byte) 0)
        return;
      base.UpdateWhenCurrentLocation(time);
      this.chimneyTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.chimneyTimer <= 0)
      {
        FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(Game1.MasterPlayer);
        if (homeOfFarmer != null && homeOfFarmer.hasActiveFireplace())
        {
          Point porchStandingSpot = homeOfFarmer.getPorchStandingSpot();
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2((float) (porchStandingSpot.X * 64 + 4 * ((int) (NetFieldBase<int, NetInt>) Game1.MasterPlayer.houseUpgradeLevel >= 2 ? 9 : -5)), (float) (porchStandingSpot.Y * 64 - 420)), false, 1f / 500f, Color.Gray)
          {
            alpha = 0.75f,
            motion = new Vector2(0.0f, -0.5f),
            acceleration = new Vector2(1f / 500f, 0.0f),
            interval = 99999f,
            layerDepth = 1f,
            scale = 2f,
            scaleChange = 0.02f,
            rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0)
          });
        }
        for (int index = 0; index < this.buildings.Count; ++index)
        {
          if (this.buildings[index].indoors.Value is Cabin && (this.buildings[index].indoors.Value as Cabin).hasActiveFireplace())
            this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2((float) (((int) (NetFieldBase<int, NetInt>) this.buildings[index].tileX + 4) * 64 - 20), (float) (((int) (NetFieldBase<int, NetInt>) this.buildings[index].tileY + 3) * 64 - 420)), false, 1f / 500f, Color.Gray)
            {
              alpha = 0.75f,
              motion = new Vector2(0.0f, -0.5f),
              acceleration = new Vector2(1f / 500f, 0.0f),
              interval = 99999f,
              layerDepth = 1f,
              scale = 2f,
              scaleChange = 0.02f,
              rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0)
            });
        }
        this.chimneyTimer = 500;
      }
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
        this._tempAnimals.Add(pair);
      foreach (KeyValuePair<long, FarmAnimal> tempAnimal in this._tempAnimals)
      {
        if (tempAnimal.Value.updateWhenCurrentLocation(time, (GameLocation) this))
          this.animals.Remove(tempAnimal.Key);
      }
      this._tempAnimals.Clear();
      if (this.shippingBinLid == null)
        return;
      bool flag = false;
      foreach (Character farmer in this.farmers)
      {
        if (farmer.GetBoundingBox().Intersects(this.shippingBinLidOpenArea))
        {
          this.openShippingBinLid();
          flag = true;
        }
      }
      if (!flag)
        this.closeShippingBinLid();
      this.updateShippingBinLid(time);
    }

    public int getTotalCrops()
    {
      int totalCrops = 0;
      foreach (TerrainFeature terrainFeature in this.terrainFeatures.Values)
      {
        if (terrainFeature is HoeDirt && (terrainFeature as HoeDirt).crop != null && !(bool) (NetFieldBase<bool, NetBool>) (terrainFeature as HoeDirt).crop.dead)
          ++totalCrops;
      }
      return totalCrops;
    }

    public int getTotalCropsReadyForHarvest()
    {
      int cropsReadyForHarvest = 0;
      foreach (TerrainFeature terrainFeature in this.terrainFeatures.Values)
      {
        if (terrainFeature is HoeDirt && (terrainFeature as HoeDirt).readyForHarvest())
          ++cropsReadyForHarvest;
      }
      return cropsReadyForHarvest;
    }

    public int getTotalUnwateredCrops()
    {
      int totalUnwateredCrops = 0;
      foreach (TerrainFeature terrainFeature in this.terrainFeatures.Values)
      {
        if (terrainFeature is HoeDirt && (terrainFeature as HoeDirt).crop != null && (terrainFeature as HoeDirt).needsWatering() && (int) (NetFieldBase<int, NetInt>) (terrainFeature as HoeDirt).state != 1)
          ++totalUnwateredCrops;
      }
      return totalUnwateredCrops;
    }

    public int getTotalGreenhouseCropsReadyForHarvest()
    {
      if (!Game1.MasterPlayer.mailReceived.Contains("ccPantry"))
        return -1;
      int cropsReadyForHarvest = 0;
      foreach (TerrainFeature terrainFeature in Game1.getLocationFromName("Greenhouse").terrainFeatures.Values)
      {
        if (terrainFeature is HoeDirt && (terrainFeature as HoeDirt).readyForHarvest())
          ++cropsReadyForHarvest;
      }
      return cropsReadyForHarvest;
    }

    private GreenhouseBuilding getGreenhouseBuilding()
    {
      foreach (Building building in this.buildings)
      {
        if (building is GreenhouseBuilding)
          return building as GreenhouseBuilding;
      }
      return (GreenhouseBuilding) null;
    }

    public int getTotalOpenHoeDirt()
    {
      int totalOpenHoeDirt = 0;
      foreach (TerrainFeature terrainFeature in this.terrainFeatures.Values)
      {
        if (terrainFeature is HoeDirt && (terrainFeature as HoeDirt).crop == null && !this.objects.ContainsKey(terrainFeature.currentTileLocation))
          ++totalOpenHoeDirt;
      }
      return totalOpenHoeDirt;
    }

    public bool ShouldSpawnMountainOres()
    {
      if (!this._mountainForageRectangle.HasValue)
      {
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle();
        string[] strArray = this.getMapProperty("SpawnMountainFarmOreRect").Split(' ');
        if (strArray.Length == 4)
        {
          try
          {
            rectangle.X = int.Parse(strArray[0]);
            rectangle.Y = int.Parse(strArray[1]);
            rectangle.Width = int.Parse(strArray[2]);
            rectangle.Height = int.Parse(strArray[3]);
          }
          catch (Exception ex)
          {
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Width = 0;
            rectangle.Height = 0;
          }
        }
        this._mountainForageRectangle = new Microsoft.Xna.Framework.Rectangle?(rectangle);
      }
      return this._mountainForageRectangle.Value.Width > 0;
    }

    public bool ShouldSpawnForestFarmForage()
    {
      if (this.map != null)
      {
        if (!this._shouldSpawnForestFarmForage.HasValue)
          this._shouldSpawnForestFarmForage = new bool?(this.map.Properties.ContainsKey("SpawnForestFarmForage"));
        if (this._shouldSpawnForestFarmForage.Value)
          return true;
      }
      return Game1.whichFarm == 2;
    }

    public bool ShouldSpawnBeachFarmForage()
    {
      if (this.map != null)
      {
        if (!this._shouldSpawnBeachFarmForage.HasValue)
          this._shouldSpawnBeachFarmForage = new bool?(this.map.Properties.ContainsKey("SpawnBeachFarmForage"));
        if (this._shouldSpawnBeachFarmForage.Value)
          return true;
      }
      return Game1.whichFarm == 6;
    }

    public bool SpawnsForage() => this.ShouldSpawnForestFarmForage() || this.ShouldSpawnBeachFarmForage();

    public int getTotalForageItems()
    {
      int totalForageItems = 0;
      foreach (Object @object in this.objects.Values)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) @object.isSpawnedObject)
          ++totalForageItems;
      }
      return totalForageItems;
    }

    public int getNumberOfMachinesReadyForHarvest()
    {
      int machinesReadyForHarvest = 0;
      foreach (Object @object in this.objects.Values)
      {
        if (@object.IsConsideredReadyMachineForComputer())
          ++machinesReadyForHarvest;
      }
      foreach (Object @object in Game1.getLocationFromName("FarmHouse").objects.Values)
      {
        if (@object.IsConsideredReadyMachineForComputer())
          ++machinesReadyForHarvest;
      }
      foreach (Building building in this.buildings)
      {
        if (building.indoors.Value != null)
        {
          foreach (Object @object in building.indoors.Value.objects.Values)
          {
            if (@object.IsConsideredReadyMachineForComputer())
              ++machinesReadyForHarvest;
          }
        }
      }
      return machinesReadyForHarvest;
    }

    public bool doesFarmCaveNeedHarvesting() => this.farmCaveReady.Value;

    public class LightningStrikeEvent : NetEventArg
    {
      public Vector2 boltPosition;
      public bool createBolt;
      public bool bigFlash;
      public bool smallFlash;
      public bool destroyedTerrainFeature;

      public void Read(BinaryReader reader)
      {
        this.createBolt = reader.ReadBoolean();
        this.bigFlash = reader.ReadBoolean();
        this.smallFlash = reader.ReadBoolean();
        this.destroyedTerrainFeature = reader.ReadBoolean();
        this.boltPosition.X = (float) reader.ReadInt32();
        this.boltPosition.Y = (float) reader.ReadInt32();
      }

      public void Write(BinaryWriter writer)
      {
        writer.Write(this.createBolt);
        writer.Write(this.bigFlash);
        writer.Write(this.smallFlash);
        writer.Write(this.destroyedTerrainFeature);
        writer.Write((int) this.boltPosition.X);
        writer.Write((int) this.boltPosition.Y);
      }
    }
  }
}
