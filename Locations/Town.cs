// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Town
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;
using xTile.Tiles;

namespace StardewValley.Locations
{
  public class Town : GameLocation
  {
    private TemporaryAnimatedSprite minecartSteam;
    private bool ccRefurbished;
    private bool ccJoja;
    private bool playerCheckedBoard;
    private bool isShowingDestroyedJoja;
    private bool isShowingUpgradedPamHouse;
    private bool isShowingSpecialOrdersBoard;
    private LocalizedContentManager mapLoader;
    [XmlElement("daysUntilCommunityUpgrade")]
    public readonly NetInt daysUntilCommunityUpgrade = new NetInt(0);
    private NetArray<bool, NetBool> garbageChecked = new NetArray<bool, NetBool>(8);
    private Vector2 clockCenter = new Vector2(3392f, 1056f);
    private Vector2 ccFacadePosition = new Vector2(3044f, 940f);
    private Vector2 ccFacadePositionBottom = new Vector2(3044f, 1140f);
    public static Microsoft.Xna.Framework.Rectangle minuteHandSource = new Microsoft.Xna.Framework.Rectangle(363, 395, 5, 13);
    public static Microsoft.Xna.Framework.Rectangle hourHandSource = new Microsoft.Xna.Framework.Rectangle(369, 399, 5, 9);
    public static Microsoft.Xna.Framework.Rectangle clockNub = new Microsoft.Xna.Framework.Rectangle(375, 404, 4, 4);
    public static Microsoft.Xna.Framework.Rectangle jojaFacadeTop = new Microsoft.Xna.Framework.Rectangle(424, 1275, 174, 50);
    public static Microsoft.Xna.Framework.Rectangle jojaFacadeBottom = new Microsoft.Xna.Framework.Rectangle(424, 1325, 174, 51);
    public static Microsoft.Xna.Framework.Rectangle jojaFacadeWinterOverlay = new Microsoft.Xna.Framework.Rectangle(66, 1678, 174, 25);

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.daysUntilCommunityUpgrade, (INetSerializable) this.garbageChecked);
    }

    public Town()
    {
    }

    public Town(string map, string name)
      : base(map, name)
    {
    }

    protected override LocalizedContentManager getMapLoader()
    {
      if (this.mapLoader == null)
        this.mapLoader = Game1.game1.xTileContent.CreateTemporary();
      return this.mapLoader;
    }

    public override void UpdateMapSeats()
    {
      base.UpdateMapSeats();
      if (!Game1.IsMasterGame)
        return;
      for (int index = this.mapSeats.Count - 1; index >= 0; --index)
      {
        if ((double) this.mapSeats[index].tilePosition.Value.X == 24.0 && (double) this.mapSeats[index].tilePosition.Value.Y == 13.0 && this.mapSeats[index].seatType.Value == "swings")
          this.mapSeats.RemoveAt(index);
      }
    }

    public override void performTenMinuteUpdate(int timeOfDay)
    {
      base.performTenMinuteUpdate(timeOfDay);
      if (!Game1.isStartingToGetDarkOut())
        this.addClintMachineGraphics();
      else
        AmbientLocationSounds.removeSound(new Vector2(100f, 79f));
    }

    public void checkedBoard() => this.playerCheckedBoard = true;

    private void addClintMachineGraphics()
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(302, 1946, 15, 16), (float) (7000 - Game1.gameTimeInterval), 1, 1, new Vector2(100f, 79f) * 64f + new Vector2(9f, 6f) * 4f, false, false, 0.5188f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        shakeIntensity = 1f
      });
      for (int index = 0; index < 10; ++index)
        Utility.addSmokePuff((GameLocation) this, new Vector2(101f, 78f) * 64f + new Vector2(4f, 4f) * 4f, index * ((7000 - Game1.gameTimeInterval) / 16));
      Vector2 vector2 = new Vector2(643f, 1305f);
      if (Game1.currentSeason.Equals("fall"))
        vector2 = new Vector2(304f, 256f);
      for (int index1 = 0; index1 < Game1.random.Next(1, 4); ++index1)
      {
        for (int index2 = 0; index2 < 16; ++index2)
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle((int) vector2.X, (int) vector2.Y, 5, 18), 50f, 4, 1, new Vector2(100f, 78f) * 64f + new Vector2((float) (-5 - index2 * 4), 0.0f) * 4f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            delayBeforeAnimationStart = index1 * 1500 + 100 * index2
          });
        Utility.addSmokePuff((GameLocation) this, new Vector2(100f, 78f) * 64f + new Vector2(-70f, -6f) * 4f, index1 * 1500 + 1600);
      }
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      for (int index = 0; index < this.garbageChecked.Length; ++index)
        this.garbageChecked[index] = false;
      if (Game1.dayOfMonth == 2 && Game1.currentSeason.Equals("spring") && !Game1.MasterPlayer.mailReceived.Contains("JojaMember") && !this.isTileOccupiedForPlacement(new Vector2(57f, 16f)))
        this.objects.Add(new Vector2(57f, 16f), new StardewValley.Object(Vector2.Zero, 55));
      if (this.daysUntilCommunityUpgrade.Value <= 0)
        return;
      --this.daysUntilCommunityUpgrade.Value;
      if (this.daysUntilCommunityUpgrade.Value > 0)
        return;
      if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
      {
        Game1.MasterPlayer.mailReceived.Add("pamHouseUpgrade");
        Game1.player.changeFriendship(1000, Game1.getCharacterFromName("Pam"));
      }
      else
        Game1.MasterPlayer.mailReceived.Add("communityUpgradeShortcuts");
    }

    public override string checkForBuriedItem(
      int xLocation,
      int yLocation,
      bool explosion,
      bool detectOnly,
      Farmer who)
    {
      if (!who.secretNotesSeen.Contains(17) || xLocation != 98 || yLocation != 4 || who.mailReceived.Contains("SecretNote17_done"))
        return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
      who.mailReceived.Add("SecretNote17_done");
      Game1.createObjectDebris(126, xLocation, yLocation, who.UniqueMultiplayerID, (GameLocation) this);
      return "";
    }

    public override bool CanPlantTreesHere(int sapling_index, int tile_x, int tile_y) => StardewValley.Object.isWildTreeSeed(sapling_index) && this.doesTileHavePropertyNoNull(tile_x, tile_y, "Type", "Back") == "Dirt";

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.map.GetLayer("Buildings").Tiles[tileLocation] != null && who.mount == null)
      {
        switch (this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex)
        {
          case 78:
            string str = this.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
            int index1 = str != null ? Convert.ToInt32(str.Split(' ')[1]) : -1;
            if (index1 >= 0 && index1 < this.garbageChecked.Length)
            {
              if (!this.garbageChecked[index1])
              {
                this.garbageChecked[index1] = true;
                Random random = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + 777 + index1 * 77);
                int num1 = random.Next(0, 100);
                for (int index2 = 0; index2 < num1; ++index2)
                  random.NextDouble();
                int num2 = random.Next(0, 100);
                for (int index3 = 0; index3 < num2; ++index3)
                  random.NextDouble();
                Game1.stats.incrementStat("trashCansChecked", 1);
                int num3 = Utility.getSeasonNumber(Game1.currentSeason) * 17;
                bool flag1 = Game1.stats.getStat("trashCansChecked") > 20U && random.NextDouble() < 0.01;
                bool flag2 = Game1.stats.getStat("trashCansChecked") > 20U && random.NextDouble() < 0.002;
                if (flag2)
                  this.playSound("explosion");
                else if (flag1)
                  this.playSound("crit");
                List<TemporaryAnimatedSprite> temporaryAnimatedSpriteList = new List<TemporaryAnimatedSprite>();
                temporaryAnimatedSpriteList.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(22 + num3, 0, 16, 10), new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f + new Vector2(0.0f, -6f) * 4f, false, 0.0f, Color.White)
                {
                  interval = flag2 ? 4000f : 1000f,
                  motion = flag2 ? new Vector2(4f, -20f) : new Vector2(0.0f, (float) ((flag1 ? -7.0 : (double) (Game1.random.Next(-1, 3) + (Game1.random.NextDouble() < 0.1 ? -2 : 0))) - 8.0)),
                  rotationChange = flag2 ? 0.4f : 0.0f,
                  acceleration = new Vector2(0.0f, 0.7f),
                  yStopCoordinate = tileLocation.Y * 64 - 24,
                  layerDepth = flag2 ? 1f : (float) ((tileLocation.Y + 1) * 64 + 2) / 10000f,
                  scale = 4f,
                  Parent = (GameLocation) this,
                  shakeIntensity = flag2 ? 0.0f : 1f,
                  reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x =>
                  {
                    this.removeTemporarySpritesWithID(97654);
                    this.playSound("thudStep");
                    for (int index4 = 0; index4 < 3; ++index4)
                      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f + new Vector2((float) (index4 * 6), (float) (Game1.random.Next(3) - 3)) * 4f, false, 0.02f, Color.DimGray)
                      {
                        alpha = 0.85f,
                        motion = new Vector2((float) ((double) index4 * 0.300000011920929 - 0.600000023841858), -1f),
                        acceleration = new Vector2(1f / 500f, 0.0f),
                        interval = 99999f,
                        layerDepth = (float) ((tileLocation.Y + 1) * 64 + 3) / 10000f,
                        scale = 3f,
                        scaleChange = 0.02f,
                        rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
                        delayBeforeAnimationStart = 50
                      });
                  }),
                  id = 97654f
                });
                if (flag2)
                  temporaryAnimatedSpriteList.Last<TemporaryAnimatedSprite>().reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSpriteList.Last<TemporaryAnimatedSprite>().bounce);
                temporaryAnimatedSpriteList.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(22 + num3, 11, 16, 16), new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f + new Vector2(0.0f, -5f) * 4f, false, 0.0f, Color.White)
                {
                  interval = flag2 ? 999999f : 1000f,
                  layerDepth = (float) ((tileLocation.Y + 1) * 64 + 1) / 10000f,
                  scale = 4f,
                  id = 97654f
                });
                for (int index5 = 0; index5 < 5; ++index5)
                  temporaryAnimatedSpriteList.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(22 + Game1.random.Next(4) * 4, 32, 4, 4), new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f + new Vector2((float) Game1.random.Next(13), (float) (Game1.random.Next(3) - 3)) * 4f, false, 0.0f, Color.White)
                  {
                    interval = 500f,
                    motion = new Vector2((float) Game1.random.Next(-2, 3), -5f),
                    acceleration = new Vector2(0.0f, 0.4f),
                    layerDepth = (float) ((tileLocation.Y + 1) * 64 + 3) / 10000f,
                    scale = 4f,
                    color = Utility.getRandomRainbowColor(),
                    delayBeforeAnimationStart = Game1.random.Next(100)
                  });
                Game1.multiplayer.broadcastSprites((GameLocation) this, temporaryAnimatedSpriteList);
                this.playSound("trashcan");
                Character character = Utility.isThereAFarmerOrCharacterWithinDistance(new Vector2((float) tileLocation.X, (float) tileLocation.Y), 7, (GameLocation) this);
                if (character != null && character is NPC && !(character is Horse))
                {
                  Game1.multiplayer.globalChatInfoMessage("TrashCan", Game1.player.Name, (string) (NetFieldBase<string, NetString>) character.name);
                  if (character.name.Equals((object) "Linus"))
                  {
                    character.doEmote(32);
                    (character as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Linus"), true, true);
                    who.changeFriendship(5, character as NPC);
                    Game1.multiplayer.globalChatInfoMessage("LinusTrashCan");
                  }
                  else if ((character as NPC).Age == 2)
                  {
                    character.doEmote(28);
                    (character as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Child"), true, true);
                    who.changeFriendship(-25, character as NPC);
                  }
                  else if ((character as NPC).Age == 1)
                  {
                    character.doEmote(8);
                    (character as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Teen"), true, true);
                    who.changeFriendship(-25, character as NPC);
                  }
                  else
                  {
                    character.doEmote(12);
                    (character as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Adult"), true, true);
                    who.changeFriendship(-25, character as NPC);
                  }
                  Game1.drawDialogue(character as NPC);
                }
                if (flag2)
                  who.addItemByMenuIfNecessary((Item) new Hat(66));
                else if (flag1 || random.NextDouble() < 0.2 + who.DailyLuck)
                {
                  int parentSheetIndex = 168;
                  switch (random.Next(10))
                  {
                    case 0:
                      parentSheetIndex = 168;
                      break;
                    case 1:
                      parentSheetIndex = 167;
                      break;
                    case 2:
                      parentSheetIndex = 170;
                      break;
                    case 3:
                      parentSheetIndex = 171;
                      break;
                    case 4:
                      parentSheetIndex = 172;
                      break;
                    case 5:
                      parentSheetIndex = 216;
                      break;
                    case 6:
                      parentSheetIndex = Utility.getRandomItemFromSeason(Game1.currentSeason, tileLocation.X * 653 + tileLocation.Y * 777, false);
                      break;
                    case 7:
                      parentSheetIndex = 403;
                      break;
                    case 8:
                      parentSheetIndex = 309 + random.Next(3);
                      break;
                    case 9:
                      parentSheetIndex = 153;
                      break;
                  }
                  if (index1 == 3 && random.NextDouble() < 0.2 + who.DailyLuck)
                  {
                    parentSheetIndex = 535;
                    if (random.NextDouble() < 0.05)
                      parentSheetIndex = 749;
                  }
                  if (index1 == 4 && random.NextDouble() < 0.2 + who.DailyLuck)
                  {
                    parentSheetIndex = 378 + random.Next(3) * 2;
                    random.Next(1, 5);
                  }
                  if (index1 == 5 && random.NextDouble() < 0.2 + who.DailyLuck && Game1.dishOfTheDay != null)
                    parentSheetIndex = (int) (NetFieldBase<int, NetInt>) Game1.dishOfTheDay.parentSheetIndex != 217 ? (int) (NetFieldBase<int, NetInt>) Game1.dishOfTheDay.parentSheetIndex : 216;
                  if (index1 == 6 && random.NextDouble() < 0.2 + who.DailyLuck)
                    parentSheetIndex = 223;
                  if (index1 == 7 && random.NextDouble() < 0.2)
                  {
                    if (!Utility.HasAnyPlayerSeenEvent(191393))
                      parentSheetIndex = 167;
                    if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater") && !Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja"))
                      parentSheetIndex = random.NextDouble() >= 0.25 ? 270 : 809;
                  }
                  if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                    parentSheetIndex = 890;
                  Vector2 origin = new Vector2((float) tileLocation.X + 0.5f, (float) (tileLocation.Y - 1)) * 64f;
                  Game1.createItemDebris((Item) new StardewValley.Object(parentSheetIndex, 1), origin, 2, (GameLocation) this, (int) origin.Y + 64);
                }
              }
              Game1.haltAfterCheck = false;
              return true;
            }
            break;
          case 599:
            if (Game1.player.secretNotesSeen.Contains(19) && !Game1.player.mailReceived.Contains("SecretNote19_done"))
            {
              DelayedAction.playSoundAfterDelay("newArtifact", 250);
              Game1.player.mailReceived.Add("SecretNote19_done");
              Game1.player.addItemByMenuIfNecessary((Item) new StardewValley.Object(Vector2.Zero, 164));
              break;
            }
            break;
          case 620:
            if (Utility.HasAnyPlayerSeenEvent(191393))
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_SeedShopSign").Replace('\n', '^'));
            else
              Game1.drawObjectDialogue(((IEnumerable<string>) Game1.content.LoadString("Strings\\Locations:Town_SeedShopSign").Split('\n')).First<string>() + "^" + Game1.content.LoadString("Strings\\Locations:SeedShop_LockedWed"));
            return true;
          case 958:
          case 1080:
          case 1081:
            if (Game1.player.mount != null || this.currentEvent != null && this.currentEvent.isFestival && this.currentEvent.checkAction(tileLocation, viewport, who))
              return true;
            if (Game1.player.getTileX() <= 70)
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_PickupTruck"));
              return true;
            }
            if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
            {
              if (Game1.player.isRidingHorse() && Game1.player.mount != null)
              {
                Game1.player.mount.checkAction(Game1.player, (GameLocation) this);
                break;
              }
              Response[] answerChoices;
              if (Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom"))
                answerChoices = new Response[4]
                {
                  new Response("Mines", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Mines")),
                  new Response("Bus", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_BusStop")),
                  new Response("Quarry", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Quarry")),
                  new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel"))
                };
              else
                answerChoices = new Response[3]
                {
                  new Response("Mines", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Mines")),
                  new Response("Bus", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_BusStop")),
                  new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel"))
                };
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_ChooseDestination"), answerChoices, "Minecart");
              break;
            }
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_OutOfOrder"));
            return true;
          case 1913:
          case 1914:
          case 1945:
          case 1946:
            if (this.isShowingDestroyedJoja)
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_JojaSign_Destroyed"));
              return true;
            }
            break;
          case 1935:
          case 2270:
            if (Game1.player.secretNotesSeen.Contains(20) && !Game1.player.mailReceived.Contains("SecretNote20_done"))
            {
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Town_SpecialCharmQuestion"), this.createYesNoResponses(), "specialCharmQuestion");
              break;
            }
            break;
          case 2000:
          case 2001:
          case 2032:
          case 2033:
            if (this.isShowingDestroyedJoja)
            {
              Rumble.rumble(0.15f, 200f);
              Game1.player.completelyStopAnimatingOrDoingAction();
              this.playSoundAt("stairsdown", Game1.player.getTileLocation());
              Game1.warpFarmer("AbandonedJojaMart", 9, 13, false);
              return true;
            }
            break;
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public void crackOpenAbandonedJojaMartDoor()
    {
      this.setMapTileIndex(95, 49, 2000, "Buildings");
      this.setMapTileIndex(96, 49, 2001, "Buildings");
      this.setMapTileIndex(95, 50, 2032, "Buildings");
      this.setMapTileIndex(96, 50, 2033, "Buildings");
    }

    private void refurbishCommunityCenter()
    {
      if (this.ccRefurbished)
        return;
      this.ccRefurbished = true;
      if (Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
        this.ccJoja = true;
      if (this._appliedMapOverrides != null)
      {
        if (this._appliedMapOverrides.Contains("ccRefurbished"))
          return;
        this._appliedMapOverrides.Add("ccRefurbished");
      }
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(47, 11, 11, 9);
      for (int x = rectangle.X; x <= rectangle.Right; ++x)
      {
        for (int y = rectangle.Y; y <= rectangle.Bottom; ++y)
        {
          if (this.map.GetLayer("Back").Tiles[x, y] != null && this.map.GetLayer("Back").Tiles[x, y].TileSheet.Id.Equals(nameof (Town)) && this.map.GetLayer("Back").Tiles[x, y].TileIndex > 1200)
            this.map.GetLayer("Back").Tiles[x, y].TileIndex += 12;
          if (this.map.GetLayer("Buildings").Tiles[x, y] != null && this.map.GetLayer("Buildings").Tiles[x, y].TileSheet.Id.Equals(nameof (Town)) && this.map.GetLayer("Buildings").Tiles[x, y].TileIndex > 1200)
            this.map.GetLayer("Buildings").Tiles[x, y].TileIndex += 12;
          if (this.map.GetLayer("Front").Tiles[x, y] != null && this.map.GetLayer("Front").Tiles[x, y].TileSheet.Id.Equals(nameof (Town)) && this.map.GetLayer("Front").Tiles[x, y].TileIndex > 1200)
            this.map.GetLayer("Front").Tiles[x, y].TileIndex += 12;
          if (this.map.GetLayer("AlwaysFront").Tiles[x, y] != null && this.map.GetLayer("AlwaysFront").Tiles[x, y].TileSheet.Id.Equals(nameof (Town)) && this.map.GetLayer("AlwaysFront").Tiles[x, y].TileIndex > 1200)
            this.map.GetLayer("AlwaysFront").Tiles[x, y].TileIndex += 12;
        }
      }
    }

    private void showDestroyedJoja()
    {
      if (this.isShowingDestroyedJoja)
        return;
      this.isShowingDestroyedJoja = true;
      if (this._appliedMapOverrides != null && this._appliedMapOverrides.Contains("isShowingDestroyedJoja"))
        return;
      this._appliedMapOverrides.Add("isShowingDestroyedJoja");
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(90, 42, 11, 9);
      for (int x = rectangle.X; x <= rectangle.Right; ++x)
      {
        for (int y = rectangle.Y; y <= rectangle.Bottom; ++y)
        {
          bool flag = false;
          if (x > rectangle.X + 6 || y < rectangle.Y + 9)
            flag = true;
          if (flag && this.map.GetLayer("Back").Tiles[x, y] != null && this.map.GetLayer("Back").Tiles[x, y].TileSheet.Id.Equals(nameof (Town)) && this.map.GetLayer("Back").Tiles[x, y].TileIndex > 1200)
            this.map.GetLayer("Back").Tiles[x, y].TileIndex += 20;
          if (flag && this.map.GetLayer("Buildings").Tiles[x, y] != null && this.map.GetLayer("Buildings").Tiles[x, y].TileSheet.Id.Equals(nameof (Town)) && this.map.GetLayer("Buildings").Tiles[x, y].TileIndex > 1200)
            this.map.GetLayer("Buildings").Tiles[x, y].TileIndex += 20;
          if (flag && (x != 93 && y != 50 || x != 94 && y != 50) && this.map.GetLayer("Front").Tiles[x, y] != null && this.map.GetLayer("Front").Tiles[x, y].TileSheet.Id.Equals(nameof (Town)) && this.map.GetLayer("Front").Tiles[x, y].TileIndex > 1200)
            this.map.GetLayer("Front").Tiles[x, y].TileIndex += 20;
          if (flag && this.map.GetLayer("AlwaysFront").Tiles[x, y] != null && this.map.GetLayer("AlwaysFront").Tiles[x, y].TileSheet.Id.Equals(nameof (Town)) && this.map.GetLayer("AlwaysFront").Tiles[x, y].TileIndex > 1200)
            this.map.GetLayer("AlwaysFront").Tiles[x, y].TileIndex += 20;
        }
      }
    }

    public override bool isTileFishable(int tileX, int tileY) => this.GetSeasonForLocation() != "winter" && tileY == 26 && (tileX == 25 || tileX == 26 || tileX == 27) || tileX == 25 && tileY == 25 || tileX == 27 && tileY == 25 || base.isTileFishable(tileX, tileY);

    public void showImprovedPamHouse()
    {
      if (this.isShowingUpgradedPamHouse)
        return;
      this.isShowingUpgradedPamHouse = true;
      if (this._appliedMapOverrides != null)
      {
        if (this._appliedMapOverrides.Contains("isShowingUpgradedPamHouse"))
          return;
        this._appliedMapOverrides.Add("isShowingUpgradedPamHouse");
      }
      Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(69, 66, 8, 3);
      Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(69, 60, 8, 6);
      for (int x = rectangle1.X; x < rectangle1.Right; ++x)
      {
        for (int y = rectangle1.Y; y < rectangle1.Bottom; ++y)
        {
          if (this.map.GetLayer("Buildings").Tiles[x, y] != null)
          {
            this.map.GetLayer("Buildings").Tiles[x, y].TileIndex += 842;
            if (this.map.GetLayer("Buildings").Tiles[x, y].TileIndex == 1568)
              this.map.GetLayer("Buildings").Tiles[x, y].TileIndex = 1562;
          }
          if (this.map.GetLayer("Front").Tiles[x, y] != null && y < rectangle1.Bottom - 1)
            this.map.GetLayer("Front").Tiles[x, y].TileIndex += 842;
        }
      }
      for (int x = rectangle2.X; x < rectangle2.Right; ++x)
      {
        for (int y = rectangle2.Y; y < rectangle2.Bottom; ++y)
        {
          if (this.map.GetLayer("AlwaysFront").Tiles[x, y] == null)
            this.map.GetLayer("AlwaysFront").Tiles[x, y] = (Tile) new StaticTile(this.map.GetLayer("AlwaysFront"), this.map.GetTileSheet(nameof (Town)), BlendMode.Alpha, 1336 + (x - rectangle2.X) + (y - rectangle2.Y) * 32);
        }
      }
      if (Game1.eventUp)
        return;
      this.removeTile(63, 68, "Buildings");
      this.removeTile(62, 72, "Buildings");
      this.removeTile(74, 71, "Buildings");
    }

    public static Point GetTheaterTileOffset() => Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja") ? new Point(-43, -31) : new Point(0, 0);

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (force)
      {
        this.isShowingSpecialOrdersBoard = false;
        this.isShowingUpgradedPamHouse = false;
        this.isShowingDestroyedJoja = false;
        this.ccRefurbished = false;
      }
      if (Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter())
        this.refurbishCommunityCenter();
      if (!this.isShowingSpecialOrdersBoard && SpecialOrder.IsSpecialOrdersBoardUnlocked())
      {
        this.isShowingSpecialOrdersBoard = true;
        LargeTerrainFeature terrainFeatureAt;
        do
        {
          terrainFeatureAt = this.getLargeTerrainFeatureAt(61, 93);
          if (terrainFeatureAt != null)
            this.largeTerrainFeatures.Remove(terrainFeatureAt);
        }
        while (terrainFeatureAt != null);
        int whichTileSheet = 2;
        TileSheet tileSheet = this.map.GetTileSheet(nameof (Town));
        if (tileSheet != null)
          whichTileSheet = this.map.TileSheets.IndexOf(tileSheet);
        this.setMapTileIndex(61, 93, 2045, "Buildings", whichTileSheet);
        this.setMapTileIndex(62, 93, 2046, "Buildings", whichTileSheet);
        this.setMapTileIndex(63, 93, 2047, "Buildings", whichTileSheet);
        this.setTileProperty(61, 93, "Buildings", "Action", "SpecialOrders");
        this.setTileProperty(62, 93, "Buildings", "Action", "SpecialOrders");
        this.setTileProperty(63, 93, "Buildings", "Action", "SpecialOrders");
        this.setMapTileIndex(61, 92, 2013, "Front", whichTileSheet);
        this.setMapTileIndex(62, 92, 2014, "Front", whichTileSheet);
        this.setMapTileIndex(63, 92, 2015, "Front", whichTileSheet);
      }
      if (NetWorldState.checkAnywhereForWorldStateID("trashBearDone") && (this.currentEvent == null || this.currentEvent.id != 777111))
      {
        if (!Game1.eventUp || this.mapPath.Value == null || !this.mapPath.Value.EndsWith("Town-Fair", StringComparison.Ordinal))
          this.ApplyMapOverride("Town-TrashGone", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(57, 68, 17, 5)));
        this.ApplyMapOverride("Town-DogHouse", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(51, 65, 5, 6)));
      }
      if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
      {
        if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja"))
        {
          Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(46, 10, 15, 16);
          this.ApplyMapOverride("Town-TheaterCC", new Microsoft.Xna.Framework.Rectangle?(rectangle), new Microsoft.Xna.Framework.Rectangle?(rectangle));
        }
        else
        {
          Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(84, 41, 27, 15);
          this.ApplyMapOverride("Town-Theater", new Microsoft.Xna.Framework.Rectangle?(rectangle), new Microsoft.Xna.Framework.Rectangle?(rectangle));
        }
      }
      else if (Utility.HasAnyPlayerSeenEvent(191393))
      {
        this.showDestroyedJoja();
        if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("abandonedJojaMartAccessible"))
          this.crackOpenAbandonedJojaMartDoor();
      }
      if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        this.showImprovedPamHouse();
      if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
        return;
      this.showTownCommunityUpgradeShortcuts();
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccBoilerRoom"))
        this.minecartSteam = new TemporaryAnimatedSprite(27, new Vector2(6856f, 5008f), Color.White)
        {
          totalNumberOfLoops = 999999,
          interval = 60f,
          flipped = true
        };
      if (NetWorldState.checkAnywhereForWorldStateID("trashBearDone") && (this.currentEvent == null || this.currentEvent.id != 777111) && !Game1.isRaining && new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed).NextDouble() < 0.2)
        this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(348, 1916, 12, 20), 999f, 1, 999999, new Vector2(53f, 67f) * 64f + new Vector2(3f, 2f) * 4f, false, false, 0.98f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          id = 1f
        });
      if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
      {
        if ((long) Game1.player.team.theaterBuildDate < 0L)
          Game1.player.team.theaterBuildDate.Value = (long) Game1.Date.TotalDays;
        Point theaterTileOffset = Town.GetTheaterTileOffset();
        MovieTheater.AddMoviePoster((GameLocation) this, (float) ((91 + theaterTileOffset.X) * 64 + 32), (float) ((48 + theaterTileOffset.Y) * 64 + 64));
        MovieTheater.AddMoviePoster((GameLocation) this, (float) ((93 + theaterTileOffset.X) * 64 + 24), (float) ((48 + theaterTileOffset.Y) * 64 + 64), 1);
        Vector2 vector2 = new Vector2((float) theaterTileOffset.X, (float) theaterTileOffset.Y);
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(91f, 46f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(96f, 47f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(100f, 47f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(96f, 45f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(100f, 45f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(97f, 43f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(99f, 43f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(98f, 49f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(92f, 49f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(94f, 49f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(98f, 51f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(92f, 51f) + vector2) * 64f, 1f));
        Game1.currentLightSources.Add(new LightSource(4, (new Vector2(94f, 51f) + vector2) * 64f, 1f));
      }
      if (!Game1.currentSeason.Equals("winter"))
      {
        AmbientLocationSounds.addSound(new Vector2(26f, 26f), 0);
        AmbientLocationSounds.addSound(new Vector2(26f, 28f), 0);
      }
      if (!Game1.isStartingToGetDarkOut())
      {
        AmbientLocationSounds.addSound(new Vector2(100f, 79f), 2);
        this.addClintMachineGraphics();
      }
      if (Game1.player.mailReceived.Contains("checkedBulletinOnce"))
        this.playerCheckedBoard = true;
      if (Game1.player.eventsSeen.Contains(520702) && !Game1.player.hasMagnifyingGlass && Game1.currentSeason.Equals("winter"))
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(14.5f, 52.75f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(13.5f, 53f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(15.5f, 53f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(16f, 52.25f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(17f, 52f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(17f, 51f) * 64f + new Vector2(8f, 0.0f) * 4f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(18f, 51f) * 64f + new Vector2(5f, -7f) * 4f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(18f, 50f) * 64f + new Vector2(12f, -2f) * 4f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(21.75f, 39.5f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(21f, 39f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(21.75f, 38.25f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(22.5f, 37.5f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(21.75f, 36.75f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(23f, 36f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(22.25f, 35.25f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(23.5f, 34.6f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(23.5f, 33.6f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(24.25f, 32.6f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(26.75f, 26.75f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(27.5f, 26f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(30f, 23f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(31f, 22f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(30.5f, 21f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(31f, 20f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(30f, 19f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(29f, 18f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(29.1f, 17f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(30f, 17.7f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(31.5f, 18.2f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(30.5f, 16.8f) * 64f, false, false, 1E-06f, 0.0f, Color.White, 3f + (float) Game1.random.NextDouble(), 0.0f, 0.0f, 0.0f));
      }
      if (!Game1.MasterPlayer.mailReceived.Contains("Capsule_Broken") || !Game1.isDarkOut() || Game1.random.NextDouble() >= 0.01)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Microsoft.Xna.Framework.Rectangle(448, 546, 16, 25), new Vector2(3f, 59f) * 64f, false, 0.0f, Color.White)
      {
        scale = 4f,
        motion = new Vector2(3f, 0.0f),
        animationLength = 4,
        interval = 80f,
        totalNumberOfLoops = 200,
        layerDepth = 0.384f,
        xStopCoordinate = 384
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Microsoft.Xna.Framework.Rectangle(448, 546, 16, 25), new Vector2(58f, 108f) * 64f, false, 0.0f, Color.White)
      {
        scale = 4f,
        motion = new Vector2(3f, 0.0f),
        animationLength = 4,
        interval = 80f,
        totalNumberOfLoops = 200,
        layerDepth = 0.384f,
        xStopCoordinate = 4800
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Microsoft.Xna.Framework.Rectangle(448, 546, 16, 25), new Vector2(20f, 92.5f) * 64f, false, 0.0f, Color.White)
      {
        scale = 4f,
        motion = new Vector2(3f, 0.0f),
        animationLength = 4,
        interval = 80f,
        totalNumberOfLoops = 200,
        layerDepth = 0.384f,
        xStopCoordinate = 1664,
        delayBeforeAnimationStart = 1000
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Microsoft.Xna.Framework.Rectangle(448, 546, 16, 25), new Vector2(75f, 1f) * 64f, true, 0.0f, Color.White)
      {
        scale = 4f,
        motion = new Vector2(-4f, 0.0f),
        animationLength = 4,
        interval = 60f,
        totalNumberOfLoops = 200,
        layerDepth = 0.0064f,
        xStopCoordinate = 4352
      });
    }

    private void showTownCommunityUpgradeShortcuts()
    {
      this.removeTile(90, 2, "Buildings");
      this.removeTile(90, 1, "Front");
      this.removeTile(90, 1, "Buildings");
      this.removeTile(90, 0, "Buildings");
      this.setMapTileIndex(89, 1, 360, "Front");
      this.setMapTileIndex(89, 2, 385, "Buildings");
      this.setMapTileIndex(89, 1, 436, "Buildings");
      this.setMapTileIndex(89, 0, 411, "Buildings");
      this.removeTile(98, 3, "Buildings");
      this.removeTile(98, 2, "Buildings");
      this.removeTile(98, 1, "Buildings");
      this.removeTile(98, 0, "Buildings");
      this.setMapTileIndex(98, 3, 588, "Back");
      this.setMapTileIndex(98, 2, 588, "Back");
      this.setMapTileIndex(98, 1, 588, "Back");
      this.setMapTileIndex(98, 0, 588, "Back");
      this.setMapTileIndex(99, 3, 416, "Buildings");
      this.setMapTileIndex(99, 2, 391, "Buildings");
      this.setMapTileIndex(99, 1, 416, "Buildings");
      this.setMapTileIndex(99, 0, 391, "Buildings");
      this.removeTile(92, 104, "Buildings");
      this.removeTile(93, 104, "Buildings");
      this.removeTile(94, 104, "Buildings");
      this.removeTile(92, 105, "Buildings");
      this.removeTile(93, 105, "Buildings");
      this.removeTile(94, 105, "Buildings");
      this.removeTile(93, 106, "Buildings");
      this.removeTile(94, 106, "Buildings");
      this.removeTile(92, 103, "Front");
      this.removeTile(93, 103, "Front");
      this.removeTile(94, 103, "Front");
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      this.minecartSteam = (TemporaryAnimatedSprite) null;
      if ((Game1.locationRequest == null || Game1.locationRequest.Location == null || Game1.locationRequest.Location.IsOutdoors) && Game1.getMusicTrackName().Contains("town"))
        Game1.changeMusicTrack("none");
      if (this.mapLoader == null)
        return;
      this.mapLoader.Dispose();
      this.mapLoader = (LocalizedContentManager) null;
    }

    public void initiateMarnieLewisBush()
    {
      Game1.player.freezePause = 3000;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Marnie", new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 32), new Vector2(48f, 98f) * 64f, false, 0.0f, Color.White)
      {
        scale = 4f,
        animationLength = 4,
        interval = 200f,
        totalNumberOfLoops = 99999,
        motion = new Vector2(-3f, -12f),
        acceleration = new Vector2(0.0f, 0.4f),
        xStopCoordinate = 2880,
        yStopCoordinate = 6336,
        layerDepth = 0.64f,
        reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.marnie_landed),
        id = 888f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Lewis", new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 32), new Vector2(48f, 98f) * 64f, false, 0.0f, Color.White)
      {
        scale = 4f,
        animationLength = 4,
        interval = 200f,
        totalNumberOfLoops = 99999,
        motion = new Vector2(3f, -12f),
        acceleration = new Vector2(0.0f, 0.4f),
        xStopCoordinate = 3264,
        yStopCoordinate = 6336,
        layerDepth = 0.64f,
        id = 777f
      });
      Game1.playSound("dwop");
    }

    private void marnie_landed(int extra)
    {
      Game1.player.freezePause = 2000;
      TemporaryAnimatedSprite temporarySpriteById1 = this.getTemporarySpriteByID(777);
      if (temporarySpriteById1 != null)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Lewis", new Microsoft.Xna.Framework.Rectangle(0, 32, 16, 32), temporarySpriteById1.position, false, 0.0f, Color.White)
        {
          scale = 4f,
          animationLength = 4,
          interval = 60f,
          totalNumberOfLoops = 50,
          layerDepth = 0.64f,
          id = 0.0f,
          motion = new Vector2(8f, 0.0f)
        });
      TemporaryAnimatedSprite temporarySpriteById2 = this.getTemporarySpriteByID(888);
      if (temporarySpriteById2 != null)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Marnie", new Microsoft.Xna.Framework.Rectangle(0, 32, 16, 32), temporarySpriteById2.position, true, 0.0f, Color.White)
        {
          scale = 4f,
          animationLength = 4,
          interval = 60f,
          totalNumberOfLoops = 50,
          layerDepth = 0.64f,
          id = 1f,
          motion = new Vector2(-8f, 0.0f)
        });
      this.removeTemporarySpritesWithID(777);
      this.removeTemporarySpritesWithID(888);
      for (int index = 0; index < 3200; index += 200)
        DelayedAction.playSoundAfterDelay("grassyStep", 100 + index);
    }

    public void initiateMagnifyingGlassGet()
    {
      Game1.player.freezePause = 3000;
      if (Game1.player.getTileX() >= 31)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Krobus", new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 24), new Vector2(29f, 13f) * 64f, false, 0.0f, Color.White)
        {
          scale = 4f,
          animationLength = 4,
          interval = 200f,
          totalNumberOfLoops = 99999,
          motion = new Vector2(3f, -12f),
          acceleration = new Vector2(0.0f, 0.4f),
          xStopCoordinate = 2048,
          yStopCoordinate = 960,
          layerDepth = 1f,
          reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.mgThief_landed),
          id = 777f
        });
      else
        this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Krobus", new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 24), new Vector2(29f, 13f) * 64f, false, 0.0f, Color.White)
        {
          scale = 4f,
          animationLength = 4,
          interval = 200f,
          totalNumberOfLoops = 99999,
          motion = new Vector2(2f, -12f),
          acceleration = new Vector2(0.0f, 0.4f),
          xStopCoordinate = 1984,
          yStopCoordinate = 832,
          layerDepth = 0.0896f,
          reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.mgThief_landed),
          id = 777f
        });
      Game1.playSound("dwop");
    }

    private void mgThief_landed(int extra)
    {
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
      if (temporarySpriteById == null)
        return;
      temporarySpriteById.animationLength = 1;
      temporarySpriteById.shakeIntensity = 1f;
      temporarySpriteById.interval = 1500f;
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.totalNumberOfLoops = 1;
      temporarySpriteById.currentNumberOfLoops = 0;
      temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.mgThief_speech);
      Game1.playSound("snowyStep");
    }

    private void mgThief_speech(int extra)
    {
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_mgThiefMessage"));
      Game1.afterDialogues = new Game1.afterFadeFunction(this.mgThief_afterSpeech);
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
      if (temporarySpriteById == null)
        return;
      temporarySpriteById.animationLength = 4;
      temporarySpriteById.shakeIntensity = 0.0f;
      temporarySpriteById.interval = 200f;
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.totalNumberOfLoops = 9999;
      temporarySpriteById.currentNumberOfLoops = 0;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Krobus", new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 24), temporarySpriteById.position, false, 0.0f, Color.White)
      {
        scale = 4f,
        animationLength = 4,
        interval = 200f,
        totalNumberOfLoops = 99999,
        layerDepth = 0.0896f,
        id = 777f
      });
    }

    private void mgThief_afterSpeech()
    {
      Game1.player.holdUpItemThenMessage((Item) new SpecialItem(5));
      Game1.afterDialogues = new Game1.afterFadeFunction(this.mgThief_afterGlass);
      Game1.player.hasMagnifyingGlass = true;
      Game1.player.removeQuest(31);
    }

    private void mgThief_afterGlass()
    {
      Game1.player.freezePause = 1500;
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
      if (temporarySpriteById == null)
        return;
      temporarySpriteById.animationLength = 1;
      temporarySpriteById.shakeIntensity = 1f;
      temporarySpriteById.interval = 500f;
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.totalNumberOfLoops = 1;
      temporarySpriteById.currentNumberOfLoops = 0;
      temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.mg_disappear);
    }

    private void mg_disappear(int extra)
    {
      Game1.player.freezePause = 1000;
      TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(777);
      if (temporarySpriteById == null)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Krobus", new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 24), temporarySpriteById.position, false, 0.0f, Color.White)
      {
        scale = 4f,
        animationLength = 4,
        interval = 60f,
        totalNumberOfLoops = 50,
        layerDepth = 0.0896f,
        id = 777f,
        motion = new Vector2(0.0f, 8f)
      });
      for (int index = 0; index < 3200; index += 200)
        DelayedAction.playSoundAfterDelay("snowyStep", 100 + index);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      if (this.minecartSteam == null)
        return;
      this.minecartSteam.update(time);
    }

    public override void draw(SpriteBatch spriteBatch)
    {
      base.draw(spriteBatch);
      if (this.minecartSteam != null)
        this.minecartSteam.draw(spriteBatch);
      if (this.ccJoja && !this._appliedMapOverrides.Contains("Town-TheaterCC"))
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.ccFacadePositionBottom), new Microsoft.Xna.Framework.Rectangle?(Town.jojaFacadeBottom), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.128f);
      if (!this.playerCheckedBoard)
      {
        float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2616f, 3472f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.98f);
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2656f, 3512f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(175, 425, 12, 12)), Color.White * 0.75f, 0.0f, new Vector2(6f, 6f), 4f, SpriteEffects.None, 1f);
      }
      if (Game1.CanAcceptDailyQuest())
      {
        float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2692f, 3528f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8)), Color.White, 0.0f, new Vector2(1f, 4f), 4f + Math.Max(0.0f, (float) (0.25 - (double) num / 16.0)), SpriteEffects.None, 1f);
      }
      if (!SpecialOrder.IsSpecialOrdersBoardUnlocked() || Game1.player.team.acceptedSpecialOrderTypes.Contains("") || Game1.eventUp)
        return;
      float num1 = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3997.6f, 5908.8f + num1)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8)), Color.White, 0.0f, new Vector2(1f, 4f), 4f + Math.Max(0.0f, (float) (0.25 - (double) num1 / 8.0)), SpriteEffects.None, 1f);
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
      bool flag = this.IsUsingMagicBait(who);
      if ((double) bobberTile.X < 30.0 && (double) bobberTile.Y < 30.0)
        return Game1.random.NextDouble() < 0.1 ? (StardewValley.Object) new Furniture(2427, Vector2.Zero) : new StardewValley.Object(Game1.random.NextDouble() < 0.5 ? 388 : 390, 1);
      float num = 0.0f;
      if (who != null && who.CurrentTool is FishingRod && (who.CurrentTool as FishingRod).getBobberAttachmentIndex() == 856)
        num += 0.05f;
      if ((double) who.getTileLocation().Y < 15.0 && who.FishingLevel >= 3 && Game1.random.NextDouble() < 0.2 + (double) num)
      {
        if (Game1.player.team.SpecialOrderRuleActive("LEGENDARY_FAMILY"))
          return new StardewValley.Object(899, 1);
        if (!who.fishCaught.ContainsKey(160) && Game1.currentSeason.Equals("fall") | flag)
          return new StardewValley.Object(160, 1);
      }
      return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      if (this.ccJoja && !this._appliedMapOverrides.Contains("Town-TheaterCC"))
      {
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.ccFacadePosition), new Microsoft.Xna.Framework.Rectangle?(Town.jojaFacadeTop), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.128f);
        if (Game1.IsWinter)
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.ccFacadePosition), new Microsoft.Xna.Framework.Rectangle?(Town.jojaFacadeWinterOverlay), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1281f);
      }
      else if (!this.ccJoja && this.ccRefurbished)
      {
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.clockCenter), new Microsoft.Xna.Framework.Rectangle?(Town.hourHandSource), Color.White, (float) (2.0 * Math.PI * ((double) (Game1.timeOfDay % 1200) / 1200.0) + (double) Game1.gameTimeInterval / 7000.0 / 23.0), new Vector2(2.5f, 8f), 4f, SpriteEffects.None, 0.98f);
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.clockCenter), new Microsoft.Xna.Framework.Rectangle?(Town.minuteHandSource), Color.White, (float) (2.0 * Math.PI * ((double) (Game1.timeOfDay % 1000 % 100 % 60) / 60.0) + (double) Game1.gameTimeInterval / 7000.0 * 1.01999998092651), new Vector2(2.5f, 12f), 4f, SpriteEffects.None, 0.99f);
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.clockCenter), new Microsoft.Xna.Framework.Rectangle?(Town.clockNub), Color.White, 0.0f, new Vector2(2f, 2f), 4f, SpriteEffects.None, 1f);
      }
      base.drawAboveAlwaysFrontLayer(b);
    }

    public override bool performAction(string action, Farmer who, Location tileLocation) => base.performAction(action, who, tileLocation);
  }
}
