// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.MiniatureTerrainFeature
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;

namespace StardewValley.Menus
{
  public class MiniatureTerrainFeature
  {
    private TerrainFeature feature;
    private Vector2 positionOnScreen;
    private Vector2 tileLocation;
    private float scale;

    public MiniatureTerrainFeature(
      TerrainFeature feature,
      Vector2 positionOnScreen,
      Vector2 tileLocation,
      float scale)
    {
      this.feature = feature;
      this.positionOnScreen = positionOnScreen;
      this.scale = scale;
      this.tileLocation = tileLocation;
    }

    public void draw(SpriteBatch b) => this.feature.drawInMenu(b, this.positionOnScreen, this.tileLocation, this.scale, 0.86f);
  }
}
