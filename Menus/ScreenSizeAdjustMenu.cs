// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ScreenSizeAdjustMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
  public class ScreenSizeAdjustMenu : IClickableMenu
  {
    public ScreenSizeAdjustMenu() => Game1.shouldDrawSafeAreaBounds = true;

    public override void update(GameTime time) => base.update(time);

    public override void receiveGamePadButton(Buttons b)
    {
      if (b == Buttons.B)
        this.exitThisMenu();
      else
        base.receiveGamePadButton(b);
    }

    public override void receiveKeyPress(Keys key)
    {
      if (((IEnumerable<InputButton>) Game1.options.moveUpButton).Contains<InputButton>(new InputButton(key)))
        Game1.AdjustScreenScale(-0.01f);
      else if (((IEnumerable<InputButton>) Game1.options.moveDownButton).Contains<InputButton>(new InputButton(key)))
        Game1.AdjustScreenScale(0.01f);
      if (key != Keys.Escape)
        return;
      this.exitThisMenu();
    }

    protected override void cleanupBeforeExit()
    {
      Game1.shouldDrawSafeAreaBounds = false;
      base.cleanupBeforeExit();
    }

    public override void draw(SpriteBatch b)
    {
      SpriteBatch spriteBatch = b;
      Texture2D staminaRect = Game1.staminaRect;
      Viewport viewport1 = Game1.graphics.GraphicsDevice.Viewport;
      int width = viewport1.Width;
      viewport1 = Game1.graphics.GraphicsDevice.Viewport;
      int height1 = viewport1.Height;
      Rectangle destinationRectangle = new Rectangle(0, 0, width, height1);
      Color color = Color.Black * 0.75f;
      spriteBatch.Draw(staminaRect, destinationRectangle, color);
      Vector2 vector2_1;
      ref Vector2 local = ref vector2_1;
      Viewport viewport2 = Game1.graphics.GraphicsDevice.Viewport;
      double x = (double) (viewport2.Width / 2);
      viewport2 = Game1.graphics.GraphicsDevice.Viewport;
      double y = (double) (viewport2.Height / 2);
      local = new Vector2((float) x, (float) y);
      SpriteFont smallFont = Game1.smallFont;
      string text1 = Game1.content.LoadString("Strings\\UI:DisplayAdjustmentText");
      Vector2 vector2_2 = smallFont.MeasureString(text1);
      vector2_2.X += 32f;
      Vector2 position = vector2_1 - vector2_2 / 2f;
      int num = 32;
      int height2 = Math.Max((int) vector2_2.Y, 32);
      Game1.DrawBox((int) position.X - num, (int) position.Y, (int) vector2_2.X + num * 2, height2);
      b.DrawString(smallFont, text1, position + new Vector2(4f, 4f), Game1.textShadowColor);
      b.DrawString(smallFont, text1, position, Game1.textColor);
      string text2 = Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel");
      position.Y -= vector2_2.Y / 2f;
      position.Y += (float) height2;
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
        position.Y += 48f;
      else
        position.Y += 32f;
      position.X += vector2_2.X + (float) num;
      position.X -= smallFont.MeasureString(text2).X;
      b.DrawString(smallFont, text2, position, Color.White);
      position.X -= smallFont.MeasureString("XX").X;
      position += smallFont.MeasureString("X") / 2f;
      b.Draw(Game1.controllerMaps, position, new Rectangle?(Utility.controllerMapSourceRect(new Rectangle(569, 260, 28, 28))), Color.White, 0.0f, new Vector2(14f, 14f), 1f, SpriteEffects.None, 0.99f);
    }
  }
}
