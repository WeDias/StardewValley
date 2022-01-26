// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.IWorldState
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;

namespace StardewValley.Network
{
  public interface IWorldState : INetObject<NetFields>
  {
    ServerPrivacy ServerPrivacy { get; set; }

    WorldDate Date { get; }

    bool IsTimePaused { get; set; }

    bool IsPaused { get; set; }

    bool IsGoblinRemoved { get; set; }

    bool IsSubmarineLocked { get; set; }

    int MinesDifficulty { get; set; }

    int SkullCavesDifficulty { get; set; }

    int LowestMineLevelForOrder { get; set; }

    int LowestMineLevel { get; set; }

    int WeatherForTomorrow { get; set; }

    Dictionary<string, string> BundleData { get; }

    NetBundles Bundles { get; }

    NetIntDictionary<bool, NetBool> BundleRewards { get; }

    NetVector2Dictionary<int, NetInt> MuseumPieces { get; }

    NetIntDelta LostBooksFound { get; }

    NetIntDelta GoldenWalnuts { get; }

    NetIntDelta GoldenWalnutsFound { get; }

    NetIntDelta MiniShippingBinsObtained { get; }

    NetBool GoldenCoconutCracked { get; }

    NetBool ParrotPlatformsUnlocked { get; }

    NetStringDictionary<bool, NetBool> FoundBuriedNuts { get; }

    NetStringDictionary<bool, NetBool> IslandVisitors { get; }

    NetIntDictionary<global::LocationWeather, NetRef<global::LocationWeather>> LocationWeather { get; }

    int VisitsUntilY1Guarantee { get; set; }

    Game1.MineChestType ShuffleMineChests { get; set; }

    NetInt HighestPlayerLimit { get; }

    NetInt CurrentPlayerLimit { get; }

    NetRef<Object> DishOfTheDay { get; }

    void RegisterSpecialCurrencies();

    global::LocationWeather GetWeatherForLocation(
      GameLocation.LocationContext location_context);

    Dictionary<string, string> GetUnlocalizedBundleData();

    void SetBundleData(Dictionary<string, string> data);

    bool hasWorldStateID(string id);

    void addWorldStateID(string id);

    void removeWorldStateID(string id);

    void UpdateFromGame1();

    void WriteToGame1();
  }
}
