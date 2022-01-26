// Decompiled with JetBrains decompiler
// Type: StardewValley.LivestockSprite
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;

namespace StardewValley
{
  public class LivestockSprite : AnimatedSprite
  {
    public LivestockSprite(string texture, int currentFrame)
      : base(texture, 0, 64, 96)
    {
    }

    public override void faceDirection(int direction)
    {
      switch (direction)
      {
        case 0:
          this.currentFrame = this.Texture.Width / 64 * 2 + this.currentFrame % (this.Texture.Width / 64);
          break;
        case 1:
          this.currentFrame = this.Texture.Width / 128 + this.currentFrame % (this.Texture.Width / 128);
          break;
        case 2:
          this.currentFrame %= this.Texture.Width / 64;
          break;
        case 3:
          this.currentFrame = this.Texture.Width / 128 * 3 + this.currentFrame % (this.Texture.Width / 128);
          break;
      }
      this.UpdateSourceRect();
    }

    public override void UpdateSourceRect()
    {
      switch (this.currentFrame)
      {
        case 0:
        case 1:
        case 2:
        case 3:
          this.SourceRect = new Rectangle(this.currentFrame * 64, 0, 64, 96);
          break;
        case 4:
        case 5:
        case 6:
        case 7:
          this.SourceRect = new Rectangle(this.currentFrame % 4 * 64 * 2, 96, 128, 96);
          break;
        case 8:
        case 9:
        case 10:
        case 11:
          this.SourceRect = new Rectangle(this.currentFrame % 4 * 64, 192, 64, 96);
          break;
        case 12:
        case 13:
        case 14:
        case 15:
          this.SourceRect = new Rectangle(this.currentFrame % 4 * 64 * 2, 288, 128, 96);
          break;
        case 24:
        case 25:
        case 26:
        case 27:
          this.SourceRect = new Rectangle((this.currentFrame - 20) * 64, 192, 64, 96);
          break;
      }
    }
  }
}
