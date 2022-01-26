// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.BeachNightMarket
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class BeachNightMarket : GameLocation
  {
    private Texture2D shopClosedTexture;
    private float smokeTimer;
    private string paintingMailKey;
    private bool hasReceivedFreeGift;
    private bool hasShownCCUpgrade;

    public BeachNightMarket() => this.forceLoadPathLayerLights = true;

    public BeachNightMarket(string mapPath, string name)
      : base(mapPath, name)
    {
      this.forceLoadPathLayerLights = true;
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      this.objects.Clear();
      this.hasReceivedFreeGift = false;
      this.paintingMailKey = "NightMarketYear" + Game1.year.ToString() + "Day" + this.getDayOfNightMarket().ToString() + "_paintingSold";
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (Game1.timeOfDay < 1700)
      {
        b.Draw(this.shopClosedTexture, Game1.GlobalToLocal(new Vector2(39f, 29f) * 64f + new Vector2(-1f, -3f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(72, 167, 16, 17)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
        b.Draw(this.shopClosedTexture, Game1.GlobalToLocal(new Vector2(47f, 34f) * 64f + new Vector2(7f, -3f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(45, 170, 26, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
        b.Draw(this.shopClosedTexture, Game1.GlobalToLocal(new Vector2(19f, 31f) * 64f + new Vector2(6f, 10f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(89, 164, 18, 23)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
      }
      if (Game1.player.mailReceived.Contains(this.paintingMailKey))
        return;
      b.Draw(this.shopClosedTexture, Game1.GlobalToLocal(new Vector2(41f, 33f) * 64f + new Vector2(2f, 2f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(144 + (this.getDayOfNightMarket() - 1 + (Game1.year - 1) % 3 * 3) * 28, 201, 28, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.225f);
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.map.GetLayer("Buildings").Tiles[tileLocation] != null)
      {
        switch (this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex)
        {
          case 68:
            if (Game1.timeOfDay < 1700)
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterClosed"));
              break;
            }
            if (Game1.player.mailReceived.Contains(this.paintingMailKey))
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterSold"));
              break;
            }
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterQuestion"), this.createYesNoResponses(), "PainterQuestion");
            break;
          case 69:
          case 877:
            if (Game1.timeOfDay < 1700)
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverClosed"));
              break;
            }
            if (!this.hasReceivedFreeGift)
            {
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverQuestion"), this.createYesNoResponses(), "GiftGiverQuestion");
              break;
            }
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverEnjoy"));
            break;
          case 70:
            if (Game1.timeOfDay < 1700)
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_Closed"));
              break;
            }
            Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(this.geMagicShopStock(), who: "magicBoatShop");
            break;
          case 399:
            if (Game1.timeOfDay < 1700)
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_Closed"));
              break;
            }
            Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getTravelingMerchantStock((int) ((long) Game1.uniqueIDForThisGame + (long) Game1.stats.DaysPlayed)), who: "TravelerNightMarket", on_purchase: new Func<ISalable, Farmer, int, bool>(Utility.onTravelingMerchantShopPurchase));
            break;
          case 595:
            if (Game1.timeOfDay < 1700)
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_Closed"));
              break;
            }
            Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(this.getBlueBoatStock(), who: "BlueBoat");
            break;
          case 653:
            if ((Game1.getLocationFromName("Submarine") as Submarine).submerged.Value || Game1.netWorldState.Value.IsSubmarineLocked)
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_SubmarineInUse"));
              return true;
            }
            break;
          case 1285:
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_WarperQuestion"), this.createYesNoResponses(), "WarperQuestion");
            break;
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public int getDayOfNightMarket()
    {
      switch (Game1.dayOfMonth)
      {
        case 15:
          return 1;
        case 16:
          return 2;
        case 17:
          return 3;
        default:
          return -1;
      }
    }

    public override bool catchOceanCrabPotFishFromThisSpot(int x, int y) => true;

    public override StardewValley.Object getFish(
      float millisecondsAfterNibble,
      int bait,
      int waterDepth,
      Farmer who,
      double baitPotency,
      Vector2 bobberTile,
      string locationName = null)
    {
      return Game1.getLocationFromName("Beach") is Beach locationFromName ? locationFromName.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName) : base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    }

    public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
    {
      switch (questionAndAnswer)
      {
        case "WarperQuestion_Yes":
          if (Game1.player.Money < 250)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
          }
          else
          {
            Game1.player.Money -= 250;
            Game1.player.CanMove = true;
            new StardewValley.Object(688, 1).performUseAction((GameLocation) this);
            Game1.player.freezePause = 5000;
          }
          return true;
        case "PainterQuestion_Yes":
          if (Game1.player.mailReceived.Contains(this.paintingMailKey))
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterSold"));
            break;
          }
          if (Game1.player.Money < 1200)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            break;
          }
          Game1.player.Money -= 1200;
          Game1.activeClickableMenu = (IClickableMenu) null;
          Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item) new Furniture(1838 + ((this.getDayOfNightMarket() - 1) * 2 + (Game1.year - 1) % 3 * 6), Vector2.Zero));
          Game1.multiplayer.globalChatInfoMessage("Lupini", Game1.player.Name);
          Game1.multiplayer.broadcastPartyWideMail(this.paintingMailKey, Multiplayer.PartyWideMessageQueue.SeenMail, true);
          break;
        case "GiftGiverQuestion_Yes":
          if (this.hasReceivedFreeGift)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverEnjoy"));
            break;
          }
          Game1.player.freezePause = 5000;
          this.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = this.shopClosedTexture,
            layerDepth = 0.2442f,
            scale = 4f,
            sourceRectStartingPos = new Vector2(354f, 168f),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(354, 168, 32, 32),
            animationLength = 1,
            id = 777f,
            holdLastFrame = true,
            interval = 250f,
            position = new Vector2(13f, 36f) * 64f,
            delayBeforeAnimationStart = 500,
            endFunction = new TemporaryAnimatedSprite.endBehavior(this.getFreeGiftPartOne)
          });
          this.hasReceivedFreeGift = true;
          break;
        case null:
          return false;
      }
      return base.answerDialogueAction(questionAndAnswer, questionParams);
    }

    public void getFreeGiftPartOne(int extra)
    {
      this.removeTemporarySpritesWithIDLocal(777f);
      Game1.soundBank.PlayCue("Milking");
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.shopClosedTexture,
        layerDepth = 0.2442f,
        scale = 4f,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(386, 168, 32, 32),
        animationLength = 1,
        id = 778f,
        holdLastFrame = true,
        interval = 9500f,
        position = new Vector2(13f, 36f) * 64f
      });
      for (int index = 0; index <= 2000; index += 100)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = this.shopClosedTexture,
          delayBeforeAnimationStart = index,
          id = 778f,
          layerDepth = 0.2443f,
          scale = 4f,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(362, 170, 2, 2),
          animationLength = 1,
          interval = 100f,
          position = new Vector2(13f, 36f) * 64f + new Vector2(8f, 12f) * 4f,
          motion = new Vector2(0.0f, 2f)
        });
        if (index == 2000)
          this.temporarySprites.Last<TemporaryAnimatedSprite>().endFunction = new TemporaryAnimatedSprite.endBehavior(this.getFreeGift);
      }
    }

    public void getFreeGift(int extra)
    {
      Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item) new StardewValley.Object(395, 1));
      this.removeTemporarySpritesWithIDLocal(778f);
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (force)
        this.hasShownCCUpgrade = false;
      if ((bool) (NetFieldBase<bool, NetBool>) (Game1.getLocationFromName("Beach") as Beach).bridgeFixed || NetWorldState.checkAnywhereForWorldStateID("beachBridgeFixed"))
        Beach.fixBridge((GameLocation) this);
      if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
        return;
      Beach.showCommunityUpgradeShortcuts((GameLocation) this, ref this.hasShownCCUpgrade);
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (Game1.timeOfDay >= 1700)
        Game1.changeMusicTrack("night_market");
      else
        Game1.changeMusicTrack("ocean");
      this.shopClosedTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
      this.temporarySprites.Add((TemporaryAnimatedSprite) new EmilysParrot(new Vector2(2968f, 2056f)));
      this.paintingMailKey = "NightMarketYear" + Game1.year.ToString() + "Day" + this.getDayOfNightMarket().ToString() + "_paintingSold";
    }

    public override void performTenMinuteUpdate(int timeOfDay)
    {
      base.performTenMinuteUpdate(timeOfDay);
      if (timeOfDay != 1700)
        return;
      if (Game1.currentSeason.Equals("winter") && Game1.dayOfMonth >= 15 && Game1.dayOfMonth <= 17)
        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Events:BeachNightMarket_NowOpen"));
      if (!Game1.currentLocation.Equals((GameLocation) this))
        return;
      Game1.changeMusicTrack("night_market");
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.shopClosedTexture,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(89, 164, 18, 23),
        layerDepth = 1f / 1000f,
        interval = 100f,
        position = new Vector2(19f, 31f) * 64f + new Vector2(6f, 10f) * 4f,
        scale = 4f,
        animationLength = 3
      });
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this.smokeTimer -= (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.smokeTimer > 0.0)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.shopClosedTexture,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 180, 9, 11),
        sourceRectStartingPos = new Vector2(0.0f, 180f),
        layerDepth = 1f,
        interval = 250f,
        position = new Vector2(35f, 38f) * 64f + new Vector2(9f, 6f) * 4f,
        scale = 4f,
        scaleChange = 0.005f,
        alpha = 0.75f,
        alphaFade = 0.005f,
        motion = new Vector2(0.0f, -0.5f),
        acceleration = new Vector2((float) (Game1.random.NextDouble() - 0.5) / 100f, 0.0f),
        animationLength = 3,
        holdLastFrame = true
      });
      this.smokeTimer = 1250f;
    }

    public Dictionary<ISalable, int[]> getBlueBoatStock() => new Dictionary<ISalable, int[]>()
    {
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 40),
        new int[2]{ 200, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 41),
        new int[2]{ 200, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 42),
        new int[2]{ 200, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 43),
        new int[2]{ 200, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 44),
        new int[2]{ 200, int.MaxValue }
      },
      {
        (ISalable) new Furniture(2397, Vector2.Zero),
        new int[2]{ 800, int.MaxValue }
      },
      {
        (ISalable) new Furniture(2398, Vector2.Zero),
        new int[2]{ 800, int.MaxValue }
      },
      {
        (ISalable) new Furniture(1975, Vector2.Zero),
        new int[2]{ 1000, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 48),
        new int[2]{ 500, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 184),
        new int[2]{ 500, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 188),
        new int[2]{ 500, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 192),
        new int[2]{ 500, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 196),
        new int[2]{ 500, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 200),
        new int[2]{ 500, int.MaxValue }
      },
      {
        (ISalable) new StardewValley.Object(Vector2.Zero, 204),
        new int[2]{ 500, int.MaxValue }
      }
    };

    public Dictionary<ISalable, int[]> geMagicShopStock()
    {
      Dictionary<ISalable, int[]> dictionary = new Dictionary<ISalable, int[]>();
      switch (this.getDayOfNightMarket())
      {
        case 1:
          dictionary.Add((ISalable) new StardewValley.Object(Vector2.Zero, 47), new int[2]
          {
            200,
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(Vector2.Zero, 52), new int[2]
          {
            500,
            int.MaxValue
          });
          dictionary.Add((ISalable) new Hat(39), new int[2]
          {
            5000,
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(472, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[472].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(473, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[473].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(474, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[474].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(475, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[475].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(427, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[427].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(477, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[477].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(429, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[429].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          if (Game1.year > 1)
            dictionary.Add((ISalable) new StardewValley.Object(476, 1), new int[2]
            {
              Convert.ToInt32(Game1.objectInformation[476].Split('/')[1]) * 2,
              int.MaxValue
            });
          dictionary.Add((ISalable) new Furniture(1796, Vector2.Zero), new int[2]
          {
            15000,
            int.MaxValue
          });
          break;
        case 2:
          dictionary.Add((ISalable) new StardewValley.Object(Vector2.Zero, 33), new int[2]
          {
            200,
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(Vector2.Zero, 53), new int[2]
          {
            500,
            int.MaxValue
          });
          dictionary.Add((ISalable) new Hat(39), new int[2]
          {
            2500,
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(479, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[479].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(480, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[480].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(481, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[481].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(482, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[482].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(483, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[483].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(484, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[484].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(453, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[453].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(455, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[455].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(302, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[302].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(487, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[487].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(431, 1), new int[2]
          {
            (int) (200.0 * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          if (Game1.year > 1)
            dictionary.Add((ISalable) new StardewValley.Object(485, 1), new int[2]
            {
              (int) ((double) (Convert.ToInt32(Game1.objectInformation[485].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
              int.MaxValue
            });
          dictionary.Add((ISalable) new Furniture(1796, Vector2.Zero), new int[2]
          {
            15000,
            int.MaxValue
          });
          break;
        case 3:
          dictionary.Add((ISalable) new StardewValley.Object(Vector2.Zero, 46), new int[2]
          {
            200,
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(Vector2.Zero, 54), new int[2]
          {
            500,
            int.MaxValue
          });
          dictionary.Add((ISalable) new Hat(39), new int[2]
          {
            10000,
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(487, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[487].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(488, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[488].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(490, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[490].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(491, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[491].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(492, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[492].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(493, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[493].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(483, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[483].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(425, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[425].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(299, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[299].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(301, 1), new int[2]
          {
            (int) ((double) (Convert.ToInt32(Game1.objectInformation[301].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          dictionary.Add((ISalable) new StardewValley.Object(431, 1), new int[2]
          {
            (int) (200.0 * (double) Game1.MasterPlayer.difficultyModifier),
            int.MaxValue
          });
          if (Game1.year > 1)
            dictionary.Add((ISalable) new StardewValley.Object(489, 1), new int[2]
            {
              (int) ((double) (Convert.ToInt32(Game1.objectInformation[489].Split('/')[1]) * 2) * (double) Game1.MasterPlayer.difficultyModifier),
              int.MaxValue
            });
          dictionary.Add((ISalable) new Furniture(1796, Vector2.Zero), new int[2]
          {
            15000,
            int.MaxValue
          });
          break;
      }
      int num1 = 0;
      int num2 = 0;
      foreach (KeyValuePair<Vector2, int> pair in Game1.netWorldState.Value.MuseumPieces.Pairs)
      {
        string str = Game1.objectInformation[pair.Value].Split('/')[3];
        if (str.Contains("Arch"))
          ++num2;
        if (str.Contains("Minerals"))
          ++num1;
      }
      if (num2 >= 20)
        dictionary.Add((ISalable) new StardewValley.Object(Vector2.Zero, 139), new int[2]
        {
          5000,
          int.MaxValue
        });
      if (num1 + num2 >= 40)
        dictionary.Add((ISalable) new StardewValley.Object(Vector2.Zero, 140), new int[2]
        {
          5000,
          int.MaxValue
        });
      return dictionary;
    }
  }
}
