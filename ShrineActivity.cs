// Decompiled with JetBrains decompiler
// Type: StardewValley.ShrineActivity
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;

namespace StardewValley
{
  public class ShrineActivity : FarmActivity
  {
    public override bool AttemptActivity(Farm farm)
    {
      this.activityPosition = new Vector2(8f, 8f);
      this.activityDirection = 0;
      return true;
    }
  }
}
