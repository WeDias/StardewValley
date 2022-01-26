// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.LavaCrab
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System;

namespace StardewValley.Monsters
{
  public class LavaCrab : Monster
  {
    public LavaCrab()
    {
    }

    public LavaCrab(Vector2 position)
      : base("Lava Crab", position)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.position.Field.AxisAlignedMovement = true;
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
        damage1 = -1;
      else if (this.Sprite.currentFrame % 4 == 0)
      {
        damage1 = 0;
        this.currentLocation.playSound("crafting");
      }
      else
      {
        this.Health -= damage1;
        this.currentLocation.playSound("hitEnemy");
        this.setTrajectory(xTrajectory, yTrajectory);
        if (this.Health <= 0)
        {
          Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(44, this.Position, Color.Purple, 10));
          this.currentLocation.playSound("monsterdead");
        }
      }
      return damage1;
    }

    protected override void updateAnimation(GameTime time)
    {
      if (this.isMoving() || this.withinPlayerThreshold())
      {
        if (this.FacingDirection == 0)
          this.Sprite.AnimateUp(time);
        else if (this.FacingDirection == 3)
          this.Sprite.AnimateLeft(time);
        else if (this.FacingDirection == 1)
          this.Sprite.AnimateRight(time);
        else if (this.FacingDirection == 2)
          this.Sprite.AnimateDown(time);
        if (this.Sprite.currentFrame % 4 == 0)
        {
          ++this.Sprite.currentFrame;
          this.Sprite.UpdateSourceRect();
        }
      }
      else
        this.Sprite.StopAnimation();
      this.resetAnimationSpeed();
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      base.behaviorAtGameTick(time);
      if (this.withinPlayerThreshold())
        return;
      this.Halt();
    }
  }
}
