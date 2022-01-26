// Decompiled with JetBrains decompiler
// Type: StardewValley.SeaMonsterTemporarySprite
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley
{
  public class SeaMonsterTemporarySprite : TemporaryAnimatedSprite
  {
    public new Texture2D texture;

    public SeaMonsterTemporarySprite(
      float animationInterval,
      int animationLength,
      int numberOfLoops,
      Vector2 position)
      : base(-666, animationInterval, animationLength, numberOfLoops, position, false, false)
    {
      this.texture = Game1.content.Load<Texture2D>("LooseSprites\\SeaMonster");
      Game1.playSound("pullItemFromWater");
      this.currentParentTileIndex = 0;
    }

    public override void draw(
      SpriteBatch spriteBatch,
      bool localPosition = false,
      int xOffset = 0,
      int yOffset = 0,
      float extraAlpha = 1f)
    {
      spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, this.Position), new Rectangle?(new Rectangle(this.currentParentTileIndex * 16, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.Position.Y + 32.0) / 10000.0));
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
          this.currentParentTileIndex = 2;
        }
      }
      if (this.currentNumberOfLoops >= this.totalNumberOfLoops)
      {
        this.position.Y += 2f;
        if ((double) this.position.Y >= (double) Game1.currentLocation.Map.DisplayHeight)
          return true;
      }
      return false;
    }
  }
}
