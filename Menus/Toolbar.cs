// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.Toolbar
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
  public class Toolbar : IClickableMenu
  {
    private List<ClickableComponent> buttons = new List<ClickableComponent>();
    private new int yPositionOnScreen;
    private string hoverTitle = "";
    private Item hoverItem;
    private float transparency = 1f;
    public string[] slotText = new string[12]
    {
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
      "0",
      "-",
      "="
    };
    public Rectangle toolbarTextSource = new Rectangle(0, 256, 60, 60);

    public Toolbar()
      : base(Game1.uiViewport.Width / 2 - 384 - 64, Game1.uiViewport.Height, 896, 208)
    {
      for (int index = 0; index < 12; ++index)
        this.buttons.Add(new ClickableComponent(new Rectangle(Game1.uiViewport.Width / 2 - 384 + index * 64, this.yPositionOnScreen - 96 + 8, 64, 64), index.ToString() ?? ""));
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (Game1.player.UsingTool || Game1.IsChatting)
        return;
      foreach (ClickableComponent button in this.buttons)
      {
        if (button.containsPoint(x, y))
        {
          Game1.player.CurrentToolIndex = Convert.ToInt32(button.name);
          if (Game1.player.ActiveObject != null)
          {
            Game1.player.showCarrying();
            Game1.playSound("pickUpItem");
            break;
          }
          Game1.player.showNotCarrying();
          Game1.playSound("stoneStep");
          break;
        }
      }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      this.hoverItem = (Item) null;
      foreach (ClickableComponent button in this.buttons)
      {
        if (button.containsPoint(x, y))
        {
          int int32 = Convert.ToInt32(button.name);
          if (int32 < Game1.player.items.Count && Game1.player.items[int32] != null)
          {
            button.scale = Math.Min(button.scale + 0.05f, 1.1f);
            this.hoverTitle = Game1.player.items[int32].DisplayName;
            this.hoverItem = Game1.player.items[int32];
          }
        }
        else
          button.scale = Math.Max(button.scale - 0.025f, 1f);
      }
    }

    public void shifted(bool right)
    {
      if (right)
      {
        for (int index = 0; index < this.buttons.Count; ++index)
          this.buttons[index].scale = (float) (1.0 + (double) index * 0.0299999993294477);
      }
      else
      {
        for (int index = this.buttons.Count - 1; index >= 0; --index)
          this.buttons[index].scale = (float) (1.0 + (double) (11 - index) * 0.0299999993294477);
      }
    }

    public override void update(GameTime time)
    {
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      for (int index = 0; index < 12; ++index)
        this.buttons[index].bounds = new Rectangle(Game1.uiViewport.Width / 2 - 384 + index * 64, this.yPositionOnScreen - 96 + 8, 64, 64);
    }

    public override bool isWithinBounds(int x, int y) => new Rectangle(this.buttons.First<ClickableComponent>().bounds.X, this.buttons.First<ClickableComponent>().bounds.Y, this.buttons.Last<ClickableComponent>().bounds.X - this.buttons.First<ClickableComponent>().bounds.X + 64, 64).Contains(x, y);

    public override void draw(SpriteBatch b)
    {
      if (Game1.activeClickableMenu != null)
        return;
      Point center = Game1.player.GetBoundingBox().Center;
      Vector2 globalPosition = new Vector2((float) center.X, (float) center.Y);
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, globalPosition);
      bool flag;
      if (Game1.options.pinToolbarToggle)
      {
        flag = false;
        this.transparency = Math.Min(1f, this.transparency + 0.075f);
        if ((double) local.Y > (double) (Game1.viewport.Height - 192))
          this.transparency = Math.Max(0.33f, this.transparency - 0.15f);
      }
      else
      {
        flag = (double) local.Y > (double) (Game1.viewport.Height / 2 + 64);
        this.transparency = 1f;
      }
      int num = Utility.makeSafeMarginY(8);
      int positionOnScreen1 = this.yPositionOnScreen;
      if (!flag)
      {
        this.yPositionOnScreen = Game1.uiViewport.Height;
        this.yPositionOnScreen += 8;
        this.yPositionOnScreen -= num;
      }
      else
      {
        this.yPositionOnScreen = 112;
        this.yPositionOnScreen -= 8;
        this.yPositionOnScreen += num;
      }
      int positionOnScreen2 = this.yPositionOnScreen;
      if (positionOnScreen1 != positionOnScreen2)
      {
        for (int index = 0; index < 12; ++index)
          this.buttons[index].bounds.Y = this.yPositionOnScreen - 96 + 8;
      }
      IClickableMenu.drawTextureBox(b, Game1.menuTexture, this.toolbarTextSource, Game1.uiViewport.Width / 2 - 384 - 16, this.yPositionOnScreen - 96 - 8, 800, 96, Color.White * this.transparency, drawShadow: false);
      for (int index = 0; index < 12; ++index)
      {
        Vector2 position = new Vector2((float) (Game1.uiViewport.Width / 2 - 384 + index * 64), (float) (this.yPositionOnScreen - 96 + 8));
        b.Draw(Game1.menuTexture, position, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, Game1.player.CurrentToolIndex == index ? 56 : 10)), Color.White * this.transparency);
        if (!Game1.options.gamepadControls)
          b.DrawString(Game1.tinyFont, this.slotText[index], position + new Vector2(4f, -8f), Color.DimGray * this.transparency);
      }
      for (int index = 0; index < 12; ++index)
      {
        this.buttons[index].scale = Math.Max(1f, this.buttons[index].scale - 0.025f);
        Vector2 location = new Vector2((float) (Game1.uiViewport.Width / 2 - 384 + index * 64), (float) (this.yPositionOnScreen - 96 + 8));
        if (Game1.player.items.Count > index && Game1.player.items.ElementAt<Item>(index) != null)
          Game1.player.items[index].drawInMenu(b, location, Game1.player.CurrentToolIndex == index ? 0.9f : this.buttons.ElementAt<ClickableComponent>(index).scale * 0.8f, this.transparency, 0.88f);
      }
      if (this.hoverItem == null)
        return;
      IClickableMenu.drawToolTip(b, this.hoverItem.getDescription(), this.hoverItem.DisplayName, this.hoverItem);
      this.hoverItem = (Item) null;
    }
  }
}
