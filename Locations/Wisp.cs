// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Wisp
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StardewValley.Locations
{
  public class Wisp
  {
    public Vector2 position;
    public Vector2 drawPosition;
    public Vector2[] oldPositions = new Vector2[16];
    public int oldPositionIndex;
    public int index;
    public int tailUpdateTimer;
    public float rotationSpeed;
    public float rotationOffset;
    public float rotationRadius = 16f;
    public float age;
    public float lifeTime = 1f;
    public Color baseColor;

    public Wisp(int index) => this.Reinitialize();

    public virtual void Reinitialize()
    {
      this.baseColor = Color.White * Utility.RandomFloat(0.25f, 0.75f);
      this.rotationOffset = Utility.RandomFloat(0.0f, 360f);
      this.rotationSpeed = Utility.RandomFloat(0.5f, 2f);
      this.rotationRadius = Utility.RandomFloat(8f, 32f);
      this.lifeTime = Utility.RandomFloat(6f, 12f);
      this.age = 0.0f;
      this.position = new Vector2((float) Game1.random.Next(0, Game1.currentLocation.map.DisplayWidth), (float) Game1.random.Next(0, Game1.currentLocation.map.DisplayHeight));
      this.drawPosition = Vector2.Zero;
      for (int index = 0; index < this.oldPositions.Length; ++index)
        this.oldPositions[index] = Vector2.Zero;
    }

    public virtual void Update(GameTime time)
    {
      this.age += (float) time.ElapsedGameTime.TotalSeconds;
      this.position.X -= Math.Max(0.4f, Math.Min(1f, (float) this.index * 0.01f)) - (float) ((double) this.index * 0.00999999977648258 * Math.Sin(2.0 * Math.PI * (double) time.TotalGameTime.Milliseconds / 8000.0));
      this.position.Y += Math.Max(0.5f, Math.Min(1.2f, (float) this.index * 0.02f));
      if ((double) this.age >= (double) this.lifeTime)
        this.Reinitialize();
      else if ((double) this.position.Y > (double) Game1.currentLocation.map.DisplayHeight)
        this.Reinitialize();
      else if ((double) this.position.X < 0.0)
        this.Reinitialize();
      this.drawPosition = this.position + new Vector2((float) Math.Sin((double) this.age * (double) this.rotationSpeed + (double) this.rotationOffset), (float) Math.Sin((double) this.age * (double) this.rotationSpeed + (double) this.rotationOffset)) * this.rotationRadius;
      --this.tailUpdateTimer;
      if (this.tailUpdateTimer > 0)
        return;
      this.tailUpdateTimer = 6;
      this.oldPositionIndex = (this.oldPositionIndex + 1) % this.oldPositions.Length;
      this.oldPositions[this.oldPositionIndex] = this.drawPosition;
    }

    public virtual void Draw(SpriteBatch b)
    {
      Color color = this.baseColor * Utility.Lerp(0.0f, 1f, (float) Math.Sin((double) this.age / (double) this.lifeTime * Math.PI));
      float rotation = (float) ((double) this.age * (double) this.rotationSpeed * 2.0 + (double) this.rotationOffset * (double) this.index);
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.drawPosition), new Rectangle?(new Rectangle(346 + (int) ((double) this.age / 0.25 + (double) this.rotationOffset) % 4 * 5, 1971, 5, 5)), color, rotation, new Vector2(2.5f, 2.5f), 4f, SpriteEffects.None, 1f);
      int index1 = this.oldPositionIndex;
      for (int index2 = 0; index2 < this.oldPositions.Length; ++index2)
      {
        ++index1;
        if (index1 >= this.oldPositions.Length)
          index1 = 0;
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.oldPositions[index1]), new Rectangle?(new Rectangle(356, 1971, 5, 5)), color * ((float) index2 / (float) this.oldPositions.Length), rotation - (float) index2, new Vector2(2.5f, 2.5f), 2f, SpriteEffects.None, 1f);
      }
    }
  }
}
