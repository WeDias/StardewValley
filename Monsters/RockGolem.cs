// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.RockGolem
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace StardewValley.Monsters
{
  public class RockGolem : Monster
  {
    private readonly NetBool seenPlayer = new NetBool();

    public RockGolem()
    {
    }

    public RockGolem(Vector2 position)
      : base("Stone Golem", position)
    {
      this.IsWalkingTowardPlayer = false;
      this.Slipperiness = 2;
      this.jitteriness.Value = 0.0;
      this.HideShadow = true;
    }

    public RockGolem(Vector2 position, MineShaft mineArea)
      : this(position)
    {
      int mineLevel = mineArea.mineLevel;
      if (mineLevel > 80)
      {
        this.DamageToFarmer *= 2;
        this.Health = (int) ((double) this.Health * 2.5);
      }
      else
      {
        if (mineLevel <= 40)
          return;
        this.DamageToFarmer = (int) ((double) this.DamageToFarmer * 1.5);
        this.Health = (int) ((double) this.Health * 1.75);
      }
    }

    /// <summary>
    /// constructor for wilderness golems that spawn on combat farm.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="difficultyMod">player combat level is good</param>
    public RockGolem(Vector2 position, int difficultyMod)
      : base("Wilderness Golem", position)
    {
      this.IsWalkingTowardPlayer = false;
      this.Slipperiness = 3;
      this.HideShadow = true;
      this.jitteriness.Value = 0.0;
      this.DamageToFarmer += difficultyMod;
      this.Health += (int) ((double) (difficultyMod * difficultyMod) * 2.0);
      this.ExperienceGained += difficultyMod;
      if (difficultyMod >= 5 && Game1.random.NextDouble() < 0.05)
        this.objectsToDrop.Add(749);
      if (difficultyMod >= 5 && Game1.random.NextDouble() < 0.2)
        this.objectsToDrop.Add(770);
      if (difficultyMod >= 10 && Game1.random.NextDouble() < 0.01)
        this.objectsToDrop.Add(386);
      if (difficultyMod >= 10 && Game1.random.NextDouble() < 0.01)
        this.objectsToDrop.Add(386);
      if (difficultyMod >= 10 && Game1.random.NextDouble() < 0.001)
        this.objectsToDrop.Add(74);
      this.Sprite.currentFrame = 16;
      this.Sprite.loop = false;
      this.Sprite.UpdateSourceRect();
    }

    public RockGolem(Vector2 position, bool alreadySpawned)
      : base("Stone Golem", position)
    {
      if (alreadySpawned)
      {
        this.IsWalkingTowardPlayer = true;
        this.seenPlayer.Value = true;
        this.moveTowardPlayerThreshold.Value = 16;
      }
      else
        this.IsWalkingTowardPlayer = false;
      this.Sprite.loop = false;
      this.Slipperiness = 2;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.seenPlayer);
      this.position.Field.AxisAlignedMovement = true;
    }

    public override List<Item> getExtraDropItems()
    {
      if (this.name.Equals((object) "Wilderness Golem"))
      {
        if (Game1.random.NextDouble() <= 0.0001)
          return new List<Item>() { (Item) new Hat(40) };
        if (Game1.currentSeason.Equals("spring") && Game1.random.NextDouble() < 33.0 / 400.0)
        {
          List<Item> extraDropItems = new List<Item>();
          int num = Game1.random.Next(2, 6);
          for (int index = 0; index < num; ++index)
            extraDropItems.Add((Item) new StardewValley.Object(273, 1));
          return extraDropItems;
        }
      }
      return base.getExtraDropItems();
    }

    public override void BuffForAdditionalDifficulty(int additional_difficulty)
    {
      base.BuffForAdditionalDifficulty(additional_difficulty);
      this.resilience.Value *= 2;
      ++this.Speed;
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
      this.focusedOnFarmers = true;
      this.IsWalkingTowardPlayer = true;
      if (Game1.random.NextDouble() < (double) (NetFieldBase<double, NetDouble>) this.missChance - (double) (NetFieldBase<double, NetDouble>) this.missChance * addedPrecision)
      {
        damage1 = -1;
      }
      else
      {
        this.Health -= damage1;
        this.setTrajectory(xTrajectory, yTrajectory);
        if (this.Health <= 0)
          this.deathAnimation();
        else
          this.currentLocation.playSound("rockGolemHit");
        this.currentLocation.playSound("hitEnemy");
      }
      return damage1;
    }

    protected override void localDeathAnimation()
    {
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.Position, Color.DarkGray, 10));
      this.currentLocation.localSound("rockGolemDie");
    }

    protected override void sharedDeathAnimation()
    {
      GameLocation currentLocation = this.currentLocation;
      string textureName = (string) (NetFieldBase<string, NetString>) this.Sprite.textureName;
      Rectangle sourcerectangle = new Rectangle(0, 576, 64, 64);
      Rectangle boundingBox = this.GetBoundingBox();
      int x = boundingBox.Center.X;
      boundingBox = this.GetBoundingBox();
      int y1 = boundingBox.Center.Y;
      int numberOfChunks = Game1.random.Next(4, 9);
      int y2 = (int) this.getTileLocation().Y;
      Game1.createRadialDebris(currentLocation, textureName, sourcerectangle, 32, x, y1, numberOfChunks, y2);
    }

    public override void noMovementProgressNearPlayerBehavior()
    {
      if (!this.IsWalkingTowardPlayer)
        return;
      this.Halt();
      this.faceGeneralDirection(this.Player.getStandingPosition());
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      if (this.IsWalkingTowardPlayer)
        base.behaviorAtGameTick(time);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.seenPlayer)
      {
        if (this.withinPlayerThreshold())
        {
          this.currentLocation.playSound("rockGolemSpawn");
          this.seenPlayer.Value = true;
        }
        else
        {
          this.Sprite.currentFrame = 16;
          this.Sprite.loop = false;
          this.Sprite.UpdateSourceRect();
        }
      }
      else if (this.Sprite.currentFrame >= 16)
      {
        this.Sprite.Animate(time, 16, 8, 75f);
        if (this.Sprite.currentFrame < 24)
          return;
        this.Sprite.loop = true;
        this.Sprite.currentFrame = 0;
        this.moveTowardPlayerThreshold.Value = 16;
        this.IsWalkingTowardPlayer = true;
        this.jitteriness.Value = 0.01;
        this.HideShadow = false;
      }
      else
      {
        if (!this.IsWalkingTowardPlayer || Game1.random.NextDouble() >= 0.001 || !Utility.isOnScreen(this.getStandingPosition(), 0))
          return;
        this.controller = new PathFindController((Character) this, this.currentLocation, new Point((int) this.Player.getTileLocation().X, (int) this.Player.getTileLocation().Y), -1, (PathFindController.endBehavior) null, 200);
      }
    }

    protected override void updateMonsterSlaveAnimation(GameTime time)
    {
      if (this.IsWalkingTowardPlayer)
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
      if (!(bool) (NetFieldBase<bool, NetBool>) this.seenPlayer)
      {
        this.Sprite.currentFrame = 16;
        this.Sprite.loop = false;
        this.Sprite.UpdateSourceRect();
      }
      else
      {
        if (this.Sprite.currentFrame < 16)
          return;
        this.Sprite.Animate(time, 16, 8, 75f);
        if (this.Sprite.currentFrame < 24)
          return;
        this.Sprite.loop = true;
        this.Sprite.currentFrame = 0;
        this.Sprite.UpdateSourceRect();
      }
    }
  }
}
