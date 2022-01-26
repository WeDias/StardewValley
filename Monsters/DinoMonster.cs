// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.DinoMonster
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;

namespace StardewValley.Monsters
{
  public class DinoMonster : Monster
  {
    public int timeUntilNextAttack;
    protected bool _hasPlayedFireSound;
    public readonly NetBool firing = new NetBool(false);
    public NetInt attackState = new NetInt();
    public int nextFireTime;
    public int totalFireTime;
    public int nextChangeDirectionTime;
    public int nextWanderTime;
    public bool wanderState;

    public DinoMonster()
    {
    }

    public DinoMonster(Vector2 position)
      : base("Pepper Rex", position)
    {
      this.Sprite.SpriteWidth = 32;
      this.Sprite.SpriteHeight = 32;
      this.Sprite.UpdateSourceRect();
      this.timeUntilNextAttack = 2000;
      this.nextChangeDirectionTime = Game1.random.Next(1000, 3000);
      this.nextWanderTime = Game1.random.Next(1000, 2000);
    }

    protected override void initNetFields()
    {
      this.NetFields.AddFields((INetSerializable) this.attackState, (INetSerializable) this.firing);
      base.initNetFields();
    }

    public override void reloadSprite()
    {
      base.reloadSprite();
      this.Sprite.SpriteWidth = 32;
      this.Sprite.SpriteHeight = 32;
      this.Sprite.UpdateSourceRect();
    }

    public override void draw(SpriteBatch b)
    {
      if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128))
        return;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(56f, (float) (16 + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), Color.White, this.rotation, new Vector2(16f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));
      if (!this.isGlowing)
        return;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(56f, (float) (16 + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, 0.0f, new Vector2(16f, 16f), 4f * Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale), this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) ((double) this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
    }

    public override Rectangle GetBoundingBox()
    {
      Vector2 position = this.Position;
      return new Rectangle((int) position.X + 8, (int) position.Y, this.Sprite.SpriteWidth * 4 * 3 / 4, 64);
    }

    public override List<Item> getExtraDropItems()
    {
      List<Item> extraDropItems = new List<Item>();
      if (Game1.random.NextDouble() < 0.100000001490116)
        extraDropItems.Add((Item) new StardewValley.Object(107, 1));
      else
        extraDropItems.Add(Utility.GetRandom<Item>(new List<Item>()
        {
          (Item) new StardewValley.Object(580, 1),
          (Item) new StardewValley.Object(583, 1),
          (Item) new StardewValley.Object(584, 1)
        }));
      return extraDropItems;
    }

    protected override void sharedDeathAnimation()
    {
      this.currentLocation.playSound("skeletonDie");
      this.currentLocation.playSound("grunt");
      for (int index = 0; index < 16; ++index)
      {
        GameLocation currentLocation = this.currentLocation;
        string textureName = (string) (NetFieldBase<string, NetString>) this.Sprite.textureName;
        Rectangle sourcerectangle = new Rectangle(64, 128, 16, 16);
        Rectangle boundingBox = this.GetBoundingBox();
        double left = (double) boundingBox.Left;
        boundingBox = this.GetBoundingBox();
        double right = (double) boundingBox.Right;
        double t1 = Game1.random.NextDouble();
        int xPosition = (int) Utility.Lerp((float) left, (float) right, (float) t1);
        boundingBox = this.GetBoundingBox();
        double bottom = (double) boundingBox.Bottom;
        boundingBox = this.GetBoundingBox();
        double top = (double) boundingBox.Top;
        double t2 = Game1.random.NextDouble();
        int yPosition = (int) Utility.Lerp((float) bottom, (float) top, (float) t2);
        int y = (int) this.getTileLocation().Y;
        Color white = Color.White;
        Game1.createRadialDebris(currentLocation, textureName, sourcerectangle, 16, xPosition, yPosition, 1, y, white, 4f);
      }
    }

