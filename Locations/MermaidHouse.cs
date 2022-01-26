// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.MermaidHouse
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class MermaidHouse : GameLocation
  {
    private Texture2D mermaidSprites;
    private float showTimer;
    private float curtainMovement;
    private float curtainOpenPercent;
    private float blackBGAlpha;
    private float bigMermaidAlpha;
    private float oldStopWatchTime;
    private float finalLeftMermaidAlpha;
    private float finalRightMermaidAlpha;
    private float finalBigMermaidAlpha;
    private float fairyTimer;
    private int[] mermaidFrames;
    private Stopwatch stopWatch;
    private List<Vector2> bubbles;
    private List<TemporaryAnimatedSprite> sparkles;
    private List<TemporaryAnimatedSprite> alwaysFrontTempSprites;
    private List<int> lastFiveClamTones;
    private Farmer pearlRecipient;

    public MermaidHouse()
    {
    }

    public MermaidHouse(string mapPath, string name)
      : base(mapPath, name)
    {
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.mermaidSprites = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
      Game1.ambientLight = Color.White;
      Game1.changeMusicTrack("none");
      this.finalLeftMermaidAlpha = 0.0f;
      this.finalRightMermaidAlpha = 0.0f;
      this.finalBigMermaidAlpha = 0.0f;
      this.blackBGAlpha = 0.0f;
      this.bigMermaidAlpha = 0.0f;
      this.oldStopWatchTime = 0.0f;
      this.showTimer = 0.0f;
      this.curtainMovement = 0.0f;
      this.curtainOpenPercent = 0.0f;
      this.fairyTimer = 0.0f;
      this.stopWatch = new Stopwatch();
      this.bubbles = new List<Vector2>();
      this.sparkles = new List<TemporaryAnimatedSprite>();
      this.alwaysFrontTempSprites = new List<TemporaryAnimatedSprite>();
      this.lastFiveClamTones = new List<int>();
      this.pearlRecipient = (Farmer) null;
      this.mermaidFrames = new int[93]
      {
        1,
        0,
        2,
        0,
        1,
        0,
        2,
        0,
        3,
        3,
        3,
        4,
        3,
        3,
        3,
        4,
        3,
        3,
        3,
        4,
        3,
        3,
        3,
        4,
        3,
        3,
        3,
        4,
        3,
        3,
        4,
        4,
        3,
        3,
        3,
        3,
        0,
        0,
        0,
        0,
        3,
        3,
        3,
        4,
        3,
        3,
        3,
        4,
        3,
        3,
        3,
        4,
        3,
        3,
        3,
        4,
        3,
        3,
        3,
        4,
        3,
        3,
        4,
        4,
        3,
        3,
        3,
        3,
        0,
        0,
        0,
        0,
        3,
        3,
        3,
        3,
        4,
        4,
        4,
        4,
        3,
        3,
        3,
        3,
        0,
        0,
        5,
        6,
        5,
        6,
        7,
        8,
        8
      };
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.map.GetLayer("Buildings").Tiles[tileLocation] != null)
      {
        switch (this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex)
        {
          case 56:
            this.playClamTone(0, who);
            return true;
          case 57:
            this.playClamTone(1, who);
            return true;
          case 58:
            this.playClamTone(2, who);
            return true;
          case 59:
            this.playClamTone(3, who);
            return true;
          case 60:
            this.playClamTone(4, who);
            return true;
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public void playClamTone(int which) => this.playClamTone(which, (Farmer) null);

    public void playClamTone(int which, Farmer who)
    {
      if ((double) this.oldStopWatchTime < 68000.0)
        return;
      ICue cue = Game1.soundBank.GetCue("clam_tone");
      switch (which)
      {
        case 0:
          cue.SetVariable("Pitch", 300);
          this.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = this.mermaidSprites,
            color = Color.HotPink,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(125, 126, 11, 12),
            scale = 4f,
            position = new Vector2(35f, 98f) * 4f,
            interval = 1000f,
            animationLength = 1,
            alphaFade = 0.03f,
            layerDepth = 0.0001f
          });
          break;
        case 1:
          cue.SetVariable("Pitch", 600);
          this.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = this.mermaidSprites,
            color = Color.Orange,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(125, 126, 11, 12),
            scale = 4f,
            position = new Vector2(51f, 98f) * 4f,
            interval = 1000f,
            animationLength = 1,
            alphaFade = 0.03f,
            layerDepth = 0.0001f
          });
          break;
        case 2:
          cue.SetVariable("Pitch", 800);
          this.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = this.mermaidSprites,
            color = Color.Yellow,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(125, 126, 11, 12),
            scale = 4f,
            position = new Vector2(67f, 98f) * 4f,
            interval = 1000f,
            animationLength = 1,
            alphaFade = 0.03f,
            layerDepth = 0.0001f
          });
          break;
        case 3:
          cue.SetVariable("Pitch", 1000);
          this.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = this.mermaidSprites,
            color = Color.Cyan,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(125, 126, 11, 12),
            scale = 4f,
            position = new Vector2(83f, 98f) * 4f,
            interval = 1000f,
            animationLength = 1,
            alphaFade = 0.03f,
            layerDepth = 0.0001f
          });
          break;
        case 4:
          cue.SetVariable("Pitch", 1200);
          this.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = this.mermaidSprites,
            color = Color.Lime,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(125, 126, 11, 12),
            scale = 4f,
            position = new Vector2(99f, 98f) * 4f,
            interval = 1000f,
            animationLength = 1,
            alphaFade = 0.03f,
            layerDepth = 0.0001f
          });
          break;
      }
      cue.Play();
      this.lastFiveClamTones.Add(which);
      if (this.lastFiveClamTones.Count > 5)
        this.lastFiveClamTones.RemoveAt(0);
      if (this.lastFiveClamTones.Count != 5 || this.lastFiveClamTones[0] != 0 || this.lastFiveClamTones[1] != 4 || this.lastFiveClamTones[2] != 3 || this.lastFiveClamTones[3] != 1 || this.lastFiveClamTones[4] != 2 || who == null || who.mailReceived.Contains("gotPearl"))
        return;
      who.freezePause = 4500;
      this.fairyTimer = 3500f;
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        interval = 1f,
        delayBeforeAnimationStart = 885,
        texture = this.mermaidSprites,
        endFunction = new TemporaryAnimatedSprite.endBehavior(this.playClamTone),
        extraInfoForEndBehavior = 0
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        interval = 1f,
        delayBeforeAnimationStart = 1270,
        texture = this.mermaidSprites,
        endFunction = new TemporaryAnimatedSprite.endBehavior(this.playClamTone),
        extraInfoForEndBehavior = 4
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        interval = 1f,
        delayBeforeAnimationStart = 1655,
        texture = this.mermaidSprites,
        endFunction = new TemporaryAnimatedSprite.endBehavior(this.playClamTone),
        extraInfoForEndBehavior = 3
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        interval = 1f,
        delayBeforeAnimationStart = 2040,
        texture = this.mermaidSprites,
        endFunction = new TemporaryAnimatedSprite.endBehavior(this.playClamTone),
        extraInfoForEndBehavior = 1
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        interval = 1f,
        delayBeforeAnimationStart = 2425,
        texture = this.mermaidSprites,
        endFunction = new TemporaryAnimatedSprite.endBehavior(this.playClamTone),
        extraInfoForEndBehavior = 2
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.mermaidSprites,
        delayBeforeAnimationStart = 885,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(2, (int) sbyte.MaxValue, 19, 18),
        sourceRectStartingPos = new Vector2(2f, (float) sbyte.MaxValue),
        scale = 4f,
        position = new Vector2(28f, 49f) * 4f,
        interval = 96f,
        animationLength = 4,
        totalNumberOfLoops = 121
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.mermaidSprites,
        delayBeforeAnimationStart = 1270,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(2, (int) sbyte.MaxValue, 19, 18),
        sourceRectStartingPos = new Vector2(2f, (float) sbyte.MaxValue),
        scale = 4f,
        position = new Vector2(108f, 49f) * 4f,
        interval = 96f,
        animationLength = 4,
        totalNumberOfLoops = 117
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.mermaidSprites,
        delayBeforeAnimationStart = 1655,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(2, (int) sbyte.MaxValue, 19, 18),
        sourceRectStartingPos = new Vector2(2f, (float) sbyte.MaxValue),
        scale = 4f,
        position = new Vector2(88f, 39f) * 4f,
        interval = 96f,
        animationLength = 4,
        totalNumberOfLoops = 113
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.mermaidSprites,
        delayBeforeAnimationStart = 2040,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(2, (int) sbyte.MaxValue, 19, 18),
        sourceRectStartingPos = new Vector2(2f, (float) sbyte.MaxValue),
        scale = 4f,
        position = new Vector2(48f, 39f) * 4f,
        interval = 96f,
        animationLength = 4,
        totalNumberOfLoops = 19
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.mermaidSprites,
        delayBeforeAnimationStart = 2425,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(2, (int) sbyte.MaxValue, 19, 18),
        sourceRectStartingPos = new Vector2(2f, (float) sbyte.MaxValue),
        scale = 4f,
        position = new Vector2(68f, 29f) * 4f,
        interval = 96f,
        animationLength = 4,
        totalNumberOfLoops = 15
      });
      this.pearlRecipient = who;
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      foreach (TemporaryAnimatedSprite sparkle in this.sparkles)
        sparkle.draw(b, true);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(58f, 54f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.mermaidFrames[Math.Min((int) ((double) this.stopWatch.ElapsedMilliseconds / 769.230773925781), this.mermaidFrames.Length - 1)] * 28, 80, 28, 36)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0009f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(27f, 29f) * 4f + new Vector2((float) (Math.Sin((double) this.stopWatch.ElapsedMilliseconds / 1000.0) * 4.0 * 4.0), (float) (Math.Cos((double) this.stopWatch.ElapsedMilliseconds / 1000.0) * 4.0 * 4.0))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(2 + (int) ((double) this.showTimer % 400.0 / 100.0) * 19, (int) sbyte.MaxValue, 19, 18)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0009f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(97f, 29f) * 4f + new Vector2((float) (Math.Cos((double) this.stopWatch.ElapsedMilliseconds / 1000.0 + 0.100000001490116) * 4.0 * 4.0), (float) (Math.Sin((double) this.stopWatch.ElapsedMilliseconds / 1000.0 + 0.100000001490116) * 4.0 * 4.0))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(2 + (int) ((double) this.showTimer % 400.0 / 100.0) * 19, (int) sbyte.MaxValue, 19, 18)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0009f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(16f, 16f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int) (144.0 + 57.0 * (double) this.curtainOpenPercent), 119, (int) (57.0 * (1.0 - (double) this.curtainOpenPercent)), 81)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2((float) (73.0 + 57.0 * (double) this.curtainOpenPercent), 16f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(200, 119, (int) (57.0 * (1.0 - (double) this.curtainOpenPercent)), 81)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      base.drawAboveAlwaysFrontLayer(b);
      b.Draw(Game1.staminaRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * this.blackBGAlpha);
      int num = Game1.graphics.GraphicsDevice.Viewport.Bounds.Height / 4;
      for (int index = -448; index < Game1.graphics.GraphicsDevice.Viewport.Width + 448; index += 448)
      {
        b.Draw(this.mermaidSprites, new Vector2((float) (index - (int) ((double) this.stopWatch.ElapsedMilliseconds / 6.0 % 448.0)), (float) (num - num * 3 / 4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(144, 32, 112, 48)), Color.Lime * this.blackBGAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
        b.Draw(this.mermaidSprites, new Vector2((float) (index + 112) - (float) ((double) this.stopWatch.ElapsedMilliseconds / 6.0 % 448.0), (float) ((double) num - (double) num / 4.0 + Math.Sin((double) this.stopWatch.ElapsedMilliseconds / 1000.0) * 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(177, 0, 16, 16)), Color.White * this.blackBGAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
        b.Draw(this.mermaidSprites, new Vector2((float) (index + (int) ((double) this.stopWatch.ElapsedMilliseconds / 6.0 % 448.0)), (float) (num * 2 - num * 3 / 4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(144, 32, 112, 48)), Color.Cyan * this.blackBGAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
        b.Draw(this.mermaidSprites, new Vector2((float) (index + 112) + (float) ((double) this.stopWatch.ElapsedMilliseconds / 6.0 % 448.0), (float) ((double) (num * 2) - (double) num / 4.0 + Math.Sin((double) this.stopWatch.ElapsedMilliseconds / 1000.0 + 4.0) * 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(161, 0, 16, 16)), Color.White * this.blackBGAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f / 1000f);
        b.Draw(this.mermaidSprites, new Vector2((float) (index - (int) ((double) this.stopWatch.ElapsedMilliseconds / 6.0 % 448.0)), (float) (num * 3 - num * 3 / 4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(144, 32, 112, 48)), Color.Orange * this.blackBGAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
        b.Draw(this.mermaidSprites, new Vector2((float) (index + 112) - (float) ((double) this.stopWatch.ElapsedMilliseconds / 6.0 % 448.0), (float) ((double) (num * 3) - (double) num / 4.0 + Math.Sin((double) this.stopWatch.ElapsedMilliseconds / 1000.0 + 3.0) * 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(129, 0, 16, 16)), Color.White * this.blackBGAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
        b.Draw(this.mermaidSprites, new Vector2((float) (index + (int) ((double) this.stopWatch.ElapsedMilliseconds / 6.0 % 448.0)), (float) (num * 4 - num * 3 / 4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(144, 32, 112, 48)), Color.HotPink * this.blackBGAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
        b.Draw(this.mermaidSprites, new Vector2((float) (index + 112) + (float) ((double) this.stopWatch.ElapsedMilliseconds / 6.0 % 448.0), (float) ((double) (num * 4) - (double) num / 4.0 + Math.Sin((double) this.stopWatch.ElapsedMilliseconds / 1000.0 + 2.0) * 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(145, 0, 16, 16)), Color.White * this.blackBGAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f / 1000f);
      }
      b.Draw(this.mermaidSprites, new Vector2((float) (Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.X - 112) + (float) (Math.Sin((double) this.stopWatch.ElapsedMilliseconds / 1000.0) * 64.0 * 2.0), (float) (Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.Y - 140) + (float) (Math.Cos((double) this.stopWatch.ElapsedMilliseconds / 1000.0 * 2.0 + Math.PI / 2.0) * 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int) (57L * (this.stopWatch.ElapsedMilliseconds % 1538L / 769L)), 0, 57, 70)), Color.White * this.bigMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
      foreach (TemporaryAnimatedSprite alwaysFrontTempSprite in this.alwaysFrontTempSprites)
        alwaysFrontTempSprite.draw(b, true);
      foreach (Vector2 bubble in this.bubbles)
        b.Draw(this.mermaidSprites, bubble + new Vector2((float) (Math.Sin((double) this.stopWatch.ElapsedMilliseconds / 1000.0 * 4.0 + (double) bubble.X) * 4.0 * 6.0), 0.0f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(132, 20, 8, 8)), Color.White * this.blackBGAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(-20f, 50f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * this.finalLeftMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(-20f, 50f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Orange * this.finalLeftMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0011f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(-30f, 90f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * this.finalLeftMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(-30f, 90f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Cyan * this.finalLeftMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0011f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(-40f, 130f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * this.finalLeftMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(-40f, 130f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Lime * this.finalLeftMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0011f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(150f, 50f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * this.finalRightMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f / 1000f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(150f, 50f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Orange * this.finalRightMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.0011f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(160f, 90f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * this.finalRightMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f / 1000f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(160f, 90f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Cyan * this.finalRightMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.0011f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(170f, 130f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * this.finalRightMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f / 1000f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(170f, 130f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Lime * this.finalRightMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.0011f);
      b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(43f, 180f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int) (57L * (this.stopWatch.ElapsedMilliseconds % 1538L / 769L)), 0, 57, 70)), Color.White * this.finalBigMermaidAlpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      ICue cue = Game1.currentSong;
      if (!Game1.game1.IsMainInstance)
        cue = GameRunner.instance.gameInstances[0].instanceCurrentSong;
      base.UpdateWhenCurrentLocation(time);
      if (this.stopWatch == null)
        return;
      if (!Game1.shouldTimePass())
      {
        if (this.stopWatch != null && this.stopWatch.IsRunning)
          this.stopWatch.Stop();
        if (cue != null && cue.Name.Equals("mermaidSong") && !cue.IsPaused && cue.IsPlaying)
          cue.Pause();
      }
      else
      {
        if (this.stopWatch != null && !this.stopWatch.IsRunning && cue != null && cue.Name.Equals("mermaidSong") && cue.IsPaused)
          this.stopWatch.Start();
        if (cue != null && cue.Name.Equals("mermaidSong") && cue.IsPaused)
          cue.Resume();
      }
      TimeSpan elapsedGameTime;
      if (Game1.shouldTimePass())
      {
        double showTimer = (double) this.showTimer;
        this.showTimer += (float) time.ElapsedGameTime.Milliseconds;
        if ((cue != null && cue.Name.Equals("mermaidSong") && cue.IsPlaying || (double) Game1.options.musicVolumeLevel <= 0.0 && (double) Game1.options.ambientVolumeLevel <= 0.0) && !this.stopWatch.IsRunning)
          this.stopWatch.Start();
        if ((double) this.curtainMovement != 0.0)
        {
          double curtainOpenPercent = (double) this.curtainOpenPercent;
          double curtainMovement = (double) this.curtainMovement;
          elapsedGameTime = time.ElapsedGameTime;
          double milliseconds = (double) elapsedGameTime.Milliseconds;
          double num = curtainMovement * milliseconds;
          this.curtainOpenPercent = Math.Max(0.0f, Math.Min(1f, (float) (curtainOpenPercent + num)));
        }
        if (showTimer < 3000.0 && (double) this.showTimer >= 3000.0)
          Game1.changeMusicTrack("mermaidSong");
        if (this.stopWatch != null && this.stopWatch.ElapsedMilliseconds > 0L && this.stopWatch.ElapsedMilliseconds < 1000L)
          this.curtainMovement = 0.0004f;
        for (int index = this.sparkles.Count - 1; index >= 0; --index)
        {
          if (this.sparkles[index].update(time))
            this.sparkles.RemoveAt(index);
        }
        for (int index = this.alwaysFrontTempSprites.Count - 1; index >= 0; --index)
        {
          if (this.alwaysFrontTempSprites[index].update(time))
            this.alwaysFrontTempSprites.RemoveAt(index);
        }
        if (this.stopWatch.ElapsedMilliseconds >= 30000L && this.stopWatch.ElapsedMilliseconds < 50000L && ((double) this.blackBGAlpha < 1.0 || (double) this.bigMermaidAlpha < 1.0))
        {
          this.blackBGAlpha += 0.01f;
          this.bigMermaidAlpha += 0.01f;
        }
        if (this.stopWatch.ElapsedMilliseconds > 27692L && this.stopWatch.ElapsedMilliseconds < 55385L)
        {
          if ((double) this.oldStopWatchTime % 769.0 > (double) (this.stopWatch.ElapsedMilliseconds % 769L))
            this.bubbles.Add(new Vector2((float) Game1.random.Next((int) ((double) Game1.graphics.GraphicsDevice.Viewport.Width / (double) Game1.options.zoomLevel) - 64), (float) Game1.graphics.GraphicsDevice.Viewport.Height / Game1.options.zoomLevel));
          for (int index1 = 0; index1 < this.bubbles.Count; ++index1)
          {
            List<Vector2> bubbles = this.bubbles;
            int index2 = index1;
            double x = (double) this.bubbles[index1].X;
            double y1 = (double) this.bubbles[index1].Y;
            elapsedGameTime = time.ElapsedGameTime;
            double num = 0.100000001490116 * (double) elapsedGameTime.Milliseconds;
            double y2 = y1 - num;
            Vector2 vector2 = new Vector2((float) x, (float) y2);
            bubbles[index2] = vector2;
          }
        }
        Viewport viewport;
        if ((double) this.oldStopWatchTime < 36923.0 && this.stopWatch.ElapsedMilliseconds >= 36923L)
        {
          List<TemporaryAnimatedSprite> frontTempSprites = this.alwaysFrontTempSprites;
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite();
          temporaryAnimatedSprite.texture = this.mermaidSprites;
          temporaryAnimatedSprite.xPeriodic = true;
          temporaryAnimatedSprite.xPeriodicLoopTime = 2000f;
          temporaryAnimatedSprite.xPeriodicRange = 32f;
          temporaryAnimatedSprite.motion = new Vector2(0.0f, -4f);
          temporaryAnimatedSprite.sourceRectStartingPos = new Vector2(67f, 189f);
          temporaryAnimatedSprite.sourceRect = new Microsoft.Xna.Framework.Rectangle(67, 189, 24, 53);
          temporaryAnimatedSprite.totalNumberOfLoops = 100;
          temporaryAnimatedSprite.animationLength = 3;
          temporaryAnimatedSprite.pingPong = true;
          temporaryAnimatedSprite.interval = 192f;
          temporaryAnimatedSprite.delayBeforeAnimationStart = 0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double x1 = (double) viewport.Width / 4.0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double y3 = (double) (viewport.Height - 1);
          temporaryAnimatedSprite.initialPosition = new Vector2((float) x1, (float) y3);
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double x2 = (double) viewport.Width / (double) Game1.options.zoomLevel / 4.0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double y4 = (double) viewport.Height / (double) Game1.options.zoomLevel - 1.0;
          temporaryAnimatedSprite.position = new Vector2((float) x2, (float) y4);
          temporaryAnimatedSprite.scale = 4f;
          temporaryAnimatedSprite.layerDepth = 1f;
          frontTempSprites.Add(temporaryAnimatedSprite);
        }
        if ((double) this.oldStopWatchTime < 40000.0 && this.stopWatch.ElapsedMilliseconds >= 40000L)
        {
          List<TemporaryAnimatedSprite> frontTempSprites = this.alwaysFrontTempSprites;
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite();
          temporaryAnimatedSprite.texture = this.mermaidSprites;
          temporaryAnimatedSprite.xPeriodic = true;
          temporaryAnimatedSprite.xPeriodicLoopTime = 2000f;
          temporaryAnimatedSprite.xPeriodicRange = 32f;
          temporaryAnimatedSprite.motion = new Vector2(0.0f, -4f);
          temporaryAnimatedSprite.sourceRectStartingPos = new Vector2(67f, 189f);
          temporaryAnimatedSprite.sourceRect = new Microsoft.Xna.Framework.Rectangle(67, 189, 24, 53);
          temporaryAnimatedSprite.totalNumberOfLoops = 100;
          temporaryAnimatedSprite.animationLength = 3;
          temporaryAnimatedSprite.pingPong = true;
          temporaryAnimatedSprite.interval = 192f;
          temporaryAnimatedSprite.delayBeforeAnimationStart = 0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double x3 = (double) viewport.Width * 3.0 / 4.0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double y5 = (double) (viewport.Height - 1);
          temporaryAnimatedSprite.initialPosition = new Vector2((float) x3, (float) y5);
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double x4 = (double) viewport.Width / (double) Game1.options.zoomLevel * 3.0 / 4.0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double y6 = (double) viewport.Height / (double) Game1.options.zoomLevel - 1.0;
          temporaryAnimatedSprite.position = new Vector2((float) x4, (float) y6);
          temporaryAnimatedSprite.scale = 4f;
          temporaryAnimatedSprite.layerDepth = 1f;
          frontTempSprites.Add(temporaryAnimatedSprite);
        }
        if ((double) this.oldStopWatchTime < 43077.0 && this.stopWatch.ElapsedMilliseconds >= 43077L)
        {
          List<TemporaryAnimatedSprite> frontTempSprites = this.alwaysFrontTempSprites;
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite();
          temporaryAnimatedSprite.texture = this.mermaidSprites;
          temporaryAnimatedSprite.xPeriodic = true;
          temporaryAnimatedSprite.xPeriodicLoopTime = 2000f;
          temporaryAnimatedSprite.xPeriodicRange = 32f;
          temporaryAnimatedSprite.motion = new Vector2(0.0f, -4f);
          temporaryAnimatedSprite.sourceRectStartingPos = new Vector2(67f, 189f);
          temporaryAnimatedSprite.sourceRect = new Microsoft.Xna.Framework.Rectangle(67, 189, 24, 53);
          temporaryAnimatedSprite.totalNumberOfLoops = 100;
          temporaryAnimatedSprite.animationLength = 3;
          temporaryAnimatedSprite.pingPong = true;
          temporaryAnimatedSprite.interval = 192f;
          temporaryAnimatedSprite.delayBeforeAnimationStart = 0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double x5 = (double) viewport.Width / 4.0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double y7 = (double) (viewport.Height - 1);
          temporaryAnimatedSprite.initialPosition = new Vector2((float) x5, (float) y7);
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double x6 = (double) viewport.Width / (double) Game1.options.zoomLevel / 4.0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double y8 = (double) viewport.Height / (double) Game1.options.zoomLevel - 1.0;
          temporaryAnimatedSprite.position = new Vector2((float) x6, (float) y8);
          temporaryAnimatedSprite.scale = 4f;
          temporaryAnimatedSprite.layerDepth = 1f;
          frontTempSprites.Add(temporaryAnimatedSprite);
        }
        if ((double) this.oldStopWatchTime < 46154.0 && this.stopWatch.ElapsedMilliseconds >= 46154L)
        {
          List<TemporaryAnimatedSprite> frontTempSprites = this.alwaysFrontTempSprites;
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite();
          temporaryAnimatedSprite.texture = this.mermaidSprites;
          temporaryAnimatedSprite.xPeriodic = true;
          temporaryAnimatedSprite.xPeriodicLoopTime = 2000f;
          temporaryAnimatedSprite.xPeriodicRange = 32f;
          temporaryAnimatedSprite.motion = new Vector2(0.0f, -4f);
          temporaryAnimatedSprite.sourceRectStartingPos = new Vector2(67f, 189f);
          temporaryAnimatedSprite.sourceRect = new Microsoft.Xna.Framework.Rectangle(67, 189, 24, 53);
          temporaryAnimatedSprite.totalNumberOfLoops = 100;
          temporaryAnimatedSprite.animationLength = 3;
          temporaryAnimatedSprite.pingPong = true;
          temporaryAnimatedSprite.interval = 192f;
          temporaryAnimatedSprite.delayBeforeAnimationStart = 0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double x7 = (double) viewport.Width * 3.0 / 4.0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double y9 = (double) (viewport.Height - 1);
          temporaryAnimatedSprite.initialPosition = new Vector2((float) x7, (float) y9);
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double x8 = (double) viewport.Width / (double) Game1.options.zoomLevel * 3.0 / 4.0;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          double y10 = (double) viewport.Height / (double) Game1.options.zoomLevel - 1.0;
          temporaryAnimatedSprite.position = new Vector2((float) x8, (float) y10);
          temporaryAnimatedSprite.scale = 4f;
          temporaryAnimatedSprite.layerDepth = 1f;
          frontTempSprites.Add(temporaryAnimatedSprite);
        }
        if (this.stopWatch.ElapsedMilliseconds >= 52308L && ((double) this.blackBGAlpha > 0.0 || (double) this.bigMermaidAlpha > 0.0))
        {
          this.blackBGAlpha -= 0.01f;
          this.bigMermaidAlpha -= 0.01f;
        }
        if (this.stopWatch.ElapsedMilliseconds >= 58462L && this.stopWatch.ElapsedMilliseconds < 60000L && (double) this.finalLeftMermaidAlpha < 1.0)
          this.finalLeftMermaidAlpha += 0.01f;
        if (this.stopWatch.ElapsedMilliseconds >= 60000L && this.stopWatch.ElapsedMilliseconds < 62000L && (double) this.finalRightMermaidAlpha < 1.0)
          this.finalRightMermaidAlpha += 0.01f;
        if (this.stopWatch.ElapsedMilliseconds >= 61538L && this.stopWatch.ElapsedMilliseconds < 63538L && (double) this.finalBigMermaidAlpha < 1.0)
          this.finalBigMermaidAlpha += 0.01f;
        if (this.stopWatch.ElapsedMilliseconds >= 64615L && ((double) this.finalBigMermaidAlpha < 1.0 || (double) this.finalRightMermaidAlpha < 1.0 || (double) this.finalLeftMermaidAlpha < 1.0))
        {
          this.finalBigMermaidAlpha -= 0.01f;
          this.finalRightMermaidAlpha -= 0.01f;
          this.finalLeftMermaidAlpha -= 0.01f;
        }
        if ((double) this.oldStopWatchTime < 64808.0 && this.stopWatch.ElapsedMilliseconds >= 64808L)
        {
          for (int index = 0; index < 200; ++index)
            this.sparkles.Add(new TemporaryAnimatedSprite()
            {
              texture = this.mermaidSprites,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 146, 16, 13),
              animationLength = 9,
              interval = 100f,
              delayBeforeAnimationStart = index * 10,
              position = Utility.getRandomPositionOnScreenNotOnMap(),
              scale = 4f
            });
          Utility.addSprinklesToLocation((GameLocation) this, 5, 5, 9, 5, 2000, 100, Color.White);
        }
        if ((double) this.oldStopWatchTime < 67500.0 && this.stopWatch.ElapsedMilliseconds >= 67500L)
          this.curtainMovement = -0.0003f;
        this.oldStopWatchTime = (float) this.stopWatch.ElapsedMilliseconds;
      }
      if ((double) this.fairyTimer <= 0.0)
        return;
      double fairyTimer = (double) this.fairyTimer;
      elapsedGameTime = time.ElapsedGameTime;
      double milliseconds1 = (double) elapsedGameTime.Milliseconds;
      this.fairyTimer = (float) (fairyTimer - milliseconds1);
      if ((double) this.fairyTimer < 200.0 && this.pearlRecipient != null && (int) this.pearlRecipient.facingDirection == 0)
        this.pearlRecipient.faceDirection(1);
      if ((double) this.fairyTimer < 100.0 && this.pearlRecipient != null)
        this.pearlRecipient.faceDirection(2);
      if ((double) this.fairyTimer > 0.0 || this.pearlRecipient == null)
        return;
      foreach (TemporaryAnimatedSprite temporarySprite in this.temporarySprites)
        temporarySprite.alphaFade = 0.01f;
      this.pearlRecipient.addItemByMenuIfNecessaryElseHoldUp((Item) new StardewValley.Object(797, 1));
      this.pearlRecipient.mailReceived.Add("gotPearl");
    }
  }
}
