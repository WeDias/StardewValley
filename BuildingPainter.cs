// Decompiled with JetBrains decompiler
// Type: StardewValley.BuildingPainter
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  [XmlInclude(typeof (BuildingPaintColor))]
  public class BuildingPainter
  {
    [XmlIgnore]
    public static Dictionary<string, List<List<int>>> paintMaskLookup = new Dictionary<string, List<List<int>>>();

    public static Texture2D Apply(
      Texture2D base_texture,
      string mask_path,
      BuildingPaintColor color)
    {
      List<List<int>> intListList = (List<List<int>>) null;
      if (BuildingPainter.paintMaskLookup.ContainsKey(mask_path))
      {
        intListList = BuildingPainter.paintMaskLookup[mask_path];
      }
      else
      {
        try
        {
          Texture2D texture2D = Game1.content.Load<Texture2D>(mask_path);
          Color[] data = new Color[texture2D.Width * texture2D.Height];
          texture2D.GetData<Color>(data);
          intListList = new List<List<int>>();
          for (int index = 0; index < 3; ++index)
            intListList.Add(new List<int>());
          for (int index = 0; index < data.Length; ++index)
          {
            if (data[index] == Color.Red)
              intListList[0].Add(index);
            else if (data[index] == Color.Lime)
              intListList[1].Add(index);
            else if (data[index] == Color.Blue)
              intListList[2].Add(index);
          }
          BuildingPainter.paintMaskLookup[mask_path] = intListList;
        }
        catch (Exception ex)
        {
          BuildingPainter.paintMaskLookup[mask_path] = (List<List<int>>) null;
        }
      }
      if (intListList == null)
        return (Texture2D) null;
      if (!color.RequiresRecolor())
        return (Texture2D) null;
      Color[] colorArray = new Color[base_texture.Width * base_texture.Height];
      base_texture.GetData<Color>(colorArray);
      Texture2D texture2D1 = new Texture2D(Game1.graphics.GraphicsDevice, base_texture.Width, base_texture.Height);
      if (!color.Color1Default.Value)
      {
        BuildingPainter._ApplyPaint(0, -100, 0, colorArray, intListList[0]);
        BuildingPainter._ApplyPaint(color.Color1Hue.Value, color.Color1Saturation.Value, color.Color1Lightness.Value, colorArray, intListList[0]);
      }
      if (!color.Color2Default.Value)
      {
        BuildingPainter._ApplyPaint(0, -100, 0, colorArray, intListList[1]);
        BuildingPainter._ApplyPaint(color.Color2Hue.Value, color.Color2Saturation.Value, color.Color2Lightness.Value, colorArray, intListList[1]);
      }
      if (!color.Color3Default.Value)
      {
        BuildingPainter._ApplyPaint(0, -100, 0, colorArray, intListList[2]);
        BuildingPainter._ApplyPaint(color.Color3Hue.Value, color.Color3Saturation.Value, color.Color3Lightness.Value, colorArray, intListList[2]);
      }
      texture2D1.SetData<Color>(colorArray);
      return texture2D1;
    }

    protected static void _ApplyPaint(
      int h_shift,
      int s_shift,
      int l_shift,
      Color[] pixels,
      List<int> indices)
    {
      foreach (int index in indices)
      {
        Color pixel = pixels[index];
        double h;
        double s;
        double l;
        Utility.RGBtoHSL((int) pixel.R, (int) pixel.G, (int) pixel.B, out h, out s, out l);
        h += (double) h_shift;
        s += (double) s_shift / 100.0;
        l += (double) l_shift / 100.0;
        while (h > 360.0)
          h -= 360.0;
        while (h < 0.0)
          h += 360.0;
        if (s < 0.0)
          s = 0.0;
        if (s > 1.0)
          s = 1.0;
        if (l < 0.0)
          l = 0.0;
        if (l > 1.0)
          l = 1.0;
        int r;
        int g;
        int b;
        Utility.HSLtoRGB(h, s, l, out r, out g, out b);
        pixel.R = (byte) r;
        pixel.G = (byte) g;
        pixel.B = (byte) b;
        pixels[index] = pixel;
      }
    }
  }
}
