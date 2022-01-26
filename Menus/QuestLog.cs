// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.QuestLog
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class QuestLog : IClickableMenu
  {
    public const int questsPerPage = 6;
    public const int region_forwardButton = 101;
    public const int region_backButton = 102;
    public const int region_rewardBox = 103;
    public const int region_cancelQuestButton = 104;
    private List<List<IQuest>> pages;
    public List<ClickableComponent> questLogButtons;
    private int currentPage;
    private int questPage = -1;
    public ClickableTextureComponent forwardButton;
    public ClickableTextureComponent backButton;
    public ClickableTextureComponent rewardBox;
    public ClickableTextureComponent cancelQuestButton;
    protected IQuest _shownQuest;
    protected List<string> _objectiveText;
    protected float _contentHeight;
    protected float _scissorRectHeight;
    public float scrollAmount;
    public ClickableTextureComponent upArrow;
    public ClickableTextureComponent downArrow;
    public ClickableTextureComponent scrollBar;
    private bool scrolling;
    public Rectangle scrollBarBounds;
    private string hoverText = "";

    public QuestLog()
      : base(0, 0, 0, 0, true)
    {
      Game1.dayTimeMoneyBox.DismissQuestPing();
      Game1.playSound("bigSelect");
      this.paginateQuests();
      this.width = 832;
      this.height = 576;
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
        this.height += 64;
      Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
      this.xPositionOnScreen = (int) centeringOnScreen.X;
      this.yPositionOnScreen = (int) centeringOnScreen.Y + 32;
      this.questLogButtons = new List<ClickableComponent>();
      for (int index = 0; index < 6; ++index)
        this.questLogButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 16 + index * ((this.height - 32) / 6), this.width - 32, (this.height - 32) / 6 + 4), index.ToString() ?? "")
        {
          myID = index,
          downNeighborID = -7777,
          upNeighborID = index > 0 ? index - 1 : -1,
          rightNeighborID = -7777,
          leftNeighborID = -7777,
          fullyImmutable = true
        });
      this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 20, this.yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - 64, this.yPositionOnScreen + 8, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
      textureComponent1.myID = 102;
      textureComponent1.rightNeighborID = -7777;
      this.backButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 64 - 48, this.yPositionOnScreen + this.height - 48, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
      textureComponent2.myID = 101;
      this.forwardButton = textureComponent2;
      ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 80, this.yPositionOnScreen + this.height - 32 - 96, 96, 96), Game1.mouseCursors, new Rectangle(293, 360, 24, 24), 4f, true);
      textureComponent3.myID = 103;
      this.rewardBox = textureComponent3;
      ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 4, this.yPositionOnScreen + this.height + 4, 48, 48), Game1.mouseCursors, new Rectangle(322, 498, 12, 12), 4f, true);
      textureComponent4.myID = 104;
      this.cancelQuestButton = textureComponent4;
      int x = this.xPositionOnScreen + this.width + 16;
      this.upArrow = new ClickableTextureComponent(new Rectangle(x, this.yPositionOnScreen + 96, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
      this.downArrow = new ClickableTextureComponent(new Rectangle(x, this.yPositionOnScreen + this.height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
      this.scrollBarBounds = new Rectangle();
      this.scrollBarBounds.X = this.upArrow.bounds.X + 12;
      this.scrollBarBounds.Width = 24;
      this.scrollBarBounds.Y = this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4;
      this.scrollBarBounds.Height = this.downArrow.bounds.Y - 4 - this.scrollBarBounds.Y;
      this.scrollBar = new ClickableTextureComponent(new Rectangle(this.scrollBarBounds.X, this.scrollBarBounds.Y, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
      if (!Game1.options.SnappyMenus)
        return;
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
      if (oldID >= 0 && oldID < 6 && this.questPage == -1)
      {
        switch (direction)
        {
          case 1:
            if (this.currentPage < this.pages.Count - 1)
            {
              this.currentlySnappedComponent = this.getComponentWithID(101);
              this.currentlySnappedComponent.leftNeighborID = oldID;
              break;
            }
            break;
          case 2:
            if (oldID < 5 && this.pages[this.currentPage].Count - 1 > oldID)
            {
              this.currentlySnappedComponent = this.getComponentWithID(oldID + 1);
              break;
            }
            break;
          case 3:
            if (this.currentPage > 0)
            {
              this.currentlySnappedComponent = this.getComponentWithID(102);
              this.currentlySnappedComponent.rightNeighborID = oldID;
              break;
            }
            break;
        }
      }
      else if (oldID == 102)
      {
        if (this.questPage != -1)
          return;
        this.currentlySnappedComponent = this.getComponentWithID(0);
      }
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void receiveGamePadButton(Buttons b)
    {
      if (b == Buttons.RightTrigger && this.questPage == -1 && this.currentPage < this.pages.Count - 1)
      {
        this.nonQuestPageForwardButton();
      }
      else
      {
        if (b != Buttons.LeftTrigger || this.questPage != -1 || this.currentPage <= 0)
          return;
        this.nonQuestPageBackButton();
      }
    }

    private void paginateQuests()
    {
      this.pages = new List<List<IQuest>>();
      for (int index = Game1.player.team.specialOrders.Count - 1; index >= 0; --index)
      {
        int num = index;
        while (this.pages.Count <= num / 6)
          this.pages.Add(new List<IQuest>());
        if (!Game1.player.team.specialOrders[index].IsHidden())
          this.pages[num / 6].Add((IQuest) Game1.player.team.specialOrders[index]);
      }
      for (int index = Game1.player.questLog.Count - 1; index >= 0; --index)
      {
        if (Game1.player.questLog[index] == null || (bool) (NetFieldBase<bool, NetBool>) Game1.player.questLog[index].destroy)
          Game1.player.questLog.RemoveAt(index);
        else if (Game1.player.questLog[index] == null || !Game1.player.questLog[index].IsHidden())
        {
          int num = Game1.player.visibleQuestCount - 1 - index;
          while (this.pages.Count <= num / 6)
            this.pages.Add(new List<IQuest>());
          this.pages[num / 6].Add((IQuest) Game1.player.questLog[index]);
        }
      }
      if (this.pages.Count == 0)
        this.pages.Add(new List<IQuest>());
      this.currentPage = Math.Min(Math.Max(this.currentPage, 0), this.pages.Count - 1);
      this.questPage = -1;
    }

    public bool NeedsScroll() => (this._shownQuest == null || !this._shownQuest.ShouldDisplayAsComplete()) && this.questPage != -1 && (double) this._contentHeight > (double) this._scissorRectHeight;

    public override void receiveScrollWheelAction(int direction)
    {
      if (this.NeedsScroll())
      {
        float num = this.scrollAmount - (float) (Math.Sign(direction) * 64 / 2);
        if ((double) num < 0.0)
          num = 0.0f;
        if ((double) num > (double) this._contentHeight - (double) this._scissorRectHeight)
          num = this._contentHeight - this._scissorRectHeight;
        if ((double) this.scrollAmount != (double) num)
        {
          this.scrollAmount = num;
          Game1.playSound("shiny4");
          this.SetScrollBarFromAmount();
        }
      }
      base.receiveScrollWheelAction(direction);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public override void performHoverAction(int x, int y)
    {
      this.hoverText = "";
      base.performHoverAction(x, y);
      if (this.questPage == -1)
      {
        for (int index = 0; index < this.questLogButtons.Count; ++index)
        {
          if (this.pages.Count > 0 && this.pages[0].Count > index && this.questLogButtons[index].containsPoint(x, y) && !this.questLogButtons[index].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            Game1.playSound("Cowboy_gunshot");
        }
      }
      else if (this._shownQuest.CanBeCancelled() && this.cancelQuestButton.containsPoint(x, y))
        this.hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11364");
      this.forwardButton.tryHover(x, y, 0.2f);
      this.backButton.tryHover(x, y, 0.2f);
      this.cancelQuestButton.tryHover(x, y, 0.2f);
      if (!this.NeedsScroll())
        return;
      this.upArrow.tryHover(x, y);
      this.downArrow.tryHover(x, y);
      this.scrollBar.tryHover(x, y);
      int num = this.scrolling ? 1 : 0;
    }

    public override void receiveKeyPress(Keys key)
    {
      if (Game1.isAnyGamePadButtonBeingPressed() && this.questPage != -1 && Game1.options.doesInputListContain(Game1.options.menuButton, key))
        this.exitQuestPage();
      else
        base.receiveKeyPress(key);
      if (!Game1.options.doesInputListContain(Game1.options.journalButton, key) || !this.readyToClose())
        return;
      Game1.exitActiveMenu();
      Game1.playSound("bigDeSelect");
    }

    private void nonQuestPageForwardButton()
    {
      ++this.currentPage;
      Game1.playSound("shwip");
      if (!Game1.options.SnappyMenus || this.currentPage != this.pages.Count - 1)
        return;
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    private void nonQuestPageBackButton()
    {
      --this.currentPage;
      Game1.playSound("shwip");
      if (!Game1.options.SnappyMenus || this.currentPage != 0)
        return;
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override void leftClickHeld(int x, int y)
    {
      if (GameMenu.forcePreventClose)
        return;
      base.leftClickHeld(x, y);
      if (!this.scrolling)
        return;
      this.SetScrollFromY(y);
    }

    public override void releaseLeftClick(int x, int y)
    {
      if (GameMenu.forcePreventClose)
        return;
      base.releaseLeftClick(x, y);
      this.scrolling = false;
    }

    public virtual void SetScrollFromY(int y)
    {
      int y1 = this.scrollBar.bounds.Y;
      this.scrollAmount = Utility.Clamp((float) (y - this.scrollBarBounds.Y) / (float) (this.scrollBarBounds.Height - this.scrollBar.bounds.Height), 0.0f, 1f) * (this._contentHeight - this._scissorRectHeight);
      this.SetScrollBarFromAmount();
      int y2 = this.scrollBar.bounds.Y;
      if (y1 == y2)
        return;
      Game1.playSound("shiny4");
    }

    public void UpArrowPressed()
    {
      this.upArrow.scale = this.upArrow.baseScale;
      this.scrollAmount -= 64f;
      if ((double) this.scrollAmount < 0.0)
        this.scrollAmount = 0.0f;
      this.SetScrollBarFromAmount();
    }

    public void DownArrowPressed()
    {
      this.downArrow.scale = this.downArrow.baseScale;
      this.scrollAmount += 64f;
      if ((double) this.scrollAmount > (double) this._contentHeight - (double) this._scissorRectHeight)
        this.scrollAmount = this._contentHeight - this._scissorRectHeight;
      this.SetScrollBarFromAmount();
    }

    private void SetScrollBarFromAmount()
    {
      if (!this.NeedsScroll())
      {
        this.scrollAmount = 0.0f;
      }
      else
      {
        if ((double) this.scrollAmount < 8.0)
          this.scrollAmount = 0.0f;
        if ((double) this.scrollAmount > (double) this._contentHeight - (double) this._scissorRectHeight - 8.0)
          this.scrollAmount = this._contentHeight - this._scissorRectHeight;
        this.scrollBar.bounds.Y = (int) ((double) this.scrollBarBounds.Y + (double) (this.scrollBarBounds.Height - this.scrollBar.bounds.Height) / (double) Math.Max(1f, this._contentHeight - this._scissorRectHeight) * (double) this.scrollAmount);
      }
    }

    public override void applyMovementKey(int direction)
    {
      base.applyMovementKey(direction);
      if (!this.NeedsScroll())
        return;
      if (direction == 0)
      {
        this.UpArrowPressed();
      }
      else
      {
        if (direction != 2)
          return;
        this.DownArrowPressed();
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);
      if (Game1.activeClickableMenu == null)
        return;
      if (this.questPage == -1)
      {
        for (int index = 0; index < this.questLogButtons.Count; ++index)
        {
          if (this.pages.Count > 0 && this.pages[this.currentPage].Count > index && this.questLogButtons[index].containsPoint(x, y))
          {
            Game1.playSound("smallSelect");
            this.questPage = index;
            this._shownQuest = this.pages[this.currentPage][index];
            this._objectiveText = this._shownQuest.GetObjectiveDescriptions();
            this._shownQuest.MarkAsViewed();
            this.scrollAmount = 0.0f;
            this.SetScrollBarFromAmount();
            if (!Game1.options.SnappyMenus)
              return;
            this.currentlySnappedComponent = this.getComponentWithID(102);
            this.currentlySnappedComponent.rightNeighborID = -7777;
            this.currentlySnappedComponent.downNeighborID = this.HasMoneyReward() ? 103 : (this._shownQuest.CanBeCancelled() ? 104 : -1);
            this.snapCursorToCurrentSnappedComponent();
            return;
          }
        }
        if (this.currentPage < this.pages.Count - 1 && this.forwardButton.containsPoint(x, y))
          this.nonQuestPageForwardButton();
        else if (this.currentPage > 0 && this.backButton.containsPoint(x, y))
        {
          this.nonQuestPageBackButton();
        }
        else
        {
          Game1.playSound("bigDeSelect");
          this.exitThisMenu();
        }
      }
      else
      {
        Quest shownQuest = this._shownQuest as Quest;
        if (this.questPage != -1 && this._shownQuest.ShouldDisplayAsComplete() && this._shownQuest.HasMoneyReward() && this.rewardBox.containsPoint(x, y))
        {
          Game1.player.Money += this._shownQuest.GetMoneyReward();
          Game1.playSound("purchaseRepeat");
          this._shownQuest.OnMoneyRewardClaimed();
        }
        else if (this.questPage != -1 && shownQuest != null && !(bool) (NetFieldBase<bool, NetBool>) shownQuest.completed && (bool) (NetFieldBase<bool, NetBool>) shownQuest.canBeCancelled && this.cancelQuestButton.containsPoint(x, y))
        {
          shownQuest.accepted.Value = false;
          if (shownQuest.dailyQuest.Value && shownQuest.dayQuestAccepted.Value == Game1.Date.TotalDays)
            Game1.player.acceptedDailyQuest.Set(false);
          Game1.player.questLog.Remove(shownQuest);
          this.pages[this.currentPage].RemoveAt(this.questPage);
          this.questPage = -1;
          Game1.playSound("trashcan");
          if (Game1.options.SnappyMenus && this.currentPage == 0)
          {
            this.currentlySnappedComponent = this.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
          }
        }
        else if (!this.NeedsScroll() || this.backButton.containsPoint(x, y))
          this.exitQuestPage();
        if (!this.NeedsScroll())
          return;
        if (this.downArrow.containsPoint(x, y) && (double) this.scrollAmount < (double) this._contentHeight - (double) this._scissorRectHeight)
        {
          this.DownArrowPressed();
          Game1.playSound("shwip");
        }
        else if (this.upArrow.containsPoint(x, y) && (double) this.scrollAmount > 0.0)
        {
          this.UpArrowPressed();
          Game1.playSound("shwip");
        }
        else if (this.scrollBar.containsPoint(x, y))
          this.scrolling = true;
        else if (this.scrollBarBounds.Contains(x, y))
        {
          this.scrolling = true;
        }
        else
        {
          if (this.downArrow.containsPoint(x, y) || x <= this.xPositionOnScreen + this.width || x >= this.xPositionOnScreen + this.width + 128 || y <= this.yPositionOnScreen || y >= this.yPositionOnScreen + this.height)
            return;
          this.scrolling = true;
          this.leftClickHeld(x, y);
          this.releaseLeftClick(x, y);
        }
      }
    }

    public bool HasReward() => this._shownQuest.HasReward();

    public bool HasMoneyReward() => this._shownQuest.HasMoneyReward();

    public void exitQuestPage()
    {
      if (this._shownQuest.OnLeaveQuestPage())
        this.pages[this.currentPage].RemoveAt(this.questPage);
      this.questPage = -1;
      this.paginateQuests();
      Game1.playSound("shwip");
      if (!Game1.options.SnappyMenus)
        return;
      this.snapToDefaultClickableComponent();
    }

    public override void update(GameTime time)
    {
      base.update(time);
      if (this.questPage == -1 || !this.HasReward())
        return;
      this.rewardBox.scale = this.rewardBox.baseScale + Game1.dialogueButtonScale / 20f;
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
      SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11373"), this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen - 64);
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White, 4f);
      if (this.questPage == -1)
      {
        for (int index = 0; index < this.questLogButtons.Count; ++index)
        {
          if (this.pages.Count<List<IQuest>>() > 0 && this.pages[this.currentPage].Count<IQuest>() > index)
          {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), this.questLogButtons[index].bounds.X, this.questLogButtons[index].bounds.Y, this.questLogButtons[index].bounds.Width, this.questLogButtons[index].bounds.Height, this.questLogButtons[index].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White, 4f, false);
            if (this.pages[this.currentPage][index].ShouldDisplayAsNew() || this.pages[this.currentPage][index].ShouldDisplayAsComplete())
              Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.questLogButtons[index].bounds.X + 64 + 4), (float) (this.questLogButtons[index].bounds.Y + 44)), new Rectangle(this.pages[this.currentPage][index].ShouldDisplayAsComplete() ? 341 : 317, 410, 23, 9), Color.White, 0.0f, new Vector2(11f, 4f), (float) (4.0 + (double) Game1.dialogueButtonScale * 10.0 / 250.0), layerDepth: 0.99f);
            else
              Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.questLogButtons[index].bounds.X + 32), (float) (this.questLogButtons[index].bounds.Y + 28)), this.pages[this.currentPage][index].IsTimedQuest() ? new Rectangle(410, 501, 9, 9) : new Rectangle(395 + (this.pages[this.currentPage][index].IsTimedQuest() ? 3 : 0), 497, 3, 8), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.99f);
            this.pages[this.currentPage][index].IsTimedQuest();
            SpriteText.drawString(b, this.pages[this.currentPage][index].GetName(), this.questLogButtons[index].bounds.X + 128 + 4, this.questLogButtons[index].bounds.Y + 20);
          }
        }
      }
      else
      {
        SpriteText.drawStringHorizontallyCenteredAt(b, this._shownQuest.GetName(), this.xPositionOnScreen + this.width / 2 + (!this._shownQuest.IsTimedQuest() || this._shownQuest.GetDaysLeft() <= 0 ? 0 : Math.Max(32, SpriteText.getWidthOfString(this._shownQuest.GetName()) / 3) - 32), this.yPositionOnScreen + 32);
        if (this._shownQuest.IsTimedQuest() && this._shownQuest.GetDaysLeft() > 0)
        {
          Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 32), (float) (this.yPositionOnScreen + 48 - 8)), new Rectangle(410, 501, 9, 9), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.99f);
          Utility.drawTextWithShadow(b, Game1.parseText(this.pages[this.currentPage][this.questPage].GetDaysLeft() > 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11374", (object) this.pages[this.currentPage][this.questPage].GetDaysLeft()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest_FinalDay"), Game1.dialogueFont, this.width - 128), Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + 80), (float) (this.yPositionOnScreen + 48 - 8)), Game1.textColor);
        }
        string text1 = Game1.parseText(this._shownQuest.GetDescription(), Game1.dialogueFont, this.width - 128);
        Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
        Vector2 vector2 = Game1.dialogueFont.MeasureString(text1);
        Rectangle scissor_rect = new Rectangle()
        {
          X = this.xPositionOnScreen + 32,
          Y = this.yPositionOnScreen + 96
        };
        scissor_rect.Height = this.yPositionOnScreen + this.height - 32 - scissor_rect.Y;
        scissor_rect.Width = this.width - 64;
        this._scissorRectHeight = (float) scissor_rect.Height;
        Rectangle screen = Utility.ConstrainScissorRectToScreen(scissor_rect);
        b.End();
        SpriteBatch spriteBatch = b;
        BlendState alphaBlend = BlendState.AlphaBlend;
        SamplerState pointClamp = SamplerState.PointClamp;
        RasterizerState rasterizerState = new RasterizerState();
        rasterizerState.ScissorTestEnable = true;
        Matrix? transformMatrix = new Matrix?();
        spriteBatch.Begin(blendState: alphaBlend, samplerState: pointClamp, rasterizerState: rasterizerState, transformMatrix: transformMatrix);
        Game1.graphics.GraphicsDevice.ScissorRectangle = screen;
        Utility.drawTextWithShadow(b, text1, Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + 64), (float) ((double) this.yPositionOnScreen - (double) this.scrollAmount + 96.0)), Game1.textColor);
        float y = (float) ((double) (this.yPositionOnScreen + 96) + (double) vector2.Y + 32.0) - this.scrollAmount;
        if (this._shownQuest.ShouldDisplayAsComplete())
        {
          b.End();
          b.GraphicsDevice.ScissorRectangle = scissorRectangle;
          b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
          SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11376"), this.xPositionOnScreen + 32 + 4, this.rewardBox.bounds.Y + 21 + 4);
          this.rewardBox.draw(b);
          if (this.HasMoneyReward())
          {
            b.Draw(Game1.mouseCursors, new Vector2((float) (this.rewardBox.bounds.X + 16), (float) (this.rewardBox.bounds.Y + 16) - Game1.dialogueButtonScale / 2f), new Rectangle?(new Rectangle(280, 410, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this._shownQuest.GetMoneyReward()), this.xPositionOnScreen + 448, this.rewardBox.bounds.Y + 21 + 4);
          }
        }
        else
        {
          for (int index1 = 0; index1 < this._objectiveText.Count; ++index1)
          {
            if (this._shownQuest != null)
            {
              SpecialOrder shownQuest = this._shownQuest as SpecialOrder;
            }
            string text2 = this._objectiveText[index1];
            int num1 = this.width - 192;
            SpriteFont dialogueFont1 = Game1.dialogueFont;
            int width1 = num1;
            string text3 = Game1.parseText(text2, dialogueFont1, width1);
            bool flag = false;
            if (this._shownQuest != null && this._shownQuest is SpecialOrder)
              flag = (this._shownQuest as SpecialOrder).objectives[index1].IsComplete();
            if (!flag)
              Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 96) + (float) (8.0 * (double) Game1.dialogueButtonScale / 10.0), y), new Rectangle(412, 495, 5, 4), Color.White, 1.570796f, Vector2.Zero);
            Color color1 = Color.DarkBlue;
            if (flag)
              color1 = Game1.unselectedOptionColor;
            Utility.drawTextWithShadow(b, text3, Game1.dialogueFont, new Vector2((float) (this.xPositionOnScreen + 128), y - 8f), color1);
            y += Game1.dialogueFont.MeasureString(text3).Y;
            if (this._shownQuest != null && this._shownQuest is SpecialOrder)
            {
              OrderObjective objective = (this._shownQuest as SpecialOrder).objectives[index1];
              if (objective.GetMaxCount() > 1 && objective.ShouldShowProgress())
              {
                Color color2 = Color.DarkRed;
                Color color3 = Color.Red;
                if (objective.GetCount() >= objective.GetMaxCount())
                {
                  color3 = Color.LimeGreen;
                  color2 = Color.Green;
                }
                int num2 = 64;
                int num3 = 160;
                int num4 = 4;
                Rectangle rectangle1 = new Rectangle(0, 224, 47, 12);
                Rectangle rectangle2 = new Rectangle(47, 224, 1, 12);
                int num5 = 3;
                int num6 = 3;
                int width2 = 5;
                int num7 = objective.GetCount();
                string str1 = num7.ToString();
                num7 = objective.GetMaxCount();
                string str2 = num7.ToString();
                string text4 = str1 + "/" + str2;
                SpriteFont dialogueFont2 = Game1.dialogueFont;
                num7 = objective.GetMaxCount();
                string str3 = num7.ToString();
                num7 = objective.GetMaxCount();
                string str4 = num7.ToString();
                string text5 = str3 + "/" + str4;
                int x1 = (int) dialogueFont2.MeasureString(text5).X;
                int x2 = (int) Game1.dialogueFont.MeasureString(text4).X;
                int x3 = this.xPositionOnScreen + this.width - num2 - x2;
                int num8 = this.xPositionOnScreen + this.width - num2 - x1;
                Utility.drawTextWithShadow(b, text4, Game1.dialogueFont, new Vector2((float) x3, y), Color.DarkBlue);
                Rectangle rectangle3 = new Rectangle(this.xPositionOnScreen + num2, (int) y, this.width - num2 * 2 - num3, rectangle1.Height * 4);
                if (rectangle3.Right > num8 - 16)
                {
                  int num9 = rectangle3.Right - (num8 - 16);
                  rectangle3.Width -= num9;
                }
                b.Draw(Game1.mouseCursors2, new Rectangle(rectangle3.X, rectangle3.Y, width2 * 4, rectangle3.Height), new Rectangle?(new Rectangle(rectangle1.X, rectangle1.Y, width2, rectangle1.Height)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                b.Draw(Game1.mouseCursors2, new Rectangle(rectangle3.X + width2 * 4, rectangle3.Y, rectangle3.Width - 2 * width2 * 4, rectangle3.Height), new Rectangle?(new Rectangle(rectangle1.X + width2, rectangle1.Y, rectangle1.Width - 2 * width2, rectangle1.Height)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                b.Draw(Game1.mouseCursors2, new Rectangle(rectangle3.Right - width2 * 4, rectangle3.Y, width2 * 4, rectangle3.Height), new Rectangle?(new Rectangle(rectangle1.Right - width2, rectangle1.Y, width2, rectangle1.Height)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                float num10 = (float) objective.GetCount() / (float) objective.GetMaxCount();
                if (objective.GetMaxCount() < num4)
                  num4 = objective.GetMaxCount();
                rectangle3.X += 4 * num5;
                rectangle3.Width -= 4 * num5 * 2;
                for (int index2 = 1; index2 < num4; ++index2)
                  b.Draw(Game1.mouseCursors2, new Vector2((float) rectangle3.X + (float) rectangle3.Width * ((float) index2 / (float) num4), (float) rectangle3.Y), new Rectangle?(rectangle2), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
                rectangle3.Y += 4 * num6;
                rectangle3.Height -= 4 * num6 * 2;
                Rectangle destinationRectangle = new Rectangle(rectangle3.X, rectangle3.Y, (int) ((double) rectangle3.Width * (double) num10) - 4, rectangle3.Height);
                b.Draw(Game1.staminaRect, destinationRectangle, new Rectangle?(), color3, 0.0f, Vector2.Zero, SpriteEffects.None, (float) destinationRectangle.Y / 10000f);
                destinationRectangle.X = destinationRectangle.Right;
                destinationRectangle.Width = 4;
                b.Draw(Game1.staminaRect, destinationRectangle, new Rectangle?(), color2, 0.0f, Vector2.Zero, SpriteEffects.None, (float) destinationRectangle.Y / 10000f);
                y += (float) ((rectangle1.Height + 4) * 4);
              }
            }
            this._contentHeight = y + this.scrollAmount - (float) screen.Y;
          }
          b.End();
          b.GraphicsDevice.ScissorRectangle = scissorRectangle;
          b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
          if (this._shownQuest.CanBeCancelled())
            this.cancelQuestButton.draw(b);
          if (this.NeedsScroll())
          {
            if ((double) this.scrollAmount > 0.0)
              b.Draw(Game1.staminaRect, new Rectangle(screen.X, screen.Top, screen.Width, 4), Color.Black * 0.15f);
            if ((double) this.scrollAmount < (double) this._contentHeight - (double) this._scissorRectHeight)
              b.Draw(Game1.staminaRect, new Rectangle(screen.X, screen.Bottom - 4, screen.Width, 4), Color.Black * 0.15f);
          }
        }
      }
      if (this.NeedsScroll())
      {
        this.upArrow.draw(b);
        this.downArrow.draw(b);
        this.scrollBar.draw(b);
      }
      if (this.currentPage < this.pages.Count - 1 && this.questPage == -1)
        this.forwardButton.draw(b);
      if (this.currentPage > 0 || this.questPage != -1)
        this.backButton.draw(b);
      base.draw(b);
      Game1.mouseCursorTransparency = 1f;
      this.drawMouse(b);
      if (this.hoverText.Length <= 0)
        return;
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont);
    }
  }
}
