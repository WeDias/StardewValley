// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.MuseumMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using xTile.Dimensions;

namespace StardewValley.Menus
{
  public class MuseumMenu : MenuWithInventory
  {
    public const int startingState = 0;
    public const int placingInMuseumState = 1;
    public const int exitingState = 2;
    public int fadeTimer;
    public int state;
    public int menuPositionOffset;
    public bool fadeIntoBlack;
    public bool menuMovingDown;
    public float blackFadeAlpha;
    public SparklingText sparkleText;
    public Vector2 globalLocationOfSparklingArtifact;
    private bool holdingMuseumPiece;
    public bool reOrganizing;

    public MuseumMenu(InventoryMenu.highlightThisItem highlighterMethod)
      : base(highlighterMethod, true)
    {
      this.fadeTimer = 800;
      this.fadeIntoBlack = true;
      this.movePosition(0, Game1.uiViewport.Height - this.yPositionOnScreen - this.height);
      Game1.player.forceCanMove();
      if (Game1.options.SnappyMenus)
      {
        if (this.okButton != null)
          this.okButton.myID = 106;
        this.populateClickableComponentList();
        this.currentlySnappedComponent = this.getComponentWithID(0);
        this.snapCursorToCurrentSnappedComponent();
      }
      Game1.displayHUD = false;
    }

    public override bool shouldClampGamePadCursor() => true;

