// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveGame
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Ionic.Zlib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.GameData;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace StardewValley
{
  public class SaveGame
  {
    public static XmlSerializer serializer = new XmlSerializer(typeof (SaveGame), new Type[25]
    {
      typeof (Tool),
      typeof (GameLocation),
      typeof (Duggy),
      typeof (Bug),
      typeof (BigSlime),
      typeof (Ghost),
      typeof (Child),
      typeof (Pet),
      typeof (Dog),
      typeof (Cat),
      typeof (Horse),
      typeof (GreenSlime),
      typeof (LavaCrab),
      typeof (RockCrab),
      typeof (ShadowGuy),
      typeof (SquidKid),
      typeof (Grub),
      typeof (Fly),
      typeof (DustSpirit),
      typeof (Quest),
      typeof (MetalHead),
      typeof (ShadowGirl),
      typeof (Monster),
      typeof (JunimoHarvester),
      typeof (TerrainFeature)
    });
    public static XmlSerializer farmerSerializer = new XmlSerializer(typeof (Farmer), new Type[1]
    {
      typeof (Tool)
    });
    public static XmlSerializer locationSerializer = new XmlSerializer(typeof (GameLocation), new Type[24]
    {
      typeof (Tool),
      typeof (Duggy),
      typeof (Ghost),
      typeof (GreenSlime),
      typeof (LavaCrab),
      typeof (RockCrab),
      typeof (ShadowGuy),
      typeof (Child),
      typeof (Pet),
      typeof (Dog),
      typeof (Cat),
      typeof (Horse),
      typeof (SquidKid),
      typeof (Grub),
      typeof (Fly),
      typeof (DustSpirit),
      typeof (Bug),
      typeof (BigSlime),
      typeof (BreakableContainer),
      typeof (MetalHead),
      typeof (ShadowGirl),
      typeof (Monster),
      typeof (JunimoHarvester),
      typeof (TerrainFeature)
    });
    [InstancedStatic]
    public static bool IsProcessing;
    [InstancedStatic]
    public static bool CancelToTitle;
    public Farmer player;
    public List<GameLocation> locations;
    public string currentSeason;
    public string samBandName;
    public string elliottBookName;
    public List<string> mailbox;
    public List<string> broadcastedMail;
    public List<string> worldStateIDs;
    public int lostBooksFound = -1;
    public int goldenWalnuts = -1;
    public int goldenWalnutsFound;
    public int miniShippingBinsObtained;
    public bool mineShrineActivated;
    public bool goldenCoconutCracked;
    public bool parrotPlatformsUnlocked;
    public bool farmPerfect;
    public List<string> foundBuriedNuts = new List<string>();
    public int visitsUntilY1Guarantee = -1;
    public Game1.MineChestType shuffleMineChests;
    public int dayOfMonth;
    public int year;
    public int farmerWallpaper;
    public int FarmerFloor;
    public int currentWallpaper;
    public int currentFloor;
    public int currentSongIndex;
    public int? countdownToWedding;
    public Point incubatingEgg;
    public double chanceToRainTomorrow;
    public double dailyLuck;
    public ulong uniqueIDForThisGame;
    public bool weddingToday;
    public bool isRaining;
    public bool isDebrisWeather;
    public bool shippingTax;
    public bool isLightning;
    public bool isSnowing;
    public bool shouldSpawnMonsters;
    public bool hasApplied1_3_UpdateChanges;
    public bool hasApplied1_4_UpdateChanges;
    public Stats stats;
    [InstancedStatic]
    public static SaveGame loaded;
    public float musicVolume;
    public float soundVolume;
    public int[] cropsOfTheWeek;
    public Object dishOfTheDay;
    public int highestPlayerLimit = -1;
    public int moveBuildingPermissionMode;
    public SerializableDictionary<GameLocation.LocationContext, LocationWeather> locationWeather;
    public SerializableDictionary<string, string> bannedUsers = new SerializableDictionary<string, string>();
    public SerializableDictionary<string, string> bundleData = new SerializableDictionary<string, string>();
    public SerializableDictionary<string, int> limitedNutDrops = new SerializableDictionary<string, int>();
    public long latestID;
    public Options options;
    public SerializableDictionary<long, Options> splitscreenOptions = new SerializableDictionary<long, Options>();
    public SerializableDictionary<string, string> CustomData = new SerializableDictionary<string, string>();
    public SerializableDictionary<int, MineInfo> mine_permanentMineChanges;
    [Obsolete]
    [XmlIgnore]
    public List<ResourceClump> mine_resourceClumps = new List<ResourceClump>();
    [Obsolete]
    [XmlIgnore]
    public int mine_mineLevel;
    [Obsolete]
    [XmlIgnore]
    public int mine_nextLevel;
    public int mine_lowestLevelReached;
    public int minecartHighScore;
    public int weatherForTomorrow;
    public string whichFarm;
    public int mine_lowestLevelReachedForOrder = -1;
    public int skullCavesDifficulty;
    public int minesDifficulty;
    public int currentGemBirdIndex;
    public NetLeaderboards junimoKartLeaderboards;
    public List<SpecialOrder> specialOrders;
    public List<SpecialOrder> availableSpecialOrders;
    public List<string> completedSpecialOrders;
    public List<string> acceptedSpecialOrderTypes = new List<string>();
    public List<Item> returnedDonations;
    public List<Item> junimoChest;
    public List<string> collectedNutTracker = new List<string>();
    public SerializableDictionary<FarmerPair, Friendship> farmerFriendships = new SerializableDictionary<FarmerPair, Friendship>();
    public SerializableDictionary<int, long> cellarAssignments = new SerializableDictionary<int, long>();
    public int lastAppliedSaveFix;
    public string gameVersion = Game1.version;
    public string gameVersionLabel;

    public static XmlSerializer GetSerializer(Type type) => new XmlSerializer(type);

    public static IEnumerator<int> Save()
    {
      SaveGame.IsProcessing = true;
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        IEnumerator<int> save = SaveGame.getSaveEnumerator();
        while (save.MoveNext())
          yield return save.Current;
        yield return 100;
        save = (IEnumerator<int>) null;
      }
      else
      {
        Console.WriteLine("SaveGame.Save() called.");
        yield return 1;
        IEnumerator<int> loader = SaveGame.getSaveEnumerator();
        Task saveTask = new Task((Action) (() =>
        {
          Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
          if (loader == null)
            return;
          do
            ;
          while (loader.MoveNext() && loader.Current < 100);
        }));
        Game1.hooks.StartTask(saveTask, nameof (Save));
        while (!saveTask.IsCanceled && !saveTask.IsCompleted && !saveTask.IsFaulted)
          yield return 1;
        SaveGame.IsProcessing = false;
        if (saveTask.IsFaulted)
        {
          Exception baseException = saveTask.Exception.GetBaseException();
          Console.WriteLine("saveTask failed with an exception");
          Console.WriteLine((object) baseException);
          if (!(baseException is TaskCanceledException))
            throw baseException;
          Game1.ExitToTitle();
        }
        else
        {
          Console.WriteLine("SaveGame.Save() completed without exceptions.");
          yield return 100;
          saveTask = (Task) null;
        }
      }
    }

    public static string FilterFileName(string fileName)
    {
      foreach (char c in fileName)
      {
        if (!char.IsLetterOrDigit(c))
          fileName = fileName.Replace(c.ToString() ?? "", "");
      }
      return fileName;
    }

    public static IEnumerator<int> getSaveEnumerator()
    {
      if (SaveGame.CancelToTitle)
        throw new TaskCanceledException();
      yield return 1;
      SaveGame saveData = new SaveGame()
      {
        player = Game1.player
      };
      saveData.player.gameVersion = Game1.version;
      saveData.player.gameVersionLabel = Game1.versionLabel;
      saveData.locations = new List<GameLocation>();
      saveData.locations.AddRange((IEnumerable<GameLocation>) Game1.locations);
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
        location.cleanupBeforeSave();
      saveData.currentSeason = Game1.currentSeason;
      saveData.samBandName = Game1.samBandName;
      saveData.broadcastedMail = new List<string>();
      saveData.bannedUsers = Game1.bannedUsers;
      foreach (string str in (NetList<string, NetString>) Game1.player.team.broadcastedMail)
        saveData.broadcastedMail.Add(str);
      saveData.skullCavesDifficulty = Game1.netWorldState.Value.SkullCavesDifficulty;
      saveData.minesDifficulty = Game1.netWorldState.Value.MinesDifficulty;
      saveData.visitsUntilY1Guarantee = Game1.netWorldState.Value.VisitsUntilY1Guarantee;
      saveData.shuffleMineChests = Game1.netWorldState.Value.ShuffleMineChests;
      saveData.elliottBookName = Game1.elliottBookName;
      saveData.dayOfMonth = Game1.dayOfMonth;
      saveData.year = Game1.year;
      saveData.farmerWallpaper = Game1.farmerWallpaper;
      saveData.FarmerFloor = Game1.FarmerFloor;
      saveData.chanceToRainTomorrow = Game1.chanceToRainTomorrow;
      saveData.dailyLuck = Game1.player.team.sharedDailyLuck.Value;
      saveData.isRaining = Game1.isRaining;
      saveData.isLightning = Game1.isLightning;
      saveData.isSnowing = Game1.isSnowing;
      saveData.isDebrisWeather = Game1.isDebrisWeather;
      saveData.shouldSpawnMonsters = Game1.spawnMonstersAtNight;
      saveData.specialOrders = Game1.player.team.specialOrders.ToList<SpecialOrder>();
      saveData.availableSpecialOrders = Game1.player.team.availableSpecialOrders.ToList<SpecialOrder>();
      saveData.completedSpecialOrders = Game1.player.team.completedSpecialOrders.Keys.ToList<string>();
      saveData.collectedNutTracker = new List<string>((IEnumerable<string>) Game1.player.team.collectedNutTracker.Keys);
      saveData.acceptedSpecialOrderTypes = Game1.player.team.acceptedSpecialOrderTypes.ToList<string>();
      saveData.returnedDonations = Game1.player.team.returnedDonations.ToList<Item>();
      saveData.junimoChest = Game1.player.team.junimoChest.ToList<Item>();
      saveData.weddingToday = Game1.weddingToday;
      saveData.whichFarm = Game1.whichFarm != 7 ? Game1.whichFarm.ToString() : Game1.whichModFarm.ID;
      saveData.minecartHighScore = Game1.minecartHighScore;
      saveData.junimoKartLeaderboards = Game1.player.team.junimoKartScores;
      saveData.lastAppliedSaveFix = (int) Game1.lastAppliedSaveFix;
      saveData.locationWeather = new SerializableDictionary<GameLocation.LocationContext, LocationWeather>();
      foreach (int key in Game1.netWorldState.Value.LocationWeather.Keys)
      {
        LocationWeather locationWeather = Game1.netWorldState.Value.LocationWeather[key];
        saveData.locationWeather[(GameLocation.LocationContext) key] = locationWeather;
      }
      saveData.cellarAssignments = new SerializableDictionary<int, long>();
      foreach (int key in Game1.player.team.cellarAssignments.Keys)
        saveData.cellarAssignments[key] = Game1.player.team.cellarAssignments[key];
      saveData.uniqueIDForThisGame = Game1.uniqueIDForThisGame;
      saveData.musicVolume = Game1.options.musicVolumeLevel;
      saveData.soundVolume = Game1.options.soundVolumeLevel;
      saveData.shippingTax = Game1.shippingTax;
      saveData.cropsOfTheWeek = Game1.cropsOfTheWeek;
      saveData.mine_lowestLevelReached = Game1.netWorldState.Value.LowestMineLevel;
      saveData.mine_lowestLevelReachedForOrder = Game1.netWorldState.Value.LowestMineLevelForOrder;
      saveData.currentGemBirdIndex = Game1.currentGemBirdIndex;
      saveData.mine_permanentMineChanges = MineShaft.permanentMineChanges;
      saveData.currentFloor = Game1.currentFloor;
      saveData.currentWallpaper = Game1.currentWallpaper;
      saveData.dishOfTheDay = Game1.dishOfTheDay;
      saveData.latestID = (long) Game1.multiplayer.latestID;
      saveData.highestPlayerLimit = (int) (NetFieldBase<int, NetInt>) Game1.netWorldState.Value.HighestPlayerLimit;
      saveData.options = Game1.options;
      saveData.splitscreenOptions = Game1.splitscreenOptions;
      saveData.CustomData = Game1.CustomData;
      saveData.worldStateIDs = Game1.worldStateIDs;
      saveData.currentSongIndex = Game1.currentSongIndex;
      saveData.weatherForTomorrow = Game1.weatherForTomorrow;
      saveData.goldenWalnuts = (int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.GoldenWalnuts;
      saveData.goldenWalnutsFound = (int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.GoldenWalnutsFound;
      saveData.miniShippingBinsObtained = (int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.MiniShippingBinsObtained;
      saveData.goldenCoconutCracked = Game1.netWorldState.Value.GoldenCoconutCracked.Value;
      saveData.parrotPlatformsUnlocked = Game1.netWorldState.Value.ParrotPlatformsUnlocked.Value;
      saveData.farmPerfect = Game1.player.team.farmPerfect.Value;
      saveData.lostBooksFound = (int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.LostBooksFound;
      saveData.foundBuriedNuts = new List<string>((IEnumerable<string>) Game1.netWorldState.Value.FoundBuriedNuts.Keys);
      saveData.mineShrineActivated = (bool) (NetFieldBase<bool, NetBool>) Game1.player.team.mineShrineActivated;
      saveData.gameVersion = Game1.version;
      saveData.gameVersionLabel = Game1.versionLabel;
      saveData.limitedNutDrops = new SerializableDictionary<string, int>();
      foreach (string key in Game1.player.team.limitedNutDrops.Keys)
        saveData.limitedNutDrops[key] = Game1.player.team.limitedNutDrops[key];
      saveData.bundleData = new SerializableDictionary<string, string>();
      Dictionary<string, string> unlocalizedBundleData = Game1.netWorldState.Value.GetUnlocalizedBundleData();
      foreach (string key in unlocalizedBundleData.Keys)
        saveData.bundleData[key] = unlocalizedBundleData[key];
      saveData.moveBuildingPermissionMode = (int) Game1.player.team.farmhandsCanMoveBuildings.Value;
      saveData.hasApplied1_3_UpdateChanges = Game1.hasApplied1_3_UpdateChanges;
      saveData.hasApplied1_4_UpdateChanges = Game1.hasApplied1_4_UpdateChanges;
      foreach (FarmerPair key in Game1.player.team.friendshipData.Keys)
        saveData.farmerFriendships[key] = Game1.player.team.friendshipData[key];
      string tmpString = "_STARDEWVALLEYSAVETMP";
      bool save_backups_and_metadata = true;
      string str1 = SaveGame.FilterFileName(Game1.GetSaveGameName());
      string filenameNoTmpString = str1 + "_" + Game1.uniqueIDForThisGame.ToString();
      string filenameWithTmpString = str1 + "_" + Game1.uniqueIDForThisGame.ToString() + tmpString;
      string save_directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves", filenameNoTmpString + Path.DirectorySeparatorChar.ToString());
      if (Game1.savePathOverride != "")
      {
        save_directory = Game1.savePathOverride;
        if (Game1.savePathOverride != "")
          save_backups_and_metadata = false;
      }
      string path = Path.Combine(save_directory, filenameWithTmpString);
      SaveGame.ensureFolderStructureExists();
      string justFarmerFilePath = Path.Combine(save_directory, "SaveGameInfo" + tmpString);
      if (File.Exists(path))
        File.Delete(path);
      if (save_backups_and_metadata && File.Exists(justFarmerFilePath))
        File.Delete(justFarmerFilePath);
      Stream fstream = (Stream) null;
      try
      {
        fstream = (Stream) File.Create(path);
      }
      catch (IOException ex)
      {
        if (fstream != null)
        {
          fstream.Close();
          fstream.Dispose();
        }
        Game1.gameMode = (byte) 9;
        Game1.debugOutput = Game1.parseText(ex.Message);
        yield break;
      }
      MemoryStream mstream1 = new MemoryStream(1024);
      MemoryStream mstream2 = new MemoryStream(1024);
      byte[] buffer1 = (byte[]) null;
      if (SaveGame.CancelToTitle)
        throw new TaskCanceledException();
      yield return 2;
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.CloseOutput = false;
      Console.WriteLine("Saving without compression...");
      MemoryStream output = mstream1;
      XmlWriter xmlWriter1 = XmlWriter.Create((Stream) output, settings);
      xmlWriter1.WriteStartDocument();
      SaveGame.serializer.Serialize(xmlWriter1, (object) saveData);
      xmlWriter1.WriteEndDocument();
      xmlWriter1.Flush();
      xmlWriter1.Close();
      output.Close();
      buffer1 = mstream1.ToArray();
      mstream1 = (MemoryStream) null;
      if (SaveGame.CancelToTitle)
        throw new TaskCanceledException();
      yield return 2;
      fstream.Write(buffer1, 0, buffer1.Length);
      fstream.Close();
      buffer1 = (byte[]) null;
      mstream1 = (MemoryStream) null;
      if (save_backups_and_metadata)
      {
        Game1.player.saveTime = (int) (DateTime.UtcNow - new DateTime(2012, 6, 22)).TotalMinutes;
        try
        {
          fstream = (Stream) File.Create(justFarmerFilePath);
        }
        catch (IOException ex)
        {
          fstream?.Close();
          Game1.gameMode = (byte) 9;
          Game1.debugOutput = Game1.parseText(ex.Message);
          yield break;
        }
        XmlWriter xmlWriter2 = XmlWriter.Create(fstream, new XmlWriterSettings()
        {
          CloseOutput = false
        });
        xmlWriter2.WriteStartDocument();
        SaveGame.farmerSerializer.Serialize(xmlWriter2, (object) Game1.player);
        xmlWriter2.WriteEndDocument();
        xmlWriter2.Flush();
        fstream.Close();
      }
      if (SaveGame.CancelToTitle)
        throw new TaskCanceledException();
      yield return 2;
      string str2 = Path.Combine(save_directory, filenameNoTmpString);
      justFarmerFilePath = Path.Combine(save_directory, "SaveGameInfo");
      if (save_backups_and_metadata)
      {
        string str3 = Path.Combine(save_directory, filenameNoTmpString + "_old");
        string str4 = Path.Combine(save_directory, "SaveGameInfo_old");
        if (File.Exists(str3))
          File.Delete(str3);
        if (File.Exists(str4))
          File.Delete(str4);
        try
        {
          File.Move(str2, str3);
          File.Move(justFarmerFilePath, str4);
        }
        catch (Exception ex)
        {
        }
      }
      if (File.Exists(str2))
        File.Delete(str2);
      if (save_backups_and_metadata && File.Exists(justFarmerFilePath))
        File.Delete(justFarmerFilePath);
      string str5 = Path.Combine(save_directory, filenameWithTmpString);
      if (File.Exists(str5))
        File.Move(str5, str5.Replace(tmpString, ""));
      if (save_backups_and_metadata)
      {
        justFarmerFilePath = Path.Combine(save_directory, "SaveGameInfo" + tmpString);
        if (File.Exists(justFarmerFilePath))
          File.Move(justFarmerFilePath, justFarmerFilePath.Replace(tmpString, ""));
      }
      if (SaveGame.CancelToTitle)
        throw new TaskCanceledException();
      yield return 100;
    }

    public static bool IsNewGameSaveNameCollision(string save_name)
    {
      string str = save_name;
      foreach (char c in str)
      {
        if (!char.IsLetterOrDigit(c))
          str = str.Replace(c.ToString() ?? "", "");
      }
      if (!str.EndsWith(Path.DirectorySeparatorChar.ToString()))
        str += Path.DirectorySeparatorChar.ToString();
      string path2 = str + "_" + Game1.uniqueIDForThisGame.ToString();
      return new FileInfo(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), "Saves"), path2)).Directory.Exists;
    }

    public static void ensureFolderStructureExists(string tmpString = "")
    {
      string str = Game1.GetSaveGameName();
      foreach (char c in str)
      {
        if (!char.IsLetterOrDigit(c))
          str = str.Replace(c.ToString() ?? "", "");
      }
      string path2 = str + "_" + Game1.uniqueIDForThisGame.ToString() + tmpString;
      FileInfo fileInfo1 = new FileInfo(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), "Saves"), path2));
      if (!fileInfo1.Directory.Exists)
        fileInfo1.Directory.Create();
      FileInfo fileInfo2 = new FileInfo(Path.Combine(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), "Saves"), path2), "dummy"));
      if (!fileInfo2.Directory.Exists)
        fileInfo2.Directory.Create();
    }

    public static void Load(string filename)
    {
      Game1.gameMode = (byte) 6;
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4690");
      Game1.currentLoader = SaveGame.getLoadEnumerator(filename);
    }

    public static void LoadFarmType()
    {
      List<ModFarmType> modFarmTypeList = Game1.content.Load<List<ModFarmType>>("Data\\AdditionalFarms");
      Game1.whichFarm = -1;
      if (modFarmTypeList != null)
      {
        foreach (ModFarmType modFarmType in modFarmTypeList)
        {
          if (modFarmType.ID == SaveGame.loaded.whichFarm)
          {
            Game1.whichModFarm = modFarmType;
            Game1.whichFarm = 7;
            break;
          }
        }
      }
      if (SaveGame.loaded.whichFarm == null)
        Game1.whichFarm = 0;
      if (Game1.whichFarm >= 0)
        return;
      int result = 0;
      if (int.TryParse(SaveGame.loaded.whichFarm, out result))
      {
        Game1.whichFarm = result;
      }
      else
      {
        Game1.whichFarm = -1;
        throw new Exception(SaveGame.loaded.whichFarm + " is not a valid farm type.");
      }
    }

    public static IEnumerator<int> getLoadEnumerator(string file)
    {
      Game1.SetSaveName(((IEnumerable<string>) Path.GetFileNameWithoutExtension(file).Split('_')).FirstOrDefault<string>());
      Console.WriteLine("getLoadEnumerator('{0}')", (object) file);
      Stopwatch stopwatch = Stopwatch.StartNew();
      Game1.loadingMessage = "Accessing save...";
      SaveGame saveData = new SaveGame();
      SaveGame.IsProcessing = true;
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      yield return 1;
      Stream stream = (Stream) null;
      string fullFilePath = file;
      Game1.savePathOverride = Path.GetDirectoryName(file);
      if (Game1.savePathOverride == "")
        fullFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves", file, file);
      if (!File.Exists(fullFilePath))
      {
        fullFilePath += ".xml";
        if (!File.Exists(fullFilePath))
        {
          Game1.gameMode = (byte) 9;
          Game1.debugOutput = "File does not exist (-_-)";
          yield break;
        }
      }
      yield return 5;
      try
      {
        stream = (Stream) new MemoryStream(File.ReadAllBytes(fullFilePath), false);
      }
      catch (IOException ex)
      {
        Game1.gameMode = (byte) 9;
        Game1.debugOutput = Game1.parseText(ex.Message);
        if (stream == null)
        {
          yield break;
        }
        else
        {
          stream.Close();
          yield break;
        }
      }
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4696");
      yield return 7;
      int num = (int) (byte) stream.ReadByte();
      --stream.Position;
      if (num == 120)
      {
        Console.WriteLine("zlib stream detected...");
        stream = (Stream) new ZlibStream(stream, CompressionMode.Decompress);
      }
      else
        Console.WriteLine("regular stream detected...");
      Task deserializeTask;
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        SaveGame.loaded = (SaveGame) SaveGame.serializer.Deserialize(stream);
      }
      else
      {
        SaveGame pendingSaveGame = (SaveGame) null;
        deserializeTask = new Task((Action) (() =>
        {
          Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
          pendingSaveGame = (SaveGame) SaveGame.serializer.Deserialize(stream);
        }));
        Game1.hooks.StartTask(deserializeTask, "Load_Deserialize");
        while (!deserializeTask.IsCanceled && !deserializeTask.IsCompleted && !deserializeTask.IsFaulted)
          yield return 20;
        if (deserializeTask.IsFaulted)
        {
          Exception baseException = deserializeTask.Exception.GetBaseException();
          Console.WriteLine("deserializeTask failed with an exception");
          Console.WriteLine((object) baseException);
          throw baseException;
        }
        SaveGame.loaded = pendingSaveGame;
        deserializeTask = (Task) null;
      }
      stream.Dispose();
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4697");
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      yield return 20;
      SaveGame.LoadFarmType();
      Game1.year = SaveGame.loaded.year;
      Game1.netWorldState.Value.CurrentPlayerLimit.Set(Game1.multiplayer.playerLimit);
      if (SaveGame.loaded.highestPlayerLimit >= 0)
        Game1.netWorldState.Value.HighestPlayerLimit.Set(SaveGame.loaded.highestPlayerLimit);
      else
        Game1.netWorldState.Value.HighestPlayerLimit.Set(Math.Max(Game1.netWorldState.Value.HighestPlayerLimit.Value, Game1.multiplayer.MaxPlayers));
      Game1.uniqueIDForThisGame = SaveGame.loaded.uniqueIDForThisGame;
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        Game1.loadForNewGame(true);
      }
      else
      {
        deserializeTask = new Task((Action) (() =>
        {
          Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
          Game1.loadForNewGame(true);
        }));
        Game1.hooks.StartTask(deserializeTask, "Load_LoadForNewGame");
        while (!deserializeTask.IsCanceled && !deserializeTask.IsCompleted && !deserializeTask.IsFaulted)
          yield return 24;
        if (deserializeTask.IsFaulted)
        {
          Exception baseException = deserializeTask.Exception.GetBaseException();
          Console.WriteLine("loadNewGameTask failed with an exception");
          Console.WriteLine((object) baseException);
          throw baseException;
        }
        if (SaveGame.CancelToTitle)
          Game1.ExitToTitle();
        yield return 25;
        deserializeTask = (Task) null;
      }
      Game1.weatherForTomorrow = SaveGame.loaded.weatherForTomorrow;
      Game1.dayOfMonth = SaveGame.loaded.dayOfMonth;
      Game1.year = SaveGame.loaded.year;
      Game1.currentSeason = SaveGame.loaded.currentSeason;
      Game1.worldStateIDs = SaveGame.loaded.worldStateIDs;
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4698");
      if (SaveGame.loaded.mine_permanentMineChanges != null)
      {
        MineShaft.permanentMineChanges = SaveGame.loaded.mine_permanentMineChanges;
        Game1.netWorldState.Value.LowestMineLevel = SaveGame.loaded.mine_lowestLevelReached;
        Game1.netWorldState.Value.LowestMineLevelForOrder = SaveGame.loaded.mine_lowestLevelReachedForOrder;
      }
      Game1.currentGemBirdIndex = SaveGame.loaded.currentGemBirdIndex;
      if (SaveGame.loaded.bundleData.Count > 0)
      {
        Game1.netWorldState.Value.SetBundleData((Dictionary<string, string>) SaveGame.loaded.bundleData);
        foreach (string key in Game1.netWorldState.Value.BundleData.Keys)
          saveData.bundleData[key] = Game1.netWorldState.Value.BundleData[key];
      }
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      yield return 26;
      Game1.isRaining = SaveGame.loaded.isRaining;
      Game1.isLightning = SaveGame.loaded.isLightning;
      Game1.isSnowing = SaveGame.loaded.isSnowing;
      Game1.lastAppliedSaveFix = (SaveGame.SaveFixes) SaveGame.loaded.lastAppliedSaveFix;
      if (Game1.IsMasterGame)
        Game1.netWorldState.Value.UpdateFromGame1();
      if (SaveGame.loaded.locationWeather != null)
      {
        foreach (GameLocation.LocationContext key in SaveGame.loaded.locationWeather.Keys)
          Game1.netWorldState.Value.GetWeatherForLocation(key).CopyFrom(SaveGame.loaded.locationWeather[key]);
      }
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        SaveGame.loadDataToFarmer(SaveGame.loaded.player);
      }
      else
      {
        deserializeTask = new Task((Action) (() =>
        {
          Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
          SaveGame.loadDataToFarmer(SaveGame.loaded.player);
        }));
        Game1.hooks.StartTask(deserializeTask, "Load_Farmer");
        while (!deserializeTask.IsCanceled && !deserializeTask.IsCompleted && !deserializeTask.IsFaulted)
          yield return 1;
        if (deserializeTask.IsFaulted)
        {
          Exception baseException = deserializeTask.Exception.GetBaseException();
          Console.WriteLine("loadFarmerTask failed with an exception");
          Console.WriteLine((object) baseException);
          throw baseException;
        }
        deserializeTask = (Task) null;
      }
      Game1.player = SaveGame.loaded.player;
      if (Game1.MasterPlayer.hasOrWillReceiveMail("leoMoved") && Game1.getLocationFromName("Mountain") is Mountain locationFromName)
      {
        locationFromName.reloadMap();
        locationFromName.ApplyTreehouseIfNecessary();
        if (locationFromName.treehouseDoorDirty)
        {
          locationFromName.treehouseDoorDirty = false;
          NPC.populateRoutesFromLocationToLocationList();
        }
      }
      Game1.addParrotBoyIfNecessary();
      foreach (FarmerPair key in SaveGame.loaded.farmerFriendships.Keys)
        Game1.player.team.friendshipData[key] = SaveGame.loaded.farmerFriendships[key];
      Game1.spawnMonstersAtNight = SaveGame.loaded.shouldSpawnMonsters;
      Game1.player.team.limitedNutDrops.Clear();
      if ((NetFieldBase<IWorldState, NetRef<IWorldState>>) Game1.netWorldState != (NetRef<IWorldState>) null && Game1.netWorldState.Value != null)
        Game1.netWorldState.Value.RegisterSpecialCurrencies();
      if (SaveGame.loaded.limitedNutDrops != null)
      {
        foreach (string key in SaveGame.loaded.limitedNutDrops.Keys)
        {
          if (SaveGame.loaded.limitedNutDrops[key] > 0)
            Game1.player.team.limitedNutDrops[key] = SaveGame.loaded.limitedNutDrops[key];
        }
      }
      Game1.player.team.completedSpecialOrders.Clear();
      foreach (string completedSpecialOrder in SaveGame.loaded.completedSpecialOrders)
        Game1.player.team.completedSpecialOrders[completedSpecialOrder] = true;
      Game1.player.team.specialOrders.Clear();
      foreach (SpecialOrder specialOrder in SaveGame.loaded.specialOrders)
        Game1.player.team.specialOrders.Add(specialOrder);
      Game1.player.team.availableSpecialOrders.Clear();
      foreach (SpecialOrder availableSpecialOrder in SaveGame.loaded.availableSpecialOrders)
        Game1.player.team.availableSpecialOrders.Add(availableSpecialOrder);
      Game1.player.team.acceptedSpecialOrderTypes.Clear();
      Game1.player.team.acceptedSpecialOrderTypes.AddRange((IEnumerable<string>) SaveGame.loaded.acceptedSpecialOrderTypes);
      Game1.player.team.collectedNutTracker.Clear();
      foreach (string key in SaveGame.loaded.collectedNutTracker)
        Game1.player.team.collectedNutTracker[key] = true;
      Game1.player.team.junimoChest.Clear();
      foreach (Item obj in SaveGame.loaded.junimoChest)
        Game1.player.team.junimoChest.Add(obj);
      Game1.player.team.returnedDonations.Clear();
      foreach (Item returnedDonation in SaveGame.loaded.returnedDonations)
        Game1.player.team.returnedDonations.Add(returnedDonation);
      if (SaveGame.loaded.stats != null)
        Game1.player.stats = SaveGame.loaded.stats;
      if (SaveGame.loaded.mailbox != null && !Game1.player.mailbox.Any())
      {
        Game1.player.mailbox.Clear();
        Game1.player.mailbox.AddRange((IEnumerable<string>) SaveGame.loaded.mailbox);
      }
      Game1.random = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + 1);
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4699");
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      yield return 36;
      if (SaveGame.loaded.cellarAssignments != null)
      {
        foreach (int key in SaveGame.loaded.cellarAssignments.Keys)
          Game1.player.team.cellarAssignments[key] = SaveGame.loaded.cellarAssignments[key];
      }
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        SaveGame.loadDataToLocations(SaveGame.loaded.locations);
      }
      else
      {
        deserializeTask = new Task((Action) (() =>
        {
          Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
          SaveGame.loadDataToLocations(SaveGame.loaded.locations);
        }));
        Game1.hooks.StartTask(deserializeTask, "Load_Locations");
        while (!deserializeTask.IsCanceled && !deserializeTask.IsCompleted && !deserializeTask.IsFaulted)
          yield return 1;
        if (deserializeTask.IsFaulted)
        {
          Exception baseException = deserializeTask.Exception.GetBaseException();
          Console.WriteLine("loadLocationsTask failed with an exception");
          Console.WriteLine((object) baseException);
          throw deserializeTask.Exception.GetBaseException();
        }
        deserializeTask = (Task) null;
      }
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        int money = allFarmer.Money;
        if (!Game1.player.team.individualMoney.ContainsKey((long) allFarmer.uniqueMultiplayerID))
          Game1.player.team.individualMoney.Add((long) allFarmer.uniqueMultiplayerID, new NetIntDelta(money));
        Game1.player.team.individualMoney[(long) allFarmer.uniqueMultiplayerID].Value = money;
      }
      Game1.updateCellarAssignments();
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        if (location is BuildableGameLocation)
        {
          foreach (Building building in (location as BuildableGameLocation).buildings)
          {
            if (building.indoors.Value is FarmHouse)
              (building.indoors.Value as FarmHouse).updateCellarWarps();
          }
        }
        if (location is FarmHouse)
          (location as FarmHouse).updateCellarWarps();
      }
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      yield return 50;
      yield return 51;
      Game1.isDebrisWeather = SaveGame.loaded.isDebrisWeather;
      if (Game1.isDebrisWeather)
        Game1.populateDebrisWeatherArray();
      else
        Game1.debrisWeather.Clear();
      yield return 53;
      Game1.player.team.sharedDailyLuck.Value = SaveGame.loaded.dailyLuck;
      yield return 54;
      yield return 55;
      Game1.setGraphicsForSeason();
      yield return 56;
      Game1.samBandName = SaveGame.loaded.samBandName;
      Game1.elliottBookName = SaveGame.loaded.elliottBookName;
      Game1.shippingTax = SaveGame.loaded.shippingTax;
      Game1.cropsOfTheWeek = SaveGame.loaded.cropsOfTheWeek;
      yield return 60;
      FurniturePlacer.addAllFurnitureOwnedByFarmer();
      yield return 63;
      Game1.weddingToday = SaveGame.loaded.weddingToday;
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4700");
      yield return 64;
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4701");
      yield return 73;
      Game1.farmerWallpaper = SaveGame.loaded.farmerWallpaper;
      yield return 75;
      Game1.updateWallpaperInFarmHouse(Game1.farmerWallpaper);
      yield return 77;
      Game1.FarmerFloor = SaveGame.loaded.FarmerFloor;
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      yield return 79;
      Game1.updateFloorInFarmHouse(Game1.FarmerFloor);
      Game1.options.musicVolumeLevel = SaveGame.loaded.musicVolume;
      Game1.options.soundVolumeLevel = SaveGame.loaded.soundVolume;
      yield return 83;
      if (SaveGame.loaded.countdownToWedding.HasValue && SaveGame.loaded.countdownToWedding.Value != 0 && SaveGame.loaded.player.spouse != null && SaveGame.loaded.player.spouse != "")
      {
        WorldDate worldDate = new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
        worldDate.TotalDays += SaveGame.loaded.countdownToWedding.Value;
        Friendship friendship = SaveGame.loaded.player.friendshipData[SaveGame.loaded.player.spouse];
        friendship.Status = FriendshipStatus.Engaged;
        friendship.WeddingDate = worldDate;
      }
      yield return 85;
      yield return 87;
      Game1.chanceToRainTomorrow = SaveGame.loaded.chanceToRainTomorrow;
      yield return 88;
      yield return 95;
      Game1.currentSongIndex = SaveGame.loaded.currentSongIndex;
      Game1.fadeToBlack = true;
      Game1.fadeIn = false;
      Game1.fadeToBlackAlpha = 0.99f;
      Vector2 mostRecentBed = Game1.player.mostRecentBed;
      if ((double) Game1.player.mostRecentBed.X <= 0.0)
        Game1.player.Position = new Vector2(192f, 384f);
      Game1.removeFrontLayerForFarmBuildings();
      Game1.addNewFarmBuildingMaps();
      GameLocation gameLocation = (GameLocation) null;
      if (Game1.player.lastSleepLocation.Value != null && Game1.isLocationAccessible(Game1.player.lastSleepLocation.Value))
        gameLocation = Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) Game1.player.lastSleepLocation);
      bool flag1 = true;
      if (gameLocation != null && (Game1.player.sleptInTemporaryBed.Value || gameLocation.GetFurnitureAt(Utility.PointToVector2((Point) (NetFieldBase<Point, NetPoint>) Game1.player.lastSleepPoint)) is BedFurniture))
      {
        Game1.currentLocation = gameLocation;
        Game1.player.currentLocation = Game1.currentLocation;
        Game1.player.Position = Utility.PointToVector2((Point) (NetFieldBase<Point, NetPoint>) Game1.player.lastSleepPoint) * 64f;
        flag1 = false;
      }
      if (flag1)
        Game1.currentLocation = Game1.getLocationFromName("FarmHouse");
      Game1.currentLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
      Game1.player.CanMove = true;
      Game1.player.ReequipEnchantments();
      if (SaveGame.loaded.junimoKartLeaderboards != null)
        Game1.player.team.junimoKartScores.LoadScores(SaveGame.loaded.junimoKartLeaderboards.GetScores());
      Game1.minecartHighScore = SaveGame.loaded.minecartHighScore;
      Game1.currentWallpaper = SaveGame.loaded.currentWallpaper;
      Game1.currentFloor = SaveGame.loaded.currentFloor;
      Game1.options = SaveGame.loaded.options;
      Game1.splitscreenOptions = SaveGame.loaded.splitscreenOptions;
      Game1.CustomData = SaveGame.loaded.CustomData;
      Game1.hasApplied1_3_UpdateChanges = SaveGame.loaded.hasApplied1_3_UpdateChanges;
      Game1.hasApplied1_4_UpdateChanges = SaveGame.loaded.hasApplied1_4_UpdateChanges;
      Game1.RefreshQuestOfTheDay();
      Game1.player.team.broadcastedMail.Clear();
      if (SaveGame.loaded.broadcastedMail != null)
      {
        foreach (string str in SaveGame.loaded.broadcastedMail)
          Game1.player.team.broadcastedMail.Add(str);
      }
      if (Game1.options == null)
      {
        Game1.options = new Options();
        Game1.options.LoadDefaultOptions();
      }
      else
      {
        Game1.options.platformClampValues();
        Game1.options.SaveDefaultOptions();
      }
      try
      {
        StartupPreferences startupPreferences = new StartupPreferences();
        startupPreferences.loadPreferences(false, false);
        Game1.options.gamepadMode = startupPreferences.gamepadMode;
      }
      catch (Exception ex)
      {
      }
      if (Game1.soundBank != null)
        Game1.initializeVolumeLevels();
      Game1.multiplayer.latestID = (ulong) SaveGame.loaded.latestID;
      Game1.netWorldState.Value.SkullCavesDifficulty = SaveGame.loaded.skullCavesDifficulty;
      Game1.netWorldState.Value.MinesDifficulty = SaveGame.loaded.minesDifficulty;
      Game1.netWorldState.Value.VisitsUntilY1Guarantee = SaveGame.loaded.visitsUntilY1Guarantee;
      Game1.netWorldState.Value.ShuffleMineChests = SaveGame.loaded.shuffleMineChests;
      Game1.netWorldState.Value.DishOfTheDay.Value = SaveGame.loaded.dishOfTheDay;
      if (Game1.IsRainingHere())
        Game1.changeMusicTrack("rain", true);
      Game1.updateWeatherIcon();
      Game1.netWorldState.Value.MiniShippingBinsObtained.Set(SaveGame.loaded.miniShippingBinsObtained);
      Game1.netWorldState.Value.LostBooksFound.Set(SaveGame.loaded.lostBooksFound);
      Game1.netWorldState.Value.GoldenWalnuts.Set(SaveGame.loaded.goldenWalnuts);
      Game1.netWorldState.Value.GoldenWalnutsFound.Set(SaveGame.loaded.goldenWalnutsFound);
      Game1.netWorldState.Value.GoldenCoconutCracked.Value = SaveGame.loaded.goldenCoconutCracked;
      Game1.netWorldState.Value.FoundBuriedNuts.Clear();
      foreach (string foundBuriedNut in SaveGame.loaded.foundBuriedNuts)
        Game1.netWorldState.Value.FoundBuriedNuts[foundBuriedNut] = true;
      IslandSouth.SetupIslandSchedules();
      Game1.player.team.farmhandsCanMoveBuildings.Value = (FarmerTeam.RemoteBuildingPermissions) SaveGame.loaded.moveBuildingPermissionMode;
      Game1.player.team.mineShrineActivated.Value = SaveGame.loaded.mineShrineActivated;
      if (Game1.multiplayerMode == (byte) 2)
      {
        if (Program.sdk.Networking != null && Game1.options.serverPrivacy == ServerPrivacy.InviteOnly)
          Game1.options.setServerMode("invite");
        else if (Program.sdk.Networking != null && Game1.options.serverPrivacy == ServerPrivacy.FriendsOnly)
          Game1.options.setServerMode("friends");
        else
          Game1.options.setServerMode("friends");
      }
      Game1.bannedUsers = SaveGame.loaded.bannedUsers;
      bool flag2 = false;
      if (SaveGame.loaded.lostBooksFound < 0)
        flag2 = true;
      SaveGame.loaded = (SaveGame) null;
      Game1.currentLocation.lastTouchActionLocation = Game1.player.getTileLocation();
      if (Game1.player.horseName.Value == null)
      {
        Horse horse = Utility.findHorse(Guid.Empty);
        if (horse != null && horse.displayName != "")
        {
          Game1.player.horseName.Value = horse.displayName;
          horse.ownerId.Value = Game1.player.UniqueMultiplayerID;
        }
      }
      Game1.UpdateHorseOwnership();
      foreach (Item obj in (NetList<Item, NetRef<Item>>) Game1.player.items)
      {
        if (obj != null && obj is Object)
          (obj as Object).reloadSprite();
      }
      Game1.gameMode = (byte) 3;
      Game1.AddModNPCs();
      try
      {
        Game1.fixProblems();
      }
      catch (Exception ex)
      {
      }
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        LevelUpMenu.AddMissedProfessionChoices(allFarmer);
        LevelUpMenu.AddMissedLevelRecipes(allFarmer);
        LevelUpMenu.RevalidateHealth(allFarmer);
      }
      SaveGame.updateWedding();
      foreach (Building building in Game1.getFarm().buildings)
      {
        if ((int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0 && building.indoors.Value is Cabin)
          (building.indoors.Value as Cabin).updateFarmLayout();
        if ((int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0 && building.indoors.Value is Shed)
        {
          Shed interior = building.indoors.Value as Shed;
          interior.updateLayout();
          building.updateInteriorWarps((GameLocation) interior);
        }
      }
      if (!Game1.hasApplied1_3_UpdateChanges)
        Game1.apply1_3_UpdateChanges();
      if (!Game1.hasApplied1_4_UpdateChanges)
      {
        Game1.apply1_4_UpdateChanges();
      }
      else
      {
        if (flag2)
          Game1.recalculateLostBookCount();
        Game1.UpdateFarmPerfection();
        Game1.doMorningStuff();
      }
      int lastAppliedSaveFix = (int) Game1.lastAppliedSaveFix;
      while (lastAppliedSaveFix < 35)
      {
        if (Enum.IsDefined(typeof (SaveGame.SaveFixes), (object) lastAppliedSaveFix))
        {
          ++lastAppliedSaveFix;
          Console.WriteLine("Applying save fix: " + ((SaveGame.SaveFixes) lastAppliedSaveFix).ToString());
          Game1.applySaveFix((SaveGame.SaveFixes) lastAppliedSaveFix);
          Game1.lastAppliedSaveFix = (SaveGame.SaveFixes) lastAppliedSaveFix;
        }
      }
      if (flag1 && Game1.player.currentLocation is FarmHouse)
        Game1.player.Position = Utility.PointToVector2((Game1.player.currentLocation as FarmHouse).GetPlayerBedSpot()) * 64f;
      BedFurniture.ShiftPositionForBed(Game1.player);
      Game1.stats.checkForAchievements();
      if (Game1.stats.stat_dictionary.ContainsKey("walnutsFound"))
      {
        Game1.netWorldState.Value.GoldenWalnutsFound.Value += (int) Game1.stats.stat_dictionary["walnutsFound"];
        Game1.stats.stat_dictionary.Remove("walnutsFound");
      }
      if (Game1.IsMasterGame)
        Game1.netWorldState.Value.UpdateFromGame1();
      Console.WriteLine("getLoadEnumerator() exited, elapsed = '{0}'", (object) stopwatch.Elapsed);
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      SaveGame.IsProcessing = false;
      Game1.player.currentLocation.lastTouchActionLocation = Game1.player.getTileLocation();
      Game1.player.currentLocation.resetForPlayerEntry();
      Game1.player.showToolUpgradeAvailability();
      Game1.dayTimeMoneyBox.questsDirty = true;
      yield return 100;
    }

    private static void updateWedding()
    {
    }

    public static void loadDataToFarmer(Farmer target)
    {
      Farmer farmer = target;
      target.gameVersion = farmer.gameVersion;
      target.items.CopyFrom((IList<Item>) farmer.items);
      target.canMove = true;
      target.Sprite = (AnimatedSprite) new FarmerSprite((string) null);
      target.FarmerSprite.setOwner(target);
      if (target.cookingRecipes == null || target.cookingRecipes.Count() == 0)
        target.cookingRecipes.Add("Fried Egg", 0);
      if (target.craftingRecipes == null || target.craftingRecipes.Count() == 0)
        target.craftingRecipes.Add("Lumber", 0);
      if (!target.songsHeard.Contains("title_day"))
        target.songsHeard.Add("title_day");
      if (!target.songsHeard.Contains("title_night"))
        target.songsHeard.Add("title_night");
      if (target.addedSpeed > 0)
        target.addedSpeed = 0;
      target.maxItems.Value = (int) (NetFieldBase<int, NetInt>) farmer.maxItems;
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) target.maxItems; ++index)
      {
        if (target.items.Count <= index)
          target.items.Add((Item) null);
      }
      if (target.FarmerRenderer == null)
        target.FarmerRenderer = new FarmerRenderer(target.getTexture(), target);
      target.changeGender(farmer.IsMale);
      target.changeAccessory((int) (NetFieldBase<int, NetInt>) farmer.accessory);
      target.changeShirt((int) (NetFieldBase<int, NetInt>) farmer.shirt);
      target.changePants((Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) farmer.pantsColor);
      target.changeSkinColor((int) (NetFieldBase<int, NetInt>) farmer.skin);
      target.changeHairColor((Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) farmer.hairstyleColor);
      target.changeHairStyle((int) (NetFieldBase<int, NetInt>) farmer.hair);
      target.changeShoeColor((int) (NetFieldBase<int, NetInt>) farmer.shoes);
      target.changeEyeColor((Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) farmer.newEyeColor);
      target.Stamina = farmer.Stamina;
      target.health = farmer.health;
      target.MaxStamina = farmer.MaxStamina;
      target.mostRecentBed = farmer.mostRecentBed;
      target.Position = target.mostRecentBed;
      target.position.X -= 64f;
      if (!target.craftingRecipes.ContainsKey("Wood Path"))
        target.craftingRecipes.Add("Wood Path", 1);
      if (!target.craftingRecipes.ContainsKey("Gravel Path"))
        target.craftingRecipes.Add("Gravel Path", 1);
      if (!target.craftingRecipes.ContainsKey("Cobblestone Path"))
        target.craftingRecipes.Add("Cobblestone Path", 1);
      if (target.friendships != null && target.friendshipData.Count() == 0)
      {
        foreach (KeyValuePair<string, int[]> friendship in (Dictionary<string, int[]>) target.friendships)
          target.friendshipData[friendship.Key] = new Friendship(friendship.Value[0])
          {
            GiftsThisWeek = friendship.Value[1],
            TalkedToToday = (uint) friendship.Value[2] > 0U,
            GiftsToday = friendship.Value[3],
            ProposalRejected = (uint) friendship.Value[4] > 0U
          };
        target.friendships = (SerializableDictionary<string, int[]>) null;
      }
      if (target.spouse != null && target.spouse != "")
      {
        bool flag = target.spouse.Contains("engaged");
        string key = target.spouse.Replace("engaged", "");
        Friendship friendship = target.friendshipData[key];
        if (((friendship.Status == FriendshipStatus.Friendly ? 1 : (friendship.Status == FriendshipStatus.Dating ? 1 : 0)) | (flag ? 1 : 0)) != 0)
        {
          friendship.Status = !flag ? FriendshipStatus.Married : FriendshipStatus.Engaged;
          target.spouse = key;
          if (!flag)
          {
            friendship.WeddingDate = new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
            friendship.WeddingDate.TotalDays -= target.daysMarried;
            target.daysMarried = 0;
          }
        }
      }
      target.questLog.Filter((Func<Quest, bool>) (x => x != null));
      target.songsHeard = target.songsHeard.Distinct<string>().ToList<string>();
      target.ConvertClothingOverrideToClothesItems();
      target.UpdateClothing();
    }

    public static void loadDataToLocations(List<GameLocation> gamelocations)
    {
      foreach (GameLocation gamelocation in gamelocations)
      {
        if (gamelocation is Cellar && Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) gamelocation.name) == null)
          Game1.locations.Add((GameLocation) new Cellar("Maps\\Cellar", (string) (NetFieldBase<string, NetString>) gamelocation.name));
        if (gamelocation is FarmHouse)
        {
          GameLocation locationFromName = Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) gamelocation.name);
          (locationFromName as FarmHouse).setMapForUpgradeLevel((locationFromName as FarmHouse).upgradeLevel);
          (locationFromName as FarmHouse).fireplaceOn.Value = (bool) (NetFieldBase<bool, NetBool>) (gamelocation as FarmHouse).fireplaceOn;
          (locationFromName as FarmHouse).fridge.Value = (Chest) (NetFieldBase<Chest, NetRef<Chest>>) (gamelocation as FarmHouse).fridge;
          (locationFromName as FarmHouse).farmerNumberOfOwner = (gamelocation as FarmHouse).farmerNumberOfOwner;
          (locationFromName as FarmHouse).ReadWallpaperAndFloorTileData();
        }
        if (gamelocation.name.Equals((object) "Farm"))
        {
          GameLocation locationFromName = Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) gamelocation.name);
          foreach (Building building in ((BuildableGameLocation) gamelocation).buildings)
            building.load();
          ((BuildableGameLocation) locationFromName).buildings.Set((ICollection<Building>) ((BuildableGameLocation) gamelocation).buildings);
          foreach (FarmAnimal farmAnimal in ((Farm) gamelocation).animals.Values)
            farmAnimal.reload((Building) null);
          ((Farm) locationFromName).greenhouseUnlocked.Value = ((Farm) gamelocation).greenhouseUnlocked.Value;
          ((Farm) locationFromName).greenhouseMoved.Value = ((Farm) gamelocation).greenhouseMoved.Value;
          ((Farm) locationFromName).UpdatePatio();
        }
        if (gamelocation.name.Equals((object) "MovieTheater"))
          (Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) gamelocation.name) as MovieTheater).dayFirstEntered.Set((int) (NetFieldBase<int, NetInt>) (gamelocation as MovieTheater).dayFirstEntered);
      }
      Game1.netWorldState.Value.ParrotPlatformsUnlocked.Set(SaveGame.loaded.parrotPlatformsUnlocked);
      Game1.player.team.farmPerfect.Value = SaveGame.loaded.farmPerfect;
      foreach (GameLocation gamelocation in gamelocations)
      {
        GameLocation locationFromName = Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) gamelocation.name);
        if (locationFromName != null)
        {
          locationFromName.miniJukeboxCount.Value = gamelocation.miniJukeboxCount.Value;
          locationFromName.miniJukeboxTrack.Value = gamelocation.miniJukeboxTrack.Value;
          locationFromName.furniture.Set((ICollection<Furniture>) gamelocation.furniture);
          foreach (Furniture furniture in locationFromName.furniture)
            furniture.updateDrawPosition();
          for (int index = gamelocation.characters.Count - 1; index >= 0; --index)
            SaveGame.initializeCharacter(gamelocation.characters[index], locationFromName);
          foreach (LargeTerrainFeature largeTerrainFeature in gamelocation.largeTerrainFeatures)
          {
            largeTerrainFeature.currentLocation = locationFromName;
            largeTerrainFeature.loadSprite();
          }
          foreach (TerrainFeature terrainFeature in gamelocation.terrainFeatures.Values)
          {
            terrainFeature.currentLocation = locationFromName;
            terrainFeature.loadSprite();
          }
          foreach (KeyValuePair<Vector2, Object> pair in gamelocation.objects.Pairs)
          {
            pair.Value.initializeLightSource(pair.Key);
            pair.Value.reloadSprite();
          }
          if (gamelocation.name.Equals((object) "Farm"))
          {
            ((BuildableGameLocation) locationFromName).buildings.Set((ICollection<Building>) ((BuildableGameLocation) gamelocation).buildings);
            foreach (FarmAnimal farmAnimal in ((Farm) gamelocation).animals.Values)
              farmAnimal.reload((Building) null);
            foreach (Building building in ((BuildableGameLocation) locationFromName).buildings)
              building.load();
          }
          if (locationFromName != null)
          {
            locationFromName.characters.Set((ICollection<NPC>) gamelocation.characters);
            locationFromName.netObjects.Set((IEnumerable<KeyValuePair<Vector2, Object>>) gamelocation.netObjects.Pairs);
            locationFromName.numberOfSpawnedObjectsOnMap = gamelocation.numberOfSpawnedObjectsOnMap;
            locationFromName.terrainFeatures.Set((IEnumerable<KeyValuePair<Vector2, TerrainFeature>>) gamelocation.terrainFeatures.Pairs);
            locationFromName.largeTerrainFeatures.Set((ICollection<LargeTerrainFeature>) gamelocation.largeTerrainFeatures);
            if (locationFromName.name.Equals((object) "Farm"))
            {
              ((Farm) locationFromName).animals.MoveFrom(((Farm) gamelocation).animals);
              (locationFromName as Farm).piecesOfHay.Value = (int) (NetFieldBase<int, NetInt>) (gamelocation as Farm).piecesOfHay;
              List<ResourceClump> other = new List<ResourceClump>((IEnumerable<ResourceClump>) (gamelocation as Farm).resourceClumps);
              (gamelocation as Farm).resourceClumps.Clear();
              (locationFromName as Farm).resourceClumps.Set((ICollection<ResourceClump>) other);
              (locationFromName as Farm).hasSeenGrandpaNote = (gamelocation as Farm).hasSeenGrandpaNote;
              (locationFromName as Farm).grandpaScore = (gamelocation as Farm).grandpaScore;
              (locationFromName as Farm).petBowlWatered.Set((gamelocation as Farm).petBowlWatered.Value);
            }
            if (locationFromName.name.Equals((object) "Town"))
              (locationFromName as Town).daysUntilCommunityUpgrade.Value = (int) (NetFieldBase<int, NetInt>) (gamelocation as Town).daysUntilCommunityUpgrade;
            if (locationFromName is Beach)
              (locationFromName as Beach).bridgeFixed.Value = (bool) (NetFieldBase<bool, NetBool>) (gamelocation as Beach).bridgeFixed;
            if (locationFromName is Woods)
            {
              (locationFromName as Woods).stumps.MoveFrom((NetList<ResourceClump, NetRef<ResourceClump>>) (gamelocation as Woods).stumps);
              (locationFromName as Woods).hasUnlockedStatue.Value = (gamelocation as Woods).hasUnlockedStatue.Value;
            }
            if (locationFromName is CommunityCenter)
              (locationFromName as CommunityCenter).areasComplete.Set((IList<bool>) (gamelocation as CommunityCenter).areasComplete);
            if (locationFromName is ShopLocation && gamelocation is ShopLocation)
            {
              (locationFromName as ShopLocation).itemsFromPlayerToSell.MoveFrom((NetList<Item, NetRef<Item>>) (gamelocation as ShopLocation).itemsFromPlayerToSell);
              (locationFromName as ShopLocation).itemsToStartSellingTomorrow.MoveFrom((NetList<Item, NetRef<Item>>) (gamelocation as ShopLocation).itemsToStartSellingTomorrow);
            }
            if (locationFromName is Forest)
            {
              if (Game1.dayOfMonth % 7 % 5 == 0)
              {
                (locationFromName as Forest).travelingMerchantDay = true;
                (locationFromName as Forest).travelingMerchantBounds.Clear();
                (locationFromName as Forest).travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(1472, 640, 492, 112));
                (locationFromName as Forest).travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(1652, 744, 76, 48));
                (locationFromName as Forest).travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(1812, 744, 104, 48));
                foreach (Microsoft.Xna.Framework.Rectangle travelingMerchantBound in (locationFromName as Forest).travelingMerchantBounds)
                  Utility.clearObjectsInArea(travelingMerchantBound, locationFromName);
              }
              (locationFromName as Forest).log = (gamelocation as Forest).log;
            }
            locationFromName.TransferDataFromSavedLocation(gamelocation);
            if (locationFromName is IslandLocation)
              (locationFromName as IslandLocation).AddAdditionalWalnutBushes();
          }
        }
      }
      List<NPC> npcList = new List<NPC>();
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        for (int index = 0; index < location.characters.Count; ++index)
          npcList.Add(location.characters[index]);
      }
      for (int index = 0; index < npcList.Count; ++index)
        npcList[index].reloadSprite();
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        if (location is BuildableGameLocation)
        {
          foreach (Building building in (location as BuildableGameLocation).buildings)
          {
            GameLocation gameLocation = building.indoors.Value;
            if (gameLocation != null)
            {
              for (int index = gameLocation.characters.Count - 1; index >= 0; --index)
              {
                if (index < gameLocation.characters.Count)
                  gameLocation.characters[index].reloadSprite();
              }
            }
          }
        }
      }
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        if (location.name.Equals((object) "Farm"))
        {
          Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) location.name);
          foreach (Building building in ((BuildableGameLocation) location).buildings)
          {
            if (building is Stable && (int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0)
              (building as Stable).grabHorse();
          }
        }
      }
      if (Game1.getLocationFromName("FarmCave") is FarmCave locationFromName1)
        locationFromName1.UpdateReadyFlag();
      Game1.player.currentLocation = (GameLocation) Utility.getHomeOfFarmer(Game1.player);
    }

    public static void initializeCharacter(NPC c, GameLocation location)
    {
      string defaultMap = c.DefaultMap;
      Vector2 defaultPosition = c.DefaultPosition;
      c.reloadData();
      if (c.DefaultMap != defaultMap)
      {
        c.DefaultMap = defaultMap;
        c.DefaultPosition = defaultPosition;
      }
      if (!c.DefaultPosition.Equals(Vector2.Zero))
        c.Position = c.DefaultPosition;
      c.currentLocation = location;
      if (c.datingFarmer)
      {
        Friendship friendship = Game1.player.friendshipData[c.Name];
        if (!friendship.IsDating())
          friendship.Status = FriendshipStatus.Dating;
        c.datingFarmer = false;
      }
      else
      {
        if (!c.divorcedFromFarmer)
          return;
        Friendship friendship = Game1.player.friendshipData[c.Name];
        if (!friendship.IsDivorced())
        {
          friendship.RoommateMarriage = false;
          friendship.Status = FriendshipStatus.Divorced;
        }
        c.divorcedFromFarmer = false;
      }
    }

    public enum SaveFixes
    {
      NONE,
      StoredBigCraftablesStackFix,
      PorchedCabinBushesFix,
      ChangeObeliskFootprintHeight,
      CreateStorageDressers,
      InferPreserves,
      TransferHatSkipHairFlag,
      RevealSecretNoteItemTastes,
      TransferHoneyTypeToPreserves,
      TransferNoteBlockScale,
      FixCropHarvestAmountsAndInferSeedIndex,
      Level9PuddingFishingRecipe,
      Level9PuddingFishingRecipe2,
      quarryMineBushes,
      MissingQisChallenge,
      BedsToFurniture,
      ChildBedsToFurniture,
      ModularizeFarmStructures,
      FixFlooringFlags,
      AddBugSteakRecipe,
      AddBirdie,
      AddTownBush,
      AddNewRingRecipes1_5,
      ResetForges,
      AddSquidInkRavioli,
      MakeDarkSwordVampiric,
      FixRingSheetIndex,
      FixBeachFarmBushes,
      AddCampfireKit,
      Level9PuddingFishingRecipe3,
      OstrichIncubatorFragility,
      FixBotchedBundleData,
      LeoChildrenFix,
      Leo6HeartGermanFix,
      BirdieQuestRemovedFix,
      SkippedSummit,
      MAX,
    }
  }
}
