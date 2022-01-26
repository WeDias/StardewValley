// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Squirrel
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class Squirrel : Critter
  {
    private int nextNibbleTimer = 1000;
    private int treeRunTimer;
    private int characterCheckTimer = 200;
    private bool running;
    private Tree climbed;
    private Vector2 treeTile;

    public Squirrel(Vector2 position, bool flip)
    {
      this.position = position * 64f;
      this.flip = flip;
      this.baseFrame = 60;
      this.sprite = new AnimatedSprite(Critter.critterTexture, this.baseFrame, 32, 32);
      this.sprite.loop = false;
      this.startingPosition = position;
    }

    private void doneNibbling(Farmer who) => this.nextNibbleTimer = Game1.random.Next(2000);

    public override void draw(SpriteBatch b)
    {
      this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2((float) ((this.treeRunTimer > 0 ? (this.flip ? 224 : -16) : 0) - 64), (float) ((double) this.yJumpOffset - 64.0 + (double) this.yOffset + (this.treeRunTimer > 0 ? (this.flip ? 0.0 : 128.0) : 0.0)))), (float) (((double) this.position.Y + 64.0 + (this.treeRunTimer > 0 ? 128.0 : 0.0)) / 10000.0 + (double) this.position.X / 1000000.0), 0, 0, Color.White, this.flip, 4f, this.treeRunTimer > 0 ? (float) ((this.flip ? 1.0 : -1.0) * Math.PI / 2.0) : 0.0f);
      if (this.treeRunTimer > 0)
        return;
      b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(0.0f, 60f)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 3f + Math.Max(-3f, (float) (((double) this.yJumpOffset + (double) this.yOffset) / 16.0)), SpriteEffects.None, (float) (((double) this.position.Y - 1.0) / 10000.0));
    }

    public override bool update(GameTime time, GameLocation environment)
    {
      this.nextNibbleTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.sprite.CurrentAnimation == null && this.nextNibbleTimer <= 0)
      {
        int num = Game1.random.Next(2, 8);
        List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>();
        for (int index = 0; index < num; ++index)
        {
          animation.Add(new FarmerSprite.AnimationFrame(this.baseFrame, 200));
          animation.Add(new FarmerSprite.AnimationFrame(this.baseFrame + 1, 200));
        }
        animation.Add(new FarmerSprite.AnimationFrame(this.baseFrame, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doneNibbling)));
        this.sprite.setCurrentAnimation(animation);
      }
      this.characterCheckTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.characterCheckTimer <= 0 && !this.running)
      {
        if (Utility.isThereAFarmerOrCharacterWithinDistance(this.position / 64f, 12, environment) != null)
        {
          this.running = true;
          this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(this.baseFrame + 2, 50),
            new FarmerSprite.AnimationFrame(this.baseFrame + 3, 50),
            new FarmerSprite.AnimationFrame(this.baseFrame + 4, 50),
            new FarmerSprite.AnimationFrame(this.baseFrame + 5, 120),
            new FarmerSprite.AnimationFrame(this.baseFrame + 6, 80),
            new FarmerSprite.AnimationFrame(this.baseFrame + 7, 50)
          });
          this.sprite.loop = true;
        }
        this.characterCheckTimer = 200;
      }
      if (this.running)
      {
        if (this.treeRunTimer > 0)
          this.position.Y -= 4f;
        else
          this.position.X += this.flip ? -4f : 4f;
      }
      if (this.running && this.characterCheckTimer <= 0 && this.treeRunTimer <= 0)
      {
        this.characterCheckTimer = 100;
        Vector2 key = new Vector2((float) (int) ((double) this.position.X / 64.0), (float) ((int) this.position.Y / 64));
        if (environment.terrainFeatures.ContainsKey(key) && environment.terrainFeatures[key] is Tree)
        {
          this.treeRunTimer = 700;
          this.climbed = environment.terrainFeatures[key] as Tree;
          this.treeTile = key;
          this.position = key * 64f;
          return false;
        }
        key = new Vector2((float) (int) (((double) this.position.X + 64.0 + 1.0) / 64.0), (float) ((int) this.position.Y / 64));
      }
      if (this.treeRunTimer > 0)
      {
        this.treeRunTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.treeRunTimer <= 0)
        {
          this.climbed.performUseAction(this.treeTile, environment);
          return true;
        }
      }
      return base.update(time, environment);
    }
  }
}
