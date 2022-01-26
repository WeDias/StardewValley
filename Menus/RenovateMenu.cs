// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.RenovateMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewValley.Menus
{
  public class RenovateMenu : IClickableMenu
  {
    public const int region_okButton = 101;
    public const int region_randomButton = 103;
    public const int region_namingBox = 104;
    public static int menuHeight = 320;
    public static int menuWidth = 448;
    public ClickableTextureComponent okButton;
    public ClickableTextureComponent hovered;
    private bool freeze;
    protected HouseRenovation _renovation;
    protected string _oldLocation;
    protected Point _oldPosition;
    protected int _selectedIndex = -1;
    protected int _animatingIndex = -1;
    protected int _buildAnimationTimer;
    protected int _buildAnimationCount;

    public RenovateMenu(HouseRenovation renovation)
      : base(Game1.uiViewport.Width / 2 - RenovateMenu.menuWidth / 2 - IClickableMenu.borderWidth * 2, (Game1.uiViewport.Height - RenovateMenu.menuHeight - IClickableMenu.borderWidth * 2) / 4, RenovateMenu.menuWidth + IClickableMenu.borderWidth * 2, RenovateMenu.menuHeight + IClickableMenu.borderWidth)
    {
      this.height += 64;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 64 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
      textureComponent.myID = 101;
      textureComponent.upNeighborID = 103;
      textureComponent.leftNeighborID = 103;
      this.okButton = textureComponent;
      this._renovation = renovation;
      RenovateMenu.menuHeight = 320;
      RenovateMenu.menuWidth = 448;
      if (Game1.options.SnappyMenus)
      {
        this.populateClickableComponentList();
        this.snapToDefaultClickableComponent();
      }
      this.SetupForRenovationPlacement();
    }

    public override bool shouldClampGamePadCursor() => true;

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public void SetupForReturn()
    {
      this.freeze = true;
      LocationRequest locationRequest = Game1.getLocationRequest(this._oldLocation);
      locationRequest.OnWarp += (LocationRequest.Callback) (() =>
      {
        Console.WriteLine("Display farmer true");
        Game1.player.viewingLocation.Value = (string) null;
        Game1.displayHUD = true;
        Game1.displayFarmer = true;
        this.freeze = false;
        Game1.viewportFreeze = false;
        this.FinalizeReturn();
      });
      Game1.warpFarmer(locationRequest, this._oldPosition.X, this._oldPosition.Y, (int) Game1.player.facingDirection);
    }

    public void FinalizeReturn()
    {
      this.exitThisMenu(false);
      Game1.player.forceCanMove();
      this.freeze = false;
    }

    public void SetupForRenovationPlacement()
    {
      Game1.currentLocation.cleanupBeforePlayerExit();
      Game1.displayFarmer = false;
      this._oldLocation = Game1.currentLocation.NameOrUniqueName;
      this._oldPosition = Game1.player.getTileLocationPoint();
      Game1.currentLocation = this._renovation.location;
      Game1.player.viewingLocation.Value = this._renovation.location.NameOrUniqueName;
      Game1.currentLocation.resetForPlayerEntry();
      Game1.globalFadeToClear();
      this.freeze = false;
      this.okButton.bounds.X = Game1.uiViewport.Width - 128;
      this.okButton.bounds.Y = Game1.uiViewport.Height - 128;
      Game1.displayHUD = false;
      Game1.viewportFreeze = true;
      Vector2 vector2 = new Vector2();
      int num = 0;
      foreach (List<Microsoft.Xna.Framework.Rectangle> renovationBound in this._renovation.renovationBounds)
      {
        foreach (Microsoft.Xna.Framework.Rectangle rectangle in renovationBound)
        {
          vector2.X += (float) rectangle.Center.X;
          vector2.Y += (float) rectangle.Center.Y;
          ++num;
        }
      }
      if (num > 0)
      {
        vector2.X = (float) (int) Math.Round((double) vector2.X / (double) num);
        vector2.Y = (float) (int) Math.Round((double) vector2.Y / (double) num);
      }
      Game1.viewport.Location = new Location((int) (((double) vector2.X + 0.5) * 64.0) - Game1.viewport.Width / 2, (int) (((double) vector2.Y + 0.5) * 64.0) - Game1.viewport.Height / 2);
      Game1.panScreen(0, 0);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (Game1.globalFade || this.freeze)
        return;
      if (this.okButton != null && this.okButton.containsPoint(x, y) && this.readyToClose())
      {
        this.SetupForReturn();
        Game1.playSound("smallSelect");
      }
      else
      {
        Vector2 vector2 = new Vector2((float) (((double) Utility.ModifyCoordinateFromUIScale((float) x) + (double) Game1.viewport.X) / 64.0), (float) (((double) Utility.ModifyCoordinateFromUIScale((float) y) + (double) Game1.viewport.Y) / 64.0));
        for (int index = 0; index < this._renovation.renovationBounds.Count; ++index)
        {
          foreach (Microsoft.Xna.Framework.Rectangle rectangle in this._renovation.renovationBounds[index])
          {
            if (rectangle.Contains((int) vector2.X, (int) vector2.Y))
            {
              this.CompleteRenovation(index);
              return;
            }
          }
        }
      }
    }

    public virtual void AnimateRenovation()
    {
      if (this._buildAnimationTimer == 0)
        return;
      this._buildAnimationTimer -= (int) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
      if (this._buildAnimationTimer > 0)
        return;
      if (this._buildAnimationCount > 0)
      {
        --this._buildAnimationCount;
        if (this._renovation.animationType == HouseRenovation.AnimationType.Destroy)
        {
          this._buildAnimationTimer = 50;
          for (int index = 0; index < 5; ++index)
          {
            Microsoft.Xna.Framework.Rectangle random = Utility.GetRandom<Microsoft.Xna.Framework.Rectangle>(this._renovation.renovationBounds[this._animatingIndex]);
            int x = (int) Utility.RandomFloat((float) ((random.Left - 1) * 64), (float) (64 * random.Right));
            int y = (int) Utility.RandomFloat((float) ((random.Top - 1) * 64), (float) (64 * random.Bottom));
            this._renovation.location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, new Vector2((float) x, (float) y), false, Game1.random.NextDouble() < 0.5));
            this._renovation.location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, new Vector2((float) x, (float) y), false, Game1.random.NextDouble() < 0.5));
            this._renovation.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2((float) x, (float) y), false, 0.0f, Color.White)
            {
              interval = 30f,
              totalNumberOfLoops = 99999,
              animationLength = 4,
              scale = 4f,
              alphaFade = 0.01f
            });
          }
        }
        else
        {
          this._buildAnimationTimer = 500;
          Game1.playSound("axe");
          for (int index = 0; index < 20; ++index)
          {
            Microsoft.Xna.Framework.Rectangle random = Utility.GetRandom<Microsoft.Xna.Framework.Rectangle>(this._renovation.renovationBounds[this._animatingIndex]);
            int x = (int) Utility.RandomFloat((float) ((random.Left - 1) * 64), (float) (64 * random.Right));
            int y = (int) Utility.RandomFloat((float) ((random.Top - 1) * 64), (float) (64 * random.Bottom));
            this._renovation.location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float) (Game1.random.Next(30, 90) - 64), 6, 1, new Vector2((float) x, (float) y), false, Game1.random.NextDouble() < 0.5));
            this._renovation.location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float) (Game1.random.Next(30, 90) - 64), 6, 1, new Vector2((float) x, (float) y), false, Game1.random.NextDouble() < 0.5));
          }
        }
      }
      else
      {
        this._buildAnimationTimer = 0;
        this.SetupForReturn();
      }
    }

    public virtual void CompleteRenovation(int selected_index)
    {
      if (this._renovation.validate != null && !this._renovation.validate(this._renovation, selected_index))
        return;
      this.freeze = true;
      if (this._renovation.animationType == HouseRenovation.AnimationType.Destroy)
      {
        Game1.playSound("explosion");
        this._buildAnimationCount = 10;
      }
      else
        this._buildAnimationCount = 3;
      this._buildAnimationTimer = -1;
      this._animatingIndex = this._selectedIndex;
      if (this._renovation.onRenovation != null)
      {
        this._renovation.onRenovation(this._renovation, selected_index);
        Game1.player.renovateEvent.Fire(this._renovation.location.NameOrUniqueName);
      }
      this.AnimateRenovation();
    }

    public override bool overrideSnappyMenuCursorMovementBan() => true;

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      if (b != Buttons.B || Game1.globalFade)
        return;
      this.SetupForReturn();
      Game1.playSound("smallSelect");
    }

    public override bool readyToClose() => !this.freeze && base.readyToClose();

    public override void receiveKeyPress(Keys key)
    {
      if (Game1.globalFade || this.freeze)
        return;
      if (!Game1.globalFade)
      {
        if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
        {
          this.SetupForReturn();
        }
        else
        {
          if (Game1.options.SnappyMenus || this.freeze)
            return;
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
      }
      else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !Game1.globalFade)
      {
        if (!this.readyToClose())
          return;
        Game1.player.forceCanMove();
        Game1.exitActiveMenu();
        Game1.playSound("bigDeSelect");
      }
      else
      {
        if (!Game1.options.SnappyMenus)
          return;
        base.receiveKeyPress(key);
      }
    }

    public override void update(GameTime time)
    {
      base.update(time);
      this.AnimateRenovation();
      int num1 = Game1.getOldMouseX(false) + Game1.viewport.X;
      int num2 = Game1.getOldMouseY(false) + Game1.viewport.Y;
      if (!this.freeze)
      {
        if (num1 - Game1.viewport.X < 64)
          Game1.panScreen(-8, 0);
        else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= -64)
          Game1.panScreen(8, 0);
        if (num2 - Game1.viewport.Y < 64)
          Game1.panScreen(0, -8);
        else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
          Game1.panScreen(0, 8);
      }
      foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
        this.receiveKeyPress(pressedKey);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      this.hovered = (ClickableTextureComponent) null;
      if (Game1.globalFade || this.freeze)
        return;
      if (this.okButton != null)
      {
        if (this.okButton.containsPoint(x, y))
          this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
        else
          this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
      }
      Vector2 vector2 = new Vector2((float) (((double) Utility.ModifyCoordinateFromUIScale((float) x) + (double) Game1.viewport.X) / 64.0), (float) (((double) Utility.ModifyCoordinateFromUIScale((float) y) + (double) Game1.viewport.Y) / 64.0));
      this._selectedIndex = -1;
      for (int index = 0; index < this._renovation.renovationBounds.Count; ++index)
      {
        foreach (Microsoft.Xna.Framework.Rectangle rectangle in this._renovation.renovationBounds[index])
        {
          if (rectangle.Contains((int) vector2.X, (int) vector2.Y))
          {
            this._selectedIndex = index;
            break;
          }
        }
      }
    }

    public static string getAnimalTitle(string name)
    {
      switch (name)
      {
        case "Chicken":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5922");
        case "Dairy Cow":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5927");
        case "Duck":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5937");
        case "Goat":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5933");
        case "Pig":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5948");
        case "Rabbit":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5945");
        case "Sheep":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5942");
        default:
          return "";
      }
    }

    public static string getAnimalDescription(string name)
    {
      switch (name)
      {
        case "Chicken":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11334") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11335");
        case "Dairy Cow":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11343") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344");
        case "Duck":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11337") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11335");
        case "Goat":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11349") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344");
        case "Pig":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11346") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344");
        case "Rabbit":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11340") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11335");
        case "Sheep":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11352") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344");
        default:
          return "";
      }
    }

    public override void draw(SpriteBatch b)
    {
      if (!Game1.globalFade && !this.freeze)
      {
        Game1.StartWorldDrawInUI(b);
        for (int index = 0; index < this._renovation.renovationBounds.Count; ++index)
        {
          foreach (Microsoft.Xna.Framework.Rectangle rectangle in this._renovation.renovationBounds[index])
          {
            for (int left = rectangle.Left; left < rectangle.Right; ++left)
            {
              for (int top = rectangle.Top; top < rectangle.Bottom; ++top)
              {
                int num = 0;
                if (index == this._selectedIndex)
                  num = 1;
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) left, (float) top) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194 + num * 16, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
              }
            }
          }
        }
        Game1.EndWorldDrawInUI(b);
      }
      if (!Game1.globalFade && !this.freeze)
      {
        string placementText = this._renovation.placementText;
        SpriteText.drawStringWithScrollBackground(b, placementText, Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(placementText) / 2, 16);
      }
      if (!Game1.globalFade && !this.freeze && this.okButton != null)
        this.okButton.draw(b);
      Game1.mouseCursorTransparency = 1f;
      this.drawMouse(b);
    }
  }
}
