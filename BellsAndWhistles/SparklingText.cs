// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.SparklingText
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class SparklingText
  {
    public static int maxDistanceForSparkle = 32;
    private SpriteFont font;
    private Color color;
    private Color sparkleColor;
    private bool rainbow;
    private int millisecondsDuration;
    private int amplitude;
    private int period;
    private int colorCycle;
    public string text;
    private float[] individualCharacterOffsets;
    public float offsetDecay = 1f;
    public float alpha = 1f;
    public float textWidth;
    public float drawnTextWidth;
    public float layerDepth = 1f;
    private double sparkleFrequency;
    private List<TemporaryAnimatedSprite> sparkles;
    private List<Vector2> sparkleTrash;
    private Rectangle boundingBox;

    public SparklingText(
      SpriteFont font,
      string text,
      Color color,
      Color sparkleColor,
      bool rainbow = false,
      double sparkleFrequency = 0.1,
      int millisecondsDuration = 2500,
      int amplitude = -1,
      int speed = 500,
      float depth = 1f)
    {
      if (amplitude == -1)
        amplitude = 64;
      SparklingText.maxDistanceForSparkle = 32;
      this.font = font;
      this.color = color;
      this.sparkleColor = sparkleColor;
      this.text = text;
      this.rainbow = rainbow;
      if (rainbow)
        color = Color.Yellow;
      this.sparkleFrequency = sparkleFrequency;
      this.millisecondsDuration = millisecondsDuration;
      this.individualCharacterOffsets = new float[text.Length];
      this.amplitude = amplitude;
      this.period = speed;
      this.sparkles = new List<TemporaryAnimatedSprite>();
      this.boundingBox = new Rectangle(-SparklingText.maxDistanceForSparkle, -SparklingText.maxDistanceForSparkle, (int) font.MeasureString(text).X + SparklingText.maxDistanceForSparkle * 2, (int) font.MeasureString(text).Y + SparklingText.maxDistanceForSparkle * 2);
      this.sparkleTrash = new List<Vector2>();
      this.textWidth = font.MeasureString(text).X;
      this.layerDepth = depth;
      int num = 0;
      for (int index = 0; index < text.Length; ++index)
        num += (int) font.MeasureString(text[index].ToString() ?? "").X;
      this.drawnTextWidth = (float) num;
    }

    public bool update(GameTime time)
    {
      this.millisecondsDuration -= time.ElapsedGameTime.Milliseconds;
      this.offsetDecay -= 1f / 1000f;
      this.amplitude = (int) ((double) this.amplitude * (double) this.offsetDecay);
      if (this.millisecondsDuration <= 500)
        this.alpha = (float) this.millisecondsDuration / 500f;
      for (int index = 0; index < this.individualCharacterOffsets.Length; ++index)
        this.individualCharacterOffsets[index] = (float) (this.amplitude / 2) * (float) Math.Sin(2.0 * Math.PI / (double) this.period * (double) (this.millisecondsDuration - index * 100));
      if (this.millisecondsDuration > 500 && Game1.random.NextDouble() < this.sparkleFrequency)
        this.sparkles.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 704, 64, 64), (float) (Game1.random.Next(100, 600) / 6), 6, 0, new Vector2((float) Game1.random.Next(this.boundingBox.X, this.boundingBox.Right), (float) Game1.random.Next(this.boundingBox.Y, this.boundingBox.Bottom)), false, false, this.layerDepth, 0.0f, this.rainbow ? this.color : this.sparkleColor, 1f, 0.0f, 0.0f, 0.0f));
      for (int index = this.sparkles.Count - 1; index >= 0; --index)
      {
        if (this.sparkles[index].update(time))
          this.sparkles.RemoveAt(index);
      }
      if (this.rainbow)
        this.incrementRainbowColors();
      return this.millisecondsDuration <= 0;
    }

    private void incrementRainbowColors()
    {
      if (this.colorCycle != 0)
        return;
      if ((this.color.G += (byte) 4) >= byte.MaxValue)
      {
        this.colorCycle = 1;
      }
      else
      {
        if (this.colorCycle != 1)
          return;
        if ((this.color.R -= (byte) 4) <= (byte) 0)
        {
          this.colorCycle = 2;
        }
        else
        {
          if (this.colorCycle != 2)
            return;
          if ((this.color.B += (byte) 4) >= byte.MaxValue)
          {
            this.colorCycle = 3;
          }
          else
          {
            if (this.colorCycle != 3)
              return;
            if ((this.color.G -= (byte) 4) <= (byte) 0)
            {
              this.colorCycle = 4;
            }
            else
            {
              if (this.colorCycle != 4)
                return;
              if (++this.color.R >= byte.MaxValue)
              {
                this.colorCycle = 5;
              }
              else
              {
                if (this.colorCycle != 5 || (this.color.B -= (byte) 4) > (byte) 0)
                  return;
                this.colorCycle = 0;
              }
            }
          }
        }
      }
    }

    private static Color getRainbowColorFromIndex(int index)
    {
      switch (index % 8)
      {
        case 0:
          return Color.Red;
        case 1:
          return Color.Orange;
        case 2:
          return Color.Yellow;
        case 3:
          return Color.Chartreuse;
        case 4:
          return Color.Green;
        case 5:
          return Color.Cyan;
        case 6:
          return Color.Blue;
        case 7:
          return Color.Violet;
        default:
          return Color.White;
      }
    }

    public void draw(SpriteBatch b, Vector2 onScreenPosition)
    {
      int x1 = 0;
      for (int index = 0; index < this.text.Length; ++index)
      {
        SpriteBatch spriteBatch1 = b;
        SpriteFont font1 = this.font;
        char ch = this.text[index];
        string text1 = ch.ToString() ?? "";
        Vector2 position1 = onScreenPosition + new Vector2((float) (x1 - 2), this.individualCharacterOffsets[index]);
        Color black1 = Color.Black;
        Vector2 zero1 = Vector2.Zero;
        spriteBatch1.DrawString(font1, text1, position1, black1, 0.0f, zero1, 1f, SpriteEffects.None, 0.99f);
        SpriteBatch spriteBatch2 = b;
        SpriteFont font2 = this.font;
        ch = this.text[index];
        string text2 = ch.ToString() ?? "";
        Vector2 position2 = onScreenPosition + new Vector2((float) (x1 + 2), this.individualCharacterOffsets[index]);
        Color black2 = Color.Black;
        Vector2 zero2 = Vector2.Zero;
        spriteBatch2.DrawString(font2, text2, position2, black2, 0.0f, zero2, 1f, SpriteEffects.None, 0.991f);
        SpriteBatch spriteBatch3 = b;
        SpriteFont font3 = this.font;
        ch = this.text[index];
        string text3 = ch.ToString() ?? "";
        Vector2 position3 = onScreenPosition + new Vector2((float) x1, this.individualCharacterOffsets[index] - 2f);
        Color black3 = Color.Black;
        Vector2 zero3 = Vector2.Zero;
        spriteBatch3.DrawString(font3, text3, position3, black3, 0.0f, zero3, 1f, SpriteEffects.None, 0.992f);
        SpriteBatch spriteBatch4 = b;
        SpriteFont font4 = this.font;
        ch = this.text[index];
        string text4 = ch.ToString() ?? "";
        Vector2 position4 = onScreenPosition + new Vector2((float) x1, this.individualCharacterOffsets[index] + 2f);
        Color black4 = Color.Black;
        Vector2 zero4 = Vector2.Zero;
        spriteBatch4.DrawString(font4, text4, position4, black4, 0.0f, zero4, 1f, SpriteEffects.None, 0.993f);
        SpriteBatch spriteBatch5 = b;
        SpriteFont font5 = this.font;
        ch = this.text[index];
        string text5 = ch.ToString() ?? "";
        Vector2 position5 = onScreenPosition + new Vector2((float) x1, this.individualCharacterOffsets[index]);
        Color color = this.rainbow ? SparklingText.getRainbowColorFromIndex(index) : this.color * this.alpha;
        Vector2 zero5 = Vector2.Zero;
        double layerDepth = (double) this.layerDepth;
        spriteBatch5.DrawString(font5, text5, position5, color, 0.0f, zero5, 1f, SpriteEffects.None, (float) layerDepth);
        int num = x1;
        SpriteFont font6 = this.font;
        ch = this.text[index];
        string text6 = ch.ToString() ?? "";
        int x2 = (int) font6.MeasureString(text6).X;
        x1 = num + x2;
      }
      this.font.MeasureString(this.text);
      foreach (TemporaryAnimatedSprite sparkle in this.sparkles)
      {
        sparkle.Position += onScreenPosition;
        sparkle.draw(b, true);
        sparkle.Position -= onScreenPosition;
      }
    }
  }
}
