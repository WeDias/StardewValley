// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.BoundingBoxGroup
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace StardewValley.Util
{
  public class BoundingBoxGroup
  {
    private List<Rectangle> rectangles = new List<Rectangle>();

    public bool Intersects(Rectangle rect)
    {
      foreach (Rectangle rectangle in this.rectangles)
      {
        if (rectangle.Intersects(rect))
          return true;
      }
      return false;
    }

    public bool Contains(int x, int y)
    {
      foreach (Rectangle rectangle in this.rectangles)
      {
        if (rectangle.Contains(x, y))
          return true;
      }
      return false;
    }

    public void Add(Rectangle rect)
    {
      if (this.rectangles.Contains(rect))
        return;
      this.rectangles.Add(rect);
    }

    public void ClearNonIntersecting(Rectangle rect)
    {
      for (int index = this.rectangles.Count - 1; index >= 0; --index)
      {
        if (!this.rectangles[index].Intersects(rect))
          this.rectangles.RemoveAt(index);
      }
    }

    public void Clear() => this.rectangles.Clear();

    public void Draw(SpriteBatch b)
    {
      foreach (Rectangle rectangle in this.rectangles)
      {
        rectangle.Offset(-Game1.viewport.X, -Game1.viewport.Y);
        b.Draw(Game1.fadeToBlackRect, rectangle, Color.Green * 0.5f);
      }
    }

    public bool IsEmpty() => this.rectangles.Count == 0;
  }
}
