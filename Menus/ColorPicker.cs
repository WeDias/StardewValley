// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ColorPicker
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StardewValley.Menus
{
  public class ColorPicker
  {
    public const int sliderChunks = 24;
    private int colorIndexSelection;
    private Rectangle bounds;
    public SliderBar hueBar;
    public SliderBar valueBar;
    public SliderBar saturationBar;
    public SliderBar recentSliderBar;
    public string Name;
    public Color LastColor;
    public bool Dirty;

    public ColorPicker(string name, int x, int y)
    {
      this.Name = name;
      this.hueBar = new SliderBar(0, 0, 50);
      this.saturationBar = new SliderBar(0, 20, 50);
      this.valueBar = new SliderBar(0, 40, 50);
      this.bounds = new Rectangle(x, y, SliderBar.defaultWidth, 60);
    }

    public Color getSelectedColor() => this.HsvToRgb((double) this.hueBar.value / 100.0 * 360.0, (double) this.saturationBar.value / 100.0, (double) this.valueBar.value / 100.0);

    public Color click(int x, int y)
    {
      if (this.bounds.Contains(x, y))
      {
        x -= this.bounds.X;
        y -= this.bounds.Y;
        if (this.hueBar.bounds.Contains(x, y))
        {
          this.hueBar.click(x, y);
          this.recentSliderBar = this.hueBar;
        }
        if (this.saturationBar.bounds.Contains(x, y))
        {
          this.recentSliderBar = this.saturationBar;
          this.saturationBar.click(x, y);
        }
        if (this.valueBar.bounds.Contains(x, y))
        {
          this.recentSliderBar = this.valueBar;
          this.valueBar.click(x, y);
        }
      }
      return this.getSelectedColor();
    }

    public void changeHue(int amount)
    {
      this.hueBar.changeValueBy(amount);
      this.recentSliderBar = this.hueBar;
    }

    public void changeSaturation(int amount)
    {
      this.saturationBar.changeValueBy(amount);
      this.recentSliderBar = this.saturationBar;
    }

    public void changeValue(int amount)
    {
      this.valueBar.changeValueBy(amount);
      this.recentSliderBar = this.valueBar;
    }

    public Color clickHeld(int x, int y)
    {
      if (this.recentSliderBar != null)
      {
        x = Math.Max(x, this.bounds.X);
        x = Math.Min(x, this.bounds.Right - 1);
        y = this.recentSliderBar.bounds.Center.Y;
        x -= this.bounds.X;
        if (this.recentSliderBar.Equals((object) this.hueBar))
          this.hueBar.click(x, y);
        if (this.recentSliderBar.Equals((object) this.saturationBar))
          this.saturationBar.click(x, y);
        if (this.recentSliderBar.Equals((object) this.valueBar))
          this.valueBar.click(x, y);
      }
      return this.getSelectedColor();
    }

    public void releaseClick()
    {
      this.hueBar.release(0, 0);
      this.saturationBar.release(0, 0);
      this.valueBar.release(0, 0);
      this.recentSliderBar = (SliderBar) null;
    }

    public void draw(SpriteBatch b)
    {
      for (int index = 0; index < 24; ++index)
      {
        Color rgb = this.HsvToRgb((double) index / 24.0 * 360.0, 0.9, 0.9);
        b.Draw(Game1.staminaRect, new Rectangle(this.bounds.X + this.bounds.Width / 24 * index, this.bounds.Y + this.hueBar.bounds.Center.Y - 2, this.hueBar.bounds.Width / 24, 4), rgb);
      }
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.bounds.X + (int) ((double) this.hueBar.value / 100.0 * (double) this.hueBar.bounds.Width)), (float) (this.bounds.Y + this.hueBar.bounds.Center.Y)), new Rectangle?(new Rectangle(64, 256, 32, 32)), Color.White, 0.0f, new Vector2(16f, 9f), 1f, SpriteEffects.None, 0.86f);
      Utility.drawTextWithShadow(b, this.hueBar.value.ToString() ?? "", Game1.smallFont, new Vector2((float) (this.bounds.X + this.bounds.Width + 8), (float) (this.bounds.Y + this.hueBar.bounds.Y)), Game1.textColor);
      for (int index = 0; index < 24; ++index)
      {
        Color rgb = this.HsvToRgb((double) this.hueBar.value / 100.0 * 360.0, (double) index / 24.0, (double) this.valueBar.value / 100.0);
        b.Draw(Game1.staminaRect, new Rectangle(this.bounds.X + this.bounds.Width / 24 * index, this.bounds.Y + this.saturationBar.bounds.Center.Y - 2, this.saturationBar.bounds.Width / 24, 4), rgb);
      }
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.bounds.X + (int) ((double) this.saturationBar.value / 100.0 * (double) this.saturationBar.bounds.Width)), (float) (this.bounds.Y + this.saturationBar.bounds.Center.Y)), new Rectangle?(new Rectangle(64, 256, 32, 32)), Color.White, 0.0f, new Vector2(16f, 9f), 1f, SpriteEffects.None, 0.87f);
      Utility.drawTextWithShadow(b, this.saturationBar.value.ToString() ?? "", Game1.smallFont, new Vector2((float) (this.bounds.X + this.bounds.Width + 8), (float) (this.bounds.Y + this.saturationBar.bounds.Y)), Game1.textColor);
      for (int index = 0; index < 24; ++index)
      {
        Color rgb = this.HsvToRgb((double) this.hueBar.value / 100.0 * 360.0, (double) this.saturationBar.value / 100.0, (double) index / 24.0);
        b.Draw(Game1.staminaRect, new Rectangle(this.bounds.X + this.bounds.Width / 24 * index, this.bounds.Y + this.valueBar.bounds.Center.Y - 2, this.valueBar.bounds.Width / 24, 4), rgb);
      }
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.bounds.X + (int) ((double) this.valueBar.value / 100.0 * (double) this.valueBar.bounds.Width)), (float) (this.bounds.Y + this.valueBar.bounds.Center.Y)), new Rectangle?(new Rectangle(64, 256, 32, 32)), Color.White, 0.0f, new Vector2(16f, 9f), 1f, SpriteEffects.None, 0.86f);
      Utility.drawTextWithShadow(b, this.valueBar.value.ToString() ?? "", Game1.smallFont, new Vector2((float) (this.bounds.X + this.bounds.Width + 8), (float) (this.bounds.Y + this.valueBar.bounds.Y)), Game1.textColor);
    }

    public bool containsPoint(int x, int y) => this.bounds.Contains(x, y);

    public void setColor(Color color)
    {
      float h;
      float s;
      float v;
      this.RGBtoHSV((float) color.R, (float) color.G, (float) color.B, out h, out s, out v);
      if (float.IsNaN(h))
        h = 0.0f;
      if (float.IsNaN(s))
        s = 0.0f;
      if (float.IsNaN(h))
        h = 0.0f;
      this.hueBar.value = (int) ((double) h / 360.0 * 100.0);
      this.saturationBar.value = (int) ((double) s * 100.0);
      this.valueBar.value = (int) ((double) v / (double) byte.MaxValue * 100.0);
    }

    private void RGBtoHSV(float r, float g, float b, out float h, out float s, out float v)
    {
      float num1 = Math.Min(Math.Min(r, g), b);
      float num2 = Math.Max(Math.Max(r, g), b);
      v = num2;
      float num3 = num2 - num1;
      if ((double) num2 != 0.0)
      {
        s = num3 / num2;
        h = (double) r != (double) num2 ? ((double) g != (double) num2 ? (float) (4.0 + ((double) r - (double) g) / (double) num3) : (float) (2.0 + ((double) b - (double) r) / (double) num3)) : (g - b) / num3;
        h *= 60f;
        if ((double) h >= 0.0)
          return;
        h += 360f;
      }
      else
      {
        s = 0.0f;
        h = -1f;
      }
    }

    private Color HsvToRgb(double h, double S, double V)
    {
      double num1 = h;
      while (num1 < 0.0)
      {
        ++num1;
        if (num1 < -1000000.0)
          num1 = 0.0;
      }
      while (num1 >= 360.0)
        --num1;
      double num2;
      double num3;
      double num4;
      if (V <= 0.0)
      {
        double num5;
        num2 = num5 = 0.0;
        num3 = num5;
        num4 = num5;
      }
      else if (S <= 0.0)
      {
        double num6;
        num2 = num6 = V;
        num3 = num6;
        num4 = num6;
      }
      else
      {
        double d = num1 / 60.0;
        int num7 = (int) Math.Floor(d);
        double num8 = d - (double) num7;
        double num9 = V * (1.0 - S);
        double num10 = V * (1.0 - S * num8);
        double num11 = V * (1.0 - S * (1.0 - num8));
        switch (num7)
        {
          case -1:
            num4 = V;
            num3 = num9;
            num2 = num10;
            break;
          case 0:
            num4 = V;
            num3 = num11;
            num2 = num9;
            break;
          case 1:
            num4 = num10;
            num3 = V;
            num2 = num9;
            break;
          case 2:
            num4 = num9;
            num3 = V;
            num2 = num11;
            break;
          case 3:
            num4 = num9;
            num3 = num10;
            num2 = V;
            break;
          case 4:
            num4 = num11;
            num3 = num9;
            num2 = V;
            break;
          case 5:
            num4 = V;
            num3 = num9;
            num2 = num10;
            break;
          case 6:
            num4 = V;
            num3 = num11;
            num2 = num9;
            break;
          default:
            double num12;
            num2 = num12 = V;
            num3 = num12;
            num4 = num12;
            break;
        }
      }
      return new Color(this.Clamp((int) (num4 * (double) byte.MaxValue)), this.Clamp((int) (num3 * (double) byte.MaxValue)), this.Clamp((int) (num2 * (double) byte.MaxValue)));
    }

    private int Clamp(int i)
    {
      if (i < 0)
        return 0;
      return i > (int) byte.MaxValue ? (int) byte.MaxValue : i;
    }
  }
}