    public override void receiveKeyPress(Keys key)
    {
      if (this.fadeTimer > 0)
        return;
      if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.menuButton) && this.readyToClose())
      {
        this.state = 2;
        this.fadeTimer = 500;
        this.fadeIntoBlack = true;
      }
      else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.menuButton) && !this.holdingMuseumPiece && this.menuMovingDown)
      {
        if (this.heldItem != null)
        {
          Game1.playSound("bigDeSelect");
          Utility.CollectOrDrop(this.heldItem);
          this.heldItem = (Item) null;
        }
        this.ReturnToDonatableItems();
      }
      else if (Game1.options.SnappyMenus && this.heldItem == null && !this.reOrganizing)
        base.receiveKeyPress(key);
      if (!Game1.options.SnappyMenus)
      {
        if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
          Game1.panScreen(0, 4);
        else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
          Game1.panScreen(4, 0);
        else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
        {
          Game1.panScreen(0, -4);
        }
        else
        {
          if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
            return;
          Game1.panScreen(-4, 0);
        }
      }
      else
      {
        if (this.heldItem == null && !this.reOrganizing)
          return;
        LibraryMuseum currentLocation = Game1.currentLocation as LibraryMuseum;
        Vector2 vector2 = new Vector2((float) (int) (((double) Utility.ModifyCoordinateFromUIScale((float) Game1.getMouseX()) + (double) Game1.viewport.X) / 64.0), (float) (int) (((double) Utility.ModifyCoordinateFromUIScale((float) Game1.getMouseY()) + (double) Game1.viewport.Y) / 64.0));
        if (!currentLocation.isTileSuitableForMuseumPiece((int) vector2.X, (int) vector2.Y) && (!this.reOrganizing || !currentLocation.museumPieces.ContainsKey(vector2)))
        {
          vector2 = currentLocation.getFreeDonationSpot();
          Game1.setMousePosition((int) Utility.ModifyCoordinateForUIScale((float) ((double) vector2.X * 64.0 - (double) Game1.viewport.X + 32.0)), (int) Utility.ModifyCoordinateForUIScale((float) ((double) vector2.Y * 64.0 - (double) Game1.viewport.Y + 32.0)));
        }
        else
        {
          if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton))
            vector2 = currentLocation.findMuseumPieceLocationInDirection(vector2, 0, 21, !this.reOrganizing);
          else if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton))
            vector2 = currentLocation.findMuseumPieceLocationInDirection(vector2, 1, 21, !this.reOrganizing);
          else if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton))
            vector2 = currentLocation.findMuseumPieceLocationInDirection(vector2, 2, 21, !this.reOrganizing);
          else if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton))
            vector2 = currentLocation.findMuseumPieceLocationInDirection(vector2, 3, 21, !this.reOrganizing);
          if (!Game1.viewport.Contains(new Location((int) ((double) vector2.X * 64.0 + 32.0), Game1.viewport.Y + 1)))
            Game1.panScreen((int) ((double) vector2.X * 64.0 - (double) Game1.viewport.X), 0);
          else if (!Game1.viewport.Contains(new Location(Game1.viewport.X + 1, (int) ((double) vector2.Y * 64.0 + 32.0))))
            Game1.panScreen(0, (int) ((double) vector2.Y * 64.0 - (double) Game1.viewport.Y));
          Game1.setMousePosition((int) Utility.ModifyCoordinateForUIScale((float) ((int) vector2.X * 64 - Game1.viewport.X + 32)), (int) Utility.ModifyCoordinateForUIScale((float) ((int) vector2.Y * 64 - Game1.viewport.Y + 32)));
        }
      }
    }

    public override bool overrideSnappyMenuCursorMovementBan() => false;

    public override void receiveGamePadButton(Buttons b)
    {
      if (b == Buttons.B)
      {
        if (this.holdingMuseumPiece)
          return;
        int fadeTimer = this.fadeTimer;
      }
      else
      {
        if (this.menuMovingDown || b != Buttons.DPadUp && b != Buttons.LeftThumbstickUp || !Game1.options.SnappyMenus || this.currentlySnappedComponent == null || this.currentlySnappedComponent.myID >= 12)
          return;
        this.reOrganizing = true;
        this.menuMovingDown = true;
        this.receiveKeyPress(Game1.options.moveUpButton[0].key);
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.fadeTimer > 0)
        return;
      Item heldItem = this.heldItem;
      if (!this.holdingMuseumPiece)
      {
        int inventoryPositionOfClick = this.inventory.getInventoryPositionOfClick(x, y);
        if (this.heldItem == null)
        {
          if (inventoryPositionOfClick >= 0 && inventoryPositionOfClick < this.inventory.actualInventory.Count && this.inventory.highlightMethod(this.inventory.actualInventory[inventoryPositionOfClick]))
          {
            this.heldItem = this.inventory.actualInventory[inventoryPositionOfClick].getOne();
            --this.inventory.actualInventory[inventoryPositionOfClick].Stack;
            if (this.inventory.actualInventory[inventoryPositionOfClick].Stack <= 0)
              this.inventory.actualInventory[inventoryPositionOfClick] = (Item) null;
          }
        }
        else
          this.heldItem = this.inventory.leftClick(x, y, this.heldItem);
      }
      if (heldItem == null && this.heldItem != null && Game1.isAnyGamePadButtonBeingPressed())
        this.receiveGamePadButton(Buttons.DPadUp);
      if (heldItem != null && this.heldItem != null && (y < Game1.viewport.Height - (this.height - (IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192)) || this.menuMovingDown))
      {
        int x1 = (int) ((double) Utility.ModifyCoordinateFromUIScale((float) x) + (double) Game1.viewport.X) / 64;
        int y1 = (int) ((double) Utility.ModifyCoordinateFromUIScale((float) y) + (double) Game1.viewport.Y) / 64;
        if ((Game1.currentLocation as LibraryMuseum).isTileSuitableForMuseumPiece(x1, y1) && (Game1.currentLocation as LibraryMuseum).isItemSuitableForDonation(this.heldItem))
        {
          int parentSheetIndex = (int) (NetFieldBase<int, NetInt>) this.heldItem.parentSheetIndex;
          int count = (Game1.currentLocation as LibraryMuseum).getRewardsForPlayer(Game1.player).Count;
          (Game1.currentLocation as LibraryMuseum).museumPieces.Add(new Vector2((float) x1, (float) y1), (this.heldItem as Object).parentSheetIndex);
          Game1.playSound("stoneStep");
          if ((Game1.currentLocation as LibraryMuseum).getRewardsForPlayer(Game1.player).Count > count && !this.holdingMuseumPiece)
          {
            this.sparkleText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:NewReward"), Color.MediumSpringGreen, Color.White);
            Game1.playSound("reward");
            this.globalLocationOfSparklingArtifact = new Vector2((float) (x1 * 64 + 32) - this.sparkleText.textWidth / 2f, (float) (y1 * 64 - 48));
          }
          else
            Game1.playSound("newArtifact");
          Game1.player.completeQuest(24);
          --this.heldItem.Stack;
          if (this.heldItem.Stack <= 0)
            this.heldItem = (Item) null;
          int num = (Game1.currentLocation as LibraryMuseum).museumPieces.Count();
          if (!this.holdingMuseumPiece)
          {
            Game1.stats.checkForArchaeologyAchievements();
            switch (num)
            {
              case 40:
                Game1.multiplayer.globalChatInfoMessage("Museum40", (string) (NetFieldBase<string, NetString>) Game1.player.farmName);
                break;
              case 95:
                Game1.multiplayer.globalChatInfoMessage("MuseumComplete", (string) (NetFieldBase<string, NetString>) Game1.player.farmName);
                break;
              default:
                Game1.multiplayer.globalChatInfoMessage("donation", (string) (NetFieldBase<string, NetString>) Game1.player.name, "object:" + parentSheetIndex.ToString());
                break;
            }
          }
          this.ReturnToDonatableItems();
        }
      }
      else if (this.heldItem == null && !this.inventory.isWithinBounds(x, y))
      {
        Vector2 key = new Vector2((float) ((int) ((double) Utility.ModifyCoordinateFromUIScale((float) x) + (double) Game1.viewport.X) / 64), (float) ((int) ((double) Utility.ModifyCoordinateFromUIScale((float) y) + (double) Game1.viewport.Y) / 64));
        LibraryMuseum currentLocation = Game1.currentLocation as LibraryMuseum;
        if (currentLocation.museumPieces.ContainsKey(key))
        {
          this.heldItem = (Item) new Object(currentLocation.museumPieces[key], 1);
          currentLocation.museumPieces.Remove(key);
          this.holdingMuseumPiece = !currentLocation.museumAlreadyHasArtifact((int) (NetFieldBase<int, NetInt>) this.heldItem.parentSheetIndex);
        }
      }
      if (this.heldItem != null && heldItem == null)
      {
        this.menuMovingDown = true;
        this.reOrganizing = false;
      }
      if (this.okButton == null || !this.okButton.containsPoint(x, y) || !this.readyToClose())
        return;
      if (this.fadeTimer <= 0)
        Game1.playSound("bigDeSelect");
      this.state = 2;
      this.fadeTimer = 800;
      this.fadeIntoBlack = true;
    }

    public virtual void ReturnToDonatableItems()
    {
      this.menuMovingDown = false;
      this.holdingMuseumPiece = false;
      this.reOrganizing = false;
      if (!Game1.options.SnappyMenus)
        return;
      this.movePosition(0, -this.menuPositionOffset);
      this.menuPositionOffset = 0;
      this.snapCursorToCurrentSnappedComponent();
    }

    public override bool readyToClose() => !this.holdingMuseumPiece && this.heldItem == null && !this.menuMovingDown;

    protected override void cleanupBeforeExit()
    {
      if (this.heldItem != null)
      {
        this.heldItem = Game1.player.addItemToInventory(this.heldItem);
        if (this.heldItem != null)
        {
          Game1.createItemDebris(this.heldItem, Game1.player.Position, -1);
          this.heldItem = (Item) null;
        }
      }
      Game1.displayHUD = true;
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      Item heldItem = this.heldItem;
      if (this.fadeTimer <= 0)
        base.receiveRightClick(x, y, true);
      if (this.heldItem == null || heldItem != null)
        return;
      this.menuMovingDown = true;
    }

    public override void update(GameTime time)
    {
      base.update(time);
      if (this.sparkleText != null && this.sparkleText.update(time))
        this.sparkleText = (SparklingText) null;
      if (this.fadeTimer > 0)
      {
        this.fadeTimer -= time.ElapsedGameTime.Milliseconds;
        this.blackFadeAlpha = !this.fadeIntoBlack ? (float) (1.0 - (1500.0 - (double) this.fadeTimer) / 1500.0) : (float) (0.0 + (1500.0 - (double) this.fadeTimer) / 1500.0);
        if (this.fadeTimer <= 0)
        {
          switch (this.state)
          {
            case 0:
              this.state = 1;
              Game1.viewportFreeze = true;
              Game1.viewport.Location = new Location(1152, 128);
              Game1.clampViewportToGameMap();
              this.fadeTimer = 800;
              this.fadeIntoBlack = false;
              break;
            case 2:
              Game1.viewportFreeze = false;
              this.fadeIntoBlack = false;
              this.fadeTimer = 800;
              this.state = 3;
              break;
            case 3:
              this.exitThisMenuNoSound();
              break;
          }
        }
      }
      if (this.menuMovingDown && this.menuPositionOffset < this.height / 3)
      {
        this.menuPositionOffset += 8;
        this.movePosition(0, 8);
      }
      else if (!this.menuMovingDown && this.menuPositionOffset > 0)
      {
        this.menuPositionOffset -= 8;
        this.movePosition(0, -8);
      }
      int num1 = Game1.getOldMouseX(false) + Game1.viewport.X;
      int num2 = Game1.getOldMouseY(false) + Game1.viewport.Y;
      if (!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num1 - Game1.viewport.X < 64 || (double) Game1.input.GetGamePadState().ThumbSticks.Right.X < 0.0)
      {
        Game1.panScreen(-4, 0);
        if ((double) Game1.input.GetGamePadState().ThumbSticks.Right.X < 0.0)
          this.snapCursorToCurrentMuseumSpot();
      }
      else if (!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num1 - (Game1.viewport.X + Game1.viewport.Width) >= -64 || (double) Game1.input.GetGamePadState().ThumbSticks.Right.X > 0.0)
      {
        Game1.panScreen(4, 0);
        if ((double) Game1.input.GetGamePadState().ThumbSticks.Right.X > 0.0)
          this.snapCursorToCurrentMuseumSpot();
      }
      if (!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num2 - Game1.viewport.Y < 64 || (double) Game1.input.GetGamePadState().ThumbSticks.Right.Y > 0.0)
      {
        Game1.panScreen(0, -4);
        if ((double) Game1.input.GetGamePadState().ThumbSticks.Right.Y > 0.0)
          this.snapCursorToCurrentMuseumSpot();
      }
      else if (!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64 || (double) Game1.input.GetGamePadState().ThumbSticks.Right.Y < 0.0)
      {
        Game1.panScreen(0, 4);
        if ((double) Game1.input.GetGamePadState().ThumbSticks.Right.Y < 0.0)
          this.snapCursorToCurrentMuseumSpot();
      }
      foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
        this.receiveKeyPress(pressedKey);
    }

    private void snapCursorToCurrentMuseumSpot()
    {
      if (!this.menuMovingDown)
        return;
      Vector2 vector2 = new Vector2((float) ((Game1.getMouseX(false) + Game1.viewport.X) / 64), (float) ((Game1.getMouseY(false) + Game1.viewport.Y) / 64));
      Game1.setMousePosition((int) vector2.X * 64 - Game1.viewport.X + 32, (int) vector2.Y * 64 - Game1.viewport.Y + 32, false);
    }

    public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.movePosition(0, Game1.viewport.Height - this.yPositionOnScreen - this.height);
      Game1.player.forceCanMove();
    }

    public override void draw(SpriteBatch b)
    {
      if ((this.fadeTimer <= 0 || !this.fadeIntoBlack) && this.state != 3)
      {
        if (this.heldItem != null)
        {
          Game1.StartWorldDrawInUI(b);
          for (int y = Game1.viewport.Y / 64 - 1; y < (Game1.viewport.Y + Game1.viewport.Height) / 64 + 2; ++y)
          {
            for (int x = Game1.viewport.X / 64 - 1; x < (Game1.viewport.X + Game1.viewport.Width) / 64 + 1; ++x)
            {
              if ((Game1.currentLocation as LibraryMuseum).isTileSuitableForMuseumPiece(x, y))
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29)), Color.LightGreen);
            }
          }
          Game1.EndWorldDrawInUI(b);
        }
        if (!this.holdingMuseumPiece)
          this.draw(b, false, false);
        if (!this.hoverText.Equals(""))
          IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
        if (this.heldItem != null)
          this.heldItem.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 8), (float) (Game1.getOldMouseY() + 8)), 1f);
        this.drawMouse(b);
        if (this.sparkleText != null)
          this.sparkleText.draw(b, Utility.ModifyCoordinatesForUIScale(Game1.GlobalToLocal(Game1.viewport, this.globalLocationOfSparklingArtifact)));
      }
      b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * this.blackFadeAlpha);
    }
  }
}
