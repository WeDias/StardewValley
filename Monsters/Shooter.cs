// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Shooter
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Projectiles;
using System;
using System.Xml.Serialization;

namespace StardewValley.Monsters
{
  public class Shooter : Monster
  {
    public NetBool shooting = new NetBool();
    public int shotsLeft;
    public float nextShot;
    public int projectileSpeed = 12;
    public int projectileDebuff = 26;
    public int numberOfShotsPerFire = 1;
    public float aimTime = 0.25f;
    public float burstTime = 0.25f;
    public float aimEndTime = 1f;
    public int firedProjectile = 12;
    public string damageSound = "shadowHit";
    public string fireSound = "Cowboy_gunshot";
    public int projectileRange = 10;
    public int desiredDistance = 5;
    public int fireRange = 8;
    [XmlIgnore]
    public NetEvent0 fireEvent = new NetEvent0();

    public Shooter()
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.shooting, (INetSerializable) this.fireEvent);
      this.fireEvent.onEvent += new NetEvent0.Event(this.OnFire);
    }

    public override int GetBaseDifficultyLevel() => 1;

    public virtual void OnFire() => this.shakeTimer = 250;

    public override bool ShouldActuallyMoveAwayFromPlayer() => this.Player != null && Math.Abs(this.Player.getTileX() - this.getTileX()) < this.desiredDistance && Math.Abs(this.Player.getTileY() - this.getTileY()) < this.desiredDistance || base.ShouldActuallyMoveAwayFromPlayer();

    public override Rectangle GetBoundingBox() => base.GetBoundingBox();

    public Shooter(Vector2 position)
      : base("Shadow Sniper", position)
    {
      this.Sprite.SpriteHeight = 32;
      this.Sprite.SpriteWidth = 32;
      this.forceOneTileWide.Value = true;
      this.Sprite.UpdateSourceRect();
      this.InitializeVariant();
    }

    public Shooter(Vector2 position, string monster_name)
      : base(monster_name, position)
    {
      this.Sprite.SpriteHeight = 32;
      this.Sprite.SpriteWidth = 32;
      this.forceOneTileWide.Value = true;
      this.Sprite.UpdateSourceRect();
      this.InitializeVariant();
    }

    public virtual void InitializeVariant()
    {
      if (!(this.Name == "Shadow Sniper"))
      {
        int num = this.Name == "Skeleton Gunner" ? 1 : 0;
      }
      this.nextShot = 1f;
    }

    public override void reloadSprite()
    {
      this.Sprite = new AnimatedSprite("Characters\\Monsters\\" + this.Name);
      this.Sprite.SpriteHeight = 32;
      this.Sprite.UpdateSourceRect();
    }

    protected override void updateAnimation(GameTime time)
    {
      if (this.shooting.Value)
      {
        if (this.FacingDirection == 2)
          this.Sprite.CurrentFrame = 16;
        else if (this.FacingDirection == 1)
          this.Sprite.CurrentFrame = 17;
        else if (this.FacingDirection == 0)
          this.Sprite.CurrentFrame = 18;
        else if (this.FacingDirection == 3)
          this.Sprite.CurrentFrame = 19;
      }
      if (Game1.IsMasterGame || !this.isMoving())
        return;
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

    public override void behaviorAtGameTick(GameTime time)
    {
      if (!this.shooting.Value)
      {
        if ((double) this.nextShot > 0.0)
          this.nextShot -= (float) time.ElapsedGameTime.TotalSeconds;
        else if (this.Player != null)
        {
          int tileX1 = this.Player.getTileX();
          int tileY1 = this.Player.getTileY();
          int tileX2 = this.getTileX();
          int tileY2 = this.getTileY();
          if (Math.Abs(tileX1 - tileX2) <= this.fireRange && Math.Abs(tileY1 - tileY2) <= this.fireRange && (Math.Abs(tileX1 - tileX2) < 2 || Math.Abs(tileY1 - tileY2) < 2))
          {
            this.Halt();
            this.faceGeneralDirection(this.Player.getStandingPosition());
            this.shooting.Value = true;
            this.nextShot = this.aimTime;
            this.shotsLeft = this.numberOfShotsPerFire;
          }
        }
      }
      else
      {
        this.xVelocity = 0.0f;
        this.yVelocity = 0.0f;
        if (this.shotsLeft > 0)
        {
          if ((double) this.nextShot > 0.0)
          {
            this.nextShot -= (float) time.ElapsedGameTime.TotalSeconds;
            if ((double) this.nextShot <= 0.0)
            {
              Vector2 vector2_1 = Vector2.Zero;
              float num = 0.0f;
              if ((int) this.facingDirection == 0)
              {
                vector2_1 = new Vector2(0.0f, -1f);
                num = 0.0f;
              }
              if ((int) this.facingDirection == 3)
              {
                vector2_1 = new Vector2(-1f, 0.0f);
                num = -1.570796f;
              }
              if ((int) this.facingDirection == 1)
              {
                vector2_1 = new Vector2(1f, 0.0f);
                num = 1.570796f;
              }
              if ((int) this.facingDirection == 2)
              {
                vector2_1 = new Vector2(0.0f, 1f);
                num = 3.141593f;
              }
              Vector2 vector2_2 = vector2_1 * (float) this.projectileSpeed;
              this.fireEvent.Fire();
              this.currentLocation.playSound(this.fireSound);
              BasicProjectile basicProjectile = new BasicProjectile(this.DamageToFarmer, this.firedProjectile, 0, 0, 0.0f, vector2_2.X, vector2_2.Y, this.Position, "", "", false, location: this.currentLocation, firer: ((Character) this));
              basicProjectile.startingRotation.Value = num;
              basicProjectile.height.Value = 24f;
              basicProjectile.debuff.Value = this.projectileDebuff;
              basicProjectile.ignoreTravelGracePeriod.Value = true;
              basicProjectile.IgnoreLocationCollision = true;
              basicProjectile.maxTravelDistance.Value = 64 * this.projectileRange;
              this.currentLocation.projectiles.Add((Projectile) basicProjectile);
              --this.shotsLeft;
              this.nextShot = this.shotsLeft != 0 ? this.burstTime : this.aimEndTime;
            }
          }
        }
        else if ((double) this.nextShot > 0.0)
        {
          this.nextShot -= (float) time.ElapsedGameTime.TotalSeconds;
        }
        else
        {
          this.shooting.Value = false;
          this.nextShot = 2f;
        }
      }
      base.behaviorAtGameTick(time);
    }

    public override void updateMovement(GameLocation location, GameTime time)
    {
      if (this.shooting.Value)
        this.MovePosition(time, Game1.viewport, location);
      else
        base.updateMovement(location, time);
    }

    public override int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      Farmer who)
    {
      this.shooting.Value = false;
      this.shotsLeft = 0;
      this.nextShot = Math.Max(0.5f, this.nextShot);
      this.currentLocation.playSound(this.damageSound);
      return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
    }

    protected override void localDeathAnimation()
    {
      if (!(this.Name == "Shadow Sniper"))
        return;
      Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(45, this.Position, Color.White, 10), this.currentLocation);
      for (int index = 1; index < 3; ++index)
      {
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(0.0f, 1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(0.0f, -1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(1f, 0.0f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(-1f, 0.0f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
      }
      this.currentLocation.localSound("shadowDie");
    }

    protected override void sharedDeathAnimation()
    {
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, 16, 5), 16, this.getStandingX(), this.getStandingY() - 32, 1, this.getStandingY() / 64, Color.White, 4f);
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(this.Sprite.SourceRect.X + 2, this.Sprite.SourceRect.Y + 5, 16, 5), 10, this.getStandingX(), this.getStandingY() - 32, 1, this.getStandingY() / 64, Color.White, 4f);
    }

    public override void update(GameTime time, GameLocation location)
    {
      base.update(time, location);
      this.fireEvent.Poll();
    }
  }
}
