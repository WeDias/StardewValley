// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.QuestContainerMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class QuestContainerMenu : MenuWithInventory
  {
    public InventoryMenu ItemsToGrabMenu;
    public Func<Item, int> stackCapacityCheck;
    public Action onItemChanged;
    public Action onConfirm;

    public QuestContainerMenu(
      IList<Item> inventory,
      int rows = 3,
      InventoryMenu.highlightThisItem highlight_method = null,
      Func<Item, int> stack_capacity_check = null,
      Action on_item_changed = null,
      Action on_confirm = null)
      : base(highlight_method, true)
    {
      this.onItemChanged += on_item_changed;
      this.onConfirm += on_confirm;
      int count = inventory.Count;
      int num = 64 * (count / rows);
      this.ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - num / 2, this.yPositionOnScreen + 64, false, inventory, capacity: count, rows: rows);
      this.stackCapacityCheck = stack_capacity_check;
      for (int index = 0; index < this.ItemsToGrabMenu.actualInventory.Count; ++index)
      {
        if (index >= this.ItemsToGrabMenu.actualInventory.Count - this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows)
          this.ItemsToGrabMenu.inventory[index].downNeighborID = index + 53910;
      }
      for (int index = 0; index < this.inventory.inventory.Count; ++index)
      {
        this.inventory.inventory[index].myID = index + 53910;
        if (this.inventory.inventory[index].downNeighborID != -1)
          this.inventory.inventory[index].downNeighborID += 53910;
        if (this.inventory.inventory[index].rightNeighborID != -1)
          this.inventory.inventory[index].rightNeighborID += 53910;
        if (this.inventory.inventory[index].leftNeighborID != -1)
          this.inventory.inventory[index].leftNeighborID += 53910;
        if (this.inventory.inventory[index].upNeighborID != -1)
          this.inventory.inventory[index].upNeighborID += 53910;
        if (index < 12)
          this.inventory.inventory[index].upNeighborID = this.ItemsToGrabMenu.actualInventory.Count - this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows;
        foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Right))
          clickableComponent.rightNeighborID = this.okButton.myID;
      }
      this.dropItemInvisibleButton.myID = -500;
      this.ItemsToGrabMenu.dropItemInvisibleButton.myID = -500;
      this.populateClickableComponentList();
      if (!Game1.options.SnappyMenus)
        return;
      this.setCurrentlySnappedComponentTo(53910);
      this.snapCursorToCurrentSnappedComponent();
    }

    public virtual int GetDonatableAmount(Item item)
    {
      if (item == null)
        return 0;
      int val1 = item.Stack;
      if (this.stackCapacityCheck != null)
        val1 = Math.Min(val1, this.stackCapacityCheck(item));
      return val1;
    }

    public virtual Item TryToGrab(Item item, int amount)
    {
      int num = Math.Min(amount, item.Stack);
      if (num == 0)
        return item;
      Item one = item.getOne();
      one.Stack = num;
      item.Stack -= num;
      InventoryMenu.highlightThisItem highlightMethod = this.inventory.highlightMethod;
      this.inventory.highlightMethod = new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems);
      Item addItem = this.inventory.tryToAddItem(one);
      this.inventory.highlightMethod = highlightMethod;
      if (addItem != null)
        item.Stack += addItem.Stack;
      if (this.onItemChanged != null)
        this.onItemChanged();
      return item.Stack <= 0 ? (Item) null : item;
    }

    public virtual Item TryToPlace(Item item, int amount)
    {
      Math.Min(amount, item.Stack);
      int num = Math.Min(amount, this.GetDonatableAmount(item));
      if (num == 0)
        return item;
      Item one = item.getOne();
      one.Stack = num;
      item.Stack -= num;
      Item addItem = this.ItemsToGrabMenu.tryToAddItem(one, "Ship");
      if (addItem != null)
        item.Stack += addItem.Stack;
      if (this.onItemChanged != null)
        this.onItemChanged();
      return item.Stack <= 0 ? (Item) null : item;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.isWithinBounds(x, y))
      {
        Item itemAt = this.inventory.getItemAt(x, y);
        if (itemAt != null)
          this.inventory.actualInventory[this.inventory.getInventoryPositionOfClick(x, y)] = this.TryToPlace(itemAt, itemAt.Stack);
      }
      if (this.ItemsToGrabMenu.isWithinBounds(x, y))
      {
        Item itemAt = this.ItemsToGrabMenu.getItemAt(x, y);
        if (itemAt != null)
          this.ItemsToGrabMenu.actualInventory[this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y)] = this.TryToGrab(itemAt, itemAt.Stack);
      }
      if (!this.okButton.containsPoint(x, y) || !this.readyToClose())
        return;
      this.exitThisMenu();
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      if (this.heldItem != null)
      {
        int stack = this.heldItem.Stack;
      }
      Item heldItem = this.heldItem;
      if (this.isWithinBounds(x, y))
      {
        Item itemAt = this.inventory.getItemAt(x, y);
        if (itemAt != null)
          this.inventory.actualInventory[this.inventory.getInventoryPositionOfClick(x, y)] = this.TryToPlace(itemAt, 1);
      }
      if (!this.ItemsToGrabMenu.isWithinBounds(x, y))
        return;
      Item itemAt1 = this.ItemsToGrabMenu.getItemAt(x, y);
      if (itemAt1 == null)
        return;
      this.ItemsToGrabMenu.actualInventory[this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y)] = this.TryToGrab(itemAt1, 1);
    }

    protected override void cleanupBeforeExit()
    {
      if (this.onConfirm != null)
        this.onConfirm();
      base.cleanupBeforeExit();
    }

    public override void update(GameTime time) => base.update(time);

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      this.ItemsToGrabMenu.hover(x, y, this.heldItem);
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
      this.draw(b, false, false);
      Game1.drawDialogueBox(this.ItemsToGrabMenu.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder, this.ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder, this.ItemsToGrabMenu.width + IClickableMenu.borderWidth * 2 + IClickableMenu.spaceToClearSideBorder * 2, this.ItemsToGrabMenu.height + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth * 2, false, true);
      this.ItemsToGrabMenu.draw(b);
      if (!this.hoverText.Equals(""))
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      if (this.heldItem != null)
        this.heldItem.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 16), (float) (Game1.getOldMouseY() + 16)), 1f);
      this.drawMouse(b);
      if (this.ItemsToGrabMenu.descriptionTitle == null || this.ItemsToGrabMenu.descriptionTitle.Length <= 1)
        return;
      IClickableMenu.drawHoverText(b, this.ItemsToGrabMenu.descriptionTitle, Game1.smallFont, 32 + (this.heldItem != null ? 16 : -21), 32 + (this.heldItem != null ? 16 : -21));
    }

    public enum ChangeType
    {
      None,
      Place,
      Grab,
    }
  }
}
