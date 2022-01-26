// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Grub
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using System;
using System.Xml.Serialization;

namespace StardewValley.Monsters
{
  public class Grub : Monster
  {
    public const int healthToRunAway = 8;
    private readonly NetBool leftDrift = new NetBool();
    private readonly NetBool pupating = new NetBool();
    [XmlElement("hard")]
    public readonly NetBool hard = new NetBool();
    private int metamorphCounter = 2000;
    private readonly NetFloat targetRotation = new NetFloat();

    public Grub()
    {
    }

    public Grub(Vector2 position)
      : this(position, false)
    {
    }

    public Grub(Vector2 position, bool hard)
      : base(nameof (Grub), position)
    {
      if (Game1.random.NextDouble() < 0.5)
        this.leftDrift.Value = true;
      this.FacingDirection = Game1.random.Next(4);
      this.targetRotation.Value = this.rotation = (float) Game1.random.Next(4) / 3.141593f;
      this.hard.Value = hard;
      if (!hard)
        return;
      this.DamageToFarmer *= 3;
      this.Health *= 5;
      this.MaxHealth = this.Health;
      this.ExperienceGained *= 3;
      if (Game1.random.NextDouble() >= 0.1)
        return;
      this.objectsToDrop.Add(456);
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.leftDrift, (INetSerializable) this.pupating, (INetSerializable) this.hard, (INetSerializable) this.targetRotation);
      this.position.Field.AxisAlignedMovement = true;
    }

    public override void reloadSprite()
    {
      base.reloadSprite();
      this.Sprite.SpriteHeight = 24;
      this.Sprite.UpdateSourceRect();
    }

