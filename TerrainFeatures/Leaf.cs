// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Leaf
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;

namespace StardewValley.TerrainFeatures
{
  public class Leaf
  {
    public Vector2 position;
    public float rotation;
    public float rotationRate;
    public float yVelocity;
    public int type;

    public Leaf(Vector2 position, float rotationRate, int type, float yVelocity)
    {
      this.position = position;
      this.rotationRate = rotationRate;
      this.type = type;
      this.yVelocity = yVelocity;
    }
  }
}
