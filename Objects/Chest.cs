// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Chest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley.Objects
{
  [XmlInclude(typeof (MeleeWeapon))]
  public class Chest : StardewValley.Object
  {
    public const int capacity = 36;
    [XmlElement("currentLidFrame")]
    public readonly NetInt startingLidFrame = new NetInt(501);
    public readonly NetInt lidFrameCount = new NetInt(5);
    private int currentLidFrame;
    [XmlElement("frameCounter")]
    public readonly NetInt frameCounter = new NetInt(-1);
    [XmlElement("coins")]
    public readonly NetInt coins = new NetInt();
    public readonly NetObjectList<Item> items = new NetObjectList<Item>();
    public readonly NetLongDictionary<NetObjectList<Item>, NetRef<NetObjectList<Item>>> separateWalletItems = new NetLongDictionary<NetObjectList<Item>, NetRef<NetObjectList<Item>>>();
    [XmlElement("chestType")]
    public readonly NetString chestType = new NetString("");
    [XmlElement("tint")]
    public readonly NetColor tint = new NetColor(Color.White);
    [XmlElement("playerChoiceColor")]
    public readonly NetColor playerChoiceColor = new NetColor(Color.Black);
    [XmlElement("playerChest")]
    public readonly NetBool playerChest = new NetBool();
    [XmlElement("fridge")]
    public readonly NetBool fridge = new NetBool();
    [XmlElement("giftbox")]
    public readonly NetBool giftbox = new NetBool();
    [XmlElement("giftboxIndex")]
    public readonly NetInt giftboxIndex = new NetInt();
    [XmlElement("spriteIndexOverride")]
    public readonly NetInt bigCraftableSpriteIndex = new NetInt(-1);
    [XmlElement("dropContents")]
    public readonly NetBool dropContents = new NetBool(false);
    [XmlElement("synchronized")]
    public readonly NetBool synchronized = new NetBool(false);
    [XmlIgnore]
    protected int _shippingBinFrameCounter;
    [XmlIgnore]
    protected bool _farmerNearby;
    [XmlIgnore]
    public NetVector2 kickStartTile = new NetVector2(new Vector2(-1000f, -1000f));
    [XmlIgnore]
    public Vector2? localKickStartTile;
    [XmlIgnore]
    public float kickProgress = -1f;
    [XmlIgnore]
    public readonly NetEvent0 openChestEvent = new NetEvent0();
    [XmlElement("specialChestType")]
    public readonly NetEnum<Chest.SpecialChestTypes> specialChestType = new NetEnum<Chest.SpecialChestTypes>();
    [XmlIgnore]
    public readonly NetMutex mutex = new NetMutex();

    [XmlIgnore]
    public Chest.SpecialChestTypes SpecialChestType
    {
      get => this.specialChestType.Value;
      set => this.specialChestType.Value = value;
    }

    [XmlIgnore]
    public Color Tint
    {
      get => (Color) (NetFieldBase<Color, NetColor>) this.tint;
      set => this.tint.Value = value;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.startingLidFrame, (INetSerializable) this.frameCounter, (INetSerializable) this.coins, (INetSerializable) this.items, (INetSerializable) this.chestType, (INetSerializable) this.tint, (INetSerializable) this.playerChoiceColor, (INetSerializable) this.playerChest, (INetSerializable) this.fridge, (INetSerializable) this.giftbox, (INetSerializable) this.giftboxIndex, (INetSerializable) this.mutex.NetFields, (INetSerializable) this.lidFrameCount, (INetSerializable) this.bigCraftableSpriteIndex, (INetSerializable) this.dropContents, this.openChestEvent.NetFields, (INetSerializable) this.synchronized, (INetSerializable) this.specialChestType, (INetSerializable) this.kickStartTile, (INetSerializable) this.separateWalletItems);
      this.openChestEvent.onEvent += new NetEvent0.Event(this.performOpenChest);
      this.kickStartTile.fieldChangeVisibleEvent += (NetFieldBase<Vector2, NetVector2>.FieldChange) ((field, old_value, new_value) =>
      {
        if (Game1.gameMode == (byte) 6 || (double) new_value.X == -1000.0 || (double) new_value.Y == -1000.0)
          return;
        this.localKickStartTile = new Vector2?((Vector2) (NetFieldBase<Vector2, NetVector2>) this.kickStartTile);
        this.kickProgress = 0.0f;
      });
    }

    public Chest()
    {
      this.Name = nameof (Chest);
      this.type.Value = "interactive";
      this.boundingBox.Value = new Microsoft.Xna.Framework.Rectangle((int) this.tileLocation.X * 64, (int) this.tileLocation.Y * 64, 64, 64);
    }

    public Chest(bool playerChest, Vector2 tileLocation, int parentSheetIndex = 130)
      : base(tileLocation, parentSheetIndex)
    {
      this.Name = nameof (Chest);
      this.type.Value = "Crafting";
      if (playerChest)
      {
        this.playerChest.Value = playerChest;
        this.startingLidFrame.Value = parentSheetIndex + 1;
        this.bigCraftable.Value = true;
        this.canBeSetDown.Value = true;
      }
      else
        this.lidFrameCount.Value = 3;
    }

    public Chest(bool playerChest, int parentSheedIndex = 130)
      : base(Vector2.Zero, parentSheedIndex)
    {
      this.Name = nameof (Chest);
      this.type.Value = "Crafting";
      if (playerChest)
      {
        this.playerChest.Value = playerChest;
        this.startingLidFrame.Value = parentSheedIndex + 1;
        this.bigCraftable.Value = true;
        this.canBeSetDown.Value = true;
      }
      else
        this.lidFrameCount.Value = 3;
    }

    public Chest(Vector2 location)
    {
      this.tileLocation.Value = location;
      this.name = nameof (Chest);
      this.type.Value = "interactive";
      this.boundingBox.Value = new Microsoft.Xna.Framework.Rectangle((int) this.tileLocation.X * 64, (int) this.tileLocation.Y * 64, 64, 64);
    }

    public Chest(string type, Vector2 location, MineShaft mine)
    {
      this.tileLocation.Value = location;
      if (!(type == "OreChest"))
      {
        if (!(type == "dungeon"))
        {
          if (type == "Grand")
          {
            this.tint.Value = new Color(150, 150, (int) byte.MaxValue);
            this.coins.Value = (int) location.Y % 8 + 6;
          }
        }
        else
        {
          switch ((int) location.X % 5)
          {
            case 1:
              this.coins.Value = (int) location.Y % 3 + 2;
              break;
            case 2:
              this.items.Add((Item) new StardewValley.Object((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, 382, (int) location.Y % 3 + 1));
              break;
            case 3:
              this.items.Add((Item) new StardewValley.Object((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, mine.getMineArea() == 0 ? 378 : (mine.getMineArea() == 40 ? 380 : 384), (int) location.Y % 3 + 1));
              break;
            case 4:
              this.chestType.Value = "Monster";
              break;
          }
        }
      }
      else
      {
        for (int index = 0; index < 8; ++index)
          this.items.Add((Item) new StardewValley.Object((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, Game1.random.NextDouble() < 0.5 ? 384 : 382, 1));
      }
      this.name = nameof (Chest);
      this.lidFrameCount.Value = 3;
      this.type.Value = "interactive";
      this.boundingBox.Value = new Microsoft.Xna.Framework.Rectangle((int) this.tileLocation.X * 64, (int) this.tileLocation.Y * 64, 64, 64);
    }

    public Chest(
      int parent_sheet_index,
      Vector2 tile_location,
      int starting_lid_frame,
      int lid_frame_count)
      : base(tile_location, parent_sheet_index)
    {
      this.playerChest.Value = true;
      this.startingLidFrame.Value = starting_lid_frame;
      this.lidFrameCount.Value = lid_frame_count;
      this.bigCraftable.Value = true;
      this.canBeSetDown.Value = true;
    }

    public Chest(int coins, List<Item> items, Vector2 location, bool giftbox = false, int giftboxIndex = 0)
    {
      this.name = nameof (Chest);
      this.type.Value = "interactive";
      this.giftbox.Value = giftbox;
      this.giftboxIndex.Value = giftboxIndex;
      if (!this.giftbox.Value)
        this.lidFrameCount.Value = 3;
      if (items != null)
        this.items.Set((IList<Item>) items);
      this.coins.Value = coins;
      this.tileLocation.Value = location;
      this.boundingBox.Value = new Microsoft.Xna.Framework.Rectangle((int) this.tileLocation.X * 64, (int) this.tileLocation.Y * 64, 64, 64);
    }

    public void resetLidFrame() => this.currentLidFrame = (int) (NetFieldBase<int, NetInt>) this.startingLidFrame;

    public void fixLidFrame()
    {
      if (this.currentLidFrame == 0)
        this.currentLidFrame = (int) (NetFieldBase<int, NetInt>) this.startingLidFrame;
      if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
        return;
      if ((bool) (NetFieldBase<bool, NetBool>) this.playerChest)
      {
        if (this.GetMutex().IsLocked() && !this.GetMutex().IsLockHeld())
        {
          this.currentLidFrame = this.getLastLidFrame();
        }
        else
        {
          if (this.GetMutex().IsLocked())
            return;
          this.currentLidFrame = (int) (NetFieldBase<int, NetInt>) this.startingLidFrame;
        }
      }
      else
      {
        if (this.currentLidFrame != this.startingLidFrame.Value || !this.GetMutex().IsLocked() || this.GetMutex().IsLockHeld())
          return;
        this.currentLidFrame = this.getLastLidFrame();
      }
    }

    public int getLastLidFrame() => this.startingLidFrame.Value + this.lidFrameCount.Value - 1;

    public override bool performObjectDropInAction(Item dropIn, bool probe, Farmer who) => false;

    public override bool performToolAction(Tool t, GameLocation location)
    {
      if (t != null && t.getLastFarmerToUse() != null && t.getLastFarmerToUse() != Game1.player)
        return false;
      if ((bool) (NetFieldBase<bool, NetBool>) this.playerChest)
      {
        if (t == null || t is MeleeWeapon || !t.isHeavyHitter() || !base.performToolAction(t, location))
          return false;
        Farmer player = t.getLastFarmerToUse();
        if (player != null)
        {
          Vector2 c = this.TileLocation;
          if ((double) c.X == 0.0 && (double) c.Y == 0.0)
          {
            bool flag = false;
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in location.objects.Pairs)
            {
              if (pair.Value == this)
              {
                c.X = (float) (int) pair.Key.X;
                c.Y = (float) (int) pair.Key.Y;
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              c = player.GetToolLocation() / 64f;
              c.X = (float) (int) c.X;
              c.Y = (float) (int) c.Y;
            }
          }
          this.GetMutex().RequestLock((Action) (() =>
          {
            this.clearNulls();
            if (this.isEmpty())
            {
              this.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, location);
              if (location.Objects.Remove(c) && this.type.Equals((object) "Crafting") && (int) (NetFieldBase<int, NetInt>) this.fragility != 2)
              {
                NetCollection<Debris> debris1 = location.debris;
                int objectIndex = (bool) (NetFieldBase<bool, NetBool>) this.bigCraftable ? -this.ParentSheetIndex : this.ParentSheetIndex;
                Vector2 toolLocation = player.GetToolLocation();
                Microsoft.Xna.Framework.Rectangle boundingBox = player.GetBoundingBox();
                double x = (double) boundingBox.Center.X;
                boundingBox = player.GetBoundingBox();
                double y = (double) boundingBox.Center.Y;
                Vector2 playerPosition = new Vector2((float) x, (float) y);
                Debris debris2 = new Debris(objectIndex, toolLocation, playerPosition);
                debris1.Add(debris2);
              }
            }
            else if (t != null && t.isHeavyHitter() && !(t is MeleeWeapon))
            {
              location.playSound("hammer");
              this.shakeTimer = 100;
              if (t != player.CurrentTool)
              {
                Vector2 vector2 = Vector2.Zero;
                vector2 = player.FacingDirection != 1 ? (player.FacingDirection != 3 ? (player.FacingDirection != 0 ? new Vector2(0.0f, 1f) : new Vector2(0.0f, -1f)) : new Vector2(-1f, 0.0f)) : new Vector2(1f, 0.0f);
                if ((double) this.TileLocation.X == 0.0 && (double) this.TileLocation.Y == 0.0 && location.getObjectAtTile((int) c.X, (int) c.Y) == this)
                  this.TileLocation = c;
                this.MoveToSafePosition(location, this.TileLocation, prioritize_direction: new Vector2?(vector2));
              }
            }
            this.GetMutex().ReleaseLock();
          }));
        }
        return false;
      }
      return t != null && t is Pickaxe && this.currentLidFrame == this.getLastLidFrame() && (int) (NetFieldBase<int, NetInt>) this.frameCounter == -1 && this.isEmpty();
    }

    public void addContents(int coins, Item item)
    {
      this.coins.Value += coins;
      this.items.Add(item);
    }

    public bool MoveToSafePosition(
      GameLocation location,
      Vector2 tile_position,
      int depth = 0,
      Vector2? prioritize_direction = null)
    {
      List<Vector2> list = new List<Vector2>();
      list.AddRange((IEnumerable<Vector2>) new Vector2[4]
      {
        new Vector2(1f, 0.0f),
        new Vector2(-1f, 0.0f),
        new Vector2(0.0f, -1f),
        new Vector2(0.0f, 1f)
      });
      Utility.Shuffle<Vector2>(Game1.random, list);
      if (prioritize_direction.HasValue)
      {
        list.Remove(-prioritize_direction.Value);
        list.Insert(0, -prioritize_direction.Value);
        list.Remove(prioritize_direction.Value);
        list.Insert(0, prioritize_direction.Value);
      }
      foreach (Vector2 vector2_1 in list)
      {
        Vector2 vector2_2 = tile_position + vector2_1;
        if (this.canBePlacedHere(location, vector2_2) && location.isTilePlaceable(vector2_2))
        {
          if (location.objects.ContainsKey(this.TileLocation) && !location.objects.ContainsKey(vector2_2))
          {
            location.objects.Remove(this.TileLocation);
            this.kickStartTile.Value = this.TileLocation;
            this.TileLocation = vector2_2;
            location.objects[vector2_2] = (StardewValley.Object) this;
            this.boundingBox.Value = new Microsoft.Xna.Framework.Rectangle((int) this.tileLocation.X * 64, (int) this.tileLocation.Y * 64, 64, 64);
          }
          return true;
        }
      }
      Utility.Shuffle<Vector2>(Game1.random, list);
      if (prioritize_direction.HasValue)
      {
        list.Remove(-prioritize_direction.Value);
        list.Insert(0, -prioritize_direction.Value);
        list.Remove(prioritize_direction.Value);
        list.Insert(0, prioritize_direction.Value);
      }
      if (depth < 3)
      {
        foreach (Vector2 vector2 in list)
        {
          Vector2 tile_position1 = tile_position + vector2;
          if (location.isPointPassable(new Location((int) ((double) tile_position1.X + 0.5) * 64, (int) ((double) tile_position1.Y + 0.5) * 64), Game1.viewport) && this.MoveToSafePosition(location, tile_position1, depth + 1, prioritize_direction))
            return true;
        }
      }
      return false;
    }

    public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
    {
      this.localKickStartTile = new Vector2?();
      this.kickProgress = -1f;
      return base.placementAction(location, x, y, who);
    }

    public void destroyAndDropContents(Vector2 pointToDropAt, GameLocation location)
    {
      List<Item> objList = new List<Item>();
      objList.AddRange((IEnumerable<Item>) this.items);
      if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
      {
        foreach (NetObjectList<Item> collection in this.separateWalletItems.Values)
          objList.AddRange((IEnumerable<Item>) collection);
      }
      if (objList.Count > 0)
        location.playSound("throwDownITem");
      foreach (Item obj in objList)
      {
        if (obj != null)
          Game1.createItemDebris(obj, pointToDropAt, Game1.random.Next(4), location);
      }
      this.items.Clear();
      this.separateWalletItems.Clear();
      this.clearNulls();
    }

    public void dumpContents(GameLocation location)
    {
      if (this.synchronized.Value && (this.GetMutex().IsLocked() || !Game1.IsMasterGame) && !this.GetMutex().IsLockHeld())
        return;
      if (this.items.Count > 0 && !this.chestType.Equals((object) "Monster") && this.items.Count >= 1 && (this.GetMutex().IsLockHeld() || !(bool) (NetFieldBase<bool, NetBool>) this.playerChest))
      {
        bool flag = Utility.IsNormalObjectAtParentSheetIndex(this.items[0], 434);
        if (location is FarmHouse)
        {
          if ((location as FarmHouse).owner.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Objects:ParsnipSeedPackage_SomeoneElse"));
            return;
          }
          if (!flag)
          {
            Game1.player.addQuest(6);
            Game1.dayTimeMoneyBox.PingQuestLog();
          }
        }
        if (flag)
        {
          string str = location is FarmHouse ? "CF_Spouse" : "CF_Mines";
          if (!Game1.player.mailReceived.Contains(str))
          {
            Game1.player.eatObject(this.items[0] as StardewValley.Object, true);
            Game1.player.mailReceived.Add(str);
          }
          this.items.Clear();
        }
        else if (this.dropContents.Value)
        {
          foreach (Item obj in (NetList<Item, NetRef<Item>>) this.items)
          {
            if (obj != null)
              Game1.createItemDebris(obj, this.tileLocation.Value * 64f, -1, location);
          }
          this.items.Clear();
          this.clearNulls();
          if (location is VolcanoDungeon)
          {
            if (this.bigCraftableSpriteIndex.Value == 223)
              Game1.player.team.RequestLimitedNutDrops("VolcanoNormalChest", location, (int) this.tileLocation.Value.X * 64, (int) this.tileLocation.Value.Y * 64, 1);
            else if (this.bigCraftableSpriteIndex.Value == 227)
              Game1.player.team.RequestLimitedNutDrops("VolcanoRareChest", location, (int) this.tileLocation.Value.X * 64, (int) this.tileLocation.Value.Y * 64, 1);
          }
        }
        else if (!this.synchronized.Value || this.GetMutex().IsLockHeld())
        {
          Item obj = this.items[0];
          this.items[0] = (Item) null;
          this.items.RemoveAt(0);
          Game1.player.addItemByMenuIfNecessaryElseHoldUp(obj);
          if (location is Caldera)
            Game1.player.mailReceived.Add("CalderaTreasure");
          ItemGrabMenu grab_menu = Game1.activeClickableMenu as ItemGrabMenu;
          if (grab_menu != null)
          {
            ItemGrabMenu itemGrabMenu = grab_menu;
            itemGrabMenu.behaviorBeforeCleanup = itemGrabMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (menu => grab_menu.DropRemainingItems());
          }
        }
        if (Game1.mine != null)
          Game1.mine.chestConsumed();
      }
      if (this.chestType.Equals((object) "Monster"))
      {
        Monster monsterForThisLevel = Game1.mine.getMonsterForThisLevel(Game1.CurrentMineLevel, (int) this.tileLocation.X, (int) this.tileLocation.Y);
        Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer(new Point((int) this.tileLocation.X, (int) this.tileLocation.Y), 8f, Game1.player);
        monsterForThisLevel.xVelocity = velocityTowardPlayer.X;
        monsterForThisLevel.yVelocity = velocityTowardPlayer.Y;
        location.characters.Add((NPC) monsterForThisLevel);
        location.playSound("explosion");
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), false, Game1.random.NextDouble() < 0.5));
        location.objects.Remove((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Chest.cs.12531"), Color.Red, 3500f));
      }
      else
        Game1.player.gainExperience(5, 25 + Game1.CurrentMineLevel);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.giftbox)
        return;
      TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Giftbox", new Microsoft.Xna.Framework.Rectangle(0, (int) (NetFieldBase<int, NetInt>) this.giftboxIndex * 32, 16, 32), 80f, 11, 1, this.tileLocation.Value * 64f - new Vector2(0.0f, 52f), false, false, this.tileLocation.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        destroyable = false,
        holdLastFrame = true
      };
      if (location.netObjects.ContainsKey((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation) && location.netObjects[(Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation] == this)
      {
        Game1.multiplayer.broadcastSprites(location, temporaryAnimatedSprite);
        location.removeObject((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, false);
      }
      else
        location.temporarySprites.Add(temporaryAnimatedSprite);
    }

    public NetMutex GetMutex() => this.specialChestType.Value == Chest.SpecialChestTypes.JunimoChest ? Game1.player.team.junimoChestMutex : this.mutex;

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (justCheckingForActivity)
        return true;
      if ((bool) (NetFieldBase<bool, NetBool>) this.giftbox)
      {
        Game1.player.Halt();
        Game1.player.freezePause = 1000;
        who.currentLocation.playSound("Ship");
        this.dumpContents(who.currentLocation);
      }
      else if ((bool) (NetFieldBase<bool, NetBool>) this.playerChest)
      {
        if (!Game1.didPlayerJustRightClick(true))
          return false;
        this.GetMutex().RequestLock((Action) (() =>
        {
          if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
          {
            this.OpenMiniShippingMenu();
          }
          else
          {
            this.frameCounter.Value = 5;
            Game1.playSound((bool) (NetFieldBase<bool, NetBool>) this.fridge ? "doorCreak" : "openChest");
            Game1.player.Halt();
            Game1.player.freezePause = 1000;
          }
        }));
      }
      else if (!(bool) (NetFieldBase<bool, NetBool>) this.playerChest)
      {
        if (this.currentLidFrame == this.startingLidFrame.Value && (int) (NetFieldBase<int, NetInt>) this.frameCounter <= -1)
        {
          who.currentLocation.playSound("openChest");
          if (this.synchronized.Value)
            this.GetMutex().RequestLock((Action) (() => this.openChestEvent.Fire()));
          else
            this.performOpenChest();
        }
        else if (this.currentLidFrame == this.getLastLidFrame() && this.items.Count > 0 && !this.synchronized.Value)
        {
          Item obj = this.items[0];
          this.items[0] = (Item) null;
          this.items.RemoveAt(0);
          if (Game1.mine != null)
            Game1.mine.chestConsumed();
          who.addItemByMenuIfNecessaryElseHoldUp(obj);
          ItemGrabMenu grab_menu = Game1.activeClickableMenu as ItemGrabMenu;
          if (grab_menu != null)
          {
            ItemGrabMenu itemGrabMenu = grab_menu;
            itemGrabMenu.behaviorBeforeCleanup = itemGrabMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (menu => grab_menu.DropRemainingItems());
          }
        }
      }
      if (this.items.Count == 0 && (int) (NetFieldBase<int, NetInt>) this.coins == 0 && !(bool) (NetFieldBase<bool, NetBool>) this.playerChest)
      {
        who.currentLocation.removeObject((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, false);
        who.currentLocation.playSound("woodWhack");
      }
      return true;
    }

    public virtual void OpenMiniShippingMenu()
    {
      Game1.playSound("shwip");
      this.ShowMenu();
    }

    public virtual void performOpenChest() => this.frameCounter.Value = 5;

    public virtual void grabItemFromChest(Item item, Farmer who)
    {
      if (!who.couldInventoryAcceptThisItem(item))
        return;
      this.GetItemsForPlayer(Game1.player.UniqueMultiplayerID).Remove(item);
      this.clearNulls();
      this.ShowMenu();
    }

    public virtual Item addItem(Item item)
    {
      item.resetState();
      this.clearNulls();
      NetObjectList<Item> netObjectList = this.items;
      if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin || this.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
        netObjectList = this.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);
      for (int index = 0; index < netObjectList.Count; ++index)
      {
        if (netObjectList[index] != null && netObjectList[index].canStackWith((ISalable) item))
        {
          item.Stack = netObjectList[index].addToStack(item);
          if (item.Stack <= 0)
            return (Item) null;
        }
      }
      if (netObjectList.Count >= this.GetActualCapacity())
        return item;
      netObjectList.Add(item);
      return (Item) null;
    }

    public virtual int GetActualCapacity()
    {
      if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin || this.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
        return 9;
      return this.SpecialChestType == Chest.SpecialChestTypes.Enricher ? 1 : 36;
    }

    public virtual void CheckAutoLoad(Farmer who)
    {
      if (who.currentLocation == null)
        return;
      StardewValley.Object @object = (StardewValley.Object) null;
      if (!who.currentLocation.objects.TryGetValue(new Vector2(this.TileLocation.X, this.TileLocation.Y + 1f), out @object) || @object == null)
        return;
      @object.AttemptAutoLoad(who);
    }

    public virtual void ShowMenu()
    {
      if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
        Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), false, true, new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), canBeExitedWithKey: true, source: 1, sourceItem: ((bool) (NetFieldBase<bool, NetBool>) this.fridge ? (Item) null : (Item) this), context: ((object) this));
      else if (this.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
        Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: ((bool) (NetFieldBase<bool, NetBool>) this.fridge ? (Item) null : (Item) this), context: ((object) this));
      else if (this.SpecialChestType == Chest.SpecialChestTypes.AutoLoader)
      {
        ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) this.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: ((bool) (NetFieldBase<bool, NetBool>) this.fridge ? (Item) null : (Item) this), context: ((object) this));
        itemGrabMenu.exitFunction = itemGrabMenu.exitFunction + (IClickableMenu.onExit) (() => this.CheckAutoLoad(Game1.player));
        Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
      }
      else if (this.SpecialChestType == Chest.SpecialChestTypes.Enricher)
        Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), false, true, new InventoryMenu.highlightThisItem(StardewValley.Object.HighlightFertilizers), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: ((bool) (NetFieldBase<bool, NetBool>) this.fridge ? (Item) null : (Item) this), context: ((object) this));
      else
        Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: ((bool) (NetFieldBase<bool, NetBool>) this.fridge ? (Item) null : (Item) this), context: ((object) this));
    }

    public virtual void grabItemFromInventory(Item item, Farmer who)
    {
      if (item.Stack == 0)
        item.Stack = 1;
      Item obj = this.addItem(item);
      if (obj == null)
        who.removeItemFromInventory(item);
      else
        obj = who.addItemToInventory(obj);
      this.clearNulls();
      int id = Game1.activeClickableMenu.currentlySnappedComponent != null ? Game1.activeClickableMenu.currentlySnappedComponent.myID : -1;
      this.ShowMenu();
      (Game1.activeClickableMenu as ItemGrabMenu).heldItem = obj;
      if (id == -1)
        return;
      Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(id);
      Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
    }

    public NetObjectList<Item> GetItemsForPlayer(long id)
    {
      if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin && Game1.player.team.useSeparateWallets.Value && this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin && Game1.player.team.useSeparateWallets.Value)
      {
        if (!this.separateWalletItems.ContainsKey(id))
          this.separateWalletItems[id] = new NetObjectList<Item>();
        return this.separateWalletItems[id];
      }
      return this.SpecialChestType == Chest.SpecialChestTypes.JunimoChest ? Game1.player.team.junimoChest : this.items;
    }

    public virtual bool isEmpty()
    {
      if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin && Game1.player.team.useSeparateWallets.Value)
      {
        foreach (NetObjectList<Item> source in this.separateWalletItems.Values)
        {
          for (int index = source.Count<Item>() - 1; index >= 0; --index)
          {
            if (source[index] != null)
              return false;
          }
        }
        return true;
      }
      if (this.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
      {
        NetObjectList<Item> itemsForPlayer = this.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);
        for (int index = itemsForPlayer.Count - 1; index >= 0; --index)
        {
          if (itemsForPlayer[index] != null)
            return false;
        }
        return true;
      }
      for (int index = this.items.Count - 1; index >= 0; --index)
      {
        if (this.items[index] != null)
          return false;
      }
      return true;
    }

    public virtual void clearNulls()
    {
      if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin || this.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
      {
        NetObjectList<Item> itemsForPlayer = this.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);
        for (int index = itemsForPlayer.Count - 1; index >= 0; --index)
        {
          if (itemsForPlayer[index] == null)
            itemsForPlayer.RemoveAt(index);
        }
      }
      else
      {
        for (int index = this.items.Count - 1; index >= 0; --index)
        {
          if (this.items[index] == null)
            this.items.RemoveAt(index);
        }
      }
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      if (this.synchronized.Value)
        this.openChestEvent.Poll();
      if (this.localKickStartTile.HasValue)
      {
        if (Game1.currentLocation == environment)
        {
          if ((double) this.kickProgress == 0.0)
          {
            if (Utility.isOnScreen((this.localKickStartTile.Value + new Vector2(0.5f, 0.5f)) * 64f, 64))
              Game1.playSound("clubhit");
            this.shakeTimer = 100;
          }
        }
        else
        {
          this.localKickStartTile = new Vector2?();
          this.kickProgress = -1f;
        }
        if ((double) this.kickProgress >= 0.0)
        {
          float num = 0.25f;
          this.kickProgress += (float) time.ElapsedGameTime.TotalSeconds / num;
          if ((double) this.kickProgress >= 1.0)
          {
            this.kickProgress = -1f;
            this.localKickStartTile = new Vector2?();
          }
        }
      }
      else
        this.kickProgress = -1f;
      this.fixLidFrame();
      this.mutex.Update(environment);
      if (this.shakeTimer > 0)
      {
        this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.shakeTimer <= 0)
          this.health = 10;
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.playerChest)
      {
        if (this.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
        {
          this.UpdateFarmerNearby(environment);
          if (this._shippingBinFrameCounter > -1)
          {
            --this._shippingBinFrameCounter;
            if (this._shippingBinFrameCounter <= 0)
            {
              this._shippingBinFrameCounter = 5;
              if (this._farmerNearby && this.currentLidFrame < this.getLastLidFrame())
                ++this.currentLidFrame;
              else if (!this._farmerNearby && this.currentLidFrame > this.startingLidFrame.Value)
                --this.currentLidFrame;
              else
                this._shippingBinFrameCounter = -1;
            }
          }
          if (Game1.activeClickableMenu != null || !this.GetMutex().IsLockHeld())
            return;
          this.GetMutex().ReleaseLock();
        }
        else if ((int) (NetFieldBase<int, NetInt>) this.frameCounter > -1 && this.currentLidFrame < this.getLastLidFrame() + 1)
        {
          --this.frameCounter.Value;
          if ((int) (NetFieldBase<int, NetInt>) this.frameCounter > 0 || !this.GetMutex().IsLockHeld())
            return;
          if (this.currentLidFrame == this.getLastLidFrame())
          {
            this.ShowMenu();
            this.frameCounter.Value = -1;
          }
          else
          {
            this.frameCounter.Value = 5;
            ++this.currentLidFrame;
          }
        }
        else
        {
          if (((int) (NetFieldBase<int, NetInt>) this.frameCounter != -1 || this.currentLidFrame <= (int) (NetFieldBase<int, NetInt>) this.startingLidFrame) && this.currentLidFrame < this.getLastLidFrame() || Game1.activeClickableMenu != null || !this.GetMutex().IsLockHeld())
            return;
          this.GetMutex().ReleaseLock();
          this.currentLidFrame = this.getLastLidFrame();
          this.frameCounter.Value = 2;
          environment.localSound("doorCreakReverse");
        }
      }
      else
      {
        if ((int) (NetFieldBase<int, NetInt>) this.frameCounter <= -1 || this.currentLidFrame > this.getLastLidFrame())
          return;
        --this.frameCounter.Value;
        if ((int) (NetFieldBase<int, NetInt>) this.frameCounter > 0)
          return;
        if (this.currentLidFrame == this.getLastLidFrame())
        {
          this.dumpContents(environment);
          this.frameCounter.Value = -1;
        }
        else
        {
          this.frameCounter.Value = 10;
          ++this.currentLidFrame;
          if (this.currentLidFrame != this.getLastLidFrame())
            return;
          this.frameCounter.Value += 5;
        }
      }
    }

    public virtual void UpdateFarmerNearby(GameLocation location, bool animate = true)
    {
      bool flag = false;
      foreach (Farmer farmer in location.farmers)
      {
        if ((double) Math.Abs((float) farmer.getTileX() - this.tileLocation.X) <= 1.0 && (double) Math.Abs((float) farmer.getTileY() - this.tileLocation.Y) <= 1.0)
        {
          flag = true;
          break;
        }
      }
      if (flag == this._farmerNearby)
        return;
      this._farmerNearby = flag;
      this._shippingBinFrameCounter = 5;
      if (!animate)
      {
        this._shippingBinFrameCounter = -1;
        if (this._farmerNearby)
          this.currentLidFrame = this.getLastLidFrame();
        else
          this.currentLidFrame = this.startingLidFrame.Value;
      }
      else
      {
        if (Game1.gameMode == (byte) 6)
          return;
        if (this._farmerNearby)
          location.localSound("doorCreak");
        else
          location.localSound("doorCreakReverse");
      }
    }

    public override void actionOnPlayerEntry()
    {
      this.fixLidFrame();
      if (this.specialChestType.Value == Chest.SpecialChestTypes.MiniShippingBin)
        this.UpdateFarmerNearby(Game1.currentLocation, false);
      this.kickProgress = -1f;
      this.localKickStartTile = new Vector2?();
      if ((bool) (NetFieldBase<bool, NetBool>) this.playerChest || this.items.Count != 0 || (int) (NetFieldBase<int, NetInt>) this.coins != 0)
        return;
      this.currentLidFrame = this.getLastLidFrame();
    }

    public virtual void SetBigCraftableSpriteIndex(
      int sprite_index,
      int starting_lid_frame = -1,
      int lid_frame_count = 3)
    {
      this.bigCraftableSpriteIndex.Value = sprite_index;
      if (starting_lid_frame >= 0)
        this.startingLidFrame.Value = starting_lid_frame;
      else
        this.startingLidFrame.Value = sprite_index + 1;
      this.lidFrameCount.Value = lid_frame_count;
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow)
    {
      base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      float b1 = (float) x;
      float b2 = (float) y;
      if (this.localKickStartTile.HasValue)
      {
        b1 = Utility.Lerp(this.localKickStartTile.Value.X, b1, this.kickProgress);
        b2 = Utility.Lerp(this.localKickStartTile.Value.Y, b2, this.kickProgress);
      }
      float layerDepth = Math.Max(0.0f, (float) ((((double) b2 + 1.0) * 64.0 - 24.0) / 10000.0)) + b1 * 1E-05f;
      if (this.localKickStartTile.HasValue)
      {
        spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (((double) b1 + 0.5) * 64.0), (float) (((double) b2 + 0.5) * 64.0))), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.Black * 0.5f, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 0.0001f);
        b2 -= (float) Math.Sin((double) this.kickProgress * Math.PI) * 0.5f;
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.playerChest && (this.ParentSheetIndex == 130 || this.ParentSheetIndex == 232))
      {
        if (this.playerChoiceColor.Value.Equals(Color.Black))
        {
          spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) b1 * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)), (float) (((double) b2 - 1.0) * 64.0))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.ParentSheetIndex, 16, 32)), this.tint.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
          spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) b1 * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)), (float) (((double) b2 - 1.0) * 64.0))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.currentLidFrame, 16, 32)), this.tint.Value * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
        }
        else
        {
          spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(b1 * 64f, (float) (((double) b2 - 1.0) * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.ParentSheetIndex == 130 ? 168 : this.ParentSheetIndex, 16, 32)), this.playerChoiceColor.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
          spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(b1 * 64f, (float) ((double) b2 * 64.0 + 20.0))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, (this.ParentSheetIndex == 130 ? 168 : this.ParentSheetIndex) / 8 * 32 + 53, 16, 11)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 2E-05f);
          spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(b1 * 64f, (float) (((double) b2 - 1.0) * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.ParentSheetIndex == 130 ? this.currentLidFrame + 46 : this.currentLidFrame + 8, 16, 32)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 2E-05f);
          spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(b1 * 64f, (float) (((double) b2 - 1.0) * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.ParentSheetIndex == 130 ? this.currentLidFrame + 38 : this.currentLidFrame, 16, 32)), this.playerChoiceColor.Value * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
        }
      }
      else if ((bool) (NetFieldBase<bool, NetBool>) this.playerChest)
      {
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) b1 * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)), (float) (((double) b2 - 1.0) * 64.0))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.ParentSheetIndex, 16, 32)), this.tint.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) b1 * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)), (float) (((double) b2 - 1.0) * 64.0))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.currentLidFrame, 16, 32)), this.tint.Value * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
      }
      else if ((bool) (NetFieldBase<bool, NetBool>) this.giftbox)
      {
        spriteBatch.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(16f, 53f), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 5f, SpriteEffects.None, 1E-07f);
        if (this.items.Count <= 0 && (int) (NetFieldBase<int, NetInt>) this.coins <= 0)
          return;
        int y1 = (int) (NetFieldBase<int, NetInt>) this.giftboxIndex * 32;
        spriteBatch.Draw(Game1.giftboxTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) b1 * 64.0 + (this.shakeTimer > 0 ? (double) Game1.random.Next(-1, 2) : 0.0)), (float) ((double) b2 * 64.0 - 52.0))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y1, 16, 32)), (Color) (NetFieldBase<Color, NetColor>) this.tint, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
      }
      else
      {
        int tilePosition = 500;
        Texture2D texture2D = Game1.objectSpriteSheet;
        int height = 16;
        int num = 0;
        if (this.bigCraftableSpriteIndex.Value >= 0)
        {
          tilePosition = this.bigCraftableSpriteIndex.Value;
          texture2D = Game1.bigCraftableSpriteSheet;
          height = 32;
          num = -64;
        }
        if (this.bigCraftableSpriteIndex.Value < 0)
          spriteBatch.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(16f, 53f), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 5f, SpriteEffects.None, 1E-07f);
        spriteBatch.Draw(texture2D, Game1.GlobalToLocal(Game1.viewport, new Vector2(b1 * 64f, b2 * 64f + (float) num)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(texture2D, tilePosition, 16, height)), (Color) (NetFieldBase<Color, NetColor>) this.tint, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
        Vector2 globalPosition = new Vector2(b1 * 64f, b2 * 64f + (float) num);
        if (this.bigCraftableSpriteIndex.Value < 0)
        {
          switch (this.currentLidFrame)
          {
            case 501:
              globalPosition.Y -= 32f;
              break;
            case 502:
              globalPosition.Y -= 40f;
              break;
            case 503:
              globalPosition.Y -= 60f;
              break;
          }
        }
        spriteBatch.Draw(texture2D, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(texture2D, this.currentLidFrame, 16, height)), (Color) (NetFieldBase<Color, NetColor>) this.tint, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
      }
    }

    public virtual void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f, bool local = false)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.playerChest)
        return;
      if (this.playerChoiceColor.Equals(Color.Black))
      {
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, local ? new Vector2((float) x, (float) (y - 64)) : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) ((y - 1) * 64))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 16, 32)), this.tint.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.89f : (float) (y * 64 + 4) / 10000f);
      }
      else
      {
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, local ? new Vector2((float) x, (float) (y - 64)) : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) ((y - 1) * 64 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.ParentSheetIndex == 130 ? 168 : this.ParentSheetIndex, 16, 32)), this.playerChoiceColor.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.9f : (float) (y * 64 + 4) / 10000f);
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, local ? new Vector2((float) x, (float) (y - 64)) : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) ((y - 1) * 64 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.ParentSheetIndex == 130 ? this.currentLidFrame + 38 : this.currentLidFrame, 16, 32)), this.playerChoiceColor.Value * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.9f : (float) (y * 64 + 5) / 10000f);
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, local ? new Vector2((float) x, (float) (y + 20)) : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 + 20))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, (this.ParentSheetIndex == 130 ? 168 : this.ParentSheetIndex) / 8 * 32 + 53, 16, 11)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.91f : (float) (y * 64 + 6) / 10000f);
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, local ? new Vector2((float) x, (float) (y - 64)) : Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) ((y - 1) * 64 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.ParentSheetIndex == 130 ? this.currentLidFrame + 46 : this.currentLidFrame + 8, 16, 32)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.91f : (float) (y * 64 + 6) / 10000f);
      }
    }

    public enum SpecialChestTypes
    {
      None,
      MiniShippingBin,
      JunimoChest,
      AutoLoader,
      Enricher,
    }
  }
}
