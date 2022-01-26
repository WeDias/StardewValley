// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.DyeMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class DyeMenu : MenuWithInventory
  {
    protected int _timeUntilCraft;
    public List<ClickableTextureComponent> dyePots;
    public ClickableTextureComponent dyeButton;
    public const int DYE_POT_ID_OFFSET = 5000;
    public Texture2D dyeTexture;
    protected Dictionary<Item, int> _highlightDictionary;
    protected Dictionary<string, Item> _lastValidEquippedItems;
    protected bool _shouldPrismaticDye;
    protected List<Vector2> _slotDrawPositions;
    protected int _hoveredPotIndex = -1;
    protected int[] _dyeDropAnimationFrames;
    public const int MILLISECONDS_PER_DROP_FRAME = 50;
    public const int TOTAL_DROP_FRAMES = 10;
    public string[][] validPotColors = new string[6][]
    {
      new string[4]
      {
        "color_red",
        "color_salmon",
        "color_dark_red",
        "color_pink"
      },
      new string[5]
      {
        "color_orange",
        "color_dark_orange",
        "color_dark_brown",
        "color_brown",
        "color_copper"
      },
      new string[4]
      {
        "color_yellow",
        "color_dark_yellow",
        "color_gold",
        "color_sand"
      },
      new string[5]
      {
        "color_green",
        "color_dark_green",
        "color_lime",
        "color_yellow_green",
        "color_jade"
      },
      new string[6]
      {
        "color_blue",
        "color_dark_blue",
        "color_dark_cyan",
        "color_light_cyan",
        "color_cyan",
        "color_aquamarine"
      },
      new string[6]
      {
        "color_purple",
        "color_dark_purple",
        "color_dark_pink",
        "color_pale_violet_red",
        "color_poppyseed",
        "color_iridium"
      }
    };
    protected bool _heldItemIsEquipped;
    protected string displayedDescription = "";
    public List<ClickableTextureComponent> dyedClothesDisplays;
    protected Vector2 _dyedClothesDisplayPosition;

    public DyeMenu()
      : base(okButton: true, trashCan: true, inventoryXOffset: 12, inventoryYOffset: 132)
    {
      if (this.yPositionOnScreen == IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder)
        this.movePosition(0, -IClickableMenu.spaceToClearTopBorder);
      Game1.playSound("bigSelect");
      this.inventory.highlightMethod = new InventoryMenu.highlightThisItem(this.HighlightItems);
      this.dyeTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\dye_bench");
      this.dyedClothesDisplays = new List<ClickableTextureComponent>();
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
      this.GenerateHighlightDictionary();
      this._UpdateDescriptionText();
    }

    protected void _CreateButtons()
    {
      this._slotDrawPositions = this.inventory.GetSlotDrawPositions();
      Dictionary<int, Item> dictionary = new Dictionary<int, Item>();
      if (this.dyePots != null)
      {
        for (int index = 0; index < this.dyePots.Count; ++index)
          dictionary[index] = this.dyePots[index].item;
      }
      this.dyePots = new List<ClickableTextureComponent>();
      for (int key = 0; key < this.validPotColors.Length; ++key)
      {
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 - 4 + 68 + 18 * key * 4, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 132, 64, 64), this.dyeTexture, new Rectangle(32 + 16 * key, 80, 16, 16), 4f);
        textureComponent.myID = key + 5000;
        textureComponent.downNeighborID = -99998;
        textureComponent.leftNeighborID = -99998;
        textureComponent.rightNeighborID = -99998;
        textureComponent.upNeighborID = -99998;
        textureComponent.item = dictionary.ContainsKey(key) ? dictionary[key] : (Item) null;
        this.dyePots.Add(textureComponent);
      }
      this._dyeDropAnimationFrames = new int[this.dyePots.Count];
      for (int index = 0; index < this._dyeDropAnimationFrames.Length; ++index)
        this._dyeDropAnimationFrames[index] = -1;
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 448, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 200, 96, 96), this.dyeTexture, new Rectangle(0, 80, 24, 24), 4f);
      textureComponent1.myID = 1000;
      textureComponent1.downNeighborID = -99998;
      textureComponent1.leftNeighborID = -99998;
      textureComponent1.rightNeighborID = -99998;
      textureComponent1.upNeighborID = -99998;
      textureComponent1.item = this.dyeButton != null ? this.dyeButton.item : (Item) null;
      this.dyeButton = textureComponent1;
      if (this.inventory.inventory != null && this.inventory.inventory.Count >= 12)
      {
        for (int index = 0; index < 12; ++index)
        {
          if (this.inventory.inventory[index] != null)
            this.inventory.inventory[index].upNeighborID = -99998;
        }
      }
      this.dyedClothesDisplays.Clear();
      this._dyedClothesDisplayPosition = new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 692), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 232));
      Vector2 clothesDisplayPosition = this._dyedClothesDisplayPosition;
      int num = 0;
      if (Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value)
        ++num;
      if (Game1.player.pantsItem.Value != null && Game1.player.pantsItem.Value.dyeable.Value)
        ++num;
      clothesDisplayPosition.X -= (float) (num * 64 / 2);
      if (Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value)
      {
        ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle((int) clothesDisplayPosition.X, (int) clothesDisplayPosition.Y, 64, 64), (Texture2D) null, new Rectangle(0, 0, 64, 64), 4f);
        textureComponent2.item = (Item) Game1.player.shirtItem.Value;
        clothesDisplayPosition.X += 64f;
        this.dyedClothesDisplays.Add(textureComponent2);
      }
      if (Game1.player.pantsItem.Value == null || !Game1.player.pantsItem.Value.dyeable.Value)
        return;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle((int) clothesDisplayPosition.X, (int) clothesDisplayPosition.Y, 64, 64), (Texture2D) null, new Rectangle(0, 0, 64, 64), 4f);
      textureComponent3.item = (Item) Game1.player.pantsItem.Value;
      clothesDisplayPosition.X += 64f;
      this.dyedClothesDisplays.Add(textureComponent3);
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
      if (i == null || i != null && !i.canBeTrashed())
        return false;
      if (this._highlightDictionary == null)
        this.GenerateHighlightDictionary();
      if (!this._highlightDictionary.ContainsKey(i))
      {
        this._highlightDictionary = (Dictionary<Item, int>) null;
        this.GenerateHighlightDictionary();
      }
      if (this._hoveredPotIndex >= 0)
        return this._hoveredPotIndex == this._highlightDictionary[i];
      return this._highlightDictionary[i] >= 0 && this.dyePots[this._highlightDictionary[i]].item == null;
    }

    public void GenerateHighlightDictionary()
    {
      this._highlightDictionary = new Dictionary<Item, int>();
      foreach (Item key in new List<Item>((IEnumerable<Item>) this.inventory.actualInventory))
      {
        if (key != null)
          this._highlightDictionary[key] = this.GetPotIndex(key);
      }
    }

    private void _DyePotClicked(ClickableTextureComponent dye_pot)
    {
      Item obj = dye_pot.item;
      int index1 = this.dyePots.IndexOf(dye_pot);
      if (index1 < 0)
        return;
      if (this.heldItem == null || this.heldItem.canBeTrashed() && this.GetPotIndex(this.heldItem) == index1)
      {
        bool flag = false;
        if (dye_pot.item != null && this.heldItem != null && dye_pot.item.canStackWith((ISalable) this.heldItem))
        {
          ++this.heldItem.Stack;
          dye_pot.item = (Item) null;
          Game1.playSound("quickSlosh");
          return;
        }
        dye_pot.item = this.heldItem == null ? (Item) null : this.heldItem.getOne();
        if (this.heldItem != null)
        {
          int stack = this.heldItem.Stack;
          --this.heldItem.Stack;
          if (stack == this.heldItem.Stack && stack == 1)
            flag = true;
        }
        if (this.heldItem != null && this.heldItem.Stack <= 0 | flag)
          this.heldItem = obj;
        else if (this.heldItem != null && obj != null)
        {
          Item inventory = Game1.player.addItemToInventory(this.heldItem);
          if (inventory != null)
            Game1.createItemDebris(inventory, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
          this.heldItem = obj;
        }
        else if (obj != null)
          this.heldItem = obj;
        else if (this.heldItem != null && obj == null && Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift))
        {
          Game1.player.addItemToInventory(this.heldItem);
          this.heldItem = (Item) null;
        }
        if (obj != dye_pot.item)
        {
          this._dyeDropAnimationFrames[index1] = 0;
          Game1.playSound("quickSlosh");
          int num = 0;
          for (int index2 = 0; index2 < this.dyePots.Count; ++index2)
          {
            if (this.dyePots[index2].item != null)
              ++num;
          }
          if (num >= this.dyePots.Count)
            DelayedAction.playSoundAfterDelay("newArtifact", 200);
        }
        this._highlightDictionary = (Dictionary<Item, int>) null;
        this.GenerateHighlightDictionary();
      }
      this._UpdateDescriptionText();
    }

    public Color GetColorForPot(int index)
    {
      switch (index)
      {
        case 0:
          return new Color(220, 0, 0);
        case 1:
          return new Color((int) byte.MaxValue, 128, 0);
        case 2:
          return new Color((int) byte.MaxValue, 230, 0);
        case 3:
          return new Color(10, 143, 0);
        case 4:
          return new Color(46, 105, 203);
        case 5:
          return new Color(115, 41, 181);
        default:
          return Color.Black;
      }
    }

    public int GetPotIndex(Item item)
    {
      for (int potIndex = 0; potIndex < this.validPotColors.Length; ++potIndex)
      {
        for (int index = 0; index < this.validPotColors[potIndex].Length; ++index)
        {
          if (item.HasContextTag(this.validPotColors[potIndex][index]))
            return potIndex;
        }
      }
      return -1;
    }

    public override void receiveKeyPress(Keys key)
    {
      if (key == Keys.Delete)
      {
        if (this.heldItem == null || !this.heldItem.canBeTrashed())
          return;
        Utility.trashItem(this.heldItem);
        this.heldItem = (Item) null;
      }
      else
        base.receiveKeyPress(key);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      Item heldItem = this.heldItem;
      Game1.player.IsEquippedItem(heldItem);
      int x1 = x;
      int y1 = y;
      KeyboardState keyboardState;
      int num;
      if (this.heldItem == null)
      {
        keyboardState = Game1.GetKeyboardState();
        num = !keyboardState.IsKeyDown(Keys.LeftShift) ? 1 : 0;
      }
      else
        num = 1;
      base.receiveLeftClick(x1, y1, num != 0);
      keyboardState = Game1.GetKeyboardState();
      if (keyboardState.IsKeyDown(Keys.LeftShift) && heldItem != this.heldItem && this.heldItem != null)
      {
        foreach (ClickableTextureComponent dyePot in this.dyePots)
        {
          if (dyePot.item == null)
            this._DyePotClicked(dyePot);
          if (this.heldItem == null)
            return;
        }
      }
      if (this.IsBusy())
        return;
      bool flag = this.heldItem != null;
      foreach (ClickableTextureComponent dyePot in this.dyePots)
      {
        if (dyePot.containsPoint(x, y))
        {
          this._DyePotClicked(dyePot);
          if (flag || this.heldItem == null)
            return;
          keyboardState = Game1.GetKeyboardState();
          if (!keyboardState.IsKeyDown(Keys.LeftShift))
            return;
          this.heldItem = Game1.player.addItemToInventory(this.heldItem);
          return;
        }
      }
      if (this.dyeButton.containsPoint(x, y))
      {
        if (this.heldItem == null && this.CanDye())
        {
          Game1.playSound("glug");
          for (int index = 0; index < this.dyePots.Count; ++index)
          {
            if (this.dyePots[index].item != null)
            {
              --this.dyePots[index].item.Stack;
              if (this.dyePots[index].item.Stack <= 0)
                this.dyePots[index].item = (Item) null;
            }
          }
          Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(CharacterCustomization.Source.DyePots);
          this._UpdateDescriptionText();
        }
        else
          Game1.playSound("sell");
      }
      if (this.heldItem == null || this.isWithinBounds(x, y) || !this.heldItem.canBeTrashed())
        return;
      Game1.playSound("throwDownITem");
      Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
      this.heldItem = (Item) null;
    }

    public bool CanDye()
    {
      for (int index = 0; index < this.dyePots.Count; ++index)
      {
        if (this.dyePots[index].item == null)
          return false;
      }
      return true;
    }

    protected virtual bool CheckHeldItem(Func<Item, bool> f = null) => f == null ? this.heldItem != null : f(this.heldItem);

    public static bool IsWearingDyeable() => Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value || Game1.player.pantsItem.Value != null && Game1.player.pantsItem.Value.dyeable.Value;

    protected void _UpdateDescriptionText()
    {
      if (!DyeMenu.IsWearingDyeable())
        this.displayedDescription = Game1.content.LoadString("Strings\\UI:DyePot_NoDyeable");
      else if (this.CanDye())
        this.displayedDescription = Game1.content.LoadString("Strings\\UI:DyePot_CanDye");
      else
        this.displayedDescription = Game1.content.LoadString("Strings\\UI:DyePot_Help");
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      if (this.IsBusy())
        return;
      base.receiveRightClick(x, y, true);
    }

    public override void performHoverAction(int x, int y)
    {
      if (x <= this.dyePots[0].bounds.X || x >= this.dyePots.Last<ClickableTextureComponent>().bounds.Right || y <= this.dyePots[0].bounds.Y || y >= this.dyePots[0].bounds.Bottom)
        this._hoveredPotIndex = -1;
      if (this.IsBusy())
        return;
      this.hoveredItem = (Item) null;
      base.performHoverAction(x, y);
      this.hoverText = "";
      foreach (ClickableTextureComponent dyedClothesDisplay in this.dyedClothesDisplays)
      {
        if (dyedClothesDisplay.containsPoint(x, y))
          this.hoveredItem = dyedClothesDisplay.item;
      }
      for (int index = 0; index < this.dyePots.Count; ++index)
      {
        if (this.dyePots[index].containsPoint(x, y))
        {
          this.dyePots[index].tryHover(x, y, 0.0f);
          this._hoveredPotIndex = index;
        }
      }
      if (!this.CanDye())
        return;
      this.dyeButton.tryHover(x, y, 0.2f);
    }

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
      this.descriptionText = this.displayedDescription;
      if (this.CanDye())
      {
        this.dyeButton.sourceRect.Y = 180;
        this.dyeButton.sourceRect.X = (int) (time.TotalGameTime.TotalMilliseconds % 600.0 / 100.0) * 24;
      }
      else
      {
        this.dyeButton.sourceRect.Y = 80;
        this.dyeButton.sourceRect.X = 0;
      }
      for (int index = 0; index < this.dyePots.Count; ++index)
      {
        if (this._dyeDropAnimationFrames[index] >= 0)
        {
          this._dyeDropAnimationFrames[index] += time.ElapsedGameTime.Milliseconds;
          if (this._dyeDropAnimationFrames[index] >= 500)
            this._dyeDropAnimationFrames[index] = -1;
        }
      }
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
      this.draw(b, true, true, 50, 160, (int) byte.MaxValue);
      b.Draw(this.dyeTexture, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 - 4), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder)), new Rectangle?(new Rectangle(0, 0, 142, 80)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      for (int index = 0; index < this._slotDrawPositions.Count; ++index)
      {
        if (index < this.inventory.actualInventory.Count && this.inventory.actualInventory[index] != null && this._highlightDictionary.ContainsKey(this.inventory.actualInventory[index]))
        {
          int highlight = this._highlightDictionary[this.inventory.actualInventory[index]];
          if (highlight >= 0)
          {
            Color colorForPot = this.GetColorForPot(highlight);
            if (this._hoveredPotIndex == -1 && this.HighlightItems(this.inventory.actualInventory[index]))
              b.Draw(this.dyeTexture, this._slotDrawPositions[index], new Rectangle?(new Rectangle(32, 96, 32, 32)), colorForPot, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
          }
        }
      }
      this.dyeButton.draw(b, Color.White * (this.CanDye() ? 1f : 0.55f), 0.96f);
      this.dyeButton.drawItem(b, 16, 16);
      string text = Game1.content.LoadString("Strings\\UI:DyePot_WillDye");
      Vector2 clothesDisplayPosition = this._dyedClothesDisplayPosition;
      Vector2 position = new Vector2(clothesDisplayPosition.X - Game1.smallFont.MeasureString(text).X / 2f, (float) (int) clothesDisplayPosition.Y - Game1.smallFont.MeasureString(text).Y);
      Utility.drawTextWithColoredShadow(b, text, Game1.smallFont, position, Game1.textColor * 0.75f, Color.Black * 0.2f);
      foreach (ClickableTextureComponent dyedClothesDisplay in this.dyedClothesDisplays)
        dyedClothesDisplay.drawItem(b);
      for (int index = 0; index < this.dyePots.Count; ++index)
      {
        this.dyePots[index].drawItem(b, yOffset: -16);
        if (this._dyeDropAnimationFrames[index] >= 0)
        {
          Color colorForPot = this.GetColorForPot(index);
          b.Draw(this.dyeTexture, new Vector2((float) this.dyePots[index].bounds.X, (float) (this.dyePots[index].bounds.Y - 12)), new Rectangle?(new Rectangle(this._dyeDropAnimationFrames[index] / 50 * 16, 128, 16, 16)), colorForPot, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
        }
        this.dyePots[index].draw(b);
      }
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
      Utility.CollectOrDrop(this.heldItem);
      for (int index = 0; index < this.dyePots.Count; ++index)
      {
        if (this.dyePots[index].item != null)
          Utility.CollectOrDrop(this.dyePots[index].item);
      }
      this.heldItem = (Item) null;
      this.dyeButton.item = (Item) null;
    }
  }
}
