// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.ShadowGirl
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System;

namespace StardewValley.Monsters
{
  public class ShadowGirl : Monster
  {
    public const int blockTimeBeforePathfinding = 500;
    private new Vector2 lastPosition = Vector2.Zero;
    private int howLongOnThisPosition;

    public ShadowGirl()
    {
    }

    public ShadowGirl(Vector2 position)
      : base("Shadow Girl", position)
    {
      this.IsWalkingTowardPlayer = false;
      this.moveTowardPlayerThreshold.Value = 8;
      if (!Game1.MasterPlayer.friendshipData.ContainsKey("???") || Game1.MasterPlayer.friendshipData["???"].Points < 1250)
        return;
      this.DamageToFarmer = 0;
    }

    public override void reloadSprite() => this.Sprite = new AnimatedSprite("Characters\\Monsters\\Shadow Girl");

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
        if (this.Player.CurrentTool.Name.Equals("Holy Sword") && !isBomb)
        {
          this.Health -= damage * 3 / 4;
          this.currentLocation.debris.Add(new Debris((damage * 3 / 4).ToString() ?? "", 1, new Vector2((float) this.getStandingX(), (float) this.getStandingY()), Color.LightBlue, 1f, 0.0f));
        }
        this.Health -= damage1;
        this.setTrajectory(xTrajectory, yTrajectory);
        if (this.Health <= 0)
          this.deathAnimation();
      }
      return damage1;
    }

    protected override void localDeathAnimation() => this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(45, this.Position, Color.White, 10));

    protected override void sharedDeathAnimation()
    {
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, 64, 21), 64, this.getStandingX(), this.getStandingY() - 32, 1, this.getStandingY() / 64, Color.White);
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(this.Sprite.SourceRect.X + 10, this.Sprite.SourceRect.Y + 21, 64, 21), 42, this.getStandingX(), this.getStandingY() - 32, 1, this.getStandingY() / 64, Color.White);
    }

    public override void update(GameTime time, GameLocation location)
    {
      if (!location.farmers.Any())
        return;
      if (!this.Player.isRafting || !this.withinPlayerThreshold(4))
      {
        this.updateGlow();
        this.updateEmote(time);
        if (this.controller == null)
          this.updateMovement(location, time);
        if (this.controller != null && this.controller.update(time))
          this.controller = (PathFindController) null;
      }
      this.behaviorAtGameTick(time);
      if ((double) this.Position.X >= 0.0 && (double) this.Position.X <= (double) (location.map.GetLayer("Back").LayerWidth * 64) && (double) this.Position.Y >= 0.0 && (double) this.Position.Y <= (double) (location.map.GetLayer("Back").LayerHeight * 64))
        return;
      location.characters.Remove((NPC) this);
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      base.behaviorAtGameTick(time);
      this.addedSpeed = 0;
      this.speed = 3;
      if (this.howLongOnThisPosition > 500 && this.controller == null)
      {
        this.IsWalkingTowardPlayer = false;
        this.controller = new PathFindController((Character) this, this.currentLocation, new Point((int) this.Player.getTileLocation().X, (int) this.Player.getTileLocation().Y), Game1.random.Next(4), (PathFindController.endBehavior) null, 300);
        this.timeBeforeAIMovementAgain = 2000f;
        this.howLongOnThisPosition = 0;
      }
      else if (this.controller == null)
        this.IsWalkingTowardPlayer = true;
      if (this.Position.Equals(this.lastPosition))
        this.howLongOnThisPosition += time.ElapsedGameTime.Milliseconds;
      else
        this.howLongOnThisPosition = 0;
      this.lastPosition = this.Position;
    }
  }
}
