// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Leaper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using System;
using xTile.Dimensions;

namespace StardewValley.Monsters
{
  public class Leaper : Monster
  {
    public NetFloat leapDuration = new NetFloat(0.75f);
    public NetFloat leapProgress = new NetFloat(0.0f);
    public NetBool leaping = new NetBool(false);
    public NetVector2 leapStartPosition = new NetVector2();
    public NetVector2 leapEndPosition = new NetVector2();
    public float nextLeap;

    public Leaper()
    {
    }

    public Leaper(Vector2 position)
      : base("Spider", position)
    {
      this.forceOneTileWide.Value = true;
      this.IsWalkingTowardPlayer = false;
      this.nextLeap = Utility.RandomFloat(1f, 1.5f);
      this.isHardModeMonster.Value = true;
      this.reloadSprite();
    }

    public override int GetBaseDifficultyLevel() => 1;

    public override void reloadSprite()
    {
      base.reloadSprite();
      this.Sprite.SpriteWidth = 32;
      this.Sprite.SpriteHeight = 32;
      this.Sprite.UpdateSourceRect();
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.leapDuration, (INetSerializable) this.leapProgress, (INetSerializable) this.leapStartPosition, (INetSerializable) this.leapEndPosition, (INetSerializable) this.leaping);
      this.leapProgress.Interpolated(true, true);
      this.leaping.Interpolated(true, true);
      this.leaping.fieldChangeVisibleEvent += new NetFieldBase<bool, NetBool>.FieldChange(this.OnLeapingChanged);
    }

    public virtual void OnLeapingChanged(NetBool field, bool old_value, bool new_value)
    {
    }

    public override bool isInvincible() => this.leaping.Value || base.isInvincible();

    public override void updateMovement(GameLocation location, GameTime time)
    {
    }

