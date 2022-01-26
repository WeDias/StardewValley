// Decompiled with JetBrains decompiler
// Type: StardewValley.BatTemporarySprite
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley
{
  public class BatTemporarySprite : TemporaryAnimatedSprite
  {
    public new Texture2D texture;
    private bool moveLeft;
    private int horizontalSpeed;
    private float verticalSpeed;

    public BatTemporarySprite(Vector2 position)
      : base(-666, 100f, 4, 99999, position, false, false)
    {
      this.texture = Game1.content.Load<Texture2D>("LooseSprites\\Bat");
      this.currentParentTileIndex = 0;
      if ((double) position.X > (double) (Game1.currentLocation.Map.DisplayWidth / 2))
        this.moveLeft = true;
      this.horizontalSpeed = Game1.random.Next(1, 8);
      this.verticalSpeed = (float) Game1.random.Next(3, 7);
      this.interval = (float) (160.0 - ((double) this.horizontalSpeed + (double) this.verticalSpeed) * 10.0);
    }

    public override void draw(
      SpriteBatch spriteBatch,
      bool localPosition = false,
      int xOffset = 0,
      int yOffset = 0,
      float extraAlpha = 1f)
    {
      spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, this.Position), new Rectangle?(new Rectangle(this.currentParentTileIndex * 64, 0, 64, 64)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) (((double) this.Position.Y + 32.0) / 10000.0));
    }

    public override bool update(GameTime time)
    {
      this.timer += (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.timer > (double) this.interval)
      {
        ++this.currentParentTileIndex;
        this.timer = 0.0f;
        if (this.currentParentTileIndex >= this.animationLength)
        {
          ++this.currentNumberOfLoops;
          this.currentParentTileIndex = 0;
        }
      }
      if (this.moveLeft)
        this.position.X -= (float) this.horizontalSpeed;
      else
        this.position.X += (float) this.horizontalSpeed;
      this.position.Y += this.verticalSpeed;
      this.verticalSpeed -= 0.1f;
      return (double) this.position.Y >= (double) Game1.currentLocation.Map.DisplayHeight || (double) this.position.Y < 0.0 || (double) this.position.X < 0.0 || (double) this.position.X >= (double) Game1.currentLocation.Map.DisplayWidth;
    }
  }
}
