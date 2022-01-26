// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.InventoryMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class InventoryMenu : IClickableMenu
  {
    public const int region_inventorySlot0 = 0;
    public const int region_inventorySlot1 = 1;
    public const int region_inventorySlot2 = 2;
    public const int region_inventorySlot3 = 3;
    public const int region_inventorySlot4 = 4;
    public const int region_inventorySlot5 = 5;
    public const int region_inventorySlot6 = 6;
    public const int region_inventorySlot7 = 7;
    public const int region_inventorySlot8 = 8;
    public const int region_inventorySlot9 = 9;
    public const int region_inventorySlot10 = 10;
    public const int region_inventorySlot11 = 11;
    public const int region_inventorySlot12 = 12;
    public const int region_inventorySlot13 = 13;
    public const int region_inventorySlot14 = 14;
    public const int region_inventorySlot15 = 15;
    public const int region_inventorySlot16 = 16;
    public const int region_inventorySlot17 = 17;
    public const int region_inventorySlot18 = 18;
    public const int region_inventorySlot19 = 19;
    public const int region_inventorySlot20 = 20;
    public const int region_inventorySlot21 = 21;
    public const int region_inventorySlot22 = 22;
    public const int region_inventorySlot23 = 23;
    public const int region_inventorySlot24 = 24;
    public const int region_inventorySlot25 = 25;
    public const int region_inventorySlot26 = 26;
    public const int region_inventorySlot27 = 27;
    public const int region_inventorySlot28 = 28;
    public const int region_inventorySlot29 = 29;
    public const int region_inventorySlot30 = 30;
    public const int region_inventorySlot31 = 31;
    public const int region_inventorySlot32 = 32;
    public const int region_inventorySlot33 = 33;
    public const int region_inventorySlot34 = 34;
    public const int region_inventorySlot35 = 35;
    public const int region_dropButton = 107;
    public const int region_inventoryArea = 9000;
    public string hoverText = "";
    public string hoverTitle = "";
    public string descriptionTitle = "";
    public string descriptionText = "";
    public List<ClickableComponent> inventory = new List<ClickableComponent>();
    protected Dictionary<int, double> _iconShakeTimer = new Dictionary<int, double>();
    public IList<Item> actualInventory;
    public InventoryMenu.highlightThisItem highlightMethod;
    public ItemGrabMenu.behaviorOnItemSelect onAddItem;
    public bool playerInventory;
    public bool drawSlots;
    public bool showGrayedOutSlots;
    public int capacity;
    public int rows;
    public int horizontalGap;
    public int verticalGap;
    public ClickableComponent dropItemInvisibleButton;
    public string moveItemSound = "dwop";

    public InventoryMenu(
      int xPosition,
      int yPosition,
      bool playerInventory,
      IList<Item> actualInventory = null,
      InventoryMenu.highlightThisItem highlightMethod = null,
      int capacity = -1,
      int rows = 3,
      int horizontalGap = 0,
      int verticalGap = 0,
      bool drawSlots = true)
      : base(xPosition, yPosition, 64 * ((capacity == -1 ? 36 : capacity) / rows), 64 * rows + 16)
    {
      this.drawSlots = drawSlots;
      this.horizontalGap = horizontalGap;
      this.verticalGap = verticalGap;
      this.rows = rows;
      this.capacity = capacity == -1 ? 36 : capacity;
      this.playerInventory = playerInventory;
      this.actualInventory = actualInventory;
      if (actualInventory == null)
        this.actualInventory = (IList<Item>) Game1.player.items;
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) Game1.player.maxItems; ++index)
      {
        if (Game1.player.items.Count <= index)
          Game1.player.items.Add((Item) null);
      }
      for (int index = 0; index < this.capacity; ++index)
        this.inventory.Add(new ClickableComponent(new Rectangle(xPosition + index % (this.capacity / rows) * 64 + horizontalGap * (index % (this.capacity / rows)), this.yPositionOnScreen + index / (this.capacity / rows) * (64 + verticalGap) + (index / (this.capacity / rows) - 1) * 4 - (index > this.capacity / rows || !playerInventory || verticalGap != 0 ? 0 : 12), 64, 64), index.ToString() ?? "")
        {
          myID = index,
          leftNeighborID = index % (this.capacity / rows) != 0 ? index - 1 : 107,
          rightNeighborID = (index + 1) % (this.capacity / rows) != 0 ? index + 1 : 106,
          downNeighborID = index >= this.actualInventory.Count - this.capacity / rows ? 102 : index + this.capacity / rows,
          upNeighborID = index < this.capacity / rows ? 12340 + index : index - this.capacity / rows,
          region = 9000,
          upNeighborImmutable = true,
          downNeighborImmutable = true,
          leftNeighborImmutable = true,
          rightNeighborImmutable = true
        });
      this.highlightMethod = highlightMethod;
      if (highlightMethod == null)
        this.highlightMethod = new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems);
      this.dropItemInvisibleButton = new ClickableComponent(new Rectangle(xPosition - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128, this.yPositionOnScreen - 12, 64, 64), "")
      {
        myID = playerInventory ? 107 : -500,
        rightNeighborID = 0
      };
      foreach (ClickableComponent clickableComponent in this.GetBorder(InventoryMenu.BorderSide.Top))
        clickableComponent.upNeighborImmutable = false;
      foreach (ClickableComponent clickableComponent in this.GetBorder(InventoryMenu.BorderSide.Bottom))
        clickableComponent.downNeighborImmutable = false;
      foreach (ClickableComponent clickableComponent in this.GetBorder(InventoryMenu.BorderSide.Left))
        clickableComponent.leftNeighborImmutable = false;
      foreach (ClickableComponent clickableComponent in this.GetBorder(InventoryMenu.BorderSide.Right))
        clickableComponent.rightNeighborImmutable = false;
    }

    public List<ClickableComponent> GetBorder(InventoryMenu.BorderSide side)
    {
      List<ClickableComponent> border = new List<ClickableComponent>();
      int num = this.capacity / this.rows;
      switch (side)
      {
        case InventoryMenu.BorderSide.Top:
          for (int index = 0; index < this.inventory.Count; ++index)
          {
            if (index < num)
              border.Add(this.inventory[index]);
          }
          break;
        case InventoryMenu.BorderSide.Left:
          for (int index = 0; index < this.inventory.Count; ++index)
          {
            if (index % num == 0)
              border.Add(this.inventory[index]);
          }
          break;
        case InventoryMenu.BorderSide.Right:
          for (int index = 0; index < this.inventory.Count; ++index)
          {
            if (index % num == num - 1)
              border.Add(this.inventory[index]);
          }
          break;
        case InventoryMenu.BorderSide.Bottom:
          for (int index = 0; index < this.inventory.Count; ++index)
          {
            if (index >= this.actualInventory.Count - num)
              border.Add(this.inventory[index]);
          }
          break;
      }
      return border;
    }

    public static bool highlightAllItems(Item i) => true;

    public static bool highlightNoItems(Item i) => false;

    public void movePosition(int x, int y)
    {
      this.xPositionOnScreen += x;
      this.yPositionOnScreen += y;
      foreach (ClickableComponent clickableComponent in this.inventory)
      {
        clickableComponent.bounds.X += x;
        clickableComponent.bounds.Y += y;
      }
      this.dropItemInvisibleButton.bounds.X += x;
      this.dropItemInvisibleButton.bounds.Y += y;
    }

    public void ShakeItem(Item item) => this.ShakeItem(this.actualInventory.IndexOf(item));

    public void ShakeItem(int index)
    {
      if (index < 0 || index >= this.inventory.Count)
        return;
      this._iconShakeTimer[index] = Game1.currentGameTime.TotalGameTime.TotalSeconds + 0.5;
    }

    public Item tryToAddItem(Item toPlace, string sound = "coin")
    {
      if (toPlace == null)
        return (Item) null;
      int stack = toPlace.Stack;
      foreach (ClickableComponent clickableComponent in this.inventory)
      {
        int int32 = Convert.ToInt32(clickableComponent.name);
        if (int32 < this.actualInventory.Count && this.actualInventory[int32] != null && this.highlightMethod(this.actualInventory[int32]) && this.actualInventory[int32].canStackWith((ISalable) toPlace))
        {
          toPlace.Stack = this.actualInventory[int32].addToStack(toPlace);
          if (toPlace.Stack <= 0)
          {
            try
            {
              Game1.playSound(sound);
              if (this.onAddItem != null)
                this.onAddItem(toPlace, this.playerInventory ? Game1.player : (Farmer) null);
            }
            catch (Exception ex)
            {
            }
            return (Item) null;
          }
        }
      }
      foreach (ClickableComponent clickableComponent in this.inventory)
      {
        int int32 = Convert.ToInt32(clickableComponent.name);
        if (int32 < this.actualInventory.Count && (this.actualInventory[int32] == null || this.highlightMethod(this.actualInventory[int32])) && this.actualInventory[int32] == null)
        {
          if (!string.IsNullOrEmpty(sound))
          {
            try
            {
              Game1.playSound(sound);
            }
            catch (Exception ex)
            {
            }
          }
          return Utility.addItemToInventory(toPlace, int32, this.actualInventory, this.onAddItem);
        }
      }
      if (toPlace.Stack < stack)
        Game1.playSound(sound);
      return toPlace;
    }

    public int getInventoryPositionOfClick(int x, int y)
    {
      for (int index = 0; index < this.inventory.Count; ++index)
      {
        if (this.inventory[index] != null && this.inventory[index].bounds.Contains(x, y))
          return Convert.ToInt32(this.inventory[index].name);
      }
      return -1;
    }

    public Item leftClick(int x, int y, Item toPlace, bool playSound = true)
    {
      foreach (ClickableComponent clickableComponent in this.inventory)
      {
        if (clickableComponent.containsPoint(x, y))
        {
          int int32 = Convert.ToInt32(clickableComponent.name);
          if (int32 < this.actualInventory.Count && (this.actualInventory[int32] == null || this.highlightMethod(this.actualInventory[int32]) || this.actualInventory[int32].canStackWith((ISalable) toPlace)))
          {
            if (this.actualInventory[int32] != null)
            {
              if (toPlace != null)
              {
                if (playSound)
                  Game1.playSound("stoneStep");
                return Utility.addItemToInventory(toPlace, int32, this.actualInventory, this.onAddItem);
              }
              if (playSound)
                Game1.playSound(this.moveItemSound);
              return Utility.removeItemFromInventory(int32, this.actualInventory);
            }
            if (toPlace != null)
            {
              if (playSound)
                Game1.playSound("stoneStep");
              return Utility.addItemToInventory(toPlace, int32, this.actualInventory, this.onAddItem);
            }
          }
        }
      }
      return toPlace;
    }

    public Vector2 snapToClickableComponent(int x, int y)
    {
      foreach (ClickableComponent clickableComponent in this.inventory)
      {
        if (clickableComponent.containsPoint(x, y))
          return new Vector2((float) clickableComponent.bounds.X, (float) clickableComponent.bounds.Y);
      }
      return new Vector2((float) x, (float) y);
    }

    public Item getItemAt(int x, int y)
    {
      foreach (ClickableComponent c in this.inventory)
      {
        if (c.containsPoint(x, y))
          return this.getItemFromClickableComponent(c);
      }
      return (Item) null;
    }

    public Item getItemFromClickableComponent(ClickableComponent c)
    {
      if (c != null)
      {
        int int32 = Convert.ToInt32(c.name);
        if (int32 < this.actualInventory.Count)
          return this.actualInventory[int32];
      }
      return (Item) null;
    }

    public Item rightClick(
      int x,
      int y,
      Item toAddTo,
      bool playSound = true,
      bool onlyCheckToolAttachments = false)
    {
      foreach (ClickableComponent clickableComponent in this.inventory)
      {
        int int32 = Convert.ToInt32(clickableComponent.name);
        if (clickableComponent.containsPoint(x, y) && int32 < this.actualInventory.Count && (this.actualInventory[int32] == null || this.highlightMethod(this.actualInventory[int32])) && int32 < this.actualInventory.Count && this.actualInventory[int32] != null)
        {
          if (this.actualInventory[int32] is Tool && (toAddTo == null || toAddTo is StardewValley.Object) && (this.actualInventory[int32] as Tool).canThisBeAttached((StardewValley.Object) toAddTo))
            return (Item) (this.actualInventory[int32] as Tool).attach(toAddTo == null ? (StardewValley.Object) null : (StardewValley.Object) toAddTo);
          if (onlyCheckToolAttachments)
            return toAddTo;
          if (toAddTo == null)
          {
            if (this.actualInventory[int32].maximumStackSize() != -1)
            {
              if (int32 == Game1.player.CurrentToolIndex && this.actualInventory[int32] != null && this.actualInventory[int32].Stack == 1)
                this.actualInventory[int32].actionWhenStopBeingHeld(Game1.player);
              Item one = this.actualInventory[int32].getOne();
              if (this.actualInventory[int32].Stack > 1)
              {
                if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, new InputButton[1]
                {
                  new InputButton(Keys.LeftShift)
                }))
                {
                  one.Stack = (int) Math.Ceiling((double) this.actualInventory[int32].Stack / 2.0);
                  this.actualInventory[int32].Stack /= 2;
                  goto label_17;
                }
              }
              if (this.actualInventory[int32].Stack == 1)
                this.actualInventory[int32] = (Item) null;
              else
                --this.actualInventory[int32].Stack;
label_17:
              if (this.actualInventory[int32] != null && this.actualInventory[int32].Stack <= 0)
                this.actualInventory[int32] = (Item) null;
              if (playSound)
                Game1.playSound(this.moveItemSound);
              return one;
            }
          }
          else if (this.actualInventory[int32].canStackWith((ISalable) toAddTo) && toAddTo.Stack < toAddTo.maximumStackSize())
          {
            if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, new InputButton[1]
            {
              new InputButton(Keys.LeftShift)
            }))
            {
              int val2 = (int) Math.Ceiling((double) this.actualInventory[int32].Stack / 2.0);
              int num = Math.Min(toAddTo.maximumStackSize() - toAddTo.Stack, val2);
              toAddTo.Stack += num;
              this.actualInventory[int32].Stack -= num;
            }
            else
            {
              ++toAddTo.Stack;
              --this.actualInventory[int32].Stack;
            }
            if (playSound)
              Game1.playSound(this.moveItemSound);
            if (this.actualInventory[int32].Stack <= 0)
            {
              if (int32 == Game1.player.CurrentToolIndex)
                this.actualInventory[int32].actionWhenStopBeingHeld(Game1.player);
              this.actualInventory[int32] = (Item) null;
            }
            return toAddTo;
          }
        }
      }
      return toAddTo;
    }

    public Item hover(int x, int y, Item heldItem)
    {
      this.descriptionText = "";
      this.descriptionTitle = "";
      this.hoverText = "";
      this.hoverTitle = "";
      Item o = (Item) null;
      foreach (ClickableComponent clickableComponent in this.inventory)
      {
        int int32 = Convert.ToInt32(clickableComponent.name);
        clickableComponent.scale = Math.Max(1f, clickableComponent.scale - 0.025f);
        if (clickableComponent.containsPoint(x, y) && int32 < this.actualInventory.Count && (this.actualInventory[int32] == null || this.highlightMethod(this.actualInventory[int32])) && int32 < this.actualInventory.Count && this.actualInventory[int32] != null)
        {
          this.descriptionTitle = this.actualInventory[int32].DisplayName;
          this.descriptionText = Environment.NewLine + this.actualInventory[int32].getDescription();
          clickableComponent.scale = Math.Min(clickableComponent.scale + 0.05f, 1.1f);
          string hoverBoxText = this.actualInventory[int32].getHoverBoxText(heldItem);
          if (hoverBoxText != null)
          {
            this.hoverText = hoverBoxText;
            this.hoverTitle = this.actualInventory[int32].DisplayName;
          }
          else
          {
            this.hoverText = this.actualInventory[int32].getDescription();
            this.hoverTitle = this.actualInventory[int32].DisplayName;
          }
          if (o == null)
            o = this.actualInventory[int32];
        }
      }
      if (o != null && o is StardewValley.Object && (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).couldThisIngredienteBeUsedInABundle(o as StardewValley.Object))
        GameMenu.bundleItemHovered = true;
      return o;
    }

    public override void setUpForGamePadMode()
    {
      base.setUpForGamePadMode();
      if (this.inventory == null || this.inventory.Count <= 0)
        return;
      Game1.setMousePosition(this.inventory[0].bounds.Right - this.inventory[0].bounds.Width / 8, this.inventory[0].bounds.Bottom - this.inventory[0].bounds.Height / 8);
    }

    public override void draw(SpriteBatch b) => this.draw(b, -1, -1, -1);

    public override void draw(SpriteBatch b, int red = -1, int green = -1, int blue = -1)
    {
      for (int key = 0; key < this.inventory.Count; ++key)
      {
        if (this._iconShakeTimer.ContainsKey(key) && Game1.currentGameTime.TotalGameTime.TotalSeconds >= this._iconShakeTimer[key])
          this._iconShakeTimer.Remove(key);
      }
      Color color = red == -1 ? Color.White : new Color((int) Utility.Lerp((float) red, (float) Math.Min((int) byte.MaxValue, red + 150), 0.65f), (int) Utility.Lerp((float) green, (float) Math.Min((int) byte.MaxValue, green + 150), 0.65f), (int) Utility.Lerp((float) blue, (float) Math.Min((int) byte.MaxValue, blue + 150), 0.65f));
      Texture2D texture = red == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
      if (this.drawSlots)
      {
        for (int index = 0; index < this.capacity; ++index)
        {
          Vector2 position = new Vector2((float) (this.xPositionOnScreen + index % (this.capacity / this.rows) * 64 + this.horizontalGap * (index % (this.capacity / this.rows))), (float) (this.yPositionOnScreen + index / (this.capacity / this.rows) * (64 + this.verticalGap) + (index / (this.capacity / this.rows) - 1) * 4 - (index >= this.capacity / this.rows || !this.playerInventory || this.verticalGap != 0 ? 0 : 12)));
          b.Draw(texture, position, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), color, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
          if ((this.playerInventory || this.showGrayedOutSlots) && index >= (int) (NetFieldBase<int, NetInt>) Game1.player.maxItems)
            b.Draw(texture, position, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57)), color * 0.5f, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
          if (!Game1.options.gamepadControls && index < 12 && this.playerInventory)
          {
            string str;
            switch (index)
            {
              case 9:
                str = "0";
                break;
              case 10:
                str = "-";
                break;
              case 11:
                str = "=";
                break;
              default:
                str = (index + 1).ToString() ?? "";
                break;
            }
            string text = str;
            Vector2 vector2 = Game1.tinyFont.MeasureString(text);
            b.DrawString(Game1.tinyFont, text, position + new Vector2((float) (32.0 - (double) vector2.X / 2.0), -vector2.Y), index == Game1.player.CurrentToolIndex ? Color.Red : Color.DimGray);
          }
        }
        for (int index = 0; index < this.capacity; ++index)
        {
          Vector2 location = new Vector2((float) (this.xPositionOnScreen + index % (this.capacity / this.rows) * 64 + this.horizontalGap * (index % (this.capacity / this.rows))), (float) (this.yPositionOnScreen + index / (this.capacity / this.rows) * (64 + this.verticalGap) + (index / (this.capacity / this.rows) - 1) * 4 - (index >= this.capacity / this.rows || !this.playerInventory || this.verticalGap != 0 ? 0 : 12)));
          if (this.actualInventory.Count > index && this.actualInventory.ElementAt<Item>(index) != null)
          {
            bool drawShadow = this.highlightMethod(this.actualInventory[index]);
            if (this._iconShakeTimer.ContainsKey(index))
              location += 1f * new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
            this.actualInventory[index].drawInMenu(b, location, this.inventory.Count > index ? this.inventory[index].scale : 1f, !this.highlightMethod(this.actualInventory[index]) ? 0.25f : 1f, 0.865f, StackDrawType.Draw, Color.White, drawShadow);
          }
        }
      }
      else
      {
        for (int index = 0; index < this.capacity; ++index)
        {
          Vector2 location = new Vector2((float) (this.xPositionOnScreen + index % (this.capacity / this.rows) * 64 + this.horizontalGap * (index % (this.capacity / this.rows))), (float) (this.yPositionOnScreen + index / (this.capacity / this.rows) * (64 + this.verticalGap) + (index / (this.capacity / this.rows) - 1) * 4 - (index >= this.capacity / this.rows || !this.playerInventory || this.verticalGap != 0 ? 0 : 12)));
          if (this.actualInventory.Count > index && this.actualInventory.ElementAt<Item>(index) != null)
          {
            bool drawShadow = this.highlightMethod(this.actualInventory[index]);
            if (this._iconShakeTimer.ContainsKey(index))
              location += 1f * new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
            this.actualInventory[index].drawInMenu(b, location, this.inventory.Count > index ? this.inventory[index].scale : 1f, !drawShadow ? 0.25f : 1f, 0.865f, StackDrawType.Draw, Color.White, drawShadow);
          }
        }
      }
    }

    public List<Vector2> GetSlotDrawPositions()
    {
      List<Vector2> slotDrawPositions = new List<Vector2>();
      for (int index = 0; index < this.capacity; ++index)
        slotDrawPositions.Add(new Vector2((float) (this.xPositionOnScreen + index % (this.capacity / this.rows) * 64 + this.horizontalGap * (index % (this.capacity / this.rows))), (float) (this.yPositionOnScreen + index / (this.capacity / this.rows) * (64 + this.verticalGap) + (index / (this.capacity / this.rows) - 1) * 4 - (index >= this.capacity / this.rows || !this.playerInventory || this.verticalGap != 0 ? 0 : 12))));
      return slotDrawPositions;
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) => base.gameWindowSizeChanged(oldBounds, newBounds);

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
    }

    public delegate bool highlightThisItem(Item i);

    public enum BorderSide
    {
      Top,
      Left,
      Right,
      Bottom,
    }
  }
}
