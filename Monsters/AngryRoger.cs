// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.AngryRoger
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using xTile.Dimensions;

namespace StardewValley.Monsters
{
  public class AngryRoger : Monster
  {
    public const float rotationIncrement = 0.04908739f;
    private int wasHitCounter;
    private float targetRotation;
    private bool turningRight;
    private bool seenPlayer;
    private int identifier = Game1.random.Next(-99999, 99999);
    private int yOffset;
    private int yOffsetExtra;

    public AngryRoger()
    {
    }

    public AngryRoger(Vector2 position)
      : base("Ghost", position)
    {
      this.Slipperiness = 8;
      this.isGlider.Value = true;
      this.HideShadow = true;
    }

    /// <summary>constructor for non-default ghosts</summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    public AngryRoger(Vector2 position, string name)
      : base(name, position)
    {
      this.Slipperiness = 8;
      this.isGlider.Value = true;
      this.HideShadow = true;
    }

    public override void reloadSprite() => this.Sprite = new AnimatedSprite("Characters\\Monsters\\" + (string) (NetFieldBase<string, NetString>) this.name);

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
          Game1.currentLightSources.Add(new LightSource(5, new Vector2(this.Position.X + 8f, this.Position.Y + 64f), 1f, Color.White * 0.7f, this.identifier));
      }
      Microsoft.Xna.Framework.Rectangle boundingBox = this.Player.GetBoundingBox();
      int x1 = boundingBox.Center.X;
      boundingBox = this.GetBoundingBox();
      int x2 = boundingBox.Center.X;
      float num1 = (float) -(x1 - x2);
      float num2 = (float) (this.Player.GetBoundingBox().Center.Y - this.GetBoundingBox().Center.Y);
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
      this.faceGeneralDirection(this.Player.getStandingPosition());
      this.resetAnimationSpeed();
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      base.behaviorAtGameTick(time);
      if (!this.GetBoundingBox().Intersects(this.Player.GetBoundingBox()) || !this.Player.temporarilyInvincible)
        return;
      int num = 0;
      Vector2 vector2;
      ref Vector2 local1 = ref vector2;
      double x1 = (double) (this.Player.GetBoundingBox().Center.X / 64 + Game1.random.Next(-12, 12));
      Microsoft.Xna.Framework.Rectangle boundingBox = this.Player.GetBoundingBox();
      double y1 = (double) (boundingBox.Center.Y / 64 + Game1.random.Next(-12, 12));
      local1 = new Vector2((float) x1, (float) y1);
      for (; num < 3 && ((double) vector2.X >= (double) this.currentLocation.map.GetLayer("Back").LayerWidth || (double) vector2.Y >= (double) this.currentLocation.map.GetLayer("Back").LayerHeight || (double) vector2.X < 0.0 || (double) vector2.Y < 0.0 || this.currentLocation.map.GetLayer("Back").Tiles[(int) vector2.X, (int) vector2.Y] == null || !this.currentLocation.isTilePassable(new Location((int) vector2.X, (int) vector2.Y), Game1.viewport) || vector2.Equals(new Vector2((float) (this.Player.getStandingX() / 64), (float) (this.Player.getStandingY() / 64)))); ++num)
      {
        ref Vector2 local2 = ref vector2;
        boundingBox = this.Player.GetBoundingBox();
        double x2 = (double) (boundingBox.Center.X / 64 + Game1.random.Next(-12, 12));
        boundingBox = this.Player.GetBoundingBox();
        double y2 = (double) (boundingBox.Center.Y / 64 + Game1.random.Next(-12, 12));
        local2 = new Vector2((float) x2, (float) y2);
      }
      if (num >= 3)
        return;
      this.Position = new Vector2(vector2.X * 64f, (float) ((double) vector2.Y * 64.0 - 32.0));
      this.Halt();
    }
  }
}
