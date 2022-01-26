// Decompiled with JetBrains decompiler
// Type: StardewValley.GameLocation
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.GameData.Movies;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley
{
  [XmlInclude(typeof (Farm))]
  [XmlInclude(typeof (Beach))]
  [XmlInclude(typeof (AnimalHouse))]
  [XmlInclude(typeof (SlimeHutch))]
  [XmlInclude(typeof (Shed))]
  [XmlInclude(typeof (LibraryMuseum))]
  [XmlInclude(typeof (AdventureGuild))]
  [XmlInclude(typeof (Woods))]
  [XmlInclude(typeof (Railroad))]
  [XmlInclude(typeof (Summit))]
  [XmlInclude(typeof (Forest))]
  [XmlInclude(typeof (ShopLocation))]
  [XmlInclude(typeof (SeedShop))]
  [XmlInclude(typeof (FishShop))]
  [XmlInclude(typeof (BathHousePool))]
  [XmlInclude(typeof (FarmHouse))]
  [XmlInclude(typeof (Cabin))]
  [XmlInclude(typeof (Club))]
  [XmlInclude(typeof (BusStop))]
  [XmlInclude(typeof (CommunityCenter))]
  [XmlInclude(typeof (Desert))]
  [XmlInclude(typeof (FarmCave))]
  [XmlInclude(typeof (JojaMart))]
  [XmlInclude(typeof (MineShaft))]
  [XmlInclude(typeof (Mountain))]
  [XmlInclude(typeof (Sewer))]
  [XmlInclude(typeof (WizardHouse))]
  [XmlInclude(typeof (Town))]
  [XmlInclude(typeof (Cellar))]
  [XmlInclude(typeof (Submarine))]
  [XmlInclude(typeof (MermaidHouse))]
  [XmlInclude(typeof (BeachNightMarket))]
  [XmlInclude(typeof (MovieTheater))]
  [XmlInclude(typeof (ManorHouse))]
  [XmlInclude(typeof (AbandonedJojaMart))]
  [XmlInclude(typeof (Mine))]
  [XmlInclude(typeof (BoatTunnel))]
  [XmlInclude(typeof (IslandWest))]
  [XmlInclude(typeof (IslandEast))]
  [XmlInclude(typeof (IslandSouth))]
  [XmlInclude(typeof (IslandNorth))]
  [XmlInclude(typeof (IslandSouthEast))]
  [XmlInclude(typeof (IslandSouthEastCave))]
  [XmlInclude(typeof (IslandFarmCave))]
  [XmlInclude(typeof (IslandWestCave1))]
  [XmlInclude(typeof (IslandFieldOffice))]
  [XmlInclude(typeof (IslandHut))]
  [XmlInclude(typeof (IslandFarmHouse))]
  [XmlInclude(typeof (IslandSecret))]
  [XmlInclude(typeof (IslandShrine))]
  [XmlInclude(typeof (IslandLocation))]
  [XmlInclude(typeof (IslandForestLocation))]
  [XmlInclude(typeof (Caldera))]
  [XmlInclude(typeof (BugLand))]
  [InstanceStatics]
  public class GameLocation : INetObject<NetFields>, IEquatable<GameLocation>
  {
    public const int minDailyWeeds = 5;
    public const int maxDailyWeeds = 12;
    public const int minDailyObjectSpawn = 1;
    public const int maxDailyObjectSpawn = 4;
    public const int maxSpawnedObjectsAtOnce = 6;
    public const int maxTriesForDebrisPlacement = 3;
    public const int maxTriesForObjectSpawn = 6;
    public const double chanceForStumpOrBoulderRespawn = 0.2;
    public const double chanceForClay = 0.03;
    public const string OVERRIDE_MAP_TILESHEET_PREFIX = "zzzzz";
    public const string PHONE_DIAL_SOUND = "telephone_buttonPush";
    public const int PHONE_RING_DURATION = 4950;
    public const string PHONE_PICKUP_SOUND = "bigSelect";
    public const string PHONE_HANGUP_SOUND = "openBox";
    public const int forageDataIndex = 0;
    public const int fishDataIndex = 4;
    public const int diggablesDataIndex = 8;
    [XmlIgnore]
    public string seasonOverride;
    [XmlIgnore]
    public GameLocation.LocationContext locationContext = ~GameLocation.LocationContext.Default;
    [XmlIgnore]
    public GameLocation.afterQuestionBehavior afterQuestion;
    [XmlIgnore]
    public Map map;
    [XmlIgnore]
    public readonly NetString mapPath = new NetString().Interpolated(false, false);
    [XmlIgnore]
    protected string loadedMapPath;
    public readonly NetCollection<NPC> characters = new NetCollection<NPC>();
    [XmlIgnore]
    public readonly NetVector2Dictionary<Object, NetRef<Object>> netObjects = new NetVector2Dictionary<Object, NetRef<Object>>();
    [XmlIgnore]
    public readonly Dictionary<Vector2, Object> overlayObjects = new Dictionary<Vector2, Object>((IEqualityComparer<Vector2>) GameLocation.tilePositionComparer);
    [XmlElement("objects")]
    public readonly OverlaidDictionary objects;
    private readonly List<Object> tempObjects = new List<Object>();
    [XmlIgnore]
    public NetList<MapSeat, NetRef<MapSeat>> mapSeats = new NetList<MapSeat, NetRef<MapSeat>>();
    protected bool _mapSeatsDirty;
    [XmlIgnore]
    private List<KeyValuePair<Vector2, Object>> _objectUpdateList = new List<KeyValuePair<Vector2, Object>>();
    [XmlIgnore]
    public List<TemporaryAnimatedSprite> temporarySprites = new List<TemporaryAnimatedSprite>();
    [XmlIgnore]
    public List<Action> postFarmEventOvernightActions = new List<Action>();
    [XmlIgnore]
    public readonly NetObjectList<Warp> warps = new NetObjectList<Warp>();
    [XmlIgnore]
    public readonly NetPointDictionary<string, NetString> doors = new NetPointDictionary<string, NetString>();
    [XmlIgnore]
    public readonly InteriorDoorDictionary interiorDoors;
    [XmlIgnore]
    public readonly FarmerCollection farmers;
    [XmlIgnore]
    public readonly NetCollection<Projectile> projectiles = new NetCollection<Projectile>();
    public readonly NetCollection<ResourceClump> resourceClumps = new NetCollection<ResourceClump>();
    public readonly NetCollection<LargeTerrainFeature> largeTerrainFeatures = new NetCollection<LargeTerrainFeature>();
    [XmlIgnore]
    public List<TerrainFeature> _activeTerrainFeatures = new List<TerrainFeature>();
    [XmlIgnore]
    public List<Critter> critters;
    [XmlElement("terrainFeatures")]
    public readonly NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = new NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>();
    [XmlIgnore]
    public readonly NetCollection<Debris> debris = new NetCollection<Debris>();
    [XmlIgnore]
    public readonly NetPoint fishSplashPoint = new NetPoint(Point.Zero);
    [XmlIgnore]
    public readonly NetPoint orePanPoint = new NetPoint(Point.Zero);
    [XmlIgnore]
    public TemporaryAnimatedSprite fishSplashAnimation;
    [XmlIgnore]
    public TemporaryAnimatedSprite orePanAnimation;
    [XmlIgnore]
    public WaterTiles waterTiles;
    [XmlIgnore]
    protected HashSet<string> _appliedMapOverrides;
    [XmlElement("uniqueName")]
    public readonly NetString uniqueName = new NetString();
    [XmlElement("name")]
    public readonly NetString name = new NetString();
    [XmlElement("waterColor")]
    public readonly NetColor waterColor = new NetColor(Microsoft.Xna.Framework.Color.White * 0.33f);
    [XmlIgnore]
    public string lastQuestionKey;
    [XmlIgnore]
    public Vector2 lastTouchActionLocation = Vector2.Zero;
    [XmlElement("lightLevel")]
    protected readonly NetFloat lightLevel = new NetFloat(0.0f);
    [XmlElement("isFarm")]
    public readonly NetBool isFarm = new NetBool();
    [XmlElement("isOutdoors")]
    public readonly NetBool isOutdoors = new NetBool();
    [XmlIgnore]
    public readonly NetBool isGreenhouse = new NetBool();
    [XmlElement("isStructure")]
    public readonly NetBool isStructure = new NetBool();
    [XmlElement("ignoreDebrisWeather")]
    public readonly NetBool ignoreDebrisWeather = new NetBool();
    [XmlElement("ignoreOutdoorLighting")]
    public readonly NetBool ignoreOutdoorLighting = new NetBool();
    [XmlElement("ignoreLights")]
    public readonly NetBool ignoreLights = new NetBool();
    [XmlElement("treatAsOutdoors")]
    public readonly NetBool treatAsOutdoors = new NetBool();
    [XmlIgnore]
    public bool wasUpdated;
    private List<Vector2> terrainFeaturesToRemoveList = new List<Vector2>();
    public int numberOfSpawnedObjectsOnMap;
    [XmlIgnore]
    public bool showDropboxIndicator;
    [XmlIgnore]
    public Vector2 dropBoxIndicatorLocation;
    [XmlElement("miniJukeboxCount")]
    public readonly NetInt miniJukeboxCount = new NetInt();
    [XmlElement("miniJukeboxTrack")]
    public readonly NetString miniJukeboxTrack = new NetString("");
    [XmlIgnore]
    public readonly NetString randomMiniJukeboxTrack = new NetString();
    [XmlIgnore]
    public Event currentEvent;
    [XmlIgnore]
    public Object actionObjectForQuestionDialogue;
    [XmlIgnore]
    public int waterAnimationIndex;
    [XmlIgnore]
    public int waterAnimationTimer;
    [XmlIgnore]
    public bool waterTileFlip;
    [XmlIgnore]
    public bool forceViewportPlayerFollow;
    [XmlIgnore]
    public bool forceLoadPathLayerLights;
    [XmlIgnore]
    public float waterPosition;
    [XmlIgnore]
    public readonly NetAudio netAudio;
    [XmlIgnore]
    public readonly NetIntDictionary<LightSource, NetRef<LightSource>> sharedLights = new NetIntDictionary<LightSource, NetRef<LightSource>>();
    private readonly NetEvent1Field<float, NetFloat> removeTemporarySpritesWithIDEvent = new NetEvent1Field<float, NetFloat>();
    private readonly NetEvent1Field<int, NetInt> rumbleAndFadeEvent = new NetEvent1Field<int, NetInt>();
    private readonly NetEvent1<GameLocation.DamagePlayersEventArg> damagePlayersEvent = new NetEvent1<GameLocation.DamagePlayersEventArg>();
    [XmlIgnore]
    public NetList<Vector2, NetVector2> lightGlows = new NetList<Vector2, NetVector2>();
    public static readonly int JOURNAL_INDEX = 1000;
    public static readonly float FIRST_SECRET_NOTE_CHANCE = 0.8f;
    public static readonly float LAST_SECRET_NOTE_CHANCE = 0.12f;
    public static readonly int NECKLACE_SECRET_NOTE_INDEX = 25;
    public static readonly int CAROLINES_NECKLACE_ITEM = 191;
    public static readonly string CAROLINES_NECKLACE_MAIL = "carolinesNecklace";
    public static TilePositionComparer tilePositionComparer = new TilePositionComparer();
    protected List<Vector2> _startingCabinLocations = new List<Vector2>();
    [XmlIgnore]
    public bool wasInhabited;
    [XmlIgnore]
    protected bool _madeMapModifications;
    /// <summary>
    /// Used for modders to store metadata to this object. This data is synchronized in multiplayer and saved to the save data.
    /// </summary>
    [XmlIgnore]
    public ModDataDictionary modData = new ModDataDictionary();
    public readonly NetCollection<Furniture> furniture = new NetCollection<Furniture>();
    protected readonly NetMutexQueue<Guid> furnitureToRemove = new NetMutexQueue<Guid>();
    protected bool ignoreWarps;
    private Vector2 snowPos;
    private const int fireIDBase = 944468;
    private static HashSet<int> secretNotesSeen = new HashSet<int>();
    private static List<int> unseenSecretNotes = new List<int>();
    private static HashSet<int> journalsSeen = new HashSet<int>();
    private static List<int> unseenJournals = new List<int>();
    private static readonly char[] ForwardSlash = new char[1]
    {
      '/'
    };

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    [XmlIgnore]
    public NetRoot<GameLocation> Root => this.NetFields.Root as NetRoot<GameLocation>;

    [XmlIgnore]
    public string NameOrUniqueName => this.uniqueName.Value != null ? this.uniqueName.Value : this.name.Value;

    [XmlIgnore]
    public float LightLevel
    {
      get => (float) (NetFieldBase<float, NetFloat>) this.lightLevel;
      set => this.lightLevel.Value = value;
    }

    [XmlIgnore]
    public Map Map
    {
      get
      {
        this.updateMap();
        return this.map;
      }
      set => this.map = value;
    }

    [XmlIgnore]
    public OverlaidDictionary Objects => this.objects;

    [XmlIgnore]
    public List<TemporaryAnimatedSprite> TemporarySprites => this.temporarySprites;

    public string Name => (string) (NetFieldBase<string, NetString>) this.name;

    [XmlIgnore]
    public bool IsFarm
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isFarm;
      set => this.isFarm.Value = value;
    }

    [XmlIgnore]
    public bool IsOutdoors
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isOutdoors;
      set => this.isOutdoors.Value = value;
    }

    public bool IsGreenhouse
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isGreenhouse;
      set => this.isGreenhouse.Value = value;
    }

    public virtual bool SeedsIgnoreSeasonsHere() => this.IsGreenhouse;

    public virtual bool CanPlantSeedsHere(int crop_index, int tile_x, int tile_y) => this.IsGreenhouse;

    public virtual bool CanPlantTreesHere(int sapling_index, int tile_x, int tile_y)
    {
      if (Object.isWildTreeSeed(sapling_index) && this.IsOutdoors && this.doesTileHavePropertyNoNull(tile_x, tile_y, "Type", "Back") == "Dirt")
        return true;
      bool flag = false;
      if (this.map != null && this.map.Properties.ContainsKey("ForceAllowTreePlanting"))
        flag = true;
      return this.IsGreenhouse | flag;
    }

    /// <summary>Get the mod populated metadata as it will be serialized for game saving. Identical to <see cref="F:StardewValley.GameLocation.modData" /> except returns null during save if it is empty. It is strongly recommended to use <see cref="F:StardewValley.GameLocation.modData" /> instead.</summary>
    [XmlElement("modData")]
    public ModDataDictionary modDataForSerialization
    {
      get => this.modData.GetForSerialization();
      set => this.modData.SetFromSerialization(value);
    }

    protected virtual void initNetFields()
    {
      this.NetFields.AddFields((INetSerializable) this.mapPath, (INetSerializable) this.uniqueName, (INetSerializable) this.name, (INetSerializable) this.lightLevel, (INetSerializable) this.sharedLights, (INetSerializable) this.isFarm, (INetSerializable) this.isOutdoors, (INetSerializable) this.isStructure, (INetSerializable) this.ignoreDebrisWeather, (INetSerializable) this.ignoreOutdoorLighting, (INetSerializable) this.ignoreLights, (INetSerializable) this.treatAsOutdoors, (INetSerializable) this.warps, (INetSerializable) this.doors, (INetSerializable) this.interiorDoors, (INetSerializable) this.waterColor, (INetSerializable) this.netObjects, (INetSerializable) this.projectiles, (INetSerializable) this.largeTerrainFeatures, (INetSerializable) this.terrainFeatures, (INetSerializable) this.characters, (INetSerializable) this.debris, (INetSerializable) this.netAudio.NetFields, (INetSerializable) this.removeTemporarySpritesWithIDEvent, (INetSerializable) this.rumbleAndFadeEvent, (INetSerializable) this.damagePlayersEvent, (INetSerializable) this.lightGlows, (INetSerializable) this.fishSplashPoint, (INetSerializable) this.orePanPoint, (INetSerializable) this.isGreenhouse, (INetSerializable) this.miniJukeboxCount, (INetSerializable) this.miniJukeboxTrack, (INetSerializable) this.randomMiniJukeboxTrack, (INetSerializable) this.resourceClumps);
      this.NetFields.AddFields((INetSerializable) this.furniture, (INetSerializable) this.furnitureToRemove.NetFields);
      this.NetFields.AddField((INetSerializable) this.mapSeats);
      this.NetFields.AddField((INetSerializable) this.modData);
      this.sharedLights.OnValueAdded += (NetDictionary<int, LightSource, NetRef<LightSource>, SerializableDictionary<int, LightSource>, NetIntDictionary<LightSource, NetRef<LightSource>>>.ContentsChangeEvent) ((identifier, light) =>
      {
        if (Game1.currentLocation != this)
          return;
        Game1.currentLightSources.Add(light);
      });
      this.sharedLights.OnValueRemoved += (NetDictionary<int, LightSource, NetRef<LightSource>, SerializableDictionary<int, LightSource>, NetIntDictionary<LightSource, NetRef<LightSource>>>.ContentsChangeEvent) ((identifier, light) =>
      {
        if (Game1.currentLocation != this)
          return;
        Game1.currentLightSources.Remove(light);
      });
      this.netObjects.OnConflictResolve += (NetDictionary<Vector2, Object, NetRef<Object>, SerializableDictionary<Vector2, Object>, NetVector2Dictionary<Object, NetRef<Object>>>.ConflictResolveEvent) ((pos, rejected, accepted) =>
      {
        if (!Game1.IsMasterGame)
          return;
        Object @object = rejected.Value;
        if (@object == null)
          return;
        @object.NetFields.Parent = (INetSerializable) null;
        @object.dropItem(this, pos * 64f, pos * 64f);
      });
      this.removeTemporarySpritesWithIDEvent.onEvent += new AbstractNetEvent1<float>.Event(this.removeTemporarySpritesWithIDLocal);
      this.rumbleAndFadeEvent.onEvent += new AbstractNetEvent1<int>.Event(this.performRumbleAndFade);
      this.damagePlayersEvent.onEvent += new AbstractNetEvent1<GameLocation.DamagePlayersEventArg>.Event(this.performDamagePlayers);
      this.fishSplashPoint.fieldChangeVisibleEvent += (NetFieldBase<Point, NetPoint>.FieldChange) ((field, oldValue, newValue) => this.updateFishSplashAnimation());
      this.orePanPoint.fieldChangeVisibleEvent += (NetFieldBase<Point, NetPoint>.FieldChange) ((field, oldValue, newValue) => this.updateOrePanAnimation());
      this.characters.OnValueRemoved += (NetCollection<NPC>.ContentsChangeEvent) (npc => npc.Removed());
      this.terrainFeatures.OnValueAdded += (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.ContentsChangeEvent) ((pos, tf) =>
      {
        switch (tf)
        {
          case Flooring _:
            (tf as Flooring).OnAdded(this, pos);
            break;
          case HoeDirt _:
            (tf as HoeDirt).OnAdded(this, pos);
            break;
        }
        this.OnTerrainFeatureAdded(tf, pos);
      });
      this.terrainFeatures.OnValueRemoved += (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.ContentsChangeEvent) ((pos, tf) =>
      {
        switch (tf)
        {
          case Flooring _:
            (tf as Flooring).OnRemoved(this, pos);
            break;
          case HoeDirt _:
            (tf as HoeDirt).OnRemoved(this, pos);
            break;
        }
        this.OnTerrainFeatureRemoved(tf);
      });
      this.largeTerrainFeatures.OnValueAdded += (NetCollection<LargeTerrainFeature>.ContentsChangeEvent) (tf => this.OnTerrainFeatureAdded((TerrainFeature) tf, tf.currentTileLocation));
      this.largeTerrainFeatures.OnValueRemoved += (NetCollection<LargeTerrainFeature>.ContentsChangeEvent) (tf => this.OnTerrainFeatureRemoved((TerrainFeature) tf));
      this.furniture.InterpolationWait = false;
      this.furniture.OnValueAdded += (NetCollection<Furniture>.ContentsChangeEvent) (f => f.OnAdded(this, f.TileLocation));
      this.furnitureToRemove.Processor = new Action<Guid>(this.removeQueuedFurniture);
    }

    public virtual void InvalidateCachedMultiplayerMap(
      Dictionary<string, CachedMultiplayerMap> cached_data)
    {
      if (Game1.IsMasterGame || !cached_data.ContainsKey(this.NameOrUniqueName))
        return;
      cached_data.Remove(this.NameOrUniqueName);
    }

    public virtual void MakeMapModifications(bool force = false)
    {
      if (force)
        this._appliedMapOverrides.Clear();
      this.interiorDoors.MakeMapModifications();
      string name = (string) (NetFieldBase<string, NetString>) this.name;
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
      {
        case 636013712:
          if (!(name == "HaleyHouse") || !Game1.player.eventsSeen.Contains(463391) || Game1.player.spouse != null && Game1.player.spouse.Equals("Emily"))
            break;
          this.setMapTileIndex(14, 4, 2173, "Buildings");
          this.setMapTileIndex(14, 3, 2141, "Buildings");
          this.setMapTileIndex(14, 3, 219, "Back");
          break;
        case 1695005214:
          if (!(name == "Sunroom"))
            break;
          string imageSource = this.map.TileSheets[1].ImageSource;
          Path.GetFileName(imageSource);
          string path1 = Path.GetDirectoryName(imageSource);
          if (string.IsNullOrWhiteSpace(path1))
            path1 = "Maps";
          this.map.TileSheets[1].ImageSource = Path.Combine(path1, "CarolineGreenhouseTiles" + (Game1.IsRainingHere(this) || Game1.timeOfDay > Game1.getTrulyDarkTime() ? "_rainy" : ""));
          this.map.DisposeTileSheets(Game1.mapDisplayDevice);
          this.map.LoadTileSheets(Game1.mapDisplayDevice);
          break;
        case 2028543928:
          if (!(name == "Backwoods"))
            break;
          if (Game1.netWorldState.Value.hasWorldStateID("golemGrave"))
            this.ApplyMapOverride("Backwoods_GraveSite");
          if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts") || this._appliedMapOverrides.Contains("Backwoods_Staircase"))
            break;
          this.ApplyMapOverride("Backwoods_Staircase");
          LargeTerrainFeature largeTerrainFeature1 = (LargeTerrainFeature) null;
          foreach (LargeTerrainFeature largeTerrainFeature2 in this.largeTerrainFeatures)
          {
            if (largeTerrainFeature2.tilePosition.Equals((object) new Vector2(37f, 16f)))
            {
              largeTerrainFeature1 = largeTerrainFeature2;
              break;
            }
          }
          if (largeTerrainFeature1 == null)
            break;
          this.largeTerrainFeatures.Remove(largeTerrainFeature1);
          break;
        case 2680503661:
          if (!(name == "AbandonedJojaMart"))
            break;
          if (!Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
          {
            StaticTile[] junimoNoteTileFrames = CommunityCenter.getJunimoNoteTileFrames(0, this.map);
            string layerId = "Buildings";
            Point point = new Point(8, 8);
            this.map.GetLayer(layerId).Tiles[point.X, point.Y] = (Tile) new AnimatedTile(this.map.GetLayer(layerId), junimoNoteTileFrames, 70L);
            break;
          }
          this.removeTile(8, 8, "Buildings");
          break;
        case 2841403676:
          if (!(name == "WitchSwamp"))
            break;
          if (Game1.MasterPlayer.mailReceived.Contains("henchmanGone"))
          {
            this.removeTile(20, 29, "Buildings");
            break;
          }
          this.setMapTileIndex(20, 29, 10, "Buildings");
          break;
        case 2909376585:
          if (!(name == "Saloon") || !NetWorldState.checkAnywhereForWorldStateID("saloonSportsRoom"))
            break;
          this.refurbishMapPortion(new Microsoft.Xna.Framework.Rectangle(32, 1, 7, 9), "RefurbishedSaloonRoom", Point.Zero);
          Game1.currentLightSources.Add(new LightSource(1, new Vector2(33f, 7f) * 64f, 4f));
          Game1.currentLightSources.Add(new LightSource(1, new Vector2(36f, 7f) * 64f, 4f));
          Game1.currentLightSources.Add(new LightSource(1, new Vector2(34f, 5f) * 64f, 4f));
          break;
        case 3755589785:
          if (!(name == "WitchHut") || !Game1.player.mailReceived.Contains("hasPickedUpMagicInk"))
            break;
          this.setMapTileIndex(4, 11, 113, "Buildings");
          ((IDictionary<string, PropertyValue>) this.map.GetLayer("Buildings").Tiles[4, 11].Properties).Remove("Action");
          break;
      }
    }

    public virtual bool ApplyCachedMultiplayerMap(
      Dictionary<string, CachedMultiplayerMap> cached_data,
      string requested_map_path)
    {
      if (Game1.IsMasterGame || !cached_data.ContainsKey(this.NameOrUniqueName))
        return false;
      CachedMultiplayerMap cachedMultiplayerMap = cached_data[this.NameOrUniqueName];
      if (cachedMultiplayerMap.mapPath == requested_map_path)
      {
        this._appliedMapOverrides = cachedMultiplayerMap.appliedMapOverrides;
        this.map = cachedMultiplayerMap.map;
        this.loadedMapPath = cachedMultiplayerMap.loadedMapPath;
        return true;
      }
      cached_data.Remove(this.NameOrUniqueName);
      return false;
    }

    public virtual void StoreCachedMultiplayerMap(
      Dictionary<string, CachedMultiplayerMap> cached_data)
    {
      if (Game1.IsMasterGame)
        return;
      switch (this)
      {
        case VolcanoDungeon _:
          break;
        case MineShaft _:
          break;
        default:
          cached_data[this.NameOrUniqueName] = new CachedMultiplayerMap()
          {
            map = this.map,
            appliedMapOverrides = this._appliedMapOverrides,
            mapPath = (string) (NetFieldBase<string, NetString>) this.mapPath,
            loadedMapPath = this.loadedMapPath
          };
          break;
      }
    }

    public virtual void TransferDataFromSavedLocation(GameLocation l)
    {
      this.modData.Clear();
      if (l.modData != null)
      {
        foreach (string key in l.modData.Keys)
          this.modData[key] = l.modData[key];
      }
      this.SelectRandomMiniJukeboxTrack();
      this.UpdateMapSeats();
    }

    public virtual void OnTerrainFeatureAdded(TerrainFeature feature, Vector2 location)
    {
      if (feature == null)
        return;
      feature.currentLocation = this;
      feature.currentTileLocation = location;
      feature.OnAddedToLocation(this, location);
      this.UpdateTerrainFeatureUpdateSubscription(feature);
    }

    public virtual void OnTerrainFeatureRemoved(TerrainFeature feature)
    {
      if (feature == null)
        return;
      if (feature.NeedsUpdate)
        this._activeTerrainFeatures.Remove(feature);
      feature.currentLocation = (GameLocation) null;
    }

    public virtual void UpdateTerrainFeatureUpdateSubscription(TerrainFeature feature)
    {
      if (feature.NeedsUpdate)
        this._activeTerrainFeatures.Add(feature);
      else
        this._activeTerrainFeatures.Remove(feature);
    }

    public string GetSeasonForLocation()
    {
      if (this.seasonOverride == null)
      {
        if (this.map == null && this.mapPath.Value != null)
          this.reloadMap();
        if (this.map != null)
        {
          this.seasonOverride = (string) null;
          PropertyValue propertyValue = (PropertyValue) null;
          this.seasonOverride = !this.map.Properties.TryGetValue("SeasonOverride", out propertyValue) ? "" : propertyValue.ToString();
        }
      }
      return this.seasonOverride != null && this.seasonOverride.Length == 0 ? Game1.currentSeason : this.seasonOverride;
    }

    public bool isTemp() => this.Name.StartsWith("Temp") || this.Name.Equals("fishingGame") || this.Name.Equals("tent");

    private void updateFishSplashAnimation()
    {
      if (this.fishSplashPoint.Value == Point.Zero)
        this.fishSplashAnimation = (TemporaryAnimatedSprite) null;
      else
        this.fishSplashAnimation = new TemporaryAnimatedSprite(51, new Vector2((float) (this.fishSplashPoint.X * 64), (float) (this.fishSplashPoint.Y * 64)), Microsoft.Xna.Framework.Color.White, 10, animationInterval: 80f, numberOfLoops: 999999);
    }

    private void updateOrePanAnimation()
    {
      if (this.orePanPoint.Value == Point.Zero)
        this.orePanAnimation = (TemporaryAnimatedSprite) null;
      else
        this.orePanAnimation = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16, 16), new Vector2((float) (this.orePanPoint.X * 64 + 32), (float) (this.orePanPoint.Y * 64 + 32)), false, 0.0f, Microsoft.Xna.Framework.Color.White)
        {
          totalNumberOfLoops = 9999999,
          interval = 100f,
          scale = 3f,
          animationLength = 6
        };
    }

    public GameLocation()
    {
      this.farmers = new FarmerCollection(this);
      this.interiorDoors = new InteriorDoorDictionary(this);
      this.netAudio = new NetAudio(this);
      this.objects = new OverlaidDictionary(this.netObjects, this.overlayObjects);
      this._appliedMapOverrides = new HashSet<string>();
      this.terrainFeatures.SetEqualityComparer((IEqualityComparer<Vector2>) GameLocation.tilePositionComparer);
      this.netObjects.SetEqualityComparer((IEqualityComparer<Vector2>) GameLocation.tilePositionComparer);
      this.objects.SetEqualityComparer((IEqualityComparer<Vector2>) GameLocation.tilePositionComparer, ref this.netObjects, ref this.overlayObjects);
      this.initNetFields();
    }

    public GameLocation(string mapPath, string name)
      : this()
    {
      this.mapPath.Set(mapPath);
      this.name.Value = name;
      if (name.Contains("Farm") || name.Contains("Coop") || name.Contains("Barn") || name.Equals("SlimeHutch"))
        this.isFarm.Value = true;
      if (name == "Greenhouse")
        this.IsGreenhouse = true;
      this.reloadMap();
      this.loadObjects();
    }

    public void playSound(string audioName, NetAudio.SoundContext soundContext = NetAudio.SoundContext.Default) => this.netAudio.Play(audioName, soundContext);

    public void playSoundPitched(string audioName, int pitch, NetAudio.SoundContext soundContext = NetAudio.SoundContext.Default) => this.netAudio.PlayPitched(audioName, Vector2.Zero, pitch, soundContext);

    public void playSoundAt(string audioName, Vector2 position, NetAudio.SoundContext soundContext = NetAudio.SoundContext.Default) => this.netAudio.PlayAt(audioName, position, soundContext);

    public void localSound(string audioName) => this.netAudio.PlayLocal(audioName);

    public void localSoundAt(string audioName, Vector2 position) => this.netAudio.PlayLocalAt(audioName, position);

    private bool doorHasStateOpen(Point door)
    {
      bool flag;
      return this.interiorDoors.TryGetValue(door, out flag) & flag;
    }

    protected virtual LocalizedContentManager getMapLoader() => Game1.game1.xTileContent;

    public void ApplyMapOverride(
      Map override_map,
      string override_key,
      Microsoft.Xna.Framework.Rectangle? source_rect = null,
      Microsoft.Xna.Framework.Rectangle? dest_rect = null)
    {
      if (this._appliedMapOverrides.Contains(override_key))
        return;
      this._appliedMapOverrides.Add(override_key);
      this.updateSeasonalTileSheets(override_map);
      Dictionary<TileSheet, TileSheet> dictionary1 = new Dictionary<TileSheet, TileSheet>();
      foreach (TileSheet tileSheet1 in override_map.TileSheets)
      {
        TileSheet tileSheet2 = this.map.GetTileSheet(tileSheet1.Id);
        string str1 = "";
        string str2 = "";
        if (tileSheet2 != null)
          str1 = tileSheet2.ImageSource;
        if (str2 != null)
          str2 = tileSheet1.ImageSource;
        if (tileSheet2 == null || str2 != str1)
        {
          tileSheet2 = new TileSheet("zzzzz_" + override_key + "_" + tileSheet1.Id, this.map, tileSheet1.ImageSource, tileSheet1.SheetSize, tileSheet1.TileSize);
          for (int tileIndex = 0; tileIndex < tileSheet1.TileCount; ++tileIndex)
            tileSheet2.TileIndexProperties[tileIndex].CopyFrom(tileSheet1.TileIndexProperties[tileIndex]);
          this.map.AddTileSheet(tileSheet2);
        }
        else if (tileSheet2.TileCount < tileSheet1.TileCount)
        {
          int tileCount = tileSheet2.TileCount;
          tileSheet2.SheetWidth = tileSheet1.SheetWidth;
          tileSheet2.SheetHeight = tileSheet1.SheetHeight;
          for (int tileIndex = tileCount; tileIndex < tileSheet1.TileCount; ++tileIndex)
            tileSheet2.TileIndexProperties[tileIndex].CopyFrom(tileSheet1.TileIndexProperties[tileIndex]);
        }
        dictionary1[tileSheet1] = tileSheet2;
      }
      Dictionary<Layer, Layer> dictionary2 = new Dictionary<Layer, Layer>();
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < override_map.Layers.Count; ++index)
      {
        num1 = Math.Max(num1, override_map.Layers[index].LayerWidth);
        num2 = Math.Max(num2, override_map.Layers[index].LayerHeight);
      }
      if (!source_rect.HasValue)
        source_rect = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, num1, num2));
      int num3 = 0;
      int num4 = 0;
      for (int index = 0; index < this.map.Layers.Count; ++index)
      {
        num3 = Math.Max(num3, this.map.Layers[index].LayerWidth);
        num4 = Math.Max(num4, this.map.Layers[index].LayerHeight);
      }
      for (int index = 0; index < override_map.Layers.Count; ++index)
      {
        Layer layer = this.map.GetLayer(override_map.Layers[index].Id);
        if (layer == null)
        {
          layer = new Layer(override_map.Layers[index].Id, this.map, new Size(num3, num4), override_map.Layers[index].TileSize);
          this.map.AddLayer(layer);
        }
        dictionary2[override_map.Layers[index]] = layer;
      }
      if (!dest_rect.HasValue)
        dest_rect = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, num3, num4));
      int x1 = source_rect.Value.X;
      int y1 = source_rect.Value.Y;
      int x2 = dest_rect.Value.X;
      int y2 = dest_rect.Value.Y;
      for (int index1 = 0; index1 < source_rect.Value.Width; ++index1)
      {
        for (int index2 = 0; index2 < source_rect.Value.Height; ++index2)
        {
          Point point1 = new Point(x1 + index1, y1 + index2);
          Point point2 = new Point(x2 + index1, y2 + index2);
          bool flag = false;
          for (int index3 = 0; index3 < override_map.Layers.Count; ++index3)
          {
            Layer layer1 = override_map.Layers[index3];
            Layer layer2 = dictionary2[layer1];
            if (layer2 != null && point2.X < layer2.LayerWidth && point2.Y < layer2.LayerHeight && (flag || override_map.Layers[index3].Tiles[point1.X, point1.Y] != null))
            {
              flag = true;
              if (point1.X < layer1.LayerWidth && point1.Y < layer1.LayerHeight)
              {
                if (layer1.Tiles[point1.X, point1.Y] == null)
                {
                  layer2.Tiles[point2.X, point2.Y] = (Tile) null;
                }
                else
                {
                  Tile tile1 = layer1.Tiles[point1.X, point1.Y];
                  Tile tile2 = (Tile) null;
                  if (tile1 is StaticTile)
                    tile2 = (Tile) new StaticTile(layer2, dictionary1[tile1.TileSheet], tile1.BlendMode, tile1.TileIndex);
                  else if (tile1 is AnimatedTile)
                  {
                    AnimatedTile animatedTile = tile1 as AnimatedTile;
                    StaticTile[] tileFrames = new StaticTile[animatedTile.TileFrames.Length];
                    for (int index4 = 0; index4 < animatedTile.TileFrames.Length; ++index4)
                    {
                      StaticTile tileFrame = animatedTile.TileFrames[index4];
                      tileFrames[index4] = new StaticTile(layer2, dictionary1[tileFrame.TileSheet], tileFrame.BlendMode, tileFrame.TileIndex);
                    }
                    tile2 = (Tile) new AnimatedTile(layer2, tileFrames, animatedTile.FrameInterval);
                  }
                  tile2?.Properties.CopyFrom(tile1.Properties);
                  layer2.Tiles[point2.X, point2.Y] = tile2;
                }
              }
            }
          }
        }
      }
      this.map.LoadTileSheets(Game1.mapDisplayDevice);
      if (!Game1.IsMasterGame)
        return;
      this._mapSeatsDirty = true;
    }

    public virtual bool RunLocationSpecificEventCommand(
      Event current_event,
      string command_string,
      bool first_run,
      params string[] args)
    {
      return true;
    }

    public void ApplyMapOverride(
      string map_name,
      Microsoft.Xna.Framework.Rectangle? source_rect = null,
      Microsoft.Xna.Framework.Rectangle? destination_rect = null)
    {
      if (this._appliedMapOverrides.Contains(map_name))
        return;
      this.ApplyMapOverride(Game1.game1.xTileContent.Load<Map>("Maps\\" + map_name), map_name, source_rect, destination_rect);
    }

    public void ApplyMapOverride(
      string map_name,
      string override_key_name,
      Microsoft.Xna.Framework.Rectangle? source_rect = null,
      Microsoft.Xna.Framework.Rectangle? destination_rect = null)
    {
      if (this._appliedMapOverrides.Contains(override_key_name))
        return;
      this.ApplyMapOverride(Game1.game1.xTileContent.Load<Map>("Maps\\" + map_name), override_key_name, source_rect, destination_rect);
    }

    public virtual void UpdateMapSeats()
    {
      this._mapSeatsDirty = false;
      if (!Game1.IsMasterGame)
        return;
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      Dictionary<string, string> dictionary2 = Game1.content.Load<Dictionary<string, string>>("Data\\ChairTiles");
      this.mapSeats.Clear();
      Layer layer = this.map.GetLayer("Buildings");
      if (layer == null)
        return;
      for (int x = 0; x < layer.LayerWidth; ++x)
      {
        for (int y = 0; y < layer.LayerHeight; ++y)
        {
          Tile tile = layer.Tiles[x, y];
          if (tile != null)
          {
            string key1 = Path.GetFileNameWithoutExtension(tile.TileSheet.ImageSource);
            if (dictionary1.ContainsKey(key1))
            {
              key1 = dictionary1[key1];
            }
            else
            {
              if (key1.StartsWith("summer_") || key1.StartsWith("winter_") || key1.StartsWith("fall_"))
                key1 = "spring_" + key1.Substring(key1.IndexOf('_') + 1);
              dictionary1[key1] = key1;
            }
            int sheetWidth = tile.TileSheet.SheetWidth;
            int num1 = tile.TileIndex % sheetWidth;
            int num2 = tile.TileIndex / sheetWidth;
            string key2 = key1 + "/" + num1.ToString() + "/" + num2.ToString();
            if (dictionary2.ContainsKey(key2))
            {
              MapSeat mapSeat = MapSeat.FromData(dictionary2[key2], x, y);
              if (mapSeat != null)
                this.mapSeats.Add(mapSeat);
            }
          }
        }
      }
    }

    public virtual void OnMapLoad(Map map)
    {
    }

    public void loadMap(string mapPath, bool force_reload = false)
    {
      if (force_reload)
      {
        LocalizedContentManager contentManager = Program.gamePtr.CreateContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
        this.map = contentManager.Load<Map>(mapPath);
        contentManager.Unload();
        this.InvalidateCachedMultiplayerMap(Game1.multiplayer.cachedMultiplayerMaps);
      }
      else if (!this.ApplyCachedMultiplayerMap(Game1.multiplayer.cachedMultiplayerMaps, mapPath))
        this.map = this.getMapLoader().Load<Map>(mapPath);
      this.loadedMapPath = mapPath;
      this.OnMapLoad(this.map);
      this.updateSeasonalTileSheets(this.map);
      this.map.LoadTileSheets(Game1.mapDisplayDevice);
      if (Game1.IsMasterGame)
        this._mapSeatsDirty = true;
      PropertyValue propertyValue1;
      this.map.Properties.TryGetValue("Outdoors", out propertyValue1);
      if (propertyValue1 != null)
        this.isOutdoors.Value = true;
      PropertyValue propertyValue2;
      if (this.map.Properties.TryGetValue("IsFarm", out propertyValue2) && propertyValue2 != null)
        this.isFarm.Value = true;
      if (this.map.Properties.TryGetValue("IsGreenhouse", out propertyValue2) && propertyValue2 != null)
        this.isGreenhouse.Value = true;
      PropertyValue propertyValue3;
      this.map.Properties.TryGetValue("forceLoadPathLayerLights", out propertyValue3);
      if (propertyValue3 != null)
        this.forceLoadPathLayerLights = true;
      PropertyValue propertyValue4;
      this.map.Properties.TryGetValue("TreatAsOutdoors", out propertyValue4);
      if (propertyValue4 != null)
        this.treatAsOutdoors.Value = true;
      bool flag1 = false;
      PropertyValue propertyValue5;
      this.map.Properties.TryGetValue("indoorWater", out propertyValue5);
      if (propertyValue5 != null)
        flag1 = true;
      if (((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors | flag1 || this is Sewer || this is Submarine) && !(this is Desert))
      {
        this.waterTiles = (WaterTiles) new bool[this.map.Layers[0].LayerWidth, this.map.Layers[0].LayerHeight];
        bool flag2 = false;
        for (int index1 = 0; index1 < this.map.Layers[0].LayerWidth; ++index1)
        {
          for (int index2 = 0; index2 < this.map.Layers[0].LayerHeight; ++index2)
          {
            string str = this.doesTileHaveProperty(index1, index2, "Water", "Back");
            if (str != null)
            {
              flag2 = true;
              if (str == "I")
                this.waterTiles.waterTiles[index1, index2] = new WaterTiles.WaterTileData(true, false);
              else
                this.waterTiles[index1, index2] = true;
            }
          }
        }
        if (!flag2)
          this.waterTiles = (WaterTiles) null;
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors)
        this.critters = new List<Critter>();
      this.loadLights();
    }

    public virtual void HandleGrassGrowth(int dayOfMonth)
    {
      if (dayOfMonth == 1)
      {
        if (this is Farm || this.getMapProperty("ClearEmptyDirtOnNewMonth") != "")
        {
          for (int index = this.terrainFeatures.Count() - 1; index >= 0; --index)
          {
            if (this.terrainFeatures.Pairs.ElementAt(index).Value is HoeDirt && (this.terrainFeatures.Pairs.ElementAt(index).Value as HoeDirt).crop == null && Game1.random.NextDouble() < 0.8)
              this.terrainFeatures.Remove(this.terrainFeatures.Pairs.ElementAt(index).Key);
          }
        }
        if (this is Farm || this.getMapProperty("SpawnDebrisOnNewMonth") != "")
          this.spawnWeedsAndStones(20, spawnFromOldWeeds: false);
        if (Game1.currentSeason.Equals("spring") && Game1.stats.DaysPlayed > 1U)
        {
          if (this is Farm || this.getMapProperty("SpawnDebrisOnNewYear") != "")
          {
            this.spawnWeedsAndStones(40, spawnFromOldWeeds: false);
            this.spawnWeedsAndStones(40, true, false);
          }
          if (this is Farm || this.getMapProperty("SpawnRandomGrassOnNewYear") != "")
          {
            for (int index = 0; index < 15; ++index)
            {
              int num1 = Game1.random.Next(this.map.DisplayWidth / 64);
              int num2 = Game1.random.Next(this.map.DisplayHeight / 64);
              Vector2 vector2 = new Vector2((float) num1, (float) num2);
              Object @object;
              this.objects.TryGetValue(vector2, out @object);
              if (@object == null && this.doesTileHaveProperty(num1, num2, "Diggable", "Back") != null && this.doesTileHaveProperty(num1, num2, "NoSpawn", "Back") == null && this.isTileLocationOpen(new Location(num1, num2)) && !this.isTileOccupied(vector2) && this.doesTileHaveProperty(num1, num2, "Water", "Back") == null)
                this.terrainFeatures.Add(vector2, (TerrainFeature) new Grass(1, 4));
            }
            this.growWeedGrass(40);
          }
          if (this.getMapProperty("SpawnGrassFromPathsOnNewYear") != null)
          {
            Layer layer = this.map.GetLayer("Paths");
            if (layer != null)
            {
              for (int x = 0; x < layer.LayerWidth; ++x)
              {
                for (int y = 0; y < layer.LayerHeight; ++y)
                {
                  Vector2 vector2 = new Vector2((float) x, (float) y);
                  Object @object;
                  this.objects.TryGetValue(vector2, out @object);
                  if (@object == null && this.getTileIndexAt(new Point(x, y), "Paths") == 22 && this.isTileLocationOpen(new Location(x, y)) && !this.isTileOccupied(vector2))
                    this.terrainFeatures.Add(vector2, (TerrainFeature) new Grass(1, 4));
                }
              }
            }
          }
        }
      }
      if (!(this is Farm) && !(this.getMapProperty("EnableGrassSpread") != "") || !(Game1.currentSeason != "winter") && !(this.getMapProperty("AllowGrassGrowInWinter") != ""))
        return;
      this.growWeedGrass(1);
    }

    public void reloadMap()
    {
      if ((NetFieldBase<string, NetString>) this.mapPath != (NetString) null)
        this.loadMap((string) (NetFieldBase<string, NetString>) this.mapPath);
      else
        this.map = (Map) null;
      this.loadedMapPath = (string) (NetFieldBase<string, NetString>) this.mapPath;
    }

    public virtual bool canSlimeMateHere() => true;

    public virtual bool canSlimeHatchHere() => true;

    public void addCharacter(NPC character) => this.characters.Add(character);

    public NetCollection<NPC> getCharacters() => this.characters;

    public static Microsoft.Xna.Framework.Rectangle getSourceRectForObject(int tileIndex) => new Microsoft.Xna.Framework.Rectangle(tileIndex * 16 % Game1.objectSpriteSheet.Width, tileIndex * 16 / Game1.objectSpriteSheet.Width * 16, 16, 16);

    public Warp isCollidingWithWarp(Microsoft.Xna.Framework.Rectangle position, Character character)
    {
      if (this.ignoreWarps)
        return (Warp) null;
      foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) this.warps)
      {
        if ((character is NPC || !warp.npcOnly.Value) && (warp.X == (int) Math.Floor((double) position.Left / 64.0) || warp.X == (int) Math.Floor((double) position.Right / 64.0)) && (warp.Y == (int) Math.Floor((double) position.Top / 64.0) || warp.Y == (int) Math.Floor((double) position.Bottom / 64.0)))
        {
          if (warp.TargetName == "BoatTunnel" && character is NPC)
            return new Warp(warp.X, warp.Y, "IslandSouth", 17, 43, false);
          return warp.TargetName == "VolcanoEntrance" ? new Warp(warp.X, warp.Y, "VolcanoDungeon0", warp.TargetX, warp.TargetY, false) : warp;
        }
      }
      return (Warp) null;
    }

    public Warp isCollidingWithWarpOrDoor(Microsoft.Xna.Framework.Rectangle position, Character character = null) => this.isCollidingWithWarp(position, character) ?? this.isCollidingWithDoors(position, character);

    public virtual Warp isCollidingWithDoors(Microsoft.Xna.Framework.Rectangle position, Character character = null)
    {
      for (int corner = 0; corner < 4; ++corner)
      {
        Vector2 cornersOfThisRectangle = Utility.getCornersOfThisRectangle(ref position, corner);
        Point point = new Point((int) cornersOfThisRectangle.X / 64, (int) cornersOfThisRectangle.Y / 64);
        foreach (KeyValuePair<Point, string> pair in this.doors.Pairs)
        {
          Point key = pair.Key;
          if (point.Equals(key))
            return this.getWarpFromDoor(key, character);
        }
      }
      return (Warp) null;
    }

    public virtual Warp getWarpFromDoor(Point door, Character character = null)
    {
      string[] strArray = this.doesTileHaveProperty(door.X, door.Y, "Action", "Buildings").Split(' ');
      if (strArray[0].Equals("WarpCommunityCenter"))
        return new Warp(door.X, door.Y, "CommunityCenter", 32, 23, false);
      if (strArray[0].Equals("Warp_Sunroom_Door"))
        return new Warp(door.X, door.Y, "Sunroom", 5, 13, false);
      return (strArray[0].Equals("WarpBoatTunnel") || strArray.Length > 3 && strArray[3].Equals("BoatTunnel")) && character is NPC ? new Warp(door.X, door.Y, "IslandSouth", 17, 43, false) : new Warp(door.X, door.Y, strArray[3], Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), false);
    }

    public void addResourceClumpAndRemoveUnderlyingTerrain(
      int resourceClumpIndex,
      int width,
      int height,
      Vector2 tile)
    {
      for (int index1 = 0; index1 < width; ++index1)
      {
        for (int index2 = 0; index2 < height; ++index2)
          this.removeEverythingExceptCharactersFromThisTile((int) tile.X + index1, (int) tile.Y + index2);
      }
      this.resourceClumps.Add(new ResourceClump(resourceClumpIndex, width, height, tile));
    }

    public virtual bool canFishHere() => true;

    public virtual bool CanRefillWateringCanOnTile(int tileX, int tileY)
    {
      if (this.doesTileHaveProperty(tileX, tileY, "Water", "Back") != null || this.doesTileHaveProperty(tileX, tileY, "WaterSource", "Back") != null)
        return true;
      if ((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors || !this.doesTileHavePropertyNoNull(tileX, tileY, "Action", "Buildings").Equals("kitchen"))
        return false;
      return this.getTileIndexAt(tileX, tileY, "Buildings") == 172 || this.getTileIndexAt(tileX, tileY, "Buildings") == 257;
    }

    public virtual bool isTileBuildingFishable(int tileX, int tileY) => false;

    public virtual bool isTileFishable(int tileX, int tileY) => this.isTileBuildingFishable(tileX, tileY) || this.doesTileHaveProperty(tileX, tileY, "Water", "Back") != null && this.doesTileHaveProperty(tileX, tileY, "NoFishing", "Back") == null && this.getTileIndexAt(tileX, tileY, "Buildings") == -1 || this.doesTileHaveProperty(tileX, tileY, "Water", "Buildings") != null;

    public bool isFarmerCollidingWithAnyCharacter()
    {
      foreach (NPC character in this.characters)
      {
        if (character != null && Game1.player.GetBoundingBox().Intersects(character.GetBoundingBox()))
          return true;
      }
      return false;
    }

    public bool isCollidingPosition(Microsoft.Xna.Framework.Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer) => this.isCollidingPosition(position, viewport, isFarmer, 0, false, (Character) null, false);

    public bool isCollidingPosition(Microsoft.Xna.Framework.Rectangle position, xTile.Dimensions.Rectangle viewport, Character character) => this.isCollidingPosition(position, viewport, false, 0, false, character, false);

    public bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider)
    {
      return this.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, (Character) null, false);
    }

    protected bool _TestCornersWorld(
      int top,
      int bottom,
      int left,
      int right,
      Func<int, int, bool> action)
    {
      return action(right, top) || action(right, bottom) || action(left, top) || action(left, bottom);
    }

    protected bool _TestCornersWorld(
      int top,
      int bottom,
      int left,
      int right,
      int center,
      bool bigger_than_tile,
      Func<int, int, bool> action)
    {
      return this._TestCornersWorld(top, bottom, left, right, action) || bigger_than_tile && (action(center, top) || action(center, bottom));
    }

    protected bool _TestCornersTiles(
      Vector2 top_right,
      Vector2 top_left,
      Vector2 bottom_right,
      Vector2 bottom_left,
      Vector2 top_mid,
      Vector2 bottom_mid,
      Vector2 player_top_right,
      Vector2 player_top_left,
      Vector2 player_bottom_right,
      Vector2 player_bottom_left,
      Vector2 player_top_mid,
      Vector2 player_bottom_mid,
      bool bigger_than_tile,
      Func<Vector2, Vector2, bool> action)
    {
      return action(top_right, player_top_right) || action(top_left, player_top_left) || action(bottom_left, player_bottom_left) || action(bottom_right, player_bottom_right) || bigger_than_tile && (action(top_mid, player_top_mid) || action(bottom_mid, player_bottom_mid));
    }

    public Furniture GetFurnitureAt(Vector2 tile_position)
    {
      Point point = new Point();
      point.X = (int) ((double) (int) tile_position.X + 0.5) * 64;
      point.Y = (int) ((double) (int) tile_position.Y + 0.5) * 64;
      foreach (Furniture furnitureAt in this.furniture)
      {
        if (furnitureAt.getBoundingBox(furnitureAt.TileLocation).Contains(point))
          return furnitureAt;
      }
      return (Furniture) null;
    }

    public virtual bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character)
    {
      if (!Game1.eventUp && (character == null || character.willDestroyObjectsUnderfoot))
      {
        foreach (Furniture furniture in this.furniture)
        {
          if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type != 12 && furniture.IntersectsForCollision(position) && (!isFarmer || !(character as Farmer).TemporaryPassableTiles.Intersects(position)))
            return true;
        }
      }
      return this.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, false);
    }

    public virtual bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character,
      bool pathfinding,
      bool projectile = false,
      bool ignoreCharacterRequirement = false)
    {
      foreach (ResourceClump resourceClump in this.resourceClumps)
      {
        if (!glider && resourceClump.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) resourceClump.tile).Intersects(position))
          return true;
      }
      bool flag = Game1.eventUp;
      if (flag && Game1.CurrentEvent != null && !Game1.CurrentEvent.ignoreObjectCollisions)
        flag = false;
      this.updateMap();
      if (position.Right < 0 || position.X > this.map.Layers[0].DisplayWidth || position.Bottom < 0 || position.Top > this.map.Layers[0].DisplayHeight)
        return false;
      if (character == null && !ignoreCharacterRequirement)
        return true;
      Vector2 vector2_1 = new Vector2((float) (position.Right / 64), (float) (position.Top / 64));
      Vector2 vector2_2 = new Vector2((float) (position.Left / 64), (float) (position.Top / 64));
      Vector2 vector2_3 = new Vector2((float) (position.Right / 64), (float) (position.Bottom / 64));
      Vector2 vector2_4 = new Vector2((float) (position.Left / 64), (float) (position.Bottom / 64));
      bool bigger_than_tile = position.Width > 64;
      Vector2 bottom_mid = new Vector2((float) (position.Center.X / 64), (float) (position.Bottom / 64));
      Vector2 top_mid = new Vector2((float) (position.Center.X / 64), (float) (position.Top / 64));
      Microsoft.Xna.Framework.Rectangle boundingBox1 = Game1.player.GetBoundingBox();
      Vector2 player_top_right = new Vector2((float) ((boundingBox1.Right - 1) / 64), (float) (boundingBox1.Top / 64));
      Vector2 player_top_left = new Vector2((float) (boundingBox1.Left / 64), (float) (boundingBox1.Top / 64));
      Vector2 player_bottom_right = new Vector2((float) ((boundingBox1.Right - 1) / 64), (float) ((boundingBox1.Bottom - 1) / 64));
      Vector2 player_bottom_left = new Vector2((float) (boundingBox1.Left / 64), (float) ((boundingBox1.Bottom - 1) / 64));
      Vector2 player_bottom_mid = new Vector2((float) (boundingBox1.Center.X / 64), (float) ((boundingBox1.Bottom - 1) / 64));
      Vector2 player_top_mid = new Vector2((float) (boundingBox1.Center.X / 64), (float) (boundingBox1.Top / 64));
      if (character != null & isFarmer && character is Farmer)
      {
        Farmer farmer = character as Farmer;
        if (farmer.bridge != null && farmer.onBridge.Value && position.Right >= farmer.bridge.bridgeBounds.X && position.Left <= farmer.bridge.bridgeBounds.Right)
          return this._TestCornersWorld(position.Top, position.Bottom, position.Left, position.Right, (Func<int, int, bool>) ((x, y) => y > farmer.bridge.bridgeBounds.Bottom || y < farmer.bridge.bridgeBounds.Top));
      }
      if (!glider && (!flag || character != null && !isFarmer && (!pathfinding || !character.willDestroyObjectsUnderfoot)) && this._TestCornersTiles(vector2_1, vector2_2, vector2_3, vector2_4, top_mid, bottom_mid, player_top_right, player_top_left, player_bottom_right, player_bottom_left, player_top_mid, player_bottom_mid, bigger_than_tile, (Func<Vector2, Vector2, bool>) ((corner, player_corner) =>
      {
        Object o;
        this.objects.TryGetValue(corner, out o);
        return o != null && !o.IsHoeDirt && !o.isPassable() && !Game1.player.TemporaryPassableTiles.Intersects(o.getBoundingBox(corner)) && o.getBoundingBox(corner).Intersects(position) && character != null && (!(character is FarmAnimal) || !o.isAnimalProduct()) && character.collideWith(o) && (!isFarmer || corner != player_corner);
      })))
        return true;
      if (!glider && !flag)
      {
        foreach (Furniture furniture in this.furniture)
        {
          if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type != 12 && furniture.IntersectsForCollision(position) && (!isFarmer || !furniture.IntersectsForCollision(Game1.player.GetBoundingBox())))
            return true;
        }
      }
      if (this.largeTerrainFeatures != null && !glider)
      {
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
        {
          if (largeTerrainFeature.getBoundingBox().Intersects(position))
            return true;
        }
      }
      if (!flag && !glider && this._TestCornersTiles(vector2_1, vector2_2, vector2_3, vector2_4, top_mid, bottom_mid, player_top_right, player_top_left, player_bottom_right, player_bottom_left, player_top_mid, player_bottom_mid, bigger_than_tile, (Func<Vector2, Vector2, bool>) ((corner, player_corner) =>
      {
        if (this.terrainFeatures.ContainsKey(corner) && this.terrainFeatures[corner].getBoundingBox(corner).Intersects(position))
        {
          if (!pathfinding)
            this.terrainFeatures[corner].doCollisionAction(position, Game1.player.speed + Game1.player.addedSpeed, corner, character, this);
          if (this.terrainFeatures.ContainsKey(corner) && !this.terrainFeatures[corner].isPassable(character) && (!isFarmer || corner != player_corner))
            return true;
        }
        return false;
      })) || character != null && character.hasSpecialCollisionRules() && (character.isColliding(this, vector2_1) || character.isColliding(this, vector2_2) || character.isColliding(this, vector2_3) || character.isColliding(this, vector2_4)))
        return true;
      if ((isFarmer && (this.currentEvent == null || this.currentEvent.playerControlSequence) || character != null && (bool) (NetFieldBase<bool, NetBool>) character.collidesWithOtherCharacters) && !pathfinding)
      {
        for (int index = this.characters.Count - 1; index >= 0; --index)
        {
          NPC character1 = this.characters[index];
          if (character1 != null && (character == null || !character.Equals((object) character1)))
          {
            Microsoft.Xna.Framework.Rectangle boundingBox2 = character1.GetBoundingBox();
            if (character1.layingDown)
            {
              boundingBox2.Y -= 64;
              boundingBox2.Height += 64;
            }
            if (boundingBox2.Intersects(position) && !Game1.player.temporarilyInvincible)
              character1.behaviorOnFarmerPushing();
            if (isFarmer && !flag && !character1.farmerPassesThrough && boundingBox2.Intersects(position) && !Game1.player.temporarilyInvincible && Game1.player.TemporaryPassableTiles.IsEmpty() && (!character1.IsMonster || !(bool) (NetFieldBase<bool, NetBool>) ((Monster) character1).isGlider && !Game1.player.GetBoundingBox().Intersects(character1.GetBoundingBox())) && !character1.IsInvisible && !Game1.player.GetBoundingBox().Intersects(boundingBox2) || !isFarmer && boundingBox2.Intersects(position))
              return true;
          }
        }
      }
      Layer back_layer = this.map.GetLayer("Back");
      Layer buildings_layer = this.map.GetLayer("Buildings");
      if (isFarmer)
      {
        if (this.currentEvent != null && this.currentEvent.checkForCollision(position, character != null ? character as Farmer : Game1.player) || Game1.player.currentUpgrade != null && Game1.player.currentUpgrade.daysLeftTillUpgradeDone <= 3 && this.name.Equals((object) "Farm") && position.Intersects(new Microsoft.Xna.Framework.Rectangle((int) Game1.player.currentUpgrade.positionOfCarpenter.X, (int) Game1.player.currentUpgrade.positionOfCarpenter.Y + 64, 64, 32)))
          return true;
      }
      else
      {
        if (!pathfinding && !(character is Monster) && damagesFarmer == 0 && !glider)
        {
          foreach (Farmer farmer in this.farmers)
          {
            if (position.Intersects(farmer.GetBoundingBox()))
              return true;
          }
        }
        if (((bool) (NetFieldBase<bool, NetBool>) this.isFarm || this.name.Value.StartsWith("UndergroundMine") || this is IslandLocation) && character != null && !character.Name.Contains("NPC") && !character.eventActor && !glider)
        {
          PropertyValue barrier = (PropertyValue) null;
          Tile t;
          if (this._TestCornersWorld(position.Top, position.Bottom, position.Left, position.Right, (Func<int, int, bool>) ((x, y) =>
          {
            t = back_layer.PickTile(new Location(x, y), viewport.Size);
            if (t != null)
            {
              t.Properties.TryGetValue("NPCBarrier", out barrier);
              if (barrier != null)
                return true;
            }
            return false;
          })))
            return true;
        }
        if (glider && !projectile)
          return false;
      }
      if (!isFarmer || !Game1.player.isRafting)
      {
        PropertyValue barrier = (PropertyValue) null;
        Tile t;
        if (this._TestCornersWorld(position.Top, position.Bottom, position.Left, position.Right, (Func<int, int, bool>) ((x, y) =>
        {
          t = back_layer.PickTile(new Location(x, y), viewport.Size);
          if (t != null)
          {
            t.Properties.TryGetValue("TemporaryBarrier", out barrier);
            if (barrier != null)
              return true;
          }
          return false;
        })))
          return true;
      }
      if (!isFarmer || !Game1.player.isRafting)
      {
        PropertyValue passable = (PropertyValue) null;
        Tile tmp;
        if ((!(character is FarmAnimal) || !(character as FarmAnimal).IsActuallySwimming()) && this._TestCornersWorld(position.Top, position.Bottom, position.Left, position.Right, position.Center.X, bigger_than_tile, (Func<int, int, bool>) ((x, y) =>
        {
          Tile tile = back_layer.PickTile(new Location(x, y), viewport.Size);
          if (tile != null)
          {
            tile.TileIndexProperties.TryGetValue("Passable", out passable);
            if (passable == null)
              tile.Properties.TryGetValue("Passable", out passable);
            if (passable != null && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(x, y)))
              return true;
          }
          return false;
        })) || this._TestCornersWorld(position.Top, position.Bottom, position.Left, position.Right, position.Center.X, bigger_than_tile, (Func<int, int, bool>) ((x, y) =>
        {
          tmp = buildings_layer.PickTile(new Location(x, y), viewport.Size);
          if (tmp != null)
          {
            if (projectile && this is VolcanoDungeon)
            {
              Tile tile = back_layer.PickTile(new Location(x, y), viewport.Size);
              if (tile != null)
              {
                PropertyValue propertyValue = (PropertyValue) null;
                if (propertyValue == null)
                  tile.TileIndexProperties.TryGetValue("Water", out propertyValue);
                if (propertyValue == null)
                  tile.Properties.TryGetValue("Water", out propertyValue);
                if (propertyValue != null)
                  return false;
              }
            }
            tmp.TileIndexProperties.TryGetValue("Shadow", out passable);
            if (passable == null)
              tmp.TileIndexProperties.TryGetValue("Passable", out passable);
            if (passable == null)
              tmp.Properties.TryGetValue("Passable", out passable);
            if (projectile)
            {
              if (passable == null)
                tmp.TileIndexProperties.TryGetValue("ProjectilePassable", out passable);
              if (passable == null)
                tmp.Properties.TryGetValue("ProjectilePassable", out passable);
            }
            if (passable == null && !isFarmer)
              tmp.TileIndexProperties.TryGetValue("NPCPassable", out passable);
            if (passable == null && !isFarmer)
              tmp.Properties.TryGetValue("NPCPassable", out passable);
            if (passable == null && !isFarmer && character != null && character.canPassThroughActionTiles())
              tmp.Properties.TryGetValue("Action", out passable);
            if ((passable == null || passable.ToString().Length == 0) && (!isFarmer || !Game1.player.TemporaryPassableTiles.Contains(x, y)))
              return character == null || character.shouldCollideWithBuildingLayer(this);
          }
          return false;
        })))
          return true;
        if (!isFarmer && passable != null && (passable.ToString().StartsWith("Door ") || passable.ToString() == "Door"))
        {
          this.openDoor(new Location(position.Center.X / 64, position.Bottom / 64), false);
          this.openDoor(new Location(position.Center.X / 64, position.Top / 64), Game1.currentLocation.Equals(this));
        }
        return false;
      }
      PropertyValue passable1 = (PropertyValue) null;
      Tile t1;
      return this._TestCornersWorld(position.Top, position.Bottom, position.Left, position.Right, (Func<int, int, bool>) ((x, y) =>
      {
        t1 = back_layer.PickTile(new Location(x, y), viewport.Size);
        t1?.TileIndexProperties.TryGetValue("Water", out passable1);
        if (passable1 != null)
          return false;
        if (this.isTileLocationOpen(new Location(x / 64, y / 64)) && !this.isTileOccupiedForPlacement(new Vector2((float) (x / 64), (float) (y / 64))))
        {
          Game1.player.isRafting = false;
          Game1.player.Position = new Vector2((float) (x / 64 * 64), (float) (y / 64 * 64 - 32));
          Game1.player.setTrajectory(0, 0);
        }
        return true;
      }));
    }

    public bool isTilePassable(Location tileLocation, xTile.Dimensions.Rectangle viewport)
    {
      PropertyValue propertyValue = (PropertyValue) null;
      Tile tile1 = this.map.GetLayer("Back").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
      tile1?.TileIndexProperties.TryGetValue("Passable", out propertyValue);
      Tile tile2 = this.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
      return propertyValue == null && tile2 == null && tile1 != null;
    }

    public bool isPointPassable(Location location, xTile.Dimensions.Rectangle viewport)
    {
      PropertyValue propertyValue1 = (PropertyValue) null;
      PropertyValue propertyValue2 = (PropertyValue) null;
      this.map.GetLayer("Back").PickTile(new Location(location.X, location.Y), viewport.Size)?.TileIndexProperties.TryGetValue("Passable", out propertyValue1);
      Tile tile = this.map.GetLayer("Buildings").PickTile(new Location(location.X, location.Y), viewport.Size);
      tile?.TileIndexProperties.TryGetValue("Shadow", out propertyValue2);
      if (propertyValue1 != null)
        return false;
      return tile == null || propertyValue2 != null;
    }

    public bool isTilePassable(Microsoft.Xna.Framework.Rectangle nextPosition, xTile.Dimensions.Rectangle viewport) => this.isPointPassable(new Location(nextPosition.Left, nextPosition.Top), viewport) && this.isPointPassable(new Location(nextPosition.Left, nextPosition.Bottom), viewport) && this.isPointPassable(new Location(nextPosition.Right, nextPosition.Top), viewport) && this.isPointPassable(new Location(nextPosition.Right, nextPosition.Bottom), viewport);

    public bool isTileOnMap(Vector2 position) => (double) position.X >= 0.0 && (double) position.X < (double) this.map.Layers[0].LayerWidth && (double) position.Y >= 0.0 && (double) position.Y < (double) this.map.Layers[0].LayerHeight;

    public bool isTileOnMap(int x, int y) => x >= 0 && x < this.map.Layers[0].LayerWidth && y >= 0 && y < this.map.Layers[0].LayerHeight;

    public void busLeave()
    {
      NPC npc = (NPC) null;
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        if (this.characters[index].Name.Equals("Pam"))
        {
          npc = this.characters[index];
          this.characters.RemoveAt(index);
          break;
        }
      }
      if (npc == null)
        return;
      Game1.changeMusicTrack("none");
      this.localSound("openBox");
      if (this.name.Equals((object) "BusStop"))
      {
        Game1.warpFarmer("Desert", 32, 27, true);
        npc.followSchedule = false;
        npc.Position = new Vector2(1984f, 1752f);
        npc.faceDirection(2);
        npc.CurrentDialogue.Peek().temporaryDialogue = Game1.parseText(Game1.content.LoadString("Strings\\Locations:Desert_BusArrived"));
        Game1.getLocationFromName("Desert").characters.Add(npc);
      }
      else
      {
        npc.CurrentDialogue.Peek().temporaryDialogue = (string) null;
        Game1.warpFarmer("BusStop", 9, 9, true);
        if (Game1.timeOfDay >= 2300)
        {
          npc.Position = new Vector2(1152f, 408f);
          npc.faceDirection(2);
          Game1.getLocationFromName("Trailer").characters.Add(npc);
        }
        else if (Game1.timeOfDay >= 1700)
        {
          npc.Position = new Vector2(448f, 1112f);
          npc.faceDirection(1);
          Game1.getLocationFromName("Saloon").characters.Add(npc);
        }
        else
        {
          npc.Position = new Vector2(512f, 600f);
          npc.faceDirection(2);
          Game1.getLocationFromName("BusStop").characters.Add(npc);
          npc.Sprite.currentFrame = 0;
        }
        npc.DirectionsToNewLocation = (SchedulePathDescription) null;
        npc.followSchedule = true;
      }
    }

    public int numberOfObjectsWithName(string name)
    {
      int num = 0;
      foreach (Item obj in this.objects.Values)
      {
        if (obj.Name.Equals(name))
          ++num;
      }
      return num;
    }

    public virtual Point getWarpPointTo(string location, Character character = null)
    {
      foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) this.warps)
      {
        if (warp.TargetName.Equals(location))
          return new Point(warp.X, warp.Y);
        if (warp.TargetName.Equals("BoatTunnel") && location == "IslandSouth")
          return new Point(warp.X, warp.Y);
      }
      foreach (KeyValuePair<Point, string> pair in this.doors.Pairs)
      {
        if (pair.Value.Equals("BoatTunnel") && location == "IslandSouth" || pair.Value.Equals(location))
          return pair.Key;
      }
      return Point.Zero;
    }

    public Point getWarpPointTarget(Point warpPointLocation, Character character = null)
    {
      foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) this.warps)
      {
        if (warp.X == warpPointLocation.X && warp.Y == warpPointLocation.Y)
          return new Point(warp.TargetX, warp.TargetY);
      }
      foreach (KeyValuePair<Point, string> pair in this.doors.Pairs)
      {
        if (pair.Key.Equals(warpPointLocation))
        {
          string str = this.doesTileHaveProperty(warpPointLocation.X, warpPointLocation.Y, "Action", "Buildings");
          if (str != null && str.Contains("Warp"))
          {
            string[] strArray = str.Split(' ');
            if (strArray[0].Equals("WarpCommunityCenter"))
              return new Point(32, 23);
            if (strArray[0].Equals("Warp_Sunroom_Door"))
              return new Point(5, 13);
            if (character is NPC && (strArray[0].Equals("WarpBoatTunnel") || strArray.Length > 3 && strArray[3].Equals("BoatTunnel")))
              return new Point(17, 43);
            return strArray[3].Equals("Trailer") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade") ? new Point(13, 24) : new Point(Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]));
          }
        }
      }
      return Point.Zero;
    }

    public virtual bool HasLocationOverrideDialogue(NPC character) => false;

    public virtual string GetLocationOverrideDialogue(NPC character) => !this.HasLocationOverrideDialogue(character) ? (string) null : "";

    public void boardBus(Vector2 playerTileLocation)
    {
      if (!Game1.player.hasBusTicket && !this.name.Equals((object) "Desert"))
        return;
      bool flag = false;
      int num = this.characters.Count - 1;
      while (num >= 0)
        --num;
      if (flag)
      {
        Game1.player.hasBusTicket = false;
        Game1.player.CanMove = false;
        Game1.viewportFreeze = true;
        Game1.player.position.X = -99999f;
        Game1.boardingBus = true;
      }
      else
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Bus_NoDriver"));
    }

    public NPC doesPositionCollideWithCharacter(float x, float y)
    {
      foreach (NPC character in this.characters)
      {
        if (character.GetBoundingBox().Contains((int) x, (int) y))
          return character;
      }
      return (NPC) null;
    }

    public NPC doesPositionCollideWithCharacter(Microsoft.Xna.Framework.Rectangle r, bool ignoreMonsters = false)
    {
      foreach (NPC character in this.characters)
      {
        if (character.GetBoundingBox().Intersects(r) && (!character.IsMonster || !ignoreMonsters))
          return character;
      }
      return (NPC) null;
    }

    public void switchOutNightTiles()
    {
      try
      {
        PropertyValue propertyValue;
        this.map.Properties.TryGetValue("NightTiles", out propertyValue);
        if (propertyValue != null)
        {
          string[] strArray = propertyValue.ToString().Split(' ');
          for (int index = 0; index < strArray.Length; index += 4)
          {
            if (!strArray[index + 3].Equals("726") && !strArray[index + 3].Equals("720") || !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
              this.map.GetLayer(strArray[index]).Tiles[Convert.ToInt32(strArray[index + 1]), Convert.ToInt32(strArray[index + 2])].TileIndex = Convert.ToInt32(strArray[index + 3]);
          }
        }
      }
      catch (Exception ex)
      {
      }
      switch (this)
      {
        case MineShaft _:
          break;
        case Woods _:
          break;
        default:
          this.lightGlows.Clear();
          break;
      }
    }

    public virtual void checkForMusic(GameTime time)
    {
      if (!Game1.startedJukeboxMusic && Game1.getMusicTrackName() == "IslandMusic" && Game1.currentLocation != null && Game1.currentLocation.GetLocationContext() != GameLocation.LocationContext.Island)
        Game1.changeMusicTrack("none", true);
      if (Game1.getMusicTrackName() == "rain" && !Game1.IsRainingHere())
        Game1.changeMusicTrack("none", true);
      if (Utility.IsDesertLocation(this))
        return;
      if (Game1.getMusicTrackName() == "sam_acoustic1" && Game1.isMusicContextActiveButNotPlaying())
        Game1.changeMusicTrack("none", true);
      if (!(this is MineShaft) && Game1.getMusicTrackName().Contains("Ambient") && !Game1.getMusicTrackName().Contains(Game1.currentSeason) && this.getMapProperty("Music") != Game1.getMusicTrackName())
        Game1.changeMusicTrack("none", true);
      if (this.IsOutdoors && Game1.isMusicContextActiveButNotPlaying() && !Game1.IsRainingHere(this) && !Game1.eventUp)
      {
        if (!Game1.isDarkOut())
        {
          string currentSeason = Game1.currentSeason;
          if (!(currentSeason == "spring"))
          {
            if (!(currentSeason == "summer"))
            {
              if (!(currentSeason == "fall"))
              {
                if (currentSeason == "winter")
                  Game1.changeMusicTrack("winter_day_ambient", true);
              }
              else
                Game1.changeMusicTrack("fall_day_ambient", true);
            }
            else
              Game1.changeMusicTrack("summer_day_ambient", true);
          }
          else
            Game1.changeMusicTrack("spring_day_ambient", true);
        }
        else if (Game1.isDarkOut() && Game1.timeOfDay < 2500)
        {
          string currentSeason = Game1.currentSeason;
          if (!(currentSeason == "spring"))
          {
            if (!(currentSeason == "summer"))
            {
              if (!(currentSeason == "fall"))
              {
                if (currentSeason == "winter")
                  Game1.changeMusicTrack("none", true);
              }
              else
                Game1.changeMusicTrack("spring_night_ambient", true);
            }
            else
              Game1.changeMusicTrack("spring_night_ambient", true);
          }
          else
            Game1.changeMusicTrack("spring_night_ambient", true);
        }
      }
      else if (Game1.isMusicContextActiveButNotPlaying() && Game1.IsRainingHere(this) && !Game1.showingEndOfNightStuff)
        Game1.changeMusicTrack("rain", true);
      if (!Game1.isRaining && (!Game1.currentSeason.Equals("fall") || !Game1.isDebrisWeather) && !Game1.currentSeason.Equals("winter") && !Game1.eventUp && Game1.timeOfDay < 1800 && this.name.Equals((object) "Town") && (Game1.isMusicContextActiveButNotPlaying() || Game1.getMusicTrackName().Contains("ambient")))
      {
        Game1.changeMusicTrack("springtown");
      }
      else
      {
        if (!this.name.Equals((object) "AnimalShop") && !this.name.Equals((object) "ScienceHouse") || !Game1.isMusicContextActiveButNotPlaying() && !Game1.getMusicTrackName().Contains("ambient") || this.currentEvent != null)
          return;
        Game1.changeMusicTrack("marnieShop");
      }
    }

    public NPC isCollidingWithCharacter(Microsoft.Xna.Framework.Rectangle box)
    {
      if (Game1.isFestival() && this.currentEvent != null)
      {
        foreach (NPC actor in this.currentEvent.actors)
        {
          if (actor.GetBoundingBox().Intersects(box))
            return actor;
        }
      }
      foreach (NPC character in this.characters)
      {
        if (character.GetBoundingBox().Intersects(box))
          return character;
      }
      return (NPC) null;
    }

    public virtual void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      if (this.critters != null && Game1.farmEvent == null)
      {
        for (int index = 0; index < this.critters.Count; ++index)
          this.critters[index].drawAboveFrontLayer(b);
      }
      foreach (Character character in this.characters)
        character.drawAboveAlwaysFrontLayer(b);
      foreach (TemporaryAnimatedSprite temporarySprite in this.TemporarySprites)
      {
        if (temporarySprite.drawAboveAlwaysFront)
          temporarySprite.draw(b);
      }
      foreach (Projectile projectile in this.projectiles)
        projectile.draw(b);
    }

    public bool moveObject(int oldX, int oldY, int newX, int newY)
    {
      Vector2 key1 = new Vector2((float) oldX, (float) oldY);
      Vector2 key2 = new Vector2((float) newX, (float) newY);
      if (!this.objects.ContainsKey(key1) || this.objects.ContainsKey(key2))
        return false;
      Object @object = this.objects[key1];
      @object.tileLocation.Value = key2;
      this.objects.Remove(key1);
      this.objects.Add(key2, @object);
      return true;
    }

    private void getGalaxySword()
    {
      Game1.flashAlpha = 1f;
      Game1.player.holdUpItemThenMessage((Item) new MeleeWeapon(4));
      Game1.player.reduceActiveItemByOne();
      if (!Game1.player.addItemToInventoryBool((Item) new MeleeWeapon(4)))
        Game1.createItemDebris((Item) new MeleeWeapon(4), Game1.player.getStandingPosition(), 1);
      Game1.player.mailReceived.Add("galaxySword");
      Game1.player.jitterStrength = 0.0f;
      Game1.screenGlowHold = false;
      Game1.multiplayer.globalChatInfoMessage("GalaxySword", Game1.player.Name);
    }

    public virtual void performTouchAction(string fullActionString, Vector2 playerStandingPosition)
    {
      if (Game1.eventUp)
        return;
      try
      {
        string s = fullActionString.Split(' ')[0];
        // ISSUE: reference to a compiler-generated method
        switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(s))
        {
          case 295776207:
            if (!(s == "Warp"))
              break;
            string locationName = fullActionString.Split(' ')[1];
            int int32_1 = Convert.ToInt32(fullActionString.Split(' ')[2]);
            int int32_2 = Convert.ToInt32(fullActionString.Split(' ')[3]);
            string str1 = fullActionString.Split(' ').Length > 4 ? fullActionString.Split(' ')[4] : (string) null;
            if (str1 != null && !Game1.player.mailReceived.Contains(str1))
              break;
            Game1.warpFarmer(locationName, int32_1, int32_2, false);
            break;
          case 327122275:
            if (!(s == "Emote"))
              break;
            this.getCharacterFromName(fullActionString.Split(' ')[1]).doEmote(Convert.ToInt32(fullActionString.Split(' ')[2]));
            break;
          case 764036487:
            if (!(s == "legendarySword"))
              break;
            if (Game1.player.ActiveObject != null && Utility.IsNormalObjectAtParentSheetIndex((Item) Game1.player.ActiveObject, 74) && !Game1.player.mailReceived.Contains("galaxySword"))
            {
              Game1.player.Halt();
              Game1.player.faceDirection(2);
              Game1.player.showCarrying();
              Game1.player.jitterStrength = 1f;
              Game1.pauseThenDoFunction(7000, new Game1.afterFadeFunction(this.getGalaxySword));
              Game1.changeMusicTrack("none", music_context: Game1.MusicContext.Event);
              this.playSound("crit");
              Game1.screenGlowOnce(new Microsoft.Xna.Framework.Color(30, 0, 150), true, 0.01f, 0.999f);
              DelayedAction.playSoundAfterDelay("stardrop", 1500);
              Game1.screenOverlayTempSprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), 500, Microsoft.Xna.Framework.Color.White, 10, 2000));
              Game1.afterDialogues += (Game1.afterFadeFunction) (() => Game1.stopMusicTrack(Game1.MusicContext.Event));
              break;
            }
            if (Game1.player.mailReceived.Contains("galaxySword"))
              break;
            this.localSound("SpringBirds");
            break;
          case 799419560:
            if (!(s == "Sleep") || Game1.newDay || !Game1.shouldTimePass() || !Game1.player.hasMoved || Game1.player.passedOut)
              break;
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep"), this.createYesNoResponses(), "Sleep", (Object) null);
            break;
          case 1301151257:
            if (!(s == "PoolEntrance"))
              break;
            if (!(bool) (NetFieldBase<bool, NetBool>) Game1.player.swimming)
            {
              Game1.player.swimTimer = 800;
              Game1.player.swimming.Value = true;
              Game1.player.position.Y += 16f;
              Game1.player.yVelocity = -8f;
              this.playSound("pullItemFromWater");
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(27, 100f, 4, 0, new Vector2(Game1.player.Position.X, (float) (Game1.player.getStandingY() - 40)), false, false)
              {
                layerDepth = 1f,
                motion = new Vector2(0.0f, 2f)
              });
            }
            else
            {
              Game1.player.jump();
              Game1.player.swimTimer = 800;
              Game1.player.position.X = playerStandingPosition.X * 64f;
              this.playSound("pullItemFromWater");
              Game1.player.yVelocity = 8f;
              Game1.player.swimming.Value = false;
            }
            Game1.player.noMovementPause = 500;
            break;
          case 1421563949:
            if (!(s == "FaceDirection") || this.getCharacterFromName(fullActionString.Split(' ')[1]) == null)
              break;
            this.getCharacterFromName(fullActionString.Split(' ')[1]).faceDirection(Convert.ToInt32(fullActionString.Split(' ')[2]));
            break;
          case 1817135690:
            if (!(s == "WomensLocker") || !Game1.player.IsMale)
              break;
            Game1.player.position.Y += (float) ((Game1.player.Speed + Game1.player.addedSpeed) * 2);
            Game1.player.Halt();
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WomensLocker_WrongGender"));
            break;
          case 2295680585:
            if (!(s == "Door"))
              break;
            for (int index = 1; index < fullActionString.Split(' ').Length; ++index)
            {
              if (Game1.player.getFriendshipHeartLevelForNPC(fullActionString.Split(' ')[index]) < 2 && index == fullActionString.Split(' ').Length - 1)
              {
                Farmer player = Game1.player;
                player.Position = player.Position - Game1.player.getMostRecentMovementVector() * 2f;
                Game1.player.yVelocity = 0.0f;
                Game1.player.Halt();
                Game1.player.TemporaryPassableTiles.Clear();
                if (Game1.player.getTileLocation().Equals(this.lastTouchActionLocation))
                {
                  if ((double) Game1.player.Position.Y > (double) this.lastTouchActionLocation.Y * 64.0 + 32.0)
                    Game1.player.position.Y += 4f;
                  else
                    Game1.player.position.Y -= 4f;
                  this.lastTouchActionLocation = Vector2.Zero;
                }
                if (Game1.player.mailReceived.Contains("doorUnlock" + fullActionString.Split(' ')[1]) && (fullActionString.Split(' ').Length == 2 || Game1.player.mailReceived.Contains("doorUnlock" + fullActionString.Split(' ')[2])) || fullActionString.Split(' ').Length == 3 && Game1.player.mailReceived.Contains("doorUnlock" + fullActionString.Split(' ')[2]))
                  break;
                if (fullActionString.Split(' ').Length == 2)
                {
                  NPC characterFromName = Game1.getCharacterFromName(fullActionString.Split(' ')[1]);
                  string str2 = characterFromName.Gender == 0 ? "Male" : "Female";
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_" + str2, (object) characterFromName.displayName));
                  break;
                }
                NPC characterFromName1 = Game1.getCharacterFromName(fullActionString.Split(' ')[1]);
                NPC characterFromName2 = Game1.getCharacterFromName(fullActionString.Split(' ')[2]);
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_Couple", (object) characterFromName1.displayName, (object) characterFromName2.displayName));
                break;
              }
              if (index != fullActionString.Split(' ').Length - 1 && Game1.player.getFriendshipHeartLevelForNPC(fullActionString.Split(' ')[index]) >= 2)
              {
                if (Game1.player.mailReceived.Contains("doorUnlock" + fullActionString.Split(' ')[index]))
                  break;
                Game1.player.mailReceived.Add("doorUnlock" + fullActionString.Split(' ')[index]);
                break;
              }
              if (index == fullActionString.Split(' ').Length - 1 && Game1.player.getFriendshipHeartLevelForNPC(fullActionString.Split(' ')[index]) >= 2)
              {
                if (Game1.player.mailReceived.Contains("doorUnlock" + fullActionString.Split(' ')[index]))
                  break;
                Game1.player.mailReceived.Add("doorUnlock" + fullActionString.Split(' ')[index]);
                break;
              }
            }
            break;
          case 2596419570:
            if (!(s == "ChangeIntoSwimsuit"))
              break;
            Game1.player.changeIntoSwimsuit();
            break;
          case 3302649497:
            if (!(s == "Bus"))
              break;
            this.boardBus(playerStandingPosition);
            break;
          case 3579946100:
            if (!(s == "MensLocker") || Game1.player.IsMale)
              break;
            Game1.player.position.Y += (float) ((Game1.player.Speed + Game1.player.addedSpeed) * 2);
            Game1.player.Halt();
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MensLocker_WrongGender"));
            break;
          case 3711744508:
            if (!(s == "MagicalSeal") || Game1.player.mailReceived.Contains("krobusUnseal"))
              break;
            Farmer player1 = Game1.player;
            player1.Position = player1.Position - Game1.player.getMostRecentMovementVector() * 2f;
            Game1.player.yVelocity = 0.0f;
            Game1.player.Halt();
            Game1.player.TemporaryPassableTiles.Clear();
            if (Game1.player.getTileLocation().Equals(this.lastTouchActionLocation))
            {
              if ((double) Game1.player.position.Y > (double) this.lastTouchActionLocation.Y * 64.0 + 32.0)
                Game1.player.position.Y += 4f;
              else
                Game1.player.position.Y -= 4f;
              this.lastTouchActionLocation = Vector2.Zero;
            }
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_MagicSeal"));
            for (int index = 0; index < 40; ++index)
            {
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(666, 1851, 8, 8), 25f, 4, 2, new Vector2(3f, 19f) * 64f + new Vector2((float) (index % 4 * 16 - 8), (float) (-(index / 4) * 64 / 4)), false, false)
              {
                layerDepth = (float) (0.115199998021126 + (double) index / 10000.0),
                color = new Microsoft.Xna.Framework.Color(100 + index * 4, index * 5, 120 + index * 4),
                pingPong = true,
                delayBeforeAnimationStart = index * 10,
                scale = 4f,
                alphaFade = 0.01f
              });
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(666, 1851, 8, 8), 25f, 4, 2, new Vector2(3f, 17f) * 64f + new Vector2((float) (index % 4 * 16 - 8), (float) (index / 4 * 64 / 4)), false, false)
              {
                layerDepth = (float) (0.115199998021126 + (double) index / 10000.0),
                color = new Microsoft.Xna.Framework.Color(232 - index * 4, 192 - index * 6, (int) byte.MaxValue - index * 4),
                pingPong = true,
                delayBeforeAnimationStart = 320 + index * 10,
                scale = 4f,
                alphaFade = 0.01f
              });
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(666, 1851, 8, 8), 25f, 4, 2, new Vector2(3f, 19f) * 64f + new Vector2((float) (index % 4 * 16 - 8), (float) (-(index / 4) * 64 / 4)), false, false)
              {
                layerDepth = (float) (0.115199998021126 + (double) index / 10000.0),
                color = new Microsoft.Xna.Framework.Color(100 + index * 4, index * 6, 120 + index * 4),
                pingPong = true,
                delayBeforeAnimationStart = 640 + index * 10,
                scale = 4f,
                alphaFade = 0.01f
              });
            }
            Game1.player.jitterStrength = 2f;
            Game1.player.freezePause = 500;
            this.playSound("debuffHit");
            break;
          case 3998141403:
            if (!(s == "ChangeOutOfSwimsuit"))
              break;
            Game1.player.changeOutOfSwimSuit();
            break;
          case 4271868850:
            if (!(s == "MagicWarp"))
              break;
            string locationToWarp = fullActionString.Split(' ')[1];
            int locationX = Convert.ToInt32(fullActionString.Split(' ')[2]);
            int locationY = Convert.ToInt32(fullActionString.Split(' ')[3]);
            string str3 = fullActionString.Split(' ').Length > 4 ? fullActionString.Split(' ')[4] : (string) null;
            if (str3 != null && !Game1.player.mailReceived.Contains(str3))
              break;
            for (int index = 0; index < 12; ++index)
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) Game1.player.position.X - 256, (int) Game1.player.position.X + 192), (float) Game1.random.Next((int) Game1.player.position.Y - 256, (int) Game1.player.position.Y + 192)), false, Game1.random.NextDouble() < 0.5));
            this.playSound("wand");
            Game1.freezeControls = true;
            Game1.displayFarmer = false;
            Game1.player.CanMove = false;
            Game1.flashAlpha = 1f;
            DelayedAction.fadeAfterDelay((Game1.afterFadeFunction) (() =>
            {
              Game1.warpFarmer(locationToWarp, locationX, locationY, false);
              Game1.fadeToBlackAlpha = 0.99f;
              Game1.screenGlow = false;
              Game1.displayFarmer = true;
              Game1.player.CanMove = true;
              Game1.freezeControls = false;
            }), 1000);
            new Microsoft.Xna.Framework.Rectangle(Game1.player.GetBoundingBox().X, Game1.player.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
            int num = 0;
            for (int x = Game1.player.getTileX() + 8; x >= Game1.player.getTileX() - 8; --x)
            {
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(6, new Vector2((float) x, (float) Game1.player.getTileY()) * 64f, Microsoft.Xna.Framework.Color.White, animationInterval: 50f)
              {
                layerDepth = 1f,
                delayBeforeAnimationStart = num * 25,
                motion = new Vector2(-0.25f, 0.0f)
              });
              ++num;
            }
            break;
        }
      }
      catch (Exception ex)
      {
      }
    }

    public virtual void updateMap()
    {
      if (object.Equals((object) this.mapPath.Value, (object) this.loadedMapPath))
        return;
      this.reloadMap();
    }

    public LargeTerrainFeature getLargeTerrainFeatureAt(int tileX, int tileY)
    {
      foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
      {
        if (largeTerrainFeature.getBoundingBox().Contains(tileX * 64 + 32, tileY * 64 + 32))
          return largeTerrainFeature;
      }
      return (LargeTerrainFeature) null;
    }

    public virtual void UpdateWhenCurrentLocation(GameTime time)
    {
      this.updateMap();
      if (this.wasUpdated)
        return;
      this.wasUpdated = true;
      if (this._mapSeatsDirty)
        this.UpdateMapSeats();
      this.furnitureToRemove.Update(this);
      foreach (Object @object in this.furniture)
        @object.updateWhenCurrentLocation(time, this);
      AmbientLocationSounds.update(time);
      if (this.critters != null)
      {
        for (int index = this.critters.Count - 1; index >= 0; --index)
        {
          if (this.critters[index].update(time, this))
            this.critters.RemoveAt(index);
        }
      }
      if (this.fishSplashAnimation != null)
      {
        this.fishSplashAnimation.update(time);
        if (Game1.random.NextDouble() < 0.02)
          this.temporarySprites.Add(new TemporaryAnimatedSprite(0, this.fishSplashAnimation.position + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-32, 32)), Microsoft.Xna.Framework.Color.White * 0.3f));
      }
      if (this.orePanAnimation != null)
      {
        this.orePanAnimation.update(time);
        if (Game1.random.NextDouble() < 0.05)
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16, 16), this.orePanAnimation.position + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-32, 32)), false, 0.02f, Microsoft.Xna.Framework.Color.White * 0.8f)
          {
            scale = 2f,
            animationLength = 6,
            interval = 100f
          });
      }
      this.interiorDoors.Update(time);
      this.updateWater(time);
      this.Map.Update((long) time.ElapsedGameTime.Milliseconds);
      int index1 = 0;
      while (index1 < this.debris.Count)
      {
        if (this.debris[index1].updateChunks(time, this))
          this.debris.RemoveAt(index1);
        else
          ++index1;
      }
      if (Game1.shouldTimePass() || Game1.isFestival())
      {
        int index2 = 0;
        while (index2 < this.projectiles.Count)
        {
          if (this.projectiles[index2].update(time, this))
            this.projectiles.RemoveAt(index2);
          else
            ++index2;
        }
      }
      if (true)
      {
        for (int index3 = 0; index3 < this._activeTerrainFeatures.Count; ++index3)
        {
          TerrainFeature activeTerrainFeature = this._activeTerrainFeatures[index3];
          if (activeTerrainFeature.tickUpdate(time, activeTerrainFeature.currentTileLocation, this))
            this.terrainFeaturesToRemoveList.Add(activeTerrainFeature.currentTileLocation);
        }
      }
      else
      {
        foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.terrainFeatures.Pairs)
        {
          if (pair.Value.tickUpdate(time, pair.Key, this))
            this.terrainFeaturesToRemoveList.Add(pair.Key);
        }
      }
      foreach (Vector2 featuresToRemove in this.terrainFeaturesToRemoveList)
        this.terrainFeatures.Remove(featuresToRemove);
      this.terrainFeaturesToRemoveList.Clear();
      if (this.largeTerrainFeatures != null)
      {
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
          largeTerrainFeature.tickUpdate(time, this);
      }
      foreach (ResourceClump resourceClump in this.resourceClumps)
        resourceClump.tickUpdate(time, (Vector2) (NetFieldBase<Vector2, NetVector2>) resourceClump.tile, this);
      if (Game1.timeOfDay >= 2000 && (double) (float) (NetFieldBase<float, NetFloat>) this.lightLevel > 0.0 && this.name.Equals((object) "FarmHouse"))
        Game1.currentLightSources.Add(new LightSource(4, new Vector2(64f, 448f), 2f));
      if (this.currentEvent != null)
      {
        bool flag;
        do
        {
          int currentCommand = this.currentEvent.CurrentCommand;
          this.currentEvent.checkForNextCommand(this, time);
          if (this.currentEvent != null)
          {
            flag = this.currentEvent.simultaneousCommand;
            if (currentCommand == this.currentEvent.CurrentCommand)
              flag = false;
          }
          else
            flag = false;
        }
        while (flag);
      }
      foreach (Object @object in this.objects.Values)
        this.tempObjects.Add(@object);
      foreach (Object tempObject in this.tempObjects)
        tempObject.updateWhenCurrentLocation(time, this);
      this.tempObjects.Clear();
      if (Game1.gameMode != (byte) 3 || this != Game1.currentLocation)
        return;
      if (!Utility.IsDesertLocation(Game1.currentLocation))
      {
        if ((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors && !Game1.IsRainingHere(this) && Game1.random.NextDouble() < 0.002 && Game1.isMusicContextActiveButNotPlaying() && Game1.timeOfDay < 2000 && !Game1.currentSeason.Equals("winter") && !this.name.Equals((object) "Desert"))
          this.localSound("SpringBirds");
        else if (!Game1.IsRainingHere(this) && (bool) (NetFieldBase<bool, NetBool>) this.isOutdoors && Game1.timeOfDay > 2100 && Game1.currentSeason.Equals("summer") && Game1.random.NextDouble() < 0.0005 && !(this is Beach) && !this.name.Equals((object) "temp"))
          this.localSound("crickets");
        else if (Game1.IsRainingHere(this) && (bool) (NetFieldBase<bool, NetBool>) this.isOutdoors && !this.name.Equals((object) "Town") && !Game1.eventUp && (double) Game1.options.musicVolumeLevel > 0.0 && Game1.random.NextDouble() < 0.00015)
          this.localSound("rainsound");
      }
      Vector2 vector2 = new Vector2((float) (Game1.player.getStandingX() / 64), (float) (Game1.player.getStandingY() / 64));
      if (this.lastTouchActionLocation.Equals(Vector2.Zero))
      {
        string fullActionString = this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "TouchAction", "Back");
        this.lastTouchActionLocation = new Vector2((float) (Game1.player.getStandingX() / 64), (float) (Game1.player.getStandingY() / 64));
        if (fullActionString != null)
          this.performTouchAction(fullActionString, vector2);
      }
      else if (!this.lastTouchActionLocation.Equals(vector2))
        this.lastTouchActionLocation = Vector2.Zero;
      foreach (Character farmer in this.farmers)
      {
        Vector2 tileLocation = farmer.getTileLocation();
        foreach (Vector2 adjacentTilesOffset in Character.AdjacentTilesOffsets)
        {
          Object @object;
          if (this.objects.TryGetValue(tileLocation + adjacentTilesOffset, out @object))
            @object.farmerAdjacentAction(this);
        }
      }
      if (!Game1.boardingBus)
        return;
      NPC characterFromName = this.getCharacterFromName("Pam");
      if (characterFromName == null || this.doesTileHaveProperty(characterFromName.getStandingX() / 64, characterFromName.getStandingY() / 64, "TouchAction", "Back") == null)
        return;
      this.busLeave();
    }

    public void updateWater(GameTime time)
    {
      this.waterAnimationTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.waterAnimationTimer <= 0)
      {
        this.waterAnimationIndex = (this.waterAnimationIndex + 1) % 10;
        this.waterAnimationTimer = 200;
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isFarm)
        this.waterPosition += (float) ((Math.Sin((double) time.TotalGameTime.Milliseconds / 1000.0) + 1.0) * 0.150000005960464);
      else
        this.waterPosition += 0.1f;
      if ((double) this.waterPosition < 64.0)
        return;
      this.waterPosition -= 64f;
      this.waterTileFlip = !this.waterTileFlip;
    }

    public NPC getCharacterFromName(string name)
    {
      NPC characterFromName = (NPC) null;
      foreach (NPC character in this.characters)
      {
        if (character.Name.Equals(name))
          return character;
      }
      return characterFromName;
    }

    protected virtual void updateCharacters(GameTime time)
    {
      bool flag = Game1.shouldTimePass();
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        NPC character = this.characters[index];
        if (character != null && (flag || character is Horse || character.forceUpdateTimer > 0))
        {
          character.currentLocation = this;
          character.update(time, this);
          if (index < this.characters.Count && character is Monster && ((Monster) character).Health <= 0)
            this.characters.RemoveAt(index);
        }
        else if (character != null)
        {
          if (character.hasJustStartedFacingPlayer)
            character.updateFaceTowardsFarmer(time, this);
          character.updateEmote(time);
        }
      }
    }

    public virtual void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
    {
      this.netAudio.Update();
      this.removeTemporarySpritesWithIDEvent.Poll();
      this.rumbleAndFadeEvent.Poll();
      this.damagePlayersEvent.Poll();
      if (!ignoreWasUpdatedFlush)
        this.wasUpdated = false;
      this.updateCharacters(time);
      for (int index = this.temporarySprites.Count - 1; index >= 0; --index)
      {
        TemporaryAnimatedSprite temporaryAnimatedSprite = index < this.temporarySprites.Count ? this.temporarySprites[index] : (TemporaryAnimatedSprite) null;
        if (index < this.temporarySprites.Count && temporaryAnimatedSprite != null && temporaryAnimatedSprite.update(time) && index < this.temporarySprites.Count)
          this.temporarySprites.RemoveAt(index);
      }
    }

    public Response[] createYesNoResponses() => new Response[2]
    {
      new Response("Yes", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes")).SetHotKey(Keys.Y),
      new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")).SetHotKey(Keys.Escape)
    };

    public void createQuestionDialogue(string question, Response[] answerChoices, string dialogKey)
    {
      this.lastQuestionKey = dialogKey;
      Game1.drawObjectQuestionDialogue(question, ((IEnumerable<Response>) answerChoices).ToList<Response>());
    }

    public void createQuestionDialogueWithCustomWidth(
      string question,
      Response[] answerChoices,
      string dialogKey)
    {
      int width = SpriteText.getWidthOfString(question) + 64;
      this.lastQuestionKey = dialogKey;
      Game1.drawObjectQuestionDialogue(question, ((IEnumerable<Response>) answerChoices).ToList<Response>(), width);
    }

    public void createQuestionDialogue(
      string question,
      Response[] answerChoices,
      GameLocation.afterQuestionBehavior afterDialogueBehavior,
      NPC speaker = null)
    {
      this.afterQuestion = afterDialogueBehavior;
      Game1.drawObjectQuestionDialogue(question, ((IEnumerable<Response>) answerChoices).ToList<Response>());
      if (speaker == null)
        return;
      Game1.objectDialoguePortraitPerson = speaker;
    }

    public void createQuestionDialogue(
      string question,
      Response[] answerChoices,
      string dialogKey,
      Object actionObject)
    {
      this.lastQuestionKey = dialogKey;
      Game1.drawObjectQuestionDialogue(question, ((IEnumerable<Response>) answerChoices).ToList<Response>());
      this.actionObjectForQuestionDialogue = actionObject;
    }

    public virtual Point GetMapPropertyPosition(string key, int default_x, int default_y)
    {
      string str = "";
      if (this.Map.Properties.ContainsKey(key))
        str = this.map.Properties[key].ToString();
      if (str != "")
      {
        int result1 = -1;
        int result2 = -1;
        string[] strArray = str.Split(' ');
        if (strArray.Length >= 2 && int.TryParse(strArray[0], out result1) && int.TryParse(strArray[1], out result2))
          return new Point(result1, result2);
      }
      return new Point(default_x, default_y);
    }

    public virtual void monsterDrop(Monster monster, int x, int y, Farmer who)
    {
      int coinsToDrop = (int) (NetFieldBase<int, NetInt>) monster.coinsToDrop;
      IList<int> objectsToDrop = (IList<int>) monster.objectsToDrop;
      Vector2 vector2;
      ref Vector2 local = ref vector2;
      Microsoft.Xna.Framework.Rectangle boundingBox = Game1.player.GetBoundingBox();
      double x1 = (double) boundingBox.Center.X;
      boundingBox = Game1.player.GetBoundingBox();
      double y1 = (double) boundingBox.Center.Y;
      local = new Vector2((float) x1, (float) y1);
      List<Item> extraDropItems1 = monster.getExtraDropItems();
      if (Game1.player.isWearingRing(526))
      {
        string str = "";
        Game1.content.Load<Dictionary<string, string>>("Data\\Monsters").TryGetValue(monster.Name, out str);
        if (str != null && str.Length > 0)
        {
          string[] strArray = str.Split('/')[6].Split(' ');
          for (int index = 0; index < strArray.Length; index += 2)
          {
            if (Game1.random.NextDouble() < Convert.ToDouble(strArray[index + 1]))
              objectsToDrop.Add(Convert.ToInt32(strArray[index]));
          }
        }
      }
      for (int index = 0; index < objectsToDrop.Count; ++index)
      {
        int objectIndex = objectsToDrop[index];
        if (objectIndex < 0)
          this.debris.Add(monster.ModifyMonsterLoot(new Debris(Math.Abs(objectIndex), Game1.random.Next(1, 4), new Vector2((float) x, (float) y), vector2)));
        else
          this.debris.Add(monster.ModifyMonsterLoot(new Debris(objectIndex, new Vector2((float) x, (float) y), vector2)));
      }
      for (int index = 0; index < extraDropItems1.Count; ++index)
        this.debris.Add(monster.ModifyMonsterLoot(new Debris(extraDropItems1[index], new Vector2((float) x, (float) y), vector2)));
      if (Game1.player.isWearingRing(526))
      {
        List<Item> extraDropItems2 = monster.getExtraDropItems();
        for (int index = 0; index < extraDropItems2.Count; ++index)
        {
          Item one = extraDropItems2[index].getOne();
          one.Stack = extraDropItems2[index].Stack;
          one.HasBeenInInventory = false;
          this.debris.Add(monster.ModifyMonsterLoot(new Debris(one, new Vector2((float) x, (float) y), vector2)));
        }
      }
      if (this.HasUnlockedAreaSecretNotes(Game1.player) && Game1.random.NextDouble() < 0.033)
      {
        Object unseenSecretNote = this.tryToCreateUnseenSecretNote(Game1.player);
        if (unseenSecretNote != null)
          monster.ModifyMonsterLoot(Game1.createItemDebris((Item) unseenSecretNote, new Vector2((float) x, (float) y), -1, this));
      }
      if (this is Woods && Game1.random.NextDouble() < 0.1)
        monster.ModifyMonsterLoot(Game1.createItemDebris((Item) new Object(292, 1), new Vector2((float) x, (float) y), -1, this));
      if ((bool) (NetFieldBase<bool, NetBool>) monster.isHardModeMonster && Game1.stats.getStat("hardModeMonstersKilled") > 50U && Game1.random.NextDouble() < 0.001 + (double) who.LuckLevel * 0.000199999994947575)
      {
        monster.ModifyMonsterLoot(Game1.createItemDebris((Item) new Object(896, 1), new Vector2((float) x, (float) y), -1, this));
      }
      else
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) monster.isHardModeMonster || Game1.random.NextDouble() >= 0.008 + (double) who.LuckLevel * (1.0 / 500.0))
          return;
        monster.ModifyMonsterLoot(Game1.createItemDebris((Item) new Object(858, 1), new Vector2((float) x, (float) y), -1, this));
      }
    }

    public virtual bool HasUnlockedAreaSecretNotes(Farmer who) => this.GetLocationContext() == GameLocation.LocationContext.Island || who.hasMagnifyingGlass;

    public bool damageMonster(
      Microsoft.Xna.Framework.Rectangle areaOfEffect,
      int minDamage,
      int maxDamage,
      bool isBomb,
      Farmer who)
    {
      return this.damageMonster(areaOfEffect, minDamage, maxDamage, isBomb, 1f, 0, 0.0f, 1f, false, who);
    }

    private bool isMonsterDamageApplicable(Farmer who, Monster monster, bool horizontalBias = true)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) monster.isGlider && !(who.CurrentTool is Slingshot) && !monster.ignoreDamageLOS.Value)
      {
        Point tileLocationPoint1 = who.getTileLocationPoint();
        Point tileLocationPoint2 = monster.getTileLocationPoint();
        if (Math.Abs(tileLocationPoint1.X - tileLocationPoint2.X) + Math.Abs(tileLocationPoint1.Y - tileLocationPoint2.Y) > 1)
        {
          int num1 = tileLocationPoint2.X - tileLocationPoint1.X;
          int num2 = tileLocationPoint2.Y - tileLocationPoint1.Y;
          Vector2 key = new Vector2((float) tileLocationPoint1.X, (float) tileLocationPoint1.Y);
          while (num1 != 0 || num2 != 0)
          {
            if (horizontalBias)
            {
              if (Math.Abs(num1) >= Math.Abs(num2))
              {
                key.X += (float) Math.Sign(num1);
                num1 -= Math.Sign(num1);
              }
              else
              {
                key.Y += (float) Math.Sign(num2);
                num2 -= Math.Sign(num2);
              }
            }
            else if (Math.Abs(num2) >= Math.Abs(num1))
            {
              key.Y += (float) Math.Sign(num2);
              num2 -= Math.Sign(num2);
            }
            else
            {
              key.X += (float) Math.Sign(num1);
              num1 -= Math.Sign(num1);
            }
            if (this.objects.ContainsKey(key) && !this.objects[key].isPassable() || this.BlocksDamageLOS((int) key.X, (int) key.Y))
              return false;
          }
        }
      }
      return true;
    }

    public virtual bool BlocksDamageLOS(int x, int y) => this.getTileIndexAt(x, y, "Buildings") != -1 && this.doesTileHaveProperty(x, y, "Passable", "Buildings") == null;

    public bool damageMonster(
      Microsoft.Xna.Framework.Rectangle areaOfEffect,
      int minDamage,
      int maxDamage,
      bool isBomb,
      float knockBackModifier,
      int addedPrecision,
      float critChance,
      float critMultiplier,
      bool triggerMonsterInvincibleTimer,
      Farmer who)
    {
      bool flag1 = false;
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        if (index < this.characters.Count && this.characters[index] is Monster character && character.IsMonster && character.Health > 0 && character.TakesDamageFromHitbox(areaOfEffect))
        {
          if (character.currentLocation == null)
            character.currentLocation = this;
          if (!character.IsInvisible && !character.isInvincible() && (isBomb || this.isMonsterDamageApplicable(who, character) || this.isMonsterDamageApplicable(who, character, false)))
          {
            bool flag2 = !isBomb && who != null && who.CurrentTool != null && who.CurrentTool is MeleeWeapon && (int) (NetFieldBase<int, NetInt>) (who.CurrentTool as MeleeWeapon).type == 1;
            bool flag3 = false;
            if (flag2 && MeleeWeapon.daggerHitsLeft > 1)
              flag3 = true;
            if (flag3)
              triggerMonsterInvincibleTimer = false;
            flag1 = true;
            if (Game1.currentLocation == this)
              Rumble.rumble(0.1f + (float) (Game1.random.NextDouble() / 8.0), (float) (200 + Game1.random.Next(-50, 50)));
            Microsoft.Xna.Framework.Rectangle boundingBox = character.GetBoundingBox();
            Vector2 trajectory = Utility.getAwayFromPlayerTrajectory(boundingBox, who);
            if ((double) knockBackModifier > 0.0)
              trajectory *= knockBackModifier;
            else
              trajectory = new Vector2(character.xVelocity, character.yVelocity);
            if (character.Slipperiness == -1)
              trajectory = Vector2.Zero;
            bool flag4 = false;
            if (who != null && who.CurrentTool != null && character.hitWithTool(who.CurrentTool))
              return false;
            if (who.professions.Contains(25))
              critChance += critChance * 0.5f;
            int amount1;
            if (maxDamage >= 0)
            {
              int num = Game1.random.Next(minDamage, maxDamage + 1);
              if (who != null && Game1.random.NextDouble() < (double) critChance + (double) who.LuckLevel * ((double) critChance / 40.0))
              {
                flag4 = true;
                this.playSound("crit");
              }
              int amount2 = Math.Max(1, (flag4 ? (int) ((double) num * (double) critMultiplier) : num) + (who != null ? who.attack * 3 : 0));
              if (who != null && who.professions.Contains(24))
                amount2 = (int) Math.Ceiling((double) amount2 * 1.10000002384186);
              if (who != null && who.professions.Contains(26))
                amount2 = (int) Math.Ceiling((double) amount2 * 1.14999997615814);
              if (who != null & flag4 && who.professions.Contains(29))
                amount2 = (int) ((double) amount2 * 2.0);
              if (who != null)
              {
                foreach (BaseEnchantment enchantment in who.enchantments)
                  enchantment.OnCalculateDamage(character, this, who, ref amount2);
              }
              amount1 = character.takeDamage(amount2, (int) trajectory.X, (int) trajectory.Y, isBomb, (double) addedPrecision / 10.0, who);
              character.stunTime = !flag3 ? 0 : 50;
              if (amount1 == -1)
              {
                this.debris.Add(new Debris("Miss", 1, new Vector2((float) boundingBox.Center.X, (float) boundingBox.Center.Y), Microsoft.Xna.Framework.Color.LightGray, 1f, 0.0f));
              }
              else
              {
                this.removeDamageDebris(character);
                this.debris.Add(new Debris(amount1, new Vector2((float) (boundingBox.Center.X + 16), (float) boundingBox.Center.Y), flag4 ? Microsoft.Xna.Framework.Color.Yellow : new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 130, 0), flag4 ? (float) (1.0 + (double) amount1 / 300.0) : 1f, (Character) character));
                if (who != null)
                {
                  foreach (BaseEnchantment enchantment in who.enchantments)
                    enchantment.OnDealDamage(character, this, who, ref amount1);
                }
              }
              if (triggerMonsterInvincibleTimer)
                character.setInvincibleCountdown(450 / (flag2 ? 3 : 2));
            }
            else
            {
              amount1 = -2;
              character.setTrajectory(trajectory);
              if (character.Slipperiness > 10)
              {
                character.xVelocity /= 2f;
                character.yVelocity /= 2f;
              }
            }
            if (who != null && who.CurrentTool != null && who.CurrentTool.Name.Equals("Galaxy Sword"))
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(50, 120), 6, 1, new Vector2((float) (boundingBox.Center.X - 32), (float) (boundingBox.Center.Y - 32)), false, false));
            if (character.Health <= 0)
            {
              if (!(bool) (NetFieldBase<bool, NetBool>) this.isFarm)
                who.checkForQuestComplete((NPC) null, 1, 1, (Item) null, character.Name, 4);
              if (!(bool) (NetFieldBase<bool, NetBool>) this.isFarm && Game1.player.team.specialOrders != null)
              {
                foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                {
                  if (specialOrder.onMonsterSlain != null)
                    specialOrder.onMonsterSlain(Game1.player, character);
                }
              }
              if (who != null)
              {
                foreach (BaseEnchantment enchantment in who.enchantments)
                  enchantment.OnMonsterSlay(character, this, who);
              }
              if (who != null && who.leftRing.Value != null)
                who.leftRing.Value.onMonsterSlay(character, this, who);
              if (who != null && who.rightRing.Value != null)
                who.rightRing.Value.onMonsterSlay(character, this, who);
              if (who != null && !(bool) (NetFieldBase<bool, NetBool>) this.isFarm && (!(character is GreenSlime) || (bool) (NetFieldBase<bool, NetBool>) (character as GreenSlime).firstGeneration))
              {
                if (who.IsLocalPlayer)
                  Game1.stats.monsterKilled(character.Name);
                else if (Game1.IsMasterGame)
                  who.queueMessage((byte) 25, Game1.player, (object) character.Name);
              }
              this.monsterDrop(character, boundingBox.Center.X, boundingBox.Center.Y, who);
              if (who != null && !(bool) (NetFieldBase<bool, NetBool>) this.isFarm)
                who.gainExperience(4, character.ExperienceGained);
              if ((bool) (NetFieldBase<bool, NetBool>) character.isHardModeMonster)
                Game1.stats.incrementStat("hardModeMonstersKilled", 1);
              this.characters.Remove((NPC) character);
              ++Game1.stats.MonstersKilled;
            }
            else if (amount1 > 0)
            {
              character.shedChunks(Game1.random.Next(1, 3));
              if (flag4)
              {
                Vector2 standingPosition = character.getStandingPosition();
                Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(15, 50), 6, 1, standingPosition - new Vector2(32f, 32f), false, Game1.random.NextDouble() < 0.5)
                {
                  scale = 0.75f,
                  alpha = flag4 ? 0.75f : 0.5f
                });
                Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(15, 50), 6, 1, standingPosition - new Vector2((float) (32 + Game1.random.Next(-21, 21) + 32), (float) (32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                {
                  scale = 0.5f,
                  delayBeforeAnimationStart = 50,
                  alpha = flag4 ? 0.75f : 0.5f
                });
                Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(15, 50), 6, 1, standingPosition - new Vector2((float) (32 + Game1.random.Next(-21, 21) - 32), (float) (32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                {
                  scale = 0.5f,
                  delayBeforeAnimationStart = 100,
                  alpha = flag4 ? 0.75f : 0.5f
                });
                Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(15, 50), 6, 1, standingPosition - new Vector2((float) (32 + Game1.random.Next(-21, 21) + 32), (float) (32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                {
                  scale = 0.5f,
                  delayBeforeAnimationStart = 150,
                  alpha = flag4 ? 0.75f : 0.5f
                });
                Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(15, 50), 6, 1, standingPosition - new Vector2((float) (32 + Game1.random.Next(-21, 21) - 32), (float) (32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                {
                  scale = 0.5f,
                  delayBeforeAnimationStart = 200,
                  alpha = flag4 ? 0.75f : 0.5f
                });
              }
            }
          }
        }
      }
      return flag1;
    }

    public void moveCharacters(GameTime time)
    {
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        NPC character = this.characters[index];
        if (!character.IsInvisible)
          character.updateMovement(this, time);
      }
    }

    public void growWeedGrass(int iterations)
    {
      for (int index1 = 0; index1 < iterations; ++index1)
      {
        for (int index2 = this.terrainFeatures.Count() - 1; index2 >= 0; --index2)
        {
          KeyValuePair<Vector2, TerrainFeature> keyValuePair = this.terrainFeatures.Pairs.ElementAt(index2);
          if (keyValuePair.Value is Grass && Game1.random.NextDouble() < 0.65)
          {
            if ((int) (NetFieldBase<int, NetInt>) ((Grass) keyValuePair.Value).numberOfWeeds < 4)
              ((Grass) keyValuePair.Value).numberOfWeeds.Value = Math.Max(0, Math.Min(4, (int) (NetFieldBase<int, NetInt>) ((Grass) keyValuePair.Value).numberOfWeeds + Game1.random.Next(3)));
            else if ((int) (NetFieldBase<int, NetInt>) ((Grass) keyValuePair.Value).numberOfWeeds >= 4)
            {
              int x = (int) keyValuePair.Key.X;
              int y = (int) keyValuePair.Key.Y;
              if (this.isTileOnMap(x, y) && !this.isTileOccupied(keyValuePair.Key + new Vector2(-1f, 0.0f)) && this.isTileLocationOpenIgnoreFrontLayers(new Location(x - 1, y)) && this.doesTileHaveProperty(x - 1, y, "Diggable", "Back") != null && this.doesTileHaveProperty(x - 1, y, "NoSpawn", "Back") == null && Game1.random.NextDouble() < 0.25)
                this.terrainFeatures.Add(keyValuePair.Key + new Vector2(-1f, 0.0f), (TerrainFeature) new Grass((int) (byte) (NetFieldBase<byte, NetByte>) ((Grass) keyValuePair.Value).grassType, Game1.random.Next(1, 3)));
              if (this.isTileOnMap(x, y) && !this.isTileOccupied(keyValuePair.Key + new Vector2(1f, 0.0f)) && this.isTileLocationOpenIgnoreFrontLayers(new Location(x + 1, y)) && this.doesTileHaveProperty(x + 1, y, "Diggable", "Back") != null && this.doesTileHaveProperty(x + 1, y, "NoSpawn", "Back") == null && Game1.random.NextDouble() < 0.25)
                this.terrainFeatures.Add(keyValuePair.Key + new Vector2(1f, 0.0f), (TerrainFeature) new Grass((int) (byte) (NetFieldBase<byte, NetByte>) ((Grass) keyValuePair.Value).grassType, Game1.random.Next(1, 3)));
              if (this.isTileOnMap(x, y) && !this.isTileOccupied(keyValuePair.Key + new Vector2(0.0f, 1f)) && this.isTileLocationOpenIgnoreFrontLayers(new Location(x, y + 1)) && this.doesTileHaveProperty(x, y + 1, "Diggable", "Back") != null && this.doesTileHaveProperty(x, y + 1, "NoSpawn", "Back") == null && Game1.random.NextDouble() < 0.25)
                this.terrainFeatures.Add(keyValuePair.Key + new Vector2(0.0f, 1f), (TerrainFeature) new Grass((int) (byte) (NetFieldBase<byte, NetByte>) ((Grass) keyValuePair.Value).grassType, Game1.random.Next(1, 3)));
              if (this.isTileOnMap(x, y) && !this.isTileOccupied(keyValuePair.Key + new Vector2(0.0f, -1f)) && this.isTileLocationOpenIgnoreFrontLayers(new Location(x, y - 1)) && this.doesTileHaveProperty(x, y - 1, "Diggable", "Back") != null && this.doesTileHaveProperty(x, y - 1, "NoSpawn", "Back") == null && Game1.random.NextDouble() < 0.25)
                this.terrainFeatures.Add(keyValuePair.Key + new Vector2(0.0f, -1f), (TerrainFeature) new Grass((int) (byte) (NetFieldBase<byte, NetByte>) ((Grass) keyValuePair.Value).grassType, Game1.random.Next(1, 3)));
            }
          }
        }
      }
    }

    public void removeDamageDebris(Monster monster) => this.debris.Filter((Func<Debris, bool>) (d => d.toHover == null || !d.toHover.Equals((object) monster) || d.nonSpriteChunkColor.Equals(Microsoft.Xna.Framework.Color.Yellow) || (double) d.timeSinceDoneBouncing <= 900.0));

    public void spawnWeeds(bool weedsOnly)
    {
      int num1 = Game1.random.Next((bool) (NetFieldBase<bool, NetBool>) this.isFarm ? 5 : 2, (bool) (NetFieldBase<bool, NetBool>) this.isFarm ? 12 : 6);
      if (Game1.dayOfMonth == 1 && Game1.currentSeason.Equals("spring"))
        num1 *= 15;
      if (this.name.Equals((object) "Desert"))
        num1 = Game1.random.NextDouble() < 0.1 ? 1 : 0;
      for (int index = 0; index < num1; ++index)
      {
        int num2 = 0;
        while (num2 < 3)
        {
          int num3 = Game1.random.Next(this.map.DisplayWidth / 64);
          int num4 = Game1.random.Next(this.map.DisplayHeight / 64);
          Vector2 vector2 = new Vector2((float) num3, (float) num4);
          Object @object;
          this.objects.TryGetValue(vector2, out @object);
          int which = -1;
          int num5 = -1;
          if (this.name.Equals((object) "Desert"))
          {
            if (Game1.random.NextDouble() >= 0.5)
              ;
          }
          else if (Game1.random.NextDouble() < 0.15 + (weedsOnly ? 0.05 : 0.0))
            which = 1;
          else if (!weedsOnly && Game1.random.NextDouble() < 0.35)
            num5 = 1;
          else if (!weedsOnly && !(bool) (NetFieldBase<bool, NetBool>) this.isFarm && Game1.random.NextDouble() < 0.35)
            num5 = 2;
          if (num5 != -1)
          {
            if (this is Farm && Game1.random.NextDouble() < 0.25)
              return;
          }
          else if (@object == null && this.doesTileHaveProperty(num3, num4, "Diggable", "Back") != null && this.isTileLocationOpen(new Location(num3, num4)) && !this.isTileOccupied(vector2) && this.doesTileHaveProperty(num3, num4, "Water", "Back") == null)
          {
            switch (this.doesTileHaveProperty(num3, num4, "NoSpawn", "Back"))
            {
              case "Grass":
              case "All":
              case "True":
                continue;
              default:
                if (which != -1 && !this.GetSeasonForLocation().Equals("winter") && this.name.Equals((object) "Farm"))
                {
                  int numberOfWeeds = Game1.random.Next(1, 3);
                  this.terrainFeatures.Add(vector2, (TerrainFeature) new Grass(which, numberOfWeeds));
                  break;
                }
                break;
            }
          }
          ++num2;
        }
      }
    }

    public bool addCharacterAtRandomLocation(NPC n)
    {
      Vector2 tileLocation = new Vector2((float) Game1.random.Next(0, this.map.GetLayer("Back").LayerWidth), (float) Game1.random.Next(0, this.map.GetLayer("Back").LayerHeight));
      int num;
      for (num = 0; num < 6 && (this.isTileOccupied(tileLocation) || !this.isTilePassable(new Location((int) tileLocation.X, (int) tileLocation.Y), Game1.viewport) || this.map.GetLayer("Back").Tiles[(int) tileLocation.X, (int) tileLocation.Y] == null || this.map.GetLayer("Back").Tiles[(int) tileLocation.X, (int) tileLocation.Y].Properties.ContainsKey("NPCBarrier")); ++num)
        tileLocation = new Vector2((float) Game1.random.Next(0, this.map.GetLayer("Back").LayerWidth), (float) Game1.random.Next(0, this.map.GetLayer("Back").LayerHeight));
      if (num >= 6)
        return false;
      n.Position = tileLocation * new Vector2(64f, 64f) - new Vector2(0.0f, (float) (n.Sprite.SpriteHeight - 64));
      this.addCharacter(n);
      return true;
    }

    public virtual void OnMiniJukeboxAdded()
    {
      ++this.miniJukeboxCount.Value;
      this.UpdateMiniJukebox();
    }

    public virtual void OnMiniJukeboxRemoved()
    {
      --this.miniJukeboxCount.Value;
      this.UpdateMiniJukebox();
    }

    public virtual void UpdateMiniJukebox()
    {
      if (this.miniJukeboxCount.Value > 0)
        return;
      this.miniJukeboxCount.Set(0);
      this.miniJukeboxTrack.Set("");
    }

    public virtual bool IsMiniJukeboxPlaying()
    {
      if (this.miniJukeboxCount.Value <= 0 || !(this.miniJukeboxTrack.Value != ""))
        return false;
      return !this.IsOutdoors || !Game1.IsRainingHere(this);
    }

    public virtual void DayUpdate(int dayOfMonth)
    {
      this.netAudio.StopPlaying("fuse");
      this.SelectRandomMiniJukeboxTrack();
      if (this.critters != null)
        this.critters.Clear();
      int index1 = 0;
      while (index1 < this.debris.Count)
      {
        Debris debri = this.debris[index1];
        if (debri.isEssentialItem() && Game1.IsMasterGame)
        {
          if (Utility.IsNormalObjectAtParentSheetIndex(debri.item, 73))
          {
            debri.collect(Game1.player);
          }
          else
          {
            Item obj = debri.item;
            debri.item = (Item) null;
            Game1.player.team.returnedDonations.Add(obj);
            Game1.player.team.newLostAndFoundItems.Value = true;
          }
          this.debris.RemoveAt(index1);
        }
        else
          ++index1;
      }
      this.updateMap();
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
      this.temporarySprites.Clear();
      KeyValuePair<Vector2, TerrainFeature>[] array = this.terrainFeatures.Pairs.ToArray<KeyValuePair<Vector2, TerrainFeature>>();
      foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in array)
      {
        if (!this.isTileOnMap(keyValuePair.Key))
          this.terrainFeatures.Remove(keyValuePair.Key);
        else
          keyValuePair.Value.dayUpdate(this, keyValuePair.Key);
      }
      foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in array)
      {
        if (keyValuePair.Value is HoeDirt hoeDirt)
          hoeDirt.updateNeighbors(this, keyValuePair.Key);
      }
      if (this.largeTerrainFeatures != null)
      {
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures.ToArray<LargeTerrainFeature>())
          largeTerrainFeature.dayUpdate(this);
      }
      List<KeyValuePair<Vector2, Object>> keyValuePairList1 = new List<KeyValuePair<Vector2, Object>>();
      for (int index2 = this.objects.Count() - 1; index2 >= 0; --index2)
      {
        OverlaidDictionary.PairsCollection pairs = this.objects.Pairs;
        KeyValuePair<Vector2, Object> keyValuePair1 = pairs.ElementAt(index2);
        keyValuePair1.Value.DayUpdate(this);
        pairs = this.objects.Pairs;
        keyValuePair1 = pairs.ElementAt(index2);
        if (keyValuePair1.Value.destroyOvernight)
        {
          List<KeyValuePair<Vector2, Object>> keyValuePairList2 = keyValuePairList1;
          pairs = this.objects.Pairs;
          KeyValuePair<Vector2, Object> keyValuePair2 = pairs.ElementAt(index2);
          keyValuePairList2.Add(keyValuePair2);
        }
      }
      foreach (KeyValuePair<Vector2, Object> keyValuePair in keyValuePairList1)
      {
        if (this.objects.ContainsKey(keyValuePair.Key) && this.objects[keyValuePair.Key] == keyValuePair.Value)
        {
          if (keyValuePair.Value != null)
            keyValuePair.Value.performRemoveAction(keyValuePair.Key, this);
          this.objects.Remove(keyValuePair.Key);
        }
      }
      if (!(this is FarmHouse))
        this.debris.Filter((Func<Debris, bool>) (d => d.item != null));
      if (((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors || this.map.Properties.ContainsKey("ForceSpawnForageables")) && !this.map.Properties.ContainsKey("skipWeedGrowth"))
      {
        if (Game1.dayOfMonth % 7 == 0 && !(this is Farm))
        {
          Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0);
          if (this is IslandWest)
            rectangle = new Microsoft.Xna.Framework.Rectangle(31, 3, 77, 66);
          for (int index3 = this.objects.Count() - 1; index3 >= 0; --index3)
          {
            OverlaidDictionary.PairsCollection pairs = this.objects.Pairs;
            KeyValuePair<Vector2, Object> keyValuePair = pairs.ElementAt(index3);
            if ((bool) (NetFieldBase<bool, NetBool>) keyValuePair.Value.isSpawnedObject)
            {
              ref Microsoft.Xna.Framework.Rectangle local = ref rectangle;
              pairs = this.objects.Pairs;
              keyValuePair = pairs.ElementAt(index3);
              Point point = Utility.Vector2ToPoint(keyValuePair.Key);
              if (!local.Contains(point))
              {
                OverlaidDictionary objects = this.objects;
                pairs = this.objects.Pairs;
                keyValuePair = pairs.ElementAt(index3);
                Vector2 key = keyValuePair.Key;
                objects.Remove(key);
              }
            }
          }
          this.numberOfSpawnedObjectsOnMap = 0;
          this.spawnObjects();
          this.spawnObjects();
        }
        this.spawnObjects();
        if (Game1.dayOfMonth == 1)
          this.spawnObjects();
        if (Game1.stats.DaysPlayed < 4U)
          this.spawnObjects();
        bool flag = false;
        foreach (Component layer in this.map.Layers)
        {
          if (layer.Id.Equals("Paths"))
          {
            flag = true;
            break;
          }
        }
        if (flag && !(this is Farm))
        {
          for (int x = 0; x < this.map.Layers[0].LayerWidth; ++x)
          {
            for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
            {
              if (this.map.GetLayer("Paths").Tiles[x, y] != null && Game1.random.NextDouble() < 0.5)
              {
                Vector2 vector2 = new Vector2((float) x, (float) y);
                int which = -1;
                switch (this.map.GetLayer("Paths").Tiles[x, y].TileIndex)
                {
                  case 9:
                    which = 1;
                    if (Game1.currentSeason.Equals("winter"))
                    {
                      which += 3;
                      break;
                    }
                    break;
                  case 10:
                    which = 2;
                    if (Game1.currentSeason.Equals("winter"))
                    {
                      which += 3;
                      break;
                    }
                    break;
                  case 11:
                    which = 3;
                    break;
                  case 12:
                    which = 6;
                    break;
                  case 31:
                    which = 9;
                    break;
                  case 32:
                    which = 8;
                    break;
                }
                if (which != -1 && this.GetFurnitureAt(vector2) == null && !this.terrainFeatures.ContainsKey(vector2) && !this.objects.ContainsKey(vector2))
                  this.terrainFeatures.Add(vector2, (TerrainFeature) new Tree(which, 2));
              }
            }
          }
        }
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isFarm && !this.SeedsIgnoreSeasonsHere())
      {
        ICollection<Vector2> source = (ICollection<Vector2>) new List<Vector2>((IEnumerable<Vector2>) this.terrainFeatures.Keys);
        for (int index4 = source.Count - 1; index4 >= 0; --index4)
        {
          if (this.terrainFeatures[source.ElementAt<Vector2>(index4)] is HoeDirt && ((this.terrainFeatures[source.ElementAt<Vector2>(index4)] as HoeDirt).crop == null || (bool) (NetFieldBase<bool, NetBool>) (this.terrainFeatures[source.ElementAt<Vector2>(index4)] as HoeDirt).crop.forageCrop))
            this.terrainFeatures.Remove(source.ElementAt<Vector2>(index4));
        }
      }
      int num = this.characters.Count - 1;
      while (num >= 0)
        --num;
      this.lightLevel.Value = 0.0f;
      this.name.Equals((object) "BugLand");
      foreach (Furniture furniture in this.furniture)
      {
        furniture.minutesElapsed(Utility.CalculateMinutesUntilMorning(Game1.timeOfDay), this);
        furniture.DayUpdate(this);
      }
      this.addLightGlows();
      if (this is Farm)
        return;
      this.HandleGrassGrowth(dayOfMonth);
    }

    public void addLightGlows()
    {
      int trulyDarkTime = Game1.getTrulyDarkTime();
      if ((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors || Game1.timeOfDay >= trulyDarkTime && !Game1.newDay)
        return;
      this.lightGlows.Clear();
      PropertyValue propertyValue;
      this.map.Properties.TryGetValue("DayTiles", out propertyValue);
      if (propertyValue == null)
        return;
      string[] strArray = propertyValue.ToString().Trim().Split(' ');
      for (int index = 0; index < strArray.Length; index += 4)
      {
        if (this.map.GetLayer(strArray[index]).PickTile(new Location(Convert.ToInt32(strArray[index + 1]) * 64, Convert.ToInt32(strArray[index + 2]) * 64), new Size(Game1.viewport.Width, Game1.viewport.Height)) != null)
        {
          this.map.GetLayer(strArray[index]).Tiles[Convert.ToInt32(strArray[index + 1]), Convert.ToInt32(strArray[index + 2])].TileIndex = Convert.ToInt32(strArray[index + 3]);
          switch (Convert.ToInt32(strArray[index + 3]))
          {
            case 256:
              this.lightGlows.Add(new Vector2((float) Convert.ToInt32(strArray[index + 1]), (float) Convert.ToInt32(strArray[index + 2])) * 64f + new Vector2(32f, 64f));
              continue;
            case 257:
              this.lightGlows.Add(new Vector2((float) Convert.ToInt32(strArray[index + 1]), (float) Convert.ToInt32(strArray[index + 2])) * 64f + new Vector2(32f, -4f));
              continue;
            case 405:
              this.lightGlows.Add(new Vector2((float) Convert.ToInt32(strArray[index + 1]), (float) Convert.ToInt32(strArray[index + 2])) * 64f + new Vector2(32f, 32f));
              this.lightGlows.Add(new Vector2((float) Convert.ToInt32(strArray[index + 1]), (float) Convert.ToInt32(strArray[index + 2])) * 64f + new Vector2(96f, 32f));
              continue;
            case 469:
              this.lightGlows.Add(new Vector2((float) Convert.ToInt32(strArray[index + 1]), (float) Convert.ToInt32(strArray[index + 2])) * 64f + new Vector2(32f, 36f));
              continue;
            case 1224:
              this.lightGlows.Add(new Vector2((float) Convert.ToInt32(strArray[index + 1]), (float) Convert.ToInt32(strArray[index + 2])) * 64f + new Vector2(32f, 32f));
              continue;
            default:
              continue;
          }
        }
      }
    }

    public NPC isCharacterAtTile(Vector2 tileLocation)
    {
      NPC npc = (NPC) null;
      tileLocation.X = (float) (int) tileLocation.X;
      tileLocation.Y = (float) (int) tileLocation.Y;
      if (this.currentEvent == null)
      {
        foreach (NPC character in this.characters)
        {
          if (character.getTileLocation().Equals(tileLocation))
            return character;
        }
      }
      else
      {
        foreach (NPC actor in this.currentEvent.actors)
        {
          if (actor.getTileLocation().Equals(tileLocation))
            return actor;
        }
      }
      return npc;
    }

    public void ResetCharacterDialogues()
    {
      for (int index = this.characters.Count - 1; index >= 0; --index)
        this.characters[index].resetCurrentDialogue();
    }

    public string getMapProperty(string propertyName)
    {
      PropertyValue propertyValue = (PropertyValue) null;
      this.Map.Properties.TryGetValue(propertyName, out propertyValue);
      return propertyValue == null ? "" : propertyValue.ToString();
    }

    public virtual void tryToAddCritters(bool onlyIfOnScreen = false)
    {
      if (Game1.CurrentEvent != null)
        return;
      double num1;
      double chance1 = num1 = Math.Max(0.15, Math.Min(0.5, (double) (this.map.Layers[0].LayerWidth * this.map.Layers[0].LayerHeight) / 15000.0));
      double chance2 = num1;
      double chance3 = num1 / 2.0;
      double chance4 = num1 / 2.0;
      double chance5 = num1 / 8.0;
      double num2 = num1 * 2.0;
      if (Game1.IsRainingHere(this))
        return;
      this.addClouds(num2 / (onlyIfOnScreen ? 2.0 : 1.0), onlyIfOnScreen);
      if (this is Beach || this.critters == null || this.critters.Count > (Game1.currentSeason.Equals("summer") ? 20 : 10))
        return;
      this.addBirdies(chance1, onlyIfOnScreen);
      this.addButterflies(chance2, onlyIfOnScreen);
      this.addBunnies(chance3, onlyIfOnScreen);
      this.addSquirrels(chance4, onlyIfOnScreen);
      this.addWoodpecker(chance5, onlyIfOnScreen);
      if (!Game1.isDarkOut() || Game1.random.NextDouble() >= 0.01)
        return;
      this.addOwl();
    }

    public void addClouds(double chance, bool onlyIfOnScreen = false)
    {
      if (!Game1.currentSeason.Equals("summer") || Game1.IsRainingHere(this) || Game1.weatherIcon == 4 || Game1.timeOfDay >= Game1.getStartingToGetDarkTime() - 100)
        return;
      while (Game1.random.NextDouble() < Math.Min(0.9, chance))
      {
        Vector2 position = this.getRandomTile();
        if (onlyIfOnScreen)
          position = Game1.random.NextDouble() < 0.5 ? new Vector2((float) this.map.Layers[0].LayerWidth, (float) Game1.random.Next(this.map.Layers[0].LayerHeight)) : new Vector2((float) Game1.random.Next(this.map.Layers[0].LayerWidth), (float) this.map.Layers[0].LayerHeight);
        if (onlyIfOnScreen || !Utility.isOnScreen(position * 64f, 1280))
        {
          Cloud c = new Cloud(position);
          bool flag = true;
          if (this.critters != null)
          {
            foreach (Critter critter in this.critters)
            {
              if (critter is Cloud && critter.getBoundingBox(0, 0).Intersects(c.getBoundingBox(0, 0)))
              {
                flag = false;
                break;
              }
            }
          }
          if (flag)
            this.addCritter((Critter) c);
        }
      }
    }

    public void addOwl() => this.critters.Add((Critter) new Owl(new Vector2((float) Game1.random.Next(64, this.map.Layers[0].LayerWidth * 64 - 64), (float) sbyte.MinValue)));

    public void setFireplace(bool on, int tileLocationX, int tileLocationY, bool playSound = true)
    {
      int num = 944468 + tileLocationX * 1000 + tileLocationY;
      if (on)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2((float) tileLocationX, (float) tileLocationY) * 64f + new Vector2(32f, -32f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
        {
          interval = 50f,
          totalNumberOfLoops = 99999,
          animationLength = 4,
          light = true,
          lightID = num,
          id = (float) num,
          lightRadius = 2f,
          scale = 4f,
          layerDepth = (float) (((double) tileLocationY + 1.10000002384186) * 64.0 / 10000.0)
        });
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2((float) (tileLocationX + 1), (float) tileLocationY) * 64f + new Vector2(-16f, -32f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
        {
          delayBeforeAnimationStart = 10,
          interval = 50f,
          totalNumberOfLoops = 99999,
          animationLength = 4,
          light = true,
          lightID = num,
          id = (float) num,
          lightRadius = 2f,
          scale = 4f,
          layerDepth = (float) (((double) tileLocationY + 1.10000002384186) * 64.0 / 10000.0)
        });
        if (playSound && Game1.gameMode != (byte) 6)
          this.localSound("fireball");
        AmbientLocationSounds.addSound(new Vector2((float) tileLocationX, (float) tileLocationY), 1);
      }
      else
      {
        this.removeTemporarySpritesWithID(num);
        Utility.removeLightSource(num);
        if (playSound)
          this.localSound("fireball");
        AmbientLocationSounds.removeSound(new Vector2((float) tileLocationX, (float) tileLocationY));
      }
    }

    public void addWoodpecker(double chance, bool onlyIfOnScreen = false)
    {
      if (Game1.isStartingToGetDarkOut() || onlyIfOnScreen || this is Town || this is Desert || Game1.random.NextDouble() >= chance)
        return;
      for (int index1 = 0; index1 < 3; ++index1)
      {
        int index2 = Game1.random.Next(this.terrainFeatures.Count());
        if (this.terrainFeatures.Count() > 0)
        {
          NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.PairsCollection pairs = this.terrainFeatures.Pairs;
          if (pairs.ElementAt(index2).Value is Tree)
          {
            pairs = this.terrainFeatures.Pairs;
            if ((int) (NetFieldBase<int, NetInt>) (pairs.ElementAt(index2).Value as Tree).treeType != 2)
            {
              pairs = this.terrainFeatures.Pairs;
              if ((int) (NetFieldBase<int, NetInt>) (pairs.ElementAt(index2).Value as Tree).growthStage >= 5)
              {
                List<Critter> critters = this.critters;
                pairs = this.terrainFeatures.Pairs;
                KeyValuePair<Vector2, TerrainFeature> keyValuePair = pairs.ElementAt(index2);
                Tree tree = keyValuePair.Value as Tree;
                pairs = this.terrainFeatures.Pairs;
                keyValuePair = pairs.ElementAt(index2);
                Vector2 key = keyValuePair.Key;
                Woodpecker woodpecker = new Woodpecker(tree, key);
                critters.Add((Critter) woodpecker);
                break;
              }
            }
          }
        }
      }
    }

    public void addSquirrels(double chance, bool onlyIfOnScreen = false)
    {
      if (Game1.isStartingToGetDarkOut() || onlyIfOnScreen)
        return;
      switch (this)
      {
        case Farm _:
          break;
        case Town _:
          break;
        case Desert _:
          break;
        default:
          if (Game1.random.NextDouble() >= chance)
            break;
          for (int index1 = 0; index1 < 3; ++index1)
          {
            int index2 = Game1.random.Next(this.terrainFeatures.Count());
            if (this.terrainFeatures.Count() > 0)
            {
              NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.PairsCollection pairs = this.terrainFeatures.Pairs;
              if (pairs.ElementAt(index2).Value is Tree)
              {
                pairs = this.terrainFeatures.Pairs;
                if ((int) (NetFieldBase<int, NetInt>) (pairs.ElementAt(index2).Value as Tree).growthStage >= 5)
                {
                  pairs = this.terrainFeatures.Pairs;
                  if (!(bool) (NetFieldBase<bool, NetBool>) (pairs.ElementAt(index2).Value as Tree).stump)
                  {
                    pairs = this.terrainFeatures.Pairs;
                    Vector2 key = pairs.ElementAt(index2).Key;
                    int num = Game1.random.Next(4, 7);
                    bool flip = Game1.random.NextDouble() < 0.5;
                    bool flag = true;
                    for (int index3 = 0; index3 < num; ++index3)
                    {
                      key.X += flip ? 1f : -1f;
                      if (!this.isTileLocationTotallyClearAndPlaceable(key))
                      {
                        flag = false;
                        break;
                      }
                    }
                    if (flag)
                    {
                      this.critters.Add((Critter) new Squirrel(key, flip));
                      break;
                    }
                  }
                }
              }
            }
          }
          break;
      }
    }

    public void addBunnies(double chance, bool onlyIfOnScreen = false)
    {
      if (onlyIfOnScreen || this is Farm || this is Desert || Game1.random.NextDouble() >= chance || this.largeTerrainFeatures == null)
        return;
      for (int index1 = 0; index1 < 3; ++index1)
      {
        int index2 = Game1.random.Next(this.largeTerrainFeatures.Count);
        if (this.largeTerrainFeatures.Count > 0 && this.largeTerrainFeatures[index2] is StardewValley.TerrainFeatures.Bush)
        {
          Vector2 tilePosition = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.largeTerrainFeatures[index2].tilePosition;
          int num = Game1.random.Next(5, 12);
          bool flip = Game1.random.NextDouble() < 0.5;
          bool flag = true;
          for (int index3 = 0; index3 < num; ++index3)
          {
            tilePosition.X += flip ? 1f : -1f;
            if (!this.largeTerrainFeatures[index2].getBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle((int) tilePosition.X * 64, (int) tilePosition.Y * 64, 64, 64)) && !this.isTileLocationTotallyClearAndPlaceable(tilePosition))
            {
              flag = false;
              break;
            }
          }
          if (flag)
          {
            this.critters.Add((Critter) new Rabbit(tilePosition, flip));
            break;
          }
        }
      }
    }

    public void instantiateCrittersList()
    {
      if (this.critters != null)
        return;
      this.critters = new List<Critter>();
    }

    public void addCritter(Critter c)
    {
      if (this.critters == null)
        return;
      this.critters.Add(c);
    }

    public void addButterflies(double chance, bool onlyIfOnScreen = false)
    {
      bool islandButterfly = this.GetLocationContext() == GameLocation.LocationContext.Island;
      bool flag = Game1.currentSeason.Equals("summer") | islandButterfly && Game1.isDarkOut();
      if (Game1.timeOfDay >= 1500 && !flag || ((Game1.currentSeason.Equals("spring") ? 1 : (Game1.currentSeason.Equals("summer") ? 1 : 0)) | (islandButterfly ? 1 : 0)) == 0)
        return;
      chance = Math.Min(0.8, chance * 1.5);
      while (Game1.random.NextDouble() < chance)
      {
        Vector2 randomTile = this.getRandomTile();
        if (!onlyIfOnScreen || !Utility.isOnScreen(randomTile * 64f, 64))
        {
          if (flag)
            this.critters.Add((Critter) new Firefly(randomTile));
          else
            this.critters.Add((Critter) new Butterfly(randomTile, islandButterfly));
          while (Game1.random.NextDouble() < 0.4)
          {
            if (flag)
              this.critters.Add((Critter) new Firefly(randomTile + new Vector2((float) Game1.random.Next(-2, 3), (float) Game1.random.Next(-2, 3))));
            else
              this.critters.Add((Critter) new Butterfly(randomTile + new Vector2((float) Game1.random.Next(-2, 3), (float) Game1.random.Next(-2, 3)), islandButterfly));
          }
        }
      }
    }

    public void addBirdies(double chance, bool onlyIfOnScreen = false)
    {
      if (Game1.timeOfDay >= 1500)
        return;
      switch (this)
      {
        case Desert _:
          break;
        case Railroad _:
          break;
        case Farm _:
          break;
        default:
          if (Game1.currentSeason.Equals("summer"))
            break;
label_16:
          while (Game1.random.NextDouble() < chance)
          {
            int num1 = Game1.random.Next(1, 4);
            bool flag = false;
            int num2 = 0;
            while (true)
            {
              if (!flag && num2 < 5)
              {
                Vector2 randomTile = this.getRandomTile();
                if ((!onlyIfOnScreen || !Utility.isOnScreen(randomTile * 64f, 64)) && this.isAreaClear(new Microsoft.Xna.Framework.Rectangle((int) randomTile.X - 2, (int) randomTile.Y - 2, 5, 5)))
                {
                  List<Critter> crittersToAdd = new List<Critter>();
                  int startingIndex = Game1.currentSeason.Equals("fall") ? 45 : 25;
                  if (Game1.random.NextDouble() < 0.5 && Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
                    startingIndex = Game1.currentSeason.Equals("fall") ? 135 : 125;
                  for (int index = 0; index < num1; ++index)
                    crittersToAdd.Add((Critter) new Birdie(-100, -100, startingIndex));
                  this.addCrittersStartingAtTile(randomTile, crittersToAdd);
                  flag = true;
                }
                ++num2;
              }
              else
                goto label_16;
            }
          }
          break;
      }
    }

    public void addJumperFrog(Vector2 tileLocation)
    {
      if (this.critters == null)
        return;
      this.critters.Add((Critter) new Frog(tileLocation));
    }

    public void addFrog()
    {
      if (!Game1.IsRainingHere(this) || Game1.currentSeason.Equals("winter"))
        return;
      for (int index1 = 0; index1 < 3; ++index1)
      {
        Vector2 randomTile = this.getRandomTile();
        if (this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "Water", "Back") != null && this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y - 1, "Water", "Back") != null && this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "Passable", "Buildings") == null)
        {
          int num = 10;
          bool forceFlip = Game1.random.NextDouble() < 0.5;
          for (int index2 = 0; index2 < num; ++index2)
          {
            randomTile.X += forceFlip ? 1f : -1f;
            if (this.isTileOnMap((int) randomTile.X, (int) randomTile.Y) && this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "Water", "Back") == null)
            {
              this.critters.Add((Critter) new Frog(randomTile, true, forceFlip));
              return;
            }
          }
        }
      }
    }

    public void checkForSpecialCharacterIconAtThisTile(Vector2 tileLocation)
    {
      if (this.currentEvent == null)
        return;
      this.currentEvent.checkForSpecialCharacterIconAtThisTile(tileLocation);
    }

    private void addCrittersStartingAtTile(Vector2 tile, List<Critter> crittersToAdd)
    {
      if (crittersToAdd == null)
        return;
      int num = 0;
      HashSet<Vector2> vector2Set = new HashSet<Vector2>();
      for (; crittersToAdd.Count > 0 && num < 20; ++num)
      {
        if (vector2Set.Contains(tile))
        {
          tile = Utility.getTranslatedVector2(tile, Game1.random.Next(4), 1f);
        }
        else
        {
          if (this.isTileLocationTotallyClearAndPlaceable(tile))
          {
            crittersToAdd.Last<Critter>().position = tile * 64f;
            crittersToAdd.Last<Critter>().startingPosition = tile * 64f;
            this.critters.Add(crittersToAdd.Last<Critter>());
            crittersToAdd.RemoveAt(crittersToAdd.Count - 1);
          }
          tile = Utility.getTranslatedVector2(tile, Game1.random.Next(4), 1f);
          vector2Set.Add(tile);
        }
      }
    }

    public bool isAreaClear(Microsoft.Xna.Framework.Rectangle area)
    {
      for (int left = area.Left; left < area.Right; ++left)
      {
        for (int top = area.Top; top < area.Bottom; ++top)
        {
          if (!this.isTileLocationTotallyClearAndPlaceable(new Vector2((float) left, (float) top)))
            return false;
        }
      }
      return true;
    }

    public void refurbishMapPortion(
      Microsoft.Xna.Framework.Rectangle areaToRefurbish,
      string refurbishedMapName,
      Point mapReaderStartPoint)
    {
      Map map = Game1.game1.xTileContent.Load<Map>("Maps\\" + refurbishedMapName);
      Point point = mapReaderStartPoint;
      ((IDictionary<string, PropertyValue>) this.map.Properties).Remove("DayTiles");
      ((IDictionary<string, PropertyValue>) this.map.Properties).Remove("NightTiles");
      for (int index1 = 0; index1 < areaToRefurbish.Width; ++index1)
      {
        for (int index2 = 0; index2 < areaToRefurbish.Height; ++index2)
        {
          if (map.GetLayer("Back").Tiles[point.X + index1, point.Y + index2] != null)
          {
            this.map.GetLayer("Back").Tiles[areaToRefurbish.X + index1, areaToRefurbish.Y + index2] = (Tile) new StaticTile(this.map.GetLayer("Back"), this.map.TileSheets[0], BlendMode.Alpha, map.GetLayer("Back").Tiles[point.X + index1, point.Y + index2].TileIndex);
            foreach (string key in (IEnumerable<string>) map.GetLayer("Back").Tiles[point.X + index1, point.Y + index2].Properties.Keys)
              this.map.GetLayer("Back").Tiles[areaToRefurbish.X + index1, areaToRefurbish.Y + index2].Properties.Add(key, map.GetLayer("Back").Tiles[point.X + index1, point.Y + index2].Properties[key]);
          }
          if (map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2] != null)
          {
            this.map.GetLayer("Buildings").Tiles[areaToRefurbish.X + index1, areaToRefurbish.Y + index2] = (Tile) new StaticTile(this.map.GetLayer("Buildings"), this.map.TileSheets[0], BlendMode.Alpha, map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2].TileIndex);
            this.adjustMapLightPropertiesForLamp(map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2].TileIndex, areaToRefurbish.X + index1, areaToRefurbish.Y + index2, "Buildings");
            foreach (string key in (IEnumerable<string>) map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2].Properties.Keys)
              this.map.GetLayer("Buildings").Tiles[areaToRefurbish.X + index1, areaToRefurbish.Y + index2].Properties.Add(key, map.GetLayer("Back").Tiles[point.X + index1, point.Y + index2].Properties[key]);
          }
          else
            this.map.GetLayer("Buildings").Tiles[areaToRefurbish.X + index1, areaToRefurbish.Y + index2] = (Tile) null;
          if (index2 < areaToRefurbish.Height - 1 && map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2] != null)
          {
            this.map.GetLayer("Front").Tiles[areaToRefurbish.X + index1, areaToRefurbish.Y + index2] = (Tile) new StaticTile(this.map.GetLayer("Front"), this.map.TileSheets[0], BlendMode.Alpha, map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2].TileIndex);
            this.adjustMapLightPropertiesForLamp(map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2].TileIndex, areaToRefurbish.X + index1, areaToRefurbish.Y + index2, "Front");
          }
          else if (index2 < areaToRefurbish.Height - 1)
            this.map.GetLayer("Front").Tiles[areaToRefurbish.X + index1, areaToRefurbish.Y + index2] = (Tile) null;
        }
      }
    }

    public Vector2 getRandomTile() => new Vector2((float) Game1.random.Next(this.Map.Layers[0].LayerWidth), (float) Game1.random.Next(this.Map.Layers[0].LayerHeight));

    public void setUpLocationSpecificFlair()
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isOutdoors)
      {
        switch (this)
        {
          case FarmHouse _:
          case IslandFarmHouse _:
            break;
          default:
            PropertyValue propertyValue;
            this.map.Properties.TryGetValue("AmbientLight", out propertyValue);
            if (propertyValue == null)
            {
              Game1.ambientLight = new Microsoft.Xna.Framework.Color(100, 120, 30);
              break;
            }
            break;
        }
      }
      string name = (string) (NetFieldBase<string, NetString>) this.name;
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
      {
        case 10819997:
          if (!(name == "LeoTreeHouse"))
            break;
          List<TemporaryAnimatedSprite> temporarySprites = this.temporarySprites;
          EmilysParrot emilysParrot = new EmilysParrot(new Vector2(88f, 224f));
          emilysParrot.layerDepth = 1f;
          emilysParrot.id = 5858585f;
          temporarySprites.Add((TemporaryAnimatedSprite) emilysParrot);
          this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(71, 334, 12, 11), new Vector2(304f, 32f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            layerDepth = 1f / 1000f,
            interval = 700f,
            animationLength = 3,
            totalNumberOfLoops = 999999,
            scale = 4f
          });
          this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(47, 334, 12, 11), new Vector2(112f, -25.6f), true, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            layerDepth = 1f / 1000f,
            interval = 300f,
            animationLength = 3,
            totalNumberOfLoops = 999999,
            scale = 4f
          });
          this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(71, 334, 12, 11), new Vector2(224f, -25.6f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            layerDepth = 1f / 1000f,
            interval = 800f,
            animationLength = 3,
            totalNumberOfLoops = 999999,
            scale = 4f
          });
          Game1.changeMusicTrack("sad_kid", true, Game1.MusicContext.SubLocation);
          break;
        case 524243468:
          if (!(name == "BugLand"))
            break;
          if (!Game1.player.hasDarkTalisman && this.isTileLocationTotallyClearAndPlaceable(31, 5))
            this.overlayObjects.Add(new Vector2(31f, 5f), (Object) new Chest(0, new List<Item>()
            {
              (Item) new SpecialItem(6)
            }, new Vector2(31f, 5f))
            {
              Tint = Microsoft.Xna.Framework.Color.Gray
            });
          using (List<NPC>.Enumerator enumerator = this.characters.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              NPC current = enumerator.Current;
              if (current is Grub)
                (current as Grub).setHard();
              else if (current is Fly)
                (current as Fly).setHard();
            }
            break;
          }
        case 636013712:
          if (!(name == "HaleyHouse") || !Game1.player.eventsSeen.Contains(463391) || Game1.player.spouse != null && Game1.player.spouse.Equals("Emily"))
            break;
          this.temporarySprites.Add((TemporaryAnimatedSprite) new EmilysParrot(new Vector2(912f, 160f)));
          break;
        case 720888915:
          if (!(name == "JojaMart"))
            break;
          Game1.changeMusicTrack("Hospital_Ambient");
          Game1.ambientLight = new Microsoft.Xna.Framework.Color(0, 0, 0);
          if (Game1.random.NextDouble() >= 0.5)
            break;
          NPC characterFromName1 = Game1.getCharacterFromName("Morris");
          if (characterFromName1 == null || characterFromName1.currentLocation != this)
            break;
          string path1 = "Strings\\SpeechBubbles:JojaMart_Morris_Greeting";
          characterFromName1.showTextAboveHead(Game1.content.LoadString(path1));
          break;
        case 746089795:
          if (!(name == "ScienceHouse"))
            break;
          if (Game1.random.NextDouble() < 0.5 && Game1.player.currentLocation != null && (bool) (NetFieldBase<bool, NetBool>) Game1.player.currentLocation.isOutdoors)
          {
            NPC characterFromName2 = Game1.getCharacterFromName("Robin");
            if (characterFromName2 != null && characterFromName2.getTileY() == 18)
            {
              string path2 = "";
              switch (Game1.random.Next(4))
              {
                case 0:
                  path2 = Game1.isRaining ? "Strings\\SpeechBubbles:ScienceHouse_Robin_Raining1" : "Strings\\SpeechBubbles:ScienceHouse_Robin_NotRaining1";
                  break;
                case 1:
                  path2 = Game1.isSnowing ? "Strings\\SpeechBubbles:ScienceHouse_Robin_Snowing" : "Strings\\SpeechBubbles:ScienceHouse_Robin_NotSnowing";
                  break;
                case 2:
                  path2 = Game1.player.getFriendshipHeartLevelForNPC("Robin") > 4 ? "Strings\\SpeechBubbles:ScienceHouse_Robin_CloseFriends" : "Strings\\SpeechBubbles:ScienceHouse_Robin_NotCloseFriends";
                  break;
                case 3:
                  path2 = Game1.isRaining ? "Strings\\SpeechBubbles:ScienceHouse_Robin_Raining2" : "Strings\\SpeechBubbles:ScienceHouse_Robin_NotRaining2";
                  break;
                case 4:
                  path2 = "Strings\\SpeechBubbles:ScienceHouse_Robin_Greeting";
                  break;
              }
              if (Game1.random.NextDouble() < 0.001)
                path2 = "Strings\\SpeechBubbles:ScienceHouse_Robin_RareGreeting";
              characterFromName2.showTextAboveHead(Game1.content.LoadString(path2, (object) Game1.player.Name));
            }
          }
          if (this.getCharacterFromName("Robin") != null || !Game1.IsVisitingIslandToday("Robin"))
            break;
          this.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.mouseCursors2,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(129, 210, 13, 16),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(129f, 210f),
            interval = 50000f,
            totalNumberOfLoops = 9999,
            position = new Vector2(7f, 18f) * 64f + new Vector2(3f, 0.0f) * 4f,
            scale = 4f,
            layerDepth = 0.1281f,
            id = 777f
          });
          break;
        case 807500499:
          if (!(name == "Hospital"))
            break;
          if (!Game1.isRaining)
            Game1.changeMusicTrack("distantBanjo");
          Game1.ambientLight = new Microsoft.Xna.Framework.Color(100, 100, 60);
          if (Game1.random.NextDouble() >= 0.5)
            break;
          NPC characterFromName3 = Game1.getCharacterFromName("Maru");
          if (characterFromName3 == null || characterFromName3.currentLocation != this || characterFromName3.isDivorcedFrom(Game1.player))
            break;
          string path3 = "";
          switch (Game1.random.Next(5))
          {
            case 0:
              path3 = "Strings\\SpeechBubbles:Hospital_Maru_Greeting1";
              break;
            case 1:
              path3 = "Strings\\SpeechBubbles:Hospital_Maru_Greeting2";
              break;
            case 2:
              path3 = "Strings\\SpeechBubbles:Hospital_Maru_Greeting3";
              break;
            case 3:
              path3 = "Strings\\SpeechBubbles:Hospital_Maru_Greeting4";
              break;
            case 4:
              path3 = "Strings\\SpeechBubbles:Hospital_Maru_Greeting5";
              break;
          }
          if (Game1.player.spouse != null && Game1.player.spouse.Equals("Maru"))
          {
            string path4 = "Strings\\SpeechBubbles:Hospital_Maru_Spouse";
            characterFromName3.showTextAboveHead(Game1.content.LoadString(path4), 2);
            break;
          }
          characterFromName3.showTextAboveHead(Game1.content.LoadString(path3));
          break;
        case 820770722:
          if (!(name == "Summit"))
            break;
          Game1.ambientLight = Microsoft.Xna.Framework.Color.Black;
          break;
        case 1167876998:
          if (!(name == "ManorHouse"))
            break;
          Game1.ambientLight = new Microsoft.Xna.Framework.Color(150, 120, 50);
          NPC characterFromName4 = Game1.getCharacterFromName("Lewis");
          if (characterFromName4 == null || characterFromName4.currentLocation != this)
            break;
          string str1 = Game1.timeOfDay < 1200 ? "Morning" : (Game1.timeOfDay < 1700 ? "Afternoon" : "Evening");
          characterFromName4.faceTowardFarmerForPeriod(3000, 15, false, Game1.player);
          characterFromName4.showTextAboveHead(Game1.content.LoadString("Strings\\SpeechBubbles:ManorHouse_Lewis_" + str1));
          break;
        case 1367472567:
          if (!(name == "Blacksmith"))
            break;
          AmbientLocationSounds.addSound(new Vector2(9f, 10f), 2);
          AmbientLocationSounds.changeSpecificVariable("Frequency", 2f, 2);
          Game1.changeMusicTrack("none");
          break;
        case 1428365440:
          if (!(name == "SeedShop"))
            break;
          this.setFireplace(true, 25, 13, false);
          if (Game1.random.NextDouble() < 0.5 && Game1.player.getTileY() > 10)
          {
            NPC characterFromName5 = Game1.getCharacterFromName("Pierre");
            if (characterFromName5 != null && characterFromName5.getTileY() == 17 && characterFromName5.currentLocation == this)
            {
              string str2 = "";
              switch (Game1.random.Next(5))
              {
                case 0:
                  str2 = Game1.IsWinter ? "Winter" : "NotWinter";
                  break;
                case 1:
                  str2 = Game1.IsSummer ? "Summer" : "NotSummer";
                  break;
                case 2:
                  str2 = "Greeting1";
                  break;
                case 3:
                  str2 = "Greeting2";
                  break;
                case 4:
                  str2 = Game1.isRaining ? "Raining" : "NotRaining";
                  break;
              }
              if (Game1.random.NextDouble() < 0.001)
                str2 = "RareGreeting";
              string format = Game1.content.LoadString("Strings\\SpeechBubbles:SeedShop_Pierre_" + str2);
              if (format.Contains('^'))
                format = !Game1.player.IsMale ? format.Split('^')[1] : format.Split('^')[0];
              characterFromName5.showTextAboveHead(string.Format(format, (object) Game1.player.Name));
            }
          }
          if (this.getCharacterFromName("Pierre") != null || !Game1.IsVisitingIslandToday("Pierre"))
            break;
          this.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.mouseCursors2,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(129, 210, 13, 16),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(129f, 210f),
            interval = 50000f,
            totalNumberOfLoops = 9999,
            position = new Vector2(5f, 17f) * 64f + new Vector2(3f, 0.0f) * 4f,
            scale = 4f,
            layerDepth = 0.1217f,
            id = 777f
          });
          break;
        case 1446049731:
          if (!(name == "CommunityCenter") || !(this is CommunityCenter) || !Game1.isLocationAccessible("CommunityCenter") && (this.currentEvent == null || this.currentEvent.id != 191393))
            break;
          this.setFireplace(true, 31, 8, false);
          this.setFireplace(true, 32, 8, false);
          this.setFireplace(true, 33, 8, false);
          break;
        case 1695005214:
          if (!(name == "Sunroom"))
            break;
          Game1.ambientLight = new Microsoft.Xna.Framework.Color(0, 0, 0);
          AmbientLocationSounds.addSound(new Vector2(3f, 4f), 0);
          if (this.largeTerrainFeatures.Count == 0)
          {
            StardewValley.TerrainFeatures.Bush bush = new StardewValley.TerrainFeatures.Bush(new Vector2(6f, 7f), 3, this, -999);
            bush.greenhouseBush.Value = true;
            bush.loadSprite();
            bush.health = 99f;
            this.largeTerrainFeatures.Add((LargeTerrainFeature) bush);
          }
          if (Game1.IsRainingHere(this))
            break;
          Game1.changeMusicTrack("SunRoom", music_context: Game1.MusicContext.SubLocation);
          this.critters = new List<Critter>();
          this.critters.Add((Critter) new Butterfly(this.getRandomTile()).setStayInbounds(true));
          while (Game1.random.NextDouble() < 0.5)
            this.critters.Add((Critter) new Butterfly(this.getRandomTile()).setStayInbounds(true));
          break;
        case 1840909614:
          if (!(name == "SandyHouse"))
            break;
          Game1.changeMusicTrack("distantBanjo");
          Game1.ambientLight = new Microsoft.Xna.Framework.Color(0, 0, 0);
          if (Game1.random.NextDouble() >= 0.5)
            break;
          NPC characterFromName6 = Game1.getCharacterFromName("Sandy");
          if (characterFromName6 == null || characterFromName6.currentLocation != this)
            break;
          string path5 = "";
          switch (Game1.random.Next(5))
          {
            case 0:
              path5 = "Strings\\SpeechBubbles:SandyHouse_Sandy_Greeting1";
              break;
            case 1:
              path5 = "Strings\\SpeechBubbles:SandyHouse_Sandy_Greeting2";
              break;
            case 2:
              path5 = "Strings\\SpeechBubbles:SandyHouse_Sandy_Greeting3";
              break;
            case 3:
              path5 = "Strings\\SpeechBubbles:SandyHouse_Sandy_Greeting4";
              break;
            case 4:
              path5 = "Strings\\SpeechBubbles:SandyHouse_Sandy_Greeting5";
              break;
          }
          characterFromName6.showTextAboveHead(Game1.content.LoadString(path5));
          break;
        case 1919215024:
          if (!(name == "ElliottHouse"))
            break;
          Game1.changeMusicTrack("communityCenter");
          NPC characterFromName7 = Game1.getCharacterFromName("Elliott");
          if (characterFromName7 == null || characterFromName7.currentLocation != this || characterFromName7.isDivorcedFrom(Game1.player))
            break;
          string path6 = "";
          switch (Game1.random.Next(3))
          {
            case 0:
              path6 = "Strings\\SpeechBubbles:ElliottHouse_Elliott_Greeting1";
              break;
            case 1:
              path6 = "Strings\\SpeechBubbles:ElliottHouse_Elliott_Greeting2";
              break;
            case 2:
              path6 = "Strings\\SpeechBubbles:ElliottHouse_Elliott_Greeting3";
              break;
          }
          characterFromName7.faceTowardFarmerForPeriod(3000, 15, false, Game1.player);
          characterFromName7.showTextAboveHead(Game1.content.LoadString(path6, (object) Game1.player.Name));
          break;
        case 2233558176:
          if (!(name == "Greenhouse") || !Game1.isDarkOut())
            break;
          Game1.ambientLight = Game1.outdoorLight;
          break;
        case 2680503661:
          if (!(name == "AbandonedJojaMart") || Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
            break;
          Point point = new Point(8, 8);
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) (point.X * 64), (float) (point.Y * 64)), 1f));
          this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (point.X * 64), (float) (point.Y * 64)), Microsoft.Xna.Framework.Color.White)
          {
            layerDepth = 1f,
            interval = 50f,
            motion = new Vector2(1f, 0.0f),
            acceleration = new Vector2(-0.005f, 0.0f)
          });
          this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (point.X * 64 - 12), (float) (point.Y * 64 - 12)), Microsoft.Xna.Framework.Color.White)
          {
            scale = 0.75f,
            layerDepth = 1f,
            interval = 50f,
            motion = new Vector2(1f, 0.0f),
            acceleration = new Vector2(-0.005f, 0.0f),
            delayBeforeAnimationStart = 50
          });
          this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (point.X * 64 - 12), (float) (point.Y * 64 + 12)), Microsoft.Xna.Framework.Color.White)
          {
            layerDepth = 1f,
            interval = 50f,
            motion = new Vector2(1f, 0.0f),
            acceleration = new Vector2(-0.005f, 0.0f),
            delayBeforeAnimationStart = 100
          });
          this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (point.X * 64), (float) (point.Y * 64)), Microsoft.Xna.Framework.Color.White)
          {
            layerDepth = 1f,
            scale = 0.75f,
            interval = 50f,
            motion = new Vector2(1f, 0.0f),
            acceleration = new Vector2(-0.005f, 0.0f),
            delayBeforeAnimationStart = 150
          });
          if (this.characters.Count != 0)
            break;
          this.characters.Add((NPC) new Junimo(new Vector2(8f, 7f) * 64f, 6));
          break;
        case 2708986271:
          if (!(name == "ArchaeologyHouse"))
            break;
          this.setFireplace(true, 43, 4, false);
          if (Game1.random.NextDouble() >= 0.5 || !Game1.player.hasOrWillReceiveMail("artifactFound"))
            break;
          NPC characterFromName8 = Game1.getCharacterFromName("Gunther");
          if (characterFromName8 == null || characterFromName8.currentLocation != this)
            break;
          string str3 = "";
          switch (Game1.random.Next(5))
          {
            case 0:
              str3 = "Greeting1";
              break;
            case 1:
              str3 = "Greeting2";
              break;
            case 2:
              str3 = "Greeting3";
              break;
            case 3:
              str3 = "Greeting4";
              break;
            case 4:
              str3 = "Greeting5";
              break;
          }
          if (Game1.random.NextDouble() < 0.001)
            str3 = "RareGreeting";
          characterFromName8.showTextAboveHead(Game1.content.LoadString("Strings\\SpeechBubbles:ArchaeologyHouse_Gunther_" + str3));
          break;
        case 2880093601:
          if (!(name == "QiNutRoom"))
            break;
          Game1.changeMusicTrack("clubloop", music_context: Game1.MusicContext.SubLocation);
          Game1.ambientLight = new Microsoft.Xna.Framework.Color(100, 120, 30);
          break;
        case 2909376585:
          if (!(name == "Saloon"))
            break;
          if (Game1.timeOfDay >= 1700)
          {
            this.setFireplace(true, 22, 17, false);
            Game1.changeMusicTrack("Saloon1");
          }
          if (Game1.random.NextDouble() < 0.25)
          {
            NPC characterFromName9 = Game1.getCharacterFromName("Gus");
            if (characterFromName9 != null && characterFromName9.getTileY() == 18 && characterFromName9.currentLocation == this)
            {
              string str4 = "";
              switch (Game1.random.Next(5))
              {
                case 0:
                  str4 = "Greeting";
                  break;
                case 1:
                  str4 = Game1.IsSummer ? "Summer" : "NotSummer";
                  break;
                case 2:
                  str4 = Game1.isSnowing ? "Snowing1" : "NotSnowing1";
                  break;
                case 3:
                  str4 = Game1.isRaining ? "Raining" : "NotRaining";
                  break;
                case 4:
                  str4 = Game1.isSnowing ? "Snowing2" : "NotSnowing2";
                  break;
              }
              if (Game1.random.NextDouble() < 0.001)
                str4 = "RareGreeting";
              characterFromName9.showTextAboveHead(Game1.content.LoadString("Strings\\SpeechBubbles:Saloon_Gus_" + str4));
            }
          }
          if (this.getCharacterFromName("Gus") == null && Game1.IsVisitingIslandToday("Gus"))
            this.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.mouseCursors2,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(129, 210, 13, 16),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(129f, 210f),
              interval = 50000f,
              totalNumberOfLoops = 9999,
              position = new Vector2(11f, 18f) * 64f + new Vector2(3f, 0.0f) * 4f,
              scale = 4f,
              layerDepth = 0.1281f,
              id = 777f
            });
          if (Game1.dayOfMonth % 7 != 0 || !NetWorldState.checkAnywhereForWorldStateID("saloonSportsRoom") || Game1.timeOfDay >= 1500)
            break;
          Texture2D texture2D = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          this.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(368, 336, 19, 14),
            animationLength = 7,
            sourceRectStartingPos = new Vector2(368f, 336f),
            interval = 5000f,
            totalNumberOfLoops = 99999,
            position = new Vector2(34f, 3f) * 64f + new Vector2(7f, 13f) * 4f,
            scale = 4f,
            layerDepth = 0.0401f,
            id = 2400f
          });
          break;
        case 3030632101:
          if (!(name == "LeahHouse"))
            break;
          Game1.changeMusicTrack("distantBanjo");
          NPC characterFromName10 = Game1.getCharacterFromName("Leah");
          if (Game1.IsFall || Game1.IsWinter || Game1.isRaining)
            this.setFireplace(true, 11, 4, false);
          if (characterFromName10 == null || characterFromName10.currentLocation != this || characterFromName10.isDivorcedFrom(Game1.player))
            break;
          string path7 = "";
          switch (Game1.random.Next(3))
          {
            case 0:
              path7 = "Strings\\SpeechBubbles:LeahHouse_Leah_Greeting1";
              break;
            case 1:
              path7 = "Strings\\SpeechBubbles:LeahHouse_Leah_Greeting2";
              break;
            case 2:
              path7 = "Strings\\SpeechBubbles:LeahHouse_Leah_Greeting3";
              break;
          }
          characterFromName10.faceTowardFarmerForPeriod(3000, 15, false, Game1.player);
          characterFromName10.showTextAboveHead(Game1.content.LoadString(path7, (object) Game1.player.Name));
          break;
        case 3095702198:
          if (!(name == "AdventureGuild"))
            break;
          this.setFireplace(true, 9, 11, false);
          if (Game1.random.NextDouble() >= 0.5)
            break;
          NPC characterFromName11 = Game1.getCharacterFromName("Marlon");
          if (characterFromName11 == null)
            break;
          string path8 = "";
          switch (Game1.random.Next(5))
          {
            case 0:
              path8 = "Strings\\SpeechBubbles:AdventureGuild_Marlon_Greeting_" + (Game1.player.IsMale ? "Male" : "Female");
              break;
            case 1:
              path8 = "Strings\\SpeechBubbles:AdventureGuild_Marlon_Greeting1";
              break;
            case 2:
              path8 = "Strings\\SpeechBubbles:AdventureGuild_Marlon_Greeting2";
              break;
            case 3:
              path8 = "Strings\\SpeechBubbles:AdventureGuild_Marlon_Greeting3";
              break;
            case 4:
              path8 = "Strings\\SpeechBubbles:AdventureGuild_Marlon_Greeting4";
              break;
          }
          characterFromName11.showTextAboveHead(Game1.content.LoadString(path8));
          break;
        case 3755589785:
          if (!(name == "WitchHut") || !Game1.player.mailReceived.Contains("cursed_doll") || this.farmers.Any())
            break;
          this.characters.Clear();
          this.addCharacter((NPC) new Bat(new Vector2(7f, 6f) * 64f, -666));
          if (Game1.stats.getStat("childrenTurnedToDoves") > 1U)
            this.addCharacter((NPC) new Bat(new Vector2(4f, 7f) * 64f, -666));
          if (Game1.stats.getStat("childrenTurnedToDoves") > 2U)
            this.addCharacter((NPC) new Bat(new Vector2(10f, 7f) * 64f, -666));
          for (int index = 4; (long) index <= (long) Game1.stats.getStat("childrenTurnedToDoves"); ++index)
            this.addCharacter((NPC) new Bat(Utility.getRandomPositionInThisRectangle(new Microsoft.Xna.Framework.Rectangle(1, 4, 13, 4), Game1.random) * 64f + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-32, 32)), -666));
          break;
        case 3978811393:
          if (!(name == "AnimalShop"))
            break;
          this.setFireplace(true, 3, 14, false);
          if (Game1.random.NextDouble() < 0.5)
          {
            NPC characterFromName12 = Game1.getCharacterFromName("Marnie");
            if (characterFromName12 != null && characterFromName12.getTileY() == 14)
            {
              string path9 = "";
              switch (Game1.random.Next(4))
              {
                case 0:
                  path9 = "Strings\\SpeechBubbles:AnimalShop_Marnie_Greeting1";
                  break;
                case 1:
                  path9 = "Strings\\SpeechBubbles:AnimalShop_Marnie_Greeting2";
                  break;
                case 2:
                  path9 = Game1.player.getFriendshipHeartLevelForNPC("Marnie") > 4 ? "Strings\\SpeechBubbles:AnimalShop_Marnie_CloseFriends" : "Strings\\SpeechBubbles:AnimalShop_Marnie_NotCloseFriends";
                  break;
                case 3:
                  path9 = Game1.isRaining ? "Strings\\SpeechBubbles:AnimalShop_Marnie_Raining" : "Strings\\SpeechBubbles:AnimalShop_Marnie_NotRaining";
                  break;
                case 4:
                  path9 = "Strings\\SpeechBubbles:AnimalShop_Marnie_Greeting3";
                  break;
              }
              if (Game1.random.NextDouble() < 0.001)
                path9 = "Strings\\SpeechBubbles:AnimalShop_Marnie_RareGreeting";
              characterFromName12.showTextAboveHead(Game1.content.LoadString(path9, (object) Game1.player.Name, (object) Game1.player.farmName));
            }
          }
          if (this.getCharacterFromName("Marnie") == null && Game1.IsVisitingIslandToday("Marnie"))
            this.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.mouseCursors2,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(129, 210, 13, 16),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(129f, 210f),
              interval = 50000f,
              totalNumberOfLoops = 9999,
              position = new Vector2(13f, 14f) * 64f + new Vector2(3f, 0.0f) * 4f,
              scale = 4f,
              layerDepth = 0.1025f,
              id = 777f
            });
          if (Game1.netWorldState.Value.hasWorldStateID("m_painting0"))
          {
            this.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.mouseCursors,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(25, 1925, 25, 23),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(25f, 1925f),
              interval = 5000f,
              totalNumberOfLoops = 9999,
              position = new Vector2(16f, 1f) * 64f + new Vector2(3f, 1f) * 4f,
              scale = 4f,
              layerDepth = 0.1f,
              id = 777f
            });
            break;
          }
          if (Game1.netWorldState.Value.hasWorldStateID("m_painting1"))
          {
            this.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.mouseCursors,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 1925, 25, 23),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(0.0f, 1925f),
              interval = 5000f,
              totalNumberOfLoops = 9999,
              position = new Vector2(16f, 1f) * 64f + new Vector2(3f, 1f) * 4f,
              scale = 4f,
              layerDepth = 0.1f,
              id = 777f
            });
            break;
          }
          if (!Game1.netWorldState.Value.hasWorldStateID("m_painting2"))
            break;
          this.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.mouseCursors,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 1948, 25, 24),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(0.0f, 1948f),
            interval = 5000f,
            totalNumberOfLoops = 9999,
            position = new Vector2(16f, 1f) * 64f + new Vector2(3f, 1f) * 4f,
            scale = 4f,
            layerDepth = 0.1f,
            id = 777f
          });
          break;
      }
    }

    public virtual void hostSetup()
    {
      if (!Game1.IsMasterGame || this.farmers.Any() || this.HasFarmerWatchingBroadcastEventReturningHere())
        return;
      this.interiorDoors.ResetSharedState();
    }

    public virtual bool HasFarmerWatchingBroadcastEventReturningHere()
    {
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.locationBeforeForcedEvent.Value != null && allFarmer.locationBeforeForcedEvent.Value == this.NameOrUniqueName)
          return true;
      }
      return false;
    }

    public void resetForPlayerEntry()
    {
      Game1.updateWeatherIcon();
      Game1.hooks.OnGameLocation_ResetForPlayerEntry(this, (Action) (() =>
      {
        this._madeMapModifications = false;
        if (!this.farmers.Any() && !this.HasFarmerWatchingBroadcastEventReturningHere())
          this.resetSharedState();
        this.resetLocalState();
        if (this._madeMapModifications)
          return;
        this._madeMapModifications = true;
        this.MakeMapModifications();
      }));
      Microsoft.Xna.Framework.Rectangle boundingBox = Game1.player.GetBoundingBox();
      foreach (Furniture furniture in this.furniture)
      {
        if (furniture.getBoundingBox(furniture.TileLocation).Intersects(boundingBox) && furniture.IntersectsForCollision(boundingBox) && !furniture.isPassable())
          Game1.player.TemporaryPassableTiles.Add(furniture.getBoundingBox(furniture.TileLocation));
      }
    }

    protected virtual void resetLocalState()
    {
      Game1.elliottPiano = 0;
      Game1.crabPotOverlayTiles.Clear();
      Utility.killAllStaticLoopingSoundCues();
      Game1.player.bridge = (SuspensionBridge) null;
      Game1.player.SetOnBridge(false);
      if (Game1.CurrentEvent == null && !this.Name.ToLower().Contains("bath"))
        Game1.player.canOnlyWalk = false;
      if (!(this is Farm))
      {
        for (int index = this.temporarySprites.Count - 1; index >= 0; --index)
        {
          if (this.temporarySprites[index].clearOnAreaEntry())
            this.temporarySprites.RemoveAt(index);
        }
      }
      if (Game1.options != null)
      {
        if (Game1.isOneOfTheseKeysDown(Game1.GetKeyboardState(), Game1.options.runButton))
          Game1.player.setRunning(!Game1.options.autoRun, true);
        else
          Game1.player.setRunning(Game1.options.autoRun, true);
      }
      Game1.UpdateViewPort(false, new Point(Game1.player.getStandingX(), Game1.player.getStandingY()));
      Game1.previousViewportPosition = new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
      Game1.PushUIMode();
      foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
        onScreenMenu.gameWindowSizeChanged(new Microsoft.Xna.Framework.Rectangle(Game1.uiViewport.X, Game1.uiViewport.Y, Game1.uiViewport.Width, Game1.uiViewport.Height), new Microsoft.Xna.Framework.Rectangle(Game1.uiViewport.X, Game1.uiViewport.Y, Game1.uiViewport.Width, Game1.uiViewport.Height));
      Game1.PopUIMode();
      this.ignoreWarps = false;
      if (Game1.newDaySync == null || Game1.newDaySync.hasFinished())
      {
        if (Game1.player.rightRing.Value != null)
          Game1.player.rightRing.Value.onNewLocation(Game1.player, this);
        if (Game1.player.leftRing.Value != null)
          Game1.player.leftRing.Value.onNewLocation(Game1.player, this);
      }
      this.forceViewportPlayerFollow = this.Map.Properties.ContainsKey("ViewportFollowPlayer");
      this.lastTouchActionLocation = Game1.player.getTileLocation();
      for (int index = Game1.player.questLog.Count - 1; index >= 0; --index)
        Game1.player.questLog[index].adjustGameLocation(this);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isOutdoors)
        Game1.player.FarmerSprite.currentStep = "thudStep";
      this._updateAmbientLighting();
      this.setUpLocationSpecificFlair();
      PropertyValue propertyValue1;
      this.map.Properties.TryGetValue("UniquePortrait", out propertyValue1);
      if (propertyValue1 != null)
      {
        foreach (string name in propertyValue1.ToString().Split(' '))
        {
          NPC characterFromName = Game1.getCharacterFromName(name);
          if (this.characters.Contains(characterFromName))
          {
            try
            {
              characterFromName.Portrait = Game1.content.Load<Texture2D>("Portraits\\" + characterFromName.Name + "_" + (string) (NetFieldBase<string, NetString>) this.name);
              characterFromName.uniquePortraitActive = true;
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
      PropertyValue propertyValue2;
      this.map.Properties.TryGetValue("Light", out propertyValue2);
      if (propertyValue2 != null && !(bool) (NetFieldBase<bool, NetBool>) this.ignoreLights)
      {
        string[] strArray = propertyValue2.ToString().Split(' ');
        for (int index = 0; index < strArray.Length; index += 3)
          Game1.currentLightSources.Add(new LightSource(Convert.ToInt32(strArray[index + 2]), new Vector2((float) (Convert.ToInt32(strArray[index]) * 64 + 32), (float) (Convert.ToInt32(strArray[index + 1]) * 64 + 32)), 1f, LightSource.LightContext.MapLight));
      }
      propertyValue2 = (PropertyValue) null;
      this.map.Properties.TryGetValue("WindowLight", out propertyValue2);
      if (propertyValue2 != null && !(bool) (NetFieldBase<bool, NetBool>) this.ignoreLights)
      {
        string[] strArray = propertyValue2.ToString().Split(' ');
        for (int index = 0; index < strArray.Length; index += 3)
          Game1.currentLightSources.Add(new LightSource(Convert.ToInt32(strArray[index + 2]), new Vector2((float) (Convert.ToInt32(strArray[index]) * 64 + 32), (float) (Convert.ToInt32(strArray[index + 1]) * 64 + 32)), 1f, LightSource.LightContext.WindowLight));
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors || (bool) (NetFieldBase<bool, NetBool>) this.treatAsOutdoors)
      {
        PropertyValue propertyValue3;
        this.map.Properties.TryGetValue("BrookSounds", out propertyValue3);
        if (propertyValue3 != null)
        {
          string[] strArray = propertyValue3.ToString().Split(' ');
          for (int index = 0; index < strArray.Length; index += 3)
            AmbientLocationSounds.addSound(new Vector2((float) Convert.ToInt32(strArray[index]), (float) Convert.ToInt32(strArray[index + 1])), Convert.ToInt32(strArray[index + 2]));
        }
        Game1.randomizeRainPositions();
        Game1.randomizeDebrisWeatherPositions(Game1.debrisWeather);
      }
      foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.terrainFeatures.Pairs)
        pair.Value.performPlayerEntryAction(pair.Key);
      if (this.largeTerrainFeatures != null)
      {
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
          largeTerrainFeature.performPlayerEntryAction((Vector2) (NetFieldBase<Vector2, NetVector2>) largeTerrainFeature.tilePosition);
      }
      foreach (KeyValuePair<Vector2, Object> pair in this.objects.Pairs)
        pair.Value.actionOnPlayerEntry();
      if ((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors && Game1.shouldPlayMorningSong())
        Game1.playMorningSong();
      PropertyValue propertyValue4 = (PropertyValue) null;
      this.map.Properties.TryGetValue("Music", out propertyValue4);
      if (propertyValue4 != null)
      {
        string[] strArray = propertyValue4.ToString().Split(' ');
        if (strArray.Length > 1)
        {
          if (Game1.timeOfDay >= Convert.ToInt32(strArray[0]) && Game1.timeOfDay < Convert.ToInt32(strArray[1]) && !strArray[2].Equals(Game1.getMusicTrackName()))
            Game1.changeMusicTrack(strArray[2]);
        }
        else if (Game1.getMusicTrackName() == "none" || Game1.isMusicContextActiveButNotPlaying() || !strArray[0].Equals(Game1.getMusicTrackName()))
          Game1.changeMusicTrack(strArray[0]);
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors)
      {
        ((FarmerSprite) Game1.player.Sprite).currentStep = "sandyStep";
        this.tryToAddCritters();
      }
      this.interiorDoors.ResetLocalState();
      int trulyDarkTime = Game1.getTrulyDarkTime();
      if (Game1.timeOfDay < trulyDarkTime && (!Game1.IsRainingHere(this) || this.name.Equals((object) "SandyHouse")))
      {
        PropertyValue propertyValue5;
        this.map.Properties.TryGetValue("DayTiles", out propertyValue5);
        if (propertyValue5 != null)
        {
          string[] strArray = propertyValue5.ToString().Trim().Split(' ');
          for (int index = 0; index < strArray.Length; index += 4)
          {
            if ((!strArray[index + 3].Equals("720") || !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade")) && this.map.GetLayer(strArray[index]).Tiles[Convert.ToInt32(strArray[index + 1]), Convert.ToInt32(strArray[index + 2])] != null)
              this.map.GetLayer(strArray[index]).Tiles[Convert.ToInt32(strArray[index + 1]), Convert.ToInt32(strArray[index + 2])].TileIndex = Convert.ToInt32(strArray[index + 3]);
          }
        }
      }
      else if (Game1.timeOfDay >= trulyDarkTime || Game1.IsRainingHere(this) && !this.name.Equals((object) "SandyHouse"))
        this.switchOutNightTiles();
      if (this.name.Equals((object) "Coop"))
      {
        string[] strArray = this.getMapProperty("Feed").Split(' ');
        this.map.GetLayer("Buildings").Tiles[Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1])].TileIndex = Game1.MasterPlayer.Feed > 0 ? 31 : 35;
      }
      else if (this.name.Equals((object) "Barn"))
      {
        string[] strArray = this.getMapProperty("Feed").Split(' ');
        this.map.GetLayer("Buildings").Tiles[Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1])].TileIndex = Game1.MasterPlayer.Feed > 0 ? 31 : 35;
      }
      if (this.name.Equals((object) "Club"))
        Game1.changeMusicTrack("clubloop");
      else if (Game1.getMusicTrackName().Equals("clubloop"))
        Game1.changeMusicTrack("none");
      if (Game1.killScreen && Game1.activeClickableMenu != null && !Game1.dialogueUp)
      {
        Game1.activeClickableMenu.emergencyShutDown();
        Game1.exitActiveMenu();
      }
      if (Game1.activeClickableMenu == null && !Game1.warpingForForcedRemoteEvent)
        this.checkForEvents();
      Game1.currentLightSources.UnionWith((IEnumerable<LightSource>) this.sharedLights.Values);
      foreach (NPC character in this.characters)
        character.behaviorOnLocalFarmerLocationEntry(this);
      foreach (Furniture furniture in this.furniture)
        furniture.resetOnPlayerEntry(this);
      this.updateFishSplashAnimation();
      this.updateOrePanAnimation();
      this.showDropboxIndicator = false;
      foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
      {
        if (!specialOrder.ShouldDisplayAsComplete())
        {
          foreach (OrderObjective objective in specialOrder.objectives)
          {
            if (objective is DonateObjective)
            {
              DonateObjective donateObjective = objective as DonateObjective;
              if ((NetFieldBase<string, NetString>) donateObjective.dropBoxGameLocation != (NetString) null && donateObjective.GetDropboxLocationName() == this.Name)
              {
                this.showDropboxIndicator = true;
                this.dropBoxIndicatorLocation = donateObjective.dropBoxTileLocation.Value * 64f + new Vector2(7f, 0.0f) * 4f;
              }
            }
          }
        }
      }
    }

    protected void _updateAmbientLighting()
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isOutdoors || (bool) (NetFieldBase<bool, NetBool>) this.ignoreOutdoorLighting)
      {
        PropertyValue propertyValue;
        this.map.Properties.TryGetValue("AmbientLight", out propertyValue);
        if (propertyValue != null)
        {
          string[] strArray = propertyValue.ToString().Split(' ');
          Game1.ambientLight = new Microsoft.Xna.Framework.Color(Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]));
        }
        else
          Game1.ambientLight = Game1.isDarkOut() || (double) (float) (NetFieldBase<float, NetFloat>) this.lightLevel > 0.0 ? new Microsoft.Xna.Framework.Color(180, 180, 0) : Microsoft.Xna.Framework.Color.White;
        if (!Game1.getMusicTrackName().Contains("ambient"))
          return;
        Game1.changeMusicTrack("none", Game1.currentTrackOverrideable);
      }
      else
      {
        if (this is Desert)
          return;
        Game1.ambientLight = Game1.IsRainingHere(this) ? new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 200, 80) : Microsoft.Xna.Framework.Color.White;
      }
    }

    public void SelectRandomMiniJukeboxTrack()
    {
      if (this.miniJukeboxTrack.Value != "random")
        return;
      Farmer farmer = Game1.player;
      if (this is FarmHouse && (this as FarmHouse).owner != null)
        farmer = (this as FarmHouse).owner;
      List<string> list = farmer.songsHeard.Distinct<string>().ToList<string>();
      ChooseFromListMenu.FilterJukeboxTracks(list);
      this.randomMiniJukeboxTrack.Value = Utility.GetRandom<string>(list);
    }

    protected virtual void resetSharedState()
    {
      this.SelectRandomMiniJukeboxTrack();
      for (int index = this.characters.Count - 1; index >= 0; --index)
        this.characters[index].behaviorOnFarmerLocationEntry(this, Game1.player);
      PropertyValue propertyValue;
      this.Map.Properties.TryGetValue("UniqueSprite", out propertyValue);
      if (propertyValue != null)
      {
        foreach (string name in propertyValue.ToString().Split(' '))
        {
          NPC characterFromName = Game1.getCharacterFromName(name);
          if (this.characters.Contains(characterFromName))
          {
            try
            {
              characterFromName.Sprite.LoadTexture("Characters\\" + NPC.getTextureNameForCharacter(characterFromName.Name) + "_" + (string) (NetFieldBase<string, NetString>) this.name);
              characterFromName.uniqueSpriteActive = true;
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
      if (!(this is MineShaft))
      {
        string lower = Game1.currentSeason.ToLower();
        if (!(lower == "spring"))
        {
          if (!(lower == "summer"))
          {
            if (!(lower == "fall"))
            {
              if (lower == "winter")
                this.waterColor.Value = new Microsoft.Xna.Framework.Color(130, 80, (int) byte.MaxValue) * 0.5f;
            }
            else
              this.waterColor.Value = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 130, 200) * 0.5f;
          }
          else
            this.waterColor.Value = new Microsoft.Xna.Framework.Color(60, 240, (int) byte.MaxValue) * 0.5f;
        }
        else
          this.waterColor.Value = new Microsoft.Xna.Framework.Color(120, 200, (int) byte.MaxValue) * 0.5f;
      }
      if (!this.name.Equals((object) "Mountain") || Game1.timeOfDay >= 2000 && Game1.currentSeason.Equals("summer") && Game1.random.NextDouble() < 0.3 || !Game1.IsRainingHere(this) || Game1.currentSeason.Equals("winter"))
        return;
      Game1.random.NextDouble();
    }

    public LightSource getLightSource(int identifier)
    {
      LightSource lightSource;
      this.sharedLights.TryGetValue(identifier, out lightSource);
      return lightSource;
    }

    public bool hasLightSource(int identifier) => this.sharedLights.ContainsKey(identifier);

    public void removeLightSource(int identifier) => this.sharedLights.Remove(identifier);

    public void repositionLightSource(int identifier, Vector2 position)
    {
      LightSource lightSource;
      this.sharedLights.TryGetValue(identifier, out lightSource);
      if (lightSource == null)
        return;
      lightSource.position.Value = position;
    }

    public virtual bool isTileOccupiedForPlacement(Vector2 tileLocation, Object toPlace = null)
    {
      foreach (ResourceClump resourceClump in this.resourceClumps)
      {
        if (resourceClump.occupiesTile((int) tileLocation.X, (int) tileLocation.Y))
          return true;
      }
      Object @object;
      this.objects.TryGetValue(tileLocation, out @object);
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 64, 64);
      Microsoft.Xna.Framework.Rectangle boundingBox;
      for (int index = 0; index < this.characters.Count; ++index)
      {
        if (this.characters[index] != null)
        {
          boundingBox = this.characters[index].GetBoundingBox();
          if (boundingBox.Intersects(rectangle))
            return true;
        }
      }
      if (this.isTileOccupiedByFarmer(tileLocation) != null && (toPlace == null || !toPlace.isPassable()))
        return true;
      if (this.largeTerrainFeatures != null)
      {
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
        {
          boundingBox = largeTerrainFeature.getBoundingBox();
          if (boundingBox.Intersects(rectangle))
            return true;
        }
      }
      if (toPlace != null && toPlace.Category == -19)
      {
        if (toPlace.Category == -19 && this.terrainFeatures.ContainsKey(tileLocation) && this.terrainFeatures[tileLocation] is HoeDirt)
        {
          HoeDirt terrainFeature = this.terrainFeatures[tileLocation] as HoeDirt;
          if ((int) (NetFieldBase<int, NetInt>) (this.terrainFeatures[tileLocation] as HoeDirt).fertilizer != 0 || ((int) (NetFieldBase<int, NetInt>) toPlace.parentSheetIndex == 368 || (int) (NetFieldBase<int, NetInt>) toPlace.parentSheetIndex == 368) && terrainFeature.crop != null && (int) (NetFieldBase<int, NetInt>) terrainFeature.crop.currentPhase != 0)
            return true;
        }
      }
      else if (this.terrainFeatures.ContainsKey(tileLocation) && rectangle.Intersects(this.terrainFeatures[tileLocation].getBoundingBox(tileLocation)) && (!this.terrainFeatures[tileLocation].isPassable() || this.terrainFeatures[tileLocation] is HoeDirt && ((HoeDirt) this.terrainFeatures[tileLocation]).crop != null || toPlace != null && toPlace.isSapling()))
        return true;
      if ((toPlace == null || !(toPlace is BedFurniture) || this.isTilePassable(new Location((int) tileLocation.X, (int) tileLocation.Y), Game1.viewport) || !this.isTilePassable(new Location((int) tileLocation.X, (int) tileLocation.Y + 1), Game1.viewport)) && !this.isTilePassable(new Location((int) tileLocation.X, (int) tileLocation.Y), Game1.viewport) && (toPlace == null || !(toPlace is Wallpaper)))
        return true;
      if (toPlace != null && (toPlace.Category == -74 || toPlace.Category == -19) && @object != null && @object is IndoorPot)
      {
        if ((int) (NetFieldBase<int, NetInt>) toPlace.parentSheetIndex == 251)
        {
          if ((@object as IndoorPot).bush.Value == null && (@object as IndoorPot).hoeDirt.Value.crop == null)
            return false;
        }
        else if ((@object as IndoorPot).hoeDirt.Value.canPlantThisSeedHere((int) (NetFieldBase<int, NetInt>) toPlace.parentSheetIndex, (int) tileLocation.X, (int) tileLocation.Y, toPlace.Category == -19) && (@object as IndoorPot).bush.Value == null)
          return false;
      }
      return @object != null;
    }

    public Farmer isTileOccupiedByFarmer(Vector2 tileLocation)
    {
      foreach (Farmer farmer in this.farmers)
      {
        if (farmer.getTileLocation().Equals(tileLocation))
          return farmer;
      }
      return (Farmer) null;
    }

    public virtual bool isTileOccupied(
      Vector2 tileLocation,
      string characterToIgnore = "",
      bool ignoreAllCharacters = false)
    {
      foreach (ResourceClump resourceClump in this.resourceClumps)
      {
        if (resourceClump.occupiesTile((int) tileLocation.X, (int) tileLocation.Y))
          return true;
      }
      Object @object;
      this.objects.TryGetValue(tileLocation, out @object);
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) tileLocation.X * 64 + 1, (int) tileLocation.Y * 64 + 1, 62, 62);
      Microsoft.Xna.Framework.Rectangle boundingBox;
      if (!ignoreAllCharacters)
      {
        for (int index = 0; index < this.characters.Count; ++index)
        {
          if (this.characters[index] != null && !this.characters[index].Name.Equals(characterToIgnore))
          {
            boundingBox = this.characters[index].GetBoundingBox();
            if (boundingBox.Intersects(rectangle))
              return true;
          }
        }
      }
      if (this.terrainFeatures.ContainsKey(tileLocation) && rectangle.Intersects(this.terrainFeatures[tileLocation].getBoundingBox(tileLocation)) && (!NPC.isCheckingSpouseTileOccupancy || !this.terrainFeatures[tileLocation].isPassable()))
        return true;
      if (this.largeTerrainFeatures != null)
      {
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
        {
          boundingBox = largeTerrainFeature.getBoundingBox();
          if (boundingBox.Intersects(rectangle))
            return true;
        }
      }
      Furniture furnitureAt = this.GetFurnitureAt(tileLocation);
      if (furnitureAt != null && !furnitureAt.isPassable())
        return true;
      return (!NPC.isCheckingSpouseTileOccupancy || @object == null || !@object.isPassable()) && @object != null;
    }

    public virtual bool isTileOccupiedIgnoreFloors(Vector2 tileLocation, string characterToIgnore = "")
    {
      Object @object;
      this.objects.TryGetValue(tileLocation, out @object);
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) tileLocation.X * 64 + 1, (int) tileLocation.Y * 64 + 1, 62, 62);
      Microsoft.Xna.Framework.Rectangle boundingBox;
      for (int index = 0; index < this.characters.Count; ++index)
      {
        if (this.characters[index] != null && !this.characters[index].name.Equals((object) characterToIgnore))
        {
          boundingBox = this.characters[index].GetBoundingBox();
          if (boundingBox.Intersects(rectangle))
            return true;
        }
      }
      if (this.terrainFeatures.ContainsKey(tileLocation) && rectangle.Intersects(this.terrainFeatures[tileLocation].getBoundingBox(tileLocation)) && !this.terrainFeatures[tileLocation].isPassable())
        return true;
      if (this.largeTerrainFeatures != null)
      {
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
        {
          boundingBox = largeTerrainFeature.getBoundingBox();
          if (boundingBox.Intersects(rectangle))
            return true;
        }
      }
      Furniture furnitureAt = this.GetFurnitureAt(tileLocation);
      return furnitureAt != null && !furnitureAt.isPassable() || @object != null;
    }

    public bool isTileHoeDirt(Vector2 tileLocation) => this.terrainFeatures.ContainsKey(tileLocation) && this.terrainFeatures[tileLocation] is HoeDirt || this.objects.ContainsKey(tileLocation) && this.objects[tileLocation] is IndoorPot;

    public void playTerrainSound(
      Vector2 tileLocation,
      Character who = null,
      bool showTerrainDisturbAnimation = true)
    {
      string audioName = "thudStep";
      if (Game1.currentLocation.IsOutdoors || Game1.currentLocation.Name.ToLower().Contains("mine"))
      {
        switch (Game1.currentLocation.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "Type", "Back"))
        {
          case "Dirt":
            audioName = "sandyStep";
            break;
          case "Stone":
            audioName = "stoneStep";
            break;
          case "Grass":
            audioName = this.GetSeasonForLocation().Equals("winter") ? "snowyStep" : "grassyStep";
            break;
          case "Wood":
            audioName = "woodyStep";
            break;
          case null:
            if (Game1.currentLocation.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "Water", "Back") != null)
            {
              audioName = "waterSlosh";
              break;
            }
            break;
        }
      }
      else
        audioName = "thudStep";
      if (Game1.currentLocation.terrainFeatures.ContainsKey(tileLocation) && Game1.currentLocation.terrainFeatures[tileLocation] is Flooring)
        audioName = ((Flooring) Game1.currentLocation.terrainFeatures[tileLocation]).getFootstepSound();
      if (who != null & showTerrainDisturbAnimation && audioName.Equals("sandyStep"))
      {
        Vector2 zero = Vector2.Zero;
        if (who.shouldShadowBeOffset)
          zero = who.drawOffset.Value;
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 64, 64, 64), 50f, 4, 1, new Vector2(who.Position.X + (float) Game1.random.Next(-8, 8), who.Position.Y + (float) Game1.random.Next(-16, 0)) + zero, false, Game1.random.NextDouble() < 0.5, 0.0001f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.01f, 0.0f, (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 128.0)));
      }
      else if (who != null & showTerrainDisturbAnimation && this.GetSeasonForLocation().Equals("winter") && audioName.Equals("grassyStep"))
      {
        Vector2 zero = Vector2.Zero;
        if (who.shouldShadowBeOffset)
          zero = who.drawOffset.Value;
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(who.Position.X, who.Position.Y) + zero, false, false, 0.0001f, 1f / 1000f, Microsoft.Xna.Framework.Color.White, 1f, 0.01f, 0.0f, 0.0f));
      }
      if (audioName.Length <= 0)
        return;
      this.localSound(audioName);
    }

    public bool checkTileIndexAction(int tileIndex)
    {
      switch (tileIndex)
      {
        case 1799:
        case 1824:
        case 1825:
        case 1826:
        case 1827:
        case 1828:
        case 1829:
        case 1830:
        case 1831:
        case 1832:
        case 1833:
          if (this.Name.Equals("AbandonedJojaMart"))
          {
            (Game1.getLocationFromName("AbandonedJojaMart") as AbandonedJojaMart).checkBundle();
            return true;
          }
          break;
      }
      return false;
    }

    public virtual bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (who.IsSitting())
      {
        who.StopSitting();
        return true;
      }
      Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64);
      foreach (Farmer farmer in this.farmers)
      {
        if (farmer != Game1.player && farmer.GetBoundingBox().Intersects(rectangle1) && farmer.checkAction(who, this))
          return true;
      }
      if (this.currentEvent != null && this.currentEvent.isFestival)
        return this.currentEvent.checkAction(tileLocation, viewport, who);
      foreach (NPC character in this.characters)
      {
        if (character != null && !character.IsMonster && (!who.isRidingHorse() || !(character is Horse)) && character.GetBoundingBox().Intersects(rectangle1) && character.checkAction(who, this))
        {
          if (who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, false) || who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, true))
            who.faceGeneralDirection(character.getStandingPosition(), 0, false, false);
          return true;
        }
      }
      if (who.IsLocalPlayer && who.currentUpgrade != null && this.name.Equals((object) "Farm") && tileLocation.Equals((object) new Location((int) ((double) who.currentUpgrade.positionOfCarpenter.X + 32.0) / 64, (int) ((double) who.currentUpgrade.positionOfCarpenter.Y + 32.0) / 64)))
      {
        if (who.currentUpgrade.daysLeftTillUpgradeDone == 1)
          Game1.drawDialogue(Game1.getCharacterFromName("Robin"), Game1.content.LoadString("Data\\ExtraDialogue:Farm_RobinWorking_ReadyTomorrow"));
        else
          Game1.drawDialogue(Game1.getCharacterFromName("Robin"), Game1.content.LoadString("Data\\ExtraDialogue:Farm_RobinWorking" + (Game1.random.Next(2) + 1).ToString()));
      }
      foreach (ResourceClump resourceClump in this.resourceClumps)
      {
        if (resourceClump.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) resourceClump.tile).Intersects(rectangle1) && resourceClump.performUseAction(new Vector2((float) tileLocation.X, (float) tileLocation.Y), this))
          return true;
      }
      Vector2 key = new Vector2((float) tileLocation.X, (float) tileLocation.Y);
      if (this.objects.ContainsKey(key) && this.objects[key].Type != null)
      {
        if (who.isRidingHorse() && !(this.objects[key] is Fence))
          return false;
        if (key.Equals(who.getTileLocation()) && !this.objects[key].isPassable())
        {
          Tool t1 = (Tool) new Pickaxe();
          t1.DoFunction(Game1.currentLocation, -1, -1, 0, who);
          if (this.objects[key].performToolAction(t1, this))
          {
            this.objects[key].performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.objects[key].tileLocation, Game1.currentLocation);
            Object @object = this.objects[key];
            Vector2 toolLocation = who.GetToolLocation();
            Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
            double x = (double) boundingBox.Center.X;
            boundingBox = who.GetBoundingBox();
            double y = (double) boundingBox.Center.Y;
            Vector2 destination = new Vector2((float) x, (float) y);
            @object.dropItem(this, toolLocation, destination);
            Game1.currentLocation.Objects.Remove(key);
            return true;
          }
          Tool t2 = (Tool) new Axe();
          t2.DoFunction(Game1.currentLocation, -1, -1, 0, who);
          if (this.objects.ContainsKey(key) && this.objects[key].performToolAction(t2, this))
          {
            this.objects[key].performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.objects[key].tileLocation, Game1.currentLocation);
            Object @object = this.objects[key];
            Vector2 toolLocation = who.GetToolLocation();
            Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
            double x = (double) boundingBox.Center.X;
            boundingBox = who.GetBoundingBox();
            double y = (double) boundingBox.Center.Y;
            Vector2 destination = new Vector2((float) x, (float) y);
            @object.dropItem(this, toolLocation, destination);
            Game1.currentLocation.Objects.Remove(key);
            return true;
          }
          if (!this.objects.ContainsKey(key))
            return true;
        }
        if (this.objects.ContainsKey(key) && (this.objects[key].Type.Equals("Crafting") || this.objects[key].Type.Equals("interactive")))
        {
          if (who.ActiveObject == null && this.objects[key].checkForAction(who))
            return true;
          if (this.objects.ContainsKey(key))
          {
            if (who.CurrentItem == null)
              return this.objects[key].checkForAction(who);
            Object @object = this.objects[key].heldObject.Value;
            this.objects[key].heldObject.Value = (Object) null;
            bool flag1 = this.objects[key].performObjectDropInAction(who.CurrentItem, true, who);
            this.objects[key].heldObject.Value = @object;
            bool flag2 = this.objects[key].performObjectDropInAction(who.CurrentItem, false, who);
            if (flag1 | flag2 && who.isMoving())
              Game1.haltAfterCheck = false;
            if (!flag2)
              return this.objects[key].checkForAction(who) | flag1;
            who.reduceActiveItemByOne();
            return true;
          }
        }
        else if (this.objects.ContainsKey(key) && (bool) (NetFieldBase<bool, NetBool>) this.objects[key].isSpawnedObject)
        {
          int quality = (int) (NetFieldBase<int, NetInt>) this.objects[key].quality;
          Random random = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + (int) key.X + (int) key.Y * 777);
          if (who.professions.Contains(16) && this.objects[key].isForage(this))
            this.objects[key].Quality = 4;
          else if (this.objects[key].isForage(this))
          {
            if (random.NextDouble() < (double) who.ForagingLevel / 30.0)
              this.objects[key].Quality = 2;
            else if (random.NextDouble() < (double) who.ForagingLevel / 15.0)
              this.objects[key].Quality = 1;
          }
          if ((bool) (NetFieldBase<bool, NetBool>) this.objects[key].questItem && this.objects[key].questId.Value != 0 && !who.hasQuest((int) (NetFieldBase<int, NetInt>) this.objects[key].questId))
            return false;
          if (who.couldInventoryAcceptThisItem((Item) this.objects[key]))
          {
            if (who.IsLocalPlayer)
            {
              this.localSound("pickUpItem");
              DelayedAction.playSoundAfterDelay("coin", 300);
            }
            who.animateOnce(279 + who.FacingDirection);
            if (!this.isFarmBuildingInterior())
            {
              if (this.objects[key].isForage(this))
                who.gainExperience(2, 7);
            }
            else
              who.gainExperience(0, 5);
            who.addItemToInventoryBool(this.objects[key].getOne());
            ++Game1.stats.ItemsForaged;
            if (who.professions.Contains(13) && random.NextDouble() < 0.2 && !(bool) (NetFieldBase<bool, NetBool>) this.objects[key].questItem && who.couldInventoryAcceptThisItem((Item) this.objects[key]) && !this.isFarmBuildingInterior())
            {
              who.addItemToInventoryBool(this.objects[key].getOne());
              who.gainExperience(2, 7);
            }
            this.objects.Remove(key);
            return true;
          }
          this.objects[key].Quality = quality;
        }
      }
      if (who.isRidingHorse())
      {
        who.mount.checkAction(who, this);
        return true;
      }
      foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.terrainFeatures.Pairs)
      {
        if (pair.Value.getBoundingBox(pair.Key).Intersects(rectangle1) && pair.Value.performUseAction(pair.Key, this))
        {
          Game1.haltAfterCheck = false;
          return true;
        }
      }
      if (this.largeTerrainFeatures != null)
      {
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
        {
          if (largeTerrainFeature.getBoundingBox().Intersects(rectangle1) && largeTerrainFeature.performUseAction((Vector2) (NetFieldBase<Vector2, NetVector2>) largeTerrainFeature.tilePosition, this))
          {
            Game1.haltAfterCheck = false;
            return true;
          }
        }
      }
      string action = (string) null;
      Tile tile = this.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
      if (tile != null)
      {
        PropertyValue propertyValue;
        tile.Properties.TryGetValue("Action", out propertyValue);
        if (propertyValue != null)
          action = propertyValue.ToString();
      }
      if (action == null)
        action = this.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
      NPC npc = this.isCharacterAtTile(key + new Vector2(0.0f, 1f));
      if (action != null)
      {
        if (this.currentEvent != null || npc == null || npc.IsInvisible || npc.IsMonster || who.isRidingHorse() && npc is Horse || !Utility.withinRadiusOfPlayer(npc.getStandingX(), npc.getStandingY(), 1, who) || !npc.checkAction(who, this))
          return this.performAction(action, who, tileLocation);
        if (who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, who.IsCarrying()))
          who.faceGeneralDirection(npc.getStandingPosition(), 0, false, false);
        return true;
      }
      if (tile != null && this.checkTileIndexAction(tile.TileIndex))
        return true;
      foreach (MapSeat mapSeat in this.mapSeats)
      {
        if (mapSeat.OccupiesTile(tileLocation.X, tileLocation.Y) && !mapSeat.IsBlocked(this))
        {
          who.BeginSitting((ISittable) mapSeat);
          return true;
        }
      }
      Point point = new Point(tileLocation.X * 64, (tileLocation.Y - 1) * 64);
      bool flag = Game1.didPlayerJustRightClick();
      foreach (Furniture furniture in this.furniture)
      {
        Microsoft.Xna.Framework.Rectangle rectangle2 = furniture.boundingBox.Value;
        if (rectangle2.Contains((int) ((double) key.X * 64.0), (int) ((double) key.Y * 64.0)) && (int) (NetFieldBase<int, NetInt>) furniture.furniture_type != 12)
        {
          if (!flag)
            return furniture.clicked(who);
          return who.ActiveObject != null && furniture.performObjectDropInAction((Item) who.ActiveObject, false, who) || furniture.checkForAction(who, false);
        }
        if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type == 6)
        {
          rectangle2 = furniture.boundingBox.Value;
          if (rectangle2.Contains(point))
          {
            if (!flag)
              return furniture.clicked(who);
            return who.ActiveObject != null && furniture.performObjectDropInAction((Item) who.ActiveObject, false, who) || furniture.checkForAction(who, false);
          }
        }
      }
      return false;
    }

    public virtual bool CanFreePlaceFurniture() => false;

    public virtual bool LowPriorityLeftClick(int x, int y, Farmer who)
    {
      if (Game1.activeClickableMenu != null)
        return false;
      Microsoft.Xna.Framework.Rectangle rectangle;
      for (int index = this.furniture.Count - 1; index >= 0; --index)
      {
        Furniture furniture = this.furniture[index];
        if (this.CanFreePlaceFurniture() || furniture.IsCloseEnoughToFarmer(who))
        {
          if (!furniture.isPassable())
          {
            rectangle = furniture.boundingBox.Value;
            if (rectangle.Contains(x, y) && furniture.canBeRemoved(who))
            {
              furniture.AttemptRemoval((Action<Furniture>) (f =>
              {
                Guid job = this.furniture.GuidOf(f);
                if (this.furnitureToRemove.Contains(job))
                  return;
                this.furnitureToRemove.Add(job);
              }));
              return true;
            }
          }
          rectangle = furniture.boundingBox.Value;
          if (rectangle.Contains(x, y) && furniture.heldObject.Value != null)
          {
            furniture.clicked(who);
            return true;
          }
          if (!furniture.isGroundFurniture() && furniture.canBeRemoved(who))
          {
            int y1 = y;
            if (this is DecoratableLocation)
            {
              y1 = (this as DecoratableLocation).GetWallTopY(x / 64, y / 64);
              if (y1 == -1)
                y1 = y * 64;
            }
            rectangle = furniture.boundingBox.Value;
            if (rectangle.Contains(x, y1))
            {
              furniture.AttemptRemoval((Action<Furniture>) (f =>
              {
                Guid job = this.furniture.GuidOf(f);
                if (this.furnitureToRemove.Contains(job))
                  return;
                this.furnitureToRemove.Add(job);
              }));
              return true;
            }
          }
        }
      }
      for (int index = this.furniture.Count - 1; index >= 0; --index)
      {
        Furniture furniture = this.furniture[index];
        if ((this.CanFreePlaceFurniture() || furniture.IsCloseEnoughToFarmer(who)) && furniture.isPassable())
        {
          rectangle = furniture.boundingBox.Value;
          if (rectangle.Contains(x, y) && furniture.canBeRemoved(who))
          {
            furniture.AttemptRemoval((Action<Furniture>) (f =>
            {
              Guid job = this.furniture.GuidOf(f);
              if (this.furnitureToRemove.Contains(job))
                return;
              this.furnitureToRemove.Add(job);
            }));
            return true;
          }
        }
      }
      return false;
    }

    public virtual bool CanPlaceThisFurnitureHere(Furniture furniture)
    {
      if (furniture == null)
        return false;
      if (furniture.furniture_type.Value == 15)
      {
        switch (this)
        {
          case FarmHouse _:
          case IslandFarmHouse _:
            break;
          default:
            return false;
        }
      }
      int placementRestriction = furniture.placementRestriction;
      return (placementRestriction == 0 || placementRestriction == 2) && (this is DecoratableLocation || !this.IsOutdoors) || (placementRestriction == 1 || placementRestriction == 2) && !(this is DecoratableLocation) && this.IsOutdoors;
    }

    [Obsolete("These values returned by this function are no longer used by the game (except for rare, backwards compatibility related cases.) Check DecoratableLocation's wallpaper/flooring related functionality instead.")]
    public virtual List<Microsoft.Xna.Framework.Rectangle> getWalls() => new List<Microsoft.Xna.Framework.Rectangle>();

    protected virtual void removeQueuedFurniture(Guid guid)
    {
      Farmer player = Game1.player;
      if (!this.furniture.ContainsGuid(guid))
        return;
      Furniture furniture = this.furniture[guid];
      if (!player.couldInventoryAcceptThisItem((Item) furniture))
        return;
      furniture.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture.tileLocation, this);
      this.furniture.Remove(guid);
      bool flag = false;
      for (int index = 0; index < 12; ++index)
      {
        if (player.items[index] == null)
        {
          player.items[index] = (Item) furniture;
          player.CurrentToolIndex = index;
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        Item inventory = player.addItemToInventory((Item) furniture, 11);
        player.addItemToInventory(inventory);
        player.CurrentToolIndex = 11;
      }
      this.localSound("coin");
    }

    public virtual bool leftClick(int x, int y, Farmer who)
    {
      Vector2 key = new Vector2((float) (x / 64), (float) (y / 64));
      if (!this.objects.ContainsKey(key) || !this.objects[key].clicked(who))
        return false;
      this.objects.Remove(key);
      return true;
    }

    public virtual int getExtraMillisecondsPerInGameMinuteForThisLocation() => 0;

    public bool isTileLocationTotallyClearAndPlaceable(int x, int y) => this.isTileLocationTotallyClearAndPlaceable(new Vector2((float) x, (float) y));

    public virtual bool isTileLocationTotallyClearAndPlaceableIgnoreFloors(Vector2 v) => this.isTileOnMap(v) && !this.isTileOccupiedIgnoreFloors(v) && this.isTilePassable(new Location((int) v.X, (int) v.Y), Game1.viewport) && this.isTilePlaceable(v);

    public void ActivateKitchen(NetRef<Chest> fridge)
    {
      List<NetMutex> mutexes = new List<NetMutex>();
      List<Chest> mini_fridges = new List<Chest>();
      foreach (Object @object in this.objects.Values)
      {
        if (@object != null && (bool) (NetFieldBase<bool, NetBool>) @object.bigCraftable && @object is Chest && @object.ParentSheetIndex == 216)
        {
          mini_fridges.Add(@object as Chest);
          mutexes.Add((@object as Chest).mutex);
        }
      }
      if ((NetFieldBase<Chest, NetRef<Chest>>) fridge != (NetRef<Chest>) null && fridge.Value.mutex.IsLocked())
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
      }
      else
      {
        MultipleMutexRequest multiple_mutex_request = (MultipleMutexRequest) null;
        multiple_mutex_request = new MultipleMutexRequest(mutexes, (Action) (() => fridge.Value.mutex.RequestLock((Action) (() =>
        {
          List<Chest> material_containers = new List<Chest>();
          if ((NetFieldBase<Chest, NetRef<Chest>>) fridge != (NetRef<Chest>) null)
            material_containers.Add((Chest) (NetFieldBase<Chest, NetRef<Chest>>) fridge);
          material_containers.AddRange((IEnumerable<Chest>) mini_fridges);
          Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);
          Game1.activeClickableMenu = (IClickableMenu) new CraftingPage((int) centeringOnScreen.X, (int) centeringOnScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true, true, material_containers);
          Game1.activeClickableMenu.exitFunction = (IClickableMenu.onExit) (() =>
          {
            fridge.Value.mutex.ReleaseLock();
            multiple_mutex_request.ReleaseLocks();
          });
        }), (Action) (() =>
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
          multiple_mutex_request.ReleaseLocks();
        }))), (Action) (() => Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"))));
      }
    }

    public virtual bool isTileLocationTotallyClearAndPlaceable(Vector2 v)
    {
      Vector2 vector2 = v * 64f;
      vector2.X += 32f;
      vector2.Y += 32f;
      foreach (Furniture furniture in this.furniture)
      {
        if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type != 12 && !furniture.isPassable() && furniture.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture.tileLocation).Contains((int) vector2.X, (int) vector2.Y) && !furniture.AllowPlacementOnThisTile((int) v.X, (int) v.Y))
          return false;
      }
      return this.isTileOnMap(v) && !this.isTileOccupied(v) && this.isTilePassable(new Location((int) v.X, (int) v.Y), Game1.viewport) && this.isTilePlaceable(v);
    }

    public virtual bool isTilePlaceable(Vector2 v, Item item = null)
    {
      if (this.doesTileHaveProperty((int) v.X, (int) v.Y, "NoFurniture", "Back") != null && (item == null || !(item is Object) || !(item as Object).isPassable() || !Game1.currentLocation.IsOutdoors || this.doesTileHavePropertyNoNull((int) v.X, (int) v.Y, "NoFurniture", "Back").Equals("total")))
        return false;
      if (this.doesTileHaveProperty((int) v.X, (int) v.Y, "Water", "Back") == null)
        return true;
      return item != null && item.canBePlacedInWater();
    }

    public virtual bool shouldShadowBeDrawnAboveBuildingsLayer(Vector2 p)
    {
      TerrainFeature terrainFeature;
      return this.doesTileHaveProperty((int) p.X, (int) p.Y, "Passable", "Buildings") != null || this.terrainFeatures.TryGetValue(p, out terrainFeature) && terrainFeature is HoeDirt || this.doesTileHaveProperty((int) p.X, (int) p.Y, "Water", "Back") != null && (!(this.getTileSheetIDAt((int) p.X, (int) p.Y, "Buildings") == "Town") || this.getTileIndexAt((int) p.X, (int) p.Y, "Buildings") < 1004 || this.getTileIndexAt((int) p.X, (int) p.Y, "Buildings") > 1013);
    }

    public void openDoor(Location tileLocation, bool playSound)
    {
      try
      {
        int tileIndexAt = this.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings");
        Point key = new Point(tileLocation.X, tileLocation.Y);
        if (!this.interiorDoors.ContainsKey(key))
          return;
        this.interiorDoors[key] = true;
        if (!playSound)
          return;
        Vector2 position = new Vector2((float) tileLocation.X, (float) tileLocation.Y);
        if (tileIndexAt == 120)
          this.playSoundAt("doorOpen", position);
        else
          this.playSoundAt("doorCreak", position);
      }
      catch (Exception ex)
      {
      }
    }

    public void doStarpoint(string which)
    {
      if (!(which == "3"))
      {
        if (!(which == "4") || Game1.player.ActiveObject == null || Game1.player.ActiveObject == null || (bool) (NetFieldBase<bool, NetBool>) Game1.player.ActiveObject.bigCraftable || (int) (NetFieldBase<int, NetInt>) Game1.player.ActiveObject.parentSheetIndex != 203)
          return;
        Object o = new Object(Vector2.Zero, 162);
        if (!Game1.player.couldInventoryAcceptThisItem((Item) o) && (int) (NetFieldBase<int, NetInt>) Game1.player.ActiveObject.stack > 1)
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
        }
        else
        {
          Game1.player.reduceActiveItemByOne();
          Game1.player.makeThisTheActiveObject(o);
          this.localSound("croak");
          Game1.flashAlpha = 1f;
        }
      }
      else
      {
        if (Game1.player.ActiveObject == null || Game1.player.ActiveObject == null || (bool) (NetFieldBase<bool, NetBool>) Game1.player.ActiveObject.bigCraftable || (int) (NetFieldBase<int, NetInt>) Game1.player.ActiveObject.parentSheetIndex != 307)
          return;
        Object o = new Object(Vector2.Zero, 161);
        if (!Game1.player.couldInventoryAcceptThisItem((Item) o) && (int) (NetFieldBase<int, NetInt>) Game1.player.ActiveObject.stack > 1)
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
        }
        else
        {
          Game1.player.reduceActiveItemByOne();
          Game1.player.makeThisTheActiveObject(o);
          this.localSound("discoverMineral");
          Game1.flashAlpha = 1f;
        }
      }
    }

    public virtual string FormatCompletionLine(Func<Farmer, float> check)
    {
      KeyValuePair<Farmer, float> farmCompletion = Utility.GetFarmCompletion(check);
      return farmCompletion.Key == Game1.player ? farmCompletion.Value.ToString() : "(" + farmCompletion.Key.Name + ") " + farmCompletion.Value.ToString();
    }

    public virtual string FormatCompletionLine(
      Func<Farmer, bool> check,
      string true_value,
      string false_value)
    {
      KeyValuePair<Farmer, bool> farmCompletion = Utility.GetFarmCompletion(check);
      if (farmCompletion.Key != Game1.player)
        return "(" + farmCompletion.Key.Name + ") " + (farmCompletion.Value ? true_value : false_value);
      return !farmCompletion.Value ? false_value : true_value;
    }

    public virtual void ShowQiCat()
    {
      if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") && !Game1.MasterPlayer.mailReceived.Contains("GotPerfectionStatue"))
      {
        Game1.MasterPlayer.mailReceived.Add("GotPerfectionStatue");
        Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item) new Object(Vector2.Zero, 280));
      }
      else
      {
        Game1.playSound("qi_shop");
        StringBuilder stringBuilder = new StringBuilder();
        bool flag = Game1.viewport.Height < 850;
        string[] messages = new string[2];
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_Title") + "^");
        stringBuilder.AppendLine("----------------^");
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_Shipped") + ": " + this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Floor((double) Utility.getFarmerItemsShippedPercent(farmer) * 100.0))) + "%^");
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_Obelisks") + ": " + Math.Min(Utility.numObelisksOnFarm(), 4).ToString() + "/4^");
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_GoldClock") + ": " + (Game1.getFarm().isBuildingConstructed("Gold Clock") ? Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes") : Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")) + "^");
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_MonsterSlayer") + ": " + this.FormatCompletionLine((Func<Farmer, bool>) (farmer => farmer.hasCompletedAllMonsterSlayerQuests.Value), Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes"), Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")) + "^");
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_GreatFriends") + ": " + this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Floor((double) Utility.getMaxedFriendshipPercent(farmer) * 100.0))) + "%^");
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_FarmerLevel") + ": " + this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Min(farmer.Level, 25))) + "/25^");
        if (flag)
        {
          stringBuilder.AppendLine("...");
          messages[0] = stringBuilder.ToString();
          stringBuilder.Clear();
        }
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_Stardrops") + ": " + this.FormatCompletionLine((Func<Farmer, bool>) (farmer => Utility.foundAllStardrops(farmer)), Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes"), Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")) + "^");
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_Cooking") + ": " + this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Floor((double) Utility.getCookedRecipesPercent(farmer) * 100.0))) + "%^");
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_Crafting") + ": " + this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Floor((double) Utility.getCraftedRecipesPercent(farmer) * 100.0))) + "%^");
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_Fish") + ": " + this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Floor((double) Utility.getFishCaughtPercent(farmer) * 100.0))) + "%^");
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_GoldenWalnut") + ": " + Math.Min((int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.GoldenWalnutsFound, 130).ToString() + "/" + 130.ToString() + "^");
        stringBuilder.AppendLine("----------------^");
        stringBuilder.AppendLine(Utility.loadStringShort("UI", "PT_Total") + ": " + Math.Floor((double) Utility.percentGameComplete() * 100.0).ToString() + "%^");
        if (flag)
        {
          messages[1] = stringBuilder.ToString();
          Game1.multipleDialogues(messages);
        }
        else
          Game1.drawDialogueNoTyping(stringBuilder.ToString());
      }
    }

    public virtual bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (action != null && who.IsLocalPlayer)
      {
        string[] strArray = action.Split(' ');
        string s = strArray[0];
        // ISSUE: reference to a compiler-generated method
        switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(s))
        {
          case 4774130:
            if (s == "EvilShrineRight")
            {
              if (Game1.spawnMonstersAtNight)
              {
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineRightDeActivate"), this.createYesNoResponses(), "evilShrineRightDeActivate");
                goto label_386;
              }
              else
              {
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineRightActivate"), this.createYesNoResponses(), "evilShrineRightActivate");
                goto label_386;
              }
            }
            else
              goto default;
          case 48641340:
            if (s == "SpiritAltar")
            {
              if (who.ActiveObject != null && (double) (NetFieldBase<double, NetDouble>) Game1.player.team.sharedDailyLuck != -0.12 && (double) (NetFieldBase<double, NetDouble>) Game1.player.team.sharedDailyLuck != 0.12)
              {
                if (who.ActiveObject.Price >= 60)
                {
                  this.temporarySprites.Add(new TemporaryAnimatedSprite(352, 70f, 2, 2, new Vector2((float) (tileLocation.X * 64), (float) (tileLocation.Y * 64)), false, false));
                  Game1.player.team.sharedDailyLuck.Value = 0.12;
                  this.playSound("money");
                }
                else
                {
                  this.temporarySprites.Add(new TemporaryAnimatedSprite(362, 50f, 6, 1, new Vector2((float) (tileLocation.X * 64), (float) (tileLocation.Y * 64)), false, false));
                  Game1.player.team.sharedDailyLuck.Value = -0.12;
                  this.playSound("thunder");
                }
                who.ActiveObject = (Object) null;
                who.showNotCarrying();
                goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 49977355:
            if (s == "Crib")
            {
              foreach (NPC character in this.characters)
              {
                if (character is Child)
                {
                  if ((character as Child).Age == 1)
                  {
                    (character as Child).toss(who);
                    return true;
                  }
                  if ((character as Child).Age == 0)
                  {
                    Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:FarmHouse_Crib_NewbornSleeping", (object) character.displayName)));
                    return true;
                  }
                  if ((character as Child).isInCrib() && (character as Child).Age == 2)
                    return character.checkAction(who, this);
                }
              }
              return false;
            }
            goto default;
          case 135767117:
            if (s == "Jukebox")
            {
              Game1.activeClickableMenu = (IClickableMenu) new ChooseFromListMenu(Game1.player.songsHeard.Distinct<string>().ToList<string>(), new ChooseFromListMenu.actionOnChoosingListOption(ChooseFromListMenu.playSongAction), true);
              goto label_386;
            }
            else
              goto default;
          case 139067618:
            if (s == "IceCreamStand")
            {
              if (this.isCharacterAtTile(new Vector2((float) tileLocation.X, (float) (tileLocation.Y - 2))) != null || this.isCharacterAtTile(new Vector2((float) tileLocation.X, (float) (tileLocation.Y - 1))) != null || this.isCharacterAtTile(new Vector2((float) tileLocation.X, (float) (tileLocation.Y - 3))) != null)
              {
                Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(new Dictionary<ISalable, int[]>()
                {
                  {
                    (ISalable) new Object(233, 1),
                    new int[2]{ 250, int.MaxValue }
                  }
                });
                goto label_386;
              }
              else if (Game1.currentSeason.Equals("summer"))
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:IceCreamStand_ComeBackLater"));
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:IceCreamStand_NotSummer"));
                goto label_386;
              }
            }
            else
              goto default;
          case 183343509:
            if (s == "ColaMachine")
            {
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_ColaMachine_Question"), this.createYesNoResponses(), "buyJojaCola");
              goto label_386;
            }
            else
              goto default;
          case 199988773:
            if (s == "DropBox")
            {
              string box_id = strArray[1];
              int minimum_capacity = 0;
              foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
              {
                if (specialOrder.UsesDropBox(box_id))
                  minimum_capacity = Math.Max(minimum_capacity, specialOrder.GetMinimumDropBoxCapacity(box_id));
              }
              foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
              {
                SpecialOrder order = specialOrder;
                if (order.UsesDropBox(box_id))
                {
                  order.donateMutex.RequestLock((Action) (() =>
                  {
                    while (order.donatedItems.Count < minimum_capacity)
                      order.donatedItems.Add((Item) null);
                    Game1.activeClickableMenu = (IClickableMenu) new QuestContainerMenu((IList<Item>) order.donatedItems, highlight_method: new InventoryMenu.highlightThisItem(order.HighlightAcceptableItems), stack_capacity_check: new Func<Item, int>(order.GetAcceptCount), on_item_changed: new Action(order.UpdateDonationCounts), on_confirm: new Action(order.ConfirmCompleteDonations));
                  }));
                  return true;
                }
              }
              return false;
            }
            goto default;
          case 234320812:
            if (s == "SandDragon")
            {
              if (who.ActiveObject != null && (int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex == 768 && !who.hasOrWillReceiveMail("TH_SandDragon") && who.hasOrWillReceiveMail("TH_MayorFridge"))
              {
                who.reduceActiveItemByOne();
                Game1.player.CanMove = false;
                this.localSound("eat");
                Game1.player.mailReceived.Add("TH_SandDragon");
                Game1.multipleDialogues(new string[2]
                {
                  Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_ConsumeEssence"),
                  Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_MrQiNote")
                });
                Game1.player.removeQuest(4);
                Game1.player.addQuest(5);
                goto label_386;
              }
              else if (who.hasOrWillReceiveMail("TH_SandDragon"))
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_MrQiNote"));
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_Initial"));
                goto label_386;
              }
            }
            else
              goto default;
          case 267393898:
            if (s == "Notes")
            {
              this.readNote(Convert.ToInt32(strArray[1]));
              goto label_386;
            }
            else
              goto default;
          case 295776207:
            if (s == "Warp")
            {
              who.faceGeneralDirection(new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f);
              Rumble.rumble(0.15f, 200f);
              if (strArray.Length < 5)
                this.playSoundAt("doorClose", new Vector2((float) tileLocation.X, (float) tileLocation.Y));
              Game1.warpFarmer(strArray[3], Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), false);
              goto label_386;
            }
            else
              goto default;
          case 297990791:
            if (s == "NPCMessage")
            {
              NPC characterFromName = Game1.getCharacterFromName(strArray[1]);
              if (characterFromName != null && characterFromName.currentLocation == who.currentLocation)
              {
                if (Utility.tileWithinRadiusOfPlayer(characterFromName.getTileX(), characterFromName.getTileY(), 14, who))
                {
                  try
                  {
                    string path = action.Substring(action.IndexOf('"') + 1).Split('/')[0];
                    string str = path.Substring(path.IndexOf(':') + 1);
                    characterFromName.setNewDialogue(Game1.content.LoadString(path), true);
                    Game1.drawDialogue(characterFromName);
                    if ((str == "AnimalShop_Marnie_Trash" || str == "JoshHouse_Alex_Trash" || str == "SamHouse_Sam_Trash" || str == "SeedShop_Abigail_Drawers") && who != null)
                      Game1.multiplayer.globalChatInfoMessage("Caught_Snooping", (string) (NetFieldBase<string, NetString>) who.name, characterFromName.displayName);
                    return true;
                  }
                  catch (Exception ex)
                  {
                    return false;
                  }
                }
              }
              try
              {
                Game1.drawDialogueNoTyping(Game1.content.LoadString(action.Substring(action.IndexOf('"')).Split('/')[1].Replace("\"", "")));
                return false;
              }
              catch (Exception ex)
              {
                return false;
              }
            }
            else
              goto default;
          case 371676316:
            if (s == "MineElevator")
            {
              if (MineShaft.lowestLevelReached < 5)
              {
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Mines_MineElevator_NotWorking")));
                goto label_386;
              }
              else
              {
                Game1.activeClickableMenu = (IClickableMenu) new MineElevatorMenu();
                goto label_386;
              }
            }
            else
              goto default;
          case 414528787:
            if (s == "QiCoins")
            {
              if (who.clubCoins > 0)
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_QiCoins", (object) who.clubCoins));
                goto label_386;
              }
              else
              {
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_QiCoins_BuyStarter"), this.createYesNoResponses(), "BuyClubCoins");
                goto label_386;
              }
            }
            else
              goto default;
          case 570160120:
            if (s == "MagicInk")
            {
              if (!who.mailReceived.Contains("hasPickedUpMagicInk"))
              {
                who.mailReceived.Add("hasPickedUpMagicInk");
                who.hasMagicInk = true;
                this.setMapTileIndex(4, 11, 113, "Buildings");
                who.addItemByMenuIfNecessaryElseHoldUp((Item) new SpecialItem(7));
                goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 634795166:
            if (s == "ClubSlots")
            {
              Game1.currentMinigame = (IMinigame) new Slots();
              goto label_386;
            }
            else
              goto default;
          case 760041535:
            if (s == "SummitBoulder")
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SummitBoulder"));
              goto label_386;
            }
            else
              goto default;
          case 837292325:
            if (s == "BuyBackpack")
            {
              Response response1 = new Response("Purchase", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Response2000"));
              Response response2 = new Response("Purchase", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Response10000"));
              Response response3 = new Response("Not", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_ResponseNo"));
              if ((int) (NetFieldBase<int, NetInt>) Game1.player.maxItems == 12)
              {
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question24"), new Response[2]
                {
                  response1,
                  response3
                }, "Backpack");
                goto label_386;
              }
              else if ((int) (NetFieldBase<int, NetInt>) Game1.player.maxItems < 36)
              {
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question36"), new Response[2]
                {
                  response2,
                  response3
                }, "Backpack");
                goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 895720287:
            if (s == "HospitalShop")
            {
              if (this.isCharacterAtTile(who.getTileLocation() + new Vector2(0.0f, -2f)) != null || this.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) != null)
              {
                Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getHospitalStock());
                goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 908820861:
            if (s == "Letter")
            {
              Game1.drawLetterMessage(Game1.content.LoadString("Strings\\StringsFromMaps:" + strArray[1].Replace("\"", "")));
              goto label_386;
            }
            else
              goto default;
          case 1094091226:
            if (s == "Arcade_Prairie")
            {
              this.showPrairieKingMenu();
              goto label_386;
            }
            else
              goto default;
          case 1135412759:
            if (s == "Carpenter" && who.getTileY() > tileLocation.Y)
              return this.carpenters(tileLocation);
            goto default;
          case 1140116675:
            if (s == "JojaShop")
            {
              Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getJojaStock());
              goto label_386;
            }
            else
              goto default;
          case 1148451341:
            if (s == "DogStatue")
            {
              if (GameLocation.canRespec(0) || GameLocation.canRespec(3) || GameLocation.canRespec(2) || GameLocation.canRespec(4) || GameLocation.canRespec(1))
              {
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatue"), this.createYesNoResponses(), "dogStatue");
                goto label_386;
              }
              else
              {
                string str = Game1.content.LoadString("Strings\\Locations:Sewer_DogStatue");
                Game1.drawObjectDialogue(str.Substring(0, str.LastIndexOf('^')));
                goto label_386;
              }
            }
            else
              goto default;
          case 1152722114:
            if (s == "QiGemShop")
            {
              Game1.playSound("qi_shop");
              Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.GetQiChallengeRewardStock(Game1.player), 4, context: "QiGemShop")
              {
                purchaseSound = "qi_shop_purchase"
              };
              return true;
            }
            goto default;
          case 1272937089:
            if (s == "SpecialOrders")
            {
              Game1.player.team.ordersBoardMutex.RequestLock((Action) (() =>
              {
                Game1.activeClickableMenu = (IClickableMenu) new SpecialOrdersBoard()
                {
                  behaviorBeforeCleanup = (Action<IClickableMenu>) (menu => Game1.player.team.ordersBoardMutex.ReleaseLock())
                };
              }));
              goto label_386;
            }
            else
              goto default;
          case 1367472567:
            if (s == "Blacksmith" && who.getTileY() > tileLocation.Y)
              return this.blacksmith(tileLocation);
            goto default;
          case 1379459566:
            if (s == "WarpMensLocker")
            {
              if (!who.IsMale)
              {
                if (who.IsLocalPlayer)
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MensLocker_WrongGender"));
                return true;
              }
              who.faceGeneralDirection(new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f);
              if (strArray.Length < 5)
                this.playSoundAt("doorClose", new Vector2((float) tileLocation.X, (float) tileLocation.Y));
              Game1.warpFarmer(strArray[3], Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), false);
              goto label_386;
            }
            else
              goto default;
          case 1473946688:
            if (s == "GoldenScythe")
            {
              if (!Game1.player.mailReceived.Contains("gotGoldenScythe"))
              {
                if (!Game1.player.isInventoryFull())
                {
                  Game1.playSound("parry");
                  Game1.player.mailReceived.Add("gotGoldenScythe");
                  this.setMapTileIndex(29, 4, 245, "Front");
                  this.setMapTileIndex(30, 4, 246, "Front");
                  this.setMapTileIndex(29, 5, 261, "Front");
                  this.setMapTileIndex(30, 5, 262, "Front");
                  this.setMapTileIndex(29, 6, 277, "Buildings");
                  this.setMapTileIndex(30, 56, 278, "Buildings");
                  Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item) new MeleeWeapon(53));
                  goto label_386;
                }
                else
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                  goto label_386;
                }
              }
              else
              {
                Game1.changeMusicTrack("none");
                this.performTouchAction("MagicWarp Mine 67 10", Game1.player.getStandingPosition());
                goto label_386;
              }
            }
            else
              goto default;
          case 1555723527:
            if (s == "BlackJack")
              goto label_319;
            else
              goto default;
          case 1573286044:
            if (s == "ClubCards")
              goto label_319;
            else
              goto default;
          case 1666484702:
            if (s == "Tailoring")
            {
              if (who.eventsSeen.Contains(992559))
              {
                Game1.activeClickableMenu = (IClickableMenu) new TailoringMenu();
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:HaleyHouse_SewingMachine"));
                goto label_386;
              }
            }
            else
              goto default;
          case 1673854597:
            if (s == "WizardShrine")
            {
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WizardTower_WizardShrine").Replace('\n', '^'), this.createYesNoResponses(), "WizardShrine");
              goto label_386;
            }
            else
              goto default;
          case 1687261568:
            if (s == "ClubComputer")
              goto label_325;
            else
              goto default;
          case 1716769139:
            if (s == "Dialogue")
            {
              Game1.drawDialogueNoTyping(this.actionParamsToString(strArray));
              goto label_386;
            }
            else
              goto default;
          case 1719994463:
            if (s == "WizardBook")
            {
              if (who.mailReceived.Contains("hasPickedUpMagicInk") || who.hasMagicInk)
              {
                Game1.activeClickableMenu = (IClickableMenu) new CarpenterMenu(true);
                goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 1722787773:
            if (s == "MessageOnce")
            {
              if (!who.eventsSeen.Contains(Convert.ToInt32(strArray[1])))
              {
                who.eventsSeen.Add(Convert.ToInt32(strArray[1]));
                Game1.drawObjectDialogue(Game1.parseText(this.actionParamsToString(strArray).Substring(this.actionParamsToString(strArray).IndexOf(' '))));
                goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 1803743016:
            if (s == "NPCSpeechMessageNoRadius")
            {
              NPC characterFromName = Game1.getCharacterFromName(strArray[1]);
              if (characterFromName != null)
              {
                try
                {
                  characterFromName.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:" + strArray[2]), true);
                  Game1.drawDialogue(characterFromName);
                  return true;
                }
                catch (Exception ex)
                {
                  return false;
                }
              }
              else
              {
                try
                {
                  NPC speaker = new NPC((AnimatedSprite) null, Vector2.Zero, "", 0, strArray[1], false, (Dictionary<int, int[]>) null, Game1.temporaryContent.Load<Texture2D>("Portraits\\" + strArray[1]));
                  speaker.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:" + strArray[2]), true);
                  Game1.drawDialogue(speaker);
                  return true;
                }
                catch (Exception ex)
                {
                  return false;
                }
              }
            }
            else
              goto default;
          case 1806134029:
            if (s == "BuyQiCoins")
            {
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_Buy100Coins"), this.createYesNoResponses(), "BuyQiCoins");
              goto label_386;
            }
            else
              goto default;
          case 1852246243:
            if (s == "NextMineLevel")
              goto label_296;
            else
              goto default;
          case 1856350152:
            if (s == "Yoba")
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_Yoba"));
              goto label_386;
            }
            else
              goto default;
          case 1959071256:
            if (s == "WizardHatch")
            {
              if (who.friendshipData.ContainsKey("Wizard") && who.friendshipData["Wizard"].Points >= 1000)
              {
                this.playSoundAt("doorClose", new Vector2((float) tileLocation.X, (float) tileLocation.Y));
                Game1.warpFarmer("WizardHouseBasement", 4, 4, true);
                goto label_386;
              }
              else
              {
                NPC character = this.characters[0];
                character.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Data\\ExtraDialogue:Wizard_Hatch"), character));
                Game1.drawDialogue(character);
                goto label_386;
              }
            }
            else
              goto default;
          case 2039065585:
            if (s == "QiCat")
            {
              this.ShowQiCat();
              goto label_386;
            }
            else
              goto default;
          case 2039622173:
            if (s == "LockedDoorWarp")
            {
              who.faceGeneralDirection(new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f);
              this.lockedDoorWarp(strArray);
              goto label_386;
            }
            else
              goto default;
          case 2072952568:
            if (s == "Theater_BoxOffice")
            {
              if (Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
              {
                if (Game1.isFestival())
                {
                  Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieTheater_ClosedFestival")));
                  goto label_386;
                }
                else if (Game1.timeOfDay > 2100)
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_BoxOfficeClosed"));
                  goto label_386;
                }
                else if (MovieTheater.GetMovieForDate(Game1.Date) != null)
                {
                  Dictionary<ISalable, int[]> itemPriceAndStock = new Dictionary<ISalable, int[]>();
                  Object key = new Object(809, 1);
                  itemPriceAndStock.Add((ISalable) key, new int[2]
                  {
                    key.salePrice(),
                    int.MaxValue
                  });
                  Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(itemPriceAndStock, who: "boxOffice");
                  goto label_386;
                }
                else
                  goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 2250926415:
            if (s == "Tutorial")
            {
              Game1.activeClickableMenu = (IClickableMenu) new TutorialMenu();
              goto label_386;
            }
            else
              goto default;
          case 2279498422:
            if (s == "WarpGreenhouse")
            {
              if (Game1.MasterPlayer.mailReceived.Contains("ccPantry"))
              {
                who.faceGeneralDirection(new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f);
                this.playSoundAt("doorClose", new Vector2((float) tileLocation.X, (float) tileLocation.Y));
                GameLocation locationFromName = Game1.getLocationFromName("Greenhouse");
                int tileX = 10;
                int tileY = 23;
                if (locationFromName != null)
                {
                  foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) locationFromName.warps)
                  {
                    if (warp.TargetName == "Farm")
                    {
                      tileX = warp.X;
                      tileY = warp.Y - 1;
                      break;
                    }
                  }
                }
                Game1.warpFarmer("Greenhouse", tileX, tileY, false);
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Farm_GreenhouseRuins"));
                goto label_386;
              }
            }
            else
              goto default;
          case 2295680585:
            if (s == "Door")
            {
              if (strArray.Length > 1 && !Game1.eventUp)
              {
                for (int index = 1; index < strArray.Length; ++index)
                {
                  if (who.getFriendshipHeartLevelForNPC(strArray[index]) >= 2 || Game1.player.mailReceived.Contains("doorUnlock" + strArray[index]))
                  {
                    Rumble.rumble(0.1f, 100f);
                    if (!Game1.player.mailReceived.Contains("doorUnlock" + strArray[index]))
                      Game1.player.mailReceived.Add("doorUnlock" + strArray[index]);
                    this.openDoor(tileLocation, true);
                    return true;
                  }
                }
                if (strArray.Length == 2 && Game1.getCharacterFromName(strArray[1]) != null)
                {
                  NPC characterFromName = Game1.getCharacterFromName(strArray[1]);
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_" + (characterFromName.Gender == 0 ? "Male" : "Female"), (object) characterFromName.displayName));
                  goto label_386;
                }
                else if (Game1.getCharacterFromName(strArray[1]) != null && Game1.getCharacterFromName(strArray[2]) != null)
                {
                  NPC characterFromName1 = Game1.getCharacterFromName(strArray[1]);
                  NPC characterFromName2 = Game1.getCharacterFromName(strArray[2]);
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_Couple", (object) characterFromName1.displayName, (object) characterFromName2.displayName));
                  goto label_386;
                }
                else if (Game1.getCharacterFromName(strArray[1]) != null)
                {
                  NPC characterFromName = Game1.getCharacterFromName(strArray[1]);
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_" + (characterFromName.Gender == 0 ? "Male" : "Female"), (object) characterFromName.displayName));
                  goto label_386;
                }
                else
                  goto label_386;
              }
              else
              {
                this.openDoor(tileLocation, true);
                return true;
              }
            }
            else
              goto default;
          case 2308862302:
            if (s == "MonsterGrave")
            {
              if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en && who.eventsSeen.Contains(6963327))
              {
                Game1.multipleDialogues(new string[2]
                {
                  "Abigail took a life to save mine...",
                  "I'll never forget that."
                });
                goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 2413466880:
            if (s == "RailroadBox")
            {
              if (who.ActiveObject != null && (int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex == 394 && !who.hasOrWillReceiveMail("TH_Railroad") && who.hasOrWillReceiveMail("TH_Tunnel"))
              {
                who.reduceActiveItemByOne();
                Game1.player.CanMove = false;
                this.localSound("Ship");
                Game1.player.mailReceived.Add("TH_Railroad");
                Game1.multipleDialogues(new string[2]
                {
                  Game1.content.LoadString("Strings\\Locations:Railroad_Box_ConsumeShell"),
                  Game1.content.LoadString("Strings\\Locations:Railroad_Box_MrQiNote")
                });
                Game1.player.removeQuest(2);
                Game1.player.addQuest(3);
                goto label_386;
              }
              else if (who.hasOrWillReceiveMail("TH_Railroad"))
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Railroad_Box_MrQiNote"));
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Railroad_Box_Initial"));
                goto label_386;
              }
            }
            else
              goto default;
          case 2471112148:
            if (s == "FarmerFile")
              goto label_325;
            else
              goto default;
          case 2510785065:
            if (s == "EvilShrineCenter")
            {
              if (who.isDivorced())
              {
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineCenter"), this.createYesNoResponses(), "evilShrineCenter");
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineCenterInactive"));
                goto label_386;
              }
            }
            else
              goto default;
          case 2528917107:
            if (s == "StorageBox")
            {
              this.openStorageBox(this.actionParamsToString(strArray));
              goto label_386;
            }
            else
              goto default;
          case 2636547234:
            if (s == "Theater_Poster")
            {
              if (Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
              {
                MovieData movieForDate = MovieTheater.GetMovieForDate(Game1.Date);
                if (movieForDate != null)
                {
                  Game1.multipleDialogues(new string[2]
                  {
                    Game1.content.LoadString("Strings\\Locations:Theater_Poster_0", (object) movieForDate.Title),
                    Game1.content.LoadString("Strings\\Locations:Theater_Poster_1", (object) movieForDate.Description)
                  });
                  goto label_386;
                }
                else
                  goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 2675290304:
            if (s == "QiChallengeBoard")
            {
              Game1.player.team.qiChallengeBoardMutex.RequestLock((Action) (() =>
              {
                Game1.activeClickableMenu = (IClickableMenu) new SpecialOrdersBoard("Qi")
                {
                  behaviorBeforeCleanup = (Action<IClickableMenu>) (menu => Game1.player.team.qiChallengeBoardMutex.ReleaseLock())
                };
              }));
              goto label_386;
            }
            else
              goto default;
          case 2724686923:
            if (s == "LeoParrot")
            {
              TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(5858585);
              if (temporarySpriteById != null && temporarySpriteById is EmilysParrot)
              {
                (temporarySpriteById as EmilysParrot).doAction();
                goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 2738932126:
            if (s == "ClubSeller")
            {
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_ClubSeller"), new Response[2]
              {
                new Response("I'll", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_Yes")),
                new Response("No", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_No"))
              }, "ClubSeller");
              goto label_386;
            }
            else
              goto default;
          case 2764184545:
            if (s == "MinecartTransport")
            {
              if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
              {
                Response[] answerChoices;
                if (Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom"))
                  answerChoices = new Response[4]
                  {
                    new Response("Town", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Town")),
                    new Response("Bus", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_BusStop")),
                    new Response("Quarry", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Quarry")),
                    new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel"))
                  };
                else
                  answerChoices = new Response[3]
                  {
                    new Response("Town", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Town")),
                    new Response("Bus", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_BusStop")),
                    new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel"))
                  };
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_ChooseDestination"), answerChoices, "Minecart");
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_OutOfOrder"));
                goto label_386;
              }
            }
            else
              goto default;
          case 2802141700:
            if (s == "Billboard")
            {
              Game1.activeClickableMenu = (IClickableMenu) new Billboard(strArray[1].Equals("3"));
              goto label_386;
            }
            else
              goto default;
          case 2817094304:
            if (s == "Incubator")
            {
              (this as AnimalHouse).incubator();
              goto label_386;
            }
            else
              goto default;
          case 2832988535:
            if (s == "AdventureShop")
            {
              this.adventureShop();
              goto label_386;
            }
            else
              goto default;
          case 2909376585:
            if (s == "Saloon" && who.getTileY() > tileLocation.Y)
              return this.saloon(tileLocation);
            goto default;
          case 2920208772:
            if (s == "Message")
              break;
            goto default;
          case 2959114096:
            if (s == "GetLumber")
            {
              this.GetLumber();
              goto label_386;
            }
            else
              goto default;
          case 2987480683:
            if (s == "Mailbox")
            {
              if (this is Farm)
              {
                Point mailboxPosition = Game1.player.getMailboxPosition();
                if (tileLocation.X != mailboxPosition.X || tileLocation.Y != mailboxPosition.Y)
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Farm_OtherPlayerMailbox"));
                  goto label_386;
                }
              }
              this.mailbox();
              goto label_386;
            }
            else
              goto default;
          case 3158608557:
            if (s == "Starpoint")
            {
              try
              {
                this.doStarpoint(strArray[1]);
                goto label_386;
              }
              catch (Exception ex)
              {
                goto label_386;
              }
            }
            else
              goto default;
          case 3162274371:
            if (s == "MineSign")
            {
              Game1.drawObjectDialogue(Game1.parseText(this.actionParamsToString(strArray)));
              goto label_386;
            }
            else
              goto default;
          case 3211203767:
            if (s == "HMTGF")
            {
              if (who.ActiveObject != null && who.ActiveObject != null && !(bool) (NetFieldBase<bool, NetBool>) who.ActiveObject.bigCraftable && (int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex == 155)
              {
                Object o = new Object(Vector2.Zero, 155);
                if (!Game1.player.couldInventoryAcceptThisItem((Item) o) && (int) (NetFieldBase<int, NetInt>) Game1.player.ActiveObject.stack > 1)
                {
                  Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                  goto label_386;
                }
                else
                {
                  Game1.player.reduceActiveItemByOne();
                  Game1.player.makeThisTheActiveObject(o);
                  this.localSound("discoverMineral");
                  Game1.flashAlpha = 1f;
                  goto label_386;
                }
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 3212056737:
            if (s == "ElliottPiano")
            {
              this.playElliottPiano(int.Parse(strArray[1]));
              goto label_386;
            }
            else
              goto default;
          case 3244018497:
            if (s == "WarpCommunityCenter")
            {
              if (Game1.MasterPlayer.mailReceived.Contains("ccDoorUnlock") || Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
              {
                this.playSoundAt("doorClose", new Vector2((float) tileLocation.X, (float) tileLocation.Y));
                Game1.warpFarmer("CommunityCenter", 32, 23, false);
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8175"));
                goto label_386;
              }
            }
            else
              goto default;
          case 3275078514:
            if (s == "Warp_Sunroom_Door")
            {
              if (who.getFriendshipHeartLevelForNPC("Caroline") >= 2)
              {
                this.playSoundAt("doorClose", new Vector2((float) tileLocation.X, (float) tileLocation.Y));
                Game1.warpFarmer("Sunroom", 5, 13, false);
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Caroline_Sunroom_Door"));
                goto label_386;
              }
            }
            else
              goto default;
          case 3327972754:
            if (s == "TunnelSafe")
            {
              if (who.ActiveObject != null && (int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex == 787 && !who.hasOrWillReceiveMail("TH_Tunnel"))
              {
                who.reduceActiveItemByOne();
                Game1.player.CanMove = false;
                this.playSound("openBox");
                DelayedAction.playSoundAfterDelay("doorCreakReverse", 500);
                Game1.player.mailReceived.Add("TH_Tunnel");
                Game1.multipleDialogues(new string[2]
                {
                  Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_ConsumeBattery"),
                  Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_MrQiNote")
                });
                Game1.player.addQuest(2);
                goto label_386;
              }
              else if (who.hasOrWillReceiveMail("TH_Tunnel"))
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_MrQiNote"));
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_Initial"));
                goto label_386;
              }
            }
            else
              goto default;
          case 3329002772:
            if (s == "Minecart")
            {
              this.openChest(tileLocation, 4, Game1.random.Next(3, 7));
              goto label_386;
            }
            else
              goto default;
          case 3355121210:
            if (s == "Theater_PosterComingSoon")
            {
              if (Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
              {
                WorldDate date = new WorldDate(Game1.Date);
                date.TotalDays += 28;
                MovieData movieForDate = MovieTheater.GetMovieForDate(date);
                if (movieForDate != null)
                {
                  Game1.multipleDialogues(new string[1]
                  {
                    Game1.content.LoadString("Strings\\Locations:Theater_Poster_Coming_Soon", (object) movieForDate.Title)
                  });
                  goto label_386;
                }
                else
                  goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 3371180897:
            if (s == "Craft")
            {
              GameLocation.openCraftingMenu(this.actionParamsToString(strArray));
              goto label_386;
            }
            else
              goto default;
          case 3372476774:
            if (s == "LumberPile")
            {
              if (!who.hasOrWillReceiveMail("TH_LumberPile") && who.hasOrWillReceiveMail("TH_SandDragon"))
              {
                Game1.player.hasClubCard = true;
                Game1.player.CanMove = false;
                Game1.player.mailReceived.Add("TH_LumberPile");
                Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item) new SpecialItem(2));
                Game1.player.removeQuest(5);
                goto label_386;
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 3385614082:
            if (s == "playSound")
            {
              this.localSound(strArray[1]);
              goto label_386;
            }
            else
              goto default;
          case 3403315211:
            if (s == "Buy" && who.getTileY() >= tileLocation.Y)
              return this.openShopMenu(strArray[1]);
            goto default;
          case 3419754368:
            if (s == "Material")
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8205", (object) who.WoodPieces, (object) who.StonePieces).Replace("\n", "^"));
              goto label_386;
            }
            else
              goto default;
          case 3459692634:
            if (s == "MessageSpeech")
              break;
            goto default;
          case 3603749081:
            if (s == "EnterSewer")
            {
              if (who.hasRustyKey && !who.mailReceived.Contains("OpenedSewer"))
              {
                this.playSound("openBox");
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Forest_OpenedSewer")));
                who.mailReceived.Add("OpenedSewer");
                goto label_386;
              }
              else if (who.mailReceived.Contains("OpenedSewer"))
              {
                this.playSoundAt("stairsdown", new Vector2((float) tileLocation.X, (float) tileLocation.Y));
                Game1.warpFarmer("Sewer", 16, 11, 2);
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
                goto label_386;
              }
            }
            else
              goto default;
          case 3642125430:
            if (s == "ExitMine")
            {
              this.createQuestionDialogue(" ", new Response[3]
              {
                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Mines_LeaveMine")),
                new Response("Go", Game1.content.LoadString("Strings\\Locations:Mines_GoUp")),
                new Response("Do", Game1.content.LoadString("Strings\\Locations:Mines_DoNothing"))
              }, "ExitMine");
              goto label_386;
            }
            else
              goto default;
          case 3672442469:
            if (s == "Theater_Entrance")
            {
              if (Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
              {
                if (Game1.player.team.movieMutex.IsLocked())
                {
                  Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieTheater_CurrentlyShowing")));
                  goto label_386;
                }
                else if (Game1.isFestival())
                {
                  Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieTheater_ClosedFestival")));
                  goto label_386;
                }
                else if (Game1.timeOfDay > 2100 || Game1.timeOfDay < 900)
                {
                  string sub1 = Game1.getTimeOfDayString(900).Replace(" ", "");
                  string sub2 = Game1.getTimeOfDayString(2100).Replace(" ", "");
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_OpenRange", (object) sub1, (object) sub2));
                  goto label_386;
                }
                else if ((int) (NetFieldBase<int, NetInt>) Game1.player.lastSeenMovieWeek >= Game1.Date.TotalWeeks)
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_AlreadySeen"));
                  goto label_386;
                }
                else
                {
                  NPC npc = (NPC) null;
                  foreach (MovieInvitation movieInvitation in Game1.player.team.movieInvitations)
                  {
                    if (movieInvitation.farmer == Game1.player && !movieInvitation.fulfilled && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player)
                    {
                      npc = movieInvitation.invitedNPC;
                      break;
                    }
                  }
                  if (npc != null && Game1.player.hasItemInInventory(809, 1))
                  {
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_WatchWithFriendPrompt", (object) npc.displayName), Game1.currentLocation.createYesNoResponses(), "EnterTheaterSpendTicket");
                    goto label_386;
                  }
                  else if (Game1.player.hasItemInInventory(809, 1))
                  {
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_WatchAlonePrompt"), Game1.currentLocation.createYesNoResponses(), "EnterTheaterSpendTicket");
                    goto label_386;
                  }
                  else
                  {
                    Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieTheater_NoTicket")));
                    goto label_386;
                  }
                }
              }
              else
                goto label_386;
            }
            else
              goto default;
          case 3754457510:
            if (s == "SkullDoor")
            {
              if (who.hasSkullKey)
              {
                if (!who.hasUnlockedSkullDoor)
                {
                  Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:SkullCave_SkullDoor_Unlock")));
                  DelayedAction.playSoundAfterDelay("openBox", 500);
                  DelayedAction.playSoundAfterDelay("openBox", 700);
                  Game1.addMailForTomorrow("skullCave");
                  who.hasUnlockedSkullDoor = true;
                  who.completeQuest(19);
                  goto label_386;
                }
                else
                {
                  who.completelyStopAnimatingOrDoingAction();
                  this.playSound("doorClose");
                  DelayedAction.playSoundAfterDelay("stairsdown", 500, this);
                  Game1.enterMine(121);
                  MineShaft.numberOfCraftedStairsUsedThisRun = 0;
                  goto label_386;
                }
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SkullCave_SkullDoor_Locked"));
                goto label_386;
              }
            }
            else
              goto default;
          case 3848897750:
            if (s == "Mine")
              goto label_296;
            else
              goto default;
          case 3912414904:
            if (s == "DwarfGrave")
            {
              if (who.canUnderstandDwarves)
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_DwarfGrave_Translated").Replace('\n', '^'));
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8214"));
                goto label_386;
              }
            }
            else
              goto default;
          case 3961338715:
            if (s == "Lamp")
            {
              if ((double) (float) (NetFieldBase<float, NetFloat>) this.lightLevel == 0.0)
                this.lightLevel.Value = 0.6f;
              else
                this.lightLevel.Value = 0.0f;
              this.playSound("openBox");
              goto label_386;
            }
            else
              goto default;
          case 3970342769:
            if (s == "ClubShop")
            {
              Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getQiShopStock(), 2);
              goto label_386;
            }
            else
              goto default;
          case 3978811393:
            if (s == "AnimalShop" && who.getTileY() > tileLocation.Y)
              return this.animalShop(tileLocation);
            goto default;
          case 4012092003:
            if (s == "Arcade_Minecart")
            {
              if (who.hasSkullKey)
              {
                Response[] answerChoices = new Response[3]
                {
                  new Response("Progress", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_ProgressMode")),
                  new Response("Endless", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_EndlessMode")),
                  new Response("Exit", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Exit"))
                };
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Menu"), answerChoices, "MinecartGame");
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Inactive"));
                goto label_386;
              }
            }
            else
              goto default;
          case 4067857873:
            if (s == "EvilShrineLeft")
            {
              if (who.getChildrenCount() == 0)
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineLeftInactive"));
                goto label_386;
              }
              else
              {
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineLeft"), this.createYesNoResponses(), "evilShrineLeft");
                goto label_386;
              }
            }
            else
              goto default;
          case 4073653847:
            if (s == "ItemChest")
            {
              this.openItemChest(tileLocation, Convert.ToInt32(strArray[1]));
              goto label_386;
            }
            else
              goto default;
          case 4090469326:
            if (s == "RemoveChest")
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:RemoveChest"));
              this.map.GetLayer("Buildings").Tiles[tileLocation.X, tileLocation.Y] = (Tile) null;
              goto label_386;
            }
            else
              goto default;
          case 4097465949:
            if (s == "EmilyRoomObject")
            {
              if (Game1.player.eventsSeen.Contains(463391) && (Game1.player.spouse == null || !Game1.player.spouse.Equals("Emily")))
              {
                TemporaryAnimatedSprite temporarySpriteById = this.getTemporarySpriteByID(5858585);
                if (temporarySpriteById != null && temporarySpriteById is EmilysParrot)
                {
                  (temporarySpriteById as EmilysParrot).doAction();
                  goto label_386;
                }
                else
                  goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:HaleyHouse_EmilyRoomObject"));
                goto label_386;
              }
            }
            else
              goto default;
          case 4104253281:
            if (s == "ElliottBook")
            {
              if (who.eventsSeen.Contains(41))
              {
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ElliottHouse_ElliottBook_Filled", (object) Game1.elliottBookName, (object) who.displayName)));
                goto label_386;
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ElliottHouse_ElliottBook_Blank"));
                goto label_386;
              }
            }
            else
              goto default;
          case 4212892660:
            if (s == "WarpWomensLocker")
            {
              if (who.IsMale)
              {
                if (who.IsLocalPlayer)
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WomensLocker_WrongGender"));
                return true;
              }
              who.faceGeneralDirection(new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f);
              if (strArray.Length < 5)
                this.playSoundAt("doorClose", new Vector2((float) tileLocation.X, (float) tileLocation.Y));
              Game1.warpFarmer(strArray[3], Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), false);
              goto label_386;
            }
            else
              goto default;
          case 4237471600:
            if (s == "DyePot")
            {
              if (who.eventsSeen.Contains(992559))
              {
                if (!DyeMenu.IsWearingDyeable())
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:DyePot_NoDyeable"));
                  goto label_386;
                }
                else
                {
                  Game1.activeClickableMenu = (IClickableMenu) new DyeMenu();
                  goto label_386;
                }
              }
              else
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:HaleyHouse_DyePot"));
                goto label_386;
              }
            }
            else
              goto default;
          case 4284593235:
            if (s == "MineWallDecor")
            {
              this.getWallDecorItem(tileLocation);
              goto label_386;
            }
            else
              goto default;
          default:
            return false;
        }
        Game1.drawDialogueNoTyping(Game1.content.LoadString("Strings\\StringsFromMaps:" + strArray[1].Replace("\"", "")));
        goto label_386;
label_296:
        this.playSound("stairsdown");
        Game1.enterMine(strArray.Length == 1 ? 1 : Convert.ToInt32(strArray[1]));
        goto label_386;
label_319:
        if (strArray.Length > 1 && strArray[1].Equals("1000"))
        {
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_HS"), new Response[2]
          {
            new Response("Play", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Play")),
            new Response("Leave", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Leave"))
          }, "CalicoJackHS");
          goto label_386;
        }
        else
        {
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJack"), new Response[3]
          {
            new Response("Play", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Play")),
            new Response("Leave", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Leave")),
            new Response("Rules", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Rules"))
          }, "CalicoJack");
          goto label_386;
        }
label_325:
        this.farmerFile();
label_386:
        return true;
      }
      if (action != null && !who.IsLocalPlayer)
      {
        string str = action.ToString().Split(' ')[0];
        if (!(str == "Minecart"))
        {
          if (!(str == "RemoveChest"))
          {
            if (!(str == "Door"))
            {
              if (str == "TV")
                Game1.tvStation = Game1.random.Next(2);
            }
            else
              this.openDoor(tileLocation, true);
          }
          else
            this.map.GetLayer("Buildings").Tiles[tileLocation.X, tileLocation.Y] = (Tile) null;
        }
        else
          this.openChest(tileLocation, 4, Game1.random.Next(3, 7));
      }
      return false;
    }

    public void showPrairieKingMenu()
    {
      if (Game1.player.jotpkProgress.Value == null)
      {
        Game1.currentMinigame = (IMinigame) new AbigailGame();
      }
      else
      {
        Response[] answerChoices = new Response[3]
        {
          new Response("Continue", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Cowboy_Continue")),
          new Response("NewGame", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Cowboy_NewGame")),
          new Response("Exit", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Exit"))
        };
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Cowboy_Menu"), answerChoices, "CowboyGame");
      }
    }

    public Vector2 findNearestObject(
      Vector2 startingPoint,
      int objectIndex,
      bool bigCraftable)
    {
      int num = 0;
      Queue<Vector2> vector2Queue1 = new Queue<Vector2>();
      vector2Queue1.Enqueue(startingPoint);
      HashSet<Vector2> vector2Set1 = new HashSet<Vector2>();
      List<Vector2> vector2List = new List<Vector2>();
      Queue<Vector2> vector2Queue2;
      HashSet<Vector2> vector2Set2;
      while (num < 1000)
      {
        if (this.objects.ContainsKey(startingPoint) && (int) (NetFieldBase<int, NetInt>) this.objects[startingPoint].parentSheetIndex == objectIndex && (bool) (NetFieldBase<bool, NetBool>) this.objects[startingPoint].bigCraftable == bigCraftable)
        {
          vector2Queue2 = (Queue<Vector2>) null;
          vector2Set2 = (HashSet<Vector2>) null;
          return startingPoint;
        }
        ++num;
        vector2Set1.Add(startingPoint);
        List<Vector2> adjacentTileLocations = Utility.getAdjacentTileLocations(startingPoint);
        for (int index = 0; index < adjacentTileLocations.Count; ++index)
        {
          if (!vector2Set1.Contains(adjacentTileLocations[index]))
            vector2Queue1.Enqueue(adjacentTileLocations[index]);
        }
        startingPoint = vector2Queue1.Dequeue();
      }
      vector2Queue2 = (Queue<Vector2>) null;
      vector2Set2 = (HashSet<Vector2>) null;
      return Vector2.Zero;
    }

    public void lockedDoorWarp(string[] actionParams)
    {
      bool flag = Game1.player.HasTownKey;
      if (GameLocation.AreStoresClosedForFestival() && this.Name != "Desert" && this.GetLocationContext() == GameLocation.LocationContext.Default)
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:FestivalDay_DoorLocked")));
      else if (actionParams[3].Equals("SeedShop") && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Wed") && !Utility.HasAnyPlayerSeenEvent(191393) && !flag)
      {
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:SeedShop_LockedWed")));
      }
      else
      {
        int num = Convert.ToInt32(actionParams[4]);
        if (actionParams[3] == "FishShop" && Game1.player.mailReceived.Contains("willyHours"))
          num = 800;
        if (flag)
        {
          if (flag && this.Name == "Desert")
            flag = false;
          if (flag && this.GetLocationContext() != GameLocation.LocationContext.Default)
            flag = false;
          if (flag && this is BeachNightMarket && actionParams[3] != "FishShop")
            flag = false;
        }
        if ((flag || Game1.timeOfDay >= num && Game1.timeOfDay < Convert.ToInt32(actionParams[5])) && (actionParams.Length < 7 || Game1.currentSeason.Equals("winter") || Game1.player.friendshipData.ContainsKey(actionParams[6]) && Game1.player.friendshipData[actionParams[6]].Points >= Convert.ToInt32(actionParams[7])))
        {
          Rumble.rumble(0.15f, 200f);
          Game1.player.completelyStopAnimatingOrDoingAction();
          this.playSoundAt("doorClose", Game1.player.getTileLocation());
          Game1.warpFarmer(actionParams[3], Convert.ToInt32(actionParams[1]), Convert.ToInt32(actionParams[2]), false);
        }
        else if (actionParams.Length < 7)
        {
          string sub1 = Game1.getTimeOfDayString(Convert.ToInt32(actionParams[4])).Replace(" ", "");
          if (actionParams[3] == "FishShop" && Game1.player.mailReceived.Contains("willyHours"))
            sub1 = Game1.getTimeOfDayString(800).Replace(" ", "");
          string sub2 = Game1.getTimeOfDayString(Convert.ToInt32(actionParams[5])).Replace(" ", "");
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_OpenRange", (object) sub1, (object) sub2));
        }
        else if (Game1.timeOfDay < Convert.ToInt32(actionParams[4]) || Game1.timeOfDay >= Convert.ToInt32(actionParams[5]))
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
        }
        else
        {
          NPC characterFromName = Game1.getCharacterFromName(actionParams[6]);
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_FriendsOnly", (object) characterFromName.displayName));
        }
      }
    }

    public void playElliottPiano(int key)
    {
      if (Game1.IsMultiplayer && (long) Game1.player.uniqueMultiplayerID % 111L == 0L)
      {
        switch (key)
        {
          case 1:
            this.playSoundPitched("toyPiano", 500);
            break;
          case 2:
            this.playSoundPitched("toyPiano", 1200);
            break;
          case 3:
            this.playSoundPitched("toyPiano", 1400);
            break;
          case 4:
            this.playSoundPitched("toyPiano", 2000);
            break;
        }
      }
      else
      {
        switch (key)
        {
          case 1:
            this.playSoundPitched("toyPiano", 1100);
            break;
          case 2:
            this.playSoundPitched("toyPiano", 1500);
            break;
          case 3:
            this.playSoundPitched("toyPiano", 1600);
            break;
          case 4:
            this.playSoundPitched("toyPiano", 1800);
            break;
        }
        switch (Game1.elliottPiano)
        {
          case 0:
            if (key == 2)
            {
              ++Game1.elliottPiano;
              break;
            }
            Game1.elliottPiano = 0;
            break;
          case 1:
            if (key == 4)
            {
              ++Game1.elliottPiano;
              break;
            }
            Game1.elliottPiano = 0;
            break;
          case 2:
            if (key == 3)
            {
              ++Game1.elliottPiano;
              break;
            }
            Game1.elliottPiano = 0;
            break;
          case 3:
            if (key == 2)
            {
              ++Game1.elliottPiano;
              break;
            }
            Game1.elliottPiano = 0;
            break;
          case 4:
            if (key == 3)
            {
              ++Game1.elliottPiano;
              break;
            }
            Game1.elliottPiano = 0;
            break;
          case 5:
            if (key == 4)
            {
              ++Game1.elliottPiano;
              break;
            }
            Game1.elliottPiano = 0;
            break;
          case 6:
            if (key == 2)
            {
              ++Game1.elliottPiano;
              break;
            }
            Game1.elliottPiano = 0;
            break;
          case 7:
            if (key == 1)
            {
              Game1.elliottPiano = 0;
              NPC characterFromName = this.getCharacterFromName("Elliott");
              if (Game1.eventUp || characterFromName == null || characterFromName.isMoving())
                break;
              characterFromName.faceTowardFarmerForPeriod(1000, 100, false, Game1.player);
              characterFromName.doEmote(20);
              break;
            }
            Game1.elliottPiano = 0;
            break;
        }
      }
    }

    public void readNote(int which)
    {
      if ((int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.LostBooksFound >= which)
      {
        string message = Game1.content.LoadString("Strings\\Notes:" + which.ToString()).Replace('\n', '^');
        if (!Game1.player.mailReceived.Contains("lb_" + which.ToString()))
          Game1.player.mailReceived.Add("lb_" + which.ToString());
        this.removeTemporarySpritesWithIDLocal((float) which);
        Game1.drawLetterMessage(message);
      }
      else
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Notes:Missing")));
    }

    public void mailbox()
    {
      if (Game1.mailbox.Count > 0)
      {
        if (!Game1.player.mailReceived.Contains(Game1.mailbox.First<string>()) && !Game1.mailbox.First<string>().Contains("passedOut") && !Game1.mailbox.First<string>().Contains("Cooking"))
          Game1.player.mailReceived.Add(Game1.mailbox.First<string>());
        string str = Game1.mailbox.First<string>();
        Game1.mailbox.RemoveAt(0);
        Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
        string mail = dictionary.ContainsKey(str) ? dictionary[str] : "";
        if (str.StartsWith("passedOut "))
        {
          string[] strArray = str.Split(' ');
          int Seed = strArray.Length > 1 ? Convert.ToInt32(strArray[1]) : 0;
          switch (new Random(Seed).Next(Game1.player.getSpouse() == null || !Game1.player.getSpouse().Name.Equals("Harvey") ? 3 : 2))
          {
            case 0:
              mail = !Game1.MasterPlayer.hasCompletedCommunityCenter() || Game1.MasterPlayer.mailReceived.Contains("JojaMember") ? string.Format(dictionary["passedOut1_" + (Seed > 0 ? "Billed" : "NotBilled") + "_" + (Game1.player.IsMale ? "Male" : "Female")], (object) Seed) : string.Format(dictionary["passedOut4"], (object) Seed);
              break;
            case 1:
              mail = string.Format(dictionary["passedOut2"], (object) Seed);
              break;
            case 2:
              mail = string.Format(dictionary["passedOut3_" + (Seed > 0 ? "Billed" : "NotBilled")], (object) Seed);
              break;
          }
        }
        else if (str.StartsWith("passedOut"))
        {
          string[] strArray = str.Split(' ');
          if (strArray.Length > 1)
          {
            int int32 = Convert.ToInt32(strArray[1]);
            mail = string.Format(dictionary[strArray[0]], (object) int32);
          }
        }
        if (mail.Length == 0)
          return;
        Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(mail, str);
      }
      else
      {
        if (Game1.mailbox.Count != 0)
          return;
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8429"));
      }
    }

    public void farmerFile() => Game1.multipleDialogues(new string[2]
    {
      Game1.content.LoadString("Strings\\UI:FarmerFile_1", (object) Game1.player.Name, (object) Game1.stats.StepsTaken, (object) Game1.stats.GiftsGiven, (object) Game1.stats.DaysPlayed, (object) Game1.stats.DirtHoed, (object) Game1.stats.ItemsCrafted, (object) Game1.stats.ItemsCooked, (object) Game1.stats.PiecesOfTrashRecycled).Replace('\n', '^'),
      Game1.content.LoadString("Strings\\UI:FarmerFile_2", (object) Game1.stats.MonstersKilled, (object) Game1.stats.FishCaught, (object) Game1.stats.TimesFished, (object) Game1.stats.SeedsSown, (object) Game1.stats.ItemsShipped).Replace('\n', '^')
    });

    public void openItemChest(Location location, int whichObject)
    {
      this.playSound("openBox");
      if (Game1.player.ActiveObject != null)
        return;
      if (whichObject == 434)
      {
        Game1.player.ActiveObject = new Object(Vector2.Zero, 434, "Cosmic Fruit", false, false, false, false);
        Game1.player.eatHeldObject();
      }
      else
        this.debris.Add(new Debris(whichObject, new Vector2((float) (location.X * 64), (float) (location.Y * 64)), Game1.player.Position));
      ++this.map.GetLayer("Buildings").Tiles[location.X, location.Y].TileIndex;
      this.map.GetLayer("Buildings").Tiles[location].Properties["Action"] = new PropertyValue("RemoveChest");
    }

    public void getWallDecorItem(Location location)
    {
    }

    public static string getFavoriteItemName(string character)
    {
      string favoriteItemName = "???";
      if (Game1.NPCGiftTastes.ContainsKey(character))
      {
        string[] strArray = Game1.NPCGiftTastes[character].Split('/')[1].Split(' ');
        favoriteItemName = Game1.objectInformation[Convert.ToInt32(strArray[Game1.random.Next(strArray.Length)])].Split('/')[0];
      }
      return favoriteItemName;
    }

    public static void openCraftingMenu(string nameOfCraftingDevice) => Game1.activeClickableMenu = (IClickableMenu) new GameMenu(4);

    private void openStorageBox(string which)
    {
    }

    public virtual bool openShopMenu(string which)
    {
      if (which.Equals("Fish"))
      {
        if (this.getCharacterFromName("Willy") == null || (double) this.getCharacterFromName("Willy").getTileLocation().Y >= (double) Game1.player.getTileY())
          return false;
        Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getFishShopStock(Game1.player), who: "Willy");
        return true;
      }
      if (this is SeedShop)
      {
        if (this.getCharacterFromName("Pierre") != null && this.getCharacterFromName("Pierre").getTileLocation().Equals(new Vector2(4f, 17f)) && Game1.player.getTileY() > this.getCharacterFromName("Pierre").getTileY())
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu((this as SeedShop).shopStock(), who: "Pierre");
        else if (this.getCharacterFromName("Pierre") == null && Game1.IsVisitingIslandToday("Pierre"))
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_MoneyBox"));
          Game1.afterDialogues = (Game1.afterFadeFunction) (() => Game1.activeClickableMenu = (IClickableMenu) new ShopMenu((this as SeedShop).shopStock()));
        }
        else
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8525"));
        return true;
      }
      if (!this.name.Equals((object) "SandyHouse"))
        return false;
      NPC characterFromName = this.getCharacterFromName("Sandy");
      if (characterFromName != null && characterFromName.currentLocation == this)
        Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(this.sandyShopStock(), who: "Sandy", on_purchase: new Func<ISalable, Farmer, int, bool>(this.onSandyShopPurchase));
      return true;
    }

    public virtual bool isObjectAt(int x, int y)
    {
      Vector2 key = new Vector2((float) (x / 64), (float) (y / 64));
      foreach (Object @object in this.furniture)
      {
        if (@object.boundingBox.Value.Contains(x, y))
          return true;
      }
      return this.objects.ContainsKey(key);
    }

    public virtual bool isObjectAtTile(int tileX, int tileY)
    {
      Vector2 key = new Vector2((float) tileX, (float) tileY);
      foreach (Object @object in this.furniture)
      {
        if (@object.boundingBox.Value.Contains(tileX * 64, tileY * 64))
          return true;
      }
      return this.objects.ContainsKey(key);
    }

    public virtual Object getObjectAt(int x, int y)
    {
      Vector2 key = new Vector2((float) (x / 64), (float) (y / 64));
      foreach (Furniture objectAt in this.furniture)
      {
        if (objectAt.boundingBox.Value.Contains(x, y))
          return (Object) objectAt;
      }
      return this.objects.ContainsKey(key) ? this.objects[key] : (Object) null;
    }

    public Object getObjectAtTile(int x, int y) => this.getObjectAt(x * 64, y * 64);

    private bool onSandyShopPurchase(ISalable item, Farmer who, int amount)
    {
      Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Sandy, item, amount);
      return false;
    }

    private Dictionary<ISalable, int[]> sandyShopStock()
    {
      Dictionary<ISalable, int[]> dictionary = new Dictionary<ISalable, int[]>();
      Utility.AddStock(dictionary, (Item) new Object(802, int.MaxValue), (int) (75.0 * (double) Game1.MasterPlayer.difficultyModifier));
      Utility.AddStock(dictionary, (Item) new Object(478, int.MaxValue));
      Utility.AddStock(dictionary, (Item) new Object(486, int.MaxValue));
      Utility.AddStock(dictionary, (Item) new Object(494, int.MaxValue));
      Dictionary<ISalable, int[]> stock = dictionary;
      Object @object = new Object(Vector2.Zero, 196);
      @object.Stack = int.MaxValue;
      Utility.AddStock(stock, (Item) @object);
      switch (Game1.dayOfMonth % 7)
      {
        case 0:
          Utility.AddStock(dictionary, (Item) new Object(233, int.MaxValue));
          break;
        case 1:
          Utility.AddStock(dictionary, (Item) new Object(88, 1), 200, 10);
          break;
        case 2:
          Utility.AddStock(dictionary, (Item) new Object(90, int.MaxValue));
          break;
        case 3:
          Utility.AddStock(dictionary, (Item) new Object(749, 1), 500, 3);
          break;
        case 4:
          Utility.AddStock(dictionary, (Item) new Object(466, int.MaxValue));
          break;
        case 5:
          Utility.AddStock(dictionary, (Item) new Object(340, int.MaxValue));
          break;
        case 6:
          Utility.AddStock(dictionary, (Item) new Object(371, int.MaxValue), 100);
          break;
      }
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      Clothing key = new Clothing(1000 + random.Next((int) sbyte.MaxValue));
      dictionary.Add((ISalable) key, new int[2]
      {
        1000,
        int.MaxValue
      });
      dictionary.Add((ISalable) new Furniture(2655, Vector2.Zero), new int[2]
      {
        700,
        int.MaxValue
      });
      switch (Game1.dayOfMonth % 7)
      {
        case 0:
          dictionary.Add((ISalable) new Furniture(2720, Vector2.Zero), new int[2]
          {
            3000,
            int.MaxValue
          });
          break;
        case 1:
          dictionary.Add((ISalable) new Furniture(2802, Vector2.Zero), new int[2]
          {
            2000,
            int.MaxValue
          });
          break;
        case 2:
          dictionary.Add((ISalable) new Furniture(2734 + random.Next(4) * 2, Vector2.Zero), new int[2]
          {
            500,
            int.MaxValue
          });
          break;
        case 3:
          dictionary.Add((ISalable) new Furniture(2584, Vector2.Zero), new int[2]
          {
            5000,
            int.MaxValue
          });
          break;
        case 4:
          dictionary.Add((ISalable) new Furniture(2794, Vector2.Zero), new int[2]
          {
            2500,
            int.MaxValue
          });
          break;
        case 5:
          dictionary.Add((ISalable) new Furniture(2784, Vector2.Zero), new int[2]
          {
            2500,
            int.MaxValue
          });
          break;
        case 6:
          dictionary.Add((ISalable) new Furniture(2748, Vector2.Zero), new int[2]
          {
            500,
            int.MaxValue
          });
          dictionary.Add((ISalable) new Furniture(2812, Vector2.Zero), new int[2]
          {
            500,
            int.MaxValue
          });
          break;
      }
      Game1.player.team.synchronizedShopStock.UpdateLocalStockWithSyncedQuanitities(SynchronizedShopStock.SynchedShop.Sandy, dictionary);
      return dictionary;
    }

    public virtual bool saloon(Location tileLocation)
    {
      foreach (NPC character in this.characters)
      {
        if (character.Name.Equals("Gus"))
        {
          if (character.getTileY() != Game1.player.getTileY() - 1 && character.getTileY() != Game1.player.getTileY() - 2)
            return false;
          character.facePlayer(Game1.player);
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getSaloonStock(), who: "Gus", on_purchase: ((Func<ISalable, Farmer, int, bool>) ((item, farmer, amount) =>
          {
            Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Saloon, item, amount);
            return false;
          })));
          return true;
        }
      }
      if (this.getCharacterFromName("Gus") != null || !Game1.IsVisitingIslandToday("Gus"))
        return false;
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_MoneyBox"));
      Game1.afterDialogues = (Game1.afterFadeFunction) (() => Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getSaloonStock(), on_purchase: ((Func<ISalable, Farmer, int, bool>) ((item, farmer, amount) =>
      {
        Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Saloon, item, amount);
        return false;
      }))));
      return true;
    }

    private void adventureShop()
    {
      if (Game1.player.itemsLostLastDeath.Count > 0)
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:AdventureGuild_Greeting"), new List<Response>()
        {
          new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")),
          new Response("Recovery", Game1.content.LoadString("Strings\\Locations:AdventureGuild_ItemRecovery")),
          new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave"))
        }.ToArray(), "adventureGuild");
      else
        Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getAdventureShopStock(), who: "Marlon");
    }

    public virtual bool carpenters(Location tileLocation)
    {
      if (Game1.player.currentUpgrade != null)
        return false;
      foreach (NPC character in this.characters)
      {
        if (character.Name.Equals("Robin"))
        {
          if ((double) Vector2.Distance(character.getTileLocation(), new Vector2((float) tileLocation.X, (float) tileLocation.Y)) > 3.0)
            return false;
          character.faceDirection(2);
          if ((int) (NetFieldBase<int, NetInt>) Game1.player.daysUntilHouseUpgrade < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
          {
            List<Response> responseList = new List<Response>();
            responseList.Add(new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")));
            if (Game1.IsMasterGame)
            {
              if ((int) (NetFieldBase<int, NetInt>) Game1.player.houseUpgradeLevel < 3)
                responseList.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
              else if ((Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter()) && (int) (NetFieldBase<int, NetInt>) (Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade <= 0)
              {
                if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                  responseList.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
                  responseList.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
              }
            }
            else if ((int) (NetFieldBase<int, NetInt>) Game1.player.houseUpgradeLevel < 3)
              responseList.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
            if ((int) (NetFieldBase<int, NetInt>) Game1.player.houseUpgradeLevel >= 2)
            {
              if (Game1.IsMasterGame)
                responseList.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse")));
              else
                responseList.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
            }
            responseList.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
            responseList.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), responseList.ToArray(), "carpenter");
          }
          else
            Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getCarpenterStock(), who: "Robin");
          return true;
        }
      }
      if (this.getCharacterFromName("Robin") == null && Game1.IsVisitingIslandToday("Robin"))
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_MoneyBox"));
        Game1.afterDialogues = (Game1.afterFadeFunction) (() => Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getCarpenterStock()));
        return true;
      }
      if (!Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue"))
        return false;
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
      return true;
    }

    public virtual bool blacksmith(Location tileLocation)
    {
      foreach (NPC character in this.characters)
      {
        if (character.Name.Equals("Clint"))
        {
          Vector2 tileLocation1 = character.getTileLocation();
          if (!tileLocation1.Equals(new Vector2((float) tileLocation.X, (float) (tileLocation.Y - 1))))
          {
            tileLocation1 = character.getTileLocation();
            tileLocation1.Equals(new Vector2((float) (tileLocation.X - 1), (float) (tileLocation.Y - 1)));
          }
          character.faceDirection(2);
          if (Game1.player.toolBeingUpgraded.Value == null)
          {
            Response[] answerChoices;
            if (Game1.player.hasItemInInventory(535, 1) || Game1.player.hasItemInInventory(536, 1) || Game1.player.hasItemInInventory(537, 1) || Game1.player.hasItemInInventory(749, 1) || Game1.player.hasItemInInventory(275, 1) || Game1.player.hasItemInInventory(791, 1))
              answerChoices = new Response[4]
              {
                new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
                new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")),
                new Response("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")),
                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
              };
            else
              answerChoices = new Response[3]
              {
                new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
                new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")),
                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
              };
            this.createQuestionDialogue("", answerChoices, "Blacksmith");
          }
          else if ((int) (NetFieldBase<int, NetInt>) Game1.player.daysLeftForToolUpgrade <= 0)
          {
            if (Game1.player.freeSpotsInInventory() > 0 || Game1.player.toolBeingUpgraded.Value is GenericTool)
            {
              Tool tool = Game1.player.toolBeingUpgraded.Value;
              Game1.player.toolBeingUpgraded.Value = (Tool) null;
              Game1.player.hasReceivedToolUpgradeMessageYet = false;
              Game1.player.holdUpItemThenMessage((Item) tool);
              if (tool is GenericTool)
                tool.actionWhenClaimed();
              else
                Game1.player.addItemToInventoryBool((Item) tool);
              if (Game1.player.team.useSeparateWallets.Value && tool.UpgradeLevel == 4)
                Game1.multiplayer.globalChatInfoMessage("IridiumToolUpgrade", Game1.player.Name, tool.DisplayName);
            }
            else
              Game1.drawDialogue(character, Game1.content.LoadString("Data\\ExtraDialogue:Clint_NoInventorySpace"));
          }
          else
            Game1.drawDialogue(character, Game1.content.LoadString("Data\\ExtraDialogue:Clint_StillWorking", (object) Game1.player.toolBeingUpgraded.Value.DisplayName));
          return true;
        }
      }
      return false;
    }

    public virtual bool animalShop(Location tileLocation)
    {
      foreach (NPC character in this.characters)
      {
        if (character.Name.Equals("Marnie"))
        {
          if (!character.getTileLocation().Equals(new Vector2((float) tileLocation.X, (float) (tileLocation.Y - 1))))
            return false;
          character.faceDirection(2);
          this.createQuestionDialogue("", new Response[3]
          {
            new Response("Supplies", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Supplies")),
            new Response("Purchase", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals")),
            new Response("Leave", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Leave"))
          }, "Marnie");
          return true;
        }
      }
      if (this.getCharacterFromName("Marnie") == null && Game1.IsVisitingIslandToday("Marnie"))
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_MoneyBox"));
        Game1.afterDialogues = (Game1.afterFadeFunction) (() => Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getAnimalShopStock()));
        return true;
      }
      if (!Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue"))
        return false;
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Absent").Replace('\n', '^'));
      return true;
    }

    private void gunther()
    {
      if ((this as LibraryMuseum).doesFarmerHaveAnythingToDonate(Game1.player))
      {
        Response[] answerChoices;
        if ((this as LibraryMuseum).getRewardsForPlayer(Game1.player).Count > 0)
          answerChoices = new Response[3]
          {
            new Response("Donate", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Donate")),
            new Response("Collect", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Collect")),
            new Response("Leave", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Leave"))
          };
        else
          answerChoices = new Response[2]
          {
            new Response("Donate", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Donate")),
            new Response("Leave", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Leave"))
          };
        this.createQuestionDialogue("", answerChoices, "Museum");
      }
      else if ((this as LibraryMuseum).getRewardsForPlayer(Game1.player).Count > 0)
        this.createQuestionDialogue("", new Response[2]
        {
          new Response("Collect", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Collect")),
          new Response("Leave", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Leave"))
        }, "Museum");
      else if (Game1.player.achievements.Contains(5))
        Game1.drawDialogue(Game1.getCharacterFromName("Gunther"), Game1.parseText(Game1.content.LoadString("Data\\ExtraDialogue:Gunther_MuseumComplete")));
      else
        Game1.drawDialogue(Game1.getCharacterFromName("Gunther"), Game1.player.mailReceived.Contains("artifactFound") ? Game1.parseText(Game1.content.LoadString("Data\\ExtraDialogue:Gunther_NothingToDonate")) : Game1.content.LoadString("Data\\ExtraDialogue:Gunther_NoArtifactsFound"));
    }

    public void openChest(Location location, int debrisType, int numberOfChunks)
    {
      int[] debrisType1 = new int[1]{ debrisType };
      this.openChest(location, debrisType1, numberOfChunks);
    }

    public void openChest(Location location, int[] debrisType, int numberOfChunks)
    {
      this.playSound("openBox");
      ++this.map.GetLayer("Buildings").Tiles[location.X, location.Y].TileIndex;
      for (int index = 0; index < debrisType.Length; ++index)
        Game1.createDebris(debrisType[index], location.X, location.Y, numberOfChunks);
      this.map.GetLayer("Buildings").Tiles[location].Properties["Action"] = new PropertyValue("RemoveChest");
    }

    public string actionParamsToString(string[] actionparams)
    {
      string str = actionparams[1];
      for (int index = 2; index < actionparams.Length; ++index)
        str = str + " " + actionparams[index];
      return str;
    }

    private void GetLumber()
    {
      if (Game1.player.ActiveObject == null && Game1.player.WoodPieces > 0)
      {
        Game1.player.grabObject(new Object(Vector2.Zero, 30, "Lumber", true, true, false, false));
        --Game1.player.WoodPieces;
      }
      else
      {
        if (Game1.player.ActiveObject == null || !Game1.player.ActiveObject.Name.Equals("Lumber"))
          return;
        Game1.player.CanMove = false;
        switch (Game1.player.FacingDirection)
        {
          case 0:
            ((FarmerSprite) Game1.player.Sprite).animateBackwardsOnce(80, 75f);
            break;
          case 1:
            ((FarmerSprite) Game1.player.Sprite).animateBackwardsOnce(72, 75f);
            break;
          case 2:
            ((FarmerSprite) Game1.player.Sprite).animateBackwardsOnce(64, 75f);
            break;
          case 3:
            ((FarmerSprite) Game1.player.Sprite).animateBackwardsOnce(88, 75f);
            break;
        }
        Game1.player.ActiveObject = (Object) null;
        ++Game1.player.WoodPieces;
      }
    }

    public void removeTile(Location tileLocation, string layer) => this.Map.GetLayer(layer).Tiles[tileLocation.X, tileLocation.Y] = (Tile) null;

    public void removeTile(int x, int y, string layer) => this.Map.GetLayer(layer).Tiles[x, y] = (Tile) null;

    public void characterTrampleTile(Vector2 tile)
    {
      switch (this)
      {
        case FarmHouse _:
          break;
        case IslandFarmHouse _:
          break;
        case Farm _:
          break;
        default:
          TerrainFeature terrainFeature;
          this.terrainFeatures.TryGetValue(tile, out terrainFeature);
          if (terrainFeature == null || !(terrainFeature is Tree) || (int) (NetFieldBase<int, NetInt>) (terrainFeature as Tree).growthStage >= 1 || !(terrainFeature as Tree).instantDestroy(tile, this))
            break;
          this.terrainFeatures.Remove(tile);
          break;
      }
    }

    public bool characterDestroyObjectWithinRectangle(Microsoft.Xna.Framework.Rectangle rect, bool showDestroyedObject)
    {
      switch (this)
      {
        case FarmHouse _:
        case IslandFarmHouse _:
          return false;
        default:
          foreach (Farmer farmer in this.farmers)
          {
            if (rect.Intersects(farmer.GetBoundingBox()))
              return false;
          }
          Vector2 vector2 = new Vector2((float) (rect.X / 64), (float) (rect.Y / 64));
          Object o;
          this.objects.TryGetValue(vector2, out o);
          if (this.checkDestroyItem(o, vector2, showDestroyedObject))
            return true;
          TerrainFeature tf;
          this.terrainFeatures.TryGetValue(vector2, out tf);
          if (this.checkDestroyTerrainFeature(tf, vector2))
            return true;
          vector2.X = (float) (rect.Right / 64);
          this.objects.TryGetValue(vector2, out o);
          if (this.checkDestroyItem(o, vector2, showDestroyedObject))
            return true;
          this.terrainFeatures.TryGetValue(vector2, out tf);
          if (this.checkDestroyTerrainFeature(tf, vector2))
            return true;
          vector2.X = (float) (rect.X / 64);
          vector2.Y = (float) (rect.Bottom / 64);
          this.objects.TryGetValue(vector2, out o);
          if (this.checkDestroyItem(o, vector2, showDestroyedObject))
            return true;
          this.terrainFeatures.TryGetValue(vector2, out tf);
          if (this.checkDestroyTerrainFeature(tf, vector2))
            return true;
          vector2.X = (float) (rect.Right / 64);
          this.objects.TryGetValue(vector2, out o);
          if (this.checkDestroyItem(o, vector2, showDestroyedObject))
            return true;
          this.terrainFeatures.TryGetValue(vector2, out tf);
          return this.checkDestroyTerrainFeature(tf, vector2);
      }
    }

    private bool checkDestroyTerrainFeature(TerrainFeature tf, Vector2 tilePositionToTry)
    {
      if (tf != null && tf is Tree && (tf as Tree).instantDestroy(tilePositionToTry, this))
        this.terrainFeatures.Remove(tilePositionToTry);
      return false;
    }

    private bool checkDestroyItem(Object o, Vector2 tilePositionToTry, bool showDestroyedObject)
    {
      if (o == null || o.IsHoeDirt || o.isPassable() || this.map.GetLayer("Back").Tiles[(int) tilePositionToTry.X, (int) tilePositionToTry.Y].Properties.ContainsKey("NPCBarrier"))
        return false;
      if (!o.IsHoeDirt)
      {
        if (o.IsSpawnedObject)
          --this.numberOfSpawnedObjectsOnMap;
        if (showDestroyedObject && !(bool) (NetFieldBase<bool, NetBool>) o.bigCraftable)
          Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(o.ParentSheetIndex, 150f, 1, 3, new Vector2(tilePositionToTry.X * 64f, tilePositionToTry.Y * 64f), false, (bool) (NetFieldBase<bool, NetBool>) o.flipped)
          {
            alphaFade = 0.01f
          });
        o.performToolAction((Tool) null, this);
        if (this.objects.ContainsKey(tilePositionToTry))
        {
          if (o is Chest)
          {
            Chest chest = o as Chest;
            if ((chest.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin || chest.SpecialChestType == Chest.SpecialChestTypes.JunimoChest) && chest.MoveToSafePosition(this, tilePositionToTry))
              return true;
            chest.destroyAndDropContents(tilePositionToTry * 64f, this);
          }
          this.objects.Remove(tilePositionToTry);
        }
      }
      return true;
    }

    public Object removeObject(Vector2 location, bool showDestroyedObject)
    {
      Object object1;
      this.objects.TryGetValue(location, out object1);
      if (object1 == null || !(object1.CanBeGrabbed | showDestroyedObject))
        return (Object) null;
      if (object1.IsSpawnedObject)
        --this.numberOfSpawnedObjectsOnMap;
      Object object2 = this.objects[location];
      this.objects.Remove(location);
      if (showDestroyedObject)
        Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(object2.Type.Equals("Crafting") ? object2.ParentSheetIndex : object2.ParentSheetIndex + 1, 150f, 1, 3, new Vector2(location.X * 64f, location.Y * 64f), true, (bool) (NetFieldBase<bool, NetBool>) object2.bigCraftable, (bool) (NetFieldBase<bool, NetBool>) object2.flipped));
      if (object1.Name.Contains("Weed"))
        ++Game1.stats.WeedsEliminated;
      return object2;
    }

    public void removeTileProperty(int tileX, int tileY, string layer, string key)
    {
      try
      {
        if (this.map == null)
          return;
        Layer layer1 = this.map.GetLayer(layer);
        if (layer1 == null)
          return;
        Tile tile = layer1.Tiles[tileX, tileY];
        if (tile == null)
          return;
        IPropertyCollection properties = tile.Properties;
        if (!properties.ContainsKey(key))
          return;
        ((IDictionary<string, PropertyValue>) properties).Remove(key);
      }
      catch (Exception ex)
      {
      }
    }

    public void setTileProperty(int tileX, int tileY, string layer, string key, string value)
    {
      try
      {
        if (this.map == null)
          return;
        Layer layer1 = this.map.GetLayer(layer);
        if (layer1 == null)
          return;
        Tile tile = layer1.Tiles[tileX, tileY];
        if (tile == null)
          return;
        IPropertyCollection properties = tile.Properties;
        if (!properties.ContainsKey(key))
          properties.Add(key, new PropertyValue(value));
        else
          properties[key] = (PropertyValue) value;
      }
      catch (Exception ex)
      {
      }
    }

    private void removeDirt(Vector2 location)
    {
      Object @object;
      this.objects.TryGetValue(location, out @object);
      if (@object == null || !@object.IsHoeDirt)
        return;
      this.objects.Remove(location);
    }

    public void removeBatch(List<Vector2> locations)
    {
      foreach (Vector2 location in locations)
        this.objects.Remove(location);
    }

    public void setObjectAt(float x, float y, Object o)
    {
      Vector2 key = new Vector2(x, y);
      if (this.objects.ContainsKey(key))
        this.objects[key] = o;
      else
        this.objects.Add(key, o);
    }

    public virtual void cleanupBeforeSave()
    {
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        if (this.characters[index] is Junimo)
          this.characters.RemoveAt(index);
      }
      if (!this.name.Equals((object) "WitchHut"))
        return;
      this.characters.Clear();
    }

    public virtual void cleanupForVacancy()
    {
      int index = 0;
      while (index < this.debris.Count)
      {
        Debris debri = this.debris[index];
        if (debri.isEssentialItem() && Game1.IsMasterGame && debri.collect(Game1.player))
          this.debris.RemoveAt(index);
        else
          ++index;
      }
    }

    public virtual void cleanupBeforePlayerExit()
    {
      int index1 = 0;
      while (index1 < this.debris.Count)
      {
        Debris debri = this.debris[index1];
        if (debri.isEssentialItem() && debri.player.Value != null && debri.player.Value == Game1.player && debri.collect((Farmer) debri.player))
          this.debris.RemoveAt(index1);
        else
          ++index1;
      }
      Game1.currentLightSources.Clear();
      if (this.critters != null)
        this.critters.Clear();
      for (int index2 = Game1.onScreenMenus.Count - 1; index2 >= 0; --index2)
      {
        IClickableMenu onScreenMenu = Game1.onScreenMenus[index2];
        if (onScreenMenu.destroy)
        {
          Game1.onScreenMenus.RemoveAt(index2);
          if (onScreenMenu is IDisposable)
            (onScreenMenu as IDisposable).Dispose();
        }
      }
      if (Game1.getMusicTrackName() == "sam_acoustic1")
        Game1.changeMusicTrack("none");
      bool flag = Game1.locationRequest == null || Game1.locationRequest.Location == null || Game1.locationRequest.Location.IsOutdoors;
      if (((Game1.getMusicTrackName().Contains(Game1.currentSeason) || Game1.getMusicTrackName().Contains("night_ambient") || Game1.getMusicTrackName().Contains("day_ambient") || Game1.getMusicTrackName().Equals("rain") || Game1.eventUp ? 0 : ((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors ? 1 : 0)) & (flag ? 1 : 0)) != 0 && this.GetLocationContext() == GameLocation.LocationContext.Default)
        Game1.changeMusicTrack("none");
      AmbientLocationSounds.onLocationLeave();
      if (((this.name.Equals((object) "AnimalShop") || this.name.Equals((object) "ScienceHouse") ? (Game1.getMusicTrackName().Equals("marnieShop") ? 1 : 0) : 0) & (flag ? 1 : 0)) != 0)
        Game1.changeMusicTrack("none");
      if (this.name.Equals((object) "Saloon") && (Game1.getMusicTrackName().Contains("Saloon") || Game1.startedJukeboxMusic))
        Game1.changeMusicTrack("none");
      if (this.name.Equals((object) "LeahHouse"))
        Game1.changeMusicTrack("none");
      if (this.name.Equals((object) "ElliottHouse"))
        Game1.changeMusicTrack("none");
      if (this.name.Equals((object) "IslandSouthEastCave"))
        Game1.changeMusicTrack("none");
      if (this.name.Equals((object) "IslandFarmCave"))
        Game1.changeMusicTrack("none");
      if (this.name.Equals((object) "IslandNorthCave1"))
        Game1.changeMusicTrack("none");
      if (this.name.Equals((object) "IslandWestCave1"))
        Game1.changeMusicTrack("none");
      if (this.name.Equals((object) "IslandFieldOffice"))
        Game1.changeMusicTrack("none");
      switch (this)
      {
        case LibraryMuseum _:
        case JojaMart _:
          Game1.changeMusicTrack("none");
          break;
      }
      Game1.startedJukeboxMusic = false;
      if (((!this.name.Equals((object) "Hospital") ? 0 : (Game1.getMusicTrackName().Equals("distantBanjo") ? 1 : 0)) & (flag ? 1 : 0)) != 0)
        Game1.changeMusicTrack("none");
      if (Game1.player.rightRing.Value != null)
        Game1.player.rightRing.Value.onLeaveLocation(Game1.player, this);
      if (Game1.player.leftRing.Value != null)
        Game1.player.leftRing.Value.onLeaveLocation(Game1.player, this);
      if (Game1.locationRequest == null || (string) (NetFieldBase<string, NetString>) this.name != Game1.locationRequest.Name)
      {
        foreach (NPC character in this.characters)
        {
          if (character.uniqueSpriteActive)
          {
            character.Sprite.LoadTexture("Characters\\" + NPC.getTextureNameForCharacter(character.Name));
            character.uniqueSpriteActive = false;
          }
          if (character.uniquePortraitActive)
          {
            character.Portrait = Game1.content.Load<Texture2D>("Portraits\\" + NPC.getTextureNameForCharacter(character.Name));
            character.uniquePortraitActive = false;
          }
        }
      }
      if (this.name.Equals((object) "AbandonedJojaMart"))
      {
        if (this.farmers.Count <= 1)
        {
          for (int index3 = this.characters.Count - 1; index3 >= 0; --index3)
          {
            if (this.characters[index3] is Junimo)
              this.characters.RemoveAt(index3);
          }
        }
        Game1.changeMusicTrack("none");
      }
      this.furnitureToRemove.Clear();
      Game1.stopMusicTrack(Game1.MusicContext.SubLocation);
      this.interiorDoors.CleanUpLocalState();
      Game1.temporaryContent.Unload();
      Utility.CollectGarbage();
    }

    public static int getWeedForSeason(Random r, string season)
    {
      if (!(season == "spring"))
      {
        if (!(season == " summer"))
        {
          if (!(season == "fall"))
          {
            if (season == "winter")
              ;
            return 674;
          }
          if (r.NextDouble() < 0.33)
            return 786;
          return r.NextDouble() >= 0.5 ? 679 : 678;
        }
        if (r.NextDouble() < 0.33)
          return 785;
        return r.NextDouble() >= 0.5 ? 677 : 676;
      }
      if (r.NextDouble() < 0.33)
        return 784;
      return r.NextDouble() >= 0.5 ? 675 : 674;
    }

    private void startSleep()
    {
      Game1.player.timeWentToBed.Value = Game1.timeOfDay;
      if (Game1.IsMultiplayer)
      {
        Game1.player.team.SetLocalReady("sleep", true);
        Game1.dialogueUp = false;
        Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog("sleep", true, (ConfirmationDialog.behavior) (who => this.doSleep()), (ConfirmationDialog.behavior) (who =>
        {
          if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ReadyCheckDialog)
            (Game1.activeClickableMenu as ReadyCheckDialog).closeDialog(who);
          who.timeWentToBed.Value = 0;
        }));
      }
      else
        this.doSleep();
      if (Game1.player.team.announcedSleepingFarmers.Contains(Game1.player))
        return;
      Game1.player.team.announcedSleepingFarmers.Add(Game1.player);
      if (!Game1.IsMultiplayer || (FarmerTeam.SleepAnnounceModes) (NetFieldBase<FarmerTeam.SleepAnnounceModes, NetEnum<FarmerTeam.SleepAnnounceModes>>) Game1.player.team.sleepAnnounceMode != FarmerTeam.SleepAnnounceModes.All && ((FarmerTeam.SleepAnnounceModes) (NetFieldBase<FarmerTeam.SleepAnnounceModes, NetEnum<FarmerTeam.SleepAnnounceModes>>) Game1.player.team.sleepAnnounceMode != FarmerTeam.SleepAnnounceModes.First || Game1.player.team.announcedSleepingFarmers.Count<Farmer>() != 1))
        return;
      string str = "GoneToBed";
      if (Game1.random.NextDouble() < 0.75)
      {
        if (Game1.timeOfDay < 1800)
          str += "Early";
        else if (Game1.timeOfDay > 2530)
          str += "Late";
      }
      int num = 0;
      for (int index = 0; index < 2; ++index)
      {
        if (Game1.random.NextDouble() < 0.25)
          ++num;
      }
      Game1.multiplayer.globalChatInfoMessage(str + num.ToString(), Game1.player.displayName);
    }

    private void doSleep()
    {
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.lightLevel == 0.0 && Game1.timeOfDay < 2000)
      {
        this.lightLevel.Value = 0.6f;
        this.localSound("openBox");
        if (Game1.IsMasterGame)
          Game1.NewDay(600f);
      }
      else if ((double) (float) (NetFieldBase<float, NetFloat>) this.lightLevel > 0.0 && Game1.timeOfDay >= 2000)
      {
        this.lightLevel.Value = 0.0f;
        this.localSound("openBox");
        if (Game1.IsMasterGame)
          Game1.NewDay(600f);
      }
      else if (Game1.IsMasterGame)
        Game1.NewDay(0.0f);
      Game1.player.lastSleepLocation.Value = Game1.currentLocation.NameOrUniqueName;
      Game1.player.lastSleepPoint.Value = Game1.player.getTileLocationPoint();
      Game1.player.mostRecentBed = Game1.player.Position;
      Game1.player.doEmote(24);
      Game1.player.freezePause = 2000;
    }

    public virtual bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
    {
      switch (questionAndAnswer)
      {
        case "Backpack_Purchase":
          if ((int) (NetFieldBase<int, NetInt>) Game1.player.maxItems == 12 && Game1.player.Money >= 2000)
          {
            Game1.player.Money -= 2000;
            Game1.player.maxItems.Value += 12;
            for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) Game1.player.maxItems; ++index)
            {
              if (Game1.player.items.Count <= index)
                Game1.player.items.Add((Item) null);
            }
            Game1.player.holdUpItemThenMessage((Item) new SpecialItem(99, Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8708")));
            Game1.multiplayer.globalChatInfoMessage("BackpackLarge", Game1.player.Name);
            break;
          }
          if ((int) (NetFieldBase<int, NetInt>) Game1.player.maxItems < 36 && Game1.player.Money >= 10000)
          {
            Game1.player.Money -= 10000;
            Game1.player.maxItems.Value += 12;
            Game1.player.holdUpItemThenMessage((Item) new SpecialItem(99, Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8709")));
            for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) Game1.player.maxItems; ++index)
            {
              if (Game1.player.items.Count <= index)
                Game1.player.items.Add((Item) null);
            }
            Game1.multiplayer.globalChatInfoMessage("BackpackDeluxe", Game1.player.Name);
            break;
          }
          if ((int) (NetFieldBase<int, NetInt>) Game1.player.maxItems != 36)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney2"));
            break;
          }
          break;
        case "Backpack_Yes":
          this.tryToBuyNewBackpack();
          break;
        case "Blacksmith_Process":
          Game1.activeClickableMenu = (IClickableMenu) new GeodeMenu();
          break;
        case "Blacksmith_Shop":
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getBlacksmithStock(), who: "Clint");
          break;
        case "Blacksmith_Upgrade":
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getBlacksmithUpgradeStock(Game1.player), who: "ClintUpgrade");
          break;
        case "Bouquet_Yes":
          if (Game1.player.Money >= 500)
          {
            if (Game1.player.ActiveObject == null)
            {
              Game1.player.Money -= 500;
              Game1.player.grabObject(new Object(Vector2.Zero, 458, (string) null, false, true, false, false));
              return true;
            }
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
          break;
        case "BuyClubCoins_Yes":
          if (Game1.player.Money >= 1000)
          {
            Game1.player.Money -= 1000;
            Game1.player.clubCoins += 10;
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
          break;
        case "BuyQiCoins_Yes":
          if (Game1.player.Money >= 1000)
          {
            Game1.player.Money -= 1000;
            this.localSound("Pickup_Coin15");
            Game1.player.clubCoins += 100;
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8715"));
          break;
        case "CalicoJackHS_Play":
          if (Game1.player.clubCoins >= 1000)
          {
            Game1.currentMinigame = (IMinigame) new CalicoJack(highStakes: true);
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJackHS_NotEnoughCoins"));
          break;
        case "CalicoJack_Play":
          if (Game1.player.clubCoins >= 100)
          {
            Game1.currentMinigame = (IMinigame) new CalicoJack();
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_NotEnoughCoins"));
          break;
        case "CalicoJack_Rules":
          Game1.multipleDialogues(new string[2]
          {
            Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Rules1"),
            Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Rules2")
          });
          break;
        case "ClearHouse_Yes":
          Vector2 tileLocation = Game1.player.getTileLocation();
          foreach (Vector2 adjacentTilesOffset in Character.AdjacentTilesOffsets)
          {
            Vector2 key = tileLocation + adjacentTilesOffset;
            if (this.objects.ContainsKey(key))
              this.objects.Remove(key);
          }
          break;
        case "ClubCard_That's":
        case "ClubCard_Yes.":
          this.playSound("explosion");
          Game1.flashAlpha = 5f;
          this.characters.Remove(this.getCharacterFromName("Bouncer"));
          if (this.characters.Count > 0)
          {
            this.characters[0].faceDirection(1);
            this.characters[0].setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Sandy_PlayerClubMember"));
            this.characters[0].doEmote(16);
          }
          Game1.pauseThenMessage(500, Game1.content.LoadString("Strings\\Locations:Club_Bouncer_PlayerClubMember"), false);
          Game1.player.Halt();
          Game1.getCharacterFromName("Mister Qi").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:MisterQi_PlayerClubMember"));
          break;
        case "ClubSeller_I'll":
          if (Game1.player.Money >= 1000000)
          {
            Game1.player.Money -= 1000000;
            Game1.exitActiveMenu();
            Game1.player.forceCanMove();
            Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item) new Object(Vector2.Zero, (int) sbyte.MaxValue));
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_NotEnoughMoney"));
          break;
        case "CowboyGame_Continue":
          Game1.currentMinigame = (IMinigame) new AbigailGame();
          break;
        case "CowboyGame_NewGame":
          Game1.player.jotpkProgress.Value = (AbigailGame.JOTPKProgress) null;
          Game1.currentMinigame = (IMinigame) new AbigailGame();
          break;
        case "Drum_Change":
          Game1.drawItemNumberSelection("drumTone", -1);
          break;
        case "Dungeon_Go":
          Game1.enterMine(Game1.CurrentMineLevel + 1);
          break;
        case "Eat_No":
          Game1.player.isEating = false;
          Game1.player.completelyStopAnimatingOrDoingAction();
          break;
        case "Eat_Yes":
          Game1.player.isEating = false;
          Game1.player.eatHeldObject();
          break;
        case "EnterTheaterSpendTicket_Yes":
          Game1.player.removeItemsFromInventory(809, 1);
          Rumble.rumble(0.15f, 200f);
          Game1.player.completelyStopAnimatingOrDoingAction();
          this.playSoundAt("doorClose", Game1.player.getTileLocation());
          Game1.warpFarmer("MovieTheater", 13, 15, 0);
          break;
        case "EnterTheater_Yes":
          Rumble.rumble(0.15f, 200f);
          Game1.player.completelyStopAnimatingOrDoingAction();
          this.playSoundAt("doorClose", Game1.player.getTileLocation());
          Game1.warpFarmer("MovieTheater", 13, 15, 0);
          break;
        case "ExitMine_Go":
          Game1.enterMine(Game1.CurrentMineLevel - 1);
          break;
        case "ExitMine_Leave":
        case "ExitMine_Yes":
          if (Game1.CurrentMineLevel == 77377)
            Game1.warpFarmer("Mine", 67, 10, true);
          else if (Game1.CurrentMineLevel > 120)
            Game1.warpFarmer("SkullCave", 3, 4, 2);
          else
            Game1.warpFarmer("Mine", 23, 8, false);
          Game1.changeMusicTrack("none");
          break;
        case "ExitToTitle_Yes":
          Game1.fadeScreenToBlack();
          Game1.exitToTitle = true;
          break;
        case "Flute_Change":
          Game1.drawItemNumberSelection("flutePitch", -1);
          break;
        case "Mariner_Buy":
          if (Game1.player.Money >= 5000)
          {
            Game1.player.Money -= 5000;
            Game1.player.grabObject(new Object(Vector2.Zero, 460, (string) null, false, true, false, false));
            return true;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
          break;
        case "Marnie_Purchase":
          Game1.player.forceCanMove();
          Game1.activeClickableMenu = (IClickableMenu) new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock());
          break;
        case "Marnie_Supplies":
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getAnimalShopStock(), who: "Marnie");
          break;
        case "Mine_Enter":
          Game1.enterMine(1);
          break;
        case "Mine_No":
          Response[] answerChoices = new Response[2]
          {
            new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")),
            new Response("Yes", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes"))
          };
          this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Mines_ResetMine")), answerChoices, "ResetMine");
          break;
        case "Mine_Return":
          Game1.enterMine(Game1.player.deepestMineLevel);
          break;
        case "Mine_Yes":
          if (Game1.CurrentMineLevel > 120)
          {
            Game1.warpFarmer("SkullCave", 3, 4, 2);
            break;
          }
          Game1.warpFarmer("UndergroundMine", 16, 16, false);
          break;
        case "MinecartGame_Endless":
          Game1.currentMinigame = (IMinigame) new MineCart(0, 2);
          break;
        case "MinecartGame_Progress":
          Game1.currentMinigame = (IMinigame) new MineCart(0, 3);
          break;
        case "Minecart_Bus":
          Game1.player.Halt();
          Game1.player.freezePause = 700;
          Game1.warpFarmer("BusStop", 4, 4, 2);
          break;
        case "Minecart_Mines":
          Game1.player.Halt();
          Game1.player.freezePause = 700;
          Game1.warpFarmer("Mine", 13, 9, 1);
          if (Game1.getMusicTrackName() == "springtown")
          {
            Game1.changeMusicTrack("none");
            break;
          }
          break;
        case "Minecart_Quarry":
          Game1.player.Halt();
          Game1.player.freezePause = 700;
          Game1.warpFarmer("Mountain", 124, 12, 2);
          break;
        case "Minecart_Town":
          Game1.player.Halt();
          Game1.player.freezePause = 700;
          Game1.warpFarmer("Town", 105, 80, 1);
          break;
        case "Quest_No":
          Game1.currentBillboard = 0;
          break;
        case "Quest_Yes":
          Game1.questOfTheDay.dailyQuest.Value = true;
          Game1.questOfTheDay.accept();
          Game1.currentBillboard = 0;
          Game1.player.questLog.Add(Game1.questOfTheDay);
          break;
        case "RemoveIncubatingEgg_Yes":
          Game1.player.ActiveObject = new Object(Vector2.Zero, (this as AnimalHouse).incubatingEgg.Y, (string) null, false, true, false, false);
          Game1.player.showCarrying();
          (this as AnimalHouse).incubatingEgg.Y = -1;
          this.map.GetLayer("Front").Tiles[1, 2].TileIndex = 45;
          break;
        case "Shaft_Jump":
          if (this is MineShaft)
          {
            (this as MineShaft).enterMineShaft();
            break;
          }
          break;
        case "ShrineOfChallenge_Yes":
          Game1.player.team.toggleMineShrineOvernight.Value = true;
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ChallengeShrine_Activated"));
          Game1.multiplayer.globalChatInfoMessage(!Game1.player.team.mineShrineActivated.Value ? "HardModeMinesActivated" : "HardModeMinesDeactivated", Game1.player.Name);
          DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
          {
            if (!Game1.player.team.mineShrineActivated.Value)
            {
              Game1.playSound("fireball");
              this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(8.75f, 5.8f) * 64f + new Vector2(32f, -32f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
              {
                interval = 50f,
                totalNumberOfLoops = 99999,
                animationLength = 4,
                light = true,
                lightID = 888,
                id = 888f,
                lightRadius = 2f,
                scale = 4f,
                yPeriodic = true,
                lightcolor = new Microsoft.Xna.Framework.Color(100, 0, 0),
                yPeriodicLoopTime = 1000f,
                yPeriodicRange = 4f,
                layerDepth = 0.04544f
              });
              this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(10.75f, 5.8f) * 64f + new Vector2(32f, -32f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
              {
                interval = 50f,
                totalNumberOfLoops = 99999,
                animationLength = 4,
                light = true,
                lightID = 889,
                id = 889f,
                lightRadius = 2f,
                scale = 4f,
                lightcolor = new Microsoft.Xna.Framework.Color(100, 0, 0),
                yPeriodic = true,
                yPeriodicLoopTime = 1100f,
                yPeriodicRange = 4f,
                layerDepth = 0.04544f
              });
            }
            else
            {
              this.removeTemporarySpritesWithID(888);
              this.removeTemporarySpritesWithID(889);
              Game1.playSound("fireball");
            }
          }), 500);
          break;
        case "Sleep_Yes":
          this.startSleep();
          break;
        case "Smelt_Copper":
          Game1.player.CopperPieces -= 10;
          this.smeltBar(new Object(Vector2.Zero, 334, "Copper Bar", false, true, false, false), 60);
          break;
        case "Smelt_Gold":
          Game1.player.GoldPieces -= 10;
          this.smeltBar(new Object(Vector2.Zero, 336, "Gold Bar", false, true, false, false), 300);
          break;
        case "Smelt_Iridium":
          Game1.player.IridiumPieces -= 10;
          this.smeltBar(new Object(Vector2.Zero, 337, "Iridium Bar", false, true, false, false), 1440);
          break;
        case "Smelt_Iron":
          Game1.player.IronPieces -= 10;
          this.smeltBar(new Object(Vector2.Zero, 335, "Iron Bar", false, true, false, false), 120);
          break;
        case "WizardShrine_Yes":
          if (Game1.player.Money >= 500)
          {
            Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(CharacterCustomization.Source.Wizard);
            Game1.player.Money -= 500;
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney2"));
          break;
        case "adventureGuild_Recovery":
          Game1.player.forceCanMove();
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getAdventureRecoveryStock(), who: "Marlon_Recovery");
          break;
        case "adventureGuild_Shop":
          Game1.player.forceCanMove();
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getAdventureShopStock(), who: "Marlon");
          break;
        case "buyJojaCola_Yes":
          if (Game1.player.Money >= 75)
          {
            Game1.player.Money -= 75;
            Game1.player.addItemByMenuIfNecessary((Item) new Object(167, 1));
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
          break;
        case "carpenter_CommunityUpgrade":
          this.communityUpgradeOffer();
          break;
        case "carpenter_Construct":
          Game1.activeClickableMenu = (IClickableMenu) new CarpenterMenu();
          break;
        case "carpenter_Renovate":
          Game1.player.forceCanMove();
          HouseRenovation.ShowRenovationMenu();
          break;
        case "carpenter_Shop":
          Game1.player.forceCanMove();
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getCarpenterStock(), who: "Robin");
          break;
        case "carpenter_Upgrade":
          this.houseUpgradeOffer();
          break;
        case "communityUpgrade_Yes":
          this.communityUpgradeAccept();
          break;
        case "dogStatue_Yes":
          if (Game1.player.Money < 10000)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            break;
          }
          List<Response> responseList1 = new List<Response>();
          if (GameLocation.canRespec(0))
            responseList1.Add(new Response("farming", Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604")));
          if (GameLocation.canRespec(3))
            responseList1.Add(new Response("mining", Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605")));
          if (GameLocation.canRespec(2))
            responseList1.Add(new Response("foraging", Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606")));
          if (GameLocation.canRespec(1))
            responseList1.Add(new Response("fishing", Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607")));
          if (GameLocation.canRespec(4))
            responseList1.Add(new Response("combat", Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608")));
          responseList1.Add(new Response("cancel", Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueCancel")));
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueQuestion"), responseList1.ToArray(), "professionForget");
          break;
        case "evilShrineCenter_Yes":
          if (Game1.player.Money >= 30000)
          {
            Game1.player.Money -= 30000;
            Game1.player.wipeExMemories();
            Game1.multiplayer.globalChatInfoMessage("EvilShrine", (string) (NetFieldBase<string, NetString>) Game1.player.name);
            Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(468f, 328f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
            {
              interval = 50f,
              totalNumberOfLoops = 99999,
              animationLength = 7,
              layerDepth = 0.0385f,
              scale = 4f
            });
            this.playSound("fireball");
            DelayedAction.playSoundAfterDelay("debuffHit", 500, this);
            int num = 0;
            Game1.player.faceDirection(2);
            Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
            {
              new FarmerSprite.AnimationFrame(94, 1500),
              new FarmerSprite.AnimationFrame(0, 1)
            });
            Game1.player.freezePause = 1500;
            Game1.player.jitterStrength = 1f;
            for (int index = 0; index < 20; ++index)
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(7f, 5f) * 64f + new Vector2((float) Game1.random.Next(-32, 64), (float) Game1.random.Next(16)), false, 1f / 500f, Microsoft.Xna.Framework.Color.SlateGray)
              {
                alpha = 0.75f,
                motion = new Vector2(0.0f, -0.5f),
                acceleration = new Vector2(-1f / 500f, 0.0f),
                interval = 99999f,
                layerDepth = (float) (0.0320000015199184 + (double) Game1.random.Next(100) / 10000.0),
                scale = 3f,
                scaleChange = 0.01f,
                rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
                delayBeforeAnimationStart = index * 25
              });
            for (int index = 0; index < 16; ++index)
            {
              foreach (Vector2 vector2 in Utility.getBorderOfThisRectangle(Utility.getRectangleCenteredAt(new Vector2(7f, 5f), 2 + index * 2)))
              {
                if (num % 2 == 0)
                {
                  Multiplayer multiplayer = Game1.multiplayer;
                  TemporaryAnimatedSprite[] temporaryAnimatedSpriteArray = new TemporaryAnimatedSprite[1];
                  TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(692, 1853, 4, 4), 25f, 1, 16, vector2 * 64f + new Vector2(32f, 32f), false, false);
                  temporaryAnimatedSprite.layerDepth = 1f;
                  temporaryAnimatedSprite.delayBeforeAnimationStart = index * 50;
                  temporaryAnimatedSprite.scale = 4f;
                  temporaryAnimatedSprite.scaleChange = 1f;
                  int r = (int) byte.MaxValue - (int) Utility.getRedToGreenLerpColor(1f / (float) (index + 1)).R;
                  Microsoft.Xna.Framework.Color toGreenLerpColor = Utility.getRedToGreenLerpColor(1f / (float) (index + 1));
                  int g = (int) byte.MaxValue - (int) toGreenLerpColor.G;
                  toGreenLerpColor = Utility.getRedToGreenLerpColor(1f / (float) (index + 1));
                  int b = (int) byte.MaxValue - (int) toGreenLerpColor.B;
                  temporaryAnimatedSprite.color = new Microsoft.Xna.Framework.Color(r, g, b);
                  temporaryAnimatedSprite.acceleration = new Vector2(-0.1f, 0.0f);
                  temporaryAnimatedSpriteArray[0] = temporaryAnimatedSprite;
                  multiplayer.broadcastSprites(this, temporaryAnimatedSpriteArray);
                }
                ++num;
              }
            }
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering"));
          break;
        case "evilShrineLeft_Yes":
          if (Game1.player.removeItemsFromInventory(74, 1))
          {
            Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(156f, 388f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
            {
              interval = 50f,
              totalNumberOfLoops = 99999,
              animationLength = 7,
              layerDepth = 0.0385f,
              scale = 4f
            });
            for (int index = 0; index < 20; ++index)
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(2f, 6f) * 64f + new Vector2((float) Game1.random.Next(-32, 64), (float) Game1.random.Next(16)), false, 1f / 500f, Microsoft.Xna.Framework.Color.LightGray)
              {
                alpha = 0.75f,
                motion = new Vector2(1f, -0.5f),
                acceleration = new Vector2(-1f / 500f, 0.0f),
                interval = 99999f,
                layerDepth = (float) (0.0384000018239021 + (double) Game1.random.Next(100) / 10000.0),
                scale = 3f,
                scaleChange = 0.01f,
                rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
                delayBeforeAnimationStart = index * 25
              });
            this.playSound("fireball");
            Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(2f, 5f) * 64f, false, true, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(4f, -2f)
            });
            if (Game1.player.getChildrenCount() > 1)
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(2f, 5f) * 64f, false, true, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                motion = new Vector2(4f, -1.5f),
                delayBeforeAnimationStart = 50
              });
            string message = "";
            foreach (NPC child in Game1.player.getChildren())
              message += Game1.content.LoadString("Strings\\Locations:WitchHut_Goodbye", (object) child.getName());
            Game1.showGlobalMessage(message);
            Game1.player.getRidOfChildren();
            Game1.multiplayer.globalChatInfoMessage("EvilShrine", (string) (NetFieldBase<string, NetString>) Game1.player.name);
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering"));
          break;
        case "evilShrineRightActivate_Yes":
          if (Game1.player.removeItemsFromInventory(203, 1))
          {
            Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(780f, 388f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
            {
              interval = 50f,
              totalNumberOfLoops = 99999,
              animationLength = 7,
              layerDepth = 0.0385f,
              scale = 4f
            });
            this.playSound("fireball");
            DelayedAction.playSoundAfterDelay("batScreech", 500, this);
            for (int index = 0; index < 20; ++index)
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(12f, 6f) * 64f + new Vector2((float) Game1.random.Next(-32, 64), (float) Game1.random.Next(16)), false, 1f / 500f, Microsoft.Xna.Framework.Color.DarkSlateBlue)
              {
                alpha = 0.75f,
                motion = new Vector2(-0.1f, -0.5f),
                acceleration = new Vector2(-1f / 500f, 0.0f),
                interval = 99999f,
                layerDepth = (float) (0.0384000018239021 + (double) Game1.random.Next(100) / 10000.0),
                scale = 3f,
                scaleChange = 0.01f,
                rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
                delayBeforeAnimationStart = index * 60
              });
            Game1.player.freezePause = 1501;
            for (int index = 0; index < 28; ++index)
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(540, 347, 13, 13), 50f, 4, 9999, new Vector2(12f, 5f) * 64f, false, true, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                delayBeforeAnimationStart = 500 + index * 25,
                motion = new Vector2((float) (Game1.random.Next(1, 5) * (Game1.random.NextDouble() < 0.5 ? -1 : 1)), (float) (Game1.random.Next(1, 5) * (Game1.random.NextDouble() < 0.5 ? -1 : 1)))
              });
            Game1.spawnMonstersAtNight = true;
            Game1.multiplayer.globalChatInfoMessage("MonstersActivated", (string) (NetFieldBase<string, NetString>) Game1.player.name);
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering"));
          break;
        case "evilShrineRightDeActivate_Yes":
          if (Game1.player.removeItemsFromInventory(203, 1))
          {
            Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(780f, 388f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
            {
              interval = 50f,
              totalNumberOfLoops = 99999,
              animationLength = 7,
              layerDepth = 0.0385f,
              scale = 4f
            });
            this.playSound("fireball");
            for (int index = 0; index < 20; ++index)
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(12f, 6f) * 64f + new Vector2((float) Game1.random.Next(-32, 64), (float) Game1.random.Next(16)), false, 1f / 500f, Microsoft.Xna.Framework.Color.DarkSlateBlue)
              {
                alpha = 0.75f,
                motion = new Vector2(0.0f, -0.5f),
                acceleration = new Vector2(-1f / 500f, 0.0f),
                interval = 99999f,
                layerDepth = (float) (0.0384000018239021 + (double) Game1.random.Next(100) / 10000.0),
                scale = 3f,
                scaleChange = 0.01f,
                rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
                delayBeforeAnimationStart = index * 25
              });
            Game1.spawnMonstersAtNight = false;
            Game1.multiplayer.globalChatInfoMessage("MonstersDeActivated", (string) (NetFieldBase<string, NetString>) Game1.player.name);
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering"));
          break;
        case "jukebox_Yes":
          Game1.drawItemNumberSelection("jukebox", -1);
          Game1.jukeboxPlaying = true;
          break;
        case "mariner_Buy":
          if (Game1.player.Money >= 5000)
          {
            Game1.player.Money -= 5000;
            Farmer player = Game1.player;
            Object @object = new Object(460, 1);
            @object.specialItem = true;
            player.addItemByMenuIfNecessary((Item) @object);
            if (Game1.activeClickableMenu == null)
            {
              Game1.player.holdUpItemThenMessage((Item) new Object(460, 1));
              break;
            }
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
          break;
        case "professionForget_combat":
          if (Game1.player.newLevels.Contains(new Point(4, 5)) || Game1.player.newLevels.Contains(new Point(4, 10)))
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueAlready"));
            break;
          }
          Game1.player.Money = Math.Max(0, Game1.player.Money - 10000);
          GameLocation.RemoveProfession(26);
          GameLocation.RemoveProfession(27);
          GameLocation.RemoveProfession(29);
          GameLocation.RemoveProfession(25);
          GameLocation.RemoveProfession(28);
          GameLocation.RemoveProfession(24);
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
          int num1 = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[4]);
          if (num1 >= 5)
            Game1.player.newLevels.Add(new Point(4, 5));
          if (num1 >= 10)
            Game1.player.newLevels.Add(new Point(4, 10));
          DelayedAction.playSoundAfterDelay("dog_bark", 300);
          DelayedAction.playSoundAfterDelay("dog_bark", 900);
          break;
        case "professionForget_farming":
          if (Game1.player.newLevels.Contains(new Point(0, 5)) || Game1.player.newLevels.Contains(new Point(0, 10)))
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueAlready"));
            break;
          }
          Game1.player.Money = Math.Max(0, Game1.player.Money - 10000);
          GameLocation.RemoveProfession(0);
          GameLocation.RemoveProfession(1);
          GameLocation.RemoveProfession(3);
          GameLocation.RemoveProfession(5);
          GameLocation.RemoveProfession(2);
          GameLocation.RemoveProfession(4);
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
          int num2 = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[0]);
          if (num2 >= 5)
            Game1.player.newLevels.Add(new Point(0, 5));
          if (num2 >= 10)
            Game1.player.newLevels.Add(new Point(0, 10));
          DelayedAction.playSoundAfterDelay("dog_bark", 300);
          DelayedAction.playSoundAfterDelay("dog_bark", 900);
          break;
        case "professionForget_fishing":
          if (Game1.player.newLevels.Contains(new Point(1, 5)) || Game1.player.newLevels.Contains(new Point(1, 10)))
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueAlready"));
            break;
          }
          Game1.player.Money = Math.Max(0, Game1.player.Money - 10000);
          GameLocation.RemoveProfession(8);
          GameLocation.RemoveProfession(11);
          GameLocation.RemoveProfession(10);
          GameLocation.RemoveProfession(6);
          GameLocation.RemoveProfession(9);
          GameLocation.RemoveProfession(7);
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
          int num3 = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[1]);
          if (num3 >= 5)
            Game1.player.newLevels.Add(new Point(1, 5));
          if (num3 >= 10)
            Game1.player.newLevels.Add(new Point(1, 10));
          DelayedAction.playSoundAfterDelay("dog_bark", 300);
          DelayedAction.playSoundAfterDelay("dog_bark", 900);
          break;
        case "professionForget_foraging":
          if (Game1.player.newLevels.Contains(new Point(2, 5)) || Game1.player.newLevels.Contains(new Point(2, 10)))
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueAlready"));
            break;
          }
          Game1.player.Money = Math.Max(0, Game1.player.Money - 10000);
          GameLocation.RemoveProfession(16);
          GameLocation.RemoveProfession(14);
          GameLocation.RemoveProfession(17);
          GameLocation.RemoveProfession(12);
          GameLocation.RemoveProfession(13);
          GameLocation.RemoveProfession(15);
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
          int num4 = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[2]);
          if (num4 >= 5)
            Game1.player.newLevels.Add(new Point(2, 5));
          if (num4 >= 10)
            Game1.player.newLevels.Add(new Point(2, 10));
          DelayedAction.playSoundAfterDelay("dog_bark", 300);
          DelayedAction.playSoundAfterDelay("dog_bark", 900);
          break;
        case "professionForget_mining":
          if (Game1.player.newLevels.Contains(new Point(3, 5)) || Game1.player.newLevels.Contains(new Point(3, 10)))
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueAlready"));
            break;
          }
          Game1.player.Money = Math.Max(0, Game1.player.Money - 10000);
          GameLocation.RemoveProfession(23);
          GameLocation.RemoveProfession(21);
          GameLocation.RemoveProfession(18);
          GameLocation.RemoveProfession(19);
          GameLocation.RemoveProfession(22);
          GameLocation.RemoveProfession(20);
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
          int num5 = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[3]);
          if (num5 >= 5)
            Game1.player.newLevels.Add(new Point(3, 5));
          if (num5 >= 10)
            Game1.player.newLevels.Add(new Point(3, 10));
          DelayedAction.playSoundAfterDelay("dog_bark", 300);
          DelayedAction.playSoundAfterDelay("dog_bark", 900);
          break;
        case "specialCharmQuestion_Yes":
          if (Game1.player.hasItemInInventory(446, 1))
          {
            Game1.player.holdUpItemThenMessage((Item) new SpecialItem(3));
            Game1.player.removeFirstOfThisItemFromInventory(446);
            Game1.player.hasSpecialCharm = true;
            Game1.player.mailReceived.Add("SecretNote20_done");
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_specialCharmNoFoot"));
          break;
        case "taxvote_Against":
          Game1.addMailForTomorrow("taxRejected");
          ++this.currentEvent.currentCommand;
          break;
        case "taxvote_For":
          Game1.shippingTax = true;
          Game1.addMailForTomorrow("taxPassed");
          ++this.currentEvent.currentCommand;
          break;
        case "telephone_AdventureGuild":
          this.playShopPhoneNumberSounds(questionAndAnswer);
          Game1.player.freezePause = 4950;
          DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
          {
            Game1.playSound("bigSelect");
            NPC character = Game1.getCharacterFromName("Marlon");
            Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            if (Game1.player.mailForTomorrow.Contains("MarlonRecovery"))
            {
              Game1.drawDialogue(character, Game1.content.LoadString("Strings\\Characters:Phone_Marlon_AlreadyRecovering"));
            }
            else
            {
              Game1.drawDialogue(character, Game1.content.LoadString("Strings\\Characters:Phone_Marlon_Open"));
              Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
              {
                if (Game1.player.itemsLostLastDeath.Count > 0)
                {
                  Game1.player.forceCanMove();
                  Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getAdventureRecoveryStock(), who: "Marlon_Recovery");
                }
                else
                  Game1.drawDialogue(character, Game1.content.LoadString("Strings\\Characters:Phone_Marlon_NoDeathItems"));
              });
            }
          }), 4950);
          break;
        case "telephone_AnimalShop":
          this.playShopPhoneNumberSounds(questionAndAnswer);
          Game1.player.freezePause = 4950;
          DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
          {
            Game1.playSound("bigSelect");
            NPC characterFromName = Game1.getCharacterFromName("Marnie");
            Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            if (GameLocation.AreStoresClosedForFestival())
              Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Marnie_ClosedDay"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            else if (characterFromName.dayScheduleName.Value == "fall_18" || characterFromName.dayScheduleName.Value == "winter_18" || characterFromName.dayScheduleName.Value == "Tue" || characterFromName.dayScheduleName.Value == "Mon")
              Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Marnie_ClosedDay"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            else if (Game1.timeOfDay >= 900 && Game1.timeOfDay < 1600)
              Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Marnie_Open" + (Game1.random.NextDouble() < 0.01 ? "_Rare" : "")));
            else
              Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Marnie_Closed"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            Game1.afterDialogues += (Game1.afterFadeFunction) (() => Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), new List<Response>()
            {
              new Response("AnimalShop_CheckAnimalPrices", Game1.content.LoadString("Strings\\Characters:Phone_CheckAnimalPrices")),
              new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
            }.ToArray(), "telephone"));
          }), 4950);
          break;
        case "telephone_AnimalShop_CheckAnimalPrices":
          Game1.activeClickableMenu = (IClickableMenu) new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock());
          if (Game1.activeClickableMenu is PurchaseAnimalsMenu activeClickableMenu1)
          {
            activeClickableMenu1.readOnly = true;
            PurchaseAnimalsMenu purchaseAnimalsMenu = activeClickableMenu1;
            purchaseAnimalsMenu.behaviorBeforeCleanup = purchaseAnimalsMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (closed_menu => this.answerDialogueAction("HangUp", new string[0]));
            break;
          }
          break;
        case "telephone_Blacksmith":
          this.playShopPhoneNumberSounds(questionAndAnswer);
          Game1.player.freezePause = 4950;
          DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
          {
            Game1.playSound("bigSelect");
            NPC characterFromName = Game1.getCharacterFromName("Clint");
            if (GameLocation.AreStoresClosedForFestival())
              Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Clint_Festival"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            else if (Game1.player.daysLeftForToolUpgrade.Value > 0)
            {
              int sub1 = Game1.player.daysLeftForToolUpgrade.Value;
              if (sub1 == 1)
                Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Clint_Working_OneDay"));
              else
                Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Clint_Working", (object) sub1));
            }
            else
            {
              string str = characterFromName.dayScheduleName.Value;
              if (!(str == "winter_16"))
              {
                if (str == "Fri")
                  Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Clint_Festival"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
                else if (Game1.timeOfDay >= 900 && Game1.timeOfDay < 1600)
                  Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Clint_Open" + (Game1.random.NextDouble() < 0.01 ? "_Rare" : "")));
                else
                  Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Clint_Closed"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
              }
              else
                Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Clint_Festival"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            }
            Game1.afterDialogues += (Game1.afterFadeFunction) (() => Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), new List<Response>()
            {
              new Response("Blacksmith_UpgradeCost", Game1.content.LoadString("Strings\\Characters:Phone_CheckToolCost")),
              new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
            }.ToArray(), "telephone"));
          }), 4950);
          break;
        case "telephone_Blacksmith_UpgradeCost":
          this.answerDialogueAction("Blacksmith_Upgrade", new string[0]);
          if (Game1.activeClickableMenu is ShopMenu activeClickableMenu2)
          {
            activeClickableMenu2.readOnly = true;
            ShopMenu shopMenu = activeClickableMenu2;
            shopMenu.behaviorBeforeCleanup = shopMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (closed_menu => this.answerDialogueAction("HangUp", new string[0]));
            break;
          }
          break;
        case "telephone_Carpenter":
          this.playShopPhoneNumberSounds(questionAndAnswer);
          Game1.player.freezePause = 4950;
          DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
          {
            Game1.playSound("bigSelect");
            NPC characterFromName = Game1.getCharacterFromName("Robin");
            if (GameLocation.AreStoresClosedForFestival())
              Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Robin_Festival"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            else if (Game1.getLocationFromName("Town") is Town locationFromName2 && locationFromName2.daysUntilCommunityUpgrade.Value > 0)
            {
              int sub1 = locationFromName2.daysUntilCommunityUpgrade.Value;
              if (sub1 == 1)
                Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Robin_Working_OneDay"));
              else
                Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Robin_Working", (object) sub1));
            }
            else if (Game1.getFarm().isThereABuildingUnderConstruction())
            {
              Building underConstruction = Game1.getFarm().getBuildingUnderConstruction();
              int sub1 = 0;
              if (underConstruction != null)
                sub1 = (int) (NetFieldBase<int, NetInt>) underConstruction.daysUntilUpgrade;
              if (sub1 == 1)
                Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Robin_Working_OneDay"));
              else
                Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Robin_Working", (object) sub1));
            }
            else
            {
              string str = characterFromName.dayScheduleName.Value;
              if (!(str == "summer_18"))
              {
                if (str == "Tue")
                  Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Robin_Workout"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
                else if (Game1.timeOfDay >= 900 && Game1.timeOfDay < 1700)
                  Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Robin_Open" + (Game1.random.NextDouble() < 0.01 ? "_Rare" : "")));
                else
                  Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Robin_Closed"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
              }
              else
                Game1.drawDialogue(characterFromName, Game1.content.LoadString("Strings\\Characters:Phone_Robin_Festival"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            }
            Game1.afterDialogues += (Game1.afterFadeFunction) (() =>
            {
              List<Response> responseList3 = new List<Response>();
              responseList3.Add(new Response("Carpenter_ShopStock", Game1.content.LoadString("Strings\\Characters:Phone_CheckSeedStock")));
              if ((int) (NetFieldBase<int, NetInt>) Game1.player.houseUpgradeLevel < 3)
                responseList3.Add(new Response("Carpenter_HouseCost", Game1.content.LoadString("Strings\\Characters:Phone_CheckHouseCost")));
              responseList3.Add(new Response("Carpenter_BuildingCost", Game1.content.LoadString("Strings\\Characters:Phone_CheckBuildingCost")));
              responseList3.Add(new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp")));
              Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), responseList3.ToArray(), "telephone");
            });
          }), 4950);
          break;
        case "telephone_Carpenter_BuildingCost":
          this.answerDialogueAction("carpenter_Construct", new string[0]);
          if (Game1.activeClickableMenu is CarpenterMenu activeClickableMenu3)
          {
            activeClickableMenu3.readOnly = true;
            CarpenterMenu carpenterMenu = activeClickableMenu3;
            carpenterMenu.behaviorBeforeCleanup = carpenterMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (closed_menu => this.answerDialogueAction("HangUp", new string[0]));
            break;
          }
          break;
        case "telephone_Carpenter_HouseCost":
          NPC characterFromName1 = Game1.getCharacterFromName("Robin");
          string str1 = Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse" + ((int) (NetFieldBase<int, NetInt>) Game1.player.houseUpgradeLevel + 1).ToString());
          if (str1.Contains('.'))
            str1 = str1.Substring(0, str1.LastIndexOf('.') + 1);
          else if (str1.Contains('。'))
            str1 = str1.Substring(0, str1.LastIndexOf('。') + 1);
          string dialogue = str1;
          Texture2D overridePortrait = Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine");
          Game1.drawDialogue(characterFromName1, dialogue, overridePortrait);
          Game1.afterDialogues += (Game1.afterFadeFunction) (() => this.answerDialogueAction("HangUp", new string[0]));
          break;
        case "telephone_Carpenter_ShopStock":
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getCarpenterStock());
          if (Game1.activeClickableMenu is ShopMenu)
          {
            ShopMenu activeClickableMenu4 = Game1.activeClickableMenu as ShopMenu;
            activeClickableMenu4.readOnly = true;
            activeClickableMenu4.behaviorBeforeCleanup = activeClickableMenu4.behaviorBeforeCleanup + (Action<IClickableMenu>) (closed_menu => this.answerDialogueAction("HangUp", new string[0]));
            break;
          }
          break;
        case "telephone_HangUp":
          this.playSound("openBox");
          break;
        case "telephone_Saloon":
          this.playShopPhoneNumberSounds(questionAndAnswer);
          Game1.player.freezePause = 4950;
          DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
          {
            Game1.playSound("bigSelect");
            NPC characterFromName2 = Game1.getCharacterFromName("Gus");
            if (GameLocation.AreStoresClosedForFestival())
              Game1.drawDialogue(characterFromName2, Game1.content.LoadString("Strings\\Characters:Phone_Gus_Festival"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            else if (Game1.timeOfDay >= 1200 && Game1.timeOfDay < 2400 && (characterFromName2.dayScheduleName.Value != "fall_4" || Game1.timeOfDay >= 1700))
            {
              if (Game1.dishOfTheDay != null)
                Game1.drawDialogue(characterFromName2, Game1.content.LoadString("Strings\\Characters:Phone_Gus_Open" + (Game1.random.NextDouble() < 0.01 ? "_Rare" : ""), (object) Game1.dishOfTheDay.DisplayName));
              else
                Game1.drawDialogue(characterFromName2, Game1.content.LoadString("Strings\\Characters:Phone_Gus_Open_NoDishOfTheDay"));
            }
            else if (Game1.dishOfTheDay != null && Game1.timeOfDay < 2400)
              Game1.drawDialogue(characterFromName2, Game1.content.LoadString("Strings\\Characters:Phone_Gus_Closed", (object) Game1.dishOfTheDay.DisplayName), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            else
              Game1.drawDialogue(characterFromName2, Game1.content.LoadString("Strings\\Characters:Phone_Gus_Closed_NoDishOfTheDay"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            this.answerDialogueAction("HangUp", new string[0]);
          }), 4950);
          break;
        case "telephone_SeedShop":
          this.playShopPhoneNumberSounds(questionAndAnswer);
          Game1.player.freezePause = 4950;
          DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
          {
            Game1.playSound("bigSelect");
            NPC characterFromName3 = Game1.getCharacterFromName("Pierre");
            string str2 = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            if (GameLocation.AreStoresClosedForFestival())
              Game1.drawDialogue(characterFromName3, Game1.content.LoadString("Strings\\Characters:Phone_Pierre_Festival"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            else if ((Game1.isLocationAccessible("CommunityCenter") || str2 != "Wed") && Game1.timeOfDay >= 900 && Game1.timeOfDay < 1700)
              Game1.drawDialogue(characterFromName3, Game1.content.LoadString("Strings\\Characters:Phone_Pierre_Open" + (Game1.random.NextDouble() < 0.01 ? "_Rare" : "")));
            else
              Game1.drawDialogue(characterFromName3, Game1.content.LoadString("Strings\\Characters:Phone_Pierre_Closed"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
            Game1.afterDialogues += (Game1.afterFadeFunction) (() => Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), new List<Response>()
            {
              new Response("SeedShop_CheckSeedStock", Game1.content.LoadString("Strings\\Characters:Phone_CheckSeedStock")),
              new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
            }.ToArray(), "telephone"));
          }), 4950);
          break;
        case "telephone_SeedShop_CheckSeedStock":
          if (Game1.getLocationFromName("SeedShop") is SeedShop locationFromName3)
          {
            Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(locationFromName3.shopStock(), who: "Pierre");
            if (Game1.activeClickableMenu is ShopMenu activeClickableMenu5)
            {
              activeClickableMenu5.readOnly = true;
              ShopMenu shopMenu = activeClickableMenu5;
              shopMenu.behaviorBeforeCleanup = shopMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (closed_menu => this.answerDialogueAction("HangUp", new string[0]));
              break;
            }
            break;
          }
          this.answerDialogueAction("HangUp", new string[0]);
          break;
        case "upgrade_Yes":
          this.houseUpgradeAccept();
          break;
        case null:
          return false;
        default:
          return false;
      }
      return true;
    }

    private void playShopPhoneNumberSounds(string whichShop)
    {
      Random random = new Random(whichShop.GetHashCode());
      DelayedAction.playSoundAfterDelay("telephone_dialtone", 495, pitch: 1200);
      DelayedAction.playSoundAfterDelay("telephone_buttonPush", 1200, pitch: (1200 + random.Next(-4, 5) * 100));
      DelayedAction.playSoundAfterDelay("telephone_buttonPush", 1370, pitch: (1200 + random.Next(-4, 5) * 100));
      DelayedAction.playSoundAfterDelay("telephone_buttonPush", 1600, pitch: (1200 + random.Next(-4, 5) * 100));
      DelayedAction.playSoundAfterDelay("telephone_buttonPush", 1850, pitch: (1200 + random.Next(-4, 5) * 100));
      DelayedAction.playSoundAfterDelay("telephone_buttonPush", 2030, pitch: (1200 + random.Next(-4, 5) * 100));
      DelayedAction.playSoundAfterDelay("telephone_buttonPush", 2250, pitch: (1200 + random.Next(-4, 5) * 100));
      DelayedAction.playSoundAfterDelay("telephone_buttonPush", 2410, pitch: (1200 + random.Next(-4, 5) * 100));
      DelayedAction.playSoundAfterDelay("telephone_ringingInEar", 3150);
    }

    public virtual bool answerDialogue(Response answer)
    {
      string[] questionParams = this.lastQuestionKey != null ? this.lastQuestionKey.Split(' ') : (string[]) null;
      string questionAndAnswer = questionParams != null ? questionParams[0] + "_" + answer.responseKey : (string) null;
      if (answer.responseKey.Equals("Move"))
      {
        Game1.player.grabObject(this.actionObjectForQuestionDialogue);
        this.removeObject(this.actionObjectForQuestionDialogue.TileLocation, false);
        this.actionObjectForQuestionDialogue = (Object) null;
        return true;
      }
      if (this.afterQuestion != null)
      {
        this.afterQuestion(Game1.player, answer.responseKey);
        this.afterQuestion = (GameLocation.afterQuestionBehavior) null;
        Game1.objectDialoguePortraitPerson = (NPC) null;
        return true;
      }
      return questionAndAnswer != null && this.answerDialogueAction(questionAndAnswer, questionParams);
    }

    public static bool AreStoresClosedForFestival() => Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason) && Utility.getStartTimeOfFestival() < 1900;

    public static void RemoveProfession(int profession)
    {
      if (!Game1.player.professions.Contains(profession))
        return;
      LevelUpMenu.removeImmediateProfessionPerk(profession);
      Game1.player.professions.Remove(profession);
    }

    public static bool canRespec(int skill_index) => Game1.player.GetUnmodifiedSkillLevel(skill_index) >= 5 && !Game1.player.newLevels.Contains(new Point(skill_index, 5)) && !Game1.player.newLevels.Contains(new Point(skill_index, 10));

    public void setObject(Vector2 v, Object o)
    {
      if (this.objects.ContainsKey(v))
        this.objects[v] = o;
      else
        this.objects.Add(v, o);
    }

    private void houseUpgradeOffer()
    {
      switch ((int) (NetFieldBase<int, NetInt>) Game1.player.houseUpgradeLevel)
      {
        case 0:
          this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse1")), this.createYesNoResponses(), "upgrade");
          break;
        case 1:
          this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse2")), this.createYesNoResponses(), "upgrade");
          break;
        case 2:
          this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse3")), this.createYesNoResponses(), "upgrade");
          break;
      }
    }

    private void communityUpgradeOffer()
    {
      if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
      {
        this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_CommunityUpgrade1")), this.createYesNoResponses(), "communityUpgrade");
        if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgradeAsked"))
          return;
        Game1.MasterPlayer.mailReceived.Add("pamHouseUpgradeAsked");
      }
      else
      {
        if (Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
          return;
        this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_CommunityUpgrade2")), this.createYesNoResponses(), "communityUpgrade");
      }
    }

    public virtual bool catchOceanCrabPotFishFromThisSpot(int x, int y) => false;

    public virtual float getExtraTrashChanceForCrabPot(int x, int y) => 0.0f;

    private void communityUpgradeAccept()
    {
      if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
      {
        if (Game1.player.Money >= 500000 && Game1.player.hasItemInInventory(388, 950))
        {
          Game1.player.Money -= 500000;
          Game1.player.removeItemsFromInventory(388, 950);
          Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_PamUpgrade_Accepted"));
          Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
          (Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade.Value = 3;
          Game1.multiplayer.globalChatInfoMessage("CommunityUpgrade", Game1.player.Name);
        }
        else if (Game1.player.Money < 500000)
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
        else
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood3"));
      }
      else
      {
        if (Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
          return;
        if (Game1.player.Money >= 300000)
        {
          Game1.player.Money -= 300000;
          Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
          Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
          (Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade.Value = 3;
          Game1.multiplayer.globalChatInfoMessage("CommunityUpgrade", Game1.player.Name);
        }
        else
        {
          if (Game1.player.Money >= 300000)
            return;
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
        }
      }
    }

    private void houseUpgradeAccept()
    {
      switch ((int) (NetFieldBase<int, NetInt>) Game1.player.houseUpgradeLevel)
      {
        case 0:
          if (Game1.player.Money >= 10000 && Game1.player.hasItemInInventory(388, 450))
          {
            Game1.player.daysUntilHouseUpgrade.Value = 3;
            Game1.player.Money -= 10000;
            Game1.player.removeItemsFromInventory(388, 450);
            Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
            Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
            Game1.multiplayer.globalChatInfoMessage("HouseUpgrade", Game1.player.Name, Lexicon.getPossessivePronoun((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale));
            break;
          }
          if (Game1.player.Money < 10000)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood1"));
          break;
        case 1:
          if (Game1.player.Money >= 50000 && Game1.player.hasItemInInventory(709, 150))
          {
            Game1.player.daysUntilHouseUpgrade.Value = 3;
            Game1.player.Money -= 50000;
            Game1.player.removeItemsFromInventory(709, 150);
            Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
            Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
            Game1.multiplayer.globalChatInfoMessage("HouseUpgrade", Game1.player.Name, Lexicon.getPossessivePronoun((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale));
            break;
          }
          if (Game1.player.Money < 50000)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood2"));
          break;
        case 2:
          if (Game1.player.Money >= 100000)
          {
            Game1.player.daysUntilHouseUpgrade.Value = 3;
            Game1.player.Money -= 100000;
            Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
            Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
            Game1.multiplayer.globalChatInfoMessage("HouseUpgrade", Game1.player.Name, Lexicon.getPossessivePronoun((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale));
            break;
          }
          if (Game1.player.Money >= 100000)
            break;
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
          break;
      }
    }

    private void smeltBar(Object bar, int minutesUntilReady)
    {
      --Game1.player.CoalPieces;
      this.actionObjectForQuestionDialogue.heldObject.Value = bar;
      this.actionObjectForQuestionDialogue.minutesUntilReady.Value = minutesUntilReady;
      this.actionObjectForQuestionDialogue.showNextIndex.Value = true;
      this.actionObjectForQuestionDialogue = (Object) null;
      this.playSound("openBox");
      this.playSound("furnace");
      ++Game1.stats.BarsSmelted;
    }

    public void tryToBuyNewBackpack()
    {
      int num = 0;
      switch (Game1.player.MaxItems)
      {
        case 4:
          num = 3500;
          break;
        case 9:
          num = 7500;
          break;
        case 14:
          num = 15000;
          break;
      }
      if (Game1.player.Money >= num)
      {
        Game1.player.increaseBackpackSize(5);
        Game1.player.Money -= num;
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:Backpack_Bought", (object) Game1.player.MaxItems));
        this.checkForMapChanges();
      }
      else
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
    }

    public void checkForMapChanges()
    {
      if (!this.name.Equals((object) "SeedShop") || Game1.player.MaxItems != 19)
        return;
      this.map.GetLayer("Front").Tiles[10, 21] = (Tile) new StaticTile(this.map.GetLayer("Front"), this.map.GetTileSheet("TownHouseIndoors"), BlendMode.Alpha, 203);
    }

    public void removeStumpOrBoulder(int tileX, int tileY, Object o)
    {
      List<Vector2> locations = new List<Vector2>();
      switch (o.Name)
      {
        case "Boulder1/4":
        case "Stump1/4":
          locations.Add(new Vector2((float) tileX, (float) tileY));
          locations.Add(new Vector2((float) (tileX + 1), (float) tileY));
          locations.Add(new Vector2((float) tileX, (float) (tileY + 1)));
          locations.Add(new Vector2((float) (tileX + 1), (float) (tileY + 1)));
          break;
        case "Boulder2/4":
        case "Stump2/4":
          locations.Add(new Vector2((float) tileX, (float) tileY));
          locations.Add(new Vector2((float) (tileX - 1), (float) tileY));
          locations.Add(new Vector2((float) tileX, (float) (tileY + 1)));
          locations.Add(new Vector2((float) (tileX - 1), (float) (tileY + 1)));
          break;
        case "Boulder3/4":
        case "Stump3/4":
          locations.Add(new Vector2((float) tileX, (float) tileY));
          locations.Add(new Vector2((float) (tileX + 1), (float) tileY));
          locations.Add(new Vector2((float) tileX, (float) (tileY - 1)));
          locations.Add(new Vector2((float) (tileX + 1), (float) (tileY - 1)));
          break;
        case "Boulder4/4":
        case "Stump4/4":
          locations.Add(new Vector2((float) tileX, (float) tileY));
          locations.Add(new Vector2((float) (tileX - 1), (float) tileY));
          locations.Add(new Vector2((float) tileX, (float) (tileY - 1)));
          locations.Add(new Vector2((float) (tileX - 1), (float) (tileY - 1)));
          break;
      }
      int initialParentTileIndex = o.Name.Contains("Stump") ? 5 : 3;
      if (Game1.currentSeason.Equals("winter"))
        initialParentTileIndex += 376;
      for (int index = 0; index < locations.Count; ++index)
        Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(initialParentTileIndex, (float) Game1.random.Next(150, 400), 1, 3, new Vector2(locations[index].X * 64f, locations[index].Y * 64f), true, (bool) (NetFieldBase<bool, NetBool>) o.flipped));
      this.removeBatch(locations);
    }

    public void destroyObject(Vector2 tileLocation, Farmer who) => this.destroyObject(tileLocation, false, who);

    public void destroyObject(Vector2 tileLocation, bool hardDestroy, Farmer who)
    {
      if (!this.objects.ContainsKey(tileLocation) || this.objects[tileLocation].IsHoeDirt || (int) (NetFieldBase<int, NetInt>) this.objects[tileLocation].fragility == 2 || this.objects[tileLocation] is Chest || (int) (NetFieldBase<int, NetInt>) this.objects[tileLocation].parentSheetIndex == 165)
        return;
      Object o = this.objects[tileLocation];
      bool flag = false;
      if (o.Type != null && (o.Type.Equals("Fish") || o.Type.Equals("Cooking") || o.Type.Equals("Crafting")))
      {
        Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(o.ParentSheetIndex, 150f, 1, 3, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f), true, (bool) (NetFieldBase<bool, NetBool>) o.bigCraftable, (bool) (NetFieldBase<bool, NetBool>) o.flipped));
        flag = true;
      }
      else if (o.Name.Contains("Stump") || o.Name.Contains("Boulder"))
      {
        flag = true;
        this.removeStumpOrBoulder((int) tileLocation.X, (int) tileLocation.Y, o);
      }
      else if (o.CanBeGrabbed | hardDestroy)
        flag = true;
      if (this is MineShaft && !o.Name.Contains("Lumber"))
        flag = true;
      if (o.Name.Contains("Stone") && !(bool) (NetFieldBase<bool, NetBool>) o.bigCraftable && !(o is Fence))
      {
        flag = true;
        this.OnStoneDestroyed((int) (NetFieldBase<int, NetInt>) o.parentSheetIndex, (int) tileLocation.X, (int) tileLocation.Y, who);
      }
      if (!flag)
        return;
      this.objects.Remove(tileLocation);
    }

    public GameLocation.LocationContext GetLocationContext()
    {
      if (this.locationContext == ~GameLocation.LocationContext.Default)
      {
        if (this.map == null)
          this.reloadMap();
        this.locationContext = GameLocation.LocationContext.Default;
        PropertyValue propertyValue = (PropertyValue) null;
        if (this.map == null)
          return GameLocation.LocationContext.Default;
        string str = !this.map.Properties.TryGetValue("LocationContext", out propertyValue) ? "" : propertyValue.ToString();
        if (str != "" && !Enum.TryParse<GameLocation.LocationContext>(str, out this.locationContext))
          this.locationContext = GameLocation.LocationContext.Default;
      }
      return this.locationContext;
    }

    public virtual bool sinkDebris(Debris debris, Vector2 chunkTile, Vector2 chunkPosition)
    {
      if (debris.isEssentialItem() || (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) debris.debrisType == Debris.DebrisType.OBJECT && (int) (NetFieldBase<int, NetInt>) debris.chunkType == 74)
        return false;
      if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) debris.debrisType == Debris.DebrisType.CHUNKS)
      {
        this.localSound("quickSlosh");
        this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 150f, 3, 0, chunkPosition, false, Game1.random.NextDouble() < 0.5, 1f / 1000f, 0.02f, Microsoft.Xna.Framework.Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f));
      }
      else
      {
        this.TemporarySprites.Add(new TemporaryAnimatedSprite(28, 300f, 2, 1, chunkPosition, false, false));
        this.localSound("dropItemInWater");
      }
      return true;
    }

    public virtual bool doesTileSinkDebris(int xTile, int yTile, Debris.DebrisType type) => type == Debris.DebrisType.CHUNKS ? this.doesTileHaveProperty(xTile, yTile, "Water", "Back") != null && this.getTileIndexAt(xTile, yTile, "Buildings") == -1 : this.doesTileHaveProperty(xTile, yTile, "Water", "Back") != null && !this.isTileUpperWaterBorder(this.getTileIndexAt(xTile, yTile, "Buildings")) && this.doesTileHaveProperty(xTile, yTile, "Passable", "Buildings") == null;

    private bool isTileUpperWaterBorder(int index)
    {
      switch (index)
      {
        case 183:
        case 184:
        case 185:
        case 211:
        case 1182:
        case 1183:
        case 1184:
        case 1210:
          return true;
        default:
          return false;
      }
    }

    public virtual bool doesEitherTileOrTileIndexPropertyEqual(
      int xTile,
      int yTile,
      string propertyName,
      string layerName,
      string propertyValue)
    {
      PropertyValue propertyValue1 = (PropertyValue) null;
      if (this.map != null && this.map.GetLayer(layerName) != null)
      {
        Tile tile = this.map.GetLayer(layerName).PickTile(new Location(xTile * 64, yTile * 64), Game1.viewport.Size);
        if (tile != null && tile.TileIndexProperties.TryGetValue(propertyName, out propertyValue1) && propertyValue1.ToString() == propertyValue || tile != null && this.map.GetLayer(layerName).PickTile(new Location(xTile * 64, yTile * 64), Game1.viewport.Size).Properties.TryGetValue(propertyName, out propertyValue1) && propertyValue1.ToString() == propertyValue)
          return true;
      }
      return propertyValue == null;
    }

    public virtual string doesTileHaveProperty(
      int xTile,
      int yTile,
      string propertyName,
      string layerName)
    {
      foreach (Furniture furniture in this.furniture)
      {
        if ((double) xTile >= (double) furniture.tileLocation.X - (double) furniture.GetAdditionalTilePropertyRadius() && (double) xTile < (double) furniture.tileLocation.X + (double) furniture.getTilesWide() + (double) furniture.GetAdditionalTilePropertyRadius() && (double) yTile >= (double) furniture.tileLocation.Y - (double) furniture.GetAdditionalTilePropertyRadius() && (double) yTile < (double) furniture.tileLocation.Y + (double) furniture.getTilesHigh() + (double) furniture.GetAdditionalTilePropertyRadius())
        {
          string property_value = (string) null;
          if (furniture.DoesTileHaveProperty(xTile, yTile, propertyName, layerName, ref property_value))
            return property_value;
        }
      }
      PropertyValue propertyValue = (PropertyValue) null;
      if (this.map != null && this.map.GetLayer(layerName) != null)
      {
        Tile tile = this.map.GetLayer(layerName).PickTile(new Location(xTile * 64, yTile * 64), Game1.viewport.Size);
        tile?.TileIndexProperties.TryGetValue(propertyName, out propertyValue);
        if (propertyValue == null && tile != null)
          this.map.GetLayer(layerName).PickTile(new Location(xTile * 64, yTile * 64), Game1.viewport.Size).Properties.TryGetValue(propertyName, out propertyValue);
      }
      return propertyValue?.ToString();
    }

    public virtual string doesTileHavePropertyNoNull(
      int xTile,
      int yTile,
      string propertyName,
      string layerName)
    {
      foreach (Furniture furniture in this.furniture)
      {
        if ((double) xTile >= (double) furniture.tileLocation.X && (double) xTile < (double) furniture.tileLocation.X + (double) furniture.getTilesWide() && (double) yTile >= (double) furniture.tileLocation.Y && (double) yTile < (double) furniture.tileLocation.Y + (double) furniture.getTilesHigh())
        {
          string property_value = (string) null;
          if (furniture.DoesTileHaveProperty(xTile, yTile, propertyName, layerName, ref property_value))
            return property_value ?? "";
        }
      }
      PropertyValue propertyValue1 = (PropertyValue) null;
      PropertyValue propertyValue2 = (PropertyValue) null;
      if (this.map != null && this.map.GetLayer(layerName) != null)
      {
        Tile tile = this.map.GetLayer(layerName).PickTile(new Location(xTile * 64, yTile * 64), Game1.viewport.Size);
        tile?.TileIndexProperties.TryGetValue(propertyName, out propertyValue1);
        if (tile != null)
          this.map.GetLayer(layerName).PickTile(new Location(xTile * 64, yTile * 64), Game1.viewport.Size).Properties.TryGetValue(propertyName, out propertyValue2);
        if (propertyValue2 != null)
          propertyValue1 = propertyValue2;
      }
      return propertyValue1 != null ? propertyValue1.ToString() : "";
    }

    public bool isWaterTile(int xTile, int yTile) => this.doesTileHaveProperty(xTile, yTile, "Water", "Back") != null;

    public bool isOpenWater(int xTile, int yTile)
    {
      if (!this.isWaterTile(xTile, yTile))
        return false;
      int tileIndexAt = this.getTileIndexAt(xTile, yTile, "Buildings");
      if (tileIndexAt != -1)
      {
        bool flag = true;
        if (this.getTileSheetIDAt(xTile, yTile, "Buildings") == "outdoors" && (tileIndexAt == 759 || tileIndexAt == 628 || tileIndexAt == 629 || tileIndexAt == 734))
          flag = false;
        if (flag)
          return false;
      }
      return !this.objects.ContainsKey(new Vector2((float) xTile, (float) yTile));
    }

    public bool isCropAtTile(int tileX, int tileY)
    {
      Vector2 key = new Vector2((float) tileX, (float) tileY);
      return this.terrainFeatures.ContainsKey(key) && this.terrainFeatures[key] is HoeDirt && ((HoeDirt) this.terrainFeatures[key]).crop != null;
    }

    public virtual bool dropObject(
      Object obj,
      Vector2 dropLocation,
      xTile.Dimensions.Rectangle viewport,
      bool initialPlacement,
      Farmer who = null)
    {
      obj.isSpawnedObject.Value = true;
      Vector2 vector2 = new Vector2((float) ((int) dropLocation.X / 64), (float) ((int) dropLocation.Y / 64));
      if (!this.isTileOnMap(vector2) || this.map.GetLayer("Back").PickTile(new Location((int) dropLocation.X, (int) dropLocation.Y), Game1.viewport.Size) == null || this.map.GetLayer("Back").Tiles[(int) vector2.X, (int) vector2.Y].TileIndexProperties.ContainsKey("Unplaceable"))
        return false;
      if ((bool) (NetFieldBase<bool, NetBool>) obj.bigCraftable)
      {
        obj.tileLocation.Value = vector2;
        if (!(bool) (NetFieldBase<bool, NetBool>) this.isFarm || !(bool) (NetFieldBase<bool, NetBool>) obj.setOutdoors && (bool) (NetFieldBase<bool, NetBool>) this.isOutdoors || !(bool) (NetFieldBase<bool, NetBool>) obj.setIndoors && !(bool) (NetFieldBase<bool, NetBool>) this.isOutdoors || obj.performDropDownAction(who))
          return false;
      }
      else if (obj.Type != null && obj.Type.Equals("Crafting") && obj.performDropDownAction(who))
        obj.CanBeSetDown = false;
      bool flag = this.isTilePassable(new Location((int) vector2.X, (int) vector2.Y), viewport) && !this.isTileOccupiedForPlacement(vector2);
      if ((obj.CanBeSetDown | initialPlacement) & flag && !this.isTileHoeDirt(vector2))
      {
        obj.TileLocation = vector2;
        if (this.objects.ContainsKey(vector2))
          return false;
        this.objects.Add(vector2, obj);
      }
      else if (this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Water", "Back") != null)
      {
        Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(28, 300f, 2, 1, dropLocation, false, (bool) (NetFieldBase<bool, NetBool>) obj.flipped));
        this.playSound("dropItemInWater");
      }
      else
      {
        if (obj.CanBeSetDown && !flag)
          return false;
        if (obj.ParentSheetIndex >= 0 && obj.Type != null)
        {
          if (obj.Type.Equals("Fish") || obj.Type.Equals("Cooking") || obj.Type.Equals("Crafting"))
            Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(obj.ParentSheetIndex, 150f, 1, 3, dropLocation, true, (bool) (NetFieldBase<bool, NetBool>) obj.flipped));
          else
            Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(obj.ParentSheetIndex + 1, 150f, 1, 3, dropLocation, true, (bool) (NetFieldBase<bool, NetBool>) obj.flipped));
        }
      }
      return true;
    }

    private void rumbleAndFade(int milliseconds) => this.rumbleAndFadeEvent.Fire(milliseconds);

    private void performRumbleAndFade(int milliseconds)
    {
      if (Game1.currentLocation != this)
        return;
      Rumble.rumbleAndFade(1f, (float) milliseconds);
    }

    private void damagePlayers(Microsoft.Xna.Framework.Rectangle area, int damage) => this.damagePlayersEvent.Fire(new GameLocation.DamagePlayersEventArg()
    {
      Area = area,
      Damage = damage
    });

    private void performDamagePlayers(GameLocation.DamagePlayersEventArg arg)
    {
      if (Game1.player.currentLocation != this || !Game1.player.GetBoundingBox().Intersects(arg.Area) || Game1.player.onBridge.Value)
        return;
      Game1.player.takeDamage(arg.Damage, true, (Monster) null);
    }

    public void explode(
      Vector2 tileLocation,
      int radius,
      Farmer who,
      bool damageFarmers = true,
      int damage_amount = -1)
    {
      bool flag = false;
      this.updateMap();
      Vector2 vector2 = new Vector2(Math.Min((float) (this.map.Layers[0].LayerWidth - 1), Math.Max(0.0f, tileLocation.X - (float) radius)), Math.Min((float) (this.map.Layers[0].LayerHeight - 1), Math.Max(0.0f, tileLocation.Y - (float) radius)));
      bool[,] circleOutlineGrid1 = Game1.getCircleOutlineGrid(radius);
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) ((double) tileLocation.X - (double) radius) * 64, (int) ((double) tileLocation.Y - (double) radius) * 64, (radius * 2 + 1) * 64, (radius * 2 + 1) * 64);
      if (damage_amount > 0)
        this.damageMonster(rectangle, damage_amount, damage_amount, true, who);
      else
        this.damageMonster(rectangle, radius * 6, radius * 8, true, who);
      List<TemporaryAnimatedSprite> sprites = new List<TemporaryAnimatedSprite>();
      sprites.Add(new TemporaryAnimatedSprite(23, 9999f, 6, 1, new Vector2(vector2.X * 64f, vector2.Y * 64f), false, Game1.random.NextDouble() < 0.5)
      {
        light = true,
        lightRadius = (float) radius,
        lightcolor = Microsoft.Xna.Framework.Color.Black,
        alphaFade = (float) (0.0299999993294477 - (double) radius * (3.0 / 1000.0)),
        Parent = this
      });
      this.rumbleAndFade(300 + radius * 100);
      if (damageFarmers)
      {
        if (damage_amount > 0)
          this.damagePlayers(rectangle, damage_amount);
        else
          this.damagePlayers(rectangle, radius * 3);
      }
      for (int index = this.terrainFeatures.Count() - 1; index >= 0; --index)
      {
        KeyValuePair<Vector2, TerrainFeature> keyValuePair = this.terrainFeatures.Pairs.ElementAt(index);
        if (keyValuePair.Value.getBoundingBox(keyValuePair.Key).Intersects(rectangle) && keyValuePair.Value.performToolAction((Tool) null, radius / 2, keyValuePair.Key, this))
          this.terrainFeatures.Remove(keyValuePair.Key);
      }
      for (int index1 = 0; index1 < radius * 2 + 1; ++index1)
      {
        for (int index2 = 0; index2 < radius * 2 + 1; ++index2)
        {
          if (index1 == 0 || index2 == 0 || index1 == radius * 2 || index2 == radius * 2)
            flag = circleOutlineGrid1[index1, index2];
          else if (circleOutlineGrid1[index1, index2])
          {
            flag = !flag;
            if (!flag)
            {
              if (this.objects.ContainsKey(vector2) && this.objects[vector2].onExplosion(who, this))
                this.destroyObject(vector2, who);
              if (Game1.random.NextDouble() < 0.45)
              {
                if (Game1.random.NextDouble() < 0.5)
                  sprites.Add(new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, new Vector2(vector2.X * 64f, vector2.Y * 64f), false, Game1.random.NextDouble() < 0.5)
                  {
                    delayBeforeAnimationStart = Game1.random.Next(700)
                  });
                else
                  sprites.Add(new TemporaryAnimatedSprite(5, new Vector2(vector2.X * 64f, vector2.Y * 64f), Microsoft.Xna.Framework.Color.White, animationInterval: 50f)
                  {
                    delayBeforeAnimationStart = Game1.random.Next(200),
                    scale = (float) Game1.random.Next(5, 15) / 10f
                  });
              }
            }
          }
          if (flag)
          {
            this.explosionAt(vector2.X, vector2.Y);
            if (this.objects.ContainsKey(vector2) && this.objects[vector2].onExplosion(who, this))
              this.destroyObject(vector2, who);
            if (Game1.random.NextDouble() < 0.45)
            {
              if (Game1.random.NextDouble() < 0.5)
                sprites.Add(new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, new Vector2(vector2.X * 64f, vector2.Y * 64f), false, Game1.random.NextDouble() < 0.5)
                {
                  delayBeforeAnimationStart = Game1.random.Next(700)
                });
              else
                sprites.Add(new TemporaryAnimatedSprite(5, new Vector2(vector2.X * 64f, vector2.Y * 64f), Microsoft.Xna.Framework.Color.White, animationInterval: 50f)
                {
                  delayBeforeAnimationStart = Game1.random.Next(200),
                  scale = (float) Game1.random.Next(5, 15) / 10f
                });
            }
            sprites.Add(new TemporaryAnimatedSprite(6, new Vector2(vector2.X * 64f, vector2.Y * 64f), Microsoft.Xna.Framework.Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: (Vector2.Distance(vector2, tileLocation) * 20f)));
          }
          ++vector2.Y;
          vector2.Y = Math.Min((float) (this.map.Layers[0].LayerHeight - 1), Math.Max(0.0f, vector2.Y));
        }
        ++vector2.X;
        vector2.Y = Math.Min((float) (this.map.Layers[0].LayerWidth - 1), Math.Max(0.0f, vector2.X));
        vector2.Y = tileLocation.Y - (float) radius;
        vector2.Y = Math.Min((float) (this.map.Layers[0].LayerHeight - 1), Math.Max(0.0f, vector2.Y));
      }
      Game1.multiplayer.broadcastSprites(this, sprites);
      radius /= 2;
      bool[,] circleOutlineGrid2 = Game1.getCircleOutlineGrid(radius);
      vector2 = new Vector2((float) (int) ((double) tileLocation.X - (double) radius), (float) (int) ((double) tileLocation.Y - (double) radius));
      for (int index3 = 0; index3 < radius * 2 + 1; ++index3)
      {
        for (int index4 = 0; index4 < radius * 2 + 1; ++index4)
        {
          if (index3 == 0 || index4 == 0 || index3 == radius * 2 || index4 == radius * 2)
            flag = circleOutlineGrid2[index3, index4];
          else if (circleOutlineGrid2[index3, index4])
          {
            flag = !flag;
            if (!flag && !this.objects.ContainsKey(vector2) && Game1.random.NextDouble() < 0.9 && this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Diggable", "Back") != null && !this.isTileHoeDirt(vector2))
            {
              this.checkForBuriedItem((int) vector2.X, (int) vector2.Y, true, false, who);
              this.makeHoeDirt(vector2);
            }
          }
          if (flag && !this.objects.ContainsKey(vector2) && Game1.random.NextDouble() < 0.9 && this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Diggable", "Back") != null && !this.isTileHoeDirt(vector2))
          {
            this.checkForBuriedItem((int) vector2.X, (int) vector2.Y, true, false, who);
            this.makeHoeDirt(vector2);
          }
          ++vector2.Y;
          vector2.Y = Math.Min((float) (this.map.Layers[0].LayerHeight - 1), Math.Max(0.0f, vector2.Y));
        }
        ++vector2.X;
        vector2.Y = Math.Min((float) (this.map.Layers[0].LayerWidth - 1), Math.Max(0.0f, vector2.X));
        vector2.Y = tileLocation.Y - (float) radius;
        vector2.Y = Math.Min((float) (this.map.Layers[0].LayerHeight - 1), Math.Max(0.0f, vector2.Y));
      }
    }

    public virtual void explosionAt(float x, float y)
    {
    }

    public void removeTemporarySpritesWithID(int id) => this.removeTemporarySpritesWithID((float) id);

    public void removeTemporarySpritesWithID(float id) => this.removeTemporarySpritesWithIDEvent.Fire(id);

    public void removeTemporarySpritesWithIDLocal(float id)
    {
      for (int index = this.temporarySprites.Count - 1; index >= 0; --index)
      {
        if ((double) this.temporarySprites[index].id == (double) id)
        {
          if (this.temporarySprites[index].hasLit)
            Utility.removeLightSource(this.temporarySprites[index].lightID);
          this.temporarySprites.RemoveAt(index);
        }
      }
    }

    public void makeHoeDirt(Vector2 tileLocation, bool ignoreChecks = false)
    {
      if (!ignoreChecks && (this.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "Diggable", "Back") == null || this.isTileOccupied(tileLocation) || !this.isTilePassable(new Location((int) tileLocation.X, (int) tileLocation.Y), Game1.viewport)) || this is MineShaft && (this as MineShaft).getMineArea() == 77377)
        return;
      this.terrainFeatures.Add(tileLocation, (TerrainFeature) new HoeDirt(!Game1.IsRainingHere(this) || !(bool) (NetFieldBase<bool, NetBool>) this.isOutdoors || this.Name.Equals("Desert") ? 0 : 1, this));
    }

    public int numberOfObjectsOfType(int index, bool bigCraftable)
    {
      int num = 0;
      foreach (KeyValuePair<Vector2, Object> pair in this.Objects.Pairs)
      {
        if ((int) (NetFieldBase<int, NetInt>) pair.Value.parentSheetIndex == index && (bool) (NetFieldBase<bool, NetBool>) pair.Value.bigCraftable == bigCraftable)
          ++num;
      }
      return num;
    }

    public void passTimeForObjects(int timeElapsed)
    {
      lock (this._objectUpdateList)
      {
        this._objectUpdateList.Clear();
        foreach (KeyValuePair<Vector2, Object> pair in this.objects.Pairs)
          this._objectUpdateList.Add(pair);
        for (int index = this._objectUpdateList.Count - 1; index >= 0; --index)
        {
          KeyValuePair<Vector2, Object> objectUpdate = this._objectUpdateList[index];
          if (objectUpdate.Value.minutesElapsed(timeElapsed, this))
            this.objects.Remove(objectUpdate.Key);
        }
        this._objectUpdateList.Clear();
      }
    }

    public virtual void performTenMinuteUpdate(int timeOfDay)
    {
      foreach (Object @object in this.furniture)
        @object.minutesElapsed(10, this);
      for (int index = 0; index < this.characters.Count; ++index)
      {
        NPC character = this.characters[index];
        if (!character.IsInvisible)
        {
          character.checkSchedule(timeOfDay);
          character.performTenMinuteUpdate(timeOfDay, this);
        }
      }
      this.passTimeForObjects(10);
      if ((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors)
      {
        Random r = new Random(timeOfDay + (int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed);
        if (this.Equals(Game1.currentLocation))
          this.tryToAddCritters(true);
        if (Game1.IsMasterGame)
        {
          Point point1 = this.fishSplashPoint.Value;
          if (point1.Equals(Point.Zero) && r.NextDouble() < 0.5 && (!(this is Farm) || Game1.whichFarm == 1))
          {
            for (int index = 0; index < 2; ++index)
            {
              Point point2 = new Point(r.Next(0, this.map.GetLayer("Back").LayerWidth), r.Next(0, this.map.GetLayer("Back").LayerHeight));
              if (this.isOpenWater(point2.X, point2.Y) && this.doesTileHaveProperty(point2.X, point2.Y, "NoFishing", "Back") == null)
              {
                int land = FishingRod.distanceToLand(point2.X, point2.Y, this);
                if (land > 1 && land < 5)
                {
                  if (Game1.player.currentLocation.Equals(this))
                    this.playSound("waterSlosh");
                  this.fishSplashPoint.Value = point2;
                  break;
                }
              }
            }
          }
          else
          {
            point1 = this.fishSplashPoint.Value;
            if (!point1.Equals(Point.Zero) && r.NextDouble() < 0.1)
              this.fishSplashPoint.Value = Point.Zero;
          }
          this.performOrePanTenMinuteUpdate(r);
        }
      }
      if (Game1.dayOfMonth % 7 == 0 && this.name.Equals((object) "Saloon") && Game1.timeOfDay >= 1200 && Game1.timeOfDay <= 1500 && NetWorldState.checkAnywhereForWorldStateID("saloonSportsRoom"))
      {
        if (Game1.timeOfDay == 1500)
        {
          this.removeTemporarySpritesWithID(2400);
        }
        else
        {
          bool flag1 = Game1.random.NextDouble() < 0.25;
          bool flag2 = Game1.random.NextDouble() < 0.25;
          List<NPC> npcList = new List<NPC>();
          foreach (NPC character in this.characters)
          {
            if (character.getTileY() < 12 && character.getTileX() > 26 && Game1.random.NextDouble() < (flag1 | flag2 ? 0.66 : 0.25))
              npcList.Add(character);
          }
          foreach (NPC npc in npcList)
          {
            npc.showTextAboveHead(Game1.content.LoadString("Strings\\Characters:Saloon_" + (flag1 ? "goodEvent" : (flag2 ? "badEvent" : "neutralEvent")) + "_" + Game1.random.Next(5).ToString()));
            if (flag1 && Game1.random.NextDouble() < 0.55)
              npc.jump();
          }
        }
      }
      if (!this.name.Equals((object) "BugLand") || Game1.random.NextDouble() > 0.2 || !Game1.currentLocation.Equals(this))
        return;
      this.characters.Add((NPC) new Fly(this.getRandomTile() * 64f, true));
    }

    public virtual void performOrePanTenMinuteUpdate(Random r)
    {
      Point point;
      if (Game1.MasterPlayer.mailReceived.Contains("ccFishTank") && !(this is Beach))
      {
        point = this.orePanPoint.Value;
        if (point.Equals(Point.Zero) && r.NextDouble() < 0.5)
        {
          for (int index = 0; index < 6; ++index)
          {
            Point p = new Point(r.Next(0, this.Map.GetLayer("Back").LayerWidth), r.Next(0, this.Map.GetLayer("Back").LayerHeight));
            if (this.isOpenWater(p.X, p.Y) && FishingRod.distanceToLand(p.X, p.Y, this) <= 1 && this.getTileIndexAt(p, "Buildings") == -1)
            {
              if (Game1.player.currentLocation.Equals(this))
                this.playSound("slosh");
              this.orePanPoint.Value = p;
              break;
            }
          }
          return;
        }
      }
      point = this.orePanPoint.Value;
      if (point.Equals(Point.Zero) || r.NextDouble() >= 0.1)
        return;
      this.orePanPoint.Value = Point.Zero;
    }

    public bool dropObject(Object obj) => this.dropObject(obj, obj.TileLocation, Game1.viewport, false);

    public virtual int getFishingLocation(Vector2 tile) => -1;

    public virtual bool IsUsingMagicBait(Farmer who) => who != null && who.CurrentTool != null && who.CurrentTool is FishingRod && (who.CurrentTool as FishingRod).getBaitAttachmentIndex() == 908;

    public virtual Object getFish(
      float millisecondsAfterNibble,
      int bait,
      int waterDepth,
      Farmer who,
      double baitPotency,
      Vector2 bobberTile,
      string locationName = null)
    {
      int parentSheetIndex = -1;
      Dictionary<string, string> dictionary1 = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
      bool flag1 = false;
      bool flag2 = this.IsUsingMagicBait(who);
      string key = locationName == null ? (string) (NetFieldBase<string, NetString>) this.name : locationName;
      if (key == "BeachNightMarket")
        key = "Beach";
      if (this.name.Equals((object) "WitchSwamp") && !Game1.MasterPlayer.mailReceived.Contains("henchmanGone") && Game1.random.NextDouble() < 0.25 && !Game1.player.hasItemInInventory(308, 1))
        return new Object(308, 1);
      if (dictionary1.ContainsKey(key))
      {
        string[] strArray1 = dictionary1[key].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');
        if (flag2)
        {
          List<string> stringList = new List<string>();
          for (int index = 0; index < 4; ++index)
          {
            if (dictionary1[key].Split('/')[4 + index].Split(' ').Length > 1)
              stringList.AddRange((IEnumerable<string>) dictionary1[key].Split('/')[4 + index].Split(' '));
          }
          strArray1 = stringList.ToArray();
        }
        Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
        if (strArray1.Length > 1)
        {
          for (int index = 0; index < strArray1.Length; index += 2)
            dictionary2[strArray1[index]] = strArray1[index + 1];
        }
        string[] array = dictionary2.Keys.ToArray<string>();
        Dictionary<int, string> dictionary3 = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
        Utility.Shuffle<string>(Game1.random, array);
        for (int index1 = 0; index1 < array.Length; ++index1)
        {
          bool flag3 = true;
          string[] strArray2 = dictionary3[Convert.ToInt32(array[index1])].Split('/');
          string[] strArray3 = strArray2[5].Split(' ');
          int int32 = Convert.ToInt32(dictionary2[array[index1]]);
          if (int32 == -1 || this.getFishingLocation(who.getTileLocation()) == int32)
          {
            for (int index2 = 0; index2 < strArray3.Length; index2 += 2)
            {
              if (Game1.timeOfDay >= Convert.ToInt32(strArray3[index2]) && Game1.timeOfDay < Convert.ToInt32(strArray3[index2 + 1]))
              {
                flag3 = false;
                break;
              }
            }
          }
          if (!strArray2[7].Equals("both"))
          {
            if (strArray2[7].Equals("rainy") && !Game1.IsRainingHere(this))
              flag3 = true;
            else if (strArray2[7].Equals("sunny") && Game1.IsRainingHere(this))
              flag3 = true;
          }
          if (flag2)
            flag3 = false;
          bool flag4 = who != null && who.CurrentTool != null && who.CurrentTool is FishingRod && (int) (NetFieldBase<int, NetInt>) who.CurrentTool.upgradeLevel == 1;
          if (Convert.ToInt32(strArray2[1]) >= 50 & flag4)
            flag3 = true;
          if (who.FishingLevel < Convert.ToInt32(strArray2[12]))
            flag3 = true;
          if (!flag3)
          {
            double num1 = Convert.ToDouble(strArray2[10]);
            double num2 = Convert.ToDouble(strArray2[11]) * num1;
            double val1 = num1 - (double) Math.Max(0, Convert.ToInt32(strArray2[9]) - waterDepth) * num2 + (double) who.FishingLevel / 50.0;
            if (flag4)
              val1 *= 1.1;
            double num3 = Math.Min(val1, 0.899999976158142);
            if (num3 < 0.25 && who != null && who.CurrentTool != null && who.CurrentTool is FishingRod && (who.CurrentTool as FishingRod).getBobberAttachmentIndex() == 856)
            {
              float num4 = 0.25f;
              float num5 = 0.08f;
              num3 = ((double) num4 - (double) num5) / (double) num4 * num3 + ((double) num4 - (double) num5) / 2.0;
            }
            if (Game1.random.NextDouble() <= num3)
            {
              parentSheetIndex = Convert.ToInt32(array[index1]);
              break;
            }
          }
        }
      }
      bool flag5 = false;
      if (parentSheetIndex == -1)
      {
        parentSheetIndex = Game1.random.Next(167, 173);
        flag5 = true;
      }
      if ((who.fishCaught == null || who.fishCaught.Count() == 0) && parentSheetIndex >= 152)
        parentSheetIndex = 145;
      if (!Game1.isFestival() && Game1.random.NextDouble() <= 0.15 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        parentSheetIndex = 890;
      if (who.currentLocation.HasUnlockedAreaSecretNotes(who) & flag5 && Game1.random.NextDouble() < 0.08)
      {
        Object unseenSecretNote = this.tryToCreateUnseenSecretNote(who);
        if (unseenSecretNote != null)
          return unseenSecretNote;
      }
      Object fish = new Object(parentSheetIndex, 1);
      if (flag1)
        fish.scale.X = 1f;
      return fish;
    }

    public virtual bool isActionableTile(int xTile, int yTile, Farmer who)
    {
      bool flag = false;
      string str = this.doesTileHaveProperty(xTile, yTile, "Action", "Buildings");
      if (str != null)
      {
        flag = true;
        if (str.StartsWith("DropBox"))
        {
          flag = false;
          string[] strArray = str.Split(' ');
          if (strArray.Length >= 2 && Game1.player.team.specialOrders != null)
          {
            foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
            {
              if (specialOrder.UsesDropBox(strArray[1]))
              {
                flag = true;
                break;
              }
            }
          }
        }
        if (str.Contains("Message"))
        {
          if (str.Contains("Speech"))
            Game1.isSpeechAtCurrentCursorTile = true;
          else
            Game1.isInspectionAtCurrentCursorTile = true;
        }
      }
      if (this.objects.ContainsKey(new Vector2((float) xTile, (float) yTile)) && this.objects[new Vector2((float) xTile, (float) yTile)].isActionable(who))
        flag = true;
      if (!Game1.isFestival() && this.terrainFeatures.ContainsKey(new Vector2((float) xTile, (float) yTile)) && this.terrainFeatures[new Vector2((float) xTile, (float) yTile)].isActionable())
        flag = true;
      if (flag && !Utility.tileWithinRadiusOfPlayer(xTile, yTile, 1, who))
        Game1.mouseCursorTransparency = 0.5f;
      return flag;
    }

    public virtual void digUpArtifactSpot(int xLocation, int yLocation, Farmer who)
    {
      Random random = new Random(xLocation * 2000 + yLocation + (int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed);
      int objectIndex = -1;
      bool flag1 = who != null && who.CurrentTool != null && who.CurrentTool is Hoe && who.CurrentTool.hasEnchantmentOfType<ArchaeologistEnchantment>();
      foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>) Game1.objectInformation)
      {
        string[] strArray1 = keyValuePair.Value.Split('/');
        if (strArray1[3].Contains("Arch"))
        {
          string[] strArray2 = strArray1[6].Split(' ');
          for (int index = 0; index < strArray2.Length; index += 2)
          {
            if (strArray2[index].Equals((string) (NetFieldBase<string, NetString>) this.name) && random.NextDouble() < (flag1 ? 2.0 : 1.0) * Convert.ToDouble(strArray2[index + 1], (IFormatProvider) CultureInfo.InvariantCulture))
            {
              objectIndex = keyValuePair.Key;
              break;
            }
          }
        }
        if (objectIndex != -1)
          break;
      }
      if (random.NextDouble() < 0.2 && !(this is Farm))
        objectIndex = 102;
      if (objectIndex == 102 && (int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.LostBooksFound >= 21)
        objectIndex = 770;
      if (objectIndex != -1)
      {
        Game1.createObjectDebris(objectIndex, xLocation, yLocation, who.UniqueMultiplayerID);
        who.gainExperience(5, 25);
      }
      else
      {
        bool flag2 = who != null && who.CurrentTool != null && who.CurrentTool is Hoe && who.CurrentTool.hasEnchantmentOfType<GenerousEnchantment>();
        float num1 = 0.5f;
        if (Game1.GetSeasonForLocation(this).Equals("winter") && random.NextDouble() < 0.5 && !(this is Desert))
        {
          if (random.NextDouble() < 0.4)
          {
            Game1.createObjectDebris(416, xLocation, yLocation, who.UniqueMultiplayerID);
            if (!flag2 || random.NextDouble() >= (double) num1)
              return;
            Game1.createObjectDebris(416, xLocation, yLocation, who.UniqueMultiplayerID);
          }
          else
          {
            Game1.createObjectDebris(412, xLocation, yLocation, who.UniqueMultiplayerID);
            if (!flag2 || random.NextDouble() >= (double) num1)
              return;
            Game1.createObjectDebris(412, xLocation, yLocation, who.UniqueMultiplayerID);
          }
        }
        else
        {
          if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
            Game1.createMultipleObjectDebris(890, xLocation, yLocation, random.Next(2, 6), who.UniqueMultiplayerID, this);
          if (Game1.GetSeasonForLocation(this).Equals("spring") && random.NextDouble() < 1.0 / 16.0)
          {
            switch (this)
            {
              case Desert _:
              case Beach _:
                break;
              default:
                Game1.createMultipleObjectDebris(273, xLocation, yLocation, random.Next(2, 6), who.UniqueMultiplayerID, this);
                if (!flag2 || random.NextDouble() >= (double) num1)
                  return;
                Game1.createMultipleObjectDebris(273, xLocation, yLocation, random.Next(2, 6), who.UniqueMultiplayerID, this);
                return;
            }
          }
          if (Game1.random.NextDouble() <= 0.2 && (Game1.MasterPlayer.mailReceived.Contains("guntherBones") || Game1.player.team.specialOrders.Where<SpecialOrder>((Func<SpecialOrder, bool>) (x => (string) (NetFieldBase<string, NetString>) x.questKey == "Gunther")) != null && Game1.player.team.specialOrders.Where<SpecialOrder>((Func<SpecialOrder, bool>) (x => (string) (NetFieldBase<string, NetString>) x.questKey == "Gunther")).Count<SpecialOrder>() > 0))
            Game1.createMultipleObjectDebris(881, xLocation, yLocation, random.Next(2, 6), who.UniqueMultiplayerID, this);
          Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
          if (!dictionary.ContainsKey((string) (NetFieldBase<string, NetString>) this.name))
            return;
          string[] strArray = dictionary[(string) (NetFieldBase<string, NetString>) this.name].Split('/')[8].Split(' ');
          if (strArray.Length == 0 || strArray[0].Equals("-1"))
            return;
          for (int index = 0; index < strArray.Length; index += 2)
          {
            if (random.NextDouble() <= Convert.ToDouble(strArray[index + 1]))
            {
              int num2 = Convert.ToInt32(strArray[index]);
              if (Game1.objectInformation.ContainsKey(num2) && (Game1.objectInformation[num2].Split('/')[3].Contains("Arch") || num2 == 102))
              {
                if (num2 == 102 && (int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.LostBooksFound >= 21)
                  num2 = 770;
                Game1.createObjectDebris(num2, xLocation, yLocation, who.UniqueMultiplayerID);
                break;
              }
              if (num2 == 330 && this.HasUnlockedAreaSecretNotes(who) && Game1.random.NextDouble() < 0.11)
              {
                Object unseenSecretNote = this.tryToCreateUnseenSecretNote(who);
                if (unseenSecretNote != null)
                {
                  Game1.createItemDebris((Item) unseenSecretNote, new Vector2((float) xLocation + 0.5f, (float) yLocation + 0.5f) * 64f, -1, this);
                  break;
                }
              }
              else if (num2 == 330 && Game1.stats.DaysPlayed > 28U && Game1.random.NextDouble() < 0.1)
                Game1.createMultipleObjectDebris(688 + Game1.random.Next(3), xLocation, yLocation, 1, who.UniqueMultiplayerID);
              Game1.createMultipleObjectDebris(num2, xLocation, yLocation, random.Next(1, 4), who.UniqueMultiplayerID);
              if (!flag2 || random.NextDouble() >= (double) num1)
                break;
              Game1.createMultipleObjectDebris(num2, xLocation, yLocation, random.Next(1, 4), who.UniqueMultiplayerID);
              break;
            }
          }
        }
      }
    }

    public virtual string checkForBuriedItem(
      int xLocation,
      int yLocation,
      bool explosion,
      bool detectOnly,
      Farmer who)
    {
      Random random = new Random(xLocation * 2000 + yLocation * 77 + (int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + (int) Game1.stats.DirtHoed);
      string str = this.doesTileHaveProperty(xLocation, yLocation, "Treasure", "Back");
      if (str != null)
      {
        string[] strArray = str.Split(' ');
        if (detectOnly)
          return strArray[0];
        switch (strArray[0])
        {
          case "Arch":
            Game1.createObjectDebris(Convert.ToInt32(strArray[1]), xLocation, yLocation);
            break;
          case "CaveCarrot":
            Game1.createObjectDebris(78, xLocation, yLocation);
            break;
          case "Coal":
            Game1.createDebris(4, xLocation, yLocation, Convert.ToInt32(strArray[1]));
            break;
          case "Coins":
            Game1.createObjectDebris(330, xLocation, yLocation);
            break;
          case "Copper":
            Game1.createDebris(0, xLocation, yLocation, Convert.ToInt32(strArray[1]));
            break;
          case "Gold":
            Game1.createDebris(6, xLocation, yLocation, Convert.ToInt32(strArray[1]));
            break;
          case "Iridium":
            Game1.createDebris(10, xLocation, yLocation, Convert.ToInt32(strArray[1]));
            break;
          case "Iron":
            Game1.createDebris(2, xLocation, yLocation, Convert.ToInt32(strArray[1]));
            break;
          case "Object":
            Game1.createObjectDebris(Convert.ToInt32(strArray[1]), xLocation, yLocation);
            if (Convert.ToInt32(strArray[1]) == 78)
            {
              ++Game1.stats.CaveCarrotsFound;
              break;
            }
            break;
        }
        this.map.GetLayer("Back").Tiles[xLocation, yLocation].Properties["Treasure"] = (PropertyValue) null;
      }
      else
      {
        bool flag = who != null && who.CurrentTool != null && who.CurrentTool is Hoe && who.CurrentTool.hasEnchantmentOfType<GenerousEnchantment>();
        float num = 0.5f;
        if (!(bool) (NetFieldBase<bool, NetBool>) this.isFarm && (bool) (NetFieldBase<bool, NetBool>) this.isOutdoors && Game1.GetSeasonForLocation(this).Equals("winter") && random.NextDouble() < 0.08 && !explosion && !detectOnly && !(this is Desert))
        {
          Game1.createObjectDebris(random.NextDouble() < 0.5 ? 412 : 416, xLocation, yLocation);
          if (flag && random.NextDouble() < (double) num)
            Game1.createObjectDebris(random.NextDouble() < 0.5 ? 412 : 416, xLocation, yLocation);
          return "";
        }
        if ((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors && random.NextDouble() < 0.03 && !explosion)
        {
          if (detectOnly)
          {
            this.map.GetLayer("Back").Tiles[xLocation, yLocation].Properties.Add("Treasure", new PropertyValue("Object " + 330.ToString()));
            return "Object";
          }
          Game1.createObjectDebris(330, xLocation, yLocation);
          if (flag && random.NextDouble() < (double) num)
            Game1.createObjectDebris(330, xLocation, yLocation);
          return "";
        }
      }
      return "";
    }

    public void setAnimatedMapTile(
      int tileX,
      int tileY,
      int[] animationTileIndexes,
      long interval,
      string layer,
      string action,
      int whichTileSheet = 0)
    {
      StaticTile[] tileFrames = new StaticTile[((IEnumerable<int>) animationTileIndexes).Count<int>()];
      for (int index = 0; index < ((IEnumerable<int>) animationTileIndexes).Count<int>(); ++index)
        tileFrames[index] = new StaticTile(this.map.GetLayer(layer), this.map.TileSheets[whichTileSheet], BlendMode.Alpha, animationTileIndexes[index]);
      this.map.GetLayer(layer).Tiles[tileX, tileY] = (Tile) new AnimatedTile(this.map.GetLayer(layer), tileFrames, interval);
      if (action == null)
        return;
      switch (layer)
      {
        case "Buildings":
          this.map.GetLayer("Buildings").Tiles[tileX, tileY].Properties.Add("Action", new PropertyValue(action));
          break;
      }
    }

    public virtual bool AllowMapModificationsInResetState() => false;

    public void setMapTile(
      int tileX,
      int tileY,
      int index,
      string layer,
      string action,
      int whichTileSheet = 0)
    {
      this.map.GetLayer(layer).Tiles[tileX, tileY] = (Tile) new StaticTile(this.map.GetLayer(layer), this.map.TileSheets[whichTileSheet], BlendMode.Alpha, index);
      if (action == null)
        return;
      switch (layer)
      {
        case "Buildings":
          this.map.GetLayer("Buildings").Tiles[tileX, tileY].Properties.Add("Action", new PropertyValue(action));
          break;
      }
    }

    public void setMapTileIndex(
      int tileX,
      int tileY,
      int index,
      string layer,
      int whichTileSheet = 0)
    {
      if (this.map == null)
        return;
      try
      {
        if (this.map.GetLayer(layer).Tiles[tileX, tileY] != null)
        {
          if (index == -1)
            this.map.GetLayer(layer).Tiles[tileX, tileY] = (Tile) null;
          else
            this.map.GetLayer(layer).Tiles[tileX, tileY].TileIndex = index;
        }
        else
        {
          if (index == -1)
            return;
          this.map.GetLayer(layer).Tiles[tileX, tileY] = (Tile) new StaticTile(this.map.GetLayer(layer), this.map.TileSheets[whichTileSheet], BlendMode.Alpha, index);
        }
      }
      catch (Exception ex)
      {
      }
    }

    public virtual void shiftObjects(int dx, int dy)
    {
      List<KeyValuePair<Vector2, Object>> keyValuePairList = new List<KeyValuePair<Vector2, Object>>((IEnumerable<KeyValuePair<Vector2, Object>>) this.objects.Pairs);
      this.objects.Clear();
      foreach (KeyValuePair<Vector2, Object> keyValuePair in keyValuePairList)
      {
        if (keyValuePair.Value.lightSource != null)
          this.removeLightSource((int) (NetFieldBase<int, NetInt>) keyValuePair.Value.lightSource.identifier);
        keyValuePair.Value.tileLocation.Value = new Vector2(keyValuePair.Key.X + (float) dx, keyValuePair.Key.Y + (float) dy);
        this.objects.Add((Vector2) (NetFieldBase<Vector2, NetVector2>) keyValuePair.Value.tileLocation, keyValuePair.Value);
        keyValuePair.Value.initializeLightSource((Vector2) (NetFieldBase<Vector2, NetVector2>) keyValuePair.Value.tileLocation);
      }
    }

    public int getTileIndexAt(Point p, string layer) => this.getTileIndexAt(p.X, p.Y, layer);

    public int getTileIndexAt(int x, int y, string layer)
    {
      Layer layer1 = this.map.GetLayer(layer);
      if (layer1 == null)
        return -1;
      Tile tile = layer1.Tiles[x, y];
      return tile != null ? tile.TileIndex : -1;
    }

    public string getTileSheetIDAt(int x, int y, string layer)
    {
      if (this.map.GetLayer(layer) == null)
        return "";
      Tile tile = this.map.GetLayer(layer).Tiles[x, y];
      return tile != null ? tile.TileSheet.Id : "";
    }

    public void OnStoneDestroyed(int indexOfStone, int x, int y, Farmer who)
    {
      if (who != null && Game1.random.NextDouble() <= 0.02 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        Game1.createMultipleObjectDebris(890, x, y, 1, who.UniqueMultiplayerID, this);
      if (!this.Name.StartsWith("UndergroundMine"))
      {
        if (indexOfStone == 343 || indexOfStone == 450)
        {
          Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 + x * 2000 + y);
          if (random.NextDouble() < 0.035 && Game1.stats.DaysPlayed > 1U)
            Game1.createObjectDebris(535 + (Game1.stats.DaysPlayed <= 60U || random.NextDouble() >= 0.2 ? (Game1.stats.DaysPlayed <= 120U || random.NextDouble() >= 0.2 ? 0 : 2) : 1), x, y, who.UniqueMultiplayerID, this);
          if (random.NextDouble() < 0.035 * (who.professions.Contains(21) ? 2.0 : 1.0) && Game1.stats.DaysPlayed > 1U)
            Game1.createObjectDebris(382, x, y, who.UniqueMultiplayerID, this);
          if (random.NextDouble() < 0.01 && Game1.stats.DaysPlayed > 1U)
            Game1.createObjectDebris(390, x, y, who.UniqueMultiplayerID, this);
        }
        this.breakStone(indexOfStone, x, y, who, new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 + x * 4000 + y));
      }
      else
        (this as MineShaft).checkStoneForItems(indexOfStone, x, y, who);
    }

    protected virtual bool breakStone(int indexOfStone, int x, int y, Farmer who, Random r)
    {
      int howMuch = 0;
      int num1 = who.professions.Contains(18) ? 1 : 0;
      if (indexOfStone == 44)
        indexOfStone = Game1.random.Next(1, 8) * 2;
      if (indexOfStone <= 668)
      {
        if (indexOfStone <= 77)
        {
          switch (indexOfStone - 2)
          {
            case 0:
              Game1.createObjectDebris(72, x, y, (long) who.uniqueMultiplayerID, this);
              howMuch = 150;
              goto label_48;
            case 1:
            case 3:
            case 5:
            case 7:
            case 9:
            case 11:
              goto label_48;
            case 2:
              Game1.createObjectDebris(64, x, y, (long) who.uniqueMultiplayerID, this);
              howMuch = 80;
              goto label_48;
            case 4:
              Game1.createObjectDebris(70, x, y, (long) who.uniqueMultiplayerID, this);
              howMuch = 40;
              goto label_48;
            case 6:
              Game1.createObjectDebris(66, x, y, (long) who.uniqueMultiplayerID, this);
              howMuch = 16;
              goto label_48;
            case 8:
              Game1.createObjectDebris(68, x, y, (long) who.uniqueMultiplayerID, this);
              howMuch = 16;
              goto label_48;
            case 10:
              Game1.createObjectDebris(60, x, y, (long) who.uniqueMultiplayerID, this);
              howMuch = 80;
              goto label_48;
            case 12:
              Game1.createObjectDebris(62, x, y, (long) who.uniqueMultiplayerID, this);
              howMuch = 40;
              goto label_48;
            default:
              if (indexOfStone != 25)
              {
                switch (indexOfStone - 75)
                {
                  case 0:
                    Game1.createObjectDebris(535, x, y, (long) who.uniqueMultiplayerID, this);
                    howMuch = 8;
                    goto label_48;
                  case 1:
                    Game1.createObjectDebris(536, x, y, (long) who.uniqueMultiplayerID, this);
                    howMuch = 16;
                    goto label_48;
                  case 2:
                    Game1.createObjectDebris(537, x, y, (long) who.uniqueMultiplayerID, this);
                    howMuch = 32;
                    goto label_48;
                  default:
                    goto label_48;
                }
              }
              else
              {
                Game1.createMultipleObjectDebris(719, x, y, r.Next(2, 5), (long) who.uniqueMultiplayerID, this);
                howMuch = 5;
                if (this is IslandLocation && r.NextDouble() < 0.1)
                {
                  Game1.player.team.RequestLimitedNutDrops("MusselStone", this, x * 64, y * 64, 5);
                  goto label_48;
                }
                else
                  goto label_48;
              }
          }
        }
        else if (indexOfStone != 95)
        {
          if (indexOfStone != 290)
          {
            if (indexOfStone != 668)
              goto label_48;
          }
          else
            goto label_43;
        }
        else
        {
          Game1.createMultipleObjectDebris(909, x, y, num1 + r.Next(1, 3) + (r.NextDouble() < (double) who.LuckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) who.MiningLevel / 200.0 ? 1 : 0), (long) who.uniqueMultiplayerID, this);
          howMuch = 18;
          goto label_48;
        }
      }
      else
      {
        if (indexOfStone <= 764)
        {
          if (indexOfStone != 670)
          {
            if (indexOfStone != 751)
            {
              if (indexOfStone == 764)
              {
                Game1.createMultipleObjectDebris(384, x, y, num1 + r.Next(1, 4) + (r.NextDouble() < (double) who.LuckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) who.MiningLevel / 100.0 ? 1 : 0), (long) who.uniqueMultiplayerID, this);
                howMuch = 18;
                Game1.multiplayer.broadcastSprites(this, Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(x * 64, (y - 1) * 64, 32, 96), 3, Microsoft.Xna.Framework.Color.Yellow * 0.5f, 175, 100));
                goto label_48;
              }
              else
                goto label_48;
            }
          }
          else
            goto label_40;
        }
        else if (indexOfStone != 765)
        {
          switch (indexOfStone - 816)
          {
            case 0:
            case 1:
              if (r.NextDouble() < 0.1)
                Game1.createObjectDebris(823, x, y, (long) who.uniqueMultiplayerID, this);
              else if (r.NextDouble() < 0.015)
                Game1.createObjectDebris(824, x, y, (long) who.uniqueMultiplayerID, this);
              else if (r.NextDouble() < 0.1)
                Game1.createObjectDebris(579 + r.Next(11), x, y, (long) who.uniqueMultiplayerID, this);
              Game1.createMultipleObjectDebris(881, x, y, num1 + r.Next(1, 3) + (r.NextDouble() < (double) who.LuckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) who.MiningLevel / 100.0 ? 1 : 0), (long) who.uniqueMultiplayerID, this);
              howMuch = 6;
              goto label_48;
            case 2:
              Game1.createMultipleObjectDebris(330, x, y, num1 + r.Next(1, 3) + (r.NextDouble() < (double) who.LuckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) who.MiningLevel / 100.0 ? 1 : 0), (long) who.uniqueMultiplayerID, this);
              howMuch = 6;
              goto label_48;
            case 3:
              Game1.createObjectDebris(749, x, y, (long) who.uniqueMultiplayerID, this);
              howMuch = 64;
              goto label_48;
            default:
              switch (indexOfStone - 843)
              {
                case 0:
                case 1:
                  Game1.createMultipleObjectDebris(848, x, y, num1 + r.Next(1, 3) + (r.NextDouble() < (double) who.LuckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) who.MiningLevel / 200.0 ? 1 : 0), (long) who.uniqueMultiplayerID, this);
                  howMuch = 12;
                  goto label_48;
                case 2:
                case 3:
                case 4:
                  goto label_40;
                case 6:
                  break;
                case 7:
                  goto label_43;
                default:
                  goto label_48;
              }
              break;
          }
        }
        else
        {
          Game1.createMultipleObjectDebris(386, x, y, num1 + r.Next(1, 4) + (r.NextDouble() < (double) who.LuckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) who.MiningLevel / 100.0 ? 1 : 0), (long) who.uniqueMultiplayerID, this);
          Game1.multiplayer.broadcastSprites(this, Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(x * 64, (y - 1) * 64, 32, 96), 6, Microsoft.Xna.Framework.Color.BlueViolet * 0.5f, 175, 100));
          if (r.NextDouble() < 0.04)
            Game1.createMultipleObjectDebris(74, x, y, 1);
          howMuch = 50;
          goto label_48;
        }
        Game1.createMultipleObjectDebris(378, x, y, num1 + r.Next(1, 4) + (r.NextDouble() < (double) who.LuckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) who.MiningLevel / 100.0 ? 1 : 0), (long) who.uniqueMultiplayerID, this);
        howMuch = 5;
        Game1.multiplayer.broadcastSprites(this, Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(x * 64, (y - 1) * 64, 32, 96), 3, Microsoft.Xna.Framework.Color.Orange * 0.5f, 175, 100));
        goto label_48;
      }
label_40:
      Game1.createMultipleObjectDebris(390, x, y, num1 + r.Next(1, 3) + (r.NextDouble() < (double) who.LuckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) who.MiningLevel / 100.0 ? 1 : 0), (long) who.uniqueMultiplayerID, this);
      howMuch = 3;
      if (r.NextDouble() < 0.08)
      {
        Game1.createMultipleObjectDebris(382, x, y, 1 + num1, (long) who.uniqueMultiplayerID, this);
        howMuch = 4;
        goto label_48;
      }
      else
        goto label_48;
label_43:
      Game1.createMultipleObjectDebris(380, x, y, num1 + r.Next(1, 4) + (r.NextDouble() < (double) who.LuckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) who.MiningLevel / 100.0 ? 1 : 0), (long) who.uniqueMultiplayerID, this);
      howMuch = 12;
      Game1.multiplayer.broadcastSprites(this, Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(x * 64, (y - 1) * 64, 32, 96), 3, Microsoft.Xna.Framework.Color.White * 0.5f, 175, 100));
label_48:
      if (who.professions.Contains(19) && r.NextDouble() < 0.5)
      {
        switch (indexOfStone)
        {
          case 2:
            Game1.createObjectDebris(72, x, y, (long) who.uniqueMultiplayerID, this);
            howMuch = 100;
            break;
          case 4:
            Game1.createObjectDebris(64, x, y, (long) who.uniqueMultiplayerID, this);
            howMuch = 50;
            break;
          case 6:
            Game1.createObjectDebris(70, x, y, (long) who.uniqueMultiplayerID, this);
            howMuch = 20;
            break;
          case 8:
            Game1.createObjectDebris(66, x, y, (long) who.uniqueMultiplayerID, this);
            howMuch = 8;
            break;
          case 10:
            Game1.createObjectDebris(68, x, y, (long) who.uniqueMultiplayerID, this);
            howMuch = 8;
            break;
          case 12:
            Game1.createObjectDebris(60, x, y, (long) who.uniqueMultiplayerID, this);
            howMuch = 50;
            break;
          case 14:
            Game1.createObjectDebris(62, x, y, (long) who.uniqueMultiplayerID, this);
            howMuch = 20;
            break;
        }
      }
      if (indexOfStone == 46)
      {
        Game1.createDebris(10, x, y, r.Next(1, 4), this);
        Game1.createDebris(6, x, y, r.Next(1, 5), this);
        if (r.NextDouble() < 0.25)
          Game1.createMultipleObjectDebris(74, x, y, 1, (long) who.uniqueMultiplayerID, this);
        howMuch = 50;
        ++Game1.stats.MysticStonesCrushed;
      }
      if (((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors || (bool) (NetFieldBase<bool, NetBool>) this.treatAsOutdoors) && howMuch == 0)
      {
        double num2 = who.DailyLuck / 2.0 + (double) who.MiningLevel * 0.005 + (double) who.LuckLevel * 0.001;
        Random random = new Random(x * 1000 + y + (int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
        Game1.createDebris(14, x, y, 1, this);
        who.gainExperience(3, 1);
        if (who.professions.Contains(21) && random.NextDouble() < 0.05 * (1.0 + num2))
          Game1.createObjectDebris(382, x, y, who.UniqueMultiplayerID, this);
        if (random.NextDouble() < 0.05 * (1.0 + num2))
        {
          random.Next(1, 3);
          random.NextDouble();
          double num3 = 0.1 * (1.0 + num2);
          Game1.createObjectDebris(382, x, y, who.UniqueMultiplayerID, this);
          Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(25, new Vector2((float) (64 * x), (float) (64 * y)), Microsoft.Xna.Framework.Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 80f, sourceRectHeight: 128));
          who.gainExperience(3, 5);
        }
      }
      if (this.HasUnlockedAreaSecretNotes(who) && r.NextDouble() < 3.0 / 400.0)
      {
        Object unseenSecretNote = this.tryToCreateUnseenSecretNote(who);
        if (unseenSecretNote != null)
          Game1.createItemDebris((Item) unseenSecretNote, new Vector2((float) x + 0.5f, (float) y + 0.75f) * 64f, (int) Game1.player.facingDirection, this);
      }
      who.gainExperience(3, howMuch);
      return howMuch > 0;
    }

    public bool isBehindBush(Vector2 Tile)
    {
      if (this.largeTerrainFeatures != null)
      {
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) Tile.X * 64, (int) ((double) Tile.Y + 1.0) * 64, 64, 128);
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
        {
          if (largeTerrainFeature.getBoundingBox().Intersects(rectangle))
            return true;
        }
      }
      return false;
    }

    public bool isBehindTree(Vector2 Tile)
    {
      if (this.terrainFeatures != null)
      {
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) ((double) Tile.X - 1.0) * 64, (int) Tile.Y * 64, 192, 256);
        foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.terrainFeatures.Pairs)
        {
          if (pair.Value is Tree && pair.Value.getBoundingBox(pair.Key).Intersects(rectangle))
            return true;
        }
      }
      return false;
    }

    public virtual void spawnObjects()
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed);
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
      if (dictionary.ContainsKey((string) (NetFieldBase<string, NetString>) this.name))
      {
        string str = dictionary[(string) (NetFieldBase<string, NetString>) this.name].Split('/')[Utility.getSeasonNumber(this.GetSeasonForLocation())];
        if (!str.Equals("-1") && this.numberOfSpawnedObjectsOnMap < 6)
        {
          string[] strArray = str.Split(' ');
          int num1 = random.Next(1, Math.Min(5, 7 - this.numberOfSpawnedObjectsOnMap));
          for (int index1 = 0; index1 < num1; ++index1)
          {
            for (int index2 = 0; index2 < 11; ++index2)
            {
              int num2 = random.Next(this.map.DisplayWidth / 64);
              int num3 = random.Next(this.map.DisplayHeight / 64);
              Vector2 vector2 = new Vector2((float) num2, (float) num3);
              Object @object;
              this.objects.TryGetValue(vector2, out @object);
              int index3 = random.Next(strArray.Length / 2) * 2;
              if (@object == null && this.doesTileHaveProperty(num2, num3, "Spawnable", "Back") != null && !this.doesEitherTileOrTileIndexPropertyEqual(num2, num3, "Spawnable", "Back", "F") && random.NextDouble() < Convert.ToDouble(strArray[index3 + 1], (IFormatProvider) CultureInfo.InvariantCulture) && this.isTileLocationTotallyClearAndPlaceable(num2, num3) && this.getTileIndexAt(num2, num3, "AlwaysFront") == -1 && this.getTileIndexAt(num2, num3, "Front") == -1 && !this.isBehindBush(vector2) && (Game1.random.NextDouble() < 0.1 || !this.isBehindTree(vector2)) && this.dropObject(new Object(vector2, Convert.ToInt32(strArray[index3]), (string) null, false, true, false, true), new Vector2((float) (num2 * 64), (float) (num3 * 64)), Game1.viewport, true))
              {
                ++this.numberOfSpawnedObjectsOnMap;
                break;
              }
            }
          }
        }
      }
      List<Vector2> source = new List<Vector2>();
      foreach (KeyValuePair<Vector2, Object> pair in this.objects.Pairs)
      {
        if ((int) (NetFieldBase<int, NetInt>) pair.Value.parentSheetIndex == 590)
          source.Add(pair.Key);
      }
      switch (this)
      {
        case Farm _:
        case IslandWest _:
label_19:
          for (int index = source.Count - 1; index >= 0; --index)
          {
            if ((!(this is IslandNorth) || (double) source[index].X >= 26.0) && Game1.random.NextDouble() < 0.15)
            {
              this.objects.Remove(source.ElementAt<Vector2>(index));
              source.RemoveAt(index);
            }
          }
          if (source.Count > (this is Farm ? 0 : 1) && (!this.GetSeasonForLocation().Equals("winter") || source.Count > 4))
            break;
          double num4 = 1.0;
          while (random.NextDouble() < num4)
          {
            int num5 = random.Next(this.map.DisplayWidth / 64);
            int num6 = random.Next(this.map.DisplayHeight / 64);
            Vector2 vector2 = new Vector2((float) num5, (float) num6);
            if (this.isTileLocationTotallyClearAndPlaceable(vector2) && this.getTileIndexAt(num5, num6, "AlwaysFront") == -1 && this.getTileIndexAt(num5, num6, "Front") == -1 && !this.isBehindBush(vector2) && (this.doesTileHaveProperty(num5, num6, "Diggable", "Back") != null || this.GetSeasonForLocation().Equals("winter") && this.doesTileHaveProperty(num5, num6, "Type", "Back") != null && this.doesTileHaveProperty(num5, num6, "Type", "Back").Equals("Grass")))
            {
              if (!this.name.Equals((object) "Forest") || num5 < 93 || num6 > 22)
                this.objects.Add(vector2, new Object(vector2, 590, 1));
              else
                continue;
            }
            num4 *= 0.75;
            if (this.GetSeasonForLocation().Equals("winter"))
              num4 += 0.100000001490116;
          }
          break;
        default:
          this.spawnWeedsAndStones();
          goto label_19;
      }
    }

    public bool isTileLocationOpen(Location location)
    {
      if (this.map.GetLayer("Buildings").Tiles[location.X, location.Y] != null || this.doesTileHaveProperty(location.X, location.Y, "Water", "Back") != null || this.map.GetLayer("Front").Tiles[location.X, location.Y] != null)
        return false;
      return this.map.GetLayer("AlwaysFront") == null || this.map.GetLayer("AlwaysFront").Tiles[location.X, location.Y] == null;
    }

    public bool isTileLocationOpenIgnoreFrontLayers(Location location) => this.map.GetLayer("Buildings").Tiles[location.X, location.Y] == null && this.doesTileHaveProperty(location.X, location.Y, "Water", "Back") == null;

    public void spawnWeedsAndStones(int numDebris = -1, bool weedsOnly = false, bool spawnFromOldWeeds = true)
    {
      switch (this)
      {
        case Farm _:
        case IslandWest _:
          if (Game1.getFarm().isBuildingConstructed("Gold Clock"))
            return;
          break;
      }
      bool flag1 = false;
      if (this is Beach || this.GetSeasonForLocation().Equals("winter") || this is Desert)
        return;
      int num1 = numDebris != -1 ? numDebris : (Game1.random.NextDouble() < 0.95 ? (Game1.random.NextDouble() < 0.25 ? Game1.random.Next(10, 21) : Game1.random.Next(5, 11)) : 0);
      if (Game1.IsRainingHere(this))
        num1 *= 2;
      if (Game1.dayOfMonth == 1)
        num1 *= 5;
      if (this.objects.Count() <= 0 & spawnFromOldWeeds)
        return;
      if (!(this is Farm))
        num1 /= 2;
      for (int index = 0; index < num1; ++index)
      {
        Vector2 vector2_1 = spawnFromOldWeeds ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : new Vector2((float) Game1.random.Next(this.map.Layers[0].LayerWidth), (float) Game1.random.Next(this.map.Layers[0].LayerHeight));
        if (!spawnFromOldWeeds && this is IslandWest)
          vector2_1 = new Vector2((float) Game1.random.Next(57, 97), (float) Game1.random.Next(44, 68));
        while (spawnFromOldWeeds && vector2_1.Equals(Vector2.Zero))
          vector2_1 = new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
        KeyValuePair<Vector2, Object> keyValuePair = new KeyValuePair<Vector2, Object>(Vector2.Zero, (Object) null);
        if (spawnFromOldWeeds)
          keyValuePair = this.objects.Pairs.ElementAt(Game1.random.Next(this.objects.Count()));
        Vector2 vector2_2 = spawnFromOldWeeds ? keyValuePair.Key : Vector2.Zero;
        int num2;
        switch (this)
        {
          case Mountain _ when (double) vector2_1.X + (double) vector2_2.X > 100.0:
          case IslandNorth _:
            continue;
          case Farm _:
            num2 = 1;
            break;
          default:
            num2 = this is IslandWest ? 1 : 0;
            break;
        }
        int xTile = (int) ((double) vector2_1.X + (double) vector2_2.X);
        int yTile = (int) ((double) vector2_1.Y + (double) vector2_2.Y);
        Vector2 vector2_3 = vector2_1 + vector2_2;
        bool flag2 = false;
        int num3 = this.doesTileHaveProperty(xTile, yTile, "Diggable", "Back") != null ? 1 : 0;
        if (num2 == num3 && this.doesTileHaveProperty(xTile, yTile, "NoSpawn", "Back") == null && this.doesTileHaveProperty(xTile, yTile, "Type", "Back") != "Wood")
        {
          bool flag3 = false;
          if (this.isTileLocationTotallyClearAndPlaceable(vector2_3))
            flag3 = true;
          else if (spawnFromOldWeeds)
          {
            if (this.objects.ContainsKey(vector2_3))
            {
              Object @object = this.objects[vector2_3];
              if (!@object.bigCraftable.Value || @object.ParentSheetIndex != 105 && @object.ParentSheetIndex != 264)
                flag3 = true;
            }
            if (!flag3 && this.terrainFeatures.ContainsKey(vector2_3))
            {
              TerrainFeature terrainFeature = this.terrainFeatures[vector2_3];
              if (terrainFeature is HoeDirt || terrainFeature is Flooring)
                flag3 = true;
            }
          }
          if (flag3)
          {
            if (spawnFromOldWeeds)
              flag2 = true;
            else if (!this.objects.ContainsKey(vector2_3))
              flag2 = true;
          }
        }
        if (flag2)
        {
          int parentSheetIndex = -1;
          if (this is Desert)
          {
            parentSheetIndex = 750;
          }
          else
          {
            if (Game1.random.NextDouble() < 0.5 && !weedsOnly && (!spawnFromOldWeeds || keyValuePair.Value.Name.Equals("Stone") || keyValuePair.Value.Name.Contains("Twig")))
              parentSheetIndex = Game1.random.NextDouble() >= 0.5 ? (Game1.random.NextDouble() < 0.5 ? 343 : 450) : (Game1.random.NextDouble() < 0.5 ? 294 : 295);
            else if (!spawnFromOldWeeds || keyValuePair.Value.Name.Contains("Weed"))
              parentSheetIndex = GameLocation.getWeedForSeason(Game1.random, this.GetSeasonForLocation());
            if (this is Farm && !spawnFromOldWeeds && Game1.random.NextDouble() < 0.05)
            {
              this.terrainFeatures.Add(vector2_3, (TerrainFeature) new Tree(Game1.random.Next(3) + 1, Game1.random.Next(3)));
              continue;
            }
          }
          if (parentSheetIndex != -1)
          {
            bool flag4 = false;
            if (this.objects.ContainsKey(vector2_1 + vector2_2))
            {
              Object @object = this.objects[vector2_1 + vector2_2];
              if (!(@object is Fence) && !(@object is Chest))
              {
                if (@object.name != null && !@object.Name.Contains("Weed") && !@object.Name.Equals("Stone") && !@object.name.Contains("Twig") && @object.name.Length > 0)
                {
                  flag4 = true;
                  Game1.debugOutput = @object.Name + " was destroyed";
                }
                this.objects.Remove(vector2_1 + vector2_2);
              }
              else
                continue;
            }
            if (this.terrainFeatures.ContainsKey(vector2_1 + vector2_2))
            {
              try
              {
                flag4 = this.terrainFeatures[vector2_1 + vector2_2] is HoeDirt || this.terrainFeatures[vector2_1 + vector2_2] is Flooring;
              }
              catch (Exception ex)
              {
              }
              if (!flag4)
                break;
              this.terrainFeatures.Remove(vector2_1 + vector2_2);
            }
            if (flag4 && this is Farm && Game1.stats.DaysPlayed > 1U && !flag1)
            {
              flag1 = true;
              Game1.multiplayer.broadcastGlobalMessage("Strings\\Locations:Farm_WeedsDestruction", false);
            }
            this.objects.Add(vector2_1 + vector2_2, new Object(vector2_1 + vector2_2, parentSheetIndex, 1));
          }
        }
      }
    }

    public virtual void removeEverythingExceptCharactersFromThisTile(int x, int y)
    {
      Vector2 key = new Vector2((float) x, (float) y);
      if (this.terrainFeatures.ContainsKey(key))
        this.terrainFeatures.Remove(key);
      if (!this.objects.ContainsKey(key))
        return;
      this.objects.Remove(key);
    }

    public virtual string getFootstepSoundReplacement(string footstep) => footstep;

    public virtual void removeEverythingFromThisTile(int x, int y)
    {
      for (int index = this.resourceClumps.Count - 1; index >= 0; --index)
      {
        if ((double) this.resourceClumps[index].tile.X == (double) x && (double) this.resourceClumps[index].tile.Y == (double) y)
          this.resourceClumps.RemoveAt(index);
      }
      Vector2 vector2 = new Vector2((float) x, (float) y);
      if (this.terrainFeatures.ContainsKey(vector2))
        this.terrainFeatures.Remove(vector2);
      if (this.objects.ContainsKey(vector2))
        this.objects.Remove(vector2);
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        if (this.characters[index].getTileLocation().Equals(vector2) && this.characters[index] is Monster)
          this.characters.RemoveAt(index);
      }
    }

    public virtual Dictionary<string, string> GetLocationEvents()
    {
      string str1 = (string) (NetFieldBase<string, NetString>) this.name;
      if ((NetFieldBase<string, NetString>) this.uniqueName != (NetString) null && this.uniqueName.Value != null && this.uniqueName.Value.Equals(Game1.player.homeLocation.Value))
        str1 = "FarmHouse";
      Dictionary<string, string> locationEvents = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + str1);
      if (this.Name == "IslandFarmHouse")
      {
        locationEvents = new Dictionary<string, string>((IDictionary<string, string>) locationEvents);
        Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\FarmHouse");
        foreach (string key in dictionary.Keys)
        {
          string str2 = dictionary[key];
          if (key.StartsWith("558291/") || key.StartsWith("558292/"))
            locationEvents[key] = str2;
        }
      }
      else if (this.Name == "Trailer_Big")
      {
        locationEvents = new Dictionary<string, string>((IDictionary<string, string>) locationEvents);
        Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\Trailer");
        if (dictionary != null)
        {
          foreach (string key in dictionary.Keys)
          {
            string str3 = dictionary[key];
            if (!(str1 == "Trailer_Big") || !locationEvents.ContainsKey(key))
            {
              if (key.StartsWith("36/"))
                str3 = str3.Replace("/farmer -30 30 0", "/farmer 12 19 0").Replace("/playSound doorClose/warp farmer 12 9", "/move farmer 0 -10 0");
              else if (key.StartsWith("35/"))
                str3 = str3.Replace("/farmer -30 30 0", "/farmer 12 19 0").Replace("/warp farmer 12 9/playSound doorClose", "/move farmer 0 -10 0").Replace("/warp farmer -40 -40/playSound doorClose", "/move farmer 0 10 0/warp farmer -40 -40");
              locationEvents[key] = str3;
            }
          }
        }
      }
      return locationEvents;
    }

    public virtual void checkForEvents()
    {
      if (Game1.killScreen && !Game1.eventUp)
      {
        if (this.name.Equals((object) "Mine"))
        {
          string sub1;
          string sub2;
          switch (Game1.random.Next(7))
          {
            case 0:
              sub1 = "Robin";
              sub2 = "Data\\ExtraDialogue:Mines_PlayerKilled_Robin";
              break;
            case 1:
              sub1 = "Clint";
              sub2 = "Data\\ExtraDialogue:Mines_PlayerKilled_Clint";
              break;
            case 2:
              sub1 = "Maru";
              sub2 = Game1.player.spouse == null || !Game1.player.spouse.Equals("Maru") ? "Data\\ExtraDialogue:Mines_PlayerKilled_Maru_NotSpouse" : "Data\\ExtraDialogue:Mines_PlayerKilled_Maru_Spouse";
              break;
            default:
              sub1 = "Linus";
              sub2 = "Data\\ExtraDialogue:Mines_PlayerKilled_Linus";
              break;
          }
          if (Game1.random.NextDouble() < 0.1 && Game1.player.spouse != null && !Game1.player.isEngaged() && Game1.player.spouse.Length > 1)
          {
            sub1 = Game1.player.spouse;
            sub2 = Game1.player.IsMale ? "Data\\ExtraDialogue:Mines_PlayerKilled_Spouse_PlayerMale" : "Data\\ExtraDialogue:Mines_PlayerKilled_Spouse_PlayerFemale";
          }
          this.currentEvent = new Event(Game1.content.LoadString("Data\\Events\\Mine:PlayerKilled", (object) sub1, (object) sub2, (object) Game1.player.Name));
        }
        else if (this is IslandLocation)
        {
          string sub1 = "Willy";
          string sub2 = "Data\\ExtraDialogue:Island_willy_rescue";
          if (Game1.player.friendshipData.ContainsKey("Leo") && Game1.random.NextDouble() < 0.5)
          {
            sub1 = "Leo";
            sub2 = "Data\\ExtraDialogue:Island_leo_rescue";
          }
          this.currentEvent = new Event(Game1.content.LoadString("Data\\Events\\IslandSouth:PlayerKilled", (object) sub1, (object) sub2, (object) Game1.player.Name));
        }
        else if (this.name.Equals((object) "Hospital"))
          this.currentEvent = new Event(Game1.content.LoadString("Data\\Events\\Hospital:PlayerKilled", (object) Game1.player.Name));
        Game1.eventUp = true;
        Game1.killScreen = false;
        Game1.player.health = 10;
      }
      else if (!Game1.eventUp && Game1.weddingsToday.Count > 0 && (Game1.CurrentEvent == null || Game1.CurrentEvent.id != -2) && Game1.currentLocation != null && Game1.currentLocation.Name != "Temp")
      {
        this.currentEvent = Game1.getAvailableWeddingEvent();
        if (this.currentEvent == null)
          return;
        this.startEvent(this.currentEvent);
      }
      else
      {
        if (Game1.eventUp || Game1.farmEvent != null)
          return;
        string festival = Game1.currentSeason + Game1.dayOfMonth.ToString();
        try
        {
          Event @event = new Event();
          if (@event.tryToLoadFestival(festival))
            this.currentEvent = @event;
        }
        catch (Exception ex)
        {
        }
        if (!Game1.eventUp && this.currentEvent == null && Game1.farmEvent == null)
        {
          Dictionary<string, string> locationEvents;
          try
          {
            string name = (string) (NetFieldBase<string, NetString>) this.name;
            locationEvents = this.GetLocationEvents();
          }
          catch (Exception ex)
          {
            return;
          }
          if (locationEvents != null)
          {
            string[] array = locationEvents.Keys.ToArray<string>();
            for (int index = 0; index < array.Length; ++index)
            {
              int eventID = this.checkEventPrecondition(array[index]);
              if (eventID != -1)
              {
                this.currentEvent = new Event(locationEvents[array[index]], eventID);
                break;
              }
            }
            if (this.currentEvent == null && this.name.Equals((object) "Farm") && Game1.IsMasterGame && !Game1.player.mailReceived.Contains("rejectedPet") && Game1.stats.DaysPlayed >= 20U && !Game1.player.hasPet())
            {
              for (int index = 0; index < array.Length; ++index)
              {
                if (array[index].Contains("dog") && !Game1.player.catPerson || array[index].Contains("cat") && Game1.player.catPerson)
                {
                  this.currentEvent = new Event(locationEvents[array[index]]);
                  Game1.player.eventsSeen.Add(Convert.ToInt32(array[index].Split('/')[0]));
                  break;
                }
              }
            }
          }
        }
        if (this.currentEvent == null)
          return;
        this.startEvent(this.currentEvent);
      }
    }

    public Event findEventById(int id, Farmer farmerActor = null)
    {
      if (id == -2)
      {
        long? spouse = Game1.player.team.GetSpouse(farmerActor.UniqueMultiplayerID);
        if (farmerActor == null || !spouse.HasValue)
          return Utility.getWeddingEvent(farmerActor);
        if (Game1.otherFarmers.ContainsKey(spouse.Value))
        {
          Farmer otherFarmer = Game1.otherFarmers[spouse.Value];
          return Utility.getPlayerWeddingEvent(farmerActor, otherFarmer);
        }
      }
      Dictionary<string, string> locationEvents;
      try
      {
        locationEvents = this.GetLocationEvents();
      }
      catch (Exception ex)
      {
        return (Event) null;
      }
      foreach (KeyValuePair<string, string> keyValuePair in locationEvents)
      {
        if (keyValuePair.Key.Split('/')[0] == id.ToString())
          return new Event(keyValuePair.Value, id, farmerActor);
      }
      return (Event) null;
    }

    public virtual void startEvent(Event evt)
    {
      if (Game1.eventUp || Game1.eventOver)
        return;
      this.currentEvent = evt;
      if (evt.exitLocation == null)
        evt.exitLocation = Game1.getLocationRequest((bool) (NetFieldBase<bool, NetBool>) this.isStructure ? this.uniqueName.Value : this.Name, (bool) (NetFieldBase<bool, NetBool>) this.isStructure);
      if (Game1.player.mount != null)
      {
        Horse mount = Game1.player.mount;
        mount.currentLocation = this;
        mount.dismount();
        Microsoft.Xna.Framework.Rectangle boundingBox = mount.GetBoundingBox();
        Vector2 position = mount.Position;
        if (mount.currentLocation != null && mount.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, (Character) mount, true))
        {
          boundingBox.X -= 64;
          if (!mount.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, (Character) mount, true))
          {
            position.X -= 64f;
            mount.Position = position;
          }
          else
          {
            boundingBox.X += 128;
            if (!mount.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, (Character) mount, true))
            {
              position.X += 64f;
              mount.Position = position;
            }
          }
        }
      }
      foreach (NPC character in this.characters)
        character.clearTextAboveHead();
      Game1.eventUp = true;
      Game1.displayHUD = false;
      Game1.player.CanMove = false;
      Game1.player.showNotCarrying();
      if (this.critters == null)
        return;
      this.critters.Clear();
    }

    public virtual void drawBackground(SpriteBatch b)
    {
    }

    public virtual void drawWater(SpriteBatch b)
    {
      if (this.currentEvent != null)
        this.currentEvent.drawUnderWater(b);
      if (this.waterTiles == null)
        return;
      for (int y = Math.Max(0, Game1.viewport.Y / 64 - 1); y < Math.Min(this.map.Layers[0].LayerHeight, (Game1.viewport.Y + Game1.viewport.Height) / 64 + 2); ++y)
      {
        for (int x = Math.Max(0, Game1.viewport.X / 64 - 1); x < Math.Min(this.map.Layers[0].LayerWidth, (Game1.viewport.X + Game1.viewport.Width) / 64 + 1); ++x)
        {
          if (this.waterTiles.waterTiles[x, y].isWater && this.waterTiles.waterTiles[x, y].isVisible)
            this.drawWaterTile(b, x, y);
        }
      }
    }

    public virtual void drawWaterTile(SpriteBatch b, int x, int y) => this.drawWaterTile(b, x, y, (Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) this.waterColor);

    public void drawWaterTile(SpriteBatch b, int x, int y, Microsoft.Xna.Framework.Color color)
    {
      int num = y == this.map.Layers[0].LayerHeight - 1 ? 1 : (!this.waterTiles[x, y + 1] ? 1 : 0);
      bool flag = y == 0 || !this.waterTiles[x, y - 1];
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - (!flag ? (int) this.waterPosition : 0)))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.waterAnimationIndex * 64, 2064 + ((x + y) % 2 == 0 ? (this.waterTileFlip ? 128 : 0) : (this.waterTileFlip ? 0 : 128)) + (flag ? (int) this.waterPosition : 0), 64, 64 + (flag ? (int) -(double) this.waterPosition : 0))), color, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.56f);
      if (num == 0)
        return;
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) ((y + 1) * 64 - (int) this.waterPosition))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.waterAnimationIndex * 64, 2064 + ((x + (y + 1)) % 2 == 0 ? (this.waterTileFlip ? 128 : 0) : (this.waterTileFlip ? 0 : 128)), 64, 64 - (int) (64.0 - (double) this.waterPosition) - 1)), color, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.56f);
    }

    public virtual void drawFloorDecorations(SpriteBatch b)
    {
      if (!Game1.isFestival())
      {
        Vector2 vector2 = new Vector2();
        for (int index1 = Game1.viewport.Y / 64 - 1; index1 < (Game1.viewport.Y + Game1.viewport.Height) / 64 + 7; ++index1)
        {
          for (int index2 = Game1.viewport.X / 64 - 1; index2 < (Game1.viewport.X + Game1.viewport.Width) / 64 + 3; ++index2)
          {
            vector2.X = (float) index2;
            vector2.Y = (float) index1;
            TerrainFeature terrainFeature;
            if (this.terrainFeatures.TryGetValue(vector2, out terrainFeature) && terrainFeature is Flooring)
              terrainFeature.draw(b, vector2);
          }
        }
      }
      if (Game1.eventUp)
      {
        switch (this)
        {
          case Farm _:
          case FarmHouse _:
            break;
          default:
            return;
        }
      }
      Furniture.isDrawingLocationFurniture = true;
      foreach (Furniture furniture in this.furniture)
      {
        if (furniture.furniture_type.Value == 12)
          furniture.draw(b, -1, -1, 1f);
      }
      Furniture.isDrawingLocationFurniture = false;
    }

    public TemporaryAnimatedSprite getTemporarySpriteByID(int id)
    {
      for (int index = 0; index < this.temporarySprites.Count; ++index)
      {
        if ((double) this.temporarySprites[index].id == (double) id)
          return this.temporarySprites[index];
      }
      return (TemporaryAnimatedSprite) null;
    }

    protected void drawDebris(SpriteBatch b)
    {
      int num = 0;
      foreach (Debris debri in this.debris)
      {
        ++num;
        Microsoft.Xna.Framework.Rectangle bounds;
        if (debri.item != null)
        {
          if (debri.item is Object && (bool) (NetFieldBase<bool, NetBool>) (debri.item as Object).bigCraftable)
            debri.item.drawInMenu(b, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) debri.Chunks[0].position + new Vector2(32f, 32f))), 1.6f, 1f, (float) (((double) (debri.chunkFinalYLevel + 64 + 8) + (double) debri.Chunks[0].position.X / 10000.0) / 10000.0), StackDrawType.Hide, Microsoft.Xna.Framework.Color.White, true);
          else
            debri.item.drawInMenu(b, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) debri.Chunks[0].position + new Vector2(32f, 32f))), (float) (0.800000011920929 + (double) debri.itemQuality * 0.100000001490116), 1f, (float) (((double) (debri.chunkFinalYLevel + 64 + 8) + (double) debri.Chunks[0].position.X / 10000.0) / 10000.0), StackDrawType.Hide, Microsoft.Xna.Framework.Color.White, true);
        }
        else if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) debri.debrisType == Debris.DebrisType.LETTERS)
          Game1.drawWithBorder((string) (NetFieldBase<string, NetString>) debri.debrisMessage, Microsoft.Xna.Framework.Color.Black, (Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) debri.nonSpriteChunkColor, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) debri.Chunks[0].position)), debri.Chunks[0].rotation, debri.Chunks[0].scale, (float) (((double) debri.Chunks[0].position.Y + 64.0) / 10000.0));
        else if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) debri.debrisType == Debris.DebrisType.NUMBERS)
          NumberSprite.draw((int) (NetFieldBase<int, NetInt>) debri.chunkType, b, Game1.GlobalToLocal(Game1.viewport, Utility.snapDrawPosition(new Vector2(debri.Chunks[0].position.X, (float) debri.chunkFinalYLevel - ((float) debri.chunkFinalYLevel - debri.Chunks[0].position.Y)))), (Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) debri.nonSpriteChunkColor, debri.Chunks[0].scale * 0.75f, (float) (0.980000019073486 + 9.99999974737875E-05 * (double) num), debri.Chunks[0].alpha, -1 * (int) ((double) debri.chunkFinalYLevel - (double) debri.Chunks[0].position.Y) / 2);
        else if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) debri.debrisType == Debris.DebrisType.SPRITECHUNKS)
        {
          for (int index = 0; index < debri.Chunks.Count; ++index)
            b.Draw(debri.spriteChunkSheet, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) debri.Chunks[index].position)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int) (NetFieldBase<int, NetInt>) debri.Chunks[index].xSpriteSheet, (int) (NetFieldBase<int, NetInt>) debri.Chunks[index].ySpriteSheet, Math.Min((int) (NetFieldBase<int, NetInt>) debri.sizeOfSourceRectSquares, debri.spriteChunkSheet.Bounds.Width), Math.Min((int) (NetFieldBase<int, NetInt>) debri.sizeOfSourceRectSquares, debri.spriteChunkSheet.Bounds.Height))), debri.nonSpriteChunkColor.Value * debri.Chunks[index].alpha, debri.Chunks[index].rotation, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) debri.sizeOfSourceRectSquares / 2), (float) ((int) (NetFieldBase<int, NetInt>) debri.sizeOfSourceRectSquares / 2)), debri.Chunks[index].scale, SpriteEffects.None, (float) (((double) (debri.chunkFinalYLevel + 16) + (double) debri.Chunks[index].position.X / 10000.0) / 10000.0));
        }
        else if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) debri.debrisType == Debris.DebrisType.SQUARES)
        {
          for (int index = 0; index < debri.Chunks.Count; ++index)
            b.Draw(Game1.littleEffect, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) debri.Chunks[index].position)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 4, 4)), (Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) debri.nonSpriteChunkColor, 0.0f, Vector2.Zero, (float) (1.0 + (double) (float) (NetFieldBase<float, NetFloat>) debri.Chunks[index].yVelocity / 2.0), SpriteEffects.None, (float) (((double) debri.Chunks[index].position.Y + 64.0) / 10000.0));
        }
        else if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) debri.debrisType != Debris.DebrisType.CHUNKS)
        {
          for (int index = 0; index < debri.Chunks.Count; ++index)
          {
            if (debri.Chunks[index].debrisType <= 0)
            {
              b.Draw(Game1.bigCraftableSpriteSheet, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) debri.Chunks[index].position + new Vector2(32f, 64f))), new Microsoft.Xna.Framework.Rectangle?(Game1.getArbitrarySourceRect(Game1.bigCraftableSpriteSheet, 16, 32, -debri.Chunks[index].debrisType)), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(8f, 32f), 3.2f, SpriteEffects.None, (float) (((double) (debri.chunkFinalYLevel + 48) + (double) debri.Chunks[index].position.X / 10000.0) / 10000.0));
              SpriteBatch spriteBatch = b;
              Texture2D shadowTexture = Game1.shadowTexture;
              Vector2 position = Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, new Vector2(debri.Chunks[index].position.X + 25.6f, (float) ((debri.chunksMoveTowardPlayer ? (double) debri.Chunks[index].position.Y + 8.0 : (double) debri.chunkFinalYLevel) + 32.0))));
              Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
              Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
              bounds = Game1.shadowTexture.Bounds;
              double x = (double) bounds.Center.X;
              bounds = Game1.shadowTexture.Bounds;
              double y = (double) bounds.Center.Y;
              Vector2 origin = new Vector2((float) x, (float) y);
              double scale = (double) Math.Min(3f, (float) (3.0 - (debri.chunksMoveTowardPlayer ? 0.0 : ((double) debri.chunkFinalYLevel - (double) debri.Chunks[index].position.Y) / 128.0)));
              double layerDepth = (double) debri.chunkFinalYLevel / 10000.0;
              spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
            }
            else
            {
              b.Draw(Game1.objectSpriteSheet, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) debri.Chunks[index].position)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, debri.Chunks[index].debrisType, 16, 16)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) debri.debrisType == Debris.DebrisType.RESOURCE || (bool) (NetFieldBase<bool, NetBool>) debri.floppingFish ? 4f : (float) (4.0 * (0.800000011920929 + (double) debri.itemQuality * 0.100000001490116)), !(bool) (NetFieldBase<bool, NetBool>) debri.floppingFish || debri.Chunks[index].bounces % 2 != 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, (float) (((double) (debri.chunkFinalYLevel + 32) + (double) debri.Chunks[index].position.X / 10000.0) / 10000.0));
              SpriteBatch spriteBatch = b;
              Texture2D shadowTexture = Game1.shadowTexture;
              Vector2 position = Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, new Vector2(debri.Chunks[index].position.X + 25.6f, (float) ((debri.chunksMoveTowardPlayer ? (double) debri.Chunks[index].position.Y + 8.0 : (double) debri.chunkFinalYLevel) + 32.0) + (float) (12 * debri.itemQuality))));
              Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
              Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White * 0.75f;
              bounds = Game1.shadowTexture.Bounds;
              double x = (double) bounds.Center.X;
              bounds = Game1.shadowTexture.Bounds;
              double y = (double) bounds.Center.Y;
              Vector2 origin = new Vector2((float) x, (float) y);
              double scale = (double) Math.Min(3f, (float) (3.0 - (debri.chunksMoveTowardPlayer ? 0.0 : ((double) debri.chunkFinalYLevel - (double) debri.Chunks[index].position.Y) / 96.0)));
              double layerDepth = (double) debri.chunkFinalYLevel / 10000.0;
              spriteBatch.Draw(shadowTexture, position, sourceRectangle, color, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
            }
          }
        }
        else
        {
          for (int index = 0; index < debri.Chunks.Count; ++index)
            b.Draw(Game1.debrisSpriteSheet, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) debri.Chunks[index].position)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, debri.Chunks[index].debrisType, 16, 16)), (Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) debri.chunksColor, 0.0f, Vector2.Zero, 4f * (float) (NetFieldBase<float, NetFloat>) debri.scale, SpriteEffects.None, (float) (((double) debri.Chunks[index].position.Y + 128.0 + (double) debri.Chunks[index].position.X / 10000.0) / 10000.0));
        }
      }
    }

    public virtual bool shouldHideCharacters() => false;

    protected virtual void drawCharacters(SpriteBatch b)
    {
      if (this.shouldHideCharacters() || Game1.eventUp && (Game1.CurrentEvent == null || !Game1.CurrentEvent.showWorldCharacters))
        return;
      for (int index = 0; index < this.characters.Count; ++index)
      {
        if (this.characters[index] != null)
          this.characters[index].draw(b);
      }
    }

    protected virtual void drawFarmers(SpriteBatch b)
    {
      if (this.shouldHideCharacters() || Game1.currentMinigame != null)
        return;
      if (this.currentEvent == null || this.currentEvent.isFestival || this.currentEvent.farmerActors.Count == 0)
      {
        foreach (Farmer farmer in this.farmers)
        {
          if (!Game1.multiplayer.isDisconnecting(farmer.UniqueMultiplayerID))
            farmer.draw(b);
        }
      }
      else
        this.currentEvent.drawFarmers(b);
    }

    public virtual void DrawFarmerUsernames(SpriteBatch b)
    {
      if (this.shouldHideCharacters() || Game1.currentMinigame != null || this.currentEvent != null && !this.currentEvent.isFestival && this.currentEvent.farmerActors.Count != 0)
        return;
      foreach (Farmer farmer in this.farmers)
      {
        if (!Game1.multiplayer.isDisconnecting(farmer.UniqueMultiplayerID))
          farmer.DrawUsername(b);
      }
    }

    public virtual void draw(SpriteBatch b)
    {
      foreach (MapSeat mapSeat in this.mapSeats)
        mapSeat.Draw(b);
      foreach (ResourceClump resourceClump in this.resourceClumps)
        resourceClump.draw(b, (Vector2) (NetFieldBase<Vector2, NetVector2>) resourceClump.tile);
      List<Farmer> farmerList = new List<Farmer>();
      foreach (Farmer farmer in this.farmers)
      {
        farmer.drawLayerDisambiguator = 0.0f;
        farmerList.Add(farmer);
      }
      if (farmerList.Contains(Game1.player))
      {
        farmerList.Remove(Game1.player);
        farmerList.Insert(0, Game1.player);
      }
      float num1 = 0.0001f;
      for (int index1 = 0; index1 < farmerList.Count; ++index1)
      {
        for (int index2 = index1 + 1; index2 < farmerList.Count; ++index2)
        {
          Farmer farmer1 = farmerList[index1];
          Farmer farmer2 = farmerList[index2];
          if (!farmer2.IsSitting() && (double) Math.Abs(farmer1.getDrawLayer() - farmer2.getDrawLayer()) < (double) num1 && (double) Math.Abs(farmer1.position.X - farmer2.position.X) < 64.0)
            farmer2.drawLayerDisambiguator += farmer1.getDrawLayer() - num1 - farmer2.getDrawLayer();
        }
      }
      this.drawCharacters(b);
      this.drawFarmers(b);
      if (this.critters != null && Game1.farmEvent == null)
      {
        for (int index = 0; index < this.critters.Count; ++index)
          this.critters[index].draw(b);
      }
      this.drawDebris(b);
      if (!Game1.eventUp || this.currentEvent != null && this.currentEvent.showGroundObjects)
      {
        Vector2 key = new Vector2();
        for (int index3 = Game1.viewport.Y / 64 - 1; index3 < (Game1.viewport.Y + Game1.viewport.Height) / 64 + 3; ++index3)
        {
          for (int index4 = Game1.viewport.X / 64 - 1; index4 < (Game1.viewport.X + Game1.viewport.Width) / 64 + 1; ++index4)
          {
            key.X = (float) index4;
            key.Y = (float) index3;
            Object @object;
            if (this.objects.TryGetValue(key, out @object))
              @object.draw(b, (int) key.X, (int) key.Y);
          }
        }
      }
      foreach (TemporaryAnimatedSprite temporarySprite in this.TemporarySprites)
      {
        if (!temporarySprite.drawAboveAlwaysFront)
          temporarySprite.draw(b);
      }
      this.interiorDoors.Draw(b);
      if (this.largeTerrainFeatures != null)
      {
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
          largeTerrainFeature.draw(b);
      }
      if (this.fishSplashAnimation != null)
        this.fishSplashAnimation.draw(b);
      if (this.orePanAnimation != null)
        this.orePanAnimation.draw(b);
      if (!Game1.eventUp || this is Farm || this is FarmHouse)
      {
        Furniture.isDrawingLocationFurniture = true;
        foreach (Furniture furniture in this.furniture)
        {
          if (furniture.furniture_type.Value != 12)
            furniture.draw(b, -1, -1, 1f);
        }
        Furniture.isDrawingLocationFurniture = false;
      }
      if (!this.showDropboxIndicator || Game1.eventUp)
        return;
      float num2 = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(this.dropBoxIndicatorLocation.X, this.dropBoxIndicatorLocation.Y + num2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(114, 53, 6, 10)), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(1f, 4f), 4f, SpriteEffects.None, 1f);
    }

    public virtual void drawAboveFrontLayer(SpriteBatch b)
    {
      if (!Game1.isFestival())
      {
        Vector2 vector2 = new Vector2();
        for (int index1 = Game1.viewport.Y / 64 - 1; index1 < (Game1.viewport.Y + Game1.viewport.Height) / 64 + 7; ++index1)
        {
          for (int index2 = Game1.viewport.X / 64 - 1; index2 < (Game1.viewport.X + Game1.viewport.Width) / 64 + 3; ++index2)
          {
            vector2.X = (float) index2;
            vector2.Y = (float) index1;
            TerrainFeature terrainFeature;
            if (this.terrainFeatures.TryGetValue(vector2, out terrainFeature) && !(terrainFeature is Flooring))
              terrainFeature.draw(b, vector2);
          }
        }
      }
      if (!(this is MineShaft))
      {
        foreach (NPC character in this.characters)
        {
          if (character is Monster monster)
            monster.drawAboveAllLayers(b);
        }
      }
      if (this.lightGlows.Count > 0)
        this.drawLightGlows(b);
      this.DrawFarmerUsernames(b);
    }

    public virtual void drawLightGlows(SpriteBatch b)
    {
      foreach (Vector2 lightGlow in this.lightGlows)
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, lightGlow), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(21, 1695, 41, 67)), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(19f, 22f), 4f, SpriteEffects.None, 1f);
    }

    public Object tryToCreateUnseenSecretNote(Farmer who)
    {
      bool flag = this.GetLocationContext() == GameLocation.LocationContext.Island;
      if (who != null && ((!who.hasMagnifyingGlass ? 0 : (!flag ? 1 : 0)) | (flag ? 1 : 0)) != 0)
      {
        Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\SecretNotes");
        int num1 = 0;
        foreach (int key in dictionary.Keys)
        {
          if (flag)
          {
            if (key >= GameLocation.JOURNAL_INDEX)
              ++num1;
          }
          else if (key < GameLocation.JOURNAL_INDEX)
            ++num1;
        }
        GameLocation.secretNotesSeen.Clear();
        foreach (int num2 in (NetList<int, NetInt>) who.secretNotesSeen)
        {
          if (num2 < GameLocation.JOURNAL_INDEX)
            GameLocation.secretNotesSeen.Add(num2);
          else
            GameLocation.journalsSeen.Add(num2);
        }
        int num3 = !flag ? GameLocation.secretNotesSeen.Count : GameLocation.journalsSeen.Count;
        if (num3 == num1)
          return (Object) null;
        float num4 = (float) (num1 - 1 - num3) / (float) Math.Max(1, num1 - 1);
        float num5 = GameLocation.LAST_SECRET_NOTE_CHANCE + (GameLocation.FIRST_SECRET_NOTE_CHANCE - GameLocation.LAST_SECRET_NOTE_CHANCE) * num4;
        if (Game1.random.NextDouble() < (double) num5)
        {
          GameLocation.unseenJournals.Clear();
          GameLocation.unseenSecretNotes.Clear();
          foreach (int key in dictionary.Keys)
          {
            if (key < GameLocation.JOURNAL_INDEX && !GameLocation.secretNotesSeen.Contains(key) && !who.hasItemInInventoryNamed("Secret Note #" + key.ToString()) && (key != 10 || who.mailReceived.Contains("QiChallengeComplete")))
              GameLocation.unseenSecretNotes.Add(key);
            else if (key >= GameLocation.JOURNAL_INDEX && !GameLocation.journalsSeen.Contains(key) && !who.hasItemInInventoryNamed("Journal Scrap #" + (key - GameLocation.JOURNAL_INDEX).ToString()))
              GameLocation.unseenJournals.Add(key);
          }
          if (!flag && GameLocation.unseenSecretNotes.Count > 0)
          {
            int unseenSecretNote1 = GameLocation.unseenSecretNotes[Game1.random.Next(GameLocation.unseenSecretNotes.Count)];
            Object unseenSecretNote2 = new Object(79, 1);
            unseenSecretNote2.name = unseenSecretNote2.name + " #" + unseenSecretNote1.ToString();
            return unseenSecretNote2;
          }
          if (flag && GameLocation.unseenJournals.Count > 0)
          {
            int num6 = GameLocation.unseenJournals.First<int>();
            Object unseenSecretNote = new Object(842, 1);
            unseenSecretNote.name = unseenSecretNote.name + " #" + (num6 - GameLocation.JOURNAL_INDEX).ToString();
            return unseenSecretNote;
          }
        }
      }
      return (Object) null;
    }

    public virtual bool performToolAction(Tool t, int tileX, int tileY)
    {
      for (int index = this.resourceClumps.Count - 1; index >= 0; --index)
      {
        if (this.resourceClumps[index] != null && this.resourceClumps[index].getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) this.resourceClumps[index].tile).Contains(tileX * 64, tileY * 64) && this.resourceClumps[index].performToolAction(t, 1, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.resourceClumps[index].tile, this))
        {
          this.resourceClumps.RemoveAt(index);
          return true;
        }
      }
      return false;
    }

    public virtual void seasonUpdate(string season, bool onLoad = false)
    {
      for (int index = this.terrainFeatures.Count() - 1; index >= 0; --index)
      {
        if (this.terrainFeatures.Values.ElementAt<TerrainFeature>(index).seasonUpdate(onLoad))
          this.terrainFeatures.Remove(this.terrainFeatures.Keys.ElementAt<Vector2>(index));
      }
      if (this.largeTerrainFeatures != null)
      {
        for (int index = this.largeTerrainFeatures.Count - 1; index >= 0; --index)
        {
          if (this.largeTerrainFeatures.ElementAt<LargeTerrainFeature>(index).seasonUpdate(onLoad))
            this.largeTerrainFeatures.Remove(this.largeTerrainFeatures.ElementAt<LargeTerrainFeature>(index));
        }
      }
      foreach (NPC character in this.getCharacters())
      {
        if (!character.IsMonster)
          character.resetSeasonalDialogue();
      }
      if (this.IsOutdoors && !onLoad)
      {
        for (int index = this.objects.Count() - 1; index >= 0; --index)
        {
          OverlaidDictionary.PairsCollection pairs = this.objects.Pairs;
          KeyValuePair<Vector2, Object> keyValuePair = pairs.ElementAt(index);
          if (keyValuePair.Value.IsSpawnedObject)
          {
            pairs = this.objects.Pairs;
            keyValuePair = pairs.ElementAt(index);
            if (!keyValuePair.Value.Name.Equals("Stone"))
            {
              OverlaidDictionary objects = this.objects;
              pairs = this.objects.Pairs;
              keyValuePair = pairs.ElementAt(index);
              Vector2 key = keyValuePair.Key;
              objects.Remove(key);
              continue;
            }
          }
          pairs = this.objects.Pairs;
          keyValuePair = pairs.ElementAt(index);
          if ((int) (NetFieldBase<int, NetInt>) keyValuePair.Value.parentSheetIndex == 590)
          {
            pairs = this.objects.Pairs;
            keyValuePair = pairs.ElementAt(index);
            int x = (int) keyValuePair.Key.X;
            pairs = this.objects.Pairs;
            keyValuePair = pairs.ElementAt(index);
            int y = (int) keyValuePair.Key.Y;
            if (this.doesTileHavePropertyNoNull(x, y, "Diggable", "Back") == "")
            {
              OverlaidDictionary objects = this.objects;
              pairs = this.objects.Pairs;
              keyValuePair = pairs.ElementAt(index);
              Vector2 key = keyValuePair.Key;
              objects.Remove(key);
            }
          }
        }
        this.numberOfSpawnedObjectsOnMap = 0;
      }
      string lower = season.ToLower();
      if (!(lower == "spring"))
      {
        if (!(lower == "summer"))
        {
          if (!(lower == "fall"))
          {
            if (lower == "winter")
              this.waterColor.Value = new Microsoft.Xna.Framework.Color(130, 80, (int) byte.MaxValue) * 0.5f;
          }
          else
            this.waterColor.Value = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 130, 200) * 0.5f;
        }
        else
          this.waterColor.Value = new Microsoft.Xna.Framework.Color(60, 240, (int) byte.MaxValue) * 0.5f;
      }
      else
        this.waterColor.Value = new Microsoft.Xna.Framework.Color(120, 200, (int) byte.MaxValue) * 0.5f;
      if (onLoad || !(season == "spring") || Game1.stats.daysPlayed <= 1U || this is Farm)
        return;
      this.loadWeeds();
    }

    public virtual void updateSeasonalTileSheets(Map map = null)
    {
      if (map == null)
        map = this.Map;
      if (!(this is Summit) && (!this.IsOutdoors || this.Name.Equals("Desert")))
        return;
      map.DisposeTileSheets(Game1.mapDisplayDevice);
      for (int index = 0; index < map.TileSheets.Count; ++index)
        map.TileSheets[index].ImageSource = GameLocation.GetSeasonalTilesheetName(map.TileSheets[index].ImageSource, this.GetSeasonForLocation());
      map.LoadTileSheets(Game1.mapDisplayDevice);
    }

    public static string GetSeasonalTilesheetName(string sheet_path, string current_season)
    {
      string fileName = Path.GetFileName(sheet_path);
      if (fileName.StartsWith("spring_") || fileName.StartsWith("summer_") || fileName.StartsWith("fall_") || fileName.StartsWith("winter_"))
        sheet_path = Path.Combine(Path.GetDirectoryName(sheet_path), current_season + fileName.Substring(fileName.IndexOf('_')));
      return sheet_path;
    }

    public virtual int checkEventPrecondition(string precondition)
    {
      string[] strArray1 = precondition.Split(GameLocation.ForwardSlash);
      int result;
      if (!int.TryParse(strArray1[0], out result) || Game1.player.eventsSeen.Contains(result))
        return -1;
      for (int index1 = 1; index1 < strArray1.Length; ++index1)
      {
        if (strArray1[index1][0] == 'e')
        {
          if (this.checkEventsSeenPreconditions(strArray1[index1].Split(' ')))
            return -1;
        }
        else if (strArray1[index1][0] == 'h')
        {
          if (Game1.player.hasPet() || Game1.player.catPerson && !strArray1[index1].Split(' ')[1].ToString().ToLower().Equals("cat") || !Game1.player.catPerson && !strArray1[index1].Split(' ')[1].ToString().ToLower().Equals("dog"))
            return -1;
        }
        else if (strArray1[index1][0] == 'H')
        {
          string[] strArray2 = strArray1[index1].Split(' ');
          if (strArray2[0] == "H")
          {
            if (!Game1.IsMasterGame)
              return -1;
          }
          else if (strArray2[0] == "Hn")
          {
            if (!Game1.MasterPlayer.mailReceived.Contains(strArray2[1]))
              return -1;
          }
          else if (strArray2[0] == "Hl" && Game1.MasterPlayer.mailReceived.Contains(strArray2[1]))
            return -1;
        }
        else if (strArray1[index1][0] == '*')
        {
          string[] strArray3 = strArray1[index1].Split(' ');
          if (strArray3[0] == "*")
          {
            if (!NetWorldState.checkAnywhereForWorldStateID(strArray3[1]))
              return -1;
          }
          else if (strArray3[0] == "*n")
          {
            if (!Game1.MasterPlayer.mailReceived.Contains(strArray3[1]) && !Game1.player.mailReceived.Contains(strArray3[1]))
              return -1;
          }
          else if (strArray3[0] == "*l" && (Game1.MasterPlayer.mailReceived.Contains(strArray3[1]) || Game1.player.mailReceived.Contains(strArray3[1])))
            return -1;
        }
        else if (strArray1[index1][0] == 'm')
        {
          if ((long) Game1.player.totalMoneyEarned < (long) Convert.ToInt32(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'M')
        {
          if (Game1.player.Money < Convert.ToInt32(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'c')
        {
          if (Game1.player.freeSpotsInInventory() < Convert.ToInt32(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'C')
        {
          if (!Game1.MasterPlayer.eventsSeen.Contains(191393) && !Game1.MasterPlayer.eventsSeen.Contains(502261) && !Game1.MasterPlayer.hasCompletedCommunityCenter())
            return -1;
        }
        else if (strArray1[index1][0] == 'X')
        {
          if (Game1.MasterPlayer.eventsSeen.Contains(191393) || Game1.MasterPlayer.eventsSeen.Contains(502261) || Game1.MasterPlayer.hasCompletedCommunityCenter())
            return -1;
        }
        else if (strArray1[index1][0] == 'D')
        {
          string key = strArray1[index1].Split(' ')[1];
          if (!Game1.player.friendshipData.ContainsKey(key) || !Game1.player.friendshipData[key].IsDating())
            return -1;
        }
        else if (strArray1[index1][0] == 'j')
        {
          if ((long) Game1.stats.DaysPlayed <= (long) Convert.ToInt32(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'J')
        {
          if (!this.checkJojaCompletePrerequisite())
            return -1;
        }
        else if (strArray1[index1][0] == 'f')
        {
          if (!this.checkFriendshipPrecondition(strArray1[index1].Split(' ')))
            return -1;
        }
        else if (strArray1[index1][0] == 'F')
        {
          if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
            return -1;
        }
        else if (strArray1[index1][0] == 'r')
        {
          string[] strArray4 = strArray1[index1].Split(' ');
          if (Game1.random.NextDouble() > Convert.ToDouble(strArray4[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 's')
        {
          if (!this.checkItemsPrecondition(strArray1[index1].Split(' ')))
            return -1;
        }
        else if (strArray1[index1][0] == 'S')
        {
          if (!Game1.player.secretNotesSeen.Contains(Convert.ToInt32(strArray1[index1].Split(' ')[1])))
            return -1;
        }
        else if (strArray1[index1][0] == 'q')
        {
          if (!this.checkDialoguePrecondition(strArray1[index1].Split(' ')))
            return -1;
        }
        else if (strArray1[index1][0] == 'n')
        {
          if (!Game1.player.mailReceived.Contains(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'N')
        {
          if (Game1.netWorldState.Value.GoldenWalnutsFound.Value < Convert.ToInt32(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'l')
        {
          if (Game1.player.mailReceived.Contains(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'L')
        {
          if (!(this is FarmHouse) || (this as FarmHouse).upgradeLevel < 2)
            return -1;
        }
        else if (strArray1[index1][0] == 't')
        {
          string[] strArray5 = strArray1[index1].Split(' ');
          if (Game1.timeOfDay < Convert.ToInt32(strArray5[1]) || Game1.timeOfDay > Convert.ToInt32(strArray5[2]))
            return -1;
        }
        else if (strArray1[index1][0] == 'w')
        {
          string[] strArray6 = strArray1[index1].Split(' ');
          if (strArray6[1].Equals("rainy") && !Game1.IsRainingHere(this) || strArray6[1].Equals("sunny") && Game1.IsRainingHere(this))
            return -1;
        }
        else if (strArray1[index1][0] == 'd')
        {
          if (((IEnumerable<string>) strArray1[index1].Split(' ')).Contains<string>(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
            return -1;
        }
        else if (strArray1[index1][0] == 'o')
        {
          if (Game1.player.spouse != null && Game1.player.spouse.Equals(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'O')
        {
          if (Game1.player.spouse == null || !Game1.player.spouse.Equals(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'R')
        {
          if (strArray1[index1] == "Rf")
          {
            if (Game1.player.hasCurrentOrPendingRoommate())
              return -1;
          }
          else if (!Game1.player.hasCurrentOrPendingRoommate())
            return -1;
        }
        else if (strArray1[index1][0] == 'v')
        {
          if (Game1.getCharacterFromName(strArray1[index1].Split(' ')[1]).IsInvisible)
            return -1;
        }
        else if (strArray1[index1][0] == 'p')
        {
          if (!this.isCharacterHere(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'z')
        {
          string[] strArray7 = strArray1[index1].Split(' ');
          if (Game1.currentSeason.Equals(strArray7[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'B')
        {
          if (Utility.getHomeOfFarmer(Game1.player).GetSpouseBed() == null)
            return -1;
        }
        else if (strArray1[index1][0] == 'b')
        {
          if (Game1.player.timesReachedMineBottom < Convert.ToInt32(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else if (strArray1[index1][0] == 'y')
        {
          if (Game1.year < Convert.ToInt32(strArray1[index1].Split(' ')[1]) || Convert.ToInt32(strArray1[index1].Split(' ')[1]) == 1 && Game1.year != 1)
            return -1;
        }
        else if (strArray1[index1][0] == 'g')
        {
          if (!(Game1.player.IsMale ? "male" : "female").Equals(strArray1[index1].Split(' ')[1].ToLower()))
            return -1;
        }
        else if (strArray1[index1][0] == 'i')
        {
          if (!Game1.player.hasItemInInventory(Convert.ToInt32(strArray1[index1].Split(' ')[1]), 1) && (Game1.player.ActiveObject == null || Game1.player.ActiveObject.ParentSheetIndex != Convert.ToInt32(strArray1[index1].Split(' ')[1])))
            return -1;
        }
        else if (strArray1[index1][0] == 'k')
        {
          if (!this.checkEventsSeenPreconditions(strArray1[index1].Split(' ')))
            return -1;
        }
        else if (strArray1[index1][0] == 'a')
        {
          bool flag = false;
          string[] strArray8 = strArray1[index1].Split(' ');
          for (int index2 = 1; index2 < strArray8.Length - 1; index2 += 2)
          {
            if (Game1.xLocationAfterWarp == Convert.ToInt32(strArray8[index2]) && Game1.yLocationAfterWarp == Convert.ToInt32(strArray8[index2 + 1]))
              flag = true;
          }
          if (!flag)
            return -1;
        }
        else if (strArray1[index1][0] == 'A')
        {
          if (Game1.player.activeDialogueEvents.ContainsKey(strArray1[index1].Split(' ')[1]))
            return -1;
        }
        else
        {
          if (strArray1[index1][0] == 'x')
          {
            string[] strArray9 = strArray1[index1].Split(' ');
            if (strArray9.Length == 2)
              Game1.addMailForTomorrow(strArray9[1]);
            else
              Game1.player.mailbox.Add(strArray9[1]);
            Game1.player.eventsSeen.Add(Convert.ToInt32(strArray1[0]));
            return -1;
          }
          if (strArray1[index1][0] == 'u')
          {
            bool flag = false;
            string[] strArray10 = strArray1[index1].Split(' ');
            for (int index3 = 1; index3 < strArray10.Length; ++index3)
            {
              if (Game1.dayOfMonth == Convert.ToInt32(strArray10[index3]))
              {
                flag = true;
                break;
              }
            }
            if (!flag)
              return -1;
          }
          else
          {
            if (strArray1[index1][0] != 'U')
              return -1;
            int int32 = Convert.ToInt32(strArray1[index1].Split(' ')[1]);
            string str = Game1.currentSeason;
            int day = Game1.dayOfMonth;
            for (int index4 = 0; index4 < int32; ++index4)
            {
              if (Utility.isFestivalDay(day, str))
                return -1;
              ++day;
              if (day > 28)
              {
                day = 1;
                str = Utility.getSeasonNameFromNumber((Utility.getSeasonNumber(str) + 1) % 4).ToLower();
              }
            }
          }
        }
      }
      return result;
    }

    private bool isCharacterHere(string name)
    {
      foreach (NPC character in this.characters)
      {
        if (character.Name.Equals(name) && !character.IsInvisible)
          return true;
      }
      return false;
    }

    private bool checkJojaCompletePrerequisite() => Utility.hasFinishedJojaRoute();

    private bool checkEventsSeenPreconditions(string[] eventIDs)
    {
      for (int index = 1; index < eventIDs.Length; ++index)
      {
        if (int.TryParse(eventIDs[index], out int _) && Game1.player.eventsSeen.Contains(Convert.ToInt32(eventIDs[index])))
          return false;
      }
      return true;
    }

    private bool checkFriendshipPrecondition(string[] friendshipString)
    {
      for (int index = 1; index < friendshipString.Length; index += 2)
      {
        if (!Game1.player.friendshipData.ContainsKey(friendshipString[index]) || Game1.player.friendshipData[friendshipString[index]].Points < Convert.ToInt32(friendshipString[index + 1]))
          return false;
      }
      return true;
    }

    private bool checkItemsPrecondition(string[] itemString)
    {
      for (int index = 1; index < itemString.Length; index += 2)
      {
        if (!Game1.player.basicShipped.ContainsKey(Convert.ToInt32(itemString[index])) || Game1.player.basicShipped[Convert.ToInt32(itemString[index])] < Convert.ToInt32(itemString[index + 1]))
          return false;
      }
      return true;
    }

    private bool checkDialoguePrecondition(string[] dialogueString)
    {
      for (int index = 1; index < dialogueString.Length; index += 2)
      {
        if (!Game1.player.DialogueQuestionsAnswered.Contains(Convert.ToInt32(dialogueString[index])))
          return false;
      }
      return true;
    }

    public virtual void updateWarps()
    {
      this.warps.Clear();
      PropertyValue propertyValue1;
      this.map.Properties.TryGetValue("NPCWarp", out propertyValue1);
      if (propertyValue1 != null)
      {
        string[] strArray = propertyValue1.ToString().Split(' ');
        for (int index = 0; index < strArray.Length; index += 5)
          this.warps.Add(new Warp(Convert.ToInt32(strArray[index]), Convert.ToInt32(strArray[index + 1]), strArray[index + 2], Convert.ToInt32(strArray[index + 3]), Convert.ToInt32(strArray[index + 4]), false, true));
      }
      PropertyValue propertyValue2 = (PropertyValue) null;
      this.map.Properties.TryGetValue("Warp", out propertyValue2);
      if (propertyValue2 == null)
        return;
      string[] strArray1 = propertyValue2.ToString().Split(' ');
      for (int index = 0; index < strArray1.Length; index += 5)
        this.warps.Add(new Warp(Convert.ToInt32(strArray1[index]), Convert.ToInt32(strArray1[index + 1]), strArray1[index + 2], Convert.ToInt32(strArray1[index + 3]), Convert.ToInt32(strArray1[index + 4]), false));
    }

    public void loadWeeds()
    {
      if (this.map == null)
        return;
      bool flag = false;
      foreach (Component layer in this.map.Layers)
      {
        if (layer.Id.Equals("Paths"))
        {
          flag = true;
          break;
        }
      }
      if ((((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors ? 1 : ((bool) (NetFieldBase<bool, NetBool>) this.treatAsOutdoors ? 1 : 0)) & (flag ? 1 : 0)) == 0)
        return;
      for (int x = 0; x < this.map.Layers[0].LayerWidth; ++x)
      {
        for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
        {
          Tile tile = this.map.GetLayer("Paths").Tiles[x, y];
          if (tile != null)
          {
            Vector2 vector2 = new Vector2((float) x, (float) y);
            switch (tile.TileIndex)
            {
              case 13:
              case 14:
              case 15:
                if (this.CanLoadPathObjectHere(vector2))
                {
                  this.objects.Add(vector2, new Object(vector2, GameLocation.getWeedForSeason(Game1.random, this.GetSeasonForLocation()), 1));
                  continue;
                }
                continue;
              case 16:
                if (this.CanLoadPathObjectHere(vector2))
                {
                  this.objects.Add(vector2, new Object(vector2, Game1.random.NextDouble() < 0.5 ? 343 : 450, 1));
                  continue;
                }
                continue;
              case 17:
                if (this.CanLoadPathObjectHere(vector2))
                {
                  this.objects.Add(vector2, new Object(vector2, Game1.random.NextDouble() < 0.5 ? 343 : 450, 1));
                  continue;
                }
                continue;
              case 18:
                if (this.CanLoadPathObjectHere(vector2))
                {
                  this.objects.Add(vector2, new Object(vector2, Game1.random.NextDouble() < 0.5 ? 294 : 295, 1));
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
      }
    }

    public bool CanLoadPathObjectHere(Vector2 tile)
    {
      if (this.objects.ContainsKey(tile) || this.terrainFeatures.ContainsKey(tile) || this.getLargeTerrainFeatureAt((int) tile.X, (int) tile.Y) != null)
        return false;
      Vector2 vector2 = tile * 64f;
      vector2.X += 32f;
      vector2.Y += 32f;
      foreach (Furniture furniture in this.furniture)
      {
        if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type != 12 && !furniture.isPassable() && furniture.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture.tileLocation).Contains((int) vector2.X, (int) vector2.Y) && !furniture.AllowPlacementOnThisTile((int) tile.X, (int) tile.Y))
          return false;
      }
      return true;
    }

    public void loadObjects()
    {
      this._startingCabinLocations.Clear();
      if (this.map == null)
        return;
      this.updateWarps();
      PropertyValue propertyValue1;
      this.map.Properties.TryGetValue(Game1.currentSeason.Substring(0, 1).ToUpper() + Game1.currentSeason.Substring(1) + "_Objects", out propertyValue1);
      bool flag = false;
      foreach (Component layer in this.map.Layers)
      {
        if (layer.Id.Equals("Paths"))
        {
          flag = true;
          break;
        }
      }
      PropertyValue propertyValue2;
      this.map.Properties.TryGetValue("Trees", out propertyValue2);
      if (propertyValue2 != null)
      {
        string[] strArray = propertyValue2.ToString().Split(' ');
        for (int index = 0; index < strArray.Length; index += 3)
          this.terrainFeatures.Add(new Vector2((float) Convert.ToInt32(strArray[index]), (float) Convert.ToInt32(strArray[index + 1])), (TerrainFeature) new Tree(Convert.ToInt32(strArray[index + 2]) + 1, 5));
      }
      if (this is FishShop || this is SeedShop)
        this.updateDoors();
      if ((((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors || this.name.Equals((object) "BathHouse_Entry") || (bool) (NetFieldBase<bool, NetBool>) this.treatAsOutdoors ? 1 : (this.map.Properties.ContainsKey("forceLoadObjects") ? 1 : 0)) & (flag ? 1 : 0)) == 0)
        return;
      for (int x = 0; x < this.map.Layers[0].LayerWidth; ++x)
      {
        for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
        {
          Tile tile = this.map.GetLayer("Paths").Tiles[x, y];
          if (tile != null)
          {
            Vector2 vector2 = new Vector2((float) x, (float) y);
            int which = -1;
            switch (tile.TileIndex)
            {
              case 9:
                which = 1;
                if (Game1.currentSeason.Equals("winter"))
                {
                  which += 3;
                  break;
                }
                break;
              case 10:
                which = 2;
                if (Game1.currentSeason.Equals("winter"))
                {
                  which += 3;
                  break;
                }
                break;
              case 11:
                which = 3;
                break;
              case 12:
                which = 6;
                break;
              case 31:
                which = 9;
                break;
              case 32:
                which = 8;
                break;
            }
            if (which != -1)
            {
              if (this.GetFurnitureAt(vector2) == null && !this.terrainFeatures.ContainsKey(vector2) && !this.objects.ContainsKey(vector2))
                this.terrainFeatures.Add(vector2, (TerrainFeature) new Tree(which, 5));
            }
            else
            {
              switch (tile.TileIndex)
              {
                case 13:
                case 14:
                case 15:
                  if (!this.objects.ContainsKey(vector2))
                  {
                    this.objects.Add(vector2, new Object(vector2, GameLocation.getWeedForSeason(Game1.random, this.GetSeasonForLocation()), 1));
                    continue;
                  }
                  continue;
                case 16:
                  if (!this.objects.ContainsKey(vector2))
                  {
                    this.objects.Add(vector2, new Object(vector2, Game1.random.NextDouble() < 0.5 ? 343 : 450, 1));
                    continue;
                  }
                  continue;
                case 17:
                  if (!this.objects.ContainsKey(vector2))
                  {
                    this.objects.Add(vector2, new Object(vector2, Game1.random.NextDouble() < 0.5 ? 343 : 450, 1));
                    continue;
                  }
                  continue;
                case 18:
                  if (!this.objects.ContainsKey(vector2))
                  {
                    this.objects.Add(vector2, new Object(vector2, Game1.random.NextDouble() < 0.5 ? 294 : 295, 1));
                    continue;
                  }
                  continue;
                case 19:
                  this.addResourceClumpAndRemoveUnderlyingTerrain(602, 2, 2, vector2);
                  continue;
                case 20:
                  this.addResourceClumpAndRemoveUnderlyingTerrain(672, 2, 2, vector2);
                  continue;
                case 21:
                  this.addResourceClumpAndRemoveUnderlyingTerrain(600, 2, 2, vector2);
                  continue;
                case 22:
                  if (!this.terrainFeatures.ContainsKey(vector2))
                  {
                    this.terrainFeatures.Add(vector2, (TerrainFeature) new Grass(1, 3));
                    continue;
                  }
                  continue;
                case 23:
                  if (!this.terrainFeatures.ContainsKey(vector2))
                  {
                    this.terrainFeatures.Add(vector2, (TerrainFeature) new Tree(Game1.random.Next(1, 4), Game1.random.Next(2, 4)));
                    continue;
                  }
                  continue;
                case 24:
                  if (!this.terrainFeatures.ContainsKey(vector2))
                  {
                    this.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(vector2, 2, this));
                    continue;
                  }
                  continue;
                case 25:
                  if (!this.terrainFeatures.ContainsKey(vector2))
                  {
                    this.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(vector2, 1, this));
                    continue;
                  }
                  continue;
                case 26:
                  if (!this.terrainFeatures.ContainsKey(vector2))
                  {
                    this.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(vector2, 0, this));
                    continue;
                  }
                  continue;
                case 27:
                  this.changeMapProperties("BrookSounds", vector2.X.ToString() + " " + vector2.Y.ToString() + " 0");
                  continue;
                case 28:
                  if ((string) (NetFieldBase<string, NetString>) this.name == "BugLand")
                    continue;
                  continue;
                case 29:
                case 30:
                  if (Game1.startingCabins > 0)
                  {
                    PropertyValue propertyValue3 = (PropertyValue) null;
                    tile.Properties.TryGetValue("Order", out propertyValue3);
                    if (propertyValue3 != null && int.Parse(propertyValue3.ToString()) <= Game1.startingCabins && (tile.TileIndex == 29 && !Game1.cabinsSeparate || tile.TileIndex == 30 && Game1.cabinsSeparate))
                    {
                      this._startingCabinLocations.Add(vector2);
                      continue;
                    }
                    continue;
                  }
                  continue;
                case 33:
                  if (!this.terrainFeatures.ContainsKey(vector2))
                  {
                    this.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(vector2, 4, this));
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            }
          }
        }
      }
      if (propertyValue1 != null && !Game1.eventUp)
        this.spawnObjects();
      this.updateDoors();
    }

    public void BuildStartingCabins()
    {
      if (this._startingCabinLocations.Count > 0)
      {
        List<string> stringList = new List<string>();
        switch (Game1.whichFarm)
        {
          case 1:
            stringList.Add("Plank Cabin");
            stringList.Add("Log Cabin");
            stringList.Add("Stone Cabin");
            break;
          case 3:
          case 4:
            stringList.Add("Stone Cabin");
            stringList.Add("Log Cabin");
            stringList.Add("Plank Cabin");
            break;
          default:
            bool flag = Game1.random.NextDouble() < 0.5;
            stringList.Add(flag ? "Log Cabin" : "Plank Cabin");
            stringList.Add("Stone Cabin");
            stringList.Add(flag ? "Plank Cabin" : "Log Cabin");
            break;
        }
        for (int index = 0; index < this._startingCabinLocations.Count; ++index)
        {
          if (this is BuildableGameLocation)
          {
            this.clearArea((int) this._startingCabinLocations[index].X, (int) this._startingCabinLocations[index].Y, 5, 3);
            this.clearArea((int) this._startingCabinLocations[index].X + 2, (int) this._startingCabinLocations[index].Y + 3, 1, 1);
            this.setTileProperty((int) this._startingCabinLocations[index].X + 2, (int) this._startingCabinLocations[index].Y + 3, "Back", "NoSpawn", "All");
            Building b = new Building(new BluePrint(stringList[index])
            {
              magical = true
            }, this._startingCabinLocations[index]);
            b.daysOfConstructionLeft.Value = 0;
            b.load();
            (this as BuildableGameLocation).buildStructure(b, this._startingCabinLocations[index], Game1.player, true);
            b.removeOverlappingBushes(this);
          }
        }
      }
      this._startingCabinLocations.Clear();
    }

    public void updateDoors()
    {
      this.doors.Clear();
      Layer layer = this.map.GetLayer("Buildings");
      for (int x = 0; x < layer.LayerWidth; ++x)
      {
        for (int y = 0; y < layer.LayerHeight; ++y)
        {
          if (layer.Tiles[x, y] != null)
          {
            PropertyValue propertyValue = (PropertyValue) null;
            layer.Tiles[x, y].Properties.TryGetValue("Action", out propertyValue);
            if (propertyValue != null && propertyValue.ToString().Contains("Warp"))
            {
              string[] strArray = propertyValue.ToString().Split(' ');
              if (strArray[0].Equals("WarpBoatTunnel"))
                this.doors.Add(new Point(x, y), new NetString("BoatTunnel"));
              if (strArray[0].Equals("WarpCommunityCenter"))
                this.doors.Add(new Point(x, y), new NetString("CommunityCenter"));
              else if (strArray[0].Equals("Warp_Sunroom_Door"))
                this.doors.Add(new Point(x, y), new NetString("Sunroom"));
              else if ((!this.name.Equals((object) "Mountain") || x != 8 || y != 20) && strArray.Length > 3)
                this.doors.Add(new Point(x, y), new NetString(strArray[3]));
            }
          }
        }
      }
    }

    private void clearArea(int startingX, int startingY, int width, int height)
    {
      for (int x = startingX; x < startingX + width; ++x)
      {
        for (int y = startingY; y < startingY + height; ++y)
          this.removeEverythingExceptCharactersFromThisTile(x, y);
      }
    }

    public bool isTerrainFeatureAt(int x, int y)
    {
      Vector2 key = new Vector2((float) x, (float) y);
      if (this.terrainFeatures.ContainsKey(key) && !this.terrainFeatures[key].isPassable())
        return true;
      if (this.largeTerrainFeatures != null)
      {
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(x * 64, y * 64, 64, 64);
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
        {
          if (largeTerrainFeature.getBoundingBox().Intersects(rectangle))
            return true;
        }
      }
      return false;
    }

    public void loadLights()
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.isOutdoors && !Game1.isFestival() && !this.forceLoadPathLayerLights)
        return;
      switch (this)
      {
        case FarmHouse _:
          break;
        case IslandFarmHouse _:
          break;
        default:
          bool flag = false;
          foreach (Component layer in this.map.Layers)
          {
            if (layer.Id.Equals("Paths"))
            {
              flag = true;
              break;
            }
          }
          for (int x = 0; x < this.map.Layers[0].LayerWidth; ++x)
          {
            for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
            {
              if (!(bool) (NetFieldBase<bool, NetBool>) this.isOutdoors && !this.map.Properties.ContainsKey("IgnoreLightingTiles"))
              {
                Tile tile1 = this.map.GetLayer("Front").Tiles[x, y];
                if (tile1 != null)
                  this.adjustMapLightPropertiesForLamp(tile1.TileIndex, x, y, "Front");
                Tile tile2 = this.map.GetLayer("Buildings").Tiles[x, y];
                if (tile2 != null)
                  this.adjustMapLightPropertiesForLamp(tile2.TileIndex, x, y, "Buildings");
              }
              if (flag)
              {
                Tile tile = this.map.GetLayer("Paths").Tiles[x, y];
                if (tile != null)
                  this.adjustMapLightPropertiesForLamp(tile.TileIndex, x, y, "Paths");
              }
            }
          }
          break;
      }
    }

    public bool isFarmBuildingInterior() => this is AnimalHouse;

    public virtual bool CanBeRemotedlyViewed() => false;

    protected void adjustMapLightPropertiesForLamp(int tile, int x, int y, string layer)
    {
      string tileSheetIdAt = this.getTileSheetIDAt(x, y, layer);
      if (this.isFarmBuildingInterior())
      {
        if (!(tileSheetIdAt == "Coop") && !(tileSheetIdAt == "barn"))
          return;
        switch (tile)
        {
          case 24:
            this.changeMapProperties("DayTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + tile.ToString());
            string[] strArray = new string[7]
            {
              layer,
              " ",
              x.ToString(),
              " ",
              y.ToString(),
              " ",
              null
            };
            int num = 26;
            strArray[6] = num.ToString();
            this.changeMapProperties("NightTiles", string.Concat(strArray));
            string str1 = x.ToString();
            num = y + 1;
            string str2 = num.ToString();
            this.changeMapProperties("WindowLight", str1 + " " + str2 + " 4");
            string str3 = x.ToString();
            num = y + 3;
            string str4 = num.ToString();
            this.changeMapProperties("WindowLight", str3 + " " + str4 + " 4");
            break;
          case 25:
            this.changeMapProperties("DayTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + tile.ToString());
            this.changeMapProperties("NightTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + 12.ToString());
            break;
          case 46:
            this.changeMapProperties("DayTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + tile.ToString());
            this.changeMapProperties("NightTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + 53.ToString());
            break;
        }
      }
      else if (tile == 8 && layer == "Paths")
      {
        this.changeMapProperties("Light", x.ToString() + " " + y.ToString() + " 4");
      }
      else
      {
        if (!(tileSheetIdAt == "indoor"))
          return;
        switch (tile)
        {
          case 225:
            if (this.name.Contains("BathHouse") || this.name.Contains("Club") || this.name.Equals((object) "SeedShop") && (x == 36 || x == 37))
              break;
            this.changeMapProperties("DayTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + tile.ToString());
            string[] strArray1 = new string[7]
            {
              layer,
              " ",
              x.ToString(),
              " ",
              y.ToString(),
              " ",
              null
            };
            int num1 = 1222;
            strArray1[6] = num1.ToString();
            this.changeMapProperties("NightTiles", string.Concat(strArray1));
            string[] strArray2 = new string[7]
            {
              layer,
              " ",
              x.ToString(),
              " ",
              null,
              null,
              null
            };
            num1 = y + 1;
            strArray2[4] = num1.ToString();
            strArray2[5] = " ";
            num1 = 257;
            strArray2[6] = num1.ToString();
            this.changeMapProperties("DayTiles", string.Concat(strArray2));
            this.changeMapProperties("NightTiles", layer + " " + x.ToString() + " " + (y + 1).ToString() + " " + 1254.ToString());
            this.changeMapProperties("WindowLight", x.ToString() + " " + y.ToString() + " 4");
            this.changeMapProperties("WindowLight", x.ToString() + " " + (y + 1).ToString() + " 4");
            break;
          case 256:
            this.changeMapProperties("DayTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + tile.ToString());
            string[] strArray3 = new string[7]
            {
              layer,
              " ",
              x.ToString(),
              " ",
              y.ToString(),
              " ",
              null
            };
            int num2 = 1253;
            strArray3[6] = num2.ToString();
            this.changeMapProperties("NightTiles", string.Concat(strArray3));
            string[] strArray4 = new string[7]
            {
              layer,
              " ",
              x.ToString(),
              " ",
              null,
              null,
              null
            };
            num2 = y + 1;
            strArray4[4] = num2.ToString();
            strArray4[5] = " ";
            num2 = 288;
            strArray4[6] = num2.ToString();
            this.changeMapProperties("DayTiles", string.Concat(strArray4));
            this.changeMapProperties("NightTiles", layer + " " + x.ToString() + " " + (y + 1).ToString() + " " + 1285.ToString());
            this.changeMapProperties("WindowLight", x.ToString() + " " + y.ToString() + " 4");
            this.changeMapProperties("WindowLight", x.ToString() + " " + (y + 1).ToString() + " 4");
            break;
          case 480:
            this.changeMapProperties("DayTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + tile.ToString());
            this.changeMapProperties("NightTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + 809.ToString());
            this.changeMapProperties("Light", x.ToString() + " " + y.ToString() + " 4");
            break;
          case 826:
            this.changeMapProperties("DayTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + tile.ToString());
            this.changeMapProperties("NightTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + 827.ToString());
            this.changeMapProperties("Light", x.ToString() + " " + y.ToString() + " 4");
            break;
          case 1344:
            this.changeMapProperties("DayTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + tile.ToString());
            this.changeMapProperties("NightTiles", layer + " " + x.ToString() + " " + y.ToString() + " " + 1345.ToString());
            this.changeMapProperties("Light", x.ToString() + " " + y.ToString() + " 4");
            break;
          case 1346:
            this.changeMapProperties("DayTiles", "Front " + x.ToString() + " " + y.ToString() + " " + tile.ToString());
            string[] strArray5 = new string[6]
            {
              "Front ",
              x.ToString(),
              " ",
              y.ToString(),
              " ",
              null
            };
            int num3 = 1347;
            strArray5[5] = num3.ToString();
            this.changeMapProperties("NightTiles", string.Concat(strArray5));
            string[] strArray6 = new string[6]
            {
              "Buildings ",
              x.ToString(),
              " ",
              null,
              null,
              null
            };
            num3 = y + 1;
            strArray6[3] = num3.ToString();
            strArray6[4] = " ";
            num3 = 452;
            strArray6[5] = num3.ToString();
            this.changeMapProperties("DayTiles", string.Concat(strArray6));
            string[] strArray7 = new string[6]
            {
              "Buildings ",
              x.ToString(),
              " ",
              null,
              null,
              null
            };
            num3 = y + 1;
            strArray7[3] = num3.ToString();
            strArray7[4] = " ";
            num3 = 453;
            strArray7[5] = num3.ToString();
            this.changeMapProperties("NightTiles", string.Concat(strArray7));
            this.changeMapProperties("Light", x.ToString() + " " + y.ToString() + " 4");
            break;
        }
      }
    }

    private void changeMapProperties(string propertyName, string toAdd)
    {
      try
      {
        bool flag = true;
        if (!this.map.Properties.ContainsKey(propertyName))
        {
          this.map.Properties.Add(propertyName, new PropertyValue(string.Empty));
          flag = false;
        }
        if (this.map.Properties[propertyName].ToString().Contains(toAdd))
          return;
        StringBuilder stringBuilder = new StringBuilder(this.map.Properties[propertyName].ToString());
        if (flag)
          stringBuilder.Append(" ");
        stringBuilder.Append(toAdd);
        this.map.Properties[propertyName] = new PropertyValue(stringBuilder.ToString());
      }
      catch (Exception ex)
      {
      }
    }

    public override bool Equals(object obj) => obj is GameLocation && this.Equals(obj as GameLocation);

    public bool Equals(GameLocation other) => this.isStructure.Get() == other.isStructure.Get() && string.Equals(this.NameOrUniqueName, other.NameOrUniqueName, StringComparison.Ordinal);

    public enum LocationContext
    {
      Default,
      Island,
      MAX,
    }

    public delegate void afterQuestionBehavior(Farmer who, string whichAnswer);

    private struct DamagePlayersEventArg : NetEventArg
    {
      public Microsoft.Xna.Framework.Rectangle Area;
      public int Damage;

      public void Read(BinaryReader reader)
      {
        this.Area = reader.ReadRectangle();
        this.Damage = reader.ReadInt32();
      }

      public void Write(BinaryWriter writer)
      {
        writer.WriteRectangle(this.Area);
        writer.Write(this.Damage);
      }
    }
  }
}
