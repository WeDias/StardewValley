// Decompiled with JetBrains decompiler
// Type: StardewValley.ColorChanger
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StardewValley
{
  internal class ColorChanger
  {
    [InstancedStatic]
    private static Color[] _buffer;

    public static Texture2D swapColor(
      Texture2D texture,
      int targetColorIndex,
      int red,
      int green,
      int blue)
    {
      return ColorChanger.swapColor(texture, targetColorIndex, red, green, blue, 0, texture.Width * texture.Height);
    }

    private static Color[] getBuffer(int len)
    {
      if (ColorChanger._buffer == null || ColorChanger._buffer.Length < len)
        ColorChanger._buffer = new Color[len];
      return ColorChanger._buffer;
    }

    public static unsafe Texture2D swapColor(
      Texture2D texture,
      int targetColorIndex1,
      int r1,
      int g1,
      int b1,
      int startPixelIndex,
      int endPixelIndex)
    {
      r1 = Math.Min(Math.Max(1, r1), (int) byte.MaxValue);
      g1 = Math.Min(Math.Max(1, g1), (int) byte.MaxValue);
      b1 = Math.Min(Math.Max(1, b1), (int) byte.MaxValue);
      uint packedValue1 = new Color(r1, g1, b1).PackedValue;
      int num = texture.Width * texture.Height;
      Color[] buffer = ColorChanger.getBuffer(num);
      texture.GetData<Color>(buffer, 0, num);
      uint packedValue2 = buffer[targetColorIndex1].PackedValue;
      fixed (Color* colorPtr1 = buffer)
      {
        Color* colorPtr2 = colorPtr1 + startPixelIndex;
        Color* colorPtr3 = colorPtr1 + endPixelIndex;
        for (Color* colorPtr4 = colorPtr2; colorPtr4 <= colorPtr3; ++colorPtr4)
        {
          if ((int) colorPtr4->PackedValue == (int) packedValue2)
            colorPtr4->PackedValue = packedValue1;
        }
      }
      texture.SetData<Color>(buffer, 0, num);
      return texture;
    }

    public static unsafe void swapColors(
      Texture2D texture,
      int targetColorIndex1,
      byte r1,
      byte g1,
      byte b1,
      int targetColorIndex2,
      byte r2,
      byte g2,
      byte b2)
    {
      r1 = Math.Min(Math.Max((byte) 1, r1), byte.MaxValue);
      g1 = Math.Min(Math.Max((byte) 1, g1), byte.MaxValue);
      b1 = Math.Min(Math.Max((byte) 1, b1), byte.MaxValue);
      r2 = Math.Min(Math.Max((byte) 1, r2), byte.MaxValue);
      g2 = Math.Min(Math.Max((byte) 1, g2), byte.MaxValue);
      b2 = Math.Min(Math.Max((byte) 1, b2), byte.MaxValue);
      Color color1 = new Color((int) r1, (int) g1, (int) b1);
      Color color2 = new Color((int) r2, (int) g2, (int) b2);
      uint packedValue1 = color1.PackedValue;
      uint packedValue2 = color2.PackedValue;
      int num1 = texture.Width * texture.Height;
      Color[] buffer = ColorChanger.getBuffer(num1);
      texture.GetData<Color>(buffer, 0, num1);
      Color color3 = buffer[targetColorIndex1];
      Color color4 = buffer[targetColorIndex2];
      uint packedValue3 = color3.PackedValue;
      uint packedValue4 = color4.PackedValue;
      int num2 = 0;
      int num3 = num1;
      fixed (Color* colorPtr1 = buffer)
      {
        Color* colorPtr2 = colorPtr1 + num2;
        Color* colorPtr3 = colorPtr1 + num3;
        for (Color* colorPtr4 = colorPtr2; colorPtr4 <= colorPtr3; ++colorPtr4)
        {
          if ((int) colorPtr4->PackedValue == (int) packedValue3)
            colorPtr4->PackedValue = packedValue1;
          else if ((int) colorPtr4->PackedValue == (int) packedValue4)
            colorPtr4->PackedValue = packedValue2;
        }
      }
      texture.SetData<Color>(buffer, 0, num1);
    }
  }
}
