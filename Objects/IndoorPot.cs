// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.IndoorPot
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.TerrainFeatures;
using System;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class IndoorPot : StardewValley.Object
  {
    [XmlElement("hoeDirt")]
    public readonly NetRef<HoeDirt> hoeDirt = new NetRef<HoeDirt>();
    [XmlElement("bush")]
    public readonly NetRef<Bush> bush = new NetRef<Bush>();
    [XmlIgnore]
    private readonly NetBool bushLoadDirty = new NetBool(true);

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.hoeDirt, (INetSerializable) this.bush, (INetSerializable) this.bushLoadDirty);
    }

    public IndoorPot()
    {
    }

    public IndoorPot(Vector2 tileLocation)
      : base(tileLocation, 62)
    {
      this.hoeDirt.Value = new HoeDirt();
      if (Game1.IsRainingHere(Game1.currentLocation) && (bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.isOutdoors)
        this.hoeDirt.Value.state.Value = 1;
      this.showNextIndex.Value = (int) (NetFieldBase<int, NetInt>) this.hoeDirt.Value.state == 1;
    }

    public override void DayUpdate(GameLocation location)
    {
      base.DayUpdate(location);
      this.hoeDirt.Value.dayUpdate(location, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      if (Game1.IsRainingHere(location) && (bool) (NetFieldBase<bool, NetBool>) location.isOutdoors)
        this.hoeDirt.Value.state.Value = 1;
      this.showNextIndex.Value = (int) (NetFieldBase<int, NetInt>) this.hoeDirt.Value.state == 1;
      if (this.heldObject.Value != null)
        this.readyForHarvest.Value = true;
      if (this.bush.Value == null)
        return;
      this.bush.Value.dayUpdate(location);
    }

    public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
    {
      if (who != null && dropInItem != null && this.bush.Value == null && this.hoeDirt.Value.canPlantThisSeedHere((int) (NetFieldBase<int, NetInt>) dropInItem.parentSheetIndex, (int) this.tileLocation.X, (int) this.tileLocation.Y, dropInItem.Category == -19))
      {
        if ((int) (NetFieldBase<int, NetInt>) dropInItem.parentSheetIndex == 805)
        {
          if (!probe)
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
          return false;
        }
        if ((int) (NetFieldBase<int, NetInt>) dropInItem.parentSheetIndex == 499)
        {
          if (!probe)
          {
            Game1.playSound("cancel");
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Objects:AncientFruitPot"));
          }
          return false;
        }
        if (!probe)
        {
          if (!this.hoeDirt.Value.plant((int) (NetFieldBase<int, NetInt>) dropInItem.parentSheetIndex, (int) this.tileLocation.X, (int) this.tileLocation.Y, who, dropInItem.Category == -19, who.currentLocation))
            return false;
        }
        else
          this.heldObject.Value = new StardewValley.Object();
        return true;
      }
      if (who == null || dropInItem == null || this.hoeDirt.Value.crop != null || this.bush.Value != null || !(dropInItem is StardewValley.Object) || (bool) (NetFieldBase<bool, NetBool>) (dropInItem as StardewValley.Object).bigCraftable || (int) (NetFieldBase<int, NetInt>) dropInItem.parentSheetIndex != 251)
        return false;
      if (probe)
      {
        this.heldObject.Value = new StardewValley.Object();
      }
      else
      {
        this.bush.Value = new Bush((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, 3, who.currentLocation);
        if (!who.currentLocation.IsOutdoors)
        {
          this.bush.Value.greenhouseBush.Value = true;
          this.bush.Value.loadSprite();
          Game1.playSound("coin");
        }
      }
      return true;
    }

    public override bool performToolAction(Tool t, GameLocation location)
    {
      if (t != null)
      {
        this.hoeDirt.Value.performToolAction(t, -1, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, location);
        if (this.bush.Value != null)
        {
          if (this.bush.Value.performToolAction(t, -1, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, location))
            this.bush.Value = (Bush) null;
          return false;
        }
      }
      if ((int) (NetFieldBase<int, NetInt>) this.hoeDirt.Value.state == 1)
        this.showNextIndex.Value = true;
      return base.performToolAction(t, location);
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (who != null)
      {
        if (justCheckingForActivity)
        {
          string season = Game1.GetSeasonForLocation(who.currentLocation);
          if (this.bush.Value != null && (int) (NetFieldBase<int, NetInt>) this.bush.Value.overrideSeason != -1)
            season = Utility.getSeasonNameFromNumber((int) (NetFieldBase<int, NetInt>) this.bush.Value.overrideSeason);
          if (this.hoeDirt.Value.readyForHarvest() || this.heldObject.Value != null)
            return true;
          return this.bush.Value != null && this.bush.Value.inBloom(season, Game1.dayOfMonth);
        }
        if (who.isMoving())
          Game1.haltAfterCheck = false;
        if (this.heldObject.Value != null)
        {
          int num = who.addItemToInventoryBool((Item) this.heldObject.Value) ? 1 : 0;
          if (num == 0)
            return num != 0;
          this.heldObject.Value = (StardewValley.Object) null;
          this.readyForHarvest.Value = false;
          Game1.playSound("coin");
          return num != 0;
        }
        bool flag = this.hoeDirt.Value.performUseAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, who.currentLocation);
        if (flag)
          return flag;
        if (this.hoeDirt.Value.crop != null && (int) (NetFieldBase<int, NetInt>) this.hoeDirt.Value.crop.currentPhase > 0 && (double) this.hoeDirt.Value.getMaxShake() == 0.0)
        {
          this.hoeDirt.Value.shake((float) Math.PI / 32f, 0.06283186f, Game1.random.NextDouble() < 0.5);
          DelayedAction.playSoundAfterDelay("leafrustle", Game1.random.Next(100));
        }
        if (this.bush.Value != null)
          this.bush.Value.performUseAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, who.currentLocation);
      }
      return false;
    }

    public override void actionOnPlayerEntry()
    {
      base.actionOnPlayerEntry();
      if (this.hoeDirt.Value == null)
        return;
      this.hoeDirt.Value.performPlayerEntryAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      base.updateWhenCurrentLocation(time, environment);
      this.hoeDirt.Value.tickUpdate(time, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, environment);
      this.bush.Value?.tickUpdate(time, environment);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.bushLoadDirty)
        return;
      this.bush.Value?.loadSprite();
      this.bushLoadDirty.Value = false;
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      Vector2 vector2 = this.getScale() * 4f;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64)));
      Rectangle destinationRectangle = new Rectangle((int) ((double) local.X - (double) vector2.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y - (double) vector2.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (64.0 + (double) vector2.X), (int) (128.0 + (double) vector2.Y / 2.0));
      spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable((bool) (NetFieldBase<bool, NetBool>) this.showNextIndex ? this.ParentSheetIndex + 1 : this.ParentSheetIndex)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 105 ? 0.00350000010803342 : 0.0) + (double) x * 9.99999974737875E-06));
      if ((int) (NetFieldBase<int, NetInt>) this.hoeDirt.Value.fertilizer != 0)
      {
        Rectangle fertilizerSourceRect = this.hoeDirt.Value.GetFertilizerSourceRect((int) (NetFieldBase<int, NetInt>) this.hoeDirt.Value.fertilizer) with
        {
          Width = 13,
          Height = 13
        };
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) this.tileLocation.X * 64.0 + 4.0), (float) ((double) this.tileLocation.Y * 64.0 - 12.0))), new Rectangle?(fertilizerSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.tileLocation.Y + 0.649999976158142) * 64.0 / 10000.0 + (double) x * 9.99999974737875E-06));
      }
      if (this.hoeDirt.Value.crop != null)
        this.hoeDirt.Value.crop.drawWithOffset(spriteBatch, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, (int) (NetFieldBase<int, NetInt>) this.hoeDirt.Value.state != 1 || (int) (NetFieldBase<int, NetInt>) this.hoeDirt.Value.crop.currentPhase != 0 || (bool) (NetFieldBase<bool, NetBool>) this.hoeDirt.Value.crop.raisedSeeds ? Color.White : new Color(180, 100, 200) * 1f, this.hoeDirt.Value.getShakeRotation(), new Vector2(32f, 8f));
      if (this.heldObject.Value != null)
        this.heldObject.Value.draw(spriteBatch, x * 64, y * 64 - 48, (float) (((double) this.tileLocation.Y + 0.660000026226044) * 64.0 / 10000.0 + (double) x * 9.99999974737875E-06), 1f);
      if (this.bush.Value == null)
        return;
      this.bush.Value.draw(spriteBatch, new Vector2((float) x, (float) y), -24f);
    }
  }
}
