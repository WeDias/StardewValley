// Decompiled with JetBrains decompiler
// Type: StardewValley.PowerfulEnchantment
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using StardewValley.Tools;

namespace StardewValley
{
  public class PowerfulEnchantment : BaseEnchantment
  {
    public override string GetName() => "Powerful";

    public override bool CanApplyTo(Item item)
    {
      if (!(item is Tool))
        return false;
      return item is Pickaxe || item is Axe;
    }

    protected override void _ApplyTo(Item item)
    {
      base._ApplyTo(item);
      if (!(item is Tool tool))
        return;
      if (tool is Pickaxe)
        (tool as Pickaxe).additionalPower.Value += this.GetLevel();
      if (!(tool is Axe))
        return;
      (tool as Axe).additionalPower.Value += 2 * this.GetLevel();
    }

    protected override void _UnapplyTo(Item item)
    {
      base._UnapplyTo(item);
      if (!(item is Tool tool))
        return;
      if (tool is Pickaxe)
        (tool as Pickaxe).additionalPower.Value -= this.GetLevel();
      if (!(tool is Axe))
        return;
      (tool as Axe).additionalPower.Value -= 2 * this.GetLevel();
    }
  }
}
