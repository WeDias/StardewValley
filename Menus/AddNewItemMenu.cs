// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.AddNewItemMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley.Menus
{
  public class AddNewItemMenu : IClickableMenu
  {
    private InventoryMenu playerInventory;
    private ClickableComponent garbage;

    public AddNewItemMenu()
      : base(Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (300 + IClickableMenu.borderWidth * 2) / 2, 800 + IClickableMenu.borderWidth * 2, 300 + IClickableMenu.borderWidth * 2)
    {
      this.playerInventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder, true);
      this.garbage = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width + IClickableMenu.spaceToClearSideBorder, this.yPositionOnScreen + this.height - 64, 64, 64), "Garbage");
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
    }

    public override void draw(SpriteBatch b)
    {
    }
  }
}
