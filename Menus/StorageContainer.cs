// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.StorageContainer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class StorageContainer : MenuWithInventory
  {
    public InventoryMenu ItemsToGrabMenu;
    private TemporaryAnimatedSprite poof;
    private StorageContainer.behaviorOnItemChange itemChangeBehavior;

    public StorageContainer(
      IList<Item> inventory,
      int capacity,
      int rows = 3,
      StorageContainer.behaviorOnItemChange itemChangeBehavior = null,
      InventoryMenu.highlightThisItem highlightMethod = null)
      : base(highlightMethod, true, true)
    {
      this.itemChangeBehavior = itemChangeBehavior;
      int num = 64 * (capacity / rows);
      this.ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - num / 2, this.yPositionOnScreen + 64, false, inventory, capacity: capacity, rows: rows);
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
      }
      this.dropItemInvisibleButton.myID = -500;
      this.ItemsToGrabMenu.dropItemInvisibleButton.myID = -500;
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.setCurrentlySnappedComponentTo(53910);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      int num = 64 * (this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows);
      int rows = this.ItemsToGrabMenu.rows;
      this.ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - num / 2, this.yPositionOnScreen + 64, false, this.ItemsToGrabMenu.actualInventory, capacity: this.ItemsToGrabMenu.capacity, rows: this.ItemsToGrabMenu.rows);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      Item heldItem = this.heldItem;
      int num = heldItem != null ? heldItem.Stack : -1;
      if (this.isWithinBounds(x, y))
      {
        base.receiveLeftClick(x, y, false);
        if (this.itemChangeBehavior == null && heldItem == null && this.heldItem != null && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
          this.heldItem = this.ItemsToGrabMenu.tryToAddItem(this.heldItem, "Ship");
      }
      bool flag = true;
      if (this.ItemsToGrabMenu.isWithinBounds(x, y))
      {
        this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
        if (this.heldItem != null && heldItem == null || this.heldItem != null && heldItem != null && !this.heldItem.Equals((object) heldItem))
        {
          if (this.itemChangeBehavior != null)
            flag = this.itemChangeBehavior(this.heldItem, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), heldItem, this, true);
          if (flag)
            Game1.playSound("dwop");
        }
        if (this.heldItem == null && heldItem != null || this.heldItem != null && heldItem != null && !this.heldItem.Equals((object) heldItem))
        {
          Item old = this.heldItem;
          if (this.heldItem == null && this.ItemsToGrabMenu.getItemAt(x, y) != null && num < this.ItemsToGrabMenu.getItemAt(x, y).Stack)
          {
            old = heldItem.getOne();
            old.Stack = num;
          }
          if (this.itemChangeBehavior != null)
            flag = this.itemChangeBehavior(heldItem, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), old, this);
          if (flag)
            Game1.playSound("Ship");
        }
        if (this.heldItem is StardewValley.Object && (bool) (NetFieldBase<bool, NetBool>) (this.heldItem as StardewValley.Object).isRecipe)
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
        else if (Game1.oldKBState.IsKeyDown(Keys.LeftShift) && Game1.player.addItemToInventoryBool(this.heldItem))
        {
          this.heldItem = (Item) null;
          if (this.itemChangeBehavior != null)
            flag = this.itemChangeBehavior(this.heldItem, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), heldItem, this, true);
          if (flag)
            Game1.playSound("coin");
        }
      }
      if (this.okButton.containsPoint(x, y) && this.readyToClose())
      {
        Game1.playSound("bigDeSelect");
        Game1.exitActiveMenu();
      }
      if (!this.trashCan.containsPoint(x, y) || this.heldItem == null || !this.heldItem.canBeTrashed())
        return;
      Utility.trashItem(this.heldItem);
      this.heldItem = (Item) null;
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      int num1 = this.heldItem != null ? this.heldItem.Stack : 0;
      Item heldItem = this.heldItem;
      if (this.isWithinBounds(x, y))
      {
        base.receiveRightClick(x, y, true);
        if (this.itemChangeBehavior == null && heldItem == null && this.heldItem != null && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
          this.heldItem = this.ItemsToGrabMenu.tryToAddItem(this.heldItem, "Ship");
      }
      if (!this.ItemsToGrabMenu.isWithinBounds(x, y))
        return;
      this.heldItem = this.ItemsToGrabMenu.rightClick(x, y, this.heldItem, false);
      if (this.heldItem != null && heldItem == null || this.heldItem != null && heldItem != null && !this.heldItem.Equals((object) heldItem) || this.heldItem != null && heldItem != null && this.heldItem.Equals((object) heldItem) && this.heldItem.Stack != num1)
      {
        if (this.itemChangeBehavior != null)
        {
          int num2 = this.itemChangeBehavior(this.heldItem, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), heldItem, this, true) ? 1 : 0;
        }
        Game1.playSound("dwop");
      }
      if (this.heldItem == null && heldItem != null || this.heldItem != null && heldItem != null && !this.heldItem.Equals((object) heldItem))
      {
        if (this.itemChangeBehavior != null)
        {
          int num3 = this.itemChangeBehavior(heldItem, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), this.heldItem, this) ? 1 : 0;
        }
        Game1.playSound("Ship");
      }
      if (this.heldItem is StardewValley.Object && (bool) (NetFieldBase<bool, NetBool>) (this.heldItem as StardewValley.Object).isRecipe)
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
        if (!Game1.oldKBState.IsKeyDown(Keys.LeftShift) || !Game1.player.addItemToInventoryBool(this.heldItem))
          return;
        this.heldItem = (Item) null;
        Game1.playSound("coin");
        if (this.itemChangeBehavior == null)
          return;
        int num4 = this.itemChangeBehavior(this.heldItem, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), heldItem, this, true) ? 1 : 0;
      }
    }

    public override void update(GameTime time)
    {
      base.update(time);
      if (this.poof == null || !this.poof.update(time))
        return;
      this.poof = (TemporaryAnimatedSprite) null;
    }

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
      if (this.poof != null)
        this.poof.draw(b, true);
      if (!this.hoverText.Equals(""))
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      if (this.heldItem != null)
        this.heldItem.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 16), (float) (Game1.getOldMouseY() + 16)), 1f);
      this.drawMouse(b);
      if (this.ItemsToGrabMenu.descriptionTitle == null || this.ItemsToGrabMenu.descriptionTitle.Length <= 1)
        return;
      IClickableMenu.drawHoverText(b, this.ItemsToGrabMenu.descriptionTitle, Game1.smallFont, 32 + (this.heldItem != null ? 16 : -21), 32 + (this.heldItem != null ? 16 : -21));
    }

    public delegate bool behaviorOnItemChange(
      Item i,
      int position,
      Item old,
      StorageContainer container,
      bool onRemoval = false);
  }
}
