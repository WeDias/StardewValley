// Decompiled with JetBrains decompiler
// Type: StardewValley.BottomlessEnchantment
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using StardewValley.Tools;

namespace StardewValley
{
  public class BottomlessEnchantment : WateringCanEnchantment
  {
    public override string GetName() => "Bottomless";

    protected override void _ApplyTo(Item item)
    {
      base._ApplyTo(item);
      if (!(item is WateringCan wateringCan))
        return;
      wateringCan.IsBottomless = true;
      wateringCan.WaterLeft = wateringCan.waterCanMax;
    }

    protected override void _UnapplyTo(Item item)
    {
      base._UnapplyTo(item);
      if (!(item is WateringCan wateringCan))
        return;
      wateringCan.IsBottomless = false;
    }
  }
}
