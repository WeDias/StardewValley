// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.OptionsButton
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StardewValley.Menus
{
  public class OptionsButton : OptionsElement
  {
    private Action action;

    public OptionsButton(string label, Action action)
      : base(label)
    {
      this.action = action;
      this.bounds = new Rectangle(32, 0, (int) Game1.dialogueFont.MeasureString(label).X + 64, 68);
    }

    public override void receiveLeftClick(int x, int y)
    {
      if (!this.greyedOut && this.bounds.Contains(x, y) && this.action != null)
        this.action();
      base.receiveLeftClick(x, y);
    }

    public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
    {
      float draw_layer = (float) (0.800000011920929 - (double) (slotY + this.bounds.Y) * 9.99999997475243E-07);
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), slotX + this.bounds.X, slotY + this.bounds.Y, this.bounds.Width, this.bounds.Height, Color.White * (this.greyedOut ? 0.33f : 1f), 4f, draw_layer: draw_layer);
      Vector2 vector2 = Game1.dialogueFont.MeasureString(this.label) / 2f;
      vector2.X = (float) ((int) ((double) vector2.X / 4.0) * 4);
      vector2.Y = (float) ((int) ((double) vector2.Y / 4.0) * 4);
      Utility.drawTextWithShadow(b, this.label, Game1.dialogueFont, new Vector2((float) (slotX + this.bounds.Center.X), (float) (slotY + this.bounds.Center.Y)) - vector2, Game1.textColor * (this.greyedOut ? 0.33f : 1f), layerDepth: (draw_layer + 1E-06f), shadowIntensity: 0.0f);
    }
  }
}
