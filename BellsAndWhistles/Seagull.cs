// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Seagull
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class Seagull : Critter
  {
    public const int walkingSpeed = 2;
    public const int flyingSpeed = 4;
    public const int walking = 0;
    public const int flyingAway = 1;
    public const int flyingToLand = 4;
    public const int swimming = 2;
    public const int stopped = 3;
    private int state;
    private int characterCheckTimer = 200;
    private bool moveLeft;

    public Seagull(Vector2 position, int startingState)
      : base(0, position)
    {
      this.moveLeft = Game1.random.NextDouble() < 0.5;
      this.startingPosition = position;
      this.state = startingState;
    }

    public void hop(Farmer who) => this.gravityAffectedDY = -4f;

    public override bool update(GameTime time, GameLocation environment)
    {
      this.characterCheckTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.characterCheckTimer < 0)
      {
        Character character = Utility.isThereAFarmerOrCharacterWithinDistance(this.position / 64f, 4, environment);
        this.characterCheckTimer = 200;
        if (character != null && this.state != 1)
        {
          if (Game1.random.NextDouble() < 0.25)
            Game1.playSound("seagulls");
          this.state = 1;
          this.moveLeft = (double) character.Position.X > (double) this.position.X;
          this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 10), 80),
            new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 11), 80),
            new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 12), 80),
            new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 13), 100)
          });
          this.sprite.loop = true;
        }
      }
      switch (this.state)
      {
        case 0:
          if (this.moveLeft && !environment.isCollidingPosition(this.getBoundingBox(-2, 0), Game1.viewport, false, 0, false, (Character) null, false, ignoreCharacterRequirement: true))
            this.position.X -= 2f;
          else if (!this.moveLeft && !environment.isCollidingPosition(this.getBoundingBox(2, 0), Game1.viewport, false, 0, false, (Character) null, false, ignoreCharacterRequirement: true))
            this.position.X += 2f;
          if (Game1.random.NextDouble() < 0.005)
          {
            this.state = 3;
            this.sprite.loop = false;
            this.sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
            this.sprite.currentFrame = 0;
            break;
          }
          break;
        case 1:
          if (this.moveLeft)
            this.position.X -= 4f;
          else
            this.position.X += 4f;
          this.yOffset -= 2f;
          break;
        case 2:
          this.sprite.currentFrame = this.baseFrame + 9;
          float yOffset = this.yOffset;
          if ((time.TotalGameTime.TotalMilliseconds + (double) ((int) this.position.X * 4)) % 2000.0 < 1000.0)
            this.yOffset = 2f;
          else
            this.yOffset = 0.0f;
          if ((double) this.yOffset > (double) yOffset)
          {
            environment.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 0, 64, 64), 150f, 8, 0, new Vector2(this.position.X - 32f, this.position.Y - 32f), false, Game1.random.NextDouble() < 0.5, 1f / 1000f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
            break;
          }
          break;
        case 3:
          if (Game1.random.NextDouble() < 0.003 && this.sprite.CurrentAnimation == null)
          {
            this.sprite.loop = false;
            switch (Game1.random.Next(4))
            {
              case 0:
                List<FarmerSprite.AnimationFrame> animation1 = new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 2), 100),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 3), 100),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 4), 200),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 5), 200)
                };
                int num1 = Game1.random.Next(5);
                for (int index = 0; index < num1; ++index)
                {
                  animation1.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 4), 200));
                  animation1.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 5), 200));
                }
                this.sprite.setCurrentAnimation(animation1);
                break;
              case 1:
                this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(6, (int) (short) Game1.random.Next(500, 4000))
                });
                break;
              case 2:
                List<FarmerSprite.AnimationFrame> animation2 = new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 6), 500),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 7), 100, false, false, new AnimatedSprite.endOfAnimationBehavior(this.hop)),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 8), 100)
                };
                int num2 = Game1.random.Next(3);
                for (int index = 0; index < num2; ++index)
                {
                  animation2.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 7), 100));
                  animation2.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 8), 100));
                }
                this.sprite.setCurrentAnimation(animation2);
                break;
              case 3:
                this.state = 0;
                this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame((int) (short) this.baseFrame, 200),
                  new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 1), 200)
                });
                this.sprite.loop = true;
                this.moveLeft = Game1.random.NextDouble() < 0.5;
                if (Game1.random.NextDouble() < 0.33)
                {
                  this.moveLeft = (double) this.position.X > (double) this.startingPosition.X;
                  break;
                }
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
      this.flip = !this.moveLeft;
      return base.update(time, environment);
    }
  }
}
