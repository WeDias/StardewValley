// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ItemGrabMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class ItemGrabMenu : MenuWithInventory
  {
    public const int region_organizationButtons = 15923;
    public const int region_itemsToGrabMenuModifier = 53910;
    public const int region_fillStacksButton = 12952;
    public const int region_organizeButton = 106;
    public const int region_colorPickToggle = 27346;
    public const int region_specialButton = 12485;
    public const int region_lastShippedHolder = 12598;
    public const int source_none = 0;
    public const int source_chest = 1;
    public const int source_gift = 2;
    public const int source_fishingChest = 3;
    public const int specialButton_junimotoggle = 1;
    public InventoryMenu ItemsToGrabMenu;
    private TemporaryAnimatedSprite poof;
    public bool reverseGrab;
    public bool showReceivingMenu = true;
    public bool drawBG = true;
    public bool destroyItemOnClick;
    public bool canExitOnKey;
    public bool playRightClickSound;
    public bool allowRightClick;
    public bool shippingBin;
    private string message;
    private ItemGrabMenu.behaviorOnItemSelect behaviorFunction;
    public ItemGrabMenu.behaviorOnItemSelect behaviorOnItemGrab;
    private Item hoverItem;
    private Item sourceItem;
    public ClickableTextureComponent fillStacksButton;
    public ClickableTextureComponent organizeButton;
    public ClickableTextureComponent colorPickerToggleButton;
    public ClickableTextureComponent specialButton;
    public ClickableTextureComponent lastShippedHolder;
    public List<ClickableComponent> discreteColorPickerCC;
    public int source;
    public int whichSpecialButton;
    public object context;
    private bool snappedtoBottom;
    public DiscreteColorPicker chestColorPicker;
    private bool essential;
    protected List<ItemGrabMenu.TransferredItemSprite> _transferredItemSprites = new List<ItemGrabMenu.TransferredItemSprite>();
    protected bool _sourceItemInCurrentLocation;
    public ClickableTextureComponent junimoNoteIcon;
    private int junimoNotePulser;

    public ItemGrabMenu(IList<Item> inventory, object context = null)
      : base(okButton: true, trashCan: true)
    {
      this.context = context;
      this.ItemsToGrabMenu = new InventoryMenu(this.xPositionOnScreen + 32, this.yPositionOnScreen, false, inventory);
      this.trashCan.myID = 106;
      this.ItemsToGrabMenu.populateClickableComponentList();
      for (int index = 0; index < this.ItemsToGrabMenu.inventory.Count; ++index)
      {
        if (this.ItemsToGrabMenu.inventory[index] != null)
        {
          this.ItemsToGrabMenu.inventory[index].myID += 53910;
          this.ItemsToGrabMenu.inventory[index].upNeighborID += 53910;
          this.ItemsToGrabMenu.inventory[index].rightNeighborID += 53910;
          this.ItemsToGrabMenu.inventory[index].downNeighborID = -7777;
          this.ItemsToGrabMenu.inventory[index].leftNeighborID += 53910;
          this.ItemsToGrabMenu.inventory[index].fullyImmutable = true;
          if (index % (this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows) == 0)
            this.ItemsToGrabMenu.inventory[index].leftNeighborID = this.dropItemInvisibleButton.myID;
          if (index % (this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows) == this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows - 1)
            this.ItemsToGrabMenu.inventory[index].rightNeighborID = this.trashCan.myID;
        }
      }
      for (int index = 0; index < this.GetColumnCount(); ++index)
      {
        if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count >= this.GetColumnCount())
          this.inventory.inventory[index].upNeighborID = this.shippingBin ? 12598 : -7777;
      }
      if (!this.shippingBin)
      {
        for (int index = 0; index < this.GetColumnCount() * 3; ++index)
        {
          if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count > index)
          {
            this.inventory.inventory[index].upNeighborID = -7777;
            this.inventory.inventory[index].upNeighborImmutable = true;
          }
        }
      }
      if (this.trashCan != null)
        this.trashCan.leftNeighborID = 11;
      if (this.okButton != null)
        this.okButton.leftNeighborID = 11;
      this.populateClickableComponentList();
      if (Game1.options.SnappyMenus)
        this.snapToDefaultClickableComponent();
      this.inventory.showGrayedOutSlots = true;
      this.SetupBorderNeighbors();
    }

    public virtual void DropRemainingItems()
    {
      if (this.ItemsToGrabMenu == null || this.ItemsToGrabMenu.actualInventory == null)
        return;
      foreach (Item obj in (IEnumerable<Item>) this.ItemsToGrabMenu.actualInventory)
      {
        if (obj != null)
          Game1.createItemDebris(obj, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
      }
      this.ItemsToGrabMenu.actualInventory.Clear();
    }

    public override bool readyToClose() => base.readyToClose();

    public ItemGrabMenu(
      IList<Item> inventory,
      bool reverseGrab,
      bool showReceivingMenu,
      InventoryMenu.highlightThisItem highlightFunction,
      ItemGrabMenu.behaviorOnItemSelect behaviorOnItemSelectFunction,
      string message,
      ItemGrabMenu.behaviorOnItemSelect behaviorOnItemGrab = null,
      bool snapToBottom = false,
      bool canBeExitedWithKey = false,
      bool playRightClickSound = true,
      bool allowRightClick = true,
      bool showOrganizeButton = false,
      int source = 0,
      Item sourceItem = null,
      int whichSpecialButton = -1,
      object context = null)
      : base(highlightFunction, true, true, menuOffsetHack: 64)
    {
      this.source = source;
      this.message = message;
      this.reverseGrab = reverseGrab;
      this.showReceivingMenu = showReceivingMenu;
      this.playRightClickSound = playRightClickSound;
      this.allowRightClick = allowRightClick;
      this.inventory.showGrayedOutSlots = true;
      this.sourceItem = sourceItem;
      this._sourceItemInCurrentLocation = sourceItem != null && ((IEnumerable<Item>) Game1.currentLocation.objects.Values).Contains<Item>(sourceItem);
      if (source == 1 && sourceItem != null && sourceItem is Chest && (sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.None)
      {
        this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, itemToDrawColored: ((Item) new Chest(true, sourceItem.ParentSheetIndex)));
        this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((Color) (NetFieldBase<Color, NetColor>) (sourceItem as Chest).playerChoiceColor);
        (this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
        ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64 - 160, 64, 64), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), 4f);
        textureComponent1.hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker");
        textureComponent1.myID = 27346;
        textureComponent1.downNeighborID = -99998;
        textureComponent1.leftNeighborID = 53921;
        textureComponent1.region = 15923;
        this.colorPickerToggleButton = textureComponent1;
        if (InventoryPage.ShouldShowJunimoNoteIcon())
        {
          ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64 - 216, 64, 64), "", Game1.content.LoadString("Strings\\UI:GameMenu_JunimoNote_Hover"), Game1.mouseCursors, new Rectangle(331, 374, 15, 14), 4f);
          textureComponent2.myID = 898;
          textureComponent2.leftNeighborID = 11;
          textureComponent2.downNeighborID = 106;
          this.junimoNoteIcon = textureComponent2;
        }
      }
      this.whichSpecialButton = whichSpecialButton;
      this.context = context;
      if (whichSpecialButton == 1)
      {
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64 - 160, 64, 64), Game1.mouseCursors, new Rectangle(108, 491, 16, 16), 4f);
        textureComponent.myID = 12485;
        textureComponent.downNeighborID = showOrganizeButton ? 12952 : 5948;
        textureComponent.region = 15923;
        textureComponent.leftNeighborID = 53921;
        this.specialButton = textureComponent;
        if (context != null && context is JunimoHut)
          this.specialButton.sourceRect.X = (bool) (NetFieldBase<bool, NetBool>) (context as JunimoHut).noHarvest ? 124 : 108;
      }
      if (snapToBottom)
      {
        this.movePosition(0, Game1.uiViewport.Height - (this.yPositionOnScreen + this.height - IClickableMenu.spaceToClearTopBorder));
        this.snappedtoBottom = true;
      }
      if (source == 1 && sourceItem != null && sourceItem is Chest && (sourceItem as Chest).GetActualCapacity() != 36)
      {
        int actualCapacity = (sourceItem as Chest).GetActualCapacity();
        int rows = 3;
        if (actualCapacity < 9)
          rows = 1;
        int num = 64 * (actualCapacity / rows);
        this.ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - num / 2, this.yPositionOnScreen + 64, false, inventory, highlightFunction, actualCapacity, rows);
        if ((sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
          this.inventory.moveItemSound = "Ship";
      }
      else
        this.ItemsToGrabMenu = new InventoryMenu(this.xPositionOnScreen + 32, this.yPositionOnScreen, false, inventory, highlightFunction);
      this.ItemsToGrabMenu.populateClickableComponentList();
      for (int index = 0; index < this.ItemsToGrabMenu.inventory.Count; ++index)
      {
        if (this.ItemsToGrabMenu.inventory[index] != null)
        {
          this.ItemsToGrabMenu.inventory[index].myID += 53910;
          this.ItemsToGrabMenu.inventory[index].upNeighborID += 53910;
          this.ItemsToGrabMenu.inventory[index].rightNeighborID += 53910;
          this.ItemsToGrabMenu.inventory[index].downNeighborID = -7777;
          this.ItemsToGrabMenu.inventory[index].leftNeighborID += 53910;
          this.ItemsToGrabMenu.inventory[index].fullyImmutable = true;
        }
      }
      this.behaviorFunction = behaviorOnItemSelectFunction;
      this.behaviorOnItemGrab = behaviorOnItemGrab;
      this.canExitOnKey = canBeExitedWithKey;
      if (showOrganizeButton)
      {
        ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64 - 64 - 16, 64, 64), "", Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"), Game1.mouseCursors, new Rectangle(103, 469, 16, 16), 4f);
        textureComponent3.myID = 12952;
        textureComponent3.upNeighborID = this.colorPickerToggleButton != null ? 27346 : (this.specialButton != null ? 12485 : -500);
        textureComponent3.downNeighborID = 106;
        textureComponent3.leftNeighborID = 53921;
        textureComponent3.region = 15923;
        this.fillStacksButton = textureComponent3;
        ClickableTextureComponent textureComponent4 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64, 64, 64), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"), Game1.mouseCursors, new Rectangle(162, 440, 16, 16), 4f);
        textureComponent4.myID = 106;
        textureComponent4.upNeighborID = 12952;
        textureComponent4.downNeighborID = 5948;
        textureComponent4.leftNeighborID = 53921;
        textureComponent4.region = 15923;
        this.organizeButton = textureComponent4;
      }
      this.RepositionSideButtons();
      if (this.chestColorPicker != null)
      {
        this.discreteColorPickerCC = new List<ClickableComponent>();
        for (int index = 0; index < this.chestColorPicker.totalColors; ++index)
          this.discreteColorPickerCC.Add(new ClickableComponent(new Rectangle(this.chestColorPicker.xPositionOnScreen + IClickableMenu.borderWidth / 2 + index * 9 * 4, this.chestColorPicker.yPositionOnScreen + IClickableMenu.borderWidth / 2, 36, 28), "")
          {
            myID = index + 4343,
            rightNeighborID = index < this.chestColorPicker.totalColors - 1 ? index + 4343 + 1 : -1,
            leftNeighborID = index > 0 ? index + 4343 - 1 : -1,
            downNeighborID = this.ItemsToGrabMenu == null || this.ItemsToGrabMenu.inventory.Count <= 0 ? 0 : 53910
          });
      }
      if (this.organizeButton != null)
      {
        foreach (ClickableComponent clickableComponent in this.ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right))
          clickableComponent.rightNeighborID = this.organizeButton.myID;
      }
      if (this.trashCan != null && this.inventory.inventory.Count >= 12 && this.inventory.inventory[11] != null)
        this.inventory.inventory[11].rightNeighborID = 5948;
      if (this.trashCan != null)
        this.trashCan.leftNeighborID = 11;
      if (this.okButton != null)
        this.okButton.leftNeighborID = 11;
      ClickableComponent clickableComponent1 = this.ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right).FirstOrDefault<ClickableComponent>();
      if (clickableComponent1 != null)
      {
        if (this.organizeButton != null)
          this.organizeButton.leftNeighborID = clickableComponent1.myID;
        if (this.specialButton != null)
          this.specialButton.leftNeighborID = clickableComponent1.myID;
        if (this.fillStacksButton != null)
          this.fillStacksButton.leftNeighborID = clickableComponent1.myID;
        if (this.junimoNoteIcon != null)
          this.junimoNoteIcon.leftNeighborID = clickableComponent1.myID;
      }
      this.populateClickableComponentList();
      if (Game1.options.SnappyMenus)
        this.snapToDefaultClickableComponent();
      this.SetupBorderNeighbors();
    }

    public virtual void RepositionSideButtons()
    {
      List<ClickableComponent> clickableComponentList = new List<ClickableComponent>();
      if (this.organizeButton != null)
        clickableComponentList.Add((ClickableComponent) this.organizeButton);
      if (this.fillStacksButton != null)
        clickableComponentList.Add((ClickableComponent) this.fillStacksButton);
      if (this.colorPickerToggleButton != null)
        clickableComponentList.Add((ClickableComponent) this.colorPickerToggleButton);
      if (this.specialButton != null)
        clickableComponentList.Add((ClickableComponent) this.specialButton);
      if (this.junimoNoteIcon != null)
        clickableComponentList.Add((ClickableComponent) this.junimoNoteIcon);
      int num = 80;
      if (clickableComponentList.Count >= 4)
        num = 72;
      for (int index = 0; index < clickableComponentList.Count; ++index)
      {
        ClickableComponent clickableComponent = clickableComponentList[index];
        if (index > 0 && clickableComponentList.Count > 1)
          clickableComponent.downNeighborID = clickableComponentList[index - 1].myID;
        if (index < clickableComponentList.Count - 1 && clickableComponentList.Count > 1)
          clickableComponent.upNeighborID = clickableComponentList[index + 1].myID;
        clickableComponent.bounds.X = this.xPositionOnScreen + this.width;
        clickableComponent.bounds.Y = this.yPositionOnScreen + this.height / 3 - 64 - num * index;
      }
    }

    public void SetupBorderNeighbors()
    {
      foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Right))
      {
        clickableComponent.rightNeighborID = -99998;
        clickableComponent.rightNeighborImmutable = true;
      }
      List<ClickableComponent> border = this.ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right);
      bool flag = false;
      foreach (ClickableComponent clickableComponent in this.allClickableComponents)
      {
        if (clickableComponent.region == 15923)
        {
          flag = true;
          break;
        }
      }
      foreach (ClickableComponent clickableComponent in border)
      {
        if (flag)
        {
          clickableComponent.rightNeighborID = -99998;
          clickableComponent.rightNeighborImmutable = true;
        }
        else
          clickableComponent.rightNeighborID = -1;
      }
      for (int index = 0; index < this.GetColumnCount(); ++index)
      {
        if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count >= 12)
          this.inventory.inventory[index].upNeighborID = this.shippingBin ? 12598 : (this.discreteColorPickerCC == null || this.ItemsToGrabMenu == null || this.ItemsToGrabMenu.inventory.Count > index || !Game1.player.showChestColorPicker ? (this.ItemsToGrabMenu.inventory.Count > index ? 53910 + index : 53910) : 4343);
        this.ItemsToGrabMenu.inventory[index].upNeighborID = this.discreteColorPickerCC == null || this.ItemsToGrabMenu == null || this.ItemsToGrabMenu.inventory.Count <= index || !Game1.player.showChestColorPicker ? -1 : 4343;
      }
      if (this.shippingBin)
        return;
      for (int index = 0; index < 36; ++index)
      {
        if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count > index)
        {
          this.inventory.inventory[index].upNeighborID = -7777;
          this.inventory.inventory[index].upNeighborImmutable = true;
        }
      }
    }

    public virtual int GetColumnCount() => this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows;

    public ItemGrabMenu setEssential(bool essential)
    {
      this.essential = essential;
      return this;
    }

    public void initializeShippingBin()
    {
      this.shippingBin = true;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width / 2 - 48, this.yPositionOnScreen + this.height / 2 - 80 - 64, 96, 96), "", Game1.content.LoadString("Strings\\UI:ShippingBin_LastItem"), Game1.mouseCursors, new Rectangle(293, 360, 24, 24), 4f);
      textureComponent.myID = 12598;
      textureComponent.region = 12598;
      this.lastShippedHolder = textureComponent;
      for (int index = 0; index < this.GetColumnCount(); ++index)
      {
        if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count >= this.GetColumnCount())
        {
          this.inventory.inventory[index].upNeighborID = -7777;
          if (index == 11)
            this.inventory.inventory[index].rightNeighborID = 5948;
        }
      }
      this.populateClickableComponentList();
      if (!Game1.options.SnappyMenus)
        return;
      this.snapToDefaultClickableComponent();
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
      switch (direction)
      {
        case 0:
          if (this.shippingBin && Game1.getFarm().lastItemShipped != null && oldID < 12)
          {
            this.currentlySnappedComponent = this.getComponentWithID(12598);
            this.currentlySnappedComponent.downNeighborID = oldID;
            this.snapCursorToCurrentSnappedComponent();
            break;
          }
          if (oldID < 53910 && oldID >= 12)
          {
            this.currentlySnappedComponent = this.getComponentWithID(oldID - 12);
            break;
          }
          int num1 = oldID + this.GetColumnCount() * 2;
          for (int index = 0; index < 3 && this.ItemsToGrabMenu.inventory.Count <= num1; ++index)
            num1 -= this.GetColumnCount();
          if (this.showReceivingMenu)
          {
            if (num1 < 0)
            {
              if (this.ItemsToGrabMenu.inventory.Count > 0)
                this.currentlySnappedComponent = this.getComponentWithID(53910 + this.ItemsToGrabMenu.inventory.Count - 1);
              else if (this.discreteColorPickerCC != null)
                this.currentlySnappedComponent = this.getComponentWithID(4343);
            }
            else
            {
              this.currentlySnappedComponent = this.getComponentWithID(num1 + 53910);
              if (this.currentlySnappedComponent == null)
                this.currentlySnappedComponent = this.getComponentWithID(53910);
            }
          }
          this.snapCursorToCurrentSnappedComponent();
          break;
        case 2:
          for (int index = 0; index < 12; ++index)
          {
            if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count >= this.GetColumnCount() && this.shippingBin)
              this.inventory.inventory[index].upNeighborID = this.shippingBin ? 12598 : Math.Min(index, this.ItemsToGrabMenu.inventory.Count - 1) + 53910;
          }
          if (!this.shippingBin && oldID >= 53910)
          {
            int num2 = oldID - 53910;
            if (num2 + this.GetColumnCount() <= this.ItemsToGrabMenu.inventory.Count - 1)
            {
              this.currentlySnappedComponent = this.getComponentWithID(num2 + this.GetColumnCount() + 53910);
              this.snapCursorToCurrentSnappedComponent();
              break;
            }
          }
          this.currentlySnappedComponent = this.getComponentWithID(oldRegion == 12598 ? 0 : (oldID - 53910) % this.GetColumnCount());
          this.snapCursorToCurrentSnappedComponent();
          break;
      }
    }

    public override void snapToDefaultClickableComponent()
    {
      if (this.shippingBin)
        this.currentlySnappedComponent = this.getComponentWithID(0);
      else if (this.source == 1 && this.sourceItem != null && this.sourceItem is Chest && (this.sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
        this.currentlySnappedComponent = this.getComponentWithID(0);
      else
        this.currentlySnappedComponent = this.getComponentWithID(this.ItemsToGrabMenu.inventory.Count <= 0 || !this.showReceivingMenu ? 0 : 53910);
      this.snapCursorToCurrentSnappedComponent();
    }

    public void setSourceItem(Item item)
    {
      this.sourceItem = item;
      this.chestColorPicker = (DiscreteColorPicker) null;
      this.colorPickerToggleButton = (ClickableTextureComponent) null;
      if (this.source == 1 && this.sourceItem != null && this.sourceItem is Chest && (this.sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.None)
      {
        this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, itemToDrawColored: ((Item) new Chest(true, this.sourceItem.ParentSheetIndex)));
        this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((Color) (NetFieldBase<Color, NetColor>) (this.sourceItem as Chest).playerChoiceColor);
        (this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
        this.colorPickerToggleButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64 - 160, 64, 64), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), 4f)
        {
          hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker")
        };
      }
      this.RepositionSideButtons();
    }

    public override bool IsAutomaticSnapValid(
      int direction,
      ClickableComponent a,
      ClickableComponent b)
    {
      return (direction != 1 || !this.ItemsToGrabMenu.inventory.Contains(a) || !this.inventory.inventory.Contains(b)) && base.IsAutomaticSnapValid(direction, a, b);
    }

    public void setBackgroundTransparency(bool b) => this.drawBG = b;

    public void setDestroyItemOnClick(bool b) => this.destroyItemOnClick = b;

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      if (!this.allowRightClick)
      {
        this.receiveRightClickOnlyToolAttachments(x, y);
      }
      else
      {
        base.receiveRightClick(x, y, playSound && this.playRightClickSound);
        if (this.heldItem == null && this.showReceivingMenu)
        {
          this.heldItem = this.ItemsToGrabMenu.rightClick(x, y, this.heldItem, false);
          if (this.heldItem != null && this.behaviorOnItemGrab != null)
          {
            this.behaviorOnItemGrab(this.heldItem, Game1.player);
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
              (Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
            if (Game1.options.SnappyMenus)
            {
              (Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = this.currentlySnappedComponent;
              (Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
            }
          }
          if (Utility.IsNormalObjectAtParentSheetIndex(this.heldItem, 326))
          {
            this.heldItem = (Item) null;
            Game1.player.canUnderstandDwarves = true;
            this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float) (x - x % 64 + 16), (float) (y - y % 64 + 16)), false, false);
            Game1.playSound("fireball");
          }
          else if (this.heldItem is StardewValley.Object && Utility.IsNormalObjectAtParentSheetIndex(this.heldItem, 434))
          {
            StardewValley.Object heldItem = this.heldItem as StardewValley.Object;
            this.heldItem = (Item) null;
            this.exitThisMenu(false);
            Game1.player.eatObject(heldItem, true);
          }
          else if (this.heldItem is StardewValley.Object && (bool) (NetFieldBase<bool, NetBool>) (this.heldItem as StardewValley.Object).isRecipe)
          {
            string key = this.heldItem.Name.Substring(0, this.heldItem.Name.IndexOf("Recipe") - 1);
            try
            {
              if ((this.heldItem as StardewValley.Object).Category == -7)
                Game1.player.cookingRecipes.Add(key, 0);
              else
                Game1.player.craftingRecipes.Add(key, 0);
              this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float) (x - x % 64 + 16), (float) (y - y % 64 + 16)), false, false);
              Game1.playSound("newRecipe");
            }
            catch (Exception ex)
            {
            }
            this.heldItem = (Item) null;
          }
          else
          {
            if (!Game1.player.addItemToInventoryBool(this.heldItem))
              return;
            this.heldItem = (Item) null;
            Game1.playSound("coin");
          }
        }
        else
        {
          if (!this.reverseGrab && this.behaviorFunction == null)
            return;
          this.behaviorFunction(this.heldItem, Game1.player);
          if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
            (Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
          if (!this.destroyItemOnClick)
            return;
          this.heldItem = (Item) null;
        }
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      if (this.snappedtoBottom)
        this.movePosition((newBounds.Width - oldBounds.Width) / 2, Game1.uiViewport.Height - (this.yPositionOnScreen + this.height - IClickableMenu.spaceToClearTopBorder));
      else
        this.movePosition((newBounds.Width - oldBounds.Width) / 2, (newBounds.Height - oldBounds.Height) / 2);
      if (this.ItemsToGrabMenu != null)
        this.ItemsToGrabMenu.gameWindowSizeChanged(oldBounds, newBounds);
      this.RepositionSideButtons();
      if (this.source != 1 || this.sourceItem == null || !(this.sourceItem is Chest) || (this.sourceItem as Chest).SpecialChestType != Chest.SpecialChestTypes.None)
        return;
      this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, itemToDrawColored: ((Item) new Chest(true, this.sourceItem.ParentSheetIndex)));
      this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((Color) (NetFieldBase<Color, NetColor>) (this.sourceItem as Chest).playerChoiceColor);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, !this.destroyItemOnClick);
      if (this.shippingBin && this.lastShippedHolder.containsPoint(x, y))
      {
        if (Game1.getFarm().lastItemShipped == null)
          return;
        Game1.getFarm().getShippingBin(Game1.player).Remove(Game1.getFarm().lastItemShipped);
        if (Game1.player.addItemToInventoryBool(Game1.getFarm().lastItemShipped))
        {
          Game1.playSound("coin");
          Game1.getFarm().lastItemShipped = (Item) null;
          if (Game1.player.ActiveObject == null)
            return;
          Game1.player.showCarrying();
          Game1.player.Halt();
        }
        else
          Game1.getFarm().getShippingBin(Game1.player).Add(Game1.getFarm().lastItemShipped);
      }
      else
      {
        if (this.chestColorPicker != null)
        {
          this.chestColorPicker.receiveLeftClick(x, y, true);
          if (this.sourceItem != null && this.sourceItem is Chest)
            (this.sourceItem as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
        }
        if (this.colorPickerToggleButton != null && this.colorPickerToggleButton.containsPoint(x, y))
        {
          Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
          this.chestColorPicker.visible = Game1.player.showChestColorPicker;
          try
          {
            Game1.playSound("drumkit6");
          }
          catch (Exception ex)
          {
          }
          this.SetupBorderNeighbors();
        }
        else if (this.whichSpecialButton != -1 && this.specialButton != null && this.specialButton.containsPoint(x, y))
        {
          Game1.playSound("drumkit6");
          if (this.whichSpecialButton != 1 || this.context == null || !(this.context is JunimoHut))
            return;
          (this.context as JunimoHut).noHarvest.Value = !(bool) (NetFieldBase<bool, NetBool>) (this.context as JunimoHut).noHarvest;
          this.specialButton.sourceRect.X = (bool) (NetFieldBase<bool, NetBool>) (this.context as JunimoHut).noHarvest ? 124 : 108;
        }
        else
        {
          if (this.heldItem == null && this.showReceivingMenu)
          {
            this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
            if (this.heldItem != null && this.behaviorOnItemGrab != null)
            {
              this.behaviorOnItemGrab(this.heldItem, Game1.player);
              if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
              {
                (Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
                if (Game1.options.SnappyMenus)
                {
                  (Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                  (Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
                }
              }
            }
            if (this.heldItem is StardewValley.Object && Utility.IsNormalObjectAtParentSheetIndex((Item) (this.heldItem as StardewValley.Object), 326))
            {
              this.heldItem = (Item) null;
              Game1.player.canUnderstandDwarves = true;
              this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float) (x - x % 64 + 16), (float) (y - y % 64 + 16)), false, false);
              Game1.playSound("fireball");
            }
            else if (this.heldItem is StardewValley.Object && Utility.IsNormalObjectAtParentSheetIndex((Item) (this.heldItem as StardewValley.Object), 102))
            {
              this.heldItem = (Item) null;
              Game1.player.foundArtifact(102, 1);
              this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float) (x - x % 64 + 16), (float) (y - y % 64 + 16)), false, false);
              Game1.playSound("fireball");
            }
            if (this.heldItem is StardewValley.Object && Utility.IsNormalObjectAtParentSheetIndex(this.heldItem, 434))
            {
              StardewValley.Object heldItem = this.heldItem as StardewValley.Object;
              this.heldItem = (Item) null;
              this.exitThisMenu(false);
              Game1.player.eatObject(heldItem, true);
            }
            else if (this.heldItem is StardewValley.Object && (bool) (NetFieldBase<bool, NetBool>) (this.heldItem as StardewValley.Object).isRecipe)
            {
              string key = this.heldItem.Name.Substring(0, this.heldItem.Name.IndexOf("Recipe") - 1);
              try
              {
                if ((this.heldItem as StardewValley.Object).Category == -7)
                  Game1.player.cookingRecipes.Add(key, 0);
                else
                  Game1.player.craftingRecipes.Add(key, 0);
                this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float) (x - x % 64 + 16), (float) (y - y % 64 + 16)), false, false);
                Game1.playSound("newRecipe");
              }
              catch (Exception ex)
              {
              }
              this.heldItem = (Item) null;
            }
            else if (Game1.player.addItemToInventoryBool(this.heldItem))
            {
              this.heldItem = (Item) null;
              Game1.playSound("coin");
            }
          }
          else if ((this.reverseGrab || this.behaviorFunction != null) && this.isWithinBounds(x, y))
          {
            this.behaviorFunction(this.heldItem, Game1.player);
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
            {
              (Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
              if (Game1.options.SnappyMenus)
              {
                (Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                (Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
              }
            }
            if (this.destroyItemOnClick)
            {
              this.heldItem = (Item) null;
              return;
            }
          }
          if (this.organizeButton != null && this.organizeButton.containsPoint(x, y))
          {
            ClickableComponent snappedComponent = this.currentlySnappedComponent;
            ItemGrabMenu.organizeItemsInList(this.ItemsToGrabMenu.actualInventory);
            Item heldItem = this.heldItem;
            this.heldItem = (Item) null;
            Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu(this.ItemsToGrabMenu.actualInventory, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), this.behaviorFunction, (string) null, this.behaviorOnItemGrab, canBeExitedWithKey: true, showOrganizeButton: true, source: this.source, sourceItem: this.sourceItem, whichSpecialButton: this.whichSpecialButton, context: this.context).setEssential(this.essential);
            if (snappedComponent != null)
            {
              Game1.activeClickableMenu.setCurrentlySnappedComponentTo(snappedComponent.myID);
              if (Game1.options.SnappyMenus)
                this.snapCursorToCurrentSnappedComponent();
            }
            (Game1.activeClickableMenu as ItemGrabMenu).heldItem = heldItem;
            Game1.playSound("Ship");
          }
          else if (this.fillStacksButton != null && this.fillStacksButton.containsPoint(x, y))
          {
            this.FillOutStacks();
            Game1.playSound("Ship");
          }
          else if (this.junimoNoteIcon != null && this.junimoNoteIcon.containsPoint(x, y))
          {
            if (!this.readyToClose())
              return;
            Game1.activeClickableMenu = (IClickableMenu) new JunimoNoteMenu(true);
          }
          else
          {
            if (this.heldItem == null || this.isWithinBounds(x, y) || !this.heldItem.canBeTrashed())
              return;
            this.DropHeldItem();
          }
        }
      }
    }

    public virtual void DropHeldItem()
    {
      if (this.heldItem == null)
        return;
      Game1.playSound("throwDownITem");
      Console.WriteLine("Dropping " + this.heldItem.Name);
      int direction = (int) Game1.player.facingDirection;
      if (this.context is LibraryMuseum)
        direction = 2;
      Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), direction);
      if (this.inventory.onAddItem != null)
        this.inventory.onAddItem(this.heldItem, Game1.player);
      this.heldItem = (Item) null;
    }

    public void FillOutStacks()
    {
      for (int index1 = 0; index1 < this.ItemsToGrabMenu.actualInventory.Count; ++index1)
      {
        Item other = this.ItemsToGrabMenu.actualInventory[index1];
        if (other != null && other.maximumStackSize() > 1)
        {
          for (int index2 = 0; index2 < this.inventory.actualInventory.Count; ++index2)
          {
            Item obj1 = this.inventory.actualInventory[index2];
            if (obj1 != null && other.canStackWith((ISalable) obj1))
            {
              this._transferredItemSprites.Add(new ItemGrabMenu.TransferredItemSprite(obj1.getOne(), this.inventory.inventory[index2].bounds.X, this.inventory.inventory[index2].bounds.Y));
              int stack1 = obj1.Stack;
              if (other.getRemainingStackSpace() > 0)
              {
                stack1 = other.addToStack(obj1);
                this.ItemsToGrabMenu.ShakeItem(other);
              }
              int stack2;
              for (obj1.Stack = stack1; obj1.Stack > 0; obj1.Stack = stack2)
              {
                Item obj2 = (Item) null;
                if (Utility.canItemBeAddedToThisInventoryList(other.getOne(), this.ItemsToGrabMenu.actualInventory, this.ItemsToGrabMenu.capacity))
                {
                  if (obj2 == null)
                  {
                    for (int index3 = 0; index3 < this.ItemsToGrabMenu.actualInventory.Count; ++index3)
                    {
                      if (this.ItemsToGrabMenu.actualInventory[index3] != null && this.ItemsToGrabMenu.actualInventory[index3].canStackWith((ISalable) other) && this.ItemsToGrabMenu.actualInventory[index3].getRemainingStackSpace() > 0)
                      {
                        obj2 = this.ItemsToGrabMenu.actualInventory[index3];
                        break;
                      }
                    }
                  }
                  if (obj2 == null)
                  {
                    for (int index4 = 0; index4 < this.ItemsToGrabMenu.actualInventory.Count; ++index4)
                    {
                      if (this.ItemsToGrabMenu.actualInventory[index4] == null)
                      {
                        obj2 = this.ItemsToGrabMenu.actualInventory[index4] = other.getOne();
                        obj2.Stack = 0;
                        break;
                      }
                    }
                  }
                  if (obj2 == null && this.ItemsToGrabMenu.actualInventory.Count < this.ItemsToGrabMenu.capacity)
                  {
                    obj2 = other.getOne();
                    obj2.Stack = 0;
                    this.ItemsToGrabMenu.actualInventory.Add(obj2);
                  }
                  if (obj2 != null)
                  {
                    stack2 = obj2.addToStack(obj1);
                    this.ItemsToGrabMenu.ShakeItem(obj2);
                  }
                  else
                    break;
                }
                else
                  break;
              }
              if (obj1.Stack == 0)
                this.inventory.actualInventory[index2] = (Item) null;
            }
          }
        }
      }
    }

    public static void organizeItemsInList(IList<Item> items)
    {
      List<Item> objList = new List<Item>((IEnumerable<Item>) items);
      List<Item> collection = new List<Item>();
      for (int index = 0; index < objList.Count; ++index)
      {
        if (objList[index] == null)
        {
          objList.RemoveAt(index);
          --index;
        }
        else if (objList[index] is Tool)
        {
          collection.Add(objList[index]);
          objList.RemoveAt(index);
          --index;
        }
      }
      for (int index1 = 0; index1 < objList.Count; ++index1)
      {
        Item obj1 = objList[index1];
        if (obj1.getRemainingStackSpace() > 0)
        {
          for (int index2 = index1 + 1; index2 < objList.Count; ++index2)
          {
            Item obj2 = objList[index2];
            if (obj1.canStackWith((ISalable) obj2))
            {
              obj2.Stack = obj1.addToStack(obj2);
              if (obj2.Stack == 0)
              {
                objList.RemoveAt(index2);
                --index2;
              }
            }
          }
        }
      }
      objList.Sort();
      objList.InsertRange(0, (IEnumerable<Item>) collection);
      for (int index = 0; index < items.Count; ++index)
        items[index] = (Item) null;
      for (int index = 0; index < objList.Count; ++index)
        items[index] = objList[index];
    }

    public bool areAllItemsTaken()
    {
      for (int index = 0; index < this.ItemsToGrabMenu.actualInventory.Count; ++index)
      {
        if (this.ItemsToGrabMenu.actualInventory[index] != null)
          return false;
      }
      return true;
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      if (b == Buttons.Back && this.organizeButton != null)
      {
        ItemGrabMenu.organizeItemsInList((IList<Item>) Game1.player.items);
        Game1.playSound("Ship");
      }
      if (b == Buttons.RightShoulder)
      {
        ClickableComponent componentWithId = this.getComponentWithID(12952);
        if (componentWithId != null)
        {
          this.setCurrentlySnappedComponentTo(componentWithId.myID);
          this.snapCursorToCurrentSnappedComponent();
        }
        else
        {
          int num = -1;
          ClickableComponent clickableComponent1 = (ClickableComponent) null;
          foreach (ClickableComponent clickableComponent2 in this.allClickableComponents)
          {
            if (clickableComponent2.region == 15923 && (num == -1 || clickableComponent2.bounds.Y < num))
            {
              num = clickableComponent2.bounds.Y;
              clickableComponent1 = clickableComponent2;
            }
          }
          if (clickableComponent1 != null)
          {
            this.setCurrentlySnappedComponentTo(clickableComponent1.myID);
            this.snapCursorToCurrentSnappedComponent();
          }
        }
      }
      if (this.shippingBin || b != Buttons.LeftShoulder)
        return;
      ClickableComponent componentWithId1 = this.getComponentWithID(53910);
      if (componentWithId1 != null)
      {
        this.setCurrentlySnappedComponentTo(componentWithId1.myID);
        this.snapCursorToCurrentSnappedComponent();
      }
      else
      {
        if (this.getComponentWithID(0) == null)
          return;
        this.setCurrentlySnappedComponentTo(0);
        this.snapCursorToCurrentSnappedComponent();
      }
    }

    public override void receiveKeyPress(Keys key)
    {
      if (Game1.options.snappyMenus && Game1.options.gamepadControls)
        this.applyMovementKey(key);
      if ((this.canExitOnKey || this.areAllItemsTaken()) && Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
      {
        this.exitThisMenu();
        if (Game1.currentLocation.currentEvent != null && Game1.currentLocation.currentEvent.CurrentCommand > 0)
          ++Game1.currentLocation.currentEvent.CurrentCommand;
      }
      else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.heldItem != null)
        Game1.setMousePosition(this.trashCan.bounds.Center);
      if (key != Keys.Delete || this.heldItem == null || !this.heldItem.canBeTrashed())
        return;
      Utility.trashItem(this.heldItem);
      this.heldItem = (Item) null;
    }

    public override void update(GameTime time)
    {
      base.update(time);
      if (this.poof != null && this.poof.update(time))
        this.poof = (TemporaryAnimatedSprite) null;
      if (this.chestColorPicker != null)
        this.chestColorPicker.update(time);
      if (this.sourceItem != null && this.sourceItem is Chest && this._sourceItemInCurrentLocation)
      {
        Vector2 tileLocation = (Vector2) (NetFieldBase<Vector2, NetVector2>) (this.sourceItem as StardewValley.Object).tileLocation;
        if (tileLocation != Vector2.Zero && !Game1.currentLocation.objects.ContainsKey(tileLocation))
        {
          if (Game1.activeClickableMenu != null)
            Game1.activeClickableMenu.emergencyShutDown();
          Game1.exitActiveMenu();
        }
      }
      for (int index = 0; index < this._transferredItemSprites.Count; ++index)
      {
        if (this._transferredItemSprites[index].Update(time))
        {
          this._transferredItemSprites.RemoveAt(index);
          --index;
        }
      }
    }

    public override void performHoverAction(int x, int y)
    {
      this.hoveredItem = (Item) null;
      this.hoverText = "";
      base.performHoverAction(x, y);
      if (this.colorPickerToggleButton != null)
      {
        this.colorPickerToggleButton.tryHover(x, y, 0.25f);
        if (this.colorPickerToggleButton.containsPoint(x, y))
          this.hoverText = this.colorPickerToggleButton.hoverText;
      }
      if (this.organizeButton != null)
      {
        this.organizeButton.tryHover(x, y, 0.25f);
        if (this.organizeButton.containsPoint(x, y))
          this.hoverText = this.organizeButton.hoverText;
      }
      if (this.fillStacksButton != null)
      {
        this.fillStacksButton.tryHover(x, y, 0.25f);
        if (this.fillStacksButton.containsPoint(x, y))
          this.hoverText = this.fillStacksButton.hoverText;
      }
      if (this.specialButton != null)
        this.specialButton.tryHover(x, y, 0.25f);
      if (this.showReceivingMenu)
      {
        Item obj = this.ItemsToGrabMenu.hover(x, y, this.heldItem);
        if (obj != null)
          this.hoveredItem = obj;
      }
      if (this.junimoNoteIcon != null)
      {
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
      if (this.hoverText != null)
        return;
      if (this.organizeButton != null)
      {
        this.hoverText = (string) null;
        this.organizeButton.tryHover(x, y);
        if (this.organizeButton.containsPoint(x, y))
          this.hoverText = this.organizeButton.hoverText;
      }
      if (this.shippingBin)
      {
        this.hoverText = (string) null;
        if (this.lastShippedHolder.containsPoint(x, y) && Game1.getFarm().lastItemShipped != null)
          this.hoverText = this.lastShippedHolder.hoverText;
      }
      if (this.chestColorPicker == null)
        return;
      this.chestColorPicker.performHoverAction(x, y);
    }

    public override void draw(SpriteBatch b)
    {
      if (this.drawBG)
        b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
      this.draw(b, false, false);
      if (this.showReceivingMenu)
      {
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen - 64), (float) (this.yPositionOnScreen + this.height / 2 + 64 + 16)), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen - 64), (float) (this.yPositionOnScreen + this.height / 2 + 64 - 16)), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen - 40), (float) (this.yPositionOnScreen + this.height / 2 + 64 - 44)), new Rectangle?(new Rectangle(4, 372, 8, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        if ((this.source != 1 || this.sourceItem == null || !(this.sourceItem is Chest) || (this.sourceItem as Chest).SpecialChestType != Chest.SpecialChestTypes.MiniShippingBin && (this.sourceItem as Chest).SpecialChestType != Chest.SpecialChestTypes.JunimoChest && (this.sourceItem as Chest).SpecialChestType != Chest.SpecialChestTypes.Enricher) && this.source != 0)
        {
          b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen - 72), (float) (this.yPositionOnScreen + 64 + 16)), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen - 72), (float) (this.yPositionOnScreen + 64 - 16)), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          Rectangle rectangle = new Rectangle((int) sbyte.MaxValue, 412, 10, 11);
          switch (this.source)
          {
            case 2:
              rectangle.X += 20;
              break;
            case 3:
              rectangle.X += 10;
              break;
          }
          b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen - 52), (float) (this.yPositionOnScreen + 64 - 44)), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }
        Game1.drawDialogueBox(this.ItemsToGrabMenu.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder, this.ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder, this.ItemsToGrabMenu.width + IClickableMenu.borderWidth * 2 + IClickableMenu.spaceToClearSideBorder * 2, this.ItemsToGrabMenu.height + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth * 2, false, true);
        this.ItemsToGrabMenu.draw(b);
      }
      else if (this.message != null)
        Game1.drawDialogueBox(Game1.uiViewport.Width / 2, this.ItemsToGrabMenu.yPositionOnScreen + this.ItemsToGrabMenu.height / 2, false, false, this.message);
      if (this.poof != null)
        this.poof.draw(b, true);
      foreach (ItemGrabMenu.TransferredItemSprite transferredItemSprite in this._transferredItemSprites)
        transferredItemSprite.Draw(b);
      if (this.shippingBin && Game1.getFarm().lastItemShipped != null)
      {
        this.lastShippedHolder.draw(b);
        Game1.getFarm().lastItemShipped.drawInMenu(b, new Vector2((float) (this.lastShippedHolder.bounds.X + 16), (float) (this.lastShippedHolder.bounds.Y + 16)), 1f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.lastShippedHolder.bounds.X - 8), (float) (this.lastShippedHolder.bounds.Bottom - 100)), new Rectangle?(new Rectangle(325, 448, 5, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.lastShippedHolder.bounds.X + 84), (float) (this.lastShippedHolder.bounds.Bottom - 100)), new Rectangle?(new Rectangle(325, 448, 5, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.lastShippedHolder.bounds.X - 8), (float) (this.lastShippedHolder.bounds.Bottom - 44)), new Rectangle?(new Rectangle(325, 452, 5, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.lastShippedHolder.bounds.X + 84), (float) (this.lastShippedHolder.bounds.Bottom - 44)), new Rectangle?(new Rectangle(325, 452, 5, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      if (this.colorPickerToggleButton != null)
        this.colorPickerToggleButton.draw(b);
      else if (this.specialButton != null)
        this.specialButton.draw(b);
      if (this.chestColorPicker != null)
        this.chestColorPicker.draw(b);
      if (this.organizeButton != null)
        this.organizeButton.draw(b);
      if (this.fillStacksButton != null)
        this.fillStacksButton.draw(b);
      if (this.junimoNoteIcon != null)
        this.junimoNoteIcon.draw(b);
      if (this.hoverText != null && (this.hoveredItem == null || this.hoveredItem == null || this.ItemsToGrabMenu == null))
      {
        if (this.hoverAmount > 0)
          IClickableMenu.drawToolTip(b, this.hoverText, "", (Item) null, true, moneyAmountToShowAtBottom: this.hoverAmount);
        else
          IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      }
      if (this.hoveredItem != null)
        IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem, this.heldItem != null);
      else if (this.hoveredItem != null && this.ItemsToGrabMenu != null)
        IClickableMenu.drawToolTip(b, this.ItemsToGrabMenu.descriptionText, this.ItemsToGrabMenu.descriptionTitle, this.hoveredItem, this.heldItem != null);
      if (this.heldItem != null)
        this.heldItem.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 8), (float) (Game1.getOldMouseY() + 8)), 1f);
      Game1.mouseCursorTransparency = 1f;
      this.drawMouse(b);
    }

    public override void emergencyShutDown()
    {
      base.emergencyShutDown();
      Console.WriteLine("ItemGrabMenu.emergencyShutDown");
      if (this.heldItem != null)
      {
        Console.WriteLine("Taking " + this.heldItem.Name);
        this.heldItem = Game1.player.addItemToInventory(this.heldItem);
      }
      if (this.heldItem != null)
        this.DropHeldItem();
      if (this.essential)
      {
        Console.WriteLine("essential");
        foreach (Item obj in (IEnumerable<Item>) this.ItemsToGrabMenu.actualInventory)
        {
          if (obj != null)
          {
            Console.WriteLine("Taking " + obj.Name);
            Item inventory = Game1.player.addItemToInventory(obj);
            if (inventory != null)
            {
              Console.WriteLine("Dropping " + inventory.Name);
              Game1.createItemDebris(inventory, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
            }
          }
        }
      }
      else
        Console.WriteLine("essential");
    }

    public delegate void behaviorOnItemSelect(Item item, Farmer who);

    public class TransferredItemSprite
    {
      public Item item;
      public Vector2 position;
      public float age;
      public float alpha = 1f;

      public TransferredItemSprite(Item transferred_item, int start_x, int start_y)
      {
        this.item = transferred_item;
        this.position.X = (float) start_x;
        this.position.Y = (float) start_y;
      }

      public bool Update(GameTime time)
      {
        float num = 0.15f;
        this.position.Y -= (float) (time.ElapsedGameTime.TotalSeconds * 128.0);
        this.age += (float) time.ElapsedGameTime.TotalSeconds;
        this.alpha = (float) (1.0 - (double) this.age / (double) num);
        return (double) this.age >= (double) num;
      }

      public void Draw(SpriteBatch b) => this.item.drawInMenu(b, this.position, 1f, this.alpha, 0.9f, StackDrawType.Hide, Color.White, false);
    }
  }
}
