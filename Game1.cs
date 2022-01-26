// Decompiled with JetBrains decompiler
// Type: StardewValley.Game1
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using SkiaSharp;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Events;
using StardewValley.GameData;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.Quests;
using StardewValley.SDKs;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xTile;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley
{
  /// <summary>This is the main type for your game</summary>
  [InstanceStatics]
  public class Game1 : InstanceGame
  {
    public bool ScreenshotBusy;
    public bool takingMapScreenshot;
    public const int defaultResolutionX = 1280;
    public const int defaultResolutionY = 720;
    public const int pixelZoom = 4;
    public const int tileSize = 64;
    public const int smallestTileSize = 16;
    public const int up = 0;
    public const int right = 1;
    public const int down = 2;
    public const int left = 3;
    public const int spriteIndexForOveralls = 3854;
    public const int colorToleranceForOveralls = 60;
    public const int spriteIndexForOverallsBorder = 3846;
    public const int colorToloranceForOverallsBorder = 20;
    public const int dialogueBoxTileHeight = 5;
    public const int realMilliSecondsPerGameTenMinutes = 7000;
    public const int rainDensity = 70;
    public const int millisecondsPerDialogueLetterType = 30;
    public const float pickToolDelay = 500f;
    public const int defaultMinFishingBiteTime = 600;
    public const int defaultMaxFishingBiteTime = 30000;
    public const int defaultMinFishingNibbleTime = 340;
    public const int defaultMaxFishingNibbleTime = 800;
    public const int minWallpaperPrice = 75;
    public const int maxWallpaperPrice = 500;
    public const int rainLoopLength = 70;
    public const int weather_sunny = 0;
    public const int weather_rain = 1;
    public const int weather_debris = 2;
    public const int weather_lightning = 3;
    public const int weather_festival = 4;
    public const int weather_snow = 5;
    public const int weather_wedding = 6;
    public const byte singlePlayer = 0;
    public const byte multiplayerClient = 1;
    public const byte multiplayerServer = 2;
    public const byte logoScreenGameMode = 4;
    public const byte titleScreenGameMode = 0;
    public const byte loadScreenGameMode = 1;
    public const byte newGameMode = 2;
    public const byte playingGameMode = 3;
    public const byte loadingMode = 6;
    public const byte saveMode = 7;
    public const byte saveCompleteMode = 8;
    public const byte selectGameScreen = 9;
    public const byte creditsMode = 10;
    public const byte errorLogMode = 11;
    public static readonly string version = "1.5.6";
    public static readonly string versionLabel = "Hotfix #3";
    public const float keyPollingThreshold = 650f;
    public const float toolHoldPerPowerupLevel = 600f;
    public const float startingMusicVolume = 1f;
    /// <summary>
    /// ContentManager specifically for loading xTile.Map(s).
    /// Will be unloaded when returning to title.
    /// </summary>
    public LocalizedContentManager xTileContent;
    public static DelayedAction morningSongPlayAction;
    private static LocalizedContentManager _temporaryContent;
    [NonInstancedStatic]
    public static GraphicsDeviceManager graphics;
    [NonInstancedStatic]
    public static LocalizedContentManager content;
    public static SpriteBatch spriteBatch;
    public static GamePadState oldPadState;
    public static float thumbStickSensitivity = 0.1f;
    public static float runThreshold = 0.5f;
    public static int rightStickHoldTime = 0;
    public static int emoteMenuShowTime = 250;
    public static int nextFarmerWarpOffsetX = 0;
    public static int nextFarmerWarpOffsetY = 0;
    public static KeyboardState oldKBState;
    public static MouseState oldMouseState;
    [NonInstancedStatic]
    public static Game1 keyboardFocusInstance = (Game1) null;
    private static Farmer _player;
    public static NetFarmerRoot serverHost;
    protected static bool _isWarping = false;
    [NonInstancedStatic]
    public static bool hasLocalClientsOnly = false;
    public static bool isUsingBackToFrontSorting = false;
    protected static StringBuilder _debugStringBuilder = new StringBuilder();
    public static Dictionary<string, GameLocation> _locationLookup = new Dictionary<string, GameLocation>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public IList<GameLocation> _locations = (IList<GameLocation>) new List<GameLocation>();
    public static Viewport defaultDeviceViewport;
    public static LocationRequest locationRequest;
    public static bool warpingForForcedRemoteEvent = false;
    public GameLocation instanceGameLocation;
    public static IDisplayDevice mapDisplayDevice;
    [NonInstancedStatic]
    public static Microsoft.Xna.Framework.Rectangle safeAreaBounds = new Microsoft.Xna.Framework.Rectangle();
    [NonInstancedStatic]
    public static bool shouldDrawSafeAreaBounds = false;
    public static xTile.Dimensions.Rectangle viewport;
    public static xTile.Dimensions.Rectangle uiViewport;
    public static Texture2D objectSpriteSheet;
    public static Texture2D cropSpriteSheet;
    public static Texture2D mailboxTexture;
    public static Texture2D emoteSpriteSheet;
    public static Texture2D debrisSpriteSheet;
    public static Texture2D toolIconBox;
    public static Texture2D rainTexture;
    public static Texture2D bigCraftableSpriteSheet;
    public static Texture2D swordSwipe;
    public static Texture2D swordSwipeDark;
    public static Texture2D buffsIcons;
    public static Texture2D daybg;
    public static Texture2D nightbg;
    public static Texture2D logoScreenTexture;
    public static Texture2D tvStationTexture;
    public static Texture2D cloud;
    public static Texture2D menuTexture;
    public static Texture2D uncoloredMenuTexture;
    public static Texture2D lantern;
    public static Texture2D windowLight;
    public static Texture2D sconceLight;
    public static Texture2D cauldronLight;
    public static Texture2D shadowTexture;
    public static Texture2D mouseCursors;
    public static Texture2D mouseCursors2;
    public static Texture2D giftboxTexture;
    public static Texture2D controllerMaps;
    public static Texture2D indoorWindowLight;
    public static Texture2D animations;
    public static Texture2D titleScreenBG;
    public static Texture2D logo;
    public static Texture2D concessionsSpriteSheet;
    public static Texture2D birdsSpriteSheet;
    public static Dictionary<string, Stack<Dialogue>> npcDialogues = new Dictionary<string, Stack<Dialogue>>();
    protected readonly List<Farmer> _farmerShadows = new List<Farmer>();
    public static Queue<DelayedAction.delayedBehavior> morningQueue = new Queue<DelayedAction.delayedBehavior>();
    [NonInstancedStatic]
    protected internal static ModHooks hooks = new ModHooks();
    public static InputState input = new InputState();
    protected static IInputSimulator inputSimulator = (IInputSimulator) null;
    public const string objectSpriteSheetName = "Maps\\springobjects";
    public const string animationsName = "TileSheets\\animations";
    public const string mouseCursorsName = "LooseSprites\\Cursors";
    public const string mouseCursors2Name = "LooseSprites\\Cursors2";
    public const string giftboxName = "LooseSprites\\Giftbox";
    public const string toolSpriteSheetName = "TileSheets\\tools";
    public const string bigCraftableSpriteSheetName = "TileSheets\\Craftables";
    public const string debrisSpriteSheetName = "TileSheets\\debris";
    public const string parrotSheetName = "LooseSprites\\parrots";
    public const string hatsSheetName = "Characters\\Farmer\\hats";
    private static Texture2D _toolSpriteSheet = (Texture2D) null;
    public static Dictionary<Vector2, int> crabPotOverlayTiles = new Dictionary<Vector2, int>();
    protected static bool _setSaveName = false;
    protected static string _currentSaveName = "";
    public static string savePathOverride = "";
    public static List<string> mailDeliveredFromMailForTomorrow = new List<string>();
    private static RenderTarget2D _lightmap;
    public static Texture2D fadeToBlackRect;
    public static Texture2D staminaRect;
    public static Texture2D currentCoopTexture;
    public static Texture2D currentBarnTexture;
    public static Texture2D currentHouseTexture;
    public static Texture2D greenhouseTexture;
    public static Texture2D littleEffect;
    public static SpriteFont dialogueFont;
    public static SpriteFont smallFont;
    public static SpriteFont tinyFont;
    public static SpriteFont tinyFontBorder;
    public static float pickToolInterval;
    public static float screenGlowAlpha = 0.0f;
    public static float flashAlpha = 0.0f;
    public static float starCropShimmerPause;
    public static float noteBlockTimer;
    public static int currentGemBirdIndex = 0;
    public Dictionary<string, object> newGameSetupOptions = new Dictionary<string, object>();
    public static bool dialogueUp = false;
    public static bool dialogueTyping = false;
    public static bool pickingTool = false;
    public static bool isQuestion = false;
    public static bool particleRaining = false;
    public static bool newDay = false;
    public static bool inMine = false;
    public static bool menuUp = false;
    public static bool eventUp = false;
    public static bool viewportFreeze = false;
    public static bool eventOver = false;
    public static bool nameSelectUp = false;
    public static bool screenGlow = false;
    public static bool screenGlowHold = false;
    public static bool screenGlowUp;
    public static bool progressBar = false;
    public static bool killScreen = false;
    public static bool coopDwellerBorn;
    public static bool messagePause;
    public static bool boardingBus;
    public static bool listeningForKeyControlDefinitions;
    public static bool weddingToday;
    public static bool exitToTitle;
    public static bool debugMode;
    public static bool displayHUD = true;
    public static bool displayFarmer = true;
    public static bool showKeyHelp;
    public static bool shippingTax;
    public static bool dialogueButtonShrinking;
    public static bool jukeboxPlaying;
    public static bool drawLighting;
    public static bool quit;
    public static bool startedJukeboxMusic;
    public static bool drawGrid;
    public static bool freezeControls;
    public static bool saveOnNewDay;
    public static bool panMode;
    public static bool showingEndOfNightStuff;
    public static bool wasRainingYesterday;
    public static bool hasLoadedGame;
    public static bool isActionAtCurrentCursorTile;
    public static bool isInspectionAtCurrentCursorTile;
    public static bool isSpeechAtCurrentCursorTile;
    public static bool paused;
    public static bool isTimePaused;
    public static bool frameByFrame;
    public static bool lastCursorMotionWasMouse;
    public static bool showingHealth = false;
    public static bool cabinsSeparate = false;
    public static bool hasApplied1_3_UpdateChanges = false;
    public static bool hasApplied1_4_UpdateChanges = false;
    public static bool showingHealthBar = false;
    private static Action postExitToTitleCallback = (Action) null;
    protected int _lastUsedDisplay = -1;
    public bool wasAskedLeoMemory;
    public float controllerSlingshotSafeTime;
    public static Game1.BundleType bundleType = Game1.BundleType.Default;
    public static bool isRaining = false;
    public static bool isSnowing = false;
    public static bool isLightning = false;
    public static bool isDebrisWeather = false;
    public static int weatherForTomorrow;
    public float zoomModifier = 1f;
    private static ScreenFade screenFade;
    public static string currentSeason = "spring";
    public static SerializableDictionary<string, string> bannedUsers = new SerializableDictionary<string, string>();
    private static object _debugOutputLock = new object();
    private static string _debugOutput;
    public static string requestedMusicTrack = "";
    public static string selectedItemsType;
    public static string nameSelectType;
    public static string messageAfterPause = "";
    public static string fertilizer = "";
    public static string samBandName = "The Alfalfas";
    public static string slotResult;
    public static string keyHelpString = "";
    public static string lastDebugInput = "";
    public static string loadingMessage = "";
    public static string errorMessage = "";
    protected Dictionary<Game1.MusicContext, KeyValuePair<string, bool>> _instanceRequestedMusicTracks = new Dictionary<Game1.MusicContext, KeyValuePair<string, bool>>();
    protected Game1.MusicContext _instanceActiveMusicContext;
    public static bool requestedMusicTrackOverrideable;
    public static bool currentTrackOverrideable;
    public static bool requestedMusicDirty = false;
    protected bool _useUnscaledLighting;
    protected bool _didInitiateItemStow;
    public bool instanceIsOverridingTrack;
    private static string[] _shortDayDisplayName = new string[7];
    public static Queue<string> currentObjectDialogue = new Queue<string>();
    public static List<string> worldStateIDs = new List<string>();
    public static List<Response> questionChoices = new List<Response>();
    public static int xLocationAfterWarp;
    public static int yLocationAfterWarp;
    public static int gameTimeInterval;
    public static int currentQuestionChoice;
    public static int currentDialogueCharacterIndex;
    public static int dialogueTypingInterval;
    public static int dayOfMonth = 0;
    public static int year = 1;
    public static int timeOfDay = 600;
    public static int timeOfDayAfterFade = -1;
    public static int numberOfSelectedItems = -1;
    public static int priceOfSelectedItem;
    public static int currentWallpaper;
    public static int farmerWallpaper = 22;
    public static int wallpaperPrice = 75;
    public static int currentFloor = 3;
    public static int FarmerFloor = 29;
    public static int floorPrice = 75;
    public static int dialogueWidth;
    public static int menuChoice;
    public static int tvStation = -1;
    public static int currentBillboard;
    public static int facingDirectionAfterWarp;
    public static int tmpTimeOfDay;
    public static int percentageToWinStardewHero = 70;
    public static int mouseClickPolling;
    public static int gamePadXButtonPolling;
    public static int gamePadAButtonPolling;
    public static int weatherIcon;
    public static int hitShakeTimer;
    public static int staminaShakeTimer;
    public static int pauseThenDoFunctionTimer;
    public static int currentSongIndex = 3;
    public static int cursorTileHintCheckTimer;
    public static int timerUntilMouseFade;
    public static int minecartHighScore;
    public static int whichFarm;
    public static int startingCabins;
    public static ModFarmType whichModFarm = (ModFarmType) null;
    public static ulong? startingGameSeed = new ulong?();
    public static int elliottPiano = 0;
    public static SaveGame.SaveFixes lastAppliedSaveFix;
    public static List<int> dealerCalicoJackTotal;
    public static Microsoft.Xna.Framework.Color morningColor = Microsoft.Xna.Framework.Color.LightBlue;
    public static Microsoft.Xna.Framework.Color eveningColor = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, 0);
    public static Microsoft.Xna.Framework.Color unselectedOptionColor = new Microsoft.Xna.Framework.Color(100, 100, 100);
    public static Microsoft.Xna.Framework.Color screenGlowColor;
    public static NPC currentSpeaker;
    public static Random random = new Random(DateTime.Now.Millisecond);
    public static Random recentMultiplayerRandom = new Random();
    public static IDictionary<int, string> objectInformation;
    public static IDictionary<int, string> bigCraftablesInformation;
    public static IDictionary<int, string> clothingInformation;
    public static IDictionary<string, string> objectContextTags;
    public static List<HUDMessage> hudMessages = new List<HUDMessage>();
    public static IDictionary<string, string> NPCGiftTastes;
    public static float musicPlayerVolume;
    public static float ambientPlayerVolume;
    public static float pauseAccumulator;
    public static float pauseTime;
    public static float upPolling;
    public static float downPolling;
    public static float rightPolling;
    public static float leftPolling;
    public static float debrisSoundInterval;
    public static float toolHold;
    public static float windGust;
    public static float dialogueButtonScale = 1f;
    public static float creditsTimer;
    public static float globalOutdoorLighting;
    public ICue instanceCurrentSong;
    public static IAudioCategory musicCategory;
    public static IAudioCategory soundCategory;
    public static IAudioCategory ambientCategory;
    public static IAudioCategory footstepCategory;
    public PlayerIndex instancePlayerOneIndex;
    [NonInstancedStatic]
    public static IAudioEngine audioEngine;
    [NonInstancedStatic]
    public static WaveBank waveBank;
    [NonInstancedStatic]
    public static WaveBank waveBank1_4;
    [NonInstancedStatic]
    public static ISoundBank soundBank;
    public static Vector2 shiny = Vector2.Zero;
    public static Vector2 previousViewportPosition;
    public static Vector2 currentCursorTile;
    public static Vector2 lastCursorTile = Vector2.Zero;
    public static Vector2 snowPos;
    public Microsoft.Xna.Framework.Rectangle localMultiplayerWindow;
    public static RainDrop[] rainDrops = new RainDrop[70];
    public static double chanceToRainTomorrow = 0.0;
    public static ICue chargeUpSound;
    public static ICue wind;
    public static NetAudioCueManager locationCues = new NetAudioCueManager();
    public static List<WeatherDebris> debrisWeather = new List<WeatherDebris>();
    public static List<TemporaryAnimatedSprite> screenOverlayTempSprites = new List<TemporaryAnimatedSprite>();
    public static List<TemporaryAnimatedSprite> uiOverlayTempSprites = new List<TemporaryAnimatedSprite>();
    private static byte _gameMode;
    private bool _isSaving;
    protected internal static Multiplayer multiplayer = new Multiplayer();
    public static byte multiplayerMode;
    public static IEnumerator<int> currentLoader;
    public static ulong uniqueIDForThisGame = Utility.NewUniqueIdForThisGame();
    public static int[] cropsOfTheWeek;
    public static int[] directionKeyPolling = new int[4];
    public static Quest questOfTheDay;
    public static MoneyMadeScreen moneyMadeScreen;
    public static HashSet<LightSource> currentLightSources = new HashSet<LightSource>();
    public static Microsoft.Xna.Framework.Color ambientLight;
    public static Microsoft.Xna.Framework.Color outdoorLight = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, 0);
    public static Microsoft.Xna.Framework.Color textColor = new Microsoft.Xna.Framework.Color(34, 17, 34);
    public static Microsoft.Xna.Framework.Color textShadowColor = new Microsoft.Xna.Framework.Color(206, 156, 95);
    public static IClickableMenu overlayMenu;
    private static IClickableMenu _activeClickableMenu;
    public static bool isCheckingNonMousePlacement = false;
    private static IMinigame _currentMinigame = (IMinigame) null;
    public static IList<IClickableMenu> onScreenMenus = (IList<IClickableMenu>) new List<IClickableMenu>();
    private const int _fpsHistory = 120;
    protected static List<float> _fpsList = new List<float>(120);
    protected static Stopwatch _fpsStopwatch = new Stopwatch();
    protected static float _fps = 0.0f;
    public static Dictionary<int, string> achievements;
    public static BuffsDisplay buffsDisplay;
    public static DayTimeMoneyBox dayTimeMoneyBox;
    public static NetRootDictionary<long, Farmer> otherFarmers;
    private static readonly FarmerCollection _onlineFarmers = new FarmerCollection();
    public static IGameServer server;
    public static Client client;
    public KeyboardDispatcher instanceKeyboardDispatcher;
    public static Background background;
    public static FarmEvent farmEvent;
    public static Game1.afterFadeFunction afterFade;
    public static Game1.afterFadeFunction afterDialogues;
    public static Game1.afterFadeFunction afterViewport;
    public static Game1.afterFadeFunction viewportReachedTarget;
    public static Game1.afterFadeFunction afterPause;
    public static GameTime currentGameTime;
    public static IList<DelayedAction> delayedActions = (IList<DelayedAction>) new List<DelayedAction>();
    public static Stack<IClickableMenu> endOfNightMenus = new Stack<IClickableMenu>();
    public Options instanceOptions;
    [NonInstancedStatic]
    public static SerializableDictionary<long, Options> splitscreenOptions = new SerializableDictionary<long, Options>();
    public static Game1 game1;
    public static Point lastMousePositionBeforeFade;
    public static int ticks;
    public static EmoteMenu emoteMenu;
    [NonInstancedStatic]
    public static SerializableDictionary<string, string> CustomData = new SerializableDictionary<string, string>();
    public static NetRoot<IWorldState> netWorldState;
    public static ChatBox chatBox;
    public TextEntryMenu instanceTextEntry;
    public static SpecialCurrencyDisplay specialCurrencyDisplay = (SpecialCurrencyDisplay) null;
    public LocalCoopJoinMenu localCoopJoinMenu;
    public static bool drawbounds;
    private static string debugPresenceString;
    public static List<Action> remoteEventQueue = new List<Action>();
    public static List<long> weddingsToday = new List<long>();
    public int instanceIndex;
    public int instanceId;
    public static bool overrideGameMenuReset;
    protected bool _windowResizing;
    protected Point _oldMousePosition;
    protected bool _oldGamepadConnectedState;
    protected int _oldScrollWheelValue;
    public static Point viewportCenter;
    public static Vector2 viewportTarget = new Vector2((float) int.MinValue, (float) int.MinValue);
    public static float viewportSpeed = 2f;
    public static int viewportHold;
    private static bool _cursorDragEnabled = false;
    private static bool _cursorDragPrevEnabled = false;
    private static bool _cursorSpeedDirty = true;
    private const float CursorBaseSpeed = 16f;
    private static float _cursorSpeed = 16f;
    private static float _cursorSpeedScale = 1f;
    private static float _cursorUpdateElapsedSec = 0.0f;
    private static int thumbstickPollingTimer;
    public static bool toggleFullScreen;
    public static string whereIsTodaysFest;
    public const string NO_LETTER_MAIL = "%&NL&%";
    public const string BROADCAST_MAIL_FOR_TOMORROW_PREFIX = "%&MFT&%";
    public const string BROADCAST_SEEN_MAIL_PREFIX = "%&SM&%";
    public const string BROADCAST_MAILBOX_PREFIX = "%&MB&%";
    public bool isLocalMultiplayerNewDayActive;
    protected static Task _newDayTask;
    private static Action _afterNewDayAction;
    public static NewDaySynchronizer newDaySync;
    public static bool forceSnapOnNextViewportUpdate = false;
    public static Vector2 currentViewportTarget;
    public static Vector2 viewportPositionLerp;
    public static float screenGlowRate = 0.005f;
    public static float screenGlowMax;
    public static bool haltAfterCheck = false;
    public static bool uiMode = false;
    public static RenderTarget2D nonUIRenderTarget = (RenderTarget2D) null;
    public static int uiModeCount = 0;
    protected static int _oldUIModeCount = 0;
    private string panModeString;
    public static bool conventionMode = false;
    private EventTest eventTest;
    private bool panFacingDirectionWait;
    public static bool isRunningMacro = false;
    public static int thumbstickMotionMargin;
    public static float thumbstickMotionAccell = 1f;
    public static int triggerPolling;
    public static int rightClickPolling;
    private RenderTarget2D _screen;
    private RenderTarget2D _uiScreen;
    public static Microsoft.Xna.Framework.Color bgColor = new Microsoft.Xna.Framework.Color(5, 3, 4);
    protected readonly BlendState lightingBlend = new BlendState()
    {
      ColorBlendFunction = BlendFunction.ReverseSubtract,
      ColorDestinationBlend = Blend.One,
      ColorSourceBlend = Blend.SourceColor
    };
    public bool isDrawing;
    [NonInstancedStatic]
    public static bool isRenderingScreenBuffer = false;
    protected bool _lastDrewMouseCursor;
    protected static int _activatedTick = 0;
    public static int mouseCursor = 0;
    private static float _mouseCursorTransparency = 1f;
    public static bool wasMouseVisibleThisFrame = true;
    public static NPC objectDialoguePortraitPerson;

    public static bool GetHasRoomAnotherFarm() => true;

    public bool CanTakeScreenshots()
    {
      int num = Environment.Is64BitProcess ? 1 : 0;
      return true;
    }

    public bool CanBrowseScreenshots()
    {
      string path2 = "Screenshots";
      return Directory.Exists(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.OSVersion.Platform != PlatformID.Unix ? Environment.SpecialFolder.ApplicationData : Environment.SpecialFolder.LocalApplicationData), "StardewValley"), path2));
    }

    public bool CanZoomScreenshots() => true;

    public void BrowseScreenshots()
    {
      string path2 = "Screenshots";
      string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.OSVersion.Platform != PlatformID.Unix ? Environment.SpecialFolder.ApplicationData : Environment.SpecialFolder.LocalApplicationData), "StardewValley"), path2);
      if (!Directory.Exists(path))
        return;
      try
      {
        Process.Start(new ProcessStartInfo()
        {
          FileName = path,
          UseShellExecute = true,
          Verb = "open"
        });
      }
      catch (Exception ex)
      {
      }
    }

    public unsafe string takeMapScreenshot(float? in_scale, string screenshot_name, Action onDone)
    {
      float scale = in_scale.Value;
      if (screenshot_name == null || screenshot_name.Trim() == "")
        screenshot_name = SaveGame.FilterFileName((string) (NetFieldBase<string, NetString>) Game1.player.name) + "_" + DateTime.UtcNow.Month.ToString() + "-" + DateTime.UtcNow.Day.ToString() + "-" + DateTime.UtcNow.Year.ToString() + "_" + ((int) DateTime.UtcNow.TimeOfDay.TotalMilliseconds).ToString();
      if (Game1.currentLocation == null)
        return (string) null;
      string path2_1 = screenshot_name + ".png";
      int num1 = 0;
      int num2 = 0;
      int num3 = Game1.currentLocation.map.DisplayWidth;
      int num4 = Game1.currentLocation.map.DisplayHeight;
      try
      {
        PropertyValue propertyValue = (PropertyValue) null;
        if (Game1.currentLocation.map.Properties.TryGetValue("ScreenshotRegion", out propertyValue))
        {
          string[] strArray = propertyValue.ToString().Split(' ');
          num1 = int.Parse(strArray[0]) * 64;
          num2 = int.Parse(strArray[1]) * 64;
          num3 = (int.Parse(strArray[2]) + 1) * 64 - num1;
          num4 = (int.Parse(strArray[3]) + 1) * 64 - num2;
        }
      }
      catch (Exception ex)
      {
        num1 = 0;
        num2 = 0;
        num3 = Game1.currentLocation.map.DisplayWidth;
        num4 = Game1.currentLocation.map.DisplayHeight;
      }
      int num5 = (int) ((double) num3 * (double) scale);
      int num6 = (int) ((double) num4 * (double) scale);
      SKSurface skSurface = (SKSurface) null;
      bool flag1;
      int width1;
      int height1;
      do
      {
        flag1 = false;
        width1 = (int) ((double) num3 * (double) scale);
        height1 = (int) ((double) num4 * (double) scale);
        try
        {
          skSurface = SKSurface.Create(width1, height1, SKColorType.Rgb888x, SKAlphaType.Opaque);
        }
        catch (Exception ex)
        {
          Console.WriteLine("Map Screenshot: Error trying to create Bitmap: " + ex.ToString());
          flag1 = true;
        }
        if (flag1)
          scale -= 0.25f;
        if ((double) scale <= 0.0)
          return (string) null;
      }
      while (flag1);
      int num7 = 2048;
      int num8 = (int) ((double) num7 * (double) scale);
      xTile.Dimensions.Rectangle viewport = Game1.viewport;
      bool displayHud = Game1.displayHUD;
      this.takingMapScreenshot = true;
      float baseZoomLevel = Game1.options.baseZoomLevel;
      Game1.options.baseZoomLevel = 1f;
      RenderTarget2D lightmap = Game1._lightmap;
      Game1._lightmap = (RenderTarget2D) null;
      bool flag2 = false;
      try
      {
        Game1.allocateLightmap(num7, num7);
        int num9 = (int) Math.Ceiling((double) width1 / (double) num8);
        int num10 = (int) Math.Ceiling((double) height1 / (double) num8);
        for (int index1 = 0; index1 < num10; ++index1)
        {
          for (int index2 = 0; index2 < num9; ++index2)
          {
            int width2 = num8;
            int height2 = num8;
            int x = index2 * num8;
            int y = index1 * num8;
            if (x + num8 > width1)
              width2 += width1 - (x + num8);
            if (y + num8 > height1)
              height2 += height1 - (y + num8);
            if (height2 > 0 && width2 > 0)
            {
              Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(x, y, width2, height2);
              RenderTarget2D renderTarget2D = new RenderTarget2D(Game1.graphics.GraphicsDevice, num7, num7, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
              Game1.viewport = new xTile.Dimensions.Rectangle(index2 * num7 + num1, index1 * num7 + num2, num7, num7);
              this._draw(Game1.currentGameTime, renderTarget2D);
              RenderTarget2D renderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, width2, height2, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
              this.GraphicsDevice.SetRenderTarget(renderTarget);
              Game1.spriteBatch.Begin(blendState: BlendState.Opaque, samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.Default, rasterizerState: RasterizerState.CullNone);
              Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
              Game1.spriteBatch.Draw((Texture2D) renderTarget2D, Vector2.Zero, new Microsoft.Xna.Framework.Rectangle?(renderTarget2D.Bounds), white, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
              Game1.spriteBatch.End();
              renderTarget2D.Dispose();
              this.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
              Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[width2 * height2];
              renderTarget.GetData<Microsoft.Xna.Framework.Color>(data);
              SKImageInfo skImageInfo = new SKImageInfo(width2, height2, SKColorType.Rgb888x);
              SKBitmap bitmap = new SKBitmap(rectangle.Width, rectangle.Height, SKColorType.Rgb888x, SKAlphaType.Opaque);
              byte* numPtr1 = (byte*) bitmap.GetPixels().ToPointer();
              for (int index3 = 0; index3 < height2; ++index3)
              {
                for (int index4 = 0; index4 < width2; ++index4)
                {
                  byte* numPtr2 = numPtr1;
                  byte* numPtr3 = numPtr2 + 1;
                  int r = (int) data[index4 + index3 * width2].R;
                  *numPtr2 = (byte) r;
                  byte* numPtr4 = numPtr3;
                  byte* numPtr5 = numPtr4 + 1;
                  int g = (int) data[index4 + index3 * width2].G;
                  *numPtr4 = (byte) g;
                  byte* numPtr6 = numPtr5;
                  byte* numPtr7 = numPtr6 + 1;
                  int b = (int) data[index4 + index3 * width2].B;
                  *numPtr6 = (byte) b;
                  byte* numPtr8 = numPtr7;
                  numPtr1 = numPtr8 + 1;
                  *numPtr8 = byte.MaxValue;
                }
              }
              SKPaint paint = new SKPaint();
              skSurface.Canvas.DrawBitmap(bitmap, SKRect.Create((float) rectangle.X, (float) rectangle.Y, (float) width2, (float) height2), paint);
              bitmap.Dispose();
              renderTarget.Dispose();
            }
          }
        }
        string path2_2 = "Screenshots";
        int folder = Environment.OSVersion.Platform != PlatformID.Unix ? 26 : 28;
        string path = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder) folder), "StardewValley"), path2_2), path2_1);
        FileInfo fileInfo = new FileInfo(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder) folder), "StardewValley"), path2_2), "asdfasdf"));
        if (!fileInfo.Directory.Exists)
          fileInfo.Directory.Create();
        skSurface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).SaveTo((Stream) new FileStream(path, FileMode.OpenOrCreate));
        skSurface.Dispose();
      }
      catch (Exception ex)
      {
        Console.WriteLine("Map Screenshot: Error taking screenshot: " + ex.ToString());
        this.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
        flag2 = true;
      }
      if (Game1._lightmap != null)
      {
        Game1._lightmap.Dispose();
        Game1._lightmap = (RenderTarget2D) null;
      }
      Game1._lightmap = lightmap;
      Game1.options.baseZoomLevel = baseZoomLevel;
      this.takingMapScreenshot = false;
      Game1.displayHUD = displayHud;
      Game1.viewport = viewport;
      return flag2 ? (string) null : path2_1;
    }

    public void CleanupReturningToTitle()
    {
      if (!Game1.game1.IsMainInstance)
      {
        GameRunner.instance.RemoveGameInstance(this);
      }
      else
      {
        foreach (Game1 gameInstance in GameRunner.instance.gameInstances)
        {
          if (gameInstance != this)
            GameRunner.instance.RemoveGameInstance(gameInstance);
        }
      }
      Console.WriteLine("CleanupReturningToTitle()");
      LocalizedContentManager.localizedAssetNames.Clear();
      Event.invalidFestivals.Clear();
      NPC.invalidDialogueFiles.Clear();
      SaveGame.CancelToTitle = false;
      Game1.overlayMenu = (IClickableMenu) null;
      Game1.multiplayer.cachedMultiplayerMaps.Clear();
      Game1.keyboardFocusInstance = (Game1) null;
      Game1.multiplayer.Disconnect(Multiplayer.DisconnectType.ExitedToMainMenu);
      BuildingPaintMenu.savedColors = (List<Vector3>) null;
      Game1.startingGameSeed = new ulong?();
      Game1._afterNewDayAction = (Action) null;
      Game1._currentMinigame = (IMinigame) null;
      Game1.gameMode = (byte) 0;
      this._isSaving = false;
      Game1._mouseCursorTransparency = 1f;
      Game1._newDayTask = (Task) null;
      Game1.newDaySync = (NewDaySynchronizer) null;
      Game1.resetPlayer();
      Game1.serverHost = (NetFarmerRoot) null;
      Game1.afterDialogues = (Game1.afterFadeFunction) null;
      Game1.afterFade = (Game1.afterFadeFunction) null;
      Game1.afterPause = (Game1.afterFadeFunction) null;
      Game1.afterViewport = (Game1.afterFadeFunction) null;
      Game1.ambientLight = new Microsoft.Xna.Framework.Color(0, 0, 0, 0);
      Game1.background = (Background) null;
      Game1.startedJukeboxMusic = false;
      Game1.boardingBus = false;
      Game1.chanceToRainTomorrow = 0.0;
      Game1.chatBox = (ChatBox) null;
      if (Game1.specialCurrencyDisplay != null)
        Game1.specialCurrencyDisplay.Cleanup();
      Game1.specialCurrencyDisplay = (SpecialCurrencyDisplay) null;
      Game1.client = (Client) null;
      Game1.cloud = (Texture2D) null;
      Game1.conventionMode = false;
      Game1.coopDwellerBorn = false;
      Game1.creditsTimer = 0.0f;
      Game1.cropsOfTheWeek = (int[]) null;
      Game1.currentBarnTexture = (Texture2D) null;
      Game1.currentBillboard = 0;
      Game1.currentCoopTexture = (Texture2D) null;
      Game1.currentCursorTile = Vector2.Zero;
      Game1.currentDialogueCharacterIndex = 0;
      Game1.currentFloor = 3;
      Game1.currentHouseTexture = (Texture2D) null;
      Game1.currentLightSources.Clear();
      Game1.currentLoader = (IEnumerator<int>) null;
      Game1.currentLocation = (GameLocation) null;
      Game1.currentObjectDialogue.Clear();
      Game1.currentQuestionChoice = 0;
      Game1.currentSeason = "spring";
      Game1.currentSongIndex = 3;
      Game1.currentSpeaker = (NPC) null;
      Game1.currentViewportTarget = Vector2.Zero;
      Game1.currentWallpaper = 0;
      Game1.cursorTileHintCheckTimer = 0;
      Game1.CustomData = new SerializableDictionary<string, string>();
      Game1.player.team.sharedDailyLuck.Value = 0.001;
      Game1.dayOfMonth = 0;
      Game1.dealerCalicoJackTotal = (List<int>) null;
      Game1.debrisSoundInterval = 0.0f;
      Game1.debrisWeather.Clear();
      Game1.debugMode = false;
      Game1.debugOutput = (string) null;
      Game1.debugPresenceString = "In menus";
      Game1.delayedActions.Clear();
      Game1.morningSongPlayAction = (DelayedAction) null;
      Game1.dialogueButtonScale = 1f;
      Game1.dialogueButtonShrinking = false;
      Game1.dialogueTyping = false;
      Game1.dialogueTypingInterval = 0;
      Game1.dialogueUp = false;
      Game1.dialogueWidth = 1024;
      Game1.displayFarmer = true;
      Game1.displayHUD = true;
      Game1.downPolling = 0.0f;
      Game1.drawGrid = false;
      Game1.drawLighting = false;
      Game1.elliottBookName = "Blue Tower";
      Game1.endOfNightMenus.Clear();
      Game1.errorMessage = "";
      Game1.eveningColor = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, 0, (int) byte.MaxValue);
      Game1.eventOver = false;
      Game1.eventUp = false;
      Game1.exitToTitle = false;
      Game1.facingDirectionAfterWarp = 0;
      Game1.fadeIn = true;
      Game1.fadeToBlack = false;
      Game1.fadeToBlackAlpha = 1.02f;
      Game1.FarmerFloor = 29;
      Game1.farmerWallpaper = 22;
      Game1.farmEvent = (FarmEvent) null;
      Game1.fertilizer = "";
      Game1.flashAlpha = 0.0f;
      Game1.floorPrice = 75;
      Game1.freezeControls = false;
      Game1.gamePadAButtonPolling = 0;
      Game1.gameTimeInterval = 0;
      Game1.globalFade = false;
      Game1.globalFadeSpeed = 0.0f;
      Game1.globalOutdoorLighting = 0.0f;
      Game1.greenhouseTexture = (Texture2D) null;
      Game1.haltAfterCheck = false;
      Game1.hasLoadedGame = false;
      Game1.hitShakeTimer = 0;
      Game1.hudMessages.Clear();
      Game1.inMine = false;
      Game1.isActionAtCurrentCursorTile = false;
      Game1.isDebrisWeather = false;
      Game1.isInspectionAtCurrentCursorTile = false;
      Game1.isLightning = false;
      Game1.isQuestion = false;
      Game1.isRaining = false;
      Game1.isSnowing = false;
      Game1.jukeboxPlaying = false;
      Game1.keyHelpString = "";
      Game1.killScreen = false;
      Game1.lastCursorMotionWasMouse = true;
      Game1.lastCursorTile = Vector2.Zero;
      Game1.lastDebugInput = "";
      Game1.lastMousePositionBeforeFade = Point.Zero;
      Game1.leftPolling = 0.0f;
      Game1.listeningForKeyControlDefinitions = false;
      Game1.loadingMessage = "";
      Game1.locationRequest = (LocationRequest) null;
      Game1.warpingForForcedRemoteEvent = false;
      Game1.locations.Clear();
      Game1.logo = (Texture2D) null;
      Game1.logoScreenTexture = (Texture2D) null;
      Game1.mailbox.Clear();
      Game1.mailboxTexture = (Texture2D) null;
      Game1.mapDisplayDevice = (IDisplayDevice) new XnaDisplayDevice((ContentManager) Game1.content, this.GraphicsDevice);
      Game1.menuChoice = 0;
      Game1.menuUp = false;
      Game1.messageAfterPause = "";
      Game1.messagePause = false;
      Game1.minecartHighScore = 0;
      Game1.moneyMadeScreen = (MoneyMadeScreen) null;
      Game1.mouseClickPolling = 0;
      Game1.mouseCursor = 0;
      Game1.multiplayerMode = (byte) 0;
      Game1.nameSelectType = (string) null;
      Game1.nameSelectUp = false;
      Game1.netWorldState = new NetRoot<IWorldState>((IWorldState) new NetWorldState());
      Game1.newDay = false;
      Game1.nonWarpFade = false;
      Game1.noteBlockTimer = 0.0f;
      Game1.npcDialogues = (Dictionary<string, Stack<Dialogue>>) null;
      Game1.numberOfSelectedItems = -1;
      Game1.objectDialoguePortraitPerson = (NPC) null;
      Game1.hasApplied1_3_UpdateChanges = false;
      Game1.hasApplied1_4_UpdateChanges = false;
      Game1.remoteEventQueue.Clear();
      if (Game1.bannedUsers != null)
        Game1.bannedUsers.Clear();
      Game1.onScreenMenus.Clear();
      Game1.onScreenMenus.Add((IClickableMenu) new Toolbar());
      Game1.dayTimeMoneyBox = new DayTimeMoneyBox();
      Game1.onScreenMenus.Add((IClickableMenu) Game1.dayTimeMoneyBox);
      Game1.buffsDisplay = new BuffsDisplay();
      Game1.onScreenMenus.Add((IClickableMenu) Game1.buffsDisplay);
      bool gamepadControls = Game1.options.gamepadControls;
      bool snappyMenus = Game1.options.snappyMenus;
      Game1.options = new Options();
      Game1.options.gamepadControls = gamepadControls;
      Game1.options.snappyMenus = snappyMenus;
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
        otherFarmer.Value.unload();
      Game1.otherFarmers.Clear();
      Game1.outdoorLight = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, 0, (int) byte.MaxValue);
      Game1.overlayMenu = (IClickableMenu) null;
      Game1.overrideGameMenuReset = false;
      this.panFacingDirectionWait = false;
      Game1.panMode = false;
      this.panModeString = (string) null;
      Game1.particleRaining = false;
      Game1.pauseAccumulator = 0.0f;
      Game1.paused = false;
      Game1.pauseThenDoFunctionTimer = 0;
      Game1.pauseTime = 0.0f;
      Game1.percentageToWinStardewHero = 70;
      Game1.pickingTool = false;
      Game1.pickToolInterval = 0.0f;
      Game1.previousViewportPosition = Vector2.Zero;
      Game1.priceOfSelectedItem = 0;
      Game1.progressBar = false;
      Game1.questionChoices.Clear();
      Game1.questOfTheDay = (Quest) null;
      Game1.quit = false;
      Game1.rightClickPolling = 0;
      Game1.rightPolling = 0.0f;
      Game1.runThreshold = 0.5f;
      Game1.samBandName = "The Alfalfas";
      Game1.saveOnNewDay = true;
      Game1.startingCabins = 0;
      Game1.cabinsSeparate = false;
      Game1.screenGlow = false;
      Game1.screenGlowAlpha = 0.0f;
      Game1.screenGlowColor = new Microsoft.Xna.Framework.Color(0, 0, 0, 0);
      Game1.screenGlowHold = false;
      Game1.screenGlowMax = 0.0f;
      Game1.screenGlowRate = 0.005f;
      Game1.screenGlowUp = false;
      Game1.screenOverlayTempSprites.Clear();
      Game1.uiOverlayTempSprites.Clear();
      Game1.selectedItemsType = (string) null;
      Game1.server = (IGameServer) null;
      Game1.shiny = Vector2.Zero;
      this.newGameSetupOptions.Clear();
      Game1.shippingTax = false;
      Game1.showingEndOfNightStuff = false;
      Game1.showKeyHelp = false;
      Game1.slotResult = (string) null;
      Game1.spawnMonstersAtNight = false;
      Game1.staminaShakeTimer = 0;
      Game1.starCropShimmerPause = 0.0f;
      Game1.swordSwipe = (Texture2D) null;
      Game1.swordSwipeDark = (Texture2D) null;
      Game1.textColor = new Microsoft.Xna.Framework.Color(34, 17, 34, (int) byte.MaxValue);
      Game1.textShadowColor = new Microsoft.Xna.Framework.Color(206, 156, 95, (int) byte.MaxValue);
      Game1.thumbstickMotionAccell = 1f;
      Game1.thumbstickMotionMargin = 0;
      Game1.thumbstickPollingTimer = 0;
      Game1.thumbStickSensitivity = 0.1f;
      Game1.timeOfDay = 600;
      Game1.timeOfDayAfterFade = -1;
      Game1.timerUntilMouseFade = 0;
      Game1.titleScreenBG = (Texture2D) null;
      Game1.tmpTimeOfDay = 0;
      Game1.toggleFullScreen = false;
      Game1.toolHold = 0.0f;
      Game1.toolIconBox = (Texture2D) null;
      Game1.ResetToolSpriteSheet();
      Game1.triggerPolling = 0;
      Game1.tvStation = -1;
      Game1.tvStationTexture = (Texture2D) null;
      Game1.uniqueIDForThisGame = (ulong) (DateTime.UtcNow - new DateTime(2012, 6, 22)).TotalSeconds;
      Game1.upPolling = 0.0f;
      Game1.viewportFreeze = false;
      Game1.viewportHold = 0;
      Game1.viewportPositionLerp = Vector2.Zero;
      Game1.viewportReachedTarget = (Game1.afterFadeFunction) null;
      Game1.viewportSpeed = 2f;
      Game1.viewportTarget = new Vector2((float) int.MinValue, (float) int.MinValue);
      Game1.wallpaperPrice = 75;
      Game1.wasMouseVisibleThisFrame = true;
      Game1.wasRainingYesterday = false;
      Game1.weatherForTomorrow = 0;
      Game1.elliottPiano = 0;
      Game1.weatherIcon = 0;
      Game1.weddingToday = false;
      Game1.whereIsTodaysFest = (string) null;
      Game1.worldStateIDs.Clear();
      Game1.whichFarm = 0;
      Game1.windGust = 0.0f;
      Game1.xLocationAfterWarp = 0;
      Game1.game1.xTileContent.Dispose();
      Game1.game1.xTileContent = this.CreateContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
      Game1.year = 1;
      Game1.yLocationAfterWarp = 0;
      Game1.mailDeliveredFromMailForTomorrow.Clear();
      Game1.bundleType = Game1.BundleType.Default;
      JojaMart.Morris = (NPC) null;
      AmbientLocationSounds.onLocationLeave();
      WeatherDebris.globalWind = -0.25f;
      Utility.killAllStaticLoopingSoundCues();
      TitleMenu.subMenu = (IClickableMenu) null;
      OptionsDropDown.selected = (OptionsDropDown) null;
      JunimoNoteMenu.tempSprites.Clear();
      JunimoNoteMenu.screenSwipe = (ScreenSwipe) null;
      JunimoNoteMenu.canClick = true;
      GameMenu.forcePreventClose = false;
      Club.timesPlayedCalicoJack = 0;
      MineShaft.activeMines.Clear();
      MineShaft.permanentMineChanges.Clear();
      MineShaft.numberOfCraftedStairsUsedThisRun = 0;
      MineShaft.mushroomLevelsGeneratedToday.Clear();
      Desert.boughtMagicRockCandy = false;
      VolcanoDungeon.activeLevels.Clear();
      Rumble.stopRumbling();
      Game1.game1.refreshWindowSettings();
      if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is TitleMenu))
        return;
      (Game1.activeClickableMenu as TitleMenu).applyPreferences();
      Game1.activeClickableMenu.gameWindowSizeChanged(Game1.graphics.GraphicsDevice.Viewport.Bounds, Game1.graphics.GraphicsDevice.Viewport.Bounds);
    }

    public bool IsActiveNoOverlay => this.IsActive && !Program.sdk.HasOverlay;

    public static void GetHasRoomAnotherFarmAsync(ReportHasRoomAnotherFarm callback)
    {
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        bool hasRoomAnotherFarm = Game1.GetHasRoomAnotherFarm();
        callback(hasRoomAnotherFarm);
      }
      else
      {
        Task task = new Task((Action) (() =>
        {
          Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
          callback(Game1.GetHasRoomAnotherFarm());
        }));
        Game1.hooks.StartTask(task, "Farm_SpaceCheck");
      }
    }

    private static string GameModeToString(byte mode)
    {
      switch (mode)
      {
        case 0:
          return string.Format("titleScreenGameMode ({0})", (object) mode);
        case 1:
          return string.Format("loadScreenGameMode ({0})", (object) mode);
        case 2:
          return string.Format("newGameMode ({0})", (object) mode);
        case 3:
          return string.Format("playingGameMode ({0})", (object) mode);
        case 4:
          return string.Format("logoScreenGameMode ({0})", (object) mode);
        case 6:
          return string.Format("loadingMode ({0})", (object) mode);
        case 7:
          return string.Format("saveMode ({0})", (object) mode);
        case 8:
          return string.Format("saveCompleteMode ({0})", (object) mode);
        case 9:
          return string.Format("selectGameScreen ({0})", (object) mode);
        case 10:
          return string.Format("creditsMode ({0})", (object) mode);
        case 11:
          return string.Format("errorLogMode ({0})", (object) mode);
        default:
          return string.Format("unknown ({0})", (object) mode);
      }
    }

    public static string GetVersionString() => string.IsNullOrEmpty(Game1.versionLabel) ? Game1.version : Game1.version + " " + Game1.versionLabel;

    public static LocalizedContentManager temporaryContent
    {
      get
      {
        if (Game1._temporaryContent == null)
          Game1._temporaryContent = Game1.content.CreateTemporary();
        return Game1._temporaryContent;
      }
    }

    public static Farmer player
    {
      get => Game1._player;
      set
      {
        if (Game1._player != null)
        {
          Game1._player.unload();
          Game1._player = (Farmer) null;
        }
        Game1._player = value;
      }
    }

    public static bool isWarping => Game1._isWarping;

    public static IList<GameLocation> locations => Game1.game1._locations;

    public static GameLocation currentLocation
    {
      get => Game1.game1.instanceGameLocation;
      set => Game1.game1.instanceGameLocation = value;
    }

    public static Texture2D toolSpriteSheet
    {
      get
      {
        if (Game1._toolSpriteSheet == null)
          Game1.ResetToolSpriteSheet();
        return Game1._toolSpriteSheet;
      }
    }

    public static void ResetToolSpriteSheet()
    {
      if (Game1._toolSpriteSheet != null)
      {
        Game1._toolSpriteSheet.Dispose();
        Game1._toolSpriteSheet = (Texture2D) null;
      }
      Texture2D texture2D1 = Game1.content.Load<Texture2D>("TileSheets\\tools");
      int width = texture2D1.Width;
      int height = texture2D1.Height;
      int levelCount = texture2D1.LevelCount;
      Texture2D texture2D2 = new Texture2D(Game1.game1.GraphicsDevice, width, height, false, SurfaceFormat.Color);
      Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[width * height];
      texture2D1.GetData<Microsoft.Xna.Framework.Color>(data);
      texture2D2.SetData<Microsoft.Xna.Framework.Color>(data);
      Game1._toolSpriteSheet = texture2D2;
    }

    public static RenderTarget2D lightmap => Game1._lightmap;

    public static void SetSaveName(string new_save_name)
    {
      if (new_save_name == null)
        new_save_name = "";
      Game1._currentSaveName = new_save_name;
      Game1._setSaveName = true;
    }

    public static string GetSaveGameName(bool set_value = true)
    {
      if (!Game1._setSaveName & set_value)
      {
        string str1 = Game1.MasterPlayer.farmName.Value;
        string str2 = str1;
        int num = 2;
        while (SaveGame.IsNewGameSaveNameCollision(str2))
        {
          str2 = str1 + num.ToString();
          ++num;
        }
        Game1.SetSaveName(str2);
      }
      return Game1._currentSaveName;
    }

    private static void allocateLightmap(int width, int height)
    {
      int num1 = 32;
      float num2 = 1f;
      if (Game1.options != null)
      {
        num1 = Game1.options.lightingQuality;
        num2 = !Game1.game1.useUnscaledLighting ? Game1.options.zoomLevel : 1f;
      }
      int width1 = (int) ((double) width * (1.0 / (double) num2) + 64.0) / (num1 / 2);
      int height1 = (int) ((double) height * (1.0 / (double) num2) + 64.0) / (num1 / 2);
      if (Game1.lightmap != null && Game1.lightmap.Width == width1 && Game1.lightmap.Height == height1)
        return;
      if (Game1._lightmap != null)
        Game1._lightmap.Dispose();
      Game1._lightmap = new RenderTarget2D(Game1.graphics.GraphicsDevice, width1, height1, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
    }

    public static bool spawnMonstersAtNight
    {
      get => (bool) (NetFieldBase<bool, NetBool>) Game1.player.team.spawnMonstersAtNight;
      set => Game1.player.team.spawnMonstersAtNight.Value = value;
    }

    public static bool fadeToBlack
    {
      get => Game1.screenFade.fadeToBlack;
      set => Game1.screenFade.fadeToBlack = value;
    }

    public static bool fadeIn
    {
      get => Game1.screenFade.fadeIn;
      set => Game1.screenFade.fadeIn = value;
    }

    public static bool globalFade
    {
      get => Game1.screenFade.globalFade;
      set => Game1.screenFade.globalFade = value;
    }

    public static bool nonWarpFade
    {
      get => Game1.screenFade.nonWarpFade;
      set => Game1.screenFade.nonWarpFade = value;
    }

    public static float fadeToBlackAlpha
    {
      get => Game1.screenFade.fadeToBlackAlpha;
      set => Game1.screenFade.fadeToBlackAlpha = value;
    }

    public static float globalFadeSpeed
    {
      get => Game1.screenFade.globalFadeSpeed;
      set => Game1.screenFade.globalFadeSpeed = value;
    }

    public static string CurrentSeasonDisplayName => Game1.content.LoadString("Strings\\StringsFromCSFiles:" + Game1.currentSeason);

    public static string debugOutput
    {
      get => Game1._debugOutput;
      set
      {
        lock (Game1._debugOutputLock)
        {
          if (!(Game1._debugOutput != value))
            return;
          Game1._debugOutput = value;
          if (string.IsNullOrEmpty(Game1._debugOutput))
            return;
          Console.WriteLine("DebugOutput: {0}", (object) Game1._debugOutput);
        }
      }
    }

    public static string elliottBookName
    {
      get
      {
        if (Game1.player != null && Game1.player.DialogueQuestionsAnswered.Contains(958699))
          return Game1.content.LoadString("Strings\\Events:ElliottBook_mystery");
        return Game1.player != null && Game1.player.DialogueQuestionsAnswered.Contains(958700) ? Game1.content.LoadString("Strings\\Events:ElliottBook_romance") : Game1.content.LoadString("Strings\\Events:ElliottBook_default");
      }
      set
      {
      }
    }

    protected static Dictionary<Game1.MusicContext, KeyValuePair<string, bool>> _requestedMusicTracks
    {
      get => Game1.game1._instanceRequestedMusicTracks;
      set => Game1.game1._instanceRequestedMusicTracks = value;
    }

    protected static Game1.MusicContext _activeMusicContext
    {
      get => Game1.game1._instanceActiveMusicContext;
      set => Game1.game1._instanceActiveMusicContext = value;
    }

    public static bool isOverridingTrack
    {
      get => Game1.game1.instanceIsOverridingTrack;
      set => Game1.game1.instanceIsOverridingTrack = value;
    }

    public bool useUnscaledLighting
    {
      get => this._useUnscaledLighting;
      set
      {
        if (this._useUnscaledLighting == value)
          return;
        this._useUnscaledLighting = value;
        Game1.allocateLightmap(this.localMultiplayerWindow.Width, this.localMultiplayerWindow.Height);
      }
    }

    public static IList<string> mailbox => (IList<string>) Game1.player.mailbox;

    public static ICue currentSong
    {
      get => Game1.game1.instanceCurrentSong;
      set => Game1.game1.instanceCurrentSong = value;
    }

    public static PlayerIndex playerOneIndex
    {
      get => Game1.game1.instancePlayerOneIndex;
      set => Game1.game1.instancePlayerOneIndex = value;
    }

    public static byte gameMode
    {
      get => Game1._gameMode;
      set
      {
        if ((int) Game1._gameMode == (int) value)
          return;
        Console.WriteLine("gameMode was '{0}', set to '{1}'.", (object) Game1.GameModeToString(Game1._gameMode), (object) Game1.GameModeToString(value));
        Game1._gameMode = value;
      }
    }

    public bool IsSaving
    {
      get => this._isSaving;
      set => this._isSaving = value;
    }

    public static Stats stats => Game1.player.stats;

    public static IClickableMenu activeClickableMenu
    {
      get => Game1._activeClickableMenu;
      set
      {
        if (Game1._activeClickableMenu is IDisposable && !Game1._activeClickableMenu.HasDependencies())
          (Game1._activeClickableMenu as IDisposable).Dispose();
        if (Game1._activeClickableMenu != null && value == null)
          Game1.timerUntilMouseFade = 0;
        if (Game1.textEntry != null && Game1._activeClickableMenu != value)
          Game1.closeTextEntry();
        Game1._activeClickableMenu = value;
        if (Game1._activeClickableMenu == null || Game1.eventUp && (Game1.CurrentEvent == null || !Game1.CurrentEvent.playerControlSequence || Game1.player.UsingTool))
          return;
        Game1.player.Halt();
      }
    }

    public static IMinigame currentMinigame
    {
      get => Game1._currentMinigame;
      set
      {
        Game1._currentMinigame = value;
        if (value == null)
        {
          if (Game1.currentLocation != null)
            Game1.setRichPresence("location", (object) Game1.currentLocation.Name);
          Game1.randomizeDebrisWeatherPositions(Game1.debrisWeather);
          Game1.randomizeRainPositions();
        }
        else
        {
          if (value.minigameId() == null)
            return;
          Game1.setRichPresence("minigame", (object) value.minigameId());
        }
      }
    }

    public static bool canHaveWeddingOnDay(int day, string season) => !Utility.isFestivalDay(day, season);

    public static void RefreshQuestOfTheDay()
    {
      Game1.questOfTheDay = Utility.getQuestOfTheDay();
      if (!Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason) && !Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
        return;
      Game1.questOfTheDay = (Quest) null;
    }

    public static void ExitToTitle(Action postExitCallback = null)
    {
      Game1._requestedMusicTracks.Clear();
      Game1.UpdateRequestedMusicTrack();
      Game1.changeMusicTrack("none");
      Game1.setGameMode((byte) 0);
      Game1.exitToTitle = true;
      Game1.postExitToTitleCallback = postExitCallback;
    }

    public static Object dishOfTheDay
    {
      get => Game1.netWorldState.Value.DishOfTheDay.Value;
      set => Game1.netWorldState.Value.DishOfTheDay.Value = value;
    }

    public static KeyboardDispatcher keyboardDispatcher
    {
      get => Game1.game1.instanceKeyboardDispatcher;
      set => Game1.game1.instanceKeyboardDispatcher = value;
    }

    public static Options options
    {
      get => Game1.game1.instanceOptions;
      set => Game1.game1.instanceOptions = value;
    }

    public static TextEntryMenu textEntry
    {
      get => Game1.game1.instanceTextEntry;
      set => Game1.game1.instanceTextEntry = value;
    }

    public static WorldDate Date => Game1.netWorldState.Value.Date;

    public static bool NetTimePaused => Game1.netWorldState.Get().IsTimePaused;

    public static bool HostPaused => Game1.netWorldState.Get().IsPaused;

    public static bool IsMultiplayer => Game1.otherFarmers.Count > 0;

    public static bool IsClient => Game1.multiplayerMode == (byte) 1;

    public static bool IsServer => Game1.multiplayerMode == (byte) 2;

    public static bool IsMasterGame => Game1.multiplayerMode == (byte) 0 || Game1.multiplayerMode == (byte) 2;

    public static Farmer MasterPlayer => !Game1.IsMasterGame ? Game1.serverHost.Value : Game1.player;

    public static bool IsChatting
    {
      get => Game1.chatBox != null && Game1.chatBox.isActive();
      set
      {
        if (value == Game1.chatBox.isActive())
          return;
        if (value)
          Game1.chatBox.activate();
        else
          Game1.chatBox.clickAway();
      }
    }

    public static Event CurrentEvent => Game1.currentLocation == null ? (Event) null : Game1.currentLocation.currentEvent;

    public static MineShaft mine
    {
      get
      {
        if (Game1.locationRequest != null && Game1.locationRequest.Location is MineShaft)
          return Game1.locationRequest.Location as MineShaft;
        return Game1.currentLocation is MineShaft ? Game1.currentLocation as MineShaft : (MineShaft) null;
      }
    }

    public static int CurrentMineLevel => Game1.currentLocation is MineShaft ? (Game1.currentLocation as MineShaft).mineLevel : 0;

    public Game1(PlayerIndex player_index, int index)
      : this()
    {
      this.instancePlayerOneIndex = player_index;
      this.instanceIndex = index;
    }

    public Game1()
    {
      this.instanceId = GameRunner.instance.GetNewInstanceID();
      if (Program.gamePtr == null)
        Program.gamePtr = this;
      Game1._temporaryContent = this.CreateContentManager(this.Content.ServiceProvider, this.Content.RootDirectory);
    }

    public void TranslateFields()
    {
      LocalizedContentManager.localizedAssetNames.Clear();
      Game1.samBandName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2156");
      Game1.elliottBookName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2157");
      Game1.objectSpriteSheet = Game1.content.Load<Texture2D>("Maps\\springobjects");
      Game1.dialogueFont = Game1.content.Load<SpriteFont>("Fonts\\SpriteFont1");
      Game1.smallFont = Game1.content.Load<SpriteFont>("Fonts\\SmallFont");
      Game1.smallFont.LineSpacing = 26;
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.ko:
          Game1.smallFont.LineSpacing += 16;
          break;
        case LocalizedContentManager.LanguageCode.tr:
          Game1.smallFont.LineSpacing += 4;
          break;
        case LocalizedContentManager.LanguageCode.mod:
          Game1.smallFont.LineSpacing = LocalizedContentManager.CurrentModLanguage.SmallFontLineSpacing;
          break;
      }
      Game1.tinyFont = Game1.content.Load<SpriteFont>("Fonts\\tinyFont");
      Game1.tinyFontBorder = Game1.content.Load<SpriteFont>("Fonts\\tinyFontBorder");
      Game1.objectInformation = (IDictionary<int, string>) Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
      Game1.clothingInformation = (IDictionary<int, string>) Game1.content.Load<Dictionary<int, string>>("Data\\ClothingInformation");
      Game1.bigCraftablesInformation = (IDictionary<int, string>) Game1.content.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation");
      Game1.achievements = Game1.content.Load<Dictionary<int, string>>("Data\\Achievements");
      CraftingRecipe.craftingRecipes = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
      CraftingRecipe.cookingRecipes = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
      MovieTheater.ClearCachedLocalizedData();
      Game1.mouseCursors = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
      Game1.mouseCursors2 = Game1.content.Load<Texture2D>("LooseSprites\\Cursors2");
      Game1.giftboxTexture = Game1.content.Load<Texture2D>("LooseSprites\\Giftbox");
      Game1.controllerMaps = Game1.content.Load<Texture2D>("LooseSprites\\ControllerMaps");
      Game1.NPCGiftTastes = (IDictionary<string, string>) Game1.content.Load<Dictionary<string, string>>("Data\\NPCGiftTastes");
      Game1._shortDayDisplayName[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3042");
      Game1._shortDayDisplayName[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3043");
      Game1._shortDayDisplayName[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3044");
      Game1._shortDayDisplayName[3] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3045");
      Game1._shortDayDisplayName[4] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3046");
      Game1._shortDayDisplayName[5] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3047");
      Game1._shortDayDisplayName[6] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3048");
    }

    public void exitEvent(object sender, EventArgs e)
    {
      Game1.multiplayer.Disconnect(Multiplayer.DisconnectType.ClosedGame);
      Game1.keyboardDispatcher.Cleanup();
    }

    public void refreshWindowSettings() => GameRunner.instance.OnWindowSizeChange((object) null, (EventArgs) null);

    public void Window_ClientSizeChanged(object sender, EventArgs e)
    {
      if (this._windowResizing)
        return;
      Console.WriteLine("Window_ClientSizeChanged(); Window.ClientBounds={0}", (object) this.Window.ClientBounds);
      if (Game1.options == null)
      {
        Console.WriteLine("Window_ClientSizeChanged(); options is null, returning.");
      }
      else
      {
        this._windowResizing = true;
        int w = Game1.graphics.IsFullScreen ? Game1.graphics.PreferredBackBufferWidth : this.Window.ClientBounds.Width;
        int h = Game1.graphics.IsFullScreen ? Game1.graphics.PreferredBackBufferHeight : this.Window.ClientBounds.Height;
        GameRunner.instance.ExecuteForInstances((Action<Game1>) (instance => instance.SetWindowSize(w, h)));
        this._windowResizing = false;
      }
    }

    public virtual void SetWindowSize(int w, int h)
    {
      Microsoft.Xna.Framework.Rectangle oldBounds = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height);
      Microsoft.Xna.Framework.Rectangle clientBounds1 = this.Window.ClientBounds;
      bool flag = false;
      if (Environment.OSVersion.Platform == PlatformID.Win32NT)
      {
        if (w < 1280 && !Game1.graphics.IsFullScreen)
        {
          w = 1280;
          flag = true;
        }
        if (h < 720 && !Game1.graphics.IsFullScreen)
        {
          h = 720;
          flag = true;
        }
      }
      if (!Game1.graphics.IsFullScreen && this.Window.AllowUserResizing)
      {
        Game1.graphics.PreferredBackBufferWidth = w;
        Game1.graphics.PreferredBackBufferHeight = h;
      }
      if (flag)
      {
        Microsoft.Xna.Framework.Rectangle clientBounds2 = this.Window.ClientBounds;
      }
      if (this.IsMainInstance && Game1.graphics.SynchronizeWithVerticalRetrace != Game1.options.vsyncEnabled)
      {
        Game1.graphics.SynchronizeWithVerticalRetrace = Game1.options.vsyncEnabled;
        Console.WriteLine("Vsync toggled: " + Game1.graphics.SynchronizeWithVerticalRetrace.ToString());
      }
      Game1.graphics.ApplyChanges();
      try
      {
        this.localMultiplayerWindow = !Game1.graphics.IsFullScreen ? new Microsoft.Xna.Framework.Rectangle(0, 0, w, h) : new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
      }
      catch (Exception ex)
      {
      }
      Game1.defaultDeviceViewport = new Viewport(this.localMultiplayerWindow);
      List<Vector4> vector4List = new List<Vector4>();
      if (GameRunner.instance.gameInstances.Count <= 1)
        vector4List.Add(new Vector4(0.0f, 0.0f, 1f, 1f));
      else if (GameRunner.instance.gameInstances.Count == 2)
      {
        vector4List.Add(new Vector4(0.0f, 0.0f, 0.5f, 1f));
        vector4List.Add(new Vector4(0.5f, 0.0f, 0.5f, 1f));
      }
      else if (GameRunner.instance.gameInstances.Count == 3)
      {
        vector4List.Add(new Vector4(0.0f, 0.0f, 1f, 0.5f));
        vector4List.Add(new Vector4(0.0f, 0.5f, 0.5f, 0.5f));
        vector4List.Add(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
      }
      else if (GameRunner.instance.gameInstances.Count == 4)
      {
        vector4List.Add(new Vector4(0.0f, 0.0f, 0.5f, 0.5f));
        vector4List.Add(new Vector4(0.5f, 0.0f, 0.5f, 0.5f));
        vector4List.Add(new Vector4(0.0f, 0.5f, 0.5f, 0.5f));
        vector4List.Add(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
      }
      this.zoomModifier = GameRunner.instance.gameInstances.Count > 1 ? 0.5f : 1f;
      Vector4 vector4 = vector4List[Game1.game1.instanceIndex];
      Vector2? nullable = new Vector2?();
      if (this.uiScreen != null)
        nullable = new Vector2?(new Vector2((float) this.uiScreen.Width, (float) this.uiScreen.Height));
      this.localMultiplayerWindow.X = (int) ((double) w * (double) vector4.X);
      this.localMultiplayerWindow.Y = (int) ((double) h * (double) vector4.Y);
      this.localMultiplayerWindow.Width = (int) Math.Ceiling((double) w * (double) vector4.Z);
      this.localMultiplayerWindow.Height = (int) Math.Ceiling((double) h * (double) vector4.W);
      try
      {
        int width1 = (int) Math.Ceiling((double) this.localMultiplayerWindow.Width * (1.0 / (double) Game1.options.zoomLevel));
        int height1 = (int) Math.Ceiling((double) this.localMultiplayerWindow.Height * (1.0 / (double) Game1.options.zoomLevel));
        this.screen = new RenderTarget2D(Game1.graphics.GraphicsDevice, width1, height1, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        this.screen.Name = "Screen";
        int width2 = (int) Math.Ceiling((double) this.localMultiplayerWindow.Width / (double) Game1.options.uiScale);
        int height2 = (int) Math.Ceiling((double) this.localMultiplayerWindow.Height / (double) Game1.options.uiScale);
        this.uiScreen = new RenderTarget2D(Game1.graphics.GraphicsDevice, width2, height2, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        this.uiScreen.Name = "UI Screen";
      }
      catch (Exception ex)
      {
      }
      Game1.updateViewportForScreenSizeChange(false, this.localMultiplayerWindow.Width, this.localMultiplayerWindow.Height);
      if (nullable.HasValue && (double) nullable.Value.X == (double) this.uiScreen.Width && (double) nullable.Value.Y == (double) this.uiScreen.Height)
        return;
      Game1.PushUIMode();
      if (Game1.textEntry != null)
        Game1.textEntry.gameWindowSizeChanged(oldBounds, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
      foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
        onScreenMenu.gameWindowSizeChanged(oldBounds, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
      if (Game1.currentMinigame != null)
        Game1.currentMinigame.changeScreenSize();
      if (Game1.activeClickableMenu != null)
        Game1.activeClickableMenu.gameWindowSizeChanged(oldBounds, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
      if (Game1.activeClickableMenu is GameMenu && !Game1.overrideGameMenuReset)
      {
        if ((Game1.activeClickableMenu as GameMenu).GetCurrentPage() is OptionsPage)
          ((Game1.activeClickableMenu as GameMenu).GetCurrentPage() as OptionsPage).preWindowSizeChange();
        Game1.activeClickableMenu = (IClickableMenu) new GameMenu((Game1.activeClickableMenu as GameMenu).currentTab);
        if ((Game1.activeClickableMenu as GameMenu).GetCurrentPage() is OptionsPage)
          ((Game1.activeClickableMenu as GameMenu).GetCurrentPage() as OptionsPage).postWindowSizeChange();
      }
      Game1.PopUIMode();
    }

    private void Game1_Exiting(object sender, EventArgs e) => Program.sdk.Shutdown();

    public static void setGameMode(byte mode)
    {
      Console.WriteLine("setGameMode( '{0}' )", (object) Game1.GameModeToString(mode));
      Game1._gameMode = mode;
      if (Game1.temporaryContent != null)
        Game1.temporaryContent.Unload();
      switch (mode)
      {
        case 0:
          bool flag = false;
          if (Game1.activeClickableMenu != null && Game1.currentGameTime != null && Game1.currentGameTime.TotalGameTime.TotalSeconds > 10.0)
            flag = true;
          if (Game1.game1.instanceIndex > 0)
            break;
          Game1.activeClickableMenu = (IClickableMenu) new TitleMenu();
          if (!flag)
            break;
          (Game1.activeClickableMenu as TitleMenu).skipToTitleButtons();
          break;
        case 3:
          Game1.hasApplied1_3_UpdateChanges = true;
          Game1.hasApplied1_4_UpdateChanges = false;
          break;
      }
    }

    public static void updateViewportForScreenSizeChange(
      bool fullscreenChange,
      int width,
      int height)
    {
      Game1.forceSnapOnNextViewportUpdate = true;
      if (Game1.graphics.GraphicsDevice != null)
        Game1.allocateLightmap(width, height);
      width = (int) Math.Ceiling((double) width / (double) Game1.options.zoomLevel);
      height = (int) Math.Ceiling((double) height / (double) Game1.options.zoomLevel);
      Point centerPoint = new Point(Game1.viewport.X + Game1.viewport.Width / 2, Game1.viewport.Y + Game1.viewport.Height / 2);
      bool flag = false;
      if (Game1.viewport.Width != width || Game1.viewport.Height != height)
        flag = true;
      Game1.viewport = new xTile.Dimensions.Rectangle(centerPoint.X - width / 2, centerPoint.Y - height / 2, width, height);
      if (Game1.currentLocation == null)
        return;
      if (Game1.eventUp)
      {
        if (Game1.IsFakedBlackScreen() || !Game1.currentLocation.IsOutdoors)
          return;
        Game1.clampViewportToGameMap();
      }
      else
      {
        if (((Game1.viewport.X >= 0 ? 1 : (!Game1.currentLocation.IsOutdoors ? 1 : 0)) | (fullscreenChange ? 1 : 0)) != 0)
        {
          centerPoint = new Point(Game1.viewport.X + Game1.viewport.Width / 2, Game1.viewport.Y + Game1.viewport.Height / 2);
          Game1.viewport = new xTile.Dimensions.Rectangle(centerPoint.X - width / 2, centerPoint.Y - height / 2, width, height);
          Game1.UpdateViewPort(true, centerPoint);
        }
        if (!flag)
          return;
        Game1.forceSnapOnNextViewportUpdate = true;
        Game1.randomizeRainPositions();
        Game1.randomizeDebrisWeatherPositions(Game1.debrisWeather);
      }
    }

    public void Instance_Initialize() => this.Initialize();

    public static bool IsFading()
    {
      if (Game1.globalFade || Game1.fadeIn && (double) Game1.fadeToBlackAlpha > 0.0)
        return true;
      return Game1.fadeToBlack && (double) Game1.fadeToBlackAlpha < 1.0;
    }

    public static bool IsFakedBlackScreen() => Game1.currentMinigame == null && (Game1.CurrentEvent == null || Game1.CurrentEvent.currentCustomEventScript == null) && Game1.eventUp && (double) (int) Math.Floor((double) new Point(Game1.viewport.X + Game1.viewport.Width / 2, Game1.viewport.Y + Game1.viewport.Height / 2).X / 64.0) <= -200.0;

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      Game1.keyboardDispatcher = new KeyboardDispatcher(this.Window);
      Game1.screenFade = new ScreenFade(new Func<bool>(this.onFadeToBlackComplete), new Action(Game1.onFadedBackInComplete));
      Game1.options = new Options();
      Game1.options.musicVolumeLevel = 1f;
      Game1.options.soundVolumeLevel = 1f;
      Game1.otherFarmers = new NetRootDictionary<long, Farmer>();
      Game1.otherFarmers.Serializer = SaveGame.farmerSerializer;
      Game1.viewport = new xTile.Dimensions.Rectangle(new Size(Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight));
      string rootDirectory = this.Content.RootDirectory;
      if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", rootDirectory, "XACT", "FarmerSounds.xgs")))
        File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootDirectory, "XACT", "FarmerSounds.xgs"));
      if (this.IsMainInstance)
      {
        try
        {
          AudioEngine engine = new AudioEngine(Path.Combine(rootDirectory, "XACT", "FarmerSounds.xgs"));
          engine.GetReverbSettings()[18] = 4f;
          engine.GetReverbSettings()[17] = -12f;
          Game1.audioEngine = (IAudioEngine) new AudioEngineWrapper(engine);
          Game1.waveBank = new WaveBank(Game1.audioEngine.Engine, Path.Combine(rootDirectory, "XACT", "Wave Bank.xwb"));
          Game1.waveBank1_4 = new WaveBank(Game1.audioEngine.Engine, Path.Combine(rootDirectory, "XACT", "Wave Bank(1.4).xwb"));
          Game1.soundBank = (ISoundBank) new SoundBankWrapper(new SoundBank(Game1.audioEngine.Engine, Path.Combine(rootDirectory, "XACT", "Sound Bank.xsb")));
        }
        catch (Exception ex)
        {
          Console.WriteLine("Game.Initialize() caught exception initializing XACT:\n{0}", (object) ex);
          Game1.audioEngine = (IAudioEngine) new DummyAudioEngine();
          Game1.soundBank = (ISoundBank) new DummySoundBank();
        }
      }
      Game1.audioEngine.Update();
      Game1.musicCategory = Game1.audioEngine.GetCategory("Music");
      Game1.soundCategory = Game1.audioEngine.GetCategory("Sound");
      Game1.ambientCategory = Game1.audioEngine.GetCategory("Ambient");
      Game1.footstepCategory = Game1.audioEngine.GetCategory("Footsteps");
      Game1.currentSong = (ICue) null;
      if (Game1.soundBank != null)
      {
        Game1.wind = Game1.soundBank.GetCue("wind");
        Game1.chargeUpSound = Game1.soundBank.GetCue("toolCharge");
      }
      Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
      int width = viewport.Width;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int height = viewport.Height;
      this.screen = new RenderTarget2D(Game1.graphics.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
      Game1.allocateLightmap(width, height);
      AmbientLocationSounds.InitShared();
      Game1.previousViewportPosition = Vector2.Zero;
      Game1.PushUIMode();
      Game1.PopUIMode();
      Game1.setRichPresence("menus");
    }

    /// <summary>one-time changes for loaded files for the 1.3 update</summary>
    public static void apply1_3_UpdateChanges()
    {
      if (!Game1.IsMasterGame)
        return;
      if (!Game1.player.craftingRecipes.ContainsKey("Wood Sign"))
        Game1.player.craftingRecipes.Add("Wood Sign", 0);
      if (!Game1.player.craftingRecipes.ContainsKey("Stone Sign"))
        Game1.player.craftingRecipes.Add("Stone Sign", 0);
      FarmHouse locationFromName = Game1.getLocationFromName("FarmHouse") as FarmHouse;
      locationFromName.furniture.Add(new Furniture(1792, Utility.PointToVector2(locationFromName.getFireplacePoint())));
      if (!Game1.MasterPlayer.mailReceived.Contains("JojaMember") && !Game1.getLocationFromName("Town").isTileOccupiedForPlacement(new Vector2(57f, 16f)))
        Game1.getLocationFromName("Town").objects.Add(new Vector2(57f, 16f), new Object(Vector2.Zero, 55));
      Game1.MarkFloorChestAsCollectedIfNecessary(10);
      Game1.MarkFloorChestAsCollectedIfNecessary(20);
      Game1.MarkFloorChestAsCollectedIfNecessary(40);
      Game1.MarkFloorChestAsCollectedIfNecessary(50);
      Game1.MarkFloorChestAsCollectedIfNecessary(60);
      Game1.MarkFloorChestAsCollectedIfNecessary(70);
      Game1.MarkFloorChestAsCollectedIfNecessary(80);
      Game1.MarkFloorChestAsCollectedIfNecessary(90);
      Game1.MarkFloorChestAsCollectedIfNecessary(100);
      Game1.hasApplied1_3_UpdateChanges = true;
    }

    /// <summary>one-time changes for loaded files for the 1.4 update</summary>
    public static void apply1_4_UpdateChanges()
    {
      if (!Game1.IsMasterGame)
        return;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        foreach (string key in allFarmer.friendshipData.Keys)
          allFarmer.friendshipData[key].Points = Math.Min(allFarmer.friendshipData[key].Points, 3125);
        if (allFarmer.ForagingLevel >= 7 && !allFarmer.craftingRecipes.ContainsKey("Tree Fertilizer"))
          allFarmer.craftingRecipes.Add("Tree Fertilizer", 0);
      }
      foreach (KeyValuePair<string, string> keyValuePair in Game1.netWorldState.Value.BundleData)
      {
        int int32 = Convert.ToInt32(keyValuePair.Key.Split('/')[1]);
        if (!Game1.netWorldState.Value.Bundles.ContainsKey(int32))
          Game1.netWorldState.Value.Bundles.Add(int32, new NetArray<bool, NetBool>(keyValuePair.Value.Split('/')[2].Split(' ').Length));
        if (!Game1.netWorldState.Value.BundleRewards.ContainsKey(int32))
          Game1.netWorldState.Value.BundleRewards.Add(int32, new NetBool(false));
      }
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        foreach (Item obj in (NetList<Item, NetRef<Item>>) allFarmer.items)
        {
          if (obj != null)
            obj.HasBeenInInventory = true;
        }
      }
      Game1.recalculateLostBookCount();
      Utility.iterateChestsAndStorage((Action<Item>) (item => item.HasBeenInInventory = true));
      foreach (TerrainFeature terrainFeature in Game1.getLocationFromName("Greenhouse").terrainFeatures.Values)
      {
        if (terrainFeature is HoeDirt)
          ((HoeDirt) terrainFeature).isGreenhouseDirt.Value = true;
      }
      Game1.hasApplied1_4_UpdateChanges = true;
    }

    public static void applySaveFix(SaveGame.SaveFixes save_fix)
    {
      switch (save_fix)
      {
        case SaveGame.SaveFixes.StoredBigCraftablesStackFix:
          Utility.iterateChestsAndStorage((Action<Item>) (item =>
          {
            if (!(item is Object))
              return;
            Object @object = item as Object;
            if (!(bool) (NetFieldBase<bool, NetBool>) @object.bigCraftable || @object.Stack != 0)
              return;
            @object.Stack = 1;
          }));
          break;
        case SaveGame.SaveFixes.PorchedCabinBushesFix:
          using (List<Building>.Enumerator enumerator = Game1.getFarm().buildings.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Building current = enumerator.Current;
              if ((int) (NetFieldBase<int, NetInt>) current.daysOfConstructionLeft <= 0 && current.indoors.Value is Cabin)
                current.removeOverlappingBushes((GameLocation) Game1.getFarm());
            }
            break;
          }
        case SaveGame.SaveFixes.ChangeObeliskFootprintHeight:
          using (List<Building>.Enumerator enumerator = Game1.getFarm().buildings.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Building current = enumerator.Current;
              if (current.buildingType.Value.Contains("Obelisk"))
              {
                current.tilesHigh.Value = 2;
                ++current.tileY.Value;
              }
            }
            break;
          }
        case SaveGame.SaveFixes.CreateStorageDressers:
          Utility.iterateChestsAndStorage((Action<Item>) (item =>
          {
            if (!(item is Clothing))
              return;
            item.Category = -100;
          }));
          List<DecoratableLocation> decoratableLocationList = new List<DecoratableLocation>();
          foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
          {
            if (location is DecoratableLocation)
              decoratableLocationList.Add(location as DecoratableLocation);
          }
          foreach (Building building in Game1.getFarm().buildings)
          {
            if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors != (NetRef<GameLocation>) null)
            {
              GameLocation indoors = (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors;
              if (indoors is DecoratableLocation)
                decoratableLocationList.Add(indoors as DecoratableLocation);
            }
          }
          using (List<DecoratableLocation>.Enumerator enumerator = decoratableLocationList.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              DecoratableLocation current = enumerator.Current;
              List<Furniture> furnitureList = new List<Furniture>();
              for (int index = 0; index < current.furniture.Count; ++index)
              {
                Furniture furniture = current.furniture[index];
                if (furniture.ParentSheetIndex == 704 || furniture.ParentSheetIndex == 709 || furniture.ParentSheetIndex == 714 || furniture.ParentSheetIndex == 719)
                {
                  StorageFurniture storageFurniture = new StorageFurniture(furniture.ParentSheetIndex, furniture.TileLocation, (int) (NetFieldBase<int, NetInt>) furniture.currentRotation);
                  furnitureList.Add((Furniture) storageFurniture);
                  current.furniture.RemoveAt(index);
                  --index;
                }
              }
              for (int index = 0; index < furnitureList.Count; ++index)
                current.furniture.Add(furnitureList[index]);
            }
            break;
          }
        case SaveGame.SaveFixes.InferPreserves:
          int[] preserve_item_indices = new int[4]
          {
            350,
            348,
            344,
            342
          };
          string[] suffixes = new string[3]
          {
            " Juice",
            " Wine",
            " Jelly"
          };
          Object.PreserveType[] suffix_preserve_types = new Object.PreserveType[3]
          {
            Object.PreserveType.Juice,
            Object.PreserveType.Wine,
            Object.PreserveType.Jelly
          };
          string[] prefixes = new string[1]{ "Pickled " };
          Object.PreserveType[] prefix_preserve_types = new Object.PreserveType[1]
          {
            Object.PreserveType.Pickle
          };
          Utility.iterateAllItems((Action<Item>) (item =>
          {
            if (!(item is Object) || !Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex) || !((IEnumerable<int>) preserve_item_indices).Contains<int>(item.ParentSheetIndex) || (item as Object).preserve.Value.HasValue)
              return;
            for (int index = 0; index < suffixes.Length; ++index)
            {
              string str1 = suffixes[index];
              if (item.Name.EndsWith(str1))
              {
                string str2 = item.Name.Substring(0, item.Name.Length - str1.Length);
                int num = -1;
                foreach (int key in (IEnumerable<int>) Game1.objectInformation.Keys)
                {
                  if (Game1.objectInformation[key].Substring(0, Game1.objectInformation[key].IndexOf('/')).Equals(str2))
                  {
                    num = key;
                    break;
                  }
                }
                if (num >= 0)
                {
                  (item as Object).preservedParentSheetIndex.Value = num;
                  (item as Object).preserve.Value = new Object.PreserveType?(suffix_preserve_types[index]);
                  return;
                }
              }
            }
            for (int index = 0; index < prefixes.Length; ++index)
            {
              string str3 = prefixes[index];
              if (item.Name.StartsWith(str3))
              {
                string str4 = item.Name.Substring(str3.Length);
                int num = -1;
                foreach (int key in (IEnumerable<int>) Game1.objectInformation.Keys)
                {
                  if (Game1.objectInformation[key].Substring(0, Game1.objectInformation[key].IndexOf('/')).Equals(str4))
                  {
                    num = key;
                    break;
                  }
                }
                if (num >= 0)
                {
                  (item as Object).preservedParentSheetIndex.Value = num;
                  (item as Object).preserve.Value = new Object.PreserveType?(prefix_preserve_types[index]);
                  break;
                }
              }
            }
          }));
          break;
        case SaveGame.SaveFixes.TransferHatSkipHairFlag:
          Utility.iterateAllItems((Action<Item>) (item =>
          {
            if (!(item is StardewValley.Objects.Hat))
              return;
            StardewValley.Objects.Hat hat = item as StardewValley.Objects.Hat;
            if (!hat.skipHairDraw)
              return;
            hat.hairDrawType.Set(0);
            hat.skipHairDraw = false;
          }));
          break;
        case SaveGame.SaveFixes.RevealSecretNoteItemTastes:
          Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\SecretNotes");
          for (int key = 0; key < 21; ++key)
          {
            if (dictionary.ContainsKey(key) && Game1.player.secretNotesSeen.Contains(key))
              Utility.ParseGiftReveals(dictionary[key]);
          }
          break;
        case SaveGame.SaveFixes.TransferHoneyTypeToPreserves:
          new int[1][0] = 340;
          Utility.iterateAllItems((Action<Item>) (item =>
          {
            if (!(item is Object) || !Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex) || item.ParentSheetIndex != 340 || (item as Object).preservedParentSheetIndex.Value > 0)
              return;
            if ((item as Object).honeyType.Value.HasValue && (item as Object).honeyType.Value.Value >= ~Object.HoneyType.Wild)
            {
              (item as Object).preservedParentSheetIndex.Value = (int) (item as Object).honeyType.Value.Value;
            }
            else
            {
              (item as Object).honeyType.Value = new Object.HoneyType?(Object.HoneyType.Wild);
              (item as Object).preservedParentSheetIndex.Value = -1;
            }
          }));
          break;
        case SaveGame.SaveFixes.TransferNoteBlockScale:
          Utility.iterateAllItems((Action<Item>) (item =>
          {
            if (!(item is Object) || !Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex) || item.ParentSheetIndex != 363 && item.ParentSheetIndex != 464)
              return;
            (item as Object).preservedParentSheetIndex.Value = (int) (item as Object).scale.X;
          }));
          break;
        case SaveGame.SaveFixes.FixCropHarvestAmountsAndInferSeedIndex:
          Utility.iterateAllCrops((Action<Crop>) (crop => crop.ResetCropYield()));
          break;
        case SaveGame.SaveFixes.Level9PuddingFishingRecipe2:
        case SaveGame.SaveFixes.Level9PuddingFishingRecipe3:
          if (Game1.player.cookingRecipes.ContainsKey("Ocean Mineral Pudding"))
            Game1.player.cookingRecipes.Remove("Ocean Mineral Pudding");
          if (Game1.player.fishingLevel.Value < 9 || Game1.player.cookingRecipes.ContainsKey("Seafoam Pudding"))
            break;
          Game1.player.cookingRecipes.Add("Seafoam Pudding", 0);
          break;
        case SaveGame.SaveFixes.quarryMineBushes:
          GameLocation locationFromName1 = Game1.getLocationFromName("Mountain");
          locationFromName1.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(new Vector2(101f, 18f), 1, locationFromName1));
          locationFromName1.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(new Vector2(104f, 21f), 0, locationFromName1));
          locationFromName1.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(new Vector2(105f, 18f), 0, locationFromName1));
          break;
        case SaveGame.SaveFixes.MissingQisChallenge:
          using (IEnumerator<Farmer> enumerator = Game1.getAllFarmers().GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Farmer current = enumerator.Current;
              if (current.mailReceived.Contains("skullCave") && !current.hasQuest(20) && !current.hasOrWillReceiveMail("QiChallengeComplete"))
                current.addQuest(20);
            }
            break;
          }
        case SaveGame.SaveFixes.BedsToFurniture:
          List<GameLocation> gameLocationList1 = new List<GameLocation>();
          gameLocationList1.Add(Game1.getLocationFromName("FarmHouse"));
          foreach (Building building in Game1.getFarm().buildings)
          {
            if (building.indoors.Value != null & building.indoors.Value is FarmHouse)
              gameLocationList1.Add((GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors);
          }
          using (List<GameLocation>.Enumerator enumerator = gameLocationList1.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              if (enumerator.Current is FarmHouse current)
              {
                for (int index1 = 0; index1 < current.map.Layers[0].TileWidth; ++index1)
                {
                  for (int index2 = 0; index2 < current.map.Layers[0].TileHeight; ++index2)
                  {
                    if (current.doesTileHaveProperty(index1, index2, "DefaultBedPosition", "Back") != null)
                    {
                      if (current.upgradeLevel == 0)
                      {
                        current.furniture.Add((Furniture) new BedFurniture(BedFurniture.DEFAULT_BED_INDEX, new Vector2((float) index1, (float) index2)));
                      }
                      else
                      {
                        int which = BedFurniture.DOUBLE_BED_INDEX;
                        if (!current.owner.activeDialogueEvents.ContainsKey("pennyRedecorating"))
                        {
                          if (current.owner.mailReceived.Contains("pennyQuilt0"))
                            which = 2058;
                          if (current.owner.mailReceived.Contains("pennyQuilt1"))
                            which = 2064;
                          if (current.owner.mailReceived.Contains("pennyQuilt2"))
                            which = 2070;
                        }
                        current.furniture.Add((Furniture) new BedFurniture(which, new Vector2((float) index1, (float) index2)));
                      }
                    }
                  }
                }
              }
            }
            break;
          }
        case SaveGame.SaveFixes.ChildBedsToFurniture:
          List<GameLocation> gameLocationList2 = new List<GameLocation>();
          gameLocationList2.Add(Game1.getLocationFromName("FarmHouse"));
          foreach (Building building in Game1.getFarm().buildings)
          {
            if (building.indoors.Value != null & building.indoors.Value is FarmHouse)
              gameLocationList2.Add((GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors);
          }
          using (List<GameLocation>.Enumerator enumerator = gameLocationList2.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              if (enumerator.Current is FarmHouse current)
              {
                for (int index3 = 0; index3 < current.map.Layers[0].TileWidth; ++index3)
                {
                  for (int index4 = 0; index4 < current.map.Layers[0].TileHeight; ++index4)
                  {
                    if (current.doesTileHaveProperty(index3, index4, "DefaultChildBedPosition", "Back") != null)
                      current.furniture.Add((Furniture) new BedFurniture(BedFurniture.CHILD_BED_INDEX, new Vector2((float) index3, (float) index4)));
                  }
                }
              }
            }
            break;
          }
        case SaveGame.SaveFixes.ModularizeFarmStructures:
          Game1.getFarm().AddModularShippingBin();
          break;
        case SaveGame.SaveFixes.FixFlooringFlags:
          Utility.ForAllLocations((Action<GameLocation>) (location =>
          {
            foreach (TerrainFeature terrainFeature in location.terrainFeatures.Values)
            {
              if (terrainFeature is Flooring)
                (terrainFeature as Flooring).ApplyFlooringFlags();
            }
          }));
          break;
        case SaveGame.SaveFixes.AddBugSteakRecipe:
          if (Game1.player.combatLevel.Value < 2 || Game1.player.craftingRecipes.ContainsKey("Bug Steak"))
            break;
          Game1.player.craftingRecipes.Add("Bug Steak", 0);
          break;
        case SaveGame.SaveFixes.AddBirdie:
          Game1.addBirdieIfNecessary();
          break;
        case SaveGame.SaveFixes.AddTownBush:
          if (!(Game1.getLocationFromName("Town") is Town locationFromName2))
            break;
          Vector2 tileLocation = new Vector2(61f, 93f);
          if (locationFromName2.getLargeTerrainFeatureAt((int) tileLocation.X, (int) tileLocation.Y) != null)
            break;
          locationFromName2.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(tileLocation, 2, (GameLocation) locationFromName2));
          break;
        case SaveGame.SaveFixes.AddNewRingRecipes1_5:
          if (Game1.player.combatLevel.Value >= 7 && !Game1.player.craftingRecipes.ContainsKey("Thorns Ring"))
            Game1.player.craftingRecipes.Add("Thorns Ring", 0);
          if (Game1.player.miningLevel.Value < 4 || Game1.player.craftingRecipes.ContainsKey("Glowstone Ring"))
            break;
          Game1.player.craftingRecipes.Add("Glowstone Ring", 0);
          break;
        case SaveGame.SaveFixes.ResetForges:
          Utility.iterateAllItems((Action<Item>) (item =>
          {
            if (!(item is MeleeWeapon))
              return;
            (item as MeleeWeapon).RecalculateAppliedForges();
          }));
          break;
        case SaveGame.SaveFixes.AddSquidInkRavioli:
          if (Game1.player.combatLevel.Value < 9 || Game1.player.cookingRecipes.ContainsKey("Squid Ink Ravioli"))
            break;
          Game1.player.cookingRecipes.Add("Squid Ink Ravioli", 0);
          break;
        case SaveGame.SaveFixes.MakeDarkSwordVampiric:
          Utility.iterateAllItems((Action<Item>) (item =>
          {
            if (!(item is MeleeWeapon) || (item as MeleeWeapon).InitialParentTileIndex != 2)
              return;
            (item as MeleeWeapon).AddEnchantment((BaseEnchantment) new VampiricEnchantment());
          }));
          break;
        case SaveGame.SaveFixes.FixRingSheetIndex:
          Utility.iterateAllItems((Action<Item>) (item =>
          {
            if (!(item is Ring) || item.ParentSheetIndex != -1)
              return;
            item.ParentSheetIndex = (item as Ring).indexInTileSheet.Value;
          }));
          break;
        case SaveGame.SaveFixes.FixBeachFarmBushes:
          if (Game1.whichFarm != 6)
            break;
          Farm farm = Game1.getFarm();
          Vector2[] vector2Array = new Vector2[4]
          {
            new Vector2(77f, 4f),
            new Vector2(78f, 3f),
            new Vector2(83f, 4f),
            new Vector2(83f, 3f)
          };
          foreach (Vector2 vector2 in vector2Array)
          {
            foreach (LargeTerrainFeature largeTerrainFeature in farm.largeTerrainFeatures)
            {
              if (largeTerrainFeature.tilePosition.Value == vector2)
              {
                if (largeTerrainFeature is StardewValley.TerrainFeatures.Bush)
                {
                  if (largeTerrainFeature is StardewValley.TerrainFeatures.Bush bush)
                  {
                    bush.tilePosition.Value = new Vector2(bush.tilePosition.X, bush.tilePosition.Y + 1f);
                    break;
                  }
                  break;
                }
                break;
              }
            }
          }
          break;
        case SaveGame.SaveFixes.AddCampfireKit:
          if (Game1.player.foragingLevel.Value < 9 || Game1.player.craftingRecipes.ContainsKey("Cookout Kit"))
            break;
          Game1.player.craftingRecipes.Add("Cookout Kit", 0);
          break;
        case SaveGame.SaveFixes.OstrichIncubatorFragility:
          Utility.iterateAllItems((Action<Item>) (item =>
          {
            if (!(item is Object) || (item as Object).Fragility != 2 || !(item.Name == "Ostrich Incubator"))
              return;
            (item as Object).Fragility = 0;
          }));
          break;
        case SaveGame.SaveFixes.FixBotchedBundleData:
          Dictionary<string, string> data = new Dictionary<string, string>();
          foreach (string key in Game1.netWorldState.Value.BundleData.Keys)
          {
            List<string> values = new List<string>((IEnumerable<string>) Game1.netWorldState.Value.BundleData[key].Split('/'));
            int result = 0;
            while (values.Count > 4 && !int.TryParse(values[values.Count - 1], out result))
            {
              string str = values[values.Count - 1];
              if (!char.IsDigit(str[str.Length - 1]) || !str.Contains(":") || !str.Contains("\\"))
                values.RemoveAt(values.Count - 1);
              else
                break;
            }
            data[key] = string.Join("/", (IEnumerable<string>) values);
          }
          Game1.netWorldState.Value.SetBundleData(data);
          break;
        case SaveGame.SaveFixes.LeoChildrenFix:
          Utility.FixChildNameCollisions();
          break;
        case SaveGame.SaveFixes.Leo6HeartGermanFix:
          if (!Utility.HasAnyPlayerSeenEvent(6497428) || Game1.MasterPlayer.hasOrWillReceiveMail("leoMoved"))
            break;
          Game1.addMailForTomorrow("leoMoved", true, true);
          Game1.player.team.requestLeoMove.Fire();
          break;
        case SaveGame.SaveFixes.BirdieQuestRemovedFix:
          using (IEnumerator<Farmer> enumerator = Game1.getAllFarmers().GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Farmer current = enumerator.Current;
              if (current.hasQuest(130))
              {
                foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) current.questLog)
                {
                  if ((int) (NetFieldBase<int, NetInt>) quest.id == 130)
                    quest.canBeCancelled.Value = true;
                }
              }
              if (current.hasOrWillReceiveMail("birdieQuestBegun") && !current.hasOrWillReceiveMail("birdieQuestFinished") && !current.hasQuest(130))
                current.addQuest(130);
            }
            break;
          }
        case SaveGame.SaveFixes.SkippedSummit:
          if (!Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
            break;
          using (IEnumerator<Farmer> enumerator = Game1.getAllFarmers().GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Farmer current = enumerator.Current;
              if (current.mailReceived.Contains("Summit_event") && !current.songsHeard.Contains("end_credits"))
                current.mailReceived.Remove("Summit_event");
            }
            break;
          }
      }
    }

    public static void recalculateLostBookCount()
    {
      int val1 = 0;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.archaeologyFound.ContainsKey(102) && allFarmer.archaeologyFound[102][0] > 0)
        {
          val1 = Math.Max(val1, allFarmer.archaeologyFound[102][0]);
          if (!allFarmer.mailForTomorrow.Contains("lostBookFound%&NL&%"))
            allFarmer.mailForTomorrow.Add("lostBookFound%&NL&%");
        }
      }
      Game1.netWorldState.Value.LostBooksFound.Value = val1;
    }

    public static void MarkFloorChestAsCollectedIfNecessary(int floor_number)
    {
      if (MineShaft.permanentMineChanges == null || !MineShaft.permanentMineChanges.ContainsKey(floor_number) || MineShaft.permanentMineChanges[floor_number].chestsLeft > 0)
        return;
      Game1.player.chestConsumedMineLevels[floor_number] = true;
    }

    public static void pauseThenDoFunction(int pauseTime, Game1.afterFadeFunction function)
    {
      Game1.afterPause = function;
      Game1.pauseThenDoFunctionTimer = pauseTime;
    }

    public static string dayOrNight()
    {
      string str = "_day";
      if (DateTime.Now.TimeOfDay.TotalHours >= (double) (int) (1.75 * Math.Sin(2.0 * Math.PI / 365.0 * (double) DateTime.Now.DayOfYear - 79.0) + 18.75) || DateTime.Now.TimeOfDay.TotalHours < 5.0)
        str = "_night";
      return str;
    }

    protected internal virtual LocalizedContentManager CreateContentManager(
      IServiceProvider serviceProvider,
      string rootDirectory)
    {
      return new LocalizedContentManager(serviceProvider, rootDirectory);
    }

    public void Instance_LoadContent() => this.LoadContent();

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      Game1.content = this.CreateContentManager(this.Content.ServiceProvider, this.Content.RootDirectory);
      this.xTileContent = this.CreateContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
      Game1.mapDisplayDevice = (IDisplayDevice) new XnaDisplayDevice((ContentManager) Game1.content, this.GraphicsDevice);
      CraftingRecipe.InitShared();
      Critter.InitShared();
      Game1.spriteBatch = new SpriteBatch(this.GraphicsDevice);
      Game1.concessionsSpriteSheet = Game1.content.Load<Texture2D>("LooseSprites\\Concessions");
      Game1.birdsSpriteSheet = Game1.content.Load<Texture2D>("LooseSprites\\birds");
      Game1.daybg = Game1.content.Load<Texture2D>("LooseSprites\\daybg");
      Game1.nightbg = Game1.content.Load<Texture2D>("LooseSprites\\nightbg");
      Game1.menuTexture = Game1.content.Load<Texture2D>("Maps\\MenuTiles");
      Game1.uncoloredMenuTexture = Game1.content.Load<Texture2D>("Maps\\MenuTilesUncolored");
      Game1.lantern = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\lantern");
      Game1.windowLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\windowLight");
      Game1.sconceLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\sconceLight");
      Game1.cauldronLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\greenLight");
      Game1.indoorWindowLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\indoorWindowLight");
      Game1.shadowTexture = Game1.content.Load<Texture2D>("LooseSprites\\shadow");
      Game1.mouseCursors = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
      Game1.mouseCursors2 = Game1.content.Load<Texture2D>("LooseSprites\\Cursors2");
      Game1.giftboxTexture = Game1.content.Load<Texture2D>("LooseSprites\\Giftbox");
      Game1.controllerMaps = Game1.content.Load<Texture2D>("LooseSprites\\ControllerMaps");
      Game1.animations = Game1.content.Load<Texture2D>("TileSheets\\animations");
      Game1.achievements = Game1.content.Load<Dictionary<int, string>>("Data\\Achievements");
      Game1.fadeToBlackRect = new Texture2D(this.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
      Microsoft.Xna.Framework.Color[] data1 = new Microsoft.Xna.Framework.Color[1]
      {
        Microsoft.Xna.Framework.Color.White
      };
      Game1.fadeToBlackRect.SetData<Microsoft.Xna.Framework.Color>(data1);
      Game1.dialogueWidth = Math.Min(1024, Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Width - 256);
      NameSelect.load();
      Game1.NPCGiftTastes = (IDictionary<string, string>) Game1.content.Load<Dictionary<string, string>>("Data\\NPCGiftTastes");
      Microsoft.Xna.Framework.Color[] data2 = new Microsoft.Xna.Framework.Color[1];
      Game1.staminaRect = new Texture2D(this.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
      Game1.onScreenMenus.Clear();
      Game1.onScreenMenus.Add((IClickableMenu) new Toolbar());
      for (int index = 0; index < data2.Length; ++index)
        data2[index] = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      Game1.staminaRect.SetData<Microsoft.Xna.Framework.Color>(data2);
      Game1.saveOnNewDay = true;
      Game1.littleEffect = new Texture2D(this.GraphicsDevice, 4, 4, false, SurfaceFormat.Color);
      Microsoft.Xna.Framework.Color[] data3 = new Microsoft.Xna.Framework.Color[16];
      for (int index = 0; index < data3.Length; ++index)
        data3[index] = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
      Game1.littleEffect.SetData<Microsoft.Xna.Framework.Color>(data3);
      for (int index = 0; index < 70; ++index)
        Game1.rainDrops[index] = new RainDrop(Game1.random.Next(Game1.viewport.Width), Game1.random.Next(Game1.viewport.Height), Game1.random.Next(4), Game1.random.Next(70));
      Game1.dayTimeMoneyBox = new DayTimeMoneyBox();
      Game1.onScreenMenus.Add((IClickableMenu) Game1.dayTimeMoneyBox);
      Game1.buffsDisplay = new BuffsDisplay();
      Game1.onScreenMenus.Add((IClickableMenu) Game1.buffsDisplay);
      Game1.dialogueFont = Game1.content.Load<SpriteFont>("Fonts\\SpriteFont1");
      Game1.dialogueFont.LineSpacing = 42;
      Game1.smallFont = Game1.content.Load<SpriteFont>("Fonts\\SmallFont");
      Game1.smallFont.LineSpacing = 26;
      Game1.tinyFont = Game1.content.Load<SpriteFont>("Fonts\\tinyFont");
      Game1.tinyFontBorder = Game1.content.Load<SpriteFont>("Fonts\\tinyFontBorder");
      Game1.objectSpriteSheet = Game1.content.Load<Texture2D>("Maps\\springobjects");
      Game1.cropSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\crops");
      Game1.emoteSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\emotes");
      Game1.debrisSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\debris");
      Game1.bigCraftableSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\Craftables");
      Game1.rainTexture = Game1.content.Load<Texture2D>("TileSheets\\rain");
      Game1.buffsIcons = Game1.content.Load<Texture2D>("TileSheets\\BuffsIcons");
      Game1.objectInformation = (IDictionary<int, string>) Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
      Game1.clothingInformation = (IDictionary<int, string>) Game1.content.Load<Dictionary<int, string>>("Data\\ClothingInformation");
      Game1.objectContextTags = (IDictionary<string, string>) Game1.content.Load<Dictionary<string, string>>("Data\\ObjectContextTags");
      Game1.bigCraftablesInformation = (IDictionary<int, string>) Game1.content.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation");
      if (Game1.gameMode == (byte) 4)
      {
        Game1.fadeToBlackAlpha = -0.5f;
        Game1.fadeIn = true;
      }
      if (Game1.random.NextDouble() < 0.7)
      {
        Game1.isDebrisWeather = true;
        Game1.populateDebrisWeatherArray();
      }
      FarmerRenderer.hairStylesTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hairstyles");
      FarmerRenderer.shirtsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\shirts");
      FarmerRenderer.pantsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\pants");
      FarmerRenderer.hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");
      FarmerRenderer.accessoriesTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\accessories");
      Furniture.furnitureTexture = Game1.content.Load<Texture2D>("TileSheets\\furniture");
      Furniture.furnitureFrontTexture = Game1.content.Load<Texture2D>("TileSheets\\furnitureFront");
      MapSeat.mapChairTexture = Game1.content.Load<Texture2D>("TileSheets\\ChairTiles");
      SpriteText.spriteTexture = Game1.content.Load<Texture2D>("LooseSprites\\font_bold");
      SpriteText.coloredTexture = Game1.content.Load<Texture2D>("LooseSprites\\font_colored");
      Tool.weaponsTexture = Game1.content.Load<Texture2D>("TileSheets\\weapons");
      Projectile.projectileSheet = Game1.content.Load<Texture2D>("TileSheets\\Projectiles");
      Game1._shortDayDisplayName[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3042");
      Game1._shortDayDisplayName[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3043");
      Game1._shortDayDisplayName[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3044");
      Game1._shortDayDisplayName[3] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3045");
      Game1._shortDayDisplayName[4] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3046");
      Game1._shortDayDisplayName[5] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3047");
      Game1._shortDayDisplayName[6] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3048");
      Game1.netWorldState = new NetRoot<IWorldState>((IWorldState) new NetWorldState());
      Game1.resetPlayer();
      Game1.setGameMode((byte) 0);
    }

    public static void resetPlayer() => Game1.player = new Farmer(new FarmerSprite((string) null), new Vector2(192f, 192f), 1, "", Farmer.initialTools(), true);

    public static void resetVariables()
    {
      Game1.xLocationAfterWarp = 0;
      Game1.yLocationAfterWarp = 0;
      Game1.gameTimeInterval = 0;
      Game1.currentQuestionChoice = 0;
      Game1.currentDialogueCharacterIndex = 0;
      Game1.dialogueTypingInterval = 0;
      Game1.dayOfMonth = 0;
      Game1.year = 1;
      Game1.timeOfDay = 600;
      Game1.timeOfDayAfterFade = -1;
      Game1.numberOfSelectedItems = -1;
      Game1.priceOfSelectedItem = 0;
      Game1.currentWallpaper = 0;
      Game1.farmerWallpaper = 22;
      Game1.wallpaperPrice = 75;
      Game1.currentFloor = 3;
      Game1.FarmerFloor = 29;
      Game1.floorPrice = 75;
      Game1.facingDirectionAfterWarp = 0;
      Game1.dialogueWidth = 0;
      Game1.menuChoice = 0;
      Game1.tvStation = -1;
      Game1.currentBillboard = 0;
      Game1.facingDirectionAfterWarp = 0;
      Game1.tmpTimeOfDay = 0;
      Game1.percentageToWinStardewHero = 70;
      Game1.mouseClickPolling = 0;
      Game1.weatherIcon = 0;
      Game1.hitShakeTimer = 0;
      Game1.staminaShakeTimer = 0;
      Game1.pauseThenDoFunctionTimer = 0;
      Game1.weatherForTomorrow = 0;
      Game1.currentSongIndex = 3;
    }

    public static void playSound(string cueName)
    {
      if (Game1.soundBank == null)
        return;
      try
      {
        Game1.soundBank.PlayCue(cueName);
      }
      catch (Exception ex)
      {
        Game1.debugOutput = Game1.parseText(ex.Message);
        Console.WriteLine((object) ex);
      }
    }

    public static void playSoundPitched(string cueName, int pitch)
    {
      if (Game1.soundBank == null)
        return;
      try
      {
        ICue cue = Game1.soundBank.GetCue(cueName);
        cue.SetVariable("Pitch", pitch);
        cue.Play();
        try
        {
          if (cue.IsPitchBeingControlledByRPC)
            return;
          cue.Pitch = Utility.Lerp(-1f, 1f, (float) pitch / 2400f);
        }
        catch (Exception ex)
        {
        }
      }
      catch (Exception ex)
      {
        Game1.debugOutput = Game1.parseText(ex.Message);
        Console.WriteLine((object) ex);
      }
    }

    public static void setRichPresence(string friendlyName, object argument = null)
    {
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(friendlyName))
      {
        case 200649126:
          if (!(friendlyName == "location"))
            break;
          Game1.debugPresenceString = string.Format("At {0}", argument);
          break;
        case 819128320:
          if (!(friendlyName == "giantcrop"))
            break;
          Game1.debugPresenceString = string.Format("Just harvested a Giant {0}", argument);
          break;
        case 1266391031:
          if (!(friendlyName == "wedding"))
            break;
          Game1.debugPresenceString = string.Format("Getting married to {0}", argument);
          break;
        case 2353551851:
          if (!(friendlyName == "menus"))
            break;
          Game1.debugPresenceString = "In menus";
          break;
        case 2448968664:
          if (!(friendlyName == "minigame"))
            break;
          Game1.debugPresenceString = string.Format("Playing {0}", argument);
          break;
        case 2899391285:
          if (!(friendlyName == "fishing"))
            break;
          Game1.debugPresenceString = string.Format("Fishing at {0}", argument);
          break;
        case 3532452870:
          if (!(friendlyName == "earnings"))
            break;
          Game1.debugPresenceString = string.Format("Made {0}g last night", argument);
          break;
        case 3801893813:
          if (!(friendlyName == "festival"))
            break;
          Game1.debugPresenceString = string.Format("At {0}", argument);
          break;
      }
    }

    public static void GenerateBundles(Game1.BundleType bundle_type, bool use_seed = true)
    {
      Random rng = !use_seed ? new Random() : new Random((int) Game1.uniqueIDForThisGame * 9);
      if (bundle_type == Game1.BundleType.Remixed)
      {
        Dictionary<string, string> data = new BundleGenerator().Generate("Data\\RandomBundles", rng);
        Game1.netWorldState.Value.SetBundleData(data);
      }
      else
        Game1.netWorldState.Value.SetBundleData(Game1.content.LoadBase<Dictionary<string, string>>("Data\\Bundles"));
    }

    public void SetNewGameOption<T>(string key, T val) => this.newGameSetupOptions[key] = (object) val;

    public T GetNewGameOption<T>(string key) => !this.newGameSetupOptions.ContainsKey(key) ? default (T) : (T) this.newGameSetupOptions[key];

    public static void loadForNewGame(bool loadedGame = false)
    {
      if (Game1.startingGameSeed.HasValue)
        Game1.uniqueIDForThisGame = Game1.startingGameSeed.Value;
      Game1.specialCurrencyDisplay = new SpecialCurrencyDisplay();
      Game1.flushLocationLookup();
      Game1.locations.Clear();
      Game1.mailbox.Clear();
      Game1.currentLightSources.Clear();
      if (Game1.dealerCalicoJackTotal != null)
        Game1.dealerCalicoJackTotal.Clear();
      Game1.questionChoices.Clear();
      Game1.hudMessages.Clear();
      Game1.weddingToday = false;
      Game1.timeOfDay = 600;
      Game1.currentSeason = "spring";
      if (!loadedGame)
        Game1.year = 1;
      Game1.dayOfMonth = 0;
      Game1.pickingTool = false;
      Game1.isQuestion = false;
      Game1.nonWarpFade = false;
      Game1.particleRaining = false;
      Game1.newDay = false;
      Game1.inMine = false;
      Game1.menuUp = false;
      Game1.eventUp = false;
      Game1.viewportFreeze = false;
      Game1.eventOver = false;
      Game1.nameSelectUp = false;
      Game1.screenGlow = false;
      Game1.screenGlowHold = false;
      Game1.screenGlowUp = false;
      Game1.progressBar = false;
      Game1.isRaining = false;
      Game1.killScreen = false;
      Game1.coopDwellerBorn = false;
      Game1.messagePause = false;
      Game1.isDebrisWeather = false;
      Game1.boardingBus = false;
      Game1.listeningForKeyControlDefinitions = false;
      Game1.weddingToday = false;
      Game1.exitToTitle = false;
      Game1.isRaining = false;
      Game1.dialogueUp = false;
      Game1.currentBillboard = 0;
      Game1.postExitToTitleCallback = (Action) null;
      Game1.displayHUD = true;
      Game1.messageAfterPause = "";
      Game1.fertilizer = "";
      Game1.samBandName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2156");
      Game1.slotResult = "";
      Game1.background = (Background) null;
      Game1.currentCursorTile = Vector2.Zero;
      if (!loadedGame)
        Game1.lastAppliedSaveFix = SaveGame.SaveFixes.SkippedSummit;
      Game1.resetVariables();
      Game1.chanceToRainTomorrow = 0.0;
      Game1.player.team.sharedDailyLuck.Value = 0.001;
      if (!loadedGame)
      {
        Game1.options = new Options();
        Game1.options.LoadDefaultOptions();
        Game1.initializeVolumeLevels();
      }
      Game1.game1.CheckGamepadMode();
      Game1.cropsOfTheWeek = Utility.cropsOfTheWeek();
      Game1.onScreenMenus.Add((IClickableMenu) (Game1.chatBox = new ChatBox()));
      Game1.outdoorLight = Microsoft.Xna.Framework.Color.White;
      Game1.ambientLight = Microsoft.Xna.Framework.Color.White;
      int parentSheetIndex = Game1.random.Next(194, 240);
      while (((IEnumerable<int>) Utility.getForbiddenDishesOfTheDay()).Contains<int>(parentSheetIndex))
        parentSheetIndex = Game1.random.Next(194, 240);
      int initialStack = Game1.random.Next(1, 4 + (Game1.random.NextDouble() < 0.08 ? 10 : 0));
      Game1.netWorldState.Value.DishOfTheDay.Value = new Object(Vector2.Zero, parentSheetIndex, initialStack);
      Game1.locations.Clear();
      Game1.locations.Add((GameLocation) new Farm("Maps\\" + Farm.getMapNameFromTypeInt(Game1.whichFarm), "Farm"));
      Game1.getFarm().BuildStartingCabins();
      Game1.forceSnapOnNextViewportUpdate = true;
      Game1.currentLocation = (GameLocation) new FarmHouse("Maps\\FarmHouse", "FarmHouse");
      Game1.currentLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
      Game1.locations.Add(Game1.currentLocation);
      if (Game1.whichFarm == 3 || Game1.getFarm().ShouldSpawnMountainOres())
      {
        for (int index = 0; index < 28; ++index)
          Game1.getFarm().doDailyMountainFarmUpdate();
      }
      else if (Game1.whichFarm == 5)
      {
        for (int index = 0; index < 10; ++index)
          Game1.getFarm().doDailyMountainFarmUpdate();
      }
      Game1.locations.Add((GameLocation) new FarmCave("Maps\\FarmCave", "FarmCave"));
      Game1.locations.Add((GameLocation) new Town("Maps\\Town", "Town"));
      Game1.locations.Add(new GameLocation("Maps\\JoshHouse", "JoshHouse"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\George", 0, 16, 32), new Vector2(1024f, 1408f), "JoshHouse", 0, "George", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\George")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Evelyn", 0, 16, 32), new Vector2(128f, 1088f), "JoshHouse", 1, "Evelyn", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Evelyn")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Alex", 0, 16, 32), new Vector2(1216f, 320f), "JoshHouse", 3, "Alex", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Alex")));
      Game1.locations.Add(new GameLocation("Maps\\HaleyHouse", "HaleyHouse"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Emily", 0, 16, 32), new Vector2(1024f, 320f), "HaleyHouse", 2, "Emily", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Emily")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Haley", 0, 16, 32), new Vector2(512f, 448f), "HaleyHouse", 1, "Haley", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Haley")));
      Game1.locations.Add(new GameLocation("Maps\\SamHouse", "SamHouse"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Jodi", 0, 16, 32), new Vector2(256f, 320f), "SamHouse", 0, "Jodi", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Jodi")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Sam", 0, 16, 32), new Vector2(1408f, 832f), "SamHouse", 1, "Sam", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Sam")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Vincent", 0, 16, 32), new Vector2(640f, 1472f), "SamHouse", 2, "Vincent", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Vincent")));
      Game1.addKentIfNecessary();
      Game1.locations.Add(new GameLocation("Maps\\Blacksmith", "Blacksmith"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Clint", 0, 16, 32), new Vector2(192f, 832f), "Blacksmith", 2, "Clint", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Clint")));
      Game1.locations.Add((GameLocation) new ManorHouse("Maps\\ManorHouse", "ManorHouse"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Lewis", 0, 16, 32), new Vector2(512f, 320f), "ManorHouse", 0, "Lewis", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Lewis")));
      Game1.locations.Add((GameLocation) new SeedShop("Maps\\SeedShop", "SeedShop"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Caroline", 0, 16, 32), new Vector2(1408f, 320f), "SeedShop", 2, "Caroline", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Caroline")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Abigail", 0, 16, 32), new Vector2(64f, 580f), "SeedShop", 3, "Abigail", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Abigail")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Pierre", 0, 16, 32), new Vector2(256f, 1088f), "SeedShop", 2, "Pierre", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Pierre")));
      Game1.locations.Add(new GameLocation("Maps\\Saloon", "Saloon"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Gus", 0, 16, 32), new Vector2(1152f, 384f), "Saloon", 2, "Gus", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Gus")));
      Game1.locations.Add(new GameLocation("Maps\\Trailer", "Trailer"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Pam", 0, 16, 32), new Vector2(960f, 256f), "Trailer", 2, "Pam", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Pam")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Penny", 0, 16, 32), new Vector2(256f, 576f), "Trailer", 1, "Penny", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Penny")));
      Game1.locations.Add(new GameLocation("Maps\\Hospital", "Hospital"));
      Game1.locations.Add(new GameLocation("Maps\\HarveyRoom", "HarveyRoom"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Harvey", 0, 16, 32), new Vector2(832f, 256f), "HarveyRoom", 1, "Harvey", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Harvey")));
      Game1.locations.Add((GameLocation) new Beach("Maps\\Beach", "Beach"));
      Game1.locations.Add(new GameLocation("Maps\\ElliottHouse", "ElliottHouse"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Elliott", 0, 16, 32), new Vector2(64f, 320f), "ElliottHouse", 0, "Elliott", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Elliott")));
      Game1.locations.Add((GameLocation) new Mountain("Maps\\Mountain", "Mountain"));
      Game1.locations.Add(new GameLocation("Maps\\ScienceHouse", "ScienceHouse"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Maru", 0, 16, 32), new Vector2(128f, 256f), "ScienceHouse", 3, "Maru", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Maru")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Robin", 0, 16, 32), new Vector2(1344f, 256f), "ScienceHouse", 1, "Robin", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Robin")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Demetrius", 0, 16, 32), new Vector2(1216f, 256f), "ScienceHouse", 1, "Demetrius", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Demetrius")));
      Game1.locations.Add(new GameLocation("Maps\\SebastianRoom", "SebastianRoom"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Sebastian", 0, 16, 32), new Vector2(640f, 576f), "SebastianRoom", 1, "Sebastian", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Sebastian")));
      GameLocation gameLocation = new GameLocation("Maps\\Tent", "Tent");
      Game1.locations.Add(gameLocation);
      gameLocation.addCharacter(new NPC(new AnimatedSprite("Characters\\Linus", 0, 16, 32), new Vector2(2f, 2f) * 64f, "Tent", 2, "Linus", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Linus")));
      Game1.locations.Add((GameLocation) new Forest("Maps\\Forest", "Forest"));
      Game1.locations.Add((GameLocation) new WizardHouse("Maps\\WizardHouse", "WizardHouse"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Wizard", 0, 16, 32), new Vector2(192f, 1088f), "WizardHouse", 2, "Wizard", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Wizard")));
      Game1.locations.Add(new GameLocation("Maps\\AnimalShop", "AnimalShop"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Marnie", 0, 16, 32), new Vector2(768f, 896f), "AnimalShop", 2, "Marnie", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Marnie")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Shane", 0, 16, 32), new Vector2(1600f, 384f), "AnimalShop", 3, "Shane", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Shane")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Jas", 0, 16, 32), new Vector2(256f, 384f), "AnimalShop", 2, "Jas", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Jas")));
      Game1.locations.Add(new GameLocation("Maps\\LeahHouse", "LeahHouse"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Leah", 0, 16, 32), new Vector2(192f, 448f), "LeahHouse", 3, "Leah", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Leah")));
      Game1.locations.Add((GameLocation) new BusStop("Maps\\BusStop", "BusStop"));
      Game1.locations.Add((GameLocation) new Mine("Maps\\Mine", "Mine"));
      Game1.locations[Game1.locations.Count - 1].objects.Add(new Vector2(27f, 8f), new Object(Vector2.Zero, 78));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Dwarf", 0, 16, 24), new Vector2(2752f, 384f), "Mine", 2, "Dwarf", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Dwarf"))
      {
        Breather = false
      });
      Game1.locations.Add((GameLocation) new Sewer("Maps\\Sewer", "Sewer"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Krobus", 0, 16, 24), new Vector2(31f, 17f) * 64f, "Sewer", 2, "Krobus", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Krobus")));
      Game1.locations.Add((GameLocation) new BugLand("Maps\\BugLand", "BugLand"));
      Game1.locations.Add((GameLocation) new Desert("Maps\\Desert", "Desert"));
      Game1.locations.Add((GameLocation) new Club("Maps\\Club", "Club"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\MrQi", 0, 16, 32), new Vector2(512f, 256f), "Club", 0, "Mister Qi", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\MrQi")));
      Game1.locations.Add(new GameLocation("Maps\\SandyHouse", "SandyHouse"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Sandy", 0, 16, 32), new Vector2(128f, 320f), "SandyHouse", 2, "Sandy", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Sandy")));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Bouncer", 0, 16, 32), new Vector2(1088f, 192f), "SandyHouse", 2, "Bouncer", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Bouncer")));
      Game1.locations.Add((GameLocation) new LibraryMuseum("Maps\\ArchaeologyHouse", "ArchaeologyHouse"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Gunther", 0, 16, 32), new Vector2(192f, 512f), "ArchaeologyHouse", 2, "Gunther", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Gunther")));
      Game1.locations.Add(new GameLocation("Maps\\WizardHouseBasement", "WizardHouseBasement"));
      Game1.locations.Add((GameLocation) new AdventureGuild("Maps\\AdventureGuild", "AdventureGuild"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Marlon", 0, 16, 32), new Vector2(320f, 704f), "AdventureGuild", 2, "Marlon", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Marlon")));
      Game1.locations.Add((GameLocation) new Woods("Maps\\Woods", "Woods"));
      Game1.locations.Add((GameLocation) new Railroad("Maps\\Railroad", "Railroad"));
      Game1.locations.Add(new GameLocation("Maps\\WitchSwamp", "WitchSwamp"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Henchman", 0, 16, 32), new Vector2(1280f, 1856f), "WitchSwamp", 2, "Henchman", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Henchman")));
      Game1.locations.Add(new GameLocation("Maps\\WitchHut", "WitchHut"));
      Game1.locations.Add(new GameLocation("Maps\\WitchWarpCave", "WitchWarpCave"));
      Game1.locations.Add((GameLocation) new Summit("Maps\\Summit", "Summit"));
      Game1.locations.Add((GameLocation) new FishShop("Maps\\FishShop", "FishShop"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\Willy", 0, 16, 32), new Vector2(320f, 256f), "FishShop", 2, "Willy", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Willy")));
      Game1.locations.Add(new GameLocation("Maps\\BathHouse_Entry", "BathHouse_Entry"));
      Game1.locations.Add(new GameLocation("Maps\\BathHouse_MensLocker", "BathHouse_MensLocker"));
      Game1.locations.Add(new GameLocation("Maps\\BathHouse_WomensLocker", "BathHouse_WomensLocker"));
      Game1.locations.Add((GameLocation) new BathHousePool("Maps\\BathHouse_Pool", "BathHouse_Pool"));
      Game1.locations.Add((GameLocation) new CommunityCenter("CommunityCenter"));
      Game1.locations.Add((GameLocation) new JojaMart("Maps\\JojaMart", "JojaMart"));
      Game1.locations.Add(new GameLocation("Maps\\Greenhouse", "Greenhouse"));
      Game1.locations.Add(new GameLocation("Maps\\SkullCave", "SkullCave"));
      Game1.locations.Add(new GameLocation("Maps\\Backwoods", "Backwoods"));
      Game1.locations.Add(new GameLocation("Maps\\Tunnel", "Tunnel"));
      Game1.locations.Add(new GameLocation("Maps\\Trailer_big", "Trailer_Big"));
      Game1.locations.Add((GameLocation) new Cellar("Maps\\Cellar", "Cellar"));
      for (int index = 1; index < (int) (NetFieldBase<int, NetInt>) Game1.netWorldState.Value.HighestPlayerLimit; ++index)
        Game1.locations.Add((GameLocation) new Cellar("Maps\\Cellar", "Cellar" + (index + 1).ToString()));
      Game1.locations.Add((GameLocation) new BeachNightMarket("Maps\\Beach-NightMarket", "BeachNightMarket"));
      Game1.locations.Add((GameLocation) new MermaidHouse("Maps\\MermaidHouse", "MermaidHouse"));
      Game1.locations.Add((GameLocation) new Submarine("Maps\\Submarine", "Submarine"));
      Game1.locations.Add((GameLocation) new AbandonedJojaMart("Maps\\AbandonedJojaMart", "AbandonedJojaMart"));
      Game1.locations.Add((GameLocation) new MovieTheater("Maps\\MovieTheater", "MovieTheater"));
      Game1.locations.Add(new GameLocation("Maps\\Sunroom", "Sunroom"));
      Game1.locations.Add((GameLocation) new BoatTunnel("Maps\\BoatTunnel", "BoatTunnel"));
      Game1.locations.Add((GameLocation) new IslandSouth("Maps\\Island_S", "IslandSouth"));
      Game1.locations.Add((GameLocation) new IslandSouthEast("Maps\\Island_SE", "IslandSouthEast"));
      Game1.locations.Add((GameLocation) new IslandSouthEastCave("Maps\\IslandSouthEastCave", "IslandSouthEastCave"));
      Game1.locations.Add((GameLocation) new IslandEast("Maps\\Island_E", "IslandEast"));
      Game1.locations.Add((GameLocation) new IslandWest("Maps\\Island_W", "IslandWest"));
      Game1.addBirdieIfNecessary();
      Game1.locations.Add((GameLocation) new IslandNorth("Maps\\Island_N", "IslandNorth"));
      Game1.locations.Add((GameLocation) new IslandHut("Maps\\Island_Hut", "IslandHut"));
      Game1.locations.Add((GameLocation) new IslandWestCave1("Maps\\IslandWestCave1", "IslandWestCave1"));
      Game1.locations.Add((GameLocation) new IslandLocation("Maps\\IslandNorthCave1", "IslandNorthCave1"));
      Game1.locations.Add((GameLocation) new IslandFieldOffice("Maps\\Island_FieldOffice", "IslandFieldOffice"));
      Game1.locations.Add((GameLocation) new IslandFarmHouse("Maps\\IslandFarmHouse", "IslandFarmHouse"));
      Game1.locations.Add((GameLocation) new IslandLocation("Maps\\Island_CaptainRoom", "CaptainRoom"));
      Game1.locations.Add((GameLocation) new IslandShrine("Maps\\Island_Shrine", "IslandShrine"));
      Game1.locations.Add((GameLocation) new IslandFarmCave("Maps\\Island_FarmCave", "IslandFarmCave"));
      Game1.locations.Add((GameLocation) new Caldera("Maps\\Caldera", "Caldera"));
      Game1.locations.Add(new GameLocation("Maps\\LeoTreeHouse", "LeoTreeHouse"));
      Game1.locations.Add((GameLocation) new IslandLocation("Maps\\QiNutRoom", "QiNutRoom"));
      Game1.locations[Game1.locations.Count - 1].addCharacter(new NPC(new AnimatedSprite("Characters\\MrQi", 0, 16, 32), new Vector2(448f, 256f), "QiNutRoom", 0, "Mister Qi", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\MrQi")));
      if (!loadedGame)
      {
        foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
        {
          if (location is IslandLocation)
            (location as IslandLocation).AddAdditionalWalnutBushes();
        }
      }
      Game1.AddModNPCs();
      NPC.populateRoutesFromLocationToLocationList();
      if (!loadedGame)
      {
        Game1.GenerateBundles(Game1.bundleType);
        foreach (string str in Game1.netWorldState.Value.BundleData.Values)
        {
          string[] strArray = str.Split('/')[2].Split(' ');
          if (Game1.game1.GetNewGameOption<bool>("YearOneCompletable"))
          {
            for (int index = 0; index < strArray.Length; index += 3)
            {
              if (strArray[index] == "266")
              {
                int maxValue = (16 - 2) * 2 + 3;
                Random random = new Random((int) Game1.uniqueIDForThisGame * 12);
                Game1.netWorldState.Value.VisitsUntilY1Guarantee = random.Next(2, maxValue);
              }
            }
          }
        }
        Game1.netWorldState.Value.ShuffleMineChests = Game1.game1.GetNewGameOption<Game1.MineChestType>("MineChests");
        if (Game1.game1.newGameSetupOptions.ContainsKey("SpawnMonstersAtNight"))
          Game1.spawnMonstersAtNight = Game1.game1.GetNewGameOption<bool>("SpawnMonstersAtNight");
      }
      Game1.player.ConvertClothingOverrideToClothesItems();
      Game1.player.addQuest(9);
      Game1.player.currentLocation = Game1.getLocationFromName("FarmHouse");
      Game1.player.gameVersion = Game1.version;
      Game1.hudMessages.Clear();
      Game1.hasLoadedGame = true;
      Game1.setGraphicsForSeason();
      if (!loadedGame)
        Game1._setSaveName = false;
      Game1.game1.newGameSetupOptions.Clear();
      Game1.updateCellarAssignments();
      if (loadedGame || !((NetFieldBase<IWorldState, NetRef<IWorldState>>) Game1.netWorldState != (NetRef<IWorldState>) null) || Game1.netWorldState.Value == null)
        return;
      Game1.netWorldState.Value.RegisterSpecialCurrencies();
    }

    public bool IsFirstInstanceAtThisLocation(
      GameLocation location,
      Func<Game1, bool> additional_check = null)
    {
      return GameRunner.instance.GetFirstInstanceAtThisLocation(location, additional_check) == this;
    }

    public bool IsLocalCoopJoinable() => GameRunner.instance.gameInstances.Count < GameRunner.instance.GetMaxSimultaneousPlayers() && !Game1.IsClient;

    public static void StartLocalMultiplayerIfNecessary()
    {
      if (Game1.multiplayerMode != (byte) 0)
        return;
      Console.WriteLine("Starting multiplayer server for local multiplayer...");
      Game1.multiplayerMode = (byte) 2;
      if (Game1.server != null)
        return;
      Game1.multiplayer.StartLocalMultiplayerServer();
    }

    public static void EndLocalMultiplayer()
    {
    }

    public static void addParrotBoyIfNecessary()
    {
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("addedParrotBoy"))
        return;
      if (Game1.getCharacterFromName("Leo", useLocationsListOnly: true) == null)
        Game1.getLocationFromNameInLocationsList("IslandHut").addCharacter(new NPC(new AnimatedSprite("Characters\\ParrotBoy", 0, 16, 32), new Vector2(320f, 384f), "IslandHut", 2, "Leo", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\ParrotBoy"))
        {
          Breather = false
        });
      if (Game1.player.friendshipData.ContainsKey("Leo"))
        return;
      Game1.player.friendshipData.Add("Leo", new Friendship());
    }

    public static void addBirdieIfNecessary()
    {
      if (Game1.getCharacterFromName("Birdie", useLocationsListOnly: true) != null)
        return;
      Game1.getLocationFromNameInLocationsList("IslandWest").addCharacter(new NPC(new AnimatedSprite("Characters\\Birdie", 0, 16, 32), new Vector2(1088f, 3712f), "IslandWest", 3, "Birdie", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Birdie")));
    }

    public static void addKentIfNecessary()
    {
      if (Game1.year <= 1)
        return;
      if (Game1.getCharacterFromName("Kent", useLocationsListOnly: true) == null)
        Game1.getLocationFromNameInLocationsList("SamHouse").addCharacter(new NPC(new AnimatedSprite("Characters\\Kent", 0, 16, 32), new Vector2(512f, 832f), "SamHouse", 2, "Kent", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Kent")));
      if (Game1.player.friendshipData.ContainsKey("Kent"))
        return;
      Game1.player.friendshipData.Add("Kent", new Friendship());
    }

    public void Instance_UnloadContent() => this.UnloadContent();

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    {
      base.UnloadContent();
      Game1.spriteBatch.Dispose();
      Game1.content.Unload();
      this.xTileContent.Unload();
      if (Game1.server == null)
        return;
      Game1.server.stopServer();
    }

    public void errorUpdateLoop()
    {
      if (Game1.GetKeyboardState().IsKeyDown(Keys.B))
      {
        Program.GameTesterMode = false;
        Game1.gameMode = (byte) 3;
      }
      if (Game1.GetKeyboardState().IsKeyDown(Keys.Escape))
      {
        Program.gamePtr.Exit();
        Environment.Exit(1);
      }
      this.Update(new GameTime());
      this.BeginDraw();
      this.Draw(new GameTime());
      this.EndDraw();
    }

    public static void showRedMessage(string message)
    {
      Game1.addHUDMessage(new HUDMessage(message, 3));
      if (!message.Contains("Inventory"))
      {
        Game1.playSound("cancel");
      }
      else
      {
        if (Game1.player.mailReceived.Contains("BackpackTip"))
          return;
        Game1.player.mailReceived.Add("BackpackTip");
        Game1.addMailForTomorrow("pierreBackpack");
      }
    }

    public static void showRedMessageUsingLoadString(string loadString) => Game1.showRedMessage(Game1.content.LoadString(loadString));

    public static bool didPlayerJustLeftClick(bool ignoreNonMouseHeldInput = false) => Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton != ButtonState.Pressed || Game1.input.GetGamePadState().Buttons.X == ButtonState.Pressed && (!ignoreNonMouseHeldInput || !Game1.oldPadState.IsButtonDown(Buttons.X)) || Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.useToolButton) && (!ignoreNonMouseHeldInput || Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.useToolButton));

    public static bool didPlayerJustRightClick(bool ignoreNonMouseHeldInput = false) => Game1.input.GetMouseState().RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton != ButtonState.Pressed || Game1.input.GetGamePadState().Buttons.A == ButtonState.Pressed && (!ignoreNonMouseHeldInput || !Game1.oldPadState.IsButtonDown(Buttons.A)) || Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.actionButton) && (!ignoreNonMouseHeldInput || !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.actionButton));

    public static bool didPlayerJustClickAtAll(bool ignoreNonMouseHeldInput = false) => Game1.didPlayerJustLeftClick(ignoreNonMouseHeldInput) || Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput);

    public static void showGlobalMessage(string message) => Game1.addHUDMessage(new HUDMessage(message, ""));

    public static void globalFadeToBlack(Game1.afterFadeFunction afterFade = null, float fadeSpeed = 0.02f) => Game1.screenFade.GlobalFadeToBlack(afterFade, fadeSpeed);

    public static void globalFadeToClear(Game1.afterFadeFunction afterFade = null, float fadeSpeed = 0.02f) => Game1.screenFade.GlobalFadeToClear(afterFade, fadeSpeed);

    public void CheckGamepadMode()
    {
      bool gamepadControls = Game1.options.gamepadControls;
      if (Game1.options.gamepadMode == Options.GamepadModes.ForceOn)
        Game1.options.gamepadControls = true;
      else if (Game1.options.gamepadMode == Options.GamepadModes.ForceOff)
      {
        Game1.options.gamepadControls = false;
      }
      else
      {
        MouseState mouseState = Game1.input.GetMouseState();
        KeyboardState keyboardState = Game1.GetKeyboardState();
        GamePadState gamePadState = Game1.input.GetGamePadState();
        bool flag1 = false;
        if ((mouseState.LeftButton == ButtonState.Pressed || mouseState.MiddleButton == ButtonState.Pressed || mouseState.RightButton == ButtonState.Pressed || mouseState.ScrollWheelValue != this._oldScrollWheelValue || (mouseState.X != this._oldMousePosition.X || mouseState.Y != this._oldMousePosition.Y) && Game1.lastCursorMotionWasMouse || keyboardState.GetPressedKeys().Length != 0) && (keyboardState.GetPressedKeys().Length != 1 || keyboardState.GetPressedKeys()[0] != Keys.Pause))
        {
          flag1 = true;
          if (Program.sdk is SteamHelper && (Program.sdk as SteamHelper).IsRunningOnSteamDeck())
            flag1 = false;
        }
        this._oldScrollWheelValue = mouseState.ScrollWheelValue;
        this._oldMousePosition.X = mouseState.X;
        this._oldMousePosition.Y = mouseState.Y;
        bool flag2 = Game1.isAnyGamePadButtonBeingPressed() || Game1.isDPadPressed() || Game1.isGamePadThumbstickInMotion() || (double) gamePadState.Triggers.Left != 0.0 || (double) gamePadState.Triggers.Right != 0.0;
        if (this._oldGamepadConnectedState != gamePadState.IsConnected)
        {
          this._oldGamepadConnectedState = gamePadState.IsConnected;
          if (this._oldGamepadConnectedState)
          {
            Game1.options.gamepadControls = true;
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2574"));
          }
          else
          {
            Game1.options.gamepadControls = false;
            if (this.instancePlayerOneIndex != ~PlayerIndex.One)
            {
              Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2575"));
              if (Game1.CanShowPauseMenu() && Game1.activeClickableMenu == null)
                Game1.activeClickableMenu = (IClickableMenu) new GameMenu();
            }
          }
        }
        if (flag1 && Game1.options.gamepadControls)
          Game1.options.gamepadControls = false;
        if (!Game1.options.gamepadControls & flag2)
          Game1.options.gamepadControls = true;
        if (gamepadControls == Game1.options.gamepadControls || !Game1.options.gamepadControls)
          return;
        Game1.lastMousePositionBeforeFade = new Point(this.localMultiplayerWindow.Width / 2, this.localMultiplayerWindow.Height / 2);
        if (Game1.activeClickableMenu != null)
        {
          Game1.activeClickableMenu.setUpForGamePadMode();
          if (Game1.options.SnappyMenus)
          {
            Game1.activeClickableMenu.populateClickableComponentList();
            Game1.activeClickableMenu.snapToDefaultClickableComponent();
          }
        }
        Game1.timerUntilMouseFade = 0;
      }
    }

    public void Instance_Update(GameTime gameTime) => this.Update(gameTime);

    protected override void Update(GameTime gameTime)
    {
      GameTime gameTime1 = gameTime;
      DebugTools.BeforeGameUpdate(this, ref gameTime1);
      Game1.input.UpdateStates();
      TimeSpan elapsedGameTime;
      if (Game1.input.GetGamePadState().IsButtonDown(Buttons.RightStick))
      {
        int rightStickHoldTime = Game1.rightStickHoldTime;
        elapsedGameTime = gameTime.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        Game1.rightStickHoldTime = rightStickHoldTime + milliseconds;
      }
      GameMenu.bundleItemHovered = false;
      this._update(gameTime1);
      if (Game1.IsMultiplayer && Game1.player != null)
      {
        Game1.player.requestingTimePause.Value = !Game1.shouldTimePass(LocalMultiplayer.IsLocalMultiplayer(true));
        if (Game1.IsMasterGame)
        {
          bool flag = false;
          if (LocalMultiplayer.IsLocalMultiplayer(true))
          {
            flag = true;
            foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
            {
              if (!onlineFarmer.requestingTimePause.Value)
              {
                flag = false;
                break;
              }
            }
          }
          Game1.netWorldState.Value.IsTimePaused = flag;
        }
      }
      elapsedGameTime = gameTime.ElapsedGameTime;
      Rumble.update((float) elapsedGameTime.Milliseconds);
      if (Game1.options.gamepadControls && Game1.thumbstickMotionMargin > 0)
      {
        int thumbstickMotionMargin = Game1.thumbstickMotionMargin;
        elapsedGameTime = gameTime.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        Game1.thumbstickMotionMargin = thumbstickMotionMargin - milliseconds;
      }
      if (!Game1.input.GetGamePadState().IsButtonDown(Buttons.RightStick))
        Game1.rightStickHoldTime = 0;
      base.Update(gameTime);
    }

    public void Instance_OnActivated(object sender, EventArgs args) => this.OnActivated(sender, args);

    protected override void OnActivated(object sender, EventArgs args)
    {
      base.OnActivated(sender, args);
      Game1._activatedTick = Game1.ticks + 1;
      Game1.input.IgnoreKeys(Game1.GetKeyboardState().GetPressedKeys());
    }

    public bool HasKeyboardFocus() => Game1.keyboardFocusInstance == null ? this.IsMainInstance : Game1.keyboardFocusInstance == this;

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    private void _update(GameTime gameTime)
    {
      if (Game1.graphics.GraphicsDevice == null)
        return;
      bool flag1 = false;
      if (Game1.options != null && !this.takingMapScreenshot)
      {
        if ((double) Game1.options.baseUIScale != (double) Game1.options.desiredUIScale)
        {
          if ((double) Game1.options.desiredUIScale < 0.0)
            Game1.options.desiredUIScale = Game1.options.desiredBaseZoomLevel;
          Game1.options.baseUIScale = Game1.options.desiredUIScale;
          flag1 = true;
        }
        if ((double) Game1.options.desiredBaseZoomLevel != (double) Game1.options.baseZoomLevel)
        {
          Game1.options.baseZoomLevel = Game1.options.desiredBaseZoomLevel;
          Game1.forceSnapOnNextViewportUpdate = true;
          flag1 = true;
        }
      }
      if (flag1)
        this.refreshWindowSettings();
      this.CheckGamepadMode();
      FarmAnimal.NumPathfindingThisTick = 0;
      Game1.options.reApplySetOptions();
      if (Game1.toggleFullScreen)
      {
        Game1.toggleFullscreen();
        Game1.toggleFullScreen = false;
      }
      Game1.input.Update();
      if (Game1.frameByFrame)
      {
        KeyboardState keyboardState = Game1.GetKeyboardState();
        if (keyboardState.IsKeyDown(Keys.Escape) && Game1.oldKBState.IsKeyUp(Keys.Escape))
          Game1.frameByFrame = false;
        bool flag2 = false;
        keyboardState = Game1.GetKeyboardState();
        if (keyboardState.IsKeyDown(Keys.G) && Game1.oldKBState.IsKeyUp(Keys.G))
          flag2 = true;
        if (!flag2)
        {
          Game1.oldKBState = Game1.GetKeyboardState();
          return;
        }
      }
      if (Game1.client != null && Game1.client.timedOut)
        Game1.multiplayer.clientRemotelyDisconnected(Game1.client.pendingDisconnect);
      if (Game1._newDayTask != null)
      {
        if (Game1._newDayTask.Status == TaskStatus.Created)
          Game1.hooks.StartTask(Game1._newDayTask, "NewDay");
        if (Game1._newDayTask.Status >= TaskStatus.RanToCompletion)
        {
          if (Game1._newDayTask.IsFaulted)
          {
            Exception baseException = Game1._newDayTask.Exception.GetBaseException();
            Console.WriteLine("_newDayTask failed with an exception");
            Console.WriteLine((object) baseException);
            throw new Exception("Error on new day: \n---------------\n" + baseException.Message + "\n" + baseException.StackTrace + "\n---------------\n");
          }
          Game1._newDayTask = (Task) null;
          Utility.CollectGarbage();
        }
        Game1.UpdateChatBox();
      }
      else if (this.isLocalMultiplayerNewDayActive)
        Game1.UpdateChatBox();
      else if (this.IsSaving)
      {
        Game1.PushUIMode();
        Game1.activeClickableMenu?.update(gameTime);
        if (Game1.overlayMenu != null)
        {
          Game1.overlayMenu.update(gameTime);
          if (Game1.overlayMenu == null)
          {
            Game1.PopUIMode();
            return;
          }
        }
        Game1.PopUIMode();
        Game1.UpdateChatBox();
      }
      else
      {
        if (Game1.exitToTitle)
        {
          Game1.exitToTitle = false;
          this.CleanupReturningToTitle();
          Utility.CollectGarbage();
          if (Game1.postExitToTitleCallback != null)
            Game1.postExitToTitleCallback();
        }
        TimeSpan elapsedGameTime = gameTime.ElapsedGameTime;
        Game1.SetFreeCursorElapsed((float) elapsedGameTime.TotalSeconds);
        Program.sdk.Update();
        if (Game1.game1.IsMainInstance)
        {
          Game1.keyboardFocusInstance = Game1.game1;
          foreach (Game1 gameInstance in GameRunner.instance.gameInstances)
          {
            if (gameInstance.instanceKeyboardDispatcher.Subscriber != null && gameInstance.instanceTextEntry != null)
            {
              Game1.keyboardFocusInstance = gameInstance;
              break;
            }
          }
        }
        if (this.IsMainInstance)
        {
          int displayIndex = this.Window.GetDisplayIndex();
          if (this._lastUsedDisplay != -1 && this._lastUsedDisplay != displayIndex)
          {
            StartupPreferences startupPreferences = new StartupPreferences();
            startupPreferences.loadPreferences(false, false);
            startupPreferences.displayIndex = displayIndex;
            startupPreferences.savePreferences(false);
          }
          this._lastUsedDisplay = displayIndex;
        }
        if (this.HasKeyboardFocus())
          Game1.keyboardDispatcher.Poll();
        else
          Game1.keyboardDispatcher.Discard();
        if (Game1.gameMode == (byte) 6)
          Game1.multiplayer.UpdateLoading();
        if (Game1.gameMode == (byte) 3)
        {
          Game1.multiplayer.UpdateEarly();
          if (Game1.player != null && Game1.player.team != null)
            Game1.player.team.Update();
        }
        if ((Game1.paused || !this.IsActiveNoOverlay && Program.releaseBuild) && (Game1.options == null || Game1.options.pauseWhenOutOfFocus || Game1.paused) && Game1.multiplayerMode == (byte) 0)
        {
          Game1.UpdateChatBox();
        }
        else
        {
          if (Game1.quit)
            this.Exit();
          Game1.currentGameTime = gameTime;
          if (Game1.gameMode == (byte) 11)
            return;
          ++Game1.ticks;
          if (this.IsActiveNoOverlay)
            this.checkForEscapeKeys();
          Game1.updateMusic();
          Game1.updateRaindropPosition();
          if (Game1.globalFade)
            Game1.screenFade.UpdateGlobalFade();
          else if (Game1.pauseThenDoFunctionTimer > 0)
          {
            Game1.freezeControls = true;
            int thenDoFunctionTimer = Game1.pauseThenDoFunctionTimer;
            elapsedGameTime = gameTime.ElapsedGameTime;
            int milliseconds = elapsedGameTime.Milliseconds;
            Game1.pauseThenDoFunctionTimer = thenDoFunctionTimer - milliseconds;
            if (Game1.pauseThenDoFunctionTimer <= 0)
            {
              Game1.freezeControls = false;
              if (Game1.afterPause != null)
                Game1.afterPause();
            }
          }
          bool flag3 = false;
          if (Game1.options.gamepadControls && Game1.activeClickableMenu != null && Game1.activeClickableMenu.shouldClampGamePadCursor())
            flag3 = true;
          if (flag3)
          {
            Point mousePositionRaw = Game1.getMousePositionRaw();
            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, this.localMultiplayerWindow.Width, this.localMultiplayerWindow.Height);
            if (mousePositionRaw.X < rectangle.X)
              mousePositionRaw.X = rectangle.X;
            else if (mousePositionRaw.X > rectangle.Right)
              mousePositionRaw.X = rectangle.Right;
            if (mousePositionRaw.Y < rectangle.Y)
              mousePositionRaw.Y = rectangle.Y;
            else if (mousePositionRaw.Y > rectangle.Bottom)
              mousePositionRaw.Y = rectangle.Bottom;
            Game1.setMousePositionRaw(mousePositionRaw.X, mousePositionRaw.Y);
          }
          if (Game1.gameMode == (byte) 3 || Game1.gameMode == (byte) 2)
          {
            if (!Game1.warpingForForcedRemoteEvent && !Game1.eventUp && Game1.remoteEventQueue.Count > 0 && Game1.player != null && Game1.player.isCustomized.Value && (!Game1.fadeIn || (double) Game1.fadeToBlackAlpha <= 0.0))
            {
              if (Game1.activeClickableMenu != null)
              {
                Game1.activeClickableMenu.emergencyShutDown();
                Game1.exitActiveMenu();
              }
              else if (Game1.currentMinigame != null && Game1.currentMinigame.forceQuit())
                Game1.currentMinigame = (IMinigame) null;
              if (Game1.activeClickableMenu == null && Game1.currentMinigame == null && Game1.player.freezePause <= 0)
              {
                Action remoteEvent = Game1.remoteEventQueue[0];
                Game1.remoteEventQueue.RemoveAt(0);
                remoteEvent();
              }
            }
            Farmer player = Game1.player;
            long millisecondsPlayed = (long) player.millisecondsPlayed;
            elapsedGameTime = gameTime.ElapsedGameTime;
            long milliseconds1 = (long) (uint) elapsedGameTime.Milliseconds;
            player.millisecondsPlayed = (ulong) (millisecondsPlayed + milliseconds1);
            bool flag4 = true;
            if (Game1.currentMinigame != null && !Game1.HostPaused)
            {
              if ((double) Game1.pauseTime > 0.0)
                Game1.updatePause(gameTime);
              if (Game1.fadeToBlack)
              {
                Game1.screenFade.UpdateFadeAlpha(gameTime);
                if ((double) Game1.fadeToBlackAlpha >= 1.0)
                  Game1.fadeToBlack = false;
              }
              else
              {
                if (Game1.thumbstickMotionMargin > 0)
                {
                  int thumbstickMotionMargin = Game1.thumbstickMotionMargin;
                  elapsedGameTime = gameTime.ElapsedGameTime;
                  int milliseconds2 = elapsedGameTime.Milliseconds;
                  Game1.thumbstickMotionMargin = thumbstickMotionMargin - milliseconds2;
                }
                KeyboardState keyboardState = new KeyboardState();
                MouseState mouseState = new MouseState();
                GamePadState gamePadState = new GamePadState();
                if (this.IsActive)
                {
                  keyboardState = Game1.GetKeyboardState();
                  mouseState = Game1.input.GetMouseState();
                  gamePadState = Game1.input.GetGamePadState();
                  bool flag5 = false;
                  if (Game1.chatBox != null && Game1.chatBox.isActive())
                    flag5 = true;
                  else if (Game1.textEntry != null)
                    flag5 = true;
                  if (flag5)
                  {
                    keyboardState = new KeyboardState();
                    gamePadState = new GamePadState();
                  }
                  else
                  {
                    foreach (Keys pressedKey in keyboardState.GetPressedKeys())
                    {
                      if (!Game1.oldKBState.IsKeyDown(pressedKey) && Game1.currentMinigame != null)
                        Game1.currentMinigame.receiveKeyPress(pressedKey);
                    }
                    if (Game1.options.gamepadControls)
                    {
                      if (Game1.currentMinigame == null)
                      {
                        Game1.oldMouseState = mouseState;
                        Game1.oldKBState = keyboardState;
                        Game1.oldPadState = gamePadState;
                        Game1.UpdateChatBox();
                        return;
                      }
                      foreach (Buttons pressedButton in Utility.getPressedButtons(gamePadState, Game1.oldPadState))
                      {
                        if (Game1.currentMinigame != null)
                          Game1.currentMinigame.receiveKeyPress(Utility.mapGamePadButtonToKey(pressedButton));
                      }
                      if (Game1.currentMinigame == null)
                      {
                        Game1.oldMouseState = mouseState;
                        Game1.oldKBState = keyboardState;
                        Game1.oldPadState = gamePadState;
                        Game1.UpdateChatBox();
                        return;
                      }
                      GamePadThumbSticks thumbSticks = gamePadState.ThumbSticks;
                      if ((double) thumbSticks.Right.Y < -0.200000002980232)
                      {
                        thumbSticks = Game1.oldPadState.ThumbSticks;
                        if ((double) thumbSticks.Right.Y >= -0.200000002980232)
                          Game1.currentMinigame.receiveKeyPress(Keys.Down);
                      }
                      thumbSticks = gamePadState.ThumbSticks;
                      if ((double) thumbSticks.Right.Y > 0.200000002980232)
                      {
                        thumbSticks = Game1.oldPadState.ThumbSticks;
                        if ((double) thumbSticks.Right.Y <= 0.200000002980232)
                          Game1.currentMinigame.receiveKeyPress(Keys.Up);
                      }
                      thumbSticks = gamePadState.ThumbSticks;
                      if ((double) thumbSticks.Right.X < -0.200000002980232)
                      {
                        thumbSticks = Game1.oldPadState.ThumbSticks;
                        if ((double) thumbSticks.Right.X >= -0.200000002980232)
                          Game1.currentMinigame.receiveKeyPress(Keys.Left);
                      }
                      thumbSticks = gamePadState.ThumbSticks;
                      if ((double) thumbSticks.Right.X > 0.200000002980232)
                      {
                        thumbSticks = Game1.oldPadState.ThumbSticks;
                        if ((double) thumbSticks.Right.X <= 0.200000002980232)
                          Game1.currentMinigame.receiveKeyPress(Keys.Right);
                      }
                      thumbSticks = Game1.oldPadState.ThumbSticks;
                      if ((double) thumbSticks.Right.Y < -0.200000002980232)
                      {
                        thumbSticks = gamePadState.ThumbSticks;
                        if ((double) thumbSticks.Right.Y >= -0.200000002980232)
                          Game1.currentMinigame.receiveKeyRelease(Keys.Down);
                      }
                      thumbSticks = Game1.oldPadState.ThumbSticks;
                      if ((double) thumbSticks.Right.Y > 0.200000002980232)
                      {
                        thumbSticks = gamePadState.ThumbSticks;
                        if ((double) thumbSticks.Right.Y <= 0.200000002980232)
                          Game1.currentMinigame.receiveKeyRelease(Keys.Up);
                      }
                      thumbSticks = Game1.oldPadState.ThumbSticks;
                      if ((double) thumbSticks.Right.X < -0.200000002980232)
                      {
                        thumbSticks = gamePadState.ThumbSticks;
                        if ((double) thumbSticks.Right.X >= -0.200000002980232)
                          Game1.currentMinigame.receiveKeyRelease(Keys.Left);
                      }
                      thumbSticks = Game1.oldPadState.ThumbSticks;
                      if ((double) thumbSticks.Right.X > 0.200000002980232)
                      {
                        thumbSticks = gamePadState.ThumbSticks;
                        if ((double) thumbSticks.Right.X <= 0.200000002980232)
                          Game1.currentMinigame.receiveKeyRelease(Keys.Right);
                      }
                      if (Game1.isGamePadThumbstickInMotion() && Game1.currentMinigame != null && !Game1.currentMinigame.overrideFreeMouseMovement())
                      {
                        int mouseX = Game1.getMouseX();
                        thumbSticks = gamePadState.ThumbSticks;
                        int num1 = (int) ((double) thumbSticks.Left.X * (double) Game1.thumbstickToMouseModifier);
                        int x = mouseX + num1;
                        int mouseY = Game1.getMouseY();
                        thumbSticks = gamePadState.ThumbSticks;
                        int num2 = (int) ((double) thumbSticks.Left.Y * (double) Game1.thumbstickToMouseModifier);
                        int y = mouseY - num2;
                        Game1.setMousePosition(x, y);
                      }
                      else if (Game1.getMouseX() != Game1.getOldMouseX() || Game1.getMouseY() != Game1.getOldMouseY())
                        Game1.lastCursorMotionWasMouse = true;
                    }
                    foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
                    {
                      if (!keyboardState.IsKeyDown(pressedKey) && Game1.currentMinigame != null)
                        Game1.currentMinigame.receiveKeyRelease(pressedKey);
                    }
                    if (Game1.options.gamepadControls)
                    {
                      if (Game1.currentMinigame == null)
                      {
                        Game1.oldMouseState = mouseState;
                        Game1.oldKBState = keyboardState;
                        Game1.oldPadState = gamePadState;
                        Game1.UpdateChatBox();
                        return;
                      }
                      if (gamePadState.IsConnected && gamePadState.IsButtonDown(Buttons.X) && !Game1.oldPadState.IsButtonDown(Buttons.X))
                        Game1.currentMinigame.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
                      else if (gamePadState.IsConnected && gamePadState.IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A))
                        Game1.currentMinigame.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY());
                      else if (gamePadState.IsConnected && !gamePadState.IsButtonDown(Buttons.X) && Game1.oldPadState.IsButtonDown(Buttons.X))
                        Game1.currentMinigame.releaseRightClick(Game1.getMouseX(), Game1.getMouseY());
                      else if (gamePadState.IsConnected && !gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
                        Game1.currentMinigame.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
                      foreach (Buttons pressedButton in Utility.getPressedButtons(Game1.oldPadState, gamePadState))
                      {
                        if (Game1.currentMinigame != null)
                          Game1.currentMinigame.receiveKeyRelease(Utility.mapGamePadButtonToKey(pressedButton));
                      }
                      if (gamePadState.IsConnected && gamePadState.IsButtonDown(Buttons.A) && Game1.currentMinigame != null)
                        Game1.currentMinigame.leftClickHeld(0, 0);
                    }
                    if (Game1.currentMinigame == null)
                    {
                      Game1.oldMouseState = mouseState;
                      Game1.oldKBState = keyboardState;
                      Game1.oldPadState = gamePadState;
                      Game1.UpdateChatBox();
                      return;
                    }
                    if (Game1.currentMinigame != null && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton != ButtonState.Pressed)
                      Game1.currentMinigame.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY());
                    if (Game1.currentMinigame != null && mouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton != ButtonState.Pressed)
                      Game1.currentMinigame.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
                    if (Game1.currentMinigame != null && mouseState.LeftButton == ButtonState.Released && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
                      Game1.currentMinigame.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
                    if (Game1.currentMinigame != null && mouseState.RightButton == ButtonState.Released && Game1.oldMouseState.RightButton == ButtonState.Pressed)
                      Game1.currentMinigame.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
                    if (Game1.currentMinigame != null && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
                      Game1.currentMinigame.leftClickHeld(Game1.getMouseX(), Game1.getMouseY());
                  }
                }
                if (Game1.currentMinigame != null && Game1.currentMinigame.tick(gameTime))
                {
                  Game1.oldMouseState = mouseState;
                  Game1.oldKBState = keyboardState;
                  Game1.oldPadState = gamePadState;
                  if (Game1.currentMinigame != null)
                    Game1.currentMinigame.unload();
                  Game1.currentMinigame = (IMinigame) null;
                  Game1.fadeIn = true;
                  Game1.fadeToBlackAlpha = 1f;
                  Game1.UpdateChatBox();
                  return;
                }
                if (Game1.currentMinigame == null && Game1.IsMusicContextActive(Game1.MusicContext.MiniGame))
                  Game1.stopMusicTrack(Game1.MusicContext.MiniGame);
                Game1.oldMouseState = mouseState;
                Game1.oldKBState = keyboardState;
                Game1.oldPadState = gamePadState;
              }
              flag4 = Game1.IsMultiplayer || Game1.currentMinigame == null || Game1.currentMinigame.doMainGameUpdates();
            }
            else if (Game1.farmEvent != null && !Game1.HostPaused && Game1.farmEvent.tickUpdate(gameTime))
            {
              Game1.farmEvent.makeChangesToLocation();
              Game1.timeOfDay = 600;
              Game1.UpdateOther(gameTime);
              Game1.displayHUD = true;
              Game1.farmEvent = (FarmEvent) null;
              Game1.netWorldState.Value.WriteToGame1();
              Game1.currentLocation = Game1.player.currentLocation;
              if (Game1.currentLocation is FarmHouse currentLocation)
              {
                Game1.player.Position = Utility.PointToVector2(currentLocation.GetPlayerBedSpot()) * 64f;
                BedFurniture.ShiftPositionForBed(Game1.player);
              }
              else
                BedFurniture.ApplyWakeUpPosition(Game1.player);
              Game1.changeMusicTrack("none");
              Game1.currentLocation.resetForPlayerEntry();
              if (Game1.player.IsSitting())
                Game1.player.StopSitting(false);
              Game1.player.forceCanMove();
              Game1.freezeControls = false;
              Game1.displayFarmer = true;
              Game1.outdoorLight = Microsoft.Xna.Framework.Color.White;
              Game1.viewportFreeze = false;
              Game1.fadeToBlackAlpha = 0.0f;
              Game1.fadeToBlack = false;
              Game1.globalFadeToClear();
              Game1.RemoveDeliveredMailForTomorrow();
              Game1.handlePostFarmEventActions();
              Game1.showEndOfNightStuff();
            }
            if (flag4)
            {
              if (Game1.endOfNightMenus.Count > 0 && Game1.activeClickableMenu == null)
              {
                Game1.activeClickableMenu = Game1.endOfNightMenus.Pop();
                if (Game1.activeClickableMenu != null && Game1.options.SnappyMenus)
                  Game1.activeClickableMenu.snapToDefaultClickableComponent();
              }
              if (Game1.specialCurrencyDisplay != null)
                Game1.specialCurrencyDisplay.Update(gameTime);
              if (Game1.currentLocation != null && Game1.currentMinigame == null)
              {
                if (Game1.emoteMenu != null)
                {
                  Game1.emoteMenu.update(gameTime);
                  if (Game1.emoteMenu != null)
                  {
                    Game1.PushUIMode();
                    Game1.emoteMenu.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
                    KeyboardState keyboardState = Game1.GetKeyboardState();
                    MouseState mouseState = Game1.input.GetMouseState();
                    if (mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released)
                    {
                      Game1.emoteMenu.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY(), true);
                    }
                    else
                    {
                      mouseState = Game1.input.GetMouseState();
                      if (mouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton == ButtonState.Released)
                        Game1.emoteMenu.receiveRightClick(Game1.getMouseX(), Game1.getMouseY(), true);
                      else if (Game1.isOneOfTheseKeysDown(keyboardState, Game1.options.menuButton) || Game1.isOneOfTheseKeysDown(keyboardState, Game1.options.emoteButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.emoteButton))
                        Game1.emoteMenu.exitThisMenu(false);
                    }
                    Game1.PopUIMode();
                    Game1.oldKBState = keyboardState;
                    Game1.oldMouseState = Game1.input.GetMouseState();
                  }
                }
                else if (Game1.textEntry != null)
                {
                  Game1.PushUIMode();
                  Game1.updateTextEntry(gameTime);
                  Game1.PopUIMode();
                }
                else if (Game1.activeClickableMenu != null)
                {
                  Game1.PushUIMode();
                  Game1.updateActiveMenu(gameTime);
                  Game1.PopUIMode();
                }
                else
                {
                  if ((double) Game1.pauseTime > 0.0)
                    Game1.updatePause(gameTime);
                  if (!Game1.globalFade && !Game1.freezeControls && Game1.activeClickableMenu == null && (this.IsActiveNoOverlay || Game1.inputSimulator != null))
                    this.UpdateControlInput(gameTime);
                }
              }
              if (Game1.showingEndOfNightStuff && Game1.endOfNightMenus.Count == 0 && Game1.activeClickableMenu == null)
              {
                if (Game1.newDaySync != null)
                  Game1.newDaySync = (NewDaySynchronizer) null;
                Game1.player.team.endOfNightStatus.WithdrawState();
                Game1.showingEndOfNightStuff = false;
                Action afterNewDayAction = Game1._afterNewDayAction;
                if (afterNewDayAction != null)
                {
                  Game1._afterNewDayAction = (Action) null;
                  afterNewDayAction();
                }
                Game1.player.ReequipEnchantments();
                Game1.globalFadeToClear(new Game1.afterFadeFunction(Game1.doMorningStuff));
              }
              if (Game1.currentLocation != null)
              {
                if (!Game1.HostPaused && !Game1.showingEndOfNightStuff)
                {
                  if (Game1.IsMultiplayer || Game1.activeClickableMenu == null && Game1.currentMinigame == null)
                    Game1.UpdateGameClock(gameTime);
                  this.UpdateCharacters(gameTime);
                  this.UpdateLocations(gameTime);
                  if (Game1.currentMinigame == null)
                  {
                    Game1.UpdateViewPort(false, this.getViewportCenter());
                  }
                  else
                  {
                    Game1.previousViewportPosition.X = (float) Game1.viewport.X;
                    Game1.previousViewportPosition.Y = (float) Game1.viewport.Y;
                  }
                  Game1.UpdateOther(gameTime);
                }
                if (Game1.messagePause)
                {
                  KeyboardState keyboardState = Game1.GetKeyboardState();
                  MouseState mouseState = Game1.input.GetMouseState();
                  GamePadState gamePadState = Game1.input.GetGamePadState();
                  if (Game1.isOneOfTheseKeysDown(keyboardState, Game1.options.actionButton) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.actionButton))
                    Game1.pressActionButton(keyboardState, mouseState, gamePadState);
                  Game1.oldKBState = keyboardState;
                  Game1.oldPadState = gamePadState;
                }
              }
            }
            else if (Game1.textEntry != null)
            {
              Game1.PushUIMode();
              Game1.updateTextEntry(gameTime);
              Game1.PopUIMode();
            }
          }
          else
          {
            this.UpdateTitleScreen(gameTime);
            if (Game1.textEntry != null)
            {
              Game1.PushUIMode();
              Game1.updateTextEntry(gameTime);
              Game1.PopUIMode();
            }
            else if (Game1.activeClickableMenu != null)
            {
              Game1.PushUIMode();
              Game1.updateActiveMenu(gameTime);
              Game1.PopUIMode();
            }
            if (Game1.gameMode == (byte) 10)
              Game1.UpdateOther(gameTime);
          }
          if (Game1.audioEngine != null)
            Game1.audioEngine.Update();
          Game1.UpdateChatBox();
          if (Game1.gameMode == (byte) 6)
            return;
          Game1.multiplayer.UpdateLate();
        }
      }
    }

    public static int CurrentPlayerLimit => (NetFieldBase<IWorldState, NetRef<IWorldState>>) Game1.netWorldState == (NetRef<IWorldState>) null || Game1.netWorldState.Value == null || (NetFieldBase<int, NetInt>) Game1.netWorldState.Value.CurrentPlayerLimit == (NetInt) null ? Game1.multiplayer.playerLimit : Game1.netWorldState.Value.CurrentPlayerLimit.Value;

    public static void showTextEntry(TextBox text_box)
    {
      Game1.timerUntilMouseFade = 0;
      Game1.PushUIMode();
      Game1.textEntry = new TextEntryMenu(text_box);
      Game1.PopUIMode();
    }

    public static void closeTextEntry()
    {
      if (Game1.textEntry != null)
        Game1.textEntry = (TextEntryMenu) null;
      if (Game1.activeClickableMenu == null || !Game1.options.SnappyMenus)
        return;
      if (Game1.activeClickableMenu is TitleMenu && TitleMenu.subMenu != null)
        TitleMenu.subMenu.snapCursorToCurrentSnappedComponent();
      else
        Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
    }

    public static bool isDarkOut() => Game1.timeOfDay >= Game1.getTrulyDarkTime();

    public static bool isStartingToGetDarkOut() => Game1.timeOfDay >= Game1.getStartingToGetDarkTime();

    public static int getStartingToGetDarkTime()
    {
      string currentSeason = Game1.currentSeason;
      if (currentSeason == "spring" || currentSeason == "summer")
        return 1800;
      if (currentSeason == "fall")
        return 1700;
      return currentSeason == "winter" ? 1600 : 1800;
    }

    public static void updateCellarAssignments()
    {
      if (!Game1.IsMasterGame)
        return;
      Game1.player.team.cellarAssignments[1] = Game1.MasterPlayer.UniqueMultiplayerID;
      for (int key = 2; key <= Game1.netWorldState.Value.HighestPlayerLimit.Value; ++key)
      {
        string name = "Cellar" + key.ToString();
        if (key != 1 && Game1.getLocationFromName(name) != null)
        {
          if (Game1.player.team.cellarAssignments.ContainsKey(key) && Game1.getFarmerMaybeOffline(Game1.player.team.cellarAssignments[key]) == null)
            Game1.player.team.cellarAssignments.Remove(key);
          if (!Game1.player.team.cellarAssignments.ContainsKey(key))
          {
            foreach (Farmer allFarmer in Game1.getAllFarmers())
            {
              if (!Game1.player.team.cellarAssignments.Values.Contains<long>(allFarmer.UniqueMultiplayerID))
              {
                Game1.player.team.cellarAssignments[key] = allFarmer.UniqueMultiplayerID;
                break;
              }
            }
          }
        }
      }
    }

    public static int getModeratelyDarkTime() => (Game1.getTrulyDarkTime() + Game1.getStartingToGetDarkTime()) / 2;

    public static int getTrulyDarkTime() => Game1.getStartingToGetDarkTime() + 200;

    public static void playMorningSong()
    {
      if (Game1.IsRainingHere() || Game1.IsLightningHere() || Game1.eventUp || Game1.dayOfMonth <= 0 || Game1.currentLocation.Name.Equals("Desert"))
        return;
      if (Game1.currentLocation.GetLocationContext() == GameLocation.LocationContext.Island)
      {
        if (!Game1.MasterPlayer.hasOrWillReceiveMail("Island_FirstParrot"))
          return;
        Game1.morningSongPlayAction = DelayedAction.playMusicAfterDelay("IslandMusic", 500);
      }
      else
        Game1.morningSongPlayAction = DelayedAction.playMusicAfterDelay(Game1.currentSeason + Math.Max(1, Game1.currentSongIndex).ToString(), 500);
    }

    public static void doMorningStuff()
    {
      Game1.playMorningSong();
      DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
      {
        while (Game1.morningQueue.Count > 0)
          Game1.morningQueue.Dequeue()();
      }), 1000);
      if (!Game1.player.hasPendingCompletedQuests)
        return;
      Game1.dayTimeMoneyBox.PingQuestLog();
    }

    /// <summary>
    /// adds a function that will be called 1 second after fully waking up in the morning. These will not be saved, so only use for "fluff" functions, like sending multiplayer chat messages, etc.
    /// </summary>
    /// <param name="func"></param>
    public static void addMorningFluffFunction(DelayedAction.delayedBehavior func) => Game1.morningQueue.Enqueue(func);

    private Point getViewportCenter()
    {
      if ((double) Game1.viewportTarget.X != (double) int.MinValue)
      {
        if ((double) Math.Abs((float) Game1.viewportCenter.X - Game1.viewportTarget.X) > (double) Game1.viewportSpeed || (double) Math.Abs((float) Game1.viewportCenter.Y - Game1.viewportTarget.Y) > (double) Game1.viewportSpeed)
        {
          Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(Game1.viewportCenter, Game1.viewportTarget, Game1.viewportSpeed);
          Game1.viewportCenter.X += (int) Math.Round((double) velocityTowardPoint.X);
          Game1.viewportCenter.Y += (int) Math.Round((double) velocityTowardPoint.Y);
        }
        else
        {
          if (Game1.viewportReachedTarget != null)
          {
            Game1.viewportReachedTarget();
            Game1.viewportReachedTarget = (Game1.afterFadeFunction) null;
          }
          Game1.viewportHold -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
          if (Game1.viewportHold <= 0)
          {
            Game1.viewportTarget = new Vector2((float) int.MinValue, (float) int.MinValue);
            if (Game1.afterViewport != null)
              Game1.afterViewport();
          }
        }
        return Game1.viewportCenter;
      }
      Farmer playerOrEventFarmer = Game1.getPlayerOrEventFarmer();
      Game1.viewportCenter.X = playerOrEventFarmer.getStandingX();
      Game1.viewportCenter.Y = playerOrEventFarmer.getStandingY();
      return Game1.viewportCenter;
    }

    public static void afterFadeReturnViewportToPlayer()
    {
      Game1.viewportTarget = new Vector2((float) int.MinValue, (float) int.MinValue);
      Game1.viewportHold = 0;
      Game1.viewportFreeze = false;
      Game1.viewportCenter.X = Game1.player.getStandingX();
      Game1.viewportCenter.Y = Game1.player.getStandingY();
      Game1.globalFadeToClear();
    }

    public static bool isViewportOnCustomPath() => (double) Game1.viewportTarget.X != (double) int.MinValue;

    public static void moveViewportTo(
      Vector2 target,
      float speed,
      int holdTimer = 0,
      Game1.afterFadeFunction reachedTarget = null,
      Game1.afterFadeFunction endFunction = null)
    {
      Game1.viewportTarget = target;
      Game1.viewportSpeed = speed;
      Game1.viewportHold = holdTimer;
      Game1.afterViewport = endFunction;
      Game1.viewportReachedTarget = reachedTarget;
    }

    public static Farm getFarm() => Game1.getLocationFromName("Farm") as Farm;

    public static void setMousePosition(int x, int y, bool ui_scale)
    {
      if (ui_scale)
        Game1.setMousePositionRaw((int) ((double) x * (double) Game1.options.uiScale), (int) ((double) y * (double) Game1.options.uiScale));
      else
        Game1.setMousePositionRaw((int) ((double) x * (double) Game1.options.zoomLevel), (int) ((double) y * (double) Game1.options.zoomLevel));
    }

    public static void setMousePosition(int x, int y) => Game1.setMousePosition(x, y, Game1.uiMode);

    public static void setMousePosition(Point position, bool ui_scale) => Game1.setMousePosition(position.X, position.Y, ui_scale);

    public static void setMousePosition(Point position) => Game1.setMousePosition(position, Game1.uiMode);

    public static void setMousePositionRaw(Point position) => Game1.setMousePositionRaw(position.X, position.Y);

    public static void setMousePositionRaw(int x, int y)
    {
      Game1.input.SetMousePosition(x, y);
      Game1.InvalidateOldMouseMovement();
      Game1.lastCursorMotionWasMouse = false;
    }

    public static Point getMousePositionRaw() => new Point(Game1.getMouseXRaw(), Game1.getMouseYRaw());

    public static Point getMousePosition(bool ui_scale) => new Point(Game1.getMouseX(ui_scale), Game1.getMouseY(ui_scale));

    public static Point getMousePosition() => Game1.getMousePosition(Game1.uiMode);

    private static float thumbstickToMouseModifier
    {
      get
      {
        if (Game1._cursorSpeedDirty)
          Game1.ComputeCursorSpeed();
        return (float) ((double) Game1._cursorSpeed / 720.0 * (double) Game1.viewport.Height * Game1.currentGameTime.ElapsedGameTime.TotalSeconds);
      }
    }

    private static void ComputeCursorSpeed()
    {
      Game1._cursorSpeedDirty = false;
      GamePadState gamePadState = Game1.input.GetGamePadState();
      float num1 = 0.9f;
      bool flag = false;
      Vector2 vector2 = gamePadState.ThumbSticks.Left;
      double num2 = (double) vector2.Length();
      vector2 = gamePadState.ThumbSticks.Right;
      float num3 = vector2.Length();
      double num4 = (double) num1;
      if (num2 > num4 || (double) num3 > (double) num1)
        flag = true;
      float min = 0.7f;
      float max = 2f;
      float num5 = 1f;
      if (Game1._cursorDragEnabled)
      {
        min = 0.5f;
        max = 2f;
        num5 = 1f;
      }
      if (!flag)
        num5 = -5f;
      if (Game1._cursorDragPrevEnabled != Game1._cursorDragEnabled)
        Game1._cursorSpeedScale *= 0.5f;
      Game1._cursorDragPrevEnabled = Game1._cursorDragEnabled;
      Game1._cursorSpeedScale += Game1._cursorUpdateElapsedSec * num5;
      Game1._cursorSpeedScale = MathHelper.Clamp(Game1._cursorSpeedScale, min, max);
      double num6 = 16.0 / Game1.game1.TargetElapsedTime.TotalSeconds * (double) Game1._cursorSpeedScale;
      float num7 = (float) num6 - Game1._cursorSpeed;
      Game1._cursorSpeed = (float) num6;
      Game1._cursorUpdateElapsedSec = 0.0f;
      if (!Game1.debugMode)
        return;
      Console.WriteLine("_cursorSpeed={0}, _cursorSpeedScale={1}, deltaSpeed={2}", (object) Game1._cursorSpeed.ToString("0.0"), (object) Game1._cursorSpeedScale.ToString("0.0"), (object) num7.ToString("0.0"));
    }

    private static void SetFreeCursorElapsed(float elapsedSec)
    {
      if ((double) elapsedSec == (double) Game1._cursorUpdateElapsedSec)
        return;
      Game1._cursorUpdateElapsedSec = elapsedSec;
      Game1._cursorSpeedDirty = true;
    }

    public static void ResetFreeCursorDrag()
    {
      if (Game1._cursorDragEnabled)
        Game1._cursorSpeedDirty = true;
      Game1._cursorDragEnabled = false;
    }

    public static void SetFreeCursorDrag()
    {
      if (!Game1._cursorDragEnabled)
        Game1._cursorSpeedDirty = true;
      Game1._cursorDragEnabled = true;
    }

    public static void updateActiveMenu(GameTime gameTime)
    {
      IClickableMenu iclickableMenu = Game1.activeClickableMenu;
      while (iclickableMenu.GetChildMenu() != null)
        iclickableMenu = iclickableMenu.GetChildMenu();
      if (!Program.gamePtr.IsActiveNoOverlay && Program.releaseBuild)
      {
        if (iclickableMenu == null || !iclickableMenu.IsActive())
          return;
        iclickableMenu.update(gameTime);
      }
      else
      {
        MouseState mouseState = Game1.input.GetMouseState();
        KeyboardState keyboardState = Game1.GetKeyboardState();
        GamePadState gamePadState = Game1.input.GetGamePadState();
        if (Game1.CurrentEvent != null)
        {
          if (mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released || Game1.options.gamepadControls && gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonUp(Buttons.A))
            Game1.CurrentEvent.receiveMouseClick(Game1.getMouseX(), Game1.getMouseY());
          else if (Game1.options.gamepadControls && gamePadState.IsButtonDown(Buttons.Back) && Game1.oldPadState.IsButtonUp(Buttons.Back) && !Game1.CurrentEvent.skipped && Game1.CurrentEvent.skippable)
          {
            Game1.CurrentEvent.skipped = true;
            Game1.CurrentEvent.skipEvent();
            Game1.freezeControls = false;
          }
          if (Game1.CurrentEvent != null && Game1.CurrentEvent.skipped)
          {
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldKBState = keyboardState;
            Game1.oldPadState = gamePadState;
            return;
          }
        }
        if (Game1.options.gamepadControls && iclickableMenu != null && iclickableMenu.IsActive())
        {
          if (Game1.isGamePadThumbstickInMotion() && (!Game1.options.snappyMenus || iclickableMenu.overrideSnappyMenuCursorMovementBan()))
            Game1.setMousePositionRaw((int) ((double) mouseState.X + (double) gamePadState.ThumbSticks.Left.X * (double) Game1.thumbstickToMouseModifier), (int) ((double) mouseState.Y - (double) gamePadState.ThumbSticks.Left.Y * (double) Game1.thumbstickToMouseModifier));
          if (iclickableMenu != null && iclickableMenu.IsActive() && (Game1.chatBox == null || !Game1.chatBox.isActive()))
          {
            foreach (Buttons pressedButton in Utility.getPressedButtons(gamePadState, Game1.oldPadState))
            {
              iclickableMenu.receiveGamePadButton(pressedButton);
              if (iclickableMenu == null || !iclickableMenu.IsActive())
                break;
            }
            foreach (Buttons heldButton in Utility.getHeldButtons(gamePadState))
            {
              if (iclickableMenu != null && iclickableMenu.IsActive())
                iclickableMenu.gamePadButtonHeld(heldButton);
              if (iclickableMenu == null || !iclickableMenu.IsActive())
                break;
            }
          }
        }
        if ((Game1.getMouseX() != Game1.getOldMouseX() || Game1.getMouseY() != Game1.getOldMouseY()) && !Game1.isGamePadThumbstickInMotion() && !Game1.isDPadPressed())
          Game1.lastCursorMotionWasMouse = true;
        Game1.ResetFreeCursorDrag();
        if (iclickableMenu != null && iclickableMenu.IsActive())
          iclickableMenu.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
        if (iclickableMenu != null && iclickableMenu.IsActive())
          iclickableMenu.update(gameTime);
        if (iclickableMenu != null && iclickableMenu.IsActive() && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released)
        {
          if (Game1.chatBox != null && Game1.chatBox.isActive() && Game1.chatBox.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()))
            Game1.chatBox.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY(), true);
          else
            iclickableMenu.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY());
        }
        else if (iclickableMenu != null && iclickableMenu.IsActive() && mouseState.RightButton == ButtonState.Pressed && (Game1.oldMouseState.RightButton == ButtonState.Released || (double) Game1.mouseClickPolling > 650.0 && !(iclickableMenu is DialogueBox)))
        {
          iclickableMenu.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
          if ((double) Game1.mouseClickPolling > 650.0)
            Game1.mouseClickPolling = 600;
          if ((iclickableMenu == null || !iclickableMenu.IsActive()) && Game1.activeClickableMenu == null)
          {
            Game1.rightClickPolling = 500;
            Game1.mouseClickPolling = 0;
          }
        }
        if (mouseState.ScrollWheelValue != Game1.oldMouseState.ScrollWheelValue && iclickableMenu != null && iclickableMenu.IsActive())
        {
          if (Game1.chatBox != null && Game1.chatBox.choosingEmoji && Game1.chatBox.emojiMenu.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
            Game1.chatBox.receiveScrollWheelAction(mouseState.ScrollWheelValue - Game1.oldMouseState.ScrollWheelValue);
          else
            iclickableMenu.receiveScrollWheelAction(mouseState.ScrollWheelValue - Game1.oldMouseState.ScrollWheelValue);
        }
        if (Game1.options.gamepadControls && iclickableMenu != null && iclickableMenu.IsActive())
        {
          Game1.thumbstickPollingTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
          if (Game1.thumbstickPollingTimer <= 0)
          {
            if ((double) gamePadState.ThumbSticks.Right.Y > 0.200000002980232)
              iclickableMenu.receiveScrollWheelAction(1);
            else if ((double) gamePadState.ThumbSticks.Right.Y < -0.200000002980232)
              iclickableMenu.receiveScrollWheelAction(-1);
          }
          if (Game1.thumbstickPollingTimer <= 0)
            Game1.thumbstickPollingTimer = 220 - (int) ((double) Math.Abs(gamePadState.ThumbSticks.Right.Y) * 170.0);
          if ((double) Math.Abs(gamePadState.ThumbSticks.Right.Y) < 0.200000002980232)
            Game1.thumbstickPollingTimer = 0;
        }
        if (iclickableMenu != null && iclickableMenu.IsActive() && mouseState.LeftButton == ButtonState.Released && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
          iclickableMenu.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
        else if (iclickableMenu != null && iclickableMenu.IsActive() && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
          iclickableMenu.leftClickHeld(Game1.getMouseX(), Game1.getMouseY());
        foreach (Keys pressedKey in keyboardState.GetPressedKeys())
        {
          if (iclickableMenu != null && iclickableMenu.IsActive() && !((IEnumerable<Keys>) Game1.oldKBState.GetPressedKeys()).Contains<Keys>(pressedKey))
            iclickableMenu.receiveKeyPress(pressedKey);
        }
        TimeSpan elapsedGameTime;
        if (Game1.chatBox == null || !Game1.chatBox.isActive())
        {
          if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveUpButton) || Game1.options.snappyMenus && Game1.options.gamepadControls && ((double) Math.Abs(gamePadState.ThumbSticks.Left.X) < (double) gamePadState.ThumbSticks.Left.Y || gamePadState.IsButtonDown(Buttons.DPadUp)))
          {
            ref int local = ref Game1.directionKeyPolling[0];
            int num = local;
            elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
            int milliseconds = elapsedGameTime.Milliseconds;
            local = num - milliseconds;
          }
          else if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveRightButton) || Game1.options.snappyMenus && Game1.options.gamepadControls && ((double) gamePadState.ThumbSticks.Left.X > (double) Math.Abs(gamePadState.ThumbSticks.Left.Y) || gamePadState.IsButtonDown(Buttons.DPadRight)))
          {
            ref int local = ref Game1.directionKeyPolling[1];
            int num = local;
            elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
            int milliseconds = elapsedGameTime.Milliseconds;
            local = num - milliseconds;
          }
          else if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveDownButton) || Game1.options.snappyMenus && Game1.options.gamepadControls && ((double) Math.Abs(gamePadState.ThumbSticks.Left.X) < (double) Math.Abs(gamePadState.ThumbSticks.Left.Y) || gamePadState.IsButtonDown(Buttons.DPadDown)))
          {
            ref int local = ref Game1.directionKeyPolling[2];
            int num = local;
            elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
            int milliseconds = elapsedGameTime.Milliseconds;
            local = num - milliseconds;
          }
          else if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveLeftButton) || Game1.options.snappyMenus && Game1.options.gamepadControls && ((double) Math.Abs(gamePadState.ThumbSticks.Left.X) > (double) Math.Abs(gamePadState.ThumbSticks.Left.Y) || gamePadState.IsButtonDown(Buttons.DPadLeft)))
          {
            ref int local = ref Game1.directionKeyPolling[3];
            int num = local;
            elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
            int milliseconds = elapsedGameTime.Milliseconds;
            local = num - milliseconds;
          }
          if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveUpButton) && (!Game1.options.snappyMenus || !Game1.options.gamepadControls || (double) gamePadState.ThumbSticks.Left.Y < 0.1 && gamePadState.IsButtonUp(Buttons.DPadUp)))
            Game1.directionKeyPolling[0] = 250;
          if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveRightButton) && (!Game1.options.snappyMenus || !Game1.options.gamepadControls || (double) gamePadState.ThumbSticks.Left.X < 0.1 && gamePadState.IsButtonUp(Buttons.DPadRight)))
            Game1.directionKeyPolling[1] = 250;
          if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveDownButton) && (!Game1.options.snappyMenus || !Game1.options.gamepadControls || (double) gamePadState.ThumbSticks.Left.Y > -0.1 && gamePadState.IsButtonUp(Buttons.DPadDown)))
            Game1.directionKeyPolling[2] = 250;
          if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveLeftButton) && (!Game1.options.snappyMenus || !Game1.options.gamepadControls || (double) gamePadState.ThumbSticks.Left.X > -0.1 && gamePadState.IsButtonUp(Buttons.DPadLeft)))
            Game1.directionKeyPolling[3] = 250;
          if (Game1.directionKeyPolling[0] <= 0 && iclickableMenu != null && iclickableMenu.IsActive())
          {
            iclickableMenu.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton));
            Game1.directionKeyPolling[0] = 70;
          }
          if (Game1.directionKeyPolling[1] <= 0 && iclickableMenu != null && iclickableMenu.IsActive())
          {
            iclickableMenu.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton));
            Game1.directionKeyPolling[1] = 70;
          }
          if (Game1.directionKeyPolling[2] <= 0 && iclickableMenu != null && iclickableMenu.IsActive())
          {
            iclickableMenu.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton));
            Game1.directionKeyPolling[2] = 70;
          }
          if (Game1.directionKeyPolling[3] <= 0 && iclickableMenu != null && iclickableMenu.IsActive())
          {
            iclickableMenu.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton));
            Game1.directionKeyPolling[3] = 70;
          }
          if (Game1.options.gamepadControls && iclickableMenu != null && iclickableMenu.IsActive())
          {
            if (!iclickableMenu.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.A) && (!Game1.oldPadState.IsButtonDown(Buttons.A) || (double) Game1.gamePadAButtonPolling > 650.0 && !(iclickableMenu is DialogueBox)))
            {
              iclickableMenu.receiveLeftClick(Game1.getMousePosition().X, Game1.getMousePosition().Y);
              if ((double) Game1.gamePadAButtonPolling > 650.0)
                Game1.gamePadAButtonPolling = 600;
            }
            else if (!iclickableMenu.areGamePadControlsImplemented() && !gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
              iclickableMenu.releaseLeftClick(Game1.getMousePosition().X, Game1.getMousePosition().Y);
            else if (!iclickableMenu.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.X) && (!Game1.oldPadState.IsButtonDown(Buttons.X) || (double) Game1.gamePadXButtonPolling > 650.0 && !(iclickableMenu is DialogueBox)))
            {
              iclickableMenu.receiveRightClick(Game1.getMousePosition().X, Game1.getMousePosition().Y);
              if ((double) Game1.gamePadXButtonPolling > 650.0)
                Game1.gamePadXButtonPolling = 600;
            }
            foreach (Buttons pressedButton in Utility.getPressedButtons(gamePadState, Game1.oldPadState))
            {
              if (iclickableMenu != null && iclickableMenu.IsActive())
              {
                Keys key = Utility.mapGamePadButtonToKey(pressedButton);
                if (!(iclickableMenu is FarmhandMenu) || Game1.game1.IsMainInstance || !Game1.options.doesInputListContain(Game1.options.menuButton, key))
                  iclickableMenu.receiveKeyPress(key);
              }
              else
                break;
            }
            if (iclickableMenu != null && iclickableMenu.IsActive() && !iclickableMenu.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
              iclickableMenu.leftClickHeld(Game1.getMousePosition().X, Game1.getMousePosition().Y);
            if (gamePadState.IsButtonDown(Buttons.X))
            {
              int padXbuttonPolling = Game1.gamePadXButtonPolling;
              elapsedGameTime = gameTime.ElapsedGameTime;
              int milliseconds = elapsedGameTime.Milliseconds;
              Game1.gamePadXButtonPolling = padXbuttonPolling + milliseconds;
            }
            else
              Game1.gamePadXButtonPolling = 0;
            if (gamePadState.IsButtonDown(Buttons.A))
            {
              int padAbuttonPolling = Game1.gamePadAButtonPolling;
              elapsedGameTime = gameTime.ElapsedGameTime;
              int milliseconds = elapsedGameTime.Milliseconds;
              Game1.gamePadAButtonPolling = padAbuttonPolling + milliseconds;
            }
            else
              Game1.gamePadAButtonPolling = 0;
            if (!iclickableMenu.IsActive() && Game1.activeClickableMenu == null)
            {
              Game1.rightClickPolling = 500;
              Game1.gamePadXButtonPolling = 0;
              Game1.gamePadAButtonPolling = 0;
            }
          }
        }
        else
        {
          int num1 = Game1.options.SnappyMenus ? 1 : 0;
        }
        if (mouseState.RightButton == ButtonState.Pressed)
        {
          int mouseClickPolling = Game1.mouseClickPolling;
          elapsedGameTime = gameTime.ElapsedGameTime;
          int milliseconds = elapsedGameTime.Milliseconds;
          Game1.mouseClickPolling = mouseClickPolling + milliseconds;
        }
        else
          Game1.mouseClickPolling = 0;
        Game1.oldMouseState = Game1.input.GetMouseState();
        Game1.oldKBState = keyboardState;
        Game1.oldPadState = gamePadState;
      }
    }

    public static void AdjustScreenScale(float offset)
    {
    }

    public void ShowScreenScaleMenu()
    {
      switch (Game1.activeClickableMenu)
      {
        case null:
        case ScreenSizeAdjustMenu _:
          if (Game1.activeClickableMenu != null)
            break;
          Game1.activeClickableMenu = (IClickableMenu) new ScreenSizeAdjustMenu();
          break;
        default:
          Game1.activeClickableMenu.SetChildMenu((IClickableMenu) new ScreenSizeAdjustMenu());
          break;
      }
    }

    public bool ShowLocalCoopJoinMenu()
    {
      if (!this.IsMainInstance || Game1.gameMode != (byte) 3)
        return false;
      int num = 0;
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building.indoors.Value is Cabin)
        {
          Farmer farmer = (building.indoors.Value as Cabin).farmhand.Value;
          if (farmer == null)
            ++num;
          else if (!farmer.isActive())
            ++num;
        }
      }
      if (num == 0)
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:CoopMenu_NoSlots"));
        return false;
      }
      if (Game1.currentMinigame != null || Game1.activeClickableMenu != null || !this.IsLocalCoopJoinable())
        return false;
      Game1.playSound("bigSelect");
      Game1.activeClickableMenu = (IClickableMenu) new LocalCoopJoinMenu();
      return true;
    }

    public static void updateTextEntry(GameTime gameTime)
    {
      MouseState mouseState = Game1.input.GetMouseState();
      KeyboardState keyboardState = Game1.GetKeyboardState();
      GamePadState gamePadState = Game1.input.GetGamePadState();
      ButtonCollection buttonCollection;
      if (Game1.options.gamepadControls)
      {
        switch (Game1.textEntry)
        {
          case null:
          case null:
            break;
          default:
            buttonCollection = Utility.getPressedButtons(gamePadState, Game1.oldPadState);
            foreach (Buttons b in buttonCollection)
            {
              Game1.textEntry.receiveGamePadButton(b);
              if (Game1.textEntry == null)
                break;
            }
            buttonCollection = Utility.getHeldButtons(gamePadState);
            foreach (Buttons b in buttonCollection)
            {
              if (Game1.textEntry != null)
                Game1.textEntry.gamePadButtonHeld(b);
              if (Game1.textEntry == null)
                break;
            }
            break;
        }
      }
      if (Game1.textEntry != null)
        Game1.textEntry.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
      if (Game1.textEntry != null)
        Game1.textEntry.update(gameTime);
      if (Game1.textEntry != null && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released)
        Game1.textEntry.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY(), true);
      else if (Game1.textEntry != null && mouseState.RightButton == ButtonState.Pressed && (Game1.oldMouseState.RightButton == ButtonState.Released || (double) Game1.mouseClickPolling > 650.0))
      {
        Game1.textEntry.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
        if ((double) Game1.mouseClickPolling > 650.0)
          Game1.mouseClickPolling = 600;
        if (Game1.textEntry == null)
        {
          Game1.rightClickPolling = 500;
          Game1.mouseClickPolling = 0;
        }
      }
      if (mouseState.ScrollWheelValue != Game1.oldMouseState.ScrollWheelValue && Game1.textEntry != null)
      {
        if (Game1.chatBox != null && Game1.chatBox.choosingEmoji && Game1.chatBox.emojiMenu.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
          Game1.chatBox.receiveScrollWheelAction(mouseState.ScrollWheelValue - Game1.oldMouseState.ScrollWheelValue);
        else
          Game1.textEntry.receiveScrollWheelAction(mouseState.ScrollWheelValue - Game1.oldMouseState.ScrollWheelValue);
      }
      TimeSpan elapsedGameTime;
      GamePadThumbSticks thumbSticks;
      if (Game1.options.gamepadControls && Game1.textEntry != null)
      {
        int thumbstickPollingTimer = Game1.thumbstickPollingTimer;
        elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        Game1.thumbstickPollingTimer = thumbstickPollingTimer - milliseconds;
        if (Game1.thumbstickPollingTimer <= 0)
        {
          if ((double) gamePadState.ThumbSticks.Right.Y > 0.200000002980232)
            Game1.textEntry.receiveScrollWheelAction(1);
          else if ((double) gamePadState.ThumbSticks.Right.Y < -0.200000002980232)
            Game1.textEntry.receiveScrollWheelAction(-1);
        }
        if (Game1.thumbstickPollingTimer <= 0)
        {
          thumbSticks = gamePadState.ThumbSticks;
          Game1.thumbstickPollingTimer = 220 - (int) ((double) Math.Abs(thumbSticks.Right.Y) * 170.0);
        }
        thumbSticks = gamePadState.ThumbSticks;
        if ((double) Math.Abs(thumbSticks.Right.Y) < 0.200000002980232)
          Game1.thumbstickPollingTimer = 0;
      }
      if (Game1.textEntry != null && mouseState.LeftButton == ButtonState.Released && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
        Game1.textEntry.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
      else if (Game1.textEntry != null && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
        Game1.textEntry.leftClickHeld(Game1.getMouseX(), Game1.getMouseY());
      foreach (Keys pressedKey in keyboardState.GetPressedKeys())
      {
        if (Game1.textEntry != null && !((IEnumerable<Keys>) Game1.oldKBState.GetPressedKeys()).Contains<Keys>(pressedKey))
          Game1.textEntry.receiveKeyPress(pressedKey);
      }
      if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveUpButton))
      {
        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
        {
          thumbSticks = gamePadState.ThumbSticks;
          double num = (double) Math.Abs(thumbSticks.Left.X);
          thumbSticks = gamePadState.ThumbSticks;
          double y = (double) thumbSticks.Left.Y;
          if (num < y || gamePadState.IsButtonDown(Buttons.DPadUp))
            goto label_47;
        }
        if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveRightButton))
        {
          if (Game1.options.snappyMenus && Game1.options.gamepadControls)
          {
            thumbSticks = gamePadState.ThumbSticks;
            double x = (double) thumbSticks.Left.X;
            thumbSticks = gamePadState.ThumbSticks;
            double num = (double) Math.Abs(thumbSticks.Left.Y);
            if (x > num || gamePadState.IsButtonDown(Buttons.DPadRight))
              goto label_51;
          }
          if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveDownButton))
          {
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
              thumbSticks = gamePadState.ThumbSticks;
              double num1 = (double) Math.Abs(thumbSticks.Left.X);
              thumbSticks = gamePadState.ThumbSticks;
              double num2 = (double) Math.Abs(thumbSticks.Left.Y);
              if (num1 < num2 || gamePadState.IsButtonDown(Buttons.DPadDown))
                goto label_55;
            }
            if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveLeftButton))
            {
              if (Game1.options.snappyMenus && Game1.options.gamepadControls)
              {
                thumbSticks = gamePadState.ThumbSticks;
                double num3 = (double) Math.Abs(thumbSticks.Left.X);
                thumbSticks = gamePadState.ThumbSticks;
                double num4 = (double) Math.Abs(thumbSticks.Left.Y);
                if (num3 <= num4 && !gamePadState.IsButtonDown(Buttons.DPadLeft))
                  goto label_60;
              }
              else
                goto label_60;
            }
            ref int local = ref Game1.directionKeyPolling[3];
            int num = local;
            elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
            int milliseconds = elapsedGameTime.Milliseconds;
            local = num - milliseconds;
            goto label_60;
          }
label_55:
          ref int local1 = ref Game1.directionKeyPolling[2];
          int num5 = local1;
          elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
          int milliseconds1 = elapsedGameTime.Milliseconds;
          local1 = num5 - milliseconds1;
          goto label_60;
        }
label_51:
        ref int local2 = ref Game1.directionKeyPolling[1];
        int num6 = local2;
        elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
        int milliseconds2 = elapsedGameTime.Milliseconds;
        local2 = num6 - milliseconds2;
        goto label_60;
      }
label_47:
      ref int local3 = ref Game1.directionKeyPolling[0];
      int num7 = local3;
      elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
      int milliseconds3 = elapsedGameTime.Milliseconds;
      local3 = num7 - milliseconds3;
label_60:
      if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveUpButton))
      {
        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
        {
          thumbSticks = gamePadState.ThumbSticks;
          if ((double) thumbSticks.Left.Y >= 0.1 || !gamePadState.IsButtonUp(Buttons.DPadUp))
            goto label_64;
        }
        Game1.directionKeyPolling[0] = 250;
      }
label_64:
      if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveRightButton))
      {
        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
        {
          thumbSticks = gamePadState.ThumbSticks;
          if ((double) thumbSticks.Left.X >= 0.1 || !gamePadState.IsButtonUp(Buttons.DPadRight))
            goto label_68;
        }
        Game1.directionKeyPolling[1] = 250;
      }
label_68:
      if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveDownButton))
      {
        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
        {
          thumbSticks = gamePadState.ThumbSticks;
          if ((double) thumbSticks.Left.Y <= -0.1 || !gamePadState.IsButtonUp(Buttons.DPadDown))
            goto label_72;
        }
        Game1.directionKeyPolling[2] = 250;
      }
label_72:
      if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveLeftButton))
      {
        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
        {
          thumbSticks = gamePadState.ThumbSticks;
          if ((double) thumbSticks.Left.X <= -0.1 || !gamePadState.IsButtonUp(Buttons.DPadLeft))
            goto label_76;
        }
        Game1.directionKeyPolling[3] = 250;
      }
label_76:
      if (Game1.directionKeyPolling[0] <= 0 && Game1.textEntry != null)
      {
        Game1.textEntry.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton));
        Game1.directionKeyPolling[0] = 70;
      }
      if (Game1.directionKeyPolling[1] <= 0 && Game1.textEntry != null)
      {
        Game1.textEntry.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton));
        Game1.directionKeyPolling[1] = 70;
      }
      if (Game1.directionKeyPolling[2] <= 0 && Game1.textEntry != null)
      {
        Game1.textEntry.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton));
        Game1.directionKeyPolling[2] = 70;
      }
      if (Game1.directionKeyPolling[3] <= 0 && Game1.textEntry != null)
      {
        Game1.textEntry.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton));
        Game1.directionKeyPolling[3] = 70;
      }
      if (Game1.options.gamepadControls && Game1.textEntry != null)
      {
        if (!Game1.textEntry.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.A) && (!Game1.oldPadState.IsButtonDown(Buttons.A) || (double) Game1.gamePadAButtonPolling > 650.0))
        {
          Game1.textEntry.receiveLeftClick(Game1.getMousePosition().X, Game1.getMousePosition().Y, true);
          if ((double) Game1.gamePadAButtonPolling > 650.0)
            Game1.gamePadAButtonPolling = 600;
        }
        else if (!Game1.textEntry.areGamePadControlsImplemented() && !gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
          Game1.textEntry.releaseLeftClick(Game1.getMousePosition().X, Game1.getMousePosition().Y);
        else if (!Game1.textEntry.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.X) && (!Game1.oldPadState.IsButtonDown(Buttons.X) || (double) Game1.gamePadXButtonPolling > 650.0))
        {
          Game1.textEntry.receiveRightClick(Game1.getMousePosition().X, Game1.getMousePosition().Y);
          if ((double) Game1.gamePadXButtonPolling > 650.0)
            Game1.gamePadXButtonPolling = 600;
        }
        buttonCollection = Utility.getPressedButtons(gamePadState, Game1.oldPadState);
        foreach (Buttons b in buttonCollection)
        {
          if (Game1.textEntry != null)
            Game1.textEntry.receiveKeyPress(Utility.mapGamePadButtonToKey(b));
          else
            break;
        }
        if (Game1.textEntry != null && !Game1.textEntry.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
          Game1.textEntry.leftClickHeld(Game1.getMousePosition().X, Game1.getMousePosition().Y);
        if (gamePadState.IsButtonDown(Buttons.X))
        {
          int padXbuttonPolling = Game1.gamePadXButtonPolling;
          elapsedGameTime = gameTime.ElapsedGameTime;
          int milliseconds4 = elapsedGameTime.Milliseconds;
          Game1.gamePadXButtonPolling = padXbuttonPolling + milliseconds4;
        }
        else
          Game1.gamePadXButtonPolling = 0;
        if (gamePadState.IsButtonDown(Buttons.A))
        {
          int padAbuttonPolling = Game1.gamePadAButtonPolling;
          elapsedGameTime = gameTime.ElapsedGameTime;
          int milliseconds5 = elapsedGameTime.Milliseconds;
          Game1.gamePadAButtonPolling = padAbuttonPolling + milliseconds5;
        }
        else
          Game1.gamePadAButtonPolling = 0;
        if (Game1.textEntry == null)
        {
          Game1.rightClickPolling = 500;
          Game1.gamePadAButtonPolling = 0;
          Game1.gamePadXButtonPolling = 0;
        }
      }
      if (mouseState.RightButton == ButtonState.Pressed)
      {
        int mouseClickPolling = Game1.mouseClickPolling;
        elapsedGameTime = gameTime.ElapsedGameTime;
        int milliseconds6 = elapsedGameTime.Milliseconds;
        Game1.mouseClickPolling = mouseClickPolling + milliseconds6;
      }
      else
        Game1.mouseClickPolling = 0;
      Game1.oldMouseState = Game1.input.GetMouseState();
      Game1.oldKBState = keyboardState;
      Game1.oldPadState = gamePadState;
    }

    public static string DateCompiled()
    {
      System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
      return version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString() + "." + version.Revision.ToString();
    }

    public static void updatePause(GameTime gameTime)
    {
      Game1.pauseTime -= (float) gameTime.ElapsedGameTime.Milliseconds;
      if (Game1.player.isCrafting && Game1.random.NextDouble() < 0.007)
        Game1.playSound("crafting");
      if ((double) Game1.pauseTime > 0.0)
        return;
      if (Game1.currentObjectDialogue.Count == 0)
        Game1.messagePause = false;
      Game1.pauseTime = 0.0f;
      if (Game1.messageAfterPause != null && !Game1.messageAfterPause.Equals(""))
      {
        Game1.player.isCrafting = false;
        Game1.drawObjectDialogue(Game1.messageAfterPause);
        Game1.messageAfterPause = "";
        if (Game1.player.ActiveObject != null)
        {
          int num = (bool) (NetFieldBase<bool, NetBool>) Game1.player.ActiveObject.bigCraftable ? 1 : 0;
        }
        if (Game1.killScreen)
        {
          Game1.killScreen = false;
          Game1.player.health = 10;
        }
      }
      else if (Game1.killScreen)
      {
        Game1.multiplayer.globalChatInfoMessage("PlayerDeath", Game1.player.Name);
        Game1.screenGlow = false;
        if (Game1.currentLocation.Name.StartsWith("UndergroundMine") && Game1.mine.getMineArea() != 121)
          Game1.warpFarmer("Mine", 22, 9, false);
        else if (Game1.currentLocation is IslandLocation)
          Game1.warpFarmer("IslandSouth", 13, 33, false);
        else
          Game1.warpFarmer("Hospital", 20, 12, false);
      }
      Game1.progressBar = false;
      if (Game1.currentLocation.currentEvent == null)
        return;
      ++Game1.currentLocation.currentEvent.CurrentCommand;
    }

    public static void CheckValidFullscreenResolution(ref int width, ref int height)
    {
      int num1 = width;
      int num2 = height;
      foreach (Microsoft.Xna.Framework.Graphics.DisplayMode supportedDisplayMode in Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
      {
        if (supportedDisplayMode.Width >= 1280 && supportedDisplayMode.Width == num1 && supportedDisplayMode.Height == num2)
        {
          width = num1;
          height = num2;
          return;
        }
      }
      foreach (Microsoft.Xna.Framework.Graphics.DisplayMode supportedDisplayMode in Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
      {
        if (supportedDisplayMode.Width >= 1280 && supportedDisplayMode.Width == Game1.graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width && supportedDisplayMode.Height == Game1.graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height)
        {
          width = Game1.graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
          height = Game1.graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
          return;
        }
      }
      bool flag = false;
      foreach (Microsoft.Xna.Framework.Graphics.DisplayMode supportedDisplayMode in Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
      {
        if (supportedDisplayMode.Width >= 1280 && num1 > supportedDisplayMode.Width)
        {
          width = supportedDisplayMode.Width;
          height = supportedDisplayMode.Height;
          flag = true;
        }
      }
      if (flag)
        return;
      Console.WriteLine("Requested fullscreen resolution not valid, switching to windowed.");
      width = 1280;
      height = 720;
      Game1.options.fullscreen = false;
    }

    public static void toggleNonBorderlessWindowedFullscreen()
    {
      int width = Game1.options.preferredResolutionX;
      int height = Game1.options.preferredResolutionY;
      Game1.graphics.HardwareModeSwitch = Game1.options.fullscreen && !Game1.options.windowedBorderlessFullscreen;
      if (Game1.options.fullscreen && !Game1.options.windowedBorderlessFullscreen)
        Game1.CheckValidFullscreenResolution(ref width, ref height);
      if (!Game1.options.fullscreen && !Game1.options.windowedBorderlessFullscreen)
      {
        width = 1280;
        height = 720;
      }
      Game1.graphics.PreferredBackBufferWidth = width;
      Game1.graphics.PreferredBackBufferHeight = height;
      if (Game1.options.fullscreen != Game1.graphics.IsFullScreen)
        Game1.graphics.ToggleFullScreen();
      Game1.graphics.ApplyChanges();
      Game1.updateViewportForScreenSizeChange(true, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
      GameRunner.instance.OnWindowSizeChange((object) null, (EventArgs) null);
    }

    public static void toggleFullscreen()
    {
      if (Game1.options.windowedBorderlessFullscreen)
      {
        Game1.graphics.HardwareModeSwitch = false;
        Game1.graphics.IsFullScreen = true;
        Game1.graphics.ApplyChanges();
        Game1.graphics.PreferredBackBufferWidth = Program.gamePtr.Window.ClientBounds.Width;
        Game1.graphics.PreferredBackBufferHeight = Program.gamePtr.Window.ClientBounds.Height;
      }
      else
        Game1.toggleNonBorderlessWindowedFullscreen();
      GameRunner.instance.OnWindowSizeChange((object) null, (EventArgs) null);
    }

    public static bool isFullscreen => Game1.graphics.IsFullScreen;

    private void checkForEscapeKeys()
    {
      KeyboardState keyboardState = Game1.input.GetKeyboardState();
      if (!this.IsMainInstance)
        return;
      if (keyboardState.IsKeyDown(Keys.LeftAlt) && keyboardState.IsKeyDown(Keys.Enter) && (Game1.oldKBState.IsKeyUp(Keys.LeftAlt) || Game1.oldKBState.IsKeyUp(Keys.Enter)))
      {
        if (Game1.options.isCurrentlyFullscreen() || Game1.options.isCurrentlyWindowedBorderless())
          Game1.options.setWindowedOption(1);
        else
          Game1.options.setWindowedOption(0);
      }
      if (!Game1.player.UsingTool && !Game1.freezeControls || !keyboardState.IsKeyDown(Keys.RightShift) || !keyboardState.IsKeyDown(Keys.R) || !keyboardState.IsKeyDown(Keys.Delete))
        return;
      Game1.freezeControls = false;
      Game1.player.forceCanMove();
      Game1.player.completelyStopAnimatingOrDoingAction();
      Game1.player.UsingTool = false;
    }

    public static bool IsPressEvent(ref KeyboardState state, Keys key)
    {
      if (!state.IsKeyDown(key) || Game1.oldKBState.IsKeyDown(key))
        return false;
      Game1.oldKBState = state;
      return true;
    }

    public static bool IsPressEvent(ref GamePadState state, Buttons btn)
    {
      if (!state.IsConnected || !state.IsButtonDown(btn) || Game1.oldPadState.IsButtonDown(btn))
        return false;
      Game1.oldPadState = state;
      return true;
    }

    public static bool isOneOfTheseKeysDown(KeyboardState state, InputButton[] keys)
    {
      foreach (InputButton key in keys)
      {
        if (key.key != Keys.None && state.IsKeyDown(key.key))
          return true;
      }
      return false;
    }

    public static bool areAllOfTheseKeysUp(KeyboardState state, InputButton[] keys)
    {
      foreach (InputButton key in keys)
      {
        if (key.key != Keys.None && !state.IsKeyUp(key.key))
          return false;
      }
      return true;
    }

    private void UpdateTitleScreen(GameTime time)
    {
      if (Game1.quit)
      {
        this.Exit();
        Game1.changeMusicTrack("none");
      }
      switch (Game1.gameMode)
      {
        case 6:
          Game1._requestedMusicTracks = new Dictionary<Game1.MusicContext, KeyValuePair<string, bool>>();
          Game1.requestedMusicTrack = "none";
          Game1.requestedMusicTrackOverrideable = false;
          Game1.requestedMusicDirty = true;
          if (Game1.currentLoader == null || Game1.currentLoader.MoveNext())
            break;
          if (Game1.gameMode == (byte) 3)
          {
            Game1.setGameMode((byte) 3);
            Game1.fadeIn = true;
            Game1.fadeToBlackAlpha = 0.99f;
            break;
          }
          Game1.ExitToTitle();
          break;
        case 7:
          Game1.currentLoader.MoveNext();
          break;
        case 8:
          Game1.pauseAccumulator -= (float) time.ElapsedGameTime.Milliseconds;
          if ((double) Game1.pauseAccumulator > 0.0)
            break;
          Game1.pauseAccumulator = 0.0f;
          Game1.setGameMode((byte) 3);
          if (Game1.currentObjectDialogue.Count <= 0)
            break;
          Game1.messagePause = true;
          Game1.pauseTime = 1E+10f;
          Game1.fadeToBlackAlpha = 1f;
          Game1.player.CanMove = false;
          break;
        default:
          if (Game1.game1.instanceIndex > 0)
          {
            if (Game1.activeClickableMenu != null || Game1.ticks <= 1)
              break;
            Game1.activeClickableMenu = (IClickableMenu) new FarmhandMenu(Game1.multiplayer.InitClient((Client) new LidgrenClient("localhost")));
            Game1.activeClickableMenu.populateClickableComponentList();
            if (!Game1.options.SnappyMenus)
              break;
            Game1.activeClickableMenu.snapToDefaultClickableComponent();
            break;
          }
          if ((double) Game1.fadeToBlackAlpha < 1.0 && Game1.fadeIn)
            Game1.fadeToBlackAlpha += 0.02f;
          else if ((double) Game1.fadeToBlackAlpha > 0.0 && Game1.fadeToBlack)
            Game1.fadeToBlackAlpha -= 0.02f;
          if ((double) Game1.pauseTime > 0.0)
            Game1.pauseTime = Math.Max(0.0f, Game1.pauseTime - (float) time.ElapsedGameTime.Milliseconds);
          if (Game1.gameMode == (byte) 0 && (double) Game1.fadeToBlackAlpha >= 0.98)
          {
            double fadeToBlackAlpha = (double) Game1.fadeToBlackAlpha;
          }
          if ((double) Game1.fadeToBlackAlpha >= 1.0)
          {
            if (Game1.gameMode == (byte) 4 && !Game1.fadeToBlack)
            {
              Game1.fadeIn = false;
              Game1.fadeToBlack = true;
              Game1.fadeToBlackAlpha = 2.5f;
            }
            else if (Game1.gameMode == (byte) 0 && Game1.currentSong == null && Game1.soundBank != null && (double) Game1.pauseTime <= 0.0 && this.IsMainInstance && Game1.soundBank != null)
            {
              Game1.currentSong = Game1.soundBank.GetCue("spring_day_ambient");
              Game1.currentSong.Play();
            }
            if (Game1.gameMode != (byte) 0 || Game1.activeClickableMenu != null || Game1.quit)
              break;
            Game1.activeClickableMenu = (IClickableMenu) new TitleMenu();
            break;
          }
          if ((double) Game1.fadeToBlackAlpha > 0.0)
            break;
          if (Game1.gameMode == (byte) 4 && Game1.fadeToBlack)
          {
            Game1.fadeIn = true;
            Game1.fadeToBlack = false;
            Game1.setGameMode((byte) 0);
            Game1.pauseTime = 2000f;
            break;
          }
          if (Game1.gameMode != (byte) 0 || !Game1.fadeToBlack || Game1.menuChoice != 0)
            break;
          Game1.currentLoader = Utility.generateNewFarm(Game1.IsClient);
          Game1.setGameMode((byte) 6);
          Game1.loadingMessage = Game1.IsClient ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2574", (object) Game1.client.serverName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2575");
          Game1.exitActiveMenu();
          break;
      }
    }

    private void UpdateLocations(GameTime time)
    {
      Game1.locationCues.Update(Game1.currentLocation);
      if (Game1.menuUp && !Game1.IsMultiplayer)
        return;
      if (Game1.IsClient)
      {
        Game1.currentLocation.UpdateWhenCurrentLocation(time);
        foreach (GameLocation activeLocation in Game1.multiplayer.activeLocations())
        {
          activeLocation.updateEvenIfFarmerIsntHere(time);
          if (activeLocation is BuildableGameLocation)
          {
            foreach (Building building in (activeLocation as BuildableGameLocation).buildings)
            {
              if (building.indoors.Value != null && building.indoors.Value != Game1.currentLocation)
                building.indoors.Value.updateEvenIfFarmerIsntHere(time);
            }
          }
        }
      }
      else
      {
        foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
        {
          bool flag = location.farmers.Any();
          if (!flag && location.CanBeRemotedlyViewed())
          {
            if (Game1.player.currentLocation == location)
            {
              flag = true;
            }
            else
            {
              foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
              {
                if (farmer.viewingLocation.Value != null && farmer.viewingLocation.Value.Equals(location.Name))
                {
                  flag = true;
                  break;
                }
              }
            }
          }
          if (flag)
            location.UpdateWhenCurrentLocation(time);
          location.updateEvenIfFarmerIsntHere(time);
          if (location.wasInhabited != flag)
          {
            location.wasInhabited = flag;
            if (Game1.IsMasterGame)
              location.cleanupForVacancy();
          }
          if (location is BuildableGameLocation)
          {
            foreach (Building building in (location as BuildableGameLocation).buildings)
            {
              GameLocation gameLocation = building.indoors.Value;
              if (gameLocation != null)
              {
                if (gameLocation.farmers.Any())
                  gameLocation.UpdateWhenCurrentLocation(time);
                gameLocation.updateEvenIfFarmerIsntHere(time);
              }
            }
          }
        }
        if (Game1.currentLocation.isTemp())
        {
          Game1.currentLocation.UpdateWhenCurrentLocation(time);
          Game1.currentLocation.updateEvenIfFarmerIsntHere(time);
        }
        MineShaft.UpdateMines(time);
        VolcanoDungeon.UpdateLevels(time);
      }
    }

    public static void performTenMinuteClockUpdate() => Game1.hooks.OnGame1_PerformTenMinuteClockUpdate((Action) (() =>
    {
      int trulyDarkTime = Game1.getTrulyDarkTime();
      Game1.gameTimeInterval = 0;
      if (Game1.IsMasterGame)
        Game1.timeOfDay += 10;
      if (Game1.timeOfDay % 100 >= 60)
        Game1.timeOfDay = Game1.timeOfDay - Game1.timeOfDay % 100 + 100;
      Game1.timeOfDay = Math.Min(Game1.timeOfDay, 2600);
      if (Game1.isLightning && Game1.timeOfDay < 2400 && Game1.IsMasterGame)
        Utility.performLightningUpdate(Game1.timeOfDay);
      if (Game1.timeOfDay == trulyDarkTime)
        Game1.currentLocation.switchOutNightTiles();
      else if (Game1.timeOfDay == Game1.getModeratelyDarkTime())
      {
        if (Game1.currentLocation.IsOutdoors && !Game1.IsRainingHere())
          Game1.ambientLight = Microsoft.Xna.Framework.Color.White;
        if (!Game1.IsRainingHere() && !(Game1.currentLocation is MineShaft) && Game1.currentSong != null && !Game1.currentSong.Name.Contains("ambient") && Game1.currentLocation is Town)
          Game1.changeMusicTrack("none");
      }
      if (Game1.getMusicTrackName().StartsWith(Game1.currentSeason) && !Game1.getMusicTrackName().Contains("ambient") && !Game1.eventUp && Game1.isDarkOut())
        Game1.changeMusicTrack("none", true);
      if ((bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.isOutdoors && !Game1.IsRainingHere() && !Game1.eventUp && Game1.getMusicTrackName().Contains("day") && Game1.isDarkOut())
        Game1.changeMusicTrack("none", true);
      if (Game1.weatherIcon == 1)
      {
        int int32 = Convert.ToInt32(Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + Game1.dayOfMonth.ToString())["conditions"].Split('/')[1].Split(' ')[0]);
        if (Game1.whereIsTodaysFest == null)
          Game1.whereIsTodaysFest = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + Game1.dayOfMonth.ToString())["conditions"].Split('/')[0];
        if (Game1.timeOfDay == int32)
        {
          Dictionary<string, string> dictionary = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + Game1.dayOfMonth.ToString());
          string str = dictionary["conditions"].Split('/')[0];
          if (dictionary.ContainsKey("locationDisplayName"))
            str = dictionary["locationDisplayName"];
          else if (!(str == "Forest"))
          {
            if (!(str == "Town"))
            {
              if (str == "Beach")
                str = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2639");
            }
            else
              str = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2637");
          }
          else
            str = Game1.currentSeason.Equals("winter") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2634") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2635");
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2640", (object) Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + Game1.dayOfMonth.ToString())["name"]) + str);
        }
      }
      Game1.player.performTenMinuteUpdate();
      switch (Game1.timeOfDay)
      {
        case 1200:
          if ((bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.isOutdoors && !Game1.IsRainingHere() && (Game1.currentSong == null || Game1.currentSong.IsStopped || Game1.currentSong.Name.ToLower().Contains("ambient")))
          {
            Game1.playMorningSong();
            break;
          }
          break;
        case 2000:
          if (!Game1.IsRainingHere() && Game1.currentLocation is Town)
          {
            Game1.changeMusicTrack("none");
            break;
          }
          break;
        case 2400:
          Game1.dayTimeMoneyBox.timeShakeTimer = 2000;
          Game1.player.doEmote(24);
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2652"));
          break;
        case 2500:
          Game1.dayTimeMoneyBox.timeShakeTimer = 2000;
          Game1.player.doEmote(24);
          break;
        case 2600:
          Game1.dayTimeMoneyBox.timeShakeTimer = 2000;
          if (Game1.player.mount != null)
            Game1.player.mount.dismount();
          if (Game1.player.IsSitting())
            Game1.player.StopSitting(false);
          if (Game1.player.UsingTool && (Game1.player.CurrentTool == null || !(Game1.player.CurrentTool is FishingRod currentTool2) || !currentTool2.isReeling && !currentTool2.pullingOutOfWater))
          {
            Game1.player.completelyStopAnimatingOrDoingAction();
            break;
          }
          break;
        case 2800:
          if (Game1.activeClickableMenu != null)
          {
            Game1.activeClickableMenu.emergencyShutDown();
            Game1.exitActiveMenu();
          }
          Game1.player.startToPassOut();
          if (Game1.player.mount != null)
          {
            Game1.player.mount.dismount();
            break;
          }
          break;
      }
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        GameLocation gameLocation = location;
        if (gameLocation.NameOrUniqueName == Game1.currentLocation.NameOrUniqueName)
          gameLocation = Game1.currentLocation;
        gameLocation.performTenMinuteUpdate(Game1.timeOfDay);
        if (gameLocation is Farm)
          ((BuildableGameLocation) gameLocation).timeUpdate(10);
      }
      MineShaft.UpdateMines10Minutes(Game1.timeOfDay);
      VolcanoDungeon.UpdateLevels10Minutes(Game1.timeOfDay);
      if (!Game1.IsMasterGame || Game1.farmEvent != null)
        return;
      Game1.netWorldState.Value.UpdateFromGame1();
    }));

    public static bool shouldPlayMorningSong(bool loading_game = false) => !Game1.eventUp && (double) Game1.options.musicVolumeLevel > 0.025 && Game1.timeOfDay < 1200 && (loading_game || Game1.currentSong == null || Game1.requestedMusicTrack.ToLower().Contains("ambient"));

    public static void UpdateGameClock(GameTime time)
    {
      if (Game1.shouldTimePass() && !Game1.IsClient)
        Game1.gameTimeInterval += time.ElapsedGameTime.Milliseconds;
      if (Game1.timeOfDay >= Game1.getTrulyDarkTime())
      {
        float num = Math.Min(0.93f, (float) (0.75 + ((double) ((int) ((double) (Game1.timeOfDay - Game1.timeOfDay % 100) + (double) (Game1.timeOfDay % 100 / 10) * 16.6599998474121) - Game1.getTrulyDarkTime()) + (double) Game1.gameTimeInterval / 7000.0 * 16.6000003814697) * 0.000624999986030161));
        Game1.outdoorLight = (Game1.IsRainingHere() ? Game1.ambientLight : Game1.eveningColor) * num;
      }
      else if (Game1.timeOfDay >= Game1.getStartingToGetDarkTime())
      {
        float num = Math.Min(0.93f, (float) (0.300000011920929 + ((double) ((int) ((double) (Game1.timeOfDay - Game1.timeOfDay % 100) + (double) (Game1.timeOfDay % 100 / 10) * 16.6599998474121) - Game1.getStartingToGetDarkTime()) + (double) Game1.gameTimeInterval / 7000.0 * 16.6000003814697) * 0.00224999990314245));
        Game1.outdoorLight = (Game1.IsRainingHere() ? Game1.ambientLight : Game1.eveningColor) * num;
      }
      else if (Game1.IsRainingHere())
        Game1.outdoorLight = Game1.ambientLight * 0.3f;
      if (Game1.currentLocation == null || Game1.gameTimeInterval <= 7000 + Game1.currentLocation.getExtraMillisecondsPerInGameMinuteForThisLocation())
        return;
      if (Game1.panMode)
        Game1.gameTimeInterval = 0;
      else
        Game1.performTenMinuteClockUpdate();
    }

    public static Event getAvailableWeddingEvent()
    {
      if (Game1.weddingsToday.Count <= 0)
        return (Event) null;
      long id = Game1.weddingsToday[0];
      Game1.weddingsToday.RemoveAt(0);
      Farmer farmerMaybeOffline1 = Game1.getFarmerMaybeOffline(id);
      if (farmerMaybeOffline1 == null)
        return (Event) null;
      if (farmerMaybeOffline1.hasRoommate())
        return (Event) null;
      if (Game1.IsMultiplayer)
      {
        Farmer farmer = (farmerMaybeOffline1.NetFields.Root as NetRoot<Farmer>).Clone().Value;
      }
      Event availableWeddingEvent;
      if (farmerMaybeOffline1.spouse != null)
      {
        availableWeddingEvent = Utility.getWeddingEvent(farmerMaybeOffline1);
      }
      else
      {
        long? spouse = farmerMaybeOffline1.team.GetSpouse(farmerMaybeOffline1.UniqueMultiplayerID);
        Farmer farmerMaybeOffline2 = Game1.getFarmerMaybeOffline(spouse.Value);
        if (farmerMaybeOffline2 == null)
          return (Event) null;
        if (!Game1.getOnlineFarmers().Contains(farmerMaybeOffline1) || !Game1.getOnlineFarmers().Contains(farmerMaybeOffline2))
          return (Event) null;
        Game1.player.team.GetFriendship(farmerMaybeOffline1.UniqueMultiplayerID, spouse.Value).Status = FriendshipStatus.Married;
        Game1.player.team.GetFriendship(farmerMaybeOffline1.UniqueMultiplayerID, spouse.Value).WeddingDate = new WorldDate(Game1.Date);
        availableWeddingEvent = Utility.getPlayerWeddingEvent(farmerMaybeOffline1, farmerMaybeOffline2);
      }
      return availableWeddingEvent;
    }

    public static void checkForNewLevelPerks()
    {
      Dictionary<string, string> dictionary1 = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
      int level = Game1.player.Level;
      foreach (string key in dictionary1.Keys)
      {
        string[] strArray = dictionary1[key].Split('/')[3].Split(' ');
        if (strArray[0].Equals("l") && Convert.ToInt32(strArray[1]) <= level && !Game1.player.cookingRecipes.ContainsKey(key))
        {
          Game1.player.cookingRecipes.Add(key, 0);
          Game1.currentObjectDialogue.Enqueue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2666") + key));
          Game1.currentDialogueCharacterIndex = 1;
          Game1.dialogueUp = true;
          Game1.dialogueTyping = true;
        }
        else if (strArray[0].Equals("s"))
        {
          int int32 = Convert.ToInt32(strArray[2]);
          bool flag = false;
          string str = strArray[1];
          if (!(str == "Farming"))
          {
            if (!(str == "Fishing"))
            {
              if (!(str == "Mining"))
              {
                if (!(str == "Combat"))
                {
                  if (!(str == "Foraging"))
                  {
                    if (str == "Luck" && Game1.player.LuckLevel >= int32)
                      flag = true;
                  }
                  else if (Game1.player.ForagingLevel >= int32)
                    flag = true;
                }
                else if (Game1.player.CombatLevel >= int32)
                  flag = true;
              }
              else if (Game1.player.MiningLevel >= int32)
                flag = true;
            }
            else if (Game1.player.FishingLevel >= int32)
              flag = true;
          }
          else if (Game1.player.FarmingLevel >= int32)
            flag = true;
          if (flag && !Game1.player.cookingRecipes.ContainsKey(key))
          {
            Game1.player.cookingRecipes.Add(key, 0);
            Game1.currentObjectDialogue.Enqueue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2666") + key));
            Game1.currentDialogueCharacterIndex = 1;
            Game1.dialogueUp = true;
            Game1.dialogueTyping = true;
          }
        }
      }
      Dictionary<string, string> dictionary2 = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
      foreach (string key in dictionary2.Keys)
      {
        string[] strArray = dictionary2[key].Split('/')[4].Split(' ');
        if (strArray[0].Equals("l") && Convert.ToInt32(strArray[1]) <= level && !Game1.player.craftingRecipes.ContainsKey(key))
        {
          Game1.player.craftingRecipes.Add(key, 0);
          Game1.currentObjectDialogue.Enqueue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2677") + key));
          Game1.currentDialogueCharacterIndex = 1;
          Game1.dialogueUp = true;
          Game1.dialogueTyping = true;
        }
        else if (strArray[0].Equals("s"))
        {
          int int32 = Convert.ToInt32(strArray[2]);
          bool flag = false;
          string str = strArray[1];
          if (!(str == "Farming"))
          {
            if (!(str == "Fishing"))
            {
              if (!(str == "Mining"))
              {
                if (!(str == "Combat"))
                {
                  if (!(str == "Foraging"))
                  {
                    if (str == "Luck" && Game1.player.LuckLevel >= int32)
                      flag = true;
                  }
                  else if (Game1.player.ForagingLevel >= int32)
                    flag = true;
                }
                else if (Game1.player.CombatLevel >= int32)
                  flag = true;
              }
              else if (Game1.player.MiningLevel >= int32)
                flag = true;
            }
            else if (Game1.player.FishingLevel >= int32)
              flag = true;
          }
          else if (Game1.player.FarmingLevel >= int32)
            flag = true;
          if (flag && !Game1.player.craftingRecipes.ContainsKey(key))
          {
            Game1.player.craftingRecipes.Add(key, 0);
            Game1.currentObjectDialogue.Enqueue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2677") + key));
            Game1.currentDialogueCharacterIndex = 1;
            Game1.dialogueUp = true;
            Game1.dialogueTyping = true;
          }
        }
      }
    }

    public static void exitActiveMenu() => Game1.activeClickableMenu = (IClickableMenu) null;

    public static void fadeScreenToBlack() => Game1.screenFade.FadeScreenToBlack();

    public static void fadeClear() => Game1.screenFade.FadeClear();

    private bool onFadeToBlackComplete()
    {
      bool blackComplete = false;
      if (Game1.killScreen)
      {
        Game1.viewportFreeze = true;
        Game1.viewport.X = -10000;
      }
      if (Game1.exitToTitle)
      {
        Game1.menuUp = false;
        Game1.setGameMode((byte) 4);
        Game1.menuChoice = 0;
        Game1.fadeIn = false;
        Game1.fadeToBlack = true;
        Game1.fadeToBlackAlpha = 0.01f;
        Game1.exitToTitle = false;
        Game1.changeMusicTrack("none");
        Game1.debrisWeather.Clear();
        return true;
      }
      if (Game1.timeOfDayAfterFade != -1)
      {
        Game1.timeOfDay = Game1.timeOfDayAfterFade;
        Game1.timeOfDayAfterFade = -1;
      }
      if (!Game1.nonWarpFade && Game1.locationRequest != null && !Game1.menuUp)
      {
        GameLocation currentLocation = Game1.currentLocation;
        if (Game1.emoteMenu != null)
          Game1.emoteMenu.exitThisMenuNoSound();
        if (Game1.client != null && Game1.currentLocation != null)
          Game1.currentLocation.StoreCachedMultiplayerMap(Game1.multiplayer.cachedMultiplayerMaps);
        Game1.currentLocation.cleanupBeforePlayerExit();
        Game1.multiplayer.broadcastLocationDelta(Game1.currentLocation);
        bool flag = false;
        Game1.displayFarmer = true;
        if (Game1.eventOver)
        {
          Game1.eventFinished();
          if (Game1.dayOfMonth == 0)
            Game1.newDayAfterFade((Action) (() => Game1.player.Position = new Vector2(320f, 320f)));
          return true;
        }
        if (Game1.locationRequest.IsRequestFor(Game1.currentLocation) && Game1.player.previousLocationName != "" && !Game1.eventUp && !Game1.currentLocation.Name.StartsWith("UndergroundMine"))
        {
          Game1.player.Position = new Vector2((float) (Game1.xLocationAfterWarp * 64), (float) (Game1.yLocationAfterWarp * 64 - (Game1.player.Sprite.getHeight() - 32) + 16));
          Game1.viewportFreeze = false;
          Game1.currentLocation.resetForPlayerEntry();
          flag = true;
        }
        else
        {
          if (Game1.locationRequest.Name.StartsWith("UndergroundMine"))
          {
            if (!Game1.currentLocation.Name.StartsWith("UndergroundMine"))
              Game1.changeMusicTrack("none");
            MineShaft location = Game1.locationRequest.Location as MineShaft;
            if (Game1.player.IsSitting())
              Game1.player.StopSitting(false);
            Game1.player.Halt();
            Game1.player.forceCanMove();
            if (!Game1.IsClient || Game1.locationRequest.Location != null && (NetFieldBase<GameLocation, NetRef<GameLocation>>) Game1.locationRequest.Location.Root != (NetRef<GameLocation>) null)
            {
              location.resetForPlayerEntry();
              flag = true;
            }
            Game1.currentLocation = (GameLocation) location;
            Game1.currentLocation.Map.LoadTileSheets(Game1.mapDisplayDevice);
            Game1.checkForRunButton(Game1.GetKeyboardState());
          }
          if (!Game1.eventUp && !Game1.menuUp)
            Game1.player.Position = new Vector2((float) (Game1.xLocationAfterWarp * 64), (float) (Game1.yLocationAfterWarp * 64 - (Game1.player.Sprite.getHeight() - 32) + 16));
          if (!Game1.locationRequest.Name.StartsWith("UndergroundMine"))
          {
            Game1.currentLocation = Game1.locationRequest.Location;
            if (!Game1.IsClient)
            {
              Game1.locationRequest.Loaded(Game1.locationRequest.Location);
              Game1.currentLocation.resetForPlayerEntry();
              flag = true;
            }
            Game1.currentLocation.Map.LoadTileSheets(Game1.mapDisplayDevice);
            if (!Game1.viewportFreeze && Game1.currentLocation.Map.DisplayWidth <= Game1.viewport.Width)
              Game1.viewport.X = (Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width) / 2;
            if (!Game1.viewportFreeze && Game1.currentLocation.Map.DisplayHeight <= Game1.viewport.Height)
              Game1.viewport.Y = (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2;
            Game1.checkForRunButton(Game1.GetKeyboardState(), true);
          }
          if (!Game1.eventUp)
            Game1.viewportFreeze = false;
        }
        Game1.forceSnapOnNextViewportUpdate = true;
        Game1.player.FarmerSprite.PauseForSingleAnimation = false;
        Game1.player.faceDirection(Game1.facingDirectionAfterWarp);
        Game1._isWarping = false;
        if (Game1.player.ActiveObject != null)
          Game1.player.showCarrying();
        else
          Game1.player.showNotCarrying();
        if (Game1.IsClient)
        {
          if (Game1.locationRequest.Location != null && (NetFieldBase<GameLocation, NetRef<GameLocation>>) Game1.locationRequest.Location.Root != (NetRef<GameLocation>) null && Game1.multiplayer.isActiveLocation(Game1.locationRequest.Location))
          {
            Game1.currentLocation = Game1.locationRequest.Location;
            Game1.locationRequest.Loaded(Game1.locationRequest.Location);
            if (!flag)
              Game1.currentLocation.resetForPlayerEntry();
            Game1.player.currentLocation = Game1.currentLocation;
            Game1.locationRequest.Warped(Game1.currentLocation);
            Game1.currentLocation.updateSeasonalTileSheets();
            if (Game1.IsDebrisWeatherHere())
              Game1.populateDebrisWeatherArray();
            Game1.warpingForForcedRemoteEvent = false;
            Game1.locationRequest = (LocationRequest) null;
          }
          else
          {
            Game1.requestLocationInfoFromServer();
            if (Game1.currentLocation == null)
              return true;
          }
        }
        else
        {
          Game1.player.currentLocation = Game1.locationRequest.Location;
          Game1.locationRequest.Warped(Game1.locationRequest.Location);
          Game1.locationRequest = (LocationRequest) null;
        }
        if (Game1.locationRequest == null && Game1.currentLocation.Name == "Farm" && !Game1.eventUp)
        {
          if ((double) Game1.player.position.X / 64.0 >= (double) (Game1.currentLocation.map.Layers[0].LayerWidth - 1))
            Game1.player.position.X -= 64f;
          else if ((double) Game1.player.position.Y / 64.0 >= (double) (Game1.currentLocation.map.Layers[0].LayerHeight - 1))
            Game1.player.position.Y -= 32f;
          if ((double) Game1.player.position.Y / 64.0 >= (double) (Game1.currentLocation.map.Layers[0].LayerHeight - 2))
            Game1.player.position.X -= 48f;
        }
        if (currentLocation != null && currentLocation.Name.StartsWith("UndergroundMine") && Game1.currentLocation != null && !Game1.currentLocation.Name.StartsWith("UndergroundMine"))
          MineShaft.OnLeftMines();
        blackComplete = true;
      }
      if (Game1.newDay)
      {
        Game1.newDayAfterFade((Action) (() =>
        {
          if (Game1.eventOver)
          {
            Game1.eventFinished();
            if (Game1.dayOfMonth == 0)
              Game1.newDayAfterFade((Action) (() => Game1.player.Position = new Vector2(320f, 320f)));
          }
          Game1.nonWarpFade = false;
          Game1.fadeIn = false;
        }));
        return true;
      }
      if (Game1.eventOver)
      {
        Game1.eventFinished();
        if (Game1.dayOfMonth == 0)
          Game1.newDayAfterFade((Action) (() =>
          {
            Game1.currentLocation.resetForPlayerEntry();
            Game1.nonWarpFade = false;
            Game1.fadeIn = false;
          }));
        return true;
      }
      if (Game1.boardingBus)
      {
        Game1.boardingBus = false;
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2694") + (Game1.currentLocation.Name.Equals("Desert") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2696") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2697")));
        Game1.messagePause = true;
        Game1.viewportFreeze = false;
      }
      if (Game1.IsRainingHere() && Game1.currentSong != null && Game1.currentSong != null && Game1.currentSong.Name.Equals("rain"))
      {
        if (Game1.currentLocation.IsOutdoors)
          Game1.currentSong.SetVariable("Frequency", 100f);
        else if (!Game1.currentLocation.Name.StartsWith("UndergroundMine"))
          Game1.currentSong.SetVariable("Frequency", 15f);
      }
      return blackComplete;
    }

    private static void onFadedBackInComplete()
    {
      if (Game1.killScreen)
        Game1.pauseThenMessage(1500, "..." + Game1.player.Name + "?", false);
      else if (!Game1.eventUp)
        Game1.player.CanMove = true;
      Game1.checkForRunButton(Game1.oldKBState, true);
    }

    public static void UpdateOther(GameTime time)
    {
      if (Game1.currentLocation == null || !Game1.player.passedOut && Game1.screenFade.UpdateFade(time))
        return;
      if (Game1.dialogueUp || Game1.currentBillboard != 0)
        Game1.player.CanMove = false;
      for (int index = Game1.delayedActions.Count - 1; index >= 0; --index)
      {
        DelayedAction delayedAction = Game1.delayedActions[index];
        if (delayedAction.update(time) && Game1.delayedActions.Contains(delayedAction))
          Game1.delayedActions.Remove(delayedAction);
      }
      if (Game1.timeOfDay >= 2600 || (double) Game1.player.stamina <= -15.0)
      {
        if (Game1.currentMinigame != null && Game1.currentMinigame.forceQuit())
          Game1.currentMinigame = (IMinigame) null;
        if (Game1.currentMinigame == null && Game1.player.canMove && Game1.player.freezePause <= 0 && !Game1.player.UsingTool && !Game1.eventUp && (Game1.IsMasterGame || (bool) (NetFieldBase<bool, NetBool>) Game1.player.isCustomized) && Game1.locationRequest == null && Game1.activeClickableMenu == null)
        {
          Game1.player.startToPassOut();
          Game1.player.freezePause = 7000;
        }
      }
      for (int index = Game1.screenOverlayTempSprites.Count - 1; index >= 0; --index)
      {
        if (Game1.screenOverlayTempSprites[index].update(time))
          Game1.screenOverlayTempSprites.RemoveAt(index);
      }
      for (int index = Game1.uiOverlayTempSprites.Count - 1; index >= 0; --index)
      {
        if (Game1.uiOverlayTempSprites[index].update(time))
          Game1.uiOverlayTempSprites.RemoveAt(index);
      }
      if (Game1.pickingTool)
      {
        Game1.pickToolInterval += (float) time.ElapsedGameTime.Milliseconds;
        if ((double) Game1.pickToolInterval > 500.0)
        {
          Game1.pickingTool = false;
          Game1.pickToolInterval = 0.0f;
          if (!Game1.eventUp)
            Game1.player.CanMove = true;
          Game1.player.UsingTool = false;
          switch (Game1.player.FacingDirection)
          {
            case 0:
              Game1.player.Sprite.currentFrame = 16;
              break;
            case 1:
              Game1.player.Sprite.currentFrame = 8;
              break;
            case 2:
              Game1.player.Sprite.currentFrame = 0;
              break;
            case 3:
              Game1.player.Sprite.currentFrame = 24;
              break;
          }
          if (!Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift))
            Game1.player.setRunning(Game1.options.autoRun);
        }
        else if ((double) Game1.pickToolInterval > 83.3333358764648)
        {
          switch (Game1.player.FacingDirection)
          {
            case 0:
              Game1.player.FarmerSprite.setCurrentFrame(196);
              break;
            case 1:
              Game1.player.FarmerSprite.setCurrentFrame(194);
              break;
            case 2:
              Game1.player.FarmerSprite.setCurrentFrame(192);
              break;
            case 3:
              Game1.player.FarmerSprite.setCurrentFrame(198);
              break;
          }
        }
      }
      if ((Game1.player.CanMove || Game1.player.UsingTool) && Game1.shouldTimePass())
        Game1.buffsDisplay.update(time);
      if (Game1.player.CurrentItem != null)
        Game1.player.CurrentItem.actionWhenBeingHeld(Game1.player);
      float dialogueButtonScale = Game1.dialogueButtonScale;
      Game1.dialogueButtonScale = (float) (16.0 * Math.Sin(time.TotalGameTime.TotalMilliseconds % 1570.0 / 500.0));
      if ((double) dialogueButtonScale > (double) Game1.dialogueButtonScale && !Game1.dialogueButtonShrinking)
        Game1.dialogueButtonShrinking = true;
      else if ((double) dialogueButtonScale < (double) Game1.dialogueButtonScale && Game1.dialogueButtonShrinking)
        Game1.dialogueButtonShrinking = false;
      TimeSpan elapsedGameTime;
      if (Game1.player.currentUpgrade != null && Game1.currentLocation.Name.Equals("Farm") && Game1.player.currentUpgrade.daysLeftTillUpgradeDone <= 3)
      {
        BuildingUpgrade currentUpgrade = Game1.player.currentUpgrade;
        elapsedGameTime = time.ElapsedGameTime;
        double milliseconds = (double) elapsedGameTime.Milliseconds;
        currentUpgrade.update((float) milliseconds);
      }
      if (Game1.screenGlow)
      {
        if (Game1.screenGlowUp || Game1.screenGlowHold)
        {
          if (Game1.screenGlowHold)
          {
            Game1.screenGlowAlpha = Math.Min(Game1.screenGlowAlpha + Game1.screenGlowRate, Game1.screenGlowMax);
          }
          else
          {
            Game1.screenGlowAlpha = Math.Min(Game1.screenGlowAlpha + 0.03f, 0.6f);
            if ((double) Game1.screenGlowAlpha >= 0.600000023841858)
              Game1.screenGlowUp = false;
          }
        }
        else
        {
          Game1.screenGlowAlpha -= 0.01f;
          if ((double) Game1.screenGlowAlpha <= 0.0)
            Game1.screenGlow = false;
        }
      }
      for (int index = Game1.hudMessages.Count - 1; index >= 0; --index)
      {
        if (Game1.hudMessages.ElementAt<HUDMessage>(index).update(time))
          Game1.hudMessages.RemoveAt(index);
      }
      Game1.updateWeather(time);
      if (!Game1.fadeToBlack)
        Game1.currentLocation.checkForMusic(time);
      if ((double) Game1.debrisSoundInterval > 0.0)
      {
        double debrisSoundInterval = (double) Game1.debrisSoundInterval;
        elapsedGameTime = time.ElapsedGameTime;
        double milliseconds = (double) elapsedGameTime.Milliseconds;
        Game1.debrisSoundInterval = (float) (debrisSoundInterval - milliseconds);
      }
      double noteBlockTimer = (double) Game1.noteBlockTimer;
      elapsedGameTime = time.ElapsedGameTime;
      double milliseconds1 = (double) elapsedGameTime.Milliseconds;
      Game1.noteBlockTimer = (float) (noteBlockTimer + milliseconds1);
      if ((double) Game1.noteBlockTimer > 1000.0)
      {
        Game1.noteBlockTimer = 0.0f;
        if (Game1.player.health < 20 && Game1.CurrentEvent == null)
        {
          Game1.hitShakeTimer = 250;
          if (Game1.player.health <= 10)
          {
            Game1.hitShakeTimer = 500;
            if (Game1.showingHealthBar && (double) Game1.fadeToBlackAlpha <= 0.0)
            {
              for (int index = 0; index < 3; ++index)
                Game1.uiOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(366, 412, 5, 6), new Vector2((float) (Game1.random.Next(32) + Game1.uiViewport.Width - 112), (float) (Game1.uiViewport.Height - 224 - (Game1.player.maxHealth - 100) - 16 + 4)), false, 0.017f, Microsoft.Xna.Framework.Color.Red)
                {
                  motion = new Vector2(-1.5f, (float) (Game1.random.Next(-1, 2) - 8)),
                  acceleration = new Vector2(0.0f, 0.5f),
                  local = true,
                  scale = 4f,
                  delayBeforeAnimationStart = index * 150
                });
            }
          }
        }
      }
      if (Game1.showKeyHelp && !Game1.eventUp)
      {
        Game1.keyHelpString = "";
        if (Game1.dialogueUp)
          Game1.keyHelpString += Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2716");
        else if (Game1.menuUp)
        {
          Game1.keyHelpString += Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2719");
          Game1.keyHelpString += Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2720");
        }
        else if (Game1.player.ActiveObject != null)
        {
          Game1.keyHelpString += Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2727");
          Game1.keyHelpString = Game1.keyHelpString + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2728");
          if (Game1.player.numberOfItemsInInventory() < (int) (NetFieldBase<int, NetInt>) Game1.player.maxItems)
            Game1.keyHelpString = Game1.keyHelpString + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2729");
          if (Game1.player.numberOfItemsInInventory() > 0)
            Game1.keyHelpString = Game1.keyHelpString + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2730");
          Game1.keyHelpString = Game1.keyHelpString + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2731");
          Game1.keyHelpString = Game1.keyHelpString + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2732");
        }
        else
        {
          Game1.keyHelpString += Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2733");
          if (Game1.player.CurrentTool != null)
            Game1.keyHelpString = Game1.keyHelpString + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2734", (object) Game1.player.CurrentTool.DisplayName);
          if (Game1.player.numberOfItemsInInventory() > 0)
            Game1.keyHelpString = Game1.keyHelpString + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2735");
          Game1.keyHelpString = Game1.keyHelpString + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2731");
          Game1.keyHelpString = Game1.keyHelpString + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2732");
        }
      }
      Game1.drawLighting = Game1.currentLocation.IsOutdoors && !Game1.outdoorLight.Equals(Microsoft.Xna.Framework.Color.White) || !Game1.ambientLight.Equals(Microsoft.Xna.Framework.Color.White) || Game1.currentLocation is MineShaft && !((MineShaft) Game1.currentLocation).getLightingColor(time).Equals(Microsoft.Xna.Framework.Color.White);
      if (Game1.player.hasBuff(26))
        Game1.drawLighting = true;
      if (Game1.hitShakeTimer > 0)
      {
        int hitShakeTimer = Game1.hitShakeTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds2 = elapsedGameTime.Milliseconds;
        Game1.hitShakeTimer = hitShakeTimer - milliseconds2;
      }
      if (Game1.staminaShakeTimer > 0)
      {
        int staminaShakeTimer = Game1.staminaShakeTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds3 = elapsedGameTime.Milliseconds;
        Game1.staminaShakeTimer = staminaShakeTimer - milliseconds3;
      }
      if (Game1.background != null)
        Game1.background.update(Game1.viewport);
      int tileHintCheckTimer = Game1.cursorTileHintCheckTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
      Game1.cursorTileHintCheckTimer = tileHintCheckTimer - totalMilliseconds;
      Game1.currentCursorTile.X = (float) ((Game1.viewport.X + Game1.getOldMouseX()) / 64);
      Game1.currentCursorTile.Y = (float) ((Game1.viewport.Y + Game1.getOldMouseY()) / 64);
      if (Game1.cursorTileHintCheckTimer <= 0 || !Game1.currentCursorTile.Equals(Game1.lastCursorTile))
      {
        Game1.cursorTileHintCheckTimer = 250;
        Game1.updateCursorTileHint();
        if (Game1.player.CanMove)
          Game1.checkForRunButton(Game1.oldKBState, true);
      }
      if (!Game1.currentLocation.Name.StartsWith("UndergroundMine"))
        MineShaft.timeSinceLastMusic = 200000;
      if (Game1.activeClickableMenu != null || Game1.farmEvent != null || Game1.keyboardDispatcher == null || Game1.IsChatting)
        return;
      Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) null;
    }

    public static void updateWeather(GameTime time)
    {
      if (Game1.IsSnowingHere() && (bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.isOutdoors && !(Game1.currentLocation is Desert))
      {
        Vector2 current = new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
        Game1.snowPos = Game1.updateFloatingObjectPositionForMovement(Game1.snowPos, current, Game1.previousViewportPosition, -1f);
      }
      TimeSpan elapsedGameTime;
      if (Game1.IsRainingHere() && Game1.currentLocation.IsOutdoors)
      {
        for (int index = 0; index < Game1.rainDrops.Length; ++index)
        {
          if (Game1.rainDrops[index].frame == 0)
          {
            ref int local = ref Game1.rainDrops[index].accumulator;
            int num = local;
            elapsedGameTime = time.ElapsedGameTime;
            int milliseconds = elapsedGameTime.Milliseconds;
            local = num + milliseconds;
            if (Game1.rainDrops[index].accumulator >= 70)
            {
              Game1.rainDrops[index].position += new Vector2((float) (index * 8 / Game1.rainDrops.Length - 16), (float) (32 - index * 8 / Game1.rainDrops.Length));
              Game1.rainDrops[index].accumulator = 0;
              if (Game1.random.NextDouble() < 0.1)
                ++Game1.rainDrops[index].frame;
              if (Game1.currentLocation is IslandNorth || Game1.currentLocation is Caldera)
              {
                Point p = new Point((int) ((double) Game1.rainDrops[index].position.X + (double) Game1.viewport.X) / 64, (int) ((double) Game1.rainDrops[index].position.Y + (double) Game1.viewport.Y) / 64);
                --p.Y;
                if (Game1.currentLocation.isTileOnMap(p.X, p.Y) && Game1.currentLocation.getTileIndexAt(p, "Back") == -1 && Game1.currentLocation.getTileIndexAt(p, "Buildings") == -1)
                  Game1.rainDrops[index].frame = 0;
              }
              if ((double) Game1.rainDrops[index].position.Y > (double) (Game1.viewport.Height + 64))
                Game1.rainDrops[index].position.Y = -64f;
            }
          }
          else
          {
            ref int local = ref Game1.rainDrops[index].accumulator;
            int num = local;
            elapsedGameTime = time.ElapsedGameTime;
            int milliseconds = elapsedGameTime.Milliseconds;
            local = num + milliseconds;
            if (Game1.rainDrops[index].accumulator > 70)
            {
              Game1.rainDrops[index].frame = (Game1.rainDrops[index].frame + 1) % 4;
              Game1.rainDrops[index].accumulator = 0;
              if (Game1.rainDrops[index].frame == 0)
                Game1.rainDrops[index].position = new Vector2((float) Game1.random.Next(Game1.viewport.Width), (float) Game1.random.Next(Game1.viewport.Height));
            }
          }
        }
      }
      else if (Game1.IsDebrisWeatherHere() && Game1.currentLocation.IsOutdoors && !(bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.ignoreDebrisWeather)
      {
        if (Game1.currentSeason.Equals("fall") && Game1.random.NextDouble() < 0.001 && (double) Game1.windGust == 0.0 && (double) WeatherDebris.globalWind >= -0.5)
        {
          Game1.windGust += (float) Game1.random.Next(-10, -1) / 100f;
          if (Game1.soundBank != null)
          {
            Game1.wind = Game1.soundBank.GetCue("wind");
            Game1.wind.Play();
          }
        }
        else if ((double) Game1.windGust != 0.0)
        {
          Game1.windGust = Math.Max(-5f, Game1.windGust * 1.02f);
          WeatherDebris.globalWind = Game1.windGust - 0.5f;
          if ((double) Game1.windGust < -0.200000002980232 && Game1.random.NextDouble() < 0.007)
            Game1.windGust = 0.0f;
        }
        foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
          weatherDebris.update();
      }
      if ((double) WeatherDebris.globalWind >= -0.5 || Game1.wind == null)
        return;
      WeatherDebris.globalWind = Math.Min(-0.5f, WeatherDebris.globalWind + 0.015f);
      Game1.wind.SetVariable("Volume", (float) (-(double) WeatherDebris.globalWind * 20.0));
      Game1.wind.SetVariable("Frequency", (float) (-(double) WeatherDebris.globalWind * 20.0));
      if ((double) WeatherDebris.globalWind != -0.5)
        return;
      Game1.wind.Stop(AudioStopOptions.AsAuthored);
    }

    public static void updateCursorTileHint()
    {
      if (Game1.activeClickableMenu != null)
        return;
      Game1.mouseCursorTransparency = 1f;
      Game1.isActionAtCurrentCursorTile = false;
      Game1.isInspectionAtCurrentCursorTile = false;
      Game1.isSpeechAtCurrentCursorTile = false;
      int xTile = (Game1.viewport.X + Game1.getOldMouseX()) / 64;
      int yTile = (Game1.viewport.Y + Game1.getOldMouseY()) / 64;
      if (Game1.currentLocation != null)
      {
        Game1.isActionAtCurrentCursorTile = Game1.currentLocation.isActionableTile(xTile, yTile, Game1.player);
        if (!Game1.isActionAtCurrentCursorTile)
          Game1.isActionAtCurrentCursorTile = Game1.currentLocation.isActionableTile(xTile, yTile + 1, Game1.player);
      }
      Game1.lastCursorTile = Game1.currentCursorTile;
    }

    public static void updateMusic()
    {
      if (Game1.soundBank == null)
        return;
      if (Game1.game1.IsMainInstance)
      {
        Game1 game1 = (Game1) null;
        string newTrackName = (string) null;
        int num1 = 1;
        int num2 = 2;
        int num3 = 5;
        int num4 = 6;
        int num5 = 7;
        int num6 = 0;
        float num7 = (float) Game1.GetDefaultSongPriority(Game1.getMusicTrackName(), Game1.game1.instanceIsOverridingTrack);
        Game1.MusicContext musicContext = Game1.MusicContext.Default;
        foreach (Game1 gameInstance in GameRunner.instance.gameInstances)
        {
          Game1.MusicContext activeMusicContext = gameInstance._instanceActiveMusicContext;
          if (gameInstance.IsMainInstance)
            musicContext = activeMusicContext;
          string song_name = (string) null;
          string str = (string) null;
          if (gameInstance._instanceRequestedMusicTracks.ContainsKey(activeMusicContext))
            song_name = gameInstance._instanceRequestedMusicTracks[activeMusicContext].Key;
          if (gameInstance.instanceIsOverridingTrack && gameInstance.instanceCurrentSong != null)
            str = gameInstance.instanceCurrentSong.Name;
          if (activeMusicContext == Game1.MusicContext.Event && num6 < num4)
          {
            if (song_name != null)
            {
              num6 = num4;
              game1 = gameInstance;
              newTrackName = song_name;
            }
          }
          else if (activeMusicContext == Game1.MusicContext.MiniGame && num6 < num3)
          {
            if (song_name != null)
            {
              num6 = num3;
              game1 = gameInstance;
              newTrackName = song_name;
            }
          }
          else if (activeMusicContext == Game1.MusicContext.SubLocation && num6 < num1)
          {
            if (song_name != null)
            {
              num6 = num1;
              game1 = gameInstance;
              newTrackName = str == null ? song_name : str;
            }
          }
          else if (song_name == "mermaidSong")
          {
            num6 = num5;
            game1 = gameInstance;
            newTrackName = song_name;
          }
          if (activeMusicContext == Game1.MusicContext.Default && musicContext <= activeMusicContext && song_name != null)
          {
            float defaultSongPriority = (float) Game1.GetDefaultSongPriority(song_name, gameInstance.instanceIsOverridingTrack);
            if ((double) num7 < (double) defaultSongPriority)
            {
              num7 = defaultSongPriority;
              num6 = num2;
              game1 = gameInstance;
              newTrackName = str == null ? song_name : str;
            }
          }
        }
        if (game1 == null || game1 == Game1.game1)
        {
          if (Game1.doesMusicContextHaveTrack(Game1.MusicContext.ImportantSplitScreenMusic))
            Game1.stopMusicTrack(Game1.MusicContext.ImportantSplitScreenMusic);
        }
        else if (newTrackName == null && Game1.doesMusicContextHaveTrack(Game1.MusicContext.ImportantSplitScreenMusic))
          Game1.stopMusicTrack(Game1.MusicContext.ImportantSplitScreenMusic);
        else if (newTrackName != null && Game1.getMusicTrackName(Game1.MusicContext.ImportantSplitScreenMusic) != newTrackName)
          Game1.changeMusicTrack(newTrackName, music_context: Game1.MusicContext.ImportantSplitScreenMusic);
      }
      string name = "";
      bool flag1 = false;
      bool flag2 = false;
      if (Game1.currentLocation != null && Game1.currentLocation.IsMiniJukeboxPlaying() && (!Game1.requestedMusicDirty || Game1.requestedMusicTrackOverrideable) && Game1.currentTrackOverrideable)
      {
        name = "";
        flag2 = true;
        string str = Game1.currentLocation.miniJukeboxTrack.Value;
        if (str == "random")
          str = Game1.currentLocation.randomMiniJukeboxTrack.Value != null ? Game1.currentLocation.randomMiniJukeboxTrack.Value : "";
        if (Game1.currentSong == null || !Game1.currentSong.IsPlaying || Game1.currentSong.Name != str)
        {
          name = str;
          Game1.requestedMusicDirty = false;
          flag1 = true;
        }
      }
      if (Game1.isOverridingTrack != flag2)
      {
        Game1.isOverridingTrack = flag2;
        if (!Game1.isOverridingTrack)
          Game1.requestedMusicDirty = true;
      }
      if (Game1.requestedMusicDirty)
      {
        name = Game1.requestedMusicTrack;
        flag1 = Game1.requestedMusicTrackOverrideable;
      }
      if (!name.Equals(""))
      {
        Game1.musicPlayerVolume = Math.Max(0.0f, Math.Min(Game1.options.musicVolumeLevel, Game1.musicPlayerVolume - 0.01f));
        Game1.ambientPlayerVolume = Math.Max(0.0f, Math.Min(Game1.options.musicVolumeLevel, Game1.ambientPlayerVolume - 0.01f));
        if (Game1.game1.IsMainInstance)
        {
          Game1.musicCategory.SetVolume(Game1.musicPlayerVolume);
          Game1.ambientCategory.SetVolume(Game1.ambientPlayerVolume);
        }
        if ((double) Game1.musicPlayerVolume != 0.0 || (double) Game1.ambientPlayerVolume != 0.0 || Game1.currentSong == null)
          return;
        if (name.Equals("none"))
        {
          Game1.jukeboxPlaying = false;
          Game1.currentSong.Stop(AudioStopOptions.Immediate);
        }
        else if (((double) Game1.options.musicVolumeLevel != 0.0 || (double) Game1.options.ambientVolumeLevel != 0.0) && (!name.Equals("rain") || Game1.endOfNightMenus.Count == 0))
        {
          if (Game1.game1.IsMainInstance)
          {
            Game1.currentSong.Stop(AudioStopOptions.Immediate);
            Game1.currentSong.Dispose();
          }
          Game1.currentSong = Game1.soundBank.GetCue(name);
          if (Game1.game1.IsMainInstance)
            Game1.currentSong.Play();
          if (Game1.game1.IsMainInstance && Game1.currentSong != null && Game1.currentSong.Name.Equals("rain") && Game1.currentLocation != null)
          {
            if (Game1.IsRainingHere())
            {
              if (Game1.currentLocation.IsOutdoors)
                Game1.currentSong.SetVariable("Frequency", 100f);
              else if (!Game1.currentLocation.Name.StartsWith("UndergroundMine"))
                Game1.currentSong.SetVariable("Frequency", 15f);
            }
            else if (Game1.eventUp)
              Game1.currentSong.SetVariable("Frequency", 100f);
          }
        }
        else
          Game1.currentSong.Stop(AudioStopOptions.Immediate);
        Game1.currentTrackOverrideable = flag1;
        Game1.requestedMusicDirty = false;
      }
      else if ((double) Game1.musicPlayerVolume < (double) Game1.options.musicVolumeLevel || (double) Game1.ambientPlayerVolume < (double) Game1.options.ambientVolumeLevel)
      {
        if ((double) Game1.musicPlayerVolume < (double) Game1.options.musicVolumeLevel)
        {
          Game1.musicPlayerVolume = Math.Min(1f, Game1.musicPlayerVolume += 0.01f);
          if (Game1.game1.IsMainInstance)
            Game1.musicCategory.SetVolume(Game1.options.musicVolumeLevel);
        }
        if ((double) Game1.ambientPlayerVolume >= (double) Game1.options.ambientVolumeLevel)
          return;
        Game1.ambientPlayerVolume = Math.Min(1f, Game1.ambientPlayerVolume += 0.015f);
        if (!Game1.game1.IsMainInstance)
          return;
        Game1.ambientCategory.SetVolume(Game1.ambientPlayerVolume);
      }
      else
      {
        if (Game1.currentSong == null || Game1.currentSong.IsPlaying || Game1.currentSong.IsStopped)
          return;
        Game1.currentSong = Game1.soundBank.GetCue(Game1.currentSong.Name);
        if (!Game1.game1.IsMainInstance)
          return;
        Game1.currentSong.Play();
      }
    }

    public static int GetDefaultSongPriority(string song_name, bool is_playing_override)
    {
      if (is_playing_override)
        return 9;
      if (song_name.Equals("none"))
        return 0;
      if (song_name.EndsWith("_day_ambient") || song_name.EndsWith("_night_ambient") || song_name.Equals("rain"))
        return 1;
      if (song_name.StartsWith(Game1.currentSeason))
        return 2;
      if (song_name.Contains("town"))
        return 3;
      if (song_name.Equals("jungle_ambience") || song_name.Contains("Ambient"))
        return 7;
      if (song_name.Equals("IslandMusic"))
        return 8;
      return song_name.EndsWith("Mine") ? 20 : 10;
    }

    public static void updateRainDropPositionForPlayerMovement(int direction) => Game1.updateRainDropPositionForPlayerMovement(direction, false);

    public static void updateRainDropPositionForPlayerMovement(
      int direction,
      bool overrideConstraints)
    {
      Game1.updateRainDropPositionForPlayerMovement(direction, overrideConstraints, (float) Game1.player.speed);
    }

    public static void updateRainDropPositionForPlayerMovement(
      int direction,
      bool overrideConstraints,
      float speed)
    {
      if (!overrideConstraints && (!Game1.IsRainingHere() && !Game1.IsDebrisWeatherHere() || !Game1.currentLocation.IsOutdoors || direction != 0 && direction != 2 && (Game1.player.getStandingX() < Game1.viewport.Width / 2 || Game1.player.getStandingX() > Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width / 2) || direction != 1 && direction != 3 && (Game1.player.getStandingY() < Game1.viewport.Height / 2 || Game1.player.getStandingY() > Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height / 2)))
        return;
      if (Game1.IsRainingHere())
      {
        for (int index = 0; index < Game1.rainDrops.Length; ++index)
        {
          switch (direction)
          {
            case 0:
              Game1.rainDrops[index].position.Y += speed;
              if ((double) Game1.rainDrops[index].position.Y > (double) (Game1.viewport.Height + 64))
              {
                Game1.rainDrops[index].position.Y = -64f;
                break;
              }
              break;
            case 1:
              Game1.rainDrops[index].position.X -= speed;
              if ((double) Game1.rainDrops[index].position.X < -64.0)
              {
                Game1.rainDrops[index].position.X = (float) Game1.viewport.Width;
                break;
              }
              break;
            case 2:
              Game1.rainDrops[index].position.Y -= speed;
              if ((double) Game1.rainDrops[index].position.Y < -64.0)
              {
                Game1.rainDrops[index].position.Y = (float) Game1.viewport.Height;
                break;
              }
              break;
            case 3:
              Game1.rainDrops[index].position.X += speed;
              if ((double) Game1.rainDrops[index].position.X > (double) (Game1.viewport.Width + 64))
              {
                Game1.rainDrops[index].position.X = -64f;
                break;
              }
              break;
          }
        }
      }
      else
        Game1.updateDebrisWeatherForMovement(Game1.debrisWeather, direction, overrideConstraints, speed);
    }

    public static void initializeVolumeLevels()
    {
      if (LocalMultiplayer.IsLocalMultiplayer() && !Game1.game1.IsMainInstance)
        return;
      Game1.soundCategory.SetVolume(Game1.options.soundVolumeLevel);
      Game1.musicCategory.SetVolume(Game1.options.musicVolumeLevel);
      Game1.ambientCategory.SetVolume(Game1.options.ambientVolumeLevel);
      Game1.footstepCategory.SetVolume(Game1.options.footstepVolumeLevel);
    }

    public static void updateDebrisWeatherForMovement(
      List<WeatherDebris> debris,
      int direction,
      bool overrideConstraints,
      float speed)
    {
      if ((double) Game1.fadeToBlackAlpha > 0.0 || debris == null)
        return;
      foreach (WeatherDebris debri in debris)
      {
        switch (direction)
        {
          case 0:
            debri.position.Y += speed;
            if ((double) debri.position.Y > (double) (Game1.viewport.Height + 64))
            {
              debri.position.Y = -64f;
              continue;
            }
            continue;
          case 1:
            debri.position.X -= speed;
            if ((double) debri.position.X < -64.0)
            {
              debri.position.X = (float) Game1.viewport.Width;
              continue;
            }
            continue;
          case 2:
            debri.position.Y -= speed;
            if ((double) debri.position.Y < -64.0)
            {
              debri.position.Y = (float) Game1.viewport.Height;
              continue;
            }
            continue;
          case 3:
            debri.position.X += speed;
            if ((double) debri.position.X > (double) (Game1.viewport.Width + 64))
            {
              debri.position.X = -64f;
              continue;
            }
            continue;
          default:
            continue;
        }
      }
    }

    public static Vector2 updateFloatingObjectPositionForMovement(
      Vector2 w,
      Vector2 current,
      Vector2 previous,
      float speed)
    {
      if ((double) current.Y < (double) previous.Y)
        w.Y -= Math.Abs(current.Y - previous.Y) * speed;
      else if ((double) current.Y > (double) previous.Y)
        w.Y += Math.Abs(current.Y - previous.Y) * speed;
      if ((double) current.X > (double) previous.X)
        w.X += Math.Abs(current.X - previous.X) * speed;
      else if ((double) current.X < (double) previous.X)
        w.X -= Math.Abs(current.X - previous.X) * speed;
      return w;
    }

    public static void updateRaindropPosition()
    {
      if (Game1.IsRainingHere())
      {
        int num1 = Game1.viewport.X - (int) Game1.previousViewportPosition.X;
        int num2 = Game1.viewport.Y - (int) Game1.previousViewportPosition.Y;
        for (int index = 0; index < Game1.rainDrops.Length; ++index)
        {
          Game1.rainDrops[index].position.X -= (float) num1 * 1f;
          Game1.rainDrops[index].position.Y -= (float) num2 * 1f;
          if ((double) Game1.rainDrops[index].position.Y > (double) (Game1.viewport.Height + 64))
            Game1.rainDrops[index].position.Y = -64f;
          else if ((double) Game1.rainDrops[index].position.X < -64.0)
            Game1.rainDrops[index].position.X = (float) Game1.viewport.Width;
          else if ((double) Game1.rainDrops[index].position.Y < -64.0)
            Game1.rainDrops[index].position.Y = (float) Game1.viewport.Height;
          else if ((double) Game1.rainDrops[index].position.X > (double) (Game1.viewport.Width + 64))
            Game1.rainDrops[index].position.X = -64f;
        }
      }
      else
        Game1.updateDebrisWeatherForMovement(Game1.debrisWeather);
    }

    public static void updateDebrisWeatherForMovement(List<WeatherDebris> debris)
    {
      if (debris == null || (double) Game1.fadeToBlackAlpha >= 1.0)
        return;
      int num1 = Game1.viewport.X - (int) Game1.previousViewportPosition.X;
      int num2 = Game1.viewport.Y - (int) Game1.previousViewportPosition.Y;
      int num3 = 16;
      foreach (WeatherDebris debri in debris)
      {
        debri.position.X -= (float) num1 * 1f;
        debri.position.Y -= (float) num2 * 1f;
        if ((double) debri.position.Y > (double) (Game1.viewport.Height + 64 + num3))
          debri.position.Y = -64f;
        else if ((double) debri.position.X < (double) (-64 - num3))
          debri.position.X = (float) Game1.viewport.Width;
        else if ((double) debri.position.Y < (double) (-64 - num3))
          debri.position.Y = (float) Game1.viewport.Height;
        else if ((double) debri.position.X > (double) (Game1.viewport.Width + 64 + num3))
          debri.position.X = -64f;
      }
    }

    public static void randomizeRainPositions()
    {
      for (int index = 0; index < 70; ++index)
        Game1.rainDrops[index] = new RainDrop(Game1.random.Next(Game1.viewport.Width), Game1.random.Next(Game1.viewport.Height), Game1.random.Next(4), Game1.random.Next(70));
    }

    public static void randomizeDebrisWeatherPositions(List<WeatherDebris> debris)
    {
      if (debris == null)
        return;
      foreach (WeatherDebris debri in debris)
        debri.position = Utility.getRandomPositionOnScreen();
    }

    public static void eventFinished()
    {
      Game1.player.canOnlyWalk = false;
      if (Game1.player.bathingClothes.Value)
        Game1.player.canOnlyWalk = true;
      Game1.eventOver = false;
      Game1.eventUp = false;
      Game1.player.CanMove = true;
      Game1.displayHUD = true;
      Game1.player.faceDirection(Game1.player.orientationBeforeEvent);
      Game1.player.completelyStopAnimatingOrDoingAction();
      Game1.viewportFreeze = false;
      Action action = (Action) null;
      if (Game1.currentLocation.currentEvent.onEventFinished != null)
      {
        action = Game1.currentLocation.currentEvent.onEventFinished;
        Game1.currentLocation.currentEvent.onEventFinished = (Action) null;
      }
      LocationRequest locationRequest = (LocationRequest) null;
      if (Game1.currentLocation.currentEvent != null)
      {
        locationRequest = Game1.currentLocation.currentEvent.exitLocation;
        Game1.currentLocation.currentEvent.cleanup();
        Game1.currentLocation.currentEvent = (Event) null;
      }
      if (Game1.player.ActiveObject != null)
        Game1.player.showCarrying();
      if (Game1.IsRainingHere() && (Game1.currentSong == null || !Game1.currentSong.Name.Equals("rain")) && !Game1.currentLocation.Name.StartsWith("UndergroundMine"))
        Game1.changeMusicTrack("rain", true);
      else if (!Game1.IsRainingHere() && (Game1.currentSong == null || Game1.currentSong.Name == null || !Game1.currentSong.Name.Contains(Game1.currentSeason)))
        Game1.changeMusicTrack("none", true);
      if (Game1.dayOfMonth != 0)
        Game1.currentLightSources.Clear();
      if (locationRequest == null && Game1.currentLocation != null && Game1.locationRequest == null)
        locationRequest = new LocationRequest(Game1.currentLocation.NameOrUniqueName, (bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.isStructure, Game1.currentLocation);
      if (locationRequest != null)
      {
        if (locationRequest.Location is Farm && (double) Game1.player.positionBeforeEvent.Y == 64.0)
          ++Game1.player.positionBeforeEvent.X;
        locationRequest.OnWarp += (LocationRequest.Callback) (() => Game1.player.locationBeforeForcedEvent.Value = (string) null);
        Game1.warpFarmer(locationRequest, (int) Game1.player.positionBeforeEvent.X, (int) Game1.player.positionBeforeEvent.Y, Game1.player.orientationBeforeEvent);
      }
      else
      {
        Game1.player.setTileLocation(Game1.player.positionBeforeEvent);
        Game1.player.locationBeforeForcedEvent.Value = (string) null;
      }
      Game1.nonWarpFade = false;
      Game1.fadeToBlackAlpha = 1f;
      if (action == null)
        return;
      action();
    }

    public static void populateDebrisWeatherArray()
    {
      Game1.debrisWeather.Clear();
      Game1.isDebrisWeather = true;
      int num = Game1.random.Next(16, 64);
      int which = Game1.currentSeason.Equals("spring") ? 0 : (Game1.currentSeason.Equals("winter") ? 3 : 2);
      for (int index = 0; index < num; ++index)
        Game1.debrisWeather.Add(new WeatherDebris(new Vector2((float) Game1.random.Next(0, Game1.viewport.Width), (float) Game1.random.Next(0, Game1.viewport.Height)), which, (float) Game1.random.Next(15) / 500f, (float) Game1.random.Next(-10, 0) / 50f, (float) Game1.random.Next(10) / 50f));
    }

    private static void newSeason()
    {
      string currentSeason = Game1.currentSeason;
      if (!(currentSeason == "spring"))
      {
        if (!(currentSeason == "summer"))
        {
          if (!(currentSeason == "fall"))
          {
            if (currentSeason == "winter")
              Game1.currentSeason = "spring";
          }
          else
            Game1.currentSeason = "winter";
        }
        else
          Game1.currentSeason = "fall";
      }
      else
        Game1.currentSeason = "summer";
      Game1.setGraphicsForSeason();
      Game1.dayOfMonth = 1;
      Utility.ForAllLocations((Action<GameLocation>) (l => l.seasonUpdate(Game1.GetSeasonForLocation(l))));
    }

    public static void playItemNumberSelectSound()
    {
      if (Game1.selectedItemsType.Equals("flutePitch"))
      {
        if (Game1.soundBank == null)
          return;
        ICue cue = Game1.soundBank.GetCue("flute");
        cue.SetVariable("Pitch", 100 * Game1.numberOfSelectedItems);
        cue.Play();
      }
      else if (Game1.selectedItemsType.Equals("drumTone"))
        Game1.playSound("drumkit" + Game1.numberOfSelectedItems.ToString());
      else
        Game1.playSound("toolSwap");
    }

    public static void slotsDone()
    {
      Response[] answerChoices = new Response[2]
      {
        new Response("Play", Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2766")),
        new Response("Leave", Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2768"))
      };
      if (Game1.slotResult[3] == 'x')
      {
        Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2769", (object) Game1.player.clubCoins), answerChoices, Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2771") + Game1.currentLocation.map.GetLayer("Buildings").PickTile(new Location((int) ((double) Game1.player.GetGrabTile().X * 64.0), (int) ((double) Game1.player.GetGrabTile().Y * 64.0)), Game1.viewport.Size).Properties["Action"].ToString().Split(' ')[1]);
        Game1.currentDialogueCharacterIndex = Game1.currentObjectDialogue.Peek().Length - 1;
      }
      else
      {
        Game1.playSound("money");
        string str = Game1.slotResult.Substring(0, 3).Equals("===") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2776") : "";
        Game1.player.clubCoins += Convert.ToInt32(Game1.slotResult.Substring(3));
        Game1.currentLocation.createQuestionDialogue(Game1.parseText(str + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2777", (object) Game1.slotResult.Substring(3))), answerChoices, Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2771") + Game1.currentLocation.map.GetLayer("Buildings").PickTile(new Location((int) ((double) Game1.player.GetGrabTile().X * 64.0), (int) ((double) Game1.player.GetGrabTile().Y * 64.0)), Game1.viewport.Size).Properties["Action"].ToString().Split(' ')[1]);
        Game1.currentDialogueCharacterIndex = Game1.currentObjectDialogue.Peek().Length - 1;
      }
    }

    public static void prepareMultiplayerWedding(Farmer farmer)
    {
    }

    public static void prepareSpouseForWedding(Farmer farmer)
    {
      NPC characterFromName = Game1.getCharacterFromName(farmer.spouse);
      characterFromName.Schedule = (Dictionary<int, SchedulePathDescription>) null;
      characterFromName.DefaultMap = farmer.homeLocation.Value;
      characterFromName.DefaultPosition = Utility.PointToVector2((Game1.getLocationFromName(farmer.homeLocation.Value) as FarmHouse).getSpouseBedSpot(farmer.spouse)) * 64f;
      characterFromName.DefaultFacingDirection = 2;
    }

    public static void AddModNPCs()
    {
      LocalizedContentManager localizedContentManager = new LocalizedContentManager(Game1.game1.Content.ServiceProvider, Game1.game1.Content.RootDirectory);
      Dictionary<string, string> dictionary1 = localizedContentManager.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      Dictionary<string, string> dictionary2 = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
      foreach (string key in dictionary2.Keys)
      {
        if (!dictionary1.ContainsKey(key))
        {
          try
          {
            if (Game1.getCharacterFromName(key, useLocationsListOnly: true) == null)
            {
              string str = dictionary2[key];
              Game1.getLocationFromNameInLocationsList(str.Split('/')[10].Split(' ')[0])?.addCharacter(new NPC(new AnimatedSprite("Characters\\" + NPC.getTextureNameForCharacter(key), 0, 16, 32), new Vector2((float) (Convert.ToInt32(str.Split('/')[10].Split(' ')[1]) * 64), (float) (Convert.ToInt32(str.Split('/')[10].Split(' ')[2]) * 64)), str.Split('/')[10].Split(' ')[0], 0, key, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\" + NPC.getTextureNameForCharacter(key)), false));
            }
          }
          catch (Exception ex)
          {
          }
        }
      }
      localizedContentManager.Unload();
    }

    public static void fixProblems()
    {
      if (!Game1.IsMasterGame)
        return;
      List<NPC> pooledList = Utility.getPooledList();
      try
      {
        Utility.getAllCharacters(pooledList);
        Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
        foreach (string key in dictionary.Keys)
        {
          bool flag = false;
          if ((!(key == "Kent") || Game1.year > 1) && (!(key == "Leo") || Game1.MasterPlayer.hasOrWillReceiveMail("addedParrotBoy")))
          {
            foreach (NPC npc in pooledList)
            {
              if (npc.isVillager() && npc.Name.Equals(key))
              {
                flag = true;
                if ((bool) (NetFieldBase<bool, NetBool>) npc.datable)
                {
                  if (npc.getSpouse() == null)
                  {
                    string str = dictionary[key].Split('/')[10].Split(' ')[0];
                    if (npc.DefaultMap != str)
                    {
                      if (!npc.DefaultMap.ToLower().Contains("cabin"))
                      {
                        if (!npc.DefaultMap.Equals("FarmHouse"))
                          break;
                      }
                      Console.WriteLine("Fixing " + npc.Name + " who was improperly divorced and left stranded");
                      npc.PerformDivorce();
                      break;
                    }
                    break;
                  }
                  break;
                }
                break;
              }
            }
            if (!flag)
            {
              try
              {
                Game1.getLocationFromName(dictionary[key].Split('/')[10].Split(' ')[0]).addCharacter(new NPC(new AnimatedSprite("Characters\\" + NPC.getTextureNameForCharacter(key), 0, 16, 32), new Vector2((float) (Convert.ToInt32(dictionary[key].Split('/')[10].Split(' ')[1]) * 64), (float) (Convert.ToInt32(dictionary[key].Split('/')[10].Split(' ')[2]) * 64)), dictionary[key].Split('/')[10].Split(' ')[0], 0, key, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\" + NPC.getTextureNameForCharacter(key)), false));
              }
              catch (Exception ex)
              {
              }
            }
          }
        }
      }
      finally
      {
        Utility.returnPooledList(pooledList);
      }
      int num1 = Game1.getAllFarmers().Count<Farmer>();
      Dictionary<Type, int> missingTools = new Dictionary<Type, int>();
      missingTools.Add(typeof (Axe), num1);
      missingTools.Add(typeof (Pickaxe), num1);
      missingTools.Add(typeof (Hoe), num1);
      missingTools.Add(typeof (WateringCan), num1);
      missingTools.Add(typeof (Wand), 0);
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.hasOrWillReceiveMail("ReturnScepter"))
          missingTools[typeof (Wand)]++;
      }
      int missingScythes = num1;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.toolBeingUpgraded.Value != null && missingTools.ContainsKey(allFarmer.toolBeingUpgraded.Value.GetType()))
          missingTools[allFarmer.toolBeingUpgraded.Value.GetType()]--;
        for (int index = 0; index < allFarmer.items.Count; ++index)
        {
          if (allFarmer.items[index] != null)
            Game1.checkIsMissingTool(missingTools, ref missingScythes, allFarmer.items[index]);
        }
      }
      bool flag1 = true;
      for (int index = 0; index < missingTools.Count; ++index)
      {
        if (missingTools.ElementAt<KeyValuePair<Type, int>>(index).Value > 0)
        {
          flag1 = false;
          break;
        }
      }
      if (missingScythes > 0)
        flag1 = false;
      if (flag1)
        return;
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        List<Debris> debrisList = new List<Debris>();
        foreach (Debris debri in location.debris)
        {
          Item obj = debri.item;
          if (obj != null)
          {
            for (int index = 0; index < missingTools.Count; ++index)
            {
              if (obj.GetType() == missingTools.ElementAt<KeyValuePair<Type, int>>(index).Key)
                debrisList.Add(debri);
            }
            if (obj is MeleeWeapon && (obj as MeleeWeapon).Name.Equals("Scythe"))
              debrisList.Add(debri);
          }
        }
        foreach (Debris debris in debrisList)
          location.debris.Remove(debris);
      }
      Utility.iterateChestsAndStorage((Action<Item>) (item => Game1.checkIsMissingTool(missingTools, ref missingScythes, item)));
      List<string> stringList1 = new List<string>();
label_82:
      for (int index = 0; index < missingTools.Count; ++index)
      {
        KeyValuePair<Type, int> keyValuePair = missingTools.ElementAt<KeyValuePair<Type, int>>(index);
        if (keyValuePair.Value > 0)
        {
          int num2 = 0;
          while (true)
          {
            int num3 = num2;
            keyValuePair = missingTools.ElementAt<KeyValuePair<Type, int>>(index);
            int num4 = keyValuePair.Value;
            if (num3 < num4)
            {
              List<string> stringList2 = stringList1;
              keyValuePair = missingTools.ElementAt<KeyValuePair<Type, int>>(index);
              string str = keyValuePair.Key.ToString();
              stringList2.Add(str);
              ++num2;
            }
            else
              goto label_82;
          }
        }
      }
      for (int index = 0; index < missingScythes; ++index)
        stringList1.Add("Scythe");
      if (stringList1.Count > 0)
        Game1.addMailForTomorrow("foundLostTools");
      for (int index = 0; index < stringList1.Count; ++index)
      {
        Item obj = (Item) null;
        string str = stringList1[index];
        if (!(str == "StardewValley.Tools.Axe"))
        {
          if (!(str == "StardewValley.Tools.Hoe"))
          {
            if (!(str == "StardewValley.Tools.WateringCan"))
            {
              if (!(str == "Scythe"))
              {
                if (!(str == "StardewValley.Tools.Pickaxe"))
                {
                  if (str == "StardewValley.Tools.Wand")
                    obj = (Item) new Wand();
                }
                else
                  obj = (Item) new Pickaxe();
              }
              else
                obj = (Item) new MeleeWeapon(47);
            }
            else
              obj = (Item) new WateringCan();
          }
          else
            obj = (Item) new Hoe();
        }
        else
          obj = (Item) new Axe();
        if (obj != null)
        {
          if (Game1.newDaySync != null)
            Game1.player.team.newLostAndFoundItems.Value = true;
          Game1.player.team.returnedDonations.Add(obj);
        }
      }
    }

    private static void checkIsMissingTool(
      Dictionary<Type, int> missingTools,
      ref int missingScythes,
      Item item)
    {
      for (int index = 0; index < missingTools.Count; ++index)
      {
        if (item.GetType() == missingTools.ElementAt<KeyValuePair<Type, int>>(index).Key)
          missingTools[missingTools.ElementAt<KeyValuePair<Type, int>>(index).Key]--;
      }
      if (!(item is MeleeWeapon) || !(item as MeleeWeapon).Name.Equals("Scythe"))
        return;
      --missingScythes;
    }

    public static void newDayAfterFade(Action after)
    {
      if (Game1.player.currentLocation != null)
      {
        if (Game1.player.rightRing.Value != null)
          Game1.player.rightRing.Value.onLeaveLocation(Game1.player, Game1.player.currentLocation);
        if (Game1.player.leftRing.Value != null)
          Game1.player.leftRing.Value.onLeaveLocation(Game1.player, Game1.player.currentLocation);
      }
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        Game1 game1 = Game1.game1;
        Game1.hooks.OnGame1_NewDayAfterFade((Action) (() =>
        {
          Game1.game1.isLocalMultiplayerNewDayActive = true;
          Game1._afterNewDayAction = after;
          GameRunner.instance.activeNewDayProcesses.Add(new KeyValuePair<Game1, IEnumerator<int>>(Game1.game1, Game1._newDayAfterFade()));
        }));
      }
      else
        Game1.hooks.OnGame1_NewDayAfterFade((Action) (() =>
        {
          Game1._afterNewDayAction = after;
          if (Game1._newDayTask != null)
          {
            Console.WriteLine("Warning: There is already a _newDayTask; unusual code path.");
            Console.WriteLine(Environment.StackTrace);
            Console.WriteLine();
          }
          else
            Game1._newDayTask = new Task((Action) (() =>
            {
              Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
              IEnumerator<int> enumerator = Game1._newDayAfterFade();
              do
                ;
              while (enumerator.MoveNext());
            }));
        }));
    }

    public static bool CanAcceptDailyQuest() => Game1.questOfTheDay != null && !Game1.player.acceptedDailyQuest.Value && Game1.questOfTheDay.questDescription != null && Game1.questOfTheDay.questDescription.Length != 0;

    private static IEnumerator<int> _newDayAfterFade()
    {
      Game1.newDaySync.start();
      Game1.flushLocationLookup();
      try
      {
        Game1.fixProblems();
      }
      catch (Exception ex)
      {
      }
      foreach (Farmer allFarmer in Game1.getAllFarmers())
        allFarmer.FarmerSprite.PauseForSingleAnimation = false;
      Game1.whereIsTodaysFest = (string) null;
      if (Game1.wind != null)
      {
        Game1.wind.Stop(AudioStopOptions.Immediate);
        Game1.wind = (ICue) null;
      }
      foreach (int key in new List<int>((IEnumerable<int>) Game1.player.chestConsumedMineLevels.Keys))
      {
        if (key > 120)
          Game1.player.chestConsumedMineLevels.Remove(key);
      }
      Game1.player.currentEyes = 0;
      int Seed;
      if (Game1.IsMasterGame)
      {
        Game1.player.team.announcedSleepingFarmers.Clear();
        Seed = (int) Game1.uniqueIDForThisGame / 100 + (int) Game1.stats.DaysPlayed * 10 + 1 + (int) Game1.stats.StepsTaken;
        Game1.newDaySync.sendVar<NetInt, int>("seed", Seed);
      }
      else
      {
        while (!Game1.newDaySync.isVarReady("seed"))
          yield return 0;
        Seed = Game1.newDaySync.waitForVar<NetInt, int>("seed");
      }
      Game1.random = new Random(Seed);
      for (int index = 0; index < Game1.dayOfMonth; ++index)
        Game1.random.Next();
      Game1.player.team.endOfNightStatus.UpdateState("sleep");
      Game1.newDaySync.barrier("sleep");
      while (!Game1.newDaySync.isBarrierReady("sleep"))
        yield return 0;
      Game1.gameTimeInterval = 0;
      Game1.game1.wasAskedLeoMemory = false;
      Game1.player.team.Update();
      Game1.player.team.NewDay();
      Game1.player.passedOut = false;
      Game1.player.CanMove = true;
      Game1.player.FarmerSprite.PauseForSingleAnimation = false;
      Game1.player.FarmerSprite.StopAnimation();
      Game1.player.completelyStopAnimatingOrDoingAction();
      Game1.changeMusicTrack("none");
      int parentSheetIndex = Game1.random.Next(194, 240);
      while (((IEnumerable<int>) Utility.getForbiddenDishesOfTheDay()).Contains<int>(parentSheetIndex))
        parentSheetIndex = Game1.random.Next(194, 240);
      int initialStack = Game1.random.Next(1, 4 + (Game1.random.NextDouble() < 0.08 ? 10 : 0));
      if (Game1.IsMasterGame)
        Game1.dishOfTheDay = new Object(Vector2.Zero, parentSheetIndex, initialStack);
      Game1.newDaySync.barrier("dishOfTheDay");
      while (!Game1.newDaySync.isBarrierReady("dishOfTheDay"))
        yield return 0;
      Game1.npcDialogues = (Dictionary<string, Stack<Dialogue>>) null;
      foreach (NPC allCharacter in Utility.getAllCharacters())
        allCharacter.updatedDialogueYet = false;
      int minutesUntilMorning = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        location.currentEvent = (Event) null;
        if (Game1.IsMasterGame)
          location.passTimeForObjects(minutesUntilMorning);
      }
      if (Game1.IsMasterGame)
      {
        foreach (Building building in Game1.getFarm().buildings)
        {
          if (building.indoors.Value != null)
            building.indoors.Value.passTimeForObjects(minutesUntilMorning);
        }
      }
      Game1.globalOutdoorLighting = 0.0f;
      Game1.outdoorLight = Microsoft.Xna.Framework.Color.White;
      Game1.ambientLight = Microsoft.Xna.Framework.Color.White;
      if (Game1.isLightning && Game1.IsMasterGame)
        Utility.overnightLightning();
      Game1.tmpTimeOfDay = Game1.timeOfDay;
      if (Game1.MasterPlayer.hasOrWillReceiveMail("ccBulletinThankYou") && !Game1.player.hasOrWillReceiveMail("ccBulletinThankYou"))
        Game1.addMailForTomorrow("ccBulletinThankYou");
      Game1.ReceiveMailForTomorrow();
      if (Game1.player.friendshipData.Count() > 0)
      {
        string key = Game1.player.friendshipData.Keys.ElementAt<string>(Game1.random.Next(Game1.player.friendshipData.Keys.Count<string>()));
        if (Game1.random.NextDouble() < (double) (Game1.player.friendshipData[key].Points / 250) * 0.1 && (Game1.player.spouse == null || !Game1.player.spouse.Equals(key)) && Game1.content.Load<Dictionary<string, string>>("Data\\mail").ContainsKey(key))
          Game1.mailbox.Add(key);
      }
      MineShaft.clearActiveMines();
      VolcanoDungeon.ClearAllLevels();
      for (int index = Game1.player.enchantments.Count - 1; index >= 0; --index)
        Game1.player.enchantments[index].OnUnequip(Game1.player);
      Game1.player.dayupdate();
      if (Game1.IsMasterGame)
        Game1.player.team.sharedDailyLuck.Value = Math.Min(0.100000001490116, (double) Game1.random.Next(-100, 101) / 1000.0);
      ++Game1.dayOfMonth;
      ++Game1.stats.DaysPlayed;
      Game1.startedJukeboxMusic = false;
      Game1.player.dayOfMonthForSaveGame = new int?(Game1.dayOfMonth);
      Game1.player.seasonForSaveGame = new int?(Utility.getSeasonNumber(Game1.currentSeason));
      Game1.player.yearForSaveGame = new int?(Game1.year);
      Game1.player.showToolUpgradeAvailability();
      if (Game1.IsMasterGame)
      {
        Game1.queueWeddingsForToday();
        Game1.newDaySync.sendVar<NetRef<NetLongList>, NetLongList>("weddingsToday", new NetLongList((IEnumerable<long>) Game1.weddingsToday));
      }
      else
      {
        while (!Game1.newDaySync.isVarReady("weddingsToday"))
          yield return 0;
        Game1.weddingsToday = new List<long>((IEnumerable<long>) Game1.newDaySync.waitForVar<NetRef<NetLongList>, NetLongList>("weddingsToday"));
      }
      Game1.weddingToday = false;
      foreach (long id in Game1.weddingsToday)
      {
        Farmer farmer = Game1.getFarmer(id);
        if (farmer != null && !farmer.hasCurrentOrPendingRoommate())
        {
          Game1.weddingToday = true;
          break;
        }
      }
      if (Game1.player.spouse != null && Game1.player.isEngaged() && Game1.weddingsToday.Contains(Game1.player.UniqueMultiplayerID))
      {
        Friendship friendship = Game1.player.friendshipData[Game1.player.spouse];
        if (friendship.CountdownToWedding <= 1)
        {
          friendship.Status = FriendshipStatus.Married;
          friendship.WeddingDate = new WorldDate(Game1.Date);
          Game1.prepareSpouseForWedding(Game1.player);
        }
      }
      NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>> additional_shipped_items = new NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>>();
      if (Game1.IsMasterGame)
        Utility.ForAllLocations((Action<GameLocation>) (location =>
        {
          foreach (Object @object in location.objects.Values)
          {
            if (@object is Chest && @object is Chest chest2 && chest2.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
            {
              if ((bool) (NetFieldBase<bool, NetBool>) Game1.player.team.useSeparateWallets)
              {
                foreach (long key in chest2.separateWalletItems.Keys)
                {
                  if (!additional_shipped_items.ContainsKey(key))
                    additional_shipped_items[key] = new NetList<Item, NetRef<Item>>();
                  List<Item> objList = new List<Item>((IEnumerable<Item>) chest2.separateWalletItems[key]);
                  chest2.separateWalletItems[key].Clear();
                  foreach (Item obj in objList)
                    additional_shipped_items[key].Add(obj);
                }
              }
              else
              {
                NetCollection<Item> shippingBin = Game1.getFarm().getShippingBin(Game1.player);
                List<Item> objList = new List<Item>((IEnumerable<Item>) chest2.items);
                chest2.items.Clear();
                foreach (Item obj in objList)
                  shippingBin.Add(obj);
              }
              chest2.items.Clear();
              chest2.separateWalletItems.Clear();
            }
          }
        }));
      if (Game1.IsMasterGame)
      {
        Game1.newDaySync.sendVar<NetRef<NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>>>, NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>>>("additional_shipped_items", additional_shipped_items);
      }
      else
      {
        while (!Game1.newDaySync.isVarReady("additional_shipped_items"))
          yield return 0;
        additional_shipped_items = Game1.newDaySync.waitForVar<NetRef<NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>>>, NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>>>("additional_shipped_items");
      }
      if (Game1.player.team.useSeparateWallets.Value)
      {
        NetCollection<Item> shippingBin = Game1.getFarm().getShippingBin(Game1.player);
        if (additional_shipped_items.ContainsKey(Game1.player.UniqueMultiplayerID))
        {
          foreach (Item obj in additional_shipped_items[Game1.player.UniqueMultiplayerID])
            shippingBin.Add(obj);
        }
      }
      Game1.newDaySync.barrier("handleMiniShippingBins");
      while (!Game1.newDaySync.isBarrierReady("handleMiniShippingBins"))
        yield return 0;
      NetCollection<Item> shippingBin1 = Game1.getFarm().getShippingBin(Game1.player);
      foreach (Item obj in shippingBin1)
        Game1.player.displayedShippedItems.Add(obj);
      if (Game1.player.useSeparateWallets || !Game1.player.useSeparateWallets && Game1.player.IsMainPlayer)
      {
        int num1 = 0;
        foreach (Item obj in shippingBin1)
        {
          int num2 = 0;
          if (obj is Object)
          {
            num2 = (obj as Object).sellToStorePrice() * obj.Stack;
            num1 += num2;
          }
          if (Game1.player.team.specialOrders != null)
          {
            foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
            {
              if (specialOrder.onItemShipped != null)
                specialOrder.onItemShipped(Game1.player, obj, num2);
            }
          }
        }
        Game1.player.Money += num1;
      }
      if (Game1.IsMasterGame)
      {
        if (Game1.currentSeason.Equals("winter") && Game1.dayOfMonth == 18)
        {
          GameLocation locationFromName1 = Game1.getLocationFromName("Submarine");
          if (locationFromName1.objects.Count() >= 0)
            Utility.transferPlacedObjectsFromOneLocationToAnother(locationFromName1, (GameLocation) null, new Vector2?(new Vector2(20f, 20f)), Game1.getLocationFromName("Beach"));
          GameLocation locationFromName2 = Game1.getLocationFromName("MermaidHouse");
          if (locationFromName2.objects.Count() >= 0)
            Utility.transferPlacedObjectsFromOneLocationToAnother(locationFromName2, (GameLocation) null, new Vector2?(new Vector2(21f, 20f)), Game1.getLocationFromName("Beach"));
        }
        if (Game1.player.hasOrWillReceiveMail("pamHouseUpgrade") && !Game1.player.hasOrWillReceiveMail("transferredObjectsPamHouse"))
        {
          Game1.addMailForTomorrow("transferredObjectsPamHouse", true);
          GameLocation locationFromName3 = Game1.getLocationFromName("Trailer");
          GameLocation locationFromName4 = Game1.getLocationFromName("Trailer_Big");
          if (locationFromName3.objects.Count() >= 0)
            Utility.transferPlacedObjectsFromOneLocationToAnother(locationFromName3, locationFromName4, new Vector2?(new Vector2(14f, 23f)));
        }
        if (Utility.HasAnyPlayerSeenEvent(191393) && !Game1.player.hasOrWillReceiveMail("transferredObjectsJojaMart"))
        {
          Game1.addMailForTomorrow("transferredObjectsJojaMart", true);
          GameLocation locationFromName = Game1.getLocationFromName("JojaMart");
          if (locationFromName.objects.Count() >= 0)
            Utility.transferPlacedObjectsFromOneLocationToAnother(locationFromName, (GameLocation) null, new Vector2?(new Vector2(89f, 51f)), Game1.getLocationFromName("Town"));
        }
      }
      if (Game1.player.useSeparateWallets && Game1.player.IsMainPlayer)
      {
        foreach (Farmer allFarmhand in Game1.getAllFarmhands())
        {
          if (!allFarmhand.isActive() && !allFarmhand.isUnclaimedFarmhand)
          {
            int num3 = 0;
            foreach (Item obj in Game1.getFarm().getShippingBin(allFarmhand))
            {
              int num4 = 0;
              if (obj is Object)
              {
                num4 = (obj as Object).sellToStorePrice(allFarmhand.UniqueMultiplayerID) * obj.Stack;
                num3 += num4;
              }
              if (Game1.player.team.specialOrders != null)
              {
                foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                {
                  if (specialOrder.onItemShipped != null)
                    specialOrder.onItemShipped(Game1.player, obj, num4);
                }
              }
            }
            Game1.player.team.AddIndividualMoney(allFarmhand, num3);
            Game1.getFarm().getShippingBin(allFarmhand).Clear();
          }
        }
      }
      List<NPC> divorceNPCs = new List<NPC>();
      if (Game1.IsMasterGame)
      {
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (allFarmer.isActive() && (bool) (NetFieldBase<bool, NetBool>) allFarmer.divorceTonight && allFarmer.getSpouse() != null)
            divorceNPCs.Add(allFarmer.getSpouse());
        }
      }
      Game1.newDaySync.barrier("player.dayupdate");
      while (!Game1.newDaySync.isBarrierReady("player.dayupdate"))
        yield return 0;
      if ((bool) (NetFieldBase<bool, NetBool>) Game1.player.divorceTonight)
        Game1.player.doDivorce();
      Game1.newDaySync.barrier("player.divorce");
      while (!Game1.newDaySync.isBarrierReady("player.divorce"))
        yield return 0;
      if (Game1.IsMasterGame)
      {
        foreach (NPC npc in divorceNPCs)
        {
          if (npc.getSpouse() == null)
            npc.PerformDivorce();
        }
      }
      Game1.newDaySync.barrier("player.finishDivorce");
      while (!Game1.newDaySync.isBarrierReady("player.finishDivorce"))
        yield return 0;
      if (Game1.IsMasterGame && (bool) (NetFieldBase<bool, NetBool>) Game1.player.changeWalletTypeTonight)
      {
        if (Game1.player.useSeparateWallets)
          ManorHouse.MergeWallets();
        else
          ManorHouse.SeparateWallets();
      }
      Game1.newDaySync.barrier("player.wallets");
      while (!Game1.newDaySync.isBarrierReady("player.wallets"))
        yield return 0;
      Game1.getFarm().lastItemShipped = (Item) null;
      Game1.getFarm().getShippingBin(Game1.player).Clear();
      Game1.newDaySync.barrier("clearShipping");
      while (!Game1.newDaySync.isBarrierReady("clearShipping"))
        yield return 0;
      if (Game1.IsClient)
      {
        Game1.multiplayer.sendFarmhand();
        Game1.newDaySync.processMessages();
      }
      Game1.newDaySync.barrier("sendFarmhands");
      while (!Game1.newDaySync.isBarrierReady("sendFarmhands"))
        yield return 0;
      if (Game1.IsMasterGame)
        Game1.multiplayer.saveFarmhands();
      Game1.newDaySync.barrier("saveFarmhands");
      while (!Game1.newDaySync.isBarrierReady("saveFarmhands"))
        yield return 0;
      if (Game1.IsMasterGame && Game1.dayOfMonth >= 15 && Game1.dayOfMonth <= 17 && Game1.currentSeason.Equals("winter") && Game1.IsMasterGame && Game1.netWorldState.Value.VisitsUntilY1Guarantee >= 0)
        --Game1.netWorldState.Value.VisitsUntilY1Guarantee;
      if (Game1.dayOfMonth == 27 && Game1.currentSeason.Equals("spring"))
      {
        int year1 = Game1.year;
      }
      if (Game1.dayOfMonth == 29)
      {
        Game1.newSeason();
        if (!Game1.currentSeason.Equals("winter"))
          Game1.cropsOfTheWeek = Utility.cropsOfTheWeek();
        if (Game1.currentSeason.Equals("spring"))
        {
          ++Game1.year;
          if (Game1.year == 2)
            Game1.addKentIfNecessary();
        }
        int year2 = Game1.year;
      }
      if (Game1.IsMasterGame && (Game1.dayOfMonth == 1 || Game1.dayOfMonth == 8 || Game1.dayOfMonth == 15 || Game1.dayOfMonth == 22))
        SpecialOrder.UpdateAvailableSpecialOrders(true);
      if (Game1.IsMasterGame)
        Game1.netWorldState.Value.UpdateFromGame1();
      Game1.newDaySync.barrier("date");
      while (!Game1.newDaySync.isBarrierReady("date"))
        yield return 0;
      if (Game1.IsMasterGame)
      {
        for (int index = 0; index < Game1.player.team.specialOrders.Count; ++index)
        {
          SpecialOrder specialOrder = Game1.player.team.specialOrders[index];
          if ((SpecialOrder.QuestState) (NetFieldBase<SpecialOrder.QuestState, NetEnum<SpecialOrder.QuestState>>) specialOrder.questState != SpecialOrder.QuestState.Complete && specialOrder.GetDaysLeft() <= 0)
          {
            specialOrder.OnFail();
            Game1.player.team.specialOrders.RemoveAt(index);
            --index;
          }
        }
      }
      Game1.newDaySync.barrier("processOrders");
      while (!Game1.newDaySync.isBarrierReady("processOrders"))
        yield return 0;
      List<string> stringList = new List<string>((IEnumerable<string>) Game1.player.team.mailToRemoveOvernight);
      List<int> intList = new List<int>((IEnumerable<int>) Game1.player.team.itemsToRemoveOvernight);
      if (Game1.IsMasterGame)
      {
        foreach (string rule in Game1.player.team.specialRulesRemovedToday)
          SpecialOrder.RemoveSpecialRuleAtEndOfDay(rule);
      }
      Game1.player.team.specialRulesRemovedToday.Clear();
      foreach (int parent_sheet_index in intList)
      {
        if (Game1.IsMasterGame)
        {
          Game1.game1._PerformRemoveNormalItemFromWorldOvernight(parent_sheet_index);
          foreach (Farmer allFarmer in Game1.getAllFarmers())
            Game1.game1._PerformRemoveNormalItemFromFarmerOvernight(allFarmer, parent_sheet_index);
        }
        else
          Game1.game1._PerformRemoveNormalItemFromFarmerOvernight(Game1.player, parent_sheet_index);
      }
      foreach (string mail_key in stringList)
      {
        if (Game1.IsMasterGame)
        {
          foreach (Farmer allFarmer in Game1.getAllFarmers())
            allFarmer.RemoveMail(mail_key, allFarmer == Game1.MasterPlayer);
        }
        else
          Game1.player.RemoveMail(mail_key);
      }
      Game1.newDaySync.barrier("removeItemsFromWorld");
      while (!Game1.newDaySync.isBarrierReady("removeItemsFromWorld"))
        yield return 0;
      if (Game1.IsMasterGame)
      {
        Game1.player.team.itemsToRemoveOvernight.Clear();
        Game1.player.team.mailToRemoveOvernight.Clear();
      }
      if (Game1.content.Load<Dictionary<string, string>>("Data\\mail").ContainsKey(Game1.currentSeason + "_" + Game1.dayOfMonth.ToString() + "_" + Game1.year.ToString()))
        Game1.mailbox.Add(Game1.currentSeason + "_" + Game1.dayOfMonth.ToString() + "_" + Game1.year.ToString());
      else if (Game1.content.Load<Dictionary<string, string>>("Data\\mail").ContainsKey(Game1.currentSeason + "_" + Game1.dayOfMonth.ToString()))
        Game1.mailbox.Add(Game1.currentSeason + "_" + Game1.dayOfMonth.ToString());
      if (Game1.IsMasterGame && Game1.player.team.toggleMineShrineOvernight.Value)
      {
        Game1.player.team.toggleMineShrineOvernight.Value = false;
        Game1.player.team.mineShrineActivated.Value = !Game1.player.team.mineShrineActivated.Value;
        if (Game1.player.team.mineShrineActivated.Value)
          ++Game1.netWorldState.Value.MinesDifficulty;
        else
          --Game1.netWorldState.Value.MinesDifficulty;
      }
      if (Game1.IsMasterGame)
      {
        if (!Game1.player.team.SpecialOrderRuleActive("MINE_HARD") && Game1.netWorldState.Value.MinesDifficulty > 1)
          Game1.netWorldState.Value.MinesDifficulty = 1;
        if (!Game1.player.team.SpecialOrderRuleActive("SC_HARD") && Game1.netWorldState.Value.SkullCavesDifficulty > 0)
          Game1.netWorldState.Value.SkullCavesDifficulty = 0;
      }
      Game1.RefreshQuestOfTheDay();
      Game1.weatherForTomorrow = Game1.getWeatherModificationsForDate(Game1.Date, Game1.weatherForTomorrow);
      if (Game1.weddingToday)
        Game1.weatherForTomorrow = 6;
      Game1.wasRainingYesterday = Game1.isRaining || Game1.isLightning;
      if (Game1.weatherForTomorrow == 1 || Game1.weatherForTomorrow == 3)
        Game1.isRaining = true;
      if (Game1.weatherForTomorrow == 3)
        Game1.isLightning = true;
      if (Game1.weatherForTomorrow == 0 || Game1.weatherForTomorrow == 2 || Game1.weatherForTomorrow == 4 || Game1.weatherForTomorrow == 5 || Game1.weatherForTomorrow == 6)
      {
        Game1.isRaining = false;
        Game1.isLightning = false;
        Game1.isSnowing = false;
        if (Game1.weatherForTomorrow == 5)
          Game1.isSnowing = true;
      }
      if (!Game1.isRaining && !Game1.isLightning)
      {
        ++Game1.currentSongIndex;
        if (Game1.currentSongIndex > 3 || Game1.dayOfMonth == 1)
          Game1.currentSongIndex = 1;
      }
      if (Game1.IsMasterGame)
        Game1.game1.SetOtherLocationWeatherForTomorrow(Game1.random);
      if ((Game1.isRaining || Game1.isSnowing || Game1.isLightning) && Game1.currentLocation.GetLocationContext() == GameLocation.LocationContext.Default)
        Game1.changeMusicTrack("none");
      else if (Game1.weatherForTomorrow == 4 && Game1.weatherForTomorrow == 6)
        Game1.changeMusicTrack("none");
      Game1.debrisWeather.Clear();
      Game1.isDebrisWeather = false;
      if (Game1.weatherForTomorrow == 2)
        Game1.populateDebrisWeatherArray();
      Game1.chanceToRainTomorrow = !Game1.currentSeason.Equals("summer") ? (!Game1.currentSeason.Equals("winter") ? 0.183 : 0.63) : (Game1.dayOfMonth > 1 ? 0.12 + (double) Game1.dayOfMonth * (3.0 / 1000.0) : 0.0);
      if (Game1.random.NextDouble() < Game1.chanceToRainTomorrow)
      {
        Game1.weatherForTomorrow = 1;
        if (Game1.currentSeason.Equals("summer") && Game1.random.NextDouble() < 0.85 || !Game1.currentSeason.Equals("winter") && Game1.random.NextDouble() < 0.25 && Game1.dayOfMonth > 2 && Game1.stats.DaysPlayed > 27U)
          Game1.weatherForTomorrow = 3;
        if (Game1.currentSeason.Equals("winter"))
          Game1.weatherForTomorrow = 5;
      }
      else
        Game1.weatherForTomorrow = Game1.stats.DaysPlayed <= 2U || (!Game1.currentSeason.Equals("spring") || Game1.random.NextDouble() >= 0.2) && (!Game1.currentSeason.Equals("fall") || Game1.random.NextDouble() >= 0.6) || Game1.weddingToday ? 0 : 2;
      if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
        Game1.weatherForTomorrow = 4;
      if (Game1.stats.DaysPlayed == 2U)
        Game1.weatherForTomorrow = 1;
      if (Game1.IsMasterGame)
      {
        Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Default).weatherForTomorrow.Value = Game1.weatherForTomorrow;
        Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Default).isRaining.Value = Game1.isRaining;
        Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Default).isSnowing.Value = Game1.isSnowing;
        Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Default).isLightning.Value = Game1.isLightning;
        Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Default).isDebrisWeather.Value = Game1.isDebrisWeather;
      }
      foreach (NPC allCharacter in Utility.getAllCharacters())
      {
        Game1.player.mailReceived.Remove(allCharacter.Name);
        Game1.player.mailReceived.Remove(allCharacter.Name + "Cooking");
        allCharacter.drawOffset.Value = Vector2.Zero;
      }
      FarmAnimal.reservedGrass.Clear();
      if (Game1.IsMasterGame)
      {
        int num;
        NPC.hasSomeoneRepairedTheFences = (num = 0) != 0;
        NPC.hasSomeoneFedTheAnimals = num != 0;
        NPC.hasSomeoneFedThePet = num != 0;
        NPC.hasSomeoneWateredCrops = num != 0;
        foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
        {
          location.ResetCharacterDialogues();
          location.DayUpdate(Game1.dayOfMonth);
        }
        Game1.UpdateHorseOwnership();
        foreach (NPC allCharacter in Utility.getAllCharacters())
        {
          allCharacter.islandScheduleName.Value = (string) null;
          allCharacter.currentScheduleDelay = 0.0f;
        }
        foreach (NPC allCharacter in Utility.getAllCharacters())
          allCharacter.dayUpdate(Game1.dayOfMonth);
        IslandSouth.SetupIslandSchedules();
        HashSet<NPC> purchased_item_npcs = new HashSet<NPC>();
        Game1.UpdateShopPlayerItemInventory("SeedShop", purchased_item_npcs);
        Game1.UpdateShopPlayerItemInventory("FishShop", purchased_item_npcs);
      }
      if (Game1.IsMasterGame && (bool) (NetFieldBase<bool, NetBool>) Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Island).isRaining)
      {
        Vector2 tile_position = new Vector2(0.0f, 0.0f);
        IslandLocation islandLocation = (IslandLocation) null;
        List<int> list = new List<int>();
        for (int index = 0; index < 4; ++index)
          list.Add(index);
        Utility.Shuffle<int>(new Random((int) Game1.uniqueIDForThisGame), list);
        switch (list[Game1.currentGemBirdIndex])
        {
          case 0:
            islandLocation = Game1.getLocationFromName("IslandSouth") as IslandLocation;
            tile_position = new Vector2(10f, 30f);
            break;
          case 1:
            islandLocation = Game1.getLocationFromName("IslandNorth") as IslandLocation;
            tile_position = new Vector2(56f, 56f);
            break;
          case 2:
            islandLocation = Game1.getLocationFromName("Islandwest") as IslandLocation;
            tile_position = new Vector2(53f, 51f);
            break;
          case 3:
            islandLocation = Game1.getLocationFromName("IslandEast") as IslandLocation;
            tile_position = new Vector2(21f, 35f);
            break;
        }
        Game1.currentGemBirdIndex = (Game1.currentGemBirdIndex + 1) % 4;
        if (islandLocation != null)
          islandLocation.locationGemBird.Value = new IslandGemBird(tile_position, IslandGemBird.GetBirdTypeForLocation(islandLocation.Name));
      }
      if (Game1.IsMasterGame)
      {
        foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
        {
          if (Game1.IsRainingHere(location) && location.IsOutdoors)
          {
            foreach (KeyValuePair<Vector2, TerrainFeature> pair in location.terrainFeatures.Pairs)
            {
              if (pair.Value is HoeDirt && (int) (NetFieldBase<int, NetInt>) ((HoeDirt) pair.Value).state != 2)
                ((HoeDirt) pair.Value).state.Value = 1;
            }
          }
        }
        GameLocation locationFromName = Game1.getLocationFromName("Farm");
        if (Game1.IsRainingHere(locationFromName))
          (locationFromName as Farm).petBowlWatered.Value = true;
      }
      if (Game1.player.currentUpgrade != null)
      {
        --Game1.player.currentUpgrade.daysLeftTillUpgradeDone;
        if (Game1.getLocationFromName("Farm").objects.ContainsKey(new Vector2(Game1.player.currentUpgrade.positionOfCarpenter.X / 64f, Game1.player.currentUpgrade.positionOfCarpenter.Y / 64f)))
          Game1.getLocationFromName("Farm").objects.Remove(new Vector2(Game1.player.currentUpgrade.positionOfCarpenter.X / 64f, Game1.player.currentUpgrade.positionOfCarpenter.Y / 64f));
        if (Game1.player.currentUpgrade.daysLeftTillUpgradeDone == 0)
        {
          string whichBuilding = Game1.player.currentUpgrade.whichBuilding;
          if (!(whichBuilding == "House"))
          {
            if (!(whichBuilding == "Coop"))
            {
              if (!(whichBuilding == "Barn"))
              {
                if (whichBuilding == "Greenhouse")
                {
                  Game1.player.hasGreenhouse = true;
                  Game1.greenhouseTexture = Game1.content.Load<Texture2D>("BuildingUpgrades\\Greenhouse");
                }
              }
              else
              {
                ++Game1.player.BarnUpgradeLevel;
                Game1.currentBarnTexture = Game1.content.Load<Texture2D>("BuildingUpgrades\\Barn" + Game1.player.BarnUpgradeLevel.ToString());
              }
            }
            else
            {
              ++Game1.player.CoopUpgradeLevel;
              Game1.currentCoopTexture = Game1.content.Load<Texture2D>("BuildingUpgrades\\Coop" + Game1.player.CoopUpgradeLevel.ToString());
            }
          }
          else
          {
            ++Game1.player.HouseUpgradeLevel;
            Game1.currentHouseTexture = Game1.content.Load<Texture2D>("Buildings\\House" + Game1.player.HouseUpgradeLevel.ToString());
          }
          Game1.stats.checkForBuildingUpgradeAchievements();
          Game1.removeFrontLayerForFarmBuildings();
          Game1.addNewFarmBuildingMaps();
          Game1.player.currentUpgrade = (BuildingUpgrade) null;
          Game1.changeInvisibility("Robin", false);
        }
        else if (Game1.player.currentUpgrade.daysLeftTillUpgradeDone == 3)
          Game1.changeInvisibility("Robin", true);
      }
      Game1.newDaySync.barrier("buildingUpgrades");
      while (!Game1.newDaySync.isBarrierReady("buildingUpgrades"))
        yield return 0;
      Game1.stats.AverageBedtime = (uint) Game1.timeOfDay;
      Game1.timeOfDay = 600;
      Game1.newDay = false;
      if (Game1.IsMasterGame)
        Game1.netWorldState.Value.UpdateFromGame1();
      if (Game1.player.currentLocation != null)
      {
        Game1.player.currentLocation.resetForPlayerEntry();
        BedFurniture.ApplyWakeUpPosition(Game1.player);
        Game1.forceSnapOnNextViewportUpdate = true;
        Game1.UpdateViewPort(false, new Point(Game1.player.getStandingX(), Game1.player.getStandingY()));
        Game1.previousViewportPosition = new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
      }
      Game1.player.sleptInTemporaryBed.Value = false;
      int currentWallpaper = Game1.currentWallpaper;
      Game1.wallpaperPrice = Game1.random.Next(75, 500) + Game1.player.HouseUpgradeLevel * 100;
      Game1.wallpaperPrice -= Game1.wallpaperPrice % 5;
      int currentFloor = Game1.currentFloor;
      Game1.floorPrice = Game1.random.Next(75, 500) + Game1.player.HouseUpgradeLevel * 100;
      Game1.floorPrice -= Game1.floorPrice % 5;
      Game1.updateWeatherIcon();
      Game1.freezeControls = false;
      if (Game1.stats.DaysPlayed > 1U || !Game1.IsMasterGame)
      {
        Game1.farmEvent = (FarmEvent) null;
        if (Game1.IsMasterGame)
        {
          Game1.farmEvent = Utility.pickFarmEvent();
          Game1.newDaySync.sendVar<NetRef<FarmEvent>, FarmEvent>("farmEvent", Game1.farmEvent);
        }
        else
        {
          while (!Game1.newDaySync.isVarReady("farmEvent"))
            yield return 0;
          Game1.farmEvent = Game1.newDaySync.waitForVar<NetRef<FarmEvent>, FarmEvent>("farmEvent");
        }
        if (Game1.farmEvent == null)
          Game1.farmEvent = Utility.pickPersonalFarmEvent();
        if (Game1.farmEvent != null && Game1.farmEvent.setUp())
          Game1.farmEvent = (FarmEvent) null;
      }
      if (Game1.farmEvent == null)
        Game1.RemoveDeliveredMailForTomorrow();
      if (Game1.player.team.newLostAndFoundItems.Value)
        Game1.morningQueue.Enqueue((DelayedAction.delayedBehavior) (() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:NewLostAndFoundItems"))));
      Game1.newDaySync.barrier("mail");
      while (!Game1.newDaySync.isBarrierReady("mail"))
        yield return 0;
      if (Game1.IsMasterGame)
        Game1.player.team.newLostAndFoundItems.Value = false;
      foreach (Building building in Game1.getFarm().buildings)
      {
        if ((int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0 && building.indoors.Value is Cabin)
        {
          Game1.player.slotCanHost = true;
          break;
        }
      }
      if ((double) Utility.percentGameComplete() >= 1.0)
        Game1.player.team.farmPerfect.Value = true;
      Game1.newDaySync.barrier("checkcompletion");
      while (!Game1.newDaySync.isBarrierReady("checkcompletion"))
        yield return 0;
      Game1.UpdateFarmPerfection();
      if (Game1.farmEvent == null)
      {
        Game1.handlePostFarmEventActions();
        Game1.showEndOfNightStuff();
      }
      if (Game1.server != null)
        Game1.server.updateLobbyData();
      divorceNPCs = (List<NPC>) null;
    }

    public virtual void SetOtherLocationWeatherForTomorrow(Random random)
    {
      Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Island).InitializeDayWeather();
      if (Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Island).weatherForTomorrow.Value == 1)
        Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Island).isRaining.Value = true;
      Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Island).weatherForTomorrow.Value = 0;
      if (random.NextDouble() < 0.24)
        Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Island).weatherForTomorrow.Value = 1;
      if (Utility.doesAnyFarmerHaveOrWillReceiveMail("Visited_Island"))
        return;
      Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Island).weatherForTomorrow.Value = 0;
    }

    public static void UpdateFarmPerfection()
    {
      if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") || !Game1.MasterPlayer.hasCompletedCommunityCenter() && !Utility.hasFinishedJojaRoute() || !Game1.player.team.farmPerfect.Value)
        return;
      Game1.addMorningFluffFunction((DelayedAction.delayedBehavior) (() =>
      {
        Game1.changeMusicTrack("none");
        if (Game1.IsMasterGame)
          Game1.multiplayer.globalChatInfoMessageEvenInSinglePlayer("Eternal1");
        Game1.playSound("discoverMineral");
        if (Game1.IsMasterGame)
          DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => Game1.multiplayer.globalChatInfoMessageEvenInSinglePlayer("Eternal2", (string) (NetFieldBase<string, NetString>) Game1.MasterPlayer.farmName)), 4000);
        Game1.player.mailReceived.Add("Farm_Eternal");
        DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
        {
          Game1.playSound("thunder_small");
          if (Game1.IsMultiplayer)
          {
            if (!Game1.IsMasterGame)
              return;
            Game1.multiplayer.globalChatInfoMessage("Eternal3");
          }
          else
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\UI:Chat_Eternal3"));
        }), 12000);
      }));
    }

    public static bool IsRainingHere(GameLocation location = null)
    {
      if ((NetFieldBase<IWorldState, NetRef<IWorldState>>) Game1.netWorldState == (NetRef<IWorldState>) null)
        return false;
      if (location == null)
        location = Game1.currentLocation;
      return location != null && (bool) (NetFieldBase<bool, NetBool>) Game1.netWorldState.Value.GetWeatherForLocation(location.GetLocationContext()).isRaining;
    }

    public static bool IsLightningHere(GameLocation location = null)
    {
      if ((NetFieldBase<IWorldState, NetRef<IWorldState>>) Game1.netWorldState == (NetRef<IWorldState>) null)
        return false;
      if (location == null)
        location = Game1.currentLocation;
      return location != null && (bool) (NetFieldBase<bool, NetBool>) Game1.netWorldState.Value.GetWeatherForLocation(location.GetLocationContext()).isLightning;
    }

    public static bool IsSnowingHere(GameLocation location = null)
    {
      if ((NetFieldBase<IWorldState, NetRef<IWorldState>>) Game1.netWorldState == (NetRef<IWorldState>) null)
        return false;
      if (location == null)
        location = Game1.currentLocation;
      return location != null && (bool) (NetFieldBase<bool, NetBool>) Game1.netWorldState.Value.GetWeatherForLocation(location.GetLocationContext()).isSnowing;
    }

    public static bool IsDebrisWeatherHere(GameLocation location = null)
    {
      if ((NetFieldBase<IWorldState, NetRef<IWorldState>>) Game1.netWorldState == (NetRef<IWorldState>) null)
        return false;
      if (location == null)
        location = Game1.currentLocation;
      return location != null && (bool) (NetFieldBase<bool, NetBool>) Game1.netWorldState.Value.GetWeatherForLocation(location.GetLocationContext()).isDebrisWeather;
    }

    public static int getWeatherModificationsForDate(WorldDate date, int default_weather)
    {
      int modificationsForDate = default_weather;
      int num = date.TotalDays - Game1.Date.TotalDays;
      if (date.DayOfMonth == 1 || (long) Game1.stats.DaysPlayed + (long) num <= 4L)
        modificationsForDate = 0;
      if ((long) Game1.stats.DaysPlayed + (long) num == 3L)
        modificationsForDate = 1;
      if (date.Season.Equals("summer") && date.DayOfMonth % 13 == 0)
        modificationsForDate = 3;
      if (Utility.isFestivalDay(date.DayOfMonth, date.Season))
        modificationsForDate = 4;
      if (date.Season.Equals("winter") && date.DayOfMonth >= 14 && date.DayOfMonth <= 16)
        modificationsForDate = 0;
      return modificationsForDate;
    }

    public static void UpdateShopPlayerItemInventory(
      string location_name,
      HashSet<NPC> purchased_item_npcs)
    {
      GameLocation locationFromName = Game1.getLocationFromName(location_name);
      if (locationFromName == null)
        return;
      ShopLocation shopLocation = locationFromName as ShopLocation;
      for (int index1 = shopLocation.itemsFromPlayerToSell.Count - 1; index1 >= 0; --index1)
      {
        for (int index2 = 0; index2 < shopLocation.itemsFromPlayerToSell[index1].Stack; ++index2)
        {
          if (Game1.random.NextDouble() < 0.04 && shopLocation.itemsFromPlayerToSell[index1] is Object && (int) (NetFieldBase<int, NetInt>) (shopLocation.itemsFromPlayerToSell[index1] as Object).edibility != -300)
          {
            NPC randomTownNpc = Utility.getRandomTownNPC();
            if (randomTownNpc.Age != 2 && randomTownNpc.getSpouse() == null)
            {
              if (!purchased_item_npcs.Contains(randomTownNpc))
              {
                randomTownNpc.addExtraDialogues(shopLocation.getPurchasedItemDialogueForNPC(shopLocation.itemsFromPlayerToSell[index1] as Object, randomTownNpc));
                purchased_item_npcs.Add(randomTownNpc);
              }
              --shopLocation.itemsFromPlayerToSell[index1].Stack;
            }
          }
          else if (Game1.random.NextDouble() < 0.15)
            --shopLocation.itemsFromPlayerToSell[index1].Stack;
          if (shopLocation.itemsFromPlayerToSell[index1].Stack <= 0)
          {
            shopLocation.itemsFromPlayerToSell.RemoveAt(index1);
            break;
          }
        }
      }
    }

    private static void handlePostFarmEventActions()
    {
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        if (location is BuildableGameLocation)
        {
          foreach (Building building in (location as BuildableGameLocation).buildings)
          {
            if (building.indoors.Value != null)
            {
              foreach (Action eventOvernightAction in building.indoors.Value.postFarmEventOvernightActions)
                eventOvernightAction();
              building.indoors.Value.postFarmEventOvernightActions.Clear();
            }
          }
        }
        foreach (Action eventOvernightAction in location.postFarmEventOvernightActions)
          eventOvernightAction();
        location.postFarmEventOvernightActions.Clear();
      }
      if (!Game1.IsMasterGame)
        return;
      Mountain locationFromName = Game1.getLocationFromName("Mountain") as Mountain;
      locationFromName.ApplyTreehouseIfNecessary();
      if (!locationFromName.treehouseDoorDirty)
        return;
      locationFromName.treehouseDoorDirty = false;
      NPC.populateRoutesFromLocationToLocationList();
    }

    public static void ReceiveMailForTomorrow(string mail_to_transfer = null)
    {
      foreach (string str1 in (NetList<string, NetString>) Game1.player.mailForTomorrow)
      {
        if (str1 != null)
        {
          string str2 = str1.Replace("%&NL&%", "");
          if (mail_to_transfer == null || !(mail_to_transfer != str1) || !(mail_to_transfer != str2))
          {
            Game1.mailDeliveredFromMailForTomorrow.Add(str1);
            if (str1.Contains("%&NL&%"))
            {
              if (!Game1.player.mailReceived.Contains(str2))
                Game1.player.mailReceived.Add(str2);
            }
            else
              Game1.mailbox.Add(str1);
          }
        }
      }
    }

    public static void RemoveDeliveredMailForTomorrow()
    {
      Game1.ReceiveMailForTomorrow("abandonedJojaMartAccessible");
      foreach (string str in Game1.mailDeliveredFromMailForTomorrow)
      {
        if (Game1.player.mailForTomorrow.Contains(str))
          Game1.player.mailForTomorrow.Remove(str);
      }
      Game1.mailDeliveredFromMailForTomorrow.Clear();
    }

    public static void queueWeddingsForToday()
    {
      Game1.weddingsToday.Clear();
      Game1.weddingToday = false;
      if (!Game1.canHaveWeddingOnDay(Game1.dayOfMonth, Game1.currentSeason))
        return;
      foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.getOnlineFarmers().OrderBy<Farmer, long>((Func<Farmer, long>) (farmer => farmer.UniqueMultiplayerID)))
      {
        if (farmer.spouse != null && farmer.isEngaged() && farmer.friendshipData[farmer.spouse].CountdownToWedding <= 1)
          Game1.weddingsToday.Add(farmer.UniqueMultiplayerID);
        if (farmer.team.IsEngaged(farmer.UniqueMultiplayerID))
        {
          long? spouse = farmer.team.GetSpouse(farmer.UniqueMultiplayerID);
          if (spouse.HasValue && !Game1.weddingsToday.Contains(spouse.Value))
          {
            Farmer farmerMaybeOffline = Game1.getFarmerMaybeOffline(spouse.Value);
            if (farmerMaybeOffline != null && Game1.getOnlineFarmers().Contains(farmerMaybeOffline) && Game1.getOnlineFarmers().Contains(farmer) && Game1.player.team.GetFriendship(farmer.UniqueMultiplayerID, spouse.Value).CountdownToWedding <= 1)
              Game1.weddingsToday.Add(farmer.UniqueMultiplayerID);
          }
        }
      }
    }

    public static bool PollForEndOfNewDaySync()
    {
      if (!Game1.IsMultiplayer)
      {
        Game1.newDaySync = (NewDaySynchronizer) null;
        Game1.currentLocation.resetForPlayerEntry();
        return true;
      }
      if (Game1.newDaySync.readyForFinish())
      {
        if (Game1.IsMasterGame && Game1.newDaySync != null && !Game1.newDaySync.hasFinished())
          Game1.newDaySync.finish();
        if (Game1.newDaySync != null && Game1.newDaySync.hasFinished())
        {
          Game1.newDaySync = (NewDaySynchronizer) null;
          Game1.currentLocation.resetForPlayerEntry();
          return true;
        }
      }
      return false;
    }

    public static void FinishNewDaySync()
    {
      if (Game1.IsMasterGame && Game1.newDaySync != null && !Game1.newDaySync.hasFinished())
        Game1.newDaySync.finish();
      Game1.newDaySync = (NewDaySynchronizer) null;
    }

    public static void updateWeatherIcon()
    {
      Game1.weatherIcon = !Game1.IsSnowingHere() ? (!Game1.IsRainingHere() ? (!Game1.IsDebrisWeatherHere() || !Game1.currentSeason.Equals("spring") ? (!Game1.IsDebrisWeatherHere() || !Game1.currentSeason.Equals("fall") ? (!Game1.IsDebrisWeatherHere() || !Game1.currentSeason.Equals("winter") ? (!Game1.weddingToday ? 2 : 0) : 7) : 6) : 3) : 4) : 7;
      if (Game1.IsLightningHere())
        Game1.weatherIcon = 5;
      if (!Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
        return;
      Game1.weatherIcon = 1;
    }

    public static void showEndOfNightStuff() => Game1.hooks.OnGame1_ShowEndOfNightStuff((Action) (() =>
    {
      bool flag1 = false;
      if (Game1.player.displayedShippedItems.Count > 0)
      {
        Game1.endOfNightMenus.Push((IClickableMenu) new ShippingMenu(Game1.player.displayedShippedItems));
        Game1.player.displayedShippedItems.Clear();
        flag1 = true;
      }
      bool flag2 = false;
      if (Game1.player.newLevels.Count > 0 && !flag1)
        Game1.endOfNightMenus.Push((IClickableMenu) new SaveGameMenu());
      for (int index = Game1.player.newLevels.Count - 1; index >= 0; --index)
      {
        int count = Game1.player.newLevels.Count;
        Game1.endOfNightMenus.Push((IClickableMenu) new LevelUpMenu(Game1.player.newLevels[index].X, Game1.player.newLevels[index].Y));
        flag2 = true;
      }
      if (flag2)
        Game1.playSound("newRecord");
      if (Game1.client != null && Game1.client.timedOut)
        return;
      if (Game1.endOfNightMenus.Count > 0)
      {
        Game1.showingEndOfNightStuff = true;
        Game1.activeClickableMenu = Game1.endOfNightMenus.Pop();
      }
      else
      {
        Game1.showingEndOfNightStuff = true;
        Game1.activeClickableMenu = (IClickableMenu) new SaveGameMenu();
      }
    }));

    private static void updateWallpaperInSeedShop()
    {
      GameLocation locationFromName = Game1.getLocationFromName("SeedShop");
      for (int x = 9; x < 12; ++x)
      {
        locationFromName.Map.GetLayer("Back").Tiles[x, 15] = (Tile) new StaticTile(locationFromName.Map.GetLayer("Back"), locationFromName.Map.GetTileSheet("Wallpapers"), BlendMode.Alpha, Game1.currentWallpaper);
        locationFromName.Map.GetLayer("Back").Tiles[x, 16] = (Tile) new StaticTile(locationFromName.Map.GetLayer("Back"), locationFromName.Map.GetTileSheet("Wallpapers"), BlendMode.Alpha, Game1.currentWallpaper);
      }
    }

    public static void setGraphicsForSeason()
    {
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        string seasonForLocation = Game1.GetSeasonForLocation(location);
        location.seasonUpdate(seasonForLocation, true);
        location.updateSeasonalTileSheets();
        if (location.IsOutdoors)
        {
          if (seasonForLocation.Equals("spring"))
          {
            foreach (KeyValuePair<Vector2, Object> pair in location.Objects.Pairs)
            {
              if ((pair.Value.Name.Contains("Stump") || pair.Value.Name.Contains("Boulder") || pair.Value.Name.Equals("Stick") || pair.Value.Name.Equals("Stone")) && pair.Value.ParentSheetIndex >= 378 && pair.Value.ParentSheetIndex <= 391)
                pair.Value.ParentSheetIndex -= 376;
            }
            Game1.eveningColor = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, 0);
          }
          else if (seasonForLocation.Equals("summer"))
          {
            foreach (KeyValuePair<Vector2, Object> pair in location.Objects.Pairs)
            {
              if (pair.Value.Name.Contains("Weed") && pair.Value.ParentSheetIndex != 882 && pair.Value.ParentSheetIndex != 883 && pair.Value.ParentSheetIndex != 884)
              {
                if ((int) (NetFieldBase<int, NetInt>) pair.Value.parentSheetIndex == 792)
                  ++pair.Value.ParentSheetIndex;
                else if (Game1.random.NextDouble() < 0.3)
                  pair.Value.ParentSheetIndex = 676;
                else if (Game1.random.NextDouble() < 0.3)
                  pair.Value.ParentSheetIndex = 677;
              }
            }
            Game1.eveningColor = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, 0);
          }
          else if (seasonForLocation.Equals("fall"))
          {
            foreach (KeyValuePair<Vector2, Object> pair in location.Objects.Pairs)
            {
              if (pair.Value.Name.Contains("Weed") && pair.Value.ParentSheetIndex != 882 && pair.Value.ParentSheetIndex != 883 && pair.Value.ParentSheetIndex != 884)
              {
                if ((int) (NetFieldBase<int, NetInt>) pair.Value.parentSheetIndex == 793)
                  ++pair.Value.ParentSheetIndex;
                else if (Game1.random.NextDouble() < 0.5)
                  pair.Value.ParentSheetIndex = 678;
                else
                  pair.Value.ParentSheetIndex = 679;
              }
            }
            Game1.eveningColor = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, 0);
            foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
              weatherDebris.which = 2;
          }
          else if (seasonForLocation.Equals("winter"))
          {
            for (int index = location.Objects.Count() - 1; index >= 0; --index)
            {
              Object @object = location.Objects[location.Objects.Keys.ElementAt<Vector2>(index)];
              if (@object.Name.Contains("Weed"))
              {
                if (@object.ParentSheetIndex != 882 && @object.ParentSheetIndex != 883 && @object.ParentSheetIndex != 884)
                  location.Objects.Remove(location.Objects.Keys.ElementAt<Vector2>(index));
              }
              else if ((!@object.Name.Contains("Stump") && !@object.Name.Contains("Boulder") && !@object.Name.Equals("Stick") && !@object.Name.Equals("Stone") || @object.ParentSheetIndex > 100) && location.IsOutdoors && !(bool) (NetFieldBase<bool, NetBool>) @object.isHoedirt)
                @object.name.Equals("HoeDirt");
            }
            foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
              weatherDebris.which = 3;
            Game1.eveningColor = new Microsoft.Xna.Framework.Color(245, 225, 170);
          }
        }
      }
    }

    private static void updateFloorInSeedShop()
    {
      GameLocation locationFromName = Game1.getLocationFromName("SeedShop");
      for (int x = 9; x < 12; ++x)
      {
        locationFromName.Map.GetLayer("Back").Tiles[x, 17] = (Tile) new StaticTile(locationFromName.Map.GetLayer("Back"), locationFromName.Map.GetTileSheet("Floors"), BlendMode.Alpha, Game1.currentFloor);
        locationFromName.Map.GetLayer("Back").Tiles[x, 18] = (Tile) new StaticTile(locationFromName.Map.GetLayer("Back"), locationFromName.Map.GetTileSheet("Floors"), BlendMode.Alpha, Game1.currentFloor);
      }
    }

    public static void pauseThenMessage(
      int millisecondsPause,
      string message,
      bool showProgressBar)
    {
      Game1.messageAfterPause = message;
      Game1.pauseTime = (float) millisecondsPause;
      Game1.progressBar = showProgressBar;
    }

    public static void updateWallpaperInFarmHouse(int wallpaper)
    {
      GameLocation locationFromName = Game1.getLocationFromName("FarmHouse");
      PropertyValue propertyValue;
      locationFromName.Map.Properties.TryGetValue("Wallpaper", out propertyValue);
      if (propertyValue == null)
        return;
      string[] strArray = propertyValue.ToString().Split(' ');
      for (int index = 0; index < strArray.Length; index += 4)
      {
        int int32_1 = Convert.ToInt32(strArray[index]);
        int int32_2 = Convert.ToInt32(strArray[index + 1]);
        int int32_3 = Convert.ToInt32(strArray[index + 2]);
        int int32_4 = Convert.ToInt32(strArray[index + 3]);
        for (int x = int32_1; x < int32_1 + int32_3; ++x)
        {
          for (int y = int32_2; y < int32_2 + int32_4; ++y)
            locationFromName.Map.GetLayer("Back").Tiles[x, y] = (Tile) new StaticTile(locationFromName.Map.GetLayer("Back"), locationFromName.Map.GetTileSheet("Wallpapers"), BlendMode.Alpha, wallpaper);
        }
      }
    }

    public static void updateFloorInFarmHouse(int floor)
    {
      GameLocation locationFromName = Game1.getLocationFromName("FarmHouse");
      PropertyValue propertyValue;
      locationFromName.Map.Properties.TryGetValue("Floor", out propertyValue);
      if (propertyValue == null)
        return;
      string[] strArray = propertyValue.ToString().Split(' ');
      for (int index = 0; index < strArray.Length; index += 4)
      {
        int int32_1 = Convert.ToInt32(strArray[index]);
        int int32_2 = Convert.ToInt32(strArray[index + 1]);
        int int32_3 = Convert.ToInt32(strArray[index + 2]);
        int int32_4 = Convert.ToInt32(strArray[index + 3]);
        for (int x = int32_1; x < int32_1 + int32_3; ++x)
        {
          for (int y = int32_2; y < int32_2 + int32_4; ++y)
            locationFromName.Map.GetLayer("Back").Tiles[x, y] = (Tile) new StaticTile(locationFromName.Map.GetLayer("Back"), locationFromName.Map.GetTileSheet("Floors"), BlendMode.Alpha, floor);
        }
      }
    }

    public static bool IsVisitingIslandToday(string npc_name) => Game1.netWorldState.Value.IslandVisitors.ContainsKey(npc_name);

    public static bool shouldTimePass(bool ignore_multiplayer = false)
    {
      if (Game1.isFestival() || Game1.CurrentEvent != null && Game1.CurrentEvent.isWedding || Game1.farmEvent != null)
        return false;
      if (Game1.IsMultiplayer && !ignore_multiplayer)
        return !Game1.netWorldState.Value.IsTimePaused;
      if (Game1.paused || Game1.freezeControls || Game1.overlayMenu != null || Game1.isTimePaused || Game1.eventUp)
        return false;
      switch (Game1.activeClickableMenu)
      {
        case null:
        case BobberBar _:
          return Game1.player.CanMove || Game1.player.UsingTool || Game1.player.forceTimePass;
        default:
          return false;
      }
    }

    public static Farmer getPlayerOrEventFarmer() => Game1.eventUp && Game1.CurrentEvent != null && !Game1.CurrentEvent.isFestival && Game1.CurrentEvent.farmer != null ? Game1.CurrentEvent.farmer : Game1.player;

    public static void UpdateViewPort(bool overrideFreeze, Point centerPoint)
    {
      Game1.previousViewportPosition.X = (float) Game1.viewport.X;
      Game1.previousViewportPosition.Y = (float) Game1.viewport.Y;
      Farmer playerOrEventFarmer = Game1.getPlayerOrEventFarmer();
      if (Game1.currentLocation == null)
        return;
      if (!Game1.viewportFreeze | overrideFreeze)
      {
        bool flag = (double) Math.Abs(Game1.currentViewportTarget.X + (float) (Game1.viewport.Width / 2) - (float) playerOrEventFarmer.getStandingX()) > 64.0 || (double) Math.Abs(Game1.currentViewportTarget.Y + (float) (Game1.viewport.Height / 2) - (float) playerOrEventFarmer.getStandingY()) > 64.0;
        if (Game1.forceSnapOnNextViewportUpdate)
          flag = true;
        if (centerPoint.X >= Game1.viewport.Width / 2 && centerPoint.X <= Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width / 2)
        {
          if (playerOrEventFarmer.isRafting | flag)
            Game1.currentViewportTarget.X = (float) (centerPoint.X - Game1.viewport.Width / 2);
          else if ((double) Math.Abs(Game1.currentViewportTarget.X - (Game1.currentViewportTarget.X = (float) (centerPoint.X - Game1.viewport.Width / 2))) > (double) playerOrEventFarmer.getMovementSpeed())
            Game1.currentViewportTarget.X += (float) Math.Sign(Game1.currentViewportTarget.X - (Game1.currentViewportTarget.X = (float) (centerPoint.X - Game1.viewport.Width / 2))) * playerOrEventFarmer.getMovementSpeed();
        }
        else if (centerPoint.X < Game1.viewport.Width / 2 && Game1.viewport.Width <= Game1.currentLocation.Map.DisplayWidth)
        {
          if (playerOrEventFarmer.isRafting | flag)
            Game1.currentViewportTarget.X = 0.0f;
          else if ((double) Math.Abs(Game1.currentViewportTarget.X - 0.0f) > (double) playerOrEventFarmer.getMovementSpeed())
            Game1.currentViewportTarget.X -= (float) Math.Sign(Game1.currentViewportTarget.X - 0.0f) * playerOrEventFarmer.getMovementSpeed();
        }
        else if (Game1.viewport.Width <= Game1.currentLocation.Map.DisplayWidth)
        {
          if (playerOrEventFarmer.isRafting | flag)
            Game1.currentViewportTarget.X = (float) (Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width);
          else if ((double) Math.Abs(Game1.currentViewportTarget.X - (float) (Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width)) > (double) playerOrEventFarmer.getMovementSpeed())
            Game1.currentViewportTarget.X += (float) Math.Sign(Game1.currentViewportTarget.X - (float) (Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width)) * playerOrEventFarmer.getMovementSpeed();
        }
        else if (Game1.currentLocation.Map.DisplayWidth < Game1.viewport.Width)
        {
          if (playerOrEventFarmer.isRafting | flag)
            Game1.currentViewportTarget.X = (float) ((Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width) / 2);
          else if ((double) Math.Abs(Game1.currentViewportTarget.X - (float) ((Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width) / 2)) > (double) playerOrEventFarmer.getMovementSpeed())
            Game1.currentViewportTarget.X -= (float) Math.Sign(Game1.currentViewportTarget.X - (float) ((Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width) / 2)) * playerOrEventFarmer.getMovementSpeed();
        }
        if (centerPoint.Y >= Game1.viewport.Height / 2 && centerPoint.Y <= Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height / 2)
        {
          if (playerOrEventFarmer.isRafting | flag)
            Game1.currentViewportTarget.Y = (float) (centerPoint.Y - Game1.viewport.Height / 2);
          else if ((double) Math.Abs(Game1.currentViewportTarget.Y - (float) (centerPoint.Y - Game1.viewport.Height / 2)) >= (double) playerOrEventFarmer.getMovementSpeed())
            Game1.currentViewportTarget.Y -= (float) Math.Sign(Game1.currentViewportTarget.Y - (float) (centerPoint.Y - Game1.viewport.Height / 2)) * playerOrEventFarmer.getMovementSpeed();
        }
        else if (centerPoint.Y < Game1.viewport.Height / 2 && Game1.viewport.Height <= Game1.currentLocation.Map.DisplayHeight)
        {
          if (playerOrEventFarmer.isRafting | flag)
            Game1.currentViewportTarget.Y = 0.0f;
          else if ((double) Math.Abs(Game1.currentViewportTarget.Y - 0.0f) > (double) playerOrEventFarmer.getMovementSpeed())
            Game1.currentViewportTarget.Y -= (float) Math.Sign(Game1.currentViewportTarget.Y - 0.0f) * playerOrEventFarmer.getMovementSpeed();
          Game1.currentViewportTarget.Y = 0.0f;
        }
        else if (Game1.viewport.Height <= Game1.currentLocation.Map.DisplayHeight)
        {
          if (playerOrEventFarmer.isRafting | flag)
            Game1.currentViewportTarget.Y = (float) (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height);
          else if ((double) Math.Abs(Game1.currentViewportTarget.Y - (float) (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height)) > (double) playerOrEventFarmer.getMovementSpeed())
            Game1.currentViewportTarget.Y -= (float) Math.Sign(Game1.currentViewportTarget.Y - (float) (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height)) * playerOrEventFarmer.getMovementSpeed();
        }
        else if (Game1.currentLocation.Map.DisplayHeight < Game1.viewport.Height)
        {
          if (playerOrEventFarmer.isRafting | flag)
            Game1.currentViewportTarget.Y = (float) ((Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2);
          else if ((double) Math.Abs(Game1.currentViewportTarget.Y - (float) ((Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2)) > (double) playerOrEventFarmer.getMovementSpeed())
            Game1.currentViewportTarget.Y -= (float) Math.Sign(Game1.currentViewportTarget.Y - (float) ((Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2)) * playerOrEventFarmer.getMovementSpeed();
        }
      }
      if (Game1.currentLocation.forceViewportPlayerFollow)
      {
        Game1.currentViewportTarget.X = playerOrEventFarmer.Position.X - (float) (Game1.viewport.Width / 2);
        Game1.currentViewportTarget.Y = playerOrEventFarmer.Position.Y - (float) (Game1.viewport.Height / 2);
      }
      bool flag1 = false;
      if (Game1.forceSnapOnNextViewportUpdate)
      {
        flag1 = true;
        Game1.forceSnapOnNextViewportUpdate = false;
      }
      if ((double) Game1.currentViewportTarget.X == (double) int.MinValue || Game1.viewportFreeze && !overrideFreeze)
        return;
      int num1 = (int) ((double) Game1.currentViewportTarget.X - (double) Game1.viewport.X);
      if (Math.Abs(num1) > 128)
        Game1.viewportPositionLerp.X = Game1.currentViewportTarget.X;
      else
        Game1.viewportPositionLerp.X += (float) ((double) num1 * (double) playerOrEventFarmer.getMovementSpeed() * 0.0299999993294477);
      int num2 = (int) ((double) Game1.currentViewportTarget.Y - (double) Game1.viewport.Y);
      if (Math.Abs(num2) > 128)
        Game1.viewportPositionLerp.Y = (float) (int) Game1.currentViewportTarget.Y;
      else
        Game1.viewportPositionLerp.Y += (float) ((double) num2 * (double) playerOrEventFarmer.getMovementSpeed() * 0.0299999993294477);
      if (flag1)
      {
        Game1.viewportPositionLerp.X = (float) (int) Game1.currentViewportTarget.X;
        Game1.viewportPositionLerp.Y = (float) (int) Game1.currentViewportTarget.Y;
      }
      Game1.viewport.X = (int) Game1.viewportPositionLerp.X;
      Game1.viewport.Y = (int) Game1.viewportPositionLerp.Y;
    }

    private void UpdateCharacters(GameTime time)
    {
      if (Game1.CurrentEvent != null && Game1.CurrentEvent.farmer != null && Game1.CurrentEvent.farmer != Game1.player)
        Game1.CurrentEvent.farmer.Update(time, Game1.currentLocation);
      Game1.player.Update(time, Game1.currentLocation);
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      {
        if (otherFarmer.Key != Game1.player.UniqueMultiplayerID)
          otherFarmer.Value.UpdateIfOtherPlayer(time);
      }
    }

    public static void addMail(string mailName, bool noLetter = false, bool sendToEveryone = false)
    {
      if (sendToEveryone)
      {
        Game1.multiplayer.broadcastPartyWideMail(mailName, Multiplayer.PartyWideMessageQueue.SeenMail, noLetter);
      }
      else
      {
        mailName = mailName.Trim();
        mailName = mailName.Replace(Environment.NewLine, "");
        if (Game1.player.hasOrWillReceiveMail(mailName))
          return;
        if (noLetter)
          Game1.player.mailReceived.Add(mailName);
        else
          Game1.player.mailbox.Add(mailName);
      }
    }

    public static void addMailForTomorrow(string mailName, bool noLetter = false, bool sendToEveryone = false)
    {
      if (sendToEveryone)
      {
        Game1.multiplayer.broadcastPartyWideMail(mailName, no_letter: noLetter);
      }
      else
      {
        mailName = mailName.Trim();
        mailName = mailName.Replace(Environment.NewLine, "");
        if (Game1.player.hasOrWillReceiveMail(mailName))
          return;
        if (noLetter)
          mailName += "%&NL&%";
        Game1.player.mailForTomorrow.Add(mailName);
        if (!sendToEveryone || !Game1.IsMultiplayer)
          return;
        foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
        {
          if (farmer != Game1.player && !Game1.player.hasOrWillReceiveMail(mailName))
            farmer.mailForTomorrow.Add(mailName);
        }
      }
    }

    public static void drawDialogue(NPC speaker)
    {
      if (speaker.CurrentDialogue.Count == 0)
        return;
      Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(speaker.CurrentDialogue.Peek());
      Game1.dialogueUp = true;
      if (!Game1.eventUp)
      {
        Game1.player.Halt();
        Game1.player.CanMove = false;
      }
      if (speaker == null)
        return;
      Game1.currentSpeaker = speaker;
    }

    public static void drawDialogueNoTyping(NPC speaker, string dialogue)
    {
      if (speaker == null)
        Game1.currentObjectDialogue.Enqueue(dialogue);
      else if (dialogue != null)
        speaker.CurrentDialogue.Push(new Dialogue(dialogue, speaker));
      Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(speaker.CurrentDialogue.Peek());
      Game1.dialogueUp = true;
      Game1.player.CanMove = false;
      if (speaker == null)
        return;
      Game1.currentSpeaker = speaker;
    }

    public static void multipleDialogues(string[] messages)
    {
      Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(((IEnumerable<string>) messages).ToList<string>());
      Game1.dialogueUp = true;
      Game1.player.CanMove = false;
    }

    public static void drawDialogueNoTyping(string dialogue)
    {
      Game1.drawObjectDialogue(dialogue);
      if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is DialogueBox))
        return;
      (Game1.activeClickableMenu as DialogueBox).finishTyping();
    }

    public static void drawDialogue(NPC speaker, string dialogue)
    {
      speaker.CurrentDialogue.Push(new Dialogue(dialogue, speaker));
      Game1.drawDialogue(speaker);
    }

    public static void drawDialogue(NPC speaker, string dialogue, Texture2D overridePortrait)
    {
      speaker.CurrentDialogue.Push(new Dialogue(dialogue, speaker)
      {
        overridePortrait = overridePortrait
      });
      Game1.drawDialogue(speaker);
    }

    public static void drawItemNumberSelection(string itemType, int price)
    {
      Game1.selectedItemsType = itemType;
      Game1.numberOfSelectedItems = 0;
      Game1.priceOfSelectedItem = price;
      if (itemType.Equals("calicoJackBet"))
        Game1.currentObjectDialogue.Enqueue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2946", (object) Game1.player.clubCoins));
      else if (itemType.Equals("flutePitch"))
      {
        Game1.currentObjectDialogue.Enqueue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2949"));
        Game1.numberOfSelectedItems = (int) Game1.currentLocation.actionObjectForQuestionDialogue.scale.X / 100;
      }
      else if (itemType.Equals("drumTone"))
      {
        Game1.currentObjectDialogue.Enqueue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2951"));
        Game1.numberOfSelectedItems = (int) Game1.currentLocation.actionObjectForQuestionDialogue.scale.X;
      }
      else if (itemType.Equals("jukebox"))
        Game1.currentObjectDialogue.Enqueue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2953"));
      else if (itemType.Equals("Fuel"))
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2955"));
      else if (Game1.currentSpeaker != null)
        Game1.setDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2956"), false);
      else
        Game1.currentObjectDialogue.Enqueue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2957"));
    }

    public static void setDialogue(string dialogue, bool typing)
    {
      if (Game1.currentSpeaker != null)
      {
        Game1.currentSpeaker.CurrentDialogue.Peek().setCurrentDialogue(dialogue);
        if (typing)
          Game1.drawDialogue(Game1.currentSpeaker);
        else
          Game1.drawDialogueNoTyping(Game1.currentSpeaker, (string) null);
      }
      else if (typing)
        Game1.drawObjectDialogue(dialogue);
      else
        Game1.drawDialogueNoTyping(dialogue);
    }

    private static void checkIfDialogueIsQuestion()
    {
      if (Game1.currentSpeaker == null || Game1.currentSpeaker.CurrentDialogue.Count <= 0 || !Game1.currentSpeaker.CurrentDialogue.Peek().isCurrentDialogueAQuestion())
        return;
      Game1.questionChoices.Clear();
      Game1.isQuestion = true;
      List<NPCDialogueResponse> npcResponseOptions = Game1.currentSpeaker.CurrentDialogue.Peek().getNPCResponseOptions();
      for (int index = 0; index < npcResponseOptions.Count; ++index)
        Game1.questionChoices.Add((Response) npcResponseOptions[index]);
    }

    public static void drawLetterMessage(string message) => Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(message);

    public static void drawObjectDialogue(string dialogue)
    {
      if (Game1.activeClickableMenu != null)
        Game1.activeClickableMenu.emergencyShutDown();
      Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(dialogue);
      Game1.player.CanMove = false;
      Game1.dialogueUp = true;
    }

    public static void drawObjectQuestionDialogue(
      string dialogue,
      List<Response> choices,
      int width)
    {
      Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(dialogue, choices, width);
      Game1.dialogueUp = true;
      Game1.player.CanMove = false;
    }

    public static void drawObjectQuestionDialogue(string dialogue, List<Response> choices)
    {
      Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(dialogue, choices);
      Game1.dialogueUp = true;
      Game1.player.CanMove = false;
    }

    public static bool IsSummer => Game1.currentSeason.Equals("summer");

    public static bool IsSpring => Game1.currentSeason.Equals("spring");

    public static bool IsFall => Game1.currentSeason.Equals("fall");

    public static bool IsWinter => Game1.currentSeason.Equals("winter");

    public static void removeThisCharacterFromAllLocations(NPC toDelete)
    {
      for (int index = 0; index < Game1.locations.Count; ++index)
      {
        if (Game1.locations[index].characters.Contains(toDelete))
          Game1.locations[index].characters.Remove(toDelete);
      }
    }

    public static void warpCharacter(NPC character, string targetLocationName, Point position) => Game1.warpCharacter(character, targetLocationName, new Vector2((float) position.X, (float) position.Y));

    public static void warpCharacter(NPC character, string targetLocationName, Vector2 position) => Game1.warpCharacter(character, Game1.getLocationFromName(targetLocationName), position);

    public static void warpCharacter(NPC character, GameLocation targetLocation, Vector2 position)
    {
      if (character.currentLocation == null)
        throw new ArgumentException("In warpCharacter, the character's currentLocation must not be null");
      if (Game1.currentSeason.Equals("winter") && Game1.dayOfMonth >= 15 && Game1.dayOfMonth <= 17 && targetLocation.name.Equals((object) "Beach"))
        targetLocation = Game1.getLocationFromName("BeachNightMarket");
      if (targetLocation.name.Equals((object) "Trailer") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
      {
        targetLocation = Game1.getLocationFromName("Trailer_Big");
        if ((double) position.X == 12.0 && (double) position.Y == 9.0)
        {
          position.X = 13f;
          position.Y = 24f;
        }
      }
      if (Game1.IsClient)
      {
        Game1.multiplayer.requestCharacterWarp(character, targetLocation, position);
      }
      else
      {
        if (!targetLocation.characters.Contains(character))
        {
          character.currentLocation.characters.Remove(character);
          targetLocation.addCharacter(character);
        }
        character.isCharging = false;
        character.speed = 2;
        character.blockedInterval = 0;
        string str = NPC.getTextureNameForCharacter(character.Name);
        bool flag = false;
        if (character.isVillager())
        {
          if (character.Name.Equals("Maru"))
          {
            if (targetLocation.Name.Equals("Hospital"))
            {
              str = character.Name + "_" + targetLocation.Name;
              flag = true;
            }
            else if (!targetLocation.Name.Equals("Hospital") && character.Sprite.textureName.Value != character.Name)
            {
              str = character.Name;
              flag = true;
            }
          }
          else if (character.Name.Equals("Shane"))
          {
            if (targetLocation.Name.Equals("JojaMart"))
            {
              str = character.Name + "_" + targetLocation.Name;
              flag = true;
            }
            else if (!targetLocation.Name.Equals("JojaMart") && character.Sprite.textureName.Value != character.Name)
            {
              str = character.Name;
              flag = true;
            }
          }
        }
        if (flag)
          character.Sprite.LoadTexture("Characters\\" + str);
        character.position.X = position.X * 64f;
        character.position.Y = position.Y * 64f;
        if (character.CurrentDialogue.Count > 0 && character.CurrentDialogue.Peek().removeOnNextMove && !character.getTileLocation().Equals(character.DefaultPosition / 64f))
          character.CurrentDialogue.Pop();
        if (targetLocation is FarmHouse)
          character.arriveAtFarmHouse(targetLocation as FarmHouse);
        else
          character.arriveAt(targetLocation);
        if (character.currentLocation != null && !character.currentLocation.Equals(targetLocation))
          character.currentLocation.characters.Remove(character);
        character.currentLocation = targetLocation;
      }
    }

    public static LocationRequest getLocationRequest(
      string locationName,
      bool isStructure = false)
    {
      return locationName != null ? new LocationRequest(locationName, isStructure, Game1.getLocationFromName(locationName, isStructure)) : throw new ArgumentException();
    }

    public static void warpHome()
    {
      LocationRequest locationRequest = Game1.getLocationRequest(Game1.player.homeLocation.Value);
      locationRequest.OnWarp += (LocationRequest.Callback) (() => Game1.player.position.Set(Utility.PointToVector2((Game1.currentLocation as FarmHouse).GetPlayerBedSpot()) * 64f));
      Game1.warpFarmer(locationRequest, 5, 9, Game1.player.FacingDirection);
    }

    public static void warpFarmer(string locationName, int tileX, int tileY, bool flip) => Game1.warpFarmer(Game1.getLocationRequest(locationName), tileX, tileY, flip ? (Game1.player.FacingDirection + 2) % 4 : Game1.player.FacingDirection);

    public static void warpFarmer(
      string locationName,
      int tileX,
      int tileY,
      int facingDirectionAfterWarp)
    {
      Game1.warpFarmer(Game1.getLocationRequest(locationName), tileX, tileY, facingDirectionAfterWarp);
    }

    public static void warpFarmer(
      string locationName,
      int tileX,
      int tileY,
      int facingDirectionAfterWarp,
      bool isStructure)
    {
      Game1.warpFarmer(Game1.getLocationRequest(locationName, isStructure), tileX, tileY, facingDirectionAfterWarp);
    }

    public virtual bool ShouldDismountOnWarp(
      Horse mount,
      GameLocation old_location,
      GameLocation new_location)
    {
      return mount != null && Game1.currentLocation != null && Game1.currentLocation.IsOutdoors && new_location != null && !new_location.IsOutdoors;
    }

    public static void warpFarmer(
      LocationRequest locationRequest,
      int tileX,
      int tileY,
      int facingDirectionAfterWarp)
    {
      int warp_offset_x = Game1.nextFarmerWarpOffsetX;
      int warp_offset_y = Game1.nextFarmerWarpOffsetY;
      Game1.nextFarmerWarpOffsetX = 0;
      Game1.nextFarmerWarpOffsetY = 0;
      if (locationRequest.Name.Equals("Beach") && Game1.currentSeason.Equals("winter") && Game1.dayOfMonth >= 15 && Game1.dayOfMonth <= 17 && !Game1.eventUp)
        locationRequest = Game1.getLocationRequest("BeachNightMarket");
      if (locationRequest.Name.Equals("Farm") && Game1.currentLocation.NameOrUniqueName == "Greenhouse")
      {
        bool flag = false;
        foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) Game1.currentLocation.warps)
        {
          if (warp.TargetX == tileX && warp.TargetY == tileY)
          {
            flag = true;
            break;
          }
        }
        if (flag)
        {
          Building building1 = (Building) null;
          foreach (Building building2 in Game1.getFarm().buildings)
          {
            if (building2 is GreenhouseBuilding)
            {
              building1 = building2;
              break;
            }
          }
          if (building1 != null)
          {
            tileX = building1.getPointForHumanDoor().X;
            tileY = building1.getPointForHumanDoor().Y + 1;
          }
        }
      }
      if (locationRequest.Name == "IslandSouth" && tileX <= 15 && tileY <= 6)
      {
        tileX = 21;
        tileY = 43;
      }
      if (locationRequest.Name.StartsWith("VolcanoDungeon"))
      {
        warp_offset_x = 0;
        warp_offset_y = 0;
      }
      if (Game1.player.isRidingHorse() && Game1.currentLocation != null)
      {
        GameLocation new_location = locationRequest.Location ?? Game1.getLocationFromName(locationRequest.Name);
        if (Game1.game1.ShouldDismountOnWarp(Game1.player.mount, Game1.currentLocation, new_location))
        {
          Game1.player.mount.dismount();
          warp_offset_x = 0;
          warp_offset_y = 0;
        }
      }
      if (locationRequest.Name.Equals("Trailer") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
      {
        locationRequest = Game1.getLocationRequest("Trailer_Big");
        tileX = 13;
        tileY = 24;
      }
      if (locationRequest.Name.Equals("Farm"))
      {
        Farm farm = Game1.getFarm();
        if (Game1.currentLocation.NameOrUniqueName == "FarmCave" && tileX == 34 && tileY == 6)
        {
          switch (Game1.whichFarm)
          {
            case 5:
              tileX = 30;
              tileY = 36;
              break;
            case 6:
              tileX = 34;
              tileY = 16;
              break;
          }
          Point propertyPosition = farm.GetMapPropertyPosition("FarmCaveEntry", tileX, tileY);
          tileX = propertyPosition.X;
          tileY = propertyPosition.Y;
        }
        else if (Game1.currentLocation.NameOrUniqueName == "Forest" && tileX == 41 && tileY == 64)
        {
          switch (Game1.whichFarm)
          {
            case 5:
              tileX = 40;
              tileY = 64;
              break;
            case 6:
              tileX = 82;
              tileY = 103;
              break;
          }
          Point propertyPosition = farm.GetMapPropertyPosition("ForestEntry", tileX, tileY);
          tileX = propertyPosition.X;
          tileY = propertyPosition.Y;
        }
        else if (Game1.currentLocation.NameOrUniqueName == "BusStop" && tileX == 79 && tileY == 17)
        {
          Point propertyPosition = farm.GetMapPropertyPosition("BusStopEntry", tileX, tileY);
          tileX = propertyPosition.X;
          tileY = propertyPosition.Y;
        }
        else if (Game1.currentLocation.NameOrUniqueName == "Backwoods" && tileX == 40 && tileY == 0)
        {
          Point propertyPosition = farm.GetMapPropertyPosition("BackwoodsEntry", tileX, tileY);
          tileX = propertyPosition.X;
          tileY = propertyPosition.Y;
        }
        else if (Game1.currentLocation.NameOrUniqueName == "FarmHouse" && tileX == 64 && tileY == 15)
        {
          Point mainFarmHouseEntry = farm.GetMainFarmHouseEntry();
          tileX = mainFarmHouseEntry.X;
          tileY = mainFarmHouseEntry.Y;
        }
      }
      if (locationRequest.Name.Equals("Club") && !Game1.player.hasClubCard)
      {
        locationRequest = Game1.getLocationRequest("SandyHouse");
        locationRequest.OnWarp += (LocationRequest.Callback) (() =>
        {
          NPC characterFromName = Game1.currentLocation.getCharacterFromName("Bouncer");
          if (characterFromName == null)
            return;
          Vector2 vector2 = new Vector2(17f, 4f);
          characterFromName.showTextAboveHead(Game1.content.LoadString("Strings\\Locations:Club_Bouncer_TextAboveHead" + (Game1.random.Next(2) + 1).ToString()));
          int num = Game1.random.Next();
          Game1.currentLocation.playSound("thudStep");
          Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(288, 100f, 1, 24, vector2 * 64f, true, false, Game1.currentLocation, Game1.player)
          {
            shakeIntensity = 0.5f,
            shakeIntensityChange = 1f / 500f,
            extraInfoForEndBehavior = num,
            endFunction = new TemporaryAnimatedSprite.endBehavior(Game1.currentLocation.removeTemporarySpritesWithID)
          }, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector2 * 64f + new Vector2(5f, 0.0f) * 4f, true, false, 0.0263f, 0.0f, Microsoft.Xna.Framework.Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = (float) num
          }, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector2 * 64f + new Vector2(5f, 0.0f) * 4f, true, true, 0.0263f, 0.0f, Microsoft.Xna.Framework.Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
          {
            delayBeforeAnimationStart = 100,
            id = (float) num
          }, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector2 * 64f + new Vector2(5f, 0.0f) * 4f, true, false, 0.0263f, 0.0f, Microsoft.Xna.Framework.Color.White, 3f, 0.0f, 0.0f, 0.0f)
          {
            delayBeforeAnimationStart = 200,
            id = (float) num
          });
          Game1.currentLocation.netAudio.StartPlaying("fuse");
        });
        tileX = 17;
        tileY = 4;
      }
      if (Game1.weatherIcon == 1 && Game1.whereIsTodaysFest != null && locationRequest.Name.Equals(Game1.whereIsTodaysFest) && !Game1.warpingForForcedRemoteEvent && Game1.timeOfDay <= Convert.ToInt32(Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + Game1.dayOfMonth.ToString())["conditions"].Split('/')[1].Split(' ')[1]))
      {
        if (Game1.timeOfDay < Convert.ToInt32(Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + Game1.dayOfMonth.ToString())["conditions"].Split('/')[1].Split(' ')[0]))
        {
          if (Game1.currentLocation.Name.Equals("Hospital"))
          {
            locationRequest = Game1.getLocationRequest("BusStop");
            tileX = 34;
            tileY = 23;
          }
          else
          {
            Game1.player.Position = Game1.player.lastPosition;
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2973"));
            return;
          }
        }
        else
        {
          if (Game1.IsMultiplayer)
          {
            Game1.player.team.SetLocalReady("festivalStart", true);
            Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior) (who =>
            {
              Game1.exitActiveMenu();
              if (Game1.player.mount != null)
              {
                Game1.player.mount.dismount();
                warp_offset_x = 0;
                warp_offset_y = 0;
              }
              Game1.performWarpFarmer(locationRequest, tileX, tileY, facingDirectionAfterWarp);
            }));
            return;
          }
          if (Game1.player.mount != null)
          {
            Game1.player.mount.dismount();
            warp_offset_x = 0;
            warp_offset_y = 0;
          }
        }
      }
      tileX += warp_offset_x;
      tileY += warp_offset_y;
      Game1.performWarpFarmer(locationRequest, tileX, tileY, facingDirectionAfterWarp);
    }

    private static void performWarpFarmer(
      LocationRequest locationRequest,
      int tileX,
      int tileY,
      int facingDirectionAfterWarp)
    {
      if ((Game1.currentLocation.Name.Equals("Town") || Game1.jukeboxPlaying) && Game1.getLocationFromName(locationRequest.Name).IsOutdoors && Game1.currentSong != null && (Game1.currentSong.Name.Contains("town") || Game1.jukeboxPlaying))
        Game1.changeMusicTrack("none");
      if (locationRequest.Location != null)
      {
        if (tileX >= locationRequest.Location.Map.Layers[0].LayerWidth - 1)
          --tileX;
        if (Game1.IsMasterGame)
          locationRequest.Location.hostSetup();
      }
      Console.WriteLine("Warping to " + locationRequest.Name);
      if (Game1.player.IsSitting())
        Game1.player.StopSitting(false);
      Game1.player.previousLocationName = Game1.player.currentLocation != null ? (string) (NetFieldBase<string, NetString>) Game1.player.currentLocation.name : "";
      Game1.locationRequest = locationRequest;
      Game1.xLocationAfterWarp = tileX;
      Game1.yLocationAfterWarp = tileY;
      Game1._isWarping = true;
      Game1.facingDirectionAfterWarp = facingDirectionAfterWarp;
      Game1.fadeScreenToBlack();
      Game1.setRichPresence("location", (object) locationRequest.Name);
    }

    public static void requestLocationInfoFromServer()
    {
      if (Game1.locationRequest != null)
        Game1.client.sendMessage((byte) 5, (object) (short) Game1.xLocationAfterWarp, (object) (short) Game1.yLocationAfterWarp, (object) Game1.locationRequest.Name, (object) (byte) (Game1.locationRequest.IsStructure ? 1 : 0));
      Game1.currentLocation = (GameLocation) null;
      Game1.player.Position = new Vector2((float) (Game1.xLocationAfterWarp * 64), (float) (Game1.yLocationAfterWarp * 64 - (Game1.player.Sprite.getHeight() - 32) + 16));
      Game1.player.faceDirection(Game1.facingDirectionAfterWarp);
    }

    public static void changeInvisibility(string name, bool invisibility) => Game1.getCharacterFromName(name).IsInvisible = invisibility;

    public static T getCharacterFromName<T>(string name, bool mustBeVillager = true) where T : NPC
    {
      if (Game1.currentLocation != null)
      {
        foreach (NPC character in Game1.currentLocation.getCharacters())
        {
          if (character is T && character.Name.Equals(name) && (!mustBeVillager || character.isVillager()))
            return (T) character;
        }
      }
      for (int index = 0; index < Game1.locations.Count; ++index)
      {
        foreach (NPC character in Game1.locations[index].getCharacters())
        {
          if (character is T && !(Game1.locations[index] is MovieTheater) && character.Name.Equals(name) && (!mustBeVillager || character.isVillager()))
            return (T) character;
        }
      }
      if (Game1.getFarm() != null)
      {
        foreach (Building building in Game1.getFarm().buildings)
        {
          if (building.indoors.Value != null)
          {
            foreach (NPC character in building.indoors.Value.characters)
            {
              if (character is T && character.Name.Equals(name) && (!mustBeVillager || character.isVillager()))
                return (T) character;
            }
          }
        }
      }
      return default (T);
    }

    public static NPC getCharacterFromName(
      string name,
      bool mustBeVillager = true,
      bool useLocationsListOnly = false)
    {
      if (!useLocationsListOnly)
      {
        switch (Game1.currentLocation)
        {
          case null:
          case MovieTheater _:
            break;
          default:
            using (List<NPC>.Enumerator enumerator = Game1.currentLocation.getCharacters().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                NPC current = enumerator.Current;
                if (!current.eventActor && current.Name.Equals(name) && (!mustBeVillager || current.isVillager()))
                  return current;
              }
              break;
            }
        }
      }
      for (int index = 0; index < Game1.locations.Count; ++index)
      {
        if (!(Game1.locations[index] is MovieTheater))
        {
          foreach (NPC character in Game1.locations[index].getCharacters())
          {
            if (!character.eventActor && character.Name.Equals(name) && (!mustBeVillager || character.isVillager()))
              return character;
          }
        }
      }
      if (Game1.getFarm() != null)
      {
        foreach (Building building in Game1.getFarm().buildings)
        {
          if (building.indoors.Value != null)
          {
            foreach (NPC character in building.indoors.Value.characters)
            {
              if (character.Name.Equals(name) && (!mustBeVillager || character.isVillager()))
                return character;
            }
          }
        }
      }
      return (NPC) null;
    }

    public static NPC removeCharacterFromItsLocation(string name, bool must_be_villager = true)
    {
      if (!Game1.IsMasterGame)
        return (NPC) null;
      for (int index1 = 0; index1 < Game1.locations.Count; ++index1)
      {
        if (!(Game1.locations[index1] is MovieTheater))
        {
          for (int index2 = 0; index2 < Game1.locations[index1].getCharacters().Count; ++index2)
          {
            if (Game1.locations[index1].getCharacters()[index2].Name.Equals(name) && (!must_be_villager || Game1.locations[index1].getCharacters()[index2].isVillager()))
            {
              NPC character = Game1.locations[index1].characters[index2];
              Game1.locations[index1].characters.RemoveAt(index2);
              return character;
            }
          }
        }
      }
      return (NPC) null;
    }

    public static GameLocation getLocationFromName(string name) => Game1.getLocationFromName(name, false);

    public static GameLocation getLocationFromName(string name, bool isStructure)
    {
      if (string.IsNullOrEmpty(name))
        return (GameLocation) null;
      if (Game1.currentLocation != null)
      {
        if (!isStructure && string.Equals((string) (NetFieldBase<string, NetString>) Game1.currentLocation.name, name, StringComparison.OrdinalIgnoreCase))
          return Game1.currentLocation;
        if (!isStructure && (bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.isStructure && (NetFieldBase<GameLocation, NetRef<GameLocation>>) Game1.currentLocation.Root != (NetRef<GameLocation>) null && string.Equals((string) (NetFieldBase<string, NetString>) Game1.currentLocation.Root.Value.name, name, StringComparison.OrdinalIgnoreCase))
          return Game1.currentLocation.Root.Value;
        if (isStructure && (string) (NetFieldBase<string, NetString>) Game1.currentLocation.uniqueName == name)
          return Game1.currentLocation;
      }
      GameLocation gameLocation;
      return Game1._locationLookup.TryGetValue(name, out gameLocation) ? gameLocation : Game1.getLocationFromNameInLocationsList(name, isStructure);
    }

    public static GameLocation getLocationFromNameInLocationsList(
      string name,
      bool isStructure = false)
    {
      for (int index = 0; index < Game1.locations.Count; ++index)
      {
        if (!isStructure)
        {
          if (string.Equals(Game1.locations[index].Name, name, StringComparison.OrdinalIgnoreCase))
          {
            Game1._locationLookup[Game1.locations[index].Name] = Game1.locations[index];
            return Game1.locations[index];
          }
        }
        else
        {
          GameLocation structure = Game1.findStructure(Game1.locations[index], name);
          if (structure != null)
          {
            Game1._locationLookup[name] = structure;
            return structure;
          }
        }
      }
      if (name.StartsWith("UndergroundMine", StringComparison.OrdinalIgnoreCase))
        return (GameLocation) MineShaft.GetMine(name);
      if (name.StartsWith("VolcanoDungeon", StringComparison.OrdinalIgnoreCase))
        return (GameLocation) VolcanoDungeon.GetLevel(name);
      return !isStructure ? Game1.getLocationFromName(name, true) : (GameLocation) null;
    }

    public static void flushLocationLookup() => Game1._locationLookup.Clear();

    public static void removeLocationFromLocationLookup(string name_or_unique_name)
    {
      List<string> stringList = new List<string>();
      foreach (string key in Game1._locationLookup.Keys)
      {
        if (Game1._locationLookup[key].NameOrUniqueName == name_or_unique_name)
          stringList.Add(key);
      }
      foreach (string key in stringList)
        Game1._locationLookup.Remove(key);
    }

    public static void removeLocationFromLocationLookup(GameLocation location)
    {
      List<string> stringList = new List<string>();
      foreach (string key in Game1._locationLookup.Keys)
      {
        if (Game1._locationLookup[key] == location)
          stringList.Add(key);
      }
      foreach (string key in stringList)
        Game1._locationLookup.Remove(key);
    }

    public static GameLocation findStructure(GameLocation parentLocation, string name)
    {
      if (!(parentLocation is BuildableGameLocation))
        return (GameLocation) null;
      foreach (Building building in (parentLocation as BuildableGameLocation).buildings)
      {
        if (building.indoors.Value != null && building.indoors.Value.uniqueName.Equals((object) name))
          return (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors;
      }
      return (GameLocation) null;
    }

    public static void addNewFarmBuildingMaps()
    {
      if (Game1.player.CoopUpgradeLevel >= 1 && Game1.getLocationFromName("Coop") == null)
      {
        Game1.locations.Add(new GameLocation("Maps\\Coop" + Game1.player.CoopUpgradeLevel.ToString(), "Coop"));
        Game1.getLocationFromName("Farm").setTileProperty(21, 10, "Buildings", "Action", "Warp 2 9 Coop");
        Game1.currentCoopTexture = Game1.content.Load<Texture2D>("BuildingUpgrades\\Coop" + Game1.player.coopUpgradeLevel.ToString());
      }
      else if (Game1.getLocationFromName("Coop") != null)
      {
        Game1.getLocationFromName("Coop").map = Game1.content.Load<Map>("Maps\\Coop" + Game1.player.CoopUpgradeLevel.ToString());
        Game1.currentCoopTexture = Game1.content.Load<Texture2D>("BuildingUpgrades\\Coop" + Game1.player.coopUpgradeLevel.ToString());
      }
      if (Game1.player.BarnUpgradeLevel >= 1 && Game1.getLocationFromName("Barn") == null)
      {
        Game1.locations.Add(new GameLocation("Maps\\Barn" + Game1.player.BarnUpgradeLevel.ToString(), "Barn"));
        Game1.getLocationFromName("Farm").warps.Add(new Warp(14, 9, "Barn", 11, 14, false));
        Game1.currentBarnTexture = Game1.content.Load<Texture2D>("BuildingUpgrades\\Barn" + Game1.player.barnUpgradeLevel.ToString());
      }
      else if (Game1.getLocationFromName("Barn") != null)
      {
        Game1.getLocationFromName("Barn").map = Game1.content.Load<Map>("Maps\\Barn" + Game1.player.BarnUpgradeLevel.ToString());
        Game1.currentBarnTexture = Game1.content.Load<Texture2D>("BuildingUpgrades\\Barn" + Game1.player.barnUpgradeLevel.ToString());
      }
      FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
      if (Game1.player.HouseUpgradeLevel >= 1 && homeOfFarmer.Map.Id.Equals("FarmHouse"))
      {
        homeOfFarmer.updateMap();
        int currentWallpaper = Game1.currentWallpaper;
        int currentFloor = Game1.currentFloor;
        Game1.currentWallpaper = Game1.farmerWallpaper;
        Game1.currentFloor = Game1.FarmerFloor;
        Game1.updateFloorInFarmHouse(Game1.currentFloor);
        Game1.updateWallpaperInFarmHouse(Game1.currentWallpaper);
        Game1.currentWallpaper = currentWallpaper;
        Game1.currentFloor = currentFloor;
      }
      if (!Game1.player.hasGreenhouse || Game1.getLocationFromName("FarmGreenHouse") != null)
        return;
      Game1.locations.Add(new GameLocation("Maps\\FarmGreenHouse", "FarmGreenHouse"));
      Game1.getLocationFromName("Farm").setTileProperty(3, 10, "Buildings", "Action", "Warp 5 15 FarmGreenHouse");
      Game1.greenhouseTexture = Game1.content.Load<Texture2D>("BuildingUpgrades\\Greenhouse");
    }

    public static bool waitingToPassOut() => Game1.activeClickableMenu is ReadyCheckDialog && (Game1.activeClickableMenu as ReadyCheckDialog).checkName == "sleep" && !(Game1.activeClickableMenu as ReadyCheckDialog).isCancelable();

    public static void PassOutNewDay()
    {
      Game1.player.lastSleepLocation.Value = Game1.currentLocation.NameOrUniqueName;
      Game1.player.lastSleepPoint.Value = Game1.player.getTileLocationPoint();
      if (!Game1.IsMultiplayer)
      {
        Game1.NewDay(0.0f);
      }
      else
      {
        Game1.player.FarmerSprite.setCurrentSingleFrame(5, (short) 3000);
        Game1.player.FarmerSprite.PauseForSingleAnimation = true;
        Game1.player.passedOut = true;
        if (Game1.activeClickableMenu != null)
        {
          Game1.activeClickableMenu.emergencyShutDown();
          Game1.exitActiveMenu();
        }
        Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog("sleep", false, (ConfirmationDialog.behavior) (_ => Game1.NewDay(0.0f)));
      }
    }

    public static void NewDay(float timeToPause)
    {
      Game1.currentMinigame = (IMinigame) null;
      Game1.newDay = true;
      Game1.newDaySync = new NewDaySynchronizer();
      if ((bool) (NetFieldBase<bool, NetBool>) Game1.player.isInBed || Game1.player.passedOut)
      {
        Game1.nonWarpFade = true;
        Game1.screenFade.FadeScreenToBlack(Game1.player.passedOut ? 1.1f : 0.0f);
        Game1.player.Halt();
        Game1.player.currentEyes = 1;
        Game1.player.blinkTimer = -4000;
        Game1.player.CanMove = false;
        Game1.player.passedOut = false;
        Game1.pauseTime = timeToPause;
      }
      if (Game1.activeClickableMenu == null || Game1.dialogueUp)
        return;
      Game1.activeClickableMenu.emergencyShutDown();
      Game1.exitActiveMenu();
    }

    public static void screenGlowOnce(Microsoft.Xna.Framework.Color glowColor, bool hold, float rate = 0.005f, float maxAlpha = 0.3f)
    {
      Game1.screenGlowMax = maxAlpha;
      Game1.screenGlowRate = rate;
      Game1.screenGlowAlpha = 0.0f;
      Game1.screenGlowUp = true;
      Game1.screenGlowColor = glowColor;
      Game1.screenGlow = true;
      Game1.screenGlowHold = hold;
    }

    public static void removeTilesFromLayer(GameLocation l, string layer, Microsoft.Xna.Framework.Rectangle area)
    {
      for (int x = area.X; x < area.Right; ++x)
      {
        for (int y = area.Y; y < area.Bottom; ++y)
          l.Map.GetLayer(layer).Tiles[x, y] = (Tile) null;
      }
    }

    public static void removeFrontLayerForFarmBuildings()
    {
    }

    public static string shortDayNameFromDayOfSeason(int dayOfSeason)
    {
      switch (dayOfSeason % 7)
      {
        case 0:
          return "Sun";
        case 1:
          return "Mon";
        case 2:
          return "Tue";
        case 3:
          return "Wed";
        case 4:
          return "Thu";
        case 5:
          return "Fri";
        case 6:
          return "Sat";
        default:
          return "";
      }
    }

    public static string shortDayDisplayNameFromDayOfSeason(int dayOfSeason) => dayOfSeason < 0 ? string.Empty : Game1._shortDayDisplayName[dayOfSeason % 7];

    public static void showNameSelectScreen(string type)
    {
      Game1.nameSelectType = type;
      Game1.nameSelectUp = true;
    }

    public static void nameSelectionDone()
    {
    }

    public static void tryToBuySelectedItems()
    {
      if (Game1.selectedItemsType.Equals("flutePitch"))
      {
        Game1.currentObjectDialogue.Clear();
        Game1.currentLocation.actionObjectForQuestionDialogue.scale.X = (float) (Game1.numberOfSelectedItems * 100);
        Game1.dialogueUp = false;
        Game1.player.CanMove = true;
        Game1.numberOfSelectedItems = -1;
      }
      else if (Game1.selectedItemsType.Equals("drumTone"))
      {
        Game1.currentObjectDialogue.Clear();
        Game1.currentLocation.actionObjectForQuestionDialogue.scale.X = (float) Game1.numberOfSelectedItems;
        Game1.dialogueUp = false;
        Game1.player.CanMove = true;
        Game1.numberOfSelectedItems = -1;
      }
      else if (Game1.selectedItemsType.Equals("jukebox"))
      {
        Game1.changeMusicTrack(Game1.player.songsHeard.ElementAt<string>(Game1.numberOfSelectedItems));
        Game1.dialogueUp = false;
        Game1.player.CanMove = true;
        Game1.numberOfSelectedItems = -1;
      }
      else if (Game1.player.Money >= Game1.priceOfSelectedItem * Game1.numberOfSelectedItems && Game1.numberOfSelectedItems > 0)
      {
        bool flag = true;
        string selectedItemsType = Game1.selectedItemsType;
        if (!(selectedItemsType == "Animal Food"))
        {
          if (!(selectedItemsType == "Fuel"))
          {
            if (selectedItemsType == "Star Token")
            {
              Game1.player.festivalScore += Game1.numberOfSelectedItems;
              Game1.dialogueUp = false;
              Game1.player.canMove = true;
            }
          }
          else
            ((Lantern) Game1.player.getToolFromName("Lantern")).fuelLeft += Game1.numberOfSelectedItems;
        }
        else
        {
          Game1.player.Feed += Game1.numberOfSelectedItems;
          Game1.setDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3072"), false);
        }
        if (!flag)
          return;
        Game1.player.Money -= Game1.priceOfSelectedItem * Game1.numberOfSelectedItems;
        Game1.numberOfSelectedItems = -1;
        Game1.playSound("purchase");
      }
      else
      {
        if (Game1.player.Money >= Game1.priceOfSelectedItem * Game1.numberOfSelectedItems)
          return;
        Game1.currentObjectDialogue.Dequeue();
        Game1.setDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3078"), false);
        Game1.numberOfSelectedItems = -1;
      }
    }

    public static void throwActiveObjectDown()
    {
      Game1.player.CanMove = false;
      switch (Game1.player.FacingDirection)
      {
        case 0:
          ((FarmerSprite) Game1.player.Sprite).animateBackwardsOnce(80, 50f);
          break;
        case 1:
          ((FarmerSprite) Game1.player.Sprite).animateBackwardsOnce(72, 50f);
          break;
        case 2:
          ((FarmerSprite) Game1.player.Sprite).animateBackwardsOnce(64, 50f);
          break;
        case 3:
          ((FarmerSprite) Game1.player.Sprite).animateBackwardsOnce(88, 50f);
          break;
      }
      Game1.player.reduceActiveItemByOne();
      Game1.playSound("throwDownITem");
    }

    public static void runTestEvent()
    {
      StreamReader streamReader = new StreamReader("test_event.txt");
      string str = streamReader.ReadLine();
      string event_string = streamReader.ReadToEnd();
      event_string = event_string.Replace("\r\n", "/");
      Console.WriteLine(event_string);
      LocationRequest locationRequest = Game1.getLocationRequest(str);
      locationRequest.OnWarp += (LocationRequest.Callback) (() =>
      {
        Game1.currentLocation.currentEvent = new Event(event_string);
        Game1.currentLocation.checkForEvents();
      });
      int x = 8;
      int y = 8;
      Utility.getDefaultWarpLocation(str, ref x, ref y);
      Game1.warpFarmer(locationRequest, x, y, Game1.player.FacingDirection);
    }

    public static bool isMusicContextActiveButNotPlaying(Game1.MusicContext music_context = Game1.MusicContext.Default) => Game1._activeMusicContext == music_context && (Game1.getMusicTrackName() == "none" || Game1.currentSong != null && Game1.currentSong.Name == Game1.getMusicTrackName() && !Game1.currentSong.IsPlaying);

    public static bool IsMusicContextActive(Game1.MusicContext music_context = Game1.MusicContext.Default) => Game1._activeMusicContext != music_context;

    public static bool doesMusicContextHaveTrack(Game1.MusicContext music_context = Game1.MusicContext.Default) => Game1._requestedMusicTracks.ContainsKey(music_context);

    public static string getMusicTrackName(Game1.MusicContext music_context = Game1.MusicContext.Default) => Game1._requestedMusicTracks.ContainsKey(music_context) ? Game1._requestedMusicTracks[music_context].Key : "none";

    public static void stopMusicTrack(Game1.MusicContext music_context)
    {
      if (!Game1._requestedMusicTracks.ContainsKey(music_context))
        return;
      Game1._requestedMusicTracks.Remove(music_context);
      Game1.UpdateRequestedMusicTrack();
    }

    public static void changeMusicTrack(
      string newTrackName,
      bool track_interruptable = false,
      Game1.MusicContext music_context = Game1.MusicContext.Default)
    {
      if (music_context == Game1.MusicContext.Default && Game1.morningSongPlayAction != null)
      {
        if (Game1.delayedActions.Contains(Game1.morningSongPlayAction))
          Game1.delayedActions.Remove(Game1.morningSongPlayAction);
        Game1.morningSongPlayAction = (DelayedAction) null;
      }
      if (music_context != Game1.MusicContext.ImportantSplitScreenMusic && !Game1.player.songsHeard.Contains(newTrackName))
        Utility.farmerHeardSong(newTrackName);
      Game1._requestedMusicTracks[music_context] = new KeyValuePair<string, bool>(newTrackName, track_interruptable);
      Game1.UpdateRequestedMusicTrack();
    }

    public static void UpdateRequestedMusicTrack()
    {
      Game1._activeMusicContext = Game1.MusicContext.Default;
      KeyValuePair<string, bool> keyValuePair = new KeyValuePair<string, bool>("none", true);
      for (int key = 0; key < 5; ++key)
      {
        if (Game1._requestedMusicTracks.ContainsKey((Game1.MusicContext) key))
        {
          if (key != 4)
            Game1._activeMusicContext = (Game1.MusicContext) key;
          keyValuePair = Game1._requestedMusicTracks[(Game1.MusicContext) key];
        }
      }
      if (!(keyValuePair.Key != Game1.requestedMusicTrack) && keyValuePair.Value == Game1.requestedMusicTrackOverrideable)
        return;
      Game1.requestedMusicDirty = true;
      Game1.requestedMusicTrack = keyValuePair.Key;
      Game1.requestedMusicTrackOverrideable = keyValuePair.Value;
    }

    public static void enterMine(int whatLevel)
    {
      Game1.inMine = true;
      Game1.warpFarmer("UndergroundMine" + whatLevel.ToString(), 6, 6, 2);
    }

    public static string GetSeasonForLocation(GameLocation location)
    {
      if (location == null)
        return Game1.currentSeason;
      return location.Name == "Greenhouse" ? "spring" : location.GetSeasonForLocation();
    }

    public static void getSteamAchievement(string which)
    {
      if (which.Equals("0"))
        which = "a0";
      Program.sdk.GetAchievement(which);
    }

    public static void getAchievement(int which, bool allowBroadcasting = true)
    {
      if (Game1.player.achievements.Contains(which) || Game1.gameMode != (byte) 3)
        return;
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Achievements");
      if (!dictionary.ContainsKey(which))
        return;
      string message = dictionary[which].Split('^')[0];
      Game1.player.achievements.Add(which);
      if (which < 32 & allowBroadcasting)
      {
        if (Game1.stats.isSharedAchievement(which))
        {
          Game1.multiplayer.sendSharedAchievementMessage(which);
        }
        else
        {
          string str = Game1.player.Name;
          if (str == "")
            str = Game1.content.LoadString("Strings\\UI:Chat_PlayerJoinedNewName");
          Game1.multiplayer.globalChatInfoMessage("Achievement", str, "achievement:" + which.ToString());
        }
      }
      Game1.playSound("achievement");
      Program.sdk.GetAchievement(which.ToString() ?? "");
      Game1.addHUDMessage(new HUDMessage(message, true));
      if (Game1.player.hasOrWillReceiveMail("hatter"))
        return;
      Game1.addMailForTomorrow("hatter");
    }

    public static void createMultipleObjectDebris(int index, int xTile, int yTile, int number)
    {
      for (int index1 = 0; index1 < number; ++index1)
        Game1.createObjectDebris(index, xTile, yTile);
    }

    public static void createMultipleObjectDebris(
      int index,
      int xTile,
      int yTile,
      int number,
      GameLocation location)
    {
      for (int index1 = 0; index1 < number; ++index1)
        Game1.createObjectDebris(index, xTile, yTile, -1, 0, 1f, location);
    }

    public static void createMultipleObjectDebris(
      int index,
      int xTile,
      int yTile,
      int number,
      float velocityMultiplier)
    {
      for (int index1 = 0; index1 < number; ++index1)
        Game1.createObjectDebris(index, xTile, yTile, velocityMultiplyer: velocityMultiplier);
    }

    public static void createMultipleObjectDebris(
      int index,
      int xTile,
      int yTile,
      int number,
      long who)
    {
      for (int index1 = 0; index1 < number; ++index1)
        Game1.createObjectDebris(index, xTile, yTile, who);
    }

    public static void createMultipleObjectDebris(
      int index,
      int xTile,
      int yTile,
      int number,
      long who,
      GameLocation location)
    {
      for (int index1 = 0; index1 < number; ++index1)
        Game1.createObjectDebris(index, xTile, yTile, who, location);
    }

    public static void createDebris(int debrisType, int xTile, int yTile, int numberOfChunks) => Game1.createDebris(debrisType, xTile, yTile, numberOfChunks, Game1.currentLocation);

    public static void createDebris(
      int debrisType,
      int xTile,
      int yTile,
      int numberOfChunks,
      GameLocation location)
    {
      if (location == null)
        location = Game1.currentLocation;
      location.debris.Add(new Debris(debrisType, numberOfChunks, new Vector2((float) (xTile * 64 + 32), (float) (yTile * 64 + 32)), new Vector2((float) Game1.player.getStandingX(), (float) Game1.player.getStandingY())));
    }

    public static Debris createItemDebris(
      Item item,
      Vector2 origin,
      int direction,
      GameLocation location = null,
      int groundLevel = -1)
    {
      if (location == null)
        location = Game1.currentLocation;
      Vector2 targetLocation = new Vector2(origin.X, origin.Y);
      switch (direction)
      {
        case -1:
          targetLocation = Game1.player.getStandingPosition();
          break;
        case 0:
          origin.X -= 32f;
          origin.Y -= (float) (128 + Game1.recentMultiplayerRandom.Next(32));
          targetLocation.Y -= 192f;
          break;
        case 1:
          origin.X += 42f;
          origin.Y -= (float) (32 - Game1.recentMultiplayerRandom.Next(8));
          targetLocation.X += 256f;
          break;
        case 2:
          origin.X -= 32f;
          origin.Y += (float) Game1.recentMultiplayerRandom.Next(32);
          targetLocation.Y += 96f;
          break;
        case 3:
          origin.X -= 64f;
          origin.Y -= (float) (32 - Game1.recentMultiplayerRandom.Next(8));
          targetLocation.X -= 256f;
          break;
      }
      Debris itemDebris = new Debris(item, origin, targetLocation);
      if (groundLevel != -1)
        itemDebris.chunkFinalYLevel = groundLevel;
      location.debris.Add(itemDebris);
      return itemDebris;
    }

    public static void createRadialDebris(
      GameLocation location,
      int debrisType,
      int xTile,
      int yTile,
      int numberOfChunks,
      bool resource,
      int groundLevel = -1,
      bool item = false,
      int color = -1)
    {
      if (groundLevel == -1)
        groundLevel = yTile * 64 + 32;
      Vector2 debrisOrigin = new Vector2((float) (xTile * 64 + 64), (float) (yTile * 64 + 64));
      if (item)
      {
        for (; numberOfChunks > 0; --numberOfChunks)
        {
          switch (Game1.random.Next(4))
          {
            case 0:
              location.debris.Add(new Debris((Item) new Object(Vector2.Zero, debrisType, 1), debrisOrigin, debrisOrigin + new Vector2(-64f, 0.0f)));
              break;
            case 1:
              location.debris.Add(new Debris((Item) new Object(Vector2.Zero, debrisType, 1), debrisOrigin, debrisOrigin + new Vector2(64f, 0.0f)));
              break;
            case 2:
              location.debris.Add(new Debris((Item) new Object(Vector2.Zero, debrisType, 1), debrisOrigin, debrisOrigin + new Vector2(0.0f, 64f)));
              break;
            case 3:
              location.debris.Add(new Debris((Item) new Object(Vector2.Zero, debrisType, 1), debrisOrigin, debrisOrigin + new Vector2(0.0f, -64f)));
              break;
          }
        }
      }
      if (resource)
      {
        location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(-64f, 0.0f)));
        ++numberOfChunks;
        location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(64f, 0.0f)));
        ++numberOfChunks;
        location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, -64f)));
        ++numberOfChunks;
        location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, 64f)));
      }
      else
      {
        location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(-64f, 0.0f), groundLevel, color));
        ++numberOfChunks;
        location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(64f, 0.0f), groundLevel, color));
        ++numberOfChunks;
        location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, -64f), groundLevel, color));
        ++numberOfChunks;
        location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, 64f), groundLevel, color));
      }
    }

    public static void createRadialDebris(
      GameLocation location,
      string texture,
      Microsoft.Xna.Framework.Rectangle sourcerectangle,
      int xTile,
      int yTile,
      int numberOfChunks)
    {
      Game1.createRadialDebris(location, texture, sourcerectangle, xTile, yTile, numberOfChunks, yTile);
    }

    public static void createWaterDroplets(
      string texture,
      Microsoft.Xna.Framework.Rectangle sourcerectangle,
      int xPosition,
      int yPosition,
      int numberOfChunks,
      int groundLevelTile)
    {
      Vector2 debrisOrigin = new Vector2((float) xPosition, (float) yPosition);
      Game1.currentLocation.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(-64f, 0.0f), groundLevelTile * 64));
      Game1.currentLocation.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(64f, 0.0f), groundLevelTile * 64));
      Game1.currentLocation.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, -64f), groundLevelTile * 64));
      Game1.currentLocation.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, 64f), groundLevelTile * 64));
    }

    public static void createRadialDebris(
      GameLocation location,
      string texture,
      Microsoft.Xna.Framework.Rectangle sourcerectangle,
      int xTile,
      int yTile,
      int numberOfChunks,
      int groundLevelTile)
    {
      Game1.createRadialDebris(location, texture, sourcerectangle, 8, xTile * 64 + 32 + Game1.random.Next(32), yTile * 64 + 32 + Game1.random.Next(32), numberOfChunks, groundLevelTile);
    }

    public static void createRadialDebris(
      GameLocation location,
      string texture,
      Microsoft.Xna.Framework.Rectangle sourcerectangle,
      int sizeOfSourceRectSquares,
      int xPosition,
      int yPosition,
      int numberOfChunks,
      int groundLevelTile)
    {
      Vector2 debrisOrigin = new Vector2((float) xPosition, (float) yPosition);
      location.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(-64f, 0.0f), groundLevelTile * 64, sizeOfSourceRectSquares));
      location.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(64f, 0.0f), groundLevelTile * 64, sizeOfSourceRectSquares));
      location.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, -64f), groundLevelTile * 64, sizeOfSourceRectSquares));
      location.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, 64f), groundLevelTile * 64, sizeOfSourceRectSquares));
    }

    public static void createRadialDebris(
      GameLocation location,
      string texture,
      Microsoft.Xna.Framework.Rectangle sourcerectangle,
      int sizeOfSourceRectSquares,
      int xPosition,
      int yPosition,
      int numberOfChunks,
      int groundLevelTile,
      Microsoft.Xna.Framework.Color color)
    {
      Game1.createRadialDebris(location, texture, sourcerectangle, sizeOfSourceRectSquares, xPosition, yPosition, numberOfChunks, groundLevelTile, color, 1f);
    }

    public static void createRadialDebris(
      GameLocation location,
      string texture,
      Microsoft.Xna.Framework.Rectangle sourcerectangle,
      int sizeOfSourceRectSquares,
      int xPosition,
      int yPosition,
      int numberOfChunks,
      int groundLevelTile,
      Microsoft.Xna.Framework.Color color,
      float scale)
    {
      Vector2 debrisOrigin = new Vector2((float) xPosition, (float) yPosition);
      for (; numberOfChunks > 0; --numberOfChunks)
      {
        switch (Game1.random.Next(4))
        {
          case 0:
            Debris debris1 = new Debris(texture, sourcerectangle, 1, debrisOrigin, debrisOrigin + new Vector2(-64f, 0.0f), groundLevelTile * 64, sizeOfSourceRectSquares);
            debris1.nonSpriteChunkColor.Value = color;
            location?.debris.Add(debris1);
            debris1.Chunks[0].scale = scale;
            break;
          case 1:
            Debris debris2 = new Debris(texture, sourcerectangle, 1, debrisOrigin, debrisOrigin + new Vector2(64f, 0.0f), groundLevelTile * 64, sizeOfSourceRectSquares);
            debris2.nonSpriteChunkColor.Value = color;
            location?.debris.Add(debris2);
            debris2.Chunks[0].scale = scale;
            break;
          case 2:
            Debris debris3 = new Debris(texture, sourcerectangle, 1, debrisOrigin, debrisOrigin + new Vector2((float) Game1.random.Next(-64, 64), -64f), groundLevelTile * 64, sizeOfSourceRectSquares);
            debris3.nonSpriteChunkColor.Value = color;
            location?.debris.Add(debris3);
            debris3.Chunks[0].scale = scale;
            break;
          case 3:
            Debris debris4 = new Debris(texture, sourcerectangle, 1, debrisOrigin, debrisOrigin + new Vector2((float) Game1.random.Next(-64, 64), 64f), groundLevelTile * 64, sizeOfSourceRectSquares);
            debris4.nonSpriteChunkColor.Value = color;
            location?.debris.Add(debris4);
            debris4.Chunks[0].scale = scale;
            break;
        }
      }
    }

    public static void createObjectDebris(int objectIndex, int xTile, int yTile, long whichPlayer) => Game1.currentLocation.debris.Add(new Debris(objectIndex, new Vector2((float) (xTile * 64 + 32), (float) (yTile * 64 + 32)), Game1.getFarmer(whichPlayer).getStandingPosition()));

    public static void createObjectDebris(
      int objectIndex,
      int xTile,
      int yTile,
      long whichPlayer,
      GameLocation location)
    {
      location.debris.Add(new Debris(objectIndex, new Vector2((float) (xTile * 64 + 32), (float) (yTile * 64 + 32)), Game1.getFarmer(whichPlayer).getStandingPosition()));
    }

    public static void createObjectDebris(
      int objectIndex,
      int xTile,
      int yTile,
      GameLocation location)
    {
      Game1.createObjectDebris(objectIndex, xTile, yTile, -1, 0, 1f, location);
    }

    public static void createObjectDebris(
      int objectIndex,
      int xTile,
      int yTile,
      int groundLevel = -1,
      int itemQuality = 0,
      float velocityMultiplyer = 1f,
      GameLocation location = null)
    {
      if (location == null)
        location = Game1.currentLocation;
      Debris debris = new Debris(objectIndex, new Vector2((float) (xTile * 64 + 32), (float) (yTile * 64 + 32)), new Vector2((float) Game1.player.getStandingX(), (float) Game1.player.getStandingY()))
      {
        itemQuality = itemQuality
      };
      foreach (Chunk chunk in debris.Chunks)
      {
        chunk.xVelocity.Value *= velocityMultiplyer;
        chunk.yVelocity.Value *= velocityMultiplyer;
      }
      if (groundLevel != -1)
        debris.chunkFinalYLevel = groundLevel;
      location.debris.Add(debris);
    }

    public static Farmer getFarmer(long id)
    {
      if (Game1.player.UniqueMultiplayerID == id)
        return Game1.player;
      foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
      {
        if (farmer.UniqueMultiplayerID == id)
          return farmer;
      }
      return !Game1.IsMultiplayer ? Game1.player : Game1.MasterPlayer;
    }

    public static Farmer getFarmerMaybeOffline(long id)
    {
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.UniqueMultiplayerID == id)
          return allFarmer;
      }
      return (Farmer) null;
    }

    public static IEnumerable<Farmer> getAllFarmers() => Enumerable.Repeat<Farmer>(Game1.MasterPlayer, 1).Concat<Farmer>(Game1.getAllFarmhands());

    public static IEnumerable<Farmer> getAllFarmhands()
    {
      if (Game1.getFarm() != null)
      {
        foreach (Building building in Game1.getFarm().buildings)
        {
          if (building.indoors.Value is Cabin)
          {
            Farmer otherFarmer = (building.indoors.Value as Cabin).farmhand.Value;
            if (otherFarmer != null)
            {
              if (otherFarmer.isActive())
                otherFarmer = Game1.otherFarmers[otherFarmer.UniqueMultiplayerID];
              yield return otherFarmer;
            }
          }
        }
      }
    }

    public static FarmerCollection getOnlineFarmers() => Game1._onlineFarmers;

    public static void farmerFindsArtifact(int objectIndex) => Game1.player.addItemToInventoryBool((Item) new Object(objectIndex, 1));

    public static bool doesHUDMessageExist(string s)
    {
      for (int index = 0; index < Game1.hudMessages.Count; ++index)
      {
        if (s.Equals(Game1.hudMessages[index].message))
          return true;
      }
      return false;
    }

    public static void addHUDMessage(HUDMessage message)
    {
      if (message.type != null || message.whatType != 0)
      {
        for (int index = 0; index < Game1.hudMessages.Count; ++index)
        {
          if (message.type != null && Game1.hudMessages[index].type != null && Game1.hudMessages[index].type.Equals(message.type) && Game1.hudMessages[index].add == message.add)
          {
            Game1.hudMessages[index].number = message.add ? Game1.hudMessages[index].number + message.number : Game1.hudMessages[index].number - message.number;
            Game1.hudMessages[index].timeLeft = 3500f;
            Game1.hudMessages[index].transparency = 1f;
            return;
          }
          if (message.whatType == Game1.hudMessages[index].whatType && message.whatType != 1 && message.message != null && message.message.Equals(Game1.hudMessages[index].message))
          {
            Game1.hudMessages[index].timeLeft = message.timeLeft;
            Game1.hudMessages[index].transparency = 1f;
            return;
          }
        }
      }
      Game1.hudMessages.Add(message);
      for (int index = Game1.hudMessages.Count - 1; index >= 0; --index)
      {
        if (Game1.hudMessages[index].noIcon)
        {
          HUDMessage hudMessage = Game1.hudMessages[index];
          Game1.hudMessages.RemoveAt(index);
          Game1.hudMessages.Add(hudMessage);
        }
      }
    }

    public static void nextMineLevel() => Game1.warpFarmer("UndergroundMine" + (Game1.CurrentMineLevel + 1).ToString(), 16, 16, false);

    public static void showSwordswipeAnimation(
      int direction,
      Vector2 source,
      float animationSpeed,
      bool flip)
    {
      switch (direction)
      {
        case 0:
          Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(-1, animationSpeed, 5, 1, new Vector2(source.X + 32f, source.Y), false, false, !flip, -1.570796f));
          break;
        case 1:
          Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(-1, animationSpeed, 5, 1, new Vector2((float) ((double) source.X + 96.0 + 16.0), source.Y + 48f), false, flip, false, flip ? -3.141593f : 0.0f));
          break;
        case 2:
          Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(-1, animationSpeed, 5, 1, new Vector2(source.X + 32f, source.Y + 128f), false, false, !flip, 1.570796f));
          break;
        case 3:
          Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(-1, animationSpeed, 5, 1, new Vector2((float) ((double) source.X - 32.0 - 16.0), source.Y + 48f), false, !flip, false, flip ? -3.141593f : 0.0f));
          break;
      }
    }

    public static void removeSquareDebrisFromTile(int tileX, int tileY) => Game1.currentLocation.debris.Filter((Func<Debris, bool>) (debris => (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) debris.debrisType != Debris.DebrisType.SQUARES || (int) ((double) debris.Chunks[0].position.X / 64.0) != tileX || debris.chunkFinalYLevel / 64 != tileY));

    public static void removeDebris(Debris.DebrisType type) => Game1.currentLocation.debris.Filter((Func<Debris, bool>) (debris => (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) debris.debrisType != type));

    public static void toolAnimationDone(Farmer who)
    {
      float stamina = Game1.player.Stamina;
      if (who.CurrentTool == null)
        return;
      if ((double) who.Stamina > 0.0)
      {
        int power = (int) (((double) Game1.toolHold + 20.0) / 600.0) + 1;
        Vector2 toolLocation = who.GetToolLocation();
        if (who.CurrentTool is FishingRod && ((FishingRod) who.CurrentTool).isFishing)
          who.canReleaseTool = false;
        else if (!(who.CurrentTool is FishingRod))
        {
          who.UsingTool = false;
          if (who.CurrentTool.Name.Contains("Seeds"))
          {
            if (!Game1.eventUp)
            {
              who.CurrentTool.DoFunction(Game1.currentLocation, who.getStandingX(), who.getStandingY(), power, who);
              if (((Seeds) who.CurrentTool).NumberInStack <= 0)
                who.removeItemFromInventory((Item) who.CurrentTool);
            }
          }
          else if (who.CurrentTool.Name.Equals("Watering Can"))
          {
            switch (who.FacingDirection)
            {
              case 0:
              case 2:
                who.CurrentTool.DoFunction(Game1.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, power, who);
                break;
              case 1:
              case 3:
                who.CurrentTool.DoFunction(Game1.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, power, who);
                break;
            }
          }
          else if (who.CurrentTool is MeleeWeapon)
          {
            who.CurrentTool.CurrentParentTileIndex = who.CurrentTool.IndexOfMenuItemView;
          }
          else
          {
            if (who.CurrentTool.Name.Equals("Wand"))
              who.CurrentTool.CurrentParentTileIndex = who.CurrentTool.IndexOfMenuItemView;
            who.CurrentTool.DoFunction(Game1.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, power, who);
          }
        }
        else
          who.UsingTool = false;
      }
      else if ((bool) (NetFieldBase<bool, NetBool>) who.CurrentTool.instantUse)
        who.CurrentTool.DoFunction(Game1.currentLocation, 0, 0, 0, who);
      else
        who.UsingTool = false;
      who.lastClick = Vector2.Zero;
      Game1.toolHold = 0.0f;
      if (who.IsLocalPlayer && !Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift))
        who.setRunning(Game1.options.autoRun);
      if (!who.UsingTool && who.FarmerSprite.PauseForSingleAnimation)
        who.FarmerSprite.StopAnimation();
      if ((double) Game1.player.Stamina > 0.0 || (double) stamina <= 0.0)
        return;
      Game1.player.doEmote(36);
    }

    public static bool pressActionButton(
      KeyboardState currentKBState,
      MouseState currentMouseState,
      GamePadState currentPadState)
    {
      if (Game1.IsChatting)
        currentKBState = new KeyboardState();
      if (Game1.dialogueTyping)
      {
        bool flag = true;
        Game1.dialogueTyping = false;
        if (Game1.currentSpeaker != null)
          Game1.currentDialogueCharacterIndex = Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue().Length;
        else if (Game1.currentObjectDialogue.Count > 0)
          Game1.currentDialogueCharacterIndex = Game1.currentObjectDialogue.Peek().Length;
        else
          flag = false;
        Game1.dialogueTypingInterval = 0;
        Game1.oldKBState = currentKBState;
        Game1.oldMouseState = Game1.input.GetMouseState();
        Game1.oldPadState = currentPadState;
        if (flag)
        {
          Game1.playSound("dialogueCharacterClose");
          return false;
        }
      }
      if (Game1.dialogueUp && Game1.numberOfSelectedItems == -1)
      {
        if (Game1.isQuestion)
        {
          Game1.isQuestion = false;
          if (Game1.currentSpeaker != null)
          {
            if (Game1.currentSpeaker.CurrentDialogue.Peek().chooseResponse(Game1.questionChoices[Game1.currentQuestionChoice]))
            {
              Game1.currentDialogueCharacterIndex = 1;
              Game1.dialogueTyping = true;
              Game1.oldKBState = currentKBState;
              Game1.oldMouseState = Game1.input.GetMouseState();
              Game1.oldPadState = currentPadState;
              return false;
            }
          }
          else
          {
            Game1.dialogueUp = false;
            if (Game1.eventUp && Game1.currentLocation.afterQuestion == null)
            {
              Game1.currentLocation.currentEvent.answerDialogue(Game1.currentLocation.lastQuestionKey, Game1.currentQuestionChoice);
              Game1.currentQuestionChoice = 0;
              Game1.oldKBState = currentKBState;
              Game1.oldMouseState = Game1.input.GetMouseState();
              Game1.oldPadState = currentPadState;
            }
            else if (Game1.currentLocation.answerDialogue(Game1.questionChoices[Game1.currentQuestionChoice]))
            {
              Game1.currentQuestionChoice = 0;
              Game1.oldKBState = currentKBState;
              Game1.oldMouseState = Game1.input.GetMouseState();
              Game1.oldPadState = currentPadState;
              return false;
            }
            if (Game1.dialogueUp)
            {
              Game1.currentDialogueCharacterIndex = 1;
              Game1.dialogueTyping = true;
              Game1.oldKBState = currentKBState;
              Game1.oldMouseState = Game1.input.GetMouseState();
              Game1.oldPadState = currentPadState;
              return false;
            }
          }
          Game1.currentQuestionChoice = 0;
        }
        string str = (string) null;
        if (Game1.currentSpeaker != null)
        {
          if (!Game1.currentSpeaker.immediateSpeak)
          {
            str = Game1.currentSpeaker.CurrentDialogue.Count > 0 ? Game1.currentSpeaker.CurrentDialogue.Peek().exitCurrentDialogue() : (string) null;
          }
          else
          {
            Game1.currentSpeaker.immediateSpeak = false;
            return false;
          }
        }
        if (str == null)
        {
          if (Game1.currentSpeaker != null && Game1.currentSpeaker.CurrentDialogue.Count > 0 && Game1.currentSpeaker.CurrentDialogue.Peek().isOnFinalDialogue() && Game1.currentSpeaker.CurrentDialogue.Count > 0)
            Game1.currentSpeaker.CurrentDialogue.Pop();
          Game1.dialogueUp = false;
          if (Game1.messagePause)
            Game1.pauseTime = 500f;
          if (Game1.currentObjectDialogue.Count > 0)
            Game1.currentObjectDialogue.Dequeue();
          Game1.currentDialogueCharacterIndex = 0;
          if (Game1.currentObjectDialogue.Count > 0)
          {
            Game1.dialogueUp = true;
            Game1.questionChoices.Clear();
            Game1.oldKBState = currentKBState;
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldPadState = currentPadState;
            Game1.dialogueTyping = true;
            return false;
          }
          Game1.tvStation = -1;
          if (Game1.currentSpeaker != null && !Game1.currentSpeaker.Name.Equals("Gunther") && !Game1.eventUp && !(bool) (NetFieldBase<bool, NetBool>) Game1.currentSpeaker.doingEndOfRouteAnimation)
            Game1.currentSpeaker.doneFacingPlayer(Game1.player);
          Game1.currentSpeaker = (NPC) null;
          if (!Game1.eventUp)
            Game1.player.CanMove = true;
          else if (Game1.currentLocation.currentEvent.CurrentCommand > 0 || Game1.currentLocation.currentEvent.specialEventVariable1)
          {
            if (!Game1.isFestival() || !Game1.currentLocation.currentEvent.canMoveAfterDialogue())
              ++Game1.currentLocation.currentEvent.CurrentCommand;
            else
              Game1.player.CanMove = true;
          }
          Game1.questionChoices.Clear();
          Game1.playSound("smallSelect");
        }
        else
        {
          Game1.playSound("smallSelect");
          Game1.currentDialogueCharacterIndex = 0;
          Game1.dialogueTyping = true;
          Game1.checkIfDialogueIsQuestion();
        }
        Game1.oldKBState = currentKBState;
        Game1.oldMouseState = Game1.input.GetMouseState();
        Game1.oldPadState = currentPadState;
        if (Game1.questOfTheDay != null && (bool) (NetFieldBase<bool, NetBool>) Game1.questOfTheDay.accepted && Game1.questOfTheDay is SocializeQuest)
          Game1.questOfTheDay.checkIfComplete(number2: -1);
        Game1.afterFadeFunction afterDialogues = Game1.afterDialogues;
        return false;
      }
      if (Game1.currentBillboard != 0)
      {
        Game1.currentBillboard = 0;
        Game1.player.CanMove = true;
        Game1.oldKBState = currentKBState;
        Game1.oldMouseState = Game1.input.GetMouseState();
        Game1.oldPadState = currentPadState;
        return false;
      }
      if (!Game1.player.UsingTool && !Game1.pickingTool && !Game1.menuUp && (!Game1.eventUp || Game1.currentLocation.currentEvent != null && Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.nameSelectUp && Game1.numberOfSelectedItems == -1 && !Game1.fadeToBlack)
      {
        if (Game1.wasMouseVisibleThisFrame && Game1.currentLocation is IAnimalLocation)
        {
          Vector2 position = new Vector2((float) (Game1.getOldMouseX() + Game1.viewport.X), (float) (Game1.getOldMouseY() + Game1.viewport.Y));
          if (Utility.withinRadiusOfPlayer((int) position.X, (int) position.Y, 1, Game1.player) && ((Game1.currentLocation as IAnimalLocation).CheckPetAnimal(position, Game1.player) || Game1.didPlayerJustRightClick(true) && (Game1.currentLocation as IAnimalLocation).CheckInspectAnimal(position, Game1.player)))
            return true;
        }
        Vector2 vector2_1 = new Vector2((float) (Game1.getOldMouseX() + Game1.viewport.X), (float) (Game1.getOldMouseY() + Game1.viewport.Y)) / 64f;
        Vector2 vector2_2 = vector2_1;
        if (!Game1.wasMouseVisibleThisFrame || (double) Game1.mouseCursorTransparency == 0.0 || !Utility.tileWithinRadiusOfPlayer((int) vector2_1.X, (int) vector2_1.Y, 1, Game1.player))
          vector2_1 = Game1.player.GetGrabTile();
        bool flag1 = false;
        if (!Game1.eventUp || Game1.isFestival())
        {
          if (Game1.tryToCheckAt(vector2_1, Game1.player))
            return false;
          if (Game1.player.isRidingHorse())
          {
            Game1.player.mount.checkAction(Game1.player, Game1.player.currentLocation);
            return false;
          }
          if (!Game1.player.canMove)
            return false;
          if (!flag1)
          {
            switch (Game1.player.currentLocation.isCharacterAtTile(vector2_1))
            {
              case null:
              case null:
                break;
              default:
                flag1 = true;
                break;
            }
          }
          bool flag2 = false;
          if (Game1.player.ActiveObject != null && !(Game1.player.ActiveObject is Furniture))
          {
            if (Game1.player.ActiveObject.performUseAction(Game1.currentLocation))
            {
              Game1.player.reduceActiveItemByOne();
              Game1.oldKBState = currentKBState;
              Game1.oldMouseState = Game1.input.GetMouseState();
              Game1.oldPadState = currentPadState;
              return false;
            }
            int stack = Game1.player.ActiveObject.Stack;
            Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
            if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.actionButton))
              Game1.isCheckingNonMousePlacement = true;
            Vector2 placementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, Game1.currentLocation, (Item) Game1.player.ActiveObject, (int) vector2_1.X * 64 + 32, (int) vector2_1.Y * 64 + 32);
            if (!Game1.isCheckingNonMousePlacement && Game1.player.ActiveObject is Wallpaper && Utility.tryToPlaceItem(Game1.currentLocation, (Item) Game1.player.ActiveObject, (int) vector2_2.X * 64, (int) vector2_2.Y * 64))
            {
              Game1.isCheckingNonMousePlacement = false;
              return true;
            }
            if (Utility.tryToPlaceItem(Game1.currentLocation, (Item) Game1.player.ActiveObject, (int) placementPosition.X, (int) placementPosition.Y))
            {
              Game1.isCheckingNonMousePlacement = false;
              return true;
            }
            if (!Game1.eventUp && (Game1.player.ActiveObject == null || Game1.player.ActiveObject.Stack < stack || Game1.player.ActiveObject.isPlaceable()))
              flag2 = true;
            Game1.isCheckingNonMousePlacement = false;
          }
          if (!flag2 && !flag1)
          {
            ++vector2_1.Y;
            if (Game1.player.FacingDirection >= 0 && Game1.player.FacingDirection <= 3)
            {
              Vector2 vector2_3 = vector2_1 - Game1.player.getTileLocation();
              if ((double) vector2_3.X > 0.0 || (double) vector2_3.Y > 0.0)
                vector2_3.Normalize();
              if ((double) Vector2.Dot(Utility.DirectionsTileVectors[Game1.player.FacingDirection], vector2_3) >= 0.0 && Game1.tryToCheckAt(vector2_1, Game1.player))
                return false;
            }
            if (Game1.player.ActiveObject != null && Game1.player.ActiveObject is Furniture && !Game1.eventUp)
            {
              (Game1.player.ActiveObject as Furniture).rotate();
              Game1.playSound("dwoop");
              Game1.oldKBState = currentKBState;
              Game1.oldMouseState = Game1.input.GetMouseState();
              Game1.oldPadState = currentPadState;
              return false;
            }
            vector2_1.Y -= 2f;
            if (Game1.player.FacingDirection >= 0 && Game1.player.FacingDirection <= 3 && !flag1)
            {
              Vector2 vector2_4 = vector2_1 - Game1.player.getTileLocation();
              if ((double) vector2_4.X > 0.0 || (double) vector2_4.Y > 0.0)
                vector2_4.Normalize();
              if ((double) Vector2.Dot(Utility.DirectionsTileVectors[Game1.player.FacingDirection], vector2_4) >= 0.0 && Game1.tryToCheckAt(vector2_1, Game1.player))
                return false;
            }
            if (Game1.player.ActiveObject != null && Game1.player.ActiveObject is Furniture && !Game1.eventUp)
            {
              (Game1.player.ActiveObject as Furniture).rotate();
              Game1.playSound("dwoop");
              Game1.oldKBState = currentKBState;
              Game1.oldMouseState = Game1.input.GetMouseState();
              Game1.oldPadState = currentPadState;
              return false;
            }
            if (Game1.tryToCheckAt(Game1.player.getTileLocation(), Game1.player))
              return false;
            if (Game1.player.ActiveObject != null && Game1.player.ActiveObject is Furniture && !Game1.eventUp)
            {
              (Game1.player.ActiveObject as Furniture).rotate();
              Game1.playSound("dwoop");
              Game1.oldKBState = currentKBState;
              Game1.oldMouseState = Game1.input.GetMouseState();
              Game1.oldPadState = currentPadState;
              return false;
            }
          }
          if (!Game1.player.isEating && Game1.player.ActiveObject != null && !Game1.dialogueUp && !Game1.eventUp && !Game1.player.canOnlyWalk && !Game1.player.FarmerSprite.PauseForSingleAnimation && !Game1.fadeToBlack && Game1.player.ActiveObject.Edibility != -300 && Game1.didPlayerJustRightClick(true))
          {
            if (Game1.player.team.SpecialOrderRuleActive("SC_NO_FOOD") && Game1.player.currentLocation is MineShaft && (Game1.player.currentLocation as MineShaft).getMineArea() == 121)
            {
              Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"), 3));
              return false;
            }
            if (Game1.buffsDisplay.hasBuff(25) && Game1.player.ActiveObject != null && !Game1.player.ActiveObject.HasContextTag("ginger_item"))
            {
              Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Nauseous_CantEat"), 3));
              return false;
            }
            Game1.player.faceDirection(2);
            Game1.player.itemToEat = (Item) Game1.player.ActiveObject;
            Game1.player.FarmerSprite.setCurrentSingleAnimation(304);
            Game1.currentLocation.createQuestionDialogue(Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) Game1.player.ActiveObject.parentSheetIndex].Split('/').Length <= 6 || !Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) Game1.player.ActiveObject.parentSheetIndex].Split('/')[6].Equals("drink") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3160", (object) Game1.player.ActiveObject.DisplayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3159", (object) Game1.player.ActiveObject.DisplayName), Game1.currentLocation.createYesNoResponses(), "Eat");
            Game1.oldKBState = currentKBState;
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldPadState = currentPadState;
            return false;
          }
        }
        else
        {
          if (Game1.CurrentEvent != null)
            Game1.CurrentEvent.receiveActionPress((int) vector2_1.X, (int) vector2_1.Y);
          Game1.oldKBState = currentKBState;
          Game1.oldMouseState = Game1.input.GetMouseState();
          Game1.oldPadState = currentPadState;
          return false;
        }
      }
      else if (Game1.numberOfSelectedItems != -1)
      {
        Game1.tryToBuySelectedItems();
        Game1.playSound("smallSelect");
        Game1.oldKBState = currentKBState;
        Game1.oldMouseState = Game1.input.GetMouseState();
        Game1.oldPadState = currentPadState;
        return false;
      }
      if (Game1.player.CurrentTool == null || !(Game1.player.CurrentTool is MeleeWeapon) || !Game1.player.CanMove || Game1.player.canOnlyWalk || Game1.eventUp || (bool) (NetFieldBase<bool, NetBool>) Game1.player.onBridge || !Game1.didPlayerJustRightClick(true))
        return true;
      ((MeleeWeapon) Game1.player.CurrentTool).animateSpecialMove(Game1.player);
      return false;
    }

    public static bool IsPerformingMousePlacement() => (double) Game1.mouseCursorTransparency != 0.0 && Game1.wasMouseVisibleThisFrame && (Game1.lastCursorMotionWasMouse || Game1.player.ActiveObject != null && (Game1.player.ActiveObject.isPlaceable() || Game1.player.ActiveObject.Category == -74 || Game1.player.ActiveObject.isSapling()));

    public static Vector2 GetPlacementGrabTile() => !Game1.IsPerformingMousePlacement() ? Game1.player.GetGrabTile() : new Vector2((float) (Game1.getOldMouseX() + Game1.viewport.X), (float) (Game1.getOldMouseY() + Game1.viewport.Y)) / 64f;

    public static bool tryToCheckAt(Vector2 grabTile, Farmer who)
    {
      if (Game1.player.onBridge.Value)
        return false;
      Game1.haltAfterCheck = true;
      if (!Utility.tileWithinRadiusOfPlayer((int) grabTile.X, (int) grabTile.Y, 1, Game1.player) || !Game1.hooks.OnGameLocation_CheckAction(Game1.currentLocation, new Location((int) grabTile.X, (int) grabTile.Y), Game1.viewport, who, (Func<bool>) (() => Game1.currentLocation.checkAction(new Location((int) grabTile.X, (int) grabTile.Y), Game1.viewport, who))))
        return false;
      Game1.updateCursorTileHint();
      who.lastGrabTile = grabTile;
      if (who.CanMove && Game1.haltAfterCheck)
      {
        who.faceGeneralDirection(grabTile * 64f);
        who.Halt();
      }
      Game1.oldKBState = Game1.GetKeyboardState();
      Game1.oldMouseState = Game1.input.GetMouseState();
      Game1.oldPadState = Game1.input.GetGamePadState();
      return true;
    }

    public static void pressSwitchToolButton()
    {
      if (Game1.player.netItemStowed.Value)
      {
        Game1.player.netItemStowed.Set(false);
        Game1.player.UpdateItemStow();
      }
      MouseState mouseState = Game1.input.GetMouseState();
      int num1;
      if (mouseState.ScrollWheelValue <= Game1.oldMouseState.ScrollWheelValue)
      {
        mouseState = Game1.input.GetMouseState();
        num1 = mouseState.ScrollWheelValue < Game1.oldMouseState.ScrollWheelValue ? 1 : 0;
      }
      else
        num1 = -1;
      int num2 = num1;
      if (Game1.options.gamepadControls && num2 == 0)
      {
        GamePadState gamePadState = Game1.input.GetGamePadState();
        if (gamePadState.IsButtonDown(Buttons.LeftTrigger))
        {
          num2 = -1;
        }
        else
        {
          gamePadState = Game1.input.GetGamePadState();
          if (gamePadState.IsButtonDown(Buttons.RightTrigger))
            num2 = 1;
        }
      }
      if (Game1.options.invertScrollDirection)
        num2 *= -1;
      if (num2 == 0)
        return;
      Game1.player.CurrentToolIndex = (Game1.player.CurrentToolIndex + num2) % 12;
      if (Game1.player.CurrentToolIndex < 0)
        Game1.player.CurrentToolIndex = 11;
      for (int index = 0; index < 12 && Game1.player.CurrentItem == null; ++index)
      {
        Game1.player.CurrentToolIndex = (num2 + Game1.player.CurrentToolIndex) % 12;
        if (Game1.player.CurrentToolIndex < 0)
          Game1.player.CurrentToolIndex = 11;
      }
      Game1.playSound("toolSwap");
      if (Game1.player.ActiveObject != null)
        Game1.player.showCarrying();
      else
        Game1.player.showNotCarrying();
      if (Game1.player.CurrentTool == null || Game1.player.CurrentTool.Name.Equals("Seeds") || Game1.player.CurrentTool.Name.Contains("Sword") || (bool) (NetFieldBase<bool, NetBool>) Game1.player.CurrentTool.instantUse)
        return;
      Game1.player.CurrentTool.CurrentParentTileIndex = Game1.player.CurrentTool.CurrentParentTileIndex - Game1.player.CurrentTool.CurrentParentTileIndex % 8 + 2;
    }

    public static void switchToolAnimation()
    {
      Game1.pickToolInterval = 0.0f;
      Game1.player.CanMove = false;
      Game1.pickingTool = true;
      Game1.playSound("toolSwap");
      switch (Game1.player.FacingDirection)
      {
        case 0:
          Game1.player.FarmerSprite.setCurrentFrame(196);
          break;
        case 1:
          Game1.player.FarmerSprite.setCurrentFrame(194);
          break;
        case 2:
          Game1.player.FarmerSprite.setCurrentFrame(192);
          break;
        case 3:
          Game1.player.FarmerSprite.setCurrentFrame(198);
          break;
      }
      if (Game1.player.CurrentTool != null && !Game1.player.CurrentTool.Name.Equals("Seeds") && !Game1.player.CurrentTool.Name.Contains("Sword") && !(bool) (NetFieldBase<bool, NetBool>) Game1.player.CurrentTool.instantUse)
        Game1.player.CurrentTool.CurrentParentTileIndex = Game1.player.CurrentTool.CurrentParentTileIndex - Game1.player.CurrentTool.CurrentParentTileIndex % 8 + 2;
      if (Game1.player.ActiveObject == null)
        return;
      Game1.player.showCarrying();
    }

    public static bool pressUseToolButton()
    {
      bool initiateItemStow = Game1.game1._didInitiateItemStow;
      Game1.game1._didInitiateItemStow = false;
      if (Game1.fadeToBlack)
        return false;
      Game1.player.toolPower = 0;
      Game1.player.toolHold = 0;
      bool flag = false;
      if (Game1.player.CurrentTool == null && Game1.player.ActiveObject == null)
      {
        Vector2 key = Game1.player.GetToolLocation() / 64f;
        key.X = (float) (int) key.X;
        key.Y = (float) (int) key.Y;
        if (Game1.currentLocation.Objects.ContainsKey(key))
        {
          Object @object = Game1.currentLocation.Objects[key];
          if (!(bool) (NetFieldBase<bool, NetBool>) @object.readyForHarvest && @object.heldObject.Value == null && !(@object is Fence) && !(@object is CrabPot) && (NetFieldBase<string, NetString>) @object.type != (NetString) null && (@object.type.Equals((object) "Crafting") || @object.type.Equals((object) "interactive")) && !@object.name.Equals("Twig"))
          {
            flag = true;
            @object.setHealth(@object.getHealth() - 1);
            @object.shakeTimer = 300;
            Game1.currentLocation.playSound("hammer");
            if (@object.getHealth() < 2)
            {
              Game1.currentLocation.playSound("hammer");
              if (@object.getHealth() < 1)
              {
                Tool t = (Tool) new Pickaxe();
                t.DoFunction(Game1.currentLocation, -1, -1, 0, Game1.player);
                if (@object.performToolAction(t, Game1.currentLocation))
                {
                  @object.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) @object.tileLocation, Game1.currentLocation);
                  if (@object.type.Equals((object) "Crafting") && (int) (NetFieldBase<int, NetInt>) @object.fragility != 2)
                    Game1.currentLocation.debris.Add(new Debris((bool) (NetFieldBase<bool, NetBool>) @object.bigCraftable ? -@object.ParentSheetIndex : @object.ParentSheetIndex, Game1.player.GetToolLocation(), new Vector2((float) Game1.player.GetBoundingBox().Center.X, (float) Game1.player.GetBoundingBox().Center.Y)));
                  Game1.currentLocation.Objects.Remove(key);
                  return true;
                }
              }
            }
          }
        }
      }
      if (Game1.currentMinigame == null && !Game1.player.UsingTool && (Game1.player.IsSitting() || Game1.player.isRidingHorse() || Game1.player.onBridge.Value || Game1.dialogueUp || Game1.eventUp && !Game1.CurrentEvent.canPlayerUseTool() && (!Game1.currentLocation.currentEvent.playerControlSequence || Game1.activeClickableMenu == null && Game1.currentMinigame == null) || Game1.player.CurrentTool != null && Game1.currentLocation.doesPositionCollideWithCharacter(Utility.getRectangleCenteredAt(Game1.player.GetToolLocation(), 64), true) != null && Game1.currentLocation.doesPositionCollideWithCharacter(Utility.getRectangleCenteredAt(Game1.player.GetToolLocation(), 64), true).isVillager()))
      {
        Game1.pressActionButton(Game1.GetKeyboardState(), Game1.input.GetMouseState(), Game1.input.GetGamePadState());
        return false;
      }
      if (Game1.player.canOnlyWalk)
        return true;
      Vector2 position = !Game1.wasMouseVisibleThisFrame ? Game1.player.GetToolLocation() : new Vector2((float) (Game1.getOldMouseX() + Game1.viewport.X), (float) (Game1.getOldMouseY() + Game1.viewport.Y));
      if (Utility.canGrabSomethingFromHere((int) position.X, (int) position.Y, Game1.player))
      {
        Vector2 tile = new Vector2(position.X / 64f, position.Y / 64f);
        if (Game1.hooks.OnGameLocation_CheckAction(Game1.currentLocation, new Location((int) tile.X, (int) tile.Y), Game1.viewport, Game1.player, (Func<bool>) (() => Game1.currentLocation.checkAction(new Location((int) tile.X, (int) tile.Y), Game1.viewport, Game1.player))))
        {
          Game1.updateCursorTileHint();
          return true;
        }
        if (!Game1.currentLocation.terrainFeatures.ContainsKey(tile))
          return false;
        Game1.currentLocation.terrainFeatures[tile].performUseAction(tile, Game1.currentLocation);
        return true;
      }
      if (Game1.currentLocation.leftClick((int) position.X, (int) position.Y, Game1.player))
        return true;
      Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
      if (Game1.player.ActiveObject != null)
      {
        if (Game1.options.allowStowing && Game1.CanPlayerStowItem(Game1.GetPlacementGrabTile()))
        {
          if (!(Game1.didPlayerJustLeftClick() | initiateItemStow))
            return true;
          Game1.game1._didInitiateItemStow = true;
          Game1.playSound("stoneStep");
          Game1.player.netItemStowed.Set(true);
          return true;
        }
        if (Utility.withinRadiusOfPlayer((int) position.X, (int) position.Y, 1, Game1.player) && Game1.hooks.OnGameLocation_CheckAction(Game1.currentLocation, new Location((int) position.X / 64, (int) position.Y / 64), Game1.viewport, Game1.player, (Func<bool>) (() => Game1.currentLocation.checkAction(new Location((int) position.X / 64, (int) position.Y / 64), Game1.viewport, Game1.player))))
          return true;
        Vector2 placementGrabTile = Game1.GetPlacementGrabTile();
        Vector2 placementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, Game1.currentLocation, (Item) Game1.player.ActiveObject, (int) placementGrabTile.X * 64, (int) placementGrabTile.Y * 64);
        if (Utility.tryToPlaceItem(Game1.currentLocation, (Item) Game1.player.ActiveObject, (int) placementPosition.X, (int) placementPosition.Y))
        {
          Game1.isCheckingNonMousePlacement = false;
          return true;
        }
        Game1.isCheckingNonMousePlacement = false;
      }
      if (Game1.currentLocation.LowPriorityLeftClick((int) position.X, (int) position.Y, Game1.player))
        return true;
      if (Game1.options.allowStowing && Game1.player.netItemStowed.Value && !flag && (initiateItemStow || Game1.didPlayerJustLeftClick(true)))
      {
        Game1.game1._didInitiateItemStow = true;
        Game1.playSound("toolSwap");
        Game1.player.netItemStowed.Set(false);
        return true;
      }
      if (Game1.player.UsingTool)
      {
        Game1.player.lastClick = new Vector2((float) (int) position.X, (float) (int) position.Y);
        Game1.player.CurrentTool.DoFunction(Game1.player.currentLocation, (int) Game1.player.lastClick.X, (int) Game1.player.lastClick.Y, 1, Game1.player);
        return true;
      }
      if (Game1.player.ActiveObject == null && !Game1.player.isEating && Game1.player.CurrentTool != null)
      {
        if ((double) Game1.player.Stamina <= 20.0 && Game1.player.CurrentTool != null && !(Game1.player.CurrentTool is MeleeWeapon) && !Game1.eventUp)
        {
          Game1.staminaShakeTimer = 1000;
          for (int index = 0; index < 4; ++index)
            Game1.uiOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(366, 412, 5, 6), new Vector2((float) (Game1.random.Next(32) + Game1.uiViewport.Width - 56), (float) (Game1.uiViewport.Height - 224 - 16 - (int) ((double) (Game1.player.MaxStamina - 270) * 0.715))), false, 0.012f, Microsoft.Xna.Framework.Color.SkyBlue)
            {
              motion = new Vector2(-2f, -10f),
              acceleration = new Vector2(0.0f, 0.5f),
              local = true,
              scale = (float) (4 + Game1.random.Next(-1, 0)),
              delayBeforeAnimationStart = index * 30
            });
        }
        if (Game1.player.CurrentTool == null || !(Game1.player.CurrentTool is MeleeWeapon) || Game1.didPlayerJustLeftClick(true))
        {
          int facingDirection = Game1.player.FacingDirection;
          Vector2 toolLocation = Game1.player.GetToolLocation(position);
          Game1.player.FacingDirection = Game1.player.getGeneralDirectionTowards(new Vector2((float) (int) toolLocation.X, (float) (int) toolLocation.Y));
          Game1.player.lastClick = new Vector2((float) (int) position.X, (float) (int) position.Y);
          Game1.player.BeginUsingTool();
          if (!(bool) (NetFieldBase<bool, NetBool>) Game1.player.usingTool)
            Game1.player.FacingDirection = facingDirection;
          else if (Game1.player.FarmerSprite.IsPlayingBasicAnimation(facingDirection, true) || Game1.player.FarmerSprite.IsPlayingBasicAnimation(facingDirection, false))
            Game1.player.FarmerSprite.StopAnimation();
        }
      }
      return false;
    }

    public static bool CanPlayerStowItem(Vector2 position)
    {
      if (Game1.player.ActiveObject == null || (bool) (NetFieldBase<bool, NetBool>) Game1.player.ActiveObject.bigCraftable || Game1.player.ActiveObject is Furniture)
        return false;
      if (Game1.player.ActiveObject != null && (Game1.player.ActiveObject.Category == -74 || Game1.player.ActiveObject.Category == -19))
      {
        Vector2 placementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, Game1.currentLocation, (Item) Game1.player.ActiveObject, (int) position.X * 64, (int) position.Y * 64);
        if (Utility.playerCanPlaceItemHere(Game1.player.currentLocation, (Item) Game1.player.ActiveObject, (int) placementPosition.X, (int) placementPosition.Y, Game1.player) && (!Object.isWildTreeSeed(Game1.player.ActiveObject.ParentSheetIndex) && !Game1.player.ActiveObject.isSapling() || Game1.IsPerformingMousePlacement()))
          return false;
      }
      return true;
    }

    public static int getMouseXRaw() => Game1.input.GetMouseState().X;

    public static int getMouseYRaw() => Game1.input.GetMouseState().Y;

    public static bool IsOnMainThread() => Thread.CurrentThread != null && !Thread.CurrentThread.IsBackground;

    public static void PushUIMode()
    {
      if (!Game1.IsOnMainThread())
        return;
      ++Game1.uiModeCount;
      if (Game1.uiModeCount <= 0 || Game1.uiMode)
        return;
      Game1.uiMode = true;
      if (Game1.game1.isDrawing && Game1.IsOnMainThread())
      {
        if (Game1.game1.uiScreen != null && !Game1.game1.uiScreen.IsDisposed)
        {
          RenderTargetBinding[] renderTargets = Game1.graphics.GraphicsDevice.GetRenderTargets();
          Game1.nonUIRenderTarget = renderTargets.Length == 0 ? (RenderTarget2D) null : renderTargets[0].RenderTarget as RenderTarget2D;
          Game1.SetRenderTarget(Game1.game1.uiScreen);
        }
        if (Game1.isRenderingScreenBuffer)
          Game1.SetRenderTarget((RenderTarget2D) null);
      }
      Game1.uiViewport = new xTile.Dimensions.Rectangle(0, 0, (int) Math.Ceiling((double) Game1.viewport.Width * (double) Game1.options.zoomLevel / (double) Game1.options.uiScale), (int) Math.Ceiling((double) Game1.viewport.Height * (double) Game1.options.zoomLevel / (double) Game1.options.uiScale))
      {
        X = Game1.viewport.X,
        Y = Game1.viewport.Y
      };
    }

    public static void PopUIMode()
    {
      if (!Game1.IsOnMainThread())
        return;
      --Game1.uiModeCount;
      if (Game1.uiModeCount > 0 || !Game1.uiMode)
        return;
      if (Game1.game1.isDrawing)
      {
        if (Game1.graphics.GraphicsDevice.GetRenderTargets().Length != 0 && Game1.graphics.GraphicsDevice.GetRenderTargets()[0].RenderTarget == Game1.game1.uiScreen)
        {
          if (Game1.nonUIRenderTarget != null && !Game1.nonUIRenderTarget.IsDisposed)
            Game1.SetRenderTarget(Game1.nonUIRenderTarget);
          else
            Game1.SetRenderTarget((RenderTarget2D) null);
        }
        if (Game1.isRenderingScreenBuffer)
          Game1.SetRenderTarget((RenderTarget2D) null);
      }
      Game1.nonUIRenderTarget = (RenderTarget2D) null;
      Game1.uiMode = false;
    }

    public static void SetRenderTarget(RenderTarget2D target)
    {
      if (Game1.isRenderingScreenBuffer || !Game1.IsOnMainThread())
        return;
      Game1.graphics.GraphicsDevice.SetRenderTarget(target);
    }

    public static void InUIMode(Action action)
    {
      Game1.PushUIMode();
      try
      {
        action();
      }
      finally
      {
        Game1.PopUIMode();
      }
    }

    public static void StartWorldDrawInUI(SpriteBatch b)
    {
      Game1._oldUIModeCount = 0;
      if (!Game1.uiMode)
        return;
      Game1._oldUIModeCount = Game1.uiModeCount;
      b?.End();
      while (Game1.uiModeCount > 0)
        Game1.PopUIMode();
      b?.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    }

    public static void EndWorldDrawInUI(SpriteBatch b)
    {
      if (Game1._oldUIModeCount > 0)
      {
        b?.End();
        for (int index = 0; index < Game1._oldUIModeCount; ++index)
          Game1.PushUIMode();
        b?.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      }
      Game1._oldUIModeCount = 0;
    }

    public static int getMouseX() => Game1.getMouseX(Game1.uiMode);

    public static int getMouseX(bool ui_scale) => ui_scale ? (int) ((double) Game1.input.GetMouseState().X / (double) Game1.options.uiScale) : (int) ((double) Game1.input.GetMouseState().X * (1.0 / (double) Game1.options.zoomLevel));

    public static int getOldMouseX() => Game1.getOldMouseX(Game1.uiMode);

    public static int getOldMouseX(bool ui_scale) => ui_scale ? (int) ((double) Game1.oldMouseState.X / (double) Game1.options.uiScale) : (int) ((double) Game1.oldMouseState.X * (1.0 / (double) Game1.options.zoomLevel));

    public static int getMouseY() => Game1.getMouseY(Game1.uiMode);

    public static int getMouseY(bool ui_scale) => ui_scale ? (int) ((double) Game1.input.GetMouseState().Y / (double) Game1.options.uiScale) : (int) ((double) Game1.input.GetMouseState().Y * (1.0 / (double) Game1.options.zoomLevel));

    public static int getOldMouseY() => Game1.getOldMouseY(Game1.uiMode);

    public static int getOldMouseY(bool ui_scale) => ui_scale ? (int) ((double) Game1.oldMouseState.Y / (double) Game1.options.uiScale) : (int) ((double) Game1.oldMouseState.Y * (1.0 / (double) Game1.options.zoomLevel));

    public static void pressAddItemToInventoryButton()
    {
    }

    public static int numberOfPlayers() => Game1._onlineFarmers.Count;

    public static bool isFestival() => Game1.currentLocation != null && Game1.currentLocation.currentEvent != null && Game1.currentLocation.currentEvent.isFestival;

    public bool parseDebugInput(string debugInput)
    {
      Game1.lastDebugInput = debugInput;
      debugInput = debugInput.Trim();
      string[] debugSplit = debugInput.Split(' ');
      try
      {
        if (Game1.panMode)
        {
          if (debugSplit[0].Equals("exit") || debugSplit[0].ToLower().Equals("panmode"))
          {
            Game1.panMode = false;
            Game1.viewportFreeze = false;
            this.panModeString = "";
            Game1.debugMode = false;
            Game1.debugOutput = "";
            this.panFacingDirectionWait = false;
            Game1.inputSimulator = (IInputSimulator) null;
            return true;
          }
          if (debugSplit[0].Equals("clear"))
          {
            this.panModeString = "";
            Game1.debugOutput = "";
            this.panFacingDirectionWait = false;
            return true;
          }
          if (this.panFacingDirectionWait)
            return false;
          int result = 0;
          if (int.TryParse(debugSplit[0], out result))
          {
            this.panModeString = this.panModeString + (this.panModeString.Length > 0 ? "/" : "") + result.ToString() + " ";
            Game1.debugOutput = this.panModeString + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3191");
          }
          return true;
        }
        switch (debugSplit[0].ToLowerInvariant())
        {
          case "achieve":
            Program.sdk.GetAchievement(debugSplit[1]);
            goto case "break";
          case "achievement":
            Game1.getAchievement(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "addallcrafting":
            using (Dictionary<string, string>.KeyCollection.Enumerator enumerator = CraftingRecipe.craftingRecipes.Keys.GetEnumerator())
            {
              while (enumerator.MoveNext())
                Game1.player.craftingRecipes.Add(enumerator.Current, 0);
              goto case "break";
            }
          case "addhour":
            Game1.addHour();
            goto case "break";
          case "addjunimo":
          case "aj":
          case "j":
            (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).addCharacter((NPC) new Junimo(new Vector2((float) Convert.ToInt32(debugSplit[1]), (float) Convert.ToInt32(debugSplit[2])) * 64f, Convert.ToInt32(debugSplit[3])));
            goto case "break";
          case "addkent":
            Game1.addKentIfNecessary();
            goto case "break";
          case "addminute":
            Game1.addMinute();
            goto case "break";
          case "addotherfarmer":
            Farmer owner = new Farmer(new FarmerSprite("Characters\\Farmer\\farmer_base"), new Vector2(Game1.player.Position.X - 64f, Game1.player.Position.Y), 2, Dialogue.randomName(), (List<Item>) null, true);
            owner.changeShirt(Game1.random.Next(40));
            owner.changePants(new Microsoft.Xna.Framework.Color(Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue)));
            owner.changeHairStyle(Game1.random.Next(FarmerRenderer.hairStylesTexture.Height / 96 * 8));
            if (Game1.random.NextDouble() < 0.5)
              owner.changeHat(Game1.random.Next(-1, FarmerRenderer.hatsTexture.Height / 80 * 12));
            else
              Game1.player.changeHat(-1);
            owner.changeHairColor(new Microsoft.Xna.Framework.Color(Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue)));
            owner.changeSkinColor(Game1.random.Next(16));
            owner.FarmerSprite.setOwner(owner);
            owner.currentLocation = Game1.currentLocation;
            Game1.otherFarmers.Add((long) Game1.random.Next(), owner);
            goto case "break";
          case "addquartz":
            if (debugSplit.Length > 1)
            {
              for (int index = 0; index < Convert.ToInt32(debugSplit[1]) - 1; ++index)
              {
                Vector2 randomTile = Game1.getFarm().getRandomTile();
                if (!Game1.getFarm().terrainFeatures.ContainsKey(randomTile))
                  Game1.getFarm().terrainFeatures.Add(randomTile, (TerrainFeature) new Quartz(1 + Game1.random.Next(2), Utility.getRandomRainbowColor()));
              }
              goto case "break";
            }
            else
              goto case "break";
          case "al":
          case "ambientlight":
            Game1.ambientLight = new Microsoft.Xna.Framework.Color(Convert.ToInt32(debugSplit[1]), Convert.ToInt32(debugSplit[2]), Convert.ToInt32(debugSplit[3]));
            goto case "break";
          case "allbundles":
            foreach (KeyValuePair<int, NetArray<bool, NetBool>> keyValuePair in (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).bundles.FieldDict)
            {
              for (int index = 0; index < keyValuePair.Value.Count; ++index)
                keyValuePair.Value[index] = true;
            }
            Game1.playSound("crystal");
            goto case "break";
          case "allmail":
            using (Dictionary<string, string>.KeyCollection.Enumerator enumerator = Game1.content.Load<Dictionary<string, string>>("Data\\mail").Keys.GetEnumerator())
            {
              while (enumerator.MoveNext())
                Game1.addMailForTomorrow(enumerator.Current);
              goto case "break";
            }
          case "allmailread":
            using (Dictionary<string, string>.KeyCollection.Enumerator enumerator = Game1.content.Load<Dictionary<string, string>>("Data\\mail").Keys.GetEnumerator())
            {
              while (enumerator.MoveNext())
                Game1.player.mailReceived.Add(enumerator.Current);
              goto case "break";
            }
          case "animal":
            Utility.addAnimalToFarm(new FarmAnimal(debugInput.Substring(debugInput.IndexOf(' ')).Trim(), Game1.multiplayer.getNewID(), Game1.player.UniqueMultiplayerID));
            goto case "break";
          case "animalinfo":
            Game1.showGlobalMessage(Game1.getFarm().getAllFarmAnimals().Count.ToString() ?? "");
            goto case "break";
          case "animationpreviewtool":
          case "apt":
            Game1.activeClickableMenu = (IClickableMenu) new AnimationPreviewTool();
            goto case "break";
          case "ax":
            Game1.player.addItemToInventoryBool((Item) new Axe());
            Game1.playSound("coin");
            goto case "break";
          case "b":
          case "bi":
          case "big":
          case "bigitem":
            if (Game1.bigCraftablesInformation.ContainsKey(Convert.ToInt32(debugSplit[1])))
            {
              Game1.playSound("coin");
              Farmer player = Game1.player;
              Object @object = new Object(Vector2.Zero, Convert.ToInt32(debugSplit[1]));
              @object.Stack = ((IEnumerable<string>) debugSplit).Count<string>() > 2 ? Convert.ToInt32(debugSplit[2]) : 1;
              player.addItemToInventory((Item) @object);
              goto case "break";
            }
            else
              goto case "break";
          case "backpack":
            Game1.player.increaseBackpackSize(Math.Min(36 - Game1.player.items.Count<Item>(), Convert.ToInt32(debugSplit[1])));
            goto case "break";
          case "barn":
          case "upgradebarn":
            Game1.player.BarnUpgradeLevel = Math.Min(3, Game1.player.BarnUpgradeLevel + 1);
            Game1.removeFrontLayerForFarmBuildings();
            Game1.addNewFarmBuildingMaps();
            goto case "break";
          case "bc":
          case "buildcoop":
            Game1.getFarm().buildStructure(new BluePrint("Coop"), new Vector2((float) Convert.ToInt32(debugSplit[1]), (float) Convert.ToInt32(debugSplit[2])), Game1.player);
            Game1.getFarm().buildings.Last<Building>().daysOfConstructionLeft.Value = 0;
            goto case "break";
          case "beachbridge":
            (Game1.getLocationFromName("Beach") as Beach).bridgeFixed.Value = !(bool) (NetFieldBase<bool, NetBool>) (Game1.getLocationFromName("Beach") as Beach).bridgeFixed;
            if (!(bool) (NetFieldBase<bool, NetBool>) (Game1.getLocationFromName("Beach") as Beach).bridgeFixed)
            {
              (Game1.getLocationFromName("Beach") as Beach).setMapTile(58, 13, 284, "Buildings", (string) null, 1);
              goto case "break";
            }
            else
              goto case "break";
          case "befriendanimals":
            if (Game1.currentLocation is AnimalHouse)
            {
              using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.ValuesCollection.Enumerator enumerator = (Game1.currentLocation as AnimalHouse).animals.Values.GetEnumerator())
              {
                while (enumerator.MoveNext())
                  enumerator.Current.friendshipTowardFarmer.Value = debugSplit.Length > 1 ? Convert.ToInt32(debugSplit[1]) : 1000;
                goto case "break";
              }
            }
            else
              goto case "break";
          case "bluebook":
            Game1.player.items.Add((Item) new Blueprints());
            goto case "break";
          case "blueprint":
            Game1.player.blueprints.Add(debugInput.Substring(debugInput.IndexOf(' ')).Trim());
            goto case "break";
          case "boatjourney":
            Game1.currentMinigame = (IMinigame) new BoatJourney();
            goto case "break";
          case "boots":
            Game1.player.addItemToInventoryBool((Item) new Boots(Convert.ToInt32(debugSplit[1])));
            Game1.playSound("coin");
            goto case "break";
          case "bpm":
            Building buildingAt = Game1.getFarm().getBuildingAt(Game1.player.getTileLocation() + new Vector2(0.0f, -1f));
            if (buildingAt != null)
            {
              Game1.activeClickableMenu = (IClickableMenu) new BuildingPaintMenu(buildingAt);
              goto case "break";
            }
            else
            {
              Farm farm_location = Game1.getFarm();
              Game1.activeClickableMenu = (IClickableMenu) new BuildingPaintMenu("House", (Func<Texture2D>) (() => farm_location.paintedHouseTexture != null ? farm_location.paintedHouseTexture : Farm.houseTextures), farm_location.houseSource.Value, farm_location.housePaintColor.Value);
              goto case "break";
            }
          case "break":
label_1159:
            return true;
          case "broadcastmail":
            if (debugSplit.Length > 1)
            {
              Game1.addMailForTomorrow(string.Join(" ", ((IEnumerable<string>) debugSplit).Skip<string>(1)), sendToEveryone: true);
              goto case "break";
            }
            else
              goto case "break";
          case "broadcastmailbox":
            Game1.addMail(debugSplit[1], sendToEveryone: true);
            goto case "break";
          case "buff":
            Game1.buffsDisplay.addOtherBuff(new Buff(Convert.ToInt32(debugSplit[1])));
            goto case "break";
          case "build":
            Game1.getFarm().buildStructure(new BluePrint(debugSplit[1].Replace('9', ' ')), debugSplit.Length > 3 ? new Vector2((float) Convert.ToInt32(debugSplit[2]), (float) Convert.ToInt32(debugSplit[3])) : new Vector2((float) (Game1.player.getTileX() + 1), (float) Game1.player.getTileY()), Game1.player);
            Game1.getFarm().buildings.Last<Building>().daysOfConstructionLeft.Value = 0;
            goto case "break";
          case "bundle":
            CommunityCenter locationFromName1 = Game1.getLocationFromName("CommunityCenter") as CommunityCenter;
            int int32_1 = Convert.ToInt32(debugSplit[1]);
            foreach (KeyValuePair<int, NetArray<bool, NetBool>> keyValuePair in locationFromName1.bundles.FieldDict)
            {
              if (keyValuePair.Key == int32_1)
              {
                for (int index = 0; index < keyValuePair.Value.Count; ++index)
                  keyValuePair.Value[index] = true;
              }
            }
            Game1.playSound("crystal");
            goto case "break";
          case "busdriveback":
            (Game1.getLocationFromName("BusStop") as BusStop).busDriveBack();
            goto case "break";
          case "busdriveoff":
            (Game1.getLocationFromName("BusStop") as BusStop).busDriveOff();
            goto case "break";
          case "c":
          case "canmove":
          case "cm":
            Game1.player.isEating = false;
            Game1.player.CanMove = true;
            Game1.player.UsingTool = false;
            Game1.player.usingSlingshot = false;
            Game1.player.FarmerSprite.PauseForSingleAnimation = false;
            if (Game1.player.CurrentTool is FishingRod)
              (Game1.player.CurrentTool as FishingRod).isFishing = false;
            if (Game1.player.mount != null)
            {
              Game1.player.mount.dismount();
              goto case "break";
            }
            else
              goto case "break";
          case "can":
          case "wateringcan":
            Game1.player.addItemToInventoryBool((Item) new WateringCan());
            Game1.playSound("coin");
            goto case "break";
          case "cat":
            Game1.currentLocation.characters.Add((NPC) new Cat(Convert.ToInt32(debugSplit[1]), Convert.ToInt32(debugSplit[2]), ((IEnumerable<string>) debugSplit).Count<string>() > 3 ? Convert.ToInt32(debugSplit[3]) : 0));
            goto case "break";
          case "caughtfish":
          case "fishcaught":
            Game1.stats.FishCaught = (uint) Convert.ToInt32(debugSplit[1]);
            goto case "break";
          case "ccload":
            (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).loadArea(Convert.ToInt32(debugSplit[1]));
            (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).markAreaAsComplete(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "ccloadcutscene":
            (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).restoreAreaCutscene(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "changestat":
            Game1.stats.stat_dictionary[debugSplit[1]] = Convert.ToUInt32(debugSplit[2]);
            goto case "break";
          case "changewallet":
            if (Game1.IsMasterGame)
            {
              Game1.player.changeWalletTypeTonight.Value = true;
              goto case "break";
            }
            else
              goto case "break";
          case "characterinfo":
            Game1.showGlobalMessage(Game1.currentLocation.characters.Count.ToString() + " characters on this map");
            goto case "break";
          case "child":
          case "kid":
            if (Game1.player.getChildren().Count > 0)
            {
              ++Game1.player.getChildren()[0].Age;
              Game1.player.getChildren()[0].reloadSprite();
              goto case "break";
            }
            else
            {
              (Game1.getLocationFromName("FarmHouse") as FarmHouse).characters.Add((NPC) new Child("Baby", Game1.random.NextDouble() < 0.5, Game1.random.NextDouble() < 0.5, Game1.player));
              goto case "break";
            }
          case "child2":
            if (Game1.player.getChildrenCount() > 1)
            {
              ++Game1.player.getChildren()[1].Age;
              Game1.player.getChildren()[1].reloadSprite();
              goto case "break";
            }
            else
            {
              (Game1.getLocationFromName("FarmHouse") as FarmHouse).characters.Add((NPC) new Child("Baby2", Game1.random.NextDouble() < 0.5, Game1.random.NextDouble() < 0.5, Game1.player));
              goto case "break";
            }
          case "ci":
          case "clear":
            Game1.player.clearBackpack();
            goto case "break";
          case "clearbuffs":
            Game1.player.ClearBuffs();
            goto case "break";
          case "clearcharacters":
            Game1.currentLocation.characters.Clear();
            goto case "break";
          case "clearchildren":
            Game1.player.getRidOfChildren();
            goto case "break";
          case "clearfarm":
            for (int x = 0; x < Game1.getFarm().map.Layers[0].LayerWidth; ++x)
            {
              for (int y = 0; y < Game1.getFarm().map.Layers[0].LayerHeight; ++y)
                Game1.getFarm().removeEverythingExceptCharactersFromThisTile(x, y);
            }
            goto case "break";
          case "clearfishcaught":
            Game1.player.fishCaught.Clear();
            goto case "break";
          case "clearfurniture":
            (Game1.currentLocation as FarmHouse).furniture.Clear();
            goto case "break";
          case "clearlightglows":
            Game1.currentLocation.lightGlows.Clear();
            goto case "break";
          case "clearmail":
            Game1.player.mailReceived.Clear();
            goto case "break";
          case "clearmuseum":
            (Game1.getLocationFromName("ArchaeologyHouse") as LibraryMuseum).museumPieces.Clear();
            goto case "break";
          case "clearquests":
            Game1.player.questLog.Clear();
            goto case "break";
          case "clearspecials":
            Game1.player.hasRustyKey = false;
            Game1.player.hasSkullKey = false;
            Game1.player.hasSpecialCharm = false;
            Game1.player.hasDarkTalisman = false;
            Game1.player.hasMagicInk = false;
            Game1.player.hasClubCard = false;
            Game1.player.canUnderstandDwarves = false;
            Game1.player.hasMagnifyingGlass = false;
            goto case "break";
          case "clone":
            Game1.currentLocation.characters.Add(Utility.fuzzyCharacterSearch(debugSplit[1]));
            goto case "break";
          case "clothes":
            Game1.playSound("coin");
            Game1.player.addItemToInventoryBool((Item) new Clothing(Convert.ToInt32(debugSplit[1])));
            goto case "break";
          case "cmenu":
          case "customize":
          case "customizemenu":
            Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(CharacterCustomization.Source.NewGame);
            goto case "break";
          case "collectquest":
            Game1.player.questLog.Add((Quest) new ResourceCollectionQuest());
            goto case "break";
          case "completecc":
            Game1.player.mailReceived.Add("ccCraftsRoom");
            Game1.player.mailReceived.Add("ccVault");
            Game1.player.mailReceived.Add("ccFishTank");
            Game1.player.mailReceived.Add("ccBoilerRoom");
            Game1.player.mailReceived.Add("ccPantry");
            Game1.player.mailReceived.Add("ccBulletin");
            Game1.player.mailReceived.Add("ccBoilerRoom");
            Game1.player.mailReceived.Add("ccPantry");
            Game1.player.mailReceived.Add("ccBulletin");
            CommunityCenter locationFromName2 = Game1.getLocationFromName("CommunityCenter") as CommunityCenter;
            for (int area = 0; area < locationFromName2.areasComplete.Count; ++area)
              locationFromName2.markAreaAsComplete(area);
            goto case "break";
          case "completejoja":
            Game1.player.mailReceived.Add("ccCraftsRoom");
            Game1.player.mailReceived.Add("ccVault");
            Game1.player.mailReceived.Add("ccFishTank");
            Game1.player.mailReceived.Add("ccBoilerRoom");
            Game1.player.mailReceived.Add("ccPantry");
            Game1.player.mailReceived.Add("jojaCraftsRoom");
            Game1.player.mailReceived.Add("jojaVault");
            Game1.player.mailReceived.Add("jojaFishTank");
            Game1.player.mailReceived.Add("jojaBoilerRoom");
            Game1.player.mailReceived.Add("jojaPantry");
            Game1.player.mailReceived.Add("JojaMember");
            goto case "break";
          case "completequest":
            Game1.player.completeQuest(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "completespecialorders":
          case "cso":
            using (NetList<SpecialOrder, NetRef<SpecialOrder>>.Enumerator enumerator = Game1.player.team.specialOrders.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                foreach (OrderObjective objective in enumerator.Current.objectives)
                  objective.SetCount(objective.maxCount.Value);
              }
              goto case "break";
            }
          case "conventionmode":
            Game1.conventionMode = !Game1.conventionMode;
            goto case "break";
          case "cooking":
            using (Dictionary<string, string>.KeyCollection.Enumerator enumerator = CraftingRecipe.cookingRecipes.Keys.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                string current = enumerator.Current;
                if (!Game1.player.cookingRecipes.ContainsKey(current))
                  Game1.player.cookingRecipes.Add(current, 0);
              }
              goto case "break";
            }
          case "cookingrecipe":
            Game1.player.cookingRecipes.Add(debugInput.Substring(debugInput.IndexOf(' ')).Trim(), 0);
            goto case "break";
          case "coop":
          case "upgradecoop":
            Game1.player.CoopUpgradeLevel = Math.Min(3, Game1.player.CoopUpgradeLevel + 1);
            Game1.removeFrontLayerForFarmBuildings();
            Game1.addNewFarmBuildingMaps();
            goto case "break";
          case "crafting":
            using (Dictionary<string, string>.KeyCollection.Enumerator enumerator = CraftingRecipe.craftingRecipes.Keys.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                string current = enumerator.Current;
                if (!Game1.player.craftingRecipes.ContainsKey(current))
                  Game1.player.craftingRecipes.Add(current, 0);
              }
              goto case "break";
            }
          case "craftingrecipe":
            Game1.player.craftingRecipes.Add(debugInput.Substring(debugInput.IndexOf(' ')).Trim(), 0);
            goto case "break";
          case "crane":
            Game1.currentMinigame = (IMinigame) new CraneGame();
            goto case "break";
          case "createdebris":
          case "mainmenu":
            if (debugSplit.Length < 2)
            {
              Game1.debugOutput = "Invalid parameters; call like: createDebris <itemId>";
              goto case "break";
            }
            else
            {
              Game1.createObjectDebris(Convert.ToInt32(debugSplit[1]), Game1.player.getTileX(), Game1.player.getTileY());
              goto case "break";
            }
          case "createdino":
            Game1.currentLocation.characters.Add((NPC) new DinoMonster(Game1.player.position.Value + new Vector2(100f, 0.0f)));
            goto case "break";
          case "createsplash":
            Point point = new Point();
            if ((int) Game1.player.facingDirection == 3)
              point.X = -4;
            else if ((int) Game1.player.facingDirection == 1)
              point.X = 4;
            else if ((int) Game1.player.facingDirection == 0)
              point.Y = 4;
            else if ((int) Game1.player.facingDirection == 2)
              point.Y = -4;
            Game1.player.currentLocation.fishSplashPoint.Set(new Point(Game1.player.getTileX() + point.X, Game1.player.getTileX() + point.Y));
            goto case "break";
          case "crib":
            if (Game1.getLocationFromName(Game1.player.homeLocation.Value) is FarmHouse locationFromName3)
            {
              int int32_2 = Convert.ToInt32(debugSplit[1]);
              locationFromName3.cribStyle.Value = int32_2;
              goto case "break";
            }
            else
              goto case "break";
          case "dap":
          case "daysplayed":
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3332", (object) (int) Game1.stats.DaysPlayed));
            goto case "break";
          case "darktalisman":
            Game1.player.hasDarkTalisman = true;
            Game1.getLocationFromName("Railroad").setMapTile(54, 35, 287, "Buildings", "", 1);
            Game1.getLocationFromName("Railroad").setMapTile(54, 34, 262, "Front", "", 1);
            Game1.getLocationFromName("WitchHut").setMapTile(4, 11, 114, "Buildings", "", 1);
            Game1.getLocationFromName("WitchHut").setTileProperty(4, 11, "Buildings", "Action", "MagicInk");
            Game1.player.hasMagicInk = false;
            Game1.player.mailReceived.Clear();
            goto case "break";
          case "darts":
            Game1.currentMinigame = (IMinigame) new Darts();
            goto case "break";
          case "dateplayer":
            using (IEnumerator<Farmer> enumerator = Game1.getAllFarmers().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                Farmer current = enumerator.Current;
                if (current != Game1.player && (bool) (NetFieldBase<bool, NetBool>) current.isCustomized)
                {
                  Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, current.UniqueMultiplayerID).Status = FriendshipStatus.Dating;
                  break;
                }
              }
              goto case "break";
            }
          case "dating":
            Game1.player.friendshipData[debugSplit[1]].Status = FriendshipStatus.Dating;
            goto case "break";
          case "day":
            Game1.stats.DaysPlayed = (uint) (Utility.getSeasonNumber(Game1.currentSeason) * 28 + Convert.ToInt32(debugSplit[1]) + (Game1.year - 1) * 4 * 28);
            Game1.dayOfMonth = Convert.ToInt32(debugSplit[1]);
            goto case "break";
          case "dayupdate":
            Game1.currentLocation.DayUpdate(Game1.dayOfMonth);
            if (debugSplit.Length > 1)
            {
              for (int index = 0; index < Convert.ToInt32(debugSplit[1]) - 1; ++index)
                Game1.currentLocation.DayUpdate(Game1.dayOfMonth);
              goto case "break";
            }
            else
              goto case "break";
          case "db":
            Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(Utility.fuzzyCharacterSearch(debugSplit.Length > 1 ? debugSplit[1] : "Pierre").CurrentDialogue.Peek());
            goto case "break";
          case "debrisweather":
            Game1.debrisWeather.Clear();
            Game1.isDebrisWeather = !Game1.isDebrisWeather;
            if (Game1.isDebrisWeather)
            {
              Game1.populateDebrisWeatherArray();
              goto case "break";
            }
            else
              goto case "break";
          case "deletearch":
            Game1.player.archaeologyFound.Clear();
            Game1.player.fishCaught.Clear();
            Game1.player.mineralsFound.Clear();
            Game1.player.mailReceived.Clear();
            goto case "break";
          case "deliveryquest":
            Game1.player.questLog.Add((Quest) new ItemDeliveryQuest());
            goto case "break";
          case "dialogue":
            Utility.fuzzyCharacterSearch(debugSplit[1]).CurrentDialogue.Push(new Dialogue(debugInput.Substring(debugInput.IndexOf("0") + 1), Utility.fuzzyCharacterSearch(debugSplit[1])));
            goto case "break";
          case "die":
            Game1.player.health = 0;
            goto case "break";
          case "divorce":
            Game1.player.divorceTonight.Value = true;
            goto case "break";
          case "doesitemexist":
            Game1.showGlobalMessage(Utility.doesItemWithThisIndexExistAnywhere(Convert.ToInt32(debugSplit[1]), debugSplit.Length > 2) ? "Yes" : "No");
            goto case "break";
          case "dog":
            Game1.currentLocation.characters.Add((NPC) new Dog(Convert.ToInt32(debugSplit[1]), Convert.ToInt32(debugSplit[2]), ((IEnumerable<string>) debugSplit).Count<string>() > 3 ? Convert.ToInt32(debugSplit[3]) : 0));
            goto case "break";
          case "dp":
            Game1.stats.daysPlayed = (uint) Convert.ToInt32(debugSplit[1]);
            goto case "break";
          case "drawbounds":
            Game1.drawbounds = !Game1.drawbounds;
            goto case "break";
          case "dye":
            Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;
            string str1 = debugSplit[2].ToLower().Trim();
            if (!(str1 == "black"))
            {
              if (!(str1 == "red"))
              {
                if (!(str1 == "blue"))
                {
                  if (!(str1 == "yellow"))
                  {
                    if (!(str1 == "white"))
                    {
                      if (str1 == "green")
                        color = new Microsoft.Xna.Framework.Color(10, 143, 0);
                    }
                    else
                      color = Microsoft.Xna.Framework.Color.White;
                  }
                  else
                    color = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 230, 0);
                }
                else
                  color = new Microsoft.Xna.Framework.Color(0, 100, 220);
              }
              else
                color = new Microsoft.Xna.Framework.Color(220, 0, 0);
            }
            else
              color = Microsoft.Xna.Framework.Color.Black;
            float strength = 1f;
            if (debugSplit.Length > 2)
              strength = float.Parse(debugSplit[3]);
            string str2 = debugSplit[1].ToLower().Trim();
            if (!(str2 == "shirt"))
            {
              if (str2 == "pants" && Game1.player.pantsItem.Value != null)
              {
                Game1.player.pantsItem.Value.Dye(color, strength);
                goto case "break";
              }
              else
                goto case "break";
            }
            else if (Game1.player.shirtItem.Value != null)
            {
              Game1.player.shirtItem.Value.Dye(color, strength);
              goto case "break";
            }
            else
              goto case "break";
          case "dyeAll":
            Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(CharacterCustomization.Source.DyePots);
            goto case "break";
          case "dyemenu":
            Game1.activeClickableMenu = (IClickableMenu) new DyeMenu();
            goto case "break";
          case "dyepants":
            Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(Game1.player.pantsItem.Value);
            goto case "break";
          case "dyeshirt":
            Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(Game1.player.shirtItem.Value);
            goto case "break";
          case "ebi":
          case "eventbyid":
            if (debugSplit.Length < 1)
            {
              Game1.debugOutput = "Event ID not specified";
              return true;
            }
            foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
            {
              string locationName = location.Name;
              if (locationName == "Pool")
                locationName = "BathHouse_Pool";
              Dictionary<string, string> location_events = (Dictionary<string, string>) null;
              try
              {
                location_events = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName);
              }
              catch (Exception ex)
              {
                continue;
              }
              if (location_events != null)
              {
                foreach (string key1 in location_events.Keys)
                {
                  string key = key1;
                  string[] strArray = key.Split('/');
                  if (strArray[0] == debugSplit[1])
                  {
                    int event_id = -1;
                    if (int.TryParse(strArray[0], out event_id))
                    {
                      while (Game1.player.eventsSeen.Contains(event_id))
                        Game1.player.eventsSeen.Remove(event_id);
                    }
                    LocationRequest locationRequest = Game1.getLocationRequest(locationName);
                    locationRequest.OnLoad += (LocationRequest.Callback) (() => Game1.currentLocation.currentEvent = new Event(location_events[key], event_id));
                    int x = 8;
                    int y = 8;
                    Utility.getDefaultWarpLocation(locationRequest.Name, ref x, ref y);
                    Game1.warpFarmer(locationRequest, x, y, Game1.player.FacingDirection);
                    Game1.debugOutput = "Starting event " + key;
                    return true;
                  }
                }
              }
            }
            Game1.debugOutput = "Event not found.";
            goto case "break";
          case "ee":
            Game1.pauseTime = 0.0f;
            Game1.nonWarpFade = true;
            Game1.eventFinished();
            Game1.fadeScreenToBlack();
            Game1.viewportFreeze = false;
            goto case "break";
          case "end":
            Game1.warpFarmer("Town", 20, 20, false);
            Game1.getLocationFromName("Town").currentEvent = new Event(Utility.getStardewHeroCelebrationEventString(90));
            this.makeCelebrationWeatherDebris();
            Utility.perpareDayForStardewCelebration(90);
            goto case "break";
          case "endevent":
          case "leaveevent":
            Game1.pauseTime = 0.0f;
            Game1.player.eventsSeen.Clear();
            Game1.player.dialogueQuestionsAnswered.Clear();
            Game1.player.mailReceived.Clear();
            Game1.nonWarpFade = true;
            Game1.eventFinished();
            Game1.fadeScreenToBlack();
            Game1.viewportFreeze = false;
            goto case "break";
          case "energize":
            Game1.player.Stamina = (float) Game1.player.MaxStamina;
            if (debugSplit.Length > 1)
            {
              Game1.player.Stamina = (float) Convert.ToInt32(debugSplit[1]);
              goto case "break";
            }
            else
              goto case "break";
          case "engaged":
            Game1.player.changeFriendship(2500, Utility.fuzzyCharacterSearch(debugSplit[1]));
            Game1.player.spouse = debugSplit[1];
            Game1.player.friendshipData[debugSplit[1]].Status = FriendshipStatus.Engaged;
            WorldDate date1 = Game1.Date;
            ++date1.TotalDays;
            Game1.player.friendshipData[debugSplit[1]].WeddingDate = date1;
            goto case "break";
          case "engageplayer":
            using (IEnumerator<Farmer> enumerator = Game1.getAllFarmers().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                Farmer current = enumerator.Current;
                if (current != Game1.player && (bool) (NetFieldBase<bool, NetBool>) current.isCustomized)
                {
                  Friendship friendship = Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, current.UniqueMultiplayerID);
                  friendship.Status = FriendshipStatus.Engaged;
                  friendship.WeddingDate = Game1.Date;
                  ++friendship.WeddingDate.TotalDays;
                  break;
                }
              }
              goto case "break";
            }
          case "event":
            if (debugSplit.Length <= 3)
              Game1.player.eventsSeen.Clear();
            GameLocation gameLocation = Utility.fuzzyLocationSearch(debugSplit[1]);
            if (gameLocation == null)
            {
              Game1.debugOutput = "No location with name " + debugSplit[1];
              goto case "break";
            }
            else
            {
              string locationName = gameLocation.Name;
              if (locationName == "Pool")
                locationName = "BathHouse_Pool";
              if (Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName).ElementAt<KeyValuePair<string, string>>(Convert.ToInt32(debugSplit[2])).Key.Contains('/'))
              {
                LocationRequest locationRequest = Game1.getLocationRequest(locationName);
                locationRequest.OnLoad += (LocationRequest.Callback) (() =>
                {
                  GameLocation currentLocation = Game1.currentLocation;
                  KeyValuePair<string, string> keyValuePair = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName).ElementAt<KeyValuePair<string, string>>(Convert.ToInt32(debugSplit[2]));
                  string eventString = keyValuePair.Value;
                  keyValuePair = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName).ElementAt<KeyValuePair<string, string>>(Convert.ToInt32(debugSplit[2]));
                  int int32_3 = Convert.ToInt32(keyValuePair.Key.Split('/')[0]);
                  Event @event = new Event(eventString, int32_3);
                  currentLocation.currentEvent = @event;
                });
                Game1.warpFarmer(locationRequest, 8, 8, Game1.player.FacingDirection);
                goto case "break";
              }
              else
                goto case "break";
            }
          case "eventover":
            Game1.eventFinished();
            goto case "break";
          case "eventseen":
          case "seenevent":
            Game1.player.eventsSeen.Add(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "eventtest":
            this.eventTest = new EventTest(((IEnumerable<string>) debugSplit).Count<string>() > 1 ? debugSplit[1] : "", ((IEnumerable<string>) debugSplit).Count<string>() > 2 ? Convert.ToInt32(debugSplit[2]) : 0);
            goto case "break";
          case "eventtestspecific":
            this.eventTest = new EventTest(debugSplit);
            goto case "break";
          case "everythingshop":
            Dictionary<ISalable, int[]> itemPriceAndStock = new Dictionary<ISalable, int[]>();
            itemPriceAndStock.Add((ISalable) new Furniture(1226, Vector2.Zero), new int[2]
            {
              0,
              int.MaxValue
            });
            foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>) Game1.objectInformation)
            {
              try
              {
                itemPriceAndStock.Add((ISalable) new Object(keyValuePair.Key, 1), new int[2]
                {
                  0,
                  int.MaxValue
                });
              }
              catch (Exception ex)
              {
              }
            }
            foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>) Game1.bigCraftablesInformation)
            {
              try
              {
                itemPriceAndStock.Add((ISalable) new Object(Vector2.Zero, keyValuePair.Key), new int[2]
                {
                  0,
                  int.MaxValue
                });
              }
              catch (Exception ex)
              {
              }
            }
            foreach (KeyValuePair<int, string> keyValuePair in Game1.content.Load<Dictionary<int, string>>("Data\\weapons"))
            {
              try
              {
                itemPriceAndStock.Add((ISalable) new MeleeWeapon(keyValuePair.Key), new int[2]
                {
                  0,
                  int.MaxValue
                });
              }
              catch (Exception ex)
              {
              }
            }
            foreach (KeyValuePair<int, string> keyValuePair in Game1.content.Load<Dictionary<int, string>>("Data\\furniture"))
            {
              try
              {
                itemPriceAndStock.Add((ISalable) new Furniture(keyValuePair.Key, Vector2.Zero), new int[2]
                {
                  0,
                  int.MaxValue
                });
              }
              catch (Exception ex)
              {
              }
            }
            Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(itemPriceAndStock);
            goto case "break";
          case "exhaust":
            Game1.player.Stamina = -15f;
            goto case "break";
          case "experience":
            int which = 0;
            if (debugSplit[1].Count<char>() > 1)
            {
              string lower = debugSplit[1].ToLower();
              if (!(lower == "farming"))
              {
                if (!(lower == "fishing"))
                {
                  if (!(lower == "mining"))
                  {
                    if (!(lower == "foraging"))
                    {
                      if (lower == "combat")
                        which = 4;
                    }
                    else
                      which = 2;
                  }
                  else
                    which = 3;
                }
                else
                  which = 1;
              }
              else
                which = 0;
            }
            else
              which = Convert.ToInt32(debugSplit[1]);
            Game1.player.gainExperience(which, Convert.ToInt32(debugSplit[2]));
            goto case "break";
          case "f":
          case "fin":
          case "fuzzyitemnamed":
            int result1 = -1;
            int result2 = 1;
            if (debugSplit.Length > 2)
              int.TryParse(debugSplit[2], out result2);
            if (debugSplit.Length > 3)
              int.TryParse(debugSplit[3], out result1);
            Item obj1 = Utility.fuzzyItemSearch(debugSplit[1], result2);
            if (obj1 != null)
            {
              if (result1 >= 0 && obj1 is Object)
                (obj1 as Object).quality.Value = result1;
              Game1.player.addItemToInventory(obj1);
              Game1.playSound("coin");
              string str3 = obj1.GetType().ToString();
              if (str3.Contains('.'))
              {
                str3 = str3.Substring(str3.LastIndexOf('.') + 1);
                if (obj1 is Object && (bool) (NetFieldBase<bool, NetBool>) (obj1 as Object).bigCraftable)
                  str3 = "Big Craftable";
              }
              Game1.debugOutput = "Added " + obj1.DisplayName + " (" + str3 + ")";
              goto case "break";
            }
            else
            {
              Game1.debugOutput = "No item found with name " + debugSplit[1];
              goto case "break";
            }
          case "face":
          case "facedirection":
          case "fd":
            if (debugSplit[1].Equals("farmer"))
            {
              Game1.player.Halt();
              Game1.player.completelyStopAnimatingOrDoingAction();
              Game1.player.faceDirection(Convert.ToInt32(debugSplit[2]));
              goto case "break";
            }
            else
            {
              Utility.fuzzyCharacterSearch(debugSplit[1]).faceDirection(Convert.ToInt32(debugSplit[2]));
              goto case "break";
            }
          case "faceplayer":
            Utility.fuzzyCharacterSearch(debugSplit[1]).faceTowardFarmer = true;
            goto case "break";
          case "farmmap":
            for (int index = 0; index < Game1.locations.Count; ++index)
            {
              if (Game1.locations[index] is Farm)
                Game1.locations.RemoveAt(index);
              if (Game1.locations[index] is FarmHouse)
                Game1.locations.RemoveAt(index);
            }
            Game1.whichFarm = Convert.ToInt32(debugSplit[1]);
            Game1.locations.Add((GameLocation) new Farm("Maps\\" + Farm.getMapNameFromTypeInt(Game1.whichFarm), "Farm"));
            Game1.locations.Add((GameLocation) new FarmHouse("Maps\\FarmHouse", "FarmHouse"));
            goto case "break";
          case "fb":
          case "fillbin":
            Game1.getFarm().getShippingBin(Game1.player).Add((Item) new Object(24, 1));
            Game1.getFarm().getShippingBin(Game1.player).Add((Item) new Object(82, 1));
            Game1.getFarm().getShippingBin(Game1.player).Add((Item) new Object(136, 1));
            Game1.getFarm().getShippingBin(Game1.player).Add((Item) new Object(16, 1));
            Game1.getFarm().getShippingBin(Game1.player).Add((Item) new Object(388, 1));
            goto case "break";
          case "fbf":
          case "framebyframe":
            Game1.frameByFrame = !Game1.frameByFrame;
            if (Game1.frameByFrame)
            {
              Game1.playSound("bigSelect");
              goto case "break";
            }
            else
            {
              Game1.playSound("bigDeSelect");
              goto case "break";
            }
          case "fbp":
          case "fill":
          case "fillbackpack":
          case "fillbp":
            for (int index = 0; index < Game1.player.items.Count; ++index)
            {
              if (Game1.player.items[index] == null)
              {
                int num = -1;
                while (!Game1.objectInformation.ContainsKey(num))
                {
                  num = Game1.random.Next(1000);
                  if (num != 390 && (!Game1.objectInformation.ContainsKey(num) || Game1.objectInformation[num].Split('/')[0] == "Stone"))
                    num = -1;
                  else if (!Game1.objectInformation.ContainsKey(num) || Game1.objectInformation[num].Split('/')[0].Contains("Weed"))
                    num = -1;
                  else if (!Game1.objectInformation.ContainsKey(num) || Game1.objectInformation[num].Split('/')[3].Contains("Crafting"))
                    num = -1;
                  else if (!Game1.objectInformation.ContainsKey(num) || Game1.objectInformation[num].Split('/')[3].Contains("Seed"))
                    num = -1;
                }
                bool flag = false;
                if (num >= 516 && num <= 534)
                  flag = true;
                if (flag)
                  Game1.player.items[index] = (Item) new Ring(num);
                else
                  Game1.player.items[index] = (Item) new Object(num, 1);
              }
            }
            goto case "break";
          case "fencedecay":
            using (OverlaidDictionary.ValuesCollection.Enumerator enumerator = Game1.currentLocation.objects.Values.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                object current = (object) enumerator.Current;
                if (current is Fence)
                  (current as Fence).health.Value -= (float) Convert.ToInt32(debugSplit[1]);
              }
              goto case "break";
            }
          case "festival":
            Dictionary<string, string> dictionary1 = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + debugSplit[1]);
            if (dictionary1 != null)
            {
              string str4 = new string(debugSplit[1].Where<char>(new Func<char, bool>(char.IsLetter)).ToArray<char>());
              int int32_4 = Convert.ToInt32(new string(debugSplit[1].Where<char>(new Func<char, bool>(char.IsDigit)).ToArray<char>()));
              this.parseDebugInput("season " + str4);
              this.parseDebugInput("day " + int32_4.ToString());
              this.parseDebugInput("time " + Convert.ToInt32(dictionary1["conditions"].Split('/')[1].Split(' ')[0]).ToString());
              this.parseDebugInput("warp " + dictionary1["conditions"].Split('/')[0] + " 1 1");
              goto case "break";
            }
            else
              goto case "break";
          case "festivalscore":
            Game1.player.festivalScore += Convert.ToInt32(debugSplit[1]);
            goto case "break";
          case "ff":
          case "furniture":
            if (debugSplit.Length < 2)
            {
              Furniture furniture = (Furniture) null;
              while (furniture == null)
              {
                try
                {
                  furniture = new Furniture(Game1.random.Next(1613), Vector2.Zero);
                }
                catch (Exception ex)
                {
                }
              }
              Game1.player.addItemToInventoryBool((Item) furniture);
              goto case "break";
            }
            else
            {
              Game1.player.addItemToInventoryBool((Item) new Furniture(Convert.ToInt32(debugSplit[1]), Vector2.Zero));
              goto case "break";
            }
          case "fillwithobject":
            int int32_5 = Convert.ToInt32(debugSplit[1]);
            bool flag1 = ((IEnumerable<string>) debugSplit).Count<string>() > 2 && Convert.ToBoolean(debugSplit[2]);
            for (int y = 0; y < Game1.currentLocation.map.Layers[0].LayerHeight; ++y)
            {
              for (int x = 0; x < Game1.currentLocation.map.Layers[0].LayerWidth; ++x)
              {
                Vector2 vector2 = new Vector2((float) x, (float) y);
                if (Game1.currentLocation.isTileLocationTotallyClearAndPlaceable(vector2))
                  Game1.currentLocation.setObject(vector2, flag1 ? new Object(vector2, int32_5) : new Object(int32_5, 1));
              }
            }
            goto case "break";
          case "fish":
            Game1.activeClickableMenu = (IClickableMenu) new BobberBar(Convert.ToInt32(debugSplit[1]), 0.5f, true, (Game1.player.CurrentTool as FishingRod).attachments[1] != null ? (Game1.player.CurrentTool as FishingRod).attachments[1].ParentSheetIndex : -1);
            goto case "break";
          case "fishing":
            Game1.player.FishingLevel = Convert.ToInt32(debugSplit[1]);
            goto case "break";
          case "fixanimals":
            Farm farm1 = Game1.getFarm();
            using (List<Building>.Enumerator enumerator = farm1.buildings.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                Building current = enumerator.Current;
                if (current.indoors.Value != null && current.indoors.Value is AnimalHouse)
                {
                  foreach (FarmAnimal farmAnimal in (current.indoors.Value as AnimalHouse).animals.Values)
                  {
                    foreach (Building building in farm1.buildings)
                    {
                      if (building.indoors.Value != null && building.indoors.Value is AnimalHouse && (building.indoors.Value as AnimalHouse).animalsThatLiveHere.Contains((long) farmAnimal.myID) && !building.Equals((object) farmAnimal.home))
                      {
                        for (int index = (building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count - 1; index >= 0; --index)
                        {
                          if ((building.indoors.Value as AnimalHouse).animalsThatLiveHere[index] == (long) farmAnimal.myID)
                          {
                            (building.indoors.Value as AnimalHouse).animalsThatLiveHere.RemoveAt(index);
                            Game1.playSound("crystal");
                          }
                        }
                      }
                    }
                  }
                  for (int index = (current.indoors.Value as AnimalHouse).animalsThatLiveHere.Count - 1; index >= 0; --index)
                  {
                    if (Utility.getAnimal((current.indoors.Value as AnimalHouse).animalsThatLiveHere[index]) == null)
                    {
                      (current.indoors.Value as AnimalHouse).animalsThatLiveHere.RemoveAt(index);
                      Game1.playSound("crystal");
                    }
                  }
                }
              }
              goto case "break";
            }
          case "fixweapons":
            Game1.applySaveFix(SaveGame.SaveFixes.ResetForges);
            Game1.debugOutput = "Reset forged weapon attributes.";
            goto case "break";
          case "floor":
            (Game1.getLocationFromName("FarmHouse") as FarmHouse).SetFloor(debugSplit[1], (string) null);
            goto case "break";
          case "fo":
          case "frameoffset":
            int num1 = debugSplit[2].Contains('s') ? -1 : 1;
            int[] featureXoffsetPerFrame = FarmerRenderer.featureXOffsetPerFrame;
            int int32_6 = Convert.ToInt32(debugSplit[1]);
            int num2 = num1;
            char ch = debugSplit[2].Last<char>();
            int int32_7 = Convert.ToInt32(ch.ToString() ?? "");
            int num3 = (int) (short) (num2 * int32_7);
            featureXoffsetPerFrame[int32_6] = num3;
            int num4 = debugSplit[3].Contains('s') ? -1 : 1;
            int[] featureYoffsetPerFrame = FarmerRenderer.featureYOffsetPerFrame;
            int int32_8 = Convert.ToInt32(debugSplit[1]);
            int num5 = num4;
            ch = debugSplit[3].Last<char>();
            int int32_9 = Convert.ToInt32(ch.ToString() ?? "");
            int num6 = (int) (short) (num5 * int32_9);
            featureYoffsetPerFrame[int32_8] = num6;
            if (debugSplit.Length > 4)
            {
              int num7 = debugSplit[4].Contains('s') ? -1 : 1;
              goto case "break";
            }
            else
              goto case "break";
          case "forge":
            Game1.activeClickableMenu = (IClickableMenu) new ForgeMenu();
            goto case "break";
          case "friend":
          case "friendship":
            NPC npc = Utility.fuzzyCharacterSearch(debugSplit[1]);
            if (npc != null && !Game1.player.friendshipData.ContainsKey(npc.Name))
              Game1.player.friendshipData.Add(npc.Name, new Friendship());
            Game1.player.friendshipData[npc.Name].Points = Convert.ToInt32(debugSplit[2]);
            goto case "break";
          case "friendall":
            using (DisposableList<NPC>.Enumerator enumerator = Utility.getAllCharacters().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                NPC current = enumerator.Current;
                if ((current.CanSocialize || !(current.Name != "Sandy") || !(current.Name == "Krobus")) && !(current.Name == "Marlon"))
                {
                  if (current != null && !Game1.player.friendshipData.ContainsKey(current.Name))
                    Game1.player.friendshipData.Add(current.Name, new Friendship());
                  Game1.player.changeFriendship(((IEnumerable<string>) debugSplit).Count<string>() > 1 ? Convert.ToInt32(debugSplit[1]) : 2500, current);
                }
              }
              goto case "break";
            }
          case "fruittrees":
            using (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.PairsCollection.Enumerator enumerator = Game1.currentLocation.terrainFeatures.Pairs.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<Vector2, TerrainFeature> current = enumerator.Current;
                if (current.Value is FruitTree)
                {
                  (current.Value as FruitTree).daysUntilMature.Value -= 27;
                  current.Value.dayUpdate(Game1.currentLocation, current.Key);
                }
              }
              goto case "break";
            }
          case "gamemode":
            Game1.setGameMode(Convert.ToByte(debugSplit[1]));
            goto case "break";
          case "gamepad":
            Game1.options.gamepadControls = !Game1.options.gamepadControls;
            Game1.options.mouseControls = !Game1.options.gamepadControls;
            Game1.showGlobalMessage(Game1.options.gamepadControls ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3209") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3210"));
            goto case "break";
          case "gem":
            Game1.player.QiGems += Convert.ToInt32(debugSplit[1]);
            goto case "break";
          case "getallquests":
            using (Dictionary<int, string>.Enumerator enumerator = Game1.content.Load<Dictionary<int, string>>("Data\\Quests").GetEnumerator())
            {
              while (enumerator.MoveNext())
                Game1.player.addQuest(enumerator.Current.Key);
              goto case "break";
            }
          case "getindex":
            Item obj2 = Utility.fuzzyItemSearch(debugSplit[1]);
            Game1.debugOutput = obj2 == null ? "No item found with name " + debugSplit[1] : obj2.DisplayName + "'s index is " + obj2.ParentSheetIndex.ToString();
            goto case "break";
          case "getstat":
            Game1.debugOutput = Game1.stats.GetType().GetProperty(debugSplit[1]).GetValue((object) Game1.stats, (object[]) null).ToString();
            goto case "break";
          case "gm":
          case "inv":
          case "invincible":
            if (Game1.player.temporarilyInvincible)
            {
              Game1.player.temporaryInvincibilityTimer = 0;
              Game1.playSound("bigDeSelect");
              goto case "break";
            }
            else
            {
              Game1.player.temporarilyInvincible = true;
              Game1.player.temporaryInvincibilityTimer = -1000000000;
              Game1.playSound("bigSelect");
              goto case "break";
            }
          case "gold":
            Game1.player.Money += 1000000;
            goto case "break";
          case "grass":
            for (int x = 0; x < Game1.getFarm().Map.Layers[0].LayerWidth; ++x)
            {
              for (int y = 0; y < Game1.getFarm().Map.Layers[0].LayerHeight; ++y)
              {
                if (Game1.getFarm().isTileLocationTotallyClearAndPlaceable(new Vector2((float) x, (float) y)))
                  Game1.getFarm().terrainFeatures.Add(new Vector2((float) x, (float) y), (TerrainFeature) new Grass(1, 4));
              }
            }
            goto case "break";
          case "growanimals":
            using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.ValuesCollection.Enumerator enumerator = (Game1.currentLocation as AnimalHouse).animals.Values.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                FarmAnimal current = enumerator.Current;
                current.age.Value = (int) (byte) (NetFieldBase<byte, NetByte>) current.ageWhenMature - 1;
                current.dayUpdate(Game1.currentLocation);
              }
              goto case "break";
            }
          case "growanimalsfarm":
            using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.ValuesCollection.Enumerator enumerator = (Game1.currentLocation as Farm).animals.Values.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                FarmAnimal current = enumerator.Current;
                if (current.isBaby())
                {
                  current.age.Value = (int) (byte) (NetFieldBase<byte, NetByte>) current.ageWhenMature - 1;
                  current.dayUpdate(Game1.currentLocation);
                }
              }
              goto case "break";
            }
          case "growcrops":
            using (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.PairsCollection.Enumerator enumerator = Game1.currentLocation.terrainFeatures.Pairs.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<Vector2, TerrainFeature> current = enumerator.Current;
                if (current.Value is HoeDirt && (current.Value as HoeDirt).crop != null)
                {
                  for (int index = 0; index < Convert.ToInt32(debugSplit[1]); ++index)
                  {
                    if ((current.Value as HoeDirt).crop != null)
                      (current.Value as HoeDirt).crop.newDay(1, -1, (int) current.Key.X, (int) current.Key.Y, Game1.currentLocation);
                  }
                }
              }
              goto case "break";
            }
          case "growgrass":
            Game1.currentLocation.spawnWeeds(false);
            Game1.currentLocation.growWeedGrass(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "growwildtrees":
            for (int index = Game1.currentLocation.terrainFeatures.Count() - 1; index >= 0; --index)
            {
              Vector2 vector2 = Game1.currentLocation.terrainFeatures.Keys.ElementAt<Vector2>(index);
              if (Game1.currentLocation.terrainFeatures[vector2] is Tree)
              {
                (Game1.currentLocation.terrainFeatures[vector2] as Tree).growthStage.Value = 4;
                (Game1.currentLocation.terrainFeatures[vector2] as Tree).fertilized.Value = true;
                (Game1.currentLocation.terrainFeatures[vector2] as Tree).dayUpdate(Game1.currentLocation, vector2);
                (Game1.currentLocation.terrainFeatures[vector2] as Tree).fertilized.Value = false;
              }
            }
            goto case "break";
          case "haircolor":
            Game1.player.changeHairColor(new Microsoft.Xna.Framework.Color(Convert.ToInt32(debugSplit[1]), Convert.ToInt32(debugSplit[2]), Convert.ToInt32(debugSplit[3])));
            goto case "break";
          case "hairstyle":
            Game1.player.changeHairStyle(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "hat":
            Game1.player.changeHat(Convert.ToInt32(debugSplit[1]));
            Game1.playSound("coin");
            goto case "break";
          case "heal":
            Game1.player.health = Game1.player.maxHealth;
            goto case "break";
          case "hoe":
            Game1.player.addItemToInventoryBool((Item) new Hoe());
            Game1.playSound("coin");
            goto case "break";
          case "horse":
            Game1.currentLocation.characters.Add((NPC) new Horse(GuidHelper.NewGuid(), Convert.ToInt32(debugSplit[1]), Convert.ToInt32(debugSplit[2])));
            goto case "break";
          case "house":
          case "houseupgrade":
          case "hu":
            Utility.getHomeOfFarmer(Game1.player).moveObjectsForHouseUpgrade(Convert.ToInt32(debugSplit[1]));
            Utility.getHomeOfFarmer(Game1.player).setMapForUpgradeLevel(Convert.ToInt32(debugSplit[1]));
            Game1.player.HouseUpgradeLevel = Convert.ToInt32(debugSplit[1]);
            Game1.removeFrontLayerForFarmBuildings();
            Game1.addNewFarmBuildingMaps();
            Utility.getHomeOfFarmer(Game1.player).ReadWallpaperAndFloorTileData();
            Utility.getHomeOfFarmer(Game1.player).RefreshFloorObjectNeighbors();
            goto case "break";
          case "hurry":
            Utility.fuzzyCharacterSearch(debugSplit[1]).warpToPathControllerDestination();
            goto case "break";
          case "i":
          case "item":
            if (Game1.objectInformation.ContainsKey(Convert.ToInt32(debugSplit[1])))
            {
              Game1.playSound("coin");
              Game1.player.addItemToInventoryBool((Item) new Object(Convert.ToInt32(debugSplit[1]), debugSplit.Length >= 3 ? Convert.ToInt32(debugSplit[2]) : 1, quality: (debugSplit.Length >= 4 ? Convert.ToInt32(debugSplit[3]) : 0)));
              goto case "break";
            }
            else
              goto case "break";
          case "in":
          case "itemnamed":
            using (IEnumerator<int> enumerator = Game1.objectInformation.Keys.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                int current = enumerator.Current;
                if (Game1.objectInformation[current].Substring(0, Game1.objectInformation[current].IndexOf('/')).ToLower().Replace(" ", "").Equals(debugSplit[1].ToLower()))
                {
                  Game1.player.addItemToInventory((Item) new Object(current, debugSplit.Length >= 3 ? Convert.ToInt32(debugSplit[2]) : 1, quality: (debugSplit.Length >= 4 ? Convert.ToInt32(debugSplit[3]) : 0)));
                  Game1.playSound("coin");
                }
              }
              goto case "break";
            }
          case "inputsim":
          case "is":
            if (Game1.inputSimulator != null)
              Game1.inputSimulator = (IInputSimulator) null;
            if (debugSplit.Length < 2)
            {
              Game1.debugOutput = "Invalid arguments, call as: inputSim <simType>";
              goto case "break";
            }
            else
            {
              string lower = debugSplit[1].ToLower();
              if (!(lower == "spamtool"))
              {
                if (lower == "spamlr")
                {
                  Game1.inputSimulator = (IInputSimulator) new LeftRightClickSpamInputSimulator();
                  goto case "break";
                }
                else
                {
                  Game1.debugOutput = "No input simulator found for " + debugSplit[1];
                  goto case "break";
                }
              }
              else
              {
                Game1.inputSimulator = (IInputSimulator) new ToolSpamInputSimulator();
                goto case "break";
              }
            }
          case "invitemovie":
            if (debugSplit.Length < 2)
            {
              Game1.debugOutput = "/inviteMovie (npc)";
              goto case "break";
            }
            else
            {
              NPC invited_npc = Utility.fuzzyCharacterSearch(debugSplit[1]);
              if (invited_npc == null)
              {
                Game1.debugOutput = "Invalid NPC";
                goto case "break";
              }
              else
              {
                MovieTheater.Invite(Game1.player, invited_npc);
                goto case "break";
              }
            }
          case "jn":
          case "junimonote":
            (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).addJunimoNote(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "jump":
            float jumpVelocity = 8f;
            if (debugSplit.Length > 2)
              jumpVelocity = (float) Convert.ToDouble(debugSplit[2]);
            if (debugSplit[1].Equals("farmer"))
            {
              Game1.player.jump(jumpVelocity);
              goto case "break";
            }
            else
            {
              Utility.fuzzyCharacterSearch(debugSplit[1]).jump(jumpVelocity);
              goto case "break";
            }
          case "junimogoodbye":
            (Game1.currentLocation as CommunityCenter).junimoGoodbyeDance();
            goto case "break";
          case "junimostar":
            ((Game1.getLocationFromName("CommunityCenter") as CommunityCenter).characters[0] as Junimo).returnToJunimoHutToFetchStar((GameLocation) (Game1.getLocationFromName("CommunityCenter") as CommunityCenter));
            goto case "break";
          case "killall":
            string str5 = debugSplit[1];
            using (IEnumerator<GameLocation> enumerator = Game1.locations.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                GameLocation current = enumerator.Current;
                if (!current.Equals(Game1.currentLocation))
                {
                  current.characters.Clear();
                }
                else
                {
                  for (int index = current.characters.Count - 1; index >= 0; --index)
                  {
                    if (!current.characters[index].Name.Equals(str5))
                      current.characters.RemoveAt(index);
                  }
                }
              }
              goto case "break";
            }
          case "killallhorses":
            using (IEnumerator<GameLocation> enumerator = Game1.locations.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                GameLocation current = enumerator.Current;
                for (int index = current.characters.Count - 1; index >= 0; --index)
                {
                  if (current.characters[index] is Horse)
                  {
                    current.characters.RemoveAt(index);
                    Game1.playSound("drumkit0");
                  }
                }
              }
              goto case "break";
            }
          case "killmonsterstat":
          case "kms":
            string str6 = debugSplit[1].Replace("0", " ");
            int int32_10 = Convert.ToInt32(debugSplit[2]);
            if (Game1.stats.specificMonstersKilled.ContainsKey(str6))
              Game1.stats.specificMonstersKilled[str6] = int32_10;
            else
              Game1.stats.specificMonstersKilled.Add(str6, int32_10);
            Game1.debugOutput = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3159", (object) str6, (object) int32_10);
            goto case "break";
          case "killnpc":
            for (int index1 = Game1.locations.Count - 1; index1 >= 0; --index1)
            {
              for (int index2 = 0; index2 < Game1.locations[index1].characters.Count; ++index2)
              {
                if (Game1.locations[index1].characters[index2].Name.Equals(debugSplit[1]))
                {
                  Game1.locations[index1].characters.RemoveAt(index2);
                  break;
                }
              }
            }
            goto case "break";
          case "ladder":
          case "shaft":
            if (debugSplit.Length > 1)
            {
              Game1.mine.createLadderDown(Convert.ToInt32(debugSplit[1]), Convert.ToInt32(debugSplit[2]), debugSplit[0] == "shaft");
              goto case "break";
            }
            else
            {
              Game1.mine.createLadderDown(Game1.player.getTileX(), Game1.player.getTileY() + 1, debugSplit[0] == "shaft");
              goto case "break";
            }
          case "language":
            Game1.activeClickableMenu = (IClickableMenu) new LanguageSelectionMenu();
            goto case "break";
          case "lantern":
            Game1.player.items.Add((Item) new Lantern());
            goto case "break";
          case "levelup":
            Game1.activeClickableMenu = debugSplit.Length <= 3 ? (IClickableMenu) new LevelUpMenu(Convert.ToInt32(debugSplit[1]), Convert.ToInt32(debugSplit[2])) : (IClickableMenu) new LevelUpMenu(Convert.ToInt32(debugSplit[1]), Convert.ToInt32(debugSplit[2]));
            goto case "break";
          case "listtags":
            if (Game1.player.CurrentItem != null)
            {
              string str7 = "Tags on " + Game1.player.CurrentItem.DisplayName + ": ";
              foreach (string contextTag in Game1.player.CurrentItem.GetContextTagList())
                str7 = str7 + contextTag + " ";
              Game1.debugOutput = str7.Trim();
              goto case "break";
            }
            else
              goto case "break";
          case "loaddialogue":
            NPC speaker = Utility.fuzzyCharacterSearch(debugSplit[1]);
            string path1 = debugSplit[2];
            string masterDialogue = Game1.content.LoadString(path1).Replace("{", "<").Replace("}", ">");
            speaker.CurrentDialogue.Push(new Dialogue(masterDialogue, speaker));
            Game1.drawDialogue(Utility.fuzzyCharacterSearch(debugSplit[1]));
            goto case "break";
          case "localInfo":
            Game1.debugOutput = "";
            int num8 = 0;
            int num9 = 0;
            int num10 = 0;
            foreach (TerrainFeature terrainFeature in Game1.currentLocation.terrainFeatures.Values)
            {
              switch (terrainFeature)
              {
                case Grass _:
                  ++num8;
                  continue;
                case Tree _:
                  ++num9;
                  continue;
                default:
                  ++num10;
                  continue;
              }
            }
            Game1.debugOutput = Game1.debugOutput + "Grass:" + num8.ToString() + ",  ";
            Game1.debugOutput = Game1.debugOutput + "Trees:" + num9.ToString() + ",  ";
            Game1.debugOutput = Game1.debugOutput + "Other Terrain Features:" + num10.ToString() + ",  ";
            Game1.debugOutput = Game1.debugOutput + "Objects: " + Game1.currentLocation.objects.Count().ToString() + ",  ";
            Game1.debugOutput = Game1.debugOutput + "temporarySprites: " + Game1.currentLocation.temporarySprites.Count.ToString() + ",  ";
            Game1.drawObjectDialogue(Game1.debugOutput);
            goto case "break";
          case "logbandwidth":
            if (Game1.IsServer)
            {
              Game1.server.LogBandwidth = !Game1.server.LogBandwidth;
              Game1.debugOutput = "Turned " + (Game1.server.LogBandwidth ? "on" : "off") + " server bandwidth logging";
              goto case "break";
            }
            else if (Game1.IsClient)
            {
              Game1.client.LogBandwidth = !Game1.client.LogBandwidth;
              Game1.debugOutput = "Turned " + (Game1.client.LogBandwidth ? "on" : "off") + " client bandwidth logging";
              goto case "break";
            }
            else
            {
              Game1.debugOutput = "Cannot toggle bandwidth logging in non-multiplayer games";
              goto case "break";
            }
          case "lookup":
          case "lu":
            using (IEnumerator<int> enumerator = Game1.objectInformation.Keys.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                int current = enumerator.Current;
                if (Game1.objectInformation[current].Substring(0, Game1.objectInformation[current].IndexOf('/')).ToLower().Equals(debugInput.Substring(debugInput.IndexOf(' ') + 1)))
                  Game1.debugOutput = debugSplit[1] + " " + current.ToString();
              }
              goto case "break";
            }
          case "m":
          case "musicvolume":
          case "mv":
            Game1.musicPlayerVolume = (float) Convert.ToDouble(debugSplit[1]);
            Game1.options.musicVolumeLevel = (float) Convert.ToDouble(debugSplit[1]);
            Game1.musicCategory.SetVolume(Game1.options.musicVolumeLevel);
            goto case "break";
          case "mailfortomorrow":
          case "mft":
            Game1.addMailForTomorrow(debugSplit[1].Replace('0', '_'), debugSplit.Length > 2);
            goto case "break";
          case "makeex":
            Game1.player.friendshipData[debugSplit[1]].RoommateMarriage = false;
            Game1.player.friendshipData[debugSplit[1]].Status = FriendshipStatus.Divorced;
            goto case "break";
          case "makeinedible":
            if (Game1.player.ActiveObject != null)
            {
              Game1.player.ActiveObject.edibility.Value = -300;
              goto case "break";
            }
            else
              goto case "break";
          case "marry":
            NPC n1 = Utility.fuzzyCharacterSearch(debugSplit[1]);
            if (n1 != null && !Game1.player.friendshipData.ContainsKey(n1.Name))
              Game1.player.friendshipData.Add(n1.Name, new Friendship());
            Game1.player.changeFriendship(2500, n1);
            Game1.player.spouse = n1.Name;
            Game1.player.friendshipData[n1.Name].WeddingDate = new WorldDate(Game1.Date);
            Game1.player.friendshipData[n1.Name].Status = FriendshipStatus.Married;
            Game1.prepareSpouseForWedding(Game1.player);
            goto case "break";
          case "marryplayer":
            using (FarmerCollection.Enumerator enumerator = Game1.getOnlineFarmers().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                Farmer current = enumerator.Current;
                if (current != Game1.player && (bool) (NetFieldBase<bool, NetBool>) current.isCustomized)
                {
                  Friendship friendship = Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, current.UniqueMultiplayerID);
                  friendship.Status = FriendshipStatus.Married;
                  friendship.WeddingDate = Game1.Date;
                  break;
                }
              }
              goto case "break";
            }
          case "md":
          case "minedifficulty":
            if (debugSplit.Length > 1)
              Game1.netWorldState.Value.MinesDifficulty = Convert.ToInt32(debugSplit[1]);
            Game1.debugOutput = "Mine difficulty: " + Game1.netWorldState.Value.MinesDifficulty.ToString();
            goto case "break";
          case "mergewallets":
            if (Game1.IsMasterGame)
            {
              ManorHouse.MergeWallets();
              goto case "break";
            }
            else
              goto case "break";
          case "minegame":
            int mode = 3;
            if (debugSplit.Length >= 2 && debugSplit[1] == "infinite")
              mode = 2;
            Game1.currentMinigame = (IMinigame) new MineCart(0, mode);
            goto case "break";
          case "mineinfo":
            int num11 = MineShaft.lowestLevelReached;
            string str8 = num11.ToString();
            num11 = Game1.player.deepestMineLevel;
            string str9 = num11.ToString();
            Game1.debugOutput = "MineShaft.lowestLevelReached = " + str8 + "\nplayer.deepestMineLevel = " + str9;
            goto case "break";
          case "minelevel":
            Game1.enterMine(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "minigame":
            switch (debugSplit[1])
            {
              case "blastoff":
                Game1.currentMinigame = (IMinigame) new RobotBlastoff();
                goto label_1159;
              case "cowboy":
                Game1.updateViewportForScreenSizeChange(false, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
                Game1.currentMinigame = (IMinigame) new AbigailGame();
                goto label_1159;
              case "fishing":
                Game1.currentMinigame = (IMinigame) new FishingGame();
                goto label_1159;
              case "grandpa":
                Game1.currentMinigame = (IMinigame) new GrandpaStory();
                goto label_1159;
              case "haleyCows":
                Game1.currentMinigame = (IMinigame) new HaleyCowPictures();
                goto label_1159;
              case "marucomet":
                Game1.currentMinigame = (IMinigame) new MaruComet();
                goto label_1159;
              case "minecart":
                Game1.currentMinigame = (IMinigame) new MineCart(0, 3);
                goto label_1159;
              case "plane":
                Game1.currentMinigame = (IMinigame) new PlaneFlyBy();
                goto label_1159;
              case "slots":
                Game1.currentMinigame = (IMinigame) new Slots();
                goto label_1159;
              case "target":
                Game1.currentMinigame = (IMinigame) new TargetGame();
                goto label_1159;
              default:
                goto label_1159;
            }
          case "money":
            Game1.player.Money = Convert.ToInt32(debugSplit[1]);
            goto case "break";
          case "monster":
            Type type = Type.GetType("StardewValley.Monsters." + debugSplit[1]);
            Vector2 vector2_1 = new Vector2(Convert.ToSingle(debugSplit[2]), Convert.ToSingle(debugSplit[3])) * 64f;
            object[] objArray;
            if (debugSplit.Length > 4)
            {
              string s = string.Join(" ", ((IEnumerable<string>) debugSplit).Skip<string>(4));
              int result3 = -1;
              if (int.TryParse(s, out result3))
                objArray = new object[2]
                {
                  (object) vector2_1,
                  (object) result3
                };
              else
                objArray = new object[2]
                {
                  (object) vector2_1,
                  (object) s
                };
            }
            else
              objArray = new object[1]{ (object) vector2_1 };
            Game1.currentLocation.characters.Add((NPC) (Activator.CreateInstance(type, objArray) as Monster));
            goto case "break";
          case "morepollen":
            for (int index = 0; index < Convert.ToInt32(debugSplit[1]); ++index)
              Game1.debrisWeather.Add(new WeatherDebris(new Vector2((float) Game1.random.Next(0, Game1.graphics.GraphicsDevice.Viewport.Width), (float) Game1.random.Next(0, Game1.graphics.GraphicsDevice.Viewport.Height)), 0, (float) Game1.random.Next(15) / 500f, (float) Game1.random.Next(-10, 0) / 50f, (float) Game1.random.Next(10) / 50f));
            goto case "break";
          case "movebuilding":
            Game1.getFarm().getBuildingAt(new Vector2((float) Convert.ToInt32(debugSplit[1]), (float) Convert.ToInt32(debugSplit[2]))).tileX.Value = Convert.ToInt32(debugSplit[3]);
            Game1.getFarm().getBuildingAt(new Vector2((float) Convert.ToInt32(debugSplit[1]), (float) Convert.ToInt32(debugSplit[2]))).tileY.Value = Convert.ToInt32(debugSplit[4]);
            goto case "break";
          case "movie":
            List<List<Character>> group1 = new List<List<Character>>();
            List<List<Character>> group2 = new List<List<Character>>();
            int index3 = Game1.random.Next(20);
            Character character1 = (Character) null;
            string movie_title = ((IEnumerable<string>) debugSplit).Count<string>() > 1 ? debugSplit[1] : "fall_movie_1";
            if (debugSplit.Length > 1)
              character1 = (Character) Utility.fuzzyCharacterSearch(debugSplit[1]);
            if (debugSplit.Length > 2)
              movie_title = debugSplit[2];
            if (character1 == null)
              character1 = (Character) Utility.getTownNPCByGiftTasteIndex(index3);
            group1.Add(new List<Character>()
            {
              (Character) Game1.player,
              character1
            });
            int index4 = (index3 + 1) % 25;
            int num12 = Game1.random.Next(3);
            for (int index5 = 0; index5 < num12; ++index5)
            {
              if (Game1.random.NextDouble() < 0.8)
              {
                if (Game1.random.NextDouble() < 0.5)
                {
                  group1.Add(new List<Character>()
                  {
                    (Character) Utility.getTownNPCByGiftTasteIndex(index4),
                    (Character) Utility.getTownNPCByGiftTasteIndex(index4 + 1)
                  });
                  index4 = (index4 + 2) % 25;
                }
                else
                {
                  group1.Add(new List<Character>()
                  {
                    (Character) Utility.getTownNPCByGiftTasteIndex(index4)
                  });
                  index4 = (index4 + 1) % 25;
                }
              }
            }
            for (int index6 = 0; index6 < 2; ++index6)
            {
              if (Game1.random.NextDouble() < 0.8)
              {
                if (Game1.random.NextDouble() < 0.33)
                {
                  group2.Add(new List<Character>()
                  {
                    (Character) Utility.getTownNPCByGiftTasteIndex(index4),
                    (Character) Utility.getTownNPCByGiftTasteIndex(index4 + 1)
                  });
                  index4 = (index4 + 2) % 25;
                }
                else if (Game1.random.NextDouble() < 5.0)
                {
                  group2.Add(new List<Character>()
                  {
                    (Character) Utility.getTownNPCByGiftTasteIndex(index4),
                    (Character) Utility.getTownNPCByGiftTasteIndex(index4 + 1),
                    (Character) Utility.getTownNPCByGiftTasteIndex(index4 + 2)
                  });
                  index4 = (index4 + 3) % 25;
                }
                else
                {
                  group2.Add(new List<Character>()
                  {
                    (Character) Utility.getTownNPCByGiftTasteIndex(index4)
                  });
                  index4 = (index4 + 1) % 25;
                }
              }
            }
            MovieTheaterScreeningEvent event_generator = new MovieTheaterScreeningEvent();
            Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => Game1.currentLocation.startEvent(event_generator.getMovieEvent(movie_title, group1, group2))));
            goto case "break";
          case "mp":
            Game1.player.addItemToInventoryBool((Item) new MilkPail());
            goto case "break";
          case "museumloot":
            using (IEnumerator<KeyValuePair<int, string>> enumerator = Game1.objectInformation.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<int, string> current = enumerator.Current;
                string str10 = current.Value.Split('/')[3];
                if ((str10.Contains("Arch") || str10.Contains("Minerals")) && !Game1.player.mineralsFound.ContainsKey(current.Key) && !Game1.player.archaeologyFound.ContainsKey(current.Key))
                {
                  if (str10.Contains("Arch"))
                    Game1.player.foundArtifact(current.Key, 1);
                  else
                    Game1.player.addItemToInventoryBool((Item) new Object(current.Key, 1));
                }
                if (Game1.player.freeSpotsInInventory() == 0)
                  return true;
              }
              goto case "break";
            }
          case "mushroomtrees":
            using (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.ValuesCollection.Enumerator enumerator = Game1.currentLocation.terrainFeatures.Values.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                TerrainFeature current = enumerator.Current;
                if (current is Tree)
                  (current as Tree).treeType.Value = 7;
              }
              goto case "break";
            }
          case "nd":
          case "newday":
          case "sleep":
            Game1.player.isInBed.Value = true;
            Game1.player.sleptInTemporaryBed.Value = true;
            Game1.currentLocation.answerDialogueAction("Sleep_Yes", (string[]) null);
            goto case "break";
          case "netclear":
            Game1.multiplayer.logging.Clear();
            goto case "break";
          case "netdump":
            Game1.debugOutput = "Wrote log to " + Game1.multiplayer.logging.Dump();
            goto case "break";
          case "nethost":
            Game1.multiplayer.StartServer();
            goto case "break";
          case "netjoin":
            Game1.activeClickableMenu = (IClickableMenu) new FarmhandMenu();
            goto case "break";
          case "netlog":
            Game1.multiplayer.logging.IsLogging = !Game1.multiplayer.logging.IsLogging;
            Game1.debugOutput = "Turned " + (Game1.multiplayer.logging.IsLogging ? "on" : "off") + " network write logging";
            goto case "break";
          case "newmuseumloot":
            using (IEnumerator<KeyValuePair<int, string>> enumerator = Game1.objectInformation.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<int, string> current = enumerator.Current;
                string str11 = current.Value.Split('/')[3];
                if ((str11.Contains("Arch") || str11.Contains("Minerals")) && !Game1.netWorldState.Value.MuseumPieces.Values.Contains<int>(current.Key))
                  Game1.player.addItemToInventoryBool((Item) new Object(current.Key, 1));
                if (Game1.player.freeSpotsInInventory() == 0)
                  return true;
              }
              goto case "break";
            }
          case "nosave":
          case "ns":
            Game1.saveOnNewDay = !Game1.saveOnNewDay;
            if (!Game1.saveOnNewDay)
              Game1.playSound("bigDeSelect");
            else
              Game1.playSound("bigSelect");
            Game1.debugOutput = "Saving is now " + (Game1.saveOnNewDay ? "enabled" : "disabled");
            goto case "break";
          case "note":
            if (!Game1.player.archaeologyFound.ContainsKey(102))
              Game1.player.archaeologyFound.Add(102, new int[2]);
            Game1.player.archaeologyFound[102][0] = 18;
            Game1.netWorldState.Value.LostBooksFound.Value = 18;
            Game1.currentLocation.readNote(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "oldminegame":
            Game1.currentMinigame = (IMinigame) new OldMineCart(0, 3);
            goto case "break";
          case "ordersboard":
            Game1.activeClickableMenu = (IClickableMenu) new SpecialOrdersBoard();
            goto case "break";
          case "owl":
            Game1.currentLocation.addOwl();
            goto case "break";
          case "pan":
            Game1.player.addItemToInventoryBool((Item) new Pan());
            goto case "break";
          case "panmode":
          case "pm":
            Game1.panMode = true;
            Game1.viewportFreeze = true;
            Game1.debugMode = true;
            this.panFacingDirectionWait = false;
            this.panModeString = "";
            goto case "break";
          case "pants":
            Game1.player.changePants(new Microsoft.Xna.Framework.Color(Convert.ToInt32(debugSplit[1]), Convert.ToInt32(debugSplit[2]), Convert.ToInt32(debugSplit[3])));
            goto case "break";
          case "pathspousetome":
          case "pstm":
            if (Game1.player.getSpouse() != null)
            {
              NPC spouse = Game1.player.getSpouse();
              if (spouse.currentLocation != Game1.currentLocation)
                Game1.warpCharacter(spouse, Game1.currentLocation.NameOrUniqueName, Game1.player.getTileLocationPoint());
              spouse.exploreFarm.Value = true;
              Game1.player.getSpouse().PathToOnFarm(Game1.player.getTileLocationPoint());
              goto case "break";
            }
            else
              goto case "break";
          case "pauseanimals":
            if (Game1.currentLocation is IAnimalLocation)
            {
              using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.ValuesCollection.Enumerator enumerator = (Game1.currentLocation as IAnimalLocation).Animals.Values.GetEnumerator())
              {
                while (enumerator.MoveNext())
                  enumerator.Current.pauseTimer = int.MaxValue;
                goto case "break";
              }
            }
            else
              goto case "break";
          case "pausetime":
            Game1.isTimePaused = !Game1.isTimePaused;
            if (Game1.isTimePaused)
            {
              Game1.playSound("bigSelect");
              goto case "break";
            }
            else
            {
              Game1.playSound("bigDeSelect");
              goto case "break";
            }
          case "perfection":
            this.parseDebugInput("friendAll");
            this.parseDebugInput("cooking");
            this.parseDebugInput("crafting");
            for (int index7 = Game1.player.craftingRecipes.Count() - 1; index7 >= 0; --index7)
              Game1.player.craftingRecipes[Game1.player.craftingRecipes.Pairs.ElementAt(index7).Key] = 1;
            foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>) Game1.objectInformation)
            {
              if (keyValuePair.Value.Split('/')[3].Contains("Fish"))
                Game1.player.fishCaught.Add(keyValuePair.Key, new int[3]);
              string str12 = keyValuePair.Value.Split('/')[3];
              if (Object.isPotentialBasicShippedCategory(keyValuePair.Key, str12.Substring(str12.Length - 3)))
                Game1.player.basicShipped.Add(keyValuePair.Key, 1);
              Game1.player.recipesCooked.Add(keyValuePair.Key, 1);
            }
            this.parseDebugInput("walnut 130");
            Game1.player.mailReceived.Add("CF_Fair");
            Game1.player.mailReceived.Add("CF_Fish");
            Game1.player.mailReceived.Add("CF_Sewer");
            Game1.player.mailReceived.Add("CF_Mines");
            Game1.player.mailReceived.Add("CF_Spouse");
            Game1.player.mailReceived.Add("CF_Statue");
            Game1.player.mailReceived.Add("museumComplete");
            Game1.player.miningLevel.Value = 10;
            Game1.player.fishingLevel.Value = 10;
            Game1.player.foragingLevel.Value = 10;
            Game1.player.combatLevel.Value = 10;
            Game1.player.farmingLevel.Value = 10;
            Game1.getFarm().buildStructure(new BluePrint("Water Obelisk"), new Vector2(0.0f, 0.0f), Game1.player, true, true);
            Game1.getFarm().buildStructure(new BluePrint("Earth Obelisk"), new Vector2(4f, 0.0f), Game1.player, true, true);
            Game1.getFarm().buildStructure(new BluePrint("Desert Obelisk"), new Vector2(8f, 0.0f), Game1.player, true, true);
            Game1.getFarm().buildStructure(new BluePrint("Island Obelisk"), new Vector2(12f, 0.0f), Game1.player, true, true);
            Game1.getFarm().buildStructure(new BluePrint("Gold Clock"), new Vector2(16f, 0.0f), Game1.player, true, true);
            using (Dictionary<string, string>.Enumerator enumerator = Game1.content.Load<Dictionary<string, string>>("Data\\Monsters").GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<string, string> current = enumerator.Current;
                for (int index8 = 0; index8 < 500; ++index8)
                  Game1.stats.monsterKilled(current.Key);
              }
              goto case "break";
            }
          case "pettofarm":
            Game1.getCharacterFromName<Pet>(Game1.player.getPetName(), false).setAtFarmPosition();
            goto case "break";
          case "pgb":
            Game1.debugOutput = "Gem birds: North " + IslandGemBird.GetBirdTypeForLocation("IslandNorth").ToString() + " South " + IslandGemBird.GetBirdTypeForLocation("IslandSouth").ToString() + " East " + IslandGemBird.GetBirdTypeForLocation("IslandEast").ToString() + " West " + IslandGemBird.GetBirdTypeForLocation("IslandWest").ToString();
            goto case "break";
          case "phone":
            this.ShowTelephoneMenu();
            goto case "break";
          case "pick":
          case "pickax":
          case "pickaxe":
            Game1.player.addItemToInventoryBool((Item) new Pickaxe());
            Game1.playSound("coin");
            goto case "break";
          case "plaque":
            (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).addStarToPlaque();
            goto case "break";
          case "playmusic":
            Game1.changeMusicTrack(debugSplit[1]);
            goto case "break";
          case "playsound":
          case "ps":
            Game1.playSound(debugSplit[1]);
            goto case "break";
          case "pole":
            Game1.player.addItemToInventoryBool((Item) new FishingRod(debugSplit.Length > 1 ? Convert.ToInt32(debugSplit[1]) : 0));
            goto case "break";
          case "ppp":
          case "printplayerpos":
            Game1.debugOutput = "Player tile position is " + Game1.player.getTileLocation().ToString() + " (World position: " + Game1.player.Position.ToString() + ")";
            goto case "break";
          case "pregnant":
            WorldDate date2 = Game1.Date;
            ++date2.TotalDays;
            Game1.player.GetSpouseFriendship().NextBirthingDate = date2;
            goto case "break";
          case "profession":
            Game1.player.professions.Add(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "qb":
          case "qiboard":
            Game1.activeClickableMenu = (IClickableMenu) new SpecialOrdersBoard("Qi");
            goto case "break";
          case "quest":
            Game1.player.questLog.Add(Quest.getQuestFromId(Convert.ToInt32(debugSplit[1])));
            goto case "break";
          case "question":
            Game1.player.dialogueQuestionsAnswered.Add(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "quests":
            foreach (int key in Game1.content.Load<Dictionary<int, string>>("Data\\Quests").Keys)
            {
              if (!Game1.player.hasQuest(key))
                Game1.player.addQuest(key);
            }
            Game1.player.questLog.Add((Quest) new ItemDeliveryQuest());
            Game1.player.questLog.Add((Quest) new SlayMonsterQuest());
            goto case "break";
          case "r":
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation.resetForPlayerEntry();
            goto case "break";
          case "rain":
            Game1.isRaining = !Game1.isRaining;
            Game1.isDebrisWeather = false;
            goto case "break";
          case "readyforharvest":
          case "rfh":
            Game1.currentLocation.objects[new Vector2((float) Convert.ToInt32(debugSplit[1]), (float) Convert.ToInt32(debugSplit[2]))].minutesUntilReady.Value = 1;
            goto case "break";
          case "refuel":
            if (Game1.player.getToolFromName("Lantern") != null)
            {
              ((Lantern) Game1.player.getToolFromName("Lantern")).fuelLeft = 100;
              goto case "break";
            }
            else
              goto case "break";
          case "removebuildings":
            Game1.getFarm().buildings.Clear();
            goto case "break";
          case "removedebris":
            Game1.currentLocation.debris.Clear();
            goto case "break";
          case "removedirt":
            for (int index9 = Game1.currentLocation.terrainFeatures.Count() - 1; index9 >= 0; --index9)
            {
              NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.PairsCollection pairs = Game1.currentLocation.terrainFeatures.Pairs;
              KeyValuePair<Vector2, TerrainFeature> keyValuePair = pairs.ElementAt(index9);
              if (keyValuePair.Value is HoeDirt)
              {
                NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = Game1.currentLocation.terrainFeatures;
                pairs = Game1.currentLocation.terrainFeatures.Pairs;
                keyValuePair = pairs.ElementAt(index9);
                Vector2 key = keyValuePair.Key;
                terrainFeatures.Remove(key);
              }
            }
            goto case "break";
          case "removefurniture":
            Game1.currentLocation.furniture.Clear();
            goto case "break";
          case "removelargetf":
            Game1.currentLocation.largeTerrainFeatures.Clear();
            goto case "break";
          case "removelights":
            Game1.currentLightSources.Clear();
            goto case "break";
          case "removenpc":
            foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
            {
              foreach (NPC character2 in location.characters)
              {
                if (character2.Name == debugSplit[1])
                {
                  location.characters.Remove(character2);
                  Game1.debugOutput = "Removed " + debugSplit[1] + " from " + location.Name;
                  return true;
                }
              }
              if (location is BuildableGameLocation)
              {
                foreach (Building building in (location as BuildableGameLocation).buildings)
                {
                  if (building.indoors.Value != null)
                  {
                    foreach (NPC character3 in building.indoors.Value.characters)
                    {
                      if (character3.Name == debugSplit[1])
                      {
                        building.indoors.Value.characters.Remove(character3);
                        Game1.debugOutput = "Removed " + debugSplit[1] + " from " + (string) (NetFieldBase<string, NetString>) building.indoors.Value.uniqueName;
                        return true;
                      }
                    }
                  }
                }
              }
            }
            Game1.debugOutput = "Couldn't find " + debugSplit[1];
            goto case "break";
          case "removeobjects":
            Game1.currentLocation.objects.Clear();
            goto case "break";
          case "removequest":
            Game1.player.removeQuest(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "removeterrainfeatures":
          case "removetf":
            Game1.currentLocation.terrainFeatures.Clear();
            goto case "break";
          case "renovate":
            HouseRenovation.ShowRenovationMenu();
            goto case "break";
          case "resetachievements":
            Program.sdk.ResetAchievements();
            goto case "break";
          case "resetjunimonotes":
            using (Dictionary<int, NetArray<bool, NetBool>>.ValueCollection.Enumerator enumerator = (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).bundles.FieldDict.Values.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                NetArray<bool, NetBool> current = enumerator.Current;
                for (int index10 = 0; index10 < current.Count; ++index10)
                  current[index10] = false;
              }
              goto case "break";
            }
          case "resetmines":
            MineShaft.permanentMineChanges.Clear();
            Game1.playSound("jingle1");
            goto case "break";
          case "resetworldstate":
            Game1.worldStateIDs.Clear();
            Game1.netWorldState.Value = (IWorldState) new NetWorldState();
            this.parseDebugInput("deleteArch");
            Game1.player.mailReceived.Clear();
            Game1.player.eventsSeen.Clear();
            goto case "break";
          case "resource":
            Debris.getDebris(Convert.ToInt32(debugSplit[1]), Convert.ToInt32(debugSplit[2]));
            goto case "break";
          case "returneddonations":
            Game1.player.team.CheckReturnedDonations();
            goto case "break";
          case "ring":
            Game1.player.addItemToInventoryBool((Item) new Ring(Convert.ToInt32(debugSplit[1])));
            Game1.playSound("coin");
            goto case "break";
          case "rm":
          case "runmacro":
            if (Game1.isRunningMacro)
            {
              Game1.debugOutput = "You cannot run a macro from within a macro.";
              goto case "break";
            }
            else
            {
              Game1.isRunningMacro = true;
              string path2 = "macro.txt";
              if (debugSplit.Length > 1)
                path2 = string.Join(" ", ((IEnumerable<string>) debugSplit).Skip<string>(1)) + ".txt";
              try
              {
                StreamReader streamReader = new StreamReader(path2);
                string text_to_send;
                while ((text_to_send = streamReader.ReadLine()) != null)
                  Game1.chatBox.textBoxEnter(text_to_send);
                Game1.debugOutput = "Executed macro file " + path2;
                streamReader.Close();
              }
              catch (Exception ex)
              {
                Game1.debugOutput = "Error running macro file " + path2 + "(" + ex.Message + ")";
              }
              Game1.isRunningMacro = false;
              goto case "break";
            }
          case "rte":
          case "runtestevent":
            Game1.runTestEvent();
            goto case "break";
          case "save":
            Game1.saveOnNewDay = !Game1.saveOnNewDay;
            if (Game1.saveOnNewDay)
            {
              Game1.playSound("bigSelect");
              goto case "break";
            }
            else
            {
              Game1.playSound("bigDeSelect");
              goto case "break";
            }
          case "sb":
            Utility.fuzzyCharacterSearch(debugSplit[1]).showTextAboveHead(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3206"));
            goto case "break";
          case "scd":
          case "skullcavedifficulty":
            Game1.netWorldState.Value.SkullCavesDifficulty = Convert.ToInt32(debugSplit[1]);
            Game1.debugOutput = "Skull Cave difficulty: " + Game1.netWorldState.Value.SkullCavesDifficulty.ToString();
            goto case "break";
          case "scissors":
          case "shears":
            Game1.player.addItemToInventoryBool((Item) new Shears());
            goto case "break";
          case "sdkinfo":
          case "steaminfo":
            Program.sdk.DebugInfo();
            goto case "break";
          case "season":
            if (debugSplit.Length >= 1 && Utility.getSeasonNumber(debugSplit[1].ToLower()) >= 0)
            {
              Game1.currentSeason = debugSplit[1].ToLower();
              Game1.setGraphicsForSeason();
              goto case "break";
            }
            else
              goto case "break";
          case "seenmail":
            Game1.player.mailReceived.Add(debugSplit[1]);
            goto case "break";
          case "separatewallets":
            if (Game1.IsMasterGame)
            {
              ManorHouse.SeparateWallets();
              goto case "break";
            }
            else
              goto case "break";
          case "setframe":
          case "sf":
            Game1.player.FarmerSprite.PauseForSingleAnimation = true;
            Game1.player.FarmerSprite.setCurrentSingleAnimation(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "setstat":
            Game1.stats.GetType().GetProperty(debugSplit[1]).SetValue((object) Game1.stats, (object) Convert.ToUInt32(debugSplit[2]), (object[]) null);
            goto case "break";
          case "setupbigfarm":
            this.parseDebugInput("clearFarm");
            this.parseDebugInput("build Deluxe9Coop 4 9");
            this.parseDebugInput("build Deluxe9Coop 10 9");
            this.parseDebugInput("build Deluxe9Coop 36 11");
            this.parseDebugInput("build Deluxe9Barn 16 9");
            this.parseDebugInput("build Deluxe9Barn 3 16");
            for (int index11 = 0; index11 < 48; ++index11)
              this.parseDebugInput("animal White Chicken");
            for (int index12 = 0; index12 < 32; ++index12)
              this.parseDebugInput("animal Cow");
            for (int index13 = 0; index13 < Game1.getFarm().buildings.Count<Building>(); ++index13)
              Game1.getFarm().buildings[index13].doAction(Utility.PointToVector2((Point) (NetFieldBase<Point, NetPoint>) Game1.getFarm().buildings[index13].animalDoor) + new Vector2((float) (int) (NetFieldBase<int, NetInt>) Game1.getFarm().buildings[index13].tileX, (float) (int) (NetFieldBase<int, NetInt>) Game1.getFarm().buildings[index13].tileY), Game1.player);
            this.parseDebugInput("build Mill 30 20");
            this.parseDebugInput("build Stable 46 10");
            this.parseDebugInput("build Silo 54 14");
            this.parseDebugInput("build Junimo9Hut 48 52");
            this.parseDebugInput("build Junimo9Hut 55 52");
            this.parseDebugInput("build Junimo9Hut 59 52");
            this.parseDebugInput("build Junimo9Hut 65 52");
            for (int x = 11; x < 23; ++x)
            {
              for (int y = 14; y < 25; ++y)
                Game1.getFarm().terrainFeatures.Add(new Vector2((float) x, (float) y), (TerrainFeature) new Grass(1, 4));
            }
            for (int x = 3; x < 23; ++x)
            {
              for (int y = 57; y < 61; ++y)
                Game1.getFarm().terrainFeatures.Add(new Vector2((float) x, (float) y), (TerrainFeature) new Grass(1, 4));
            }
            for (int y = 17; y < 25; ++y)
              Game1.getFarm().terrainFeatures.Add(new Vector2(64f, (float) y), (TerrainFeature) new Flooring(6));
            for (int x = 35; x < 64; ++x)
              Game1.getFarm().terrainFeatures.Add(new Vector2((float) x, 24f), (TerrainFeature) new Flooring(6));
            for (int index14 = 38; index14 < 76; ++index14)
            {
              for (int index15 = 18; index15 < 52; ++index15)
              {
                if (Game1.getFarm().isTileLocationTotallyClearAndPlaceable(new Vector2((float) index14, (float) index15)))
                {
                  Game1.getFarm().terrainFeatures.Add(new Vector2((float) index14, (float) index15), (TerrainFeature) new HoeDirt());
                  (Game1.getFarm().terrainFeatures[new Vector2((float) index14, (float) index15)] as HoeDirt).plant(472 + Game1.random.Next(5), index14, index15, Game1.player, false, (GameLocation) Game1.getFarm());
                }
              }
            }
            this.parseDebugInput("growCrops 8");
            Game1.getFarm().terrainFeatures.Add(new Vector2(8f, 25f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(11f, 25f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(14f, 25f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(17f, 25f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(20f, 25f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(23f, 25f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(8f, 28f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(11f, 28f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(14f, 28f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(17f, 28f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(20f, 28f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(23f, 28f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(8f, 31f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(11f, 31f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(14f, 31f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(17f, 31f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(20f, 31f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            Game1.getFarm().terrainFeatures.Add(new Vector2(23f, 31f), (TerrainFeature) new FruitTree(628 + Game1.random.Next(2), 4));
            for (int x = 3; x < 15; ++x)
            {
              for (int y = 36; y < 45; ++y)
              {
                if (Game1.getFarm().isTileLocationTotallyClearAndPlaceable(new Vector2((float) x, (float) y)))
                {
                  Game1.getFarm().objects.Add(new Vector2((float) x, (float) y), new Object(new Vector2((float) x, (float) y), 12));
                  Game1.getFarm().objects[new Vector2((float) x, (float) y)].performObjectDropInAction((Item) new Object(454, 1), false, Game1.player);
                }
              }
            }
            for (int x = 16; x < 26; ++x)
            {
              for (int y = 36; y < 45; ++y)
              {
                if (Game1.getFarm().isTileLocationTotallyClearAndPlaceable(new Vector2((float) x, (float) y)))
                  Game1.getFarm().objects.Add(new Vector2((float) x, (float) y), new Object(new Vector2((float) x, (float) y), 13));
              }
            }
            for (int x = 3; x < 15; ++x)
            {
              for (int y = 47; y < 57; ++y)
              {
                if (Game1.getFarm().isTileLocationTotallyClearAndPlaceable(new Vector2((float) x, (float) y)))
                  Game1.getFarm().objects.Add(new Vector2((float) x, (float) y), new Object(new Vector2((float) x, (float) y), 16));
              }
            }
            for (int x = 16; x < 26; ++x)
            {
              for (int y = 47; y < 57; ++y)
              {
                if (Game1.getFarm().isTileLocationTotallyClearAndPlaceable(new Vector2((float) x, (float) y)))
                  Game1.getFarm().objects.Add(new Vector2((float) x, (float) y), new Object(new Vector2((float) x, (float) y), 15));
              }
            }
            for (int x = 28; x < 38; ++x)
            {
              for (int y = 26; y < 46; ++y)
              {
                if (Game1.getFarm().isTileLocationTotallyClearAndPlaceable(new Vector2((float) x, (float) y)))
                  new Torch(new Vector2((float) x, (float) y), 1, 93).placementAction((GameLocation) Game1.getFarm(), x * 64, y * 64);
              }
            }
            goto case "break";
          case "setupfarm":
            Game1.getFarm().buildings.Clear();
            for (int x = 0; x < Game1.getFarm().map.Layers[0].LayerWidth; ++x)
            {
              for (int y = 0; y < 16 + (debugSplit.Length > 1 ? 32 : 0); ++y)
                Game1.getFarm().removeEverythingExceptCharactersFromThisTile(x, y);
            }
            for (int x = 56; x < 71; ++x)
            {
              for (int y = 17; y < 34; ++y)
              {
                Game1.getFarm().removeEverythingExceptCharactersFromThisTile(x, y);
                if (x > 57 && y > 18 && x < 70 && y < 29)
                  Game1.getFarm().terrainFeatures.Add(new Vector2((float) x, (float) y), (TerrainFeature) new HoeDirt());
              }
            }
            Game1.getFarm().buildStructure(new BluePrint("Coop"), new Vector2(52f, 11f), Game1.player);
            Game1.getFarm().buildings.Last<Building>().daysOfConstructionLeft.Value = 0;
            Game1.getFarm().buildStructure(new BluePrint("Silo"), new Vector2(36f, 9f), Game1.player);
            Game1.getFarm().buildings.Last<Building>().daysOfConstructionLeft.Value = 0;
            Game1.getFarm().buildStructure(new BluePrint("Barn"), new Vector2(42f, 10f), Game1.player);
            Game1.getFarm().buildings.Last<Building>().daysOfConstructionLeft.Value = 0;
            Game1.player.getToolFromName("Ax").UpgradeLevel = 4;
            Game1.player.getToolFromName("Watering Can").UpgradeLevel = 4;
            Game1.player.getToolFromName("Hoe").UpgradeLevel = 4;
            Game1.player.getToolFromName("Pickaxe").UpgradeLevel = 4;
            Game1.player.Money += 20000;
            Game1.player.addItemToInventoryBool((Item) new Shears());
            Game1.player.addItemToInventoryBool((Item) new MilkPail());
            Game1.player.addItemToInventoryBool((Item) new Object(472, 999));
            Game1.player.addItemToInventoryBool((Item) new Object(473, 999));
            Game1.player.addItemToInventoryBool((Item) new Object(322, 999));
            Game1.player.addItemToInventoryBool((Item) new Object(388, 999));
            Game1.player.addItemToInventoryBool((Item) new Object(390, 999));
            goto case "break";
          case "setupfishpondfarm":
            int num13 = ((IEnumerable<string>) debugSplit).Count<string>() > 1 ? Convert.ToInt32(debugSplit[1]) : 10;
            this.parseDebugInput("clearFarm");
            for (int index16 = 4; index16 < 77; index16 += 6)
            {
              for (int index17 = 9; index17 < 60; index17 += 6)
                this.parseDebugInput("build Fish9Pond " + index16.ToString() + " " + index17.ToString());
            }
            foreach (Building building in Game1.getFarm().buildings)
            {
              int key = Game1.random.Next(128, 159);
              if (Game1.random.NextDouble() < 0.15)
                key = Game1.random.Next(698, 724);
              if (Game1.random.NextDouble() < 0.05)
                key = Game1.random.Next(796, 801);
              if (Game1.objectInformation.ContainsKey(key) && Game1.objectInformation[key].Split('/')[3].Contains("-4"))
                (building as FishPond).fishType.Value = key;
              else
                (building as FishPond).fishType.Value = Game1.random.NextDouble() < 0.5 ? 393 : 397;
              (building as FishPond).maxOccupants.Value = 10;
              (building as FishPond).currentOccupants.Value = num13;
              (building as FishPond).GetFishObject();
            }
            this.parseDebugInput("dayUpdate 1");
            goto case "break";
          case "shirt":
            Game1.player.changeShirt(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "showMail":
          case "showmail":
            if (debugSplit.Length < 2)
            {
              Game1.debugOutput = "Not enough parameters, expecting: showMail <mailTitle>";
              goto case "break";
            }
            else
            {
              string str13 = debugSplit[1];
              Dictionary<string, string> dictionary2 = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
              Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(dictionary2.ContainsKey(str13) ? dictionary2[str13] : "", str13);
              goto case "break";
            }
          case "showexperience":
            Game1.debugOutput = Convert.ToString(Game1.player.experiencePoints[Convert.ToInt32(debugSplit[1])]);
            goto case "break";
          case "showplurals":
            List<string> stringList = new List<string>();
            foreach (string str14 in (IEnumerable<string>) Game1.objectInformation.Values)
              stringList.Add(str14.Split('/')[0]);
            foreach (string str15 in (IEnumerable<string>) Game1.bigCraftablesInformation.Values)
              stringList.Add(str15.Split('/')[0]);
            stringList.Sort();
            using (List<string>.Enumerator enumerator = stringList.GetEnumerator())
            {
              while (enumerator.MoveNext())
                Console.WriteLine(Lexicon.makePlural(enumerator.Current));
              goto case "break";
            }
          case "shufflebundles":
            Game1.GenerateBundles(Game1.BundleType.Remixed, false);
            goto case "break";
          case "skincolor":
            Game1.player.changeSkinColor(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "skullgear":
            Game1.player.hasSkullKey = true;
            Game1.player.MaxItems = 32;
            Game1.player.leftRing.Value = new Ring(527);
            Game1.player.rightRing.Value = new Ring(523);
            Game1.player.boots.Value = new Boots(514);
            Game1.player.clearBackpack();
            Pickaxe pickaxe = new Pickaxe();
            pickaxe.UpgradeLevel = 4;
            Game1.player.addItemToInventory((Item) pickaxe);
            Game1.player.addItemToInventory((Item) new MeleeWeapon(4));
            Game1.player.addItemToInventory((Item) new Object(226, 20));
            Game1.player.addItemToInventory((Item) new Object(288, 20));
            Game1.player.professions.Add(24);
            Game1.player.maxHealth = 75;
            goto case "break";
          case "skullkey":
            Game1.player.hasSkullKey = true;
            goto case "break";
          case "sl":
            Game1.player.shiftToolbar(false);
            goto case "break";
          case "slayquest":
            Game1.player.questLog.Add((Quest) new SlayMonsterQuest());
            goto case "break";
          case "slimecraft":
            Game1.player.craftingRecipes.Add("Slime Incubator", 0);
            Game1.player.craftingRecipes.Add("Slime Egg-Press", 0);
            Game1.playSound("crystal");
            goto case "break";
          case "slingshot":
            Game1.player.addItemToInventoryBool((Item) new Slingshot());
            Game1.playSound("coin");
            goto case "break";
          case "sn":
            Game1.player.hasMagnifyingGlass = true;
            if (debugSplit.Length > 1)
            {
              int int32_11 = Convert.ToInt32(debugSplit[1]);
              Object object1 = new Object(79, 1);
              Object object2 = object1;
              object2.name = object2.name + " #" + int32_11.ToString();
              Game1.player.addItemToInventory((Item) object1);
              goto case "break";
            }
            else
            {
              Game1.player.addItemToInventory((Item) Game1.currentLocation.tryToCreateUnseenSecretNote(Game1.player));
              goto case "break";
            }
          case "spawncoopsandbarns":
            if (Game1.currentLocation is Farm)
            {
              int int32_12 = Convert.ToInt32(debugSplit[1]);
              for (int index18 = 0; index18 < int32_12; ++index18)
              {
                for (int index19 = 0; index19 < 20; ++index19)
                {
                  bool flag2 = Game1.random.NextDouble() < 0.5;
                  if (Game1.getFarm().buildStructure(new BluePrint(flag2 ? "Deluxe Coop" : "Deluxe Barn"), Game1.getFarm().getRandomTile(), Game1.player))
                  {
                    Game1.getFarm().buildings.Last<Building>().daysOfConstructionLeft.Value = 0;
                    Game1.getFarm().buildings.Last<Building>().doAction(Utility.PointToVector2((Point) (NetFieldBase<Point, NetPoint>) Game1.getFarm().buildings.Last<Building>().animalDoor) + new Vector2((float) (int) (NetFieldBase<int, NetInt>) Game1.getFarm().buildings.Last<Building>().tileX, (float) (int) (NetFieldBase<int, NetInt>) Game1.getFarm().buildings.Last<Building>().tileY), Game1.player);
                    for (int index20 = 0; index20 < 16; ++index20)
                      Utility.addAnimalToFarm(new FarmAnimal(flag2 ? "White Chicken" : "Cow", (long) Game1.random.Next(int.MaxValue), (long) Game1.player.uniqueMultiplayerID));
                    break;
                  }
                }
              }
              goto case "break";
            }
            else
              goto case "break";
          case "spawnweeds":
            for (int index21 = 0; index21 < Convert.ToInt32(debugSplit[1]); ++index21)
              Game1.currentLocation.spawnWeedsAndStones(1);
            goto case "break";
          case "specialitem":
            Game1.player.specialItems.Add(Convert.ToInt32(debugSplit[1]));
            goto case "break";
          case "specialorder":
            SpecialOrder specialOrder = SpecialOrder.GetSpecialOrder(debugSplit[1], new int?());
            if (specialOrder != null)
            {
              Game1.player.team.specialOrders.Add(specialOrder);
              goto case "break";
            }
            else
              goto case "break";
          case "specials":
            Game1.player.hasRustyKey = true;
            Game1.player.hasSkullKey = true;
            Game1.player.hasSpecialCharm = true;
            Game1.player.hasDarkTalisman = true;
            Game1.player.hasMagicInk = true;
            Game1.player.hasClubCard = true;
            Game1.player.canUnderstandDwarves = true;
            Game1.player.hasMagnifyingGlass = true;
            Game1.player.eventsSeen.Add(2120303);
            Game1.player.eventsSeen.Add(3910979);
            Game1.player.HasTownKey = true;
            goto case "break";
          case "speech":
            Utility.fuzzyCharacterSearch(debugSplit[1]).CurrentDialogue.Push(new Dialogue(debugInput.Substring(debugInput.IndexOf("0") + 1), Utility.fuzzyCharacterSearch(debugSplit[1])));
            Game1.drawDialogue(Utility.fuzzyCharacterSearch(debugSplit[1]));
            goto case "break";
          case "speed":
            if (debugSplit.Length < 2)
            {
              Game1.debugOutput = "Missing parameters. Run as: 'speed <value> (minutes=30)'";
              goto case "break";
            }
            else
            {
              for (int index22 = Game1.buffsDisplay.otherBuffs.Count - 1; index22 >= 0; --index22)
              {
                if (Game1.buffsDisplay.otherBuffs[index22].source == "Debug Speed")
                {
                  Game1.buffsDisplay.otherBuffs[index22].removeBuff();
                  Game1.buffsDisplay.otherBuffs.RemoveAt(index22);
                }
              }
              int minutesDuration = 30;
              if (debugSplit.Length > 2)
                minutesDuration = Convert.ToInt32(debugSplit[2]);
              Game1.buffsDisplay.addOtherBuff(new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, Convert.ToInt32(debugSplit[1]), 0, 0, minutesDuration, "Debug Speed", "Debug Speed"));
              goto case "break";
            }
          case "split":
            if (debugSplit.Length >= 2)
            {
              int player_index = int.Parse(debugSplit[1]);
              GameRunner.instance.AddGameInstance((PlayerIndex) player_index);
              goto case "break";
            }
            else
            {
              this.ShowLocalCoopJoinMenu();
              goto case "break";
            }
          case "spreaddirt":
            Farm farm2 = Game1.getFarm();
            for (int index23 = 0; index23 < farm2.map.Layers[0].LayerWidth; ++index23)
            {
              for (int index24 = 0; index24 < farm2.map.Layers[0].LayerHeight; ++index24)
              {
                if (!farm2.terrainFeatures.ContainsKey(new Vector2((float) index23, (float) index24)) && farm2.doesTileHaveProperty(index23, index24, "Diggable", "Back") != null && farm2.isTileLocationTotallyClearAndPlaceable(new Vector2((float) index23, (float) index24)))
                  farm2.terrainFeatures.Add(new Vector2((float) index23, (float) index24), (TerrainFeature) new HoeDirt());
              }
            }
            goto case "break";
          case "spreadseeds":
            using (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.PairsCollection.Enumerator enumerator = Game1.getFarm().terrainFeatures.Pairs.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<Vector2, TerrainFeature> current = enumerator.Current;
                if (current.Value is HoeDirt)
                  (current.Value as HoeDirt).crop = new Crop(Convert.ToInt32(debugSplit[1]), (int) current.Key.X, (int) current.Key.Y);
              }
              goto case "break";
            }
          case "sprinkle":
            Utility.addSprinklesToLocation(Game1.currentLocation, Game1.player.getTileX(), Game1.player.getTileY(), 7, 7, 2000, 100, Microsoft.Xna.Framework.Color.White);
            goto case "break";
          case "sr":
            Game1.player.shiftToolbar(true);
            goto case "break";
          case "stoprafting":
            Game1.player.isRafting = false;
            goto case "break";
          case "tailor":
            Game1.activeClickableMenu = (IClickableMenu) new TailoringMenu();
            goto case "break";
          case "tailorrecipelisttool":
          case "trlt":
            Game1.activeClickableMenu = (IClickableMenu) new TailorRecipeListTool();
            goto case "break";
          case "test":
            Game1.currentMinigame = (IMinigame) new Test();
            goto case "break";
          case "testnut":
            Game1.createItemDebris((Item) new Object(73, 1), Vector2.Zero, 2);
            goto case "break";
          case "time":
            Game1.timeOfDay = Convert.ToInt32(debugSplit[1]);
            Game1.outdoorLight = Microsoft.Xna.Framework.Color.White;
            goto case "break";
          case "tls":
            this.useUnscaledLighting = !this.useUnscaledLighting;
            Game1.debugOutput = "Toggled Lighting Scale: useUnscaledLighting: " + this.useUnscaledLighting.ToString();
            goto case "break";
          case "togglecatperson":
            Game1.player.catPerson = !Game1.player.catPerson;
            goto case "break";
          case "tool":
            Game1.player.getToolFromName(debugSplit[1]).UpgradeLevel = Convert.ToInt32(debugSplit[2]);
            goto case "break";
          case "toss":
            Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(738, 2700f, 1, 0, Game1.player.getTileLocation() * 64f, false, false)
            {
              rotationChange = (float) Math.PI / 32f,
              motion = new Vector2(0.0f, -6f),
              acceleration = new Vector2(0.0f, 0.08f)
            });
            goto case "break";
          case "townkey":
            Game1.player.HasTownKey = true;
            goto case "break";
          case "train":
            (Game1.getLocationFromName("Railroad") as Railroad).setTrainComing(7500);
            goto case "break";
          case "trashcan":
            Game1.player.trashCanLevel = Convert.ToInt32(debugSplit[1]);
            goto case "break";
          case "tv":
            Game1.player.addItemToInventoryBool((Item) new TV(Game1.random.NextDouble() < 0.5 ? 1466 : 1468, Vector2.Zero));
            goto case "break";
          case "uiscale":
          case "us":
            Game1.options.desiredUIScale = (float) Convert.ToInt32(debugSplit[1]) / 100f;
            goto case "break";
          case "unpauseanimals":
            if (Game1.currentLocation is IAnimalLocation)
            {
              using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.ValuesCollection.Enumerator enumerator = (Game1.currentLocation as IAnimalLocation).Animals.Values.GetEnumerator())
              {
                while (enumerator.MoveNext())
                  enumerator.Current.pauseTimer = 0;
                goto case "break";
              }
            }
            else
              goto case "break";
          case "upgradehouse":
            Game1.player.HouseUpgradeLevel = Math.Min(3, Game1.player.HouseUpgradeLevel + 1);
            Game1.removeFrontLayerForFarmBuildings();
            Game1.addNewFarmBuildingMaps();
            goto case "break";
          case "version":
            Game1.debugOutput = typeof (Game1).Assembly.GetName().Version?.ToString() ?? "";
            goto case "break";
          case "viewport":
            Game1.viewport.X = Convert.ToInt32(debugSplit[1]) * 64;
            Game1.viewport.Y = Convert.ToInt32(debugSplit[2]) * 64;
            goto case "break";
          case "volcano":
            Game1.warpFarmer("VolcanoDungeon" + Convert.ToInt32(debugSplit[1]).ToString(), 0, 1, 2);
            goto case "break";
          case "w":
          case "wall":
            (Game1.getLocationFromName("FarmHouse") as FarmHouse).SetWallpaper(debugSplit[1], (string) null);
            goto case "break";
          case "wallpaper":
          case "wp":
            if (((IEnumerable<string>) debugSplit).Count<string>() > 1)
            {
              Game1.player.addItemToInventoryBool((Item) new Wallpaper(Convert.ToInt32(debugSplit[1])));
              goto case "break";
            }
            else
            {
              bool isFloor = Game1.random.NextDouble() < 0.5;
              Game1.player.addItemToInventoryBool((Item) new Wallpaper(isFloor ? Game1.random.Next(40) : Game1.random.Next(112), isFloor));
              goto case "break";
            }
          case "walnut":
            Game1.netWorldState.Value.GoldenWalnuts.Value += Convert.ToInt32(debugSplit[1]);
            Game1.netWorldState.Value.GoldenWalnutsFound.Value += Convert.ToInt32(debugSplit[1]);
            goto case "break";
          case "wand":
            Game1.player.addItemToInventoryBool((Item) new Wand());
            Game1.playSound("coin");
            goto case "break";
          case "warp":
            GameLocation location1 = Utility.fuzzyLocationSearch(debugSplit[1]);
            if (location1 != null)
            {
              int x = 0;
              int y = 0;
              if (debugSplit.Length >= 4)
              {
                x = Convert.ToInt32(debugSplit[2]);
                y = Convert.ToInt32(debugSplit[3]);
              }
              else
                Utility.getDefaultWarpLocation(location1.Name, ref x, ref y);
              Game1.warpFarmer(new LocationRequest(location1.NameOrUniqueName, location1.uniqueName.Value != null, location1), x, y, 2);
              Game1.debugOutput = "Warping player to " + location1.NameOrUniqueName + " at " + x.ToString() + ", " + y.ToString();
              goto case "break";
            }
            else
            {
              Game1.debugOutput = "No location with name " + debugSplit[1];
              goto case "break";
            }
          case "warpanimaltome":
          case "watm":
            if (!(Game1.currentLocation is IAnimalLocation))
            {
              Game1.debugOutput = "Animals not allowed in current location.";
              goto case "break";
            }
            else
            {
              IAnimalLocation currentLocation = Game1.currentLocation as IAnimalLocation;
              FarmAnimal farmAnimal = Utility.fuzzyAnimalSearch(debugSplit[1]);
              if (farmAnimal != null)
              {
                Game1.debugOutput = "Warping " + farmAnimal.displayName;
                (farmAnimal.currentLocation as IAnimalLocation).Animals.Remove((long) farmAnimal.myID);
                currentLocation.Animals.Add((long) farmAnimal.myID, farmAnimal);
                farmAnimal.Position = Game1.player.Position;
                farmAnimal.controller = (PathFindController) null;
                goto case "break";
              }
              else
              {
                Game1.debugOutput = "Couldn't find character named " + debugSplit[1];
                goto case "break";
              }
            }
          case "warpcharacter":
          case "wc":
            NPC character4 = Utility.fuzzyCharacterSearch(debugSplit[1], false);
            if (character4 != null)
            {
              if (debugSplit.Length < 4)
              {
                Game1.debugOutput = "Missing parameters, run as: 'wc <npcName> <x> <y> [facingDirection=1]'";
                goto case "break";
              }
              else
              {
                int direction = 2;
                if (debugSplit.Length >= 5)
                  direction = Convert.ToInt32(debugSplit[4]);
                Game1.warpCharacter(character4, Game1.currentLocation.Name, new Vector2((float) Convert.ToInt32(debugSplit[2]), (float) Convert.ToInt32(debugSplit[3])));
                character4.faceDirection(direction);
                character4.controller = (PathFindController) null;
                character4.Halt();
                goto case "break";
              }
            }
            else
              goto case "break";
          case "warpcharacterto":
          case "wct":
            NPC character5 = Utility.fuzzyCharacterSearch(debugSplit[1]);
            if (character5 != null)
            {
              if (debugSplit.Length < 5)
              {
                Game1.debugOutput = "Missing parameters, run as: 'wct <npcName> <locationName> <x> <y> [facingDirection=1]'";
                goto case "break";
              }
              else
              {
                int direction = 2;
                if (debugSplit.Length >= 6)
                  direction = Convert.ToInt32(debugSplit[4]);
                Game1.warpCharacter(character5, debugSplit[2], new Vector2((float) Convert.ToInt32(debugSplit[3]), (float) Convert.ToInt32(debugSplit[4])));
                character5.faceDirection(direction);
                character5.controller = (PathFindController) null;
                character5.Halt();
                goto case "break";
              }
            }
            else
              goto case "break";
          case "warpcharactertome":
          case "wctm":
            NPC character6 = Utility.fuzzyCharacterSearch(debugSplit[1], false);
            if (character6 != null)
            {
              Game1.debugOutput = "Warping " + character6.displayName;
              Game1.warpCharacter(character6, Game1.currentLocation.Name, new Vector2((float) Game1.player.getTileX(), (float) Game1.player.getTileY()));
              character6.controller = (PathFindController) null;
              character6.Halt();
              goto case "break";
            }
            else
            {
              Game1.debugOutput = "Couldn't find character named " + debugSplit[1];
              goto case "break";
            }
          case "warphome":
          case "wh":
            Game1.warpHome();
            goto case "break";
          case "warpshop":
          case "ws":
            if (debugSplit.Length < 2)
            {
              Game1.debugOutput = "Missing argument. Run as: 'warpshop <npcname>'";
              goto case "break";
            }
            else
            {
              switch (debugSplit[1].ToLower())
              {
                case "clint":
                  this.parseDebugInput("warp Blacksmith 3 15");
                  this.parseDebugInput("wct Clint Blacksmith 3 13");
                  goto label_1159;
                case "dwarf":
                  this.parseDebugInput("warp Mine 43 7");
                  goto label_1159;
                case "gus":
                  this.parseDebugInput("warp Saloon 10 20");
                  this.parseDebugInput("wct Gus Saloon 10 18");
                  goto label_1159;
                case "krobus":
                  this.parseDebugInput("warp Sewer 31 19");
                  goto label_1159;
                case "marnie":
                  this.parseDebugInput("warp AnimalShop 12 16");
                  this.parseDebugInput("wct Marnie AnimalShop 12 14");
                  goto label_1159;
                case "pam":
                  this.parseDebugInput("warp BusStop 7 12");
                  this.parseDebugInput("wct Pam BusStop 11 10");
                  goto label_1159;
                case "pierre":
                  this.parseDebugInput("warp SeedShop 4 19");
                  this.parseDebugInput("wct Pierre SeedShop 4 17");
                  goto label_1159;
                case "robin":
                  this.parseDebugInput("warp ScienceHouse 8 20");
                  this.parseDebugInput("wct Robin ScienceHouse 8 18");
                  goto label_1159;
                case "sandy":
                  this.parseDebugInput("warp SandyHouse 2 7");
                  this.parseDebugInput("wct Sandy SandyHouse 2 5");
                  goto label_1159;
                case "willy":
                  this.parseDebugInput("warp FishShop 6 6");
                  this.parseDebugInput("wct Willy FishShop 6 4");
                  goto label_1159;
                case "wizard":
                  if (!Game1.player.eventsSeen.Contains(418172))
                    Game1.player.eventsSeen.Add(418172);
                  Game1.player.hasMagicInk = true;
                  this.parseDebugInput("warp WizardHouse 2 14");
                  goto label_1159;
                default:
                  Game1.debugOutput = "That npc doesn't have a shop or it isn't handled by this command";
                  goto label_1159;
              }
            }
          case "warptocharacter":
          case "wtc":
            if (debugSplit.Length < 2)
            {
              Game1.debugOutput = "Missing parameters, run as: 'wtc <npcName>'";
              goto case "break";
            }
            else
            {
              NPC n2 = Utility.fuzzyCharacterSearch(debugSplit[1]);
              if (n2 == null)
              {
                Game1.debugOutput = "Could not find valid character " + debugSplit[1];
                goto case "break";
              }
              else
              {
                this.parseDebugInput("warp " + Utility.getGameLocationOfCharacter(n2).Name + " " + n2.getTileX().ToString() + " " + n2.getTileY().ToString());
                goto case "break";
              }
            }
          case "warptoplayer":
          case "wtp":
            if (debugSplit.Length < 2)
            {
              Game1.debugOutput = "Missing parameters, run as: 'wtp <playerName>'";
              goto case "break";
            }
            else
            {
              string str16 = debugSplit[1].ToLower().Replace(" ", "");
              Farmer farmer = (Farmer) null;
              foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
              {
                if (onlineFarmer.displayName.Replace(" ", "").ToLower() == str16)
                {
                  farmer = onlineFarmer;
                  break;
                }
              }
              if (farmer == null)
              {
                Game1.debugOutput = "Could not find other farmer " + debugSplit[1];
                goto case "break";
              }
              else
              {
                this.parseDebugInput("warp " + farmer.currentLocation.NameOrUniqueName + " " + farmer.getTileX().ToString() + " " + farmer.getTileY().ToString());
                goto case "break";
              }
            }
          case "water":
            using (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.ValuesCollection.Enumerator enumerator = Game1.currentLocation.terrainFeatures.Values.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                TerrainFeature current = enumerator.Current;
                if (current is HoeDirt)
                  (current as HoeDirt).state.Value = 1;
              }
              goto case "break";
            }
          case "watercolor":
            Game1.currentLocation.waterColor.Value = new Microsoft.Xna.Framework.Color(Convert.ToInt32(debugSplit[1]), Convert.ToInt32(debugSplit[2]), Convert.ToInt32(debugSplit[3])) * 0.5f;
            goto case "break";
          case "weapon":
            Game1.player.addItemToInventoryBool((Item) new MeleeWeapon(Convert.ToInt32(debugSplit[1])));
            goto case "break";
          case "wedding":
            Game1.player.spouse = debugSplit[1];
            Game1.weddingsToday.Add(Game1.player.UniqueMultiplayerID);
            goto case "break";
          case "where":
          case "whereis":
            NPC n3 = Utility.fuzzyCharacterSearch(debugSplit[1], false);
            string[] strArray1 = new string[7]
            {
              n3.Name,
              " is at ",
              Utility.getGameLocationOfCharacter(n3).NameOrUniqueName,
              ", ",
              null,
              null,
              null
            };
            int num14 = n3.getTileX();
            strArray1[4] = num14.ToString();
            strArray1[5] = ",";
            num14 = n3.getTileY();
            strArray1[6] = num14.ToString();
            Game1.debugOutput = string.Concat(strArray1);
            goto case "break";
          case "whereore":
            Game1.debugOutput = Convert.ToString((object) Game1.currentLocation.orePanPoint.Value);
            goto case "break";
          case "year":
            Game1.year = Convert.ToInt32(debugSplit[1]);
            goto case "break";
          case "zl":
          case "zoomlevel":
            Game1.options.desiredBaseZoomLevel = (float) Convert.ToInt32(debugSplit[1]) / 100f;
            goto case "break";
          default:
            return false;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Debug command error: " + ex?.ToString());
        Game1.debugOutput = ex.Message;
        return false;
      }
    }

    public void RecountWalnuts()
    {
      if (!Game1.IsMasterGame || !(Game1.getLocationFromName("IslandHut") is IslandHut locationFromName))
        return;
      int num = 130 - locationFromName.ShowNutHint();
      Game1.netWorldState.Value.GoldenWalnutsFound.Value = num;
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        if (location is IslandLocation)
        {
          foreach (ParrotUpgradePerch parrotUpgradePerch in (location as IslandLocation).parrotUpgradePerches)
          {
            if (parrotUpgradePerch.currentState.Value == ParrotUpgradePerch.UpgradeState.Complete)
              num -= (int) (NetFieldBase<int, NetInt>) parrotUpgradePerch.requiredNuts;
          }
        }
      }
      if (Game1.MasterPlayer.hasOrWillReceiveMail("Island_VolcanoShortcutOut"))
        num -= 5;
      if (Game1.MasterPlayer.hasOrWillReceiveMail("Island_VolcanoBridge"))
        num -= 5;
      Game1.netWorldState.Value.GoldenWalnuts.Value = num;
    }

    public void ResetIslandLocations()
    {
      Game1.netWorldState.Value.GoldenWalnutsFound.Value = 0;
      string[] strArray = new string[14]
      {
        "birdieQuestBegun",
        "birdieQuestFinished",
        "tigerSlimeNut",
        "Island_W_BuriedTreasureNut",
        "Island_W_BuriedTreasure",
        "islandNorthCaveOpened",
        "Saw_Flame_Sprite_North_North",
        "Saw_Flame_Sprite_North_South",
        "Island_N_BuriedTreasureNut",
        "Island_W_BuriedTreasure",
        "Saw_Flame_Sprite_South",
        "Visited_Island",
        "Island_FirstParrot",
        "gotBirdieReward"
      };
      Game1.player.team.collectedNutTracker.Clear();
      for (int index1 = 0; index1 < Game1.player.mailReceived.Count; ++index1)
      {
        if (Game1.player.mailReceived[index1].StartsWith("Island_Upgrade"))
        {
          Game1.player.mailReceived.RemoveAt(index1);
          --index1;
        }
        else
        {
          for (int index2 = 0; index2 < strArray.Length; ++index2)
          {
            if (Game1.player.mailReceived[index1].Contains(strArray[index2]))
            {
              Game1.player.mailReceived.RemoveAt(index1);
              --index1;
              break;
            }
          }
        }
      }
      for (int index3 = 0; index3 < Game1.player.mailForTomorrow.Count; ++index3)
      {
        if (Game1.player.mailForTomorrow[index3].StartsWith("Island_Upgrade"))
        {
          Game1.player.mailForTomorrow.RemoveAt(index3);
          --index3;
        }
        for (int index4 = 0; index4 < strArray.Length; ++index4)
        {
          if (Game1.player.mailForTomorrow[index3].Contains(strArray[index4]))
          {
            Game1.player.mailForTomorrow.RemoveAt(index3);
            --index3;
            break;
          }
        }
      }
      for (int index5 = 0; index5 < Game1.player.team.broadcastedMail.Count; ++index5)
      {
        if (Game1.player.team.broadcastedMail[index5].StartsWith("Island_Upgrade"))
        {
          Game1.player.team.broadcastedMail.RemoveAt(index5);
          --index5;
        }
        for (int index6 = 0; index6 < strArray.Length; ++index6)
        {
          if (Game1.player.team.broadcastedMail[index5].Contains(strArray[index6]))
          {
            Game1.player.team.broadcastedMail.RemoveAt(index5);
            --index5;
            break;
          }
        }
      }
      for (int index = 0; index < Game1.player.secretNotesSeen.Count; ++index)
      {
        if (Game1.player.secretNotesSeen[index] >= 1000)
        {
          Game1.player.secretNotesSeen.RemoveAt(index);
          --index;
        }
      }
      Game1.player.team.limitedNutDrops.Clear();
      Game1.netWorldState.Value.GoldenCoconutCracked.Value = false;
      Game1.netWorldState.Value.GoldenWalnuts.Set(0);
      Game1.netWorldState.Value.ParrotPlatformsUnlocked.Value = false;
      Game1.netWorldState.Value.FoundBuriedNuts.Clear();
      for (int index = 0; index < Game1.locations.Count; ++index)
      {
        GameLocation location = Game1.locations[index];
        if (location.GetLocationContext() == GameLocation.LocationContext.Island)
        {
          Game1._locationLookup.Clear();
          object[] objArray = new object[2]
          {
            (object) location.mapPath.Value,
            (object) location.name.Value
          };
          GameLocation instance;
          try
          {
            instance = Activator.CreateInstance(location.GetType(), objArray) as GameLocation;
          }
          catch (Exception ex)
          {
            instance = Activator.CreateInstance(location.GetType()) as GameLocation;
          }
          Game1.locations[index] = instance;
          Game1._locationLookup.Clear();
        }
      }
      Game1.addBirdieIfNecessary();
    }

    public void ShowTelephoneMenu()
    {
      Game1.playSound("openBox");
      List<Response> responseList = new List<Response>();
      responseList.Add(new Response("Carpenter", Game1.getCharacterFromName("Robin").displayName));
      responseList.Add(new Response("Blacksmith", Game1.getCharacterFromName("Clint").displayName));
      responseList.Add(new Response("SeedShop", Game1.getCharacterFromName("Pierre").displayName));
      responseList.Add(new Response("AnimalShop", Game1.getCharacterFromName("Marnie").displayName));
      responseList.Add(new Response("Saloon", Game1.getCharacterFromName("Gus").displayName));
      if (Game1.player.mailReceived.Contains("Gil_Telephone"))
        responseList.Add(new Response("AdventureGuild", Game1.getCharacterFromName("Marlon").displayName));
      responseList.Add(new Response("HangUp", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")));
      Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectNumber"), responseList.ToArray(), "telephone");
    }

    public void requestDebugInput()
    {
      Game1.chatBox.activate();
      Game1.chatBox.setText("/");
    }

    private void makeCelebrationWeatherDebris()
    {
      Game1.debrisWeather.Clear();
      Game1.isDebrisWeather = true;
      int num1 = Game1.random.Next(80, 100);
      int num2 = 22;
      for (int index = 0; index < num1; ++index)
        Game1.debrisWeather.Add(new WeatherDebris(new Vector2((float) Game1.random.Next(0, Game1.graphics.GraphicsDevice.Viewport.Width), (float) Game1.random.Next(0, Game1.graphics.GraphicsDevice.Viewport.Height)), num2 + Game1.random.Next(2), (float) Game1.random.Next(15) / 500f, (float) Game1.random.Next(-10, 0) / 50f, (float) Game1.random.Next(10) / 50f));
    }

    private void panModeSuccess(KeyboardState currentKBState)
    {
      this.panFacingDirectionWait = false;
      Game1.playSound("smallSelect");
      if (currentKBState.IsKeyDown(Keys.LeftShift))
        this.panModeString += " (animation_name_here)";
      Game1.debugOutput = this.panModeString;
    }

    private void updatePanModeControls(MouseState currentMouseState, KeyboardState currentKBState)
    {
      if (currentKBState.IsKeyDown(Keys.F8) && !Game1.oldKBState.IsKeyDown(Keys.F8))
      {
        this.requestDebugInput();
      }
      else
      {
        if (!this.panFacingDirectionWait)
        {
          if (currentKBState.IsKeyDown(Keys.W))
            Game1.viewport.Y -= 16;
          if (currentKBState.IsKeyDown(Keys.A))
            Game1.viewport.X -= 16;
          if (currentKBState.IsKeyDown(Keys.S))
            Game1.viewport.Y += 16;
          if (currentKBState.IsKeyDown(Keys.D))
            Game1.viewport.X += 16;
        }
        else
        {
          if (currentKBState.IsKeyDown(Keys.W))
          {
            this.panModeString += "0";
            this.panModeSuccess(currentKBState);
          }
          if (currentKBState.IsKeyDown(Keys.A))
          {
            this.panModeString += "3";
            this.panModeSuccess(currentKBState);
          }
          if (currentKBState.IsKeyDown(Keys.S))
          {
            this.panModeString += "2";
            this.panModeSuccess(currentKBState);
          }
          if (currentKBState.IsKeyDown(Keys.D))
          {
            this.panModeString += "1";
            this.panModeSuccess(currentKBState);
          }
        }
        if (Game1.getMouseX(false) < 192)
        {
          Game1.viewport.X -= 8;
          Game1.viewport.X -= (192 - Game1.getMouseX()) / 8;
        }
        if (Game1.getMouseX(false) > Game1.viewport.Width - 192)
        {
          Game1.viewport.X += 8;
          Game1.viewport.X += (Game1.getMouseX() - Game1.viewport.Width + 192) / 8;
        }
        if (Game1.getMouseY(false) < 192)
        {
          Game1.viewport.Y -= 8;
          Game1.viewport.Y -= (192 - Game1.getMouseY()) / 8;
        }
        if (Game1.getMouseY(false) > Game1.viewport.Height - 192)
        {
          Game1.viewport.Y += 8;
          Game1.viewport.Y += (Game1.getMouseY() - Game1.viewport.Height + 192) / 8;
        }
        if (currentMouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released && this.panModeString != null && this.panModeString.Length > 0)
        {
          int x = (Game1.getMouseX() + Game1.viewport.X) / 64;
          int y = (Game1.getMouseY() + Game1.viewport.Y) / 64;
          this.panModeString = this.panModeString + Game1.currentLocation.Name + " " + x.ToString() + " " + y.ToString() + " ";
          this.panFacingDirectionWait = true;
          Game1.currentLocation.playTerrainSound(new Vector2((float) x, (float) y));
          Game1.debugOutput = this.panModeString;
        }
        if (currentMouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton == ButtonState.Released)
        {
          Warp warp = Game1.currentLocation.isCollidingWithWarpOrDoor(new Microsoft.Xna.Framework.Rectangle(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, 1, 1));
          if (warp != null)
          {
            Game1.currentLocation = Game1.getLocationFromName(warp.TargetName);
            Game1.currentLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
            Game1.viewport.X = warp.TargetX * 64 - Game1.viewport.Width / 2;
            Game1.viewport.Y = warp.TargetY * 64 - Game1.viewport.Height / 2;
            Game1.playSound("dwop");
          }
        }
        if (currentKBState.IsKeyDown(Keys.Escape) && !Game1.oldKBState.IsKeyDown(Keys.Escape))
        {
          Warp warp = Game1.currentLocation.warps[0];
          Game1.currentLocation = Game1.getLocationFromName(warp.TargetName);
          Game1.currentLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
          Game1.viewport.X = warp.TargetX * 64 - Game1.viewport.Width / 2;
          Game1.viewport.Y = warp.TargetY * 64 - Game1.viewport.Height / 2;
          Game1.playSound("dwop");
        }
        if (Game1.viewport.X < -64)
          Game1.viewport.X = -64;
        if (Game1.viewport.X + Game1.viewport.Width > Game1.currentLocation.Map.Layers[0].LayerWidth * 64 + 128)
          Game1.viewport.X = Game1.currentLocation.Map.Layers[0].LayerWidth * 64 + 128 - Game1.viewport.Width;
        if (Game1.viewport.Y < -64)
          Game1.viewport.Y = -64;
        if (Game1.viewport.Y + Game1.viewport.Height > Game1.currentLocation.Map.Layers[0].LayerHeight * 64 + 128)
          Game1.viewport.Y = Game1.currentLocation.Map.Layers[0].LayerHeight * 64 + 128 - Game1.viewport.Height;
        Game1.oldMouseState = Game1.input.GetMouseState();
        Game1.oldKBState = currentKBState;
      }
    }

    public static bool isLocationAccessible(string locationName)
    {
      if (!(locationName == "CommunityCenter"))
      {
        if (!(locationName == "JojaMart"))
        {
          if (!(locationName == "Railroad") || Game1.stats.DaysPlayed > 31U)
            return true;
        }
        else if (!Utility.HasAnyPlayerSeenEvent(191393))
          return true;
      }
      else if (Game1.player.eventsSeen.Contains(191393))
        return true;
      return false;
    }

    public static bool isDPadPressed() => Game1.isDPadPressed(Game1.input.GetGamePadState());

    public static bool isDPadPressed(GamePadState pad_state) => pad_state.DPad.Up == ButtonState.Pressed || pad_state.DPad.Down == ButtonState.Pressed || pad_state.DPad.Left == ButtonState.Pressed || pad_state.DPad.Right == ButtonState.Pressed;

    public static bool isGamePadThumbstickInMotion(double threshold = 0.2)
    {
      bool flag = false;
      GamePadState gamePadState = Game1.input.GetGamePadState();
      if ((double) gamePadState.ThumbSticks.Left.X < -threshold || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft))
        flag = true;
      if ((double) gamePadState.ThumbSticks.Left.X > threshold || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight))
        flag = true;
      if ((double) gamePadState.ThumbSticks.Left.Y < -threshold || gamePadState.IsButtonDown(Buttons.LeftThumbstickUp))
        flag = true;
      if ((double) gamePadState.ThumbSticks.Left.Y > threshold || gamePadState.IsButtonDown(Buttons.LeftThumbstickDown))
        flag = true;
      if ((double) gamePadState.ThumbSticks.Right.X < -threshold)
        flag = true;
      if ((double) gamePadState.ThumbSticks.Right.X > threshold)
        flag = true;
      if ((double) gamePadState.ThumbSticks.Right.Y < -threshold)
        flag = true;
      if ((double) gamePadState.ThumbSticks.Right.Y > threshold)
        flag = true;
      if (flag)
        Game1.thumbstickMotionMargin = 50;
      return Game1.thumbstickMotionMargin > 0;
    }

    public static bool isAnyGamePadButtonBeingPressed() => Utility.getPressedButtons(Game1.input.GetGamePadState(), Game1.oldPadState).Count > 0;

    public static bool isAnyGamePadButtonBeingHeld() => Utility.getHeldButtons(Game1.input.GetGamePadState()).Count > 0;

    private static void UpdateChatBox()
    {
      if (Game1.chatBox == null)
        return;
      KeyboardState keyboardState = Game1.input.GetKeyboardState();
      GamePadState gamePadState = Game1.input.GetGamePadState();
      if (Game1.IsChatting)
      {
        if (Game1.textEntry != null)
          return;
        if (gamePadState.IsButtonDown(Buttons.A))
        {
          MouseState mouseState = Game1.input.GetMouseState();
          if (Game1.chatBox != null && Game1.chatBox.isActive() && !Game1.chatBox.isHoveringOverClickable(mouseState.X, mouseState.Y))
          {
            Game1.oldPadState = gamePadState;
            Game1.oldKBState = keyboardState;
            Game1.showTextEntry((TextBox) Game1.chatBox.chatBox);
          }
        }
        if (!keyboardState.IsKeyDown(Keys.Escape) && !gamePadState.IsButtonDown(Buttons.B) && !gamePadState.IsButtonDown(Buttons.Back))
          return;
        Game1.chatBox.clickAway();
        Game1.oldKBState = keyboardState;
      }
      else
      {
        if (Game1.keyboardDispatcher.Subscriber != null || (!Game1.isOneOfTheseKeysDown(keyboardState, Game1.options.chatButton) || !Game1.game1.HasKeyboardFocus()) && (gamePadState.IsButtonDown(Buttons.RightStick) || Game1.rightStickHoldTime <= 0 || Game1.rightStickHoldTime >= Game1.emoteMenuShowTime))
          return;
        Game1.chatBox.activate();
        if (!keyboardState.IsKeyDown(Keys.OemQuestion))
          return;
        Game1.chatBox.setText("/");
      }
    }

    public static KeyboardState GetKeyboardState()
    {
      KeyboardState keyboardState = Game1.input.GetKeyboardState();
      if (Game1.chatBox != null)
      {
        if (Game1.IsChatting)
          return new KeyboardState();
        if (Game1.keyboardDispatcher.Subscriber == null && Game1.isOneOfTheseKeysDown(keyboardState, Game1.options.chatButton) && Game1.game1.HasKeyboardFocus())
          return new KeyboardState();
      }
      return keyboardState;
    }

    private void UpdateControlInput(GameTime time)
    {
      KeyboardState currentKBState = Game1.GetKeyboardState();
      MouseState currentMouseState = Game1.input.GetMouseState();
      GamePadState currentPadState = Game1.input.GetGamePadState();
      if (Game1.ticks < Game1._activatedTick + 2 && Game1.oldKBState.IsKeyDown(Keys.Tab) != currentKBState.IsKeyDown(Keys.Tab))
      {
        List<Keys> list = ((IEnumerable<Keys>) Game1.oldKBState.GetPressedKeys()).ToList<Keys>();
        if (currentKBState.IsKeyDown(Keys.Tab))
          list.Add(Keys.Tab);
        else
          list.Remove(Keys.Tab);
        Game1.oldKBState = new KeyboardState(list.ToArray());
      }
      Game1.hooks.OnGame1_UpdateControlInput(ref currentKBState, ref currentMouseState, ref currentPadState, (Action) (() =>
      {
        GamePadThumbSticks thumbSticks;
        if (Game1.options.gamepadControls)
        {
          bool flag = false;
          if ((double) Math.Abs(currentPadState.ThumbSticks.Right.X) <= 0.0)
          {
            thumbSticks = currentPadState.ThumbSticks;
            if ((double) Math.Abs(thumbSticks.Right.Y) <= 0.0)
              goto label_4;
          }
          double x1 = (double) currentMouseState.X;
          thumbSticks = currentPadState.ThumbSticks;
          double num1 = (double) thumbSticks.Right.X * (double) Game1.thumbstickToMouseModifier;
          int x2 = (int) (x1 + num1);
          double y1 = (double) currentMouseState.Y;
          thumbSticks = currentPadState.ThumbSticks;
          double num2 = (double) thumbSticks.Right.Y * (double) Game1.thumbstickToMouseModifier;
          int y2 = (int) (y1 - num2);
          Game1.setMousePositionRaw(x2, y2);
          flag = true;
label_4:
          if (Game1.IsChatting)
            flag = true;
          if (((Game1.getMouseX() == Game1.getOldMouseX() && Game1.getMouseY() == Game1.getOldMouseY() || Game1.getMouseX() == 0 ? 0 : ((uint) Game1.getMouseY() > 0U ? 1 : 0)) | (flag ? 1 : 0)) != 0)
          {
            if (flag)
            {
              if (Game1.timerUntilMouseFade <= 0)
                Game1.lastMousePositionBeforeFade = new Point(this.localMultiplayerWindow.Width / 2, this.localMultiplayerWindow.Height / 2);
            }
            else
              Game1.lastCursorMotionWasMouse = true;
            if (Game1.timerUntilMouseFade <= 0 && !Game1.lastCursorMotionWasMouse)
              Game1.setMousePositionRaw(Game1.lastMousePositionBeforeFade.X, Game1.lastMousePositionBeforeFade.Y);
            Game1.timerUntilMouseFade = 4000;
          }
        }
        else if (Game1.getMouseX() != Game1.getOldMouseX() || Game1.getMouseY() != Game1.getOldMouseY())
          Game1.lastCursorMotionWasMouse = true;
        bool actionButtonPressed = false;
        bool switchToolButtonPressed = false;
        bool useToolButtonPressed = false;
        bool useToolButtonReleased = false;
        bool addItemToInventoryButtonPressed = false;
        bool cancelButtonPressed = false;
        bool moveUpPressed = false;
        bool moveRightPressed = false;
        bool moveLeftPressed = false;
        bool moveDownPressed = false;
        bool moveUpReleased = false;
        bool moveRightReleased = false;
        bool moveDownReleased = false;
        bool moveLeftReleased = false;
        bool moveUpHeld = false;
        bool moveRightHeld = false;
        bool moveDownHeld = false;
        bool moveLeftHeld = false;
        bool flag1 = false;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.actionButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.actionButton) || currentMouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton == ButtonState.Released)
        {
          actionButtonPressed = true;
          Game1.rightClickPolling = 250;
        }
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.useToolButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.useToolButton) || currentMouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released)
          useToolButtonPressed = true;
        if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.useToolButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton) || currentMouseState.LeftButton == ButtonState.Released && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
          useToolButtonReleased = true;
        if (currentMouseState.ScrollWheelValue != Game1.oldMouseState.ScrollWheelValue)
          switchToolButtonPressed = true;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.cancelButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.cancelButton) || currentMouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton == ButtonState.Released)
          cancelButtonPressed = true;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveUpButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveUpButton))
          moveUpPressed = true;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveRightButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveRightButton))
          moveRightPressed = true;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveDownButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveDownButton))
          moveDownPressed = true;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveLeftButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveLeftButton))
          moveLeftPressed = true;
        if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveUpButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveUpButton))
          moveUpReleased = true;
        if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveRightButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveRightButton))
          moveRightReleased = true;
        if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveDownButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveDownButton))
          moveDownReleased = true;
        if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveLeftButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveLeftButton))
          moveLeftReleased = true;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveUpButton))
          moveUpHeld = true;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveRightButton))
          moveRightHeld = true;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveDownButton))
          moveDownHeld = true;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveLeftButton))
          moveLeftHeld = true;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.useToolButton) || currentMouseState.LeftButton == ButtonState.Pressed)
          flag1 = true;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.actionButton) || currentMouseState.RightButton == ButtonState.Pressed)
        {
          Game1.rightClickPolling -= time.ElapsedGameTime.Milliseconds;
          if (Game1.rightClickPolling <= 0)
          {
            Game1.rightClickPolling = 100;
            actionButtonPressed = true;
          }
        }
        if (Game1.options.gamepadControls)
        {
          if (currentKBState.GetPressedKeys().Length != 0 || currentMouseState.LeftButton == ButtonState.Pressed || currentMouseState.RightButton == ButtonState.Pressed)
            Game1.timerUntilMouseFade = 4000;
          if (currentPadState.IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A))
          {
            actionButtonPressed = true;
            Game1.lastCursorMotionWasMouse = false;
            Game1.rightClickPolling = 250;
          }
          if (currentPadState.IsButtonDown(Buttons.X) && !Game1.oldPadState.IsButtonDown(Buttons.X))
          {
            useToolButtonPressed = true;
            Game1.lastCursorMotionWasMouse = false;
          }
          if (!currentPadState.IsButtonDown(Buttons.X) && Game1.oldPadState.IsButtonDown(Buttons.X))
            useToolButtonReleased = true;
          if (currentPadState.IsButtonDown(Buttons.RightTrigger) && !Game1.oldPadState.IsButtonDown(Buttons.RightTrigger))
          {
            switchToolButtonPressed = true;
            Game1.triggerPolling = 300;
          }
          else if (currentPadState.IsButtonDown(Buttons.LeftTrigger) && !Game1.oldPadState.IsButtonDown(Buttons.LeftTrigger))
          {
            switchToolButtonPressed = true;
            Game1.triggerPolling = 300;
          }
          if (currentPadState.IsButtonDown(Buttons.X))
            flag1 = true;
          if (currentPadState.IsButtonDown(Buttons.A))
          {
            Game1.rightClickPolling -= time.ElapsedGameTime.Milliseconds;
            if (Game1.rightClickPolling <= 0)
            {
              Game1.rightClickPolling = 100;
              actionButtonPressed = true;
            }
          }
          if (currentPadState.IsButtonDown(Buttons.RightTrigger) || currentPadState.IsButtonDown(Buttons.LeftTrigger))
          {
            Game1.triggerPolling -= time.ElapsedGameTime.Milliseconds;
            if (Game1.triggerPolling <= 0)
            {
              Game1.triggerPolling = 100;
              switchToolButtonPressed = true;
            }
          }
          if (currentPadState.IsButtonDown(Buttons.RightShoulder) && !Game1.oldPadState.IsButtonDown(Buttons.RightShoulder))
            Game1.player.shiftToolbar(true);
          if (currentPadState.IsButtonDown(Buttons.LeftShoulder) && !Game1.oldPadState.IsButtonDown(Buttons.LeftShoulder))
            Game1.player.shiftToolbar(false);
          if (currentPadState.IsButtonDown(Buttons.DPadUp) && !Game1.oldPadState.IsButtonDown(Buttons.DPadUp))
            moveUpPressed = true;
          else if (!currentPadState.IsButtonDown(Buttons.DPadUp) && Game1.oldPadState.IsButtonDown(Buttons.DPadUp))
            moveUpReleased = true;
          if (currentPadState.IsButtonDown(Buttons.DPadRight) && !Game1.oldPadState.IsButtonDown(Buttons.DPadRight))
            moveRightPressed = true;
          else if (!currentPadState.IsButtonDown(Buttons.DPadRight) && Game1.oldPadState.IsButtonDown(Buttons.DPadRight))
            moveRightReleased = true;
          if (currentPadState.IsButtonDown(Buttons.DPadDown) && !Game1.oldPadState.IsButtonDown(Buttons.DPadDown))
            moveDownPressed = true;
          else if (!currentPadState.IsButtonDown(Buttons.DPadDown) && Game1.oldPadState.IsButtonDown(Buttons.DPadDown))
            moveDownReleased = true;
          if (currentPadState.IsButtonDown(Buttons.DPadLeft) && !Game1.oldPadState.IsButtonDown(Buttons.DPadLeft))
            moveLeftPressed = true;
          else if (!currentPadState.IsButtonDown(Buttons.DPadLeft) && Game1.oldPadState.IsButtonDown(Buttons.DPadLeft))
            moveLeftReleased = true;
          if (currentPadState.IsButtonDown(Buttons.DPadUp))
            moveUpHeld = true;
          if (currentPadState.IsButtonDown(Buttons.DPadRight))
            moveRightHeld = true;
          if (currentPadState.IsButtonDown(Buttons.DPadDown))
            moveDownHeld = true;
          if (currentPadState.IsButtonDown(Buttons.DPadLeft))
            moveLeftHeld = true;
          thumbSticks = currentPadState.ThumbSticks;
          if ((double) thumbSticks.Left.X < -0.2)
          {
            moveLeftPressed = true;
            moveLeftHeld = true;
          }
          else
          {
            thumbSticks = currentPadState.ThumbSticks;
            if ((double) thumbSticks.Left.X > 0.2)
            {
              moveRightPressed = true;
              moveRightHeld = true;
            }
          }
          thumbSticks = currentPadState.ThumbSticks;
          if ((double) thumbSticks.Left.Y < -0.2)
          {
            moveDownPressed = true;
            moveDownHeld = true;
          }
          else
          {
            thumbSticks = currentPadState.ThumbSticks;
            if ((double) thumbSticks.Left.Y > 0.2)
            {
              moveUpPressed = true;
              moveUpHeld = true;
            }
          }
          thumbSticks = Game1.oldPadState.ThumbSticks;
          if ((double) thumbSticks.Left.X < -0.2 && !moveLeftHeld)
            moveLeftReleased = true;
          thumbSticks = Game1.oldPadState.ThumbSticks;
          if ((double) thumbSticks.Left.X > 0.2 && !moveRightHeld)
            moveRightReleased = true;
          thumbSticks = Game1.oldPadState.ThumbSticks;
          if ((double) thumbSticks.Left.Y < -0.2 && !moveDownHeld)
            moveDownReleased = true;
          thumbSticks = Game1.oldPadState.ThumbSticks;
          if ((double) thumbSticks.Left.Y > 0.2 && !moveUpHeld)
            moveUpReleased = true;
          if ((double) this.controllerSlingshotSafeTime > 0.0)
          {
            if (!currentPadState.IsButtonDown(Buttons.DPadUp) && !currentPadState.IsButtonDown(Buttons.DPadDown) && !currentPadState.IsButtonDown(Buttons.DPadLeft) && !currentPadState.IsButtonDown(Buttons.DPadRight))
            {
              thumbSticks = currentPadState.ThumbSticks;
              if ((double) Math.Abs(thumbSticks.Left.X) < 0.04)
              {
                thumbSticks = currentPadState.ThumbSticks;
                if ((double) Math.Abs(thumbSticks.Left.Y) < 0.04)
                  this.controllerSlingshotSafeTime = 0.0f;
              }
            }
            if ((double) this.controllerSlingshotSafeTime <= 0.0)
            {
              this.controllerSlingshotSafeTime = 0.0f;
            }
            else
            {
              this.controllerSlingshotSafeTime -= (float) time.ElapsedGameTime.TotalSeconds;
              moveUpPressed = false;
              moveDownPressed = false;
              moveLeftPressed = false;
              moveRightPressed = false;
              moveUpHeld = false;
              moveDownHeld = false;
              moveLeftHeld = false;
              moveRightHeld = false;
            }
          }
        }
        else
          this.controllerSlingshotSafeTime = 0.0f;
        Game1.ResetFreeCursorDrag();
        if (flag1)
          Game1.mouseClickPolling += time.ElapsedGameTime.Milliseconds;
        else
          Game1.mouseClickPolling = 0;
        if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.toolbarSwap) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.toolbarSwap))
          Game1.player.shiftToolbar(!currentKBState.IsKeyDown(Keys.LeftControl));
        if (Game1.mouseClickPolling > 250 && (Game1.player.CurrentTool == null || !(Game1.player.CurrentTool is FishingRod) || (int) (NetFieldBase<int, NetInt>) Game1.player.CurrentTool.upgradeLevel <= 0))
        {
          useToolButtonPressed = true;
          Game1.mouseClickPolling = 100;
        }
        Game1.PushUIMode();
        foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
        {
          if ((Game1.displayHUD || onScreenMenu == Game1.chatBox) && Game1.wasMouseVisibleThisFrame && onScreenMenu.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()))
            onScreenMenu.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
        }
        Game1.PopUIMode();
        if (Game1.chatBox != null && Game1.chatBox.chatBox.Selected && Game1.oldMouseState.ScrollWheelValue != currentMouseState.ScrollWheelValue)
          Game1.chatBox.receiveScrollWheelAction(currentMouseState.ScrollWheelValue - Game1.oldMouseState.ScrollWheelValue);
        if (Game1.panMode)
        {
          this.updatePanModeControls(currentMouseState, currentKBState);
        }
        else
        {
          if (Game1.inputSimulator != null)
          {
            if (currentKBState.IsKeyDown(Keys.Escape))
              Game1.inputSimulator = (IInputSimulator) null;
            else
              Game1.inputSimulator.SimulateInput(ref actionButtonPressed, ref switchToolButtonPressed, ref useToolButtonPressed, ref useToolButtonReleased, ref addItemToInventoryButtonPressed, ref cancelButtonPressed, ref moveUpPressed, ref moveRightPressed, ref moveLeftPressed, ref moveDownPressed, ref moveUpReleased, ref moveRightReleased, ref moveLeftReleased, ref moveDownReleased, ref moveUpHeld, ref moveRightHeld, ref moveLeftHeld, ref moveDownHeld);
          }
          if (useToolButtonReleased && Game1.player.CurrentTool != null && Game1.CurrentEvent == null && (double) Game1.pauseTime <= 0.0 && Game1.player.CurrentTool.onRelease(Game1.currentLocation, Game1.getMouseX(), Game1.getMouseY(), Game1.player))
          {
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldKBState = currentKBState;
            Game1.oldPadState = currentPadState;
            Game1.player.usingSlingshot = false;
            Game1.player.canReleaseTool = true;
            Game1.player.UsingTool = false;
            Game1.player.CanMove = true;
          }
          else
          {
            if ((useToolButtonPressed && !Game1.isAnyGamePadButtonBeingPressed() || actionButtonPressed && Game1.isAnyGamePadButtonBeingPressed()) && (double) Game1.pauseTime <= 0.0 && Game1.wasMouseVisibleThisFrame)
            {
              Game1.PushUIMode();
              foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
              {
                if (Game1.displayHUD || onScreenMenu == Game1.chatBox)
                {
                  if ((!Game1.IsChatting || onScreenMenu == Game1.chatBox) && (!(onScreenMenu is LevelUpMenu) || (onScreenMenu as LevelUpMenu).informationUp) && onScreenMenu.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()))
                  {
                    onScreenMenu.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY());
                    Game1.PopUIMode();
                    Game1.oldMouseState = Game1.input.GetMouseState();
                    Game1.oldKBState = currentKBState;
                    Game1.oldPadState = currentPadState;
                    return;
                  }
                  if (onScreenMenu == Game1.chatBox && Game1.options.gamepadControls && Game1.IsChatting)
                  {
                    Game1.oldMouseState = Game1.input.GetMouseState();
                    Game1.oldKBState = currentKBState;
                    Game1.oldPadState = currentPadState;
                    Game1.PopUIMode();
                    return;
                  }
                  onScreenMenu.clickAway();
                }
              }
              Game1.PopUIMode();
            }
            if (Game1.IsChatting || Game1.player.freezePause > 0)
            {
              if (Game1.IsChatting)
              {
                foreach (Buttons pressedButton in Utility.getPressedButtons(currentPadState, Game1.oldPadState))
                  Game1.chatBox.receiveGamePadButton(pressedButton);
              }
              Game1.oldMouseState = Game1.input.GetMouseState();
              Game1.oldKBState = currentKBState;
              Game1.oldPadState = currentPadState;
            }
            else
            {
              if (Game1.paused || Game1.HostPaused)
              {
                if (Game1.HostPaused && Game1.IsMasterGame && (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.menuButton) || currentPadState.IsButtonDown(Buttons.B) || currentPadState.IsButtonDown(Buttons.Back)))
                {
                  Game1.netWorldState.Value.IsPaused = false;
                  if (Game1.chatBox != null)
                    Game1.chatBox.globalInfoMessage("Resumed");
                }
                else
                {
                  Game1.oldMouseState = Game1.input.GetMouseState();
                  return;
                }
              }
              if (Game1.eventUp)
              {
                if (Game1.currentLocation.currentEvent == null && Game1.locationRequest == null)
                  Game1.eventUp = false;
                else if (actionButtonPressed | useToolButtonPressed)
                  Game1.CurrentEvent?.receiveMouseClick(Game1.getMouseX(), Game1.getMouseY());
              }
              bool flag2 = Game1.eventUp || Game1.farmEvent != null;
              if (actionButtonPressed || Game1.dialogueUp & useToolButtonPressed)
              {
                Game1.PushUIMode();
                foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
                {
                  if (Game1.wasMouseVisibleThisFrame && (Game1.displayHUD || onScreenMenu == Game1.chatBox) && onScreenMenu.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()) && (!(onScreenMenu is LevelUpMenu) || (onScreenMenu as LevelUpMenu).informationUp))
                  {
                    onScreenMenu.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
                    Game1.oldMouseState = Game1.input.GetMouseState();
                    if (!Game1.isAnyGamePadButtonBeingPressed())
                    {
                      Game1.PopUIMode();
                      Game1.oldKBState = currentKBState;
                      Game1.oldPadState = currentPadState;
                      return;
                    }
                  }
                }
                Game1.PopUIMode();
                if (!Game1.pressActionButton(currentKBState, currentMouseState, currentPadState))
                {
                  Game1.oldKBState = currentKBState;
                  Game1.oldMouseState = Game1.input.GetMouseState();
                  Game1.oldPadState = currentPadState;
                  return;
                }
              }
              if (useToolButtonPressed && (!Game1.player.UsingTool || Game1.player.CurrentTool != null && Game1.player.CurrentTool is MeleeWeapon) && !Game1.player.isEating && !Game1.pickingTool && !Game1.dialogueUp && !Game1.menuUp && Game1.farmEvent == null && (Game1.player.CanMove || Game1.player.CurrentTool != null && (Game1.player.CurrentTool.Name.Equals("Fishing Rod") || Game1.player.CurrentTool is MeleeWeapon)))
              {
                if (Game1.player.CurrentTool != null && (!(Game1.player.CurrentTool is MeleeWeapon) || Game1.didPlayerJustLeftClick(true)))
                  Game1.player.FireTool();
                if (!Game1.pressUseToolButton() && Game1.player.canReleaseTool && Game1.player.UsingTool)
                {
                  Tool currentTool = Game1.player.CurrentTool;
                }
                if (Game1.player.UsingTool)
                {
                  Game1.oldMouseState = Game1.input.GetMouseState();
                  Game1.oldKBState = currentKBState;
                  Game1.oldPadState = currentPadState;
                  return;
                }
              }
              if (useToolButtonReleased && this._didInitiateItemStow)
                this._didInitiateItemStow = false;
              if (useToolButtonReleased && Game1.player.canReleaseTool && Game1.player.UsingTool && Game1.player.CurrentTool != null)
                Game1.player.EndUsingTool();
              if (switchToolButtonPressed && !Game1.player.UsingTool && !Game1.dialogueUp && (Game1.pickingTool || Game1.player.CanMove) && !Game1.player.areAllItemsNull() && !flag2)
                Game1.pressSwitchToolButton();
              if (cancelButtonPressed)
              {
                if (Game1.numberOfSelectedItems != -1)
                {
                  Game1.numberOfSelectedItems = -1;
                  Game1.dialogueUp = false;
                  Game1.player.CanMove = true;
                }
                else if (Game1.nameSelectUp && NameSelect.cancel())
                {
                  Game1.nameSelectUp = false;
                  Game1.playSound("bigDeSelect");
                }
              }
              if (Game1.player.CurrentTool != null & flag1 && Game1.player.canReleaseTool && !flag2 && !Game1.dialogueUp && !Game1.menuUp && (double) Game1.player.Stamina >= 1.0 && !(Game1.player.CurrentTool is FishingRod))
              {
                int num3 = Game1.player.CurrentTool.hasEnchantmentOfType<ReachingToolEnchantment>() ? 1 : 0;
                if (Game1.player.toolHold <= 0 && (int) (NetFieldBase<int, NetInt>) Game1.player.CurrentTool.upgradeLevel + num3 > Game1.player.toolPower)
                {
                  float num4 = 1f;
                  if (Game1.player.CurrentTool != null)
                    num4 = Game1.player.CurrentTool.AnimationSpeedModifier;
                  Game1.player.toolHold = (int) (600.0 * (double) num4);
                }
                else if ((int) (NetFieldBase<int, NetInt>) Game1.player.CurrentTool.upgradeLevel + num3 > Game1.player.toolPower)
                {
                  Game1.player.toolHold -= time.ElapsedGameTime.Milliseconds;
                  if (Game1.player.toolHold <= 0)
                    Game1.player.toolPowerIncrease();
                }
              }
              if ((double) Game1.upPolling >= 650.0)
                Game1.upPolling -= 100f;
              else if ((double) Game1.downPolling >= 650.0)
                Game1.downPolling -= 100f;
              else if ((double) Game1.rightPolling >= 650.0)
                Game1.rightPolling -= 100f;
              else if ((double) Game1.leftPolling >= 650.0)
                Game1.leftPolling -= 100f;
              else if (!Game1.nameSelectUp && (double) Game1.pauseTime <= 0.0 && Game1.locationRequest == null && !Game1.player.UsingTool && (!flag2 || Game1.CurrentEvent != null && Game1.CurrentEvent.playerControlSequence))
              {
                if (Game1.player.movementDirections.Count < 2)
                {
                  int count = Game1.player.movementDirections.Count;
                  if (moveUpHeld)
                    Game1.player.setMoving((byte) 1);
                  if (moveRightHeld)
                    Game1.player.setMoving((byte) 2);
                  if (moveDownHeld)
                    Game1.player.setMoving((byte) 4);
                  if (moveLeftHeld)
                    Game1.player.setMoving((byte) 8);
                }
                if (moveUpReleased || Game1.player.movementDirections.Contains(0) && !moveUpHeld)
                {
                  Game1.player.setMoving((byte) 33);
                  if (Game1.player.movementDirections.Count == 0)
                    Game1.player.setMoving((byte) 64);
                }
                if (moveRightReleased || Game1.player.movementDirections.Contains(1) && !moveRightHeld)
                {
                  Game1.player.setMoving((byte) 34);
                  if (Game1.player.movementDirections.Count == 0)
                    Game1.player.setMoving((byte) 64);
                }
                if (moveDownReleased || Game1.player.movementDirections.Contains(2) && !moveDownHeld)
                {
                  Game1.player.setMoving((byte) 36);
                  if (Game1.player.movementDirections.Count == 0)
                    Game1.player.setMoving((byte) 64);
                }
                if (moveLeftReleased || Game1.player.movementDirections.Contains(3) && !moveLeftHeld)
                {
                  Game1.player.setMoving((byte) 40);
                  if (Game1.player.movementDirections.Count == 0)
                    Game1.player.setMoving((byte) 64);
                }
                if (!moveUpHeld && !moveRightHeld && !moveDownHeld && !moveLeftHeld && !Game1.player.UsingTool || Game1.activeClickableMenu != null)
                  Game1.player.Halt();
              }
              else if (Game1.isQuestion)
              {
                if (moveUpPressed)
                {
                  Game1.currentQuestionChoice = Math.Max(Game1.currentQuestionChoice - 1, 0);
                  Game1.playSound("toolSwap");
                }
                else if (moveDownPressed)
                {
                  Game1.currentQuestionChoice = Math.Min(Game1.currentQuestionChoice + 1, Game1.questionChoices.Count - 1);
                  Game1.playSound("toolSwap");
                }
              }
              else if (Game1.numberOfSelectedItems != -1 && !Game1.dialogueTyping)
              {
                int val2 = 99;
                if (Game1.selectedItemsType.Equals("Animal Food"))
                  val2 = 999 - Game1.player.Feed;
                else if (Game1.selectedItemsType.Equals("calicoJackBet"))
                  val2 = Math.Min(Game1.player.clubCoins, 999);
                else if (Game1.selectedItemsType.Equals("flutePitch"))
                  val2 = 26;
                else if (Game1.selectedItemsType.Equals("drumTone"))
                  val2 = 6;
                else if (Game1.selectedItemsType.Equals("jukebox"))
                  val2 = Game1.player.songsHeard.Count - 1;
                else if (Game1.selectedItemsType.Equals("Fuel"))
                  val2 = 100 - ((Lantern) Game1.player.getToolFromName("Lantern")).fuelLeft;
                if (moveRightPressed)
                {
                  Game1.numberOfSelectedItems = Math.Min(Game1.numberOfSelectedItems + 1, val2);
                  Game1.playItemNumberSelectSound();
                }
                else if (moveLeftPressed)
                {
                  Game1.numberOfSelectedItems = Math.Max(Game1.numberOfSelectedItems - 1, 0);
                  Game1.playItemNumberSelectSound();
                }
                else if (moveUpPressed)
                {
                  Game1.numberOfSelectedItems = Math.Min(Game1.numberOfSelectedItems + 10, val2);
                  Game1.playItemNumberSelectSound();
                }
                else if (moveDownPressed)
                {
                  Game1.numberOfSelectedItems = Math.Max(Game1.numberOfSelectedItems - 10, 0);
                  Game1.playItemNumberSelectSound();
                }
              }
              if (moveUpHeld && !Game1.player.CanMove)
                Game1.upPolling += (float) time.ElapsedGameTime.Milliseconds;
              else if (moveDownHeld && !Game1.player.CanMove)
                Game1.downPolling += (float) time.ElapsedGameTime.Milliseconds;
              else if (moveRightHeld && !Game1.player.CanMove)
                Game1.rightPolling += (float) time.ElapsedGameTime.Milliseconds;
              else if (moveLeftHeld && !Game1.player.CanMove)
                Game1.leftPolling += (float) time.ElapsedGameTime.Milliseconds;
              else if (moveUpReleased)
                Game1.upPolling = 0.0f;
              else if (moveDownReleased)
                Game1.downPolling = 0.0f;
              else if (moveRightReleased)
                Game1.rightPolling = 0.0f;
              else if (moveLeftReleased)
                Game1.leftPolling = 0.0f;
              if (Game1.debugMode)
              {
                if (currentKBState.IsKeyDown(Keys.Q))
                  Game1.oldKBState.IsKeyDown(Keys.Q);
                if (currentKBState.IsKeyDown(Keys.P) && !Game1.oldKBState.IsKeyDown(Keys.P))
                  Game1.NewDay(0.0f);
                if (currentKBState.IsKeyDown(Keys.M) && !Game1.oldKBState.IsKeyDown(Keys.M))
                {
                  Game1.dayOfMonth = 28;
                  Game1.NewDay(0.0f);
                }
                if (currentKBState.IsKeyDown(Keys.T) && !Game1.oldKBState.IsKeyDown(Keys.T))
                  Game1.addHour();
                if (currentKBState.IsKeyDown(Keys.Y) && !Game1.oldKBState.IsKeyDown(Keys.Y))
                  Game1.addMinute();
                if (currentKBState.IsKeyDown(Keys.D1) && !Game1.oldKBState.IsKeyDown(Keys.D1))
                  Game1.warpFarmer("Mountain", 15, 35, false);
                if (currentKBState.IsKeyDown(Keys.D2) && !Game1.oldKBState.IsKeyDown(Keys.D2))
                  Game1.warpFarmer("Town", 35, 35, false);
                if (currentKBState.IsKeyDown(Keys.D3) && !Game1.oldKBState.IsKeyDown(Keys.D3))
                  Game1.warpFarmer("Farm", 64, 15, false);
                if (currentKBState.IsKeyDown(Keys.D4) && !Game1.oldKBState.IsKeyDown(Keys.D4))
                  Game1.warpFarmer("Forest", 34, 13, false);
                if (currentKBState.IsKeyDown(Keys.D5) && !Game1.oldKBState.IsKeyDown(Keys.D4))
                  Game1.warpFarmer("Beach", 34, 10, false);
                if (currentKBState.IsKeyDown(Keys.D6) && !Game1.oldKBState.IsKeyDown(Keys.D6))
                  Game1.warpFarmer("Mine", 18, 12, false);
                if (currentKBState.IsKeyDown(Keys.D7) && !Game1.oldKBState.IsKeyDown(Keys.D7))
                  Game1.warpFarmer("SandyHouse", 16, 3, false);
                if (currentKBState.IsKeyDown(Keys.K) && !Game1.oldKBState.IsKeyDown(Keys.K))
                  Game1.enterMine(Game1.mine.mineLevel + 1);
                if (currentKBState.IsKeyDown(Keys.H) && !Game1.oldKBState.IsKeyDown(Keys.H))
                  Game1.player.changeHat(Game1.random.Next(FarmerRenderer.hatsTexture.Height / 80 * 12));
                if (currentKBState.IsKeyDown(Keys.I) && !Game1.oldKBState.IsKeyDown(Keys.I))
                  Game1.player.changeHairStyle(Game1.random.Next(FarmerRenderer.hairStylesTexture.Height / 96 * 8));
                if (currentKBState.IsKeyDown(Keys.J) && !Game1.oldKBState.IsKeyDown(Keys.J))
                {
                  Game1.player.changeShirt(Game1.random.Next(40));
                  Game1.player.changePants(new Microsoft.Xna.Framework.Color(Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue)));
                }
                if (currentKBState.IsKeyDown(Keys.L) && !Game1.oldKBState.IsKeyDown(Keys.L))
                {
                  Game1.player.changeShirt(Game1.random.Next(40));
                  Game1.player.changePants(new Microsoft.Xna.Framework.Color(Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue)));
                  Game1.player.changeHairStyle(Game1.random.Next(FarmerRenderer.hairStylesTexture.Height / 96 * 8));
                  if (Game1.random.NextDouble() < 0.5)
                    Game1.player.changeHat(Game1.random.Next(-1, FarmerRenderer.hatsTexture.Height / 80 * 12));
                  else
                    Game1.player.changeHat(-1);
                  Game1.player.changeHairColor(new Microsoft.Xna.Framework.Color(Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue)));
                  Game1.player.changeSkinColor(Game1.random.Next(16));
                }
                if (currentKBState.IsKeyDown(Keys.U) && !Game1.oldKBState.IsKeyDown(Keys.U))
                {
                  (Game1.getLocationFromName("FarmHouse") as FarmHouse).setWallpaper(Game1.random.Next(112), persist: true);
                  (Game1.getLocationFromName("FarmHouse") as FarmHouse).setFloor(Game1.random.Next(40), persist: true);
                }
                if (currentKBState.IsKeyDown(Keys.F2))
                  Game1.oldKBState.IsKeyDown(Keys.F2);
                if (currentKBState.IsKeyDown(Keys.F5) && !Game1.oldKBState.IsKeyDown(Keys.F5))
                  Game1.displayFarmer = !Game1.displayFarmer;
                if (currentKBState.IsKeyDown(Keys.F6))
                  Game1.oldKBState.IsKeyDown(Keys.F6);
                if (currentKBState.IsKeyDown(Keys.F7) && !Game1.oldKBState.IsKeyDown(Keys.F7))
                  Game1.drawGrid = !Game1.drawGrid;
                if (currentKBState.IsKeyDown(Keys.B) && !Game1.oldKBState.IsKeyDown(Keys.B))
                  Game1.player.shiftToolbar(false);
                if (currentKBState.IsKeyDown(Keys.N) && !Game1.oldKBState.IsKeyDown(Keys.N))
                  Game1.player.shiftToolbar(true);
                if (currentKBState.IsKeyDown(Keys.F10) && !Game1.oldKBState.IsKeyDown(Keys.F10) && Game1.server == null)
                  Game1.multiplayer.StartServer();
              }
              else if (!Game1.player.UsingTool)
              {
                if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot1) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot1))
                  Game1.player.CurrentToolIndex = 0;
                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot2) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot2))
                  Game1.player.CurrentToolIndex = 1;
                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot3) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot3))
                  Game1.player.CurrentToolIndex = 2;
                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot4) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot4))
                  Game1.player.CurrentToolIndex = 3;
                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot5) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot5))
                  Game1.player.CurrentToolIndex = 4;
                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot6) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot6))
                  Game1.player.CurrentToolIndex = 5;
                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot7) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot7))
                  Game1.player.CurrentToolIndex = 6;
                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot8) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot8))
                  Game1.player.CurrentToolIndex = 7;
                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot9) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot9))
                  Game1.player.CurrentToolIndex = 8;
                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot10) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot10))
                  Game1.player.CurrentToolIndex = 9;
                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot11) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot11))
                  Game1.player.CurrentToolIndex = 10;
                else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot12) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot12))
                  Game1.player.CurrentToolIndex = 11;
              }
              if ((Game1.options.gamepadControls && Game1.rightStickHoldTime >= Game1.emoteMenuShowTime && Game1.activeClickableMenu == null || Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.emoteButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.emoteButton)) && !Game1.debugMode && Game1.player.CanEmote())
              {
                if (Game1.player.CanMove)
                  Game1.player.Halt();
                Game1.emoteMenu = new EmoteMenu();
                Game1.emoteMenu.gamepadMode = Game1.options.gamepadControls && Game1.rightStickHoldTime >= Game1.emoteMenuShowTime;
                Game1.timerUntilMouseFade = 0;
              }
              if (!Program.releaseBuild)
              {
                if (Game1.IsPressEvent(ref currentKBState, Keys.F3) || Game1.IsPressEvent(ref currentPadState, Buttons.LeftStick))
                {
                  Game1.debugMode = !Game1.debugMode;
                  if (Game1.gameMode == (byte) 11)
                    Game1.gameMode = (byte) 3;
                }
                if (Game1.IsPressEvent(ref currentKBState, Keys.F8))
                  this.requestDebugInput();
              }
              if (currentKBState.IsKeyDown(Keys.F4) && !Game1.oldKBState.IsKeyDown(Keys.F4))
              {
                Game1.displayHUD = !Game1.displayHUD;
                Game1.playSound("smallSelect");
                if (!Game1.displayHUD)
                  Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3666"));
              }
              bool flag3 = Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.menuButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.menuButton);
              bool flag4 = Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.journalButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.journalButton);
              bool flag5 = Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.mapButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.mapButton);
              if (Game1.options.gamepadControls && !flag3)
                flag3 = currentPadState.IsButtonDown(Buttons.Start) && !Game1.oldPadState.IsButtonDown(Buttons.Start) || currentPadState.IsButtonDown(Buttons.B) && !Game1.oldPadState.IsButtonDown(Buttons.B);
              if (Game1.options.gamepadControls && !flag4)
                flag4 = currentPadState.IsButtonDown(Buttons.Back) && !Game1.oldPadState.IsButtonDown(Buttons.Back);
              if (Game1.options.gamepadControls && !flag5)
                flag5 = currentPadState.IsButtonDown(Buttons.Y) && !Game1.oldPadState.IsButtonDown(Buttons.Y);
              if (flag3 && Game1.CanShowPauseMenu())
              {
                if (Game1.activeClickableMenu == null)
                {
                  Game1.PushUIMode();
                  Game1.activeClickableMenu = (IClickableMenu) new GameMenu();
                  Game1.PopUIMode();
                }
                else if (Game1.activeClickableMenu.readyToClose())
                  Game1.exitActiveMenu();
              }
              if (((Game1.dayOfMonth <= 0 ? 0 : (Game1.player.CanMove ? 1 : 0)) & (flag4 ? 1 : 0)) != 0 && !Game1.dialogueUp && !flag2)
              {
                if (Game1.activeClickableMenu == null)
                  Game1.activeClickableMenu = (IClickableMenu) new QuestLog();
              }
              else if (((!flag2 ? 0 : (Game1.CurrentEvent != null ? 1 : 0)) & (flag4 ? 1 : 0)) != 0 && !Game1.CurrentEvent.skipped && Game1.CurrentEvent.skippable)
              {
                Game1.CurrentEvent.skipped = true;
                Game1.CurrentEvent.skipEvent();
                Game1.freezeControls = false;
              }
              if (((!Game1.options.gamepadControls || Game1.dayOfMonth <= 0 || !Game1.player.CanMove ? 0 : (Game1.isAnyGamePadButtonBeingPressed() ? 1 : 0)) & (flag5 ? 1 : 0)) != 0 && !Game1.dialogueUp && !flag2)
              {
                if (Game1.activeClickableMenu == null)
                {
                  Game1.PushUIMode();
                  Game1.activeClickableMenu = (IClickableMenu) new GameMenu(4);
                  Game1.PopUIMode();
                }
              }
              else if (((Game1.dayOfMonth <= 0 ? 0 : (Game1.player.CanMove ? 1 : 0)) & (flag5 ? 1 : 0)) != 0 && !Game1.dialogueUp && !flag2 && Game1.activeClickableMenu == null)
              {
                Game1.PushUIMode();
                Game1.activeClickableMenu = (IClickableMenu) new GameMenu(3);
                Game1.PopUIMode();
              }
              Game1.checkForRunButton(currentKBState);
              Game1.oldKBState = currentKBState;
              Game1.oldMouseState = Game1.input.GetMouseState();
              Game1.oldPadState = currentPadState;
            }
          }
        }
      }));
    }

    public static bool CanShowPauseMenu() => Game1.dayOfMonth > 0 && Game1.player.CanMove && !Game1.dialogueUp && (!Game1.eventUp || Game1.isFestival() && Game1.CurrentEvent.festivalTimer <= 0) && Game1.currentMinigame == null && Game1.farmEvent == null;

    private static void addHour()
    {
      Game1.timeOfDay += 100;
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        for (int index = 0; index < location.getCharacters().Count; ++index)
        {
          location.getCharacters()[index].checkSchedule(Game1.timeOfDay);
          location.getCharacters()[index].checkSchedule(Game1.timeOfDay - 50);
          location.getCharacters()[index].checkSchedule(Game1.timeOfDay - 60);
          location.getCharacters()[index].checkSchedule(Game1.timeOfDay - 70);
          location.getCharacters()[index].checkSchedule(Game1.timeOfDay - 80);
          location.getCharacters()[index].checkSchedule(Game1.timeOfDay - 90);
        }
      }
      switch (Game1.timeOfDay)
      {
        case 1900:
          Game1.globalOutdoorLighting = 0.5f;
          Game1.currentLocation.switchOutNightTiles();
          break;
        case 2000:
          Game1.globalOutdoorLighting = 0.7f;
          if (Game1.IsRainingHere())
            break;
          Game1.changeMusicTrack("none");
          break;
        case 2100:
          Game1.globalOutdoorLighting = 0.9f;
          break;
        case 2200:
          Game1.globalOutdoorLighting = 1f;
          break;
      }
    }

    private static void addMinute()
    {
      if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift))
        Game1.timeOfDay -= 10;
      else
        Game1.timeOfDay += 10;
      if (Game1.timeOfDay % 100 == 60)
        Game1.timeOfDay += 40;
      if (Game1.timeOfDay % 100 == 90)
        Game1.timeOfDay -= 40;
      Game1.currentLocation.performTenMinuteUpdate(Game1.timeOfDay);
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        for (int index = 0; index < location.getCharacters().Count; ++index)
          location.getCharacters()[index].checkSchedule(Game1.timeOfDay);
      }
      if (Game1.isLightning && Game1.IsMasterGame)
        Utility.performLightningUpdate(Game1.timeOfDay);
      switch (Game1.timeOfDay)
      {
        case 1750:
          Game1.globalOutdoorLighting = 0.0f;
          Game1.outdoorLight = Microsoft.Xna.Framework.Color.White;
          break;
        case 1900:
          Game1.globalOutdoorLighting = 0.5f;
          Game1.currentLocation.switchOutNightTiles();
          break;
        case 2000:
          Game1.globalOutdoorLighting = 0.7f;
          if (Game1.IsRainingHere())
            break;
          Game1.changeMusicTrack("none");
          break;
        case 2100:
          Game1.globalOutdoorLighting = 0.9f;
          break;
        case 2200:
          Game1.globalOutdoorLighting = 1f;
          break;
      }
    }

    public static void checkForRunButton(KeyboardState kbState, bool ignoreKeyPressQualifier = false)
    {
      bool running = Game1.player.running;
      bool flag1 = Game1.isOneOfTheseKeysDown(kbState, Game1.options.runButton) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.runButton) | ignoreKeyPressQualifier;
      bool flag2 = !Game1.isOneOfTheseKeysDown(kbState, Game1.options.runButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.runButton) | ignoreKeyPressQualifier;
      if (Game1.options.gamepadControls)
      {
        if (!Game1.options.autoRun && (double) Math.Abs(Vector2.Distance(Game1.input.GetGamePadState().ThumbSticks.Left, Vector2.Zero)) > 0.899999976158142)
          flag1 = true;
        else if ((double) Math.Abs(Vector2.Distance(Game1.oldPadState.ThumbSticks.Left, Vector2.Zero)) > 0.899999976158142 && (double) Math.Abs(Vector2.Distance(Game1.input.GetGamePadState().ThumbSticks.Left, Vector2.Zero)) <= 0.899999976158142)
          flag2 = true;
      }
      if (flag1 && !Game1.player.canOnlyWalk)
      {
        Game1.player.setRunning(!Game1.options.autoRun);
        Game1.player.setMoving(Game1.player.running ? (byte) 16 : (byte) 48);
      }
      else if (flag2 && !Game1.player.canOnlyWalk)
      {
        Game1.player.setRunning(Game1.options.autoRun);
        Game1.player.setMoving(Game1.player.running ? (byte) 16 : (byte) 48);
      }
      if (Game1.player.running == running || Game1.player.UsingTool)
        return;
      Game1.player.Halt();
    }

    public static void drawTitleScreenBackground(
      GameTime gameTime,
      string dayNight,
      int weatherDebrisOffsetDay)
    {
    }

    public static Vector2 getMostRecentViewportMotion() => new Vector2((float) Game1.viewport.X - Game1.previousViewportPosition.X, (float) Game1.viewport.Y - Game1.previousViewportPosition.Y);

    public RenderTarget2D screen
    {
      get => this._screen;
      set
      {
        if (this._screen != null)
        {
          this._screen.Dispose();
          this._screen = (RenderTarget2D) null;
        }
        this._screen = value;
      }
    }

    public RenderTarget2D uiScreen
    {
      get => this._uiScreen;
      set
      {
        if (this._uiScreen != null)
        {
          this._uiScreen.Dispose();
          this._uiScreen = (RenderTarget2D) null;
        }
        this._uiScreen = value;
      }
    }

    protected virtual void drawOverlays(SpriteBatch spriteBatch)
    {
      if (this.takingMapScreenshot)
        return;
      spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if (Game1.overlayMenu != null)
        Game1.overlayMenu.draw(spriteBatch);
      if (Game1.chatBox != null)
      {
        Game1.chatBox.update(Game1.currentGameTime);
        Game1.chatBox.draw(spriteBatch);
      }
      if (Game1.textEntry != null)
        Game1.textEntry.draw(spriteBatch);
      if ((Game1.displayHUD || Game1.eventUp || Game1.currentLocation is Summit) && Game1.currentBillboard == 0 && Game1.gameMode == (byte) 3 && !Game1.freezeControls && !Game1.panMode)
        this.drawMouseCursor();
      spriteBatch.End();
    }

    public static void setBGColor(byte r, byte g, byte b)
    {
      Game1.bgColor.R = r;
      Game1.bgColor.G = g;
      Game1.bgColor.B = b;
    }

    public void Instance_Draw(GameTime gameTime) => this.Draw(gameTime);

    /// <summary>This is called when the game should draw itself.</summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      this.isDrawing = true;
      RenderTarget2D renderTarget2D = (RenderTarget2D) null;
      if (this.ShouldDrawOnBuffer())
        renderTarget2D = this.screen;
      if (this.uiScreen != null)
      {
        Game1.SetRenderTarget(this.uiScreen);
        this.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);
        Game1.SetRenderTarget(renderTarget2D);
      }
      GameTime time = gameTime;
      DebugTools.BeforeGameDraw(this, ref time);
      this._draw(time, renderTarget2D);
      Game1.isRenderingScreenBuffer = true;
      this.renderScreenBuffer(renderTarget2D);
      Game1.isRenderingScreenBuffer = false;
      if (Game1.uiModeCount != 0)
      {
        Console.WriteLine("WARNING: Mismatched UI Mode Push/Pop counts. Correcting.");
        while (Game1.uiModeCount < 0)
          Game1.PushUIMode();
        while (Game1.uiModeCount > 0)
          Game1.PopUIMode();
      }
      base.Draw(gameTime);
      this.isDrawing = false;
    }

    public virtual bool ShouldDrawOnBuffer() => LocalMultiplayer.IsLocalMultiplayer() || (double) Game1.options.zoomLevel != 1.0;

    public static bool ShouldShowOnscreenUsernames() => false;

    public virtual bool checkCharacterTilesForShadowDrawFlag(Character character)
    {
      if (character is Farmer && (character as Farmer).onBridge.Value)
        return true;
      Microsoft.Xna.Framework.Rectangle boundingBox = character.GetBoundingBox();
      boundingBox.Height += 8;
      int num1 = boundingBox.Right / 64;
      int num2 = boundingBox.Bottom / 64;
      int num3 = boundingBox.Left / 64;
      int num4 = boundingBox.Top / 64;
      for (int x = num3; x <= num1; ++x)
      {
        for (int y = num4; y <= num2; ++y)
        {
          if (Game1.currentLocation.shouldShadowBeDrawnAboveBuildingsLayer(new Vector2((float) x, (float) y)))
            return true;
        }
      }
      return false;
    }

    protected virtual void _draw(GameTime gameTime, RenderTarget2D target_screen)
    {
      Game1.showingHealthBar = false;
      if (Game1._newDayTask != null || this.isLocalMultiplayerNewDayActive)
      {
        this.GraphicsDevice.Clear(Game1.bgColor);
      }
      else
      {
        if (target_screen != null)
          Game1.SetRenderTarget(target_screen);
        if (this.IsSaving)
        {
          this.GraphicsDevice.Clear(Game1.bgColor);
          Game1.PushUIMode();
          IClickableMenu activeClickableMenu = Game1.activeClickableMenu;
          if (activeClickableMenu != null)
          {
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            activeClickableMenu.draw(Game1.spriteBatch);
            Game1.spriteBatch.End();
          }
          if (Game1.overlayMenu != null)
          {
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            Game1.overlayMenu.draw(Game1.spriteBatch);
            Game1.spriteBatch.End();
          }
          Game1.PopUIMode();
        }
        else
        {
          this.GraphicsDevice.Clear(Game1.bgColor);
          if (Game1.activeClickableMenu != null && Game1.options.showMenuBackground && Game1.activeClickableMenu.showWithoutTransparencyIfOptionIsSet() && !this.takingMapScreenshot)
          {
            Game1.PushUIMode();
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            Game1.activeClickableMenu.drawBackground(Game1.spriteBatch);
            for (IClickableMenu iclickableMenu = Game1.activeClickableMenu; iclickableMenu != null; iclickableMenu = iclickableMenu.GetChildMenu())
              iclickableMenu.draw(Game1.spriteBatch);
            if (Game1.specialCurrencyDisplay != null)
              Game1.specialCurrencyDisplay.Draw(Game1.spriteBatch);
            Game1.spriteBatch.End();
            this.drawOverlays(Game1.spriteBatch);
            Game1.PopUIMode();
          }
          else if (Game1.gameMode == (byte) 11)
          {
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3685"), new Vector2(16f, 16f), Microsoft.Xna.Framework.Color.HotPink);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3686"), new Vector2(16f, 32f), new Microsoft.Xna.Framework.Color(0, (int) byte.MaxValue, 0));
            Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.parseText(Game1.errorMessage, Game1.dialogueFont, Game1.graphics.GraphicsDevice.Viewport.Width), new Vector2(16f, 48f), Microsoft.Xna.Framework.Color.White);
            Game1.spriteBatch.End();
          }
          else if (Game1.currentMinigame != null)
          {
            Game1.currentMinigame.draw(Game1.spriteBatch);
            if (Game1.globalFade && !Game1.menuUp && (!Game1.nameSelectUp || Game1.messagePause))
            {
              Game1.PushUIMode();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              Game1.spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Microsoft.Xna.Framework.Color.Black * (Game1.gameMode == (byte) 0 ? 1f - Game1.fadeToBlackAlpha : Game1.fadeToBlackAlpha));
              Game1.spriteBatch.End();
              Game1.PopUIMode();
            }
            Game1.PushUIMode();
            this.drawOverlays(Game1.spriteBatch);
            Game1.PopUIMode();
            Game1.SetRenderTarget(target_screen);
          }
          else if (Game1.showingEndOfNightStuff)
          {
            Game1.PushUIMode();
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            if (Game1.activeClickableMenu != null)
            {
              for (IClickableMenu iclickableMenu = Game1.activeClickableMenu; iclickableMenu != null; iclickableMenu = iclickableMenu.GetChildMenu())
                iclickableMenu.draw(Game1.spriteBatch);
            }
            Game1.spriteBatch.End();
            this.drawOverlays(Game1.spriteBatch);
            Game1.PopUIMode();
          }
          else if (Game1.gameMode == (byte) 6 || Game1.gameMode == (byte) 3 && Game1.currentLocation == null)
          {
            Game1.PushUIMode();
            this.GraphicsDevice.Clear(Game1.bgColor);
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            string str1 = "";
            for (int index = 0; (double) index < gameTime.TotalGameTime.TotalMilliseconds % 999.0 / 333.0; ++index)
              str1 += ".";
            string str2 = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3688");
            string s = str2 + str1;
            string str3 = str2 + "... ";
            int widthOfString = SpriteText.getWidthOfString(str3);
            int height = 64;
            int x = 64;
            int y = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - height;
            SpriteText.drawString(Game1.spriteBatch, s, x, y, width: widthOfString, height: height, drawBGScroll: 0, placeHolderScrollWidthText: str3);
            Game1.spriteBatch.End();
            this.drawOverlays(Game1.spriteBatch);
            Game1.PopUIMode();
          }
          else
          {
            Viewport viewport1;
            if (Game1.gameMode == (byte) 0)
            {
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            }
            else
            {
              if (Game1.gameMode == (byte) 3 && Game1.dayOfMonth == 0 && Game1.newDay)
              {
                base.Draw(gameTime);
                return;
              }
              if (Game1.drawLighting)
              {
                Game1.SetRenderTarget(Game1.lightmap);
                this.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.White * 0.0f);
                Matrix matrix = Matrix.Identity;
                if (this.useUnscaledLighting)
                  matrix = Matrix.CreateScale(Game1.options.zoomLevel);
                Game1.spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp, transformMatrix: new Matrix?(matrix));
                Microsoft.Xna.Framework.Color color = !Game1.currentLocation.Name.StartsWith("UndergroundMine") || !(Game1.currentLocation is MineShaft) ? (Game1.ambientLight.Equals(Microsoft.Xna.Framework.Color.White) || Game1.IsRainingHere() && (bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.isOutdoors ? Game1.outdoorLight : Game1.ambientLight) : (Game1.currentLocation as MineShaft).getLightingColor(gameTime);
                float num = 1f;
                if (Game1.player.hasBuff(26))
                {
                  if (color == Microsoft.Xna.Framework.Color.White)
                  {
                    color = new Microsoft.Xna.Framework.Color(0.75f, 0.75f, 0.75f);
                  }
                  else
                  {
                    color.R = (byte) Utility.Lerp((float) color.R, (float) byte.MaxValue, 0.5f);
                    color.G = (byte) Utility.Lerp((float) color.G, (float) byte.MaxValue, 0.5f);
                    color.B = (byte) Utility.Lerp((float) color.B, (float) byte.MaxValue, 0.5f);
                  }
                  num = 0.33f;
                }
                Game1.spriteBatch.Draw(Game1.staminaRect, Game1.lightmap.Bounds, color);
                foreach (LightSource currentLightSource in Game1.currentLightSources)
                {
                  if (!Game1.IsRainingHere() && !Game1.isDarkOut() || currentLightSource.lightContext.Value != LightSource.LightContext.WindowLight)
                  {
                    if (currentLightSource.PlayerID != 0L && currentLightSource.PlayerID != Game1.player.UniqueMultiplayerID)
                    {
                      Farmer farmerMaybeOffline = Game1.getFarmerMaybeOffline(currentLightSource.PlayerID);
                      if (farmerMaybeOffline == null || farmerMaybeOffline.currentLocation != null && farmerMaybeOffline.currentLocation.Name != Game1.currentLocation.Name || (bool) (NetFieldBase<bool, NetBool>) farmerMaybeOffline.hidden)
                        continue;
                    }
                    if (Utility.isOnScreen((Vector2) (NetFieldBase<Vector2, NetVector2>) currentLightSource.position, (int) ((double) (float) (NetFieldBase<float, NetFloat>) currentLightSource.radius * 64.0 * 4.0)))
                      Game1.spriteBatch.Draw(currentLightSource.lightTexture, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetFieldBase<Vector2, NetVector2>) currentLightSource.position) / (float) (Game1.options.lightingQuality / 2), new Microsoft.Xna.Framework.Rectangle?(currentLightSource.lightTexture.Bounds), currentLightSource.color.Value * num, 0.0f, new Vector2((float) (currentLightSource.lightTexture.Bounds.Width / 2), (float) (currentLightSource.lightTexture.Bounds.Height / 2)), (float) (NetFieldBase<float, NetFloat>) currentLightSource.radius / (float) (Game1.options.lightingQuality / 2), SpriteEffects.None, 0.9f);
                  }
                }
                Game1.spriteBatch.End();
                Game1.SetRenderTarget(target_screen);
              }
              this.GraphicsDevice.Clear(Game1.bgColor);
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              if (Game1.background != null)
                Game1.background.draw(Game1.spriteBatch);
              Game1.currentLocation.drawBackground(Game1.spriteBatch);
              Game1.mapDisplayDevice.BeginScene(Game1.spriteBatch);
              Game1.currentLocation.Map.GetLayer("Back").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
              Game1.currentLocation.drawWater(Game1.spriteBatch);
              Game1.spriteBatch.End();
              Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
              Game1.currentLocation.drawFloorDecorations(Game1.spriteBatch);
              Game1.spriteBatch.End();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              this._farmerShadows.Clear();
              if (Game1.currentLocation.currentEvent != null && !Game1.currentLocation.currentEvent.isFestival && Game1.currentLocation.currentEvent.farmerActors.Count > 0)
              {
                foreach (Farmer farmerActor in Game1.currentLocation.currentEvent.farmerActors)
                {
                  if (farmerActor.IsLocalPlayer && Game1.displayFarmer || !(bool) (NetFieldBase<bool, NetBool>) farmerActor.hidden)
                    this._farmerShadows.Add(farmerActor);
                }
              }
              else
              {
                foreach (Farmer farmer in Game1.currentLocation.farmers)
                {
                  if (farmer.IsLocalPlayer && Game1.displayFarmer || !(bool) (NetFieldBase<bool, NetBool>) farmer.hidden)
                    this._farmerShadows.Add(farmer);
                }
              }
              if (!Game1.currentLocation.shouldHideCharacters())
              {
                if (Game1.CurrentEvent == null)
                {
                  foreach (NPC character in Game1.currentLocation.characters)
                  {
                    if (!(bool) (NetFieldBase<bool, NetBool>) character.swimming && !character.HideShadow && !character.IsInvisible && !this.checkCharacterTilesForShadowDrawFlag((Character) character))
                    {
                      SpriteBatch spriteBatch = Game1.spriteBatch;
                      Texture2D shadowTexture = Game1.shadowTexture;
                      Vector2 local = Game1.GlobalToLocal(Game1.viewport, character.GetShadowOffset() + character.Position + new Vector2((float) (character.GetSpriteWidthForPositioning() * 4) / 2f, (float) (character.GetBoundingBox().Height + (character.IsMonster ? 0 : 12))));
                      Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
                      Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
                      Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
                      double x = (double) bounds.Center.X;
                      bounds = Game1.shadowTexture.Bounds;
                      double y = (double) bounds.Center.Y;
                      Vector2 origin = new Vector2((float) x, (float) y);
                      double scale = (double) Math.Max(0.0f, (float) (4.0 + (double) character.yJumpOffset / 40.0) * (float) (NetFieldBase<float, NetFloat>) character.scale);
                      double layerDepth = (double) Math.Max(0.0f, (float) character.getStandingY() / 10000f) - 9.99999997475243E-07;
                      spriteBatch.Draw(shadowTexture, local, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
                    }
                  }
                }
                else
                {
                  foreach (NPC actor in Game1.CurrentEvent.actors)
                  {
                    if ((Game1.CurrentEvent == null || !Game1.CurrentEvent.ShouldHideCharacter(actor)) && !(bool) (NetFieldBase<bool, NetBool>) actor.swimming && !actor.HideShadow && !this.checkCharacterTilesForShadowDrawFlag((Character) actor))
                    {
                      SpriteBatch spriteBatch = Game1.spriteBatch;
                      Texture2D shadowTexture = Game1.shadowTexture;
                      Vector2 local = Game1.GlobalToLocal(Game1.viewport, actor.GetShadowOffset() + actor.Position + new Vector2((float) (actor.GetSpriteWidthForPositioning() * 4) / 2f, (float) (actor.GetBoundingBox().Height + (actor.IsMonster ? 0 : (actor.Sprite.SpriteHeight <= 16 ? -4 : 12)))));
                      Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
                      Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
                      Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
                      double x = (double) bounds.Center.X;
                      bounds = Game1.shadowTexture.Bounds;
                      double y = (double) bounds.Center.Y;
                      Vector2 origin = new Vector2((float) x, (float) y);
                      double scale = (double) Math.Max(0.0f, (float) (4.0 + (double) actor.yJumpOffset / 40.0)) * (double) (float) (NetFieldBase<float, NetFloat>) actor.scale;
                      double layerDepth = (double) Math.Max(0.0f, (float) actor.getStandingY() / 10000f) - 9.99999997475243E-07;
                      spriteBatch.Draw(shadowTexture, local, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
                    }
                  }
                }
                foreach (Farmer farmerShadow in this._farmerShadows)
                {
                  if (!Game1.multiplayer.isDisconnecting(farmerShadow.UniqueMultiplayerID) && !(bool) (NetFieldBase<bool, NetBool>) farmerShadow.swimming && !farmerShadow.isRidingHorse() && !farmerShadow.IsSitting() && (Game1.currentLocation == null || !this.checkCharacterTilesForShadowDrawFlag((Character) farmerShadow)))
                  {
                    SpriteBatch spriteBatch = Game1.spriteBatch;
                    Texture2D shadowTexture = Game1.shadowTexture;
                    Vector2 local = Game1.GlobalToLocal(farmerShadow.GetShadowOffset() + farmerShadow.Position + new Vector2(32f, 24f));
                    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
                    Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
                    Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
                    double x = (double) bounds.Center.X;
                    bounds = Game1.shadowTexture.Bounds;
                    double y = (double) bounds.Center.Y;
                    Vector2 origin = new Vector2((float) x, (float) y);
                    double scale = 4.0 - (!farmerShadow.running && !farmerShadow.UsingTool || farmerShadow.FarmerSprite.currentAnimationIndex <= 1 ? 0.0 : (double) Math.Abs(FarmerRenderer.featureYOffsetPerFrame[farmerShadow.FarmerSprite.CurrentFrame]) * 0.5);
                    spriteBatch.Draw(shadowTexture, local, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, 0.0f);
                  }
                }
              }
              Layer layer = Game1.currentLocation.Map.GetLayer("Buildings");
              layer.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
              Game1.mapDisplayDevice.EndScene();
              Game1.spriteBatch.End();
              Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
              if (!Game1.currentLocation.shouldHideCharacters())
              {
                if (Game1.CurrentEvent == null)
                {
                  foreach (NPC character in Game1.currentLocation.characters)
                  {
                    if (!(bool) (NetFieldBase<bool, NetBool>) character.swimming && !character.HideShadow && !(bool) (NetFieldBase<bool, NetBool>) character.isInvisible && this.checkCharacterTilesForShadowDrawFlag((Character) character))
                    {
                      SpriteBatch spriteBatch = Game1.spriteBatch;
                      Texture2D shadowTexture = Game1.shadowTexture;
                      Vector2 local = Game1.GlobalToLocal(Game1.viewport, character.GetShadowOffset() + character.Position + new Vector2((float) (character.GetSpriteWidthForPositioning() * 4) / 2f, (float) (character.GetBoundingBox().Height + (character.IsMonster ? 0 : 12))));
                      Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
                      Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
                      Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
                      double x = (double) bounds.Center.X;
                      bounds = Game1.shadowTexture.Bounds;
                      double y = (double) bounds.Center.Y;
                      Vector2 origin = new Vector2((float) x, (float) y);
                      double scale = (double) Math.Max(0.0f, (float) (4.0 + (double) character.yJumpOffset / 40.0) * (float) (NetFieldBase<float, NetFloat>) character.scale);
                      double layerDepth = (double) Math.Max(0.0f, (float) character.getStandingY() / 10000f) - 9.99999997475243E-07;
                      spriteBatch.Draw(shadowTexture, local, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
                    }
                  }
                }
                else
                {
                  foreach (NPC actor in Game1.CurrentEvent.actors)
                  {
                    if ((Game1.CurrentEvent == null || !Game1.CurrentEvent.ShouldHideCharacter(actor)) && !(bool) (NetFieldBase<bool, NetBool>) actor.swimming && !actor.HideShadow && this.checkCharacterTilesForShadowDrawFlag((Character) actor))
                    {
                      SpriteBatch spriteBatch = Game1.spriteBatch;
                      Texture2D shadowTexture = Game1.shadowTexture;
                      Vector2 local = Game1.GlobalToLocal(Game1.viewport, actor.GetShadowOffset() + actor.Position + new Vector2((float) (actor.GetSpriteWidthForPositioning() * 4) / 2f, (float) (actor.GetBoundingBox().Height + (actor.IsMonster ? 0 : 12))));
                      Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
                      Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
                      Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
                      double x = (double) bounds.Center.X;
                      bounds = Game1.shadowTexture.Bounds;
                      double y = (double) bounds.Center.Y;
                      Vector2 origin = new Vector2((float) x, (float) y);
                      double scale = (double) Math.Max(0.0f, (float) (4.0 + (double) actor.yJumpOffset / 40.0) * (float) (NetFieldBase<float, NetFloat>) actor.scale);
                      double layerDepth = (double) Math.Max(0.0f, (float) actor.getStandingY() / 10000f) - 9.99999997475243E-07;
                      spriteBatch.Draw(shadowTexture, local, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
                    }
                  }
                }
                foreach (Farmer farmerShadow in this._farmerShadows)
                {
                  float num = Math.Max(0.0001f, farmerShadow.getDrawLayer() + 0.00011f) - 0.0001f;
                  if (!(bool) (NetFieldBase<bool, NetBool>) farmerShadow.swimming && !farmerShadow.isRidingHorse() && !farmerShadow.IsSitting() && Game1.currentLocation != null && this.checkCharacterTilesForShadowDrawFlag((Character) farmerShadow))
                  {
                    SpriteBatch spriteBatch = Game1.spriteBatch;
                    Texture2D shadowTexture = Game1.shadowTexture;
                    Vector2 local = Game1.GlobalToLocal(farmerShadow.GetShadowOffset() + farmerShadow.Position + new Vector2(32f, 24f));
                    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
                    Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
                    Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
                    double x = (double) bounds.Center.X;
                    bounds = Game1.shadowTexture.Bounds;
                    double y = (double) bounds.Center.Y;
                    Vector2 origin = new Vector2((float) x, (float) y);
                    double scale = 4.0 - (!farmerShadow.running && !farmerShadow.UsingTool || farmerShadow.FarmerSprite.currentAnimationIndex <= 1 ? 0.0 : (double) Math.Abs(FarmerRenderer.featureYOffsetPerFrame[farmerShadow.FarmerSprite.CurrentFrame]) * 0.5);
                    double layerDepth = (double) num;
                    spriteBatch.Draw(shadowTexture, local, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
                  }
                }
              }
              if ((Game1.eventUp || Game1.killScreen) && !Game1.killScreen && Game1.currentLocation.currentEvent != null)
                Game1.currentLocation.currentEvent.draw(Game1.spriteBatch);
              if (Game1.player.currentUpgrade != null && Game1.player.currentUpgrade.daysLeftTillUpgradeDone <= 3 && Game1.currentLocation.Name.Equals("Farm"))
                Game1.spriteBatch.Draw(Game1.player.currentUpgrade.workerTexture, Game1.GlobalToLocal(Game1.viewport, Game1.player.currentUpgrade.positionOfCarpenter), new Microsoft.Xna.Framework.Rectangle?(Game1.player.currentUpgrade.getSourceRectangle()), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) (((double) Game1.player.currentUpgrade.positionOfCarpenter.Y + 48.0) / 10000.0));
              Game1.currentLocation.draw(Game1.spriteBatch);
              foreach (Vector2 key in Game1.crabPotOverlayTiles.Keys)
              {
                Tile tile = layer.Tiles[(int) key.X, (int) key.Y];
                if (tile != null)
                {
                  Vector2 local = Game1.GlobalToLocal(Game1.viewport, key * 64f);
                  Location location = new Location((int) local.X, (int) local.Y);
                  Game1.mapDisplayDevice.DrawTile(tile, location, (float) (((double) key.Y * 64.0 - 1.0) / 10000.0));
                }
              }
              if (Game1.eventUp && Game1.currentLocation.currentEvent != null)
              {
                string messageToScreen = Game1.currentLocation.currentEvent.messageToScreen;
              }
              if (Game1.player.ActiveObject == null && (Game1.player.UsingTool || Game1.pickingTool) && Game1.player.CurrentTool != null && (!Game1.player.CurrentTool.Name.Equals("Seeds") || Game1.pickingTool))
                Game1.drawTool(Game1.player);
              if (Game1.currentLocation.Name.Equals("Farm"))
                this.drawFarmBuildings();
              if (Game1.tvStation >= 0)
                Game1.spriteBatch.Draw(Game1.tvStationTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(400f, 160f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Game1.tvStation * 24, 0, 24, 15)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);
              if (Game1.panMode)
              {
                Game1.spriteBatch.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle((int) Math.Floor((double) (Game1.getOldMouseX() + Game1.viewport.X) / 64.0) * 64 - Game1.viewport.X, (int) Math.Floor((double) (Game1.getOldMouseY() + Game1.viewport.Y) / 64.0) * 64 - Game1.viewport.Y, 64, 64), Microsoft.Xna.Framework.Color.Lime * 0.75f);
                foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) Game1.currentLocation.warps)
                  Game1.spriteBatch.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(warp.X * 64 - Game1.viewport.X, warp.Y * 64 - Game1.viewport.Y, 64, 64), Microsoft.Xna.Framework.Color.Red * 0.75f);
              }
              Game1.mapDisplayDevice.BeginScene(Game1.spriteBatch);
              Game1.currentLocation.Map.GetLayer("Front").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
              Game1.mapDisplayDevice.EndScene();
              Game1.currentLocation.drawAboveFrontLayer(Game1.spriteBatch);
              Game1.spriteBatch.End();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              if (Game1.currentLocation.Map.GetLayer("AlwaysFront") != null)
              {
                Game1.mapDisplayDevice.BeginScene(Game1.spriteBatch);
                Game1.currentLocation.Map.GetLayer("AlwaysFront").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
                Game1.mapDisplayDevice.EndScene();
              }
              if ((double) Game1.toolHold > 400.0 && Game1.player.CurrentTool.UpgradeLevel >= 1 && Game1.player.canReleaseTool)
              {
                Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;
                switch ((int) ((double) Game1.toolHold / 600.0) + 2)
                {
                  case 1:
                    color = Tool.copperColor;
                    break;
                  case 2:
                    color = Tool.steelColor;
                    break;
                  case 3:
                    color = Tool.goldColor;
                    break;
                  case 4:
                    color = Tool.iridiumColor;
                    break;
                }
                Game1.spriteBatch.Draw(Game1.littleEffect, new Microsoft.Xna.Framework.Rectangle((int) Game1.player.getLocalPosition(Game1.viewport).X - 2, (int) Game1.player.getLocalPosition(Game1.viewport).Y - (Game1.player.CurrentTool.Name.Equals("Watering Can") ? 0 : 64) - 2, (int) ((double) Game1.toolHold % 600.0 * 0.0799999982118607) + 4, 12), Microsoft.Xna.Framework.Color.Black);
                Game1.spriteBatch.Draw(Game1.littleEffect, new Microsoft.Xna.Framework.Rectangle((int) Game1.player.getLocalPosition(Game1.viewport).X, (int) Game1.player.getLocalPosition(Game1.viewport).Y - (Game1.player.CurrentTool.Name.Equals("Watering Can") ? 0 : 64), (int) ((double) Game1.toolHold % 600.0 * 0.0799999982118607), 8), color);
              }
              if (!Game1.IsFakedBlackScreen())
                this.drawWeather(gameTime, target_screen);
              if (Game1.farmEvent != null)
                Game1.farmEvent.draw(Game1.spriteBatch);
              if ((double) Game1.currentLocation.LightLevel > 0.0 && Game1.timeOfDay < 2000)
              {
                SpriteBatch spriteBatch = Game1.spriteBatch;
                Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
                viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                Microsoft.Xna.Framework.Rectangle bounds = viewport1.Bounds;
                Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.Black * Game1.currentLocation.LightLevel;
                spriteBatch.Draw(fadeToBlackRect, bounds, color);
              }
              if (Game1.screenGlow)
              {
                SpriteBatch spriteBatch = Game1.spriteBatch;
                Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
                viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                Microsoft.Xna.Framework.Rectangle bounds = viewport1.Bounds;
                Microsoft.Xna.Framework.Color color = Game1.screenGlowColor * Game1.screenGlowAlpha;
                spriteBatch.Draw(fadeToBlackRect, bounds, color);
              }
              Game1.currentLocation.drawAboveAlwaysFrontLayer(Game1.spriteBatch);
              if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is FishingRod && ((Game1.player.CurrentTool as FishingRod).isTimingCast || (double) (Game1.player.CurrentTool as FishingRod).castingChosenCountdown > 0.0 || (Game1.player.CurrentTool as FishingRod).fishCaught || (Game1.player.CurrentTool as FishingRod).showingTreasure))
                Game1.player.CurrentTool.draw(Game1.spriteBatch);
              Game1.spriteBatch.End();
              Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
              if (Game1.eventUp && Game1.currentLocation.currentEvent != null)
              {
                foreach (NPC actor in Game1.currentLocation.currentEvent.actors)
                {
                  if (actor.isEmoting)
                  {
                    Vector2 localPosition = actor.getLocalPosition(Game1.viewport);
                    if (actor.NeedsBirdieEmoteHack())
                      localPosition.X += 64f;
                    localPosition.Y -= 140f;
                    if (actor.Age == 2)
                      localPosition.Y += 32f;
                    else if (actor.Gender == 1)
                      localPosition.Y += 10f;
                    Game1.spriteBatch.Draw(Game1.emoteSpriteSheet, localPosition, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(actor.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, actor.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) actor.getStandingY() / 10000f);
                  }
                }
              }
              Game1.spriteBatch.End();
              if (Game1.drawLighting && !Game1.IsFakedBlackScreen())
              {
                Game1.spriteBatch.Begin(blendState: this.lightingBlend, samplerState: SamplerState.LinearClamp);
                Viewport viewport2 = this.GraphicsDevice.Viewport with
                {
                  Bounds = target_screen != null ? target_screen.Bounds : this.GraphicsDevice.PresentationParameters.Bounds
                };
                this.GraphicsDevice.Viewport = viewport2;
                float scale = (float) (Game1.options.lightingQuality / 2);
                if (this.useUnscaledLighting)
                  scale /= Game1.options.zoomLevel;
                Game1.spriteBatch.Draw((Texture2D) Game1.lightmap, Vector2.Zero, new Microsoft.Xna.Framework.Rectangle?(Game1.lightmap.Bounds), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
                if (Game1.IsRainingHere() && (bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.isOutdoors && !(Game1.currentLocation is Desert))
                  Game1.spriteBatch.Draw(Game1.staminaRect, viewport2.Bounds, Microsoft.Xna.Framework.Color.OrangeRed * 0.45f);
                Game1.spriteBatch.End();
              }
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              if (Game1.drawGrid)
              {
                int num1 = -Game1.viewport.X % 64;
                float num2 = (float) (-Game1.viewport.Y % 64);
                int num3 = num1;
                while (true)
                {
                  int num4 = num3;
                  viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                  int width = viewport1.Width;
                  if (num4 < width)
                  {
                    SpriteBatch spriteBatch = Game1.spriteBatch;
                    Texture2D staminaRect = Game1.staminaRect;
                    int x = num3;
                    int y = (int) num2;
                    viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                    int height = viewport1.Height;
                    Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle(x, y, 1, height);
                    Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.Red * 0.5f;
                    spriteBatch.Draw(staminaRect, destinationRectangle, color);
                    num3 += 64;
                  }
                  else
                    break;
                }
                float num5 = num2;
                while (true)
                {
                  double num6 = (double) num5;
                  viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                  double height = (double) viewport1.Height;
                  if (num6 < height)
                  {
                    SpriteBatch spriteBatch = Game1.spriteBatch;
                    Texture2D staminaRect = Game1.staminaRect;
                    int x = num1;
                    int y = (int) num5;
                    viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                    int width = viewport1.Width;
                    Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle(x, y, width, 1);
                    Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.Red * 0.5f;
                    spriteBatch.Draw(staminaRect, destinationRectangle, color);
                    num5 += 64f;
                  }
                  else
                    break;
                }
              }
              if (Game1.ShouldShowOnscreenUsernames() && Game1.currentLocation != null)
                Game1.currentLocation.DrawFarmerUsernames(Game1.spriteBatch);
              if (Game1.currentBillboard != 0 && !this.takingMapScreenshot)
                this.drawBillboard();
              if (!Game1.eventUp && Game1.farmEvent == null && Game1.currentBillboard == 0 && Game1.gameMode == (byte) 3 && !this.takingMapScreenshot && Game1.isOutdoorMapSmallerThanViewport())
              {
                SpriteBatch spriteBatch1 = Game1.spriteBatch;
                Texture2D fadeToBlackRect1 = Game1.fadeToBlackRect;
                int width1 = -Game1.viewport.X;
                viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                int height1 = viewport1.Height;
                Microsoft.Xna.Framework.Rectangle destinationRectangle1 = new Microsoft.Xna.Framework.Rectangle(0, 0, width1, height1);
                Microsoft.Xna.Framework.Color black1 = Microsoft.Xna.Framework.Color.Black;
                spriteBatch1.Draw(fadeToBlackRect1, destinationRectangle1, black1);
                SpriteBatch spriteBatch2 = Game1.spriteBatch;
                Texture2D fadeToBlackRect2 = Game1.fadeToBlackRect;
                int x = -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64;
                viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                int width2 = viewport1.Width - (-Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64);
                viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                int height2 = viewport1.Height;
                Microsoft.Xna.Framework.Rectangle destinationRectangle2 = new Microsoft.Xna.Framework.Rectangle(x, 0, width2, height2);
                Microsoft.Xna.Framework.Color black2 = Microsoft.Xna.Framework.Color.Black;
                spriteBatch2.Draw(fadeToBlackRect2, destinationRectangle2, black2);
                SpriteBatch spriteBatch3 = Game1.spriteBatch;
                Texture2D fadeToBlackRect3 = Game1.fadeToBlackRect;
                viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                Microsoft.Xna.Framework.Rectangle destinationRectangle3 = new Microsoft.Xna.Framework.Rectangle(0, 0, viewport1.Width, -Game1.viewport.Y);
                Microsoft.Xna.Framework.Color black3 = Microsoft.Xna.Framework.Color.Black;
                spriteBatch3.Draw(fadeToBlackRect3, destinationRectangle3, black3);
                SpriteBatch spriteBatch4 = Game1.spriteBatch;
                Texture2D fadeToBlackRect4 = Game1.fadeToBlackRect;
                int y = -Game1.viewport.Y + Game1.currentLocation.map.Layers[0].LayerHeight * 64;
                viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                int width3 = viewport1.Width;
                viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                int height3 = viewport1.Height - (-Game1.viewport.Y + Game1.currentLocation.map.Layers[0].LayerHeight * 64);
                Microsoft.Xna.Framework.Rectangle destinationRectangle4 = new Microsoft.Xna.Framework.Rectangle(0, y, width3, height3);
                Microsoft.Xna.Framework.Color black4 = Microsoft.Xna.Framework.Color.Black;
                spriteBatch4.Draw(fadeToBlackRect4, destinationRectangle4, black4);
              }
              Game1.spriteBatch.End();
              Game1.PushUIMode();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              if ((Game1.displayHUD || Game1.eventUp) && Game1.currentBillboard == 0 && Game1.gameMode == (byte) 3 && !Game1.freezeControls && !Game1.panMode && !Game1.HostPaused && !this.takingMapScreenshot)
                this.drawHUD();
              else if (Game1.activeClickableMenu == null)
              {
                FarmEvent farmEvent = Game1.farmEvent;
              }
              if (Game1.hudMessages.Count > 0 && !this.takingMapScreenshot)
              {
                for (int index = Game1.hudMessages.Count - 1; index >= 0; --index)
                  Game1.hudMessages[index].draw(Game1.spriteBatch, index);
              }
              Game1.spriteBatch.End();
              Game1.PopUIMode();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            }
            if (Game1.farmEvent != null)
            {
              Game1.farmEvent.draw(Game1.spriteBatch);
              Game1.spriteBatch.End();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            }
            Game1.PushUIMode();
            if (Game1.dialogueUp && !Game1.nameSelectUp && !Game1.messagePause && (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is DialogueBox)) && !this.takingMapScreenshot)
              this.drawDialogueBox();
            if (Game1.progressBar && !this.takingMapScreenshot)
            {
              Game1.spriteBatch.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle((Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Width - Game1.dialogueWidth) / 2, Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 128, Game1.dialogueWidth, 32), Microsoft.Xna.Framework.Color.LightGray);
              Game1.spriteBatch.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Width - Game1.dialogueWidth) / 2, Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 128, (int) ((double) Game1.pauseAccumulator / (double) Game1.pauseTime * (double) Game1.dialogueWidth), 32), Microsoft.Xna.Framework.Color.DimGray);
            }
            Game1.spriteBatch.End();
            Game1.PopUIMode();
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            if (Game1.eventUp && Game1.currentLocation != null && Game1.currentLocation.currentEvent != null)
              Game1.currentLocation.currentEvent.drawAfterMap(Game1.spriteBatch);
            if (!Game1.IsFakedBlackScreen() && Game1.IsRainingHere() && Game1.currentLocation != null && (bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.isOutdoors && !(Game1.currentLocation is Desert))
            {
              SpriteBatch spriteBatch = Game1.spriteBatch;
              Texture2D staminaRect = Game1.staminaRect;
              viewport1 = Game1.graphics.GraphicsDevice.Viewport;
              Microsoft.Xna.Framework.Rectangle bounds = viewport1.Bounds;
              Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.Blue * 0.2f;
              spriteBatch.Draw(staminaRect, bounds, color);
            }
            if ((Game1.fadeToBlack || Game1.globalFade) && !Game1.menuUp && (!Game1.nameSelectUp || Game1.messagePause) && !this.takingMapScreenshot)
            {
              Game1.spriteBatch.End();
              Game1.PushUIMode();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              SpriteBatch spriteBatch = Game1.spriteBatch;
              Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
              viewport1 = Game1.graphics.GraphicsDevice.Viewport;
              Microsoft.Xna.Framework.Rectangle bounds = viewport1.Bounds;
              Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.Black * (Game1.gameMode == (byte) 0 ? 1f - Game1.fadeToBlackAlpha : Game1.fadeToBlackAlpha);
              spriteBatch.Draw(fadeToBlackRect, bounds, color);
              Game1.spriteBatch.End();
              Game1.PopUIMode();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            }
            else if ((double) Game1.flashAlpha > 0.0 && !this.takingMapScreenshot)
            {
              if (Game1.options.screenFlash)
              {
                SpriteBatch spriteBatch = Game1.spriteBatch;
                Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
                viewport1 = Game1.graphics.GraphicsDevice.Viewport;
                Microsoft.Xna.Framework.Rectangle bounds = viewport1.Bounds;
                Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White * Math.Min(1f, Game1.flashAlpha);
                spriteBatch.Draw(fadeToBlackRect, bounds, color);
              }
              Game1.flashAlpha -= 0.1f;
            }
            if ((Game1.messagePause || Game1.globalFade) && Game1.dialogueUp && !this.takingMapScreenshot)
              this.drawDialogueBox();
            if (!this.takingMapScreenshot)
            {
              foreach (TemporaryAnimatedSprite overlayTempSprite in Game1.screenOverlayTempSprites)
                overlayTempSprite.draw(Game1.spriteBatch, true);
              Game1.spriteBatch.End();
              Game1.PushUIMode();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              foreach (TemporaryAnimatedSprite overlayTempSprite in Game1.uiOverlayTempSprites)
                overlayTempSprite.draw(Game1.spriteBatch, true);
              Game1.spriteBatch.End();
              Game1.PopUIMode();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            }
            if (Game1.debugMode)
            {
              StringBuilder debugStringBuilder = Game1._debugStringBuilder;
              debugStringBuilder.Clear();
              if (Game1.panMode)
              {
                debugStringBuilder.Append((Game1.getOldMouseX() + Game1.viewport.X) / 64);
                debugStringBuilder.Append(",");
                debugStringBuilder.Append((Game1.getOldMouseY() + Game1.viewport.Y) / 64);
              }
              else
              {
                debugStringBuilder.Append("player: ");
                debugStringBuilder.Append(Game1.player.getStandingX() / 64);
                debugStringBuilder.Append(", ");
                debugStringBuilder.Append(Game1.player.getStandingY() / 64);
              }
              debugStringBuilder.Append(" mouseTransparency: ");
              debugStringBuilder.Append(Game1.mouseCursorTransparency);
              debugStringBuilder.Append(" mousePosition: ");
              debugStringBuilder.Append(Game1.getMouseX());
              debugStringBuilder.Append(",");
              debugStringBuilder.Append(Game1.getMouseY());
              debugStringBuilder.Append(Environment.NewLine);
              debugStringBuilder.Append(" mouseWorldPosition: ");
              debugStringBuilder.Append(Game1.getMouseX() + Game1.viewport.X);
              debugStringBuilder.Append(",");
              debugStringBuilder.Append(Game1.getMouseY() + Game1.viewport.Y);
              debugStringBuilder.Append("  debugOutput: ");
              debugStringBuilder.Append(Game1.debugOutput);
              Game1.spriteBatch.DrawString(Game1.smallFont, debugStringBuilder, new Vector2((float) this.GraphicsDevice.Viewport.GetTitleSafeArea().X, (float) (this.GraphicsDevice.Viewport.GetTitleSafeArea().Y + Game1.smallFont.LineSpacing * 8)), Microsoft.Xna.Framework.Color.Red, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999999f);
            }
            Game1.spriteBatch.End();
            Game1.PushUIMode();
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            if (Game1.showKeyHelp && !this.takingMapScreenshot)
              Game1.spriteBatch.DrawString(Game1.smallFont, Game1.keyHelpString, new Vector2(64f, (float) (Game1.viewport.Height - 64 - (Game1.dialogueUp ? 192 + (Game1.isQuestion ? Game1.questionChoices.Count * 64 : 0) : 0)) - Game1.smallFont.MeasureString(Game1.keyHelpString).Y), Microsoft.Xna.Framework.Color.LightGray, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999999f);
            if (Game1.activeClickableMenu != null && !this.takingMapScreenshot)
            {
              for (IClickableMenu iclickableMenu = Game1.activeClickableMenu; iclickableMenu != null; iclickableMenu = iclickableMenu.GetChildMenu())
                iclickableMenu.draw(Game1.spriteBatch);
            }
            else if (Game1.farmEvent != null)
              Game1.farmEvent.drawAboveEverything(Game1.spriteBatch);
            if (Game1.specialCurrencyDisplay != null)
              Game1.specialCurrencyDisplay.Draw(Game1.spriteBatch);
            if (Game1.emoteMenu != null && !this.takingMapScreenshot)
              Game1.emoteMenu.draw(Game1.spriteBatch);
            if (Game1.HostPaused && !this.takingMapScreenshot)
            {
              string s = Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10378");
              SpriteText.drawStringWithScrollBackground(Game1.spriteBatch, s, 96, 32);
            }
            Game1.spriteBatch.End();
            this.drawOverlays(Game1.spriteBatch);
            Game1.PopUIMode();
          }
        }
      }
    }

    public virtual void drawWeather(GameTime time, RenderTarget2D target_screen)
    {
      if (Game1.IsSnowingHere() && (bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.isOutdoors && !(Game1.currentLocation is Desert))
      {
        Game1.snowPos.X %= 64f;
        Vector2 position = new Vector2();
        for (float num1 = (float) ((double) Game1.snowPos.X % 64.0 - 64.0); (double) num1 < (double) Game1.viewport.Width; num1 += 64f)
        {
          for (float num2 = (float) ((double) Game1.snowPos.Y % 64.0 - 64.0); (double) num2 < (double) Game1.viewport.Height; num2 += 64f)
          {
            position.X = (float) (int) num1;
            position.Y = (float) (int) num2;
            Game1.spriteBatch.Draw(Game1.mouseCursors, position, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(368 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1200.0) / 75 * 16, 192, 16, 16)), Microsoft.Xna.Framework.Color.White * 0.8f * Game1.options.snowTransparency, 0.0f, Vector2.Zero, 4.001f, SpriteEffects.None, 1f);
          }
        }
      }
      if (Game1.currentLocation.IsOutdoors && !(bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.ignoreDebrisWeather && Game1.IsDebrisWeatherHere() && !Game1.currentLocation.Name.Equals("Desert"))
      {
        if (this.takingMapScreenshot)
        {
          if (Game1.debrisWeather != null)
          {
            foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
            {
              Vector2 position = weatherDebris.position;
              weatherDebris.position = new Vector2((float) Game1.random.Next(Game1.viewport.Width - weatherDebris.sourceRect.Width * 3), (float) Game1.random.Next(Game1.viewport.Height - weatherDebris.sourceRect.Height * 3));
              weatherDebris.draw(Game1.spriteBatch);
              weatherDebris.position = position;
            }
          }
        }
        else if (Game1.viewport.X > -Game1.viewport.Width)
        {
          foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
            weatherDebris.draw(Game1.spriteBatch);
        }
      }
      if (!Game1.IsRainingHere() || !Game1.currentLocation.IsOutdoors || Game1.currentLocation.Name.Equals("Desert") || Game1.currentLocation is Summit)
        return;
      if (this.takingMapScreenshot)
      {
        for (int index = 0; index < Game1.rainDrops.Length; ++index)
        {
          Vector2 position = new Vector2((float) Game1.random.Next(Game1.viewport.Width - 64), (float) Game1.random.Next(Game1.viewport.Height - 64));
          Game1.spriteBatch.Draw(Game1.rainTexture, position, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.rainTexture, Game1.rainDrops[index].frame)), Microsoft.Xna.Framework.Color.White);
        }
      }
      else
      {
        if (Game1.eventUp && !Game1.currentLocation.isTileOnMap(new Vector2((float) (Game1.viewport.X / 64), (float) (Game1.viewport.Y / 64))))
          return;
        for (int index = 0; index < Game1.rainDrops.Length; ++index)
          Game1.spriteBatch.Draw(Game1.rainTexture, Game1.rainDrops[index].position, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.rainTexture, Game1.rainDrops[index].frame)), Microsoft.Xna.Framework.Color.White);
      }
    }

    protected virtual void renderScreenBuffer(RenderTarget2D target_screen)
    {
      Game1.graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      if (this.takingMapScreenshot || LocalMultiplayer.IsLocalMultiplayer() || target_screen != null && target_screen.IsContentLost)
        return;
      if (this.ShouldDrawOnBuffer() && target_screen != null)
      {
        this.GraphicsDevice.Clear(Game1.bgColor);
        Game1.spriteBatch.Begin(blendState: BlendState.Opaque, samplerState: SamplerState.LinearClamp, depthStencilState: DepthStencilState.Default, rasterizerState: RasterizerState.CullNone);
        Game1.spriteBatch.Draw((Texture2D) target_screen, new Vector2(0.0f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(target_screen.Bounds), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, Game1.options.zoomLevel, SpriteEffects.None, 1f);
        Game1.spriteBatch.End();
        Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.LinearClamp, depthStencilState: DepthStencilState.Default, rasterizerState: RasterizerState.CullNone);
        Game1.spriteBatch.Draw((Texture2D) this.uiScreen, new Vector2(0.0f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(this.uiScreen.Bounds), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, Game1.options.uiScale, SpriteEffects.None, 1f);
        Game1.spriteBatch.End();
      }
      else
      {
        Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.LinearClamp, depthStencilState: DepthStencilState.Default, rasterizerState: RasterizerState.CullNone);
        Game1.spriteBatch.Draw((Texture2D) this.uiScreen, new Vector2(0.0f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(this.uiScreen.Bounds), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, Game1.options.uiScale, SpriteEffects.None, 1f);
        Game1.spriteBatch.End();
      }
    }

    public virtual void DrawSplitScreenWindow()
    {
      if (!LocalMultiplayer.IsLocalMultiplayer())
        return;
      Game1.graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      if (this.screen != null && this.screen.IsContentLost)
        return;
      Viewport viewport = this.GraphicsDevice.Viewport;
      this.GraphicsDevice.Viewport = this.GraphicsDevice.Viewport = Game1.defaultDeviceViewport;
      Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.LinearClamp, depthStencilState: DepthStencilState.Default, rasterizerState: RasterizerState.CullNone);
      Game1.spriteBatch.Draw((Texture2D) this.screen, new Vector2((float) this.localMultiplayerWindow.X, (float) this.localMultiplayerWindow.Y), new Microsoft.Xna.Framework.Rectangle?(this.screen.Bounds), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, this.instanceOptions.zoomLevel, SpriteEffects.None, 1f);
      if (this.uiScreen != null)
        Game1.spriteBatch.Draw((Texture2D) this.uiScreen, new Vector2((float) this.localMultiplayerWindow.X, (float) this.localMultiplayerWindow.Y), new Microsoft.Xna.Framework.Rectangle?(this.uiScreen.Bounds), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, this.instanceOptions.uiScale, SpriteEffects.None, 1f);
      Game1.spriteBatch.End();
      this.GraphicsDevice.Viewport = viewport;
    }

    /// ###########################
    ///             METHODS FOR DRAWING THINGS.
    ///             ############################
    public static void drawWithBorder(
      string message,
      Microsoft.Xna.Framework.Color borderColor,
      Microsoft.Xna.Framework.Color insideColor,
      Vector2 position)
    {
      Game1.drawWithBorder(message, borderColor, insideColor, position, 0.0f, 1f, 1f, false);
    }

    public static void drawWithBorder(
      string message,
      Microsoft.Xna.Framework.Color borderColor,
      Microsoft.Xna.Framework.Color insideColor,
      Vector2 position,
      float rotate,
      float scale,
      float layerDepth)
    {
      Game1.drawWithBorder(message, borderColor, insideColor, position, rotate, scale, layerDepth, false);
    }

    public static void drawWithBorder(
      string message,
      Microsoft.Xna.Framework.Color borderColor,
      Microsoft.Xna.Framework.Color insideColor,
      Vector2 position,
      float rotate,
      float scale,
      float layerDepth,
      bool tiny)
    {
      string[] strArray = message.Split(Utility.CharSpace);
      int num = 0;
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (strArray[index].Contains("="))
        {
          Game1.spriteBatch.DrawString(tiny ? Game1.tinyFont : Game1.dialogueFont, strArray[index], new Vector2(position.X + (float) num, position.Y), Microsoft.Xna.Framework.Color.Purple, rotate, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
          num += (int) ((double) (tiny ? Game1.tinyFont : Game1.dialogueFont).MeasureString(strArray[index]).X + 8.0);
        }
        else
        {
          Game1.spriteBatch.DrawString(tiny ? Game1.tinyFont : Game1.dialogueFont, strArray[index], new Vector2(position.X + (float) num, position.Y), insideColor, rotate, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
          num += (int) ((double) (tiny ? Game1.tinyFont : Game1.dialogueFont).MeasureString(strArray[index]).X + 8.0);
        }
      }
    }

    public static bool isOutdoorMapSmallerThanViewport()
    {
      if (Game1.uiMode || Game1.currentLocation == null || !Game1.currentLocation.IsOutdoors || Game1.currentLocation is Summit)
        return false;
      return Game1.currentLocation.map.Layers[0].LayerWidth * 64 < Game1.viewport.Width || Game1.currentLocation.map.Layers[0].LayerHeight * 64 < Game1.viewport.Height;
    }

    protected virtual void drawHUD()
    {
      if (Game1.eventUp || Game1.farmEvent != null)
        return;
      float num1 = 0.625f;
      Vector2 position1 = new Vector2((float) (Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Right - 48 - 8), (float) (Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 224 - 16 - (int) ((double) (Game1.player.MaxStamina - 270) * (double) num1)));
      if (Game1.isOutdoorMapSmallerThanViewport())
        position1.X = Math.Min(position1.X, (float) (-Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 48));
      if (Game1.staminaShakeTimer > 0)
      {
        position1.X += (float) Game1.random.Next(-3, 4);
        position1.Y += (float) Game1.random.Next(-3, 4);
      }
      Game1.spriteBatch.Draw(Game1.mouseCursors, position1, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(256, 408, 12, 16)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      Game1.spriteBatch.Draw(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle((int) position1.X, (int) ((double) position1.Y + 64.0), 48, Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 64 - 16 - (int) ((double) position1.Y + 64.0 - 8.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(256, 424, 12, 16)), Microsoft.Xna.Framework.Color.White);
      Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(position1.X, (float) ((double) position1.Y + 224.0 + (double) (int) ((double) (Game1.player.MaxStamina - 270) * (double) num1) - 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(256, 448, 12, 16)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      Microsoft.Xna.Framework.Rectangle destinationRectangle1 = new Microsoft.Xna.Framework.Rectangle((int) position1.X + 12, (int) position1.Y + 16 + 32 + (int) ((double) (Game1.player.MaxStamina - (int) Math.Max(0.0f, Game1.player.Stamina)) * (double) num1), 24, (int) ((double) Game1.player.Stamina * (double) num1));
      int num2;
      if ((double) Game1.getOldMouseX() >= (double) position1.X && (double) Game1.getOldMouseY() >= (double) position1.Y)
      {
        num2 = (int) Math.Max(0.0f, Game1.player.Stamina);
        string str1 = num2.ToString();
        num2 = Game1.player.MaxStamina;
        string str2 = num2.ToString();
        Game1.drawWithBorder(str1 + "/" + str2, Microsoft.Xna.Framework.Color.Black * 0.0f, Microsoft.Xna.Framework.Color.White, position1 + new Vector2((float) (-(double) Game1.dialogueFont.MeasureString("999/999").X - 16.0 - (Game1.showingHealth ? 64.0 : 0.0)), 64f));
      }
      Microsoft.Xna.Framework.Color toGreenLerpColor1 = Utility.getRedToGreenLerpColor(Game1.player.stamina / (float) (int) (NetFieldBase<int, NetInt>) Game1.player.maxStamina);
      Game1.spriteBatch.Draw(Game1.staminaRect, destinationRectangle1, toGreenLerpColor1);
      destinationRectangle1.Height = 4;
      toGreenLerpColor1.R = (byte) Math.Max(0, (int) toGreenLerpColor1.R - 50);
      toGreenLerpColor1.G = (byte) Math.Max(0, (int) toGreenLerpColor1.G - 50);
      Game1.spriteBatch.Draw(Game1.staminaRect, destinationRectangle1, toGreenLerpColor1);
      if ((bool) (NetFieldBase<bool, NetBool>) Game1.player.exhausted)
      {
        Game1.spriteBatch.Draw(Game1.mouseCursors, position1 - new Vector2(0.0f, 11f) * 4f, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(191, 406, 12, 11)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        if ((double) Game1.getOldMouseX() >= (double) position1.X && (double) Game1.getOldMouseY() >= (double) position1.Y - 44.0)
          Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3747"), Microsoft.Xna.Framework.Color.Black * 0.0f, Microsoft.Xna.Framework.Color.White, position1 + new Vector2((float) (-(double) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3747")).X - 16.0 - (Game1.showingHealth ? 64.0 : 0.0)), 96f));
      }
      switch (Game1.currentLocation)
      {
        case MineShaft _:
        case Woods _:
        case SlimeHutch _:
        case VolcanoDungeon _:
label_12:
          Game1.showingHealthBar = true;
          Game1.showingHealth = true;
          int num3 = 168 + (Game1.player.maxHealth - 100);
          int height = (int) ((double) Game1.player.health / (double) Game1.player.maxHealth * (double) num3);
          position1.X -= (float) (56 + (Game1.hitShakeTimer > 0 ? Game1.random.Next(-3, 4) : 0));
          position1.Y = (float) (Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 224 - 16 - (Game1.player.maxHealth - 100));
          Game1.spriteBatch.Draw(Game1.mouseCursors, position1, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(268, 408, 12, 16)), Game1.player.health < 20 ? Microsoft.Xna.Framework.Color.Pink * (float) (Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / ((double) Game1.player.health * 50.0)) / 4.0 + 0.899999976158142) : Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          Game1.spriteBatch.Draw(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle((int) position1.X, (int) ((double) position1.Y + 64.0), 48, Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 64 - 16 - (int) ((double) position1.Y + 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(268, 424, 12, 16)), Game1.player.health < 20 ? Microsoft.Xna.Framework.Color.Pink * (float) (Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / ((double) Game1.player.health * 50.0)) / 4.0 + 0.899999976158142) : Microsoft.Xna.Framework.Color.White);
          Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(position1.X, (float) ((double) position1.Y + 224.0 + (double) (Game1.player.maxHealth - 100) - 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(268, 448, 12, 16)), Game1.player.health < 20 ? Microsoft.Xna.Framework.Color.Pink * (float) (Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / ((double) Game1.player.health * 50.0)) / 4.0 + 0.899999976158142) : Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          Microsoft.Xna.Framework.Rectangle destinationRectangle2 = new Microsoft.Xna.Framework.Rectangle((int) position1.X + 12, (int) position1.Y + 16 + 32 + num3 - height, 24, height);
          Microsoft.Xna.Framework.Color toGreenLerpColor2 = Utility.getRedToGreenLerpColor((float) Game1.player.health / (float) Game1.player.maxHealth);
          Game1.spriteBatch.Draw(Game1.staminaRect, destinationRectangle2, new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), toGreenLerpColor2, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);
          toGreenLerpColor2.R = (byte) Math.Max(0, (int) toGreenLerpColor2.R - 50);
          toGreenLerpColor2.G = (byte) Math.Max(0, (int) toGreenLerpColor2.G - 50);
          if ((double) Game1.getOldMouseX() >= (double) position1.X && (double) Game1.getOldMouseY() >= (double) position1.Y && (double) Game1.getOldMouseX() < (double) position1.X + 32.0)
          {
            num2 = Math.Max(0, Game1.player.health);
            Game1.drawWithBorder(num2.ToString() + "/" + Game1.player.maxHealth.ToString(), Microsoft.Xna.Framework.Color.Black * 0.0f, Microsoft.Xna.Framework.Color.Red, position1 + new Vector2((float) (-(double) Game1.dialogueFont.MeasureString("999/999").X - 32.0), 64f));
          }
          destinationRectangle2.Height = 4;
          Game1.spriteBatch.Draw(Game1.staminaRect, destinationRectangle2, new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), toGreenLerpColor2, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);
          break;
        default:
          if (Game1.player.health >= Game1.player.maxHealth)
          {
            Game1.showingHealth = false;
            break;
          }
          goto label_12;
      }
      Object activeObject = Game1.player.ActiveObject;
      foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
      {
        if (onScreenMenu != Game1.chatBox)
        {
          onScreenMenu.update(Game1.currentGameTime);
          onScreenMenu.draw(Game1.spriteBatch);
        }
      }
      if (!Game1.player.professions.Contains(17) || !Game1.currentLocation.IsOutdoors)
        return;
      foreach (KeyValuePair<Vector2, Object> pair in Game1.currentLocation.objects.Pairs)
      {
        if (((bool) (NetFieldBase<bool, NetBool>) pair.Value.isSpawnedObject || pair.Value.ParentSheetIndex == 590) && !Utility.isOnScreen(pair.Key * 64f + new Vector2(32f, 32f), 64))
        {
          Microsoft.Xna.Framework.Rectangle bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
          Vector2 renderPos = new Vector2();
          float rotation = 0.0f;
          if ((double) pair.Key.X * 64.0 > (double) (Game1.viewport.MaxCorner.X - 64))
          {
            renderPos.X = (float) (bounds.Right - 8);
            rotation = 1.570796f;
          }
          else if ((double) pair.Key.X * 64.0 < (double) Game1.viewport.X)
          {
            renderPos.X = 8f;
            rotation = -1.570796f;
          }
          else
            renderPos.X = pair.Key.X * 64f - (float) Game1.viewport.X;
          if ((double) pair.Key.Y * 64.0 > (double) (Game1.viewport.MaxCorner.Y - 64))
          {
            renderPos.Y = (float) (bounds.Bottom - 8);
            rotation = 3.141593f;
          }
          else
            renderPos.Y = (double) pair.Key.Y * 64.0 >= (double) Game1.viewport.Y ? pair.Key.Y * 64f - (float) Game1.viewport.Y : 8f;
          if ((double) renderPos.X == 8.0 && (double) renderPos.Y == 8.0)
            rotation += 0.7853982f;
          if ((double) renderPos.X == 8.0 && (double) renderPos.Y == (double) (bounds.Bottom - 8))
            rotation += 0.7853982f;
          if ((double) renderPos.X == (double) (bounds.Right - 8) && (double) renderPos.Y == 8.0)
            rotation -= 0.7853982f;
          if ((double) renderPos.X == (double) (bounds.Right - 8) && (double) renderPos.Y == (double) (bounds.Bottom - 8))
            rotation -= 0.7853982f;
          Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(412, 495, 5, 4);
          float scale = 4f;
          Vector2 renderSize = new Vector2((float) rectangle.Width * scale, (float) rectangle.Height * scale);
          Vector2 position2 = Utility.makeSafe(renderPos, renderSize);
          Game1.spriteBatch.Draw(Game1.mouseCursors, position2, new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, rotation, new Vector2(2f, 2f), scale, SpriteEffects.None, 1f);
        }
      }
      if (Game1.currentLocation.orePanPoint.Equals((object) Point.Zero) || Utility.isOnScreen(Utility.PointToVector2((Point) (NetFieldBase<Point, NetPoint>) Game1.currentLocation.orePanPoint) * 64f + new Vector2(32f, 32f), 64))
        return;
      Vector2 position3 = new Vector2();
      float rotation1 = 0.0f;
      Viewport viewport;
      if (Game1.currentLocation.orePanPoint.X * 64 > Game1.viewport.MaxCorner.X - 64)
      {
        ref Vector2 local = ref position3;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        double num4 = (double) (viewport.Bounds.Right - 8);
        local.X = (float) num4;
        rotation1 = 1.570796f;
      }
      else if (Game1.currentLocation.orePanPoint.X * 64 < Game1.viewport.X)
      {
        position3.X = 8f;
        rotation1 = -1.570796f;
      }
      else
        position3.X = (float) (Game1.currentLocation.orePanPoint.X * 64 - Game1.viewport.X);
      if (Game1.currentLocation.orePanPoint.Y * 64 > Game1.viewport.MaxCorner.Y - 64)
      {
        ref Vector2 local = ref position3;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        double num5 = (double) (viewport.Bounds.Bottom - 8);
        local.Y = (float) num5;
        rotation1 = 3.141593f;
      }
      else
        position3.Y = Game1.currentLocation.orePanPoint.Y * 64 >= Game1.viewport.Y ? (float) (Game1.currentLocation.orePanPoint.Y * 64 - Game1.viewport.Y) : 8f;
      if ((double) position3.X == 8.0 && (double) position3.Y == 8.0)
        rotation1 += 0.7853982f;
      if ((double) position3.X == 8.0)
      {
        double y = (double) position3.Y;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        double num6 = (double) (viewport.Bounds.Bottom - 8);
        if (y == num6)
          rotation1 += 0.7853982f;
      }
      double x1 = (double) position3.X;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      double num7 = (double) (viewport.Bounds.Right - 8);
      if (x1 == num7 && (double) position3.Y == 8.0)
        rotation1 -= 0.7853982f;
      double x2 = (double) position3.X;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      double num8 = (double) (viewport.Bounds.Right - 8);
      if (x2 == num8)
      {
        double y = (double) position3.Y;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        double num9 = (double) (viewport.Bounds.Bottom - 8);
        if (y == num9)
          rotation1 -= 0.7853982f;
      }
      Game1.spriteBatch.Draw(Game1.mouseCursors, position3, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(412, 495, 5, 4)), Microsoft.Xna.Framework.Color.Cyan, rotation1, new Vector2(2f, 2f), 4f, SpriteEffects.None, 1f);
    }

    public static void InvalidateOldMouseMovement()
    {
      MouseState mouseState = Game1.input.GetMouseState();
      Game1.oldMouseState = new MouseState(mouseState.X, mouseState.Y, Game1.oldMouseState.ScrollWheelValue, Game1.oldMouseState.LeftButton, Game1.oldMouseState.MiddleButton, Game1.oldMouseState.RightButton, Game1.oldMouseState.XButton1, Game1.oldMouseState.XButton2);
    }

    public static bool IsRenderingNonNativeUIScale() => (double) Game1.options.uiScale != (double) Game1.options.zoomLevel;

    public virtual void drawMouseCursor()
    {
      if (Game1.activeClickableMenu == null && Game1.timerUntilMouseFade > 0)
      {
        Game1.timerUntilMouseFade -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        Game1.lastMousePositionBeforeFade = Game1.getMousePosition();
      }
      if (Game1.options.gamepadControls && Game1.timerUntilMouseFade <= 0 && Game1.activeClickableMenu == null && (Game1.emoteMenu == null || Game1.emoteMenu.gamepadMode))
        Game1.mouseCursorTransparency = 0.0f;
      if (Game1.activeClickableMenu == null && Game1.mouseCursor > -1 && Game1.currentLocation != null)
      {
        if (Game1.IsRenderingNonNativeUIScale())
        {
          Game1.spriteBatch.End();
          Game1.PopUIMode();
          if (this.ShouldDrawOnBuffer())
            Game1.SetRenderTarget(this.screen);
          else
            Game1.SetRenderTarget((RenderTarget2D) null);
          Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        }
        if ((double) Game1.mouseCursorTransparency <= 0.0 || !Utility.canGrabSomethingFromHere(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y, Game1.player) || Game1.mouseCursor == 3)
        {
          if (Game1.player.ActiveObject != null && Game1.mouseCursor != 3 && !Game1.eventUp && Game1.currentMinigame == null && !Game1.player.isRidingHorse() && Game1.player.CanMove && Game1.displayFarmer)
          {
            if ((double) Game1.mouseCursorTransparency > 0.0 || Game1.options.showPlacementTileForGamepad)
            {
              Game1.player.ActiveObject.drawPlacementBounds(Game1.spriteBatch, Game1.currentLocation);
              if ((double) Game1.mouseCursorTransparency > 0.0)
              {
                Game1.spriteBatch.End();
                Game1.PushUIMode();
                Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                bool flag = Utility.playerCanPlaceItemHere(Game1.currentLocation, Game1.player.CurrentItem, Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, Game1.player) || Utility.isThereAnObjectHereWhichAcceptsThisItem(Game1.currentLocation, Game1.player.CurrentItem, Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y) && Utility.withinRadiusOfPlayer(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, 1, Game1.player);
                Game1.player.CurrentItem.drawInMenu(Game1.spriteBatch, new Vector2((float) (Game1.getMouseX() + 16), (float) (Game1.getMouseY() + 16)), flag ? (float) ((double) Game1.dialogueButtonScale / 75.0 + 1.0) : 1f, flag ? 1f : 0.5f, 0.999f);
                Game1.spriteBatch.End();
                Game1.PopUIMode();
                Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              }
            }
          }
          else if (Game1.mouseCursor == 0 && Game1.isActionAtCurrentCursorTile && Game1.currentMinigame == null)
            Game1.mouseCursor = Game1.isSpeechAtCurrentCursorTile ? 4 : (Game1.isInspectionAtCurrentCursorTile ? 5 : 2);
          else if ((double) Game1.mouseCursorTransparency > 0.0)
          {
            NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> netLongDictionary = (NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>) null;
            if (Game1.currentLocation is Farm)
              netLongDictionary = (Game1.currentLocation as Farm).animals;
            if (Game1.currentLocation is AnimalHouse)
              netLongDictionary = (Game1.currentLocation as AnimalHouse).animals;
            if (netLongDictionary != null)
            {
              Vector2 target = new Vector2((float) (Game1.getOldMouseX() + Game1.uiViewport.X), (float) (Game1.getOldMouseY() + Game1.uiViewport.Y));
              Game1.player.getGeneralDirectionTowards(target);
              bool flag = Utility.withinRadiusOfPlayer((int) target.X, (int) target.Y, 1, Game1.player);
              foreach (KeyValuePair<long, FarmAnimal> pair in netLongDictionary.Pairs)
              {
                Microsoft.Xna.Framework.Rectangle cursorPetBoundingBox = pair.Value.GetCursorPetBoundingBox();
                if (!(bool) (NetFieldBase<bool, NetBool>) pair.Value.wasPet && cursorPetBoundingBox.Contains((int) target.X, (int) target.Y))
                {
                  Game1.mouseCursor = 2;
                  if (!flag)
                  {
                    Game1.mouseCursorTransparency = 0.5f;
                    break;
                  }
                  break;
                }
              }
            }
          }
        }
        if (Game1.IsRenderingNonNativeUIScale())
        {
          Game1.spriteBatch.End();
          Game1.PushUIMode();
          Game1.SetRenderTarget(this.uiScreen);
          Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        }
        if (Game1.currentMinigame != null)
          Game1.mouseCursor = 0;
        if (!Game1.freezeControls && !Game1.options.hardwareCursor)
          Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float) Game1.getMouseX(), (float) Game1.getMouseY()), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.mouseCursor, 16, 16)), Microsoft.Xna.Framework.Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, (float) (4.0 + (double) Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
        Game1.wasMouseVisibleThisFrame = (double) Game1.mouseCursorTransparency > 0.0;
        this._lastDrewMouseCursor = Game1.wasMouseVisibleThisFrame;
      }
      Game1.mouseCursor = 0;
      if (Game1.isActionAtCurrentCursorTile || Game1.activeClickableMenu != null)
        return;
      Game1.mouseCursorTransparency = 1f;
    }

    public static float mouseCursorTransparency
    {
      get => Game1._mouseCursorTransparency;
      set => Game1._mouseCursorTransparency = value;
    }

    public static void panScreen(int x, int y)
    {
      int uiModeCount = Game1.uiModeCount;
      while (Game1.uiModeCount > 0)
        Game1.PopUIMode();
      Game1.previousViewportPosition.X = (float) Game1.viewport.Location.X;
      Game1.previousViewportPosition.Y = (float) Game1.viewport.Location.Y;
      Game1.viewport.X += x;
      Game1.viewport.Y += y;
      Game1.clampViewportToGameMap();
      Game1.updateRaindropPosition();
      for (int index = 0; index < uiModeCount; ++index)
        Game1.PushUIMode();
    }

    public static void clampViewportToGameMap()
    {
      if (Game1.viewport.X < 0)
        Game1.viewport.X = 0;
      if (Game1.viewport.X > Game1.currentLocation.map.DisplayWidth - Game1.viewport.Width)
        Game1.viewport.X = Game1.currentLocation.map.DisplayWidth - Game1.viewport.Width;
      if (Game1.viewport.Y < 0)
        Game1.viewport.Y = 0;
      if (Game1.viewport.Y <= Game1.currentLocation.map.DisplayHeight - Game1.viewport.Height)
        return;
      Game1.viewport.Y = Game1.currentLocation.map.DisplayHeight - Game1.viewport.Height;
    }

    public void drawBillboard()
    {
    }

    protected void drawDialogueBox()
    {
      if (Game1.currentSpeaker != null)
      {
        int height = Math.Max((int) Game1.dialogueFont.MeasureString(Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue()).Y, 320);
        Game1.drawDialogueBox((this.GraphicsDevice.Viewport.GetTitleSafeArea().Width - Math.Min(1280, this.GraphicsDevice.Viewport.GetTitleSafeArea().Width - 128)) / 2, this.GraphicsDevice.Viewport.GetTitleSafeArea().Height - height, Math.Min(1280, this.GraphicsDevice.Viewport.GetTitleSafeArea().Width - 128), height, true, false, objectDialogueWithPortrait: (Game1.objectDialoguePortraitPerson != null && Game1.currentSpeaker == null));
      }
      else
      {
        int count = Game1.currentObjectDialogue.Count;
      }
    }

    public static void drawDialogueBox(string message) => Game1.drawDialogueBox(Game1.viewport.Width / 2, Game1.viewport.Height / 2, false, false, message);

    public static void drawDialogueBox(
      int centerX,
      int centerY,
      bool speaker,
      bool drawOnlyBox,
      string message)
    {
      string text = (string) null;
      if (speaker && Game1.currentSpeaker != null)
        text = Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue();
      else if (message != null)
        text = message;
      else if (Game1.currentObjectDialogue.Count > 0)
        text = Game1.currentObjectDialogue.Peek();
      if (text == null)
        return;
      Vector2 vector2 = Game1.dialogueFont.MeasureString(text);
      int width = (int) vector2.X + 128;
      int height = (int) vector2.Y + 128;
      Game1.drawDialogueBox(centerX - width / 2, centerY - height / 2, width, height, speaker, drawOnlyBox, message, Game1.objectDialoguePortraitPerson != null && !speaker);
    }

    public static void DrawBox(int x, int y, int width, int height, Microsoft.Xna.Framework.Color? color = null)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64);
      rectangle.X = 64;
      rectangle.Y = 128;
      Texture2D texture = Game1.menuTexture;
      Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
      Microsoft.Xna.Framework.Color color1 = Microsoft.Xna.Framework.Color.White;
      if (color.HasValue)
      {
        white = color.Value;
        texture = Game1.uncoloredMenuTexture;
        color1 = new Microsoft.Xna.Framework.Color((int) Utility.Lerp((float) white.R, (float) Math.Min((int) byte.MaxValue, (int) white.R + 150), 0.65f), (int) Utility.Lerp((float) white.G, (float) Math.Min((int) byte.MaxValue, (int) white.G + 150), 0.65f), (int) Utility.Lerp((float) white.B, (float) Math.Min((int) byte.MaxValue, (int) white.B + 150), 0.65f));
      }
      Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x, y, width, height), new Microsoft.Xna.Framework.Rectangle?(rectangle), color1);
      rectangle.Y = 0;
      Vector2 vector2 = new Vector2((float) -rectangle.Width * 0.5f, (float) -rectangle.Height * 0.5f);
      rectangle.X = 0;
      Game1.spriteBatch.Draw(texture, new Vector2((float) x + vector2.X, (float) y + vector2.Y), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
      rectangle.X = 192;
      Game1.spriteBatch.Draw(texture, new Vector2((float) x + vector2.X + (float) width, (float) y + vector2.Y), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
      rectangle.Y = 192;
      Game1.spriteBatch.Draw(texture, new Vector2((float) (x + width) + vector2.X, (float) (y + height) + vector2.Y), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
      rectangle.X = 0;
      Game1.spriteBatch.Draw(texture, new Vector2((float) x + vector2.X, (float) (y + height) + vector2.Y), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
      rectangle.X = 128;
      rectangle.Y = 0;
      Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(64 + x + (int) vector2.X, y + (int) vector2.Y, width - 64, 64), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
      rectangle.Y = 192;
      Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(64 + x + (int) vector2.X, y + (int) vector2.Y + height, width - 64, 64), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
      rectangle.Y = 128;
      rectangle.X = 0;
      Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + (int) vector2.X, y + (int) vector2.Y + 64, 64, height - 64), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
      rectangle.X = 192;
      Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + width + (int) vector2.X, y + (int) vector2.Y + 64, 64, height - 64), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
    }

    public static void drawDialogueBox(
      int x,
      int y,
      int width,
      int height,
      bool speaker,
      bool drawOnlyBox,
      string message = null,
      bool objectDialogueWithPortrait = false,
      bool ignoreTitleSafe = true,
      int r = -1,
      int g = -1,
      int b = -1)
    {
      if (!drawOnlyBox)
        return;
      Microsoft.Xna.Framework.Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
      int height1 = titleSafeArea.Height;
      int width1 = titleSafeArea.Width;
      int dialogueX = 0;
      int num1 = 0;
      if (!ignoreTitleSafe)
        num1 = y > titleSafeArea.Y ? 0 : titleSafeArea.Y - y;
      int num2 = 0;
      width = Math.Min(titleSafeArea.Width, width);
      if (!Game1.isQuestion && Game1.currentSpeaker == null && Game1.currentObjectDialogue.Count > 0 && !drawOnlyBox)
      {
        width = (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).X + 128;
        height = (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y + 64;
        x = width1 / 2 - width / 2;
        num2 = height > 256 ? -(height - 256) : 0;
      }
      Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64);
      int addedTileHeightForQuestions = -1;
      if (Game1.questionChoices.Count >= 3)
        addedTileHeightForQuestions = Game1.questionChoices.Count - 3;
      if (!drawOnlyBox && Game1.currentObjectDialogue.Count > 0)
      {
        if ((double) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y >= (double) (height - 128))
        {
          addedTileHeightForQuestions -= (int) (((double) (height - 128) - (double) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y) / 64.0) - 1;
        }
        else
        {
          height += (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y / 2;
          num2 -= (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y / 2;
          if ((int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y / 2 > 64)
            addedTileHeightForQuestions = 0;
        }
      }
      if (Game1.currentSpeaker != null && Game1.isQuestion && Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue().Substring(0, Game1.currentDialogueCharacterIndex).Contains(Environment.NewLine))
        ++addedTileHeightForQuestions;
      rectangle1.Width = 64;
      rectangle1.Height = 64;
      rectangle1.X = 64;
      rectangle1.Y = 128;
      Microsoft.Xna.Framework.Color color = r == -1 ? Microsoft.Xna.Framework.Color.White : new Microsoft.Xna.Framework.Color(r, g, b);
      Texture2D texture = r == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
      Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(28 + x + dialogueX, 28 + y - 64 * addedTileHeightForQuestions + num1 + num2, width - 64, height - 64 + addedTileHeightForQuestions * 64), new Microsoft.Xna.Framework.Rectangle?(rectangle1), r == -1 ? color : new Microsoft.Xna.Framework.Color((int) Utility.Lerp((float) r, (float) Math.Min((int) byte.MaxValue, r + 150), 0.65f), (int) Utility.Lerp((float) g, (float) Math.Min((int) byte.MaxValue, g + 150), 0.65f), (int) Utility.Lerp((float) b, (float) Math.Min((int) byte.MaxValue, b + 150), 0.65f)));
      rectangle1.Y = 0;
      rectangle1.X = 0;
      Game1.spriteBatch.Draw(texture, new Vector2((float) (x + dialogueX), (float) (y - 64 * addedTileHeightForQuestions + num1 + num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
      rectangle1.X = 192;
      Game1.spriteBatch.Draw(texture, new Vector2((float) (x + width + dialogueX - 64), (float) (y - 64 * addedTileHeightForQuestions + num1 + num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
      rectangle1.Y = 192;
      Game1.spriteBatch.Draw(texture, new Vector2((float) (x + width + dialogueX - 64), (float) (y + height + num1 - 64 + num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
      rectangle1.X = 0;
      Game1.spriteBatch.Draw(texture, new Vector2((float) (x + dialogueX), (float) (y + height + num1 - 64 + num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
      rectangle1.X = 128;
      rectangle1.Y = 0;
      Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(64 + x + dialogueX, y - 64 * addedTileHeightForQuestions + num1 + num2, width - 128, 64), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
      rectangle1.Y = 192;
      Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(64 + x + dialogueX, y + height + num1 - 64 + num2, width - 128, 64), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
      rectangle1.Y = 128;
      rectangle1.X = 0;
      Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + dialogueX, y - 64 * addedTileHeightForQuestions + num1 + 64 + num2, 64, height - 128 + addedTileHeightForQuestions * 64), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
      rectangle1.X = 192;
      Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + width + dialogueX - 64, y - 64 * addedTileHeightForQuestions + num1 + 64 + num2, 64, height - 128 + addedTileHeightForQuestions * 64), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
      if (objectDialogueWithPortrait && Game1.objectDialoguePortraitPerson != null || speaker && Game1.currentSpeaker != null && Game1.currentSpeaker.CurrentDialogue.Count > 0 && Game1.currentSpeaker.CurrentDialogue.Peek().showPortrait)
      {
        Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64);
        NPC npc = objectDialogueWithPortrait ? Game1.objectDialoguePortraitPerson : Game1.currentSpeaker;
        switch (objectDialogueWithPortrait ? (Game1.objectDialoguePortraitPerson.Name.Equals(Game1.player.spouse) ? "$l" : "$neutral") : npc.CurrentDialogue.Peek().CurrentEmotion)
        {
          case "$a":
            rectangle2 = new Microsoft.Xna.Framework.Rectangle(64, 128, 64, 64);
            break;
          case "$h":
            rectangle2 = new Microsoft.Xna.Framework.Rectangle(64, 0, 64, 64);
            break;
          case "$k":
          case "$neutral":
            rectangle2 = new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64);
            break;
          case "$l":
            rectangle2 = new Microsoft.Xna.Framework.Rectangle(0, 128, 64, 64);
            break;
          case "$s":
            rectangle2 = new Microsoft.Xna.Framework.Rectangle(0, 64, 64, 64);
            break;
          case "$u":
            rectangle2 = new Microsoft.Xna.Framework.Rectangle(64, 64, 64, 64);
            break;
          default:
            rectangle2 = Game1.getSourceRectForStandardTileSheet(npc.Portrait, Convert.ToInt32(npc.CurrentDialogue.Peek().CurrentEmotion.Substring(1)));
            break;
        }
        Game1.spriteBatch.End();
        Game1.spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
        if (npc.Portrait != null)
        {
          Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float) (dialogueX + x + 768), (float) (height1 - 320 - 64 * addedTileHeightForQuestions - 256 + num1 + 16 - 60 + num2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(333, 305, 80, 87)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.98f);
          Game1.spriteBatch.Draw(npc.Portrait, new Vector2((float) (dialogueX + x + 768 + 32), (float) (height1 - 320 - 64 * addedTileHeightForQuestions - 256 + num1 + 16 - 60 + num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle2), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        }
        Game1.spriteBatch.End();
        Game1.spriteBatch.Begin();
        if (Game1.isQuestion)
          Game1.spriteBatch.DrawString(Game1.dialogueFont, npc.displayName, new Vector2((float) (928.0 - (double) Game1.dialogueFont.MeasureString(npc.displayName).X / 2.0) + (float) dialogueX + (float) x, (float) ((double) (height1 - 320 - 64 * addedTileHeightForQuestions) - (double) Game1.dialogueFont.MeasureString(npc.displayName).Y + (double) num1 + 21.0) + (float) num2) + new Vector2(2f, 2f), new Microsoft.Xna.Framework.Color(150, 150, 150));
        Game1.spriteBatch.DrawString(Game1.dialogueFont, npc.Name.Equals("DwarfKing") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3754") : (npc.Name.Equals("Lewis") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3756") : npc.displayName), new Vector2((float) (dialogueX + x + 896 + 32) - Game1.dialogueFont.MeasureString(npc.Name.Equals("Lewis") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3756") : npc.displayName).X / 2f, (float) ((double) (height1 - 320 - 64 * addedTileHeightForQuestions) - (double) Game1.dialogueFont.MeasureString(npc.Name.Equals("Lewis") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3756") : npc.displayName).Y + (double) num1 + 21.0 + 8.0) + (float) num2), Game1.textColor);
      }
      if (drawOnlyBox || Game1.nameSelectUp && (!Game1.messagePause || Game1.currentObjectDialogue == null))
        return;
      string text = "";
      if (Game1.currentSpeaker != null && Game1.currentSpeaker.CurrentDialogue.Count > 0)
      {
        if (Game1.currentSpeaker.CurrentDialogue.Peek() == null || Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue().Length < Game1.currentDialogueCharacterIndex - 1)
        {
          Game1.dialogueUp = false;
          Game1.currentDialogueCharacterIndex = 0;
          Game1.playSound("dialogueCharacterClose");
          Game1.player.forceCanMove();
          return;
        }
        text = Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue().Substring(0, Game1.currentDialogueCharacterIndex);
      }
      else if (message != null)
        text = message;
      else if (Game1.currentObjectDialogue.Count > 0)
        text = Game1.currentObjectDialogue.Peek().Length <= 1 ? "" : Game1.currentObjectDialogue.Peek().Substring(0, Game1.currentDialogueCharacterIndex);
      Vector2 position = (double) Game1.dialogueFont.MeasureString(text).X <= (double) (width1 - 256 - dialogueX) ? (Game1.currentSpeaker == null || Game1.currentSpeaker.CurrentDialogue.Count <= 0 ? (message == null ? (!Game1.isQuestion ? new Vector2((float) (width1 / 2) - Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Count == 0 ? "" : Game1.currentObjectDialogue.Peek()).X / 2f + (float) dialogueX, (float) (y + 4 + num2)) : new Vector2((float) (width1 / 2) - Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Count == 0 ? "" : Game1.currentObjectDialogue.Peek()).X / 2f + (float) dialogueX, (float) (height1 - 64 * addedTileHeightForQuestions - 256 - (16 + (Game1.questionChoices.Count - 2) * 64) + num1 + num2))) : new Vector2((float) (width1 / 2) - Game1.dialogueFont.MeasureString(text).X / 2f + (float) dialogueX, (float) (y + 96 + 4))) : new Vector2((float) (width1 / 2) - Game1.dialogueFont.MeasureString(Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue()).X / 2f + (float) dialogueX, (float) (height1 - 64 * addedTileHeightForQuestions - 256 - 16 + num1 + num2))) : new Vector2((float) (128 + dialogueX), (float) (height1 - 64 * addedTileHeightForQuestions - 256 - 16 + num1 + num2));
      if (!drawOnlyBox)
      {
        Game1.spriteBatch.DrawString(Game1.dialogueFont, text, position + new Vector2(3f, 0.0f), Game1.textShadowColor);
        Game1.spriteBatch.DrawString(Game1.dialogueFont, text, position + new Vector2(3f, 3f), Game1.textShadowColor);
        Game1.spriteBatch.DrawString(Game1.dialogueFont, text, position + new Vector2(0.0f, 3f), Game1.textShadowColor);
        Game1.spriteBatch.DrawString(Game1.dialogueFont, text, position, Game1.textColor);
      }
      if ((double) Game1.dialogueFont.MeasureString(text).Y <= 64.0)
        num1 += 64;
      if (Game1.isQuestion && !Game1.dialogueTyping)
      {
        for (int index = 0; index < Game1.questionChoices.Count; ++index)
        {
          if (Game1.currentQuestionChoice == index)
          {
            position.X = (float) (80 + dialogueX + x);
            position.Y = (float) ((double) (height1 - (5 + addedTileHeightForQuestions + 1) * 64) + (text.Trim().Length > 0 ? (double) Game1.dialogueFont.MeasureString(text).Y : 0.0) + 128.0) + (float) (48 * index) - (float) (16 + (Game1.questionChoices.Count - 2) * 64) + (float) num1 + (float) num2;
            Game1.spriteBatch.End();
            Game1.spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
            Game1.spriteBatch.Draw(Game1.objectSpriteSheet, position + new Vector2((float) Math.Cos((double) Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) * 3f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(26)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            Game1.spriteBatch.End();
            Game1.spriteBatch.Begin();
            position.X = (float) (160 + dialogueX + x);
            position.Y = (float) ((double) (height1 - (5 + addedTileHeightForQuestions + 1) * 64) + (text.Trim().Length > 1 ? (double) Game1.dialogueFont.MeasureString(text).Y : 0.0) + 128.0) - (float) ((Game1.questionChoices.Count - 2) * 64) + (float) (48 * index) + (float) num1 + (float) num2;
            Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.questionChoices[index].responseText, position, Game1.textColor);
          }
          else
          {
            position.X = (float) (128 + dialogueX + x);
            position.Y = (float) ((double) (height1 - (5 + addedTileHeightForQuestions + 1) * 64) + (text.Trim().Length > 1 ? (double) Game1.dialogueFont.MeasureString(text).Y : 0.0) + 128.0) - (float) ((Game1.questionChoices.Count - 2) * 64) + (float) (48 * index) + (float) num1 + (float) num2;
            Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.questionChoices[index].responseText, position, Game1.unselectedOptionColor);
          }
        }
      }
      else if (Game1.numberOfSelectedItems != -1 && !Game1.dialogueTyping)
        Game1.drawItemSelectDialogue(x, y, dialogueX, num1 + num2, height1, addedTileHeightForQuestions, text);
      if (drawOnlyBox || Game1.dialogueTyping || message != null)
        return;
      Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float) (x + dialogueX + width - 96), (float) (y + height + num1 + num2 - 96) - Game1.dialogueButtonScale), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.dialogueButtonShrinking || (double) Game1.dialogueButtonScale >= 8.0 ? 2 : 3)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999999f);
    }

    private static void drawItemSelectDialogue(
      int x,
      int y,
      int dialogueX,
      int dialogueY,
      int screenHeight,
      int addedTileHeightForQuestions,
      string text)
    {
      string selectedItemsType = Game1.selectedItemsType;
      string text1;
      if (!(selectedItemsType == "flutePitch") && !(selectedItemsType == "drumTome"))
      {
        if (selectedItemsType == "jukebox")
          text1 = "@ " + Game1.player.songsHeard.ElementAt<string>(Game1.numberOfSelectedItems) + " >  ";
        else
          text1 = "@ " + Game1.numberOfSelectedItems.ToString() + " >  " + (Game1.priceOfSelectedItem * Game1.numberOfSelectedItems).ToString() + "g";
      }
      else
        text1 = "@ " + Game1.numberOfSelectedItems.ToString() + " >  ";
      if (Game1.currentLocation.Name.Equals("Club"))
        text1 = "@ " + Game1.numberOfSelectedItems.ToString() + " >  ";
      Game1.spriteBatch.DrawString(Game1.dialogueFont, text1, new Vector2((float) (dialogueX + x + 64), (float) ((double) (screenHeight - (5 + addedTileHeightForQuestions + 1) * 64) + (double) Game1.dialogueFont.MeasureString(text).Y + 104.0) + (float) dialogueY), Game1.textColor);
    }

    protected void drawFarmBuildings()
    {
      int coopUpgradeLevel = Game1.player.CoopUpgradeLevel;
      switch (Game1.player.BarnUpgradeLevel)
      {
        case 1:
          Game1.spriteBatch.Draw(Game1.currentBarnTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(768f, 320f)), new Microsoft.Xna.Framework.Rectangle?(Game1.currentBarnTexture.Bounds), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, Math.Max(0.0f, 0.0576f));
          break;
        case 2:
          Game1.spriteBatch.Draw(Game1.currentBarnTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(640f, 256f)), new Microsoft.Xna.Framework.Rectangle?(Game1.currentBarnTexture.Bounds), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, Math.Max(0.0f, 0.0576f));
          break;
      }
      if (!Game1.player.hasGreenhouse)
        return;
      Game1.spriteBatch.Draw(Game1.greenhouseTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(64f, 320f)), new Microsoft.Xna.Framework.Rectangle?(Game1.greenhouseTexture.Bounds), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, Math.Max(0.0f, 0.0576f));
    }

    public static void drawPlayerHeldObject(Farmer f)
    {
      if (Game1.eventUp && (Game1.currentLocation.currentEvent == null || !Game1.currentLocation.currentEvent.showActiveObject) || f.FarmerSprite.PauseForSingleAnimation || f.isRidingHorse() || (bool) (NetFieldBase<bool, NetBool>) f.bathingClothes || f.onBridge.Value)
        return;
      float x = f.getLocalPosition(Game1.viewport).X + ((double) f.rotation < 0.0 ? -8f : ((double) f.rotation > 0.0 ? 8f : 0.0f)) + (float) (f.FarmerSprite.CurrentAnimationFrame.xOffset * 4);
      float y = f.getLocalPosition(Game1.viewport).Y - 128f + (float) (f.FarmerSprite.CurrentAnimationFrame.positionOffset * 4) + (float) (FarmerRenderer.featureYOffsetPerFrame[f.FarmerSprite.CurrentFrame] * 4);
      if ((bool) (NetFieldBase<bool, NetBool>) f.ActiveObject.bigCraftable)
        y -= 64f;
      if (f.isEating)
      {
        x = f.getLocalPosition(Game1.viewport).X - 21f;
        y = (float) ((double) f.getLocalPosition(Game1.viewport).Y - 128.0 + 12.0);
      }
      if (f.isEating && (!f.isEating || f.Sprite.currentFrame > 218))
        return;
      f.ActiveObject.drawWhenHeld(Game1.spriteBatch, new Vector2((float) (int) x, (float) (int) y), f);
    }

    public static void drawTool(Farmer f) => Game1.drawTool(f, f.CurrentTool.CurrentParentTileIndex);

    public static void drawTool(Farmer f, int currentToolIndex)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(currentToolIndex * 16 % Game1.toolSpriteSheet.Width, currentToolIndex * 16 / Game1.toolSpriteSheet.Width * 16, 16, 32);
      Vector2 playerPosition = f.getLocalPosition(Game1.viewport) + f.jitter + f.armOffset;
      float num1 = 0.0f;
      if (f.FacingDirection == 0)
        num1 = -1f / 500f;
      if (Game1.pickingTool)
      {
        int y = (int) playerPosition.Y - 128;
        Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2(playerPosition.X, (float) y), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 32) / 10000f));
      }
      else if (f.CurrentTool is MeleeWeapon)
        ((MeleeWeapon) f.CurrentTool).drawDuringUse(f.Sprite.currentAnimationIndex, f.FacingDirection, Game1.spriteBatch, playerPosition, f);
      else if (f.FarmerSprite.isUsingWeapon())
      {
        MeleeWeapon.drawDuringUse(f.Sprite.currentAnimationIndex, f.FacingDirection, Game1.spriteBatch, playerPosition, f, MeleeWeapon.getSourceRect(f.FarmerSprite.CurrentToolIndex), f.FarmerSprite.getWeaponTypeFromAnimation(), false);
      }
      else
      {
        if (f.CurrentTool is FishingRod)
        {
          if ((f.CurrentTool as FishingRod).fishCaught || (f.CurrentTool as FishingRod).showingTreasure)
          {
            f.CurrentTool.draw(Game1.spriteBatch);
            return;
          }
          rectangle = new Microsoft.Xna.Framework.Rectangle(f.Sprite.currentAnimationIndex * 48, 288, 48, 48);
          if (f.FacingDirection == 2 || f.FacingDirection == 0)
            rectangle.Y += 48;
          else if ((f.CurrentTool as FishingRod).isFishing && (!(f.CurrentTool as FishingRod).isReeling || (f.CurrentTool as FishingRod).hit))
            playerPosition.Y += 8f;
          if ((f.CurrentTool as FishingRod).isFishing)
            rectangle.X += (5 - f.Sprite.currentAnimationIndex) * 48;
          if ((f.CurrentTool as FishingRod).isReeling)
          {
            if (f.FacingDirection == 2 || f.FacingDirection == 0)
            {
              rectangle.X = 288;
              if (f.IsLocalPlayer && Game1.didPlayerJustClickAtAll())
                rectangle.X = 0;
            }
            else
            {
              rectangle.X = 288;
              rectangle.Y = 240;
              if (f.IsLocalPlayer && Game1.didPlayerJustClickAtAll())
                rectangle.Y += 48;
            }
          }
          if (f.FarmerSprite.CurrentFrame == 57)
            rectangle.Height = 0;
          if (f.FacingDirection == 0)
            playerPosition.X += 16f;
        }
        if (f.CurrentTool != null)
          f.CurrentTool.draw(Game1.spriteBatch);
        if (f.CurrentTool is Slingshot || f.CurrentTool is Shears || f.CurrentTool is MilkPail || f.CurrentTool is Pan)
          return;
        int num2 = 0;
        int num3 = 0;
        if (f.CurrentTool is WateringCan)
        {
          num2 += 80;
          num3 = f.FacingDirection == 1 ? 32 : (f.FacingDirection == 3 ? -32 : 0);
          if (f.Sprite.currentAnimationIndex == 0 || f.Sprite.currentAnimationIndex == 1)
            num3 = num3 * 3 / 2;
        }
        if (f.FacingDirection == 1)
        {
          int num4 = 0;
          if (f.Sprite.currentAnimationIndex > 2)
          {
            Point tileLocationPoint = f.getTileLocationPoint();
            ++tileLocationPoint.X;
            --tileLocationPoint.Y;
            if (!(f.CurrentTool is WateringCan) && f.currentLocation.getTileIndexAt(tileLocationPoint, "Front") != -1)
              return;
            ++tileLocationPoint.Y;
            if (f.currentLocation.getTileIndexAt(tileLocationPoint, "Front") == -1)
              num4 += 16;
          }
          else if (f.CurrentTool is WateringCan && f.Sprite.currentAnimationIndex == 1)
          {
            Point tileLocationPoint = f.getTileLocationPoint();
            --tileLocationPoint.X;
            --tileLocationPoint.Y;
            if (f.currentLocation.getTileIndexAt(tileLocationPoint, "Front") != -1 && (double) f.Position.Y % 64.0 < 32.0)
              return;
          }
          if (f.CurrentTool != null && f.CurrentTool is FishingRod)
          {
            Microsoft.Xna.Framework.Color color = (f.CurrentTool as FishingRod).getColor();
            switch (f.Sprite.currentAnimationIndex)
            {
              case 0:
                if ((f.CurrentTool as FishingRod).isReeling)
                {
                  Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2(playerPosition.X - 64f + (float) num3, playerPosition.Y - 160f + (float) num2), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                  break;
                }
                if ((f.CurrentTool as FishingRod).isFishing || (f.CurrentTool as FishingRod).doneWithAnimation)
                {
                  Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2(playerPosition.X - 64f + (float) num3, playerPosition.Y - 160f + (float) num2), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                  break;
                }
                if ((f.CurrentTool as FishingRod).hasDoneFucntionYet && !(f.CurrentTool as FishingRod).pullingOutOfWater)
                  break;
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2(playerPosition.X - 64f + (float) num3, playerPosition.Y - 160f + (float) num2), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 1:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2(playerPosition.X - 64f + (float) num3, (float) ((double) playerPosition.Y - 160.0 + 8.0) + (float) num2), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 2:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2((float) ((double) playerPosition.X - 96.0 + 32.0) + (float) num3, (float) ((double) playerPosition.Y - 128.0 - 24.0) + (float) num2), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 3:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2((float) ((double) playerPosition.X - 96.0 + 24.0) + (float) num3, (float) ((double) playerPosition.Y - 128.0 - 32.0) + (float) num2), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 4:
                if ((f.CurrentTool as FishingRod).isFishing || (f.CurrentTool as FishingRod).doneWithAnimation)
                {
                  Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2(playerPosition.X - 64f + (float) num3, playerPosition.Y - 160f + (float) num2), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                  break;
                }
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2(playerPosition.X - 64f + (float) num3, (float) ((double) playerPosition.Y - 160.0 + 4.0) + (float) num2), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 5:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2(playerPosition.X - 64f + (float) num3, playerPosition.Y - 160f + (float) num2), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
            }
          }
          else if (f.CurrentTool != null && f.CurrentTool.Name.Contains("Sword"))
          {
            switch (f.Sprite.currentAnimationIndex)
            {
              case 0:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 64.0 - 20.0), playerPosition.Y + 28f)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, -0.3926991f, new Vector2(4f, 60f), 1f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 1:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 64.0 - 12.0), (float) ((double) playerPosition.Y + 64.0 - 8.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(4f, 60f), 1f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 2:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 64.0 - 12.0), (float) ((double) playerPosition.Y + 64.0 - 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.3926991f, new Vector2(4f, 60f), 1f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 3:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 64.0 - 12.0), playerPosition.Y + 64f)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.7853981f, new Vector2(4f, 60f), 1f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 4:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 64.0 - 16.0), (float) ((double) playerPosition.Y + 64.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 3f * (float) Math.PI / 8f, new Vector2(4f, 60f), 1f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 5:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 64.0 - 16.0), (float) ((double) playerPosition.Y + 64.0 + 8.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 1.570796f, new Vector2(4f, 60f), 1f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 6:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 64.0 - 16.0), (float) ((double) playerPosition.Y + 64.0 + 12.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 1.963495f, new Vector2(4f, 60f), 1f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 7:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 64.0 - 16.0), (float) ((double) playerPosition.Y + 64.0 + 12.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 1.963495f, new Vector2(4f, 60f), 1f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
            }
          }
          else if (f.CurrentTool != null && f.CurrentTool is WateringCan)
          {
            switch (f.Sprite.currentAnimationIndex)
            {
              case 0:
              case 1:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2((float) (int) ((double) playerPosition.X + (double) num3 - 4.0), (float) (int) ((double) playerPosition.Y - 128.0 + 8.0 + (double) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num4) / 10000f));
                break;
              case 2:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2((float) ((int) playerPosition.X + num3 + 24), (float) (int) ((double) playerPosition.Y - 128.0 - 8.0 + (double) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.2617994f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num4) / 10000f));
                break;
              case 3:
                rectangle.X += 16;
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, new Vector2((float) (int) ((double) playerPosition.X + (double) num3 + 8.0), (float) (int) ((double) playerPosition.Y - 128.0 - 24.0 + (double) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num4) / 10000f));
                break;
            }
          }
          else
          {
            switch (f.Sprite.currentAnimationIndex)
            {
              case 0:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 32.0 - 4.0) + (float) num3 - (float) Math.Min(8, f.toolPower * 4), (float) ((double) playerPosition.Y - 128.0 + 24.0) + (float) num2 + (float) Math.Min(8, f.toolPower * 4))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, (float) (-0.261799395084381 - (double) Math.Min(f.toolPower, 2) * 0.0490873865783215), new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num4) / 10000f));
                break;
              case 1:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 32.0 - 24.0) + (float) num3, (float) ((double) playerPosition.Y - 124.0 + (double) num2 + 64.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.2617994f, new Vector2(0.0f, 32f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num4) / 10000f));
                break;
              case 2:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 32.0 + (double) num3 - 4.0), (float) ((double) playerPosition.Y - 132.0 + (double) num2 + 64.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.7853982f, new Vector2(0.0f, 32f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num4) / 10000f));
                break;
              case 3:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 32.0 + 28.0) + (float) num3, playerPosition.Y - 64f + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 1.832596f, new Vector2(0.0f, 32f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num4) / 10000f));
                break;
              case 4:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 32.0 + 28.0) + (float) num3, (float) ((double) playerPosition.Y - 64.0 + 4.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 1.832596f, new Vector2(0.0f, 32f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num4) / 10000f));
                break;
              case 5:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 64.0 + 12.0) + (float) num3, (float) ((double) playerPosition.Y - 128.0 + 32.0 + (double) num2 + 128.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.7853982f, new Vector2(0.0f, 32f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num4) / 10000f));
                break;
              case 6:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 42.0 + 8.0) + (float) num3, (float) ((double) playerPosition.Y - 64.0 + 24.0 + (double) num2 + 128.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 128f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num4) / 10000f));
                break;
            }
          }
        }
        else if (f.FacingDirection == 3)
        {
          int num5 = 0;
          if (f.Sprite.currentAnimationIndex > 2)
          {
            Point tileLocationPoint = f.getTileLocationPoint();
            --tileLocationPoint.X;
            --tileLocationPoint.Y;
            if (!(f.CurrentTool is WateringCan) && f.currentLocation.getTileIndexAt(tileLocationPoint, "Front") != -1 && (double) f.Position.Y % 64.0 < 32.0)
              return;
            ++tileLocationPoint.Y;
            if (f.currentLocation.getTileIndexAt(tileLocationPoint, "Front") == -1)
              num5 += 16;
          }
          else if (f.CurrentTool is WateringCan && f.Sprite.currentAnimationIndex == 1)
          {
            Point tileLocationPoint = f.getTileLocationPoint();
            --tileLocationPoint.X;
            --tileLocationPoint.Y;
            if (f.currentLocation.getTileIndexAt(tileLocationPoint, "Front") != -1 && (double) f.Position.Y % 64.0 < 32.0)
              return;
          }
          if (f.CurrentTool != null && f.CurrentTool is FishingRod)
          {
            Microsoft.Xna.Framework.Color color = (f.CurrentTool as FishingRod).getColor();
            switch (f.Sprite.currentAnimationIndex)
            {
              case 0:
                if ((f.CurrentTool as FishingRod).isReeling)
                {
                  Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num3, playerPosition.Y - 160f + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                  break;
                }
                if ((f.CurrentTool as FishingRod).isFishing || (f.CurrentTool as FishingRod).doneWithAnimation)
                {
                  Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num3, playerPosition.Y - 160f + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                  break;
                }
                if ((f.CurrentTool as FishingRod).hasDoneFucntionYet && !(f.CurrentTool as FishingRod).pullingOutOfWater)
                  break;
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num3, playerPosition.Y - 160f + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 1:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num3, (float) ((double) playerPosition.Y - 160.0 + 8.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 2:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 96.0 + 32.0) + (float) num3, (float) ((double) playerPosition.Y - 128.0 - 24.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 3:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 96.0 + 24.0) + (float) num3, (float) ((double) playerPosition.Y - 128.0 - 32.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 4:
                if ((f.CurrentTool as FishingRod).isFishing || (f.CurrentTool as FishingRod).doneWithAnimation)
                {
                  Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num3, playerPosition.Y - 160f + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                  break;
                }
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num3, (float) ((double) playerPosition.Y - 160.0 + 4.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
              case 5:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num3, playerPosition.Y - 160f + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 64) / 10000f));
                break;
            }
          }
          else if (f.CurrentTool != null && f.CurrentTool is WateringCan)
          {
            switch (f.Sprite.currentAnimationIndex)
            {
              case 0:
              case 1:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num3 - 4.0), (float) ((double) playerPosition.Y - 128.0 + 8.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num5) / 10000f));
                break;
              case 2:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num3 - 16.0), playerPosition.Y - 128f + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, -0.2617994f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num5) / 10000f));
                break;
              case 3:
                rectangle.X += 16;
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num3 - 16.0), (float) ((double) playerPosition.Y - 128.0 - 24.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num5) / 10000f));
                break;
            }
          }
          else
          {
            switch (f.Sprite.currentAnimationIndex)
            {
              case 0:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 32.0 + 8.0) + (float) num3 + (float) Math.Min(8, f.toolPower * 4), (float) ((double) playerPosition.Y - 128.0 + 8.0) + (float) num2 + (float) Math.Min(8, f.toolPower * 4))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, (float) (0.261799395084381 + (double) Math.Min(f.toolPower, 2) * 0.0490873865783215), new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num5) / 10000f));
                break;
              case 1:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 16f + (float) num3, (float) ((double) playerPosition.Y - 128.0 + 16.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, -0.2617994f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num5) / 10000f));
                break;
              case 2:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 64.0 + 4.0) + (float) num3, (float) ((double) playerPosition.Y - 128.0 + 60.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, -0.7853982f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num5) / 10000f));
                break;
              case 3:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 64.0 + 20.0) + (float) num3, (float) ((double) playerPosition.Y - 64.0 + 76.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, -1.832596f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num5) / 10000f));
                break;
              case 4:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 64.0 + 24.0) + (float) num3, playerPosition.Y + 24f + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, -1.832596f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, Math.Max(0.0f, num1 + (float) (f.GetBoundingBox().Bottom + num5) / 10000f));
                break;
            }
          }
        }
        else
        {
          if (f.CurrentTool is MeleeWeapon && f.FacingDirection == 0)
            return;
          if (f.Sprite.currentAnimationIndex > 2 && (!(f.CurrentTool is FishingRod) || (f.CurrentTool as FishingRod).isCasting || (f.CurrentTool as FishingRod).castedButBobberStillInAir || (f.CurrentTool as FishingRod).isTimingCast))
          {
            Point tileLocationPoint = f.getTileLocationPoint();
            if (f.currentLocation.getTileIndexAt(tileLocationPoint, "Front") != -1 && (double) f.Position.Y % 64.0 < 32.0 && (double) f.Position.Y % 64.0 > 16.0)
              return;
          }
          else if (f.CurrentTool is FishingRod && f.Sprite.currentAnimationIndex <= 2)
          {
            Point tileLocationPoint = f.getTileLocationPoint();
            --tileLocationPoint.Y;
            if (f.currentLocation.getTileIndexAt(tileLocationPoint, "Front") != -1)
              return;
          }
          if (f.CurrentTool != null && f.CurrentTool is FishingRod || currentToolIndex >= 48 && currentToolIndex <= 55 && !(f.CurrentTool as FishingRod).fishCaught)
          {
            Microsoft.Xna.Framework.Color color = (f.CurrentTool as FishingRod).getColor();
            switch (f.Sprite.currentAnimationIndex)
            {
              case 0:
                if ((f.CurrentTool as FishingRod).showingTreasure || (f.CurrentTool as FishingRod).fishCaught || f.FacingDirection == 0 && (f.CurrentTool as FishingRod).isFishing && !(f.CurrentTool as FishingRod).isReeling)
                  break;
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + (f.FacingDirection == 0 ? 0 : 128)) / 10000f));
                break;
              case 1:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + (f.FacingDirection == 0 ? 0 : 128)) / 10000f));
                break;
              case 2:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + (f.FacingDirection == 0 ? 0 : 128)) / 10000f));
                break;
              case 3:
                if (f.FacingDirection != 2)
                  break;
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + (f.FacingDirection == 0 ? 0 : 128)) / 10000f));
                break;
              case 4:
                if (f.FacingDirection == 0 && (f.CurrentTool as FishingRod).isFishing)
                {
                  Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 80f, playerPosition.Y - 96f)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipVertically, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 128) / 10000f));
                  break;
                }
                if (f.FacingDirection != 2)
                  break;
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + (f.FacingDirection == 0 ? 0 : 128)) / 10000f));
                break;
              case 5:
                if (f.FacingDirection != 2 || (f.CurrentTool as FishingRod).showingTreasure || (f.CurrentTool as FishingRod).fishCaught)
                  break;
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + (f.FacingDirection == 0 ? 0 : 128)) / 10000f));
                break;
            }
          }
          else if (f.CurrentTool != null && f.CurrentTool is WateringCan)
          {
            switch (f.Sprite.currentAnimationIndex)
            {
              case 0:
              case 1:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X + (float) num3, (float) ((double) playerPosition.Y - 128.0 + 16.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) f.GetBoundingBox().Bottom / 10000f));
                break;
              case 2:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X + (float) num3, (float) ((double) playerPosition.Y - 128.0 - (f.FacingDirection == 2 ? -4.0 : 32.0)) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) f.GetBoundingBox().Bottom / 10000f));
                break;
              case 3:
                if (f.FacingDirection == 2)
                  rectangle.X += 16;
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num3 - (f.FacingDirection == 2 ? 4.0 : 0.0)), (float) ((double) playerPosition.Y - 128.0 - (f.FacingDirection == 2 ? -24.0 : 64.0)) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) f.GetBoundingBox().Bottom / 10000f));
                break;
            }
          }
          else
          {
            switch (f.Sprite.currentAnimationIndex)
            {
              case 0:
                if (f.FacingDirection == 0)
                {
                  Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X + (float) num3, (float) ((double) playerPosition.Y - 128.0 - 8.0) + (float) num2 + (float) Math.Min(8, f.toolPower * 4))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() - 8) / 10000f));
                  break;
                }
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num3 - 20.0), (float) ((double) playerPosition.Y - 128.0 + 12.0) + (float) num2 + (float) Math.Min(8, f.toolPower * 4))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 8) / 10000f));
                break;
              case 1:
                if (f.FacingDirection == 0)
                {
                  Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num3 + 4.0), (float) ((double) playerPosition.Y - 128.0 + 40.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() - 8) / 10000f));
                  break;
                }
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num3 - 12.0), (float) ((double) playerPosition.Y - 128.0 + 32.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, -0.1308997f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 8) / 10000f));
                break;
              case 2:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X + (float) num3, (float) ((double) playerPosition.Y - 128.0 + 64.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) ((f.getStandingY() + f.FacingDirection == 0 ? -8.0 : 8.0) / 10000.0)));
                break;
              case 3:
                if (f.FacingDirection == 0)
                  break;
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X + (float) num3, (float) ((double) playerPosition.Y - 64.0 + 44.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 8) / 10000f));
                break;
              case 4:
                if (f.FacingDirection == 0)
                  break;
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X + (float) num3, (float) ((double) playerPosition.Y - 64.0 + 48.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 8) / 10000f));
                break;
              case 5:
                Game1.spriteBatch.Draw(Game1.toolSpriteSheet, Utility.snapToInt(new Vector2(playerPosition.X + (float) num3, (float) ((double) playerPosition.Y - 64.0 + 32.0) + (float) num2)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, num1 + (float) (f.getStandingY() + 8) / 10000f));
                break;
            }
          }
        }
      }
    }

    /// ####################
    ///             OTHER HELPER METHODS
    ///             ####################
    public static Vector2 GlobalToLocal(xTile.Dimensions.Rectangle viewport, Vector2 globalPosition) => new Vector2(globalPosition.X - (float) viewport.X, globalPosition.Y - (float) viewport.Y);

    public static bool IsEnglish() => Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en;

    public static Vector2 GlobalToLocal(Vector2 globalPosition) => new Vector2(globalPosition.X - (float) Game1.viewport.X, globalPosition.Y - (float) Game1.viewport.Y);

    public static Microsoft.Xna.Framework.Rectangle GlobalToLocal(
      xTile.Dimensions.Rectangle viewport,
      Microsoft.Xna.Framework.Rectangle globalPosition)
    {
      return new Microsoft.Xna.Framework.Rectangle(globalPosition.X - viewport.X, globalPosition.Y - viewport.Y, globalPosition.Width, globalPosition.Height);
    }

    public static string parseText(string text, SpriteFont whichFont, int width)
    {
      if (text == null)
        return "";
      string str1 = string.Empty;
      string str2 = string.Empty;
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.ja:
        case LocalizedContentManager.LanguageCode.zh:
        case LocalizedContentManager.LanguageCode.th:
          foreach (char ch in text)
          {
            if ((double) whichFont.MeasureString(str1 + ch.ToString()).Length() > (double) width || ch.Equals((object) Environment.NewLine))
            {
              str2 = str2 + str1 + Environment.NewLine;
              str1 = string.Empty;
            }
            if (!ch.Equals((object) Environment.NewLine))
              str1 += ch.ToString();
          }
          return str2 + str1;
        case LocalizedContentManager.LanguageCode.fr:
          if (text.Contains("^"))
          {
            string[] strArray = text.Split('^');
            text = !Game1.player.IsMale ? strArray[1] : strArray[0];
            break;
          }
          break;
      }
      foreach (string str3 in text.Split(' '))
      {
        try
        {
          if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr && str3.StartsWith("\n-"))
          {
            str2 = str2 + str1 + Environment.NewLine;
            str1 = string.Empty;
          }
          if ((double) whichFont.MeasureString(str1 + str3).X > (double) width || str3.Equals(Environment.NewLine))
          {
            str2 = str2 + str1 + Environment.NewLine;
            str1 = string.Empty;
          }
          if (!str3.Equals(Environment.NewLine))
            str1 = str1 + str3 + " ";
        }
        catch (Exception ex)
        {
          Console.WriteLine("Exception measuring string: " + ex?.ToString());
        }
      }
      return str2 + str1;
    }

    public static void UpdateHorseOwnership()
    {
      bool flag1 = false;
      Dictionary<long, Horse> dictionary1 = new Dictionary<long, Horse>();
      HashSet<Horse> horseSet = new HashSet<Horse>();
      List<Stable> stableList = new List<Stable>();
      foreach (Building building in Game1.getFarm().buildings)
      {
        if (building is Stable && (int) (NetFieldBase<int, NetInt>) building.daysOfConstructionLeft <= 0)
        {
          Stable stable = building as Stable;
          stableList.Add(stable);
        }
      }
      foreach (Stable stable in stableList)
      {
        if (stable.owner.Value == -6666666L && Game1.getFarmerMaybeOffline(-6666666L) == null)
          stable.owner.Value = Game1.player.UniqueMultiplayerID;
        stable.grabHorse();
      }
      foreach (Stable stable in stableList)
      {
        Horse stableHorse = stable.getStableHorse();
        if (stableHorse != null && !horseSet.Contains(stableHorse) && stableHorse.getOwner() != null && !dictionary1.ContainsKey(stableHorse.getOwner().UniqueMultiplayerID) && stableHorse.getOwner().horseName.Value != null && stableHorse.getOwner().horseName.Value.Length > 0 && stableHorse.Name == stableHorse.getOwner().horseName.Value)
        {
          dictionary1[stableHorse.getOwner().UniqueMultiplayerID] = stableHorse;
          horseSet.Add(stableHorse);
          if (flag1)
            Console.WriteLine("Assigned horse " + stableHorse.Name + " to " + stableHorse.getOwner().Name + " (Exact match)");
        }
      }
      Dictionary<string, Farmer> dictionary2 = new Dictionary<string, Farmer>();
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer != null && allFarmer.horseName.Value != null && allFarmer.horseName.Value.Length != 0)
        {
          bool flag2 = false;
          foreach (Horse horse in horseSet)
          {
            if (horse.getOwner() == allFarmer)
            {
              flag2 = true;
              break;
            }
          }
          if (!flag2)
            dictionary2[(string) (NetFieldBase<string, NetString>) allFarmer.horseName] = allFarmer;
        }
      }
      foreach (Stable stable in stableList)
      {
        Horse stableHorse = stable.getStableHorse();
        if (stableHorse != null && !horseSet.Contains(stableHorse) && stableHorse.getOwner() != null && stableHorse.Name != null && stableHorse.Name.Length > 0 && dictionary2.ContainsKey(stableHorse.Name) && !dictionary1.ContainsKey(dictionary2[stableHorse.Name].UniqueMultiplayerID))
        {
          stable.owner.Value = dictionary2[stableHorse.Name].UniqueMultiplayerID;
          stable.updateHorseOwnership();
          dictionary1[stableHorse.getOwner().UniqueMultiplayerID] = stableHorse;
          horseSet.Add(stableHorse);
          if (flag1)
            Console.WriteLine("Assigned horse " + stableHorse.Name + " to " + stableHorse.getOwner().Name + " (Name match from different owner.)");
        }
      }
      foreach (Stable stable in stableList)
      {
        Horse stableHorse = stable.getStableHorse();
        if (stableHorse != null && !horseSet.Contains(stableHorse) && stableHorse.getOwner() != null && !dictionary1.ContainsKey(stableHorse.getOwner().UniqueMultiplayerID))
        {
          dictionary1[stableHorse.getOwner().UniqueMultiplayerID] = stableHorse;
          horseSet.Add(stableHorse);
          stable.updateHorseOwnership();
          if (flag1)
            Console.WriteLine("Assigned horse " + stableHorse.Name + " to " + stableHorse.getOwner().Name + " (Owner's only stable)");
        }
      }
      foreach (Stable stable in stableList)
      {
        Horse stableHorse = stable.getStableHorse();
        if (stableHorse != null && !horseSet.Contains(stableHorse))
        {
          foreach (Horse horse in horseSet)
          {
            if ((NetFieldBase<long, NetLong>) stableHorse.ownerId == horse.ownerId)
            {
              stable.owner.Value = 0L;
              stable.updateHorseOwnership();
              if (flag1)
              {
                Console.WriteLine("Unassigned horse (stable owner already has a horse).");
                break;
              }
              break;
            }
          }
        }
      }
    }

    public static string LoadStringByGender(int npcGender, string key) => npcGender == 0 ? ((IEnumerable<string>) Game1.content.LoadString(key).Split('/')).First<string>() : ((IEnumerable<string>) Game1.content.LoadString(key).Split('/')).Last<string>();

    public static string LoadStringByGender(
      int npcGender,
      string key,
      params object[] substitutions)
    {
      if (npcGender == 0)
      {
        string format = ((IEnumerable<string>) Game1.content.LoadString(key).Split('/')).First<string>();
        if (substitutions.Length != 0)
        {
          try
          {
            return string.Format(format, substitutions);
          }
          catch (Exception ex)
          {
            return format;
          }
        }
      }
      string format1 = ((IEnumerable<string>) Game1.content.LoadString(key).Split('/')).Last<string>();
      if (substitutions.Length == 0)
        return format1;
      try
      {
        return string.Format(format1, substitutions);
      }
      catch (Exception ex)
      {
        return format1;
      }
    }

    public static string parseText(string text) => Game1.parseText(text, Game1.dialogueFont, Game1.dialogueWidth);

    public static bool isThisPositionVisibleToPlayer(string locationName, Vector2 position) => locationName.Equals(Game1.currentLocation.Name) && new Microsoft.Xna.Framework.Rectangle((int) ((double) Game1.player.Position.X - (double) (Game1.viewport.Width / 2)), (int) ((double) Game1.player.Position.Y - (double) (Game1.viewport.Height / 2)), Game1.viewport.Width, Game1.viewport.Height).Contains(new Point((int) position.X, (int) position.Y));

    public static Microsoft.Xna.Framework.Rectangle getSourceRectForStandardTileSheet(
      Texture2D tileSheet,
      int tilePosition,
      int width = -1,
      int height = -1)
    {
      if (width == -1)
        width = 64;
      if (height == -1)
        height = 64;
      return new Microsoft.Xna.Framework.Rectangle(tilePosition * width % tileSheet.Width, tilePosition * width / tileSheet.Width * height, width, height);
    }

    public static Microsoft.Xna.Framework.Rectangle getSquareSourceRectForNonStandardTileSheet(
      Texture2D tileSheet,
      int tileWidth,
      int tileHeight,
      int tilePosition)
    {
      return new Microsoft.Xna.Framework.Rectangle(tilePosition * tileWidth % tileSheet.Width, tilePosition * tileWidth / tileSheet.Width * tileHeight, tileWidth, tileHeight);
    }

    public static Microsoft.Xna.Framework.Rectangle getArbitrarySourceRect(
      Texture2D tileSheet,
      int tileWidth,
      int tileHeight,
      int tilePosition)
    {
      return tileSheet != null ? new Microsoft.Xna.Framework.Rectangle(tilePosition * tileWidth % tileSheet.Width, tilePosition * tileWidth / tileSheet.Width * tileHeight, tileWidth, tileHeight) : Microsoft.Xna.Framework.Rectangle.Empty;
    }

    public static string getTimeOfDayString(int time)
    {
      string str1 = time % 100 == 0 ? "0" : string.Empty;
      string str2;
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.ja:
          str2 = time / 100 % 12 == 0 ? "0" : (time / 100 % 12).ToString();
          break;
        case LocalizedContentManager.LanguageCode.ru:
        case LocalizedContentManager.LanguageCode.pt:
        case LocalizedContentManager.LanguageCode.es:
        case LocalizedContentManager.LanguageCode.de:
        case LocalizedContentManager.LanguageCode.th:
        case LocalizedContentManager.LanguageCode.fr:
        case LocalizedContentManager.LanguageCode.tr:
        case LocalizedContentManager.LanguageCode.hu:
          string str3 = (time / 100 % 24).ToString();
          str2 = time / 100 % 24 <= 9 ? "0" + str3 : str3;
          break;
        case LocalizedContentManager.LanguageCode.zh:
          str2 = time / 100 % 24 == 0 ? "00" : (time / 100 % 12 == 0 ? "12" : (time / 100 % 12).ToString());
          break;
        default:
          str2 = time / 100 % 12 == 0 ? "12" : (time / 100 % 12).ToString();
          break;
      }
      string timeOfDayString = str2 + ":" + (time % 100).ToString() + str1;
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.en:
          timeOfDayString = timeOfDayString + " " + (time < 1200 || time >= 2400 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370") : Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371"));
          break;
        case LocalizedContentManager.LanguageCode.ja:
          timeOfDayString = time < 1200 || time >= 2400 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370") + " " + timeOfDayString : Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371") + " " + timeOfDayString;
          break;
        case LocalizedContentManager.LanguageCode.zh:
          timeOfDayString = time < 600 || time >= 2400 ? "凌晨 " + timeOfDayString : (time < 1200 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370") + " " + timeOfDayString : (time < 1300 ? "中午  " + timeOfDayString : (time < 1900 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371") + " " + timeOfDayString : "晚上  " + timeOfDayString)));
          break;
        case LocalizedContentManager.LanguageCode.fr:
          string str4;
          if (time % 100 != 0)
          {
            if (time / 100 != 24)
            {
              int num = time / 100;
              string str5 = num.ToString();
              num = time % 100;
              string str6 = num.ToString();
              str4 = str5 + "h" + str6;
            }
            else
              str4 = "00h";
          }
          else
            str4 = time / 100 == 24 ? "00h" : (time / 100).ToString() + "h";
          timeOfDayString = str4;
          break;
        case LocalizedContentManager.LanguageCode.mod:
          return LocalizedContentManager.FormatTimeString(time, LocalizedContentManager.CurrentModLanguage.TimeFormat).ToString();
      }
      return timeOfDayString;
    }

    public bool checkBigCraftableBoundariesForFrontLayer() => Game1.currentLocation.Map.GetLayer("Front").PickTile(new Location(Game1.player.getStandingX() - 32, (int) Game1.player.Position.Y - 38), Game1.viewport.Size) != null || Game1.currentLocation.Map.GetLayer("Front").PickTile(new Location(Game1.player.getStandingX() + 32, (int) Game1.player.Position.Y - 38), Game1.viewport.Size) != null || Game1.currentLocation.Map.GetLayer("Front").PickTile(new Location(Game1.player.getStandingX() - 32, (int) Game1.player.Position.Y - 38 - 64), Game1.viewport.Size) != null || Game1.currentLocation.Map.GetLayer("Front").PickTile(new Location(Game1.player.getStandingX() + 32, (int) Game1.player.Position.Y - 38 - 64), Game1.viewport.Size) != null;

    public static bool[,] getCircleOutlineGrid(int radius)
    {
      bool[,] circleOutlineGrid = new bool[radius * 2 + 1, radius * 2 + 1];
      int num1 = 1 - radius;
      int num2 = 1;
      int num3 = -2 * radius;
      int num4 = 0;
      int num5 = radius;
      int index1 = radius;
      int index2 = radius;
      circleOutlineGrid[index1, index2 + radius] = true;
      circleOutlineGrid[index1, index2 - radius] = true;
      circleOutlineGrid[index1 + radius, index2] = true;
      circleOutlineGrid[index1 - radius, index2] = true;
      while (num4 < num5)
      {
        if (num1 >= 0)
        {
          --num5;
          num3 += 2;
          num1 += num3;
        }
        ++num4;
        num2 += 2;
        num1 += num2;
        circleOutlineGrid[index1 + num4, index2 + num5] = true;
        circleOutlineGrid[index1 - num4, index2 + num5] = true;
        circleOutlineGrid[index1 + num4, index2 - num5] = true;
        circleOutlineGrid[index1 - num4, index2 - num5] = true;
        circleOutlineGrid[index1 + num5, index2 + num4] = true;
        circleOutlineGrid[index1 - num5, index2 + num4] = true;
        circleOutlineGrid[index1 + num5, index2 - num4] = true;
        circleOutlineGrid[index1 - num5, index2 - num4] = true;
      }
      return circleOutlineGrid;
    }

    public static Microsoft.Xna.Framework.Color getColorForTreasureType(string type)
    {
      switch (type)
      {
        case "Arch":
          return Microsoft.Xna.Framework.Color.White;
        case "Coal":
          return Microsoft.Xna.Framework.Color.Black;
        case "Coins":
          return Microsoft.Xna.Framework.Color.Yellow;
        case "Copper":
          return Microsoft.Xna.Framework.Color.Sienna;
        case "Gold":
          return Microsoft.Xna.Framework.Color.Gold;
        case "Iridium":
          return Microsoft.Xna.Framework.Color.Purple;
        case "Iron":
          return Microsoft.Xna.Framework.Color.LightSlateGray;
        default:
          return Microsoft.Xna.Framework.Color.SaddleBrown;
      }
    }

    public static string GetFarmTypeID() => Game1.whichFarm == 7 && Game1.whichModFarm != null ? Game1.whichModFarm.ID : Game1.whichFarm.ToString();

    public static string GetFarmTypeModData(string key) => Game1.whichFarm == 7 && Game1.whichModFarm != null && Game1.whichModFarm.ModData != null && Game1.whichModFarm.ModData.ContainsKey(key) ? Game1.whichModFarm.ModData[key] : (string) null;

    public void _PerformRemoveNormalItemFromWorldOvernight(int parent_sheet_index)
    {
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
        this._RecursiveRemoveThisNormalItemLocation(location, parent_sheet_index);
      foreach (GameLocation activeMine in MineShaft.activeMines)
        this._RecursiveRemoveThisNormalItemLocation(activeMine, parent_sheet_index);
      foreach (GameLocation activeLevel in VolcanoDungeon.activeLevels)
        this._RecursiveRemoveThisNormalItemLocation(activeLevel, parent_sheet_index);
      for (int index = 0; index < Game1.player.team.returnedDonations.Count; ++index)
      {
        if (this._RecursiveRemoveThisNormalItemItem(Game1.player.team.returnedDonations[index], parent_sheet_index))
        {
          Game1.player.team.returnedDonations.RemoveAt(index);
          --index;
        }
      }
      for (int index = 0; index < Game1.player.team.junimoChest.Count; ++index)
      {
        if (this._RecursiveRemoveThisNormalItemItem(Game1.player.team.junimoChest[index], parent_sheet_index))
        {
          Game1.player.team.junimoChest.RemoveAt(index);
          --index;
        }
      }
      foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
      {
        for (int index = 0; index < specialOrder.donatedItems.Count; ++index)
        {
          if (this._RecursiveRemoveThisNormalItemItem(specialOrder.donatedItems[index], parent_sheet_index))
            specialOrder.donatedItems[index] = (Item) null;
        }
      }
    }

    protected virtual void _PerformRemoveNormalItemFromFarmerOvernight(
      Farmer farmer,
      int parent_sheet_index)
    {
      for (int index = 0; index < farmer.items.Count; ++index)
      {
        if (this._RecursiveRemoveThisNormalItemItem(farmer.items[index], parent_sheet_index))
          farmer.items[index] = (Item) null;
      }
      for (int index = 0; index < farmer.itemsLostLastDeath.Count; ++index)
      {
        if (this._RecursiveRemoveThisNormalItemItem(farmer.itemsLostLastDeath[index], parent_sheet_index))
        {
          farmer.itemsLostLastDeath.RemoveAt(index);
          --index;
        }
      }
      if (farmer.recoveredItem == null || !this._RecursiveRemoveThisNormalItemItem(farmer.recoveredItem, parent_sheet_index))
        return;
      farmer.recoveredItem = (Item) null;
      farmer.mailbox.Remove("MarlonRecovery");
      farmer.mailForTomorrow.Remove("MarlonRecovery");
    }

    protected virtual bool _RecursiveRemoveThisNormalItemItem(
      Item this_item,
      int parent_sheet_index)
    {
      if (this_item == null)
        return false;
      if (this_item is Object)
      {
        Object @object = this_item as Object;
        if (@object.heldObject.Value != null && this._RecursiveRemoveThisNormalItemItem((Item) @object.heldObject.Value, parent_sheet_index))
        {
          @object.heldObject.Value = (Object) null;
          @object.readyForHarvest.Value = false;
          @object.showNextIndex.Value = false;
        }
        if (@object is StorageFurniture)
        {
          bool flag = false;
          for (int index = 0; index < (@object as StorageFurniture).heldItems.Count; ++index)
          {
            Item heldItem = (@object as StorageFurniture).heldItems[index];
            if (heldItem != null && this._RecursiveRemoveThisNormalItemItem(heldItem, parent_sheet_index))
            {
              (@object as StorageFurniture).heldItems[index] = (Item) null;
              flag = true;
            }
          }
          if (flag)
            (@object as StorageFurniture).ClearNulls();
        }
        if (@object is IndoorPot)
        {
          IndoorPot indoorPot = @object as IndoorPot;
          if ((NetFieldBase<HoeDirt, NetRef<HoeDirt>>) indoorPot.hoeDirt != (NetRef<HoeDirt>) null)
            this._RecursiveRemoveThisNormalItemDirt((HoeDirt) (NetFieldBase<HoeDirt, NetRef<HoeDirt>>) indoorPot.hoeDirt, (GameLocation) null, Vector2.Zero, parent_sheet_index);
        }
        if (@object is Chest)
        {
          bool flag = false;
          for (int index = 0; index < (@object as Chest).items.Count; ++index)
          {
            Item this_item1 = (@object as Chest).items[index];
            if (this_item1 != null && this._RecursiveRemoveThisNormalItemItem(this_item1, parent_sheet_index))
            {
              (@object as Chest).items[index] = (Item) null;
              flag = true;
            }
          }
          if (flag)
            (@object as Chest).clearNulls();
        }
        if (@object.heldObject.Value != null && this._RecursiveRemoveThisNormalItemItem((Item) (Object) (NetFieldBase<Object, NetRef<Object>>) @object.heldObject, parent_sheet_index))
          @object.heldObject.Value = (Object) null;
      }
      return Utility.IsNormalObjectAtParentSheetIndex(this_item, parent_sheet_index);
    }

    protected virtual void _RecursiveRemoveThisNormalItemDirt(
      HoeDirt dirt,
      GameLocation location,
      Vector2 coord,
      int parent_sheet_index)
    {
      if (dirt.crop == null || dirt.crop.indexOfHarvest.Value != parent_sheet_index)
        return;
      dirt.destroyCrop(coord, false, location);
    }

    protected virtual void _RecursiveRemoveThisNormalItemLocation(
      GameLocation l,
      int parent_sheet_index)
    {
      if (l == null)
        return;
      if (l != null)
      {
        List<Guid> guidList = new List<Guid>();
        foreach (Furniture this_item in l.furniture)
        {
          if (this._RecursiveRemoveThisNormalItemItem((Item) this_item, parent_sheet_index))
            guidList.Add(l.furniture.GuidOf(this_item));
        }
        foreach (Guid guid in guidList)
          l.furniture.Remove(guid);
        foreach (NPC character in l.characters)
        {
          if (character is Monster)
          {
            Monster monster = character as Monster;
            if (monster.objectsToDrop != null && monster.objectsToDrop.Count > 0)
            {
              for (int index = monster.objectsToDrop.Count - 1; index >= 0; --index)
              {
                if (monster.objectsToDrop[index] == parent_sheet_index)
                  monster.objectsToDrop.RemoveAt(index);
              }
            }
          }
        }
      }
      if (l is IslandFarmHouse)
      {
        for (int index = 0; index < (l as IslandFarmHouse).fridge.Value.items.Count; ++index)
        {
          Item this_item = (l as IslandFarmHouse).fridge.Value.items[index];
          if (this_item != null && this._RecursiveRemoveThisNormalItemItem(this_item, parent_sheet_index))
            (l as IslandFarmHouse).fridge.Value.items[index] = (Item) null;
        }
      }
      foreach (Vector2 key in l.terrainFeatures.Keys)
      {
        TerrainFeature terrainFeature = l.terrainFeatures[key];
        if (terrainFeature is HoeDirt)
          this._RecursiveRemoveThisNormalItemDirt(terrainFeature as HoeDirt, l, key, parent_sheet_index);
      }
      if (l is FarmHouse)
      {
        for (int index = 0; index < (l as FarmHouse).fridge.Value.items.Count; ++index)
        {
          Item this_item = (l as FarmHouse).fridge.Value.items[index];
          if (this_item != null && this._RecursiveRemoveThisNormalItemItem(this_item, parent_sheet_index))
            (l as FarmHouse).fridge.Value.items[index] = (Item) null;
        }
      }
      if (l is BuildableGameLocation)
      {
        foreach (Building building in (l as BuildableGameLocation).buildings)
        {
          if (building.indoors.Value != null)
            this._RecursiveRemoveThisNormalItemLocation(building.indoors.Value, parent_sheet_index);
          if (building is Mill)
          {
            for (int index = 0; index < (building as Mill).output.Value.items.Count; ++index)
            {
              Item this_item = (building as Mill).output.Value.items[index];
              if (this_item != null && this._RecursiveRemoveThisNormalItemItem(this_item, parent_sheet_index))
                (building as Mill).output.Value.items[index] = (Item) null;
            }
          }
          else if (building is JunimoHut)
          {
            bool flag = false;
            Chest chest = (building as JunimoHut).output.Value;
            for (int index = 0; index < chest.items.Count; ++index)
            {
              Item this_item = chest.items[index];
              if (this_item != null && this._RecursiveRemoveThisNormalItemItem(this_item, parent_sheet_index))
              {
                chest.items[index] = (Item) null;
                flag = true;
              }
            }
            if (flag)
              chest.clearNulls();
          }
        }
      }
      foreach (Vector2 key in new List<Vector2>((IEnumerable<Vector2>) l.objects.Keys))
      {
        if (this._RecursiveRemoveThisNormalItemItem((Item) l.objects[key], parent_sheet_index))
          l.objects.Remove(key);
      }
      for (int index = 0; index < l.debris.Count; ++index)
      {
        Debris debri = l.debris[index];
        if (debri.item != null && this._RecursiveRemoveThisNormalItemItem(debri.item, parent_sheet_index))
        {
          l.debris.RemoveAt(index);
          --index;
        }
      }
    }

    public enum BundleType
    {
      Default,
      Remixed,
    }

    public enum MineChestType
    {
      Default,
      Remixed,
    }

    public enum MusicContext
    {
      Default,
      SubLocation,
      Event,
      MiniGame,
      ImportantSplitScreenMusic,
      MAX,
    }

    public delegate void afterFadeFunction();
  }
}
