// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Bird
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Network;
using System;

namespace StardewValley.BellsAndWhistles
{
  public class Bird
  {
    public Vector2 position;
    public Point startPosition;
    public Point endPosition;
    public float pathPosition;
    public float velocity;
    public int framesUntilNextMove;
    public Bird.BirdState birdState;
    public PerchingBirds context;
    public int peckFrames;
    public int nextPeck;
    public int peckDirection;
    public int birdType;
    public int flapFrames = 2;
    public float flyArcHeight;

    public Bird()
    {
      this.position = new Vector2(0.0f, 0.0f);
      this.startPosition = new Point(0, 0);
      this.endPosition = new Point(0, 0);
      this.birdType = Game1.random.Next(0, 4);
    }

    public Bird(Point point, PerchingBirds context, int bird_type = 0, int flap_frames = 2)
    {
      this.startPosition.X = this.endPosition.X = point.X;
      this.startPosition.Y = this.endPosition.Y = point.Y;
      this.position.X = (float) (((double) this.startPosition.X + 0.5) * 64.0);
      this.position.Y = (float) (((double) this.startPosition.Y + 0.5) * 64.0);
      this.context = context;
      this.birdType = bird_type;
      this.framesUntilNextMove = Game1.random.Next(100, 300);
      this.peckDirection = Game1.random.Next(0, 2);
      this.flapFrames = flap_frames;
    }

    public void Draw(SpriteBatch b)
    {
      Vector2 vector2_1 = new Vector2(this.position.X, this.position.Y);
      vector2_1.X += (float) (Math.Sin((double) Game1.currentGameTime.TotalGameTime.Milliseconds * (1.0 / 400.0)) * (double) this.velocity * 2.0);
      vector2_1.Y += (float) (Math.Sin((double) Game1.currentGameTime.TotalGameTime.Milliseconds * (3.0 / 500.0)) * (double) this.velocity * 2.0);
      vector2_1.Y += (float) Math.Sin((double) this.pathPosition * Math.PI) * -this.flyArcHeight;
      SpriteEffects effects = SpriteEffects.None;
      int num;
      if (this.birdState == Bird.BirdState.Idle)
      {
        if (this.peckDirection == 1)
          effects = SpriteEffects.FlipHorizontally;
        num = this.context.ShouldBirdsRoost() ? (this.peckFrames <= 0 ? 8 : 9) : (this.peckFrames <= 0 ? 0 : 1);
      }
      else
      {
        Vector2 vector2_2 = new Vector2((float) (this.endPosition.X - this.startPosition.X), (float) (this.endPosition.Y - this.startPosition.Y));
        vector2_2.Normalize();
        if ((double) Math.Abs(vector2_2.X) > (double) Math.Abs(vector2_2.Y))
        {
          num = 2;
          if ((double) vector2_2.X > 0.0)
            effects = SpriteEffects.FlipHorizontally;
        }
        else if ((double) vector2_2.Y > 0.0)
        {
          num = 2 + this.flapFrames;
          if ((double) vector2_2.X > 0.0)
            effects = SpriteEffects.FlipHorizontally;
        }
        else
        {
          num = 2 + this.flapFrames * 2;
          if ((double) vector2_2.X < 0.0)
            effects = SpriteEffects.FlipHorizontally;
        }
        if ((double) this.pathPosition > 0.949999988079071)
          num += Game1.currentGameTime.TotalGameTime.Milliseconds / 50 % this.flapFrames;
        else if ((double) this.pathPosition <= 0.75)
          num += Game1.currentGameTime.TotalGameTime.Milliseconds / 100 % this.flapFrames;
      }
      Rectangle rectangle = new Rectangle(this.context.GetBirdWidth() * num, this.context.GetBirdHeight() * this.birdType, this.context.GetBirdWidth(), this.context.GetBirdHeight());
      Rectangle local = Game1.GlobalToLocal(Game1.viewport, new Rectangle((int) vector2_1.X, (int) vector2_1.Y, this.context.GetBirdWidth() * 4, this.context.GetBirdHeight() * 4));
      b.Draw(this.context.GetTexture(), local, new Rectangle?(rectangle), Color.White, 0.0f, this.context.GetBirdOrigin(), effects, this.position.Y / 10000f);
    }

