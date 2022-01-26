// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Cabin
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class Cabin : FarmHouse
  {
    private static Random farmhandIDRandom = new Random();
    [XmlElement("farmhand")]
    public readonly NetRef<Farmer> farmhand = new NetRef<Farmer>();
    [XmlIgnore]
    public readonly NetMutex inventoryMutex = new NetMutex();

    [XmlIgnore]
    public override Farmer owner => this.getFarmhand().Value.isActive() ? Game1.otherFarmers[this.getFarmhand().Value.UniqueMultiplayerID] : this.getFarmhand().Value;

    [XmlIgnore]
    public override int upgradeLevel
    {
      get => this.farmhand.Value == null ? 0 : (int) (NetFieldBase<int, NetInt>) this.owner.houseUpgradeLevel;
      set => this.owner.houseUpgradeLevel.Value = value;
    }

    public Cabin()
    {
    }

    public Cabin(string map)
      : base(map, nameof (Cabin))
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.farmhand, (INetSerializable) this.inventoryMutex.NetFields);
    }

    public NetRef<Farmer> getFarmhand()
    {
      if (this.farmhand.Value == null)
      {
        this.farmhand.Value = new Farmer(new FarmerSprite((string) null), new Vector2(0.0f, 0.0f), 1, "", Farmer.initialTools(), true);
        this.farmhand.Value.UniqueMultiplayerID = Utility.RandomLong(Cabin.farmhandIDRandom);
        this.farmhand.Value.questLog.Add((Quest) (Quest.getQuestFromId(9) as SocializeQuest));
        this.resetFarmhandState();
      }
      return this.farmhand;
    }

    public void resetFarmhandState()
    {
      if (this.farmhand.Value == null)
        return;
      this.farmhand.Value.farmName.Value = Game1.MasterPlayer.farmName.Value;
      this.farmhand.Value.homeLocation.Value = (string) (NetFieldBase<string, NetString>) this.uniqueName;
      if (this.farmhand.Value.lastSleepLocation.Value == null || this.farmhand.Value.lastSleepLocation.Value == (string) (NetFieldBase<string, NetString>) this.uniqueName)
      {
        this.farmhand.Value.currentLocation = (GameLocation) this;
        this.farmhand.Value.Position = Utility.PointToVector2(this.GetPlayerBedSpot()) * 64f;
      }
      this.farmhand.Value.resetState();
    }

    public void saveFarmhand(NetFarmerRoot farmhand)
    {
      farmhand.CloneInto(this.farmhand);
      this.resetFarmhandState();
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.map.GetLayer("Buildings").Tiles[tileLocation] != null)
      {
        switch (this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex)
        {
          case 647:
          case 648:
            if (!this.getFarmhand().Value.isActive())
            {
              this.inventoryMutex.RequestLock((Action) (() =>
              {
                this.playSound("Ship");
                this.openFarmhandInventory();
              }));
              return true;
            }
            break;
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
    {
      base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
      this.inventoryMutex.Update(Game1.getOnlineFarmers());
      if (!this.inventoryMutex.IsLockHeld() || Game1.activeClickableMenu is ItemGrabMenu)
        return;
      this.inventoryMutex.ReleaseLock();
    }

    public NetObjectList<Item> getInventory() => this.getFarmhand().Value.items;

    public void openFarmhandInventory() => Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.getInventory(), false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromPlayerInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromFarmhandInventory), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, context: ((object) this));

    public bool isInventoryOpen() => this.inventoryMutex.IsLocked();

    private void grabItemFromPlayerInventory(Item item, Farmer who)
    {
      if (item.Stack == 0)
        item.Stack = 1;
      Item inventory = this.getFarmhand().Value.addItemToInventory(item);
      if (inventory == null)
        who.removeItemFromInventory(item);
      else
        who.addItemToInventory(inventory);
      int id = Game1.activeClickableMenu.currentlySnappedComponent != null ? Game1.activeClickableMenu.currentlySnappedComponent.myID : -1;
      this.openFarmhandInventory();
      if (id == -1)
        return;
      Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(id);
      Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
    }

    private void grabItemFromFarmhandInventory(Item item, Farmer who)
    {
      if (!who.couldInventoryAcceptThisItem(item))
        return;
      this.getInventory().Remove(item);
      this.openFarmhandInventory();
    }

    public override void updateWarps()
    {
      base.updateWarps();
      if (Game1.getFarm() == null)
        return;
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building.indoors.Value == this)
          building.updateInteriorWarps();
      }
    }

    public List<Item> demolish()
    {
      List<Item> list = new List<Item>((IEnumerable<Item>) this.getInventory()).Where<Item>((Func<Item, bool>) (item => item != null)).ToList<Item>();
      this.getInventory().Clear();
      Farmer.removeInitialTools(list);
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      foreach (NPC character in new List<NPC>((IEnumerable<NPC>) this.characters))
      {
        if (character.isVillager() && dictionary.ContainsKey(character.Name))
        {
          character.reloadDefaultLocation();
          character.clearSchedule();
          Game1.warpCharacter(character, character.DefaultMap, character.DefaultPosition / 64f);
        }
        if (character is Pet)
          (character as Pet).warpToFarmHouse(Game1.MasterPlayer);
      }
      if (Game1.getLocationFromName(this.GetCellarName()) is Cellar locationFromName)
      {
        locationFromName.objects.Clear();
        locationFromName.setUpAgingBoards();
      }
      if (this.farmhand.Value != null)
        Game1.player.team.DeleteFarmhand(this.farmhand.Value);
      Game1.updateCellarAssignments();
      return list;
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      if (this.farmhand.Value == null)
        return;
      this.farmhand.Value.stamina = (float) this.farmhand.Value.maxStamina.Value;
    }

    public override Point getPorchStandingSpot()
    {
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building.isCabin && this == building.indoors.Value)
          return building.getPorchStandingSpot();
      }
      return base.getPorchStandingSpot();
    }
  }
}
