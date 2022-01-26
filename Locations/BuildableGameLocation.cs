// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.BuildableGameLocation
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using xTile;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class BuildableGameLocation : GameLocation
  {
    public readonly NetCollection<Building> buildings = new NetCollection<Building>();

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddField((INetSerializable) this.buildings);
      this.buildings.InterpolationWait = false;
    }

    public BuildableGameLocation()
    {
    }

    public BuildableGameLocation(string mapPath, string name)
      : base(mapPath, name)
    {
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      foreach (Building building in this.buildings)
        building.dayUpdate(dayOfMonth);
    }

    public override void cleanupBeforeSave()
    {
      foreach (Building building in this.buildings)
      {
        if (building.indoors.Value != null)
          building.indoors.Value.cleanupBeforeSave();
      }
    }

    public override bool performToolAction(Tool t, int tileX, int tileY)
    {
      foreach (Building building in this.buildings)
      {
        if (building.occupiesTile(new Vector2((float) tileX, (float) tileY)))
          building.performToolAction(t, tileX, tileY);
      }
      return base.performToolAction(t, tileX, tileY);
    }

    public virtual void timeUpdate(int timeElapsed)
    {
      foreach (Building building in this.buildings)
      {
        if (building.indoors.Value != null && building.indoors.Value is AnimalHouse)
        {
          foreach (KeyValuePair<long, FarmAnimal> pair in ((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors).animals.Pairs)
            pair.Value.updatePerTenMinutes(Game1.timeOfDay, (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors);
        }
      }
    }

    public Building getBuildingAt(Vector2 tile)
    {
      foreach (Building building in this.buildings)
      {
        if (!building.isTilePassable(tile))
          return building;
      }
      return (Building) null;
    }

    public Building getBuildingByName(string name)
    {
      foreach (Building building in this.buildings)
      {
        if (string.Equals(building.nameOfIndoors, name, StringComparison.Ordinal))
          return building;
      }
      return (Building) null;
    }

    public override bool leftClick(int x, int y, Farmer who)
    {
      foreach (Building building in this.buildings)
      {
        if (building.CanLeftClick(x, y))
          building.leftClicked();
      }
      return base.leftClick(x, y, who);
    }

    public bool destroyStructure(Vector2 tile)
    {
      Building buildingAt = this.getBuildingAt(tile);
      if (buildingAt == null)
        return false;
      buildingAt.performActionOnDemolition((GameLocation) this);
      this.buildings.Remove(buildingAt);
      return true;
    }

    public bool destroyStructure(Building b)
    {
      b.performActionOnDemolition((GameLocation) this);
      return this.buildings.Remove(b);
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
      if (!glider && this.buildings.Count > 0)
      {
        Microsoft.Xna.Framework.Rectangle boundingBox = Game1.player.GetBoundingBox();
        FarmAnimal farmAnimal = character as FarmAnimal;
        bool flag1 = character is JunimoHarvester;
        bool flag2 = character is NPC;
        foreach (Building building in this.buildings)
        {
          if (building.intersects(position) && (!isFarmer || !building.intersects(boundingBox)))
          {
            if (farmAnimal != null)
            {
              Microsoft.Xna.Framework.Rectangle rectForAnimalDoor = building.getRectForAnimalDoor();
              rectForAnimalDoor.Height += 64;
              if (rectForAnimalDoor.Contains(position) && building.buildingType.Value.Contains(farmAnimal.buildingTypeILiveIn.Value))
                continue;
            }
            else if (flag1)
            {
              Microsoft.Xna.Framework.Rectangle rectForAnimalDoor = building.getRectForAnimalDoor();
              rectForAnimalDoor.Height += 64;
              if (rectForAnimalDoor.Contains(position))
                continue;
            }
            else if (flag2)
            {
              Microsoft.Xna.Framework.Rectangle rectForHumanDoor = building.getRectForHumanDoor();
              rectForHumanDoor.Height += 64;
              if (rectForHumanDoor.Contains(position))
                continue;
            }
            return true;
          }
        }
      }
      return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement);
    }

    public override bool isActionableTile(int xTile, int yTile, Farmer who)
    {
      foreach (Building building in this.buildings)
      {
        if (building.isActionableTile(xTile, yTile, who))
          return true;
      }
      return base.isActionableTile(xTile, yTile, who);
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      foreach (Building building in this.buildings)
      {
        if (building.doAction(new Vector2((float) tileLocation.X, (float) tileLocation.Y), who))
          return true;
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public override bool isTileOccupied(
      Vector2 tileLocation,
      string characterToIngore = "",
      bool ignoreAllCharacters = false)
    {
      foreach (Building building in this.buildings)
      {
        if (!building.isTilePassable(tileLocation))
          return true;
      }
      return base.isTileOccupied(tileLocation, characterToIngore, ignoreAllCharacters);
    }

    public override bool isTileOccupiedForPlacement(Vector2 tileLocation, StardewValley.Object toPlace = null)
    {
      foreach (Building building in this.buildings)
      {
        if (building.isTileOccupiedForPlacement(tileLocation, toPlace))
          return true;
      }
      return base.isTileOccupiedForPlacement(tileLocation, toPlace);
    }

    public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
    {
      base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
      foreach (Building building in this.buildings)
        building.updateWhenFarmNotCurrentLocation(time);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      if (this.wasUpdated && Game1.gameMode != (byte) 0)
        return;
      base.UpdateWhenCurrentLocation(time);
      foreach (Building building in this.buildings)
        building.Update(time);
    }

    public override void drawFloorDecorations(SpriteBatch b)
    {
      int num1 = 1;
      Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X / 64 - num1, Game1.viewport.Y / 64 - num1, (int) Math.Ceiling((double) Game1.viewport.Width / 64.0) + 2 * num1, (int) Math.Ceiling((double) Game1.viewport.Height / 64.0) + 3 + 2 * num1);
      Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle();
      foreach (Building building in this.buildings)
      {
        int tilePropertyRadius = building.GetAdditionalTilePropertyRadius();
        rectangle2.X = (int) (NetFieldBase<int, NetInt>) building.tileX - tilePropertyRadius;
        rectangle2.Width = (int) (NetFieldBase<int, NetInt>) building.tilesWide + tilePropertyRadius * 2;
        int num2 = (int) (NetFieldBase<int, NetInt>) building.tileY + (int) (NetFieldBase<int, NetInt>) building.tilesHigh + tilePropertyRadius;
        int num3 = num2 - (int) Math.Ceiling((double) building.getSourceRect().Height * 4.0 / 64.0) - tilePropertyRadius;
        rectangle2.Y = num3;
        rectangle2.Height = num2 - num3;
        if (rectangle2.Intersects(rectangle1))
          building.drawBackground(b);
      }
      base.drawFloorDecorations(b);
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      int num1 = 1;
      Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X / 64 - num1, Game1.viewport.Y / 64 - num1, (int) Math.Ceiling((double) Game1.viewport.Width / 64.0) + 2 * num1, (int) Math.Ceiling((double) Game1.viewport.Height / 64.0) + 3 + 2 * num1);
      Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle();
      foreach (Building building in this.buildings)
      {
        int tilePropertyRadius = building.GetAdditionalTilePropertyRadius();
        rectangle2.X = (int) (NetFieldBase<int, NetInt>) building.tileX - tilePropertyRadius;
        rectangle2.Width = (int) (NetFieldBase<int, NetInt>) building.tilesWide + tilePropertyRadius * 2;
        int num2 = (int) (NetFieldBase<int, NetInt>) building.tileY + (int) (NetFieldBase<int, NetInt>) building.tilesHigh + tilePropertyRadius;
        int num3 = num2 - (int) Math.Ceiling((double) building.getSourceRect().Height * 4.0 / 64.0) - tilePropertyRadius;
        rectangle2.Y = num3;
        rectangle2.Height = num2 - num3;
        if (rectangle2.Intersects(rectangle1))
          building.draw(b);
      }
    }

    public void tryToUpgrade(Building toUpgrade, BluePrint blueprint)
    {
      if (toUpgrade != null && blueprint.name != null && toUpgrade.buildingType.Equals((object) blueprint.nameOfBuildingToUpgrade))
      {
        if (toUpgrade.indoors.Value.farmers.Any())
        {
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\Locations:BuildableLocation_CantUpgrade_SomeoneInside"), Color.Red, 3500f));
        }
        else
        {
          toUpgrade.indoors.Value.map = Game1.game1.xTileContent.Load<Map>("Maps\\" + blueprint.mapToWarpTo);
          toUpgrade.indoors.Value.name.Value = blueprint.mapToWarpTo;
          toUpgrade.indoors.Value.isStructure.Value = true;
          toUpgrade.buildingType.Value = blueprint.name;
          toUpgrade.resetTexture();
          if (toUpgrade.indoors.Value is AnimalHouse)
            ((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) toUpgrade.indoors).resetPositionsOfAllAnimals();
          this.playSound("axe");
          blueprint.consumeResources();
          toUpgrade.performActionOnUpgrade((GameLocation) this);
          toUpgrade.color.Value = Color.White;
          Game1.exitActiveMenu();
          Game1.multiplayer.globalChatInfoMessage("BuildingBuild", Game1.player.Name, Utility.AOrAn(blueprint.displayName), blueprint.displayName, (string) (NetFieldBase<string, NetString>) Game1.player.farmName);
        }
      }
      else
      {
        if (toUpgrade == null)
          return;
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\Locations:BuildableLocation_CantUpgrade_IncorrectBuildingType"), Color.Red, 3500f));
      }
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      foreach (Building building in this.buildings)
        building.resetLocalState();
    }

    public bool isBuildingConstructed(string name)
    {
      foreach (Building building in this.buildings)
      {
        if (building.buildingType.Value.Equals(name) && (int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0)
          return true;
      }
      return false;
    }

    public int getNumberBuildingsConstructed(string name)
    {
      int buildingsConstructed = 0;
      foreach (Building building in this.buildings)
      {
        if (building.buildingType.Value.Contains(name) && (int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0 && (int) (NetFieldBase<int, NetInt>) building.daysUntilUpgrade <= 0)
          ++buildingsConstructed;
      }
      return buildingsConstructed;
    }

    public bool isThereABuildingUnderConstruction()
    {
      foreach (Building building in this.buildings)
      {
        if ((int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft > 0 || (int) (NetFieldBase<int, NetInt>) building.daysUntilUpgrade > 0)
          return true;
      }
      return false;
    }

    public Building getBuildingUnderConstruction()
    {
      foreach (Building building in this.buildings)
      {
        if ((int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft > 0 || (int) (NetFieldBase<int, NetInt>) building.daysUntilUpgrade > 0)
          return building;
      }
      return (Building) null;
    }

    public bool buildStructure(
      Building b,
      Vector2 tileLocation,
      Farmer who,
      bool skipSafetyChecks = false)
    {
      if (!skipSafetyChecks)
      {
        for (int index1 = 0; index1 < (int) (NetFieldBase<int, NetInt>) b.tilesHigh; ++index1)
        {
          for (int index2 = 0; index2 < (int) (NetFieldBase<int, NetInt>) b.tilesWide; ++index2)
            this.pokeTileForConstruction(new Vector2(tileLocation.X + (float) index2, tileLocation.Y + (float) index1));
        }
        foreach (Point additionalPlacementTile in b.additionalPlacementTiles)
        {
          int x = additionalPlacementTile.X;
          int y = additionalPlacementTile.Y;
          this.pokeTileForConstruction(new Vector2(tileLocation.X + (float) x, tileLocation.Y + (float) y));
        }
        for (int index3 = 0; index3 < (int) (NetFieldBase<int, NetInt>) b.tilesHigh; ++index3)
        {
          for (int index4 = 0; index4 < (int) (NetFieldBase<int, NetInt>) b.tilesWide; ++index4)
          {
            Vector2 vector2 = new Vector2(tileLocation.X + (float) index4, tileLocation.Y + (float) index3);
            if (!this.buildings.Contains(b) || !b.occupiesTile(vector2))
            {
              if (!this.isBuildable(vector2))
                return false;
              foreach (Character farmer in this.farmers)
              {
                if (farmer.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(index4 * 64, index3 * 64, 64, 64)))
                  return false;
              }
            }
          }
        }
        foreach (Point additionalPlacementTile in b.additionalPlacementTiles)
        {
          int x = additionalPlacementTile.X;
          int y = additionalPlacementTile.Y;
          Vector2 vector2 = new Vector2(tileLocation.X + (float) x, tileLocation.Y + (float) y);
          if (!this.buildings.Contains(b) || !b.occupiesTile(vector2))
          {
            if (!this.isBuildable(vector2))
              return false;
            foreach (Character farmer in this.farmers)
            {
              if (farmer.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(x * 64, y * 64, 64, 64)))
                return false;
            }
          }
        }
        if (b.humanDoor.Value != new Point(-1, -1))
        {
          Vector2 vector2 = tileLocation + new Vector2((float) b.humanDoor.X, (float) (b.humanDoor.Y + 1));
          if ((!this.buildings.Contains(b) || !b.occupiesTile(vector2)) && !this.isBuildable(vector2) && !this.isPath(vector2))
            return false;
        }
        string message = b.isThereAnythingtoPreventConstruction((GameLocation) this, tileLocation);
        if (message != null)
        {
          Game1.addHUDMessage(new HUDMessage(message, Color.Red, 3500f));
          return false;
        }
      }
      b.tileX.Value = (int) tileLocation.X;
      b.tileY.Value = (int) tileLocation.Y;
      if (b.indoors.Value != null && b.indoors.Value is AnimalHouse)
      {
        foreach (long num in (NetList<long, NetLong>) (b.indoors.Value as AnimalHouse).animalsThatLiveHere)
        {
          FarmAnimal animal1 = Utility.getAnimal(num);
          if (animal1 != null)
          {
            animal1.homeLocation.Value = tileLocation;
            animal1.home = b;
          }
          else if (animal1 == null && (b.indoors.Value as AnimalHouse).animals.ContainsKey(num))
          {
            FarmAnimal animal2 = (b.indoors.Value as AnimalHouse).animals[num];
            animal2.homeLocation.Value = tileLocation;
            animal2.home = b;
          }
        }
      }
      if (b.indoors.Value != null)
      {
        foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) b.indoors.Value.warps)
        {
          if (warp.TargetName == this.Name)
          {
            warp.TargetX = b.humanDoor.X + (int) (NetFieldBase<int, NetInt>) b.tileX;
            warp.TargetY = b.humanDoor.Y + (int) (NetFieldBase<int, NetInt>) b.tileY + 1;
          }
        }
      }
      if (!this.buildings.Contains(b))
        this.buildings.Add(b);
      Action<Vector2> action = (Action<Vector2>) (tile_location =>
      {
        if (!Utility.IsNormalObjectAtParentSheetIndex((Item) this.getObjectAtTile((int) tile_location.X, (int) tile_location.Y), 590))
          return;
        this.removeObject(tile_location, false);
      });
      for (int index5 = 0; index5 < (int) (NetFieldBase<int, NetInt>) b.tilesHigh; ++index5)
      {
        for (int index6 = 0; index6 < (int) (NetFieldBase<int, NetInt>) b.tilesWide; ++index6)
          action(new Vector2(tileLocation.X + (float) index6, tileLocation.Y + (float) index5));
      }
      foreach (Point additionalPlacementTile in b.additionalPlacementTiles)
      {
        int x = additionalPlacementTile.X;
        int y = additionalPlacementTile.Y;
        action(new Vector2(tileLocation.X + (float) x, tileLocation.Y + (float) y));
      }
      return true;
    }

    public override string doesTileHaveProperty(
      int xTile,
      int yTile,
      string propertyName,
      string layerName)
    {
      foreach (Building building in this.buildings)
      {
        int tilePropertyRadius = building.GetAdditionalTilePropertyRadius();
        if (xTile >= (int) (NetFieldBase<int, NetInt>) building.tileX - tilePropertyRadius && xTile < (int) (NetFieldBase<int, NetInt>) building.tileX + (int) (NetFieldBase<int, NetInt>) building.tilesWide + tilePropertyRadius && yTile >= (int) (NetFieldBase<int, NetInt>) building.tileY - tilePropertyRadius && yTile < (int) (NetFieldBase<int, NetInt>) building.tileY + (int) (NetFieldBase<int, NetInt>) building.tilesHigh + tilePropertyRadius)
        {
          string property_value = (string) null;
          if (building.doesTileHaveProperty(xTile, yTile, propertyName, layerName, ref property_value))
            return property_value;
        }
      }
      return base.doesTileHaveProperty(xTile, yTile, propertyName, layerName);
    }

    public override string doesTileHavePropertyNoNull(
      int xTile,
      int yTile,
      string propertyName,
      string layerName)
    {
      foreach (Building building in this.buildings)
      {
        int tilePropertyRadius = building.GetAdditionalTilePropertyRadius();
        if (xTile >= (int) (NetFieldBase<int, NetInt>) building.tileX - tilePropertyRadius && xTile < (int) (NetFieldBase<int, NetInt>) building.tileX + (int) (NetFieldBase<int, NetInt>) building.tilesWide + tilePropertyRadius && yTile >= (int) (NetFieldBase<int, NetInt>) building.tileY - tilePropertyRadius && yTile < (int) (NetFieldBase<int, NetInt>) building.tileY + (int) (NetFieldBase<int, NetInt>) building.tilesHigh + tilePropertyRadius)
        {
          string property_value = (string) null;
          if (building.doesTileHaveProperty(xTile, yTile, propertyName, layerName, ref property_value))
            return property_value ?? "";
        }
      }
      return base.doesTileHavePropertyNoNull(xTile, yTile, propertyName, layerName);
    }

    public virtual void pokeTileForConstruction(Vector2 tile)
    {
    }

    public bool isBuildable(Vector2 tileLocation) => (!Game1.player.getTileLocation().Equals(tileLocation) || !Game1.player.currentLocation.Equals((GameLocation) this)) && (!this.isTileOccupiedForPlacement(tileLocation, (StardewValley.Object) null) || Utility.IsNormalObjectAtParentSheetIndex((Item) this.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y), 590)) && this.GetFurnitureAt(tileLocation) == null && this.isTilePassable(new Location((int) tileLocation.X, (int) tileLocation.Y), Game1.viewport) && this.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "NoFurniture", "Back") == null && (Game1.currentLocation.doesTileHavePropertyNoNull((int) tileLocation.X, (int) tileLocation.Y, "Buildable", "Back").ToLower().Equals("t") || Game1.currentLocation.doesTileHavePropertyNoNull((int) tileLocation.X, (int) tileLocation.Y, "Buildable", "Back").ToLower().Equals("true") || Game1.currentLocation.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "Diggable", "Back") != null && !Game1.currentLocation.doesTileHavePropertyNoNull((int) tileLocation.X, (int) tileLocation.Y, "Buildable", "Back").ToLower().Equals("f"));

    public bool isPath(Vector2 tileLocation)
    {
      StardewValley.Object @object = (StardewValley.Object) null;
      TerrainFeature terrainFeature = (TerrainFeature) null;
      this.objects.TryGetValue(tileLocation, out @object);
      this.terrainFeatures.TryGetValue(tileLocation, out terrainFeature);
      if (terrainFeature == null || !terrainFeature.isPassable())
        return false;
      return @object == null || @object.isPassable();
    }

    public override Point getWarpPointTo(string location, Character character = null)
    {
      foreach (Building building in this.buildings)
      {
        if (building.indoors.Value != null && (building.indoors.Value.Name == location || building.indoors.Value.uniqueName.Value != null && building.indoors.Value.uniqueName.Value == location))
          return building.getPointForHumanDoor();
      }
      return base.getWarpPointTo(location, character);
    }

    public override Warp isCollidingWithDoors(Microsoft.Xna.Framework.Rectangle position, Character character = null)
    {
      for (int corner = 0; corner < 4; ++corner)
      {
        Vector2 cornersOfThisRectangle = Utility.getCornersOfThisRectangle(ref position, corner);
        Point point = new Point((int) cornersOfThisRectangle.X / 64, (int) cornersOfThisRectangle.Y / 64);
        foreach (Building building in this.buildings)
        {
          Point humanDoor = (Point) (NetFieldBase<Point, NetPoint>) building.humanDoor;
          if (building.indoors.Value != null && point.Equals(building.getPointForHumanDoor()))
            return this.getWarpFromDoor(building.getPointForHumanDoor(), character);
        }
      }
      return base.isCollidingWithDoors(position, character);
    }

    public override Warp getWarpFromDoor(Point door, Character character = null)
    {
      foreach (Building building in this.buildings)
      {
        Point humanDoor = (Point) (NetFieldBase<Point, NetPoint>) building.humanDoor;
        if (building.indoors.Value != null && door == building.getPointForHumanDoor())
          return new Warp(door.X, door.Y, (string) (NetFieldBase<string, NetString>) building.indoors.Value.uniqueName, building.indoors.Value.warps[0].X, building.indoors.Value.warps[0].Y - 1, false);
      }
      return base.getWarpFromDoor(door, character);
    }

    public bool buildStructure(
      BluePrint structureForPlacement,
      Vector2 tileLocation,
      Farmer who,
      bool magicalConstruction = false,
      bool skipSafetyChecks = false)
    {
      if (!skipSafetyChecks)
      {
        for (int index1 = 0; index1 < structureForPlacement.tilesHeight; ++index1)
        {
          for (int index2 = 0; index2 < structureForPlacement.tilesWidth; ++index2)
            this.pokeTileForConstruction(new Vector2(tileLocation.X + (float) index2, tileLocation.Y + (float) index1));
        }
        foreach (Point additionalPlacementTile in structureForPlacement.additionalPlacementTiles)
        {
          int x = additionalPlacementTile.X;
          int y = additionalPlacementTile.Y;
          this.pokeTileForConstruction(new Vector2(tileLocation.X + (float) x, tileLocation.Y + (float) y));
        }
        for (int index3 = 0; index3 < structureForPlacement.tilesHeight; ++index3)
        {
          for (int index4 = 0; index4 < structureForPlacement.tilesWidth; ++index4)
          {
            if (!this.isBuildable(new Vector2(tileLocation.X + (float) index4, tileLocation.Y + (float) index3)))
              return false;
            foreach (Character farmer in this.farmers)
            {
              if (farmer.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(index4 * 64, index3 * 64, 64, 64)))
                return false;
            }
          }
        }
        foreach (Point additionalPlacementTile in structureForPlacement.additionalPlacementTiles)
        {
          int x = additionalPlacementTile.X;
          int y = additionalPlacementTile.Y;
          if (!this.isBuildable(new Vector2(tileLocation.X + (float) x, tileLocation.Y + (float) y)))
            return false;
          foreach (Character farmer in this.farmers)
          {
            if (farmer.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(x * 64, y * 64, 64, 64)))
              return false;
          }
        }
        if (structureForPlacement.humanDoor != new Point(-1, -1))
        {
          Vector2 tileLocation1 = tileLocation + new Vector2((float) structureForPlacement.humanDoor.X, (float) (structureForPlacement.humanDoor.Y + 1));
          if (!this.isBuildable(tileLocation1) && !this.isPath(tileLocation1))
            return false;
        }
      }
      Building building;
      switch (structureForPlacement.name)
      {
        case "Barn":
        case "Big Barn":
        case "Deluxe Barn":
          building = (Building) new Barn(structureForPlacement, tileLocation);
          break;
        case "Big Coop":
        case "Coop":
        case "Deluxe Coop":
          building = (Building) new Coop(structureForPlacement, tileLocation);
          break;
        case "Fish Pond":
          building = (Building) new FishPond(structureForPlacement, tileLocation);
          break;
        case "Greenhouse":
          building = (Building) new GreenhouseBuilding(structureForPlacement, tileLocation);
          break;
        case "Junimo Hut":
          building = (Building) new JunimoHut(structureForPlacement, tileLocation);
          break;
        case "Mill":
          building = (Building) new Mill(structureForPlacement, tileLocation);
          break;
        case "Shipping Bin":
          building = (Building) new ShippingBin(structureForPlacement, tileLocation);
          break;
        case "Stable":
          building = (Building) new Stable(GuidHelper.NewGuid(), structureForPlacement, tileLocation);
          break;
        default:
          building = new Building(structureForPlacement, tileLocation);
          break;
      }
      building.owner.Value = who.UniqueMultiplayerID;
      if (!skipSafetyChecks)
      {
        string message = building.isThereAnythingtoPreventConstruction((GameLocation) this, tileLocation);
        if (message != null)
        {
          Game1.addHUDMessage(new HUDMessage(message, Color.Red, 3500f));
          return false;
        }
      }
      for (int index5 = 0; index5 < structureForPlacement.tilesHeight; ++index5)
      {
        for (int index6 = 0; index6 < structureForPlacement.tilesWidth; ++index6)
          this.terrainFeatures.Remove(new Vector2(tileLocation.X + (float) index6, tileLocation.Y + (float) index5));
      }
      this.buildings.Add(building);
      building.performActionOnConstruction((GameLocation) this);
      if (magicalConstruction)
        Game1.multiplayer.globalChatInfoMessage("BuildingMagicBuild", Game1.player.Name, Utility.AOrAn(structureForPlacement.displayName), structureForPlacement.displayName, (string) (NetFieldBase<string, NetString>) Game1.player.farmName);
      else
        Game1.multiplayer.globalChatInfoMessage("BuildingBuild", Game1.player.Name, Utility.AOrAn(structureForPlacement.displayName), structureForPlacement.displayName, (string) (NetFieldBase<string, NetString>) Game1.player.farmName);
      return true;
    }
  }
}
