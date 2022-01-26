// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.BedFurniture
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Network;
using System;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class BedFurniture : Furniture
  {
    public static int DEFAULT_BED_INDEX = 2048;
    public static int DOUBLE_BED_INDEX = 2052;
    public static int CHILD_BED_INDEX = 2076;
    [XmlIgnore]
    public int bedTileOffset;
    [XmlIgnore]
    protected bool _alreadyAttempingRemoval;
    [XmlIgnore]
    public static bool ignoreContextualBedSpotOffset = false;
    [XmlIgnore]
    protected NetEnum<BedFurniture.BedType> _bedType = new NetEnum<BedFurniture.BedType>(BedFurniture.BedType.Any);
    [XmlIgnore]
    public NetMutex mutex = new NetMutex();

    [XmlElement("bedType")]
    public BedFurniture.BedType bedType
    {
      get
      {
        if (this._bedType.Value == BedFurniture.BedType.Any)
        {
          BedFurniture.BedType bedType = BedFurniture.BedType.Single;
          string[] data = this.getData();
          if (data.Length > 1)
          {
            string[] strArray = data[1].Split(' ');
            if (strArray.Length > 1)
            {
              if (strArray[1] == "double")
                bedType = BedFurniture.BedType.Double;
              else if (strArray[1] == "child")
                bedType = BedFurniture.BedType.Child;
            }
          }
          this._bedType.Value = bedType;
        }
        return this._bedType.Value;
      }
      set => this._bedType.Value = value;
    }

    public BedFurniture()
    {
    }

    public BedFurniture(int which, Vector2 tile, int initialRotations)
      : base(which, tile, initialRotations)
    {
    }

    public BedFurniture(int which, Vector2 tile)
      : base(which, tile)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this._bedType, (INetSerializable) this.mutex.NetFields);
    }

    public virtual bool IsBeingSleptIn(GameLocation location)
    {
      if (this.mutex.IsLocked())
        return true;
      foreach (Character farmer in location.farmers)
      {
        if (farmer.GetBoundingBox().Intersects(this.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation)))
          return true;
      }
      return false;
    }

    public override void DayUpdate(GameLocation location)
    {
      base.DayUpdate(location);
      this.mutex.ReleaseLock();
    }

    public virtual void ReserveForNPC() => this.mutex.RequestLock();

    public override void AttemptRemoval(Action<Furniture> removal_action)
    {
      if (this._alreadyAttempingRemoval)
      {
        this._alreadyAttempingRemoval = false;
      }
      else
      {
        this._alreadyAttempingRemoval = true;
        this.mutex.RequestLock((Action) (() =>
        {
          this._alreadyAttempingRemoval = false;
          if (removal_action == null)
            return;
          removal_action((Furniture) this);
          this.mutex.ReleaseLock();
        }), (Action) (() => this._alreadyAttempingRemoval = false));
      }
    }

    public static BedFurniture GetBedAtTile(GameLocation location, int x, int y)
    {
      if (location == null)
        return (BedFurniture) null;
      foreach (Furniture bedAtTile in location.furniture)
      {
        if (Utility.doesRectangleIntersectTile(bedAtTile.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) bedAtTile.tileLocation), x, y) && bedAtTile is BedFurniture)
          return bedAtTile as BedFurniture;
      }
      return (BedFurniture) null;
    }

    public static void ApplyWakeUpPosition(Farmer who)
    {
      if (who.lastSleepLocation.Value != null && Game1.isLocationAccessible((string) (NetFieldBase<string, NetString>) who.lastSleepLocation) && Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) who.lastSleepLocation) != null && (BedFurniture.IsBedHere(Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) who.lastSleepLocation), who.lastSleepPoint.Value.X, who.lastSleepPoint.Value.Y) || who.sleptInTemporaryBed.Value || Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) who.lastSleepLocation) is IslandFarmHouse))
      {
        GameLocation locationFromName = Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) who.lastSleepLocation);
        who.Position = Utility.PointToVector2(who.lastSleepPoint.Value) * 64f;
        who.currentLocation = locationFromName;
        BedFurniture.ShiftPositionForBed(who);
      }
      else
      {
        GameLocation locationFromName = Game1.getLocationFromName(who.homeLocation.Value);
        who.currentLocation = locationFromName;
        who.Position = Utility.PointToVector2((locationFromName as FarmHouse).GetPlayerBedSpot()) * 64f;
        BedFurniture.ShiftPositionForBed(who);
      }
      if (who != Game1.player)
        return;
      Game1.currentLocation = who.currentLocation;
    }

    public static void ShiftPositionForBed(Farmer who)
    {
      BedFurniture bedAtTile = BedFurniture.GetBedAtTile(who.currentLocation, (int) ((double) who.position.X / 64.0), (int) ((double) who.position.Y / 64.0));
      if (bedAtTile != null)
      {
        who.Position = Utility.PointToVector2(bedAtTile.GetBedSpot()) * 64f;
        if (bedAtTile.bedType != BedFurniture.BedType.Double)
        {
          if (who.currentLocation.map == null)
            who.currentLocation.reloadMap();
          if (!who.currentLocation.isTileLocationTotallyClearAndPlaceable(new Vector2(bedAtTile.TileLocation.X - 1f, bedAtTile.TileLocation.Y + 1f)))
          {
            who.faceDirection(3);
          }
          else
          {
            who.position.X -= 64f;
            who.faceDirection(1);
          }
        }
        else
        {
          bool flag = false;
          if (who.currentLocation is FarmHouse)
          {
            FarmHouse currentLocation = who.currentLocation as FarmHouse;
            if (currentLocation.owner != null)
            {
              long? spouse = currentLocation.owner.team.GetSpouse(currentLocation.owner.UniqueMultiplayerID);
              long uniqueMultiplayerId = who.UniqueMultiplayerID;
              if (spouse.GetValueOrDefault() == uniqueMultiplayerId & spouse.HasValue)
                flag = true;
              else if (currentLocation.owner != who && !currentLocation.owner.isMarried())
                flag = true;
            }
          }
          if (flag)
          {
            who.position.X += 64f;
            who.faceDirection(3);
          }
          else
          {
            who.position.X -= 64f;
            who.faceDirection(1);
          }
        }
      }
      who.position.Y += 32f;
      if (who.NetFields.Root == null)
        return;
      (who.NetFields.Root as NetRoot<Farmer>).CancelInterpolation();
    }

    public virtual bool CanModifyBed(GameLocation location, Farmer who)
    {
      if (location is FarmHouse)
      {
        FarmHouse farmHouse = location as FarmHouse;
        if (farmHouse.owner != who)
        {
          long? spouse = farmHouse.owner.team.GetSpouse(farmHouse.owner.UniqueMultiplayerID);
          long uniqueMultiplayerId = who.UniqueMultiplayerID;
          if (!(spouse.GetValueOrDefault() == uniqueMultiplayerId & spouse.HasValue))
            return false;
        }
      }
      return true;
    }

    public override int GetAdditionalFurniturePlacementStatus(
      GameLocation location,
      int x,
      int y,
      Farmer who = null)
    {
      if (this.bedType == BedFurniture.BedType.Double)
      {
        if (!location.isTileLocationTotallyClearAndPlaceable(new Vector2((float) (x / 64 - 1), (float) (y / 64 + 1))))
          return -1;
      }
      else if (!location.isTileLocationTotallyClearAndPlaceable(new Vector2((float) (x / 64 - 1), (float) (y / 64 + 1))) && !location.isTileLocationTotallyClearAndPlaceable(new Vector2((float) (x / 64 + this.getTilesWide()), (float) (y / 64 + 1))))
        return -1;
      return base.GetAdditionalFurniturePlacementStatus(location, x, y, who);
    }

    public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
    {
      this._alreadyAttempingRemoval = false;
      if (!this.CanModifyBed(location, who))
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Bed_CantMoveOthersBeds"));
        return false;
      }
      if (!(location is FarmHouse) || (this.bedType != BedFurniture.BedType.Child || (location as FarmHouse).upgradeLevel >= 2) && (this.bedType != BedFurniture.BedType.Double || (location as FarmHouse).upgradeLevel >= 1))
        return base.placementAction(location, x, y, who);
      Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Bed_NeedsUpgrade"));
      return false;
    }

    public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
    {
      this._alreadyAttempingRemoval = false;
      base.performRemoveAction(tileLocation, environment);
    }

    public override void hoverAction()
    {
      if (Game1.player.GetBoundingBox().Intersects(this.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation)))
        return;
      base.hoverAction();
    }

    public override bool canBeRemoved(Farmer who)
    {
      if (!this.CanModifyBed(who.currentLocation, who))
      {
        if (!Game1.player.GetBoundingBox().Intersects(this.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation)))
          Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Bed_CantMoveOthersBeds"));
        return false;
      }
      if (!this.IsBeingSleptIn(who.currentLocation))
        return true;
      if (!Game1.player.GetBoundingBox().Intersects(this.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation)))
        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Bed_InUse"));
      return false;
    }

    public override Item getOne()
    {
      BedFurniture one = new BedFurniture((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      one.drawPosition.Value = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawPosition;
      one.defaultBoundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.defaultBoundingBox;
      one.boundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.boundingBox;
      one.currentRotation.Value = (int) (NetFieldBase<int, NetInt>) this.currentRotation - 1;
      one.isOn.Value = false;
      one.rotations.Value = (int) (NetFieldBase<int, NetInt>) this.rotations;
      one.bedType = this.bedType;
      one.rotate();
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    public virtual Point GetBedSpot() => new Point((int) this.tileLocation.X + 1, (int) this.tileLocation.Y + 1);

    public override void resetOnPlayerEntry(GameLocation environment, bool dropDown = false) => this.UpdateBedTile(false);

    public virtual void UpdateBedTile(bool check_bounds)
    {
      Rectangle boundingBox = this.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      if (this.bedType == BedFurniture.BedType.Double)
      {
        this.bedTileOffset = 1;
      }
      else
      {
        if (check_bounds && boundingBox.Intersects(Game1.player.GetBoundingBox()))
          return;
        if ((double) Game1.player.Position.X > (double) boundingBox.Center.X)
          this.bedTileOffset = 0;
        else
          this.bedTileOffset = 1;
      }
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      this.mutex.Update(Game1.getOnlineFarmers());
      this.UpdateBedTile(true);
      base.updateWhenCurrentLocation(time, environment);
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      if (this.isTemporarilyInvisible)
        return;
      if (Furniture.isDrawingLocationFurniture)
      {
        Rectangle rectangle = this.sourceRect.Value;
        spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawPosition + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero)), new Rectangle?(rectangle), Color.White * alpha, 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (this.boundingBox.Value.Top + 1) / 10000f);
        rectangle.X += rectangle.Width;
        spriteBatch.Draw(Furniture.furnitureTexture, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawPosition + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero)), new Rectangle?(rectangle), Color.White * alpha, 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (this.boundingBox.Value.Bottom - 1) / 10000f);
      }
      else
        base.draw(spriteBatch, x, y, alpha);
    }

    public override bool AllowPlacementOnThisTile(int x, int y) => this.bedType == BedFurniture.BedType.Child && (double) y == (double) this.TileLocation.Y + 1.0 || base.AllowPlacementOnThisTile(x, y);

    public override bool IntersectsForCollision(Rectangle rect)
    {
      Rectangle boundingBox = this.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      if ((boundingBox with { Height = 64 }).Intersects(rect))
        return true;
      Rectangle rectangle = boundingBox;
      rectangle.Y += 128;
      rectangle.Height -= 128;
      return rectangle.Intersects(rect);
    }

    public override int GetAdditionalTilePropertyRadius() => 1;

    public static bool IsBedHere(GameLocation location, int x, int y)
    {
      if (location == null)
        return false;
      BedFurniture.ignoreContextualBedSpotOffset = true;
      if (location.doesTileHaveProperty(x, y, "Bed", "Back") != null)
      {
        BedFurniture.ignoreContextualBedSpotOffset = false;
        return true;
      }
      BedFurniture.ignoreContextualBedSpotOffset = false;
      return false;
    }

    public override bool DoesTileHaveProperty(
      int tile_x,
      int tile_y,
      string property_name,
      string layer_name,
      ref string property_value)
    {
      if (this.bedType == BedFurniture.BedType.Double && (double) tile_x == (double) this.tileLocation.X - 1.0 && (double) tile_y == (double) this.tileLocation.Y + 1.0 && layer_name == "Back" && property_name == "NoFurniture")
      {
        property_value = "t";
        return true;
      }
      if ((double) tile_x >= (double) this.tileLocation.X && (double) tile_x < (double) this.tileLocation.X + (double) this.getTilesWide() && (double) tile_y == (double) this.tileLocation.Y + 1.0 && layer_name == "Back")
      {
        if (property_name == "Bed")
        {
          property_value = "t";
          return true;
        }
        if (this.bedType != BedFurniture.BedType.Child)
        {
          int num = (int) this.tileLocation.X + this.bedTileOffset;
          if (BedFurniture.ignoreContextualBedSpotOffset)
            num = (int) this.tileLocation.X + 1;
          if (tile_x == num && property_name == "TouchAction")
          {
            property_value = "Sleep";
            return true;
          }
        }
      }
      return false;
    }

    public enum BedType
    {
      Any = -1, // 0xFFFFFFFF
      Single = 0,
      Double = 1,
      Child = 2,
    }
  }
}
