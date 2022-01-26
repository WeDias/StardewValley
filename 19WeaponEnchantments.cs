// Decompiled with JetBrains decompiler
// Type: StardewValley.FishingRodEnchantment
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using StardewValley.Tools;
using System.Xml.Serialization;

namespace StardewValley
{
  [XmlInclude(typeof (FishingRodEnchantment))]
  public class FishingRodEnchantment : BaseEnchantment
  {
    public override bool CanApplyTo(Item item) => item is FishingRod;
  }
}
