// Decompiled with JetBrains decompiler
// Type: StardewValley.FlowerWatchActivity
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using System;

namespace StardewValley
{
  public class FlowerWatchActivity : FarmActivity
  {
    protected override bool _AttemptActivity(Farm farm)
    {
      HoeDirt randomCrop = this.GetRandomCrop(farm, (Func<Crop, bool>) (crop => crop.currentPhase.Value >= crop.phaseDays.Count - 1 && new Object(Vector2.Zero, crop.indexOfHarvest.Value, 1).category.Value == -80));
      if (randomCrop == null)
        return false;
      this.activityPosition = randomCrop.currentTileLocation;
      return true;
    }
  }
}
