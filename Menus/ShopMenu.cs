// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ShopMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class ShopMenu : IClickableMenu
  {
    public const int region_shopButtonModifier = 3546;
    public const int region_upArrow = 97865;
    public const int region_downArrow = 97866;
    public const int region_tabStartIndex = 99999;
    public const int howManyRecipesFitOnPage = 28;
    public const int infiniteStock = 2147483647;
    public const int salePriceIndex = 0;
    public const int stockIndex = 1;
    public const int extraTradeItemIndex = 2;
    public const int extraTradeItemCountIndex = 3;
    public const int itemsPerPage = 4;
    public const int numberRequiredForExtraItemTrade = 5;
    private string descriptionText = "";
    private string hoverText = "";
    private string boldTitleText = "";
    public string purchaseSound = "purchaseClick";
    public string purchaseRepeatSound = "purchaseRepeat";
    public string storeContext = "";
    public InventoryMenu inventory;
    public ISalable heldItem;
    public ISalable hoveredItem;
    private Texture2D wallpapers;
    private Texture2D floors;
    private int lastWallpaperFloorPrice;
    private TemporaryAnimatedSprite poof;
    private Rectangle scrollBarRunner;
    public List<ISalable> forSale = new List<ISalable>();
    public List<ClickableComponent> forSaleButtons = new List<ClickableComponent>();
    public List<int> categoriesToSellHere = new List<int>();
    public Dictionary<ISalable, int[]> itemPriceAndStock = new Dictionary<ISalable, int[]>();
    private float sellPercentage = 1f;
    private List<TemporaryAnimatedSprite> animations = new List<TemporaryAnimatedSprite>();
    public int hoverPrice = -1;
    public int currency;
    public int currentItemIndex;
    public ClickableTextureComponent upArrow;
    public ClickableTextureComponent downArrow;
    public ClickableTextureComponent scrollBar;
    public NPC portraitPerson;
    public string potraitPersonDialogue;
    public object source;
    private bool scrolling;
    public Func<ISalable, Farmer, int, bool> onPurchase;
    public Func<ISalable, bool> onSell;
    public Func<int, bool> canPurchaseCheck;
    public List<ClickableTextureComponent> tabButtons = new List<ClickableTextureComponent>();
    protected int currentTab;
    protected bool _isStorageShop;
    public bool readOnly;
    public HashSet<ISalable> buyBackItems = new HashSet<ISalable>();
    public Dictionary<ISalable, ISalable> buyBackItemsToResellTomorrow = new Dictionary<ISalable, ISalable>();

    public ShopMenu(
      Dictionary<ISalable, int[]> itemPriceAndStock,
      int currency = 0,
      string who = null,
      Func<ISalable, Farmer, int, bool> on_purchase = null,
      Func<ISalable, bool> on_sell = null,
      string context = null)
      : this(itemPriceAndStock.Keys.ToList<ISalable>(), currency, who, on_purchase, on_sell, context)
    {
      this.itemPriceAndStock = itemPriceAndStock;
      if (this.potraitPersonDialogue != null)
        return;
      this.setUpShopOwner(who);
    }

    public ShopMenu(
      List<ISalable> itemsForSale,
      int currency = 0,
      string who = null,
      Func<ISalable, Farmer, int, bool> on_purchase = null,
      Func<ISalable, bool> on_sell = null,
      string context = null)
      : base(Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 1000 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true)
    {
      foreach (ISalable key in itemsForSale)
      {
        if (key is StardewValley.Object && (bool) (NetFieldBase<bool, NetBool>) (key as StardewValley.Object).isRecipe)
        {
          if (!Game1.player.knowsRecipe(key.Name))
            key.Stack = 1;
          else
            continue;
        }
        this.forSale.Add(key);
        this.itemPriceAndStock.Add(key, new int[2]
        {
          key.salePrice(),
          key.Stack
        });
      }
      if (this.itemPriceAndStock.Count >= 2)
        this.setUpShopOwner(who);
      this.updatePosition();
      this.currency = currency;
      this.onPurchase = on_purchase;
      this.onSell = on_sell;
      Game1.player.forceCanMove();
      switch (context)
      {
        case "QiGemShop":
          this.inventory = new InventoryMenu(this.xPositionOnScreen + this.width, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 320 + 40, false, highlightMethod: new InventoryMenu.highlightThisItem(this.highlightItemToSell))
          {
            showGrayedOutSlots = true
          };
          this.inventory.movePosition(-this.inventory.width - 32, 0);
          this.currency = currency;
          ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16, this.yPositionOnScreen + 16, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
          textureComponent1.myID = 97865;
          textureComponent1.downNeighborID = 106;
          textureComponent1.leftNeighborID = 3546;
          this.upArrow = textureComponent1;
          ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16, this.yPositionOnScreen + this.height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
          textureComponent2.myID = 106;
          textureComponent2.upNeighborID = 97865;
          textureComponent2.leftNeighborID = 3546;
          this.downArrow = textureComponent2;
          this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upArrow.bounds.X + 12, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
          this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, this.scrollBar.bounds.Width, this.height - 64 - this.upArrow.bounds.Height - 28);
          for (int index = 0; index < 4; ++index)
            this.forSaleButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 16 + index * ((this.height - 256) / 4), this.width - 32, (this.height - 256) / 4 + 4), index.ToString() ?? "")
            {
              myID = index + 3546,
              rightNeighborID = 97865,
              fullyImmutable = true
            });
          this.updateSaleButtonNeighbors();
          if (context == null)
            context = (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name;
          this.storeContext = context;
          this.setUpStoreForContext();
          if (this.tabButtons.Count > 0)
          {
            foreach (ClickableComponent forSaleButton in this.forSaleButtons)
              forSaleButton.leftNeighborID = -99998;
          }
          this.applyTab();
          foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Top))
            clickableComponent.upNeighborID = -99998;
          if (Game1.options.snappyMenus && Game1.options.gamepadControls)
          {
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
          }
          if (currency != 4)
            break;
          Game1.specialCurrencyDisplay.ShowCurrency("qiGems");
          break;
        default:
          Game1.playSound("dwop");
          goto case "QiGemShop";
      }
    }

    public void updateSaleButtonNeighbors()
    {
      ClickableComponent clickableComponent = this.forSaleButtons[0];
      for (int index = 0; index < this.forSaleButtons.Count; ++index)
      {
        ClickableComponent forSaleButton = this.forSaleButtons[index];
        forSaleButton.upNeighborImmutable = true;
        forSaleButton.downNeighborImmutable = true;
        forSaleButton.upNeighborID = index > 0 ? index + 3546 - 1 : -7777;
        forSaleButton.downNeighborID = index >= 3 || index >= this.forSale.Count - 1 ? -7777 : index + 3546 + 1;
        if (index >= this.forSale.Count)
        {
          if (forSaleButton == this.currentlySnappedComponent)
          {
            this.currentlySnappedComponent = clickableComponent;
            if (Game1.options.SnappyMenus)
              this.snapCursorToCurrentSnappedComponent();
          }
        }
        else
          clickableComponent = forSaleButton;
      }
    }

    public virtual void setUpStoreForContext()
    {
      this.tabButtons = new List<ClickableTextureComponent>();
      switch (this.storeContext)
      {
        case "AdventureGuild":
          this.categoriesToSellHere.AddRange((IEnumerable<int>) new int[4]
          {
            -28,
            -98,
            -97,
            -96
          });
          break;
        case "AnimalShop":
          this.categoriesToSellHere.AddRange((IEnumerable<int>) new int[4]
          {
            -18,
            -6,
            -5,
            -14
          });
          break;
        case "Blacksmith":
        case "VolcanoShop":
          this.categoriesToSellHere.AddRange((IEnumerable<int>) new int[3]
          {
            -12,
            -2,
            -15
          });
          break;
        case "Catalogue":
          ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(96, 48, 16, 16), 4f);
          textureComponent1.myID = 99999 + this.tabButtons.Count;
          textureComponent1.upNeighborID = -99998;
          textureComponent1.downNeighborID = -99998;
          textureComponent1.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent1);
          ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(48, 64, 16, 16), 4f);
          textureComponent2.myID = 99999 + this.tabButtons.Count;
          textureComponent2.upNeighborID = -99998;
          textureComponent2.downNeighborID = -99998;
          textureComponent2.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent2);
          ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(32, 64, 16, 16), 4f);
          textureComponent3.myID = 99999 + this.tabButtons.Count;
          textureComponent3.upNeighborID = -99998;
          textureComponent3.downNeighborID = -99998;
          textureComponent3.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent3);
          break;
        case "Dresser":
          this.categoriesToSellHere.AddRange((IEnumerable<int>) new int[4]
          {
            -95,
            -100,
            -97,
            -96
          });
          this._isStorageShop = true;
          ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(0, 48, 16, 16), 4f);
          textureComponent4.myID = 99999 + this.tabButtons.Count;
          textureComponent4.upNeighborID = -99998;
          textureComponent4.downNeighborID = -99998;
          textureComponent4.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent4);
          ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(16, 48, 16, 16), 4f);
          textureComponent5.myID = 99999 + this.tabButtons.Count;
          textureComponent5.upNeighborID = -99998;
          textureComponent5.downNeighborID = -99998;
          textureComponent5.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent5);
          ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(32, 48, 16, 16), 4f);
          textureComponent6.myID = 99999 + this.tabButtons.Count;
          textureComponent6.upNeighborID = -99998;
          textureComponent6.downNeighborID = -99998;
          textureComponent6.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent6);
          ClickableTextureComponent textureComponent7 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(48, 48, 16, 16), 4f);
          textureComponent7.myID = 99999 + this.tabButtons.Count;
          textureComponent7.upNeighborID = -99998;
          textureComponent7.downNeighborID = -99998;
          textureComponent7.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent7);
          ClickableTextureComponent textureComponent8 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(0, 64, 16, 16), 4f);
          textureComponent8.myID = 99999 + this.tabButtons.Count;
          textureComponent8.upNeighborID = -99998;
          textureComponent8.downNeighborID = -99998;
          textureComponent8.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent8);
          ClickableTextureComponent textureComponent9 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(16, 64, 16, 16), 4f);
          textureComponent9.myID = 99999 + this.tabButtons.Count;
          textureComponent9.upNeighborID = -99998;
          textureComponent9.downNeighborID = -99998;
          textureComponent9.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent9);
          break;
        case "FishShop":
          this.categoriesToSellHere.AddRange((IEnumerable<int>) new int[4]
          {
            -4,
            -23,
            -21,
            -22
          });
          break;
        case "FishTank":
          this._isStorageShop = true;
          break;
        case "Furniture Catalogue":
          ClickableTextureComponent textureComponent10 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(96, 48, 16, 16), 4f);
          textureComponent10.myID = 99999 + this.tabButtons.Count;
          textureComponent10.upNeighborID = -99998;
          textureComponent10.downNeighborID = -99998;
          textureComponent10.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent10);
          ClickableTextureComponent textureComponent11 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(80, 48, 16, 16), 4f);
          textureComponent11.myID = 99999 + this.tabButtons.Count;
          textureComponent11.upNeighborID = -99998;
          textureComponent11.downNeighborID = -99998;
          textureComponent11.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent11);
          ClickableTextureComponent textureComponent12 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(64, 48, 16, 16), 4f);
          textureComponent12.myID = 99999 + this.tabButtons.Count;
          textureComponent12.upNeighborID = -99998;
          textureComponent12.downNeighborID = -99998;
          textureComponent12.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent12);
          ClickableTextureComponent textureComponent13 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(64, 64, 16, 16), 4f);
          textureComponent13.myID = 99999 + this.tabButtons.Count;
          textureComponent13.upNeighborID = -99998;
          textureComponent13.downNeighborID = -99998;
          textureComponent13.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent13);
          ClickableTextureComponent textureComponent14 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(96, 64, 16, 16), 4f);
          textureComponent14.myID = 99999 + this.tabButtons.Count;
          textureComponent14.upNeighborID = -99998;
          textureComponent14.downNeighborID = -99998;
          textureComponent14.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent14);
          ClickableTextureComponent textureComponent15 = new ClickableTextureComponent(new Rectangle(0, 0, 64, 64), Game1.mouseCursors2, new Rectangle(80, 64, 16, 16), 4f);
          textureComponent15.myID = 99999 + this.tabButtons.Count;
          textureComponent15.upNeighborID = -99998;
          textureComponent15.downNeighborID = -99998;
          textureComponent15.rightNeighborID = 3546;
          this.tabButtons.Add(textureComponent15);
          break;
        case "ReturnedDonations":
          this._isStorageShop = true;
          break;
        case "ScienceHouse":
          this.categoriesToSellHere.AddRange((IEnumerable<int>) new int[1]
          {
            -16
          });
          break;
        case "SeedShop":
          this.categoriesToSellHere.AddRange((IEnumerable<int>) new int[14]
          {
            -81,
            -75,
            -79,
            -80,
            -74,
            -17,
            -18,
            -6,
            -26,
            -5,
            -14,
            -19,
            -7,
            -25
          });
          break;
      }
      this.repositionTabs();
      if (!this._isStorageShop)
        return;
      this.purchaseSound = (string) null;
      this.purchaseRepeatSound = (string) null;
    }

    public void repositionTabs()
    {
      for (int index = 0; index < this.tabButtons.Count; ++index)
      {
        if (index == this.currentTab)
          this.tabButtons[index].bounds.X = this.xPositionOnScreen - 56;
        else
          this.tabButtons[index].bounds.X = this.xPositionOnScreen - 64;
        this.tabButtons[index].bounds.Y = this.yPositionOnScreen + index * 16 * 4 + 16;
      }
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
      switch (direction)
      {
        case 0:
          if (this.currentItemIndex <= 0)
            break;
          this.upArrowPressed();
          this.currentlySnappedComponent = this.getComponentWithID(3546);
          this.snapCursorToCurrentSnappedComponent();
          break;
        case 2:
          if (this.currentItemIndex < Math.Max(0, this.forSale.Count - 4))
          {
            this.downArrowPressed();
            break;
          }
          int num = -1;
          for (int index = 0; index < 12; ++index)
          {
            this.inventory.inventory[index].upNeighborID = oldID;
            if (num == -1 && this.heldItem != null && this.inventory.actualInventory != null && this.inventory.actualInventory.Count > index && this.inventory.actualInventory[index] == null)
              num = index;
          }
          this.currentlySnappedComponent = this.getComponentWithID(num != -1 ? num : 0);
          this.snapCursorToCurrentSnappedComponent();
          break;
      }
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(3546);
      this.snapCursorToCurrentSnappedComponent();
    }

    public void setUpShopOwner(string who)
    {
      if (who == null)
        return;
      Random random = new Random((int) ((long) Game1.uniqueIDForThisGame + (long) Game1.stats.DaysPlayed));
      string text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11457");
      switch (who)
      {
        case "BlueBoat":
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.blueboat");
          break;
        case "Clint":
          this.portraitPerson = Game1.getCharacterFromName("Clint");
          switch (Game1.random.Next(3))
          {
            case 0:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11469");
              break;
            case 1:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11470");
              break;
            case 2:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11471");
              break;
          }
          break;
        case "ClintUpgrade":
          this.portraitPerson = Game1.getCharacterFromName("Clint");
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11474");
          break;
        case "Concessions":
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:MovieTheaterConcessions" + Game1.random.Next(5).ToString());
          break;
        case "DesertTrade":
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:DesertTrader" + (random.Next(2) + 1).ToString());
          if (random.NextDouble() < 0.2)
          {
            int num = random.Next(2) + 3;
            text = Game1.content.LoadString("Strings\\StringsFromCSFiles:DesertTrader" + num.ToString() + (num == 4 ? "_" + ((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? "male" : "female") : ""));
            break;
          }
          break;
        case "Dwarf":
          this.portraitPerson = Game1.getCharacterFromName("Dwarf");
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11492");
          break;
        case "Gus":
          this.portraitPerson = Game1.getCharacterFromName("Gus");
          switch (Game1.random.Next(4))
          {
            case 0:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11511");
              break;
            case 1:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11512", (object) this.itemPriceAndStock.ElementAt<KeyValuePair<ISalable, int[]>>(random.Next(this.itemPriceAndStock.Count)).Key.DisplayName);
              break;
            case 2:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11513");
              break;
            case 3:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11514");
              break;
          }
          break;
        case "HatMouse":
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11494");
          break;
        case "IslandTrade":
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:IslandTrader" + (random.Next(2) + 1).ToString());
          if (random.NextDouble() < 0.2)
          {
            int num = random.Next(2) + 3;
            text = Game1.content.LoadString("Strings\\StringsFromCSFiles:IslandTrader" + num.ToString() + (num == 4 ? "_" + ((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? "male" : "female") : ""));
          }
          if (Game1.stats.getStat("hardModeMonstersKilled") > 50U && Game1.dayOfMonth != 28 && random.NextDouble() < 0.2)
          {
            text = Game1.content.LoadString("Strings\\StringsFromCSFiles:IslandTraderSecret");
            break;
          }
          break;
        case "Krobus":
          this.portraitPerson = Game1.getCharacterFromName("Krobus");
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11497");
          break;
        case "KrobusGone":
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:KrobusShopGone");
          break;
        case "Marlon":
          this.portraitPerson = Game1.getCharacterFromName("Marlon");
          switch (random.Next(4))
          {
            case 0:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11517");
              break;
            case 1:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11518");
              break;
            case 2:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11519");
              break;
            case 3:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11520");
              break;
          }
          if (random.NextDouble() < 0.001)
          {
            text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11521");
            break;
          }
          break;
        case "Marlon_Recovery":
          this.portraitPerson = Game1.getCharacterFromName("Marlon");
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ItemRecovery_Description");
          break;
        case "Marnie":
          this.portraitPerson = Game1.getCharacterFromName("Marnie");
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11507");
          if (random.NextDouble() < 0.0001)
          {
            text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11508");
            break;
          }
          break;
        case "Pierre":
          this.portraitPerson = Game1.getCharacterFromName("Pierre");
          switch (Game1.dayOfMonth % 7)
          {
            case 0:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11487");
              break;
            case 1:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11481");
              break;
            case 2:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11482");
              break;
            case 3:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11483");
              break;
            case 4:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11484");
              break;
            case 5:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11485");
              break;
            case 6:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11486");
              break;
          }
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11488") + text;
          if (Game1.dayOfMonth == 28)
          {
            text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11489");
            break;
          }
          break;
        case "Robin":
          this.portraitPerson = Game1.getCharacterFromName("Robin");
          switch (Game1.random.Next(5))
          {
            case 0:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11460");
              break;
            case 1:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11461");
              break;
            case 2:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11462");
              break;
            case 3:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11463");
              break;
            case 4:
              string displayName = this.itemPriceAndStock.ElementAt<KeyValuePair<ISalable, int[]>>(Game1.random.Next(2, this.itemPriceAndStock.Count)).Key.DisplayName;
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11464", (object) displayName, (object) Lexicon.getRandomPositiveAdjectiveForEventOrPerson(), (object) Lexicon.getProperArticleForWord(displayName));
              break;
          }
          break;
        case "Sandy":
          this.portraitPerson = Game1.getCharacterFromName("Sandy");
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11524");
          if (random.NextDouble() < 0.0001)
          {
            text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11525");
            break;
          }
          break;
        case "Traveler":
          switch (random.Next(5))
          {
            case 0:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11499");
              break;
            case 1:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11500");
              break;
            case 2:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11501");
              break;
            case 3:
              text = this.itemPriceAndStock.Count <= 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11504") : Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11502", (object) this.itemPriceAndStock.ElementAt<KeyValuePair<ISalable, int[]>>(random.Next(this.itemPriceAndStock.Count)).Key.DisplayName);
              break;
            case 4:
              text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11504");
              break;
          }
          break;
        case "TravelerNightMarket":
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.travelernightmarket");
          break;
        case "VolcanoShop":
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:VolcanoShop" + random.Next(4).ToString());
          if (random.NextDouble() < 0.1)
          {
            text = Game1.content.LoadString("Strings\\StringsFromCSFiles:VolcanoShop4");
            break;
          }
          break;
        case "Willy":
          this.portraitPerson = Game1.getCharacterFromName("Willy");
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11477");
          if (Game1.random.NextDouble() < 0.05)
          {
            text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11478");
            break;
          }
          break;
        case "boxOffice":
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:MovieTheaterBoxOffice");
          break;
        case "magicBoatShop":
          text = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.magicBoat");
          break;
      }
      this.potraitPersonDialogue = Game1.parseText(text, Game1.dialogueFont, 304);
    }

    public bool highlightItemToSell(Item i)
    {
      if (this.heldItem != null)
        return this.heldItem.canStackWith((ISalable) i);
      return this.categoriesToSellHere.Contains(i.Category);
    }

    public static int getPlayerCurrencyAmount(Farmer who, int currencyType)
    {
      switch (currencyType)
      {
        case 0:
          return who.Money;
        case 1:
          return who.festivalScore;
        case 2:
          return who.clubCoins;
        case 4:
          return who.QiGems;
        default:
          return 0;
      }
    }

    public override void leftClickHeld(int x, int y)
    {
      base.leftClickHeld(x, y);
      if (!this.scrolling)
        return;
      int y1 = this.scrollBar.bounds.Y;
      this.scrollBar.bounds.Y = Math.Min(this.yPositionOnScreen + this.height - 64 - 12 - this.scrollBar.bounds.Height, Math.Max(y, this.yPositionOnScreen + this.upArrow.bounds.Height + 20));
      float num = (float) (y - this.scrollBarRunner.Y) / (float) this.scrollBarRunner.Height;
      this.currentItemIndex = Math.Min(Math.Max(0, this.forSale.Count - 4), Math.Max(0, (int) ((double) this.forSale.Count * (double) num)));
      this.setScrollBarToCurrentIndex();
      this.updateSaleButtonNeighbors();
      int y2 = this.scrollBar.bounds.Y;
      if (y1 == y2)
        return;
      Game1.playSound("shiny4");
    }

    public override void releaseLeftClick(int x, int y)
    {
      base.releaseLeftClick(x, y);
      this.scrolling = false;
    }

    private void setScrollBarToCurrentIndex()
    {
      if (this.forSale.Count <= 0)
        return;
      this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, this.forSale.Count - 4 + 1) * this.currentItemIndex + this.upArrow.bounds.Bottom + 4;
      if (this.currentItemIndex != this.forSale.Count - 4)
        return;
      this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 4;
    }

    public override void receiveScrollWheelAction(int direction)
    {
      base.receiveScrollWheelAction(direction);
      if (direction > 0 && this.currentItemIndex > 0)
      {
        this.upArrowPressed();
        Game1.playSound("shiny4");
      }
      else
      {
        if (direction >= 0 || this.currentItemIndex >= Math.Max(0, this.forSale.Count - 4))
          return;
        this.downArrowPressed();
        Game1.playSound("shiny4");
      }
    }

    private void downArrowPressed()
    {
      this.downArrow.scale = this.downArrow.baseScale;
      ++this.currentItemIndex;
      this.setScrollBarToCurrentIndex();
      this.updateSaleButtonNeighbors();
    }

    private void upArrowPressed()
    {
      this.upArrow.scale = this.upArrow.baseScale;
      --this.currentItemIndex;
      this.setScrollBarToCurrentIndex();
      this.updateSaleButtonNeighbors();
    }

    public override void receiveKeyPress(Keys key)
    {
      if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.heldItem != null && this.heldItem is Item)
      {
        Item heldItem = this.heldItem as Item;
        this.heldItem = (ISalable) null;
        if (Utility.CollectOrDrop(heldItem))
          Game1.playSound("stoneStep");
        else
          Game1.playSound("throwDownITem");
      }
      else
        base.receiveKeyPress(key);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y);
      if (Game1.activeClickableMenu == null)
        return;
      Vector2 clickableComponent = this.inventory.snapToClickableComponent(x, y);
      if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.forSale.Count - 4))
      {
        this.downArrowPressed();
        Game1.playSound("shwip");
      }
      else if (this.upArrow.containsPoint(x, y) && this.currentItemIndex > 0)
      {
        this.upArrowPressed();
        Game1.playSound("shwip");
      }
      else if (this.scrollBar.containsPoint(x, y))
        this.scrolling = true;
      else if (!this.downArrow.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && x < this.xPositionOnScreen + this.width + 128 && y > this.yPositionOnScreen && y < this.yPositionOnScreen + this.height)
      {
        this.scrolling = true;
        this.leftClickHeld(x, y);
        this.releaseLeftClick(x, y);
      }
      for (int index = 0; index < this.tabButtons.Count; ++index)
      {
        if (this.tabButtons[index].containsPoint(x, y))
          this.switchTab(index);
      }
      this.currentItemIndex = Math.Max(0, Math.Min(this.forSale.Count - 4, this.currentItemIndex));
      if (this.heldItem == null && !this.readOnly)
      {
        Item sold_item = this.inventory.leftClick(x, y, (Item) null, false);
        if (sold_item != null)
        {
          Item obj;
          if (this.onSell != null)
          {
            if (this.onSell((ISalable) sold_item))
              obj = (Item) null;
          }
          else
          {
            int sell_unit_price = sold_item is StardewValley.Object ? (int) ((double) (sold_item as StardewValley.Object).sellToStorePrice() * (double) this.sellPercentage) : (int) ((double) (sold_item.salePrice() / 2) * (double) this.sellPercentage);
            ShopMenu.chargePlayer(Game1.player, this.currency, -sell_unit_price * sold_item.Stack);
            int num = sold_item.Stack / 8 + 2;
            for (int index = 0; index < num; ++index)
            {
              this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16, 64, 16, 16), 9999f, 1, 999, clickableComponent + new Vector2(32f, 32f), false, false)
              {
                alphaFade = 0.025f,
                motion = new Vector2((float) Game1.random.Next(-3, 4), -4f),
                acceleration = new Vector2(0.0f, 0.5f),
                delayBeforeAnimationStart = index * 25,
                scale = 2f
              });
              this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16, 64, 16, 16), 9999f, 1, 999, clickableComponent + new Vector2(32f, 32f), false, false)
              {
                scale = 4f,
                alphaFade = 0.025f,
                delayBeforeAnimationStart = index * 50,
                motion = Utility.getVelocityTowardPoint(new Point((int) clickableComponent.X + 32, (int) clickableComponent.Y + 32), new Vector2((float) (this.xPositionOnScreen - 36), (float) (this.yPositionOnScreen + this.height - this.inventory.height - 16)), 8f),
                acceleration = Utility.getVelocityTowardPoint(new Point((int) clickableComponent.X + 32, (int) clickableComponent.Y + 32), new Vector2((float) (this.xPositionOnScreen - 36), (float) (this.yPositionOnScreen + this.height - this.inventory.height - 16)), 0.5f)
              });
            }
            ISalable key = (ISalable) null;
            if (this.CanBuyback())
              key = this.AddBuybackItem((ISalable) sold_item, sell_unit_price, sold_item.Stack);
            if (sold_item is StardewValley.Object && (int) (NetFieldBase<int, NetInt>) (sold_item as StardewValley.Object).edibility != -300)
            {
              Item one = sold_item.getOne();
              one.Stack = sold_item.Stack;
              if (key != null && this.buyBackItemsToResellTomorrow.ContainsKey(key))
                this.buyBackItemsToResellTomorrow[key].Stack += sold_item.Stack;
              else if (Game1.currentLocation is ShopLocation)
              {
                if (key != null)
                  this.buyBackItemsToResellTomorrow[key] = (ISalable) one;
                (Game1.currentLocation as ShopLocation).itemsToStartSellingTomorrow.Add(one);
              }
            }
            obj = (Item) null;
            Game1.playSound("sell");
            Game1.playSound("purchase");
            if (this.inventory.getItemAt(x, y) == null)
              this.animations.Add(new TemporaryAnimatedSprite(5, clickableComponent + new Vector2(32f, 32f), Color.White)
              {
                motion = new Vector2(0.0f, -0.5f)
              });
          }
          this.updateSaleButtonNeighbors();
        }
      }
      else
        this.heldItem = (ISalable) this.inventory.leftClick(x, y, this.heldItem as Item);
      for (int index = 0; index < this.forSaleButtons.Count; ++index)
      {
        if (this.currentItemIndex + index < this.forSale.Count && this.forSaleButtons[index].containsPoint(x, y))
        {
          int num = this.currentItemIndex + index;
          if (this.forSale[num] != null)
          {
            int val1 = Game1.oldKBState.IsKeyDown(Keys.LeftShift) ? Math.Min(Math.Min(Game1.oldKBState.IsKeyDown(Keys.LeftControl) ? 25 : 5, ShopMenu.getPlayerCurrencyAmount(Game1.player, this.currency) / Math.Max(1, this.itemPriceAndStock[this.forSale[num]][0])), Math.Max(1, this.itemPriceAndStock[this.forSale[num]][1])) : 1;
            if (this.storeContext == "ReturnedDonations")
              val1 = this.itemPriceAndStock[this.forSale[num]][1];
            int numberToBuy = Math.Min(val1, this.forSale[num].maximumStackSize());
            if (numberToBuy == -1)
              numberToBuy = 1;
            if (this.canPurchaseCheck != null && !this.canPurchaseCheck(num))
              return;
            if (numberToBuy > 0 && this.tryToPurchaseItem(this.forSale[num], this.heldItem, numberToBuy, x, y, num))
            {
              this.itemPriceAndStock.Remove(this.forSale[num]);
              this.forSale.RemoveAt(num);
            }
            else if (numberToBuy <= 0)
            {
              if (this.itemPriceAndStock[this.forSale[num]].Length != 0 && this.itemPriceAndStock[this.forSale[num]][0] > 0)
                Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
              Game1.playSound("cancel");
            }
            if (this.heldItem != null && (this._isStorageShop || Game1.options.SnappyMenus || Game1.oldKBState.IsKeyDown(Keys.LeftShift) && this.heldItem.maximumStackSize() == 1) && Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu && Game1.player.addItemToInventoryBool(this.heldItem as Item))
            {
              this.heldItem = (ISalable) null;
              DelayedAction.playSoundAfterDelay("coin", 100);
            }
          }
          this.currentItemIndex = Math.Max(0, Math.Min(this.forSale.Count - 4, this.currentItemIndex));
          this.updateSaleButtonNeighbors();
          this.setScrollBarToCurrentIndex();
          return;
        }
      }
      if (!this.readyToClose() || x >= this.xPositionOnScreen - 64 && y >= this.yPositionOnScreen - 64 && x <= this.xPositionOnScreen + this.width + 128 && y <= this.yPositionOnScreen + this.height + 64)
        return;
      this.exitThisMenu();
    }

    public virtual bool CanBuyback() => true;

    public virtual void BuyBuybackItem(ISalable bought_item, int price, int stack)
    {
      Game1.player.totalMoneyEarned -= (uint) price;
      if (Game1.player.useSeparateWallets)
        Game1.player.stats.IndividualMoneyEarned -= (uint) price;
      if (!this.buyBackItemsToResellTomorrow.ContainsKey(bought_item))
        return;
      ISalable salable = this.buyBackItemsToResellTomorrow[bought_item];
      salable.Stack -= stack;
      if (salable.Stack > 0)
        return;
      this.buyBackItemsToResellTomorrow.Remove(bought_item);
      (Game1.currentLocation as ShopLocation).itemsToStartSellingTomorrow.Remove(salable as Item);
    }

    public virtual ISalable AddBuybackItem(
      ISalable sold_item,
      int sell_unit_price,
      int stack)
    {
      ISalable key = (ISalable) null;
      while (stack > 0)
      {
        key = (ISalable) null;
        foreach (ISalable buyBackItem in this.buyBackItems)
        {
          if (buyBackItem.canStackWith(sold_item) && buyBackItem.Stack < buyBackItem.maximumStackSize())
          {
            key = buyBackItem;
            break;
          }
        }
        if (key == null)
        {
          key = sold_item.GetSalableInstance();
          int num = Math.Min(stack, key.maximumStackSize());
          this.buyBackItems.Add(key);
          this.itemPriceAndStock.Add(key, new int[2]
          {
            sell_unit_price,
            num
          });
          key.Stack = num;
          stack -= num;
        }
        else
        {
          int num = Math.Min(stack, key.maximumStackSize() - key.Stack);
          int[] numArray = this.itemPriceAndStock[key];
          numArray[1] += num;
          this.itemPriceAndStock[key] = numArray;
          key.Stack = numArray[1];
          stack -= num;
        }
      }
      this.forSale = this.itemPriceAndStock.Keys.ToList<ISalable>();
      return key;
    }

    public override bool IsAutomaticSnapValid(
      int direction,
      ClickableComponent a,
      ClickableComponent b)
    {
      return (direction != 1 || !((IEnumerable<ClickableComponent>) this.tabButtons).Contains<ClickableComponent>(a) || !((IEnumerable<ClickableComponent>) this.tabButtons).Contains<ClickableComponent>(b)) && base.IsAutomaticSnapValid(direction, a, b);
    }

    public virtual void switchTab(int new_tab)
    {
      this.currentTab = new_tab;
      Game1.playSound("shwip");
      this.applyTab();
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
        return;
      this.snapCursorToCurrentSnappedComponent();
    }

    public virtual void applyTab()
    {
      if (this.storeContext == "Dresser")
      {
        if (this.currentTab == 0)
        {
          this.forSale = this.itemPriceAndStock.Keys.ToList<ISalable>();
        }
        else
        {
          this.forSale.Clear();
          foreach (ISalable key in this.itemPriceAndStock.Keys)
          {
            if (key is Item obj)
            {
              if (this.currentTab == 1)
              {
                if (obj.Category == -95)
                  this.forSale.Add((ISalable) obj);
              }
              else if (this.currentTab == 2)
              {
                if (obj is Clothing && (obj as Clothing).clothesType.Value == 0)
                  this.forSale.Add((ISalable) obj);
              }
              else if (this.currentTab == 3)
              {
                if (obj is Clothing && (obj as Clothing).clothesType.Value == 1)
                  this.forSale.Add((ISalable) obj);
              }
              else if (this.currentTab == 4)
              {
                if (obj.Category == -97)
                  this.forSale.Add((ISalable) obj);
              }
              else if (this.currentTab == 5 && obj.Category == -96)
                this.forSale.Add((ISalable) obj);
            }
          }
        }
      }
      else if (this.storeContext == "Catalogue")
      {
        if (this.currentTab == 0)
        {
          this.forSale = this.itemPriceAndStock.Keys.ToList<ISalable>();
        }
        else
        {
          this.forSale.Clear();
          foreach (ISalable key in this.itemPriceAndStock.Keys)
          {
            if (key is Item obj)
            {
              if (this.currentTab == 1)
              {
                if (obj is Wallpaper && (obj as Wallpaper).isFloor.Value)
                  this.forSale.Add((ISalable) obj);
              }
              else if (this.currentTab == 2 && obj is Wallpaper && !(obj as Wallpaper).isFloor.Value)
                this.forSale.Add((ISalable) obj);
            }
          }
        }
      }
      else if (this.storeContext == "Furniture Catalogue")
      {
        if (this.currentTab == 0)
        {
          this.forSale = this.itemPriceAndStock.Keys.ToList<ISalable>();
        }
        else
        {
          this.forSale.Clear();
          foreach (ISalable key in this.itemPriceAndStock.Keys)
          {
            if (key is Item obj)
            {
              if (this.currentTab == 1)
              {
                if (obj is Furniture && ((obj as Furniture).furniture_type.Value == 5 || (obj as Furniture).furniture_type.Value == 4 || (obj as Furniture).furniture_type.Value == 11))
                  this.forSale.Add((ISalable) obj);
              }
              else if (this.currentTab == 2)
              {
                if (obj is Furniture && ((obj as Furniture).furniture_type.Value == 0 || (obj as Furniture).furniture_type.Value == 1 || (obj as Furniture).furniture_type.Value == 2 || (obj as Furniture).furniture_type.Value == 3))
                  this.forSale.Add((ISalable) obj);
              }
              else if (this.currentTab == 3)
              {
                if (obj is Furniture && ((obj as Furniture).furniture_type.Value == 6 || (obj as Furniture).furniture_type.Value == 13))
                  this.forSale.Add((ISalable) obj);
              }
              else if (this.currentTab == 4)
              {
                if (obj is Furniture && (obj as Furniture).furniture_type.Value == 12)
                  this.forSale.Add((ISalable) obj);
              }
              else if (this.currentTab == 5 && obj is Furniture && ((obj as Furniture).furniture_type.Value == 7 || (obj as Furniture).furniture_type.Value == 17 || (obj as Furniture).furniture_type.Value == 10 || (obj as Furniture).furniture_type.Value == 8 || (obj as Furniture).furniture_type.Value == 9 || (obj as Furniture).furniture_type.Value == 14))
                this.forSale.Add((ISalable) obj);
            }
          }
        }
      }
      this.currentItemIndex = 0;
      this.setScrollBarToCurrentIndex();
      this.updateSaleButtonNeighbors();
    }

    public override bool readyToClose() => this.heldItem == null && this.animations.Count == 0;

    public override void emergencyShutDown()
    {
      base.emergencyShutDown();
      if (this.heldItem == null)
        return;
      Game1.player.addItemToInventoryBool(this.heldItem as Item);
      Game1.playSound("coin");
    }

    public static void chargePlayer(Farmer who, int currencyType, int amount)
    {
      switch (currencyType)
      {
        case 0:
          who.Money -= amount;
          break;
        case 1:
          who.festivalScore -= amount;
          break;
        case 2:
          who.clubCoins -= amount;
          break;
        case 4:
          who.QiGems -= amount;
          break;
      }
    }

    private bool tryToPurchaseItem(
      ISalable item,
      ISalable held_item,
      int numberToBuy,
      int x,
      int y,
      int indexInForSaleList)
    {
      if (this.readOnly)
        return false;
      if (held_item == null)
      {
        if (this.itemPriceAndStock[item][1] == 0)
        {
          this.hoveredItem = (ISalable) null;
          return true;
        }
        if (item.GetSalableInstance().maximumStackSize() < numberToBuy)
          numberToBuy = Math.Max(1, item.GetSalableInstance().maximumStackSize());
        int num1 = this.itemPriceAndStock[item][0] * numberToBuy;
        int num2 = -1;
        int num3 = 5;
        if (this.itemPriceAndStock[item].Length > 2)
        {
          num2 = this.itemPriceAndStock[item][2];
          if (this.itemPriceAndStock[item].Length > 3)
            num3 = this.itemPriceAndStock[item][3];
          num3 *= numberToBuy;
        }
        if (ShopMenu.getPlayerCurrencyAmount(Game1.player, this.currency) >= num1 && (num2 == -1 || Game1.player.hasItemInInventory(num2, num3)))
        {
          this.heldItem = item.GetSalableInstance();
          this.heldItem.Stack = numberToBuy;
          if (this.storeContext == "QiGemShop" || this.storeContext == "StardewFair")
            this.heldItem.Stack *= item.Stack;
          else if (this.itemPriceAndStock[item][1] == int.MaxValue && item.Stack != int.MaxValue)
            this.heldItem.Stack *= item.Stack;
          if (!this.heldItem.CanBuyItem(Game1.player) && !item.IsInfiniteStock() && (!(this.heldItem is StardewValley.Object) || !(bool) (NetFieldBase<bool, NetBool>) (this.heldItem as StardewValley.Object).isRecipe))
          {
            Game1.playSound("smallSelect");
            this.heldItem = (ISalable) null;
            return false;
          }
          if (this.itemPriceAndStock[item][1] != int.MaxValue && !item.IsInfiniteStock())
          {
            this.itemPriceAndStock[item][1] -= numberToBuy;
            this.forSale[indexInForSaleList].Stack -= numberToBuy;
          }
          if (this.CanBuyback() && this.buyBackItems.Contains(item))
            this.BuyBuybackItem(item, num1, numberToBuy);
          ShopMenu.chargePlayer(Game1.player, this.currency, num1);
          if (num2 != -1)
            Game1.player.removeItemsFromInventory(num2, num3);
          if (!this._isStorageShop && item.actionWhenPurchased())
          {
            if (this.heldItem is StardewValley.Object && (bool) (NetFieldBase<bool, NetBool>) (this.heldItem as StardewValley.Object).isRecipe)
            {
              string key = this.heldItem.Name.Substring(0, this.heldItem.Name.IndexOf("Recipe") - 1);
              try
              {
                if ((this.heldItem as StardewValley.Object).Category == -7)
                  Game1.player.cookingRecipes.Add(key, 0);
                else
                  Game1.player.craftingRecipes.Add(key, 0);
                Game1.playSound("newRecipe");
              }
              catch (Exception ex)
              {
              }
            }
            held_item = (ISalable) null;
            this.heldItem = (ISalable) null;
          }
          else
          {
            if (this.heldItem != null && this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).ParentSheetIndex == 858)
            {
              Game1.player.team.addQiGemsToTeam.Fire(this.heldItem.Stack);
              this.heldItem = (ISalable) null;
            }
            if (Game1.mouseClickPolling > 300)
            {
              if (this.purchaseRepeatSound != null)
                Game1.playSound(this.purchaseRepeatSound);
            }
            else if (this.purchaseSound != null)
              Game1.playSound(this.purchaseSound);
          }
          if (this.onPurchase != null && this.onPurchase(item, Game1.player, numberToBuy))
            this.exitThisMenu();
        }
        else
        {
          if (num1 > 0)
            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
          Game1.playSound("cancel");
        }
      }
      else if (held_item.canStackWith(item))
      {
        numberToBuy = Math.Min(numberToBuy, held_item.maximumStackSize() - held_item.Stack);
        if (numberToBuy > 0)
        {
          int num4 = this.itemPriceAndStock[item][0] * numberToBuy;
          int num5 = -1;
          int num6 = 5;
          if (this.itemPriceAndStock[item].Length > 2)
          {
            num5 = this.itemPriceAndStock[item][2];
            if (this.itemPriceAndStock[item].Length > 3)
              num6 = this.itemPriceAndStock[item][3];
            num6 *= numberToBuy;
          }
          int stack1 = item.Stack;
          item.Stack = numberToBuy + this.heldItem.Stack;
          if (!item.CanBuyItem(Game1.player))
          {
            item.Stack = stack1;
            Game1.playSound("cancel");
            return false;
          }
          item.Stack = stack1;
          if (ShopMenu.getPlayerCurrencyAmount(Game1.player, this.currency) >= num4 && (num5 == -1 || Game1.player.hasItemInInventory(num5, num6)))
          {
            int stack2 = numberToBuy;
            if (this.itemPriceAndStock[item][1] == int.MaxValue && item.Stack != int.MaxValue)
              stack2 *= item.Stack;
            this.heldItem.Stack += stack2;
            if (this.itemPriceAndStock[item][1] != int.MaxValue && !item.IsInfiniteStock())
            {
              this.itemPriceAndStock[item][1] -= numberToBuy;
              this.forSale[indexInForSaleList].Stack -= numberToBuy;
            }
            if (this.CanBuyback() && this.buyBackItems.Contains(item))
              this.BuyBuybackItem(item, num4, stack2);
            ShopMenu.chargePlayer(Game1.player, this.currency, num4);
            if (Game1.mouseClickPolling > 300)
            {
              if (this.purchaseRepeatSound != null)
                Game1.playSound(this.purchaseRepeatSound);
            }
            else if (this.purchaseSound != null)
              Game1.playSound(this.purchaseSound);
            if (num5 != -1)
              Game1.player.removeItemsFromInventory(num5, num6);
            if (!this._isStorageShop && item.actionWhenPurchased())
              this.heldItem = (ISalable) null;
            if (this.onPurchase != null && this.onPurchase(item, Game1.player, numberToBuy))
              this.exitThisMenu();
          }
          else
          {
            if (num4 > 0)
              Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
            Game1.playSound("cancel");
          }
        }
      }
      if (this.itemPriceAndStock[item][1] > 0)
        return false;
      if (this.buyBackItems.Contains(item))
        this.buyBackItems.Remove(item);
      this.hoveredItem = (ISalable) null;
      return true;
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      Vector2 clickableComponent = this.inventory.snapToClickableComponent(x, y);
      if (this.heldItem == null && !this.readOnly)
      {
        ISalable sold_item = (ISalable) this.inventory.rightClick(x, y, (Item) null, false);
        ISalable salable1;
        if (sold_item != null)
        {
          if (this.onSell != null)
          {
            if (this.onSell(sold_item))
              salable1 = (ISalable) null;
          }
          else
          {
            int sell_unit_price = sold_item is StardewValley.Object ? (int) ((double) (sold_item as StardewValley.Object).sellToStorePrice() * (double) this.sellPercentage) : (int) ((double) (sold_item.salePrice() / 2) * (double) this.sellPercentage);
            int stack = sold_item.Stack;
            ISalable salable2 = sold_item;
            ShopMenu.chargePlayer(Game1.player, this.currency, -sell_unit_price * stack);
            ISalable key = (ISalable) null;
            if (this.CanBuyback())
              key = this.AddBuybackItem(sold_item, sell_unit_price, stack);
            salable1 = (ISalable) null;
            if (Game1.mouseClickPolling > 300)
            {
              if (this.purchaseRepeatSound != null)
                Game1.playSound(this.purchaseRepeatSound);
            }
            else if (this.purchaseSound != null)
              Game1.playSound(this.purchaseSound);
            int num = 2;
            for (int index = 0; index < num; ++index)
            {
              this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16, 64, 16, 16), 9999f, 1, 999, clickableComponent + new Vector2(32f, 32f), false, false)
              {
                alphaFade = 0.025f,
                motion = new Vector2((float) Game1.random.Next(-3, 4), -4f),
                acceleration = new Vector2(0.0f, 0.5f),
                delayBeforeAnimationStart = index * 25,
                scale = 2f
              });
              this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16, 64, 16, 16), 9999f, 1, 999, clickableComponent + new Vector2(32f, 32f), false, false)
              {
                scale = 4f,
                alphaFade = 0.025f,
                delayBeforeAnimationStart = index * 50,
                motion = Utility.getVelocityTowardPoint(new Point((int) clickableComponent.X + 32, (int) clickableComponent.Y + 32), new Vector2((float) (this.xPositionOnScreen - 36), (float) (this.yPositionOnScreen + this.height - this.inventory.height - 16)), 8f),
                acceleration = Utility.getVelocityTowardPoint(new Point((int) clickableComponent.X + 32, (int) clickableComponent.Y + 32), new Vector2((float) (this.xPositionOnScreen - 36), (float) (this.yPositionOnScreen + this.height - this.inventory.height - 16)), 0.5f)
              });
            }
            if (key != null && this.buyBackItemsToResellTomorrow.ContainsKey(key))
              this.buyBackItemsToResellTomorrow[key].Stack += stack;
            else if (salable2 is StardewValley.Object && (int) (NetFieldBase<int, NetInt>) (salable2 as StardewValley.Object).edibility != -300 && Game1.random.NextDouble() < 0.0399999991059303 && Game1.currentLocation is ShopLocation)
            {
              ISalable salableInstance = salable2.GetSalableInstance();
              if (key != null)
                this.buyBackItemsToResellTomorrow[key] = salableInstance;
              (Game1.currentLocation as ShopLocation).itemsToStartSellingTomorrow.Add(salableInstance as Item);
            }
            if (this.inventory.getItemAt(x, y) == null)
            {
              Game1.playSound("sell");
              this.animations.Add(new TemporaryAnimatedSprite(5, clickableComponent + new Vector2(32f, 32f), Color.White)
              {
                motion = new Vector2(0.0f, -0.5f)
              });
            }
          }
        }
      }
      else
        this.heldItem = (ISalable) this.inventory.rightClick(x, y, this.heldItem as Item);
      for (int index = 0; index < this.forSaleButtons.Count; ++index)
      {
        if (this.currentItemIndex + index < this.forSale.Count && this.forSaleButtons[index].containsPoint(x, y))
        {
          int num = this.currentItemIndex + index;
          if (this.forSale[num] == null)
            break;
          int numberToBuy = 1;
          if (this.itemPriceAndStock[this.forSale[num]][0] > 0)
            numberToBuy = Game1.oldKBState.IsKeyDown(Keys.LeftShift) ? Math.Min(Math.Min(Game1.oldKBState.IsKeyDown(Keys.LeftControl) ? 25 : 5, ShopMenu.getPlayerCurrencyAmount(Game1.player, this.currency) / this.itemPriceAndStock[this.forSale[num]][0]), this.itemPriceAndStock[this.forSale[num]][1]) : 1;
          if (this.canPurchaseCheck != null && !this.canPurchaseCheck(num))
            break;
          if (numberToBuy > 0 && this.tryToPurchaseItem(this.forSale[num], this.heldItem, numberToBuy, x, y, num))
          {
            this.itemPriceAndStock.Remove(this.forSale[num]);
            this.forSale.RemoveAt(num);
          }
          if (this.heldItem != null && (this._isStorageShop || Game1.options.SnappyMenus) && Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu && Game1.player.addItemToInventoryBool(this.heldItem as Item))
          {
            this.heldItem = (ISalable) null;
            DelayedAction.playSoundAfterDelay("coin", 100);
          }
          this.setScrollBarToCurrentIndex();
          break;
        }
      }
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      this.descriptionText = "";
      this.hoverText = "";
      this.hoveredItem = (ISalable) null;
      this.hoverPrice = -1;
      this.boldTitleText = "";
      this.upArrow.tryHover(x, y);
      this.downArrow.tryHover(x, y);
      this.scrollBar.tryHover(x, y);
      if (this.scrolling)
        return;
      for (int index = 0; index < this.forSaleButtons.Count; ++index)
      {
        if (this.currentItemIndex + index < this.forSale.Count && this.forSaleButtons[index].containsPoint(x, y))
        {
          ISalable key = this.forSale[this.currentItemIndex + index];
          if (this.canPurchaseCheck == null || this.canPurchaseCheck(this.currentItemIndex + index))
          {
            this.hoverText = key.getDescription();
            this.boldTitleText = key.DisplayName;
            if (!this._isStorageShop)
              this.hoverPrice = this.itemPriceAndStock == null || !this.itemPriceAndStock.ContainsKey(key) ? key.salePrice() : this.itemPriceAndStock[key][0];
            this.hoveredItem = key;
            this.forSaleButtons[index].scale = Math.Min(this.forSaleButtons[index].scale + 0.03f, 1.1f);
          }
        }
        else
          this.forSaleButtons[index].scale = Math.Max(1f, this.forSaleButtons[index].scale - 0.03f);
      }
      if (this.heldItem != null)
        return;
      foreach (ClickableComponent c in this.inventory.inventory)
      {
        if (c.containsPoint(x, y))
        {
          Item clickableComponent = this.inventory.getItemFromClickableComponent(c);
          if (clickableComponent != null && (this.inventory.highlightMethod == null || this.inventory.highlightMethod(clickableComponent)))
          {
            if (this._isStorageShop)
            {
              this.hoverText = clickableComponent.getDescription();
              this.boldTitleText = clickableComponent.DisplayName;
              this.hoveredItem = (ISalable) clickableComponent;
            }
            else
            {
              this.hoverText = clickableComponent.DisplayName + " x" + clickableComponent.Stack.ToString();
              if (clickableComponent is StardewValley.Object @object && @object.needsToBeDonated())
                this.hoverText = this.hoverText + "\n\n" + clickableComponent.getDescription() + "\n";
              this.hoverPrice = (clickableComponent is StardewValley.Object ? (int) ((double) (clickableComponent as StardewValley.Object).sellToStorePrice() * (double) this.sellPercentage) : (int) ((double) (clickableComponent.salePrice() / 2) * (double) this.sellPercentage)) * clickableComponent.Stack;
            }
          }
        }
      }
    }

    public override void update(GameTime time)
    {
      base.update(time);
      if (this.poof != null && this.poof.update(time))
        this.poof = (TemporaryAnimatedSprite) null;
      this.repositionTabs();
    }

    public void drawCurrency(SpriteBatch b)
    {
      if (this._isStorageShop)
        return;
      if (this.currency == 0)
        Game1.dayTimeMoneyBox.drawMoneyBox(b, this.xPositionOnScreen - 36, this.yPositionOnScreen + this.height - this.inventory.height - 12);
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      if (b != Buttons.RightTrigger && b != Buttons.LeftTrigger)
        return;
      if (this.currentlySnappedComponent != null && this.currentlySnappedComponent.myID >= 3546)
      {
        int num = -1;
        for (int index = 0; index < 12; ++index)
        {
          this.inventory.inventory[index].upNeighborID = 3546 + this.forSaleButtons.Count - 1;
          if (num == -1 && this.heldItem != null && this.inventory.actualInventory != null && this.inventory.actualInventory.Count > index && this.inventory.actualInventory[index] == null)
            num = index;
        }
        this.currentlySnappedComponent = this.getComponentWithID(num != -1 ? num : 0);
        this.snapCursorToCurrentSnappedComponent();
      }
      else
        this.snapToDefaultClickableComponent();
      Game1.playSound("shiny4");
    }

    private int getHoveredItemExtraItemIndex() => this.itemPriceAndStock != null && this.hoveredItem != null && this.itemPriceAndStock.ContainsKey(this.hoveredItem) && this.itemPriceAndStock[this.hoveredItem].Length > 2 ? this.itemPriceAndStock[this.hoveredItem][2] : -1;

    private int getHoveredItemExtraItemAmount() => this.itemPriceAndStock != null && this.hoveredItem != null && this.itemPriceAndStock.ContainsKey(this.hoveredItem) && this.itemPriceAndStock[this.hoveredItem].Length > 3 ? this.itemPriceAndStock[this.hoveredItem][3] : 5;

    public void updatePosition()
    {
      this.width = 1000 + IClickableMenu.borderWidth * 2;
      this.height = 600 + IClickableMenu.borderWidth * 2;
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
      int num = this.xPositionOnScreen - 320;
      bool flag = false;
      if (this.portraitPerson != null)
        flag = true;
      if (this.potraitPersonDialogue != null && this.potraitPersonDialogue != "")
        flag = true;
      if (((num <= 0 ? 0 : (Game1.options.showMerchantPortraits ? 1 : 0)) & (flag ? 1 : 0)) != 0)
        return;
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - (1000 + IClickableMenu.borderWidth * 2) / 2;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
    }

    protected override void cleanupBeforeExit()
    {
      if (this.currency == 4)
        Game1.specialCurrencyDisplay.ShowCurrency((string) null);
      base.cleanupBeforeExit();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      this.updatePosition();
      this.initializeUpperRightCloseButton();
      Game1.player.forceCanMove();
      this.inventory = new InventoryMenu(this.xPositionOnScreen + this.width, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 320 + 40, false, highlightMethod: new InventoryMenu.highlightThisItem(this.highlightItemToSell))
      {
        showGrayedOutSlots = true
      };
      this.inventory.movePosition(-this.inventory.width - 32, 0);
      this.upArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16, this.yPositionOnScreen + 16, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
      this.downArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16, this.yPositionOnScreen + this.height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
      this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upArrow.bounds.X + 12, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
      this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, this.scrollBar.bounds.Width, this.height - 64 - this.upArrow.bounds.Height - 28);
      this.forSaleButtons.Clear();
      for (int index = 0; index < 4; ++index)
        this.forSaleButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 16 + index * ((this.height - 256) / 4), this.width - 32, (this.height - 256) / 4 + 4), index.ToString() ?? ""));
      if (this.tabButtons.Count > 0)
      {
        foreach (ClickableComponent forSaleButton in this.forSaleButtons)
          forSaleButton.leftNeighborID = -99998;
      }
      this.repositionTabs();
      foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Top))
        clickableComponent.upNeighborID = -99998;
    }

    public void setItemPriceAndStock(Dictionary<ISalable, int[]> new_stock)
    {
      this.itemPriceAndStock = new_stock;
      this.forSale = this.itemPriceAndStock.Keys.ToList<ISalable>();
      this.applyTab();
    }

    public override void draw(SpriteBatch b)
    {
      if (!Game1.options.showMenuBackground)
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
      Texture2D texture = Game1.mouseCursors;
      Rectangle sourceRect1 = new Rectangle(384, 373, 18, 18);
      Rectangle sourceRect2 = new Rectangle(384, 396, 15, 15);
      int color1 = -1;
      bool flag1 = true;
      Rectangle rectangle = new Rectangle(296, 363, 18, 18);
      Color color2 = Color.Wheat;
      if (this.storeContext == "QiGemShop")
      {
        texture = Game1.mouseCursors2;
        sourceRect1 = new Rectangle(0, 256, 18, 18);
        sourceRect2 = new Rectangle(18, 256, 15, 15);
        color1 = 4;
        color2 = Color.Blue;
        flag1 = true;
        rectangle = new Rectangle(33, 256, 18, 18);
      }
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen + this.width - this.inventory.width - 32 - 24, this.yPositionOnScreen + this.height - 256 + 40, this.inventory.width + 56, this.height - 448 + 20, Color.White, 4f);
      IClickableMenu.drawTextureBox(b, texture, sourceRect1, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height - 256 + 32 + 4, Color.White, 4f);
      this.drawCurrency(b);
      for (int index = 0; index < this.forSaleButtons.Count; ++index)
      {
        if (this.currentItemIndex + index < this.forSale.Count)
        {
          bool flag2 = false;
          if (this.canPurchaseCheck != null && !this.canPurchaseCheck(this.currentItemIndex + index))
            flag2 = true;
          IClickableMenu.drawTextureBox(b, texture, sourceRect2, this.forSaleButtons[index].bounds.X, this.forSaleButtons[index].bounds.Y, this.forSaleButtons[index].bounds.Width, this.forSaleButtons[index].bounds.Height, !this.forSaleButtons[index].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) || this.scrolling ? Color.White : color2, 4f, false);
          ISalable key = this.forSale[this.currentItemIndex + index];
          bool flag3 = key.Stack > 1 && key.Stack != int.MaxValue && this.itemPriceAndStock[key][1] == int.MaxValue;
          StackDrawType drawStackNumber;
          if (this.storeContext == "QiGemShop")
          {
            drawStackNumber = StackDrawType.HideButShowQuality;
            flag3 = key.Stack > 1;
          }
          else if (this.itemPriceAndStock[key][1] == int.MaxValue)
          {
            drawStackNumber = StackDrawType.HideButShowQuality;
          }
          else
          {
            drawStackNumber = StackDrawType.Draw_OneInclusive;
            if (this._isStorageShop)
              drawStackNumber = StackDrawType.Draw;
          }
          string s = key.DisplayName;
          if (flag3)
            s = s + " x" + key.Stack.ToString();
          if (this.forSale[this.currentItemIndex + index].ShouldDrawIcon())
          {
            if (flag1)
              b.Draw(texture, new Vector2((float) (this.forSaleButtons[index].bounds.X + 32 - 12), (float) (this.forSaleButtons[index].bounds.Y + 24 - 4)), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            this.forSale[this.currentItemIndex + index].drawInMenu(b, new Vector2((float) (this.forSaleButtons[index].bounds.X + 32 - 8), (float) (this.forSaleButtons[index].bounds.Y + 24)), 1f, 1f, 0.9f, drawStackNumber, Color.White * (!flag2 ? 1f : 0.25f), true);
            if (this.buyBackItems.Contains(this.forSale[this.currentItemIndex + index]))
              b.Draw(Game1.mouseCursors2, new Vector2((float) (this.forSaleButtons[index].bounds.X + 32 - 8), (float) (this.forSaleButtons[index].bounds.Y + 24)), new Rectangle?(new Rectangle(64, 240, 16, 16)), Color.White * (!flag2 ? 1f : 0.25f), 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 1f);
            SpriteText.drawString(b, s, this.forSaleButtons[index].bounds.X + 96 + 8, this.forSaleButtons[index].bounds.Y + 28, alpha: (flag2 ? 0.5f : 1f), color: color1);
          }
          else
            SpriteText.drawString(b, s, this.forSaleButtons[index].bounds.X + 32 + 8, this.forSaleButtons[index].bounds.Y + 28, alpha: (flag2 ? 0.5f : 1f), color: color1);
          if (this.itemPriceAndStock[this.forSale[this.currentItemIndex + index]][0] > 0)
          {
            SpriteText.drawString(b, this.itemPriceAndStock[this.forSale[this.currentItemIndex + index]][0].ToString() + " ", this.forSaleButtons[index].bounds.Right - SpriteText.getWidthOfString(this.itemPriceAndStock[this.forSale[this.currentItemIndex + index]][0].ToString() + " ") - 60, this.forSaleButtons[index].bounds.Y + 28, alpha: (ShopMenu.getPlayerCurrencyAmount(Game1.player, this.currency) < this.itemPriceAndStock[this.forSale[this.currentItemIndex + index]][0] || flag2 ? 0.5f : 1f), color: color1);
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.forSaleButtons[index].bounds.Right - 52), (float) (this.forSaleButtons[index].bounds.Y + 40 - 4)), new Rectangle(193 + this.currency * 9, 373, 9, 10), Color.White * (!flag2 ? 1f : 0.25f), 0.0f, Vector2.Zero, 4f, layerDepth: 1f, shadowIntensity: (!flag2 ? 0.35f : 0.0f));
          }
          else if (this.itemPriceAndStock[this.forSale[this.currentItemIndex + index]].Length > 2)
          {
            int quantity = 5;
            int num = this.itemPriceAndStock[this.forSale[this.currentItemIndex + index]][2];
            if (this.itemPriceAndStock[this.forSale[this.currentItemIndex + index]].Length > 3)
              quantity = this.itemPriceAndStock[this.forSale[this.currentItemIndex + index]][3];
            bool flag4 = Game1.player.hasItemInInventory(num, quantity);
            if (this.canPurchaseCheck != null && !this.canPurchaseCheck(this.currentItemIndex + index))
              flag4 = false;
            float widthOfString = (float) SpriteText.getWidthOfString("x" + quantity.ToString());
            Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2((float) (this.forSaleButtons[index].bounds.Right - 88) - widthOfString, (float) (this.forSaleButtons[index].bounds.Y + 28 - 4)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, num, 16, 16), Color.White * (flag4 ? 1f : 0.25f), 0.0f, Vector2.Zero, shadowIntensity: (flag4 ? 0.35f : 0.0f));
            SpriteText.drawString(b, "x" + quantity.ToString(), this.forSaleButtons[index].bounds.Right - (int) widthOfString - 16, this.forSaleButtons[index].bounds.Y + 44, alpha: (flag4 ? 1f : 0.5f), color: color1);
          }
        }
      }
      if (this.forSale.Count == 0 && !this._isStorageShop)
        SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11583"), this.xPositionOnScreen + this.width / 2 - SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11583")) / 2, this.yPositionOnScreen + this.height / 2 - 128);
      this.inventory.draw(b);
      for (int index = this.animations.Count - 1; index >= 0; --index)
      {
        if (this.animations[index].update(Game1.currentGameTime))
          this.animations.RemoveAt(index);
        else
          this.animations[index].draw(b, true);
      }
      if (this.poof != null)
        this.poof.draw(b);
      this.upArrow.draw(b);
      this.downArrow.draw(b);
      for (int index = 0; index < this.tabButtons.Count; ++index)
        this.tabButtons[index].draw(b);
      if (this.forSale.Count > 4)
      {
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f);
        this.scrollBar.draw(b);
      }
      if (!this.hoverText.Equals(""))
      {
        if (this.hoveredItem is StardewValley.Object && (bool) (NetFieldBase<bool, NetBool>) (this.hoveredItem as StardewValley.Object).isRecipe)
          IClickableMenu.drawToolTip(b, " ", this.boldTitleText, this.hoveredItem as Item, this.heldItem != null, currencySymbol: this.currency, extraItemToShowIndex: this.getHoveredItemExtraItemIndex(), extraItemToShowAmount: this.getHoveredItemExtraItemAmount(), craftingIngredients: new CraftingRecipe(this.hoveredItem.Name.Replace(" Recipe", "")), moneyAmountToShowAtBottom: (this.hoverPrice > 0 ? this.hoverPrice : -1));
        else
          IClickableMenu.drawToolTip(b, this.hoverText, this.boldTitleText, this.hoveredItem as Item, this.heldItem != null, currencySymbol: this.currency, extraItemToShowIndex: this.getHoveredItemExtraItemIndex(), extraItemToShowAmount: this.getHoveredItemExtraItemAmount(), moneyAmountToShowAtBottom: (this.hoverPrice > 0 ? this.hoverPrice : -1));
      }
      if (this.heldItem != null)
        this.heldItem.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 8), (float) (Game1.getOldMouseY() + 8)), 1f, 1f, 0.9f, StackDrawType.Draw, Color.White, true);
      base.draw(b);
      int x = this.xPositionOnScreen - 320;
      if (x > 0 && Game1.options.showMerchantPortraits)
      {
        if (this.portraitPerson != null)
        {
          Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) x, (float) this.yPositionOnScreen), new Rectangle(603, 414, 74, 74), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.91f);
          if (this.portraitPerson.Portrait != null)
            b.Draw(this.portraitPerson.Portrait, new Vector2((float) (x + 20), (float) (this.yPositionOnScreen + 20)), new Rectangle?(new Rectangle(0, 0, 64, 64)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.92f);
        }
        if (this.potraitPersonDialogue != null)
        {
          int overrideX = this.xPositionOnScreen - (int) Game1.dialogueFont.MeasureString(this.potraitPersonDialogue).X - 64;
          if (overrideX > 0)
            IClickableMenu.drawHoverText(b, this.potraitPersonDialogue, Game1.dialogueFont, overrideX: overrideX, overrideY: (this.yPositionOnScreen + (this.portraitPerson != null ? 312 : 0)));
        }
      }
      this.drawMouse(b);
    }
  }
}
