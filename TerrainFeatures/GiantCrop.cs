// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.GiantCrop
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

namespace StardewValley.TerrainFeatures
{
  public class GiantCrop : ResourceClump
  {
    public const int cauliflower = 0;
    public const int melon = 1;
    public const int pumpkin = 2;
    [XmlElement("which")]
    public readonly NetInt which = new NetInt();
    [XmlElement("forSale")]
    public readonly NetBool forSale = new NetBool();

    public GiantCrop() => this.NetFields.AddFields((INetSerializable) this.which, (INetSerializable) this.forSale);

    public GiantCrop(int indexOfSmallerVersion, Vector2 tile)
      : this()
    {
      this.tile.Value = tile;
      this.parentSheetIndex.Value = indexOfSmallerVersion;
      switch (indexOfSmallerVersion)
      {
        case 190:
          this.which.Value = 0;
          break;
        case 254:
          this.which.Value = 1;
          break;
        case 276:
          this.which.Value = 2;
          break;
      }
      this.width.Value = 3;
      this.height.Value = 3;
      this.health.Value = 3f;
    }

    public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation) => spriteBatch.Draw(Game1.cropSpriteSheet, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f - new Vector2((double) this.shakeTimer > 0.0 ? (float) (Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 2.0) : 0.0f, 64f)), new Rectangle?(new Rectangle(112 + (int) (NetFieldBase<int, NetInt>) this.which * 48, 512, 48, 63)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tileLocation.Y + 2.0) * 64.0 / 10000.0));

    public override bool performToolAction(
      Tool t,
      int damage,
      Vector2 tileLocation,
      GameLocation location)
    {
      if (t == null || !(t is Axe))
        return false;
      location.playSound("axchop");
      int num1 = (int) (NetFieldBase<int, NetInt>) t.upgradeLevel / 2 + 1;
      this.health.Value -= (float) num1;
      Game1.createRadialDebris(Game1.currentLocation, 12, (int) tileLocation.X + 1, (int) tileLocation.Y + 1, Game1.random.Next(4, 9), false);
      if (t is Axe && t.hasEnchantmentOfType<ShavingEnchantment>() && Game1.random.NextDouble() <= (double) num1 / 5.0)
      {
        Debris debris = new Debris((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, new Vector2((float) ((double) tileLocation.X * 64.0 + 96.0), (float) (((double) tileLocation.Y + 0.5) * 64.0)), new Vector2((float) Game1.player.getStandingX(), (float) Game1.player.getStandingY()));
        debris.Chunks[0].xVelocity.Value += (float) Game1.random.Next(-10, 11) / 10f;
        debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 128.0);
        location.debris.Add(debris);
      }
      if ((double) this.shakeTimer <= 0.0)
      {
        this.shakeTimer = 100f;
        this.NeedsUpdate = true;
      }
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health > 0.0)
        return false;
      t.getLastFarmerToUse().gainExperience(5, 50 * (((int) (NetFieldBase<int, NetInt>) t.getLastFarmerToUse().luckLevel + 1) / 2));
      if (location.HasUnlockedAreaSecretNotes(t.getLastFarmerToUse()))
      {
        StardewValley.Object unseenSecretNote = location.tryToCreateUnseenSecretNote(t.getLastFarmerToUse());
        if (unseenSecretNote != null)
          Game1.createItemDebris((Item) unseenSecretNote, tileLocation * 64f, -1, location);
      }
      Random random;
      if (Game1.IsMultiplayer)
      {
        Game1.recentMultiplayerRandom = new Random((int) tileLocation.X * 1000 + (int) tileLocation.Y);
        random = Game1.recentMultiplayerRandom;
      }
      else
        random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + (int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
      int num2 = random.Next(15, 22);
      if (Game1.IsMultiplayer)
        Game1.createMultipleObjectDebris((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, (int) tileLocation.X + 1, (int) tileLocation.Y + 1, num2, t.getLastFarmerToUse().UniqueMultiplayerID, location);
      else
        Game1.createRadialDebris(location, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, (int) tileLocation.X, (int) tileLocation.Y, num2, false, item: true);
      Game1.setRichPresence("giantcrop", (object) new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 1).Name);
      Game1.createRadialDebris(Game1.currentLocation, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(4, 9), false);
      location.playSound("stumpCrack");
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, tileLocation * 64f, Color.White));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 0.0f)) * 64f, Color.White, animationInterval: 110f));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 1f)) * 64f, Color.White, flipped: true, animationInterval: 80f));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(0.0f, 1f)) * 64f, Color.White, animationInterval: 90f));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, tileLocation * 64f + new Vector2(32f, 32f), Color.White, animationInterval: 70f));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, tileLocation * 64f, Color.White));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(2f, 0.0f)) * 64f, Color.White, animationInterval: 110f));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(2f, 1f)) * 64f, Color.White, flipped: true, animationInterval: 80f));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(2f, 2f)) * 64f, Color.White, animationInterval: 90f));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, tileLocation * 64f + new Vector2(96f, 96f), Color.White, animationInterval: 70f));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(0.0f, 2f)) * 64f, Color.White, animationInterval: 110f));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 2f)) * 64f, Color.White, flipped: true, animationInterval: 80f));
      return true;
    }
  }
}
