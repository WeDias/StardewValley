// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.FantasyBoardGame
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace StardewValley.Minigames
{
  public class FantasyBoardGame : IMinigame
  {
    public int borderSourceWidth = 138;
    public int borderSourceHeight = 74;
    public int slideSourceWidth = 128;
    public int slideSourceHeight = 64;
    private LocalizedContentManager content;
    private Texture2D slides;
    private Texture2D border;
    public int whichSlide;
    public int shakeTimer;
    public int endTimer;
    private string grade = "";

    public FantasyBoardGame()
    {
      this.content = Game1.content.CreateTemporary();
      this.slides = this.content.Load<Texture2D>("LooseSprites\\boardGame");
      this.border = this.content.Load<Texture2D>("LooseSprites\\boardGameBorder");
      Game1.globalFadeToClear();
    }

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public bool tick(GameTime time)
    {
      TimeSpan elapsedGameTime;
      if (this.shakeTimer > 0)
      {
        int shakeTimer = this.shakeTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.shakeTimer = shakeTimer - milliseconds;
      }
      Game1.currentLocation.currentEvent.checkForNextCommand(Game1.currentLocation, time);
      if (Game1.activeClickableMenu != null)
      {
        Game1.PushUIMode();
        Game1.activeClickableMenu.update(time);
        Game1.PopUIMode();
      }
      if (this.endTimer > 0)
      {
        int endTimer = this.endTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.endTimer = endTimer - milliseconds;
        if (this.endTimer <= 0 && this.whichSlide == -1)
          Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.end));
      }
      if (Game1.activeClickableMenu != null)
      {
        Game1.PushUIMode();
        Game1.activeClickableMenu.performHoverAction(Game1.getOldMouseX(), Game1.getOldMouseY());
        Game1.PopUIMode();
      }
      return false;
    }

    public void end()
    {
      this.unload();
      ++Game1.currentLocation.currentEvent.CurrentCommand;
      Game1.currentMinigame = (IMinigame) null;
    }

    public void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (Game1.activeClickableMenu == null)
        return;
      Game1.PushUIMode();
      Game1.activeClickableMenu.receiveLeftClick(x, y);
      Game1.PopUIMode();
    }

    public void leftClickHeld(int x, int y)
    {
    }

    public void receiveRightClick(int x, int y, bool playSound = true)
    {
      Game1.pressActionButton(Game1.GetKeyboardState(), Game1.input.GetMouseState(), Game1.input.GetGamePadState());
      if (Game1.activeClickableMenu == null)
        return;
      Game1.PushUIMode();
      Game1.activeClickableMenu.receiveRightClick(x, y);
      Game1.PopUIMode();
    }

    public void releaseLeftClick(int x, int y)
    {
    }

    public void releaseRightClick(int x, int y)
    {
    }

    public void receiveKeyPress(Keys k)
    {
      if (Game1.isQuestion)
      {
        if (Game1.options.doesInputListContain(Game1.options.moveUpButton, k))
        {
          Game1.currentQuestionChoice = Math.Max(Game1.currentQuestionChoice - 1, 0);
          Game1.playSound("toolSwap");
        }
        else
        {
          if (!Game1.options.doesInputListContain(Game1.options.moveDownButton, k))
            return;
          Game1.currentQuestionChoice = Math.Min(Game1.currentQuestionChoice + 1, Game1.questionChoices.Count - 1);
          Game1.playSound("toolSwap");
        }
      }
      else
      {
        if (Game1.activeClickableMenu == null)
          return;
        Game1.PushUIMode();
        Game1.activeClickableMenu.receiveKeyPress(k);
        Game1.PopUIMode();
      }
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void draw(SpriteBatch b)
    {
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if (this.whichSlide >= 0)
      {
        Vector2 vector2 = new Vector2();
        if (this.shakeTimer > 0)
          vector2 = new Vector2((float) Game1.random.Next(-2, 2), (float) Game1.random.Next(-2, 2));
        b.Draw(this.border, vector2 + new Vector2((float) (Game1.viewport.Width / 2 - this.borderSourceWidth * 4 / 2), (float) (Game1.viewport.Height / 2 - this.borderSourceHeight * 4 / 2 - 128)), new Rectangle?(new Rectangle(0, 0, this.borderSourceWidth, this.borderSourceHeight)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
        b.Draw(this.slides, vector2 + new Vector2((float) (Game1.viewport.Width / 2 - this.slideSourceWidth * 4 / 2), (float) (Game1.viewport.Height / 2 - this.slideSourceHeight * 4 / 2 - 128)), new Rectangle?(new Rectangle(this.whichSlide % 2 * this.slideSourceWidth, this.whichSlide / 2 * this.slideSourceHeight, this.slideSourceWidth, this.slideSourceHeight)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
      }
      else
      {
        string str = Game1.content.LoadString("Strings\\StringsFromCSFiles:FantasyBoardGame.cs.11980", (object) this.grade);
        float num = (float) Math.Sin((double) (this.endTimer / 1000)) * 8f;
        Game1.drawWithBorder(str, Game1.textColor, Color.Purple, new Vector2((float) (Game1.viewport.Width / 2) - Game1.dialogueFont.MeasureString(str).X / 2f, num + (float) (Game1.viewport.Height / 2)));
      }
      b.End();
      if (Game1.activeClickableMenu == null)
        return;
      Game1.PushUIMode();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      Game1.activeClickableMenu.draw(b);
      b.End();
      Game1.PopUIMode();
    }

    public void changeScreenSize()
    {
    }

    public void unload() => this.content.Unload();

    public void afterFade()
    {
      this.whichSlide = -1;
      int num = 0;
      if (Game1.player.mailReceived.Contains("savedFriends"))
        ++num;
      if (Game1.player.mailReceived.Contains("destroyedPods"))
        ++num;
      if (Game1.player.mailReceived.Contains("killedSkeleton"))
        ++num;
      switch (num)
      {
        case 0:
          this.grade = "D";
          break;
        case 1:
          this.grade = "C";
          break;
        case 2:
          this.grade = "B";
          break;
        case 3:
          this.grade = "A";
          break;
      }
      Game1.playSound("newArtifact");
      this.endTimer = 5500;
    }

    public void receiveEventPoke(int data)
    {
      if (data == -1)
        this.shakeTimer = 1000;
      else if (data == -2)
        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.afterFade));
      else
        this.whichSlide = data;
    }

    public string minigameId() => nameof (FantasyBoardGame);

    public bool doMainGameUpdates() => false;

    public bool forceQuit() => false;
  }
}
