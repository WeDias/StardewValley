// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.ColoredObject
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class ColoredObject : StardewValley.Object
  {
    [XmlElement("color")]
    public readonly NetColor color = new NetColor();
    [XmlElement("colorSameIndexAsParentSheetIndex")]
    public readonly NetBool colorSameIndexAsParentSheetIndex = new NetBool();

    public bool ColorSameIndexAsParentSheetIndex
    {
      get => this.colorSameIndexAsParentSheetIndex.Value;
      set => this.colorSameIndexAsParentSheetIndex.Value = value;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddField((INetSerializable) this.color);
      this.NetFields.AddField((INetSerializable) this.colorSameIndexAsParentSheetIndex);
    }

    public ColoredObject()
    {
    }

    public ColoredObject(int parentSheetIndex, int stack, Color color)
      : base(parentSheetIndex, stack)
    {
      this.color.Value = color;
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color colorOverride,
      bool drawShadow)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.isRecipe)
      {
        transparency = 0.5f;
        scaleSize *= 0.75f;
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)), Color.White * transparency, 0.0f, new Vector2(32f, 64f) * scaleSize, (double) scaleSize < 0.200000002980232 ? scaleSize : scaleSize / 2f, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex + 1)), this.color.Value * transparency, 0.0f, new Vector2(32f, 64f) * scaleSize, (double) scaleSize < 0.200000002980232 ? scaleSize : scaleSize / 2f, SpriteEffects.None, Math.Min(1f, layerDepth + 2E-05f));
      }
      else
      {
        spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 16, 16)), Color.White * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex + ((bool) (NetFieldBase<bool, NetBool>) this.colorSameIndexAsParentSheetIndex ? 0 : 1), 16, 16)), this.color.Value * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, Math.Min(1f, layerDepth + 2E-05f));
        if ((drawStackNumber == StackDrawType.Draw && this.maximumStackSize() > 1 && this.Stack > 1 || drawStackNumber == StackDrawType.Draw_OneInclusive) && (double) scaleSize > 0.3 && this.Stack != int.MaxValue)
          Utility.drawTinyDigits((int) (NetFieldBase<int, NetInt>) this.stack, spriteBatch, location + new Vector2((float) (64 - Utility.getWidthOfTinyDigitString((int) (NetFieldBase<int, NetInt>) this.stack, 3f * scaleSize)) + 3f * scaleSize, (float) (64.0 - 18.0 * (double) scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
        if (drawStackNumber != StackDrawType.Hide && (int) (NetFieldBase<int, NetInt>) this.quality > 0)
        {
          Rectangle rectangle = (int) (NetFieldBase<int, NetInt>) this.quality < 4 ? new Rectangle(338 + ((int) (NetFieldBase<int, NetInt>) this.quality - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8);
          Texture2D mouseCursors = Game1.mouseCursors;
          float num = (int) (NetFieldBase<int, NetInt>) this.quality < 4 ? 0.0f : (float) ((Math.Cos((double) Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
          spriteBatch.Draw(mouseCursors, location + new Vector2(12f, 52f + num), new Rectangle?(rectangle), Color.White * transparency, 0.0f, new Vector2(4f, 4f), (float) (3.0 * (double) scaleSize * (1.0 + (double) num)), SpriteEffects.None, layerDepth);
        }
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isRecipe)
        return;
      spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(16f, 16f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 451, 16, 16)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, Math.Min(1f, layerDepth + 0.0001f));
    }

    public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
    {
      base.drawWhenHeld(spriteBatch, objectPosition, f);
      spriteBatch.Draw(Game1.objectSpriteSheet, objectPosition, new Rectangle?(GameLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex + 1)), (Color) (NetFieldBase<Color, NetColor>) this.color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 4) / 10000f));
    }

    public override Item getOne()
    {
      ColoredObject one = new ColoredObject((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 1, (Color) (NetFieldBase<Color, NetColor>) this.color);
      one.Quality = (int) (NetFieldBase<int, NetInt>) this.quality;
      one.Price = (int) (NetFieldBase<int, NetInt>) this.price;
      one.HasBeenInInventory = this.HasBeenInInventory;
      one.HasBeenPickedUpByFarmer = (bool) (NetFieldBase<bool, NetBool>) this.hasBeenPickedUpByFarmer;
      one.SpecialVariable = this.SpecialVariable;
      one.preserve.Set(this.preserve.Value);
      one.preservedParentSheetIndex.Set(this.preservedParentSheetIndex.Value);
      one.Name = this.Name;
      one.colorSameIndexAsParentSheetIndex.Value = this.colorSameIndexAsParentSheetIndex.Value;
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        Vector2 scale = this.getScale();
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64)));
        Rectangle destinationRectangle = new Rectangle((int) ((double) local.X - (double) scale.X / 2.0), (int) ((double) local.Y - (double) scale.Y / 2.0), (int) (64.0 + (double) scale.X), (int) (128.0 + (double) scale.Y / 2.0));
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable((bool) (NetFieldBase<bool, NetBool>) this.showNextIndex ? this.ParentSheetIndex + 1 : this.ParentSheetIndex)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 - 1) / 10000f));
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable(this.ParentSheetIndex + 1)), (Color) (NetFieldBase<Color, NetColor>) this.color, 0.0f, Vector2.Zero, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 - 1) / 10000f));
        if (this.Name.Equals("Loom") && (int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady > 0)
          spriteBatch.Draw(Game1.objectSpriteSheet, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 0.0f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 435, 16, 16)), Color.White, this.scale.X, new Vector2(32f, 32f), 1f, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 - 1) / 10000f));
      }
      else if (!Game1.eventUp || Game1.currentLocation.IsFarm)
      {
        Rectangle boundingBox;
        if (!this.ColorSameIndexAsParentSheetIndex)
        {
          if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 590)
            spriteBatch.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 53f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 1E-07f);
          SpriteBatch spriteBatch1 = spriteBatch;
          Texture2D objectSpriteSheet = Game1.objectSpriteSheet;
          Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32), (float) (y * 64 + 32)));
          Rectangle? sourceRectangle = new Rectangle?(GameLocation.getSourceRectForObject(this.ParentSheetIndex));
          Color white = Color.White;
          Vector2 origin = new Vector2(8f, 8f);
          Vector2 scale1 = this.scale;
          double scale2 = (double) this.scale.Y > 1.0 ? (double) this.getScale().Y : 4.0;
          int effects = (bool) (NetFieldBase<bool, NetBool>) this.flipped ? 1 : 0;
          boundingBox = this.getBoundingBox(new Vector2((float) x, (float) y));
          double layerDepth = (double) boundingBox.Bottom / 10000.0;
          spriteBatch1.Draw(objectSpriteSheet, local, sourceRectangle, white, 0.0f, origin, (float) scale2, (SpriteEffects) effects, (float) layerDepth);
        }
        SpriteBatch spriteBatch2 = spriteBatch;
        Texture2D objectSpriteSheet1 = Game1.objectSpriteSheet;
        Vector2 local1 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))));
        Rectangle? sourceRectangle1 = new Rectangle?(GameLocation.getSourceRectForObject(this.ParentSheetIndex + ((bool) (NetFieldBase<bool, NetBool>) this.colorSameIndexAsParentSheetIndex ? 0 : 1)));
        Color color = (Color) (NetFieldBase<Color, NetColor>) this.color;
        Vector2 origin1 = new Vector2(8f, 8f);
        Vector2 scale3 = this.scale;
        double scale4 = (double) this.scale.Y > 1.0 ? (double) this.getScale().Y : 4.0;
        int effects1 = (bool) (NetFieldBase<bool, NetBool>) this.flipped ? 1 : 0;
        boundingBox = this.getBoundingBox(new Vector2((float) x, (float) y));
        double layerDepth1 = (double) boundingBox.Bottom / 10000.0;
        spriteBatch2.Draw(objectSpriteSheet1, local1, sourceRectangle1, color, 0.0f, origin1, (float) scale4, (SpriteEffects) effects1, (float) layerDepth1);
      }
      if (this.Name == null || !this.Name.Contains("Table") || this.heldObject.Value == null)
        return;
      spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable ? 48 : 21)))), new Rectangle?(GameLocation.getSourceRectForObject(this.heldObject.Value.ParentSheetIndex)), Color.White, 0.0f, Vector2.Zero, 1f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (y * 64 + 64 + 1) / 10000f);
    }
  }
}
