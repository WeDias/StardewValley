// Decompiled with JetBrains decompiler
// Type: StardewValley.VampiricEnchantment
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System;

namespace StardewValley
{
  public class VampiricEnchantment : BaseWeaponEnchantment
  {
    protected override void _OnDealDamage(
      Monster monster,
      GameLocation location,
      Farmer who,
      ref int amount)
    {
    }

    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
      if (Game1.random.NextDouble() >= 0.0900000035762787)
        return;
      int number = Math.Max(1, (int) ((double) (m.MaxHealth + Game1.random.Next(-m.MaxHealth / 10, m.MaxHealth / 15 + 1)) * 0.100000001490116));
      who.health = Math.Min(who.maxHealth, Game1.player.health + number);
      location.debris.Add(new Debris(number, new Vector2((float) Game1.player.getStandingX(), (float) Game1.player.getStandingY()), Color.Lime, 1f, (Character) who));
      Game1.playSound("healSound");
    }

    public override string GetName() => "Vampiric";
  }
}
