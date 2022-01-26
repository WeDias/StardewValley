// Decompiled with JetBrains decompiler
// Type: StardewValley.NumberSprite
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StardewValley
{
  public class NumberSprite
  {
    public const int textureX = 512;
    public const int textureY = 128;
    public const int digitWidth = 8;
    public const int digitHeight = 8;
    public const int groupWidth = 48;

    public static void draw(
      int number,
      SpriteBatch b,
      Vector2 position,
      Color c,
      float scale,
      float layerDepth,
      float alpha,
      int secondDigitOffset,
      int spaceBetweenDigits = 0)
    {
      int num1 = 1;
      secondDigitOffset = Math.Min(0, secondDigitOffset);
      do
      {
        int num2 = number % 10;
        number /= 10;
        int x = 512 + num2 * 8 % 48;
        int y = 128 + num2 * 8 / 48 * 8;
        b.Draw(Game1.mouseCursors, position + new Vector2(0.0f, num1 == 2 ? (float) secondDigitOffset : 0.0f), new Rectangle?(new Rectangle(x, y, 8, 8)), c * alpha, 0.0f, new Vector2(4f, 4f), 4f * scale, SpriteEffects.None, layerDepth);
        position.X -= (float) (8.0 * (double) scale * 4.0 - 4.0) - (float) spaceBetweenDigits;
        ++num1;
      }
      while (number > 0);
    }

    public static int getHeight() => 8;

    public static int getWidth(string number) => NumberSprite.getWidth(Convert.ToInt32(number));

    public static int getWidth(int number)
    {
      int width = 8;
      number /= 10;
      while (number != 0)
      {
        number /= 10;
        width += 8;
      }
      return width;
    }

    public static int numberOfDigits(int number)
    {
      int num = 1;
      number /= 10;
      while (number != 0)
      {
        number /= 10;
        ++num;
      }
      return num;
    }
  }
}
