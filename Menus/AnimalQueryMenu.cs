// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.AnimalQueryMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;

namespace StardewValley.Menus
{
  public class AnimalQueryMenu : IClickableMenu
  {
    public const int region_okButton = 101;
    public const int region_love = 102;
    public const int region_sellButton = 103;
    public const int region_moveHomeButton = 104;
    public const int region_noButton = 105;
    public const int region_allowReproductionButton = 106;
    public const int region_fullnessHover = 107;
    public const int region_happinessHover = 108;
    public const int region_loveHover = 109;
    public const int region_textBoxCC = 110;
    public new static int width = 384;
    public new static int height = 512;
    private FarmAnimal animal;
    private TextBox textBox;
    private TextBoxEvent e;
    public ClickableTextureComponent okButton;
    public ClickableTextureComponent love;
    public ClickableTextureComponent sellButton;
    public ClickableTextureComponent moveHomeButton;
    public ClickableTextureComponent yesButton;
    public ClickableTextureComponent noButton;
    public ClickableTextureComponent allowReproductionButton;
    public ClickableComponent fullnessHover;
    public ClickableComponent happinessHover;
    public ClickableComponent loveHover;
    public ClickableComponent textBoxCC;
    private double fullnessLevel;
    private double happinessLevel;
    private double loveLevel;
    private bool confirmingSell;
    private bool movingAnimal;
    private string hoverText = "";
    private string parentName;

