// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Duggy
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;

namespace StardewValley.Monsters
{
  public class Duggy : Monster
  {
    private double chanceToDisappear = 0.03;

    public Duggy() => this.HideShadow = true;

    public Duggy(Vector2 position)
      : base(nameof (Duggy), position)
    {
      this.IsWalkingTowardPlayer = false;
      this.IsInvisible = true;
      this.DamageToFarmer = 0;
      this.Sprite.currentFrame = 0;
      this.HideShadow = true;
    }

    public Duggy(Vector2 position, bool magmaDuggy)
      : base("Magma Duggy", position)
    {
      this.IsWalkingTowardPlayer = false;
      this.IsInvisible = true;
      this.DamageToFarmer = 0;
      this.Sprite.currentFrame = 0;
      this.HideShadow = true;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.position.Field.Interpolated(false, true);
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
        this.Health -= damage1;
        this.currentLocation.playSound("hitEnemy");
        if (this.Health <= 0)
          this.deathAnimation();
      }
      return damage1;
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

    public override void update(GameTime time, GameLocation location)
    {
      if (this.invincibleCountdown > 0)
      {
        this.glowingColor = Color.Cyan;
        this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
        if (this.invincibleCountdown <= 0)
          this.stopGlowing();
      }
      if (!location.farmers.Any())
        return;
      this.behaviorAtGameTick(time);
      if ((double) this.Position.X < 0.0 || (double) this.Position.X > (double) (location.map.GetLayer("Back").LayerWidth * 64) || (double) this.Position.Y < 0.0 || (double) this.Position.Y > (double) (location.map.GetLayer("Back").LayerHeight * 64))
        location.characters.Remove((NPC) this);
      this.updateGlow();
    }

    public override void draw(SpriteBatch b)
    {
      if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128))
        return;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (this.GetBoundingBox().Height / 2 + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), Color.White, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));
      if (!this.isGlowing)
        return;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (this.GetBoundingBox().Height / 2 + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) ((double) this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      base.behaviorAtGameTick(time);
      this.isEmoting = false;
      this.Sprite.loop = false;
      Rectangle boundingBox = this.GetBoundingBox();
      if (this.Sprite.currentFrame < 4)
      {
        boundingBox.Inflate(128, 128);
        if (!this.IsInvisible || boundingBox.Contains(this.Player.getStandingX(), this.Player.getStandingY()))
        {
          if (this.IsInvisible)
          {
            if (this.currentLocation.map.GetLayer("Back").Tiles[(int) this.Player.getTileLocation().X, (int) this.Player.getTileLocation().Y].Properties.ContainsKey("NPCBarrier") || !this.currentLocation.map.GetLayer("Back").Tiles[(int) this.Player.getTileLocation().X, (int) this.Player.getTileLocation().Y].TileIndexProperties.ContainsKey("Diggable") && this.currentLocation.map.GetLayer("Back").Tiles[(int) this.Player.getTileLocation().X, (int) this.Player.getTileLocation().Y].TileIndex != 0)
              return;
            this.Position = new Vector2(this.Player.Position.X, this.Player.Position.Y + (float) this.Player.Sprite.SpriteHeight - (float) this.Sprite.SpriteHeight);
            this.currentLocation.localSound(nameof (Duggy));
            this.Position = this.Player.getTileLocation() * 64f;
          }
          this.IsInvisible = false;
          this.Sprite.interval = 100f;
          this.Sprite.AnimateDown(time);
        }
      }
      if (this.Sprite.currentFrame >= 4 && this.Sprite.currentFrame < 8)
      {
        boundingBox.Inflate((int) sbyte.MinValue, (int) sbyte.MinValue);
        this.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 8, false, (Character) this);
        this.Sprite.AnimateRight(time);
        this.Sprite.interval = 220f;
        this.DamageToFarmer = 8;
      }
      if (this.Sprite.currentFrame >= 8)
        this.Sprite.AnimateUp(time);
      if (this.Sprite.currentFrame < 10)
        return;
      this.IsInvisible = true;
      this.Sprite.currentFrame = 0;
      Vector2 tileLocation = this.getTileLocation();
      this.currentLocation.map.GetLayer("Back").Tiles[(int) tileLocation.X, (int) tileLocation.Y].TileIndex = 0;
      this.currentLocation.removeEverythingExceptCharactersFromThisTile((int) tileLocation.X, (int) tileLocation.Y);
      this.DamageToFarmer = 0;
    }
  }
}
