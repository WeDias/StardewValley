// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.PondFishSilhouette
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;

namespace StardewValley.Buildings
{
  public class PondFishSilhouette
  {
    public Vector2 position;
    protected FishPond _pond;
    protected StardewValley.Object _fishObject;
    protected Vector2 _velocity = Vector2.Zero;
    protected float nextDart;
    protected bool _upRight;
    protected float _age;
    protected float _wiggleTimer;
    protected float _sinkAmount = 1f;
    protected float _randomOffset;
    protected bool _flipped;

    public PondFishSilhouette(FishPond pond)
    {
      this._pond = pond;
      this._fishObject = this._pond.GetFishObject();
      if (this._fishObject.HasContextTag("fish_upright"))
        this._upRight = true;
      this.position = (this._pond.GetCenterTile() + new Vector2(0.5f, 0.5f)) * 64f;
      this._age = 0.0f;
      this._randomOffset = Utility.Lerp(0.0f, 500f, (float) Game1.random.NextDouble());
      this.ResetDartTime();
    }

    public void ResetDartTime() => this.nextDart = Utility.Lerp(20f, 40f, (float) Game1.random.NextDouble());

    public void Draw(SpriteBatch b)
    {
      float num1 = 0.7853982f;
      if (this._upRight)
        num1 = 0.0f;
      SpriteEffects effects = SpriteEffects.None;
      float rotation = num1 + (float) (Math.Sin((double) this._wiggleTimer + (double) this._randomOffset) * 2.0 * 3.14159274101257 / 180.0);
      if ((double) this._velocity.Y < 0.0)
        rotation -= 0.1745329f;
      if ((double) this._velocity.Y > 0.0)
        rotation += 0.1745329f;
      if (this._flipped)
      {
        effects = SpriteEffects.FlipHorizontally;
        rotation *= -1f;
      }
      float num2 = Utility.Lerp(0.75f, 0.65f, Utility.Clamp(this._sinkAmount, 0.0f, 1f)) * Utility.Lerp(1f, 0.75f, (float) (int) (NetFieldBase<int, NetInt>) this._pond.currentOccupants / 10f);
      Vector2 position = this.position;
      position.Y += (float) Math.Sin((double) this._age * 2.0 + (double) this._randomOffset) * 5f;
      position.Y += (float) (int) ((double) this._sinkAmount * 4.0);
      float num3 = Utility.Lerp(0.25f, 0.15f, Utility.Clamp(this._sinkAmount, 0.0f, 1f));
      Vector2 origin = new Vector2(8f, 8f);
      b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, position), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this._fishObject.ParentSheetIndex, 16, 16)), Color.Black * num3, rotation, origin, 4f * num2, effects, (float) ((double) this.position.Y / 10000.0 + 9.99999997475243E-07));
    }

    public bool IsMoving() => (double) this._velocity.LengthSquared() > 0.0;

    public void Update(float time)
    {
      this.nextDart -= time;
      this._age += time;
      this._wiggleTimer += time;
      if ((double) this.nextDart <= 0.0 || (double) this.nextDart <= 0.5 && Game1.random.NextDouble() < 0.100000001490116)
      {
        this.ResetDartTime();
        int num = Game1.random.Next(0, 2) * 2 - 1;
        this._flipped = num < 0;
        this._velocity = new Vector2((float) num * Utility.Lerp(50f, 100f, (float) Game1.random.NextDouble()), Utility.Lerp(-50f, 50f, (float) Game1.random.NextDouble()));
      }
      bool flag = false;
      if ((double) this._velocity.LengthSquared() > 0.0)
      {
        flag = true;
        this._wiggleTimer += time * 30f;
        this._sinkAmount = Utility.MoveTowards(this._sinkAmount, 0.0f, 2f * time);
      }
      else
        this._sinkAmount = Utility.MoveTowards(this._sinkAmount, 1f, 1f * time);
      this.position += this._velocity * time;
      for (int index = 0; index < this._pond.GetFishSilhouettes().Count; ++index)
      {
        PondFishSilhouette fishSilhouette = this._pond.GetFishSilhouettes()[index];
        if (fishSilhouette != this)
        {
          float num1 = 30f;
          float num2 = 30f;
          if (this.IsMoving())
            num1 = 0.0f;
          if (fishSilhouette.IsMoving())
            num2 = 0.0f;
          if ((double) Math.Abs(fishSilhouette.position.X - this.position.X) < 32.0)
          {
            if ((double) fishSilhouette.position.X > (double) this.position.X)
            {
              fishSilhouette.position.X += num2 * time;
              this.position.X += -num1 * time;
            }
            else
            {
              fishSilhouette.position.X -= num2 * time;
              this.position.X += num1 * time;
            }
          }
          if ((double) Math.Abs(fishSilhouette.position.Y - this.position.Y) < 32.0)
          {
            if ((double) fishSilhouette.position.Y > (double) this.position.Y)
            {
              fishSilhouette.position.Y += num2 * time;
              this.position.Y += -1f * time;
            }
            else
            {
              fishSilhouette.position.Y -= num2 * time;
              this.position.Y += 1f * time;
            }
          }
        }
      }
      this._velocity.X = Utility.MoveTowards(this._velocity.X, 0.0f, 50f * time);
      this._velocity.Y = Utility.MoveTowards(this._velocity.Y, 0.0f, 20f * time);
      float num3 = 1.3f;
      if ((double) this.position.X > ((double) ((int) (NetFieldBase<int, NetInt>) this._pond.tileX + (int) (NetFieldBase<int, NetInt>) this._pond.tilesWide) - (double) num3) * 64.0)
      {
        this.position.X = (float) (((double) ((int) (NetFieldBase<int, NetInt>) this._pond.tileX + (int) (NetFieldBase<int, NetInt>) this._pond.tilesWide) - (double) num3) * 64.0);
        this._velocity.X *= -1f;
        if (flag && (Game1.random.NextDouble() < 0.25 || (double) Math.Abs(this._velocity.X) > 30.0))
          this._flipped = !this._flipped;
      }
      if ((double) this.position.X < ((double) (int) (NetFieldBase<int, NetInt>) this._pond.tileX + (double) num3) * 64.0)
      {
        this.position.X = (float) (((double) (int) (NetFieldBase<int, NetInt>) this._pond.tileX + (double) num3) * 64.0);
        this._velocity.X *= -1f;
        if (flag && (Game1.random.NextDouble() < 0.25 || (double) Math.Abs(this._velocity.X) > 30.0))
          this._flipped = !this._flipped;
      }
      if ((double) this.position.Y > ((double) ((int) (NetFieldBase<int, NetInt>) this._pond.tileY + (int) (NetFieldBase<int, NetInt>) this._pond.tilesHigh) - (double) num3) * 64.0)
      {
        this.position.Y = (float) (((double) ((int) (NetFieldBase<int, NetInt>) this._pond.tileY + (int) (NetFieldBase<int, NetInt>) this._pond.tilesHigh) - (double) num3) * 64.0);
        this._velocity.Y *= -1f;
      }
      if ((double) this.position.Y >= ((double) (int) (NetFieldBase<int, NetInt>) this._pond.tileY + (double) num3) * 64.0)
        return;
      this.position.Y = (float) (((double) (int) (NetFieldBase<int, NetInt>) this._pond.tileY + (double) num3) * 64.0);
      this._velocity.Y *= -1f;
    }
  }
}
