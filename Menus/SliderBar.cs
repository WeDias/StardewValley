// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.SliderBar
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StardewValley.Menus
{
  public class SliderBar
  {
    public static int defaultWidth = 128;
    public const int defaultHeight = 20;
    public int value;
    public Rectangle bounds;

    public SliderBar(int x, int y, int initialValue)
    {
      this.bounds = new Rectangle(x, y, SliderBar.defaultWidth, 20);
      this.value = initialValue;
    }

    public int click(int x, int y)
    {
      if (this.bounds.Contains(x, y))
      {
        x -= this.bounds.X;
        this.value = (int) ((double) x / (double) this.bounds.Width * 100.0);
      }
      return this.value;
    }

    public void changeValueBy(int amount)
    {
      this.value += amount;
      this.value = Math.Max(0, Math.Min(100, this.value));
    }

    public void release(int x, int y)
    {
    }

    public void draw(SpriteBatch b)
    {
      b.Draw(Game1.staminaRect, new Rectangle(this.bounds.X, this.bounds.Center.Y - 2, this.bounds.Width, 4), Color.DarkGray);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.bounds.X + (int) ((double) this.value / 100.0 * (double) this.bounds.Width) + 4), (float) this.bounds.Center.Y), new Rectangle?(new Rectangle(64, 256, 32, 32)), Color.White, 0.0f, new Vector2(16f, 9f), 1f, SpriteEffects.None, 0.86f);
    }
  }
}
