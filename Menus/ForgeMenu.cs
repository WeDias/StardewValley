// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ForgeMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class ForgeMenu : MenuWithInventory
  {
    protected int _timeUntilCraft;
    protected int _clankEffectTimer;
    protected int _sparklingTimer;
    public const int region_leftIngredient = 998;
    public const int region_rightIngredient = 997;
    public const int region_startButton = 996;
    public const int region_resultItem = 995;
    public const int region_unforgeButton = 994;
    public ClickableTextureComponent craftResultDisplay;
    public ClickableTextureComponent leftIngredientSpot;
    public ClickableTextureComponent rightIngredientSpot;
    public ClickableTextureComponent startTailoringButton;
    public ClickableComponent unforgeButton;
    public List<ClickableComponent> equipmentIcons = new List<ClickableComponent>();
    public const int region_ring_1 = 110;
    public const int region_ring_2 = 111;
    public const int CRAFT_TIME = 1600;
    public Texture2D forgeTextures;
    protected Dictionary<Item, bool> _highlightDictionary;
    protected Dictionary<string, Item> _lastValidEquippedItems;
    protected List<TemporaryAnimatedSprite> tempSprites = new List<TemporaryAnimatedSprite>();
    private bool unforging;
    protected string displayedDescription = "";
    protected ForgeMenu.CraftState _craftState;
    public Vector2 questionMarkOffset;

    public ForgeMenu()
      : base(okButton: true, trashCan: true, inventoryXOffset: 12, inventoryYOffset: 132)
    {
      Game1.playSound("bigSelect");
      if (this.yPositionOnScreen == IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder)
        this.movePosition(0, -IClickableMenu.spaceToClearTopBorder);
      this.inventory.highlightMethod = new InventoryMenu.highlightThisItem(this.HighlightItems);
      this.forgeTextures = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\ForgeMenu");
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
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 204, this.yPositionOnScreen + 212, 64, 64), this.forgeTextures, new Rectangle(142, 0, 16, 16), 4f);
      textureComponent1.myID = 998;
      textureComponent1.downNeighborID = -99998;
      textureComponent1.leftNeighborID = 110;
      textureComponent1.rightNeighborID = 997;
      textureComponent1.item = this.leftIngredientSpot != null ? this.leftIngredientSpot.item : (Item) null;
      textureComponent1.fullyImmutable = true;
      this.leftIngredientSpot = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 348, this.yPositionOnScreen + 212, 64, 64), this.forgeTextures, new Rectangle(142, 0, 16, 16), 4f);
      textureComponent2.myID = 997;
      textureComponent2.downNeighborID = 996;
      textureComponent2.leftNeighborID = 998;
      textureComponent2.rightNeighborID = 994;
      textureComponent2.item = this.rightIngredientSpot != null ? this.rightIngredientSpot.item : (Item) null;
      textureComponent2.fullyImmutable = true;
      this.rightIngredientSpot = textureComponent2;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 204, this.yPositionOnScreen + 308, 52, 56), this.forgeTextures, new Rectangle(0, 80, 13, 14), 4f);
      textureComponent3.myID = 996;
      textureComponent3.downNeighborID = -99998;
      textureComponent3.leftNeighborID = 111;
      textureComponent3.rightNeighborID = 994;
      textureComponent3.upNeighborID = 998;
      textureComponent3.item = this.startTailoringButton != null ? this.startTailoringButton.item : (Item) null;
      textureComponent3.fullyImmutable = true;
      this.startTailoringButton = textureComponent3;
      this.unforgeButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 484, this.yPositionOnScreen + 312, 40, 44), "Unforge")
      {
        myID = 994,
        downNeighborID = -99998,
        leftNeighborID = 996,
        rightNeighborID = 995,
        upNeighborID = 997,
        fullyImmutable = true
      };
      if (this.inventory.inventory != null && this.inventory.inventory.Count >= 12)
      {
        for (int index = 0; index < 12; ++index)
        {
          if (this.inventory.inventory[index] != null)
            this.inventory.inventory[index].upNeighborID = -99998;
        }
      }
      ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 660, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 232, 64, 64), this.forgeTextures, new Rectangle(0, 208, 16, 16), 4f);
      textureComponent4.myID = 995;
      textureComponent4.downNeighborID = -99998;
      textureComponent4.leftNeighborID = 996;
      textureComponent4.upNeighborID = 997;
      textureComponent4.item = this.craftResultDisplay != null ? this.craftResultDisplay.item : (Item) null;
      this.craftResultDisplay = textureComponent4;
      this.equipmentIcons = new List<ClickableComponent>();
      this.equipmentIcons.Add(new ClickableComponent(new Rectangle(0, 0, 64, 64), "Ring1")
      {
        myID = 110,
        leftNeighborID = -99998,
        downNeighborID = -99998,
        upNeighborID = -99998,
        rightNeighborID = -99998
      });
      this.equipmentIcons.Add(new ClickableComponent(new Rectangle(0, 0, 64, 64), "Ring2")
      {
        myID = 111,
        upNeighborID = -99998,
        downNeighborID = -99998,
        rightNeighborID = -99998,
        leftNeighborID = -99998
      });
      for (int index = 0; index < this.equipmentIcons.Count; ++index)
      {
        this.equipmentIcons[index].bounds.X = this.xPositionOnScreen - 64 + 9;
        this.equipmentIcons[index].bounds.Y = this.yPositionOnScreen + 192 + index * 64;
      }
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public bool IsBusy() => this._timeUntilCraft > 0 || this._sparklingTimer > 0;

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
      if (Game1.player.leftRing.Value != null)
        objList.Add((Item) Game1.player.leftRing.Value);
      if (Game1.player.rightRing.Value != null)
        objList.Add((Item) Game1.player.rightRing.Value);
      foreach (Item obj in objList)
      {
        if (obj != null)
        {
          if (Utility.IsNormalObjectAtParentSheetIndex(obj, 848))
            this._highlightDictionary[obj] = true;
          else if (this.leftIngredientSpot.item == null && this.rightIngredientSpot.item == null)
          {
            bool flag = false;
            if (obj is Ring)
              flag = true;
            if (obj is Tool && BaseEnchantment.GetAvailableEnchantmentsForItem(obj as Tool).Count > 0)
              flag = true;
            if (BaseEnchantment.GetEnchantmentFromItem((Item) null, obj) != null)
              flag = true;
            this._highlightDictionary[obj] = flag;
          }
          else
            this._highlightDictionary[obj] = (this.leftIngredientSpot.item == null || this.rightIngredientSpot.item == null) && (this.leftIngredientSpot.item == null ? this.IsValidCraft(obj, this.rightIngredientSpot.item) : this.IsValidCraft(this.leftIngredientSpot.item, obj));
        }
      }
    }

    private void _leftIngredientSpotClicked()
    {
      Item obj = this.leftIngredientSpot.item;
      if (this.heldItem != null && !this.IsValidCraftIngredient(this.heldItem) || this.heldItem != null && !(this.heldItem is Tool) && !(this.heldItem is Ring))
        return;
      Game1.playSound("stoneStep");
      this.leftIngredientSpot.item = this.heldItem;
      this.heldItem = obj;
      this._highlightDictionary = (Dictionary<Item, bool>) null;
      this._ValidateCraft();
    }

    public bool IsValidCraftIngredient(Item item) => item.canBeTrashed() || item is Tool && BaseEnchantment.GetAvailableEnchantmentsForItem(item as Tool).Count > 0;

    private void _rightIngredientSpotClicked()
    {
      Item obj = this.rightIngredientSpot.item;
      if (this.heldItem != null && !this.IsValidCraftIngredient(this.heldItem) || this.heldItem != null && (int) (NetFieldBase<int, NetInt>) this.heldItem.parentSheetIndex == 848)
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
      Item heldItem1 = this.heldItem;
      Game1.player.IsEquippedItem(heldItem1);
      base.receiveLeftClick(x, y, true);
      foreach (ClickableComponent equipmentIcon in this.equipmentIcons)
      {
        if (equipmentIcon.containsPoint(x, y))
        {
          string name = equipmentIcon.name;
          if (!(name == "Ring1"))
          {
            if (!(name == "Ring2") || !this.HighlightItems((Item) Game1.player.rightRing.Value) && Game1.player.rightRing.Value != null)
              return;
            Item heldItem2 = this.heldItem;
            Item obj = (Item) Game1.player.rightRing.Value;
            if (obj == this.heldItem)
              return;
            switch (heldItem2)
            {
              case null:
              case Ring _:
                if (Game1.player.rightRing.Value != null)
                  Game1.player.rightRing.Value.onUnequip(Game1.player, Game1.currentLocation);
                Game1.player.rightRing.Value = heldItem2 as Ring;
                this.heldItem = obj;
                if (Game1.player.rightRing.Value != null)
                {
                  Game1.player.rightRing.Value.onEquip(Game1.player, Game1.currentLocation);
                  Game1.playSound("crit");
                }
                else if (this.heldItem != null)
                  Game1.playSound("dwop");
                this._highlightDictionary = (Dictionary<Item, bool>) null;
                this._ValidateCraft();
                return;
              default:
                return;
            }
          }
          else
          {
            if (!this.HighlightItems((Item) Game1.player.leftRing.Value) && Game1.player.leftRing.Value != null)
              return;
            Item heldItem3 = this.heldItem;
            Item obj = (Item) Game1.player.leftRing.Value;
            if (obj == this.heldItem)
              return;
            switch (heldItem3)
            {
              case null:
              case Ring _:
                if (Game1.player.leftRing.Value != null)
                  Game1.player.leftRing.Value.onUnequip(Game1.player, Game1.currentLocation);
                Game1.player.leftRing.Value = heldItem3 as Ring;
                this.heldItem = obj;
                if (Game1.player.leftRing.Value != null)
                {
                  Game1.player.leftRing.Value.onEquip(Game1.player, Game1.currentLocation);
                  Game1.playSound("crit");
                }
                else if (this.heldItem != null)
                  Game1.playSound("dwop");
                this._highlightDictionary = (Dictionary<Item, bool>) null;
                this._ValidateCraft();
                return;
              default:
                return;
            }
          }
        }
      }
      KeyboardState keyboardState = Game1.GetKeyboardState();
      if (keyboardState.IsKeyDown(Keys.LeftShift) && heldItem1 != this.heldItem && this.heldItem != null)
      {
        if (this.heldItem is Tool || this.heldItem is Ring && this.leftIngredientSpot.item == null)
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
          if (!flag && this.IsValidCraft(this.leftIngredientSpot.item, this.rightIngredientSpot.item) && Game1.player.hasItemInInventory(848, this.GetForgeCost(this.leftIngredientSpot.item, this.rightIngredientSpot.item)))
          {
            Game1.playSound("bigSelect");
            this.startTailoringButton.scale = this.startTailoringButton.baseScale;
            this._timeUntilCraft = 1600;
            this._clankEffectTimer = 300;
            this._UpdateDescriptionText();
            int forgeCost = this.GetForgeCost(this.leftIngredientSpot.item, this.rightIngredientSpot.item);
            for (int index = 0; index < forgeCost; ++index)
              this.tempSprites.Add(new TemporaryAnimatedSprite("", new Rectangle(143, 17, 14, 15), new Vector2((float) (this.xPositionOnScreen + 276), (float) (this.yPositionOnScreen + 300)), false, 0.1f, Color.White)
              {
                texture = this.forgeTextures,
                motion = new Vector2(-4f, -4f),
                scale = 4f,
                layerDepth = 1f,
                startSound = "boulderCrack",
                delayBeforeAnimationStart = 1400 / forgeCost * index
              });
            if (this.rightIngredientSpot.item != null && (int) (NetFieldBase<int, NetInt>) this.rightIngredientSpot.item.parentSheetIndex == 74)
            {
              this._sparklingTimer = 900;
              Rectangle bounds1 = this.leftIngredientSpot.bounds;
              bounds1.Offset(-32, -32);
              List<TemporaryAnimatedSprite> temporaryAnimatedSpriteList = Utility.sparkleWithinArea(bounds1, 6, Color.White, 80, 1600);
              temporaryAnimatedSpriteList.First<TemporaryAnimatedSprite>().startSound = "discoverMineral";
              this.tempSprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) temporaryAnimatedSpriteList);
              Rectangle bounds2 = this.rightIngredientSpot.bounds;
              bounds2.Inflate(-16, -16);
              Utility.getRandomPositionInThisRectangle(bounds2, Game1.random);
              int num = 30;
              for (int index = 0; index < num; ++index)
                this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(114, 48, 2, 2), Utility.getRandomPositionInThisRectangle(bounds2, Game1.random), false, 0.0f, Color.White)
                {
                  motion = new Vector2(-4f, 0.0f),
                  yPeriodic = true,
                  yPeriodicRange = 16f,
                  yPeriodicLoopTime = 1200f,
                  scale = 4f,
                  layerDepth = 1f,
                  animationLength = 12,
                  interval = (float) Game1.random.Next(20, 40),
                  totalNumberOfLoops = 1,
                  delayBeforeAnimationStart = this._clankEffectTimer / num * index
                });
            }
          }
          else
            Game1.playSound("sell");
        }
        else
          Game1.playSound("sell");
      }
      else if (this.unforgeButton.containsPoint(x, y))
      {
        if (this.rightIngredientSpot.item == null)
        {
          if (this.IsValidUnforge())
          {
            if (this.leftIngredientSpot.item is MeleeWeapon && !Game1.player.couldInventoryAcceptThisObject(848, (this.leftIngredientSpot.item as MeleeWeapon).GetTotalForgeLevels() * 5 + ((this.leftIngredientSpot.item as MeleeWeapon).GetTotalForgeLevels() - 1) * 2))
            {
              this.displayedDescription = Game1.content.LoadString("Strings\\UI:Forge_noroom");
              Game1.playSound("cancel");
            }
            else if (this.leftIngredientSpot.item is CombinedRing && Game1.player.freeSpotsInInventory() < 2)
            {
              this.displayedDescription = Game1.content.LoadString("Strings\\UI:Forge_noroom");
              Game1.playSound("cancel");
            }
            else
            {
              this.unforging = true;
              this._timeUntilCraft = 1600;
              int num = this.GetForgeCost(this.leftIngredientSpot.item, this.rightIngredientSpot.item) / 2;
              for (int index = 0; index < num; ++index)
              {
                Vector2 vector2 = new Vector2((float) Game1.random.Next(-4, 5), (float) Game1.random.Next(-4, 5));
                if ((double) vector2.X == 0.0 && (double) vector2.Y == 0.0)
                  vector2 = new Vector2(-4f, -4f);
                this.tempSprites.Add(new TemporaryAnimatedSprite("", new Rectangle(143, 17, 14, 15), new Vector2((float) this.leftIngredientSpot.bounds.X, (float) this.leftIngredientSpot.bounds.Y), false, 0.1f, Color.White)
                {
                  alpha = 0.01f,
                  alphaFade = -0.1f,
                  alphaFadeFade = -0.005f,
                  texture = this.forgeTextures,
                  motion = vector2,
                  scale = 4f,
                  layerDepth = 1f,
                  startSound = "boulderCrack",
                  delayBeforeAnimationStart = 1100 / num * index
                });
              }
              Game1.playSound("debuffHit");
            }
          }
          else
          {
            this.displayedDescription = Game1.content.LoadString("Strings\\UI:Forge_unforge_invalid");
            Game1.playSound("cancel");
          }
        }
        else
        {
          this.displayedDescription = !this.IsValidUnforge(true) ? Game1.content.LoadString("Strings\\UI:Forge_unforge_invalid") : Game1.content.LoadString("Strings\\UI:Forge_unforge_right_slot");
          Game1.playSound("cancel");
        }
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

    public virtual int GetForgeCostAtLevel(int level) => 10 + level * 5;

    public virtual int GetForgeCost(Item left_item, Item right_item)
    {
      if (right_item != null && (int) (NetFieldBase<int, NetInt>) right_item.parentSheetIndex == 896 || right_item != null && (int) (NetFieldBase<int, NetInt>) right_item.parentSheetIndex == 74)
        return 20;
      if (right_item != null && (int) (NetFieldBase<int, NetInt>) right_item.parentSheetIndex == 72 || left_item is MeleeWeapon && right_item is MeleeWeapon)
        return 10;
      if (left_item != null && left_item is Tool)
        return this.GetForgeCostAtLevel((left_item as Tool).GetTotalForgeLevels());
      return left_item != null && left_item is Ring && right_item != null && right_item is Ring ? 20 : 1;
    }

    protected void _ValidateCraft()
    {
      Item left_item = this.leftIngredientSpot.item;
      Item right_item = this.rightIngredientSpot.item;
      if (left_item == null || right_item == null)
        this._craftState = ForgeMenu.CraftState.MissingIngredients;
      else if (this.IsValidCraft(left_item, right_item))
      {
        this._craftState = ForgeMenu.CraftState.Valid;
        Item one = left_item.getOne();
        if (right_item != null && Utility.IsNormalObjectAtParentSheetIndex(right_item, 72))
        {
          (one as Tool).AddEnchantment((BaseEnchantment) new DiamondEnchantment());
          this.craftResultDisplay.item = one;
        }
        else
          this.craftResultDisplay.item = this.CraftItem(one, right_item.getOne());
      }
      else
        this._craftState = ForgeMenu.CraftState.InvalidRecipe;
      this._UpdateDescriptionText();
    }

    protected void _UpdateDescriptionText()
    {
      if (this.IsBusy())
      {
        if (this.rightIngredientSpot.item != null && (int) (NetFieldBase<int, NetInt>) this.rightIngredientSpot.item.parentSheetIndex == 74)
          this.displayedDescription = Game1.content.LoadString("Strings\\UI:Forge_enchanting");
        else
          this.displayedDescription = Game1.content.LoadString("Strings\\UI:Forge_forging");
      }
      else if (this._craftState == ForgeMenu.CraftState.MissingIngredients)
        this.displayedDescription = this.displayedDescription = Game1.content.LoadString("Strings\\UI:Forge_description1") + Environment.NewLine + Environment.NewLine + Game1.content.LoadString("Strings\\UI:Forge_description2");
      else if (this._craftState == ForgeMenu.CraftState.MissingShards)
      {
        if (this.heldItem != null && this.heldItem.ParentSheetIndex == 848)
          this.displayedDescription = Game1.content.LoadString("Strings\\UI:Forge_shards");
        else
          this.displayedDescription = Game1.content.LoadString("Strings\\UI:Forge_notenoughshards");
      }
      else if (this._craftState == ForgeMenu.CraftState.Valid)
      {
        if (!this.CanFitCraftedItem())
          this.displayedDescription = Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588");
        else
          this.displayedDescription = Game1.content.LoadString("Strings\\UI:Forge_valid");
      }
      else if (this._craftState == ForgeMenu.CraftState.InvalidRecipe)
        this.displayedDescription = Game1.content.LoadString("Strings\\UI:Forge_wrongorder");
      else
        this.displayedDescription = "";
    }

    public bool IsValidCraft(Item left_item, Item right_item) => left_item != null && right_item != null && (left_item is Tool && (left_item as Tool).CanForge(right_item) || left_item is Ring && right_item is Ring && (left_item as Ring).CanCombine(right_item as Ring));

    public Item CraftItem(Item left_item, Item right_item, bool forReal = false)
    {
      if (left_item == null || right_item == null)
        return (Item) null;
      switch (left_item)
      {
        case Tool _ when !(left_item as Tool).Forge(right_item, forReal):
          return (Item) null;
        case Ring _ when right_item is Ring:
          left_item = (Item) (left_item as Ring).Combine(right_item as Ring);
          break;
      }
      return left_item;
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
      if (this.leftIngredientSpot.item.Stack > 0 && this.leftIngredientSpot.item.maximumStackSize() != 1)
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
          if (this.equipmentIcons[index].name == "Ring1")
            this.hoveredItem = (Item) Game1.player.leftRing.Value;
          else if (this.equipmentIcons[index].name == "Ring2")
            this.hoveredItem = (Item) Game1.player.rightRing.Value;
        }
      }
      if (this.craftResultDisplay.visible && this.craftResultDisplay.containsPoint(x, y) && this.craftResultDisplay.item != null)
        this.hoveredItem = this.craftResultDisplay.item;
      if (this.leftIngredientSpot.containsPoint(x, y) && this.leftIngredientSpot.item != null)
        this.hoveredItem = this.leftIngredientSpot.item;
      if (this.rightIngredientSpot.containsPoint(x, y) && this.rightIngredientSpot.item != null)
        this.hoveredItem = this.rightIngredientSpot.item;
      if (this.unforgeButton.containsPoint(x, y))
        this.hoverText = Game1.content.LoadString("Strings\\UI:Forge_Unforge");
      if (this._craftState == ForgeMenu.CraftState.Valid && this.CanFitCraftedItem())
        this.startTailoringButton.tryHover(x, y, 0.33f);
      else
        this.startTailoringButton.tryHover(-999, -999);
    }

    public bool CanFitCraftedItem() => this.craftResultDisplay.item == null || Utility.canItemBeAddedToThisInventoryList(this.craftResultDisplay.item, this.inventory.actualInventory);

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
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
      for (int index = this.tempSprites.Count - 1; index >= 0; --index)
      {
        if (this.tempSprites[index].update(time))
          this.tempSprites.RemoveAt(index);
      }
      if (this.leftIngredientSpot.item != null && this.rightIngredientSpot.item != null && !Game1.player.hasItemInInventory(848, this.GetForgeCost(this.leftIngredientSpot.item, this.rightIngredientSpot.item)))
      {
        if (this._craftState != ForgeMenu.CraftState.MissingShards)
        {
          this._craftState = ForgeMenu.CraftState.MissingShards;
          this.craftResultDisplay.item = (Item) null;
          this._UpdateDescriptionText();
        }
      }
      else if (this._craftState == ForgeMenu.CraftState.MissingShards)
        this._ValidateCraft();
      this.descriptionText = this.displayedDescription;
      ref Vector2 local1 = ref this.questionMarkOffset;
      TimeSpan timeSpan = time.TotalGameTime;
      double num1 = Math.Sin(timeSpan.TotalSeconds * 2.5) * 4.0;
      local1.X = (float) num1;
      ref Vector2 local2 = ref this.questionMarkOffset;
      timeSpan = time.TotalGameTime;
      double num2 = Math.Cos(timeSpan.TotalSeconds * 5.0) * -4.0;
      local2.Y = (float) num2;
      bool flag = this.CanFitCraftedItem();
      if (((this._craftState != ForgeMenu.CraftState.Valid ? 0 : (!this.IsBusy() ? 1 : 0)) & (flag ? 1 : 0)) != 0)
        this.craftResultDisplay.visible = true;
      else
        this.craftResultDisplay.visible = false;
      if (this._timeUntilCraft <= 0 && this._sparklingTimer <= 0)
        return;
      this.startTailoringButton.tryHover(this.startTailoringButton.bounds.Center.X, this.startTailoringButton.bounds.Center.Y, 0.33f);
      int timeUntilCraft = this._timeUntilCraft;
      timeSpan = time.ElapsedGameTime;
      int totalMilliseconds1 = (int) timeSpan.TotalMilliseconds;
      this._timeUntilCraft = timeUntilCraft - totalMilliseconds1;
      int clankEffectTimer = this._clankEffectTimer;
      timeSpan = time.ElapsedGameTime;
      int totalMilliseconds2 = (int) timeSpan.TotalMilliseconds;
      this._clankEffectTimer = clankEffectTimer - totalMilliseconds2;
      if (this._timeUntilCraft <= 0 && this._sparklingTimer > 0)
      {
        int sparklingTimer = this._sparklingTimer;
        timeSpan = time.ElapsedGameTime;
        int totalMilliseconds3 = (int) timeSpan.TotalMilliseconds;
        this._sparklingTimer = sparklingTimer - totalMilliseconds3;
      }
      else if (this._clankEffectTimer <= 0 && !this.unforging)
      {
        this._clankEffectTimer = 450;
        if (this.rightIngredientSpot.item != null && (int) (NetFieldBase<int, NetInt>) this.rightIngredientSpot.item.parentSheetIndex == 74)
        {
          Rectangle bounds = this.rightIngredientSpot.bounds;
          bounds.Inflate(-16, -16);
          Utility.getRandomPositionInThisRectangle(bounds, Game1.random);
          int num3 = 30;
          for (int index = 0; index < num3; ++index)
            this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(114, 48, 2, 2), Utility.getRandomPositionInThisRectangle(bounds, Game1.random), false, 0.0f, Color.White)
            {
              motion = new Vector2(-4f, 0.0f),
              yPeriodic = true,
              yPeriodicRange = 16f,
              yPeriodicLoopTime = 1200f,
              scale = 4f,
              layerDepth = 1f,
              animationLength = 12,
              interval = (float) Game1.random.Next(20, 40),
              totalNumberOfLoops = 1,
              delayBeforeAnimationStart = this._clankEffectTimer / num3 * index
            });
        }
        else
        {
          Game1.playSound("crafting");
          Game1.playSound("clank");
          Rectangle bounds = this.leftIngredientSpot.bounds;
          bounds.Inflate(-21, -21);
          Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(bounds, Game1.random);
          this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(114, 46, 2, 2), positionInThisRectangle, false, 0.015f, Color.White)
          {
            motion = new Vector2(-1f, -10f),
            acceleration = new Vector2(0.0f, 0.6f),
            scale = 4f,
            layerDepth = 1f,
            animationLength = 12,
            interval = 30f,
            totalNumberOfLoops = 1
          });
          this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(114, 46, 2, 2), positionInThisRectangle, false, 0.015f, Color.White)
          {
            motion = new Vector2(0.0f, -8f),
            acceleration = new Vector2(0.0f, 0.48f),
            scale = 4f,
            layerDepth = 1f,
            animationLength = 12,
            interval = 30f,
            totalNumberOfLoops = 1
          });
          this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(114, 46, 2, 2), positionInThisRectangle, false, 0.015f, Color.White)
          {
            motion = new Vector2(1f, -10f),
            acceleration = new Vector2(0.0f, 0.6f),
            scale = 4f,
            layerDepth = 1f,
            animationLength = 12,
            interval = 30f,
            totalNumberOfLoops = 1
          });
          this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(114, 46, 2, 2), positionInThisRectangle, false, 0.015f, Color.White)
          {
            motion = new Vector2(-2f, -8f),
            acceleration = new Vector2(0.0f, 0.6f),
            scale = 2f,
            layerDepth = 1f,
            animationLength = 12,
            interval = 30f,
            totalNumberOfLoops = 1
          });
          this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(114, 46, 2, 2), positionInThisRectangle, false, 0.015f, Color.White)
          {
            motion = new Vector2(2f, -8f),
            acceleration = new Vector2(0.0f, 0.6f),
            scale = 2f,
            layerDepth = 1f,
            animationLength = 12,
            interval = 30f,
            totalNumberOfLoops = 1
          });
        }
      }
      if (this._timeUntilCraft > 0 || this._sparklingTimer > 0)
        return;
      if (this.unforging)
      {
        if (this.leftIngredientSpot.item is MeleeWeapon)
        {
          MeleeWeapon meleeWeapon = this.leftIngredientSpot.item as MeleeWeapon;
          int num4 = 0;
          if (meleeWeapon != null)
          {
            int totalForgeLevels = meleeWeapon.GetTotalForgeLevels(true);
            for (int level = 0; level < totalForgeLevels; ++level)
              num4 += this.GetForgeCostAtLevel(level);
            if (meleeWeapon.hasEnchantmentOfType<DiamondEnchantment>())
              num4 += this.GetForgeCost(this.leftIngredientSpot.item, (Item) new StardewValley.Object(72, 1));
            for (int index = meleeWeapon.enchantments.Count - 1; index >= 0; --index)
            {
              if (meleeWeapon.enchantments[index].IsForge())
                meleeWeapon.RemoveEnchantment(meleeWeapon.enchantments[index]);
            }
            if (meleeWeapon.appearance.Value >= 0)
            {
              meleeWeapon.appearance.Value = -1;
              meleeWeapon.IndexOfMenuItemView = meleeWeapon.getDrawnItemIndex();
              num4 += 10;
            }
            this.leftIngredientSpot.item = (Item) null;
            Game1.playSound("coin");
            this.heldItem = (Item) meleeWeapon;
          }
          Utility.CollectOrDrop((Item) new StardewValley.Object(848, num4 / 2));
        }
        else if (this.leftIngredientSpot.item is CombinedRing)
        {
          if (this.leftIngredientSpot.item is CombinedRing combinedRing)
          {
            List<Ring> ringList = new List<Ring>((IEnumerable<Ring>) combinedRing.combinedRings);
            combinedRing.combinedRings.Clear();
            foreach (Item obj in ringList)
              Utility.CollectOrDrop(obj);
            this.leftIngredientSpot.item = (Item) null;
            Game1.playSound("coin");
          }
          Utility.CollectOrDrop((Item) new StardewValley.Object(848, 10));
        }
        this.unforging = false;
        this._timeUntilCraft = 0;
        this._ValidateCraft();
      }
      else
      {
        Game1.player.removeItemsFromInventory(848, this.GetForgeCost(this.leftIngredientSpot.item, this.rightIngredientSpot.item));
        Item i = this.CraftItem(this.leftIngredientSpot.item, this.rightIngredientSpot.item, true);
        if (i != null && !Utility.canItemBeAddedToThisInventoryList(i, this.inventory.actualInventory))
        {
          Game1.playSound("cancel");
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
          this._timeUntilCraft = 0;
        }
        else
        {
          if (this.leftIngredientSpot.item == i)
            this.leftIngredientSpot.item = (Item) null;
          else
            this.SpendLeftItem();
          this.SpendRightItem();
          Game1.playSound("coin");
          this.heldItem = i;
          this._timeUntilCraft = 0;
          this._ValidateCraft();
        }
      }
    }

    public virtual bool IsValidUnforge(bool ignore_right_slot_occupancy = false) => (ignore_right_slot_occupancy || this.rightIngredientSpot.item == null) && (this.leftIngredientSpot.item != null && this.leftIngredientSpot.item is MeleeWeapon && ((this.leftIngredientSpot.item as MeleeWeapon).GetTotalForgeLevels() > 0 || (this.leftIngredientSpot.item as MeleeWeapon).appearance.Value >= 0) || this.leftIngredientSpot.item != null && this.leftIngredientSpot.item is CombinedRing);

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
      Game1.DrawBox(this.xPositionOnScreen - 64, this.yPositionOnScreen + 128, 128, 201, new Color?(new Color(116, 11, 3)));
      Game1.player.FarmerRenderer.drawMiniPortrat(b, new Vector2((float) (this.xPositionOnScreen - 64) + 9.6f, (float) (this.yPositionOnScreen + 128)), 0.87f, 4f, 2, Game1.player);
      this.draw(b, true, true, 116, 11, 3);
      b.Draw(this.forgeTextures, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 - 4), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder)), new Rectangle?(new Rectangle(0, 0, 142, 80)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      Color color = Color.White;
      if (this._craftState == ForgeMenu.CraftState.MissingShards)
        color = Color.Gray * 0.75f;
      b.Draw(this.forgeTextures, new Vector2((float) (this.xPositionOnScreen + 276), (float) (this.yPositionOnScreen + 300)), new Rectangle?(new Rectangle(142, 16, 17, 17)), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
      if (this.leftIngredientSpot.item != null && this.rightIngredientSpot.item != null && this.IsValidCraft(this.leftIngredientSpot.item, this.rightIngredientSpot.item))
      {
        int num = (this.GetForgeCost(this.leftIngredientSpot.item, this.rightIngredientSpot.item) - 10) / 5;
        switch (num)
        {
          case 0:
          case 1:
          case 2:
            b.Draw(this.forgeTextures, new Vector2((float) (this.xPositionOnScreen + 344), (float) (this.yPositionOnScreen + 320)), new Rectangle?(new Rectangle(142, 38 + num * 10, 17, 10)), Color.White * (this._craftState == ForgeMenu.CraftState.MissingShards ? 0.5f : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
            break;
        }
      }
      if (this.IsValidUnforge())
        b.Draw(this.forgeTextures, new Vector2((float) this.unforgeButton.bounds.X, (float) this.unforgeButton.bounds.Y), new Rectangle?(new Rectangle(143, 69, 11, 10)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
      if (this._craftState == ForgeMenu.CraftState.Valid)
      {
        this.startTailoringButton.draw(b, Color.White, 0.96f, (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 200 % 12);
        this.startTailoringButton.drawItem(b, 16, 16);
      }
      Point point = new Point(0, 0);
      bool flag1 = false;
      bool flag2 = false;
      Item obj = this.hoveredItem;
      if (this.heldItem != null)
        obj = this.heldItem;
      if (obj != null && obj != this.leftIngredientSpot.item && obj != this.rightIngredientSpot.item && obj != this.craftResultDisplay.item)
      {
        if (obj is Tool)
        {
          if (this.leftIngredientSpot.item is Tool)
            flag2 = true;
          else
            flag1 = true;
        }
        if (BaseEnchantment.GetEnchantmentFromItem(this.leftIngredientSpot.item, obj) != null)
          flag2 = true;
        if (obj is Ring && !(obj is CombinedRing) && (this.leftIngredientSpot.item == null || this.leftIngredientSpot.item is Ring) && (this.rightIngredientSpot.item == null || this.rightIngredientSpot.item is Ring))
        {
          flag1 = true;
          flag2 = true;
        }
      }
      foreach (ClickableComponent equipmentIcon in this.equipmentIcons)
      {
        string name = equipmentIcon.name;
        if (!(name == "Ring1"))
        {
          if (name == "Ring2")
          {
            if (Game1.player.rightRing.Value != null)
            {
              b.Draw(this.forgeTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(0, 96, 16, 16)), Color.White);
              float transparency = 1f;
              if (!this.HighlightItems((Item) (Ring) (NetFieldBase<Ring, NetRef<Ring>>) Game1.player.rightRing))
                transparency = 0.5f;
              if (Game1.player.rightRing.Value == this.heldItem)
                transparency = 0.5f;
              Game1.player.rightRing.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale, transparency, 0.866f, StackDrawType.Hide);
            }
            else
              b.Draw(this.forgeTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(16, 96, 16, 16)), Color.White);
          }
        }
        else if (Game1.player.leftRing.Value != null)
        {
          b.Draw(this.forgeTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(0, 96, 16, 16)), Color.White);
          float transparency = 1f;
          if (!this.HighlightItems((Item) (Ring) (NetFieldBase<Ring, NetRef<Ring>>) Game1.player.leftRing))
            transparency = 0.5f;
          if (Game1.player.leftRing.Value == this.heldItem)
            transparency = 0.5f;
          Game1.player.leftRing.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale, transparency, 0.866f, StackDrawType.Hide);
        }
        else
          b.Draw(this.forgeTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(16, 96, 16, 16)), Color.White);
      }
      if (!this.IsBusy())
      {
        if (flag1)
          this.leftIngredientSpot.draw(b, Color.White, 0.87f);
      }
      else if (this._clankEffectTimer > 300 || this._timeUntilCraft > 0 && this.unforging)
      {
        point.X = Game1.random.Next(-1, 2);
        point.Y = Game1.random.Next(-1, 2);
      }
      this.leftIngredientSpot.drawItem(b, point.X * 4, point.Y * 4);
      if (this.craftResultDisplay.visible)
      {
        string text = Game1.content.LoadString("Strings\\UI:Tailor_MakeResult");
        Vector2 position = new Vector2((float) this.craftResultDisplay.bounds.Center.X - Game1.smallFont.MeasureString(text).X / 2f, (float) this.craftResultDisplay.bounds.Top - Game1.smallFont.MeasureString(text).Y);
        Utility.drawTextWithColoredShadow(b, text, Game1.smallFont, position, Game1.textColor * 0.75f, Color.Black * 0.2f);
        if (this.craftResultDisplay.item != null)
          this.craftResultDisplay.drawItem(b);
      }
      if (!this.IsBusy() && flag2)
        this.rightIngredientSpot.draw(b, Color.White, 0.87f);
      this.rightIngredientSpot.drawItem(b);
      foreach (TemporaryAnimatedSprite tempSprite in this.tempSprites)
        tempSprite.draw(b, true);
      if (!this.hoverText.Equals(""))
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, this.heldItem != null ? 32 : 0, this.heldItem != null ? 32 : 0);
      else if (this.hoveredItem != null)
      {
        if (this.hoveredItem == this.craftResultDisplay.item && Utility.IsNormalObjectAtParentSheetIndex(this.rightIngredientSpot.item, 74))
          BaseEnchantment.hideEnchantmentName = true;
        IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem, this.heldItem != null);
        BaseEnchantment.hideEnchantmentName = false;
      }
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
        Utility.CollectOrDrop(this.heldItem, 2);
      if (!Game1.player.IsEquippedItem(this.leftIngredientSpot.item))
        Utility.CollectOrDrop(this.leftIngredientSpot.item, 2);
      if (!Game1.player.IsEquippedItem(this.rightIngredientSpot.item))
        Utility.CollectOrDrop(this.rightIngredientSpot.item, 2);
      if (!Game1.player.IsEquippedItem(this.startTailoringButton.item))
        Utility.CollectOrDrop(this.startTailoringButton.item, 2);
      this.heldItem = (Item) null;
      this.leftIngredientSpot.item = (Item) null;
      this.rightIngredientSpot.item = (Item) null;
      this.startTailoringButton.item = (Item) null;
    }

    public enum CraftState
    {
      MissingIngredients,
      MissingShards,
      Valid,
      InvalidRecipe,
    }
  }
}
