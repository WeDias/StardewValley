// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ChooseFromListMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class ChooseFromListMenu : IClickableMenu
  {
    public const int region_backButton = 101;
    public const int region_forwardButton = 102;
    public const int region_okButton = 103;
    public const int region_cancelButton = 104;
    public const int w = 640;
    public const int h = 192;
    public ClickableTextureComponent backButton;
    public ClickableTextureComponent forwardButton;
    public ClickableTextureComponent okButton;
    public ClickableTextureComponent cancelButton;
    private List<string> options = new List<string>();
    private int index;
    private ChooseFromListMenu.actionOnChoosingListOption chooseAction;
    private bool isJukebox;

    public ChooseFromListMenu(
      List<string> options,
      ChooseFromListMenu.actionOnChoosingListOption chooseAction,
      bool isJukebox = false,
      string default_selection = null)
      : base(Game1.uiViewport.Width / 2 - 320, Game1.uiViewport.Height - 64 - 192, 640, 192)
    {
      this.chooseAction = chooseAction;
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - 128 - 4, this.yPositionOnScreen + 85, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent1.myID = 101;
      textureComponent1.rightNeighborID = 102;
      this.backButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 640 + 16 + 64, this.yPositionOnScreen + 85, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent2.myID = 102;
      textureComponent2.leftNeighborID = 101;
      textureComponent2.rightNeighborID = 103;
      this.forwardButton = textureComponent2;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width + 128 + 8, this.yPositionOnScreen + 192 - 128, 64, 64), (string) null, (string) null, Game1.mouseCursors, new Rectangle(175, 379, 16, 15), 4f);
      textureComponent3.myID = 103;
      textureComponent3.leftNeighborID = 102;
      textureComponent3.rightNeighborID = 104;
      this.okButton = textureComponent3;
      ClickableTextureComponent textureComponent4 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width + 192 + 12, this.yPositionOnScreen + 192 - 128, 64, 64), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
      textureComponent4.myID = 104;
      textureComponent4.leftNeighborID = 103;
      this.cancelButton = textureComponent4;
      Game1.playSound("bigSelect");
      this.isJukebox = isJukebox;
      if (isJukebox)
        ChooseFromListMenu.FilterJukeboxTracks(options);
      this.options = options;
      if (default_selection != null)
      {
        int num = options.IndexOf(default_selection);
        if (num >= 0)
          this.index = num;
      }
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public static void FilterJukeboxTracks(List<string> options)
    {
      for (int index = options.Count - 1; index >= 0; --index)
      {
        if (!ChooseFromListMenu.IsValidJukeboxSong(options[index]))
        {
          options.RemoveAt(index);
        }
        else
        {
          switch (options[index])
          {
            case "buglevelloop":
              options.RemoveAt(index);
              continue;
            case "coin":
              options.RemoveAt(index);
              continue;
            case "communityCenter":
              options.RemoveAt(index);
              continue;
            case "jojaOfficeSoundscape":
              options.RemoveAt(index);
              continue;
            case "nightTime":
              options.RemoveAt(index);
              continue;
            case "ocean":
              options.RemoveAt(index);
              continue;
            case "title_day":
              options.RemoveAt(index);
              options.Add("MainTheme");
              continue;
            default:
              continue;
          }
        }
      }
    }

    public static bool IsValidJukeboxSong(string name)
    {
      name = name.ToLower();
      return !(name.Trim() == "") && !name.Contains("ambient") && !name.Contains("bigdrums") && !name.Contains("clubloop") && !name.Contains("ambience");
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(103);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - 320;
      this.yPositionOnScreen = Game1.uiViewport.Height - 64 - 192;
      this.backButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - 128 - 4, this.yPositionOnScreen + 85, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      this.forwardButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 640 + 16 + 64, this.yPositionOnScreen + 85, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      this.okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width + 128 + 8, this.yPositionOnScreen + 192 - 128, 64, 64), (string) null, (string) null, Game1.mouseCursors, new Rectangle(175, 379, 16, 15), 4f);
      this.cancelButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width + 192 + 12, this.yPositionOnScreen + 192 - 128, 64, 64), (string) null, (string) null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
    }

    public static void playSongAction(string s)
    {
      Game1.startedJukeboxMusic = true;
      Game1.changeMusicTrack(s);
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      this.okButton.tryHover(x, y);
      this.cancelButton.tryHover(x, y);
      this.backButton.tryHover(x, y);
      this.forwardButton.tryHover(x, y);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);
      if (this.okButton.containsPoint(x, y) && this.chooseAction != null)
      {
        this.chooseAction(this.options[this.index]);
        Game1.playSound("select");
      }
      if (this.cancelButton.containsPoint(x, y))
        this.exitThisMenu();
      if (this.backButton.containsPoint(x, y))
      {
        --this.index;
        if (this.index < 0)
          this.index = this.options.Count - 1;
        this.backButton.scale = this.backButton.baseScale - 1f;
        Game1.playSound("shwip");
      }
      if (!this.forwardButton.containsPoint(x, y))
        return;
      ++this.index;
      this.index %= this.options.Count;
      Game1.playSound("shwip");
      this.forwardButton.scale = this.forwardButton.baseScale - 1f;
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      string str = "Summer (The Sun Can Bend An Orange Sky)";
      int x = (int) Game1.dialogueFont.MeasureString(this.isJukebox ? str : this.options[this.index]).X;
      IClickableMenu.drawTextureBox(b, this.xPositionOnScreen + this.width / 2 - x / 2 - 16, this.yPositionOnScreen + 64 - 4, x + 32, 80, Color.White);
      if (this.index < this.options.Count)
        Utility.drawTextWithShadow(b, this.isJukebox ? Utility.getSongTitleFromCueName(this.options[this.index]) : this.options[this.index], Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + this.width / 2) - Game1.dialogueFont.MeasureString(this.isJukebox ? Utility.getSongTitleFromCueName(this.options[this.index]) : this.options[this.index]).X / 2f, (float) (this.yPositionOnScreen + this.height / 2 - 16)), Game1.textColor);
      this.okButton.draw(b);
      this.cancelButton.draw(b);
      this.forwardButton.draw(b);
      this.backButton.draw(b);
      if (this.isJukebox)
        SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\UI:JukeboxMenu_Title"), this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen - 32);
      this.drawMouse(b);
    }

    public delegate void actionOnChoosingListOption(string s);
  }
}
