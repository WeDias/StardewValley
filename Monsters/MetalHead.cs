// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.MetalHead
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Monsters
{
  public class MetalHead : Monster
  {
    [XmlElement("c")]
    public readonly NetColor c = new NetColor();

    public MetalHead()
    {
    }

    public MetalHead(Vector2 tileLocation, MineShaft mine)
      : this(tileLocation, mine.getMineArea())
    {
    }

    public MetalHead(string name, Vector2 tileLocation)
      : base(name, tileLocation)
    {
      this.Sprite.SpriteHeight = 16;
      this.Sprite.UpdateSourceRect();
      this.c.Value = Color.White;
      this.IsWalkingTowardPlayer = true;
    }

    public MetalHead(Vector2 tileLocation, int mineArea)
      : base("Metal Head", tileLocation)
    {
      this.Sprite.SpriteHeight = 16;
      this.Sprite.UpdateSourceRect();
      this.c.Value = Color.White;
      this.IsWalkingTowardPlayer = true;
      switch (mineArea)
      {
        case 0:
          this.c.Value = Color.White;
          break;
        case 40:
          this.c.Value = Color.Turquoise;
          this.Health *= 2;
          break;
        case 80:
          this.c.Value = Color.White;
          this.Health *= 3;
          break;
      }
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.c);
      this.position.Field.AxisAlignedMovement = true;
    }

    public override int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      Farmer who)
    {
      return this.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, "clank");
    }

    protected override void localDeathAnimation()
    {
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.Position, Color.DarkGray, 10, animationInterval: 70f));
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.Position + new Vector2(-32f, 0.0f), Color.DarkGray, 10, animationInterval: 70f)
      {
        delayBeforeAnimationStart = 300
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.Position + new Vector2(32f, 0.0f), Color.DarkGray, 10, animationInterval: 70f)
      {
        delayBeforeAnimationStart = 600
      });
      this.currentLocation.localSound("monsterdead");
      Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.MediumPurple, 10)
      {
        holdLastFrame = true,
        alphaFade = 0.01f,
        interval = 70f
      }, this.currentLocation);
      base.localDeathAnimation();
    }

    public override void draw(SpriteBatch b)
    {
      if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128))
        return;
      SpriteBatch spriteBatch = b;
      Texture2D shadowTexture = Game1.shadowTexture;
      Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, 42f + this.yOffset);
      Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
      Color white = Color.White;
      Rectangle bounds = Game1.shadowTexture.Bounds;
      double x = (double) bounds.Center.X;
      bounds = Game1.shadowTexture.Bounds;
      double y = (double) bounds.Center.Y;
      Vector2 origin = new Vector2((float) x, (float) y);
      double scale = 3.5 + (double) (float) (NetFieldBase<float, NetFloat>) this.scale + (double) this.yOffset / 30.0;
      double layerDepth = (double) (this.getStandingY() - 1) / 10000.0;
      spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (48 + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), (Color) (NetFieldBase<Color, NetColor>) this.c, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));
    }

    public override void shedChunks(int number, float scale)
    {
      GameLocation currentLocation = this.currentLocation;
      string textureName = (string) (NetFieldBase<string, NetString>) this.Sprite.textureName;
      Rectangle sourcerectangle = new Rectangle(0, this.Sprite.getHeight() * 4, 16, 16);
      Rectangle boundingBox = this.GetBoundingBox();
      int x = boundingBox.Center.X;
      boundingBox = this.GetBoundingBox();
      int y1 = boundingBox.Center.Y;
      int numberOfChunks = number;
      int y2 = (int) this.getTileLocation().Y;
      Color white = Color.White;
      double scale1 = (double) scale * 4.0;
      Game1.createRadialDebris(currentLocation, textureName, sourcerectangle, 8, x, y1, numberOfChunks, y2, white, (float) scale1);
    }

    public override List<Item> getExtraDropItems()
    {
      List<Item> extraDropItems = new List<Item>();
      if ((Game1.stats.getMonstersKilled((string) (NetFieldBase<string, NetString>) this.name) + (int) Game1.uniqueIDForThisGame) % 100 == 0)
        extraDropItems.Add((Item) new Hat(51));
      return extraDropItems;
    }

    protected override void updateMonsterSlaveAnimation(GameTime time)
    {
      if (this.isMoving())
      {
        if (this.FacingDirection == 0)
          this.Sprite.AnimateUp(time);
        else if (this.FacingDirection == 3)
          this.Sprite.AnimateLeft(time);
        else if (this.FacingDirection == 1)
        {
          this.Sprite.AnimateRight(time);
        }
        else
        {
          if (this.FacingDirection != 2)
            return;
          this.Sprite.AnimateDown(time);
        }
      }
      else
        this.Sprite.StopAnimation();
    }
  }
}
