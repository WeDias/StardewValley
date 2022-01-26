// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.CrabPot
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Tiles;

namespace StardewValley.Objects
{
  public class CrabPot : StardewValley.Object
  {
    public const int lidFlapTimerInterval = 60;
    private float yBob;
    [XmlElement("directionOffset")]
    public readonly NetVector2 directionOffset = new NetVector2();
    [XmlElement("bait")]
    public readonly NetRef<StardewValley.Object> bait = new NetRef<StardewValley.Object>();
    public int tileIndexToShow;
    private bool lidFlapping;
    private bool lidClosing;
    private float lidFlapTimer;
    private float shakeTimer;
    private Vector2 shake;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.directionOffset, (INetSerializable) this.bait);
    }

    public CrabPot()
    {
    }

    public List<Vector2> getOverlayTiles(GameLocation location)
    {
      List<Vector2> tiles = new List<Vector2>();
      if ((double) this.directionOffset.Y < 0.0)
        this.addOverlayTilesIfNecessary(location, (int) this.TileLocation.X, (int) this.tileLocation.Y, tiles);
      this.addOverlayTilesIfNecessary(location, (int) this.TileLocation.X, (int) this.tileLocation.Y + 1, tiles);
      if ((double) this.directionOffset.X < 0.0)
        this.addOverlayTilesIfNecessary(location, (int) this.TileLocation.X - 1, (int) this.tileLocation.Y + 1, tiles);
      if ((double) this.directionOffset.X > 0.0)
        this.addOverlayTilesIfNecessary(location, (int) this.TileLocation.X + 1, (int) this.tileLocation.Y + 1, tiles);
      return tiles;
    }

    protected void addOverlayTilesIfNecessary(
      GameLocation location,
      int tile_x,
      int tile_y,
      List<Vector2> tiles)
    {
      if (location != Game1.currentLocation || location.getTileIndexAt(tile_x, tile_y, "Buildings") < 0 || location.doesTileHaveProperty(tile_x, tile_y + 1, "Back", "Water") != null)
        return;
      tiles.Add(new Vector2((float) tile_x, (float) tile_y));
    }

    public void addOverlayTiles(GameLocation location)
    {
      if (location != Game1.currentLocation)
        return;
      foreach (Vector2 overlayTile in this.getOverlayTiles(location))
      {
        if (!Game1.crabPotOverlayTiles.ContainsKey(overlayTile))
          Game1.crabPotOverlayTiles[overlayTile] = 0;
        Game1.crabPotOverlayTiles[overlayTile]++;
      }
    }

    public void removeOverlayTiles(GameLocation location)
    {
      if (location != Game1.currentLocation)
        return;
      foreach (Vector2 overlayTile in this.getOverlayTiles(location))
      {
        if (Game1.crabPotOverlayTiles.ContainsKey(overlayTile))
        {
          Game1.crabPotOverlayTiles[overlayTile]--;
          if (Game1.crabPotOverlayTiles[overlayTile] <= 0)
            Game1.crabPotOverlayTiles.Remove(overlayTile);
        }
      }
    }

    public CrabPot(Vector2 tileLocation, int stack = 1)
      : base(tileLocation, 710, "Crab Pot", true, false, false, false)
    {
      this.type.Value = "interactive";
      this.tileIndexToShow = (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex;
    }

    public static bool IsValidCrabPotLocationTile(GameLocation location, int x, int y)
    {
      if (location is Caldera)
        return false;
      Vector2 key = new Vector2((float) x, (float) y);
      bool flag = location.doesTileHaveProperty(x + 1, y, "Water", "Back") != null && location.doesTileHaveProperty(x - 1, y, "Water", "Back") != null || location.doesTileHaveProperty(x, y + 1, "Water", "Back") != null && location.doesTileHaveProperty(x, y - 1, "Water", "Back") != null;
      return !location.objects.ContainsKey(key) && flag && location.doesTileHaveProperty((int) key.X, (int) key.Y, "Water", "Back") != null && location.doesTileHaveProperty((int) key.X, (int) key.Y, "Passable", "Buildings") == null;
    }

    public override void actionOnPlayerEntry()
    {
      this.updateOffset(Game1.currentLocation);
      this.addOverlayTiles(Game1.currentLocation);
      base.actionOnPlayerEntry();
    }

    public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
    {
      Vector2 vector2 = new Vector2((float) (x / 64), (float) (y / 64));
      if (who != null)
        this.owner.Value = who.UniqueMultiplayerID;
      if (!CrabPot.IsValidCrabPotLocationTile(location, (int) vector2.X, (int) vector2.Y))
        return false;
      this.tileLocation.Value = new Vector2((float) (x / 64), (float) (y / 64));
      location.objects.Add((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, (StardewValley.Object) this);
      location.playSound("waterSlosh");
      DelayedAction.playSoundAfterDelay("slosh", 150);
      this.updateOffset(location);
      this.addOverlayTiles(location);
      return true;
    }

    public void updateOffset(GameLocation location)
    {
      Vector2 zero = Vector2.Zero;
      if (this.checkLocation(location, this.tileLocation.X - 1f, this.tileLocation.Y))
        zero += new Vector2(32f, 0.0f);
      if (this.checkLocation(location, this.tileLocation.X + 1f, this.tileLocation.Y))
        zero += new Vector2(-32f, 0.0f);
      if ((double) zero.X != 0.0 && this.checkLocation(location, this.tileLocation.X + (float) Math.Sign(zero.X), this.tileLocation.Y + 1f))
        zero += new Vector2(0.0f, -42f);
      if (this.checkLocation(location, this.tileLocation.X, this.tileLocation.Y - 1f))
        zero += new Vector2(0.0f, 32f);
      if (this.checkLocation(location, this.tileLocation.X, this.tileLocation.Y + 1f))
        zero += new Vector2(0.0f, -42f);
      this.directionOffset.Value = zero;
    }

    protected bool checkLocation(GameLocation location, float tile_x, float tile_y) => location.doesTileHaveProperty((int) tile_x, (int) tile_y, "Water", "Back") == null || location.doesTileHaveProperty((int) tile_x, (int) tile_y, "Passable", "Buildings") != null;

    public override bool canBePlacedInWater() => true;

    public override Item getOne()
    {
      StardewValley.Object one = new StardewValley.Object((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 1);
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
    {
      if (!(dropInItem is StardewValley.Object @object))
        return false;
      Farmer farmer = Game1.getFarmer((long) this.owner);
      if (@object.Category != -21 || this.bait.Value != null || farmer != null && farmer.professions.Contains(11))
        return false;
      if (!probe)
      {
        if (who != null)
          this.owner.Value = who.UniqueMultiplayerID;
        this.bait.Value = @object.getOne() as StardewValley.Object;
        who.currentLocation.playSound("Ship");
        this.lidFlapping = true;
        this.lidFlapTimer = 60f;
      }
      return true;
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (this.tileIndexToShow == 714)
      {
        if (justCheckingForActivity)
          return true;
        StardewValley.Object @object = this.heldObject.Value;
        this.heldObject.Value = (StardewValley.Object) null;
        if (who.IsLocalPlayer && !who.addItemToInventoryBool((Item) @object))
        {
          this.heldObject.Value = @object;
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
          return false;
        }
        Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
        if (dictionary.ContainsKey((int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex))
        {
          string[] strArray = dictionary[(int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex].Split('/');
          int minValue = strArray.Length > 5 ? Convert.ToInt32(strArray[5]) : 1;
          int num = strArray.Length > 5 ? Convert.ToInt32(strArray[6]) : 10;
          who.caughtFish((int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, Game1.random.Next(minValue, num + 1));
        }
        this.readyForHarvest.Value = false;
        this.tileIndexToShow = 710;
        this.lidFlapping = true;
        this.lidFlapTimer = 60f;
        this.bait.Value = (StardewValley.Object) null;
        who.animateOnce(279 + who.FacingDirection);
        who.currentLocation.playSound("fishingRodBend");
        DelayedAction.playSoundAfterDelay("coin", 500);
        who.gainExperience(1, 5);
        this.shake = Vector2.Zero;
        this.shakeTimer = 0.0f;
        return true;
      }
      if (this.bait.Value == null)
      {
        if (justCheckingForActivity)
          return true;
        if (Game1.didPlayerJustClickAtAll(true))
        {
          if (Game1.player.addItemToInventoryBool(this.getOne()))
          {
            if (who.isMoving())
              Game1.haltAfterCheck = false;
            Game1.playSound("coin");
            Game1.currentLocation.objects.Remove((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
            return true;
          }
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
        }
      }
      return false;
    }

    public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
    {
      this.removeOverlayTiles(environment);
      base.performRemoveAction(tileLocation, environment);
    }

    public override void DayUpdate(GameLocation location)
    {
      bool flag1 = Game1.getFarmer((long) this.owner) != null && Game1.getFarmer((long) this.owner).professions.Contains(11);
      bool flag2 = Game1.getFarmer((long) this.owner) != null && Game1.getFarmer((long) this.owner).professions.Contains(10);
      if ((long) this.owner == 0L && Game1.player.professions.Contains(11))
        flag2 = true;
      if (!(this.bait.Value != null | flag1) || this.heldObject.Value != null)
        return;
      this.tileIndexToShow = 714;
      this.readyForHarvest.Value = true;
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 + (int) this.tileLocation.X * 1000 + (int) this.tileLocation.Y);
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
      List<int> intList = new List<int>();
      double num1 = flag2 ? 0.0 : 0.2;
      if (!flag2)
        num1 += (double) location.getExtraTrashChanceForCrabPot((int) this.tileLocation.X, (int) this.tileLocation.Y);
      if (random.NextDouble() > num1)
      {
        foreach (KeyValuePair<int, string> keyValuePair in dictionary)
        {
          if (keyValuePair.Value.Contains("trap"))
          {
            bool flag3 = location is Beach || location.catchOceanCrabPotFishFromThisSpot((int) this.tileLocation.X, (int) this.tileLocation.Y);
            string[] strArray = keyValuePair.Value.Split('/');
            if ((!strArray[4].Equals("ocean") || flag3) && !(strArray[4].Equals("freshwater") & flag3))
            {
              if (flag2)
              {
                intList.Add(keyValuePair.Key);
              }
              else
              {
                double num2 = Convert.ToDouble(strArray[2]);
                if (random.NextDouble() < num2)
                {
                  this.heldObject.Value = new StardewValley.Object(keyValuePair.Key, 1);
                  break;
                }
              }
            }
          }
        }
      }
      if (this.heldObject.Value != null)
        return;
      if (flag2 && intList.Count > 0)
        this.heldObject.Value = new StardewValley.Object(intList[random.Next(intList.Count)], 1);
      else
        this.heldObject.Value = new StardewValley.Object(random.Next(168, 173), 1);
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      if (this.lidFlapping)
      {
        this.lidFlapTimer -= (float) time.ElapsedGameTime.Milliseconds;
        if ((double) this.lidFlapTimer <= 0.0)
        {
          this.tileIndexToShow += this.lidClosing ? -1 : 1;
          if (this.tileIndexToShow >= 713 && !this.lidClosing)
          {
            this.lidClosing = true;
            --this.tileIndexToShow;
          }
          else if (this.tileIndexToShow <= 709 && this.lidClosing)
          {
            this.lidClosing = false;
            ++this.tileIndexToShow;
            this.lidFlapping = false;
            if (this.bait.Value != null)
              this.tileIndexToShow = 713;
          }
          this.lidFlapTimer = 60f;
        }
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.readyForHarvest && this.heldObject.Value != null)
      {
        this.shakeTimer -= (float) time.ElapsedGameTime.Milliseconds;
        if ((double) this.shakeTimer < 0.0)
          this.shakeTimer = (float) Game1.random.Next(2800, 3200);
      }
      if ((double) this.shakeTimer > 2000.0)
        this.shake.X = (float) Game1.random.Next(-1, 2);
      else
        this.shake.X = 0.0f;
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      if (this.heldObject.Value != null)
        this.tileIndexToShow = 714;
      else if (this.tileIndexToShow == 0)
        this.tileIndexToShow = (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex;
      this.yBob = (float) (Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 500.0 + (double) (x * 64)) * 8.0 + 8.0);
      if ((double) this.yBob <= 1.0 / 1000.0)
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 0, 64, 64), 150f, 8, 0, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.directionOffset + new Vector2((float) (x * 64 + 4), (float) (y * 64 + 32)), false, Game1.random.NextDouble() < 0.5, 1f / 1000f, 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f));
      Tile tile = Game1.currentLocation.Map.GetLayer("Buildings").Tiles[x, y];
      spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.directionOffset + new Vector2((float) (x * 64), (float) (y * 64 + (int) this.yBob))) + this.shake, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.tileIndexToShow, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) (y * 64) + (double) this.directionOffset.Y + (double) (x % 4)) / 10000.0));
      if (Game1.currentLocation.waterTiles != null && x < Game1.currentLocation.waterTiles.waterTiles.GetLength(0) && y < Game1.currentLocation.waterTiles.waterTiles.GetLength(1) && Game1.currentLocation.waterTiles.waterTiles[x, y].isWater)
      {
        if (Game1.currentLocation.waterTiles.waterTiles[x, y].isVisible)
        {
          spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.directionOffset + new Vector2((float) (x * 64 + 4), (float) (y * 64 + 48))) + this.shake, new Rectangle?(new Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2112 + ((x + y) % 2 == 0 ? (Game1.currentLocation.waterTileFlip ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 0 : 128)), 56, 16 + (int) this.yBob)), (Color) (NetFieldBase<Color, NetColor>) Game1.currentLocation.waterColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) (((double) (y * 64) + (double) this.directionOffset.Y + (double) (x % 4)) / 9999.0));
        }
        else
        {
          Color color = Utility.MultiplyColor(new Color(135, 135, 135, 215), Game1.currentLocation.waterColor.Value);
          spriteBatch.Draw(Game1.staminaRect, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.directionOffset + new Vector2((float) (x * 64 + 4), (float) (y * 64 + 48))) + this.shake, new Rectangle?(), color, 0.0f, Vector2.Zero, new Vector2(56f, (float) (16 + (int) this.yBob)), SpriteEffects.None, (float) (((double) (y * 64) + (double) this.directionOffset.Y + (double) (x % 4)) / 9999.0));
        }
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.readyForHarvest || this.heldObject.Value == null)
        return;
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.directionOffset + new Vector2((float) (x * 64 - 8), (float) (y * 64 - 96 - 16) + num)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) ((y + 1) * 64) / 10000.0 + 9.99999997475243E-07 + (double) this.tileLocation.X / 10000.0));
      spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.directionOffset + new Vector2((float) (x * 64 + 32), (float) (y * 64 - 64 - 8) + num)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.heldObject.Value.parentSheetIndex, 16, 16)), Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float) ((double) ((y + 1) * 64) / 10000.0 + 9.99999974737875E-06 + (double) this.tileLocation.X / 10000.0));
    }
  }
}
