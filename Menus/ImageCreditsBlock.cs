// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ImageCreditsBlock
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley.Menus
{
  internal class ImageCreditsBlock : ICreditsBlock
  {
    private ClickableTextureComponent clickableComponent;
    private int animationFrames;

    public ImageCreditsBlock(
      Texture2D texture,
      Rectangle sourceRect,
      int pixelZoom,
      int animationFrames)
    {
      this.animationFrames = animationFrames;
      this.clickableComponent = new ClickableTextureComponent(new Rectangle(0, 0, sourceRect.Width * pixelZoom, sourceRect.Height * pixelZoom), texture, sourceRect, (float) pixelZoom);
    }

    public override void draw(int topLeftX, int topLeftY, int widthToOccupy, SpriteBatch b) => b.Draw(this.clickableComponent.texture, new Rectangle(topLeftX + widthToOccupy / 2 - this.clickableComponent.bounds.Width / 2, topLeftY, this.clickableComponent.bounds.Width, this.clickableComponent.bounds.Height), new Rectangle?(new Rectangle(this.clickableComponent.sourceRect.X + this.clickableComponent.sourceRect.Width * (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0 / (double) (600 / this.animationFrames)), this.clickableComponent.sourceRect.Y, this.clickableComponent.sourceRect.Width, this.clickableComponent.sourceRect.Height)), Color.White);

    public override int getHeight(int maxWidth) => this.clickableComponent.bounds.Height;
  }
}
