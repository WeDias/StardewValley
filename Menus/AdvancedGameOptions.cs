// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.AdvancedGameOptions
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class AdvancedGameOptions : IClickableMenu
  {
    public const int itemsPerPage = 7;
    private string hoverText = "";
    public List<ClickableComponent> optionSlots = new List<ClickableComponent>();
    public int currentItemIndex;
    private ClickableTextureComponent upArrow;
    private ClickableTextureComponent downArrow;
    private ClickableTextureComponent scrollBar;
    public ClickableTextureComponent okButton;
    public List<Action> applySettingCallbacks = new List<Action>();
    public Dictionary<OptionsElement, string> tooltips = new Dictionary<OptionsElement, string>();
    public int ID_okButton = 10000;
    private bool scrolling;
    public List<OptionsElement> options = new List<OptionsElement>();
    private Rectangle scrollBarBounds;
    protected static int _lastSelectedIndex;
    protected static int _lastCurrentItemIndex;
    protected int _lastHoveredIndex;
    protected int _hoverDuration;
    public const int WINDOW_WIDTH = 800;
    public const int WINDOW_HEIGHT = 500;
    public bool initialMonsterSpawnAtValue;
    private int optionsSlotHeld = -1;

    public AdvancedGameOptions()
      : base(Game1.uiViewport.Width / 2 - 400, Game1.uiViewport.Height / 2 - 250, 800, 500)
    {
      int x = this.xPositionOnScreen + this.width + 16;
      this.upArrow = new ClickableTextureComponent(new Rectangle(x, this.yPositionOnScreen, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
      this.downArrow = new ClickableTextureComponent(new Rectangle(x, this.yPositionOnScreen + this.height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
      this.scrollBarBounds = new Rectangle();
      this.scrollBarBounds.X = this.upArrow.bounds.X + 12;
      this.scrollBarBounds.Width = 24;
      this.scrollBarBounds.Y = this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4;
      this.scrollBarBounds.Height = this.downArrow.bounds.Y - 4 - this.scrollBarBounds.Y;
      this.scrollBar = new ClickableTextureComponent(new Rectangle(this.scrollBarBounds.X, this.scrollBarBounds.Y, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
      for (int index = 0; index < 7; ++index)
        this.optionSlots.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + index * ((this.height - 16) / 7), this.width - 16, this.height / 7), index.ToString() ?? "")
        {
          myID = index,
          downNeighborID = index < 6 ? index + 1 : -7777,
          upNeighborID = index > 0 ? index - 1 : -7777,
          fullyImmutable = true
        });
      this.PopulateOptions();
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen + this.height + 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent.myID = this.ID_okButton;
      textureComponent.upNeighborID = -99998;
      this.okButton = textureComponent;
      this.populateClickableComponentList();
      if (!Game1.options.SnappyMenus)
        return;
      this.setCurrentlySnappedComponentTo(this.ID_okButton);
      this.snapCursorToCurrentSnappedComponent();
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
      base.customSnapBehavior(direction, oldRegion, oldID);
      if (oldID == 6 && direction == 2)
      {
        if (this.currentItemIndex < Math.Max(0, this.options.Count - 7))
        {
          this.downArrowPressed();
          Game1.playSound("shiny4");
        }
        else
        {
          this.currentlySnappedComponent = this.getComponentWithID(this.ID_okButton);
          if (this.currentlySnappedComponent == null)
            return;
          this.currentlySnappedComponent.upNeighborID = Math.Min(this.options.Count, 7) - 1;
        }
      }
      else
      {
        if (oldID != 0 || direction != 0)
          return;
        if (this.currentItemIndex > 0)
        {
          this.upArrowPressed();
          Game1.playSound("shiny4");
        }
        else
          this.snapCursorToCurrentSnappedComponent();
      }
    }

    public virtual void PopulateOptions()
    {
      this.options.Clear();
      this.tooltips.Clear();
      this.applySettingCallbacks.Clear();
      this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\UI:AGO_Label")));
      this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\UI:AGO_CCB"))
      {
        style = OptionsElement.Style.OptionLabel
      });
      this.AddDropdown<Game1.BundleType>("", Game1.content.LoadString("Strings\\UI:AGO_CCB_Tooltip"), (Func<Game1.BundleType>) (() => Game1.bundleType), (Action<Game1.BundleType>) (val => Game1.bundleType = val), new KeyValuePair<string, Game1.BundleType>(Game1.content.LoadString("Strings\\UI:AGO_CCB_Normal"), Game1.BundleType.Default), new KeyValuePair<string, Game1.BundleType>(Game1.content.LoadString("Strings\\UI:AGO_CCB_Remixed"), Game1.BundleType.Remixed));
      this.AddCheckbox(Game1.content.LoadString("Strings\\UI:AGO_Year1Completable"), Game1.content.LoadString("Strings\\UI:AGO_Year1Completable_Tooltip"), (Func<bool>) (() => Game1.game1.GetNewGameOption<bool>("YearOneCompletable")), (Action<bool>) (val => Game1.game1.SetNewGameOption<bool>("YearOneCompletable", val)));
      this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\UI:AGO_MineTreasureShuffle"))
      {
        style = OptionsElement.Style.OptionLabel
      });
      this.AddDropdown<Game1.MineChestType>("", Game1.content.LoadString("Strings\\UI:AGO_MineTreasureShuffle_Tooltip"), (Func<Game1.MineChestType>) (() => Game1.game1.GetNewGameOption<Game1.MineChestType>("MineChests")), (Action<Game1.MineChestType>) (val => Game1.game1.SetNewGameOption<Game1.MineChestType>("MineChests", val)), new KeyValuePair<string, Game1.MineChestType>(Game1.content.LoadString("Strings\\UI:AGO_CCB_Normal"), Game1.MineChestType.Default), new KeyValuePair<string, Game1.MineChestType>(Game1.content.LoadString("Strings\\UI:AGO_CCB_Remixed"), Game1.MineChestType.Remixed));
      this.AddCheckbox(Game1.content.LoadString("Strings\\UI:AGO_FarmMonsters"), Game1.content.LoadString("Strings\\UI:AGO_FarmMonsters_Tooltip"), (Func<bool>) (() =>
      {
        bool flag = Game1.spawnMonstersAtNight;
        if (Game1.game1.newGameSetupOptions.ContainsKey("SpawnMonstersAtNight"))
          flag = Game1.game1.GetNewGameOption<bool>("SpawnMonstersAtNight");
        this.initialMonsterSpawnAtValue = flag;
        return flag;
      }), (Action<bool>) (val =>
      {
        if (this.initialMonsterSpawnAtValue == val)
          return;
        Game1.game1.SetNewGameOption<bool>("SpawnMonstersAtNight", val);
      }));
      this.AddDropdown<float>(Game1.content.LoadString("Strings\\UI:Character_Difficulty"), Game1.content.LoadString("Strings\\UI:AGO_ProfitMargin_Tooltip"), (Func<float>) (() => Game1.player.difficultyModifier), (Action<float>) (val => Game1.player.difficultyModifier = val), new KeyValuePair<string, float>(Game1.content.LoadString("Strings\\UI:Character_Normal"), 1f), new KeyValuePair<string, float>("75%", 0.75f), new KeyValuePair<string, float>("50%", 0.5f), new KeyValuePair<string, float>("25%", 0.25f));
      this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\UI:AGO_MPOptions_Label")));
      this.AddDropdown<int>(Game1.content.LoadString("Strings\\UI:Character_StartingCabins"), Game1.content.LoadString("Strings\\UI:AGO_StartingCabins_Tooltip"), (Func<int>) (() => Game1.startingCabins), (Action<int>) (val => Game1.startingCabins = val), new KeyValuePair<string, int>(Game1.content.LoadString("Strings\\UI:Character_none"), 0), new KeyValuePair<string, int>("1", 1), new KeyValuePair<string, int>("2", 2), new KeyValuePair<string, int>("3", 3));
      this.AddDropdown<bool>(Game1.content.LoadString("Strings\\UI:Character_CabinLayout"), Game1.content.LoadString("Strings\\UI:AGO_CabinLayout_Tooltip"), (Func<bool>) (() => Game1.cabinsSeparate), (Action<bool>) (val => Game1.cabinsSeparate = val), new KeyValuePair<string, bool>(Game1.content.LoadString("Strings\\UI:Character_Close"), false), new KeyValuePair<string, bool>(Game1.content.LoadString("Strings\\UI:Character_Separate"), true));
      this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\UI:AGO_OtherOptions_Label")));
      this.options.Add(new OptionsElement(Game1.content.LoadString("Strings\\UI:AGO_RandomSeed"))
      {
        style = OptionsElement.Style.OptionLabel
      });
      OptionsTextEntry optionsTextEntry = this.AddTextEntry("", Game1.content.LoadString("Strings\\UI:AGO_RandomSeed_Tooltip"), (Func<string>) (() => !Game1.startingGameSeed.HasValue ? "" : Game1.startingGameSeed.Value.ToString()), (Action<string>) (val =>
      {
        val.Trim();
        if (string.IsNullOrEmpty(val))
        {
          Game1.startingGameSeed = new ulong?();
        }
        else
        {
          ulong result = 0;
          for (; val.Length > 0; val = val.Substring(0, val.Length - 1))
          {
            if (ulong.TryParse(val, out result))
            {
              Game1.startingGameSeed = new ulong?(result);
              break;
            }
          }
        }
      }));
      optionsTextEntry.textBox.numbersOnly = true;
      optionsTextEntry.textBox.textLimit = 9;
      for (int count = this.options.Count; count < 7; ++count)
        this.options.Add(new OptionsElement(""));
    }

    public virtual void CloseAndApply()
    {
      foreach (Action applySettingCallback in this.applySettingCallbacks)
        applySettingCallback();
      this.applySettingCallbacks.Clear();
      this.exitThisMenu();
    }

    public virtual OptionsTextEntry AddTextEntry(
      string label,
      string tooltip,
      Func<string> get,
      Action<string> set)
    {
      OptionsTextEntry option_element = new OptionsTextEntry(label, -999);
      this.tooltips[(OptionsElement) option_element] = tooltip;
      option_element.textBox.Text = get();
      this.applySettingCallbacks.Add((Action) (() => set(option_element.textBox.Text)));
      this.options.Add((OptionsElement) option_element);
      return option_element;
    }

    public OptionsDropDown AddDropdown<T>(
      string label,
      string tooltip,
      Func<T> get,
      Action<T> set,
      params KeyValuePair<string, T>[] dropdown_options)
    {
      OptionsDropDown option_element = new OptionsDropDown(label, -999);
      this.tooltips[(OptionsElement) option_element] = tooltip;
      foreach (KeyValuePair<string, T> dropdownOption in dropdown_options)
      {
        option_element.dropDownDisplayOptions.Add(dropdownOption.Key);
        option_element.dropDownOptions.Add(dropdownOption.Value.ToString());
      }
      option_element.RecalculateBounds();
      T obj = get();
      int num = 0;
      for (int index = 0; index < dropdown_options.Length; ++index)
      {
        KeyValuePair<string, T> dropdownOption = dropdown_options[index];
        if ((object) dropdownOption.Value == null && (object) obj == null || (object) dropdownOption.Value != null && (object) obj != null && dropdownOption.Value.Equals((object) obj))
        {
          num = index;
          break;
        }
      }
      option_element.selectedOption = num;
      this.applySettingCallbacks.Add((Action) (() => set(dropdown_options[option_element.selectedOption].Value)));
      this.options.Add((OptionsElement) option_element);
      return option_element;
    }

    public virtual OptionsCheckbox AddCheckbox(
      string label,
      string tooltip,
      Func<bool> get,
      Action<bool> set)
    {
      OptionsCheckbox option_element = new OptionsCheckbox(label, -999);
      this.tooltips[(OptionsElement) option_element] = tooltip;
      option_element.isChecked = get();
      this.applySettingCallbacks.Add((Action) (() => set(option_element.isChecked)));
      this.options.Add((OptionsElement) option_element);
      return option_element;
    }

    public override bool readyToClose() => false;

    public override void snapToDefaultClickableComponent()
    {
      base.snapToDefaultClickableComponent();
      this.currentlySnappedComponent = this.getComponentWithID(this.ID_okButton);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void applyMovementKey(int direction)
    {
      if (this.IsDropdownActive())
      {
        if (this.optionsSlotHeld == -1 || this.optionsSlotHeld + this.currentItemIndex >= this.options.Count || !(this.options[this.currentItemIndex + this.optionsSlotHeld] is OptionsDropDown) || direction == 2)
          ;
      }
      else
        base.applyMovementKey(direction);
    }

    private void setScrollBarToCurrentIndex()
    {
      if (this.options.Count <= 0)
        return;
      this.scrollBar.bounds.Y = this.scrollBarBounds.Y + this.scrollBarBounds.Height / Math.Max(1, this.options.Count - 7) * this.currentItemIndex;
      if (this.currentItemIndex != this.options.Count - 7)
        return;
      this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 4;
    }

    public override void snapCursorToCurrentSnappedComponent()
    {
      if (this.currentlySnappedComponent != null && this.currentlySnappedComponent.myID < this.options.Count)
      {
        if (this.options[this.currentlySnappedComponent.myID + this.currentItemIndex] is OptionsDropDown option)
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + option.bounds.Right - 32, this.currentlySnappedComponent.bounds.Center.Y - 4);
        else if (this.options[this.currentlySnappedComponent.myID + this.currentItemIndex] is OptionsPlusMinusButton)
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + 64, this.currentlySnappedComponent.bounds.Center.Y + 4);
        else if (this.options[this.currentlySnappedComponent.myID + this.currentItemIndex] is OptionsInputListener)
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Right - 48, this.currentlySnappedComponent.bounds.Center.Y - 12);
        else
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + 48, this.currentlySnappedComponent.bounds.Center.Y - 12);
      }
      else
      {
        if (this.currentlySnappedComponent == null)
          return;
        base.snapCursorToCurrentSnappedComponent();
      }
    }

    protected override void cleanupBeforeExit() => base.cleanupBeforeExit();

    public virtual void SetScrollFromY(int y)
    {
      int y1 = this.scrollBar.bounds.Y;
      this.currentItemIndex = (int) Utility.Lerp(0.0f, (float) (this.options.Count - 7), Utility.Clamp((float) (y - this.scrollBarBounds.Y) / (float) this.scrollBarBounds.Height, 0.0f, 1f));
      this.setScrollBarToCurrentIndex();
      int y2 = this.scrollBar.bounds.Y;
      if (y1 == y2)
        return;
      Game1.playSound("shiny4");
    }

    public override void leftClickHeld(int x, int y)
    {
      if (GameMenu.forcePreventClose)
        return;
      base.leftClickHeld(x, y);
      if (this.scrolling)
      {
        this.SetScrollFromY(y);
      }
      else
      {
        if (this.optionsSlotHeld == -1 || this.optionsSlotHeld + this.currentItemIndex >= this.options.Count)
          return;
        this.options[this.currentItemIndex + this.optionsSlotHeld].leftClickHeld(x - this.optionSlots[this.optionsSlotHeld].bounds.X, y - this.optionSlots[this.optionsSlotHeld].bounds.Y);
      }
    }

    public override ClickableComponent getCurrentlySnappedComponent() => this.currentlySnappedComponent;

    public override void setCurrentlySnappedComponentTo(int id)
    {
      this.currentlySnappedComponent = this.getComponentWithID(id);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void receiveKeyPress(Keys key)
    {
      if (this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count || Game1.options.snappyMenus && Game1.options.gamepadControls)
      {
        if (this.currentlySnappedComponent != null && Game1.options.snappyMenus && Game1.options.gamepadControls && this.options.Count > this.currentItemIndex + this.currentlySnappedComponent.myID && this.currentItemIndex + this.currentlySnappedComponent.myID >= 0)
          this.options[this.currentItemIndex + this.currentlySnappedComponent.myID].receiveKeyPress(key);
        else if (this.options.Count > this.currentItemIndex + this.optionsSlotHeld && this.currentItemIndex + this.optionsSlotHeld >= 0)
          this.options[this.currentItemIndex + this.optionsSlotHeld].receiveKeyPress(key);
      }
      base.receiveKeyPress(key);
    }

    public override void receiveScrollWheelAction(int direction)
    {
      if (GameMenu.forcePreventClose || this.IsDropdownActive())
        return;
      base.receiveScrollWheelAction(direction);
      if (direction > 0 && this.currentItemIndex > 0)
      {
        this.upArrowPressed();
        Game1.playSound("shiny4");
      }
      else if (direction < 0 && this.currentItemIndex < Math.Max(0, this.options.Count - 7))
      {
        this.downArrowPressed();
        Game1.playSound("shiny4");
      }
      if (!Game1.options.SnappyMenus)
        return;
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void releaseLeftClick(int x, int y)
    {
      if (GameMenu.forcePreventClose)
        return;
      base.releaseLeftClick(x, y);
      if (this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count)
        this.options[this.currentItemIndex + this.optionsSlotHeld].leftClickReleased(x - this.optionSlots[this.optionsSlotHeld].bounds.X, y - this.optionSlots[this.optionsSlotHeld].bounds.Y);
      this.optionsSlotHeld = -1;
      this.scrolling = false;
    }

    public bool IsDropdownActive() => this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count && this.options[this.currentItemIndex + this.optionsSlotHeld] is OptionsDropDown;

    private void downArrowPressed()
    {
      if (this.IsDropdownActive())
        return;
      this.downArrow.scale = this.downArrow.baseScale;
      ++this.currentItemIndex;
      this.UnsubscribeFromSelectedTextbox();
      this.setScrollBarToCurrentIndex();
    }

    public virtual void UnsubscribeFromSelectedTextbox()
    {
      if (Game1.keyboardDispatcher.Subscriber == null)
        return;
      foreach (OptionsElement option in this.options)
      {
        if (option is OptionsTextEntry && Game1.keyboardDispatcher.Subscriber == (option as OptionsTextEntry).textBox)
        {
          Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) null;
          break;
        }
      }
    }

    public void preWindowSizeChange()
    {
      AdvancedGameOptions._lastSelectedIndex = this.getCurrentlySnappedComponent() != null ? this.getCurrentlySnappedComponent().myID : -1;
      AdvancedGameOptions._lastCurrentItemIndex = this.currentItemIndex;
    }

    public void postWindowSizeChange()
    {
      if (Game1.options.SnappyMenus)
        Game1.activeClickableMenu.setCurrentlySnappedComponentTo(AdvancedGameOptions._lastSelectedIndex);
      this.currentItemIndex = AdvancedGameOptions._lastCurrentItemIndex;
      this.setScrollBarToCurrentIndex();
    }

    private void upArrowPressed()
    {
      if (this.IsDropdownActive())
        return;
      this.upArrow.scale = this.upArrow.baseScale;
      --this.currentItemIndex;
      this.UnsubscribeFromSelectedTextbox();
      this.setScrollBarToCurrentIndex();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (GameMenu.forcePreventClose)
        return;
      if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.options.Count - 7))
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
      this.currentItemIndex = Math.Max(0, Math.Min(this.options.Count - 7, this.currentItemIndex));
      if (this.okButton.containsPoint(x, y))
      {
        this.CloseAndApply();
      }
      else
      {
        this.UnsubscribeFromSelectedTextbox();
        for (int index = 0; index < this.optionSlots.Count; ++index)
        {
          if (this.optionSlots[index].bounds.Contains(x, y) && this.currentItemIndex + index < this.options.Count && this.options[this.currentItemIndex + index].bounds.Contains(x - this.optionSlots[index].bounds.X, y - this.optionSlots[index].bounds.Y))
          {
            this.options[this.currentItemIndex + index].receiveLeftClick(x - this.optionSlots[index].bounds.X, y - this.optionSlots[index].bounds.Y);
            this.optionsSlotHeld = index;
            break;
          }
        }
      }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      this.okButton.tryHover(x, y);
      for (int index = 0; index < this.optionSlots.Count; ++index)
      {
        if (this.currentItemIndex >= 0 && this.currentItemIndex + index < this.options.Count && this.options[this.currentItemIndex + index].bounds.Contains(x - this.optionSlots[index].bounds.X, y - this.optionSlots[index].bounds.Y))
        {
          Game1.SetFreeCursorDrag();
          break;
        }
      }
      if (this.scrollBarBounds.Contains(x, y))
        Game1.SetFreeCursorDrag();
      if (GameMenu.forcePreventClose)
        return;
      this.hoverText = "";
      int num1 = -1;
      if (!this.IsDropdownActive())
      {
        for (int index = 0; index < this.optionSlots.Count; ++index)
        {
          if (this.optionSlots[index].containsPoint(x, y) && index + this.currentItemIndex < this.options.Count && this.hoverText == "")
            num1 = index + this.currentItemIndex;
        }
      }
      if (this._lastHoveredIndex != num1)
      {
        this._lastHoveredIndex = num1;
        this._hoverDuration = 0;
      }
      else
        this._hoverDuration += (int) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
      if (this._lastHoveredIndex >= 0 && this._hoverDuration >= 500)
      {
        OptionsElement option = this.options[this._lastHoveredIndex];
        if (this.tooltips.ContainsKey(option))
          this.hoverText = this.tooltips[option];
      }
      this.upArrow.tryHover(x, y);
      this.downArrow.tryHover(x, y);
      this.scrollBar.tryHover(x, y);
      int num2 = this.scrolling ? 1 : 0;
    }

    public override void draw(SpriteBatch b)
    {
      SpriteBatch spriteBatch = b;
      Texture2D staminaRect = Game1.staminaRect;
      Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
      int width = viewport.Width;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int height = viewport.Height;
      Rectangle destinationRectangle = new Rectangle(0, 0, width, height);
      Color color = Color.Black * 0.75f;
      spriteBatch.Draw(staminaRect, destinationRectangle, color);
      Game1.DrawBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
      this.okButton.draw(b);
      b.End();
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      for (int index = 0; index < this.optionSlots.Count; ++index)
      {
        if (this.currentItemIndex >= 0 && this.currentItemIndex + index < this.options.Count)
          this.options[this.currentItemIndex + index].draw(b, this.optionSlots[index].bounds.X, this.optionSlots[index].bounds.Y, (IClickableMenu) this);
      }
      b.End();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if (this.options.Count > 7)
      {
        this.upArrow.draw(b);
        this.downArrow.draw(b);
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarBounds.X, this.scrollBarBounds.Y, this.scrollBarBounds.Width, this.scrollBarBounds.Height, Color.White, 4f, false);
        this.scrollBar.draw(b);
      }
      if (!this.hoverText.Equals(""))
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      this.drawMouse(b);
    }
  }
}
