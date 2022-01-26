// Decompiled with JetBrains decompiler
// Type: StardewValley.BugKillerEnchantment
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using StardewValley.Monsters;

namespace StardewValley
{
  public class BugKillerEnchantment : BaseWeaponEnchantment
  {
    protected override void _OnDealDamage(
      Monster monster,
      GameLocation location,
      Farmer who,
      ref int amount)
    {
      switch (monster)
      {
        case Grub _:
        case Fly _:
        case Bug _:
        case Leaper _:
        case LavaCrab _:
        case RockCrab _:
          amount = (int) ((double) amount * 2.0);
          break;
      }
    }

    public override string GetName() => "Bug Killer";
  }
}