    public AnimalQueryMenu(FarmAnimal animal)
      : base(Game1.uiViewport.Width / 2 - AnimalQueryMenu.width / 2, Game1.uiViewport.Height / 2 - AnimalQueryMenu.height / 2, AnimalQueryMenu.width, AnimalQueryMenu.height)
    {
      Game1.player.Halt();
      Game1.player.faceGeneralDirection(animal.Position, 0, false, false);
      AnimalQueryMenu.width = 384;
      AnimalQueryMenu.height = 512;
      this.animal = animal;
      this.textBox = new TextBox((Texture2D) null, (Texture2D) null, Game1.dialogueFont, Game1.textColor);
      this.textBox.X = Game1.uiViewport.Width / 2 - 128 - 12;
      this.textBox.Y = this.yPositionOnScreen - 4 + 128;
      this.textBox.Width = 256;
      this.textBox.Height = 192;
      this.textBoxCC = new ClickableComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X, this.textBox.Y, this.textBox.Width, 64), "")
      {
        myID = 110,
        downNeighborID = 104
      };
      this.textBox.Text = animal.displayName;
      Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) this.textBox;
      this.textBox.Selected = false;
      if ((long) animal.parentId != -1L)
      {
        FarmAnimal animal1 = Utility.getAnimal((long) animal.parentId);
        if (animal1 != null)
          this.parentName = animal1.displayName;
      }
      if (animal.sound.Value != null && Game1.soundBank != null && !Game1.options.muteAnimalSounds)
      {
        ICue cue = Game1.soundBank.GetCue(animal.sound.Value);
        cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
        cue.Play();
      }
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + AnimalQueryMenu.width + 4, this.yPositionOnScreen + AnimalQueryMenu.height - 64 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent1.myID = 101;
      textureComponent1.upNeighborID = -99998;
      this.okButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + AnimalQueryMenu.width + 4, this.yPositionOnScreen + AnimalQueryMenu.height - 192 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(0, 384, 16, 16), 4f);
      textureComponent2.myID = 103;
      textureComponent2.downNeighborID = -99998;
      textureComponent2.upNeighborID = 104;
      this.sellButton = textureComponent2;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + AnimalQueryMenu.width + 4, this.yPositionOnScreen + AnimalQueryMenu.height - 256 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(16, 384, 16, 16), 4f);
      textureComponent3.myID = 104;
      textureComponent3.downNeighborID = 103;
      textureComponent3.upNeighborID = 110;
      this.moveHomeButton = textureComponent3;
      if (!animal.isBaby() && !animal.isCoopDweller() && animal.CanHavePregnancy())
      {
        ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + AnimalQueryMenu.width + 16, this.yPositionOnScreen + AnimalQueryMenu.height - 128 - IClickableMenu.borderWidth + 8, 36, 36), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle((bool) (NetFieldBase<bool, NetBool>) animal.allowReproduction ? 128 : 137, 393, 9, 9), 4f);
        textureComponent4.myID = 106;
        textureComponent4.downNeighborID = 101;
        textureComponent4.upNeighborID = 103;
        this.allowReproductionButton = textureComponent4;
      }
      ClickableTextureComponent textureComponent5 = new ClickableTextureComponent((Math.Round((double) (int) (NetFieldBase<int, NetInt>) animal.friendshipTowardFarmer, 0) / 10.0).ToString() + "<", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32 + 16, this.yPositionOnScreen - 32 + IClickableMenu.spaceToClearTopBorder + 256 - 32, AnimalQueryMenu.width - 128, 64), (string) null, "Friendship", Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(172, 512, 16, 16), 4f);
      textureComponent5.myID = 102;
      this.love = textureComponent5;
      this.loveHover = new ClickableComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 192 - 32, AnimalQueryMenu.width, 64), "Friendship")
      {
        myID = 109
      };
      this.fullnessLevel = (double) (byte) (NetFieldBase<byte, NetByte>) animal.fullness / (double) byte.MaxValue;
      if (animal.home != null && animal.home.indoors.Value != null)
      {
        int num = animal.home.indoors.Value.numberOfObjectsWithName("Hay");
        if (num > 0)
        {
          int count = (animal.home.indoors.Value as AnimalHouse).animalsThatLiveHere.Count;
          this.fullnessLevel = Math.Min(1.0, this.fullnessLevel + (double) num / (double) count);
        }
      }
      else
        Utility.fixAllAnimals();
      this.happinessLevel = (double) (byte) (NetFieldBase<byte, NetByte>) animal.happiness / (double) byte.MaxValue;
      this.loveLevel = (double) (int) (NetFieldBase<int, NetInt>) animal.friendshipTowardFarmer / 1000.0;
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public override bool shouldClampGamePadCursor() => this.movingAnimal;

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(101);
      this.snapCursorToCurrentSnappedComponent();
    }

    public void textBoxEnter(TextBox sender)
    {
    }

    public override void receiveKeyPress(Keys key)
    {
      if (Game1.globalFade)
        return;
      if (((IEnumerable<InputButton>) Game1.options.menuButton).Contains<InputButton>(new InputButton(key)) && (this.textBox == null || !this.textBox.Selected))
      {
        Game1.playSound("smallSelect");
        if (this.readyToClose())
        {
          Game1.exitActiveMenu();
          if (this.textBox.Text.Length <= 0 || Utility.areThereAnyOtherAnimalsWithThisName(this.textBox.Text))
            return;
          this.animal.displayName = this.textBox.Text;
          this.animal.Name = this.textBox.Text;
        }
        else
        {
          if (!this.movingAnimal)
            return;
          Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.prepareForReturnFromPlacement));
        }
      }
      else
      {
        if (!Game1.options.SnappyMenus || ((IEnumerable<InputButton>) Game1.options.menuButton).Contains<InputButton>(new InputButton(key)) && this.textBox != null && this.textBox.Selected)
          return;
        base.receiveKeyPress(key);
      }
    }

    public override void update(GameTime time)
    {
      base.update(time);
      if (!this.movingAnimal)
        return;
      int num1 = Game1.getOldMouseX(false) + Game1.viewport.X;
      int num2 = Game1.getOldMouseY(false) + Game1.viewport.Y;
      if (num1 - Game1.viewport.X < 64)
        Game1.panScreen(-8, 0);
      else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= -64)
        Game1.panScreen(8, 0);
      if (num2 - Game1.viewport.Y < 64)
        Game1.panScreen(0, -8);
      else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
        Game1.panScreen(0, 8);
      foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
        this.receiveKeyPress(pressedKey);
    }

    public void finishedPlacingAnimal()
    {
      Game1.exitActiveMenu();
      Game1.currentLocation = Game1.player.currentLocation;
      Game1.currentLocation.resetForPlayerEntry();
      Game1.globalFadeToClear();
      Game1.displayHUD = true;
      Game1.viewportFreeze = false;
      Game1.displayFarmer = true;
      Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_HomeChanged"), Color.LimeGreen, 3500f));
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (Game1.globalFade)
        return;
      if (this.movingAnimal)
      {
        if (this.okButton != null && this.okButton.containsPoint(x, y))
        {
          Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.prepareForReturnFromPlacement));
          Game1.playSound("smallSelect");
        }
        Vector2 tile = new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX(false)) / 64), (float) ((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64));
        Building buildingAt = (Game1.getLocationFromName("Farm") as Farm).getBuildingAt(tile);
        if (buildingAt == null)
          return;
        if (buildingAt.buildingType.Value.Contains(this.animal.buildingTypeILiveIn.Value))
        {
          if ((buildingAt.indoors.Value as AnimalHouse).isFull())
            Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_BuildingFull"));
          else if (buildingAt.Equals((object) this.animal.home))
          {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_AlreadyHome"));
          }
          else
          {
            (this.animal.home.indoors.Value as AnimalHouse).animalsThatLiveHere.Remove((long) this.animal.myID);
            if ((this.animal.home.indoors.Value as AnimalHouse).animals.ContainsKey((long) this.animal.myID))
            {
              (buildingAt.indoors.Value as AnimalHouse).animals.Add((long) this.animal.myID, this.animal);
              (this.animal.home.indoors.Value as AnimalHouse).animals.Remove((long) this.animal.myID);
            }
            this.animal.home = buildingAt;
            this.animal.homeLocation.Value = new Vector2((float) (int) (NetFieldBase<int, NetInt>) buildingAt.tileX, (float) (int) (NetFieldBase<int, NetInt>) buildingAt.tileY);
            (buildingAt.indoors.Value as AnimalHouse).animalsThatLiveHere.Add((long) this.animal.myID);
            this.animal.makeSound();
            Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.finishedPlacingAnimal));
          }
        }
        else
          Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_CantLiveThere", (object) this.animal.shortDisplayType()));
      }
      else if (this.confirmingSell)
      {
        if (this.yesButton.containsPoint(x, y))
        {
          Game1.player.Money += this.animal.getSellPrice();
          (this.animal.home.indoors.Value as AnimalHouse).animalsThatLiveHere.Remove((long) this.animal.myID);
          this.animal.health.Value = -1;
          if (this.animal.foundGrass != null && FarmAnimal.reservedGrass.Contains(this.animal.foundGrass))
            FarmAnimal.reservedGrass.Remove(this.animal.foundGrass);
          int num1 = this.animal.frontBackSourceRect.Width / 2;
          for (int index = 0; index < num1; ++index)
          {
            int num2 = Game1.random.Next(25, 200);
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, this.animal.Position + new Vector2((float) Game1.random.Next(-32, this.animal.frontBackSourceRect.Width * 3), (float) Game1.random.Next(-32, this.animal.frontBackSourceRect.Height * 3)), new Color((int) byte.MaxValue - num2, (int) byte.MaxValue, (int) byte.MaxValue - num2), animationInterval: (Game1.random.NextDouble() < 0.5 ? 50f : (float) Game1.random.Next(30, 200)), sourceRectWidth: 64, sourceRectHeight: 64, delay: (Game1.random.NextDouble() < 0.5 ? 0 : Game1.random.Next(0, 600)))
            {
              scale = (float) Game1.random.Next(2, 5) * 0.25f,
              alpha = (float) Game1.random.Next(2, 5) * 0.25f,
              motion = new Vector2(0.0f, (float) -Game1.random.NextDouble())
            });
          }
          Game1.playSound("newRecipe");
          Game1.playSound("money");
          Game1.exitActiveMenu();
        }
        else
        {
          if (!this.noButton.containsPoint(x, y))
            return;
          this.confirmingSell = false;
          Game1.playSound("smallSelect");
          if (!Game1.options.SnappyMenus)
            return;
          this.currentlySnappedComponent = this.getComponentWithID(103);
          this.snapCursorToCurrentSnappedComponent();
        }
      }
      else
      {
        if (this.okButton != null && this.okButton.containsPoint(x, y) && this.readyToClose())
        {
          Game1.exitActiveMenu();
          if (this.textBox.Text.Length > 0 && !Utility.areThereAnyOtherAnimalsWithThisName(this.textBox.Text))
          {
            this.animal.displayName = this.textBox.Text;
            this.animal.Name = this.textBox.Text;
          }
          Game1.playSound("smallSelect");
        }
        if (this.sellButton.containsPoint(x, y))
        {
          this.confirmingSell = true;
          ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(Game1.uiViewport.Width / 2 - 64 - 4, Game1.uiViewport.Height / 2 - 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
          textureComponent1.myID = 111;
          textureComponent1.rightNeighborID = 105;
          this.yesButton = textureComponent1;
          ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(Game1.uiViewport.Width / 2 + 4, Game1.uiViewport.Height / 2 - 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
          textureComponent2.myID = 105;
          textureComponent2.leftNeighborID = 111;
          this.noButton = textureComponent2;
          Game1.playSound("smallSelect");
          if (!Game1.options.SnappyMenus)
            return;
          this.populateClickableComponentList();
          this.currentlySnappedComponent = (ClickableComponent) this.noButton;
          this.snapCursorToCurrentSnappedComponent();
        }
        else
        {
          if (this.moveHomeButton.containsPoint(x, y))
          {
            Game1.playSound("smallSelect");
            Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.prepareForAnimalPlacement));
          }
          if (this.allowReproductionButton != null && this.allowReproductionButton.containsPoint(x, y))
          {
            Game1.playSound("drumkit6");
            this.animal.allowReproduction.Value = !(bool) (NetFieldBase<bool, NetBool>) this.animal.allowReproduction;
            this.allowReproductionButton.sourceRect.X = !(bool) (NetFieldBase<bool, NetBool>) this.animal.allowReproduction ? 137 : 128;
          }
          this.textBox.Update();
        }
      }
    }

    public override bool overrideSnappyMenuCursorMovementBan() => this.movingAnimal;

    public void prepareForAnimalPlacement()
    {
      this.movingAnimal = true;
      Game1.currentLocation = Game1.getLocationFromName("Farm");
      Game1.globalFadeToClear();
      this.okButton.bounds.X = Game1.uiViewport.Width - 128;
      this.okButton.bounds.Y = Game1.uiViewport.Height - 128;
      Game1.displayHUD = false;
      Game1.viewportFreeze = true;
      Game1.viewport.Location = new Location(3136, 320);
      Game1.panScreen(0, 0);
      Game1.currentLocation.resetForPlayerEntry();
      Game1.displayFarmer = false;
    }

    public void prepareForReturnFromPlacement()
    {
      Game1.currentLocation = Game1.player.currentLocation;
      Game1.currentLocation.resetForPlayerEntry();
      Game1.globalFadeToClear();
      this.okButton.bounds.X = this.xPositionOnScreen + AnimalQueryMenu.width + 4;
      this.okButton.bounds.Y = this.yPositionOnScreen + AnimalQueryMenu.height - 64 - IClickableMenu.borderWidth;
      Game1.displayHUD = true;
      Game1.viewportFreeze = false;
      Game1.displayFarmer = true;
      this.movingAnimal = false;
    }

    public override bool readyToClose()
    {
      this.textBox.Selected = false;
      return base.readyToClose() && !this.movingAnimal && !Game1.globalFade;
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      if (Game1.globalFade)
        return;
      if (this.readyToClose())
      {
        Game1.exitActiveMenu();
        if (this.textBox.Text.Length > 0 && !Utility.areThereAnyOtherAnimalsWithThisName(this.textBox.Text))
        {
          this.animal.displayName = this.textBox.Text;
          this.animal.Name = this.textBox.Text;
        }
        Game1.playSound("smallSelect");
      }
      else
      {
        if (!this.movingAnimal)
          return;
        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.prepareForReturnFromPlacement));
      }
    }

    public override void performHoverAction(int x, int y)
    {
      this.hoverText = "";
      if (this.movingAnimal)
      {
        Vector2 tile = new Vector2((float) ((Game1.viewport.X + Game1.getOldMouseX(false)) / 64), (float) ((Game1.viewport.Y + Game1.getOldMouseY(false)) / 64));
        Farm locationFromName = Game1.getLocationFromName("Farm") as Farm;
        foreach (Building building in locationFromName.buildings)
          building.color.Value = Color.White;
        Building buildingAt = locationFromName.getBuildingAt(tile);
        if (buildingAt != null)
        {
          if (buildingAt.buildingType.Value.Contains(this.animal.buildingTypeILiveIn.Value) && !(buildingAt.indoors.Value as AnimalHouse).isFull() && !buildingAt.Equals((object) this.animal.home))
            buildingAt.color.Value = Color.LightGreen * 0.8f;
          else
            buildingAt.color.Value = Color.Red * 0.8f;
        }
      }
      if (this.okButton != null)
      {
        if (this.okButton.containsPoint(x, y))
          this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
        else
          this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
      }
      if (this.sellButton != null)
      {
        if (this.sellButton.containsPoint(x, y))
        {
          this.sellButton.scale = Math.Min(4.1f, this.sellButton.scale + 0.05f);
          this.hoverText = Game1.content.LoadString("Strings\\UI:AnimalQuery_Sell", (object) this.animal.getSellPrice());
        }
        else
          this.sellButton.scale = Math.Max(4f, this.sellButton.scale - 0.05f);
      }
      if (this.moveHomeButton != null)
      {
        if (this.moveHomeButton.containsPoint(x, y))
        {
          this.moveHomeButton.scale = Math.Min(4.1f, this.moveHomeButton.scale + 0.05f);
          this.hoverText = Game1.content.LoadString("Strings\\UI:AnimalQuery_Move");
        }
        else
          this.moveHomeButton.scale = Math.Max(4f, this.moveHomeButton.scale - 0.05f);
      }
      if (this.allowReproductionButton != null)
      {
        if (this.allowReproductionButton.containsPoint(x, y))
        {
          this.allowReproductionButton.scale = Math.Min(4.1f, this.allowReproductionButton.scale + 0.05f);
          this.hoverText = Game1.content.LoadString("Strings\\UI:AnimalQuery_AllowReproduction");
        }
        else
          this.allowReproductionButton.scale = Math.Max(4f, this.allowReproductionButton.scale - 0.05f);
      }
      if (this.yesButton != null)
      {
        if (this.yesButton.containsPoint(x, y))
          this.yesButton.scale = Math.Min(1.1f, this.yesButton.scale + 0.05f);
        else
          this.yesButton.scale = Math.Max(1f, this.yesButton.scale - 0.05f);
      }
      if (this.noButton == null)
        return;
      if (this.noButton.containsPoint(x, y))
        this.noButton.scale = Math.Min(1.1f, this.noButton.scale + 0.05f);
      else
        this.noButton.scale = Math.Max(1f, this.noButton.scale - 0.05f);
    }

    public override void draw(SpriteBatch b)
    {
      if (!this.movingAnimal && !Game1.globalFade)
      {
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
        Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen + 128, AnimalQueryMenu.width, AnimalQueryMenu.height - 128, false, true);
        if ((byte) (NetFieldBase<byte, NetByte>) this.animal.harvestType != (byte) 2)
          this.textBox.Draw(b);
        int sub1 = (this.animal.GetDaysOwned() + 1) / 28 + 1;
        string text1 = sub1 <= 1 ? Game1.content.LoadString("Strings\\UI:AnimalQuery_Age1") : Game1.content.LoadString("Strings\\UI:AnimalQuery_AgeN", (object) sub1);
        if ((int) (NetFieldBase<int, NetInt>) this.animal.age < (int) (byte) (NetFieldBase<byte, NetByte>) this.animal.ageWhenMature)
          text1 += Game1.content.LoadString("Strings\\UI:AnimalQuery_AgeBaby");
        Utility.drawTextWithShadow(b, text1, Game1.smallFont, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128)), Game1.textColor);
        int num1 = 0;
        if (this.parentName != null)
        {
          num1 = 21;
          Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AnimalQuery_Parent", (object) this.parentName), Game1.smallFont, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32), (float) (32 + this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128)), Game1.textColor);
        }
        int num2 = this.loveLevel * 1000.0 % 200.0 >= 100.0 ? (int) (this.loveLevel * 1000.0 / 200.0) : -100;
        for (int index = 0; index < 5; ++index)
        {
          b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 96 + 32 * index), (float) (num1 + this.yPositionOnScreen - 32 + 320)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(211 + (this.loveLevel * 1000.0 <= (double) ((index + 1) * 195) ? 7 : 0), 428, 7, 6)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
          if (num2 == index)
            b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 96 + 32 * index), (float) (num1 + this.yPositionOnScreen - 32 + 320)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(211, 428, 4, 6)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.891f);
        }
        Utility.drawTextWithShadow(b, Game1.parseText(this.animal.getMoodMessage(), Game1.smallFont, AnimalQueryMenu.width - IClickableMenu.spaceToClearSideBorder * 2 - 64), Game1.smallFont, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32), (float) (num1 + this.yPositionOnScreen + 384 - 64 + 4)), Game1.textColor);
        this.okButton.draw(b);
        this.sellButton.draw(b);
        this.moveHomeButton.draw(b);
        if (this.allowReproductionButton != null)
          this.allowReproductionButton.draw(b);
        if (this.confirmingSell)
        {
          b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
          Game1.drawDialogueBox(Game1.uiViewport.Width / 2 - 160, Game1.uiViewport.Height / 2 - 192, 320, 256, false, true);
          string text2 = Game1.content.LoadString("Strings\\UI:AnimalQuery_ConfirmSell");
          b.DrawString(Game1.dialogueFont, text2, new Vector2((float) (Game1.uiViewport.Width / 2) - Game1.dialogueFont.MeasureString(text2).X / 2f, (float) (Game1.uiViewport.Height / 2 - 96 + 8)), Game1.textColor);
          this.yesButton.draw(b);
          this.noButton.draw(b);
        }
        else if (this.hoverText != null && this.hoverText.Length > 0)
          IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      }
      else if (!Game1.globalFade)
      {
        string text = Game1.content.LoadString("Strings\\UI:AnimalQuery_ChooseBuilding", (object) this.animal.displayHouse, (object) this.animal.displayType);
        Game1.drawDialogueBox(32, -64, (int) Game1.dialogueFont.MeasureString(text).X + IClickableMenu.borderWidth * 2 + 16, 128 + IClickableMenu.borderWidth * 2, false, true);
        b.DrawString(Game1.dialogueFont, text, new Vector2((float) (32 + IClickableMenu.spaceToClearSideBorder * 2 + 8), 44f), Game1.textColor);
        this.okButton.draw(b);
      }
      this.drawMouse(b);
    }
  }
}
