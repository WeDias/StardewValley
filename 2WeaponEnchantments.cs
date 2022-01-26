// Decompiled with JetBrains decompiler
// Type: StardewValley.MagicEnchantment
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;

namespace StardewValley
{
  public class MagicEnchantment : BaseWeaponEnchantment
  {
    protected override void _OnSwing(MeleeWeapon weapon, Farmer farmer)
    {
      base._OnSwing(weapon, farmer);
      Vector2 vector2 = new Vector2();
      Vector2 startingPosition = farmer.getStandingPosition() - new Vector2(32f, 32f);
      switch (farmer.facingDirection.Value)
      {
        case 0:
          vector2.Y = -1f;
          break;
        case 1:
          vector2.X = 1f;
          break;
        case 2:
          vector2.Y = 1f;
          break;
        case 3:
          vector2.X = -1f;
          break;
      }
      float num = 32f;
      vector2 *= 10f;
      BasicProjectile basicProjectile = new BasicProjectile((int) Math.Ceiling((double) weapon.minDamage.Value / 4.0), 11, 0, 1, num * ((float) Math.PI / 180f), vector2.X, vector2.Y, startingPosition, "", "", false, true, farmer.currentLocation, (Character) farmer);
      basicProjectile.ignoreTravelGracePeriod.Value = true;
      basicProjectile.ignoreMeleeAttacks.Value = true;
      basicProjectile.maxTravelDistance.Value = 256;
      basicProjectile.height.Value = 32f;
      farmer.currentLocation.projectiles.Add((Projectile) basicProjectile);
    }

    public override string GetName() => "Starburst";
  }
}
