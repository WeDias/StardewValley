// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.CalderaMonkey
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class CalderaMonkey : Critter
  {
    private const int phase_tailBOB = 0;
    private const int phase_footPaddle = 1;
    private const int phase_relaxing = 2;
    private const int phase_scream = 3;
    public Rectangle movementRectangle;
    private int currentPhase;
    private int currentFrame;
    private float nextFrameTimer;
    private float nextPhaseTimer;
    private float currentFrameDelay;
    protected Rectangle _baseSourceRectangle = new Rectangle(0, 309, 20, 24);
    protected Vector2 movementDirection = Vector2.Zero;
    private List<Vector2> buddies = new List<Vector2>();
    private Texture2D texture;
    private Texture2D swimShadow;

    public CalderaMonkey()
    {
      this.sprite = new AnimatedSprite(Critter.critterTexture, 0, 18, 18);
      this.sprite.SourceRect = this._baseSourceRectangle;
      this.sprite.ignoreSourceRectUpdates = true;
      this.texture = Game1.temporaryContent.Load<Texture2D>(Critter.critterTexture);
      this.swimShadow = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\swimShadow");
    }

    public CalderaMonkey(Vector2 start_position)
      : this()
    {
      this.position = start_position;
      this.sprite = new AnimatedSprite(Critter.critterTexture, 0, 18, 18);
      this.sprite.SourceRect = this._baseSourceRectangle;
      this.sprite.ignoreSourceRectUpdates = true;
      if (Game1.random.NextDouble() < 0.5)
        this.buddies.Add(new Vector2(-96f, 76.8f) + this.position);
      if (Game1.random.NextDouble() < 0.5)
        this.buddies.Add(new Vector2(32f, 134.4f) + this.position);
      if (Game1.random.NextDouble() < 0.5)
        this.buddies.Add(new Vector2(128f, 44.8f) + this.position);
      this.texture = Game1.temporaryContent.Load<Texture2D>(Critter.critterTexture);
    }

    public override bool update(GameTime time, GameLocation environment)
    {
      this.nextFrameTimer -= (float) (int) time.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.nextPhaseTimer >= 0.0)
      {
        this.nextPhaseTimer -= (float) (int) time.ElapsedGameTime.TotalMilliseconds;
        if ((double) this.nextPhaseTimer <= 0.0)
        {
          if (this.currentPhase != 3 || Game1.random.NextDouble() >= 0.2)
            this.currentPhase = Game1.random.Next(4);
          this.nextFrameTimer = 0.0f;
          switch (this.currentPhase)
          {
            case 0:
              this.currentFrameDelay = (float) Game1.random.Next(400, 500);
              this.nextPhaseTimer = (float) Game1.random.Next(3000, 8000);
              break;
            case 1:
              this.currentFrameDelay = (float) Game1.random.Next(300, 1200);
              this.nextPhaseTimer = (float) Game1.random.Next(3000, 6000);
              break;
            case 2:
              this.nextPhaseTimer = (float) Game1.random.Next(3000, 8000);
              break;
            case 3:
              this.nextPhaseTimer = (float) Game1.random.Next(700, 3000);
              this.nextFrameTimer = 400f;
              if (Game1.activeClickableMenu == null)
                environment.playSound("monkey1");
              this.setFrame(5);
              break;
          }
        }
      }
      switch (this.currentPhase)
      {
        case 0:
          if ((double) this.nextFrameTimer <= 0.0)
          {
            if (this.currentFrame == 0)
              this.setFrame(1);
            else
              this.setFrame(0);
            if (Game1.random.NextDouble() < 0.2)
            {
              this.setFrame(6);
              this.nextFrameTimer = 200f;
              break;
            }
            this.nextFrameTimer = this.currentFrameDelay;
            break;
          }
          break;
        case 1:
          if ((double) this.nextFrameTimer <= 0.0)
          {
            if (this.currentFrame == 2)
              this.setFrame(3);
            else
              this.setFrame(2);
            this.nextFrameTimer = this.currentFrameDelay;
            if (Game1.activeClickableMenu == null)
              environment.playSound("slosh");
            environment.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 0, 64, 64), 150f, 3, 0, this.position + new Vector2(this.currentFrame == 2 ? 32f : -8f, 48f), false, Game1.random.NextDouble() < 0.5, 1f / 1000f, 0.02f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f));
            break;
          }
          break;
        case 2:
          this.setFrame(4);
          break;
        case 3:
          if ((double) this.nextFrameTimer <= 0.0)
          {
            this.setFrame(0);
            break;
          }
          break;
      }
      return base.update(time, environment);
    }

    private void setFrame(int frame)
    {
      this.sprite.sourceRect.X = frame * 20;
      this.currentFrame = frame;
    }

    public override void draw(SpriteBatch b)
    {
      this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, Utility.snapDrawPosition(this.position + new Vector2(0.0f, this.yJumpOffset - 20f + this.yOffset))), (float) (((double) this.position.Y + 64.0 - 32.0) / 10000.0), 0, 0, Color.White, this.flip, 4f);
      for (int index = 0; index < this.buddies.Count; ++index)
      {
        TimeSpan totalGameTime = Game1.currentGameTime.TotalGameTime;
        float y = (float) Math.Sin(totalGameTime.TotalMilliseconds / 500.0 + (double) (index * 100)) * 4f;
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, this.buddies[index]);
        b.Draw(this.texture, local + new Vector2(0.0f, y), new Rectangle?(new Rectangle(14 * index, 333, 14, 12 - (int) y / 2)), Color.White, 0.0f, Vector2.Zero, 4f, (double) local.X > 1408.0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.buddies[index].Y / 10000f);
        b.Draw(Game1.staminaRect, new Rectangle((int) local.X + (int) y + 8, (int) local.Y + 44 + 2, 56 - (int) y * 2 - 16, 4), new Rectangle?(Game1.staminaRect.Bounds), new Color((int) byte.MaxValue, (int) byte.MaxValue, 150) * 0.55f, 0.0f, Vector2.Zero, SpriteEffects.None, (float) ((double) this.buddies[index].Y / 10000.0 + 1.0 / 1000.0));
        SpriteBatch spriteBatch = b;
        Texture2D swimShadow = this.swimShadow;
        Vector2 position = local + new Vector2(-4f, 48f);
        totalGameTime = Game1.currentGameTime.TotalGameTime;
        Rectangle? sourceRectangle = new Rectangle?(new Rectangle((int) totalGameTime.TotalMilliseconds % 700 / 70 * 16, 0, 16, 16));
        Color white = Color.White;
        Vector2 zero = Vector2.Zero;
        double layerDepth = (double) this.buddies[index].Y / 10000.0 - 1.0 / 1000.0;
        spriteBatch.Draw(swimShadow, position, sourceRectangle, white, 0.0f, zero, 4f, SpriteEffects.None, (float) layerDepth);
      }
    }

    public override void drawAboveFrontLayer(SpriteBatch b)
    {
    }
  }
}
