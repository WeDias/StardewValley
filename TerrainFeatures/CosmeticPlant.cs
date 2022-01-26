// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.CosmeticPlant
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
  public class CosmeticPlant : Grass
  {
    [XmlElement("flipped")]
    public readonly NetBool flipped = new NetBool();
    [XmlElement("scale")]
    public readonly NetFloat scale = new NetFloat(1f);
    [XmlElement("xOffset")]
    private readonly NetInt xOffset = new NetInt();
    [XmlElement("yOffset")]
    private readonly NetInt yOffset = new NetInt();

    public CosmeticPlant() => this.initFields();

    public CosmeticPlant(int which)
      : base(which, 1)
    {
      this.initFields();
      this.flipped.Value = Game1.random.NextDouble() < 0.5;
    }

    private void initFields() => this.NetFields.AddFields((INetSerializable) this.flipped, (INetSerializable) this.scale, (INetSerializable) this.xOffset, (INetSerializable) this.yOffset);

    public override Rectangle getBoundingBox(Vector2 tileLocation) => new Rectangle((int) ((double) tileLocation.X * 64.0 + 16.0), (int) (((double) tileLocation.Y + 1.0) * 64.0 - 8.0 - 4.0), 8, 8);

    public override bool seasonUpdate(bool onLoad) => false;

    public override string textureName() => "TerrainFeatures\\upperCavePlants";

    public override void loadSprite()
    {
      this.xOffset.Value = Game1.random.Next(-2, 3) * 4;
      this.yOffset.Value = Game1.random.Next(-2, 1) * 4;
    }

    public override bool performToolAction(
      Tool t,
      int explosion,
      Vector2 tileLocation,
      GameLocation location = null)
    {
      if (t != null && t is MeleeWeapon && (int) (NetFieldBase<int, NetInt>) ((MeleeWeapon) t).type != 2 || explosion > 0)
      {
        this.shake(3f * (float) Math.PI / 32f, (float) Math.PI / 40f, Game1.random.NextDouble() < 0.5);
        int num = explosion <= 0 ? ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel == 3 ? 3 : (int) (NetFieldBase<int, NetInt>) t.upgradeLevel + 1) : Math.Max(1, explosion + 2 - Game1.random.Next(2));
        Game1.createRadialDebris(location, this.textureName(), new Rectangle((int) (byte) (NetFieldBase<byte, NetByte>) this.grassType * 16, 6, 7, 6), (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(6, 14));
        this.numberOfWeeds.Value = (int) (NetFieldBase<int, NetInt>) this.numberOfWeeds - num;
        if ((int) (NetFieldBase<int, NetInt>) this.numberOfWeeds <= 0)
        {
          Random random = new Random((int) ((double) Game1.uniqueIDForThisGame + (double) tileLocation.X * 7.0 + (double) tileLocation.Y * 11.0 + (double) Game1.CurrentMineLevel + (double) Game1.player.timesReachedMineBottom));
          if (random.NextDouble() < 0.005)
            Game1.createObjectDebris(114, (int) tileLocation.X, (int) tileLocation.Y, -1, 0, 1f, location);
          else if (random.NextDouble() < 0.01)
            Game1.createDebris(random.NextDouble() < 0.5 ? 4 : 8, (int) tileLocation.X, (int) tileLocation.Y, random.Next(1, 2), location);
          else if (random.NextDouble() < 0.02)
            Game1.createDebris(92, (int) tileLocation.X, (int) tileLocation.Y, random.Next(2, 4), location);
          return true;
        }
      }
      return false;
    }

    public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation) => spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f) + new Vector2((float) (32 + (int) (NetFieldBase<int, NetInt>) this.xOffset), (float) (60 + (int) (NetFieldBase<int, NetInt>) this.yOffset))), new Rectangle?(new Rectangle((int) (byte) (NetFieldBase<byte, NetByte>) this.grassType * 16, 0, 16, 24)), Color.White, this.shakeRotation, new Vector2(8f, 23f), 4f * (float) (NetFieldBase<float, NetFloat>) this.scale, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) (this.getBoundingBox(tileLocation).Y - 4) + (double) tileLocation.X / 900.0 + (double) (float) (NetFieldBase<float, NetFloat>) this.scale / 100.0) / 10000.0));
  }
}
