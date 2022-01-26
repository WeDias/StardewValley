// Decompiled with JetBrains decompiler
// Type: StardewValley.NPC
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley
{
  public class NPC : Character, IComparable
  {
    public const int minimum_square_pause = 6000;
    public const int maximum_square_pause = 12000;
    public const int portrait_width = 64;
    public const int portrait_height = 64;
    public const int portrait_neutral_index = 0;
    public const int portrait_happy_index = 1;
    public const int portrait_sad_index = 2;
    public const int portrait_custom_index = 3;
    public const int portrait_blush_index = 4;
    public const int portrait_angry_index = 5;
    public const int startingFriendship = 0;
    public const int defaultSpeed = 2;
    public const int maxGiftsPerWeek = 2;
    public const int friendshipPointsPerHeartLevel = 250;
    public const int maxFriendshipPoints = 2500;
    public const int gift_taste_love = 0;
    public const int gift_taste_like = 2;
    public const int gift_taste_neutral = 8;
    public const int gift_taste_dislike = 4;
    public const int gift_taste_hate = 6;
    public const int textStyle_shake = 0;
    public const int textStyle_fade = 1;
    public const int textStyle_none = 2;
    public const int adult = 0;
    public const int teen = 1;
    public const int child = 2;
    public const int neutral = 0;
    public const int polite = 1;
    public const int rude = 2;
    public const int outgoing = 0;
    public const int shy = 1;
    public const int positive = 0;
    public const int negative = 1;
    public const int male = 0;
    public const int female = 1;
    public const int undefined = 2;
    public const int other = 0;
    public const int desert = 1;
    public const int town = 2;
    public static bool isCheckingSpouseTileOccupancy = false;
    private static List<List<string>> routesFromLocationToLocation;
    private Dictionary<int, SchedulePathDescription> schedule;
    private Dictionary<string, string> dialogue;
    private SchedulePathDescription directionsToNewLocation;
    private int directionIndex;
    private int lengthOfWalkingSquareX;
    private int lengthOfWalkingSquareY;
    private int squarePauseAccumulation;
    private int squarePauseTotal;
    private int squarePauseOffset;
    public Microsoft.Xna.Framework.Rectangle lastCrossroad;
    private Texture2D portrait;
    private Vector2 nextSquarePosition;
    protected int shakeTimer;
    private bool isWalkingInSquare;
    private readonly NetBool isWalkingTowardPlayer = new NetBool();
    protected string textAboveHead;
    protected int textAboveHeadPreTimer;
    protected int textAboveHeadTimer;
    protected int textAboveHeadStyle;
    protected int textAboveHeadColor;
    protected float textAboveHeadAlpha;
    public int daysAfterLastBirth = -1;
    private string extraDialogueMessageToAddThisMorning;
    [XmlElement("birthday_Season")]
    public readonly NetString birthday_Season = new NetString();
    [XmlElement("birthday_Day")]
    public readonly NetInt birthday_Day = new NetInt();
    [XmlElement("age")]
    public readonly NetInt age = new NetInt();
    [XmlElement("manners")]
    public readonly NetInt manners = new NetInt();
    [XmlElement("socialAnxiety")]
    public readonly NetInt socialAnxiety = new NetInt();
    [XmlElement("optimism")]
    public readonly NetInt optimism = new NetInt();
    [XmlElement("gender")]
    public readonly NetInt gender = new NetInt();
    [XmlIgnore]
    public readonly NetBool breather = new NetBool(true);
    [XmlIgnore]
    public readonly NetBool isSleeping = new NetBool(false);
    [XmlElement("sleptInBed")]
    public readonly NetBool sleptInBed = new NetBool(true);
    [XmlIgnore]
    public readonly NetBool hideShadow = new NetBool();
    [XmlElement("isInvisible")]
    public readonly NetBool isInvisible = new NetBool(false);
    [XmlElement("lastSeenMovieWeek")]
    public readonly NetInt lastSeenMovieWeek = new NetInt(-1);
    [XmlIgnore]
    public readonly NetString syncedPortraitPath = new NetString();
    public bool datingFarmer;
    public bool divorcedFromFarmer;
    [XmlElement("datable")]
    public readonly NetBool datable = new NetBool();
    [XmlIgnore]
    public bool uniqueSpriteActive;
    [XmlIgnore]
    public bool uniquePortraitActive;
    [XmlIgnore]
    public bool updatedDialogueYet;
    [XmlIgnore]
    public bool immediateSpeak;
    [XmlIgnore]
    public bool ignoreScheduleToday;
    protected int defaultFacingDirection;
    [XmlElement("defaultPosition")]
    private readonly NetVector2 defaultPosition = new NetVector2();
    [XmlElement("defaultMap")]
    public readonly NetString defaultMap = new NetString();
    public string loveInterest;
    protected int idForClones = -1;
    public int id = -1;
    public int homeRegion;
    public int daysUntilNotInvisible;
    public bool followSchedule = true;
    [XmlIgnore]
    public PathFindController temporaryController;
    [XmlElement("moveTowardPlayerThreshold")]
    public readonly NetInt moveTowardPlayerThreshold = new NetInt();
    [XmlIgnore]
    public float rotation;
    [XmlIgnore]
    public float yOffset;
    [XmlIgnore]
    public float swimTimer;
    [XmlIgnore]
    public float timerSinceLastMovement;
    [XmlIgnore]
    public string mapBeforeEvent;
    [XmlIgnore]
    public Vector2 positionBeforeEvent;
    [XmlIgnore]
    public Vector2 lastPosition;
    [XmlIgnore]
    public float currentScheduleDelay;
    [XmlIgnore]
    public float scheduleDelaySeconds;
    [XmlIgnore]
    public bool layingDown;
    [XmlIgnore]
    public Vector2 appliedRouteAnimationOffset = Vector2.Zero;
    [XmlIgnore]
    public string[] routeAnimationMetadata;
    [XmlElement("hasSaidAfternoonDialogue")]
    private NetBool hasSaidAfternoonDialogue = new NetBool(false);
    [XmlIgnore]
    public static bool hasSomeoneWateredCrops;
    [XmlIgnore]
    public static bool hasSomeoneFedThePet;
    [XmlIgnore]
    public static bool hasSomeoneFedTheAnimals;
    [XmlIgnore]
    public static bool hasSomeoneRepairedTheFences = false;
    [XmlIgnore]
    protected bool _skipRouteEndIntro;
    [NonInstancedStatic]
    public static HashSet<string> invalidDialogueFiles = new HashSet<string>();
    [XmlIgnore]
    protected bool _hasLoadedMasterScheduleData;
    [XmlIgnore]
    protected Dictionary<string, string> _masterScheduleData;
    [XmlIgnore]
    protected string _lastLoadedScheduleKey;
    [XmlIgnore]
    public readonly NetList<MarriageDialogueReference, NetRef<MarriageDialogueReference>> currentMarriageDialogue = new NetList<MarriageDialogueReference, NetRef<MarriageDialogueReference>>();
    public readonly NetBool hasBeenKissedToday = new NetBool(false);
    [XmlIgnore]
    public readonly NetRef<MarriageDialogueReference> marriageDefaultDialogue = new NetRef<MarriageDialogueReference>((MarriageDialogueReference) null);
    [XmlIgnore]
    public readonly NetBool shouldSayMarriageDialogue = new NetBool(false);
    [XmlIgnore]
    public readonly NetBool exploreFarm = new NetBool(false);
    [XmlIgnore]
    public float nextFarmActivityScan;
    [XmlIgnore]
    protected List<FarmActivity> _farmActivities;
    [XmlIgnore]
    protected float _farmActivityWeightTotal;
    [XmlIgnore]
    protected FarmActivity _currentFarmActivity;
    public readonly NetEvent0 removeHenchmanEvent = new NetEvent0();
    private bool isPlayingSleepingAnimation;
    public readonly NetBool shouldPlayRobinHammerAnimation = new NetBool();
    private bool isPlayingRobinHammerAnimation;
    public readonly NetBool shouldPlaySpousePatioAnimation = new NetBool();
    private bool isPlayingSpousePatioAnimation = (bool) (NetFieldBase<bool, NetBool>) new NetBool();
    public readonly NetBool shouldWearIslandAttire = new NetBool();
    private bool isWearingIslandAttire;
    public readonly NetBool isMovingOnPathFindPath = new NetBool();
    public List<KeyValuePair<int, SchedulePathDescription>> queuedSchedulePaths = new List<KeyValuePair<int, SchedulePathDescription>>();
    public int lastAttemptedSchedule = -1;
    [XmlIgnore]
    public readonly NetBool doingEndOfRouteAnimation = new NetBool();
    private bool currentlyDoingEndOfRouteAnimation;
    [XmlIgnore]
    public readonly NetBool goingToDoEndOfRouteAnimation = new NetBool();
    [XmlIgnore]
    public readonly NetString endOfRouteMessage = new NetString();
    [XmlElement("dayScheduleName")]
    public readonly NetString dayScheduleName = new NetString();
    [XmlElement("islandScheduleName")]
    public readonly NetString islandScheduleName = new NetString();
    private int[] routeEndIntro;
    private int[] routeEndAnimation;
    private int[] routeEndOutro;
    [XmlIgnore]
    public string nextEndOfRouteMessage;
    private string loadedEndOfRouteBehavior;
    [XmlIgnore]
    protected string _startedEndOfRouteBehavior;
    [XmlIgnore]
    protected string _finishingEndOfRouteBehavior;
    [XmlIgnore]
    protected int _beforeEndOfRouteAnimationFrame;
    public readonly NetString endOfRouteBehaviorName = new NetString();
    private Point previousEndPoint;
    public int squareMovementFacingPreference;
    public const int NO_TRY = 9999999;
    private bool returningToEndPoint;
    private string nameOfTodaysSchedule = "";

    [XmlIgnore]
    public SchedulePathDescription DirectionsToNewLocation
    {
      get => this.directionsToNewLocation;
      set => this.directionsToNewLocation = value;
    }

    [XmlIgnore]
    public int DirectionIndex
    {
      get => this.directionIndex;
      set => this.directionIndex = value;
    }

    public int DefaultFacingDirection
    {
      get => this.defaultFacingDirection;
      set => this.defaultFacingDirection = value;
    }

    [XmlIgnore]
    public Dictionary<string, string> Dialogue
    {
      get
      {
        switch (this)
        {
          case Monster _:
            return (Dictionary<string, string>) null;
          case Pet _:
            return (Dictionary<string, string>) null;
          case Horse _:
            return (Dictionary<string, string>) null;
          case Child _:
            return (Dictionary<string, string>) null;
          default:
            if (this.dialogue == null)
            {
              string assetName = "Characters\\Dialogue\\" + this.GetDialogueSheetName();
              if (NPC.invalidDialogueFiles.Contains(assetName))
                this.dialogue = new Dictionary<string, string>();
              try
              {
                this.dialogue = Game1.content.Load<Dictionary<string, string>>(assetName).Select<KeyValuePair<string, string>, KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, KeyValuePair<string, string>>) (pair =>
                {
                  string key = pair.Key;
                  string str1 = pair.Value;
                  if (str1.Contains("¦"))
                    str1 = !Game1.player.IsMale ? str1.Substring(str1.IndexOf("¦") + 1) : str1.Substring(0, str1.IndexOf("¦"));
                  string str2 = str1;
                  return new KeyValuePair<string, string>(key, str2);
                })).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (p => p.Key), (Func<KeyValuePair<string, string>, string>) (p => p.Value));
              }
              catch (ContentLoadException ex)
              {
                NPC.invalidDialogueFiles.Add(assetName);
                this.dialogue = new Dictionary<string, string>();
              }
            }
            return this.dialogue;
        }
      }
    }

    public string DefaultMap
    {
      get => this.defaultMap.Value;
      set => this.defaultMap.Value = value;
    }

    public Vector2 DefaultPosition
    {
      get => this.defaultPosition.Value;
      set => this.defaultPosition.Value = value;
    }

    [XmlIgnore]
    public Texture2D Portrait
    {
      get
      {
        if (this.portrait == null)
        {
          try
          {
            string assetName = !string.IsNullOrEmpty(this.syncedPortraitPath.Value) ? (string) (NetFieldBase<string, NetString>) this.syncedPortraitPath : "Portraits\\" + this.getTextureName();
            if (this.isWearingIslandAttire)
            {
              try
              {
                this.portrait = Game1.content.Load<Texture2D>(assetName + "_Beach");
              }
              catch (ContentLoadException ex)
              {
                this.portrait = (Texture2D) null;
              }
            }
            if (this.portrait == null)
              this.portrait = Game1.content.Load<Texture2D>(assetName);
          }
          catch (ContentLoadException ex)
          {
            this.portrait = (Texture2D) null;
          }
        }
        return this.portrait;
      }
      set => this.portrait = value;
    }

    [XmlIgnore]
    public Dictionary<int, SchedulePathDescription> Schedule
    {
      get => this.schedule;
      set => this.schedule = value;
    }

    public bool IsWalkingInSquare
    {
      get => this.isWalkingInSquare;
      set => this.isWalkingInSquare = value;
    }

    public bool IsWalkingTowardPlayer
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isWalkingTowardPlayer;
      set => this.isWalkingTowardPlayer.Value = value;
    }

    [XmlIgnore]
    public Stack<StardewValley.Dialogue> CurrentDialogue
    {
      get
      {
        Stack<StardewValley.Dialogue> currentDialogue = (Stack<StardewValley.Dialogue>) null;
        if (Game1.npcDialogues == null)
          Game1.npcDialogues = new Dictionary<string, Stack<StardewValley.Dialogue>>();
        Game1.npcDialogues.TryGetValue(this.Name, out currentDialogue);
        if (currentDialogue == null)
          currentDialogue = Game1.npcDialogues[this.Name] = this.loadCurrentDialogue();
        return currentDialogue;
      }
      set
      {
        if (Game1.npcDialogues == null)
          return;
        Game1.npcDialogues[this.Name] = value;
      }
    }

    [XmlIgnore]
    public string Birthday_Season
    {
      get => (string) (NetFieldBase<string, NetString>) this.birthday_Season;
      set => this.birthday_Season.Value = value;
    }

    [XmlIgnore]
    public int Birthday_Day
    {
      get => (int) (NetFieldBase<int, NetInt>) this.birthday_Day;
      set => this.birthday_Day.Value = value;
    }

    [XmlIgnore]
    public int Age
    {
      get => (int) (NetFieldBase<int, NetInt>) this.age;
      set => this.age.Value = value;
    }

    [XmlIgnore]
    public int Manners
    {
      get => (int) (NetFieldBase<int, NetInt>) this.manners;
      set => this.manners.Value = value;
    }

    [XmlIgnore]
    public int SocialAnxiety
    {
      get => (int) (NetFieldBase<int, NetInt>) this.socialAnxiety;
      set => this.socialAnxiety.Value = value;
    }

    [XmlIgnore]
    public int Optimism
    {
      get => (int) (NetFieldBase<int, NetInt>) this.optimism;
      set => this.optimism.Value = value;
    }

    [XmlIgnore]
    public int Gender
    {
      get => (int) (NetFieldBase<int, NetInt>) this.gender;
      set => this.gender.Value = value;
    }

    [XmlIgnore]
    public bool Breather
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.breather;
      set => this.breather.Value = value;
    }

    [XmlIgnore]
    public bool HideShadow
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.hideShadow;
      set => this.hideShadow.Value = value;
    }

    [XmlIgnore]
    public bool HasPartnerForDance
    {
      get
      {
        foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
        {
          if (onlineFarmer.dancePartner.TryGetVillager() == this)
            return true;
        }
        return false;
      }
    }

    [XmlIgnore]
    public bool IsInvisible
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isInvisible;
      set => this.isInvisible.Value = value;
    }

    public virtual bool CanSocialize
    {
      get
      {
        if (this.Name.Equals("Leo") && !Game1.MasterPlayer.mailReceived.Contains("addedParrotBoy") || this.Name.Equals("Sandy") && !Game1.MasterPlayer.mailReceived.Contains("ccVault") || this.Name.Equals("???") || this.Name.Equals("Bouncer") || this.Name.Equals("Marlon") || this.Name.Equals("Gil") || this.Name.Equals("Gunther") || this.Name.Equals("Henchman") || this.Name.Equals("Birdie") || this.IsMonster)
          return false;
        switch (this)
        {
          case Horse _:
          case Pet _:
          case JunimoHarvester _:
            return false;
          default:
            if (!this.Name.Equals("Dwarf") && !this.Name.Contains("Qi"))
            {
              switch (this)
              {
                case Pet _:
                case Horse _:
                case Junimo _:
                  break;
                default:
                  return !this.Name.Equals("Krobus") || Game1.player.friendshipData.ContainsKey("Krobus");
              }
            }
            return false;
        }
      }
    }

    public NPC()
    {
    }

    public NPC(
      AnimatedSprite sprite,
      Vector2 position,
      int facingDir,
      string name,
      LocalizedContentManager content = null)
      : base(sprite, position, 2, name)
    {
      this.faceDirection(facingDir);
      this.defaultPosition.Value = position;
      this.defaultFacingDirection = facingDir;
      this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle((int) position.X, (int) position.Y + 64, 64, 64);
      if (content == null)
        return;
      try
      {
        this.portrait = content.Load<Texture2D>("Portraits\\" + name);
      }
      catch (Exception ex)
      {
      }
    }

    public NPC(
      AnimatedSprite sprite,
      Vector2 position,
      string defaultMap,
      int facingDirection,
      string name,
      bool datable,
      Dictionary<int, int[]> schedule,
      Texture2D portrait)
      : this(sprite, position, defaultMap, facingDirection, name, schedule, portrait, false)
    {
      this.datable.Value = datable;
    }

    public NPC(
      AnimatedSprite sprite,
      Vector2 position,
      string defaultMap,
      int facingDir,
      string name,
      Dictionary<int, int[]> schedule,
      Texture2D portrait,
      bool eventActor,
      string syncedPortraitPath = null)
      : base(sprite, position, 2, name)
    {
      this.portrait = portrait;
      this.syncedPortraitPath.Value = syncedPortraitPath;
      this.faceDirection(facingDir);
      if (!eventActor)
        this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle((int) position.X, (int) position.Y + 64, 64, 64);
      this.reloadData();
      this.defaultPosition.Value = position;
      this.defaultMap.Value = defaultMap;
      this.currentLocation = Game1.getLocationFromName(defaultMap);
      this.defaultFacingDirection = facingDir;
    }

    public virtual void reloadData()
    {
      try
      {
        Dictionary<string, string> source = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
        if (this is Child || !source.ContainsKey((string) (NetFieldBase<string, NetString>) this.name))
          return;
        string[] strArray = source[(string) (NetFieldBase<string, NetString>) this.name].Split('/');
        string str1 = strArray[0];
        if (!(str1 == "teen"))
        {
          if (str1 == "child")
            this.Age = 2;
        }
        else
          this.Age = 1;
        string str2 = strArray[1];
        if (!(str2 == "rude"))
        {
          if (str2 == "polite")
            this.Manners = 1;
        }
        else
          this.Manners = 2;
        string str3 = strArray[2];
        if (!(str3 == "shy"))
        {
          if (str3 == "outgoing")
            this.SocialAnxiety = 0;
        }
        else
          this.SocialAnxiety = 1;
        string str4 = strArray[3];
        if (!(str4 == "positive"))
        {
          if (str4 == "negative")
            this.Optimism = 1;
        }
        else
          this.Optimism = 0;
        string str5 = strArray[4];
        if (!(str5 == "female"))
        {
          if (str5 == "undefined")
            this.Gender = 2;
        }
        else
          this.Gender = 1;
        string str6 = strArray[5];
        if (!(str6 == "datable"))
        {
          if (str6 == "not-datable")
            this.datable.Value = false;
        }
        else
          this.datable.Value = true;
        this.loveInterest = strArray[6];
        string str7 = strArray[7];
        if (!(str7 == "Desert"))
        {
          if (!(str7 == "Other"))
          {
            if (str7 == "Town")
              this.homeRegion = 2;
          }
          else
            this.homeRegion = 0;
        }
        else
          this.homeRegion = 1;
        if (strArray.Length > 8 && strArray[8].Length > 0)
        {
          this.Birthday_Season = strArray[8].Split(' ')[0];
          this.Birthday_Day = Convert.ToInt32(strArray[8].Split(' ')[1]);
        }
        for (int index = 0; index < source.Count; ++index)
        {
          if (source.ElementAt<KeyValuePair<string, string>>(index).Key.Equals((string) (NetFieldBase<string, NetString>) this.name))
          {
            this.id = index;
            break;
          }
        }
        if (!this.isMarried())
          this.reloadDefaultLocation();
        this.displayName = strArray[11];
      }
      catch (Exception ex)
      {
      }
    }

    public virtual void reloadDefaultLocation()
    {
      try
      {
        Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
        if (!dictionary.ContainsKey((string) (NetFieldBase<string, NetString>) this.name))
          return;
        string[] strArray = dictionary[this.Name].Split('/')[10].Split(' ');
        this.DefaultMap = strArray[0];
        this.DefaultPosition = new Vector2((float) (Convert.ToInt32(strArray[1]) * 64), (float) (Convert.ToInt32(strArray[2]) * 64));
      }
      catch (Exception ex)
      {
      }
    }

    public virtual bool canTalk() => true;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.birthday_Season, (INetSerializable) this.birthday_Day, (INetSerializable) this.datable, (INetSerializable) this.shouldPlayRobinHammerAnimation, (INetSerializable) this.shouldPlaySpousePatioAnimation, (INetSerializable) this.isWalkingTowardPlayer, (INetSerializable) this.moveTowardPlayerThreshold, (INetSerializable) this.age, (INetSerializable) this.manners, (INetSerializable) this.socialAnxiety, (INetSerializable) this.optimism, (INetSerializable) this.gender, (INetSerializable) this.breather, (INetSerializable) this.isSleeping, (INetSerializable) this.hideShadow, (INetSerializable) this.isInvisible, (INetSerializable) this.defaultMap, (INetSerializable) this.defaultPosition, (INetSerializable) this.removeHenchmanEvent, (INetSerializable) this.doingEndOfRouteAnimation, (INetSerializable) this.goingToDoEndOfRouteAnimation, (INetSerializable) this.endOfRouteMessage, (INetSerializable) this.endOfRouteBehaviorName, (INetSerializable) this.lastSeenMovieWeek, (INetSerializable) this.currentMarriageDialogue, (INetSerializable) this.marriageDefaultDialogue, (INetSerializable) this.shouldSayMarriageDialogue, (INetSerializable) this.hasBeenKissedToday, (INetSerializable) this.syncedPortraitPath, (INetSerializable) this.hasSaidAfternoonDialogue, (INetSerializable) this.dayScheduleName, (INetSerializable) this.islandScheduleName, (INetSerializable) this.sleptInBed, (INetSerializable) this.shouldWearIslandAttire, (INetSerializable) this.isMovingOnPathFindPath);
      this.position.Field.AxisAlignedMovement = true;
      this.removeHenchmanEvent.onEvent += new NetEvent0.Event(this.performRemoveHenchman);
    }

    protected override string translateName(string name)
    {
      switch (name)
      {
        case "Bear":
          return Game1.content.LoadString("Strings\\NPCNames:Bear");
        case "Birdie":
          return Game1.content.LoadString("Strings\\NPCNames:Birdie");
        case "Bouncer":
          return Game1.content.LoadString("Strings\\NPCNames:Bouncer");
        case "Gil":
          return Game1.content.LoadString("Strings\\NPCNames:Gil");
        case "Governor":
          return Game1.content.LoadString("Strings\\NPCNames:Governor");
        case "Grandpa":
          return Game1.content.LoadString("Strings\\NPCNames:Grandpa");
        case "Gunther":
          return Game1.content.LoadString("Strings\\NPCNames:Gunther");
        case "Henchman":
          return Game1.content.LoadString("Strings\\NPCNames:Henchman");
        case "Kel":
          return Game1.content.LoadString("Strings\\NPCNames:Kel");
        case "Marlon":
          return Game1.content.LoadString("Strings\\NPCNames:Marlon");
        case "Mister Qi":
          return Game1.content.LoadString("Strings\\NPCNames:MisterQi");
        case "Morris":
          return Game1.content.LoadString("Strings\\NPCNames:Morris");
        case "Old Mariner":
          return Game1.content.LoadString("Strings\\NPCNames:OldMariner");
        case "Welwick":
          return Game1.content.LoadString("Strings\\NPCNames:Welwick");
        default:
          Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
          if (!dictionary.ContainsKey(name))
            return name;
          string[] strArray = dictionary[name].Split('/');
          return strArray[strArray.Length - 1];
      }
    }

    public string getName() => this.displayName != null && this.displayName.Length > 0 ? this.displayName : this.Name;

    public string getTextureName() => NPC.getTextureNameForCharacter(this.Name);

    public static string getTextureNameForCharacter(string character_name)
    {
      string nameForCharacter = character_name == "Old Mariner" ? "Mariner" : (character_name == "Dwarf King" ? "DwarfKing" : (character_name == "Mister Qi" ? "MrQi" : (character_name == "???" ? "Monsters\\Shadow Guy" : (character_name == "Leo" ? "ParrotBoy" : character_name))));
      if (character_name.Equals(Utility.getOtherFarmerNames()[0]))
        nameForCharacter = Game1.player.IsMale ? "maleRival" : "femaleRival";
      return nameForCharacter;
    }

    public virtual bool PathToOnFarm(
      Point destination,
      PathFindController.endBehavior on_path_success = null)
    {
      this.controller = (PathFindController) null;
      Stack<Point> pathOnFarm = PathFindController.FindPathOnFarm(this.getTileLocationPoint(), destination, this.currentLocation, 2000);
      if (pathOnFarm == null)
        return false;
      this.controller = new PathFindController(pathOnFarm, (Character) this, this.currentLocation);
      this.controller.nonDestructivePathing = true;
      this.ignoreScheduleToday = true;
      this.controller.endBehaviorFunction += on_path_success;
      return true;
    }

    public virtual void OnFinishPathForActivity(Character c, GameLocation location) => this._currentFarmActivity.BeginActivity();

    public void resetPortrait() => this.portrait = (Texture2D) null;

    public void resetSeasonalDialogue() => this.dialogue = (Dictionary<string, string>) null;

    public virtual void reloadSprite()
    {
      string textureName = this.getTextureName();
      if (!this.IsMonster)
      {
        this.Sprite = new AnimatedSprite("Characters\\" + textureName);
        if (!this.Name.Contains("Dwarf") && !this.Name.Equals("Krobus"))
          this.Sprite.SpriteHeight = 32;
      }
      else
        this.Sprite = new AnimatedSprite("Monsters\\" + textureName);
      this.resetPortrait();
      int num = this.IsInvisible ? 1 : 0;
      if (!Game1.newDay && Game1.gameMode != (byte) 6)
        return;
      this.faceDirection(this.DefaultFacingDirection);
      this.previousEndPoint = new Point((int) this.defaultPosition.X / 64, (int) this.defaultPosition.Y / 64);
      this.Schedule = this.getSchedule(Game1.dayOfMonth);
      this.faceDirection(this.defaultFacingDirection);
      this.resetSeasonalDialogue();
      this.resetCurrentDialogue();
      if (this.isMarried() && !(bool) (NetFieldBase<bool, NetBool>) this.getSpouse().divorceTonight && !this.IsInvisible)
        this.marriageDuties();
      this.updateConstructionAnimation();
      try
      {
        this.displayName = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions")[this.Name].Split('/')[11];
      }
      catch (Exception ex)
      {
      }
    }

    private void updateConstructionAnimation()
    {
      bool flag = Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason);
      if (Game1.IsMasterGame && this.Name == "Robin" && !flag)
      {
        if ((int) (NetFieldBase<int, NetInt>) Game1.player.daysUntilHouseUpgrade > 0)
        {
          Farm farm = Game1.getFarm();
          Game1.warpCharacter(this, "Farm", new Vector2((float) (farm.GetMainFarmHouseEntry().X + 4), (float) (farm.GetMainFarmHouseEntry().Y - 1)));
          this.isPlayingRobinHammerAnimation = false;
          this.shouldPlayRobinHammerAnimation.Value = true;
          return;
        }
        if (Game1.getFarm().isThereABuildingUnderConstruction())
        {
          Building underConstruction = Game1.getFarm().getBuildingUnderConstruction();
          if ((int) (NetFieldBase<int, NetInt>) underConstruction.daysUntilUpgrade > 0 && underConstruction.indoors.Value != null)
          {
            if (this.currentLocation != null)
              this.currentLocation.characters.Remove(this);
            this.currentLocation = (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) underConstruction.indoors;
            if (this.currentLocation != null && !this.currentLocation.characters.Contains(this))
              this.currentLocation.addCharacter(this);
            if (underConstruction.nameOfIndoorsWithoutUnique.Contains("Shed"))
            {
              this.setTilePosition(2, 2);
              this.position.X -= 28f;
            }
            else
              this.setTilePosition(1, 5);
          }
          else
          {
            Game1.warpCharacter(this, "Farm", new Vector2((float) ((int) (NetFieldBase<int, NetInt>) underConstruction.tileX + (int) (NetFieldBase<int, NetInt>) underConstruction.tilesWide / 2), (float) ((int) (NetFieldBase<int, NetInt>) underConstruction.tileY + (int) (NetFieldBase<int, NetInt>) underConstruction.tilesHigh / 2)));
            this.position.X += 16f;
            this.position.Y -= 32f;
          }
          this.isPlayingRobinHammerAnimation = false;
          this.shouldPlayRobinHammerAnimation.Value = true;
          return;
        }
        if ((Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade.Value > 0)
        {
          if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
          {
            Game1.warpCharacter(this, "Backwoods", new Vector2(41f, 23f));
            this.isPlayingRobinHammerAnimation = false;
            this.shouldPlayRobinHammerAnimation.Value = true;
            return;
          }
          Game1.warpCharacter(this, "Town", new Vector2(77f, 68f));
          this.isPlayingRobinHammerAnimation = false;
          this.shouldPlayRobinHammerAnimation.Value = true;
          return;
        }
      }
      this.shouldPlayRobinHammerAnimation.Value = false;
    }

    private void doPlayRobinHammerAnimation()
    {
      this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(24, 75),
        new FarmerSprite.AnimationFrame(25, 75),
        new FarmerSprite.AnimationFrame(26, 300, false, false, new AnimatedSprite.endOfAnimationBehavior(this.robinHammerSound)),
        new FarmerSprite.AnimationFrame(27, 1000, false, false, new AnimatedSprite.endOfAnimationBehavior(this.robinVariablePause))
      });
      this.ignoreScheduleToday = true;
      bool flag = (int) (NetFieldBase<int, NetInt>) Game1.player.daysUntilHouseUpgrade == 1 || (int) (NetFieldBase<int, NetInt>) (Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade == 1;
      this.CurrentDialogue.Clear();
      this.CurrentDialogue.Push(new StardewValley.Dialogue(flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3927") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3926"), this));
    }

    public void showTextAboveHead(
      string Text,
      int spriteTextColor = -1,
      int style = 2,
      int duration = 3000,
      int preTimer = 0)
    {
      this.textAboveHeadAlpha = 0.0f;
      this.textAboveHead = Text;
      this.textAboveHeadPreTimer = preTimer;
      this.textAboveHeadTimer = duration;
      this.textAboveHeadStyle = style;
      this.textAboveHeadColor = spriteTextColor;
    }

    public void moveToNewPlaceForEvent(int xTile, int yTile, string oldMap)
    {
      this.mapBeforeEvent = oldMap;
      this.positionBeforeEvent = this.Position;
      this.Position = new Vector2((float) (xTile * 64), (float) (yTile * 64 - 96));
    }

    public virtual bool hitWithTool(Tool t) => false;

    public bool canReceiveThisItemAsGift(Item i)
    {
      switch (i)
      {
        case Object _:
        case Ring _:
        case StardewValley.Objects.Hat _:
        case Boots _:
        case MeleeWeapon _:
        case Clothing _:
          return true;
        default:
          return false;
      }
    }

    public int getGiftTasteForThisItem(Item item)
    {
      int tasteForThisItem = 8;
      if (item is Object)
      {
        Object @object = item as Object;
        string str1;
        Game1.NPCGiftTastes.TryGetValue(this.Name, out str1);
        string[] strArray1 = str1.Split('/');
        int parentSheetIndex = @object.ParentSheetIndex;
        int category = @object.Category;
        string str2 = parentSheetIndex.ToString() ?? "";
        string str3 = category.ToString() ?? "";
        if (((IEnumerable<string>) Game1.NPCGiftTastes["Universal_Love"].Split(' ')).Contains<string>(str3))
          tasteForThisItem = 0;
        else if (((IEnumerable<string>) Game1.NPCGiftTastes["Universal_Hate"].Split(' ')).Contains<string>(str3))
          tasteForThisItem = 6;
        else if (((IEnumerable<string>) Game1.NPCGiftTastes["Universal_Like"].Split(' ')).Contains<string>(str3))
          tasteForThisItem = 2;
        else if (((IEnumerable<string>) Game1.NPCGiftTastes["Universal_Dislike"].Split(' ')).Contains<string>(str3))
          tasteForThisItem = 4;
        if (this.CheckTasteContextTags(item, Game1.NPCGiftTastes["Universal_Love"].Split(' ')))
          tasteForThisItem = 0;
        else if (this.CheckTasteContextTags(item, Game1.NPCGiftTastes["Universal_Hate"].Split(' ')))
          tasteForThisItem = 6;
        else if (this.CheckTasteContextTags(item, Game1.NPCGiftTastes["Universal_Like"].Split(' ')))
          tasteForThisItem = 2;
        else if (this.CheckTasteContextTags(item, Game1.NPCGiftTastes["Universal_Dislike"].Split(' ')))
          tasteForThisItem = 4;
        bool flag1 = false;
        bool flag2 = false;
        if (((IEnumerable<string>) Game1.NPCGiftTastes["Universal_Love"].Split(' ')).Contains<string>(str2))
        {
          tasteForThisItem = 0;
          flag1 = true;
        }
        else if (((IEnumerable<string>) Game1.NPCGiftTastes["Universal_Hate"].Split(' ')).Contains<string>(str2))
        {
          tasteForThisItem = 6;
          flag1 = true;
        }
        else if (((IEnumerable<string>) Game1.NPCGiftTastes["Universal_Like"].Split(' ')).Contains<string>(str2))
        {
          tasteForThisItem = 2;
          flag1 = true;
        }
        else if (((IEnumerable<string>) Game1.NPCGiftTastes["Universal_Dislike"].Split(' ')).Contains<string>(str2))
        {
          tasteForThisItem = 4;
          flag1 = true;
        }
        else if (((IEnumerable<string>) Game1.NPCGiftTastes["Universal_Neutral"].Split(' ')).Contains<string>(str2))
        {
          tasteForThisItem = 8;
          flag1 = true;
          flag2 = true;
        }
        if (@object.type.Contains("Arch"))
        {
          tasteForThisItem = 4;
          if (this.Name.Equals("Penny") || this.name.Equals((object) "Dwarf"))
            tasteForThisItem = 2;
        }
        if (tasteForThisItem == 8 && !flag2)
        {
          if ((int) (NetFieldBase<int, NetInt>) @object.edibility != -300 && (int) (NetFieldBase<int, NetInt>) @object.edibility < 0)
            tasteForThisItem = 6;
          else if ((int) (NetFieldBase<int, NetInt>) @object.price < 20)
            tasteForThisItem = 4;
        }
        if (str1 != null)
        {
          List<string[]> strArrayList = new List<string[]>();
          for (int index1 = 0; index1 < 10; index1 += 2)
          {
            string[] strArray2 = strArray1[index1 + 1].Split(' ');
            string[] strArray3 = new string[strArray2.Length];
            for (int index2 = 0; index2 < strArray2.Length; ++index2)
            {
              if (strArray2[index2].Length > 0)
                strArray3[index2] = strArray2[index2];
            }
            strArrayList.Add(strArray3);
          }
          if (((IEnumerable<string>) strArrayList[0]).Contains<string>(str2))
            return 0;
          if (((IEnumerable<string>) strArrayList[3]).Contains<string>(str2))
            return 6;
          if (((IEnumerable<string>) strArrayList[1]).Contains<string>(str2))
            return 2;
          if (((IEnumerable<string>) strArrayList[2]).Contains<string>(str2))
            return 4;
          if (((IEnumerable<string>) strArrayList[4]).Contains<string>(str2))
            return 8;
          if (this.CheckTasteContextTags(item, strArrayList[0]))
            return 0;
          if (this.CheckTasteContextTags(item, strArrayList[3]))
            return 6;
          if (this.CheckTasteContextTags(item, strArrayList[1]))
            return 2;
          if (this.CheckTasteContextTags(item, strArrayList[2]))
            return 4;
          if (this.CheckTasteContextTags(item, strArrayList[4]))
            return 8;
          if (!flag1)
          {
            if (category != 0 && ((IEnumerable<string>) strArrayList[0]).Contains<string>(str3))
              return 0;
            if (category != 0 && ((IEnumerable<string>) strArrayList[3]).Contains<string>(str3))
              return 6;
            if (category != 0 && ((IEnumerable<string>) strArrayList[1]).Contains<string>(str3))
              return 2;
            if (category != 0 && ((IEnumerable<string>) strArrayList[2]).Contains<string>(str3))
              return 4;
            if (category != 0 && ((IEnumerable<string>) strArrayList[4]).Contains<string>(str3))
              return 8;
          }
        }
      }
      return tasteForThisItem;
    }

    public virtual bool CheckTasteContextTags(Item item, string[] list)
    {
      foreach (string tag in list)
      {
        if (tag != null && tag.Length > 0 && !char.IsNumber(tag[0]) && tag[0] != '-' && item.HasContextTag(tag))
          return true;
      }
      return false;
    }

    private void goblinDoorEndBehavior(Character c, GameLocation l)
    {
      l.characters.Remove(this);
      l.playSound("doorClose");
    }

    private void performRemoveHenchman()
    {
      this.Sprite.CurrentFrame = 4;
      Game1.netWorldState.Value.IsGoblinRemoved = true;
      Game1.player.removeQuest(27);
      Stack<Point> pathToEndPoint = new Stack<Point>();
      pathToEndPoint.Push(new Point(20, 21));
      pathToEndPoint.Push(new Point(20, 22));
      pathToEndPoint.Push(new Point(20, 23));
      pathToEndPoint.Push(new Point(20, 24));
      pathToEndPoint.Push(new Point(20, 25));
      pathToEndPoint.Push(new Point(20, 26));
      pathToEndPoint.Push(new Point(20, 27));
      pathToEndPoint.Push(new Point(20, 28));
      this.addedSpeed = 2;
      this.controller = new PathFindController(pathToEndPoint, (Character) this, this.currentLocation);
      this.controller.endBehaviorFunction = new PathFindController.endBehavior(this.goblinDoorEndBehavior);
      this.showTextAboveHead(Game1.content.LoadString("Strings\\Characters:Henchman6"));
      Game1.player.mailReceived.Add("henchmanGone");
      this.currentLocation.removeTile(20, 29, "Buildings");
    }

    private void engagementResponse(Farmer who, bool asRoommate = false)
    {
      Game1.changeMusicTrack("none");
      who.spouse = this.Name;
      if (!asRoommate)
        Game1.multiplayer.globalChatInfoMessage("Engaged", Game1.player.Name, this.displayName);
      Friendship friendship = who.friendshipData[this.Name];
      friendship.Status = FriendshipStatus.Engaged;
      friendship.RoommateMarriage = asRoommate;
      WorldDate worldDate = new WorldDate(Game1.Date);
      worldDate.TotalDays += 3;
      while (!Game1.canHaveWeddingOnDay(worldDate.DayOfMonth, worldDate.Season))
        ++worldDate.TotalDays;
      friendship.WeddingDate = worldDate;
      this.CurrentDialogue.Clear();
      if (asRoommate && Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue").ContainsKey(this.Name + "Roommate0"))
      {
        this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")[this.Name + "Roommate0"], this));
        string masterDialogue1 = Game1.content.LoadStringReturnNullIfNotFound("Strings\\StringsFromCSFiles:" + this.Name + "_EngagedRoommate");
        if (masterDialogue1 != null)
        {
          this.CurrentDialogue.Push(new StardewValley.Dialogue(masterDialogue1, this));
        }
        else
        {
          string masterDialogue2 = Game1.content.LoadStringReturnNullIfNotFound("Strings\\StringsFromCSFiles:" + this.Name + "_Engaged");
          if (masterDialogue2 != null)
            this.CurrentDialogue.Push(new StardewValley.Dialogue(masterDialogue2, this));
          else
            this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3980"), this));
        }
      }
      else
      {
        if (Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue").ContainsKey(this.Name + "0"))
          this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")[this.Name + "0"], this));
        string masterDialogue = Game1.content.LoadStringReturnNullIfNotFound("Strings\\StringsFromCSFiles:" + this.Name + "_Engaged");
        if (masterDialogue != null)
          this.CurrentDialogue.Push(new StardewValley.Dialogue(masterDialogue, this));
        else
          this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3980"), this));
      }
      who.changeFriendship(1, this);
      who.reduceActiveItemByOne();
      who.completelyStopAnimatingOrDoingAction();
      Game1.drawDialogue(this);
    }

    public virtual void tryToReceiveActiveObject(Farmer who)
    {
      who.Halt();
      who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
      if (this.name.Equals((object) "Henchman") && Game1.currentLocation.name.Equals((object) "WitchSwamp"))
      {
        if (who.ActiveObject != null && (int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex == 308)
        {
          if (this.controller != null)
            return;
          who.currentLocation.localSound("coin");
          who.reduceActiveItemByOne();
          this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\Characters:Henchman5"), this));
          Game1.drawDialogue(this);
          who.freezePause = 2000;
          this.removeHenchmanEvent.Fire();
        }
        else
        {
          if (who.ActiveObject == null)
            return;
          if ((int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex == 684)
            this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\Characters:Henchman4"), this));
          else
            this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\Characters:Henchman3"), this));
          Game1.drawDialogue(this);
        }
      }
      else
      {
        if (Game1.player.team.specialOrders != null)
        {
          foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
          {
            if (specialOrder.onItemDelivered != null)
            {
              foreach (Func<Farmer, NPC, Item, int> invocation in specialOrder.onItemDelivered.GetInvocationList())
              {
                if (invocation(Game1.player, this, (Item) who.ActiveObject) > 0)
                {
                  if (who.ActiveObject.Stack > 0)
                    return;
                  who.ActiveObject = (Object) null;
                  who.showNotCarrying();
                  return;
                }
              }
            }
          }
        }
        if (Game1.questOfTheDay != null && (bool) (NetFieldBase<bool, NetBool>) Game1.questOfTheDay.accepted && !(bool) (NetFieldBase<bool, NetBool>) Game1.questOfTheDay.completed && Game1.questOfTheDay is ItemDeliveryQuest && Game1.questOfTheDay.checkIfComplete(this, number2: -1, item: ((Item) who.ActiveObject)))
        {
          who.reduceActiveItemByOne();
          who.completelyStopAnimatingOrDoingAction();
          if (Game1.random.NextDouble() >= 0.3 || this.Name.Equals("Wizard"))
            return;
          this.doEmote(32);
        }
        else if (Game1.questOfTheDay != null && Game1.questOfTheDay is FishingQuest && Game1.questOfTheDay.checkIfComplete(this, who.ActiveObject.ParentSheetIndex, 1))
        {
          who.reduceActiveItemByOne();
          who.completelyStopAnimatingOrDoingAction();
          if (Game1.random.NextDouble() >= 0.3 || this.Name.Equals("Wizard"))
            return;
          this.doEmote(32);
        }
        else if (who.ActiveObject != null && Utility.IsNormalObjectAtParentSheetIndex((Item) who.ActiveObject, 897))
        {
          if (this.Name.Equals("Pierre") && !Game1.player.hasOrWillReceiveMail("PierreStocklist"))
          {
            Game1.addMail("PierreStocklist", true, true);
            who.reduceActiveItemByOne();
            who.completelyStopAnimatingOrDoingAction();
            who.currentLocation.localSound("give_gift");
            Game1.player.team.itemsToRemoveOvernight.Add(897);
            this.setNewDialogue(Game1.content.LoadString("Strings\\Characters:PierreStockListDialogue"), true);
            Game1.drawDialogue(this);
            Game1.afterDialogues += (Game1.afterFadeFunction) (() => Game1.multiplayer.globalChatInfoMessage("StockList"));
          }
          else
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
        }
        else if (who.ActiveObject != null && !(bool) (NetFieldBase<bool, NetBool>) who.ActiveObject.bigCraftable && (int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex == 71 && this.Name.Equals("Lewis") && who.hasQuest(102))
        {
          if (who.currentLocation != null && who.currentLocation.Name == "IslandSouth")
            Game1.player.activeDialogueEvents["lucky_pants_lewis"] = 28;
          who.completeQuest(102);
          Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Quests");
          this.setNewDialogue(dictionary[102].Length > 9 ? dictionary[102].Split('/')[9] : Game1.content.LoadString("Data\\ExtraDialogue:LostItemQuest_DefaultThankYou"));
          Game1.drawDialogue(this);
          Game1.player.changeFriendship(250, this);
          who.ActiveObject = (Object) null;
        }
        else
        {
          int parentSheetIndex1;
          if (who.ActiveObject != null)
          {
            Dictionary<string, string> dialogue1 = this.Dialogue;
            parentSheetIndex1 = who.ActiveObject.ParentSheetIndex;
            string key1 = "reject_" + parentSheetIndex1.ToString();
            if (dialogue1.ContainsKey(key1))
            {
              Dictionary<string, string> dialogue2 = this.Dialogue;
              parentSheetIndex1 = who.ActiveObject.ParentSheetIndex;
              string key2 = "reject_" + parentSheetIndex1.ToString();
              this.setNewDialogue(dialogue2[key2]);
              Game1.drawDialogue(this);
              return;
            }
          }
          if (who.ActiveObject != null && (bool) (NetFieldBase<bool, NetBool>) who.ActiveObject.questItem)
          {
            if (who.hasQuest(130))
            {
              Dictionary<string, string> dialogue3 = this.Dialogue;
              parentSheetIndex1 = who.ActiveObject.ParentSheetIndex;
              string key3 = "accept_" + parentSheetIndex1.ToString();
              if (dialogue3.ContainsKey(key3))
              {
                Dictionary<string, string> dialogue4 = this.Dialogue;
                parentSheetIndex1 = who.ActiveObject.ParentSheetIndex;
                string key4 = "accept_" + parentSheetIndex1.ToString();
                this.setNewDialogue(dialogue4[key4]);
                Game1.drawDialogue(this);
                this.CurrentDialogue.Peek().onFinish = (Action) (() =>
                {
                  int parentSheetIndex2 = who.ActiveObject.ParentSheetIndex;
                  Object o = new Object(who.ActiveObject.ParentSheetIndex + 1, 1)
                  {
                    specialItem = true
                  };
                  o.questItem.Value = true;
                  who.reduceActiveItemByOne();
                  DelayedAction.playSoundAfterDelay("coin", 200);
                  DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => who.addItemByMenuIfNecessary((Item) o)), 200);
                  Game1.player.freezePause = 550;
                  DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1919", (object) o.DisplayName, (object) Lexicon.getProperArticleForWord(o.DisplayName)))), 550);
                });
                return;
              }
            }
            if (who.checkForQuestComplete(this, -1, -1, (Item) who.ActiveObject, "", 9, 3) || !((string) (NetFieldBase<string, NetString>) this.name != "Birdie"))
              return;
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3954"));
          }
          else
          {
            if (who.checkForQuestComplete(this, -1, -1, (Item) null, "", 10))
              return;
            if ((int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex == 809 && !(bool) (NetFieldBase<bool, NetBool>) who.ActiveObject.bigCraftable)
            {
              if (!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
              else if (this.Name.Equals("Dwarf") && !who.canUnderstandDwarves)
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
              else if (this.Name.Equals("Krobus") && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) == "Fri")
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
              else if (!this.CanSocialize && !this.Name.Equals("Dwarf") || !this.isVillager())
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_CantInvite", (object) this.displayName)));
              else if (!who.friendshipData.ContainsKey(this.Name))
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
              else if (who.friendshipData[this.Name].IsDivorced())
              {
                if (who == Game1.player)
                  Game1.multiplayer.globalChatInfoMessage("MovieInviteReject", Game1.player.displayName, this.displayName);
                this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\Characters:Divorced_gift"), this));
                Game1.drawDialogue(this);
              }
              else if (who.lastSeenMovieWeek.Value >= Game1.Date.TotalWeeks)
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_FarmerAlreadySeen")));
              else if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_Festival")));
              else if (Game1.timeOfDay > 2100)
              {
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_Closed")));
              }
              else
              {
                foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
                {
                  if (movieInvitation.farmer == who)
                  {
                    Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_AlreadyInvitedSomeone", (object) movieInvitation.invitedNPC.displayName)));
                    return;
                  }
                }
                this.faceTowardFarmerForPeriod(4000, 3, false, who);
                foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
                {
                  if (movieInvitation.invitedNPC == this)
                  {
                    if (who == Game1.player)
                      Game1.multiplayer.globalChatInfoMessage("MovieInviteReject", Game1.player.displayName, this.displayName);
                    this.CurrentDialogue.Push(new StardewValley.Dialogue(this.GetDispositionModifiedString("Strings\\Characters:MovieInvite_InvitedBySomeoneElse", (object) movieInvitation.farmer.displayName), this));
                    Game1.drawDialogue(this);
                    return;
                  }
                }
                if (this.lastSeenMovieWeek.Value >= Game1.Date.TotalWeeks)
                {
                  if (who == Game1.player)
                    Game1.multiplayer.globalChatInfoMessage("MovieInviteReject", Game1.player.displayName, this.displayName);
                  this.CurrentDialogue.Push(new StardewValley.Dialogue(this.GetDispositionModifiedString("Strings\\Characters:MovieInvite_AlreadySeen"), this));
                  Game1.drawDialogue(this);
                }
                else if (MovieTheater.GetResponseForMovie(this) == "reject")
                {
                  if (who == Game1.player)
                    Game1.multiplayer.globalChatInfoMessage("MovieInviteReject", Game1.player.displayName, this.displayName);
                  this.CurrentDialogue.Push(new StardewValley.Dialogue(this.GetDispositionModifiedString("Strings\\Characters:MovieInvite_Reject"), this));
                  Game1.drawDialogue(this);
                }
                else
                {
                  if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en && this.getSpouse() != null && this.getSpouse().Equals((object) who) && (string) (NetFieldBase<string, NetString>) this.name != "Krobus")
                    this.CurrentDialogue.Push(new StardewValley.Dialogue(this.GetDispositionModifiedString("Strings\\Characters:MovieInvite_Spouse_" + (string) (NetFieldBase<string, NetString>) this.name), this));
                  else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en && this.dialogue != null && this.dialogue.ContainsKey("MovieInvitation"))
                    this.CurrentDialogue.Push(new StardewValley.Dialogue(this.dialogue["MovieInvitation"], this));
                  else
                    this.CurrentDialogue.Push(new StardewValley.Dialogue(this.GetDispositionModifiedString("Strings\\Characters:MovieInvite_Invited"), this));
                  Game1.drawDialogue(this);
                  who.reduceActiveItemByOne();
                  who.completelyStopAnimatingOrDoingAction();
                  who.currentLocation.localSound("give_gift");
                  MovieTheater.Invite(who, this);
                  if (who != Game1.player)
                    return;
                  Game1.multiplayer.globalChatInfoMessage("MovieInviteAccept", Game1.player.displayName, this.displayName);
                }
              }
            }
            else
            {
              if (!Game1.NPCGiftTastes.ContainsKey(this.Name))
                return;
              foreach (string key in who.activeDialogueEvents.Keys)
              {
                if (key.Contains("dumped") && this.Dialogue.ContainsKey(key))
                {
                  this.doEmote(12);
                  return;
                }
              }
              who.completeQuest(25);
              string str = this.Name.ToLower().Replace(' ', '_');
              if (who.ActiveObject.HasContextTag("propose_roommate_" + str))
              {
                if (who.getFriendshipHeartLevelForNPC(this.Name) >= 10 && (int) (NetFieldBase<int, NetInt>) who.houseUpgradeLevel >= 1 && !who.isMarried() && !who.isEngaged())
                  this.engagementResponse(who, true);
                else
                  Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
              }
              else if (who.ActiveObject.ParentSheetIndex == 808 && this.Name.Equals("Krobus"))
              {
                if (who.getFriendshipHeartLevelForNPC(this.Name) < 10 || (int) (NetFieldBase<int, NetInt>) who.houseUpgradeLevel < 1 || who.isMarried() || who.isEngaged())
                  return;
                this.engagementResponse(who, true);
              }
              else if (who.ActiveObject.ParentSheetIndex == 458)
              {
                if (!(bool) (NetFieldBase<bool, NetBool>) this.datable || who.spouse != this.Name && this.isMarriedOrEngaged())
                {
                  if (Game1.random.NextDouble() < 0.5)
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3955", (object) this.displayName));
                  }
                  else
                  {
                    this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3956") : Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.3957"), this));
                    Game1.drawDialogue(this);
                  }
                }
                else if ((bool) (NetFieldBase<bool, NetBool>) this.datable && who.friendshipData.ContainsKey(this.Name) && who.friendshipData[this.Name].IsDating())
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:AlreadyDatingBouquet", (object) this.displayName));
                else if ((bool) (NetFieldBase<bool, NetBool>) this.datable && who.friendshipData.ContainsKey(this.Name) && who.friendshipData[this.Name].IsDivorced())
                {
                  this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\Characters:Divorced_bouquet"), this));
                  Game1.drawDialogue(this);
                }
                else if ((bool) (NetFieldBase<bool, NetBool>) this.datable && who.friendshipData.ContainsKey(this.Name) && who.friendshipData[this.Name].Points < 1000)
                {
                  this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3958") : Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.3959"), this));
                  Game1.drawDialogue(this);
                }
                else if ((bool) (NetFieldBase<bool, NetBool>) this.datable && who.friendshipData.ContainsKey(this.Name) && who.friendshipData[this.Name].Points < 2000)
                {
                  this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3960") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3961"), this));
                  Game1.drawDialogue(this);
                }
                else
                {
                  Friendship friendship = who.friendshipData[this.Name];
                  if (!friendship.IsDating())
                  {
                    friendship.Status = FriendshipStatus.Dating;
                    Game1.multiplayer.globalChatInfoMessage("Dating", Game1.player.Name, this.displayName);
                  }
                  this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.random.NextDouble() < 0.5 ? Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.3962") : Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.3963"), this));
                  who.changeFriendship(25, this);
                  who.reduceActiveItemByOne();
                  who.completelyStopAnimatingOrDoingAction();
                  this.doEmote(20);
                  Game1.drawDialogue(this);
                }
              }
              else if (who.ActiveObject.ParentSheetIndex == 277)
              {
                if (!(bool) (NetFieldBase<bool, NetBool>) this.datable || who.friendshipData.ContainsKey(this.Name) && !who.friendshipData[this.Name].IsDating() || who.friendshipData.ContainsKey(this.Name) && who.friendshipData[this.Name].IsMarried())
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Wilted_Bouquet_Meaningless", (object) this.displayName));
                }
                else
                {
                  if (!who.friendshipData.ContainsKey(this.Name) || !who.friendshipData[this.Name].IsDating())
                    return;
                  Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Wilted_Bouquet_Effect", (object) this.displayName));
                  Game1.multiplayer.globalChatInfoMessage("BreakUp", Game1.player.Name, this.displayName);
                  who.reduceActiveItemByOne();
                  who.friendshipData[this.Name].Status = FriendshipStatus.Friendly;
                  who.completelyStopAnimatingOrDoingAction();
                  who.friendshipData[this.Name].Points = Math.Min(who.friendshipData[this.Name].Points, 1250);
                  string name = (string) (NetFieldBase<string, NetString>) this.name;
                  if (!(name == "Maru") && !(name == "Haley"))
                  {
                    if (!(name == "Shane") && !(name == "Alex"))
                      this.doEmote(28);
                  }
                  else
                    this.doEmote(12);
                  this.CurrentDialogue.Clear();
                  this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Characters\\Dialogue\\" + this.GetDialogueSheetName() + ":breakUp"), this));
                  Game1.drawDialogue(this);
                }
              }
              else if (who.ActiveObject.ParentSheetIndex == 460)
              {
                if (who.isMarried() || who.isEngaged())
                {
                  if (who.hasCurrentOrPendingRoommate())
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:TriedToMarryButKrobus"));
                  else if (who.isEngaged())
                  {
                    this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.random.NextDouble() < 0.5 ? Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.3965") : Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.3966"), this));
                    Game1.drawDialogue(this);
                  }
                  else
                  {
                    this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3967") : Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.3968"), this));
                    Game1.drawDialogue(this);
                  }
                }
                else if (!(bool) (NetFieldBase<bool, NetBool>) this.datable || this.isMarriedOrEngaged() || who.friendshipData.ContainsKey(this.Name) && who.friendshipData[this.Name].IsDivorced() || who.friendshipData.ContainsKey(this.Name) && who.friendshipData[this.Name].Points < 1500)
                {
                  if (Game1.random.NextDouble() < 0.5)
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3969", (object) this.displayName));
                  }
                  else
                  {
                    this.CurrentDialogue.Push(new StardewValley.Dialogue(this.Gender == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3970") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3971"), this));
                    Game1.drawDialogue(this);
                  }
                }
                else if ((bool) (NetFieldBase<bool, NetBool>) this.datable && who.friendshipData.ContainsKey(this.Name) && who.friendshipData[this.Name].Points < 2500)
                {
                  if (!who.friendshipData[this.Name].ProposalRejected)
                  {
                    this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3972") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3973"), this));
                    Game1.drawDialogue(this);
                    who.changeFriendship(-20, this);
                    who.friendshipData[this.Name].ProposalRejected = true;
                  }
                  else
                  {
                    this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.random.NextDouble() < 0.5 ? Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.3974") : Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.3975"), this));
                    Game1.drawDialogue(this);
                    who.changeFriendship(-50, this);
                  }
                }
                else if ((bool) (NetFieldBase<bool, NetBool>) this.datable && (int) (NetFieldBase<int, NetInt>) who.houseUpgradeLevel < 1)
                {
                  if (Game1.random.NextDouble() < 0.5)
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3969", (object) this.displayName));
                  }
                  else
                  {
                    this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3972"), this));
                    Game1.drawDialogue(this);
                  }
                }
                else
                  this.engagementResponse(who);
              }
              else if (who.friendshipData.ContainsKey(this.Name) && who.friendshipData[this.Name].GiftsThisWeek < 2 || who.spouse != null && who.spouse.Equals(this.Name) || this is Child || this.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
              {
                if (who.friendshipData[this.Name].IsDivorced())
                {
                  this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\Characters:Divorced_gift"), this));
                  Game1.drawDialogue(this);
                }
                else if (who.friendshipData[this.Name].GiftsToday == 1)
                {
                  Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3981", (object) this.displayName)));
                }
                else
                {
                  this.receiveGift(who.ActiveObject, who);
                  who.reduceActiveItemByOne();
                  who.completelyStopAnimatingOrDoingAction();
                  this.faceTowardFarmerForPeriod(4000, 3, false, who);
                  if (!(bool) (NetFieldBase<bool, NetBool>) this.datable || who.spouse == null || !(who.spouse != this.Name) || who.hasCurrentOrPendingRoommate() || Utility.isMale(who.spouse) != Utility.isMale(this.Name) || Game1.random.NextDouble() >= 0.3 - (double) who.LuckLevel / 100.0 - who.DailyLuck || this.isBirthday(Game1.currentSeason, Game1.dayOfMonth) || !who.friendshipData[this.Name].IsDating())
                    return;
                  NPC characterFromName = Game1.getCharacterFromName(who.spouse);
                  who.changeFriendship(-30, characterFromName);
                  characterFromName.CurrentDialogue.Clear();
                  characterFromName.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3985", (object) this.displayName), characterFromName));
                }
              }
              else
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3987", (object) this.displayName, (object) 2)));
            }
          }
        }
      }
    }

    public string GetDispositionModifiedString(string path, params object[] substitutions)
    {
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      List<string> stringList = new List<string>();
      string str = "";
      stringList.Add(this.name.Value);
      if (Game1.player.isMarried() && Game1.player.getSpouse() == this)
        stringList.Add("spouse");
      string name = this.Name;
      ref string local = ref str;
      if (dictionary.TryGetValue(name, out local))
      {
        string[] strArray = str.Split('/');
        if (strArray.Length > 4)
        {
          stringList.Add(strArray[1]);
          stringList.Add(strArray[2]);
          stringList.Add(strArray[3]);
          stringList.Add(strArray[0]);
        }
      }
      foreach (string s in stringList)
      {
        string path1 = path + "_" + Utility.capitalizeFirstLetter(s);
        string dispositionModifiedString = Game1.content.LoadString(path1, substitutions);
        if (!(dispositionModifiedString == path1))
          return dispositionModifiedString;
      }
      return Game1.content.LoadString(path, substitutions);
    }

    public void haltMe(Farmer who) => this.Halt();

    public virtual bool checkAction(Farmer who, GameLocation l)
    {
      if (this.IsInvisible)
        return false;
      if (this.isSleeping.Value)
      {
        if (!this.isEmoting)
          this.doEmote(24);
        this.shake(250);
        return false;
      }
      if (!who.CanMove)
        return false;
      if (this.Name.Equals("Henchman") && l.Name.Equals("WitchSwamp"))
      {
        if (!Game1.player.mailReceived.Contains("Henchman1"))
        {
          Game1.player.mailReceived.Add("Henchman1");
          this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\Characters:Henchman1"), this));
          Game1.drawDialogue(this);
          Game1.player.addQuest(27);
          Game1.player.friendshipData.Add("Henchman", new Friendship());
        }
        else
        {
          if (who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift() && !who.isRidingHorse())
          {
            this.tryToReceiveActiveObject(who);
            return true;
          }
          if (this.controller == null)
          {
            this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\Characters:Henchman2"), this));
            Game1.drawDialogue(this);
          }
        }
        return true;
      }
      bool flag1 = false;
      if (who.pantsItem.Value != null && (int) (NetFieldBase<int, NetInt>) who.pantsItem.Value.parentSheetIndex == 15 && (this.Name.Equals("Lewis") || this.Name.Equals("Marnie")))
        flag1 = true;
      if (Game1.NPCGiftTastes.ContainsKey(this.Name) && !Game1.player.friendshipData.ContainsKey(this.Name))
      {
        Game1.player.friendshipData.Add(this.Name, new Friendship(0));
        if (this.Name.Equals("Krobus"))
        {
          this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3990"), this));
          Game1.drawDialogue(this);
          return true;
        }
      }
      if (who.checkForQuestComplete(this, -1, -1, (Item) who.ActiveObject, (string) null, questTypeToIgnore: 5))
      {
        this.faceTowardFarmerForPeriod(6000, 3, false, who);
        return true;
      }
      if (this.Name.Equals("Krobus") && who.hasQuest(28))
      {
        this.CurrentDialogue.Push(new StardewValley.Dialogue(l is Sewer ? Game1.content.LoadString("Strings\\Characters:KrobusDarkTalisman") : Game1.content.LoadString("Strings\\Characters:KrobusDarkTalisman_elsewhere"), this));
        Game1.drawDialogue(this);
        who.removeQuest(28);
        who.mailReceived.Add("krobusUnseal");
        if (l is Sewer)
        {
          DelayedAction.addTemporarySpriteAfterDelay(new TemporaryAnimatedSprite("TileSheets\\Projectiles", new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16), 3000f, 1, 0, new Vector2(31f, 17f) * 64f, false, false)
          {
            scale = 4f,
            delayBeforeAnimationStart = 1,
            startSound = "debuffSpell",
            motion = new Vector2(-9f, 1f),
            rotationChange = (float) Math.PI / 64f,
            light = true,
            lightRadius = 1f,
            lightcolor = new Microsoft.Xna.Framework.Color(150, 0, 50),
            layerDepth = 1f,
            alphaFade = 3f / 1000f
          }, l, 200, true);
          DelayedAction.addTemporarySpriteAfterDelay(new TemporaryAnimatedSprite("TileSheets\\Projectiles", new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16), 3000f, 1, 0, new Vector2(31f, 17f) * 64f, false, false)
          {
            startSound = "debuffSpell",
            delayBeforeAnimationStart = 1,
            scale = 4f,
            motion = new Vector2(-9f, 1f),
            rotationChange = (float) Math.PI / 64f,
            light = true,
            lightRadius = 1f,
            lightcolor = new Microsoft.Xna.Framework.Color(150, 0, 50),
            layerDepth = 1f,
            alphaFade = 3f / 1000f
          }, l, 700, true);
        }
        return true;
      }
      if (this.Name.Equals(who.spouse) && who.IsLocalPlayer)
      {
        int timeOfDay = Game1.timeOfDay;
        if (this.Sprite.CurrentAnimation == null)
          this.faceDirection(-3);
        if (this.Sprite.CurrentAnimation == null && who.friendshipData.ContainsKey((string) (NetFieldBase<string, NetString>) this.name) && who.friendshipData[(string) (NetFieldBase<string, NetString>) this.name].Points >= 3125 && !who.mailReceived.Contains("CF_Spouse"))
        {
          this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString(Game1.player.isRoommate(who.spouse) ? "Strings\\StringsFromCSFiles:Krobus_Stardrop" : "Strings\\StringsFromCSFiles:NPC.cs.4001"), this));
          Game1.player.addItemByMenuIfNecessary((Item) new Object(Vector2.Zero, 434, "Cosmic Fruit", false, false, false, false));
          this.shouldSayMarriageDialogue.Value = false;
          this.currentMarriageDialogue.Clear();
          who.mailReceived.Add("CF_Spouse");
          return true;
        }
        if (this.Sprite.CurrentAnimation == null && !this.hasTemporaryMessageAvailable() && this.currentMarriageDialogue.Count == 0 && this.CurrentDialogue.Count == 0 && Game1.timeOfDay < 2200 && !this.isMoving() && who.ActiveObject == null)
        {
          this.faceGeneralDirection(who.getStandingPosition(), 0, false, false);
          who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
          if (this.FacingDirection == 3 || this.FacingDirection == 1)
          {
            int frame = 28;
            bool flag2 = true;
            switch (this.Name)
            {
              case "Abigail":
                frame = 33;
                flag2 = false;
                break;
              case "Alex":
                frame = 42;
                flag2 = true;
                break;
              case "Elliott":
                frame = 35;
                flag2 = false;
                break;
              case "Emily":
                frame = 33;
                flag2 = false;
                break;
              case "Harvey":
                frame = 31;
                flag2 = false;
                break;
              case "Krobus":
                frame = 16;
                flag2 = true;
                break;
              case "Leah":
                frame = 25;
                flag2 = true;
                break;
              case "Maru":
                frame = 28;
                flag2 = false;
                break;
              case "Penny":
                frame = 35;
                flag2 = true;
                break;
              case "Sam":
                frame = 36;
                flag2 = true;
                break;
              case "Sebastian":
                frame = 40;
                flag2 = false;
                break;
              case "Shane":
                frame = 34;
                flag2 = false;
                break;
            }
            bool flip = flag2 && this.FacingDirection == 3 || !flag2 && this.FacingDirection == 1;
            if (who.getFriendshipHeartLevelForNPC(this.Name) > 9 && this.sleptInBed.Value)
            {
              int milliseconds = Game1.IsMultiplayer ? 1000 : 10;
              this.movementPause = milliseconds;
              this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
              {
                new FarmerSprite.AnimationFrame(frame, milliseconds, false, flip, new AnimatedSprite.endOfAnimationBehavior(this.haltMe), true)
              });
              if (!this.hasBeenKissedToday.Value)
              {
                who.changeFriendship(10, this);
                if (who.hasCurrentOrPendingRoommate())
                  Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\emojis", new Microsoft.Xna.Framework.Rectangle(0, 0, 9, 9), 2000f, 1, 0, new Vector2((float) this.getTileX(), (float) this.getTileY()) * 64f + new Vector2(16f, -64f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(0.0f, -0.5f),
                    alphaFade = 0.01f
                  });
                else
                  Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), 2000f, 1, 0, new Vector2((float) this.getTileX(), (float) this.getTileY()) * 64f + new Vector2(16f, -64f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(0.0f, -0.5f),
                    alphaFade = 0.01f
                  });
                l.playSound("dwop", NetAudio.SoundContext.NPC);
                who.exhausted.Value = false;
              }
              this.hasBeenKissedToday.Value = true;
              this.Sprite.UpdateSourceRect();
            }
            else
            {
              this.faceDirection(Game1.random.NextDouble() < 0.5 ? 2 : 0);
              this.doEmote(12);
            }
            int facingDirection = 1;
            if (flag2 && !flip || !flag2 & flip)
              facingDirection = 3;
            who.PerformKiss(facingDirection);
            return true;
          }
        }
      }
      bool flag3 = false;
      if (who.friendshipData.ContainsKey(this.Name) || this.Name == "Mister Qi")
      {
        if (this.getSpouse() == Game1.player && this.shouldSayMarriageDialogue.Value && this.currentMarriageDialogue.Count > 0 && this.currentMarriageDialogue.Count > 0)
        {
          while (this.currentMarriageDialogue.Count > 0)
          {
            MarriageDialogueReference dialogueReference = this.currentMarriageDialogue[this.currentMarriageDialogue.Count - 1];
            if (dialogueReference == this.marriageDefaultDialogue.Value)
              this.marriageDefaultDialogue.Value = (MarriageDialogueReference) null;
            this.currentMarriageDialogue.RemoveAt(this.currentMarriageDialogue.Count - 1);
            this.CurrentDialogue.Push(dialogueReference.GetDialogue(this));
          }
          flag3 = true;
        }
        if (!flag3)
        {
          flag3 = this.checkForNewCurrentDialogue(who.friendshipData.ContainsKey(this.Name) ? who.friendshipData[this.Name].Points / 250 : 0);
          if (!flag3)
            flag3 = this.checkForNewCurrentDialogue(who.friendshipData.ContainsKey(this.Name) ? who.friendshipData[this.Name].Points / 250 : 0, true);
        }
      }
      if (who.IsLocalPlayer && who.friendshipData.ContainsKey(this.Name) && (this.endOfRouteMessage.Value != null | flag3 || this.currentLocation != null && this.currentLocation.HasLocationOverrideDialogue(this)))
      {
        if (!flag3 && this.setTemporaryMessages(who))
        {
          Game1.player.checkForQuestComplete(this, -1, -1, (Item) null, (string) null, 5);
          return false;
        }
        if (this.Sprite.Texture.Bounds.Height > 32)
          this.faceTowardFarmerForPeriod(5000, 4, false, who);
        if (who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift() && !who.isRidingHorse())
        {
          this.tryToReceiveActiveObject(who);
          this.faceTowardFarmerForPeriod(3000, 4, false, who);
          return true;
        }
        this.grantConversationFriendship(who);
        Game1.drawDialogue(this);
        return true;
      }
      if (this.canTalk() && who.hasClubCard && this.Name.Equals("Bouncer") && who.IsLocalPlayer)
      {
        Response[] answerChoices = new Response[2]
        {
          new Response("Yes.", Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4018")),
          new Response("That's", Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4020"))
        };
        l.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4021"), answerChoices, "ClubCard");
      }
      else if (this.canTalk() && this.CurrentDialogue.Count > 0)
      {
        if (!this.Name.Contains("King") && who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift() && !who.isRidingHorse())
        {
          if (who.IsLocalPlayer)
            this.tryToReceiveActiveObject(who);
          else
            this.faceTowardFarmerForPeriod(3000, 4, false, who);
          return true;
        }
        if (this.CurrentDialogue.Count >= 1 || this.endOfRouteMessage.Value != null || this.currentLocation != null && this.currentLocation.HasLocationOverrideDialogue(this))
        {
          if (this.setTemporaryMessages(who))
          {
            Game1.player.checkForQuestComplete(this, -1, -1, (Item) null, (string) null, 5);
            return false;
          }
          if (this.Sprite.Texture.Bounds.Height > 32)
            this.faceTowardFarmerForPeriod(5000, 4, false, who);
          if (who.IsLocalPlayer)
          {
            this.grantConversationFriendship(who);
            if (!flag1)
            {
              Game1.drawDialogue(this);
              return true;
            }
          }
        }
        else if (!(bool) (NetFieldBase<bool, NetBool>) this.doingEndOfRouteAnimation)
        {
          try
          {
            if (who.friendshipData.ContainsKey(this.Name))
              this.faceTowardFarmerForPeriod(who.friendshipData[this.Name].Points / 125 * 1000 + 1000, 4, false, who);
          }
          catch (Exception ex)
          {
          }
          if (Game1.random.NextDouble() < 0.1)
            this.doEmote(8);
        }
      }
      else if (this.canTalk() && !Game1.game1.wasAskedLeoMemory && Game1.CurrentEvent == null && (string) (NetFieldBase<string, NetString>) this.name == "Leo" && this.currentLocation != null && (this.currentLocation.NameOrUniqueName == "LeoTreeHouse" || this.currentLocation.NameOrUniqueName == "Mountain") && Game1.MasterPlayer.hasOrWillReceiveMail("leoMoved") && this.GetUnseenLeoEvent().HasValue && this.CanRevisitLeoMemory(this.GetUnseenLeoEvent()))
      {
        Game1.drawDialogue(this, Game1.content.LoadString("Strings\\Characters:Leo_Memory"));
        Game1.afterDialogues += new Game1.afterFadeFunction(this.AskLeoMemoryPrompt);
      }
      else
      {
        if (who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift() && !who.isRidingHorse())
        {
          if (this.Name.Equals("Bouncer"))
            return true;
          this.tryToReceiveActiveObject(who);
          this.faceTowardFarmerForPeriod(3000, 4, false, who);
          return true;
        }
        if (this.Name.Equals("Krobus"))
        {
          if (l is Sewer)
          {
            Game1.activeClickableMenu = (IClickableMenu) new ShopMenu((l as Sewer).getShadowShopStock(), who: "Krobus", on_purchase: new Func<ISalable, Farmer, int, bool>((l as Sewer).onShopPurchase));
            return true;
          }
        }
        else if (this.Name.Equals("Dwarf") && who.canUnderstandDwarves && l is Mine)
        {
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(Utility.getDwarfShopStock(), who: "Dwarf");
          return true;
        }
      }
      if (flag1)
      {
        if ((double) this.yJumpVelocity != 0.0 || this.Sprite.CurrentAnimation != null)
          return true;
        if (this.Name.Equals("Lewis"))
        {
          this.faceTowardFarmerForPeriod(1000, 3, false, who);
          this.jump();
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(26, 1000, false, false, (AnimatedSprite.endOfAnimationBehavior) (x => this.doEmote(12)), true)
          });
          this.Sprite.loop = false;
          this.shakeTimer = 1000;
          l.playSound("batScreech");
        }
        else if (this.Name.Equals("Marnie"))
        {
          this.faceTowardFarmerForPeriod(1000, 3, false, who);
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(33, 150, false, false, (AnimatedSprite.endOfAnimationBehavior) (x => l.playSound("dustMeep"))),
            new FarmerSprite.AnimationFrame(34, 180),
            new FarmerSprite.AnimationFrame(33, 180, false, false, (AnimatedSprite.endOfAnimationBehavior) (x => l.playSound("dustMeep"))),
            new FarmerSprite.AnimationFrame(34, 180),
            new FarmerSprite.AnimationFrame(33, 180, false, false, (AnimatedSprite.endOfAnimationBehavior) (x => l.playSound("dustMeep"))),
            new FarmerSprite.AnimationFrame(34, 180),
            new FarmerSprite.AnimationFrame(33, 180, false, false, (AnimatedSprite.endOfAnimationBehavior) (x => l.playSound("dustMeep"))),
            new FarmerSprite.AnimationFrame(34, 180)
          });
          this.Sprite.loop = false;
        }
        return true;
      }
      if (this.setTemporaryMessages(who) || !(bool) (NetFieldBase<bool, NetBool>) this.doingEndOfRouteAnimation && (bool) (NetFieldBase<bool, NetBool>) this.goingToDoEndOfRouteAnimation || this.endOfRouteMessage.Value == null)
        return false;
      Game1.drawDialogue(this);
      return true;
    }

    public void grantConversationFriendship(Farmer who, int amount = 20)
    {
      if (this.Name.Contains("King") || who.hasPlayerTalkedToNPC(this.Name) || !who.friendshipData.ContainsKey(this.Name))
        return;
      who.friendshipData[this.Name].TalkedToToday = true;
      Game1.player.checkForQuestComplete(this, -1, -1, (Item) null, (string) null, 5);
      if (this.isDivorcedFrom(who))
        return;
      who.changeFriendship(amount, this);
    }

    public virtual void AskLeoMemoryPrompt()
    {
      GameLocation currentLocation = this.currentLocation;
      Response[] answerChoices = new Response[2]
      {
        new Response("Yes", Game1.content.LoadString("Strings\\Characters:Leo_Memory_Answer_Yes")),
        new Response("No", Game1.content.LoadString("Strings\\Characters:Leo_Memory_Answer_No"))
      };
      string question = Game1.content.LoadStringReturnNullIfNotFound("Strings\\Characters:Leo_Memory_" + this.GetUnseenLeoEvent().Value.Value.ToString()) ?? "";
      currentLocation.createQuestionDialogue(question, answerChoices, new GameLocation.afterQuestionBehavior(this.OnLeoMemoryResponse), this);
    }

    public bool CanRevisitLeoMemory(KeyValuePair<string, int>? event_data)
    {
      if (!event_data.HasValue)
        return false;
      string key1 = event_data.Value.Key;
      int num = event_data.Value.Value;
      Dictionary<string, string> dictionary;
      try
      {
        dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + key1);
      }
      catch (Exception ex)
      {
        return false;
      }
      if (dictionary == null)
        return false;
      foreach (string key2 in dictionary.Keys)
      {
        if (key2.Split('/')[0] == num.ToString())
        {
          GameLocation locationFromName = Game1.getLocationFromName(key1);
          string precondition = key2.Replace("/e 1039573", "").Replace("/Hl leoMoved", "");
          if (locationFromName != null && locationFromName.checkEventPrecondition(precondition) != -1)
            return true;
        }
      }
      return false;
    }

    public KeyValuePair<string, int>? GetUnseenLeoEvent()
    {
      List<int> intList = new List<int>();
      if (!Game1.player.eventsSeen.Contains(6497423))
        return new KeyValuePair<string, int>?(new KeyValuePair<string, int>("IslandWest", 6497423));
      if (!Game1.player.eventsSeen.Contains(6497421))
        return new KeyValuePair<string, int>?(new KeyValuePair<string, int>("IslandNorth", 6497421));
      return !Game1.player.eventsSeen.Contains(6497428) ? new KeyValuePair<string, int>?(new KeyValuePair<string, int>("IslandSouth", 6497428)) : new KeyValuePair<string, int>?();
    }

    public void OnLeoMemoryResponse(Farmer who, string whichAnswer)
    {
      if (whichAnswer.ToLower() == "yes")
      {
        KeyValuePair<string, int>? unseenLeoEvent = this.GetUnseenLeoEvent();
        if (!unseenLeoEvent.HasValue)
          return;
        string key1 = unseenLeoEvent.Value.Key;
        int event_id = unseenLeoEvent.Value.Value;
        Dictionary<string, string> location_events = (Dictionary<string, string>) null;
        try
        {
          location_events = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + key1);
        }
        catch (Exception ex)
        {
          return;
        }
        if (location_events == null)
          return;
        int old_x = Game1.player.getTileX();
        int old_y = Game1.player.getTileY();
        string old_location = Game1.player.currentLocation.NameOrUniqueName;
        int old_direction = Game1.player.FacingDirection;
        foreach (string key2 in location_events.Keys)
        {
          string key = key2;
          if (key.Split('/')[0] == event_id.ToString())
          {
            LocationRequest location_request = Game1.getLocationRequest(key1);
            Game1.warpingForForcedRemoteEvent = true;
            location_request.OnWarp += (LocationRequest.Callback) (() =>
            {
              Event evt = new Event(location_events[key], event_id);
              evt.isMemory = true;
              evt.setExitLocation(old_location, old_x, old_y);
              Game1.player.orientationBeforeEvent = old_direction;
              location_request.Location.currentEvent = evt;
              location_request.Location.startEvent(evt);
              Game1.warpingForForcedRemoteEvent = false;
            });
            int x = 8;
            int y = 8;
            Utility.getDefaultWarpLocation(location_request.Name, ref x, ref y);
            Game1.warpFarmer(location_request, x, y, Game1.player.FacingDirection);
          }
        }
      }
      else
        Game1.game1.wasAskedLeoMemory = true;
    }

    public bool isDivorcedFrom(Farmer who) => who != null && who.friendshipData.ContainsKey(this.Name) && who.friendshipData[this.Name].IsDivorced();

    public override void MovePosition(
      GameTime time,
      xTile.Dimensions.Rectangle viewport,
      GameLocation currentLocation)
    {
      if (this.movementPause > 0)
        return;
      this.faceTowardFarmerTimer = 0;
      base.MovePosition(time, viewport, currentLocation);
    }

    public GameLocation getHome() => this.isMarried() && this.getSpouse() != null ? (GameLocation) Utility.getHomeOfFarmer(this.getSpouse()) : Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) this.defaultMap);

    public override bool canPassThroughActionTiles() => true;

    public virtual void behaviorOnFarmerPushing()
    {
    }

    public virtual void behaviorOnFarmerLocationEntry(GameLocation location, Farmer who)
    {
      if (this.Sprite == null || this.Sprite.CurrentAnimation != null || this.Sprite.SourceRect.Height <= 32)
        return;
      this.Sprite.SpriteWidth = 16;
      this.Sprite.SpriteHeight = 16;
      this.Sprite.currentFrame = 0;
    }

    public virtual void behaviorOnLocalFarmerLocationEntry(GameLocation location)
    {
      this.shouldPlayRobinHammerAnimation.CancelInterpolation();
      this.shouldPlaySpousePatioAnimation.CancelInterpolation();
      this.shouldWearIslandAttire.CancelInterpolation();
      this.isSleeping.CancelInterpolation();
      this.doingEndOfRouteAnimation.CancelInterpolation();
      this._skipRouteEndIntro = this.doingEndOfRouteAnimation.Value;
      this.endOfRouteBehaviorName.CancelInterpolation();
      if (!this.isSleeping.Value)
        return;
      this.drawOffset.CancelInterpolation();
      this.position.Field.CancelInterpolation();
    }

    public override void updateMovement(GameLocation location, GameTime time)
    {
      this.lastPosition = this.Position;
      if (this.DirectionsToNewLocation != null && !Game1.newDay)
      {
        Point standingXy1 = this.getStandingXY();
        if (standingXy1.X < -64 || standingXy1.X > location.map.DisplayWidth + 64 || standingXy1.Y < -64 || standingXy1.Y > location.map.DisplayHeight + 64)
        {
          this.IsWalkingInSquare = false;
          Game1.warpCharacter(this, this.DefaultMap, this.DefaultPosition);
          location.characters.Remove(this);
        }
        else if (this.IsWalkingInSquare)
        {
          this.returnToEndPoint();
          this.MovePosition(time, Game1.viewport, location);
        }
        else
        {
          if (!this.followSchedule)
            return;
          this.MovePosition(time, Game1.viewport, location);
          Warp warp = location.isCollidingWithWarp(this.GetBoundingBox(), (Character) this);
          PropertyValue propertyValue = (PropertyValue) null;
          location.map.GetLayer("Buildings").PickTile(this.nextPositionPoint(), Game1.viewport.Size)?.Properties.TryGetValue("Action", out propertyValue);
          string[] strArray = propertyValue == null ? (string[]) null : propertyValue.ToString().Split(Utility.CharSpace);
          if (strArray == null)
          {
            Point standingXy2 = this.getStandingXY();
            location.map.GetLayer("Buildings").PickTile(new Location(standingXy2.X, standingXy2.Y), Game1.viewport.Size)?.Properties.TryGetValue("Action", out propertyValue);
            strArray = propertyValue == null ? (string[]) null : propertyValue.ToString().Split(Utility.CharSpace);
          }
          if (warp != null)
          {
            switch (location)
            {
              case BusStop _ when warp.TargetName.Equals("Farm"):
                Point entryLocation = ((this.isMarried() ? (GameLocation) (this.getHome() as FarmHouse) : Game1.getLocationFromName(this.getSpouse().homeLocation.Value)) as FarmHouse).getEntryLocation();
                warp = new Warp(warp.X, warp.Y, this.getSpouse().homeLocation.Value, entryLocation.X, entryLocation.Y, false);
                break;
              case FarmHouse _ when warp.TargetName.Equals("Farm"):
                warp = new Warp(warp.X, warp.Y, "BusStop", 0, 23, false);
                break;
            }
            Game1.warpCharacter(this, warp.TargetName, new Vector2((float) (warp.TargetX * 64), (float) (warp.TargetY * 64 - this.Sprite.getHeight() / 2 - 16)));
            location.characters.Remove(this);
          }
          else if (strArray != null && strArray.Length >= 1 && strArray[0].Contains("Warp"))
          {
            Game1.warpCharacter(this, strArray[3], new Vector2((float) Convert.ToInt32(strArray[1]), (float) Convert.ToInt32(strArray[2])));
            if (Game1.currentLocation.name.Equals(location.name) && Utility.isOnScreen(this.getStandingPosition(), 192) && !Game1.eventUp)
              location.playSound("doorClose", NetAudio.SoundContext.NPC);
            location.characters.Remove(this);
          }
          else if (strArray != null && strArray.Length >= 1 && strArray[0].Contains("Door"))
          {
            location.openDoor(new Location(this.nextPositionPoint().X / 64, this.nextPositionPoint().Y / 64), Game1.player.currentLocation.Equals(location));
          }
          else
          {
            if (location.map.GetLayer("Paths") == null)
              return;
            Point standingXy3 = this.getStandingXY();
            Tile tile = location.map.GetLayer("Paths").PickTile(new Location(standingXy3.X, standingXy3.Y), Game1.viewport.Size);
            Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
            boundingBox.Inflate(2, 2);
            if (tile == null || !new Microsoft.Xna.Framework.Rectangle(standingXy3.X - standingXy3.X % 64, standingXy3.Y - standingXy3.Y % 64, 64, 64).Contains(boundingBox))
              return;
            switch (tile.TileIndex)
            {
              case 0:
                if (this.getDirection() == 3)
                {
                  this.SetMovingOnlyUp();
                  break;
                }
                if (this.getDirection() != 2)
                  break;
                this.SetMovingOnlyRight();
                break;
              case 1:
                if (this.getDirection() == 3)
                {
                  this.SetMovingOnlyDown();
                  break;
                }
                if (this.getDirection() != 0)
                  break;
                this.SetMovingOnlyRight();
                break;
              case 2:
                if (this.getDirection() == 1)
                {
                  this.SetMovingOnlyDown();
                  break;
                }
                if (this.getDirection() != 0)
                  break;
                this.SetMovingOnlyLeft();
                break;
              case 3:
                if (this.getDirection() == 1)
                {
                  this.SetMovingOnlyUp();
                  break;
                }
                if (this.getDirection() != 2)
                  break;
                this.SetMovingOnlyLeft();
                break;
              case 4:
                this.changeSchedulePathDirection();
                this.moveCharacterOnSchedulePath();
                break;
              case 7:
                this.ReachedEndPoint();
                break;
            }
          }
        }
      }
      else
      {
        if (!this.IsWalkingInSquare)
          return;
        this.randomSquareMovement(time);
        this.MovePosition(time, Game1.viewport, location);
      }
    }

    public void facePlayer(Farmer who)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.facingDirectionBeforeSpeakingToPlayer == -1)
        this.facingDirectionBeforeSpeakingToPlayer.Value = this.getFacingDirection();
      this.faceDirection((who.FacingDirection + 2) % 4);
    }

    public void doneFacingPlayer(Farmer who)
    {
    }

    public virtual void UpdateFarmExploration(GameTime time, GameLocation location)
    {
      if (this._farmActivities == null)
        this.InitializeFarmActivities();
      if (this._currentFarmActivity != null)
      {
        if (!this._currentFarmActivity.IsPerformingActivity() || !this._currentFarmActivity.Update(time))
          return;
        this._currentFarmActivity.EndActivity();
        this._currentFarmActivity = (FarmActivity) null;
      }
      else
      {
        this.nextFarmActivityScan -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this.nextFarmActivityScan > 0.0)
          return;
        bool flag = false;
        if (this.FindFarmActivity())
          flag = true;
        if (flag)
          return;
        this.nextFarmActivityScan = 3f;
      }
    }

    public virtual void InitializeFarmActivities()
    {
      this._farmActivities = new List<FarmActivity>();
      this._farmActivities.Add(new CropWatchActivity().Initialize(this));
      this._farmActivities.Add(new FlowerWatchActivity().Initialize(this));
      this._farmActivities.Add(new ArtifactSpotWatchActivity().Initialize(this, 0.5f));
      this._farmActivities.Add(new TreeActivity().Initialize(this));
      this._farmActivities.Add(new ClearingActivity().Initialize(this));
      this._farmActivities.Add(new ShrineActivity().Initialize(this, 0.1f));
      this._farmActivities.Add(new MailActivity().Initialize(this, 0.1f));
      this._farmActivityWeightTotal = 0.0f;
      foreach (FarmActivity farmActivity in this._farmActivities)
        this._farmActivityWeightTotal += farmActivity.weight;
    }

    public virtual bool FindFarmActivity()
    {
      if (!(this.currentLocation is Farm))
        return false;
      Farm currentLocation = this.currentLocation as Farm;
      float num = Utility.RandomFloat(0.0f, this._farmActivityWeightTotal);
      FarmActivity farmActivity1 = (FarmActivity) null;
      foreach (FarmActivity farmActivity2 in this._farmActivities)
      {
        num -= farmActivity2.weight;
        if ((double) num <= 0.0)
        {
          if (farmActivity2.AttemptActivity(currentLocation))
          {
            farmActivity1 = farmActivity2;
            break;
          }
          break;
        }
      }
      if (farmActivity1 == null || farmActivity1.IsTileBlockedFromSight(farmActivity1.activityPosition) || !this.PathToOnFarm(Utility.Vector2ToPoint(farmActivity1.activityPosition), new PathFindController.endBehavior(this.OnFinishPathForActivity)))
        return false;
      this._currentFarmActivity = farmActivity1;
      return true;
    }

    public override void update(GameTime time, GameLocation location)
    {
      TimeSpan timeSpan;
      if (Game1.IsMasterGame && (double) this.currentScheduleDelay > 0.0)
      {
        double currentScheduleDelay = (double) this.currentScheduleDelay;
        timeSpan = time.ElapsedGameTime;
        double totalSeconds = timeSpan.TotalSeconds;
        this.currentScheduleDelay = (float) (currentScheduleDelay - totalSeconds);
        if ((double) this.currentScheduleDelay <= 0.0)
        {
          this.currentScheduleDelay = -1f;
          this.checkSchedule(Game1.timeOfDay);
          this.currentScheduleDelay = 0.0f;
        }
      }
      this.removeHenchmanEvent.Poll();
      if (Game1.IsMasterGame && (bool) (NetFieldBase<bool, NetBool>) this.exploreFarm)
        this.UpdateFarmExploration(time, location);
      if (Game1.IsMasterGame && this.shouldWearIslandAttire.Value && (this.currentLocation == null || this.currentLocation.GetLocationContext() == GameLocation.LocationContext.Default))
        this.shouldWearIslandAttire.Value = false;
      if (this._startedEndOfRouteBehavior == null && this._finishingEndOfRouteBehavior == null && this.loadedEndOfRouteBehavior != this.endOfRouteBehaviorName.Value)
        this.loadEndOfRouteBehavior((string) (NetFieldBase<string, NetString>) this.endOfRouteBehaviorName);
      if (!this.currentlyDoingEndOfRouteAnimation && string.Equals(this.loadedEndOfRouteBehavior, this.endOfRouteBehaviorName.Value, StringComparison.Ordinal) && (bool) (NetFieldBase<bool, NetBool>) this.doingEndOfRouteAnimation)
        this.reallyDoAnimationAtEndOfScheduleRoute();
      else if (this.currentlyDoingEndOfRouteAnimation && !(bool) (NetFieldBase<bool, NetBool>) this.doingEndOfRouteAnimation)
        this.finishEndOfRouteAnimation();
      this.currentlyDoingEndOfRouteAnimation = (bool) (NetFieldBase<bool, NetBool>) this.doingEndOfRouteAnimation;
      if (this.shouldWearIslandAttire.Value && !this.isWearingIslandAttire)
        this.wearIslandAttire();
      else if (!this.shouldWearIslandAttire.Value && this.isWearingIslandAttire)
        this.wearNormalClothes();
      bool flag1 = this.isSleeping.Value;
      if (flag1 && !this.isPlayingSleepingAnimation)
        this.playSleepingAnimation();
      else if (!flag1 && this.isPlayingSleepingAnimation)
      {
        this.Sprite.StopAnimation();
        this.isPlayingSleepingAnimation = false;
      }
      bool flag2 = this.shouldPlayRobinHammerAnimation.Value;
      if (flag2 && !this.isPlayingRobinHammerAnimation)
      {
        this.doPlayRobinHammerAnimation();
        this.isPlayingRobinHammerAnimation = true;
      }
      else if (!flag2 && this.isPlayingRobinHammerAnimation)
      {
        this.Sprite.StopAnimation();
        this.isPlayingRobinHammerAnimation = false;
      }
      bool flag3 = this.shouldPlaySpousePatioAnimation.Value;
      if (flag3 && !this.isPlayingSpousePatioAnimation)
      {
        this.doPlaySpousePatioAnimation();
        this.isPlayingSpousePatioAnimation = true;
      }
      else if (!flag3 && this.isPlayingSpousePatioAnimation)
      {
        this.Sprite.StopAnimation();
        this.isPlayingSpousePatioAnimation = false;
      }
      if (this.returningToEndPoint)
      {
        this.returnToEndPoint();
        this.MovePosition(time, Game1.viewport, location);
      }
      else if (this.temporaryController != null)
      {
        if (this.temporaryController.update(time))
        {
          int num = this.temporaryController.NPCSchedule ? 1 : 0;
          this.temporaryController = (PathFindController) null;
          if (num != 0)
          {
            this.currentScheduleDelay = -1f;
            this.checkSchedule(Game1.timeOfDay);
            this.currentScheduleDelay = 0.0f;
          }
        }
        this.updateEmote(time);
      }
      else
        base.update(time, location);
      if (this.textAboveHeadTimer > 0)
      {
        if (this.textAboveHeadPreTimer > 0)
        {
          int aboveHeadPreTimer = this.textAboveHeadPreTimer;
          timeSpan = time.ElapsedGameTime;
          int milliseconds = timeSpan.Milliseconds;
          this.textAboveHeadPreTimer = aboveHeadPreTimer - milliseconds;
        }
        else
        {
          int textAboveHeadTimer = this.textAboveHeadTimer;
          timeSpan = time.ElapsedGameTime;
          int milliseconds = timeSpan.Milliseconds;
          this.textAboveHeadTimer = textAboveHeadTimer - milliseconds;
          this.textAboveHeadAlpha = this.textAboveHeadTimer <= 500 ? Math.Max(0.0f, this.textAboveHeadAlpha - 0.04f) : Math.Min(1f, this.textAboveHeadAlpha + 0.1f);
        }
      }
      if (this.isWalkingInSquare && !this.returningToEndPoint)
        this.randomSquareMovement(time);
      if (this.Sprite != null && this.Sprite.CurrentAnimation != null && !Game1.eventUp && Game1.IsMasterGame && this.Sprite.animateOnce(time))
        this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
      if (this.movementPause > 0 && (!Game1.dialogueUp || this.controller != null))
      {
        this.freezeMotion = true;
        int movementPause = this.movementPause;
        timeSpan = time.ElapsedGameTime;
        int milliseconds = timeSpan.Milliseconds;
        this.movementPause = movementPause - milliseconds;
        if (this.movementPause <= 0)
          this.freezeMotion = false;
      }
      if (this.shakeTimer > 0)
      {
        int shakeTimer = this.shakeTimer;
        timeSpan = time.ElapsedGameTime;
        int milliseconds = timeSpan.Milliseconds;
        this.shakeTimer = shakeTimer - milliseconds;
      }
      if (this.lastPosition.Equals(this.Position))
      {
        double sinceLastMovement = (double) this.timerSinceLastMovement;
        timeSpan = time.ElapsedGameTime;
        double milliseconds = (double) timeSpan.Milliseconds;
        this.timerSinceLastMovement = (float) (sinceLastMovement + milliseconds);
      }
      else
        this.timerSinceLastMovement = 0.0f;
      if ((bool) (NetFieldBase<bool, NetBool>) this.swimming)
      {
        timeSpan = time.TotalGameTime;
        this.yOffset = (float) (Math.Cos(timeSpan.TotalMilliseconds / 2000.0) * 4.0);
        float swimTimer1 = this.swimTimer;
        double swimTimer2 = (double) this.swimTimer;
        timeSpan = time.ElapsedGameTime;
        double milliseconds = (double) timeSpan.Milliseconds;
        this.swimTimer = (float) (swimTimer2 - milliseconds);
        if ((double) this.timerSinceLastMovement == 0.0)
        {
          if ((double) swimTimer1 > 400.0 && (double) this.swimTimer <= 400.0 && location.Equals(Game1.currentLocation))
          {
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2(this.Position.X, (float) (this.getStandingY() - 32)), false, Game1.random.NextDouble() < 0.5, 0.01f, 0.01f, Microsoft.Xna.Framework.Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
            location.playSound("slosh", NetAudio.SoundContext.NPC);
          }
          if ((double) this.swimTimer < 0.0)
          {
            this.swimTimer = 800f;
            if (location.Equals(Game1.currentLocation))
            {
              location.playSound("slosh", NetAudio.SoundContext.NPC);
              Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2(this.Position.X, (float) (this.getStandingY() - 32)), false, Game1.random.NextDouble() < 0.5, 0.01f, 0.01f, Microsoft.Xna.Framework.Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
            }
          }
        }
        else if ((double) this.swimTimer < 0.0)
          this.swimTimer = 100f;
      }
      if (!Game1.IsMasterGame)
        return;
      this.isMovingOnPathFindPath.Value = this.controller != null && this.temporaryController != null;
    }

    public virtual void wearIslandAttire()
    {
      try
      {
        this.Sprite.LoadTexture("Characters\\" + NPC.getTextureNameForCharacter(this.name.Value) + "_Beach");
      }
      catch (ContentLoadException ex)
      {
        this.Sprite.LoadTexture("Characters\\" + NPC.getTextureNameForCharacter(this.name.Value));
      }
      this.isWearingIslandAttire = true;
      this.resetPortrait();
    }

    public virtual void wearNormalClothes()
    {
      this.Sprite.LoadTexture("Characters\\" + NPC.getTextureNameForCharacter(this.name.Value));
      this.isWearingIslandAttire = false;
      this.resetPortrait();
    }

    public virtual void performTenMinuteUpdate(int timeOfDay, GameLocation l)
    {
      if (Game1.eventUp)
        return;
      if (Game1.random.NextDouble() < 0.1 && this.Dialogue != null && this.Dialogue.ContainsKey((string) (NetFieldBase<string, NetString>) l.name + "_Ambient"))
      {
        string[] strArray = this.Dialogue[(string) (NetFieldBase<string, NetString>) l.name + "_Ambient"].Split('/');
        int preTimer = Game1.random.Next(4) * 1000;
        this.showTextAboveHead(strArray[Game1.random.Next(strArray.Length)], preTimer: preTimer);
      }
      else
      {
        if (!this.isMoving() || !(bool) (NetFieldBase<bool, NetBool>) l.isOutdoors || timeOfDay >= 1800 || Game1.random.NextDouble() >= 0.3 + (this.SocialAnxiety == 0 ? 0.25 : (this.SocialAnxiety == 1 ? (this.Manners == 2 ? -1.0 : -0.2) : 0.0)) || this.Age == 1 && (this.Manners != 1 || this.SocialAnxiety != 0) || this.isMarried())
          return;
        Character c = Utility.isThereAFarmerOrCharacterWithinDistance(this.getTileLocation(), 4, l);
        if (c.Name.Equals(this.Name) || c is Horse)
          return;
        Dictionary<string, string> dictionary = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\NPCDispositions");
        if (!dictionary.ContainsKey(this.Name) || ((IEnumerable<string>) dictionary[this.Name].Split('/')[9].Split(' ')).Contains<string>(c.Name) || !this.isFacingToward(c.getTileLocation()))
          return;
        this.sayHiTo(c);
      }
    }

    public void sayHiTo(Character c)
    {
      if (this.getHi(c.displayName) == null)
        return;
      this.showTextAboveHead(this.getHi(c.displayName));
      if (!(c is NPC) || Game1.random.NextDouble() >= 0.66 || (c as NPC).getHi(this.displayName) == null)
        return;
      (c as NPC).showTextAboveHead((c as NPC).getHi(this.displayName), preTimer: (1000 + Game1.random.Next(500)));
    }

    public string getHi(string nameToGreet)
    {
      if (this.Age == 2)
        return this.SocialAnxiety != 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4059") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4058");
      if (this.SocialAnxiety == 1)
        return Game1.random.NextDouble() >= 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4061") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4060");
      if (this.SocialAnxiety == 0)
      {
        if (Game1.random.NextDouble() < 0.33)
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4062");
        return Game1.random.NextDouble() >= 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4068", (object) nameToGreet) : (Game1.timeOfDay < 1200 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4063") : (Game1.timeOfDay < 1700 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4064") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4065"))) + ", " + Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4066", (object) nameToGreet);
      }
      if (Game1.random.NextDouble() < 0.33)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4060");
      return Game1.random.NextDouble() >= 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4072") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4071", (object) nameToGreet);
    }

    public bool isFacingToward(Vector2 tileLocation)
    {
      switch (this.FacingDirection)
      {
        case 0:
          return (double) this.getTileY() > (double) tileLocation.Y;
        case 1:
          return (double) this.getTileX() < (double) tileLocation.X;
        case 2:
          return (double) this.getTileY() < (double) tileLocation.Y;
        case 3:
          return (double) this.getTileX() > (double) tileLocation.X;
        default:
          return false;
      }
    }

    public virtual void arriveAt(GameLocation l)
    {
      if (Game1.eventUp || Game1.random.NextDouble() >= 0.5 || this.Dialogue == null || !this.Dialogue.ContainsKey((string) (NetFieldBase<string, NetString>) l.name + "_Entry"))
        return;
      string[] strArray = this.Dialogue[(string) (NetFieldBase<string, NetString>) l.name + "_Entry"].Split('/');
      this.showTextAboveHead(strArray[Game1.random.Next(strArray.Length)]);
    }

    public override void Halt()
    {
      base.Halt();
      this.shouldPlaySpousePatioAnimation.Value = false;
      this.isPlayingSleepingAnimation = false;
      this.isCharging = false;
      this.speed = 2;
      this.addedSpeed = 0;
      if (!this.isSleeping.Value)
        return;
      this.playSleepingAnimation();
      this.Sprite.UpdateSourceRect();
    }

    public void addExtraDialogues(string dialogues)
    {
      if (this.updatedDialogueYet)
      {
        if (dialogues == null)
          return;
        this.CurrentDialogue.Push(new StardewValley.Dialogue(dialogues, this));
      }
      else
        this.extraDialogueMessageToAddThisMorning = dialogues;
    }

    public void PerformDivorce()
    {
      this.reloadDefaultLocation();
      Game1.warpCharacter(this, (string) (NetFieldBase<string, NetString>) this.defaultMap, this.DefaultPosition / 64f);
    }

    public string tryToGetMarriageSpecificDialogueElseReturnDefault(
      string dialogueKey,
      string defaultMessage = "")
    {
      Dictionary<string, string> dictionary1 = (Dictionary<string, string>) null;
      bool flag = false;
      if (this.isRoommate())
      {
        try
        {
          if (Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\MarriageDialogue" + this.GetDialogueSheetName() + "Roommate") != null)
          {
            flag = true;
            dictionary1 = Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\MarriageDialogue" + this.GetDialogueSheetName() + "Roommate");
            if (dictionary1 != null)
            {
              if (dictionary1.ContainsKey(dialogueKey))
                return dictionary1[dialogueKey];
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
      if (!flag)
      {
        try
        {
          dictionary1 = Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\MarriageDialogue" + this.GetDialogueSheetName());
        }
        catch (Exception ex)
        {
        }
      }
      if (dictionary1 != null && dictionary1.ContainsKey(dialogueKey))
        return dictionary1[dialogueKey];
      Dictionary<string, string> dictionary2 = Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\MarriageDialogue");
      if (this.isRoommate() && dictionary2 != null && dictionary2.ContainsKey(dialogueKey + "Roommate"))
        return dictionary2[dialogueKey + "Roommate"];
      return dictionary2 != null && dictionary2.ContainsKey(dialogueKey) ? dictionary2[dialogueKey] : defaultMessage;
    }

    public void resetCurrentDialogue()
    {
      this.CurrentDialogue = (Stack<StardewValley.Dialogue>) null;
      this.shouldSayMarriageDialogue.Value = false;
      this.currentMarriageDialogue.Clear();
    }

    private Stack<StardewValley.Dialogue> loadCurrentDialogue()
    {
      this.updatedDialogueYet = true;
      Friendship friendship;
      int heartLevel = Game1.player.friendshipData.TryGetValue(this.Name, out friendship) ? friendship.Points / 250 : 0;
      Stack<StardewValley.Dialogue> dialogueStack = new Stack<StardewValley.Dialogue>();
      Random random = new Random((int) Game1.stats.DaysPlayed * 77 + (int) Game1.uniqueIDForThisGame / 2 + 2 + (int) this.defaultPosition.X * 77 + (int) this.defaultPosition.Y * 777);
      if (random.NextDouble() < 0.025 && heartLevel >= 1)
      {
        Dictionary<string, string> dictionary1 = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
        string str1;
        if (dictionary1.TryGetValue(this.Name, out str1))
        {
          string[] strArray1 = str1.Split('/')[9].Split(' ');
          if (strArray1.Length > 1)
          {
            int index = random.Next(strArray1.Length / 2) * 2;
            string str2 = strArray1[index];
            string sub1_1 = str2;
            if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en && Game1.getCharacterFromName(str2) != null)
              sub1_1 = Game1.getCharacterFromName(str2).displayName;
            string str3 = strArray1[index + 1].Replace("'", "").Replace("_", " ");
            string str4;
            bool flag = dictionary1.TryGetValue(str2, out str4) && str4.Split('/')[4].Equals("male");
            Dictionary<string, string> dictionary2 = Game1.content.Load<Dictionary<string, string>>("Data\\NPCGiftTastes");
            if (dictionary2.ContainsKey(str2))
            {
              string str5 = (string) null;
              string str6;
              if (str3.Length <= 2 || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja)
                str6 = sub1_1;
              else if (!flag)
                str6 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4080", (object) str3);
              else
                str6 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4079", (object) str3);
              string sub1_2 = str6;
              string masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4083", (object) sub1_2);
              int int32;
              if (random.NextDouble() < 0.5)
              {
                string[] strArray2 = dictionary2[str2].Split('/')[1].Split(' ');
                int32 = Convert.ToInt32(strArray2[random.Next(strArray2.Length)]);
                if (this.Name == "Penny" && str2 == "Pam")
                {
                  while (int32 == 303 || int32 == 346 || int32 == 348 || int32 == 459)
                    int32 = Convert.ToInt32(strArray2[random.Next(strArray2.Length)]);
                }
                string str7;
                if (Game1.objectInformation.TryGetValue(int32, out str7))
                {
                  str5 = str7.Split('/')[4];
                  masterDialogue += Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4084", (object) str5);
                  if (this.Age == 2)
                  {
                    masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4086", (object) sub1_1, (object) str5) + (flag ? Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4088") : Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4089"));
                  }
                  else
                  {
                    switch (random.Next(5))
                    {
                      case 0:
                        masterDialogue = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4091", (object) sub1_2, (object) str5);
                        break;
                      case 1:
                        masterDialogue = flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4094", (object) sub1_2, (object) str5) : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4097", (object) sub1_2, (object) str5);
                        break;
                      case 2:
                        masterDialogue = flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4100", (object) sub1_2, (object) str5) : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4103", (object) sub1_2, (object) str5);
                        break;
                      case 3:
                        masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4106", (object) sub1_2, (object) str5);
                        break;
                    }
                    if (random.NextDouble() < 0.65)
                    {
                      switch (random.Next(5))
                      {
                        case 0:
                          masterDialogue += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4109") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4111");
                          break;
                        case 1:
                          masterDialogue += flag ? (random.NextDouble() < 0.5 ? Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4113") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4114")) : (random.NextDouble() < 0.5 ? Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4115") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4116"));
                          break;
                        case 2:
                          masterDialogue += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4118") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4120");
                          break;
                        case 3:
                          masterDialogue += Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4125");
                          break;
                        case 4:
                          masterDialogue += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4126") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4128");
                          break;
                      }
                      if (str2.Equals("Abigail") && random.NextDouble() < 0.5)
                        masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4128", (object) sub1_1, (object) str5);
                    }
                  }
                }
              }
              else
              {
                try
                {
                  int32 = Convert.ToInt32(dictionary2[str2].Split('/')[7].Split(' ')[random.Next(dictionary2[str2].Split('/')[7].Split(' ').Length)]);
                }
                catch (Exception ex)
                {
                  int32 = Convert.ToInt32(dictionary2["Universal_Hate"].Split(' ')[random.Next(dictionary2["Universal_Hate"].Split(' ').Length)]);
                }
                if (Game1.objectInformation.ContainsKey(int32))
                {
                  str5 = Game1.objectInformation[int32].Split('/')[4];
                  masterDialogue += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4135", (object) str5, (object) Lexicon.getRandomNegativeFoodAdjective()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4138", (object) str5, (object) Lexicon.getRandomNegativeFoodAdjective());
                  if (this.Age == 2)
                  {
                    string str8;
                    if (!flag)
                      str8 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4144", (object) sub1_1, (object) str5);
                    else
                      str8 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4141", (object) sub1_1, (object) str5);
                    masterDialogue = str8;
                  }
                  else
                  {
                    switch (random.Next(4))
                    {
                      case 0:
                        masterDialogue = (random.NextDouble() < 0.5 ? Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4146") : "") + Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4147", (object) sub1_2, (object) str5);
                        break;
                      case 1:
                        string str9;
                        if (!flag)
                        {
                          if (random.NextDouble() >= 0.5)
                            str9 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4154", (object) sub1_2, (object) str5);
                          else
                            str9 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4153", (object) sub1_2, (object) str5);
                        }
                        else if (random.NextDouble() >= 0.5)
                          str9 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4152", (object) sub1_2, (object) str5);
                        else
                          str9 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4149", (object) sub1_2, (object) str5);
                        masterDialogue = str9;
                        break;
                      case 2:
                        masterDialogue = flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4161", (object) sub1_2, (object) str5) : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4164", (object) sub1_2, (object) str5);
                        break;
                    }
                    if (random.NextDouble() < 0.65)
                    {
                      switch (random.Next(5))
                      {
                        case 0:
                          masterDialogue += Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4170");
                          break;
                        case 1:
                          masterDialogue += Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4171");
                          break;
                        case 2:
                          masterDialogue += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4172") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4174");
                          break;
                        case 3:
                          masterDialogue += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4176") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4178");
                          break;
                        case 4:
                          masterDialogue += Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4180");
                          break;
                      }
                      if (this.Name.Equals("Lewis") && random.NextDouble() < 0.5)
                        masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4182", (object) sub1_1, (object) str5);
                    }
                  }
                }
              }
              if (str5 != null)
              {
                if (Game1.getCharacterFromName(str2) != null)
                  masterDialogue = masterDialogue + "%revealtaste" + str2 + int32.ToString();
                dialogueStack.Clear();
                if (masterDialogue.Length > 0)
                {
                  try
                  {
                    masterDialogue = masterDialogue.Substring(0, 1).ToUpper() + masterDialogue.Substring(1, masterDialogue.Length - 1);
                  }
                  catch (Exception ex)
                  {
                  }
                }
                dialogueStack.Push(new StardewValley.Dialogue(masterDialogue, this));
                return dialogueStack;
              }
            }
          }
        }
      }
      if (this.Dialogue != null && this.Dialogue.Count != 0)
      {
        string masterDialogue = "";
        dialogueStack.Clear();
        if (Game1.player.spouse != null && Game1.player.spouse == this.Name)
        {
          if (Game1.player.spouse.Equals(this.Name) && Game1.player.isEngaged())
          {
            if (Game1.player.hasCurrentOrPendingRoommate() && Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue").ContainsKey(this.Name + "Roommate0"))
              dialogueStack.Push(new StardewValley.Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")[this.Name + "Roommate" + random.Next(2).ToString()], this));
            else if (Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue").ContainsKey(this.Name + "0"))
              dialogueStack.Push(new StardewValley.Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")[this.Name + random.Next(2).ToString()], this));
          }
          else if (!Game1.newDay && this.marriageDefaultDialogue.Value != null && !this.shouldSayMarriageDialogue.Value)
          {
            dialogueStack.Push(this.marriageDefaultDialogue.Value.GetDialogue(this));
            this.marriageDefaultDialogue.Value = (MarriageDialogueReference) null;
          }
        }
        else if (this.idForClones == -1)
        {
          if (Game1.player.friendshipData.ContainsKey(this.Name))
          {
            if (Game1.player.friendshipData[this.Name].IsDivorced())
            {
              try
              {
                dialogueStack.Push(new StardewValley.Dialogue(Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\" + this.GetDialogueSheetName())["divorced"], this));
                return dialogueStack;
              }
              catch (Exception ex)
              {
              }
            }
          }
          if (Game1.isRaining && random.NextDouble() < 0.5 && (this.currentLocation == null || this.currentLocation.GetLocationContext() == GameLocation.LocationContext.Default) && (!this.Name.Equals("Krobus") || !(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) == "Fri")) && (!this.Name.Equals("Penny") || !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade")))
          {
            try
            {
              dialogueStack.Push(new StardewValley.Dialogue(Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\rainy")[this.GetDialogueSheetName()], this));
              return dialogueStack;
            }
            catch (Exception ex)
            {
            }
          }
          StardewValley.Dialogue dialogue = this.tryToRetrieveDialogue(Game1.currentSeason + "_", heartLevel) ?? this.tryToRetrieveDialogue("", heartLevel);
          if (dialogue != null)
            dialogueStack.Push(dialogue);
        }
        else
        {
          this.Dialogue.TryGetValue(this.idForClones.ToString() ?? "", out masterDialogue);
          dialogueStack.Push(new StardewValley.Dialogue(masterDialogue, this));
        }
      }
      else if (this.Name.Equals("Bouncer"))
        dialogueStack.Push(new StardewValley.Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4192"), this));
      if (this.extraDialogueMessageToAddThisMorning != null)
        dialogueStack.Push(new StardewValley.Dialogue(this.extraDialogueMessageToAddThisMorning, this));
      return dialogueStack;
    }

    public bool checkForNewCurrentDialogue(int heartLevel, bool noPreface = false)
    {
      foreach (string key1 in Game1.player.activeDialogueEvents.Keys)
      {
        if (this.Dialogue.ContainsKey(key1))
        {
          string key2 = key1;
          if (!key2.Equals("") && !Game1.player.mailReceived.Contains(this.Name + "_" + key2))
          {
            this.CurrentDialogue.Clear();
            this.CurrentDialogue.Push(new StardewValley.Dialogue(this.Dialogue[key2], this));
            if (!key1.Contains("dumped"))
              Game1.player.mailReceived.Add(this.Name + "_" + key2);
            return true;
          }
        }
      }
      string str = Game1.currentSeason.Equals("spring") || noPreface ? "" : Game1.currentSeason;
      Dictionary<string, string> dialogue1 = this.Dialogue;
      string[] strArray1 = new string[6]
      {
        str,
        (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name,
        "_",
        this.getTileX().ToString(),
        "_",
        null
      };
      int num = this.getTileY();
      strArray1[5] = num.ToString();
      string key3 = string.Concat(strArray1);
      if (dialogue1.ContainsKey(key3))
      {
        Stack<StardewValley.Dialogue> currentDialogue = this.CurrentDialogue;
        Dictionary<string, string> dialogue2 = this.Dialogue;
        string[] strArray2 = new string[6]
        {
          str,
          (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name,
          "_",
          null,
          null,
          null
        };
        num = this.getTileX();
        strArray2[3] = num.ToString();
        strArray2[4] = "_";
        num = this.getTileY();
        strArray2[5] = num.ToString();
        string key4 = string.Concat(strArray2);
        currentDialogue.Push(new StardewValley.Dialogue(dialogue2[key4], this)
        {
          removeOnNextMove = true
        });
        return true;
      }
      if (this.Dialogue.ContainsKey(str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
      {
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this.Dialogue[str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)], this)
        {
          removeOnNextMove = true
        });
        return true;
      }
      if (heartLevel >= 10 && this.Dialogue.ContainsKey(str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "10"))
      {
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this.Dialogue[str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "10"], this)
        {
          removeOnNextMove = true
        });
        return true;
      }
      if (heartLevel >= 8 && this.Dialogue.ContainsKey(str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "8"))
      {
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this.Dialogue[str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "8"], this)
        {
          removeOnNextMove = true
        });
        return true;
      }
      if (heartLevel >= 6 && this.Dialogue.ContainsKey(str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "6"))
      {
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this.Dialogue[str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "6"], this)
        {
          removeOnNextMove = true
        });
        return true;
      }
      if (heartLevel >= 4 && this.Dialogue.ContainsKey(str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "4"))
      {
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this.Dialogue[str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "4"], this)
        {
          removeOnNextMove = true
        });
        return true;
      }
      if (heartLevel >= 2 && this.Dialogue.ContainsKey(str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "2"))
      {
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this.Dialogue[str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name + "2"], this)
        {
          removeOnNextMove = true
        });
        return true;
      }
      if (!this.Dialogue.ContainsKey(str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name))
        return false;
      this.CurrentDialogue.Push(new StardewValley.Dialogue(this.Dialogue[str + (string) (NetFieldBase<string, NetString>) Game1.currentLocation.name], this)
      {
        removeOnNextMove = true
      });
      return true;
    }

    public StardewValley.Dialogue tryToRetrieveDialogue(
      string preface,
      int heartLevel,
      string appendToEnd = "")
    {
      int num = Game1.year;
      if (Game1.year > 2)
        num = 2;
      if (Game1.player.spouse != null && Game1.player.spouse.Length > 0 && appendToEnd.Equals(""))
      {
        if (Game1.player.hasCurrentOrPendingRoommate())
        {
          StardewValley.Dialogue retrieveDialogue = this.tryToRetrieveDialogue(preface, heartLevel, "_roommate_" + Game1.player.spouse);
          if (retrieveDialogue != null)
            return retrieveDialogue;
        }
        else
        {
          StardewValley.Dialogue retrieveDialogue = this.tryToRetrieveDialogue(preface, heartLevel, "_inlaw_" + Game1.player.spouse);
          if (retrieveDialogue != null)
            return retrieveDialogue;
        }
      }
      string str = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
      if (this.Name == "Pierre" && (Game1.isLocationAccessible("CommunityCenter") || Game1.player.HasTownKey) && str == "Wed")
        str = "Sat";
      if (this.Dialogue.ContainsKey(preface + Game1.dayOfMonth.ToString() + appendToEnd) && num == 1)
        return new StardewValley.Dialogue(this.Dialogue[preface + Game1.dayOfMonth.ToString() + appendToEnd], this);
      if (this.Dialogue.ContainsKey(preface + Game1.dayOfMonth.ToString() + "_" + num.ToString() + appendToEnd))
        return new StardewValley.Dialogue(this.Dialogue[preface + Game1.dayOfMonth.ToString() + "_" + num.ToString() + appendToEnd], this);
      if (heartLevel >= 10 && this.Dialogue.ContainsKey(preface + str + "10" + appendToEnd))
      {
        if (!this.Dialogue.ContainsKey(preface + str + "10_" + num.ToString() + appendToEnd))
          return new StardewValley.Dialogue(this.Dialogue[preface + str + "10" + appendToEnd], this);
        return new StardewValley.Dialogue(this.Dialogue[preface + str + "10_" + num.ToString() + appendToEnd], this);
      }
      if (heartLevel >= 8 && this.Dialogue.ContainsKey(preface + str + "8" + appendToEnd))
      {
        if (!this.Dialogue.ContainsKey(preface + str + "8_" + num.ToString() + appendToEnd))
          return new StardewValley.Dialogue(this.Dialogue[preface + str + "8" + appendToEnd], this);
        return new StardewValley.Dialogue(this.Dialogue[preface + str + "8_" + num.ToString() + appendToEnd], this);
      }
      if (heartLevel >= 6 && this.Dialogue.ContainsKey(preface + str + "6" + appendToEnd))
      {
        if (!this.Dialogue.ContainsKey(preface + str + "6_" + num.ToString()))
          return new StardewValley.Dialogue(this.Dialogue[preface + str + "6" + appendToEnd], this);
        return new StardewValley.Dialogue(this.Dialogue[preface + str + "6_" + num.ToString() + appendToEnd], this);
      }
      if (heartLevel >= 4 && this.Dialogue.ContainsKey(preface + str + "4" + appendToEnd))
      {
        if (preface == "fall_" && str == "Mon" && this.Name.Equals("Penny") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
          if (!this.Dialogue.ContainsKey(preface + str + "_" + num.ToString() + appendToEnd))
            return new StardewValley.Dialogue(this.Dialogue["fall_Mon"], this);
          return new StardewValley.Dialogue(this.Dialogue[preface + str + "_" + num.ToString() + appendToEnd], this);
        }
        if (!this.Dialogue.ContainsKey(preface + str + "4_" + num.ToString()))
          return new StardewValley.Dialogue(this.Dialogue[preface + str + "4" + appendToEnd], this);
        return new StardewValley.Dialogue(this.Dialogue[preface + str + "4_" + num.ToString() + appendToEnd], this);
      }
      if (heartLevel >= 2 && this.Dialogue.ContainsKey(preface + str + "2" + appendToEnd))
      {
        if (!this.Dialogue.ContainsKey(preface + str + "2_" + num.ToString()))
          return new StardewValley.Dialogue(this.Dialogue[preface + str + "2" + appendToEnd], this);
        return new StardewValley.Dialogue(this.Dialogue[preface + str + "2_" + num.ToString() + appendToEnd], this);
      }
      if (!this.Dialogue.ContainsKey(preface + str + appendToEnd))
        return (StardewValley.Dialogue) null;
      if (this.Name.Equals("Caroline") && Game1.isLocationAccessible("CommunityCenter") && preface == "summer_" && str == "Mon")
        return new StardewValley.Dialogue(this.Dialogue["summer_Wed"], this);
      if (!this.Dialogue.ContainsKey(preface + str + "_" + num.ToString() + appendToEnd))
        return new StardewValley.Dialogue(this.Dialogue[preface + str + appendToEnd], this);
      return new StardewValley.Dialogue(this.Dialogue[preface + str + "_" + num.ToString() + appendToEnd], this);
    }

    public void clearSchedule() => this.schedule = (Dictionary<int, SchedulePathDescription>) null;

    public void checkSchedule(int timeOfDay)
    {
      if ((double) this.currentScheduleDelay == 0.0 && (double) this.scheduleDelaySeconds > 0.0)
      {
        this.currentScheduleDelay = this.scheduleDelaySeconds;
      }
      else
      {
        if (this.returningToEndPoint)
          return;
        this.updatedDialogueYet = false;
        this.extraDialogueMessageToAddThisMorning = (string) null;
        if (this.ignoreScheduleToday || this.schedule == null)
          return;
        SchedulePathDescription schedulePathDescription = (SchedulePathDescription) null;
        if (this.lastAttemptedSchedule < timeOfDay)
        {
          this.lastAttemptedSchedule = timeOfDay;
          this.schedule.TryGetValue(timeOfDay, out schedulePathDescription);
          if (schedulePathDescription != null)
            this.queuedSchedulePaths.Add(new KeyValuePair<int, SchedulePathDescription>(timeOfDay, schedulePathDescription));
          schedulePathDescription = (SchedulePathDescription) null;
        }
        if (this.controller != null && this.controller.pathToEndPoint != null && this.controller.pathToEndPoint.Count > 0)
          return;
        if (this.queuedSchedulePaths.Count > 0 && timeOfDay >= this.queuedSchedulePaths[0].Key)
          schedulePathDescription = this.queuedSchedulePaths[0].Value;
        if (schedulePathDescription == null)
          return;
        this.prepareToDisembarkOnNewSchedulePath();
        if (this.returningToEndPoint || this.temporaryController != null)
          return;
        this.directionsToNewLocation = schedulePathDescription;
        if (this.queuedSchedulePaths.Count > 0)
          this.queuedSchedulePaths.RemoveAt(0);
        this.controller = new PathFindController(this.directionsToNewLocation.route, (Character) this, Utility.getGameLocationOfCharacter(this))
        {
          finalFacingDirection = this.directionsToNewLocation.facingDirection,
          endBehaviorFunction = this.getRouteEndBehaviorFunction(this.directionsToNewLocation.endOfRouteBehavior, this.directionsToNewLocation.endOfRouteMessage)
        };
        if (this.controller.pathToEndPoint == null || this.controller.pathToEndPoint.Count == 0)
        {
          if (this.controller.endBehaviorFunction != null)
            this.controller.endBehaviorFunction((Character) this, this.currentLocation);
          this.controller = (PathFindController) null;
        }
        if (this.directionsToNewLocation == null || this.directionsToNewLocation.route == null)
          return;
        this.previousEndPoint = this.directionsToNewLocation.route.Count > 0 ? this.directionsToNewLocation.route.Last<Point>() : Point.Zero;
      }
    }

    private void finishEndOfRouteAnimation()
    {
      this._finishingEndOfRouteBehavior = this._startedEndOfRouteBehavior;
      this._startedEndOfRouteBehavior = (string) null;
      if (this._finishingEndOfRouteBehavior == "change_beach")
      {
        this.shouldWearIslandAttire.Value = true;
        this.currentlyDoingEndOfRouteAnimation = false;
      }
      else if (this._finishingEndOfRouteBehavior == "change_normal")
      {
        this.shouldWearIslandAttire.Value = false;
        this.currentlyDoingEndOfRouteAnimation = false;
      }
      while (this.CurrentDialogue.Count > 0 && this.CurrentDialogue.Peek().removeOnNextMove)
        this.CurrentDialogue.Pop();
      this.shouldSayMarriageDialogue.Value = false;
      this.currentMarriageDialogue.Clear();
      this.nextEndOfRouteMessage = (string) null;
      this.endOfRouteMessage.Value = (string) null;
      if (this.currentlyDoingEndOfRouteAnimation && this.routeEndOutro != null)
      {
        List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>();
        for (int index = 0; index < this.routeEndOutro.Length; ++index)
        {
          if (index == this.routeEndOutro.Length - 1)
            animation.Add(new FarmerSprite.AnimationFrame(this.routeEndOutro[index], 100, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.routeEndAnimationFinished), true));
          else
            animation.Add(new FarmerSprite.AnimationFrame(this.routeEndOutro[index], 100, 0, false, false));
        }
        if (animation.Count > 0)
          this.Sprite.setCurrentAnimation(animation);
        else
          this.routeEndAnimationFinished((Farmer) null);
        if (this._finishingEndOfRouteBehavior == null)
          return;
        this.finishRouteBehavior(this._finishingEndOfRouteBehavior);
      }
      else
        this.routeEndAnimationFinished((Farmer) null);
    }

    private void prepareToDisembarkOnNewSchedulePath()
    {
      this.finishEndOfRouteAnimation();
      this.doingEndOfRouteAnimation.Value = false;
      this.currentlyDoingEndOfRouteAnimation = false;
      if (!this.isMarried())
        return;
      if (this.temporaryController == null && Utility.getGameLocationOfCharacter(this) is FarmHouse)
      {
        this.temporaryController = new PathFindController((Character) this, this.getHome(), new Point(this.getHome().warps[0].X, this.getHome().warps[0].Y), 2, true)
        {
          NPCSchedule = true
        };
        if (this.temporaryController.pathToEndPoint == null || this.temporaryController.pathToEndPoint.Count <= 0)
        {
          this.temporaryController = (PathFindController) null;
          this.schedule = (Dictionary<int, SchedulePathDescription>) null;
        }
        else
          this.followSchedule = true;
      }
      else
      {
        if (!(Utility.getGameLocationOfCharacter(this) is Farm))
          return;
        this.temporaryController = (PathFindController) null;
        this.schedule = (Dictionary<int, SchedulePathDescription>) null;
      }
    }

    public void checkForMarriageDialogue(int timeOfDay, GameLocation location)
    {
      if (this.Name == "Krobus" && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) == "Fri")
        return;
      switch (timeOfDay)
      {
        case 1100:
          this.setRandomAfternoonMarriageDialogue(1100, location);
          break;
        case 1800:
          if (!(location is FarmHouse))
            break;
          int num = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + timeOfDay + (int) this.getSpouse().UniqueMultiplayerID).Next(Game1.isRaining ? 7 : 6) - 1;
          string str = num < 0 ? this.Name : num.ToString() ?? "";
          this.currentMarriageDialogue.Clear();
          this.addMarriageDialogue("MarriageDialogue", (Game1.isRaining ? "Rainy" : "Indoor") + "_Night_" + str, false);
          break;
      }
    }

    private void routeEndAnimationFinished(Farmer who)
    {
      this.doingEndOfRouteAnimation.Value = false;
      this.freezeMotion = false;
      this.Sprite.SpriteHeight = 32;
      this.Sprite.oldFrame = this._beforeEndOfRouteAnimationFrame;
      this.Sprite.StopAnimation();
      this.endOfRouteMessage.Value = (string) null;
      this.isCharging = false;
      this.speed = 2;
      this.addedSpeed = 0;
      this.goingToDoEndOfRouteAnimation.Value = false;
      if (this.isWalkingInSquare)
        this.returningToEndPoint = true;
      if (this._finishingEndOfRouteBehavior == "penny_dishes")
        this.drawOffset.Value = new Vector2(0.0f, 0.0f);
      if (this.appliedRouteAnimationOffset != Vector2.Zero)
      {
        this.drawOffset.Value = new Vector2(0.0f, 0.0f);
        this.appliedRouteAnimationOffset = Vector2.Zero;
      }
      this._finishingEndOfRouteBehavior = (string) null;
    }

    public bool isOnSilentTemporaryMessage() => ((bool) (NetFieldBase<bool, NetBool>) this.doingEndOfRouteAnimation || !(bool) (NetFieldBase<bool, NetBool>) this.goingToDoEndOfRouteAnimation) && this.endOfRouteMessage.Value != null && this.endOfRouteMessage.Value.ToLower().Equals("silent");

    public bool hasTemporaryMessageAvailable() => !this.isDivorcedFrom(Game1.player) && (this.currentLocation != null && this.currentLocation.HasLocationOverrideDialogue(this) || this.endOfRouteMessage.Value != null && ((bool) (NetFieldBase<bool, NetBool>) this.doingEndOfRouteAnimation || !(bool) (NetFieldBase<bool, NetBool>) this.goingToDoEndOfRouteAnimation));

    public bool setTemporaryMessages(Farmer who)
    {
      if (this.isOnSilentTemporaryMessage())
        return true;
      if (this.endOfRouteMessage.Value != null && ((bool) (NetFieldBase<bool, NetBool>) this.doingEndOfRouteAnimation || !(bool) (NetFieldBase<bool, NetBool>) this.goingToDoEndOfRouteAnimation))
      {
        if (!this.isDivorcedFrom(Game1.player) && (!this.endOfRouteMessage.Value.Contains("marriage") || this.getSpouse() == Game1.player))
        {
          this._PushTemporaryDialogue((string) (NetFieldBase<string, NetString>) this.endOfRouteMessage);
          return false;
        }
      }
      else if (this.currentLocation != null && this.currentLocation.HasLocationOverrideDialogue(this))
      {
        this._PushTemporaryDialogue(this.currentLocation.GetLocationOverrideDialogue(this));
        return false;
      }
      return false;
    }

    protected void _PushTemporaryDialogue(string dialogue_key)
    {
      if (dialogue_key.StartsWith("Resort"))
      {
        string path = "Resort_Marriage" + dialogue_key.Substring(6);
        if (Game1.content.LoadStringReturnNullIfNotFound(path) != null)
          dialogue_key = path;
      }
      if (this.CurrentDialogue.Count != 0 && !(this.CurrentDialogue.Peek().temporaryDialogueKey != dialogue_key))
        return;
      this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.LoadString(dialogue_key), this)
      {
        removeOnNextMove = true,
        temporaryDialogueKey = dialogue_key
      });
    }

    private void walkInSquareAtEndOfRoute(Character c, GameLocation l) => this.startRouteBehavior((string) (NetFieldBase<string, NetString>) this.endOfRouteBehaviorName);

    private void doAnimationAtEndOfScheduleRoute(Character c, GameLocation l)
    {
      this.doingEndOfRouteAnimation.Value = true;
      this.reallyDoAnimationAtEndOfScheduleRoute();
      this.currentlyDoingEndOfRouteAnimation = true;
    }

    private void reallyDoAnimationAtEndOfScheduleRoute()
    {
      this._startedEndOfRouteBehavior = this.loadedEndOfRouteBehavior;
      bool flag = false;
      if (this._startedEndOfRouteBehavior == "change_beach")
        flag = true;
      else if (this._startedEndOfRouteBehavior == "change_normal")
        flag = true;
      if (!flag)
      {
        if (this._startedEndOfRouteBehavior == "penny_dishes")
          this.drawOffset.Value = new Vector2(0.0f, 16f);
        if (this._startedEndOfRouteBehavior.EndsWith("_sleep"))
        {
          this.layingDown = true;
          this.HideShadow = true;
        }
        if (this.routeAnimationMetadata != null)
        {
          for (int index = 0; index < this.routeAnimationMetadata.Length; ++index)
          {
            string[] strArray = this.routeAnimationMetadata[index].Split(' ');
            if (strArray[0] == "laying_down")
            {
              this.layingDown = true;
              this.HideShadow = true;
            }
            else if (strArray[0] == "offset")
              this.appliedRouteAnimationOffset = new Vector2((float) int.Parse(strArray[1]), (float) int.Parse(strArray[2]));
          }
        }
        if (this.appliedRouteAnimationOffset != Vector2.Zero)
          this.drawOffset.Value = this.appliedRouteAnimationOffset;
        if (this._skipRouteEndIntro)
        {
          this.doMiddleAnimation((Farmer) null);
        }
        else
        {
          List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>();
          for (int index = 0; index < this.routeEndIntro.Length; ++index)
          {
            if (index == this.routeEndIntro.Length - 1)
              animation.Add(new FarmerSprite.AnimationFrame(this.routeEndIntro[index], 100, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doMiddleAnimation), true));
            else
              animation.Add(new FarmerSprite.AnimationFrame(this.routeEndIntro[index], 100, 0, false, false));
          }
          this.Sprite.setCurrentAnimation(animation);
        }
      }
      this._skipRouteEndIntro = false;
      this.doingEndOfRouteAnimation.Value = true;
      this.freezeMotion = true;
      this._beforeEndOfRouteAnimationFrame = this.Sprite.oldFrame;
    }

    private void doMiddleAnimation(Farmer who)
    {
      List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>();
      for (int index = 0; index < this.routeEndAnimation.Length; ++index)
        animation.Add(new FarmerSprite.AnimationFrame(this.routeEndAnimation[index], 100, 0, false, false));
      this.Sprite.setCurrentAnimation(animation);
      this.Sprite.loop = true;
      if (this._startedEndOfRouteBehavior == null)
        return;
      this.startRouteBehavior(this._startedEndOfRouteBehavior);
    }

    private void startRouteBehavior(string behaviorName)
    {
      if (behaviorName.Length > 0 && behaviorName[0] == '"')
      {
        if (!Game1.IsMasterGame)
          return;
        this.endOfRouteMessage.Value = behaviorName.Replace("\"", "");
      }
      else
      {
        if (behaviorName.Contains("square_") && Game1.IsMasterGame)
        {
          this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle(this.getTileX() * 64, this.getTileY() * 64, 64, 64);
          string[] strArray = behaviorName.Split('_');
          this.walkInSquare(Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), 6000);
          this.squareMovementFacingPreference = strArray.Length <= 3 ? -1 : Convert.ToInt32(strArray[3]);
        }
        if (behaviorName.Contains("sleep"))
        {
          this.isPlayingSleepingAnimation = true;
          this.playSleepingAnimation();
        }
        if (!(behaviorName == "abigail_videogames"))
        {
          if (!(behaviorName == "dick_fish"))
          {
            if (!(behaviorName == "clint_hammer"))
            {
              if (!(behaviorName == "birdie_fish"))
                return;
              this.extendSourceRect(16, 0);
              this.Sprite.SpriteWidth = 32;
              this.Sprite.ignoreSourceRectUpdates = false;
              this.Sprite.currentFrame = 8;
            }
            else
            {
              this.extendSourceRect(16, 0);
              this.Sprite.SpriteWidth = 32;
              this.Sprite.ignoreSourceRectUpdates = false;
              this.Sprite.currentFrame = 8;
              this.Sprite.CurrentAnimation[14] = new FarmerSprite.AnimationFrame(9, 100, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.clintHammerSound));
            }
          }
          else
          {
            this.extendSourceRect(0, 32);
            this.Sprite.tempSpriteHeight = 64;
            this.drawOffset.Value = new Vector2(0.0f, 96f);
            this.Sprite.ignoreSourceRectUpdates = false;
            if (!Utility.isOnScreen(Utility.Vector2ToPoint(this.Position), 64, this.currentLocation))
              return;
            this.currentLocation.playSoundAt("slosh", this.getTileLocation());
          }
        }
        else
        {
          if (!Game1.IsMasterGame)
            return;
          Game1.multiplayer.broadcastSprites(Utility.getGameLocationOfCharacter(this), new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(167, 1714, 19, 14), 100f, 3, 999999, new Vector2(2f, 3f) * 64f + new Vector2(7f, 12f) * 4f, false, false, 0.0002f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 688f
          });
          this.doEmote(52);
        }
      }
    }

    public void playSleepingAnimation()
    {
      this.isSleeping.Value = true;
      Vector2 vector2 = new Vector2(0.0f, this.name.Equals((object) "Sebastian") ? 12f : -4f);
      if (this.isMarried())
        vector2.X = -12f;
      this.drawOffset.Value = vector2;
      if (this.isPlayingSleepingAnimation)
        return;
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions");
      if (dictionary.ContainsKey(this.name.Value.ToLower() + "_sleep"))
      {
        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(Convert.ToInt32(dictionary[this.name.Value.ToLower() + "_sleep"].Split('/')[0]), 100, false, false)
        });
        this.Sprite.loop = true;
      }
      this.isPlayingSleepingAnimation = true;
    }

    private void finishRouteBehavior(string behaviorName)
    {
      if (!(behaviorName == "abigail_videogames"))
      {
        if (behaviorName == "birdie_fish" || behaviorName == "clint_hammer" || behaviorName == "dick_fish")
        {
          this.reloadSprite();
          this.Sprite.SpriteWidth = 16;
          this.Sprite.SpriteHeight = 32;
          this.Sprite.UpdateSourceRect();
          this.drawOffset.Value = Vector2.Zero;
          this.Halt();
          this.movementPause = 1;
        }
      }
      else
        Utility.getGameLocationOfCharacter(this).removeTemporarySpritesWithID(688);
      if (!this.layingDown)
        return;
      this.layingDown = false;
      this.HideShadow = false;
    }

    public bool IsReturningToEndPoint() => this.returningToEndPoint;

    public void StartActivityWalkInSquare(int square_width, int square_height, int pause_offset)
    {
      this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle(this.getTileX() * 64, this.getTileY() * 64, 64, 64);
      this.walkInSquare(square_height, square_height, pause_offset);
    }

    public void EndActivityRouteEndBehavior() => this.finishEndOfRouteAnimation();

    public void StartActivityRouteEndBehavior(string behavior_name, string end_message)
    {
      PathFindController.endBehavior behaviorFunction = this.getRouteEndBehaviorFunction(behavior_name, end_message);
      if (behaviorFunction == null)
        return;
      behaviorFunction((Character) this, this.currentLocation);
    }

    private PathFindController.endBehavior getRouteEndBehaviorFunction(
      string behaviorName,
      string endMessage)
    {
      if (endMessage != null || behaviorName != null && behaviorName.Length > 0 && behaviorName[0] == '"')
        this.nextEndOfRouteMessage = endMessage.Replace("\"", "");
      if (behaviorName == null)
        return (PathFindController.endBehavior) null;
      if (behaviorName.Length > 0 && behaviorName.Contains("square_"))
      {
        this.endOfRouteBehaviorName.Value = behaviorName;
        return new PathFindController.endBehavior(this.walkInSquareAtEndOfRoute);
      }
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions");
      if (behaviorName == "change_beach" || behaviorName == "change_normal")
      {
        this.endOfRouteBehaviorName.Value = behaviorName;
        this.goingToDoEndOfRouteAnimation.Value = true;
      }
      else
      {
        if (!dictionary.ContainsKey(behaviorName))
          return (PathFindController.endBehavior) null;
        this.endOfRouteBehaviorName.Value = behaviorName;
        this.loadEndOfRouteBehavior((string) (NetFieldBase<string, NetString>) this.endOfRouteBehaviorName);
        this.goingToDoEndOfRouteAnimation.Value = true;
      }
      return new PathFindController.endBehavior(this.doAnimationAtEndOfScheduleRoute);
    }

    private void loadEndOfRouteBehavior(string name)
    {
      this.loadedEndOfRouteBehavior = name;
      if (name.Length > 0 && name.Contains("square_"))
        return;
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions");
      if (!dictionary.ContainsKey(name))
        return;
      string[] source = dictionary[name].Split('/');
      this.routeEndIntro = Utility.parseStringToIntArray(source[0]);
      this.routeEndAnimation = Utility.parseStringToIntArray(source[1]);
      this.routeEndOutro = Utility.parseStringToIntArray(source[2]);
      if (source.Length > 3 && source[3] != "")
        this.nextEndOfRouteMessage = source[3];
      if (source.Length > 4)
        this.routeAnimationMetadata = ((IEnumerable<string>) source).Skip<string>(4).ToArray<string>();
      else
        this.routeAnimationMetadata = (string[]) null;
    }

    public void warp(bool wasOutdoors)
    {
    }

    public void shake(int duration) => this.shakeTimer = duration;

    public void setNewDialogue(string s, bool add = false, bool clearOnMovement = false)
    {
      if (!add)
        this.CurrentDialogue.Clear();
      this.CurrentDialogue.Push(new StardewValley.Dialogue(s, this)
      {
        removeOnNextMove = clearOnMovement
      });
    }

    public void setNewDialogue(
      string dialogueSheetName,
      string dialogueSheetKey,
      int numberToAppend = -1,
      bool add = false,
      bool clearOnMovement = false)
    {
      if (!add)
        this.CurrentDialogue.Clear();
      string str = numberToAppend == -1 ? this.Name : "";
      if (dialogueSheetName.Contains("Marriage"))
      {
        if (this.getSpouse() != Game1.player)
          return;
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this.tryToGetMarriageSpecificDialogueElseReturnDefault(dialogueSheetKey + (numberToAppend != -1 ? numberToAppend.ToString() ?? "" : "") + str), this)
        {
          removeOnNextMove = clearOnMovement
        });
      }
      else
      {
        if (!Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\" + dialogueSheetName).ContainsKey(dialogueSheetKey + (numberToAppend != -1 ? numberToAppend.ToString() ?? "" : "") + str))
          return;
        this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\" + dialogueSheetName)[dialogueSheetKey + (numberToAppend != -1 ? numberToAppend.ToString() ?? "" : "") + str], this)
        {
          removeOnNextMove = clearOnMovement
        });
      }
    }

    public string GetDialogueSheetName() => this.Name == "Leo" && this.DefaultMap != "IslandHut" ? this.Name + "Mainland" : this.Name;

    public void setSpouseRoomMarriageDialogue()
    {
      this.currentMarriageDialogue.Clear();
      this.addMarriageDialogue("MarriageDialogue", "spouseRoom_" + this.Name, false);
    }

    public void setRandomAfternoonMarriageDialogue(
      int time,
      GameLocation location,
      bool countAsDailyAfternoon = false)
    {
      if (this.Name == "Krobus" && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) == "Fri" || this.hasSaidAfternoonDialogue.Value)
        return;
      if (countAsDailyAfternoon)
        this.hasSaidAfternoonDialogue.Value = true;
      Random random = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + time);
      int heartLevelForNpc = this.getSpouse().getFriendshipHeartLevelForNPC(this.Name);
      switch (location)
      {
        case FarmHouse _ when random.NextDouble() < 0.5:
          if (heartLevelForNpc < 9)
          {
            this.currentMarriageDialogue.Clear();
            this.addMarriageDialogue("MarriageDialogue", random.NextDouble() < (double) heartLevelForNpc / 11.0 ? "Neutral_" : "Bad_" + random.Next(10).ToString(), false);
            break;
          }
          if (random.NextDouble() < 0.05)
          {
            this.currentMarriageDialogue.Clear();
            this.addMarriageDialogue("MarriageDialogue", Game1.currentSeason + "_" + this.Name, false);
            break;
          }
          if (heartLevelForNpc >= 10 && random.NextDouble() < 0.5 || heartLevelForNpc >= 11 && random.NextDouble() < 0.75)
          {
            this.currentMarriageDialogue.Clear();
            this.addMarriageDialogue("MarriageDialogue", "Good_" + random.Next(10).ToString(), false);
            break;
          }
          this.currentMarriageDialogue.Clear();
          this.addMarriageDialogue("MarriageDialogue", "Neutral_" + random.Next(10).ToString(), false);
          break;
        case Farm _:
          if (random.NextDouble() < 0.2)
          {
            this.currentMarriageDialogue.Clear();
            this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + this.Name, false);
            break;
          }
          this.currentMarriageDialogue.Clear();
          this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + random.Next(5).ToString(), false);
          break;
      }
    }

    public bool isBirthday(string season, int day) => this.Birthday_Season != null && this.Birthday_Season.Equals(season) && this.Birthday_Day == day;

    public Object getFavoriteItem()
    {
      string str;
      Game1.NPCGiftTastes.TryGetValue(this.Name, out str);
      return str != null ? new Object(Convert.ToInt32(str.Split('/')[1].Split(' ')[0]), 1) : (Object) null;
    }

    public void receiveGift(
      Object o,
      Farmer giver,
      bool updateGiftLimitInfo = true,
      float friendshipChangeMultiplier = 1f,
      bool showResponse = true)
    {
      string str1;
      Game1.NPCGiftTastes.TryGetValue(this.Name, out str1);
      string[] strArray = str1.Split('/');
      float num = 1f;
      switch (o.Quality)
      {
        case 1:
          num = 1.1f;
          break;
        case 2:
          num = 1.25f;
          break;
        case 4:
          num = 1.5f;
          break;
      }
      if ((this.Birthday_Season == null || !Game1.currentSeason.Equals(this.Birthday_Season) ? 0 : (Game1.dayOfMonth == this.Birthday_Day ? 1 : 0)) != 0)
      {
        friendshipChangeMultiplier = 8f;
        string str2 = this.Manners == 2 ? Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4274") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4275");
        if (Game1.random.NextDouble() < 0.5)
          str2 = this.Manners == 2 ? Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4276") : Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4277");
        string str3 = this.Manners == 2 ? Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4278") : Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4279");
        strArray[0] = str2;
        strArray[2] = str2;
        strArray[4] = str3;
        strArray[6] = str3;
        strArray[8] = this.Manners == 2 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4280") : Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4281");
      }
      giver?.onGiftGiven(this, o);
      if (this.getSpouse() != null && this.getSpouse().Equals((object) giver))
        friendshipChangeMultiplier /= 2f;
      if (str1 == null)
        return;
      ++Game1.stats.GiftsGiven;
      giver.currentLocation.localSound("give_gift");
      if (updateGiftLimitInfo)
      {
        ++giver.friendshipData[this.Name].GiftsToday;
        ++giver.friendshipData[this.Name].GiftsThisWeek;
        giver.friendshipData[this.Name].LastGiftDate = new WorldDate(Game1.Date);
      }
      int tasteForThisItem = this.getGiftTasteForThisItem((Item) o);
      switch (giver.FacingDirection)
      {
        case 0:
          ((FarmerSprite) giver.Sprite).animateBackwardsOnce(80, 50f);
          break;
        case 1:
          ((FarmerSprite) giver.Sprite).animateBackwardsOnce(72, 50f);
          break;
        case 2:
          ((FarmerSprite) giver.Sprite).animateBackwardsOnce(64, 50f);
          break;
        case 3:
          ((FarmerSprite) giver.Sprite).animateBackwardsOnce(88, 50f);
          break;
      }
      List<string> stringList = new List<string>();
      for (int index = 0; index < 8; index += 2)
        stringList.Add(strArray[index]);
      if (this.Name.Equals("Krobus") && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) == "Fri")
      {
        for (int index = 0; index < stringList.Count; ++index)
          stringList[index] = "...";
      }
      switch (tasteForThisItem)
      {
        case 0:
          if (this.Name.Contains("Dwarf"))
          {
            if (showResponse)
              Game1.drawDialogue(this, giver.canUnderstandDwarves ? stringList[0] : StardewValley.Dialogue.convertToDwarvish(stringList[0]));
          }
          else if (showResponse)
            Game1.drawDialogue(this, stringList[0] + "$h");
          giver.changeFriendship((int) (80.0 * (double) friendshipChangeMultiplier * (double) num), this);
          this.doEmote(20);
          this.faceTowardFarmerForPeriod(15000, 4, false, giver);
          break;
        case 2:
          if (this.Name.Contains("Dwarf"))
          {
            if (showResponse)
              Game1.drawDialogue(this, giver.canUnderstandDwarves ? stringList[1] : StardewValley.Dialogue.convertToDwarvish(stringList[1]));
          }
          else if (showResponse)
            Game1.drawDialogue(this, stringList[1] + "$h");
          giver.changeFriendship((int) (45.0 * (double) friendshipChangeMultiplier * (double) num), this);
          this.faceTowardFarmerForPeriod(7000, 3, true, giver);
          break;
        case 4:
          if (this.Name.Contains("Dwarf"))
          {
            if (showResponse)
              Game1.drawDialogue(this, giver.canUnderstandDwarves ? stringList[2] : StardewValley.Dialogue.convertToDwarvish(stringList[2]));
          }
          else if (showResponse)
            Game1.drawDialogue(this, stringList[2] + "$s");
          giver.changeFriendship((int) (-20.0 * (double) friendshipChangeMultiplier), this);
          break;
        case 6:
          if (this.Name.Contains("Dwarf"))
          {
            if (showResponse)
              Game1.drawDialogue(this, giver.canUnderstandDwarves ? stringList[3] : StardewValley.Dialogue.convertToDwarvish(stringList[3]));
          }
          else if (showResponse)
            Game1.drawDialogue(this, stringList[3] + "$s");
          giver.changeFriendship((int) (-40.0 * (double) friendshipChangeMultiplier), this);
          this.faceTowardFarmerForPeriod(15000, 4, true, giver);
          this.doEmote(12);
          break;
        default:
          if (this.Name.Contains("Dwarf"))
          {
            if (showResponse)
              Game1.drawDialogue(this, giver.canUnderstandDwarves ? strArray[8] : StardewValley.Dialogue.convertToDwarvish(strArray[8]));
          }
          else if (showResponse)
            Game1.drawDialogue(this, strArray[8]);
          giver.changeFriendship((int) (20.0 * (double) friendshipChangeMultiplier), this);
          break;
      }
    }

    public override void draw(SpriteBatch b, float alpha = 1f)
    {
      if (this.Sprite == null || this.IsInvisible || !Utility.isOnScreen(this.Position, 128) && (!this.eventActor || !(this.currentLocation is Summit)))
        return;
      if ((bool) (NetFieldBase<bool, NetBool>) this.swimming)
      {
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (80 + this.yJumpOffset * 2)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero) - new Vector2(0.0f, this.yOffset), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, this.Sprite.SourceRect.Width, this.Sprite.SourceRect.Height / 2 - (int) ((double) this.yOffset / 4.0))), Microsoft.Xna.Framework.Color.White, this.rotation, new Vector2(32f, 96f) / 4f, Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));
        Vector2 localPosition = this.getLocalPosition(Game1.viewport);
        b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int) localPosition.X + (int) this.yOffset + 8, (int) localPosition.Y - 128 + this.Sprite.SourceRect.Height * 4 + 48 + this.yJumpOffset * 2 - (int) this.yOffset, this.Sprite.SourceRect.Width * 4 - (int) this.yOffset * 2 - 16, 4), new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), Microsoft.Xna.Framework.Color.White * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, (float) ((double) this.getStandingY() / 10000.0 + 1.0 / 1000.0));
      }
      else
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.GetSpriteWidthForPositioning() * 4 / 2), (float) (this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), Microsoft.Xna.Framework.Color.White * alpha, this.rotation, new Vector2((float) (this.Sprite.SpriteWidth / 2), (float) ((double) this.Sprite.SpriteHeight * 3.0 / 4.0)), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip || this.Sprite.CurrentAnimation != null && this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.getStandingY() / 10000f));
      if (this.Breather && this.shakeTimer <= 0 && !(bool) (NetFieldBase<bool, NetBool>) this.swimming && this.Sprite.currentFrame < 16 && !this.farmerPassesThrough)
      {
        Microsoft.Xna.Framework.Rectangle sourceRect = this.Sprite.SourceRect;
        sourceRect.Y += this.Sprite.SpriteHeight / 2 + this.Sprite.SpriteHeight / 32;
        sourceRect.Height = this.Sprite.SpriteHeight / 4;
        sourceRect.X += this.Sprite.SpriteWidth / 4;
        sourceRect.Width = this.Sprite.SpriteWidth / 2;
        Vector2 vector2 = new Vector2((float) (this.Sprite.SpriteWidth * 4 / 2), 8f);
        if (this.Age == 2)
        {
          sourceRect.Y += this.Sprite.SpriteHeight / 6 + 1;
          sourceRect.Height /= 2;
          vector2.Y += (float) (this.Sprite.SpriteHeight / 8 * 4);
          if (this is Child)
          {
            if ((this as Child).Age == 0)
              vector2.X -= 12f;
            else if ((this as Child).Age == 1)
              vector2.X -= 4f;
          }
        }
        else if (this.Gender == 1)
        {
          ++sourceRect.Y;
          vector2.Y -= 4f;
          sourceRect.Height /= 2;
        }
        float num = Math.Max(0.0f, (float) (Math.Ceiling(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 600.0 + (double) this.defaultPosition.X * 20.0)) / 4.0));
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + vector2 + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(sourceRect), Microsoft.Xna.Framework.Color.White * alpha, this.rotation, new Vector2((float) (sourceRect.Width / 2), (float) (sourceRect.Height / 2 + 1)), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f + num, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.992f : (float) ((double) this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
      }
      if (this.isGlowing)
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.GetSpriteWidthForPositioning() * 4 / 2), (float) (this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, this.rotation, new Vector2((float) (this.Sprite.SpriteWidth / 2), (float) ((double) this.Sprite.SpriteHeight * 3.0 / 4.0)), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.99f : (float) ((double) this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
      if (!this.IsEmoting || Game1.eventUp)
        return;
      switch (this)
      {
        case Child _:
          break;
        case Pet _:
          break;
        default:
          Vector2 localPosition1 = this.getLocalPosition(Game1.viewport);
          localPosition1.Y -= (float) (32 + this.Sprite.SpriteHeight * 4);
          b.Draw(Game1.emoteSpriteSheet, localPosition1, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) this.getStandingY() / 10000f);
          break;
      }
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      if (this.textAboveHeadTimer <= 0 || this.textAboveHead == null)
        return;
      Vector2 local = Game1.GlobalToLocal(new Vector2((float) this.getStandingX(), (float) (this.getStandingY() - this.Sprite.SpriteHeight * 4 - 64 + this.yJumpOffset)));
      if (this.textAboveHeadStyle == 0)
        local += new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
      if (this.NeedsBirdieEmoteHack())
        local.X += (float) (-this.GetBoundingBox().Width / 4 + 64);
      if (this.shouldShadowBeOffset)
        local += this.drawOffset.Value;
      SpriteText.drawStringWithScrollCenteredAt(b, this.textAboveHead, (int) local.X, (int) local.Y, alpha: this.textAboveHeadAlpha, color: this.textAboveHeadColor, scrollType: 1, layerDepth: ((float) ((double) (this.getTileY() * 64) / 10000.0 + 1.0 / 1000.0 + (double) this.getTileX() / 10000.0)));
    }

    public bool NeedsBirdieEmoteHack() => Game1.eventUp && this.Sprite.SpriteWidth == 32 && this.Name == "Birdie";

    public void warpToPathControllerDestination()
    {
      if (this.controller == null)
        return;
      while (this.controller.pathToEndPoint.Count > 2)
      {
        this.controller.pathToEndPoint.Pop();
        this.controller.handleWarps(new Microsoft.Xna.Framework.Rectangle(this.controller.pathToEndPoint.Peek().X * 64, this.controller.pathToEndPoint.Peek().Y * 64, 64, 64));
        this.Position = new Vector2((float) (this.controller.pathToEndPoint.Peek().X * 64), (float) (this.controller.pathToEndPoint.Peek().Y * 64 + 16));
        this.Halt();
      }
    }

    public virtual Microsoft.Xna.Framework.Rectangle getMugShotSourceRect() => new Microsoft.Xna.Framework.Rectangle(0, this.Age == 2 ? 4 : 0, 16, 24);

    public void getHitByPlayer(Farmer who, GameLocation location)
    {
      this.doEmote(12);
      if (who == null)
      {
        if (Game1.IsMultiplayer)
          return;
        who = Game1.player;
      }
      if (who.friendshipData.ContainsKey(this.Name))
      {
        who.changeFriendship(-30, this);
        if (who.IsLocalPlayer)
        {
          this.CurrentDialogue.Clear();
          this.CurrentDialogue.Push(new StardewValley.Dialogue(Game1.random.NextDouble() < 0.5 ? Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4293") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4294"), this));
        }
        location.debris.Add(new Debris((string) (NetFieldBase<string, NetString>) this.Sprite.textureName, Game1.random.Next(3, 8), new Vector2((float) this.GetBoundingBox().Center.X, (float) this.GetBoundingBox().Center.Y)));
      }
      if (this.Name.Equals("Bouncer"))
        location.localSound("crafting");
      else
        location.localSound("hitEnemy");
    }

    public void walkInSquare(int squareWidth, int squareHeight, int squarePauseOffset)
    {
      this.isWalkingInSquare = true;
      this.lengthOfWalkingSquareX = squareWidth;
      this.lengthOfWalkingSquareY = squareHeight;
      this.squarePauseOffset = squarePauseOffset;
    }

    public void moveTowardPlayer(int threshold)
    {
      this.isWalkingTowardPlayer.Value = true;
      this.moveTowardPlayerThreshold.Value = threshold;
    }

    protected virtual Farmer findPlayer() => Game1.MasterPlayer;

    public virtual bool withinPlayerThreshold() => this.withinPlayerThreshold((int) (NetFieldBase<int, NetInt>) this.moveTowardPlayerThreshold);

    public virtual bool withinPlayerThreshold(int threshold)
    {
      if (this.currentLocation != null && !this.currentLocation.farmers.Any())
        return false;
      Vector2 tileLocation1 = this.findPlayer().getTileLocation();
      Vector2 tileLocation2 = this.getTileLocation();
      return (double) Math.Abs(tileLocation2.X - tileLocation1.X) <= (double) threshold && (double) Math.Abs(tileLocation2.Y - tileLocation1.Y) <= (double) threshold;
    }

    private Stack<Point> addToStackForSchedule(Stack<Point> original, Stack<Point> toAdd)
    {
      if (toAdd == null)
        return original;
      original = new Stack<Point>((IEnumerable<Point>) original);
      while (original.Count > 0)
        toAdd.Push(original.Pop());
      return toAdd;
    }

    private SchedulePathDescription pathfindToNextScheduleLocation(
      string startingLocation,
      int startingX,
      int startingY,
      string endingLocation,
      int endingX,
      int endingY,
      int finalFacingDirection,
      string endBehavior,
      string endMessage)
    {
      Stack<Point> pointStack = new Stack<Point>();
      Point startPoint = new Point(startingX, startingY);
      List<string> stringList = !startingLocation.Equals(endingLocation, StringComparison.Ordinal) ? this.getLocationRoute(startingLocation, endingLocation) : (List<string>) null;
      if (stringList != null)
      {
        for (int index = 0; index < stringList.Count; ++index)
        {
          GameLocation locationFromName = Game1.getLocationFromName(stringList[index]);
          if (locationFromName.Name.Equals("Trailer") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
            locationFromName = Game1.getLocationFromName("Trailer_Big");
          if (index < stringList.Count - 1)
          {
            Point warpPointTo = locationFromName.getWarpPointTo(stringList[index + 1]);
            if (warpPointTo.Equals(Point.Zero) || startPoint.Equals(Point.Zero))
              throw new Exception("schedule pathing tried to find a warp point that doesn't exist.");
            pointStack = this.addToStackForSchedule(pointStack, PathFindController.findPathForNPCSchedules(startPoint, warpPointTo, locationFromName, 30000));
            startPoint = locationFromName.getWarpPointTarget(warpPointTo, (Character) this);
          }
          else
            pointStack = this.addToStackForSchedule(pointStack, PathFindController.findPathForNPCSchedules(startPoint, new Point(endingX, endingY), locationFromName, 30000));
        }
      }
      else if (startingLocation.Equals(endingLocation, StringComparison.Ordinal))
      {
        GameLocation locationFromName = Game1.getLocationFromName(startingLocation);
        if (locationFromName.Name.Equals("Trailer") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
          locationFromName = Game1.getLocationFromName("Trailer_Big");
        pointStack = PathFindController.findPathForNPCSchedules(startPoint, new Point(endingX, endingY), locationFromName, 30000);
      }
      return new SchedulePathDescription(pointStack, finalFacingDirection, endBehavior, endMessage);
    }

    private List<string> getLocationRoute(string startingLocation, string endingLocation)
    {
      foreach (List<string> source in NPC.routesFromLocationToLocation)
      {
        if (source.First<string>().Equals(startingLocation, StringComparison.Ordinal) && source.Last<string>().Equals(endingLocation, StringComparison.Ordinal) && ((int) (NetFieldBase<int, NetInt>) this.gender == 0 || !source.Contains<string>("BathHouse_MensLocker", (IEqualityComparer<string>) StringComparer.Ordinal)) && ((int) (NetFieldBase<int, NetInt>) this.gender != 0 || !source.Contains<string>("BathHouse_WomensLocker", (IEqualityComparer<string>) StringComparer.Ordinal)))
          return source;
      }
      return (List<string>) null;
    }

    /// <summary>
    /// returns true if location is inaccessable and should use "Default" instead.
    /// 
    /// 
    /// </summary>
    /// <param name="locationName"></param>
    /// <param name="tileX"></param>
    /// <param name="tileY"></param>
    /// <param name="facingDirection"></param>
    /// <returns></returns>
    private bool changeScheduleForLocationAccessibility(
      ref string locationName,
      ref int tileX,
      ref int tileY,
      ref int facingDirection)
    {
      string str = locationName;
      if (!(str == "JojaMart") && !(str == "Railroad"))
      {
        if (str == "CommunityCenter")
          return !Game1.isLocationAccessible(locationName);
      }
      else if (!Game1.isLocationAccessible(locationName))
      {
        if (!this.hasMasterScheduleEntry(locationName + "_Replacement"))
          return true;
        string[] strArray = this.getMasterScheduleEntry(locationName + "_Replacement").Split(' ');
        locationName = strArray[0];
        tileX = Convert.ToInt32(strArray[1]);
        tileY = Convert.ToInt32(strArray[2]);
        facingDirection = Convert.ToInt32(strArray[3]);
      }
      return false;
    }

    public Dictionary<int, SchedulePathDescription> parseMasterSchedule(
      string rawData)
    {
      string[] strArray1 = rawData.Split('/');
      Dictionary<int, SchedulePathDescription> masterSchedule = new Dictionary<int, SchedulePathDescription>();
      int index1 = 0;
      if (strArray1[0].Contains("GOTO"))
      {
        string currentSeason = strArray1[0].Split(' ')[1];
        if (currentSeason.ToLower().Equals("season"))
          currentSeason = Game1.currentSeason;
        try
        {
          strArray1 = this.getMasterScheduleRawData()[currentSeason].Split('/');
        }
        catch (Exception ex)
        {
          return this.parseMasterSchedule(this.getMasterScheduleEntry("spring"));
        }
      }
      if (strArray1[0].Contains("NOT"))
      {
        string[] strArray2 = strArray1[0].Split(' ');
        if (strArray2[1].ToLower() == "friendship")
        {
          int index2 = 2;
          bool flag = false;
          for (; index2 < strArray2.Length; index2 += 2)
          {
            string name = strArray2[index2];
            int result = 0;
            if (int.TryParse(strArray2[index2 + 1], out result))
            {
              foreach (Farmer allFarmer in Game1.getAllFarmers())
              {
                if (allFarmer.getFriendshipHeartLevelForNPC(name) >= result)
                {
                  flag = true;
                  break;
                }
              }
            }
            if (flag)
              break;
          }
          if (flag)
            return this.parseMasterSchedule(this.getMasterScheduleEntry("spring"));
          ++index1;
        }
      }
      else if (strArray1[0].Contains("MAIL"))
      {
        string id = strArray1[0].Split(' ')[1];
        if (Game1.MasterPlayer.mailReceived.Contains(id) || NetWorldState.checkAnywhereForWorldStateID(id))
          index1 += 2;
        else
          ++index1;
      }
      if (strArray1[index1].Contains("GOTO"))
      {
        string currentSeason = strArray1[index1].Split(' ')[1];
        if (currentSeason.ToLower().Equals("season"))
          currentSeason = Game1.currentSeason;
        else if (currentSeason.ToLower().Equals("no_schedule"))
        {
          this.followSchedule = false;
          return (Dictionary<int, SchedulePathDescription>) null;
        }
        return this.parseMasterSchedule(this.getMasterScheduleEntry(currentSeason));
      }
      Point point1 = this.isMarried() ? new Point(0, 23) : new Point((int) this.defaultPosition.X / 64, (int) this.defaultPosition.Y / 64);
      string startingLocation = this.isMarried() ? "BusStop" : (string) (NetFieldBase<string, NetString>) this.defaultMap;
      int val2 = 610;
      string targetLocationName = this.DefaultMap;
      int x = (int) ((double) this.defaultPosition.X / 64.0);
      int y = (int) ((double) this.defaultPosition.Y / 64.0);
      bool flag1 = false;
      for (int index3 = index1; index3 < strArray1.Length && strArray1.Length > 1; ++index3)
      {
        int index4 = 0;
        string[] strArray3 = strArray1[index3].Split(' ');
        bool flag2 = false;
        string str1 = strArray3[index4];
        if (str1.Length > 0 && strArray3[index4][0] == 'a')
        {
          flag2 = true;
          str1 = str1.Substring(1);
        }
        int num1 = Convert.ToInt32(str1);
        int index5 = index4 + 1;
        string locationName = strArray3[index5];
        string endBehavior = (string) null;
        string endMessage = (string) null;
        int result1 = 0;
        int result2 = 0;
        int result3 = 2;
        int index6;
        if (locationName == "bed")
        {
          if (this.isMarried())
          {
            locationName = "BusStop";
            result1 = -1;
            result2 = 23;
            result3 = 3;
          }
          else
          {
            string str2 = (string) null;
            if (this.hasMasterScheduleEntry("default"))
              str2 = this.getMasterScheduleEntry("default");
            else if (this.hasMasterScheduleEntry("spring"))
              str2 = this.getMasterScheduleEntry("spring");
            if (str2 != null)
            {
              try
              {
                string[] strArray4 = str2.Split('/');
                string[] strArray5 = strArray4[strArray4.Length - 1].Split(' ');
                locationName = strArray5[1];
                if (strArray5.Length > 3)
                {
                  if (int.TryParse(strArray5[2], out result1))
                  {
                    if (int.TryParse(strArray5[3], out result2))
                      goto label_51;
                  }
                  str2 = (string) null;
                }
                else
                  str2 = (string) null;
              }
              catch (Exception ex)
              {
                str2 = (string) null;
              }
            }
label_51:
            if (str2 == null)
            {
              locationName = targetLocationName;
              result1 = x;
              result2 = y;
            }
          }
          index6 = index5 + 1;
          Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions");
          string str3 = this.name.Value.ToLower() + "_sleep";
          string key = str3;
          if (dictionary.ContainsKey(key))
            endBehavior = str3;
        }
        else
        {
          if (int.TryParse(locationName, out int _))
          {
            locationName = startingLocation;
            --index5;
          }
          int index7 = index5 + 1;
          result1 = Convert.ToInt32(strArray3[index7]);
          int index8 = index7 + 1;
          result2 = Convert.ToInt32(strArray3[index8]);
          index6 = index8 + 1;
          try
          {
            if (strArray3.Length > index6)
            {
              if (int.TryParse(strArray3[index6], out result3))
                ++index6;
              else
                result3 = 2;
            }
          }
          catch (Exception ex)
          {
            result3 = 2;
          }
        }
        if (this.changeScheduleForLocationAccessibility(ref locationName, ref result1, ref result2, ref result3))
          return this.getMasterScheduleRawData().ContainsKey("default") ? this.parseMasterSchedule(this.getMasterScheduleEntry("default")) : this.parseMasterSchedule(this.getMasterScheduleEntry("spring"));
        if (index6 < strArray3.Length)
        {
          if (strArray3[index6].Length > 0 && strArray3[index6][0] == '"')
          {
            endMessage = strArray1[index3].Substring(strArray1[index3].IndexOf('"'));
          }
          else
          {
            endBehavior = strArray3[index6];
            int index9 = index6 + 1;
            if (index9 < strArray3.Length && strArray3[index9].Length > 0 && strArray3[index9][0] == '"')
              endMessage = strArray1[index3].Substring(strArray1[index3].IndexOf('"')).Replace("\"", "");
          }
        }
        if (num1 == 0)
        {
          flag1 = true;
          targetLocationName = locationName;
          x = result1;
          y = result2;
          startingLocation = locationName;
          point1.X = result1;
          point1.Y = result2;
          this.previousEndPoint = new Point(result1, result2);
        }
        else
        {
          SchedulePathDescription scheduleLocation = this.pathfindToNextScheduleLocation(startingLocation, point1.X, point1.Y, locationName, result1, result2, result3, endBehavior, endMessage);
          if (flag2)
          {
            int num2 = 0;
            Point? nullable = new Point?();
            foreach (Point point2 in scheduleLocation.route)
            {
              if (!nullable.HasValue)
              {
                nullable = new Point?(point2);
              }
              else
              {
                if (Math.Abs(nullable.Value.X - point2.X) + Math.Abs(nullable.Value.Y - point2.Y) == 1)
                  num2 += 64;
                nullable = new Point?(point2);
              }
            }
            int num3 = (int) Math.Round((double) (num2 / 2) / 420.0) * 10;
            num1 = Math.Max(Utility.ConvertMinutesToTime(Utility.ConvertTimeToMinutes(num1) - num3), val2);
          }
          masterSchedule.Add(num1, scheduleLocation);
          point1.X = result1;
          point1.Y = result2;
          startingLocation = locationName;
          val2 = num1;
        }
      }
      if (Game1.IsMasterGame & flag1)
        Game1.warpCharacter(this, targetLocationName, new Point(x, y));
      if (this._lastLoadedScheduleKey != null && Game1.IsMasterGame)
        this.dayScheduleName.Value = this._lastLoadedScheduleKey;
      return masterSchedule;
    }

    public Dictionary<int, SchedulePathDescription> getSchedule(
      int dayOfMonth)
    {
      if (!this.Name.Equals("Robin") || Game1.player.currentUpgrade != null)
        this.IsInvisible = false;
      if (this.Name.Equals("Willy") && Game1.stats.DaysPlayed < 2U || this.daysUntilNotInvisible > 0)
        this.IsInvisible = true;
      else if (this.Schedule != null)
        this.followSchedule = true;
      if (this.getMasterScheduleRawData() == null)
        return (Dictionary<int, SchedulePathDescription>) null;
      if ((NetFieldBase<string, NetString>) this.islandScheduleName != (NetString) null && this.islandScheduleName.Value != null && this.islandScheduleName.Value != "")
      {
        this.nameOfTodaysSchedule = this.islandScheduleName.Value;
        return this.Schedule;
      }
      if (this.isMarried())
      {
        if (this.hasMasterScheduleEntry("marriage_" + Game1.currentSeason + "_" + Game1.dayOfMonth.ToString()))
        {
          this.nameOfTodaysSchedule = "marriage_" + Game1.currentSeason + "_" + Game1.dayOfMonth.ToString();
          return this.parseMasterSchedule(this.getMasterScheduleEntry("marriage_" + Game1.currentSeason + "_" + Game1.dayOfMonth.ToString()));
        }
        string str = Game1.shortDayNameFromDayOfSeason(dayOfMonth);
        if (this.Name.Equals("Penny") && (str.Equals("Tue") || str.Equals("Wed") || str.Equals("Fri")) || this.Name.Equals("Maru") && (str.Equals("Tue") || str.Equals("Thu")) || this.Name.Equals("Harvey") && (str.Equals("Tue") || str.Equals("Thu")))
        {
          this.nameOfTodaysSchedule = "marriageJob";
          return this.parseMasterSchedule(this.getMasterScheduleEntry("marriageJob"));
        }
        if (!Game1.isRaining && this.hasMasterScheduleEntry("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
        {
          this.nameOfTodaysSchedule = "marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
          return this.parseMasterSchedule(this.getMasterScheduleEntry("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)));
        }
        this.followSchedule = false;
        return (Dictionary<int, SchedulePathDescription>) null;
      }
      if (this.hasMasterScheduleEntry(Game1.currentSeason + "_" + Game1.dayOfMonth.ToString()))
        return this.parseMasterSchedule(this.getMasterScheduleEntry(Game1.currentSeason + "_" + Game1.dayOfMonth.ToString()));
      int playerFriendshipLevel = Utility.GetAllPlayerFriendshipLevel(this);
      if (playerFriendshipLevel >= 0)
        playerFriendshipLevel /= 250;
      for (; playerFriendshipLevel > 0; --playerFriendshipLevel)
      {
        if (this.hasMasterScheduleEntry(Game1.dayOfMonth.ToString() + "_" + playerFriendshipLevel.ToString()))
          return this.parseMasterSchedule(this.getMasterScheduleEntry(Game1.dayOfMonth.ToString() + "_" + playerFriendshipLevel.ToString()));
      }
      if (this.hasMasterScheduleEntry(string.Empty + Game1.dayOfMonth.ToString()))
        return this.parseMasterSchedule(this.getMasterScheduleEntry(string.Empty + Game1.dayOfMonth.ToString()));
      if (this.Name.Equals("Pam") && Game1.player.mailReceived.Contains("ccVault"))
        return this.parseMasterSchedule(this.getMasterScheduleEntry("bus"));
      if (Game1.IsRainingHere(this.currentLocation))
      {
        if (Game1.random.NextDouble() < 0.5 && this.hasMasterScheduleEntry("rain2"))
          return this.parseMasterSchedule(this.getMasterScheduleEntry("rain2"));
        if (this.hasMasterScheduleEntry("rain"))
          return this.parseMasterSchedule(this.getMasterScheduleEntry("rain"));
      }
      List<string> values = new List<string>()
      {
        Game1.currentSeason,
        Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)
      };
      playerFriendshipLevel = Utility.GetAllPlayerFriendshipLevel(this);
      if (playerFriendshipLevel >= 0)
        playerFriendshipLevel /= 250;
      while (playerFriendshipLevel > 0)
      {
        values.Add(string.Empty + playerFriendshipLevel.ToString());
        if (this.hasMasterScheduleEntry(string.Join("_", (IEnumerable<string>) values)))
          return this.parseMasterSchedule(this.getMasterScheduleEntry(string.Join("_", (IEnumerable<string>) values)));
        --playerFriendshipLevel;
        values.RemoveAt(values.Count - 1);
      }
      if (this.hasMasterScheduleEntry(string.Join("_", (IEnumerable<string>) values)))
        return this.parseMasterSchedule(this.getMasterScheduleEntry(string.Join("_", (IEnumerable<string>) values)));
      if (this.hasMasterScheduleEntry(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
        return this.parseMasterSchedule(this.getMasterScheduleEntry(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)));
      if (this.hasMasterScheduleEntry(Game1.currentSeason))
        return this.parseMasterSchedule(this.getMasterScheduleEntry(Game1.currentSeason));
      if (this.hasMasterScheduleEntry("spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
        return this.parseMasterSchedule(this.getMasterScheduleEntry("spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)));
      values.RemoveAt(values.Count - 1);
      values.Add("spring");
      playerFriendshipLevel = Utility.GetAllPlayerFriendshipLevel(this);
      if (playerFriendshipLevel >= 0)
        playerFriendshipLevel /= 250;
      while (playerFriendshipLevel > 0)
      {
        values.Add(string.Empty + playerFriendshipLevel.ToString());
        if (this.hasMasterScheduleEntry(string.Join("_", (IEnumerable<string>) values)))
          return this.parseMasterSchedule(this.getMasterScheduleEntry(string.Join("_", (IEnumerable<string>) values)));
        --playerFriendshipLevel;
        values.RemoveAt(values.Count - 1);
      }
      return this.hasMasterScheduleEntry("spring") ? this.parseMasterSchedule(this.getMasterScheduleEntry("spring")) : (Dictionary<int, SchedulePathDescription>) null;
    }

    public virtual void handleMasterScheduleFileLoadError(Exception e)
    {
    }

    public virtual void InvalidateMasterSchedule() => this._hasLoadedMasterScheduleData = false;

    public Dictionary<string, string> getMasterScheduleRawData()
    {
      if (!this._hasLoadedMasterScheduleData)
      {
        this._hasLoadedMasterScheduleData = true;
        try
        {
          this._masterScheduleData = !(this.Name == "Leo") ? Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + this.Name) : (!(this.DefaultMap == "IslandHut") ? Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + this.Name + "Mainland") : Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + this.Name));
        }
        catch (Exception ex)
        {
          this.handleMasterScheduleFileLoadError(ex);
        }
      }
      return this._masterScheduleData;
    }

    public string getMasterScheduleEntry(string schedule_key)
    {
      if (this.getMasterScheduleRawData() == null)
        throw new KeyNotFoundException("The schedule file for NPC '" + this.Name + "' could not be loaded...");
      string masterScheduleEntry;
      this._lastLoadedScheduleKey = this._masterScheduleData.TryGetValue(schedule_key, out masterScheduleEntry) ? schedule_key : throw new KeyNotFoundException("The schedule file for NPC '" + this.Name + "' has no schedule named '" + schedule_key + "'.");
      return masterScheduleEntry;
    }

    public bool hasMasterScheduleEntry(string key) => this.getMasterScheduleRawData() != null && this.getMasterScheduleRawData().ContainsKey(key);

    public virtual bool isRoommate()
    {
      if (!this.isVillager())
        return false;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.spouse != null && allFarmer.spouse.Equals(this.Name) && !allFarmer.isEngaged() && allFarmer.isRoommate(this.Name))
          return true;
      }
      return false;
    }

    public bool isMarried()
    {
      if (!this.isVillager())
        return false;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.spouse != null && allFarmer.spouse.Equals(this.Name) && !allFarmer.isEngaged())
          return true;
      }
      return false;
    }

    public bool isMarriedOrEngaged()
    {
      if (!this.isVillager())
        return false;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.spouse != null && allFarmer.spouse.Equals(this.Name))
          return true;
      }
      return false;
    }

    public virtual void dayUpdate(int dayOfMonth)
    {
      this.isMovingOnPathFindPath.Value = false;
      this.queuedSchedulePaths.Clear();
      this.lastAttemptedSchedule = -1;
      if (this.layingDown)
      {
        this.layingDown = false;
        this.HideShadow = false;
      }
      this.exploreFarm.Value = false;
      this.shouldWearIslandAttire.Value = false;
      if (this.isWearingIslandAttire)
        this.wearNormalClothes();
      this.layingDown = false;
      if (this.appliedRouteAnimationOffset != Vector2.Zero)
      {
        this.drawOffset.Value = Vector2.Zero;
        this.appliedRouteAnimationOffset = Vector2.Zero;
      }
      if (this.currentLocation != null && this.defaultMap.Value != null)
        Game1.warpCharacter(this, (string) (NetFieldBase<string, NetString>) this.defaultMap, this.defaultPosition.Value / 64f);
      if (this.Name.Equals("Maru") || this.Name.Equals("Shane"))
        this.Sprite.LoadTexture("Characters\\" + NPC.getTextureNameForCharacter(this.Name));
      if (this.Name.Equals("Willy") || this.Name.Equals("Clint"))
      {
        this.Sprite.SpriteWidth = 16;
        this.Sprite.SpriteHeight = 32;
        this.Sprite.ignoreSourceRectUpdates = false;
        this.Sprite.UpdateSourceRect();
        this.IsInvisible = false;
      }
      if (Game1.IsMasterGame && this.Name.Equals("Elliott") && Game1.netWorldState.Value.hasWorldStateID("elliottGone"))
      {
        this.daysUntilNotInvisible = 7;
        Game1.netWorldState.Value.removeWorldStateID("elliottGone");
        Game1.worldStateIDs.Remove("elliottGone");
      }
      this.drawOffset.Value = Vector2.Zero;
      if (Game1.IsMasterGame && this.daysUntilNotInvisible > 0)
      {
        this.IsInvisible = true;
        --this.daysUntilNotInvisible;
        if (this.daysUntilNotInvisible <= 0)
          this.IsInvisible = false;
      }
      this.resetForNewDay(dayOfMonth);
      this.updateConstructionAnimation();
      if (!this.isMarried() || (bool) (NetFieldBase<bool, NetBool>) this.getSpouse().divorceTonight || this.IsInvisible)
        return;
      this.marriageDuties();
    }

    public virtual void resetForNewDay(int dayOfMonth)
    {
      this.sleptInBed.Value = true;
      if (this.isMarried() && !this.isRoommate())
      {
        FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(this.getSpouse());
        if (homeOfFarmer != null && homeOfFarmer.GetSpouseBed() == null)
          this.sleptInBed.Value = false;
      }
      this.doingEndOfRouteAnimation.Value = false;
      this.Halt();
      this.hasBeenKissedToday.Value = false;
      this.currentMarriageDialogue.Clear();
      this.marriageDefaultDialogue.Value = (MarriageDialogueReference) null;
      this.shouldSayMarriageDialogue.Value = false;
      this.isSleeping.Value = false;
      this.drawOffset.Value = Vector2.Zero;
      this.faceTowardFarmer = false;
      this.faceTowardFarmerTimer = 0;
      this.drawOffset.Value = Vector2.Zero;
      this.hasSaidAfternoonDialogue.Value = false;
      this.isPlayingSleepingAnimation = false;
      this.ignoreScheduleToday = false;
      this.Halt();
      this.controller = (PathFindController) null;
      this.temporaryController = (PathFindController) null;
      this.directionsToNewLocation = (SchedulePathDescription) null;
      this.faceDirection(this.DefaultFacingDirection);
      this.previousEndPoint = new Point((int) this.defaultPosition.X / 64, (int) this.defaultPosition.Y / 64);
      this.isWalkingInSquare = false;
      this.returningToEndPoint = false;
      this.lastCrossroad = Microsoft.Xna.Framework.Rectangle.Empty;
      this._startedEndOfRouteBehavior = (string) null;
      this._finishingEndOfRouteBehavior = (string) null;
      if (this.isVillager())
        this.Schedule = this.getSchedule(dayOfMonth);
      this.endOfRouteMessage.Value = (string) null;
    }

    public void returnHomeFromFarmPosition(Farm farm)
    {
      Farmer spouse = this.getSpouse();
      if (spouse == null)
        return;
      FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(spouse);
      Point porchStandingSpot = homeOfFarmer.getPorchStandingSpot();
      if (this.exploreFarm.Value)
      {
        this.drawOffset.Value = Vector2.Zero;
        GameLocation home = this.getHome();
        string name = home.Name;
        if (home.uniqueName.Value != null)
          name = home.uniqueName.Value;
        this.willDestroyObjectsUnderfoot = true;
        Point warpPointTo = farm.getWarpPointTo(name, (Character) this);
        Game1.player.getSpouse().PathToOnFarm(warpPointTo);
      }
      else if (this.getTileLocationPoint().Equals(porchStandingSpot))
      {
        this.drawOffset.Value = Vector2.Zero;
        GameLocation home = this.getHome();
        string name = home.Name;
        if (home.uniqueName.Value != null)
          name = home.uniqueName.Value;
        this.willDestroyObjectsUnderfoot = true;
        Point warpPointTo = farm.getWarpPointTo(name, (Character) this);
        this.controller = new PathFindController((Character) this, (GameLocation) farm, warpPointTo, 0)
        {
          NPCSchedule = true
        };
      }
      else
      {
        if (this.shouldPlaySpousePatioAnimation.Value && farm.farmers.Any())
          return;
        this.drawOffset.Value = Vector2.Zero;
        this.Halt();
        this.controller = (PathFindController) null;
        this.temporaryController = (PathFindController) null;
        this.ignoreScheduleToday = true;
        Game1.warpCharacter(this, (GameLocation) homeOfFarmer, Utility.PointToVector2(homeOfFarmer.getKitchenStandingSpot()));
      }
    }

    public virtual Vector2 GetSpousePatioPosition() => Utility.PointToVector2(Game1.getFarm().spousePatioSpot);

    public void setUpForOutdoorPatioActivity()
    {
      Vector2 spousePatioPosition = this.GetSpousePatioPosition();
      if (NPC.checkTileOccupancyForSpouse((GameLocation) Game1.getFarm(), spousePatioPosition))
        return;
      Game1.warpCharacter(this, "Farm", spousePatioPosition);
      this.popOffAnyNonEssentialItems();
      this.currentMarriageDialogue.Clear();
      this.addMarriageDialogue("MarriageDialogue", "patio_" + this.Name, false);
      this.setTilePosition((int) spousePatioPosition.X, (int) spousePatioPosition.Y);
      this.shouldPlaySpousePatioAnimation.Value = true;
    }

    private void doPlaySpousePatioAnimation()
    {
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\SpousePatios");
      if (dictionary != null && dictionary.ContainsKey(this.Name))
      {
        string[] strArray1 = dictionary[this.Name].Split('/');
        if (strArray1.Length > 2)
        {
          int[] stringToIntArray = Utility.parseStringToIntArray(strArray1[2]);
          Point zero = Point.Zero;
          if (strArray1.Length > 3)
          {
            string[] strArray2 = strArray1[3].Split(' ');
            if (strArray2.Length > 1)
            {
              zero.X = int.Parse(strArray2[0]) * 4;
              zero.Y = int.Parse(strArray2[1]) * 4;
            }
          }
          this.drawOffset.Value = Utility.PointToVector2(zero);
          List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>();
          for (int index = 0; index < stringToIntArray.Length; ++index)
            animation.Add(new FarmerSprite.AnimationFrame(stringToIntArray[index], 100, 0, false, false));
          this.Sprite.setCurrentAnimation(animation);
          return;
        }
      }
      string name = (string) (NetFieldBase<string, NetString>) this.name;
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
      {
        case 161540545:
          if (!(name == "Sebastian"))
            break;
          this.drawOffset.Value = new Vector2(16f, 40f);
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(32, 500, 64, false, false),
            new FarmerSprite.AnimationFrame(36, 500, 64, false, false),
            new FarmerSprite.AnimationFrame(32, 500, 64, false, false),
            new FarmerSprite.AnimationFrame(36, 500, 64, false, false),
            new FarmerSprite.AnimationFrame(32, 500, 64, false, false),
            new FarmerSprite.AnimationFrame(36, 500, 64, false, false),
            new FarmerSprite.AnimationFrame(32, 500, 64, false, false),
            new FarmerSprite.AnimationFrame(36, 2000, 64, false, false),
            new FarmerSprite.AnimationFrame(33, 100, 64, false, false),
            new FarmerSprite.AnimationFrame(34, 100, 64, false, false),
            new FarmerSprite.AnimationFrame(35, 3000, 64, false, false),
            new FarmerSprite.AnimationFrame(34, 100, 64, false, false),
            new FarmerSprite.AnimationFrame(33, 100, 64, false, false),
            new FarmerSprite.AnimationFrame(32, 1500, 64, false, false)
          });
          break;
        case 587846041:
          if (!(name == "Penny"))
            break;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(18, 6000),
            new FarmerSprite.AnimationFrame(19, 500)
          });
          break;
        case 1067922812:
          if (!(name == "Sam"))
            break;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(25, 3000),
            new FarmerSprite.AnimationFrame(27, 500),
            new FarmerSprite.AnimationFrame(26, 100),
            new FarmerSprite.AnimationFrame(28, 100),
            new FarmerSprite.AnimationFrame(27, 500),
            new FarmerSprite.AnimationFrame(25, 2000),
            new FarmerSprite.AnimationFrame(27, 500),
            new FarmerSprite.AnimationFrame(26, 100),
            new FarmerSprite.AnimationFrame(29, 100),
            new FarmerSprite.AnimationFrame(30, 100),
            new FarmerSprite.AnimationFrame(32, 500),
            new FarmerSprite.AnimationFrame(31, 1000),
            new FarmerSprite.AnimationFrame(30, 100),
            new FarmerSprite.AnimationFrame(29, 100)
          });
          break;
        case 1281010426:
          if (!(name == "Maru"))
            break;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(16, 4000),
            new FarmerSprite.AnimationFrame(17, 200),
            new FarmerSprite.AnimationFrame(18, 200),
            new FarmerSprite.AnimationFrame(19, 200),
            new FarmerSprite.AnimationFrame(20, 200),
            new FarmerSprite.AnimationFrame(21, 200),
            new FarmerSprite.AnimationFrame(22, 200),
            new FarmerSprite.AnimationFrame(23, 200)
          });
          break;
        case 1708213605:
          if (!(name == "Alex"))
            break;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(34, 4000),
            new FarmerSprite.AnimationFrame(33, 300),
            new FarmerSprite.AnimationFrame(28, 200),
            new FarmerSprite.AnimationFrame(29, 100),
            new FarmerSprite.AnimationFrame(30, 100),
            new FarmerSprite.AnimationFrame(31, 100),
            new FarmerSprite.AnimationFrame(32, 100),
            new FarmerSprite.AnimationFrame(31, 100),
            new FarmerSprite.AnimationFrame(30, 100),
            new FarmerSprite.AnimationFrame(29, 100),
            new FarmerSprite.AnimationFrame(28, 800),
            new FarmerSprite.AnimationFrame(29, 100),
            new FarmerSprite.AnimationFrame(30, 100),
            new FarmerSprite.AnimationFrame(31, 100),
            new FarmerSprite.AnimationFrame(32, 100),
            new FarmerSprite.AnimationFrame(31, 100),
            new FarmerSprite.AnimationFrame(30, 100),
            new FarmerSprite.AnimationFrame(29, 100),
            new FarmerSprite.AnimationFrame(28, 800),
            new FarmerSprite.AnimationFrame(33, 200)
          });
          break;
        case 1866496948:
          if (!(name == "Shane"))
            break;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(28, 4000, 64, false, false),
            new FarmerSprite.AnimationFrame(29, 800, 64, false, false)
          });
          break;
        case 2010304804:
          if (!(name == "Harvey"))
            break;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(42, 6000),
            new FarmerSprite.AnimationFrame(43, 1000),
            new FarmerSprite.AnimationFrame(39, 100),
            new FarmerSprite.AnimationFrame(43, 500),
            new FarmerSprite.AnimationFrame(39, 100),
            new FarmerSprite.AnimationFrame(43, 1000),
            new FarmerSprite.AnimationFrame(42, 5000),
            new FarmerSprite.AnimationFrame(43, 3000)
          });
          break;
        case 2434294092:
          if (!(name == "Haley"))
            break;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(30, 2000),
            new FarmerSprite.AnimationFrame(31, 200),
            new FarmerSprite.AnimationFrame(24, 2000),
            new FarmerSprite.AnimationFrame(25, 1000),
            new FarmerSprite.AnimationFrame(32, 200),
            new FarmerSprite.AnimationFrame(33, 2000),
            new FarmerSprite.AnimationFrame(32, 200),
            new FarmerSprite.AnimationFrame(25, 2000),
            new FarmerSprite.AnimationFrame(32, 200),
            new FarmerSprite.AnimationFrame(33, 2000)
          });
          break;
        case 2571828641:
          if (!(name == "Emily"))
            break;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(54, 4000, 64, false, false)
          });
          break;
        case 2732913340:
          if (!(name == "Abigail"))
            break;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(16, 500),
            new FarmerSprite.AnimationFrame(17, 500),
            new FarmerSprite.AnimationFrame(18, 500),
            new FarmerSprite.AnimationFrame(19, 500)
          });
          break;
        case 2826247323:
          if (!(name == "Leah"))
            break;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(16, 100),
            new FarmerSprite.AnimationFrame(17, 100),
            new FarmerSprite.AnimationFrame(18, 100),
            new FarmerSprite.AnimationFrame(19, 300),
            new FarmerSprite.AnimationFrame(16, 100),
            new FarmerSprite.AnimationFrame(17, 100),
            new FarmerSprite.AnimationFrame(18, 100),
            new FarmerSprite.AnimationFrame(19, 1000),
            new FarmerSprite.AnimationFrame(16, 100),
            new FarmerSprite.AnimationFrame(17, 100),
            new FarmerSprite.AnimationFrame(18, 100),
            new FarmerSprite.AnimationFrame(19, 300),
            new FarmerSprite.AnimationFrame(16, 100),
            new FarmerSprite.AnimationFrame(17, 100),
            new FarmerSprite.AnimationFrame(18, 100),
            new FarmerSprite.AnimationFrame(19, 300),
            new FarmerSprite.AnimationFrame(16, 100),
            new FarmerSprite.AnimationFrame(17, 100),
            new FarmerSprite.AnimationFrame(18, 100),
            new FarmerSprite.AnimationFrame(19, 2000)
          });
          break;
        case 3066176300:
          if (!(name == "Elliott"))
            break;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(33, 3000),
            new FarmerSprite.AnimationFrame(32, 500),
            new FarmerSprite.AnimationFrame(33, 3000),
            new FarmerSprite.AnimationFrame(32, 500),
            new FarmerSprite.AnimationFrame(33, 2000),
            new FarmerSprite.AnimationFrame(34, 1500)
          });
          break;
      }
    }

    public bool isGaySpouse()
    {
      Farmer spouse = this.getSpouse();
      if (spouse == null)
        return false;
      if (this.Gender == 0 && spouse.IsMale)
        return true;
      return this.Gender == 1 && !spouse.IsMale;
    }

    public bool canGetPregnant()
    {
      if (this is Horse || this.Name.Equals("Krobus") || this.isRoommate())
        return false;
      Farmer spouse = this.getSpouse();
      if (spouse == null || (bool) (NetFieldBase<bool, NetBool>) spouse.divorceTonight)
        return false;
      int heartLevelForNpc = spouse.getFriendshipHeartLevelForNPC(this.Name);
      Friendship spouseFriendship = spouse.GetSpouseFriendship();
      List<Child> children = spouse.getChildren();
      this.defaultMap.Value = spouse.homeLocation.Value;
      FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(spouse);
      if (homeOfFarmer.cribStyle.Value <= 0 || homeOfFarmer.upgradeLevel < 2 || spouseFriendship.DaysUntilBirthing >= 0 || heartLevelForNpc < 10 || spouse.GetDaysMarried() < 7)
        return false;
      if (children.Count == 0)
        return true;
      return children.Count < 2 && children[0].Age > 2;
    }

    public void marriageDuties()
    {
      if (!Game1.newDay && Game1.gameMode != (byte) 6)
        return;
      Farmer spouse = this.getSpouse();
      if (spouse == null)
        return;
      this.shouldSayMarriageDialogue.Value = true;
      this.DefaultMap = spouse.homeLocation.Value;
      FarmHouse locationFromName = Game1.getLocationFromName(spouse.homeLocation.Value) as FarmHouse;
      Random r = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 + (int) spouse.UniqueMultiplayerID);
      int heartLevelForNpc = spouse.getFriendshipHeartLevelForNPC(this.Name);
      if (Game1.IsMasterGame && (this.currentLocation == null || !this.currentLocation.Equals((GameLocation) locationFromName)))
        Game1.warpCharacter(this, spouse.homeLocation.Value, locationFromName.getSpouseBedSpot(this.Name));
      int num1;
      if (Game1.isRaining)
      {
        NetRef<MarriageDialogueReference> marriageDefaultDialogue = this.marriageDefaultDialogue;
        num1 = r.Next(5);
        MarriageDialogueReference dialogueReference = new MarriageDialogueReference("MarriageDialogue", "Rainy_Day_" + num1.ToString(), false, Array.Empty<string>());
        marriageDefaultDialogue.Value = dialogueReference;
      }
      else
      {
        NetRef<MarriageDialogueReference> marriageDefaultDialogue = this.marriageDefaultDialogue;
        num1 = r.Next(5);
        MarriageDialogueReference dialogueReference = new MarriageDialogueReference("MarriageDialogue", "Indoor_Day_" + num1.ToString(), false, Array.Empty<string>());
        marriageDefaultDialogue.Value = dialogueReference;
      }
      this.currentMarriageDialogue.Add(new MarriageDialogueReference(this.marriageDefaultDialogue.Value.DialogueFile, this.marriageDefaultDialogue.Value.DialogueKey, this.marriageDefaultDialogue.Value.IsGendered, this.marriageDefaultDialogue.Value.Substitutions));
      if (spouse.GetSpouseFriendship().DaysUntilBirthing == 0)
      {
        this.setTilePosition(locationFromName.getKitchenStandingSpot());
        this.currentMarriageDialogue.Clear();
      }
      else
      {
        if (this.daysAfterLastBirth >= 0)
        {
          --this.daysAfterLastBirth;
          switch (this.getSpouse().getChildrenCount())
          {
            case 1:
              this.setTilePosition(locationFromName.getKitchenStandingSpot());
              if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4406", false, Array.Empty<string>()), (GameLocation) locationFromName))
                return;
              this.currentMarriageDialogue.Clear();
              num1 = r.Next(4);
              this.addMarriageDialogue("MarriageDialogue", "OneKid_" + num1.ToString(), false);
              return;
            case 2:
              this.setTilePosition(locationFromName.getKitchenStandingSpot());
              if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4406", false, Array.Empty<string>()), (GameLocation) locationFromName))
                return;
              this.currentMarriageDialogue.Clear();
              num1 = r.Next(4);
              this.addMarriageDialogue("MarriageDialogue", "TwoKids_" + num1.ToString(), false);
              return;
          }
        }
        this.setTilePosition(locationFromName.getKitchenStandingSpot());
        if (!this.sleptInBed.Value)
        {
          this.currentMarriageDialogue.Clear();
          num1 = r.Next(4);
          this.addMarriageDialogue("MarriageDialogue", "NoBed_" + num1.ToString(), false);
        }
        else if (this.tryToGetMarriageSpecificDialogueElseReturnDefault(Game1.currentSeason + "_" + Game1.dayOfMonth.ToString()).Length > 0)
        {
          if (spouse == null)
            return;
          this.currentMarriageDialogue.Clear();
          this.addMarriageDialogue("MarriageDialogue", Game1.currentSeason + "_" + Game1.dayOfMonth.ToString(), false);
        }
        else if (this.schedule != null)
        {
          if (this.nameOfTodaysSchedule.Equals("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
          {
            this.currentMarriageDialogue.Clear();
            this.addMarriageDialogue("MarriageDialogue", "funLeave_" + this.Name, false);
          }
          else
          {
            if (!this.nameOfTodaysSchedule.Equals("marriageJob"))
              return;
            this.currentMarriageDialogue.Clear();
            this.addMarriageDialogue("MarriageDialogue", "jobLeave_" + this.Name, false);
          }
        }
        else if (!Game1.isRaining && !Game1.IsWinter && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Sat") && spouse == Game1.MasterPlayer && !this.Name.Equals("Krobus"))
          this.setUpForOutdoorPatioActivity();
        else if (spouse.GetDaysMarried() >= 1 && r.NextDouble() < 1.0 - (double) Math.Max(1, heartLevelForNpc) / 12.0)
        {
          Furniture randomFurniture = locationFromName.getRandomFurniture(r);
          if (randomFurniture != null && randomFurniture.isGroundFurniture() && randomFurniture.furniture_type.Value != 15 && randomFurniture.furniture_type.Value != 12)
          {
            Point p = new Point((int) randomFurniture.tileLocation.X - 1, (int) randomFurniture.tileLocation.Y);
            if (locationFromName.isTileLocationTotallyClearAndPlaceable(p.X, p.Y))
            {
              this.setTilePosition(p);
              this.faceDirection(1);
              num1 = r.Next(10);
              switch (num1)
              {
                case 0:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4420", false);
                  return;
                case 1:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4421", false);
                  return;
                case 2:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4422", true);
                  return;
                case 3:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4423", false);
                  return;
                case 4:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4424", false);
                  return;
                case 5:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4425", false);
                  return;
                case 6:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4426", false);
                  return;
                case 7:
                  if (this.Gender == 1)
                  {
                    if (r.NextDouble() < 0.5)
                    {
                      this.currentMarriageDialogue.Clear();
                      this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4427", false);
                      return;
                    }
                    this.currentMarriageDialogue.Clear();
                    this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4429", false);
                    return;
                  }
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4431", false);
                  return;
                case 8:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4432", false);
                  return;
                case 9:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4433", false);
                  return;
                default:
                  return;
              }
            }
          }
          num1 = r.Next(5);
          switch (num1)
          {
            case 0:
              MarriageDialogueReference dialogueReference1 = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4434", false, Array.Empty<string>());
              break;
            case 1:
              MarriageDialogueReference dialogueReference2 = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4435", false, Array.Empty<string>());
              break;
            case 2:
              MarriageDialogueReference dialogueReference3 = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4436", false, Array.Empty<string>());
              break;
            case 3:
              MarriageDialogueReference dialogueReference4 = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4437", true, Array.Empty<string>());
              break;
            case 4:
              MarriageDialogueReference dialogueReference5 = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4438", false, Array.Empty<string>());
              break;
          }
          this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4406", false, Array.Empty<string>()), (GameLocation) locationFromName, true);
        }
        else
        {
          Friendship spouseFriendship = spouse.GetSpouseFriendship();
          if (spouseFriendship.DaysUntilBirthing != -1 && spouseFriendship.DaysUntilBirthing <= 7 && r.NextDouble() < 0.5)
          {
            if (this.isGaySpouse())
            {
              this.setTilePosition(locationFromName.getKitchenStandingSpot());
              if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4439", false, Array.Empty<string>()), (GameLocation) locationFromName))
                return;
              if (r.NextDouble() < 0.5)
                this.currentMarriageDialogue.Clear();
              if (r.NextDouble() < 0.5)
                this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4440", false, this.getSpouse().displayName);
              else
                this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4441", false, "%endearment");
            }
            else if (this.Gender == 1)
            {
              this.setTilePosition(locationFromName.getKitchenStandingSpot());
              if (this.spouseObstacleCheck(r.NextDouble() < 0.5 ? new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4442", false, Array.Empty<string>()) : new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4443", false, Array.Empty<string>()), (GameLocation) locationFromName))
                return;
              if (r.NextDouble() < 0.5)
                this.currentMarriageDialogue.Clear();
              NetList<MarriageDialogueReference, NetRef<MarriageDialogueReference>> marriageDialogue = this.currentMarriageDialogue;
              MarriageDialogueReference dialogueReference;
              if (r.NextDouble() >= 0.5)
                dialogueReference = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4445", false, new string[1]
                {
                  "%endearment"
                });
              else
                dialogueReference = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4444", false, new string[1]
                {
                  this.getSpouse().displayName
                });
              marriageDialogue.Add(dialogueReference);
            }
            else
            {
              this.setTilePosition(locationFromName.getKitchenStandingSpot());
              if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4446", true, Array.Empty<string>()), (GameLocation) locationFromName))
                return;
              if (r.NextDouble() < 0.5)
                this.currentMarriageDialogue.Clear();
              NetList<MarriageDialogueReference, NetRef<MarriageDialogueReference>> marriageDialogue = this.currentMarriageDialogue;
              MarriageDialogueReference dialogueReference;
              if (r.NextDouble() >= 0.5)
                dialogueReference = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4448", false, new string[1]
                {
                  "%endearment"
                });
              else
                dialogueReference = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4447", true, new string[1]
                {
                  this.getSpouse().displayName
                });
              marriageDialogue.Add(dialogueReference);
            }
          }
          else
          {
            if (r.NextDouble() < 0.07)
            {
              switch (this.getSpouse().getChildrenCount())
              {
                case 1:
                  this.setTilePosition(locationFromName.getKitchenStandingSpot());
                  if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4449", true, Array.Empty<string>()), (GameLocation) locationFromName))
                    return;
                  this.currentMarriageDialogue.Clear();
                  num1 = r.Next(4);
                  this.addMarriageDialogue("MarriageDialogue", "OneKid_" + num1.ToString(), false);
                  return;
                case 2:
                  this.setTilePosition(locationFromName.getKitchenStandingSpot());
                  if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4452", true, Array.Empty<string>()), (GameLocation) locationFromName))
                    return;
                  this.currentMarriageDialogue.Clear();
                  num1 = r.Next(4);
                  this.addMarriageDialogue("MarriageDialogue", "TwoKids_" + num1.ToString(), false);
                  return;
              }
            }
            Farm farm = Game1.getFarm();
            if (this.currentMarriageDialogue.Count > 0 && this.currentMarriageDialogue[0].IsItemGrabDialogue(this))
            {
              this.setTilePosition(locationFromName.getKitchenStandingSpot());
              this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4455", true, Array.Empty<string>()), (GameLocation) locationFromName);
            }
            else if (!Game1.isRaining && r.NextDouble() < 0.4 && !NPC.checkTileOccupancyForSpouse((GameLocation) farm, Utility.PointToVector2(locationFromName.getPorchStandingSpot())) && !this.Name.Equals("Krobus"))
            {
              bool flag1 = false;
              if (!farm.petBowlWatered.Value && !NPC.hasSomeoneFedThePet)
              {
                flag1 = true;
                farm.petBowlWatered.Set(true);
                NPC.hasSomeoneFedThePet = true;
              }
              if (r.NextDouble() < 0.6 && !Game1.currentSeason.Equals("winter") && !NPC.hasSomeoneWateredCrops)
              {
                Vector2 vector2_1 = Vector2.Zero;
                int num2 = 0;
                bool flag2 = false;
                for (; num2 < Math.Min(50, farm.terrainFeatures.Count()) && vector2_1.Equals(Vector2.Zero); ++num2)
                {
                  int index = r.Next(farm.terrainFeatures.Count());
                  NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.PairsCollection pairs = farm.terrainFeatures.Pairs;
                  KeyValuePair<Vector2, TerrainFeature> keyValuePair = pairs.ElementAt(index);
                  if (keyValuePair.Value is HoeDirt)
                  {
                    pairs = farm.terrainFeatures.Pairs;
                    keyValuePair = pairs.ElementAt(index);
                    if ((keyValuePair.Value as HoeDirt).needsWatering())
                    {
                      pairs = farm.terrainFeatures.Pairs;
                      keyValuePair = pairs.ElementAt(index);
                      vector2_1 = keyValuePair.Key;
                    }
                    else
                    {
                      pairs = farm.terrainFeatures.Pairs;
                      keyValuePair = pairs.ElementAt(index);
                      if ((keyValuePair.Value as HoeDirt).crop != null)
                        flag2 = true;
                    }
                  }
                }
                if (!vector2_1.Equals(Vector2.Zero))
                {
                  Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) vector2_1.X - 30, (int) vector2_1.Y - 30, 60, 60);
                  Vector2 vector2_2 = new Vector2();
                  for (int x = rectangle.X; x < rectangle.Right; ++x)
                  {
                    for (int y = rectangle.Y; y < rectangle.Bottom; ++y)
                    {
                      vector2_2.X = (float) x;
                      vector2_2.Y = (float) y;
                      if (farm.isTileOnMap(vector2_2) && farm.terrainFeatures.ContainsKey(vector2_2) && farm.terrainFeatures[vector2_2] is HoeDirt && Game1.IsMasterGame && (farm.terrainFeatures[vector2_2] as HoeDirt).needsWatering())
                        (farm.terrainFeatures[vector2_2] as HoeDirt).state.Value = 1;
                    }
                  }
                  this.faceDirection(2);
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4462", true);
                  if (flag1)
                    this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4463", false, Game1.player.getPetDisplayName());
                  num1 = r.Next(5);
                  this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + num1.ToString(), false);
                  NPC.hasSomeoneWateredCrops = true;
                }
                else
                {
                  this.faceDirection(2);
                  if (flag2)
                  {
                    this.currentMarriageDialogue.Clear();
                    if (Game1.gameMode == (byte) 6)
                    {
                      if (r.NextDouble() < 0.5)
                      {
                        this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4465", false, "%endearment");
                      }
                      else
                      {
                        this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4466", false, "%endearment");
                        this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4462", true);
                        if (flag1)
                          this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4463", false, Game1.player.getPetDisplayName());
                      }
                    }
                    else
                    {
                      this.currentMarriageDialogue.Clear();
                      this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4470", true);
                    }
                  }
                  else
                  {
                    this.currentMarriageDialogue.Clear();
                    num1 = r.Next(5);
                    this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + num1.ToString(), false);
                  }
                }
              }
              else if (r.NextDouble() < 0.6 && !NPC.hasSomeoneFedTheAnimals)
              {
                bool flag3 = false;
                foreach (Building building in farm.buildings)
                {
                  if ((building is Barn || building is Coop) && (int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0)
                  {
                    if (Game1.IsMasterGame)
                      (building.indoors.Value as AnimalHouse).feedAllAnimals();
                    flag3 = true;
                  }
                }
                this.faceDirection(2);
                if (flag3)
                {
                  NPC.hasSomeoneFedTheAnimals = true;
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4474", true);
                  if (flag1)
                    this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4463", false, Game1.player.getPetDisplayName());
                  this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + r.Next(5).ToString(), false);
                }
                else
                {
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + r.Next(5).ToString(), false);
                }
                if (Game1.IsMasterGame)
                  farm.petBowlWatered.Set(true);
              }
              else if (!NPC.hasSomeoneRepairedTheFences)
              {
                int num3 = 0;
                this.faceDirection(2);
                Vector2 vector2_3;
                for (vector2_3 = Vector2.Zero; num3 < Math.Min(50, farm.objects.Count()) && vector2_3.Equals(Vector2.Zero); ++num3)
                {
                  int index = r.Next(farm.objects.Count());
                  OverlaidDictionary.PairsCollection pairs = farm.objects.Pairs;
                  KeyValuePair<Vector2, Object> keyValuePair = pairs.ElementAt(index);
                  if (keyValuePair.Value is Fence)
                  {
                    pairs = farm.objects.Pairs;
                    keyValuePair = pairs.ElementAt(index);
                    vector2_3 = keyValuePair.Key;
                  }
                }
                if (!vector2_3.Equals(Vector2.Zero))
                {
                  Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) vector2_3.X - 10, (int) vector2_3.Y - 10, 20, 20);
                  Vector2 vector2_4 = new Vector2();
                  for (int x = rectangle.X; x < rectangle.Right; ++x)
                  {
                    for (int y = rectangle.Y; y < rectangle.Bottom; ++y)
                    {
                      vector2_4.X = (float) x;
                      vector2_4.Y = (float) y;
                      if (farm.isTileOnMap(vector2_4) && farm.objects.ContainsKey(vector2_4) && farm.objects[vector2_4] is Fence && Game1.IsMasterGame)
                        (farm.objects[vector2_4] as Fence).repair();
                    }
                  }
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4481", true);
                  if (flag1)
                    this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4463", false, Game1.player.getPetDisplayName());
                  num1 = r.Next(5);
                  this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + num1.ToString(), false);
                  NPC.hasSomeoneRepairedTheFences = true;
                }
                else
                {
                  this.currentMarriageDialogue.Clear();
                  num1 = r.Next(5);
                  this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + num1.ToString(), false);
                }
              }
              Game1.warpCharacter(this, "Farm", locationFromName.getPorchStandingSpot());
              this.popOffAnyNonEssentialItems();
              this.faceDirection(2);
            }
            else if (this.Name.Equals("Krobus") && Game1.isRaining && r.NextDouble() < 0.4 && !NPC.checkTileOccupancyForSpouse((GameLocation) farm, Utility.PointToVector2(locationFromName.getPorchStandingSpot())))
            {
              num1 = r.Next(5);
              this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + num1.ToString(), false);
              Game1.warpCharacter(this, "Farm", locationFromName.getPorchStandingSpot());
              this.popOffAnyNonEssentialItems();
              this.faceDirection(2);
            }
            else if (spouse.GetDaysMarried() >= 1 && r.NextDouble() < 0.045)
            {
              if (r.NextDouble() < 0.75)
              {
                Point openPointInHouse = locationFromName.getRandomOpenPointInHouse(r, 1);
                Furniture furniture;
                try
                {
                  furniture = new Furniture(Utility.getRandomSingleTileFurniture(r), new Vector2((float) openPointInHouse.X, (float) openPointInHouse.Y));
                }
                catch (Exception ex)
                {
                  furniture = (Furniture) null;
                }
                if (furniture != null && openPointInHouse.X > 0 && locationFromName.isTileLocationOpen(new Location(openPointInHouse.X - 1, openPointInHouse.Y)))
                {
                  locationFromName.furniture.Add(furniture);
                  this.setTilePosition(openPointInHouse.X - 1, openPointInHouse.Y);
                  this.faceDirection(1);
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4486", false, "%endearmentlower");
                  if (Game1.random.NextDouble() < 0.5)
                    this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4488", true);
                  else
                    this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4489", false);
                }
                else
                {
                  this.setTilePosition(locationFromName.getKitchenStandingSpot());
                  this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4490", false, Array.Empty<string>()), (GameLocation) locationFromName);
                }
              }
              else
              {
                Point openPointInHouse = locationFromName.getRandomOpenPointInHouse(r);
                if (openPointInHouse.X <= 0)
                  return;
                this.setTilePosition(openPointInHouse.X, openPointInHouse.Y);
                this.faceDirection(0);
                if (r.NextDouble() < 0.5)
                {
                  string wallpaperId = locationFromName.GetWallpaperID(openPointInHouse.X, openPointInHouse.Y);
                  if (wallpaperId == null)
                    return;
                  int num4 = r.Next(112);
                  List<int> intList = new List<int>();
                  switch (this.Name)
                  {
                    case "Abigail":
                      intList.AddRange((IEnumerable<int>) new int[10]
                      {
                        2,
                        13,
                        23,
                        26,
                        46,
                        45,
                        64,
                        77,
                        106,
                        107
                      });
                      break;
                    case "Alex":
                      intList.AddRange((IEnumerable<int>) new int[1]
                      {
                        6
                      });
                      break;
                    case "Haley":
                      intList.AddRange((IEnumerable<int>) new int[7]
                      {
                        1,
                        7,
                        10,
                        35,
                        49,
                        84,
                        99
                      });
                      break;
                    case "Krobus":
                      intList.AddRange((IEnumerable<int>) new int[2]
                      {
                        23,
                        24
                      });
                      break;
                    case "Leah":
                      intList.AddRange((IEnumerable<int>) new int[7]
                      {
                        44,
                        108,
                        43,
                        45,
                        92,
                        37,
                        29
                      });
                      break;
                    case "Sebastian":
                      intList.AddRange((IEnumerable<int>) new int[11]
                      {
                        3,
                        4,
                        12,
                        14,
                        30,
                        46,
                        47,
                        56,
                        58,
                        59,
                        107
                      });
                      break;
                    case "Shane":
                      intList.AddRange((IEnumerable<int>) new int[3]
                      {
                        6,
                        21,
                        60
                      });
                      break;
                  }
                  if (intList.Count > 0)
                    num4 = intList[r.Next(intList.Count)];
                  locationFromName.SetWallpaper(num4.ToString(), wallpaperId);
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4496", false);
                }
                else
                {
                  int floorAt = locationFromName.getFloorAt(openPointInHouse);
                  if (floorAt == -1)
                    return;
                  locationFromName.setFloor(r.Next(40), floorAt, true);
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4497", false);
                }
              }
            }
            else if (Game1.isRaining && r.NextDouble() < 0.08 && heartLevelForNpc < 11 && this.Name != "Krobus")
            {
              foreach (Furniture furniture in locationFromName.furniture)
              {
                if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type == 13 && locationFromName.isTileLocationTotallyClearAndPlaceable((int) furniture.tileLocation.X, (int) furniture.tileLocation.Y + 1))
                {
                  this.setTilePosition((int) furniture.tileLocation.X, (int) furniture.tileLocation.Y + 1);
                  this.faceDirection(0);
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4498", true);
                  return;
                }
              }
              this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4499", false, Array.Empty<string>()), (GameLocation) locationFromName, true);
            }
            else if (r.NextDouble() < 0.45)
            {
              Vector2 vector2 = Utility.PointToVector2(locationFromName.GetSpouseRoomSpot());
              this.setTilePosition((int) vector2.X, (int) vector2.Y);
              this.faceDirection(0);
              this.setSpouseRoomMarriageDialogue();
              if (!((string) (NetFieldBase<string, NetString>) this.name == "Sebastian") || !Game1.netWorldState.Value.hasWorldStateID("sebastianFrog"))
                return;
              Point spouseRoomCorner = locationFromName.GetSpouseRoomCorner();
              spouseRoomCorner.X += 2;
              spouseRoomCorner.Y += 5;
              this.setTilePosition(spouseRoomCorner);
              this.faceDirection(2);
            }
            else
            {
              this.setTilePosition(locationFromName.getKitchenStandingSpot());
              this.faceDirection(0);
              if (r.NextDouble() >= 0.2)
                return;
              this.setRandomAfternoonMarriageDialogue(Game1.timeOfDay, (GameLocation) locationFromName);
            }
          }
        }
      }
    }

    public virtual void popOffAnyNonEssentialItems()
    {
      if (!Game1.IsMasterGame || this.currentLocation == null)
        return;
      Object objectAtTile = this.currentLocation.getObjectAtTile(this.getTileX(), this.getTileY());
      if (objectAtTile == null)
        return;
      bool flag = false;
      if (Utility.IsNormalObjectAtParentSheetIndex((Item) objectAtTile, 93) || objectAtTile is Torch)
        flag = true;
      if (!flag)
        return;
      Vector2 tileLocation = objectAtTile.TileLocation;
      objectAtTile.performRemoveAction(tileLocation, this.currentLocation);
      this.currentLocation.objects.Remove(tileLocation);
      objectAtTile.dropItem(this.currentLocation, tileLocation * 64f, tileLocation * 64f);
    }

    public static bool checkTileOccupancyForSpouse(
      GameLocation location,
      Vector2 point,
      string characterToIgnore = "")
    {
      if (location == null)
        return true;
      NPC.isCheckingSpouseTileOccupancy = true;
      int num = location.isTileOccupied(point, characterToIgnore) ? 1 : 0;
      NPC.isCheckingSpouseTileOccupancy = false;
      return num != 0;
    }

    public void addMarriageDialogue(
      string dialogue_file,
      string dialogue_key,
      bool gendered = false,
      params string[] substitutions)
    {
      this.shouldSayMarriageDialogue.Value = true;
      this.currentMarriageDialogue.Add(new MarriageDialogueReference(dialogue_file, dialogue_key, gendered, substitutions));
    }

    public void clearTextAboveHead()
    {
      this.textAboveHead = (string) null;
      this.textAboveHeadPreTimer = -1;
      this.textAboveHeadTimer = -1;
    }

    public bool isVillager()
    {
      if (!this.IsMonster)
      {
        switch (this)
        {
          case Child _:
          case Pet _:
          case Horse _:
          case Junimo _:
            break;
          default:
            return !(this is JunimoHarvester);
        }
      }
      return false;
    }

    public override bool shouldCollideWithBuildingLayer(GameLocation location) => this.isMarried() && (this.Schedule == null || location is FarmHouse) || base.shouldCollideWithBuildingLayer(location);

    public void arriveAtFarmHouse(FarmHouse farmHouse)
    {
      if (Game1.newDay || !this.isMarried() || Game1.timeOfDay <= 630 || this.getTileLocationPoint().Equals(farmHouse.getSpouseBedSpot((string) (NetFieldBase<string, NetString>) this.name)))
        return;
      this.setTilePosition(farmHouse.getEntryLocation());
      this.ignoreScheduleToday = true;
      this.temporaryController = (PathFindController) null;
      this.controller = (PathFindController) null;
      if (Game1.timeOfDay >= 2130)
      {
        Point spouseBedSpot = farmHouse.getSpouseBedSpot((string) (NetFieldBase<string, NetString>) this.name);
        bool flag = farmHouse.GetSpouseBed() != null;
        PathFindController.endBehavior endBehaviorFunction = (PathFindController.endBehavior) null;
        if (flag)
          endBehaviorFunction = new PathFindController.endBehavior(FarmHouse.spouseSleepEndFunction);
        this.controller = new PathFindController((Character) this, (GameLocation) farmHouse, spouseBedSpot, 0, endBehaviorFunction);
        if (this.controller.pathToEndPoint != null && flag)
        {
          foreach (Furniture furniture in farmHouse.furniture)
          {
            if (furniture is BedFurniture && furniture.getBoundingBox(furniture.TileLocation).Intersects(new Microsoft.Xna.Framework.Rectangle(spouseBedSpot.X * 64, spouseBedSpot.Y * 64, 64, 64)))
            {
              (furniture as BedFurniture).ReserveForNPC();
              break;
            }
          }
        }
      }
      else
        this.controller = new PathFindController((Character) this, (GameLocation) farmHouse, farmHouse.getKitchenStandingSpot(), 0);
      if (this.controller.pathToEndPoint == null)
      {
        this.willDestroyObjectsUnderfoot = true;
        this.controller = new PathFindController((Character) this, (GameLocation) farmHouse, farmHouse.getKitchenStandingSpot(), 0);
        this.setNewDialogue(Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.gender, "Strings\\StringsFromCSFiles:NPC.cs.4500"));
      }
      else if (Game1.timeOfDay > 1300)
      {
        if (this.nameOfTodaysSchedule.Equals("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
          this.setNewDialogue("MarriageDialogue", "funReturn_", clearOnMovement: true);
        else if (this.nameOfTodaysSchedule.Equals("marriageJob"))
          this.setNewDialogue("MarriageDialogue", "jobReturn_");
        else if (Game1.timeOfDay < 1800)
          this.setRandomAfternoonMarriageDialogue(Game1.timeOfDay, this.currentLocation, true);
      }
      if (Game1.currentLocation != farmHouse)
        return;
      Game1.currentLocation.playSound("doorClose", NetAudio.SoundContext.NPC);
    }

    public Farmer getSpouse()
    {
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.spouse != null && allFarmer.spouse.Equals(this.Name))
          return allFarmer;
      }
      return (Farmer) null;
    }

    public string getTermOfSpousalEndearment(bool happy = true)
    {
      Farmer spouse = this.getSpouse();
      if (spouse != null)
      {
        if (this.isRoommate())
          return spouse.displayName;
        if (spouse.getFriendshipHeartLevelForNPC(this.Name) < 9)
          return spouse.displayName;
        if (happy && Game1.random.NextDouble() < 0.08)
        {
          switch (Game1.random.Next(8))
          {
            case 0:
              return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4507");
            case 1:
              return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4508");
            case 2:
              return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4509");
            case 3:
              return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4510");
            case 4:
              return !spouse.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4512") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4511");
            case 5:
              return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4513");
            case 6:
              return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4514");
            case 7:
              return !spouse.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4516") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4515");
          }
        }
        if (!happy)
        {
          switch (Game1.random.Next(2))
          {
            case 0:
              return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4517");
            case 1:
              return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4518");
            case 2:
              return spouse.displayName;
          }
        }
        switch (Game1.random.Next(5))
        {
          case 0:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4519");
          case 1:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4518");
          case 2:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4517");
          case 3:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4522");
          case 4:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4523");
        }
      }
      return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4517");
    }

    /// <summary>
    /// return true if spouse encountered obstacle.
    /// if force == true then the obstacle check will be ignored and spouse will absolutely be put into bed.
    /// </summary>
    /// <param name="backToBedMessage"></param>
    /// <param name="currentLocation"></param>
    /// <returns></returns>
    public bool spouseObstacleCheck(
      MarriageDialogueReference backToBedMessage,
      GameLocation currentLocation,
      bool force = false)
    {
      if (!force && !NPC.checkTileOccupancyForSpouse(currentLocation, this.getTileLocation(), this.Name))
        return false;
      Game1.warpCharacter(this, (string) (NetFieldBase<string, NetString>) this.defaultMap, (Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) this.defaultMap) as FarmHouse).getSpouseBedSpot((string) (NetFieldBase<string, NetString>) this.name));
      this.faceDirection(1);
      this.currentMarriageDialogue.Clear();
      this.currentMarriageDialogue.Add(backToBedMessage);
      this.shouldSayMarriageDialogue.Value = true;
      return true;
    }

    public void setTilePosition(Point p) => this.setTilePosition(p.X, p.Y);

    public void setTilePosition(int x, int y) => this.Position = new Vector2((float) (x * 64), (float) (y * 64));

    private void clintHammerSound(Farmer who) => this.currentLocation.playSoundAt("hammer", this.getTileLocation());

    private void robinHammerSound(Farmer who)
    {
      if (!Game1.currentLocation.Equals(this.currentLocation) || !Utility.isOnScreen(this.Position, 256))
        return;
      Game1.playSound(Game1.random.NextDouble() < 0.1 ? "clank" : "axchop");
      this.shakeTimer = 250;
    }

    private void robinVariablePause(Farmer who)
    {
      if (Game1.random.NextDouble() < 0.4)
        this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(27, 300, false, false, new AnimatedSprite.endOfAnimationBehavior(this.robinVariablePause));
      else if (Game1.random.NextDouble() < 0.25)
        this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(23, Game1.random.Next(500, 4000), false, false, new AnimatedSprite.endOfAnimationBehavior(this.robinVariablePause));
      else
        this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(27, Game1.random.Next(1000, 4000), false, false, new AnimatedSprite.endOfAnimationBehavior(this.robinVariablePause));
    }

    public void ReachedEndPoint()
    {
    }

    public void changeSchedulePathDirection()
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      boundingBox.Inflate(2, 2);
      Microsoft.Xna.Framework.Rectangle lastCrossroad = this.lastCrossroad;
      if (this.lastCrossroad.Intersects(boundingBox))
        return;
      this.isCharging = false;
      this.speed = 2;
      ++this.directionIndex;
      this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle(this.getStandingX() - this.getStandingX() % 64, this.getStandingY() - this.getStandingY() % 64, 64, 64);
      this.moveCharacterOnSchedulePath();
    }

    public void moveCharacterOnSchedulePath()
    {
    }

    public void randomSquareMovement(GameTime time)
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      boundingBox.Inflate(2, 2);
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) this.nextSquarePosition.X * 64, (int) this.nextSquarePosition.Y * 64, 64, 64);
      Vector2 nextSquarePosition = this.nextSquarePosition;
      if (this.nextSquarePosition.Equals(Vector2.Zero))
      {
        this.squarePauseAccumulation = 0;
        this.squarePauseTotal = Game1.random.Next(6000 + this.squarePauseOffset, 12000 + this.squarePauseOffset);
        this.nextSquarePosition = new Vector2((float) (this.lastCrossroad.X / 64 - this.lengthOfWalkingSquareX / 2 + Game1.random.Next(this.lengthOfWalkingSquareX)), (float) (this.lastCrossroad.Y / 64 - this.lengthOfWalkingSquareY / 2 + Game1.random.Next(this.lengthOfWalkingSquareY)));
      }
      else if (rectangle.Contains(boundingBox))
      {
        this.Halt();
        if (this.squareMovementFacingPreference != -1)
          this.faceDirection(this.squareMovementFacingPreference);
        this.isCharging = false;
        this.speed = 2;
      }
      else if (boundingBox.Left <= rectangle.Left)
        this.SetMovingOnlyRight();
      else if (boundingBox.Right >= rectangle.Right)
        this.SetMovingOnlyLeft();
      else if (boundingBox.Top <= rectangle.Top)
        this.SetMovingOnlyDown();
      else if (boundingBox.Bottom >= rectangle.Bottom)
        this.SetMovingOnlyUp();
      this.squarePauseAccumulation += time.ElapsedGameTime.Milliseconds;
      if (this.squarePauseAccumulation < this.squarePauseTotal || !rectangle.Contains(boundingBox))
        return;
      this.nextSquarePosition = Vector2.Zero;
      this.isCharging = false;
      this.speed = 2;
    }

    public void returnToEndPoint()
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      boundingBox.Inflate(2, 2);
      if (boundingBox.Left <= this.lastCrossroad.Left)
        this.SetMovingOnlyRight();
      else if (boundingBox.Right >= this.lastCrossroad.Right)
        this.SetMovingOnlyLeft();
      else if (boundingBox.Top <= this.lastCrossroad.Top)
        this.SetMovingOnlyDown();
      else if (boundingBox.Bottom >= this.lastCrossroad.Bottom)
        this.SetMovingOnlyUp();
      boundingBox.Inflate(-2, -2);
      if (!this.lastCrossroad.Contains(boundingBox))
        return;
      this.isWalkingInSquare = false;
      this.nextSquarePosition = Vector2.Zero;
      this.returningToEndPoint = false;
      this.Halt();
    }

    public void SetMovingOnlyUp()
    {
      this.moveUp = true;
      this.moveDown = false;
      this.moveLeft = false;
      this.moveRight = false;
    }

    public void SetMovingOnlyRight()
    {
      this.moveUp = false;
      this.moveDown = false;
      this.moveLeft = false;
      this.moveRight = true;
    }

    public void SetMovingOnlyDown()
    {
      this.moveUp = false;
      this.moveDown = true;
      this.moveLeft = false;
      this.moveRight = false;
    }

    public void SetMovingOnlyLeft()
    {
      this.moveUp = false;
      this.moveDown = false;
      this.moveLeft = true;
      this.moveRight = false;
    }

    public static void populateRoutesFromLocationToLocationList()
    {
      NPC.routesFromLocationToLocation = new List<List<string>>();
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        if (!(location is Farm) && !location.name.Equals((object) "Backwoods"))
        {
          List<string> route = new List<string>();
          NPC.exploreWarpPoints(location, route);
        }
      }
    }

    private static bool exploreWarpPoints(GameLocation l, List<string> route)
    {
      bool flag = false;
      if (l != null && !route.Contains<string>((string) (NetFieldBase<string, NetString>) l.name, (IEqualityComparer<string>) StringComparer.Ordinal))
      {
        route.Add((string) (NetFieldBase<string, NetString>) l.name);
        if (route.Count == 1 || !NPC.doesRoutesListContain(route))
        {
          if (route.Count > 1)
          {
            NPC.routesFromLocationToLocation.Add(route.ToList<string>());
            flag = true;
          }
          foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) l.warps)
          {
            string name = warp.TargetName;
            if (name == "BoatTunnel")
              name = "IslandSouth";
            if (!name.Equals("Farm", StringComparison.Ordinal) && !name.Equals("Woods", StringComparison.Ordinal) && !name.Equals("Backwoods", StringComparison.Ordinal) && !name.Equals("Tunnel", StringComparison.Ordinal) && !name.Contains("Volcano"))
              NPC.exploreWarpPoints(Game1.getLocationFromName(name), route);
          }
          foreach (Point key in l.doors.Keys)
          {
            string name = l.doors[key];
            if (name == "BoatTunnel")
              name = "IslandSouth";
            NPC.exploreWarpPoints(Game1.getLocationFromName(name), route);
          }
        }
        if (route.Count > 0)
          route.RemoveAt(route.Count - 1);
      }
      return flag;
    }

    private static bool doesRoutesListContain(List<string> route)
    {
      foreach (List<string> stringList in NPC.routesFromLocationToLocation)
      {
        if (stringList.Count == route.Count)
        {
          bool flag = true;
          for (int index = 0; index < route.Count; ++index)
          {
            if (!stringList[index].Equals(route[index], StringComparison.Ordinal))
            {
              flag = false;
              break;
            }
          }
          if (flag)
            return true;
        }
      }
      return false;
    }

    public int CompareTo(object obj) => obj is NPC ? (obj as NPC).id - this.id : 0;

    public virtual void Removed()
    {
    }
  }
}
