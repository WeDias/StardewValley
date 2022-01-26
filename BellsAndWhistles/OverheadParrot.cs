// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.OverheadParrot
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StardewValley.BellsAndWhistles
{
  public class OverheadParrot : Critter
  {
    protected Texture2D _texture;
    public Vector2 velocity;
    public float age;
    public float flyOffset;
    public float height = 64f;
    public Rectangle sourceRect;
    public Vector2 drawOffset;
    public int[] spriteFlapFrames = new int[8]
    {
      0,
      0,
      0,
      0,
      1,
      2,
      2,
      1
    };
    public int currentFlapIndex;
    public int flapFrameAccumulator;
    public Vector2 swayAmount;
    public Vector2 lastDrawPosition;
    protected bool _shouldDrawShadow;

    public OverheadParrot(Vector2 start_position)
    {
      this.position = start_position;
      this.velocity = new Vector2(Utility.RandomFloat(-4f, -2f), Utility.RandomFloat(5f, 6f));
      this._texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\parrots");
      this.sourceRect = new Rectangle(0, 0, 24, 24);
      this.sourceRect.Y = 24 * Game1.random.Next(4);
      this.currentFlapIndex = Game1.random.Next(this.spriteFlapFrames.Length);
      this.flyOffset = (float) (Game1.random.NextDouble() * 100.0);
      this.swayAmount.X = Utility.RandomFloat(16f, 32f);
      this.swayAmount.Y = Utility.RandomFloat(10f, 24f);
    }

    public override bool update(GameTime time, GameLocation environment)
    {
      ++this.flapFrameAccumulator;
      if (this.flapFrameAccumulator >= 2)
      {
        ++this.currentFlapIndex;
        if (this.currentFlapIndex >= this.spriteFlapFrames.Length)
          this.currentFlapIndex = 0;
        this.flapFrameAccumulator = 0;
      }
      this.age += (float) time.ElapsedGameTime.TotalSeconds;
      this.position = this.position + this.velocity;
      float a = (float) (((double) this.age + (double) this.flyOffset) * 1.0);
      float d = (float) (((double) this.age + (double) this.flyOffset) * 2.0);
      this.drawOffset.X = (float) Math.Sin((double) a) * this.swayAmount.X;
      this.drawOffset.Y = (float) Math.Cos((double) d) * this.swayAmount.Y;
      Vector2 drawPosition = this.GetDrawPosition();
      if (this.currentFlapIndex == 4 && this.flapFrameAccumulator == 0 && Utility.isOnScreen(drawPosition, 64))
        Game1.playSound("batFlap");
      Vector2 vector2 = drawPosition - this.lastDrawPosition;
      this.lastDrawPosition = drawPosition;
      int num = 2;
      if ((double) Math.Abs(vector2.X) < (double) Math.Abs(vector2.Y))
        num = 5;
      this.sourceRect.X = (this.spriteFlapFrames[this.currentFlapIndex] + num) * 24;
      this._shouldDrawShadow = true;
      Vector2 shadowPosition = this.GetShadowPosition();
      if (Game1.currentLocation.getTileIndexAt((int) shadowPosition.X / 64, (int) shadowPosition.Y / 64, "Back") == -1)
        this._shouldDrawShadow = false;
      return (double) this.position.X < -64.0 - (double) this.swayAmount.X * 4.0 || (double) this.position.Y > (double) (environment.map.Layers[0].DisplayHeight + 64) + ((double) this.height + (double) this.swayAmount.Y) * 4.0;
    }

    public Vector2 GetDrawPosition() => this.position + new Vector2(this.drawOffset.X, -this.height + this.drawOffset.Y) * 4f;

    public Vector2 GetShadowPosition() => this.position + new Vector2(this.drawOffset.X * 4f, -4f);

    public override void draw(SpriteBatch b)
    {
      if (!this._shouldDrawShadow)
        return;
      b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.GetShadowPosition()), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, (float) (((double) this.position.Y - 1.0) / 10000.0));
    }

    public override void drawAboveFrontLayer(SpriteBatch b) => b.Draw(this._texture, Game1.GlobalToLocal(Game1.viewport, this.GetDrawPosition()), new Rectangle?(this.sourceRect), Color.White, 0.0f, new Vector2(12f, 20f), 4f, SpriteEffects.None, this.position.Y / 10000f);
  }
}
