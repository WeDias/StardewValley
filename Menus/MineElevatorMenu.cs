// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.MineElevatorMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class MineElevatorMenu : IClickableMenu
  {
    public List<ClickableComponent> elevators = new List<ClickableComponent>();

    public MineElevatorMenu()
      : base(0, 0, 0, 0, true)
    {
      int num1 = Math.Min(MineShaft.lowestLevelReached, 120) / 5;
      this.width = num1 > 50 ? 484 + IClickableMenu.borderWidth * 2 : Math.Min(220 + IClickableMenu.borderWidth * 2, num1 * 44 + IClickableMenu.borderWidth * 2);
      this.height = Math.Max(64 + IClickableMenu.borderWidth * 3, num1 * 44 / (this.width - IClickableMenu.borderWidth) * 44 + 64 + IClickableMenu.borderWidth * 3);
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.width / 2;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.height / 2;
      Game1.playSound("crystal");
      int num2 = this.width / 44 - 1;
      int x1 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder * 3 / 4;
      int y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.borderWidth / 3;
      List<ClickableComponent> elevators1 = this.elevators;
      Rectangle bounds1 = new Rectangle(x1, y, 44, 44);
      int num3 = 0;
      string name1 = num3.ToString() ?? "";
      elevators1.Add(new ClickableComponent(bounds1, name1)
      {
        myID = 0,
        rightNeighborID = 1,
        downNeighborID = num2
      });
      int x2 = x1 + 64 - 20;
      if (x2 > this.xPositionOnScreen + this.width - IClickableMenu.borderWidth)
      {
        x2 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder * 3 / 4;
        y += 44;
      }
      for (int index = 1; index <= num1; ++index)
      {
        List<ClickableComponent> elevators2 = this.elevators;
        Rectangle bounds2 = new Rectangle(x2, y, 44, 44);
        num3 = index * 5;
        string name2 = num3.ToString() ?? "";
        elevators2.Add(new ClickableComponent(bounds2, name2)
        {
          myID = index,
          rightNeighborID = index % num2 == num2 - 1 ? -1 : index + 1,
          leftNeighborID = index % num2 == 0 ? -1 : index - 1,
          downNeighborID = index + num2,
          upNeighborID = index - num2
        });
        x2 = x2 + 64 - 20;
        if (x2 > this.xPositionOnScreen + this.width - IClickableMenu.borderWidth)
        {
          x2 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder * 3 / 4;
          y += 44;
        }
      }
      this.initializeUpperRightCloseButton();
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.isWithinBounds(x, y))
      {
        foreach (ClickableComponent elevator in this.elevators)
        {
          if (elevator.containsPoint(x, y))
          {
            Game1.playSound("smallSelect");
            if (Convert.ToInt32(elevator.name) == 0)
            {
              if (!(Game1.currentLocation is MineShaft))
                return;
              Game1.warpFarmer("Mine", 17, 4, true);
              Game1.exitActiveMenu();
              Game1.changeMusicTrack("none");
            }
            else
            {
              if (Convert.ToInt32(elevator.name) == Game1.CurrentMineLevel)
                return;
              Game1.player.ridingMineElevator = true;
              Game1.enterMine(Convert.ToInt32(elevator.name));
              Game1.exitActiveMenu();
            }
          }
        }
        base.receiveLeftClick(x, y);
      }
      else
        Game1.exitActiveMenu();
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      foreach (ClickableComponent elevator in this.elevators)
        elevator.scale = !elevator.containsPoint(x, y) ? 1f : 2f;
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
      Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen - 64 + 8, this.width + 21, this.height + 64, false, true);
      foreach (ClickableComponent elevator in this.elevators)
      {
        b.Draw(Game1.mouseCursors, new Vector2((float) (elevator.bounds.X - 4), (float) (elevator.bounds.Y + 4)), new Rectangle?(new Rectangle((double) elevator.scale > 1.0 ? 267 : 256, 256, 10, 10)), Color.Black * 0.5f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
        b.Draw(Game1.mouseCursors, new Vector2((float) elevator.bounds.X, (float) elevator.bounds.Y), new Rectangle?(new Rectangle((double) elevator.scale > 1.0 ? 267 : 256, 256, 10, 10)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.868f);
        Vector2 position = new Vector2((float) (elevator.bounds.X + 16 + NumberSprite.numberOfDigits(Convert.ToInt32(elevator.name)) * 6), (float) (elevator.bounds.Y + 24 - NumberSprite.getHeight() / 4));
        NumberSprite.draw(Convert.ToInt32(elevator.name), b, position, Game1.CurrentMineLevel == Convert.ToInt32(elevator.name) ? Color.Gray * 0.75f : Color.Gold, 0.5f, 0.86f, 1f, 0);
      }
      base.draw(b);
      this.drawMouse(b);
    }
  }
}
