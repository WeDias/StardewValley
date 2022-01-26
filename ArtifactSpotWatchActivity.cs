// Decompiled with JetBrains decompiler
// Type: StardewValley.ArtifactSpotWatchActivity
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;

namespace StardewValley
{
  public class ArtifactSpotWatchActivity : FarmActivity
  {
    protected override bool _AttemptActivity(Farm farm)
    {
      Object randomObject = this.GetRandomObject(farm, (Func<Object, bool>) (o => Utility.IsNormalObjectAtParentSheetIndex((Item) o, 595)));
      if (randomObject == null)
        return false;
      this.activityPosition = this.GetNearbyTile(farm, randomObject.TileLocation);
      return true;
    }
  }
}
