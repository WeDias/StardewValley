// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.OptionsPlusMinusButton
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class OptionsPlusMinusButton : OptionsPlusMinus
  {
    protected Rectangle _buttonBounds;
    protected Rectangle _buttonRect;
    protected Texture2D _buttonTexture;
    protected Action<string> _buttonAction;

    public OptionsPlusMinusButton(
      string label,
      int whichOptions,
      List<string> options,
      List<string> displayOptions,
      Texture2D buttonTexture,
      Rectangle buttonRect,
      Action<string> buttonAction,
      int x = -1,
      int y = -1)
      : base(label, whichOptions, options, displayOptions, x, y)
    {
      this._buttonRect = buttonRect;
      this._buttonBounds = new Rectangle(this.bounds.Left, 4 - this._buttonRect.Height / 2 + 8, this._buttonRect.Width * 4, this._buttonRect.Height * 4);
      this._buttonTexture = buttonTexture;
      this._buttonAction = buttonAction;
      int num1 = 8;
      this.plusButton.X += this._buttonBounds.Width + num1 * 4;
      this.minusButton.X += this._buttonBounds.Width + num1 * 4;
      this.bounds.Width += this._buttonBounds.Width + num1 * 4;
      int num2 = this._buttonBounds.Height - this.bounds.Height;
      if (num2 <= 0)
        return;
      this.bounds.Y -= num2 / 2;
      this.bounds.Height += num2;
      this.labelOffset.Y += (float) (num2 / 2);
    }

    public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
    {
      b.Draw(this._buttonTexture, new Vector2((float) (slotX + this._buttonBounds.X), (float) (slotY + this._buttonBounds.Y)), new Rectangle?(this._buttonRect), Color.White * (this.greyedOut ? 0.33f : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.4f);
      base.draw(b, slotX, slotY, context);
    }

    public override void receiveLeftClick(int x, int y)
    {
      if (!this.greyedOut && this._buttonBounds.Contains(x, y))
      {
        if (this._buttonAction == null)
          return;
        string str = "";
        if (this.selected >= 0 && this.selected < this.options.Count)
          str = this.options[this.selected];
        this._buttonAction(str);
      }
      else
        base.receiveLeftClick(x, y);
    }
  }
}
