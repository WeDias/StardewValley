// Decompiled with JetBrains decompiler
// Type: StardewValley.Farmer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.Tools;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley
{
  public class Farmer : Character, IComparable
  {
    public const int millisecondsPerSpeedUnit = 64;
    public const byte halt = 64;
    public const byte up = 1;
    public const byte right = 2;
    public const byte down = 4;
    public const byte left = 8;
    public const byte run = 16;
    public const byte release = 32;
    public const int farmingSkill = 0;
    public const int miningSkill = 3;
    public const int fishingSkill = 1;
    public const int foragingSkill = 2;
    public const int combatSkill = 4;
    public const int luckSkill = 5;
    public const float interpolationConstant = 0.5f;
    public const int runningSpeed = 5;
    public const int walkingSpeed = 2;
    public const int caveNothing = 0;
    public const int caveBats = 1;
    public const int caveMushrooms = 2;
    public const int millisecondsInvincibleAfterDamage = 1200;
    public const int millisecondsPerFlickerWhenInvincible = 50;
    public const int startingStamina = 270;
    public const int totalLevels = 35;
    public static int tileSlideThreshold = 32;
    public const int maxInventorySpace = 36;
    public const int hotbarSize = 12;
    public const int eyesOpen = 0;
    public const int eyesHalfShut = 4;
    public const int eyesClosed = 1;
    public const int eyesRight = 2;
    public const int eyesLeft = 3;
    public const int eyesWide = 5;
    public const int rancher = 0;
    public const int tiller = 1;
    public const int butcher = 2;
    public const int shepherd = 3;
    public const int artisan = 4;
    public const int agriculturist = 5;
    public const int fisher = 6;
    public const int trapper = 7;
    public const int angler = 8;
    public const int pirate = 9;
    public const int baitmaster = 10;
    public const int mariner = 11;
    public const int forester = 12;
    public const int gatherer = 13;
    public const int lumberjack = 14;
    public const int tapper = 15;
    public const int botanist = 16;
    public const int tracker = 17;
    public const int miner = 18;
    public const int geologist = 19;
    public const int blacksmith = 20;
    public const int burrower = 21;
    public const int excavator = 22;
    public const int gemologist = 23;
    public const int fighter = 24;
    public const int scout = 25;
    public const int brute = 26;
    public const int defender = 27;
    public const int acrobat = 28;
    public const int desperado = 29;
    public readonly NetObjectList<Quest> questLog = new NetObjectList<Quest>();
    public readonly NetIntList professions = new NetIntList();
    public readonly NetList<Point, NetPoint> newLevels = new NetList<Point, NetPoint>();
    private Queue<int> newLevelSparklingTexts = new Queue<int>();
    private SparklingText sparklingText;
    public readonly NetArray<int, NetInt> experiencePoints = new NetArray<int, NetInt>(6);
    public readonly NetObjectList<Item> items = new NetObjectList<Item>();
    public readonly NetIntList dialogueQuestionsAnswered = new NetIntList();
    public List<string> furnitureOwned = new List<string>();
    [XmlElement("cookingRecipes")]
    public readonly NetStringDictionary<int, NetInt> cookingRecipes = new NetStringDictionary<int, NetInt>();
    [XmlElement("craftingRecipes")]
    public readonly NetStringDictionary<int, NetInt> craftingRecipes = new NetStringDictionary<int, NetInt>();
    [XmlElement("activeDialogueEvents")]
    public readonly NetStringDictionary<int, NetInt> activeDialogueEvents = new NetStringDictionary<int, NetInt>();
    public readonly NetIntList eventsSeen = new NetIntList();
    public readonly NetIntList secretNotesSeen = new NetIntList();
    public List<string> songsHeard = new List<string>();
    public readonly NetIntList achievements = new NetIntList();
    public readonly NetIntList specialItems = new NetIntList();
    public readonly NetIntList specialBigCraftables = new NetIntList();
    public readonly NetStringList mailReceived = new NetStringList();
    public readonly NetStringList mailForTomorrow = new NetStringList();
    public readonly NetStringList mailbox = new NetStringList();
    public readonly NetInt timeWentToBed = new NetInt();
    [XmlIgnore]
    public bool hasMoved;
    public readonly NetBool sleptInTemporaryBed = new NetBool();
    [XmlIgnore]
    public readonly NetBool requestingTimePause = new NetBool();
    public Stats stats = new Stats();
    [XmlIgnore]
    public readonly NetCollection<Item> personalShippingBin = new NetCollection<Item>();
    [XmlIgnore]
    public IList<Item> displayedShippedItems = (IList<Item>) new List<Item>();
    public List<string> blueprints = new List<string>();
    [XmlElement("biteChime")]
    public NetInt biteChime = new NetInt(-1);
    [XmlIgnore]
    public float usernameDisplayTime;
    [XmlIgnore]
    protected NetRef<Item> _recoveredItem = new NetRef<Item>();
    public NetObjectList<Item> itemsLostLastDeath = new NetObjectList<Item>();
    public List<int> movementDirections = new List<int>();
    [XmlElement("farmName")]
    public readonly NetString farmName = new NetString("");
    [XmlElement("favoriteThing")]
    public readonly NetString favoriteThing = new NetString();
    [XmlElement("horseName")]
    public readonly NetString horseName = new NetString();
    public string slotName;
    public bool slotCanHost;
    [XmlIgnore]
    public bool hasReceivedToolUpgradeMessageYet;
    [XmlIgnore]
    private readonly NetArray<int, NetInt> appliedBuffs = new NetArray<int, NetInt>(12);
    [XmlIgnore]
    public readonly NetIntDictionary<int, NetInt> appliedSpecialBuffs = new NetIntDictionary<int, NetInt>();
    [XmlIgnore]
    public IList<OutgoingMessage> messageQueue = (IList<OutgoingMessage>) new List<OutgoingMessage>();
    [XmlIgnore]
    public readonly NetLong uniqueMultiplayerID = new NetLong(Utility.RandomLong());
    [XmlElement("userID")]
    public readonly NetString userID = new NetString("");
    [XmlIgnore]
    public string previousLocationName = "";
    [XmlIgnore]
    public readonly NetString platformType = new NetString("");
    [XmlIgnore]
    public readonly NetString platformID = new NetString("");
    [XmlIgnore]
    public readonly NetBool hasMenuOpen = new NetBool(false);
    [XmlIgnore]
    public readonly Color DEFAULT_SHIRT_COLOR = Color.White;
    [XmlIgnore]
    public readonly Color DEFAULT_PANTS_COLOR = new Color(46, 85, 183);
    public string defaultChatColor;
    public bool catPerson = true;
    public int whichPetBreed;
    [XmlIgnore]
    public bool isAnimatingMount;
    [XmlElement("acceptedDailyQuest")]
    public readonly NetBool acceptedDailyQuest = new NetBool(false);
    [XmlIgnore]
    public Item mostRecentlyGrabbedItem;
    [XmlIgnore]
    public Item itemToEat;
    [XmlElement("farmerRenderer")]
    private readonly NetRef<FarmerRenderer> farmerRenderer = new NetRef<FarmerRenderer>();
    [XmlIgnore]
    public int toolPower;
    [XmlIgnore]
    public int toolHold;
    public Vector2 mostRecentBed;
    public static Dictionary<int, string> hairStyleMetadataFile = (Dictionary<int, string>) null;
    public static List<int> allHairStyleIndices = (List<int>) null;
    public static int lastHairStyle = -1;
    [XmlIgnore]
    public static Dictionary<int, HairStyleMetadata> hairStyleMetadata = new Dictionary<int, HairStyleMetadata>();
    [XmlElement("emoteFavorites")]
    public readonly NetStringList emoteFavorites = new NetStringList();
    [XmlElement("performedEmotes")]
    public readonly NetStringDictionary<bool, NetBool> performedEmotes = new NetStringDictionary<bool, NetBool>();
    [XmlElement("shirt")]
    public readonly NetInt shirt = new NetInt(0);
    [XmlElement("hair")]
    public readonly NetInt hair = new NetInt(0);
    [XmlElement("skin")]
    public readonly NetInt skin = new NetInt(0);
    [XmlElement("shoes")]
    public readonly NetInt shoes = new NetInt(2);
    [XmlElement("accessory")]
    public readonly NetInt accessory = new NetInt(-1);
    [XmlElement("facialHair")]
    public readonly NetInt facialHair = new NetInt(-1);
    [XmlElement("pants")]
    public readonly NetInt pants = new NetInt(0);
    [XmlIgnore]
    public int currentEyes;
    [XmlIgnore]
    public int blinkTimer;
    [XmlIgnore]
    public readonly NetInt netFestivalScore = new NetInt();
    [XmlIgnore]
    public float temporarySpeedBuff;
    [XmlElement("hairstyleColor")]
    public readonly NetColor hairstyleColor = new NetColor(new Color(193, 90, 50));
    [XmlElement("pantsColor")]
    public readonly NetColor pantsColor = new NetColor(new Color(46, 85, 183));
    [XmlElement("newEyeColor")]
    public readonly NetColor newEyeColor = new NetColor(new Color(122, 68, 52));
    [XmlElement("hat")]
    public readonly NetRef<Hat> hat = new NetRef<Hat>();
    [XmlElement("boots")]
    public readonly NetRef<Boots> boots = new NetRef<Boots>();
    [XmlElement("leftRing")]
    public readonly NetRef<Ring> leftRing = new NetRef<Ring>();
    [XmlElement("rightRing")]
    public readonly NetRef<Ring> rightRing = new NetRef<Ring>();
    [XmlElement("shirtItem")]
    public readonly NetRef<Clothing> shirtItem = new NetRef<Clothing>();
    [XmlElement("pantsItem")]
    public readonly NetRef<Clothing> pantsItem = new NetRef<Clothing>();
    [XmlIgnore]
    public readonly NetDancePartner dancePartner = new NetDancePartner();
    [XmlIgnore]
    public bool ridingMineElevator;
    [XmlIgnore]
    public bool mineMovementDirectionWasUp;
    [XmlIgnore]
    public bool cameFromDungeon;
    [XmlIgnore]
    public readonly NetBool exhausted = new NetBool();
    [XmlElement("divorceTonight")]
    public readonly NetBool divorceTonight = new NetBool();
    [XmlElement("changeWalletTypeTonight")]
    public readonly NetBool changeWalletTypeTonight = new NetBool();
    [XmlIgnore]
    public AnimatedSprite.endOfAnimationBehavior toolOverrideFunction;
    [XmlIgnore]
    public NetBool onBridge = new NetBool();
    [XmlIgnore]
    public SuspensionBridge bridge;
    private readonly NetInt netDeepestMineLevel = new NetInt();
    [XmlElement("currentToolIndex")]
    private readonly NetInt currentToolIndex = new NetInt(0);
    [XmlIgnore]
    private readonly NetRef<Item> temporaryItem = new NetRef<Item>();
    [XmlIgnore]
    private readonly NetRef<Item> cursorSlotItem = new NetRef<Item>();
    [XmlIgnore]
    public readonly NetBool netItemStowed = new NetBool(false);
    protected bool _itemStowed;
    public int woodPieces;
    public int stonePieces;
    public int copperPieces;
    public int ironPieces;
    public int coalPieces;
    public int goldPieces;
    public int iridiumPieces;
    public int quartzPieces;
    public string gameVersion = "-1";
    public string gameVersionLabel;
    [XmlIgnore]
    public bool isFakeEventActor;
    [XmlElement("caveChoice")]
    public readonly NetInt caveChoice = new NetInt();
    public int feed;
    [XmlElement("farmingLevel")]
    public readonly NetInt farmingLevel = new NetInt();
    [XmlElement("miningLevel")]
    public readonly NetInt miningLevel = new NetInt();
    [XmlElement("combatLevel")]
    public readonly NetInt combatLevel = new NetInt();
    [XmlElement("foragingLevel")]
    public readonly NetInt foragingLevel = new NetInt();
    [XmlElement("fishingLevel")]
    public readonly NetInt fishingLevel = new NetInt();
    [XmlElement("luckLevel")]
    public readonly NetInt luckLevel = new NetInt();
    [XmlElement("newSkillPointsToSpend")]
    public readonly NetInt newSkillPointsToSpend = new NetInt();
    [XmlElement("addedFarmingLevel")]
    public readonly NetInt addedFarmingLevel = new NetInt();
    [XmlElement("addedMiningLevel")]
    public readonly NetInt addedMiningLevel = new NetInt();
    [XmlElement("addedCombatLevel")]
    public readonly NetInt addedCombatLevel = new NetInt();
    [XmlElement("addedForagingLevel")]
    public readonly NetInt addedForagingLevel = new NetInt();
    [XmlElement("addedFishingLevel")]
    public readonly NetInt addedFishingLevel = new NetInt();
    [XmlElement("addedLuckLevel")]
    public readonly NetInt addedLuckLevel = new NetInt();
    [XmlElement("maxStamina")]
    public readonly NetInt maxStamina = new NetInt(270);
    [XmlElement("maxItems")]
    public readonly NetInt maxItems = new NetInt(12);
    [XmlElement("lastSeenMovieWeek")]
    public readonly NetInt lastSeenMovieWeek = new NetInt(-1);
    [XmlIgnore]
    public readonly NetString viewingLocation = new NetString((string) null);
    private readonly NetFloat netStamina = new NetFloat(270f);
    public int resilience;
    public int attack;
    public int immunity;
    public float attackIncreaseModifier;
    public float knockbackModifier;
    public float weaponSpeedModifier;
    public float critChanceModifier;
    public float critPowerModifier;
    public float weaponPrecisionModifier;
    [XmlIgnore]
    public NetRoot<FarmerTeam> teamRoot = new NetRoot<FarmerTeam>(new FarmerTeam());
    public int clubCoins;
    public int trashCanLevel;
    private NetLong netMillisecondsPlayed = new NetLong();
    [XmlElement("toolBeingUpgraded")]
    public readonly NetRef<Tool> toolBeingUpgraded = new NetRef<Tool>();
    [XmlElement("daysLeftForToolUpgrade")]
    public readonly NetInt daysLeftForToolUpgrade = new NetInt();
    /// <summary>//////////////////////////////</summary>
    [XmlIgnore]
    private float timeOfLastPositionPacket;
    private int numUpdatesSinceLastDraw;
    [XmlElement("houseUpgradeLevel")]
    public readonly NetInt houseUpgradeLevel = new NetInt(0);
    [XmlElement("daysUntilHouseUpgrade")]
    public readonly NetInt daysUntilHouseUpgrade = new NetInt(-1);
    public int coopUpgradeLevel;
    public int barnUpgradeLevel;
    public bool hasGreenhouse;
    public bool hasUnlockedSkullDoor;
    public bool hasDarkTalisman;
    public bool hasMagicInk;
    public bool showChestColorPicker = true;
    public bool hasMagnifyingGlass;
    public bool hasWateringCanEnchantment;
    [XmlIgnore]
    public List<BaseEnchantment> enchantments = new List<BaseEnchantment>();
    protected NetBool hasTownKey = new NetBool(false);
    [XmlElement("magneticRadius")]
    public readonly NetInt magneticRadius = new NetInt(128);
    public int temporaryInvincibilityTimer;
    public int currentTemporaryInvincibilityDuration = 1200;
    [XmlIgnore]
    public float rotation;
    private int craftingTime = 1000;
    private int raftPuddleCounter = 250;
    private int raftBobCounter = 1000;
    public int health = 100;
    public int maxHealth = 100;
    private readonly NetInt netTimesReachedMineBottom = new NetInt(0);
    public float difficultyModifier = 1f;
    [XmlIgnore]
    public Vector2 jitter = Vector2.Zero;
    [XmlIgnore]
    public Vector2 lastPosition;
    [XmlIgnore]
    public Vector2 lastGrabTile = Vector2.Zero;
    [XmlIgnore]
    public float jitterStrength;
    [XmlIgnore]
    public float xOffset;
    [XmlElement("isMale")]
    public readonly NetBool isMale = new NetBool(true);
    [XmlIgnore]
    public bool canMove = true;
    [XmlIgnore]
    public bool running;
    [XmlIgnore]
    public bool ignoreCollisions;
    [XmlIgnore]
    public readonly NetBool usingTool = new NetBool(false);
    [XmlIgnore]
    public bool isEating;
    [XmlIgnore]
    public readonly NetBool isInBed = new NetBool(false);
    [XmlIgnore]
    public bool forceTimePass;
    [XmlIgnore]
    public bool isRafting;
    [XmlIgnore]
    public bool usingSlingshot;
    [XmlIgnore]
    public readonly NetBool bathingClothes = new NetBool(false);
    [XmlIgnore]
    public bool canOnlyWalk;
    [XmlIgnore]
    public bool temporarilyInvincible;
    public bool hasBusTicket;
    public bool stardewHero;
    public bool hasClubCard;
    public bool hasSpecialCharm;
    [XmlIgnore]
    public bool canReleaseTool;
    [XmlIgnore]
    public bool isCrafting;
    [XmlIgnore]
    public bool isEmoteAnimating;
    [XmlIgnore]
    public bool passedOut;
    [XmlIgnore]
    public bool hasNutPickupQueued;
    [XmlIgnore]
    protected int _emoteGracePeriod;
    [XmlIgnore]
    private BoundingBoxGroup temporaryPassableTiles = new BoundingBoxGroup();
    [XmlIgnore]
    public readonly NetBool hidden = new NetBool();
    [XmlElement("basicShipped")]
    public readonly NetIntDictionary<int, NetInt> basicShipped = new NetIntDictionary<int, NetInt>();
    [XmlElement("mineralsFound")]
    public readonly NetIntDictionary<int, NetInt> mineralsFound = new NetIntDictionary<int, NetInt>();
    [XmlElement("recipesCooked")]
    public readonly NetIntDictionary<int, NetInt> recipesCooked = new NetIntDictionary<int, NetInt>();
    [XmlElement("fishCaught")]
    public readonly NetIntIntArrayDictionary fishCaught = new NetIntIntArrayDictionary();
    [XmlElement("archaeologyFound")]
    public readonly NetIntIntArrayDictionary archaeologyFound = new NetIntIntArrayDictionary();
    [XmlElement("callsReceived")]
    public readonly NetIntDictionary<int, NetInt> callsReceived = new NetIntDictionary<int, NetInt>();
    public SerializableDictionary<string, SerializableDictionary<int, int>> giftedItems;
    [XmlElement("tailoredItems")]
    public readonly NetStringDictionary<int, NetInt> tailoredItems = new NetStringDictionary<int, NetInt>();
    public SerializableDictionary<string, int[]> friendships;
    [XmlElement("friendshipData")]
    public readonly NetStringDictionary<Friendship, NetRef<Friendship>> friendshipData = new NetStringDictionary<Friendship, NetRef<Friendship>>();
    [XmlIgnore]
    public NetString locationBeforeForcedEvent = new NetString((string) null);
    [XmlIgnore]
    public Vector2 positionBeforeEvent;
    [XmlIgnore]
    public int orientationBeforeEvent;
    [XmlIgnore]
    public int swimTimer;
    [XmlIgnore]
    public int regenTimer;
    [XmlIgnore]
    public int timerSinceLastMovement;
    [XmlIgnore]
    public int noMovementPause;
    [XmlIgnore]
    public int freezePause;
    [XmlIgnore]
    public float yOffset;
    public BuildingUpgrade currentUpgrade;
    [XmlElement("spouse")]
    protected readonly NetString netSpouse = new NetString();
    public string dateStringForSaveGame;
    public int? dayOfMonthForSaveGame;
    public int? seasonForSaveGame;
    public int? yearForSaveGame;
    public int overallsColor;
    public int shirtColor;
    public int skinColor;
    public int hairColor;
    public int eyeColor;
    [XmlIgnore]
    public Vector2 armOffset;
    public string bobber = "";
    private readonly NetRef<Horse> netMount = new NetRef<Horse>();
    [XmlIgnore]
    public ISittable sittingFurniture;
    [XmlIgnore]
    public NetBool isSitting = new NetBool();
    [XmlIgnore]
    public NetVector2 mapChairSitPosition = new NetVector2(new Vector2(-1f, -1f));
    [XmlIgnore]
    public NetBool hasCompletedAllMonsterSlayerQuests = new NetBool(false);
    [XmlIgnore]
    public bool isStopSitting;
    [XmlIgnore]
    protected bool _wasSitting;
    [XmlIgnore]
    public Vector2 lerpStartPosition;
    [XmlIgnore]
    public Vector2 lerpEndPosition;
    [XmlIgnore]
    public float lerpPosition = -1f;
    [XmlIgnore]
    public float lerpDuration = -1f;
    [XmlIgnore]
    protected Item _lastSelectedItem;
    [XmlElement("qiGems")]
    public NetIntDelta netQiGems = new NetIntDelta();
    [XmlElement("JOTPKProgress")]
    public NetRef<AbigailGame.JOTPKProgress> jotpkProgress = new NetRef<AbigailGame.JOTPKProgress>();
    [XmlElement("hasUsedDailyRevive")]
    public NetBool hasUsedDailyRevive = new NetBool(false);
    private readonly NetEvent0 fireToolEvent = new NetEvent0(true);
    private readonly NetEvent0 beginUsingToolEvent = new NetEvent0(true);
    private readonly NetEvent0 endUsingToolEvent = new NetEvent0(true);
    private readonly NetEvent0 sickAnimationEvent = new NetEvent0();
    private readonly NetEvent0 passOutEvent = new NetEvent0();
    private readonly NetEvent0 haltAnimationEvent = new NetEvent0();
    private readonly NetEvent1Field<Object, NetRef<Object>> drinkAnimationEvent = new NetEvent1Field<Object, NetRef<Object>>();
    private readonly NetEvent1Field<Object, NetRef<Object>> eatAnimationEvent = new NetEvent1Field<Object, NetRef<Object>>();
    private readonly NetEvent1Field<string, NetString> doEmoteEvent = new NetEvent1Field<string, NetString>();
    private readonly NetEvent1Field<long, NetLong> kissFarmerEvent = new NetEvent1Field<long, NetLong>();
    private readonly NetEvent1Field<float, NetFloat> synchronizedJumpEvent = new NetEvent1Field<float, NetFloat>();
    public readonly NetEvent1Field<string, NetString> renovateEvent = new NetEvent1Field<string, NetString>();
    [XmlElement("chestConsumedLevels")]
    public readonly NetIntDictionary<bool, NetBool> chestConsumedMineLevels = new NetIntDictionary<bool, NetBool>();
    public int saveTime;
    [XmlIgnore]
    public float drawLayerDisambiguator;
    [XmlElement("isCustomized")]
    public readonly NetBool isCustomized = new NetBool(false);
    [XmlElement("homeLocation")]
    public readonly NetString homeLocation = new NetString("FarmHouse");
    [XmlElement("lastSleepLocation")]
    public readonly NetString lastSleepLocation = new NetString();
    [XmlElement("lastSleepPoint")]
    public readonly NetPoint lastSleepPoint = new NetPoint();
    public static readonly Farmer.EmoteType[] EMOTES;
    [XmlIgnore]
    public int emoteFacingDirection = 2;
    public int daysMarried;
    private int toolPitchAccumulator;
    private int charactercollisionTimer;
    private NPC collisionNPC;
    public float movementMultiplier = 0.01f;

    public int visibleQuestCount
    {
      get
      {
        int visibleQuestCount = 0;
        foreach (SpecialOrder specialOrder in this.team.specialOrders)
        {
          if (!specialOrder.IsHidden())
            ++visibleQuestCount;
        }
        foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) this.questLog)
        {
          if (!quest.IsHidden())
            ++visibleQuestCount;
        }
        return visibleQuestCount;
      }
    }

    public Item recoveredItem
    {
      get => this._recoveredItem.Value;
      set => this._recoveredItem.Value = value;
    }

    [XmlElement("theaterBuildDate")]
    public long theaterBuildDate
    {
      get => (long) this.teamRoot.Value.theaterBuildDate;
      set => this.teamRoot.Value.theaterBuildDate.Value = value;
    }

    [XmlIgnore]
    public int festivalScore
    {
      get => (int) (NetFieldBase<int, NetInt>) this.netFestivalScore;
      set
      {
        if (Game1.player != null && Game1.player.team != null && Game1.player.team.festivalScoreStatus != null)
          Game1.player.team.festivalScoreStatus.UpdateState(Game1.player.festivalScore.ToString() ?? "");
        this.netFestivalScore.Value = value;
      }
    }

    public int deepestMineLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.netDeepestMineLevel;
      set => this.netDeepestMineLevel.Value = value;
    }

    public float stamina
    {
      get => (float) (NetFieldBase<float, NetFloat>) this.netStamina;
      set => this.netStamina.Value = value;
    }

    [XmlIgnore]
    public FarmerTeam team => Game1.player != null && this != Game1.player ? Game1.player.team : this.teamRoot.Value;

    public uint totalMoneyEarned
    {
      get => (uint) this.teamRoot.Value.totalMoneyEarned.Value;
      set
      {
        if (this.teamRoot.Value.totalMoneyEarned.Value != 0)
        {
          if (value >= 15000U && this.teamRoot.Value.totalMoneyEarned.Value < 15000)
            Game1.multiplayer.globalChatInfoMessage("Earned15k", (string) (NetFieldBase<string, NetString>) this.farmName);
          if (value >= 50000U && this.teamRoot.Value.totalMoneyEarned.Value < 50000)
            Game1.multiplayer.globalChatInfoMessage("Earned50k", (string) (NetFieldBase<string, NetString>) this.farmName);
          if (value >= 250000U && this.teamRoot.Value.totalMoneyEarned.Value < 250000)
            Game1.multiplayer.globalChatInfoMessage("Earned250k", (string) (NetFieldBase<string, NetString>) this.farmName);
          if (value >= 1000000U && this.teamRoot.Value.totalMoneyEarned.Value < 1000000)
            Game1.multiplayer.globalChatInfoMessage("Earned1m", (string) (NetFieldBase<string, NetString>) this.farmName);
          if (value >= 10000000U && this.teamRoot.Value.totalMoneyEarned.Value < 10000000)
            Game1.multiplayer.globalChatInfoMessage("Earned10m", (string) (NetFieldBase<string, NetString>) this.farmName);
          if (value >= 100000000U && this.teamRoot.Value.totalMoneyEarned.Value < 100000000)
            Game1.multiplayer.globalChatInfoMessage("Earned100m", (string) (NetFieldBase<string, NetString>) this.farmName);
        }
        this.teamRoot.Value.totalMoneyEarned.Value = (int) value;
      }
    }

    public ulong millisecondsPlayed
    {
      get => (ulong) this.netMillisecondsPlayed.Value;
      set => this.netMillisecondsPlayed.Value = (long) value;
    }

    public bool hasRustyKey
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.teamRoot.Value.hasRustyKey;
      set => this.teamRoot.Value.hasRustyKey.Value = value;
    }

    public bool hasSkullKey
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.teamRoot.Value.hasSkullKey;
      set => this.teamRoot.Value.hasSkullKey.Value = value;
    }

    public bool canUnderstandDwarves
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.teamRoot.Value.canUnderstandDwarves;
      set => this.teamRoot.Value.canUnderstandDwarves.Value = value;
    }

    public bool HasTownKey
    {
      get => this.hasTownKey.Value;
      set => this.hasTownKey.Value = value;
    }

    [XmlIgnore]
    public bool hasPendingCompletedQuests
    {
      get
      {
        foreach (SpecialOrder specialOrder in this.team.specialOrders)
        {
          if (specialOrder.participants.ContainsKey(this.UniqueMultiplayerID) && specialOrder.ShouldDisplayAsComplete())
            return true;
        }
        foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) this.questLog)
        {
          if (!quest.IsHidden() && quest.ShouldDisplayAsComplete() && !quest.destroy.Value)
            return true;
        }
        return false;
      }
    }

    [XmlElement("useSeparateWallets")]
    public bool useSeparateWallets
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.teamRoot.Value.useSeparateWallets;
      set => this.teamRoot.Value.useSeparateWallets.Value = value;
    }

    public int timesReachedMineBottom
    {
      get => (int) (NetFieldBase<int, NetInt>) this.netTimesReachedMineBottom;
      set => this.netTimesReachedMineBottom.Value = value;
    }

    public string spouse
    {
      get => this.netSpouse.Value != null && this.netSpouse.Value.Length != 0 ? this.netSpouse.Value : (string) null;
      set
      {
        if (value == null)
          this.netSpouse.Value = "";
        else
          this.netSpouse.Value = value;
      }
    }

    [XmlIgnore]
    public bool isUnclaimedFarmhand => !this.IsMainPlayer && !(bool) (NetFieldBase<bool, NetBool>) this.isCustomized;

    [XmlIgnore]
    public Horse mount
    {
      get => this.netMount.Value;
      set => this.setMount(value);
    }

    [XmlIgnore]
    public int MaxItems
    {
      get => (int) (NetFieldBase<int, NetInt>) this.maxItems;
      set => this.maxItems.Value = value;
    }

    [XmlIgnore]
    public int Level => ((int) (NetFieldBase<int, NetInt>) this.farmingLevel + (int) (NetFieldBase<int, NetInt>) this.fishingLevel + (int) (NetFieldBase<int, NetInt>) this.foragingLevel + (int) (NetFieldBase<int, NetInt>) this.combatLevel + (int) (NetFieldBase<int, NetInt>) this.miningLevel + (int) (NetFieldBase<int, NetInt>) this.luckLevel) / 2;

    [XmlIgnore]
    public int CraftingTime
    {
      get => this.craftingTime;
      set => this.craftingTime = value;
    }

    [XmlIgnore]
    public int NewSkillPointsToSpend
    {
      get => (int) (NetFieldBase<int, NetInt>) this.newSkillPointsToSpend;
      set => this.newSkillPointsToSpend.Value = value;
    }

    [XmlIgnore]
    public int FarmingLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.farmingLevel + (int) (NetFieldBase<int, NetInt>) this.addedFarmingLevel;
      set => this.farmingLevel.Value = value;
    }

    [XmlIgnore]
    public int MiningLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.miningLevel + (int) (NetFieldBase<int, NetInt>) this.addedMiningLevel;
      set => this.miningLevel.Value = value;
    }

    [XmlIgnore]
    public int CombatLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.combatLevel + (int) (NetFieldBase<int, NetInt>) this.addedCombatLevel;
      set => this.combatLevel.Value = value;
    }

    [XmlIgnore]
    public int ForagingLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.foragingLevel + (int) (NetFieldBase<int, NetInt>) this.addedForagingLevel;
      set => this.foragingLevel.Value = value;
    }

    [XmlIgnore]
    public int FishingLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.fishingLevel + (int) (NetFieldBase<int, NetInt>) this.addedFishingLevel + (this.CurrentTool == null || !this.CurrentTool.hasEnchantmentOfType<MasterEnchantment>() ? 0 : 1);
      set => this.fishingLevel.Value = value;
    }

    [XmlIgnore]
    public int LuckLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.luckLevel + (int) (NetFieldBase<int, NetInt>) this.addedLuckLevel;
      set => this.luckLevel.Value = value;
    }

    [XmlIgnore]
    public double DailyLuck => this.team.sharedDailyLuck.Value + (this.hasSpecialCharm ? 0.025000000372529 : 0.0);

    [XmlIgnore]
    public int HouseUpgradeLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.houseUpgradeLevel;
      set => this.houseUpgradeLevel.Value = value;
    }

    [XmlIgnore]
    public int CoopUpgradeLevel
    {
      get => this.coopUpgradeLevel;
      set => this.coopUpgradeLevel = value;
    }

    [XmlIgnore]
    public int BarnUpgradeLevel
    {
      get => this.barnUpgradeLevel;
      set => this.barnUpgradeLevel = value;
    }

    [XmlIgnore]
    public BoundingBoxGroup TemporaryPassableTiles
    {
      get => this.temporaryPassableTiles;
      set => this.temporaryPassableTiles = value;
    }

    [XmlIgnore]
    public IList<Item> Items
    {
      get => (IList<Item>) this.items;
      set => this.items.CopyFrom(value);
    }

    [XmlIgnore]
    public int MagneticRadius
    {
      get => this.magneticRadius.Value;
      set => this.magneticRadius.Value = value;
    }

    [XmlIgnore]
    public Object ActiveObject
    {
      get
      {
        if (this.TemporaryItem != null)
          return this.TemporaryItem is Object ? (Object) this.TemporaryItem : (Object) null;
        if (this._itemStowed)
          return (Object) null;
        return (int) (NetFieldBase<int, NetInt>) this.currentToolIndex < this.items.Count && this.items[(int) (NetFieldBase<int, NetInt>) this.currentToolIndex] != null && this.items[(int) (NetFieldBase<int, NetInt>) this.currentToolIndex] is Object ? (Object) this.items[(int) (NetFieldBase<int, NetInt>) this.currentToolIndex] : (Object) null;
      }
      set
      {
        this.netItemStowed.Set(false);
        if (value == null)
          this.removeItemFromInventory((Item) this.ActiveObject);
        else
          this.addItemToInventory((Item) value, this.CurrentToolIndex);
      }
    }

    [XmlIgnore]
    public bool IsMale
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isMale;
      set => this.isMale.Set(value);
    }

    [XmlIgnore]
    public IList<int> DialogueQuestionsAnswered => (IList<int>) this.dialogueQuestionsAnswered;

    [XmlIgnore]
    public int WoodPieces
    {
      get => this.woodPieces;
      set => this.woodPieces = value;
    }

    [XmlIgnore]
    public int StonePieces
    {
      get => this.stonePieces;
      set => this.stonePieces = value;
    }

    [XmlIgnore]
    public int CopperPieces
    {
      get => this.copperPieces;
      set => this.copperPieces = value;
    }

    [XmlIgnore]
    public int IronPieces
    {
      get => this.ironPieces;
      set => this.ironPieces = value;
    }

    [XmlIgnore]
    public int CoalPieces
    {
      get => this.coalPieces;
      set => this.coalPieces = value;
    }

    [XmlIgnore]
    public int GoldPieces
    {
      get => this.goldPieces;
      set => this.goldPieces = value;
    }

    [XmlIgnore]
    public int IridiumPieces
    {
      get => this.iridiumPieces;
      set => this.iridiumPieces = value;
    }

    [XmlIgnore]
    public int QuartzPieces
    {
      get => this.quartzPieces;
      set => this.quartzPieces = value;
    }

    [XmlIgnore]
    public int Feed
    {
      get => this.feed;
      set => this.feed = value;
    }

    [XmlIgnore]
    public bool CanMove
    {
      get => this.canMove;
      set => this.canMove = value;
    }

    [XmlIgnore]
    public bool UsingTool
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.usingTool;
      set => this.usingTool.Set(value);
    }

    [XmlIgnore]
    public Tool CurrentTool
    {
      get => this.CurrentItem != null && this.CurrentItem is Tool ? (Tool) this.CurrentItem : (Tool) null;
      set
      {
        while (this.CurrentToolIndex >= this.items.Count)
          this.items.Add((Item) null);
        this.items[this.CurrentToolIndex] = (Item) value;
      }
    }

    [XmlIgnore]
    public Item TemporaryItem
    {
      get => this.temporaryItem.Value;
      set => this.temporaryItem.Value = value;
    }

    public Item CursorSlotItem
    {
      get => this.cursorSlotItem.Value;
      set => this.cursorSlotItem.Value = value;
    }

    [XmlIgnore]
    public Item CurrentItem
    {
      get
      {
        if (this.TemporaryItem != null)
          return this.TemporaryItem;
        if (this._itemStowed)
          return (Item) null;
        return (int) (NetFieldBase<int, NetInt>) this.currentToolIndex >= this.items.Count ? (Item) null : this.items[(int) (NetFieldBase<int, NetInt>) this.currentToolIndex];
      }
    }

    [XmlIgnore]
    public int CurrentToolIndex
    {
      get => (int) (NetFieldBase<int, NetInt>) this.currentToolIndex;
      set
      {
        this.netItemStowed.Set(false);
        if ((int) (NetFieldBase<int, NetInt>) this.currentToolIndex >= 0 && this.CurrentItem != null && value != (int) (NetFieldBase<int, NetInt>) this.currentToolIndex)
          this.CurrentItem.actionWhenStopBeingHeld(this);
        this.currentToolIndex.Set(value);
      }
    }

    [XmlIgnore]
    public float Stamina
    {
      get => this.stamina;
      set => this.stamina = Math.Min((float) (int) (NetFieldBase<int, NetInt>) this.maxStamina, Math.Max(value, -16f));
    }

    [XmlIgnore]
    public int MaxStamina
    {
      get => (int) (NetFieldBase<int, NetInt>) this.maxStamina;
      set => this.maxStamina.Value = value;
    }

    public long UniqueMultiplayerID
    {
      get => (long) this.uniqueMultiplayerID;
      set => this.uniqueMultiplayerID.Value = value;
    }

    [XmlIgnore]
    public bool IsLocalPlayer
    {
      get
      {
        if (this.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
          return true;
        return Game1.CurrentEvent != null && Game1.CurrentEvent.farmer == this;
      }
    }

    [XmlIgnore]
    public bool IsMainPlayer
    {
      get
      {
        if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null && this.IsLocalPlayer)
          return true;
        return (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null && this.UniqueMultiplayerID == Game1.serverHost.Value.UniqueMultiplayerID;
      }
    }

    [XmlIgnore]
    public override AnimatedSprite Sprite
    {
      get => base.Sprite;
      set
      {
        base.Sprite = value;
        if (base.Sprite == null)
          return;
        (base.Sprite as FarmerSprite).setOwner(this);
      }
    }

    [XmlIgnore]
    public FarmerSprite FarmerSprite
    {
      get => (FarmerSprite) this.Sprite;
      set => this.Sprite = (AnimatedSprite) value;
    }

    [XmlIgnore]
    public FarmerRenderer FarmerRenderer
    {
      get => (FarmerRenderer) (NetFieldBase<FarmerRenderer, NetRef<FarmerRenderer>>) this.farmerRenderer;
      set => this.farmerRenderer.Set(value);
    }

    [XmlElement("money")]
    public int _money
    {
      get => (int) (NetFieldBase<int, NetIntDelta>) this.teamRoot.Value.GetMoney(this);
      set => this.teamRoot.Value.GetMoney(this).Value = value;
    }

    [XmlIgnore]
    public int QiGems
    {
      get => this.netQiGems.Value;
      set => this.netQiGems.Value = value;
    }

    [XmlIgnore]
    public int Money
    {
      get => this._money;
      set
      {
        if (Game1.player != this)
          throw new Exception("Cannot change another farmer's money. Use Game1.player.team.SetIndividualMoney");
        int money = this._money;
        this._money = value;
        if (value <= money)
          return;
        uint num = (uint) (value - money);
        this.totalMoneyEarned += num;
        if (Game1.player.useSeparateWallets)
          this.stats.IndividualMoneyEarned += num;
        Game1.stats.checkForMoneyAchievements();
      }
    }

    public void addUnearnedMoney(int money) => this._money += money;

    public NetStringList GetEmoteFavorites()
    {
      if (this.emoteFavorites.Count == 0)
      {
        this.emoteFavorites.Add("question");
        this.emoteFavorites.Add("heart");
        this.emoteFavorites.Add("yes");
        this.emoteFavorites.Add("happy");
        this.emoteFavorites.Add("pause");
        this.emoteFavorites.Add("sad");
        this.emoteFavorites.Add("no");
        this.emoteFavorites.Add("angry");
      }
      return this.emoteFavorites;
    }

    public Farmer()
    {
      this.farmerInit();
      this.Sprite = (AnimatedSprite) new FarmerSprite((string) null);
    }

    public Farmer(
      FarmerSprite sprite,
      Vector2 position,
      int speed,
      string name,
      List<Item> initialTools,
      bool isMale)
      : base((AnimatedSprite) sprite, position, speed, name)
    {
      this.farmerInit();
      this.Name = name;
      this.displayName = name;
      this.IsMale = isMale;
      this.stamina = (float) (int) (NetFieldBase<int, NetInt>) this.maxStamina;
      this.items.CopyFrom((IList<Item>) initialTools);
      for (int count = this.items.Count; count < (int) (NetFieldBase<int, NetInt>) this.maxItems; ++count)
        this.items.Add((Item) null);
      this.activeDialogueEvents.Add("Introduction", 6);
      if (this.currentLocation != null)
        this.mostRecentBed = Utility.PointToVector2((this.currentLocation as FarmHouse).GetPlayerBedSpot()) * 64f;
      else
        this.mostRecentBed = new Vector2(9f, 9f) * 64f;
    }

    private void farmerInit()
    {
      this.NetFields.AddFields((INetSerializable) this.uniqueMultiplayerID, (INetSerializable) this.userID, (INetSerializable) this.platformType, (INetSerializable) this.platformID, (INetSerializable) this.farmerRenderer, (INetSerializable) this.isMale, (INetSerializable) this.bathingClothes, (INetSerializable) this.shirt, (INetSerializable) this.pants, (INetSerializable) this.hair, (INetSerializable) this.skin, (INetSerializable) this.shoes, (INetSerializable) this.accessory, (INetSerializable) this.facialHair, (INetSerializable) this.hairstyleColor, (INetSerializable) this.pantsColor, (INetSerializable) this.newEyeColor, (INetSerializable) this.items, (INetSerializable) this.currentToolIndex, (INetSerializable) this.temporaryItem, (INetSerializable) this.cursorSlotItem, (INetSerializable) this.fireToolEvent, (INetSerializable) this.beginUsingToolEvent, (INetSerializable) this.endUsingToolEvent, (INetSerializable) this.hat, (INetSerializable) this.boots, (INetSerializable) this.leftRing, (INetSerializable) this.rightRing, (INetSerializable) this.hidden, (INetSerializable) this.isInBed, (INetSerializable) this.caveChoice, (INetSerializable) this.houseUpgradeLevel, (INetSerializable) this.daysUntilHouseUpgrade, (INetSerializable) this.magneticRadius, (INetSerializable) this.netSpouse, (INetSerializable) this.mailReceived, (INetSerializable) this.mailForTomorrow, (INetSerializable) this.mailbox, (INetSerializable) this.eventsSeen, (INetSerializable) this.secretNotesSeen, this.netMount.NetFields, (INetSerializable) this.dancePartner.NetFields, (INetSerializable) this.divorceTonight, (INetSerializable) this.isCustomized, (INetSerializable) this.homeLocation, (INetSerializable) this.farmName, (INetSerializable) this.favoriteThing, (INetSerializable) this.horseName, (INetSerializable) this.netMillisecondsPlayed, (INetSerializable) this.netFestivalScore, (INetSerializable) this.friendshipData, (INetSerializable) this.drinkAnimationEvent, (INetSerializable) this.eatAnimationEvent, (INetSerializable) this.sickAnimationEvent, (INetSerializable) this.passOutEvent, (INetSerializable) this.doEmoteEvent, (INetSerializable) this.questLog, (INetSerializable) this.professions, (INetSerializable) this.newLevels, (INetSerializable) this.experiencePoints, (INetSerializable) this.dialogueQuestionsAnswered, (INetSerializable) this.cookingRecipes, (INetSerializable) this.craftingRecipes, (INetSerializable) this.activeDialogueEvents, (INetSerializable) this.achievements, (INetSerializable) this.specialItems, (INetSerializable) this.specialBigCraftables, (INetSerializable) this.farmingLevel, (INetSerializable) this.miningLevel, (INetSerializable) this.combatLevel, (INetSerializable) this.foragingLevel, (INetSerializable) this.fishingLevel, (INetSerializable) this.luckLevel, (INetSerializable) this.newSkillPointsToSpend, (INetSerializable) this.addedFarmingLevel, (INetSerializable) this.addedMiningLevel, (INetSerializable) this.addedCombatLevel, (INetSerializable) this.addedForagingLevel, (INetSerializable) this.addedFishingLevel, (INetSerializable) this.addedLuckLevel, (INetSerializable) this.maxStamina, (INetSerializable) this.netStamina, (INetSerializable) this.maxItems, (INetSerializable) this.chestConsumedMineLevels, (INetSerializable) this.toolBeingUpgraded, (INetSerializable) this.daysLeftForToolUpgrade, (INetSerializable) this.exhausted, (INetSerializable) this.appliedBuffs, (INetSerializable) this.netDeepestMineLevel, (INetSerializable) this.netTimesReachedMineBottom, (INetSerializable) this.netItemStowed, (INetSerializable) this.acceptedDailyQuest, (INetSerializable) this.lastSeenMovieWeek, (INetSerializable) this.shirtItem, (INetSerializable) this.pantsItem, (INetSerializable) this.personalShippingBin, (INetSerializable) this.viewingLocation, (INetSerializable) this.kissFarmerEvent, (INetSerializable) this.haltAnimationEvent, (INetSerializable) this.synchronizedJumpEvent, (INetSerializable) this.tailoredItems, (INetSerializable) this.basicShipped, (INetSerializable) this.mineralsFound, (INetSerializable) this.recipesCooked, (INetSerializable) this.archaeologyFound, (INetSerializable) this.fishCaught, (INetSerializable) this._recoveredItem, (INetSerializable) this.itemsLostLastDeath, (INetSerializable) this.renovateEvent, (INetSerializable) this.callsReceived, (INetSerializable) this.onBridge, (INetSerializable) this.lastSleepLocation, (INetSerializable) this.lastSleepPoint, (INetSerializable) this.sleptInTemporaryBed, (INetSerializable) this.timeWentToBed, (INetSerializable) this.hasUsedDailyRevive, (INetSerializable) this.jotpkProgress, (INetSerializable) this.requestingTimePause, (INetSerializable) this.isSitting, (INetSerializable) this.mapChairSitPosition, (INetSerializable) this.netQiGems, (INetSerializable) this.locationBeforeForcedEvent, (INetSerializable) this.appliedSpecialBuffs, (INetSerializable) this.hasTownKey, (INetSerializable) this.hasCompletedAllMonsterSlayerQuests);
      this.requestingTimePause.InterpolationWait = false;
      if (this.Sprite != null)
        this.FarmerSprite.setOwner(this);
      this.netQiGems.Minimum = new int?(0);
      this.netMillisecondsPlayed.DeltaAggregateTicks = (ushort) 60;
      this.fireToolEvent.onEvent += new NetEvent0.Event(this.performFireTool);
      this.beginUsingToolEvent.onEvent += new NetEvent0.Event(this.performBeginUsingTool);
      this.endUsingToolEvent.onEvent += new NetEvent0.Event(this.performEndUsingTool);
      this.drinkAnimationEvent.onEvent += new AbstractNetEvent1<Object>.Event(this.performDrinkAnimation);
      this.eatAnimationEvent.onEvent += new AbstractNetEvent1<Object>.Event(this.performEatAnimation);
      this.sickAnimationEvent.onEvent += new NetEvent0.Event(this.performSickAnimation);
      this.passOutEvent.onEvent += new NetEvent0.Event(this.performPassOut);
      this.doEmoteEvent.onEvent += new AbstractNetEvent1<string>.Event(this.performPlayerEmote);
      this.kissFarmerEvent.onEvent += new AbstractNetEvent1<long>.Event(this.performKissFarmer);
      this.haltAnimationEvent.onEvent += new NetEvent0.Event(this.performHaltAnimation);
      this.synchronizedJumpEvent.onEvent += new AbstractNetEvent1<float>.Event(this.performSynchronizedJump);
      this.renovateEvent.onEvent += new AbstractNetEvent1<string>.Event(this.performRenovation);
      this.FarmerRenderer = new FarmerRenderer("Characters\\Farmer\\farmer_" + (this.IsMale ? "" : "girl_") + "base", this);
      this.currentLocation = Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) this.homeLocation);
      this.items.Clear();
      this.giftedItems = new SerializableDictionary<string, SerializableDictionary<int, int>>();
      this.craftingRecipes.Add("Chest", 0);
      this.craftingRecipes.Add("Wood Fence", 0);
      this.craftingRecipes.Add("Gate", 0);
      this.craftingRecipes.Add("Torch", 0);
      this.craftingRecipes.Add("Campfire", 0);
      this.craftingRecipes.Add("Wood Path", 0);
      this.craftingRecipes.Add("Cobblestone Path", 0);
      this.craftingRecipes.Add("Gravel Path", 0);
      this.craftingRecipes.Add("Wood Sign", 0);
      this.craftingRecipes.Add("Stone Sign", 0);
      this.cookingRecipes.Add("Fried Egg", 0);
      this.songsHeard.Add("title_day");
      this.songsHeard.Add("title_night");
      this.changeShirt(0);
      this.changeSkinColor(0);
      this.changeShoeColor(2);
      this.shirtItem.fieldChangeVisibleEvent += (NetFieldBase<Clothing, NetRef<Clothing>>.FieldChange) ((field, old_value, new_value) => this.UpdateClothing());
      this.pantsItem.fieldChangeVisibleEvent += (NetFieldBase<Clothing, NetRef<Clothing>>.FieldChange) ((field, old_value, new_value) => this.UpdateClothing());
      this.farmName.FilterStringEvent += new NetString.FilterString(Utility.FilterDirtyWords);
      this.name.FilterStringEvent += new NetString.FilterString(Utility.FilterDirtyWords);
    }

    public bool CanEmote() => Game1.farmEvent == null && (!Game1.eventUp || Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence || !this.IsLocalPlayer) && !this.usingSlingshot && !this.isEating && !this.UsingTool && (this.CanMove || !this.IsLocalPlayer) && !this.IsSitting() && !this.isRidingHorse() && !this.bathingClothes.Value;

    public void performRenovation(string location_name)
    {
      GameLocation locationFromName = Game1.getLocationFromName(location_name);
      if (locationFromName == null || !(locationFromName is FarmHouse farmHouse))
        return;
      farmHouse.UpdateForRenovation();
    }

    public void performPlayerEmote(string emote_string)
    {
      for (int index = 0; index < Farmer.EMOTES.Length; ++index)
      {
        Farmer.EmoteType emoteType = Farmer.EMOTES[index];
        if (emoteType.emoteString == emote_string)
        {
          this.performedEmotes[emote_string] = true;
          if (emoteType.animationFrames != null)
          {
            if (!this.CanEmote())
              break;
            if (this.isEmoteAnimating)
              this.EndEmoteAnimation();
            else if (this.FarmerSprite.PauseForSingleAnimation)
              break;
            this.isEmoteAnimating = true;
            this._emoteGracePeriod = 200;
            if (this == Game1.player)
              Game1.player.noMovementPause = Math.Max(Game1.player.noMovementPause, 200);
            this.emoteFacingDirection = emoteType.facingDirection;
            this.FarmerSprite.animateOnce(emoteType.animationFrames, new AnimatedSprite.endOfAnimationBehavior(this.OnEmoteAnimationEnd));
          }
          if (emoteType.emoteIconIndex >= 0)
          {
            this.isEmoting = false;
            this.doEmote(emoteType.emoteIconIndex, false);
          }
        }
      }
    }

    public bool ShouldHandleAnimationSound() => !LocalMultiplayer.IsLocalMultiplayer(true) || this.IsLocalPlayer;

    public static List<Item> initialTools() => new List<Item>()
    {
      (Item) new Axe(),
      (Item) new Hoe(),
      (Item) new WateringCan(),
      (Item) new Pickaxe(),
      (Item) new MeleeWeapon(47)
    };

    private void playHarpEmoteSound()
    {
      int[] numArray1 = new int[4]{ 1200, 1600, 1900, 2400 };
      switch (Game1.random.Next(5))
      {
        case 0:
          numArray1 = new int[4]{ 1200, 1600, 1900, 2400 };
          break;
        case 1:
          numArray1 = new int[4]{ 1200, 1700, 2100, 2400 };
          break;
        case 2:
          numArray1 = new int[4]{ 1100, 1400, 1900, 2300 };
          break;
        case 3:
          numArray1 = new int[3]{ 1600, 1900, 2400 };
          break;
        case 4:
          numArray1 = new int[3]{ 700, 1200, 1900 };
          break;
      }
      if (!this.IsLocalPlayer)
        return;
      if (Game1.IsMultiplayer && (long) this.uniqueMultiplayerID % 111L == 0L)
      {
        int[] numArray2 = new int[4]
        {
          800 + Game1.random.Next(4) * 100,
          1200 + Game1.random.Next(4) * 100,
          1600 + Game1.random.Next(4) * 100,
          2000 + Game1.random.Next(4) * 100
        };
        for (int index = 0; index < numArray2.Length; ++index)
        {
          DelayedAction.playSoundAfterDelay("miniharp_note", Game1.random.Next(60, 150) * index, this.currentLocation, numArray2[index]);
          if (index > 1 && Game1.random.NextDouble() < 0.25)
            break;
        }
      }
      else
      {
        for (int index = 0; index < numArray1.Length; ++index)
          DelayedAction.playSoundAfterDelay("miniharp_note", index > 0 ? 150 + Game1.random.Next(35, 51) * index : 0, this.currentLocation, numArray1[index]);
      }
    }

    private static void removeLowestUpgradeLevelTool(List<Item> items, Type toolType)
    {
      Tool tool = (Tool) null;
      foreach (Item obj in items)
      {
        if (obj is Tool && obj.GetType() == toolType && (tool == null || (int) (NetFieldBase<int, NetInt>) (obj as Tool).upgradeLevel < (int) (NetFieldBase<int, NetInt>) tool.upgradeLevel))
          tool = obj as Tool;
      }
      if (tool == null)
        return;
      items.Remove((Item) tool);
    }

    public static void removeInitialTools(List<Item> items)
    {
      Farmer.removeLowestUpgradeLevelTool(items, typeof (Axe));
      Farmer.removeLowestUpgradeLevelTool(items, typeof (Hoe));
      Farmer.removeLowestUpgradeLevelTool(items, typeof (WateringCan));
      Farmer.removeLowestUpgradeLevelTool(items, typeof (Pickaxe));
      Item obj = items.FirstOrDefault<Item>((Func<Item, bool>) (item => item is MeleeWeapon && (item as Tool).InitialParentTileIndex == 47));
      if (obj == null)
        return;
      items.Remove(obj);
    }

    public Point getMailboxPosition()
    {
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building.isCabin && building.nameOfIndoors == (string) (NetFieldBase<string, NetString>) this.homeLocation)
          return building.getMailboxPosition();
      }
      return Game1.getFarm().GetMainMailboxPosition();
    }

    public void ClearBuffs()
    {
      Game1.buffsDisplay.clearAllBuffs();
      this.stopGlowing();
      this.addedCombatLevel.Value = 0;
      this.addedFarmingLevel.Value = 0;
      this.addedFishingLevel.Value = 0;
      this.addedForagingLevel.Value = 0;
      this.addedLuckLevel.Value = 0;
      this.addedMiningLevel.Value = 0;
      this.addedSpeed = 0;
      this.attack = 0;
      Game1.player.appliedSpecialBuffs.Clear();
    }

    public void addBuffAttributes(int[] buffAttributes)
    {
      for (int index = 0; index < this.appliedBuffs.Length; ++index)
        this.appliedBuffs[index] += buffAttributes[index];
      this.addedFarmingLevel.Value += buffAttributes[0];
      this.addedFishingLevel.Value += buffAttributes[1];
      this.addedMiningLevel.Value += buffAttributes[2];
      this.addedLuckLevel.Value += buffAttributes[4];
      this.addedForagingLevel.Value += buffAttributes[5];
      this.CraftingTime -= buffAttributes[6];
      this.MaxStamina += buffAttributes[7];
      this.MagneticRadius += buffAttributes[8];
      this.resilience += buffAttributes[10];
      this.attack += buffAttributes[11];
      this.addedSpeed += buffAttributes[9];
    }

    public void removeBuffAttributes(int[] buffAttributes)
    {
      for (int index = 0; index < this.appliedBuffs.Length; ++index)
        this.appliedBuffs[index] -= buffAttributes[index];
      if (buffAttributes[0] != 0)
        this.addedFarmingLevel.Value = Math.Max(0, (int) (NetFieldBase<int, NetInt>) this.addedFarmingLevel - buffAttributes[0]);
      if (buffAttributes[1] != 0)
        this.addedFishingLevel.Value = Math.Max(0, (int) (NetFieldBase<int, NetInt>) this.addedFishingLevel - buffAttributes[1]);
      if (buffAttributes[2] != 0)
        this.addedMiningLevel.Value = Math.Max(0, (int) (NetFieldBase<int, NetInt>) this.addedMiningLevel - buffAttributes[2]);
      if (buffAttributes[4] != 0)
        this.addedLuckLevel.Value = Math.Max(0, (int) (NetFieldBase<int, NetInt>) this.addedLuckLevel - buffAttributes[4]);
      if (buffAttributes[5] != 0)
        this.addedForagingLevel.Value = Math.Max(0, (int) (NetFieldBase<int, NetInt>) this.addedForagingLevel - buffAttributes[5]);
      if (buffAttributes[6] != 0)
        this.CraftingTime = Math.Max(0, this.CraftingTime - buffAttributes[6]);
      if (buffAttributes[7] != 0)
      {
        this.MaxStamina = Math.Max(0, this.MaxStamina - buffAttributes[7]);
        this.stamina = Math.Min(this.stamina, (float) this.MaxStamina);
      }
      if (buffAttributes[8] != 0)
        this.MagneticRadius = Math.Max(0, this.MagneticRadius - buffAttributes[8]);
      if (buffAttributes[10] != 0)
        this.resilience = Math.Max(0, this.resilience - buffAttributes[10]);
      if (buffAttributes[9] != 0)
      {
        if (buffAttributes[9] < 0)
          this.addedSpeed += Math.Abs(buffAttributes[9]);
        else
          this.addedSpeed -= buffAttributes[9];
      }
      if (buffAttributes[11] == 0)
        return;
      if (buffAttributes[11] < 0)
        this.attack += Math.Abs(buffAttributes[11]);
      else
        this.attack -= buffAttributes[11];
    }

    public void removeBuffAttributes() => this.removeBuffAttributes(this.appliedBuffs.ToArray<int>());

    public bool isActive() => this == Game1.player || Game1.otherFarmers.ContainsKey(this.UniqueMultiplayerID);

    public string getTexture() => "Characters\\Farmer\\farmer_" + (this.IsMale ? "" : "girl_") + "base" + (this.isBald() ? "_bald" : "");

    public void checkForLevelTenStatus()
    {
    }

    public void unload()
    {
      if (this.FarmerRenderer == null)
        return;
      this.FarmerRenderer.unload();
    }

    public void setInventory(List<Item> newInventory)
    {
      this.items.CopyFrom((IList<Item>) newInventory);
      for (int count = this.items.Count; count < (int) (NetFieldBase<int, NetInt>) this.maxItems; ++count)
        this.items.Add((Item) null);
    }

    public void makeThisTheActiveObject(Object o)
    {
      if (this.freeSpotsInInventory() <= 0)
        return;
      Item currentItem = this.CurrentItem;
      this.ActiveObject = o;
      this.addItemToInventory(currentItem);
    }

    public int getNumberOfChildren() => this.getChildrenCount();

    private void setMount(Horse mount)
    {
      if (mount != null)
      {
        this.netMount.Value = mount;
        this.xOffset = -11f;
        this.Position = Utility.PointToVector2(mount.GetBoundingBox().Location);
        this.position.Y -= 16f;
        this.position.X -= 8f;
        this.speed = 2;
        this.showNotCarrying();
      }
      else
      {
        this.netMount.Value = (Horse) null;
        this.collisionNPC = (NPC) null;
        this.running = false;
        this.speed = !Game1.isOneOfTheseKeysDown(Game1.GetKeyboardState(), Game1.options.runButton) || Game1.options.autoRun ? 2 : 5;
        this.running = this.speed == 5;
        if (this.running)
          this.speed = 5;
        else
          this.speed = 2;
        this.completelyStopAnimatingOrDoingAction();
        this.xOffset = 0.0f;
      }
    }

    public bool isRidingHorse() => this.mount != null && !Game1.eventUp;

    public List<Child> getChildren() => Utility.getHomeOfFarmer(this).getChildren();

    public int getChildrenCount() => Utility.getHomeOfFarmer(this).getChildrenCount();

    public Tool getToolFromName(string name)
    {
      foreach (Item toolFromName in (NetList<Item, NetRef<Item>>) this.items)
      {
        if (toolFromName != null && toolFromName is Tool && toolFromName.Name.Contains(name))
          return (Tool) toolFromName;
      }
      return (Tool) null;
    }

    public override void SetMovingDown(bool b) => this.setMoving((byte) (4 + (b ? 0 : 32)));

    public override void SetMovingRight(bool b) => this.setMoving((byte) (2 + (b ? 0 : 32)));

    public override void SetMovingUp(bool b) => this.setMoving((byte) (1 + (b ? 0 : 32)));

    public override void SetMovingLeft(bool b) => this.setMoving((byte) (8 + (b ? 0 : 32)));

    public int? tryGetFriendshipLevelForNPC(string name)
    {
      Friendship friendship;
      return this.friendshipData.TryGetValue(name, out friendship) ? new int?(friendship.Points) : new int?();
    }

    public int getFriendshipLevelForNPC(string name)
    {
      Friendship friendship;
      return this.friendshipData.TryGetValue(name, out friendship) ? friendship.Points : 0;
    }

    public int getFriendshipHeartLevelForNPC(string name) => this.getFriendshipLevelForNPC(name) / 250;

    public bool isRoommate(string name)
    {
      Friendship friendship;
      return name != null && this.friendshipData.TryGetValue(name, out friendship) && friendship.IsRoommate();
    }

    public bool hasCurrentOrPendingRoommate()
    {
      Friendship friendship;
      return this.spouse != null && this.friendshipData.TryGetValue(this.spouse, out friendship) && friendship.RoommateMarriage;
    }

    public bool hasRoommate() => this.spouse != null && this.isRoommate(this.spouse);

    public bool hasAFriendWithHeartLevel(int heartLevel, bool datablesOnly)
    {
      foreach (NPC allCharacter in Utility.getAllCharacters())
      {
        if ((!datablesOnly || (bool) (NetFieldBase<bool, NetBool>) allCharacter.datable) && this.getFriendshipHeartLevelForNPC(allCharacter.Name) >= heartLevel)
          return true;
      }
      return false;
    }

    public int getTallyOfObject(int index, bool bigCraftable)
    {
      int tallyOfObject = 0;
      foreach (Item obj in (NetList<Item, NetRef<Item>>) this.items)
      {
        if (obj is Object && (obj as Object).ParentSheetIndex == index && (bool) (NetFieldBase<bool, NetBool>) (obj as Object).bigCraftable == bigCraftable)
          tallyOfObject += obj.Stack;
      }
      return tallyOfObject;
    }

    public bool areAllItemsNull()
    {
      for (int index = 0; index < this.items.Count; ++index)
      {
        if (this.items[index] != null)
          return false;
      }
      return true;
    }

    public void shippedBasic(int index, int number)
    {
      if (this.basicShipped.ContainsKey(index))
        this.basicShipped[index] += number;
      else
        this.basicShipped.Add(index, number);
    }

    public void shiftToolbar(bool right)
    {
      if (this.items == null || this.items.Count < 12 || this.UsingTool || Game1.dialogueUp || !Game1.pickingTool && !Game1.player.CanMove || this.areAllItemsNull() || Game1.eventUp || Game1.farmEvent != null)
        return;
      Game1.playSound("shwip");
      if (this.CurrentItem != null)
        this.CurrentItem.actionWhenStopBeingHeld(this);
      if (right)
      {
        List<Item> range = this.items.GetRange(0, 12);
        this.items.RemoveRange(0, 12);
        this.items.AddRange((IEnumerable<Item>) range);
      }
      else
      {
        List<Item> range = this.items.GetRange(this.items.Count - 12, 12);
        for (int index = 0; index < this.items.Count - 12; ++index)
          range.Add(this.items[index]);
        this.items.Set((IList<Item>) range);
      }
      this.netItemStowed.Set(false);
      if (this.CurrentItem != null)
        this.CurrentItem.actionWhenBeingHeld(this);
      for (int index = 0; index < Game1.onScreenMenus.Count; ++index)
      {
        if (Game1.onScreenMenus[index] is Toolbar)
        {
          (Game1.onScreenMenus[index] as Toolbar).shifted(right);
          break;
        }
      }
    }

    public void foundWalnut(int stack = 1)
    {
      Game1.netWorldState.Value.GoldenWalnuts.Value += stack;
      Game1.netWorldState.Value.GoldenWalnutsFound.Value += stack;
      if (!this.IsBusyDoingSomething())
        this.showNutPickup();
      else
        this.hasNutPickupQueued = true;
    }

    public virtual void RemoveMail(string mail_key, bool from_broadcast_list = false)
    {
      mail_key = mail_key.Replace("%&NL&%", "");
      this.mailReceived.Remove(mail_key);
      this.mailbox.Remove(mail_key);
      this.mailForTomorrow.Remove(mail_key);
      this.mailForTomorrow.Remove(mail_key + "%&NL&%");
      if (!from_broadcast_list)
        return;
      for (int index = 0; index < this.team.broadcastedMail.Count; ++index)
      {
        string str = this.team.broadcastedMail[index];
        if (str == "%&SM&%" + mail_key || str == "%&MFT&%" + mail_key || str == "%&MB&%" + mail_key)
        {
          this.team.broadcastedMail.RemoveAt(index);
          --index;
        }
      }
    }

    public virtual void showNutPickup()
    {
      if (!this.hasOrWillReceiveMail("lostWalnutFound") && !Game1.eventUp)
      {
        Game1.addMailForTomorrow("lostWalnutFound", true);
        this.holdUpItemThenMessage((Item) new Object(73, 1));
      }
      else
      {
        if (!this.hasOrWillReceiveMail("lostWalnutFound") || Game1.eventUp)
          return;
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(0, 240, 16, 16), 100f, 4, 2, new Vector2(0.0f, -96f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2(0.0f, -6f),
          acceleration = new Vector2(0.0f, 0.2f),
          stopAcceleratingWhenVelocityIsZero = true,
          attachedCharacter = (Character) this,
          positionFollowsAttachedCharacter = true
        });
      }
    }

    public void foundArtifact(int index, int number)
    {
      bool flag = false;
      if (index == 102)
      {
        if (!this.hasOrWillReceiveMail("lostBookFound"))
        {
          Game1.addMailForTomorrow("lostBookFound", true);
          flag = true;
        }
        else
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14100"));
        Game1.playSound("newRecipe");
        ++Game1.netWorldState.Value.LostBooksFound.Value;
        Game1.multiplayer.globalChatInfoMessage("LostBook", this.displayName);
      }
      if (this.archaeologyFound.ContainsKey(index))
      {
        int[] numArray = this.archaeologyFound[index];
        numArray[0] += number;
        numArray[1] += number;
        this.archaeologyFound[index] = numArray;
      }
      else
      {
        if (this.archaeologyFound.Count() == 0)
        {
          if (!this.eventsSeen.Contains(0) && index != 102)
            this.addQuest(23);
          this.mailReceived.Add("artifactFound");
          flag = true;
        }
        this.archaeologyFound.Add(index, new int[2]
        {
          number,
          number
        });
      }
      if (!flag)
        return;
      this.holdUpItemThenMessage((Item) new Object(index, 1));
    }

    public void cookedRecipe(int index)
    {
      if (this.recipesCooked.ContainsKey(index))
        this.recipesCooked[index]++;
      else
        this.recipesCooked.Add(index, 1);
    }

    public bool caughtFish(int index, int size, bool from_fish_pond = false, int numberCaught = 1)
    {
      if (index >= 167 && index < 173)
        return false;
      bool flag = false;
      if (!from_fish_pond)
      {
        if (this.fishCaught.ContainsKey(index))
        {
          int[] numArray = this.fishCaught[index];
          numArray[0] += numberCaught;
          Game1.stats.checkForFishingAchievements();
          if (size > this.fishCaught[index][1])
          {
            numArray[1] = size;
            flag = true;
          }
          this.fishCaught[index] = numArray;
        }
        else
        {
          this.fishCaught.Add(index, new int[2]
          {
            numberCaught,
            size
          });
          Game1.stats.checkForFishingAchievements();
        }
        this.checkForQuestComplete((NPC) null, index, numberCaught, (Item) null, (string) null, 7);
      }
      return flag;
    }

    public void gainExperience(int which, int howMuch)
    {
      if (which == 5 || howMuch <= 0)
        return;
      if (!this.IsLocalPlayer)
      {
        this.queueMessage((byte) 17, Game1.player, (object) which, (object) howMuch);
      }
      else
      {
        int num1 = Farmer.checkForLevelGain(this.experiencePoints[which], this.experiencePoints[which] + howMuch);
        this.experiencePoints[which] += howMuch;
        int num2 = -1;
        if (num1 != -1)
        {
          switch (which)
          {
            case 0:
              num2 = (int) (NetFieldBase<int, NetInt>) this.farmingLevel;
              this.farmingLevel.Value = num1;
              break;
            case 1:
              num2 = (int) (NetFieldBase<int, NetInt>) this.fishingLevel;
              this.fishingLevel.Value = num1;
              break;
            case 2:
              num2 = (int) (NetFieldBase<int, NetInt>) this.foragingLevel;
              this.foragingLevel.Value = num1;
              break;
            case 3:
              num2 = (int) (NetFieldBase<int, NetInt>) this.miningLevel;
              this.miningLevel.Value = num1;
              break;
            case 4:
              num2 = (int) (NetFieldBase<int, NetInt>) this.combatLevel;
              this.combatLevel.Value = num1;
              break;
            case 5:
              num2 = (int) (NetFieldBase<int, NetInt>) this.luckLevel;
              this.luckLevel.Value = num1;
              break;
          }
        }
        if (num1 <= num2)
          return;
        for (int y = num2 + 1; y <= num1; ++y)
        {
          this.newLevels.Add(new Point(which, y));
          int count = this.newLevels.Count;
        }
      }
    }

    public int getEffectiveSkillLevel(int whichSkill)
    {
      if (whichSkill < 0 || whichSkill > 5)
        return -1;
      int[] numArray = new int[6]
      {
        (int) (NetFieldBase<int, NetInt>) this.farmingLevel,
        (int) (NetFieldBase<int, NetInt>) this.fishingLevel,
        (int) (NetFieldBase<int, NetInt>) this.foragingLevel,
        (int) (NetFieldBase<int, NetInt>) this.miningLevel,
        (int) (NetFieldBase<int, NetInt>) this.combatLevel,
        (int) (NetFieldBase<int, NetInt>) this.luckLevel
      };
      for (int index = 0; index < this.newLevels.Count; ++index)
        --numArray[this.newLevels[index].X];
      return numArray[whichSkill];
    }

    public static int checkForLevelGain(int oldXP, int newXP)
    {
      int num = -1;
      if (oldXP < 100 && newXP >= 100)
        num = 1;
      if (oldXP < 380 && newXP >= 380)
        num = 2;
      if (oldXP < 770 && newXP >= 770)
        num = 3;
      if (oldXP < 1300 && newXP >= 1300)
        num = 4;
      if (oldXP < 2150 && newXP >= 2150)
        num = 5;
      if (oldXP < 3300 && newXP >= 3300)
        num = 6;
      if (oldXP < 4800 && newXP >= 4800)
        num = 7;
      if (oldXP < 6900 && newXP >= 6900)
        num = 8;
      if (oldXP < 10000 && newXP >= 10000)
        num = 9;
      if (oldXP < 15000 && newXP >= 15000)
        num = 10;
      return num;
    }

    public void revealGiftTaste(NPC npc, int parent_sheet_index)
    {
      if (!this.giftedItems.ContainsKey((string) (NetFieldBase<string, NetString>) npc.name))
        this.giftedItems[(string) (NetFieldBase<string, NetString>) npc.name] = new SerializableDictionary<int, int>();
      if (this.giftedItems[(string) (NetFieldBase<string, NetString>) npc.name].ContainsKey(parent_sheet_index))
        return;
      this.giftedItems[(string) (NetFieldBase<string, NetString>) npc.name][parent_sheet_index] = 0;
    }

    public void revealGiftTaste(NPC npc, Object item)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) item.bigCraftable)
        return;
      this.revealGiftTaste(npc, item.ParentSheetIndex);
    }

    public void onGiftGiven(NPC npc, Object item)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) item.bigCraftable)
        return;
      if (!this.giftedItems.ContainsKey((string) (NetFieldBase<string, NetString>) npc.name))
        this.giftedItems[(string) (NetFieldBase<string, NetString>) npc.name] = new SerializableDictionary<int, int>();
      if (!this.giftedItems[(string) (NetFieldBase<string, NetString>) npc.name].ContainsKey(item.ParentSheetIndex))
        this.giftedItems[(string) (NetFieldBase<string, NetString>) npc.name][item.ParentSheetIndex] = 0;
      this.giftedItems[(string) (NetFieldBase<string, NetString>) npc.name][item.ParentSheetIndex] = this.giftedItems[(string) (NetFieldBase<string, NetString>) npc.name][item.ParentSheetIndex] + 1;
      if (Game1.player.team.specialOrders == null)
        return;
      foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
      {
        if (specialOrder.onGiftGiven != null)
          specialOrder.onGiftGiven(Game1.player, npc, (Item) item);
      }
    }

    public bool hasGiftTasteBeenRevealed(NPC npc, int item_index) => this.hasItemBeenGifted(npc, item_index) || this.giftedItems.ContainsKey((string) (NetFieldBase<string, NetString>) npc.name) && this.giftedItems[(string) (NetFieldBase<string, NetString>) npc.name].ContainsKey(item_index);

    public bool hasItemBeenGifted(NPC npc, int item_index) => this.giftedItems.ContainsKey((string) (NetFieldBase<string, NetString>) npc.name) && this.giftedItems[(string) (NetFieldBase<string, NetString>) npc.name].ContainsKey(item_index) && this.giftedItems[(string) (NetFieldBase<string, NetString>) npc.name][item_index] > 0;

    public void MarkItemAsTailored(Item item)
    {
      if (item == null)
        return;
      string descriptionFromItem = Utility.getStandardDescriptionFromItem(item, 1);
      if (!this.tailoredItems.ContainsKey(descriptionFromItem))
        this.tailoredItems[descriptionFromItem] = 0;
      this.tailoredItems[descriptionFromItem]++;
    }

    public bool HasTailoredThisItem(Item item) => item != null && this.tailoredItems.ContainsKey(Utility.getStandardDescriptionFromItem(item, 1));

    public void foundMineral(int index)
    {
      if (this.mineralsFound.ContainsKey(index))
        this.mineralsFound[index]++;
      else
        this.mineralsFound.Add(index, 1);
      if (this.hasOrWillReceiveMail("artifactFound"))
        return;
      this.mailReceived.Add("artifactFound");
    }

    public void increaseBackpackSize(int howMuch)
    {
      this.MaxItems += howMuch;
      for (int index = 0; index < howMuch; ++index)
        this.items.Add((Item) null);
    }

    public override int FacingDirection
    {
      get => this.isEmoteAnimating ? this.emoteFacingDirection : (int) this.facingDirection;
      set => this.facingDirection.Set(value);
    }

    public void consumeObject(int index, int quantity)
    {
      for (int index1 = this.items.Count - 1; index1 >= 0; --index1)
      {
        if (this.items[index1] != null && this.items[index1] is Object && (int) (NetFieldBase<int, NetInt>) this.items[index1].parentSheetIndex == index)
        {
          int num = quantity;
          quantity -= this.items[index1].Stack;
          this.items[index1].Stack -= num;
          if (this.items[index1].Stack <= 0)
            this.items[index1] = (Item) null;
          if (quantity <= 0)
            break;
        }
      }
    }

    public int getItemCount(int item_index, int min_price = 0) => this.getItemCountInList((IList<Item>) this.items, item_index, min_price);

    public bool hasItemInInventory(int itemIndex, int quantity, int minPrice = 0)
    {
      if (itemIndex == 858)
        return this.QiGems >= quantity;
      if (itemIndex == 73)
        return (int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.GoldenWalnuts >= quantity;
      return this.getItemCount(itemIndex, minPrice) >= quantity;
    }

    public bool hasItemInInventoryNamed(string name)
    {
      for (int index = 0; index < this.items.Count; ++index)
      {
        if (this.items[index] != null && this.items[index].Name != null && this.items[index].Name.Equals(name))
          return true;
      }
      return false;
    }

    public int getItemCountInList(IList<Item> list, int item_index, int min_price = 0)
    {
      int itemCountInList = 0;
      for (int index = 0; index < list.Count; ++index)
      {
        if (list[index] != null && list[index] is Object && !(list[index] is Furniture) && !(list[index] is Wallpaper) && !(bool) (NetFieldBase<bool, NetBool>) (list[index] as Object).bigCraftable && (list[index].ParentSheetIndex == item_index || list[index] is Object && list[index].Category == item_index || CraftingRecipe.isThereSpecialIngredientRule((Object) list[index], item_index)))
          itemCountInList += list[index].Stack;
      }
      return itemCountInList;
    }

    public bool hasItemInList(IList<Item> list, int itemIndex, int quantity, int minPrice = 0) => this.getItemCountInList(list, itemIndex, minPrice) >= quantity;

    public void addItemByMenuIfNecessaryElseHoldUp(
      Item item,
      ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null)
    {
      this.mostRecentlyGrabbedItem = item;
      this.addItemsByMenuIfNecessary(new List<Item>()
      {
        item
      }, itemSelectedCallback);
      if (Game1.activeClickableMenu != null || Utility.IsNormalObjectAtParentSheetIndex(this.mostRecentlyGrabbedItem, 434))
        return;
      this.holdUpItemThenMessage(item);
    }

    public void addItemByMenuIfNecessary(
      Item item,
      ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null)
    {
      this.addItemsByMenuIfNecessary(new List<Item>()
      {
        item
      }, itemSelectedCallback);
    }

    public void addItemsByMenuIfNecessary(
      List<Item> itemsToAdd,
      ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null)
    {
      if (itemsToAdd == null || !this.IsLocalPlayer)
        return;
      if (itemsToAdd.Count > 0 && itemsToAdd[0] is Object && Utility.IsNormalObjectAtParentSheetIndex(itemsToAdd[0], 434))
      {
        this.eatObject(itemsToAdd[0] as Object, true);
        if (Game1.activeClickableMenu == null)
          return;
        Game1.activeClickableMenu.exitThisMenu(false);
      }
      else
      {
        for (int index = itemsToAdd.Count - 1; index >= 0; --index)
        {
          if (this.addItemToInventoryBool(itemsToAdd[index]))
          {
            if (itemSelectedCallback != null)
              itemSelectedCallback(itemsToAdd[index], this);
            itemsToAdd.Remove(itemsToAdd[index]);
          }
        }
        if (itemsToAdd.Count <= 0)
          return;
        Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) itemsToAdd).setEssential(true);
        (Game1.activeClickableMenu as ItemGrabMenu).inventory.showGrayedOutSlots = true;
        (Game1.activeClickableMenu as ItemGrabMenu).inventory.onAddItem = itemSelectedCallback;
        (Game1.activeClickableMenu as ItemGrabMenu).source = 2;
      }
    }

    public void ShowSitting()
    {
      if (!this.IsSitting())
        return;
      if (this.sittingFurniture != null)
        this.FacingDirection = this.sittingFurniture.GetSittingDirection();
      if (this.yJumpOffset != 0)
      {
        switch (this.FacingDirection)
        {
          case 0:
            this.FarmerSprite.setCurrentSingleFrame(12);
            break;
          case 1:
            this.FarmerSprite.setCurrentSingleFrame(6);
            break;
          case 2:
            this.FarmerSprite.setCurrentSingleFrame(0);
            break;
          case 3:
            this.FarmerSprite.setCurrentSingleFrame(6, flip: true);
            break;
        }
      }
      else
      {
        switch (this.FacingDirection)
        {
          case 0:
            this.FarmerSprite.setCurrentSingleFrame(113);
            this.xOffset = 0.0f;
            this.yOffset = -40f;
            break;
          case 1:
            this.FarmerSprite.setCurrentSingleFrame(117);
            this.xOffset = -4f;
            this.yOffset = -32f;
            break;
          case 2:
            this.FarmerSprite.setCurrentSingleFrame(107, secondaryArm: true);
            this.xOffset = 0.0f;
            this.yOffset = -48f;
            break;
          case 3:
            this.FarmerSprite.setCurrentSingleFrame(117, flip: true);
            this.xOffset = 4f;
            this.yOffset = -32f;
            break;
        }
      }
    }

    public void showRiding()
    {
      if (!this.isRidingHorse())
        return;
      this.xOffset = -6f;
      switch (this.FacingDirection)
      {
        case 0:
          this.FarmerSprite.setCurrentSingleFrame(113);
          break;
        case 1:
          this.FarmerSprite.setCurrentSingleFrame(106);
          this.xOffset += 2f;
          break;
        case 2:
          this.FarmerSprite.setCurrentSingleFrame(107);
          break;
        case 3:
          this.FarmerSprite.setCurrentSingleFrame(106, flip: true);
          this.xOffset = -12f;
          break;
      }
      if (this.isMoving())
      {
        switch (this.mount.Sprite.currentAnimationIndex)
        {
          case 0:
            this.yOffset = 0.0f;
            break;
          case 1:
            this.yOffset = -4f;
            break;
          case 2:
            this.yOffset = -4f;
            break;
          case 3:
            this.yOffset = 0.0f;
            break;
          case 4:
            this.yOffset = 4f;
            break;
          case 5:
            this.yOffset = 4f;
            break;
        }
      }
      else
        this.yOffset = 0.0f;
    }

    public void showCarrying()
    {
      if (Game1.eventUp || this.isRidingHorse() || Game1.killScreen || this.IsSitting())
        return;
      if ((bool) (NetFieldBase<bool, NetBool>) this.bathingClothes || this.onBridge.Value)
      {
        this.showNotCarrying();
      }
      else
      {
        if (!this.FarmerSprite.PauseForSingleAnimation && !this.isMoving())
        {
          switch (this.FacingDirection)
          {
            case 0:
              this.FarmerSprite.setCurrentFrame(144);
              break;
            case 1:
              this.FarmerSprite.setCurrentFrame(136);
              break;
            case 2:
              this.FarmerSprite.setCurrentFrame(128);
              break;
            case 3:
              this.FarmerSprite.setCurrentFrame(152);
              break;
          }
        }
        if (this.ActiveObject != null)
          this.mostRecentlyGrabbedItem = (Item) this.ActiveObject;
        if (!this.IsLocalPlayer || this.mostRecentlyGrabbedItem == null || !(this.mostRecentlyGrabbedItem is Object) || !Utility.IsNormalObjectAtParentSheetIndex(this.mostRecentlyGrabbedItem, 434))
          return;
        this.eatHeldObject();
      }
    }

    public void showNotCarrying()
    {
      if (this.FarmerSprite.PauseForSingleAnimation || this.isMoving())
        return;
      bool flag = this.canOnlyWalk || (bool) (NetFieldBase<bool, NetBool>) this.bathingClothes || this.onBridge.Value;
      switch (this.FacingDirection)
      {
        case 0:
          this.FarmerSprite.setCurrentFrame(flag ? 16 : 48, flag ? 1 : 0);
          break;
        case 1:
          this.FarmerSprite.setCurrentFrame(flag ? 8 : 40, flag ? 1 : 0);
          break;
        case 2:
          this.FarmerSprite.setCurrentFrame(flag ? 0 : 32, flag ? 1 : 0);
          break;
        case 3:
          this.FarmerSprite.setCurrentFrame(flag ? 24 : 56, flag ? 1 : 0);
          break;
      }
    }

    public int GetDaysMarried() => this.spouse == null || this.spouse == "" ? 0 : this.friendshipData[this.spouse].DaysMarried;

    public Friendship GetSpouseFriendship()
    {
      if (Game1.player.team.GetSpouse(this.UniqueMultiplayerID).HasValue)
        return Game1.player.team.GetFriendship(this.UniqueMultiplayerID, Game1.player.team.GetSpouse(this.UniqueMultiplayerID).Value);
      return this.spouse == null || this.spouse == "" ? (Friendship) null : this.friendshipData[this.spouse];
    }

    public bool hasDailyQuest()
    {
      for (int index = this.questLog.Count - 1; index >= 0; --index)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) this.questLog[index].dailyQuest)
          return true;
      }
      return false;
    }

    public void showToolUpgradeAvailability()
    {
      int dayOfMonth = Game1.dayOfMonth;
      if (!((NetFieldBase<Tool, NetRef<Tool>>) this.toolBeingUpgraded != (NetRef<Tool>) null) || (int) (NetFieldBase<int, NetInt>) this.daysLeftForToolUpgrade > 0 || this.toolBeingUpgraded.Value == null || Utility.isFestivalDay(dayOfMonth, Game1.currentSeason) || !(Game1.shortDayNameFromDayOfSeason(dayOfMonth) != "Fri") && this.hasCompletedCommunityCenter() && !Game1.isRaining || this.hasReceivedToolUpgradeMessageYet)
        return;
      if (Game1.newDay)
        Game1.morningQueue.Enqueue((DelayedAction.delayedBehavior) (() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:ToolReady", (object) this.toolBeingUpgraded.Value.DisplayName))));
      else
        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:ToolReady", (object) this.toolBeingUpgraded.Value.DisplayName));
      this.hasReceivedToolUpgradeMessageYet = true;
    }

    public void dayupdate()
    {
      if (this.IsSitting())
        this.StopSitting(false);
      this.resetFriendshipsForNewDay();
      this.hasUsedDailyRevive.Value = false;
      this.acceptedDailyQuest.Set(false);
      this.dancePartner.Value = (Character) null;
      this.festivalScore = 0;
      this.forceTimePass = false;
      if ((int) (NetFieldBase<int, NetInt>) this.daysLeftForToolUpgrade > 0)
        --this.daysLeftForToolUpgrade.Value;
      if ((int) (NetFieldBase<int, NetInt>) this.daysUntilHouseUpgrade > 0)
      {
        --this.daysUntilHouseUpgrade.Value;
        if ((int) (NetFieldBase<int, NetInt>) this.daysUntilHouseUpgrade <= 0)
        {
          FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(this);
          homeOfFarmer.moveObjectsForHouseUpgrade((int) (NetFieldBase<int, NetInt>) this.houseUpgradeLevel + 1);
          ++this.houseUpgradeLevel.Value;
          this.daysUntilHouseUpgrade.Value = -1;
          homeOfFarmer.setMapForUpgradeLevel((int) (NetFieldBase<int, NetInt>) this.houseUpgradeLevel);
          Game1.stats.checkForBuildingUpgradeAchievements();
        }
      }
      for (int index = this.questLog.Count - 1; index >= 0; --index)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) this.questLog[index].dailyQuest)
        {
          --this.questLog[index].daysLeft.Value;
          if ((int) (NetFieldBase<int, NetInt>) this.questLog[index].daysLeft <= 0 && !(bool) (NetFieldBase<bool, NetBool>) this.questLog[index].completed)
            this.questLog.RemoveAt(index);
        }
      }
      this.ClearBuffs();
      if (this.MaxStamina >= 508 && !this.mailReceived.Contains("gotMaxStamina"))
        this.mailReceived.Add("gotMaxStamina");
      if (this.leftRing.Value != null)
        this.leftRing.Value.onDayUpdate(this, this.currentLocation);
      if (this.rightRing.Value != null)
        this.rightRing.Value.onDayUpdate(this, this.currentLocation);
      this.bobber = "";
      float stamina = this.Stamina;
      this.Stamina = (float) this.MaxStamina;
      if ((bool) (NetFieldBase<bool, NetBool>) this.exhausted)
      {
        this.exhausted.Value = false;
        this.Stamina = (float) (this.MaxStamina / 2 + 1);
      }
      int val2 = (int) (NetFieldBase<int, NetInt>) this.timeWentToBed == 0 ? Game1.timeOfDay : (int) (NetFieldBase<int, NetInt>) this.timeWentToBed;
      if (val2 > 2400)
      {
        this.Stamina -= (float) (1.0 - (double) (2600 - Math.Min(2600, val2)) / 200.0) * (float) (this.MaxStamina / 2);
        if (Game1.timeOfDay > 2700)
          this.Stamina /= 2f;
      }
      if (Game1.timeOfDay < 2700 && (double) stamina > (double) this.Stamina && !(bool) (NetFieldBase<bool, NetBool>) this.exhausted)
        this.Stamina = stamina;
      this.health = this.maxHealth;
      List<string> stringList = new List<string>();
      foreach (string key in this.activeDialogueEvents.Keys.ToList<string>())
      {
        this.activeDialogueEvents[key]--;
        if (this.activeDialogueEvents[key] < 0)
        {
          if (key == "pennyRedecorating" && Utility.getHomeOfFarmer(this).GetSpouseBed() == null)
            this.activeDialogueEvents[key] = 0;
          else
            stringList.Add(key);
        }
      }
      foreach (string key in stringList)
        this.activeDialogueEvents.Remove(key);
      this.hasMoved = false;
      if (Game1.random.NextDouble() < 0.905 && !this.hasOrWillReceiveMail("RarecrowSociety") && Utility.doesItemWithThisIndexExistAnywhere(136, true) && Utility.doesItemWithThisIndexExistAnywhere(137, true) && Utility.doesItemWithThisIndexExistAnywhere(138, true) && Utility.doesItemWithThisIndexExistAnywhere(139, true) && Utility.doesItemWithThisIndexExistAnywhere(140, true) && Utility.doesItemWithThisIndexExistAnywhere(126, true) && Utility.doesItemWithThisIndexExistAnywhere(110, true) && Utility.doesItemWithThisIndexExistAnywhere(113, true))
        this.mailbox.Add("RarecrowSociety");
      this.timeWentToBed.Value = 0;
    }

    public void doDivorce()
    {
      this.divorceTonight.Value = false;
      if (!this.isMarried())
        return;
      if (this.spouse != null)
      {
        NPC spouse = this.getSpouse();
        if (spouse == null)
          return;
        this.spouse = (string) null;
        for (int index = this.specialItems.Count - 1; index >= 0; --index)
        {
          if (this.specialItems[index] == 460)
            this.specialItems.RemoveAt(index);
        }
        if (this.friendshipData.ContainsKey((string) (NetFieldBase<string, NetString>) spouse.name))
        {
          this.friendshipData[(string) (NetFieldBase<string, NetString>) spouse.name].Points = 0;
          this.friendshipData[(string) (NetFieldBase<string, NetString>) spouse.name].RoommateMarriage = false;
          this.friendshipData[(string) (NetFieldBase<string, NetString>) spouse.name].Status = FriendshipStatus.Divorced;
        }
        Utility.getHomeOfFarmer(this).showSpouseRoom();
        Game1.getFarm().UpdatePatio();
        this.removeQuest(126);
      }
      else
      {
        long? spouse = this.team.GetSpouse(this.UniqueMultiplayerID);
        if (!spouse.HasValue)
          return;
        spouse = this.team.GetSpouse(this.UniqueMultiplayerID);
        Friendship friendship = this.team.GetFriendship(this.UniqueMultiplayerID, spouse.Value);
        friendship.Points = 0;
        friendship.RoommateMarriage = false;
        friendship.Status = FriendshipStatus.Divorced;
      }
    }

    public static void showReceiveNewItemMessage(Farmer who)
    {
      string dialogue = who.mostRecentlyGrabbedItem.checkForSpecialItemHoldUpMeessage();
      if (dialogue != null)
        Game1.drawObjectDialogue(dialogue);
      else if ((int) (NetFieldBase<int, NetInt>) who.mostRecentlyGrabbedItem.parentSheetIndex == 472 && who.mostRecentlyGrabbedItem.Stack == 15)
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1918"));
      else
        Game1.drawObjectDialogue(who.mostRecentlyGrabbedItem.Stack > 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1922", (object) who.mostRecentlyGrabbedItem.Stack, (object) who.mostRecentlyGrabbedItem.DisplayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1919", (object) who.mostRecentlyGrabbedItem.DisplayName, (object) Lexicon.getProperArticleForWord(who.mostRecentlyGrabbedItem.DisplayName)));
      who.completelyStopAnimatingOrDoingAction();
    }

    public static void showEatingItem(Farmer who)
    {
      TemporaryAnimatedSprite temporaryAnimatedSprite1 = (TemporaryAnimatedSprite) null;
      if (who.itemToEat == null)
        return;
      switch (who.FarmerSprite.currentAnimationIndex)
      {
        case 1:
          temporaryAnimatedSprite1 = !who.IsLocalPlayer || who.itemToEat == null || !(who.itemToEat is Object) || !Utility.IsNormalObjectAtParentSheetIndex(who.itemToEat, 434) ? new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) (who.itemToEat as Object).parentSheetIndex, 16, 16), 254f, 1, 0, who.Position + new Vector2(-21f, -112f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f) : new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(368, 16, 16, 16), 62.75f, 8, 2, who.Position + new Vector2(-21f, -112f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
          break;
        case 2:
          if (who.IsLocalPlayer && who.itemToEat != null && who.itemToEat is Object && Utility.IsNormalObjectAtParentSheetIndex(who.itemToEat, 434))
          {
            temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(368, 16, 16, 16), 81.25f, 8, 0, who.Position + new Vector2(-21f, -108f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, -0.01f, 0.0f, 0.0f)
            {
              motion = new Vector2(0.8f, -11f),
              acceleration = new Vector2(0.0f, 0.5f)
            };
            break;
          }
          if (Game1.currentLocation == who.currentLocation)
            Game1.playSound("dwop");
          temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) (who.itemToEat as Object).parentSheetIndex, 16, 16), 650f, 1, 0, who.Position + new Vector2(-21f, -108f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, -0.01f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.8f, -11f),
            acceleration = new Vector2(0.0f, 0.5f)
          };
          break;
        case 3:
          who.yJumpVelocity = 6f;
          who.yJumpOffset = 1;
          break;
        case 4:
          if (Game1.currentLocation == who.currentLocation && who.ShouldHandleAnimationSound())
            Game1.playSound("eat");
          for (int index = 0; index < 8; ++index)
          {
            Microsoft.Xna.Framework.Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) (who.itemToEat as Object).parentSheetIndex, 16, 16);
            standardTileSheet.X += 8;
            standardTileSheet.Y += 8;
            standardTileSheet.Width = 4;
            standardTileSheet.Height = 4;
            TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("Maps\\springobjects", standardTileSheet, 400f, 1, 0, who.Position + new Vector2(24f, -48f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2((float) Game1.random.Next(-30, 31) / 10f, (float) Game1.random.Next(-6, -3)),
              acceleration = new Vector2(0.0f, 0.5f)
            };
            who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite2);
          }
          return;
        default:
          who.freezePause = 0;
          break;
      }
      if (temporaryAnimatedSprite1 == null)
        return;
      who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite1);
    }

    public static void eatItem(Farmer who)
    {
    }

    public bool hasBuff(int whichBuff) => this.appliedSpecialBuffs.ContainsKey(whichBuff);

    public bool hasOrWillReceiveMail(string id) => this.mailReceived.Contains(id) || this.mailForTomorrow.Contains(id) || Game1.mailbox.Contains(id) || this.mailForTomorrow.Contains(id + "%&NL&%");

    public static void showHoldingItem(Farmer who)
    {
      if (who.mostRecentlyGrabbedItem is SpecialItem)
      {
        TemporaryAnimatedSprite spriteForHoldingUp = (who.mostRecentlyGrabbedItem as SpecialItem).getTemporarySpriteForHoldingUp(who.Position + new Vector2(0.0f, -124f));
        spriteForHoldingUp.motion = new Vector2(0.0f, -0.1f);
        spriteForHoldingUp.scale = 4f;
        spriteForHoldingUp.interval = 2500f;
        spriteForHoldingUp.totalNumberOfLoops = 0;
        spriteForHoldingUp.animationLength = 1;
        Game1.currentLocation.temporarySprites.Add(spriteForHoldingUp);
      }
      else if (who.mostRecentlyGrabbedItem is Slingshot)
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\weapons", Game1.getSquareSourceRectForNonStandardTileSheet(Tool.weaponsTexture, 16, 16, (who.mostRecentlyGrabbedItem as Slingshot).IndexOfMenuItemView), 2500f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2(0.0f, -0.1f)
        });
      else if (who.mostRecentlyGrabbedItem is MeleeWeapon)
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\weapons", Game1.getSquareSourceRectForNonStandardTileSheet(Tool.weaponsTexture, 16, 16, (who.mostRecentlyGrabbedItem as MeleeWeapon).IndexOfMenuItemView), 2500f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2(0.0f, -0.1f)
        });
      else if (who.mostRecentlyGrabbedItem is Boots)
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSquareSourceRectForNonStandardTileSheet(Game1.objectSpriteSheet, 16, 16, (int) (NetFieldBase<int, NetInt>) (who.mostRecentlyGrabbedItem as Boots).indexInTileSheet), 2500f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2(0.0f, -0.1f)
        });
      else if (who.mostRecentlyGrabbedItem is Hat)
      {
        Hat recentlyGrabbedItem = who.mostRecentlyGrabbedItem as Hat;
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Farmer\\hats", new Microsoft.Xna.Framework.Rectangle((int) (NetFieldBase<int, NetInt>) recentlyGrabbedItem.which * 20 % FarmerRenderer.hatsTexture.Width, (int) (NetFieldBase<int, NetInt>) recentlyGrabbedItem.which * 20 / FarmerRenderer.hatsTexture.Width * 20 * 4, 20, 20), 2500f, 1, 0, who.Position + new Vector2(-8f, -124f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2(0.0f, -0.1f)
        });
      }
      else if (who.mostRecentlyGrabbedItem is Tool)
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", Game1.getSquareSourceRectForNonStandardTileSheet(Game1.toolSpriteSheet, 16, 16, (who.mostRecentlyGrabbedItem as Tool).IndexOfMenuItemView), 2500f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2(0.0f, -0.1f)
        });
      else if (who.mostRecentlyGrabbedItem is Furniture)
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\furniture", (Microsoft.Xna.Framework.Rectangle) (NetFieldBase<Microsoft.Xna.Framework.Rectangle, NetRectangle>) (who.mostRecentlyGrabbedItem as Furniture).sourceRect, 2500f, 1, 0, who.Position + new Vector2((float) (32 - (who.mostRecentlyGrabbedItem as Furniture).sourceRect.Width / 2 * 4), -188f), false, false)
        {
          motion = new Vector2(0.0f, -0.1f),
          scale = 4f,
          layerDepth = 1f
        });
      else if (who.mostRecentlyGrabbedItem is Object && !(bool) (NetFieldBase<bool, NetBool>) (who.mostRecentlyGrabbedItem as Object).bigCraftable)
      {
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) who.mostRecentlyGrabbedItem.parentSheetIndex, 16, 16), 2500f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false)
        {
          motion = new Vector2(0.0f, -0.1f),
          scale = 4f,
          layerDepth = 1f
        });
        if (who.IsLocalPlayer && Utility.IsNormalObjectAtParentSheetIndex(who.mostRecentlyGrabbedItem, 434))
          who.eatHeldObject();
      }
      else if (who.mostRecentlyGrabbedItem is Object)
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\Craftables", Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, (int) (NetFieldBase<int, NetInt>) who.mostRecentlyGrabbedItem.parentSheetIndex, 16, 32), 2500f, 1, 0, who.Position + new Vector2(0.0f, -188f), false, false)
        {
          motion = new Vector2(0.0f, -0.1f),
          scale = 4f,
          layerDepth = 1f
        });
      else if (who.mostRecentlyGrabbedItem is Ring)
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) who.mostRecentlyGrabbedItem.parentSheetIndex, 16, 16), 2500f, 1, 0, who.Position + new Vector2(-4f, -124f), false, false)
        {
          motion = new Vector2(0.0f, -0.1f),
          scale = 4f,
          layerDepth = 1f
        });
      if (who.mostRecentlyGrabbedItem == null)
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(420, 489, 25, 18), 2500f, 1, 0, who.Position + new Vector2(-20f, -152f), false, false)
        {
          motion = new Vector2(0.0f, -0.1f),
          scale = 4f,
          layerDepth = 1f
        });
      else
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(10, who.Position + new Vector2(32f, -96f), Color.White)
        {
          motion = new Vector2(0.0f, -0.1f)
        });
    }

    public void holdUpItemThenMessage(Item item, bool showMessage = true)
    {
      this.completelyStopAnimatingOrDoingAction();
      if (showMessage)
        DelayedAction.playSoundAfterDelay("getNewSpecialItem", 750);
      this.faceDirection(2);
      this.freezePause = 4000;
      this.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[3]
      {
        new FarmerSprite.AnimationFrame(57, 0),
        new FarmerSprite.AnimationFrame(57, 2500, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showHoldingItem)),
        showMessage ? new FarmerSprite.AnimationFrame((int) (short) this.FarmerSprite.CurrentFrame, 500, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showReceiveNewItemMessage), true) : new FarmerSprite.AnimationFrame((int) (short) this.FarmerSprite.CurrentFrame, 500, false, false)
      });
      this.mostRecentlyGrabbedItem = item;
      this.canMove = false;
    }

    private void checkForLevelUp()
    {
      int num1 = 600;
      int num2 = 0;
      int level = this.Level;
      for (int index = 0; index <= 35; ++index)
      {
        if (level <= index && (long) this.totalMoneyEarned >= (long) num1)
        {
          this.NewSkillPointsToSpend += 2;
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1925"), Color.Violet, 3500f));
        }
        else if ((long) this.totalMoneyEarned < (long) num1)
          break;
        int num3 = num1;
        num1 += (int) ((double) (num1 - num2) * 1.2);
        num2 = num3;
      }
    }

    public void resetState()
    {
      this.mount = (Horse) null;
      this.removeBuffAttributes();
      this.TemporaryItem = (Item) null;
      this.swimming.Value = false;
      this.bathingClothes.Value = false;
      this.ignoreCollisions = false;
      this.resetItemStates();
      this.fireToolEvent.Clear();
      this.beginUsingToolEvent.Clear();
      this.endUsingToolEvent.Clear();
      this.sickAnimationEvent.Clear();
      this.passOutEvent.Clear();
      this.drinkAnimationEvent.Clear();
      this.eatAnimationEvent.Clear();
    }

    public void resetItemStates()
    {
      for (int index = 0; index < this.items.Count; ++index)
      {
        if (this.items[index] != null)
          this.items[index].resetState();
      }
    }

    public void clearBackpack()
    {
      for (int index = 0; index < this.items.Count; ++index)
        this.items[index] = (Item) null;
    }

    public int numberOfItemsInInventory()
    {
      int num = 0;
      foreach (Item obj in (NetList<Item, NetRef<Item>>) this.items)
      {
        if (obj != null && obj is Object)
          ++num;
      }
      return num;
    }

    public void resetFriendshipsForNewDay()
    {
      foreach (string key in this.friendshipData.Keys)
      {
        bool flag = false;
        NPC n = Game1.getCharacterFromName(key) ?? (NPC) Game1.getCharacterFromName<Child>(key, false);
        if (n != null)
        {
          if (n != null && (bool) (NetFieldBase<bool, NetBool>) n.datable && !this.friendshipData[key].IsDating() && !n.isMarried())
            flag = true;
          if (this.spouse != null && key.Equals(this.spouse) && !this.hasPlayerTalkedToNPC(key))
            this.changeFriendship(-20, n);
          else if (n != null && this.friendshipData[key].IsDating() && !this.hasPlayerTalkedToNPC(key) && this.friendshipData[key].Points < 2500)
            this.changeFriendship(-8, n);
          if (this.hasPlayerTalkedToNPC(key))
            this.friendshipData[key].TalkedToToday = false;
          else if (!flag && this.friendshipData[key].Points < 2500 || flag && this.friendshipData[key].Points < 2000)
            this.changeFriendship(-2, n);
        }
      }
      WorldDate date = new WorldDate(Game1.Date);
      ++date.TotalDays;
      this.updateFriendshipGifts(date);
    }

    public virtual int GetAppliedMagneticRadius() => Math.Max(128, (int) (NetFieldBase<int, NetInt>) this.magneticRadius);

    public void updateFriendshipGifts(WorldDate date)
    {
      foreach (string key in this.friendshipData.Keys)
      {
        if (this.friendshipData[key].LastGiftDate == (WorldDate) null || date.TotalDays != this.friendshipData[key].LastGiftDate.TotalDays)
          this.friendshipData[key].GiftsToday = 0;
        if (this.friendshipData[key].LastGiftDate == (WorldDate) null || date.TotalSundayWeeks != this.friendshipData[key].LastGiftDate.TotalSundayWeeks)
        {
          if (this.friendshipData[key].GiftsThisWeek >= 2)
            this.changeFriendship(10, Game1.getCharacterFromName(key));
          this.friendshipData[key].GiftsThisWeek = 0;
        }
      }
    }

    public bool hasPlayerTalkedToNPC(string name)
    {
      if (!this.friendshipData.ContainsKey(name) && Game1.NPCGiftTastes.ContainsKey(name))
        this.friendshipData.Add(name, new Friendship());
      return this.friendshipData.ContainsKey(name) && this.friendshipData[name].TalkedToToday;
    }

    public void fuelLantern(int units)
    {
      Tool toolFromName = this.getToolFromName("Lantern");
      if (toolFromName == null)
        return;
      ((Lantern) toolFromName).fuelLeft = Math.Min(100, ((Lantern) toolFromName).fuelLeft + units);
    }

    public bool tryToCraftItem(
      List<int[]> ingredients,
      double successRate,
      int itemToCraft,
      bool bigCraftable,
      string craftingOrCooking)
    {
      List<int[]> locationOfIngredients = new List<int[]>();
      foreach (int[] ingredient in ingredients)
      {
        if (ingredient[0] <= -100)
        {
          int num = 0;
          switch (ingredient[0])
          {
            case -106:
              num = this.IridiumPieces;
              break;
            case -105:
              num = this.GoldPieces;
              break;
            case -104:
              num = this.CoalPieces;
              break;
            case -103:
              num = this.IronPieces;
              break;
            case -102:
              num = this.CopperPieces;
              break;
            case -101:
              num = this.stonePieces;
              break;
            case -100:
              num = this.WoodPieces;
              break;
          }
          if (num < ingredient[1])
            return false;
          locationOfIngredients.Add(ingredient);
        }
        else
        {
          for (int index1 = 0; index1 < ingredient[1]; ++index1)
          {
            int[] numArray = new int[2]{ 99999, -1 };
            for (int index2 = 0; index2 < this.items.Count; ++index2)
            {
              if (this.items[index2] != null && this.items[index2] is Object && this.items[index2].ParentSheetIndex == ingredient[0] && !Farmer.containsIndex(locationOfIngredients, index2))
              {
                locationOfIngredients.Add(new int[2]
                {
                  index2,
                  1
                });
                break;
              }
              if (this.items[index2] != null && this.items[index2] is Object && this.items[index2].Category == ingredient[0] && !Farmer.containsIndex(locationOfIngredients, index2) && ((Object) this.items[index2]).Price < numArray[0])
              {
                numArray[0] = ((Object) this.items[index2]).Price;
                numArray[1] = index2;
              }
              if (index2 == this.items.Count - 1)
              {
                if (numArray[1] == -1)
                  return false;
                locationOfIngredients.Add(new int[2]
                {
                  numArray[1],
                  ingredient[1]
                });
                break;
              }
            }
          }
        }
      }
      string str = "";
      switch (itemToCraft)
      {
        case 216:
          if (Game1.random.NextDouble() < 0.5)
          {
            ++itemToCraft;
            break;
          }
          break;
        case 291:
          str = this.items[locationOfIngredients[0][0]].Name;
          break;
      }
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1927", (object) craftingOrCooking));
      this.isCrafting = true;
      Game1.playSound("crafting");
      int index3 = -1;
      string message = Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1930");
      if (bigCraftable)
      {
        Game1.player.ActiveObject = new Object(Vector2.Zero, itemToCraft);
        Game1.player.showCarrying();
      }
      else if (itemToCraft < 0)
      {
        if (false)
          message = Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1935");
      }
      else
      {
        index3 = locationOfIngredients[0][0];
        if (locationOfIngredients[0][0] < 0)
        {
          for (int index4 = 0; index4 < this.items.Count; ++index4)
          {
            if (this.items[index4] == null)
            {
              index3 = index4;
              break;
            }
            if (index4 == (int) (NetFieldBase<int, NetInt>) this.maxItems - 1)
            {
              Game1.pauseThenMessage(this.craftingTime + ingredients.Count<int[]>() * 500, Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1936"), true);
              return false;
            }
          }
        }
        if (!str.Equals(""))
          this.items[index3] = (Item) new Object(Vector2.Zero, itemToCraft, str + " Bobber", true, true, false, false);
        else
          this.items[index3] = (Item) new Object(Vector2.Zero, itemToCraft, (string) null, true, true, false, false);
      }
      Game1.pauseThenMessage(this.craftingTime + ingredients.Count * 500, message, true);
      string lower = craftingOrCooking.ToLower();
      if (!(lower == "crafting"))
      {
        if (lower == "cooking")
          ++Game1.stats.ItemsCooked;
      }
      else
        ++Game1.stats.ItemsCrafted;
      foreach (int[] numArray in locationOfIngredients)
      {
        if (numArray[0] <= -100)
        {
          switch (numArray[0])
          {
            case -106:
              this.IridiumPieces -= numArray[1];
              continue;
            case -105:
              this.GoldPieces -= numArray[1];
              continue;
            case -104:
              this.CoalPieces -= numArray[1];
              continue;
            case -103:
              this.IronPieces -= numArray[1];
              continue;
            case -102:
              this.CopperPieces -= numArray[1];
              continue;
            case -101:
              this.stonePieces -= numArray[1];
              continue;
            case -100:
              this.WoodPieces -= numArray[1];
              continue;
            default:
              continue;
          }
        }
        else if (numArray[0] != index3)
          this.items[numArray[0]] = (Item) null;
      }
      return true;
    }

    private static bool containsIndex(List<int[]> locationOfIngredients, int index)
    {
      for (int index1 = 0; index1 < locationOfIngredients.Count; ++index1)
      {
        if (locationOfIngredients[index1][0] == index)
          return true;
      }
      return false;
    }

    public bool IsEquippedItem(Item item) => item != null && (item == Game1.player.hat.Value || item == Game1.player.shirtItem.Value || item == Game1.player.pantsItem.Value || item == Game1.player.leftRing.Value || item == Game1.player.rightRing.Value || item == Game1.player.boots.Value);

    public override bool collideWith(Object o)
    {
      base.collideWith(o);
      if (this.isRidingHorse() && o is Fence)
      {
        this.mount.squeezeForGate();
        switch (this.FacingDirection)
        {
          case 1:
            if ((double) o.tileLocation.X < (double) this.getTileX())
              return false;
            break;
          case 3:
            if ((double) o.tileLocation.X > (double) this.getTileX())
              return false;
            break;
        }
      }
      return true;
    }

    public void changeIntoSwimsuit()
    {
      this.bathingClothes.Value = true;
      this.Halt();
      this.setRunning(false);
      this.canOnlyWalk = true;
    }

    public void changeOutOfSwimSuit()
    {
      this.bathingClothes.Value = false;
      this.canOnlyWalk = false;
      this.Halt();
      this.FarmerSprite.StopAnimation();
      if (!Game1.options.autoRun)
        return;
      this.setRunning(true);
    }

    public bool ownsFurniture(string name)
    {
      foreach (string str in this.furnitureOwned)
      {
        if (str.Equals(name))
          return true;
      }
      return false;
    }

    public void showFrame(int frame, bool flip = false)
    {
      this.FarmerSprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(Convert.ToInt32(frame), 100, false, flip)
      }.ToArray());
      this.FarmerSprite.loop = true;
      this.FarmerSprite.PauseForSingleAnimation = true;
      this.Sprite.currentFrame = Convert.ToInt32(frame);
    }

    public void stopShowingFrame()
    {
      this.FarmerSprite.loop = false;
      this.FarmerSprite.PauseForSingleAnimation = false;
      this.completelyStopAnimatingOrDoingAction();
    }

    public Item addItemToInventory(Item item, List<Item> affected_items_list)
    {
      if (item == null)
        return (Item) null;
      if (item is SpecialItem)
        return item;
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.maxItems; ++index)
      {
        if (index < this.items.Count && this.items[index] != null && this.items[index].maximumStackSize() != -1 && this.items[index].Stack < this.items[index].maximumStackSize() && this.items[index].Name.Equals(item.Name) && (!(item is Object) || !(this.items[index] is Object) || (item as Object).quality.Value == (this.items[index] as Object).quality.Value && (item as Object).parentSheetIndex.Value == (this.items[index] as Object).parentSheetIndex.Value) && item.canStackWith((ISalable) this.items[index]))
        {
          int stack = this.items[index].addToStack(item);
          affected_items_list?.Add(this.items[index]);
          if (stack <= 0)
            return (Item) null;
          item.Stack = stack;
        }
      }
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.maxItems; ++index)
      {
        if (this.items.Count > index && this.items[index] == null)
        {
          this.items[index] = item;
          affected_items_list?.Add(this.items[index]);
          return (Item) null;
        }
      }
      return item;
    }

    public virtual void BeginSitting(ISittable furniture)
    {
      if (furniture == null || this.bathingClothes.Value || this.swimming.Value || this.isRidingHorse() || !this.CanMove || this.UsingTool || this.IsEmoting)
        return;
      Vector2? nullable = furniture.AddSittingFarmer(this);
      if (!nullable.HasValue)
        return;
      this.currentLocation.playSound("woodyStep");
      this.Halt();
      this.synchronizedJump(4f);
      this.FarmerSprite.StopAnimation();
      this.sittingFurniture = furniture;
      this.mapChairSitPosition.Value = new Vector2(-1f, -1f);
      if (this.sittingFurniture is MapSeat)
      {
        Vector2? sittingPosition = this.sittingFurniture.GetSittingPosition(this, true);
        if (sittingPosition.HasValue)
          this.mapChairSitPosition.Value = sittingPosition.Value;
      }
      this.isSitting.Value = true;
      this.LerpPosition(this.Position, new Vector2(nullable.Value.X * 64f, nullable.Value.Y * 64f), 0.15f);
      this.freezePause += 100;
    }

    public virtual void LerpPosition(Vector2 start_position, Vector2 end_position, float duration)
    {
      this.freezePause = (int) ((double) duration * 1000.0);
      this.lerpStartPosition = start_position;
      this.lerpEndPosition = end_position;
      this.lerpPosition = 0.0f;
      this.lerpDuration = duration;
    }

    public virtual void StopSitting(bool animate = true)
    {
      if (this.sittingFurniture == null)
        return;
      ISittable sittingFurniture = this.sittingFurniture;
      if (!animate)
      {
        this.mapChairSitPosition.Value = new Vector2(-1f, -1f);
        sittingFurniture.RemoveSittingFarmer(this);
      }
      bool flag1 = false;
      bool flag2 = false;
      Vector2 position = this.Position;
      if (sittingFurniture.IsSeatHere(this.currentLocation))
      {
        flag1 = true;
        List<Vector2> list = new List<Vector2>();
        Vector2 vector2_1;
        ref Vector2 local = ref vector2_1;
        Microsoft.Xna.Framework.Rectangle seatBounds1 = sittingFurniture.GetSeatBounds();
        double left = (double) seatBounds1.Left;
        seatBounds1 = sittingFurniture.GetSeatBounds();
        double top = (double) seatBounds1.Top;
        local = new Vector2((float) left, (float) top);
        if (sittingFurniture.IsSittingHere(this))
          vector2_1 = sittingFurniture.GetSittingPosition(this, true).Value;
        if (sittingFurniture.GetSittingDirection() == 2)
        {
          list.Add(vector2_1 + new Vector2(0.0f, 1f));
          this.SortSeatExitPositions(list, vector2_1 + new Vector2(1f, 0.0f), vector2_1 + new Vector2(-1f, 0.0f), vector2_1 + new Vector2(0.0f, -1f));
        }
        else if (sittingFurniture.GetSittingDirection() == 1)
        {
          list.Add(vector2_1 + new Vector2(1f, 0.0f));
          this.SortSeatExitPositions(list, vector2_1 + new Vector2(0.0f, -1f), vector2_1 + new Vector2(0.0f, 1f), vector2_1 + new Vector2(-1f, 0.0f));
        }
        else if (sittingFurniture.GetSittingDirection() == 3)
        {
          list.Add(vector2_1 + new Vector2(-1f, 0.0f));
          this.SortSeatExitPositions(list, vector2_1 + new Vector2(0.0f, 1f), vector2_1 + new Vector2(0.0f, -1f), vector2_1 + new Vector2(1f, 0.0f));
        }
        else if (sittingFurniture.GetSittingDirection() == 0)
        {
          list.Add(vector2_1 + new Vector2(0.0f, -1f));
          this.SortSeatExitPositions(list, vector2_1 + new Vector2(-1f, 0.0f), vector2_1 + new Vector2(1f, 0.0f), vector2_1 + new Vector2(0.0f, 1f));
        }
        Microsoft.Xna.Framework.Rectangle seatBounds2 = sittingFurniture.GetSeatBounds();
        seatBounds2.Inflate(1, 1);
        foreach (Vector2 vector2_2 in Utility.getBorderOfThisRectangle(seatBounds2))
          list.Add(vector2_2);
        foreach (Vector2 tileLocation in list)
        {
          this.setTileLocation(tileLocation);
          Object objectAtTile = this.currentLocation.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y);
          if (!this.currentLocation.isCollidingPosition(this.GetBoundingBox(), Game1.viewport, true, 0, false, (Character) this) && (objectAtTile == null || objectAtTile.isPassable()))
          {
            if (animate)
            {
              this.currentLocation.playSound("coin");
              this.synchronizedJump(4f);
              this.LerpPosition(vector2_1 * 64f, tileLocation * 64f, 0.15f);
            }
            flag2 = true;
            break;
          }
        }
      }
      if (!flag2)
      {
        if (animate)
          this.currentLocation.playSound("coin");
        this.Position = position;
        if (flag1)
        {
          Microsoft.Xna.Framework.Rectangle seatBounds = sittingFurniture.GetSeatBounds();
          seatBounds.X *= 64;
          seatBounds.Y *= 64;
          seatBounds.Width *= 64;
          seatBounds.Height *= 64;
          this.temporaryPassableTiles.Add(seatBounds);
        }
      }
      if (!animate)
      {
        this.sittingFurniture = (ISittable) null;
        this.isSitting.Value = false;
        this.Halt();
        this.showNotCarrying();
      }
      else
        this.isStopSitting = true;
      Game1.haltAfterCheck = false;
      this.yOffset = 0.0f;
      this.xOffset = 0.0f;
    }

    public void SortSeatExitPositions(List<Vector2> list, Vector2 a, Vector2 b, Vector2 c)
    {
      Vector2 mouse_pos = Utility.PointToVector2(Game1.getMousePosition(false)) + new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
      Vector2 zero = Vector2.Zero;
      GamePadState gamePadState;
      GamePadThumbSticks thumbSticks;
      if (!Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.moveUpButton))
      {
        if (Game1.options.gamepadControls)
        {
          gamePadState = Game1.input.GetGamePadState();
          thumbSticks = gamePadState.ThumbSticks;
          if ((double) thumbSticks.Left.Y <= 0.25)
          {
            gamePadState = Game1.input.GetGamePadState();
            if (gamePadState.IsButtonDown(Buttons.DPadUp))
              goto label_4;
          }
          else
            goto label_4;
        }
        if (!Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.moveDownButton))
        {
          if (Game1.options.gamepadControls)
          {
            gamePadState = Game1.input.GetGamePadState();
            thumbSticks = gamePadState.ThumbSticks;
            if ((double) thumbSticks.Left.Y >= -0.25)
            {
              gamePadState = Game1.input.GetGamePadState();
              if (!gamePadState.IsButtonDown(Buttons.DPadDown))
                goto label_10;
            }
          }
          else
            goto label_10;
        }
        ++zero.Y;
        goto label_10;
      }
