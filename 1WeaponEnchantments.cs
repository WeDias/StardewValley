// Decompiled with JetBrains decompiler
// Type: StardewValley.BaseWeaponEnchantment
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using StardewValley.Tools;

namespace StardewValley
{
  public class BaseWeaponEnchantment : BaseEnchantment
  {
    public override bool CanApplyTo(Item item) => item is MeleeWeapon && !(item as MeleeWeapon).isScythe();

    public void OnSwing(MeleeWeapon weapon, Farmer farmer) => this._OnSwing(weapon, farmer);

    protected virtual void _OnSwing(MeleeWeapon weapon, Farmer farmer)
    {
    }
  }
}
