// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.LocalCoopJoinMenu
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace StardewValley.Menus
{
  public class LocalCoopJoinMenu : IClickableMenu
  {
    public override void update(GameTime time)
    {
      base.update(time);
      int simultaneousPlayers = GameRunner.instance.GetMaxSimultaneousPlayers();
      if (GameRunner.instance.gameInstances.Count >= simultaneousPlayers)
        return;
      for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four && GameRunner.instance.gameInstances.Count < simultaneousPlayers; ++playerIndex)
      {
        if (GameRunner.instance.IsStartDown(playerIndex))
        {
          bool flag = false;
          foreach (Game1 gameInstance in GameRunner.instance.gameInstances)
          {
            if (gameInstance.instancePlayerOneIndex == playerIndex && !gameInstance.IsMainInstance)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            if (playerIndex == PlayerIndex.One)
              GameRunner.instance.gameInstances[0].instancePlayerOneIndex = ~PlayerIndex.One;
            GameRunner.instance.AddGameInstance(playerIndex);
          }
        }
      }
    }

    public override void receiveGamePadButton(Buttons b)
    {
      if (b == Buttons.B)
        this.exitThisMenu();
      else
        base.receiveGamePadButton(b);
    }

    public override void receiveKeyPress(Keys key)
    {
      if (key != Keys.Escape)
        return;
      this.exitThisMenu();
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
      double x1 = (double) (viewport2.Width / 2);
      viewport2 = Game1.graphics.GraphicsDevice.Viewport;
      double y = (double) (viewport2.Height / 2);
      local = new Vector2((float) x1, (float) y);
      SpriteFont smallFont = Game1.smallFont;
      string[] strArray = Game1.content.LoadString("Strings\\UI:LocalJoinPrompt").Split('*');
      Vector2 vector2_2 = smallFont.MeasureString(strArray[0]);
      vector2_2.X += 32f;
      int x2 = (int) vector2_2.X;
      vector2_2.X += smallFont.MeasureString(strArray[1]).X;
      vector2_2.Y = Math.Max(vector2_2.Y, smallFont.MeasureString(strArray[1]).Y);
      Vector2 position1 = vector2_1 - vector2_2 / 2f;
      int num = 32;
      int height2 = Math.Max((int) vector2_2.Y, 32);
      Game1.DrawBox((int) position1.X - num, (int) position1.Y, (int) vector2_2.X + num * 2, height2);
      b.DrawString(smallFont, strArray[0], position1 + new Vector2(4f, 4f), Game1.textShadowColor);
      b.DrawString(smallFont, strArray[1], position1 + new Vector2((float) x2, 0.0f) + new Vector2(4f, 4f), Game1.textShadowColor);
      Vector2 position2 = position1 + new Vector2((float) (x2 - 16), 0.0f);
      position2.Y += smallFont.MeasureString("XX").X / 2f;
      b.Draw(Game1.controllerMaps, position2 + new Vector2(4f, 4f), new Rectangle?(Utility.controllerMapSourceRect(new Rectangle(653, 260, 28, 28))), Color.Black * 0.25f, 0.0f, new Vector2(14f, 14f), 1f, SpriteEffects.None, 0.99f);
      b.Draw(Game1.controllerMaps, position2, new Rectangle?(Utility.controllerMapSourceRect(new Rectangle(653, 260, 28, 28))), Color.White, 0.0f, new Vector2(14f, 14f), 1f, SpriteEffects.None, 0.99f);
      b.DrawString(smallFont, strArray[0], position1, Game1.textColor);
      b.DrawString(smallFont, strArray[1], position1 + new Vector2((float) x2, 0.0f), Game1.textColor);
      string text = Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel");
      position1.Y -= vector2_2.Y / 2f;
      position1.Y += (float) height2;
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
        position1.Y += 48f;
      else
        position1.Y += 32f;
      position1.X += vector2_2.X + (float) num;
      position1.X -= smallFont.MeasureString(text).X;
      b.DrawString(smallFont, text, position1, Color.White);
      position1.X -= smallFont.MeasureString("XX").X;
      position1 += smallFont.MeasureString("X") / 2f;
      b.Draw(Game1.controllerMaps, position1, new Rectangle?(Utility.controllerMapSourceRect(new Rectangle(569, 260, 28, 28))), Color.White, 0.0f, new Vector2(14f, 14f), 1f, SpriteEffects.None, 0.99f);
      if (Game1.options.SnappyMenus)
        return;
      this.drawMouse(b);
    }
  }
}
