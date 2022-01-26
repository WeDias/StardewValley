// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.SquidKid
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Projectiles;
using System;

namespace StardewValley.Monsters
{
  public class SquidKid : Monster
  {
    private float lastFireball;
    private int yOffset;
    private readonly NetEvent0 fireballEvent = new NetEvent0();
    private readonly NetEvent0 hurtAnimationEvent = new NetEvent0();
    private int numFireballsLeft;
    private float firingTimer;

    public SquidKid()
    {
    }

    public SquidKid(Vector2 position)
      : base("Squid Kid", position)
    {
      this.Sprite.SpriteHeight = 16;
      this.IsWalkingTowardPlayer = false;
      this.Sprite.UpdateSourceRect();
      this.HideShadow = true;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.fireballEvent, (INetSerializable) this.hurtAnimationEvent);
      this.fireballEvent.onEvent += (NetEvent0.Event) (() =>
      {
        if (Game1.IsMasterGame)
          return;
        this.fireballFired();
      });
      this.hurtAnimationEvent.onEvent += (NetEvent0.Event) (() => this.Sprite.currentFrame = this.Sprite.currentFrame - this.Sprite.currentFrame % 4 + 3);
    }

    public override void reloadSprite() => this.Sprite = new AnimatedSprite("Characters\\Monsters\\Squid Kid");

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
        this.setTrajectory(xTrajectory, yTrajectory);
        this.currentLocation.playSound("hitEnemy");
        this.hurtAnimationEvent.Fire();
        if (this.Health <= 0)
          this.deathAnimation();
      }
      return damage1;
    }

    protected override void sharedDeathAnimation()
    {
    }

    protected override void localDeathAnimation()
    {
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite((string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(0, 64, 16, 16), 70f, 7, 0, this.Position + new Vector2(0.0f, -32f), false, false)
      {
        scale = 4f
      });
      this.currentLocation.localSound("fireball");
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(362, 30f, 6, 1, this.Position + new Vector2((float) (Game1.random.Next(64) - 16), (float) (Game1.random.Next(64) - 32)), false, Game1.random.NextDouble() < 0.5)
      {
        delayBeforeAnimationStart = 100
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(362, 30f, 6, 1, this.Position + new Vector2((float) (Game1.random.Next(64) - 16), (float) (Game1.random.Next(64) - 32)), false, Game1.random.NextDouble() < 0.5)
      {
        delayBeforeAnimationStart = 200
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(362, 30f, 6, 1, this.Position + new Vector2((float) (Game1.random.Next(64) - 16), (float) (Game1.random.Next(64) - 32)), false, Game1.random.NextDouble() < 0.5)
      {
        delayBeforeAnimationStart = 300
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(362, 30f, 6, 1, this.Position + new Vector2((float) (Game1.random.Next(64) - 16), (float) (Game1.random.Next(64) - 32)), false, Game1.random.NextDouble() < 0.5)
      {
        delayBeforeAnimationStart = 400
      });
    }

    public override void drawAboveAllLayers(SpriteBatch b)
    {
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (21 + this.yOffset)), new Rectangle?(this.Sprite.SourceRect), Color.White, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));
      SpriteBatch spriteBatch = b;
      Texture2D shadowTexture = Game1.shadowTexture;
      Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, 64f);
      Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
      Color white = Color.White;
      Rectangle bounds = Game1.shadowTexture.Bounds;
      double x = (double) bounds.Center.X;
      bounds = Game1.shadowTexture.Bounds;
      double y = (double) bounds.Center.Y;
      Vector2 origin = new Vector2((float) x, (float) y);
      double scale = 3.0 + (double) this.yOffset / 20.0;
      double layerDepth = (double) (this.getStandingY() - 1) / 10000.0;
      spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
    }

    protected override void updateAnimation(GameTime time)
    {
      base.updateAnimation(time);
      this.yOffset = (int) (Math.Sin((double) time.TotalGameTime.Milliseconds / 2000.0 * (2.0 * Math.PI)) * 15.0);
      if (this.Sprite.currentFrame % 4 != 0 && Game1.random.NextDouble() < 0.1)
        this.Sprite.currentFrame -= this.Sprite.currentFrame % 4;
      if (Game1.random.NextDouble() < 0.01)
        ++this.Sprite.currentFrame;
      this.resetAnimationSpeed();
    }

    protected override void updateMonsterSlaveAnimation(GameTime time)
    {
      if (this.isMoving())
      {
        if (this.FacingDirection == 0)
          this.Sprite.AnimateUp(time);
        else if (this.FacingDirection == 3)
          this.Sprite.AnimateLeft(time);
        else if (this.FacingDirection == 1)
          this.Sprite.AnimateRight(time);
        else if (this.FacingDirection == 2)
          this.Sprite.AnimateDown(time);
      }
      this.faceGeneralDirection(this.Player.Position);
    }

    private Vector2 fireballFired()
    {
      switch (this.FacingDirection)
      {
        case 0:
          this.Sprite.currentFrame = 3;
          return Vector2.Zero;
        case 1:
          this.Sprite.currentFrame = 7;
          return new Vector2(64f, 0.0f);
        case 2:
          this.Sprite.currentFrame = 11;
          return new Vector2(0.0f, 32f);
        case 3:
          this.Sprite.currentFrame = 15;
          return new Vector2(-32f, 0.0f);
        default:
          return Vector2.Zero;
      }
    }

    public override void update(GameTime time, GameLocation location)
    {
      base.update(time, location);
      this.fireballEvent.Poll();
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      base.behaviorAtGameTick(time);
      this.faceGeneralDirection(this.Player.Position);
      this.lastFireball = Math.Max(0.0f, this.lastFireball - (float) time.ElapsedGameTime.Milliseconds);
      if ((bool) (NetFieldBase<bool, NetBool>) this.isHardModeMonster)
      {
        if (this.numFireballsLeft <= 0 && !this.withinPlayerThreshold() || (double) this.lastFireball > 0.0)
          return;
        if ((double) this.lastFireball <= 0.0 && this.numFireballsLeft <= 0)
        {
          this.numFireballsLeft = 4;
          this.firingTimer = 0.0f;
        }
        this.firingTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
        if ((double) this.firingTimer > 0.0 || this.numFireballsLeft <= 0)
          return;
        --this.numFireballsLeft;
        this.IsWalkingTowardPlayer = false;
        Vector2 vector2_1 = new Vector2(this.Position.X, this.Position.Y + 64f);
        this.Halt();
        this.fireballEvent.Fire();
        Vector2 vector2_2 = this.fireballFired();
        Vector2 vector2_3 = vector2_1 + vector2_2;
        this.Sprite.UpdateSourceRect();
        Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(this.getStandingPosition(), new Vector2((float) this.Player.GetBoundingBox().X, (float) this.Player.GetBoundingBox().Y) + new Vector2((float) Game1.random.Next((int) sbyte.MinValue, 128)), 8f);
        BasicProjectile basicProjectile = new BasicProjectile(15, 10, 2, 4, 0.0f, velocityTowardPoint.X, velocityTowardPoint.Y, this.getStandingPosition() - new Vector2(32f, 0.0f), "", "", true, location: this.currentLocation, firer: ((Character) this));
        basicProjectile.height.Value = 48f;
        this.currentLocation.projectiles.Add((Projectile) basicProjectile);
        this.currentLocation.playSound("fireball");
        this.firingTimer = 400f;
        if (this.numFireballsLeft > 0)
          return;
        this.lastFireball = (float) Game1.random.Next(3000, 6500);
      }
      else if (this.withinPlayerThreshold() && (double) this.lastFireball == 0.0 && Game1.random.NextDouble() < 0.01)
      {
        this.IsWalkingTowardPlayer = false;
        Vector2 vector2_4 = new Vector2(this.Position.X, this.Position.Y + 64f);
        this.Halt();
        this.fireballEvent.Fire();
        Vector2 vector2_5 = this.fireballFired();
        Vector2 vector2_6 = vector2_4 + vector2_5;
        this.Sprite.UpdateSourceRect();
        Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer(Utility.Vector2ToPoint(this.getStandingPosition()), 8f, this.Player);
        BasicProjectile basicProjectile = new BasicProjectile(15, 10, 3, 4, 0.0f, velocityTowardPlayer.X, velocityTowardPlayer.Y, this.getStandingPosition() - new Vector2(32f, 0.0f), "", "", true, location: this.currentLocation, firer: ((Character) this));
        basicProjectile.height.Value = 48f;
        this.currentLocation.projectiles.Add((Projectile) basicProjectile);
        this.currentLocation.playSound("fireball");
        this.lastFireball = (float) Game1.random.Next(1200, 3500);
      }
      else
      {
        if ((double) this.lastFireball == 0.0 || Game1.random.NextDouble() >= 0.02)
          return;
        this.Halt();
        if (!this.withinPlayerThreshold())
          return;
        this.Slipperiness = 8;
        this.setTrajectory((int) Utility.getVelocityTowardPlayer(Utility.Vector2ToPoint(this.getStandingPosition()), 8f, this.Player).X, (int) -(double) Utility.getVelocityTowardPlayer(Utility.Vector2ToPoint(this.getStandingPosition()), 8f, this.Player).Y);
      }
    }
  }
}
