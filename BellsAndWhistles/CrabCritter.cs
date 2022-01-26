// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.CrabCritter
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using xTile.Dimensions;

namespace StardewValley.BellsAndWhistles
{
  public class CrabCritter : Critter
  {
    public Microsoft.Xna.Framework.Rectangle movementRectangle;
    public float nextCharacterCheck = 2f;
    public float nextFrameChange;
    public float nextMovementChange;
    public bool moving;
    public bool diving;
    public bool skittering;
    protected float skitterTime = 5f;
    protected Microsoft.Xna.Framework.Rectangle _baseSourceRectangle = new Microsoft.Xna.Framework.Rectangle(0, 272, 18, 18);
    protected int _currentFrame;
    protected int _crabVariant;
    protected Vector2 movementDirection = Vector2.Zero;
    public Microsoft.Xna.Framework.Rectangle movementBounds;

    public CrabCritter()
    {
      this.sprite = new AnimatedSprite(Critter.critterTexture, 0, 18, 18);
      this.sprite.SourceRect = this._baseSourceRectangle;
      this.sprite.ignoreSourceRectUpdates = true;
      this._crabVariant = 1;
      this.UpdateSpriteRectangle();
    }

    public CrabCritter(Vector2 start_position)
      : this()
    {
      this.position = start_position;
      float width = 256f;
      this.movementBounds = new Microsoft.Xna.Framework.Rectangle((int) ((double) start_position.X - (double) width / 2.0), (int) start_position.Y, (int) width, 0);
    }

    public override bool update(GameTime time, GameLocation environment)
    {
      this.nextFrameChange -= (float) time.ElapsedGameTime.TotalSeconds;
      if (this.skittering)
        this.skitterTime -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.nextFrameChange <= 0.0 && (this.moving || this.skittering))
      {
        ++this._currentFrame;
        if (this._currentFrame >= 4)
          this._currentFrame = 0;
        this.nextFrameChange = !this.skittering ? Utility.RandomFloat(0.05f, 0.15f) : Utility.RandomFloat(0.025f, 0.05f);
      }
      if (this.skittering)
      {
        if ((double) this.yJumpOffset >= 0.0)
        {
          if (!this.diving)
          {
            if (Game1.random.Next(0, 4) == 0)
              this.gravityAffectedDY = -4f;
            else
              this.gravityAffectedDY = -2f;
          }
          else
          {
            if (environment.doesTileHaveProperty((int) this.position.X / 64, (int) this.position.Y / 64, "Water", "Back") != null)
            {
              environment.TemporarySprites.Add(new TemporaryAnimatedSprite(28, 50f, 2, 1, this.position, false, false));
              Game1.playSound("dropItemInWater");
              return true;
            }
            this.gravityAffectedDY = -4f;
          }
        }
      }
      else
      {
        this.nextCharacterCheck -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.nextCharacterCheck <= 0.0)
        {
          Character character = Utility.isThereAFarmerOrCharacterWithinDistance(this.position / 64f, 7, environment);
          if (character != null)
          {
            this._crabVariant = 0;
            this.skittering = true;
            this.movementDirection.X = (double) character.position.X <= (double) this.position.X ? 3f : -3f;
          }
          this.nextCharacterCheck = 0.25f;
        }
        if (!this.skittering)
        {
          if (this.moving && (double) this.yJumpOffset >= 0.0)
            this.gravityAffectedDY = -1f;
          this.nextMovementChange -= (float) time.ElapsedGameTime.TotalSeconds;
          if ((double) this.nextMovementChange <= 0.0)
          {
            this.moving = !this.moving;
            if (this.moving)
              this.movementDirection.X = Game1.random.NextDouble() <= 0.5 ? -1f : 1f;
            else
              this.movementDirection = Vector2.Zero;
            this.nextMovementChange = !this.moving ? Utility.RandomFloat(0.2f, 1f) : Utility.RandomFloat(0.15f, 0.5f);
          }
        }
      }
      this.position = this.position + this.movementDirection;
      if (!this.diving && !environment.isTilePassable(new Location((int) ((double) this.position.X / 64.0), (int) ((double) this.position.Y / 64.0)), Game1.viewport))
      {
        this.position = this.position - this.movementDirection;
        this.movementDirection *= -1f;
      }
      if (!this.skittering)
      {
        if ((double) this.position.X < (double) this.movementBounds.Left)
        {
          this.position.X = (float) this.movementBounds.Left;
          this.movementDirection *= -1f;
        }
        if ((double) this.position.X > (double) this.movementBounds.Right)
        {
          this.position.X = (float) this.movementBounds.Right;
          this.movementDirection *= -1f;
        }
      }
      else if (!this.diving && environment.doesTileHaveProperty((int) ((double) this.position.X / 64.0 + (double) Math.Sign(this.movementDirection.X) * 1.0), (int) this.position.Y / 64, "Water", "Back") != null)
      {
        if ((double) this.yJumpOffset >= 0.0)
          this.gravityAffectedDY = -7f;
        this.diving = true;
      }
      this.UpdateSpriteRectangle();
      return (double) this.skitterTime <= 0.0 || base.update(time, environment);
    }

    public virtual void UpdateSpriteRectangle()
    {
      Microsoft.Xna.Framework.Rectangle baseSourceRectangle = this._baseSourceRectangle;
      baseSourceRectangle.Y += this._crabVariant * 18;
      int num = this._currentFrame;
      if (num == 3)
        num = 1;
      baseSourceRectangle.X += num * 18;
      this.sprite.SourceRect = baseSourceRectangle;
    }

    public override void draw(SpriteBatch b)
    {
      float num = this.skitterTime;
      if ((double) num > 1.0)
        num = 1f;
      if ((double) num < 0.0)
        num = 0.0f;
      this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, Utility.snapDrawPosition(this.position + new Vector2(0.0f, this.yJumpOffset - 20f + this.yOffset))), (float) (((double) this.position.Y + 64.0 - 32.0) / 10000.0), 0, 0, Color.White * num, this.flip, 4f);
      b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(32f, 40f)), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White * num, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 3f + Math.Max(-3f, (float) (((double) this.yJumpOffset + (double) this.yOffset) / 16.0)), SpriteEffects.None, (float) (((double) this.position.Y - 1.0) / 10000.0));
    }

    public override void drawAboveFrontLayer(SpriteBatch b)
    {
    }
  }
}
