// Decompiled with JetBrains decompiler
// Type: StardewValley.Torch
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;

namespace StardewValley
{
  public class Torch : Object
  {
    public const float yVelocity = 1f;
    public const float yDissapearLevel = -100f;
    public const double ashChance = 0.015;
    private float color;
    private Vector2[] ashes = new Vector2[3];
    private float smokePuffTimer;

    public Torch()
    {
    }

    public Torch(Vector2 tileLocation, int initialStack)
      : base(tileLocation, 93, initialStack)
    {
      this.boundingBox.Value = new Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 16, 16);
    }

    public Torch(Vector2 tileLocation, int initialStack, int index)
      : base(tileLocation, index, initialStack)
    {
      this.boundingBox.Value = new Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 16, 16);
    }

    public Torch(Vector2 tileLocation, int index, bool bigCraftable)
      : base(tileLocation, index)
    {
      this.boundingBox.Value = new Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 64, 64);
    }

    public override Item getOne()
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        Torch one = new Torch((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, true);
        one.IsRecipe = (bool) (NetFieldBase<bool, NetBool>) this.isRecipe;
        return (Item) one;
      }
      Torch one1 = new Torch((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, 1);
      one1.IsRecipe = (bool) (NetFieldBase<bool, NetBool>) this.isRecipe;
      one1._GetOneFrom((Item) this);
      return (Item) one1;
    }

    public override void actionOnPlayerEntry()
    {
      base.actionOnPlayerEntry();
      if (!(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable || !(bool) (NetFieldBase<bool, NetBool>) this.isOn)
        return;
      AmbientLocationSounds.addSound((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, 1);
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
        return base.checkForAction(who, justCheckingForActivity);
      if (justCheckingForActivity)
        return true;
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 278)
      {
        Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);
        Game1.activeClickableMenu = (IClickableMenu) new CraftingPage((int) centeringOnScreen.X, (int) centeringOnScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true, true);
        return true;
      }
      this.isOn.Value = !(bool) (NetFieldBase<bool, NetBool>) this.isOn;
      if ((bool) (NetFieldBase<bool, NetBool>) this.isOn)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
        {
          if (who != null)
            Game1.playSound("fireball");
          this.initializeLightSource((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
          AmbientLocationSounds.addSound((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, 1);
        }
      }
      else if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        this.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, Game1.currentLocation);
        if (who != null)
          Game1.playSound("woodyHit");
      }
      return true;
    }

    public override bool placementAction(GameLocation location, int x, int y, Farmer who)
    {
      Vector2 vector2 = new Vector2((float) (x / 64), (float) (y / 64));
      Torch torch = (bool) (NetFieldBase<bool, NetBool>) this.bigCraftable ? new Torch(vector2, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, true) : new Torch(vector2, 1, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex);
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
        torch.isOn.Value = false;
      torch.tileLocation.Value = vector2;
      torch.initializeLightSource(vector2);
      location.objects.Add(vector2, (Object) torch);
      if (who != null)
        Game1.playSound("woodyStep");
      return true;
    }

    public override void DayUpdate(GameLocation location) => base.DayUpdate(location);

    public override bool isPassable() => !(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable;

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      base.updateWhenCurrentLocation(time, environment);
      this.updateAshes((int) ((double) this.tileLocation.X * 2000.0 + (double) this.tileLocation.Y));
      this.smokePuffTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.smokePuffTimer > 0.0)
        return;
      this.smokePuffTimer = 1000f;
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 278)
        return;
      Utility.addSmokePuff(environment, this.tileLocation.Value * 64f + new Vector2(32f, -32f));
    }

    public override void actionWhenBeingHeld(Farmer who) => base.actionWhenBeingHeld(who);

    private void updateAshes(int identifier)
    {
      if (!Utility.isOnScreen(this.tileLocation.Value * 64f, 256))
        return;
      for (int index = this.ashes.Length - 1; index >= 0; --index)
      {
        Vector2 ash = this.ashes[index];
        ash.Y -= (float) (1.0 * ((double) (index + 1) * 0.25));
        if (index % 2 != 0)
          ash.X += (float) Math.Sin((double) this.ashes[index].Y / (2.0 * Math.PI)) / 2f;
        this.ashes[index] = ash;
        if (Game1.random.NextDouble() < 3.0 / 400.0 && (double) this.ashes[index].Y < -100.0)
          this.ashes[index] = new Vector2((float) (Game1.random.Next(-1, 3) * 4) * 0.75f, 0.0f);
      }
      this.color = Math.Max(-0.8f, Math.Min(0.7f, this.color + this.ashes[0].Y / 1200f));
    }

    public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
    {
      AmbientLocationSounds.removeSound((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
        this.isOn.Value = false;
      base.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, environment);
    }

    public override void draw(
      SpriteBatch spriteBatch,
      int xNonTile,
      int yNonTile,
      float layerDepth,
      float alpha = 1f)
    {
      Rectangle sourceRectForObject = GameLocation.getSourceRectForObject(this.ParentSheetIndex);
      sourceRectForObject.Y += 8;
      sourceRectForObject.Height /= 2;
      spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) xNonTile, (float) (yNonTile + 32))), new Rectangle?(sourceRectForObject), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
      sourceRectForObject.X = 276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (xNonTile * 320) + (double) (yNonTile * 49)) % 700.0 / 100.0) * 8;
      sourceRectForObject.Y = 1965;
      sourceRectForObject.Width = 8;
      sourceRectForObject.Height = 8;
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (xNonTile + 32 + 4), (float) (yNonTile + 16 + 4))), new Rectangle?(sourceRectForObject), Color.White * 0.75f, 0.0f, new Vector2(4f, 4f), 3f, SpriteEffects.None, layerDepth + 1E-05f);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (xNonTile + 32 + 4), (float) (yNonTile + 16 + 4))), new Rectangle?(new Rectangle(88, 1779, 30, 30)), Color.PaleGoldenrod * (Game1.currentLocation.IsOutdoors ? 0.35f : 0.43f), 0.0f, new Vector2(15f, 15f), (float) (8.0 + 32.0 * Math.Sin((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (xNonTile * 777) + (double) (yNonTile * 9746)) % 3140.0 / 1000.0) / 50.0), SpriteEffects.None, 1f);
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      if (Game1.eventUp && (Game1.currentLocation == null || Game1.currentLocation.currentEvent == null || !Game1.currentLocation.currentEvent.showGroundObjects) && !Game1.currentLocation.IsFarm)
        return;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        Rectangle sourceRectForObject = GameLocation.getSourceRectForObject(this.ParentSheetIndex);
        sourceRectForObject.Y += 8;
        sourceRectForObject.Height /= 2;
        SpriteBatch spriteBatch1 = spriteBatch;
        Texture2D objectSpriteSheet1 = Game1.objectSpriteSheet;
        Vector2 local1 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 + 32)));
        Rectangle? sourceRectangle1 = new Rectangle?(sourceRectForObject);
        Color white = Color.White;
        Vector2 zero1 = Vector2.Zero;
        Vector2 scale1 = this.scale;
        double scale2 = (double) this.scale.Y > 1.0 ? (double) this.getScale().Y : 4.0;
        double layerDepth1 = (double) this.getBoundingBox(new Vector2((float) x, (float) y)).Bottom / 10000.0;
        spriteBatch1.Draw(objectSpriteSheet1, local1, sourceRectangle1, white, 0.0f, zero1, (float) scale2, SpriteEffects.None, (float) layerDepth1);
        SpriteBatch spriteBatch2 = spriteBatch;
        Texture2D mouseCursors1 = Game1.mouseCursors;
        Vector2 local2 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32 + 2), (float) (y * 64 + 16)));
        Rectangle? sourceRectangle2 = new Rectangle?(new Rectangle(88, 1779, 30, 30));
        Color color1 = Color.PaleGoldenrod * (Game1.currentLocation.IsOutdoors ? 0.35f : 0.43f);
        Vector2 origin1 = new Vector2(15f, 15f);
        TimeSpan totalGameTime = Game1.currentGameTime.TotalGameTime;
        double scale3 = 4.0 + 64.0 * Math.Sin((totalGameTime.TotalMilliseconds + (double) (x * 64 * 777) + (double) (y * 64 * 9746)) % 3140.0 / 1000.0) / 50.0;
        spriteBatch2.Draw(mouseCursors1, local2, sourceRectangle2, color1, 0.0f, origin1, (float) scale3, SpriteEffects.None, 1f);
        ref Rectangle local3 = ref sourceRectForObject;
        totalGameTime = Game1.currentGameTime.TotalGameTime;
        int num = 276 + (int) ((totalGameTime.TotalMilliseconds + (double) (x * 3204) + (double) (y * 49)) % 700.0 / 100.0) * 8;
        local3.X = num;
        sourceRectForObject.Y = 1965;
        sourceRectForObject.Width = 8;
        sourceRectForObject.Height = 8;
        SpriteBatch spriteBatch3 = spriteBatch;
        Texture2D mouseCursors2 = Game1.mouseCursors;
        Vector2 local4 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32 + 4), (float) (y * 64 + 16 + 4)));
        Rectangle? sourceRectangle3 = new Rectangle?(sourceRectForObject);
        Color color2 = Color.White * 0.75f;
        Vector2 origin2 = new Vector2(4f, 4f);
        Rectangle boundingBox = this.getBoundingBox(new Vector2((float) x, (float) y));
        double layerDepth2 = (double) (boundingBox.Bottom + 1) / 10000.0;
        spriteBatch3.Draw(mouseCursors2, local4, sourceRectangle3, color2, 0.0f, origin2, 3f, SpriteEffects.None, (float) layerDepth2);
        for (int index = 0; index < this.ashes.Length; ++index)
        {
          SpriteBatch spriteBatch4 = spriteBatch;
          Texture2D objectSpriteSheet2 = Game1.objectSpriteSheet;
          Vector2 local5 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32) + this.ashes[index].X, (float) (y * 64 + 32) + this.ashes[index].Y));
          Rectangle? sourceRectangle4 = new Rectangle?(new Rectangle(344 + index % 3, 53, 1, 1));
          Color color3 = Color.White * 0.5f * (float) ((-100.0 - (double) this.ashes[index].Y / 2.0) / -100.0);
          Vector2 zero2 = Vector2.Zero;
          boundingBox = this.getBoundingBox(new Vector2((float) x, (float) y));
          double layerDepth3 = (double) boundingBox.Bottom / 10000.0;
          spriteBatch4.Draw(objectSpriteSheet2, local5, sourceRectangle4, color3, 0.0f, zero2, 3f, SpriteEffects.None, (float) layerDepth3);
        }
      }
      else
      {
        base.draw(spriteBatch, x, y, alpha);
        float num = Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (float) x * 1E-05f;
        if (!(bool) (NetFieldBase<bool, NetBool>) this.isOn)
          return;
        if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 146 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 278)
        {
          spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 16 - 4), (float) (y * 64 - 8))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 3047) + (double) (y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, num + 0.0008f);
          spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32 - 12), (float) (y * 64))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 2047) + (double) (y * 98)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, num + 0.0009f);
          spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32 - 20), (float) (y * 64 + 12))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 2077) + (double) (y * 98)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, num + 1f / 1000f);
          if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 278)
            return;
          Rectangle rectForBigCraftable = Object.getSourceRectForBigCraftable(this.ParentSheetIndex + 1);
          rectForBigCraftable.Height -= 16;
          Vector2 vector2 = this.getScale() * 4f;
          Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64 + 12)));
          Rectangle destinationRectangle = new Rectangle((int) ((double) local.X - (double) vector2.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y - (double) vector2.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (64.0 + (double) vector2.X), (int) (64.0 + (double) vector2.Y / 2.0));
          spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, new Rectangle?(rectForBigCraftable), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, num + 0.0028f);
        }
        else
          spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 16 - 8), (float) (y * 64 - 64 + 8))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 3047) + (double) (y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num + 0.0008f);
      }
    }
  }
}
