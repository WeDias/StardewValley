// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Crow
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class Crow : Critter
  {
    public const int flyingSpeed = 6;
    public const int pecking = 0;
    public const int flyingAway = 1;
    public const int sleeping = 2;
    public const int stopped = 3;
    private int state;

    public Crow(int tileX, int tileY)
      : base(14, new Vector2((float) (tileX * 64), (float) (tileY * 64)))
    {
      this.flip = Game1.random.NextDouble() < 0.5;
      this.position.X += 32f;
      this.position.Y += 32f;
      this.startingPosition = this.position;
      this.state = 0;
    }

    public void hop(Farmer who) => this.gravityAffectedDY = -4f;

    private void donePecking(Farmer who) => this.state = Game1.random.NextDouble() < 0.5 ? 0 : 3;

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
      Game1.playSound("shiny4");
    }

    public override bool update(GameTime time, GameLocation environment)
    {
      Farmer farmer = Utility.isThereAFarmerWithinDistance(this.position / 64f, 4, environment);
      if ((double) this.yJumpOffset < 0.0 && this.state != 1)
      {
        if (!this.flip && !environment.isCollidingPosition(this.getBoundingBox(-2, 0), Game1.viewport, false, 0, false, (Character) null, false, ignoreCharacterRequirement: true))
          this.position.X -= 2f;
        else if (!environment.isCollidingPosition(this.getBoundingBox(2, 0), Game1.viewport, false, 0, false, (Character) null, false, ignoreCharacterRequirement: true))
          this.position.X += 2f;
      }
      if (farmer != null && this.state != 1)
      {
        if (Game1.random.NextDouble() < 0.85)
          Game1.playSound("crow");
        this.state = 1;
        if ((double) farmer.Position.X > (double) this.position.X)
          this.flip = false;
        else
          this.flip = true;
        this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 6), 40),
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 7), 40),
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 8), 40),
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 9), 40),
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 10), 40, false, this.flip, new AnimatedSprite.endOfAnimationBehavior(this.playFlap)),
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 7), 40),
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 9), 40),
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 8), 40),
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 7), 40)
        });
        this.sprite.loop = true;
      }
      switch (this.state)
      {
        case 0:
          if (this.sprite.CurrentAnimation == null)
          {
            List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>();
            animation.Add(new FarmerSprite.AnimationFrame((int) (short) this.baseFrame, 480));
            animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 1), 170, false, this.flip));
            animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 2), 170, false, this.flip));
            int num = Game1.random.Next(1, 5);
            for (int index = 0; index < num; ++index)
            {
              animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 3), 70));
              animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 4), 100, false, this.flip, new AnimatedSprite.endOfAnimationBehavior(this.playPeck)));
            }
            animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 3), 100));
            animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 2), 70, false, this.flip));
            animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 1), 70, false, this.flip));
            animation.Add(new FarmerSprite.AnimationFrame((int) (short) this.baseFrame, 500, false, this.flip, new AnimatedSprite.endOfAnimationBehavior(this.donePecking)));
            this.sprite.loop = false;
            this.sprite.setCurrentAnimation(animation);
            break;
          }
          break;
        case 1:
          if (!this.flip)
            this.position.X -= 6f;
          else
            this.position.X += 6f;
          this.yOffset -= 2f;
          break;
        case 2:
          if (this.sprite.CurrentAnimation == null)
            this.sprite.currentFrame = this.baseFrame + 5;
          if (Game1.random.NextDouble() < 0.003 && this.sprite.CurrentAnimation == null)
          {
            this.state = 3;
            break;
          }
          break;
        case 3:
          if (Game1.random.NextDouble() < 0.008 && this.sprite.CurrentAnimation == null && (double) this.yJumpOffset >= 0.0)
          {
            switch (Game1.random.Next(5))
            {
              case 0:
                this.state = 2;
                break;
              case 1:
                this.state = 0;
                break;
              case 2:
                this.hop((Farmer) null);
                break;
              case 3:
                this.flip = !this.flip;
                this.hop((Farmer) null);
                break;
              case 4:
                this.state = 1;
                this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 6), 50),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 7), 50),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 8), 50),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 9), 50),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 10), 50, false, this.flip, new AnimatedSprite.endOfAnimationBehavior(this.playFlap)),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 7), 50),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 9), 50),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 8), 50),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 7), 50)
                });
                this.sprite.loop = true;
                break;
            }
          }
          else
          {
            if (this.sprite.CurrentAnimation == null)
            {
              this.sprite.currentFrame = this.baseFrame;
              break;
            }
            break;
          }
          break;
      }
      return base.update(time, environment);
    }
  }
}
