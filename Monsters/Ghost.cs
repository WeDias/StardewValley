// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Ghost
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
  public class Ghost : Monster
  {
    public const float rotationIncrement = 0.04908739f;
    private int wasHitCounter;
    private float targetRotation;
    private bool turningRight;
    private bool seenPlayer;
    private int identifier = Game1.random.Next(-99999, 99999);
    private int yOffset;
    private int yOffsetExtra;
    public NetInt currentState = new NetInt(0);
    public float stateTimer = -1f;
    public float nextParticle;
    public NetEnum<Ghost.GhostVariant> variant = new NetEnum<Ghost.GhostVariant>(Ghost.GhostVariant.Normal);

    public Ghost()
    {
    }

    public Ghost(Vector2 position)
      : base(nameof (Ghost), position)
    {
      this.Slipperiness = 8;
      this.isGlider.Value = true;
      this.HideShadow = true;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.variant, (INetSerializable) this.currentState);
      this.currentState.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((field, old_value, new_value) => this.stateTimer = -1f);
    }

    /// <summary>constructor for non-default ghosts</summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    public Ghost(Vector2 position, string name)
      : base(name, position)
    {
      this.Slipperiness = 8;
      this.isGlider.Value = true;
      this.HideShadow = true;
      if (!(name == "Putrid Ghost"))
        return;
      this.variant.Value = Ghost.GhostVariant.Putrid;
    }

    public override void reloadSprite() => this.Sprite = new AnimatedSprite("Characters\\Monsters\\" + (string) (NetFieldBase<string, NetString>) this.name);

    public override int GetBaseDifficultyLevel() => this.variant.Value == Ghost.GhostVariant.Putrid ? 1 : base.GetBaseDifficultyLevel();

    public override List<Item> getExtraDropItems()
    {
      if (Game1.random.NextDouble() >= 0.095 || !Game1.player.team.SpecialOrderActive("Wizard") || Game1.MasterPlayer.hasOrWillReceiveMail("ectoplasmDrop"))
        return base.getExtraDropItems();
      StardewValley.Object object1 = new StardewValley.Object(875, 1);
      object1.specialItem = true;
      StardewValley.Object object2 = object1;
      object2.questItem.Value = true;
      return new List<Item>() { (Item) object2 };
    }

    public override void drawAboveAllLayers(SpriteBatch b)
    {
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (21 + this.yOffset)), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), Color.White, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));
      SpriteBatch spriteBatch = b;
      Texture2D shadowTexture = Game1.shadowTexture;
      Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, 64f);
      Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
      Color white = Color.White;
      Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
      double x = (double) bounds.Center.X;
      bounds = Game1.shadowTexture.Bounds;
      double y = (double) bounds.Center.Y;
      Vector2 origin = new Vector2((float) x, (float) y);
      double scale = 3.0 + (double) this.yOffset / 20.0;
      double layerDepth = (double) (this.getStandingY() - 1) / 10000.0;
      spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
    }

    public override int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      Farmer who)
    {
      if (this.variant.Value == Ghost.GhostVariant.Putrid && this.currentState.Value <= 2)
        this.currentState.Value = 0;
      int damage1 = Math.Max(1, damage - (int) (NetFieldBase<int, NetInt>) this.resilience);
      this.Slipperiness = 8;
      Utility.addSprinklesToLocation(this.currentLocation, this.getTileX(), this.getTileY(), 2, 2, 101, 50, Color.LightBlue);
      if (Game1.random.NextDouble() < (double) (NetFieldBase<double, NetDouble>) this.missChance - (double) (NetFieldBase<double, NetDouble>) this.missChance * addedPrecision)
      {
        damage1 = -1;
      }
      else
      {
        if (who.CurrentTool != null && who.CurrentTool.Name.Equals("Holy Sword") && !isBomb)
        {
          this.Health -= damage * 3 / 4;
          this.currentLocation.debris.Add(new Debris((damage * 3 / 4).ToString() ?? "", 1, new Vector2((float) this.getStandingX(), (float) this.getStandingY()), Color.LightBlue, 1f, 0.0f));
        }
        this.Health -= damage1;
        if (this.Health <= 0)
          this.deathAnimation();
        this.setTrajectory(xTrajectory, yTrajectory);
      }
      this.addedSpeed = -1;
      Utility.removeLightSource(this.identifier);
      return damage1;
    }

    protected override void localDeathAnimation()
    {
      this.currentLocation.localSound("ghost");
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite((string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Microsoft.Xna.Framework.Rectangle(0, 96, 16, 24), 100f, 4, 0, this.Position, false, false, 0.9f, 1f / 1000f, Color.White, 4f, 0.01f, 0.0f, (float) Math.PI / 64f));
    }

    protected override void sharedDeathAnimation()
    {
    }

    protected override void updateAnimation(GameTime time)
    {
      this.nextParticle -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.nextParticle <= 0.0)
      {
        this.nextParticle = 1f;
        if (this.variant.Value == Ghost.GhostVariant.Putrid)
        {
          if (this.currentLocationRef.Value != null)
          {
            Vector2 standingPosition = this.getStandingPosition();
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite((string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Microsoft.Xna.Framework.Rectangle(Game1.random.Next(4) * 16, 168, 16, 24), 100f, 1, 10, this.Position + new Vector2(Utility.RandomFloat(-16f, 16f), Utility.RandomFloat(-16f, 0.0f) - (float) this.yOffset), false, false, standingPosition.Y / 10000f, 0.01f, Color.White, 4f, -0.01f, 0.0f, 0.0f)
            {
              acceleration = new Vector2(0.0f, 0.025f)
            });
          }
          this.nextParticle = Utility.RandomFloat(0.3f, 0.5f);
        }
      }
      this.yOffset = (int) (Math.Sin((double) time.TotalGameTime.Milliseconds / 1000.0 * (2.0 * Math.PI)) * 20.0) - this.yOffsetExtra;
      if (this.currentLocation == Game1.currentLocation)
      {
        bool flag = false;
        foreach (LightSource currentLightSource in Game1.currentLightSources)
        {
          if ((int) (NetFieldBase<int, NetInt>) currentLightSource.identifier == this.identifier)
          {
            currentLightSource.position.Value = new Vector2(this.Position.X + 32f, this.Position.Y + 64f + (float) this.yOffset);
            flag = true;
          }
        }
        if (!flag)
        {
          if ((string) (NetFieldBase<string, NetString>) this.name == "Carbon Ghost")
            Game1.currentLightSources.Add(new LightSource(4, new Vector2(this.Position.X + 8f, this.Position.Y + 64f), 1f, new Color(80, 30, 0), this.identifier));
          else
            Game1.currentLightSources.Add(new LightSource(5, new Vector2(this.Position.X + 8f, this.Position.Y + 64f), 1f, Color.White * 0.7f, this.identifier));
        }
      }
      if (this.variant.Value == Ghost.GhostVariant.Putrid && this.UpdateVariantAnimation(time))
        return;
      int x1 = this.Player.GetBoundingBox().Center.X;
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      int x2 = boundingBox.Center.X;
      float num1 = (float) -(x1 - x2);
      boundingBox = this.Player.GetBoundingBox();
      int y1 = boundingBox.Center.Y;
      boundingBox = this.GetBoundingBox();
      int y2 = boundingBox.Center.Y;
      float num2 = (float) (y1 - y2);
      float num3 = 400f;
      float x3 = num1 / num3;
      float num4 = num2 / num3;
      if (this.wasHitCounter <= 0)
      {
        this.targetRotation = (float) Math.Atan2(-(double) num4, (double) x3) - 1.570796f;
        if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) > 7.0 * Math.PI / 8.0 && Game1.random.NextDouble() < 0.5)
          this.turningRight = true;
        else if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) < Math.PI / 8.0)
          this.turningRight = false;
        if (this.turningRight)
          this.rotation -= (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
        else
          this.rotation += (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
        this.rotation %= 6.283185f;
        this.wasHitCounter = 0;
      }
      float num5 = Math.Min(4f, Math.Max(1f, (float) (5.0 - (double) num3 / 64.0 / 2.0)));
      float num6 = (float) Math.Cos((double) this.rotation + Math.PI / 2.0);
      float num7 = -(float) Math.Sin((double) this.rotation + Math.PI / 2.0);
      this.xVelocity += (float) (-(double) num6 * (double) num5 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
      this.yVelocity += (float) (-(double) num7 * (double) num5 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
      if ((double) Math.Abs(this.xVelocity) > (double) Math.Abs((float) (-(double) num6 * 5.0)))
        this.xVelocity -= (float) (-(double) num6 * (double) num5 / 6.0);
      if ((double) Math.Abs(this.yVelocity) > (double) Math.Abs((float) (-(double) num7 * 5.0)))
        this.yVelocity -= (float) (-(double) num7 * (double) num5 / 6.0);
      this.faceGeneralDirection(this.Player.getStandingPosition(), 0, false, false);
      this.resetAnimationSpeed();
    }

    public virtual bool UpdateVariantAnimation(GameTime time)
    {
      if (this.variant.Value != Ghost.GhostVariant.Putrid)
        return false;
      if (this.currentState.Value == 0)
      {
        if (this.Sprite.CurrentFrame >= 20)
          this.Sprite.CurrentFrame = 0;
        return false;
      }
      if (this.currentState.Value >= 1 && this.currentState.Value <= 3)
      {
        this.shakeTimer = 250;
        if (this.Player != null)
          this.faceGeneralDirection(this.Player.getStandingPosition(), 0, false, false);
        if (this.FacingDirection == 2)
          this.Sprite.CurrentFrame = 20;
        else if (this.FacingDirection == 1)
          this.Sprite.CurrentFrame = 21;
        else if (this.FacingDirection == 0)
          this.Sprite.CurrentFrame = 22;
        else if (this.FacingDirection == 3)
          this.Sprite.CurrentFrame = 23;
      }
      else if (this.currentState.Value >= 4)
      {
        this.shakeTimer = 250;
        if (this.FacingDirection == 2)
          this.Sprite.CurrentFrame = 24;
        else if (this.FacingDirection == 1)
          this.Sprite.CurrentFrame = 25;
        else if (this.FacingDirection == 0)
          this.Sprite.CurrentFrame = 26;
        else if (this.FacingDirection == 3)
          this.Sprite.CurrentFrame = 27;
      }
      return true;
    }

    public override void noMovementProgressNearPlayerBehavior()
    {
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      if ((double) this.stateTimer > 0.0)
      {
        this.stateTimer -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.stateTimer <= 0.0)
          this.stateTimer = 0.0f;
      }
      if (this.variant.Value == Ghost.GhostVariant.Putrid)
      {
        Farmer player = this.Player;
        if (this.currentState.Value == 0)
        {
          if ((double) this.stateTimer == -1.0)
            this.stateTimer = Utility.RandomFloat(1f, 2f);
          if (player != null && (double) this.stateTimer == 0.0 && (double) Math.Abs(player.Position.X - this.Position.X) < 448.0 && (double) Math.Abs(player.Position.Y - this.Position.Y) < 448.0)
          {
            this.currentState.Value = 1;
            this.currentLocation.playSound("croak");
            this.stateTimer = 0.5f;
          }
        }
        else if (this.currentState.Value == 1)
        {
          this.xVelocity = 0.0f;
          this.yVelocity = 0.0f;
          if ((double) this.stateTimer <= 0.0)
            this.currentState.Value = 2;
        }
        else if (this.currentState.Value == 2)
        {
          if (player == null)
            this.currentState.Value = 0;
          else if ((double) Math.Abs(player.Position.X - this.Position.X) < 80.0 && (double) Math.Abs(player.Position.Y - this.Position.Y) < 80.0)
          {
            this.currentState.Value = 3;
            this.stateTimer = 0.05f;
            this.xVelocity = 0.0f;
            this.yVelocity = 0.0f;
          }
          else
          {
            Vector2 vector2 = player.getStandingPosition() - this.getStandingPosition();
            if ((double) vector2.LengthSquared() == 0.0)
            {
              this.currentState.Value = 3;
              this.stateTimer = 0.15f;
            }
            else
            {
              vector2.Normalize();
              vector2 *= 10f;
              this.xVelocity = vector2.X;
              this.yVelocity = -vector2.Y;
            }
          }
        }
        else if (this.currentState.Value == 3)
        {
          this.xVelocity = 0.0f;
          this.yVelocity = 0.0f;
          if ((double) this.stateTimer <= 0.0)
          {
            this.currentState.Value = 4;
            this.stateTimer = 1f;
            Vector2 vector2_1 = Vector2.Zero;
            if ((int) this.facingDirection == 0)
              vector2_1 = new Vector2(0.0f, -1f);
            if ((int) this.facingDirection == 3)
              vector2_1 = new Vector2(-1f, 0.0f);
            if ((int) this.facingDirection == 1)
              vector2_1 = new Vector2(1f, 0.0f);
            if ((int) this.facingDirection == 2)
              vector2_1 = new Vector2(0.0f, 1f);
            Vector2 vector2_2 = vector2_1 * 6f;
            this.currentLocation.playSound("fishSlap");
            BasicProjectile basicProjectile = new BasicProjectile(this.DamageToFarmer, 7, 0, 1, (float) Math.PI / 32f, vector2_2.X, vector2_2.Y, this.Position, "", "", false, location: this.currentLocation, firer: ((Character) this));
            basicProjectile.debuff.Value = 25;
            basicProjectile.scaleGrow.Value = 0.05f;
            basicProjectile.ignoreTravelGracePeriod.Value = true;
            basicProjectile.IgnoreLocationCollision = true;
            basicProjectile.maxTravelDistance.Value = 192;
            this.currentLocation.projectiles.Add((Projectile) basicProjectile);
          }
        }
        else if (this.currentState.Value == 4 && (double) this.stateTimer <= 0.0)
        {
          this.xVelocity = 0.0f;
          this.yVelocity = 0.0f;
          this.currentState.Value = 0;
          this.stateTimer = Utility.RandomFloat(3f, 4f);
        }
      }
      base.behaviorAtGameTick(time);
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      if (!boundingBox.Intersects(this.Player.GetBoundingBox()) || !this.Player.temporarilyInvincible || this.currentState.Value != 0)
        return;
      int num = 0;
      Vector2 vector2_3;
      ref Vector2 local1 = ref vector2_3;
      boundingBox = this.Player.GetBoundingBox();
      double x1 = (double) (boundingBox.Center.X / 64 + Game1.random.Next(-12, 12));
      boundingBox = this.Player.GetBoundingBox();
      double y1 = (double) (boundingBox.Center.Y / 64 + Game1.random.Next(-12, 12));
      local1 = new Vector2((float) x1, (float) y1);
      for (; num < 3 && ((double) vector2_3.X >= (double) this.currentLocation.map.GetLayer("Back").LayerWidth || (double) vector2_3.Y >= (double) this.currentLocation.map.GetLayer("Back").LayerHeight || (double) vector2_3.X < 0.0 || (double) vector2_3.Y < 0.0 || this.currentLocation.map.GetLayer("Back").Tiles[(int) vector2_3.X, (int) vector2_3.Y] == null || !this.currentLocation.isTilePassable(new Location((int) vector2_3.X, (int) vector2_3.Y), Game1.viewport) || vector2_3.Equals(new Vector2((float) (this.Player.getStandingX() / 64), (float) (this.Player.getStandingY() / 64)))); ++num)
      {
        ref Vector2 local2 = ref vector2_3;
        boundingBox = this.Player.GetBoundingBox();
        double x2 = (double) (boundingBox.Center.X / 64 + Game1.random.Next(-12, 12));
        boundingBox = this.Player.GetBoundingBox();
        double y2 = (double) (boundingBox.Center.Y / 64 + Game1.random.Next(-12, 12));
        local2 = new Vector2((float) x2, (float) y2);
      }
      if (num >= 3)
        return;
      this.Position = new Vector2(vector2_3.X * 64f, (float) ((double) vector2_3.Y * 64.0 - 32.0));
      this.Halt();
    }

    public enum GhostVariant
    {
      Normal,
      Putrid,
    }
  }
}
