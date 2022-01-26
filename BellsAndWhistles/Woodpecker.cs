// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Woodpecker
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class Woodpecker : Critter
  {
    public const int flyingSpeed = 6;
    private bool flyingAway;
    private Tree tree;
    private int peckTimer;
    private int characterCheckTimer = 200;

    public Woodpecker(Tree tree, Vector2 position)
    {
      this.tree = tree;
      position *= 64f;
      this.position = position;
      this.position.X += 32f;
      this.position.Y += 0.0f;
      this.startingPosition = position;
      this.baseFrame = 320;
      this.sprite = new AnimatedSprite(Critter.critterTexture, 320, 16, 16);
    }

    public override void drawAboveFrontLayer(SpriteBatch b) => this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-80f, this.yJumpOffset - 64f + this.yOffset)), 1f, 0, 0, Color.White, this.flip, 4f);

    public override void draw(SpriteBatch b)
    {
      SpriteBatch spriteBatch = b;
      Texture2D shadowTexture = Game1.shadowTexture;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(0.0f, -4f));
      Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
      Color white = Color.White;
      Rectangle bounds = Game1.shadowTexture.Bounds;
      double x = (double) bounds.Center.X;
      bounds = Game1.shadowTexture.Bounds;
      double y = (double) bounds.Center.Y;
      Vector2 origin = new Vector2((float) x, (float) y);
      double scale = 3.0 + (double) Math.Max(-3f, (float) (((double) this.yJumpOffset + (double) this.yOffset) / 16.0));
      double layerDepth = ((double) this.position.Y - 1.0) / 10000.0;
      spriteBatch.Draw(shadowTexture, local, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
    }

    private void donePecking(Farmer who) => this.peckTimer = Game1.random.Next(1000, 3000);

    private void playFlap(Farmer who)
    {
      if (!Utility.isOnScreen(this.position, 64))
        return;
      Game1.playSound("batFlap");
    }

    private void playPeck(Farmer who)
    {
      if (!Utility.isOnScreen(this.position, 64))
        return;
      Game1.playSound("Cowboy_gunshot");
    }

    public override bool update(GameTime time, GameLocation environment)
    {
      if (environment == null || this.sprite == null || this.tree == null)
        return true;
      if ((double) this.yJumpOffset < 0.0 && !this.flyingAway)
      {
        if (!this.flip && !environment.isCollidingPosition(this.getBoundingBox(-2, 0), Game1.viewport, false, 0, false, (Character) null, false, ignoreCharacterRequirement: true))
          this.position.X -= 2f;
        else if (!environment.isCollidingPosition(this.getBoundingBox(2, 0), Game1.viewport, false, 0, false, (Character) null, false, ignoreCharacterRequirement: true))
          this.position.X += 2f;
      }
      this.peckTimer -= time.ElapsedGameTime.Milliseconds;
      if (!this.flyingAway && this.peckTimer <= 0 && this.sprite.CurrentAnimation == null)
      {
        int num = Game1.random.Next(2, 8);
        List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>();
        for (int index = 0; index < num; ++index)
        {
          animation.Add(new FarmerSprite.AnimationFrame(this.baseFrame, 100));
          animation.Add(new FarmerSprite.AnimationFrame(this.baseFrame + 1, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(this.playPeck)));
        }
        animation.Add(new FarmerSprite.AnimationFrame(this.baseFrame, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(this.donePecking)));
        this.sprite.setCurrentAnimation(animation);
        this.sprite.loop = false;
      }
      this.characterCheckTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.characterCheckTimer < 0)
      {
        Character character = Utility.isThereAFarmerOrCharacterWithinDistance(this.position / 64f, 6, environment);
        this.characterCheckTimer = 200;
        if ((character != null || (bool) (NetFieldBase<bool, NetBool>) this.tree.stump) && !this.flyingAway)
        {
          this.flyingAway = true;
          if (character != null && (double) character.Position.X > (double) this.position.X)
            this.flip = false;
          else
            this.flip = true;
          this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 2), 70),
            new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 3), 60, false, this.flip, new AnimatedSprite.endOfAnimationBehavior(this.playFlap)),
            new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 4), 70),
            new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 3), 60)
          });
          this.sprite.loop = true;
        }
      }
      if (this.flyingAway)
      {
        if (!this.flip)
          this.position.X -= 6f;
        else
          this.position.X += 6f;
        --this.yOffset;
      }
      return base.update(time, environment);
    }
  }
}