    public void FlyToNewPoint()
    {
      Point freeBirdPoint = this.context.GetFreeBirdPoint(this, 500);
      if (freeBirdPoint != new Point())
      {
        this.context.ReserveBirdPoint(this, freeBirdPoint);
        this.startPosition = this.endPosition;
        this.endPosition = freeBirdPoint;
        this.pathPosition = 0.0f;
        this.velocity = 0.0f;
        this.birdState = !this.context.ShouldBirdsRoost() ? Bird.BirdState.Flying : Bird.BirdState.Idle;
        float num = Utility.distance((float) this.startPosition.X, (float) this.endPosition.X, (float) this.startPosition.Y, (float) this.endPosition.Y);
        if ((double) num >= 7.0)
          this.flyArcHeight = 200f;
        else if ((double) num >= 5.0)
          this.flyArcHeight = 150f;
        else
          this.flyArcHeight = 20f;
      }
      else
        this.framesUntilNextMove = Game1.random.Next(800, 1200);
    }

    public void Update(GameTime time)
    {
      if (this.peckFrames > 0)
      {
        --this.peckFrames;
      }
      else
      {
        --this.nextPeck;
        if (this.nextPeck <= 0)
        {
          this.peckFrames = !this.context.ShouldBirdsRoost() ? this.context.peckDuration : 50;
          this.nextPeck = Game1.random.Next(10, 30);
          if (Game1.random.NextDouble() <= 0.75)
          {
            this.nextPeck += Game1.random.Next(50, 100);
            if (!this.context.ShouldBirdsRoost())
              this.peckDirection = Game1.random.Next(0, 2);
          }
        }
      }
      if (this.birdState == Bird.BirdState.Idle)
      {
        if (this.context.ShouldBirdsRoost())
          return;
        using (FarmerCollection.Enumerator enumerator = Game1.currentLocation.farmers.GetEnumerator())
        {
          if (!enumerator.MoveNext())
            return;
          Farmer current = enumerator.Current;
          double num = (double) Utility.distance(current.position.X, this.position.X, current.position.Y, this.position.Y);
          --this.framesUntilNextMove;
          if (num >= 200.0 && this.framesUntilNextMove > 0)
            return;
          this.FlyToNewPoint();
        }
      }
      else
      {
        if (this.birdState != Bird.BirdState.Flying)
          return;
        float num1 = Utility.distance((float) (this.endPosition.X * 64) + 32f, this.position.X, (float) (this.endPosition.Y * 64) + 32f, this.position.Y);
        float birdSpeed = this.context.birdSpeed;
        float num2 = 0.25f;
        this.velocity = (double) num1 <= (double) birdSpeed / (double) num2 ? Math.Max(Math.Min(num1 * num2, this.velocity), 1f) : Utility.MoveTowards(this.velocity, birdSpeed, 0.5f);
        float num3 = Utility.distance((float) this.endPosition.X + 32f, (float) this.startPosition.X + 32f, (float) this.endPosition.Y + 32f, (float) this.startPosition.Y + 32f) * 64f;
        if ((double) num3 <= 9.99999974737875E-05)
          num3 = 0.0001f;
        this.pathPosition += this.velocity / num3;
        this.position = new Vector2(Utility.Lerp((float) (this.startPosition.X * 64) + 32f, (float) (this.endPosition.X * 64) + 32f, this.pathPosition), Utility.Lerp((float) (this.startPosition.Y * 64) + 32f, (float) (this.endPosition.Y * 64) + 32f, this.pathPosition));
        if ((double) this.pathPosition < 1.0)
          return;
        this.position = new Vector2((float) (this.endPosition.X * 64) + 32f, (float) (this.endPosition.Y * 64) + 32f);
        this.birdState = Bird.BirdState.Idle;
        this.velocity = 0.0f;
        this.framesUntilNextMove = Game1.random.Next(350, 500);
        if (Game1.random.NextDouble() >= 0.75)
          return;
        this.framesUntilNextMove += Game1.random.Next(200, 300);
      }
    }

    public enum BirdState
    {
      Idle,
      Flying,
    }
  }
}
