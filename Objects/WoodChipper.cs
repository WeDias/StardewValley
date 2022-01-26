// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.WoodChipper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Collections.Generic;

namespace StardewValley.Objects
{
  public class WoodChipper : StardewValley.Object
  {
    public const int CHIP_TIME = 1000;
    public readonly NetRef<StardewValley.Object> depositedItem = new NetRef<StardewValley.Object>();
    protected bool _isAnimatingChip;
    public int nextSmokeTime;
    public int nextShakeTime;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.depositedItem);
      this.depositedItem.fieldChangeVisibleEvent += new NetFieldBase<StardewValley.Object, NetRef<StardewValley.Object>>.FieldChange(this.OnDepositedItemChange);
    }

    public void OnDepositedItemChange(NetRef<StardewValley.Object> field, StardewValley.Object old_value, StardewValley.Object new_value)
    {
      if (Game1.gameMode == (byte) 6 || new_value == null)
        return;
      this.shakeTimer = 1000;
      this._isAnimatingChip = true;
    }

    public WoodChipper()
    {
    }

    public WoodChipper(Vector2 position)
      : base(position, 211)
    {
      this.Name = "Wood Chipper";
      this.type.Value = "Crafting";
      this.bigCraftable.Value = true;
      this.canBeSetDown.Value = true;
    }

    public override void addWorkingAnimation(GameLocation environment)
    {
      if (environment == null || !environment.farmers.Any() || Game1.random.NextDouble() >= 0.35)
        return;
      for (int index = 0; index < 8; ++index)
        environment.temporarySprites.Add(new TemporaryAnimatedSprite(47, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) (Game1.random.Next(-48, 0) - 76)), new Color(200, 110, 17), animationInterval: 50f, layerDepth: ((float) (3.0 / 1000.0 + (double) Math.Max(0.0f, (float) ((((double) this.tileLocation.Y + 1.0) * 64.0 - 24.0) / 10000.0)) + (double) this.tileLocation.X * 9.99999974737875E-06)))
        {
          delayBeforeAnimationStart = index * 100
        });
      environment.playSound("woodchipper_occasional");
      this.shakeTimer = 1500;
    }

    public override bool performObjectDropInAction(Item dropped_in_item, bool probe, Farmer who)
    {
      StardewValley.Object deposited_item = dropped_in_item as StardewValley.Object;
      if (this.heldObject.Value != null || this.depositedItem.Value != null || deposited_item == null)
        return false;
      KeyValuePair<int, int> resultItem = this.GetResultItem(deposited_item);
      if (resultItem.Key >= 0)
        this.heldObject.Value = new StardewValley.Object(resultItem.Key, resultItem.Value);
      if (probe)
        return true;
      if (resultItem.Key < 0)
        return false;
      this.depositedItem.Value = dropped_in_item.getOne() as StardewValley.Object;
      this.minutesUntilReady.Value = 180;
      this.shakeTimer = 1800;
      Game1.playSound("woodchipper");
      for (int index = 0; index < 12; ++index)
        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(47, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) (Game1.random.Next(-48, 0) - 76)), new Color(200, 110, 17), animationInterval: 50f, layerDepth: ((float) (3.0 / 1000.0 + (double) Math.Max(0.0f, (float) ((((double) this.tileLocation.Y + 1.0) * 64.0 - 24.0) / 10000.0)) + (double) this.tileLocation.X * 9.99999974737875E-06)))
        {
          delayBeforeAnimationStart = index * 100
        });
      return true;
    }

    public KeyValuePair<int, int> GetResultItem(StardewValley.Object deposited_item)
    {
      int key = -1;
      int num = 0;
      if (Utility.IsNormalObjectAtParentSheetIndex((Item) deposited_item, 709))
      {
        if (Game1.random.NextDouble() <= 0.0199999995529652)
        {
          switch (Game1.random.Next(0, 3))
          {
            case 0:
              key = 724;
              break;
            case 1:
              key = 725;
              break;
            case 2:
              key = 726;
              break;
          }
          num = 1;
        }
        else
        {
          key = 388;
          num = Game1.random.NextDouble() > 0.100000001490116 ? Game1.random.Next(5, 11) : Game1.random.Next(15, 21);
        }
      }
      else if (Utility.IsNormalObjectAtParentSheetIndex((Item) deposited_item, 169))
      {
        key = 388;
        num = Game1.random.Next(5, 10);
      }
      return new KeyValuePair<int, int>(key, num);
    }

    public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
    {
      this.tileLocation.Value = new Vector2((float) (x / 64), (float) (y / 64));
      return true;
    }

    public override void performRemoveAction(Vector2 tileLocation, GameLocation environment) => base.performRemoveAction(tileLocation, environment);

    public override void actionOnPlayerEntry()
    {
      if (this.depositedItem.Value != null)
      {
        int minutesUntilReady = this.MinutesUntilReady;
      }
      base.actionOnPlayerEntry();
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (!who.IsLocalPlayer || this.heldObject.Value == null || !this.readyForHarvest.Value)
        return base.checkForAction(who, justCheckingForActivity);
      if (!justCheckingForActivity)
      {
        StardewValley.Object @object = this.heldObject.Value;
        this.heldObject.Value = (StardewValley.Object) null;
        if (who.isMoving())
          Game1.haltAfterCheck = false;
        if (!who.addItemToInventoryBool((Item) @object))
        {
          this.heldObject.Value = @object;
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
          return false;
        }
        Game1.playSound("coin");
        this.readyForHarvest.Value = false;
        this.depositedItem.Value = (StardewValley.Object) null;
        this.heldObject.Value = (StardewValley.Object) null;
        this.AttemptAutoLoad(who);
      }
      return true;
    }

    public override void onReadyForHarvest(GameLocation environment) => base.onReadyForHarvest(environment);

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      if (this.depositedItem.Value != null && this.MinutesUntilReady > 0)
      {
        this.nextShakeTime -= time.ElapsedGameTime.Milliseconds;
        this.nextSmokeTime -= time.ElapsedGameTime.Milliseconds;
        if (this.nextSmokeTime <= 0)
          this.nextSmokeTime = Game1.random.Next(3000, 6000);
        if (this.nextShakeTime <= 0)
        {
          this.nextShakeTime = Game1.random.Next(1000, 2000);
          if (this.shakeTimer <= 0)
          {
            this._isAnimatingChip = false;
            this.shakeTimer = 0;
          }
        }
      }
      base.updateWhenCurrentLocation(time, environment);
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      if (this.isTemporarilyInvisible)
        return;
      Vector2 vector2_1 = Vector2.One * 4f;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64)));
      Rectangle destinationRectangle = new Rectangle((int) ((double) local.X - (double) vector2_1.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y - (double) vector2_1.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (64.0 + (double) vector2_1.X), (int) (128.0 + (double) vector2_1.Y / 2.0));
      float layerDepth = Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (float) x * 1E-05f;
      spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable(this.ParentSheetIndex + (this.readyForHarvest.Value ? 1 : 0))), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
      if (this.shakeTimer > 0)
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, new Rectangle(destinationRectangle.X, destinationRectangle.Y + 4, destinationRectangle.Width, 60), new Rectangle?(new Rectangle(80, 833, 16, 15)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth + 0.0035f);
      if (this.depositedItem.Value != null && this.shakeTimer > 0 && this._isAnimatingChip)
      {
        float t = (float) (1.0 - (double) this.shakeTimer / 1000.0);
        Vector2 vector2_2 = local + new Vector2(32f, 32f);
        Vector2 vector2_3 = vector2_2 + new Vector2(0.0f, -16f);
        Vector2 position = new Vector2();
        position.X = Utility.Lerp(vector2_3.X, vector2_2.X, t);
        position.Y = Utility.Lerp(vector2_3.Y, vector2_2.Y, t);
        position.X += (float) (Game1.random.Next(-1, 2) * 2);
        position.Y += (float) (Game1.random.Next(-1, 2) * 2);
        float num = Utility.Lerp(1f, 0.75f, t);
        spriteBatch.Draw(Game1.objectSpriteSheet, position, new Rectangle?(GameLocation.getSourceRectForObject(this.depositedItem.Value.ParentSheetIndex)), Color.White * alpha, 0.0f, new Vector2(8f, 8f), 4f * num, (bool) (NetFieldBase<bool, NetBool>) this.depositedItem.Value.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 0.00175f);
      }
      TimeSpan totalGameTime;
      if (this.depositedItem.Value != null && this.MinutesUntilReady > 0)
      {
        totalGameTime = Game1.currentGameTime.TotalGameTime;
        int num = (int) (totalGameTime.TotalMilliseconds % 200.0) / 50;
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, local + new Vector2(6f, 17f) * 4f, new Rectangle?(new Rectangle(80 + num % 2 * 8, 848 + num / 2 * 7, 8, 7)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, local + new Vector2(3f, 9f) * 4f + new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)), new Rectangle?(new Rectangle(51, 841, 10, 6)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.readyForHarvest)
        return;
      totalGameTime = Game1.currentGameTime.TotalGameTime;
      float num1 = (float) (4.0 * Math.Round(Math.Sin(totalGameTime.TotalMilliseconds / 250.0), 2));
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 - 8), (float) (y * 64 - 96 - 16) + num1)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) ((y + 1) * 64) / 10000.0 + 9.99999997475243E-07 + (double) this.tileLocation.X / 10000.0 + ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 105 ? 0.00150000001303852 : 0.0)));
      if (this.heldObject.Value == null)
        return;
      spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32), (float) (y * 64 - 64 - 8) + num1)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.heldObject.Value.parentSheetIndex, 16, 16)), Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float) ((double) ((y + 1) * 64) / 10000.0 + 9.99999974737875E-06 + (double) this.tileLocation.X / 10000.0 + ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 105 ? 0.00150000001303852 : 0.0)));
      if (!(this.heldObject.Value is ColoredObject))
        return;
      spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32), (float) (y * 64 - 64 - 8) + num1)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.heldObject.Value.parentSheetIndex + 1, 16, 16)), (this.heldObject.Value as ColoredObject).color.Value * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float) ((double) ((y + 1) * 64) / 10000.0 + 9.99999974737875E-06 + (double) this.tileLocation.X / 10000.0 + ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 105 ? 0.00150000001303852 : 9.99999974737875E-06)));
    }
  }
}
