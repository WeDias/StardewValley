// Decompiled with JetBrains decompiler
// Type: StardewValley.MapSeat
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley
{
  public class MapSeat : INetObject<NetFields>, ISittable
  {
    [XmlIgnore]
    public static Texture2D mapChairTexture;
    [XmlIgnore]
    public NetLongDictionary<int, NetInt> sittingFarmers = new NetLongDictionary<int, NetInt>();
    [XmlIgnore]
    public NetVector2 tilePosition = new NetVector2();
    [XmlIgnore]
    public NetVector2 size = new NetVector2();
    [XmlIgnore]
    public NetInt direction = new NetInt();
    [XmlIgnore]
    public NetVector2 drawTilePosition = new NetVector2(new Vector2(-1f, -1f));
    [XmlIgnore]
    public NetBool seasonal = new NetBool();
    [XmlIgnore]
    public NetString seatType = new NetString();
    [XmlIgnore]
    public NetString textureFile = new NetString((string) null);
    [XmlIgnore]
    public string _loadedTextureFile;
    [XmlIgnore]
    public Texture2D overlayTexture;
    [XmlIgnore]
    public int localSittingDirection = 2;
    [XmlIgnore]
    public Vector3? customDrawValues;

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public MapSeat() => this.NetFields.AddFields((INetSerializable) this.sittingFarmers, (INetSerializable) this.tilePosition, (INetSerializable) this.size, (INetSerializable) this.direction, (INetSerializable) this.drawTilePosition, (INetSerializable) this.seasonal, (INetSerializable) this.seatType, (INetSerializable) this.textureFile);

    public static MapSeat FromData(string data, int x, int y)
    {
      MapSeat mapSeat = new MapSeat();
      try
      {
        string[] strArray = data.Split('/');
        mapSeat.tilePosition.Set(new Vector2((float) x, (float) y));
        mapSeat.size.Set(new Vector2((float) int.Parse(strArray[0]), (float) int.Parse(strArray[1])));
        mapSeat.seatType.Value = strArray[3];
        if (strArray[2] == "right")
          mapSeat.direction.Value = 1;
        else if (strArray[2] == "left")
          mapSeat.direction.Value = 3;
        else if (strArray[2] == "down")
          mapSeat.direction.Value = 2;
        else if (strArray[2] == "up")
          mapSeat.direction.Value = 0;
        else if (strArray[2] == "opposite")
          mapSeat.direction.Value = -2;
        mapSeat.drawTilePosition.Set(new Vector2((float) int.Parse(strArray[4]), (float) int.Parse(strArray[5])));
        mapSeat.seasonal.Value = strArray[6] == "true";
        if (strArray.Length > 7)
          mapSeat.textureFile.Value = strArray[7];
        else
          mapSeat.textureFile.Value = (string) null;
      }
      catch (Exception ex)
      {
      }
      return mapSeat;
    }

    public bool IsBlocked(GameLocation location)
    {
      Rectangle seatBounds = this.GetSeatBounds();
      seatBounds.X *= 64;
      seatBounds.Y *= 64;
      seatBounds.Width *= 64;
      seatBounds.Height *= 64;
      Rectangle rectangle = seatBounds;
      if ((int) (NetFieldBase<int, NetInt>) this.direction == 0)
      {
        rectangle.Y -= 32;
        rectangle.Height += 32;
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.direction == 2)
        rectangle.Height += 32;
      if ((int) (NetFieldBase<int, NetInt>) this.direction == 3)
      {
        rectangle.X -= 32;
        rectangle.Width += 32;
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.direction == 1)
        rectangle.Width += 32;
      foreach (NPC character in location.characters)
      {
        Rectangle boundingBox = character.GetBoundingBox();
        if (boundingBox.Intersects(seatBounds) || !character.isMovingOnPathFindPath.Value && boundingBox.Intersects(rectangle))
          return true;
      }
      return false;
    }

    public bool IsSittingHere(Farmer who) => this.sittingFarmers.ContainsKey(who.UniqueMultiplayerID);

    public bool HasSittingFarmers() => this.sittingFarmers.Count() > 0;

    public List<Vector2> GetSeatPositions(bool ignore_offsets = false)
    {
      this.customDrawValues = new Vector3?();
      List<Vector2> seatPositions = new List<Vector2>();
      if (this.seatType.Value.StartsWith("custom "))
      {
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;
        string[] strArray = this.seatType.Value.Split(' ');
        try
        {
          if (strArray.Length > 1)
            x = float.Parse(strArray[1]);
          if (strArray.Length > 2)
            y = float.Parse(strArray[2]);
          if (strArray.Length > 3)
            z = float.Parse(strArray[3]);
        }
        catch (Exception ex)
        {
        }
        this.customDrawValues = new Vector3?(new Vector3(x, y, z));
        Vector2 vector2 = new Vector2(this.tilePosition.X + this.customDrawValues.Value.X, this.tilePosition.Y);
        if (!ignore_offsets)
          vector2.Y += this.customDrawValues.Value.Y;
        seatPositions.Add(vector2);
      }
      else if (this.seatType.Value == "playground")
      {
        Vector2 vector2 = new Vector2(this.tilePosition.X + 0.75f, this.tilePosition.Y);
        if (!ignore_offsets)
          vector2.Y -= 0.1f;
        seatPositions.Add(vector2);
      }
      else if (this.seatType.Value == "ccdesk")
      {
        Vector2 vector2 = new Vector2(this.tilePosition.X + 0.5f, this.tilePosition.Y);
        if (!ignore_offsets)
          vector2.Y -= 0.4f;
        seatPositions.Add(vector2);
      }
      else
      {
        for (int index1 = 0; (double) index1 < (double) this.size.X; ++index1)
        {
          for (int index2 = 0; (double) index2 < (double) this.size.Y; ++index2)
          {
            Vector2 vector2 = new Vector2(0.0f, 0.0f);
            if (this.seatType.Value.StartsWith("bench"))
            {
              if (this.direction.Value == 2)
                vector2.Y += 0.25f;
              else if ((this.direction.Value == 3 || this.direction.Value == 1) && index2 == 0)
                vector2.Y += 0.5f;
            }
            if (this.seatType.Value.StartsWith("picnic"))
            {
              if (this.direction.Value == 2)
                vector2.Y -= 0.25f;
              else if (this.direction.Value == 0)
                vector2.Y += 0.25f;
            }
            if (this.seatType.Value.EndsWith("swings"))
              vector2.Y -= 0.5f;
            if (this.seatType.Value.EndsWith("summitbench"))
              vector2.Y -= 0.2f;
            if (this.seatType.Value.EndsWith("tall"))
              vector2.Y -= 0.3f;
            if (this.seatType.Value.EndsWith("short"))
              vector2.Y += 0.3f;
            if (ignore_offsets)
              vector2 = Vector2.Zero;
            seatPositions.Add(this.tilePosition.Value + new Vector2((float) index1 + vector2.X, (float) index2 + vector2.Y));
          }
        }
      }
      return seatPositions;
    }

    public virtual void Draw(SpriteBatch b)
    {
      if (this._loadedTextureFile != this.textureFile.Value)
      {
        this._loadedTextureFile = this.textureFile.Value;
        try
        {
          this.overlayTexture = Game1.content.Load<Texture2D>(this._loadedTextureFile);
        }
        catch (Exception ex)
        {
          this.overlayTexture = (Texture2D) null;
        }
      }
      if (this.overlayTexture == null)
        this.overlayTexture = MapSeat.mapChairTexture;
      if ((double) this.drawTilePosition.Value.X < 0.0 || !this.HasSittingFarmers())
        return;
      float num = 0.0f;
      if (this.customDrawValues.HasValue)
        num = this.customDrawValues.Value.Z;
      else if (this.seatType.Value.StartsWith("highback_chair") || this.seatType.Value.StartsWith("ccdesk"))
        num = 1f;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(this.tilePosition.X * 64f, (float) (((double) this.tilePosition.Y - (double) num) * 64.0)));
      float layerDepth = (float) (((double) (int) this.tilePosition.Y + (double) this.size.Y + 0.1) * 64.0) / 10000f;
      Rectangle rectangle = new Rectangle((int) this.drawTilePosition.Value.X * 16, (int) ((double) this.drawTilePosition.Value.Y - (double) num) * 16, (int) this.size.Value.X * 16, (int) ((double) this.size.Value.Y + (double) num) * 16);
      if (this.seasonal.Value)
      {
        if (Game1.currentLocation.GetSeasonForLocation() == "summer")
          rectangle.X += rectangle.Width;
        else if (Game1.currentLocation.GetSeasonForLocation() == "fall")
          rectangle.X += rectangle.Width * 2;
        else if (Game1.currentLocation.GetSeasonForLocation() == "winter")
          rectangle.X += rectangle.Width * 3;
      }
      b.Draw(this.overlayTexture, local, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
    }

    public bool OccupiesTile(int x, int y) => this.GetSeatBounds().Contains(x, y);

    public virtual Vector2? AddSittingFarmer(Farmer who)
    {
      if (who == Game1.player)
      {
        this.localSittingDirection = this.direction.Value;
        if (this.seatType.Value.StartsWith("stool"))
          this.localSittingDirection = Game1.player.FacingDirection;
        if (this.direction.Value == -2)
          this.localSittingDirection = Utility.GetOppositeFacingDirection(Game1.player.FacingDirection);
        if (this.seatType.Value.StartsWith("bathchair") && this.localSittingDirection == 0)
          this.localSittingDirection = 2;
      }
      List<Vector2> seatPositions = this.GetSeatPositions(false);
      int num1 = -1;
      Vector2? nullable = new Vector2?();
      float num2 = 96f;
      for (int index = 0; index < seatPositions.Count; ++index)
      {
        if (!this.sittingFarmers.Values.Contains<int>(index))
        {
          float num3 = ((seatPositions[index] + new Vector2(0.5f, 0.5f)) * 64f - who.getStandingPosition()).Length();
          if ((double) num3 < (double) num2)
          {
            num2 = num3;
            nullable = new Vector2?(seatPositions[index]);
            num1 = index;
          }
        }
      }
      if (nullable.HasValue)
        this.sittingFarmers[who.UniqueMultiplayerID] = num1;
      return nullable;
    }

    public bool IsSeatHere(GameLocation location) => location.mapSeats.Contains(this);

    public int GetSittingDirection() => this.localSittingDirection;

    public Vector2? GetSittingPosition(Farmer who, bool ignore_offsets = false) => this.sittingFarmers.ContainsKey(who.UniqueMultiplayerID) ? new Vector2?(this.GetSeatPositions(ignore_offsets)[this.sittingFarmers[who.UniqueMultiplayerID]]) : new Vector2?();

    public virtual Rectangle GetSeatBounds()
    {
      if (this.seatType.Value == "chair" && (int) (NetFieldBase<int, NetInt>) this.direction == 0)
      {
        Rectangle rectangle = new Rectangle((int) this.tilePosition.X, (int) this.tilePosition.Y + 1, (int) this.size.X, (int) this.size.Y - 1);
      }
      return new Rectangle((int) this.tilePosition.X, (int) this.tilePosition.Y, (int) this.size.X, (int) this.size.Y);
    }

    public virtual void RemoveSittingFarmer(Farmer farmer) => this.sittingFarmers.Remove(farmer.UniqueMultiplayerID);

    public virtual int GetSittingFarmerCount() => this.sittingFarmers.Count();
  }
}
