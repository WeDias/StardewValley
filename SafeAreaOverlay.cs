// Decompiled with JetBrains decompiler
// Type: StardewValley.SafeAreaOverlay
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley
{
  /// <summary>
  /// Reusable component makes it easy to check whether your important
  /// graphics are positioned inside the title safe area, by superimposing
  /// a red border that marks the edges of the safe region.
  /// </summary>
  public class SafeAreaOverlay : DrawableGameComponent
  {
    private SpriteBatch spriteBatch;
    private Texture2D dummyTexture;

    /// <summary>Constructor.</summary>
    public SafeAreaOverlay(Game game)
      : base(game)
    {
      this.DrawOrder = 1000;
    }

    /// <summary>
    /// Creates the graphics resources needed to draw the overlay.
    /// </summary>
    protected override void LoadContent()
    {
      this.spriteBatch = new SpriteBatch(Game1.graphics.GraphicsDevice);
      this.dummyTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
      this.dummyTexture.SetData<Color>(new Color[1]
      {
        Color.White
      });
    }

    /// <summary>Draws the title safe area.</summary>
    public override void Draw(GameTime gameTime)
    {
      Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
      Rectangle titleSafeArea = viewport.GetTitleSafeArea();
      int num1 = viewport.X + viewport.Width;
      int num2 = viewport.Y + viewport.Height;
      Rectangle destinationRectangle1 = new Rectangle(viewport.X, viewport.Y, titleSafeArea.X - viewport.X, viewport.Height);
      Rectangle destinationRectangle2 = new Rectangle(titleSafeArea.Right, viewport.Y, num1 - titleSafeArea.Right, viewport.Height);
      Rectangle destinationRectangle3 = new Rectangle(titleSafeArea.Left, viewport.Y, titleSafeArea.Width, titleSafeArea.Top - viewport.Y);
      Rectangle destinationRectangle4 = new Rectangle(titleSafeArea.Left, titleSafeArea.Bottom, titleSafeArea.Width, num2 - titleSafeArea.Bottom);
      Color red = Color.Red;
      this.spriteBatch.Begin();
      this.spriteBatch.Draw(this.dummyTexture, destinationRectangle1, red);
      this.spriteBatch.Draw(this.dummyTexture, destinationRectangle2, red);
      this.spriteBatch.Draw(this.dummyTexture, destinationRectangle3, red);
      this.spriteBatch.Draw(this.dummyTexture, destinationRectangle4, red);
      this.spriteBatch.End();
    }
  }
}
