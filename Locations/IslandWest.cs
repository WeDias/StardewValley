// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandWest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class IslandWest : IslandLocation
  {
    [XmlElement("addedSlimesToday")]
    private readonly NetBool addedSlimesToday = new NetBool();
    [XmlElement("sandDuggy")]
    public NetRef<SandDuggy> sandDuggy = new NetRef<SandDuggy>();
    [XmlElement("farmhouseRestored")]
    public readonly NetBool farmhouseRestored = new NetBool();
    [XmlElement("farmhouseMailbox")]
    public readonly NetBool farmhouseMailbox = new NetBool();
    [XmlElement("farmObelisk")]
    public readonly NetBool farmObelisk = new NetBool();
    public Point shippingBinPosition = new Point(90, 39);
    private TemporaryAnimatedSprite shippingBinLid;
    private Microsoft.Xna.Framework.Rectangle shippingBinLidOpenArea;

    public override void SetBuriedNutLocations()
    {
      this.buriedNutPoints.Add(new Point(21, 81));
      this.buriedNutPoints.Add(new Point(62, 76));
      this.buriedNutPoints.Add(new Point(39, 24));
      this.buriedNutPoints.Add(new Point(88, 14));
      this.buriedNutPoints.Add(new Point(43, 74));
      this.buriedNutPoints.Add(new Point(30, 75));
      base.SetBuriedNutLocations();
    }

    public override bool CanPlantSeedsHere(int crop_index, int tile_x, int tile_y) => this.getTileSheetIDAt(tile_x, tile_y, "Back") == "untitled tile sheet2" || base.CanPlantSeedsHere(crop_index, tile_x, tile_y);

    public override bool SeedsIgnoreSeasonsHere() => true;

    public override bool CanPlantTreesHere(int sapling_index, int tile_x, int tile_y)
    {
      if (this.getTileSheetIDAt(tile_x, tile_y, "Back") == "untitled tile sheet2" || StardewValley.Object.isWildTreeSeed(sapling_index))
      {
        string str = this.doesTileHavePropertyNoNull(tile_x, tile_y, "Type", "Back");
        if (str == "Dirt" || str == "Grass" || str == "")
          return true;
      }
      return base.CanPlantTreesHere(sapling_index, tile_x, tile_y);
    }

    public IslandWest()
    {
    }

    public override int getFishingLocation(Vector2 tile) => (double) tile.X > 35.0 && (double) tile.Y < 81.0 ? 2 : 1;

    public override bool catchOceanCrabPotFishFromThisSpot(int x, int y) => !(x > 38 & y < 85);

    public override void digUpArtifactSpot(int xLocation, int yLocation, Farmer who)
    {
      Random random = new Random(xLocation * 2000 + yLocation * 77 + (int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + (int) Game1.stats.DirtHoed);
      if (Game1.player.hasOrWillReceiveMail("islandNorthCaveOpened"))
      {
        if (random.NextDouble() < 0.1)
        {
          Game1.createItemDebris((Item) new StardewValley.Object(825, 1), new Vector2((float) xLocation, (float) yLocation) * 64f, -1, (GameLocation) this);
          return;
        }
        if (random.NextDouble() < 0.25)
        {
          Game1.createItemDebris((Item) new StardewValley.Object(826, 1), new Vector2((float) xLocation, (float) yLocation) * 64f, -1, (GameLocation) this);
          return;
        }
      }
      base.digUpArtifactSpot(xLocation, yLocation, who);
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
      Random random = new Random((int) bobberTile.X * 2000 + (int) bobberTile.Y * 777 + (int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + (int) Game1.stats.TimesFished);
      return Game1.player.hasOrWillReceiveMail("islandNorthCaveOpened") && random.NextDouble() < 0.1 ? new StardewValley.Object(825, 1) : base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    }

    public override bool performToolAction(Tool t, int tileX, int tileY)
    {
      if (this.sandDuggy.Value != null)
        this.sandDuggy.Value.PerformToolAction(t, tileX, tileY);
      return base.performToolAction(t, tileX, tileY);
    }

    public override List<Vector2> GetAdditionalWalnutBushes() => new List<Vector2>()
    {
      new Vector2(54f, 18f),
      new Vector2(25f, 30f),
      new Vector2(15f, 3f)
    };

    public override void draw(SpriteBatch b)
    {
      if (this.sandDuggy.Value != null)
        this.sandDuggy.Value.Draw(b);
      if (this.farmhouseRestored.Value && this.shippingBinLid != null)
        this.shippingBinLid.draw(b);
      if (this.farmhouseMailbox.Value && Game1.mailbox.Count > 0)
      {
        float num1 = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
        Point point = new Point(81, 40);
        float num2 = (float) ((double) ((point.X + 1) * 64) / 10000.0 + (double) (point.Y * 64) / 10000.0);
        float num3 = -8f;
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (point.X * 64) + num3, (float) (point.Y * 64 - 96 - 48) + num1)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num2 + 1E-06f);
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (point.X * 64 + 32 + 4) + num3, (float) (point.Y * 64 - 64 - 24 - 8) + num1)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(189, 423, 15, 13)), Color.White, 0.0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, num2 + 1E-05f);
      }
      base.draw(b);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      if (this.sandDuggy.Value != null)
        this.sandDuggy.Value.Update(time);
      if (this.farmhouseRestored.Value && this.shippingBinLid != null)
      {
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
      base.UpdateWhenCurrentLocation(time);
    }

    public IslandWest(string map, string name)
      : base(map, name)
    {
      this.sandDuggy.Value = new SandDuggy((GameLocation) this, new Point[4]
      {
        new Point(37, 87),
        new Point(41, 86),
        new Point(45, 86),
        new Point(48, 87)
      });
      this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(72, 37), new Microsoft.Xna.Framework.Rectangle(71, 29, 3, 8), 20, (Action) (() =>
      {
        Game1.addMailForTomorrow("Island_W_Obelisk", true, true);
        this.farmObelisk.Value = true;
      }), (Func<bool>) (() => this.farmObelisk.Value), "Obelisk", "Island_UpgradeHouse_Mailbox"));
      this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(81, 40), new Microsoft.Xna.Framework.Rectangle(80, 39, 3, 2), 5, (Action) (() =>
      {
        Game1.addMailForTomorrow("Island_UpgradeHouse_Mailbox", true, true);
        this.farmhouseMailbox.Value = true;
      }), (Func<bool>) (() => this.farmhouseMailbox.Value), "House_Mailbox", "Island_UpgradeHouse"));
      this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(81, 40), new Microsoft.Xna.Framework.Rectangle(74, 36, 7, 4), 20, (Action) (() =>
      {
        Game1.addMailForTomorrow("Island_UpgradeHouse", true, true);
        this.farmhouseRestored.Value = true;
      }), (Func<bool>) (() => this.farmhouseRestored.Value), "House"));
      this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(72, 10), new Microsoft.Xna.Framework.Rectangle(73, 5, 3, 5), 10, (Action) (() =>
      {
        Game1.addMailForTomorrow("Island_UpgradeParrotPlatform", true, true);
        Game1.netWorldState.Value.ParrotPlatformsUnlocked.Value = true;
      }), (Func<bool>) (() => Game1.netWorldState.Value.ParrotPlatformsUnlocked.Value), "ParrotPlatforms"));
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      switch (action)
      {
        case "FarmObelisk":
          for (int index = 0; index < 12; ++index)
            who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) who.Position.X - 256, (int) who.Position.X + 192), (float) Game1.random.Next((int) who.Position.Y - 256, (int) who.Position.Y + 192)), false, Game1.random.NextDouble() < 0.5));
          who.currentLocation.playSound("wand");
          Game1.displayFarmer = false;
          Game1.player.temporarilyInvincible = true;
          Game1.player.temporaryInvincibilityTimer = -2000;
          Game1.player.freezePause = 1000;
          Game1.flashAlpha = 1f;
          DelayedAction.fadeAfterDelay((Game1.afterFadeFunction) (() =>
          {
            int default_x = 48;
            int default_y = 7;
            switch (Game1.whichFarm)
            {
              case 5:
                default_x = 48;
                default_y = 39;
                break;
              case 6:
                default_x = 82;
                default_y = 29;
                break;
            }
            Point propertyPosition = Game1.getFarm().GetMapPropertyPosition("WarpTotemEntry", default_x, default_y);
            Game1.warpFarmer("Farm", propertyPosition.X, propertyPosition.Y, false);
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.player.temporarilyInvincible = false;
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
          }), 1000);
          new Microsoft.Xna.Framework.Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
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
          return true;
        default:
          return base.performAction(action, who, tileLocation);
      }
    }

    public override bool leftClick(int x, int y, Farmer who)
    {
      if (!this.farmhouseRestored.Value || who.ActiveObject == null || x / 64 < this.shippingBinPosition.X || x / 64 > this.shippingBinPosition.X + 1 || y / 64 < this.shippingBinPosition.Y - 1 || y / 64 > this.shippingBinPosition.Y || !who.ActiveObject.canBeShipped() || (double) Vector2.Distance(who.getTileLocation(), new Vector2((float) this.shippingBinPosition.X + 0.5f, (float) this.shippingBinPosition.Y)) > 2.0)
        return base.leftClick(x, y, who);
      Game1.getFarm().getShippingBin(who).Add((Item) who.ActiveObject);
      Game1.getFarm().lastItemShipped = (Item) who.ActiveObject;
      who.showNotCarrying();
      this.showShipment(who.ActiveObject);
      who.ActiveObject = (StardewValley.Object) null;
      return true;
    }

    public void showShipment(StardewValley.Object o, bool playThrowSound = true)
    {
      if (playThrowSound)
        this.localSound("backpackIN");
      DelayedAction.playSoundAfterDelay("Ship", playThrowSound ? 250 : 0);
      int num = Game1.random.Next();
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(524, 218, 34, 22), new Vector2(90f, 38f) * 64f + new Vector2(0.0f, 5f) * 4f, false, 0.0f, Color.White)
      {
        interval = 100f,
        totalNumberOfLoops = 1,
        animationLength = 3,
        pingPong = true,
        scale = 4f,
        layerDepth = 0.25601f,
        id = (float) num,
        extraInfoForEndBehavior = num,
        endFunction = new TemporaryAnimatedSprite.endBehavior(((GameLocation) this).removeTemporarySpritesWithID)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(524, 230, 34, 10), new Vector2(90f, 38f) * 64f + new Vector2(0.0f, 17f) * 4f, false, 0.0f, Color.White)
      {
        interval = 100f,
        totalNumberOfLoops = 1,
        animationLength = 3,
        pingPong = true,
        scale = 4f,
        layerDepth = 0.2563f,
        id = (float) num,
        extraInfoForEndBehavior = num
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) o.parentSheetIndex, 16, 16), new Vector2(90f, 38f) * 64f + new Vector2((float) (8 + Game1.random.Next(6)), 2f) * 4f, false, 0.0f, Color.White)
      {
        interval = 9999f,
        scale = 4f,
        alphaFade = 0.045f,
        layerDepth = 0.256225f,
        motion = new Vector2(0.0f, 0.3f),
        acceleration = new Vector2(0.0f, 0.2f),
        scaleChange = -0.05f
      });
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.farmhouseRestored.Value && tileLocation.X >= this.shippingBinPosition.X && tileLocation.X <= this.shippingBinPosition.X + 1 && tileLocation.Y >= this.shippingBinPosition.Y - 1 && tileLocation.Y <= this.shippingBinPosition.Y)
      {
        ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), new ItemGrabMenu.behaviorOnItemSelect(Game1.getFarm().shipItem), "", snapToBottom: true, canBeExitedWithKey: true, playRightClickSound: false, context: ((object) this));
        itemGrabMenu.initializeUpperRightCloseButton();
        itemGrabMenu.setBackgroundTransparency(false);
        itemGrabMenu.setDestroyItemOnClick(true);
        itemGrabMenu.initializeShippingBin();
        Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
        this.playSound("shwip");
        if (Game1.player.FacingDirection == 1)
          Game1.player.Halt();
        Game1.player.showCarrying();
        return true;
      }
      if (this.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings") != -1 && this.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings") == 1470)
      {
        int sub1 = Math.Max(0, Game1.netWorldState.Value.GoldenWalnutsFound.Value - 1);
        if (sub1 < 100)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:qiNutDoor", (object) sub1));
        }
        else
        {
          Game1.playSound("doorClose");
          Game1.warpFarmer("QiNutRoom", 7, 8, 0);
        }
        return true;
      }
      if (this.getCharacterFromName("Birdie") != null && !this.getCharacterFromName("Birdie").IsInvisible && (this.getCharacterFromName("Birdie").getTileLocation().Equals(new Vector2((float) tileLocation.X, (float) tileLocation.Y)) || this.getCharacterFromName("Birdie").getTileLocation().Equals(new Vector2((float) (tileLocation.X - 1), (float) tileLocation.Y))))
      {
        if (!who.mailReceived.Contains("birdieQuestBegun"))
        {
          who.Halt();
          Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandSecret_Event_BirdieIntro"), -888999))));
          who.mailReceived.Add("birdieQuestBegun");
          return true;
        }
        if (who.hasQuest(130) && !who.mailReceived.Contains("birdieQuestFinished") && who.ActiveObject != null && Utility.IsNormalObjectAtParentSheetIndex((Item) who.ActiveObject, 870))
        {
          who.Halt();
          Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
          {
            who.reduceActiveItemByOne();
            this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandSecret_Event_BirdieFinished"), -666777));
          }));
          who.mailReceived.Add("birdieQuestFinished");
          return true;
        }
        if (who.mailReceived.Contains("birdieQuestFinished"))
        {
          if (who.ActiveObject != null)
          {
            Game1.drawDialogue(this.getCharacterFromName("Birdie"), Utility.loadStringDataShort("ExtraDialogue", "Birdie_NoGift"));
          }
          else
          {
            string dialogue = (string) null;
            try
            {
              dialogue = Game1.content.LoadStringReturnNullIfNotFound("Data\\ExtraDialogue:Birdie" + Game1.dayOfMonth.ToString());
            }
            catch (Exception ex)
            {
            }
            if (dialogue != null && dialogue.Length > 0)
              Game1.drawDialogue(this.getCharacterFromName("Birdie"), dialogue);
            else
              Game1.drawDialogue(this.getCharacterFromName("Birdie"), Utility.loadStringDataShort("ExtraDialogue", "Birdie" + (Game1.dayOfMonth % 7).ToString()));
          }
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public override bool isActionableTile(int xTile, int yTile, Farmer who)
    {
      if (!Game1.eventUp)
      {
        NPC characterFromName = this.getCharacterFromName("Birdie");
        if (characterFromName != null && !characterFromName.IsInvisible && characterFromName.getTileLocation().Equals(new Vector2((float) (xTile - 1), (float) yTile)) && (!who.mailReceived.Contains("birdieQuestBegun") || who.mailReceived.Contains("birdieQuestFinished")))
        {
          Game1.isSpeechAtCurrentCursorTile = true;
          return true;
        }
      }
      return base.isActionableTile(xTile, yTile, who);
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.addedSlimesToday);
      this.NetFields.AddField((INetSerializable) this.farmhouseRestored);
      this.NetFields.AddField((INetSerializable) this.sandDuggy);
      this.NetFields.AddField((INetSerializable) this.farmhouseMailbox);
      this.NetFields.AddField((INetSerializable) this.farmObelisk);
      this.farmhouseRestored.InterpolationWait = false;
      this.farmhouseRestored.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.ApplyFarmHouseRestore();
      });
      this.farmhouseMailbox.InterpolationWait = false;
      this.farmhouseMailbox.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.ApplyFarmHouseRestore();
      });
      this.farmObelisk.InterpolationWait = false;
      this.farmObelisk.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.ApplyFarmObeliskBuild();
      });
    }

    public void ApplyFarmObeliskBuild()
    {
      if (this.map == null || this._appliedMapOverrides.Contains("Island_W_Obelisk"))
        return;
      this.ApplyMapOverride("Island_W_Obelisk", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(71, 29, 3, 9)));
    }

    public void ApplyFarmHouseRestore()
    {
      if (this.map == null)
        return;
      if (!this._appliedMapOverrides.Contains("Island_House_Restored"))
      {
        this.ApplyMapOverride("Island_House_Restored", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(74, 33, 7, 9)));
        this.ApplyMapOverride("Island_House_Bin", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.shippingBinPosition.X, this.shippingBinPosition.Y - 1, 2, 2)));
        this.ApplyMapOverride("Island_House_Cave", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(95, 30, 3, 4)));
      }
      if (!this.farmhouseMailbox.Value)
        return;
      this.setMapTileIndex(81, 40, 771, "Buildings");
      this.setMapTileIndex(81, 39, 739, "Front");
      this.setTileProperty(81, 40, "Buildings", "Action", "Mailbox");
    }

    public override void seasonUpdate(string season, bool onLoad = false)
    {
    }

    public override void updateSeasonalTileSheets(Map map = null)
    {
    }

    public override void monsterDrop(Monster monster, int x, int y, Farmer who)
    {
      base.monsterDrop(monster, x, y, who);
      if (Game1.MasterPlayer.hasOrWillReceiveMail("tigerSlimeNut"))
        return;
      int num = 0;
      foreach (NPC character in this.characters)
      {
        if (character is GreenSlime && (string) (NetFieldBase<string, NetString>) character.name == "Tiger Slime")
          ++num;
      }
      if (num != 1)
        return;
      Game1.addMailForTomorrow("tigerSlimeNut", true, true);
      Game1.player.team.RequestLimitedNutDrops("TigerSlimeNut", (GameLocation) this, x, y, 1);
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      if (l is IslandWest)
      {
        IslandWest islandWest = l as IslandWest;
        this.farmhouseRestored.Value = (bool) (NetFieldBase<bool, NetBool>) islandWest.farmhouseRestored;
        this.farmhouseMailbox.Value = (bool) (NetFieldBase<bool, NetBool>) islandWest.farmhouseMailbox;
        this.farmObelisk.Value = islandWest.farmObelisk.Value;
        this.sandDuggy.Value.whacked.Value = islandWest.sandDuggy.Value.whacked.Value;
        List<ResourceClump> other = new List<ResourceClump>((IEnumerable<ResourceClump>) islandWest.resourceClumps);
        islandWest.resourceClumps.Clear();
        this.resourceClumps.Set((ICollection<ResourceClump>) other);
      }
      base.TransferDataFromSavedLocation(l);
    }

    public override void spawnObjects()
    {
      base.spawnObjects();
      Microsoft.Xna.Framework.Rectangle r1 = new Microsoft.Xna.Framework.Rectangle(57, 78, 43, 8);
      if (Utility.getNumObjectsOfIndexWithinRectangle(r1, new int[1]
      {
        25
      }, (GameLocation) this) < 10)
      {
        Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r1, Game1.random);
        if (this.isTileLocationTotallyClearAndPlaceable((int) positionInThisRectangle.X, (int) positionInThisRectangle.Y))
          this.objects.Add(positionInThisRectangle, new StardewValley.Object(positionInThisRectangle, 25, "Stone", true, false, false, false)
          {
            MinutesUntilReady = 8,
            Flipped = Game1.random.NextDouble() < 0.5
          });
      }
      Microsoft.Xna.Framework.Rectangle r2 = new Microsoft.Xna.Framework.Rectangle(20, 71, 28, 16);
      if (Utility.getNumObjectsOfIndexWithinRectangle(r2, new int[2]
      {
        393,
        397
      }, (GameLocation) this) >= 5)
        return;
      Vector2 positionInThisRectangle1 = Utility.getRandomPositionInThisRectangle(r2, Game1.random);
      if (!this.isTileLocationTotallyClearAndPlaceable((int) positionInThisRectangle1.X, (int) positionInThisRectangle1.Y))
        return;
      this.objects.Add(positionInThisRectangle1, new StardewValley.Object(positionInThisRectangle1, Game1.random.NextDouble() < 0.1 ? 397 : 393, 1)
      {
        IsSpawnedObject = true,
        CanBeGrabbed = true
      });
    }

    public override string checkForBuriedItem(
      int xLocation,
      int yLocation,
      bool explosion,
      bool detectOnly,
      Farmer who)
    {
      if (xLocation == 18 && yLocation == 42 && who.secretNotesSeen.Contains(1004))
      {
        Game1.player.team.RequestLimitedNutDrops("Island_W_BuriedTreasureNut", (GameLocation) this, xLocation * 64, yLocation * 64, 1);
        if (!Game1.player.hasOrWillReceiveMail("Island_W_BuriedTreasure"))
        {
          Game1.createItemDebris((Item) new StardewValley.Object(877, 1), new Vector2((float) xLocation, (float) yLocation) * 64f, 1);
          Game1.addMailForTomorrow("Island_W_BuriedTreasure", true);
        }
      }
      else if (xLocation == 104 && yLocation == 74 && who.secretNotesSeen.Contains(1006))
      {
        Game1.player.team.RequestLimitedNutDrops("Island_W_BuriedTreasureNut2", (GameLocation) this, xLocation * 64, yLocation * 64, 1);
        if (!Game1.player.hasOrWillReceiveMail("Island_W_BuriedTreasure2"))
        {
          Game1.createItemDebris((Item) new StardewValley.Object(797, 1), new Vector2((float) xLocation, (float) yLocation) * 64f, 1);
          Game1.addMailForTomorrow("Island_W_BuriedTreasure2", true);
        }
      }
      return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      ICollection<Vector2> source = (ICollection<Vector2>) new List<Vector2>((IEnumerable<Vector2>) this.terrainFeatures.Keys);
      for (int index = source.Count - 1; index >= 0; --index)
      {
        if (this.terrainFeatures[source.ElementAt<Vector2>(index)] is HoeDirt && (this.terrainFeatures[source.ElementAt<Vector2>(index)] as HoeDirt).crop == null && Game1.random.NextDouble() <= 0.1)
          this.terrainFeatures.Remove(source.ElementAt<Vector2>(index));
      }
      for (int index = 0; index < this.characters.Count; ++index)
      {
        if (this.characters[index] != null && this.characters[index] is Monster)
        {
          this.characters.RemoveAt(index);
          --index;
        }
      }
      this.addedSlimesToday.Value = false;
      List<Vector2> vector2List = new List<Vector2>();
      foreach (Vector2 key in this.terrainFeatures.Keys)
      {
        if (this.terrainFeatures[key] is HoeDirt && (this.terrainFeatures[key] as HoeDirt).crop != null && (bool) (NetFieldBase<bool, NetBool>) (this.terrainFeatures[key] as HoeDirt).crop.forageCrop)
          vector2List.Add(key);
      }
      foreach (Vector2 key in vector2List)
        this.terrainFeatures.Remove(key);
      List<Microsoft.Xna.Framework.Rectangle> rectangleList = new List<Microsoft.Xna.Framework.Rectangle>();
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(31, 43, 7, 6));
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(37, 62, 6, 5));
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(48, 42, 5, 4));
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(71, 12, 5, 4));
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(50, 59, 1, 1));
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(47, 64, 1, 1));
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(36, 58, 1, 1));
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(56, 48, 1, 1));
      rectangleList.Add(new Microsoft.Xna.Framework.Rectangle(29, 46, 1, 1));
      for (int index = 0; index < 5; ++index)
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
      if (!Game1.MasterPlayer.mailReceived.Contains("Island_Turtle"))
        return;
      this.spawnWeedsAndStones(20, true);
      if (Game1.dayOfMonth % 7 != 1)
        return;
      this.spawnWeedsAndStones(20, true, false);
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (this.farmhouseRestored.Value)
        this.ApplyFarmHouseRestore();
      if (!this.farmObelisk.Value)
        return;
      this.ApplyFarmObeliskBuild();
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.shippingBinLidOpenArea = new Microsoft.Xna.Framework.Rectangle((this.shippingBinPosition.X - 1) * 64, (this.shippingBinPosition.Y - 1) * 64, 256, 192);
      this.shippingBinLid = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(134, 226, 30, 25), new Vector2((float) this.shippingBinPosition.X, (float) (this.shippingBinPosition.Y - 1)) * 64f + new Vector2(2f, -7f) * 4f, false, 0.0f, Color.White)
      {
        holdLastFrame = true,
        destroyable = false,
        interval = 20f,
        animationLength = 13,
        paused = true,
        scale = 4f,
        layerDepth = (float) ((double) ((this.shippingBinPosition.Y + 1) * 64) / 10000.0 + 9.99999974737875E-05),
        pingPong = true,
        pingPongMotion = 0
      };
      if (this.sandDuggy.Value != null)
        this.sandDuggy.Value.ResetForPlayerEntry();
      NPC characterFromName = this.getCharacterFromName("Birdie");
      if (characterFromName != null)
      {
        if (characterFromName.Sprite.SourceRect.Width < 32)
          characterFromName.extendSourceRect(16, 0);
        characterFromName.Sprite.SpriteWidth = 32;
        characterFromName.Sprite.ignoreSourceRectUpdates = false;
        characterFromName.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(8, 1000, 0, false, false),
          new FarmerSprite.AnimationFrame(9, 1000, 0, false, false)
        });
        characterFromName.Sprite.loop = true;
        characterFromName.HideShadow = true;
        characterFromName.IsInvisible = Game1.IsRainingHere((GameLocation) this);
      }
      if (Game1.timeOfDay > 1700)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(23f, 58f) * 64f + new Vector2(-16f, -32f), false, 0.0f, Color.White)
        {
          interval = 50f,
          totalNumberOfLoops = 99999,
          animationLength = 4,
          light = true,
          lightID = 987654,
          id = 987654f,
          lightRadius = 2f,
          scale = 4f,
          layerDepth = 0.37824f
        });
        AmbientLocationSounds.addSound(new Vector2(23f, 58f), 1);
      }
      if (!(Game1.currentSeason == "winter") || Game1.IsRainingHere((GameLocation) this) || !Game1.isDarkOut())
        return;
      this.addMoonlightJellies(100, new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame - 24917), new Microsoft.Xna.Framework.Rectangle(35, 0, 60, 60));
    }

    protected override void resetSharedState()
    {
      base.resetSharedState();
      if ((bool) (NetFieldBase<bool, NetBool>) this.addedSlimesToday)
        return;
      this.addedSlimesToday.Value = true;
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame + 12);
      Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(28, 24, 19, 8);
      for (int index = 5; index > 0; --index)
      {
        Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, random);
        if (this.isTileLocationTotallyClearAndPlaceable(positionInThisRectangle))
        {
          GreenSlime greenSlime = new GreenSlime(positionInThisRectangle * 64f, 0);
          greenSlime.makeTigerSlime();
          this.characters.Add((NPC) greenSlime);
        }
      }
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
  }
}
