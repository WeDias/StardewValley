// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.HotHead
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;
using System.Xml.Serialization;

namespace StardewValley.Monsters
{
  public class HotHead : MetalHead
  {
    [XmlIgnore]
    public NetFarmerRef lastAttacker = new NetFarmerRef();
    [XmlIgnore]
    public NetFloat timeUntilExplode = new NetFloat(-1f);
    [XmlIgnore]
    public NetBool angry = new NetBool();

    public HotHead()
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.lastAttacker.NetFields, (INetSerializable) this.angry, (INetSerializable) this.timeUntilExplode);
    }

    public HotHead(Vector2 position)
      : base("Hot Head", position)
    {
      this.Slipperiness *= 2;
    }

    public override int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      Farmer who)
    {
      this.lastAttacker.Value = who;
      int damage1 = base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.timeUntilExplode != -1.0 || this.Health >= 25)
        return damage1;
      this.currentLocation.netAudio.StartPlaying("fuse");
      this.timeUntilExplode.Value = 2.4f;
      this.Speed = 5;
      this.angry.Value = true;
      return damage1;
    }

    public override void behaviorAtGameTick(GameTime time)
    {
      if (Game1.IsMasterGame && (double) (float) (NetFieldBase<float, NetFloat>) this.timeUntilExplode > 0.0)
      {
        this.timeUntilExplode.Value -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) (float) (NetFieldBase<float, NetFloat>) this.timeUntilExplode <= 0.0)
        {
          this.currentLocation.netAudio.StopPlaying("fuse");
          this.timeUntilExplode.Value = 0.0f;
          this.DropBomb();
          this.Health = -9999;
          return;
        }
      }
      base.behaviorAtGameTick(time);
    }

    public virtual void DropBomb()
    {
      this.currentLocation.netAudio.StopPlaying("fuse");
      if (this.lastAttacker.Value == null)
        return;
      Farmer farmer = this.lastAttacker.Value;
      int num1 = Game1.random.Next();
      this.currentLocation.playSound("thudStep");
      Vector2 tileLocation = this.getTileLocation();
      float y = this.Position.Y;
      float num2 = 2.4f;
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.timeUntilExplode >= 0.0)
      {
        num2 = (float) (NetFieldBase<float, NetFloat>) this.timeUntilExplode;
        this.currentLocation.netAudio.StartPlaying("fuse");
      }
      int numberOfLoops = Math.Max(1, (int) ((double) num2 * 1000.0 / 100.0));
      Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("Characters\\Monsters\\Hot Head", new Rectangle(16, 64, 16, 16), 25f, 3, numberOfLoops, tileLocation * 64f, false, Game1.random.NextDouble() < 0.5)
      {
        shakeIntensity = 0.5f,
        shakeIntensityChange = 1f / 500f,
        extraInfoForEndBehavior = num1,
        endFunction = new TemporaryAnimatedSprite.endBehavior(this.currentLocation.removeTemporarySpritesWithID),
        bombRadius = 2,
        bombDamage = this.DamageToFarmer,
        Parent = this.currentLocation,
        scale = 4f,
        owner = farmer
      });
      Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, tileLocation * 64f, true, false, (float) (((double) y + 7.0) / 10000.0), 0.0f, Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
      {
        id = (float) num1
      });
      Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, tileLocation * 64f, true, false, (float) (((double) y + 7.0) / 10000.0), 0.0f, Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
      {
        delayBeforeAnimationStart = 100,
        id = (float) num1
      });
      Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279, 3, 4), 53f, 5, 9, tileLocation * 64f, true, false, (float) (((double) y + 7.0) / 10000.0), 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f)
      {
        delayBeforeAnimationStart = 200,
        id = (float) num1
      });
    }

    protected override void sharedDeathAnimation()
    {
      base.sharedDeathAnimation();
      this.DropBomb();
    }

    public override void draw(SpriteBatch b)
    {
      if (this.angry.Value)
      {
        if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128))
          return;
        Rectangle sourceRect = this.Sprite.SourceRect;
        sourceRect.Y += 80;
        b.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 42f + this.yOffset), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), (float) (3.5 + (double) (float) (NetFieldBase<float, NetFloat>) this.scale + (double) this.yOffset / 30.0), SpriteEffects.None, (float) (this.getStandingY() - 1) / 10000f);
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (48 + this.yJumpOffset)), new Rectangle?(sourceRect), (Color) (NetFieldBase<Color, NetColor>) this.c, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));
      }
      else
        base.draw(b);
    }
  }
}
