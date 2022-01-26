// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Spiker
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Xml.Serialization;

namespace StardewValley.Monsters
{
  public class Spiker : Monster
  {
    [XmlIgnore]
    public int targetDirection;
    [XmlIgnore]
    public NetBool moving = new NetBool(false);
    protected bool _localMoving;
    [XmlIgnore]
    public float nextMoveCheck;

    public Spiker()
    {
    }

    public Spiker(Vector2 position, int direction)
      : base(nameof (Spiker), position)
    {
      this.Sprite.SpriteWidth = 16;
      this.Sprite.SpriteHeight = 16;
      this.Sprite.UpdateSourceRect();
      this.targetDirection = direction;
      this.speed = 14;
      this.ignoreMovementAnimations = true;
      this.onCollision = new Monster.collisionBehavior(this.collide);
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.moving);
    }

    public static Vector3? GetSpawnPosition(GameLocation location, Point start_point)
    {
      Vector2 zero1 = Vector2.Zero;
      int num1 = Game1.random.Next(0, 2) == 0 ? 1 : -1;
      int num2 = 0;
      Vector2 zero2 = Vector2.Zero;
      int num3 = 0;
      Vector2 zero3 = Vector2.Zero;
      Point p1 = start_point;
      p1.Y += num1;
      while (location.isTileOnMap(p1.X, p1.Y) && location.getTileIndexAt(p1, "Buildings") < 0)
      {
        p1.Y += num1;
        ++num2;
      }
      Point p2 = start_point;
      p2.Y -= num1;
      while (location.isTileOnMap(p2.X, p2.Y) && location.getTileIndexAt(p2, "Buildings") < 0)
      {
        zero2.X = (float) p2.X;
        zero2.Y = (float) p2.Y;
        p2.Y -= num1;
        ++num2;
      }
      p2 = start_point;
      p2.X += num1;
      while (location.isTileOnMap(p2.X, p2.Y) && location.getTileIndexAt(p2, "Buildings") < 0)
      {
        p2.X += num1;
        ++num3;
      }
      p2 = start_point;
      p2.X -= num1;
      while (location.isTileOnMap(p2.X, p2.Y) && location.getTileIndexAt(p2, "Buildings") < 0)
      {
        zero3.X = (float) p2.X;
        zero3.Y = (float) p2.Y;
        p2.X -= num1;
        ++num3;
      }
      return num3 < num2 ? (num3 <= 4 ? new Vector3?() : new Vector3?(new Vector3(zero3.X, zero3.Y, num1 == 1 ? 1f : 3f))) : (num2 <= 4 ? new Vector3?() : new Vector3?(new Vector3(zero2.X, zero2.Y, num1 == 1 ? 2f : 0.0f)));
    }

    public override void update(GameTime time, GameLocation location)
    {
      base.update(time, location);
      if (this.moving.Value == this._localMoving)
        return;
      this._localMoving = this.moving.Value;
      if (this._localMoving)
      {
        if (this.currentLocation != Game1.currentLocation || !Utility.isOnScreen(this.Position, 64))
          return;
        Game1.playSound("parry");
      }
      else
      {
        if (this.currentLocation != Game1.currentLocation || !Utility.isOnScreen(this.Position, 64))
          return;
        Game1.playSound("hammer");
      }
    }

    public override bool passThroughCharacters() => true;

    public override void draw(SpriteBatch b) => this.Sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.Position), (float) this.GetBoundingBox().Center.Y / 10000f);

    private void collide(GameLocation location)
    {
      Rectangle rectangle = this.nextPosition(this.FacingDirection);
      foreach (Character farmer in location.farmers)
      {
        if (farmer.GetBoundingBox().Intersects(rectangle))
          return;
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.moving)
        return;
      this.moving.Value = false;
      this.targetDirection = (this.targetDirection + 2) % 4;
      this.nextMoveCheck = 0.75f;
    }

    public override void updateMovement(GameLocation location, GameTime time)
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
      return -1;
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      if ((double) this.nextMoveCheck > 0.0)
        this.nextMoveCheck -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.nextMoveCheck <= 0.0)
      {
        this.nextMoveCheck = 0.25f;
        foreach (Farmer farmer in this.currentLocation.farmers)
        {
          if ((this.targetDirection == 0 || this.targetDirection == 2) && Math.Abs(farmer.getTileX() - this.getTileX()) <= 1)
          {
            if (this.targetDirection == 0 && (double) farmer.Position.Y < (double) this.Position.Y)
            {
              this.moving.Value = true;
              break;
            }
            if (this.targetDirection == 2 && (double) farmer.Position.Y > (double) this.Position.Y)
            {
              this.moving.Value = true;
              break;
            }
          }
          if ((this.targetDirection == 3 || this.targetDirection == 1) && Math.Abs(farmer.getTileY() - this.getTileY()) <= 1)
          {
            if (this.targetDirection == 3 && (double) farmer.Position.X < (double) this.Position.X)
            {
              this.moving.Value = true;
              break;
            }
            if (this.targetDirection == 1 && (double) farmer.Position.X > (double) this.Position.X)
            {
              this.moving.Value = true;
              break;
            }
          }
        }
      }
      this.moveUp = false;
      this.moveDown = false;
      this.moveLeft = false;
      this.moveRight = false;
      if (this.moving.Value)
      {
        if (this.targetDirection == 0)
          this.moveUp = true;
        if (this.targetDirection == 2)
          this.moveDown = true;
        else if (this.targetDirection == 3)
          this.moveLeft = true;
        else if (this.targetDirection == 1)
          this.moveRight = true;
        this.MovePosition(time, Game1.viewport, this.currentLocation);
      }
      this.faceDirection(2);
    }
  }
}
