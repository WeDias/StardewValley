// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Firefly
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley.BellsAndWhistles
{
  public class Firefly : Critter
  {
    private bool glowing;
    private int glowTimer;
    private int id;
    private Vector2 motion;
    private LightSource light;

    public Firefly()
    {
    }

    public Firefly(Vector2 position)
    {
      this.baseFrame = -1;
      this.position = position * 64f;
      this.startingPosition = position * 64f;
      this.motion = new Vector2((float) Game1.random.Next(-10, 11) * 0.1f, (float) Game1.random.Next(-10, 11) * 0.1f);
      this.id = (int) ((double) position.X * 10099.0 + (double) position.Y * 77.0 + (double) Game1.random.Next(99999));
      this.light = new LightSource(4, position, (float) Game1.random.Next(4, 6) * 0.1f, Color.Purple * 0.8f, this.id);
      this.glowing = true;
      Game1.currentLightSources.Add(this.light);
    }

    public override bool update(GameTime time, GameLocation environment)
    {
      this.position = this.position + this.motion;
      this.motion.X += (float) Game1.random.Next(-1, 2) * 0.1f;
      this.motion.Y += (float) Game1.random.Next(-1, 2) * 0.1f;
      if ((double) this.motion.X < -1.0)
        this.motion.X = -1f;
      if ((double) this.motion.X > 1.0)
        this.motion.X = 1f;
      if ((double) this.motion.Y < -1.0)
        this.motion.Y = -1f;
      if ((double) this.motion.Y > 1.0)
        this.motion.Y = 1f;
      if (this.glowing)
        this.light.position.Value = this.position;
      return (double) this.position.X < (double) sbyte.MinValue || (double) this.position.Y < (double) sbyte.MinValue || (double) this.position.X > (double) environment.map.DisplayWidth || (double) this.position.Y > (double) environment.map.DisplayHeight;
    }

    public override void drawAboveFrontLayer(SpriteBatch b) => b.Draw(Game1.staminaRect, Game1.GlobalToLocal(this.position), new Rectangle?(Game1.staminaRect.Bounds), this.glowing ? Color.White : Color.Brown, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
  }
}
