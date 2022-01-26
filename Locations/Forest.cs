// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Forest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class Forest : GameLocation
  {
    [XmlIgnore]
    public readonly NetObjectList<FarmAnimal> marniesLivestock = new NetObjectList<FarmAnimal>();
    [XmlIgnore]
    public readonly NetList<Microsoft.Xna.Framework.Rectangle, NetRectangle> travelingMerchantBounds = new NetList<Microsoft.Xna.Framework.Rectangle, NetRectangle>();
    [XmlIgnore]
    public readonly NetBool netTravelingMerchantDay = new NetBool(false);
    [XmlElement("log")]
    public readonly NetRef<ResourceClump> netLog = new NetRef<ResourceClump>();
    private int chimneyTimer = 500;
    private bool hasShownCCUpgrade;
    private Microsoft.Xna.Framework.Rectangle hatterSource = new Microsoft.Xna.Framework.Rectangle(600, 1957, 64, 32);
    private Vector2 hatterPos = new Vector2(2056f, 6016f);

    [XmlIgnore]
    public bool travelingMerchantDay
    {
      get => this.netTravelingMerchantDay.Value;
      set => this.netTravelingMerchantDay.Value = value;
    }

    [XmlIgnore]
    public ResourceClump log
    {
      get => this.netLog.Value;
      set => this.netLog.Value = value;
    }

    public Forest()
    {
    }

    public Forest(string map, string name)
      : base(map, name)
    {
      this.marniesLivestock.Add(new FarmAnimal("Dairy Cow", Game1.multiplayer.getNewID(), -1L));
      this.marniesLivestock.Add(new FarmAnimal("Dairy Cow", Game1.multiplayer.getNewID(), -1L));
      this.marniesLivestock[0].Position = new Vector2(6272f, 1280f);
      this.marniesLivestock[1].Position = new Vector2(6464f, 1280f);
      this.log = new ResourceClump(602, 2, 2, new Vector2(1f, 6f));
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.marniesLivestock, (INetSerializable) this.travelingMerchantBounds, (INetSerializable) this.netTravelingMerchantDay, (INetSerializable) this.netLog);
    }

    public void removeSewerTrash()
    {
      this.ApplyMapOverride("Forest-SewerClean", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(83, 97, 24, 12)));
      this.setMapTileIndex(43, 106, -1, "Buildings");
      this.setMapTileIndex(17, 106, -1, "Buildings");
      this.setMapTileIndex(13, 105, -1, "Buildings");
      this.setMapTileIndex(4, 85, -1, "Buildings");
      this.setMapTileIndex(2, 85, -1, "Buildings");
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.addFrog();
      if (Game1.year <= 2 || this.getCharacterFromName("TrashBear") == null || !NetWorldState.checkAnywhereForWorldStateID("trashBearDone"))
        return;
      this.characters.Remove(this.getCharacterFromName("TrashBear"));
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (force)
        this.hasShownCCUpgrade = false;
      if (NetWorldState.checkAnywhereForWorldStateID("trashBearDone"))
        this.removeSewerTrash();
      if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
        return;
      this.showCommunityUpgradeShortcuts();
    }

    private void showCommunityUpgradeShortcuts()
    {
      if (this.hasShownCCUpgrade)
        return;
      this.removeTile(119, 36, "Buildings");
      LargeTerrainFeature largeTerrainFeature1 = (LargeTerrainFeature) null;
      foreach (LargeTerrainFeature largeTerrainFeature2 in this.largeTerrainFeatures)
      {
        if (largeTerrainFeature2.tilePosition.Equals((object) new Vector2(119f, 35f)))
        {
          largeTerrainFeature1 = largeTerrainFeature2;
          break;
        }
      }
      if (largeTerrainFeature1 != null)
        this.largeTerrainFeatures.Remove(largeTerrainFeature1);
      this.hasShownCCUpgrade = true;
      this.warps.Add(new Warp(120, 35, "Beach", 0, 6, false));
      this.warps.Add(new Warp(120, 36, "Beach", 0, 6, false));
    }

    protected override void resetSharedState()
    {
      base.resetSharedState();
      if (Game1.year <= 2 || Game1.isRaining || Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason) || this.getCharacterFromName("TrashBear") != null || NetWorldState.checkAnywhereForWorldStateID("trashBearDone"))
        return;
      this.characters.Add((NPC) new TrashBear());
    }

    public override bool performToolAction(Tool t, int tileX, int tileY)
    {
      if (this.log == null || !this.log.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.log.tile).Contains(tileX * 64, tileY * 64))
        return base.performToolAction(t, tileX, tileY);
      if (this.log.performToolAction(t, 1, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.log.tile, (GameLocation) this))
        this.log = (ResourceClump) null;
      return true;
    }

    protected virtual bool isWizardHouseUnlocked()
    {
      if (Game1.player.mailReceived.Contains("wizardJunimoNote") || Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
        return true;
      int num1 = Game1.MasterPlayer.mailReceived.Contains("ccFishTank") ? 1 : 0;
      bool flag1 = Game1.MasterPlayer.mailReceived.Contains("ccBulletin");
      bool flag2 = Game1.MasterPlayer.mailReceived.Contains("ccPantry");
      bool flag3 = Game1.MasterPlayer.mailReceived.Contains("ccVault");
      bool flag4 = Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom");
      bool flag5 = Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom");
      int num2 = flag1 ? 1 : 0;
      return (num1 & num2 & (flag2 ? 1 : 0) & (flag3 ? 1 : 0) & (flag4 ? 1 : 0) & (flag5 ? 1 : 0)) != 0;
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      int num = this.map.GetLayer("Buildings").Tiles[tileLocation] != null ? this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex : -1;
      if (num == 901 && !this.isWizardHouseUnlocked())
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Forest_WizardTower_Locked"));
        return false;
      }
      if (base.checkAction(tileLocation, viewport, who))
        return true;
      switch (num)
      {
        case 1394:
          if (who.hasRustyKey && !who.mailReceived.Contains("OpenedSewer"))
          {
            this.playSound("openBox");
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Forest_OpenedSewer")));
            who.mailReceived.Add("OpenedSewer");
            break;
          }
          if (who.mailReceived.Contains("OpenedSewer"))
          {
            Game1.warpFarmer("Sewer", 3, 48, 0);
            this.playSound("openChest");
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
          break;
        case 1972:
          if (who.achievements.Count > 0)
          {
            Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getHatStock(), who: "HatMouse");
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Forest_HatMouseStore_Abandoned"));
          break;
      }
      if (this.travelingMerchantDay && Game1.timeOfDay < 2000)
      {
        if (tileLocation.X == 27 && tileLocation.Y == 11)
        {
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getTravelingMerchantStock((int) ((long) Game1.uniqueIDForThisGame + (long) Game1.stats.DaysPlayed)), who: "Traveler", on_purchase: new Func<ISalable, Farmer, int, bool>(Utility.onTravelingMerchantShopPurchase));
          return true;
        }
        if (tileLocation.X == 23 && tileLocation.Y == 11)
        {
          this.playSound("pig");
          return true;
        }
      }
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64);
      if (this.log == null || !this.log.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.log.tile).Intersects(rectangle))
        return false;
      this.log.performUseAction(new Vector2((float) tileLocation.X, (float) tileLocation.Y), (GameLocation) this);
      return true;
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
      if (this.log != null && this.log.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.log.tile).Intersects(position))
        return true;
      if (this.travelingMerchantBounds != null)
      {
        foreach (Microsoft.Xna.Framework.Rectangle travelingMerchantBound in this.travelingMerchantBounds)
        {
          if (position.Intersects(travelingMerchantBound))
            return true;
        }
      }
      return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement);
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      if (dayOfMonth % 7 % 5 == 0)
      {
        this.travelingMerchantDay = true;
        this.travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(1472, 640, 492, 116));
        this.travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(1652, 744, 76, 48));
        this.travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(1812, 744, 104, 48));
        foreach (Microsoft.Xna.Framework.Rectangle travelingMerchantBound in this.travelingMerchantBounds)
          Utility.clearObjectsInArea(travelingMerchantBound, (GameLocation) this);
        if (Game1.IsMasterGame && Game1.netWorldState.Value.VisitsUntilY1Guarantee >= 0)
          --Game1.netWorldState.Value.VisitsUntilY1Guarantee;
      }
      else
      {
        this.travelingMerchantBounds.Clear();
        this.travelingMerchantDay = false;
      }
      if (Game1.currentSeason.Equals("spring"))
      {
        for (int index = 0; index < 7; ++index)
        {
          Vector2 tileLocation = new Vector2((float) Game1.random.Next(70, this.map.Layers[0].LayerWidth - 10), (float) Game1.random.Next(68, this.map.Layers[0].LayerHeight - 15));
          if ((double) tileLocation.Y > 30.0)
          {
            foreach (Vector2 openTile in Utility.recursiveFindOpenTiles((GameLocation) this, tileLocation, 16))
            {
              string str = this.doesTileHaveProperty((int) openTile.X, (int) openTile.Y, "Diggable", "Back");
              if (!this.terrainFeatures.ContainsKey(openTile) && str != null && Game1.random.NextDouble() < 1.0 - (double) Vector2.Distance(tileLocation, openTile) * 0.150000005960464)
                this.terrainFeatures.Add(openTile, (TerrainFeature) new HoeDirt(0, new Crop(true, 1, (int) openTile.X, (int) openTile.Y)));
            }
          }
        }
      }
      if (Game1.year <= 2 || this.getCharacterFromName("TrashBear") == null)
        return;
      this.characters.Remove(this.getCharacterFromName("TrashBear"));
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      foreach (FarmAnimal farmAnimal in (NetList<FarmAnimal, NetRef<FarmAnimal>>) this.marniesLivestock)
        farmAnimal.updateWhenCurrentLocation(time, (GameLocation) this);
      if (this.log != null)
        this.log.tickUpdate(time, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.log.tile, (GameLocation) this);
      if (Game1.timeOfDay >= 2000)
        return;
      if (this.travelingMerchantDay)
      {
        if (Game1.random.NextDouble() < 0.001)
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(99, 1423, 13, 19), new Vector2(1472f, 668f), false, 0.0f, Color.White)
          {
            interval = (float) Game1.random.Next(500, 1500),
            layerDepth = 0.07682f,
            scale = 4f
          });
        if (Game1.random.NextDouble() < 0.001)
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(51, 1444, 5, 5), new Vector2(1500f, 744f), false, 0.0f, Color.White)
          {
            interval = 500f,
            animationLength = 1,
            layerDepth = 0.07682f,
            scale = 4f
          });
        if (Game1.random.NextDouble() < 0.003)
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(89, 1445, 6, 3), new Vector2(1764f, 664f), false, 0.0f, Color.White)
          {
            interval = 50f,
            animationLength = 3,
            pingPong = true,
            totalNumberOfLoops = 1,
            layerDepth = 0.07682f,
            scale = 4f
          });
      }
      this.chimneyTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.chimneyTimer > 0)
        return;
      this.chimneyTimer = this.travelingMerchantDay ? 500 : Game1.random.Next(200, 2000);
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), this.travelingMerchantDay ? new Vector2(1868f, 524f) : new Vector2(5592f, 608f), false, 1f / 500f, Color.Gray)
      {
        alpha = 0.75f,
        motion = new Vector2(0.0f, -0.5f),
        acceleration = new Vector2(1f / 500f, 0.0f),
        interval = 99999f,
        layerDepth = 1f,
        scale = 3f,
        scaleChange = 0.01f,
        rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0)
      });
      if (!this.travelingMerchantDay)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(225, 1388, 7, 5), new Vector2(1868f, 536f), false, 0.0f, Color.White)
      {
        interval = (float) (this.chimneyTimer - this.chimneyTimer / 5),
        animationLength = 1,
        layerDepth = 0.99f,
        scale = 4.3f,
        scaleChange = -0.015f
      });
    }

    public override void performTenMinuteUpdate(int timeOfDay)
    {
      base.performTenMinuteUpdate(timeOfDay);
      if (!this.travelingMerchantDay || Game1.random.NextDouble() >= 0.4)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(57, 1430, 4, 12), new Vector2(1792f, 656f), false, 0.0f, Color.White)
      {
        interval = 50f,
        animationLength = 10,
        pingPong = true,
        totalNumberOfLoops = 1,
        layerDepth = 0.07682f,
        scale = 4f
      });
      if (Game1.random.NextDouble() >= 0.66)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(89, 1445, 6, 3), new Vector2(1764f, 664f), false, 0.0f, Color.White)
      {
        interval = 50f,
        animationLength = 3,
        pingPong = true,
        totalNumberOfLoops = 1,
        layerDepth = 0.07683001f,
        scale = 4f
      });
    }

    public override int getFishingLocation(Vector2 tile) => (double) tile.X < 53.0 && (double) tile.Y < 43.0 ? 1 : 0;

    public override StardewValley.Object getFish(
      float millisecondsAfterNibble,
      int bait,
      int waterDepth,
      Farmer who,
      double baitPotency,
      Vector2 bobberTile,
      string locationName = null)
    {
      bool flag = this.IsUsingMagicBait(who);
      if (who.getTileX() == 58 && who.getTileY() == 87 && who.FishingLevel >= 6 && waterDepth >= 3 && Game1.random.NextDouble() < 0.5)
      {
        if (Game1.player.team.SpecialOrderRuleActive("LEGENDARY_FAMILY"))
          return new StardewValley.Object(902, 1);
        if (!who.fishCaught.ContainsKey(775) && Game1.currentSeason.Equals("winter") | flag)
          return new StardewValley.Object(775, 1);
      }
      if ((double) bobberTile.Y <= 108.0 || Game1.player.mailReceived.Contains("caughtIridiumKrobus"))
        return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
      Game1.player.mailReceived.Add("caughtIridiumKrobus");
      return (StardewValley.Object) new Furniture(2396, Vector2.Zero);
    }

    public override void draw(SpriteBatch spriteBatch)
    {
      base.draw(spriteBatch);
      foreach (Character character in (NetList<FarmAnimal, NetRef<FarmAnimal>>) this.marniesLivestock)
        character.draw(spriteBatch);
      if (this.log != null)
        this.log.draw(spriteBatch, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.log.tile);
      if (this.travelingMerchantDay)
      {
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(1536f, 512f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(142, 1382, 109, 70)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0768f);
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(1472f, 672f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(112, 1424, 30, 24)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07681f);
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(1536f, 728f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(142, 1424, 16, 3)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07682f);
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(1544f, 600f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(71, 1966, 18, 18)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07678001f);
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(1472f, 608f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(167, 1966, 18, 18)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07678001f);
        if (Game1.timeOfDay >= 2000)
          spriteBatch.Draw(Game1.staminaRect, Game1.GlobalToLocal(Game1.viewport, new Microsoft.Xna.Framework.Rectangle(1744, 640, 64, 64)), new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 0.07684001f);
      }
      if (Game1.player.achievements.Count <= 0)
        return;
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(this.hatterPos), new Microsoft.Xna.Framework.Rectangle?(this.hatterSource), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.6016f);
    }
  }
}
