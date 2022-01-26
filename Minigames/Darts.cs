// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.Darts
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using System;

namespace StardewValley.Minigames
{
  public class Darts : IMinigame
  {
    public Darts.GameState currentGameState;
    public float stateTimer;
    public float pixelScale = 4f;
    public bool gamePaused;
    public Vector2 upperLeft;
    private int screenWidth;
    private int screenHeight;
    private Texture2D texture;
    public Vector2 cursorPosition = new Vector2(0.0f, 0.0f);
    public Vector2 aimPosition = new Vector2(0.0f, 0.0f);
    public Vector2 dartBoardCenter = Vector2.Zero;
    protected bool canCancelShot = true;
    public float chargeTime;
    public float chargeDirection = 1f;
    public float hangTime;
    public int previousPoints;
    public int points;
    public float nextPointTransferTime;
    public static ICue chargeSound;
    public Vector2 throwStartPosition;
    public Vector2 dartPosition;
    public float dartTime = -1f;
    public string lastHitString = "";
    public int lastHitAmount;
    public bool shakeScore;
    public int startingDartCount = 20;
    public int dartCount = 20;
    public int throwsCount;
    public string alternateTextString = "";
    public string gameOverString = "";
    public bool lastHitWasDouble;

    public bool overrideFreeMouseMovement() => false;

    public Darts(int dart_count = 20)
    {
      this.startingDartCount = this.dartCount = dart_count;
      this.changeScreenSize();
      this.texture = Game1.content.Load<Texture2D>("Minigames\\Darts");
      this.points = 301;
      this.SetGameState(Darts.GameState.Aiming);
    }

    public virtual void SetGameState(Darts.GameState new_state)
    {
      if (this.currentGameState == Darts.GameState.Scoring)
      {
        this.previousPoints = this.points;
        this.shakeScore = false;
        this.alternateTextString = "";
      }
      if (this.currentGameState == Darts.GameState.Charging && Darts.chargeSound != null)
      {
        Darts.chargeSound.Stop(AudioStopOptions.Immediate);
        Darts.chargeSound = (ICue) null;
      }
      this.currentGameState = new_state;
      if (this.currentGameState == Darts.GameState.Aiming)
      {
        this.dartTime = -1f;
        if (!Game1.options.gamepadControls)
          return;
        Game1.setMousePosition(Utility.Vector2ToPoint(this.TransformDraw(new Vector2((float) (this.screenWidth / 2), (float) (this.screenHeight / 2)))));
      }
      else if (this.currentGameState == Darts.GameState.Charging)
      {
        if (Darts.chargeSound == null && Game1.soundBank != null)
        {
          Darts.chargeSound = Game1.soundBank.GetCue("SinWave");
          Darts.chargeSound.Play();
        }
        this.chargeTime = 1f;
        this.chargeDirection = -1f;
        this.canCancelShot = true;
      }
      else if (this.currentGameState == Darts.GameState.Firing)
      {
        this.throwStartPosition = this.dartBoardCenter + new Vector2(Utility.RandomFloat(-64f, 64f), 200f);
        Game1.playSound("FishHit");
        this.hangTime = 0.25f;
      }
      else if (this.currentGameState == Darts.GameState.ShowScore)
      {
        this.stateTimer = 1f;
      }
      else
      {
        if (this.currentGameState != Darts.GameState.GameOver)
          return;
        if (this.points == 0)
        {
          this.gameOverString = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11943");
          Game1.playSound("yoba");
        }
        else
        {
          this.gameOverString = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11946");
          Game1.playSound("slimedead");
        }
        this.stateTimer = 3f;
      }
    }

