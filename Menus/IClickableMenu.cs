// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.IClickableMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StardewValley.Menus
{
  [InstanceStatics]
  public abstract class IClickableMenu
  {
    protected IClickableMenu _childMenu;
    protected IClickableMenu _parentMenu;
    public const int currency_g = 0;
    public const int currency_starTokens = 1;
    public const int currency_qiCoins = 2;
    public const int currency_qiGems = 4;
    public const int greyedOutSpotIndex = 57;
    public const int outerBorderWithUpArrow = 61;
    public const int lvlMarkerRedIndex = 54;
    public const int lvlMarkerGreyIndex = 55;
    public const int borderWithDownArrowIndex = 46;
    public const int borderWithUpArrowIndex = 47;
    public const int littleHeartIndex = 49;
    public const int uncheckedBoxIndex = 50;
    public const int checkedBoxIndex = 51;
    public const int presentIconIndex = 58;
    public const int itemSpotIndex = 10;
    public static int borderWidth = 40;
    public static int tabYPositionRelativeToMenuY = -48;
    public static int spaceToClearTopBorder = 96;
    public static int spaceToClearSideBorder = 16;
    public const int spaceBetweenTabs = 4;
    public int width;
    public int height;
    public int xPositionOnScreen;
    public int yPositionOnScreen;
    public int currentRegion;
    public Action<IClickableMenu> behaviorBeforeCleanup;
    public IClickableMenu.onExit exitFunction;
    public ClickableTextureComponent upperRightCloseButton;
    public bool destroy;
    public bool gamePadControlsImplemented;
    protected int _dependencies;
    public List<ClickableComponent> allClickableComponents;
    public ClickableComponent currentlySnappedComponent;

    public IClickableMenu()
    {
    }

    public IClickableMenu(int x, int y, int width, int height, bool showUpperRightCloseButton = false)
    {
      Game1.mouseCursorTransparency = 1f;
      this.initialize(x, y, width, height, showUpperRightCloseButton);
      if (Game1.gameMode != (byte) 3 || Game1.player == null || Game1.eventUp)
        return;
      Game1.player.Halt();
    }

    public void initialize(int x, int y, int width, int height, bool showUpperRightCloseButton = false)
    {
      if (Game1.player != null && !Game1.player.UsingTool && !Game1.eventUp)
        Game1.player.forceCanMove();
      this.xPositionOnScreen = x;
      this.yPositionOnScreen = y;
      this.width = width;
      this.height = height;
      if (showUpperRightCloseButton)
        this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width - 36, this.yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
      for (int index = 0; index < 4; ++index)
        Game1.directionKeyPolling[index] = 250;
    }

    public virtual bool HasFocus() => Game1.activeClickableMenu == this;

    public IClickableMenu GetChildMenu() => this._childMenu;

    public IClickableMenu GetParentMenu() => this._parentMenu;

    public void SetChildMenu(IClickableMenu menu)
    {
      this._childMenu = menu;
      if (this._childMenu == null)
        return;
      this._childMenu._parentMenu = this;
    }

    public void AddDependency() => ++this._dependencies;

    public void RemoveDependency()
    {
      --this._dependencies;
      if (this._dependencies > 0 || Game1.activeClickableMenu == this || TitleMenu.subMenu == this || !(this is IDisposable))
        return;
      (this as IDisposable).Dispose();
    }

    public bool HasDependencies() => this._dependencies > 0;

    public virtual bool areGamePadControlsImplemented() => false;

    public ClickableComponent getLastClickableComponentInThisListThatContainsThisXCoord(
      List<ClickableComponent> ccList,
      int xCoord)
    {
      for (int index = ccList.Count - 1; index >= 0; --index)
      {
        if (ccList[index].bounds.Contains(xCoord, ccList[index].bounds.Center.Y))
          return ccList[index];
      }
      return (ClickableComponent) null;
    }

    public ClickableComponent getFirstClickableComponentInThisListThatContainsThisXCoord(
      List<ClickableComponent> ccList,
      int xCoord)
    {
      for (int index = 0; index < ccList.Count; ++index)
      {
        if (ccList[index].bounds.Contains(xCoord, ccList[index].bounds.Center.Y))
          return ccList[index];
      }
      return (ClickableComponent) null;
    }

    public ClickableComponent getLastClickableComponentInThisListThatContainsThisYCoord(
      List<ClickableComponent> ccList,
      int yCoord)
    {
      for (int index = ccList.Count - 1; index >= 0; --index)
      {
        if (ccList[index].bounds.Contains(ccList[index].bounds.Center.X, yCoord))
          return ccList[index];
      }
      return (ClickableComponent) null;
    }

    public ClickableComponent getFirstClickableComponentInThisListThatContainsThisYCoord(
      List<ClickableComponent> ccList,
      int yCoord)
    {
      for (int index = 0; index < ccList.Count; ++index)
      {
        if (ccList[index].bounds.Contains(ccList[index].bounds.Center.X, yCoord))
          return ccList[index];
      }
      return (ClickableComponent) null;
    }

    public virtual void receiveGamePadButton(Buttons b)
    {
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls || b == Buttons.A)
        ;
    }

    public void drawMouse(SpriteBatch b, bool ignore_transparency = false, int cursor = -1)
    {
      if (Game1.options.hardwareCursor)
        return;
      float num = Game1.mouseCursorTransparency;
      if (ignore_transparency)
        num = 1f;
      if (cursor < 0)
        cursor = !Game1.options.snappyMenus || !Game1.options.gamepadControls ? 0 : 44;
      b.Draw(Game1.mouseCursors, new Vector2((float) Game1.getMouseX(), (float) Game1.getMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, cursor, 16, 16)), Color.White * num, 0.0f, Vector2.Zero, (float) (4.0 + (double) Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
    }

    public void populateClickableComponentList()
    {
      this.allClickableComponents = new List<ClickableComponent>();
      foreach (FieldInfo field in this.GetType().GetFields())
      {
        if (field.GetCustomAttributes(typeof (SkipForClickableAggregation), true).Length == 0 && !(field.DeclaringType == typeof (IClickableMenu)))
        {
          if (field.FieldType.IsSubclassOf(typeof (ClickableComponent)) || field.FieldType == typeof (ClickableComponent))
          {
            if (field.GetValue((object) this) != null)
              this.allClickableComponents.Add((ClickableComponent) field.GetValue((object) this));
          }
          else if (field.FieldType == typeof (List<ClickableComponent>))
          {
            List<ClickableComponent> clickableComponentList = (List<ClickableComponent>) field.GetValue((object) this);
            if (clickableComponentList != null)
            {
              for (int index = clickableComponentList.Count - 1; index >= 0; --index)
              {
                if (clickableComponentList[index] != null)
                  this.allClickableComponents.Add(clickableComponentList[index]);
              }
            }
          }
          else if (field.FieldType == typeof (List<ClickableTextureComponent>))
          {
            List<ClickableTextureComponent> textureComponentList = (List<ClickableTextureComponent>) field.GetValue((object) this);
            if (textureComponentList != null)
            {
              for (int index = textureComponentList.Count - 1; index >= 0; --index)
              {
                if (textureComponentList[index] != null)
                  this.allClickableComponents.Add((ClickableComponent) textureComponentList[index]);
              }
            }
          }
          else if (field.FieldType == typeof (List<ClickableAnimatedComponent>))
          {
            List<ClickableAnimatedComponent> animatedComponentList = (List<ClickableAnimatedComponent>) field.GetValue((object) this);
            for (int index = animatedComponentList.Count - 1; index >= 0; --index)
            {
              if (animatedComponentList[index] != null)
                this.allClickableComponents.Add((ClickableComponent) animatedComponentList[index]);
            }
          }
          else if (field.FieldType == typeof (List<Bundle>))
          {
            List<Bundle> bundleList = (List<Bundle>) field.GetValue((object) this);
            for (int index = bundleList.Count - 1; index >= 0; --index)
            {
              if (bundleList[index] != null)
                this.allClickableComponents.Add((ClickableComponent) bundleList[index]);
            }
          }
          else if (field.FieldType == typeof (InventoryMenu))
          {
            this.allClickableComponents.AddRange((IEnumerable<ClickableComponent>) ((InventoryMenu) field.GetValue((object) this)).inventory);
            this.allClickableComponents.Add(((InventoryMenu) field.GetValue((object) this)).dropItemInvisibleButton);
          }
          else if (field.FieldType == typeof (List<Dictionary<ClickableTextureComponent, CraftingRecipe>>))
          {
            foreach (Dictionary<ClickableTextureComponent, CraftingRecipe> dictionary in (List<Dictionary<ClickableTextureComponent, CraftingRecipe>>) field.GetValue((object) this))
              this.allClickableComponents.AddRange((IEnumerable<ClickableComponent>) dictionary.Keys);
          }
          else if (field.FieldType == typeof (Dictionary<int, List<List<ClickableTextureComponent>>>))
          {
            foreach (List<List<ClickableTextureComponent>> textureComponentListList in ((Dictionary<int, List<List<ClickableTextureComponent>>>) field.GetValue((object) this)).Values)
            {
              foreach (IEnumerable<ClickableComponent> collection in textureComponentListList)
                this.allClickableComponents.AddRange(collection);
            }
          }
          else if (field.FieldType == typeof (Dictionary<int, ClickableTextureComponent>))
          {
            foreach (ClickableComponent clickableComponent in ((Dictionary<int, ClickableTextureComponent>) field.GetValue((object) this)).Values)
              this.allClickableComponents.Add(clickableComponent);
          }
        }
      }
      if (Game1.activeClickableMenu is GameMenu activeClickableMenu && this == activeClickableMenu.GetCurrentPage())
        activeClickableMenu.AddTabsToClickableComponents(this);
      if (this.upperRightCloseButton == null)
        return;
      this.allClickableComponents.Add((ClickableComponent) this.upperRightCloseButton);
    }

    public virtual void applyMovementKey(int direction)
    {
      if (this.allClickableComponents == null)
        this.populateClickableComponentList();
      this.moveCursorInDirection(direction);
    }

    /// <summary>
    /// return true if this method is overriden and a default clickablecomponent is snapped to.
    /// </summary>
    /// <returns></returns>
    public virtual void snapToDefaultClickableComponent()
    {
    }

    public void applyMovementKey(Keys key)
    {
      if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
        this.applyMovementKey(0);
      else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
        this.applyMovementKey(1);
      else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
      {
        this.applyMovementKey(2);
      }
      else
      {
        if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
          return;
        this.applyMovementKey(3);
      }
    }

    /// <summary>Only use this if the child class overrides</summary>
    /// <param name="id"></param>
    public virtual void setCurrentlySnappedComponentTo(int id) => this.currentlySnappedComponent = this.getComponentWithID(id);

    public void moveCursorInDirection(int direction)
    {
      if (this.currentlySnappedComponent == null && this.allClickableComponents != null && this.allClickableComponents.Count<ClickableComponent>() > 0)
      {
        this.snapToDefaultClickableComponent();
        if (this.currentlySnappedComponent == null)
          this.currentlySnappedComponent = this.allClickableComponents.First<ClickableComponent>();
      }
      if (this.currentlySnappedComponent == null)
        return;
      ClickableComponent snappedComponent = this.currentlySnappedComponent;
      switch (direction)
      {
        case 0:
          if (this.currentlySnappedComponent.upNeighborID == -99999)
            this.snapToDefaultClickableComponent();
          else if (this.currentlySnappedComponent.upNeighborID == -99998)
            this.automaticSnapBehavior(0, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
          else if (this.currentlySnappedComponent.upNeighborID == -7777)
            this.customSnapBehavior(0, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
          else
            this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.upNeighborID);
          if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.upNeighborID != -7777 && snappedComponent.upNeighborID != -99998) && !this.currentlySnappedComponent.downNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable)
            this.currentlySnappedComponent.downNeighborID = snappedComponent.myID;
          if (this.currentlySnappedComponent == null)
          {
            this.noSnappedComponentFound(0, snappedComponent.region, snappedComponent.myID);
            break;
          }
          break;
        case 1:
          if (this.currentlySnappedComponent.rightNeighborID == -99999)
            this.snapToDefaultClickableComponent();
          else if (this.currentlySnappedComponent.rightNeighborID == -99998)
            this.automaticSnapBehavior(1, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
          else if (this.currentlySnappedComponent.rightNeighborID == -7777)
            this.customSnapBehavior(1, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
          else
            this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.rightNeighborID);
          if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.rightNeighborID != -7777 && snappedComponent.rightNeighborID != -99998) && !this.currentlySnappedComponent.leftNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable)
            this.currentlySnappedComponent.leftNeighborID = snappedComponent.myID;
          if (this.currentlySnappedComponent == null && snappedComponent.tryDefaultIfNoRightNeighborExists)
          {
            this.snapToDefaultClickableComponent();
            break;
          }
          if (this.currentlySnappedComponent == null)
          {
            this.noSnappedComponentFound(1, snappedComponent.region, snappedComponent.myID);
            break;
          }
          break;
        case 2:
          if (this.currentlySnappedComponent.downNeighborID == -99999)
            this.snapToDefaultClickableComponent();
          else if (this.currentlySnappedComponent.downNeighborID == -99998)
            this.automaticSnapBehavior(2, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
          else if (this.currentlySnappedComponent.downNeighborID == -7777)
            this.customSnapBehavior(2, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
          else
            this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.downNeighborID);
          if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.downNeighborID != -7777 && snappedComponent.downNeighborID != -99998) && !this.currentlySnappedComponent.upNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable)
            this.currentlySnappedComponent.upNeighborID = snappedComponent.myID;
          if (this.currentlySnappedComponent == null && snappedComponent.tryDefaultIfNoDownNeighborExists)
          {
            this.snapToDefaultClickableComponent();
            break;
          }
          if (this.currentlySnappedComponent == null)
          {
            this.noSnappedComponentFound(2, snappedComponent.region, snappedComponent.myID);
            break;
          }
          break;
        case 3:
          if (this.currentlySnappedComponent.leftNeighborID == -99999)
            this.snapToDefaultClickableComponent();
          else if (this.currentlySnappedComponent.leftNeighborID == -99998)
            this.automaticSnapBehavior(3, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
          else if (this.currentlySnappedComponent.leftNeighborID == -7777)
            this.customSnapBehavior(3, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
          else
            this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.leftNeighborID);
          if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.leftNeighborID != -7777 && snappedComponent.leftNeighborID != -99998) && !this.currentlySnappedComponent.rightNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable)
            this.currentlySnappedComponent.rightNeighborID = snappedComponent.myID;
          if (this.currentlySnappedComponent == null)
          {
            this.noSnappedComponentFound(3, snappedComponent.region, snappedComponent.myID);
            break;
          }
          break;
      }
      if (this.currentlySnappedComponent != null && snappedComponent != null && this.currentlySnappedComponent.region != snappedComponent.region)
        this.actionOnRegionChange(snappedComponent.region, this.currentlySnappedComponent.region);
      if (this.currentlySnappedComponent == null)
        this.currentlySnappedComponent = snappedComponent;
      this.snapCursorToCurrentSnappedComponent();
      if (this.currentlySnappedComponent == snappedComponent)
        return;
      Game1.playSound("shiny4");
    }

    public virtual void snapCursorToCurrentSnappedComponent()
    {
      if (this.currentlySnappedComponent == null)
        return;
      Game1.setMousePosition(this.currentlySnappedComponent.bounds.Right - this.currentlySnappedComponent.bounds.Width / 4, this.currentlySnappedComponent.bounds.Bottom - this.currentlySnappedComponent.bounds.Height / 4, true);
    }

    protected virtual void noSnappedComponentFound(int direction, int oldRegion, int oldID)
    {
    }

    protected virtual void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
    }

    public virtual bool IsActive()
    {
      if (this._parentMenu == null)
        return this == Game1.activeClickableMenu;
      IClickableMenu parentMenu = this._parentMenu;
      while (parentMenu != null && parentMenu._parentMenu != null)
        parentMenu = parentMenu._parentMenu;
      return parentMenu == Game1.activeClickableMenu;
    }

    public virtual void automaticSnapBehavior(int direction, int oldRegion, int oldID)
    {
      if (this.currentlySnappedComponent == null)
      {
        this.snapToDefaultClickableComponent();
      }
      else
      {
        Vector2 zero = Vector2.Zero;
        switch (direction)
        {
          case 0:
            zero.X = 0.0f;
            zero.Y = -1f;
            break;
          case 1:
            zero.X = 1f;
            zero.Y = 0.0f;
            break;
          case 2:
            zero.X = 0.0f;
            zero.Y = 1f;
            break;
          case 3:
            zero.X = -1f;
            zero.Y = 0.0f;
            break;
        }
        float num1 = -1f;
        ClickableComponent clickableComponent1 = (ClickableComponent) null;
        for (int index = 0; index < this.allClickableComponents.Count; ++index)
        {
          ClickableComponent clickableComponent2 = this.allClickableComponents[index];
          if ((clickableComponent2.leftNeighborID != -1 || clickableComponent2.rightNeighborID != -1 || clickableComponent2.upNeighborID != -1 || clickableComponent2.downNeighborID != -1) && clickableComponent2.myID != -500 && this.IsAutomaticSnapValid(direction, this.currentlySnappedComponent, clickableComponent2) && clickableComponent2.visible && clickableComponent2 != this.upperRightCloseButton && clickableComponent2 != this.currentlySnappedComponent)
          {
            Vector2 vector2_1 = new Vector2((float) (clickableComponent2.bounds.Center.X - this.currentlySnappedComponent.bounds.Center.X), (float) (clickableComponent2.bounds.Center.Y - this.currentlySnappedComponent.bounds.Center.Y));
            Vector2 vector2_2 = new Vector2(vector2_1.X, vector2_1.Y);
            vector2_2.Normalize();
            float num2 = Vector2.Dot(zero, vector2_2);
            if ((double) num2 > 0.00999999977648258)
            {
              float num3 = Vector2.DistanceSquared(Vector2.Zero, vector2_1);
              bool flag = false;
              switch (direction)
              {
                case 0:
                case 2:
                  if ((double) Math.Abs(vector2_1.X) < 32.0)
                  {
                    flag = true;
                    break;
                  }
                  break;
                case 1:
                case 3:
                  if ((double) Math.Abs(vector2_1.Y) < 32.0)
                  {
                    flag = true;
                    break;
                  }
                  break;
              }
              if (this._ShouldAutoSnapPrioritizeAlignedElements() && (double) num2 > 0.999989986419678 | flag)
                num3 *= 0.01f;
              if ((double) num1 == -1.0 || (double) num3 < (double) num1)
              {
                num1 = num3;
                clickableComponent1 = clickableComponent2;
              }
            }
          }
        }
        if (clickableComponent1 == null)
          return;
        this.currentlySnappedComponent = clickableComponent1;
      }
    }

    protected virtual bool _ShouldAutoSnapPrioritizeAlignedElements() => true;

    public virtual bool IsAutomaticSnapValid(
      int direction,
      ClickableComponent a,
      ClickableComponent b)
    {
      return true;
    }

    /// <summary>when the currentlySnappedComponent region changes</summary>
    protected virtual void actionOnRegionChange(int oldRegion, int newRegion)
    {
    }

    public ClickableComponent getComponentWithID(int id)
    {
      if (id == -500)
        return (ClickableComponent) null;
      if (this.allClickableComponents != null)
      {
        for (int index = 0; index < this.allClickableComponents.Count; ++index)
        {
          if (this.allClickableComponents[index] != null && this.allClickableComponents[index].myID == id && this.allClickableComponents[index].visible)
            return this.allClickableComponents[index];
        }
        for (int index = 0; index < this.allClickableComponents.Count; ++index)
        {
          if (this.allClickableComponents[index] != null && this.allClickableComponents[index].myAlternateID == id && this.allClickableComponents[index].visible)
            return this.allClickableComponents[index];
        }
      }
      return (ClickableComponent) null;
    }

    public void initializeUpperRightCloseButton() => this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 36, this.yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);

    public virtual void drawBackground(SpriteBatch b)
    {
      if (this is ShopMenu)
      {
        for (int x = 0; x < Game1.uiViewport.Width; x += 400)
        {
          for (int y = 0; y < Game1.uiViewport.Height; y += 384)
            b.Draw(Game1.mouseCursors, new Vector2((float) x, (float) y), new Rectangle?(new Rectangle(527, 0, 100, 96)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.08f);
        }
      }
      else
      {
        if (Game1.isDarkOut())
          b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(639, 858, 1, 144)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
        else if (Game1.IsRainingHere())
          b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(640, 858, 1, 184)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
        else
          b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(639 + Utility.getSeasonNumber(Game1.currentSeason), 1051, 1, 400)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
        b.Draw(Game1.mouseCursors, new Vector2(-120f, (float) (Game1.uiViewport.Height - 592)), new Rectangle?(new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1035 : (Game1.isRaining || Game1.isDarkOut() ? 886 : 737), 639, 148)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.08f);
        b.Draw(Game1.mouseCursors, new Vector2(2436f, (float) (Game1.uiViewport.Height - 592)), new Rectangle?(new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1035 : (Game1.isRaining || Game1.isDarkOut() ? 886 : 737), 639, 148)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.08f);
        if (!Game1.isRaining)
          return;
        b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Blue * 0.2f);
      }
    }

    public virtual bool showWithoutTransparencyIfOptionIsSet()
    {
      switch (this)
      {
        case GameMenu _:
        case ShopMenu _:
        case WheelSpinGame _:
        case ItemGrabMenu _:
          return true;
        default:
          return false;
      }
    }

    public virtual void clickAway()
    {
    }

    public virtual void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      this.xPositionOnScreen = (int) ((double) (newBounds.Width - this.width) * ((double) this.xPositionOnScreen / (double) (oldBounds.Width - this.width)));
      this.yPositionOnScreen = (int) ((double) (newBounds.Height - this.height) * ((double) this.yPositionOnScreen / (double) (oldBounds.Height - this.height)));
    }

    public virtual void setUpForGamePadMode()
    {
    }

    public virtual bool shouldClampGamePadCursor() => false;

    public virtual void releaseLeftClick(int x, int y)
    {
    }

    public virtual void leftClickHeld(int x, int y)
    {
    }

    public virtual void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.upperRightCloseButton == null || !this.readyToClose() || !this.upperRightCloseButton.containsPoint(x, y))
        return;
      if (playSound)
        Game1.playSound("bigDeSelect");
      this.exitThisMenu();
    }

    public virtual bool overrideSnappyMenuCursorMovementBan() => false;

    public virtual void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public virtual void receiveKeyPress(Keys key)
    {
      if (key == Keys.None)
        return;
      if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
      {
        this.exitThisMenu();
      }
      else
      {
        if (!Game1.options.snappyMenus || !Game1.options.gamepadControls || this.overrideSnappyMenuCursorMovementBan())
          return;
        this.applyMovementKey(key);
      }
    }

    public virtual void gamePadButtonHeld(Buttons b)
    {
    }

    public virtual ClickableComponent getCurrentlySnappedComponent() => this.currentlySnappedComponent;

    public virtual void receiveScrollWheelAction(int direction)
    {
    }

    public virtual void performHoverAction(int x, int y)
    {
      if (this.upperRightCloseButton == null)
        return;
      this.upperRightCloseButton.tryHover(x, y, 0.5f);
    }

    public virtual void draw(SpriteBatch b, int red = -1, int green = -1, int blue = -1)
    {
      if (this.upperRightCloseButton == null || !this.shouldDrawCloseButton())
        return;
      this.upperRightCloseButton.draw(b);
    }

    public virtual void draw(SpriteBatch b)
    {
      if (this.upperRightCloseButton == null || !this.shouldDrawCloseButton())
        return;
      this.upperRightCloseButton.draw(b);
    }

    public virtual bool isWithinBounds(int x, int y) => x - this.xPositionOnScreen < this.width && x - this.xPositionOnScreen >= 0 && y - this.yPositionOnScreen < this.height && y - this.yPositionOnScreen >= 0;

    public virtual void update(GameTime time)
    {
    }

    protected virtual void cleanupBeforeExit()
    {
    }

    public virtual bool shouldDrawCloseButton() => true;

    public void exitThisMenuNoSound() => this.exitThisMenu(false);

    public void exitThisMenu(bool playSound = true)
    {
      if (this.behaviorBeforeCleanup != null)
        this.behaviorBeforeCleanup(this);
      this.cleanupBeforeExit();
      if (playSound)
        Game1.playSound("bigDeSelect");
      if (this == Game1.activeClickableMenu)
        Game1.exitActiveMenu();
      else if (Game1.activeClickableMenu is GameMenu && (Game1.activeClickableMenu as GameMenu).GetCurrentPage() == this)
        Game1.exitActiveMenu();
      if (this._parentMenu != null)
      {
        IClickableMenu parentMenu = this._parentMenu;
        this._parentMenu = (IClickableMenu) null;
        parentMenu.SetChildMenu((IClickableMenu) null);
      }
      if (this.exitFunction == null)
        return;
      IClickableMenu.onExit exitFunction = this.exitFunction;
      this.exitFunction = (IClickableMenu.onExit) null;
      exitFunction();
    }

    public virtual bool autoCenterMouseCursorForGamepad() => true;

    public virtual void emergencyShutDown()
    {
    }

    public virtual bool readyToClose() => true;

    protected void drawHorizontalPartition(
      SpriteBatch b,
      int yPosition,
      bool small = false,
      int red = -1,
      int green = -1,
      int blue = -1)
    {
      Color color = red == -1 ? Color.White : new Color(red, green, blue);
      Texture2D texture = red == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
      if (small)
      {
        b.Draw(texture, new Rectangle(this.xPositionOnScreen + 32, yPosition, this.width - 64, 64), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 25)), color);
      }
      else
      {
        b.Draw(texture, new Vector2((float) this.xPositionOnScreen, (float) yPosition), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 4)), color);
        b.Draw(texture, new Rectangle(this.xPositionOnScreen + 64, yPosition, this.width - 128, 64), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 6)), color);
        b.Draw(texture, new Vector2((float) (this.xPositionOnScreen + this.width - 64), (float) yPosition), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 7)), color);
      }
    }

    protected void drawVerticalPartition(
      SpriteBatch b,
      int xPosition,
      bool small = false,
      int red = -1,
      int green = -1,
      int blue = -1)
    {
      Color color = red == -1 ? Color.White : new Color(red, green, blue);
      Texture2D texture = red == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
      if (small)
      {
        b.Draw(texture, new Rectangle(xPosition, this.yPositionOnScreen + 64 + 32, 64, this.height - 128), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 26)), color);
      }
      else
      {
        b.Draw(texture, new Vector2((float) xPosition, (float) (this.yPositionOnScreen + 64)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 1)), color);
        b.Draw(texture, new Rectangle(xPosition, this.yPositionOnScreen + 128, 64, this.height - 192), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 5)), color);
        b.Draw(texture, new Vector2((float) xPosition, (float) (this.yPositionOnScreen + this.height - 64)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 13)), color);
      }
    }

    protected void drawVerticalIntersectingPartition(
      SpriteBatch b,
      int xPosition,
      int yPosition,
      int red = -1,
      int green = -1,
      int blue = -1)
    {
      Color color = red == -1 ? Color.White : new Color(red, green, blue);
      Texture2D texture = red == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
      b.Draw(texture, new Vector2((float) xPosition, (float) yPosition), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 59)), color);
      b.Draw(texture, new Rectangle(xPosition, yPosition + 64, 64, this.yPositionOnScreen + this.height - 64 - yPosition - 64), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 63)), color);
      b.Draw(texture, new Vector2((float) xPosition, (float) (this.yPositionOnScreen + this.height - 64)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 62)), color);
    }

    protected void drawVerticalUpperIntersectingPartition(
      SpriteBatch b,
      int xPosition,
      int partitionHeight,
      int red = -1,
      int green = -1,
      int blue = -1)
    {
      Color color = red == -1 ? Color.White : new Color(red, green, blue);
      Texture2D texture = red == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
      b.Draw(texture, new Vector2((float) xPosition, (float) (this.yPositionOnScreen + 64)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 44)), color);
      b.Draw(texture, new Rectangle(xPosition, this.yPositionOnScreen + 128, 64, partitionHeight - 32), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 63)), color);
      b.Draw(texture, new Vector2((float) xPosition, (float) (this.yPositionOnScreen + partitionHeight + 64)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 39)), color);
    }

    public static void drawTextureBox(
      SpriteBatch b,
      int x,
      int y,
      int width,
      int height,
      Color color)
    {
      IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, color);
    }

    public static void drawTextureBox(
      SpriteBatch b,
      Texture2D texture,
      Rectangle sourceRect,
      int x,
      int y,
      int width,
      int height,
      Color color,
      float scale = 1f,
      bool drawShadow = true,
      float draw_layer = -1f)
    {
      int num = sourceRect.Width / 3;
      float layerDepth = draw_layer - 0.03f;
      if ((double) draw_layer < 0.0)
      {
        draw_layer = (float) (0.800000011920929 - (double) y * 9.99999997475243E-07);
        layerDepth = 0.77f;
      }
      if (drawShadow)
      {
        b.Draw(texture, new Vector2((float) (x + width - (int) ((double) num * (double) scale) - 8), (float) (y + 8)), new Rectangle?(new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        b.Draw(texture, new Vector2((float) (x - 8), (float) (y + height - (int) ((double) num * (double) scale) + 8)), new Rectangle?(new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        b.Draw(texture, new Vector2((float) (x + width - (int) ((double) num * (double) scale) - 8), (float) (y + height - (int) ((double) num * (double) scale) + 8)), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        b.Draw(texture, new Rectangle(x + (int) ((double) num * (double) scale) - 8, y + 8, width - (int) ((double) num * (double) scale) * 2, (int) ((double) num * (double) scale)), new Rectangle?(new Rectangle(sourceRect.X + num, sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        b.Draw(texture, new Rectangle(x + (int) ((double) num * (double) scale) - 8, y + height - (int) ((double) num * (double) scale) + 8, width - (int) ((double) num * (double) scale) * 2, (int) ((double) num * (double) scale)), new Rectangle?(new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        b.Draw(texture, new Rectangle(x - 8, y + (int) ((double) num * (double) scale) + 8, (int) ((double) num * (double) scale), height - (int) ((double) num * (double) scale) * 2), new Rectangle?(new Rectangle(sourceRect.X, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        b.Draw(texture, new Rectangle(x + width - (int) ((double) num * (double) scale) - 8, y + (int) ((double) num * (double) scale) + 8, (int) ((double) num * (double) scale), height - (int) ((double) num * (double) scale) * 2), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        b.Draw(texture, new Rectangle((int) ((double) num * (double) scale / 2.0) + x - 8, (int) ((double) num * (double) scale / 2.0) + y + 8, width - (int) ((double) num * (double) scale), height - (int) ((double) num * (double) scale)), new Rectangle?(new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
      }
      b.Draw(texture, new Rectangle((int) ((double) num * (double) scale) + x, (int) ((double) num * (double) scale) + y, width - (int) ((double) num * (double) scale * 2.0), height - (int) ((double) num * (double) scale * 2.0)), new Rectangle?(new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, draw_layer);
      b.Draw(texture, new Vector2((float) x, (float) y), new Rectangle?(new Rectangle(sourceRect.X, sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
      b.Draw(texture, new Vector2((float) (x + width - (int) ((double) num * (double) scale)), (float) y), new Rectangle?(new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
      b.Draw(texture, new Vector2((float) x, (float) (y + height - (int) ((double) num * (double) scale))), new Rectangle?(new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
      b.Draw(texture, new Vector2((float) (x + width - (int) ((double) num * (double) scale)), (float) (y + height - (int) ((double) num * (double) scale))), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
      b.Draw(texture, new Rectangle(x + (int) ((double) num * (double) scale), y, width - (int) ((double) num * (double) scale) * 2, (int) ((double) num * (double) scale)), new Rectangle?(new Rectangle(sourceRect.X + num, sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, draw_layer);
      b.Draw(texture, new Rectangle(x + (int) ((double) num * (double) scale), y + height - (int) ((double) num * (double) scale), width - (int) ((double) num * (double) scale) * 2, (int) ((double) num * (double) scale)), new Rectangle?(new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, draw_layer);
      b.Draw(texture, new Rectangle(x, y + (int) ((double) num * (double) scale), (int) ((double) num * (double) scale), height - (int) ((double) num * (double) scale) * 2), new Rectangle?(new Rectangle(sourceRect.X, num + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, draw_layer);
      b.Draw(texture, new Rectangle(x + width - (int) ((double) num * (double) scale), y + (int) ((double) num * (double) scale), (int) ((double) num * (double) scale), height - (int) ((double) num * (double) scale) * 2), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, draw_layer);
    }

    public void drawBorderLabel(SpriteBatch b, string text, SpriteFont font, int x, int y)
    {
      int x1 = (int) font.MeasureString(text).X;
      y += 52;
      b.Draw(Game1.mouseCursors, new Vector2((float) x, (float) y), new Rectangle?(new Rectangle(256, 267, 6, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (x + 24), (float) y), new Rectangle?(new Rectangle(262, 267, 1, 16)), Color.White, 0.0f, Vector2.Zero, new Vector2((float) x1, 4f), SpriteEffects.None, 0.87f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (x + 24 + x1), (float) y), new Rectangle?(new Rectangle(263, 267, 6, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      Utility.drawTextWithShadow(b, text, font, new Vector2((float) (x + 24), (float) (y + 20)), Game1.textColor);
    }

    public static void drawToolTip(
      SpriteBatch b,
      string hoverText,
      string hoverTitle,
      Item hoveredItem,
      bool heldItem = false,
      int healAmountToDisplay = -1,
      int currencySymbol = 0,
      int extraItemToShowIndex = -1,
      int extraItemToShowAmount = -1,
      CraftingRecipe craftingIngredients = null,
      int moneyAmountToShowAtBottom = -1)
    {
      bool flag = hoveredItem != null && hoveredItem is StardewValley.Object && (int) (NetFieldBase<int, NetInt>) (hoveredItem as StardewValley.Object).edibility != -300;
      int val1 = Math.Max(healAmountToDisplay != -1 ? (int) Game1.smallFont.MeasureString(healAmountToDisplay.ToString() + "+ Energy").X + 32 : 0, Math.Max((int) Game1.smallFont.MeasureString(hoverText).X, hoverTitle != null ? (int) Game1.dialogueFont.MeasureString(hoverTitle).X : 0)) + 32;
      if (flag)
      {
        int sub1 = 9999;
        int num1 = 92;
        int num2 = (int) Math.Max((float) val1, Math.Max(Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Energy", (object) sub1)).X + (float) num1, Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Health", (object) sub1)).X + (float) num1));
      }
      IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont, heldItem ? 40 : 0, heldItem ? 40 : 0, moneyAmountToShowAtBottom, hoverTitle, flag ? (int) (NetFieldBase<int, NetInt>) (hoveredItem as StardewValley.Object).edibility : -1, !flag || Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) (hoveredItem as StardewValley.Object).parentSheetIndex].Split('/').Length <= 7 ? (string[]) null : hoveredItem.ModifyItemBuffs(Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) (hoveredItem as StardewValley.Object).parentSheetIndex].Split('/')[7].Split(' ')), hoveredItem, currencySymbol, extraItemToShowIndex, extraItemToShowAmount, craftingIngredients: craftingIngredients);
    }

    public static void drawHoverText(
      SpriteBatch b,
      string text,
      SpriteFont font,
      int xOffset = 0,
      int yOffset = 0,
      int moneyAmountToDisplayAtBottom = -1,
      string boldTitleText = null,
      int healAmountToDisplay = -1,
      string[] buffIconsToDisplay = null,
      Item hoveredItem = null,
      int currencySymbol = 0,
      int extraItemToShowIndex = -1,
      int extraItemToShowAmount = -1,
      int overrideX = -1,
      int overrideY = -1,
      float alpha = 1f,
      CraftingRecipe craftingIngredients = null,
      IList<Item> additional_craft_materials = null)
    {
      StringBuilder text1 = (StringBuilder) null;
      if (text != null)
        text1 = new StringBuilder(text);
      IClickableMenu.drawHoverText(b, text1, font, xOffset, yOffset, moneyAmountToDisplayAtBottom, boldTitleText, healAmountToDisplay, buffIconsToDisplay, hoveredItem, currencySymbol, extraItemToShowIndex, extraItemToShowAmount, overrideX, overrideY, alpha, craftingIngredients, additional_craft_materials);
    }

    public static void drawHoverText(
      SpriteBatch b,
      StringBuilder text,
      SpriteFont font,
      int xOffset = 0,
      int yOffset = 0,
      int moneyAmountToDisplayAtBottom = -1,
      string boldTitleText = null,
      int healAmountToDisplay = -1,
      string[] buffIconsToDisplay = null,
      Item hoveredItem = null,
      int currencySymbol = 0,
      int extraItemToShowIndex = -1,
      int extraItemToShowAmount = -1,
      int overrideX = -1,
      int overrideY = -1,
      float alpha = 1f,
      CraftingRecipe craftingIngredients = null,
      IList<Item> additional_craft_materials = null)
    {
      if (text == null || text.Length == 0)
        return;
      string text1 = (string) null;
      switch (boldTitleText)
      {
        case "":
          boldTitleText = (string) null;
          break;
      }
      int num1 = Math.Max(healAmountToDisplay != -1 ? (int) font.MeasureString(healAmountToDisplay.ToString() + "+ Energy" + 32.ToString()).X : 0, Math.Max((int) font.MeasureString(text).X, boldTitleText != null ? (int) Game1.dialogueFont.MeasureString(boldTitleText).X : 0)) + 32;
      int height1 = Math.Max(20 * 3, (int) font.MeasureString(text).Y + 32 + (moneyAmountToDisplayAtBottom > -1 ? (int) ((double) font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").Y + 4.0) : 8) + (boldTitleText != null ? (int) ((double) Game1.dialogueFont.MeasureString(boldTitleText).Y + 16.0) : 0));
      if (extraItemToShowIndex != -1)
      {
        string[] strArray = Game1.objectInformation[extraItemToShowIndex].Split('/');
        string word = strArray[0];
        if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
          word = strArray[4];
        string text2 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", (object) extraItemToShowAmount, extraItemToShowAmount > 1 ? (object) Lexicon.makePlural(word) : (object) word);
        int num2 = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, extraItemToShowIndex, 16, 16).Width * 2 * 4;
        num1 = Math.Max(num1, num2 + (int) font.MeasureString(text2).X);
      }
      if (buffIconsToDisplay != null)
      {
        foreach (string str in buffIconsToDisplay)
        {
          if (!str.Equals("0"))
            height1 += 34;
        }
        height1 += 4;
      }
      if (craftingIngredients != null && Game1.options.showAdvancedCraftingInformation && craftingIngredients.getCraftCountText() != null)
        height1 += (int) font.MeasureString("T").Y;
      string text3 = (string) null;
      if (hoveredItem != null)
      {
        int startingHeight = height1 + 68 * hoveredItem.attachmentSlots();
        text3 = hoveredItem.getCategoryName();
        if (text3.Length > 0)
        {
          num1 = Math.Max(num1, (int) font.MeasureString(text3).X + 32);
          startingHeight += (int) font.MeasureString("T").Y;
        }
        int sub1 = 9999;
        int horizontalBuffer = 92;
        Point tooltipSpecialIcons = hoveredItem.getExtraSpaceNeededForTooltipSpecialIcons(font, num1, horizontalBuffer, startingHeight, text, boldTitleText, moneyAmountToDisplayAtBottom);
        num1 = tooltipSpecialIcons.X != 0 ? tooltipSpecialIcons.X : num1;
        height1 = tooltipSpecialIcons.Y != 0 ? tooltipSpecialIcons.Y : startingHeight;
        if (hoveredItem is MeleeWeapon && (hoveredItem as MeleeWeapon).GetTotalForgeLevels() > 0)
          height1 += (int) font.MeasureString("T").Y;
        if (hoveredItem is MeleeWeapon && (hoveredItem as MeleeWeapon).GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
          height1 += (int) font.MeasureString("T").Y;
        if (hoveredItem is StardewValley.Object && (int) (NetFieldBase<int, NetInt>) (hoveredItem as StardewValley.Object).edibility != -300)
        {
          if (healAmountToDisplay != -1)
            height1 += 40 * (healAmountToDisplay > 0 ? 2 : 1);
          else
            height1 += 40;
          healAmountToDisplay = (hoveredItem as StardewValley.Object).staminaRecoveredOnConsumption();
          num1 = (int) Math.Max((float) num1, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Energy", (object) sub1)).X + (float) horizontalBuffer, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Health", (object) sub1)).X + (float) horizontalBuffer));
        }
        if (buffIconsToDisplay != null)
        {
          for (int index = 0; index < buffIconsToDisplay.Length; ++index)
          {
            if (!buffIconsToDisplay[index].Equals("0") && index <= 11)
              num1 = (int) Math.Max((float) num1, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + index.ToString(), (object) sub1)).X + (float) horizontalBuffer);
          }
        }
      }
      Vector2 vector2_1 = Vector2.Zero;
      if (craftingIngredients != null)
      {
        if (Game1.options.showAdvancedCraftingInformation)
        {
          int craftableCount = craftingIngredients.getCraftableCount(additional_craft_materials);
          if (craftableCount > 1)
          {
            text1 = " (" + craftableCount.ToString() + ")";
            vector2_1 = Game1.smallFont.MeasureString(text1);
          }
        }
        num1 = (int) Math.Max((float) ((double) Game1.dialogueFont.MeasureString(boldTitleText).X + (double) vector2_1.X + 12.0), 384f);
        height1 += craftingIngredients.getDescriptionHeight(num1 - 8) + (healAmountToDisplay == -1 ? -32 : 0);
      }
      else if (text1 != null && boldTitleText != null)
      {
        vector2_1 = Game1.smallFont.MeasureString(text1);
        num1 = (int) Math.Max((float) num1, (float) ((double) Game1.dialogueFont.MeasureString(boldTitleText).X + (double) vector2_1.X + 12.0));
      }
      int x = Game1.getOldMouseX() + 32 + xOffset;
      int y1 = Game1.getOldMouseY() + 32 + yOffset;
      if (overrideX != -1)
        x = overrideX;
      if (overrideY != -1)
        y1 = overrideY;
      int num3 = x + num1;
      Rectangle safeArea = Utility.getSafeArea();
      int right1 = safeArea.Right;
      if (num3 > right1)
      {
        safeArea = Utility.getSafeArea();
        x = safeArea.Right - num1;
        y1 += 16;
      }
      int num4 = y1 + height1;
      safeArea = Utility.getSafeArea();
      int bottom = safeArea.Bottom;
      if (num4 > bottom)
      {
        x += 16;
        int num5 = x + num1;
        safeArea = Utility.getSafeArea();
        int right2 = safeArea.Right;
        if (num5 > right2)
        {
          safeArea = Utility.getSafeArea();
          x = safeArea.Right - num1;
        }
        safeArea = Utility.getSafeArea();
        y1 = safeArea.Bottom - height1;
      }
      IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y1, num1 + (craftingIngredients != null ? 21 : 0), height1, Color.White * alpha);
      if (boldTitleText != null)
      {
        Vector2 vector2_2 = Game1.dialogueFont.MeasureString(boldTitleText);
        IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y1, num1 + (craftingIngredients != null ? 21 : 0), (int) Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (hoveredItem == null || text3.Length <= 0 ? 0 : (int) font.MeasureString("asd").Y) - 4, Color.White * alpha, drawShadow: false);
        b.Draw(Game1.menuTexture, new Rectangle(x + 12, y1 + (int) Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (hoveredItem == null || text3.Length <= 0 ? 0 : (int) font.MeasureString("asd").Y) - 4, num1 - 4 * (craftingIngredients == null ? 6 : 1), 4), new Rectangle?(new Rectangle(44, 300, 4, 4)), Color.White);
        b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2((float) (x + 16), (float) (y1 + 16 + 4)) + new Vector2(2f, 2f), Game1.textShadowColor);
        b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2((float) (x + 16), (float) (y1 + 16 + 4)) + new Vector2(0.0f, 2f), Game1.textShadowColor);
        b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2((float) (x + 16), (float) (y1 + 16 + 4)), Game1.textColor);
        if (text1 != null)
          Utility.drawTextWithShadow(b, text1, Game1.smallFont, new Vector2((float) (x + 16) + vector2_2.X, (float) (int) ((double) (y1 + 16 + 4) + (double) vector2_2.Y / 2.0 - (double) vector2_1.Y / 2.0)), Game1.textColor);
        y1 += (int) Game1.dialogueFont.MeasureString(boldTitleText).Y;
      }
      int y2;
      if (hoveredItem != null && text3.Length > 0)
      {
        int num6 = y1 - 4;
        Utility.drawTextWithShadow(b, text3, font, new Vector2((float) (x + 16), (float) (num6 + 16 + 4)), hoveredItem.getCategoryColor(), horizontalShadowOffset: 2, verticalShadowOffset: 2);
        y2 = num6 + ((int) font.MeasureString("T").Y + (boldTitleText != null ? 16 : 0) + 4);
        if (hoveredItem is Tool && (hoveredItem as Tool).GetTotalForgeLevels() > 0)
        {
          string text4 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_Forged");
          Utility.drawTextWithShadow(b, text4, font, new Vector2((float) (x + 16), (float) (y2 + 16 + 4)), Color.DarkRed, horizontalShadowOffset: 2, verticalShadowOffset: 2);
          int totalForgeLevels = (hoveredItem as Tool).GetTotalForgeLevels();
          if (totalForgeLevels < (hoveredItem as Tool).GetMaxForges() && !(hoveredItem as Tool).hasEnchantmentOfType<DiamondEnchantment>())
            Utility.drawTextWithShadow(b, " (" + totalForgeLevels.ToString() + "/" + (hoveredItem as Tool).GetMaxForges().ToString() + ")", font, new Vector2((float) (x + 16) + font.MeasureString(text4).X, (float) (y2 + 16 + 4)), Color.DimGray, horizontalShadowOffset: 2, verticalShadowOffset: 2);
          y2 += (int) font.MeasureString("T").Y;
        }
        if (hoveredItem is MeleeWeapon && (hoveredItem as MeleeWeapon).GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
        {
          GalaxySoulEnchantment enchantmentOfType = (hoveredItem as MeleeWeapon).GetEnchantmentOfType<GalaxySoulEnchantment>();
          string text5 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_GalaxyForged");
          Utility.drawTextWithShadow(b, text5, font, new Vector2((float) (x + 16), (float) (y2 + 16 + 4)), Color.DarkRed, horizontalShadowOffset: 2, verticalShadowOffset: 2);
          int level = enchantmentOfType.GetLevel();
          if (level < enchantmentOfType.GetMaximumLevel())
            Utility.drawTextWithShadow(b, " (" + level.ToString() + "/" + enchantmentOfType.GetMaximumLevel().ToString() + ")", font, new Vector2((float) (x + 16) + font.MeasureString(text5).X, (float) (y2 + 16 + 4)), Color.DimGray, horizontalShadowOffset: 2, verticalShadowOffset: 2);
          y2 += (int) font.MeasureString("T").Y;
        }
      }
      else
        y2 = y1 + (boldTitleText != null ? 16 : 0);
      if (hoveredItem != null && craftingIngredients == null)
        hoveredItem.drawTooltip(b, ref x, ref y2, font, alpha, text);
      else if (text != null && text.Length != 0 && (text.Length != 1 || text[0] != ' '))
      {
        b.DrawString(font, text, new Vector2((float) (x + 16), (float) (y2 + 16 + 4)) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
        b.DrawString(font, text, new Vector2((float) (x + 16), (float) (y2 + 16 + 4)) + new Vector2(0.0f, 2f), Game1.textShadowColor * alpha);
        b.DrawString(font, text, new Vector2((float) (x + 16), (float) (y2 + 16 + 4)) + new Vector2(2f, 0.0f), Game1.textShadowColor * alpha);
        b.DrawString(font, text, new Vector2((float) (x + 16), (float) (y2 + 16 + 4)), Game1.textColor * 0.9f * alpha);
        y2 += (int) font.MeasureString(text).Y + 4;
      }
      if (craftingIngredients != null)
      {
        craftingIngredients.drawRecipeDescription(b, new Vector2((float) (x + 16), (float) (y2 - 8)), num1, additional_craft_materials);
        y2 += craftingIngredients.getDescriptionHeight(num1 - 8);
      }
      if (healAmountToDisplay != -1)
      {
        int num7 = (hoveredItem as StardewValley.Object).staminaRecoveredOnConsumption();
        if (num7 >= 0)
        {
          int num8 = (hoveredItem as StardewValley.Object).healthRecoveredOnConsumption();
          Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y2 + 16)), new Rectangle(num7 < 0 ? 140 : 0, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
          Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", (object) ((num7 > 0 ? "+" : "") + num7.ToString())), font, new Vector2((float) (x + 16 + 34 + 4), (float) (y2 + 16)), Game1.textColor);
          y2 += 34;
          if (num8 > 0)
          {
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y2 + 16)), new Rectangle(0, 438, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
            Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Health", (object) ((num8 > 0 ? "+" : "") + num8.ToString())), font, new Vector2((float) (x + 16 + 34 + 4), (float) (y2 + 16)), Game1.textColor);
            y2 += 34;
          }
        }
        else if (num7 != -300)
        {
          Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y2 + 16)), new Rectangle(140, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
          Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", (object) (num7.ToString() ?? "")), font, new Vector2((float) (x + 16 + 34 + 4), (float) (y2 + 16)), Game1.textColor);
          y2 += 34;
        }
      }
      if (buffIconsToDisplay != null)
      {
        for (int index = 0; index < buffIconsToDisplay.Length; ++index)
        {
          if (!buffIconsToDisplay[index].Equals("0"))
          {
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (x + 16 + 4), (float) (y2 + 16)), new Rectangle(10 + index * 10, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
            string str = (Convert.ToInt32(buffIconsToDisplay[index]) > 0 ? "+" : "") + buffIconsToDisplay[index] + " ";
            if (index <= 11)
              str = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + index.ToString(), (object) str);
            Utility.drawTextWithShadow(b, str, font, new Vector2((float) (x + 16 + 34 + 4), (float) (y2 + 16)), Game1.textColor);
            y2 += 34;
          }
        }
      }
      if (hoveredItem != null && hoveredItem.attachmentSlots() > 0)
      {
        hoveredItem.drawAttachments(b, x + 16, y2 + 16);
        if (moneyAmountToDisplayAtBottom > -1)
          y2 += 68 * hoveredItem.attachmentSlots();
      }
      if (moneyAmountToDisplayAtBottom > -1)
      {
        b.DrawString(font, moneyAmountToDisplayAtBottom.ToString() ?? "", new Vector2((float) (x + 16), (float) (y2 + 16 + 4)) + new Vector2(2f, 2f), Game1.textShadowColor);
        b.DrawString(font, moneyAmountToDisplayAtBottom.ToString() ?? "", new Vector2((float) (x + 16), (float) (y2 + 16 + 4)) + new Vector2(0.0f, 2f), Game1.textShadowColor);
        b.DrawString(font, moneyAmountToDisplayAtBottom.ToString() ?? "", new Vector2((float) (x + 16), (float) (y2 + 16 + 4)) + new Vector2(2f, 0.0f), Game1.textShadowColor);
        b.DrawString(font, moneyAmountToDisplayAtBottom.ToString() ?? "", new Vector2((float) (x + 16), (float) (y2 + 16 + 4)), Game1.textColor);
        switch (currencySymbol)
        {
          case 0:
            b.Draw(Game1.debrisSpriteSheet, new Vector2((float) ((double) (x + 16) + (double) font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").X + 20.0), (float) (y2 + 16 + 16)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.95f);
            break;
          case 1:
            b.Draw(Game1.mouseCursors, new Vector2((float) ((double) (x + 8) + (double) font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").X + 20.0), (float) (y2 + 16 - 5)), new Rectangle?(new Rectangle(338, 400, 8, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            break;
          case 2:
            b.Draw(Game1.mouseCursors, new Vector2((float) ((double) (x + 8) + (double) font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").X + 20.0), (float) (y2 + 16 - 7)), new Rectangle?(new Rectangle(211, 373, 9, 10)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            break;
          case 4:
            b.Draw(Game1.objectSpriteSheet, new Vector2((float) ((double) (x + 8) + (double) font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").X + 20.0), (float) (y2 + 16 - 7)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            break;
        }
        y2 += 48;
      }
      if (extraItemToShowIndex != -1)
      {
        if (moneyAmountToDisplayAtBottom == -1)
          y2 += 8;
        string sub2 = Game1.objectInformation[extraItemToShowIndex].Split('/')[4];
        string text6 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", (object) extraItemToShowAmount, (object) sub2);
        float height2 = Math.Max(font.MeasureString(text6).Y + 21f, 96f);
        IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y2 + 4, num1 + (craftingIngredients != null ? 21 : 0), (int) height2, Color.White);
        y2 += 20;
        b.DrawString(font, text6, new Vector2((float) (x + 16), (float) (y2 + 4)) + new Vector2(2f, 2f), Game1.textShadowColor);
        b.DrawString(font, text6, new Vector2((float) (x + 16), (float) (y2 + 4)) + new Vector2(0.0f, 2f), Game1.textShadowColor);
        b.DrawString(font, text6, new Vector2((float) (x + 16), (float) (y2 + 4)) + new Vector2(2f, 0.0f), Game1.textShadowColor);
        b.DrawString(Game1.smallFont, text6, new Vector2((float) (x + 16), (float) (y2 + 4)), Game1.textColor);
        b.Draw(Game1.objectSpriteSheet, new Vector2((float) (x + 16 + (int) font.MeasureString(text6).X + 21), (float) y2), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, extraItemToShowIndex, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      if (craftingIngredients == null || !Game1.options.showAdvancedCraftingInformation)
        return;
      Utility.drawTextWithShadow(b, craftingIngredients.getCraftCountText(), font, new Vector2((float) (x + 16), (float) (y2 + 16 + 4)), Game1.textColor, horizontalShadowOffset: 2, verticalShadowOffset: 2);
      int num9 = y2 + ((int) font.MeasureString("T").Y + 4);
    }

    public delegate void onExit();
  }
}
