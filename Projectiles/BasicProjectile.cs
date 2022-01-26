// Decompiled with JetBrains decompiler
// Type: StardewValley.Projectiles.BasicProjectile
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.TerrainFeatures;

namespace StardewValley.Projectiles
{
  public class BasicProjectile : Projectile
  {
    public readonly NetInt damageToFarmer = new NetInt();
    private readonly NetString collisionSound = new NetString();
    private readonly NetBool explode = new NetBool();
    private BasicProjectile.onCollisionBehavior collisionBehavior;
    public NetInt debuff = new NetInt(-1);
    public NetString debuffSound = new NetString("debuffHit");

    public BasicProjectile() => this.NetFields.AddFields((INetSerializable) this.damageToFarmer, (INetSerializable) this.collisionSound, (INetSerializable) this.explode, (INetSerializable) this.debuff, (INetSerializable) this.debuffSound);

    /// <summary>non-spell projectile constructor</summary>
    public BasicProjectile(
      int damageToFarmer,
      int parentSheetIndex,
      int bouncesTillDestruct,
      int tailLength,
      float rotationVelocity,
      float xVelocity,
      float yVelocity,
      Vector2 startingPosition,
      string collisionSound,
      string firingSound,
      bool explode,
      bool damagesMonsters = false,
      GameLocation location = null,
      Character firer = null,
      bool spriteFromObjectSheet = false,
      BasicProjectile.onCollisionBehavior collisionBehavior = null)
      : this()
    {
      this.damageToFarmer.Value = damageToFarmer;
      this.currentTileSheetIndex.Value = parentSheetIndex;
      this.bouncesLeft.Value = bouncesTillDestruct;
      this.tailLength.Value = tailLength;
      this.rotationVelocity.Value = rotationVelocity;
      this.xVelocity.Value = xVelocity;
      this.yVelocity.Value = yVelocity;
      this.position.Value = startingPosition;
      switch (firingSound)
      {
        case "":
        case null:
          this.explode.Value = explode;
          this.collisionSound.Value = collisionSound;
          this.damagesMonsters.Value = damagesMonsters;
          this.theOneWhoFiredMe.Set(location, firer);
          this.spriteFromObjectSheet.Value = spriteFromObjectSheet;
          this.collisionBehavior = collisionBehavior;
          break;
        default:
          if (location != null)
          {
            location.playSound(firingSound);
            goto case "";
          }
          else
            goto case "";
      }
    }

    /// <summary>
    ///  spell projectile constructor (uses default spell sounds and collison)
    /// </summary>
    public BasicProjectile(
      int damageToFarmer,
      int parentSheetIndex,
      int bouncesTillDestruct,
      int tailLength,
      float rotationVelocity,
      float xVelocity,
      float yVelocity,
      Vector2 startingPosition)
      : this(damageToFarmer, parentSheetIndex, bouncesTillDestruct, tailLength, rotationVelocity, xVelocity, yVelocity, startingPosition, "flameSpellHit", "flameSpell", true)
    {
    }

    public override void updatePosition(GameTime time)
    {
      this.position.X += (float) (NetFieldBase<float, NetFloat>) this.xVelocity;
      this.position.Y += (float) (NetFieldBase<float, NetFloat>) this.yVelocity;
    }

