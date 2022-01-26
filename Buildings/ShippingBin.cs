// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.ShippingBin
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Menus;
using System.Collections.Generic;

namespace StardewValley.Buildings
{
  public class ShippingBin : Building
  {
    private TemporaryAnimatedSprite shippingBinLid;
    private Farm farm;
    private Rectangle shippingBinLidOpenArea;
    protected Vector2 _lidGenerationPosition;

    public ShippingBin(BluePrint b, Vector2 tileLocation)
      : base(b, tileLocation)
    {
      this.initLid();
    }

    public ShippingBin()
    {
    }

    public void initLid()
    {
      this.shippingBinLid = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(134, 226, 30, 25), new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX, (float) ((int) (NetFieldBase<int, NetInt>) this.tileY - 1)) * 64f + new Vector2(1f, -7f) * 4f, false, 0.0f, Color.White)
      {
        holdLastFrame = true,
        destroyable = false,
        interval = 20f,
        animationLength = 13,
        paused = true,
        scale = 4f,
        layerDepth = (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + 1) * 64) / 10000.0 + 9.99999974737875E-05),
        pingPong = true,
        pingPongMotion = 0
      };
      this.shippingBinLidOpenArea = new Rectangle(((int) (NetFieldBase<int, NetInt>) this.tileX - 1) * 64, ((int) (NetFieldBase<int, NetInt>) this.tileY - 1) * 64, 256, 192);
      this._lidGenerationPosition = new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX, (float) (int) (NetFieldBase<int, NetInt>) this.tileY);
    }

    public override Rectangle getSourceRectForMenu() => new Rectangle(0, 0, this.texture.Value.Bounds.Width, this.texture.Value.Bounds.Height);

    public override void load() => base.load();

    public override void resetLocalState()
    {
      base.resetLocalState();
      if (this.shippingBinLid != null)
      {
        Rectangle shippingBinLidOpenArea = this.shippingBinLidOpenArea;
      }
      else
        this.initLid();
    }

    public override void Update(GameTime time)
    {
      base.Update(time);
      if (this.farm == null)
        this.farm = Game1.getFarm();
      if (this.shippingBinLid != null)
      {
        Rectangle shippingBinLidOpenArea = this.shippingBinLidOpenArea;
        if ((double) this._lidGenerationPosition.X == (double) (int) (NetFieldBase<int, NetInt>) this.tileX && (double) this._lidGenerationPosition.Y == (double) (int) (NetFieldBase<int, NetInt>) this.tileY)
        {
          bool flag = false;
          foreach (Character farmer in this.farm.farmers)
          {
            if (farmer.GetBoundingBox().Intersects(this.shippingBinLidOpenArea))
            {
              this.openShippingBinLid();
              flag = true;
            }
          }
          if (!flag)
            this.closeShippingBinLid();
          this.updateShippingBinLid(time);
          return;
        }
      }
      this.initLid();
    }

    private void openShippingBinLid()
    {
      if (this.shippingBinLid == null)
        return;
      if (this.shippingBinLid.pingPongMotion != 1 && Game1.currentLocation.Equals((GameLocation) this.farm))
        this.farm.localSound("doorCreak");
      this.shippingBinLid.pingPongMotion = 1;
      this.shippingBinLid.paused = false;
    }

    private void closeShippingBinLid()
    {
      if (this.shippingBinLid == null || this.shippingBinLid.currentParentTileIndex <= 0)
        return;
      if (this.shippingBinLid.pingPongMotion != -1 && Game1.currentLocation.Equals((GameLocation) this.farm))
        this.farm.localSound("doorCreakReverse");
      this.shippingBinLid.pingPongMotion = -1;
      this.shippingBinLid.paused = false;
    }

    private void updateShippingBinLid(GameTime time)
    {
      if (this.isShippingBinLidOpen(true) && this.shippingBinLid.pingPongMotion == 1)
        this.shippingBinLid.paused = true;
      else if (this.shippingBinLid.currentParentTileIndex == 0 && this.shippingBinLid.pingPongMotion == -1)
      {
        if (!this.shippingBinLid.paused && Game1.currentLocation.Equals((GameLocation) this.farm))
          this.farm.localSound("woodyStep");
        this.shippingBinLid.paused = true;
      }
      this.shippingBinLid.update(time);
    }

    private bool isShippingBinLidOpen(bool requiredToBeFullyOpen = false) => this.shippingBinLid != null && this.shippingBinLid.currentParentTileIndex >= (requiredToBeFullyOpen ? this.shippingBinLid.animationLength - 1 : 1);

    private void shipItem(Item i, Farmer who)
    {
      if (i == null)
        return;
      who.removeItemFromInventory(i);
      if (this.farm != null)
        this.farm.getShippingBin(who).Add(i);
      if (i is Object && this.farm != null)
        this.showShipment(i as Object, false);
      this.farm.lastItemShipped = i;
      if (Game1.player.ActiveObject != null)
        return;
      Game1.player.showNotCarrying();
      Game1.player.Halt();
    }

    public override bool CanLeftClick(int x, int y)
    {
      Rectangle rectangle = new Rectangle((int) (NetFieldBase<int, NetInt>) this.tileX * 64, (int) (NetFieldBase<int, NetInt>) this.tileY * 64, (int) (NetFieldBase<int, NetInt>) this.tilesWide * 64, (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64);
      rectangle.Y -= 64;
      rectangle.Height += 64;
      return rectangle.Contains(x, y);
    }

    public override bool leftClicked()
    {
      if (this.farm == null || Game1.player.ActiveObject == null || !Game1.player.ActiveObject.canBeShipped() || (double) Vector2.Distance(Game1.player.getTileLocation(), new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX + 0.5f, (float) (int) (NetFieldBase<int, NetInt>) this.tileY)) > 2.0)
        return base.leftClicked();
      this.farm.getShippingBin(Game1.player).Add((Item) Game1.player.ActiveObject);
      this.farm.lastItemShipped = (Item) Game1.player.ActiveObject;
      Game1.player.showNotCarrying();
      this.showShipment(Game1.player.ActiveObject);
      Game1.player.ActiveObject = (Object) null;
      return true;
    }

    public void showShipment(Object o, bool playThrowSound = true)
    {
      if (this.farm == null)
        return;
      if (playThrowSound)
        this.farm.localSound("backpackIN");
      DelayedAction.playSoundAfterDelay("Ship", playThrowSound ? 250 : 0);
      int num = Game1.random.Next();
      this.farm.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(524, 218, 34, 22), new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX, (float) ((int) (NetFieldBase<int, NetInt>) this.tileY - 1)) * 64f + new Vector2(-1f, 5f) * 4f, false, 0.0f, Color.White)
      {
        interval = 100f,
        totalNumberOfLoops = 1,
        animationLength = 3,
        pingPong = true,
        alpha = (float) (NetFieldBase<float, NetFloat>) this.alpha,
        scale = 4f,
        layerDepth = (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + 1) * 64) / 10000.0 + 0.000199999994947575),
        id = (float) num,
        extraInfoForEndBehavior = num,
        endFunction = new TemporaryAnimatedSprite.endBehavior(((GameLocation) this.farm).removeTemporarySpritesWithID)
      });
      this.farm.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(524, 230, 34, 10), new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX, (float) ((int) (NetFieldBase<int, NetInt>) this.tileY - 1)) * 64f + new Vector2(-1f, 17f) * 4f, false, 0.0f, Color.White)
      {
        interval = 100f,
        totalNumberOfLoops = 1,
        animationLength = 3,
        pingPong = true,
        alpha = (float) (NetFieldBase<float, NetFloat>) this.alpha,
        scale = 4f,
        layerDepth = (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + 1) * 64) / 10000.0 + 0.000300000014249235),
        id = (float) num,
        extraInfoForEndBehavior = num
      });
      this.farm.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) o.parentSheetIndex, 16, 16), new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX, (float) ((int) (NetFieldBase<int, NetInt>) this.tileY - 1)) * 64f + new Vector2((float) (7 + Game1.random.Next(6)), 2f) * 4f, false, 0.0f, Color.White)
      {
        interval = 9999f,
        scale = 4f,
        alphaFade = 0.045f,
        layerDepth = (float) ((double) (((int) (NetFieldBase<int, NetInt>) this.tileY + 1) * 64) / 10000.0 + 0.000224999996135011),
        motion = new Vector2(0.0f, 0.3f),
        acceleration = new Vector2(0.0f, 0.2f),
        scaleChange = -0.05f
      });
    }

    public override bool doAction(Vector2 tileLocation, Farmer who)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0 || (double) tileLocation.X < (double) (int) (NetFieldBase<int, NetInt>) this.tileX || (double) tileLocation.X > (double) ((int) (NetFieldBase<int, NetInt>) this.tileX + 1) || (double) tileLocation.Y != (double) (int) (NetFieldBase<int, NetInt>) this.tileY)
        return base.doAction(tileLocation, who);
      if (!Game1.didPlayerJustRightClick(true))
        return false;
      ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), new ItemGrabMenu.behaviorOnItemSelect(this.shipItem), "", snapToBottom: true, canBeExitedWithKey: true, playRightClickSound: false, context: ((object) this));
      itemGrabMenu.initializeUpperRightCloseButton();
      itemGrabMenu.setBackgroundTransparency(false);
      itemGrabMenu.setDestroyItemOnClick(true);
      itemGrabMenu.initializeShippingBin();
      Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
      if (who.IsLocalPlayer)
        Game1.playSound("shwip");
      if (Game1.player.FacingDirection == 1)
        Game1.player.Halt();
      Game1.player.showCarrying();
      return true;
    }

    public override void drawInMenu(SpriteBatch b, int x, int y)
    {
      base.drawInMenu(b, x, y);
      b.Draw(Game1.mouseCursors, new Vector2((float) (x + 4), (float) (y - 20)), new Rectangle?(new Rectangle(134, 226, 30, 25)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    }

    public override void draw(SpriteBatch b)
    {
      if (this.isMoving)
        return;
      base.draw(b);
      if (this.shippingBinLid == null || (int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
        return;
      this.shippingBinLid.color = (Color) (NetFieldBase<Color, NetColor>) this.color;
      this.shippingBinLid.draw(b, extraAlpha: ((float) (NetFieldBase<float, NetFloat>) this.alpha * ((int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0 ? (float) ((1000.0 - (double) (int) (NetFieldBase<int, NetInt>) this.newConstructionTimer) / 1000.0) : 1f)));
    }
  }
}
