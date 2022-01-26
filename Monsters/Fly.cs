// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Fly
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;

namespace StardewValley.Monsters
{
  public class Fly : Monster
  {
    public const float rotationIncrement = 0.04908739f;
    public const int volumeTileRange = 16;
    public const int spawnTime = 1000;
    private int spawningCounter = 1000;
    private int wasHitCounter;
    private float targetRotation;
    public static ICue buzz;
    private bool turningRight;
    public bool hard;

    public Fly()
    {
    }

    public Fly(Vector2 position)
      : this(position, false)
    {
    }

    public Fly(Vector2 position, bool hard)
      : base(nameof (Fly), position)
    {
      this.Slipperiness = 24 + Game1.random.Next(-10, 10);
      this.Halt();
      this.IsWalkingTowardPlayer = false;
      this.hard = hard;
      if (hard)
      {
        this.DamageToFarmer *= 2;
        this.MaxHealth *= 3;
        this.Health = this.MaxHealth;
      }
      this.HideShadow = true;
    }

    public void setHard()
    {
      this.hard = true;
      if (!this.hard)
        return;
      this.DamageToFarmer = 12;
      this.MaxHealth = 66;
      this.Health = this.MaxHealth;
    }

    public override void reloadSprite()
    {
      this.Sprite = new AnimatedSprite("Characters\\Monsters\\Fly");
      if (Game1.soundBank != null)
        Fly.buzz = Game1.soundBank.GetCue("flybuzzing");
      this.HideShadow = true;
    }

