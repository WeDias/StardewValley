// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.GameMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class GameMenu : IClickableMenu
  {
    public const int inventoryTab = 0;
    public const int skillsTab = 1;
    public const int socialTab = 2;
    public const int mapTab = 3;
    public const int craftingTab = 4;
    public const int collectionsTab = 5;
    public const int optionsTab = 6;
    public const int exitTab = 7;
    public const int region_inventoryTab = 12340;
    public const int region_skillsTab = 12341;
    public const int region_socialTab = 12342;
    public const int region_mapTab = 12343;
    public const int region_craftingTab = 12344;
    public const int region_collectionsTab = 12345;
    public const int region_optionsTab = 12346;
    public const int region_exitTab = 12347;
    public const int numberOfTabs = 7;
    public int currentTab;
    public int lastOpenedNonMapTab;
    public string hoverText = "";
    public string descriptionText = "";
    public List<ClickableComponent> tabs = new List<ClickableComponent>();
    public List<IClickableMenu> pages = new List<IClickableMenu>();
    public bool invisible;
    public static bool forcePreventClose;
    public static bool bundleItemHovered;

    public GameMenu(bool playOpeningSound = true)
      : base(Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true)
    {
      this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "inventory", Game1.content.LoadString("Strings\\UI:GameMenu_Inventory"))
      {
        myID = 12340,
        downNeighborID = 0,
        rightNeighborID = 12341,
        tryDefaultIfNoDownNeighborExists = true,
        fullyImmutable = true
      });
      this.pages.Add((IClickableMenu) new InventoryPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height));
      this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 128, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "skills", Game1.content.LoadString("Strings\\UI:GameMenu_Skills"))
      {
        myID = 12341,
        downNeighborID = 1,
        rightNeighborID = 12342,
        leftNeighborID = 12340,
        tryDefaultIfNoDownNeighborExists = true,
        fullyImmutable = true
      });
      this.pages.Add((IClickableMenu) new SkillsPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it ? 64 : 0), this.height));
      this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 192, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "social", Game1.content.LoadString("Strings\\UI:GameMenu_Social"))
      {
        myID = 12342,
        downNeighborID = 2,
        rightNeighborID = 12343,
        leftNeighborID = 12341,
        tryDefaultIfNoDownNeighborExists = true,
        fullyImmutable = true
      });
      this.pages.Add((IClickableMenu) new SocialPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width + 36, this.height));
      this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 256, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "map", Game1.content.LoadString("Strings\\UI:GameMenu_Map"))
      {
        myID = 12343,
        downNeighborID = 3,
        rightNeighborID = 12344,
        leftNeighborID = 12342,
        tryDefaultIfNoDownNeighborExists = true,
        fullyImmutable = true
      });
      this.pages.Add((IClickableMenu) new MapPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height));
      this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 320, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "crafting", Game1.content.LoadString("Strings\\UI:GameMenu_Crafting"))
      {
        myID = 12344,
        downNeighborID = 4,
        rightNeighborID = 12345,
        leftNeighborID = 12343,
        tryDefaultIfNoDownNeighborExists = true,
        fullyImmutable = true
      });
      this.pages.Add((IClickableMenu) new CraftingPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height));
      this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 384, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "collections", Game1.content.LoadString("Strings\\UI:GameMenu_Collections"))
      {
        myID = 12345,
        downNeighborID = 5,
        rightNeighborID = 12346,
        leftNeighborID = 12344,
        tryDefaultIfNoDownNeighborExists = true,
        fullyImmutable = true
      });
      this.pages.Add((IClickableMenu) new CollectionsPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width - 64 - 16, this.height));
      this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 448, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "options", Game1.content.LoadString("Strings\\UI:GameMenu_Options"))
      {
        myID = 12346,
        downNeighborID = 6,
        rightNeighborID = 12347,
        leftNeighborID = 12345,
        tryDefaultIfNoDownNeighborExists = true,
        fullyImmutable = true
      });
      int num;
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.ru:
          num = 96;
          break;
        case LocalizedContentManager.LanguageCode.fr:
        case LocalizedContentManager.LanguageCode.tr:
          num = 192;
          break;
        default:
          num = 0;
          break;
      }
      this.pages.Add((IClickableMenu) new OptionsPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width + num, this.height));
      this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 512, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "exit", Game1.content.LoadString("Strings\\UI:GameMenu_Exit"))
      {
        myID = 12347,
        downNeighborID = 7,
        leftNeighborID = 12346,
        tryDefaultIfNoDownNeighborExists = true,
        fullyImmutable = true
      });
      this.pages.Add((IClickableMenu) new ExitPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width - 64 - 16, this.height));
      if (Game1.activeClickableMenu == null & playOpeningSound)
        Game1.playSound("bigSelect");
      GameMenu.forcePreventClose = false;
      (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).refreshBundlesIngredientsInfo();
      this.pages[this.currentTab].populateClickableComponentList();
      this.AddTabsToClickableComponents(this.pages[this.currentTab]);
      if (!Game1.options.SnappyMenus)
        return;
      this.snapToDefaultClickableComponent();
    }

    public void AddTabsToClickableComponents(IClickableMenu menu) => menu.allClickableComponents.AddRange((IEnumerable<ClickableComponent>) this.tabs);

    public GameMenu(int startingTab, int extra = -1, bool playOpeningSound = true)
      : this(playOpeningSound)
    {
      this.changeTab(startingTab, false);
      if (startingTab != 6 || extra == -1)
        return;
      (this.pages[6] as OptionsPage).currentItemIndex = extra;
    }

    public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
    {
      if (this.GetCurrentPage() != null)
        this.GetCurrentPage().automaticSnapBehavior(direction, oldRegion, oldID);
      else
        base.automaticSnapBehavior(direction, oldRegion, oldID);
    }

    public override void snapToDefaultClickableComponent()
    {
      if (this.currentTab >= this.pages.Count)
        return;
      this.pages[this.currentTab].snapToDefaultClickableComponent();
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      switch (b)
      {
        case Buttons.Back:
          if (this.currentTab == 0)
          {
            this.pages[this.currentTab].receiveGamePadButton(b);
            return;
          }
          break;
        case Buttons.RightTrigger:
          if (this.currentTab == 3)
          {
            Game1.activeClickableMenu = (IClickableMenu) new GameMenu(4);
            Game1.playSound("smallSelect");
            return;
          }
          if (this.currentTab >= 7 || !this.pages[this.currentTab].readyToClose())
            return;
          this.changeTab(this.currentTab + 1);
          return;
        case Buttons.LeftTrigger:
          if (this.currentTab == 3)
          {
            Game1.activeClickableMenu = (IClickableMenu) new GameMenu(2);
            Game1.playSound("smallSelect");
            return;
          }
          if (this.currentTab <= 0 || !this.pages[this.currentTab].readyToClose())
            return;
          this.changeTab(this.currentTab - 1);
          return;
      }
      this.pages[this.currentTab].receiveGamePadButton(b);
    }

    public override void setUpForGamePadMode()
    {
      base.setUpForGamePadMode();
      if (this.pages.Count <= this.currentTab)
        return;
      this.pages[this.currentTab].setUpForGamePadMode();
    }

    public override ClickableComponent getCurrentlySnappedComponent() => this.pages[this.currentTab].getCurrentlySnappedComponent();

    public override void setCurrentlySnappedComponentTo(int id) => this.pages[this.currentTab].setCurrentlySnappedComponentTo(id);

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (!(this.pages[this.currentTab] is CollectionsPage) || (this.pages[this.currentTab] as CollectionsPage).letterviewerSubMenu == null)
        base.receiveLeftClick(x, y, playSound);
      if (!this.invisible && !GameMenu.forcePreventClose)
      {
        for (int index = 0; index < this.tabs.Count; ++index)
        {
          if (this.tabs[index].containsPoint(x, y) && this.currentTab != index && this.pages[this.currentTab].readyToClose())
          {
            this.changeTab(this.getTabNumberFromName(this.tabs[index].name));
            return;
          }
        }
      }
      this.pages[this.currentTab].receiveLeftClick(x, y);
    }

    public static string getLabelOfTabFromIndex(int index)
    {
      switch (index)
      {
        case 0:
          return Game1.content.LoadString("Strings\\UI:GameMenu_Inventory");
        case 1:
          return Game1.content.LoadString("Strings\\UI:GameMenu_Skills");
        case 2:
          return Game1.content.LoadString("Strings\\UI:GameMenu_Social");
        case 3:
          return Game1.content.LoadString("Strings\\UI:GameMenu_Map");
        case 4:
          return Game1.content.LoadString("Strings\\UI:GameMenu_Crafting");
        case 5:
          return Game1.content.LoadString("Strings\\UI:GameMenu_Collections");
        case 6:
          return Game1.content.LoadString("Strings\\UI:GameMenu_Options");
        case 7:
          return Game1.content.LoadString("Strings\\UI:GameMenu_Exit");
        default:
          return "";
      }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true) => this.pages[this.currentTab].receiveRightClick(x, y);

    public override void receiveScrollWheelAction(int direction)
    {
      base.receiveScrollWheelAction(direction);
      this.pages[this.currentTab].receiveScrollWheelAction(direction);
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      this.hoverText = "";
      this.pages[this.currentTab].performHoverAction(x, y);
      foreach (ClickableComponent tab in this.tabs)
      {
        if (tab.containsPoint(x, y))
        {
          this.hoverText = tab.label;
          break;
        }
      }
    }

    public int getTabNumberFromName(string name)
    {
      int tabNumberFromName = -1;
      switch (name)
      {
        case "collections":
          tabNumberFromName = 5;
          break;
        case "crafting":
          tabNumberFromName = 4;
          break;
        case "exit":
          tabNumberFromName = 7;
          break;
        case "inventory":
          tabNumberFromName = 0;
          break;
        case "map":
          tabNumberFromName = 3;
          break;
        case "options":
          tabNumberFromName = 6;
          break;
        case "skills":
          tabNumberFromName = 1;
          break;
        case "social":
          tabNumberFromName = 2;
          break;
      }
      return tabNumberFromName;
    }

    public override void update(GameTime time)
    {
      base.update(time);
      this.pages[this.currentTab].update(time);
    }

    public override void releaseLeftClick(int x, int y)
    {
      base.releaseLeftClick(x, y);
      this.pages[this.currentTab].releaseLeftClick(x, y);
    }

    public override void leftClickHeld(int x, int y)
    {
      base.leftClickHeld(x, y);
      this.pages[this.currentTab].leftClickHeld(x, y);
    }

    public override bool readyToClose() => !GameMenu.forcePreventClose && this.pages[this.currentTab].readyToClose();

    public void changeTab(int whichTab, bool playSound = true)
    {
      this.currentTab = this.getTabNumberFromName(this.tabs[whichTab].name);
      if (this.currentTab == 3)
      {
        this.invisible = true;
        this.width += 128;
        this.initializeUpperRightCloseButton();
      }
      else
      {
        this.lastOpenedNonMapTab = this.currentTab;
        this.width = 800 + IClickableMenu.borderWidth * 2;
        this.initializeUpperRightCloseButton();
        this.invisible = false;
      }
      if (playSound)
        Game1.playSound("smallSelect");
      this.pages[this.currentTab].populateClickableComponentList();
      this.AddTabsToClickableComponents(this.pages[this.currentTab]);
      this.setTabNeighborsForCurrentPage();
      if (!Game1.options.SnappyMenus)
        return;
      this.snapToDefaultClickableComponent();
    }

    public IClickableMenu GetCurrentPage() => this.currentTab >= this.pages.Count || this.currentTab < 0 ? (IClickableMenu) null : this.pages[this.currentTab];

    public void setTabNeighborsForCurrentPage()
    {
      switch (this.currentTab)
      {
        case 0:
          for (int index = 0; index < this.tabs.Count; ++index)
            this.tabs[index].downNeighborID = index;
          break;
        case 7:
          for (int index = 0; index < this.tabs.Count; ++index)
            this.tabs[index].downNeighborID = 535;
          break;
        default:
          for (int index = 0; index < this.tabs.Count; ++index)
            this.tabs[index].downNeighborID = -99999;
          break;
      }
    }

    public override void draw(SpriteBatch b)
    {
      if (!this.invisible)
      {
        if (!Game1.options.showMenuBackground)
          b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
        Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.pages[this.currentTab].width, this.pages[this.currentTab].height, false, true);
        b.End();
        b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
        foreach (ClickableComponent tab in this.tabs)
        {
          int num = 0;
          switch (tab.name)
          {
            case "catalogue":
              num = 7;
              break;
            case "collections":
              num = 5;
              break;
            case "coop":
              num = 1;
              break;
            case "crafting":
              num = 4;
              break;
            case "exit":
              num = 7;
              break;
            case "inventory":
              num = 0;
              break;
            case "map":
              num = 3;
              break;
            case "options":
              num = 6;
              break;
            case "skills":
              num = 1;
              break;
            case "social":
              num = 2;
              break;
          }
          b.Draw(Game1.mouseCursors, new Vector2((float) tab.bounds.X, (float) (tab.bounds.Y + (this.currentTab == this.getTabNumberFromName(tab.name) ? 8 : 0))), new Rectangle?(new Rectangle(num * 16, 368, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
          if (tab.name.Equals("skills"))
            Game1.player.FarmerRenderer.drawMiniPortrat(b, new Vector2((float) (tab.bounds.X + 8), (float) (tab.bounds.Y + 12 + (this.currentTab == this.getTabNumberFromName(tab.name) ? 8 : 0))), 0.00011f, 3f, 2, Game1.player);
        }
        b.End();
        b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        this.pages[this.currentTab].draw(b);
        if (!this.hoverText.Equals(""))
          IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      }
      else
        this.pages[this.currentTab].draw(b);
      if (!GameMenu.forcePreventClose && this.pages[this.currentTab].shouldDrawCloseButton())
        base.draw(b);
      if (Game1.options.SnappyMenus && this.pages[this.currentTab] is CollectionsPage && (this.pages[this.currentTab] as CollectionsPage).letterviewerSubMenu != null || Game1.options.hardwareCursor)
        return;
      this.drawMouse(b, true);
    }

    public override bool areGamePadControlsImplemented() => false;

    public override void receiveKeyPress(Keys key)
    {
      if (((IEnumerable<InputButton>) Game1.options.menuButton).Contains<InputButton>(new InputButton(key)) && this.readyToClose())
      {
        Game1.exitActiveMenu();
        Game1.playSound("bigDeSelect");
      }
      this.pages[this.currentTab].receiveKeyPress(key);
    }

    public override void emergencyShutDown()
    {
      base.emergencyShutDown();
      this.pages[this.currentTab].emergencyShutDown();
    }

    protected override void cleanupBeforeExit()
    {
      base.cleanupBeforeExit();
      if (!Game1.options.optionsDirty)
        return;
      Game1.options.SaveDefaultOptions();
    }
  }
}
