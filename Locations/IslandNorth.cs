// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandNorth
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class IslandNorth : IslandLocation
  {
    [XmlElement("bridgeFixed")]
    public readonly NetBool bridgeFixed = new NetBool();
    [XmlElement("traderActivated")]
    public readonly NetBool traderActivated = new NetBool();
    [XmlElement("boulderRemoved")]
    public readonly NetBool boulderRemoved = new NetBool();
    [XmlElement("caveOpened")]
    public readonly NetBool caveOpened = new NetBool();
    [XmlElement("treeNutShot")]
    public readonly NetBool treeNutShot = new NetBool();
    [XmlIgnore]
    public List<SuspensionBridge> suspensionBridges = new List<SuspensionBridge>();
    [XmlIgnore]
    protected bool _sawFlameSpriteSouth;
    [XmlIgnore]
    protected bool _sawFlameSpriteNorth;
    [XmlIgnore]
    protected bool hasTriedFirstEntryDigSiteLoad;
    private float boulderKnockTimer;
    private float boulderTextTimer;
    private string boulderTextString;
    private int boulderKnocksLeft;
    private Microsoft.Xna.Framework.Rectangle boulderPosition = new Microsoft.Xna.Framework.Rectangle(1344, 3008, 128, 64);
    private float doneHittingBoulderWithToolTimer;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddField((INetSerializable) this.bridgeFixed);
      this.bridgeFixed.InterpolationWait = false;
      this.bridgeFixed.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.ApplyFixedBridge();
      });
      this.NetFields.AddField((INetSerializable) this.traderActivated);
      this.traderActivated.InterpolationWait = false;
      this.traderActivated.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (Utility.ShouldIgnoreValueChangeCallback())
          return;
        this.ApplyIslandTraderHut();
      });
      this.NetFields.AddField((INetSerializable) this.caveOpened);
      this.caveOpened.InterpolationWait = false;
      this.caveOpened.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (Utility.ShouldIgnoreValueChangeCallback())
          return;
        this.ApplyCaveOpened();
      });
      this.NetFields.AddField((INetSerializable) this.treeNutShot);
      this.treeNutShot.InterpolationWait = false;
    }

    public override void SetBuriedNutLocations()
    {
      this.buriedNutPoints.Add(new Point(57, 79));
      this.buriedNutPoints.Add(new Point(19, 39));
      this.buriedNutPoints.Add(new Point(19, 13));
      this.buriedNutPoints.Add(new Point(54, 21));
      this.buriedNutPoints.Add(new Point(42, 77));
      this.buriedNutPoints.Add(new Point(62, 54));
      this.buriedNutPoints.Add(new Point(26, 81));
      base.SetBuriedNutLocations();
    }

    public virtual void ApplyFixedBridge()
    {
      if (this.map == null)
        return;
      this.ApplyMapOverride("Island_Bridge_Repaired", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(31, 52, 4, 3)));
    }

    public virtual void ApplyBoulderRemove()
    {
      if (this.map == null)
        return;
      this.ApplyMapOverride("Island_Boulder_Removed", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(38, 19, 6, 5)));
    }

    public virtual void ApplyIslandTraderHut()
    {
      if (this.map == null)
        return;
      this.ApplyMapOverride("Island_N_Trader", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 64, 9, 10)));
      this.removeTemporarySpritesWithIDLocal(8989f);
      this.removeTemporarySpritesWithIDLocal(8988f);
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(33.45f, 70.33f) * 64f + new Vector2(-16f, -32f), false, 0.0f, Color.White)
      {
        delayBeforeAnimationStart = 10,
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        lightID = 8989,
        id = 8989f,
        lightRadius = 2f,
        scale = 4f,
        layerDepth = 0.46144f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(39.45f, 70.33f) * 64f + new Vector2(-16f, -32f), false, 0.0f, Color.White)
      {
        delayBeforeAnimationStart = 10,
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        light = true,
        lightID = 8988,
        id = 8988f,
        lightRadius = 2f,
        scale = 4f,
        layerDepth = 0.46144f
      });
    }

    public virtual void ApplyCaveOpened()
    {
      if (Game1.player.currentLocation == null || !Game1.player.currentLocation.Equals((GameLocation) this))
        return;
      for (int index = 0; index < 12; ++index)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(146, 229 + Game1.random.Next(3) * 9, 9, 9), Utility.getRandomPositionInThisRectangle(this.boulderPosition, Game1.random), Game1.random.NextDouble() < 0.5, 0.0f, Color.White)
        {
          scale = 4f,
          motion = new Vector2((float) Game1.random.Next(-3, 1), (float) Game1.random.Next(-15, -9)),
          acceleration = new Vector2(0.0f, 0.4f),
          rotationChange = (float) Game1.random.Next(-2, 3) * 0.01f,
          drawAboveAlwaysFront = true,
          yStopCoordinate = this.boulderPosition.Bottom + 1 + Game1.random.Next(64),
          delayBeforeAnimationStart = index * 15
        });
        this.temporarySprites[this.temporarySprites.Count - 1].initialPosition.Y = (float) this.temporarySprites[this.temporarySprites.Count - 1].yStopCoordinate;
        this.temporarySprites[this.temporarySprites.Count - 1].reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.temporarySprites[this.temporarySprites.Count - 1].bounce);
      }
      for (int index = 0; index < 8; ++index)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), Utility.getRandomPositionInThisRectangle(this.boulderPosition, Game1.random) + new Vector2(-32f, -32f), false, 0.007f, Color.White)
        {
          alpha = 0.75f,
          motion = new Vector2(0.0f, -1f),
          acceleration = new Vector2(1f / 500f, 0.0f),
          interval = 99999f,
          layerDepth = 1f,
          scale = 4f,
          scaleChange = 0.02f,
          rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
          delayBeforeAnimationStart = index * 40
        });
      Game1.playSound("boulderBreak");
      Game1.player.freezePause = 3000;
      DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandNorth_Event_SafariManAppear")))))), 1000);
    }

    public override string checkForBuriedItem(
      int xLocation,
      int yLocation,
      bool explosion,
      bool detectOnly,
      Farmer who)
    {
      if (xLocation == 27 && yLocation == 28 && who.secretNotesSeen.Contains(1010))
      {
        Game1.player.team.RequestLimitedNutDrops("Island_N_BuriedTreasureNut", (GameLocation) this, xLocation * 64, yLocation * 64, 1);
        if (!Game1.player.hasOrWillReceiveMail("Island_N_BuriedTreasure"))
        {
          Game1.createItemDebris((Item) new StardewValley.Object(289, 1), new Vector2((float) xLocation, (float) yLocation) * 64f, 1);
          Game1.addMailForTomorrow("Island_N_BuriedTreasure", true);
        }
      }
      if (xLocation == 26 && yLocation == 81 && !Game1.player.team.collectedNutTracker.ContainsKey("Buried_IslandNorth_26_81"))
        DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
        {
          TemporaryAnimatedSprite t = this.getTemporarySpriteByID(79797);
          if (t == null)
            return;
          t.sourceRectStartingPos.X += 40f;
          t.sourceRect.X = 181;
          t.interval = 100f;
          t.shakeIntensity = 1f;
          this.playSound("monkey1");
          t.motion = new Vector2(-3f, -10f);
          t.acceleration = new Vector2(0.0f, 0.3f);
          t.yStopCoordinate = (int) t.position.Y + 1;
          t.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x =>
          {
            this.temporarySprites.Add(new TemporaryAnimatedSprite(50, t.position, Color.Green)
            {
              drawAboveAlwaysFront = true
            });
            this.removeTemporarySpritesWithID(79797);
            this.playSound("leafrustle");
          });
        }), 700);
      return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
    }

    public IslandNorth()
    {
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
      if (!projectile || damagesFarmer != 0 || position.Bottom >= 832)
        return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement);
      if (!position.Intersects(new Microsoft.Xna.Framework.Rectangle(3648, 576, 256, 64)))
        return false;
      if (Game1.IsMasterGame && !this.treeNutShot.Value)
      {
        Game1.player.team.MarkCollectedNut("TreeNutShot");
        this.treeNutShot.Value = true;
        Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(58.5f, 11f) * 64f, 0, (GameLocation) this, 0);
      }
      return true;
    }

    public IslandNorth(string map, string name)
      : base(map, name)
    {
      this.parrotUpgradePerches.Clear();
      this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(35, 52), new Microsoft.Xna.Framework.Rectangle(31, 52, 4, 4), 10, (Action) (() =>
      {
        Game1.addMailForTomorrow("Island_UpgradeBridge", true, true);
        this.bridgeFixed.Value = true;
      }), (Func<bool>) (() => this.bridgeFixed.Value), "Bridge", "Island_Turtle"));
      this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(32, 72), new Microsoft.Xna.Framework.Rectangle(33, 68, 5, 5), 10, (Action) (() =>
      {
        Game1.addMailForTomorrow("Island_UpgradeTrader", true, true);
        this.traderActivated.Value = true;
      }), (Func<bool>) (() => this.traderActivated.Value), "Trader", "Island_UpgradeHouse"));
      this.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(45f, 38f), 4, (GameLocation) this));
      this.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(47f, 40f), 4, (GameLocation) this));
      this.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(13f, 33f), 4, (GameLocation) this));
      this.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(5f, 30f), 4, (GameLocation) this));
    }

    protected override void resetSharedState() => base.resetSharedState();

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      if (l is IslandNorth)
      {
        IslandNorth islandNorth = l as IslandNorth;
        this.bridgeFixed.Value = (bool) (NetFieldBase<bool, NetBool>) islandNorth.bridgeFixed;
        this.boulderRemoved.Value = (bool) (NetFieldBase<bool, NetBool>) islandNorth.boulderRemoved;
        this.treeNutShot.Value = islandNorth.treeNutShot.Value;
        this.caveOpened.Value = islandNorth.caveOpened.Value;
        this.traderActivated.Value = islandNorth.traderActivated.Value;
      }
      base.TransferDataFromSavedLocation(l);
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      switch (this.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings"))
      {
        case 2074:
        case 2075:
        case 2076:
        case 2077:
        case 2078:
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(IslandNorth.getIslandMerchantTradeStock(Game1.player), who: "IslandTrade");
          return true;
        default:
          return base.checkAction(tileLocation, viewport, who);
      }
    }

    public static Dictionary<ISalable, int[]> getIslandMerchantTradeStock(
      Farmer who)
    {
      Dictionary<ISalable, int[]> merchantTradeStock = new Dictionary<ISalable, int[]>();
      Item key1 = (Item) new StardewValley.Object(688, 1);
      merchantTradeStock.Add((ISalable) key1, new int[4]
      {
        0,
        int.MaxValue,
        830,
        5
      });
      Item key2 = (Item) new StardewValley.Object(831, 1);
      merchantTradeStock.Add((ISalable) key2, new int[4]
      {
        0,
        int.MaxValue,
        881,
        2
      });
      Item key3 = (Item) new StardewValley.Object(833, 1);
      merchantTradeStock.Add((ISalable) key3, new int[4]
      {
        0,
        int.MaxValue,
        851,
        1
      });
      if (Game1.netWorldState.Value.GoldenCoconutCracked.Value)
      {
        Item key4 = (Item) new StardewValley.Object(791, 1);
        merchantTradeStock.Add((ISalable) key4, new int[4]
        {
          0,
          int.MaxValue,
          88,
          10
        });
      }
      Item key5 = (Item) new TV(2326, Vector2.Zero);
      merchantTradeStock.Add((ISalable) key5, new int[4]
      {
        0,
        int.MaxValue,
        830,
        30
      });
      Item key6 = (Item) new Furniture(2331, Vector2.Zero);
      merchantTradeStock.Add((ISalable) key6, new int[4]
      {
        0,
        int.MaxValue,
        848,
        5
      });
      if (Game1.dayOfMonth % 2 == 0)
      {
        Item key7 = (Item) new Furniture(134, Vector2.Zero);
        merchantTradeStock.Add((ISalable) key7, new int[4]
        {
          0,
          int.MaxValue,
          837,
          1
        });
      }
      Item key8 = (Item) new StardewValley.Object(69, 1);
      merchantTradeStock.Add((ISalable) key8, new int[4]
      {
        0,
        int.MaxValue,
        852,
        5
      });
      Item key9 = (Item) new StardewValley.Object(835, 1);
      merchantTradeStock.Add((ISalable) key9, new int[4]
      {
        0,
        int.MaxValue,
        719,
        75
      });
      if (Game1.dayOfMonth % 7 == 1)
      {
        Item key10 = (Item) new Hat(79);
        merchantTradeStock.Add((ISalable) key10, new int[4]
        {
          0,
          int.MaxValue,
          830,
          30
        });
      }
      if (Game1.dayOfMonth % 7 == 3)
      {
        Item key11 = (Item) new Hat(80);
        merchantTradeStock.Add((ISalable) key11, new int[4]
        {
          0,
          int.MaxValue,
          830,
          30
        });
      }
      if (Game1.dayOfMonth % 7 == 5)
      {
        Item key12 = (Item) new Hat(81);
        merchantTradeStock.Add((ISalable) key12, new int[4]
        {
          0,
          int.MaxValue,
          830,
          30
        });
      }
      Item key13 = (Item) new BedFurniture(2496, Vector2.Zero);
      merchantTradeStock.Add((ISalable) key13, new int[4]
      {
        0,
        int.MaxValue,
        848,
        100
      });
      Item key14 = (Item) new BedFurniture(2176, Vector2.Zero);
      merchantTradeStock.Add((ISalable) key14, new int[4]
      {
        0,
        int.MaxValue,
        829,
        20
      });
      if (Game1.dayOfMonth % 7 == 0)
      {
        Item key15 = (Item) new BedFurniture(2180, Vector2.Zero);
        merchantTradeStock.Add((ISalable) key15, new int[4]
        {
          0,
          int.MaxValue,
          91,
          5
        });
      }
      if (Game1.dayOfMonth % 7 == 2)
      {
        Item key16 = (Item) new Furniture(2393, Vector2.Zero);
        merchantTradeStock.Add((ISalable) key16, new int[4]
        {
          0,
          int.MaxValue,
          832,
          1
        });
      }
      if (Game1.dayOfMonth % 7 == 4)
      {
        Item key17 = (Item) new Furniture(2329, Vector2.Zero);
        merchantTradeStock.Add((ISalable) key17, new int[4]
        {
          0,
          int.MaxValue,
          834,
          5
        });
      }
      if (Game1.dayOfMonth % 7 == 6)
      {
        Item key18 = (Item) new Furniture(1228, Vector2.Zero);
        merchantTradeStock.Add((ISalable) key18, new int[4]
        {
          0,
          int.MaxValue,
          838,
          3
        });
      }
      Item key19 = (Item) new StardewValley.Object(292, 1);
      merchantTradeStock.Add((ISalable) key19, new int[4]
      {
        0,
        int.MaxValue,
        836,
        1
      });
      Item key20 = (Item) new Clothing(7);
      merchantTradeStock.Add((ISalable) key20, new int[4]
      {
        0,
        int.MaxValue,
        830,
        50
      });
      if (!Game1.player.cookingRecipes.ContainsKey("Banana Pudding"))
      {
        Item key21 = (Item) new StardewValley.Object(904, 1, true);
        merchantTradeStock.Add((ISalable) key21, new int[4]
        {
          0,
          1,
          881,
          30
        });
      }
      if (!Game1.player.cookingRecipes.ContainsKey("Deluxe Retaining Soil"))
      {
        Item key22 = (Item) new StardewValley.Object(920, 1, true);
        merchantTradeStock.Add((ISalable) key22, new int[4]
        {
          0,
          1,
          848,
          50
        });
      }
      if (Game1.dayOfMonth == 28 && Game1.stats.getStat("hardModeMonstersKilled") > 50U)
      {
        Item key23 = (Item) new StardewValley.Object(896, 1);
        merchantTradeStock.Add((ISalable) key23, new int[4]
        {
          0,
          int.MaxValue,
          910,
          10
        });
      }
      return merchantTradeStock;
    }

    public override List<Vector2> GetAdditionalWalnutBushes() => new List<Vector2>()
    {
      new Vector2(56f, 27f)
    };

    public override void digUpArtifactSpot(int xLocation, int yLocation, Farmer who)
    {
      Random random = new Random(xLocation * 2000 + yLocation * 777 + (int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + (int) Game1.stats.DirtHoed);
      if (this.bridgeFixed.Value)
      {
        if (random.NextDouble() < 0.1)
        {
          Game1.createItemDebris((Item) new StardewValley.Object(825, 1), new Vector2((float) xLocation, (float) yLocation) * 64f, -1, (GameLocation) this);
          return;
        }
        if (random.NextDouble() < 0.25)
          Game1.createMultipleObjectDebris(881, xLocation, yLocation, random.Next(1, 3) + (random.NextDouble() < (double) who.LuckLevel / 100.0 ? 1 : 0) + (random.NextDouble() < (double) who.MiningLevel / 100.0 ? 1 : 0), (long) who.uniqueMultiplayerID, (GameLocation) this);
      }
      base.digUpArtifactSpot(xLocation, yLocation, who);
    }

    public override bool catchOceanCrabPotFishFromThisSpot(int x, int y) => false;

    public override StardewValley.Object getFish(
      float millisecondsAfterNibble,
      int bait,
      int waterDepth,
      Farmer who,
      double baitPotency,
      Vector2 bobberTile,
      string locationName = null)
    {
      Random random = new Random((int) bobberTile.X * 2000 + (int) bobberTile.Y * 777 + (int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + (int) Game1.stats.TimesFished);
      if ((bool) (NetFieldBase<bool, NetBool>) (Game1.getLocationFromName(nameof (IslandNorth)) as IslandNorth).bridgeFixed && random.NextDouble() < 0.1)
        return new StardewValley.Object(821, 1);
      if (who != null && who.getTileY() >= 72)
      {
        if (!who.mailReceived.Contains("gotSecretIslandNPainting"))
        {
          who.mailReceived.Add("gotSecretIslandNPainting");
          return (StardewValley.Object) new Furniture(2419, Vector2.Zero);
        }
        if (random.NextDouble() < 0.1)
          return (StardewValley.Object) new Furniture(2419, Vector2.Zero);
      }
      if (who != null && (double) bobberTile.Y < 35.0 && (double) bobberTile.X < 4.0)
      {
        if (!who.mailReceived.Contains("gotSecretIslandNSquirrel"))
        {
          who.mailReceived.Add("gotSecretIslandNSquirrel");
          return (StardewValley.Object) new Furniture(2814, Vector2.Zero);
        }
        if (random.NextDouble() < 0.1)
          return (StardewValley.Object) new Furniture(2814, Vector2.Zero);
      }
      return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      foreach (SuspensionBridge suspensionBridge in this.suspensionBridges)
        suspensionBridge.Update(time);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.caveOpened && Utility.isOnScreen(Utility.PointToVector2(this.boulderPosition.Location), 1))
      {
        double boulderKnockTimer = (double) this.boulderKnockTimer;
        TimeSpan elapsedGameTime = time.ElapsedGameTime;
        double totalMilliseconds1 = elapsedGameTime.TotalMilliseconds;
        this.boulderKnockTimer = (float) (boulderKnockTimer - totalMilliseconds1);
        double boulderTextTimer = (double) this.boulderTextTimer;
        elapsedGameTime = time.ElapsedGameTime;
        double totalMilliseconds2 = elapsedGameTime.TotalMilliseconds;
        this.boulderTextTimer = (float) (boulderTextTimer - totalMilliseconds2);
        if ((double) this.doneHittingBoulderWithToolTimer > 0.0)
        {
          double boulderWithToolTimer = (double) this.doneHittingBoulderWithToolTimer;
          elapsedGameTime = time.ElapsedGameTime;
          double totalMilliseconds3 = elapsedGameTime.TotalMilliseconds;
          this.doneHittingBoulderWithToolTimer = (float) (boulderWithToolTimer - totalMilliseconds3);
          if ((double) this.doneHittingBoulderWithToolTimer <= 0.0)
          {
            this.boulderTextTimer = 2000f;
            this.boulderTextString = Game1.content.LoadString("Strings\\Locations:IslandNorth_CaveTool_" + Game1.random.Next(4).ToString());
          }
        }
        if (this.boulderKnocksLeft > 0)
        {
          if ((double) this.boulderKnockTimer < 0.0)
          {
            Game1.playSound("hammer");
            --this.boulderKnocksLeft;
            this.boulderKnockTimer = 500f;
            if (this.boulderKnocksLeft == 0 && Game1.random.NextDouble() < 0.5)
              DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
              {
                this.boulderTextTimer = 2000f;
                this.boulderTextString = Game1.content.LoadString("Strings\\Locations:IslandNorth_CaveHelp_" + Game1.random.Next(4).ToString());
              }), 1000);
          }
        }
        else if (Game1.random.NextDouble() < 0.002 && (double) this.boulderTextTimer < -500.0)
          this.boulderKnocksLeft = Game1.random.Next(3, 6);
      }
      if (!this._sawFlameSpriteSouth && Utility.isThereAFarmerWithinDistance(new Vector2(36f, 79f), 5, (GameLocation) this) == Game1.player)
      {
        Game1.addMailForTomorrow("Saw_Flame_Sprite_North_South", true);
        TemporaryAnimatedSprite temporarySpriteById1 = this.getTemporarySpriteByID(999);
        if (temporarySpriteById1 != null)
        {
          temporarySpriteById1.yPeriodic = false;
          temporarySpriteById1.xPeriodic = false;
          temporarySpriteById1.sourceRect.Y = 0;
          temporarySpriteById1.sourceRectStartingPos.Y = 0.0f;
          temporarySpriteById1.motion = new Vector2(1f, -4f);
          temporarySpriteById1.acceleration = new Vector2(0.0f, -0.04f);
          temporarySpriteById1.drawAboveAlwaysFront = true;
        }
        this.localSound("magma_sprite_spot");
        TemporaryAnimatedSprite temporarySpriteById2 = this.getTemporarySpriteByID(998);
        if (temporarySpriteById2 != null)
        {
          temporarySpriteById2.yPeriodic = false;
          temporarySpriteById2.xPeriodic = false;
          temporarySpriteById2.motion = new Vector2(1f, -4f);
          temporarySpriteById2.acceleration = new Vector2(0.0f, -0.04f);
        }
        this._sawFlameSpriteSouth = true;
      }
      if (!this._sawFlameSpriteNorth && Utility.isThereAFarmerWithinDistance(new Vector2(41f, 30f), 5, (GameLocation) this) == Game1.player)
      {
        Game1.addMailForTomorrow("Saw_Flame_Sprite_North_North", true);
        TemporaryAnimatedSprite temporarySpriteById3 = this.getTemporarySpriteByID(9999);
        if (temporarySpriteById3 != null)
        {
          temporarySpriteById3.yPeriodic = false;
          temporarySpriteById3.xPeriodic = false;
          temporarySpriteById3.sourceRect.Y = 0;
          temporarySpriteById3.sourceRectStartingPos.Y = 0.0f;
          temporarySpriteById3.motion = new Vector2(0.0f, -4f);
          temporarySpriteById3.acceleration = new Vector2(0.0f, -0.04f);
          temporarySpriteById3.yStopCoordinate = 1216;
          temporarySpriteById3.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x => this.removeTemporarySpritesWithID(9999));
        }
        this.localSound("magma_sprite_spot");
        TemporaryAnimatedSprite temporarySpriteById4 = this.getTemporarySpriteByID(9998);
        if (temporarySpriteById4 != null)
        {
          temporarySpriteById4.yPeriodic = false;
          temporarySpriteById4.xPeriodic = false;
          temporarySpriteById4.motion = new Vector2(0.0f, -4f);
          temporarySpriteById4.acceleration = new Vector2(0.0f, -0.04f);
          temporarySpriteById4.yStopCoordinate = 1280;
          temporarySpriteById4.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x => this.removeTemporarySpritesWithID(9998));
        }
        this._sawFlameSpriteNorth = true;
      }
      if (this.hasTriedFirstEntryDigSiteLoad)
        return;
      if (Game1.IsMasterGame && !Game1.player.hasOrWillReceiveMail("ISLAND_NORTH_DIGSITE_LOAD"))
      {
        Game1.addMail("ISLAND_NORTH_DIGSITE_LOAD", true);
        for (int index = 0; index < 40; ++index)
          this.digSiteUpdate();
      }
      this.hasTriedFirstEntryDigSiteLoad = true;
    }

    public override bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character)
    {
      return !(bool) (NetFieldBase<bool, NetBool>) this.caveOpened && this.boulderPosition.Intersects(position) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
    }

    public override bool isTilePlaceable(Vector2 tile_location, Item item = null)
    {
      Point point = Utility.Vector2ToPoint((tile_location + new Vector2(0.5f, 0.5f)) * 64f);
      return ((bool) (NetFieldBase<bool, NetBool>) this.caveOpened || !this.boulderPosition.Contains(point)) && base.isTilePlaceable(tile_location, item);
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      this.digSiteUpdate();
      List<Vector2> vector2List = new List<Vector2>();
      foreach (Vector2 key in this.terrainFeatures.Keys)
      {
        if (this.terrainFeatures[key] is HoeDirt && (this.terrainFeatures[key] as HoeDirt).crop != null && (bool) (NetFieldBase<bool, NetBool>) (this.terrainFeatures[key] as HoeDirt).crop.forageCrop)
          vector2List.Add(key);
      }
      foreach (Vector2 key in vector2List)
        this.terrainFeatures.Remove(key);
      List<Microsoft.Xna.Framework.Rectangle> rectangleList = new List<Microsoft.Xna.Framework.Rectangle>();
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(10, 51, 1, 8));
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(15, 59, 1, 4));
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(18, 34, 1, 1));
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(40, 48, 6, 6));
      for (int index = 0; index < 1; ++index)
      {
        Microsoft.Xna.Framework.Rectangle rectangle = rectangleList[Game1.random.Next(rectangleList.Count)];
        Vector2 tileLocation = new Vector2((float) Game1.random.Next(rectangle.X, rectangle.Right), (float) Game1.random.Next(rectangle.Y, rectangle.Bottom));
        foreach (Vector2 openTile in Utility.recursiveFindOpenTiles((GameLocation) this, tileLocation, 16))
        {
          string str = this.doesTileHaveProperty((int) openTile.X, (int) openTile.Y, "Diggable", "Back");
          if (!this.terrainFeatures.ContainsKey(openTile) && str != null && Game1.random.NextDouble() < 1.0 - (double) Vector2.Distance(tileLocation, openTile) * 0.349999994039536)
          {
            HoeDirt hoeDirt = new HoeDirt(0, new Crop(true, 2, (int) openTile.X, (int) openTile.Y));
            hoeDirt.state.Value = 2;
            this.terrainFeatures.Add(openTile, (TerrainFeature) hoeDirt);
          }
        }
      }
    }

    private bool isTileOpenForDigSiteStone(int tileX, int tileY) => this.doesTileHaveProperty(tileX, tileY, "Diggable", "Back") != null && this.doesTileHaveProperty(tileX, tileY, "Diggable", "Back") == "T" && this.isTileLocationTotallyClearAndPlaceable(new Vector2((float) tileX, (float) tileY));

    public void digSiteUpdate()
    {
      bool flag = false;
      Random random = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + 78);
      Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(4, 47, 22, 20);
      int num1 = 20;
      Vector2[] vector2Array = new Vector2[8]
      {
        new Vector2(18f, 49f),
        new Vector2(15f, 54f),
        new Vector2(21f, 52f),
        new Vector2(18f, 61f),
        new Vector2(23f, 57f),
        new Vector2(9f, 63f),
        new Vector2(7f, 51f),
        new Vector2(7f, 57f)
      };
      if (Utility.getNumObjectsOfIndexWithinRectangle(r, new int[9]
      {
        816,
        817,
        818,
        819,
        32,
        38,
        40,
        42,
        590
      }, (GameLocation) this) < 60)
      {
        for (int index1 = 0; index1 < num1; ++index1)
        {
          Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, Game1.random);
          Vector2 tileLocation = vector2Array[random.Next(vector2Array.Length)];
          if (this.isTileOpenForDigSiteStone((int) positionInThisRectangle.X, (int) positionInThisRectangle.Y))
          {
            if (!flag || Game1.random.NextDouble() < 0.3)
            {
              flag = true;
              this.objects.Add(positionInThisRectangle, new StardewValley.Object(positionInThisRectangle, 816 + Game1.random.Next(2), 1)
              {
                MinutesUntilReady = 4
              });
            }
            else if (Game1.random.NextDouble() < 0.1)
            {
              int x = (int) positionInThisRectangle.X;
              int y = (int) positionInThisRectangle.Y;
              if (this.isTileLocationTotallyClearAndPlaceable(positionInThisRectangle) && this.getTileIndexAt(x, y, "AlwaysFront") == -1 && this.getTileIndexAt(x, y, "Front") == -1 && !this.isBehindBush(positionInThisRectangle) && this.doesTileHaveProperty(x, y, "Diggable", "Back") != null && this.doesTileHaveProperty(x, y, "Diggable", "Back") == "T")
                this.objects.Add(positionInThisRectangle, new StardewValley.Object(positionInThisRectangle, 590, 1));
            }
            else if (Game1.random.NextDouble() < 0.06)
              this.terrainFeatures.Add(positionInThisRectangle, (TerrainFeature) new Tree(8, 1));
            else if (Game1.random.NextDouble() < 0.2)
            {
              if (this.isTileOpenForDigSiteStone((int) tileLocation.X, (int) tileLocation.Y))
              {
                int num2 = Game1.random.Next(2, 5);
                for (int index2 = 0; index2 < num2; ++index2)
                  Utility.spawnObjectAround(tileLocation, new StardewValley.Object(tileLocation, 818, 1)
                  {
                    MinutesUntilReady = 4
                  }, (GameLocation) this, false, (Action<StardewValley.Object>) (o =>
                  {
                    o.CanBeGrabbed = false;
                    o.IsSpawnedObject = false;
                  }));
              }
            }
            else if (Game1.random.NextDouble() < 0.25)
            {
              this.objects.Add(positionInThisRectangle, new StardewValley.Object(positionInThisRectangle, random.NextDouble() < 0.33 ? 785 : (random.NextDouble() < 0.5 ? 676 : 677), 1));
            }
            else
            {
              StardewValley.Object @object = new StardewValley.Object(positionInThisRectangle, Game1.random.NextDouble() < 0.25 ? 32 : (Game1.random.NextDouble() < 0.33 ? 38 : (Game1.random.NextDouble() < 0.5 ? 40 : 42)), 1);
              @object.minutesUntilReady.Value = 2;
              @object.Name = "Stone";
              this.objects.Add(positionInThisRectangle, @object);
            }
          }
        }
      }
      else
      {
        if (Utility.getNumObjectsOfIndexWithinRectangle(r, new int[3]
        {
          785,
          676,
          677
        }, (GameLocation) this) >= 100)
          return;
        int num3 = random.Next(4);
        for (int index = 0; index < num3; ++index)
        {
          Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, Game1.random);
          if (this.isTileOpenForDigSiteStone((int) positionInThisRectangle.X, (int) positionInThisRectangle.Y))
            this.objects.Add(positionInThisRectangle, new StardewValley.Object(positionInThisRectangle, random.NextDouble() < 0.33 ? 785 : (random.NextDouble() < 0.5 ? 676 : 677), 1));
        }
      }
    }

    public override void performOrePanTenMinuteUpdate(Random r)
    {
      Point point;
      if (Game1.MasterPlayer.mailReceived.Contains("ccFishTank"))
      {
        point = this.orePanPoint.Value;
        if (point.Equals(Point.Zero) && r.NextDouble() < 0.5)
        {
          for (int index = 0; index < 6; ++index)
          {
            Point p = new Point(r.Next(4, 15), r.Next(45, 70));
            if (this.isOpenWater(p.X, p.Y) && FishingRod.distanceToLand(p.X, p.Y, (GameLocation) this) <= 1 && this.getTileIndexAt(p, "Buildings") == -1)
            {
              if (Game1.player.currentLocation.Equals((GameLocation) this))
                this.playSound("slosh");
              this.orePanPoint.Value = p;
              break;
            }
          }
          return;
        }
      }
      point = this.orePanPoint.Value;
      if (point.Equals(Point.Zero) || r.NextDouble() >= 0.1)
        return;
      this.orePanPoint.Value = Point.Zero;
    }

    public override bool performToolAction(Tool t, int tileX, int tileY)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.caveOpened && tileY == 47 && (tileX == 21 || tileX == 22))
      {
        this.boulderKnockTimer = 500f;
        Game1.playSound("hammer");
        this.boulderKnocksLeft = 0;
        this.doneHittingBoulderWithToolTimer = 1200f;
      }
      return base.performToolAction(t, tileX, tileY);
    }

    public override void explosionAt(float x, float y)
    {
      base.explosionAt(x, y);
      if (this.caveOpened.Value || (double) y != 47.0 || (double) x != 21.0 && (double) x != 22.0)
        return;
      this.caveOpened.Value = true;
      Game1.addMailForTomorrow("islandNorthCaveOpened", true, true);
    }

    public override void drawBackground(SpriteBatch b)
    {
      base.drawBackground(b);
      this.DrawParallaxHorizon(b);
      if (this.treeNutShot.Value)
        return;
      b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(58.25f, 10f) * 64f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 73, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      foreach (SuspensionBridge suspensionBridge in this.suspensionBridges)
        suspensionBridge.Draw(b);
      if ((bool) (NetFieldBase<bool, NetBool>) this.caveOpened)
        return;
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Utility.PointToVector2(this.boulderPosition.Location) + new Vector2((double) this.boulderKnockTimer > 250.0 ? (float) Game1.random.Next(-1, 2) : 0.0f, (float) (((double) this.boulderKnockTimer > 250.0 ? Game1.random.Next(-1, 2) : 0) - 64))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(155, 224, 32, 32)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) this.boulderPosition.Y / 10000f);
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      base.drawAboveAlwaysFrontLayer(b);
      if ((bool) (NetFieldBase<bool, NetBool>) this.caveOpened || (double) this.boulderTextTimer <= 0.0)
        return;
      SpriteText.drawStringWithScrollCenteredAt(b, this.boulderTextString, (int) Game1.GlobalToLocal(Utility.PointToVector2(this.boulderPosition.Location)).X + 64, (int) Game1.GlobalToLocal(Utility.PointToVector2(this.boulderPosition.Location)).Y - 128 - 32, "", 1f, -1, 1, 1f, false);
    }

    public override bool isTileOccupiedForPlacement(Vector2 tileLocation, StardewValley.Object toPlace = null)
    {
      foreach (SuspensionBridge suspensionBridge in this.suspensionBridges)
      {
        if (suspensionBridge.CheckPlacementPrevention(tileLocation))
          return true;
      }
      return base.isTileOccupiedForPlacement(tileLocation, toPlace);
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (this.bridgeFixed.Value)
        this.ApplyFixedBridge();
      else
        this.ApplyMapOverride("Island_Bridge_Broken", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(31, 52, 4, 3)));
      if (this.traderActivated.Value)
        this.ApplyIslandTraderHut();
      if (!this.boulderRemoved.Value)
        return;
      this.ApplyBoulderRemove();
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (this.traderActivated.Value)
      {
        this.removeTemporarySpritesWithIDLocal(8989f);
        this.removeTemporarySpritesWithIDLocal(8988f);
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(33.45f, 70.33f) * 64f + new Vector2(-16f, -32f), false, 0.0f, Color.White)
        {
          delayBeforeAnimationStart = 10,
          interval = 50f,
          totalNumberOfLoops = 99999,
          animationLength = 4,
          light = true,
          lightID = 8989,
          id = 8989f,
          lightRadius = 2f,
          scale = 4f,
          layerDepth = 0.46144f
        });
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(39.45f, 70.33f) * 64f + new Vector2(-16f, -32f), false, 0.0f, Color.White)
        {
          delayBeforeAnimationStart = 10,
          interval = 50f,
          totalNumberOfLoops = 99999,
          animationLength = 4,
          light = true,
          lightID = 8988,
          id = 8988f,
          lightRadius = 2f,
          scale = 4f,
          layerDepth = 0.46144f
        });
      }
      if (this.caveOpened.Value && !Game1.player.hasOrWillReceiveMail("islandNorthCaveOpened"))
        Game1.addMailForTomorrow("islandNorthCaveOpened", true);
      this.suspensionBridges.Clear();
      this.suspensionBridges.Add(new SuspensionBridge(38, 39));
      if (Game1.player.hasOrWillReceiveMail("Saw_Flame_Sprite_North_South"))
        this._sawFlameSpriteSouth = true;
      if (Game1.player.hasOrWillReceiveMail("Saw_Flame_Sprite_North_North"))
        this._sawFlameSpriteNorth = true;
      if (!this._sawFlameSpriteSouth)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Monsters\\Magma Sprite", new Microsoft.Xna.Framework.Rectangle(0, 32, 16, 16), new Vector2(36f, 79f) * 64f, false, 0.0f, Color.White)
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
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\shadow", new Microsoft.Xna.Framework.Rectangle(0, 0, 12, 7), new Vector2(36.2f, 80.4f) * 64f, false, 0.0f, Color.White)
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
      if (!this._sawFlameSpriteNorth)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Monsters\\Magma Sprite", new Microsoft.Xna.Framework.Rectangle(0, 32, 16, 16), new Vector2(41f, 30f) * 64f, false, 0.0f, Color.White)
        {
          id = 9999f,
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
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\shadow", new Microsoft.Xna.Framework.Rectangle(0, 0, 12, 7), new Vector2(41.2f, 31.4f) * 64f, false, 0.0f, Color.White)
        {
          id = 9998f,
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
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + 978);
      if (!Game1.player.team.collectedNutTracker.ContainsKey("Buried_IslandNorth_26_81"))
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(141, 310, 20, 23), new Vector2(23.75f, 77.15f) * 64f, false, 0.0f, Color.White)
        {
          totalNumberOfLoops = 999999,
          animationLength = 2,
          interval = 200f,
          id = 79797f,
          layerDepth = 1f,
          scale = 4f,
          drawAboveAlwaysFront = true
        });
      }
      else
      {
        if (Game1.IsRainingHere((GameLocation) this) || random.NextDouble() >= 0.1)
          return;
        this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(141, 310, 20, 23), new Vector2(23.75f, 77.15f) * 64f, false, 0.0f, Color.White)
        {
          totalNumberOfLoops = 999999,
          animationLength = 2,
          interval = 200f,
          layerDepth = 1f,
          scale = 4f,
          drawAboveAlwaysFront = true
        });
      }
    }

    public override void seasonUpdate(string season, bool onLoad = false)
    {
    }

    public override void updateSeasonalTileSheets(Map map = null)
    {
    }
  }
}