    public override int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      Farmer who)
    {
      int damage1 = Math.Max(1, damage - (int) (NetFieldBase<int, NetInt>) this.resilience);
      if (Game1.random.NextDouble() < (double) (NetFieldBase<double, NetDouble>) this.missChance - (double) (NetFieldBase<double, NetDouble>) this.missChance * addedPrecision)
      {
        damage1 = -1;
      }
      else
      {
        this.Health -= damage1;
        this.setTrajectory(xTrajectory / 3, yTrajectory / 3);
        this.wasHitCounter = 500;
        if (this.currentLocation != null)
          this.currentLocation.playSound("hitEnemy");
        if (this.Health <= 0)
        {
          if (this.currentLocation != null)
          {
            this.currentLocation.playSound("monsterdead");
            Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.HotPink, 10)
            {
              interval = 70f
            }, this.currentLocation);
          }
          if (Game1.soundBank != null && Fly.buzz != null)
            Fly.buzz.Stop(AudioStopOptions.AsAuthored);
        }
      }
      this.addedSpeed = Game1.random.Next(-1, 1);
      return damage1;
    }

    public override void drawAboveAllLayers(SpriteBatch b)
    {
      if (!Utility.isOnScreen(this.Position, 128))
        return;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (this.GetBoundingBox().Height / 2 - 32)), new Rectangle?(this.Sprite.SourceRect), this.hard ? Color.Lime : Color.White, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) (this.getStandingY() + 8) / 10000f));
      SpriteBatch spriteBatch = b;
      Texture2D shadowTexture = Game1.shadowTexture;
      Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (this.GetBoundingBox().Height / 2));
      Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
      Color white = Color.White;
      Rectangle bounds = Game1.shadowTexture.Bounds;
      double x = (double) bounds.Center.X;
      bounds = Game1.shadowTexture.Bounds;
      double y = (double) bounds.Center.Y;
      Vector2 origin = new Vector2((float) x, (float) y);
      double layerDepth = (double) (this.getStandingY() - 1) / 10000.0;
      spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, (float) layerDepth);
      if (!this.isGlowing)
        return;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (this.GetBoundingBox().Height / 2 - 32)), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.99f : (float) ((double) this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      if (this.currentLocation == null || !(bool) (NetFieldBase<bool, NetBool>) this.currentLocation.treatAsOutdoors)
        return;
      this.drawAboveAllLayers(b);
    }

    protected override void updateAnimation(GameTime time)
    {
      if (Game1.soundBank != null && (Fly.buzz == null || !Fly.buzz.IsPlaying) && (this.currentLocation == null || this.currentLocation.Equals(Game1.currentLocation)))
      {
        Fly.buzz = Game1.soundBank.GetCue("flybuzzing");
        Fly.buzz.SetVariable("Volume", 0.0f);
        Fly.buzz.Play();
      }
      if ((double) Game1.fadeToBlackAlpha > 0.8 && Game1.fadeIn && Fly.buzz != null)
        Fly.buzz.Stop(AudioStopOptions.AsAuthored);
      else if (Fly.buzz != null)
      {
        Fly.buzz.SetVariable("Volume", Math.Max(0.0f, Fly.buzz.GetVariable("Volume") - 1f));
        float val = Math.Max(0.0f, (float) (100.0 - (double) Vector2.Distance(this.Position, this.Player.Position) / 64.0 / 16.0 * 100.0));
        if ((double) val > (double) Fly.buzz.GetVariable("Volume"))
          Fly.buzz.SetVariable("Volume", val);
      }
      TimeSpan elapsedGameTime;
      if (this.wasHitCounter >= 0)
      {
        int wasHitCounter = this.wasHitCounter;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.wasHitCounter = wasHitCounter - milliseconds;
      }
      this.Sprite.Animate(time, this.FacingDirection == 0 ? 8 : (this.FacingDirection == 2 ? 0 : this.FacingDirection * 4), 4, 75f);
      if (this.spawningCounter >= 0)
      {
        int spawningCounter = this.spawningCounter;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.spawningCounter = spawningCounter - milliseconds;
        this.Scale = (float) (1.0 - (double) this.spawningCounter / 1000.0);
      }
      else if ((this.withinPlayerThreshold() || Utility.isOnScreen((Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position, 256)) && this.invincibleCountdown <= 0)
      {
        this.faceDirection(0);
        float num1 = (float) -(this.Player.GetBoundingBox().Center.X - this.GetBoundingBox().Center.X);
        Rectangle boundingBox = this.Player.GetBoundingBox();
        int y1 = boundingBox.Center.Y;
        boundingBox = this.GetBoundingBox();
        int y2 = boundingBox.Center.Y;
        float num2 = (float) (y1 - y2);
        float num3 = Math.Max(1f, Math.Abs(num1) + Math.Abs(num2));
        if ((double) num3 < 64.0)
        {
          this.xVelocity = Math.Max(-7f, Math.Min(7f, this.xVelocity * 1.1f));
          this.yVelocity = Math.Max(-7f, Math.Min(7f, this.yVelocity * 1.1f));
        }
        float x = num1 / num3;
        float num4 = num2 / num3;
        if (this.wasHitCounter <= 0)
        {
          this.targetRotation = (float) Math.Atan2(-(double) num4, (double) x) - 1.570796f;
          if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) > 7.0 * Math.PI / 8.0 && Game1.random.NextDouble() < 0.5)
            this.turningRight = true;
          else if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) < Math.PI / 8.0)
            this.turningRight = false;
          if (this.turningRight)
            this.rotation -= (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
          else
            this.rotation += (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
          this.rotation %= 6.283185f;
          this.wasHitCounter = 5 + Game1.random.Next(-1, 2);
        }
        float num5 = Math.Min(7f, Math.Max(2f, (float) (7.0 - (double) num3 / 64.0 / 2.0)));
        float num6 = (float) Math.Cos((double) this.rotation + Math.PI / 2.0);
        float num7 = -(float) Math.Sin((double) this.rotation + Math.PI / 2.0);
        this.xVelocity += (float) (-(double) num6 * (double) num5 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
        this.yVelocity += (float) (-(double) num7 * (double) num5 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
        if ((double) Math.Abs(this.xVelocity) > (double) Math.Abs((float) (-(double) num6 * 7.0)))
          this.xVelocity -= (float) (-(double) num6 * (double) num5 / 6.0);
        if ((double) Math.Abs(this.yVelocity) > (double) Math.Abs((float) (-(double) num7 * 7.0)))
          this.yVelocity -= (float) (-(double) num7 * (double) num5 / 6.0);
      }
      this.resetAnimationSpeed();
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      base.behaviorAtGameTick(time);
      if (double.IsNaN((double) this.xVelocity) || double.IsNaN((double) this.yVelocity))
        this.Health = -500;
      if ((double) this.Position.X > -640.0 && (double) this.Position.Y > -640.0 && (double) this.Position.X < (double) (this.currentLocation.Map.Layers[0].LayerWidth * 64 + 640) && (double) this.Position.Y < (double) (this.currentLocation.Map.Layers[0].LayerHeight * 64 + 640))
        return;
      this.Health = -500;
    }

    public override void Removed()
    {
      base.Removed();
      if (Fly.buzz == null)
        return;
      Fly.buzz.Stop(AudioStopOptions.AsAuthored);
    }
  }
}
