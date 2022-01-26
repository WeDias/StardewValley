// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.OptionsTextEntry
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley.Menus
{
  public class OptionsTextEntry : OptionsElement
  {
    public const int pixelsHigh = 11;
    public TextBox textBox;

    public OptionsTextEntry(string label, int whichOption, int x = -1, int y = -1)
      : base(label, x, y, (int) Game1.smallFont.MeasureString("Windowed Borderless Mode   ").X + 48, 44, whichOption)
    {
      this.textBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D) null, Game1.smallFont, Color.Black);
      this.textBox.Width = this.bounds.Width;
    }

    public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
    {
      this.textBox.X = slotX + this.bounds.Left - 8;
      this.textBox.Y = slotY + this.bounds.Top;
      this.textBox.Draw(b);
      base.draw(b, slotX, slotY, context);
    }

    public override void receiveLeftClick(int x, int y)
    {
      this.textBox.SelectMe();
      this.textBox.Update();
    }
  }
}
