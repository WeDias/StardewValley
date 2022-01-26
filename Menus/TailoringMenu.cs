// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TailoringMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.GameData.Crafting;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class TailoringMenu : MenuWithInventory
  {
    protected int _timeUntilCraft;
    public const int region_leftIngredient = 998;
    public const int region_rightIngredient = 997;
    public const int region_startButton = 996;
    public const int region_resultItem = 995;
    public ClickableTextureComponent needleSprite;
    public ClickableTextureComponent presserSprite;
    public ClickableTextureComponent craftResultDisplay;
    public Vector2 needlePosition;
    public Vector2 presserPosition;
    public Vector2 leftIngredientStartSpot;
    public Vector2 leftIngredientEndSpot;
    protected float _rightItemOffset;
    public ClickableTextureComponent leftIngredientSpot;
    public ClickableTextureComponent rightIngredientSpot;
    public ClickableTextureComponent blankLeftIngredientSpot;
    public ClickableTextureComponent blankRightIngredientSpot;
    public ClickableTextureComponent startTailoringButton;
    public const int region_shirt = 108;
    public const int region_pants = 109;
    public const int region_hat = 101;
    public List<ClickableComponent> equipmentIcons = new List<ClickableComponent>();
    public const int CRAFT_TIME = 1500;
    public Texture2D tailoringTextures;
    public List<TailorItemRecipe> _tailoringRecipes;
    private ICue _sewingSound;
    protected Dictionary<Item, bool> _highlightDictionary;
    protected Dictionary<string, Item> _lastValidEquippedItems;
    protected bool _shouldPrismaticDye;
    protected bool _heldItemIsEquipped;
    protected bool _isDyeCraft;
    protected bool _isMultipleResultCraft;
    protected string displayedDescription = "";
    protected TailoringMenu.CraftState _craftState;
    public Vector2 questionMarkOffset;

    public TailoringMenu()
      : base(okButton: true, trashCan: true, inventoryXOffset: 12, inventoryYOffset: 132)
    {
      Game1.playSound("bigSelect");
      if (this.yPositionOnScreen == IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder)
        this.movePosition(0, -IClickableMenu.spaceToClearTopBorder);
      this.inventory.highlightMethod = new InventoryMenu.highlightThisItem(this.HighlightItems);
      this.tailoringTextures = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\tailoring");
      this._tailoringRecipes = Game1.temporaryContent.Load<List<TailorItemRecipe>>("Data\\TailoringRecipes");
      this._CreateButtons();
      if (this.trashCan != null)
        this.trashCan.myID = 106;
      if (this.okButton != null)
        this.okButton.leftNeighborID = 11;
      if (Game1.options.SnappyMenus)
      {
        this.populateClickableComponentList();
        this.snapToDefaultClickableComponent();
      }
      this._ValidateCraft();
    }

    protected void _CreateButtons()
    {
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 192, 96, 96), this.tailoringTextures, new Rectangle(0, 156, 24, 24), 4f);
      textureComponent1.myID = 998;
      textureComponent1.downNeighborID = -99998;
      textureComponent1.leftNeighborID = 109;
      textureComponent1.rightNeighborID = 996;
      textureComponent1.upNeighborID = 997;
      textureComponent1.item = this.leftIngredientSpot != null ? this.leftIngredientSpot.item : (Item) null;
      this.leftIngredientSpot = textureComponent1;
      this.leftIngredientStartSpot = new Vector2((float) this.leftIngredientSpot.bounds.X, (float) this.leftIngredientSpot.bounds.Y);
      this.leftIngredientEndSpot = this.leftIngredientStartSpot + new Vector2(256f, 0.0f);
      this.needleSprite = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 116, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 128, 96, 96), this.tailoringTextures, new Rectangle(64, 80, 16, 32), 4f);
      this.presserSprite = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 116, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 128, 96, 96), this.tailoringTextures, new Rectangle(48, 80, 16, 32), 4f);
      this.needlePosition = new Vector2((float) this.needleSprite.bounds.X, (float) this.needleSprite.bounds.Y);
      this.presserPosition = new Vector2((float) this.presserSprite.bounds.X, (float) this.presserSprite.bounds.Y);
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 400, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8, 96, 96), this.tailoringTextures, new Rectangle(0, 180, 24, 24), 4f);
      textureComponent2.myID = 997;
      textureComponent2.downNeighborID = 996;
      textureComponent2.leftNeighborID = 998;
      textureComponent2.rightNeighborID = -99998;
      textureComponent2.upNeighborID = -99998;
      textureComponent2.item = this.rightIngredientSpot != null ? this.rightIngredientSpot.item : (Item) null;
      textureComponent2.fullyImmutable = true;
      this.rightIngredientSpot = textureComponent2;
      this.blankRightIngredientSpot = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 400, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8, 96, 96), this.tailoringTextures, new Rectangle(0, 128, 24, 24), 4f);
      this.blankLeftIngredientSpot = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 192, 96, 96), this.tailoringTextures, new Rectangle(0, 128, 24, 24), 4f);
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 448, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 128, 96, 96), this.tailoringTextures, new Rectangle(24, 80, 24, 24), 4f);
      textureComponent3.myID = 996;
      textureComponent3.downNeighborID = -99998;
      textureComponent3.leftNeighborID = 998;
      textureComponent3.rightNeighborID = 995;
      textureComponent3.upNeighborID = 997;
      textureComponent3.item = this.startTailoringButton != null ? this.startTailoringButton.item : (Item) null;
      textureComponent3.fullyImmutable = true;
      this.startTailoringButton = textureComponent3;
      if (this.inventory.inventory != null && this.inventory.inventory.Count >= 12)
      {
        for (int index = 0; index < 12; ++index)
        {
          if (this.inventory.inventory[index] != null)
            this.inventory.inventory[index].upNeighborID = -99998;
        }
      }
      this.equipmentIcons = new List<ClickableComponent>();
      this.equipmentIcons.Add(new ClickableComponent(new Rectangle(0, 0, 64, 64), "Hat")
      {
        myID = 101,
        leftNeighborID = -99998,
        downNeighborID = -99998,
        upNeighborID = -99998,
        rightNeighborID = -99998
      });
      this.equipmentIcons.Add(new ClickableComponent(new Rectangle(0, 0, 64, 64), "Shirt")
      {
        myID = 108,
        upNeighborID = -99998,
        downNeighborID = -99998,
        rightNeighborID = -99998,
        leftNeighborID = -99998
      });
      this.equipmentIcons.Add(new ClickableComponent(new Rectangle(0, 0, 64, 64), "Pants")
      {
        myID = 109,
        upNeighborID = -99998,
        rightNeighborID = -99998,
        leftNeighborID = -99998,
        downNeighborID = -99998
      });
      for (int index = 0; index < this.equipmentIcons.Count; ++index)
      {
        this.equipmentIcons[index].bounds.X = this.xPositionOnScreen - 64 + 9;
        this.equipmentIcons[index].bounds.Y = this.yPositionOnScreen + 192 + index * 64;
      }
      ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 660, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 232, 64, 64), this.tailoringTextures, new Rectangle(0, 208, 16, 16), 4f);
      textureComponent4.myID = 995;
      textureComponent4.downNeighborID = -99998;
      textureComponent4.leftNeighborID = 996;
      textureComponent4.upNeighborID = 997;
      textureComponent4.item = this.craftResultDisplay != null ? this.craftResultDisplay.item : (Item) null;
      this.craftResultDisplay = textureComponent4;
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public bool IsBusy() => this._timeUntilCraft > 0;

    public override bool readyToClose() => base.readyToClose() && this.heldItem == null && !this.IsBusy();

    public bool HighlightItems(Item i)
    {
      if (i == null || i != null && !this.IsValidCraftIngredient(i))
        return false;
      if (this._highlightDictionary == null)
        this.GenerateHighlightDictionary();
      if (!this._highlightDictionary.ContainsKey(i))
      {
        this._highlightDictionary = (Dictionary<Item, bool>) null;
        this.GenerateHighlightDictionary();
      }
      return this._highlightDictionary[i];
    }

    public void GenerateHighlightDictionary()
    {
      this._highlightDictionary = new Dictionary<Item, bool>();
      List<Item> objList = new List<Item>((IEnumerable<Item>) this.inventory.actualInventory);
      if (Game1.player.pantsItem.Value != null)
        objList.Add((Item) Game1.player.pantsItem.Value);
      if (Game1.player.shirtItem.Value != null)
        objList.Add((Item) Game1.player.shirtItem.Value);
      if (Game1.player.hat.Value != null)
        objList.Add((Item) Game1.player.hat.Value);
      foreach (Item obj in objList)
      {
        if (obj != null)
          this._highlightDictionary[obj] = this.leftIngredientSpot.item == null && this.rightIngredientSpot.item == null || (this.leftIngredientSpot.item == null || this.rightIngredientSpot.item == null) && (this.leftIngredientSpot.item == null ? this.IsValidCraft(obj, this.rightIngredientSpot.item) : this.IsValidCraft(this.leftIngredientSpot.item, obj));
      }
    }

    private void _leftIngredientSpotClicked()
    {
      Item obj = this.leftIngredientSpot.item;
      if (this.heldItem != null && !this.IsValidCraftIngredient(this.heldItem))
        return;
      Game1.playSound("stoneStep");
      this.leftIngredientSpot.item = this.heldItem;
      this.heldItem = obj;
      this._highlightDictionary = (Dictionary<Item, bool>) null;
      this._ValidateCraft();
    }

    public bool IsValidCraftIngredient(Item item) => item.HasContextTag("item_lucky_purple_shorts") || item.canBeTrashed();

    private void _rightIngredientSpotClicked()
    {
      Item obj = this.rightIngredientSpot.item;
      if (this.heldItem != null && !this.IsValidCraftIngredient(this.heldItem))
        return;
      Game1.playSound("stoneStep");
      this.rightIngredientSpot.item = this.heldItem;
      this.heldItem = obj;
      this._highlightDictionary = (Dictionary<Item, bool>) null;
      this._ValidateCraft();
    }

    public override void receiveKeyPress(Keys key)
    {
      if (key == Keys.Delete)
      {
        if (this.heldItem == null || !this.IsValidCraftIngredient(this.heldItem))
          return;
        Utility.trashItem(this.heldItem);
        this.heldItem = (Item) null;
      }
      else
        base.receiveKeyPress(key);
    }

    public bool IsHoldingEquippedItem()
    {
      if (this.heldItem == null)
        return false;
      return Game1.player.IsEquippedItem(this.heldItem) || Game1.player.IsEquippedItem(Utility.PerformSpecialItemGrabReplacement(this.heldItem));
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      Item heldItem = this.heldItem;
      int num = Game1.player.IsEquippedItem(heldItem) ? 1 : 0;
      base.receiveLeftClick(x, y, true);
      if (num != 0 && this.heldItem != heldItem)
      {
        if (heldItem == Game1.player.hat.Value)
        {
          Game1.player.hat.Value = (Hat) null;
          this._highlightDictionary = (Dictionary<Item, bool>) null;
        }
        else if (heldItem == Game1.player.shirtItem.Value)
        {
          Game1.player.shirtItem.Value = (Clothing) null;
          this._highlightDictionary = (Dictionary<Item, bool>) null;
        }
        else if (heldItem == Game1.player.pantsItem.Value)
        {
          Game1.player.pantsItem.Value = (Clothing) null;
          this._highlightDictionary = (Dictionary<Item, bool>) null;
        }
      }
      foreach (ClickableComponent equipmentIcon in this.equipmentIcons)
      {
        if (equipmentIcon.containsPoint(x, y))
        {
          string name = equipmentIcon.name;
          if (!(name == "Hat"))
          {
            if (!(name == "Shirt"))
            {
              if (!(name == "Pants"))
                return;
              Item obj1 = Utility.PerformSpecialItemPlaceReplacement(this.heldItem);
              if (this.heldItem == null)
              {
                if (!this.HighlightItems((Item) (Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) Game1.player.pantsItem))
                  return;
                this.heldItem = Utility.PerformSpecialItemGrabReplacement((Item) (Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) Game1.player.pantsItem);
                if (!(this.heldItem is Clothing))
                  Game1.player.pantsItem.Value = (Clothing) null;
                Game1.playSound("dwop");
                this._highlightDictionary = (Dictionary<Item, bool>) null;
                this._ValidateCraft();
                return;
              }
              if (!(obj1 is Clothing) || (obj1 as Clothing).clothesType.Value != 1)
                return;
              Item obj2 = Utility.PerformSpecialItemGrabReplacement((Item) Game1.player.pantsItem.Value);
              if (obj2 == this.heldItem)
                obj2 = (Item) null;
              Game1.player.pantsItem.Value = obj1 as Clothing;
              this.heldItem = obj2;
              Game1.playSound("sandyStep");
              this._highlightDictionary = (Dictionary<Item, bool>) null;
              this._ValidateCraft();
              return;
            }
            Item obj3 = Utility.PerformSpecialItemPlaceReplacement(this.heldItem);
            if (this.heldItem == null)
            {
              if (!this.HighlightItems((Item) (Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) Game1.player.shirtItem))
                return;
              this.heldItem = Utility.PerformSpecialItemGrabReplacement((Item) (Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) Game1.player.shirtItem);
              Game1.playSound("dwop");
              if (!(this.heldItem is Clothing))
                Game1.player.shirtItem.Value = (Clothing) null;
              this._highlightDictionary = (Dictionary<Item, bool>) null;
              this._ValidateCraft();
              return;
            }
            if (!(this.heldItem is Clothing) || (this.heldItem as Clothing).clothesType.Value != 0)
              return;
            Item obj4 = Utility.PerformSpecialItemGrabReplacement((Item) (Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) Game1.player.shirtItem);
            if (obj4 == this.heldItem)
              obj4 = (Item) null;
            Game1.player.shirtItem.Value = obj3 as Clothing;
            this.heldItem = obj4;
            Game1.playSound("sandyStep");
            this._highlightDictionary = (Dictionary<Item, bool>) null;
            this._ValidateCraft();
            return;
          }
          Item obj5 = Utility.PerformSpecialItemPlaceReplacement(this.heldItem);
          if (this.heldItem == null)
          {
            if (!this.HighlightItems((Item) (Hat) (NetFieldBase<Hat, NetRef<Hat>>) Game1.player.hat))
              return;
            this.heldItem = Utility.PerformSpecialItemGrabReplacement((Item) (Hat) (NetFieldBase<Hat, NetRef<Hat>>) Game1.player.hat);
            Game1.playSound("dwop");
            if (!(this.heldItem is Hat))
              Game1.player.hat.Value = (Hat) null;
            this._highlightDictionary = (Dictionary<Item, bool>) null;
            this._ValidateCraft();
            return;
          }
          if (!(obj5 is Hat))
            return;
          Item obj6 = Utility.PerformSpecialItemGrabReplacement((Item) Game1.player.hat.Value);
          if (obj6 == this.heldItem)
            obj6 = (Item) null;
          Game1.player.hat.Value = obj5 as Hat;
          this.heldItem = obj6;
          Game1.playSound("grassyStep");
          this._highlightDictionary = (Dictionary<Item, bool>) null;
          this._ValidateCraft();
          return;
        }
      }
      KeyboardState keyboardState = Game1.GetKeyboardState();
      if (keyboardState.IsKeyDown(Keys.LeftShift) && heldItem != this.heldItem && this.heldItem != null)
      {
        if (this.heldItem.Name == "Cloth" || this.heldItem is Clothing && (bool) (NetFieldBase<bool, NetBool>) (this.heldItem as Clothing).dyeable)
          this._leftIngredientSpotClicked();
        else
          this._rightIngredientSpotClicked();
      }
      if (this.IsBusy())
        return;
      if (this.leftIngredientSpot.containsPoint(x, y))
      {
        this._leftIngredientSpotClicked();
        keyboardState = Game1.GetKeyboardState();
        if (keyboardState.IsKeyDown(Keys.LeftShift) && this.heldItem != null)
        {
          if (Game1.player.IsEquippedItem(this.heldItem))
            this.heldItem = (Item) null;
          else
            this.heldItem = this.inventory.tryToAddItem(this.heldItem, "");
        }
      }
      else if (this.rightIngredientSpot.containsPoint(x, y))
      {
        this._rightIngredientSpotClicked();
        keyboardState = Game1.GetKeyboardState();
        if (keyboardState.IsKeyDown(Keys.LeftShift) && this.heldItem != null)
        {
          if (Game1.player.IsEquippedItem(this.heldItem))
            this.heldItem = (Item) null;
          else
            this.heldItem = this.inventory.tryToAddItem(this.heldItem, "");
        }
      }
      else if (this.startTailoringButton.containsPoint(x, y))
      {
        if (this.heldItem == null)
        {
          bool flag = false;
          if (!this.CanFitCraftedItem())
          {
            Game1.playSound("cancel");
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            this._timeUntilCraft = 0;
            flag = true;
          }
          if (!flag && this.IsValidCraft(this.leftIngredientSpot.item, this.rightIngredientSpot.item))
          {
            Game1.playSound("bigSelect");
            this._sewingSound = Game1.soundBank.GetCue("sewing_loop");
            this._sewingSound.Play();
            this.startTailoringButton.scale = this.startTailoringButton.baseScale;
            this._timeUntilCraft = 1500;
            this._UpdateDescriptionText();
          }
          else
            Game1.playSound("sell");
        }
        else
          Game1.playSound("sell");
      }
      if (this.heldItem == null || this.isWithinBounds(x, y) || !this.heldItem.canBeTrashed())
        return;
      if (Game1.player.IsEquippedItem(this.heldItem))
      {
        if (this.heldItem == Game1.player.hat.Value)
          Game1.player.hat.Value = (Hat) null;
        else if (this.heldItem == Game1.player.shirtItem.Value)
          Game1.player.shirtItem.Value = (Clothing) null;
        else if (this.heldItem == Game1.player.pantsItem.Value)
          Game1.player.pantsItem.Value = (Clothing) null;
      }
      Game1.playSound("throwDownITem");
      Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
      this.heldItem = (Item) null;
    }

    protected virtual bool CheckHeldItem(Func<Item, bool> f = null) => f == null ? this.heldItem != null : f(this.heldItem);

    protected void _ValidateCraft()
    {
      Item left_item = this.leftIngredientSpot.item;
      Item right_item = this.rightIngredientSpot.item;
      if (left_item == null || right_item == null)
        this._craftState = TailoringMenu.CraftState.MissingIngredients;
      else if (left_item is Clothing && !(bool) (NetFieldBase<bool, NetBool>) (left_item as Clothing).dyeable)
        this._craftState = TailoringMenu.CraftState.NotDyeable;
      else if (this.IsValidCraft(left_item, right_item))
      {
        this._craftState = TailoringMenu.CraftState.Valid;
        bool shouldPrismaticDye = this._shouldPrismaticDye;
        Item one = left_item.getOne();
        this._isMultipleResultCraft = this.IsMultipleResultCraft(left_item, right_item);
        this.craftResultDisplay.item = this.CraftItem(one, right_item.getOne());
        this._isDyeCraft = this.craftResultDisplay.item == one;
        this._shouldPrismaticDye = shouldPrismaticDye;
      }
      else
        this._craftState = TailoringMenu.CraftState.InvalidRecipe;
      this._UpdateDescriptionText();
    }

    protected void _UpdateDescriptionText()
    {
      if (this.IsBusy())
        this.displayedDescription = Game1.content.LoadString("Strings\\UI:Tailor_Busy");
      else if (this._craftState == TailoringMenu.CraftState.NotDyeable)
        this.displayedDescription = Game1.content.LoadString("Strings\\UI:Tailor_NotDyeable");
      else if (this._craftState == TailoringMenu.CraftState.MissingIngredients)
        this.displayedDescription = Game1.content.LoadString("Strings\\UI:Tailor_MissingIngredients");
      else if (this._craftState == TailoringMenu.CraftState.Valid)
      {
        if (!this.CanFitCraftedItem())
          this.displayedDescription = Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588");
        else
          this.displayedDescription = Game1.content.LoadString("Strings\\UI:Tailor_Valid");
      }
      else if (this._craftState == TailoringMenu.CraftState.InvalidRecipe)
        this.displayedDescription = Game1.content.LoadString("Strings\\UI:Tailor_InvalidRecipe");
      else
        this.displayedDescription = "";
    }

    public static Color? GetDyeColor(Item dye_object)
    {
      if (dye_object != null)
      {
        if (dye_object.Name == "Prismatic Shard")
          return new Color?(Color.White);
        if (dye_object is ColoredObject)
          return new Color?((Color) (NetFieldBase<Color, NetColor>) (dye_object as ColoredObject).color);
        Dictionary<string, Color> dictionary = new Dictionary<string, Color>();
        dictionary["black"] = new Color(45, 45, 45);
        dictionary["gray"] = Color.Gray;
        dictionary["white"] = Color.White;
        dictionary["pink"] = new Color((int) byte.MaxValue, 163, 186);
        dictionary["red"] = new Color(220, 0, 0);
        dictionary["orange"] = new Color((int) byte.MaxValue, 128, 0);
        dictionary["yellow"] = new Color((int) byte.MaxValue, 230, 0);
        dictionary["green"] = new Color(10, 143, 0);
        dictionary["blue"] = new Color(46, 85, 183);
        dictionary["purple"] = new Color(115, 41, 181);
        dictionary["brown"] = new Color(130, 73, 37);
        dictionary["light_cyan"] = new Color(180, (int) byte.MaxValue, (int) byte.MaxValue);
        dictionary["cyan"] = Color.Cyan;
        dictionary["aquamarine"] = Color.Aquamarine;
        dictionary["sea_green"] = Color.SeaGreen;
        dictionary["lime"] = Color.Lime;
        dictionary["yellow_green"] = Color.GreenYellow;
        dictionary["pale_violet_red"] = Color.PaleVioletRed;
        dictionary["salmon"] = new Color((int) byte.MaxValue, 85, 95);
        dictionary["jade"] = new Color(130, 158, 93);
        dictionary["sand"] = Color.NavajoWhite;
        dictionary["poppyseed"] = new Color(82, 47, 153);
        dictionary["dark_red"] = Color.DarkRed;
        dictionary["dark_orange"] = Color.DarkOrange;
        dictionary["dark_yellow"] = Color.DarkGoldenrod;
        dictionary["dark_green"] = Color.DarkGreen;
        dictionary["dark_blue"] = Color.DarkBlue;
        dictionary["dark_purple"] = Color.DarkViolet;
        dictionary["dark_pink"] = Color.DeepPink;
        dictionary["dark_cyan"] = Color.DarkCyan;
        dictionary["dark_gray"] = Color.DarkGray;
        dictionary["dark_brown"] = Color.SaddleBrown;
        dictionary["gold"] = Color.Gold;
        dictionary["copper"] = new Color(179, 85, 0);
        dictionary["iron"] = new Color(197, 213, 224);
        dictionary["iridium"] = new Color(105, 15, (int) byte.MaxValue);
        foreach (string key in dictionary.Keys)
        {
          if (dye_object.HasContextTag("color_" + key))
            return new Color?(dictionary[key]);
        }
      }
      return new Color?();
    }

    public bool DyeItems(Clothing clothing, Item dye_object, float dye_strength_override = -1f)
    {
      if (dye_object.Name == "Prismatic Shard")
      {
        clothing.Dye(Color.White, 1f);
        clothing.isPrismatic.Set(true);
        return true;
      }
      Color? dyeColor = TailoringMenu.GetDyeColor(dye_object);
      if (!dyeColor.HasValue)
        return false;
      float strength = 0.25f;
      if (dye_object.HasContextTag("dye_medium"))
        strength = 0.5f;
      if (dye_object.HasContextTag("dye_strong"))
        strength = 1f;
      if ((double) dye_strength_override >= 0.0)
        strength = dye_strength_override;
      clothing.Dye(dyeColor.Value, strength);
      if (clothing == Game1.player.shirtItem.Value || clothing == Game1.player.pantsItem.Value)
        Game1.player.FarmerRenderer.MarkSpriteDirty();
      return true;
    }

    public TailorItemRecipe GetRecipeForItems(Item left_item, Item right_item)
    {
      foreach (TailorItemRecipe tailoringRecipe in this._tailoringRecipes)
      {
        bool flag = false;
        if (tailoringRecipe.FirstItemTags != null && tailoringRecipe.FirstItemTags.Count > 0)
        {
          if (left_item != null)
          {
            foreach (string firstItemTag in tailoringRecipe.FirstItemTags)
            {
              if (!left_item.HasContextTag(firstItemTag))
              {
                flag = true;
                break;
              }
            }
          }
          else
            continue;
        }
        if (!flag)
        {
          if (tailoringRecipe.SecondItemTags != null && tailoringRecipe.SecondItemTags.Count > 0)
          {
            if (right_item != null)
            {
              foreach (string secondItemTag in tailoringRecipe.SecondItemTags)
              {
                if (!right_item.HasContextTag(secondItemTag))
                {
                  flag = true;
                  break;
                }
              }
            }
            else
              continue;
          }
          if (!flag)
            return tailoringRecipe;
        }
      }
      return (TailorItemRecipe) null;
    }

    public bool IsValidCraft(Item left_item, Item right_item) => left_item != null && right_item != null && (left_item is Boots && right_item is Boots || left_item is Clothing && (left_item as Clothing).dyeable.Value && (right_item.HasContextTag("color_prismatic") || TailoringMenu.GetDyeColor(right_item).HasValue) || this.GetRecipeForItems(left_item, right_item) != null);

    public bool IsMultipleResultCraft(Item left_item, Item right_item)
    {
      TailorItemRecipe recipeForItems = this.GetRecipeForItems(left_item, right_item);
      return recipeForItems != null && recipeForItems.CraftedItemIDs != null && recipeForItems.CraftedItemIDs.Count > 0;
    }

    public Item CraftItem(Item left_item, Item right_item)
    {
      if (left_item == null || right_item == null)
        return (Item) null;
      switch (left_item)
      {
        case Boots _ when left_item is Boots:
          (left_item as Boots).applyStats(right_item as Boots);
          return left_item;
        case Clothing _ when (left_item as Clothing).dyeable.Value:
          if (right_item.HasContextTag("color_prismatic"))
          {
            this._shouldPrismaticDye = true;
            return left_item;
          }
          if (this.DyeItems(left_item as Clothing, right_item))
            return left_item;
          break;
      }
      TailorItemRecipe recipeForItems = this.GetRecipeForItems(left_item, right_item);
      if (recipeForItems == null)
        return (Item) null;
      int craftedItemId = recipeForItems.CraftedItemID;
      if (recipeForItems != null && recipeForItems.CraftedItemIDs != null && recipeForItems.CraftedItemIDs.Count > 0)
        craftedItemId = int.Parse(Utility.GetRandom<string>(recipeForItems.CraftedItemIDs));
      Item obj = craftedItemId >= 0 ? (craftedItemId < 2000 || craftedItemId >= 3000 ? (Item) new Clothing(craftedItemId) : (Item) new Hat(craftedItemId - 2000)) : (Item) new StardewValley.Object(-craftedItemId, 1);
      if (obj != null && obj is Clothing)
        this.DyeItems(obj as Clothing, right_item, 1f);
      if (obj is StardewValley.Object)
      {
        StardewValley.Object object1 = obj as StardewValley.Object;
        StardewValley.Object object2 = left_item as StardewValley.Object;
        StardewValley.Object object3 = right_item as StardewValley.Object;
        if (left_item is StardewValley.Object && object2.questItem.Value || right_item is StardewValley.Object && object3.questItem.Value)
          object1.questItem.Value = true;
      }
      return obj;
    }

    public void SpendRightItem()
    {
      if (this.rightIngredientSpot.item == null)
        return;
      --this.rightIngredientSpot.item.Stack;
      if (this.rightIngredientSpot.item.Stack > 0 && this.rightIngredientSpot.item.maximumStackSize() != 1)
        return;
      this.rightIngredientSpot.item = (Item) null;
    }

    public void SpendLeftItem()
    {
      if (this.leftIngredientSpot.item == null)
        return;
      --this.leftIngredientSpot.item.Stack;
      if (this.leftIngredientSpot.item.Stack > 0)
        return;
      this.leftIngredientSpot.item = (Item) null;
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      if (this.IsBusy())
        return;
      base.receiveRightClick(x, y, true);
    }

    public override void performHoverAction(int x, int y)
    {
      if (this.IsBusy())
        return;
      this.hoveredItem = (Item) null;
      base.performHoverAction(x, y);
      this.hoverText = "";
      for (int index = 0; index < this.equipmentIcons.Count; ++index)
      {
        if (this.equipmentIcons[index].containsPoint(x, y))
        {
          if (this.equipmentIcons[index].name == "Shirt")
            this.hoveredItem = (Item) Game1.player.shirtItem.Value;
          else if (this.equipmentIcons[index].name == "Hat")
            this.hoveredItem = (Item) Game1.player.hat.Value;
          else if (this.equipmentIcons[index].name == "Pants")
            this.hoveredItem = (Item) Game1.player.pantsItem.Value;
        }
      }
      if (this.craftResultDisplay.visible && this.craftResultDisplay.containsPoint(x, y) && this.craftResultDisplay.item != null)
      {
        if (this._isDyeCraft || Game1.player.HasTailoredThisItem(this.craftResultDisplay.item))
          this.hoveredItem = this.craftResultDisplay.item;
        else
          this.hoverText = Game1.content.LoadString("Strings\\UI:Tailor_MakeResultUnknown");
      }
      if (this.leftIngredientSpot.containsPoint(x, y))
      {
        if (this.leftIngredientSpot.item != null)
          this.hoveredItem = this.leftIngredientSpot.item;
        else
          this.hoverText = Game1.content.LoadString("Strings\\UI:Tailor_Feed");
      }
      if (this.rightIngredientSpot.containsPoint(x, y) && this.rightIngredientSpot.item == null)
        this.hoverText = Game1.content.LoadString("Strings\\UI:Tailor_Spool");
      this.rightIngredientSpot.tryHover(x, y);
      this.leftIngredientSpot.tryHover(x, y);
      if (this._craftState == TailoringMenu.CraftState.Valid && this.CanFitCraftedItem())
        this.startTailoringButton.tryHover(x, y, 0.33f);
      else
        this.startTailoringButton.tryHover(-999, -999);
    }

    public bool CanFitCraftedItem() => this.craftResultDisplay.item == null || Utility.canItemBeAddedToThisInventoryList(this.craftResultDisplay.item, this.inventory.actualInventory);

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      Console.WriteLine("meow:" + oldBounds.ToString() + " " + newBounds.ToString() + " " + this.width.ToString() + " " + this.height.ToString() + " " + this.yPositionOnScreen.ToString());
      base.gameWindowSizeChanged(oldBounds, newBounds);
      Console.WriteLine("meow2:" + this.yPositionOnScreen.ToString());
      int yPosition = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 192 - 16 + 128 + 4;
      this.inventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 12, yPosition, false, highlightMethod: this.inventory.highlightMethod);
      this._CreateButtons();
    }

    public override void emergencyShutDown()
    {
      this._OnCloseMenu();
      base.emergencyShutDown();
    }

    public override void update(GameTime time)
    {
      base.update(time);
      this.descriptionText = this.displayedDescription;
      this.questionMarkOffset.X = (float) (Math.Sin(time.TotalGameTime.TotalSeconds * 2.5) * 4.0);
      this.questionMarkOffset.Y = (float) (Math.Cos(time.TotalGameTime.TotalSeconds * 5.0) * -4.0);
      bool flag = this.CanFitCraftedItem();
      this.startTailoringButton.sourceRect.Y = !(this._craftState == TailoringMenu.CraftState.Valid & flag) ? 80 : 104;
      if (((this._craftState != TailoringMenu.CraftState.Valid ? 0 : (!this.IsBusy() ? 1 : 0)) & (flag ? 1 : 0)) != 0)
        this.craftResultDisplay.visible = true;
      else
        this.craftResultDisplay.visible = false;
      if (this._timeUntilCraft > 0)
      {
        this.startTailoringButton.tryHover(this.startTailoringButton.bounds.Center.X, this.startTailoringButton.bounds.Center.Y, 0.33f);
        Vector2 vector2 = new Vector2(0.0f, 0.0f);
        vector2.X = Utility.Lerp(this.leftIngredientEndSpot.X, this.leftIngredientStartSpot.X, (float) this._timeUntilCraft / 1500f);
        vector2.Y = Utility.Lerp(this.leftIngredientEndSpot.Y, this.leftIngredientStartSpot.Y, (float) this._timeUntilCraft / 1500f);
        this.leftIngredientSpot.bounds.X = (int) vector2.X;
        this.leftIngredientSpot.bounds.Y = (int) vector2.Y;
        this._timeUntilCraft -= time.ElapsedGameTime.Milliseconds;
        this.needleSprite.bounds.Location = new Point((int) this.needlePosition.X, (int) ((double) this.needlePosition.Y - 2.0 * ((double) this._timeUntilCraft % 25.0) / 25.0 * 4.0));
        this.presserSprite.bounds.Location = new Point((int) this.presserPosition.X, (int) ((double) this.presserPosition.Y - 1.0 * ((double) this._timeUntilCraft % 50.0) / 50.0 * 4.0));
        this._rightItemOffset = (float) Math.Sin(time.TotalGameTime.TotalMilliseconds * 2.0 * Math.PI / 180.0) * 2f;
        if (this._timeUntilCraft > 0)
          return;
        TailorItemRecipe recipeForItems = this.GetRecipeForItems(this.leftIngredientSpot.item, this.rightIngredientSpot.item);
        this._shouldPrismaticDye = false;
        Item i = this.CraftItem(this.leftIngredientSpot.item, this.rightIngredientSpot.item);
        if (this._sewingSound != null && this._sewingSound.IsPlaying)
          this._sewingSound.Stop(AudioStopOptions.Immediate);
        if (!Utility.canItemBeAddedToThisInventoryList(i, this.inventory.actualInventory))
        {
          Game1.playSound("cancel");
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
          this._timeUntilCraft = 0;
          return;
        }
        if (this.leftIngredientSpot.item == i)
          this.leftIngredientSpot.item = (Item) null;
        else
          this.SpendLeftItem();
        if ((recipeForItems == null || recipeForItems.SpendRightItem) && (this.readyToClose() || !this._shouldPrismaticDye))
          this.SpendRightItem();
        if (recipeForItems != null)
          Game1.player.MarkItemAsTailored(i);
        Game1.playSound("coin");
        this.heldItem = i;
        this._timeUntilCraft = 0;
        this._ValidateCraft();
        if (this._shouldPrismaticDye)
        {
          Item heldItem = this.heldItem;
          this.heldItem = (Item) null;
          if (this.readyToClose())
          {
            this.exitThisMenuNoSound();
            Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(i as Clothing);
            return;
          }
          this.heldItem = heldItem;
        }
      }
      this._rightItemOffset = 0.0f;
      this.leftIngredientSpot.bounds.X = (int) this.leftIngredientStartSpot.X;
      this.leftIngredientSpot.bounds.Y = (int) this.leftIngredientStartSpot.Y;
      this.needleSprite.bounds.Location = new Point((int) this.needlePosition.X, (int) this.needlePosition.Y);
      this.presserSprite.bounds.Location = new Point((int) this.presserPosition.X, (int) this.presserPosition.Y);
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
      b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 96f, (float) (this.yPositionOnScreen - 64)), new Rectangle?(new Rectangle(101, 80, 41, 36)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.87f);
      b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 352f, (float) (this.yPositionOnScreen - 64)), new Rectangle?(new Rectangle(101, 80, 41, 36)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 608f, (float) (this.yPositionOnScreen - 64)), new Rectangle?(new Rectangle(101, 80, 41, 36)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 256f, (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(79, 97, 22, 20)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 512f, (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(79, 97, 22, 20)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 32f, (float) (this.yPositionOnScreen + 44)), new Rectangle?(new Rectangle(81, 81, 16, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 768f, (float) (this.yPositionOnScreen + 44)), new Rectangle?(new Rectangle(81, 81, 16, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      Game1.DrawBox(this.xPositionOnScreen - 64, this.yPositionOnScreen + 128, 128, 265, new Color?(new Color(50, 160, (int) byte.MaxValue)));
      Game1.player.FarmerRenderer.drawMiniPortrat(b, new Vector2((float) (this.xPositionOnScreen - 64) + 9.6f, (float) (this.yPositionOnScreen + 128)), 0.87f, 4f, 2, Game1.player);
      this.draw(b, true, true, 50, 160, (int) byte.MaxValue);
      b.Draw(this.tailoringTextures, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 - 4), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder)), new Rectangle?(new Rectangle(0, 0, 142, 80)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      this.startTailoringButton.draw(b, Color.White, 0.96f);
      this.startTailoringButton.drawItem(b, 16, 16);
      this.presserSprite.draw(b, Color.White, 0.99f);
      this.needleSprite.draw(b, Color.White, 0.97f);
      Point point = new Point(0, 0);
      if (!this.IsBusy())
      {
        if (this.leftIngredientSpot.item != null)
          this.blankLeftIngredientSpot.draw(b);
        else
          this.leftIngredientSpot.draw(b, Color.White, 0.87f, (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000 / 200);
      }
      else
      {
        point.X = Game1.random.Next(-1, 2);
        point.Y = Game1.random.Next(-1, 2);
      }
      this.leftIngredientSpot.drawItem(b, (4 + point.X) * 4, (4 + point.Y) * 4);
      if (this.craftResultDisplay.visible)
      {
        string text = Game1.content.LoadString("Strings\\UI:Tailor_MakeResult");
        Vector2 position = new Vector2((float) this.craftResultDisplay.bounds.Center.X - Game1.smallFont.MeasureString(text).X / 2f, (float) this.craftResultDisplay.bounds.Top - Game1.smallFont.MeasureString(text).Y);
        Utility.drawTextWithColoredShadow(b, text, Game1.smallFont, position, Game1.textColor * 0.75f, Color.Black * 0.2f);
        this.craftResultDisplay.draw(b);
        if (this.craftResultDisplay.item != null)
        {
          if (this._isMultipleResultCraft)
          {
            Rectangle bounds = this.craftResultDisplay.bounds;
            bounds.X += 6;
            bounds.Y -= 8 + (int) this.questionMarkOffset.Y;
            b.Draw(this.tailoringTextures, bounds, new Rectangle?(new Rectangle(112, 208, 16, 16)), Color.White);
          }
          else if (this._isDyeCraft || Game1.player.HasTailoredThisItem(this.craftResultDisplay.item))
          {
            this.craftResultDisplay.drawItem(b);
          }
          else
          {
            if (this.craftResultDisplay.item is Hat)
              b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle?(new Rectangle(96, 208, 16, 16)), Color.White);
            else if (this.craftResultDisplay.item is Clothing)
            {
              if ((this.craftResultDisplay.item as Clothing).clothesType.Value == 1)
                b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle?(new Rectangle(64, 208, 16, 16)), Color.White);
              else if ((this.craftResultDisplay.item as Clothing).clothesType.Value == 0)
                b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle?(new Rectangle(80, 208, 16, 16)), Color.White);
            }
            else if (this.craftResultDisplay.item is StardewValley.Object @object && Utility.IsNormalObjectAtParentSheetIndex((Item) @object, 71))
              b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle?(new Rectangle(64, 208, 16, 16)), Color.White);
            Rectangle bounds = this.craftResultDisplay.bounds;
            bounds.X += 24;
            bounds.Y += 12 + (int) this.questionMarkOffset.Y;
            b.Draw(this.tailoringTextures, bounds, new Rectangle?(new Rectangle(112, 208, 16, 16)), Color.White);
          }
        }
      }
      foreach (ClickableComponent equipmentIcon in this.equipmentIcons)
      {
        string name = equipmentIcon.name;
        if (!(name == "Hat"))
        {
          if (!(name == "Shirt"))
          {
            if (name == "Pants")
            {
              if (Game1.player.pantsItem.Value != null)
              {
                b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(0, 208, 16, 16)), Color.White);
                float transparency = 1f;
                if (!this.HighlightItems((Item) (Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) Game1.player.pantsItem))
                  transparency = 0.5f;
                if (Game1.player.pantsItem.Value == this.heldItem)
                  transparency = 0.5f;
                Game1.player.pantsItem.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale, transparency, 0.866f);
              }
              else
                b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(16, 208, 16, 16)), Color.White);
            }
          }
          else if (Game1.player.shirtItem.Value != null)
          {
            b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(0, 208, 16, 16)), Color.White);
            float transparency = 1f;
            if (!this.HighlightItems((Item) (Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) Game1.player.shirtItem))
              transparency = 0.5f;
            if (Game1.player.shirtItem.Value == this.heldItem)
              transparency = 0.5f;
            Game1.player.shirtItem.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale, transparency, 0.866f);
          }
          else
            b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(32, 208, 16, 16)), Color.White);
        }
        else if (Game1.player.hat.Value != null)
        {
          b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(0, 208, 16, 16)), Color.White);
          float transparency = 1f;
          if (!this.HighlightItems((Item) (Hat) (NetFieldBase<Hat, NetRef<Hat>>) Game1.player.hat))
            transparency = 0.5f;
          if (Game1.player.hat.Value == this.heldItem)
            transparency = 0.5f;
          Game1.player.hat.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale, transparency, 0.866f, StackDrawType.Hide);
        }
        else
          b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(48, 208, 16, 16)), Color.White);
      }
      if (!this.IsBusy())
      {
        if (this.rightIngredientSpot.item != null)
          this.blankRightIngredientSpot.draw(b);
        else
          this.rightIngredientSpot.draw(b, Color.White, 0.87f, (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000 / 200);
      }
      this.rightIngredientSpot.drawItem(b, 16, (4 + (int) this._rightItemOffset) * 4);
      if (!this.hoverText.Equals(""))
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, this.heldItem != null ? 32 : 0, this.heldItem != null ? 32 : 0);
      else if (this.hoveredItem != null)
        IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem, this.heldItem != null);
      if (this.heldItem != null)
        this.heldItem.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 8), (float) (Game1.getOldMouseY() + 8)), 1f);
      if (Game1.options.hardwareCursor)
        return;
      this.drawMouse(b);
    }

    protected override void cleanupBeforeExit() => this._OnCloseMenu();

    protected void _OnCloseMenu()
    {
      if (!Game1.player.IsEquippedItem(this.heldItem))
        Utility.CollectOrDrop(this.heldItem);
      if (!Game1.player.IsEquippedItem(this.leftIngredientSpot.item))
        Utility.CollectOrDrop(this.leftIngredientSpot.item);
      if (!Game1.player.IsEquippedItem(this.rightIngredientSpot.item))
        Utility.CollectOrDrop(this.rightIngredientSpot.item);
      if (!Game1.player.IsEquippedItem(this.startTailoringButton.item))
        Utility.CollectOrDrop(this.startTailoringButton.item);
      this.heldItem = (Item) null;
      this.leftIngredientSpot.item = (Item) null;
      this.rightIngredientSpot.item = (Item) null;
      this.startTailoringButton.item = (Item) null;
    }

    protected enum CraftState
    {
      MissingIngredients,
      Valid,
      InvalidRecipe,
      NotDyeable,
    }
  }
}
