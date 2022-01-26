// Decompiled with JetBrains decompiler
// Type: StardewValley.Projectiles.DebuffingProjectile
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;

namespace StardewValley.Projectiles
{
  public class DebuffingProjectile : Projectile
  {
    private readonly NetInt debuff = new NetInt();

    public DebuffingProjectile() => this.NetFields.AddField((INetSerializable) this.debuff);

    public DebuffingProjectile(
      int debuff,
      int parentSheetIndex,
      int bouncesTillDestruct,
      int tailLength,
      float rotationVelocity,
      float xVelocity,
      float yVelocity,
      Vector2 startingPosition,
      GameLocation location = null,
      Character owner = null)
      : this()
    {
      this.theOneWhoFiredMe.Set(location, owner);
      this.debuff.Value = debuff;
      this.currentTileSheetIndex.Value = parentSheetIndex;
      this.bouncesLeft.Value = bouncesTillDestruct;
      this.tailLength.Value = tailLength;
      this.rotationVelocity.Value = rotationVelocity;
      this.xVelocity.Value = xVelocity;
      this.yVelocity.Value = yVelocity;
      this.position.Value = startingPosition;
      if (location == null)
        Game1.playSound("debuffSpell");
      else
        location.playSound("debuffSpell");
    }

    public override void updatePosition(GameTime time)
    {
      this.position.X += (float) (NetFieldBase<float, NetFloat>) this.xVelocity;
      this.position.Y += (float) (NetFieldBase<float, NetFloat>) this.yVelocity;
      this.position.X += (float) (Math.Sin((double) time.TotalGameTime.Milliseconds * Math.PI / 128.0) * 8.0);
      this.position.Y += (float) (Math.Cos((double) time.TotalGameTime.Milliseconds * Math.PI / 128.0) * 8.0);
    }

    public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
    {
      if (Game1.random.Next(11) < player.immunity || player.hasBuff(28))
        return;
      if (Game1.player == player)
        Game1.buffsDisplay.addOtherBuff(new Buff((int) (NetFieldBase<int, NetInt>) this.debuff));
      this.explosionAnimation(location);
      if ((int) (NetFieldBase<int, NetInt>) this.debuff == 19)
        location.playSound("frozen");
      else
        location.playSound("debuffHit");
    }

    public override void behaviorOnCollisionWithTerrainFeature(
      TerrainFeature t,
      Vector2 tileLocation,
      GameLocation location)
    {
      this.explosionAnimation(location);
    }

    public override void behaviorOnCollisionWithMineWall(int tileX, int tileY) => this.explosionAnimation((GameLocation) Game1.mine);

    public override void behaviorOnCollisionWithOther(GameLocation location) => this.explosionAnimation(location);

    private void explosionAnimation(GameLocation location) => Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(352, (float) Game1.random.Next(100, 150), 2, 1, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position, false, false));

    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
    }
  }
}
