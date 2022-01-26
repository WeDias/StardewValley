// Decompiled with JetBrains decompiler
// Type: StardewValley.PreservingEnchantment
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley
{
  public class PreservingEnchantment : FishingRodEnchantment
  {
    public override string GetName() => "Preserving";

    protected override void _ApplyTo(Item item) => base._ApplyTo(item);

    protected override void _UnapplyTo(Item item) => base._UnapplyTo(item);
  }
}
