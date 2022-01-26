// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Cask
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class Cask : StardewValley.Object
  {
    public const int defaultDaysToMature = 56;
    [XmlElement("agingRate")]
    public readonly NetFloat agingRate = new NetFloat();
    [XmlElement("daysToMature")]
    public readonly NetFloat daysToMature = new NetFloat();

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.agingRate, (INetSerializable) this.daysToMature);
    }

    public Cask()
    {
    }

    public Cask(Vector2 v)
      : base(v, 163)
    {
    }

    public override bool performToolAction(Tool t, GameLocation location)
    {
      if (t == null || !t.isHeavyHitter() || t is MeleeWeapon)
        return base.performToolAction(t, location);
      if (this.heldObject.Value != null)
        Game1.createItemDebris((Item) this.heldObject.Value, this.tileLocation.Value * 64f, -1);
      location.playSound("woodWhack");
      if (this.heldObject.Value == null)
        return true;
      this.heldObject.Value = (StardewValley.Object) null;
      this.readyForHarvest.Value = false;
      this.minutesUntilReady.Value = -1;
      return false;
    }

    public virtual bool IsValidCaskLocation(GameLocation location) => location is Cellar || location.getMapProperty("CanCaskHere") != "";

    public override bool performObjectDropInAction(Item dropIn, bool probe, Farmer who)
    {
      if (dropIn != null && dropIn is StardewValley.Object && (bool) (NetFieldBase<bool, NetBool>) (dropIn as StardewValley.Object).bigCraftable)
        return false;
      if (this.heldObject.Value != null)
      {
        if ((int) (NetFieldBase<int, NetInt>) dropIn.parentSheetIndex != 872)
          return false;
        if (probe)
          return true;
        if (this.heldObject.Value == null || this.heldObject.Value.Quality == 4)
          return false;
        Utility.addSprinklesToLocation(who.currentLocation, (int) this.tileLocation.X, (int) this.tileLocation.Y, 1, 2, 400, 40, Color.White);
        Game1.playSound("yoba");
        this.daysToMature.Value = this.GetDaysForQuality(this.GetNextQuality(this.heldObject.Value.Quality));
        this.checkForMaturity();
        return true;
      }
      if (!probe && (who == null || !this.IsValidCaskLocation(who.currentLocation)))
      {
        if (StardewValley.Object.autoLoadChest == null)
          Game1.showRedMessageUsingLoadString("Strings\\Objects:CaskNoCellar");
        return false;
      }
      if ((int) (NetFieldBase<int, NetInt>) this.quality >= 4 || dropIn is StardewValley.Object @object && @object.Quality >= 4)
        return false;
      float multiplierForItem = this.GetAgingMultiplierForItem(dropIn);
      if ((double) multiplierForItem <= 0.0)
        return false;
      this.heldObject.Value = dropIn.getOne() as StardewValley.Object;
      if (!probe)
      {
        this.agingRate.Value = multiplierForItem;
        this.minutesUntilReady.Value = 999999;
        this.daysToMature.Value = this.GetDaysForQuality(this.heldObject.Value.Quality);
        if (this.heldObject.Value.Quality == 4)
          this.minutesUntilReady.Value = 1;
        who.currentLocation.playSound("Ship");
        who.currentLocation.playSound("bubbles");
        Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
        {
          alphaFade = 0.005f
        });
      }
      return true;
    }

    public virtual float GetAgingMultiplierForItem(Item item)
    {
      if (item == null || !Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex))
        return 0.0f;
      switch ((int) (NetFieldBase<int, NetInt>) item.parentSheetIndex)
      {
        case 303:
          return 1.66f;
        case 346:
          return 2f;
        case 348:
          return 1f;
        case 424:
          return 4f;
        case 426:
          return 4f;
        case 459:
          return 2f;
        default:
          return 0.0f;
      }
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false) => base.checkForAction(who, justCheckingForActivity);

    public override void DayUpdate(GameLocation location)
    {
      base.DayUpdate(location);
      if (this.heldObject.Value == null)
        return;
      this.minutesUntilReady.Value = 999999;
      this.daysToMature.Value -= (float) (NetFieldBase<float, NetFloat>) this.agingRate;
      this.checkForMaturity();
    }

    public float GetDaysForQuality(int quality)
    {
      switch (quality)
      {
        case 1:
          return 42f;
        case 2:
          return 28f;
        case 4:
          return 0.0f;
        default:
          return 56f;
      }
    }

    public int GetNextQuality(int quality)
    {
      if (quality == 4 || quality == 2)
        return 4;
      return quality == 1 ? 2 : 1;
    }

    public void checkForMaturity()
    {
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.daysToMature > (double) this.GetDaysForQuality(this.GetNextQuality(this.heldObject.Value.quality.Value)))
        return;
      this.heldObject.Value.quality.Value = this.GetNextQuality(this.heldObject.Value.quality.Value);
      if (this.heldObject.Value.Quality != 4)
        return;
      this.minutesUntilReady.Value = 1;
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      base.draw(spriteBatch, x, y, alpha);
      if (this.heldObject.Value == null || (int) (NetFieldBase<int, NetInt>) this.heldObject.Value.quality <= 0)
        return;
      Vector2 vector2 = ((int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady > 0 ? new Vector2(Math.Abs(this.scale.X - 5f), Math.Abs(this.scale.Y - 5f)) : Vector2.Zero) * 4f;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64)));
      Rectangle destinationRectangle = new Rectangle((int) ((double) local.X + 32.0 - 8.0 - (double) vector2.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y + 64.0 + 8.0 - (double) vector2.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (16.0 + (double) vector2.X), (int) (16.0 + (double) vector2.Y / 2.0));
      spriteBatch.Draw(Game1.mouseCursors, destinationRectangle, new Rectangle?((int) (NetFieldBase<int, NetInt>) this.heldObject.Value.quality < 4 ? new Rectangle(338 + ((int) (NetFieldBase<int, NetInt>) this.heldObject.Value.quality - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8)), Color.White * 0.95f, 0.0f, Vector2.Zero, SpriteEffects.None, (float) ((y + 1) * 64) / 10000f);
    }
  }
}
