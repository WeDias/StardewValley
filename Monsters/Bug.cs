// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Bug
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Monsters
{
  public class Bug : Monster
  {
    [XmlElement("isArmoredBug")]
    public readonly NetBool isArmoredBug = new NetBool(false);

    public Bug()
    {
    }

    public Bug(Vector2 position, int areaType)
      : base(nameof (Bug), position)
    {
      this.Sprite.SpriteHeight = 16;
      this.Sprite.UpdateSourceRect();
      this.onCollision = new Monster.collisionBehavior(this.collide);
      this.yOffset = -32f;
      this.IsWalkingTowardPlayer = false;
      this.setMovingInFacingDirection();
      this.defaultAnimationInterval.Value = 40;
      this.collidesWithOtherCharacters.Value = false;
      if (areaType == 121)
      {
        this.isArmoredBug.Value = true;
        this.Sprite.LoadTexture("Characters\\Monsters\\Armored Bug");
        this.DamageToFarmer *= 2;
        this.Slipperiness = -1;
        this.Health = 150;
      }
      this.HideShadow = true;
    }

    public Bug(Vector2 position, int facingDirection, MineShaft mine)
      : this(position, mine.getMineArea())
    {
      this.faceDirection(facingDirection);
      this.HideShadow = true;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddField((INetSerializable) this.isArmoredBug);
    }

    protected override void updateMonsterSlaveAnimation(GameTime time)
    {
      this.Sprite.faceDirection(this.FacingDirection);
      this.Sprite.animateOnce(time);
    }

    public override bool passThroughCharacters() => true;

    public override void reloadSprite()
    {
      base.reloadSprite();
      this.Sprite.SpriteHeight = 16;
      this.Sprite.UpdateSourceRect();
    }

    private void collide(GameLocation location)
    {
      Rectangle rectangle = this.nextPosition(this.FacingDirection);
      foreach (Character farmer in location.farmers)
      {
        if (farmer.GetBoundingBox().Intersects(rectangle))
          return;
      }
      this.FacingDirection = (this.FacingDirection + 2) % 4;
      this.setMovingInFacingDirection();
    }

    public override void BuffForAdditionalDifficulty(int additional_difficulty)
    {
      this.FacingDirection = Math.Abs((this.FacingDirection + Game1.random.Next(-1, 2)) % 4);
      this.Halt();
      this.setMovingInFacingDirection();
      base.BuffForAdditionalDifficulty(additional_difficulty);
    }

    public override int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      Farmer who)
    {
      int damage1 = Math.Max(1, damage - (int) (NetFieldBase<int, NetInt>) this.resilience);
      if ((bool) (NetFieldBase<bool, NetBool>) this.isArmoredBug && (isBomb || !(who.CurrentTool is MeleeWeapon) || !(who.CurrentTool as MeleeWeapon).hasEnchantmentOfType<BugKillerEnchantment>()))
      {
        this.currentLocation.playSound("crafting");
        return 0;
      }
      if (Game1.random.NextDouble() < (double) (NetFieldBase<double, NetDouble>) this.missChance - (double) (NetFieldBase<double, NetDouble>) this.missChance * addedPrecision)
      {
        damage1 = -1;
      }
      else
      {
        this.Health -= damage1;
        this.currentLocation.playSound("hitEnemy");
        this.setTrajectory(xTrajectory / 3, yTrajectory / 3);
        if ((bool) (NetFieldBase<bool, NetBool>) this.isHardModeMonster)
        {
          this.FacingDirection = Math.Abs((this.FacingDirection + Game1.random.Next(-1, 2)) % 4);
          this.Halt();
          this.setMovingInFacingDirection();
        }
        if (this.Health <= 0)
          this.deathAnimation();
      }
      return damage1;
    }

    public override List<Item> getExtraDropItems()
    {
      if (!this.isArmoredBug.Value)
        return base.getExtraDropItems();
      List<Item> extraDropItems = new List<Item>();
      if (Game1.random.NextDouble() <= 0.1)
        extraDropItems.Add((Item) new StardewValley.Object(874, 1));
      return extraDropItems;
    }

    public override void draw(SpriteBatch b)
    {
      if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128))
        return;
      Vector2 vector2 = new Vector2();
      if (this.FacingDirection % 2 == 0)
        vector2.X = (float) (Math.Sin((double) Game1.currentGameTime.TotalGameTime.Milliseconds / 1000.0 * (2.0 * Math.PI)) * 10.0);
      else
        vector2.Y = (float) (Math.Sin((double) Game1.currentGameTime.TotalGameTime.Milliseconds / 1000.0 * (2.0 * Math.PI)) * 10.0);
      SpriteBatch spriteBatch = b;
      Texture2D shadowTexture = Game1.shadowTexture;
      Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.Sprite.SpriteWidth * 4) / 2f + vector2.X, (float) (this.GetBoundingBox().Height * 5 / 2 - 48));
      Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
      Color white = Color.White;
      Rectangle bounds = Game1.shadowTexture.Bounds;
      double x = (double) bounds.Center.X;
      bounds = Game1.shadowTexture.Bounds;
      double y = (double) bounds.Center.Y;
      Vector2 origin = new Vector2((float) x, (float) y);
      double scale = (4.0 + (double) this.yJumpOffset / 40.0) * (double) (float) (NetFieldBase<float, NetFloat>) this.scale;
      double layerDepth = (double) Math.Max(0.0f, (float) this.getStandingY() / 10000f) - 9.99999997475243E-07;
      spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) this.yJumpOffset) + vector2, new Rectangle?(this.Sprite.SourceRect), Color.White, this.rotation, new Vector2(8f, 16f), 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));
    }

    protected override void localDeathAnimation()
    {
      base.localDeathAnimation();
      this.currentLocation.localSound("slimedead");
      Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position + new Vector2(0.0f, -32f), Color.Violet, 10)
      {
        holdLastFrame = true,
        alphaFade = 0.01f,
        interval = 70f
      }, this.currentLocation);
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
      Game1.createRadialDebris(currentLocation, textureName, sourcerectangle, 8, x, y1, numberOfChunks, y2, white, 4f);
    }
  }
}
