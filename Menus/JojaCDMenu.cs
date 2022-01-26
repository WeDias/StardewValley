// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.JojaCDMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class JojaCDMenu : IClickableMenu
  {
    public new const int width = 1280;
    public new const int height = 576;
    public const int buttonWidth = 147;
    public const int buttonHeight = 30;
    private Texture2D noteTexture;
    public List<ClickableComponent> checkboxes = new List<ClickableComponent>();
    private string hoverText;
    private bool boughtSomething;
    private int exitTimer = -1;

    public JojaCDMenu(Texture2D noteTexture)
      : base(Game1.uiViewport.Width / 2 - 640, Game1.uiViewport.Height / 2 - 288, 1280, 576, true)
    {
      Game1.player.forceCanMove();
      this.noteTexture = noteTexture;
      int x = this.xPositionOnScreen + 4;
      int y = this.yPositionOnScreen + 208;
      for (int index = 0; index < 5; ++index)
      {
        this.checkboxes.Add(new ClickableComponent(new Rectangle(x, y, 588, 120), index.ToString() ?? "")
        {
          myID = index,
          rightNeighborID = index % 2 != 0 || index == 4 ? -1 : index + 1,
          leftNeighborID = index % 2 == 0 ? -1 : index - 1,
          downNeighborID = index + 2,
          upNeighborID = index - 2
        });
        x += 592;
        if (x > this.xPositionOnScreen + 1184)
        {
          x = this.xPositionOnScreen + 4;
          y += 120;
        }
      }
      if (Utility.doesAnyFarmerHaveOrWillReceiveMail("ccVault"))
        this.checkboxes[0].name = "complete";
      if (Utility.doesAnyFarmerHaveOrWillReceiveMail("ccBoilerRoom"))
        this.checkboxes[1].name = "complete";
      if (Utility.doesAnyFarmerHaveOrWillReceiveMail("ccCraftsRoom"))
        this.checkboxes[2].name = "complete";
      if (Utility.doesAnyFarmerHaveOrWillReceiveMail("ccPantry"))
        this.checkboxes[3].name = "complete";
      if (Utility.doesAnyFarmerHaveOrWillReceiveMail("ccFishTank"))
        this.checkboxes[4].name = "complete";
      this.exitFunction = new IClickableMenu.onExit(this.onExitFunction);
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
      Game1.mouseCursorTransparency = 1f;
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    private void onExitFunction()
    {
      if (!this.boughtSomething)
        return;
      JojaMart.Morris.setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Morris_JojaCDConfirm"));
      Game1.drawDialogue(JojaMart.Morris);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.exitTimer >= 0)
        return;
      base.receiveLeftClick(x, y);
      foreach (ClickableComponent checkbox in this.checkboxes)
      {
        if (checkbox.containsPoint(x, y) && !checkbox.name.Equals("complete"))
        {
          int int32 = Convert.ToInt32(checkbox.name);
          int fromButtonNumber = this.getPriceFromButtonNumber(int32);
          if (Game1.player.Money >= fromButtonNumber)
          {
            Game1.player.Money -= fromButtonNumber;
            Game1.playSound("reward");
            checkbox.name = "complete";
            this.boughtSomething = true;
            switch (int32)
            {
              case 0:
                Game1.addMailForTomorrow("jojaVault", true, true);
                Game1.addMailForTomorrow("ccVault", true, true);
                break;
              case 1:
                Game1.addMailForTomorrow("jojaBoilerRoom", true, true);
                Game1.addMailForTomorrow("ccBoilerRoom", true, true);
                break;
              case 2:
                Game1.addMailForTomorrow("jojaCraftsRoom", true, true);
                Game1.addMailForTomorrow("ccCraftsRoom", true, true);
                break;
              case 3:
                Game1.addMailForTomorrow("jojaPantry", true, true);
                Game1.addMailForTomorrow("ccPantry", true, true);
                break;
              case 4:
                Game1.addMailForTomorrow("jojaFishTank", true, true);
                Game1.addMailForTomorrow("ccFishTank", true, true);
                break;
            }
            this.exitTimer = 1000;
          }
          else
            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
        }
      }
    }

    public override bool readyToClose() => true;

    public override void update(GameTime time)
    {
      base.update(time);
      if (this.exitTimer >= 0)
      {
        this.exitTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.exitTimer <= 0)
          this.exitThisMenu();
      }
      Game1.mouseCursorTransparency = 1f;
    }

    public int getPriceFromButtonNumber(int buttonNumber)
    {
      switch (buttonNumber)
      {
        case 0:
          return 40000;
        case 1:
          return 15000;
        case 2:
          return 25000;
        case 3:
          return 35000;
        case 4:
          return 20000;
        default:
          return -1;
      }
    }

    public string getDescriptionFromButtonNumber(int buttonNumber) => Game1.content.LoadString("Strings\\UI:JojaCDMenu_Hover" + buttonNumber.ToString());

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      this.hoverText = "";
      foreach (ClickableComponent checkbox in this.checkboxes)
      {
        if (checkbox.containsPoint(x, y))
          this.hoverText = checkbox.name.Equals("complete") ? "" : Game1.parseText(this.getDescriptionFromButtonNumber(Convert.ToInt32(checkbox.name)), Game1.dialogueFont, 384);
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      base.gameWindowSizeChanged(oldBounds, newBounds);
      this.xPositionOnScreen = Game1.uiViewport.Width / 2 - 640;
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - 288;
      int x = this.xPositionOnScreen + 4;
      int y = this.yPositionOnScreen + 208;
      this.checkboxes.Clear();
      for (int index = 0; index < 5; ++index)
      {
        this.checkboxes.Add(new ClickableComponent(new Rectangle(x, y, 588, 120), index.ToString() ?? ""));
        x += 592;
        if (x > this.xPositionOnScreen + 1184)
        {
          x = this.xPositionOnScreen + 4;
          y += 120;
        }
      }
    }

    public override void receiveKeyPress(Keys key) => base.receiveKeyPress(key);

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
      b.Draw(this.noteTexture, Utility.getTopLeftPositionForCenteringOnScreen(1280, 576), new Rectangle?(new Rectangle(0, 0, 320, 144)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.79f);
      base.draw(b);
      foreach (ClickableComponent checkbox in this.checkboxes)
      {
        if (checkbox.name.Equals("complete"))
          b.Draw(this.noteTexture, new Vector2((float) (checkbox.bounds.Left + 16), (float) (checkbox.bounds.Y + 16)), new Rectangle?(new Rectangle(0, 144, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
      }
      Game1.dayTimeMoneyBox.drawMoneyBox(b, Game1.uiViewport.Width - 300 - IClickableMenu.spaceToClearSideBorder * 2, 4);
      Game1.mouseCursorTransparency = 1f;
      this.drawMouse(b);
      if (this.hoverText == null || this.hoverText.Equals(""))
        return;
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }
  }
}
