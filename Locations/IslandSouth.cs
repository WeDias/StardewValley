// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandSouth
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class IslandSouth : IslandLocation
  {
    [XmlIgnore]
    public const int ISLAND_DEPART_EVENT_ID = -157039427;
    [XmlIgnore]
    protected int _boatDirection;
    [XmlIgnore]
    public Texture2D boatTexture;
    [XmlIgnore]
    public Vector2 boatPosition;
    [XmlIgnore]
    protected int _boatOffset;
    [XmlIgnore]
    protected float _nextBubble;
    [XmlIgnore]
    protected float _nextSlosh;
    [XmlIgnore]
    protected float _nextSmoke;
    [XmlIgnore]
    public LightSource boatLight;
    [XmlIgnore]
    public LightSource boatStringLight;
    [XmlElement("shouldToggleResort")]
    public readonly NetBool shouldToggleResort = new NetBool(false);
    [XmlElement("resortOpenToday")]
    public readonly NetBool resortOpenToday = new NetBool(true);
    [XmlElement("resortRestored")]
    public readonly NetBool resortRestored = new NetBool();
    [XmlElement("westernTurtleMoved")]
    public readonly NetBool westernTurtleMoved = new NetBool();
    [XmlIgnore]
    protected bool _parrotBoyHiding;
    [XmlIgnore]
    protected bool _isFirstVisit;
    [XmlIgnore]
    protected bool _exitsBlocked;
    [XmlIgnore]
    protected bool _sawFlameSprite;
    [XmlIgnore]
    public NetEvent0 moveTurtleEvent = new NetEvent0();
    private Microsoft.Xna.Framework.Rectangle turtle1Spot = new Microsoft.Xna.Framework.Rectangle(1088, 0, 192, 192);
    private Microsoft.Xna.Framework.Rectangle turtle2Spot = new Microsoft.Xna.Framework.Rectangle(0, 640, 256, 256);

    public IslandSouth()
    {
    }

    public IslandSouth(string map, string name)
      : base(map, name)
    {
      this.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(31f, 5f), 4, (GameLocation) this));
      this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(17, 22), new Microsoft.Xna.Framework.Rectangle(12, 18, 14, 7), 20, (Action) (() =>
      {
        Game1.addMailForTomorrow("Island_Resort", true, true);
        this.resortRestored.Value = true;
      }), (Func<bool>) (() => this.resortRestored.Value), "Resort", "Island_UpgradeHouse"));
      this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(5, 9), new Microsoft.Xna.Framework.Rectangle(1, 10, 3, 4), 10, (Action) (() =>
      {
        Game1.addMailForTomorrow("Island_Turtle", true, true);
        this.westernTurtleMoved.Value = true;
        this.moveTurtleEvent.Fire();
      }), (Func<bool>) (() => this.westernTurtleMoved.Value), "Turtle", "Island_FirstParrot"));
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.resortRestored, (INetSerializable) this.westernTurtleMoved, (INetSerializable) this.shouldToggleResort, (INetSerializable) this.resortOpenToday, (INetSerializable) this.moveTurtleEvent);
      this.resortRestored.InterpolationWait = false;
      this.resortRestored.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.ApplyResortRestore();
      });
      this.moveTurtleEvent.onEvent += new NetEvent0.Event(this.ApplyWesternTurtleMove);
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      if (l is IslandSouth)
      {
        IslandSouth islandSouth = l as IslandSouth;
        this.resortRestored.Value = islandSouth.resortRestored.Value;
        this.westernTurtleMoved.Value = islandSouth.westernTurtleMoved.Value;
        this.shouldToggleResort.Value = islandSouth.shouldToggleResort.Value;
        this.resortOpenToday.Value = islandSouth.resortOpenToday.Value;
      }
      base.TransferDataFromSavedLocation(l);
    }

    public override void DayUpdate(int dayOfMonth)
    {
      if (this.shouldToggleResort.Value)
      {
        this.resortOpenToday.Value = !this.resortOpenToday.Value;
        this.shouldToggleResort.Value = false;
        this.ApplyResortRestore();
      }
      base.DayUpdate(dayOfMonth);
    }

    public void ApplyResortRestore()
    {
      if (this.map != null)
        this.ApplyUnsafeMapOverride("Island_Resort", new Microsoft.Xna.Framework.Rectangle?(), new Microsoft.Xna.Framework.Rectangle(9, 15, 26, 16));
      this.removeTile(new Location(41, 28), "Buildings");
      this.removeTile(new Location(42, 28), "Buildings");
      this.removeTile(new Location(42, 29), "Buildings");
      this.removeTile(new Location(42, 30), "Front");
      this.removeTileProperty(42, 30, "Back", "Passable");
      if (!this.resortRestored.Value)
        return;
      if (this.resortOpenToday.Value)
      {
        this.removeTile(new Location(22, 21), "Buildings");
        this.removeTile(new Location(22, 22), "Buildings");
        this.removeTile(new Location(24, 21), "Buildings");
        this.removeTile(new Location(24, 22), "Buildings");
      }
      else
      {
        this.setMapTile(22, 21, 1405, "Buildings", (string) null);
        this.setMapTile(22, 22, 1437, "Buildings", (string) null);
        this.setMapTile(24, 21, 1405, "Buildings", (string) null);
        this.setMapTile(24, 22, 1437, "Buildings", (string) null);
      }
    }

    public void ApplyWesternTurtleMove()
    {
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(789);
      if (temporarySpriteById != null)
      {
        temporarySpriteById.motion = new Vector2(-2f, 0.0f);
        temporarySpriteById.yPeriodic = true;
        temporarySpriteById.yPeriodicRange = 8f;
        temporarySpriteById.yPeriodicLoopTime = 300f;
        temporarySpriteById.shakeIntensity = 1f;
      }
      this.localSound("shadowDie");
    }

    public override void seasonUpdate(string season, bool onLoad = false)
    {
    }

    public override void updateSeasonalTileSheets(Map map = null)
    {
    }

    private void parrotBoyLands(int extra)
    {
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(888);
      if (temporarySpriteById == null)
        return;
      temporarySpriteById.sourceRect.X = 0;
      temporarySpriteById.sourceRect.Y = 32;
      temporarySpriteById.sourceRectStartingPos.X = 0.0f;
      temporarySpriteById.sourceRectStartingPos.Y = 32f;
      temporarySpriteById.motion = new Vector2(4f, 0.0f);
      temporarySpriteById.acceleration = Vector2.Zero;
      temporarySpriteById.id = 888f;
      temporarySpriteById.animationLength = 4;
      temporarySpriteById.interval = 100f;
      temporarySpriteById.totalNumberOfLoops = 10;
      temporarySpriteById.drawAboveAlwaysFront = false;
      temporarySpriteById.layerDepth = 0.1f;
      this.temporarySprites.Add(temporarySpriteById);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this.moveTurtleEvent.Poll();
      if (this.boatLight != null)
        this.boatLight.position.Value = new Vector2(3f, 1f) * 64f + this.GetBoatPosition();
      if (this.boatStringLight != null)
        this.boatStringLight.position.Value = new Vector2(3f, 4f) * 64f + this.GetBoatPosition();
      if (this._parrotBoyHiding && Utility.isThereAFarmerWithinDistance(new Vector2(29f, 16f), 4, (GameLocation) this) == Game1.player)
      {
        TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
        if (temporarySpriteById != null)
        {
          temporarySpriteById.sourceRect.X = 0;
          temporarySpriteById.sourceRectStartingPos.X = 0.0f;
          temporarySpriteById.motion = new Vector2(3f, -10f);
          temporarySpriteById.acceleration = new Vector2(0.0f, 0.4f);
          temporarySpriteById.yStopCoordinate = 992;
          temporarySpriteById.shakeIntensity = 2f;
          temporarySpriteById.id = 888f;
          temporarySpriteById.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.parrotBoyLands);
          this.localSound("parrot_squawk");
        }
      }
      if (!this._exitsBlocked && !this._sawFlameSprite && Utility.isThereAFarmerWithinDistance(new Vector2(18f, 11f), 5, (GameLocation) this) == Game1.player)
      {
        Game1.addMailForTomorrow("Saw_Flame_Sprite_South", true);
        TemporaryAnimatedSprite temporarySpriteById1 = this.getTemporarySpriteByID(999);
        if (temporarySpriteById1 != null)
        {
          temporarySpriteById1.yPeriodic = false;
          temporarySpriteById1.xPeriodic = false;
          temporarySpriteById1.sourceRect.Y = 0;
          temporarySpriteById1.sourceRectStartingPos.Y = 0.0f;
          temporarySpriteById1.motion = new Vector2(0.0f, -4f);
          temporarySpriteById1.acceleration = new Vector2(0.0f, -0.04f);
        }
        this.localSound("magma_sprite_spot");
        TemporaryAnimatedSprite temporarySpriteById2 = this.getTemporarySpriteByID(998);
        if (temporarySpriteById2 != null)
        {
          temporarySpriteById2.yPeriodic = false;
          temporarySpriteById2.xPeriodic = false;
          temporarySpriteById2.motion = new Vector2(0.0f, -4f);
          temporarySpriteById2.acceleration = new Vector2(0.0f, -0.04f);
        }
        this._sawFlameSprite = true;
      }
      if (this.currentEvent == null || this.currentEvent.id != -157039427)
        return;
      if (this._boatDirection != 0)
      {
        this._boatOffset += this._boatDirection;
        foreach (NPC actor in this.currentEvent.actors)
        {
          actor.shouldShadowBeOffset = true;
          actor.drawOffset.Y = (float) this._boatOffset;
        }
        foreach (Farmer farmerActor in this.currentEvent.farmerActors)
        {
          farmerActor.shouldShadowBeOffset = true;
          farmerActor.drawOffset.Y = (float) this._boatOffset;
        }
      }
      if ((double) this._boatDirection != 0.0)
      {
        if ((double) this._nextBubble > 0.0)
        {
          this._nextBubble -= (float) time.ElapsedGameTime.TotalSeconds;
        }
        else
        {
          Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(64, 256, 192, 64);
          r.X += (int) this.GetBoatPosition().X;
          r.Y += (int) this.GetBoatPosition().Y;
          this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 50f, 9, 1, Utility.getRandomPositionInThisRectangle(r, Game1.random), false, false, 0.0f, 0.025f, Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            acceleration = new Vector2(0.0f, -0.25f * (float) Math.Sign(this._boatDirection))
          });
          this._nextBubble = 0.01f;
        }
        if ((double) this._nextSlosh > 0.0)
        {
          this._nextSlosh -= (float) time.ElapsedGameTime.TotalSeconds;
        }
        else
        {
          Game1.playSound("waterSlosh");
          this._nextSlosh = 0.5f;
        }
      }
      if ((double) this._nextSmoke > 0.0)
      {
        this._nextSmoke -= (float) time.ElapsedGameTime.TotalSeconds;
      }
      else
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 1600, 64, 128), 200f, 9, 1, new Vector2(2f, 2.5f) * 64f + this.GetBoatPosition(), false, false, 1f, 0.025f, Color.White, 1f, 0.025f, 0.0f, 0.0f)
        {
          acceleration = new Vector2(-0.25f, -0.15f)
        });
        this._nextSmoke = 0.2f;
      }
    }

    public override void cleanupBeforePlayerExit()
    {
      this.boatLight = (LightSource) null;
      this.boatStringLight = (LightSource) null;
      base.cleanupBeforePlayerExit();
    }

    public override bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character)
    {
      return this._exitsBlocked && position.Intersects(this.turtle1Spot) || !(bool) (NetFieldBase<bool, NetBool>) this.westernTurtleMoved && position.Intersects(this.turtle2Spot) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
    }

    public override bool isTilePlaceable(Vector2 tile_location, Item item = null)
    {
      Point point = Utility.Vector2ToPoint((tile_location + new Vector2(0.5f, 0.5f)) * 64f);
      return (!this._exitsBlocked || !this.turtle1Spot.Contains(point)) && ((bool) (NetFieldBase<bool, NetBool>) this.westernTurtleMoved || !this.turtle2Spot.Contains(point)) && base.isTilePlaceable(tile_location, item);
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (!this.resortRestored.Value)
        return;
      this.ApplyResortRestore();
    }

    protected override void resetLocalState()
    {
      this._isFirstVisit = false;
      if (!Game1.player.hasOrWillReceiveMail("Visited_Island"))
      {
        Game1.addMailForTomorrow("Visited_Island", true);
        this._isFirstVisit = true;
      }
      if (Game1.player.hasOrWillReceiveMail("Saw_Flame_Sprite_South"))
        this._sawFlameSprite = true;
      this._exitsBlocked = !Game1.MasterPlayer.hasOrWillReceiveMail("Island_FirstParrot");
      this.boatLight = new LightSource(4, new Vector2(0.0f, 0.0f), 1f);
      this.boatStringLight = new LightSource(4, new Vector2(0.0f, 0.0f), 1f);
      Game1.currentLightSources.Add(this.boatLight);
      Game1.currentLightSources.Add(this.boatStringLight);
      if (!Game1.player.previousLocationName.Contains("Island") || Game1.player.getTileY() > 35)
      {
        if (Game1.isDarkOut() || Game1.isStartingToGetDarkOut() || Game1.IsRainingHere((GameLocation) this))
          Game1.changeMusicTrack("none");
        else if (this._exitsBlocked)
          Game1.changeMusicTrack("tropical_island_day_ambient", true);
        else
          Game1.changeMusicTrack("IslandMusic", true);
      }
      base.resetLocalState();
      this.boatTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\WillysBoat");
      if (Game1.random.NextDouble() < 0.25 || this._isFirstVisit)
        this.addCritter((Critter) new CrabCritter(new Vector2(37f, 30f) * 64f));
      if (this._isFirstVisit)
      {
        this.addCritter((Critter) new CrabCritter(new Vector2(21f, 35f) * 64f));
        this.addCritter((Critter) new CrabCritter(new Vector2(21f, 36f) * 64f));
        this.addCritter((Critter) new CrabCritter(new Vector2(35f, 31f) * 64f));
        if (!Game1.MasterPlayer.hasOrWillReceiveMail("addedParrotBoy"))
        {
          this._parrotBoyHiding = true;
          this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\ParrotBoy", new Microsoft.Xna.Framework.Rectangle(32, 128, 16, 32), new Vector2(29f, 15.5f) * 64f, false, 0.0f, Color.White)
          {
            id = 777f,
            scale = 4f,
            totalNumberOfLoops = 99999,
            interval = 9999f,
            animationLength = 1,
            layerDepth = 1f,
            drawAboveAlwaysFront = true
          });
        }
      }
      if (this._exitsBlocked)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(208, 94, 48, 53), new Vector2(17f, 0.0f) * 64f, false, 0.0f, Color.White)
        {
          id = 555f,
          scale = 4f,
          totalNumberOfLoops = 99999,
          interval = 9999f,
          animationLength = 1,
          layerDepth = 1f / 1000f
        });
      else if (!this._sawFlameSprite)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Monsters\\Magma Sprite", new Microsoft.Xna.Framework.Rectangle(0, 16, 16, 16), new Vector2(18f, 11f) * 64f, false, 0.0f, Color.White)
        {
          id = 999f,
          scale = 4f,
          totalNumberOfLoops = 99999,
          interval = 70f,
          light = true,
          lightRadius = 1f,
          animationLength = 7,
          layerDepth = 1f,
          yPeriodic = true,
          yPeriodicRange = 12f,
          yPeriodicLoopTime = 1000f,
          xPeriodic = true,
          xPeriodicRange = 16f,
          xPeriodicLoopTime = 1800f
        });
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\shadow", new Microsoft.Xna.Framework.Rectangle(0, 0, 12, 7), new Vector2(18.2f, 12.4f) * 64f, false, 0.0f, Color.White)
        {
          id = 998f,
          scale = 4f,
          totalNumberOfLoops = 99999,
          interval = 1000f,
          animationLength = 1,
          layerDepth = 1f / 1000f,
          yPeriodic = true,
          yPeriodicRange = 1f,
          yPeriodicLoopTime = 1000f,
          xPeriodic = true,
          xPeriodicRange = 16f,
          xPeriodicLoopTime = 1800f
        });
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.westernTurtleMoved)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(152, 101, 56, 40), new Vector2(0.5f, 10f) * 64f, false, 0.0f, Color.White)
        {
          id = 789f,
          scale = 4f,
          totalNumberOfLoops = 99999,
          interval = 9999f,
          animationLength = 1,
          layerDepth = 1f / 1000f
        });
      if (Game1.currentSeason == "winter" && !Game1.IsRainingHere((GameLocation) this) && Game1.isDarkOut())
        this.addMoonlightJellies(50, new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame - 24917), new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
      this.ResetBoat();
    }

    public override void digUpArtifactSpot(int xLocation, int yLocation, Farmer who)
    {
      Random random = new Random(xLocation * 2000 + yLocation * 767 + (int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + (int) Game1.stats.DirtHoed);
      if (Game1.player.hasOrWillReceiveMail("islandNorthCaveOpened") && random.NextDouble() < 0.25)
        Game1.createItemDebris((Item) new StardewValley.Object(824, 1), new Vector2((float) xLocation, (float) yLocation) * 64f, -1, (GameLocation) this);
      else
        base.digUpArtifactSpot(xLocation, yLocation, who);
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (tileLocation.X == 14 && tileLocation.Y == 22 && this.getCharacterFromName("Gus") != null && this.getCharacterFromName("Gus").getTileLocation().Equals(new Vector2(14f, 21f)))
      {
        Dictionary<ISalable, int[]> dictionary = new Dictionary<ISalable, int[]>();
        Utility.AddStock(dictionary, (Item) new StardewValley.Object(Vector2.Zero, 873, int.MaxValue), 300);
        Utility.AddStock(dictionary, (Item) new StardewValley.Object(Vector2.Zero, 346, int.MaxValue), 250);
        Utility.AddStock(dictionary, (Item) new StardewValley.Object(Vector2.Zero, 303, int.MaxValue), 500);
        Utility.AddStock(dictionary, (Item) new StardewValley.Object(Vector2.Zero, 459, int.MaxValue), 400);
        Utility.AddStock(dictionary, (Item) new StardewValley.Object(Vector2.Zero, 612, int.MaxValue), 200);
        StardewValley.Object object1 = new StardewValley.Object(Vector2.Zero, 348, int.MaxValue);
        StardewValley.Object object2 = new StardewValley.Object(834, 1);
        object1.Price = object2.Price * 3;
        object1.Name = object2.Name + " Wine";
        object1.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.Wine);
        object1.preservedParentSheetIndex.Value = object2.ParentSheetIndex;
        object1.Quality = 2;
        Utility.AddStock(dictionary, (Item) object1, 2500);
        if (!Game1.player.cookingRecipes.ContainsKey("Tropical Curry"))
          Utility.AddStock(dictionary, (Item) new StardewValley.Object(907, 1, true), 1000);
        Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(dictionary, who: "Gus", context: "ResortBar");
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public static bool CanVisitIslandToday(NPC npc)
    {
      if (!npc.isVillager() || !npc.CanSocialize || npc.daysUntilNotInvisible > 0 || npc.IsInvisible || (npc.Name == "Pam" || npc.Name == "Emily") && Game1.dayOfMonth == 15 && Game1.currentSeason == "fall")
        return false;
      string str = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
      return (npc.currentLocation == null || !(npc.currentLocation.NameOrUniqueName == "Farm")) && (!(str == "Tue") && !(str == "Fri") && !(str == "Wed") || !(npc.Name == "Vincent") && !(npc.Name == "Jas") && !(npc.Name == "Penny")) && (!(str == "Tue") && !(str == "Thu") || !(npc.Name == "Harvey") && !(npc.Name == "Maru")) && !Utility.IsHospitalVisitDay(npc.Name) && (!(npc.Name == "Clint") || !(str != "Fri")) && (!(npc.Name == "Robin") || !(str != "Tue")) && (!(npc.Name == "Marnie") || !(str != "Tue") || !(str != "Mon")) && !(npc.Name == "Sandy") && !(npc.Name == "Dwarf") && !(npc.Name == "Krobus") && !(npc.Name == "Wizard") && !(npc.Name == "Linus") && !(npc.Name == "Willy") && !(npc.Name == "Evelyn") && !(npc.Name == "George");
    }

    public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
    {
      switch (questionAndAnswer)
      {
        case "LeaveIsland_Yes":
          this.Depart();
          return true;
        case "ToggleResort_Yes":
          this.shouldToggleResort.Value = !this.shouldToggleResort.Value;
          bool flag = this.resortOpenToday.Value;
          if (this.shouldToggleResort.Value)
            flag = !flag;
          if (flag)
            Game1.drawDialogueNoTyping(Game1.content.LoadString("Strings\\Locations:IslandSouth_ResortWillOpenSign"));
          else
            Game1.drawDialogueNoTyping(Game1.content.LoadString("Strings\\Locations:IslandSouth_ResortWillCloseSign"));
          return true;
        case null:
          return false;
        default:
          return base.answerDialogueAction(questionAndAnswer, questionParams);
      }
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (!(action == "ResortSign"))
        return base.performAction(action, who, tileLocation);
      string path = !this.resortOpenToday.Value ? (this.shouldToggleResort.Value ? "Strings\\Locations:IslandSouth_ResortClosedWillOpenSign" : "Strings\\Locations:IslandSouth_ResortClosedSign") : (this.shouldToggleResort.Value ? "Strings\\Locations:IslandSouth_ResortOpenWillCloseSign" : "Strings\\Locations:IslandSouth_ResortOpenSign");
      this.createQuestionDialogue(Game1.content.LoadString(path), this.createYesNoResponses(), "ToggleResort");
      return true;
    }

    public override void performTouchAction(string fullActionString, Vector2 playerStandingPosition)
    {
      if (fullActionString.Split(' ')[0] == "LeaveIsland")
      {
        Response[] answerChoices = new Response[2]
        {
          new Response("Yes", Game1.content.LoadString("Strings\\Locations:Desert_Return_Yes")),
          new Response("Not", Game1.content.LoadString("Strings\\Locations:Desert_Return_No"))
        };
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Desert_Return_Question"), answerChoices, "LeaveIsland");
      }
      else
        base.performTouchAction(fullActionString, playerStandingPosition);
    }

    public void Depart() => Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
    {
      this.currentEvent = new StardewValley.Event(Game1.content.LoadString("Data\\Events\\IslandSouth:IslandDepart"), -157039427, Game1.player);
      Game1.eventUp = true;
    }));

    public static Point GetDressingRoomPoint(NPC character) => (int) (NetFieldBase<int, NetInt>) character.gender == 1 ? new Point(22, 19) : new Point(24, 19);

    public override bool HasLocationOverrideDialogue(NPC character) => (!Game1.player.friendshipData.ContainsKey(character.Name) || !Game1.player.friendshipData[character.Name].IsDivorced()) && character.islandScheduleName.Value != null;

    public override string GetLocationOverrideDialogue(NPC character)
    {
      if (Game1.timeOfDay < 1200 || !character.shouldWearIslandAttire.Value && Game1.timeOfDay < 1730 && IslandSouth.HasIslandAttire(character))
      {
        string path = "Characters\\Dialogue\\" + character.Name + ":Resort_Entering";
        if (Game1.content.LoadStringReturnNullIfNotFound(path) != null)
          return path;
      }
      if (Game1.timeOfDay >= 1800)
      {
        string path = "Characters\\Dialogue\\" + character.Name + ":Resort_Leaving";
        if (Game1.content.LoadStringReturnNullIfNotFound(path) != null)
          return path;
      }
      return "Characters\\Dialogue\\" + character.Name + ":Resort";
    }

    public static bool HasIslandAttire(NPC character)
    {
      try
      {
        Game1.temporaryContent.Load<Texture2D>("Characters\\" + NPC.getTextureNameForCharacter(character.name.Value) + "_Beach");
        if (character == null || !(character.Name == "Lewis"))
          return true;
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (allFarmer != null && allFarmer.activeDialogueEvents != null && allFarmer.activeDialogueEvents.ContainsKey("lucky_pants_lewis"))
            return true;
        }
        return false;
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public static void SetupIslandSchedules()
    {
      Game1.netWorldState.Value.IslandVisitors.Clear();
      if (Utility.isFestivalDay(Game1.Date.DayOfMonth, Game1.Date.Season) || Game1.Date.Season == "winter" && Game1.Date.DayOfMonth >= 15 && Game1.Date.DayOfMonth <= 17 || !(Game1.getLocationFromName(nameof (IslandSouth)) is IslandSouth locationFromName) || !locationFromName.resortRestored.Value || Game1.IsRainingHere((GameLocation) locationFromName) || !locationFromName.resortOpenToday.Value)
        return;
      Random random1 = new Random((int) ((double) Game1.uniqueIDForThisGame * 1.21000003814697) + (int) ((double) Game1.stats.DaysPlayed * 2.5));
      List<NPC> list = new List<NPC>();
      foreach (NPC allCharacter in Utility.getAllCharacters())
      {
        if (IslandSouth.CanVisitIslandToday(allCharacter))
          list.Add(allCharacter);
      }
      List<NPC> visitors = new List<NPC>();
      if (random1.NextDouble() < 0.4)
      {
        for (int index = 0; index < 5; ++index)
        {
          NPC random2 = Utility.GetRandom<NPC>(list, random1);
          if (random2 != null && (int) (NetFieldBase<int, NetInt>) random2.age != 2)
          {
            list.Remove(random2);
            visitors.Add(random2);
            random2.scheduleDelaySeconds = Math.Min((float) index * 0.6f, 7f);
          }
        }
      }
      else
      {
        List<List<string>> stringListList = new List<List<string>>();
        stringListList.Add(new List<string>()
        {
          "Sebastian",
          "Sam",
          "Abigail"
        });
        stringListList.Add(new List<string>()
        {
          "Jodi",
          "Kent",
          "Vincent",
          "Sam"
        });
        stringListList.Add(new List<string>()
        {
          "Jodi",
          "Vincent",
          "Sam"
        });
        stringListList.Add(new List<string>()
        {
          "Pierre",
          "Caroline",
          "Abigail"
        });
        stringListList.Add(new List<string>()
        {
          "Robin",
          "Demetrius",
          "Maru",
          "Sebastian"
        });
        stringListList.Add(new List<string>()
        {
          "Lewis",
          "Marnie"
        });
        stringListList.Add(new List<string>()
        {
          "Marnie",
          "Shane",
          "Jas"
        });
        stringListList.Add(new List<string>()
        {
          "Penny",
          "Jas",
          "Vincent"
        });
        stringListList.Add(new List<string>()
        {
          "Pam",
          "Penny"
        });
        stringListList.Add(new List<string>()
        {
          "Caroline",
          "Marnie",
          "Robin",
          "Jodi"
        });
        stringListList.Add(new List<string>()
        {
          "Haley",
          "Penny",
          "Leah",
          "Emily",
          "Maru",
          "Abigail"
        });
        stringListList.Add(new List<string>()
        {
          "Alex",
          "Sam",
          "Sebastian",
          "Elliott",
          "Shane",
          "Harvey"
        });
        List<string> stringList = stringListList[random1.Next(stringListList.Count)];
        bool flag = false;
        foreach (string name in stringList)
        {
          if (!list.Contains(Game1.getCharacterFromName(name)))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          int num = 0;
          foreach (string name in stringList)
          {
            NPC characterFromName = Game1.getCharacterFromName(name);
            list.Remove(characterFromName);
            visitors.Add(characterFromName);
            characterFromName.scheduleDelaySeconds = Math.Min((float) num * 0.6f, 7f);
            ++num;
          }
        }
        for (int index = 0; index < 5 - visitors.Count; ++index)
        {
          NPC random3 = Utility.GetRandom<NPC>(list, random1);
          if (random3 != null && (int) (NetFieldBase<int, NetInt>) random3.age != 2)
          {
            list.Remove(random3);
            visitors.Add(random3);
            random3.scheduleDelaySeconds = Math.Min((float) index * 0.6f, 7f);
          }
        }
      }
      List<IslandSouth.IslandActivityAssigments> activityAssigmentsList = new List<IslandSouth.IslandActivityAssigments>();
      Dictionary<Character, string> last_activity_assignments = new Dictionary<Character, string>();
      activityAssigmentsList.Add(new IslandSouth.IslandActivityAssigments(1200, visitors, random1, last_activity_assignments));
      activityAssigmentsList.Add(new IslandSouth.IslandActivityAssigments(1400, visitors, random1, last_activity_assignments));
      activityAssigmentsList.Add(new IslandSouth.IslandActivityAssigments(1600, visitors, random1, last_activity_assignments));
      foreach (NPC character in visitors)
      {
        StringBuilder stringBuilder = new StringBuilder("");
        bool flag1 = IslandSouth.HasIslandAttire(character);
        bool flag2 = false;
        if (flag1)
        {
          Point dressingRoomPoint = IslandSouth.GetDressingRoomPoint(character);
          stringBuilder.Append("/a1150 IslandSouth " + dressingRoomPoint.X.ToString() + " " + dressingRoomPoint.Y.ToString() + " change_beach");
          flag2 = true;
        }
        foreach (IslandSouth.IslandActivityAssigments activityAssigments in activityAssigmentsList)
        {
          string str = activityAssigments.GetScheduleStringForCharacter(character);
          if (str != "")
          {
            if (!flag2)
            {
              str = "/a" + str.Substring(1);
              flag2 = true;
            }
            stringBuilder.Append(str);
          }
        }
        if (flag1)
        {
          Point dressingRoomPoint = IslandSouth.GetDressingRoomPoint(character);
          stringBuilder.Append("/a1730 IslandSouth " + dressingRoomPoint.X.ToString() + " " + dressingRoomPoint.Y.ToString() + " change_normal");
        }
        if (character.Name == "Gus")
          stringBuilder.Append("/1800 Saloon 10 18 2/2430 bed");
        else
          stringBuilder.Append("/1800 bed");
        stringBuilder.Remove(0, 1);
        character.islandScheduleName.Value = "island";
        character.Schedule = character.parseMasterSchedule(stringBuilder.ToString());
        Game1.netWorldState.Value.IslandVisitors[character.Name] = true;
      }
    }

    public virtual void ResetBoat()
    {
      this.boatPosition = new Vector2(14f, 37f) * 64f;
      this._boatOffset = 0;
      this._boatDirection = 0;
      this._nextBubble = 0.0f;
      this._nextSmoke = 0.0f;
      this._nextSlosh = 0.0f;
    }

    public Vector2 GetBoatPosition() => this.boatPosition + new Vector2(0.0f, (float) this._boatOffset);

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      Vector2 boatPosition = this.GetBoatPosition();
      b.Draw(this.boatTexture, Game1.GlobalToLocal(boatPosition), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 96, 208)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.boatPosition.Y + 320.0) / 10000.0));
      b.Draw(this.boatTexture, Game1.GlobalToLocal(boatPosition), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(288, 0, 96, 208)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.boatPosition.Y + 616.0) / 10000.0));
      if (this.currentEvent != null && this.currentEvent.id == -157039427)
        return;
      b.Draw(this.boatTexture, Game1.GlobalToLocal(new Vector2(1184f, 2752f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 208, 32, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.272f);
    }

    public override bool RunLocationSpecificEventCommand(
      StardewValley.Event current_event,
      string command_string,
      bool first_run,
      params string[] args)
    {
      if (command_string == "boat_reset")
      {
        this.ResetBoat();
        return true;
      }
      if (!(command_string == "boat_depart"))
        return false;
      this._boatDirection = 1;
      return this._boatOffset >= 100;
    }

    public class IslandActivityAssigments
    {
      public int activityTime;
      public List<NPC> visitors;
      public Dictionary<Character, string> currentAssignments;
      public Dictionary<Character, string> currentAnimationAssignments;
      public Random random;
      public Dictionary<string, string> animationDescriptions;
      public List<Point> shoreLoungePoints = new List<Point>((IEnumerable<Point>) new Point[6]
      {
        new Point(9, 33),
        new Point(13, 33),
        new Point(17, 33),
        new Point(24, 33),
        new Point(28, 32),
        new Point(32, 31)
      });
      public List<Point> chairPoints = new List<Point>((IEnumerable<Point>) new Point[2]
      {
        new Point(20, 24),
        new Point(30, 29)
      });
      public List<Point> umbrellaPoints = new List<Point>((IEnumerable<Point>) new Point[3]
      {
        new Point(26, 26),
        new Point(28, 29),
        new Point(10, 27)
      });
      public List<Point> dancePoints = new List<Point>((IEnumerable<Point>) new Point[2]
      {
        new Point(22, 21),
        new Point(23, 21)
      });
      public List<Point> towelLoungePoints = new List<Point>((IEnumerable<Point>) new Point[4]
      {
        new Point(14, 27),
        new Point(17, 28),
        new Point(20, 27),
        new Point(23, 28)
      });
      public List<Point> drinkPoints = new List<Point>((IEnumerable<Point>) new Point[2]
      {
        new Point(12, 23),
        new Point(15, 23)
      });
      public List<Point> wanderPoints = new List<Point>((IEnumerable<Point>) new Point[3]
      {
        new Point(7, 16),
        new Point(31, 24),
        new Point(18, 13)
      });

      public IslandActivityAssigments(
        int time,
        List<NPC> visitors,
        Random seeded_random,
        Dictionary<Character, string> last_activity_assignments)
      {
        this.activityTime = time;
        this.visitors = new List<NPC>((IEnumerable<NPC>) visitors);
        this.random = seeded_random;
        Utility.Shuffle<NPC>(this.random, this.visitors);
        this.animationDescriptions = Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions");
        this.FindActivityForCharacters(last_activity_assignments);
      }

      public virtual void FindActivityForCharacters(
        Dictionary<Character, string> last_activity_assignments)
      {
        this.currentAssignments = new Dictionary<Character, string>();
        this.currentAnimationAssignments = new Dictionary<Character, string>();
        foreach (NPC visitor1 in this.visitors)
        {
          if (!this.currentAssignments.ContainsKey((Character) visitor1))
          {
            if (visitor1.Name == "Gus")
            {
              this.currentAssignments[(Character) visitor1] = "14 21 2";
              foreach (NPC visitor2 in this.visitors)
              {
                if (!this.currentAssignments.ContainsKey((Character) visitor2) && visitor2.Age != 2)
                  this.TryAssignment((Character) visitor2, this.drinkPoints, "Resort_Bar", visitor2.name.Value.ToLower() + "_beach_drink", chance: 0.5, last_activity_assignments: last_activity_assignments);
              }
            }
            if (visitor1.Name == "Sam" && this.TryAssignment((Character) visitor1, this.towelLoungePoints, "Resort_Towel", visitor1.name.Value.ToLower() + "_beach_towel", true, 0.5, last_activity_assignments))
            {
              foreach (NPC visitor3 in this.visitors)
              {
                if (!this.currentAssignments.ContainsKey((Character) visitor3) && this.animationDescriptions.ContainsKey(visitor3.Name.ToLower() + "_beach_dance"))
                {
                  int num1 = int.Parse(this.currentAssignments[(Character) visitor1].Split(' ')[0]);
                  int num2 = int.Parse(this.currentAssignments[(Character) visitor1].Split(' ')[1]);
                  this.currentAssignments.Remove((Character) visitor3);
                  this.TryAssignment((Character) visitor3, new List<Point>((IEnumerable<Point>) new Point[1]
                  {
                    new Point(num1 + 1, num2 + 1)
                  }), "Resort_Dance", visitor3.Name.ToLower() + "_beach_dance", true, last_activity_assignments: last_activity_assignments);
                  visitor3.currentScheduleDelay = 0.0f;
                  visitor1.currentScheduleDelay = 0.0f;
                  break;
                }
              }
            }
          }
        }
        foreach (NPC visitor in this.visitors)
        {
          if (!this.currentAssignments.ContainsKey((Character) visitor) && !this.TryAssignment((Character) visitor, this.towelLoungePoints, "Resort_Towel", visitor.name.Value.ToLower() + "_beach_towel", true, 0.5, last_activity_assignments) && !this.TryAssignment((Character) visitor, this.wanderPoints, "Resort_Wander", "square_3_3", chance: 0.4, last_activity_assignments: last_activity_assignments) && !this.TryAssignment((Character) visitor, this.umbrellaPoints, "Resort_Umbrella", visitor.name.Value.ToLower() + "_beach_umbrella", true, visitor.Name == "Abigail" ? 0.5 : 0.1) && (visitor.Age != 0 || !this.TryAssignment((Character) visitor, this.chairPoints, "Resort_Chair", "_beach_chair", chance: 0.4, last_activity_assignments: last_activity_assignments)))
            this.TryAssignment((Character) visitor, this.shoreLoungePoints, "Resort_Shore", last_activity_assignments: last_activity_assignments);
        }
        last_activity_assignments.Clear();
        foreach (Character key in this.currentAnimationAssignments.Keys)
          last_activity_assignments[key] = this.currentAnimationAssignments[key];
      }

      public bool TryAssignment(
        Character character,
        List<Point> points,
        string dialogue_key,
        string animation_name = null,
        bool animation_required = false,
        double chance = 1.0,
        Dictionary<Character, string> last_activity_assignments = null)
      {
        if (last_activity_assignments != null && animation_name != "" && animation_name != null && !animation_name.StartsWith("square_") && last_activity_assignments.ContainsKey(character) && last_activity_assignments[character] == animation_name || points.Count <= 0 || this.random.NextDouble() >= chance && chance < 1.0)
          return false;
        Point random = Utility.GetRandom<Point>(points, this.random);
        if (animation_name != null && animation_name != "" && !animation_name.StartsWith("square_") && !this.animationDescriptions.ContainsKey(animation_name))
        {
          if (animation_required)
            return false;
          animation_name = (string) null;
        }
        string str;
        if (animation_name == "" || animation_name == null)
          str = random.X.ToString() + " " + random.Y.ToString() + " 2";
        else
          str = random.X.ToString() + " " + random.Y.ToString() + " " + animation_name;
        if (dialogue_key != null)
        {
          dialogue_key = this.GetRandomDialogueKey("Characters\\Dialogue\\" + character.Name + ":" + dialogue_key, this.random);
          if (dialogue_key == null)
            dialogue_key = this.GetRandomDialogueKey("Characters\\Dialogue\\" + character.Name + ":Resort", this.random);
          if (dialogue_key != null)
            str = str + " \"" + dialogue_key + "\"";
        }
        this.currentAssignments[character] = str;
        points.Remove(random);
        this.currentAnimationAssignments[character] = animation_name;
        return true;
      }

      public string GetRandomDialogueKey(string dialogue_key, Random random)
      {
        if (Game1.content.LoadStringReturnNullIfNotFound(dialogue_key) == null)
          return (string) null;
        bool flag = false;
        int maxValue = 0;
        while (!flag)
        {
          ++maxValue;
          if (Game1.content.LoadStringReturnNullIfNotFound(dialogue_key + "_" + (maxValue + 1).ToString()) == null)
            flag = true;
        }
        int num = random.Next(maxValue) + 1;
        return num == 1 ? dialogue_key : dialogue_key + "_" + num.ToString();
      }

      public string GetScheduleStringForCharacter(NPC character) => this.currentAssignments.ContainsKey((Character) character) ? "/" + this.activityTime.ToString() + " IslandSouth " + this.currentAssignments[(Character) character] : "";
    }
  }
}
