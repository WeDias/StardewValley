// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Critter
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StardewValley.BellsAndWhistles
{
  public abstract class Critter
  {
    public const int spriteWidth = 32;
    public const int spriteHeight = 32;
    public const float gravity = 0.25f;
    public static string critterTexture = "TileSheets\\critters";
    public Vector2 position;
    public Vector2 startingPosition;
    public int baseFrame;
    public AnimatedSprite sprite;
    public bool flip;
    public float gravityAffectedDY;
    public float yOffset;
    public float yJumpOffset;

    public static void InitShared()
    {
    }

    public Critter()
    {
    }

    public Critter(int baseFrame, Vector2 position)
    {
      this.baseFrame = baseFrame;
      this.position = position;
      this.sprite = new AnimatedSprite(Critter.critterTexture, baseFrame, 32, 32);
      this.startingPosition = position;
    }

    public virtual Rectangle getBoundingBox(int xOffset, int yOffset) => new Rectangle((int) this.position.X - 32 + xOffset, (int) this.position.Y - 16 + yOffset, 64, 32);

    public virtual bool update(GameTime time, GameLocation environment)
    {
      this.sprite.animateOnce(time);
      if ((double) this.gravityAffectedDY < 0.0 || (double) this.yJumpOffset < 0.0)
      {
        this.yJumpOffset += this.gravityAffectedDY;
        this.gravityAffectedDY += 0.25f;
      }
      return (double) this.position.X < (double) sbyte.MinValue || (double) this.position.Y < (double) sbyte.MinValue || (double) this.position.X > (double) environment.map.DisplayWidth || (double) this.position.Y > (double) environment.map.DisplayHeight;
    }

    public virtual void draw(SpriteBatch b)
    {
      if (this.sprite == null)
        return;
      this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)), (float) ((double) this.position.Y / 10000.0 + (double) this.position.X / 100000.0), 0, 0, Color.White, this.flip, 4f);
      b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(0.0f, -4f)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 3f + Math.Max(-3f, (float) (((double) this.yJumpOffset + (double) this.yOffset) / 64.0)), SpriteEffects.None, (float) (((double) this.position.Y - 1.0) / 10000.0));
    }

    public virtual void drawAboveFrontLayer(SpriteBatch b)
    {
    }
  }
}
