// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.AboutMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StardewValley.Menus
{
  public class AboutMenu : IClickableMenu
  {
    public const int region_linkToTwitter = 91111;
    public const int region_linkToSVSite = 92222;
    public const int region_linkToChucklefish = 93333;
    public const int region_upArrow = 94444;
    public const int region_downArrow = 95555;
    public const int minWidth = 950;
    public const int maxWidth = 1200;
    public new const int height = 700;
    public ClickableComponent backButton;
    public ClickableTextureComponent upButton;
    public ClickableTextureComponent downButton;
    public List<ICreditsBlock> credits = new List<ICreditsBlock>();
    private int currentCreditsIndex;

    public AboutMenu()
    {
      this.width = 1280;
      base.height = 700;
      this.SetUpCredits();
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    public void SetUpCredits()
    {
      foreach (string rawtext in Game1.temporaryContent.Load<List<string>>("Strings\\credits"))
      {
        if (rawtext != "" && rawtext.Length >= 6 && rawtext.Substring(0, 6) == "[image")
        {
          string[] source = rawtext.Split(' ');
          string assetName = source[1];
          int int32_1 = Convert.ToInt32(source[2]);
          int int32_2 = Convert.ToInt32(source[3]);
          int width = Convert.ToInt32(source[4]);
          int height = Convert.ToInt32(source[5]);
          int int32_3 = Convert.ToInt32(source[6]);
          int animationFrames = ((IEnumerable<string>) source).Count<string>() > 7 ? Convert.ToInt32(source[7]) : 1;
          Texture2D texture = (Texture2D) null;
          try
          {
            texture = Game1.temporaryContent.Load<Texture2D>(assetName);
          }
          catch (Exception ex)
          {
          }
          if (texture != null)
          {
            if (width == -1)
            {
              width = texture.Width;
              height = texture.Height;
            }
            this.credits.Add((ICreditsBlock) new ImageCreditsBlock(texture, new Rectangle(int32_1, int32_2, width, height), int32_3, animationFrames));
          }
        }
        else if (rawtext != "" && rawtext.Length >= 6 && rawtext.Substring(0, 5) == "[link")
        {
          string url = rawtext.Split(' ')[1];
          string str = rawtext.Substring(rawtext.IndexOf(' ') + 1);
          this.credits.Add((ICreditsBlock) new LinkCreditsBlock(str.Substring(str.IndexOf(' ') + 1), url));
        }
        else
          this.credits.Add((ICreditsBlock) new TextCreditsBlock(rawtext));
      }
      if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.de && LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.es)
      {
        int currentLanguageCode = (int) LocalizedContentManager.CurrentLanguageCode;
      }
      Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, 700);
      this.xPositionOnScreen = (int) centeringOnScreen.X;
      this.yPositionOnScreen = (int) centeringOnScreen.Y;
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle((int) centeringOnScreen.X + this.width - 80, (int) centeringOnScreen.Y + 64 + 16, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12), 0.8f);
      textureComponent1.myID = 94444;
      textureComponent1.downNeighborID = 95555;
      textureComponent1.rightNeighborID = -99998;
      textureComponent1.leftNeighborID = -99998;
      this.upButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle((int) centeringOnScreen.X + this.width - 80, (int) centeringOnScreen.Y + 700 - 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11), 0.8f);
      textureComponent2.myID = 95555;
      textureComponent2.upNeighborID = -99998;
      textureComponent2.rightNeighborID = -99998;
      textureComponent2.leftNeighborID = -99998;
      this.downButton = textureComponent2;
      this.backButton = new ClickableComponent(new Rectangle(Game1.uiViewport.Width - 198 - 48, Game1.uiViewport.Height - 81 - 24, 198, 81), "")
      {
        myID = 81114,
        leftNeighborID = -99998,
        rightNeighborID = -99998,
        upNeighborID = 95555
      };
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(81114);
      this.snapCursorToCurrentSnappedComponent();
    }

    private static void LaunchBrowser(string url)
    {
      Game1.playSound("bigSelect");
      try
      {
        Process.Start(url);
      }
      catch (Exception ex)
      {
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);
      if (this.upButton.containsPoint(x, y))
      {
        if (this.currentCreditsIndex <= 0)
          return;
        --this.currentCreditsIndex;
        Game1.playSound("shiny4");
        this.upButton.scale = this.upButton.baseScale;
      }
      else if (this.downButton.containsPoint(x, y))
      {
        if (this.currentCreditsIndex >= this.credits.Count - 1)
          return;
        ++this.currentCreditsIndex;
        Game1.playSound("shiny4");
        this.downButton.scale = this.downButton.baseScale;
      }
      else
      {
        if (!this.isWithinBounds(x, y))
          return;
        int num1 = this.yPositionOnScreen + 96;
        int num2 = num1;
        int num3 = 0;
        while (num1 < this.yPositionOnScreen + 700 - 64 && this.credits.Count<ICreditsBlock>() > this.currentCreditsIndex + num3)
        {
          num1 += this.credits[this.currentCreditsIndex + num3].getHeight(this.width - 64) + (this.credits.Count<ICreditsBlock>() <= this.currentCreditsIndex + num3 + 1 || !(this.credits[this.currentCreditsIndex + num3 + 1] is ImageCreditsBlock) ? 8 : 0);
          if (y >= num2 && y < num1)
          {
            this.credits[this.currentCreditsIndex + num3].clicked();
            break;
          }
          ++num3;
          num2 = num1;
        }
      }
    }

    public override void update(GameTime time)
    {
      base.update(time);
      this.upButton.visible = this.currentCreditsIndex > 0;
      this.downButton.visible = this.currentCreditsIndex < this.credits.Count<ICreditsBlock>() - 1;
    }

    public override void receiveScrollWheelAction(int direction)
    {
      if (direction > 0 && this.currentCreditsIndex > 0)
      {
        --this.currentCreditsIndex;
        Game1.playSound("shiny4");
      }
      else
      {
        if (direction >= 0 || this.currentCreditsIndex >= this.credits.Count - 1)
          return;
        ++this.currentCreditsIndex;
        Game1.playSound("shiny4");
      }
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      this.upButton.tryHover(x, y);
      this.downButton.tryHover(x, y);
      if (!this.isWithinBounds(x, y))
        return;
      int num1 = this.yPositionOnScreen + 96;
      int num2 = num1;
      int num3 = 0;
      while (num1 < this.yPositionOnScreen + 700 - 64 && this.credits.Count<ICreditsBlock>() > this.currentCreditsIndex + num3)
      {
        num1 += this.credits[this.currentCreditsIndex + num3].getHeight(this.width - 64) + (this.credits.Count<ICreditsBlock>() <= this.currentCreditsIndex + num3 + 1 || !(this.credits[this.currentCreditsIndex + num3 + 1] is ImageCreditsBlock) ? 8 : 0);
        if (y >= num2 && y < num1)
        {
          this.credits[this.currentCreditsIndex + num3].hovered();
          break;
        }
        ++num3;
        num2 = num1;
      }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void draw(SpriteBatch b)
    {
      Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, 600);
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(473, 36, 24, 24), (int) centeringOnScreen.X, (int) centeringOnScreen.Y, this.width, 700, Color.White, 4f, false);
      int topLeftY = this.yPositionOnScreen + 96;
      for (int index = 0; topLeftY < this.yPositionOnScreen + 700 - 64 && this.credits.Count<ICreditsBlock>() > this.currentCreditsIndex + index; ++index)
      {
        this.credits[this.currentCreditsIndex + index].draw(this.xPositionOnScreen + 32, topLeftY, this.width - 64, b);
        topLeftY += this.credits[this.currentCreditsIndex + index].getHeight(this.width - 64) + (this.credits.Count<ICreditsBlock>() <= this.currentCreditsIndex + index + 1 || !(this.credits[this.currentCreditsIndex + index + 1] is ImageCreditsBlock) ? 8 : 0);
      }
      if (this.currentCreditsIndex > 0)
        this.upButton.draw(b);
      if (this.currentCreditsIndex < this.credits.Count<ICreditsBlock>() - 1)
        this.downButton.draw(b);
      if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is TitleMenu && (Game1.activeClickableMenu as TitleMenu).startupMessage.Length > 0)
        b.DrawString(Game1.smallFont, Game1.parseText((Game1.activeClickableMenu as TitleMenu).startupMessage, Game1.smallFont, 640), new Vector2(8f, (float) ((double) Game1.uiViewport.Height - (double) Game1.smallFont.MeasureString(Game1.parseText((Game1.activeClickableMenu as TitleMenu).startupMessage, Game1.smallFont, 640)).Y - 4.0)), Color.White);
      else
        b.DrawString(Game1.smallFont, "v" + Game1.GetVersionString(), new Vector2(16f, (float) ((double) Game1.uiViewport.Height - (double) Game1.smallFont.MeasureString("v" + Game1.version).Y - 8.0)), Color.White);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.SetUpCredits();
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
        return;
      int id = this.currentlySnappedComponent != null ? this.currentlySnappedComponent.myID : 81114;
      this.populateClickableComponentList();
      this.currentlySnappedComponent = this.getComponentWithID(id);
      this.snapCursorToCurrentSnappedComponent();
    }
  }
}
