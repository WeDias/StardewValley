// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ShippingMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class ShippingMenu : IClickableMenu
  {
    public const int region_okbutton = 101;
    public const int region_forwardButton = 102;
    public const int region_backButton = 103;
    public const int farming_category = 0;
    public const int foraging_category = 1;
    public const int fishing_category = 2;
    public const int mining_category = 3;
    public const int other_category = 4;
    public const int total_category = 5;
    public const int timePerIntroCategory = 500;
    public const int outroFadeTime = 800;
    public const int smokeRate = 100;
    public const int categorylabelHeight = 25;
    public int itemsPerCategoryPage = 9;
    public int currentPage = -1;
    public int currentTab;
    public List<ClickableTextureComponent> categories = new List<ClickableTextureComponent>();
    public ClickableTextureComponent okButton;
    public ClickableTextureComponent forwardButton;
    public ClickableTextureComponent backButton;
    private List<int> categoryTotals = new List<int>();
    private List<MoneyDial> categoryDials = new List<MoneyDial>();
    private Dictionary<Item, int> itemValues = new Dictionary<Item, int>();
    private Dictionary<Item, int> singleItemValues = new Dictionary<Item, int>();
    private List<List<Item>> categoryItems = new List<List<Item>>();
    private int categoryLabelsWidth;
    private int plusButtonWidth;
    private int itemSlotWidth;
    private int itemAndPlusButtonWidth;
    private int totalWidth;
    private int centerX;
    private int centerY;
    private int introTimer = 3500;
    private int outroFadeTimer;
    private int outroPauseBeforeDateChange;
    private int finalOutroTimer;
    private int smokeTimer;
    private int dayPlaqueY;
    private int moonShake = -1;
    private int timesPokedMoon;
    private float weatherX;
    private bool outro;
    private bool newDayPlaque;
    private bool savedYet;
    public List<TemporaryAnimatedSprite> animations = new List<TemporaryAnimatedSprite>();
    private SaveGameMenu saveGameMenu;
    protected bool _hasFinished;
    public bool _activated;

    public ShippingMenu(IList<Item> items)
      : base(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height)
    {
      this._activated = false;
      this.parseItems(items);
      if (!Game1.wasRainingYesterday)
        Game1.changeMusicTrack(Game1.currentSeason.Equals("summer") ? "nightTime" : "none");
      this.categoryLabelsWidth = 512;
      this.plusButtonWidth = 40;
      this.itemSlotWidth = 96;
      this.itemAndPlusButtonWidth = this.plusButtonWidth + this.itemSlotWidth + 8;
      this.totalWidth = this.categoryLabelsWidth + this.itemAndPlusButtonWidth;
      this.centerX = Game1.uiViewport.Width / 2;
      this.centerY = Game1.uiViewport.Height / 2;
      this._hasFinished = false;
      int num = -1;
      for (int index = 0; index < 6; ++index)
      {
        List<ClickableTextureComponent> categories = this.categories;
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(this.centerX + this.totalWidth / 2 - this.plusButtonWidth, this.centerY - 300 + index * 27 * 4, this.plusButtonWidth, 44), "", this.getCategoryName(index), Game1.mouseCursors, new Rectangle(392, 361, 10, 11), 4f);
        textureComponent.visible = index < 5 && this.categoryItems[index].Count > 0;
        textureComponent.myID = index;
        textureComponent.downNeighborID = index < 4 ? index + 1 : 101;
        textureComponent.upNeighborID = index > 0 ? num : -1;
        textureComponent.upNeighborImmutable = true;
        categories.Add(textureComponent);
        num = index >= 5 || this.categoryItems[index].Count <= 0 ? num : index;
      }
      this.dayPlaqueY = this.categories[0].bounds.Y - 128;
      Rectangle bounds = new Rectangle(this.centerX + this.totalWidth / 2 - this.itemAndPlusButtonWidth + 32, this.centerY + 300 - 64, 64, 64);
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11382"), bounds, (string) null, Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11382"), Game1.mouseCursors, new Rectangle(128, 256, 64, 64), 1f);
      textureComponent1.myID = 101;
      textureComponent1.upNeighborID = num;
      this.okButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + 32, this.yPositionOnScreen + this.height - 64, 48, 44), (string) null, "", Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent2.myID = 103;
      textureComponent2.rightNeighborID = -7777;
      this.backButton = textureComponent2;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width - 32 - 48, this.yPositionOnScreen + this.height - 64, 48, 44), (string) null, "", Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent3.myID = 102;
      textureComponent3.leftNeighborID = 103;
      this.forwardButton = textureComponent3;
      if (Game1.dayOfMonth == 25 && Game1.currentSeason.Equals("winter"))
        this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(640, 800, 32, 16), 80f, 2, 1000, new Vector2((float) Game1.uiViewport.Width, (float) Game1.random.Next(0, 200)), false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
        {
          motion = new Vector2(-4f, 0.0f),
          delayBeforeAnimationStart = 3000
        });
      Game1.stats.checkForShippingAchievements();
      if (!Game1.player.achievements.Contains(34) && Utility.hasFarmerShippedAllItems())
        Game1.getAchievement(34);
      this.RepositionItems();
      this.populateClickableComponentList();
      if (!Game1.options.SnappyMenus)
        return;
      this.snapToDefaultClickableComponent();
    }

    public void RepositionItems()
    {
      this.centerX = Game1.uiViewport.Width / 2;
      this.centerY = Game1.uiViewport.Height / 2;
      for (int index = 0; index < 6; ++index)
        this.categories[index].bounds = new Rectangle(this.centerX + this.totalWidth / 2 - this.plusButtonWidth, this.centerY - 300 + index * 27 * 4, this.plusButtonWidth, 44);
      this.dayPlaqueY = this.categories[0].bounds.Y - 128;
      if (this.dayPlaqueY < 0)
        this.dayPlaqueY = -64;
      this.backButton.bounds.X = this.xPositionOnScreen + 32;
      this.backButton.bounds.Y = this.yPositionOnScreen + this.height - 64;
      this.forwardButton.bounds.X = this.xPositionOnScreen + this.width - 32 - 48;
      this.forwardButton.bounds.Y = this.yPositionOnScreen + this.height - 64;
      this.okButton.bounds = new Rectangle(this.centerX + this.totalWidth / 2 - this.itemAndPlusButtonWidth + 32, this.centerY + 300 - 64, 64, 64);
      this.itemsPerCategoryPage = (int) ((double) (this.yPositionOnScreen + this.height - 64 - (this.yPositionOnScreen + 32)) / 68.0);
      if (this.currentPage < 0)
        return;
      this.currentTab = Utility.Clamp(this.currentTab, 0, (this.categoryItems[this.currentPage].Count - 1) / this.itemsPerCategoryPage);
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
      if (oldID != 103 || direction != 1 || !this.showForwardButton())
        return;
      this.currentlySnappedComponent = this.getComponentWithID(102);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
      if (this.currentPage != -1)
        this.currentlySnappedComponent = this.getComponentWithID(103);
      else
        this.currentlySnappedComponent = this.getComponentWithID(101);
      this.snapCursorToCurrentSnappedComponent();
    }

    public void parseItems(IList<Item> items)
    {
      Utility.consolidateStacks(items);
      for (int index = 0; index < 6; ++index)
      {
        this.categoryItems.Add(new List<Item>());
        this.categoryTotals.Add(0);
        this.categoryDials.Add(new MoneyDial(7, index == 5));
      }
      foreach (Item key in (IEnumerable<Item>) items)
      {
        if (key is StardewValley.Object)
        {
          StardewValley.Object o = key as StardewValley.Object;
          int categoryIndexForObject = this.getCategoryIndexForObject(o);
          this.categoryItems[categoryIndexForObject].Add((Item) o);
          int storePrice = o.sellToStorePrice();
          int num = storePrice * o.Stack;
          this.categoryTotals[categoryIndexForObject] += num;
          this.itemValues[key] = num;
          this.singleItemValues[key] = storePrice;
          Game1.stats.itemsShipped += (uint) o.Stack;
          if (o.Category == -75 || o.Category == -79)
            Game1.stats.CropsShipped += (uint) o.Stack;
          if (o.countsForShippedCollection())
            Game1.player.shippedBasic((int) (NetFieldBase<int, NetInt>) o.parentSheetIndex, (int) (NetFieldBase<int, NetInt>) o.stack);
        }
      }
      for (int index = 0; index < 5; ++index)
      {
        this.categoryTotals[5] += this.categoryTotals[index];
        this.categoryItems[5].AddRange((IEnumerable<Item>) this.categoryItems[index]);
        this.categoryDials[index].currentValue = this.categoryTotals[index];
        this.categoryDials[index].previousTargetValue = this.categoryDials[index].currentValue;
      }
      this.categoryDials[5].currentValue = this.categoryTotals[5];
      Game1.setRichPresence("earnings", (object) this.categoryTotals[5]);
    }

    public int getCategoryIndexForObject(StardewValley.Object o)
    {
      switch ((int) (NetFieldBase<int, NetInt>) o.parentSheetIndex)
      {
        case 296:
        case 396:
        case 402:
        case 406:
        case 410:
        case 414:
        case 418:
          return 1;
        default:
          switch (o.Category)
          {
            case -81:
            case -27:
            case -23:
              return 1;
            case -80:
            case -79:
            case -75:
            case -26:
            case -14:
            case -6:
            case -5:
              return 0;
            case -20:
            case -4:
              return 2;
            case -15:
            case -12:
            case -2:
              return 3;
            default:
              return 4;
          }
      }
    }

    public string getCategoryName(int index)
    {
      switch (index)
      {
        case 0:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11389");
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11390");
        case 2:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11391");
        case 3:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11392");
        case 4:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11393");
        case 5:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11394");
        default:
          return "";
      }
    }

    public override void update(GameTime time)
    {
      base.update(time);
      if (!this._activated)
      {
        this._activated = true;
        Game1.player.team.endOfNightStatus.UpdateState("shipment");
      }
      if (this._hasFinished)
      {
        if (!Game1.PollForEndOfNewDaySync())
          return;
        this.exitThisMenu(false);
      }
      else
      {
        if (this.saveGameMenu != null)
        {
          this.saveGameMenu.update(time);
          if (this.saveGameMenu.quit)
          {
            this.saveGameMenu = (SaveGameMenu) null;
            this.savedYet = true;
          }
        }
        this.weatherX += (float) time.ElapsedGameTime.Milliseconds * 0.03f;
        for (int index = this.animations.Count - 1; index >= 0; --index)
        {
          if (this.animations[index].update(time))
            this.animations.RemoveAt(index);
        }
        if (this.outro)
        {
          if (this.outroFadeTimer > 0)
            this.outroFadeTimer -= time.ElapsedGameTime.Milliseconds;
          else if (this.outroFadeTimer <= 0 && this.dayPlaqueY < this.centerY - 64)
          {
            if (this.animations.Count > 0)
              this.animations.Clear();
            this.dayPlaqueY += (int) Math.Ceiling((double) time.ElapsedGameTime.Milliseconds * 0.349999994039536);
            if (this.dayPlaqueY >= this.centerY - 64)
              this.outroPauseBeforeDateChange = 700;
          }
          else if (this.outroPauseBeforeDateChange > 0)
          {
            this.outroPauseBeforeDateChange -= time.ElapsedGameTime.Milliseconds;
            if (this.outroPauseBeforeDateChange <= 0)
            {
              this.newDayPlaque = true;
              Game1.playSound("newRecipe");
              if (!Game1.currentSeason.Equals("winter") && Game1.game1.IsMainInstance)
                DelayedAction.playSoundAfterDelay(Game1.IsRainingHere() ? "rainsound" : "rooster", 1500);
              this.finalOutroTimer = 2000;
              this.animations.Clear();
              if (!this.savedYet)
              {
                if (this.saveGameMenu != null)
                  return;
                this.saveGameMenu = new SaveGameMenu();
                return;
              }
            }
          }
          else if (this.finalOutroTimer > 0 && this.savedYet)
          {
            this.finalOutroTimer -= time.ElapsedGameTime.Milliseconds;
            if (this.finalOutroTimer <= 0)
              this._hasFinished = true;
          }
        }
        TimeSpan elapsedGameTime;
        if (this.introTimer >= 0)
        {
          int introTimer1 = this.introTimer;
          int introTimer2 = this.introTimer;
          elapsedGameTime = time.ElapsedGameTime;
          int num1 = elapsedGameTime.Milliseconds * (Game1.oldMouseState.LeftButton == ButtonState.Pressed ? 3 : 1);
          this.introTimer = introTimer2 - num1;
          if (introTimer1 % 500 < this.introTimer % 500 && this.introTimer <= 3000)
          {
            int num2 = 4 - this.introTimer / 500;
            if (num2 < 6 && num2 > -1)
            {
              if (this.categoryItems[num2].Count > 0)
              {
                Game1.playSound(this.getCategorySound(num2));
                this.categoryDials[num2].currentValue = 0;
                this.categoryDials[num2].previousTargetValue = 0;
              }
              else
                Game1.playSound("stoneStep");
            }
          }
          if (this.introTimer < 0)
          {
            if (Game1.options.SnappyMenus)
              this.snapToDefaultClickableComponent();
            Game1.playSound("money");
            this.categoryDials[5].currentValue = 0;
            this.categoryDials[5].previousTargetValue = 0;
          }
        }
        else if (Game1.dayOfMonth != 28 && !this.outro)
        {
          if (!Game1.wasRainingYesterday)
          {
            Vector2 position = new Vector2((float) Game1.uiViewport.Width, (float) Game1.random.Next(200));
            Rectangle sourceRect = new Rectangle(640, 752, 16, 16);
            int num = Game1.random.Next(1, 4);
            if (Game1.random.NextDouble() < 0.001)
            {
              bool flipped = Game1.random.NextDouble() < 0.5;
              if (Game1.random.NextDouble() < 0.5)
                this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(640, 826, 16, 8), 40f, 4, 0, new Vector2((float) Game1.random.Next(this.centerX * 2), (float) Game1.random.Next(this.centerY)), false, flipped)
                {
                  rotation = 3.141593f,
                  scale = 4f,
                  motion = new Vector2(flipped ? -8f : 8f, 8f),
                  local = true
                });
              else
                this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(258, 1680, 16, 16), 40f, 4, 0, new Vector2((float) Game1.random.Next(this.centerX * 2), (float) Game1.random.Next(this.centerY)), false, flipped)
                {
                  scale = 4f,
                  motion = new Vector2(flipped ? -8f : 8f, 8f),
                  local = true
                });
            }
            else if (Game1.random.NextDouble() < 0.0002)
            {
              position = new Vector2((float) Game1.uiViewport.Width, (float) Game1.random.Next(4, 256));
              this.animations.Add(new TemporaryAnimatedSprite("", new Rectangle(0, 0, 1, 1), 9999f, 1, 10000, position, false, false, 0.01f, 0.0f, Color.White * (0.25f + (float) Game1.random.NextDouble()), 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-0.25f, 0.0f)
              });
            }
            else if (Game1.random.NextDouble() < 5E-05)
            {
              position = new Vector2((float) Game1.uiViewport.Width, (float) (Game1.uiViewport.Height - 192));
              for (int index = 0; index < num; ++index)
              {
                this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, (float) Game1.random.Next(60, 101), 4, 100, position + new Vector2((float) ((index + 1) * Game1.random.Next(15, 18)), (float) ((index + 1) * -20)), false, false, 0.01f, 0.0f, Color.Black, 4f, 0.0f, 0.0f, 0.0f, true)
                {
                  motion = new Vector2(-1f, 0.0f)
                });
                this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, (float) Game1.random.Next(60, 101), 4, 100, position + new Vector2((float) ((index + 1) * Game1.random.Next(15, 18)), (float) ((index + 1) * 20)), false, false, 0.01f, 0.0f, Color.Black, 4f, 0.0f, 0.0f, 0.0f, true)
                {
                  motion = new Vector2(-1f, 0.0f)
                });
              }
            }
            else if (Game1.random.NextDouble() < 1E-05)
            {
              sourceRect = new Rectangle(640, 784, 16, 16);
              this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 75f, 4, 1000, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-3f, 0.0f),
                yPeriodic = true,
                yPeriodicLoopTime = 1000f,
                yPeriodicRange = 8f,
                shakeIntensity = 0.5f
              });
            }
          }
          this.smokeTimer -= time.ElapsedGameTime.Milliseconds;
          if (this.smokeTimer <= 0)
          {
            this.smokeTimer = 50;
            this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(684, 1075, 1, 1), 1000f, 1, 1000, new Vector2(188f, (float) (Game1.uiViewport.Height - 128 + 20)), false, false)
            {
              color = Game1.wasRainingYesterday ? Color.SlateGray : Color.White,
              scale = 4f,
              scaleChange = 0.0f,
              alphaFade = 1f / 400f,
              motion = new Vector2(0.0f, (float) ((double) -Game1.random.Next(25, 75) / 100.0 / 4.0)),
              acceleration = new Vector2(-1f / 1000f, 0.0f)
            });
          }
        }
        if (this.moonShake <= 0)
          return;
        int moonShake = this.moonShake;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.moonShake = moonShake - milliseconds;
      }
    }

    public string getCategorySound(int which)
    {
      switch (which)
      {
        case 0:
          return !(this.categoryItems[0][0] as StardewValley.Object).isAnimalProduct() ? "harvest" : "cluck";
        case 1:
          return "leafrustle";
        case 2:
          return "button1";
        case 3:
          return "hammer";
        case 4:
          return "coin";
        case 5:
          return "money";
        default:
          return "stoneStep";
      }
    }

    public override void applyMovementKey(int direction)
    {
      if (!this.CanReceiveInput())
        return;
      base.applyMovementKey(direction);
    }

    public override void performHoverAction(int x, int y)
    {
      if (!this.CanReceiveInput())
        return;
      base.performHoverAction(x, y);
      if (this.currentPage == -1)
      {
        this.okButton.tryHover(x, y);
        foreach (ClickableTextureComponent category in this.categories)
          category.sourceRect.X = !category.containsPoint(x, y) ? 392 : 402;
      }
      else
      {
        this.backButton.tryHover(x, y, 0.5f);
        this.forwardButton.tryHover(x, y, 0.5f);
      }
    }

    public bool CanReceiveInput() => this.introTimer <= 0 && this.saveGameMenu == null && !this.outro;

    public override void receiveKeyPress(Keys key)
    {
      if (!this.CanReceiveInput())
        return;
      if (this.introTimer <= 0 && !Game1.options.gamepadControls && (key.Equals((object) Keys.Escape) || Game1.options.doesInputListContain(Game1.options.menuButton, key)))
      {
        this.receiveLeftClick(this.okButton.bounds.Center.X, this.okButton.bounds.Center.Y, true);
      }
      else
      {
        if (this.introTimer > 0 || Game1.options.gamepadControls && Game1.options.doesInputListContain(Game1.options.menuButton, key))
          return;
        base.receiveKeyPress(key);
      }
    }

    public override void receiveGamePadButton(Buttons b)
    {
      if (!this.CanReceiveInput())
        return;
      base.receiveGamePadButton(b);
      if (b == Buttons.B && this.currentPage != -1)
      {
        if (this.currentTab == 0)
        {
          if (Game1.options.SnappyMenus)
          {
            this.currentlySnappedComponent = this.getComponentWithID(this.currentPage);
            this.snapCursorToCurrentSnappedComponent();
          }
          this.currentPage = -1;
        }
        else
          --this.currentTab;
        Game1.playSound("shwip");
      }
      else
      {
        if (b != Buttons.Start && b != Buttons.B || this.currentPage != -1 || this.outro)
          return;
        if (this.introTimer <= 0)
          this.okClicked();
        else
          this.introTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds * 2;
      }
    }

    private void okClicked()
    {
      this.outro = true;
      this.outroFadeTimer = 800;
      Game1.playSound("bigDeSelect");
      Game1.changeMusicTrack("none");
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (!this.CanReceiveInput())
        return;
      if (this.outro && !this.savedYet)
      {
        SaveGameMenu saveGameMenu = this.saveGameMenu;
      }
      else
      {
        if (this.savedYet)
          return;
        base.receiveLeftClick(x, y, playSound);
        if (this.currentPage == -1 && this.introTimer <= 0 && this.okButton.containsPoint(x, y))
          this.okClicked();
        if (this.currentPage == -1)
        {
          for (int index = 0; index < this.categories.Count; ++index)
          {
            if (this.categories[index].visible && this.categories[index].containsPoint(x, y))
            {
              this.currentPage = index;
              Game1.playSound("shwip");
              if (Game1.options.SnappyMenus)
              {
                this.currentlySnappedComponent = this.getComponentWithID(103);
                this.snapCursorToCurrentSnappedComponent();
                break;
              }
              break;
            }
          }
          if (Game1.dayOfMonth != 28 || this.timesPokedMoon > 10 || !new Rectangle(Game1.uiViewport.Width - 176, 4, 172, 172).Contains(x, y))
            return;
          this.moonShake = 100;
          ++this.timesPokedMoon;
          if (this.timesPokedMoon > 10)
            Game1.playSound("shadowDie");
          else
            Game1.playSound("thudStep");
        }
        else if (this.backButton.containsPoint(x, y))
        {
          if (this.currentTab == 0)
          {
            if (Game1.options.SnappyMenus)
            {
              this.currentlySnappedComponent = this.getComponentWithID(this.currentPage);
              this.snapCursorToCurrentSnappedComponent();
            }
            this.currentPage = -1;
          }
          else
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
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public bool showForwardButton() => this.categoryItems[this.currentPage].Count > this.itemsPerCategoryPage * (this.currentTab + 1);

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      this.initialize(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height);
      this.RepositionItems();
    }

    public override void draw(SpriteBatch b)
    {
      if (Game1.wasRainingYesterday)
      {
        b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(639, 858, 1, 184)), Game1.currentSeason.Equals("winter") ? Color.LightSlateGray : Color.SlateGray * (float) (1.0 - (double) this.introTimer / 3500.0));
        b.Draw(Game1.mouseCursors, new Rectangle(2556, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(639, 858, 1, 184)), Game1.currentSeason.Equals("winter") ? Color.LightSlateGray : Color.SlateGray * (float) (1.0 - (double) this.introTimer / 3500.0));
        for (int index = -244; index < Game1.uiViewport.Width + 244; index += 244)
          b.Draw(Game1.mouseCursors, new Vector2((float) index + (float) ((double) this.weatherX / 2.0 % 244.0), 32f), new Rectangle?(new Rectangle(643, 1142, 61, 53)), Color.DarkSlateGray * 1f * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.uiViewport.Height - 192)), new Rectangle?(new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 48)), (Game1.currentSeason.Equals("winter") ? Color.White * 0.25f : new Color(30, 62, 50)) * (float) (0.5 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(2556f, (float) (Game1.uiViewport.Height - 192)), new Rectangle?(new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 48)), (Game1.currentSeason.Equals("winter") ? Color.White * 0.25f : new Color(30, 62, 50)) * (float) (0.5 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.uiViewport.Height - 128)), new Rectangle?(new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 32)), (Game1.currentSeason.Equals("winter") ? Color.White * 0.5f : new Color(30, 62, 50)) * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(2556f, (float) (Game1.uiViewport.Height - 128)), new Rectangle?(new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 32)), (Game1.currentSeason.Equals("winter") ? Color.White * 0.5f : new Color(30, 62, 50)) * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(160f, (float) (Game1.uiViewport.Height - 128 + 16 + 8)), new Rectangle?(new Rectangle(653, 880, 10, 10)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        for (int index = -244; index < Game1.uiViewport.Width + 244; index += 244)
          b.Draw(Game1.mouseCursors, new Vector2((float) index + this.weatherX % 244f, -32f), new Rectangle?(new Rectangle(643, 1142, 61, 53)), Color.SlateGray * 0.85f * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
        foreach (TemporaryAnimatedSprite animation in this.animations)
          animation.draw(b, true);
        for (int index = -244; index < Game1.uiViewport.Width + 244; index += 244)
          b.Draw(Game1.mouseCursors, new Vector2((float) index + (float) ((double) this.weatherX * 1.5 % 244.0), (float) sbyte.MinValue), new Rectangle?(new Rectangle(643, 1142, 61, 53)), Color.LightSlateGray * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
      }
      else
      {
        b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(639, 858, 1, 184)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0));
        b.Draw(Game1.mouseCursors, new Rectangle(2556, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(639, 858, 1, 184)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0));
        b.Draw(Game1.mouseCursors, new Vector2(0.0f, 0.0f), new Rectangle?(new Rectangle(0, 1453, 639, 195)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(2556f, 0.0f), new Rectangle?(new Rectangle(0, 1453, 639, 195)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        if (Game1.dayOfMonth == 28)
        {
          b.Draw(Game1.mouseCursors, new Vector2((float) (Game1.uiViewport.Width - 176), 4f) + (this.moonShake > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(642, 835, 43, 43)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          if (this.timesPokedMoon > 10)
          {
            SpriteBatch spriteBatch = b;
            Texture2D mouseCursors = Game1.mouseCursors;
            Vector2 position = new Vector2((float) (Game1.uiViewport.Width - 136), 48f) + (this.moonShake > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero);
            int num;
            if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 4000.0 >= 200.0)
            {
              TimeSpan totalGameTime = Game1.currentGameTime.TotalGameTime;
              if (totalGameTime.TotalMilliseconds % 8000.0 > 7600.0)
              {
                totalGameTime = Game1.currentGameTime.TotalGameTime;
                if (totalGameTime.TotalMilliseconds % 8000.0 < 7800.0)
                  goto label_21;
              }
              num = 0;
              goto label_22;
            }
label_21:
            num = 21;
label_22:
            Rectangle? sourceRectangle = new Rectangle?(new Rectangle(685, 844 + num, 19, 21));
            Color color = Color.White * (float) (1.0 - (double) this.introTimer / 3500.0);
            Vector2 zero = Vector2.Zero;
            spriteBatch.Draw(mouseCursors, position, sourceRectangle, color, 0.0f, zero, 4f, SpriteEffects.None, 1f);
          }
        }
        b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.uiViewport.Height - 192)), new Rectangle?(new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 48)), (Game1.currentSeason.Equals("winter") ? Color.White * 0.25f : new Color(0, 20, 40)) * (float) (0.649999976158142 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(2556f, (float) (Game1.uiViewport.Height - 192)), new Rectangle?(new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 48)), (Game1.currentSeason.Equals("winter") ? Color.White * 0.25f : new Color(0, 20, 40)) * (float) (0.649999976158142 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.uiViewport.Height - 128)), new Rectangle?(new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 32)), (Game1.currentSeason.Equals("winter") ? Color.White * 0.5f : new Color(0, 32, 20)) * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(2556f, (float) (Game1.uiViewport.Height - 128)), new Rectangle?(new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1034 : 737, 639, 32)), (Game1.currentSeason.Equals("winter") ? Color.White * 0.5f : new Color(0, 32, 20)) * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(160f, (float) (Game1.uiViewport.Height - 128 + 16 + 8)), new Rectangle?(new Rectangle(653, 880, 10, 10)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      if (!this.outro && !Game1.wasRainingYesterday)
      {
        foreach (TemporaryAnimatedSprite animation in this.animations)
          animation.draw(b, true);
      }
      if (this.currentPage == -1)
      {
        int y = this.categories[0].bounds.Y - 128;
        if (y >= 0)
          SpriteText.drawStringWithScrollCenteredAt(b, Utility.getYesterdaysDate(), Game1.uiViewport.Width / 2, y);
        int num = -20;
        int index1 = 0;
        foreach (ClickableTextureComponent category in this.categories)
        {
          if (this.introTimer < 2500 - index1 * 500)
          {
            Vector2 vector2 = category.getVector2() + new Vector2(12f, -8f);
            if (category.visible)
            {
              category.draw(b);
              b.Draw(Game1.mouseCursors, vector2 + new Vector2(-104f, (float) (num + 4)), new Rectangle?(new Rectangle(293, 360, 24, 24)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
              this.categoryItems[index1][0].drawInMenu(b, vector2 + new Vector2(-88f, (float) (num + 16)), 1f, 1f, 0.9f, StackDrawType.Hide);
            }
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), (int) ((double) vector2.X + (double) -this.itemSlotWidth - (double) this.categoryLabelsWidth - 12.0), (int) ((double) vector2.Y + (double) num), this.categoryLabelsWidth, 104, Color.White, 4f, false);
            SpriteText.drawString(b, category.hoverText, (int) vector2.X - this.itemSlotWidth - this.categoryLabelsWidth + 8, (int) vector2.Y + 4);
            for (int index2 = 0; index2 < 6; ++index2)
              b.Draw(Game1.mouseCursors, vector2 + new Vector2((float) (-this.itemSlotWidth - 192 - 24 + index2 * 6 * 4), 12f), new Rectangle?(new Rectangle(355, 476, 7, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
            this.categoryDials[index1].draw(b, vector2 + new Vector2((float) (-this.itemSlotWidth - 192 - 48 + 4), 20f), this.categoryTotals[index1]);
            b.Draw(Game1.mouseCursors, vector2 + new Vector2((float) (-this.itemSlotWidth - 64 - 4), 12f), new Rectangle?(new Rectangle(408, 476, 9, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
          }
          ++index1;
        }
        if (this.introTimer <= 0)
          this.okButton.draw(b);
      }
      else
      {
        IClickableMenu.drawTextureBox(b, 0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height, Color.White);
        Vector2 location = new Vector2((float) (this.xPositionOnScreen + 32), (float) (this.yPositionOnScreen + 32));
        for (int index3 = this.currentTab * this.itemsPerCategoryPage; index3 < this.currentTab * this.itemsPerCategoryPage + this.itemsPerCategoryPage; ++index3)
        {
          if (this.categoryItems[this.currentPage].Count > index3)
          {
            Item key = this.categoryItems[this.currentPage][index3];
            key.drawInMenu(b, location, 1f, 1f, 1f, StackDrawType.Draw);
            bool flag = true;
            int stack;
            if (LocalizedContentManager.CurrentLanguageLatin)
            {
              string s1;
              if (flag)
              {
                s1 = key.DisplayName + " x" + Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this.singleItemValues[key]);
              }
              else
              {
                string displayName = key.DisplayName;
                string str;
                if (key.Stack <= 1)
                {
                  str = "";
                }
                else
                {
                  stack = key.Stack;
                  str = " x" + stack.ToString();
                }
                s1 = displayName + str;
              }
              SpriteText.drawString(b, s1, (int) location.X + 64 + 12, (int) location.Y + 12);
              string s2 = ".";
              for (int index4 = 0; index4 < this.width - 96 - SpriteText.getWidthOfString(s1 + Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this.itemValues[key])); index4 += SpriteText.getWidthOfString(" ."))
                s2 += " .";
              SpriteText.drawString(b, s2, (int) location.X + 80 + SpriteText.getWidthOfString(s1), (int) location.Y + 8);
              SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this.itemValues[key]), (int) location.X + this.width - 64 - SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this.itemValues[key])), (int) location.Y + 12);
            }
            else
            {
              string s3;
              if (flag)
              {
                s3 = key.DisplayName + " x" + Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this.singleItemValues[key]);
              }
              else
              {
                string displayName = key.DisplayName;
                string str;
                if (key.Stack <= 1)
                {
                  str = ".";
                }
                else
                {
                  stack = key.Stack;
                  str = " x" + stack.ToString();
                }
                s3 = displayName + str;
              }
              string s4 = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this.itemValues[key]);
              int x = (int) location.X + this.width - 64 - SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this.itemValues[key]));
              SpriteText.getWidthOfString(s3 + s4);
              while (SpriteText.getWidthOfString(s3 + s4) < 1123)
                s3 += " .";
              if (SpriteText.getWidthOfString(s3 + s4) >= 1155)
                s3 = s3.Remove(s3.Length - 1);
              SpriteText.drawString(b, s3, (int) location.X + 64 + 12, (int) location.Y + 12);
              SpriteText.drawString(b, s4, x, (int) location.Y + 12);
            }
            location.Y += 68f;
          }
        }
        this.backButton.draw(b);
        if (this.showForwardButton())
          this.forwardButton.draw(b);
      }
      if (this.outro)
      {
        b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(639, 858, 1, 184)), Color.Black * (float) (1.0 - (double) this.outroFadeTimer / 800.0));
        SpriteText.drawStringWithScrollCenteredAt(b, this.newDayPlaque ? Utility.getDateString() : Utility.getYesterdaysDate(), Game1.uiViewport.Width / 2, this.dayPlaqueY);
        foreach (TemporaryAnimatedSprite animation in this.animations)
          animation.draw(b, true);
        if (this.finalOutroTimer > 0 || this._hasFinished)
          b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * (float) (1.0 - (double) this.finalOutroTimer / 2000.0));
      }
      if (this.saveGameMenu != null)
        this.saveGameMenu.draw(b);
      if (Game1.options.SnappyMenus && (this.introTimer > 0 || this.outro))
        return;
      Game1.mouseCursorTransparency = 1f;
      this.drawMouse(b);
    }
  }
}
