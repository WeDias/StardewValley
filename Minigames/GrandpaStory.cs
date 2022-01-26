// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.GrandpaStory
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace StardewValley.Minigames
{
  public class GrandpaStory : IMinigame
  {
    public const int sceneWidth = 1294;
    public const int sceneHeight = 730;
    public const int scene_beforeGrandpa = 0;
    public const int scene_grandpaSpeech = 1;
    public const int scene_fadeOutFromGrandpa = 2;
    public const int scene_timePass = 3;
    public const int scene_jojaCorpOverhead = 4;
    public const int scene_jojaCorpPan = 5;
    public const int scene_desk = 6;
    private LocalizedContentManager content;
    private Texture2D texture;
    private float foregroundFade;
    private float backgroundFade;
    private float foregroundFadeChange;
    private float backgroundFadeChange;
    private float panX;
    private float letterScale = 0.5f;
    private float letterDy;
    private float letterDyDy;
    private int scene;
    private int totalMilliseconds;
    private int grandpaSpeechTimer;
    private int parallaxPan;
    private int letterOpenTimer;
    private bool drawGrandpa;
    private bool letterReceived;
    private bool mouseActive;
    private bool clickedLetter;
    private bool quit;
    private bool fadingToQuit;
    private Queue<string> grandpaSpeech;
    private Vector2 letterPosition = new Vector2(477f, 345f);
    private LetterViewerMenu letterView;

    public GrandpaStory()
    {
      Game1.changeMusicTrack("none");
      this.content = Game1.content.CreateTemporary();
      this.texture = this.content.Load<Texture2D>("Minigames\\jojacorps");
      this.backgroundFadeChange = 0.0003f;
      this.grandpaSpeech = new Queue<string>();
      this.grandpaSpeech.Enqueue(Game1.player.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12026") : Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12028"));
      this.grandpaSpeech.Enqueue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12029"));
      this.grandpaSpeech.Enqueue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12030"));
      this.grandpaSpeech.Enqueue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12031"));
      this.grandpaSpeech.Enqueue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12034"));
      this.grandpaSpeech.Enqueue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12035"));
      this.grandpaSpeech.Enqueue(Game1.player.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12036") : Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12038"));
      this.grandpaSpeech.Enqueue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12040"));
      Game1.player.Position = new Vector2(this.panX, (float) (Game1.graphics.GraphicsDevice.Viewport.Height / 2 - 360)) + new Vector2(3000f, 376f);
      Game1.viewport.X = 0;
      Game1.viewport.Y = 0;
      Game1.currentLocation = new GameLocation("Maps\\FarmHouse", "Temp");
      Game1.currentLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
      Game1.player.currentLocation = Game1.currentLocation;
    }

    public bool tick(GameTime time)
    {
      if (this.quit)
      {
        this.unload();
        Game1.currentMinigame = (IMinigame) new Intro();
        return false;
      }
      if (this.letterView != null)
        this.letterView.update(time);
      int totalMilliseconds = this.totalMilliseconds;
      TimeSpan elapsedGameTime = time.ElapsedGameTime;
      int milliseconds1 = elapsedGameTime.Milliseconds;
      this.totalMilliseconds = totalMilliseconds + milliseconds1;
      this.totalMilliseconds %= 9000000;
      double backgroundFade = (double) this.backgroundFade;
      double backgroundFadeChange = (double) this.backgroundFadeChange;
      elapsedGameTime = time.ElapsedGameTime;
      double milliseconds2 = (double) elapsedGameTime.Milliseconds;
      double num1 = backgroundFadeChange * milliseconds2;
      this.backgroundFade = (float) (backgroundFade + num1);
      this.backgroundFade = Math.Max(0.0f, Math.Min(1f, this.backgroundFade));
      double foregroundFade = (double) this.foregroundFade;
      double foregroundFadeChange = (double) this.foregroundFadeChange;
      elapsedGameTime = time.ElapsedGameTime;
      double milliseconds3 = (double) elapsedGameTime.Milliseconds;
      double num2 = foregroundFadeChange * milliseconds3;
      this.foregroundFade = (float) (foregroundFade + num2);
      this.foregroundFade = Math.Max(0.0f, Math.Min(1f, this.foregroundFade));
      int grandpaSpeechTimer1 = this.grandpaSpeechTimer;
      if ((double) this.foregroundFade >= 1.0 && this.fadingToQuit)
      {
        this.unload();
        Game1.currentMinigame = (IMinigame) new Intro();
        return false;
      }
      switch (this.scene)
      {
        case 0:
          if ((double) this.backgroundFade == 1.0)
          {
            if (!this.drawGrandpa)
            {
              this.foregroundFade = 1f;
              this.foregroundFadeChange = -0.0005f;
              this.drawGrandpa = true;
            }
            if ((double) this.foregroundFade == 0.0)
            {
              this.scene = 1;
              Game1.changeMusicTrack("grandpas_theme");
              break;
            }
            break;
          }
          break;
        case 1:
          int grandpaSpeechTimer2 = this.grandpaSpeechTimer;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds4 = elapsedGameTime.Milliseconds;
          this.grandpaSpeechTimer = grandpaSpeechTimer2 + milliseconds4;
          if (this.grandpaSpeechTimer >= 60000)
            this.foregroundFadeChange = 0.0005f;
          if ((double) this.foregroundFade >= 1.0)
          {
            this.drawGrandpa = false;
            this.scene = 3;
            this.grandpaSpeechTimer = 0;
            this.foregroundFade = 0.0f;
            this.foregroundFadeChange = 0.0f;
          }
          if (grandpaSpeechTimer1 % 10000 > this.grandpaSpeechTimer % 10000 && this.grandpaSpeech.Count > 0)
            this.grandpaSpeech.Dequeue();
          if (grandpaSpeechTimer1 < 25000 && this.grandpaSpeechTimer > 25000 && this.grandpaSpeech.Count > 0)
            this.grandpaSpeech.Dequeue();
          if (grandpaSpeechTimer1 < 17000 && this.grandpaSpeechTimer >= 17000)
          {
            Game1.playSound("newRecipe");
            this.letterReceived = true;
            this.letterDy = -0.6f;
            this.letterDyDy = 1f / 1000f;
          }
          if (this.letterReceived && (double) this.letterPosition.Y <= (double) Game1.viewport.Height)
          {
            double letterDy1 = (double) this.letterDy;
            double letterDyDy = (double) this.letterDyDy;
            elapsedGameTime = time.ElapsedGameTime;
            double milliseconds5 = (double) elapsedGameTime.Milliseconds;
            double num3 = letterDyDy * milliseconds5;
            this.letterDy = (float) (letterDy1 + num3);
            ref float local1 = ref this.letterPosition.Y;
            double num4 = (double) local1;
            double letterDy2 = (double) this.letterDy;
            elapsedGameTime = time.ElapsedGameTime;
            double milliseconds6 = (double) elapsedGameTime.Milliseconds;
            double num5 = letterDy2 * milliseconds6;
            local1 = (float) (num4 + num5);
            ref float local2 = ref this.letterPosition.X;
            double num6 = (double) local2;
            elapsedGameTime = time.ElapsedGameTime;
            double num7 = 0.00999999977648258 * (double) elapsedGameTime.Milliseconds;
            local2 = (float) (num6 + num7);
            double letterScale = (double) this.letterScale;
            elapsedGameTime = time.ElapsedGameTime;
            double num8 = 1.0 / 800.0 * (double) elapsedGameTime.Milliseconds;
            this.letterScale = (float) (letterScale + num8);
            if ((double) this.letterPosition.Y > (double) Game1.viewport.Height)
            {
              Game1.playSound("coin");
              break;
            }
            break;
          }
          break;
        case 3:
          int grandpaSpeechTimer3 = this.grandpaSpeechTimer;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds7 = elapsedGameTime.Milliseconds;
          this.grandpaSpeechTimer = grandpaSpeechTimer3 + milliseconds7;
          if (this.grandpaSpeechTimer > 2600 && grandpaSpeechTimer1 <= 2600)
          {
            Game1.changeMusicTrack("jojaOfficeSoundscape");
            break;
          }
          if (this.grandpaSpeechTimer > 4000)
          {
            this.grandpaSpeechTimer = 0;
            this.scene = 4;
            break;
          }
          break;
        case 4:
          int grandpaSpeechTimer4 = this.grandpaSpeechTimer;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds8 = elapsedGameTime.Milliseconds;
          this.grandpaSpeechTimer = grandpaSpeechTimer4 + milliseconds8;
          if (this.grandpaSpeechTimer >= 9000)
          {
            this.grandpaSpeechTimer = 0;
            this.scene = 5;
            Game1.player.faceDirection(1);
            Game1.player.currentEyes = 1;
          }
          if (this.grandpaSpeechTimer >= 7000)
          {
            Game1.viewport.X = 0;
            Game1.viewport.Y = 0;
            double panX = (double) this.panX;
            elapsedGameTime = time.ElapsedGameTime;
            double num9 = 0.200000002980232 * (double) elapsedGameTime.Milliseconds;
            this.panX = (float) (panX - num9);
            Game1.player.Position = new Vector2(this.panX, (float) (Game1.graphics.GraphicsDevice.Viewport.Height / 2 - 360)) + new Vector2(3612f, 572f);
            break;
          }
          break;
        case 5:
          if ((double) this.panX > (double) (Math.Max(1600, Game1.viewport.Width) - 4800))
          {
            Game1.viewport.X = 0;
            Game1.viewport.Y = 0;
            double panX = (double) this.panX;
            elapsedGameTime = time.ElapsedGameTime;
            double num10 = 0.200000002980232 * (double) elapsedGameTime.Milliseconds;
            this.panX = (float) (panX - num10);
            Game1.player.Position = new Vector2(this.panX, (float) ((double) Game1.graphics.GraphicsDevice.Viewport.Height / (double) Game1.options.zoomLevel / 2.0 - 360.0)) + new Vector2(3612f, 572f);
            break;
          }
          int grandpaSpeechTimer5 = this.grandpaSpeechTimer;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds9 = elapsedGameTime.Milliseconds;
          this.grandpaSpeechTimer = grandpaSpeechTimer5 + milliseconds9;
          if (grandpaSpeechTimer1 < 2000 && this.grandpaSpeechTimer >= 2000)
            Game1.player.currentEyes = 4;
          if (grandpaSpeechTimer1 < 3000 && this.grandpaSpeechTimer >= 3000)
          {
            Game1.player.currentEyes = 1;
            Game1.player.jitterStrength = 1f;
          }
          if (grandpaSpeechTimer1 < 3500 && this.grandpaSpeechTimer >= 3500)
            Game1.player.stopJittering();
          if (grandpaSpeechTimer1 < 4000 && this.grandpaSpeechTimer >= 4000)
          {
            Game1.player.currentEyes = 1;
            Game1.player.jitterStrength = 1f;
          }
          if (grandpaSpeechTimer1 < 4500 && this.grandpaSpeechTimer >= 4500)
          {
            Game1.player.stopJittering();
            Game1.player.doEmote(28);
          }
          if (grandpaSpeechTimer1 < 7000 && this.grandpaSpeechTimer >= 7000)
            Game1.player.currentEyes = 4;
          if (grandpaSpeechTimer1 < 8000 && this.grandpaSpeechTimer >= 8000)
            Game1.player.showFrame(33);
          if (this.grandpaSpeechTimer >= 10000)
          {
            this.scene = 6;
            this.grandpaSpeechTimer = 0;
          }
          Game1.player.Position = new Vector2(this.panX, (float) ((double) Game1.graphics.GraphicsDevice.Viewport.Height / (double) Game1.options.zoomLevel / 2.0 - 360.0)) + new Vector2(3612f, 572f);
          break;
        case 6:
          int grandpaSpeechTimer6 = this.grandpaSpeechTimer;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds10 = elapsedGameTime.Milliseconds;
          this.grandpaSpeechTimer = grandpaSpeechTimer6 + milliseconds10;
          if (this.grandpaSpeechTimer >= 2000)
          {
            int parallaxPan = this.parallaxPan;
            elapsedGameTime = time.ElapsedGameTime;
            int num11 = (int) Math.Ceiling(0.1 * (double) elapsedGameTime.Milliseconds);
            this.parallaxPan = parallaxPan + num11;
            if (this.parallaxPan >= 107)
              this.parallaxPan = 107;
          }
          if (grandpaSpeechTimer1 < 3500 && this.grandpaSpeechTimer >= 3500)
            Game1.changeMusicTrack("none");
          if (grandpaSpeechTimer1 < 5000 && this.grandpaSpeechTimer >= 5000)
            Game1.playSound("doorCreak");
          if (grandpaSpeechTimer1 < 6000 && this.grandpaSpeechTimer >= 6000)
          {
            this.mouseActive = true;
            Point center = this.clickableGrandpaLetterRect().Center;
            Game1.setMousePositionRaw((int) ((double) center.X * (double) Game1.options.zoomLevel), (int) ((double) center.Y * (double) Game1.options.zoomLevel));
          }
          if (this.clickedLetter)
          {
            int letterOpenTimer = this.letterOpenTimer;
            elapsedGameTime = time.ElapsedGameTime;
            int milliseconds11 = elapsedGameTime.Milliseconds;
            this.letterOpenTimer = letterOpenTimer + milliseconds11;
            break;
          }
          break;
      }
      Game1.player.updateEmote(time);
      if ((double) Game1.player.jitterStrength > 0.0)
        Game1.player.jitter = new Vector2((float) Game1.random.Next(-(int) ((double) Game1.player.jitterStrength * 100.0), (int) (((double) Game1.player.jitterStrength + 1.0) * 100.0)) / 100f, (float) Game1.random.Next(-(int) ((double) Game1.player.jitterStrength * 100.0), (int) (((double) Game1.player.jitterStrength + 1.0) * 100.0)) / 100f);
      return false;
    }

    public void afterFade()
    {
    }

    private Rectangle clickableGrandpaLetterRect() => new Rectangle((int) Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730).X + (286 - this.parallaxPan) * 4, (int) Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730).Y + 218 + Math.Max(0, Math.Min(60, (this.grandpaSpeechTimer - 5000) / 8)), 524, 344);

    public void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (!this.clickedLetter && this.mouseActive && (this.clickableGrandpaLetterRect().Contains(x, y) || Game1.options.SnappyMenus))
      {
        this.clickedLetter = true;
        Game1.playSound("newRecipe");
        Game1.changeMusicTrack("musicboxsong");
        this.letterView = new LetterViewerMenu(Game1.player.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12051", (object) Game1.player.Name, (object) Game1.player.farmName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12055", (object) Game1.player.Name, (object) Game1.player.farmName));
        this.letterView.exitFunction = new IClickableMenu.onExit(this.onLetterExit);
      }
      if (this.letterView == null)
        return;
      this.letterView.receiveLeftClick(x, y, true);
    }

    public void onLetterExit()
    {
      this.mouseActive = false;
      this.foregroundFadeChange = 0.0003f;
      this.fadingToQuit = true;
      if (this.letterView != null)
      {
        this.letterView.unload();
        this.letterView = (LetterViewerMenu) null;
      }
      Game1.playSound("newRecipe");
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
      if (k == Keys.Escape || Game1.options.doesInputListContain(Game1.options.menuButton, k))
      {
        if (!this.quit && !this.fadingToQuit)
          Game1.playSound("bigDeSelect");
        if (this.letterView != null)
        {
          this.letterView.unload();
          this.letterView = (LetterViewerMenu) null;
        }
        this.quit = true;
      }
      else
      {
        if (this.letterView == null)
          return;
        this.letterView.receiveKeyPress(k);
        if (Game1.input.GetGamePadState().IsButtonDown(Buttons.RightTrigger) && !Game1.oldPadState.IsButtonDown(Buttons.RightTrigger))
          this.letterView.receiveGamePadButton(Buttons.RightTrigger);
        if (!Game1.input.GetGamePadState().IsButtonDown(Buttons.LeftTrigger) || !Game1.oldPadState.IsButtonUp(Buttons.LeftTrigger))
          return;
        this.letterView.receiveGamePadButton(Buttons.LeftTrigger);
      }
    }

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public void receiveKeyRelease(Keys k)
    {
    }

    public void draw(SpriteBatch b)
    {
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), new Color(64, 136, 248));
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * this.backgroundFade);
      if (this.drawGrandpa)
      {
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730), new Rectangle?(new Rectangle(427, this.totalMilliseconds % 300 < 150 ? 240 : 0, 427, 240)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2(317f, 74f) * 3f, new Rectangle?(new Rectangle(427 + 74 * (this.totalMilliseconds % 400 / 100), 480, 74, 42)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2(320f, 75f) * 3f, new Rectangle?(new Rectangle(427, 522, 70, 32)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
        if (this.grandpaSpeechTimer > 8000 && this.grandpaSpeechTimer % 10000 < 5000)
          b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2(189f, 69f) * 3f, new Rectangle?(new Rectangle(497 + 18 * (this.totalMilliseconds % 400 / 200), 523, 18, 18)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
        if (this.grandpaSpeech.Count > 0 && this.grandpaSpeechTimer > 3000)
        {
          float scale = 1f;
          string text = this.grandpaSpeech.Peek();
          Vector2 vector2 = Game1.dialogueFont.MeasureString(text) * scale;
          float num = 3f * scale;
          Vector2 position = new Vector2((float) (Game1.viewport.Width / 2) - vector2.X / 2f, (float) ((int) Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730).Y + 669) + 3f);
          position.X -= num;
          b.DrawString(Game1.dialogueFont, text, position, Color.White * 0.25f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
          position.X += num;
          b.DrawString(Game1.dialogueFont, text, position, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        }
        if (this.letterReceived)
        {
          b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2(157f, 113f) * 3f, new Rectangle?(new Rectangle(463, 556, 37, 17)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
          if (this.grandpaSpeechTimer > 8000 && this.grandpaSpeechTimer % 10000 > 7000 && this.grandpaSpeechTimer % 10000 < 9000 && this.totalMilliseconds % 600 < 300)
            b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2(157f, 113f) * 3f, new Rectangle?(new Rectangle(500, 556, 37, 17)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
          b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + this.letterPosition, new Rectangle?(new Rectangle(729, 524, 131, 63)), Color.White, 0.0f, Vector2.Zero, this.letterScale, SpriteEffects.None, 1f);
        }
      }
      else if (this.scene == 3)
        SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12059"), (int) Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 0, 0, -200).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 0, 0, yOffset: -50).Y, 999, height: 999, layerDepth: 1f, color: 4);
      else if (this.scene == 4)
      {
        float num = (float) (1.0 - ((double) this.grandpaSpeechTimer - 7000.0) / 2000.0);
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730), new Rectangle?(new Rectangle(0, 0, 427, 240)), Color.White * num, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2(22f, 211f) * 3f, new Rectangle?(new Rectangle(264 + this.totalMilliseconds % 500 / 250 * 19, 581, 19, 17)), Color.White * num, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2(332f, 215f) * 3f, new Rectangle?(new Rectangle(305 + this.totalMilliseconds % 600 / 200 * 12, 581, 12, 12)), Color.White * num, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2(414f, 211f) * 3f, new Rectangle?(new Rectangle(460 + this.totalMilliseconds % 400 / 200 * 13, 581, 13, 17)), Color.White * num, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2(189f, 81f) * 3f, new Rectangle?(new Rectangle(426 + this.totalMilliseconds % 800 / 400 * 16, 581, 16, 16)), Color.White * num, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
      }
      if (this.scene == 4 && this.grandpaSpeechTimer >= 5000 || this.scene == 5)
      {
        b.Draw(this.texture, new Vector2(this.panX, (float) (Game1.viewport.Height / 2 - 360)), new Rectangle?(new Rectangle(0, 600, 1200, 180)), Color.White * (this.scene == 5 ? 1f : (float) (((double) this.grandpaSpeechTimer - 7000.0) / 2000.0)), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(this.texture, new Vector2(this.panX, (float) (Game1.viewport.Height / 2 - 360)) + new Vector2(1080f, 524f), new Rectangle?(new Rectangle(350 + this.totalMilliseconds % 800 / 400 * 14, 581, 14, 9)), Color.White * (this.scene == 5 ? 1f : (float) (((double) this.grandpaSpeechTimer - 7000.0) / 2000.0)), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(this.texture, new Vector2(this.panX, (float) (Game1.viewport.Height / 2 - 360)) + new Vector2(1564f, 520f), new Rectangle?(new Rectangle(383 + this.totalMilliseconds % 400 / 200 * 9, 581, 9, 7)), Color.White * (this.scene == 5 ? 1f : (float) (((double) this.grandpaSpeechTimer - 7000.0) / 2000.0)), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(this.texture, new Vector2(this.panX, (float) (Game1.viewport.Height / 2 - 360)) + new Vector2(2632f, 520f), new Rectangle?(new Rectangle(403 + this.totalMilliseconds % 600 / 300 * 8, 582, 8, 8)), Color.White * (this.scene == 5 ? 1f : (float) (((double) this.grandpaSpeechTimer - 7000.0) / 2000.0)), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(this.texture, new Vector2(this.panX, (float) (Game1.viewport.Height / 2 - 360)) + new Vector2(2604f, 504f), new Rectangle?(new Rectangle(364 + this.totalMilliseconds % 1100 / 100 * 5, 594, 5, 3)), Color.White * (this.scene == 5 ? 1f : (float) (((double) this.grandpaSpeechTimer - 7000.0) / 2000.0)), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(this.texture, new Vector2(this.panX, (float) (Game1.viewport.Height / 2 - 360)) + new Vector2(3116f, 492f), new Rectangle?(new Rectangle(343 + this.totalMilliseconds % 3000 / 1000 * 6, 593, 6, 5)), Color.White * (this.scene == 5 ? 1f : (float) (((double) this.grandpaSpeechTimer - 7000.0) / 2000.0)), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        if (this.scene == 5)
          Game1.player.draw(b);
        b.Draw(this.texture, new Vector2(this.panX, (float) (Game1.viewport.Height / 2 - 360)) + new Vector2(3580f, 540f), new Rectangle?(new Rectangle(895, 735, 29, 36)), Color.White * (this.scene == 5 ? 1f : (float) (((double) this.grandpaSpeechTimer - 7000.0) / 2000.0)), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      if (this.scene == 6)
      {
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2((float) (261 - this.parallaxPan), 145f) * 4f, new Rectangle?(new Rectangle(550, 540, 56 + this.parallaxPan, 35)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2((float) (261 - this.parallaxPan), 4f + (float) Math.Max(0, Math.Min(60, (this.grandpaSpeechTimer - 5000) / 8))) * 4f, new Rectangle?(new Rectangle(264, 434, 56 + this.parallaxPan, 141)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        if (this.grandpaSpeechTimer > 3000)
          b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2((float) (286 - this.parallaxPan), 32f + (float) Math.Max(0, Math.Min(60, (this.grandpaSpeechTimer - 5000) / 8)) + Math.Min(30f, (float) this.letterOpenTimer / 4f)) * 4f, new Rectangle?(new Rectangle(729 + Math.Min(2, this.letterOpenTimer / 200) * 131, 508, 131, 79)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730), new Rectangle?(new Rectangle(this.parallaxPan, 240, 320, 180)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(this.texture, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730) + new Vector2((float) (187.0 - (double) this.parallaxPan * 2.5), 8f) * 4f, new Rectangle?(new Rectangle(20, 428, 232, 172)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      b.End();
      Game1.PushUIMode();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if (this.letterView != null)
        this.letterView.draw(b);
      if (this.mouseActive)
        b.Draw(Game1.mouseCursors, new Vector2((float) Game1.getOldMouseX(), (float) Game1.getOldMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float) (4.0 + (double) Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
      b.End();
      Game1.PopUIMode();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), this.fadingToQuit ? new Color(64, 136, 248) * this.foregroundFade : Color.Black * this.foregroundFade);
      b.End();
    }

    public void changeScreenSize()
    {
      Game1.viewport.X = 0;
      Game1.viewport.Y = 0;
    }

    public void unload()
    {
      this.content.Unload();
      this.content = (LocalizedContentManager) null;
    }

    public void receiveEventPoke(int data)
    {
    }

    public string minigameId() => (string) null;

    public bool doMainGameUpdates() => false;

    public bool forceQuit() => false;
  }
}
