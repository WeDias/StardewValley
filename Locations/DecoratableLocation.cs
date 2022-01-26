// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.DecoratableLocation
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.GameData;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley.Locations
{
  public class DecoratableLocation : GameLocation
  {
    public readonly DecorationFacade wallPaper = new DecorationFacade();
    [XmlIgnore]
    public readonly NetStringList wallpaperIDs = new NetStringList();
    public readonly NetStringDictionary<string, NetString> appliedWallpaper = new NetStringDictionary<string, NetString>();
    [XmlIgnore]
    public readonly Dictionary<string, List<Vector3>> wallpaperTiles = new Dictionary<string, List<Vector3>>();
    public readonly DecorationFacade floor = new DecorationFacade();
    [XmlIgnore]
    public readonly NetStringList floorIDs = new NetStringList();
    public readonly NetStringDictionary<string, NetString> appliedFloor = new NetStringDictionary<string, NetString>();
    [XmlIgnore]
    public readonly Dictionary<string, List<Vector3>> floorTiles = new Dictionary<string, List<Vector3>>();
    protected Dictionary<string, TileSheet> _wallAndFloorTileSheets = new Dictionary<string, TileSheet>();
    protected Map _wallAndFloorTileSheetMap;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.appliedWallpaper.InterpolationWait = false;
      this.appliedFloor.InterpolationWait = false;
      this.NetFields.AddFields((INetSerializable) this.appliedWallpaper, (INetSerializable) this.appliedFloor, (INetSerializable) this.floorIDs, (INetSerializable) this.wallpaperIDs);
      this.appliedWallpaper.OnValueAdded += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsChangeEvent) ((key, value) => this.UpdateWallpaper(key));
      this.appliedWallpaper.OnConflictResolve += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ConflictResolveEvent) ((key, rejected, accepted) => this.UpdateWallpaper(key));
      this.appliedWallpaper.OnValueTargetUpdated += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsUpdateEvent) ((key, old_value, new_value) =>
      {
        if (this.appliedWallpaper.FieldDict.ContainsKey(key))
          this.appliedWallpaper.FieldDict[key].CancelInterpolation();
        this.UpdateWallpaper(key);
      });
      this.appliedFloor.OnValueAdded += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsChangeEvent) ((key, value) => this.UpdateFloor(key));
      this.appliedFloor.OnConflictResolve += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ConflictResolveEvent) ((key, rejected, accepted) => this.UpdateFloor(key));
      this.appliedFloor.OnValueTargetUpdated += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsUpdateEvent) ((key, old_value, new_value) =>
      {
        if (this.appliedFloor.FieldDict.ContainsKey(key))
          this.appliedFloor.FieldDict[key].CancelInterpolation();
        this.UpdateFloor(key);
      });
    }

    public DecoratableLocation()
    {
    }

    public DecoratableLocation(string mapPath, string name)
      : base(mapPath, name)
    {
    }

    public virtual void ReadWallpaperAndFloorTileData()
    {
      this.updateMap();
      this.wallpaperTiles.Clear();
      this.floorTiles.Clear();
      this.wallpaperIDs.Clear();
      this.floorIDs.Clear();
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (this.map.Properties.ContainsKey("WallIDs"))
      {
        foreach (string str in this.map.Properties["WallIDs"].ToString().Split(','))
        {
          string[] strArray = str.Trim().Split(' ');
          if (strArray.Length >= 1)
            this.wallpaperIDs.Add(strArray[0]);
          if (strArray.Length >= 2)
            dictionary[strArray[0]] = strArray[1];
        }
      }
      KeyValuePair<int, int> keyValuePair;
      if (this.wallpaperIDs.Count == 0)
      {
        List<Microsoft.Xna.Framework.Rectangle> walls = this.getWalls();
        for (int index = 0; index < walls.Count; ++index)
        {
          string key = "Wall_" + index.ToString();
          this.wallpaperIDs.Add(key);
          Microsoft.Xna.Framework.Rectangle rectangle = walls[index];
          if (!this.wallpaperTiles.ContainsKey(index.ToString()))
            this.wallpaperTiles[key] = new List<Vector3>();
          for (int left = rectangle.Left; left < rectangle.Right; ++left)
          {
            for (int top = rectangle.Top; top < rectangle.Bottom; ++top)
              this.wallpaperTiles[key].Add(new Vector3((float) left, (float) top, (float) (top - rectangle.Top)));
          }
        }
      }
      else
      {
        for (int index1 = 0; index1 < this.map.Layers[0].LayerWidth; ++index1)
        {
          for (int index2 = 0; index2 < this.map.Layers[0].LayerHeight; ++index2)
          {
            string key = this.doesTileHaveProperty(index1, index2, "WallID", "Back");
            this.getTileIndexAt(new Point(index1, index2), "Back");
            if (key != null)
            {
              if (!this.wallpaperIDs.Contains(key))
                this.wallpaperIDs.Add(key);
              if (!this.appliedWallpaper.ContainsKey(key))
              {
                this.appliedWallpaper[key] = "0";
                if (dictionary.ContainsKey(key))
                {
                  string str = dictionary[key];
                  if (this.appliedWallpaper.ContainsKey(str))
                  {
                    this.appliedWallpaper[key] = this.appliedWallpaper[str];
                  }
                  else
                  {
                    keyValuePair = this.GetWallpaperSource(str);
                    if (keyValuePair.Value >= 0)
                      this.appliedWallpaper[key] = str;
                  }
                }
              }
              if (!this.wallpaperTiles.ContainsKey(key))
                this.wallpaperTiles[key] = new List<Vector3>();
              this.wallpaperTiles[key].Add(new Vector3((float) index1, (float) index2, 0.0f));
              int sheetWidth = this.map.GetTileSheet(this.getTileSheetIDAt(index1, index2, "Back")).SheetWidth;
              if (this.IsFloorableOrWallpaperableTile(index1, index2 + 1, "Back"))
                this.wallpaperTiles[key].Add(new Vector3((float) index1, (float) (index2 + 1), 1f));
              if (this.IsFloorableOrWallpaperableTile(index1, index2 + 2, "Buildings"))
                this.wallpaperTiles[key].Add(new Vector3((float) index1, (float) (index2 + 2), 2f));
              else if (this.IsFloorableOrWallpaperableTile(index1, index2 + 2, "Back") && !this.IsFloorableTile(index1, index2 + 2, "Back"))
                this.wallpaperTiles[key].Add(new Vector3((float) index1, (float) (index2 + 2), 2f));
            }
          }
        }
      }
      dictionary.Clear();
      if (this.map.Properties.ContainsKey("FloorIDs"))
      {
        foreach (string str in this.map.Properties["FloorIDs"].ToString().Split(','))
        {
          string[] strArray = str.Trim().Split(' ');
          if (strArray.Length >= 1)
            this.floorIDs.Add(strArray[0]);
          if (strArray.Length >= 2)
            dictionary[strArray[0]] = strArray[1];
        }
      }
      if (this.floorIDs.Count == 0)
      {
        List<Microsoft.Xna.Framework.Rectangle> floors = this.getFloors();
        for (int index = 0; index < floors.Count; ++index)
        {
          string key = "Floor_" + index.ToString();
          this.floorIDs.Add(key);
          Microsoft.Xna.Framework.Rectangle rectangle = floors[index];
          if (!this.floorTiles.ContainsKey(index.ToString()))
            this.floorTiles[key] = new List<Vector3>();
          for (int left = rectangle.Left; left < rectangle.Right; ++left)
          {
            for (int top = rectangle.Top; top < rectangle.Bottom; ++top)
              this.floorTiles[key].Add(new Vector3((float) left, (float) top, 0.0f));
          }
        }
      }
      else
      {
        for (int index3 = 0; index3 < this.map.Layers[0].LayerWidth; ++index3)
        {
          for (int index4 = 0; index4 < this.map.Layers[0].LayerHeight; ++index4)
          {
            string key = this.doesTileHaveProperty(index3, index4, "FloorID", "Back");
            this.getTileIndexAt(new Point(index3, index4), "Back");
            if (key != null)
            {
              if (!this.floorIDs.Contains(key))
                this.floorIDs.Add(key);
              if (!this.appliedFloor.ContainsKey(key))
              {
                this.appliedFloor[key] = "0";
                if (dictionary.ContainsKey(key))
                {
                  string str = dictionary[key];
                  if (this.appliedFloor.ContainsKey(str))
                  {
                    this.appliedFloor[key] = this.appliedFloor[str];
                  }
                  else
                  {
                    keyValuePair = this.GetFloorSource(str);
                    if (keyValuePair.Value >= 0)
                      this.appliedFloor[key] = str;
                  }
                }
              }
              if (!this.floorTiles.ContainsKey(key))
                this.floorTiles[key] = new List<Vector3>();
              this.floorTiles[key].Add(new Vector3((float) index3, (float) index4, 0.0f));
            }
          }
        }
      }
      this.setFloors();
      this.setWallpapers();
    }

    public virtual TileSheet GetWallAndFloorTilesheet(string id)
    {
      if (this.map != this._wallAndFloorTileSheetMap)
      {
        this._wallAndFloorTileSheets.Clear();
        this._wallAndFloorTileSheetMap = this.map;
      }
      if (this._wallAndFloorTileSheets.ContainsKey(id))
        return this._wallAndFloorTileSheets[id];
      try
      {
        List<ModWallpaperOrFlooring> wallpaperOrFlooringList = Game1.content.Load<List<ModWallpaperOrFlooring>>("Data\\AdditionalWallpaperFlooring");
        ModWallpaperOrFlooring wallpaperOrFlooring1 = (ModWallpaperOrFlooring) null;
        foreach (ModWallpaperOrFlooring wallpaperOrFlooring2 in wallpaperOrFlooringList)
        {
          if (wallpaperOrFlooring2.ID == id)
          {
            wallpaperOrFlooring1 = wallpaperOrFlooring2;
            break;
          }
        }
        if (wallpaperOrFlooring1 != null)
        {
          Texture2D texture2D = Game1.content.Load<Texture2D>(wallpaperOrFlooring1.Texture);
          if (texture2D.Width / 16 != 16)
            Console.WriteLine("WARNING: Wallpaper/floor tilesheets must be 16 tiles wide.");
          TileSheet tileSheet = new TileSheet("x_WallsAndFloors_" + id, this.map, wallpaperOrFlooring1.Texture, new Size(texture2D.Width / 16, texture2D.Height / 16), new Size(16, 16));
          this.map.AddTileSheet(tileSheet);
          this.map.LoadTileSheets(Game1.mapDisplayDevice);
          this._wallAndFloorTileSheets[id] = tileSheet;
          return tileSheet;
        }
        Console.WriteLine("Error trying to load wallpaper/floor tilesheet: " + id);
        this._wallAndFloorTileSheets[id] = (TileSheet) null;
        return (TileSheet) null;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error trying to load wallpaper/floor tilesheet: " + id);
        this._wallAndFloorTileSheets[id] = (TileSheet) null;
        return (TileSheet) null;
      }
    }

    public virtual KeyValuePair<int, int> GetFloorSource(string pattern_id)
    {
      int result = -1;
      if (pattern_id.Contains(":"))
      {
        string[] strArray = pattern_id.Split(':');
        TileSheet andFloorTilesheet = this.GetWallAndFloorTilesheet(strArray[0]);
        if (int.TryParse(strArray[1], out result) && andFloorTilesheet != null)
          return new KeyValuePair<int, int>(this.map.TileSheets.IndexOf(andFloorTilesheet), result);
      }
      return int.TryParse(pattern_id, out result) ? new KeyValuePair<int, int>(this.map.TileSheets.IndexOf(this.map.GetTileSheet("walls_and_floors")), result) : new KeyValuePair<int, int>(-1, -1);
    }

    public virtual KeyValuePair<int, int> GetWallpaperSource(string pattern_id)
    {
      int result = -1;
      if (pattern_id.Contains(":"))
      {
        string[] strArray = pattern_id.Split(':');
        TileSheet andFloorTilesheet = this.GetWallAndFloorTilesheet(strArray[0]);
        if (int.TryParse(strArray[1], out result) && andFloorTilesheet != null)
          return new KeyValuePair<int, int>(this.map.TileSheets.IndexOf(andFloorTilesheet), result);
      }
      return int.TryParse(pattern_id, out result) ? new KeyValuePair<int, int>(this.map.TileSheets.IndexOf(this.map.GetTileSheet("walls_and_floors")), result) : new KeyValuePair<int, int>(-1, -1);
    }

    public virtual void UpdateFloor(string floor_id)
    {
      this.updateMap();
      if (!this.appliedFloor.ContainsKey(floor_id) || !this.floorTiles.ContainsKey(floor_id))
        return;
      string pattern_id = this.appliedFloor[floor_id];
      foreach (Vector3 vector3 in this.floorTiles[floor_id])
      {
        int x = (int) vector3.X;
        int y = (int) vector3.Y;
        KeyValuePair<int, int> floorSource = this.GetFloorSource(pattern_id);
        if (floorSource.Value >= 0)
        {
          int key = floorSource.Key;
          int num = floorSource.Value;
          int sheetWidth = this.map.TileSheets[key].SheetWidth;
          string id = this.map.TileSheets[key].Id;
          string str = "Back";
          int base_tile_sheet = num * 2 + num / (sheetWidth / 2) * sheetWidth;
          if (id == "walls_and_floors")
            base_tile_sheet += this.GetFirstFlooringTile();
          if (this.IsFloorableOrWallpaperableTile(x, y, str))
          {
            Tile tile1 = this.map.GetLayer(str).Tiles[x, y];
            this.setMapTile(x, y, this.GetFlooringIndex(base_tile_sheet, x, y), str, (string) null, key);
            Tile tile2 = this.map.GetLayer(str).Tiles[x, y];
            if (tile1 != null)
            {
              foreach (KeyValuePair<string, PropertyValue> property in (IEnumerable<KeyValuePair<string, PropertyValue>>) tile1.Properties)
                tile2.Properties[property.Key] = property.Value;
            }
          }
        }
      }
    }

    public virtual void UpdateWallpaper(string wallpaper_id)
    {
      this.updateMap();
      if (!this.appliedWallpaper.ContainsKey(wallpaper_id) || !this.wallpaperTiles.ContainsKey(wallpaper_id))
        return;
      string pattern_id = this.appliedWallpaper[wallpaper_id];
      foreach (Vector3 vector3 in this.wallpaperTiles[wallpaper_id])
      {
        int x = (int) vector3.X;
        int y = (int) vector3.Y;
        int z = (int) vector3.Z;
        KeyValuePair<int, int> wallpaperSource = this.GetWallpaperSource(pattern_id);
        if (wallpaperSource.Value >= 0)
        {
          int key = wallpaperSource.Key;
          int num = wallpaperSource.Value;
          int sheetWidth = this.map.TileSheets[key].SheetWidth;
          string str = "Back";
          if (z == 2)
          {
            str = "Buildings";
            if (!this.IsFloorableOrWallpaperableTile(x, y, "Buildings"))
              str = "Back";
          }
          if (this.IsFloorableOrWallpaperableTile(x, y, str))
          {
            Tile tile1 = this.map.GetLayer(str).Tiles[x, y];
            this.setMapTile(x, y, num / sheetWidth * sheetWidth * 3 + num % sheetWidth + z * sheetWidth, str, (string) null, key);
            Tile tile2 = this.map.GetLayer(str).Tiles[x, y];
            if (tile1 != null)
            {
              foreach (KeyValuePair<string, PropertyValue> property in (IEnumerable<KeyValuePair<string, PropertyValue>>) tile1.Properties)
                tile2.Properties[property.Key] = property.Value;
            }
          }
        }
      }
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      if (this.wasUpdated)
        return;
      base.UpdateWhenCurrentLocation(time);
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (!(this is FarmHouse))
      {
        this.ReadWallpaperAndFloorTileData();
        this.setWallpapers();
        this.setFloors();
      }
      if (this.getTileIndexAt(Game1.player.getTileX(), Game1.player.getTileY(), "Buildings") == -1)
        return;
      Game1.player.position.Y += 64f;
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (Game1.player.mailReceived.Contains("button_tut_1"))
        return;
      Game1.player.mailReceived.Add("button_tut_1");
      Game1.onScreenMenus.Add((IClickableMenu) new ButtonTutorialMenu(0));
    }

    public override void shiftObjects(int dx, int dy)
    {
      base.shiftObjects(dx, dy);
      foreach (Furniture furniture in this.furniture)
      {
        furniture.removeLights((GameLocation) this);
        furniture.tileLocation.X += (float) dx;
        furniture.tileLocation.Y += (float) dy;
        furniture.boundingBox.X += dx * 64;
        furniture.boundingBox.Y += dy * 64;
        furniture.updateDrawPosition();
        if (Game1.isDarkOut())
          furniture.addLights((GameLocation) this);
      }
      List<KeyValuePair<Vector2, TerrainFeature>> keyValuePairList = new List<KeyValuePair<Vector2, TerrainFeature>>((IEnumerable<KeyValuePair<Vector2, TerrainFeature>>) this.terrainFeatures.Pairs);
      this.terrainFeatures.Clear();
      foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in keyValuePairList)
        this.terrainFeatures.Add(new Vector2(keyValuePair.Key.X + (float) dx, keyValuePair.Key.Y + (float) dy), keyValuePair.Value);
    }

    public void moveFurniture(int oldX, int oldY, int newX, int newY)
    {
      Vector2 key = new Vector2((float) oldX, (float) oldY);
      foreach (Furniture furniture in this.furniture)
      {
        if (furniture.tileLocation.Equals((object) key))
        {
          furniture.removeLights((GameLocation) this);
          furniture.tileLocation.Value = new Vector2((float) newX, (float) newY);
          furniture.boundingBox.X = newX * 64;
          furniture.boundingBox.Y = newY * 64;
          furniture.updateDrawPosition();
          if (!Game1.isDarkOut())
            return;
          furniture.addLights((GameLocation) this);
          return;
        }
      }
      if (!this.objects.ContainsKey(key))
        return;
      StardewValley.Object @object = this.objects[key];
      this.objects.Remove(key);
      @object.tileLocation.Value = new Vector2((float) newX, (float) newY);
      this.objects.Add(new Vector2((float) newX, (float) newY), @object);
    }

    public override bool CanFreePlaceFurniture() => true;

    public virtual bool isTileOnWall(int x, int y)
    {
      foreach (string key in this.wallpaperTiles.Keys)
      {
        foreach (Vector3 vector3 in this.wallpaperTiles[key])
        {
          if ((int) vector3.X == x && (int) vector3.Y == y)
            return true;
        }
      }
      return false;
    }

    public int GetWallTopY(int x, int y)
    {
      foreach (string key in this.wallpaperTiles.Keys)
      {
        foreach (Vector3 vector3 in this.wallpaperTiles[key])
        {
          if ((int) vector3.X == x && (int) vector3.Y == y)
            return y - (int) vector3.Z;
        }
      }
      return -1;
    }

    public virtual void setFloors()
    {
      foreach (KeyValuePair<string, string> pair in this.appliedFloor.Pairs)
        this.UpdateFloor(pair.Key);
    }

    public virtual void setWallpapers()
    {
      foreach (KeyValuePair<string, string> pair in this.appliedWallpaper.Pairs)
        this.UpdateWallpaper(pair.Key);
    }

    public void SetFloor(string which, string which_room)
    {
      if (which_room == null)
      {
        foreach (string floorId in (NetList<string, NetString>) this.floorIDs)
          this.appliedFloor[floorId] = which;
      }
      else
        this.appliedFloor[which_room] = which;
    }

    public void SetWallpaper(string which, string which_room)
    {
      if (which_room == null)
      {
        foreach (string wallpaperId in (NetList<string, NetString>) this.wallpaperIDs)
          this.appliedWallpaper[wallpaperId] = which;
      }
      else
        this.appliedWallpaper[which_room] = which;
    }

    public string GetFloorID(int x, int y)
    {
      foreach (string key in this.floorTiles.Keys)
      {
        foreach (Vector3 vector3 in this.floorTiles[key])
        {
          if ((int) vector3.X == x && (int) vector3.Y == y)
            return key;
        }
      }
      return (string) null;
    }

    public string GetWallpaperID(int x, int y)
    {
      foreach (string key in this.wallpaperTiles.Keys)
      {
        foreach (Vector3 vector3 in this.wallpaperTiles[key])
        {
          if ((int) vector3.X == x && (int) vector3.Y == y)
            return key;
        }
      }
      return (string) null;
    }

    [Obsolete("Use string based SetFloor.")]
    public virtual void setFloor(int which, int whichRoom = -1, bool persist = false)
    {
      string which_room = (string) null;
      if (whichRoom >= 0 && whichRoom < this.floorIDs.Count)
        which_room = this.floorIDs[whichRoom];
      this.SetFloor(which.ToString(), which_room);
    }

    [Obsolete("Use string based SetWallpaper.")]
    public void setWallpaper(int which, int whichRoom = -1, bool persist = false)
    {
      string which_room = (string) null;
      if (whichRoom >= 0 && whichRoom < this.wallpaperIDs.Count)
        which_room = this.wallpaperIDs[whichRoom];
      this.SetWallpaper(which.ToString(), which_room);
    }

    protected bool IsFloorableTile(int x, int y, string layer_name)
    {
      switch (this.getTileIndexAt(x, y, "Buildings"))
      {
        case 197:
        case 198:
        case 199:
          if (this.getTileSheetIDAt(x, y, "Buildings") == "untitled tile sheet")
            return false;
          break;
      }
      return this.IsFloorableOrWallpaperableTile(x, y, layer_name);
    }

    public bool IsWallAndFloorTilesheet(string tilesheet_id) => tilesheet_id.StartsWith("x_WallsAndFloors_") || tilesheet_id == "walls_and_floors";

    protected bool IsFloorableOrWallpaperableTile(int x, int y, string layer_name)
    {
      Layer layer = this.map.GetLayer(layer_name);
      return layer != null && x < layer.LayerWidth && y < layer.LayerHeight && layer.Tiles[x, y] != null && layer.Tiles[x, y].TileSheet != null && this.IsWallAndFloorTilesheet(layer.Tiles[x, y].TileSheet.Id);
    }

    public override void drawFloorDecorations(SpriteBatch b) => base.drawFloorDecorations(b);

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      if (l is DecoratableLocation decoratableLocation)
      {
        NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.KeysCollection keys = decoratableLocation.appliedWallpaper.Keys;
        if (!keys.Any())
        {
          this.ReadWallpaperAndFloorTileData();
          for (int index = 0; index < decoratableLocation.wallPaper.Count; ++index)
          {
            try
            {
              this.appliedWallpaper[this.wallpaperIDs[index]] = decoratableLocation.wallPaper[index].ToString();
            }
            catch (Exception ex)
            {
            }
          }
          for (int index = 0; index < decoratableLocation.floor.Count; ++index)
          {
            try
            {
              this.appliedFloor[this.floorIDs[index]] = decoratableLocation.floor[index].ToString();
            }
            catch (Exception ex)
            {
            }
          }
        }
        else
        {
          keys = decoratableLocation.appliedWallpaper.Keys;
          foreach (string key in keys)
            this.appliedWallpaper[key] = decoratableLocation.appliedWallpaper[key];
          foreach (string key in decoratableLocation.appliedFloor.Keys)
            this.appliedFloor[key] = decoratableLocation.appliedFloor[key];
        }
      }
      this.setWallpapers();
      this.setFloors();
      base.TransferDataFromSavedLocation(l);
    }

    public Furniture getRandomFurniture(Random r) => this.furniture.Count > 0 ? this.furniture.ElementAt<Furniture>(r.Next(this.furniture.Count)) : (Furniture) null;

    public virtual int getFloorAt(Point p)
    {
      foreach (string key in this.floorTiles.Keys)
      {
        foreach (Vector3 vector3 in this.floorTiles[key])
        {
          if ((int) vector3.X == p.X && (int) vector3.Y == p.Y)
            return this.floorIDs.IndexOf(key);
        }
      }
      return -1;
    }

    public virtual int getWallForRoomAt(Point p)
    {
      foreach (string key in this.wallpaperTiles.Keys)
      {
        foreach (Vector3 vector3 in this.wallpaperTiles[key])
        {
          if ((int) vector3.X == p.X && (int) vector3.Y == p.Y)
            return this.wallpaperIDs.IndexOf(key);
        }
      }
      return -1;
    }

    public virtual int GetFirstFlooringTile() => 336;

    public virtual int GetFlooringIndex(int base_tile_sheet, int tile_x, int tile_y)
    {
      int tileIndexAt = this.getTileIndexAt(tile_x, tile_y, "Back");
      if (tileIndexAt < 0)
        return 0;
      string tileSheetIdAt = this.getTileSheetIDAt(tile_x, tile_y, "Back");
      TileSheet tileSheet = this.map.GetTileSheet(tileSheetIdAt);
      int num1 = 16;
      if (tileSheet != null)
        num1 = tileSheet.SheetWidth;
      if (tileSheetIdAt == "walls_and_floors")
        tileIndexAt -= this.GetFirstFlooringTile();
      int num2 = tileIndexAt % 2;
      int num3 = tileIndexAt % (num1 * 2) / num1;
      return base_tile_sheet + num2 + num1 * num3;
    }

    [Obsolete("Replaced by SetFloor.")]
    protected virtual void doSetVisibleFloor(int whichRoom, int which) => this.SetFloor(which.ToString(), whichRoom.ToString());

    [Obsolete("Replaced by SetWallpaper.")]
    protected virtual void doSetVisibleWallpaper(int whichRoom, int which) => this.SetWallpaper(which.ToString(), whichRoom.ToString());

    public virtual List<Microsoft.Xna.Framework.Rectangle> getFloors() => new List<Microsoft.Xna.Framework.Rectangle>();
  }
}
