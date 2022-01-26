// Decompiled with JetBrains decompiler
// Type: StardewValley.RainDrop
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;

namespace StardewValley
{
  public struct RainDrop
  {
    public int frame;
    public int accumulator;
    public Vector2 position;

    public RainDrop(int x, int y, int frame, int accumulator)
    {
      this.position = new Vector2((float) x, (float) y);
      this.frame = frame;
      this.accumulator = accumulator;
    }
  }
}