    public void setHard()
    {
      this.hard.Value = true;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.hard)
        return;
      this.DamageToFarmer = 12;
      this.Health = 100;
      this.MaxHealth = this.Health;
      this.ExperienceGained = 10;
      if (Game1.random.NextDouble() >= 0.1)
        return;
      this.objectsToDrop.Add(456);
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
        this.currentLocation.playSound("slimeHit");
        if ((bool) (NetFieldBase<bool, NetBool>) this.pupating)
        {
          this.currentLocation.playSound("crafting");
          this.setTrajectory(xTrajectory / 2, yTrajectory / 2);
          return 0;
        }
        this.Slipperiness = 4;
        this.Health -= damage1;
        this.setTrajectory(xTrajectory, yTrajectory);
        if (this.Health <= 0)
        {
          this.currentLocation.playSound("slimedead");
          Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, (bool) (NetFieldBase<bool, NetBool>) this.isHardModeMonster ? Color.LimeGreen : Color.Orange, 10)
          {
            holdLastFrame = true,
            alphaFade = 0.01f,
            interval = 50f
          }, this.currentLocation);
        }
      }
      return damage1;
    }

    public override void defaultMovementBehavior(GameTime time) => this.Scale = (float) (1.0 + 0.125 * Math.Sin(time.TotalGameTime.TotalMilliseconds / (500.0 + (double) this.Position.X / 100.0)));

    public override void BuffForAdditionalDifficulty(int additional_difficulty)
    {
      base.BuffForAdditionalDifficulty(additional_difficulty);
      this.rotation = 0.0f;
      this.targetRotation.Value = 0.0f;
    }

    public override void update(GameTime time, GameLocation location)
    {
      if ((this.Health > 8 || (bool) (NetFieldBase<bool, NetBool>) this.hard && this.Health >= this.MaxHealth) && !(bool) (NetFieldBase<bool, NetBool>) this.pupating)
      {
        base.update(time, location);
      }
      else
      {
        if (this.invincibleCountdown > 0)
        {
          this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
          if (this.invincibleCountdown <= 0)
            this.stopGlowing();
        }
        if (Game1.IsMasterGame)
          this.behaviorAtGameTick(time);
        this.updateAnimation(time);
      }
    }

    public override void draw(SpriteBatch b) => b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.Sprite.SpriteWidth * 4 / 2), (float) (this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(this.Sprite.SourceRect), (bool) (NetFieldBase<bool, NetBool>) this.hard ? Color.Lime : Color.White, this.rotation, new Vector2((float) (this.Sprite.SpriteWidth / 2), (float) ((double) this.Sprite.SpriteHeight * 3.0 / 4.0)), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip || this.Sprite.CurrentAnimation != null && this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));

    protected override void updateMonsterSlaveAnimation(GameTime time)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.pupating)
      {
        this.Scale = (float) (1.0 + Math.Sin((double) time.TotalGameTime.Milliseconds * 0.392699092626572) / 12.0);
        this.metamorphCounter -= time.ElapsedGameTime.Milliseconds;
      }
      else if (this.Health <= 8 || (bool) (NetFieldBase<bool, NetBool>) this.hard && this.Health < this.MaxHealth)
      {
        this.metamorphCounter -= time.ElapsedGameTime.Milliseconds;
        if (this.metamorphCounter > 0)
          return;
        this.Sprite.Animate(time, 16, 4, 125f);
        if (this.Sprite.currentFrame != 19)
          return;
        this.metamorphCounter = 4500;
      }
      else if (this.isMoving())
      {
        if (this.FacingDirection == 0)
          this.Sprite.AnimateUp(time);
        else if (this.FacingDirection == 3)
          this.Sprite.AnimateLeft(time);
        else if (this.FacingDirection == 1)
          this.Sprite.AnimateRight(time);
        else if (this.FacingDirection == 2)
          this.Sprite.AnimateDown(time);
        this.rotation = 0.0f;
        this.Scale = 1f;
      }
      else
      {
        if (this.withinPlayerThreshold())
          return;
        this.Halt();
        this.rotation = (float) (NetFieldBase<float, NetFloat>) this.targetRotation;
      }
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      base.behaviorAtGameTick(time);
      if ((bool) (NetFieldBase<bool, NetBool>) this.pupating)
      {
        this.Scale = (float) (1.0 + Math.Sin((double) time.TotalGameTime.Milliseconds * 0.392699092626572) / 12.0);
        this.metamorphCounter -= time.ElapsedGameTime.Milliseconds;
        if (this.metamorphCounter > 0)
          return;
        this.Health = -500;
        Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(208, 424, 32, 40), 4, this.getStandingX(), this.getStandingY(), 25, (int) this.getTileLocation().Y);
        Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(208, 424, 32, 40), 8, this.getStandingX(), this.getStandingY(), 15, (int) this.getTileLocation().Y);
        if (this.currentLocation is MineShaft)
        {
          NetCollection<NPC> characters = this.currentLocation.characters;
          MineShaft currentLocation = this.currentLocation as MineShaft;
          Fly fly = new Fly(this.Position, (bool) (NetFieldBase<bool, NetBool>) this.hard);
          fly.currentLocation = this.currentLocation;
          Monster monster = currentLocation.BuffMonsterIfNecessary((Monster) fly);
          characters.Add((NPC) monster);
        }
        else
        {
          NetCollection<NPC> characters = this.currentLocation.characters;
          Fly fly = new Fly(this.Position, (bool) (NetFieldBase<bool, NetBool>) this.hard);
          fly.currentLocation = this.currentLocation;
          characters.Add((NPC) fly);
        }
      }
      else if (this.Health <= this.MaxHealth / 2 - 2 || (bool) (NetFieldBase<bool, NetBool>) this.hard && this.Health < this.MaxHealth)
      {
        this.metamorphCounter -= time.ElapsedGameTime.Milliseconds;
        if (this.metamorphCounter <= 0)
        {
          this.Sprite.Animate(time, 16, 4, 125f);
          if (this.Sprite.currentFrame != 19)
            return;
          this.pupating.Value = true;
          this.metamorphCounter = 4500;
        }
        else
        {
          if (Math.Abs(this.Player.GetBoundingBox().Center.Y - this.GetBoundingBox().Center.Y) > 128)
          {
            if (this.Player.GetBoundingBox().Center.X > this.GetBoundingBox().Center.X)
              this.SetMovingLeft(true);
            else
              this.SetMovingRight(true);
          }
          else if (Math.Abs(this.Player.GetBoundingBox().Center.X - this.GetBoundingBox().Center.X) > 128)
          {
            if (this.Player.GetBoundingBox().Center.Y > this.GetBoundingBox().Center.Y)
              this.SetMovingUp(true);
            else
              this.SetMovingDown(true);
          }
          this.MovePosition(time, Game1.viewport, this.currentLocation);
        }
      }
      else if (this.withinPlayerThreshold())
      {
        this.Scale = 1f;
        this.rotation = 0.0f;
      }
      else
      {
        if (!this.isMoving())
          return;
        this.Halt();
        this.faceDirection(Game1.random.Next(4));
        this.targetRotation.Value = this.rotation = (float) Game1.random.Next(4) / 3.141593f;
      }
    }
  }
}
