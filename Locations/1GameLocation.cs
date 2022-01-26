// Decompiled with JetBrains decompiler
// Type: WaterTiles
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

public class WaterTiles
{
  public WaterTiles.WaterTileData[,] waterTiles;

  public static implicit operator WaterTiles(bool[,] source)
  {
    WaterTiles waterTiles = new WaterTiles();
    waterTiles.waterTiles = new WaterTiles.WaterTileData[source.GetLength(0), source.GetLength(1)];
    for (int index1 = 0; index1 < source.GetLength(0); ++index1)
    {
      for (int index2 = 0; index2 < source.GetLength(1); ++index2)
        waterTiles.waterTiles[index1, index2] = new WaterTiles.WaterTileData(source[index1, index2], true);
    }
    return waterTiles;
  }

  public bool this[int x, int y]
  {
    get => this.waterTiles[x, y].isWater;
    set => this.waterTiles[x, y] = new WaterTiles.WaterTileData(value, true);
  }

  public struct WaterTileData
  {
    public bool isWater;
    public bool isVisible;

    public WaterTileData(bool is_water, bool is_visible)
    {
      this.isWater = is_water;
      this.isVisible = is_visible;
    }
  }
}
