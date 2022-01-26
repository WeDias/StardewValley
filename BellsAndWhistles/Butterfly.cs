// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Butterfly
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class Butterfly : Critter
  {
    public const float maxSpeed = 3f;
    private int flapTimer;
    private int checkForLandingSpotTimer;
    private int landedTimer;
    private int flapSpeed = 50;
    private Vector2 motion;
    private float motionMultiplier = 1f;
    private bool summerButterfly;
    private bool islandButterfly;
    public bool stayInbounds;

    public Butterfly(Vector2 position, bool islandButterfly = false)
    {
      this.position = position * 64f;
      this.startingPosition = this.position;
      if (Game1.currentSeason.Equals("spring"))
      {
        this.baseFrame = Game1.random.NextDouble() < 0.5 ? Game1.random.Next(3) * 3 + 160 : Game1.random.Next(3) * 3 + 180;
      }
      else
      {
        this.baseFrame = Game1.random.NextDouble() < 0.5 ? Game1.random.Next(3) * 4 + 128 : Game1.random.Next(3) * 4 + 148;
        this.summerButterfly = true;
      }
      if (islandButterfly)
      {
        this.islandButterfly = islandButterfly;
        this.baseFrame = Game1.random.Next(4) * 4 + 364;
        this.summerButterfly = true;
      }
      this.motion = new Vector2((float) ((Game1.random.NextDouble() + 0.25) * 3.0 * (Game1.random.NextDouble() < 0.5 ? -1.0 : 1.0) / 2.0), (float) ((Game1.random.NextDouble() + 0.5) * 3.0 * (Game1.random.NextDouble() < 0.5 ? -1.0 : 1.0) / 2.0));
      this.flapSpeed = Game1.random.Next(45, 80);
      this.sprite = new AnimatedSprite(Critter.critterTexture, this.baseFrame, 16, 16);
      this.sprite.loop = false;
      this.startingPosition = position;
    }

    public void doneWithFlap(Farmer who) => this.flapTimer = 200 + Game1.random.Next(-5, 6);

    public Butterfly setStayInbounds(bool stayInbounds)
    {
      this.stayInbounds = stayInbounds;
      return this;
    }

    public override bool update(GameTime time, GameLocation environment)
    {
      this.flapTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.flapTimer <= 0 && this.sprite.CurrentAnimation == null)
      {
        this.motionMultiplier = 1f;
        this.motion.X += (float) Game1.random.Next(-80, 81) / 100f;
        this.motion.Y = (float) ((Game1.random.NextDouble() + 0.25) * -3.0 / 2.0);
        if ((double) Math.Abs(this.motion.X) > 1.5)
          this.motion.X = (float) (3.0 * (double) Math.Sign(this.motion.X) / 2.0);
        if ((double) Math.Abs(this.motion.Y) > 3.0)
          this.motion.Y = 3f * (float) Math.Sign(this.motion.Y);
        if (this.stayInbounds)
        {
          if ((double) this.position.X < 128.0)
            this.motion.X = 0.8f;
          if ((double) this.position.Y < 192.0)
          {
            this.motion.Y /= 2f;
            this.flapTimer = 1000;
          }
          if ((double) this.position.X > (double) (environment.map.DisplayWidth - 128))
            this.motion.X = -0.8f;
          if ((double) this.position.Y > (double) (environment.map.DisplayHeight - 128))
          {
            this.motion.Y = -1f;
            this.flapTimer = 100;
          }
        }
        if (this.summerButterfly)
          this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(this.baseFrame + 1, this.flapSpeed),
            new FarmerSprite.AnimationFrame(this.baseFrame + 2, this.flapSpeed),
            new FarmerSprite.AnimationFrame(this.baseFrame + 3, this.flapSpeed),
            new FarmerSprite.AnimationFrame(this.baseFrame + 2, this.flapSpeed),
            new FarmerSprite.AnimationFrame(this.baseFrame + 1, this.flapSpeed),
            new FarmerSprite.AnimationFrame(this.baseFrame, this.flapSpeed, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doneWithFlap))
          });
        else
          this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(this.baseFrame + 1, this.flapSpeed),
            new FarmerSprite.AnimationFrame(this.baseFrame + 2, this.flapSpeed),
            new FarmerSprite.AnimationFrame(this.baseFrame + 1, this.flapSpeed),
            new FarmerSprite.AnimationFrame(this.baseFrame, this.flapSpeed, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doneWithFlap))
          });
      }
      this.position = this.position + this.motion * this.motionMultiplier;
      this.motion.Y += 0.005f * (float) time.ElapsedGameTime.Milliseconds;
      this.motionMultiplier -= 0.0005f * (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.motionMultiplier <= 0.0)
        this.motionMultiplier = 0.0f;
      return base.update(time, environment);
    }

    public override void draw(SpriteBatch b)
    {
    }

    public override void drawAboveFrontLayer(SpriteBatch b) => this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)), this.position.Y / 10000f, 0, 0, Color.White, this.flip, 4f);
  }
}
