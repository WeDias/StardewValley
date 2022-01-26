// Decompiled with JetBrains decompiler
// Type: StardewValley.GalaxySoulEnchantment
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley
{
  public class GalaxySoulEnchantment : BaseWeaponEnchantment
  {
    public override bool IsSecondaryEnchantment() => true;

    public override bool IsForge() => false;

    public override int GetMaximumLevel() => 3;

    public override bool ShouldBeDisplayed() => false;
  }
}
