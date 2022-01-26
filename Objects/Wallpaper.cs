// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Wallpaper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.GameData;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class Wallpaper : StardewValley.Object
  {
    [XmlElement("sourceRect")]
    public readonly NetRectangle sourceRect = new NetRectangle();
    [XmlElement("isFloor")]
    public readonly NetBool isFloor = new NetBool(false);
    [XmlElement("sourceTexture")]
    public readonly NetString modDataID = new NetString((string) null);
    protected ModWallpaperOrFlooring _modData;
    private static readonly Rectangle wallpaperContainerRect = new Rectangle(39, 31, 16, 16);
    private static readonly Rectangle floorContainerRect = new Rectangle(55, 31, 16, 16);

    public Wallpaper() => this.NetFields.AddFields((INetSerializable) this.sourceRect, (INetSerializable) this.isFloor, (INetSerializable) this.modDataID);

    public Wallpaper(int which, bool isFloor = false)
      : this()
    {
      this.isFloor.Value = isFloor;
      this.ParentSheetIndex = which;
      this.name = isFloor ? "Flooring" : nameof (Wallpaper);
      this.sourceRect.Value = isFloor ? new Rectangle(which % 8 * 32, 336 + which / 8 * 32, 28, 26) : new Rectangle(which % 16 * 16, which / 16 * 48 + 8, 16, 28);
      this.price.Value = 100;
    }

    public Wallpaper(string mod_id, int which)
      : this()
    {
      this.modDataID.Value = mod_id;
      this.ParentSheetIndex = which;
      if (this.GetModData() != null)
        this.isFloor.Value = this.GetModData().IsFlooring;
      else
        this.modDataID.Value = (string) null;
      this.sourceRect.Value = (bool) (NetFieldBase<bool, NetBool>) this.isFloor ? new Rectangle(which % 8 * 32, 336 + which / 8 * 32, 28, 26) : new Rectangle(which % 16 * 16, which / 16 * 48 + 8, 16, 28);
      if (this.GetModData() != null && this.isFloor.Value)
        this.sourceRect.Y = which / 8 * 32;
      this.name = (bool) (NetFieldBase<bool, NetBool>) this.isFloor ? "Flooring" : nameof (Wallpaper);
      this.price.Value = 100;
    }

    public virtual ModWallpaperOrFlooring GetModData()
    {
      if (this.modDataID.Value == null)
        return (ModWallpaperOrFlooring) null;
      if (this._modData != null)
        return this._modData;
      foreach (ModWallpaperOrFlooring modData in Game1.content.Load<List<ModWallpaperOrFlooring>>("Data\\AdditionalWallpaperFlooring"))
      {
        if (modData.ID == this.modDataID.Value)
        {
          this._modData = modData;
          return modData;
        }
      }
      return (ModWallpaperOrFlooring) null;
    }

    protected override string loadDisplayName() => !(bool) (NetFieldBase<bool, NetBool>) this.isFloor ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13204") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13203");

    public override string getDescription() => !(bool) (NetFieldBase<bool, NetBool>) this.isFloor ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13206") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13205");

    public override bool performDropDownAction(Farmer who) => true;

    public override bool performObjectDropInAction(Item dropIn, bool probe, Farmer who) => false;

    public override bool canBePlacedHere(GameLocation l, Vector2 tile)
    {
      Vector2 vector2 = tile * 64f;
      vector2.X += 32f;
      vector2.Y += 32f;
      foreach (Furniture furniture in l.furniture)
      {
        if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type != 12 && furniture.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture.tileLocation).Contains((int) vector2.X, (int) vector2.Y))
          return false;
      }
      return true;
    }

    public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
    {
      if (who == null)
        who = Game1.player;
      if (who.currentLocation is DecoratableLocation)
      {
        Point point = new Point(x / 64, y / 64);
        DecoratableLocation currentLocation = who.currentLocation as DecoratableLocation;
        if ((bool) (NetFieldBase<bool, NetBool>) this.isFloor)
        {
          string floorId = currentLocation.GetFloorID(point.X, point.Y);
          if (floorId != null)
          {
            if (this.GetModData() != null)
              currentLocation.SetFloor(this.GetModData().ID + ":" + this.parentSheetIndex.ToString(), floorId);
            else
              currentLocation.SetFloor(this.parentSheetIndex.ToString(), floorId);
            location.playSound("coin");
            return true;
          }
        }
        else
        {
          string wallpaperId = currentLocation.GetWallpaperID(point.X, point.Y);
          if (wallpaperId != null)
          {
            if (this.GetModData() != null)
              currentLocation.SetWallpaper(this.GetModData().ID + ":" + this.parentSheetIndex.ToString(), wallpaperId);
            else
              currentLocation.SetWallpaper(this.parentSheetIndex.ToString(), wallpaperId);
            location.playSound("coin");
            return true;
          }
        }
      }
      return false;
    }

    public override bool isPlaceable() => true;

    public override Rectangle getBoundingBox(Vector2 tileLocation) => (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.boundingBox;

    public override int salePrice() => (int) (NetFieldBase<int, NetInt>) this.price;

    public override int maximumStackSize() => 1;

    public override int addToStack(Item stack) => 1;

    public override string Name => this.name;

    public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f) => this.drawInMenu(spriteBatch, objectPosition, 1f);

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
      Texture2D texture;
      if (this.GetModData() != null)
      {
        try
        {
          texture = Game1.content.Load<Texture2D>(this.GetModData().Texture);
        }
        catch (Exception ex)
        {
          texture = Game1.content.Load<Texture2D>("Maps\\walls_and_floors");
        }
      }
      else
        texture = Game1.content.Load<Texture2D>("Maps\\walls_and_floors");
      if ((bool) (NetFieldBase<bool, NetBool>) this.isFloor)
      {
        spriteBatch.Draw(Game1.mouseCursors2, location + new Vector2(32f, 32f), new Rectangle?(Wallpaper.floorContainerRect), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(texture, location + new Vector2(32f, 30f), new Rectangle?((Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.sourceRect), color * transparency, 0.0f, new Vector2(14f, 13f), 2f * scaleSize, SpriteEffects.None, layerDepth + 1f / 1000f);
      }
      else
      {
        spriteBatch.Draw(Game1.mouseCursors2, location + new Vector2(32f, 32f), new Rectangle?(Wallpaper.wallpaperContainerRect), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(texture, location + new Vector2(32f, 32f), new Rectangle?((Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.sourceRect), color * transparency, 0.0f, new Vector2(8f, 14f), 2f * scaleSize, SpriteEffects.None, layerDepth + 1f / 1000f);
      }
    }

    public override Item getOne()
    {
      Wallpaper one = this.GetModData() == null ? new Wallpaper((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, (bool) (NetFieldBase<bool, NetBool>) this.isFloor) : new Wallpaper(this.GetModData().ID, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex);
      one._GetOneFrom((Item) this);
      return (Item) one;
    }
  }
}
