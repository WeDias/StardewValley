// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.DwarvishSentry
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;

namespace StardewValley.Monsters
{
  public class DwarvishSentry : Monster
  {
    private float lastFireball;
    private int yOffset;
    private float pauseTimer;

    public DwarvishSentry()
    {
    }

    public DwarvishSentry(Vector2 position)
      : base("Dwarvish Sentry", position)
    {
      this.Sprite.SpriteHeight = 16;
      this.IsWalkingTowardPlayer = false;
      this.Sprite.UpdateSourceRect();
      this.HideShadow = true;
      this.isGlider.Value = true;
      this.Slipperiness = 1;
      this.pauseTimer = 10000f;
      DelayedAction.playSoundAfterDelay(nameof (DwarvishSentry), 500);
    }

    protected override void initNetFields() => base.initNetFields();

    public override void reloadSprite() => this.Sprite = new AnimatedSprite("Characters\\Monsters\\Dwarvish Sentry");

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
        this.currentLocation?.playSound("clank");
        if (this.Health <= 0)
          this.deathAnimation();
      }
      return damage1;
    }

    protected override void localDeathAnimation()
    {
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite((string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(0, 64, 16, 16), 70f, 7, 0, this.Position + new Vector2(0.0f, -32f), false, false)
      {
        scale = 4f
      });
      this.currentLocation.localSound("fireball");
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(362, 30f, 6, 1, this.Position + new Vector2((float) (Game1.random.Next(64) - 16), (float) (Game1.random.Next(64) - 32)), false, Game1.random.NextDouble() < 0.5)
      {
        delayBeforeAnimationStart = 100
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(362, 30f, 6, 1, this.Position + new Vector2((float) (Game1.random.Next(64) - 16), (float) (Game1.random.Next(64) - 32)), false, Game1.random.NextDouble() < 0.5)
      {
        delayBeforeAnimationStart = 200
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(362, 30f, 6, 1, this.Position + new Vector2((float) (Game1.random.Next(64) - 16), (float) (Game1.random.Next(64) - 32)), false, Game1.random.NextDouble() < 0.5)
      {
        delayBeforeAnimationStart = 300
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(362, 30f, 6, 1, this.Position + new Vector2((float) (Game1.random.Next(64) - 16), (float) (Game1.random.Next(64) - 32)), false, Game1.random.NextDouble() < 0.5)
      {
        delayBeforeAnimationStart = 400
      });
    }

    public override void drawAboveAllLayers(SpriteBatch b)
    {
      b.Draw(Game1.mouseCursors, this.getLocalPosition(Game1.viewport) + new Vector2(50f, (float) (80 + this.yOffset)), new Rectangle?(new Rectangle(536 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 350.0 / 70.0) * 8, 1945, 8, 8)), Color.White * 0.75f, 0.0f, new Vector2(8f, 16f), 4f, SpriteEffects.FlipVertically, (float) (0.990000009536743 - (double) this.position.X / 10000.0));
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (21 + this.yOffset)), new Rectangle?(this.Sprite.SourceRect), Color.White, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (1.0 - (double) this.position.X / 10000.0));
      b.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 64f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), (float) (3.0 + (double) this.yOffset / 20.0), SpriteEffects.None, (float) (this.getStandingY() - 1) / 10000f);
    }

    protected override void updateAnimation(GameTime time)
    {
      base.updateAnimation(time);
      this.yOffset = (int) (Math.Sin((double) time.TotalGameTime.Milliseconds / 2000.0 * (2.0 * Math.PI)) * 7.0);
      if (this.Sprite.currentFrame % 4 != 0 && Game1.random.NextDouble() < 0.1)
        this.Sprite.currentFrame -= this.Sprite.currentFrame % 4;
      if (Game1.random.NextDouble() < 0.01)
        ++this.Sprite.currentFrame;
      this.resetAnimationSpeed();
    }

    public override void update(GameTime time, GameLocation location) => base.update(time, location);

    public override void behaviorAtGameTick(GameTime time)
    {
      base.behaviorAtGameTick(time);
      this.faceGeneralDirection(this.Player.Position);
      this.pauseTimer += (float) (int) time.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.pauseTimer < 10000.0)
      {
        this.setTrajectory(Vector2.Normalize(this.Player.Position - this.Position) * new Vector2(1f, -1f));
      }
      else
      {
        if (Game1.random.NextDouble() >= 0.01)
          return;
        this.pauseTimer = (float) Game1.random.Next(5000);
      }
    }
  }
}
