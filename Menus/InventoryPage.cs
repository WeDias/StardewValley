// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.InventoryPage
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class InventoryPage : IClickableMenu
  {
    public const int region_inventory = 100;
    public const int region_hat = 101;
    public const int region_ring1 = 102;
    public const int region_ring2 = 103;
    public const int region_boots = 104;
    public const int region_trashCan = 105;
    public const int region_organizeButton = 106;
    public const int region_accessory = 107;
    public const int region_shirt = 108;
    public const int region_pants = 109;
    public const int region_shoes = 110;
    public InventoryMenu inventory;
    private string descriptionText = "";
    private string hoverText = "";
    private string descriptionTitle = "";
    private string hoverTitle = "";
    private int hoverAmount;
    private Item hoveredItem;
    private Item itemToHold;
    public List<ClickableComponent> equipmentIcons = new List<ClickableComponent>();
    public ClickableComponent portrait;
    public ClickableTextureComponent trashCan;
    public ClickableTextureComponent organizeButton;
    private float trashCanLidRotation;
    public ClickableTextureComponent junimoNoteIcon;
    private int junimoNotePulser;

    public InventoryPage(int x, int y, int width, int height)
      : base(x, y, width, height)
    {
      this.inventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth, true);
      this.equipmentIcons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 48, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 + 256 - 12, 64, 64), "Left Ring")
      {
        myID = 102,
        downNeighborID = 103,
        upNeighborID = Game1.player.MaxItems - 12,
        rightNeighborID = 101,
        fullyImmutable = true
      });
      this.equipmentIcons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 48, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 + 320 - 12, 64, 64), "Right Ring")
      {
        myID = 103,
        upNeighborID = 102,
        downNeighborID = 104,
        rightNeighborID = 108,
        fullyImmutable = true
      });
      this.equipmentIcons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 48, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 + 384 - 12, 64, 64), "Boots")
      {
        myID = 104,
        upNeighborID = 103,
        rightNeighborID = 109,
        fullyImmutable = true
      });
      this.portrait = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 192 - 8 - 64 + 32, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 - 8 + 64, 64, 96), "32");
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width / 3 + 576 + 32, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192 + 64, 64, 104), Game1.mouseCursors, new Rectangle(564 + Game1.player.trashCanLevel * 18, 102, 18, 26), 4f);
      textureComponent1.myID = 105;
      textureComponent1.upNeighborID = 106;
      textureComponent1.leftNeighborID = 101;
      this.trashCan = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + width, this.yPositionOnScreen + height / 3 - 64 + 8, 64, 64), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"), Game1.mouseCursors, new Rectangle(162, 440, 16, 16), 4f);
      textureComponent2.myID = 106;
      textureComponent2.downNeighborID = 105;
      textureComponent2.leftNeighborID = 11;
      textureComponent2.upNeighborID = 898;
      this.organizeButton = textureComponent2;
      this.equipmentIcons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 48 + 208, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 + 256 - 12, 64, 64), "Hat")
      {
        myID = 101,
        leftNeighborID = 102,
        downNeighborID = 108,
        upNeighborID = Game1.player.MaxItems - 12,
        rightNeighborID = 105,
        fullyImmutable = true
      });
      this.equipmentIcons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 48 + 208, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 + 320 - 12, 64, 64), "Shirt")
      {
        myID = 108,
        upNeighborID = 101,
        downNeighborID = 109,
        rightNeighborID = 105,
        leftNeighborID = 103,
        fullyImmutable = true
      });
      this.equipmentIcons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 48 + 208, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 + 384 - 12, 64, 64), "Pants")
      {
        myID = 109,
        upNeighborID = 108,
        rightNeighborID = 105,
        leftNeighborID = 104,
        fullyImmutable = true
      });
      if (!InventoryPage.ShouldShowJunimoNoteIcon())
        return;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + width, this.yPositionOnScreen + 96, 64, 64), "", Game1.content.LoadString("Strings\\UI:GameMenu_JunimoNote_Hover"), Game1.mouseCursors, new Rectangle(331, 374, 15, 14), 4f);
      textureComponent3.myID = 898;
      textureComponent3.leftNeighborID = 11;
      textureComponent3.downNeighborID = 106;
      this.junimoNoteIcon = textureComponent3;
    }

    public static bool ShouldShowJunimoNoteIcon()
    {
      if (!Game1.player.hasOrWillReceiveMail("canReadJunimoText") || Game1.player.hasOrWillReceiveMail("JojaMember"))
        return false;
      if (!Game1.MasterPlayer.hasCompletedCommunityCenter())
        return true;
      return Game1.player.hasOrWillReceiveMail("hasSeenAbandonedJunimoNote") && !Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater");
    }

    protected virtual bool checkHeldItem(Func<Item, bool> f = null) => f == null ? Game1.player.CursorSlotItem != null : f(Game1.player.CursorSlotItem);

    protected virtual Item takeHeldItem()
    {
      Item cursorSlotItem = Game1.player.CursorSlotItem;
      Game1.player.CursorSlotItem = (Item) null;
      return cursorSlotItem;
    }

    protected virtual void setHeldItem(Item item)
    {
      if (item != null)
        item.NetFields.Parent = (INetSerializable) null;
      Game1.player.CursorSlotItem = item;
    }

    public override void receiveKeyPress(Keys key)
    {
      base.receiveKeyPress(key);
      if (Game1.isAnyGamePadButtonBeingPressed() && Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.checkHeldItem())
        Game1.setMousePosition(this.trashCan.bounds.Center);
      if (key.Equals((object) Keys.Delete) && this.checkHeldItem((Func<Item, bool>) (i => i != null && i.canBeTrashed())))
        Utility.trashItem(this.takeHeldItem());
      if (Game1.options.doesInputListContain(Game1.options.inventorySlot1, key))
      {
        Game1.player.CurrentToolIndex = 0;
        Game1.playSound("toolSwap");
      }
      else if (Game1.options.doesInputListContain(Game1.options.inventorySlot2, key))
      {
        Game1.player.CurrentToolIndex = 1;
        Game1.playSound("toolSwap");
      }
      else if (Game1.options.doesInputListContain(Game1.options.inventorySlot3, key))
      {
        Game1.player.CurrentToolIndex = 2;
        Game1.playSound("toolSwap");
      }
      else if (Game1.options.doesInputListContain(Game1.options.inventorySlot4, key))
      {
        Game1.player.CurrentToolIndex = 3;
        Game1.playSound("toolSwap");
      }
      else if (Game1.options.doesInputListContain(Game1.options.inventorySlot5, key))
      {
        Game1.player.CurrentToolIndex = 4;
        Game1.playSound("toolSwap");
      }
      else if (Game1.options.doesInputListContain(Game1.options.inventorySlot6, key))
      {
        Game1.player.CurrentToolIndex = 5;
        Game1.playSound("toolSwap");
      }
      else if (Game1.options.doesInputListContain(Game1.options.inventorySlot7, key))
      {
        Game1.player.CurrentToolIndex = 6;
        Game1.playSound("toolSwap");
      }
      else if (Game1.options.doesInputListContain(Game1.options.inventorySlot8, key))
      {
        Game1.player.CurrentToolIndex = 7;
        Game1.playSound("toolSwap");
      }
      else if (Game1.options.doesInputListContain(Game1.options.inventorySlot9, key))
      {
        Game1.player.CurrentToolIndex = 8;
        Game1.playSound("toolSwap");
      }
      else if (Game1.options.doesInputListContain(Game1.options.inventorySlot10, key))
      {
        Game1.player.CurrentToolIndex = 9;
        Game1.playSound("toolSwap");
      }
      else if (Game1.options.doesInputListContain(Game1.options.inventorySlot11, key))
      {
        Game1.player.CurrentToolIndex = 10;
        Game1.playSound("toolSwap");
      }
      else
      {
        if (!Game1.options.doesInputListContain(Game1.options.inventorySlot12, key))
          return;
        Game1.player.CurrentToolIndex = 11;
        Game1.playSound("toolSwap");
      }
    }

    public override void setUpForGamePadMode()
    {
      base.setUpForGamePadMode();
      if (this.inventory != null)
        this.inventory.setUpForGamePadMode();
      this.currentRegion = 100;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      foreach (ClickableComponent equipmentIcon in this.equipmentIcons)
      {
        if (equipmentIcon.containsPoint(x, y))
        {
          bool flag = !this.checkHeldItem();
          string name = equipmentIcon.name;
          if (!(name == "Hat"))
          {
            if (!(name == "Left Ring"))
            {
              if (!(name == "Right Ring"))
              {
                if (!(name == "Boots"))
                {
                  if (!(name == "Shirt"))
                  {
                    if (name == "Pants" && this.checkHeldItem((Func<Item, bool>) (i =>
                    {
                      if (i == null || i is Clothing && (i as Clothing).clothesType.Value == 1)
                        return true;
                      return i is StardewValley.Object && (int) (NetFieldBase<int, NetInt>) i.parentSheetIndex == 71;
                    })))
                    {
                      Clothing clothing = !(Game1.player.CursorSlotItem is StardewValley.Object) || (int) (NetFieldBase<int, NetInt>) Game1.player.CursorSlotItem.parentSheetIndex != 71 ? (Clothing) this.takeHeldItem() : new Clothing(15);
                      this.setHeldItem(Utility.PerformSpecialItemGrabReplacement((Item) (Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) Game1.player.pantsItem));
                      Game1.player.pantsItem.Value = clothing;
                      if (Game1.player.pantsItem.Value != null)
                        Game1.playSound("sandyStep");
                      else if (this.checkHeldItem())
                        Game1.playSound("dwop");
                    }
                  }
                  else if (this.checkHeldItem((Func<Item, bool>) (i =>
                  {
                    if (i == null)
                      return true;
                    return i is Clothing && (i as Clothing).clothesType.Value == 0;
                  })))
                  {
                    Clothing heldItem = (Clothing) this.takeHeldItem();
                    this.setHeldItem(Utility.PerformSpecialItemGrabReplacement((Item) (Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) Game1.player.shirtItem));
                    Game1.player.shirtItem.Value = heldItem;
                    if (Game1.player.shirtItem.Value != null)
                      Game1.playSound("sandyStep");
                    else if (this.checkHeldItem())
                      Game1.playSound("dwop");
                  }
                }
                else if (this.checkHeldItem((Func<Item, bool>) (i => i == null || i is Boots)))
                {
                  Boots heldItem = (Boots) this.takeHeldItem();
                  if (Game1.player.boots.Value != null)
                    Game1.player.boots.Value.onUnequip();
                  this.setHeldItem((Item) (Boots) (NetFieldBase<Boots, NetRef<Boots>>) Game1.player.boots);
                  Game1.player.boots.Value = heldItem;
                  if (Game1.player.boots.Value != null)
                  {
                    Game1.player.boots.Value.onEquip();
                    Game1.playSound("sandyStep");
                    DelayedAction.playSoundAfterDelay("sandyStep", 150);
                  }
                  else if (this.checkHeldItem())
                    Game1.playSound("dwop");
                }
              }
              else if (this.checkHeldItem((Func<Item, bool>) (i => i == null || i is Ring)))
              {
                Ring heldItem = (Ring) this.takeHeldItem();
                if (Game1.player.rightRing.Value != null)
                  Game1.player.rightRing.Value.onUnequip(Game1.player, Game1.currentLocation);
                this.setHeldItem((Item) (Ring) (NetFieldBase<Ring, NetRef<Ring>>) Game1.player.rightRing);
                Game1.player.rightRing.Value = heldItem;
                if (Game1.player.rightRing.Value != null)
                {
                  Game1.player.rightRing.Value.onEquip(Game1.player, Game1.currentLocation);
                  Game1.playSound("crit");
                }
                else if (this.checkHeldItem())
                  Game1.playSound("dwop");
              }
            }
            else if (this.checkHeldItem((Func<Item, bool>) (i => i == null || i is Ring)))
            {
              Ring heldItem = (Ring) this.takeHeldItem();
              if (Game1.player.leftRing.Value != null)
                Game1.player.leftRing.Value.onUnequip(Game1.player, Game1.currentLocation);
              this.setHeldItem((Item) (Ring) (NetFieldBase<Ring, NetRef<Ring>>) Game1.player.leftRing);
              Game1.player.leftRing.Value = heldItem;
              if (Game1.player.leftRing.Value != null)
              {
                Game1.player.leftRing.Value.onEquip(Game1.player, Game1.currentLocation);
                Game1.playSound("crit");
              }
              else if (this.checkHeldItem())
                Game1.playSound("dwop");
            }
          }
          else if (this.checkHeldItem((Func<Item, bool>) (i =>
          {
            switch (i)
            {
              case null:
              case Hat _:
                return true;
              default:
                return i is Pan;
            }
          })))
          {
            Hat hat = Game1.player.CursorSlotItem is Pan ? new Hat(71) : (Hat) this.takeHeldItem();
            this.setHeldItem(Utility.PerformSpecialItemGrabReplacement((Item) (Hat) (NetFieldBase<Hat, NetRef<Hat>>) Game1.player.hat));
            Game1.player.hat.Value = hat;
            if (Game1.player.hat.Value != null)
              Game1.playSound("grassyStep");
            else if (this.checkHeldItem())
              Game1.playSound("dwop");
          }
          if (flag && this.checkHeldItem() && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
          {
            for (int i = 0; i < Game1.player.items.Count; i++)
            {
              if (Game1.player.items[i] == null || this.checkHeldItem((Func<Item, bool>) (item => Game1.player.items[i].canStackWith((ISalable) item))))
              {
                if (Game1.player.CurrentToolIndex == i && this.checkHeldItem())
                  Game1.player.CursorSlotItem.actionWhenBeingHeld(Game1.player);
                this.setHeldItem(Utility.addItemToInventory(this.takeHeldItem(), i, this.inventory.actualInventory));
                if (Game1.player.CurrentToolIndex == i && this.checkHeldItem())
                  Game1.player.CursorSlotItem.actionWhenStopBeingHeld(Game1.player);
                Game1.playSound("stoneStep");
                return;
              }
            }
          }
        }
      }
      this.setHeldItem(this.inventory.leftClick(x, y, this.takeHeldItem(), !Game1.oldKBState.IsKeyDown(Keys.LeftShift)));
      if (this.checkHeldItem((Func<Item, bool>) (i => i != null && Utility.IsNormalObjectAtParentSheetIndex(i, 434))))
      {
        Game1.playSound("smallSelect");
        Game1.player.eatObject(this.takeHeldItem() as StardewValley.Object, true);
        Game1.exitActiveMenu();
      }
      else if (this.checkHeldItem() && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
      {
        if (this.checkHeldItem((Func<Item, bool>) (i => i is Ring)))
        {
          if (Game1.player.leftRing.Value == null)
          {
            Game1.player.leftRing.Value = this.takeHeldItem() as Ring;
            Game1.player.leftRing.Value.onEquip(Game1.player, Game1.currentLocation);
            Game1.playSound("crit");
            return;
          }
          if (Game1.player.rightRing.Value == null)
          {
            Game1.player.rightRing.Value = this.takeHeldItem() as Ring;
            Game1.player.rightRing.Value.onEquip(Game1.player, Game1.currentLocation);
            Game1.playSound("crit");
            return;
          }
        }
        else if (this.checkHeldItem((Func<Item, bool>) (i => i is Hat)))
        {
          if (Game1.player.hat.Value == null)
          {
            Game1.player.hat.Value = this.takeHeldItem() as Hat;
            Game1.playSound("grassyStep");
            return;
          }
        }
        else if (this.checkHeldItem((Func<Item, bool>) (i => i is Boots)))
        {
          if (Game1.player.boots.Value == null)
          {
            Game1.player.boots.Value = this.takeHeldItem() as Boots;
            Game1.player.boots.Value.onEquip();
            Game1.playSound("sandyStep");
            DelayedAction.playSoundAfterDelay("sandyStep", 150);
            return;
          }
        }
        else if (this.checkHeldItem((Func<Item, bool>) (i => i is Clothing && (i as Clothing).clothesType.Value == 0)))
        {
          if (Game1.player.shirtItem.Value == null)
          {
            Game1.player.shirtItem.Value = this.takeHeldItem() as Clothing;
            Game1.playSound("sandyStep");
            DelayedAction.playSoundAfterDelay("sandyStep", 150);
            return;
          }
        }
        else if (this.checkHeldItem((Func<Item, bool>) (i => i is Clothing && (i as Clothing).clothesType.Value == 1)) && Game1.player.pantsItem.Value == null)
        {
          Game1.player.pantsItem.Value = this.takeHeldItem() as Clothing;
          Game1.playSound("sandyStep");
          DelayedAction.playSoundAfterDelay("sandyStep", 150);
          return;
        }
        if (this.inventory.getInventoryPositionOfClick(x, y) >= 12)
        {
          for (int i = 0; i < 12; i++)
          {
            if (Game1.player.items[i] == null || this.checkHeldItem((Func<Item, bool>) (item => Game1.player.items[i].canStackWith((ISalable) item))))
            {
              if (Game1.player.CurrentToolIndex == i && this.checkHeldItem())
                Game1.player.CursorSlotItem.actionWhenBeingHeld(Game1.player);
              this.setHeldItem(Utility.addItemToInventory(this.takeHeldItem(), i, this.inventory.actualInventory));
              if (this.checkHeldItem())
                Game1.player.CursorSlotItem.actionWhenStopBeingHeld(Game1.player);
              Game1.playSound("stoneStep");
              return;
            }
          }
        }
        else if (this.inventory.getInventoryPositionOfClick(x, y) < 12)
        {
          for (int i = 12; i < Game1.player.items.Count; i++)
          {
            if (Game1.player.items[i] == null || this.checkHeldItem((Func<Item, bool>) (item => Game1.player.items[i].canStackWith((ISalable) item))))
            {
              if (Game1.player.CurrentToolIndex == i && this.checkHeldItem())
                Game1.player.CursorSlotItem.actionWhenBeingHeld(Game1.player);
              this.setHeldItem(Utility.addItemToInventory(this.takeHeldItem(), i, this.inventory.actualInventory));
              if (this.checkHeldItem())
                Game1.player.CursorSlotItem.actionWhenStopBeingHeld(Game1.player);
              Game1.playSound("stoneStep");
              return;
            }
          }
        }
      }
      if (this.portrait.containsPoint(x, y))
        this.portrait.name = this.portrait.name.Equals("32") ? "8" : "32";
      if (this.trashCan.containsPoint(x, y) && this.checkHeldItem((Func<Item, bool>) (i => i != null && i.canBeTrashed())))
      {
        Utility.trashItem(this.takeHeldItem());
        if (Game1.options.SnappyMenus)
          this.snapCursorToCurrentSnappedComponent();
      }
      else if (!this.isWithinBounds(x, y) && this.checkHeldItem((Func<Item, bool>) (i => i != null && i.canBeTrashed())))
      {
        Game1.playSound("throwDownITem");
        Game1.createItemDebris(this.takeHeldItem(), Game1.player.getStandingPosition(), Game1.player.FacingDirection).DroppedByPlayerID.Value = Game1.player.UniqueMultiplayerID;
      }
      if (this.organizeButton != null && this.organizeButton.containsPoint(x, y))
      {
        ItemGrabMenu.organizeItemsInList((IList<Item>) Game1.player.items);
        Game1.playSound("Ship");
      }
      if (this.junimoNoteIcon == null || !this.junimoNoteIcon.containsPoint(x, y) || !this.readyToClose())
        return;
      Game1.activeClickableMenu = (IClickableMenu) new JunimoNoteMenu(true);
    }

    public override void receiveGamePadButton(Buttons b)
    {
      if (b != Buttons.Back || this.organizeButton == null)
        return;
      ItemGrabMenu.organizeItemsInList((IList<Item>) Game1.player.items);
      Game1.playSound("Ship");
    }

    public override void receiveRightClick(int x, int y, bool playSound = true) => this.setHeldItem(this.inventory.rightClick(x, y, this.takeHeldItem()));

    public override void performHoverAction(int x, int y)
    {
      this.hoverAmount = -1;
      this.descriptionText = "";
      this.descriptionTitle = "";
      this.hoveredItem = this.inventory.hover(x, y, Game1.player.CursorSlotItem);
      this.hoverText = this.inventory.hoverText;
      this.hoverTitle = this.inventory.hoverTitle;
      foreach (ClickableComponent equipmentIcon in this.equipmentIcons)
      {
        if (equipmentIcon.containsPoint(x, y))
        {
          string name = equipmentIcon.name;
          if (!(name == "Hat"))
          {
            if (!(name == "Right Ring"))
            {
              if (!(name == "Left Ring"))
              {
                if (!(name == "Boots"))
                {
                  if (!(name == "Shirt"))
                  {
                    if (name == "Pants" && Game1.player.pantsItem.Value != null)
                    {
                      this.hoveredItem = (Item) (Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) Game1.player.pantsItem;
                      this.hoverText = Game1.player.pantsItem.Value.getDescription();
                      this.hoverTitle = Game1.player.pantsItem.Value.DisplayName;
                    }
                  }
                  else if (Game1.player.shirtItem.Value != null)
                  {
                    this.hoveredItem = (Item) (Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) Game1.player.shirtItem;
                    this.hoverText = Game1.player.shirtItem.Value.getDescription();
                    this.hoverTitle = Game1.player.shirtItem.Value.DisplayName;
                  }
                }
                else if (Game1.player.boots.Value != null)
                {
                  this.hoveredItem = (Item) (Boots) (NetFieldBase<Boots, NetRef<Boots>>) Game1.player.boots;
                  this.hoverText = Game1.player.boots.Value.getDescription();
                  this.hoverTitle = Game1.player.boots.Value.DisplayName;
                }
              }
              else if (Game1.player.leftRing.Value != null)
              {
                this.hoveredItem = (Item) (Ring) (NetFieldBase<Ring, NetRef<Ring>>) Game1.player.leftRing;
                this.hoverText = Game1.player.leftRing.Value.getDescription();
                this.hoverTitle = Game1.player.leftRing.Value.DisplayName;
              }
            }
            else if (Game1.player.rightRing.Value != null)
            {
              this.hoveredItem = (Item) (Ring) (NetFieldBase<Ring, NetRef<Ring>>) Game1.player.rightRing;
              this.hoverText = Game1.player.rightRing.Value.getDescription();
              this.hoverTitle = Game1.player.rightRing.Value.DisplayName;
            }
          }
          else if (Game1.player.hat.Value != null)
          {
            this.hoveredItem = (Item) (Hat) (NetFieldBase<Hat, NetRef<Hat>>) Game1.player.hat;
            this.hoverText = Game1.player.hat.Value.getDescription();
            this.hoverTitle = Game1.player.hat.Value.DisplayName;
          }
          equipmentIcon.scale = Math.Min(equipmentIcon.scale + 0.05f, 1.1f);
        }
        equipmentIcon.scale = Math.Max(1f, equipmentIcon.scale - 0.025f);
      }
      if (this.portrait.containsPoint(x, y))
      {
        this.portrait.scale += 0.2f;
        this.hoverText = Game1.content.LoadString("Strings\\UI:Inventory_PortraitHover_Level", (object) Game1.player.Level) + Environment.NewLine + Game1.player.getTitle();
      }
      else
        this.portrait.scale = 0.0f;
      if (this.trashCan.containsPoint(x, y))
      {
        if ((double) this.trashCanLidRotation <= 0.0)
          Game1.playSound("trashcanlid");
        this.trashCanLidRotation = Math.Min(this.trashCanLidRotation + (float) Math.PI / 48f, 1.570796f);
        if (this.checkHeldItem() && Utility.getTrashReclamationPrice(Game1.player.CursorSlotItem, Game1.player) > 0)
        {
          this.hoverText = Game1.content.LoadString("Strings\\UI:TrashCanSale");
          this.hoverAmount = Utility.getTrashReclamationPrice(Game1.player.CursorSlotItem, Game1.player);
        }
      }
      else if ((double) this.trashCanLidRotation != 0.0)
      {
        this.trashCanLidRotation = Math.Max(this.trashCanLidRotation - 0.1308997f, 0.0f);
        if ((double) this.trashCanLidRotation == 0.0)
          Game1.playSound("thudStep");
      }
      if (this.organizeButton != null)
      {
        this.organizeButton.tryHover(x, y);
        if (this.organizeButton.containsPoint(x, y))
          this.hoverText = this.organizeButton.hoverText;
      }
      if (this.junimoNoteIcon == null)
        return;
      this.junimoNoteIcon.tryHover(x, y);
      if (this.junimoNoteIcon.containsPoint(x, y))
        this.hoverText = this.junimoNoteIcon.hoverText;
      if (GameMenu.bundleItemHovered)
      {
        this.junimoNoteIcon.scale = this.junimoNoteIcon.baseScale + (float) Math.Sin((double) this.junimoNotePulser / 100.0) / 4f;
        this.junimoNotePulser += (int) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
      }
      else
      {
        this.junimoNotePulser = 0;
        this.junimoNoteIcon.scale = this.junimoNoteIcon.baseScale;
      }
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override bool readyToClose() => !this.checkHeldItem();

    public override void draw(SpriteBatch b)
    {
      this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192);
      this.inventory.draw(b);
      foreach (ClickableComponent equipmentIcon in this.equipmentIcons)
      {
        string name = equipmentIcon.name;
        if (!(name == "Hat"))
        {
          if (!(name == "Right Ring"))
          {
            if (!(name == "Left Ring"))
            {
              if (!(name == "Boots"))
              {
                if (!(name == "Shirt"))
                {
                  if (name == "Pants")
                  {
                    if (Game1.player.pantsItem.Value != null)
                    {
                      b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White);
                      Game1.player.pantsItem.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale);
                    }
                    else
                      b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 68)), Color.White);
                  }
                }
                else if (Game1.player.shirtItem.Value != null)
                {
                  b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White);
                  Game1.player.shirtItem.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale);
                }
                else
                  b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 69)), Color.White);
              }
              else if (Game1.player.boots.Value != null)
              {
                b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White);
                Game1.player.boots.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale);
              }
              else
                b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 40)), Color.White);
            }
            else if (Game1.player.leftRing.Value != null)
            {
              b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White);
              Game1.player.leftRing.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale);
            }
            else
              b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 41)), Color.White);
          }
          else if (Game1.player.rightRing.Value != null)
          {
            b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White);
            Game1.player.rightRing.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale);
          }
          else
            b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 41)), Color.White);
        }
        else if (Game1.player.hat.Value != null)
        {
          b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White);
          Game1.player.hat.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale, 1f, 0.866f, StackDrawType.Hide);
        }
        else
          b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 42)), Color.White);
      }
      b.Draw(Game1.timeOfDay >= 1900 ? Game1.nightbg : Game1.daybg, new Vector2((float) (this.xPositionOnScreen + 192 - 64 - 8), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 - 8)), Color.White);
      FarmerRenderer.isDrawingForUI = true;
      Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(0, (bool) (NetFieldBase<bool, NetBool>) Game1.player.bathingClothes ? 108 : 0, false, false), (bool) (NetFieldBase<bool, NetBool>) Game1.player.bathingClothes ? 108 : 0, new Rectangle(0, (bool) (NetFieldBase<bool, NetBool>) Game1.player.bathingClothes ? 576 : 0, 16, 32), new Vector2((float) (this.xPositionOnScreen + 192 - 8 - 32), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 320 - 32 - 8)), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, Game1.player);
      if (Game1.timeOfDay >= 1900)
        Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(0, (bool) (NetFieldBase<bool, NetBool>) Game1.player.bathingClothes ? 108 : 0, false, false), (bool) (NetFieldBase<bool, NetBool>) Game1.player.bathingClothes ? 108 : 0, new Rectangle(0, (bool) (NetFieldBase<bool, NetBool>) Game1.player.bathingClothes ? 576 : 0, 16, 32), new Vector2((float) (this.xPositionOnScreen + 192 - 8 - 32), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 320 - 32 - 8)), Vector2.Zero, 0.8f, 2, Color.DarkBlue * 0.3f, 0.0f, 1f, Game1.player);
      FarmerRenderer.isDrawingForUI = false;
      Utility.drawTextWithShadow(b, Game1.player.Name, Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + 192 - 8) - Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f, (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 448 + 8)), Game1.textColor);
      float num1 = 32f;
      string text1 = Game1.content.LoadString("Strings\\UI:Inventory_FarmName", (object) Game1.player.farmName);
      Utility.drawTextWithShadow(b, text1, Game1.dialogueFont, new Vector2((float) ((double) this.xPositionOnScreen + (double) num1 + 512.0 + 32.0 - (double) Game1.dialogueFont.MeasureString(text1).X / 2.0), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 + 4)), Game1.textColor);
      string text2 = Game1.content.LoadString("Strings\\UI:Inventory_CurrentFunds" + (Game1.player.useSeparateWallets ? "_Separate" : ""), (object) Utility.getNumberWithCommas(Game1.player.Money));
      Utility.drawTextWithShadow(b, text2, Game1.dialogueFont, new Vector2((float) ((double) this.xPositionOnScreen + (double) num1 + 512.0 + 32.0 - (double) Game1.dialogueFont.MeasureString(text2).X / 2.0), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 320)), Game1.textColor);
      string text3 = Game1.content.LoadString("Strings\\UI:Inventory_TotalEarnings" + (Game1.player.useSeparateWallets ? "_Separate" : ""), (object) Utility.getNumberWithCommas((int) Game1.player.totalMoneyEarned));
      Utility.drawTextWithShadow(b, text3, Game1.dialogueFont, new Vector2((float) ((double) this.xPositionOnScreen + (double) num1 + 512.0 + 32.0 - (double) Game1.dialogueFont.MeasureString(text3).X / 2.0), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 384 - 4)), Game1.textColor);
      Pet pet = Game1.MasterPlayer.getPet();
      string petDisplayName = Game1.MasterPlayer.getPetDisplayName();
      if (pet != null)
      {
        Utility.drawTextWithShadow(b, petDisplayName, Game1.dialogueFont, new Vector2((float) ((double) this.xPositionOnScreen + (double) num1 + 320.0) + Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 448 + 8)), Game1.textColor);
        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) ((double) this.xPositionOnScreen + (double) num1 + 256.0) + Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 448 - 4)), new Rectangle(160 + (Game1.MasterPlayer.catPerson ? 0 : 48) + pet.whichBreed.Value * 16, 208, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f);
      }
      if (Game1.player.horseName.Value != null && Game1.player.horseName.Value != "")
      {
        Utility.drawTextWithShadow(b, Game1.player.horseName.Value, Game1.dialogueFont, new Vector2((float) ((double) this.xPositionOnScreen + (double) num1 + 384.0 + (double) Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f) + (petDisplayName != null ? (double) Math.Max(64f, Game1.dialogueFont.MeasureString(petDisplayName).X) : 0.0)), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 448 + 8)), Game1.textColor);
        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) ((double) this.xPositionOnScreen + (double) num1 + 320.0 + 8.0 + (double) Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f) + (petDisplayName != null ? (double) Math.Max(64f, Game1.dialogueFont.MeasureString(petDisplayName).X) : 0.0)), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 448 - 4)), new Rectangle(193, 192, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f);
      }
      int positionOnScreen = this.xPositionOnScreen;
      int num2 = this.width / 3;
      if (this.organizeButton != null)
        this.organizeButton.draw(b);
      this.trashCan.draw(b);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.trashCan.bounds.X + 60), (float) (this.trashCan.bounds.Y + 40)), new Rectangle?(new Rectangle(564 + Game1.player.trashCanLevel * 18, 129, 18, 10)), Color.White, this.trashCanLidRotation, new Vector2(16f, 10f), 4f, SpriteEffects.None, 0.86f);
      if (this.checkHeldItem())
        Game1.player.CursorSlotItem.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 16), (float) (Game1.getOldMouseY() + 16)), 1f);
      if (this.hoverText != null && !this.hoverText.Equals(""))
      {
        if (this.hoverAmount > 0)
          IClickableMenu.drawToolTip(b, this.hoverText, this.hoverTitle, (Item) null, true, moneyAmountToShowAtBottom: this.hoverAmount);
        else
          IClickableMenu.drawToolTip(b, this.hoverText, this.hoverTitle, this.hoveredItem, this.checkHeldItem());
      }
      if (this.junimoNoteIcon == null)
        return;
      this.junimoNoteIcon.draw(b);
    }

    public override void emergencyShutDown()
    {
      base.emergencyShutDown();
      this.setHeldItem(Game1.player.addItemToInventory(this.takeHeldItem()));
      if (!this.checkHeldItem())
        return;
      Game1.playSound("throwDownITem");
      Game1.createItemDebris(this.takeHeldItem(), Game1.player.getStandingPosition(), Game1.player.FacingDirection);
    }
  }
}
