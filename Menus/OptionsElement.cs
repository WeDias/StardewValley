// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.OptionsElement
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;

namespace StardewValley.Menus
{
  public class OptionsElement
  {
    public const int defaultX = 8;
    public const int defaultY = 4;
    public const int defaultPixelWidth = 9;
    public Rectangle bounds;
    public string label;
    public int whichOption;
    public bool greyedOut;
    public Vector2 labelOffset = Vector2.Zero;
    public OptionsElement.Style style;

    public OptionsElement(string label)
    {
      this.label = label;
      this.bounds = new Rectangle(32, 16, 36, 36);
      this.whichOption = -1;
    }

    public OptionsElement(string label, int x, int y, int width, int height, int whichOption = -1)
    {
      if (x == -1)
        x = 32;
      if (y == -1)
        y = 16;
      this.bounds = new Rectangle(x, y, width, height);
      this.label = label;
      this.whichOption = whichOption;
    }

    public OptionsElement(string label, Rectangle bounds, int whichOption)
    {
      this.whichOption = whichOption;
      this.label = label;
      this.bounds = bounds;
    }

    public virtual void receiveLeftClick(int x, int y)
    {
    }

    public virtual void leftClickHeld(int x, int y)
    {
    }

    public virtual void leftClickReleased(int x, int y)
    {
    }

    public virtual void receiveKeyPress(Keys key)
    {
    }

    public virtual void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
    {
      if (this.style == OptionsElement.Style.OptionLabel)
        Utility.drawTextWithShadow(b, this.label, Game1.dialogueFont, new Vector2((float) (slotX + this.bounds.X + (int) this.labelOffset.X), (float) (slotY + this.bounds.Y + (int) this.labelOffset.Y + 12)), this.greyedOut ? Game1.textColor * 0.33f : Game1.textColor, layerDepth: 0.1f);
      else if (this.whichOption == -1)
      {
        SpriteText.drawString(b, this.label, slotX + this.bounds.X + (int) this.labelOffset.X, slotY + this.bounds.Y + (int) this.labelOffset.Y + 12, 999, height: 999, layerDepth: 0.1f);
      }
      else
      {
        int x = slotX + this.bounds.X + this.bounds.Width + 8 + (int) this.labelOffset.X;
        int y = slotY + this.bounds.Y + (int) this.labelOffset.Y;
        string text = this.label;
        SpriteFont spriteFont = Game1.dialogueFont;
        if (context != null)
        {
          int num = context.width - 64;
          int positionOnScreen = context.xPositionOnScreen;
          if ((double) spriteFont.MeasureString(this.label).X + (double) x > (double) (num + positionOnScreen))
          {
            int width = num + positionOnScreen - x;
            spriteFont = Game1.smallFont;
            text = Game1.parseText(this.label, spriteFont, width);
            y -= (int) (((double) spriteFont.MeasureString(text).Y - (double) spriteFont.MeasureString("T").Y) / 2.0);
          }
        }
        Utility.drawTextWithShadow(b, text, spriteFont, new Vector2((float) x, (float) y), this.greyedOut ? Game1.textColor * 0.33f : Game1.textColor, layerDepth: 0.1f);
      }
    }

    public enum Style
    {
      Default,
      OptionLabel,
    }
  }
}
