// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.ShadowGuy
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewValley.Monsters
{
  public class ShadowGuy : Monster
  {
    public const int visionDistance = 8;
    public const int spellCooldown = 1500;
    private bool spottedPlayer;
    private bool casting;
    private bool teleporting;
    private int coolDown = 1500;
    private IEnumerator<Point> teleportationPath;
    private float rotationTimer;

    public ShadowGuy()
    {
    }

    public ShadowGuy(Vector2 position)
      : base("Shadow Guy", position)
    {
      if (Game1.MasterPlayer.friendshipData.ContainsKey("???") && Game1.MasterPlayer.friendshipData["???"].Points >= 1250)
        this.DamageToFarmer = 0;
      this.Halt();
    }

    public override void reloadSprite() => this.Sprite = new AnimatedSprite("Characters\\Monsters\\Shadow " + ((double) this.Position.X % 4.0 == 0.0 ? "Girl" : "Guy"));

    public override void draw(SpriteBatch b)
    {
      if (!this.casting)
      {
        base.draw(b);
      }
      else
      {
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (32 + Game1.random.Next(-8, 9)), (float) (64 + Game1.random.Next(-8, 9))), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), Color.White * 0.5f, this.rotation, new Vector2(32f, 64f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale), this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (32 + Game1.random.Next(-8, 9)), (float) (64 + Game1.random.Next(-8, 9))), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), Color.White * 0.5f, this.rotation, new Vector2(32f, 64f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale), this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) (this.getStandingY() + 1) / 10000f));
        for (int index = 0; index < 8; ++index)
          b.Draw(Projectile.projectileSheet, Game1.GlobalToLocal(Game1.viewport, this.getStandingPosition()), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(212, 20, 24, 24)), Color.White * 0.7f, this.rotationTimer + (float) ((double) index * 3.14159274101257 / 4.0), new Vector2(32f, 256f), 1.5f, SpriteEffects.None, 0.95f);
      }
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
        if (who.CurrentTool.Name.Equals("Holy Sword") && !isBomb)
        {
          this.Health -= damage * 3 / 4;
          this.currentLocation.debris.Add(new Debris((damage * 3 / 4).ToString() ?? "", 1, new Vector2((float) this.getStandingX(), (float) this.getStandingY()), Color.LightBlue, 1f, 0.0f));
        }
        this.Health -= damage1;
        if (this.casting && Game1.random.NextDouble() < 0.5)
          this.coolDown += 200;
        else if (Game1.random.NextDouble() < 0.4 + 1.0 / (double) this.Health && !this.currentLocation.IsFarm)
        {
          this.castTeleport();
          if (this.Health <= 10)
            this.speed = Math.Min(3, this.speed + 1);
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

    protected override void localDeathAnimation() => this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(45, this.Position, Color.White, 10));

    protected override void sharedDeathAnimation()
    {
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Microsoft.Xna.Framework.Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, 64, 21), 64, this.getStandingX(), this.getStandingY() - 32, 1, this.getStandingY() / 64, Color.White);
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Microsoft.Xna.Framework.Rectangle(this.Sprite.SourceRect.X + 10, this.Sprite.SourceRect.Y + 21, 64, 21), 42, this.getStandingX(), this.getStandingY() - 32, 1, this.getStandingY() / 64, Color.White);
    }

    public void castTeleport()
    {
      int num = 0;
      Vector2 vector2;
      for (vector2 = new Vector2(this.getTileLocation().X + (Game1.random.NextDouble() < 0.5 ? (float) Game1.random.Next(-5, -1) : (float) Game1.random.Next(2, 6)), this.getTileLocation().Y + (Game1.random.NextDouble() < 0.5 ? (float) Game1.random.Next(-5, -1) : (float) Game1.random.Next(2, 6))); num < 6 && (!this.currentLocation.isTileOnMap(vector2) || !this.currentLocation.isTileLocationOpen(new Location((int) vector2.X, (int) vector2.Y)) || this.currentLocation.isTileOccupiedForPlacement(vector2)); ++num)
        vector2 = new Vector2(this.getTileLocation().X + (Game1.random.NextDouble() < 0.5 ? (float) Game1.random.Next(-5, -1) : (float) Game1.random.Next(2, 6)), this.getTileLocation().Y + (Game1.random.NextDouble() < 0.5 ? (float) Game1.random.Next(-5, -1) : (float) Game1.random.Next(2, 6)));
      if (num >= 6)
        return;
      this.teleporting = true;
      this.teleportationPath = Utility.GetPointsOnLine((int) this.getTileLocation().X, (int) this.getTileLocation().Y, (int) vector2.X, (int) vector2.Y, true).GetEnumerator();
      this.coolDown = 20;
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      base.behaviorAtGameTick(time);
      if ((double) this.timeBeforeAIMovementAgain <= 0.0)
        this.IsInvisible = false;
      if (this.teleporting)
      {
        this.coolDown -= time.ElapsedGameTime.Milliseconds;
        if (this.coolDown > 0)
          return;
        if (this.teleportationPath.MoveNext())
        {
          Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite((string) (NetFieldBase<string, NetString>) this.Sprite.textureName, this.Sprite.SourceRect, this.Position, false, 0.04f, Color.White));
          this.Position = new Vector2((float) (this.teleportationPath.Current.X * 64 + 4), (float) (this.teleportationPath.Current.Y * 64 - 32 - 4));
          this.coolDown = 20;
        }
        else
        {
          this.teleporting = false;
          this.coolDown = 500;
        }
      }
      else if (!this.spottedPlayer && Utility.couldSeePlayerInPeripheralVision(this.Player, (Character) this) && Utility.doesPointHaveLineOfSightInMine(this.currentLocation, this.getTileLocation(), this.Player.getTileLocation(), 8))
      {
        this.controller = (PathFindController) null;
        this.spottedPlayer = true;
        this.Halt();
        this.facePlayer(this.Player);
        if (Game1.random.NextDouble() >= 0.3)
          return;
        this.currentLocation.playSound("shadowpeep");
      }
      else if (this.casting)
      {
        this.Halt();
        this.IsWalkingTowardPlayer = false;
        TimeSpan timeSpan = time.TotalGameTime;
        this.rotationTimer = (float) ((double) timeSpan.Milliseconds * 0.0245436932891607 / 24.0 % (1024.0 * Math.PI));
        int coolDown = this.coolDown;
        timeSpan = time.ElapsedGameTime;
        int milliseconds = timeSpan.Milliseconds;
        this.coolDown = coolDown - milliseconds;
        if (this.coolDown > 0)
          return;
        this.Scale = 1f;
        Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer(this.GetBoundingBox().Center, 15f, this.Player);
        if (this.Player.attack >= 0 && Game1.random.NextDouble() < 0.6)
        {
          this.currentLocation.projectiles.Add((Projectile) new DebuffingProjectile(18, 2, 4, 4, 0.1963495f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2((float) this.GetBoundingBox().X, (float) this.GetBoundingBox().Y)));
        }
        else
        {
          this.currentLocation.playSound("fireball");
          this.currentLocation.projectiles.Add((Projectile) new BasicProjectile(10, 3, 0, 3, 0.0f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2((float) this.GetBoundingBox().X, (float) this.GetBoundingBox().Y)));
        }
        this.casting = false;
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
          this.casting = true;
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
