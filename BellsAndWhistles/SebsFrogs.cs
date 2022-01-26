// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.SebsFrogs
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;

namespace StardewValley.BellsAndWhistles
{
  public class SebsFrogs : TemporaryAnimatedSprite
  {
    private float yOriginal;
    private bool flipJump;

    public override bool update(GameTime time)
    {
      base.update(time);
      if (!this.pingPong && this.motion.Equals(Vector2.Zero) && Game1.random.NextDouble() < 0.003)
      {
        if (Game1.random.NextDouble() < 0.4)
        {
          this.animationLength = 3;
          this.pingPong = true;
        }
        else
        {
          this.flipJump = !this.flipJump;
          this.yOriginal = this.position.Y;
          this.motion = new Vector2(this.flipJump ? -1f : 1f, -3f);
          this.acceleration = new Vector2(0.0f, 0.2f);
          this.sourceRect.X = 0;
          this.interval = (float) Game1.random.Next(110, 150);
          this.animationLength = 5;
          this.flipped = this.flipJump;
          if (this.Parent != null && this.Parent == Game1.currentLocation && Game1.random.NextDouble() < 0.03)
            Game1.playSound("croak");
        }
      }
      else if (this.pingPong && Game1.random.NextDouble() < 0.02 && this.sourceRect.X == 64)
      {
        this.animationLength = 1;
        this.pingPong = false;
        this.sourceRect.X = (int) this.sourceRectStartingPos.X;
      }
      if (!this.motion.Equals(Vector2.Zero) && (double) this.position.Y > (double) this.yOriginal)
      {
        this.motion = Vector2.Zero;
        this.acceleration = Vector2.Zero;
        this.sourceRect.X = 64;
        this.animationLength = 1;
        this.position.Y = this.yOriginal;
      }
      return false;
    }
  }
}
