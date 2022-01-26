// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.MenuWithInventory
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StardewValley.Menus
{
  public class MenuWithInventory : IClickableMenu
  {
    public const int region_okButton = 4857;
    public const int region_trashCan = 5948;
    public string descriptionText = "";
    public string hoverText = "";
    public string descriptionTitle = "";
    public InventoryMenu inventory;
    public Item heldItem;
    public Item hoveredItem;
    public int wiggleWordsTimer;
    public int hoverAmount;
    public ClickableTextureComponent okButton;
    public ClickableTextureComponent trashCan;
    public float trashCanLidRotation;
    public ClickableComponent dropItemInvisibleButton;

    public MenuWithInventory(
      InventoryMenu.highlightThisItem highlighterMethod = null,
      bool okButton = false,
      bool trashCan = false,
      int inventoryXOffset = 0,
      int inventoryYOffset = 0,
      int menuOffsetHack = 0)
      : base(Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 + menuOffsetHack, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2)
    {
      if (this.yPositionOnScreen < IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder)
        this.yPositionOnScreen = IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder;
      if (this.xPositionOnScreen < 0)
        this.xPositionOnScreen = 0;
      int yPosition = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 192 - 16 + inventoryYOffset;
      this.inventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + inventoryXOffset, yPosition, false, highlightMethod: highlighterMethod);
      if (okButton)
      {
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 192 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
        textureComponent.myID = 4857;
        textureComponent.upNeighborID = 5948;
        textureComponent.leftNeighborID = 12;
        this.okButton = textureComponent;
      }
      if (trashCan)
      {
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 192 - 32 - IClickableMenu.borderWidth - 104, 64, 104), Game1.mouseCursors, new Rectangle(564 + Game1.player.trashCanLevel * 18, 102, 18, 26), 4f);
        textureComponent.myID = 5948;
        textureComponent.downNeighborID = 4857;
        textureComponent.leftNeighborID = 12;
        textureComponent.upNeighborID = 106;
        this.trashCan = textureComponent;
      }
      this.dropItemInvisibleButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128, yPosition - 12, 64, 64), "")
      {
        myID = 107,
        rightNeighborID = 0
      };
    }

    public void movePosition(int dx, int dy)
    {
      this.xPositionOnScreen += dx;
      this.yPositionOnScreen += dy;
      this.inventory.movePosition(dx, dy);
      if (this.okButton != null)
      {
        this.okButton.bounds.X += dx;
        this.okButton.bounds.Y += dy;
      }
      if (this.trashCan != null)
      {
        this.trashCan.bounds.X += dx;
        this.trashCan.bounds.Y += dy;
      }
      if (this.dropItemInvisibleButton == null)
        return;
      this.dropItemInvisibleButton.bounds.X += dx;
      this.dropItemInvisibleButton.bounds.Y += dy;
    }

    public override bool readyToClose() => this.heldItem == null;

    public override bool isWithinBounds(int x, int y) => base.isWithinBounds(x, y);

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      this.heldItem = this.inventory.leftClick(x, y, this.heldItem, playSound);
      if (!this.isWithinBounds(x, y) && this.readyToClose() && this.trashCan != null)
        this.trashCan.containsPoint(x, y);
      if (this.okButton != null && this.okButton.containsPoint(x, y) && this.readyToClose())
      {
        this.exitThisMenu();
        if (Game1.currentLocation.currentEvent != null && Game1.currentLocation.currentEvent.CurrentCommand > 0)
          ++Game1.currentLocation.currentEvent.CurrentCommand;
        Game1.playSound("bigDeSelect");
      }
      if (this.trashCan == null || !this.trashCan.containsPoint(x, y) || this.heldItem == null || !this.heldItem.canBeTrashed())
        return;
      Utility.trashItem(this.heldItem);
      this.heldItem = (Item) null;
    }

    public override void receiveRightClick(int x, int y, bool playSound = true) => this.heldItem = this.inventory.rightClick(x, y, this.heldItem, playSound);

    public void receiveRightClickOnlyToolAttachments(int x, int y) => this.heldItem = this.inventory.rightClick(x, y, this.heldItem, onlyCheckToolAttachments: true);

    public override void performHoverAction(int x, int y)
    {
      this.descriptionText = "";
      this.descriptionTitle = "";
      this.hoveredItem = this.inventory.hover(x, y, this.heldItem);
      this.hoverText = this.inventory.hoverText;
      this.hoverAmount = 0;
      if (this.okButton != null)
      {
        if (this.okButton.containsPoint(x, y))
          this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
        else
          this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
      }
      if (this.trashCan == null)
        return;
      if (this.trashCan.containsPoint(x, y))
      {
        if ((double) this.trashCanLidRotation <= 0.0)
          Game1.playSound("trashcanlid");
        this.trashCanLidRotation = Math.Min(this.trashCanLidRotation + (float) Math.PI / 48f, 1.570796f);
        if (this.heldItem == null || Utility.getTrashReclamationPrice(this.heldItem, Game1.player) <= 0)
          return;
        this.hoverText = Game1.content.LoadString("Strings\\UI:TrashCanSale");
        this.hoverAmount = Utility.getTrashReclamationPrice(this.heldItem, Game1.player);
      }
      else
        this.trashCanLidRotation = Math.Max(this.trashCanLidRotation - (float) Math.PI / 48f, 0.0f);
    }

    public override void update(GameTime time)
    {
      if (this.wiggleWordsTimer <= 0)
        return;
      this.wiggleWordsTimer -= time.ElapsedGameTime.Milliseconds;
    }

    public virtual void draw(
      SpriteBatch b,
      bool drawUpperPortion = true,
      bool drawDescriptionArea = true,
      int red = -1,
      int green = -1,
      int blue = -1)
    {
      if (this.trashCan != null)
      {
        this.trashCan.draw(b);
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.trashCan.bounds.X + 60), (float) (this.trashCan.bounds.Y + 40)), new Rectangle?(new Rectangle(564 + Game1.player.trashCanLevel * 18, 129, 18, 10)), Color.White, this.trashCanLidRotation, new Vector2(16f, 10f), 4f, SpriteEffects.None, 0.86f);
      }
      if (drawUpperPortion)
      {
        Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, ignoreTitleSafe: false, r: red, g: green, b: blue);
        this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256, red: red, green: green, blue: blue);
        if (drawDescriptionArea)
        {
          this.drawVerticalUpperIntersectingPartition(b, this.xPositionOnScreen + 576, 328, red, green, blue);
          if (!this.descriptionText.Equals(""))
          {
            int x = this.xPositionOnScreen + 576 + 42 + (this.wiggleWordsTimer > 0 ? Game1.random.Next(-2, 3) : 0);
            int y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 32 + (this.wiggleWordsTimer > 0 ? Game1.random.Next(-2, 3) : 0);
            int num = 320;
            float scale = 0.0f;
            string text;
            do
            {
              if ((double) scale == 0.0)
                scale = 1f;
              else
                scale -= 0.1f;
              text = Game1.parseText(this.descriptionText, Game1.smallFont, (int) (224.0 / (double) scale));
            }
            while ((double) Game1.smallFont.MeasureString(text).Y > (double) num / (double) scale && (double) scale > 0.5);
            if (red == -1)
              Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((float) x, (float) y), Game1.textColor * 0.75f, scale);
            else
              Utility.drawTextWithColoredShadow(b, text, Game1.smallFont, new Vector2((float) x, (float) y), Game1.textColor * 0.75f, Color.Black * 0.2f, scale);
          }
        }
      }
      else
        Game1.drawDialogueBox(this.xPositionOnScreen - IClickableMenu.borderWidth / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 64, this.width, this.height - (IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192), false, true);
      if (this.okButton != null)
        this.okButton.draw(b);
      this.inventory.draw(b, red, green, blue);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      if (this.yPositionOnScreen < IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder)
        this.yPositionOnScreen = IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder;
      if (this.xPositionOnScreen < 0)
        this.xPositionOnScreen = 0;
      int yPosition = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 192 - 16;
      string moveItemSound = this.inventory.moveItemSound;
      this.inventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2, yPosition, false, highlightMethod: this.inventory.highlightMethod);
      this.inventory.moveItemSound = moveItemSound;
      if (this.okButton != null)
        this.okButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 192 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      if (this.trashCan == null)
        return;
      this.trashCan = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 192 - 32 - IClickableMenu.borderWidth - 104, 64, 104), Game1.mouseCursors, new Rectangle(669, 261, 16, 26), 4f);
    }

    public override void draw(SpriteBatch b) => throw new NotImplementedException();
  }
}