    public bool WasButtonHeld() => Game1.input.GetMouseState().LeftButton == ButtonState.Pressed || Game1.input.GetGamePadState().IsButtonDown(Buttons.A) || Game1.input.GetGamePadState().IsButtonDown(Buttons.X) || Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.actionButton) || Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.actionButton);

    public bool WasButtonPressed() => Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released || Game1.input.GetGamePadState().IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonUp(Buttons.A) || Game1.input.GetGamePadState().IsButtonDown(Buttons.X) && Game1.oldPadState.IsButtonUp(Buttons.X) || Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.actionButton) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.actionButton) || Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.actionButton) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.actionButton);

    public bool tick(GameTime time)
    {
      if ((double) this.stateTimer > 0.0)
      {
        this.stateTimer -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.stateTimer <= 0.0)
        {
          this.stateTimer = 0.0f;
          if (this.currentGameState == Darts.GameState.ShowScore)
          {
            if (this.lastHitAmount == 0)
            {
              if (this.dartCount <= 0)
                this.SetGameState(Darts.GameState.Scoring);
              else
                this.SetGameState(Darts.GameState.Aiming);
            }
            else
            {
              this.nextPointTransferTime = 0.5f;
              this.SetGameState(Darts.GameState.Scoring);
            }
          }
          else if (this.currentGameState == Darts.GameState.GameOver)
          {
            this.QuitGame();
            return true;
          }
        }
      }
      if (this.currentGameState == Darts.GameState.GameOver && this.WasButtonPressed())
      {
        this.QuitGame();
        return true;
      }
      this.cursorPosition = (Utility.PointToVector2(Game1.getMousePosition()) - this.upperLeft) / this.GetPixelScale();
      if (this.currentGameState == Darts.GameState.Aiming)
      {
        this.chargeTime = 1f;
        this.aimPosition = this.cursorPosition;
        this.aimPosition.X += (float) (Math.Sin(time.TotalGameTime.TotalSeconds * 0.75) * 32.0);
        this.aimPosition.Y += (float) (Math.Sin(time.TotalGameTime.TotalSeconds * 1.5) * 32.0);
        if (this.WasButtonPressed() && this.IsAiming())
          this.SetGameState(Darts.GameState.Charging);
      }
      else if (this.currentGameState == Darts.GameState.Charging)
      {
        if (Darts.chargeSound != null)
          Darts.chargeSound.SetVariable("Pitch", (float) (2400.0 * (1.0 - (double) this.chargeTime)));
        this.chargeTime += (float) time.ElapsedGameTime.TotalSeconds * this.chargeDirection;
        if ((double) this.chargeDirection < 0.0 && (double) this.chargeTime < 0.0)
        {
          this.canCancelShot = false;
          this.chargeTime = 0.0f;
          this.chargeDirection = 1f;
        }
        else if ((double) this.chargeDirection > 0.0 && (double) this.chargeTime >= 1.0)
        {
          this.chargeTime = 1f;
          this.chargeDirection = -1f;
        }
        if (!this.WasButtonHeld())
        {
          if ((double) this.chargeTime > 0.800000011920929 && this.canCancelShot)
          {
            this.SetGameState(Darts.GameState.Aiming);
            this.chargeTime = 0.0f;
          }
          else
          {
            --this.dartCount;
            ++this.throwsCount;
            this.FireDart(this.chargeTime);
          }
        }
      }
      else if (this.currentGameState == Darts.GameState.Firing)
      {
        if ((double) this.hangTime > 0.0)
        {
          this.hangTime -= (float) time.ElapsedGameTime.TotalSeconds;
          if ((double) this.hangTime <= 0.0)
          {
            float num = Utility.RandomFloat(0.0f, 6.283185f);
            this.aimPosition += new Vector2((float) Math.Sin((double) num), (float) Math.Cos((double) num)) * Utility.RandomFloat(0.0f, this.GetRadiusFromCharge() * 32f);
            Game1.playSound("cast");
            this.dartTime = 0.0f;
            this.dartPosition = this.throwStartPosition;
          }
        }
        else if ((double) this.dartTime >= 0.0)
        {
          this.dartTime += (float) (time.ElapsedGameTime.TotalSeconds / 0.75);
          this.dartPosition.X = Utility.Lerp(this.throwStartPosition.X, this.aimPosition.X, this.dartTime);
          this.dartPosition.Y = Utility.Lerp(this.throwStartPosition.Y, this.aimPosition.Y, this.dartTime);
          if ((double) this.dartTime >= 1.0)
          {
            Game1.playSound("Cowboy_gunshot");
            this.lastHitAmount = this.GetPointsForAim();
            this.SetGameState(Darts.GameState.ShowScore);
          }
        }
      }
      else if (this.currentGameState == Darts.GameState.Scoring)
      {
        if (this.lastHitAmount > 0)
        {
          if ((double) this.nextPointTransferTime > 0.0)
          {
            this.nextPointTransferTime -= (float) time.ElapsedGameTime.TotalSeconds;
            if ((double) this.nextPointTransferTime < 0.0)
            {
              int points = this.points;
              this.shakeScore = true;
              int num = 1;
              if (this.lastHitAmount > 10 && this.points > 10)
                num = 10;
              this.points -= num;
              this.lastHitAmount -= num;
              Game1.playSound("moneyDial");
              this.nextPointTransferTime = 0.05f;
              if (this.points < 0)
              {
                this.alternateTextString = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11947");
                Game1.playSound("fishEscape");
                this.nextPointTransferTime = 1f;
                this.lastHitAmount = 0;
              }
            }
          }
        }
        else
        {
          if ((double) this.nextPointTransferTime > 0.0)
            this.nextPointTransferTime -= (float) time.ElapsedGameTime.TotalSeconds;
          if ((double) this.nextPointTransferTime <= 0.0)
          {
            this.nextPointTransferTime = 0.0f;
            if (this.points == 0)
            {
              this.SetGameState(Darts.GameState.GameOver);
            }
            else
            {
              if (this.points < 0)
                this.points = this.previousPoints;
              if (this.dartCount <= 0)
                this.SetGameState(Darts.GameState.GameOver);
              else
                this.SetGameState(Darts.GameState.Aiming);
            }
          }
        }
      }
      Game1.mouseCursorTransparency = this.IsAiming() || this.currentGameState == Darts.GameState.Charging ? 0.0f : 1f;
      return false;
    }

    public virtual bool IsAiming() => this.currentGameState == Darts.GameState.Aiming && (double) this.cursorPosition.X > 0.0 && (double) this.cursorPosition.X < 320.0 && (double) this.cursorPosition.Y > 0.0 && (double) this.cursorPosition.Y < 320.0;

    public float GetRadiusFromCharge() => (float) Math.Pow((double) this.chargeTime, 0.5);

    public void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public void releaseLeftClick(int x, int y)
    {
    }

    public virtual int GetPointsForAim()
    {
      Vector2 vector2 = this.dartBoardCenter - this.aimPosition;
      float num1 = vector2.Length();
      if ((double) num1 < 5.0)
      {
        Game1.playSound("parrot");
        this.lastHitWasDouble = true;
        this.lastHitString = Game1.content.LoadString("Strings\\UI:Darts_Bullseye");
        return 50;
      }
      if ((double) num1 < 12.0)
      {
        Game1.playSound("parrot");
        this.lastHitString = Game1.content.LoadString("Strings\\UI:Darts_Bull");
        return 25;
      }
      if ((double) num1 > 88.0)
      {
        Game1.playSound("fishEscape");
        this.lastHitString = Game1.content.LoadString("Strings\\UI:Darts_OffTheIsland");
        return 0;
      }
      float num2 = (float) (Math.Atan2((double) vector2.Y, (double) vector2.X) * (180.0 / Math.PI)) - 81f;
      if ((double) num2 < 0.0)
        num2 += 360f;
      int index = (int) ((double) num2 / 18.0);
      int[] numArray = new int[20]
      {
        20,
        1,
        18,
        4,
        13,
        6,
        10,
        15,
        2,
        17,
        3,
        19,
        7,
        16,
        8,
        11,
        14,
        9,
        12,
        5
      };
      int pointsForAim = 0;
      if (index < numArray.Length)
        pointsForAim = numArray[index];
      if ((double) num1 >= 46.0 && (double) num1 < 55.0)
      {
        Game1.playSound("parrot");
        this.lastHitString = pointsForAim.ToString() + "x3";
        return pointsForAim * 3;
      }
      if ((double) num1 >= 79.0)
      {
        this.lastHitWasDouble = true;
        Game1.playSound("parrot");
        this.lastHitString = pointsForAim.ToString() + "x2";
        return pointsForAim * 2;
      }
      this.lastHitString = pointsForAim.ToString() ?? "";
      return pointsForAim;
    }

    public virtual void FireDart(float radius) => this.SetGameState(Darts.GameState.Firing);

    public void releaseRightClick(int x, int y)
    {
    }

    public void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public void receiveKeyPress(Keys k)
    {
      if (!Game1.input.GetGamePadState().IsButtonDown(Buttons.Back) && !k.Equals((object) Keys.Escape))
        return;
      this.QuitGame();
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void QuitGame()
    {
      this.unload();
      Game1.playSound("bigDeSelect");
      Game1.currentMinigame = (IMinigame) null;
      if (this.currentGameState != Darts.GameState.GameOver)
        return;
      if (this.points == 0)
      {
        bool flag = this.IsPerfectVictory();
        if (flag)
          Game1.multiplayer.globalChatInfoMessage("DartsWinPerfect", Game1.player.Name);
        else
          Game1.multiplayer.globalChatInfoMessage("DartsWin", Game1.player.Name, this.throwsCount.ToString() ?? "");
        if (!(Game1.currentLocation is IslandSouthEastCave))
          return;
        string str1 = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_Win");
        if (flag)
          str1 = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_Win_Perfect");
        string str2 = str1 + "#";
        int droppedLimitedNutCount = Game1.player.team.GetDroppedLimitedNutCount(nameof (Darts));
        string dialogue;
        if (this.startingDartCount == 20 && droppedLimitedNutCount == 0 || this.startingDartCount == 15 && droppedLimitedNutCount == 1 || this.startingDartCount == 10 && droppedLimitedNutCount == 2)
        {
          dialogue = str2 + Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_WinPrize");
          Game1.afterDialogues += (Game1.afterFadeFunction) (() => Game1.player.team.RequestLimitedNutDrops(nameof (Darts), Game1.currentLocation, 1984, 512, 3));
        }
        else
          dialogue = str2 + Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_WinNoPrize");
        Game1.drawDialogueNoTyping(dialogue);
      }
      else
      {
        if (!(Game1.currentLocation is IslandSouthEastCave))
          return;
        Game1.drawDialogueNoTyping(Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_Lose"));
      }
    }

    public void draw(SpriteBatch b)
    {
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, rasterizerState: new RasterizerState());
      b.Draw(this.texture, this.TransformDraw(new Rectangle(0, 0, 320, 320)), new Rectangle?(new Rectangle(0, 0, 320, 320)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
      if (this.IsAiming() || this.currentGameState == Darts.GameState.Charging)
        b.Draw(this.texture, this.TransformDraw(this.aimPosition), new Rectangle?(new Rectangle(0, 320, 64, 64)), Color.White * 0.5f, 0.0f, new Vector2(32f, 32f), this.GetPixelScale() * this.GetRadiusFromCharge(), SpriteEffects.None, 0.0f);
      if ((double) this.dartTime >= 0.0)
      {
        Rectangle rectangle = new Rectangle(0, 384, 16, 32);
        if ((double) this.dartTime > 0.649999976158142)
          rectangle.X = 16;
        if ((double) this.dartTime > 0.899999976158142)
          rectangle.X = 32;
        float y = (float) Math.Sin((double) this.dartTime * Math.PI) * 200f;
        float rotation = (float) Math.Atan2((double) this.aimPosition.X - (double) this.throwStartPosition.X, (double) this.throwStartPosition.Y - (double) this.aimPosition.Y);
        b.Draw(this.texture, this.TransformDraw(this.dartPosition - new Vector2(0.0f, y)), new Rectangle?(rectangle), Color.White, rotation, new Vector2(8f, 16f), this.GetPixelScale(), SpriteEffects.None, 0.02f);
      }
      Vector2 vector2_1 = this.TransformDraw(new Vector2(160f, 16f));
      Vector2 vector2_2 = Vector2.Zero;
      if (this.shakeScore)
        vector2_2 = new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
      if (this.alternateTextString != "")
        SpriteText.drawStringWithScrollCenteredAt(b, this.alternateTextString, (int) ((double) vector2_1.X + (double) vector2_2.X), (int) ((double) vector2_1.Y + (double) vector2_2.Y), "", 1f, 2, 0, 0.88f, false);
      else if (this.points >= 0)
      {
        string s1 = Game1.content.LoadString("Strings\\UI:Darts_PointsToGo", (object) this.points);
        if (this.points == 1)
          s1 = Game1.content.LoadString("Strings\\UI:Darts_PointToGo", (object) this.points);
        SpriteText.drawStringWithScrollCenteredAt(b, s1, (int) ((double) vector2_1.X + (double) vector2_2.X), (int) ((double) vector2_1.Y + (double) vector2_2.Y));
        if (this.currentGameState == Darts.GameState.ShowScore || this.currentGameState == Darts.GameState.Scoring)
        {
          if (this.shakeScore)
            vector2_2 = new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
          vector2_1.Y += 64f;
          string s2 = this.currentGameState != Darts.GameState.ShowScore ? " " + this.lastHitAmount.ToString() + " " : " " + this.lastHitString + " ";
          SpriteText.drawStringWithScrollCenteredAt(b, s2, (int) ((double) vector2_1.X + (double) vector2_2.X), (int) ((double) vector2_1.Y + (double) vector2_2.Y), "", 1f, 1, 2, 0.88f, false);
        }
      }
      for (int index = 0; index < this.dartCount; ++index)
      {
        Vector2 dest = new Vector2((float) (7 + index * 10), 317f);
        b.Draw(this.texture, this.TransformDraw(dest), new Rectangle?(new Rectangle(64, 384, 16, 32)), Color.White, 0.0f, new Vector2(0.0f, 32f), this.GetPixelScale(), SpriteEffects.None, 0.02f);
      }
      if (this.gameOverString != "")
      {
        b.Draw(Game1.staminaRect, this.TransformDraw(new Rectangle(0, 0, this.screenWidth, this.screenHeight)), new Rectangle?(), Color.Black * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
        if (this.points == 0)
        {
          Vector2 vector2_3 = this.TransformDraw(new Vector2(160f, 144f));
          SpriteText.drawStringWithScrollCenteredAt(b, this.gameOverString, (int) vector2_3.X, (int) vector2_3.Y);
          Vector2 vector2_4 = this.TransformDraw(new Vector2(160f, 176f));
          if (this.IsPerfectVictory())
            SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\UI:Darts_WinTextPerfect", (object) this.throwsCount), (int) ((double) vector2_4.X + (double) vector2_2.X), (int) ((double) vector2_4.Y + (double) vector2_2.Y), "", 1f, 1, 2, 0.88f, false);
          else
            SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\UI:Darts_WinText", (object) this.throwsCount), (int) ((double) vector2_4.X + (double) vector2_2.X), (int) ((double) vector2_4.Y + (double) vector2_2.Y), "", 1f, 1, 2, 0.88f, false);
        }
        else
        {
          Vector2 vector2_5 = this.TransformDraw(new Vector2(160f, 160f));
          SpriteText.drawStringWithScrollCenteredAt(b, this.gameOverString, (int) vector2_5.X, (int) vector2_5.Y);
        }
      }
      if (Game1.options.gamepadControls && !Game1.options.hardwareCursor)
        b.Draw(Game1.mouseCursors, new Vector2((float) Game1.getMouseX(), (float) Game1.getMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, !Game1.options.snappyMenus || !Game1.options.gamepadControls ? 0 : 44, 16, 16)), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, (float) (4.0 + (double) Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
      b.End();
    }

    public void DrawNumberString(SpriteBatch b, string text, int x, int y)
    {
      int num1 = 14;
      for (int index = 0; index < text.Length; ++index)
      {
        Rectangle rectangle = new Rectangle(96, 320, 16, 32);
        if (text[index] >= '0' && text[index] <= '9')
        {
          int num2 = (int) text[index] - 48;
          rectangle.X += num2 * 16;
        }
        else if (text[index] == 'x')
        {
          rectangle.X = 256;
        }
        else
        {
          x += num1;
          continue;
        }
        b.Draw(this.texture, this.TransformDraw(new Vector2((float) x, (float) y)), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.0f);
        x += num1;
      }
    }

    public float GetPixelScale() => this.pixelScale;

    public Rectangle TransformDraw(Rectangle dest)
    {
      dest.X = (int) Math.Round((double) dest.X * (double) this.pixelScale) + (int) this.upperLeft.X;
      dest.Y = (int) Math.Round((double) dest.Y * (double) this.pixelScale) + (int) this.upperLeft.Y;
      dest.Width = (int) ((double) dest.Width * (double) this.pixelScale);
      dest.Height = (int) ((double) dest.Height * (double) this.pixelScale);
      return dest;
    }

    public Vector2 TransformDraw(Vector2 dest)
    {
      dest.X = (float) ((int) Math.Round((double) dest.X * (double) this.pixelScale) + (int) this.upperLeft.X);
      dest.Y = (float) ((int) Math.Round((double) dest.Y * (double) this.pixelScale) + (int) this.upperLeft.Y);
      return dest;
    }

    public bool IsPerfectVictory() => this.points == 0 && this.throwsCount <= 6;

    public void changeScreenSize()
    {
      this.screenWidth = 320;
      this.screenHeight = 320;
      float num1 = 1f / Game1.options.zoomLevel;
      int width = Game1.game1.localMultiplayerWindow.Width;
      int height = Game1.game1.localMultiplayerWindow.Height;
      this.pixelScale = Math.Min(5f, Math.Min((float) width * num1 / (float) this.screenWidth, (float) height * num1 / (float) this.screenHeight));
      float num2 = 0.1f;
      this.pixelScale = (float) (int) ((double) this.pixelScale / (double) num2) * num2;
      this.upperLeft = new Vector2((float) (width / 2) * num1, (float) (height / 2) * num1);
      this.upperLeft.X -= (float) (this.screenWidth / 2) * this.pixelScale;
      this.upperLeft.Y -= (float) (this.screenHeight / 2) * this.pixelScale;
      this.dartBoardCenter = new Vector2(160f, 160f);
    }

    public void unload()
    {
      if (Darts.chargeSound != null)
      {
        Darts.chargeSound.Stop(AudioStopOptions.Immediate);
        Darts.chargeSound = (ICue) null;
      }
      Game1.stopMusicTrack(Game1.MusicContext.MiniGame);
      Game1.player.faceDirection(0);
    }

    public bool forceQuit()
    {
      this.unload();
      return true;
    }

    public void leftClickHeld(int x, int y)
    {
    }

    public void receiveEventPoke(int data) => throw new NotImplementedException();

    public string minigameId() => nameof (Darts);

    public bool doMainGameUpdates() => false;

    public enum GameState
    {
      Aiming,
      Charging,
      Firing,
      ShowScore,
      Scoring,
      GameOver,
    }
  }
}
