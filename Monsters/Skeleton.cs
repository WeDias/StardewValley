// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Skeleton
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace StardewValley.Monsters
{
  public class Skeleton : Monster
  {
    private bool spottedPlayer;
    private readonly NetBool throwing = new NetBool();
    public readonly NetBool isMage = new NetBool();
    private int controllerAttemptTimer;

    public Skeleton()
    {
    }

    public Skeleton(Vector2 position, bool isMage = false)
      : base(nameof (Skeleton), position, Game1.random.Next(4))
    {
      this.isMage.Value = isMage;
      this.reloadSprite();
      this.Sprite.SpriteHeight = 32;
      this.Sprite.UpdateSourceRect();
      this.IsWalkingTowardPlayer = false;
      this.jitteriness.Value = 0.0;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.throwing, (INetSerializable) this.isMage);
      this.position.Field.AxisAlignedMovement = true;
    }

    public override void reloadSprite()
    {
      this.Sprite = new AnimatedSprite("Characters\\Monsters\\Skeleton" + ((bool) (NetFieldBase<bool, NetBool>) this.isMage ? " Mage" : ""));
      this.Sprite.SpriteHeight = 32;
      this.Sprite.UpdateSourceRect();
    }

    public override List<Item> getExtraDropItems()
    {
      List<Item> extraDropItems = new List<Item>();
      if (Game1.random.NextDouble() < 0.04)
        extraDropItems.Add((Item) new MeleeWeapon(5));
      return extraDropItems;
    }

    public override int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      Farmer who)
    {
      this.currentLocation.playSound("skeletonHit");
      this.Slipperiness = 3;
      if ((bool) (NetFieldBase<bool, NetBool>) this.throwing)
      {
        this.throwing.Value = false;
        this.Halt();
      }
      if (this.Health - damage <= 0)
      {
        Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(46, this.Position, Color.White, 10, animationInterval: 70f));
        Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(46, this.Position + new Vector2(-16f, 0.0f), Color.White, 10, animationInterval: 70f)
        {
          delayBeforeAnimationStart = 100
        });
        Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(46, this.Position + new Vector2(16f, 0.0f), Color.White, 10, animationInterval: 70f)
        {
          delayBeforeAnimationStart = 200
        });
      }
      return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
    }

    public override void shedChunks(int number)
    {
      GameLocation currentLocation = this.currentLocation;
      string textureName = (string) (NetFieldBase<string, NetString>) this.Sprite.textureName;
      Rectangle sourcerectangle = new Rectangle(0, 128, 16, 16);
      Rectangle boundingBox = this.GetBoundingBox();
      int x = boundingBox.Center.X;
      boundingBox = this.GetBoundingBox();
      int y1 = boundingBox.Center.Y;
      int numberOfChunks = number;
      int y2 = (int) this.getTileLocation().Y;
      Color white = Color.White;
      Game1.createRadialDebris(currentLocation, textureName, sourcerectangle, 8, x, y1, numberOfChunks, y2, white, 4f);
    }

    public override void BuffForAdditionalDifficulty(int additional_difficulty)
    {
      base.BuffForAdditionalDifficulty(additional_difficulty);
      if ((bool) (NetFieldBase<bool, NetBool>) this.isMage)
        return;
      this.MaxHealth += 300;
      this.Health += 300;
    }

    protected override void sharedDeathAnimation()
    {
      this.currentLocation.playSound("skeletonDie");
      Game1.random.Next(5, 13);
      this.shedChunks(20);
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(3, Game1.random.NextDouble() < 0.5 ? 3 : 35, 10, 10), 11, this.GetBoundingBox().Center.X, this.GetBoundingBox().Center.Y, 1, (int) this.getTileLocation().Y, Color.White, 4f);
    }

    public override void update(GameTime time, GameLocation location)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.throwing)
      {
        base.update(time, location);
      }
      else
      {
        if (Game1.IsMasterGame)
          this.behaviorAtGameTick(time);
        this.updateAnimation(time);
      }
    }

    protected override void updateMonsterSlaveAnimation(GameTime time)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.throwing)
      {
        if (this.invincibleCountdown > 0)
        {
          this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
          if (this.invincibleCountdown <= 0)
            this.stopGlowing();
        }
        this.Sprite.Animate(time, 20, 5, 150f);
        if (this.Sprite.currentFrame != 24)
          return;
        this.Sprite.currentFrame = 23;
      }
      else if (this.isMoving())
      {
        if (this.FacingDirection == 0)
          this.Sprite.AnimateUp(time);
        else if (this.FacingDirection == 3)
          this.Sprite.AnimateLeft(time);
        else if (this.FacingDirection == 1)
        {
          this.Sprite.AnimateRight(time);
        }
        else
        {
          if (this.FacingDirection != 2)
            return;
          this.Sprite.AnimateDown(time);
        }
      }
      else
        this.Sprite.StopAnimation();
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.throwing)
        base.behaviorAtGameTick(time);
      TimeSpan elapsedGameTime;
      if (!this.spottedPlayer && !this.wildernessFarmMonster && Utility.doesPointHaveLineOfSightInMine(this.currentLocation, this.getTileLocation(), this.Player.getTileLocation(), 8))
      {
        this.controller = new PathFindController((Character) this, this.currentLocation, new Point(this.Player.getStandingX() / 64, this.Player.getStandingY() / 64), -1, (PathFindController.endBehavior) null, 200);
        this.spottedPlayer = true;
        if (this.controller == null || this.controller.pathToEndPoint == null || this.controller.pathToEndPoint.Count == 0)
        {
          this.Halt();
          this.facePlayer(this.Player);
        }
        this.currentLocation.playSound("skeletonStep");
        this.IsWalkingTowardPlayer = true;
      }
      else if ((bool) (NetFieldBase<bool, NetBool>) this.throwing)
      {
        if (this.invincibleCountdown > 0)
        {
          int invincibleCountdown = this.invincibleCountdown;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds = elapsedGameTime.Milliseconds;
          this.invincibleCountdown = invincibleCountdown - milliseconds;
          if (this.invincibleCountdown <= 0)
            this.stopGlowing();
        }
        this.Sprite.Animate(time, 20, 5, 150f);
        if (this.Sprite.currentFrame == 24)
        {
          this.throwing.Value = false;
          this.Sprite.currentFrame = 0;
          this.faceDirection(2);
          Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer(new Point((int) this.Position.X, (int) this.Position.Y), 8f, this.Player);
          if (this.isMage.Value)
          {
            if (Game1.random.NextDouble() < 0.5)
              this.currentLocation.projectiles.Add((Projectile) new DebuffingProjectile(19, 14, 4, 4, 0.1963495f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2(this.Position.X, this.Position.Y), this.currentLocation, (Character) this));
            else
              this.currentLocation.projectiles.Add((Projectile) new BasicProjectile(this.DamageToFarmer * 2, 9, 0, 4, 0.0f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2(this.Position.X, this.Position.Y), "flameSpellHit", "flameSpell", false, location: this.currentLocation, firer: ((Character) this)));
          }
          else
            this.currentLocation.projectiles.Add((Projectile) new BasicProjectile(this.DamageToFarmer, 4, 0, 0, 0.1963495f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2(this.Position.X, this.Position.Y), "skeletonHit", "skeletonStep", false, location: this.currentLocation, firer: ((Character) this)));
        }
      }
      else if (this.spottedPlayer && this.controller == null && Game1.random.NextDouble() < ((bool) (NetFieldBase<bool, NetBool>) this.isMage ? 0.008 : 0.002) && !this.wildernessFarmMonster && Utility.doesPointHaveLineOfSightInMine(this.currentLocation, this.getTileLocation(), this.Player.getTileLocation(), 8))
      {
        this.throwing.Value = true;
        this.Halt();
        this.Sprite.currentFrame = 20;
        this.shake(750);
      }
      else if (this.withinPlayerThreshold(2))
        this.controller = (PathFindController) null;
      else if (this.spottedPlayer && this.controller == null && this.controllerAttemptTimer <= 0)
      {
        this.controller = new PathFindController((Character) this, this.currentLocation, new Point(this.Player.getStandingX() / 64, this.Player.getStandingY() / 64), -1, (PathFindController.endBehavior) null, 200);
        this.controllerAttemptTimer = this.wildernessFarmMonster ? 2000 : 1000;
        if (this.controller == null || this.controller.pathToEndPoint == null || this.controller.pathToEndPoint.Count == 0)
          this.Halt();
      }
      else if (this.wildernessFarmMonster)
      {
        this.spottedPlayer = true;
        this.IsWalkingTowardPlayer = true;
      }
      int controllerAttemptTimer = this.controllerAttemptTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds1 = elapsedGameTime.Milliseconds;
      this.controllerAttemptTimer = controllerAttemptTimer - milliseconds1;
    }
  }
}
