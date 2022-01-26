// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TitleTextInputMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StardewValley.Menus
{
  public class TitleTextInputMenu : NamingMenu
  {
    public ClickableTextureComponent pasteButton;
    public const int region_pasteButton = 105;
    public string context = "";

    public TitleTextInputMenu(
      string title,
      NamingMenu.doneNamingBehavior b,
      string default_text = "",
      string context = "")
      : base(b, title, "")
    {
      this.context = context;
      this.textBox.limitWidth = false;
      this.textBox.Width = 512;
      this.textBox.X -= 128;
      this.randomButton.visible = false;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.textBox.X + this.textBox.Width + 32 + 4 + 64, Game1.viewport.Height / 2 - 8, 64, 64), Game1.mouseCursors, new Rectangle(274, 284, 16, 16), 4f);
      textureComponent.myID = 105;
      textureComponent.leftNeighborID = 102;
      this.pasteButton = textureComponent;
      this.pasteButton.visible = DesktopClipboard.IsAvailable;
      this.doneNamingButton.rightNeighborID = 105;
      this.doneNamingButton.bounds.X += 128;
      this.minLength = 0;
      if (Game1.options.SnappyMenus)
      {
        this.populateClickableComponentList();
        this.snapToDefaultClickableComponent();
      }
      this.textBox.Text = default_text;
    }

    public override void performHoverAction(int x, int y)
    {
      if (this.pasteButton != null)
        this.pasteButton.tryHover(x, y);
      base.performHoverAction(x, y);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.pasteButton != null && this.pasteButton.containsPoint(x, y))
      {
        string output = "";
        if (DesktopClipboard.GetText(ref output))
        {
          Game1.playSound("drumkit6");
          this.textBox.Text = output;
        }
        else
          Game1.playSound("cancel");
      }
      base.receiveLeftClick(x, y, playSound);
    }

    public override void update(GameTime time)
    {
      GamePadState gamePadState = Game1.input.GetGamePadState();
      KeyboardState keyboardState = Game1.GetKeyboardState();
      if (Game1.IsPressEvent(ref gamePadState, Buttons.B) || Game1.IsPressEvent(ref keyboardState, Keys.Escape))
      {
        if (Game1.activeClickableMenu is TitleMenu)
          (Game1.activeClickableMenu as TitleMenu).backButtonPressed();
        else
          Game1.exitActiveMenu();
      }
      base.update(time);
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (this.pasteButton == null)
        return;
      this.pasteButton.draw(b);
    }
  }
}
