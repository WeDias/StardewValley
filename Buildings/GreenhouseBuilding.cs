// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.GreenhouseBuilding
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewValley.Buildings
{
  public class GreenhouseBuilding : Building
  {
    protected Farm _farm;

    public GreenhouseBuilding(BluePrint b, Vector2 tileLocation)
      : base(b, tileLocation)
    {
    }

    protected override GameLocation getIndoors(string nameOfIndoorsWithoutUnique) => (GameLocation) null;

    public GreenhouseBuilding()
    {
    }

    protected override void initNetFields() => base.initNetFields();

    public override void drawInMenu(SpriteBatch b, int x, int y)
    {
      Microsoft.Xna.Framework.Rectangle sourceRect = this.getSourceRect();
      y += 336;
      int num = 22;
      sourceRect.Height -= num;
      sourceRect.Y += num / 2;
      b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Microsoft.Xna.Framework.Rectangle?(sourceRect), (Color) (NetFieldBase<Color, NetColor>) this.color, 0.0f, new Vector2(0.0f, (float) (sourceRect.Height / 2)), 4f, SpriteEffects.None, 0.89f);
    }

    public override Microsoft.Xna.Framework.Rectangle getSourceRect() => new Microsoft.Xna.Framework.Rectangle(0, 160, 112, 160);

    public override void Update(GameTime time) => base.Update(time);

    public override void drawInConstruction(SpriteBatch b)
    {
      float layerDepth = (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000f;
      Microsoft.Xna.Framework.Rectangle sourceRect = this.getSourceRect();
      sourceRect.Y += sourceRect.Height;
      b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64))), new Microsoft.Xna.Framework.Rectangle?(sourceRect), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, (float) sourceRect.Height), 4f, SpriteEffects.None, layerDepth);
    }

    public override void drawBackground(SpriteBatch b)
    {
      base.drawBackground(b);
      if (this.isMoving)
        return;
      this.DrawEntranceTiles(b);
      this.drawShadow(b, -1, -1);
    }

    public override void draw(SpriteBatch b)
    {
      if (this.isMoving)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0 || (int) (NetFieldBase<int, NetInt>) this.newConstructionTimer > 0)
      {
        this.drawInConstruction(b);
      }
      else
      {
        float layerDepth = (float) (((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh) * 64) / 10000f;
        Microsoft.Xna.Framework.Rectangle sourceRect = this.getSourceRect();
        if (!this.GetFarm().greenhouseUnlocked.Value)
          sourceRect.Y -= sourceRect.Height;
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64))), new Microsoft.Xna.Framework.Rectangle?(sourceRect), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, (float) sourceRect.Height), 4f, SpriteEffects.None, layerDepth);
      }
    }

    public Farm GetFarm()
    {
      if (this._farm == null)
        this._farm = Game1.getFarm();
      return this._farm;
    }

    public override string isThereAnythingtoPreventConstruction(
      GameLocation location,
      Vector2 tile_position)
    {
      return (string) null;
    }

    public override bool doesTileHaveProperty(
      int tile_x,
      int tile_y,
      string property_name,
      string layer_name,
      ref string property_value)
    {
      if (this.isMoving)
        return false;
      if (tile_x == (int) (NetFieldBase<int, NetInt>) this.tileX + this.humanDoor.X && tile_y == (int) (NetFieldBase<int, NetInt>) this.tileY + this.humanDoor.Y && layer_name == "Buildings" && property_name == "Action")
      {
        property_value = "WarpGreenhouse";
        return true;
      }
      if (tile_x >= (int) (NetFieldBase<int, NetInt>) this.tileX - 1 && tile_x <= (int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide - 1 && tile_y <= (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh && tile_y >= (int) (NetFieldBase<int, NetInt>) this.tileY || this.CanDrawEntranceTiles() && tile_x >= (int) (NetFieldBase<int, NetInt>) this.tileX + 1 && tile_x <= (int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide - 2 && tile_y == (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh + 1)
      {
        if (this.CanDrawEntranceTiles() && tile_x >= (int) (NetFieldBase<int, NetInt>) this.tileX + this.humanDoor.X - 1 && tile_x <= (int) (NetFieldBase<int, NetInt>) this.tileX + this.humanDoor.X + 1 && tile_y <= (int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh + 1 && tile_y >= (int) (NetFieldBase<int, NetInt>) this.tileY + this.humanDoor.Y + 1)
        {
          if (property_name == "Type" && layer_name == "Back")
          {
            property_value = "Stone";
            return true;
          }
          if (property_name == "NoSpawn" && layer_name == "Back")
          {
            property_value = "All";
            return true;
          }
          if (property_name == "Buildable" && layer_name == "Back")
          {
            property_value = (string) null;
            return true;
          }
        }
        if (property_name == "Buildable" && layer_name == "Back")
        {
          property_value = "T";
          return true;
        }
        if (property_name == "NoSpawn" && layer_name == "Back")
        {
          property_value = "Tree";
          return true;
        }
        if (property_name == "Diggable" && layer_name == "Back")
        {
          property_value = (string) null;
          return true;
        }
      }
      return base.doesTileHaveProperty(tile_x, tile_y, property_name, layer_name, ref property_value);
    }

    public override int GetAdditionalTilePropertyRadius() => 2;

    public virtual bool CanDrawEntranceTiles() => true;

    public virtual void DrawEntranceTiles(SpriteBatch b)
    {
      Map map = this.GetFarm().Map;
      Layer layer = map.GetLayer("Back");
      TileSheet tileSheet = map.GetTileSheet("untitled tile sheet") ?? map.TileSheets[Math.Min(1, map.TileSheets.Count - 1)];
      if (tileSheet == null)
        return;
      Vector2 zero = Vector2.Zero;
      Location location = new Location(0, 0);
      StaticTile staticTile1 = new StaticTile(layer, tileSheet, BlendMode.Alpha, 812);
      if (!this.CanDrawEntranceTiles())
        return;
      float layerDepth = 0.0f;
      Vector2 local1 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + this.humanDoor.Value.X - 1), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + this.humanDoor.Value.Y + 1)) * 64f);
      location.X = (int) local1.X;
      location.Y = (int) local1.Y;
      Game1.mapDisplayDevice.DrawTile((Tile) staticTile1, location, layerDepth);
      location.X += 64;
      Game1.mapDisplayDevice.DrawTile((Tile) staticTile1, location, layerDepth);
      location.X += 64;
      Game1.mapDisplayDevice.DrawTile((Tile) staticTile1, location, layerDepth);
      StaticTile staticTile2 = new StaticTile(layer, tileSheet, BlendMode.Alpha, 838);
      Vector2 local2 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + this.humanDoor.Value.X - 1), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + this.humanDoor.Value.Y + 2)) * 64f);
      location.X = (int) local2.X;
      location.Y = (int) local2.Y;
      Game1.mapDisplayDevice.DrawTile((Tile) staticTile2, location, layerDepth);
      location.X += 64;
      Game1.mapDisplayDevice.DrawTile((Tile) staticTile2, location, layerDepth);
      location.X += 64;
      Game1.mapDisplayDevice.DrawTile((Tile) staticTile2, location, layerDepth);
    }

    public override void drawShadow(SpriteBatch b, int localX = -1, int localY = -1)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(112, 0, 128, 144);
      if (this.CanDrawEntranceTiles())
        rectangle.Y = 144;
      b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (((int) (NetFieldBase<int, NetInt>) this.tileX - 1) * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * (localX == -1 ? (float) (NetFieldBase<float, NetFloat>) this.alpha : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
    }
  }
}
