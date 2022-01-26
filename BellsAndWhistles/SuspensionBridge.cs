// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.SuspensionBridge
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class SuspensionBridge
  {
    public Rectangle bridgeBounds;
    public List<Rectangle> bridgeEntrances = new List<Rectangle>();
    public List<Rectangle> bridgeSortRegions = new List<Rectangle>();
    public const float BRIDGE_SORT_OFFSET = 0.0256f;
    protected Texture2D _texture;
    public float shakeTime;

    public SuspensionBridge() => this._texture = Game1.content.Load<Texture2D>("LooseSprites\\SuspensionBridge");

    public SuspensionBridge(int tile_x, int tile_y)
      : this()
    {
      this.bridgeBounds = new Rectangle(tile_x * 64, tile_y * 64, 384, 64);
      this.bridgeEntrances.Add(new Rectangle((tile_x - 1) * 64, tile_y * 64, 64, 64));
      this.bridgeEntrances.Add(new Rectangle((tile_x + 6) * 64, tile_y * 64, 64, 64));
      this.bridgeSortRegions.Add(new Rectangle((tile_x - 1) * 64, (tile_y - 1) * 64, 128, 192));
      this.bridgeSortRegions.Add(new Rectangle((tile_x + 5) * 64, (tile_y - 1) * 64, 128, 192));
    }

    public virtual bool InEntranceArea(int x, int y)
    {
      foreach (Rectangle bridgeEntrance in this.bridgeEntrances)
      {
        if (bridgeEntrance.Contains(x, y))
          return true;
      }
      return false;
    }

    public virtual bool InEntranceArea(Rectangle rectangle)
    {
      foreach (Rectangle bridgeEntrance in this.bridgeEntrances)
      {
        if (bridgeEntrance.Contains(rectangle))
          return true;
      }
      return false;
    }

    public virtual bool CheckPlacementPrevention(Vector2 tileLocation)
    {
      foreach (Rectangle bridgeEntrance in this.bridgeEntrances)
      {
        if (Utility.doesRectangleIntersectTile(bridgeEntrance, (int) tileLocation.X, (int) tileLocation.Y))
          return true;
      }
      return false;
    }

    public virtual void OnFootstep(Vector2 position)
    {
      if (!this.bridgeBounds.Contains((int) position.X, (int) position.Y) || (double) position.X <= (double) (this.bridgeBounds.X + 64) || (double) position.X >= (double) (this.bridgeBounds.Right - 64))
        return;
      this.shakeTime = 0.4f;
    }

    public virtual void Update(GameTime time)
    {
      if ((double) this.shakeTime > 0.0)
      {
        this.shakeTime -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.shakeTime < 0.0)
          this.shakeTime = 0.0f;
      }
      if (Game1.player.bridge == null && this.InEntranceArea(Game1.player.GetBoundingBox()))
        Game1.player.bridge = this;
      if (Game1.player.bridge != this)
        return;
      Rectangle boundingBox = Game1.player.GetBoundingBox();
      if (boundingBox.Top >= this.bridgeBounds.Top && boundingBox.Bottom <= this.bridgeBounds.Bottom && (boundingBox.Intersects(this.bridgeBounds) || this.InEntranceArea(boundingBox)))
      {
        Game1.player.SetOnBridge(true);
      }
      else
      {
        if (this.InEntranceArea(Game1.player.GetBoundingBox()) || boundingBox.Intersects(this.bridgeBounds))
          return;
        Game1.player.SetOnBridge(false);
        Game1.player.bridge = (SuspensionBridge) null;
      }
    }

    public virtual void Draw(SpriteBatch b)
    {
      b.Draw(this._texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) this.bridgeBounds.X, (float) (this.bridgeBounds.Y - 128))), new Rectangle?(new Rectangle(0, 0, 96, 32)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.bridgeBounds.Y / 10000.0 + 0.0255999993532896));
      float[] numArray = new float[6]
      {
        0.0f,
        0.5f,
        1f,
        1f,
        0.5f,
        0.0f
      };
      for (int index = 0; index < 6; ++index)
      {
        float num = (float) (Math.Sin(Game1.currentGameTime.TotalGameTime.TotalSeconds * 10.0 + (double) (index * 5)) * 1.0 * 4.0 * (double) numArray[index] * ((double) this.shakeTime / 0.400000005960464));
        b.Draw(this._texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.bridgeBounds.X + index * 64), (float) this.bridgeBounds.Y + num)), new Rectangle?(new Rectangle(16 * index, 32, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.bridgeBounds.Y / 10000.0 + 0.0255999993532896));
      }
    }
  }
}
