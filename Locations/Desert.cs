// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Desert
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class Desert : GameLocation
  {
    public const int busDefaultXTile = 17;
    public const int busDefaultYTile = 24;
    private TemporaryAnimatedSprite busDoor;
    private Vector2 busPosition;
    private Vector2 busMotion;
    public bool drivingOff;
    public bool drivingBack;
    public bool leaving;
    private int chimneyTimer = 500;
    private Microsoft.Xna.Framework.Rectangle desertMerchantBounds = new Microsoft.Xna.Framework.Rectangle(2112, 1280, 836, 280);
    public static bool warpedToDesert;
    public static bool boughtMagicRockCandy;
    private Microsoft.Xna.Framework.Rectangle busSource = new Microsoft.Xna.Framework.Rectangle(288, 1247, 128, 64);
    private Microsoft.Xna.Framework.Rectangle pamSource = new Microsoft.Xna.Framework.Rectangle(384, 1311, 15, 19);
    private Microsoft.Xna.Framework.Rectangle transparentWindowSource = new Microsoft.Xna.Framework.Rectangle(0, 0, 21, 41);
    private Vector2 pamOffset = new Vector2(0.0f, 29f);

    public Desert()
    {
    }

    public Desert(string mapPath, string name)
      : base(mapPath, name)
    {
    }

    public static Dictionary<ISalable, int[]> getDesertMerchantTradeStock(
      Farmer who)
    {
      Dictionary<ISalable, int[]> merchantTradeStock = new Dictionary<ISalable, int[]>();
      Item key1 = (Item) new StardewValley.Object(275, 1);
      merchantTradeStock.Add((ISalable) key1, new int[4]
      {
        0,
        int.MaxValue,
        749,
        5
      });
      Item key2 = (Item) new StardewValley.Object(261, 1);
      merchantTradeStock.Add((ISalable) key2, new int[4]
      {
        0,
        int.MaxValue,
        749,
        3
      });
      Item key3 = (Item) new StardewValley.Object(253, 1);
      merchantTradeStock.Add((ISalable) key3, new int[4]
      {
        0,
        int.MaxValue,
        72,
        1
      });
      Item key4 = (Item) new StardewValley.Object(226, 1);
      merchantTradeStock.Add((ISalable) key4, new int[4]
      {
        0,
        int.MaxValue,
        64,
        1
      });
      Item key5 = (Item) new StardewValley.Object(288, 1);
      merchantTradeStock.Add((ISalable) key5, new int[4]
      {
        0,
        int.MaxValue,
        386,
        5
      });
      Item key6 = (Item) new StardewValley.Object(287, 1);
      merchantTradeStock.Add((ISalable) key6, new int[4]
      {
        0,
        int.MaxValue,
        80,
        5
      });
      if (Game1.dayOfMonth % 7 == 0)
      {
        Item key7 = (Item) new StardewValley.Object(Vector2.Zero, 71);
        merchantTradeStock.Add((ISalable) key7, new int[4]
        {
          0,
          int.MaxValue,
          70,
          1
        });
      }
      if (Game1.dayOfMonth % 7 == 1)
      {
        Item key8 = (Item) new StardewValley.Object(178, 3);
        merchantTradeStock.Add((ISalable) key8, new int[4]
        {
          0,
          int.MaxValue,
          749,
          1
        });
      }
      if (Game1.dayOfMonth % 7 == 2)
      {
        Item key9 = (Item) new StardewValley.Object(771, 1);
        merchantTradeStock.Add((ISalable) key9, new int[4]
        {
          0,
          int.MaxValue,
          390,
          5
        });
      }
      if (Game1.dayOfMonth % 7 == 3)
      {
        Item key10 = (Item) new StardewValley.Object(428, 1);
        merchantTradeStock.Add((ISalable) key10, new int[4]
        {
          0,
          int.MaxValue,
          62,
          3
        });
      }
      if (Game1.dayOfMonth % 7 == 4 && !Desert.boughtMagicRockCandy)
      {
        Item key11 = (Item) new StardewValley.Object(279, 1);
        merchantTradeStock.Add((ISalable) key11, new int[4]
        {
          0,
          1,
          74,
          3
        });
      }
      if (Game1.dayOfMonth % 7 == 5)
      {
        Item key12 = (Item) new StardewValley.Object(424, 1);
        merchantTradeStock.Add((ISalable) key12, new int[4]
        {
          0,
          int.MaxValue,
          60,
          1
        });
      }
      if (Game1.dayOfMonth % 7 == 6)
      {
        Item key13 = (Item) new StardewValley.Object(495, 1);
        merchantTradeStock.Add((ISalable) key13, new int[4]
        {
          0,
          int.MaxValue,
          496,
          2
        });
        Item key14 = (Item) new StardewValley.Object(496, 1);
        merchantTradeStock.Add((ISalable) key14, new int[4]
        {
          0,
          int.MaxValue,
          497,
          2
        });
        Item key15 = (Item) new StardewValley.Object(497, 1);
        merchantTradeStock.Add((ISalable) key15, new int[4]
        {
          0,
          int.MaxValue,
          498,
          2
        });
        Item key16 = (Item) new StardewValley.Object(498, 1);
        merchantTradeStock.Add((ISalable) key16, new int[4]
        {
          0,
          int.MaxValue,
          495,
          2
        });
      }
      if (who != null && !who.craftingRecipes.ContainsKey("Warp Totem: Desert"))
      {
        Item key17 = (Item) new StardewValley.Object(261, 1, true);
        merchantTradeStock.Add((ISalable) key17, new int[4]
        {
          0,
          1,
          337,
          10
        });
      }
      if (who != null && who.getFriendshipHeartLevelForNPC("Krobus") >= 10 && (int) (NetFieldBase<int, NetInt>) who.houseUpgradeLevel >= 1 && !who.isMarried() && !who.isEngaged() && !who.hasItemInInventory(808, 1))
      {
        Item key18 = (Item) new StardewValley.Object(808, 1);
        merchantTradeStock.Add((ISalable) key18, new int[4]
        {
          0,
          1,
          769,
          200
        });
      }
      Item key19 = (Item) new Furniture(1971, Vector2.Zero);
      merchantTradeStock.Add((ISalable) key19, new int[4]
      {
        0,
        int.MaxValue,
        767,
        200
      });
      Item key20 = (Item) new Hat(72);
      merchantTradeStock.Add((ISalable) key20, new int[4]
      {
        0,
        int.MaxValue,
        749,
        50
      });
      Item key21 = (Item) new Hat(73);
      if (Game1.stats.DaysPlayed % 2U == 0U)
        key21 = (Item) new Hat(74);
      merchantTradeStock.Add((ISalable) key21, new int[4]
      {
        0,
        int.MaxValue,
        749,
        333
      });
      Item key22 = (Item) new BedFurniture(2508, Vector2.Zero);
      merchantTradeStock.Add((ISalable) key22, new int[4]
      {
        0,
        int.MaxValue,
        797,
        1
      });
      return merchantTradeStock;
    }

    public bool boughtTraderItem(ISalable s, Farmer f, int i)
    {
      if (s.Name == "Magic Rock Candy")
        Desert.boughtMagicRockCandy = true;
      return false;
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.map.GetLayer("Buildings").Tiles[tileLocation] != null)
      {
        int tileIndex = this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex;
        return base.checkAction(tileLocation, viewport, who);
      }
      if ((tileLocation.X == 41 || tileLocation.X == 42) && tileLocation.Y == 24)
      {
        if (this.isTravelingDeserteMerchantHere())
        {
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Desert.getDesertMerchantTradeStock(Game1.player), who: "DesertTrade", on_purchase: new Func<ISalable, Farmer, int, bool>(this.boughtTraderItem));
          return true;
        }
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Desert_Trader_Closed"));
        return true;
      }
      if (tileLocation.X < 34 || tileLocation.X > 38 || tileLocation.Y != 24)
        return base.checkAction(tileLocation, viewport, who);
      Game1.soundBank.PlayCue("camel");
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
        sourceRect = new Microsoft.Xna.Framework.Rectangle(208, 591, 65, 49),
        sourceRectStartingPos = new Vector2(208f, 591f),
        animationLength = 1,
        totalNumberOfLoops = 1,
        interval = 200f,
        scale = 4f,
        position = new Vector2(536f, 340f) * 4f,
        layerDepth = 0.1332f,
        id = 999f
      });
      Game1.player.faceDirection(0);
      Game1.haltAfterCheck = false;
      return true;
    }

    public override string checkForBuriedItem(
      int xLocation,
      int yLocation,
      bool explosion,
      bool detectOnly,
      Farmer who)
    {
      if (!who.secretNotesSeen.Contains(18) || xLocation != 40 || yLocation != 55 || who.mailReceived.Contains("SecretNote18_done"))
        return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
      who.mailReceived.Add("SecretNote18_done");
      Game1.createObjectDebris((int) sbyte.MaxValue, xLocation, yLocation, who.UniqueMultiplayerID, (GameLocation) this);
      return "";
    }

    private void playerReachedBusDoor(Character c, GameLocation l)
    {
      Game1.viewportFreeze = true;
      Game1.player.position.X = -10000f;
      Game1.freezeControls = true;
      Game1.player.CanMove = false;
      this.busDriveOff();
      this.playSound("stoneStep");
    }

    public override StardewValley.Object getFish(
      float millisecondsAfterNibble,
      int bait,
      int waterDepth,
      Farmer who,
      double baitPotency,
      Vector2 bobberTile,
      string locationName = null)
    {
      return (double) bobberTile.Y > 55.0 && Game1.random.NextDouble() < 0.1 ? (StardewValley.Object) new Furniture(2334, Vector2.Zero) : base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    }

    public override bool answerDialogue(Response answer)
    {
      if (this.lastQuestionKey == null || this.afterQuestion != null || !(this.lastQuestionKey.Split(' ')[0] + "_" + answer.responseKey == "DesertBus_Yes"))
        return base.answerDialogue(answer);
      this.playerReachedBusDoor((Character) Game1.player, (GameLocation) this);
      return true;
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.leaving = false;
      Game1.ambientLight = Color.White;
      if (Game1.player.getTileX() == 35 && Game1.player.getTileY() == 43)
        Desert.warpedToDesert = true;
      if (Game1.player.getTileY() > 40 || Game1.player.getTileY() < 10)
      {
        this.drivingOff = false;
        this.drivingBack = false;
        this.busMotion = Vector2.Zero;
        this.busPosition = new Vector2(17f, 24f) * 64f;
        this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
        {
          interval = 999999f,
          animationLength = 6,
          holdLastFrame = true,
          layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.99999974737875E-06),
          scale = 4f
        };
        Game1.changeMusicTrack("wavy");
      }
      else
      {
        if (Game1.isRaining)
          Game1.changeMusicTrack("none");
        this.busPosition = new Vector2(17f, 24f) * 64f;
        this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(368, 1311, 16, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
        {
          interval = 999999f,
          animationLength = 1,
          holdLastFrame = true,
          layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.99999974737875E-06),
          scale = 4f
        };
        Game1.displayFarmer = false;
        this.busDriveBack();
      }
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 513, 208, 101),
        sourceRectStartingPos = new Vector2(0.0f, 513f),
        animationLength = 1,
        totalNumberOfLoops = 9999,
        interval = 99999f,
        scale = 4f,
        position = new Vector2(528f, 298f) * 4f,
        layerDepth = 0.1324f,
        id = 996f
      });
      if (this.isTravelingDeserteMerchantHere())
        this.temporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
          sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 614, 20, 26),
          sourceRectStartingPos = new Vector2(0.0f, 614f),
          animationLength = 1,
          totalNumberOfLoops = 999,
          interval = 99999f,
          scale = 4f,
          position = new Vector2(663f, 354f) * 4f,
          layerDepth = 0.1328f,
          id = 995f
        });
      if (Game1.timeOfDay < Game1.getModeratelyDarkTime())
        return;
      this.lightMerchantLamps();
    }

    private bool isTravelingDeserteMerchantHere() => Game1.currentSeason != "winter" || Game1.dayOfMonth < 15 || Game1.dayOfMonth > 17;

    public override bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character)
    {
      return position.Intersects(this.desertMerchantBounds) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
    }

    public override void performTenMinuteUpdate(int timeOfDay)
    {
      base.performTenMinuteUpdate(timeOfDay);
      if (Game1.currentLocation != this)
        return;
      if (this.isTravelingDeserteMerchantHere())
      {
        if (Game1.random.NextDouble() < 0.33)
          this.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(40, 614, 20, 26),
            sourceRectStartingPos = new Vector2(40f, 614f),
            animationLength = 6,
            totalNumberOfLoops = 1,
            interval = 100f,
            scale = 4f,
            position = new Vector2(663f, 354f) * 4f,
            layerDepth = 0.1336f,
            id = 997f,
            pingPong = true
          });
        else
          this.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(20, 614, 20, 26),
            sourceRectStartingPos = new Vector2(20f, 614f),
            animationLength = 1,
            totalNumberOfLoops = 1,
            interval = (float) Game1.random.Next(100, 800),
            scale = 4f,
            position = new Vector2(663f, 354f) * 4f,
            layerDepth = 0.1332f,
            id = 998f
          });
      }
      if (this.getTemporarySpriteByID(999) == null)
        this.temporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
          sourceRect = new Microsoft.Xna.Framework.Rectangle(208, 591, 65, 49),
          sourceRectStartingPos = new Vector2(208f, 591f),
          animationLength = 1,
          totalNumberOfLoops = 1,
          interval = (float) Game1.random.Next(100, 1200),
          scale = 4f,
          position = new Vector2(536f, 340f) * 4f,
          layerDepth = 0.1332f,
          id = 999f,
          delayBeforeAnimationStart = Game1.random.Next(1000)
        });
      if (timeOfDay != Game1.getModeratelyDarkTime() || Game1.currentLocation != this)
        return;
      this.lightMerchantLamps();
    }

    public void lightMerchantLamps()
    {
      if (this.getTemporarySpriteByID(1000) != null)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
        sourceRect = new Microsoft.Xna.Framework.Rectangle(181, 633, 7, 6),
        sourceRectStartingPos = new Vector2(181f, 633f),
        animationLength = 1,
        totalNumberOfLoops = 9999,
        interval = 99999f,
        scale = 4f,
        position = new Vector2(545f, 309f) * 4f,
        layerDepth = 0.134f,
        id = 1000f,
        light = true,
        lightRadius = 1f,
        lightcolor = Color.Black
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
        sourceRect = new Microsoft.Xna.Framework.Rectangle(181, 633, 7, 6),
        sourceRectStartingPos = new Vector2(181f, 633f),
        animationLength = 1,
        totalNumberOfLoops = 9999,
        interval = 99999f,
        scale = 4f,
        position = new Vector2(644f, 360f) * 4f,
        layerDepth = 0.134f,
        id = 1000f,
        light = true,
        lightRadius = 1f,
        lightcolor = Color.Black
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
        sourceRect = new Microsoft.Xna.Framework.Rectangle(181, 633, 7, 6),
        sourceRectStartingPos = new Vector2(181f, 633f),
        animationLength = 1,
        totalNumberOfLoops = 9999,
        interval = 99999f,
        scale = 4f,
        position = new Vector2(717f, 309f) * 4f,
        layerDepth = 0.134f,
        id = 1000f,
        light = true,
        lightRadius = 1f,
        lightcolor = Color.Black
      });
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      if (this.farmers.Count > 1)
        return;
      this.busDoor = (TemporaryAnimatedSprite) null;
    }

    public void busDriveOff()
    {
      this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
      {
        interval = 999999f,
        animationLength = 6,
        holdLastFrame = true,
        layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.99999974737875E-06),
        scale = 4f
      };
      this.busDoor.timer = 0.0f;
      this.busDoor.interval = 70f;
      this.busDoor.endFunction = new TemporaryAnimatedSprite.endBehavior(this.busStartMovingOff);
      this.localSound("trashcanlid");
      this.drivingBack = false;
      this.busDoor.paused = false;
    }

    public void busDriveBack()
    {
      this.busPosition.X = (float) this.map.GetLayer("Back").DisplayWidth;
      this.busDoor.Position = this.busPosition + new Vector2(16f, 26f) * 4f;
      this.drivingBack = true;
      this.drivingOff = false;
      this.localSound("busDriveOff");
      this.busMotion = new Vector2(-6f, 0.0f);
    }

    private void busStartMovingOff(int extraInfo) => Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
    {
      Game1.globalFadeToClear();
      this.localSound("batFlap");
      this.drivingOff = true;
      this.localSound("busDriveOff");
      Game1.changeMusicTrack("none");
    }));

    public override void performTouchAction(string fullActionString, Vector2 playerStandingPosition)
    {
      if (fullActionString.Split(' ')[0] == "DesertBus")
      {
        Response[] answerChoices = new Response[2]
        {
          new Response("Yes", Game1.content.LoadString("Strings\\Locations:Desert_Return_Yes")),
          new Response("Not", Game1.content.LoadString("Strings\\Locations:Desert_Return_No"))
        };
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Desert_Return_Question"), answerChoices, "DesertBus");
      }
      else
        base.performTouchAction(fullActionString, playerStandingPosition);
    }

    private void doorOpenAfterReturn(int extraInfo)
    {
      this.localSound("batFlap");
      this.busDoor = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1311, 16, 38), this.busPosition + new Vector2(16f, 26f) * 4f, false, 0.0f, Color.White)
      {
        interval = 999999f,
        animationLength = 6,
        holdLastFrame = true,
        layerDepth = (float) (((double) this.busPosition.Y + 192.0) / 10000.0 + 9.99999974737875E-06),
        scale = 4f
      };
      Game1.player.Position = new Vector2(18f, 27f) * 64f;
      this.lastTouchActionLocation = Game1.player.getTileLocation();
      Game1.displayFarmer = true;
      Game1.player.forceCanMove();
      Game1.player.faceDirection(2);
      Game1.changeMusicTrack("wavy");
    }

    private void busLeftToValley()
    {
      Game1.viewport.Y = -100000;
      Game1.viewportFreeze = true;
      Game1.warpFarmer("BusStop", 12, 10, true);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      if (this.drivingOff && !this.leaving)
      {
        this.busMotion.X -= 0.075f;
        if ((double) this.busPosition.X + 512.0 < 0.0)
        {
          this.leaving = true;
          Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.busLeftToValley), 0.01f);
        }
      }
      if (this.drivingBack && this.busMotion != Vector2.Zero)
      {
        Game1.player.Position = this.busDoor.position;
        Game1.player.freezePause = 100;
        if ((double) this.busPosition.X - 1088.0 < 256.0)
          this.busMotion.X = Math.Min(-1f, this.busMotion.X * 0.98f);
        if ((double) Math.Abs(this.busPosition.X - 1088f) <= (double) Math.Abs(this.busMotion.X * 1.5f))
        {
          this.busPosition.X = 1088f;
          this.busMotion = Vector2.Zero;
          Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
          {
            this.drivingBack = false;
            this.busDoor.Position = this.busPosition + new Vector2(16f, 26f) * 4f;
            this.busDoor.pingPong = true;
            this.busDoor.interval = 70f;
            this.busDoor.currentParentTileIndex = 5;
            this.busDoor.endFunction = new TemporaryAnimatedSprite.endBehavior(this.doorOpenAfterReturn);
            this.localSound("trashcanlid");
            Game1.globalFadeToClear();
          }));
        }
      }
      if (!this.busMotion.Equals(Vector2.Zero))
      {
        this.busPosition += this.busMotion;
        if (this.busDoor != null)
          this.busDoor.Position += this.busMotion;
      }
      if (this.busDoor != null)
        this.busDoor.update(time);
      if (!this.isTravelingDeserteMerchantHere())
        return;
      this.chimneyTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.chimneyTimer > 0)
        return;
      this.chimneyTimer = 500;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(670f, 308f) * 4f, false, 1f / 500f, new Color((int) byte.MaxValue, 222, 198))
      {
        alpha = 0.05f,
        alphaFade = -0.01f,
        alphaFadeFade = -8E-05f,
        motion = new Vector2(0.0f, -0.5f),
        acceleration = new Vector2(1f / 500f, 0.0f),
        interval = 99999f,
        layerDepth = 1f,
        scale = 3f,
        scaleChange = 0.01f,
        rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0)
      });
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      for (int x = 33; x < 46; ++x)
      {
        for (int y = 20; y < 25; ++y)
          this.removeEverythingExceptCharactersFromThisTile(x, y);
      }
      Desert.boughtMagicRockCandy = false;
    }

    public override bool isTilePlaceable(Vector2 v, Item item = null) => ((double) v.X < 33.0 || (double) v.X >= 46.0 || (double) v.Y < 20.0 || (double) v.Y >= 25.0) && base.isTilePlaceable(v, item);

    public override bool shouldHideCharacters() => this.drivingOff || this.drivingBack;

    public override void draw(SpriteBatch spriteBatch)
    {
      base.draw(spriteBatch);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) (int) this.busPosition.Y)), new Microsoft.Xna.Framework.Rectangle?(this.busSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0) / 10000.0));
      if (this.busDoor != null)
        this.busDoor.draw(spriteBatch);
      if (!this.drivingOff && !this.drivingBack)
        return;
      if (this.drivingOff && Desert.warpedToDesert)
      {
        Game1.player.faceDirection(3);
        Game1.player.blinkTimer = -1000;
        Game1.player.FarmerRenderer.draw(spriteBatch, new FarmerSprite.AnimationFrame(117, 99999, 0, false, true), 117, new Microsoft.Xna.Framework.Rectangle(48, 608, 16, 32), Game1.GlobalToLocal(new Vector2((float) (int) ((double) this.busPosition.X + 4.0), (float) (int) ((double) this.busPosition.Y - 8.0)) + this.pamOffset * 4f), Vector2.Zero, (float) (((double) this.busPosition.Y + 192.0 + 4.0) / 10000.0), Color.White, 0.0f, 1f, Game1.player);
        spriteBatch.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) ((int) this.busPosition.Y - 40)) + this.pamOffset * 4f), new Microsoft.Xna.Framework.Rectangle?(this.transparentWindowSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0 + 8.0) / 10000.0));
      }
      else
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (int) this.busPosition.X, (float) (int) this.busPosition.Y) + this.pamOffset * 4f), new Microsoft.Xna.Framework.Rectangle?(this.pamSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.busPosition.Y + 192.0 + 4.0) / 10000.0));
    }
  }
}