    protected override void localDeathAnimation() => Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.HotPink, 10)
    {
      holdLastFrame = true,
      alphaFade = 0.01f,
      interval = 70f
    }, this.currentLocation, 8, 96);

    public override void behaviorAtGameTick(GameTime time)
    {
      TimeSpan elapsedGameTime;
      if (this.attackState.Value == 1)
      {
        this.IsWalkingTowardPlayer = false;
        this.Halt();
      }
      else if (this.withinPlayerThreshold())
      {
        this.IsWalkingTowardPlayer = true;
      }
      else
      {
        this.IsWalkingTowardPlayer = false;
        int changeDirectionTime = this.nextChangeDirectionTime;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds1 = elapsedGameTime.Milliseconds;
        this.nextChangeDirectionTime = changeDirectionTime - milliseconds1;
        int nextWanderTime = this.nextWanderTime;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds2 = elapsedGameTime.Milliseconds;
        this.nextWanderTime = nextWanderTime - milliseconds2;
        if (this.nextChangeDirectionTime < 0)
        {
          this.nextChangeDirectionTime = Game1.random.Next(500, 1000);
          int facingDirection = this.FacingDirection;
          this.facingDirection.Value = (this.facingDirection.Value + (Game1.random.Next(0, 3) - 1) + 4) % 4;
        }
        if (this.nextWanderTime < 0)
        {
          this.nextWanderTime = !this.wanderState ? Game1.random.Next(1000, 3000) : Game1.random.Next(1000, 2000);
          this.wanderState = !this.wanderState;
        }
        if (this.wanderState)
        {
          this.moveLeft = this.moveUp = this.moveRight = this.moveDown = false;
          this.tryToMoveInDirection(this.facingDirection.Value, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider);
        }
      }
      int timeUntilNextAttack = this.timeUntilNextAttack;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds3 = elapsedGameTime.Milliseconds;
      this.timeUntilNextAttack = timeUntilNextAttack - milliseconds3;
      if (this.attackState.Value == 0 && this.withinPlayerThreshold(2))
      {
        this.firing.Set(false);
        if (this.timeUntilNextAttack >= 0)
          return;
        this.timeUntilNextAttack = 0;
        this.attackState.Set(1);
        this.nextFireTime = 500;
        this.totalFireTime = 3000;
        this.currentLocation.playSound("croak");
      }
      else
      {
        if (this.totalFireTime <= 0)
          return;
        if (!(bool) (NetFieldBase<bool, NetBool>) this.firing)
        {
          Farmer player = this.Player;
          if (player != null)
            this.faceGeneralDirection(player.Position);
        }
        int totalFireTime = this.totalFireTime;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds4 = elapsedGameTime.Milliseconds;
        this.totalFireTime = totalFireTime - milliseconds4;
        if (this.nextFireTime > 0)
        {
          int nextFireTime = this.nextFireTime;
          elapsedGameTime = time.ElapsedGameTime;
          int milliseconds5 = elapsedGameTime.Milliseconds;
          this.nextFireTime = nextFireTime - milliseconds5;
          if (this.nextFireTime <= 0)
          {
            if (!this.firing.Value)
            {
              this.firing.Set(true);
              this.currentLocation.playSound("furnace");
            }
            float num1 = 0.0f;
            Vector2 startingPosition = new Vector2((float) this.GetBoundingBox().Center.X - 32f, (float) this.GetBoundingBox().Center.Y - 32f);
            switch (this.facingDirection.Value)
            {
              case 0:
                this.yVelocity = -1f;
                startingPosition.Y -= 64f;
                num1 = 90f;
                break;
              case 1:
                this.xVelocity = -1f;
                startingPosition.X += 64f;
                num1 = 0.0f;
                break;
              case 2:
                this.yVelocity = 1f;
                num1 = 270f;
                break;
              case 3:
                this.xVelocity = 1f;
                startingPosition.X -= 64f;
                num1 = 180f;
                break;
            }
            float num2 = num1 + (float) Math.Sin((double) this.totalFireTime / 1000.0 * 180.0 * Math.PI / 180.0) * 25f;
            Vector2 vector2 = new Vector2((float) Math.Cos((double) num2 * Math.PI / 180.0), -(float) Math.Sin((double) num2 * Math.PI / 180.0)) * 10f;
            BasicProjectile basicProjectile = new BasicProjectile(25, 10, 0, 1, 0.1963495f, vector2.X, vector2.Y, startingPosition, "", "", false, location: this.currentLocation, firer: ((Character) this));
            basicProjectile.ignoreTravelGracePeriod.Value = true;
            basicProjectile.maxTravelDistance.Value = 256;
            this.currentLocation.projectiles.Add((Projectile) basicProjectile);
            this.nextFireTime = 50;
          }
        }
        if (this.totalFireTime > 0)
          return;
        this.totalFireTime = 0;
        this.nextFireTime = 0;
        this.attackState.Set(0);
        this.timeUntilNextAttack = Game1.random.Next(1000, 2000);
      }
    }

    protected override void updateAnimation(GameTime time)
    {
      int num = 0;
      if (this.FacingDirection == 2)
        num = 0;
      else if (this.FacingDirection == 1)
        num = 4;
      else if (this.FacingDirection == 0)
        num = 8;
      else if (this.FacingDirection == 3)
        num = 12;
      if (this.attackState.Value == 1)
      {
        if (this.firing.Value)
          this.Sprite.CurrentFrame = 16 + num;
        else
          this.Sprite.CurrentFrame = 17 + num;
      }
      else if (this.isMoving() || this.wanderState)
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
      {
        if (this.FacingDirection == 0)
          this.Sprite.AnimateUp(time);
        else if (this.FacingDirection == 3)
          this.Sprite.AnimateLeft(time);
        else if (this.FacingDirection == 1)
          this.Sprite.AnimateRight(time);
        else if (this.FacingDirection == 2)
          this.Sprite.AnimateDown(time);
        this.Sprite.StopAnimation();
      }
    }

    protected override void updateMonsterSlaveAnimation(GameTime time)
    {
      int num = 0;
      if (this.FacingDirection == 2)
        num = 0;
      else if (this.FacingDirection == 1)
        num = 4;
      else if (this.FacingDirection == 0)
        num = 8;
      else if (this.FacingDirection == 3)
        num = 12;
      if (this.attackState.Value == 1)
      {
        if (this.firing.Value)
          this.Sprite.CurrentFrame = 16 + num;
        else
          this.Sprite.CurrentFrame = 17 + num;
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

    public enum AttackState
    {
      None,
      Fireball,
      Charge,
    }
  }
}
