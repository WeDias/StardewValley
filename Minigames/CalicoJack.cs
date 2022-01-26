// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.CalicoJack
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Minigames
{
  public class CalicoJack : IMinigame
  {
    public const int cardState_flipped = -1;
    public const int cardState_up = 0;
    public const int cardState_transitioning = 400;
    public const int bet = 100;
    public const int cardWidth = 96;
    public const int dealTime = 1000;
    public const int playingTo = 21;
    public const int passNumber = 18;
    public const int dealerTurnDelay = 1000;
    public List<int[]> playerCards;
    public List<int[]> dealerCards;
    private Random r;
    private int currentBet;
    private int startTimer;
    private int dealerTurnTimer = -1;
    private int bustTimer;
    private ClickableComponent hit;
    private ClickableComponent stand;
    private ClickableComponent doubleOrNothing;
    private ClickableComponent playAgain;
    private ClickableComponent quit;
    private ClickableComponent currentlySnappedComponent;
    private bool showingResultsScreen;
    private bool playerWon;
    private bool highStakes;
    private string endMessage = "";
    private string endTitle = "";
    private string coinBuffer;

    public CalicoJack(int toBet = -1, bool highStakes = false)
    {
      string str;
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.ru:
          str = "     ";
          break;
        case LocalizedContentManager.LanguageCode.zh:
          str = "　　";
          break;
        default:
          str = "  ";
          break;
      }
      this.coinBuffer = str;
      this.highStakes = highStakes;
      this.startTimer = 1000;
      this.playerCards = new List<int[]>();
      this.dealerCards = new List<int[]>();
      this.currentBet = toBet != -1 ? toBet : (highStakes ? 1000 : 100);
      ++Club.timesPlayedCalicoJack;
      this.r = new Random(Club.timesPlayedCalicoJack + (int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame);
      Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
      int x1 = (int) ((double) viewport.Width / (double) Game1.options.zoomLevel - 128.0 - (double) SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11924")));
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int y1 = viewport.Height / 2 - 64;
      int widthOfString1 = SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11924") + "  ");
      this.hit = new ClickableComponent(new Rectangle(x1, y1, widthOfString1, 64), "", " " + Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11924") + " ");
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int x2 = (int) ((double) viewport.Width / (double) Game1.options.zoomLevel - 128.0 - (double) SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11927")));
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int y2 = viewport.Height / 2 + 32;
      int widthOfString2 = SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11927") + "  ");
      this.stand = new ClickableComponent(new Rectangle(x2, y2, widthOfString2, 64), "", " " + Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11927") + " ");
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int x3 = (int) ((double) (viewport.Width / 2) / (double) Game1.options.zoomLevel) - SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11930")) / 2;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int y3 = (int) ((double) (viewport.Height / 2) / (double) Game1.options.zoomLevel);
      int width1 = SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11930")) + 64;
      this.doubleOrNothing = new ClickableComponent(new Rectangle(x3, y3, width1, 64), "", Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11930"));
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int x4 = (int) ((double) (viewport.Width / 2) / (double) Game1.options.zoomLevel) - SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11933")) / 2;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int y4 = (int) ((double) (viewport.Height / 2) / (double) Game1.options.zoomLevel) + 64 + 16;
      int width2 = SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11933")) + 64;
      this.playAgain = new ClickableComponent(new Rectangle(x4, y4, width2, 64), "", Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11933"));
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int x5 = (int) ((double) (viewport.Width / 2) / (double) Game1.options.zoomLevel) - SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11936")) / 2;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int y5 = (int) ((double) (viewport.Height / 2) / (double) Game1.options.zoomLevel) + 64 + 96;
      int width3 = SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11936")) + 64;
      this.quit = new ClickableComponent(new Rectangle(x5, y5, width3, 64), "", Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11936"));
      this.RepositionButtons();
      if (!Game1.options.SnappyMenus)
        return;
      this.currentlySnappedComponent = this.hit;
      this.currentlySnappedComponent.snapMouseCursorToCenter();
    }

    public void RepositionButtons()
    {
      this.hit.bounds = new Rectangle((int) ((double) Game1.game1.localMultiplayerWindow.Width / (double) Game1.options.zoomLevel - 128.0 - (double) SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11924"))), Game1.viewport.Height / 2 - 64, SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11924") + "  "), 64);
      this.stand.bounds = new Rectangle((int) ((double) Game1.game1.localMultiplayerWindow.Width / (double) Game1.options.zoomLevel - 128.0 - (double) SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11927"))), Game1.viewport.Height / 2 + 32, SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11927") + "  "), 64);
      this.doubleOrNothing.bounds = new Rectangle((int) ((double) (Game1.game1.localMultiplayerWindow.Width / 2) / (double) Game1.options.zoomLevel) - (SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11930")) + 64) / 2, (int) ((double) (Game1.game1.localMultiplayerWindow.Height / 2) / (double) Game1.options.zoomLevel), SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11930")) + 64, 64);
      this.playAgain.bounds = new Rectangle((int) ((double) (Game1.game1.localMultiplayerWindow.Width / 2) / (double) Game1.options.zoomLevel) - (SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11933")) + 64) / 2, (int) ((double) (Game1.game1.localMultiplayerWindow.Height / 2) / (double) Game1.options.zoomLevel) + 64 + 16, SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11933")) + 64, 64);
      this.quit.bounds = new Rectangle((int) ((double) (Game1.game1.localMultiplayerWindow.Width / 2) / (double) Game1.options.zoomLevel) - (SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11936")) + 64) / 2, (int) ((double) (Game1.game1.localMultiplayerWindow.Height / 2) / (double) Game1.options.zoomLevel) + 64 + 96, SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11936")) + 64, 64);
    }

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public bool playButtonsActive() => this.startTimer <= 0 && this.dealerTurnTimer < 0 && !this.showingResultsScreen;

    public bool tick(GameTime time)
    {
      TimeSpan elapsedGameTime;
      for (int index = 0; index < this.playerCards.Count; ++index)
      {
        if (this.playerCards[index][1] > 0)
        {
          ref int local = ref this.playerCards[index][1];
          int num = local;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds = elapsedGameTime.Milliseconds;
          local = num - milliseconds;
          if (this.playerCards[index][1] <= 0)
            this.playerCards[index][1] = 0;
        }
      }
      for (int index = 0; index < this.dealerCards.Count; ++index)
      {
        if (this.dealerCards[index][1] > 0)
        {
          ref int local = ref this.dealerCards[index][1];
          int num = local;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds = elapsedGameTime.Milliseconds;
          local = num - milliseconds;
          if (this.dealerCards[index][1] <= 0)
            this.dealerCards[index][1] = 0;
        }
      }
      if (this.startTimer > 0)
      {
        int startTimer1 = this.startTimer;
        int startTimer2 = this.startTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.startTimer = startTimer2 - milliseconds;
        if (startTimer1 % 250 < this.startTimer % 250)
        {
          switch (startTimer1 / 250)
          {
            case 1:
              this.playerCards.Add(new int[2]
              {
                this.r.Next(1, 10),
                400
              });
              break;
            case 2:
              this.playerCards.Add(new int[2]
              {
                this.r.Next(1, 12),
                400
              });
              break;
            case 3:
              this.dealerCards.Add(new int[2]
              {
                this.r.Next(1, 10),
                400
              });
              break;
            case 4:
              this.dealerCards.Add(new int[2]
              {
                this.r.Next(1, 12),
                -1
              });
              break;
          }
          Game1.playSound("shwip");
        }
      }
      else if (this.bustTimer > 0)
      {
        int bustTimer = this.bustTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.bustTimer = bustTimer - milliseconds;
        if (this.bustTimer <= 0)
          this.endGame();
      }
      else if (this.dealerTurnTimer > 0 && !this.showingResultsScreen)
      {
        int dealerTurnTimer = this.dealerTurnTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.dealerTurnTimer = dealerTurnTimer - milliseconds;
        if (this.dealerTurnTimer <= 0)
        {
          int num1 = 0;
          foreach (int[] dealerCard in this.dealerCards)
            num1 += dealerCard[0];
          int num2 = 0;
          foreach (int[] playerCard in this.playerCards)
            num2 += playerCard[0];
          if (this.dealerCards[0][1] == -1)
          {
            this.dealerCards[0][1] = 400;
            Game1.playSound("shwip");
          }
          else if (num1 < 18 || num1 < num2 && num2 <= 21)
          {
            int num3 = this.r.Next(1, 10);
            int num4 = 21 - num1;
            if (num2 == 20 && this.r.NextDouble() < 0.5)
              num3 = num4 + this.r.Next(1, 4);
            else if (num2 == 19 && this.r.NextDouble() < 0.25)
              num3 = num4 + this.r.Next(1, 4);
            else if (num2 == 18 && this.r.NextDouble() < 0.1)
              num3 = num4 + this.r.Next(1, 4);
            this.dealerCards.Add(new int[2]{ num3, 400 });
            int num5 = num1 + this.dealerCards.Last<int[]>()[0];
            Game1.playSound("shwip");
            if (num5 > 21)
              this.bustTimer = 2000;
          }
          else
            this.bustTimer = 50;
          this.dealerTurnTimer = 1000;
        }
      }
      if (this.playButtonsActive())
      {
        this.hit.scale = this.hit.bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 1.25f : 1f;
        this.stand.scale = this.stand.bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 1.25f : 1f;
      }
      else if (this.showingResultsScreen)
      {
        this.doubleOrNothing.scale = this.doubleOrNothing.bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 1.25f : 1f;
        this.playAgain.scale = this.playAgain.bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 1.25f : 1f;
        this.quit.scale = this.quit.bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 1.25f : 1f;
      }
      return false;
    }

    public void endGame()
    {
      if (Game1.options.SnappyMenus)
      {
        this.currentlySnappedComponent = this.quit;
        this.currentlySnappedComponent.snapMouseCursorToCenter();
      }
      this.showingResultsScreen = true;
      int num1 = 0;
      foreach (int[] playerCard in this.playerCards)
        num1 += playerCard[0];
      if (num1 == 21)
      {
        Game1.playSound("reward");
        this.playerWon = true;
        this.endTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11943");
        this.endMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11944");
        Game1.player.clubCoins += this.currentBet;
      }
      else if (num1 > 21)
      {
        Game1.playSound("fishEscape");
        this.endTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11946");
        this.endMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11947");
        Game1.player.clubCoins -= this.currentBet;
      }
      else
      {
        int num2 = 0;
        foreach (int[] dealerCard in this.dealerCards)
          num2 += dealerCard[0];
        if (num2 > 21)
        {
          Game1.playSound("reward");
          this.playerWon = true;
          this.endTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11943");
          this.endMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11950");
          Game1.player.clubCoins += this.currentBet;
        }
        else if (num1 == num2)
        {
          this.endTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11951");
          this.endMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11952");
        }
        else if (num1 > num2)
        {
          Game1.playSound("reward");
          this.endTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11943");
          this.endMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11955", (object) 21);
          Game1.player.clubCoins += this.currentBet;
          this.playerWon = true;
        }
        else
        {
          Game1.playSound("fishEscape");
          this.endTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11946");
          this.endMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11958", (object) 21);
          Game1.player.clubCoins -= this.currentBet;
        }
      }
    }

    public void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.playButtonsActive() && this.bustTimer <= 0)
      {
        if (this.hit.bounds.Contains(x, y))
        {
          int num1 = 0;
          foreach (int[] playerCard in this.playerCards)
            num1 += playerCard[0];
          int num2 = this.r.Next(1, 10);
          int num3 = 21 - num1;
          if (num3 > 1 && num3 < 6 && this.r.NextDouble() < 1.0 / (double) num3)
            num2 = this.r.NextDouble() < 0.5 ? num3 : num3 - 1;
          this.playerCards.Add(new int[2]{ num2, 400 });
          Game1.playSound("shwip");
          int num4 = 0;
          foreach (int[] playerCard in this.playerCards)
            num4 += playerCard[0];
          if (num4 == 21)
            this.bustTimer = 1000;
          else if (num4 > 21)
            this.bustTimer = 1000;
        }
        if (!this.stand.bounds.Contains(x, y))
          return;
        this.dealerTurnTimer = 1000;
        Game1.playSound("coin");
      }
      else
      {
        if (!this.showingResultsScreen)
          return;
        if (this.playerWon && this.doubleOrNothing.containsPoint(x, y))
        {
          Game1.currentMinigame = (IMinigame) new CalicoJack(this.currentBet * 2, this.highStakes);
          Game1.playSound("bigSelect");
        }
        if (Game1.player.clubCoins >= this.currentBet && this.playAgain.containsPoint(x, y))
        {
          Game1.currentMinigame = (IMinigame) new CalicoJack(highStakes: this.highStakes);
          Game1.playSound("smallSelect");
        }
        if (!this.quit.containsPoint(x, y))
          return;
        Game1.currentMinigame = (IMinigame) null;
        Game1.playSound("bigDeSelect");
      }
    }

    public void leftClickHeld(int x, int y)
    {
    }

    public void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public void releaseLeftClick(int x, int y)
    {
    }

    public void releaseRightClick(int x, int y)
    {
    }

    public void receiveKeyPress(Keys k)
    {
      if (!Game1.options.SnappyMenus)
        return;
      if (Game1.options.doesInputListContain(Game1.options.moveUpButton, k))
      {
        if (this.currentlySnappedComponent.Equals((object) this.stand))
          this.currentlySnappedComponent = this.hit;
        else if (this.currentlySnappedComponent.Equals((object) this.playAgain) && this.playerWon)
          this.currentlySnappedComponent = this.doubleOrNothing;
        else if (this.currentlySnappedComponent.Equals((object) this.quit) && Game1.player.clubCoins >= this.currentBet)
          this.currentlySnappedComponent = this.playAgain;
      }
      else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, k))
      {
        if (this.currentlySnappedComponent.Equals((object) this.hit))
          this.currentlySnappedComponent = this.stand;
        else if (this.currentlySnappedComponent.Equals((object) this.doubleOrNothing))
          this.currentlySnappedComponent = this.playAgain;
        else if (this.currentlySnappedComponent.Equals((object) this.playAgain))
          this.currentlySnappedComponent = this.quit;
      }
      if (this.currentlySnappedComponent == null)
        return;
      this.currentlySnappedComponent.snapMouseCursorToCenter();
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void draw(SpriteBatch b)
    {
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      SpriteBatch spriteBatch = b;
      Texture2D staminaRect = Game1.staminaRect;
      Viewport viewport1 = Game1.graphics.GraphicsDevice.Viewport;
      int width = viewport1.Width;
      viewport1 = Game1.graphics.GraphicsDevice.Viewport;
      int height1 = viewport1.Height;
      Rectangle destinationRectangle = new Rectangle(0, 0, width, height1);
      Color color = this.highStakes ? new Color(130, 0, 82) : Color.DarkGreen;
      spriteBatch.Draw(staminaRect, destinationRectangle, color);
      Vector2 vector2_1 = new Vector2((float) (Game1.graphics.GraphicsDevice.Viewport.Width - 192), 32f);
      SpriteText.drawStringWithScrollBackground(b, this.coinBuffer + Game1.player.clubCoins.ToString(), (int) vector2_1.X, (int) vector2_1.Y);
      Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(vector2_1.X + 4f, vector2_1.Y + 4f), new Rectangle(211, 373, 9, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
      if (this.showingResultsScreen)
      {
        SpriteText.drawStringWithScrollCenteredAt(b, this.endMessage, Game1.graphics.GraphicsDevice.Viewport.Width / 2, 48);
        SpriteText.drawStringWithScrollCenteredAt(b, this.endTitle, Game1.graphics.GraphicsDevice.Viewport.Width / 2, 128);
        if (!this.endTitle.Equals(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11951")))
        {
          SpriteBatch b1 = b;
          string s = Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11965", (object) ((this.playerWon ? "" : "-") + this.currentBet.ToString() + "   "));
          Viewport viewport2 = Game1.graphics.GraphicsDevice.Viewport;
          int x = viewport2.Width / 2;
          SpriteText.drawStringWithScrollCenteredAt(b1, s, x, 256);
          SpriteBatch b2 = b;
          Texture2D mouseCursors = Game1.mouseCursors;
          viewport2 = Game1.graphics.GraphicsDevice.Viewport;
          Vector2 position = new Vector2((float) (viewport2.Width / 2 - 32 + SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11965", (object) ((this.playerWon ? "" : "-") + this.currentBet.ToString() + "   "))) / 2), 260f) + new Vector2(8f, 0.0f);
          Rectangle sourceRect = new Rectangle(211, 373, 9, 10);
          Color white = Color.White;
          Vector2 zero = Vector2.Zero;
          Utility.drawWithShadow(b2, mouseCursors, position, sourceRect, white, 0.0f, zero, 4f, layerDepth: 1f);
        }
        if (this.playerWon)
        {
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.doubleOrNothing.bounds.X, this.doubleOrNothing.bounds.Y, this.doubleOrNothing.bounds.Width, this.doubleOrNothing.bounds.Height, Color.White, 4f * this.doubleOrNothing.scale);
          SpriteText.drawString(b, this.doubleOrNothing.label, this.doubleOrNothing.bounds.X + 32, this.doubleOrNothing.bounds.Y + 8);
        }
        if (Game1.player.clubCoins >= this.currentBet)
        {
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.playAgain.bounds.X, this.playAgain.bounds.Y, this.playAgain.bounds.Width, this.playAgain.bounds.Height, Color.White, 4f * this.playAgain.scale);
          SpriteText.drawString(b, this.playAgain.label, this.playAgain.bounds.X + 32, this.playAgain.bounds.Y + 8);
        }
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.quit.bounds.X, this.quit.bounds.Y, this.quit.bounds.Width, this.quit.bounds.Height, Color.White, 4f * this.quit.scale);
        SpriteText.drawString(b, this.quit.label, this.quit.bounds.X + 32, this.quit.bounds.Y + 8);
      }
      else
      {
        Vector2 vector2_2 = new Vector2(128f, (float) (Game1.graphics.GraphicsDevice.Viewport.Height - 320));
        int num1 = 0;
        foreach (int[] playerCard in this.playerCards)
        {
          int height2 = 144;
          if (playerCard[1] > 0)
            height2 = (int) ((double) Math.Abs((float) playerCard[1] - 200f) / 200.0 * 144.0);
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, playerCard[1] > 200 || playerCard[1] == -1 ? new Rectangle(399, 396, 15, 15) : new Rectangle(384, 396, 15, 15), (int) vector2_2.X, (int) vector2_2.Y + 72 - height2 / 2, 96, height2, Color.White, 4f);
          if (playerCard[1] == 0)
            SpriteText.drawStringHorizontallyCenteredAt(b, playerCard[0].ToString() ?? "", (int) vector2_2.X + 48 - 8 + 4, (int) vector2_2.Y + 72 - 16);
          vector2_2.X += 112f;
          if (playerCard[1] == 0)
            num1 += playerCard[0];
        }
        SpriteText.drawStringWithScrollBackground(b, Game1.player.Name + ": " + num1.ToString(), 160, (int) vector2_2.Y + 144 + 32);
        vector2_2.X = 128f;
        vector2_2.Y = 128f;
        int num2 = 0;
        foreach (int[] dealerCard in this.dealerCards)
        {
          int height3 = 144;
          if (dealerCard[1] > 0)
            height3 = (int) ((double) Math.Abs((float) dealerCard[1] - 200f) / 200.0 * 144.0);
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, dealerCard[1] > 200 || dealerCard[1] == -1 ? new Rectangle(399, 396, 15, 15) : new Rectangle(384, 396, 15, 15), (int) vector2_2.X, (int) vector2_2.Y + 72 - height3 / 2, 96, height3, Color.White, 4f);
          if (dealerCard[1] == 0)
            SpriteText.drawStringHorizontallyCenteredAt(b, dealerCard[0].ToString() ?? "", (int) vector2_2.X + 48 - 8 + 4, (int) vector2_2.Y + 72 - 16);
          vector2_2.X += 112f;
          if (dealerCard[1] == 0)
            num2 += dealerCard[0];
          else if (dealerCard[1] == -1)
            num2 = -99999;
        }
        SpriteText.drawStringWithScrollBackground(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11970", num2 > 0 ? (object) (num2.ToString() ?? "") : (object) "?"), 160, 32);
        SpriteText.drawStringWithScrollBackground(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11972", (object) (this.currentBet.ToString() + this.coinBuffer)), 160, Game1.graphics.GraphicsDevice.Viewport.Height / 2 - 48);
        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (172 + SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CalicoJack.cs.11972", (object) this.currentBet))), (float) (Game1.graphics.GraphicsDevice.Viewport.Height / 2 + 4 - 48)), new Rectangle(211, 373, 9, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
        if (this.playButtonsActive())
        {
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.hit.bounds.X, this.hit.bounds.Y, this.hit.bounds.Width, this.hit.bounds.Height, Color.White, 4f * this.hit.scale);
          SpriteText.drawString(b, this.hit.label, this.hit.bounds.X + 8, this.hit.bounds.Y + 8);
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.stand.bounds.X, this.stand.bounds.Y, this.stand.bounds.Width, this.stand.bounds.Height, Color.White, 4f * this.stand.scale);
          SpriteText.drawString(b, this.stand.label, this.stand.bounds.X + 8, this.stand.bounds.Y + 8);
        }
      }
      if (!Game1.options.hardwareCursor)
        b.Draw(Game1.mouseCursors, new Vector2((float) Game1.getMouseX(), (float) Game1.getMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float) (4.0 + (double) Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
      b.End();
    }

    public void changeScreenSize() => this.RepositionButtons();

    public void unload()
    {
    }

    public void receiveEventPoke(int data)
    {
    }

    public string minigameId() => nameof (CalicoJack);

    public bool doMainGameUpdates() => false;

    public bool forceQuit() => true;
  }
}
