// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.TargetGame
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewValley.Minigames
{
  [InstanceStatics]
  public class TargetGame : IMinigame
  {
    private GameLocation location;
    private int timerToStart = 1000;
    private int gameEndTimer = 61000;
    private int showResultsTimer = -1;
    private bool gameDone;
    private bool exit;
    public static int score;
    public static int shotsFired;
    public static int successShots;
    public static int accuracy = -1;
    public static int starTokensWon;
    public List<TargetGame.Target> targets;
    private float modifierBonus;

    public TargetGame()
    {
      TargetGame.score = 0;
      TargetGame.successShots = 0;
      TargetGame.shotsFired = 0;
      this.location = new GameLocation("Maps\\TargetGame", "tent");
      Slingshot slingshot = new Slingshot();
      slingshot.attachments[0] = new StardewValley.Object(390, 999);
      Game1.player.TemporaryItem = (Item) slingshot;
      Game1.player.CurrentToolIndex = 0;
      Game1.globalFadeToClear(fadeSpeed: 0.01f);
      this.location.Map.LoadTileSheets(Game1.mapDisplayDevice);
      Game1.player.Position = new Vector2(8f, 13f) * 64f;
      this.changeScreenSize();
      this.gameEndTimer = 50000;
      this.targets = new List<TargetGame.Target>();
      this.addTargets();
    }

    public bool overrideFreeMouseMovement() => false;

    public bool tick(GameTime time)
    {
      this.location.UpdateWhenCurrentLocation(time);
      this.location.wasUpdated = false;
      this.location.updateEvenIfFarmerIsntHere(time);
      Game1.player.Stamina = (float) Game1.player.MaxStamina;
      Game1.player.Update(time, this.location);
      if ((Game1.oldKBState.GetPressedKeys().Length == 0 || Game1.oldKBState.GetPressedKeys().Length == 1 && Game1.options.doesInputListContain(Game1.options.runButton, Game1.oldKBState.GetPressedKeys()[0]) || !Game1.player.movedDuringLastTick()) && !Game1.player.UsingTool)
        Game1.player.Halt();
      if (this.timerToStart > 0)
      {
        this.timerToStart -= time.ElapsedGameTime.Milliseconds;
        if (this.timerToStart <= 0)
        {
          Game1.playSound("whistle");
          Game1.changeMusicTrack("tickTock", music_context: Game1.MusicContext.MiniGame);
        }
      }
      else if (this.showResultsTimer >= 0)
      {
        int showResultsTimer = this.showResultsTimer;
        this.showResultsTimer -= time.ElapsedGameTime.Milliseconds;
        if (showResultsTimer > 16000 && this.showResultsTimer <= 16000)
          Game1.playSound("smallSelect");
        if (showResultsTimer > 14000 && this.showResultsTimer <= 14000)
        {
          Game1.playSound("smallSelect");
          TargetGame.accuracy = (int) Math.Max(0.0, Math.Round((double) ((float) TargetGame.successShots / (float) (TargetGame.shotsFired - 1)), 2) * 100.0);
        }
        if (showResultsTimer > 11000 && this.showResultsTimer <= 11000)
        {
          if (TargetGame.accuracy >= 75)
          {
            Game1.playSound("newArtifact");
            float num = 1.5f;
            if (TargetGame.accuracy >= 85)
              num = 2f;
            if (TargetGame.accuracy >= 90)
              num = 2.5f;
            if (TargetGame.accuracy >= 95)
              num = 3f;
            if (TargetGame.accuracy >= 100)
              num = 4f;
            TargetGame.score = (int) ((double) TargetGame.score * (double) num);
            this.modifierBonus = num;
          }
          else
            Game1.playSound("smallSelect");
        }
        if (showResultsTimer > 9000 && this.showResultsTimer <= 9000)
        {
          if (TargetGame.score >= 40)
          {
            Game1.playSound("reward");
            TargetGame.starTokensWon = (int) ((double) ((TargetGame.score * 2 - 30) / 10) * 2.5);
            TargetGame.starTokensWon *= 2;
            if (TargetGame.starTokensWon > 280)
              TargetGame.starTokensWon = 500;
            Game1.player.festivalScore += TargetGame.starTokensWon;
          }
          else
            Game1.playSound("fishEscape");
        }
        if (this.showResultsTimer <= 0)
        {
          Game1.globalFadeToClear();
          Game1.player.Position = new Vector2(24f, 63f) * 64f;
          return true;
        }
      }
      else if (!this.gameDone)
      {
        this.gameEndTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.gameEndTimer <= 0)
        {
          Game1.playSound("whistle");
          this.gameEndTimer = 1000;
          Game1.player.completelyStopAnimatingOrDoingAction();
          Game1.player.canMove = false;
          this.gameDone = true;
        }
        for (int index = this.targets.Count - 1; index >= 0; --index)
        {
          if (this.targets[index].update(time, this.location))
            this.targets.RemoveAt(index);
        }
      }
      else if (this.gameDone && this.gameEndTimer > 0)
      {
        this.gameEndTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.gameEndTimer <= 0)
        {
          Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.gameDoneAfterFade), 0.01f);
          Game1.player.forceCanMove();
        }
      }
      return this.exit;
    }

    public void gameDoneAfterFade()
    {
      this.showResultsTimer = 16100;
      Game1.player.canMove = false;
      Game1.player.freezePause = 16100;
      Game1.player.Position = new Vector2(24f, 63f) * 64f;
      Game1.player.TemporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle(1536, 4032, 64, 64));
      Game1.player.faceDirection(2);
    }

    public void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (this.showResultsTimer < 0)
        Game1.pressUseToolButton();
      else if (this.showResultsTimer > 16000)
        this.showResultsTimer = 16001;
      else if (this.showResultsTimer > 14000)
        this.showResultsTimer = 14001;
      else if (this.showResultsTimer > 11000)
        this.showResultsTimer = 11001;
      else if (this.showResultsTimer > 9000)
      {
        this.showResultsTimer = 9001;
      }
      else
      {
        if (this.showResultsTimer >= 9000 || this.showResultsTimer <= 1000)
          return;
        this.showResultsTimer = 1500;
        Game1.player.freezePause = 1500;
        Game1.playSound("smallSelect");
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
      int count = this.location.projectiles.Count;
      if (this.showResultsTimer >= 0 || Game1.player.CurrentTool == null || !Game1.player.UsingTool || !Game1.player.CurrentTool.onRelease(this.location, x, y, Game1.player))
        return;
      Game1.player.usingSlingshot = false;
      Game1.player.canReleaseTool = true;
      Game1.player.UsingTool = false;
      Game1.player.CanMove = true;
      if (this.location.projectiles.Count <= count)
        return;
      ++TargetGame.shotsFired;
    }

    public void releaseRightClick(int x, int y)
    {
    }

    public void receiveKeyPress(Keys k)
    {
      if (Game1.options.doesInputListContain(Game1.options.menuButton, k))
      {
        Game1.playSound("fishEscape");
        this.showResultsTimer = 1;
      }
      if (this.showResultsTimer > 0 || this.gameEndTimer > 0)
      {
        Game1.player.Halt();
      }
      else
      {
        if (Game1.player.movementDirections.Count < 2 && !Game1.player.UsingTool && this.timerToStart <= 0)
        {
          if (Game1.options.doesInputListContain(Game1.options.moveUpButton, k))
            Game1.player.setMoving((byte) 1);
          if (Game1.options.doesInputListContain(Game1.options.moveRightButton, k))
            Game1.player.setMoving((byte) 2);
          if (Game1.options.doesInputListContain(Game1.options.moveDownButton, k))
            Game1.player.setMoving((byte) 4);
          if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, k))
            Game1.player.setMoving((byte) 8);
        }
        if (!Game1.options.doesInputListContain(Game1.options.runButton, k))
          return;
        Game1.player.setRunning(true);
      }
    }

    public void receiveKeyRelease(Keys k)
    {
      if (Game1.options.doesInputListContain(Game1.options.moveUpButton, k))
        Game1.player.setMoving((byte) 33);
      if (Game1.options.doesInputListContain(Game1.options.moveRightButton, k))
        Game1.player.setMoving((byte) 34);
      if (Game1.options.doesInputListContain(Game1.options.moveDownButton, k))
        Game1.player.setMoving((byte) 36);
      if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, k))
        Game1.player.setMoving((byte) 40);
      if (!Game1.options.doesInputListContain(Game1.options.runButton, k))
        return;
      Game1.player.setRunning(false);
    }

    public void draw(SpriteBatch b)
    {
      if (this.showResultsTimer < 0)
      {
        b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        Game1.mapDisplayDevice.BeginScene(b);
        this.location.Map.GetLayer("Back").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        SpriteBatch spriteBatch = b;
        Texture2D shadowTexture = Game1.shadowTexture;
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, Game1.player.Position + new Vector2(32f, 24f));
        Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
        Color white = Color.White;
        Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
        double x = (double) bounds.Center.X;
        bounds = Game1.shadowTexture.Bounds;
        double y = (double) bounds.Center.Y;
        Vector2 origin = new Vector2((float) x, (float) y);
        double scale = 4.0 - (Game1.player.running || Game1.player.UsingTool ? (double) Math.Abs(FarmerRenderer.featureYOffsetPerFrame[Game1.player.FarmerSprite.CurrentFrame]) * 0.800000011920929 : 0.0);
        double layerDepth = (double) Math.Max(0.0f, (float) ((double) Game1.player.getStandingY() / 10000.0 + 0.000110000000859145)) - 1.0000000116861E-07;
        spriteBatch.Draw(shadowTexture, local, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
        this.location.Map.GetLayer("Buildings").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        Game1.mapDisplayDevice.EndScene();
        b.End();
        b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
        this.location.draw(b);
        Game1.player.draw(b);
        foreach (TargetGame.Target target in this.targets)
          target.draw(b);
        b.End();
        b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        Game1.mapDisplayDevice.BeginScene(b);
        this.location.Map.GetLayer("Front").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
        Game1.mapDisplayDevice.EndScene();
        this.location.drawAboveAlwaysFrontLayer(b);
        Game1.player.CurrentTool.draw(b);
        Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.10444", (object) TargetGame.score), Color.Black, Color.White, new Vector2(32f, 32f));
        Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1514", (object) (this.gameEndTimer / 1000)), Color.Black, Color.White, new Vector2(32f, 64f));
        if (TargetGame.shotsFired > 1)
          Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:TargetGame.cs.12154", (object) (int) (Math.Round((double) ((float) TargetGame.successShots / (float) (TargetGame.shotsFired - 1)), 2) * 100.0)), Color.Black, Color.White, new Vector2(32f, 96f));
        b.End();
      }
      else
      {
        b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        Vector2 position = new Vector2((float) (Game1.viewport.Width / 2 - 128), (float) (Game1.viewport.Height / 2 - 64));
        if (this.showResultsTimer <= 16000)
          Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.10444", (object) TargetGame.score), Game1.textColor, this.showResultsTimer > 11000 || (double) this.modifierBonus <= 1.0 ? Color.White : Color.Lime, position);
        if (this.showResultsTimer <= 14000)
        {
          position.Y += 48f;
          Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:TargetGame.cs.12157", (object) TargetGame.accuracy, (object) TargetGame.successShots, (object) TargetGame.shotsFired), Game1.textColor, Color.White, position);
        }
        if (this.showResultsTimer <= 11000)
        {
          position.Y += 48f;
          if ((double) this.modifierBonus > 1.0)
            Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:TargetGame.cs.12161", (object) this.modifierBonus), Game1.textColor, Color.Yellow, position);
          else
            Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:TargetGame.cs.12163"), Game1.textColor, Color.Red, position);
        }
        if (this.showResultsTimer <= 9000)
        {
          position.Y += 64f;
          if (TargetGame.starTokensWon > 0)
          {
            float num = Math.Min(1f, (float) (this.showResultsTimer - 2000) / 4000f);
            Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12013", (object) TargetGame.starTokensWon), Game1.textColor * 0.2f * num, Color.SkyBlue * 0.3f * num, position + new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) * 4f * 2f, 0.0f, 1f, 1f);
            Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12013", (object) TargetGame.starTokensWon), Game1.textColor * 0.2f * num, Color.SkyBlue * 0.3f * num, position + new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) * 4f * 2f, 0.0f, 1f, 1f);
            Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12013", (object) TargetGame.starTokensWon), Game1.textColor * 0.2f * num, Color.SkyBlue * 0.3f * num, position + new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) * 4f * 2f, 0.0f, 1f, 1f);
            Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12013", (object) TargetGame.starTokensWon), Game1.textColor, Color.SkyBlue, position, 0.0f, 1f, 1f);
          }
          else
            Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.12021"), Game1.textColor, Color.Red, position);
        }
        if (this.showResultsTimer <= 1000)
          b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * (float) (1.0 - (double) this.showResultsTimer / 1000.0));
        b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(16, 16, 128 + (Game1.player.festivalScore > 999 ? 16 : 0), 64), Color.Black * 0.75f);
        b.Draw(Game1.mouseCursors, new Vector2(32f, 32f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(338, 400, 8, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        Game1.drawWithBorder(Game1.player.festivalScore.ToString() ?? "", Color.Black, Color.White, new Vector2(72f, 29f), 0.0f, 1f, 1f, false);
        b.End();
      }
    }

    public static void startMe()
    {
      Game1.currentMinigame = (IMinigame) new TargetGame();
      Game1.changeMusicTrack("none", music_context: Game1.MusicContext.MiniGame);
    }

    public void changeScreenSize()
    {
      Game1.viewport.X = this.location.Map.Layers[0].LayerWidth * 64 / 2 - Game1.viewport.Width / 2;
      Game1.viewport.Y = this.location.Map.Layers[0].LayerHeight * 64 / 2 - Game1.viewport.Height / 2;
    }

    public void unload()
    {
      Game1.player.TemporaryItem = (Item) null;
      Game1.currentLocation.Map.LoadTileSheets(Game1.mapDisplayDevice);
      Game1.player.forceCanMove();
      Game1.stopMusicTrack(Game1.MusicContext.MiniGame);
    }

    public void addTargets()
    {
      this.addRowOfTargetsOnLane(0, TargetGame.Target.middleLane, 1500, 5, TargetGame.Target.mediumSpeed, false);
      this.addRowOfTargetsOnLane(4000, TargetGame.Target.nearLane, 1000, 5, TargetGame.Target.mediumSpeed);
      this.addRowOfTargetsOnLane(8000, TargetGame.Target.farLane, 2000, 5, TargetGame.Target.mediumSpeed, false, TargetGame.Target.bonusTarget);
      this.addTwinPausers(8000, TargetGame.Target.superNearLane, TargetGame.Target.pauseMiddleLeft, TargetGame.Target.fastSpeed, 2000, TargetGame.Target.bonusTarget);
      this.addTwinPausers(15000, TargetGame.Target.superNearLane, TargetGame.Target.pauseFarLeft, TargetGame.Target.mediumSpeed, 4000, TargetGame.Target.bonusTarget);
      this.addRowOfTargetsOnLane(18000, TargetGame.Target.middleLane, 1500, 5, TargetGame.Target.mediumSpeed, false);
      this.addRowOfTargetsOnLane(21000, TargetGame.Target.nearLane, 1000, 5, TargetGame.Target.mediumSpeed);
      this.addTwinPausers(25000, TargetGame.Target.behindLane, TargetGame.Target.pauseFarLeft, TargetGame.Target.fastSpeed, 1500, TargetGame.Target.deluxeTarget);
      this.addRowOfTargetsOnLane(27000, TargetGame.Target.superNearLane, 500, 8, TargetGame.Target.slowSpeed);
      this.addRowOfTargetsOnLane(28000, TargetGame.Target.nearLane, 500, 8, TargetGame.Target.slowSpeed);
      this.addRowOfTargetsOnLane(29000, TargetGame.Target.middleLane, 500, 8, TargetGame.Target.slowSpeed);
      this.addRowOfTargetsOnLane(30000, TargetGame.Target.farLane, 500, 8, TargetGame.Target.slowSpeed);
      this.addTwinPausers(36000, TargetGame.Target.behindLane, TargetGame.Target.pauseFarLeft, TargetGame.Target.fastSpeed, 2000, TargetGame.Target.deluxeTarget);
      this.addRowOfTargetsOnLane(41000, TargetGame.Target.middleLane, 1500, 5, TargetGame.Target.mediumSpeed, false);
      this.addRowOfTargetsOnLane(42000, TargetGame.Target.nearLane, 1000, 5, TargetGame.Target.mediumSpeed);
      this.addRowOfTargetsOnLane(43000, TargetGame.Target.farLane, 1000, 4, TargetGame.Target.mediumSpeed, false);
    }

    private void addTwinPausers(
      int initialDelay,
      int whichLane,
      int pauseArea,
      int speed,
      int pauseTime,
      int targetType)
    {
      int pauseAndReturn = -1;
      bool spawnFromRight = false;
      if (pauseArea == TargetGame.Target.pauseFarLeft)
      {
        pauseAndReturn = TargetGame.Target.pauseFarRight;
        spawnFromRight = true;
      }
      if (pauseArea == TargetGame.Target.pauseLeft)
      {
        pauseAndReturn = TargetGame.Target.pauseRight;
        spawnFromRight = true;
      }
      if (pauseArea == TargetGame.Target.pauseMiddleLeft)
      {
        pauseAndReturn = TargetGame.Target.pauseMiddleRight;
        spawnFromRight = true;
      }
      if (pauseArea == TargetGame.Target.pauseMiddleRight)
        pauseAndReturn = TargetGame.Target.pauseMiddleLeft;
      if (pauseArea == TargetGame.Target.pauseRight)
        pauseAndReturn = TargetGame.Target.pauseLeft;
      if (pauseArea == TargetGame.Target.pauseFarRight)
        pauseAndReturn = TargetGame.Target.pauseFarLeft;
      this.targets.Add(new TargetGame.Target(initialDelay, whichLane, targetType, speed, !spawnFromRight, pauseArea, pauseTime));
      this.targets.Add(new TargetGame.Target(initialDelay, whichLane, targetType, speed, spawnFromRight, pauseAndReturn, pauseTime));
    }

    private void addRowOfTargetsOnLane(
      int initialDelayBeforeStarting,
      int whichLane,
      int delayBetween,
      int numberOfTargets,
      int speed,
      bool spawnFromRight = true,
      int targetType = 0)
    {
      for (int index = 0; index < numberOfTargets; ++index)
        this.targets.Add(new TargetGame.Target(initialDelayBeforeStarting + index * delayBetween, whichLane, targetType, speed, spawnFromRight));
    }

    public void receiveEventPoke(int data)
    {
    }

    public string minigameId() => nameof (TargetGame);

    public bool doMainGameUpdates() => false;

    public bool forceQuit() => false;

    public class Target
    {
      public static int width = 56;
      public static int spawnRightPosition = 960;
      public static int spawnLeftPosition = 0;
      public static int basicTarget = 0;
      public static int bonusTarget = 1;
      public static int deluxeTarget = 2;
      public static int mediumSpeed = 4;
      public static int slowSpeed = 2;
      public static int fastSpeed = 5;
      public static int nearLane = 448;
      public static int middleLane = 320;
      public static int farLane = 128;
      public static int superNearLane = 576;
      public static int behindLane = 832;
      public static int pauseFarRight = 832;
      public static int pauseRight = 704;
      public static int pauseMiddleRight = 576;
      public static int pauseMiddleLeft = 384;
      public static int pauseLeft = 256;
      public static int pauseFarLeft = 128;
      public Microsoft.Xna.Framework.Rectangle Position;
      private int targetType;
      private int countdownBeforeSpawn;
      private int xPausePosition;
      private int xPauseTime;
      private int speed;
      private bool spawned;
      private bool atPausePosition;
      private Microsoft.Xna.Framework.Rectangle sourceRect;

      public Target(
        int countdownBeforeSpawn,
        int whichLane,
        int type = 0,
        int speed = 4,
        bool spawnFromRight = true,
        int pauseAndReturn = -1,
        int pauseTime = -1)
      {
        this.countdownBeforeSpawn = countdownBeforeSpawn;
        this.targetType = type;
        this.speed = speed * (spawnFromRight ? -1 : 1);
        this.Position = new Microsoft.Xna.Framework.Rectangle(spawnFromRight ? TargetGame.Target.spawnRightPosition : TargetGame.Target.spawnLeftPosition, whichLane, TargetGame.Target.width, TargetGame.Target.width);
        this.xPausePosition = pauseAndReturn;
        this.xPauseTime = pauseTime;
        this.sourceRect = new Microsoft.Xna.Framework.Rectangle(289, 1184 + type * 16, 14, 14);
      }

      public bool update(GameTime time, GameLocation location)
      {
        if (this.countdownBeforeSpawn > 0)
        {
          this.countdownBeforeSpawn -= time.ElapsedGameTime.Milliseconds;
          if (this.countdownBeforeSpawn <= 0)
            this.spawned = true;
        }
        if (!this.spawned)
          return false;
        if (this.atPausePosition)
        {
          this.xPauseTime -= time.ElapsedGameTime.Milliseconds;
          if (this.xPauseTime <= 0)
          {
            this.speed = -this.speed;
            this.atPausePosition = false;
            this.xPausePosition = -1;
          }
        }
        else
        {
          this.Position.X += this.speed;
          if (this.xPausePosition != -1 && Math.Abs(this.xPausePosition - this.Position.X) <= Math.Abs(this.speed))
            this.atPausePosition = true;
        }
        if (this.Position.X < 0 || this.Position.Right > TargetGame.Target.spawnRightPosition + 64)
          return true;
        bool projectileHit = false;
        location.projectiles.Filter((Func<Projectile, bool>) (projectile =>
        {
          if (projectile.getBoundingBox().Intersects(this.Position))
          {
            this.shatter(location, projectile);
            projectileHit = true;
            if (this.targetType != TargetGame.Target.basicTarget)
            {
              projectile.behaviorOnCollisionWithOther(location);
              return false;
            }
          }
          return true;
        }));
        return projectileHit;
      }

      public void shatter(GameLocation location, Projectile stone)
      {
        int number = 0;
        if (this.targetType == TargetGame.Target.basicTarget)
        {
          Game1.playSound("breakingGlass");
          ++number;
        }
        if (this.targetType == TargetGame.Target.bonusTarget)
        {
          Game1.playSound("potterySmash");
          number += 2;
        }
        if (this.targetType == TargetGame.Target.deluxeTarget)
        {
          Game1.playSound("potterySmash");
          number += 5;
        }
        location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(304, 1183 + this.targetType * 16, 16, 16), 60f, 3, 0, new Vector2((float) (this.Position.X - 4), (float) (this.Position.Y - 4)), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
        location.debris.Add(new Debris(number, new Vector2((float) this.Position.Center.X, (float) this.Position.Center.Y), new Color((int) byte.MaxValue, 130, 0), 1f, (Character) null));
        TargetGame.score += number;
        if (!(stone is BasicProjectile) || (int) (NetFieldBase<int, NetInt>) (stone as BasicProjectile).damageToFarmer <= 0)
          return;
        ++TargetGame.successShots;
        (stone as BasicProjectile).damageToFarmer.Value = -1;
      }

      public void draw(SpriteBatch b)
      {
        if (!this.spawned)
          return;
        b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) this.Position.X, (float) (this.Position.Bottom + 32))), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.Position), new Microsoft.Xna.Framework.Rectangle?(this.sourceRect), Color.White);
      }
    }
  }
}
