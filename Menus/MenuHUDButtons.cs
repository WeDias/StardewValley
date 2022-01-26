// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.MenuHUDButtons
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class MenuHUDButtons : IClickableMenu
  {
    public new const int width = 70;
    public new const int height = 21;
    private List<ClickableComponent> buttons = new List<ClickableComponent>();
    private string hoverText = "";
    private Vector2 position;
    private Rectangle sourceRect;

    public MenuHUDButtons()
      : base(Game1.uiViewport.Width / 2 + 384 + 64, Game1.uiViewport.Height - 84 - 16, 280, 84)
    {
      for (int index = 0; index < 7; ++index)
        this.buttons.Add(new ClickableComponent(new Rectangle(Game1.uiViewport.Width / 2 + 384 + 16 + index * 9 * 4, this.yPositionOnScreen + 20, 36, 44), index.ToString() ?? ""));
      this.position = new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen);
      this.sourceRect = new Rectangle(221, 362, 70, 21);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (Game1.player.UsingTool)
        return;
      foreach (ClickableComponent button in this.buttons)
      {
        if (button.containsPoint(x, y))
        {
          Game1.activeClickableMenu = (IClickableMenu) new GameMenu(Convert.ToInt32(button.name));
          Game1.playSound("bigSelect");
          break;
        }
      }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      this.hoverText = "";
      foreach (ClickableComponent button in this.buttons)
      {
        if (button.containsPoint(x, y))
          this.hoverText = GameMenu.getLabelOfTabFromIndex(Convert.ToInt32(button.name));
      }
    }

    public override void update(GameTime time)
    {
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 + 384 + 64;
      this.yPositionOnScreen = Game1.uiViewport.Height - 84 - 16;
      for (int index = 0; index < 7; ++index)
        this.buttons[index].bounds = new Rectangle(Game1.uiViewport.Width / 2 + 384 + 16 + index * 9 * 4, this.yPositionOnScreen + 20, 36, 44);
      this.position = new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen);
    }

    public override bool isWithinBounds(int x, int y) => new Rectangle(this.buttons.First<ClickableComponent>().bounds.X, this.buttons.First<ClickableComponent>().bounds.Y, this.buttons.Last<ClickableComponent>().bounds.X - this.buttons.First<ClickableComponent>().bounds.X + 64, 64).Contains(x, y);

    public override void draw(SpriteBatch b)
    {
      if (Game1.activeClickableMenu != null)
        return;
      b.Draw(Game1.mouseCursors, this.position, new Rectangle?(this.sourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      if (this.hoverText.Equals("") || !this.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
        return;
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    }
  }
}
