// Decompiled with JetBrains decompiler
// Type: StardewValley.Multiplayer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace StardewValley
{
  public class Multiplayer
  {
    public static readonly long AllPlayers = 0;
    public const byte farmerDelta = 0;
    public const byte serverIntroduction = 1;
    public const byte playerIntroduction = 2;
    public const byte locationIntroduction = 3;
    public const byte forceEvent = 4;
    public const byte warpFarmer = 5;
    public const byte locationDelta = 6;
    public const byte locationSprites = 7;
    public const byte characterWarp = 8;
    public const byte availableFarmhands = 9;
    public const byte chatMessage = 10;
    public const byte connectionMessage = 11;
    public const byte worldDelta = 12;
    public const byte teamDelta = 13;
    public const byte newDaySync = 14;
    public const byte chatInfoMessage = 15;
    public const byte userNameUpdate = 16;
    public const byte farmerGainExperience = 17;
    public const byte serverToClientsMessage = 18;
    public const byte disconnecting = 19;
    public const byte sharedAchievement = 20;
    public const byte globalMessage = 21;
    public const byte partyWideMail = 22;
    public const byte forceKick = 23;
    public const byte removeLocationFromLookup = 24;
    public const byte farmerKilledMonster = 25;
    public const byte requestGrandpaReevaluation = 26;
    public const byte digBuriedNut = 27;
    public const byte requestPassout = 28;
    public const byte passout = 29;
    public int defaultInterpolationTicks = 15;
    public int farmerDeltaBroadcastPeriod = 3;
    public int locationDeltaBroadcastPeriod = 3;
    public int worldStateDeltaBroadcastPeriod = 3;
    public int playerLimit = 4;
    public static string kicked = "KICKED";
    public const string protocolVersion = "1.5.5";
    public readonly NetLogger logging = new NetLogger();
    protected List<long> disconnectingFarmers = new List<long>();
    public ulong latestID;
    public Dictionary<string, CachedMultiplayerMap> cachedMultiplayerMaps = new Dictionary<string, CachedMultiplayerMap>();
    public const string MSG_START_FESTIVAL_EVENT = "festivalEvent";
    public const string MSG_END_FESTIVAL = "endFest";
    public const string MSG_TRAIN_APPROACH = "trainApproach";
    public const string MSG_PLACEHOLDER = "[replace me]";

    public virtual long getNewID()
    {
      ulong num1 = (ulong) (((long) this.latestID & (long) byte.MaxValue) + 1L) & (ulong) byte.MaxValue;
      ulong uniqueMultiplayerId = (ulong) Game1.player.UniqueMultiplayerID;
      ulong num2 = uniqueMultiplayerId >> 32 ^ uniqueMultiplayerId & (ulong) uint.MaxValue;
      this.latestID = (ulong) ((long) ((ulong) DateTime.Now.Ticks / 10000UL) << 24 | (long) ((num2 >> 16 ^ num2 & (ulong) ushort.MaxValue) & (ulong) ushort.MaxValue) << 8) | num1;
      return (long) this.latestID;
    }

    public virtual int MaxPlayers => Game1.server == null ? 1 : this.playerLimit;

    public virtual bool isDisconnecting(Farmer farmer) => this.isDisconnecting(farmer.UniqueMultiplayerID);

    public virtual bool isDisconnecting(long uid) => this.disconnectingFarmers.Contains(uid);

    public virtual bool isClientBroadcastType(byte messageType)
    {
      switch (messageType)
      {
        case 0:
        case 2:
        case 4:
        case 6:
        case 7:
        case 12:
        case 13:
        case 14:
        case 15:
        case 19:
        case 20:
        case 21:
        case 22:
        case 24:
        case 26:
          return true;
        default:
          return false;
      }
    }

    public virtual bool allowSyncDelay() => Game1.newDaySync == null;

    public virtual int interpolationTicks()
    {
      if (!this.allowSyncDelay())
        return 0;
      return LocalMultiplayer.IsLocalMultiplayer(true) ? 4 : this.defaultInterpolationTicks;
    }

    public virtual IEnumerable<NetFarmerRoot> farmerRoots()
    {
      if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null)
        yield return Game1.serverHost;
      foreach (NetRoot<Farmer> netRoot in Game1.otherFarmers.Roots.Values)
      {
        if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null || (NetFieldBase<Farmer, NetRef<Farmer>>) netRoot != (NetRef<Farmer>) Game1.serverHost)
          yield return netRoot as NetFarmerRoot;
      }
    }

    public virtual NetFarmerRoot farmerRoot(long id)
    {
      if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null && id == Game1.serverHost.Value.UniqueMultiplayerID)
        return Game1.serverHost;
      return Game1.otherFarmers.ContainsKey(id) ? Game1.otherFarmers.Roots[id] as NetFarmerRoot : (NetFarmerRoot) null;
    }

    public virtual void broadcastFarmerDeltas()
    {
      foreach (NetFarmerRoot farmerRoot in this.farmerRoots())
      {
        if (farmerRoot.Dirty && Game1.player.UniqueMultiplayerID == farmerRoot.Value.UniqueMultiplayerID)
          this.broadcastFarmerDelta(farmerRoot.Value, this.writeObjectDeltaBytes<Farmer>((NetRoot<Farmer>) farmerRoot));
      }
      if (!Game1.player.teamRoot.Dirty)
        return;
      this.broadcastTeamDelta(this.writeObjectDeltaBytes<FarmerTeam>(Game1.player.teamRoot));
    }

    protected virtual void broadcastTeamDelta(byte[] delta)
    {
      if (Game1.IsServer)
      {
        foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
        {
          if (farmer != Game1.player)
            Game1.server.sendMessage(farmer.UniqueMultiplayerID, (byte) 13, Game1.player, (object) delta);
        }
      }
      else
      {
        if (!Game1.IsClient)
          return;
        Game1.client.sendMessage((byte) 13, (object) delta);
      }
    }

    protected virtual void broadcastFarmerDelta(Farmer farmer, byte[] delta)
    {
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      {
        if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
          otherFarmer.Value.queueMessage((byte) 0, farmer, (object) farmer.UniqueMultiplayerID, (object) delta);
      }
    }

    public void updateRoot<T>(T root) where T : INetRoot
    {
      foreach (long disconnectingFarmer in this.disconnectingFarmers)
        root.Disconnect(disconnectingFarmer);
      root.TickTree();
    }

    public virtual void updateRoots()
    {
      this.updateRoot<NetRoot<IWorldState>>(Game1.netWorldState);
      foreach (NetFarmerRoot farmerRoot in this.farmerRoots())
      {
        farmerRoot.Clock.InterpolationTicks = this.interpolationTicks();
        this.updateRoot<NetFarmerRoot>(farmerRoot);
      }
      Game1.player.teamRoot.Clock.InterpolationTicks = this.interpolationTicks();
      this.updateRoot<NetRoot<FarmerTeam>>(Game1.player.teamRoot);
      if (Game1.IsClient)
      {
        foreach (GameLocation activeLocation in this.activeLocations())
        {
          if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) activeLocation.Root != (NetRef<GameLocation>) null && activeLocation.Root.Value == activeLocation)
          {
            activeLocation.Root.Clock.InterpolationTicks = this.interpolationTicks();
            this.updateRoot<NetRoot<GameLocation>>(activeLocation.Root);
          }
        }
      }
      else
      {
        foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
        {
          if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) location.Root != (NetRef<GameLocation>) null)
          {
            location.Root.Clock.InterpolationTicks = this.interpolationTicks();
            this.updateRoot<NetRoot<GameLocation>>(location.Root);
          }
        }
        foreach (MineShaft activeMine in MineShaft.activeMines)
        {
          if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) activeMine.Root != (NetRef<GameLocation>) null)
          {
            activeMine.Root.Clock.InterpolationTicks = this.interpolationTicks();
            this.updateRoot<NetRoot<GameLocation>>(activeMine.Root);
          }
        }
        foreach (VolcanoDungeon activeLevel in VolcanoDungeon.activeLevels)
        {
          if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) activeLevel.Root != (NetRef<GameLocation>) null)
          {
            activeLevel.Root.Clock.InterpolationTicks = this.interpolationTicks();
            this.updateRoot<NetRoot<GameLocation>>(activeLevel.Root);
          }
        }
      }
    }

    public virtual void broadcastLocationDeltas()
    {
      if (Game1.IsClient)
      {
        foreach (GameLocation activeLocation in this.activeLocations())
        {
          if (!((NetFieldBase<GameLocation, NetRef<GameLocation>>) activeLocation.Root == (NetRef<GameLocation>) null) && activeLocation.Root.Dirty)
            this.broadcastLocationDelta(activeLocation);
        }
      }
      else
      {
        foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
        {
          if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) location.Root != (NetRef<GameLocation>) null && location.Root.Dirty)
            this.broadcastLocationDelta(location);
        }
        MineShaft.ForEach((Action<MineShaft>) (mine =>
        {
          if (!((NetFieldBase<GameLocation, NetRef<GameLocation>>) mine.Root != (NetRef<GameLocation>) null) || !mine.Root.Dirty)
            return;
          this.broadcastLocationDelta((GameLocation) mine);
        }));
        VolcanoDungeon.ForEach((Action<VolcanoDungeon>) (level =>
        {
          if (!((NetFieldBase<GameLocation, NetRef<GameLocation>>) level.Root != (NetRef<GameLocation>) null) || !level.Root.Dirty)
            return;
          this.broadcastLocationDelta((GameLocation) level);
        }));
      }
    }

    public virtual void broadcastLocationDelta(GameLocation loc)
    {
      if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) loc.Root == (NetRef<GameLocation>) null || !loc.Root.Dirty)
        return;
      byte[] bytes = this.writeObjectDeltaBytes<GameLocation>(loc.Root);
      this.broadcastLocationBytes(loc, (byte) 6, bytes);
    }

    protected virtual void broadcastLocationBytes(GameLocation loc, byte messageType, byte[] bytes)
    {
      OutgoingMessage message = new OutgoingMessage(messageType, Game1.player, new object[3]
      {
        (object) loc.isStructure.Value,
        (bool) (NetFieldBase<bool, NetBool>) loc.isStructure ? (object) loc.uniqueName.Value : (object) loc.name.Value,
        (object) bytes
      });
      this.broadcastLocationMessage(loc, message);
    }

    protected virtual void broadcastLocationMessage(GameLocation loc, OutgoingMessage message)
    {
      if (Game1.IsClient)
      {
        Game1.client.sendMessage(message);
      }
      else
      {
        Action<Farmer> action = (Action<Farmer>) (f =>
        {
          if (f == Game1.player)
            return;
          Game1.server.sendMessage(f.UniqueMultiplayerID, message);
        });
        if (this.isAlwaysActiveLocation(loc))
        {
          foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
            action(farmer);
        }
        else
        {
          foreach (Farmer farmer in loc.farmers)
            action(farmer);
          if (!(loc is BuildableGameLocation))
            return;
          foreach (Building building in (loc as BuildableGameLocation).buildings)
          {
            if (building.indoors.Value != null)
            {
              foreach (Farmer farmer in building.indoors.Value.farmers)
                action(farmer);
            }
          }
        }
      }
    }

    public virtual void broadcastSprites(
      GameLocation location,
      List<TemporaryAnimatedSprite> sprites)
    {
      this.broadcastSprites(location, sprites.ToArray());
    }

    public virtual void broadcastSprites(
      GameLocation location,
      params TemporaryAnimatedSprite[] sprites)
    {
      location.temporarySprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) sprites);
      if (sprites.Length == 0 || !Game1.IsMultiplayer)
        return;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = this.createWriter((Stream) memoryStream))
        {
          writer.Push("TemporaryAnimatedSprites");
          writer.Write(sprites.Length);
          foreach (TemporaryAnimatedSprite sprite in sprites)
            sprite.Write(writer, location);
          writer.Pop();
        }
        this.broadcastLocationBytes(location, (byte) 7, memoryStream.ToArray());
      }
    }

    public virtual void broadcastWorldStateDeltas()
    {
      if (!Game1.netWorldState.Dirty)
        return;
      byte[] numArray = this.writeObjectDeltaBytes<IWorldState>(Game1.netWorldState);
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      {
        if (otherFarmer.Value != Game1.player)
          otherFarmer.Value.queueMessage((byte) 12, Game1.player, (object) numArray);
      }
    }

    public virtual void receiveWorldState(BinaryReader msg)
    {
      Game1.netWorldState.Clock.InterpolationTicks = 0;
      this.readObjectDelta<IWorldState>(msg, Game1.netWorldState);
      Game1.netWorldState.TickTree();
      int timeOfDay = Game1.timeOfDay;
      Game1.netWorldState.Value.WriteToGame1();
      if (Game1.IsServer || timeOfDay == Game1.timeOfDay || Game1.currentLocation == null || Game1.newDaySync != null)
        return;
      Game1.performTenMinuteClockUpdate();
    }

    public virtual void requestCharacterWarp(
      NPC character,
      GameLocation targetLocation,
      Vector2 position)
    {
      if (!Game1.IsClient)
        return;
      GameLocation currentLocation = character.currentLocation;
      if (currentLocation == null)
        throw new ArgumentException("In warpCharacter, the character's currentLocation must not be null");
      Guid guid = currentLocation.characters.GuidOf(character);
      if (guid == Guid.Empty)
        throw new ArgumentException("In warpCharacter, the character must be in its currentLocation");
      OutgoingMessage message = new OutgoingMessage((byte) 8, Game1.player, new object[6]
      {
        (object) currentLocation.isStructure.Value,
        (bool) (NetFieldBase<bool, NetBool>) currentLocation.isStructure ? (object) currentLocation.uniqueName.Value : (object) currentLocation.name.Value,
        (object) guid,
        (object) targetLocation.isStructure.Value,
        (bool) (NetFieldBase<bool, NetBool>) targetLocation.isStructure ? (object) targetLocation.uniqueName.Value : (object) targetLocation.name.Value,
        (object) position
      });
      Game1.serverHost.Value.queueMessage(message);
    }

    public virtual NetRoot<GameLocation> locationRoot(GameLocation location)
    {
      if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) location.Root == (NetRef<GameLocation>) null && Game1.IsMasterGame)
      {
        new NetRoot<GameLocation>().Set(location);
        location.Root.Clock.InterpolationTicks = this.interpolationTicks();
        location.Root.MarkClean();
      }
      return location.Root;
    }

    public virtual void sendPassoutRequest()
    {
      object[] objArray = new object[1]
      {
        (object) Game1.player.UniqueMultiplayerID
      };
      if (Game1.IsMasterGame)
        this._receivePassoutRequest(Game1.player);
      else
        Game1.client.sendMessage((byte) 28, objArray);
    }

    public virtual void receivePassoutRequest(IncomingMessage msg)
    {
      if (!Game1.IsServer)
        return;
      Farmer farmer = Game1.getFarmer(msg.Reader.ReadInt64());
      if (farmer == null)
        return;
      this._receivePassoutRequest(farmer);
    }

    protected virtual void _receivePassoutRequest(Farmer farmer)
    {
      if (!Game1.IsMasterGame)
        return;
      if (farmer.lastSleepLocation.Value != null && Game1.isLocationAccessible((string) (NetFieldBase<string, NetString>) farmer.lastSleepLocation) && Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) farmer.lastSleepLocation) != null && Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) farmer.lastSleepLocation).GetLocationContext() == farmer.currentLocation.GetLocationContext() && BedFurniture.IsBedHere(Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) farmer.lastSleepLocation), farmer.lastSleepPoint.Value.X, farmer.lastSleepPoint.Value.Y))
      {
        if (Game1.IsServer && farmer != Game1.player)
        {
          object[] source = new object[4]
          {
            (object) farmer.lastSleepLocation.Value,
            (object) farmer.lastSleepPoint.X,
            (object) farmer.lastSleepPoint.Y,
            (object) true
          };
          Game1.server.sendMessage(farmer.UniqueMultiplayerID, (byte) 29, Game1.player, ((IEnumerable<object>) source).ToArray<object>());
        }
        else
          Farmer.performPassoutWarp(farmer, (string) (NetFieldBase<string, NetString>) farmer.lastSleepLocation, (Point) (NetFieldBase<Point, NetPoint>) farmer.lastSleepPoint, true);
      }
      else
      {
        string nameOrUniqueName = Utility.getHomeOfFarmer(farmer).NameOrUniqueName;
        Point bed_point = Utility.getHomeOfFarmer(farmer).GetPlayerBedSpot();
        bool has_bed = Utility.getHomeOfFarmer(farmer).GetPlayerBed() != null;
        if (farmer.currentLocation.GetLocationContext() == GameLocation.LocationContext.Island && Game1.getLocationFromName("IslandWest") is IslandWest locationFromName1 && locationFromName1.farmhouseRestored.Value && Game1.getLocationFromName("IslandFarmHouse") is IslandFarmHouse locationFromName2)
        {
          nameOrUniqueName = locationFromName2.NameOrUniqueName;
          bed_point = new Point(14, 17);
          has_bed = false;
          foreach (Furniture furniture in locationFromName2.furniture)
          {
            if (furniture is BedFurniture && (furniture as BedFurniture).bedType != BedFurniture.BedType.Child)
            {
              bed_point = (furniture as BedFurniture).GetBedSpot();
              has_bed = true;
              break;
            }
          }
        }
        if (Game1.IsServer && farmer != Game1.player)
        {
          object[] source = new object[4]
          {
            (object) nameOrUniqueName,
            (object) bed_point.X,
            (object) bed_point.Y,
            (object) has_bed
          };
          Game1.server.sendMessage(farmer.UniqueMultiplayerID, (byte) 29, Game1.player, ((IEnumerable<object>) source).ToArray<object>());
        }
        else
          Farmer.performPassoutWarp(farmer, nameOrUniqueName, bed_point, has_bed);
      }
    }

    public virtual void receivePassout(IncomingMessage msg)
    {
      if (msg.SourceFarmer != Game1.serverHost.Value)
        return;
      Farmer.performPassoutWarp(Game1.player, msg.Reader.ReadString(), new Point(msg.Reader.ReadInt32(), msg.Reader.ReadInt32()), msg.Reader.ReadBoolean());
    }

    public virtual void broadcastEvent(
      Event evt,
      GameLocation location,
      Vector2 positionBeforeEvent,
      bool use_local_farmer = true)
    {
      if (evt.id == -1)
        return;
      object[] objArray = new object[6]
      {
        (object) evt.id,
        (object) use_local_farmer,
        (object) (int) positionBeforeEvent.X,
        (object) (int) positionBeforeEvent.Y,
        (object) (byte) ((bool) (NetFieldBase<bool, NetBool>) location.isStructure ? 1 : 0),
        (bool) (NetFieldBase<bool, NetBool>) location.isStructure ? (object) location.uniqueName.Value : (object) location.Name
      };
      if (Game1.IsServer)
      {
        foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
        {
          if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
            Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 4, Game1.player, objArray);
        }
      }
      else
      {
        if (!Game1.IsClient)
          return;
        Game1.client.sendMessage((byte) 4, objArray);
      }
    }

    protected virtual void receiveRequestGrandpaReevaluation(IncomingMessage msg) => Game1.getFarm()?.requestGrandpaReevaluation();

    protected virtual void receiveFarmerKilledMonster(IncomingMessage msg)
    {
      if (msg.SourceFarmer != Game1.serverHost.Value)
        return;
      string name = msg.Reader.ReadString();
      if (name == null)
        return;
      Game1.stats.monsterKilled(name);
    }

    public virtual void broadcastRemoveLocationFromLookup(GameLocation location)
    {
      List<object> objectList = new List<object>();
      objectList.Add((object) location.NameOrUniqueName);
      if (Game1.IsServer)
      {
        foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
        {
          if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
            Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 24, Game1.player, objectList.ToArray());
        }
      }
      else
      {
        if (!Game1.IsClient)
          return;
        Game1.client.sendMessage((byte) 24, objectList.ToArray());
      }
    }

    public virtual void broadcastNutDig(GameLocation location, Point point)
    {
      if (Game1.IsMasterGame)
        this._performNutDig(location, point);
      else
        Game1.client.sendMessage((byte) 27, new List<object>()
        {
          (object) location.NameOrUniqueName,
          (object) point.X,
          (object) point.Y
        }.ToArray());
    }

    protected virtual void receiveNutDig(IncomingMessage msg)
    {
      if (!Game1.IsMasterGame)
        return;
      string name = msg.Reader.ReadString();
      Point point = new Point(msg.Reader.ReadInt32(), msg.Reader.ReadInt32());
      this._performNutDig(Game1.getLocationFromName(name), point);
    }

    protected virtual void _performNutDig(GameLocation location, Point point)
    {
      if (!(location is IslandLocation))
        return;
      IslandLocation location1 = location as IslandLocation;
      if (!location1.IsBuriedNutLocation(point))
        return;
      string key = location.NameOrUniqueName + "_" + point.X.ToString() + "_" + point.Y.ToString();
      if (Game1.netWorldState.Value.FoundBuriedNuts.ContainsKey(key))
        return;
      Game1.netWorldState.Value.FoundBuriedNuts[key] = true;
      Game1.createItemDebris((Item) new Object(73, 1), new Vector2((float) point.X, (float) point.Y) * 64f, -1, (GameLocation) location1);
    }

    public virtual void broadcastPartyWideMail(
      string mail_key,
      Multiplayer.PartyWideMessageQueue message_queue = Multiplayer.PartyWideMessageQueue.MailForTomorrow,
      bool no_letter = false)
    {
      mail_key = mail_key.Trim();
      mail_key = mail_key.Replace(Environment.NewLine, "");
      List<object> objectList = new List<object>();
      objectList.Add((object) mail_key);
      objectList.Add((object) (int) message_queue);
      objectList.Add((object) no_letter);
      this._performPartyWideMail(mail_key, message_queue, no_letter);
      if (Game1.IsServer)
      {
        foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
        {
          if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
            Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 22, Game1.player, objectList.ToArray());
        }
      }
      else
      {
        if (!Game1.IsClient)
          return;
        Game1.client.sendMessage((byte) 22, objectList.ToArray());
      }
    }

    public virtual void broadcastGrandpaReevaluation()
    {
      Game1.getFarm().requestGrandpaReevaluation();
      if (Game1.IsServer)
      {
        foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
        {
          if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
            Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 26, Game1.player);
        }
      }
      else
      {
        if (!Game1.IsClient)
          return;
        Game1.client.sendMessage((byte) 26);
      }
    }

    public virtual void broadcastGlobalMessage(
      string localization_string_key,
      bool only_show_if_empty = false,
      params string[] substitutions)
    {
      if (!only_show_if_empty || Game1.hudMessages.Count == 0)
        Game1.showGlobalMessage(Game1.content.LoadString(localization_string_key, (object[]) substitutions));
      List<object> objectList = new List<object>();
      objectList.Add((object) localization_string_key);
      objectList.Add((object) only_show_if_empty);
      objectList.Add((object) substitutions.Length);
      for (int index = 0; index < substitutions.Length; ++index)
        objectList.Add((object) substitutions[index]);
      if (Game1.IsServer)
      {
        foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
        {
          if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
            Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 21, Game1.player, objectList.ToArray());
        }
      }
      else
      {
        if (!Game1.IsClient)
          return;
        Game1.client.sendMessage((byte) 21, objectList.ToArray());
      }
    }

    public virtual NetRoot<T> readObjectFull<T>(BinaryReader reader) where T : class, INetObject<INetSerializable>
    {
      NetRoot<T> netRoot = NetRoot<T>.Connect(reader);
      netRoot.Clock.InterpolationTicks = this.defaultInterpolationTicks;
      return netRoot;
    }

    protected virtual BinaryWriter createWriter(Stream stream)
    {
      BinaryWriter writer = new BinaryWriter(stream);
      if (this.logging.IsLogging)
        writer = (BinaryWriter) new LoggingBinaryWriter(writer);
      return writer;
    }

    public virtual void writeObjectFull<T>(BinaryWriter writer, NetRoot<T> root, long? peer) where T : class, INetObject<INetSerializable> => root.CreateConnectionPacket(writer, peer);

    public virtual byte[] writeObjectFullBytes<T>(NetRoot<T> root, long? peer) where T : class, INetObject<INetSerializable>
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = this.createWriter((Stream) memoryStream))
        {
          root.CreateConnectionPacket(writer, peer);
          return memoryStream.ToArray();
        }
      }
    }

    public virtual void readObjectDelta<T>(BinaryReader reader, NetRoot<T> root) where T : class, INetObject<INetSerializable> => root.Read(reader);

    public virtual void writeObjectDelta<T>(BinaryWriter writer, NetRoot<T> root) where T : class, INetObject<INetSerializable> => root.Write(writer);

    public virtual byte[] writeObjectDeltaBytes<T>(NetRoot<T> root) where T : class, INetObject<INetSerializable>
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = this.createWriter((Stream) memoryStream))
        {
          root.Write(writer);
          return memoryStream.ToArray();
        }
      }
    }

    public virtual NetFarmerRoot readFarmer(BinaryReader reader)
    {
      NetFarmerRoot netFarmerRoot = new NetFarmerRoot();
      netFarmerRoot.ReadConnectionPacket(reader);
      netFarmerRoot.Clock.InterpolationTicks = this.defaultInterpolationTicks;
      return netFarmerRoot;
    }

    public virtual void addPlayer(NetFarmerRoot f)
    {
      long uniqueMultiplayerId = f.Value.UniqueMultiplayerID;
      f.Value.teamRoot = Game1.player.teamRoot;
      Game1.otherFarmers.Roots[uniqueMultiplayerId] = (NetRoot<Farmer>) f;
      this.disconnectingFarmers.Remove(uniqueMultiplayerId);
      if (Game1.chatBox == null)
        return;
      string sub1 = ChatBox.formattedUserNameLong(f.Value);
      Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_PlayerJoined", (object) sub1));
    }

    public virtual void receivePlayerIntroduction(BinaryReader reader) => this.addPlayer(this.readFarmer(reader));

    public virtual void broadcastPlayerIntroduction(NetFarmerRoot farmerRoot)
    {
      if (Game1.server == null)
        return;
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      {
        if (farmerRoot.Value.UniqueMultiplayerID != otherFarmer.Value.UniqueMultiplayerID)
          Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 2, farmerRoot.Value, (object) Game1.server.getUserName(farmerRoot.Value.UniqueMultiplayerID), (object) this.writeObjectFullBytes<Farmer>((NetRoot<Farmer>) farmerRoot, new long?(otherFarmer.Value.UniqueMultiplayerID)));
      }
    }

    public virtual void broadcastUserName(long farmerId, string userName)
    {
      if (Game1.server != null)
        return;
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      {
        Farmer farmer = otherFarmer.Value;
        if (farmer.UniqueMultiplayerID != farmerId)
          Game1.server.sendMessage(farmer.UniqueMultiplayerID, (byte) 16, Game1.serverHost.Value, (object) farmerId, (object) userName);
      }
    }

    public virtual string getUserName(long id)
    {
      if (id == Game1.player.UniqueMultiplayerID)
        return Game1.content.LoadString("Strings\\UI:Chat_SelfPlayerID");
      if (Game1.server != null)
        return Game1.server.getUserName(id);
      return Game1.client != null ? Game1.client.getUserName(id) : "?";
    }

    public virtual void playerDisconnected(long id)
    {
      if (!Game1.otherFarmers.ContainsKey(id) || this.disconnectingFarmers.Contains(id))
        return;
      NetFarmerRoot root = Game1.otherFarmers.Roots[id] as NetFarmerRoot;
      if (root.Value.mount != null && Game1.IsMasterGame)
        root.Value.mount.dismount();
      if (Game1.IsMasterGame)
      {
        this.saveFarmhand(root);
        root.Value.handleDisconnect();
      }
      if (Game1.player.dancePartner.Value is Farmer && ((Farmer) Game1.player.dancePartner.Value).UniqueMultiplayerID == root.Value.UniqueMultiplayerID)
        Game1.player.dancePartner.Value = (Character) null;
      if (Game1.chatBox != null)
        Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_PlayerLeft", (object) ChatBox.formattedUserNameLong(Game1.otherFarmers[id])));
      this.disconnectingFarmers.Add(id);
    }

    protected virtual void removeDisconnectedFarmers()
    {
      foreach (long disconnectingFarmer in this.disconnectingFarmers)
        Game1.otherFarmers.Remove(disconnectingFarmer);
      this.disconnectingFarmers.Clear();
    }

    public virtual void sendFarmhand() => (Game1.player.NetFields.Root as NetFarmerRoot).MarkReassigned();

    protected virtual void saveFarmhand(NetFarmerRoot farmhand)
    {
      FarmHouse homeOfFarmer = Utility.getHomeOfFarmer((Farmer) (NetFieldBase<Farmer, NetRef<Farmer>>) farmhand);
      if (!(homeOfFarmer is Cabin))
        return;
      (homeOfFarmer as Cabin).saveFarmhand(farmhand);
    }

    public virtual void saveFarmhands()
    {
      if (!Game1.IsMasterGame)
        return;
      foreach (NetRoot<Farmer> farmhand in Game1.otherFarmers.Roots.Values)
        this.saveFarmhand(farmhand as NetFarmerRoot);
    }

    public virtual void clientRemotelyDisconnected(Multiplayer.DisconnectType disconnectType)
    {
      Multiplayer.LogDisconnect(disconnectType);
      this.returnToMainMenu();
    }

    private void returnToMainMenu()
    {
      if (!Game1.game1.IsMainInstance)
        GameRunner.instance.RemoveGameInstance(Game1.game1);
      else
        Game1.ExitToTitle((Action) (() =>
        {
          (Game1.activeClickableMenu as TitleMenu).skipToTitleButtons();
          TitleMenu.subMenu = (IClickableMenu) new ConfirmationDialog(Game1.content.LoadString("Strings\\UI:Client_RemotelyDisconnected"), (ConfirmationDialog.behavior) null)
          {
            okButton = {
              visible = false
            }
          };
        }));
    }

    public static bool ShouldLogDisconnect(Multiplayer.DisconnectType disconnectType)
    {
      switch (disconnectType)
      {
        case Multiplayer.DisconnectType.ClosedGame:
        case Multiplayer.DisconnectType.ExitedToMainMenu:
        case Multiplayer.DisconnectType.ExitedToMainMenu_FromFarmhandSelect:
        case Multiplayer.DisconnectType.ServerOfflineMode:
        case Multiplayer.DisconnectType.ServerFull:
        case Multiplayer.DisconnectType.AcceptedOtherInvite:
          return false;
        default:
          return true;
      }
    }

    public static bool IsTimeout(Multiplayer.DisconnectType disconnectType)
    {
      switch (disconnectType)
      {
        case Multiplayer.DisconnectType.ClientTimeout:
        case Multiplayer.DisconnectType.LidgrenTimeout:
        case Multiplayer.DisconnectType.GalaxyTimeout:
          return true;
        default:
          return false;
      }
    }

    public static void LogDisconnect(Multiplayer.DisconnectType disconnectType)
    {
      if (Multiplayer.ShouldLogDisconnect(disconnectType))
      {
        string message = "Disconnected at : " + DateTime.Now.ToLongTimeString() + " - " + disconnectType.ToString();
        if (Game1.client != null)
          message = message + " Ping: " + Game1.client.GetPingToHost().ToString("0.#") + (Game1.client is LidgrenClient ? " ip" : " friend/invite");
        Program.WriteLog(Program.LogType.Disconnect, message, true);
      }
      Console.WriteLine("Disconnected: " + disconnectType.ToString());
    }

    public virtual void sendSharedAchievementMessage(int achievement)
    {
      if (Game1.IsClient)
      {
        Game1.client.sendMessage((byte) 20, (object) achievement);
      }
      else
      {
        if (!Game1.IsServer)
          return;
        foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
          Game1.server.sendMessage(key, (byte) 20, Game1.player, (object) achievement);
      }
    }

    public virtual void sendServerToClientsMessage(string message)
    {
      if (!Game1.IsServer)
        return;
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
        otherFarmer.Value.queueMessage((byte) 18, Game1.player, (object) message);
    }

    public virtual void sendChatMessage(
      LocalizedContentManager.LanguageCode language,
      string message,
      long recipientID)
    {
      if (Game1.IsClient)
      {
        Game1.client.sendMessage((byte) 10, (object) recipientID, (object) language, (object) message);
      }
      else
      {
        if (!Game1.IsServer)
          return;
        if (recipientID == Multiplayer.AllPlayers)
        {
          foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
            Game1.server.sendMessage(key, (byte) 10, Game1.player, (object) recipientID, (object) language, (object) message);
        }
        else
          Game1.server.sendMessage(recipientID, (byte) 10, Game1.player, (object) recipientID, (object) language, (object) message);
      }
    }

    public virtual void receiveChatMessage(
      Farmer sourceFarmer,
      long recipientID,
      LocalizedContentManager.LanguageCode language,
      string message)
    {
      if (Game1.chatBox == null)
        return;
      int chatKind = 0;
      message = Program.sdk.FilterDirtyWords(message);
      if (recipientID != Multiplayer.AllPlayers)
        chatKind = 3;
      Game1.chatBox.receiveChatMessage(sourceFarmer.UniqueMultiplayerID, chatKind, language, message);
    }

    public virtual void globalChatInfoMessage(string messageKey, params string[] args)
    {
      if (!Game1.IsMultiplayer && Game1.multiplayerMode == (byte) 0)
        return;
      this.receiveChatInfoMessage(Game1.player, messageKey, args);
      this.sendChatInfoMessage(messageKey, args);
    }

    public void globalChatInfoMessageEvenInSinglePlayer(string messageKey, params string[] args)
    {
      this.receiveChatInfoMessage(Game1.player, messageKey, args);
      this.sendChatInfoMessage(messageKey, args);
    }

    protected virtual void sendChatInfoMessage(string messageKey, params string[] args)
    {
      if (Game1.IsClient)
      {
        Game1.client.sendMessage((byte) 15, (object) messageKey, (object) args);
      }
      else
      {
        if (!Game1.IsServer)
          return;
        foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
          Game1.server.sendMessage(key, (byte) 15, Game1.player, (object) messageKey, (object) args);
      }
    }

    protected virtual void receiveChatInfoMessage(
      Farmer sourceFarmer,
      string messageKey,
      string[] args)
    {
      if (Game1.chatBox == null)
        return;
      try
      {
        string[] array = ((IEnumerable<string>) args).Select<string, string>((Func<string, string>) (arg =>
        {
          if (arg.StartsWith("achievement:"))
          {
            int int32 = Convert.ToInt32(arg.Substring("achievement:".Length));
            return Game1.content.Load<Dictionary<int, string>>("Data\\Achievements")[int32].Split('^')[0];
          }
          return arg.StartsWith("object:") ? new Object(Convert.ToInt32(arg.Substring("object:".Length)), 1).DisplayName : arg;
        })).ToArray<string>();
        Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_" + messageKey, (object[]) array));
      }
      catch (ContentLoadException ex)
      {
      }
      catch (FormatException ex)
      {
      }
      catch (OverflowException ex)
      {
      }
      catch (KeyNotFoundException ex)
      {
      }
    }

    public virtual void parseServerToClientsMessage(string message)
    {
      if (!Game1.IsClient)
        return;
      if (!(message == "festivalEvent"))
      {
        if (!(message == "endFest"))
        {
          if (!(message == "trainApproach"))
            return;
          GameLocation locationFromName = Game1.getLocationFromName("Railroad");
          if (locationFromName == null || !(locationFromName is Railroad))
            return;
          ((Railroad) locationFromName).PlayTrainApproach();
        }
        else
        {
          if (Game1.CurrentEvent == null)
            return;
          Game1.CurrentEvent.forceEndFestival(Game1.player);
        }
      }
      else
      {
        if (Game1.currentLocation.currentEvent == null)
          return;
        Game1.currentLocation.currentEvent.forceFestivalContinue();
      }
    }

    public virtual IEnumerable<GameLocation> activeLocations()
    {
      if (Game1.currentLocation != null)
        yield return Game1.currentLocation;
      Farm farm = Game1.getFarm();
      if (farm != null && farm != Game1.currentLocation)
        yield return (GameLocation) farm;
      GameLocation locationFromName1 = Game1.getLocationFromName("FarmHouse");
      if (locationFromName1 != null && locationFromName1 != Game1.currentLocation)
        yield return locationFromName1;
      GameLocation locationFromName2 = Game1.getLocationFromName("Greenhouse");
      if (locationFromName2 != null && locationFromName2 != Game1.currentLocation)
        yield return locationFromName2;
      foreach (Building building in farm.buildings)
      {
        if (building.indoors.Value != null && building.indoors.Value != Game1.currentLocation)
          yield return building.indoors.Value;
      }
    }

    public virtual bool isAlwaysActiveLocation(GameLocation location)
    {
      if (location.Name == "Farm" || location.Name == "FarmHouse" || location.Name == "Greenhouse")
        return true;
      return (NetFieldBase<GameLocation, NetRef<GameLocation>>) location.Root != (NetRef<GameLocation>) null && location.Root.Value.Equals((GameLocation) Game1.getFarm());
    }

    protected virtual void readActiveLocation(IncomingMessage msg)
    {
      bool flag = msg.Reader.ReadBoolean();
      NetRoot<GameLocation> netRoot = this.readObjectFull<GameLocation>(msg.Reader);
      if (this.isAlwaysActiveLocation(netRoot.Value))
      {
        for (int index = 0; index < Game1.locations.Count; ++index)
        {
          if (Game1.locations[index].Equals(netRoot.Value))
          {
            if (Game1.locations[index] != netRoot.Value)
            {
              if (Game1.locations[index] != null)
              {
                if (Game1.currentLocation == Game1.locations[index])
                  Game1.currentLocation = netRoot.Value;
                if (Game1.player.currentLocation == Game1.locations[index])
                  Game1.player.currentLocation = netRoot.Value;
                Game1.removeLocationFromLocationLookup(Game1.locations[index]);
              }
              Game1.locations[index] = netRoot.Value;
              break;
            }
            break;
          }
        }
      }
      if (!(Game1.locationRequest != null | flag))
        return;
      if (Game1.locationRequest != null)
      {
        Game1.currentLocation = Game1.findStructure(netRoot.Value, Game1.locationRequest.Name);
        if (Game1.currentLocation == null)
          Game1.currentLocation = netRoot.Value;
      }
      else if (flag)
        Game1.currentLocation = netRoot.Value;
      if (Game1.locationRequest != null)
      {
        Game1.locationRequest.Location = netRoot.Value;
        Game1.locationRequest.Loaded(netRoot.Value);
      }
      Game1.currentLocation.resetForPlayerEntry();
      Game1.player.currentLocation = Game1.currentLocation;
      if (Game1.locationRequest != null)
        Game1.locationRequest.Warped(netRoot.Value);
      Game1.currentLocation.updateSeasonalTileSheets();
      if (Game1.IsDebrisWeatherHere())
        Game1.populateDebrisWeatherArray();
      Game1.locationRequest = (LocationRequest) null;
    }

    public virtual bool isActiveLocation(GameLocation location) => Game1.IsMasterGame || Game1.currentLocation != null && (NetFieldBase<GameLocation, NetRef<GameLocation>>) Game1.currentLocation.Root != (NetRef<GameLocation>) null && Game1.currentLocation.Root.Value == location.Root.Value || this.isAlwaysActiveLocation(location);

    protected virtual GameLocation readLocation(BinaryReader reader)
    {
      bool isStructure = reader.ReadByte() > (byte) 0;
      GameLocation locationFromName = Game1.getLocationFromName(reader.ReadString(), isStructure);
      if (locationFromName == null || (NetFieldBase<GameLocation, NetRef<GameLocation>>) this.locationRoot(locationFromName) == (NetRef<GameLocation>) null)
        return (GameLocation) null;
      return !this.isActiveLocation(locationFromName) ? (GameLocation) null : locationFromName;
    }

    protected virtual LocationRequest readLocationRequest(BinaryReader reader)
    {
      bool isStructure = reader.ReadByte() > (byte) 0;
      return Game1.getLocationRequest(reader.ReadString(), isStructure);
    }

    protected virtual void readWarp(BinaryReader reader, int tileX, int tileY, Action afterWarp)
    {
      LocationRequest locationRequest = this.readLocationRequest(reader);
      if (afterWarp != null)
        locationRequest.OnWarp += new LocationRequest.Callback(afterWarp.Invoke);
      Game1.warpFarmer(locationRequest, tileX, tileY, Game1.player.FacingDirection);
    }

    protected virtual NPC readNPC(BinaryReader reader)
    {
      GameLocation gameLocation = this.readLocation(reader);
      Guid guid = reader.ReadGuid();
      return !gameLocation.characters.ContainsGuid(guid) ? (NPC) null : gameLocation.characters[guid];
    }

    public virtual TemporaryAnimatedSprite[] readSprites(
      BinaryReader reader,
      GameLocation location)
    {
      int length = reader.ReadInt32();
      TemporaryAnimatedSprite[] temporaryAnimatedSpriteArray = new TemporaryAnimatedSprite[length];
      for (int index = 0; index < length; ++index)
      {
        TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite();
        temporaryAnimatedSprite.Read(reader, location);
        temporaryAnimatedSprite.ticksBeforeAnimationStart += this.interpolationTicks();
        temporaryAnimatedSpriteArray[index] = temporaryAnimatedSprite;
      }
      return temporaryAnimatedSpriteArray;
    }

    protected virtual void receiveTeamDelta(BinaryReader msg) => this.readObjectDelta<FarmerTeam>(msg, Game1.player.teamRoot);

    protected virtual void receiveNewDaySync(IncomingMessage msg)
    {
      if (Game1.newDaySync == null && msg.SourceFarmer == Game1.serverHost.Value)
        Game1.NewDay(0.0f);
      if (Game1.newDaySync == null)
        return;
      Game1.newDaySync.receiveMessage(msg);
    }

    protected virtual void receiveFarmerGainExperience(IncomingMessage msg)
    {
      if (msg.SourceFarmer != Game1.serverHost.Value)
        return;
      Game1.player.gainExperience(msg.Reader.ReadInt32(), msg.Reader.ReadInt32());
    }

    protected virtual void receiveSharedAchievement(IncomingMessage msg) => Game1.getAchievement(msg.Reader.ReadInt32(), false);

    protected virtual void receiveRemoveLocationFromLookup(IncomingMessage msg) => Game1.removeLocationFromLocationLookup(msg.Reader.ReadString());

    protected virtual void receivePartyWideMail(IncomingMessage msg) => this._performPartyWideMail(msg.Reader.ReadString(), (Multiplayer.PartyWideMessageQueue) msg.Reader.ReadInt32(), msg.Reader.ReadBoolean());

    protected void _performPartyWideMail(
      string mail_key,
      Multiplayer.PartyWideMessageQueue message_queue,
      bool no_letter)
    {
      switch (message_queue)
      {
        case Multiplayer.PartyWideMessageQueue.MailForTomorrow:
          Game1.addMailForTomorrow(mail_key, no_letter);
          break;
        case Multiplayer.PartyWideMessageQueue.SeenMail:
          Game1.addMail(mail_key, no_letter);
          break;
      }
      if (no_letter)
        mail_key += "%&NL&%";
      switch (message_queue)
      {
        case Multiplayer.PartyWideMessageQueue.MailForTomorrow:
          mail_key = "%&MFT&%" + mail_key;
          break;
        case Multiplayer.PartyWideMessageQueue.SeenMail:
          mail_key = "%&SM&%" + mail_key;
          break;
      }
      if (!Game1.IsMasterGame || Game1.player.team.broadcastedMail.Contains(mail_key))
        return;
      Game1.player.team.broadcastedMail.Add(mail_key);
    }

    protected void receiveForceKick()
    {
      if (Game1.IsServer)
        return;
      this.Disconnect(Multiplayer.DisconnectType.Kicked);
      this.returnToMainMenu();
    }

    protected virtual void receiveGlobalMessage(IncomingMessage msg)
    {
      string path = msg.Reader.ReadString();
      if (msg.Reader.ReadBoolean() && Game1.hudMessages.Count > 0)
        return;
      int length = msg.Reader.ReadInt32();
      object[] objArray = new object[length];
      for (int index = 0; index < length; ++index)
        objArray[index] = (object) msg.Reader.ReadString();
      Game1.showGlobalMessage(Game1.content.LoadString(path, objArray));
    }

    public virtual void processIncomingMessage(IncomingMessage msg)
    {
      switch (msg.MessageType)
      {
        case 0:
          NetFarmerRoot root = this.farmerRoot(msg.Reader.ReadInt64());
          if (!((NetFieldBase<Farmer, NetRef<Farmer>>) root != (NetRef<Farmer>) null))
            break;
          this.readObjectDelta<Farmer>(msg.Reader, (NetRoot<Farmer>) root);
          break;
        case 2:
          this.receivePlayerIntroduction(msg.Reader);
          break;
        case 3:
          this.readActiveLocation(msg);
          break;
        case 4:
          int eventId = msg.Reader.ReadInt32();
          bool flag = msg.Reader.ReadBoolean();
          int tileX = msg.Reader.ReadInt32();
          int tileY = msg.Reader.ReadInt32();
          LocationRequest request = this.readLocationRequest(msg.Reader);
          GameLocation locationFromName = Game1.getLocationFromName(request.Name);
          if (locationFromName == null || locationFromName.findEventById(eventId) == null)
          {
            Console.WriteLine("Couldn't find event " + eventId.ToString() + " for broadcast event!");
            break;
          }
          Farmer farmerActor = (Farmer) null;
          farmerActor = !flag ? (msg.SourceFarmer.NetFields.Root as NetRoot<Farmer>).Clone().Value : (Game1.player.NetFields.Root as NetRoot<Farmer>).Clone().Value;
          int old_x = (int) Game1.player.getTileLocation().X;
          int old_y = (int) Game1.player.getTileLocation().Y;
          string old_location = Game1.player.currentLocation.NameOrUniqueName;
          int direction = Game1.player.facingDirection.Value;
          Game1.player.locationBeforeForcedEvent.Value = old_location;
          request.OnWarp += (LocationRequest.Callback) (() =>
          {
            farmerActor.currentLocation = Game1.currentLocation;
            farmerActor.completelyStopAnimatingOrDoingAction();
            farmerActor.UsingTool = false;
            farmerActor.items.Clear();
            farmerActor.hidden.Value = false;
            Event eventById = Game1.currentLocation.findEventById(eventId, farmerActor);
            Game1.currentLocation.startEvent(eventById);
            farmerActor.Position = Game1.player.Position;
            Game1.warpingForForcedRemoteEvent = false;
            string str = Game1.player.locationBeforeForcedEvent.Value;
            Game1.player.locationBeforeForcedEvent.Value = (string) null;
            eventById.setExitLocation(old_location, old_x, old_y);
            Game1.player.locationBeforeForcedEvent.Value = str;
            Game1.player.orientationBeforeEvent = direction;
          });
          Action action = (Action) (() =>
          {
            Game1.warpingForForcedRemoteEvent = true;
            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.warpFarmer(request, tileX, tileY, Game1.player.FacingDirection);
          });
          Game1.remoteEventQueue.Add(action);
          break;
        case 6:
          GameLocation gameLocation = this.readLocation(msg.Reader);
          if (gameLocation == null)
            break;
          this.readObjectDelta<GameLocation>(msg.Reader, gameLocation.Root);
          break;
        case 7:
          GameLocation location = this.readLocation(msg.Reader);
          location?.temporarySprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) this.readSprites(msg.Reader, location));
          break;
        case 8:
          NPC character = this.readNPC(msg.Reader);
          GameLocation targetLocation = this.readLocation(msg.Reader);
          if (character == null || targetLocation == null)
            break;
          Game1.warpCharacter(character, targetLocation, msg.Reader.ReadVector2());
          break;
        case 10:
          long recipientID = msg.Reader.ReadInt64();
          LocalizedContentManager.LanguageCode language = msg.Reader.ReadEnum<LocalizedContentManager.LanguageCode>();
          string message = msg.Reader.ReadString();
          this.receiveChatMessage(msg.SourceFarmer, recipientID, language, message);
          break;
        case 12:
          this.receiveWorldState(msg.Reader);
          break;
        case 13:
          this.receiveTeamDelta(msg.Reader);
          break;
        case 14:
          this.receiveNewDaySync(msg);
          break;
        case 15:
          string messageKey = msg.Reader.ReadString();
          string[] args = new string[(int) msg.Reader.ReadByte()];
          for (int index = 0; index < args.Length; ++index)
            args[index] = msg.Reader.ReadString();
          this.receiveChatInfoMessage(msg.SourceFarmer, messageKey, args);
          break;
        case 17:
          this.receiveFarmerGainExperience(msg);
          break;
        case 18:
          this.parseServerToClientsMessage(msg.Reader.ReadString());
          break;
        case 19:
          this.playerDisconnected(msg.SourceFarmer.UniqueMultiplayerID);
          break;
        case 20:
          this.receiveSharedAchievement(msg);
          break;
        case 21:
          this.receiveGlobalMessage(msg);
          break;
        case 22:
          this.receivePartyWideMail(msg);
          break;
        case 23:
          this.receiveForceKick();
          break;
        case 24:
          this.receiveRemoveLocationFromLookup(msg);
          break;
        case 25:
          this.receiveFarmerKilledMonster(msg);
          break;
        case 26:
          this.receiveRequestGrandpaReevaluation(msg);
          break;
        case 27:
          this.receiveNutDig(msg);
          break;
        case 28:
          this.receivePassoutRequest(msg);
          break;
        case 29:
          this.receivePassout(msg);
          break;
      }
    }

    public virtual void StartLocalMultiplayerServer()
    {
      Game1.server = (IGameServer) new GameServer(true);
      Game1.server.startServer();
    }

    public virtual void StartServer()
    {
      Game1.server = (IGameServer) new GameServer();
      Game1.server.startServer();
    }

    public virtual void Disconnect(Multiplayer.DisconnectType disconnectType)
    {
      if (Game1.server != null)
      {
        Game1.server.stopServer();
        Game1.server = (IGameServer) null;
        foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
          this.playerDisconnected(key);
      }
      if (Game1.client != null)
      {
        this.sendFarmhand();
        this.UpdateLate(true);
        Game1.client.disconnect();
        Game1.client = (Client) null;
      }
      Game1.otherFarmers.Clear();
      Multiplayer.LogDisconnect(disconnectType);
    }

    protected virtual void updatePendingConnections()
    {
      switch (Game1.multiplayerMode)
      {
        case 1:
          if (Game1.client == null || Game1.client.readyToPlay)
            break;
          Game1.client.receiveMessages();
          break;
        case 2:
          if (Game1.server != null || !Game1.options.enableServer)
            break;
          this.StartServer();
          break;
      }
    }

    public void UpdateLoading()
    {
      this.updatePendingConnections();
      if (Game1.server == null)
        return;
      Game1.server.receiveMessages();
    }

    public virtual void UpdateEarly()
    {
      this.updatePendingConnections();
      if (Game1.multiplayerMode == (byte) 2 && (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null && Game1.options.enableServer)
        Game1.server.initializeHost();
      if (Game1.server != null)
        Game1.server.receiveMessages();
      else if (Game1.client != null)
        Game1.client.receiveMessages();
      this.updateRoots();
      if (Game1.CurrentEvent != null)
        return;
      this.removeDisconnectedFarmers();
    }

    public virtual void UpdateLate(bool forceSync = false)
    {
      if (Game1.multiplayerMode != (byte) 0)
      {
        if (!this.allowSyncDelay() | forceSync || Game1.ticks % this.farmerDeltaBroadcastPeriod == 0)
          this.broadcastFarmerDeltas();
        if (!this.allowSyncDelay() | forceSync || Game1.ticks % this.locationDeltaBroadcastPeriod == 0)
          this.broadcastLocationDeltas();
        if (!this.allowSyncDelay() | forceSync || Game1.ticks % this.worldStateDeltaBroadcastPeriod == 0)
          this.broadcastWorldStateDeltas();
      }
      if (Game1.server != null)
        Game1.server.sendMessages();
      if (Game1.client == null)
        return;
      Game1.client.sendMessages();
    }

    public virtual void inviteAccepted()
    {
      if (!(Game1.activeClickableMenu is TitleMenu))
        return;
      TitleMenu activeClickableMenu = Game1.activeClickableMenu as TitleMenu;
      switch (TitleMenu.subMenu)
      {
        case null:
          activeClickableMenu.performButtonAction("Invite");
          break;
        case FarmhandMenu _:
        case CoopMenu _:
          TitleMenu.subMenu = (IClickableMenu) new FarmhandMenu();
          break;
      }
    }

    public virtual Client InitClient(Client client) => client;

    public virtual Server InitServer(Server server) => server;

    public static string MessageTypeToString(byte type)
    {
      switch (type)
      {
        case 0:
          return "farmerDelta";
        case 1:
          return "serverIntroduction";
        case 2:
          return "playerIntroduction";
        case 3:
          return "locationIntroduction";
        case 4:
          return "forceEvent";
        case 5:
          return "warpFarmer";
        case 6:
          return "locationDelta";
        case 7:
          return "locationSprites";
        case 8:
          return "characterWarp";
        case 9:
          return "availableFarmhands";
        case 10:
          return "chatMessage";
        case 11:
          return "connectionMessage";
        case 12:
          return "worldDelta";
        case 13:
          return "teamDelta";
        case 14:
          return "newDaySync";
        case 15:
          return "chatInfoMessage";
        case 16:
          return "userNameUpdate";
        case 17:
          return "farmerGainExperience";
        case 18:
          return "serverToClientsMessage";
        case 19:
          return "disconnecting";
        case 20:
          return "sharedAchievement";
        case 21:
          return "globalMessage";
        case 22:
          return "partyWideMail";
        case 23:
          return "forceKick";
        case 24:
          return "removeLocationFromLookup";
        case 25:
          return "farmerKilledMonster";
        case 26:
          return "requestGrandpaReevaluation";
        default:
          return type.ToString();
      }
    }

    public enum PartyWideMessageQueue
    {
      MailForTomorrow,
      SeenMail,
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private struct FarmerRoots : IEnumerable<NetFarmerRoot>, IEnumerable
    {
      public Multiplayer.FarmerRoots.Enumerator GetEnumerator() => new Multiplayer.FarmerRoots.Enumerator(true);

      IEnumerator<NetFarmerRoot> IEnumerable<NetFarmerRoot>.GetEnumerator() => (IEnumerator<NetFarmerRoot>) new Multiplayer.FarmerRoots.Enumerator(true);

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new Multiplayer.FarmerRoots.Enumerator(true);

      public struct Enumerator : IEnumerator<NetFarmerRoot>, IEnumerator, IDisposable
      {
        private Dictionary<long, NetRoot<Farmer>>.Enumerator _enumerator;
        private NetFarmerRoot _current;
        private int _step;
        private bool _done;

        public Enumerator(bool dummy)
        {
          this._enumerator = Game1.otherFarmers.Roots.GetEnumerator();
          this._current = (NetFarmerRoot) null;
          this._step = 0;
          this._done = false;
        }

        public bool MoveNext()
        {
          if (this._step == 0)
          {
            ++this._step;
            if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null)
            {
              this._current = Game1.serverHost;
              return true;
            }
          }
          while (this._enumerator.MoveNext())
          {
            NetRoot<Farmer> netRoot = this._enumerator.Current.Value;
            if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null || (NetFieldBase<Farmer, NetRef<Farmer>>) netRoot != (NetRef<Farmer>) Game1.serverHost)
            {
              this._current = netRoot as NetFarmerRoot;
              return true;
            }
          }
          this._done = true;
          this._current = (NetFarmerRoot) null;
          return false;
        }

        public NetFarmerRoot Current => this._current;

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
          get
          {
            if (this._done)
              throw new InvalidOperationException();
            return (object) this._current;
          }
        }

        void IEnumerator.Reset()
        {
          this._enumerator = Game1.otherFarmers.Roots.GetEnumerator();
          this._current = (NetFarmerRoot) null;
          this._step = 0;
          this._done = false;
        }
      }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct ActiveLocations : IEnumerable<GameLocation>, IEnumerable
    {
      public Multiplayer.ActiveLocations.Enumerator GetEnumerator() => new Multiplayer.ActiveLocations.Enumerator();

      IEnumerator<GameLocation> IEnumerable<GameLocation>.GetEnumerator() => (IEnumerator<GameLocation>) new Multiplayer.ActiveLocations.Enumerator();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new Multiplayer.ActiveLocations.Enumerator();

      public struct Enumerator : IEnumerator<GameLocation>, IEnumerator, IDisposable
      {
        private List<Building>.Enumerator _enumerator;
        private GameLocation _current;
        private int _step;
        private bool _done;

        public bool MoveNext()
        {
          if (this._step == 0)
          {
            ++this._step;
            if (Game1.currentLocation != null)
            {
              this._current = Game1.currentLocation;
              return true;
            }
          }
          if (this._step == 1)
          {
            ++this._step;
            Farm farm = Game1.getFarm();
            if (farm != null && farm != Game1.currentLocation)
            {
              this._current = (GameLocation) farm;
              return true;
            }
          }
          if (this._step == 2)
          {
            ++this._step;
            GameLocation locationFromName = Game1.getLocationFromName("FarmHouse");
            if (locationFromName != null && locationFromName != Game1.currentLocation)
            {
              this._current = locationFromName;
              return true;
            }
          }
          if (this._step == 3)
          {
            ++this._step;
            GameLocation locationFromName = Game1.getLocationFromName("Greenhouse");
            if (locationFromName != null && locationFromName != Game1.currentLocation)
            {
              this._current = locationFromName;
              return true;
            }
          }
          if (this._step == 4)
          {
            ++this._step;
            this._enumerator = Game1.getFarm().buildings.GetEnumerator();
          }
          while (this._enumerator.MoveNext())
          {
            GameLocation gameLocation = this._enumerator.Current.indoors.Value;
            if (gameLocation != null && gameLocation != Game1.currentLocation)
            {
              this._current = gameLocation;
              return true;
            }
          }
          this._done = true;
          this._current = (GameLocation) null;
          return false;
        }

        public GameLocation Current => this._current;

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
          get
          {
            if (this._done)
              throw new InvalidOperationException();
            return (object) this._current;
          }
        }

        void IEnumerator.Reset()
        {
          this._current = (GameLocation) null;
          this._step = 0;
          this._done = false;
        }
      }
    }

    public enum DisconnectType
    {
      None,
      ClosedGame,
      ExitedToMainMenu,
      ExitedToMainMenu_FromFarmhandSelect,
      HostLeft,
      ServerOfflineMode,
      ServerFull,
      Kicked,
      AcceptedOtherInvite,
      ClientTimeout,
      LidgrenTimeout,
      GalaxyTimeout,
      Timeout_FarmhandSelection,
      LidgrenDisconnect_Unknown,
    }
  }
}
