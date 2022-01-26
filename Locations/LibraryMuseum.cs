// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.LibraryMuseum
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class LibraryMuseum : GameLocation
  {
    public const int dwarvenGuide = 0;
    public const int totalArtifacts = 95;
    public const int totalNotes = 21;
    [Obsolete]
    [XmlIgnore]
    private Dictionary<int, Vector2> lostBooksLocations = new Dictionary<int, Vector2>();
    private readonly NetMutex mutex = new NetMutex();

    [XmlElement("museumPieces")]
    public NetVector2Dictionary<int, NetInt> museumPieces => Game1.netWorldState.Value.MuseumPieces;

    public LibraryMuseum()
    {
    }

    public LibraryMuseum(string mapPath, string name)
      : base(mapPath, name)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddField((INetSerializable) this.mutex.NetFields);
    }

    public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
    {
      this.mutex.Update((GameLocation) this);
      base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
    }

    public bool museumAlreadyHasArtifact(int index)
    {
      foreach (KeyValuePair<Vector2, int> pair in this.museumPieces.Pairs)
      {
        if (pair.Value == index)
          return true;
      }
      return false;
    }

    public bool isItemSuitableForDonation(Item i)
    {
      if (i is StardewValley.Object && (NetFieldBase<string, NetString>) (i as StardewValley.Object).type != (NetString) null && ((i as StardewValley.Object).type.Equals((object) "Arch") || (i as StardewValley.Object).type.Equals((object) "Minerals")))
      {
        int parentSheetIndex = (int) (NetFieldBase<int, NetInt>) (i as StardewValley.Object).parentSheetIndex;
        bool flag = false;
        foreach (KeyValuePair<Vector2, int> pair in this.museumPieces.Pairs)
        {
          if (pair.Value == parentSheetIndex)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return true;
      }
      return false;
    }

    public bool doesFarmerHaveAnythingToDonate(Farmer who)
    {
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) who.maxItems; ++index)
      {
        if (index < who.items.Count && who.items[index] is StardewValley.Object && (NetFieldBase<string, NetString>) (who.items[index] as StardewValley.Object).type != (NetString) null && ((who.items[index] as StardewValley.Object).type.Equals((object) "Arch") || (who.items[index] as StardewValley.Object).type.Equals((object) "Minerals")))
        {
          int parentSheetIndex = (int) (NetFieldBase<int, NetInt>) (who.items[index] as StardewValley.Object).parentSheetIndex;
          bool flag = false;
          foreach (KeyValuePair<Vector2, int> pair in this.museumPieces.Pairs)
          {
            if (pair.Value == parentSheetIndex)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            return true;
        }
      }
      return false;
    }

    private bool museumContainsTheseItems(int[] items, HashSet<int> museumItems)
    {
      for (int index = 0; index < items.Length; ++index)
      {
        if (!museumItems.Contains(items[index]))
          return false;
      }
      return true;
    }

    private int numberOfMuseumItemsOfType(string type)
    {
      int num = 0;
      foreach (KeyValuePair<Vector2, int> pair in this.museumPieces.Pairs)
      {
        if (Game1.objectInformation[pair.Value].Split('/')[3].Contains(type))
          ++num;
      }
      return num;
    }

    private Dictionary<int, Vector2> getLostBooksLocations()
    {
      Dictionary<int, Vector2> lostBooksLocations = new Dictionary<int, Vector2>();
      for (int index1 = 0; index1 < this.map.Layers[0].LayerWidth; ++index1)
      {
        for (int index2 = 0; index2 < this.map.Layers[0].LayerHeight; ++index2)
        {
          if (this.doesTileHaveProperty(index1, index2, "Action", "Buildings") != null && this.doesTileHaveProperty(index1, index2, "Action", "Buildings").Contains("Notes"))
            lostBooksLocations.Add(Convert.ToInt32(this.doesTileHaveProperty(index1, index2, "Action", "Buildings").Split(' ')[1]), new Vector2((float) index1, (float) index2));
        }
      }
      return lostBooksLocations;
    }

    protected override void resetLocalState()
    {
      if (!Game1.player.eventsSeen.Contains(0) && this.doesFarmerHaveAnythingToDonate(Game1.player) && !Game1.player.mailReceived.Contains("somethingToDonate"))
        Game1.player.mailReceived.Add("somethingToDonate");
      if (this.museumPieces.Count() > 0 && !Game1.player.mailReceived.Contains("somethingWasDonated"))
        Game1.player.mailReceived.Add("somethingWasDonated");
      base.resetLocalState();
      if (!Game1.isRaining)
        Game1.changeMusicTrack("libraryTheme");
      int lostBooksFound = (int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.LostBooksFound;
      Dictionary<int, Vector2> lostBooksLocations = this.getLostBooksLocations();
      for (int index = 0; index < lostBooksLocations.Count; ++index)
      {
        KeyValuePair<int, Vector2> keyValuePair = lostBooksLocations.ElementAt<KeyValuePair<int, Vector2>>(index);
        if (keyValuePair.Key <= lostBooksFound)
        {
          NetStringList mailReceived = Game1.player.mailReceived;
          keyValuePair = lostBooksLocations.ElementAt<KeyValuePair<int, Vector2>>(index);
          string str = "lb_" + keyValuePair.Key.ToString();
          if (!mailReceived.Contains(str))
          {
            List<TemporaryAnimatedSprite> temporarySprites = this.temporarySprites;
            Microsoft.Xna.Framework.Rectangle sourceRect = new Microsoft.Xna.Framework.Rectangle(144, 447, 15, 15);
            keyValuePair = lostBooksLocations.ElementAt<KeyValuePair<int, Vector2>>(index);
            double x = (double) keyValuePair.Value.X * 64.0;
            keyValuePair = lostBooksLocations.ElementAt<KeyValuePair<int, Vector2>>(index);
            double y = (double) keyValuePair.Value.Y * 64.0 - 96.0 - 16.0;
            Vector2 position = new Vector2((float) x, (float) y);
            Color white = Color.White;
            TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, position, false, 0.0f, white);
            temporaryAnimatedSprite.interval = 99999f;
            temporaryAnimatedSprite.animationLength = 1;
            temporaryAnimatedSprite.totalNumberOfLoops = 9999;
            temporaryAnimatedSprite.yPeriodic = true;
            temporaryAnimatedSprite.yPeriodicLoopTime = 4000f;
            temporaryAnimatedSprite.yPeriodicRange = 16f;
            temporaryAnimatedSprite.layerDepth = 1f;
            temporaryAnimatedSprite.scale = 4f;
            keyValuePair = lostBooksLocations.ElementAt<KeyValuePair<int, Vector2>>(index);
            temporaryAnimatedSprite.id = (float) keyValuePair.Key;
            temporarySprites.Add(temporaryAnimatedSprite);
          }
        }
      }
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      if (Game1.isRaining)
        return;
      Game1.changeMusicTrack("none");
    }

    public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
    {
      switch (questionAndAnswer)
      {
        case "Museum_Collect":
          Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.getRewardsForPlayer(Game1.player), false, true, (InventoryMenu.highlightThisItem) null, (ItemGrabMenu.behaviorOnItemSelect) null, "Rewards", new ItemGrabMenu.behaviorOnItemSelect(this.collectedReward), canBeExitedWithKey: true, playRightClickSound: false, allowRightClick: false, context: ((object) this));
          break;
        case "Museum_Donate":
          this.mutex.RequestLock((Action) (() =>
          {
            Game1.activeClickableMenu = (IClickableMenu) new MuseumMenu(new InventoryMenu.highlightThisItem(this.isItemSuitableForDonation))
            {
              exitFunction = (IClickableMenu.onExit) (() => this.mutex.ReleaseLock())
            };
          }));
          break;
        case "Museum_Rearrange_Yes":
          if (!this.mutex.IsLocked())
          {
            this.mutex.RequestLock((Action) (() =>
            {
              Game1.activeClickableMenu = (IClickableMenu) new MuseumMenu(new InventoryMenu.highlightThisItem(InventoryMenu.highlightNoItems))
              {
                exitFunction = (IClickableMenu.onExit) (() => this.mutex.ReleaseLock())
              };
            }));
            break;
          }
          break;
        case null:
          return false;
      }
      return base.answerDialogueAction(questionAndAnswer, questionParams);
    }

    public string getRewardItemKey(Item item) => "museumCollectedReward" + Utility.getStandardDescriptionFromItem(item, 1, '_');

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (action != null && who.IsLocalPlayer)
      {
        string str = action.Split(' ')[0];
        if (!(str == "Gunther"))
        {
          if (str == "Rearrange" && !this.doesFarmerHaveAnythingToDonate(Game1.player))
          {
            this.rearrange();
            return true;
          }
        }
        else
        {
          this.gunther();
          return true;
        }
      }
      return base.performAction(action, who, tileLocation);
    }

    public void rearrange()
    {
      if (this.museumPieces.Count() <= 0)
        return;
      this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Rearrange"), this.createYesNoResponses(), "Museum_Rearrange");
    }

    public List<Item> getRewardsForPlayer(Farmer who)
    {
      List<Item> rewards = new List<Item>();
      HashSet<int> museumItems = new HashSet<int>((IEnumerable<int>) this.museumPieces.Values);
      int num1 = this.numberOfMuseumItemsOfType("Arch");
      int num2 = this.numberOfMuseumItemsOfType("Minerals");
      int num3 = num1 + num2;
      if (!who.canUnderstandDwarves && museumItems.Contains(96) && museumItems.Contains(97) && museumItems.Contains(98) && museumItems.Contains(99))
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(326, 1));
      if (!who.specialBigCraftables.Contains(1305) && museumItems.Contains(113) && num1 > 4)
        this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1305, Vector2.Zero));
      if (!who.specialBigCraftables.Contains(1304) && num1 >= 15)
        this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1304, Vector2.Zero));
      if (!who.specialBigCraftables.Contains(139) && num1 >= 20)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(Vector2.Zero, 139));
      if (!who.specialBigCraftables.Contains(1545))
      {
        if (this.museumContainsTheseItems(new int[2]
        {
          108,
          122
        }, museumItems) && num1 > 10)
          this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1545, Vector2.Zero));
      }
      if (!who.specialItems.Contains(464) && museumItems.Contains(119) && num1 > 2)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(464, 1));
      if (!who.specialItems.Contains(463) && museumItems.Contains(123) && num1 > 2)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(463, 1));
      if (!who.specialItems.Contains(499) && museumItems.Contains(114))
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(499, 1));
      if (!who.knowsRecipe("Ancient Seeds") && museumItems.Contains(114))
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(499, 1, true));
      if (!who.specialBigCraftables.Contains(1301))
      {
        if (this.museumContainsTheseItems(new int[3]
        {
          579,
          581,
          582
        }, museumItems))
          this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1301, Vector2.Zero));
      }
      if (!who.specialBigCraftables.Contains(1302))
      {
        if (this.museumContainsTheseItems(new int[2]
        {
          583,
          584
        }, museumItems))
          this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1302, Vector2.Zero));
      }
      if (!who.specialBigCraftables.Contains(1303))
      {
        if (this.museumContainsTheseItems(new int[2]
        {
          580,
          585
        }, museumItems))
          this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1303, Vector2.Zero));
      }
      if (!who.specialBigCraftables.Contains(1298) && num2 > 10)
        this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1298, Vector2.Zero));
      if (!who.specialBigCraftables.Contains(1299) && num2 > 30)
        this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1299, Vector2.Zero));
      if (!who.specialBigCraftables.Contains(94) && num2 > 20)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(Vector2.Zero, 94));
      if (!who.specialBigCraftables.Contains(21) && num2 >= 50)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(Vector2.Zero, 21));
      if (!who.specialBigCraftables.Contains(131) && num2 > 40)
        this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(131, Vector2.Zero));
      foreach (Item obj in rewards)
        obj.specialItem = true;
      if (!who.mailReceived.Contains("museum5") && num3 >= 5)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(474, 9));
      if (!who.mailReceived.Contains("museum10") && num3 >= 10)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(479, 9));
      if (!who.mailReceived.Contains("museum15") && num3 >= 15)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(486, 1));
      if (!who.mailReceived.Contains("museum20") && num3 >= 20)
        this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1541, Vector2.Zero));
      if (!who.mailReceived.Contains("museum25") && num3 >= 25)
        this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1554, Vector2.Zero));
      if (!who.mailReceived.Contains("museum30") && num3 >= 30)
        this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1669, Vector2.Zero));
      if (!who.mailReceived.Contains("museum35") && num3 >= 35)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(490, 9));
      if (!who.mailReceived.Contains("museum40") && num3 >= 40)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(Vector2.Zero, 140));
      if (!who.mailReceived.Contains("museum50") && num3 >= 50)
        this.AddRewardIfUncollected(who, rewards, (Item) new Furniture(1671, Vector2.Zero));
      if (!who.mailReceived.Contains("museum70") && num3 >= 70)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(253, 3));
      if (!who.mailReceived.Contains("museum80") && num3 >= 80)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(688, 5));
      if (!who.mailReceived.Contains("museum90") && num3 >= 90)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(279, 1));
      if (!who.mailReceived.Contains("museumComplete") && num3 >= 95)
        this.AddRewardIfUncollected(who, rewards, (Item) new StardewValley.Object(434, 1));
      if (num3 >= 60)
      {
        if (!Game1.player.eventsSeen.Contains(295672))
          Game1.player.eventsSeen.Add(295672);
        else if (!Game1.player.hasRustyKey)
          Game1.player.eventsSeen.Remove(66);
      }
      return rewards;
    }

    public void AddRewardIfUncollected(Farmer farmer, List<Item> rewards, Item reward_item)
    {
      if (farmer.mailReceived.Contains(this.getRewardItemKey(reward_item)))
        return;
      rewards.Add(reward_item);
    }

    public void collectedReward(Item item, Farmer who)
    {
      if (item == null)
        return;
      if (item is StardewValley.Object)
      {
        (item as StardewValley.Object).specialItem = true;
        switch ((item as StardewValley.Object).ParentSheetIndex)
        {
          case 140:
            who.mailReceived.Add("museum40");
            break;
          case 253:
            who.mailReceived.Add("museum70");
            break;
          case 279:
            who.mailReceived.Add("museum90");
            break;
          case 434:
            who.mailReceived.Add("museumComplete");
            break;
          case 474:
            who.mailReceived.Add("museum5");
            break;
          case 479:
            who.mailReceived.Add("museum10");
            break;
          case 486:
            who.mailReceived.Add("museum15");
            break;
          case 490:
            who.mailReceived.Add("museum35");
            break;
          case 688:
            who.mailReceived.Add("museum80");
            break;
          case 1541:
            who.mailReceived.Add("museum20");
            break;
          case 1554:
            who.mailReceived.Add("museum25");
            break;
          case 1669:
            who.mailReceived.Add("museum30");
            break;
          case 1671:
            who.mailReceived.Add("museum50");
            break;
        }
      }
      if (who.hasOrWillReceiveMail(this.getRewardItemKey(item)))
        return;
      who.mailReceived.Add(this.getRewardItemKey(item));
    }

    private void gunther()
    {
      if (this.doesFarmerHaveAnythingToDonate(Game1.player) && !this.mutex.IsLocked())
      {
        Response[] answerChoices;
        if (this.getRewardsForPlayer(Game1.player).Count > 0)
          answerChoices = new Response[3]
          {
            new Response("Donate", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Donate")),
            new Response("Collect", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Collect")),
            new Response("Leave", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Leave"))
          };
        else
          answerChoices = new Response[2]
          {
            new Response("Donate", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Donate")),
            new Response("Leave", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Leave"))
          };
        this.createQuestionDialogue("", answerChoices, "Museum");
      }
      else if (this.getRewardsForPlayer(Game1.player).Count > 0)
        this.createQuestionDialogue("", new Response[2]
        {
          new Response("Collect", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Collect")),
          new Response("Leave", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Leave"))
        }, "Museum");
      else if (this.doesFarmerHaveAnythingToDonate(Game1.player) && this.mutex.IsLocked())
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NPC_Busy", (object) Game1.getCharacterFromName("Gunther").displayName));
      else if (Game1.player.achievements.Contains(5))
        Game1.drawDialogue(Game1.getCharacterFromName("Gunther"), Game1.parseText(Game1.content.LoadString("Data\\ExtraDialogue:Gunther_MuseumComplete")));
      else
        Game1.drawDialogue(Game1.getCharacterFromName("Gunther"), Game1.player.mailReceived.Contains("artifactFound") ? Game1.parseText(Game1.content.LoadString("Data\\ExtraDialogue:Gunther_NothingToDonate")) : Game1.content.LoadString("Data\\ExtraDialogue:Gunther_NoArtifactsFound"));
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      foreach (KeyValuePair<Vector2, int> pair in this.museumPieces.Pairs)
      {
        if ((double) pair.Key.X == (double) tileLocation.X && ((double) pair.Key.Y == (double) tileLocation.Y || (double) pair.Key.Y == (double) (tileLocation.Y - 1)))
        {
          Game1.drawObjectDialogue(Game1.parseText(" - " + Game1.objectInformation[pair.Value].Split('/')[4] + " - " + Environment.NewLine + Game1.objectInformation[pair.Value].Split('/')[5]));
          return true;
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public bool isTileSuitableForMuseumPiece(int x, int y)
    {
      if (!this.museumPieces.ContainsKey(new Vector2((float) x, (float) y)))
      {
        switch (this.getTileIndexAt(new Point(x, y), "Buildings"))
        {
          case 1072:
          case 1073:
          case 1074:
          case 1237:
          case 1238:
            return true;
        }
      }
      return false;
    }

    public Microsoft.Xna.Framework.Rectangle getMuseumDonationBounds() => new Microsoft.Xna.Framework.Rectangle(26, 5, 22, 13);

    public Vector2 getFreeDonationSpot()
    {
      Microsoft.Xna.Framework.Rectangle museumDonationBounds = this.getMuseumDonationBounds();
      for (int x = museumDonationBounds.X; x <= museumDonationBounds.Right; ++x)
      {
        for (int y = museumDonationBounds.Y; y <= museumDonationBounds.Bottom; ++y)
        {
          if (this.isTileSuitableForMuseumPiece(x, y))
            return new Vector2((float) x, (float) y);
        }
      }
      return new Vector2(26f, 5f);
    }

    public Vector2 findMuseumPieceLocationInDirection(
      Vector2 startingPoint,
      int direction,
      int distanceToCheck = 8,
      bool ignoreExistingItems = true)
    {
      Vector2 key = startingPoint;
      Vector2 vector2 = Vector2.Zero;
      switch (direction)
      {
        case 0:
          vector2 = new Vector2(0.0f, -1f);
          break;
        case 1:
          vector2 = new Vector2(1f, 0.0f);
          break;
        case 2:
          vector2 = new Vector2(0.0f, 1f);
          break;
        case 3:
          vector2 = new Vector2(-1f, 0.0f);
          break;
      }
      for (int index1 = 0; index1 < distanceToCheck; ++index1)
      {
        for (int index2 = 0; index2 < distanceToCheck; ++index2)
        {
          key += vector2;
          if (this.isTileSuitableForMuseumPiece((int) key.X, (int) key.Y) || !ignoreExistingItems && this.museumPieces.ContainsKey(key))
            return key;
        }
        key = startingPoint;
        int num = index1 % 2 == 0 ? -1 : 1;
        switch (direction)
        {
          case 0:
          case 2:
            key.X += (float) (num * (index1 / 2 + 1));
            break;
          case 1:
          case 3:
            key.Y += (float) (num * (index1 / 2 + 1));
            break;
        }
      }
      return startingPoint;
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      foreach (TemporaryAnimatedSprite temporarySprite in this.temporarySprites)
      {
        if ((double) temporarySprite.layerDepth >= 1.0)
          temporarySprite.draw(b);
      }
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      foreach (KeyValuePair<Vector2, int> pair in this.museumPieces.Pairs)
      {
        b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, pair.Key * 64f + new Vector2(32f, 52f)), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float) (((double) pair.Key.Y * 64.0 - 2.0) / 10000.0));
        b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, pair.Key * 64f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, pair.Value, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) pair.Key.Y * 64.0 / 10000.0));
      }
    }
  }
}
