// Decompiled with JetBrains decompiler
// Type: StardewValley.FarmerTeam
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley
{
  public class FarmerTeam : INetObject<NetFields>
  {
    public readonly NetIntDelta money = new NetIntDelta(500);
    public readonly NetLongDictionary<NetIntDelta, NetRef<NetIntDelta>> individualMoney = new NetLongDictionary<NetIntDelta, NetRef<NetIntDelta>>();
    public readonly NetIntDelta totalMoneyEarned = new NetIntDelta(0);
    public readonly NetBool hasRustyKey = new NetBool();
    public readonly NetBool hasSkullKey = new NetBool();
    public readonly NetBool canUnderstandDwarves = new NetBool();
    public readonly NetBool useSeparateWallets = new NetBool();
    public readonly NetBool newLostAndFoundItems = new NetBool();
    public readonly NetBool toggleMineShrineOvernight = new NetBool();
    public readonly NetBool mineShrineActivated = new NetBool();
    public readonly NetBool farmPerfect = new NetBool();
    public readonly NetList<string, NetString> specialRulesRemovedToday = new NetList<string, NetString>();
    public readonly NetList<int, NetInt> itemsToRemoveOvernight = new NetList<int, NetInt>();
    public readonly NetList<string, NetString> mailToRemoveOvernight = new NetList<string, NetString>();
    public NetIntDictionary<long, NetLong> cellarAssignments = new NetIntDictionary<long, NetLong>();
    public NetStringList broadcastedMail = new NetStringList();
    public NetStringDictionary<bool, NetBool> collectedNutTracker = new NetStringDictionary<bool, NetBool>();
    public NetStringDictionary<bool, NetBool> completedSpecialOrders = new NetStringDictionary<bool, NetBool>();
    public NetList<SpecialOrder, NetRef<SpecialOrder>> specialOrders = new NetList<SpecialOrder, NetRef<SpecialOrder>>();
    public NetList<SpecialOrder, NetRef<SpecialOrder>> availableSpecialOrders = new NetList<SpecialOrder, NetRef<SpecialOrder>>();
    public NetList<string, NetString> acceptedSpecialOrderTypes = new NetList<string, NetString>();
    public readonly NetCollection<Item> returnedDonations = new NetCollection<Item>();
    public readonly NetObjectList<Item> junimoChest = new NetObjectList<Item>();
    public readonly NetFarmerCollection announcedSleepingFarmers = new NetFarmerCollection();
    public readonly NetEnum<FarmerTeam.SleepAnnounceModes> sleepAnnounceMode = new NetEnum<FarmerTeam.SleepAnnounceModes>(FarmerTeam.SleepAnnounceModes.All);
    public readonly NetEnum<FarmerTeam.RemoteBuildingPermissions> farmhandsCanMoveBuildings = new NetEnum<FarmerTeam.RemoteBuildingPermissions>(FarmerTeam.RemoteBuildingPermissions.Off);
    private readonly NetStringDictionary<ReadyCheck, NetRef<ReadyCheck>> readyChecks = new NetStringDictionary<ReadyCheck, NetRef<ReadyCheck>>();
    private readonly NetLongDictionary<Proposal, NetRef<Proposal>> proposals = new NetLongDictionary<Proposal, NetRef<Proposal>>();
    public readonly NetList<MovieInvitation, NetRef<MovieInvitation>> movieInvitations = new NetList<MovieInvitation, NetRef<MovieInvitation>>();
    public readonly NetCollection<Item> luauIngredients = new NetCollection<Item>();
    public readonly NetCollection<Item> grangeDisplay = new NetCollection<Item>();
    public readonly NetMutex grangeMutex = new NetMutex();
    public readonly NetMutex returnedDonationsMutex = new NetMutex();
    public readonly NetMutex ordersBoardMutex = new NetMutex();
    public readonly NetMutex qiChallengeBoardMutex = new NetMutex();
    public readonly NetMutex junimoChestMutex = new NetMutex();
    private readonly NetEvent1Field<Rectangle, NetRectangle> festivalPropRemovalEvent = new NetEvent1Field<Rectangle, NetRectangle>();
    public readonly NetEvent1Field<int, NetInt> addQiGemsToTeam = new NetEvent1Field<int, NetInt>();
    public readonly NetEvent1Field<string, NetString> addCharacterEvent = new NetEvent1Field<string, NetString>();
    public readonly NetEvent1Field<string, NetString> requestAddCharacterEvent = new NetEvent1Field<string, NetString>();
    public readonly NetEvent0 requestLeoMove = new NetEvent0();
    public readonly NetEvent0 kickOutOfMinesEvent = new NetEvent0();
    public readonly NetEvent1Field<long, NetLong> requestSpouseSleepEvent = new NetEvent1Field<long, NetLong>();
    public readonly NetEvent1Field<int, NetInt> ringPhoneEvent = new NetEvent1Field<int, NetInt>();
    public readonly NetEvent1Field<long, NetLong> requestHorseWarpEvent = new NetEvent1Field<long, NetLong>();
    public readonly NetEvent1Field<long, NetLong> requestPetWarpHomeEvent = new NetEvent1Field<long, NetLong>();
    public readonly NetEvent1Field<long, NetLong> requestMovieEndEvent = new NetEvent1Field<long, NetLong>();
    public readonly NetEvent1Field<long, NetLong> endMovieEvent = new NetEvent1Field<long, NetLong>();
    public readonly NetEvent1Field<Guid, NetGuid> demolishStableEvent = new NetEvent1Field<Guid, NetGuid>();
    public readonly NetStringDictionary<int, NetInt> limitedNutDrops = new NetStringDictionary<int, NetInt>();
    private readonly NetEvent1<NutDropRequest> requestNutDrop = new NetEvent1<NutDropRequest>();
    public readonly NetFarmerPairDictionary<Friendship, NetRef<Friendship>> friendshipData = new NetFarmerPairDictionary<Friendship, NetRef<Friendship>>();
    public readonly NetWitnessedLock demolishLock = new NetWitnessedLock();
    public readonly NetMutex buildLock = new NetMutex();
    public readonly NetMutex movieMutex = new NetMutex();
    public readonly NetMutex goldenCoconutMutex = new NetMutex();
    public readonly SynchronizedShopStock synchronizedShopStock = new SynchronizedShopStock();
    public readonly NetLong theaterBuildDate = new NetLong(-1L);
    public readonly NetInt lastDayQueenOfSauceRerunUpdated = new NetInt(0);
    public readonly NetInt queenOfSauceRerunWeek = new NetInt(1);
    public readonly NetDouble sharedDailyLuck = new NetDouble(1.0 / 1000.0);
    public readonly NetBool spawnMonstersAtNight = new NetBool(false);
    public readonly NetLeaderboards junimoKartScores = new NetLeaderboards();
    public PlayerStatusList junimoKartStatus = new PlayerStatusList();
    public PlayerStatusList endOfNightStatus = new PlayerStatusList();
    public PlayerStatusList festivalScoreStatus = new PlayerStatusList();
    public PlayerStatusList sleepStatus = new PlayerStatusList();

    public NetFields NetFields { get; } = new NetFields();

    public FarmerTeam()
    {
      this.NetFields.AddFields((INetSerializable) this.money, (INetSerializable) this.totalMoneyEarned, (INetSerializable) this.hasRustyKey, (INetSerializable) this.hasSkullKey, (INetSerializable) this.canUnderstandDwarves, (INetSerializable) this.readyChecks, (INetSerializable) this.proposals, (INetSerializable) this.luauIngredients, (INetSerializable) this.grangeDisplay, (INetSerializable) this.grangeMutex.NetFields, (INetSerializable) this.festivalPropRemovalEvent, (INetSerializable) this.friendshipData, (INetSerializable) this.demolishLock.NetFields, (INetSerializable) this.buildLock.NetFields, (INetSerializable) this.movieInvitations, (INetSerializable) this.movieMutex.NetFields, (INetSerializable) this.requestMovieEndEvent, (INetSerializable) this.endMovieEvent, (INetSerializable) this.requestSpouseSleepEvent, (INetSerializable) this.useSeparateWallets, (INetSerializable) this.individualMoney, (INetSerializable) this.announcedSleepingFarmers.NetFields, (INetSerializable) this.sleepAnnounceMode, (INetSerializable) this.theaterBuildDate, (INetSerializable) this.demolishStableEvent, (INetSerializable) this.queenOfSauceRerunWeek, (INetSerializable) this.lastDayQueenOfSauceRerunUpdated, (INetSerializable) this.broadcastedMail, (INetSerializable) this.sharedDailyLuck, (INetSerializable) this.spawnMonstersAtNight, (INetSerializable) this.junimoKartScores.NetFields, (INetSerializable) this.cellarAssignments, (INetSerializable) this.synchronizedShopStock.NetFields, (INetSerializable) this.junimoKartStatus.NetFields, (INetSerializable) this.endOfNightStatus.NetFields, (INetSerializable) this.festivalScoreStatus.NetFields, (INetSerializable) this.sleepStatus.NetFields, (INetSerializable) this.farmhandsCanMoveBuildings, (INetSerializable) this.requestPetWarpHomeEvent, (INetSerializable) this.ringPhoneEvent, (INetSerializable) this.specialOrders, (INetSerializable) this.returnedDonations, (INetSerializable) this.returnedDonationsMutex.NetFields, (INetSerializable) this.goldenCoconutMutex.NetFields, (INetSerializable) this.requestNutDrop, (INetSerializable) this.limitedNutDrops, (INetSerializable) this.availableSpecialOrders, (INetSerializable) this.acceptedSpecialOrderTypes, (INetSerializable) this.ordersBoardMutex.NetFields, (INetSerializable) this.qiChallengeBoardMutex.NetFields, (INetSerializable) this.completedSpecialOrders, (INetSerializable) this.addCharacterEvent, (INetSerializable) this.requestAddCharacterEvent, (INetSerializable) this.requestLeoMove, (INetSerializable) this.collectedNutTracker, (INetSerializable) this.itemsToRemoveOvernight, (INetSerializable) this.mailToRemoveOvernight, (INetSerializable) this.newLostAndFoundItems, (INetSerializable) this.junimoChest, (INetSerializable) this.junimoChestMutex.NetFields, (INetSerializable) this.requestHorseWarpEvent, (INetSerializable) this.kickOutOfMinesEvent, (INetSerializable) this.toggleMineShrineOvernight, (INetSerializable) this.mineShrineActivated, (INetSerializable) this.specialRulesRemovedToday, (INetSerializable) this.addQiGemsToTeam, (INetSerializable) this.farmPerfect);
      this.newLostAndFoundItems.Interpolated(false, false);
      this.junimoKartStatus.sortMode = PlayerStatusList.SortMode.NumberSortDescending;
      this.festivalScoreStatus.sortMode = PlayerStatusList.SortMode.NumberSortDescending;
      this.endOfNightStatus.displayMode = PlayerStatusList.DisplayMode.Icons;
      this.endOfNightStatus.AddSpriteDefinition("sleep", "LooseSprites\\PlayerStatusList", 0, 0, 16, 16);
      this.endOfNightStatus.AddSpriteDefinition("level", "LooseSprites\\PlayerStatusList", 16, 0, 16, 16);
      this.endOfNightStatus.AddSpriteDefinition("shipment", "LooseSprites\\PlayerStatusList", 32, 0, 16, 16);
      this.endOfNightStatus.AddSpriteDefinition("ready", "LooseSprites\\PlayerStatusList", 48, 0, 16, 16);
      this.endOfNightStatus.iconAnimationFrames = 4;
      this.money.Minimum = new int?(0);
      this.festivalPropRemovalEvent.onEvent += (AbstractNetEvent1<Rectangle>.Event) (rect =>
      {
        if (Game1.CurrentEvent == null)
          return;
        Game1.CurrentEvent.removeFestivalProps(rect);
      });
      this.requestSpouseSleepEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnRequestSpouseSleepEvent);
      this.requestPetWarpHomeEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnRequestPetWarpHomeEvent);
      this.requestMovieEndEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnRequestMovieEndEvent);
      this.endMovieEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnEndMovieEvent);
      this.demolishStableEvent.onEvent += new AbstractNetEvent1<Guid>.Event(this.OnDemolishStableEvent);
      this.ringPhoneEvent.onEvent += new AbstractNetEvent1<int>.Event(this.OnRingPhoneEvent);
      this.requestNutDrop.onEvent += new AbstractNetEvent1<NutDropRequest>.Event(this.OnRequestNutDrop);
      this.requestAddCharacterEvent.onEvent += new AbstractNetEvent1<string>.Event(this.OnRequestAddCharacterEvent);
      this.addCharacterEvent.onEvent += new AbstractNetEvent1<string>.Event(this.OnAddCharacterEvent);
      this.requestLeoMove.onEvent += new NetEvent0.Event(this.OnRequestLeoMoveEvent);
      this.requestHorseWarpEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnRequestHorseWarp);
      this.kickOutOfMinesEvent.onEvent += new NetEvent0.Event(this.OnKickOutOfMinesEvent);
      this.addQiGemsToTeam.onEvent += new AbstractNetEvent1<int>.Event(this._AddQiGemsToTeam);
      this.requestHorseWarpEvent.InterpolationWait = false;
      this.requestSpouseSleepEvent.InterpolationWait = false;
      this.requestPetWarpHomeEvent.InterpolationWait = false;
    }

    protected virtual void _AddQiGemsToTeam(int amount) => Game1.player.QiGems += amount;

    public virtual void OnKickOutOfMinesEvent()
    {
      if (!(Game1.currentLocation is MineShaft))
        return;
      MineShaft currentLocation = Game1.currentLocation as MineShaft;
      if (currentLocation.getMineArea() == 77377)
      {
        Game1.player.completelyStopAnimatingOrDoingAction();
        Game1.warpFarmer(Game1.getLocationRequest("Mine"), 67, 10, 2);
      }
      else if (currentLocation.getMineArea() == 121)
      {
        Game1.player.completelyStopAnimatingOrDoingAction();
        Game1.warpFarmer(Game1.getLocationRequest("SkullCave"), 3, 4, 2);
      }
      else
      {
        Game1.player.completelyStopAnimatingOrDoingAction();
        Game1.warpFarmer(Game1.getLocationRequest("Mine"), 18, 4, 2);
      }
    }

    public virtual void OnRequestHorseWarp(long uid)
    {
      if (!Game1.IsMasterGame)
        return;
      Farmer farmer = Game1.getFarmer(uid);
      Horse horse = (Horse) null;
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building is Stable)
        {
          Stable stable = building as Stable;
          if (stable.getStableHorse() != null && stable.getStableHorse().getOwner() == farmer)
          {
            horse = stable.getStableHorse();
            break;
          }
        }
      }
      if (horse == null || Utility.GetHorseWarpRestrictionsForFarmer(farmer).Any<int>())
        return;
      horse.mutex.RequestLock((Action) (() =>
      {
        horse.mutex.ReleaseLock();
        GameLocation currentLocation1 = horse.currentLocation;
        Vector2 tileLocation1 = horse.getTileLocation();
        for (int index = 0; index < 8; ++index)
          Game1.multiplayer.broadcastSprites(currentLocation1, new TemporaryAnimatedSprite(10, new Vector2(tileLocation1.X + Utility.RandomFloat(-1f, 1f), tileLocation1.Y + Utility.RandomFloat(-1f, 0.0f)) * 64f, Color.White, animationInterval: 50f)
          {
            layerDepth = 1f,
            motion = new Vector2(Utility.RandomFloat(-0.5f, 0.5f), Utility.RandomFloat(-0.5f, 0.5f))
          });
        currentLocation1.playSoundAt("wand", horse.getTileLocation());
        GameLocation currentLocation2 = farmer.currentLocation;
        Vector2 tileLocation2 = farmer.getTileLocation();
        currentLocation2.playSoundAt("wand", tileLocation2);
        for (int index = 0; index < 8; ++index)
          Game1.multiplayer.broadcastSprites(currentLocation2, new TemporaryAnimatedSprite(10, new Vector2(tileLocation2.X + Utility.RandomFloat(-1f, 1f), tileLocation2.Y + Utility.RandomFloat(-1f, 0.0f)) * 64f, Color.White, animationInterval: 50f)
          {
            layerDepth = 1f,
            motion = new Vector2(Utility.RandomFloat(-0.5f, 0.5f), Utility.RandomFloat(-0.5f, 0.5f))
          });
        Game1.warpCharacter((NPC) horse, farmer.currentLocation, tileLocation2);
        int num = 0;
        for (int x = (int) tileLocation2.X + 3; x >= (int) tileLocation2.X - 3; --x)
        {
          Game1.multiplayer.broadcastSprites(currentLocation2, new TemporaryAnimatedSprite(6, new Vector2((float) x, tileLocation2.Y) * 64f, Color.White, animationInterval: 50f)
          {
            layerDepth = 1f,
            delayBeforeAnimationStart = num * 25,
            motion = new Vector2(-0.25f, 0.0f)
          });
          ++num;
        }
      }));
    }

    public virtual void OnRequestLeoMoveEvent()
    {
      if (!Game1.IsMasterGame)
        return;
      Game1.player.team.requestAddCharacterEvent.Fire("Leo");
      NPC characterFromName = Game1.getCharacterFromName("Leo");
      if (characterFromName == null)
        return;
      characterFromName.DefaultMap = "LeoTreeHouse";
      characterFromName.DefaultPosition = new Vector2(5f, 4f) * 64f;
      characterFromName.faceDirection(2);
      characterFromName.InvalidateMasterSchedule();
      if (characterFromName.Schedule != null)
        characterFromName.Schedule = (Dictionary<int, SchedulePathDescription>) null;
      characterFromName.controller = (PathFindController) null;
      characterFromName.temporaryController = (PathFindController) null;
      Game1.warpCharacter(characterFromName, Game1.getLocationFromName("Mountain"), new Vector2(16f, 8f));
      characterFromName.Halt();
      characterFromName.ignoreScheduleToday = false;
    }

    public virtual void MarkCollectedNut(string key) => this.collectedNutTracker[key] = true;

    public int GetIndividualMoney(Farmer who) => this.GetMoney(who).Value;

    public void AddIndividualMoney(Farmer who, int value) => this.GetMoney(who).Value += value;

    public void SetIndividualMoney(Farmer who, int value) => this.GetMoney(who).Value = value;

    public NetIntDelta GetMoney(Farmer who)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.useSeparateWallets)
        return this.money;
      if (!this.individualMoney.ContainsKey(who.UniqueMultiplayerID))
      {
        this.individualMoney[(long) who.uniqueMultiplayerID] = new NetIntDelta(500);
        this.individualMoney[(long) who.uniqueMultiplayerID].Minimum = new int?(0);
      }
      return this.individualMoney[(long) who.uniqueMultiplayerID];
    }

    public bool SpecialOrderActive(string special_order_key)
    {
      foreach (SpecialOrder specialOrder in this.specialOrders)
      {
        if ((string) (NetFieldBase<string, NetString>) specialOrder.questKey == special_order_key && specialOrder.questState.Value == SpecialOrder.QuestState.InProgress)
          return true;
      }
      return false;
    }

    public bool SpecialOrderRuleActive(string special_rule, SpecialOrder order_to_ignore = null)
    {
      foreach (SpecialOrder specialOrder in this.specialOrders)
      {
        if (specialOrder != order_to_ignore && specialOrder.questState.Value == SpecialOrder.QuestState.InProgress && specialOrder.specialRule.Value != null)
        {
          foreach (string str in specialOrder.specialRule.Value.Split(','))
          {
            if (str.Trim() == special_rule)
              return true;
          }
        }
      }
      return false;
    }

    public SpecialOrder GetAvailableSpecialOrder(int index = 0, string type = "")
    {
      foreach (SpecialOrder availableSpecialOrder in this.availableSpecialOrders)
      {
        if (availableSpecialOrder.orderType.Value == type)
        {
          if (index <= 0)
            return availableSpecialOrder;
          --index;
        }
      }
      return (SpecialOrder) null;
    }

    public void CheckReturnedDonations() => this.returnedDonationsMutex.RequestLock((Action) (() =>
    {
      Dictionary<ISalable, int[]> itemPriceAndStock = new Dictionary<ISalable, int[]>();
      foreach (Item returnedDonation in this.returnedDonations)
        itemPriceAndStock[(ISalable) returnedDonation] = new int[2]
        {
          0,
          returnedDonation.Stack
        };
      Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(itemPriceAndStock, on_purchase: new Func<ISalable, Farmer, int, bool>(this.OnDonatedItemWithdrawn), on_sell: new Func<ISalable, bool>(this.OnReturnedDonationDeposited), context: "ReturnedDonations")
      {
        source = (object) this,
        behaviorBeforeCleanup = (Action<IClickableMenu>) (menu => this.returnedDonationsMutex.ReleaseLock())
      };
    }));

    public bool OnDonatedItemWithdrawn(ISalable salable, Farmer who, int amount)
    {
      if (salable is Item && (salable.Stack <= 0 || salable.maximumStackSize() <= 1))
        this.returnedDonations.Remove(salable as Item);
      return false;
    }

    public bool OnReturnedDonationDeposited(ISalable deposited_salable) => false;

    public void OnRequestMovieEndEvent(long uid)
    {
      if (!Game1.IsMasterGame)
        return;
      (Game1.getLocationFromName("MovieTheater") as MovieTheater).RequestEndMovie(uid);
    }

    public void OnRequestPetWarpHomeEvent(long uid)
    {
      if (!Game1.IsMasterGame)
        return;
      Farmer who = Game1.getFarmerMaybeOffline(uid) ?? Game1.MasterPlayer;
      Pet characterFromName = Game1.getCharacterFromName<Pet>(who.getPetName(), false);
      if (characterFromName != null && characterFromName.currentLocation is FarmHouse || characterFromName == null)
        return;
      characterFromName.warpToFarmHouse(who);
    }

    public void OnRequestSpouseSleepEvent(long uid)
    {
      if (!Game1.IsMasterGame)
        return;
      Farmer farmerMaybeOffline = Game1.getFarmerMaybeOffline(uid);
      if (farmerMaybeOffline == null)
        return;
      NPC characterFromName = Game1.getCharacterFromName(farmerMaybeOffline.spouse);
      if (characterFromName == null || characterFromName.isSleeping.Value)
        return;
      FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(farmerMaybeOffline);
      Game1.warpCharacter(characterFromName, (GameLocation) homeOfFarmer, new Vector2((float) homeOfFarmer.getSpouseBedSpot(farmerMaybeOffline.spouse).X, (float) homeOfFarmer.getSpouseBedSpot(farmerMaybeOffline.spouse).Y));
      characterFromName.NetFields.CancelInterpolation();
      characterFromName.Halt();
      characterFromName.faceDirection(0);
      characterFromName.controller = (PathFindController) null;
      characterFromName.temporaryController = (PathFindController) null;
      characterFromName.ignoreScheduleToday = true;
      if (homeOfFarmer.GetSpouseBed() == null)
        return;
      FarmHouse.spouseSleepEndFunction((Character) characterFromName, (GameLocation) homeOfFarmer);
    }

    public virtual void OnRequestAddCharacterEvent(string character_name)
    {
      if (!Game1.IsMasterGame || !(character_name == "Leo") || Game1.player.hasOrWillReceiveMail("addedParrotBoy"))
        return;
      Game1.player.mailReceived.Add("addedParrotBoy");
      this.addCharacterEvent.Fire(character_name);
    }

    public virtual void OnAddCharacterEvent(string character_name)
    {
      if (!(character_name == "Leo"))
        return;
      Game1.addParrotBoyIfNecessary();
    }

    public void RequestLimitedNutDrops(
      string key,
      GameLocation location,
      int x,
      int y,
      int limit,
      int reward_amount = 1)
    {
      if (this.limitedNutDrops.ContainsKey(key) && this.limitedNutDrops[key] >= limit)
        return;
      if (location == null)
        this.requestNutDrop.Fire(new NutDropRequest(key, (string) null, new Point(x, y), limit, reward_amount));
      else
        this.requestNutDrop.Fire(new NutDropRequest(key, location.NameOrUniqueName, new Point(x, y), limit, reward_amount));
    }

    public int GetDroppedLimitedNutCount(string key) => this.limitedNutDrops.ContainsKey(key) ? this.limitedNutDrops[key] : 0;

    protected void OnRequestNutDrop(NutDropRequest request)
    {
      if (!Game1.IsMasterGame || this.limitedNutDrops.ContainsKey(request.key) && this.limitedNutDrops[request.key] >= request.limit)
        return;
      int rewardAmount = request.rewardAmount;
      int num;
      if (!this.limitedNutDrops.ContainsKey(request.key))
      {
        num = Math.Min(request.limit, rewardAmount);
        this.limitedNutDrops[request.key] = num;
      }
      else
      {
        num = Math.Min(request.limit - this.limitedNutDrops[request.key], rewardAmount);
        this.limitedNutDrops[request.key] += num;
      }
      GameLocation location = (GameLocation) null;
      if (request.locationName != "null")
        location = Game1.getLocationFromName(request.locationName);
      if (location != null)
      {
        for (int index = 0; index < num; ++index)
          Game1.createItemDebris((Item) new Object(73, 1), new Vector2((float) request.position.X, (float) request.position.Y), -1, location);
      }
      else
      {
        Game1.netWorldState.Value.GoldenWalnutsFound.Value += num;
        Game1.netWorldState.Value.GoldenWalnuts.Value += num;
      }
    }

    public void OnRingPhoneEvent(int which_call) => Phone.Ring(which_call);

    public void OnEndMovieEvent(long uid)
    {
      if (Game1.player.UniqueMultiplayerID != uid)
        return;
      Game1.player.lastSeenMovieWeek.Set(Game1.Date.TotalWeeks);
      if (Game1.CurrentEvent == null)
        return;
      Game1.CurrentEvent.onEventFinished += (Action) (() =>
      {
        LocationRequest locationRequest = Game1.getLocationRequest("MovieTheater");
        locationRequest.OnWarp += (LocationRequest.Callback) (() => { });
        Game1.warpFarmer(locationRequest, 13, 4, 2);
        Game1.fadeToBlackAlpha = 1f;
      });
      Game1.CurrentEvent.endBehaviors(new string[1]
      {
        "end"
      }, Game1.currentLocation);
    }

    public void OnDemolishStableEvent(Guid stable_guid)
    {
      if (Game1.player.mount == null || !(Game1.player.mount.HorseId == stable_guid))
        return;
      Game1.player.mount.dismount(true);
    }

    public void DeleteFarmhand(Farmer farmhand) => this.friendshipData.Filter((Func<KeyValuePair<FarmerPair, Friendship>, bool>) (pair => !pair.Key.Contains(farmhand.UniqueMultiplayerID)));

    public Friendship GetFriendship(long farmer1, long farmer2)
    {
      FarmerPair key = FarmerPair.MakePair(farmer1, farmer2);
      if (!this.friendshipData.ContainsKey(key))
        this.friendshipData.Add(key, new Friendship());
      return this.friendshipData[key];
    }

    public void AddAnyBroadcastedMail()
    {
      foreach (string str1 in (NetList<string, NetString>) this.broadcastedMail)
      {
        Multiplayer.PartyWideMessageQueue wideMessageQueue = Multiplayer.PartyWideMessageQueue.SeenMail;
        string id = str1;
        if (id.StartsWith("%&SM&%"))
        {
          id = id.Substring("%&SM&%".Length);
          wideMessageQueue = Multiplayer.PartyWideMessageQueue.SeenMail;
        }
        else if (id.StartsWith("%&MFT&%"))
        {
          id = id.Substring("%&MFT&%".Length);
          wideMessageQueue = Multiplayer.PartyWideMessageQueue.MailForTomorrow;
        }
        if (wideMessageQueue == Multiplayer.PartyWideMessageQueue.SeenMail)
        {
          if (id.Contains("%&NL&%") || id.StartsWith("NightMarketYear"))
          {
            string str2 = id.Replace("%&NL&%", "");
            if (!Game1.player.mailReceived.Contains(str2))
              Game1.player.mailReceived.Add(str2);
          }
          else if (!Game1.player.hasOrWillReceiveMail(id))
            Game1.player.mailbox.Add(id);
        }
        else if (!Game1.MasterPlayer.mailForTomorrow.Contains(id))
        {
          if (!Game1.player.hasOrWillReceiveMail(id))
          {
            if (id.Contains("%&NL&%"))
            {
              string str3 = id.Replace("%&NL&%", "");
              if (!Game1.player.mailReceived.Contains(str3))
                Game1.player.mailReceived.Add(str3);
            }
            else if (!Game1.player.mailbox.Contains(id))
              Game1.player.mailbox.Add(id);
          }
        }
        else if (!Game1.player.hasOrWillReceiveMail(id))
          Game1.player.mailForTomorrow.Add(id);
      }
    }

    public bool IsMarried(long farmer)
    {
      foreach (KeyValuePair<FarmerPair, Friendship> pair in this.friendshipData.Pairs)
      {
        if (pair.Key.Contains(farmer) && pair.Value.IsMarried())
          return true;
      }
      return false;
    }

    public bool IsEngaged(long farmer)
    {
      foreach (KeyValuePair<FarmerPair, Friendship> pair in this.friendshipData.Pairs)
      {
        if (pair.Key.Contains(farmer) && pair.Value.IsEngaged())
          return true;
      }
      return false;
    }

    public long? GetSpouse(long farmer)
    {
      foreach (KeyValuePair<FarmerPair, Friendship> pair in this.friendshipData.Pairs)
      {
        FarmerPair key = pair.Key;
        if (key.Contains(farmer) && (pair.Value.IsEngaged() || pair.Value.IsMarried()))
        {
          key = pair.Key;
          return new long?(key.GetOther(farmer));
        }
      }
      return new long?();
    }

    public void FestivalPropsRemoved(Rectangle rect) => this.festivalPropRemovalEvent.Fire(rect);

    public void SendProposal(Farmer receiver, ProposalType proposalType, Item gift = null)
    {
      Proposal proposal = new Proposal();
      proposal.sender.Value = Game1.player;
      proposal.receiver.Value = receiver;
      proposal.proposalType.Value = proposalType;
      proposal.gift.Value = gift;
      this.proposals[Game1.player.UniqueMultiplayerID] = proposal;
    }

    public Proposal GetOutgoingProposal()
    {
      Proposal proposal;
      return this.proposals.TryGetValue(Game1.player.UniqueMultiplayerID, out proposal) ? proposal : (Proposal) null;
    }

    public void RemoveOutgoingProposal() => this.proposals.Remove(Game1.player.UniqueMultiplayerID);

    public Proposal GetIncomingProposal()
    {
      foreach (Proposal incomingProposal in this.proposals.Values)
      {
        if (incomingProposal.receiver.Value == Game1.player && incomingProposal.response.Value == ProposalResponse.None)
          return incomingProposal;
      }
      return (Proposal) null;
    }

    private bool locationsMatch(GameLocation location1, GameLocation location2) => location1 != null && location2 != null && (location1.Name == location2.Name || (location1 is Mine || location1 is MineShaft && Convert.ToInt32(location1.Name.Substring("UndergroundMine".Length)) < 121) && (location2 is Mine || location2 is MineShaft && Convert.ToInt32(location2.Name.Substring("UndergroundMine".Length)) < 121) || (location1.Name.Equals("SkullCave") || location1 is MineShaft && Convert.ToInt32(location1.Name.Substring("UndergroundMine".Length)) >= 121) && (location2.Name.Equals("SkullCave") || location2 is MineShaft && Convert.ToInt32(location2.Name.Substring("UndergroundMine".Length)) >= 121));

    public double AverageDailyLuck(GameLocation inThisLocation = null)
    {
      double num = 0.0;
      int val1 = 0;
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (inThisLocation == null || this.locationsMatch(inThisLocation, onlineFarmer.currentLocation))
        {
          num += onlineFarmer.DailyLuck;
          ++val1;
        }
      }
      return num / (double) Math.Max(val1, 1);
    }

    public double AverageLuckLevel(GameLocation inThisLocation = null)
    {
      double num = 0.0;
      int val1 = 0;
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (inThisLocation == null || this.locationsMatch(inThisLocation, onlineFarmer.currentLocation))
        {
          num += (double) onlineFarmer.LuckLevel;
          ++val1;
        }
      }
      return num / (double) Math.Max(val1, 1);
    }

    public double AverageSkillLevel(int skillIndex, GameLocation inThisLocation = null)
    {
      double num = 0.0;
      int val1 = 0;
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (inThisLocation == null || this.locationsMatch(inThisLocation, onlineFarmer.currentLocation))
        {
          num += (double) onlineFarmer.GetSkillLevel(skillIndex);
          ++val1;
        }
      }
      return num / (double) Math.Max(val1, 1);
    }

    public void Update()
    {
      this.requestLeoMove.Poll();
      this.requestMovieEndEvent.Poll();
      this.endMovieEvent.Poll();
      this.ringPhoneEvent.Poll();
      this.festivalPropRemovalEvent.Poll();
      this.demolishStableEvent.Poll();
      this.requestSpouseSleepEvent.Poll();
      this.requestHorseWarpEvent.Poll();
      this.kickOutOfMinesEvent.Poll();
      this.requestPetWarpHomeEvent.Poll();
      this.requestNutDrop.Poll();
      this.requestAddCharacterEvent.Poll();
      this.addCharacterEvent.Poll();
      this.addQiGemsToTeam.Poll();
      this.grangeMutex.Update(Game1.getOnlineFarmers());
      this.returnedDonationsMutex.Update(Game1.getOnlineFarmers());
      this.ordersBoardMutex.Update(Game1.getOnlineFarmers());
      this.qiChallengeBoardMutex.Update(Game1.getOnlineFarmers());
      this.junimoChestMutex.Update(Game1.getOnlineFarmers());
      this.demolishLock.Update();
      this.buildLock.Update(Game1.getOnlineFarmers());
      this.movieMutex.Update(Game1.getOnlineFarmers());
      this.goldenCoconutMutex.Update(Game1.getOnlineFarmers());
      if (this.grangeMutex.IsLockHeld() && Game1.activeClickableMenu == null)
        this.grangeMutex.ReleaseLock();
      foreach (SpecialOrder specialOrder in this.specialOrders)
        specialOrder.Update();
      foreach (ReadyCheck readyCheck in this.readyChecks.Values)
        readyCheck.Update();
      if (Game1.IsMasterGame && this.proposals.Count() > 0)
        this.proposals.Filter((Func<KeyValuePair<long, Proposal>, bool>) (pair => this.playerIsOnline(pair.Key) && this.playerIsOnline(pair.Value.receiver.UID)));
      Proposal incomingProposal = this.GetIncomingProposal();
      if (incomingProposal != null && incomingProposal.canceled.Value)
        incomingProposal.cancelConfirmed.Value = true;
      if (Game1.dialogueUp)
        return;
      if (incomingProposal != null)
      {
        if (this.handleIncomingProposal(incomingProposal))
          return;
        incomingProposal.responseMessageKey.Value = this.genderedKey("Strings\\UI:Proposal_PlayerBusy", Game1.player);
        incomingProposal.response.Value = ProposalResponse.Rejected;
      }
      else
      {
        if (Game1.activeClickableMenu != null || this.GetOutgoingProposal() == null)
          return;
        Game1.activeClickableMenu = (IClickableMenu) new PendingProposalDialog();
      }
    }

    private string genderedKey(string baseKey, Farmer farmer) => baseKey + (farmer.IsMale ? "_Male" : "_Female");

    private bool handleIncomingProposal(Proposal proposal)
    {
      if (Game1.gameMode != (byte) 3 || Game1.activeClickableMenu != null || Game1.currentMinigame != null)
        return (ProposalType) (NetFieldBase<ProposalType, NetEnum<ProposalType>>) proposal.proposalType == ProposalType.Baby;
      if (Game1.currentLocation == null || proposal.proposalType.Value != ProposalType.Dance && Game1.CurrentEvent != null)
        return false;
      string sub2 = "";
      string responseYes = (string) null;
      string responseNo = (string) null;
      string baseKey;
      if ((ProposalType) (NetFieldBase<ProposalType, NetEnum<ProposalType>>) proposal.proposalType == ProposalType.Dance)
      {
        if (Game1.CurrentEvent == null || !Game1.CurrentEvent.isSpecificFestival("spring24"))
          return false;
        baseKey = "Strings\\UI:AskedToDance";
        responseYes = "Strings\\UI:AskedToDance_Accepted";
        responseNo = "Strings\\UI:AskedToDance_Rejected";
        if (Game1.player.dancePartner.Value != null)
          return false;
      }
      else if ((ProposalType) (NetFieldBase<ProposalType, NetEnum<ProposalType>>) proposal.proposalType == ProposalType.Marriage)
      {
        if (Game1.player.isMarried() || Game1.player.isEngaged())
        {
          proposal.response.Value = ProposalResponse.Rejected;
          proposal.responseMessageKey.Value = this.genderedKey("Strings\\UI:AskedToMarry_NotSingle", Game1.player);
          return true;
        }
        baseKey = "Strings\\UI:AskedToMarry";
        responseYes = "Strings\\UI:AskedToMarry_Accepted";
        responseNo = "Strings\\UI:AskedToMarry_Rejected";
      }
      else if ((ProposalType) (NetFieldBase<ProposalType, NetEnum<ProposalType>>) proposal.proposalType == ProposalType.Gift && (NetFieldBase<Item, NetRef<Item>>) proposal.gift != (NetRef<Item>) null)
      {
        if (!Game1.player.couldInventoryAcceptThisItem((Item) (NetFieldBase<Item, NetRef<Item>>) proposal.gift))
        {
          proposal.response.Value = ProposalResponse.Rejected;
          proposal.responseMessageKey.Value = this.genderedKey("Strings\\UI:GiftPlayerItem_NoInventorySpace", Game1.player);
          return true;
        }
        baseKey = "Strings\\UI:GivenGift";
        sub2 = proposal.gift.Value.DisplayName;
      }
      else
      {
        if ((ProposalType) (NetFieldBase<ProposalType, NetEnum<ProposalType>>) proposal.proposalType != ProposalType.Baby)
          return false;
        if (proposal.sender.Value.IsMale != Game1.player.IsMale)
        {
          baseKey = "Strings\\UI:AskedToHaveBaby";
          responseYes = "Strings\\UI:AskedToHaveBaby_Accepted";
          responseNo = "Strings\\UI:AskedToHaveBaby_Rejected";
        }
        else
        {
          baseKey = "Strings\\UI:AskedToAdoptBaby";
          responseYes = "Strings\\UI:AskedToAdoptBaby_Accepted";
          responseNo = "Strings\\UI:AskedToAdoptBaby_Rejected";
        }
      }
      string path = this.genderedKey(baseKey, (Farmer) proposal.sender);
      if (responseYes != null)
        responseYes = this.genderedKey(responseYes, Game1.player);
      if (responseNo != null)
        responseNo = this.genderedKey(responseNo, Game1.player);
      Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString(path, (object) proposal.sender.Value.Name, (object) sub2), Game1.currentLocation.createYesNoResponses(), (GameLocation.afterQuestionBehavior) ((_, answer) =>
      {
        if (proposal.canceled.Value)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:ProposalWithdrawn", (object) proposal.sender.Value.Name));
          proposal.response.Value = ProposalResponse.Rejected;
          proposal.responseMessageKey.Value = responseNo;
        }
        else if (answer == "Yes")
        {
          proposal.response.Value = ProposalResponse.Accepted;
          proposal.responseMessageKey.Value = responseYes;
          if (proposal.proposalType.Value == ProposalType.Gift || proposal.proposalType.Value == ProposalType.Marriage)
          {
            Item obj = proposal.gift.Value;
            proposal.gift.Value = (Item) null;
            Item inventory = Game1.player.addItemToInventory(obj);
            if (inventory != null)
              Game1.currentLocation.debris.Add(new Debris(inventory, Game1.player.Position));
          }
          if (proposal.proposalType.Value == ProposalType.Dance)
            Game1.player.dancePartner.Value = (Character) proposal.sender.Value;
          if (proposal.proposalType.Value == ProposalType.Marriage)
          {
            Friendship friendship = this.GetFriendship(proposal.sender.Value.UniqueMultiplayerID, Game1.player.UniqueMultiplayerID);
            friendship.Status = FriendshipStatus.Engaged;
            friendship.Proposer = proposal.sender.Value.UniqueMultiplayerID;
            WorldDate worldDate = new WorldDate(Game1.Date);
            worldDate.TotalDays += 3;
            while (!Game1.canHaveWeddingOnDay(worldDate.DayOfMonth, worldDate.Season))
              ++worldDate.TotalDays;
            friendship.WeddingDate = worldDate;
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:PlayerWeddingArranged"));
            Game1.multiplayer.globalChatInfoMessage("Engaged", Game1.player.Name, proposal.sender.Value.Name);
          }
          if (proposal.proposalType.Value == ProposalType.Baby)
          {
            Friendship friendship = this.GetFriendship(proposal.sender.Value.UniqueMultiplayerID, Game1.player.UniqueMultiplayerID);
            WorldDate worldDate1 = new WorldDate(Game1.Date);
            worldDate1.TotalDays += 14;
            WorldDate worldDate2 = worldDate1;
            friendship.NextBirthingDate = worldDate2;
          }
          Game1.player.doEmote(20);
        }
        else
        {
          proposal.response.Value = ProposalResponse.Rejected;
          proposal.responseMessageKey.Value = responseNo;
        }
      }));
      return true;
    }

    public bool playerIsOnline(long uid)
    {
      if (Game1.MasterPlayer.UniqueMultiplayerID == uid || (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null && Game1.serverHost.Value.UniqueMultiplayerID == uid)
        return true;
      return Game1.otherFarmers.ContainsKey(uid) && !Game1.multiplayer.isDisconnecting(uid);
    }

    public void SetLocalRequiredFarmers(string checkName, IEnumerable<Farmer> required_farmers)
    {
      if (!this.readyChecks.ContainsKey(checkName))
        this.readyChecks.Add(checkName, new ReadyCheck(checkName));
      this.readyChecks[checkName].SetRequiredFarmers(required_farmers);
    }

    public void SetLocalReady(string checkName, bool ready)
    {
      if (!this.readyChecks.ContainsKey(checkName))
        this.readyChecks.Add(checkName, new ReadyCheck(checkName));
      this.readyChecks[checkName].SetLocalReady(ready);
    }

    public bool IsReady(string checkName)
    {
      ReadyCheck readyCheck;
      return this.readyChecks.TryGetValue(checkName, out readyCheck) && readyCheck.IsReady();
    }

    public bool IsReadyCheckCancelable(string checkName)
    {
      ReadyCheck readyCheck;
      return this.readyChecks.TryGetValue(checkName, out readyCheck) && readyCheck.IsCancelable();
    }

    public bool IsOtherFarmerReady(string checkName, Farmer farmer)
    {
      ReadyCheck readyCheck;
      return this.readyChecks.TryGetValue(checkName, out readyCheck) && readyCheck.IsOtherFarmerReady(farmer);
    }

    public int GetNumberReady(string checkName)
    {
      ReadyCheck readyCheck;
      return !this.readyChecks.TryGetValue(checkName, out readyCheck) ? 0 : readyCheck.GetNumberReady();
    }

    public int GetNumberRequired(string checkName)
    {
      ReadyCheck readyCheck;
      return !this.readyChecks.TryGetValue(checkName, out readyCheck) ? 0 : readyCheck.GetNumberRequired();
    }

    public void NewDay()
    {
      if (Game1.IsClient)
        return;
      this.readyChecks.Clear();
      this.luauIngredients.Clear();
      if (this.grangeDisplay.Count > 0)
      {
        for (int index = 0; index < this.grangeDisplay.Count; ++index)
        {
          Item obj = this.grangeDisplay[index];
          this.grangeDisplay[index] = (Item) null;
          if (obj != null)
          {
            this.returnedDonations.Add(obj);
            this.newLostAndFoundItems.Value = true;
          }
        }
      }
      this.grangeDisplay.Clear();
      this.movieInvitations.Clear();
    }

    public enum RemoteBuildingPermissions
    {
      Off,
      OwnedBuildings,
      On,
    }

    public enum SleepAnnounceModes
    {
      All,
      First,
      Off,
    }
  }
}
