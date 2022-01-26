// Decompiled with JetBrains decompiler
// Type: StardewValley.CropWatchActivity
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using System;

namespace StardewValley
{
  public class CropWatchActivity : FarmActivity
  {
    protected Object _cropObject;

    protected override bool _AttemptActivity(Farm farm)
    {
      this._cropObject = (Object) null;
      HoeDirt randomCrop = this.GetRandomCrop(farm, (Func<Crop, bool>) (crop => crop.currentPhase.Value <= 0 && new Object(Vector2.Zero, crop.indexOfHarvest.Value, 1).category.Value != -80));
      if (randomCrop == null)
        return false;
      this._cropObject = new Object(Vector2.Zero, randomCrop.crop.indexOfHarvest.Value, 1);
      this.activityPosition = randomCrop.currentTileLocation;
      return true;
    }

    protected override void _EndActivity() => this._cropObject = (Object) null;

    protected override void _BeginActivity()
    {
      if (this._cropObject == null)
        return;
      switch (this._character.getGiftTasteForThisItem((Item) this._cropObject))
      {
        case 2:
          this._character.doEmote(32, false);
          break;
        case 6:
          this._character.doEmote(20, false);
          break;
        case 8:
          break;
        default:
          this._character.doEmote(12, false);
          break;
      }
    }
  }
}
