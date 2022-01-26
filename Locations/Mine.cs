// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Mine
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley.Locations
{
  public class Mine : GameLocation
  {
    public Mine()
    {
    }

    public Mine(string map, string name)
      : base(map, name)
    {
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      MineShaft.mushroomLevelsGeneratedToday.Clear();
    }
  }
}
