// Decompiled with JetBrains decompiler
// Type: StardewValley.WeatherDebris
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley
{
  [InstanceStatics]
  public class WeatherDebris
  {
    public const int pinkPetals = 0;
    public const int greenLeaves = 1;
    public const int fallLeaves = 2;
    public const int snow = 3;
    public const int animationInterval = 100;
    public const float gravity = -0.5f;
    public Vector2 position;
    public Rectangle sourceRect;
    public int which;
    public int animationIndex;
    public int animationTimer = 100;
    public int animationDirection = 1;
    public int animationIntervalOffset;
    public float dx;
    public float dy;
    public static float globalWind = -0.25f;
    private bool blowing;

    public WeatherDebris(Vector2 position, int which, float rotationVelocity, float dx, float dy)
    {
      this.position = position;
      this.which = which;
      this.dx = dx;
      this.dy = dy;
      switch (which)
      {
        case 0:
          this.sourceRect = new Rectangle(352, 1184, 16, 16);
          this.animationIntervalOffset = (Game1.random.Next(25) - 12) * 2;
          break;
        case 1:
          this.sourceRect = new Rectangle(352, 1200, 16, 16);
          this.animationIntervalOffset = (Game1.random.Next(25) - 12) * 2;
          break;
        case 2:
          this.sourceRect = new Rectangle(352, 1216, 16, 16);
          this.animationIntervalOffset = (Game1.random.Next(25) - 12) * 2;
          break;
        case 3:
          this.sourceRect = new Rectangle(391 + 4 * Game1.random.Next(5), 1236, 4, 4);
          break;
      }
    }

    public void update() => this.update(false);

    public void update(bool slow)
    {
      this.position.X += this.dx + (slow ? 0.0f : WeatherDebris.globalWind);
      this.position.Y += this.dy - (slow ? 0.0f : -0.5f);
      if ((double) this.dy < 0.0 && !this.blowing)
        this.dy += 0.01f;
      if (!Game1.fadeToBlack && (double) Game1.fadeToBlackAlpha <= 0.0)
      {
        if ((double) this.position.X < -80.0)
        {
          this.position.X = (float) Game1.viewport.Width;
          this.position.Y = (float) Game1.random.Next(0, Game1.viewport.Height - 64);
        }
        if ((double) this.position.Y > (double) (Game1.viewport.Height + 16))
        {
          this.position.X = (float) Game1.random.Next(0, Game1.viewport.Width);
          this.position.Y = -64f;
          this.dy = (float) Game1.random.Next(-15, 10) / (slow ? (Game1.random.NextDouble() < 0.1 ? 5f : 200f) : 50f);
          this.dx = (float) Game1.random.Next(-10, 0) / (slow ? 200f : 50f);
        }
        else if ((double) this.position.Y < -64.0)
        {
          this.position.Y = (float) Game1.viewport.Height;
          this.position.X = (float) Game1.random.Next(0, Game1.viewport.Width);
        }
      }
      if (this.blowing)
      {
        this.dy -= 0.01f;
        if (Game1.random.NextDouble() < 0.006 || (double) this.dy < -2.0)
          this.blowing = false;
      }
      else if (!slow && Game1.random.NextDouble() < 0.001 && Game1.currentSeason != null && (Game1.currentSeason.Equals("spring") || Game1.currentSeason.Equals("summer")))
        this.blowing = true;
      switch (this.which)
      {
        case 0:
        case 1:
        case 2:
        case 3:
          this.animationTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
          if (this.animationTimer > 0)
            break;
          this.animationTimer = 100 + this.animationIntervalOffset;
          this.animationIndex += this.animationDirection;
          if (this.animationDirection == 0)
            this.animationDirection = this.animationIndex < 9 ? 1 : -1;
          if (this.animationIndex > 10)
          {
            if (Game1.random.NextDouble() < 0.82)
            {
              --this.animationIndex;
              this.animationDirection = 0;
              this.dx += 0.1f;
              this.dy -= 0.2f;
            }
            else
              this.animationIndex = 0;
          }
          else if (this.animationIndex == 4 && this.animationDirection == -1)
          {
            ++this.animationIndex;
            this.animationDirection = 0;
            this.dx -= 0.1f;
            this.dy -= 0.1f;
          }
          if (this.animationIndex == 7 && this.animationDirection == -1)
            this.dy -= 0.2f;
          if (this.which == 3)
            break;
          this.sourceRect.X = 352 + this.animationIndex * 16;
          break;
      }
    }

    public void draw(SpriteBatch b) => b.Draw(Game1.mouseCursors, this.position, new Rectangle?(this.sourceRect), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, 1E-06f);
  }
}
