// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Slingshot
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Tools
{
  public class Slingshot : Tool
  {
    public const int basicDamage = 5;
    public const int basicSlingshot = 32;
    public const int masterSlingshot = 33;
    public const int galaxySlingshot = 34;
    public const int drawBackSoundThreshold = 8;
    [XmlIgnore]
    public int recentClickX;
    [XmlIgnore]
    public int recentClickY;
    [XmlIgnore]
    public int lastClickX;
    [XmlIgnore]
    public int lastClickY;
    [XmlIgnore]
    public int mouseDragAmount;
    [XmlIgnore]
    public double pullStartTime = -1.0;
    [XmlIgnore]
    public float nextAutoFire = -1f;
    private bool canPlaySound;
    [XmlIgnore]
    private readonly NetEvent0 finishEvent = new NetEvent0();
    [XmlIgnore]
    public readonly NetPoint aimPos = new NetPoint().Interpolated(true, true);

    public Slingshot()
    {
      this.InitialParentTileIndex = 32;
      this.CurrentParentTileIndex = this.InitialParentTileIndex;
      this.IndexOfMenuItemView = this.CurrentParentTileIndex;
      this.BaseName = Game1.content.Load<Dictionary<int, string>>("Data\\weapons")[(int) (NetFieldBase<int, NetInt>) this.initialParentTileIndex].Split('/')[0];
      this.numAttachmentSlots.Value = 1;
      this.attachments.SetCount(1);
    }

    public override Item getOne()
    {
      Slingshot destination = new Slingshot(this.InitialParentTileIndex);
      this.CopyEnchantments((Tool) this, (Tool) destination);
      destination._GetOneFrom((Item) this);
      return (Item) destination;
    }

    protected override string loadDisplayName()
    {
      string[] strArray = Game1.content.Load<Dictionary<int, string>>("Data\\weapons")[(int) (NetFieldBase<int, NetInt>) this.initialParentTileIndex].Split('/');
      return LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en ? strArray[strArray.Length - 1] : this.Name;
    }

    protected override string loadDescription() => Game1.content.Load<Dictionary<int, string>>("Data\\weapons")[(int) (NetFieldBase<int, NetInt>) this.initialParentTileIndex].Split('/')[1];

    public override bool doesShowTileLocationMarker() => false;

    public Slingshot(int which = 32)
    {
      this.InitialParentTileIndex = which;
      this.CurrentParentTileIndex = this.InitialParentTileIndex;
      this.IndexOfMenuItemView = this.CurrentParentTileIndex;
      this.BaseName = Game1.content.Load<Dictionary<int, string>>("Data\\weapons")[(int) (NetFieldBase<int, NetInt>) this.initialParentTileIndex].Split('/')[0];
      this.numAttachmentSlots.Value = 1;
      this.attachments.SetCount(1);
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.finishEvent, (INetSerializable) this.aimPos);
      this.finishEvent.onEvent += new NetEvent0.Event(this.doFinish);
    }

    public int GetBackArmDistance(Farmer who)
    {
      if (this.CanAutoFire() && (double) this.nextAutoFire > 0.0)
        return (int) Utility.Lerp(20f, 0.0f, this.nextAutoFire / this.GetAutoFireRate());
      return !Game1.options.useLegacySlingshotFiring ? (int) (20.0 * (double) this.GetSlingshotChargeTime()) : Math.Min(20, (int) Vector2.Distance(who.getStandingPosition(), new Vector2((float) this.aimPos.X, (float) this.aimPos.Y)) / 20);
    }

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      this.IndexOfMenuItemView = this.InitialParentTileIndex;
      if (!this.CanAutoFire())
        this.PerformFire(location, who);
      this.finish();
    }

    public virtual void PerformFire(GameLocation location, Farmer who)
    {
      if (this.attachments[0] != null)
      {
        this.updateAimPos();
        int x = this.aimPos.X;
        int y = this.aimPos.Y;
        int backArmDistance = this.GetBackArmDistance(who);
        Vector2 shootOrigin = this.GetShootOrigin(who);
        Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(this.GetShootOrigin(who), this.AdjustForHeight(new Vector2((float) x, (float) y)), (float) (15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));
        if (backArmDistance > 4 && !this.canPlaySound)
        {
          StardewValley.Object one = (StardewValley.Object) this.attachments[0].getOne();
          --this.attachments[0].Stack;
          if (this.attachments[0].Stack <= 0)
            this.attachments[0] = (StardewValley.Object) null;
          int num1 = 1;
          BasicProjectile.onCollisionBehavior collisionBehavior = (BasicProjectile.onCollisionBehavior) null;
          string collisionSound = "hammer";
          float num2 = 1f;
          if (this.InitialParentTileIndex == 33)
            num2 = 2f;
          else if (this.InitialParentTileIndex == 34)
            num2 = 4f;
          switch (one.ParentSheetIndex)
          {
            case 378:
              num1 = 10;
              ++one.ParentSheetIndex;
              break;
            case 380:
              num1 = 20;
              ++one.ParentSheetIndex;
              break;
            case 382:
              num1 = 15;
              ++one.ParentSheetIndex;
              break;
            case 384:
              num1 = 30;
              ++one.ParentSheetIndex;
              break;
            case 386:
              num1 = 50;
              ++one.ParentSheetIndex;
              break;
            case 388:
              num1 = 2;
              ++one.ParentSheetIndex;
              break;
            case 390:
              num1 = 5;
              ++one.ParentSheetIndex;
              break;
            case 441:
              num1 = 20;
              collisionBehavior = new BasicProjectile.onCollisionBehavior(BasicProjectile.explodeOnImpact);
              collisionSound = "explosion";
              break;
          }
          if (one.Category == -5)
            collisionSound = "slimedead";
          if (!Game1.options.useLegacySlingshotFiring)
          {
            velocityTowardPoint.X *= -1f;
            velocityTowardPoint.Y *= -1f;
          }
          NetCollection<Projectile> projectiles = location.projectiles;
          BasicProjectile basicProjectile = new BasicProjectile((int) ((double) num2 * (double) (num1 + Game1.random.Next(-(num1 / 2), num1 + 2)) * (1.0 + (double) who.attackIncreaseModifier)), one.ParentSheetIndex, 0, 0, (float) (Math.PI / (64.0 + (double) Game1.random.Next(-63, 64))), -velocityTowardPoint.X, -velocityTowardPoint.Y, shootOrigin - new Vector2(32f, 32f), collisionSound, "", false, true, location, (Character) who, true, collisionBehavior);
          basicProjectile.IgnoreLocationCollision = Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null;
          projectiles.Add((Projectile) basicProjectile);
        }
      }
      else
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
      this.canPlaySound = true;
    }

    public Vector2 GetShootOrigin(Farmer who) => this.AdjustForHeight(new Vector2((float) who.getStandingX(), (float) who.getStandingY()), false);

    public Vector2 AdjustForHeight(Vector2 position, bool for_cursor = true) => !Game1.options.useLegacySlingshotFiring & for_cursor ? new Vector2(position.X, position.Y) : new Vector2(position.X, (float) ((double) position.Y - 32.0 - 8.0));

    public void finish() => this.finishEvent.Fire();

    private void doFinish()
    {
      if (this.lastUser == null)
        return;
      this.lastUser.usingSlingshot = false;
      this.lastUser.canReleaseTool = true;
      this.lastUser.UsingTool = false;
      this.lastUser.canMove = true;
      this.lastUser.Halt();
      if (this.lastUser != Game1.player || !Game1.options.gamepadControls)
        return;
      Game1.game1.controllerSlingshotSafeTime = 0.2f;
    }

    public override bool canThisBeAttached(StardewValley.Object o) => o == null || !(bool) (NetFieldBase<bool, NetBool>) o.bigCraftable && ((int) (NetFieldBase<int, NetInt>) o.parentSheetIndex >= 378 && (int) (NetFieldBase<int, NetInt>) o.parentSheetIndex <= 390 || o.Category == -5 || o.Category == -79 || o.Category == -75 || (int) (NetFieldBase<int, NetInt>) o.parentSheetIndex == 441);

    public override StardewValley.Object attach(StardewValley.Object o)
    {
      StardewValley.Object attachment = this.attachments[0];
      this.attachments[0] = o;
      Game1.playSound("button1");
      return attachment;
    }

    public override string getHoverBoxText(Item hoveredItem)
    {
      if (hoveredItem != null && hoveredItem is StardewValley.Object && this.canThisBeAttached(hoveredItem as StardewValley.Object))
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14256", (object) this.DisplayName, (object) hoveredItem.DisplayName);
      return hoveredItem == null && this.attachments != null && this.attachments[0] != null ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14258", (object) this.attachments[0].DisplayName) : (string) null;
    }

    public override bool onRelease(GameLocation location, int x, int y, Farmer who)
    {
      this.DoFunction(location, x, y, 1, who);
      return true;
    }

    public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
    {
      who.usingSlingshot = true;
      who.canReleaseTool = false;
      this.mouseDragAmount = 0;
      int num = who.FacingDirection == 3 || who.FacingDirection == 1 ? 1 : (who.FacingDirection == 0 ? 2 : 0);
      who.FarmerSprite.setCurrentFrame(42 + num);
      if (!who.IsLocalPlayer)
        return true;
      Game1.oldMouseState = Game1.input.GetMouseState();
      Game1.lastMousePositionBeforeFade = Game1.getMousePosition();
      this.lastClickX = Game1.getOldMouseX() + Game1.viewport.X;
      this.lastClickY = Game1.getOldMouseY() + Game1.viewport.Y;
      this.pullStartTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;
      if (this.CanAutoFire())
        this.nextAutoFire = -1f;
      this.updateAimPos();
      return true;
    }

    public virtual float GetAutoFireRate() => 0.3f;

    public virtual bool CanAutoFire() => false;

    private void updateAimPos()
    {
      if (this.lastUser == null || !this.lastUser.IsLocalPlayer)
        return;
      Point point = Game1.getMousePosition();
      if (Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse)
      {
        Vector2 vector2_1 = Game1.oldPadState.ThumbSticks.Left;
        if ((double) vector2_1.Length() < 0.25)
        {
          vector2_1.X = 0.0f;
          vector2_1.Y = 0.0f;
          GamePadDPad dpad = Game1.oldPadState.DPad;
          if (dpad.Down == ButtonState.Pressed)
          {
            vector2_1.Y = -1f;
          }
          else
          {
            dpad = Game1.oldPadState.DPad;
            if (dpad.Up == ButtonState.Pressed)
              vector2_1.Y = 1f;
          }
          dpad = Game1.oldPadState.DPad;
          if (dpad.Left == ButtonState.Pressed)
            vector2_1.X = -1f;
          dpad = Game1.oldPadState.DPad;
          if (dpad.Right == ButtonState.Pressed)
            vector2_1.X = 1f;
          if ((double) vector2_1.X != 0.0 && (double) vector2_1.Y != 0.0)
          {
            vector2_1.Normalize();
            vector2_1 *= 1f;
          }
        }
        Vector2 shootOrigin = this.GetShootOrigin(this.lastUser);
        if (!Game1.options.useLegacySlingshotFiring && (double) vector2_1.Length() < 0.25)
        {
          if ((int) this.lastUser.facingDirection == 3)
            vector2_1 = new Vector2(-1f, 0.0f);
          else if ((int) this.lastUser.facingDirection == 1)
            vector2_1 = new Vector2(1f, 0.0f);
          else if ((int) this.lastUser.facingDirection == 0)
            vector2_1 = new Vector2(0.0f, 1f);
          else if ((int) this.lastUser.facingDirection == 2)
            vector2_1 = new Vector2(0.0f, -1f);
        }
        Vector2 vector2_2 = new Vector2(vector2_1.X, -vector2_1.Y) * 600f;
        point = Utility.Vector2ToPoint(shootOrigin + vector2_2);
        point.X -= Game1.viewport.X;
        point.Y -= Game1.viewport.Y;
      }
      int num1 = point.X + Game1.viewport.X;
      int num2 = point.Y + Game1.viewport.Y;
      this.aimPos.X = num1;
      this.aimPos.Y = num2;
    }

    public override void tickUpdate(GameTime time, Farmer who)
    {
      this.lastUser = who;
      this.finishEvent.Poll();
      if (!who.usingSlingshot)
        return;
      if (who.IsLocalPlayer)
      {
        this.updateAimPos();
        int x = this.aimPos.X;
        int y = this.aimPos.Y;
        Game1.debugOutput = "playerPos: " + who.getStandingPosition().ToString() + ", mousePos: " + x.ToString() + ", " + y.ToString();
        ++this.mouseDragAmount;
        if (!Game1.options.useLegacySlingshotFiring)
        {
          Vector2 shootOrigin = this.GetShootOrigin(who);
          Vector2 vector2 = this.AdjustForHeight(new Vector2((float) x, (float) y)) - shootOrigin;
          if ((double) Math.Abs(vector2.X) > (double) Math.Abs(vector2.Y))
          {
            if ((double) vector2.X < 0.0)
              who.faceDirection(3);
            if ((double) vector2.X > 0.0)
              who.faceDirection(1);
          }
          else
          {
            if ((double) vector2.Y < 0.0)
              who.faceDirection(0);
            if ((double) vector2.Y > 0.0)
              who.faceDirection(2);
          }
        }
        else
          who.faceGeneralDirection(new Vector2((float) x, (float) y), opposite: true);
        if (!Game1.options.useLegacySlingshotFiring)
        {
          if (this.canPlaySound && (double) this.GetSlingshotChargeTime() >= 1.0)
          {
            who.currentLocation.playSound("slingshot");
            this.canPlaySound = false;
          }
        }
        else if (this.canPlaySound && (Math.Abs(x - this.lastClickX) > 8 || Math.Abs(y - this.lastClickY) > 8) && this.mouseDragAmount > 4)
        {
          who.currentLocation.playSound("slingshot");
          this.canPlaySound = false;
        }
        if (!this.CanAutoFire())
        {
          this.lastClickX = x;
          this.lastClickY = y;
        }
        if (Game1.options.useLegacySlingshotFiring)
          Game1.mouseCursor = -1;
        if (this.CanAutoFire())
        {
          bool flag = false;
          if (this.GetBackArmDistance(who) >= 20 && (double) this.nextAutoFire < 0.0)
          {
            this.nextAutoFire = 0.0f;
            flag = true;
          }
          if ((double) this.nextAutoFire > 0.0 | flag)
          {
            this.nextAutoFire -= (float) time.ElapsedGameTime.TotalSeconds;
            if ((double) this.nextAutoFire <= 0.0)
            {
              this.PerformFire(who.currentLocation, who);
              this.nextAutoFire = this.GetAutoFireRate();
            }
          }
        }
      }
      int num = who.FacingDirection == 3 || who.FacingDirection == 1 ? 1 : (who.FacingDirection == 0 ? 2 : 0);
      who.FarmerSprite.setCurrentFrame(42 + num);
    }

    public override void drawAttachments(SpriteBatch b, int x, int y)
    {
      if (this.attachments[0] == null)
      {
        b.Draw(Game1.menuTexture, new Vector2((float) x, (float) y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 43)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
      }
      else
      {
        b.Draw(Game1.menuTexture, new Vector2((float) x, (float) y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
        this.attachments[0].drawInMenu(b, new Vector2((float) x, (float) y), 1f);
      }
    }

    public float GetSlingshotChargeTime() => this.pullStartTime < 0.0 ? 0.0f : Utility.Clamp((float) (Game1.currentGameTime.TotalGameTime.TotalSeconds - this.pullStartTime) / this.GetRequiredChargeTime(), 0.0f, 1f);

    public float GetRequiredChargeTime() => 0.3f;

    public override void draw(SpriteBatch b)
    {
      if (!this.lastUser.usingSlingshot || !this.lastUser.IsLocalPlayer)
        return;
      int x = this.aimPos.X;
      int y = this.aimPos.Y;
      Vector2 shootOrigin = this.GetShootOrigin(this.lastUser);
      Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(shootOrigin, this.AdjustForHeight(new Vector2((float) x, (float) y)), 256f);
      double num1 = Math.Sqrt((double) velocityTowardPoint.X * (double) velocityTowardPoint.X + (double) velocityTowardPoint.Y * (double) velocityTowardPoint.Y) - 181.0;
      double num2 = (double) velocityTowardPoint.X / 256.0;
      double num3 = (double) velocityTowardPoint.Y / 256.0;
      int num4 = (int) ((double) velocityTowardPoint.X - num1 * num2);
      int num5 = (int) ((double) velocityTowardPoint.Y - num1 * num3);
      if (!Game1.options.useLegacySlingshotFiring)
      {
        num4 *= -1;
        num5 *= -1;
      }
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(shootOrigin.X - (float) num4, shootOrigin.Y - (float) num5)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 43)), Color.White, 0.0f, new Vector2(32f, 32f), 1f, SpriteEffects.None, 0.999999f);
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow)
    {
      if (this.IndexOfMenuItemView == 0 || this.IndexOfMenuItemView == 21 || this.IndexOfMenuItemView == 47 || this.CurrentParentTileIndex == 47)
      {
        string baseName = this.BaseName;
        if (!(baseName == nameof (Slingshot)))
        {
          if (!(baseName == "Master Slingshot"))
          {
            if (baseName == "Galaxy Slingshot")
              this.CurrentParentTileIndex = 34;
          }
          else
            this.CurrentParentTileIndex = 33;
        }
        else
          this.CurrentParentTileIndex = 32;
        this.IndexOfMenuItemView = this.CurrentParentTileIndex;
      }
      spriteBatch.Draw(Tool.weaponsTexture, location + new Vector2(32f, 29f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, this.IndexOfMenuItemView, 16, 16)), color * transparency, 0.0f, new Vector2(8f, 8f), scaleSize * 4f, SpriteEffects.None, layerDepth);
      if (drawStackNumber == StackDrawType.Hide || this.attachments == null || this.attachments[0] == null)
        return;
      Utility.drawTinyDigits(this.attachments[0].Stack, spriteBatch, location + new Vector2((float) (64 - Utility.getWidthOfTinyDigitString(this.attachments[0].Stack, 3f * scaleSize)) + 3f * scaleSize, (float) (64.0 - 18.0 * (double) scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
    }
  }
}