label_4:
      --zero.Y;
label_10:
      if (!Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.moveLeftButton))
      {
        if (Game1.options.gamepadControls)
        {
          gamePadState = Game1.input.GetGamePadState();
          thumbSticks = gamePadState.ThumbSticks;
          if ((double) thumbSticks.Left.X >= -0.25)
          {
            gamePadState = Game1.input.GetGamePadState();
            if (gamePadState.IsButtonDown(Buttons.DPadLeft))
              goto label_14;
          }
          else
            goto label_14;
        }
        if (!Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.moveRightButton))
        {
          if (Game1.options.gamepadControls)
          {
            gamePadState = Game1.input.GetGamePadState();
            thumbSticks = gamePadState.ThumbSticks;
            if ((double) thumbSticks.Left.X <= 0.25)
            {
              gamePadState = Game1.input.GetGamePadState();
              if (!gamePadState.IsButtonDown(Buttons.DPadRight))
                goto label_20;
            }
          }
          else
            goto label_20;
        }
        ++zero.X;
        goto label_20;
      }
label_14:
      --zero.X;
label_20:
      if (zero != Vector2.Zero)
        mouse_pos = this.getStandingPosition() + zero * 64f;
      mouse_pos /= 64f;
      List<Vector2> collection = new List<Vector2>();
      collection.Add(a);
      collection.Add(b);
      collection.Add(c);
      collection.Sort((Comparison<Vector2>) ((d, e) => (d + new Vector2(0.5f, 0.5f) - mouse_pos).Length().CompareTo((e + new Vector2(0.5f, 0.5f) - mouse_pos).Length())));
      list.AddRange((IEnumerable<Vector2>) collection);
    }

    public virtual bool IsSitting() => this.isSitting.Value;

    public Item addItemToInventory(Item item) => this.addItemToInventory(item, (List<Item>) null);

    public bool isInventoryFull()
    {
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.maxItems; ++index)
      {
        if (this.items.Count > index && this.items[index] == null)
          return false;
      }
      return true;
    }

    public bool couldInventoryAcceptThisItem(Item item)
    {
      if (item is Object && (item as Object).IsRecipe || Utility.IsNormalObjectAtParentSheetIndex(item, 102) || Utility.IsNormalObjectAtParentSheetIndex(item, 73) || Utility.IsNormalObjectAtParentSheetIndex(item, 930) || Utility.IsNormalObjectAtParentSheetIndex(item, 858))
        return true;
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.maxItems; ++index)
      {
        if (this.items.Count > index && (this.items[index] == null || item is Object && this.items[index] is Object && this.items[index].Stack + item.Stack <= this.items[index].maximumStackSize() && (this.items[index] as Object).canStackWith((ISalable) item)))
          return true;
      }
      if (this.IsLocalPlayer && this.isInventoryFull() && Game1.hudMessages.Count<HUDMessage>() == 0)
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
      return false;
    }

    public bool couldInventoryAcceptThisObject(int index, int stack, int quality = 0)
    {
      if (index == 102 || index == 73 || index == 858)
        return true;
      for (int index1 = 0; index1 < (int) (NetFieldBase<int, NetInt>) this.maxItems; ++index1)
      {
        if (this.items.Count > index1 && (this.items[index1] == null || this.items[index1] is Object && this.items[index1].Stack + stack <= this.items[index1].maximumStackSize() && (this.items[index1] as Object).ParentSheetIndex == index && (int) (NetFieldBase<int, NetInt>) (this.items[index1] as Object).quality == quality))
          return true;
      }
      if (this.IsLocalPlayer && this.isInventoryFull() && Game1.hudMessages.Count<HUDMessage>() == 0)
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
      return false;
    }

    public bool hasItemOfType(string type)
    {
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.maxItems; ++index)
      {
        if (this.items.Count > index && this.items[index] is Object && (this.items[index] as Object).type.Equals((object) type))
          return true;
      }
      return false;
    }

    public NPC getSpouse() => this.isMarried() && this.spouse != null ? Game1.getCharacterFromName(this.spouse) : (NPC) null;

    public int freeSpotsInInventory()
    {
      int num = 0;
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.maxItems; ++index)
      {
        if (index < this.items.Count && this.items[index] == null)
          ++num;
      }
      return num;
    }

    public Item hasItemWithNameThatContains(string name)
    {
      for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.maxItems; ++index)
      {
        if (index < this.items.Count && this.items[index] != null && (NetFieldBase<string, NetString>) this.items[index].netName != (NetString) null && this.items[index].Name.Contains(name))
          return this.items[index];
      }
      return (Item) null;
    }

    public bool addItemToInventoryBool(Item item, bool makeActiveObject = false)
    {
      if (item == null)
        return false;
      int stack = item.Stack;
      Item obj1 = this.IsLocalPlayer ? this.addItemToInventory(item) : (Item) null;
      bool inventoryBool = obj1 == null || obj1.Stack != item.Stack || item is SpecialItem;
      if (item is Object)
        (item as Object).reloadSprite();
      bool flag = true;
      if (Utility.IsNormalObjectAtParentSheetIndex(item, 73))
        inventoryBool = true;
      if (Utility.IsNormalObjectAtParentSheetIndex(item, 858))
        inventoryBool = true;
      if (Utility.IsNormalObjectAtParentSheetIndex(item, 930))
        inventoryBool = true;
      if (Utility.IsNormalObjectAtParentSheetIndex(item, 102))
        inventoryBool = true;
      if (!inventoryBool || !this.IsLocalPlayer)
        return false;
      if (item != null)
      {
        if (this.IsLocalPlayer && !item.HasBeenInInventory)
        {
          if (item is SpecialItem)
          {
            (item as SpecialItem).actionWhenReceived(this);
            return true;
          }
          if (item is Object && (item as Object).specialItem)
          {
            if ((bool) (NetFieldBase<bool, NetBool>) (item as Object).bigCraftable || item is Furniture)
            {
              if (!this.specialBigCraftables.Contains((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex))
                this.specialBigCraftables.Add((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex);
            }
            else if (!this.specialItems.Contains((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex))
              this.specialItems.Add((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex);
          }
          if (item is Object && ((item as Object).Category == -2 || (item as Object).Type != null && (item as Object).Type.Contains("Mineral")) && !(bool) (NetFieldBase<bool, NetBool>) (item as Object).hasBeenPickedUpByFarmer)
            this.foundMineral((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex);
          else if (!(item is Furniture) && item is Object && (NetFieldBase<string, NetString>) (item as Object).type != (NetString) null && (item as Object).type.Contains("Arch") && !(bool) (NetFieldBase<bool, NetBool>) (item as Object).hasBeenPickedUpByFarmer)
            this.foundArtifact((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex, 1);
          if (Utility.IsNormalObjectAtParentSheetIndex(item, 73))
          {
            this.foundWalnut(stack);
            this.removeItemFromInventory(item);
          }
          if (Utility.IsNormalObjectAtParentSheetIndex(item, 858))
          {
            this.QiGems += stack;
            Game1.playSound("qi_shop_purchase");
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16), 100f, 1, 8, new Vector2(0.0f, -96f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(0.0f, -6f),
              acceleration = new Vector2(0.0f, 0.2f),
              stopAcceleratingWhenVelocityIsZero = true,
              attachedCharacter = (Character) this,
              positionFollowsAttachedCharacter = true
            });
            this.removeItemFromInventory(item);
          }
          if (Utility.IsNormalObjectAtParentSheetIndex(item, 930))
          {
            int number = 10;
            this.health = Math.Min(this.maxHealth, Game1.player.health + number);
            this.currentLocation.debris.Add(new Debris(number, new Vector2((float) Game1.player.getStandingX(), (float) Game1.player.getStandingY()), Color.Lime, 1f, (Character) this));
            Game1.playSound("healSound");
            this.removeItemFromInventory(item);
            flag = false;
          }
          if (Utility.IsNormalObjectAtParentSheetIndex(item, 102))
          {
            this.foundArtifact((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex, 1);
            this.removeItemFromInventory(item);
          }
          else
          {
            switch ((int) (NetFieldBase<int, NetInt>) item.parentSheetIndex)
            {
              case 378:
                Game1.stats.CopperFound += (uint) item.Stack;
                break;
              case 380:
                Game1.stats.IronFound += (uint) item.Stack;
                break;
              case 384:
                Game1.stats.GoldFound += (uint) item.Stack;
                break;
              case 386:
                Game1.stats.IridiumFound += (uint) item.Stack;
                break;
            }
          }
        }
        if (item is Object && !item.HasBeenInInventory)
          Utility.checkItemFirstInventoryAdd(item);
        Color color = Color.WhiteSmoke;
        string displayName = item.DisplayName;
        if (item is Object)
        {
          string type = (string) (NetFieldBase<string, NetString>) (item as Object).type;
          if (!(type == "Arch"))
          {
            if (!(type == "Fish"))
            {
              if (!(type == "Mineral"))
              {
                if (!(type == "Vegetable"))
                {
                  if (type == "Fruit")
                    color = Color.Pink;
                }
                else
                  color = Color.PaleGreen;
              }
              else
                color = Color.PaleVioletRed;
            }
            else
              color = Color.SkyBlue;
          }
          else
          {
            color = Color.Tan;
            displayName += Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1954");
          }
        }
        if (flag && (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is ItemGrabMenu)))
          Game1.addHUDMessage(new HUDMessage(displayName, Math.Max(1, item.Stack), true, color, item));
        if (this.freezePause <= 0)
          this.mostRecentlyGrabbedItem = item;
        if (obj1 != null & makeActiveObject && item.Stack <= 1)
        {
          int indexOfInventoryItem = this.getIndexOfInventoryItem(item);
          Item obj2 = this.items[(int) (NetFieldBase<int, NetInt>) this.currentToolIndex];
          this.items[(int) (NetFieldBase<int, NetInt>) this.currentToolIndex] = this.items[indexOfInventoryItem];
          this.items[indexOfInventoryItem] = obj2;
        }
      }
      return inventoryBool;
    }

    public int getIndexOfInventoryItem(Item item)
    {
      for (int index = 0; index < this.items.Count; ++index)
      {
        if (this.items[index] == item || this.items[index] != null && item != null && item.canStackWith((ISalable) this.items[index]))
          return index;
      }
      return -1;
    }

    public void reduceActiveItemByOne()
    {
      if (this.CurrentItem == null || --this.CurrentItem.Stack > 0)
        return;
      this.removeItemFromInventory(this.CurrentItem);
      this.showNotCarrying();
    }

    public bool removeItemsFromInventory(int index, int stack)
    {
      if (this.hasItemInInventory(index, stack))
      {
        if (index == 858 && this.QiGems >= stack)
        {
          this.QiGems -= stack;
          return true;
        }
        if (index == 73 && Game1.netWorldState.Value.GoldenWalnuts.Value >= stack)
        {
          Game1.netWorldState.Value.GoldenWalnuts.Value -= stack;
          return true;
        }
        for (int index1 = 0; index1 < this.items.Count; ++index1)
        {
          if (this.items[index1] != null && this.items[index1] is Object && (int) (NetFieldBase<int, NetInt>) (this.items[index1] as Object).parentSheetIndex == index)
          {
            if (this.items[index1].Stack > stack)
            {
              this.items[index1].Stack -= stack;
              return true;
            }
            stack -= this.items[index1].Stack;
            this.items[index1] = (Item) null;
          }
          if (stack <= 0)
            return true;
        }
      }
      return false;
    }

    public void ReequipEnchantments()
    {
      if (Game1.player.CurrentTool == null || Game1.player.CurrentTool == null)
        return;
      foreach (BaseEnchantment enchantment in Game1.player.CurrentTool.enchantments)
        enchantment.OnEquip(this);
    }

    public Item addItemToInventory(Item item, int position)
    {
      if (item != null && item is Object && (item as Object).specialItem)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) (item as Object).bigCraftable)
        {
          if (!this.specialBigCraftables.Contains((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex))
            this.specialBigCraftables.Add((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex);
        }
        else if (!this.specialItems.Contains((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex))
          this.specialItems.Add((int) (NetFieldBase<int, NetInt>) (item as Object).parentSheetIndex);
      }
      if (position < 0 || position >= this.items.Count)
        return item;
      if (this.items[position] == null)
      {
        this.items[position] = item;
        return (Item) null;
      }
      if (item != null && this.items[position].maximumStackSize() != -1 && this.items[position].Name.Equals(item.Name) && this.items[position].ParentSheetIndex == item.ParentSheetIndex && (!(item is Object) || !(this.items[position] is Object) || (NetFieldBase<int, NetInt>) (item as Object).quality == (this.items[position] as Object).quality))
      {
        int stack = this.items[position].addToStack(item);
        if (stack <= 0)
          return (Item) null;
        item.Stack = stack;
        return item;
      }
      Item inventory = this.items[position];
      this.items[position] = item;
      return inventory;
    }

    public void removeItemFromInventory(Item which)
    {
      int index = this.items.IndexOf(which);
      if (index < 0 || index >= this.items.Count)
        return;
      this.items[index].actionWhenStopBeingHeld(this);
      this.items[index] = (Item) null;
    }

    public Item removeItemFromInventory(int whichItemIndex)
    {
      if (whichItemIndex < 0 || whichItemIndex >= this.items.Count || this.items[whichItemIndex] == null)
        return (Item) null;
      Item obj = this.items[whichItemIndex];
      this.items[whichItemIndex] = (Item) null;
      obj.actionWhenStopBeingHeld(this);
      return obj;
    }

    public bool isMarried()
    {
      if (this.team.IsMarried(this.UniqueMultiplayerID))
        return true;
      return this.spouse != null && this.friendshipData.ContainsKey(this.spouse) && this.friendshipData[this.spouse].IsMarried();
    }

    public bool isEngaged()
    {
      if (this.team.IsEngaged(this.UniqueMultiplayerID))
        return true;
      return this.spouse != null && this.friendshipData.ContainsKey(this.spouse) && this.friendshipData[this.spouse].IsEngaged();
    }

    public void removeFirstOfThisItemFromInventory(int parentSheetIndexOfItem)
    {
      if (this.ActiveObject != null && this.ActiveObject.ParentSheetIndex == parentSheetIndexOfItem)
      {
        --this.ActiveObject.Stack;
        if (this.ActiveObject.Stack > 0)
          return;
        this.ActiveObject = (Object) null;
        this.showNotCarrying();
      }
      else
      {
        for (int index = 0; index < this.items.Count; ++index)
        {
          if (this.items[index] != null && this.items[index] is Object && this.items[index].ParentSheetIndex == parentSheetIndexOfItem)
          {
            --this.items[index].Stack;
            if (this.items[index].Stack > 0)
              break;
            this.items[index] = (Item) null;
            break;
          }
        }
      }
    }

    public void changeShirt(int whichShirt, bool is_customization_screen = false)
    {
      if (is_customization_screen)
      {
        int num = whichShirt - (int) (NetFieldBase<int, NetInt>) this.shirt;
        if (whichShirt < 0)
          whichShirt = 111;
        else if (whichShirt > 111)
          whichShirt = 0;
        if (whichShirt == (int) sbyte.MaxValue && !this.eventsSeen.Contains(3917601))
        {
          whichShirt = (int) sbyte.MaxValue + num;
          if (whichShirt > FarmerRenderer.shirtsTexture.Height / 32 * 16 - 1)
            whichShirt = 0;
        }
      }
      this.shirt.Set(whichShirt);
      this.FarmerRenderer.changeShirt(whichShirt);
    }

    public void changePantStyle(int whichPants, bool is_customization_screen = false)
    {
      if (is_customization_screen)
      {
        int pants = (int) (NetFieldBase<int, NetInt>) this.pants;
        if (whichPants < 0)
          whichPants = 3;
        else if (whichPants > 3)
          whichPants = 0;
      }
      this.pants.Set(whichPants);
      this.FarmerRenderer.changePants(whichPants);
    }

    public void ConvertClothingOverrideToClothesItems()
    {
      if (this.pants.Value >= 0)
      {
        Clothing clothing = new Clothing(this.pants.Value);
        clothing.clothesColor.Set((Color) (NetFieldBase<Color, NetColor>) this.pantsColor);
        this.pantsItem.Value = clothing;
        this.pants.Value = -1;
      }
      if (this.shirt.Value < 0)
        return;
      this.shirtItem.Value = new Clothing(this.shirt.Value + 1000);
      this.shirt.Value = -1;
    }

    public static Dictionary<int, string> GetHairStyleMetadataFile()
    {
      if (Farmer.hairStyleMetadataFile == null)
        Farmer.hairStyleMetadataFile = Game1.content.Load<Dictionary<int, string>>("Data\\HairData");
      return Farmer.hairStyleMetadataFile;
    }

    public static HairStyleMetadata GetHairStyleMetadata(int hair_index)
    {
      Farmer.GetHairStyleMetadataFile();
      if (Farmer.hairStyleMetadata.ContainsKey(hair_index))
        return Farmer.hairStyleMetadata[hair_index];
      HairStyleMetadata hairStyleMetadata1 = (HairStyleMetadata) null;
      try
      {
        if (Farmer.hairStyleMetadataFile.ContainsKey(hair_index))
        {
          string[] strArray = Farmer.hairStyleMetadataFile[hair_index].Split('/');
          HairStyleMetadata hairStyleMetadata2 = new HairStyleMetadata();
          hairStyleMetadata2.texture = Game1.content.Load<Texture2D>("Characters\\Farmer\\" + strArray[0]);
          hairStyleMetadata2.tileX = int.Parse(strArray[1]);
          hairStyleMetadata2.tileY = int.Parse(strArray[2]);
          hairStyleMetadata2.usesUniqueLeftSprite = strArray.Length > 3 && strArray[3].ToLower() == "true";
          if (strArray.Length > 4)
            hairStyleMetadata2.coveredIndex = int.Parse(strArray[4]);
          hairStyleMetadata2.isBaldStyle = strArray.Length > 5 && strArray[5].ToLower() == "true";
          hairStyleMetadata1 = hairStyleMetadata2;
        }
      }
      catch (Exception ex)
      {
      }
      Farmer.hairStyleMetadata[hair_index] = hairStyleMetadata1;
      return hairStyleMetadata1;
    }

    public static List<int> GetAllHairstyleIndices()
    {
      if (Farmer.allHairStyleIndices != null)
        return Farmer.allHairStyleIndices;
      Farmer.GetHairStyleMetadataFile();
      Farmer.allHairStyleIndices = new List<int>();
      int num = FarmerRenderer.hairStylesTexture.Height / 96 * 8;
      for (int index = 0; index < num; ++index)
        Farmer.allHairStyleIndices.Add(index);
      foreach (int key in Farmer.hairStyleMetadataFile.Keys)
      {
        if (key >= 0 && !Farmer.allHairStyleIndices.Contains(key))
          Farmer.allHairStyleIndices.Add(key);
      }
      Farmer.allHairStyleIndices.Sort();
      return Farmer.allHairStyleIndices;
    }

    public static int GetLastHairStyle() => Farmer.GetAllHairstyleIndices()[Farmer.GetAllHairstyleIndices().Count<int>() - 1];

    public void changeHairStyle(int whichHair)
    {
      int num = this.isBald() ? 1 : 0;
      if (Farmer.GetHairStyleMetadata(whichHair) != null)
      {
        this.hair.Set(whichHair);
      }
      else
      {
        if (whichHair < 0)
          whichHair = Farmer.GetLastHairStyle();
        else if (whichHair > Farmer.GetLastHairStyle())
          whichHair = 0;
        this.hair.Set(whichHair);
      }
      if (this.IsBaldHairStyle(whichHair))
        this.FarmerRenderer.textureName.Set(this.getTexture());
      if (num == 0 || this.isBald())
        return;
      this.FarmerRenderer.textureName.Set(this.getTexture());
    }

    public virtual bool IsBaldHairStyle(int style)
    {
      if (Farmer.GetHairStyleMetadata(this.hair.Value) != null)
        return Farmer.GetHairStyleMetadata(this.hair.Value).isBaldStyle;
      switch (style)
      {
        case 49:
        case 50:
        case 51:
        case 52:
        case 53:
        case 54:
        case 55:
          return true;
        default:
          return false;
      }
    }

    private bool isBald() => this.IsBaldHairStyle(this.getHair());

    public void changeShoeColor(int which)
    {
      this.FarmerRenderer.recolorShoes(which);
      this.shoes.Set(which);
    }

    public void changeHairColor(Color c) => this.hairstyleColor.Set(c);

    public void changePants(Color color) => this.pantsColor.Set(color);

    public void changeHat(int newHat)
    {
      if (newHat < 0)
        this.hat.Value = (Hat) null;
      else
        this.hat.Value = new Hat(newHat);
    }

    public void changeAccessory(int which)
    {
      if (which < -1)
        which = 18;
      if (which < -1)
        return;
      if (which >= 19)
        which = -1;
      this.accessory.Set(which);
    }

    public void changeSkinColor(int which, bool force = false)
    {
      if (which < 0)
        which = 23;
      else if (which >= 24)
        which = 0;
      this.skin.Set(this.FarmerRenderer.recolorSkin(which, force));
    }

    public bool hasDarkSkin() => (int) (NetFieldBase<int, NetInt>) this.skin >= 4 && (int) (NetFieldBase<int, NetInt>) this.skin <= 8 || (int) (NetFieldBase<int, NetInt>) this.skin == 14;

    public void changeEyeColor(Color c)
    {
      this.newEyeColor.Set(c);
      this.FarmerRenderer.recolorEyes(c);
    }

    public int getHair(bool ignore_hat = false)
    {
      if (this.hat.Value != null && !(bool) (NetFieldBase<bool, NetBool>) this.bathingClothes && !ignore_hat)
      {
        switch ((Hat.HairDrawType) this.hat.Value.hairDrawType.Value)
        {
          case Hat.HairDrawType.DrawObscuredHair:
            switch ((int) (NetFieldBase<int, NetInt>) this.hair)
            {
              case 1:
              case 5:
              case 6:
              case 9:
              case 11:
              case 17:
              case 20:
              case 23:
              case 24:
              case 25:
              case 27:
              case 28:
              case 29:
              case 30:
              case 32:
              case 33:
              case 34:
              case 36:
              case 39:
              case 41:
              case 43:
              case 44:
              case 45:
              case 46:
              case 47:
                return (int) (NetFieldBase<int, NetInt>) this.hair;
              case 3:
                return 11;
              case 18:
              case 19:
              case 21:
              case 31:
                return 23;
              case 42:
                return 46;
              case 48:
                return 6;
              case 49:
                return 52;
              case 50:
              case 51:
              case 52:
              case 53:
              case 54:
              case 55:
                return (int) (NetFieldBase<int, NetInt>) this.hair;
              default:
                return (int) (NetFieldBase<int, NetInt>) this.hair >= 16 ? 30 : 7;
            }
          case Hat.HairDrawType.HideHair:
            return -1;
        }
      }
      return (int) (NetFieldBase<int, NetInt>) this.hair;
    }

    public void changeGender(bool male)
    {
      if (male)
      {
        this.IsMale = true;
        this.FarmerRenderer.textureName.Set(this.getTexture());
        this.FarmerRenderer.heightOffset.Set(0);
      }
      else
      {
        this.IsMale = false;
        this.FarmerRenderer.heightOffset.Set(4);
        this.FarmerRenderer.textureName.Set(this.getTexture());
      }
      this.changeShirt((int) (NetFieldBase<int, NetInt>) this.shirt);
    }

    public void changeFriendship(int amount, NPC n)
    {
      if (n == null || !(n is Child) && !n.isVillager() || amount > 0 && n.Name.Equals("Dwarf") && !this.canUnderstandDwarves)
        return;
      if (this.friendshipData.ContainsKey(n.Name))
      {
        if (n.isDivorcedFrom(this) && amount > 0)
          return;
        this.friendshipData[n.Name].Points = Math.Max(0, Math.Min(this.friendshipData[n.Name].Points + amount, (Utility.GetMaximumHeartsForCharacter((Character) n) + 1) * 250 - 1));
        if ((bool) (NetFieldBase<bool, NetBool>) n.datable && this.friendshipData[n.Name].Points >= 2000 && !this.hasOrWillReceiveMail("Bouquet"))
          Game1.addMailForTomorrow("Bouquet");
        if ((bool) (NetFieldBase<bool, NetBool>) n.datable && this.friendshipData[n.Name].Points >= 2500 && !this.hasOrWillReceiveMail("SeaAmulet"))
          Game1.addMailForTomorrow("SeaAmulet");
        if (this.friendshipData[n.Name].Points < 0)
          this.friendshipData[n.Name].Points = 0;
      }
      else
        Game1.debugOutput = "Tried to change friendship for a friend that wasn't there.";
      Game1.stats.checkForFriendshipAchievements();
    }

    public bool knowsRecipe(string name) => this.craftingRecipes.Keys.Contains(name.Replace(" Recipe", "")) || this.cookingRecipes.Keys.Contains(name.Replace(" Recipe", ""));

    public Vector2 getUniformPositionAwayFromBox(int direction, int distance)
    {
      switch (this.FacingDirection)
      {
        case 0:
          return new Vector2((float) this.GetBoundingBox().Center.X, (float) (this.GetBoundingBox().Y - distance));
        case 1:
          return new Vector2((float) (this.GetBoundingBox().Right + distance), (float) this.GetBoundingBox().Center.Y);
        case 2:
          return new Vector2((float) this.GetBoundingBox().Center.X, (float) (this.GetBoundingBox().Bottom + distance));
        case 3:
          return new Vector2((float) (this.GetBoundingBox().X - distance), (float) this.GetBoundingBox().Center.Y);
        default:
          return Vector2.Zero;
      }
    }

    public bool hasTalkedToFriendToday(string npcName) => this.friendshipData.ContainsKey(npcName) && this.friendshipData[npcName].TalkedToToday;

    public void talkToFriend(NPC n, int friendshipPointChange = 20)
    {
      if (!this.friendshipData.ContainsKey(n.Name) || this.friendshipData[n.Name].TalkedToToday)
        return;
      this.changeFriendship(friendshipPointChange, n);
      this.friendshipData[n.Name].TalkedToToday = true;
    }

    public void moveRaft(GameLocation currentLocation, GameTime time)
    {
      float num = 0.2f;
      if (this.CanMove && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveUpButton))
      {
        this.yVelocity = Math.Max(this.yVelocity - num, (float) ((double) Math.Abs(this.xVelocity) / 2.0 - 3.0));
        this.faceDirection(0);
      }
      if (this.CanMove && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveRightButton))
      {
        this.xVelocity = Math.Min(this.xVelocity + num, (float) (3.0 - (double) Math.Abs(this.yVelocity) / 2.0));
        this.faceDirection(1);
      }
      if (this.CanMove && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveDownButton))
      {
        this.yVelocity = Math.Min(this.yVelocity + num, (float) (3.0 - (double) Math.Abs(this.xVelocity) / 2.0));
        this.faceDirection(2);
      }
      if (this.CanMove && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveLeftButton))
      {
        this.xVelocity = Math.Max(this.xVelocity - num, (float) ((double) Math.Abs(this.yVelocity) / 2.0 - 3.0));
        this.faceDirection(3);
      }
      Microsoft.Xna.Framework.Rectangle position = new Microsoft.Xna.Framework.Rectangle((int) this.Position.X, (int) ((double) this.Position.Y + 64.0 + 16.0), 64, 64);
      position.X += (int) Math.Ceiling((double) this.xVelocity);
      if (!currentLocation.isCollidingPosition(position, Game1.viewport, true))
        this.position.X += this.xVelocity;
      position.X -= (int) Math.Ceiling((double) this.xVelocity);
      position.Y += (int) Math.Floor((double) this.yVelocity);
      if (!currentLocation.isCollidingPosition(position, Game1.viewport, true))
        this.position.Y += this.yVelocity;
      if ((double) this.xVelocity != 0.0 || (double) this.yVelocity != 0.0)
      {
        this.raftPuddleCounter -= time.ElapsedGameTime.Milliseconds;
        if (this.raftPuddleCounter <= 0)
        {
          this.raftPuddleCounter = 250;
          currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2((float) position.X, (float) (position.Y - 64)), false, Game1.random.NextDouble() < 0.5, 1f / 1000f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
          if (Game1.random.NextDouble() < 0.6)
            Game1.playSound("wateringCan");
          if (Game1.random.NextDouble() < 0.6)
            this.raftBobCounter /= 2;
        }
      }
      this.raftBobCounter -= time.ElapsedGameTime.Milliseconds;
      if (this.raftBobCounter <= 0)
      {
        this.raftBobCounter = Game1.random.Next(15, 28) * 100;
        if ((double) this.yOffset <= 0.0)
        {
          this.yOffset = 4f;
          currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2((float) position.X, (float) (position.Y - 64)), false, Game1.random.NextDouble() < 0.5, 1f / 1000f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
        }
        else
          this.yOffset = 0.0f;
      }
      if ((double) this.xVelocity > 0.0)
        this.xVelocity = Math.Max(0.0f, this.xVelocity - num / 2f);
      else if ((double) this.xVelocity < 0.0)
        this.xVelocity = Math.Min(0.0f, this.xVelocity + num / 2f);
      if ((double) this.yVelocity > 0.0)
      {
        this.yVelocity = Math.Max(0.0f, this.yVelocity - num / 2f);
      }
      else
      {
        if ((double) this.yVelocity >= 0.0)
          return;
        this.yVelocity = Math.Min(0.0f, this.yVelocity + num / 2f);
      }
    }

    public void warpFarmer(Warp w, int warp_collide_direction)
    {
      if (w == null || Game1.eventUp)
        return;
      this.Halt();
      int targetX = w.TargetX;
      int targetY = w.TargetY;
      if (this.isRidingHorse())
      {
        if (warp_collide_direction == 3)
          Game1.nextFarmerWarpOffsetX = -1;
        if (warp_collide_direction == 0)
          Game1.nextFarmerWarpOffsetY = -1;
      }
      Game1.warpFarmer(w.TargetName, targetX, targetY, (bool) (NetFieldBase<bool, NetBool>) w.flipFarmer);
    }

    public void warpFarmer(Warp w) => this.warpFarmer(w, -1);

    public void startToPassOut() => this.passOutEvent.Fire();

    private void performPassOut()
    {
      if (this.isEmoteAnimating)
        this.EndEmoteAnimation();
      if (!this.swimming.Value && this.bathingClothes.Value)
        this.bathingClothes.Value = false;
      if (this.passedOut || this.FarmerSprite.isPassingOut())
        return;
      this.faceDirection(2);
      this.completelyStopAnimatingOrDoingAction();
      this.animateOnce(293);
    }

    public static void passOutFromTired(Farmer who)
    {
      if (!who.IsLocalPlayer)
        return;
      if (who.IsSitting())
        who.StopSitting(false);
      if (who.isRidingHorse())
        who.mount.dismount();
      if (Game1.activeClickableMenu != null)
      {
        Game1.activeClickableMenu.emergencyShutDown();
        Game1.exitActiveMenu();
      }
      who.completelyStopAnimatingOrDoingAction();
      if ((bool) (NetFieldBase<bool, NetBool>) who.bathingClothes)
        who.changeOutOfSwimSuit();
      who.swimming.Value = false;
      who.CanMove = false;
      who.FarmerSprite.setCurrentSingleFrame(5, (short) 3000);
      who.FarmerSprite.PauseForSingleAnimation = true;
      if (who == Game1.player && (FarmerTeam.SleepAnnounceModes) (NetFieldBase<FarmerTeam.SleepAnnounceModes, NetEnum<FarmerTeam.SleepAnnounceModes>>) Game1.player.team.sleepAnnounceMode != FarmerTeam.SleepAnnounceModes.Off)
      {
        string str = "PassedOut";
        string messageKey = "PassedOut_" + who.currentLocation.Name.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
        if (Game1.content.LoadStringReturnNullIfNotFound("Strings\\UI:Chat_" + messageKey) != null)
        {
          Game1.multiplayer.globalChatInfoMessage(messageKey, Game1.player.displayName);
        }
        else
        {
          int num = 0;
          for (int index = 0; index < 2; ++index)
          {
            if (Game1.random.NextDouble() < 0.25)
              ++num;
          }
          Game1.multiplayer.globalChatInfoMessage(str + num.ToString(), Game1.player.displayName);
        }
      }
      if (Game1.currentLocation is FarmHouse)
      {
        who.lastSleepLocation.Value = Game1.currentLocation.NameOrUniqueName;
        who.lastSleepPoint.Value = (Game1.currentLocation as FarmHouse).GetPlayerBedSpot();
      }
      Game1.multiplayer.sendPassoutRequest();
    }

    public static void performPassoutWarp(
      Farmer who,
      string bed_location_name,
      Point bed_point,
      bool has_bed)
    {
      GameLocation passOutLocation = who.currentLocationRef.Value;
      Vector2 vector2 = Utility.PointToVector2(bed_point) * 64f;
      Vector2 bed_tile = new Vector2((float) ((int) vector2.X / 64), (float) ((int) vector2.Y / 64));
      Vector2 bed_sleep_position = vector2;
      LocationRequest.Callback callback = (LocationRequest.Callback) (() =>
      {
        who.Position = bed_sleep_position;
        who.currentLocation.lastTouchActionLocation = bed_tile;
        if (who.NetFields.Root != null)
          (who.NetFields.Root as NetRoot<Farmer>).CancelInterpolation();
        if (!Game1.IsMultiplayer || Game1.timeOfDay >= 2600)
          Game1.PassOutNewDay();
        Game1.changeMusicTrack("none");
        if (passOutLocation is FarmHouse || passOutLocation is IslandFarmHouse || passOutLocation is Cellar)
          return;
        int num = Math.Min(1000, who.Money / 10);
        string str = "";
        Random random1 = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + (int) who.UniqueMultiplayerID);
        if (passOutLocation is IslandLocation)
        {
          num = Math.Min(2500, who.Money);
          if (bed_location_name == "FarmHouse")
          {
            str = "passedOutIsland";
            if (Game1.player.friendshipData.ContainsKey("Leo") && Game1.random.NextDouble() < 0.5)
            {
              str = "passedOutIsland_Leo";
              num = 0;
            }
          }
        }
        else if (random1.Next(0, 3) == 0 && Game1.MasterPlayer.hasCompletedCommunityCenter() && !Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
        {
          str = "passedOut4";
          num = 0;
        }
        else
        {
          Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
          List<int> list = new List<int>((IEnumerable<int>) new int[3]
          {
            1,
            2,
            3
          });
          if (who.getSpouse() != null && who.getSpouse().Name.Equals("Harvey"))
            list.Remove(3);
          if (Game1.MasterPlayer.hasCompletedCommunityCenter() && !Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
            list.Remove(1);
          int random2 = Utility.GetRandom<int>(list, random1);
          if (dictionary.ContainsKey("passedOut" + random2.ToString() + "_" + (num > 0 ? "Billed" : "NotBilled") + "_" + (who.IsMale ? "Male" : "Female")))
            str = "passedOut" + random2.ToString() + "_" + (num > 0 ? "Billed" : "NotBilled") + "_" + (who.IsMale ? "Male" : "Female") + " " + num.ToString();
          else if (dictionary.ContainsKey("passedOut" + random2.ToString() + "_" + (num > 0 ? "Billed" : "NotBilled")))
            str = "passedOut" + random2.ToString() + "_" + (num > 0 ? "Billed" : "NotBilled") + " " + num.ToString();
          else
            str = !dictionary.ContainsKey("passedOut" + random2.ToString()) ? "passedOut2 " + num.ToString() : "passedOut" + random2.ToString() + " " + num.ToString();
        }
        if (num > 0)
          who.Money -= num;
        if (!(str != ""))
          return;
        who.mailForTomorrow.Add(str);
      });
      if (!(bool) (NetFieldBase<bool, NetBool>) who.isInBed)
      {
        LocationRequest locationRequest = Game1.getLocationRequest(bed_location_name);
        Game1.warpFarmer(locationRequest, (int) vector2.X / 64, (int) vector2.Y / 64, 2);
        locationRequest.OnWarp += callback;
        who.FarmerSprite.setCurrentSingleFrame(5, (short) 3000);
        who.FarmerSprite.PauseForSingleAnimation = true;
      }
      else
        callback();
    }

    public static void doSleepEmote(Farmer who)
    {
      who.doEmote(24);
      who.yJumpVelocity = -2f;
    }

    public override Microsoft.Xna.Framework.Rectangle GetBoundingBox()
    {
      if (this.mount != null && !(bool) (NetFieldBase<bool, NetBool>) this.mount.dismounting)
        return this.mount.GetBoundingBox();
      Vector2 position = this.Position;
      return new Microsoft.Xna.Framework.Rectangle((int) position.X + 8, (int) position.Y + this.Sprite.getHeight() - 32, 48, 32);
    }

    public string getPetName()
    {
      foreach (NPC character in Game1.getFarm().characters)
      {
        if (character is Pet)
          return character.Name;
      }
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        foreach (NPC character in Utility.getHomeOfFarmer(allFarmer).characters)
        {
          if (character is Pet)
            return character.Name;
        }
      }
      return "the Farm";
    }

    public Pet getPet()
    {
      foreach (NPC character in Game1.getFarm().characters)
      {
        if (character is Pet)
          return character as Pet;
      }
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        foreach (NPC character in Utility.getHomeOfFarmer(allFarmer).characters)
        {
          if (character is Pet)
            return character as Pet;
        }
      }
      return (Pet) null;
    }

    public string getPetDisplayName()
    {
      foreach (NPC character in Game1.getFarm().characters)
      {
        if (character is Pet)
          return character.displayName;
      }
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        foreach (NPC character in Utility.getHomeOfFarmer(allFarmer).characters)
        {
          if (character is Pet)
            return character.displayName;
        }
      }
      return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1972");
    }

    public bool hasPet()
    {
      foreach (NPC character in Game1.getFarm().characters)
      {
        if (character is Pet)
          return true;
      }
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        foreach (NPC character in Utility.getHomeOfFarmer(allFarmer).characters)
        {
          if (character is Pet)
            return true;
        }
      }
      return false;
    }

    public void UpdateClothing() => this.FarmerRenderer.MarkSpriteDirty();

    public int GetPantsIndex()
    {
      if (this.pants.Value >= 0)
        return this.pants.Value;
      if (this.pantsItem.Value == null)
        return 14;
      return (bool) (NetFieldBase<bool, NetBool>) this.isMale || (int) (NetFieldBase<int, NetInt>) this.pantsItem.Value.indexInTileSheetFemale < 0 ? (int) (NetFieldBase<int, NetInt>) this.pantsItem.Value.indexInTileSheetMale : (int) (NetFieldBase<int, NetInt>) this.pantsItem.Value.indexInTileSheetFemale;
    }

    public int GetShirtIndex()
    {
      if (this.shirt.Value >= 0)
        return this.shirt.Value;
      return this.shirtItem.Value != null ? ((bool) (NetFieldBase<bool, NetBool>) this.isMale || (int) (NetFieldBase<int, NetInt>) this.shirtItem.Value.indexInTileSheetFemale < 0 ? (int) (NetFieldBase<int, NetInt>) this.shirtItem.Value.indexInTileSheetMale : (int) (NetFieldBase<int, NetInt>) this.shirtItem.Value.indexInTileSheetFemale) : (this.IsMale ? 209 : 41);
    }

    public List<string> GetShirtExtraData()
    {
      if (this.shirt.Value > 0)
        return new Clothing(this.shirt.Value + 1000).GetOtherData();
      return this.shirtItem.Value != null ? this.shirtItem.Value.GetOtherData() : new List<string>();
    }

    public Color GetShirtColor()
    {
      if (this.shirt.Value >= 0)
        return Color.White;
      if (this.shirtItem.Value == null)
        return this.DEFAULT_SHIRT_COLOR;
      return (bool) (NetFieldBase<bool, NetBool>) this.shirtItem.Value.isPrismatic ? Utility.GetPrismaticColor() : this.shirtItem.Value.clothesColor.Value;
    }

    public Color GetPantsColor()
    {
      if (this.pants.Value >= 0)
        return this.pantsColor.Value;
      if (this.pantsItem.Value == null)
        return Color.White;
      return (bool) (NetFieldBase<bool, NetBool>) this.pantsItem.Value.isPrismatic ? Utility.GetPrismaticColor() : this.pantsItem.Value.clothesColor.Value;
    }

    public bool movedDuringLastTick() => !this.Position.Equals(this.lastPosition);

    public int CompareTo(object obj) => ((Farmer) obj).saveTime - this.saveTime;

    public virtual void SetOnBridge(bool val)
    {
      if (Game1.player.onBridge.Value == val)
        return;
      Game1.player.onBridge.Value = val;
      if (!(bool) (NetFieldBase<bool, NetBool>) Game1.player.onBridge)
        return;
      Game1.player.showNotCarrying();
    }

    public float getDrawLayer()
    {
      if (this.onBridge.Value)
        return (float) ((double) this.getStandingY() / 10000.0 + (double) this.drawLayerDisambiguator + 0.0255999993532896);
      return this.IsSitting() && (double) this.mapChairSitPosition.Value.X != -1.0 && (double) this.mapChairSitPosition.Value.Y != -1.0 ? (float) (((double) this.mapChairSitPosition.Value.Y + 1.0) * 64.0 / 10000.0) : (float) this.getStandingY() / 10000f + this.drawLayerDisambiguator;
    }

    public override void draw(SpriteBatch b)
    {
      if (this.currentLocation == null || !this.currentLocation.Equals(Game1.currentLocation) && !this.IsLocalPlayer && !Game1.currentLocation.Name.Equals("Temp") && !this.isFakeEventActor || (bool) (NetFieldBase<bool, NetBool>) this.hidden && (this.currentLocation.currentEvent == null || this != this.currentLocation.currentEvent.farmer) && (!this.IsLocalPlayer || Game1.locationRequest == null) || this.viewingLocation.Value != null && this.IsLocalPlayer)
        return;
      if (this.isRidingHorse())
      {
        this.mount.SyncPositionToRider();
        this.mount.draw(b);
      }
      float drawLayer = this.getDrawLayer();
      Vector2 origin = new Vector2(this.xOffset, (float) (((double) this.yOffset + 128.0 - (double) (this.GetBoundingBox().Height / 2)) / 4.0 + 4.0));
      this.numUpdatesSinceLastDraw = 0;
      PropertyValue propertyValue = (PropertyValue) null;
      Tile tile = Game1.currentLocation.Map.GetLayer("Buildings").PickTile(new Location(this.getStandingX(), this.getStandingY()), Game1.viewport.Size);
      if (this.isGlowing && this.coloredBorder)
        b.Draw(this.Sprite.Texture, new Vector2(this.getLocalPosition(Game1.viewport).X - 4f, this.getLocalPosition(Game1.viewport).Y - 4f), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, 0.0f, Vector2.Zero, 1.1f, SpriteEffects.None, Math.Max(0.0f, drawLayer - 1f / 1000f));
      else if (this.isGlowing && !this.coloredBorder)
        this.FarmerRenderer.draw(b, this.FarmerSprite, this.FarmerSprite.SourceRect, this.getLocalPosition(Game1.viewport) + this.jitter + new Vector2(0.0f, (float) this.yJumpOffset), origin, Math.Max(0.0f, drawLayer + 0.00011f), this.glowingColor * this.glowingTransparency, this.rotation, this);
      tile?.TileIndexProperties.TryGetValue("Shadow", out propertyValue);
      if (propertyValue == null)
      {
        if (this.IsSitting() || !Game1.shouldTimePass() || !this.temporarilyInvincible || this.temporaryInvincibilityTimer % 100 < 50)
          this.farmerRenderer.Value.draw(b, this.FarmerSprite, this.FarmerSprite.SourceRect, this.getLocalPosition(Game1.viewport) + this.jitter + new Vector2(0.0f, (float) this.yJumpOffset), origin, Math.Max(0.0f, drawLayer + 0.0001f), Color.White, this.rotation, this);
      }
      else
      {
        this.farmerRenderer.Value.draw(b, this.FarmerSprite, this.FarmerSprite.SourceRect, this.getLocalPosition(Game1.viewport), origin, Math.Max(0.0f, drawLayer + 0.0001f), Color.White, this.rotation, this);
        this.farmerRenderer.Value.draw(b, this.FarmerSprite, this.FarmerSprite.SourceRect, this.getLocalPosition(Game1.viewport), origin, Math.Max(0.0f, drawLayer + 0.0002f), Color.Black * 0.25f, this.rotation, this);
      }
      if (this.isRafting)
        b.Draw(Game1.toolSpriteSheet, this.getLocalPosition(Game1.viewport) + new Vector2(0.0f, this.yOffset), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.toolSpriteSheet, 1)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, drawLayer - 1f / 1000f);
      if (Game1.activeClickableMenu == null && !Game1.eventUp && this.IsLocalPlayer && this.CurrentTool != null && (Game1.oldKBState.IsKeyDown(Keys.LeftShift) || Game1.options.alwaysShowToolHitLocation) && this.CurrentTool.doesShowTileLocationMarker() && (!Game1.options.hideToolHitLocationWhenInMotion || !this.isMoving()))
      {
        Vector2 target_position = Utility.PointToVector2(Game1.getMousePosition()) + new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, Utility.clampToTile(this.GetToolLocation(target_position)));
        b.Draw(Game1.mouseCursors, local, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, local.Y / 10000f);
      }
      if (this.IsEmoting)
      {
        Vector2 localPosition = this.getLocalPosition(Game1.viewport);
        localPosition.Y -= 160f;
        b.Draw(Game1.emoteSpriteSheet, localPosition, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, drawLayer);
      }
      if (this.ActiveObject != null && this.IsCarrying())
        Game1.drawPlayerHeldObject(this);
      if (this.sparklingText != null)
        this.sparklingText.draw(b, Game1.GlobalToLocal(Game1.viewport, this.Position + new Vector2((float) (32.0 - (double) this.sparklingText.textWidth / 2.0), (float) sbyte.MinValue)));
      bool flag = this.IsLocalPlayer && Game1.pickingTool;
      if (!(this.UsingTool | flag) || this.CurrentTool == null || this.CurrentTool.Name.Equals("Seeds") && !flag)
        return;
      Game1.drawTool(this);
    }

    public virtual void DrawUsername(SpriteBatch b)
    {
      if (!Game1.IsMultiplayer || Game1.multiplayer == null || LocalMultiplayer.IsLocalMultiplayer(true) || (double) this.usernameDisplayTime <= 0.0)
        return;
      string userName = Game1.multiplayer.getUserName(this.UniqueMultiplayerID);
      if (userName == null)
        return;
      Vector2 vector2 = Game1.smallFont.MeasureString(userName);
      Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, -104f) - vector2 / 2f;
      for (int x = -1; x <= 1; ++x)
      {
        for (int y = -1; y <= 1; ++y)
        {
          if (x != 0 || y != 0)
            b.DrawString(Game1.smallFont, userName, position + new Vector2((float) x, (float) y) * 2f, Color.Black, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999f);
        }
      }
      b.DrawString(Game1.smallFont, userName, position, Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
    }

    public static void drinkGlug(Farmer who)
    {
      Color color = Color.LightBlue;
      if (who.itemToEat != null)
      {
        switch (((IEnumerable<string>) who.itemToEat.Name.Split(' ')).Last<string>())
        {
          case "Beer":
            color = Color.Orange;
            break;
          case "Coffee":
          case "Cola":
          case "Espresso":
            color = new Color(46, 20, 0);
            break;
          case "Juice":
          case "Tea":
            color = Color.LightGreen;
            break;
          case "Milk":
            color = Color.White;
            break;
          case "Remedy":
            color = Color.LimeGreen;
            break;
          case "Tonic":
            color = Color.Red;
            break;
          case "Wine":
            color = Color.Purple;
            break;
        }
      }
      if (Game1.currentLocation == who.currentLocation)
        Game1.playSound("gulp");
      who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(653, 858, 1, 1), 9999f, 1, 1, who.Position + new Vector2((float) (32 + Game1.random.Next(-2, 3) * 4), -48f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 1.0 / 1000.0), 0.04f, color, 5f, 0.0f, 0.0f, 0.0f)
      {
        acceleration = new Vector2(0.0f, 0.5f)
      });
    }

    public void handleDisconnect()
    {
      if (this.currentLocation == null)
        return;
      if (this.rightRing.Value != null)
        this.rightRing.Value.onLeaveLocation(this, this.currentLocation);
      if (this.leftRing.Value == null)
        return;
      this.leftRing.Value.onLeaveLocation(this, this.currentLocation);
    }

    public bool isDivorced()
    {
      foreach (Friendship friendship in this.friendshipData.Values)
      {
        if (friendship.IsDivorced())
          return true;
      }
      return false;
    }

    public void wipeExMemories()
    {
      foreach (string key in this.friendshipData.Keys)
      {
        Friendship friendship = this.friendshipData[key];
        if (friendship.IsDivorced())
        {
          friendship.Clear();
          NPC characterFromName = Game1.getCharacterFromName(key);
          if (characterFromName != null)
          {
            characterFromName.CurrentDialogue.Clear();
            characterFromName.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\Characters:WipedMemory"), characterFromName));
            Game1.stats.incrementStat("exMemoriesWiped", 1);
          }
        }
      }
    }

    public void getRidOfChildren()
    {
      FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(this);
      List<Child> childList = new List<Child>();
      for (int index = homeOfFarmer.characters.Count - 1; index >= 0; --index)
      {
        if (homeOfFarmer.characters[index] is Child)
        {
          homeOfFarmer.GetChildBed(homeOfFarmer.characters[index].Gender)?.mutex.ReleaseLock();
          if ((homeOfFarmer.characters[index] as Child).hat.Value != null)
          {
            Hat hat = (homeOfFarmer.characters[index] as Child).hat.Value;
            (homeOfFarmer.characters[index] as Child).hat.Value = (Hat) null;
            Game1.player.team.returnedDonations.Add((Item) hat);
            Game1.player.team.newLostAndFoundItems.Value = true;
          }
          homeOfFarmer.characters.RemoveAt(index);
          Game1.stats.incrementStat("childrenTurnedToDoves", 1);
        }
      }
    }

    public void animateOnce(int whichAnimation)
    {
      this.FarmerSprite.animateOnce(whichAnimation, 100f, 6);
      this.CanMove = false;
    }

    public static void showItemIntake(Farmer who)
    {
      TemporaryAnimatedSprite temporaryAnimatedSprite = (TemporaryAnimatedSprite) null;
      Object @object = who.mostRecentlyGrabbedItem == null || !(who.mostRecentlyGrabbedItem is Object) ? (who.ActiveObject == null ? (Object) null : who.ActiveObject) : (Object) who.mostRecentlyGrabbedItem;
      if (@object == null)
        return;
      switch (who.FacingDirection)
      {
        case 0:
          switch (who.FarmerSprite.currentAnimationIndex)
          {
            case 1:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(0.0f, -32f), false, false, (float) ((double) who.getStandingY() / 10000.0 - 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 2:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(0.0f, -43f), false, false, (float) ((double) who.getStandingY() / 10000.0 - 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 3:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) ((double) who.getStandingY() / 10000.0 - 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 4:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 200f, 1, 0, who.Position + new Vector2(0.0f, -120f), false, false, (float) ((double) who.getStandingY() / 10000.0 - 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 5:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 200f, 1, 0, who.Position + new Vector2(0.0f, -120f), false, false, (float) ((double) who.getStandingY() / 10000.0 - 1.0 / 1000.0), 0.02f, Color.White, 4f, -0.02f, 0.0f, 0.0f);
              break;
          }
          break;
        case 1:
          switch (who.FarmerSprite.currentAnimationIndex)
          {
            case 1:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(28f, -64f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 2:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(24f, -72f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 3:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(4f, (float) sbyte.MinValue), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 4:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 200f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 5:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 200f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.02f, Color.White, 4f, -0.02f, 0.0f, 0.0f);
              break;
          }
          break;
        case 2:
          switch (who.FarmerSprite.currentAnimationIndex)
          {
            case 1:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(0.0f, -32f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 2:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(0.0f, -43f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 3:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 4:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 200f, 1, 0, who.Position + new Vector2(0.0f, -120f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 5:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 200f, 1, 0, who.Position + new Vector2(0.0f, -120f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.02f, Color.White, 4f, -0.02f, 0.0f, 0.0f);
              break;
          }
          break;
        case 3:
          switch (who.FarmerSprite.currentAnimationIndex)
          {
            case 1:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(-32f, -64f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 2:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(-28f, -76f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 3:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 100f, 1, 0, who.Position + new Vector2(-16f, (float) sbyte.MinValue), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 4:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 200f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
              break;
            case 5:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex, 16, 16), 200f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 0.00999999977648258), 0.02f, Color.White, 4f, -0.02f, 0.0f, 0.0f);
              break;
          }
          break;
      }
      if ((@object.Equals((object) who.ActiveObject) || who.ActiveObject != null && @object != null && @object.ParentSheetIndex == (int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex) && who.FarmerSprite.currentAnimationIndex == 5)
        temporaryAnimatedSprite = (TemporaryAnimatedSprite) null;
      if (temporaryAnimatedSprite != null)
        who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite);
      if (who.mostRecentlyGrabbedItem is ColoredObject && temporaryAnimatedSprite != null)
        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex + 1, 16, 16), temporaryAnimatedSprite.interval, 1, 0, temporaryAnimatedSprite.Position, false, false, temporaryAnimatedSprite.layerDepth + 0.0001f, temporaryAnimatedSprite.alphaFade, (Color) (NetFieldBase<Color, NetColor>) (who.mostRecentlyGrabbedItem as ColoredObject).color, 4f, temporaryAnimatedSprite.scaleChange, 0.0f, 0.0f));
      if (who.FarmerSprite.currentAnimationIndex != 5)
        return;
      who.Halt();
      who.FarmerSprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
    }

    public static void showSwordSwipe(Farmer who)
    {
      TemporaryAnimatedSprite temporaryAnimatedSprite = (TemporaryAnimatedSprite) null;
      bool flag = who.CurrentTool != null && who.CurrentTool is MeleeWeapon && (int) (NetFieldBase<int, NetInt>) (who.CurrentTool as MeleeWeapon).type == 1;
      Vector2 toolLocation = who.GetToolLocation(true);
      if (who.CurrentTool != null && who.CurrentTool is MeleeWeapon && !flag)
        (who.CurrentTool as MeleeWeapon).DoDamage(who.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, who.FacingDirection, 1, who);
      int val2 = 20;
      switch (who.FacingDirection)
      {
        case 0:
          switch (who.FarmerSprite.currentAnimationIndex)
          {
            case 0:
              if (flag)
              {
                who.yVelocity = 0.6f;
                break;
              }
              break;
            case 1:
              who.yVelocity = flag ? -0.5f : 0.5f;
              break;
            case 5:
              who.yVelocity = -0.3f;
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(518, 274, 23, 31), who.Position + new Vector2(0.0f, -32f) * 4f, false, 0.07f, Color.White)
              {
                scale = 4f,
                animationLength = 1,
                interval = (float) Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, val2),
                alpha = 0.5f,
                rotation = 3.926991f
              };
              break;
          }
          break;
        case 1:
          switch (who.FarmerSprite.currentAnimationIndex)
          {
            case 0:
              if (flag)
              {
                who.xVelocity = 0.6f;
                break;
              }
              break;
            case 1:
              who.xVelocity = flag ? -0.5f : 0.5f;
              break;
            case 5:
              who.xVelocity = -0.3f;
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(518, 274, 23, 31), who.Position + new Vector2(4f, -12f) * 4f, false, 0.07f, Color.White)
              {
                scale = 4f,
                animationLength = 1,
                interval = (float) Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, val2),
                alpha = 0.5f
              };
              break;
          }
          break;
        case 2:
          switch (who.FarmerSprite.currentAnimationIndex)
          {
            case 0:
              if (flag)
              {
                who.yVelocity = -0.6f;
                break;
              }
              break;
            case 1:
              who.yVelocity = flag ? 0.5f : -0.5f;
              break;
            case 5:
              who.yVelocity = 0.3f;
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(503, 256, 42, 17), who.Position + new Vector2(-16f, -2f) * 4f, false, 0.07f, Color.White)
              {
                scale = 4f,
                animationLength = 1,
                interval = (float) Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, val2),
                alpha = 0.5f,
                layerDepth = (float) (((double) who.Position.Y + 64.0) / 10000.0)
              };
              break;
          }
          break;
        case 3:
          switch (who.FarmerSprite.currentAnimationIndex)
          {
            case 0:
              if (flag)
              {
                who.xVelocity = -0.6f;
                break;
              }
              break;
            case 1:
              who.xVelocity = flag ? 0.5f : -0.5f;
              break;
            case 5:
              who.xVelocity = 0.3f;
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(518, 274, 23, 31), who.Position + new Vector2(-15f, -12f) * 4f, false, 0.07f, Color.White)
              {
                scale = 4f,
                animationLength = 1,
                interval = (float) Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, val2),
                flipped = true,
                alpha = 0.5f
              };
              break;
          }
          break;
      }
      if (temporaryAnimatedSprite == null)
        return;
      if (who.CurrentTool != null && who.CurrentTool is MeleeWeapon && who.CurrentTool.InitialParentTileIndex == 4)
        temporaryAnimatedSprite.color = Color.HotPink;
      who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite);
    }

    public static void showToolSwipeEffect(Farmer who)
    {
      if (who.CurrentTool != null && who.CurrentTool is WateringCan)
      {
        int facingDirection = who.FacingDirection;
      }
      else
      {
        switch (who.FacingDirection)
        {
          case 0:
            who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(18, who.Position + new Vector2(0.0f, -132f), Color.White, 4, animationInterval: ((double) who.stamina <= 0.0 ? 100f : 50f), sourceRectWidth: 64, layerDepth: 1f, sourceRectHeight: 64)
            {
              layerDepth = (float) (who.getStandingY() - 9) / 10000f
            });
            break;
          case 1:
            who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(15, who.Position + new Vector2(20f, -132f), Color.White, 4, animationInterval: ((double) who.stamina <= 0.0 ? 80f : 40f), sourceRectWidth: 128, layerDepth: 1f, sourceRectHeight: 128)
            {
              layerDepth = (float) (who.GetBoundingBox().Bottom + 1) / 10000f
            });
            break;
          case 2:
            who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(19, who.Position + new Vector2(-4f, (float) sbyte.MinValue), Color.White, 4, animationInterval: ((double) who.stamina <= 0.0 ? 80f : 40f), sourceRectWidth: 128, layerDepth: 1f, sourceRectHeight: 128)
            {
              layerDepth = (float) (who.GetBoundingBox().Bottom + 1) / 10000f
            });
            break;
          case 3:
            who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(15, who.Position + new Vector2(-92f, -132f), Color.White, 4, true, (double) who.stamina <= 0.0 ? 80f : 40f, sourceRectWidth: 128, layerDepth: 1f, sourceRectHeight: 128)
            {
              layerDepth = (float) (who.GetBoundingBox().Bottom + 1) / 10000f
            });
            break;
        }
      }
    }

    public static void canMoveNow(Farmer who)
    {
      who.CanMove = true;
      who.UsingTool = false;
      who.usingSlingshot = false;
      who.FarmerSprite.PauseForSingleAnimation = false;
      who.yVelocity = 0.0f;
      who.xVelocity = 0.0f;
    }

    public void FireTool() => this.fireToolEvent.Fire();

    public void synchronizedJump(float velocity)
    {
      if (!this.IsLocalPlayer)
        return;
      this.synchronizedJumpEvent.Fire(velocity);
      this.synchronizedJumpEvent.Poll();
    }

    protected void performSynchronizedJump(float velocity)
    {
      this.yJumpVelocity = velocity;
      this.yJumpOffset = -1;
    }

    private void performFireTool()
    {
      if (this.isEmoteAnimating)
        this.EndEmoteAnimation();
      if (this.CurrentTool == null)
        return;
      this.CurrentTool.leftClick(this);
    }

    public static void useTool(Farmer who)
    {
      if (who.toolOverrideFunction != null)
      {
        who.toolOverrideFunction(who);
      }
      else
      {
        if (who.CurrentTool == null)
          return;
        float stamina = who.stamina;
        if (who.IsLocalPlayer)
          who.CurrentTool.DoFunction(who.currentLocation, (int) who.GetToolLocation().X, (int) who.GetToolLocation().Y, 1, who);
        who.lastClick = Vector2.Zero;
        who.checkForExhaustion(stamina);
        Game1.toolHold = 0.0f;
      }
    }

    public void BeginUsingTool() => this.beginUsingToolEvent.Fire();

    private void performBeginUsingTool()
    {
      if (this.isEmoteAnimating)
        this.EndEmoteAnimation();
      if (this.CurrentTool == null)
        return;
      this.FarmerSprite.setOwner(this);
      this.CanMove = false;
      this.UsingTool = true;
      this.canReleaseTool = true;
      this.CurrentTool.beginUsing(this.currentLocation, (int) this.lastClick.X, (int) this.lastClick.Y, this);
    }

    public void EndUsingTool()
    {
      if (this == Game1.player)
        this.endUsingToolEvent.Fire();
      else
        this.performEndUsingTool();
    }

    private void performEndUsingTool()
    {
      if (this.isEmoteAnimating)
        this.EndEmoteAnimation();
      if (this.CurrentTool == null)
        return;
      this.CurrentTool.endUsing(this.currentLocation, this);
    }

    public void checkForExhaustion(float oldStamina)
    {
      if ((double) this.stamina <= 0.0 && (double) oldStamina > 0.0)
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.exhausted && this.IsLocalPlayer)
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1986"));
        this.setRunning(false);
        this.doEmote(36);
      }
      else if ((double) this.stamina <= 15.0 && (double) oldStamina > 15.0 && this.IsLocalPlayer)
        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1987"));
      if ((double) this.stamina > 0.0)
        return;
      this.exhausted.Value = true;
    }

    public void setMoving(byte command)
    {
      if (this.movementDirections.Count < 2)
      {
        if (command == (byte) 1 && !this.movementDirections.Contains(0) && !this.movementDirections.Contains(2))
          this.movementDirections.Insert(0, 0);
        if (command == (byte) 2 && !this.movementDirections.Contains(1) && !this.movementDirections.Contains(3))
          this.movementDirections.Insert(0, 1);
        if (command == (byte) 4 && !this.movementDirections.Contains(2) && !this.movementDirections.Contains(0))
          this.movementDirections.Insert(0, 2);
        if (command == (byte) 8 && !this.movementDirections.Contains(3) && !this.movementDirections.Contains(1))
          this.movementDirections.Insert(0, 3);
      }
      if (command == (byte) 33)
        this.movementDirections.Remove(0);
      if (command == (byte) 34)
        this.movementDirections.Remove(1);
      if (command == (byte) 36)
        this.movementDirections.Remove(2);
      if (command == (byte) 40)
        this.movementDirections.Remove(3);
      if (command == (byte) 16)
        this.setRunning(true);
      else if (command == (byte) 48)
        this.setRunning(false);
      if (((int) command & 64) != 64)
        return;
      this.Halt();
      this.running = false;
    }

    public void toolPowerIncrease()
    {
      if (this.toolPower == 0)
        this.toolPitchAccumulator = 0;
      ++this.toolPower;
      if (this.CurrentTool is Pickaxe && this.toolPower == 1)
        this.toolPower += 2;
      Color color = Color.White;
      int num = this.FacingDirection == 0 ? 4 : (this.FacingDirection == 2 ? 2 : 0);
      switch (this.toolPower)
      {
        case 1:
          color = Color.Orange;
          if (!(this.CurrentTool is WateringCan))
            this.FarmerSprite.CurrentFrame = 72 + num;
          this.jitterStrength = 0.25f;
          break;
        case 2:
          color = Color.LightSteelBlue;
          if (!(this.CurrentTool is WateringCan))
            ++this.FarmerSprite.CurrentFrame;
          this.jitterStrength = 0.5f;
          break;
        case 3:
          color = Color.Gold;
          this.jitterStrength = 1f;
          break;
        case 4:
          color = Color.Violet;
          this.jitterStrength = 2f;
          break;
        case 5:
          color = Color.BlueViolet;
          this.jitterStrength = 3f;
          break;
      }
      int x = this.FacingDirection == 1 ? 40 : (this.FacingDirection == 3 ? -40 : (this.FacingDirection == 2 ? 32 : 0));
      int y = 192;
      if (this.CurrentTool is WateringCan)
      {
        if ((int) this.facingDirection == 3)
          x = 48;
        else if ((int) this.facingDirection == 1)
          x = -48;
        y = 128;
        if (this.FacingDirection == 2)
          x = 0;
      }
      Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(21, this.Position - new Vector2((float) x, (float) y), color, animationInterval: 70f, sourceRectWidth: 64, layerDepth: ((float) ((double) this.getStandingY() / 10000.0 + 0.00499999988824129)), sourceRectHeight: 128));
      Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(192, 1152, 64, 64), 50f, 4, 0, this.Position - new Vector2(this.FacingDirection == 1 ? 0.0f : -64f, 128f), false, this.FacingDirection == 1, (float) this.getStandingY() / 10000f, 0.01f, Color.White, 1f, 0.0f, 0.0f, 0.0f));
      if (Game1.soundBank == null)
        return;
      ICue cue = Game1.soundBank.GetCue("toolCharge");
      cue.SetVariable("Pitch", new Random(Game1.dayOfMonth + (int) this.Position.X * 1000 + (int) this.Position.Y).Next(12, 16) * 100 + this.toolPower * 100);
      cue.Play();
    }

    public void UpdateIfOtherPlayer(GameTime time)
    {
      if (this.currentLocation == null)
        return;
      this.position.UpdateExtrapolation(this.getMovementSpeed());
      this.position.Field.InterpolationEnabled = !this.currentLocationRef.IsChanging();
      if (Game1.ShouldShowOnscreenUsernames() && (double) Game1.mouseCursorTransparency > 0.0 && this.currentLocation == Game1.currentLocation && Game1.currentMinigame == null && Game1.activeClickableMenu == null)
      {
        Vector2 localPosition = this.getLocalPosition(Game1.viewport);
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, 128, 192);
        rectangle.X = (int) ((double) localPosition.X + 32.0 - (double) (rectangle.Width / 2));
        rectangle.Y = (int) ((double) localPosition.Y - (double) rectangle.Height + 48.0);
        if (rectangle.Contains(Game1.getMouseX(false), Game1.getMouseY(false)))
          this.usernameDisplayTime = 1f;
      }
      if (this._lastSelectedItem != this.CurrentItem)
      {
        if (this._lastSelectedItem != null)
          this._lastSelectedItem.actionWhenStopBeingHeld(this);
        this._lastSelectedItem = this.CurrentItem;
      }
      this.FarmerSprite.setOwner(this);
      this.fireToolEvent.Poll();
      this.beginUsingToolEvent.Poll();
      this.endUsingToolEvent.Poll();
      this.drinkAnimationEvent.Poll();
      this.eatAnimationEvent.Poll();
      this.sickAnimationEvent.Poll();
      this.passOutEvent.Poll();
      this.doEmoteEvent.Poll();
      this.kissFarmerEvent.Poll();
      this.haltAnimationEvent.Poll();
      this.synchronizedJumpEvent.Poll();
      this.renovateEvent.Poll();
      this.FarmerSprite.checkForSingleAnimation(time);
      this.updateCommon(time, this.currentLocation);
    }

    public void forceCanMove()
    {
      this.forceTimePass = false;
      this.movementDirections.Clear();
      this.isEating = false;
      this.CanMove = true;
      Game1.freezeControls = false;
      this.freezePause = 0;
      this.UsingTool = false;
      this.usingSlingshot = false;
      this.FarmerSprite.PauseForSingleAnimation = false;
      if (!(this.CurrentTool is FishingRod))
        return;
      (this.CurrentTool as FishingRod).isFishing = false;
    }

    public void dropItem(Item i)
    {
      if (i == null || !i.canBeDropped())
        return;
      Game1.createItemDebris(i.getOne(), this.getStandingPosition(), this.FacingDirection);
    }

    public bool addEvent(string eventName, int daysActive)
    {
      if (this.activeDialogueEvents.ContainsKey(eventName))
        return false;
      this.activeDialogueEvents.Add(eventName, daysActive);
      return true;
    }

    public void dropObjectFromInventory(int parentSheetIndex, int quantity)
    {
      for (int index = 0; index < this.items.Count; ++index)
      {
        if (this.items[index] != null && this.items[index] is Object && (int) (NetFieldBase<int, NetInt>) (this.items[index] as Object).parentSheetIndex == parentSheetIndex)
        {
          while (quantity > 0)
          {
            this.dropItem(this.items[index].getOne());
            --this.items[index].Stack;
            --quantity;
            if (this.items[index].Stack <= 0)
            {
              this.items[index] = (Item) null;
              break;
            }
          }
          if (quantity <= 0)
            break;
        }
      }
    }

    public Vector2 getMostRecentMovementVector() => new Vector2(this.Position.X - this.lastPosition.X, this.Position.Y - this.lastPosition.Y);

    public void dropActiveItem()
    {
      if (this.CurrentItem == null || !this.CurrentItem.canBeDropped())
        return;
      Game1.createItemDebris(this.CurrentItem.getOne(), this.getStandingPosition(), this.FacingDirection);
      this.reduceActiveItemByOne();
    }

    public int GetSkillLevel(int index)
    {
      switch (index)
      {
        case 0:
          return this.FarmingLevel;
        case 1:
          return this.FishingLevel;
        case 2:
          return this.ForagingLevel;
        case 3:
          return this.MiningLevel;
        case 4:
          return this.CombatLevel;
        case 5:
          return this.LuckLevel;
        default:
          return 0;
      }
    }

    public int GetUnmodifiedSkillLevel(int index)
    {
      switch (index)
      {
        case 0:
          return this.farmingLevel.Value;
        case 1:
          return this.fishingLevel.Value;
        case 2:
          return this.foragingLevel.Value;
        case 3:
          return this.miningLevel.Value;
        case 4:
          return this.combatLevel.Value;
        case 5:
          return this.luckLevel.Value;
        default:
          return 0;
      }
    }

    public static string getSkillNameFromIndex(int index)
    {
      switch (index)
      {
        case 0:
          return "Farming";
        case 1:
          return "Fishing";
        case 2:
          return "Foraging";
        case 3:
          return "Mining";
        case 4:
          return "Combat";
        case 5:
          return "Luck";
        default:
          return "";
      }
    }

    public static string getSkillDisplayNameFromIndex(int index)
    {
      switch (index)
      {
        case 0:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1991");
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1993");
        case 2:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1994");
        case 3:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1992");
        case 4:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1996");
        case 5:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1995");
        default:
          return "";
      }
    }

    public bool hasCompletedCommunityCenter() => this.mailReceived.Contains("ccBoilerRoom") && this.mailReceived.Contains("ccCraftsRoom") && this.mailReceived.Contains("ccPantry") && this.mailReceived.Contains("ccFishTank") && this.mailReceived.Contains("ccVault") && this.mailReceived.Contains("ccBulletin");

    private bool localBusMoving()
    {
      if (this.currentLocation is Desert)
      {
        Desert currentLocation = this.currentLocation as Desert;
        return currentLocation.drivingOff || currentLocation.drivingBack;
      }
      if (!(this.currentLocation is BusStop))
        return false;
      BusStop currentLocation1 = this.currentLocation as BusStop;
      return currentLocation1.drivingOff || currentLocation1.drivingBack;
    }

    public virtual bool CanBeDamaged() => !this.temporarilyInvincible && !Game1.player.isEating && !Game1.fadeToBlack && !Game1.buffsDisplay.hasBuff(21);

    public void takeDamage(int damage, bool overrideParry, Monster damager)
    {
      if (Game1.eventUp || this.FarmerSprite.isPassingOut())
        return;
      int num1 = damager == null || damager.isInvincible() ? 0 : (!overrideParry ? 1 : 0);
      bool flag1 = (damager == null || !damager.isInvincible()) && (damager == null || !(damager is GreenSlime) && !(damager is BigSlime) || !this.isWearingRing(520));
      bool flag2 = this.CurrentTool != null && this.CurrentTool is MeleeWeapon && ((MeleeWeapon) this.CurrentTool).isOnSpecial && (int) (NetFieldBase<int, NetInt>) ((MeleeWeapon) this.CurrentTool).type == 3;
      bool flag3 = this.CanBeDamaged();
      int num2 = flag2 ? 1 : 0;
      if ((num1 & num2) != 0)
      {
        Rumble.rumble(0.75f, 150f);
        this.currentLocation.playSound("parry");
        damager.parried(damage, this);
      }
      else
      {
        if (!(flag1 & flag3))
          return;
        damager?.onDealContactDamage(this);
        damage += Game1.random.Next(Math.Min(-1, -damage / 8), Math.Max(1, damage / 8));
        int resilience = this.resilience;
        if (this.CurrentTool is MeleeWeapon)
          resilience += (int) (NetFieldBase<int, NetInt>) (this.CurrentTool as MeleeWeapon).addedDefense;
        if ((double) resilience >= (double) damage * 0.5)
          resilience -= (int) ((double) resilience * (double) Game1.random.Next(3) / 10.0);
        if (damager != null && this.isWearingRing(839))
        {
          Microsoft.Xna.Framework.Rectangle boundingBox = damager.GetBoundingBox();
          Vector2 vector2 = Utility.getAwayFromPlayerTrajectory(boundingBox, this) / 2f;
          int num3 = damage;
          int num4 = Math.Max(1, damage - resilience);
          if (num4 < 10)
            num3 = (int) Math.Ceiling((double) (num3 + num4) / 2.0);
          damager.takeDamage(num3, (int) vector2.X, (int) vector2.Y, false, 1.0, this);
          damager.currentLocation.debris.Add(new Debris(num3, new Vector2((float) (boundingBox.Center.X + 16), (float) boundingBox.Center.Y), new Color((int) byte.MaxValue, 130, 0), 1f, (Character) damager));
        }
        if (this.isWearingRing(524) && !Game1.buffsDisplay.hasBuff(21) && Game1.random.NextDouble() < (0.9 - (double) this.health / 100.0) / (double) (3 - this.LuckLevel / 10) + (this.health <= 15 ? 0.2 : 0.0))
        {
          this.currentLocation.playSound("yoba");
          Game1.buffsDisplay.addOtherBuff(new Buff(21));
        }
        else
        {
          Rumble.rumble(0.75f, 150f);
          damage = Math.Max(1, damage - resilience);
          this.health = Math.Max(0, this.health - damage);
          if (this.health <= 0 && this.GetEffectsOfRingMultiplier(863) > 0 && !this.hasUsedDailyRevive.Value)
          {
            Game1.player.startGlowing(new Color((int) byte.MaxValue, (int) byte.MaxValue, 0), false, 0.25f);
            DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => this.stopGlowing()), 500);
            Game1.playSound("yoba");
            for (int index = 0; index < 13; ++index)
            {
              float num5 = (float) Game1.random.Next(-32, 33);
              this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(114, 46, 2, 2), 200f, 5, 1, new Vector2(num5 + 32f, -96f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                attachedCharacter = (Character) this,
                positionFollowsAttachedCharacter = true,
                motion = new Vector2(num5 / 32f, -3f),
                delayBeforeAnimationStart = index * 50,
                alphaFade = 1f / 1000f,
                acceleration = new Vector2(0.0f, 0.1f)
              });
            }
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(157, 280, 28, 19), 2000f, 1, 1, new Vector2(-20f, -16f), false, false, 1E-06f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              attachedCharacter = (Character) this,
              positionFollowsAttachedCharacter = true,
              alpha = 0.1f,
              alphaFade = -0.01f,
              alphaFadeFade = -0.00025f
            });
            this.health = (int) Math.Min((float) this.maxHealth, (float) this.maxHealth * 0.5f + (float) this.GetEffectsOfRingMultiplier(863));
            this.hasUsedDailyRevive.Value = true;
          }
          this.temporarilyInvincible = true;
          this.temporaryInvincibilityTimer = 0;
          this.currentTemporaryInvincibilityDuration = 1200 + this.GetEffectsOfRingMultiplier(861) * 400;
          this.currentLocation.debris.Add(new Debris(damage, new Vector2((float) (this.getStandingX() + 8), (float) this.getStandingY()), Color.Red, 1f, (Character) this));
          this.currentLocation.playSound("ow");
          Game1.hitShakeTimer = 100 * damage;
        }
      }
    }

    public int GetEffectsOfRingMultiplier(int ring_index)
    {
      int ofRingMultiplier = 0;
      if (this.leftRing.Value != null)
        ofRingMultiplier += this.leftRing.Value.GetEffectsOfRingMultiplier(ring_index);
      if (this.rightRing.Value != null)
        ofRingMultiplier += this.rightRing.Value.GetEffectsOfRingMultiplier(ring_index);
      return ofRingMultiplier;
    }

    private void checkDamage(GameLocation location)
    {
      if (Game1.eventUp)
        return;
      for (int index = location.characters.Count - 1; index >= 0; --index)
      {
        if (index < location.characters.Count)
        {
          NPC character = location.characters[index];
          if (character is Monster)
          {
            Monster monster = character as Monster;
            if (monster.OverlapsFarmerForDamage(this))
            {
              monster.currentLocation = location;
              monster.collisionWithFarmerBehavior();
              if (monster.DamageToFarmer > 0)
              {
                if (this.CurrentTool != null && this.CurrentTool is MeleeWeapon && ((MeleeWeapon) this.CurrentTool).isOnSpecial && (int) (NetFieldBase<int, NetInt>) ((MeleeWeapon) this.CurrentTool).type == 3)
                  this.takeDamage(monster.DamageToFarmer, false, character as Monster);
                else
                  this.takeDamage(Math.Max(1, monster.DamageToFarmer + Game1.random.Next(-monster.DamageToFarmer / 4, monster.DamageToFarmer / 4)), false, character as Monster);
              }
            }
          }
        }
      }
    }

    public bool checkAction(Farmer who, GameLocation location)
    {
      if (who.isRidingHorse())
        who.Halt();
      if ((bool) (NetFieldBase<bool, NetBool>) this.hidden)
        return false;
      if (Game1.CurrentEvent != null)
      {
        if (!Game1.CurrentEvent.isSpecificFestival("spring24") || who.dancePartner.Value != null)
          return false;
        who.Halt();
        who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
        string question = Game1.content.LoadString("Strings\\UI:AskToDance_" + (this.IsMale ? "Male" : "Female"), (object) this.Name);
        location.createQuestionDialogue(question, location.createYesNoResponses(), (GameLocation.afterQuestionBehavior) ((_, answer) =>
        {
          if (!(answer == "Yes"))
            return;
          who.team.SendProposal(this, ProposalType.Dance);
          Game1.activeClickableMenu = (IClickableMenu) new PendingProposalDialog();
        }));
        return true;
      }
      if (who.CurrentItem != null && (int) (NetFieldBase<int, NetInt>) who.CurrentItem.parentSheetIndex == 801 && !this.isMarried() && !this.isEngaged() && !who.isMarried() && !who.isEngaged())
      {
        who.Halt();
        who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
        string question = Game1.content.LoadString("Strings\\UI:AskToMarry_" + (this.IsMale ? "Male" : "Female"), (object) this.Name);
        location.createQuestionDialogue(question, location.createYesNoResponses(), (GameLocation.afterQuestionBehavior) ((_, answer) =>
        {
          if (!(answer == "Yes"))
            return;
          who.team.SendProposal(this, ProposalType.Marriage, who.CurrentItem.getOne());
          Game1.activeClickableMenu = (IClickableMenu) new PendingProposalDialog();
        }));
        return true;
      }
      if (who.CanMove && who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift() && !(bool) (NetFieldBase<bool, NetBool>) who.ActiveObject.questItem)
      {
        who.Halt();
        who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
        string question = Game1.content.LoadString("Strings\\UI:GiftPlayerItem_" + (this.IsMale ? "Male" : "Female"), (object) who.ActiveObject.DisplayName, (object) this.Name);
        location.createQuestionDialogue(question, location.createYesNoResponses(), (GameLocation.afterQuestionBehavior) ((_, answer) =>
        {
          if (!(answer == "Yes"))
            return;
          who.team.SendProposal(this, ProposalType.Gift, who.ActiveObject.getOne());
          Game1.activeClickableMenu = (IClickableMenu) new PendingProposalDialog();
        }));
        return true;
      }
      long? spouse = this.team.GetSpouse(this.UniqueMultiplayerID);
      int num1 = spouse.HasValue ? 1 : 0;
      long uniqueMultiplayerId = who.UniqueMultiplayerID;
      long? nullable = spouse;
      long valueOrDefault = nullable.GetValueOrDefault();
      int num2 = uniqueMultiplayerId == valueOrDefault & nullable.HasValue ? 1 : 0;
      if ((num1 & num2) == 0 || !who.CanMove || who.isMoving() || this.isMoving() || !Utility.IsHorizontalDirection(this.getGeneralDirectionTowards(who.getStandingPosition(), -10, useTileCalculations: false)))
        return false;
      who.Halt();
      who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
      who.kissFarmerEvent.Fire(this.UniqueMultiplayerID);
      Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), 2000f, 1, 0, new Vector2((float) this.getTileX(), (float) this.getTileY()) * 64f + new Vector2(16f, -64f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        motion = new Vector2(0.0f, -0.5f),
        alphaFade = 0.01f
      });
      this.currentLocation.playSound("dwop", NetAudio.SoundContext.NPC);
      return true;
    }

    public void Update(GameTime time, GameLocation location)
    {
      this.FarmerSprite.setOwner(this);
      this.position.UpdateExtrapolation(this.getMovementSpeed());
      this.fireToolEvent.Poll();
      this.beginUsingToolEvent.Poll();
      this.endUsingToolEvent.Poll();
      this.drinkAnimationEvent.Poll();
      this.eatAnimationEvent.Poll();
      this.sickAnimationEvent.Poll();
      this.passOutEvent.Poll();
      this.doEmoteEvent.Poll();
      this.kissFarmerEvent.Poll();
      this.synchronizedJumpEvent.Poll();
      this.renovateEvent.Poll();
      if (this.IsLocalPlayer)
      {
        if (this.currentLocation == null)
          return;
        this.hidden.Value = this.localBusMoving() || location.currentEvent != null && !location.currentEvent.isFestival || location.currentEvent != null && location.currentEvent.doingSecretSanta || Game1.locationRequest != null || !Game1.displayFarmer;
        this.isInBed.Value = this.currentLocation.doesTileHaveProperty((int) this.getTileLocation().X, (int) this.getTileLocation().Y, "Bed", "Back") != null;
        if (!Game1.options.allowStowing)
          this.netItemStowed.Value = false;
        this.hasMenuOpen.Value = Game1.activeClickableMenu != null;
      }
      if (this.IsSitting())
      {
        this.movementDirections.Clear();
        if (this.IsSitting() && !this.isStopSitting)
        {
          if (!this.sittingFurniture.IsSeatHere(this.currentLocation))
            this.StopSitting(false);
          else if (this.sittingFurniture is MapSeat)
          {
            if (!((IEnumerable<ISittable>) this.currentLocation.mapSeats).Contains<ISittable>(this.sittingFurniture))
              this.StopSitting(false);
            else if ((this.sittingFurniture as MapSeat).IsBlocked(this.currentLocation))
              this.StopSitting();
          }
        }
      }
      if (Game1.CurrentEvent == null && !(bool) (NetFieldBase<bool, NetBool>) this.bathingClothes && !this.onBridge.Value)
        this.canOnlyWalk = false;
      if (this.noMovementPause > 0)
      {
        this.CanMove = false;
        this.noMovementPause -= time.ElapsedGameTime.Milliseconds;
        if (this.noMovementPause <= 0)
          this.CanMove = true;
      }
      if (this.freezePause > 0)
      {
        this.CanMove = false;
        this.freezePause -= time.ElapsedGameTime.Milliseconds;
        if (this.freezePause <= 0)
          this.CanMove = true;
      }
      if (this.sparklingText != null && this.sparklingText.update(time))
        this.sparklingText = (SparklingText) null;
      if (this.newLevelSparklingTexts.Count > 0 && this.sparklingText == null && !this.UsingTool && this.CanMove && Game1.activeClickableMenu == null)
      {
        this.sparklingText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2003", (object) Farmer.getSkillDisplayNameFromIndex(this.newLevelSparklingTexts.Peek())), Color.White, Color.White, true);
        this.newLevelSparklingTexts.Dequeue();
      }
      if ((double) this.lerpPosition >= 0.0)
      {
        this.lerpPosition += (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.lerpPosition >= (double) this.lerpDuration)
          this.lerpPosition = this.lerpDuration;
        this.Position = new Vector2(Utility.Lerp(this.lerpStartPosition.X, this.lerpEndPosition.X, this.lerpPosition / this.lerpDuration), Utility.Lerp(this.lerpStartPosition.Y, this.lerpEndPosition.Y, this.lerpPosition / this.lerpDuration));
        if ((double) this.lerpPosition >= (double) this.lerpDuration)
          this.lerpPosition = -1f;
      }
      if (this.isStopSitting && (double) this.lerpPosition < 0.0)
      {
        this.isStopSitting = false;
        if (this.sittingFurniture != null)
        {
          this.mapChairSitPosition.Value = new Vector2(-1f, -1f);
          this.sittingFurniture.RemoveSittingFarmer(this);
          this.sittingFurniture = (ISittable) null;
          this.isSitting.Value = false;
        }
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.isInBed && Game1.IsMultiplayer && Game1.shouldTimePass())
      {
        this.regenTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.regenTimer < 0)
        {
          this.regenTimer = 500;
          if ((double) this.stamina < (double) (int) (NetFieldBase<int, NetInt>) this.maxStamina)
            ++this.stamina;
          if (this.health < this.maxHealth)
            ++this.health;
        }
      }
      this.FarmerSprite.setOwner(this);
      this.FarmerSprite.checkForSingleAnimation(time);
      if (this.CanMove)
      {
        this.rotation = 0.0f;
        if (this.health <= 0 && !Game1.killScreen && Game1.timeOfDay < 2600)
        {
          if (this.IsSitting())
            this.StopSitting(false);
          this.CanMove = false;
          Game1.screenGlowOnce(Color.Red, true);
          Game1.killScreen = true;
          this.faceDirection(2);
          this.FarmerSprite.setCurrentFrame(5);
          this.jitterStrength = 1f;
          Game1.pauseTime = 3000f;
          Rumble.rumbleAndFade(0.75f, 1500f);
          this.freezePause = 8000;
          if (Game1.currentSong != null && Game1.currentSong.IsPlaying)
            Game1.currentSong.Stop(AudioStopOptions.Immediate);
          this.currentLocation.playSound("death");
          Game1.dialogueUp = false;
          ++Game1.stats.TimesUnconscious;
          if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is GameMenu)
          {
            Game1.activeClickableMenu.emergencyShutDown();
            Game1.activeClickableMenu = (IClickableMenu) null;
          }
        }
        switch (this.getDirection())
        {
          case 0:
            location.isCollidingWithWarp(this.nextPosition(0), (Character) this);
            break;
          case 1:
            location.isCollidingWithWarp(this.nextPosition(1), (Character) this);
            break;
          case 2:
            location.isCollidingWithWarp(this.nextPosition(2), (Character) this);
            break;
          case 3:
            location.isCollidingWithWarp(this.nextPosition(3), (Character) this);
            break;
        }
        if (this.collisionNPC != null)
          this.collisionNPC.farmerPassesThrough = true;
        if (this.movementDirections.Count > 0 && !this.isRidingHorse() && location.isCollidingWithCharacter(this.nextPosition(this.FacingDirection)) != null)
        {
          this.charactercollisionTimer += time.ElapsedGameTime.Milliseconds;
          if (this.charactercollisionTimer > 400)
            location.isCollidingWithCharacter(this.nextPosition(this.FacingDirection)).shake(50);
          if (this.charactercollisionTimer >= 1500 && this.collisionNPC == null)
          {
            this.collisionNPC = location.isCollidingWithCharacter(this.nextPosition(this.FacingDirection));
            if (this.collisionNPC.Name.Equals("Bouncer") && this.currentLocation != null && this.currentLocation.name.Equals((object) "SandyHouse"))
            {
              this.collisionNPC.showTextAboveHead(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2010"));
              this.collisionNPC = (NPC) null;
              this.charactercollisionTimer = 0;
            }
            else if (this.collisionNPC.name.Equals((object) "Henchman") && this.currentLocation != null && this.currentLocation.name.Equals((object) "WitchSwamp"))
            {
              this.collisionNPC = (NPC) null;
              this.charactercollisionTimer = 0;
            }
          }
        }
        else
        {
          this.charactercollisionTimer = 0;
          if (this.collisionNPC != null && location.isCollidingWithCharacter(this.nextPosition(this.FacingDirection)) == null)
          {
            this.collisionNPC.farmerPassesThrough = false;
            this.collisionNPC = (NPC) null;
          }
        }
      }
      if (Game1.shouldTimePass())
        MeleeWeapon.weaponsTypeUpdate(time);
      if (!Game1.eventUp || this.movementDirections.Count <= 0 || this.currentLocation.currentEvent == null || this.currentLocation.currentEvent.playerControlSequence || this.controller != null && this.controller.allowPlayerPathingInEvent)
      {
        this.lastPosition = this.Position;
        if (this.controller != null)
        {
          if (this.controller.update(time))
            this.controller = (PathFindController) null;
        }
        else if (this.controller == null)
          this.MovePosition(time, Game1.viewport, location);
      }
      if (this.hasNutPickupQueued && this.IsLocalPlayer && !this.IsBusyDoingSomething())
      {
        this.hasNutPickupQueued = false;
        this.showNutPickup();
      }
      this.updateCommon(time, location);
      this.position.Paused = this.FarmerSprite.PauseForSingleAnimation || this.UsingTool || this.isEating;
      this.checkDamage(location);
    }

    private void updateCommon(GameTime time, GameLocation location)
    {
      if ((double) this.usernameDisplayTime > 0.0)
      {
        this.usernameDisplayTime -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.usernameDisplayTime < 0.0)
          this.usernameDisplayTime = 0.0f;
      }
      if ((double) this.jitterStrength > 0.0)
        this.jitter = new Vector2((float) Game1.random.Next(-(int) ((double) this.jitterStrength * 100.0), (int) (((double) this.jitterStrength + 1.0) * 100.0)) / 100f, (float) Game1.random.Next(-(int) ((double) this.jitterStrength * 100.0), (int) (((double) this.jitterStrength + 1.0) * 100.0)) / 100f);
      if (this._wasSitting != this.isSitting.Value)
      {
        if (this._wasSitting)
        {
          this.yOffset = 0.0f;
          this.xOffset = 0.0f;
        }
        this._wasSitting = this.isSitting.Value;
      }
      if (this.yJumpOffset != 0)
      {
        this.yJumpVelocity -= 0.5f;
        this.yJumpOffset -= (int) this.yJumpVelocity;
        if (this.yJumpOffset >= 0)
        {
          this.yJumpOffset = 0;
          this.yJumpVelocity = 0.0f;
        }
      }
      this.updateMovementAnimation(time);
      this.updateEmote(time);
      this.updateGlow();
      this.currentLocationRef.Update();
      if ((bool) (NetFieldBase<bool, NetBool>) this.exhausted && (double) this.stamina <= 1.0)
      {
        this.currentEyes = 4;
        this.blinkTimer = -1000;
      }
      int blinkTimer = this.blinkTimer;
      TimeSpan timeSpan = time.ElapsedGameTime;
      int milliseconds1 = timeSpan.Milliseconds;
      this.blinkTimer = blinkTimer + milliseconds1;
      if (this.blinkTimer > 2200 && Game1.random.NextDouble() < 0.01)
      {
        this.blinkTimer = -150;
        this.currentEyes = 4;
      }
      else if (this.blinkTimer > -100)
        this.currentEyes = this.blinkTimer >= -50 ? (this.blinkTimer >= 0 ? 0 : 4) : 1;
      if (this.isCustomized.Value && this.isInBed.Value && !Game1.eventUp && (this.timerSinceLastMovement >= 3000 && Game1.timeOfDay >= 630 || this.timeWentToBed.Value != 0))
      {
        this.currentEyes = 1;
        this.blinkTimer = -10;
      }
      this.UpdateItemStow();
      if ((bool) (NetFieldBase<bool, NetBool>) this.swimming)
      {
        timeSpan = time.TotalGameTime;
        this.yOffset = (float) (Math.Cos(timeSpan.TotalMilliseconds / 2000.0) * 4.0);
        int swimTimer1 = this.swimTimer;
        int swimTimer2 = this.swimTimer;
        timeSpan = time.ElapsedGameTime;
        int milliseconds2 = timeSpan.Milliseconds;
        this.swimTimer = swimTimer2 - milliseconds2;
        if (this.timerSinceLastMovement == 0)
        {
          if (swimTimer1 > 400 && this.swimTimer <= 400 && this.IsLocalPlayer)
            Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2(this.Position.X, (float) (this.getStandingY() - 32)), false, Game1.random.NextDouble() < 0.5, 0.01f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
          if (this.swimTimer < 0)
          {
            this.swimTimer = 800;
            if (this.IsLocalPlayer)
            {
              this.currentLocation.playSound("slosh");
              Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2(this.Position.X, (float) (this.getStandingY() - 32)), false, Game1.random.NextDouble() < 0.5, 0.01f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
            }
          }
        }
        else if (!Game1.eventUp && (Game1.activeClickableMenu == null || Game1.IsMultiplayer) && !Game1.paused)
        {
          if (this.timerSinceLastMovement > 800)
            this.currentEyes = 1;
          else if (this.timerSinceLastMovement > 700)
            this.currentEyes = 4;
          if (this.swimTimer < 0)
          {
            this.swimTimer = 100;
            if ((double) this.stamina < (double) (int) (NetFieldBase<int, NetInt>) this.maxStamina)
              ++this.stamina;
            if (this.health < this.maxHealth)
              ++this.health;
          }
        }
      }
      if (!this.isMoving())
      {
        int sinceLastMovement = this.timerSinceLastMovement;
        timeSpan = time.ElapsedGameTime;
        int milliseconds3 = timeSpan.Milliseconds;
        this.timerSinceLastMovement = sinceLastMovement + milliseconds3;
      }
      else
        this.timerSinceLastMovement = 0;
      for (int index = this.items.Count - 1; index >= 0; --index)
      {
        if (this.items[index] != null && this.items[index] is Tool)
          ((Tool) this.items[index]).tickUpdate(time, this);
      }
      if (this.TemporaryItem is Tool)
        (this.TemporaryItem as Tool).tickUpdate(time, this);
      if (this.rightRing.Value != null)
        this.rightRing.Value.update(time, location, this);
      if (this.leftRing.Value != null)
        this.leftRing.Value.update(time, location, this);
      if (this.mount == null)
        return;
      this.mount.update(time, location);
      if (this.mount == null)
        return;
      this.mount.SyncPositionToRider();
    }

    public virtual bool IsBusyDoingSomething()
    {
      if (Game1.eventUp || Game1.fadeToBlack || Game1.currentMinigame != null || Game1.activeClickableMenu != null || Game1.isWarping || this.UsingTool || Game1.killScreen || this.freezePause > 0)
        return true;
      if (!this.CanMove || this.FarmerSprite.PauseForSingleAnimation)
        return false;
      int num = this.usingSlingshot ? 1 : 0;
      return false;
    }

    public void UpdateItemStow()
    {
      if (this._itemStowed == this.netItemStowed.Value)
        return;
      if (this.netItemStowed.Value && this.ActiveObject != null)
        this.ActiveObject.actionWhenStopBeingHeld(this);
      this._itemStowed = this.netItemStowed.Value;
      if (this.netItemStowed.Value || this.ActiveObject == null)
        return;
      this.ActiveObject.actionWhenBeingHeld(this);
    }

    public void addQuest(int questID)
    {
      if (this.hasQuest(questID))
        return;
      Quest questFromId = Quest.getQuestFromId(questID);
      if (questFromId == null)
        return;
      this.questLog.Add(questFromId);
      if (questFromId.IsHidden())
        return;
      Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2011"), 2));
    }

    public void removeQuest(int questID)
    {
      for (int index = this.questLog.Count - 1; index >= 0; --index)
      {
        if ((int) (NetFieldBase<int, NetInt>) this.questLog[index].id == questID)
          this.questLog.RemoveAt(index);
      }
    }

    public void completeQuest(int questID)
    {
      for (int index = this.questLog.Count - 1; index >= 0; --index)
      {
        if ((int) (NetFieldBase<int, NetInt>) this.questLog[index].id == questID)
          this.questLog[index].questComplete();
      }
    }

    public bool hasQuest(int id)
    {
      for (int index = this.questLog.Count - 1; index >= 0; --index)
      {
        if ((int) (NetFieldBase<int, NetInt>) this.questLog[index].id == id)
          return true;
      }
      return false;
    }

    public bool hasNewQuestActivity()
    {
      foreach (SpecialOrder specialOrder in this.team.specialOrders)
      {
        if (!specialOrder.IsHidden() && (specialOrder.ShouldDisplayAsNew() || specialOrder.ShouldDisplayAsComplete()))
          return true;
      }
      foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) this.questLog)
      {
        if (!quest.IsHidden() && ((bool) (NetFieldBase<bool, NetBool>) quest.showNew || (bool) (NetFieldBase<bool, NetBool>) quest.completed && !(bool) (NetFieldBase<bool, NetBool>) quest.destroy))
          return true;
      }
      return false;
    }

    public float getMovementSpeed()
    {
      float movementSpeed;
      if (Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence)
      {
        this.movementMultiplier = 0.066f;
        movementSpeed = Math.Max(1f, ((float) this.speed + (Game1.eventUp ? 0.0f : (float) this.addedSpeed + (this.isRidingHorse() ? 4.6f : this.temporarySpeedBuff))) * this.movementMultiplier * (float) Game1.currentGameTime.ElapsedGameTime.Milliseconds);
        if (this.movementDirections.Count > 1)
          movementSpeed = 0.7f * movementSpeed;
        if (Game1.CurrentEvent == null && this.hasBuff(19))
          movementSpeed = 0.0f;
      }
      else
      {
        movementSpeed = Math.Max(1f, (float) this.speed + (Game1.eventUp ? (float) Math.Max(0, Game1.CurrentEvent.farmerAddedSpeed - 2) : (float) this.addedSpeed + (this.isRidingHorse() ? 5f : this.temporarySpeedBuff)));
        if (this.movementDirections.Count > 1)
          movementSpeed = (float) Math.Max(1, (int) Math.Sqrt(2.0 * ((double) movementSpeed * (double) movementSpeed)) / 2);
      }
      return movementSpeed;
    }

    public bool isWearingRing(int ringIndex)
    {
      if (this.rightRing.Value != null && this.rightRing.Value.GetsEffectOfRing(ringIndex))
        return true;
      return this.leftRing.Value != null && this.leftRing.Value.GetsEffectOfRing(ringIndex);
    }

    public override void Halt()
    {
      if (!this.FarmerSprite.PauseForSingleAnimation && !this.isRidingHorse() && !this.UsingTool)
        base.Halt();
      this.movementDirections.Clear();
      if (!this.isEmoteAnimating && !this.UsingTool)
        this.stopJittering();
      this.armOffset = Vector2.Zero;
      if (this.isRidingHorse())
      {
        this.mount.Halt();
        this.mount.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
      }
      if (!this.IsSitting())
        return;
      this.ShowSitting();
    }

    public void stopJittering()
    {
      this.jitterStrength = 0.0f;
      this.jitter = Vector2.Zero;
    }

    public override Microsoft.Xna.Framework.Rectangle nextPosition(int direction)
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      switch (direction)
      {
        case 0:
          boundingBox.Y -= (int) Math.Ceiling((double) this.getMovementSpeed());
          break;
        case 1:
          boundingBox.X += (int) Math.Ceiling((double) this.getMovementSpeed());
          break;
        case 2:
          boundingBox.Y += (int) Math.Ceiling((double) this.getMovementSpeed());
          break;
        case 3:
          boundingBox.X -= (int) Math.Ceiling((double) this.getMovementSpeed());
          break;
      }
      return boundingBox;
    }

    public Microsoft.Xna.Framework.Rectangle nextPositionHalf(int direction)
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      switch (direction)
      {
        case 0:
          boundingBox.Y -= (int) Math.Ceiling((double) this.getMovementSpeed() / 2.0);
          break;
        case 1:
          boundingBox.X += (int) Math.Ceiling((double) this.getMovementSpeed() / 2.0);
          break;
        case 2:
          boundingBox.Y += (int) Math.Ceiling((double) this.getMovementSpeed() / 2.0);
          break;
        case 3:
          boundingBox.X -= (int) Math.Ceiling((double) this.getMovementSpeed() / 2.0);
          break;
      }
      return boundingBox;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skillType">e.g. farming, fishing, foraging</param>
    /// <param name="skillLevel">5 or 10</param>
    /// <returns></returns>
    public int getProfessionForSkill(int skillType, int skillLevel)
    {
      switch (skillLevel)
      {
        case 5:
          switch (skillType)
          {
            case 0:
              if (this.professions.Contains(0))
                return 0;
              if (this.professions.Contains(1))
                return 1;
              break;
            case 1:
              if (this.professions.Contains(6))
                return 6;
              if (this.professions.Contains(7))
                return 7;
              break;
            case 2:
              if (this.professions.Contains(12))
                return 12;
              if (this.professions.Contains(13))
                return 13;
              break;
            case 3:
              if (this.professions.Contains(18))
                return 18;
              if (this.professions.Contains(19))
                return 19;
              break;
            case 4:
              if (this.professions.Contains(24))
                return 24;
              if (this.professions.Contains(25))
                return 25;
              break;
          }
          break;
        case 10:
          switch (skillType)
          {
            case 0:
              if (this.professions.Contains(1))
              {
                if (this.professions.Contains(4))
                  return 4;
                if (this.professions.Contains(5))
                  return 5;
                break;
              }
              if (this.professions.Contains(2))
                return 2;
              if (this.professions.Contains(3))
                return 3;
              break;
            case 1:
              if (this.professions.Contains(6))
              {
                if (this.professions.Contains(8))
                  return 8;
                if (this.professions.Contains(9))
                  return 9;
                break;
              }
              if (this.professions.Contains(10))
                return 10;
              if (this.professions.Contains(11))
                return 11;
              break;
            case 2:
              if (this.professions.Contains(12))
              {
                if (this.professions.Contains(14))
                  return 14;
                if (this.professions.Contains(15))
                  return 15;
                break;
              }
              if (this.professions.Contains(16))
                return 16;
              if (this.professions.Contains(17))
                return 17;
              break;
            case 3:
              if (this.professions.Contains(18))
              {
                if (this.professions.Contains(20))
                  return 20;
                if (this.professions.Contains(21))
                  return 21;
                break;
              }
              if (this.professions.Contains(23))
                return 23;
              if (this.professions.Contains(22))
                return 22;
              break;
            case 4:
              if (this.professions.Contains(24))
              {
                if (this.professions.Contains(26))
                  return 26;
                if (this.professions.Contains(27))
                  return 27;
                break;
              }
              if (this.professions.Contains(28))
                return 28;
              if (this.professions.Contains(29))
                return 29;
              break;
          }
          break;
      }
      return -1;
    }

    public void behaviorOnMovement(int direction) => this.hasMoved = true;

    public void OnEmoteAnimationEnd(Farmer farmer)
    {
      if (farmer != this || !this.isEmoteAnimating)
        return;
      this.EndEmoteAnimation();
    }

    public void EndEmoteAnimation()
    {
      if (!this.isEmoteAnimating)
        return;
      if ((double) this.jitterStrength > 0.0)
        this.stopJittering();
      if (this.yJumpOffset != 0)
      {
        this.yJumpOffset = 0;
        this.yJumpVelocity = 0.0f;
      }
      this.FarmerSprite.PauseForSingleAnimation = false;
      this.FarmerSprite.StopAnimation();
      this.isEmoteAnimating = false;
    }

    private void broadcastHaltAnimation(Farmer who)
    {
      if (this.IsLocalPlayer)
        this.haltAnimationEvent.Fire();
      else
        Farmer.completelyStopAnimating(who);
    }

    private void performHaltAnimation() => this.completelyStopAnimatingOrDoingAction();

    public void performKissFarmer(long otherPlayerID)
    {
      Farmer farmer = Game1.getFarmer(otherPlayerID);
      if (farmer == null)
        return;
      bool flag = this.getStandingX() < farmer.getStandingX();
      this.PerformKiss(flag ? 1 : 3);
      farmer.PerformKiss(flag ? 3 : 1);
    }

    public void PerformKiss(int facingDirection)
    {
      if (Game1.eventUp || this.UsingTool || this.IsLocalPlayer && Game1.activeClickableMenu != null || this.isRidingHorse() || this.IsSitting() || this.IsEmoting || !this.CanMove)
        return;
      this.CanMove = false;
      this.FarmerSprite.PauseForSingleAnimation = false;
      this.faceDirection(facingDirection);
      this.FarmerSprite.animateOnce(new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(101, 1000, 0, false, this.FacingDirection == 3),
        new FarmerSprite.AnimationFrame(6, 1, false, this.FacingDirection == 3, new AnimatedSprite.endOfAnimationBehavior(this.broadcastHaltAnimation))
      }.ToArray());
    }

    public override void MovePosition(
      GameTime time,
      xTile.Dimensions.Rectangle viewport,
      GameLocation currentLocation)
    {
      if (this.IsSitting())
        return;
      if (Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence)
      {
        if (Game1.shouldTimePass() && this.temporarilyInvincible)
        {
          if (this.temporaryInvincibilityTimer < 0)
            this.currentTemporaryInvincibilityDuration = 1200;
          this.temporaryInvincibilityTimer += time.ElapsedGameTime.Milliseconds;
          if (this.temporaryInvincibilityTimer > this.currentTemporaryInvincibilityDuration)
          {
            this.temporarilyInvincible = false;
            this.temporaryInvincibilityTimer = 0;
          }
        }
      }
      else if (this.temporarilyInvincible)
      {
        this.temporarilyInvincible = false;
        this.temporaryInvincibilityTimer = 0;
      }
      if (Game1.activeClickableMenu != null && (Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence))
        return;
      if (this.isRafting)
      {
        this.moveRaft(currentLocation, time);
      }
      else
      {
        if ((double) this.xVelocity != 0.0 || (double) this.yVelocity != 0.0)
        {
          if (double.IsNaN((double) this.xVelocity) || double.IsNaN((double) this.yVelocity))
          {
            this.xVelocity = 0.0f;
            this.yVelocity = 0.0f;
          }
          Microsoft.Xna.Framework.Rectangle boundingBox1 = this.GetBoundingBox();
          boundingBox1.X += (int) Math.Floor((double) this.xVelocity);
          boundingBox1.Y -= (int) Math.Floor((double) this.yVelocity);
          Microsoft.Xna.Framework.Rectangle boundingBox2 = this.GetBoundingBox();
          boundingBox2.X += (int) Math.Ceiling((double) this.xVelocity);
          boundingBox2.Y -= (int) Math.Ceiling((double) this.yVelocity);
          Microsoft.Xna.Framework.Rectangle position = Microsoft.Xna.Framework.Rectangle.Union(boundingBox1, boundingBox2);
          if (!currentLocation.isCollidingPosition(position, viewport, true, -1, false, (Character) this))
          {
            this.position.X += this.xVelocity;
            this.position.Y -= this.yVelocity;
            this.xVelocity -= this.xVelocity / 16f;
            this.yVelocity -= this.yVelocity / 16f;
            if ((double) Math.Abs(this.xVelocity) <= 0.0500000007450581)
              this.xVelocity = 0.0f;
            if ((double) Math.Abs(this.yVelocity) <= 0.0500000007450581)
              this.yVelocity = 0.0f;
          }
          else
          {
            this.xVelocity -= this.xVelocity / 16f;
            this.yVelocity -= this.yVelocity / 16f;
            if ((double) Math.Abs(this.xVelocity) <= 0.0500000007450581)
              this.xVelocity = 0.0f;
            if ((double) Math.Abs(this.yVelocity) <= 0.0500000007450581)
              this.yVelocity = 0.0f;
          }
        }
        if (this.CanMove || Game1.eventUp || this.controller != null)
        {
          this.temporaryPassableTiles.ClearNonIntersecting(this.GetBoundingBox());
          float movementSpeed = this.getMovementSpeed();
          this.temporarySpeedBuff = 0.0f;
          if (this.movementDirections.Contains(0))
          {
            Warp w = Game1.currentLocation.isCollidingWithWarp(this.nextPosition(0), (Character) this);
            if (w != null && this.IsLocalPlayer)
            {
              this.warpFarmer(w, 0);
              return;
            }
            if (!currentLocation.isCollidingPosition(this.nextPosition(0), viewport, true, 0, false, (Character) this) || this.ignoreCollisions)
            {
              this.position.Y -= movementSpeed;
              this.behaviorOnMovement(0);
            }
            else if (!currentLocation.isCollidingPosition(this.nextPositionHalf(0), viewport, true, 0, false, (Character) this))
            {
              this.position.Y -= movementSpeed / 2f;
              this.behaviorOnMovement(0);
            }
            else if (this.movementDirections.Count == 1)
            {
              Microsoft.Xna.Framework.Rectangle position = this.nextPosition(0);
              position.Width /= 4;
              bool flag1 = currentLocation.isCollidingPosition(position, viewport, true, 0, false, (Character) this);
              position.X += position.Width * 3;
              bool flag2 = currentLocation.isCollidingPosition(position, viewport, true, 0, false, (Character) this);
              if (flag1 && !flag2 && !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, true, 0, false, (Character) this))
                this.position.X += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
              else if (flag2 && !flag1 && !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, true, 0, false, (Character) this))
                this.position.X -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
            }
          }
          if (this.movementDirections.Contains(2))
          {
            Warp w = Game1.currentLocation.isCollidingWithWarp(this.nextPosition(2), (Character) this);
            if (w != null && this.IsLocalPlayer)
            {
              this.warpFarmer(w, 2);
              return;
            }
            if (!currentLocation.isCollidingPosition(this.nextPosition(2), viewport, true, 0, false, (Character) this) || this.ignoreCollisions)
            {
              this.position.Y += movementSpeed;
              this.behaviorOnMovement(2);
            }
            else if (!currentLocation.isCollidingPosition(this.nextPositionHalf(2), viewport, true, 0, false, (Character) this))
            {
              this.position.Y += movementSpeed / 2f;
              this.behaviorOnMovement(2);
            }
            else if (this.movementDirections.Count == 1)
            {
              Microsoft.Xna.Framework.Rectangle position = this.nextPosition(2);
              position.Width /= 4;
              bool flag3 = currentLocation.isCollidingPosition(position, viewport, true, 0, false, (Character) this);
              position.X += position.Width * 3;
              bool flag4 = currentLocation.isCollidingPosition(position, viewport, true, 0, false, (Character) this);
              if (flag3 && !flag4 && !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, true, 0, false, (Character) this))
                this.position.X += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
              else if (flag4 && !flag3 && !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, true, 0, false, (Character) this))
                this.position.X -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
            }
          }
          if (this.movementDirections.Contains(1))
          {
            Warp w = Game1.currentLocation.isCollidingWithWarp(this.nextPosition(1), (Character) this);
            if (w != null && this.IsLocalPlayer)
            {
              this.warpFarmer(w, 1);
              return;
            }
            if (!currentLocation.isCollidingPosition(this.nextPosition(1), viewport, true, 0, false, (Character) this) || this.ignoreCollisions)
            {
              this.position.X += movementSpeed;
              this.behaviorOnMovement(1);
            }
            else if (!currentLocation.isCollidingPosition(this.nextPositionHalf(1), viewport, true, 0, false, (Character) this))
            {
              this.position.X += movementSpeed / 2f;
              this.behaviorOnMovement(1);
            }
            else if (this.movementDirections.Count == 1)
            {
              Microsoft.Xna.Framework.Rectangle position = this.nextPosition(1);
              position.Height /= 4;
              bool flag5 = currentLocation.isCollidingPosition(position, viewport, true, 0, false, (Character) this);
              position.Y += position.Height * 3;
              bool flag6 = currentLocation.isCollidingPosition(position, viewport, true, 0, false, (Character) this);
              if (flag5 && !flag6 && !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, true, 0, false, (Character) this))
                this.position.Y += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
              else if (flag6 && !flag5 && !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, true, 0, false, (Character) this))
                this.position.Y -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
            }
          }
          if (this.movementDirections.Contains(3))
          {
            Warp w = Game1.currentLocation.isCollidingWithWarp(this.nextPosition(3), (Character) this);
            if (w != null && this.IsLocalPlayer)
            {
              this.warpFarmer(w, 3);
              return;
            }
            if (!currentLocation.isCollidingPosition(this.nextPosition(3), viewport, true, 0, false, (Character) this) || this.ignoreCollisions)
            {
              this.position.X -= movementSpeed;
              this.behaviorOnMovement(3);
            }
            else if (!currentLocation.isCollidingPosition(this.nextPositionHalf(3), viewport, true, 0, false, (Character) this))
            {
              this.position.X -= movementSpeed / 2f;
              this.behaviorOnMovement(3);
            }
            else if (this.movementDirections.Count == 1)
            {
              Microsoft.Xna.Framework.Rectangle position = this.nextPosition(3);
              position.Height /= 4;
              bool flag7 = currentLocation.isCollidingPosition(position, viewport, true, 0, false, (Character) this);
              position.Y += position.Height * 3;
              bool flag8 = currentLocation.isCollidingPosition(position, viewport, true, 0, false, (Character) this);
              if (flag7 && !flag8 && !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, true, 0, false, (Character) this))
                this.position.Y += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
              else if (flag8 && !flag7 && !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, true, 0, false, (Character) this))
                this.position.Y -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
            }
          }
        }
        this.FarmerSprite.intervalModifier = this.movementDirections.Count <= 0 || this.UsingTool ? 1f : (float) (1.0 - (this.running ? 0.0299999993294477 : 0.025000000372529) * ((double) Math.Max(1f, ((float) this.speed + (Game1.eventUp ? 0.0f : (float) this.addedSpeed + (this.isRidingHorse() ? 4.6f : 0.0f))) * this.movementMultiplier * (float) Game1.currentGameTime.ElapsedGameTime.Milliseconds) * 1.25));
        if (currentLocation == null || !currentLocation.isFarmerCollidingWithAnyCharacter())
          return;
        this.temporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle((int) this.getTileLocation().X * 64, (int) this.getTileLocation().Y * 64, 64, 64));
      }
    }

    public void updateMovementAnimation(GameTime time)
    {
      TimeSpan elapsedGameTime;
      if (this._emoteGracePeriod > 0)
      {
        int emoteGracePeriod = this._emoteGracePeriod;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this._emoteGracePeriod = emoteGracePeriod - milliseconds;
      }
      if (this.isEmoteAnimating)
      {
        if ((!this.IsLocalPlayer ? this.IsRemoteMoving() : this.movementDirections.Count > 0) && this._emoteGracePeriod <= 0 || !this.FarmerSprite.PauseForSingleAnimation)
          this.EndEmoteAnimation();
      }
      bool flag = this.IsCarrying();
      if (!this.isRidingHorse())
        this.xOffset = 0.0f;
      if (this.CurrentTool is FishingRod)
      {
        FishingRod currentTool = this.CurrentTool as FishingRod;
        if (currentTool.isTimingCast || currentTool.isCasting)
        {
          currentTool.setTimingCastAnimation(this);
          return;
        }
      }
      if (this.FarmerSprite.PauseForSingleAnimation || this.UsingTool)
        return;
      if (this.IsSitting())
        this.ShowSitting();
      else if (this.IsLocalPlayer && !this.CanMove && !Game1.eventUp)
      {
        if (this.isRidingHorse() && this.mount != null && !this.isAnimatingMount)
        {
          this.showRiding();
        }
        else
        {
          if (!flag)
            return;
          this.showCarrying();
        }
      }
      else
      {
        if (this.IsLocalPlayer || this.isFakeEventActor)
        {
          this.moveUp = this.movementDirections.Contains(0);
          this.moveRight = this.movementDirections.Contains(1);
          this.moveDown = this.movementDirections.Contains(2);
          this.moveLeft = this.movementDirections.Contains(3);
          int num = this.moveUp || this.moveRight || this.moveDown ? 1 : (this.moveLeft ? 1 : 0);
          if (this.moveLeft)
            this.FacingDirection = 3;
          else if (this.moveRight)
            this.FacingDirection = 1;
          else if (this.moveUp)
            this.FacingDirection = 0;
          else if (this.moveDown)
            this.FacingDirection = 2;
          if (this.isRidingHorse() && !(bool) (NetFieldBase<bool, NetBool>) this.mount.dismounting)
            this.speed = 2;
        }
        else
        {
          this.moveLeft = this.IsRemoteMoving() && this.FacingDirection == 3;
          this.moveRight = this.IsRemoteMoving() && this.FacingDirection == 1;
          this.moveUp = this.IsRemoteMoving() && this.FacingDirection == 0;
          this.moveDown = this.IsRemoteMoving() && this.FacingDirection == 2;
          int num1 = this.moveUp || this.moveRight || this.moveDown ? 1 : (this.moveLeft ? 1 : 0);
          double num2 = (double) this.position.CurrentInterpolationSpeed();
          elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
          double num3 = (double) elapsedGameTime.Milliseconds * 0.0659999996423721;
          float num4 = (float) (num2 / num3);
          this.running = (double) Math.Abs(num4 - 5f) < (double) Math.Abs(num4 - 2f) && !(bool) (NetFieldBase<bool, NetBool>) this.bathingClothes && !this.onBridge.Value;
          if (num1 == 0)
            this.FarmerSprite.StopAnimation();
        }
        if (this.hasBuff(19))
        {
          this.running = false;
          this.moveUp = false;
          this.moveDown = false;
          this.moveLeft = false;
          this.moveRight = false;
        }
        if (this.FarmerSprite.PauseForSingleAnimation || this.UsingTool)
          return;
        if (this.isRidingHorse() && !(bool) (NetFieldBase<bool, NetBool>) this.mount.dismounting)
          this.showRiding();
        else if (this.moveLeft && this.running && !flag)
          this.FarmerSprite.animate(56, time);
        else if (this.moveRight && this.running && !flag)
          this.FarmerSprite.animate(40, time);
        else if (this.moveUp && this.running && !flag)
          this.FarmerSprite.animate(48, time);
        else if (this.moveDown && this.running && !flag)
          this.FarmerSprite.animate(32, time);
        else if (this.moveLeft && this.running)
          this.FarmerSprite.animate(152, time);
        else if (this.moveRight && this.running)
          this.FarmerSprite.animate(136, time);
        else if (this.moveUp && this.running)
          this.FarmerSprite.animate(144, time);
        else if (this.moveDown && this.running)
          this.FarmerSprite.animate(128, time);
        else if (this.moveLeft && !flag)
          this.FarmerSprite.animate(24, time);
        else if (this.moveRight && !flag)
          this.FarmerSprite.animate(8, time);
        else if (this.moveUp && !flag)
          this.FarmerSprite.animate(16, time);
        else if (this.moveDown && !flag)
          this.FarmerSprite.animate(0, time);
        else if (this.moveLeft)
          this.FarmerSprite.animate(120, time);
        else if (this.moveRight)
          this.FarmerSprite.animate(104, time);
        else if (this.moveUp)
          this.FarmerSprite.animate(112, time);
        else if (this.moveDown)
          this.FarmerSprite.animate(96, time);
        else if (flag)
          this.showCarrying();
        else
          this.showNotCarrying();
      }
    }

    public bool IsCarrying() => this.mount == null && !this.isAnimatingMount && !this.IsSitting() && !this.onBridge.Value && this.ActiveObject != null && !Game1.eventUp && !Game1.killScreen && !(this.ActiveObject is Furniture);

    public void doneEating()
    {
      this.isEating = false;
      this.completelyStopAnimatingOrDoingAction();
      this.forceCanMove();
      if (this.mostRecentlyGrabbedItem == null)
        return;
      Object itemToEat = this.itemToEat as Object;
      if (this.IsLocalPlayer && itemToEat.ParentSheetIndex == 434)
      {
        if (Utility.foundAllStardrops())
          Game1.getSteamAchievement("Achievement_Stardrop");
        this.yOffset = 0.0f;
        this.yJumpOffset = 0;
        Game1.changeMusicTrack("none");
        Game1.playSound("stardrop");
        string str1 = Game1.random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3094") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3095");
        string str2 = !this.favoriteThing.Contains("Stardew") ? (!this.favoriteThing.Equals((object) "ConcernedApe") ? str1 + (string) (NetFieldBase<string, NetString>) this.favoriteThing : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3099")) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3097");
        DelayedAction.showDialogueAfterDelay(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3100") + str2 + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3101"), 6000);
        this.MaxStamina += 34;
        this.Stamina = (float) this.MaxStamina;
        this.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1]
        {
          new FarmerSprite.AnimationFrame(57, 6000)
        });
        this.startGlowing(new Color(200, 0, (int) byte.MaxValue), false, 0.1f);
        this.jitterStrength = 1f;
        Game1.staminaShakeTimer = 12000;
        Game1.screenGlowOnce(new Color(200, 0, (int) byte.MaxValue), true);
        this.CanMove = false;
        this.freezePause = 8000;
        this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(368, 16, 16, 16), 60f, 8, 40, this.Position + new Vector2(-8f, (float) sbyte.MinValue), false, false, 1f, 0.0f, Color.White, 4f, 0.0075f, 0.0f, 0.0f)
        {
          alpha = 0.75f,
          alphaFade = 1f / 400f,
          motion = new Vector2(0.0f, -0.25f)
        });
        if (Game1.displayHUD && !Game1.eventUp)
        {
          for (int index = 0; index < 40; ++index)
          {
            List<TemporaryAnimatedSprite> overlayTempSprites = Game1.uiOverlayTempSprites;
            int rowInAnimationTexture = Game1.random.Next(10, 12);
            Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
            double x = (double) viewport.TitleSafeArea.Right / (double) Game1.options.uiScale - 48.0 - 8.0 - (double) Game1.random.Next(64);
            double num1 = (double) Game1.random.Next(-64, 64);
            viewport = Game1.graphics.GraphicsDevice.Viewport;
            double num2 = (double) viewport.TitleSafeArea.Bottom / (double) Game1.options.uiScale;
            double y = num1 + num2 - 224.0 - 16.0 - (double) (int) ((double) (Game1.player.MaxStamina - 270) * 0.715);
            Vector2 position = new Vector2((float) x, (float) y);
            Color color = Game1.random.NextDouble() < 0.5 ? Color.White : Color.Lime;
            overlayTempSprites.Add(new TemporaryAnimatedSprite(rowInAnimationTexture, position, color, animationInterval: 50f)
            {
              layerDepth = 1f,
              delayBeforeAnimationStart = 200 * index,
              interval = 100f,
              local = true
            });
          }
        }
        Utility.addSprinklesToLocation(this.currentLocation, this.getTileX(), this.getTileY(), 9, 9, 6000, 100, new Color(200, 0, (int) byte.MaxValue), motionTowardCenter: true);
        DelayedAction.stopFarmerGlowing(6000);
        Utility.addSprinklesToLocation(this.currentLocation, this.getTileX(), this.getTileY(), 9, 9, 6000, 300, Color.Cyan, motionTowardCenter: true);
        this.mostRecentlyGrabbedItem = (Item) null;
      }
      else if (this.IsLocalPlayer)
      {
        if (itemToEat != null && itemToEat.HasContextTag("ginger_item") && this.hasBuff(25))
          Game1.buffsDisplay.removeOtherBuff(25);
        string[] strArray1 = Game1.objectInformation[itemToEat.ParentSheetIndex].Split('/');
        if (Convert.ToInt32(strArray1[2]) > 0)
        {
          string[] strArray2;
          if (strArray1.Length <= 7)
            strArray2 = new string[12]
            {
              "0",
              "0",
              "0",
              "0",
              "0",
              "0",
              "0",
              "0",
              "0",
              "0",
              "0",
              "0"
            };
          else
            strArray2 = strArray1[7].Split(' ');
          string[] buffs = strArray2;
          itemToEat.ModifyItemBuffs(buffs);
          int minutesDuration = strArray1.Length > 8 ? Convert.ToInt32(strArray1[8]) : -1;
          if (itemToEat.Quality != 0)
            minutesDuration = (int) ((double) minutesDuration * 1.5);
          Buff b = new Buff(Convert.ToInt32(buffs[0]), Convert.ToInt32(buffs[1]), Convert.ToInt32(buffs[2]), Convert.ToInt32(buffs[3]), Convert.ToInt32(buffs[4]), Convert.ToInt32(buffs[5]), Convert.ToInt32(buffs[6]), Convert.ToInt32(buffs[7]), Convert.ToInt32(buffs[8]), Convert.ToInt32(buffs[9]), Convert.ToInt32(buffs[10]), buffs.Length > 11 ? Convert.ToInt32(buffs[11]) : 0, minutesDuration, strArray1[0], strArray1[4]);
          if (Utility.IsNormalObjectAtParentSheetIndex((Item) itemToEat, 921))
            b.which = 28;
          if (strArray1.Length > 6 && strArray1[6].Equals("drink"))
            Game1.buffsDisplay.tryToAddDrinkBuff(b);
          else if (Convert.ToInt32(strArray1[2]) > 0)
            Game1.buffsDisplay.tryToAddFoodBuff(b, Math.Min(120000, (int) ((double) Convert.ToInt32(strArray1[2]) / 20.0 * 30000.0)));
        }
        float stamina = this.Stamina;
        int health = this.health;
        this.Stamina = Math.Min((float) this.MaxStamina, this.Stamina + (float) itemToEat.staminaRecoveredOnConsumption());
        this.health = Math.Min(this.maxHealth, this.health + itemToEat.healthRecoveredOnConsumption());
        if ((double) stamina < (double) this.Stamina)
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3116", (object) (int) ((double) this.Stamina - (double) stamina)), 4));
        if (health < this.health)
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3118", (object) (this.health - health)), 5));
      }
      if (itemToEat == null || itemToEat.Edibility >= 0 || !this.IsLocalPlayer)
        return;
      this.CanMove = false;
      this.sickAnimationEvent.Fire();
    }

    public bool checkForQuestComplete(
      NPC n,
      int number1,
      int number2,
      Item item,
      string str,
      int questType = -1,
      int questTypeToIgnore = -1)
    {
      bool flag = false;
      for (int index = this.questLog.Count - 1; index >= 0; --index)
      {
        if (this.questLog[index] != null && (questType == -1 || (int) (NetFieldBase<int, NetInt>) this.questLog[index].questType == questType) && (questTypeToIgnore == -1 || (int) (NetFieldBase<int, NetInt>) this.questLog[index].questType != questTypeToIgnore) && this.questLog[index].checkIfComplete(n, number1, number2, item, str))
          flag = true;
      }
      return flag;
    }

    public static void completelyStopAnimating(Farmer who) => who.completelyStopAnimatingOrDoingAction();

    public void completelyStopAnimatingOrDoingAction()
    {
      this.CanMove = !Game1.eventUp;
      if (this.isEmoteAnimating)
        this.EndEmoteAnimation();
      if (this.UsingTool)
      {
        this.EndUsingTool();
        if (this.CurrentTool is FishingRod)
          (this.CurrentTool as FishingRod).resetState();
      }
      if (this.usingSlingshot && this.CurrentTool is Slingshot)
        (this.CurrentTool as Slingshot).finish();
      this.UsingTool = false;
      this.isEating = false;
      this.FarmerSprite.PauseForSingleAnimation = false;
      this.usingSlingshot = false;
      this.canReleaseTool = false;
      this.Halt();
      this.Sprite.StopAnimation();
      if (this.CurrentTool is MeleeWeapon)
        (this.CurrentTool as MeleeWeapon).isOnSpecial = false;
      this.stopJittering();
    }

    public void doEmote(int whichEmote)
    {
      if (Game1.eventUp || this.isEmoting)
        return;
      this.isEmoting = true;
      this.currentEmote = whichEmote;
      this.currentEmoteFrame = 0;
      this.emoteInterval = 0.0f;
    }

    public void performTenMinuteUpdate()
    {
      if (this.addedSpeed <= 0 || Game1.buffsDisplay.otherBuffs.Count != 0 || Game1.buffsDisplay.food != null || Game1.buffsDisplay.drink != null)
        return;
      this.addedSpeed = 0;
    }

    public void setRunning(bool isRunning, bool force = false)
    {
      if (this.canOnlyWalk || (bool) (NetFieldBase<bool, NetBool>) this.bathingClothes && !this.running || Game1.CurrentEvent != null & isRunning && !Game1.CurrentEvent.isFestival && !Game1.CurrentEvent.playerControlSequence && (this.controller == null || !this.controller.allowPlayerPathingInEvent))
        return;
      if (this.isRidingHorse())
        this.running = true;
      else if ((double) this.stamina <= 0.0)
      {
        this.speed = 2;
        if (this.running)
          this.Halt();
        this.running = false;
      }
      else if (force || this.CanMove && !this.isEating && (Game1.currentLocation.currentEvent == null || Game1.currentLocation.currentEvent.playerControlSequence) && (isRunning || !this.UsingTool) && (isRunning || !Game1.pickingTool) && (this.Sprite == null || !((FarmerSprite) this.Sprite).PauseForSingleAnimation))
      {
        this.running = isRunning;
        if (this.running)
          this.speed = 5;
        else
          this.speed = 2;
      }
      else
      {
        if (!this.UsingTool)
          return;
        this.running = isRunning;
        if (this.running)
          this.speed = 5;
        else
          this.speed = 2;
      }
    }

    public void addSeenResponse(int id) => this.dialogueQuestionsAnswered.Add(id);

    public void eatObject(Object o, bool overrideFullness = false)
    {
      if (Utility.IsNormalObjectAtParentSheetIndex((Item) o, 434))
      {
        Game1.changeMusicTrack("none");
        Game1.multiplayer.globalChatInfoMessage("Stardrop", this.Name);
      }
      if (this.getFacingDirection() != 2)
        this.faceDirection(2);
      this.itemToEat = (Item) o;
      this.mostRecentlyGrabbedItem = (Item) o;
      string[] strArray = Game1.objectInformation[o.ParentSheetIndex].Split('/');
      this.forceCanMove();
      this.completelyStopAnimatingOrDoingAction();
      if (strArray.Length > 6 && strArray[6].Equals("drink"))
      {
        if (this.IsLocalPlayer && Game1.buffsDisplay.hasBuff(7) && !overrideFullness)
        {
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2898"), Color.OrangeRed, 3500f));
          return;
        }
        this.drinkAnimationEvent.Fire(o.getOne() as Object);
      }
      else if (Convert.ToInt32(strArray[2]) != -300)
      {
        if (Game1.buffsDisplay.hasBuff(6) && !overrideFullness)
        {
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2899"), Color.OrangeRed, 3500f));
          return;
        }
        this.eatAnimationEvent.Fire(o.getOne() as Object);
      }
      this.freezePause = 20000;
      this.CanMove = false;
      this.isEating = true;
    }

    private void performDrinkAnimation(Object item)
    {
      if (this.isEmoteAnimating)
        this.EndEmoteAnimation();
      if (!this.IsLocalPlayer)
        this.itemToEat = (Item) item;
      this.FarmerSprite.animateOnce(294, 80f, 8);
      this.isEating = true;
    }

    public Farmer CreateFakeEventFarmer()
    {
      Farmer fakeEventFarmer = new Farmer(new FarmerSprite(this.FarmerSprite.textureName.Value), new Vector2(192f, 192f), 1, "", new List<Item>(), this.IsMale);
      fakeEventFarmer.Name = this.Name;
      fakeEventFarmer.displayName = this.displayName;
      fakeEventFarmer.isFakeEventActor = true;
      fakeEventFarmer.changeGender(this.IsMale);
      fakeEventFarmer.changeHairStyle((int) (NetFieldBase<int, NetInt>) this.hair);
      fakeEventFarmer.UniqueMultiplayerID = this.UniqueMultiplayerID;
      fakeEventFarmer.shirtItem.Set((Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) this.shirtItem);
      fakeEventFarmer.pantsItem.Set((Clothing) (NetFieldBase<Clothing, NetRef<Clothing>>) this.pantsItem);
      fakeEventFarmer.shirt.Set(this.shirt.Value);
      fakeEventFarmer.pants.Set(this.pants.Value);
      fakeEventFarmer.changeShoeColor(this.shoes.Value);
      fakeEventFarmer.boots.Set(this.boots.Value);
      fakeEventFarmer.leftRing.Set(this.leftRing.Value);
      fakeEventFarmer.rightRing.Set(this.rightRing.Value);
      fakeEventFarmer.hat.Set(this.hat.Value);
      fakeEventFarmer.shirtColor = this.shirtColor;
      fakeEventFarmer.pantsColor.Set(this.pantsColor.Value);
      fakeEventFarmer.changeHairColor((Color) (NetFieldBase<Color, NetColor>) this.hairstyleColor);
      fakeEventFarmer.changeSkinColor(this.skin.Value);
      fakeEventFarmer.accessory.Set(this.accessory.Value);
      fakeEventFarmer.changeEyeColor(this.newEyeColor.Value);
      fakeEventFarmer.UpdateClothing();
      return fakeEventFarmer;
    }

    private void performEatAnimation(Object item)
    {
      if (this.isEmoteAnimating)
        this.EndEmoteAnimation();
      if (!this.IsLocalPlayer)
        this.itemToEat = (Item) item;
      this.FarmerSprite.animateOnce(216, 80f, 8);
      this.isEating = true;
    }

    public void netDoEmote(string emote_type) => this.doEmoteEvent.Fire(emote_type);

    private void performSickAnimation()
    {
      if (this.isEmoteAnimating)
        this.EndEmoteAnimation();
      this.isEating = false;
      this.FarmerSprite.animateOnce(224, 350f, 4);
      this.doEmote(12);
    }

    public void eatHeldObject()
    {
      if (this.isEmoteAnimating)
        this.EndEmoteAnimation();
      if (Game1.fadeToBlack)
        return;
      if (this.ActiveObject == null)
        this.ActiveObject = (Object) this.mostRecentlyGrabbedItem;
      this.eatObject(this.ActiveObject);
      if (!this.isEating)
        return;
      this.reduceActiveItemByOne();
      this.CanMove = false;
    }

    public void grabObject(Object obj)
    {
      if (this.isEmoteAnimating)
        this.EndEmoteAnimation();
      if (obj == null)
        return;
      this.CanMove = false;
      switch (this.FacingDirection)
      {
        case 0:
          ((FarmerSprite) this.Sprite).animateOnce(80, 50f, 8);
          break;
        case 1:
          ((FarmerSprite) this.Sprite).animateOnce(72, 50f, 8);
          break;
        case 2:
          ((FarmerSprite) this.Sprite).animateOnce(64, 50f, 8);
          break;
        case 3:
          ((FarmerSprite) this.Sprite).animateOnce(88, 50f, 8);
          break;
      }
      Game1.playSound("pickUpItem");
    }

    public virtual void PlayFishBiteChime()
    {
      int num = this.biteChime.Value;
      if (num < 0)
        num = Game1.game1.instanceIndex;
      if (num > 3)
        num = 3;
      if (num == 0)
        this.currentLocation.localSound("fishBite");
      else
        this.currentLocation.localSound("fishBite_alternate_" + (num - 1).ToString());
    }

    public string getTitle()
    {
      int level = this.Level;
      if (level >= 30)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2016");
      if (level > 28)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2017");
      if (level > 26)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2018");
      if (level > 24)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2019");
      if (level > 22)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2020");
      if (level > 20)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2021");
      if (level > 18)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2022");
      if (level > 16)
        return !this.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2024") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2023");
      if (level > 14)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2025");
      if (level > 12)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2026");
      if (level > 10)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2027");
      if (level > 8)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2028");
      if (level > 6)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2029");
      if (level > 4)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2030");
      return level > 2 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2031") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2032");
    }

    public void queueMessage(byte messageType, Farmer sourceFarmer, params object[] data) => this.queueMessage(new OutgoingMessage(messageType, sourceFarmer, data));

    public void queueMessage(OutgoingMessage message) => this.messageQueue.Add(message);

    static Farmer()
    {
      Farmer.EmoteType[] emoteTypeArray = new Farmer.EmoteType[22]
      {
        new Farmer.EmoteType("happy", "Emote_Happy", 32),
        new Farmer.EmoteType("sad", "Emote_Sad", 28),
        new Farmer.EmoteType("heart", "Emote_Heart", 20),
        new Farmer.EmoteType("exclamation", "Emote_Exclamation", 16),
        new Farmer.EmoteType("note", "Emote_Note", 56),
        new Farmer.EmoteType("sleep", "Emote_Sleep", 24),
        new Farmer.EmoteType("game", "Emote_Game", 52),
        new Farmer.EmoteType("question", "Emote_Question", 8),
        new Farmer.EmoteType("x", "Emote_X", 36),
        new Farmer.EmoteType("pause", "Emote_Pause", 40),
        new Farmer.EmoteType("blush", "Emote_Blush", 60, is_hidden: true),
        new Farmer.EmoteType("angry", "Emote_Angry", 12),
        new Farmer.EmoteType("yes", "Emote_Yes", 56, new FarmerSprite.AnimationFrame[7]
        {
          new FarmerSprite.AnimationFrame(0, 250, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
          {
            if (!who.ShouldHandleAnimationSound())
              return;
            who.currentLocation.localSound("jingle1");
          })),
          new FarmerSprite.AnimationFrame(16, 150, false, false),
          new FarmerSprite.AnimationFrame(0, 250, false, false),
          new FarmerSprite.AnimationFrame(16, 150, false, false),
          new FarmerSprite.AnimationFrame(0, 250, false, false),
          new FarmerSprite.AnimationFrame(16, 150, false, false),
          new FarmerSprite.AnimationFrame(0, 250, false, false)
        }),
        new Farmer.EmoteType("no", "Emote_No", 36, new FarmerSprite.AnimationFrame[5]
        {
          new FarmerSprite.AnimationFrame(25, 250, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
          {
            if (!who.ShouldHandleAnimationSound())
              return;
            who.currentLocation.localSound("cancel");
          })),
          new FarmerSprite.AnimationFrame(27, 250, true, false),
          new FarmerSprite.AnimationFrame(25, 250, false, false),
          new FarmerSprite.AnimationFrame(27, 250, true, false),
          new FarmerSprite.AnimationFrame(25, 250, false, false)
        }),
        new Farmer.EmoteType("sick", "Emote_Sick", 12, new FarmerSprite.AnimationFrame[8]
        {
          new FarmerSprite.AnimationFrame(104, 350, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
          {
            if (!who.ShouldHandleAnimationSound())
              return;
            who.currentLocation.localSound("croak");
          })),
          new FarmerSprite.AnimationFrame(105, 350, false, false),
          new FarmerSprite.AnimationFrame(104, 350, false, false),
          new FarmerSprite.AnimationFrame(105, 350, false, false),
          new FarmerSprite.AnimationFrame(104, 350, false, false),
          new FarmerSprite.AnimationFrame(105, 350, false, false),
          new FarmerSprite.AnimationFrame(104, 350, false, false),
          new FarmerSprite.AnimationFrame(105, 350, false, false)
        }),
        new Farmer.EmoteType("laugh", "Emote_Laugh", 56, new FarmerSprite.AnimationFrame[8]
        {
          new FarmerSprite.AnimationFrame(102, 150, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
          {
            if (!who.ShouldHandleAnimationSound())
              return;
            who.currentLocation.localSound("dustMeep");
          })),
          new FarmerSprite.AnimationFrame(103, 150, false, false),
          new FarmerSprite.AnimationFrame(102, 150, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
          {
            if (!who.ShouldHandleAnimationSound())
              return;
            who.currentLocation.localSound("dustMeep");
          })),
          new FarmerSprite.AnimationFrame(103, 150, false, false),
          new FarmerSprite.AnimationFrame(102, 150, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
          {
            if (!who.ShouldHandleAnimationSound())
              return;
            who.currentLocation.localSound("dustMeep");
          })),
          new FarmerSprite.AnimationFrame(103, 150, false, false),
          new FarmerSprite.AnimationFrame(102, 150, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
          {
            if (!who.ShouldHandleAnimationSound())
              return;
            who.currentLocation.localSound("dustMeep");
          })),
          new FarmerSprite.AnimationFrame(103, 150, false, false)
        }),
        new Farmer.EmoteType("surprised", "Emote_Surprised", 16, new FarmerSprite.AnimationFrame[1]
        {
          new FarmerSprite.AnimationFrame(94, 1500, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
          {
            if (who.ShouldHandleAnimationSound())
              who.currentLocation.localSound("batScreech");
            who.jumpWithoutSound(4f);
            who.jitterStrength = 1f;
          }))
        }),
        new Farmer.EmoteType("hi", "Emote_Hi", 56, new FarmerSprite.AnimationFrame[4]
        {
          new FarmerSprite.AnimationFrame(3, 250, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
          {
            if (!who.ShouldHandleAnimationSound())
              return;
            who.currentLocation.localSound("give_gift");
          })),
          new FarmerSprite.AnimationFrame(85, 250, false, false),
          new FarmerSprite.AnimationFrame(3, 250, false, false),
          new FarmerSprite.AnimationFrame(85, 250, false, false)
        }),
        null,
        null,
        null,
        null
      };
      FarmerSprite.AnimationFrame[] frames1 = new FarmerSprite.AnimationFrame[10];
      frames1[0] = new FarmerSprite.AnimationFrame(3, 250, false, false);
      frames1[1] = new FarmerSprite.AnimationFrame(102, 50, false, false);
      FarmerSprite.AnimationFrame animationFrame1 = new FarmerSprite.AnimationFrame(10, 250, false, false);
      animationFrame1 = animationFrame1.AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
      {
        if (who.ShouldHandleAnimationSound())
          who.currentLocation.localSound("hitEnemy");
        who.jitterStrength = 1f;
      }));
      frames1[2] = animationFrame1.AddFrameEndAction((AnimatedSprite.endOfAnimationBehavior) (who => who.stopJittering()));
      frames1[3] = new FarmerSprite.AnimationFrame(3, 250, false, false);
      frames1[4] = new FarmerSprite.AnimationFrame(102, 50, false, false);
      FarmerSprite.AnimationFrame animationFrame2 = new FarmerSprite.AnimationFrame(10, 250, false, false);
      animationFrame2 = animationFrame2.AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
      {
        if (who.ShouldHandleAnimationSound())
          who.currentLocation.localSound("hitEnemy");
        who.jitterStrength = 1f;
      }));
      frames1[5] = animationFrame2.AddFrameEndAction((AnimatedSprite.endOfAnimationBehavior) (who => who.stopJittering()));
      frames1[6] = new FarmerSprite.AnimationFrame(3, 250, false, false);
      frames1[7] = new FarmerSprite.AnimationFrame(102, 50, false, false);
      FarmerSprite.AnimationFrame animationFrame3 = new FarmerSprite.AnimationFrame(10, 250, false, false);
      animationFrame3 = animationFrame3.AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
      {
        if (who.ShouldHandleAnimationSound())
          who.currentLocation.localSound("hitEnemy");
        who.jitterStrength = 1f;
      }));
      frames1[8] = animationFrame3.AddFrameEndAction((AnimatedSprite.endOfAnimationBehavior) (who => who.stopJittering()));
      frames1[9] = new FarmerSprite.AnimationFrame(3, 500, false, false);
      emoteTypeArray[18] = new Farmer.EmoteType("taunt", "Emote_Taunt", 12, frames1, is_hidden: true);
      emoteTypeArray[19] = new Farmer.EmoteType("uh", "Emote_Uh", 40, new FarmerSprite.AnimationFrame[1]
      {
        new FarmerSprite.AnimationFrame(10, 1500, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
        {
          if (!who.ShouldHandleAnimationSound())
            return;
          who.currentLocation.localSound("clam_tone");
        }))
      });
      emoteTypeArray[20] = new Farmer.EmoteType("music", "Emote_Music", 56, new FarmerSprite.AnimationFrame[9]
      {
        new FarmerSprite.AnimationFrame(98, 150, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who => who.playHarpEmoteSound())),
        new FarmerSprite.AnimationFrame(99, 150, false, false),
        new FarmerSprite.AnimationFrame(100, 150, false, false),
        new FarmerSprite.AnimationFrame(98, 150, false, false),
        new FarmerSprite.AnimationFrame(99, 150, false, false),
        new FarmerSprite.AnimationFrame(100, 150, false, false),
        new FarmerSprite.AnimationFrame(98, 150, false, false),
        new FarmerSprite.AnimationFrame(99, 150, false, false),
        new FarmerSprite.AnimationFrame(100, 150, false, false)
      }, is_hidden: true);
      FarmerSprite.AnimationFrame[] frames2 = new FarmerSprite.AnimationFrame[6];
      frames2[0] = new FarmerSprite.AnimationFrame(111, 150, false, false);
      FarmerSprite.AnimationFrame animationFrame4 = new FarmerSprite.AnimationFrame(111, 300, false, false);
      animationFrame4 = animationFrame4.AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
      {
        if (who.ShouldHandleAnimationSound())
          who.currentLocation.localSound("fishingRodBend");
        who.jitterStrength = 1f;
      }));
      frames2[1] = animationFrame4.AddFrameEndAction((AnimatedSprite.endOfAnimationBehavior) (who => who.stopJittering()));
      frames2[2] = new FarmerSprite.AnimationFrame(111, 500, false, false);
      FarmerSprite.AnimationFrame animationFrame5 = new FarmerSprite.AnimationFrame(111, 300, false, false);
      animationFrame5 = animationFrame5.AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
      {
        if (who.ShouldHandleAnimationSound())
          who.currentLocation.localSound("fishingRodBend");
        who.jitterStrength = 1f;
      }));
      frames2[3] = animationFrame5.AddFrameEndAction((AnimatedSprite.endOfAnimationBehavior) (who => who.stopJittering()));
      frames2[4] = new FarmerSprite.AnimationFrame(111, 500, false, false);
      frames2[5] = new FarmerSprite.AnimationFrame(112, 1000, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
      {
        if (who.ShouldHandleAnimationSound())
          who.currentLocation.localSound("coin");
        who.jumpWithoutSound(4f);
      }));
      emoteTypeArray[21] = new Farmer.EmoteType("jar", "Emote_Jar", frames: frames2, facing_direction: 1, is_hidden: true);
      Farmer.EMOTES = emoteTypeArray;
    }

    public class EmoteType
    {
      public string emoteString = "";
      public int emoteIconIndex = -1;
      public FarmerSprite.AnimationFrame[] animationFrames;
      public bool hidden;
      public int facingDirection = 2;
      public string displayNameKey;

      public EmoteType(
        string emote_string = "",
        string display_name_key = "",
        int icon_index = -1,
        FarmerSprite.AnimationFrame[] frames = null,
        int facing_direction = 2,
        bool is_hidden = false)
      {
        this.emoteString = emote_string;
        this.emoteIconIndex = icon_index;
        this.animationFrames = frames;
        this.facingDirection = facing_direction;
        this.hidden = is_hidden;
        this.displayNameKey = "Strings\\UI:" + display_name_key;
      }

      public string displayName => Game1.content.LoadString(this.displayNameKey);
    }
  }
}
