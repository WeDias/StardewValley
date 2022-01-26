// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.Stable
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Characters;
using System;

namespace StardewValley.Buildings
{
  public class Stable : Building
  {
    private readonly NetGuid horseId = new NetGuid();

    public Guid HorseId
    {
      get => this.horseId.Value;
      set => this.horseId.Value = value;
    }

    public Stable()
    {
    }

    public Stable(Guid horseId, BluePrint b, Vector2 tileLocation)
      : base(b, tileLocation)
    {
      this.HorseId = horseId;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddField((INetSerializable) this.horseId);
    }

    public override Rectangle getSourceRectForMenu() => new Rectangle(0, 0, this.texture.Value.Bounds.Width, this.texture.Value.Bounds.Height);

    public Horse getStableHorse() => Utility.findHorse(this.HorseId);

    public virtual void grabHorse()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
        return;
      Horse character = Utility.findHorse(this.HorseId);
      if (character == null)
      {
        character = new Horse(this.HorseId, (int) (NetFieldBase<int, NetInt>) this.tileX + 1, (int) (NetFieldBase<int, NetInt>) this.tileY + 1);
        Game1.getFarm().characters.Add((NPC) character);
      }
      else
        Game1.warpCharacter((NPC) character, "Farm", new Point((int) (NetFieldBase<int, NetInt>) this.tileX + 1, (int) (NetFieldBase<int, NetInt>) this.tileY + 1));
      character.ownerId.Value = this.owner.Value;
    }

    public virtual void updateHorseOwnership()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
        return;
      Horse horse = Utility.findHorse(this.HorseId);
      if (horse == null)
        return;
      horse.ownerId.Value = this.owner.Value;
      if (horse.getOwner() == null)
        return;
      if (horse.getOwner().horseName.Value != null)
      {
        horse.name.Value = horse.getOwner().horseName.Value;
        horse.displayName = horse.getOwner().horseName.Value;
      }
      else
      {
        horse.name.Value = "";
        horse.displayName = "";
      }
    }

    public override void dayUpdate(int dayOfMonth)
    {
      base.dayUpdate(dayOfMonth);
      this.grabHorse();
    }

    public override bool intersects(Rectangle boundingBox)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
        return base.intersects(boundingBox);
      if (!base.intersects(boundingBox))
        return false;
      return boundingBox.X < ((int) (NetFieldBase<int, NetInt>) this.tileX + 1) * 64 || boundingBox.Right >= ((int) (NetFieldBase<int, NetInt>) this.tileX + 3) * 64 || boundingBox.Y <= ((int) (NetFieldBase<int, NetInt>) this.tileY + 1) * 64;
    }

    public override void performActionOnDemolition(GameLocation location)
    {
      Horse stableHorse = this.getStableHorse();
      if (stableHorse != null && stableHorse.currentLocation != null)
        stableHorse.currentLocation.characters.Remove((NPC) stableHorse);
      Game1.player.team.demolishStableEvent.Fire(this.HorseId);
      base.performActionOnDemolition(location);
    }

    public override void Update(GameTime time) => base.Update(time);

    public override void draw(SpriteBatch b)
    {
      if (this.isMoving)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
      {
        this.drawInConstruction(b);
      }
      else
      {
        this.drawShadow(b);
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64))), new Rectangle?(this.texture.Value.Bounds), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, (float) this.texture.Value.Bounds.Height), 4f, SpriteEffects.None, (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1) * 64) / 10000f);
      }
    }
  }
}
