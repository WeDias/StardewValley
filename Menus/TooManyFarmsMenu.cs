// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TooManyFarmsMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;

namespace StardewValley.Menus
{
  public class TooManyFarmsMenu : IClickableMenu
  {
    public const int cWidth = 800;
    public const int cHeight = 180;

    public TooManyFarmsMenu()
    {
      Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800, 180);
      this.initialize((int) centeringOnScreen.X, (int) centeringOnScreen.Y, 800, 180);
    }

    public override bool readyToClose() => true;

    public override void receiveLeftClick(int x, int y, bool playSound = true) => this.exitThisMenu();

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public void drawBox(SpriteBatch b, int xPos, int yPos, int boxWidth, int boxHeight)
    {
      b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos, boxWidth, boxHeight), new Rectangle?(new Rectangle(306, 320, 16, 16)), Color.White);
      b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos - 20, boxWidth, 24), new Rectangle?(new Rectangle(275, 313, 1, 6)), Color.White);
      b.Draw(Game1.mouseCursors, new Rectangle(xPos + 12, yPos + boxHeight, boxWidth - 20, 32), new Rectangle?(new Rectangle(275, 328, 1, 8)), Color.White);
      b.Draw(Game1.mouseCursors, new Rectangle(xPos - 32, yPos + 24, 32, boxHeight - 28), new Rectangle?(new Rectangle(264, 325, 8, 1)), Color.White);
      b.Draw(Game1.mouseCursors, new Rectangle(xPos + boxWidth, yPos, 28, boxHeight), new Rectangle?(new Rectangle(293, 324, 7, 1)), Color.White);
      b.Draw(Game1.mouseCursors, new Vector2((float) (xPos - 44), (float) (yPos - 28)), new Rectangle?(new Rectangle(261, 311, 14, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (xPos + boxWidth - 8), (float) (yPos - 28)), new Rectangle?(new Rectangle(291, 311, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (xPos + boxWidth - 8), (float) (yPos + boxHeight - 8)), new Rectangle?(new Rectangle(291, 326, 12, 12)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (xPos - 44), (float) (yPos + boxHeight - 4)), new Rectangle?(new Rectangle(261, 327, 14, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    }

    public override void update(GameTime time) => base.update(time);

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
      this.drawBox(b, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
      int num = 35;
      string s = Game1.content.LoadString("Strings\\UI:TooManyFarmsMenu_TooManyFarms");
      SpriteText.drawString(b, s, this.xPositionOnScreen + num, this.yPositionOnScreen + num, width: this.width, height: this.height);
      int y = 260;
      Rectangle destinationRectangle = new Rectangle(this.xPositionOnScreen + this.width - 14 - 52, this.yPositionOnScreen + this.height - 14 - 52, 52, 52);
      Rectangle rectangle = new Rectangle(542, y, 26, 26);
      if (!Game1.options.gamepadControls)
        return;
      b.Draw(Game1.controllerMaps, destinationRectangle, new Rectangle?(rectangle), Color.White);
    }
  }
}
