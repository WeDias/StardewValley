// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Monster
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Monsters
{
  public class Monster : NPC
  {
    public const int defaultInvincibleCountdown = 450;
    public const int seekPlayerIterationLimit = 80;
    [XmlElement("damageToFarmer")]
    public readonly NetInt damageToFarmer = new NetInt();
    [XmlElement("health")]
    public readonly NetInt health = new NetInt();
    [XmlElement("maxHealth")]
    public readonly NetInt maxHealth = new NetInt();
    [XmlElement("coinsToDrop")]
    public readonly NetInt coinsToDrop = new NetInt();
    [XmlElement("durationOfRandomMovements")]
    public readonly NetInt durationOfRandomMovements = new NetInt();
    [XmlElement("resilience")]
    public readonly NetInt resilience = new NetInt();
    [XmlElement("slipperiness")]
    public readonly NetInt slipperiness = new NetInt(2);
    [XmlElement("experienceGained")]
    public readonly NetInt experienceGained = new NetInt();
    [XmlElement("jitteriness")]
    public readonly NetDouble jitteriness = new NetDouble();
    [XmlElement("missChance")]
    public readonly NetDouble missChance = new NetDouble();
    [XmlElement("isGlider")]
    public readonly NetBool isGlider = new NetBool();
    [XmlElement("mineMonster")]
    public readonly NetBool mineMonster = new NetBool();
    [XmlElement("hasSpecialItem")]
    public readonly NetBool hasSpecialItem = new NetBool();
    [XmlIgnore]
    public readonly NetFloat synchedRotation = new NetFloat().Interpolated(true, true);
    public readonly NetIntList objectsToDrop = new NetIntList();
    protected int skipHorizontal;
    protected int invincibleCountdown;
    [XmlIgnore]
    private bool skipHorizontalUp;
    protected readonly NetInt defaultAnimationInterval = new NetInt(175);
    public int stunTime;
    [XmlElement("initializedForLocation")]
    public bool initializedForLocation;
    [XmlIgnore]
    public readonly NetBool netFocusedOnFarmers = new NetBool();
    [XmlIgnore]
    public readonly NetBool netWildernessFarmMonster = new NetBool();
    private readonly NetEvent1<ParryEventArgs> parryEvent = new NetEvent1<ParryEventArgs>();
    private readonly NetEvent1Field<Vector2, NetVector2> trajectoryEvent = new NetEvent1Field<Vector2, NetVector2>();
    [XmlIgnore]
    private readonly NetEvent0 deathAnimEvent = new NetEvent0();
    [XmlElement("ignoreDamageLOS")]
    public readonly NetBool ignoreDamageLOS = new NetBool();
    protected Monster.collisionBehavior onCollision;
    [XmlElement("isHardModeMonster")]
    public NetBool isHardModeMonster = new NetBool(false);
    private int slideAnimationTimer;

    [XmlIgnore]
    public Farmer Player => this.findPlayer();

    [XmlIgnore]
    public int DamageToFarmer
    {
      get => (int) (NetFieldBase<int, NetInt>) this.damageToFarmer;
      set => this.damageToFarmer.Value = value;
    }

    [XmlIgnore]
    public int Health
    {
      get => (int) (NetFieldBase<int, NetInt>) this.health;
      set => this.health.Value = value;
    }

    [XmlIgnore]
    public int MaxHealth
    {
      get => (int) (NetFieldBase<int, NetInt>) this.maxHealth;
      set => this.maxHealth.Value = value;
    }

    [XmlIgnore]
    public int ExperienceGained
    {
      get => (int) (NetFieldBase<int, NetInt>) this.experienceGained;
      set => this.experienceGained.Value = value;
    }

    [XmlIgnore]
    public int Slipperiness
    {
      get => (int) (NetFieldBase<int, NetInt>) this.slipperiness;
      set => this.slipperiness.Value = value;
    }

    [XmlIgnore]
    public bool focusedOnFarmers
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.netFocusedOnFarmers;
      set => this.netFocusedOnFarmers.Value = value;
    }

    [XmlIgnore]
    public bool wildernessFarmMonster
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.netWildernessFarmMonster;
      set => this.netWildernessFarmMonster.Value = value;
    }

    public Monster()
    {
    }

    public Monster(string name, Vector2 position)
      : this(name, position, 2)
    {
      this.Breather = false;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.damageToFarmer, (INetSerializable) this.health, (INetSerializable) this.maxHealth, (INetSerializable) this.coinsToDrop, (INetSerializable) this.durationOfRandomMovements, (INetSerializable) this.resilience, (INetSerializable) this.slipperiness, (INetSerializable) this.experienceGained, (INetSerializable) this.jitteriness, (INetSerializable) this.missChance, (INetSerializable) this.isGlider, (INetSerializable) this.mineMonster, (INetSerializable) this.hasSpecialItem, (INetSerializable) this.objectsToDrop, (INetSerializable) this.defaultAnimationInterval, (INetSerializable) this.netFocusedOnFarmers, (INetSerializable) this.netWildernessFarmMonster, (INetSerializable) this.deathAnimEvent, (INetSerializable) this.parryEvent, (INetSerializable) this.trajectoryEvent, (INetSerializable) this.ignoreDamageLOS, (INetSerializable) this.synchedRotation, (INetSerializable) this.isHardModeMonster);
      this.position.Field.AxisAlignedMovement = false;
      this.parryEvent.onEvent += new AbstractNetEvent1<ParryEventArgs>.Event(this.handleParried);
      this.parryEvent.InterpolationWait = false;
      this.deathAnimEvent.onEvent += new NetEvent0.Event(this.localDeathAnimation);
      this.trajectoryEvent.onEvent += new AbstractNetEvent1<Vector2>.Event(this.doSetTrajectory);
      this.trajectoryEvent.InterpolationWait = false;
    }

    protected override Farmer findPlayer()
    {
      if (this.currentLocation == null)
        return Game1.player;
      Farmer player = Game1.player;
      double num = double.MaxValue;
      foreach (Farmer farmer in this.currentLocation.farmers)
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) farmer.hidden)
        {
          double playerPriority = this.findPlayerPriority(farmer);
          if (playerPriority < num)
          {
            num = playerPriority;
            player = farmer;
          }
        }
      }
      return player;
    }

    protected virtual double findPlayerPriority(Farmer f) => (double) (f.Position - this.Position).LengthSquared();

    public virtual void onDealContactDamage(Farmer who)
    {
    }

    public virtual List<Item> getExtraDropItems() => new List<Item>();

    public override bool withinPlayerThreshold() => this.focusedOnFarmers || this.withinPlayerThreshold((int) (NetFieldBase<int, NetInt>) this.moveTowardPlayerThreshold);

    public override bool IsMonster => true;

    public Monster(string name, Vector2 position, int facingDir)
      : base(new AnimatedSprite("Characters\\Monsters\\" + name), position, facingDir, name)
    {
      this.parseMonsterInfo(name);
      this.Breather = false;
    }

    public virtual void drawAboveAllLayers(SpriteBatch b)
    {
    }

    public override void draw(SpriteBatch b)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.isGlider)
        return;
      base.draw(b);
    }

    public virtual bool isInvincible() => this.invincibleCountdown > 0;

    public void setInvincibleCountdown(int time)
    {
      this.invincibleCountdown = time;
      this.startGlowing(new Color((int) byte.MaxValue, 0, 0), false, 0.25f);
      this.glowingTransparency = 1f;
    }

    protected int maxTimesReachedMineBottom()
    {
      int val1 = 0;
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
        val1 = Math.Max(val1, onlineFarmer.timesReachedMineBottom);
      return val1;
    }

    public virtual Debris ModifyMonsterLoot(Debris debris) => debris;

    public virtual int GetBaseDifficultyLevel() => 0;

    public virtual void BuffForAdditionalDifficulty(int additional_difficulty)
    {
      if (this.DamageToFarmer != 0)
      {
        this.DamageToFarmer = (int) ((double) this.DamageToFarmer * (1.0 + (double) additional_difficulty * 0.25));
        int b = 20 + (additional_difficulty - 1) * 20;
        if (this.DamageToFarmer < b)
          this.DamageToFarmer = (int) Utility.Lerp((float) this.DamageToFarmer, (float) b, 0.5f);
      }
      this.MaxHealth = (int) ((double) this.MaxHealth * (1.0 + (double) additional_difficulty * 0.5));
      int b1 = 500 + (additional_difficulty - 1) * 300;
      if (this.MaxHealth < b1)
        this.MaxHealth = (int) Utility.Lerp((float) this.MaxHealth, (float) b1, 0.5f);
      this.Health = this.MaxHealth;
      this.resilience.Value += additional_difficulty * this.resilience.Value;
      this.isHardModeMonster.Value = true;
    }

    protected void parseMonsterInfo(string name)
    {
      string[] strArray1 = Game1.content.Load<Dictionary<string, string>>("Data\\Monsters")[name].Split('/');
      this.Health = Convert.ToInt32(strArray1[0]);
      this.MaxHealth = this.Health;
      this.DamageToFarmer = Convert.ToInt32(strArray1[1]);
      this.coinsToDrop.Value = Game1.random.Next(Convert.ToInt32(strArray1[2]), Convert.ToInt32(strArray1[3]) + 1);
      this.isGlider.Value = Convert.ToBoolean(strArray1[4]);
      this.durationOfRandomMovements.Value = Convert.ToInt32(strArray1[5]);
      string[] strArray2 = strArray1[6].Split(' ');
      this.objectsToDrop.Clear();
      for (int index = 0; index < strArray2.Length; index += 2)
      {
        if (Game1.random.NextDouble() < Convert.ToDouble(strArray2[index + 1]))
          this.objectsToDrop.Add(Convert.ToInt32(strArray2[index]));
      }
      this.resilience.Value = Convert.ToInt32(strArray1[7]);
      this.jitteriness.Value = Convert.ToDouble(strArray1[8]);
      this.willDestroyObjectsUnderfoot = false;
      this.moveTowardPlayer(Convert.ToInt32(strArray1[9]));
      this.speed = Convert.ToInt32(strArray1[10]);
      this.missChance.Value = Convert.ToDouble(strArray1[11]);
      this.mineMonster.Value = Convert.ToBoolean(strArray1[12]);
      if (this.maxTimesReachedMineBottom() >= 1 && (bool) (NetFieldBase<bool, NetBool>) this.mineMonster)
      {
        this.resilience.Value += this.resilience.Value / 2;
        this.missChance.Value *= 2.0;
        this.Health += Game1.random.Next(0, this.Health);
        this.DamageToFarmer += Game1.random.Next(0, this.DamageToFarmer / 2);
        this.coinsToDrop.Value += Game1.random.Next(0, (int) (NetFieldBase<int, NetInt>) this.coinsToDrop + 1);
      }
      try
      {
        this.ExperienceGained = Convert.ToInt32(strArray1[13]);
      }
      catch (Exception ex)
      {
        this.ExperienceGained = 1;
      }
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
        return;
      this.displayName = strArray1[strArray1.Length - 1];
    }

    public virtual void InitializeForLocation(GameLocation location)
    {
      if (this.initializedForLocation)
        return;
      if ((bool) (NetFieldBase<bool, NetBool>) this.mineMonster && this.maxTimesReachedMineBottom() >= 1)
      {
        double num = 0.0;
        if (location is MineShaft)
          num = (double) (location as MineShaft).GetAdditionalDifficulty() * 0.001;
        if (Game1.random.NextDouble() < 0.001 + num)
          this.objectsToDrop.Add(Game1.random.NextDouble() < 0.5 ? 72 : 74);
      }
      if (Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS") && Game1.random.NextDouble() < ((string) (NetFieldBase<string, NetString>) this.name == "Dust Spirit" ? 0.02 : 0.05))
        this.objectsToDrop.Add(890);
      this.initializedForLocation = true;
    }

    public override void reloadSprite() => this.Sprite = new AnimatedSprite("Characters\\Monsters\\" + this.Name, 0, 16, 16);

    public virtual void shedChunks(int number) => this.shedChunks(number, 0.75f);

    public virtual void shedChunks(int number, float scale)
    {
      if (this.Sprite.Texture.Height <= this.Sprite.getHeight() * 4)
        return;
      GameLocation currentLocation = this.currentLocation;
      string textureName = (string) (NetFieldBase<string, NetString>) this.Sprite.textureName;
      Microsoft.Xna.Framework.Rectangle sourcerectangle = new Microsoft.Xna.Framework.Rectangle(0, this.Sprite.getHeight() * 4 + 16, 16, 16);
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      int x = boundingBox.Center.X;
      boundingBox = this.GetBoundingBox();
      int y1 = boundingBox.Center.Y;
      int numberOfChunks = number;
      int y2 = (int) this.getTileLocation().Y;
      Color white = Color.White;
      double scale1 = 4.0 * (double) scale;
      Game1.createRadialDebris(currentLocation, textureName, sourcerectangle, 8, x, y1, numberOfChunks, y2, white, (float) scale1);
    }

    public void deathAnimation()
    {
      this.sharedDeathAnimation();
      this.deathAnimEvent.Fire();
    }

    protected virtual void sharedDeathAnimation() => this.shedChunks(Game1.random.Next(4, 9), 0.75f);

    protected virtual void localDeathAnimation()
    {
    }

    public void parried(int damage, Farmer who) => this.parryEvent.Fire(new ParryEventArgs(damage, who));

    private void handleParried(ParryEventArgs args)
    {
      int damage = args.damage;
      Farmer who = args.who;
      if (Game1.IsMasterGame)
      {
        float xVelocity = this.xVelocity;
        float yVelocity = this.yVelocity;
        if ((double) this.xVelocity != 0.0 || (double) this.yVelocity != 0.0)
          this.currentLocation.damageMonster(this.GetBoundingBox(), damage / 2, damage / 2 + 1, false, 0.0f, 0, 0.0f, 0.0f, false, who);
        this.xVelocity = -xVelocity;
        this.yVelocity = -yVelocity;
        this.xVelocity *= (bool) (NetFieldBase<bool, NetBool>) this.isGlider ? 2f : 3.5f;
        this.yVelocity *= (bool) (NetFieldBase<bool, NetBool>) this.isGlider ? 2f : 3.5f;
      }
      this.setInvincibleCountdown(450);
    }

    public virtual int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      Farmer who)
    {
      return this.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, "hitEnemy");
    }

    public int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      string hitSound)
    {
      int damage1 = Math.Max(1, damage - (int) (NetFieldBase<int, NetInt>) this.resilience);
      this.slideAnimationTimer = 0;
      if (Game1.random.NextDouble() < (double) (NetFieldBase<double, NetDouble>) this.missChance - (double) (NetFieldBase<double, NetDouble>) this.missChance * addedPrecision)
      {
        damage1 = -1;
      }
      else
      {
        this.Health -= damage1;
        this.currentLocation.playSound(hitSound);
        this.setTrajectory(xTrajectory / 3, yTrajectory / 3);
        if (this.Health <= 0)
          this.deathAnimation();
      }
      return damage1;
    }

    public override void setTrajectory(Vector2 trajectory) => this.trajectoryEvent.Fire(trajectory);

    private void doSetTrajectory(Vector2 trajectory)
    {
      if (!Game1.IsMasterGame)
        return;
      if ((double) Math.Abs(trajectory.X) > (double) Math.Abs(this.xVelocity))
        this.xVelocity = trajectory.X;
      if ((double) Math.Abs(trajectory.Y) <= (double) Math.Abs(this.yVelocity))
        return;
      this.yVelocity = trajectory.Y;
    }

    public virtual void behaviorAtGameTick(GameTime time)
    {
      if ((double) this.timeBeforeAIMovementAgain > 0.0)
        this.timeBeforeAIMovementAgain -= (float) time.ElapsedGameTime.Milliseconds;
      if (!this.Player.isRafting || !this.withinPlayerThreshold(4))
        return;
      Microsoft.Xna.Framework.Rectangle boundingBox1 = this.Player.GetBoundingBox();
      int y1 = boundingBox1.Center.Y;
      boundingBox1 = this.GetBoundingBox();
      int y2 = boundingBox1.Center.Y;
      if (Math.Abs(y1 - y2) > 192)
      {
        Microsoft.Xna.Framework.Rectangle boundingBox2 = this.Player.GetBoundingBox();
        int x1 = boundingBox2.Center.X;
        boundingBox2 = this.GetBoundingBox();
        int x2 = boundingBox2.Center.X;
        if (x1 - x2 > 0)
          this.SetMovingLeft(true);
        else
          this.SetMovingRight(true);
      }
      else
      {
        Microsoft.Xna.Framework.Rectangle boundingBox3 = this.Player.GetBoundingBox();
        int y3 = boundingBox3.Center.Y;
        boundingBox3 = this.GetBoundingBox();
        int y4 = boundingBox3.Center.Y;
        if (y3 - y4 > 0)
          this.SetMovingUp(true);
        else
          this.SetMovingDown(true);
      }
      this.MovePosition(time, Game1.viewport, this.currentLocation);
    }

    public virtual bool passThroughCharacters() => false;

    public override bool shouldCollideWithBuildingLayer(GameLocation location) => true;

    public override void update(GameTime time, GameLocation location)
    {
      if (Game1.IsMasterGame && !this.initializedForLocation && location != null)
      {
        this.InitializeForLocation(location);
        this.initializedForLocation = true;
      }
      this.parryEvent.Poll();
      this.trajectoryEvent.Poll();
      this.deathAnimEvent.Poll();
      this.position.UpdateExtrapolation((float) (this.speed + this.addedSpeed));
      TimeSpan elapsedGameTime;
      if (this.invincibleCountdown > 0)
      {
        int invincibleCountdown = this.invincibleCountdown;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.invincibleCountdown = invincibleCountdown - milliseconds;
        if (this.invincibleCountdown <= 0)
          this.stopGlowing();
      }
      if (!location.farmers.Any())
        return;
      if (!this.Player.isRafting || !this.withinPlayerThreshold(4))
        base.update(time, location);
      if (Game1.IsMasterGame)
      {
        if (this.stunTime <= 0)
        {
          this.behaviorAtGameTick(time);
        }
        else
        {
          int stunTime = this.stunTime;
          elapsedGameTime = time.ElapsedGameTime;
          int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
          this.stunTime = stunTime - totalMilliseconds;
          if (this.stunTime < 0)
            this.stunTime = 0;
        }
      }
      this.updateAnimation(time);
      if (Game1.IsMasterGame)
        this.synchedRotation.Value = this.rotation;
      else
        this.rotation = this.synchedRotation.Value;
      if (this.controller != null && this.withinPlayerThreshold(3))
        this.controller = (PathFindController) null;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isGlider && ((double) this.Position.X < 0.0 || (double) this.Position.X > (double) (location.Map.GetLayer("Back").LayerWidth * 64) || (double) this.Position.Y < 0.0 || (double) this.Position.Y > (double) (location.map.GetLayer("Back").LayerHeight * 64)))
      {
        location.characters.Remove((NPC) this);
      }
      else
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.isGlider || (double) this.Position.X >= -2000.0)
          return;
        this.Health = -500;
      }
    }

    protected void resetAnimationSpeed()
    {
      if (this.ignoreMovementAnimations)
        return;
      this.Sprite.interval = (float) (int) (NetFieldBase<int, NetInt>) this.defaultAnimationInterval - (float) (this.speed + this.addedSpeed - 2) * 20f;
    }

    protected virtual void updateAnimation(GameTime time)
    {
      if (!Game1.IsMasterGame)
        this.updateMonsterSlaveAnimation(time);
      this.resetAnimationSpeed();
    }

    protected override void updateSlaveAnimation(GameTime time)
    {
    }

    protected virtual void updateMonsterSlaveAnimation(GameTime time) => this.Sprite.animateOnce(time);

    private bool doHorizontalMovement(GameLocation location)
    {
      bool flag = false;
      if ((double) this.Position.X > (double) this.Player.Position.X + 8.0 || this.skipHorizontal > 0 && this.Player.getStandingX() < this.getStandingX() - 8)
      {
        this.SetMovingOnlyLeft();
        if (!location.isCollidingPosition(this.nextPosition(3), Game1.viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
        {
          this.MovePosition(Game1.currentGameTime, Game1.viewport, location);
          flag = true;
        }
        else
        {
          this.faceDirection(3);
          if ((int) (NetFieldBase<int, NetInt>) this.durationOfRandomMovements > 0 && Game1.random.NextDouble() < (double) (NetFieldBase<double, NetDouble>) this.jitteriness)
          {
            if (Game1.random.NextDouble() < 0.5)
              this.tryToMoveInDirection(2, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider);
            else
              this.tryToMoveInDirection(0, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider);
            this.timeBeforeAIMovementAgain = (float) (int) (NetFieldBase<int, NetInt>) this.durationOfRandomMovements;
          }
        }
      }
      else if ((double) this.Position.X < (double) this.Player.Position.X - 8.0)
      {
        this.SetMovingOnlyRight();
        if (!location.isCollidingPosition(this.nextPosition(1), Game1.viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
        {
          this.MovePosition(Game1.currentGameTime, Game1.viewport, location);
          flag = true;
        }
        else
        {
          this.faceDirection(1);
          if ((int) (NetFieldBase<int, NetInt>) this.durationOfRandomMovements > 0 && Game1.random.NextDouble() < (double) (NetFieldBase<double, NetDouble>) this.jitteriness)
          {
            if (Game1.random.NextDouble() < 0.5)
              this.tryToMoveInDirection(2, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider);
            else
              this.tryToMoveInDirection(0, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider);
            this.timeBeforeAIMovementAgain = (float) (int) (NetFieldBase<int, NetInt>) this.durationOfRandomMovements;
          }
        }
      }
      else
      {
        this.faceGeneralDirection(this.Player.getStandingPosition());
        this.setMovingInFacingDirection();
        this.skipHorizontal = 500;
      }
      return flag;
    }

    public virtual bool ShouldActuallyMoveAwayFromPlayer() => false;

    private void checkHorizontalMovement(
      ref bool success,
      ref bool setMoving,
      ref bool scootSuccess,
      Farmer who,
      GameLocation location)
    {
      Vector2 position;
      if ((double) who.Position.X > (double) this.Position.X + 16.0)
      {
        if (this.ShouldActuallyMoveAwayFromPlayer())
          this.SetMovingOnlyLeft();
        else
          this.SetMovingOnlyRight();
        setMoving = true;
        if (!location.isCollidingPosition(this.nextPosition(1), Game1.viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
        {
          success = true;
        }
        else
        {
          this.MovePosition(Game1.currentGameTime, Game1.viewport, location);
          position = this.Position;
          if (!position.Equals(this.lastPosition))
            scootSuccess = true;
        }
      }
      if (success || (double) who.Position.X >= (double) this.Position.X - 16.0)
        return;
      if (this.ShouldActuallyMoveAwayFromPlayer())
        this.SetMovingOnlyRight();
      else
        this.SetMovingOnlyLeft();
      setMoving = true;
      if (!location.isCollidingPosition(this.nextPosition(3), Game1.viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
      {
        success = true;
      }
      else
      {
        this.MovePosition(Game1.currentGameTime, Game1.viewport, location);
        position = this.Position;
        if (position.Equals(this.lastPosition))
          return;
        scootSuccess = true;
      }
    }

    private void checkVerticalMovement(
      ref bool success,
      ref bool setMoving,
      ref bool scootSuccess,
      Farmer who,
      GameLocation location)
    {
      if (!success && (double) who.Position.Y < (double) this.Position.Y - 16.0)
      {
        if (this.ShouldActuallyMoveAwayFromPlayer())
          this.SetMovingOnlyDown();
        else
          this.SetMovingOnlyUp();
        setMoving = true;
        if (!location.isCollidingPosition(this.nextPosition(0), Game1.viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
        {
          success = true;
        }
        else
        {
          this.MovePosition(Game1.currentGameTime, Game1.viewport, location);
          if (!this.Position.Equals(this.lastPosition))
            scootSuccess = true;
        }
      }
      if (success || (double) who.Position.Y <= (double) this.Position.Y + 16.0)
        return;
      if (this.ShouldActuallyMoveAwayFromPlayer())
        this.SetMovingOnlyUp();
      else
        this.SetMovingOnlyDown();
      setMoving = true;
      if (!location.isCollidingPosition(this.nextPosition(2), Game1.viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
      {
        success = true;
      }
      else
      {
        this.MovePosition(Game1.currentGameTime, Game1.viewport, location);
        if (this.Position.Equals(this.lastPosition))
          return;
        scootSuccess = true;
      }
    }

    public override void updateMovement(GameLocation location, GameTime time)
    {
      if (this.IsWalkingTowardPlayer)
      {
        if (((int) (NetFieldBase<int, NetInt>) this.moveTowardPlayerThreshold == -1 || this.withinPlayerThreshold()) && (double) this.timeBeforeAIMovementAgain <= 0.0 && this.IsMonster && !(bool) (NetFieldBase<bool, NetBool>) this.isGlider && location.map.GetLayer("Back").Tiles[(int) this.Player.getTileLocation().X, (int) this.Player.getTileLocation().Y] != null && !location.map.GetLayer("Back").Tiles[(int) this.Player.getTileLocation().X, (int) this.Player.getTileLocation().Y].Properties.ContainsKey("NPCBarrier"))
        {
          if (this.skipHorizontal <= 0)
          {
            if (this.lastPosition.Equals(this.Position) && Game1.random.NextDouble() < 0.001)
            {
              switch (this.FacingDirection)
              {
                case 0:
                case 2:
                  if (Game1.random.NextDouble() < 0.5)
                  {
                    this.SetMovingOnlyRight();
                    break;
                  }
                  this.SetMovingOnlyLeft();
                  break;
                case 1:
                case 3:
                  if (Game1.random.NextDouble() < 0.5)
                  {
                    this.SetMovingOnlyUp();
                    break;
                  }
                  this.SetMovingOnlyDown();
                  break;
              }
              this.skipHorizontal = 700;
              return;
            }
            bool success = false;
            bool setMoving = false;
            bool scootSuccess = false;
            if ((double) this.lastPosition.X == (double) this.Position.X)
            {
              this.checkHorizontalMovement(ref success, ref setMoving, ref scootSuccess, this.Player, location);
              this.checkVerticalMovement(ref success, ref setMoving, ref scootSuccess, this.Player, location);
            }
            else
            {
              this.checkVerticalMovement(ref success, ref setMoving, ref scootSuccess, this.Player, location);
              this.checkHorizontalMovement(ref success, ref setMoving, ref scootSuccess, this.Player, location);
            }
            if (!success && !setMoving)
            {
              this.Halt();
              this.faceGeneralDirection(this.Player.getStandingPosition());
            }
            if (success)
              this.skipHorizontal = 500;
            if (scootSuccess)
              return;
          }
          else
            this.skipHorizontal -= time.ElapsedGameTime.Milliseconds;
        }
      }
      else
        this.defaultMovementBehavior(time);
      this.MovePosition(time, Game1.viewport, location);
      if (!this.Position.Equals(this.lastPosition) || !this.IsWalkingTowardPlayer || !this.withinPlayerThreshold())
        return;
      this.noMovementProgressNearPlayerBehavior();
    }

    public virtual void noMovementProgressNearPlayerBehavior()
    {
      this.Halt();
      this.faceGeneralDirection(this.Player.getStandingPosition());
    }

    public virtual void defaultMovementBehavior(GameTime time)
    {
      if (Game1.random.NextDouble() >= (double) (NetFieldBase<double, NetDouble>) this.jitteriness * 1.8 || this.skipHorizontal > 0)
        return;
      switch (Game1.random.Next(6))
      {
        case 0:
          this.SetMovingOnlyUp();
          break;
        case 1:
          this.SetMovingOnlyRight();
          break;
        case 2:
          this.SetMovingOnlyDown();
          break;
        case 3:
          this.SetMovingOnlyLeft();
          break;
        default:
          this.Halt();
          break;
      }
    }

    public virtual bool TakesDamageFromHitbox(Microsoft.Xna.Framework.Rectangle area_of_effect) => this.GetBoundingBox().Intersects(area_of_effect);

    public virtual bool OverlapsFarmerForDamage(Farmer who) => this.GetBoundingBox().Intersects(who.GetBoundingBox());

    public override void Halt()
    {
      int speed = this.speed;
      base.Halt();
      this.speed = speed;
    }

    public override void MovePosition(
      GameTime time,
      xTile.Dimensions.Rectangle viewport,
      GameLocation currentLocation)
    {
      if (this.stunTime > 0)
        return;
      this.lastPosition = this.Position;
      if ((double) this.xVelocity != 0.0 || (double) this.yVelocity != 0.0)
      {
        if (double.IsNaN((double) this.xVelocity) || double.IsNaN((double) this.yVelocity))
        {
          this.xVelocity = 0.0f;
          this.yVelocity = 0.0f;
        }
        Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
        int x = boundingBox.X;
        int y = boundingBox.Y;
        int b1 = boundingBox.X + (int) this.xVelocity;
        int b2 = boundingBox.Y - (int) this.yVelocity;
        int val1 = 1;
        bool flag1 = false;
        bool flag2 = false;
        if (this is SquidKid)
          flag2 = true;
        if (!this.isGlider.Value | flag2)
        {
          if (boundingBox.Width > 0 && Math.Abs((int) this.xVelocity) > boundingBox.Width)
            val1 = (int) Math.Max((double) val1, Math.Ceiling((double) Math.Abs((int) this.xVelocity) / (double) boundingBox.Width));
          if (boundingBox.Height > 0 && Math.Abs((int) this.yVelocity) > boundingBox.Height)
            val1 = (int) Math.Max((double) val1, Math.Ceiling((double) Math.Abs((int) this.yVelocity) / (double) boundingBox.Height));
        }
        for (int index = 1; index <= val1; ++index)
        {
          boundingBox.X = (int) Utility.Lerp((float) x, (float) b1, (float) index / (float) val1);
          boundingBox.Y = (int) Utility.Lerp((float) y, (float) b2, (float) index / (float) val1);
          bool glider = (bool) (NetFieldBase<bool, NetBool>) this.isGlider;
          if (flag2)
            glider = false;
          if (currentLocation != null && currentLocation.isCollidingPosition(boundingBox, viewport, false, this.DamageToFarmer, glider, (Character) this))
          {
            flag1 = true;
            break;
          }
        }
        if (!flag1)
        {
          this.position.X += this.xVelocity;
          this.position.Y -= this.yVelocity;
          if (this.Slipperiness < 1000)
          {
            this.xVelocity -= this.xVelocity / (float) this.Slipperiness;
            this.yVelocity -= this.yVelocity / (float) this.Slipperiness;
            if ((double) Math.Abs(this.xVelocity) <= 0.0500000007450581)
              this.xVelocity = 0.0f;
            if ((double) Math.Abs(this.yVelocity) <= 0.0500000007450581)
              this.yVelocity = 0.0f;
          }
          if (!(bool) (NetFieldBase<bool, NetBool>) this.isGlider && this.invincibleCountdown > 0)
          {
            this.slideAnimationTimer -= time.ElapsedGameTime.Milliseconds;
            if (this.slideAnimationTimer < 0 && ((double) Math.Abs(this.xVelocity) >= 3.0 || (double) Math.Abs(this.yVelocity) >= 3.0))
            {
              this.slideAnimationTimer = 100 - (int) ((double) Math.Abs(this.xVelocity) * 2.0 + (double) Math.Abs(this.yVelocity) * 2.0);
              Game1.multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(6, this.getStandingPosition() + new Vector2(-32f, -32f), Color.White * 0.75f, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 20f)
              {
                scale = 0.75f
              });
            }
          }
        }
        else if ((bool) (NetFieldBase<bool, NetBool>) this.isGlider || this.Slipperiness >= 8)
        {
          if ((bool) (NetFieldBase<bool, NetBool>) this.isGlider)
          {
            bool[] flagArray = Utility.horizontalOrVerticalCollisionDirections(boundingBox, (Character) this);
            if (flagArray[0])
            {
              this.xVelocity = -this.xVelocity;
              this.position.X += (float) Math.Sign(this.xVelocity);
              this.rotation += (float) (Math.PI + (double) Game1.random.Next(-10, 11) * Math.PI / 500.0);
            }
            if (flagArray[1])
            {
              this.yVelocity = -this.yVelocity;
              this.position.Y -= (float) Math.Sign(this.yVelocity);
              this.rotation += (float) (Math.PI + (double) Game1.random.Next(-10, 11) * Math.PI / 500.0);
            }
          }
          if (this.Slipperiness < 1000)
          {
            this.xVelocity -= (float) ((double) this.xVelocity / (double) this.Slipperiness / 4.0);
            this.yVelocity -= (float) ((double) this.yVelocity / (double) this.Slipperiness / 4.0);
            if ((double) Math.Abs(this.xVelocity) <= 0.0500000007450581)
              this.xVelocity = 0.0f;
            if ((double) Math.Abs(this.yVelocity) <= 0.0509999990463257)
              this.yVelocity = 0.0f;
          }
        }
        else
        {
          this.xVelocity -= this.xVelocity / (float) this.Slipperiness;
          this.yVelocity -= this.yVelocity / (float) this.Slipperiness;
          if ((double) Math.Abs(this.xVelocity) <= 0.0500000007450581)
            this.xVelocity = 0.0f;
          if ((double) Math.Abs(this.yVelocity) <= 0.0500000007450581)
            this.yVelocity = 0.0f;
        }
        if ((bool) (NetFieldBase<bool, NetBool>) this.isGlider)
          return;
      }
      if (this.moveUp)
      {
        if ((!Game1.eventUp || Game1.IsMultiplayer) && !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this) || this.isCharging)
        {
          this.position.Y -= (float) (this.speed + this.addedSpeed);
          if (!this.ignoreMovementAnimations)
            this.Sprite.AnimateUp(time);
          this.FacingDirection = 0;
          this.faceDirection(0);
        }
        else
        {
          Microsoft.Xna.Framework.Rectangle position = this.nextPosition(0);
          position.Width /= 4;
          bool flag3 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this);
          position.X += position.Width * 3;
          bool flag4 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this);
          if (flag3 && !flag4 && !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
            this.position.X += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
          else if (flag4 && !flag3 && !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
            this.position.X -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
          if (!currentLocation.isTilePassable(this.nextPosition(0), viewport) || !this.willDestroyObjectsUnderfoot)
            this.Halt();
          else if (this.willDestroyObjectsUnderfoot)
          {
            Vector2 vector2 = new Vector2((float) (this.getStandingX() / 64), (float) (this.getStandingY() / 64 - 1));
            if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(0), true))
            {
              currentLocation.playSound("stoneCrack");
              this.position.Y -= (float) (this.speed + this.addedSpeed);
            }
            else
              this.blockedInterval += time.ElapsedGameTime.Milliseconds;
          }
          if (this.onCollision != null)
            this.onCollision(currentLocation);
        }
      }
      else if (this.moveRight)
      {
        if ((!Game1.eventUp || Game1.IsMultiplayer) && !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this) || this.isCharging)
        {
          this.position.X += (float) (this.speed + this.addedSpeed);
          if (!this.ignoreMovementAnimations)
            this.Sprite.AnimateRight(time);
          this.FacingDirection = 1;
          this.faceDirection(1);
        }
        else
        {
          Microsoft.Xna.Framework.Rectangle position = this.nextPosition(1);
          position.Height /= 4;
          bool flag5 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this);
          position.Y += position.Height * 3;
          bool flag6 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this);
          if (flag5 && !flag6 && !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
            this.position.Y += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
          else if (flag6 && !flag5 && !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
            this.position.Y -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
          if (!currentLocation.isTilePassable(this.nextPosition(1), viewport) || !this.willDestroyObjectsUnderfoot)
            this.Halt();
          else if (this.willDestroyObjectsUnderfoot)
          {
            Vector2 vector2 = new Vector2((float) (this.getStandingX() / 64 + 1), (float) (this.getStandingY() / 64));
            if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(1), true))
            {
              currentLocation.playSound("stoneCrack");
              this.position.X += (float) (this.speed + this.addedSpeed);
            }
            else
              this.blockedInterval += time.ElapsedGameTime.Milliseconds;
          }
          if (this.onCollision != null)
            this.onCollision(currentLocation);
        }
      }
      else if (this.moveDown)
      {
        if ((!Game1.eventUp || Game1.IsMultiplayer) && !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this) || this.isCharging)
        {
          this.position.Y += (float) (this.speed + this.addedSpeed);
          if (!this.ignoreMovementAnimations)
            this.Sprite.AnimateDown(time);
          this.FacingDirection = 2;
          this.faceDirection(2);
        }
        else
        {
          Microsoft.Xna.Framework.Rectangle position = this.nextPosition(2);
          position.Width /= 4;
          bool flag7 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this);
          position.X += position.Width * 3;
          bool flag8 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this);
          if (flag7 && !flag8 && !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
            this.position.X += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
          else if (flag8 && !flag7 && !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
            this.position.X -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
          if (!currentLocation.isTilePassable(this.nextPosition(2), viewport) || !this.willDestroyObjectsUnderfoot)
            this.Halt();
          else if (this.willDestroyObjectsUnderfoot)
          {
            Vector2 vector2 = new Vector2((float) (this.getStandingX() / 64), (float) (this.getStandingY() / 64 + 1));
            if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(2), true))
            {
              currentLocation.playSound("stoneCrack");
              this.position.Y += (float) (this.speed + this.addedSpeed);
            }
            else
              this.blockedInterval += time.ElapsedGameTime.Milliseconds;
          }
          if (this.onCollision != null)
            this.onCollision(currentLocation);
        }
      }
      else if (this.moveLeft)
      {
        if ((!Game1.eventUp || Game1.IsMultiplayer) && !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this) || this.isCharging)
        {
          this.position.X -= (float) (this.speed + this.addedSpeed);
          this.FacingDirection = 3;
          if (!this.ignoreMovementAnimations)
            this.Sprite.AnimateLeft(time);
          this.faceDirection(3);
        }
        else
        {
          Microsoft.Xna.Framework.Rectangle position = this.nextPosition(3);
          position.Height /= 4;
          bool flag9 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this);
          position.Y += position.Height * 3;
          bool flag10 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this);
          if (flag9 && !flag10 && !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
            this.position.Y += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
          else if (flag10 && !flag9 && !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, false, this.DamageToFarmer, (bool) (NetFieldBase<bool, NetBool>) this.isGlider, (Character) this))
            this.position.Y -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
          if (!currentLocation.isTilePassable(this.nextPosition(3), viewport) || !this.willDestroyObjectsUnderfoot)
            this.Halt();
          else if (this.willDestroyObjectsUnderfoot)
          {
            Vector2 vector2 = new Vector2((float) (this.getStandingX() / 64 - 1), (float) (this.getStandingY() / 64));
            if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(3), true))
            {
              currentLocation.playSound("stoneCrack");
              this.position.X -= (float) (this.speed + this.addedSpeed);
            }
            else
              this.blockedInterval += time.ElapsedGameTime.Milliseconds;
          }
          if (this.onCollision != null)
            this.onCollision(currentLocation);
        }
      }
      else if (!this.ignoreMovementAnimations)
      {
        if (this.moveUp)
          this.Sprite.AnimateUp(time);
        else if (this.moveRight)
          this.Sprite.AnimateRight(time);
        else if (this.moveDown)
          this.Sprite.AnimateDown(time);
        else if (this.moveLeft)
          this.Sprite.AnimateLeft(time);
      }
      if ((this.blockedInterval < 3000 || (double) this.blockedInterval > 3750.0) && this.blockedInterval >= 5000)
      {
        this.speed = 4;
        this.isCharging = true;
        this.blockedInterval = 0;
      }
      if (this.DamageToFarmer <= 0 || Game1.random.NextDouble() >= 0.000333333333333333)
        return;
      if (this.Name.Equals("Shadow Guy") && Game1.random.NextDouble() < 0.3)
      {
        if (Game1.random.NextDouble() < 0.5)
          currentLocation.playSound("grunt");
        else
          currentLocation.playSound("shadowpeep");
      }
      else
      {
        if (this.Name.Equals("Shadow Girl"))
          return;
        if (this.Name.Equals("Ghost"))
        {
          currentLocation.playSound("ghost");
        }
        else
        {
          if (this.Name.Contains("Slime"))
            return;
          this.Name.Contains("Jelly");
        }
      }
    }

    protected delegate void collisionBehavior(GameLocation location);
  }
}
