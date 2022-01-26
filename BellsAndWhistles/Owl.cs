// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Owl
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class Owl : Critter
  {
    public Owl()
    {
    }

    public Owl(Vector2 position)
    {
      this.baseFrame = 83;
      this.position = position;
      this.sprite = new AnimatedSprite(Critter.critterTexture, this.baseFrame, 32, 32);
      this.startingPosition = position;
      this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(83, 100),
        new FarmerSprite.AnimationFrame(84, 100),
        new FarmerSprite.AnimationFrame(85, 100),
        new FarmerSprite.AnimationFrame(86, 100)
      });
    }

    public override bool update(GameTime time, GameLocation environment)
    {
      Vector2 vector2 = new Vector2((float) Game1.viewport.X - Game1.previousViewportPosition.X, (float) Game1.viewport.Y - Game1.previousViewportPosition.Y) * 0.15f;
      this.position.Y += (float) (time.ElapsedGameTime.TotalMilliseconds * 0.200000002980232);
      this.position.X += (float) (time.ElapsedGameTime.TotalMilliseconds * 0.0500000007450581);
      this.position = this.position - vector2;
      return base.update(time, environment);
    }

    public override void draw(SpriteBatch b)
    {
    }

    public override void drawAboveFrontLayer(SpriteBatch b) => this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)), (float) ((double) this.position.Y / 10000.0 + (double) this.position.X / 100000.0), 0, 0, Color.MediumBlue, this.flip, 4f);
  }
}
