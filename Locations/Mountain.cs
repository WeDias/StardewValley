// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Mountain
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Events;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley.Locations
{
  public class Mountain : GameLocation
  {
    public const int daysBeforeLandslide = 31;
    private TemporaryAnimatedSprite minecartSteam;
    private bool bridgeRestored;
    [XmlIgnore]
    public bool treehouseBuilt;
    [XmlIgnore]
    public bool treehouseDoorDirty;
    private readonly NetBool oreBoulderPresent = new NetBool();
    private readonly NetBool railroadAreaBlocked = new NetBool(Game1.stats.DaysPlayed < 31U);
    private readonly NetBool landslide = new NetBool(Game1.stats.DaysPlayed < 5U);
    private Microsoft.Xna.Framework.Rectangle landSlideRect = new Microsoft.Xna.Framework.Rectangle(3200, 256, 192, 320);
    private Microsoft.Xna.Framework.Rectangle railroadBlockRect = new Microsoft.Xna.Framework.Rectangle(512, 0, 256, 320);
    private int oldTime;
    private Microsoft.Xna.Framework.Rectangle boulderSourceRect = new Microsoft.Xna.Framework.Rectangle(439, 1385, 39, 48);
    private Microsoft.Xna.Framework.Rectangle raildroadBlocksourceRect = new Microsoft.Xna.Framework.Rectangle(640, 2176, 64, 80);
    private Microsoft.Xna.Framework.Rectangle landSlideSourceRect = new Microsoft.Xna.Framework.Rectangle(646, 1218, 48, 80);
    private Vector2 boulderPosition = new Vector2(47f, 3f) * 64f - new Vector2(4f, 3f) * 4f;

    public Mountain()
    {
    }

    public Mountain(string map, string name)
      : base(map, name)
    {
      for (int index = 0; index < 10; ++index)
        this.quarryDayUpdate();
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.oreBoulderPresent, (INetSerializable) this.railroadAreaBlocked, (INetSerializable) this.landslide);
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.map.GetLayer("Buildings").Tiles[tileLocation] != null)
      {
        switch (this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex)
        {
          case 958:
          case 1080:
          case 1081:
            if (Game1.player.mount != null)
              return true;
            if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
            {
              if (Game1.player.isRidingHorse() && Game1.player.mount != null)
              {
                Game1.player.mount.checkAction(Game1.player, (GameLocation) this);
                break;
              }
              Response[] answerChoices = new Response[4]
              {
                new Response("Bus", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_BusStop")),
                new Response("Mines", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Mines")),
                new Response("Town", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Town")),
                new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel"))
              };
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_ChooseDestination"), answerChoices, "Minecart");
              break;
            }
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_OutOfOrder"));
            return true;
          case 1136:
            if (!who.mailReceived.Contains("guildMember") && !who.hasQuest(16))
            {
              Game1.drawLetterMessage(Game1.content.LoadString("Strings\\Locations:Mountain_AdventurersGuildNote").Replace('\n', '^'));
              return true;
            }
            break;
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public void ApplyTreehouseIfNecessary()
    {
      if ((Game1.farmEvent == null || !(Game1.farmEvent is WorldChangeEvent) || (int) (NetFieldBase<int, NetInt>) (Game1.farmEvent as WorldChangeEvent).whichEvent != 14) && !Game1.MasterPlayer.mailReceived.Contains("leoMoved") && !Game1.MasterPlayer.mailReceived.Contains("leoMoved%&NL&%") || this.treehouseBuilt)
        return;
      TileSheet tileSheet = this.map.GetTileSheet("untitled tile sheet2");
      this.map.GetLayer("Buildings").Tiles[16, 6] = (Tile) new StaticTile(this.map.GetLayer("Buildings"), tileSheet, BlendMode.Alpha, 197);
      this.map.GetLayer("Buildings").Tiles[16, 7] = (Tile) new StaticTile(this.map.GetLayer("Buildings"), tileSheet, BlendMode.Alpha, 213);
      this.map.GetLayer("Back").Tiles[16, 8] = (Tile) new StaticTile(this.map.GetLayer("Back"), tileSheet, BlendMode.Alpha, 229);
      this.map.GetLayer("Buildings").Tiles[16, 7].Properties["Action"] = new PropertyValue("LockedDoorWarp 3 8 LeoTreeHouse 600 2300");
      this.treehouseBuilt = true;
      if (!Game1.IsMasterGame)
        return;
      this.updateDoors();
      this.treehouseDoorDirty = true;
    }

    private void restoreBridge()
    {
      LocalizedContentManager temporary = Game1.content.CreateTemporary();
      Map map = temporary.Load<Map>("Maps\\Mountain-BridgeFixed");
      int num1 = 92;
      int num2 = 24;
      for (int x = 0; x < map.GetLayer("Back").LayerWidth; ++x)
      {
        for (int y = 0; y < map.GetLayer("Back").LayerHeight; ++y)
        {
          this.map.GetLayer("Back").Tiles[x + num1, y + num2] = map.GetLayer("Back").Tiles[x, y] == null ? (Tile) null : (Tile) new StaticTile(this.map.GetLayer("Back"), this.map.TileSheets[0], BlendMode.Alpha, map.GetLayer("Back").Tiles[x, y].TileIndex);
          this.map.GetLayer("Buildings").Tiles[x + num1, y + num2] = map.GetLayer("Buildings").Tiles[x, y] == null ? (Tile) null : (Tile) new StaticTile(this.map.GetLayer("Buildings"), this.map.TileSheets[0], BlendMode.Alpha, map.GetLayer("Buildings").Tiles[x, y].TileIndex);
          this.map.GetLayer("Front").Tiles[x + num1, y + num2] = map.GetLayer("Front").Tiles[x, y] == null ? (Tile) null : (Tile) new StaticTile(this.map.GetLayer("Front"), this.map.TileSheets[0], BlendMode.Alpha, map.GetLayer("Front").Tiles[x, y].TileIndex);
        }
      }
      this.bridgeRestored = true;
      temporary.Unload();
    }

    protected override void resetSharedState()
    {
      base.resetSharedState();
      this.oreBoulderPresent.Value = !Game1.MasterPlayer.mailReceived.Contains("ccFishTank") || Game1.farmEvent != null;
      if (!this.objects.ContainsKey(new Vector2(29f, 9f)))
      {
        Vector2 vector2 = new Vector2(29f, 9f);
        OverlaidDictionary objects = this.objects;
        Vector2 key = vector2;
        Torch torch = new Torch(vector2, 146, true);
        torch.IsOn = false;
        torch.Fragility = 2;
        objects.Add(key, (Object) torch);
        this.objects[vector2].checkForAction((Farmer) null);
      }
      if (Game1.stats.DaysPlayed >= 5U)
        this.landslide.Value = false;
      if (Game1.stats.DaysPlayed < 31U)
        return;
      this.railroadAreaBlocked.Value = false;
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (force)
      {
        this.treehouseBuilt = false;
        this.bridgeRestored = false;
      }
      if (!this.bridgeRestored && Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccCraftsRoom"))
        this.restoreBridge();
      if (!(Game1.farmEvent is WorldChangeEvent) || (Game1.farmEvent as WorldChangeEvent).whichEvent.Value != 14)
        this.ApplyTreehouseIfNecessary();
      if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
        return;
      this.ApplyMapOverride("Mountain_Shortcuts");
      this.waterTiles[81, 37] = false;
      this.waterTiles[82, 37] = false;
      this.waterTiles[83, 37] = false;
      this.waterTiles[84, 37] = false;
      this.waterTiles[85, 37] = false;
      this.waterTiles[85, 38] = false;
      this.waterTiles[85, 39] = false;
      this.waterTiles[85, 40] = false;
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
        this.minecartSteam = new TemporaryAnimatedSprite(27, new Vector2(8072f, 656f), Color.White)
        {
          totalNumberOfLoops = 999999,
          interval = 60f,
          flipped = true
        };
      this.boulderSourceRect = new Microsoft.Xna.Framework.Rectangle(439 + (Game1.currentSeason.Equals("winter") ? 39 : 0), 1385, 39, 48);
      this.raildroadBlocksourceRect = !Game1.IsSpring ? new Microsoft.Xna.Framework.Rectangle(640, 1453, 64, 80) : new Microsoft.Xna.Framework.Rectangle(640, 2176, 64, 80);
      this.addFrog();
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      this.quarryDayUpdate();
      if (Game1.stats.DaysPlayed >= 31U)
        this.railroadAreaBlocked.Value = false;
      if (Game1.stats.DaysPlayed < 5U)
        return;
      this.landslide.Value = false;
      if (Game1.player.hasOrWillReceiveMail("landslideDone"))
        return;
      Game1.mailbox.Add("landslideDone");
    }

    private void quarryDayUpdate()
    {
      Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(106, 13, 21, 21);
      int num = 5;
      for (int index = 0; index < num; ++index)
      {
        Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, Game1.random);
        if (this.isTileOpenForQuarryStone((int) positionInThisRectangle.X, (int) positionInThisRectangle.Y))
        {
          if (Game1.random.NextDouble() < 0.06)
            this.terrainFeatures.Add(positionInThisRectangle, (TerrainFeature) new Tree(1 + Game1.random.Next(2), 1));
          else if (Game1.random.NextDouble() < 0.02)
          {
            if (Game1.random.NextDouble() < 0.1)
              this.objects.Add(positionInThisRectangle, new Object(positionInThisRectangle, 46, "Stone", true, false, false, false)
              {
                MinutesUntilReady = 12
              });
            else
              this.objects.Add(positionInThisRectangle, new Object(positionInThisRectangle, (Game1.random.Next(7) + 1) * 2, "Stone", true, false, false, false)
              {
                MinutesUntilReady = 5
              });
          }
          else if (Game1.random.NextDouble() < 0.1)
          {
            if (Game1.random.NextDouble() < 0.001)
              this.objects.Add(positionInThisRectangle, new Object(positionInThisRectangle, 765, 1)
              {
                MinutesUntilReady = 16
              });
            else if (Game1.random.NextDouble() < 0.1)
              this.objects.Add(positionInThisRectangle, new Object(positionInThisRectangle, 764, 1)
              {
                MinutesUntilReady = 8
              });
            else if (Game1.random.NextDouble() < 0.33)
              this.objects.Add(positionInThisRectangle, new Object(positionInThisRectangle, 290, 1)
              {
                MinutesUntilReady = 5
              });
            else
              this.objects.Add(positionInThisRectangle, new Object(positionInThisRectangle, 751, 1)
              {
                MinutesUntilReady = 3
              });
          }
          else
          {
            Object @object = new Object(positionInThisRectangle, Game1.random.NextDouble() < 0.25 ? 32 : (Game1.random.NextDouble() < 0.33 ? 38 : (Game1.random.NextDouble() < 0.5 ? 40 : 42)), 1);
            @object.minutesUntilReady.Value = 2;
            @object.Name = "Stone";
            this.objects.Add(positionInThisRectangle, @object);
          }
        }
      }
    }

    private bool isTileOpenForQuarryStone(int tileX, int tileY) => this.doesTileHaveProperty(tileX, tileY, "Diggable", "Back") != null && this.isTileLocationTotallyClearAndPlaceable(new Vector2((float) tileX, (float) tileY));

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      this.minecartSteam = (TemporaryAnimatedSprite) null;
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      if (this.minecartSteam != null)
        this.minecartSteam.update(time);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.landslide || (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds - 400.0) / 1600.0) % 2 == 0 || !Utility.isOnScreen(new Point(this.landSlideRect.X / 64, this.landSlideRect.Y / 64), 128))
        return;
      if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 400.0 < (double) (this.oldTime % 400))
        this.localSound("hammer");
      this.oldTime = (int) time.TotalGameTime.TotalMilliseconds;
    }

    public override Object getFish(
      float millisecondsAfterNibble,
      int bait,
      int waterDepth,
      Farmer who,
      double baitPotency,
      Vector2 bobberTile,
      string locationName = null)
    {
      bool flag = this.IsUsingMagicBait(who);
      float num = 0.0f;
      if (who != null && who.CurrentTool is FishingRod && (who.CurrentTool as FishingRod).getBobberAttachmentIndex() == 856)
        num += 0.1f;
      if (((Game1.player.team.SpecialOrderRuleActive("LEGENDARY_FAMILY") ? 1 : (Game1.isRaining ? 1 : 0)) | (flag ? 1 : 0)) != 0 && who.FishingLevel >= 10 && waterDepth >= 4 && Game1.random.NextDouble() < 0.1 + (double) num)
      {
        if (Game1.player.team.SpecialOrderRuleActive("LEGENDARY_FAMILY"))
          return new Object(900, 1);
        if (!who.fishCaught.ContainsKey(163) && Game1.currentSeason.Equals("spring") | flag)
          return new Object(163, 1);
      }
      return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    }

    public override bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character)
    {
      return (bool) (NetFieldBase<bool, NetBool>) this.landslide && position.Intersects(this.landSlideRect) || (bool) (NetFieldBase<bool, NetBool>) this.railroadAreaBlocked && position.Intersects(this.railroadBlockRect) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
    }

    public override bool isTilePlaceable(Vector2 tile_location, Item item = null)
    {
      Point point = Utility.Vector2ToPoint((tile_location + new Vector2(0.5f, 0.5f)) * 64f);
      return (!(bool) (NetFieldBase<bool, NetBool>) this.landslide || !this.landSlideRect.Contains(point)) && (!(bool) (NetFieldBase<bool, NetBool>) this.railroadAreaBlocked || !this.railroadBlockRect.Contains(point)) && base.isTilePlaceable(tile_location, item);
    }

    public override void draw(SpriteBatch spriteBatch)
    {
      base.draw(spriteBatch);
      if (this.minecartSteam != null)
        this.minecartSteam.draw(spriteBatch);
      if ((bool) (NetFieldBase<bool, NetBool>) this.oreBoulderPresent)
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.boulderPosition), new Microsoft.Xna.Framework.Rectangle?(this.boulderSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
      if ((bool) (NetFieldBase<bool, NetBool>) this.railroadAreaBlocked)
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.railroadBlockRect), new Microsoft.Xna.Framework.Rectangle?(this.raildroadBlocksourceRect), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0193f);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.landslide)
        return;
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.landSlideRect), new Microsoft.Xna.Framework.Rectangle?(this.landSlideSourceRect), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0192f);
      spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(new Vector2((float) (this.landSlideRect.X + 192 - 20), (float) (this.landSlideRect.Y + 192 + 20)) + new Vector2(32f, 24f)), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 0.0224f);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float) (this.landSlideRect.X + 192 - 20), (float) (this.landSlideRect.Y + 128))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(288 + ((int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 1600.0 % 2.0) == 0 ? 0 : (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 400.0 / 100.0) * 19), 1349, 19, 28)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0256f);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float) (this.landSlideRect.X + 256 - 20), (float) (this.landSlideRect.Y + 128))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(335, 1410, 21, 21)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0128f);
    }
  }
}
