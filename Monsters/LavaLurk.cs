// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.LavaLurk
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Monsters
{
  public class LavaLurk : Monster
  {
    [XmlIgnore]
    public List<FarmerSprite.AnimationFrame> submergedAnimation = new List<FarmerSprite.AnimationFrame>();
    [XmlIgnore]
    public List<FarmerSprite.AnimationFrame> lurkAnimation = new List<FarmerSprite.AnimationFrame>();
    [XmlIgnore]
    public List<FarmerSprite.AnimationFrame> emergeAnimation = new List<FarmerSprite.AnimationFrame>();
    [XmlIgnore]
    public List<FarmerSprite.AnimationFrame> diveAnimation = new List<FarmerSprite.AnimationFrame>();
    [XmlIgnore]
    public List<FarmerSprite.AnimationFrame> resubmergeAnimation = new List<FarmerSprite.AnimationFrame>();
    [XmlIgnore]
    public List<FarmerSprite.AnimationFrame> idleAnimation = new List<FarmerSprite.AnimationFrame>();
    [XmlIgnore]
    public List<FarmerSprite.AnimationFrame> fireAnimation = new List<FarmerSprite.AnimationFrame>();
    [XmlIgnore]
    public List<FarmerSprite.AnimationFrame> locallyPlayingAnimation;
    [XmlIgnore]
    public bool approachFarmer;
    [XmlIgnore]
    public Vector2 velocity = Vector2.Zero;
    [XmlIgnore]
    public int swimSpeed;
    [XmlIgnore]
    public Farmer targettedFarmer;
    [XmlIgnore]
    public NetEnum<LavaLurk.State> currentState = new NetEnum<LavaLurk.State>();
    [XmlIgnore]
    public float stateTimer;
    [XmlIgnore]
    public float fireTimer;

    public LavaLurk() => this.Initialize();

    public LavaLurk(Vector2 position)
      : base("Lava Lurk", position)
    {
      this.Sprite.SpriteWidth = 16;
      this.Sprite.SpriteHeight = 16;
      this.Sprite.UpdateSourceRect();
      this.Initialize();
      this.ignoreDamageLOS.Value = true;
      this.SetRandomMovement();
      this.stateTimer = Utility.RandomFloat(3f, 5f);
    }

    public override void reloadSprite()
    {
      base.reloadSprite();
      this.Sprite.SpriteWidth = 16;
      this.Sprite.SpriteHeight = 16;
      this.Sprite.UpdateSourceRect();
    }

    public virtual void Initialize()
    {
      this.HideShadow = true;
      this.submergedAnimation.AddRange((IEnumerable<FarmerSprite.AnimationFrame>) new FarmerSprite.AnimationFrame[2]
      {
        new FarmerSprite.AnimationFrame(0, 750),
        new FarmerSprite.AnimationFrame(1, 1000)
      });
      this.lurkAnimation.AddRange((IEnumerable<FarmerSprite.AnimationFrame>) new FarmerSprite.AnimationFrame[2]
      {
        new FarmerSprite.AnimationFrame(2, 250),
        new FarmerSprite.AnimationFrame(3, 250)
      });
      this.resubmergeAnimation.AddRange((IEnumerable<FarmerSprite.AnimationFrame>) new FarmerSprite.AnimationFrame[3]
      {
        new FarmerSprite.AnimationFrame(3, 250),
        new FarmerSprite.AnimationFrame(2, 250),
        new FarmerSprite.AnimationFrame(1, 250, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnDiveAnimationEnd))
      });
      this.emergeAnimation.AddRange((IEnumerable<FarmerSprite.AnimationFrame>) new FarmerSprite.AnimationFrame[4]
      {
        new FarmerSprite.AnimationFrame(2, 150),
        new FarmerSprite.AnimationFrame(3, 150),
        new FarmerSprite.AnimationFrame(4, 150),
        new FarmerSprite.AnimationFrame(5, 150, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnEmergeAnimationEnd), true)
      });
      this.diveAnimation.AddRange((IEnumerable<FarmerSprite.AnimationFrame>) new FarmerSprite.AnimationFrame[4]
      {
        new FarmerSprite.AnimationFrame(5, 150),
        new FarmerSprite.AnimationFrame(4, 150),
        new FarmerSprite.AnimationFrame(3, 150),
        new FarmerSprite.AnimationFrame(2, 150, false, false, new AnimatedSprite.endOfAnimationBehavior(this.OnDiveAnimationEnd), true)
      });
      this.idleAnimation.AddRange((IEnumerable<FarmerSprite.AnimationFrame>) new FarmerSprite.AnimationFrame[2]
      {
        new FarmerSprite.AnimationFrame(5, 500),
        new FarmerSprite.AnimationFrame(6, 500)
      });
      this.fireAnimation.AddRange((IEnumerable<FarmerSprite.AnimationFrame>) new FarmerSprite.AnimationFrame[1]
      {
        new FarmerSprite.AnimationFrame(7, 500)
      });
    }

    public virtual void OnEmergeAnimationEnd(Farmer who) => this.PlayAnimation(this.idleAnimation, true);

    public virtual void OnDiveAnimationEnd(Farmer who) => this.PlayAnimation(this.submergedAnimation, true);

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.currentState);
    }

    protected override void sharedDeathAnimation()
    {
      this.currentLocation.playSound("skeletonDie");
      this.currentLocation.playSound("grunt");
      for (int index = 0; index < 16; ++index)
      {
        GameLocation currentLocation = this.currentLocation;
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
        Game1.createRadialDebris(currentLocation, "Characters\\Monsters\\Pepper Rex", sourcerectangle, 16, xPosition, yPosition, 1, y, white, 4f);
      }
    }

    protected override void updateAnimation(GameTime time)
    {
      base.updateAnimation(time);
      if (this.currentState.Value == LavaLurk.State.Submerged)
        this.PlayAnimation(this.submergedAnimation, true);
      else if (this.currentState.Value == LavaLurk.State.Lurking)
      {
        if (this.PlayAnimation(this.lurkAnimation, false) && this.currentLocation == Game1.currentLocation && Utility.isOnScreen(this.Position, 64))
          Game1.playSound("waterSlosh");
      }
      else if (this.currentState.Value == LavaLurk.State.Emerged)
      {
        if (this.locallyPlayingAnimation != this.emergeAnimation && this.locallyPlayingAnimation != this.idleAnimation)
        {
          if (this.currentLocation == Game1.currentLocation && Utility.isOnScreen(this.Position, 64))
            Game1.playSound("waterSlosh");
          this.PlayAnimation(this.emergeAnimation, false);
        }
      }
      else if (this.currentState.Value == LavaLurk.State.Firing)
        this.PlayAnimation(this.fireAnimation, true);
      else if (this.currentState.Value == LavaLurk.State.Diving && this.locallyPlayingAnimation != this.diveAnimation && this.locallyPlayingAnimation != this.submergedAnimation && this.locallyPlayingAnimation != this.resubmergeAnimation)
      {
        if (this.currentLocation == Game1.currentLocation && Utility.isOnScreen(this.Position, 64))
          Game1.playSound("waterSlosh");
        if (this.locallyPlayingAnimation == this.lurkAnimation)
          this.PlayAnimation(this.resubmergeAnimation, false);
        else
          this.PlayAnimation(this.diveAnimation, false);
      }
      this.Sprite.animateOnce(time);
    }

    public virtual bool PlayAnimation(
      List<FarmerSprite.AnimationFrame> animation_to_play,
      bool loop)
    {
      if (this.locallyPlayingAnimation == animation_to_play)
        return false;
      this.locallyPlayingAnimation = animation_to_play;
      this.Sprite.setCurrentAnimation(animation_to_play);
      this.Sprite.loop = loop;
      if (!loop)
        this.Sprite.oldFrame = animation_to_play.Last<FarmerSprite.AnimationFrame>().frame;
      return true;
    }

    public virtual bool TargetInRange() => this.targettedFarmer != null && (double) Math.Abs(this.targettedFarmer.Position.X - this.Position.X) <= 640.0 && (double) Math.Abs(this.targettedFarmer.Position.Y - this.Position.Y) <= 640.0;

    public virtual void SetRandomMovement() => this.velocity = new Vector2(Game1.random.Next(2) == 1 ? -1f : 1f, Game1.random.Next(2) == 1 ? -1f : 1f);

    protected override void updateMonsterSlaveAnimation(GameTime time)
    {
    }

    public override int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      Farmer who)
    {
      return this.currentState.Value == LavaLurk.State.Submerged ? -1 : base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      if (this.targettedFarmer == null || this.targettedFarmer.currentLocation != this.currentLocation)
      {
        this.targettedFarmer = (Farmer) null;
        this.targettedFarmer = this.findPlayer();
      }
      if ((double) this.stateTimer > 0.0)
      {
        this.stateTimer -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.stateTimer <= 0.0)
          this.stateTimer = 0.0f;
      }
      if (this.currentState.Value == LavaLurk.State.Submerged)
      {
        this.swimSpeed = 2;
        if ((double) this.stateTimer == 0.0)
        {
          this.currentState.Value = LavaLurk.State.Lurking;
          this.stateTimer = 1f;
        }
      }
      else if (this.currentState.Value == LavaLurk.State.Lurking)
      {
        this.swimSpeed = 1;
        if ((double) this.stateTimer == 0.0)
        {
          if (this.TargetInRange())
          {
            this.currentState.Value = LavaLurk.State.Emerged;
            this.stateTimer = 1f;
            this.swimSpeed = 0;
          }
          else
          {
            this.currentState.Value = LavaLurk.State.Diving;
            this.stateTimer = 1f;
          }
        }
      }
      else if (this.currentState.Value == LavaLurk.State.Emerged)
      {
        if ((double) this.stateTimer == 0.0)
        {
          this.currentState.Value = LavaLurk.State.Firing;
          this.stateTimer = 1f;
          this.fireTimer = 0.25f;
        }
      }
      else if (this.currentState.Value == LavaLurk.State.Firing)
      {
        if ((double) this.stateTimer == 0.0)
        {
          this.currentState.Value = LavaLurk.State.Diving;
          this.stateTimer = 1f;
        }
        if ((double) this.fireTimer > 0.0)
        {
          this.fireTimer -= (float) time.ElapsedGameTime.TotalSeconds;
          if ((double) this.fireTimer <= 0.0)
          {
            this.fireTimer = 0.25f;
            if (this.targettedFarmer != null)
            {
              Vector2 startingPosition = this.Position + new Vector2(0.0f, -32f);
              Vector2 vector2_1 = this.targettedFarmer.Position - startingPosition;
              vector2_1.Normalize();
              Vector2 vector2_2 = vector2_1 * 7f;
              this.currentLocation.playSound("fireball");
              BasicProjectile basicProjectile = new BasicProjectile(25, 10, 0, 3, 0.1963495f, vector2_2.X, vector2_2.Y, startingPosition, "", "", false, location: this.currentLocation, firer: ((Character) this));
              basicProjectile.ignoreLocationCollision.Value = true;
              basicProjectile.ignoreTravelGracePeriod.Value = true;
              basicProjectile.maxTravelDistance.Value = 640;
              this.currentLocation.projectiles.Add((Projectile) basicProjectile);
            }
          }
        }
      }
      else if (this.currentState.Value == LavaLurk.State.Diving && (double) this.stateTimer == 0.0)
      {
        this.currentState.Value = LavaLurk.State.Submerged;
        this.stateTimer = Utility.RandomFloat(3f, 5f);
        this.approachFarmer = !this.approachFarmer;
        if (this.approachFarmer)
          this.targettedFarmer = this.findPlayer();
        this.SetRandomMovement();
      }
      if (this.targettedFarmer != null && this.approachFarmer)
      {
        if (this.getTileX() > this.targettedFarmer.getTileX())
          this.velocity.X = -1f;
        else if (this.getTileX() < this.targettedFarmer.getTileX())
          this.velocity.X = 1f;
        if (this.getTileY() > this.targettedFarmer.getTileY())
          this.velocity.Y = -1f;
        else if (this.getTileY() < this.targettedFarmer.getTileY())
          this.velocity.Y = 1f;
      }
      if ((double) this.velocity.X == 0.0 && (double) this.velocity.Y == 0.0)
        return;
      Rectangle boundingBox = this.GetBoundingBox();
      Vector2 position = this.Position;
      boundingBox.Inflate(48, 48);
      boundingBox.X += (int) this.velocity.X * this.swimSpeed;
      position.X += (float) ((int) this.velocity.X * this.swimSpeed);
      if (!this.CheckInWater(boundingBox))
      {
        this.velocity.X *= -1f;
        boundingBox.X += (int) this.velocity.X * this.swimSpeed;
        position.X += (float) ((int) this.velocity.X * this.swimSpeed);
      }
      boundingBox.Y += (int) this.velocity.Y * this.swimSpeed;
      position.Y += (float) ((int) this.velocity.Y * this.swimSpeed);
      if (!this.CheckInWater(boundingBox))
      {
        this.velocity.Y *= -1f;
        boundingBox.Y += (int) this.velocity.Y * this.swimSpeed;
        position.Y += (float) ((int) this.velocity.Y * this.swimSpeed);
      }
      if (!(this.Position != position))
        return;
      this.Position = position;
    }

    public static bool IsLavaTile(GameLocation location, int x, int y) => location.doesTileHaveProperty(x, y, "Water", "Back") != null;

    public bool CheckInWater(Rectangle position)
    {
      for (int x = position.Left / 64; x <= position.Right / 64; ++x)
      {
        for (int y = position.Top / 64; y <= position.Bottom / 64; ++y)
        {
          if (!LavaLurk.IsLavaTile(this.currentLocation, x, y))
            return false;
        }
      }
      return true;
    }

    public override void updateMovement(GameLocation location, GameTime time)
    {
    }

    public override Debris ModifyMonsterLoot(Debris debris)
    {
      if (debris != null)
        debris.chunksMoveTowardPlayer = true;
      return debris;
    }

    public enum State
    {
      Submerged,
      Lurking,
      Emerged,
      Firing,
      Diving,
    }
  }
}
