// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.PondQueryMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class PondQueryMenu : IClickableMenu
  {
    public const int region_okButton = 101;
    public const int region_emptyButton = 103;
    public const int region_noButton = 105;
    public const int region_nettingButton = 106;
    public new static int width = 384;
    public new static int height = 512;
    public const int unresolved_needs_extra_height = 116;
    protected FishPond _pond;
    protected StardewValley.Object _fishItem;
    protected string _statusText = "";
    public ClickableTextureComponent okButton;
    public ClickableTextureComponent emptyButton;
    public ClickableTextureComponent yesButton;
    public ClickableTextureComponent noButton;
    public ClickableTextureComponent changeNettingButton;
    private bool confirmingEmpty;
    protected Rectangle _confirmationBoxRectangle;
    protected string _confirmationText;
    protected float _age;
    private string hoverText = "";

    public PondQueryMenu(FishPond fish_pond)
      : base(Game1.uiViewport.Width / 2 - PondQueryMenu.width / 2, Game1.uiViewport.Height / 2 - PondQueryMenu.height / 2, PondQueryMenu.width, PondQueryMenu.height)
    {
      Game1.player.Halt();
      PondQueryMenu.width = 384;
      PondQueryMenu.height = 512;
      this._pond = fish_pond;
      this._fishItem = new StardewValley.Object(this._pond.fishType.Value, 1);
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + PondQueryMenu.width + 4, this.yPositionOnScreen + PondQueryMenu.height - 64 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent1.myID = 101;
      textureComponent1.upNeighborID = -99998;
      this.okButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + PondQueryMenu.width + 4, this.yPositionOnScreen + PondQueryMenu.height - 256 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, new Rectangle(32, 384, 16, 16), 4f);
      textureComponent2.myID = 103;
      textureComponent2.downNeighborID = -99998;
      this.emptyButton = textureComponent2;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + PondQueryMenu.width + 4, this.yPositionOnScreen + PondQueryMenu.height - 192 - IClickableMenu.borderWidth, 64, 64), Game1.mouseCursors, new Rectangle(48, 384, 16, 16), 4f);
      textureComponent3.myID = 106;
      textureComponent3.downNeighborID = -99998;
      textureComponent3.upNeighborID = -99998;
      this.changeNettingButton = textureComponent3;
      if (Game1.options.SnappyMenus)
      {
        this.populateClickableComponentList();
        this.snapToDefaultClickableComponent();
      }
      this.UpdateState();
      this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.measureTotalHeight() / 2;
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(101);
      this.snapCursorToCurrentSnappedComponent();
    }

    public void textBoxEnter(TextBox sender)
    {
    }

    public override void receiveKeyPress(Keys key)
    {
      if (Game1.globalFade)
        return;
      if (((IEnumerable<InputButton>) Game1.options.menuButton).Contains<InputButton>(new InputButton(key)))
      {
        Game1.playSound("smallSelect");
        if (!this.readyToClose())
          return;
        Game1.exitActiveMenu();
      }
      else
      {
        if (!Game1.options.SnappyMenus || ((IEnumerable<InputButton>) Game1.options.menuButton).Contains<InputButton>(new InputButton(key)))
          return;
        base.receiveKeyPress(key);
      }
    }

    public override void update(GameTime time)
    {
      base.update(time);
      this._age += (float) time.ElapsedGameTime.TotalSeconds;
    }

    public void finishedPlacingAnimal()
    {
      Game1.exitActiveMenu();
      Game1.currentLocation = Game1.player.currentLocation;
      Game1.currentLocation.resetForPlayerEntry();
      Game1.globalFadeToClear();
      Game1.displayHUD = true;
      Game1.viewportFreeze = false;
      Game1.displayFarmer = true;
      Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_HomeChanged"), Color.LimeGreen, 3500f));
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (Game1.globalFade)
        return;
      if (this.confirmingEmpty)
      {
        if (this.yesButton.containsPoint(x, y))
        {
          Game1.playSound("fishSlap");
          this._pond.ClearPond();
          this.exitThisMenu();
        }
        else
        {
          if (!this.noButton.containsPoint(x, y))
            return;
          this.confirmingEmpty = false;
          Game1.playSound("smallSelect");
          if (!Game1.options.SnappyMenus)
            return;
          this.currentlySnappedComponent = this.getComponentWithID(103);
          this.snapCursorToCurrentSnappedComponent();
        }
      }
      else
      {
        if (this.okButton != null && this.okButton.containsPoint(x, y) && this.readyToClose())
        {
          Game1.exitActiveMenu();
          Game1.playSound("smallSelect");
        }
        if (this.changeNettingButton.containsPoint(x, y))
        {
          Game1.playSound("drumkit6");
          ++this._pond.nettingStyle.Value;
          this._pond.nettingStyle.Value %= 4;
        }
        else
        {
          if (!this.emptyButton.containsPoint(x, y))
            return;
          this._confirmationBoxRectangle = new Rectangle(0, 0, 400, 100);
          this._confirmationBoxRectangle.X = Game1.uiViewport.Width / 2 - this._confirmationBoxRectangle.Width / 2;
          this._confirmationText = Game1.content.LoadString("Strings\\UI:PondQuery_ConfirmEmpty");
          this._confirmationText = Game1.parseText(this._confirmationText, Game1.smallFont, this._confirmationBoxRectangle.Width);
          this._confirmationBoxRectangle.Height = (int) Game1.smallFont.MeasureString(this._confirmationText).Y;
          this._confirmationBoxRectangle.Y = Game1.uiViewport.Height / 2 - this._confirmationBoxRectangle.Height / 2;
          this.confirmingEmpty = true;
          ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(Game1.uiViewport.Width / 2 - 64 - 4, this._confirmationBoxRectangle.Bottom + 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
          textureComponent1.myID = 111;
          textureComponent1.rightNeighborID = 105;
          this.yesButton = textureComponent1;
          ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(Game1.uiViewport.Width / 2 + 4, this._confirmationBoxRectangle.Bottom + 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
          textureComponent2.myID = 105;
          textureComponent2.leftNeighborID = 111;
          this.noButton = textureComponent2;
          Game1.playSound("smallSelect");
          if (!Game1.options.SnappyMenus)
            return;
          this.populateClickableComponentList();
          this.currentlySnappedComponent = (ClickableComponent) this.noButton;
          this.snapCursorToCurrentSnappedComponent();
        }
      }
    }

    public override bool readyToClose() => base.readyToClose() && !Game1.globalFade;

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
      if (Game1.globalFade || !this.readyToClose())
        return;
      Game1.exitActiveMenu();
      Game1.playSound("smallSelect");
    }

    public override void performHoverAction(int x, int y)
    {
      this.hoverText = "";
      if (this.okButton != null)
      {
        if (this.okButton.containsPoint(x, y))
          this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
        else
          this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
      }
      if (this.emptyButton != null)
      {
        if (this.emptyButton.containsPoint(x, y))
        {
          this.emptyButton.scale = Math.Min(4.1f, this.emptyButton.scale + 0.05f);
          this.hoverText = Game1.content.LoadString("Strings\\UI:PondQuery_EmptyPond", (object) 10);
        }
        else
          this.emptyButton.scale = Math.Max(4f, this.emptyButton.scale - 0.05f);
      }
      if (this.changeNettingButton != null)
      {
        if (this.changeNettingButton.containsPoint(x, y))
        {
          this.changeNettingButton.scale = Math.Min(4.1f, this.changeNettingButton.scale + 0.05f);
          this.hoverText = Game1.content.LoadString("Strings\\UI:PondQuery_ChangeNetting", (object) 10);
        }
        else
          this.changeNettingButton.scale = Math.Max(4f, this.emptyButton.scale - 0.05f);
      }
      if (this.yesButton != null)
      {
        if (this.yesButton.containsPoint(x, y))
          this.yesButton.scale = Math.Min(1.1f, this.yesButton.scale + 0.05f);
        else
          this.yesButton.scale = Math.Max(1f, this.yesButton.scale - 0.05f);
      }
      if (this.noButton == null)
        return;
      if (this.noButton.containsPoint(x, y))
        this.noButton.scale = Math.Min(1.1f, this.noButton.scale + 0.05f);
      else
        this.noButton.scale = Math.Max(1f, this.noButton.scale - 0.05f);
    }

    public static string GetFishTalkSuffix(StardewValley.Object fishItem)
    {
      List<string> contextTagList = fishItem.GetContextTagList();
      for (int index1 = 0; index1 < contextTagList.Count; ++index1)
      {
        string str = contextTagList[index1];
        if (str.StartsWith("fish_talk_"))
        {
          if (str == "fish_talk_rude")
            return "_Rude";
          if (str == "fish_talk_stiff")
            return "_Stiff";
          if (str == "fish_talk_demanding")
            return "_Demanding";
          char[] charArray = ("_" + str.Substring("fish_talk_".Length)).ToCharArray();
          bool flag = false;
          for (int index2 = 0; index2 < charArray.Length; ++index2)
          {
            if (charArray[index2] == '_')
              flag = true;
            else if (flag)
            {
              charArray[index2] = char.ToUpper(charArray[index2]);
              flag = false;
            }
          }
          return new string(charArray);
        }
      }
      return contextTagList.Contains("fish_carnivorous") ? "_Carnivore" : "";
    }

    public static string getCompletedRequestString(FishPond pond, StardewValley.Object fishItem, Random r)
    {
      if (fishItem != null)
      {
        string fishTalkSuffix = PondQueryMenu.GetFishTalkSuffix(fishItem);
        if (fishTalkSuffix != "")
          return Lexicon.capitalize(Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequestComplete" + fishTalkSuffix + r.Next(3).ToString(), (object) pond.neededItem.Value.DisplayName));
      }
      return Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequestComplete" + r.Next(7).ToString(), (object) pond.neededItem.Value.DisplayName);
    }

    public void UpdateState()
    {
      Random r = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 + (int) (NetFieldBase<int, NetInt>) this._pond.seedOffset);
      if (this._pond.currentOccupants.Value <= 0)
      {
        this._statusText = Game1.content.LoadString("Strings\\UI:PondQuery_StatusNoFish");
      }
      else
      {
        if (this._pond.neededItem.Value != null)
        {
          if ((bool) (NetFieldBase<bool, NetBool>) this._pond.hasCompletedRequest)
          {
            this._statusText = PondQueryMenu.getCompletedRequestString(this._pond, this._fishItem, r);
            return;
          }
          if (this._pond.HasUnresolvedNeeds())
          {
            string sub2 = this._pond.neededItemCount.Value.ToString() ?? "";
            if (this._pond.neededItemCount.Value <= 1)
            {
              sub2 = Lexicon.getProperArticleForWord(this._pond.neededItem.Value.DisplayName);
              if (sub2 == "")
                sub2 = Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequestOneCount");
            }
            if (this._fishItem != null)
            {
              if (this._fishItem.GetContextTagList().Contains("fish_talk_rude"))
              {
                this._statusText = Lexicon.capitalize(Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequestPending_Rude" + r.Next(3).ToString() + "_" + ((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? "Male" : "Female"), (object) Lexicon.makePlural(this._pond.neededItem.Value.DisplayName, this._pond.neededItemCount.Value == 1), (object) sub2, (object) this._pond.neededItem.Value.DisplayName));
                return;
              }
              string fishTalkSuffix = PondQueryMenu.GetFishTalkSuffix(this._fishItem);
              if (fishTalkSuffix != "")
              {
                this._statusText = Lexicon.capitalize(Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequestPending" + fishTalkSuffix + r.Next(3).ToString(), (object) Lexicon.makePlural(this._pond.neededItem.Value.DisplayName, this._pond.neededItemCount.Value == 1), (object) sub2, (object) this._pond.neededItem.Value.DisplayName));
                return;
              }
            }
            this._statusText = Lexicon.capitalize(Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequestPending" + r.Next(7).ToString(), (object) Lexicon.makePlural(this._pond.neededItem.Value.DisplayName, this._pond.neededItemCount.Value == 1), (object) sub2, (object) this._pond.neededItem.Value.DisplayName));
            return;
          }
        }
        if (this._fishItem != null && ((int) (NetFieldBase<int, NetInt>) this._fishItem.parentSheetIndex == 397 || (int) (NetFieldBase<int, NetInt>) this._fishItem.parentSheetIndex == 393))
          this._statusText = Game1.content.LoadString("Strings\\UI:PondQuery_StatusOk_Coral", (object) this._fishItem.DisplayName);
        else
          this._statusText = Game1.content.LoadString("Strings\\UI:PondQuery_StatusOk" + r.Next(7).ToString());
      }
    }

    private int measureTotalHeight() => 644 + this.measureExtraTextHeight(this.getDisplayedText());

    private int measureExtraTextHeight(string displayed_text) => Math.Max(0, (int) Game1.smallFont.MeasureString(displayed_text).Y - 90) + 4;

    private string getDisplayedText() => Game1.parseText(this._statusText, Game1.smallFont, PondQueryMenu.width - IClickableMenu.spaceToClearSideBorder * 2 - 64);

    public override void draw(SpriteBatch b)
    {
      if (!Game1.globalFade)
      {
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
        bool flag = this._pond.neededItem.Value != null && this._pond.HasUnresolvedNeeds() && !(bool) (NetFieldBase<bool, NetBool>) this._pond.hasCompletedRequest;
        string text1 = Game1.content.LoadString("Strings\\UI:PondQuery_Name", (object) this._fishItem.DisplayName);
        Vector2 vector2_1 = Game1.smallFont.MeasureString(text1);
        Game1.DrawBox((int) ((double) (Game1.uiViewport.Width / 2) - ((double) vector2_1.X + 64.0) * 0.5), this.yPositionOnScreen - 4 + 128, (int) ((double) vector2_1.X + 64.0), 64);
        Utility.drawTextWithShadow(b, text1, Game1.smallFont, new Vector2((float) (Game1.uiViewport.Width / 2) - vector2_1.X * 0.5f, (float) ((double) (this.yPositionOnScreen - 4) + 160.0 - (double) vector2_1.Y * 0.5)), Color.Black);
        string displayedText = this.getDisplayedText();
        int num1 = 0;
        if (flag)
          num1 += 116;
        int num2 = this.measureExtraTextHeight(displayedText);
        Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen + 128, PondQueryMenu.width, PondQueryMenu.height - 128 + num1 + num2, false, true);
        string text2 = Game1.content.LoadString("Strings\\UI:PondQuery_Population", (object) (this._pond.FishCount.ToString() ?? ""), (object) this._pond.maxOccupants);
        Vector2 vector2_2 = Game1.smallFont.MeasureString(text2);
        Utility.drawTextWithShadow(b, text2, Game1.smallFont, new Vector2((float) (this.xPositionOnScreen + PondQueryMenu.width / 2) - vector2_2.X * 0.5f, (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 + 128)), Game1.textColor);
        int maxOccupants = (int) (NetFieldBase<int, NetInt>) this._pond.maxOccupants;
        float num3 = 13f;
        int num4 = 0;
        int num5 = 0;
        for (int index = 0; index < maxOccupants; ++index)
        {
          float num6 = (float) Math.Sin((double) this._age * 1.0 + (double) num4 * 0.75 + (double) num5 * 0.25) * 2f;
          if (index < this._pond.FishCount)
            this._fishItem.drawInMenu(b, new Vector2((float) ((double) (this.xPositionOnScreen + PondQueryMenu.width / 2) - (double) num3 * (double) Math.Min(maxOccupants, 5) * 4.0 * 0.5 + (double) num3 * 4.0 * (double) num4 - 12.0), (float) ((double) (this.yPositionOnScreen + (int) ((double) num6 * 4.0)) + (double) (num5 * 4) * (double) num3 + 275.200012207031)), 0.75f, 1f, 0.0f, StackDrawType.Hide, Color.White, false);
          else
            this._fishItem.drawInMenu(b, new Vector2((float) ((double) (this.xPositionOnScreen + PondQueryMenu.width / 2) - (double) num3 * (double) Math.Min(maxOccupants, 5) * 4.0 * 0.5 + (double) num3 * 4.0 * (double) num4 - 12.0), (float) ((double) (this.yPositionOnScreen + (int) ((double) num6 * 4.0)) + (double) (num5 * 4) * (double) num3 + 275.200012207031)), 0.75f, 0.35f, 0.0f, StackDrawType.Hide, Color.Black, false);
          ++num4;
          if (num4 == 5)
          {
            num4 = 0;
            ++num5;
          }
        }
        Vector2 vector2_3 = Game1.smallFont.MeasureString(displayedText);
        Utility.drawTextWithShadow(b, displayedText, Game1.smallFont, new Vector2((float) (this.xPositionOnScreen + PondQueryMenu.width / 2) - vector2_3.X * 0.5f, (float) (this.yPositionOnScreen + PondQueryMenu.height + num2 - (flag ? 32 : 48)) - vector2_3.Y), Game1.textColor);
        if (flag)
        {
          this.drawHorizontalPartition(b, (int) ((double) (this.yPositionOnScreen + PondQueryMenu.height + num2) - 48.0));
          Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 60) + (float) (8.0 * (double) Game1.dialogueButtonScale / 10.0), (float) (this.yPositionOnScreen + PondQueryMenu.height + num2 + 28)), new Rectangle(412, 495, 5, 4), Color.White, 1.570796f, Vector2.Zero);
          string text3 = Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequest_Bring");
          Vector2 vector2_4 = Game1.smallFont.MeasureString(text3);
          int num7 = this.xPositionOnScreen + 88;
          float x1 = (float) num7;
          float x2 = (float) ((double) x1 + (double) vector2_4.X + 4.0);
          if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr)
          {
            x2 = (float) (num7 - 8);
            x1 = (float) (num7 + 76);
          }
          Utility.drawTextWithShadow(b, text3, Game1.smallFont, new Vector2(x1, (float) (this.yPositionOnScreen + PondQueryMenu.height + num2 + 24)), Game1.textColor);
          b.Draw(Game1.objectSpriteSheet, new Vector2(x2, (float) (this.yPositionOnScreen + PondQueryMenu.height + num2 + 4)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this._pond.neededItem.Value.parentSheetIndex, 16, 16)), Color.Black * 0.4f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          b.Draw(Game1.objectSpriteSheet, new Vector2(x2 + 4f, (float) (this.yPositionOnScreen + PondQueryMenu.height + num2)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this._pond.neededItem.Value.parentSheetIndex, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          if ((int) (NetFieldBase<int, NetIntDelta>) this._pond.neededItemCount > 1)
            Utility.drawTinyDigits((int) (NetFieldBase<int, NetIntDelta>) this._pond.neededItemCount, b, new Vector2(x2 + 48f, (float) (this.yPositionOnScreen + PondQueryMenu.height + num2 + 48)), 3f, 1f, Color.White);
        }
        this.okButton.draw(b);
        this.emptyButton.draw(b);
        this.changeNettingButton.draw(b);
        if (this.confirmingEmpty)
        {
          b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
          int num8 = 16;
          this._confirmationBoxRectangle.Width += num8;
          this._confirmationBoxRectangle.Height += num8;
          this._confirmationBoxRectangle.X -= num8 / 2;
          this._confirmationBoxRectangle.Y -= num8 / 2;
          Game1.DrawBox(this._confirmationBoxRectangle.X, this._confirmationBoxRectangle.Y, this._confirmationBoxRectangle.Width, this._confirmationBoxRectangle.Height);
          this._confirmationBoxRectangle.Width -= num8;
          this._confirmationBoxRectangle.Height -= num8;
          this._confirmationBoxRectangle.X += num8 / 2;
          this._confirmationBoxRectangle.Y += num8 / 2;
          b.DrawString(Game1.smallFont, this._confirmationText, new Vector2((float) this._confirmationBoxRectangle.X, (float) this._confirmationBoxRectangle.Y), Game1.textColor);
          this.yesButton.draw(b);
          this.noButton.draw(b);
        }
        else if (this.hoverText != null && this.hoverText.Length > 0)
          IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      }
      this.drawMouse(b);
    }
  }
}
