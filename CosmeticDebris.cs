// Decompiled with JetBrains decompiler
// Type: StardewValley.CosmeticDebris
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace StardewValley
{
  public class CosmeticDebris : TemporaryAnimatedSprite
  {
    public const float gravity = 0.3f;
    public const float bounciness = 0.45f;
    private new Vector2 position;
    private new float rotation;
    private float rotationSpeed;
    private float xVelocity;
    private float yVelocity;
    private new Rectangle sourceRect;
    private int groundYLevel;
    private int disappearTimer;
    private int lightTailLength;
    private int timeToDisappearAfterReachingGround;
    private int id;
    private new Color color;
    private ICue tapSound;
    private LightSource light;
    private Queue<Vector2> lightTail;
    private new Texture2D texture;

    public CosmeticDebris(
      Texture2D texture,
      Vector2 startingPosition,
      float rotationSpeed,
      float xVelocity,
      float yVelocity,
      int groundYLevel,
      Rectangle sourceRect,
      Color color,
      ICue tapSound,
      LightSource light,
      int lightTailLength,
      int disappearTime)
    {
      this.timeToDisappearAfterReachingGround = disappearTime;
      this.disappearTimer = this.timeToDisappearAfterReachingGround;
      this.texture = texture;
      this.position = startingPosition;
      this.rotationSpeed = rotationSpeed;
      this.xVelocity = xVelocity;
      this.yVelocity = yVelocity;
      this.sourceRect = sourceRect;
      this.groundYLevel = groundYLevel;
      this.color = color;
      this.tapSound = tapSound;
      this.light = light;
      this.id = Game1.random.Next();
      if (light != null)
      {
        light.Identifier = this.id;
        Game1.currentLightSources.Add(light);
      }
      if (lightTailLength <= 0)
        return;
      this.lightTail = new Queue<Vector2>();
      this.lightTailLength = lightTailLength;
    }

    public override bool update(GameTime time)
    {
      if (this.light != null)
        Utility.repositionLightSource(this.id, this.position);
      this.yVelocity += 0.3f;
      this.position += new Vector2(this.xVelocity, this.yVelocity);
      this.rotation += this.rotationSpeed;
      if ((double) this.position.Y >= (double) this.groundYLevel)
      {
        this.position.Y = (float) (this.groundYLevel - 1);
        this.yVelocity = -this.yVelocity;
        this.yVelocity *= 0.45f;
        this.xVelocity *= 0.45f;
        this.rotationSpeed *= 0.225f;
        if (Game1.soundBank != null && !this.tapSound.IsPlaying)
        {
          this.tapSound = Game1.soundBank.GetCue(this.tapSound.Name);
          this.tapSound.Play();
        }
        --this.disappearTimer;
      }
      if (this.disappearTimer < this.timeToDisappearAfterReachingGround)
      {
        this.disappearTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.disappearTimer <= 0)
        {
          Utility.removeLightSource(this.id);
          return true;
        }
      }
      return false;
    }

    public override void draw(
      SpriteBatch spriteBatch,
      bool localPosition = false,
      int xOffset = 0,
      int yOffset = 0,
      float extraAlpha = 1f)
    {
      spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, this.position), new Rectangle?(this.sourceRect), this.color, this.rotation, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float) (this.groundYLevel + 1) / 10000f);
    }
  }
}