    public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.damagesMonsters)
        return;
      if (this.debuff.Value != -1 && player.CanBeDamaged() && Game1.random.Next(11) >= player.immunity && !player.hasBuff(28))
      {
        if (Game1.player == player)
          Game1.buffsDisplay.addOtherBuff(new Buff((int) (NetFieldBase<int, NetInt>) this.debuff));
        location.playSound(this.debuffSound.Value);
      }
      player.takeDamage((int) (NetFieldBase<int, NetInt>) this.damageToFarmer, false, (Monster) null);
      this.explosionAnimation(location);
    }

    public override void behaviorOnCollisionWithTerrainFeature(
      TerrainFeature t,
      Vector2 tileLocation,
      GameLocation location)
    {
      t.performUseAction(tileLocation, location);
      this.explosionAnimation(location);
    }

    public override void behaviorOnCollisionWithMineWall(int tileX, int tileY) => this.explosionAnimation((GameLocation) Game1.mine);

    public override void behaviorOnCollisionWithOther(GameLocation location) => this.explosionAnimation(location);

    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.damagesMonsters)
        return;
      this.explosionAnimation(location);
      if (n is Monster)
      {
        location.damageMonster(n.GetBoundingBox(), (int) (NetFieldBase<int, NetInt>) this.damageToFarmer, (int) (NetFieldBase<int, NetInt>) this.damageToFarmer + 1, false, this.theOneWhoFiredMe.Get(location) is Farmer ? this.theOneWhoFiredMe.Get(location) as Farmer : Game1.player);
      }
      else
      {
        if (!this.spriteFromObjectSheet.Value)
          return;
        n.getHitByPlayer(this.theOneWhoFiredMe.Get(location) == null || !(this.theOneWhoFiredMe.Get(location) is Farmer) ? Game1.player : this.theOneWhoFiredMe.Get(location) as Farmer, location);
        string word = "";
        if (Game1.objectInformation.ContainsKey(this.currentTileSheetIndex.Value))
          word = Game1.objectInformation[this.currentTileSheetIndex.Value].Split('/')[4];
        else if (Game1.objectInformation.ContainsKey(this.currentTileSheetIndex.Value - 1))
          word = Game1.objectInformation[this.currentTileSheetIndex.Value - 1].Split('/')[4];
        Game1.multiplayer.globalChatInfoMessage("Slingshot_Hit", (this.theOneWhoFiredMe.Get(location) == null || !(this.theOneWhoFiredMe.Get(location) is Farmer) ? (Character) Game1.player : (Character) (this.theOneWhoFiredMe.Get(location) as Farmer)).Name, n.Name == null ? "???" : n.Name, Lexicon.prependArticle(word));
      }
    }

    private void explosionAnimation(GameLocation location)
    {
      Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet((bool) (NetFieldBase<bool, NetBool>) this.spriteFromObjectSheet ? Game1.objectSpriteSheet : Projectile.projectileSheet, (int) (NetFieldBase<int, NetInt>) this.currentTileSheetIndex);
      standardTileSheet.X += 28;
      standardTileSheet.Y += 28;
      standardTileSheet.Width = 8;
      standardTileSheet.Height = 8;
      int debrisType = 12;
      switch (this.currentTileSheetIndex.Value)
      {
        case 378:
          debrisType = 0;
          break;
        case 380:
          debrisType = 2;
          break;
        case 382:
          debrisType = 4;
          break;
        case 384:
          debrisType = 6;
          break;
        case 386:
          debrisType = 10;
          break;
        case 390:
          debrisType = 14;
          break;
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.spriteFromObjectSheet)
        Game1.createRadialDebris(location, debrisType, (int) ((double) this.position.X + 32.0) / 64, (int) ((double) this.position.Y + 32.0) / 64, 6, false);
      else
        Game1.createRadialDebris(location, "TileSheets\\Projectiles", standardTileSheet, 4, (int) this.position.X + 32, (int) this.position.Y + 32, 12, (int) ((double) this.position.Y / 64.0) + 1);
      if (this.collisionSound.Value != null && !this.collisionSound.Value.Equals(""))
        location.playSound((string) (NetFieldBase<string, NetString>) this.collisionSound);
      if ((bool) (NetFieldBase<bool, NetBool>) this.explode)
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position, false, Game1.random.NextDouble() < 0.5));
      if (this.collisionBehavior != null)
      {
        BasicProjectile.onCollisionBehavior collisionBehavior = this.collisionBehavior;
        GameLocation location1 = location;
        Rectangle boundingBox = this.getBoundingBox();
        int x = boundingBox.Center.X;
        boundingBox = this.getBoundingBox();
        int y = boundingBox.Center.Y;
        Character who = this.theOneWhoFiredMe.Get(location);
        collisionBehavior(location1, x, y, who);
      }
      this.destroyMe = true;
    }

    public static void explodeOnImpact(GameLocation location, int x, int y, Character who) => location.explode(new Vector2((float) (x / 64), (float) (y / 64)), 2, who is Farmer ? (Farmer) who : (Farmer) null);

    public delegate void onCollisionBehavior(
      GameLocation location,
      int xPosition,
      int yPosition,
      Character who);
  }
}
