// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Mummy
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace StardewValley.Monsters
{
  public class Mummy : Monster
  {
    public NetInt reviveTimer = new NetInt(0);
    public const int revivalTime = 10000;
    protected int _damageToFarmer;
    private readonly NetEvent1Field<bool, NetBool> crumbleEvent = new NetEvent1Field<bool, NetBool>();

    public Mummy()
    {
    }

    public Mummy(Vector2 position)
      : base(nameof (Mummy), position)
    {
      this.Sprite.SpriteHeight = 32;
      this.Sprite.ignoreStopAnimation = true;
      this.Sprite.UpdateSourceRect();
      this._damageToFarmer = this.damageToFarmer.Value;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.crumbleEvent, (INetSerializable) this.reviveTimer);
      this.crumbleEvent.onEvent += new AbstractNetEvent1<bool>.Event(this.performCrumble);
      this.position.Field.AxisAlignedMovement = true;
    }

    public override void reloadSprite()
    {
      this.Sprite = new AnimatedSprite("Characters\\Monsters\\Mummy");
      this.Sprite.SpriteHeight = 32;
      this.Sprite.UpdateSourceRect();
      this.Sprite.ignoreStopAnimation = true;
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
      if ((int) (NetFieldBase<int, NetInt>) this.reviveTimer > 0)
      {
        if (!isBomb)
          return -1;
        this.Health = 0;
        Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.BlueViolet, 10)
        {
          holdLastFrame = true,
          alphaFade = 0.01f,
          interval = 70f
        }, this.currentLocation);
        this.currentLocation.playSound("ghost");
        return 999;
      }
      if (Game1.random.NextDouble() < (double) (NetFieldBase<double, NetDouble>) this.missChance - (double) (NetFieldBase<double, NetDouble>) this.missChance * addedPrecision)
      {
        damage1 = -1;
      }
      else
      {
        this.Slipperiness = 2;
        this.Health -= damage1;
        this.setTrajectory(xTrajectory, yTrajectory);
        this.currentLocation.playSound("shadowHit");
        this.currentLocation.playSound("skeletonStep");
        this.IsWalkingTowardPlayer = true;
        if (this.Health <= 0)
        {
          if (!isBomb && who.CurrentTool is MeleeWeapon && (who.CurrentTool as MeleeWeapon).hasEnchantmentOfType<CrusaderEnchantment>())
          {
            Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.BlueViolet, 10)
            {
              holdLastFrame = true,
              alphaFade = 0.01f,
              interval = 70f
            }, this.currentLocation);
            this.currentLocation.playSound("ghost");
          }
          else
          {
            this.reviveTimer.Value = 10000;
            this.Health = this.MaxHealth;
            this.deathAnimation();
          }
        }
      }
      return damage1;
    }

    public override void defaultMovementBehavior(GameTime time)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.reviveTimer > 0)
        return;
      base.defaultMovementBehavior(time);
    }

    public override List<Item> getExtraDropItems()
    {
      List<Item> extraDropItems = new List<Item>();
      if (Game1.random.NextDouble() < 0.002)
        extraDropItems.Add((Item) new StardewValley.Object(485, 1));
      return extraDropItems;
    }

    protected override void sharedDeathAnimation()
    {
      this.Halt();
      this.crumble();
      this.collidesWithOtherCharacters.Value = false;
      this.IsWalkingTowardPlayer = false;
      this.moveTowardPlayerThreshold.Value = -1;
    }

    protected override void localDeathAnimation()
    {
    }

    public override void update(GameTime time, GameLocation location)
    {
      this.crumbleEvent.Poll();
      if ((int) (NetFieldBase<int, NetInt>) this.reviveTimer > 0 && this.Sprite.CurrentAnimation == null && this.Sprite.currentFrame != 19)
        this.Sprite.currentFrame = 19;
      base.update(time, location);
    }

    private void crumble(bool reverse = false) => this.crumbleEvent.Fire(reverse);

    private void performCrumble(bool reverse)
    {
      this.Sprite.setCurrentAnimation(this.getCrumbleAnimation(reverse));
      if (!reverse)
      {
        if (Game1.IsMasterGame)
          this.damageToFarmer.Value = 0;
        this.reviveTimer.Value = 10000;
        this.currentLocation.localSound("monsterdead");
      }
      else
      {
        if (Game1.IsMasterGame)
          this.damageToFarmer.Value = this._damageToFarmer;
        this.reviveTimer.Value = 0;
        this.currentLocation.localSound("skeletonDie");
      }
    }

    private List<FarmerSprite.AnimationFrame> getCrumbleAnimation(bool reverse = false)
    {
      List<FarmerSprite.AnimationFrame> crumbleAnimation = new List<FarmerSprite.AnimationFrame>();
      if (!reverse)
        crumbleAnimation.Add(new FarmerSprite.AnimationFrame(16, 100, 0, false, false));
      else
        crumbleAnimation.Add(new FarmerSprite.AnimationFrame(16, 100, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.behaviorAfterRevival), true));
      crumbleAnimation.Add(new FarmerSprite.AnimationFrame(17, 100, 0, false, false));
      crumbleAnimation.Add(new FarmerSprite.AnimationFrame(18, 100, 0, false, false));
      if (!reverse)
        crumbleAnimation.Add(new FarmerSprite.AnimationFrame(19, 100, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.behaviorAfterCrumble)));
      else
        crumbleAnimation.Add(new FarmerSprite.AnimationFrame(19, 100, 0, false, false));
      if (reverse)
        crumbleAnimation.Reverse();
      return crumbleAnimation;
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.reviveTimer <= 0 && this.withinPlayerThreshold())
        this.IsWalkingTowardPlayer = true;
      base.behaviorAtGameTick(time);
    }

    protected override void updateAnimation(GameTime time)
    {
      if (this.Sprite.CurrentAnimation != null)
      {
        if (this.Sprite.animateOnce(time))
          this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.reviveTimer > 0)
      {
        this.reviveTimer.Value -= time.ElapsedGameTime.Milliseconds;
        if ((int) (NetFieldBase<int, NetInt>) this.reviveTimer < 2000)
          this.shake((int) (NetFieldBase<int, NetInt>) this.reviveTimer);
        if ((int) (NetFieldBase<int, NetInt>) this.reviveTimer <= 0)
        {
          if (Game1.IsMasterGame)
          {
            this.crumble(true);
            this.IsWalkingTowardPlayer = true;
          }
          else
            this.reviveTimer.Value = 1;
        }
      }
      else if (!Game1.IsMasterGame)
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
      }
      this.resetAnimationSpeed();
    }

    private void behaviorAfterCrumble(Farmer who)
    {
      this.Halt();
      this.Sprite.currentFrame = 19;
      this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
    }

    private void behaviorAfterRevival(Farmer who)
    {
      this.IsWalkingTowardPlayer = true;
      this.collidesWithOtherCharacters.Value = true;
      this.Sprite.currentFrame = 0;
      this.Sprite.oldFrame = 0;
      this.moveTowardPlayerThreshold.Value = 8;
      this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
    }
  }
}
