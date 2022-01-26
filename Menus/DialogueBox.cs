// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.DialogueBox
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class DialogueBox : IClickableMenu
  {
    public List<string> dialogues = new List<string>();
    public Dialogue characterDialogue;
    public Stack<string> characterDialoguesBrokenUp = new Stack<string>();
    public List<Response> responses = new List<Response>();
    public const int portraitBoxSize = 74;
    public const int nameTagWidth = 102;
    public const int nameTagHeight = 18;
    public const int portraitPlateWidth = 115;
    public const int nameTagSideMargin = 5;
    public const float transitionRate = 3f;
    public const int characterAdvanceDelay = 30;
    public const int safetyDelay = 750;
    public int questionFinishPauseTimer;
    protected bool _showedOptions;
    public Rectangle friendshipJewel = Rectangle.Empty;
    public List<ClickableComponent> responseCC;
    public int x;
    public int y;
    public int transitionX = -1;
    public int transitionY;
    public int transitionWidth;
    public int transitionHeight;
    public int characterAdvanceTimer;
    public int characterIndexInDialogue;
    public int safetyTimer = 750;
    public int heightForQuestions;
    public int selectedResponse = -1;
    public int newPortaitShakeTimer;
    public bool transitionInitialized;
    public bool transitioning = true;
    public bool transitioningBigger = true;
    public bool dialogueContinuedOnNextPage;
    public bool dialogueFinished;
    public bool isQuestion;
    public TemporaryAnimatedSprite dialogueIcon;
    public TemporaryAnimatedSprite aboveDialogueImage;
    private string hoverText = "";

    public DialogueBox(int x, int y, int width, int height)
    {
      if (Game1.options.SnappyMenus)
        Game1.mouseCursorTransparency = 0.0f;
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
    }

    public DialogueBox(string dialogue)
    {
      if (Game1.options.SnappyMenus)
        Game1.mouseCursorTransparency = 0.0f;
      this.dialogues.AddRange((IEnumerable<string>) dialogue.Split('#'));
      this.width = Math.Min(1240, SpriteText.getWidthOfString(this.dialogues[0]) + 64);
      this.height = SpriteText.getHeightOfString(this.dialogues[0], this.width - 20) + 4;
      this.x = (int) Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height).X;
      this.y = Game1.uiViewport.Height - this.height - 64;
      this.setUpIcons();
    }

    public DialogueBox(string dialogue, List<Response> responses, int width = 1200)
    {
      if (Game1.options.SnappyMenus)
        Game1.mouseCursorTransparency = 0.0f;
      this.dialogues.Add(dialogue);
      this.responses = responses;
      this.isQuestion = true;
      this.width = width;
      this.setUpQuestions();
      this.height = this.heightForQuestions;
      this.x = (int) Utility.getTopLeftPositionForCenteringOnScreen(width, this.height).X;
      this.y = Game1.uiViewport.Height - this.height - 64;
      this.setUpIcons();
      this.characterIndexInDialogue = dialogue.Length;
      if (responses == null)
        return;
      foreach (Response response in responses)
      {
        if (response.responseText.Contains("¦"))
          response.responseText = !Game1.player.IsMale ? response.responseText.Substring(response.responseText.IndexOf("¦") + 1) : response.responseText.Substring(0, response.responseText.IndexOf("¦"));
      }
    }

    public DialogueBox(Dialogue dialogue)
    {
      if (Game1.options.SnappyMenus)
        Game1.mouseCursorTransparency = 0.0f;
      this.characterDialogue = dialogue;
      this.width = 1200;
      this.height = 384;
      this.x = (int) Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height).X;
      this.y = Game1.uiViewport.Height - this.height - 64;
      this.friendshipJewel = new Rectangle(this.x + this.width - 64, this.y + 256, 44, 44);
      dialogue.prepareDialogueForDisplay();
      this.characterDialoguesBrokenUp.Push(dialogue.getCurrentDialogue());
      this.checkDialogue(dialogue);
      this.newPortaitShakeTimer = this.characterDialogue.getPortraitIndex() == 1 ? 250 : 0;
      this.setUpForGamePadMode();
    }

    public DialogueBox(List<string> dialogues)
    {
      if (Game1.options.SnappyMenus)
        Game1.mouseCursorTransparency = 0.0f;
      this.dialogues = dialogues;
      this.width = Math.Min(1200, SpriteText.getWidthOfString(dialogues[0]) + 64);
      this.height = SpriteText.getHeightOfString(dialogues[0], this.width - 16);
      this.x = (int) Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height).X;
      this.y = Game1.uiViewport.Height - this.height - 64;
      this.setUpIcons();
    }

    public override void snapToDefaultClickableComponent()
    {
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }

    public override bool autoCenterMouseCursorForGamepad() => false;

    private void playOpeningSound() => Game1.playSound("breathin");

    public override void setUpForGamePadMode()
    {
    }

    public void closeDialogue()
    {
      if (Game1.activeClickableMenu.Equals((object) this))
      {
        Game1.exitActiveMenu();
        Game1.dialogueUp = false;
        if (this.characterDialogue != null && this.characterDialogue.speaker != null && this.characterDialogue.speaker.CurrentDialogue.Count > 0 && this.dialogueFinished && this.characterDialogue.speaker.CurrentDialogue.Count > 0)
          this.characterDialogue.speaker.CurrentDialogue.Pop();
        if (Game1.messagePause)
          Game1.pauseTime = 500f;
        if (Game1.currentObjectDialogue.Count > 0)
          Game1.currentObjectDialogue.Dequeue();
        Game1.currentDialogueCharacterIndex = 0;
        if (Game1.currentObjectDialogue.Count > 0)
        {
          Game1.dialogueUp = true;
          Game1.questionChoices.Clear();
          Game1.dialogueTyping = true;
        }
        Game1.tvStation = -1;
        if (this.characterDialogue != null && this.characterDialogue.speaker != null && !this.characterDialogue.speaker.Name.Equals("Gunther") && !Game1.eventUp && !(bool) (NetFieldBase<bool, NetBool>) this.characterDialogue.speaker.doingEndOfRouteAnimation)
          this.characterDialogue.speaker.doneFacingPlayer(Game1.player);
        Game1.currentSpeaker = (NPC) null;
        if (!Game1.eventUp)
        {
          if (!Game1.isWarping)
            Game1.player.CanMove = true;
          Game1.player.movementDirections.Clear();
        }
        else if (Game1.currentLocation.currentEvent.CurrentCommand > 0 || Game1.currentLocation.currentEvent.specialEventVariable1)
        {
          if (!Game1.isFestival() || !Game1.currentLocation.currentEvent.canMoveAfterDialogue())
            ++Game1.currentLocation.currentEvent.CurrentCommand;
          else
            Game1.player.CanMove = true;
        }
        Game1.questionChoices.Clear();
      }
      if (Game1.afterDialogues == null)
        return;
      Game1.afterFadeFunction afterDialogues = Game1.afterDialogues;
      Game1.afterDialogues = (Game1.afterFadeFunction) null;
      afterDialogues();
    }

    public void finishTyping() => this.characterIndexInDialogue = this.getCurrentString().Length;

    public void beginOutro()
    {
      this.transitioning = true;
      this.transitioningBigger = false;
      Game1.playSound("breathout");
    }

    public override void receiveRightClick(int x, int y, bool playSound = true) => this.receiveLeftClick(x, y, playSound);

    private void tryOutro()
    {
      if (Game1.activeClickableMenu == null || !Game1.activeClickableMenu.Equals((object) this))
        return;
      this.beginOutro();
    }

    public override void receiveKeyPress(Keys key)
    {
      if (this.transitioning)
        return;
      if (Game1.options.SnappyMenus && !this.isQuestion && Game1.options.doesInputListContain(Game1.options.menuButton, key))
        this.receiveLeftClick(0, 0, true);
      else if (!Game1.options.gamepadControls && Game1.options.doesInputListContain(Game1.options.actionButton, key))
        this.receiveLeftClick(0, 0, true);
      else if (this.isQuestion && !Game1.eventUp && this.characterDialogue == null)
      {
        if (this.responses != null)
        {
          foreach (Response response in this.responses)
          {
            if (response.hotkey == key && Game1.currentLocation.answerDialogue(response))
            {
              Game1.playSound("smallSelect");
              this.selectedResponse = -1;
              this.tryOutro();
              return;
            }
          }
          if (key == Keys.N)
          {
            foreach (Response response in this.responses)
            {
              if (response.hotkey == Keys.Escape && Game1.currentLocation.answerDialogue(response))
              {
                Game1.playSound("smallSelect");
                this.selectedResponse = -1;
                this.tryOutro();
                return;
              }
            }
          }
        }
        if (Game1.options.doesInputListContain(Game1.options.menuButton, key) || key == Keys.N)
        {
          if (this.responses != null && this.responses.Count > 0 && Game1.currentLocation.answerDialogue(this.responses[this.responses.Count - 1]))
            Game1.playSound("smallSelect");
          this.selectedResponse = -1;
          this.tryOutro();
        }
        else if (Game1.options.SnappyMenus)
        {
          this.safetyTimer = 0;
          base.receiveKeyPress(key);
        }
        else
        {
          if (key != Keys.Y || this.responses == null || this.responses.Count <= 0 || !this.responses[0].responseKey.Equals("Yes") || !Game1.currentLocation.answerDialogue(this.responses[0]))
            return;
          Game1.playSound("smallSelect");
          this.selectedResponse = -1;
          this.tryOutro();
        }
      }
      else
      {
        if (!Game1.options.SnappyMenus || !this.isQuestion || Game1.options.doesInputListContain(Game1.options.menuButton, key))
          return;
        base.receiveKeyPress(key);
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.transitioning)
        return;
      if (this.characterIndexInDialogue < this.getCurrentString().Length - 1)
      {
        this.characterIndexInDialogue = this.getCurrentString().Length - 1;
      }
      else
      {
        if (this.safetyTimer > 0)
          return;
        if (this.isQuestion)
        {
          if (this.selectedResponse == -1)
            return;
          this.questionFinishPauseTimer = Game1.eventUp ? 600 : 200;
          this.transitioning = true;
          this.transitionInitialized = false;
          this.transitioningBigger = true;
          if (this.characterDialogue != null)
          {
            this.characterDialoguesBrokenUp.Pop();
            this.characterDialogue.chooseResponse(this.responses[this.selectedResponse]);
            this.characterDialoguesBrokenUp.Push("");
            Game1.playSound("smallSelect");
          }
          else
          {
            Game1.dialogueUp = false;
            if (Game1.eventUp && Game1.currentLocation.afterQuestion == null)
            {
              Game1.playSound("smallSelect");
              Game1.currentLocation.currentEvent.answerDialogue(Game1.currentLocation.lastQuestionKey, this.selectedResponse);
              this.selectedResponse = -1;
              this.tryOutro();
              return;
            }
            if (Game1.currentLocation.answerDialogue(this.responses[this.selectedResponse]))
              Game1.playSound("smallSelect");
            this.selectedResponse = -1;
            this.tryOutro();
            return;
          }
        }
        else if (this.characterDialogue == null)
        {
          this.dialogues.RemoveAt(0);
          if (this.dialogues.Count == 0)
          {
            this.closeDialogue();
          }
          else
          {
            this.width = Math.Min(1200, SpriteText.getWidthOfString(this.dialogues[0]) + 64);
            this.height = SpriteText.getHeightOfString(this.dialogues[0], this.width - 16);
            this.x = (int) Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height).X;
            this.y = Game1.uiViewport.Height - this.height - 64;
            this.xPositionOnScreen = x;
            this.yPositionOnScreen = y;
            this.setUpIcons();
          }
        }
        this.characterIndexInDialogue = 0;
        if (this.characterDialogue != null)
        {
          int portraitIndex = this.characterDialogue.getPortraitIndex();
          if (this.characterDialoguesBrokenUp.Count == 0)
          {
            this.beginOutro();
            return;
          }
          this.characterDialoguesBrokenUp.Pop();
          if (this.characterDialoguesBrokenUp.Count == 0)
          {
            if (!this.characterDialogue.isCurrentStringContinuedOnNextScreen)
              this.beginOutro();
            this.characterDialogue.exitCurrentDialogue();
          }
          if (!this.characterDialogue.isDialogueFinished() && this.characterDialogue.getCurrentDialogue().Length > 0 && this.characterDialoguesBrokenUp.Count == 0)
            this.characterDialoguesBrokenUp.Push(this.characterDialogue.getCurrentDialogue());
          this.checkDialogue(this.characterDialogue);
          if (this.characterDialogue.getPortraitIndex() != portraitIndex)
            this.newPortaitShakeTimer = this.characterDialogue.getPortraitIndex() == 1 ? 250 : 50;
        }
        if (!this.transitioning)
          Game1.playSound("smallSelect");
        this.setUpIcons();
        this.safetyTimer = 750;
        if (this.getCurrentString() == null || this.getCurrentString().Length > 20)
          return;
        this.safetyTimer -= 200;
      }
    }

    private void setUpIcons()
    {
      this.dialogueIcon = (TemporaryAnimatedSprite) null;
      if (this.isQuestion)
        this.setUpQuestionIcon();
      else if (this.characterDialogue != null && (this.characterDialogue.isCurrentStringContinuedOnNextScreen || this.characterDialoguesBrokenUp.Count > 1))
        this.setUpNextPageIcon();
      else if (this.dialogues != null && this.dialogues.Count > 1)
        this.setUpNextPageIcon();
      else
        this.setUpCloseDialogueIcon();
      this.setUpForGamePadMode();
      if (this.getCurrentString() == null || this.getCurrentString().Length > 20)
        return;
      this.safetyTimer -= 200;
    }

    public override void performHoverAction(int mouseX, int mouseY)
    {
      this.hoverText = "";
      if (!this.transitioning && this.characterIndexInDialogue >= this.getCurrentString().Length - 1)
      {
        base.performHoverAction(mouseX, mouseY);
        if (this.isQuestion)
        {
          int selectedResponse = this.selectedResponse;
          int num = this.y - (this.heightForQuestions - this.height) + SpriteText.getHeightOfString(this.getCurrentString(), this.width - 16) + 48;
          for (int index = 0; index < this.responses.Count; ++index)
          {
            if (mouseY >= num && mouseY < num + SpriteText.getHeightOfString(this.responses[index].responseText, this.width - 16))
            {
              this.selectedResponse = index;
              if (this.responseCC != null && index < this.responseCC.Count)
              {
                this.currentlySnappedComponent = this.responseCC[index];
                break;
              }
              break;
            }
            num += SpriteText.getHeightOfString(this.responses[index].responseText, this.width - 16) + 16;
          }
          if (this.selectedResponse != selectedResponse)
            Game1.playSound("Cowboy_gunshot");
        }
      }
      if (this.shouldDrawFriendshipJewel() && this.friendshipJewel.Contains(mouseX, mouseY))
        this.hoverText = Game1.player.getFriendshipHeartLevelForNPC(this.characterDialogue.speaker.Name).ToString() + "/" + Utility.GetMaximumHeartsForCharacter((Character) this.characterDialogue.speaker).ToString() + "<";
      if (!Game1.options.SnappyMenus || this.currentlySnappedComponent == null)
        return;
      this.selectedResponse = this.currentlySnappedComponent.myID;
    }

    public bool shouldDrawFriendshipJewel() => this.width >= 642 && !Game1.eventUp && !this.isQuestion && !this.friendshipJewel.Equals(Rectangle.Empty) && this.characterDialogue != null && this.characterDialogue.speaker != null && Game1.player.friendshipData.ContainsKey(this.characterDialogue.speaker.Name) && this.characterDialogue.speaker.Name != "Henchman";

    private void setUpQuestionIcon() => this.dialogueIcon = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(330, 357, 7, 13), 100f, 6, 999999, new Vector2((float) (this.x + this.width - 40), (float) (this.y + this.height - 44)), false, false, 0.89f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      yPeriodic = true,
      yPeriodicLoopTime = 1500f,
      yPeriodicRange = 8f
    };

    private void setUpCloseDialogueIcon()
    {
      Vector2 position = new Vector2((float) (this.x + this.width - 40), (float) (this.y + this.height - 44));
      if (this.isPortraitBox())
        position.X -= 492f;
      this.dialogueIcon = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(289, 342, 11, 12), 80f, 11, 999999, position, false, false, 0.89f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true);
    }

    private void setUpNextPageIcon()
    {
      Vector2 position = new Vector2((float) (this.x + this.width - 40), (float) (this.y + this.height - 40));
      if (this.isPortraitBox())
        position.X -= 492f;
      this.dialogueIcon = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(232, 346, 9, 9), 90f, 6, 999999, position, false, false, 0.89f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        yPeriodic = true,
        yPeriodicLoopTime = 1500f,
        yPeriodicRange = 8f
      };
    }

    private void checkDialogue(Dialogue d)
    {
      this.isQuestion = false;
      string str1 = "";
      if (this.characterDialoguesBrokenUp.Count == 1)
        str1 = SpriteText.getSubstringBeyondHeight(this.characterDialoguesBrokenUp.Peek(), this.width - 460 - 20, this.height - 16);
      if (str1.Length > 0)
      {
        string str2 = this.characterDialoguesBrokenUp.Pop().Replace(Environment.NewLine, "");
        this.characterDialoguesBrokenUp.Push(str1.Trim());
        this.characterDialoguesBrokenUp.Push(str2.Substring(0, str2.Length - str1.Length + 1).Trim());
      }
      if (d.getCurrentDialogue().Length == 0)
        this.dialogueFinished = true;
      if (d.isCurrentStringContinuedOnNextScreen || this.characterDialoguesBrokenUp.Count > 1)
        this.dialogueContinuedOnNextPage = true;
      else if (d.getCurrentDialogue().Length == 0)
        this.beginOutro();
      if (!d.isCurrentDialogueAQuestion())
        return;
      this.responses = d.getResponseOptions();
      this.isQuestion = true;
      this.setUpQuestions();
    }

    private void setUpQuestions()
    {
      int widthConstraint = this.width - 16;
      this.heightForQuestions = SpriteText.getHeightOfString(this.getCurrentString(), widthConstraint);
      foreach (Response response in this.responses)
        this.heightForQuestions += SpriteText.getHeightOfString(response.responseText, widthConstraint) + 16;
      this.heightForQuestions += 40;
    }

    public bool isPortraitBox() => this.characterDialogue != null && this.characterDialogue.speaker != null && this.characterDialogue.speaker.Portrait != null && this.characterDialogue.showPortrait && Game1.options.showPortraits;

    public void drawBox(SpriteBatch b, int xPos, int yPos, int boxWidth, int boxHeight)
    {
      if (!this.transitionInitialized)
        return;
      b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos, boxWidth, boxHeight), new Rectangle?(new Rectangle(306, 320, 16, 16)), Color.White);
      b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos - 20, boxWidth, 24), new Rectangle?(new Rectangle(275, 313, 1, 6)), Color.White);
      b.Draw(Game1.mouseCursors, new Rectangle(xPos + 12, yPos + boxHeight, boxWidth - 20, 32), new Rectangle?(new Rectangle(275, 328, 1, 8)), Color.White);
      b.Draw(Game1.mouseCursors, new Rectangle(xPos - 32, yPos + 24, 32, boxHeight - 28), new Rectangle?(new Rectangle(264, 325, 8, 1)), Color.White);
      b.Draw(Game1.mouseCursors, new Rectangle(xPos + boxWidth, yPos, 28, boxHeight), new Rectangle?(new Rectangle(293, 324, 7, 1)), Color.White);
      b.Draw(Game1.mouseCursors, new Vector2((float) (xPos - 44), (float) (yPos - 28)), new Rectangle?(new Rectangle(261, 311, 14, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (xPos + boxWidth - 8), (float) (yPos - 28)), new Rectangle?(new Rectangle(291, 311, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (xPos + boxWidth - 8), (float) (yPos + boxHeight - 8)), new Rectangle?(new Rectangle(291, 326, 12, 12)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (xPos - 44), (float) (yPos + boxHeight - 4)), new Rectangle?(new Rectangle(261, 327, 14, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    }

    private bool shouldPortraitShake(Dialogue d)
    {
      int portraitIndex = d.getPortraitIndex();
      return d.speaker.Name.Equals("Pam") && portraitIndex == 3 || d.speaker.Name.Equals("Abigail") && portraitIndex == 7 || d.speaker.Name.Equals("Haley") && portraitIndex == 5 || d.speaker.Name.Equals("Maru") && portraitIndex == 9 || this.newPortaitShakeTimer > 0;
    }

    public void drawPortrait(SpriteBatch b)
    {
      if (this.width < 642)
        return;
      int num1 = this.x + this.width - 448 + 4;
      int num2 = this.x + this.width - num1;
      b.Draw(Game1.mouseCursors, new Rectangle(num1 - 40, this.y, 36, this.height), new Rectangle?(new Rectangle(278, 324, 9, 1)), Color.White);
      b.Draw(Game1.mouseCursors, new Vector2((float) (num1 - 40), (float) (this.y - 20)), new Rectangle?(new Rectangle(278, 313, 10, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (num1 - 40), (float) (this.y + this.height)), new Rectangle?(new Rectangle(278, 328, 10, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
      int num3 = num1 + 76;
      int num4 = this.y + this.height / 2 - 148 - 36;
      b.Draw(Game1.mouseCursors, new Vector2((float) (num1 - 8), (float) this.y), new Rectangle?(new Rectangle(583, 411, 115, 97)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
      Texture2D texture2D = this.characterDialogue.overridePortrait != null ? this.characterDialogue.overridePortrait : this.characterDialogue.speaker.Portrait;
      Rectangle rectangle = Game1.getSourceRectForStandardTileSheet(texture2D, this.characterDialogue.getPortraitIndex(), 64, 64);
      if (!texture2D.Bounds.Contains(rectangle))
        rectangle = new Rectangle(0, 0, 64, 64);
      int num5 = this.shouldPortraitShake(this.characterDialogue) ? Game1.random.Next(-1, 2) : 0;
      b.Draw(texture2D, new Vector2((float) (num3 + 16 + num5), (float) (num4 + 24)), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
      SpriteText.drawStringHorizontallyCenteredAt(b, this.characterDialogue.speaker.getName(), num1 + num2 / 2, num4 + 296 + 16);
      if (!this.shouldDrawFriendshipJewel())
        return;
      b.Draw(Game1.mouseCursors, new Vector2((float) this.friendshipJewel.X, (float) this.friendshipJewel.Y), new Rectangle?(Game1.player.getFriendshipHeartLevelForNPC(this.characterDialogue.speaker.Name) >= 10 ? new Rectangle(269, 494, 11, 11) : new Rectangle(Math.Max(140, 140 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 / 250.0) * 11), Math.Max(532, 532 + Game1.player.getFriendshipHeartLevelForNPC(this.characterDialogue.speaker.Name) / 2 * 11), 11, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
    }

    public string getCurrentString()
    {
      if (this.characterDialogue != null)
      {
        string currentString = this.characterDialoguesBrokenUp.Count <= 0 ? this.characterDialogue.getCurrentDialogue().Trim().Replace(Environment.NewLine, "") : this.characterDialoguesBrokenUp.Peek().Trim().Replace(Environment.NewLine, "");
        if (!Game1.options.showPortraits)
          currentString = this.characterDialogue.speaker.getName() + ": " + currentString;
        return currentString;
      }
      return this.dialogues.Count > 0 ? this.dialogues[0].Trim().Replace(Environment.NewLine, "") : "";
    }

    public override void update(GameTime time)
    {
      base.update(time);
      Game1.mouseCursorTransparency = !Game1.options.SnappyMenus || Game1.lastCursorMotionWasMouse ? 1f : 0.0f;
      if (this.isQuestion && this.characterIndexInDialogue >= this.getCurrentString().Length - 1 && !this.transitioning)
      {
        Game1.mouseCursorTransparency = 1f;
        if (!this._showedOptions)
        {
          this._showedOptions = true;
          if (this.responses != null)
          {
            this.responseCC = new List<ClickableComponent>();
            int y = this.y - (this.heightForQuestions - this.height) + SpriteText.getHeightOfString(this.getCurrentString(), this.width) + 48;
            for (int index = 0; index < this.responses.Count; ++index)
            {
              this.responseCC.Add(new ClickableComponent(new Rectangle(this.x + 8, y, this.width - 8, SpriteText.getHeightOfString(this.responses[index].responseText, this.width) + 16), "")
              {
                myID = index,
                downNeighborID = index < this.responses.Count - 1 ? index + 1 : -1,
                upNeighborID = index > 0 ? index - 1 : -1
              });
              y += SpriteText.getHeightOfString(this.responses[index].responseText, this.width) + 16;
            }
          }
          this.populateClickableComponentList();
          if (Game1.options.gamepadControls)
          {
            this.snapToDefaultClickableComponent();
            this.selectedResponse = this.currentlySnappedComponent.myID;
          }
        }
      }
      if (this.safetyTimer > 0)
        this.safetyTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.questionFinishPauseTimer > 0)
      {
        this.questionFinishPauseTimer -= time.ElapsedGameTime.Milliseconds;
      }
      else
      {
        TimeSpan elapsedGameTime;
        if (this.transitioning)
        {
          if (!this.transitionInitialized)
          {
            this.transitionInitialized = true;
            this.transitionX = this.x + this.width / 2;
            this.transitionY = this.y + this.height / 2;
            this.transitionWidth = 0;
            this.transitionHeight = 0;
          }
          if (this.transitioningBigger)
          {
            int transitionWidth1 = this.transitionWidth;
            this.transitionX -= (int) ((double) time.ElapsedGameTime.Milliseconds * 3.0);
            this.transitionY -= (int) ((double) time.ElapsedGameTime.Milliseconds * 3.0 * ((this.isQuestion ? (double) this.heightForQuestions : (double) this.height) / (double) this.width));
            this.transitionX = Math.Max(this.x, this.transitionX);
            this.transitionY = Math.Max(this.isQuestion ? this.y + this.height - this.heightForQuestions : this.y, this.transitionY);
            int transitionWidth2 = this.transitionWidth;
            elapsedGameTime = time.ElapsedGameTime;
            int num1 = (int) ((double) elapsedGameTime.Milliseconds * 3.0 * 2.0);
            this.transitionWidth = transitionWidth2 + num1;
            int transitionHeight = this.transitionHeight;
            elapsedGameTime = time.ElapsedGameTime;
            int num2 = (int) ((double) elapsedGameTime.Milliseconds * 3.0 * ((this.isQuestion ? (double) this.heightForQuestions : (double) this.height) / (double) this.width) * 2.0);
            this.transitionHeight = transitionHeight + num2;
            this.transitionWidth = Math.Min(this.width, this.transitionWidth);
            this.transitionHeight = Math.Min(this.isQuestion ? this.heightForQuestions : this.height, this.transitionHeight);
            if (transitionWidth1 == 0 && this.transitionWidth > 0)
              this.playOpeningSound();
            if (this.transitionX == this.x && this.transitionY == (this.isQuestion ? this.y + this.height - this.heightForQuestions : this.y))
            {
              this.transitioning = false;
              this.characterAdvanceTimer = 90;
              this.setUpIcons();
              this.transitionX = this.x;
              this.transitionY = this.y;
              this.transitionWidth = this.width;
              this.transitionHeight = this.height;
            }
          }
          else
          {
            this.transitionX += (int) ((double) time.ElapsedGameTime.Milliseconds * 3.0);
            this.transitionY += (int) ((double) time.ElapsedGameTime.Milliseconds * 3.0 * ((double) this.height / (double) this.width));
            this.transitionX = Math.Min(this.x + this.width / 2, this.transitionX);
            this.transitionY = Math.Min(this.y + this.height / 2, this.transitionY);
            int transitionWidth = this.transitionWidth;
            elapsedGameTime = time.ElapsedGameTime;
            int num3 = (int) ((double) elapsedGameTime.Milliseconds * 3.0 * 2.0);
            this.transitionWidth = transitionWidth - num3;
            int transitionHeight = this.transitionHeight;
            elapsedGameTime = time.ElapsedGameTime;
            int num4 = (int) ((double) elapsedGameTime.Milliseconds * 3.0 * ((double) this.height / (double) this.width) * 2.0);
            this.transitionHeight = transitionHeight - num4;
            this.transitionWidth = Math.Max(0, this.transitionWidth);
            this.transitionHeight = Math.Max(0, this.transitionHeight);
            if (this.transitionWidth == 0 && this.transitionHeight == 0)
              this.closeDialogue();
          }
        }
        if (!this.transitioning && this.characterIndexInDialogue < this.getCurrentString().Length)
        {
          int characterAdvanceTimer = this.characterAdvanceTimer;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds = elapsedGameTime.Milliseconds;
          this.characterAdvanceTimer = characterAdvanceTimer - milliseconds;
          if (this.characterAdvanceTimer <= 0)
          {
            this.characterAdvanceTimer = 30;
            int characterIndexInDialogue = this.characterIndexInDialogue;
            this.characterIndexInDialogue = Math.Min(this.characterIndexInDialogue + 1, this.getCurrentString().Length);
            if (this.characterIndexInDialogue != characterIndexInDialogue && this.characterIndexInDialogue == this.getCurrentString().Length)
              Game1.playSound("dialogueCharacterClose");
            if (this.characterIndexInDialogue > 1 && this.characterIndexInDialogue < this.getCurrentString().Length && Game1.options.dialogueTyping)
              Game1.playSound("dialogueCharacter");
          }
        }
        if (!this.transitioning && this.dialogueIcon != null)
          this.dialogueIcon.update(time);
        if (this.transitioning || this.newPortaitShakeTimer <= 0)
          return;
        int portaitShakeTimer = this.newPortaitShakeTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds1 = elapsedGameTime.Milliseconds;
        this.newPortaitShakeTimer = portaitShakeTimer - milliseconds1;
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      this.width = 1200;
      this.height = 384;
      this.x = (int) Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height).X;
      this.y = Game1.uiViewport.Height - this.height - 64;
      this.friendshipJewel = new Rectangle(this.x + this.width - 64, this.y + 256, 44, 44);
      this.setUpIcons();
    }

    public override void draw(SpriteBatch b)
    {
      if (this.width < 16 || this.height < 16)
        return;
      if (this.transitioning)
      {
        this.drawBox(b, this.transitionX, this.transitionY, this.transitionWidth, this.transitionHeight);
        this.drawMouse(b);
      }
      else
      {
        if (this.isQuestion)
        {
          this.drawBox(b, this.x, this.y - (this.heightForQuestions - this.height), this.width, this.heightForQuestions);
          SpriteText.drawString(b, this.getCurrentString(), this.x + 8, this.y + 12 - (this.heightForQuestions - this.height), this.characterIndexInDialogue, this.width - 16);
          if (this.characterIndexInDialogue >= this.getCurrentString().Length - 1)
          {
            int y = this.y - (this.heightForQuestions - this.height) + SpriteText.getHeightOfString(this.getCurrentString(), this.width - 16) + 48;
            for (int index = 0; index < this.responses.Count; ++index)
            {
              if (index == this.selectedResponse)
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.x + 4, y - 8, this.width - 8, SpriteText.getHeightOfString(this.responses[index].responseText, this.width) + 16, Color.White, 4f, false);
              SpriteText.drawString(b, this.responses[index].responseText, this.x + 8, y, width: this.width, alpha: (this.selectedResponse == index ? 1f : 0.6f));
              y += SpriteText.getHeightOfString(this.responses[index].responseText, this.width) + 16;
            }
          }
        }
        else
        {
          this.drawBox(b, this.x, this.y, this.width, this.height);
          if (!this.isPortraitBox() && !this.isQuestion)
            SpriteText.drawString(b, this.getCurrentString(), this.x + 8, this.y + 8, this.characterIndexInDialogue, this.width);
        }
        if (this.isPortraitBox() && !this.isQuestion)
        {
          this.drawPortrait(b);
          if (!this.isQuestion)
            SpriteText.drawString(b, this.getCurrentString(), this.x + 8, this.y + 8, this.characterIndexInDialogue, this.width - 460 - 24);
        }
        if (this.dialogueIcon != null && this.characterIndexInDialogue >= this.getCurrentString().Length - 1)
          this.dialogueIcon.draw(b, true);
        if (this.aboveDialogueImage != null)
        {
          this.drawBox(b, this.x + this.width / 2 - (int) ((double) (this.aboveDialogueImage.sourceRect.Width / 2) * (double) this.aboveDialogueImage.scale), this.y - 64 - 4 - (int) ((double) this.aboveDialogueImage.sourceRect.Height * (double) this.aboveDialogueImage.scale), (int) ((double) this.aboveDialogueImage.sourceRect.Width * (double) this.aboveDialogueImage.scale), (int) ((double) this.aboveDialogueImage.sourceRect.Height * (double) this.aboveDialogueImage.scale) + 8);
          Utility.drawWithShadow(b, this.aboveDialogueImage.texture, new Vector2((float) (this.x + this.width / 2) - (float) (this.aboveDialogueImage.sourceRect.Width / 2) * this.aboveDialogueImage.scale, (float) (this.y - 64 - (int) ((double) this.aboveDialogueImage.sourceRect.Height * (double) this.aboveDialogueImage.scale))), this.aboveDialogueImage.sourceRect, Color.White, 0.0f, Vector2.Zero, this.aboveDialogueImage.scale, layerDepth: 1f);
        }
        if (this.hoverText.Length > 0)
          SpriteText.drawStringWithScrollBackground(b, this.hoverText, this.friendshipJewel.Center.X - SpriteText.getWidthOfString(this.hoverText) / 2, this.friendshipJewel.Y - 64);
        this.drawMouse(b);
      }
    }
  }
}