    protected override void localDeathAnimation()
    {
      this.currentLocation.localSound("monsterdead");
      Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.DarkRed, 10)
      {
        holdLastFrame = true,
        alphaFade = 0.01f,
        interval = 70f
      }, this.currentLocation);
    }

    protected override void sharedDeathAnimation()
    {
    }

    public override void defaultMovementBehavior(GameTime time)
    {
    }

    public override void noMovementProgressNearPlayerBehavior()
    {
    }

    public override void update(GameTime time, GameLocation location)
    {
      this.farmerPassesThrough = true;
      base.update(time, location);
      if (this.leaping.Value)
      {
        this.yJumpGravity = 0.0f;
        float num1 = this.leapProgress.Value;
        if (!Game1.IsMasterGame)
        {
          float num2 = (this.leapStartPosition.Value - (Vector2) (NetFieldBase<Vector2, NetVector2>) this.leapEndPosition).Length();
          num1 = (double) num2 != 0.0 ? (this.leapStartPosition.Value - this.Position).Length() / num2 : 0.0f;
          if ((double) num1 < 0.0)
            num1 = 0.0f;
          if ((double) num1 > 1.0)
            num1 = 1f;
        }
        this.yJumpOffset = (int) (Math.Sin((double) num1 * Math.PI) * -64.0 * 3.0);
      }
      else
        this.yJumpOffset = 0;
    }

    protected override void updateAnimation(GameTime time)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.leaping)
        this.Sprite.CurrentFrame = 2;
      else
        this.Sprite.Animate(time, 0, 2, 500f);
      this.Sprite.UpdateSourceRect();
    }

    public virtual bool IsValidLandingTile(Vector2 tile, bool check_other_characters = false)
    {
      if (this.currentLocation is MineShaft && !(this.currentLocation as MineShaft).isTileOnClearAndSolidGround(tile) || this.currentLocation.isTileOccupied(tile, ignoreAllCharacters: true) || !this.currentLocation.isTileOnMap(tile) || !this.currentLocation.isTilePassable(new Location((int) tile.X, (int) tile.Y), Game1.viewport))
        return false;
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      if (check_other_characters && this.currentLocation != null)
      {
        foreach (Character character in this.currentLocation.characters)
        {
          if (character != this && character.GetBoundingBox().Intersects(boundingBox))
            return false;
        }
      }
      return true;
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      base.behaviorAtGameTick(time);
      if (this.leaping.Value)
      {
        this.leapProgress.Value += (float) time.ElapsedGameTime.TotalSeconds / this.leapDuration.Value;
        if ((double) this.leapProgress.Value >= 1.0)
          this.leapProgress.Value = 1f;
        this.Position = new Vector2(Utility.Lerp(this.leapStartPosition.X, this.leapEndPosition.X, (float) (NetFieldBase<float, NetFloat>) this.leapProgress), Utility.Lerp(this.leapStartPosition.Y, this.leapEndPosition.Y, (float) (NetFieldBase<float, NetFloat>) this.leapProgress));
        if ((double) this.leapProgress.Value != 1.0)
          return;
        this.leaping.Value = false;
        this.leapProgress.Value = 0.0f;
        if (this.IsValidLandingTile(this.getTileLocation(), true))
          return;
        this.nextLeap = 0.1f;
      }
      else
      {
        if ((double) this.nextLeap > 0.0)
          this.nextLeap -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.nextLeap > 0.0)
          return;
        Vector2? nullable = new Vector2?();
        Vector2 tileLocation1 = this.getTileLocation();
        tileLocation1.X = (float) (int) tileLocation1.X;
        tileLocation1.X = (float) (int) tileLocation1.X;
        if (this.withinPlayerThreshold(5) && this.Player != null)
        {
          Vector2 tileLocation2 = this.getTileLocation();
          if (Game1.random.NextDouble() < 0.600000023841858)
          {
            this.nextLeap = Utility.RandomFloat(1.25f, 1.5f);
            tileLocation2 = this.Player.getTileLocation();
            tileLocation2.X = (float) (int) Math.Round((double) tileLocation2.X);
            tileLocation2.Y = (float) (int) Math.Round((double) tileLocation2.Y);
            tileLocation2.X += (float) Game1.random.Next(-1, 2);
            tileLocation2.Y += (float) Game1.random.Next(-1, 2);
          }
          else
          {
            this.nextLeap = Utility.RandomFloat(0.1f, 0.2f);
            tileLocation2.X += (float) Game1.random.Next(-1, 2);
            tileLocation2.Y += (float) Game1.random.Next(-1, 2);
          }
          if (this.IsValidLandingTile(tileLocation2))
            nullable = new Vector2?(tileLocation2);
        }
        if (!nullable.HasValue)
        {
          for (int index = 0; index < 8; ++index)
          {
            Vector2 vector2 = new Vector2((float) Game1.random.Next(-4, 5), (float) Game1.random.Next(-4, 5));
            if (!(vector2 == Vector2.Zero))
            {
              Vector2 tile = tileLocation1 + vector2;
              if (this.IsValidLandingTile(tile))
              {
                this.nextLeap = Utility.RandomFloat(0.6f, 1.5f);
                nullable = new Vector2?(tile);
                break;
              }
            }
          }
        }
        if (nullable.HasValue)
        {
          if (Utility.isOnScreen(this.Position, 128))
            this.currentLocation.playSound("batFlap");
          this.leapProgress.Value = 0.0f;
          this.leaping.Value = true;
          this.leapStartPosition.Value = this.Position;
          this.leapEndPosition.Value = nullable.Value * 64f;
        }
        else
          this.nextLeap = Utility.RandomFloat(0.25f, 0.5f);
      }
    }

    public override void shedChunks(int number, float scale)
    {
      GameLocation currentLocation = this.currentLocation;
      string textureName = (string) (NetFieldBase<string, NetString>) this.Sprite.textureName;
      Microsoft.Xna.Framework.Rectangle sourcerectangle = new Microsoft.Xna.Framework.Rectangle(0, 64, 16, 16);
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      int x = boundingBox.Center.X;
      boundingBox = this.GetBoundingBox();
      int y1 = boundingBox.Center.Y;
      int numberOfChunks = number;
      int y2 = (int) this.getTileLocation().Y;
      Color white = Color.White;
      Game1.createRadialDebris(currentLocation, textureName, sourcerectangle, 8, x, y1, numberOfChunks, y2, white, 4f);
    }
  }
}
