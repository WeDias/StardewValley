// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.ShadowBrute
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;

namespace StardewValley.Monsters
{
  public class ShadowBrute : Monster
  {
    public ShadowBrute()
    {
    }

    public ShadowBrute(Vector2 position)
      : base("Shadow Brute", position)
    {
      this.Sprite.SpriteHeight = 32;
      this.Sprite.UpdateSourceRect();
    }

    public override void reloadSprite()
    {
      this.Sprite = new AnimatedSprite("Characters\\Monsters\\Shadow Brute");
      this.Sprite.SpriteHeight = 32;
      this.Sprite.UpdateSourceRect();
    }

    public override int takeDamage(
      int damage,
      int xTrajectory,
      int yTrajectory,
      bool isBomb,
      double addedPrecision,
      Farmer who)
    {
      this.currentLocation.playSound("shadowHit");
      return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
    }

    protected override void localDeathAnimation()
    {
      Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(45, this.Position, Color.White, 10), this.currentLocation);
      for (int index = 1; index < 3; ++index)
      {
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(0.0f, 1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(0.0f, -1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(1f, 0.0f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(-1f, 0.0f) * 64f * (float) index, Color.Gray * 0.75f, 10)
        {
          delayBeforeAnimationStart = index * 159
        });
      }
      this.currentLocation.localSound("shadowDie");
    }

    protected override void sharedDeathAnimation()
    {
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, 16, 5), 16, this.getStandingX(), this.getStandingY() - 32, 1, this.getStandingY() / 64, Color.White, 4f);
      Game1.createRadialDebris(this.currentLocation, (string) (NetFieldBase<string, NetString>) this.Sprite.textureName, new Rectangle(this.Sprite.SourceRect.X + 2, this.Sprite.SourceRect.Y + 5, 16, 5), 10, this.getStandingX(), this.getStandingY() - 32, 1, this.getStandingY() / 64, Color.White, 4f);
    }

    protected override void updateMonsterSlaveAnimation(GameTime time)
    {
      if (!this.isMoving())
        return;
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
  }
}
