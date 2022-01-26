// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.ItemPedestal
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class ItemPedestal : StardewValley.Object
  {
    [XmlIgnore]
    public NetMutex itemModifyMutex = new NetMutex();
    [XmlElement("pedestalType")]
    public NetInt pedestalType = new NetInt(0);
    [XmlElement("requiredItem")]
    public NetRef<StardewValley.Object> requiredItem = new NetRef<StardewValley.Object>();
    [XmlElement("successColor")]
    public NetColor successColor = new NetColor();
    [XmlElement("lockOnSuccess")]
    public NetBool lockOnSuccess = new NetBool();
    [XmlElement("locked")]
    public NetBool locked = new NetBool();
    [XmlElement("match")]
    public NetBool match = new NetBool();
    [XmlIgnore]
    public Texture2D texture;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.itemModifyMutex.NetFields, (INetSerializable) this.pedestalType, (INetSerializable) this.requiredItem, (INetSerializable) this.successColor, (INetSerializable) this.lockOnSuccess, (INetSerializable) this.locked, (INetSerializable) this.match);
      this.heldObject.InterpolationWait = false;
    }

    public ItemPedestal()
    {
    }

    public ItemPedestal(
      Vector2 tile,
      StardewValley.Object required_item,
      bool lock_on_success,
      Color success_color,
      int pedestal_type = 221)
      : base(tile, 0)
    {
      this.pedestalType.Value = pedestal_type;
      this.requiredItem.Value = required_item;
      this.lockOnSuccess.Value = lock_on_success;
      this.successColor.Value = success_color;
    }

    public override bool performObjectDropInAction(Item drop_in_item, bool probe, Farmer who)
    {
      if (this.locked.Value || !drop_in_item.canBeTrashed())
        return false;
      if (this.heldObject.Value != null)
      {
        this.DropObject(who);
        return false;
      }
      if (!(drop_in_item.GetType() == typeof (StardewValley.Object)))
        return false;
      if (probe)
        return true;
      StardewValley.Object placed_object = drop_in_item.getOne() as StardewValley.Object;
      this.itemModifyMutex.RequestLock((Action) (() =>
      {
        who.currentLocation.playSound("woodyStep");
        this.heldObject.Value = placed_object;
        this.UpdateItemMatch();
        this.itemModifyMutex.ReleaseLock();
      }), (Action) (() =>
      {
        if (placed_object == this.heldObject.Value)
          return;
        Game1.createItemDebris((Item) placed_object, (this.TileLocation + new Vector2(0.5f, 0.5f)) * 64f, -1, who.currentLocation);
      }));
      return true;
    }

    public virtual void UpdateItemMatch()
    {
      bool flag = false;
      if (this.heldObject.Value != null && this.requiredItem.Value != null && Utility.getStandardDescriptionFromItem((Item) this.heldObject.Value, 1) == Utility.getStandardDescriptionFromItem((Item) this.requiredItem.Value, 1))
        flag = true;
      if (flag == this.match.Value)
        return;
      this.match.Value = flag;
      if (!this.match.Value || !this.lockOnSuccess.Value)
        return;
      this.locked.Value = true;
    }

    public override bool checkForAction(Farmer who, bool checking_for_activity = false) => !this.locked.Value && (checking_for_activity || this.DropObject(who));

    public bool DropObject(Farmer who)
    {
      if (this.heldObject.Value == null)
        return false;
      this.itemModifyMutex.RequestLock((Action) (() =>
      {
        StardewValley.Object @object = this.heldObject.Value;
        this.heldObject.Value = (StardewValley.Object) null;
        if (who.addItemToInventoryBool((Item) @object))
        {
          @object.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, who.currentLocation);
          Game1.playSound("coin");
        }
        else
          this.heldObject.Value = @object;
        this.UpdateItemMatch();
        this.itemModifyMutex.ReleaseLock();
      }), (Action) (() => { }));
      return true;
    }

    public override bool performToolAction(Tool t, GameLocation location) => false;

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment) => this.itemModifyMutex.Update(environment);

    public override bool onExplosion(Farmer who, GameLocation location) => false;

    public override void DayUpdate(GameLocation location)
    {
      base.DayUpdate(location);
      this.itemModifyMutex.ReleaseLock();
    }

    public override void draw(SpriteBatch b, int x, int y, float alpha = 1f)
    {
      Vector2 globalPosition = new Vector2((float) (x * 64), (float) (y * 64));
      b.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable(this.pedestalType.Value)), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, (float) (((double) globalPosition.Y - 2.0) / 10000.0)));
      if (this.match.Value)
        b.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable(this.pedestalType.Value + 1)), this.successColor.Value, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, (float) (((double) globalPosition.Y - 1.0) / 10000.0)));
      if (this.heldObject.Value == null)
        return;
      Vector2 vector2 = new Vector2((float) x, (float) y);
      if (this.heldObject.Value.bigCraftable.Value)
        --vector2.Y;
      this.heldObject.Value.draw(b, (int) vector2.X * 64, (int) (((double) vector2.Y - 0.200000002980232) * 64.0) - 64, globalPosition.Y / 10000f, 1f);
    }
  }
}
