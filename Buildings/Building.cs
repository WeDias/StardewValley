// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.Building
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Buildings
{
  [XmlInclude(typeof (Coop))]
  [XmlInclude(typeof (Barn))]
  [XmlInclude(typeof (Stable))]
  [XmlInclude(typeof (Mill))]
  [XmlInclude(typeof (JunimoHut))]
  [XmlInclude(typeof (ShippingBin))]
  [XmlInclude(typeof (GreenhouseBuilding))]
  [XmlInclude(typeof (FishPond))]
  public class Building : INetObject<NetFields>
  {
    [XmlIgnore]
    public Lazy<Texture2D> texture;
    [XmlIgnore]
    public Texture2D paintedTexture;
    [XmlElement("indoors")]
    public readonly NetRef<GameLocation> indoors = new NetRef<GameLocation>();
    [XmlElement("tileX")]
    public readonly NetInt tileX = new NetInt();
    [XmlElement("tileY")]
    public readonly NetInt tileY = new NetInt();
    [XmlElement("tilesWide")]
    public readonly NetInt tilesWide = new NetInt();
    [XmlElement("tilesHigh")]
    public readonly NetInt tilesHigh = new NetInt();
    [XmlElement("maxOccupants")]
    public readonly NetInt maxOccupants = new NetInt();
    [XmlElement("currentOccupants")]
    public readonly NetInt currentOccupants = new NetInt();
    [XmlElement("daysOfConstructionLeft")]
    public readonly NetInt daysOfConstructionLeft = new NetInt();
    [XmlElement("daysUntilUpgrade")]
    public readonly NetInt daysUntilUpgrade = new NetInt();
    [XmlElement("buildingType")]
    public readonly NetString buildingType = new NetString();
    [XmlElement("buildingPaintColor")]
    public NetRef<BuildingPaintColor> netBuildingPaintColor = new NetRef<BuildingPaintColor>();
    [XmlIgnore]
    public NetList<Point, NetPoint> additionalPlacementTiles = new NetList<Point, NetPoint>();
    /// <summary>
    /// Used for modders to store metadata to this object. This data is synchronized in multiplayer and saved to the save data.
    /// </summary>
    [XmlIgnore]
    public ModDataDictionary modData = new ModDataDictionary();
    protected int _isCabin = -1;
    [Obsolete]
    public string baseNameOfIndoors;
    [XmlElement("humanDoor")]
    public readonly NetPoint humanDoor = new NetPoint();
    [XmlElement("animalDoor")]
    public readonly NetPoint animalDoor = new NetPoint();
    [XmlElement("color")]
    public readonly NetColor color = new NetColor(Color.White);
    [XmlElement("animalDoorOpen")]
    public readonly NetBool animalDoorOpen = new NetBool();
    [XmlElement("magical")]
    public readonly NetBool magical = new NetBool();
    [XmlElement("fadeWhenPlayerIsBehind")]
    public readonly NetBool fadeWhenPlayerIsBehind = new NetBool(true);
    [XmlElement("owner")]
    public readonly NetLong owner = new NetLong();
    [XmlElement("newConstructionTimer")]
    protected readonly NetInt newConstructionTimer = new NetInt();
    [XmlElement("alpha")]
    protected readonly NetFloat alpha = new NetFloat();
    [XmlIgnore]
    protected bool _isMoving;
    public static Rectangle leftShadow = new Rectangle(656, 394, 16, 16);
    public static Rectangle middleShadow = new Rectangle(672, 394, 16, 16);
    public static Rectangle rightShadow = new Rectangle(688, 394, 16, 16);

    /// <summary>Get the mod populated metadata as it will be serialized for game saving. Identical to <see cref="F:StardewValley.Buildings.Building.modData" /> except returns null during save if it is empty. It is strongly recommended to use <see cref="F:StardewValley.Buildings.Building.modData" /> instead.</summary>
    [XmlElement("modData")]
    public ModDataDictionary modDataForSerialization
    {
      get => this.modData.GetForSerialization();
      set => this.modData.SetFromSerialization(value);
    }

    public bool isCabin
    {
      get
      {
        if (this._isCabin == -1)
          this._isCabin = this.indoors.Value == null || !(this.indoors.Value is Cabin) ? 0 : 1;
        return this._isCabin == 1;
      }
    }

    public string nameOfIndoors
    {
      get
      {
        GameLocation gameLocation = this.indoors.Get();
        return gameLocation == null ? "null" : (string) (NetFieldBase<string, NetString>) gameLocation.uniqueName;
      }
    }

    public string nameOfIndoorsWithoutUnique => this.indoors.Value == null ? "null" : this.getBuildingMapFileName(this.indoors.Value.Name);

    public bool isMoving
    {
      get => this._isMoving;
      set
      {
        if (this._isMoving == value)
          return;
        this._isMoving = value;
        if (this._isMoving)
          this.OnStartMove();
        if (this._isMoving)
          return;
        this.OnEndMove();
      }
    }

    public NetFields NetFields { get; } = new NetFields();

    public Building()
    {
      this.resetTexture();
      this.initNetFields();
    }

    public Building(BluePrint blueprint, Vector2 tileLocation)
      : this()
    {
      this.tileX.Value = (int) tileLocation.X;
      this.tileY.Value = (int) tileLocation.Y;
      this.tilesWide.Value = blueprint.tilesWidth;
      this.tilesHigh.Value = blueprint.tilesHeight;
      this.buildingType.Value = blueprint.name;
      this.humanDoor.Value = blueprint.humanDoor;
      this.animalDoor.Value = blueprint.animalDoor;
      this.indoors.Value = this.getIndoors(this.getBuildingMapFileName(blueprint.mapToWarpTo));
      this.maxOccupants.Value = blueprint.maxOccupants;
      this.daysOfConstructionLeft.Value = blueprint.daysToConstruct;
      this.magical.Value = blueprint.magical;
      this.alpha.Value = 1f;
    }

    public virtual bool CanBePainted()
    {
      if (!Game1.content.Load<Dictionary<string, string>>("Data\\PaintData").ContainsKey((string) (NetFieldBase<string, NetString>) this.buildingType))
        return false;
      if (this is GreenhouseBuilding)
      {
        if (!Game1.getFarm().greenhouseUnlocked.Value)
          return false;
      }
      else if (this.isCabin && this.indoors.Value is Cabin cabin && cabin.upgradeLevel < 2)
        return false;
      return true;
    }

    public virtual bool hasCarpenterPermissions() => Game1.IsMasterGame || this.owner.Value == Game1.player.UniqueMultiplayerID || this.isCabin && this.indoors.Value is Cabin && (this.indoors.Value as Cabin).owner == Game1.player;

    protected virtual string getBuildingMapFileName(string name)
    {
      if (name == "Slime Hutch")
        return "SlimeHutch";
      if (name == "Big Coop")
        return "Coop2";
      if (name == "Deluxe Coop")
        return "Coop3";
      if (name == "Big Barn")
        return "Barn2";
      if (name == "Deluxe Barn")
        return "Barn3";
      return name == "Big Shed" ? "Shed2" : name;
    }

    protected virtual void initNetFields()
    {
      this.NetFields.AddFields((INetSerializable) this.indoors, (INetSerializable) this.tileX, (INetSerializable) this.tileY, (INetSerializable) this.tilesWide, (INetSerializable) this.tilesHigh, (INetSerializable) this.maxOccupants, (INetSerializable) this.currentOccupants, (INetSerializable) this.daysOfConstructionLeft, (INetSerializable) this.daysUntilUpgrade, (INetSerializable) this.buildingType, (INetSerializable) this.humanDoor, (INetSerializable) this.animalDoor, (INetSerializable) this.magical, (INetSerializable) this.animalDoorOpen, (INetSerializable) this.owner, (INetSerializable) this.newConstructionTimer, (INetSerializable) this.additionalPlacementTiles, (INetSerializable) this.netBuildingPaintColor);
      this.buildingType.fieldChangeVisibleEvent += (NetFieldBase<string, NetString>.FieldChange) ((a, b, c) => this.resetTexture());
      if (this.netBuildingPaintColor.Value == null)
        this.netBuildingPaintColor.Value = new BuildingPaintColor();
      this.NetFields.AddField((INetSerializable) this.modData);
    }

    public virtual string textureName() => "Buildings\\" + (string) (NetFieldBase<string, NetString>) this.buildingType;

    public virtual void resetTexture() => this.texture = new Lazy<Texture2D>((Func<Texture2D>) (() =>
    {
      Texture2D base_texture = Game1.content.Load<Texture2D>(this.textureName());
      if (this.paintedTexture != null)
      {
        this.paintedTexture.Dispose();
        this.paintedTexture = (Texture2D) null;
      }
      this.paintedTexture = BuildingPainter.Apply(base_texture, this.textureName() + "_PaintMask", this.netBuildingPaintColor.Value);
      if (this.paintedTexture != null)
        base_texture = this.paintedTexture;
      return base_texture;
    }));

    public int getTileSheetIndexForStructurePlacementTile(int x, int y)
    {
      if (x == this.humanDoor.X && y == this.humanDoor.Y)
        return 2;
      return x == this.animalDoor.X && y == this.animalDoor.Y ? 4 : 0;
    }

    public virtual void performTenMinuteAction(int timeElapsed)
    {
    }

    public virtual void resetLocalState()
    {
      this.alpha.Value = 1f;
      this.color.Value = Color.White;
      this.isMoving = false;
    }

    public virtual bool CanLeftClick(int x, int y) => this.intersects(new Rectangle(x, y, 1, 1));

    public virtual bool leftClicked() => false;

    public virtual bool doAction(Vector2 tileLocation, Farmer who)
    {
      if (who.isRidingHorse())
        return false;
      if (who.IsLocalPlayer && (double) tileLocation.X >= (double) (int) (NetFieldBase<int, NetInt>) this.tileX && (double) tileLocation.X < (double) ((int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide) && (double) tileLocation.Y >= (double) (int) (NetFieldBase<int, NetInt>) this.tileY && (double) tileLocation.Y < (double) ((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) && (int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:UnderConstruction"));
      }
      else
      {
        if (who.IsLocalPlayer && (double) tileLocation.X == (double) (this.humanDoor.X + (int) (NetFieldBase<int, NetInt>) this.tileX) && (double) tileLocation.Y == (double) (this.humanDoor.Y + (int) (NetFieldBase<int, NetInt>) this.tileY) && this.indoors.Value != null)
        {
          if (who.mount != null)
          {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:DismountBeforeEntering"));
            return false;
          }
          if (who.team.demolishLock.IsLocked())
          {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:CantEnter"));
            return false;
          }
          this.indoors.Value.isStructure.Value = true;
          who.currentLocation.playSoundAt("doorClose", tileLocation);
          Game1.warpFarmer(this.indoors.Value.uniqueName.Value, this.indoors.Value.warps[0].X, this.indoors.Value.warps[0].Y - 1, Game1.player.FacingDirection, true);
          return true;
        }
        if (who.IsLocalPlayer && this.buildingType.Equals((object) "Silo") && !this.isTilePassable(tileLocation))
        {
          if (who.ActiveObject != null && (int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex == 178)
          {
            if (who.ActiveObject.Stack == 0)
              who.ActiveObject.stack.Value = 1;
            int stack = who.ActiveObject.Stack;
            int addHay = (Game1.getLocationFromName("Farm") as Farm).tryToAddHay(who.ActiveObject.Stack);
            who.ActiveObject.stack.Value = addHay;
            if ((int) (NetFieldBase<int, NetInt>) who.ActiveObject.stack < stack)
            {
              Game1.playSound("Ship");
              DelayedAction.playSoundAfterDelay("grassyStep", 100);
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:AddedHay", (object) (stack - who.ActiveObject.Stack)));
            }
            if (who.ActiveObject.Stack <= 0)
              who.removeItemFromInventory((Item) who.ActiveObject);
          }
          else
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:PiecesOfHay", (object) (Game1.getLocationFromName("Farm") as Farm).piecesOfHay, (object) (Utility.numSilos() * 240)));
        }
        else
        {
          if (who.IsLocalPlayer && this.buildingType.Value.Contains("Obelisk") && !this.isTilePassable(tileLocation))
          {
            if (this.buildingType.Value == "Desert Obelisk" && Game1.player.isRidingHorse() && Game1.player.mount != null)
            {
              Game1.player.mount.checkAction(Game1.player, Game1.player.currentLocation);
            }
            else
            {
              for (int index = 0; index < 12; ++index)
                who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) who.Position.X - 256, (int) who.Position.X + 192), (float) Game1.random.Next((int) who.Position.Y - 256, (int) who.Position.Y + 192)), false, Game1.random.NextDouble() < 0.5));
              who.currentLocation.playSound("wand");
              Game1.displayFarmer = false;
              Game1.player.temporarilyInvincible = true;
              Game1.player.temporaryInvincibilityTimer = -2000;
              Game1.player.freezePause = 1000;
              Game1.flashAlpha = 1f;
              DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.obeliskWarpForReal), 1000);
              new Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
              int num = 0;
              for (int x = who.getTileX() + 8; x >= who.getTileX() - 8; --x)
              {
                who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) x, (float) who.getTileY()) * 64f, Color.White, animationInterval: 50f)
                {
                  layerDepth = 1f,
                  delayBeforeAnimationStart = num * 25,
                  motion = new Vector2(-0.25f, 0.0f)
                });
                ++num;
              }
            }
            return true;
          }
          if (who.IsLocalPlayer && who.ActiveObject != null && !this.isTilePassable(tileLocation))
            return this.performActiveObjectDropInAction(who, false);
        }
      }
      return false;
    }

    private void obeliskWarpForReal()
    {
      string buildingType = (string) (NetFieldBase<string, NetString>) this.buildingType;
      if (!(buildingType == "Earth Obelisk"))
      {
        if (!(buildingType == "Water Obelisk"))
        {
          if (!(buildingType == "Desert Obelisk"))
          {
            if (buildingType == "Island Obelisk")
              Game1.warpFarmer("IslandSouth", 11, 11, false);
          }
          else
            Game1.warpFarmer("Desert", 35, 43, false);
        }
        else
          Game1.warpFarmer("Beach", 20, 4, false);
      }
      else
        Game1.warpFarmer("Mountain", 31, 20, false);
      Game1.fadeToBlackAlpha = 0.99f;
      Game1.screenGlow = false;
      Game1.player.temporarilyInvincible = false;
      Game1.player.temporaryInvincibilityTimer = 0;
      Game1.displayFarmer = true;
    }

    public virtual bool isActionableTile(int xTile, int yTile, Farmer who) => this.humanDoor.X >= 0 && xTile == (int) (NetFieldBase<int, NetInt>) this.tileX + this.humanDoor.X && yTile == (int) (NetFieldBase<int, NetInt>) this.tileY + this.humanDoor.Y || this.animalDoor.X >= 0 && xTile == (int) (NetFieldBase<int, NetInt>) this.tileX + this.animalDoor.X && yTile == (int) (NetFieldBase<int, NetInt>) this.tileY + this.animalDoor.Y;

    public virtual void performActionOnBuildingPlacement()
    {
      if (!(Game1.getLocationFromName("Farm") is Farm locationFromName))
        return;
      for (int index1 = 0; index1 < (int) (NetFieldBase<int, NetInt>) this.tilesHigh; ++index1)
      {
        for (int index2 = 0; index2 < (int) (NetFieldBase<int, NetInt>) this.tilesWide; ++index2)
        {
          Vector2 key = new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + index2), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + index1));
          locationFromName.terrainFeatures.Remove(key);
        }
      }
      BluePrint bluePrint = new BluePrint((string) (NetFieldBase<string, NetString>) this.buildingType);
      if (bluePrint == null || bluePrint.additionalPlacementTiles == null || bluePrint.additionalPlacementTiles.Count <= 0)
        return;
      foreach (Point additionalPlacementTile in bluePrint.additionalPlacementTiles)
      {
        Vector2 key = new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + additionalPlacementTile.X), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + additionalPlacementTile.Y));
        locationFromName.terrainFeatures.Remove(key);
      }
    }

    public virtual void performActionOnConstruction(GameLocation location)
    {
      this.load();
      location.playSound("axchop");
      this.newConstructionTimer.Value = (bool) (NetFieldBase<bool, NetBool>) this.magical || (int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft <= 0 ? 2000 : 1000;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.magical)
      {
        location.playSound("axchop");
        for (int tileX = (int) (NetFieldBase<int, NetInt>) this.tileX; tileX < (int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide; ++tileX)
        {
          for (int tileY = (int) (NetFieldBase<int, NetInt>) this.tileY; tileY < (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh; ++tileY)
          {
            for (int index = 0; index < 5; ++index)
              location.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.random.NextDouble() < 0.5 ? 46 : 12, new Vector2((float) tileX, (float) tileY) * 64f + new Vector2((float) Game1.random.Next(-16, 32), (float) Game1.random.Next(-16, 32)), Color.White, 10, Game1.random.NextDouble() < 0.5)
              {
                delayBeforeAnimationStart = Math.Max(0, Game1.random.Next(-200, 400)),
                motion = new Vector2(0.0f, -1f),
                interval = (float) Game1.random.Next(50, 80)
              });
            location.temporarySprites.Add(new TemporaryAnimatedSprite(14, new Vector2((float) tileX, (float) tileY) * 64f + new Vector2((float) Game1.random.Next(-16, 32), (float) Game1.random.Next(-16, 32)), Color.White, 10, Game1.random.NextDouble() < 0.5));
          }
        }
        for (int index = 0; index < 8; ++index)
          DelayedAction.playSoundAfterDelay("dirtyHit", 250 + index * 150);
      }
      else
      {
        for (int index = 0; index < 8; ++index)
          DelayedAction.playSoundAfterDelay("dirtyHit", 100 + index * 210);
        Game1.flashAlpha = 2f;
        location.playSound("wand");
        for (int index1 = 0; index1 < this.getSourceRectForMenu().Width / 16 * 2; ++index1)
        {
          for (int index2 = this.getSourceRect().Height / 16 * 2; index2 >= 0; --index2)
          {
            location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(666, 1851, 8, 8), 40f, 4, 2, new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX, (float) (int) (NetFieldBase<int, NetInt>) this.tileY) * 64f + new Vector2((float) (index1 * 64 / 2), (float) (index2 * 64 / 2 - this.getSourceRect().Height * 4 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64)) + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-32, 32)), false, false)
            {
              layerDepth = (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + (double) index1 / 10000.0),
              pingPong = true,
              delayBeforeAnimationStart = (this.getSourceRect().Height / 16 * 2 - index2) * 100,
              scale = 4f,
              alphaFade = 0.01f,
              color = Color.AliceBlue
            });
            location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(666, 1851, 8, 8), 40f, 4, 2, new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX, (float) (int) (NetFieldBase<int, NetInt>) this.tileY) * 64f + new Vector2((float) (index1 * 64 / 2), (float) (index2 * 64 / 2 - this.getSourceRect().Height * 4 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64)) + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-32, 32)), false, false)
            {
              layerDepth = (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + (double) index1 / 10000.0 + 9.99999974737875E-05),
              pingPong = true,
              delayBeforeAnimationStart = (this.getSourceRect().Height / 16 * 2 - index2) * 100,
              scale = 4f,
              alphaFade = 0.01f,
              color = Color.AliceBlue
            });
          }
        }
      }
    }

    public virtual void performActionOnDemolition(GameLocation location)
    {
      if (this.indoors.Value != null)
        Game1.multiplayer.broadcastRemoveLocationFromLookup(this.indoors.Value);
      this.indoors.Value = (GameLocation) null;
    }

    public virtual List<Item> GetAdditionalItemsToCheckBeforeDemolish() => (List<Item>) null;

    public virtual void BeforeDemolish()
    {
      List<Item> quest_items = new List<Item>();
      Action<Item> action = (Action<Item>) (item =>
      {
        if (item == null || !(item is StardewValley.Object) || !(item as StardewValley.Object).questItem.Value)
          return;
        Item one = item.getOne();
        one.Stack = item.Stack;
        quest_items.Add(one);
      });
      List<Item> checkBeforeDemolish = this.GetAdditionalItemsToCheckBeforeDemolish();
      if (checkBeforeDemolish != null)
      {
        foreach (Item obj in checkBeforeDemolish)
          action(obj);
      }
      if (this is Mill)
      {
        foreach (Item obj in (NetList<Item, NetRef<Item>>) (this as Mill).output.Value.items)
          action(obj);
      }
      if (this is JunimoHut)
      {
        foreach (Item obj in (NetList<Item, NetRef<Item>>) (this as JunimoHut).output.Value.items)
          action(obj);
      }
      if (this.indoors.Value != null)
      {
        Utility.iterateAllItemsHere(this.indoors.Value, action);
        if (this.indoors.Value is Cabin)
          Utility.iterateAllItemsHere((GameLocation) (Game1.getLocationFromName((this.indoors.Value as Cabin).GetCellarName()) as Cellar), action);
      }
      if (quest_items.Count <= 0)
        return;
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:NewLostAndFoundItems"));
      for (int index = 0; index < quest_items.Count; ++index)
        Game1.player.team.returnedDonations.Add(quest_items[index]);
    }

    public virtual void performActionOnUpgrade(GameLocation location)
    {
    }

    public virtual string isThereAnythingtoPreventConstruction(
      GameLocation location,
      Vector2 tile_location)
    {
      return (string) null;
    }

    public virtual bool performActiveObjectDropInAction(Farmer who, bool probe) => false;

    public virtual void performToolAction(Tool t, int tileX, int tileY)
    {
    }

    public virtual void updateWhenFarmNotCurrentLocation(GameTime time)
    {
      if (this.netBuildingPaintColor.Value != null)
        this.netBuildingPaintColor.Value.Poll(new Action(this.resetTexture));
      if ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer <= 0)
        return;
      this.newConstructionTimer.Value -= time.ElapsedGameTime.Milliseconds;
      if ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0 || !(bool) (NetFieldBase<bool, NetBool>) this.magical)
        return;
      this.daysOfConstructionLeft.Value = 0;
    }

    public virtual void Update(GameTime time)
    {
      this.alpha.Value = Math.Min(1f, this.alpha.Value + 0.05f);
      int num = this.tilesHigh.Get();
      if (!this.fadeWhenPlayerIsBehind.Value || !Game1.player.GetBoundingBox().Intersects(new Rectangle(64 * (int) (NetFieldBase<int, NetInt>) this.tileX, 64 * ((int) (NetFieldBase<int, NetInt>) this.tileY + (-(this.getSourceRectForMenu().Height / 16) + num)), (int) (NetFieldBase<int, NetInt>) this.tilesWide * 64, (this.getSourceRectForMenu().Height / 16 - num) * 64 + 32)))
        return;
      this.alpha.Value = Math.Max(0.4f, this.alpha.Value - 0.09f);
    }

    public virtual void showUpgradeAnimation(GameLocation location)
    {
      this.color.Value = Color.White;
      location.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.getUpgradeSignLocation() + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-16, 16)), Color.Beige, 10, Game1.random.NextDouble() < 0.5, 75f)
      {
        motion = new Vector2(0.0f, -0.5f),
        acceleration = new Vector2(-0.02f, 0.01f),
        delayBeforeAnimationStart = Game1.random.Next(100),
        layerDepth = 0.89f
      });
      location.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.getUpgradeSignLocation() + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-16, 16)), Color.Beige, 10, Game1.random.NextDouble() < 0.5, 75f)
      {
        motion = new Vector2(0.0f, -0.5f),
        acceleration = new Vector2(-0.02f, 0.01f),
        delayBeforeAnimationStart = Game1.random.Next(40),
        layerDepth = 0.89f
      });
    }

    public virtual Vector2 getUpgradeSignLocation() => this.indoors.Value != null && this.indoors.Value is Shed ? new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + 5), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + 1)) * 64f + new Vector2(-12f, -16f) : new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 32), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 - 32));

    public virtual string getNameOfNextUpgrade()
    {
      string lower = this.buildingType.Value.ToLower();
      if (lower == "coop")
        return "Big Coop";
      if (lower == "big coop")
        return "Deluxe Coop";
      if (lower == "barn")
        return "Big Barn";
      if (lower == "big barn")
        return "Deluxe Barn";
      return lower == "shed" ? "Big Shed" : "well";
    }

    public virtual void showDestroyedAnimation(GameLocation location)
    {
      for (int tileX = (int) (NetFieldBase<int, NetInt>) this.tileX; tileX < (int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide; ++tileX)
      {
        for (int tileY = (int) (NetFieldBase<int, NetInt>) this.tileY; tileY < (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh; ++tileY)
        {
          location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, new Vector2((float) (tileX * 64), (float) (tileY * 64)) + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-16, 16)), false, Game1.random.NextDouble() < 0.5)
          {
            delayBeforeAnimationStart = Game1.random.Next(300)
          });
          location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, new Vector2((float) (tileX * 64), (float) (tileY * 64)) + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-16, 16)), false, Game1.random.NextDouble() < 0.5)
          {
            delayBeforeAnimationStart = 250 + Game1.random.Next(300)
          });
          location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(276, 1985, 12, 11), new Vector2((float) tileX, (float) tileY) * 64f + new Vector2(32f, -32f) + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-16, 16)), false, 0.0f, Color.White)
          {
            interval = 30f,
            totalNumberOfLoops = 99999,
            animationLength = 4,
            scale = 4f,
            alphaFade = 0.01f
          });
        }
      }
    }

    public virtual void dayUpdate(int dayOfMonth)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0 && !Utility.isFestivalDay(dayOfMonth, Game1.currentSeason))
      {
        --this.daysOfConstructionLeft.Value;
        if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
          return;
        Game1.player.checkForQuestComplete((NPC) null, -1, -1, (Item) null, (string) (NetFieldBase<string, NetString>) this.buildingType, 8);
        if (!this.buildingType.Equals((object) "Slime Hutch") || this.indoors.Value == null)
          return;
        this.indoors.Value.objects.Add(new Vector2(1f, 4f), new StardewValley.Object(new Vector2(1f, 4f), 156)
        {
          Fragility = 2
        });
        if (Game1.player.mailReceived.Contains("slimeHutchBuilt"))
          return;
        Game1.player.mailReceived.Add("slimeHutchBuilt");
      }
      else
      {
        if ((int) (NetFieldBase<int, NetInt>) this.daysUntilUpgrade > 0 && !Utility.isFestivalDay(dayOfMonth, Game1.currentSeason))
        {
          --this.daysUntilUpgrade.Value;
          if ((int) (NetFieldBase<int, NetInt>) this.daysUntilUpgrade <= 0)
          {
            Game1.player.checkForQuestComplete((NPC) null, -1, -1, (Item) null, this.getNameOfNextUpgrade(), 8);
            BluePrint bluePrint = new BluePrint(this.getNameOfNextUpgrade());
            this.indoors.Value.mapPath.Value = "Maps\\" + bluePrint.mapToWarpTo;
            this.indoors.Value.name.Value = bluePrint.mapToWarpTo;
            this.buildingType.Value = bluePrint.name;
            if (this.indoors.Value is AnimalHouse)
            {
              ((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) this.indoors).resetPositionsOfAllAnimals();
              ((AnimalHouse) (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) this.indoors).animalLimit.Value += 4;
              ((GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) this.indoors).loadLights();
            }
            this.upgrade();
            this.resetTexture();
          }
        }
        if (this.indoors.Value != null)
          this.indoors.Value.DayUpdate(dayOfMonth);
        if (!this.buildingType.Value.Contains("Deluxe"))
          return;
        (this.indoors.Value as AnimalHouse).feedAllAnimals();
      }
    }

    public virtual void upgrade()
    {
      if (!this.buildingType.Equals((object) "Big Shed"))
        return;
      (this.indoors.Value as Shed).setUpgradeLevel(1);
      this.updateInteriorWarps(this.indoors.Value);
    }

    public virtual Rectangle getSourceRect()
    {
      if (!this.buildingType.Value.Contains("Cabin"))
        return this.texture.Value.Bounds;
      int num = 0;
      if (this.indoors.Value is Cabin cabin)
        num = Math.Min(cabin.upgradeLevel, 2);
      return new Rectangle(num * 80, 0, 80, 112);
    }

    public virtual Rectangle getSourceRectForMenu() => this.getSourceRect();

    public virtual void updateInteriorWarps(GameLocation interior = null)
    {
      if (interior == null)
        interior = this.indoors.Value;
      if (interior == null)
        return;
      foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) interior.warps)
      {
        warp.TargetX = this.humanDoor.X + (int) (NetFieldBase<int, NetInt>) this.tileX;
        warp.TargetY = this.humanDoor.Y + (int) (NetFieldBase<int, NetInt>) this.tileY + 1;
      }
    }

    protected virtual GameLocation getIndoors(string nameOfIndoorsWithoutUnique)
    {
      GameLocation interior = (GameLocation) null;
      if (this.buildingType.Value.Equals("Slime Hutch"))
        interior = (GameLocation) new SlimeHutch("Maps\\" + nameOfIndoorsWithoutUnique, (string) (NetFieldBase<string, NetString>) this.buildingType);
      else if (this.buildingType.Value.Equals("Shed"))
        interior = (GameLocation) new Shed("Maps\\" + nameOfIndoorsWithoutUnique, (string) (NetFieldBase<string, NetString>) this.buildingType);
      else if (this.buildingType.Value.Equals("Big Shed"))
        interior = (GameLocation) new Shed("Maps\\" + nameOfIndoorsWithoutUnique, (string) (NetFieldBase<string, NetString>) this.buildingType);
      else if (this.buildingType.Value.Contains("Cabin"))
        interior = (GameLocation) new Cabin("Maps\\Cabin");
      else if (nameOfIndoorsWithoutUnique != null && nameOfIndoorsWithoutUnique.Length > 0 && !nameOfIndoorsWithoutUnique.Equals("null"))
        interior = new GameLocation("Maps\\" + nameOfIndoorsWithoutUnique, (string) (NetFieldBase<string, NetString>) this.buildingType);
      if (interior != null)
      {
        interior.uniqueName.Value = nameOfIndoorsWithoutUnique + GuidHelper.NewGuid().ToString();
        interior.IsFarm = true;
        interior.isStructure.Value = true;
        this.updateInteriorWarps(interior);
      }
      return interior;
    }

    public virtual Point getPointForHumanDoor() => new Point((int) (NetFieldBase<int, NetInt>) this.tileX + this.humanDoor.Value.X, (int) (NetFieldBase<int, NetInt>) this.tileY + this.humanDoor.Value.Y);

    public virtual Rectangle getRectForHumanDoor() => new Rectangle(this.getPointForHumanDoor().X * 64, this.getPointForHumanDoor().Y * 64, 64, 64);

    public virtual Rectangle getRectForAnimalDoor() => new Rectangle((this.animalDoor.X + (int) (NetFieldBase<int, NetInt>) this.tileX) * 64, ((int) (NetFieldBase<int, NetInt>) this.tileY + this.animalDoor.Y) * 64, 64, 64);

    public virtual void load()
    {
      GameLocation indoors = this.getIndoors(this.nameOfIndoorsWithoutUnique);
      if (indoors != null)
      {
        indoors.characters.Set((ICollection<NPC>) this.indoors.Value.characters);
        indoors.netObjects.MoveFrom(this.indoors.Value.netObjects);
        indoors.terrainFeatures.MoveFrom(this.indoors.Value.terrainFeatures);
        indoors.IsFarm = true;
        indoors.IsOutdoors = false;
        indoors.isStructure.Value = true;
        indoors.miniJukeboxCount.Set(this.indoors.Value.miniJukeboxCount.Value);
        indoors.miniJukeboxTrack.Set(this.indoors.Value.miniJukeboxTrack.Value);
        indoors.uniqueName.Value = (string) (NetFieldBase<string, NetString>) this.indoors.Value.uniqueName;
        if (indoors.uniqueName.Value == null)
          indoors.uniqueName.Value = this.nameOfIndoorsWithoutUnique + ((int) (NetFieldBase<int, NetInt>) this.tileX * 2000 + (int) (NetFieldBase<int, NetInt>) this.tileY).ToString();
        indoors.numberOfSpawnedObjectsOnMap = this.indoors.Value.numberOfSpawnedObjectsOnMap;
        if (this.indoors.Value is Shed)
          ((Shed) indoors).upgradeLevel.Set(((Shed) this.indoors.Value).upgradeLevel.Value);
        if (this.indoors.Value is AnimalHouse)
        {
          ((AnimalHouse) indoors).animals.MoveFrom(((AnimalHouse) this.indoors.Value).animals);
          ((AnimalHouse) indoors).animalsThatLiveHere.Set((IList<long>) ((AnimalHouse) this.indoors.Value).animalsThatLiveHere);
          foreach (KeyValuePair<long, FarmAnimal> pair in ((AnimalHouse) indoors).animals.Pairs)
            pair.Value.reload(this);
        }
        if (this.indoors.Value != null)
        {
          indoors.furniture.Set((ICollection<Furniture>) this.indoors.Value.furniture);
          foreach (Furniture furniture in indoors.furniture)
            furniture.updateDrawPosition();
        }
        DecoratableLocation decoratableLocation = this.indoors.Value as DecoratableLocation;
        if (this.indoors.Value is Cabin)
        {
          Cabin cabin = indoors as Cabin;
          cabin.fridge.Value = (this.indoors.Value as Cabin).fridge.Value;
          cabin.fireplaceOn.Value = (this.indoors.Value as Cabin).fireplaceOn.Value;
          cabin.farmhand.Set((Farmer) (NetFieldBase<Farmer, NetRef<Farmer>>) (this.indoors.Value as Cabin).farmhand);
          if (cabin.farmhand.Value != null)
          {
            SaveGame.loadDataToFarmer((Farmer) (NetFieldBase<Farmer, NetRef<Farmer>>) cabin.farmhand);
            cabin.resetFarmhandState();
          }
        }
        if (this.indoors.Value != null)
          indoors.TransferDataFromSavedLocation(this.indoors.Value);
        this.indoors.Value = indoors;
        this.updateInteriorWarps();
        for (int index = this.indoors.Value.characters.Count - 1; index >= 0; --index)
          SaveGame.initializeCharacter(this.indoors.Value.characters[index], this.indoors.Value);
        foreach (TerrainFeature terrainFeature in this.indoors.Value.terrainFeatures.Values)
          terrainFeature.loadSprite();
        foreach (KeyValuePair<Vector2, StardewValley.Object> pair in this.indoors.Value.objects.Pairs)
        {
          pair.Value.initializeLightSource(pair.Key);
          pair.Value.reloadSprite();
        }
        if (this.indoors.Value is AnimalHouse)
        {
          AnimalHouse animalHouse = this.indoors.Value as AnimalHouse;
          string str = this.buildingType.Value.Split(' ')[0];
          if (!(str == "Big"))
          {
            if (str == "Deluxe")
              animalHouse.animalLimit.Value = 12;
            else
              animalHouse.animalLimit.Value = 4;
          }
          else
            animalHouse.animalLimit.Value = 8;
        }
      }
      BluePrint bluePrint = new BluePrint(this.buildingType.Value);
      if (bluePrint == null)
        return;
      this.humanDoor.X = bluePrint.humanDoor.X;
      this.humanDoor.Y = bluePrint.humanDoor.Y;
      this.additionalPlacementTiles.Clear();
      this.additionalPlacementTiles.AddRange((IEnumerable<Point>) bluePrint.additionalPlacementTiles);
    }

    public bool isUnderConstruction() => (int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0;

    public bool occupiesTile(Vector2 tile) => (double) tile.X >= (double) (int) (NetFieldBase<int, NetInt>) this.tileX && (double) tile.X < (double) ((int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide) && (double) tile.Y >= (double) (int) (NetFieldBase<int, NetInt>) this.tileY && (double) tile.Y < (double) ((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh);

    public virtual bool isTilePassable(Vector2 tile)
    {
      bool flag = this.occupiesTile(tile);
      return this.isCabin && flag && (int) tile.Y == (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1 || !flag;
    }

    public virtual bool isTileOccupiedForPlacement(Vector2 tile, StardewValley.Object to_place) => !this.isTilePassable(tile) && (!this.isCabin || to_place == null || (int) tile.Y != (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1);

    public virtual bool isTileFishable(Vector2 tile) => false;

    public virtual bool CanRefillWateringCan() => (int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft <= 0 && this.buildingType.Equals((object) "Well");

    public virtual bool intersects(Rectangle boundingBox)
    {
      if (!this.isCabin || (int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
        return new Rectangle((int) (NetFieldBase<int, NetInt>) this.tileX * 64, (int) (NetFieldBase<int, NetInt>) this.tileY * 64, (int) (NetFieldBase<int, NetInt>) this.tilesWide * 64, (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64).Intersects(boundingBox);
      return new Rectangle(((int) (NetFieldBase<int, NetInt>) this.tileX + 4) * 64, ((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1) * 64, 64, 64).Intersects(boundingBox) || new Rectangle((int) (NetFieldBase<int, NetInt>) this.tileX * 64, (int) (NetFieldBase<int, NetInt>) this.tileY * 64, (int) (NetFieldBase<int, NetInt>) this.tilesWide * 64, ((int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1) * 64).Intersects(boundingBox);
    }

    public virtual void drawInMenu(SpriteBatch b, int x, int y)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.tilesWide <= 8)
      {
        this.drawShadow(b, x, y);
        b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Rectangle?(this.getSourceRect()), (Color) (NetFieldBase<Color, NetColor>) this.color, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 0.89f);
      }
      else
      {
        int num1 = 108;
        int num2 = 28;
        b.Draw(this.texture.Value, new Vector2((float) (x + num1), (float) (y + num2)), new Rectangle?(new Rectangle(this.getSourceRect().Width / 2 - 64, this.getSourceRect().Height - 136 - 2, 122, 138)), (Color) (NetFieldBase<Color, NetColor>) this.color, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 0.89f);
      }
    }

    public virtual void drawBackground(SpriteBatch b)
    {
    }

    public virtual void draw(SpriteBatch b)
    {
      if (this.isMoving)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0 || (int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0)
      {
        this.drawInConstruction(b);
      }
      else
      {
        this.drawShadow(b);
        float layerDepth = (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000f;
        if (this.isCabin)
          layerDepth = (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + ((int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1)) * 64) / 10000f;
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64))), new Rectangle?(this.getSourceRect()), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, (float) this.getSourceRect().Height), 4f, SpriteEffects.None, layerDepth);
        if ((bool) (NetFieldBase<bool, NetBool>) this.magical && this.buildingType.Value.Equals("Gold Clock"))
        {
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 92), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 - 40))), new Rectangle?(Town.hourHandSource), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, (float) (2.0 * Math.PI * ((double) (Game1.timeOfDay % 1200) / 1200.0) + (double) Game1.gameTimeInterval / 7000.0 / 23.0), new Vector2(2.5f, 8f), 3f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + 9.99999974737875E-05));
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 92), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 - 40))), new Rectangle?(Town.minuteHandSource), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, (float) (2.0 * Math.PI * ((double) (Game1.timeOfDay % 1000 % 100 % 60) / 60.0) + (double) Game1.gameTimeInterval / 7000.0 * 1.01999998092651), new Vector2(2.5f, 12f), 3f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + 0.000110000000859145));
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 92), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 - 40))), new Rectangle?(Town.clockNub), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(2f, 2f), 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + 0.000119999996968545));
        }
        if ((int) (NetFieldBase<int, NetInt>) this.daysUntilUpgrade <= 0 || this.indoors.Value == null || !(this.indoors.Value is Shed))
          return;
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.getUpgradeSignLocation()), new Rectangle?(new Rectangle(367, 309, 16, 15)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + 9.99999974737875E-05));
      }
    }

    public virtual void drawShadow(SpriteBatch b, int localX = -1, int localY = -1)
    {
      Vector2 position = localX == -1 ? Game1.GlobalToLocal(new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64))) : new Vector2((float) localX, (float) (localY + this.getSourceRectForMenu().Height * 4));
      b.Draw(Game1.mouseCursors, position, new Rectangle?(Building.leftShadow), Color.White * (localX == -1 ? (float) (NetFieldBase<float, NetFloat>) this.alpha : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
      for (int index = 1; index < (int) (NetFieldBase<int, NetInt>) this.tilesWide - 1; ++index)
        b.Draw(Game1.mouseCursors, position + new Vector2((float) (index * 64), 0.0f), new Rectangle?(Building.middleShadow), Color.White * (localX == -1 ? (float) (NetFieldBase<float, NetFloat>) this.alpha : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
      b.Draw(Game1.mouseCursors, position + new Vector2((float) (((int) (NetFieldBase<int, NetInt>) this.tilesWide - 1) * 64), 0.0f), new Rectangle?(Building.rightShadow), Color.White * (localX == -1 ? (float) (NetFieldBase<float, NetFloat>) this.alpha : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
    }

    public virtual void OnStartMove()
    {
    }

    public virtual void OnEndMove()
    {
    }

    public Point getPorchStandingSpot() => this.isCabin ? new Point((int) (NetFieldBase<int, NetInt>) this.tileX + 1, (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1) : new Point(0, 0);

    public virtual bool doesTileHaveProperty(
      int tile_x,
      int tile_y,
      string property_name,
      string layer_name,
      ref string property_value)
    {
      if (this.isCabin)
      {
        if (tile_x == this.getMailboxPosition().X && tile_y == this.getMailboxPosition().Y && property_name == "Action" && layer_name == "Buildings")
        {
          property_value = "Mailbox";
          return true;
        }
        if (tile_x == this.getPointForHumanDoor().X && tile_y == this.getPointForHumanDoor().Y && property_name == "Action" && layer_name == "Buildings")
        {
          property_value = "Warp  3 11" + this.indoors.Value.uniqueName.Value;
          return true;
        }
        if (tile_y == (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1)
        {
          if (property_name == "NoSpawn")
          {
            property_value = "All";
            return true;
          }
          if (property_name == "Buildable")
          {
            property_value = "f";
            return true;
          }
          if (property_name == "NoFurniture")
          {
            if (tile_x == (int) (NetFieldBase<int, NetInt>) this.tileX + 1)
            {
              property_value = "T";
              return true;
            }
            if (tile_x == (int) (NetFieldBase<int, NetInt>) this.tileX + 2)
            {
              property_value = "T";
              return true;
            }
            if (tile_x == (int) (NetFieldBase<int, NetInt>) this.tileX + 4)
            {
              property_value = "T";
              return true;
            }
          }
          if (property_name == "Diggable" && layer_name == "Back")
          {
            property_value = (string) null;
            return true;
          }
          if (property_name == "Type" && layer_name == "Back")
          {
            property_value = "Wood";
            return true;
          }
        }
      }
      return false;
    }

    public Point getMailboxPosition() => this.isCabin ? new Point((int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide - 1, (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1) : new Point(68, 16);

    public virtual int GetAdditionalTilePropertyRadius() => 0;

    public void removeOverlappingBushes(GameLocation location)
    {
      for (int tileX = (int) (NetFieldBase<int, NetInt>) this.tileX; tileX < (int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide; ++tileX)
      {
        for (int tileY = (int) (NetFieldBase<int, NetInt>) this.tileY; tileY < (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh; ++tileY)
        {
          if (location.isTerrainFeatureAt(tileX, tileY))
          {
            LargeTerrainFeature terrainFeatureAt = location.getLargeTerrainFeatureAt(tileX, tileY);
            if (terrainFeatureAt != null && terrainFeatureAt is Bush)
              location.largeTerrainFeatures.Remove(terrainFeatureAt);
          }
        }
      }
    }

    public virtual void drawInConstruction(SpriteBatch b)
    {
      int height = Math.Min(16, Math.Max(0, (int) (16.0 - (double) (int) (NetFieldBase<int, NetInt>) this.newConstructionTimer / 1000.0 * 16.0)));
      float num1 = (float) (2000 - (int) (NetFieldBase<int, NetInt>) this.newConstructionTimer) / 2000f;
      if ((bool) (NetFieldBase<bool, NetBool>) this.magical || (int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft <= 0)
      {
        this.drawShadow(b);
        int num2 = (int) ((double) (this.getSourceRect().Height * 4) * (1.0 - (double) num1));
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64 + num2 + 4 - num2 % 4))), new Rectangle?(new Rectangle(0, this.getSourceRect().Bottom - (int) ((double) num1 * (double) this.getSourceRect().Height), this.getSourceRectForMenu().Width, (int) ((double) this.getSourceRect().Height * (double) num1))), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, (float) this.getSourceRect().Height), 4f, SpriteEffects.None, (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000f);
        if ((bool) (NetFieldBase<bool, NetBool>) this.magical)
        {
          for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.tilesWide * 4; ++index)
          {
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + index * 16), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 - this.getSourceRect().Height * 4 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64) + (float) (this.getSourceRect().Height * 4) * (1f - num1))) + new Vector2((float) Game1.random.Next(-1, 2), (float) (Game1.random.Next(-1, 2) - (index % 2 == 0 ? 32 : 8))), new Rectangle?(new Rectangle(536 + ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer + index * 4) % 56 / 8 * 8, 1945, 8, 8)), index % 2 == 1 ? Color.Pink * (float) (NetFieldBase<float, NetFloat>) this.alpha : Color.LightPink * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, 0.0f), (float) (4.0 + (double) Game1.random.Next(100) / 100.0), SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + 9.99999974737875E-05));
            if (index % 2 == 0)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + index * 16), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 - this.getSourceRect().Height * 4 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64) + (float) (this.getSourceRect().Height * 4) * (1f - num1))) + new Vector2((float) Game1.random.Next(-1, 2), (float) (Game1.random.Next(-1, 2) + (index % 2 == 0 ? 32 : 8))), new Rectangle?(new Rectangle(536 + ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer + index * 4) % 56 / 8 * 8, 1945, 8, 8)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, 0.0f), (float) (4.0 + (double) Game1.random.Next(100) / 100.0), SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + 9.99999974737875E-05));
          }
        }
        else
        {
          for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.tilesWide * 4; ++index)
          {
            b.Draw(Game1.animations, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 - 16 + index * 16), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 - this.getSourceRect().Height * 4 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64) + (float) (this.getSourceRect().Height * 4) * (1f - num1))) + new Vector2((float) Game1.random.Next(-1, 2), (float) (Game1.random.Next(-1, 2) - (index % 2 == 0 ? 32 : 8))), new Rectangle?(new Rectangle(((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer + index * 20) % 304 / 38 * 64, 768, 64, 64)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha * ((float) (int) (NetFieldBase<int, NetInt>) this.newConstructionTimer / 500f), 0.0f, new Vector2(0.0f, 0.0f), 1f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + 9.99999974737875E-05));
            if (index % 2 == 0)
              b.Draw(Game1.animations, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 - 16 + index * 16), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 - this.getSourceRect().Height * 4 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64) + (float) (this.getSourceRect().Height * 4) * (1f - num1))) + new Vector2((float) Game1.random.Next(-1, 2), (float) (Game1.random.Next(-1, 2) - (index % 2 == 0 ? 32 : 8))), new Rectangle?(new Rectangle(((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer + index * 20) % 400 / 50 * 64, 2944, 64, 64)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.alpha * ((float) (int) (NetFieldBase<int, NetInt>) this.newConstructionTimer / 500f), 0.0f, new Vector2(0.0f, 0.0f), 1f, SpriteEffects.None, (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000.0 + 9.99999974737875E-05));
          }
        }
      }
      else
      {
        bool flag = (int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft == 1;
        for (int tileX = (int) (NetFieldBase<int, NetInt>) this.tileX; tileX < (int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide; ++tileX)
        {
          for (int tileY = (int) (NetFieldBase<int, NetInt>) this.tileY; tileY < (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh; ++tileY)
          {
            if (tileX == (int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide / 2 && tileY == (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1)
            {
              if (flag)
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4 + 16 - 4)), new Rectangle?(new Rectangle(367, 277, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4)) + ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(367, 309, 16, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (tileY * 64 + 64 - 1) / 10000f);
            }
            else if (tileX == (int) (NetFieldBase<int, NetInt>) this.tileX && tileY == (int) (NetFieldBase<int, NetInt>) this.tileY)
            {
              if (flag)
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4 + 16)), new Rectangle?(new Rectangle(351, 261, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4)) + ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(351, 293, 16, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (tileY * 64 + 64 - 1) / 10000f);
            }
            else if (tileX == (int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide - 1 && tileY == (int) (NetFieldBase<int, NetInt>) this.tileY)
            {
              if (flag)
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4 + 16)), new Rectangle?(new Rectangle(383, 261, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4)) + ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(383, 293, 16, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (tileY * 64 + 64 - 1) / 10000f);
            }
            else if (tileX == (int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide - 1 && tileY == (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1)
            {
              if (flag)
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4 + 16)), new Rectangle?(new Rectangle(383, 277, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4)) + ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(383, 325, 16, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (tileY * 64) / 10000f);
            }
            else if (tileX == (int) (NetFieldBase<int, NetInt>) this.tileX && tileY == (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1)
            {
              if (flag)
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4 + 16)), new Rectangle?(new Rectangle(351, 277, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4)) + ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(351, 325, 16, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (tileY * 64) / 10000f);
            }
            else if (tileX == (int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide - 1)
            {
              if (flag)
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4 + 16)), new Rectangle?(new Rectangle(383, 261, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4)) + ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(383, 309, 16, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (tileY * 64) / 10000f);
            }
            else if (tileY == (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1)
            {
              if (flag)
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4 + 16)), new Rectangle?(new Rectangle(367, 277, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4)) + ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(367, 325, 16, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (tileY * 64) / 10000f);
            }
            else if (tileX == (int) (NetFieldBase<int, NetInt>) this.tileX)
            {
              if (flag)
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4 + 16)), new Rectangle?(new Rectangle(351, 261, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4)) + ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(351, 309, 16, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (tileY * 64) / 10000f);
            }
            else if (tileY == (int) (NetFieldBase<int, NetInt>) this.tileY)
            {
              if (flag)
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4 + 16)), new Rectangle?(new Rectangle(367, 261, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4)) + ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(367, 293, 16, height)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (tileY * 64 + 64 - 1) / 10000f);
            }
            else if (flag)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) tileX, (float) tileY) * 64f) + new Vector2(0.0f, (float) (64 - height * 4 + 16)), new Rectangle?(new Rectangle(367, 261, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
          }
        }
      }
    }
  }
}
