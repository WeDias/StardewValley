// Decompiled with JetBrains decompiler
// Type: StardewValley.AnimalHouse
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Events;
using StardewValley.Network;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley
{
  public class AnimalHouse : GameLocation, IAnimalLocation
  {
    [XmlElement("animals")]
    public readonly NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> animals = new NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>();
    [XmlElement("animalLimit")]
    public readonly NetInt animalLimit = new NetInt(4);
    public readonly NetLongList animalsThatLiveHere = new NetLongList();
    [XmlElement("incubatingEgg")]
    public readonly NetPoint incubatingEgg = new NetPoint();
    private readonly List<KeyValuePair<long, FarmAnimal>> _tempAnimals = new List<KeyValuePair<long, FarmAnimal>>();
    [XmlIgnore]
    public bool hasShownIncubatorBuildingFullMessage;

    public NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> Animals => this.animals;

    public AnimalHouse()
    {
    }

    public AnimalHouse(string mapPath, string name)
      : base(mapPath, name)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.animals, (INetSerializable) this.animalLimit, (INetSerializable) this.animalsThatLiveHere, (INetSerializable) this.incubatingEgg);
    }

    public void updateWhenNotCurrentLocation(Building parentBuilding, GameTime time)
    {
      if (Game1.currentLocation.Equals((GameLocation) this))
        return;
      for (int index = this.animals.Count() - 1; index >= 0; --index)
        this.animals.Pairs.ElementAt(index).Value.updateWhenNotCurrentLocation(parentBuilding, time, (GameLocation) this);
    }

    public void incubator()
    {
      if (this.incubatingEgg.Y <= 0 && Game1.player.ActiveObject != null && Game1.player.ActiveObject.Category == -5)
      {
        this.incubatingEgg.X = 2;
        this.incubatingEgg.Y = Game1.player.ActiveObject.ParentSheetIndex;
        this.map.GetLayer("Front").Tiles[1, 2].TileIndex += Game1.player.ActiveObject.ParentSheetIndex == 180 || Game1.player.ActiveObject.ParentSheetIndex == 182 ? 2 : 1;
        Game1.throwActiveObjectDown();
        this.hasShownIncubatorBuildingFullMessage = false;
      }
      else
      {
        if (Game1.player.ActiveObject != null || this.incubatingEgg.Y <= 0)
          return;
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:AnimalHouse_Incubator_RemoveEgg_Question"), this.createYesNoResponses(), "RemoveIncubatingEgg");
      }
    }

    public bool isFull() => this.animalsThatLiveHere.Count >= (int) (NetFieldBase<int, NetInt>) this.animalLimit;

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

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64);
      if (who.ActiveObject != null && who.ActiveObject.Name.Equals("Hay") && this.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Trough", "Back") != null && !this.objects.ContainsKey(new Vector2((float) tileLocation.X, (float) tileLocation.Y)))
      {
        this.objects.Add(new Vector2((float) tileLocation.X, (float) tileLocation.Y), (Object) who.ActiveObject.getOne());
        who.reduceActiveItemByOne();
        who.currentLocation.playSound("coin");
        Game1.haltAfterCheck = false;
        return true;
      }
      bool flag = base.checkAction(tileLocation, viewport, who);
      return !flag && (this.CheckPetAnimal(rect, who) || Game1.didPlayerJustRightClick(true) && this.CheckInspectAnimal(rect, who)) || flag;
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

    protected override void resetSharedState()
    {
      this.resetPositionsOfAllAnimals();
      foreach (Object @object in this.objects.Values)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) @object.bigCraftable && @object.Name.Contains("Incubator") && @object.heldObject.Value != null && (int) (NetFieldBase<int, NetIntDelta>) @object.minutesUntilReady <= 0)
        {
          if (!this.isFull())
          {
            string str = "??";
            switch (@object.heldObject.Value.ParentSheetIndex)
            {
              case 107:
                str = Game1.content.LoadString("Strings\\Locations:AnimalHouse_Incubator_Hatch_DinosaurEgg");
                break;
              case 174:
              case 176:
              case 180:
              case 182:
                str = Game1.content.LoadString("Strings\\Locations:AnimalHouse_Incubator_Hatch_RegularEgg");
                break;
              case 289:
                str = Game1.content.LoadString("Strings\\Locations:AnimalHouse_Incubator_Hatch_Ostrich");
                break;
              case 305:
                str = Game1.content.LoadString("Strings\\Locations:AnimalHouse_Incubator_Hatch_VoidEgg");
                break;
              case 442:
                str = Game1.content.LoadString("Strings\\Locations:AnimalHouse_Incubator_Hatch_DuckEgg");
                break;
              case 928:
                str = Game1.content.LoadString("Strings\\Locations:AnimalHouse_Incubator_Hatch_GoldenEgg");
                break;
            }
            this.currentEvent = new Event("none/-1000 -1000/farmer 2 9 0/pause 250/message \"" + str + "\"/pause 500/animalNaming/pause 500/end");
            break;
          }
          if (!this.hasShownIncubatorBuildingFullMessage)
          {
            this.hasShownIncubatorBuildingFullMessage = true;
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:AnimalHouse_Incubator_HouseFull"));
          }
        }
      }
      base.resetSharedState();
    }

    public Building getBuilding()
    {
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building.indoors.Value != null && building.indoors.Value.Equals((GameLocation) this))
          return building;
      }
      return (Building) null;
    }

    public void addNewHatchedAnimal(string name)
    {
      bool flag = false;
      foreach (Object @object in this.objects.Values)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) @object.bigCraftable && @object.Name.Contains("Incubator") && @object.heldObject.Value != null && (int) (NetFieldBase<int, NetIntDelta>) @object.minutesUntilReady <= 0 && !this.isFull())
        {
          flag = true;
          string type = "??";
          if (@object.heldObject.Value == null)
          {
            type = "White Chicken";
          }
          else
          {
            switch (@object.heldObject.Value.ParentSheetIndex)
            {
              case 107:
                type = "Dinosaur";
                break;
              case 174:
              case 176:
                type = "White Chicken";
                break;
              case 180:
              case 182:
                type = "Brown Chicken";
                break;
              case 289:
                type = "Ostrich";
                break;
              case 305:
                type = "Void Chicken";
                break;
              case 442:
                type = "Duck";
                break;
              case 928:
                type = "Golden Chicken";
                break;
            }
          }
          FarmAnimal farmAnimal = new FarmAnimal(type, Game1.multiplayer.getNewID(), (long) Game1.player.uniqueMultiplayerID);
          farmAnimal.Name = name;
          farmAnimal.displayName = name;
          Building building = this.getBuilding();
          farmAnimal.home = building;
          farmAnimal.homeLocation.Value = new Vector2((float) (int) (NetFieldBase<int, NetInt>) building.tileX, (float) (int) (NetFieldBase<int, NetInt>) building.tileY);
          farmAnimal.setRandomPosition((GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) farmAnimal.home.indoors);
          (building.indoors.Value as AnimalHouse).animals.Add((long) farmAnimal.myID, farmAnimal);
          (building.indoors.Value as AnimalHouse).animalsThatLiveHere.Add((long) farmAnimal.myID);
          @object.heldObject.Value = (Object) null;
          @object.ParentSheetIndex = 101;
          if (type == "Ostrich")
          {
            @object.ParentSheetIndex = 254;
            break;
          }
          break;
        }
      }
      if (!flag && Game1.farmEvent != null && Game1.farmEvent is QuestionEvent)
      {
        FarmAnimal farmAnimal = new FarmAnimal((Game1.farmEvent as QuestionEvent).animal.type.Value, Game1.multiplayer.getNewID(), (long) Game1.player.uniqueMultiplayerID);
        farmAnimal.Name = name;
        farmAnimal.displayName = name;
        farmAnimal.parentId.Value = (long) (Game1.farmEvent as QuestionEvent).animal.myID;
        Building building = this.getBuilding();
        farmAnimal.home = building;
        farmAnimal.homeLocation.Value = new Vector2((float) (int) (NetFieldBase<int, NetInt>) building.tileX, (float) (int) (NetFieldBase<int, NetInt>) building.tileY);
        (Game1.farmEvent as QuestionEvent).forceProceed = true;
        farmAnimal.setRandomPosition((GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) farmAnimal.home.indoors);
        (building.indoors.Value as AnimalHouse).animals.Add((long) farmAnimal.myID, farmAnimal);
        (building.indoors.Value as AnimalHouse).animalsThatLiveHere.Add((long) farmAnimal.myID);
      }
      Game1.exitActiveMenu();
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
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
      {
        if (character != null && !character.Equals((object) pair.Value) && position.Intersects(pair.Value.GetBoundingBox()) && (!isFarmer || !Game1.player.GetBoundingBox().Intersects(pair.Value.GetBoundingBox())))
        {
          if (isFarmer)
          {
            if ((character as Farmer).TemporaryPassableTiles.Intersects(position))
              break;
          }
          pair.Value.farmerPushing();
          return true;
        }
      }
      return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
        this._tempAnimals.Add(pair);
      foreach (KeyValuePair<long, FarmAnimal> tempAnimal in this._tempAnimals)
      {
        if (tempAnimal.Value.updateWhenCurrentLocation(time, (GameLocation) this))
          this.animals.Remove(tempAnimal.Key);
      }
      this._tempAnimals.Clear();
    }

    public void resetPositionsOfAllAnimals()
    {
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
        pair.Value.setRandomPosition((GameLocation) this);
    }

    public override bool dropObject(
      Object obj,
      Vector2 location,
      xTile.Dimensions.Rectangle viewport,
      bool initialPlacement,
      Farmer who = null)
    {
      Vector2 key = new Vector2((float) (int) ((double) location.X / 64.0), (float) (int) ((double) location.Y / 64.0));
      if (!obj.Name.Equals("Hay") || this.doesTileHaveProperty((int) key.X, (int) key.Y, "Trough", "Back") == null)
        return base.dropObject(obj, location, viewport, initialPlacement);
      if (this.objects.ContainsKey(key))
        return false;
      this.objects.Add(key, obj);
      return true;
    }

    public void feedAllAnimals()
    {
      int num = 0;
      for (int index1 = 0; index1 < this.map.Layers[0].LayerWidth; ++index1)
      {
        for (int index2 = 0; index2 < this.map.Layers[0].LayerHeight; ++index2)
        {
          if (this.doesTileHaveProperty(index1, index2, "Trough", "Back") != null)
          {
            Vector2 key = new Vector2((float) index1, (float) index2);
            if (!this.objects.ContainsKey(key) && (int) (NetFieldBase<int, NetInt>) Game1.getFarm().piecesOfHay > 0)
            {
              this.objects.Add(key, new Object(178, 1));
              ++num;
              --Game1.getFarm().piecesOfHay.Value;
            }
            if (num >= (int) (NetFieldBase<int, NetInt>) this.animalLimit)
              return;
          }
        }
      }
    }

    public override void DayUpdate(int dayOfMonth)
    {
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
        pair.Value.dayUpdate((GameLocation) this);
      base.DayUpdate(dayOfMonth);
    }

    public override bool performToolAction(Tool t, int tileX, int tileY)
    {
      if (t is MeleeWeapon)
      {
        foreach (FarmAnimal farmAnimal in this.animals.Values)
        {
          if (farmAnimal.GetBoundingBox().Intersects((t as MeleeWeapon).mostRecentArea))
            farmAnimal.hitWithWeapon(t as MeleeWeapon);
        }
      }
      return false;
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
        pair.Value.draw(b);
    }
  }
}
