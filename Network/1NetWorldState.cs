// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetWorldState
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Network
{
  public class NetWorldState : IWorldState, INetObject<NetFields>
  {
    private readonly NetEnum<ServerPrivacy> serverPrivacy = new NetEnum<ServerPrivacy>();
    private readonly NetInt year = new NetInt(1);
    private readonly NetString currentSeason = new NetString("spring");
    private readonly NetInt dayOfMonth = new NetInt(0);
    private readonly NetInt timeOfDay = new NetInt();
    private readonly NetInt whichFarm = new NetInt();
    private readonly NetString whichModFarm = new NetString();
    protected string _oldModFarmType;
    private readonly NetInt daysPlayed = new NetInt();
    private readonly NetIntDictionary<global::LocationWeather, NetRef<global::LocationWeather>> locationWeather = new NetIntDictionary<global::LocationWeather, NetRef<global::LocationWeather>>();
    public readonly NetInt visitsUntilY1Guarantee = new NetInt(-1);
    public readonly NetEnum<Game1.MineChestType> shuffleMineChests = new NetEnum<Game1.MineChestType>(Game1.MineChestType.Default);
    private readonly NetBool isRaining = new NetBool();
    private readonly NetBool isSnowing = new NetBool();
    private readonly NetBool isLightning = new NetBool();
    private readonly NetBool isDebrisWeather = new NetBool();
    private readonly NetBool isPaused = new NetBool();
    private readonly NetBool isTimePaused = new NetBool();
    public readonly NetBool parrotPlatformsUnlocked = new NetBool();
    public readonly NetBool goblinRemoved = new NetBool();
    public readonly NetBool submarineLocked = new NetBool();
    public readonly NetInt weatherForTomorrow = new NetInt();
    public readonly NetInt minesDifficulty = new NetInt();
    public readonly NetInt skullCavesDifficulty = new NetInt();
    public readonly NetInt lowestMineLevel = new NetInt();
    public readonly NetInt lowestMineLevelForOrder = new NetInt(-1);
    public readonly NetInt currentSongIndex = new NetInt();
    private readonly NetBundles bundles = new NetBundles();
    private readonly NetIntDictionary<bool, NetBool> bundleRewards = new NetIntDictionary<bool, NetBool>();
    private readonly NetVector2Dictionary<int, NetInt> museumPieces = new NetVector2Dictionary<int, NetInt>();
    private readonly NetIntDelta lostBooksFound = new NetIntDelta();
    private readonly NetIntDelta goldenWalnuts = new NetIntDelta();
    private readonly NetIntDelta goldenWalnutsFound = new NetIntDelta();
    private readonly NetBool goldenCoconutCracked = new NetBool();
    private readonly NetStringDictionary<bool, NetBool> foundBuriedNuts = new NetStringDictionary<bool, NetBool>();
    private readonly NetStringList worldStateIDs = new NetStringList();
    private readonly NetIntDelta miniShippingBinsObtained = new NetIntDelta();
    private readonly NetStringDictionary<bool, NetBool> islandVisitors = new NetStringDictionary<bool, NetBool>();
    private readonly NetLong uniqueIDForThisGame = new NetLong();
    private readonly NetStringDictionary<string, NetString> netBundleData = new NetStringDictionary<string, NetString>();
    private Dictionary<string, string> _bundleData;
    private bool _bundleDataDirty = true;
    public readonly NetInt highestPlayerLimit = new NetInt(-1);
    public readonly NetInt currentPlayerLimit = new NetInt(-1);
    public readonly NetRef<StardewValley.Object> dishOfTheDay = new NetRef<StardewValley.Object>();

    public NetFields NetFields { get; } = new NetFields();

    public NetWorldState()
    {
      if (Game1.specialCurrencyDisplay != null)
        this.RegisterSpecialCurrencies();
      this.miniShippingBinsObtained.Minimum = new int?(0);
      this.goldenWalnuts.Minimum = new int?(0);
      this.goldenWalnutsFound.Minimum = new int?(0);
      this.lostBooksFound.Minimum = new int?(0);
      this.lostBooksFound.Maximum = new int?(21);
      this.NetFields.AddFields((INetSerializable) this.year, (INetSerializable) this.currentSeason, (INetSerializable) this.dayOfMonth, (INetSerializable) this.timeOfDay, (INetSerializable) this.whichFarm, (INetSerializable) this.daysPlayed, (INetSerializable) this.weatherForTomorrow, (INetSerializable) this.isRaining, (INetSerializable) this.isSnowing, (INetSerializable) this.isLightning, (INetSerializable) this.isDebrisWeather, (INetSerializable) this.isPaused, (INetSerializable) this.goblinRemoved, (INetSerializable) this.submarineLocked, (INetSerializable) this.lowestMineLevel, (INetSerializable) this.bundles, (INetSerializable) this.bundleRewards, (INetSerializable) this.museumPieces, (INetSerializable) this.worldStateIDs, (INetSerializable) this.uniqueIDForThisGame, (INetSerializable) this.currentSongIndex, (INetSerializable) this.lostBooksFound, (INetSerializable) this.highestPlayerLimit, (INetSerializable) this.currentPlayerLimit, (INetSerializable) this.goldenWalnuts, (INetSerializable) this.goldenCoconutCracked, (INetSerializable) this.locationWeather, (INetSerializable) this.parrotPlatformsUnlocked, (INetSerializable) this.netBundleData, (INetSerializable) this.visitsUntilY1Guarantee, (INetSerializable) this.isTimePaused, (INetSerializable) this.dishOfTheDay, (INetSerializable) this.shuffleMineChests, (INetSerializable) this.miniShippingBinsObtained, (INetSerializable) this.foundBuriedNuts, (INetSerializable) this.goldenWalnutsFound, (INetSerializable) this.minesDifficulty, (INetSerializable) this.skullCavesDifficulty, (INetSerializable) this.lowestMineLevelForOrder, (INetSerializable) this.islandVisitors, (INetSerializable) this.whichModFarm, (INetSerializable) this.serverPrivacy);
      this.SetBundleData(Game1.content.LoadBase<Dictionary<string, string>>("Data\\Bundles"));
      this.isTimePaused.InterpolationWait = false;
      this.netBundleData.OnConflictResolve += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ConflictResolveEvent) ((key, rejected, accepted) => this._bundleDataDirty = true);
      this.netBundleData.OnValueAdded += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsChangeEvent) ((key, value) => this._bundleDataDirty = true);
      this.netBundleData.OnValueRemoved += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsChangeEvent) ((key, value) => this._bundleDataDirty = true);
      this.serverPrivacy.fieldChangeVisibleEvent += (NetFieldBase<ServerPrivacy, NetEnum<ServerPrivacy>>.FieldChange) ((field, old_value, new_value) => { });
    }

    public virtual void RegisterSpecialCurrencies()
    {
      if (Game1.specialCurrencyDisplay == null)
        return;
      Game1.specialCurrencyDisplay.Register("walnuts", this.goldenWalnuts);
      Game1.specialCurrencyDisplay.Register("qiGems", Game1.player.netQiGems);
    }

    public void SetBundleData(Dictionary<string, string> data)
    {
      this._bundleDataDirty = true;
      this.netBundleData.CopyFrom((IEnumerable<KeyValuePair<string, string>>) data);
      foreach (string key in this.netBundleData.Keys)
      {
        string str = this.netBundleData[key];
        int int32 = Convert.ToInt32(key.Split('/')[1]);
        int length = str.Split('/')[2].Split(' ').Length;
        if (!this.bundles.ContainsKey(int32))
          this.bundles.Add(int32, new NetArray<bool, NetBool>(length));
        else if (this.bundles[int32].Length < length)
        {
          NetArray<bool, NetBool> field = new NetArray<bool, NetBool>(length);
          for (int index = 0; index < Math.Min(this.bundles[int32].Length, length); ++index)
            field[index] = this.bundles[int32][index];
          this.bundles.Remove(int32);
          this.bundles.Add(int32, field);
        }
        if (!this.bundleRewards.ContainsKey(int32))
          this.bundleRewards.Add(int32, new NetBool(false));
      }
    }

    public static bool checkAnywhereForWorldStateID(string id) => Game1.worldStateIDs.Contains(id) || Game1.netWorldState.Value.hasWorldStateID(id);

    public static void addWorldStateIDEverywhere(string id)
    {
      Game1.netWorldState.Value.addWorldStateID(id);
      if (Game1.worldStateIDs.Contains(id))
        return;
      Game1.worldStateIDs.Add(id);
    }

    public WorldDate Date => new WorldDate((int) (NetFieldBase<int, NetInt>) this.year, (string) (NetFieldBase<string, NetString>) this.currentSeason, (int) (NetFieldBase<int, NetInt>) this.dayOfMonth);

    public ServerPrivacy ServerPrivacy
    {
      get => this.serverPrivacy.Value;
      set => this.serverPrivacy.Value = value;
    }

    public bool IsTimePaused
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isTimePaused;
      set => this.isTimePaused.Value = value;
    }

    public bool IsPaused
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isPaused;
      set => this.isPaused.Value = value;
    }

    public bool IsGoblinRemoved
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.goblinRemoved;
      set => this.goblinRemoved.Value = value;
    }

    public bool IsSubmarineLocked
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.submarineLocked;
      set => this.submarineLocked.Value = value;
    }

    public int SkullCavesDifficulty
    {
      get => (int) (NetFieldBase<int, NetInt>) this.skullCavesDifficulty;
      set => this.skullCavesDifficulty.Value = value;
    }

    public int MinesDifficulty
    {
      get => (int) (NetFieldBase<int, NetInt>) this.minesDifficulty;
      set => this.minesDifficulty.Value = value;
    }

    public int LowestMineLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.lowestMineLevel;
      set => this.lowestMineLevel.Value = value;
    }

    public int LowestMineLevelForOrder
    {
      get => (int) (NetFieldBase<int, NetInt>) this.lowestMineLevelForOrder;
      set => this.lowestMineLevelForOrder.Value = value;
    }

    public int WeatherForTomorrow
    {
      get => (int) (NetFieldBase<int, NetInt>) this.weatherForTomorrow;
      set => this.weatherForTomorrow.Value = value;
    }

    public Game1.MineChestType ShuffleMineChests
    {
      get => this.shuffleMineChests.Value;
      set => this.shuffleMineChests.Value = value;
    }

    public int VisitsUntilY1Guarantee
    {
      get => this.visitsUntilY1Guarantee.Value;
      set => this.visitsUntilY1Guarantee.Value = value;
    }

    public NetBundles Bundles => this.bundles;

    public NetIntDictionary<bool, NetBool> BundleRewards => this.bundleRewards;

    public NetVector2Dictionary<int, NetInt> MuseumPieces => this.museumPieces;

    public NetStringDictionary<bool, NetBool> FoundBuriedNuts => this.foundBuriedNuts;

    public NetStringDictionary<bool, NetBool> IslandVisitors => this.islandVisitors;

    public NetIntDictionary<global::LocationWeather, NetRef<global::LocationWeather>> LocationWeather => this.locationWeather;

    public NetIntDelta MiniShippingBinsObtained => this.miniShippingBinsObtained;

    public NetIntDelta GoldenWalnutsFound => this.goldenWalnutsFound;

    public NetIntDelta GoldenWalnuts => this.goldenWalnuts;

    public NetBool GoldenCoconutCracked => this.goldenCoconutCracked;

    public NetBool ParrotPlatformsUnlocked => this.parrotPlatformsUnlocked;

    public Dictionary<string, string> BundleData
    {
      get
      {
        if (this._bundleDataDirty)
        {
          this._bundleDataDirty = false;
          this._bundleData = new Dictionary<string, string>();
          foreach (string key in this.netBundleData.Keys)
            this._bundleData[key] = this.netBundleData[key];
          if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
            this.AddLocalizedBundleNames();
        }
        return this._bundleData;
      }
    }

    public Dictionary<string, string> GetUnlocalizedBundleData()
    {
      Dictionary<string, string> unlocalizedBundleData = new Dictionary<string, string>();
      foreach (string key in this.netBundleData.Keys)
        unlocalizedBundleData[key] = this.netBundleData[key];
      return unlocalizedBundleData;
    }

    public virtual void AddLocalizedBundleNames()
    {
      List<string> stringList = new List<string>((IEnumerable<string>) this._bundleData.Keys);
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Bundles");
      foreach (string key1 in stringList)
      {
        string str1 = this._bundleData[key1].Split('/')[0];
        string str2 = str1;
        bool flag = false;
        foreach (string key2 in dictionary.Keys)
        {
          string[] source = dictionary[key2].Split('/');
          if (source[0] == str1)
          {
            str2 = ((IEnumerable<string>) source).Last<string>();
            flag = true;
            break;
          }
        }
        if (!flag)
          str2 = Game1.content.LoadString("Strings\\BundleNames:" + str1);
        this._bundleData[key1] = this._bundleData[key1] + "/" + str2;
      }
    }

    public NetIntDelta LostBooksFound => this.lostBooksFound;

    public NetInt CurrentPlayerLimit => this.currentPlayerLimit;

    public NetInt HighestPlayerLimit => this.highestPlayerLimit;

    public NetRef<StardewValley.Object> DishOfTheDay => this.dishOfTheDay;

    public bool hasWorldStateID(string id) => this.worldStateIDs.Contains(id);

    public void addWorldStateID(string id)
    {
      if (this.hasWorldStateID(id))
        return;
      this.worldStateIDs.Add(id);
    }

    public void removeWorldStateID(string id) => this.worldStateIDs.Remove(id);

    public void UpdateFromGame1()
    {
      this.year.Value = Game1.year;
      this.currentSeason.Value = Game1.currentSeason;
      this.dayOfMonth.Value = Game1.dayOfMonth;
      this.timeOfDay.Value = Game1.timeOfDay;
      this.GetWeatherForLocation(GameLocation.LocationContext.Default).weatherForTomorrow.Value = Game1.weatherForTomorrow;
      this.GetWeatherForLocation(GameLocation.LocationContext.Default).isRaining.Value = Game1.isRaining;
      this.GetWeatherForLocation(GameLocation.LocationContext.Default).isSnowing.Value = Game1.isSnowing;
      this.GetWeatherForLocation(GameLocation.LocationContext.Default).isDebrisWeather.Value = Game1.isDebrisWeather;
      this.isDebrisWeather.Value = Game1.isDebrisWeather;
      this.whichFarm.Value = Game1.whichFarm;
      this.weatherForTomorrow.Value = Game1.weatherForTomorrow;
      this.daysPlayed.Value = (int) Game1.stats.daysPlayed;
      this.currentSongIndex.Value = Game1.currentSongIndex;
      this.uniqueIDForThisGame.Value = (long) Game1.uniqueIDForThisGame;
      if (Game1.whichFarm != 7 || Game1.whichModFarm == null)
        this.whichModFarm.Value = (string) null;
      else
        this.whichModFarm.Value = Game1.whichModFarm.ID;
      this.currentPlayerLimit.Value = Game1.multiplayer.playerLimit;
      this.highestPlayerLimit.Value = Math.Max(this.highestPlayerLimit.Value, Game1.multiplayer.playerLimit);
      this.worldStateIDs.CopyFrom((IList<string>) Game1.worldStateIDs);
    }

    public global::LocationWeather GetWeatherForLocation(int location_context)
    {
      if (!this.locationWeather.ContainsKey(location_context))
        this.locationWeather[location_context] = new global::LocationWeather();
      return this.locationWeather[location_context];
    }

    public global::LocationWeather GetWeatherForLocation(
      GameLocation.LocationContext location_context)
    {
      return this.GetWeatherForLocation((int) location_context);
    }

    public void WriteToGame1()
    {
      if (Game1.farmEvent != null)
        return;
      Game1.weatherForTomorrow = this.GetWeatherForLocation(GameLocation.LocationContext.Default).weatherForTomorrow.Value;
      Game1.isRaining = this.GetWeatherForLocation(GameLocation.LocationContext.Default).isRaining.Value;
      Game1.isSnowing = this.GetWeatherForLocation(GameLocation.LocationContext.Default).isSnowing.Value;
      Game1.isLightning = this.GetWeatherForLocation(GameLocation.LocationContext.Default).isLightning.Value;
      Game1.isDebrisWeather = this.GetWeatherForLocation(GameLocation.LocationContext.Default).isDebrisWeather.Value;
      Game1.weatherForTomorrow = this.weatherForTomorrow.Value;
      Game1.worldStateIDs = this.worldStateIDs.ToList<string>();
      if (!Game1.IsServer)
      {
        bool flag = Game1.currentSeason != this.currentSeason.Value;
        Game1.year = this.year.Value;
        Game1.currentSeason = this.currentSeason.Value;
        Game1.dayOfMonth = this.dayOfMonth.Value;
        Game1.timeOfDay = this.timeOfDay.Value;
        Game1.whichFarm = this.whichFarm.Value;
        if (Game1.whichFarm != 7)
          Game1.whichModFarm = (ModFarmType) null;
        else if (this._oldModFarmType != this.whichModFarm.Value)
        {
          this._oldModFarmType = this.whichModFarm.Value;
          Game1.whichModFarm = (ModFarmType) null;
          List<ModFarmType> modFarmTypeList = Game1.content.Load<List<ModFarmType>>("Data\\AdditionalFarms");
          if (modFarmTypeList != null)
          {
            foreach (ModFarmType modFarmType in modFarmTypeList)
            {
              if (modFarmType.ID == this.whichModFarm.Value)
              {
                Game1.whichModFarm = modFarmType;
                break;
              }
            }
          }
          if (Game1.whichModFarm == null)
            throw new Exception(this.whichModFarm.Value + " is not a valid farm type.");
        }
        Game1.stats.daysPlayed = (uint) this.daysPlayed.Value;
        Game1.uniqueIDForThisGame = (ulong) this.uniqueIDForThisGame.Value;
        if (flag)
          Game1.setGraphicsForSeason();
      }
      Game1.currentSongIndex = this.currentSongIndex.Value;
      Game1.updateWeatherIcon();
      if (!this.IsGoblinRemoved)
        return;
      Game1.player.removeQuest(27);
    }
  }
}
