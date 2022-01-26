// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.NumberSelectionMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace StardewValley.Menus
{
  public class NumberSelectionMenu : IClickableMenu
  {
    public const int region_leftButton = 101;
    public const int region_rightButton = 102;
    public const int region_okButton = 103;
    public const int region_cancelButton = 104;
    private string message;
    protected int price;
    protected int minValue;
    protected int maxValue;
    protected int currentValue;
    protected int priceShake;
    protected int heldTimer;
    private NumberSelectionMenu.behaviorOnNumberSelect behaviorFunction;
    protected TextBox numberSelectedBox;
    public ClickableTextureComponent leftButton;
    public ClickableTextureComponent rightButton;
    public ClickableTextureComponent okButton;
    public ClickableTextureComponent cancelButton;

    public NumberSelectionMenu(
      string message,
      NumberSelectionMenu.behaviorOnNumberSelect behaviorOnSelection,
      int price = -1,
      int minValue = 0,
      int maxValue = 99,
      int defaultNumber = 0)
    {
      Vector2 vector2 = Game1.dialogueFont.MeasureString(message);
      int width = Math.Max((int) vector2.X, 600) + IClickableMenu.borderWidth * 2;
      int height = (int) vector2.Y + IClickableMenu.borderWidth * 2 + 160;
      this.initialize((int) this.centerPosition.X - width / 2, (int) this.centerPosition.Y - height / 2, width, height);
      this.message = message;
      this.price = price;
      this.minValue = minValue;
      this.maxValue = maxValue;
      this.currentValue = defaultNumber;
      this.behaviorFunction = behaviorOnSelection;
      this.numberSelectedBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D) null, Game1.smallFont, Game1.textColor)
      {
        X = this.xPositionOnScreen + IClickableMenu.borderWidth + 56,
        Y = this.yPositionOnScreen + IClickableMenu.borderWidth + this.height / 2,
        Text = this.currentValue.ToString() ?? "",
        numbersOnly = true,
        textLimit = (maxValue.ToString() ?? "").Length
      };
      this.numberSelectedBox.SelectMe();
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + this.height / 2, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent1.myID = 101;
      textureComponent1.rightNeighborID = 102;
      textureComponent1.upNeighborID = -99998;
      this.leftButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth + 64 + this.numberSelectedBox.Width, this.yPositionOnScreen + IClickableMenu.borderWidth + this.height / 2, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent2.myID = 102;
      textureComponent2.leftNeighborID = 101;
      textureComponent2.rightNeighborID = 103;
      textureComponent2.upNeighborID = -99998;
      this.rightButton = textureComponent2;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 21, 64, 64), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent3.myID = 103;
      textureComponent3.leftNeighborID = 102;
      textureComponent3.rightNeighborID = 104;
      textureComponent3.upNeighborID = -99998;
      this.okButton = textureComponent3;
      ClickableTextureComponent textureComponent4 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 21, 64, 64), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
      textureComponent4.myID = 104;
      textureComponent4.leftNeighborID = 103;
      textureComponent4.upNeighborID = -99998;
      this.cancelButton = textureComponent4;
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    protected virtual Vector2 centerPosition => new Vector2((float) (Game1.uiViewport.Width / 2), (float) (Game1.uiViewport.Height / 2));

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(102);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void gamePadButtonHeld(Buttons b)
    {
      base.gamePadButtonHeld(b);
      if (b != Buttons.A || this.currentlySnappedComponent == null)
        return;
      this.heldTimer += Game1.currentGameTime.ElapsedGameTime.Milliseconds;
      if (this.heldTimer <= 300)
        return;
      int num1 = (int) Math.Pow(10.0, (double) ((this.heldTimer - 300) / 3000));
      if (this.currentlySnappedComponent.myID == 102)
      {
        int val1 = this.currentValue + num1;
        int val2 = int.MaxValue;
        if (this.price != -1 && this.price != 0)
          val2 = Game1.player.Money / this.price;
        int num2 = Math.Min(val1, Math.Min(this.maxValue, val2));
        if (num2 == this.currentValue)
          return;
        this.rightButton.scale = this.rightButton.baseScale;
        this.currentValue = num2;
        this.numberSelectedBox.Text = this.currentValue.ToString() ?? "";
      }
      else
      {
        if (this.currentlySnappedComponent.myID != 101)
          return;
        int num3 = Math.Max(this.currentValue - num1, this.minValue);
        if (num3 == this.currentValue)
          return;
        this.leftButton.scale = this.leftButton.baseScale;
        this.currentValue = num3;
        this.numberSelectedBox.Text = this.currentValue.ToString() ?? "";
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.leftButton.containsPoint(x, y))
      {
        int num = this.currentValue - 1;
        if (num >= this.minValue)
        {
          this.leftButton.scale = this.leftButton.baseScale;
          this.currentValue = num;
          this.numberSelectedBox.Text = this.currentValue.ToString() ?? "";
          Game1.playSound("smallSelect");
        }
      }
      if (this.rightButton.containsPoint(x, y))
      {
        int num = this.currentValue + 1;
        if (num <= this.maxValue && (this.price == -1 || num * this.price <= Game1.player.Money))
        {
          this.rightButton.scale = this.rightButton.baseScale;
          this.currentValue = num;
          this.numberSelectedBox.Text = this.currentValue.ToString() ?? "";
          Game1.playSound("smallSelect");
        }
      }
      if (this.okButton.containsPoint(x, y))
      {
        if (this.currentValue > this.maxValue || this.currentValue < this.minValue)
        {
          this.currentValue = Math.Max(this.minValue, Math.Min(this.maxValue, this.currentValue));
          this.numberSelectedBox.Text = this.currentValue.ToString() ?? "";
        }
        else
          this.behaviorFunction(this.currentValue, this.price, Game1.player);
        Game1.playSound("smallSelect");
      }
      if (this.cancelButton.containsPoint(x, y))
      {
        Game1.exitActiveMenu();
        Game1.playSound("bigDeSelect");
        Game1.player.canMove = true;
      }
      this.numberSelectedBox.Update();
    }

    public override void receiveKeyPress(Keys key)
    {
      base.receiveKeyPress(key);
      if (key != Keys.Enter)
        return;
      this.receiveLeftClick(this.okButton.bounds.Center.X, this.okButton.bounds.Center.Y, true);
    }

    public override void update(GameTime time)
    {
      base.update(time);
      this.currentValue = 0;
      if (this.numberSelectedBox.Text != null)
        int.TryParse(this.numberSelectedBox.Text, out this.currentValue);
      if (this.priceShake > 0)
        this.priceShake -= time.ElapsedGameTime.Milliseconds;
      if (!Game1.options.SnappyMenus)
        return;
      GamePadState oldPadState = Game1.oldPadState;
      if (Game1.oldPadState.IsButtonDown(Buttons.A))
        return;
      this.heldTimer = 0;
    }

    public override void performHoverAction(int x, int y)
    {
      if (this.okButton.containsPoint(x, y) && (this.price == -1 || this.currentValue > this.minValue))
        this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.2f);
      else
        this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
      if (this.cancelButton.containsPoint(x, y))
        this.cancelButton.scale = Math.Min(this.cancelButton.scale + 0.02f, this.cancelButton.baseScale + 0.2f);
      else
        this.cancelButton.scale = Math.Max(this.cancelButton.scale - 0.02f, this.cancelButton.baseScale);
      if (this.leftButton.containsPoint(x, y))
        this.leftButton.scale = Math.Min(this.leftButton.scale + 0.02f, this.leftButton.baseScale + 0.2f);
      else
        this.leftButton.scale = Math.Max(this.leftButton.scale - 0.02f, this.leftButton.baseScale);
      if (this.rightButton.containsPoint(x, y))
        this.rightButton.scale = Math.Min(this.rightButton.scale + 0.02f, this.rightButton.baseScale + 0.2f);
      else
        this.rightButton.scale = Math.Max(this.rightButton.scale - 0.02f, this.rightButton.baseScale);
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
      Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
      b.DrawString(Game1.dialogueFont, this.message, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2)), Game1.textColor);
      this.okButton.draw(b);
      this.cancelButton.draw(b);
      this.leftButton.draw(b);
      this.rightButton.draw(b);
      if (this.price != -1)
        b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) (this.price * this.currentValue)), new Vector2((float) (this.rightButton.bounds.Right + 32 + (this.priceShake > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (this.rightButton.bounds.Y + (this.priceShake > 0 ? Game1.random.Next(-1, 2) : 0))), this.currentValue * this.price > Game1.player.Money ? Color.Red : Game1.textColor);
      this.numberSelectedBox.Draw(b);
      this.drawMouse(b);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public delegate void behaviorOnNumberSelect(int number, int price, Farmer who);
  }
}
