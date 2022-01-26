// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Beach
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Network;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class Beach : GameLocation
  {
    private NPC oldMariner;
    [XmlElement("bridgeFixed")]
    public readonly NetBool bridgeFixed = new NetBool();
    private bool hasShownCCUpgrade;

    public Beach()
    {
    }

    public Beach(string mapPath, string name)
      : base(mapPath, name)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddField((INetSerializable) this.bridgeFixed);
      this.bridgeFixed.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        Beach.fixBridge((GameLocation) this);
      });
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      if (this.wasUpdated)
        return;
      base.UpdateWhenCurrentLocation(time);
      if (this.oldMariner != null)
        this.oldMariner.update(time, (GameLocation) this);
      if (Game1.eventUp || Game1.random.NextDouble() >= 1E-06)
        return;
      Vector2 position = new Vector2((float) (Game1.random.Next(15, 47) * 64), (float) (Game1.random.Next(29, 42) * 64));
      bool flag = true;
      for (float yTile = position.Y / 64f; (double) yTile < (double) this.map.GetLayer("Back").LayerHeight; ++yTile)
      {
        if (this.doesTileHaveProperty((int) position.X / 64, (int) yTile, "Water", "Back") == null || this.doesTileHaveProperty((int) position.X / 64 - 1, (int) yTile, "Water", "Back") == null || this.doesTileHaveProperty((int) position.X / 64 + 1, (int) yTile, "Water", "Back") == null)
        {
          flag = false;
          break;
        }
      }
      if (!flag)
        return;
      this.temporarySprites.Add((TemporaryAnimatedSprite) new SeaMonsterTemporarySprite(250f, 4, Game1.random.Next(7), position));
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      if (!Game1.isRaining && !Game1.isFestival())
        Game1.changeMusicTrack("none");
      this.oldMariner = (NPC) null;
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
      float num = 0.0f;
      if (who != null && who.CurrentTool is FishingRod && (who.CurrentTool as FishingRod).getBobberAttachmentIndex() == 856)
        num += 0.07f;
      if (who.getTileX() >= 82 && who.FishingLevel >= 5 && waterDepth >= 3 && Game1.random.NextDouble() < 0.18 + (double) num)
      {
        if (Game1.player.team.SpecialOrderRuleActive("LEGENDARY_FAMILY"))
          return new StardewValley.Object(898, 1);
        if (!who.fishCaught.ContainsKey(159) && Game1.currentSeason.Equals("summer") | flag)
          return new StardewValley.Object(159, 1);
      }
      return flag && (double) bobberTile.X < 12.0 && (double) bobberTile.Y > 31.0 && waterDepth >= 3 && Game1.random.NextDouble() < 0.1 + (double) num * 1.5 ? new StardewValley.Object(798 + Game1.random.Next(3), 1) : base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(65, 11, 25, 12);
      for (float num = 1f; Game1.random.NextDouble() < (double) num; num /= 2f)
      {
        int parentSheetIndex = 393;
        if (Game1.random.NextDouble() < 0.2)
          parentSheetIndex = 397;
        Vector2 v = new Vector2((float) Game1.random.Next(rectangle1.X, rectangle1.Right), (float) Game1.random.Next(rectangle1.Y, rectangle1.Bottom));
        if (this.isTileLocationTotallyClearAndPlaceable(v))
          this.dropObject(new StardewValley.Object(parentSheetIndex, 1), v * 64f, Game1.viewport, true);
      }
      Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(66, 24, 19, 1);
      for (float num = 0.25f; Game1.random.NextDouble() < (double) num; num /= 2f)
      {
        if (Game1.random.NextDouble() < 0.1)
        {
          Vector2 v = new Vector2((float) Game1.random.Next(rectangle2.X, rectangle2.Right), (float) Game1.random.Next(rectangle2.Y, rectangle2.Bottom));
          if (this.isTileLocationTotallyClearAndPlaceable(v))
            this.dropObject(new StardewValley.Object(152, 1), v * 64f, Game1.viewport, true);
        }
      }
      if (!Game1.currentSeason.Equals("summer") || Game1.dayOfMonth < 12 || Game1.dayOfMonth > 14)
        return;
      for (int index = 0; index < 5; ++index)
        this.spawnObjects();
      for (float num = 1.5f; Game1.random.NextDouble() < (double) num; num /= 1.1f)
      {
        int parentSheetIndex = 393;
        if (Game1.random.NextDouble() < 0.2)
          parentSheetIndex = 397;
        Vector2 randomTile = this.getRandomTile();
        randomTile.Y /= 2f;
        string str = this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "Type", "Back");
        if (this.isTileLocationTotallyClearAndPlaceable(randomTile) && (str == null || !str.Equals("Wood")))
          this.dropObject(new StardewValley.Object(parentSheetIndex, 1), randomTile * 64f, Game1.viewport, true);
      }
    }

    public void doneWithBridgeFix()
    {
      Game1.globalFadeToClear();
      Game1.viewportFreeze = false;
      Game1.freezeControls = false;
    }

    public void fadedForBridgeFix()
    {
      Game1.freezeControls = true;
      DelayedAction.playSoundAfterDelay("crafting", 1000);
      DelayedAction.playSoundAfterDelay("crafting", 1500);
      DelayedAction.playSoundAfterDelay("crafting", 2000);
      DelayedAction.playSoundAfterDelay("crafting", 2500);
      DelayedAction.playSoundAfterDelay("axchop", 3000);
      DelayedAction.playSoundAfterDelay("Ship", 3200);
      Game1.viewportFreeze = true;
      Game1.viewport.X = -10000;
      this.bridgeFixed.Value = true;
      Game1.pauseThenDoFunction(4000, new Game1.afterFadeFunction(this.doneWithBridgeFix));
      Beach.fixBridge((GameLocation) this);
    }

    public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
    {
      switch (questionAndAnswer)
      {
        case "BeachBridge_Yes":
          Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.fadedForBridgeFix));
          Game1.player.removeItemsFromInventory(388, 300);
          return true;
        default:
          return base.answerDialogueAction(questionAndAnswer, questionParams);
      }
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      switch (this.map.GetLayer("Buildings").Tiles[tileLocation] != null ? this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex : -1)
      {
        case 284:
          if (who.hasItemInInventory(388, 300))
          {
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Question"), this.createYesNoResponses(), "BeachBridge");
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Beach_FixBridge_Hint"));
          break;
        case 496:
          if (!Game1.MasterPlayer.mailReceived.Contains("spring_2_1"))
          {
            Game1.drawLetterMessage(Game1.content.LoadString("Strings\\Locations:Beach_GoneFishingMessage").Replace('\n', '^'));
            return false;
          }
          break;
      }
      if (this.oldMariner == null || this.oldMariner.getTileX() != tileLocation.X || this.oldMariner.getTileY() != tileLocation.Y)
        return base.checkAction(tileLocation, viewport, who);
      string sub1 = Game1.content.LoadString("Strings\\Locations:Beach_Mariner_Player_" + (who.IsMale ? "Male" : "Female"));
      if (!who.isMarried() && who.specialItems.Contains(460) && !Utility.doesItemWithThisIndexExistAnywhere(460))
      {
        for (int index = who.specialItems.Count - 1; index >= 0; --index)
        {
          if (who.specialItems[index] == 460)
            who.specialItems.RemoveAt(index);
        }
      }
      if (who.isMarried())
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerMarried", (object) sub1)));
      else if (who.specialItems.Contains(460))
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerHasItem", (object) sub1)));
      else if (who.hasAFriendWithHeartLevel(10, true) && (int) (NetFieldBase<int, NetInt>) who.houseUpgradeLevel == 0)
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerNotUpgradedHouse", (object) sub1)));
      else if (who.hasAFriendWithHeartLevel(10, true))
      {
        Response[] answerChoices = new Response[2]
        {
          new Response("Buy", Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_AnswerYes")),
          new Response("Not", Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_AnswerNo"))
        };
        this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerBuyItem_Question", (object) sub1)), answerChoices, "mariner");
      }
      else
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Beach_Mariner_PlayerNoRelationship", (object) sub1)));
      return true;
    }

    public override bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character)
    {
      return this.oldMariner != null && position.Intersects(this.oldMariner.GetBoundingBox()) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
    }

    public override void checkForMusic(GameTime time)
    {
      if (Game1.random.NextDouble() < 0.003 && Game1.timeOfDay < 1900)
        this.localSound("seagulls");
      base.checkForMusic(time);
    }

    protected override void resetSharedState()
    {
      base.resetSharedState();
      if (!Game1.currentSeason.Equals("summer") || Game1.dayOfMonth < 12 || Game1.dayOfMonth > 14)
        return;
      this.waterColor.Value = new Color(0, (int) byte.MaxValue, 0) * 0.4f;
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (force)
        this.hasShownCCUpgrade = false;
      if ((bool) (NetFieldBase<bool, NetBool>) this.bridgeFixed)
        Beach.fixBridge((GameLocation) this);
      if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
        return;
      Beach.showCommunityUpgradeShortcuts((GameLocation) this, ref this.hasShownCCUpgrade);
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (!Game1.isRaining && !Game1.isFestival())
        Game1.changeMusicTrack("ocean");
      int number = Game1.random.Next(6);
      foreach (Vector2 vector2 in Utility.getPositionsInClusterAroundThisTile(new Vector2((float) Game1.random.Next(this.map.DisplayWidth / 64), (float) Game1.random.Next(12, this.map.DisplayHeight / 64)), number))
      {
        if (this.isTileOnMap(vector2) && (this.isTileLocationTotallyClearAndPlaceable(vector2) || this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Water", "Back") != null))
        {
          int startingState = 3;
          if (this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Water", "Back") != null)
          {
            startingState = 2;
            if (Game1.random.NextDouble() < 0.5)
              continue;
          }
          this.critters.Add((Critter) new Seagull(vector2 * 64f + new Vector2(32f, 32f), startingState));
        }
      }
      if (!Game1.isRaining || Game1.timeOfDay >= 1900)
        return;
      this.oldMariner = new NPC(new AnimatedSprite("Characters\\Mariner", 0, 16, 32), new Vector2(80f, 5f) * 64f, 2, "Old Mariner");
    }

    public static void showCommunityUpgradeShortcuts(GameLocation location, ref bool flag)
    {
      if (flag)
        return;
      flag = true;
      location.warps.Add(new Warp(-1, 4, "Forest", 119, 35, false));
      location.warps.Add(new Warp(-1, 5, "Forest", 119, 35, false));
      location.warps.Add(new Warp(-1, 6, "Forest", 119, 36, false));
      location.warps.Add(new Warp(-1, 7, "Forest", 119, 36, false));
      for (int x = 0; x < 5; ++x)
      {
        for (int y = 4; y < 7; ++y)
          location.removeTile(x, y, "Buildings");
      }
      location.removeTile(7, 6, "Buildings");
      location.removeTile(5, 6, "Buildings");
      location.removeTile(6, 6, "Buildings");
      location.setMapTileIndex(3, 7, 107, "Back");
      location.removeTile(67, 5, "Buildings");
      location.removeTile(67, 4, "Buildings");
      location.removeTile(67, 3, "Buildings");
      location.removeTile(67, 2, "Buildings");
      location.removeTile(67, 1, "Buildings");
      location.removeTile(67, 0, "Buildings");
      location.removeTile(66, 3, "Buildings");
      location.removeTile(68, 3, "Buildings");
    }

    public static void fixBridge(GameLocation location)
    {
      if (!NetWorldState.checkAnywhereForWorldStateID("beachBridgeFixed"))
        NetWorldState.addWorldStateIDEverywhere("beachBridgeFixed");
      location.updateMap();
      int whichTileSheet = location.name.Value.Contains("Market") ? 2 : 1;
      location.setMapTile(58, 13, 301, "Buildings", (string) null, whichTileSheet);
      location.setMapTile(59, 13, 301, "Buildings", (string) null, whichTileSheet);
      location.setMapTile(60, 13, 301, "Buildings", (string) null, whichTileSheet);
      location.setMapTile(61, 13, 301, "Buildings", (string) null, whichTileSheet);
      location.setMapTile(58, 14, 336, "Back", (string) null, whichTileSheet);
      location.setMapTile(59, 14, 336, "Back", (string) null, whichTileSheet);
      location.setMapTile(60, 14, 336, "Back", (string) null, whichTileSheet);
      location.setMapTile(61, 14, 336, "Back", (string) null, whichTileSheet);
    }

    public override void draw(SpriteBatch b)
    {
      if (this.oldMariner != null)
        this.oldMariner.draw(b);
      base.draw(b);
      if ((bool) (NetFieldBase<bool, NetBool>) this.bridgeFixed)
        return;
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3704f, 720f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.095401f);
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3744f, 760f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(175, 425, 12, 12)), Color.White * 0.75f, 0.0f, new Vector2(6f, 6f), 4f, SpriteEffects.None, 0.09541f);
    }
  }
}
