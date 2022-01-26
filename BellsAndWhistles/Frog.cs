// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Frog
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class Frog : Critter
  {
    private bool waterLeaper;
    private bool leapingIntoWater;
    private bool splash;
    private int characterCheckTimer = 200;
    private int beforeFadeTimer;
    private float alpha = 1f;

    public Frog(Vector2 position, bool waterLeaper = false, bool forceFlip = false)
    {
      this.waterLeaper = waterLeaper;
      this.position = position * 64f;
      this.sprite = new AnimatedSprite(Critter.critterTexture, waterLeaper ? 300 : 280, 16, 16);
      this.sprite.loop = true;
      if (!this.flip & forceFlip)
        this.flip = true;
      if (waterLeaper)
      {
        this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(300, 600),
          new FarmerSprite.AnimationFrame(304, 100),
          new FarmerSprite.AnimationFrame(305, 100),
          new FarmerSprite.AnimationFrame(306, 300),
          new FarmerSprite.AnimationFrame(305, 100),
          new FarmerSprite.AnimationFrame(304, 100)
        });
      }
      else
      {
        this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(280, 60),
          new FarmerSprite.AnimationFrame(281, 70),
          new FarmerSprite.AnimationFrame(282, 140),
          new FarmerSprite.AnimationFrame(283, 90)
        });
        this.beforeFadeTimer = 1000;
        this.flip = (double) this.position.X + 4.0 < (double) Game1.player.Position.X;
      }
      this.startingPosition = position;
    }

    public void startSplash(Farmer who) => this.splash = true;

    public override bool update(GameTime time, GameLocation environment)
    {
      if (this.waterLeaper)
      {
        if (!this.leapingIntoWater)
        {
          this.characterCheckTimer -= time.ElapsedGameTime.Milliseconds;
          if (this.characterCheckTimer <= 0)
          {
            if (Utility.isThereAFarmerOrCharacterWithinDistance(this.position / 64f, 6, environment) != null)
            {
              this.leapingIntoWater = true;
              this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
              {
                new FarmerSprite.AnimationFrame(300, 100),
                new FarmerSprite.AnimationFrame(301, 100),
                new FarmerSprite.AnimationFrame(302, 100),
                new FarmerSprite.AnimationFrame(303, 1500, false, false, new AnimatedSprite.endOfAnimationBehavior(this.startSplash), true)
              });
              this.sprite.loop = false;
              this.sprite.oldFrame = 303;
              this.gravityAffectedDY = -6f;
            }
            else if (Game1.random.NextDouble() < 0.01)
              Game1.playSound("croak");
            this.characterCheckTimer = 200;
          }
        }
        else
        {
          this.position.X += this.flip ? -4f : 4f;
          if ((double) this.gravityAffectedDY >= 0.0 && (double) this.yJumpOffset >= 0.0)
          {
            this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(300, 100),
              new FarmerSprite.AnimationFrame(301, 100),
              new FarmerSprite.AnimationFrame(302, 100),
              new FarmerSprite.AnimationFrame(303, 1500, false, false, new AnimatedSprite.endOfAnimationBehavior(this.startSplash), true)
            });
            this.sprite.loop = false;
            this.sprite.oldFrame = 303;
            this.gravityAffectedDY = -6f;
            this.yJumpOffset = 0.0f;
            if (environment.doesTileHaveProperty((int) this.position.X / 64, (int) this.position.Y / 64, "Water", "Back") != null)
              this.splash = true;
          }
        }
      }
      else
      {
        this.position.X += this.flip ? -3f : 3f;
        this.beforeFadeTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.beforeFadeTimer <= 0)
        {
          this.alpha -= 1f / 1000f * (float) time.ElapsedGameTime.Milliseconds;
          if ((double) this.alpha <= 0.0)
            return true;
        }
        if (environment.doesTileHaveProperty((int) this.position.X / 64, (int) this.position.Y / 64, "Water", "Back") != null)
          this.splash = true;
      }
      if (!this.splash)
        return base.update(time, environment);
      environment.TemporarySprites.Add(new TemporaryAnimatedSprite(28, 50f, 2, 1, this.position, false, false));
      Game1.playSound("dropItemInWater");
      return true;
    }

    public override void draw(SpriteBatch b)
    {
      this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, Utility.snapDrawPosition(this.position + new Vector2(0.0f, this.yJumpOffset - 20f + this.yOffset))), (float) (((double) this.position.Y + 64.0) / 10000.0), 0, 0, Color.White * this.alpha, this.flip, 4f);
      SpriteBatch spriteBatch = b;
      Texture2D shadowTexture = Game1.shadowTexture;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(32f, 40f));
      Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
      Color color = Color.White * this.alpha;
      Rectangle bounds = Game1.shadowTexture.Bounds;
      double x = (double) bounds.Center.X;
      bounds = Game1.shadowTexture.Bounds;
      double y = (double) bounds.Center.Y;
      Vector2 origin = new Vector2((float) x, (float) y);
      double scale = 3.0 + (double) Math.Max(-3f, (float) (((double) this.yJumpOffset + (double) this.yOffset) / 16.0));
      double layerDepth = ((double) this.position.Y - 1.0) / 10000.0;
      spriteBatch.Draw(shadowTexture, local, sourceRectangle, color, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
    }

    public override void drawAboveFrontLayer(SpriteBatch b)
    {
    }
  }
}
