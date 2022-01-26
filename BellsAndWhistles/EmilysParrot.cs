// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.EmilysParrot
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using System;

namespace StardewValley.BellsAndWhistles
{
  public class EmilysParrot : TemporaryAnimatedSprite
  {
    public const int flappingPhase = 1;
    public const int hoppingPhase = 0;
    public const int lookingSidewaysPhase = 2;
    public const int nappingPhase = 3;
    public const int headBobbingPhase = 4;
    private int currentFrame;
    private int currentFrameTimer;
    private int currentPhaseTimer;
    private int currentPhase;
    private int shakeTimer;

    public EmilysParrot(Vector2 location)
    {
      this.texture = Game1.mouseCursors;
      this.sourceRect = new Rectangle(92, 148, 9, 16);
      this.sourceRectStartingPos = new Vector2(92f, 149f);
      this.position = location;
      this.initialPosition = this.position;
      this.scale = 4f;
      this.id = 5858585f;
    }

    public void doAction()
    {
      Game1.playSound("parrot");
      this.shakeTimer = 800;
    }

    public override bool update(GameTime time)
    {
      this.currentPhaseTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.currentPhaseTimer <= 0)
      {
        this.currentPhase = Game1.random.Next(5);
        this.currentPhaseTimer = Game1.random.Next(4000, 16000);
        if (this.currentPhase == 1)
        {
          this.currentPhaseTimer /= 2;
          this.updateFlappingPhase();
        }
        else
          this.position = this.initialPosition;
      }
      TimeSpan elapsedGameTime;
      if (this.shakeTimer > 0)
      {
        this.shakeIntensity = 1f;
        int shakeTimer = this.shakeTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.shakeTimer = shakeTimer - milliseconds;
      }
      else
        this.shakeIntensity = 0.0f;
      int currentFrameTimer = this.currentFrameTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds1 = elapsedGameTime.Milliseconds;
      this.currentFrameTimer = currentFrameTimer - milliseconds1;
      if (this.currentFrameTimer <= 0)
      {
        switch (this.currentPhase)
        {
          case 0:
            if (this.currentFrame == 7)
            {
              this.currentFrame = 0;
              this.currentFrameTimer = 600;
              break;
            }
            if (Game1.random.NextDouble() < 0.5)
            {
              this.currentFrame = 7;
              this.currentFrameTimer = 300;
              break;
            }
            break;
          case 1:
            this.updateFlappingPhase();
            this.currentFrameTimer = 0;
            break;
          case 2:
            this.currentFrame = Game1.random.Next(3, 5);
            this.currentFrameTimer = 1000;
            break;
          case 3:
            this.currentFrame = this.currentFrame != 5 ? 5 : 6;
            this.currentFrameTimer = 1000;
            break;
          case 4:
            this.currentFrame = this.currentFrame != 1 || Game1.random.NextDouble() >= 0.1 ? (this.currentFrame != 2 ? Game1.random.Next(2) : 1) : 2;
            this.currentFrameTimer = 500;
            break;
        }
      }
      if (this.currentPhase == 1 && this.currentFrame != 0)
      {
        this.sourceRect.X = 38 + this.currentFrame * 13;
        this.sourceRect.Width = 13;
      }
      else
      {
        this.sourceRect.X = 92 + this.currentFrame * 9;
        this.sourceRect.Width = 9;
      }
      return false;
    }

    private void updateFlappingPhase()
    {
      this.currentFrame = 6 - this.currentPhaseTimer % 1000 / 166;
      this.currentFrame = 3 - Math.Abs(this.currentFrame - 3);
      this.position.Y = this.initialPosition.Y - (float) (4 * (3 - this.currentFrame));
      if (this.currentFrame == 0)
        this.position.X = this.initialPosition.X;
      else
        this.position.X = this.initialPosition.X - 8f;
    }
  }
}
