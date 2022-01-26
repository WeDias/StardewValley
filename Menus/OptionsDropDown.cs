// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.OptionsDropDown
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
  public class OptionsDropDown : OptionsElement
  {
    public const int pixelsHigh = 11;
    [InstancedStatic]
    public static OptionsDropDown selected;
    public List<string> dropDownOptions = new List<string>();
    public List<string> dropDownDisplayOptions = new List<string>();
    public int selectedOption;
    public int recentSlotY;
    public int startingSelected;
    private bool clicked;
    public Rectangle dropDownBounds;
    public static Rectangle dropDownBGSource = new Rectangle(433, 451, 3, 3);
    public static Rectangle dropDownButtonSource = new Rectangle(437, 450, 10, 11);

    public OptionsDropDown(string label, int whichOption, int x = -1, int y = -1)
      : base(label, x, y, (int) Game1.smallFont.MeasureString("Windowed Borderless Mode   ").X + 48, 44, whichOption)
    {
      Game1.options.setDropDownToProperValue(this);
      this.RecalculateBounds();
    }

    public virtual void RecalculateBounds()
    {
      foreach (string downDisplayOption in this.dropDownDisplayOptions)
      {
        float x = Game1.smallFont.MeasureString(downDisplayOption).X;
        if ((double) x >= (double) (this.bounds.Width - 48))
          this.bounds.Width = (int) ((double) x + 64.0);
      }
      this.dropDownBounds = new Rectangle(this.bounds.X, this.bounds.Y, this.bounds.Width - 48, this.bounds.Height * this.dropDownOptions.Count);
    }

    public override void leftClickHeld(int x, int y)
    {
      if (this.greyedOut)
        return;
      base.leftClickHeld(x, y);
      this.clicked = true;
      this.dropDownBounds.Y = Math.Min(this.dropDownBounds.Y, Game1.uiViewport.Height - this.dropDownBounds.Height - this.recentSlotY);
      if (Game1.options.SnappyMenus)
        return;
      this.selectedOption = (int) Math.Max(Math.Min((float) (y - this.dropDownBounds.Y) / (float) this.bounds.Height, (float) (this.dropDownOptions.Count - 1)), 0.0f);
    }

    public override void receiveLeftClick(int x, int y)
    {
      if (this.greyedOut)
        return;
      base.receiveLeftClick(x, y);
      this.startingSelected = this.selectedOption;
      if (!this.clicked)
        Game1.playSound("shwip");
      this.leftClickHeld(x, y);
      OptionsDropDown.selected = this;
    }

    public override void leftClickReleased(int x, int y)
    {
      if (this.greyedOut || this.dropDownOptions.Count <= 0)
        return;
      base.leftClickReleased(x, y);
      if (this.clicked)
        Game1.playSound("drumkit6");
      this.clicked = false;
      OptionsDropDown.selected = this;
      if (this.dropDownBounds.Contains(x, y) || Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse)
        Game1.options.changeDropDownOption(this.whichOption, this.dropDownOptions[this.selectedOption]);
      else
        this.selectedOption = this.startingSelected;
      OptionsDropDown.selected = (OptionsDropDown) null;
    }

    public override void receiveKeyPress(Keys key)
    {
      base.receiveKeyPress(key);
      if (!Game1.options.SnappyMenus || this.greyedOut)
        return;
      if (!this.clicked)
      {
        if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
        {
          ++this.selectedOption;
          if (this.selectedOption >= this.dropDownOptions.Count)
            this.selectedOption = 0;
          OptionsDropDown.selected = this;
          Game1.options.changeDropDownOption(this.whichOption, this.dropDownOptions[this.selectedOption]);
          OptionsDropDown.selected = (OptionsDropDown) null;
        }
        else
        {
          if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
            return;
          --this.selectedOption;
          if (this.selectedOption < 0)
            this.selectedOption = this.dropDownOptions.Count - 1;
          OptionsDropDown.selected = this;
          Game1.options.changeDropDownOption(this.whichOption, this.dropDownOptions[this.selectedOption]);
          OptionsDropDown.selected = (OptionsDropDown) null;
        }
      }
      else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
      {
        Game1.playSound("shiny4");
        ++this.selectedOption;
        if (this.selectedOption < this.dropDownOptions.Count)
          return;
        this.selectedOption = 0;
      }
      else
      {
        if (!Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
          return;
        Game1.playSound("shiny4");
        --this.selectedOption;
        if (this.selectedOption >= 0)
          return;
        this.selectedOption = this.dropDownOptions.Count - 1;
      }
    }

    public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
    {
      this.recentSlotY = slotY;
      base.draw(b, slotX, slotY, context);
      float num = this.greyedOut ? 0.33f : 1f;
      if (this.clicked)
      {
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, OptionsDropDown.dropDownBGSource, slotX + this.dropDownBounds.X, slotY + this.dropDownBounds.Y, this.dropDownBounds.Width, this.dropDownBounds.Height, Color.White * num, 4f, false, 0.97f);
        for (int index = 0; index < this.dropDownDisplayOptions.Count; ++index)
        {
          if (index == this.selectedOption)
            b.Draw(Game1.staminaRect, new Rectangle(slotX + this.dropDownBounds.X, slotY + this.dropDownBounds.Y + index * this.bounds.Height, this.dropDownBounds.Width, this.bounds.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Wheat, 0.0f, Vector2.Zero, SpriteEffects.None, 0.975f);
          b.DrawString(Game1.smallFont, this.dropDownDisplayOptions[index], new Vector2((float) (slotX + this.dropDownBounds.X + 4), (float) (slotY + this.dropDownBounds.Y + 8 + this.bounds.Height * index)), Game1.textColor * num, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.98f);
        }
        b.Draw(Game1.mouseCursors, new Vector2((float) (slotX + this.bounds.X + this.bounds.Width - 48), (float) (slotY + this.bounds.Y)), new Rectangle?(OptionsDropDown.dropDownButtonSource), Color.Wheat * num, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.981f);
      }
      else
      {
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, OptionsDropDown.dropDownBGSource, slotX + this.bounds.X, slotY + this.bounds.Y, this.bounds.Width - 48, this.bounds.Height, Color.White * num, 4f, false);
        b.DrawString(Game1.smallFont, this.selectedOption >= this.dropDownDisplayOptions.Count || this.selectedOption < 0 ? "" : this.dropDownDisplayOptions[this.selectedOption], new Vector2((float) (slotX + this.bounds.X + 4), (float) (slotY + this.bounds.Y + 8)), Game1.textColor * num, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.88f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (slotX + this.bounds.X + this.bounds.Width - 48), (float) (slotY + this.bounds.Y)), new Rectangle?(OptionsDropDown.dropDownButtonSource), Color.White * num, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
      }
    }
  }
}
