// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.RockCrab
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Tools;
using System;

namespace StardewValley.Monsters
{
  public class RockCrab : Monster
  {
    private bool waiter;
    private readonly NetBool shellGone = new NetBool();
    private readonly NetInt shellHealth = new NetInt(5);
    private readonly NetBool isStickBug = new NetBool();

    public RockCrab()
    {
    }

    public RockCrab(Vector2 position)
      : base("Rock Crab", position)
    {
      this.waiter = Game1.random.NextDouble() < 0.4;
      this.moveTowardPlayerThreshold.Value = 3;
    }

    public override void reloadSprite()
    {
      base.reloadSprite();
      this.Sprite.UpdateSourceRect();
    }

    /// <summary>constructor for Lava Crab</summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    public RockCrab(Vector2 position, string name)
      : base(name, position)
    {
      this.waiter = Game1.random.NextDouble() < 0.4;
      this.moveTowardPlayerThreshold.Value = 3;
      if (name.Equals("Iridium Crab"))
      {
        this.waiter = true;
        this.moveTowardPlayerThreshold.Value = 1;
      }
      else
      {
        if (!name.Equals("False Magma Cap"))
          return;
        this.waiter = false;
      }
    }

    public void makeStickBug()
    {
      this.isStickBug.Value = true;
      this.waiter = false;
      this.Name = "Stick Bug";
      this.DamageToFarmer = 20;
      this.MaxHealth = 700;
      this.Health = 700;
      base.reloadSprite();
      this.HideShadow = true;
      this.Sprite.SpriteHeight = 24;
      this.Sprite.UpdateSourceRect();
      this.objectsToDrop.Clear();
      this.objectsToDrop.Add(858);
      while (Game1.random.NextDouble() < 0.5)
        this.objectsToDrop.Add(858);
      this.objectsToDrop.Add(829);
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.shellGone, (INetSerializable) this.shellHealth, (INetSerializable) this.isStickBug);
      this.position.Field.AxisAlignedMovement = true;
    }

    public override bool hitWithTool(Tool t)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.isStickBug)
        return false;
      if (!(t is Pickaxe) || t.getLastFarmerToUse() == null || (int) (NetFieldBase<int, NetInt>) this.shellHealth <= 0)
        return base.hitWithTool(t);
      this.currentLocation.playSound("hammer");
      --this.shellHealth.Value;
      this.shake(500);
      this.waiter = false;
      this.moveTowardPlayerThreshold.Value = 3;
      this.setTrajectory(Utility.getAwayFromPlayerTrajectory(this.GetBoundingBox(), t.getLastFarmerToUse()));
      if ((int) (NetFieldBase<int, NetInt>) this.shellHealth <= 0)
      {
        this.shellGone.Value = true;
        this.moveTowardPlayer(-1);
        this.currentLocation.playSound("stoneCrack");
        Game1.createRadialDebris(this.currentLocation, 14, this.getTileX(), this.getTileY(), Game1.random.Next(2, 7), false);
        Game1.createRadialDebris(this.currentLocation, 14, this.getTileX(), this.getTileY(), Game1.random.Next(2, 7), false);
      }
      return true;
    }

    public override void shedChunks(int number)
    {
      GameLocation currentLocation = this.currentLocation;
      string textureName = (string) (NetFieldBase<string, NetString>) this.Sprite.textureName;
      Rectangle sourcerectangle = new Rectangle(0, 120, 16, 16);
      Rectangle boundingBox = this.GetBoundingBox();
      int x = boundingBox.Center.X;
      boundingBox = this.GetBoundingBox();
      int y1 = boundingBox.Center.Y;
      int numberOfChunks = number;
      int y2 = (int) this.getTileLocation().Y;
      Color white = Color.White;
      double scale = 4.0 * (double) (float) (NetFieldBase<float, NetFloat>) this.scale;
      Game1.createRadialDebris(currentLocation, textureName, sourcerectangle, 8, x, y1, numberOfChunks, y2, white, (float) scale);
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
      if (isBomb)
      {
        this.shellGone.Value = true;
        this.waiter = false;
        this.moveTowardPlayer(-1);
      }
      if (Game1.random.NextDouble() < (double) (NetFieldBase<double, NetDouble>) this.missChance - (double) (NetFieldBase<double, NetDouble>) this.missChance * addedPrecision)
        damage1 = -1;
      else if (this.Sprite.currentFrame % 4 == 0 && !(bool) (NetFieldBase<bool, NetBool>) this.shellGone)
      {
        damage1 = 0;
        this.currentLocation.playSound("crafting");
      }
      else
      {
        this.Health -= damage1;
        this.Slipperiness = 3;
        this.setTrajectory(xTrajectory, yTrajectory);
        this.currentLocation.playSound("hitEnemy");
        this.glowingColor = Color.Cyan;
        if (this.Health <= 0)
        {
          this.currentLocation.playSound("monsterdead");
          this.deathAnimation();
          Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.Red, 10)
          {
            holdLastFrame = true,
            alphaFade = 0.01f
          }, this.currentLocation);
        }
      }
      return damage1;
    }

    public override void update(GameTime time, GameLocation location)
    {
      if (!location.farmers.Any())
        return;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.shellGone && !this.Player.isRafting)
      {
        base.update(time, location);
      }
      else
      {
        if (this.Player.isRafting)
          return;
        if (Game1.IsMasterGame)
          this.behaviorAtGameTick(time);
        this.updateAnimation(time);
      }
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      if (this.waiter && (int) (NetFieldBase<int, NetInt>) this.shellHealth > 4)
      {
        this.moveTowardPlayerThreshold.Value = 0;
      }
      else
      {
        base.behaviorAtGameTick(time);
        if (this.isMoving() && this.Sprite.currentFrame % 4 == 0)
        {
          ++this.Sprite.currentFrame;
          this.Sprite.UpdateSourceRect();
        }
        if (!this.withinPlayerThreshold() && !(bool) (NetFieldBase<bool, NetBool>) this.shellGone)
        {
          this.Halt();
        }
        else
        {
          if (!(bool) (NetFieldBase<bool, NetBool>) this.shellGone)
            return;
          this.updateGlow();
          if (this.invincibleCountdown > 0)
          {
            this.glowingColor = Color.Cyan;
            this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
            if (this.invincibleCountdown <= 0)
              this.stopGlowing();
          }
          if (Math.Abs(this.Player.GetBoundingBox().Center.Y - this.GetBoundingBox().Center.Y) > 192)
          {
            if (this.Player.GetBoundingBox().Center.X - this.GetBoundingBox().Center.X > 0)
              this.SetMovingLeft(true);
            else
              this.SetMovingRight(true);
          }
          else if (this.Player.GetBoundingBox().Center.Y - this.GetBoundingBox().Center.Y > 0)
            this.SetMovingUp(true);
          else
            this.SetMovingDown(true);
          this.MovePosition(time, Game1.viewport, this.currentLocation);
          this.Sprite.CurrentFrame = 16 + this.Sprite.currentFrame % 4;
        }
      }
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
      else
        this.Sprite.StopAnimation();
      if (this.isMoving() && this.Sprite.currentFrame % 4 == 0)
      {
        ++this.Sprite.currentFrame;
        this.Sprite.UpdateSourceRect();
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.shellGone)
        return;
      this.updateGlow();
      if (this.invincibleCountdown > 0)
      {
        this.glowingColor = Color.Cyan;
        this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
        if (this.invincibleCountdown <= 0)
          this.stopGlowing();
      }
      this.Sprite.currentFrame = 16 + this.Sprite.currentFrame % 4;
    }
  }
}
