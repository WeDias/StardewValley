// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ItemListMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class ItemListMenu : IClickableMenu
  {
    public const int region_okbutton = 101;
    public const int region_forwardButton = 102;
    public const int region_backButton = 103;
    public int itemsPerCategoryPage = 8;
    public ClickableTextureComponent okButton;
    public ClickableTextureComponent forwardButton;
    public ClickableTextureComponent backButton;
    private List<Item> itemsToList;
    private string title;
    private int currentTab;
    private int totalValueOfItems;

    public ItemListMenu(string menuTitle, List<Item> itemList)
    {
      this.title = menuTitle;
      this.itemsToList = itemList;
      foreach (Item i in itemList)
        this.totalValueOfItems += Utility.getSellToStorePriceOfItem(i);
      this.itemsToList.Add((Item) null);
      int num1 = Game1.uiViewport.Width / 2;
      int num2 = Game1.uiViewport.Height / 2;
      this.width = Math.Min(800, Game1.uiViewport.Width - 128);
      this.height = Math.Min(720, Game1.uiViewport.Height - 128);
      if (this.height <= 720)
        this.itemsPerCategoryPage = 7;
      this.xPositionOnScreen = num1 - this.width / 2;
      this.yPositionOnScreen = num2 - this.height / 2;
      Rectangle bounds = new Rectangle(num1 + this.width / 2 + 4, num2 + this.height / 2 - 96, 64, 64);
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11382"), bounds, (string) null, Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11382"), Game1.mouseCursors, new Rectangle(128, 256, 64, 64), 1f);
      textureComponent1.myID = 101;
      textureComponent1.leftNeighborID = -7777;
      this.okButton = textureComponent1;
      if (Game1.options.gamepadControls)
        Game1.setMousePositionRaw(bounds.Center.X, bounds.Center.Y);
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen - 64, this.yPositionOnScreen + this.height - 64, 48, 44), (string) null, "", Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent2.myID = 103;
      textureComponent2.rightNeighborID = -7777;
      this.backButton = textureComponent2;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width - 32 - 48, this.yPositionOnScreen + this.height - 64, 48, 44), (string) null, "", Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent3.myID = 102;
      textureComponent3.leftNeighborID = 103;
      textureComponent3.rightNeighborID = 101;
      this.forwardButton = textureComponent3;
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(101);
      this.snapCursorToCurrentSnappedComponent();
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
      if (oldID == 103 && direction == 1)
      {
        if (this.showForwardButton())
        {
          this.currentlySnappedComponent = this.getComponentWithID(102);
          this.snapCursorToCurrentSnappedComponent();
        }
        else
          this.snapToDefaultClickableComponent();
      }
      else
      {
        if (oldID != 101 || direction != 3)
          return;
        if (this.showForwardButton())
        {
          this.currentlySnappedComponent = this.getComponentWithID(102);
          this.snapCursorToCurrentSnappedComponent();
        }
        else
        {
          if (!this.showBackButton())
            return;
          this.currentlySnappedComponent = this.getComponentWithID(103);
          this.snapCursorToCurrentSnappedComponent();
        }
      }
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      switch (b)
      {
        case Buttons.B:
          this.okClicked();
          break;
        case Buttons.RightTrigger:
          if (!this.showForwardButton())
            break;
          ++this.currentTab;
          Game1.playSound("shwip");
          break;
        case Buttons.LeftTrigger:
          if (!this.showBackButton())
            break;
          --this.currentTab;
          Game1.playSound("shwip");
          break;
      }
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      this.okButton.tryHover(x, y);
      this.backButton.tryHover(x, y);
      this.forwardButton.tryHover(x, y);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);
      if (this.okButton.containsPoint(x, y))
        this.okClicked();
      if (this.backButton.containsPoint(x, y))
      {
        if (this.currentTab != 0)
          --this.currentTab;
        Game1.playSound("shwip");
      }
      else
      {
        if (!this.showForwardButton() || !this.forwardButton.containsPoint(x, y))
          return;
        ++this.currentTab;
        Game1.playSound("shwip");
      }
    }

    private void okClicked()
    {
      Game1.activeClickableMenu = (IClickableMenu) null;
      if (Game1.CurrentEvent != null)
        ++Game1.CurrentEvent.CurrentCommand;
      Game1.playSound("bigDeSelect");
    }

    public override void draw(SpriteBatch b)
    {
      IClickableMenu.drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White);
      SpriteText.drawStringHorizontallyCenteredAt(b, this.title, this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + 32 + 12);
      Vector2 location = new Vector2((float) (this.xPositionOnScreen + 32), (float) (this.yPositionOnScreen + 96 + 4));
      for (int index = this.currentTab * this.itemsPerCategoryPage; index < this.currentTab * this.itemsPerCategoryPage + this.itemsPerCategoryPage; ++index)
      {
        if (this.itemsToList.Count > index)
        {
          if (this.itemsToList[index] == null)
          {
            SpriteText.drawString(b, Game1.content.LoadString("Strings\\UI:ItemList_ItemsLostValue", (object) this.totalValueOfItems), (int) location.X + 64 + 12, (int) location.Y + 12);
          }
          else
          {
            this.itemsToList[index].drawInMenu(b, location, 1f, 1f, 1f, StackDrawType.Draw_OneInclusive);
            SpriteText.drawString(b, this.itemsToList[index].DisplayName, (int) location.X + 64 + 12, (int) location.Y + 12);
            location.Y += 68f;
          }
        }
      }
      if (this.showBackButton())
        this.backButton.draw(b);
      if (this.showForwardButton())
        this.forwardButton.draw(b);
      this.okButton.draw(b);
      Game1.mouseCursorTransparency = 1f;
      this.drawMouse(b);
    }

    public bool showBackButton() => this.currentTab > 0;

    public bool showForwardButton() => this.itemsToList.Count > this.itemsPerCategoryPage * (this.currentTab + 1);
  }
}
