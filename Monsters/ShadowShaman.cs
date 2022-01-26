// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.ShadowShaman
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
  public class ShadowShaman : Monster
  {
    public const int visionDistance = 8;
    public const int spellCooldown = 1500;
    private bool spottedPlayer;
    private readonly NetBool casting = new NetBool();
    private int coolDown = 1500;
    private float rotationTimer;

    public ShadowShaman()
    {
    }

    public ShadowShaman(Vector2 position)
      : base("Shadow Shaman", position)
    {
      if (!Game1.MasterPlayer.friendshipData.ContainsKey("???") || Game1.MasterPlayer.friendshipData["???"].Points < 1250)
        return;
      this.DamageToFarmer = 0;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.casting);
    }

    public override void reloadSprite() => this.Sprite = new AnimatedSprite("Characters\\Monsters\\Shadow Shaman");

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.casting)
        return;
      for (int index = 0; index < 8; ++index)
        b.Draw(Projectile.projectileSheet, Game1.GlobalToLocal(Game1.viewport, this.getStandingPosition()), new Rectangle?(new Rectangle(119, 6, 3, 3)), Color.White * 0.7f, this.rotationTimer + (float) ((double) index * 3.14159274101257 / 4.0), new Vector2(8f, 48f), 6f, SpriteEffects.None, 0.95f);
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
        if (who.CurrentTool != null && who.CurrentTool.Name != null && who.CurrentTool.Name.Equals("Holy Sword") && !isBomb)
        {
          this.Health -= damage * 3 / 4;
          this.currentLocation.debris.Add(new Debris((damage * 3 / 4).ToString() ?? "", 1, new Vector2((float) this.getStandingX(), (float) this.getStandingY()), Color.LightBlue, 1f, 0.0f));
        }
        this.Health -= damage1;
        if ((bool) (NetFieldBase<bool, NetBool>) this.casting && Game1.random.NextDouble() < 0.5)
        {
          this.coolDown += 200;
        }
        else
        {
          this.setTrajectory(xTrajectory, yTrajectory);
          this.currentLocation.playSound("shadowHit");
        }
        if (this.Health <= 0)
        {
          this.currentLocation.playSound("shadowDie");
          this.deathAnimation();
        }
      }
      return damage1;
    }

    protected override void sharedDeathAnimation()
    {
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, 16, 5), 16, this.getStandingX(), this.getStandingY() - 32, 1, this.getStandingY() / 64, Color.White);
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(this.Sprite.SourceRect.X + 2, this.Sprite.SourceRect.Y + 5, 16, 5), 10, this.getStandingX(), this.getStandingY() - 32, 1, this.getStandingY() / 64, Color.White);
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(0, 10, 16, 5), 16, this.getStandingX(), this.getStandingY() - 32, 1, this.getStandingY() / 64, Color.White);
    }

    protected override void localDeathAnimation()
    {
      Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(45, this.Position, Color.White, 10), this.currentLocation);
      for (int index = 1; index < 3; ++index)
      {
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(1f, 1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(1f, -1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(-1f, 1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(-1f, -1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
      }
    }

    protected override void updateMonsterSlaveAnimation(GameTime time)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.casting)
      {
        this.Sprite.Animate(time, 16, 4, 200f);
        this.rotationTimer = (float) ((double) time.TotalGameTime.Milliseconds * 0.0245436932891607 / 24.0 % (1024.0 * Math.PI));
      }
      if (this.isMoving())
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
      base.behaviorAtGameTick(time);
      if ((double) this.timeBeforeAIMovementAgain <= 0.0)
        this.IsInvisible = false;
      if (!this.spottedPlayer && Utility.couldSeePlayerInPeripheralVision(this.Player, (Character) this) && Utility.doesPointHaveLineOfSightInMine(this.currentLocation, this.getTileLocation(), this.Player.getTileLocation(), 8))
      {
        this.controller = (PathFindController) null;
        this.spottedPlayer = true;
        this.Halt();
        this.facePlayer(this.Player);
        if (Game1.random.NextDouble() >= 0.3)
          return;
        this.currentLocation.playSound("shadowpeep");
      }
      else if ((bool) (NetFieldBase<bool, NetBool>) this.casting)
      {
        this.IsWalkingTowardPlayer = false;
        this.Sprite.Animate(time, 16, 4, 200f);
        this.rotationTimer = (float) ((double) time.TotalGameTime.Milliseconds * 0.0245436932891607 / 24.0 % (1024.0 * Math.PI));
        this.coolDown -= time.ElapsedGameTime.Milliseconds;
        if (this.coolDown > 0)
          return;
        this.Scale = 1f;
        Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer(this.GetBoundingBox().Center, 15f, this.Player);
        if (this.Player.attack >= 0 && Game1.random.NextDouble() < 0.6)
        {
          this.currentLocation.projectiles.Add((Projectile) new DebuffingProjectile(14, 7, 4, 4, 0.1963495f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2((float) this.GetBoundingBox().X, (float) this.GetBoundingBox().Y), this.currentLocation, (Character) this));
        }
        else
        {
          List<Monster> monsterList = new List<Monster>();
          foreach (NPC character in this.currentLocation.characters)
          {
            if (character is Monster && (character as Monster).withinPlayerThreshold(6))
              monsterList.Add((Monster) character);
          }
          Monster monster1 = (Monster) null;
          double num1 = 1.0;
          foreach (Monster monster2 in monsterList)
          {
            if ((double) monster2.Health / (double) monster2.MaxHealth <= num1)
            {
              monster1 = monster2;
              num1 = (double) monster2.Health / (double) monster2.MaxHealth;
            }
          }
          if (monster1 != null)
          {
            int num2 = (bool) (NetFieldBase<bool, NetBool>) this.isHardModeMonster ? 250 : 60;
            monster1.Health = Math.Min(monster1.MaxHealth, monster1.Health + num2);
            this.currentLocation.playSound("healSound");
            Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 256, 64, 64), 40f, 8, 0, monster1.Position + new Vector2(32f, 64f), false, false));
            NetCollection<Debris> debris1 = this.currentLocation.debris;
            int number = num2;
            Rectangle boundingBox = monster1.GetBoundingBox();
            double x = (double) boundingBox.Center.X;
            boundingBox = monster1.GetBoundingBox();
            double y = (double) boundingBox.Center.Y;
            Vector2 debrisOrigin = new Vector2((float) x, (float) y);
            Color green = Color.Green;
            Monster toHover = monster1;
            Debris debris2 = new Debris(number, debrisOrigin, green, 1f, (Character) toHover);
            debris1.Add(debris2);
          }
        }
        this.casting.Value = false;
        this.coolDown = 1500;
        this.IsWalkingTowardPlayer = true;
      }
      else if (this.spottedPlayer && this.withinPlayerThreshold(8))
      {
        if (this.Health < 30)
        {
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
        }
        else if (this.controller == null && !Utility.doesPointHaveLineOfSightInMine(this.currentLocation, this.getTileLocation(), this.Player.getTileLocation(), 8))
        {
          this.controller = new PathFindController((Character) this, this.currentLocation, new Point((int) this.Player.getTileLocation().X, (int) this.Player.getTileLocation().Y), -1, (PathFindController.endBehavior) null, 300);
          if (this.controller == null || this.controller.pathToEndPoint == null || this.controller.pathToEndPoint.Count == 0)
          {
            this.spottedPlayer = false;
            this.Halt();
            this.controller = (PathFindController) null;
            this.addedSpeed = 0;
          }
        }
        else if (this.coolDown <= 0 && Game1.random.NextDouble() < 0.02)
        {
          this.casting.Value = true;
          this.controller = (PathFindController) null;
          this.IsWalkingTowardPlayer = false;
          this.Halt();
          this.coolDown = 500;
        }
        this.coolDown -= time.ElapsedGameTime.Milliseconds;
      }
      else if (this.spottedPlayer)
      {
        this.IsWalkingTowardPlayer = false;
        this.spottedPlayer = false;
        this.controller = (PathFindController) null;
        this.addedSpeed = 0;
      }
      else
        this.defaultMovementBehavior(time);
    }
  }
}
