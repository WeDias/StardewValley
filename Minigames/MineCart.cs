// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.MineCart
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Minigames
{
  public class MineCart : IMinigame
  {
    public MineCart.GameStates gameState;
    public const int followDistance = 96;
    public float pixelScale = 4f;
    public const int tilesBeyondViewportToSimulate = 4;
    public const int bgLoopWidth = 96;
    public const float gravity = 0.21f;
    public const int brownArea = 0;
    public const int frostArea = 1;
    public const int darkArea = 3;
    public const int waterArea = 2;
    public const int lavaArea = 4;
    public const int heavenlyArea = 5;
    public const int sunsetArea = 6;
    public const int endingCutscene = 7;
    public const int bonusLevel1 = 8;
    public const int mushroomArea = 9;
    public const int LAST_LEVEL = 6;
    public readonly int[] infiniteModeLevels = new int[8]
    {
      0,
      1,
      2,
      3,
      5,
      9,
      4,
      6
    };
    public float shakeMagnitude;
    protected Vector2 _shakeOffset = Vector2.Zero;
    public const int infiniteMode = 2;
    public const int progressMode = 3;
    public const int respawnTime = 1400;
    public float slimeBossPosition = -100f;
    public float slimeBossSpeed;
    public float secondsOnThisLevel;
    public int fruitEatCount;
    public int currentFruitCheckIndex = -1;
    public float currentFruitCheckMagnitude;
    public Matrix transformMatrix;
    public const int distanceToTravelInMineMode = 350;
    public const int checkpointScanDistance = 16;
    public int coinCount;
    public bool gamePaused;
    private SparklingText perfectText;
    private float lakeSpeedAccumulator;
    private float backBGPosition;
    private float midBGPosition;
    private float waterFallPosition;
    private int noiseSeed = Game1.random.Next(0, int.MaxValue);
    public Vector2 upperLeft;
    private Stopwatch musicSW;
    private bool titleJunimoStartedBobbing;
    private bool lastLevelWasPerfect;
    private bool completelyPerfect = true;
    private int screenWidth;
    private int screenHeight;
    public int tileSize;
    private int waterfallWidth = 1;
    private int ytileOffset;
    private int score;
    private int levelsBeat;
    private int gameMode;
    private int livesLeft;
    private int distanceToTravel = -1;
    private int respawnCounter;
    private int currentTheme;
    private bool reachedFinish;
    private bool gameOver;
    private float screenDarkness;
    protected string cutsceneText = "";
    public float fadeDelta;
    private ICue minecartLoop;
    private Texture2D texture;
    private Dictionary<int, List<MineCart.Track>> _tracks;
    private List<MineCart.LakeDecor> lakeDecor = new List<MineCart.LakeDecor>();
    private List<Point> obstacles = new List<Point>();
    private List<MineCart.Spark> sparkShower = new List<MineCart.Spark>();
    private List<int> levelThemesFinishedThisRun = new List<int>();
    private Color backBGTint;
    private Color midBGTint;
    private Color caveTint;
    private Color lakeTint;
    private Color waterfallTint;
    private Color trackShadowTint;
    private Color trackTint;
    private Rectangle midBGSource = new Rectangle(64, 0, 96, 162);
    private Rectangle backBGSource = new Rectangle(64, 162, 96, 111);
    private Rectangle lakeBGSource = new Rectangle(0, 80, 16, 97);
    private int backBGYOffset;
    private int midBGYOffset;
    protected double _totalTime;
    private MineCart.MineCartCharacter player;
    private MineCart.MineCartCharacter trackBuilderCharacter;
    private MineCart.MineDebris titleScreenJunimo;
    private List<MineCart.Entity> _entities;
    public MineCart.LevelTransition[] LEVEL_TRANSITIONS;
    protected MineCart.BaseTrackGenerator _lastGenerator;
    protected MineCart.BaseTrackGenerator _forcedNextGenerator;
    public float screenLeftBound;
    public Point generatorPosition;
    private MineCart.BaseTrackGenerator _trackGenerator;
    protected MineCart.GoalIndicator _goalIndicator;
    public int bottomTile;
    public int topTile;
    public float deathTimer;
    protected int _lastTilePosition = -1;
    public int slimeResetPosition = -80;
    public float checkpointPosition;
    public int furthestGeneratedCheckpoint;
    public bool isJumpPressed;
    public float stateTimer;
    public int cutsceneTick;
    public float pauseBeforeTitleFadeOutTimer;
    public float mapTimer;
    private List<KeyValuePair<string, int>> _currentHighScores;
    private int currentHighScore;
    public float scoreUpdateTimer;
    protected HashSet<MineCart.CollectableFruits> _spawnedFruit;
    protected HashSet<MineCart.CollectableFruits> _collectedFruit;
    public List<int> checkpointPositions;
    protected Dictionary<MineCart.ObstacleTypes, List<Type>> _validObstacles;
    protected List<MineCart.GeneratorRoll> _generatorRolls;
    private bool _trackAddedFlip;
    protected bool _buttonState;
    public bool _wasJustChatting;

    public double totalTime => this._totalTime;

    public double totalTimeMS => this._totalTime * 1000.0;

    public MineCart(int whichTheme, int mode)
    {
      this._entities = new List<MineCart.Entity>();
      this._collectedFruit = new HashSet<MineCart.CollectableFruits>();
      this._generatorRolls = new List<MineCart.GeneratorRoll>();
      this._validObstacles = new Dictionary<MineCart.ObstacleTypes, List<Type>>();
      this.initLevelTransitions();
      if (Game1.player.team.junimoKartScores.GetScores().Count == 0)
      {
        Game1.player.team.junimoKartScores.AddScore(Game1.getCharacterFromName("Lewis").displayName, 50000);
        Game1.player.team.junimoKartScores.AddScore(Game1.getCharacterFromName("Shane").displayName, 25000);
        Game1.player.team.junimoKartScores.AddScore(Game1.getCharacterFromName("Sam").displayName, 10000);
        Game1.player.team.junimoKartScores.AddScore(Game1.getCharacterFromName("Abigail").displayName, 5000);
        Game1.player.team.junimoKartScores.AddScore(Game1.getCharacterFromName("Vincent").displayName, 250);
      }
      this.changeScreenSize();
      this.texture = Game1.content.Load<Texture2D>("Minigames\\MineCart");
      if (Game1.soundBank != null)
      {
        this.minecartLoop = Game1.soundBank.GetCue(nameof (minecartLoop));
        this.minecartLoop.Play();
        this.minecartLoop.Pause();
      }
      this.backBGYOffset = this.tileSize * 2;
      this.ytileOffset = this.screenHeight / 2 / this.tileSize;
      this.gameMode = mode;
      this.bottomTile = this.screenHeight / this.tileSize - 1;
      this.topTile = 4;
      this.currentTheme = whichTheme;
      this.ShowTitle();
    }

    public void initLevelTransitions() => this.LEVEL_TRANSITIONS = new MineCart.LevelTransition[15]
    {
      new MineCart.LevelTransition(-1, 0, 2, 5, "rrr"),
      new MineCart.LevelTransition(0, 8, 5, 5, "rddrrd", (Func<bool>) (() => this.lastLevelWasPerfect)),
      new MineCart.LevelTransition(0, 1, 5, 5, "rddlddrdd"),
      new MineCart.LevelTransition(1, 3, 6, 11, "drdrrrrrrrrruuuuu", (Func<bool>) (() => (double) this.secondsOnThisLevel <= 60.0)),
      new MineCart.LevelTransition(1, 5, 6, 11, "rrurruuu", (Func<bool>) (() => Game1.random.NextDouble() <= 0.5)),
      new MineCart.LevelTransition(1, 2, 6, 11, "rrurrrrddr"),
      new MineCart.LevelTransition(8, 5, 8, 8, "ddrruuu", (Func<bool>) (() => Game1.random.NextDouble() <= 0.5)),
      new MineCart.LevelTransition(8, 2, 8, 8, "ddrrrrddr"),
      new MineCart.LevelTransition(5, 3, 10, 7, "urruulluurrrrrddddddr"),
      new MineCart.LevelTransition(2, 3, 13, 12, "rurruuu"),
      new MineCart.LevelTransition(3, 9, 16, 8, "rruuluu", (Func<bool>) (() => Game1.random.NextDouble() <= 0.5)),
      new MineCart.LevelTransition(3, 4, 16, 8, "rrddrddr"),
      new MineCart.LevelTransition(4, 6, 20, 12, "ruuruuuuuu"),
      new MineCart.LevelTransition(9, 6, 17, 4, "rrdrrru"),
      new MineCart.LevelTransition(6, 7, 22, 4, "rr")
    };

    public void ShowTitle()
    {
      this.musicSW = new Stopwatch();
      Game1.changeMusicTrack("junimoKart", music_context: Game1.MusicContext.MiniGame);
      this.titleJunimoStartedBobbing = false;
      this.completelyPerfect = true;
      this.screenDarkness = 1f;
      this.fadeDelta = -1f;
      this.ResetState();
      this.player.enabled = false;
      this.setUpTheme(0);
      this.levelThemesFinishedThisRun.Clear();
      this.gameState = MineCart.GameStates.Title;
      this.CreateLakeDecor();
      this.RefreshHighScore();
      this.titleScreenJunimo = this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(259, 492, 14, 20), new Vector2((float) (this.screenWidth / 2 - 128 + 137), (float) (this.screenHeight / 2 - 35 + 46)), 100f, 0.0f, gravity_multiplier: 0.0f, life_time: 99999f, animation_interval: 1f, draw_depth: 0.24f));
      if (this.gameMode == 3)
        this.setUpTheme(-1);
      else
        this.setUpTheme(0);
    }

    public void RefreshHighScore()
    {
      this._currentHighScores = Game1.player.team.junimoKartScores.GetScores();
      this.currentHighScore = 0;
      if (this._currentHighScores.Count <= 0)
        return;
      this.currentHighScore = this._currentHighScores[0].Value;
    }

    public MineCart.Obstacle AddObstacle(
      MineCart.Track track,
      MineCart.ObstacleTypes obstacle_type)
    {
      if (track == null)
        return (MineCart.Obstacle) null;
      if (!this._validObstacles.ContainsKey(obstacle_type))
        return (MineCart.Obstacle) null;
      MineCart.Obstacle obstacle = this.AddEntity<MineCart.Obstacle>(Activator.CreateInstance(Utility.GetRandom<Type>(this._validObstacles[obstacle_type])) as MineCart.Obstacle);
      if (!obstacle.CanSpawnHere(track))
      {
        obstacle.Destroy();
        return (MineCart.Obstacle) null;
      }
      obstacle.position.X = track.position.X + (float) (this.tileSize / 2);
      obstacle.position.Y = (float) track.GetYAtPoint(obstacle.position.X);
      track.obstacle = obstacle;
      obstacle.InitializeObstacle(track);
      return obstacle;
    }

    public virtual T AddEntity<T>(T new_entity) where T : MineCart.Entity
    {
      this._entities.Add((MineCart.Entity) new_entity);
      new_entity.Initialize(this);
      return new_entity;
    }

    public MineCart.Track GetTrackForXPosition(float x)
    {
      int key = (int) ((double) x / (double) this.tileSize);
      return !this._tracks.ContainsKey(key) ? (MineCart.Track) null : this._tracks[key][0];
    }

    public void AddCheckpoint(int tile_x)
    {
      if (this.gameMode == 2)
        return;
      tile_x = this.GetValidCheckpointPosition(tile_x);
      if (tile_x == this.furthestGeneratedCheckpoint || tile_x <= this.furthestGeneratedCheckpoint + 8 || !this.IsTileInBounds((int) ((double) this.GetTrackForXPosition((float) (tile_x * this.tileSize)).position.Y / (double) this.tileSize)))
        return;
      this.furthestGeneratedCheckpoint = tile_x;
      MineCart.CheckpointIndicator checkpointIndicator = this.AddEntity<MineCart.CheckpointIndicator>(new MineCart.CheckpointIndicator());
      checkpointIndicator.position.X = ((float) tile_x + 0.5f) * (float) this.tileSize;
      checkpointIndicator.position.Y = (float) this.GetTrackForXPosition((float) (tile_x * this.tileSize)).GetYAtPoint(checkpointIndicator.position.X + 5f);
      this.checkpointPositions.Add(tile_x);
    }

    public List<MineCart.Track> GetTracksForXPosition(float x)
    {
      int key = (int) ((double) x / (double) this.tileSize);
      return !this._tracks.ContainsKey(key) ? (List<MineCart.Track>) null : this._tracks[key];
    }

    protected bool _IsGeneratingOnUpperHalf() => this.generatorPosition.Y <= (this.topTile + this.bottomTile) / 2;

    protected bool _IsGeneratingOnLowerHalf() => this.generatorPosition.Y >= (this.topTile + this.bottomTile) / 2;

    protected void _GenerateMoreTrack()
    {
      while ((double) (this.generatorPosition.X * this.tileSize) <= (double) this.screenLeftBound + (double) this.screenWidth + (double) (16 * this.tileSize))
      {
        if (this._trackGenerator == null)
        {
          if (this.generatorPosition.X < this.distanceToTravel)
          {
            for (int index1 = 0; index1 < 2; ++index1)
            {
              for (int index2 = 0; index2 < this._generatorRolls.Count; ++index2)
              {
                if (this._forcedNextGenerator != null)
                {
                  this._trackGenerator = this._forcedNextGenerator;
                  this._forcedNextGenerator = (MineCart.BaseTrackGenerator) null;
                  break;
                }
                if (this._generatorRolls[index2].generator != this._lastGenerator && Game1.random.NextDouble() < (double) this._generatorRolls[index2].chance && (this._generatorRolls[index2].additionalGenerationCondition == null || this._generatorRolls[index2].additionalGenerationCondition()))
                {
                  this._trackGenerator = this._generatorRolls[index2].generator;
                  this._forcedNextGenerator = this._generatorRolls[index2].forcedNextGenerator;
                  break;
                }
              }
              if (this._trackGenerator == null)
              {
                if (this._trackGenerator == null)
                {
                  if (this._lastGenerator != null)
                  {
                    this._lastGenerator = (MineCart.BaseTrackGenerator) null;
                  }
                  else
                  {
                    this._trackGenerator = (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetLength(2, 2).SetStaggerChance(0.0f).SetCheckpoint(false);
                    this._forcedNextGenerator = (MineCart.BaseTrackGenerator) null;
                  }
                }
              }
              else
                break;
            }
            this._trackGenerator.Initialize();
            this._lastGenerator = this._trackGenerator;
          }
          else
          {
            this._trackGenerator = (MineCart.BaseTrackGenerator) null;
            break;
          }
        }
        if (this._trackGenerator != null)
          this._trackGenerator.GenerateTrack();
        if (this.generatorPosition.X < this.distanceToTravel)
          this._trackGenerator = (MineCart.BaseTrackGenerator) null;
        else
          break;
      }
      if (this.generatorPosition.X < this.distanceToTravel)
        return;
      MineCart.Track track = this.AddTrack(this.generatorPosition.X, this.generatorPosition.Y);
      if (this._goalIndicator == null)
      {
        this._goalIndicator = this.AddEntity<MineCart.GoalIndicator>(new MineCart.GoalIndicator());
        this._goalIndicator.position.X = ((float) this.generatorPosition.X + 0.5f) * (float) this.tileSize;
        this._goalIndicator.position.Y = (float) track.GetYAtPoint(this._goalIndicator.position.X);
      }
      else
        this.CreatePickup(new Vector2((float) this.generatorPosition.X + 0.5f, (float) (this.generatorPosition.Y - 1)) * (float) this.tileSize, true);
      ++this.generatorPosition.X;
    }

    public MineCart.Track AddTrack(int x, int y, MineCart.Track.TrackType type = MineCart.Track.TrackType.Straight)
    {
      if (type == MineCart.Track.TrackType.UpSlope || type == MineCart.Track.TrackType.SlimeUpSlope)
        ++y;
      this._trackAddedFlip = !this._trackAddedFlip;
      MineCart.Track track_object = new MineCart.Track(type, this._trackAddedFlip);
      track_object.position.X = (float) (x * this.tileSize);
      track_object.position.Y = (float) (y * this.tileSize);
      return this.AddTrack(track_object);
    }

    public MineCart.Track AddTrack(MineCart.Track track_object)
    {
      MineCart.Track track = this.AddEntity<MineCart.Track>(track_object);
      int key = (int) ((double) track.position.X / (double) this.tileSize);
      if (!this._tracks.ContainsKey(key))
        this._tracks[key] = new List<MineCart.Track>();
      this._tracks[key].Add(track_object);
      this._tracks[key].OrderBy<MineCart.Track, float>((Func<MineCart.Track, float>) (o => o.position.Y));
      return track;
    }

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public void UpdateMapTick(float time)
    {
      this.mapTimer += time;
      MineCart.MapJunimo mapJunimo = (MineCart.MapJunimo) null;
      foreach (MineCart.Entity entity in this._entities)
      {
        if (entity is MineCart.MapJunimo)
        {
          mapJunimo = entity as MineCart.MapJunimo;
          break;
        }
      }
      if ((double) this.mapTimer >= 2.0 && mapJunimo.moveState == MineCart.MapJunimo.MoveState.Idle)
        mapJunimo.StartMoving();
      if (mapJunimo.moveState == MineCart.MapJunimo.MoveState.Moving)
        this.mapTimer = 0.0f;
      if (mapJunimo.moveState == MineCart.MapJunimo.MoveState.Finished && (double) this.mapTimer >= 1.5)
        this.fadeDelta = 1f;
      if ((double) this.screenDarkness < 1.0 || (double) this.fadeDelta <= 0.0)
        return;
      this.ShowCutscene();
    }

    public void UpdateCutsceneTick()
    {
      int num = 400;
      if (this.gamePaused)
        return;
      if (this.cutsceneTick == 0)
      {
        if (!this.minecartLoop.IsPaused)
          this.minecartLoop.Pause();
        this.cutsceneText = Game1.content.LoadString("Strings\\UI:Junimo_Kart_Level_" + this.currentTheme.ToString());
        if (this.currentTheme == 7)
          this.cutsceneText = "";
        this.player.enabled = false;
        this.screenDarkness = 1f;
        this.fadeDelta = -1f;
      }
      if (this.cutsceneTick == 100)
        this.player.enabled = true;
      if (this.currentTheme == 0)
      {
        if (this.cutsceneTick == 0)
        {
          MineCart.Roadblock roadblock1 = this.AddEntity<MineCart.Roadblock>(new MineCart.Roadblock());
          roadblock1.position.X = (float) (6 * this.tileSize);
          roadblock1.position.Y = (float) (10 * this.tileSize);
          MineCart.Roadblock roadblock2 = this.AddEntity<MineCart.Roadblock>(new MineCart.Roadblock());
          roadblock2.position.X = (float) (19 * this.tileSize);
          roadblock2.position.Y = (float) (10 * this.tileSize);
        }
        if (this.cutsceneTick == 140)
          this.player.Jump();
        if (this.cutsceneTick == 150)
          this.player.ReleaseJump();
        if (this.cutsceneTick == 130)
          this.AddEntity<MineCart.FallingBoulder>(new MineCart.FallingBoulder()).position = new Vector2(this.player.position.X + 100f, -16f);
        if (this.cutsceneTick == 160)
          this.AddEntity<MineCart.FallingBoulder>(new MineCart.FallingBoulder()).position = new Vector2(this.player.position.X + 100f, -16f);
        if (this.cutsceneTick == 190)
          this.AddEntity<MineCart.FallingBoulder>(new MineCart.FallingBoulder()).position = new Vector2(this.player.position.X + 100f, -16f);
        if (this.cutsceneTick == 270)
          this.player.Jump();
        if (this.cutsceneTick == 275)
          this.player.ReleaseJump();
      }
      if (this.currentTheme == 1)
      {
        if (this.cutsceneTick == 0)
        {
          this.AddTrack(2, 9, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(3, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(4, 8);
          this.AddTrack(5, 8);
          this.AddTrack(6, 7, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(7, 8, MineCart.Track.TrackType.IceDownSlope);
          this.AddTrack(8, 9, MineCart.Track.TrackType.IceDownSlope);
          this.AddTrack(9, 10, MineCart.Track.TrackType.IceDownSlope);
          this.AddTrack(13, 9, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(17, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(19, 10, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(21, 6, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(24, 8);
          this.AddTrack(25, 8);
          this.AddTrack(26, 8);
          this.AddTrack(27, 8);
          this.AddTrack(28, 8);
        }
        if (this.cutsceneTick == 100)
          this.player.Jump();
        if (this.cutsceneTick == 130)
          this.player.ReleaseJump();
        if (this.cutsceneTick == 200)
          this.player.Jump();
        if (this.cutsceneTick == 215)
          this.player.ReleaseJump();
        if (this.cutsceneTick == 260)
          this.player.Jump();
        if (this.cutsceneTick == 270)
          this.player.ReleaseJump();
        if (this.cutsceneTick == 304)
          this.player.Jump();
      }
      if (this.currentTheme == 4)
      {
        if (this.cutsceneTick == 0)
        {
          this.AddTrack(1, 12, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(2, 11, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(3, 10, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(4, 9, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(5, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(6, 9, MineCart.Track.TrackType.DownSlope);
          this.AddTrack(7, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(8, 9, MineCart.Track.TrackType.DownSlope);
          this.AddTrack(9, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(10, 9, MineCart.Track.TrackType.DownSlope);
          this.AddTrack(11, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(12, 9, MineCart.Track.TrackType.DownSlope);
          this.AddTrack(13, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(14, 9, MineCart.Track.TrackType.DownSlope);
          this.AddTrack(15, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(16, 9, MineCart.Track.TrackType.DownSlope);
          this.AddTrack(17, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(18, 9, MineCart.Track.TrackType.DownSlope);
          this.AddTrack(19, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(20, 9, MineCart.Track.TrackType.DownSlope);
          this.AddTrack(21, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(22, 7, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(23, 6, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(24, 5, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(25, 4, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(26, 3, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(27, 2, MineCart.Track.TrackType.UpSlope);
        }
        if (this.cutsceneTick == 100)
          this.player.Jump();
        if (this.cutsceneTick == 115)
          this.player.ReleaseJump();
        if (this.cutsceneTick == 265)
          this.player.Jump();
      }
      if (this.currentTheme == 2)
      {
        if (this.cutsceneTick == 0)
        {
          this.AddEntity<MineCart.Whale>(new MineCart.Whale());
          this.AddEntity<MineCart.PlayerBubbleSpawner>(new MineCart.PlayerBubbleSpawner());
        }
        if (this.cutsceneTick == 250)
        {
          this.player.velocity.X = 0.0f;
          foreach (MineCart.Entity entity in this._entities)
          {
            if (entity is MineCart.Whale)
            {
              Game1.playSound("croak");
              (entity as MineCart.Whale).SetState(MineCart.Whale.CurrentState.OpenMouth);
              break;
            }
          }
        }
        if (this.cutsceneTick == 260)
          this.player.Jump();
        if (this.cutsceneTick == 265)
          this.player.ReleaseJump();
        if (this.cutsceneTick == 310)
          this.player.velocity.X = -100f;
      }
      if (this.currentTheme == 3)
      {
        if (this.cutsceneTick == 0)
        {
          this.AddTrack(-1, 3);
          this.AddTrack(0, 3);
          this.AddTrack(1, 4, MineCart.Track.TrackType.DownSlope);
          this.AddTrack(2, 4);
          this.AddTrack(3, 4);
          this.AddTrack(4, 4);
          this.AddTrack(5, 4);
          this.AddTrack(6, -2);
          this.AddTrack(7, -2);
          this.AddTrack(8, -2);
          this.AddTrack(9, -2);
          this.AddTrack(19, 9);
          this.AddTrack(20, 9);
          this.AddTrack(21, 8, MineCart.Track.TrackType.UpSlope);
          this.AddTrack(22, 8);
          this.AddTrack(23, 8);
          this.AddTrack(24, 9, MineCart.Track.TrackType.DownSlope);
          this.AddTrack(25, 9);
          this.AddTrack(26, 8);
          this.AddTrack(27, 8);
          this.AddTrack(28, 8);
          this.player.position.Y = (float) (3 * this.tileSize);
        }
        if (this.cutsceneTick == 150)
          this.player.Jump();
        if (this.cutsceneTick == 130)
          this.player.ReleaseJump();
        if (this.cutsceneTick == 200)
          this.player.Jump();
        if (this.cutsceneTick == 215)
          this.player.ReleaseJump();
        if (this.cutsceneTick == 0)
        {
          MineCart.WillOWisp willOwisp = this.AddEntity<MineCart.WillOWisp>(new MineCart.WillOWisp());
          willOwisp.position.X = (float) (10 * this.tileSize);
          willOwisp.position.Y = (float) (5 * this.tileSize);
          willOwisp.visible = false;
        }
        if (this.cutsceneTick == 300)
          Game1.playSound("ghost");
        if (this.cutsceneTick >= 300 && this.cutsceneTick % 3 == 0 && this.cutsceneTick < 350)
        {
          foreach (MineCart.Entity entity in this._entities)
          {
            if (entity is MineCart.WillOWisp)
              entity.visible = !entity.visible;
          }
        }
        if (this.cutsceneTick == 350)
        {
          foreach (MineCart.Entity entity in this._entities)
          {
            if (entity is MineCart.WillOWisp)
              entity.visible = true;
          }
        }
      }
      if (this.currentTheme == 9)
      {
        if (this.cutsceneTick == 0)
        {
          this.AddTrack(0, 6);
          this.AddTrack(1, 6);
          this.AddTrack(2, 6);
          this.AddTrack(3, 6);
          MineCart.Track track = this.AddTrack(4, 6);
          MineCart.MushroomSpring mushroomSpring = this.AddEntity<MineCart.MushroomSpring>(new MineCart.MushroomSpring());
          mushroomSpring.InitializeObstacle(track);
          mushroomSpring.position = new Vector2(4.5f, 6f) * (float) this.tileSize;
          this.AddTrack(8, 6, MineCart.Track.TrackType.MushroomLeft);
          this.AddTrack(9, 6, MineCart.Track.TrackType.MushroomMiddle);
          this.AddTrack(10, 6, MineCart.Track.TrackType.MushroomRight);
          this.AddTrack(12, 10);
          List<MineCart.BalanceTrack> collection1 = new List<MineCart.BalanceTrack>();
          MineCart.NoxiousMushroom noxiousMushroom = this.AddEntity<MineCart.NoxiousMushroom>(new MineCart.NoxiousMushroom());
          noxiousMushroom.position = new Vector2(12.5f, 10f) * (float) this.tileSize;
          noxiousMushroom.nextFire = 3f;
          MineCart.BalanceTrack track_object1 = new MineCart.BalanceTrack(MineCart.Track.TrackType.MushroomLeft, false);
          track_object1.position.X = (float) (15 * this.tileSize);
          track_object1.position.Y = (float) (9 * this.tileSize);
          collection1.Add(track_object1);
          this.AddTrack((MineCart.Track) track_object1);
          MineCart.BalanceTrack track_object2 = new MineCart.BalanceTrack(MineCart.Track.TrackType.MushroomMiddle, false);
          track_object2.position.X = (float) (16 * this.tileSize);
          track_object2.position.Y = (float) (9 * this.tileSize);
          collection1.Add(track_object2);
          this.AddTrack((MineCart.Track) track_object2);
          MineCart.BalanceTrack track_object3 = new MineCart.BalanceTrack(MineCart.Track.TrackType.MushroomRight, false);
          track_object3.position.X = (float) (17 * this.tileSize);
          track_object3.position.Y = (float) (9 * this.tileSize);
          collection1.Add(track_object3);
          this.AddTrack((MineCart.Track) track_object3);
          List<MineCart.BalanceTrack> collection2 = new List<MineCart.BalanceTrack>();
          MineCart.BalanceTrack track_object4 = new MineCart.BalanceTrack(MineCart.Track.TrackType.MushroomLeft, false);
          track_object4.position.X = (float) (22 * this.tileSize);
          track_object4.position.Y = (float) (9 * this.tileSize);
          collection2.Add(track_object4);
          this.AddTrack((MineCart.Track) track_object4);
          MineCart.BalanceTrack track_object5 = new MineCart.BalanceTrack(MineCart.Track.TrackType.MushroomMiddle, false);
          track_object5.position.X = (float) (23 * this.tileSize);
          track_object5.position.Y = (float) (9 * this.tileSize);
          collection2.Add(track_object5);
          this.AddTrack((MineCart.Track) track_object5);
          MineCart.BalanceTrack track_object6 = new MineCart.BalanceTrack(MineCart.Track.TrackType.MushroomRight, false);
          track_object6.position.X = (float) (24 * this.tileSize);
          track_object6.position.Y = (float) (9 * this.tileSize);
          collection2.Add(track_object6);
          this.AddTrack((MineCart.Track) track_object6);
          foreach (MineCart.BalanceTrack balanceTrack in collection1)
          {
            balanceTrack.connectedTracks = new List<MineCart.BalanceTrack>((IEnumerable<MineCart.BalanceTrack>) collection1);
            balanceTrack.counterBalancedTracks = new List<MineCart.BalanceTrack>((IEnumerable<MineCart.BalanceTrack>) collection2);
          }
          foreach (MineCart.BalanceTrack balanceTrack in collection2)
          {
            balanceTrack.connectedTracks = new List<MineCart.BalanceTrack>((IEnumerable<MineCart.BalanceTrack>) collection2);
            balanceTrack.counterBalancedTracks = new List<MineCart.BalanceTrack>((IEnumerable<MineCart.BalanceTrack>) collection1);
          }
          this.player.position.Y = (float) (6 * this.tileSize);
        }
        if (this.cutsceneTick == 115)
          this.player.Jump();
        if (this.cutsceneTick == 120)
          this.player.ReleaseJump();
        if (this.cutsceneTick == 230)
          this.player.Jump();
        if (this.cutsceneTick == 250)
          this.player.ReleaseJump();
        if (this.cutsceneTick == 298)
          this.player.Jump();
      }
      if (this.currentTheme == 6)
      {
        if (this.cutsceneTick == 0)
        {
          this.AddTrack(0, 6);
          this.AddTrack(1, 3);
          this.AddTrack(2, 8);
          this.AddTrack(4, 4);
          this.AddTrack(5, 4);
          this.AddTrack(6, 2);
          this.AddTrack(8, 8);
          this.AddTrack(9, 1);
          this.AddTrack(10, 2);
          this.AddTrack(12, 8);
          this.AddTrack(13, 6);
          this.AddTrack(14, 6);
          this.AddTrack(15, 8);
          this.AddTrack(17, 4);
          this.AddTrack(18, 2);
          this.AddTrack(19, 2);
          this.AddTrack(20, 2);
          this.AddTrack(21, 2);
          this.AddTrack(22, 2);
          this.AddTrack(23, 2);
          this.AddTrack(24, 2);
          this.AddTrack(25, 2);
          this.AddTrack(26, 2);
          this.AddTrack(27, 2);
          this.AddTrack(28, 2);
          this.player.position.Y = (float) (6 * this.tileSize);
        }
        if (this.cutsceneTick == 129)
          this.player.Jump();
        if (this.cutsceneTick == 170)
          this.player.ReleaseJump();
        if (this.cutsceneTick == 214)
          this.player.Jump();
      }
      if (this.currentTheme == 7)
      {
        num = 800;
        if (this.cutsceneTick == 0)
        {
          if (this.completelyPerfect)
            this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(256, 182, 48, 45), new Vector2((float) (20 * this.tileSize) + 12f, (float) (10 * this.tileSize) - 21.5f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 1000f, animation_interval: 0.0f, draw_depth: 0.23f, holdLastFrame: true));
          else
            this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(256, 112, 25, 32), new Vector2((float) (20 * this.tileSize) + 12f, (float) (10 * this.tileSize) - 16f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 1000f, animation_interval: 0.0f, draw_depth: 0.23f, holdLastFrame: true));
        }
        if (this.cutsceneTick == 200)
          this.player.velocity.X = 40f;
        if (this.cutsceneTick == 250)
          this.player.velocity.X = 20f;
        if (this.cutsceneTick == 300)
          this.player.velocity.X = 0.0f;
        if (this.cutsceneTick >= 350 && this.cutsceneTick % 10 == 0 && this.cutsceneTick < 600)
        {
          Game1.playSound("junimoMeep1");
          this.AddEntity<MineCart.EndingJunimo>(new MineCart.EndingJunimo(this.completelyPerfect)).position = new Vector2((float) (20 * this.tileSize), (float) (10 * this.tileSize));
        }
      }
      if (this.cutsceneTick == num)
      {
        this.screenDarkness = 0.0f;
        this.fadeDelta = 2f;
      }
      if (this.cutsceneTick == num + 100)
      {
        this.EndCutscene();
      }
      else
      {
        if ((double) this.player.velocity.X > 0.0 && (double) this.player.position.X > (double) (this.screenWidth + this.tileSize))
        {
          if (!this.minecartLoop.IsPaused)
            this.minecartLoop.Pause();
          this.player.enabled = false;
        }
        if ((double) this.player.velocity.X < 0.0 && (double) this.player.position.X < (double) -this.tileSize)
        {
          if (!this.minecartLoop.IsPaused)
            this.minecartLoop.Pause();
          this.player.enabled = false;
        }
        if (this.currentTheme != 5 || this.cutsceneTick != 100)
          return;
        this.AddEntity<MineCart.HugeSlime>(new MineCart.HugeSlime());
        this.slimeBossPosition = -100f;
      }
    }

    public void UpdateFruitsSummary(float time)
    {
      if (this.currentTheme == 7)
      {
        this.currentFruitCheckIndex = -1;
        this.ShowCutscene();
      }
      if (this.gamePaused)
        return;
      if ((double) this.stateTimer >= 0.0)
      {
        this.stateTimer -= time;
        if ((double) this.stateTimer < 0.0)
          this.stateTimer = 0.0f;
      }
      if ((double) this.stateTimer != 0.0)
        return;
      if (this.livesLeft < 3 && this.gameMode == 3)
      {
        ++this.livesLeft;
        this.stateTimer = 0.25f;
        Game1.playSound("coin");
      }
      else
      {
        if (this.lastLevelWasPerfect && this.perfectText == null && this.gameMode == 3)
        {
          this.perfectText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:BobberBar_Perfect"), Color.Lime, Color.White, true, depth: 0.0f);
          Game1.playSound("yoba");
        }
        if (this.currentFruitCheckIndex == -1)
        {
          this.fruitEatCount = 0;
          this.currentFruitCheckIndex = 0;
          this.stateTimer = 0.5f;
        }
        else if (this.currentFruitCheckIndex >= 3)
        {
          this.perfectText = (SparklingText) null;
          this.currentFruitCheckIndex = -1;
          this.ShowMap();
        }
        else
        {
          if (this._collectedFruit.Contains((MineCart.CollectableFruits) this.currentFruitCheckIndex))
          {
            this._collectedFruit.Remove((MineCart.CollectableFruits) this.currentFruitCheckIndex);
            Game1.playSoundPitched("newArtifact", this.currentFruitCheckIndex * 100);
            ++this.fruitEatCount;
            if (this.fruitEatCount >= 3)
            {
              Game1.playSound("yoba");
              if (this.gameMode == 3)
              {
                ++this.livesLeft;
              }
              else
              {
                this.score += 5000;
                this.UpdateScoreState();
              }
            }
          }
          else
            Game1.playSoundPitched("sell", this.currentFruitCheckIndex * 100);
          this.stateTimer = 0.5f;
          this.currentFruitCheckMagnitude = 3f;
          ++this.currentFruitCheckIndex;
        }
      }
    }

    public void UpdateInput()
    {
      if (Game1.IsChatting || Game1.textEntry != null)
      {
        this._wasJustChatting = true;
      }
      else
      {
        if (this.gamePaused)
          return;
        bool flag = false;
        if (Game1.input.GetMouseState().LeftButton == ButtonState.Pressed)
          flag = true;
        if (!Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.useToolButton) && !Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.actionButton))
        {
          KeyboardState keyboardState = Game1.input.GetKeyboardState();
          if (!keyboardState.IsKeyDown(Keys.Space))
          {
            keyboardState = Game1.input.GetKeyboardState();
            if (!keyboardState.IsKeyDown(Keys.LeftShift))
              goto label_10;
          }
        }
        flag = true;
label_10:
        GamePadState gamePadState = Game1.input.GetGamePadState();
        if (!gamePadState.IsButtonDown(Buttons.A))
        {
          gamePadState = Game1.input.GetGamePadState();
          if (!gamePadState.IsButtonDown(Buttons.B))
            goto label_13;
        }
        flag = true;
label_13:
        if (flag != this._buttonState)
        {
          this._buttonState = flag;
          if (this._buttonState)
          {
            if (this.gameState == MineCart.GameStates.Title)
            {
              if ((double) this.pauseBeforeTitleFadeOutTimer != 0.0 || (double) this.screenDarkness != 0.0 || (double) this.fadeDelta > 0.0)
                return;
              this.pauseBeforeTitleFadeOutTimer = 0.5f;
              Game1.playSound("junimoMeep1");
              if (this.titleScreenJunimo != null)
              {
                this.titleScreenJunimo.Destroy();
                this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(259, 492, 14, 20), new Vector2((float) ((double) this.screenLeftBound + (double) (this.screenWidth / 2) - 128.0 + 137.0), (float) (this.screenHeight / 2 - 35 + 46)), 110f, -200f, gravity_multiplier: 3f, life_time: 99999f, animation_interval: 1f, draw_depth: 0.24f));
              }
              if (this.musicSW != null)
                this.musicSW.Stop();
              this.musicSW = (Stopwatch) null;
              return;
            }
            if (this.gameState == MineCart.GameStates.Cutscene)
            {
              this.EndCutscene();
              return;
            }
            if (this.gameState == MineCart.GameStates.Map)
            {
              this.fadeDelta = 1f;
              return;
            }
            if (this.player != null)
              this.player.QueueJump();
            this.isJumpPressed = true;
          }
          else if (!this.gamePaused)
          {
            if (this.player != null)
              this.player.ReleaseJump();
            this.isJumpPressed = false;
          }
        }
        this._wasJustChatting = false;
      }
    }

    public virtual bool CanPause() => this.gameState == MineCart.GameStates.Ingame || this.gameState == MineCart.GameStates.FruitsSummary || this.gameState == MineCart.GameStates.Cutscene || this.gameState == MineCart.GameStates.Map;

    public bool tick(GameTime time)
    {
      this.UpdateInput();
      float time1 = (float) time.ElapsedGameTime.TotalSeconds;
      if (this.gamePaused)
        time1 = 0.0f;
      if (!this.CanPause())
        this.gamePaused = false;
      this.shakeMagnitude = Utility.MoveTowards(this.shakeMagnitude, 0.0f, time1 * 3f);
      this.currentFruitCheckMagnitude = Utility.MoveTowards(this.currentFruitCheckMagnitude, 0.0f, time1 * 6f);
      this._totalTime += (double) time1;
      this.screenDarkness += this.fadeDelta * time1;
      if ((double) this.screenDarkness < 0.0)
        this.screenDarkness = 0.0f;
      if ((double) this.screenDarkness > 1.0)
        this.screenDarkness = 1f;
      if (this.gameState == MineCart.GameStates.Title)
      {
        if ((double) this.pauseBeforeTitleFadeOutTimer > 0.0)
        {
          this.pauseBeforeTitleFadeOutTimer -= 0.0166666f;
          if ((double) this.pauseBeforeTitleFadeOutTimer <= 0.0)
            this.fadeDelta = 1f;
        }
        if ((double) this.fadeDelta >= 0.0 && (double) this.screenDarkness >= 1.0)
        {
          this.restartLevel(true);
          return false;
        }
        if (Game1.random.NextDouble() < 0.1)
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(0, 250, 5, 5), Utility.getRandomPositionInThisRectangle(new Rectangle((int) this.screenLeftBound + this.screenWidth / 2 - 128, this.screenHeight / 2 - 35, 256, 71), Game1.random), 100f, 0.0f, gravity_multiplier: 0.0f, life_time: 0.6f, num_animation_frames: 6, draw_depth: 0.23f));
        if (this.musicSW != null && Game1.currentSong != null && Game1.currentSong.Name.Equals("junimoKart") && Game1.currentSong.IsPlaying && !this.musicSW.IsRunning)
          this.musicSW.Start();
        if (this.titleScreenJunimo != null && !this.titleJunimoStartedBobbing && this.musicSW != null && this.musicSW.ElapsedMilliseconds >= 48000L)
        {
          this.titleScreenJunimo.reset(new Rectangle(417, 347, 14, 20), this.titleScreenJunimo.position, 100f, 0.0f, gravity_multiplier: 0.0f, life_time: 9999f, num_animation_frames: 2, animation_interval: 0.25f, draw_depth: this.titleScreenJunimo.depth);
          this.titleJunimoStartedBobbing = true;
        }
        else if (this.titleScreenJunimo != null && this.titleJunimoStartedBobbing && this.musicSW != null && this.musicSW.ElapsedMilliseconds >= 80000L)
        {
          this.titleScreenJunimo.reset(new Rectangle(259, 492, 14, 20), this.titleScreenJunimo.position, 100f, 0.0f, gravity_multiplier: 0.0f, life_time: 99999f, animation_interval: 1f, draw_depth: 0.24f);
          this.musicSW.Stop();
          this.musicSW = (Stopwatch) null;
        }
      }
      else if (this.gameState == MineCart.GameStates.Map)
        this.UpdateMapTick(time1);
      else if (this.gameState == MineCart.GameStates.Cutscene)
      {
        if (!this.gamePaused)
          time1 = 0.0166666f;
        this.UpdateCutsceneTick();
        if (!this.gamePaused)
          ++this.cutsceneTick;
      }
      else if (this.gameState == MineCart.GameStates.FruitsSummary)
        this.UpdateFruitsSummary(time1);
      int num1 = (int) ((double) time1 * 1000.0);
      for (int index = 0; index < this._entities.Count; ++index)
      {
        if (this._entities[index] != null && this._entities[index].IsActive())
          this._entities[index].Update(time1);
      }
      if ((double) this.deathTimer <= 0.0 && this.respawnCounter > 0)
      {
        for (int index = 0; index < this._entities.Count; ++index)
          this._entities[index].OnPlayerReset();
      }
      for (int index = 0; index < this._entities.Count; ++index)
      {
        if (this._entities[index] != null && this._entities[index].ShouldReap())
        {
          this._entities.RemoveAt(index);
          --index;
        }
      }
      float screenLeftBound = this.screenLeftBound;
      if (this.gameState == MineCart.GameStates.Ingame)
      {
        this.secondsOnThisLevel += time1;
        if ((double) this.screenDarkness >= 1.0 && this.gameOver)
        {
          if (this.gameMode == 3)
          {
            this.ShowTitle();
          }
          else
          {
            this.levelsBeat = 0;
            this.coinCount = 0;
            this.setUpTheme(0);
            this.restartLevel(true);
          }
          return false;
        }
        if (this.checkpointPositions.Count > 0)
        {
          for (int index = 0; index < this.checkpointPositions.Count; index = index - 1 + 1)
          {
            this.GetTrackForXPosition((float) (this.checkpointPositions[index] * this.tileSize));
            if ((double) this.player.position.X >= (double) (this.checkpointPositions[index] * this.tileSize))
            {
              foreach (MineCart.Entity entity in this._entities)
              {
                if (entity is MineCart.CheckpointIndicator && (int) ((double) entity.position.X / (double) this.tileSize) == this.checkpointPositions[index])
                {
                  (entity as MineCart.CheckpointIndicator).Activate();
                  break;
                }
              }
              this.checkpointPosition = ((float) this.checkpointPositions[index] + 0.5f) * (float) this.tileSize;
              this.ReapEntities();
              this.checkpointPositions.RemoveAt(index);
            }
            else
              break;
          }
        }
        float num2 = 0.0f;
        if (this.gameState == MineCart.GameStates.Cutscene)
        {
          this.screenLeftBound = 0.0f;
        }
        else
        {
          if ((double) this.deathTimer <= 0.0 && this.respawnCounter > 0)
          {
            this.screenLeftBound = (double) this.screenLeftBound - (double) Math.Max(this.player.position.X - 96f, num2) <= 400.0 ? ((double) this.screenLeftBound - (double) Math.Max(this.player.position.X - 96f, num2) <= 200.0 ? Utility.MoveTowards(this.screenLeftBound, Math.Max(this.player.position.X - 96f, num2), 300f * time1) : Utility.MoveTowards(this.screenLeftBound, Math.Max(this.player.position.X - 96f, num2), 600f * time1)) : Utility.MoveTowards(this.screenLeftBound, Math.Max(this.player.position.X - 96f, 0.0f), 1200f * time1);
            if ((double) this.screenLeftBound < (double) num2)
              this.screenLeftBound = num2;
          }
          else if ((double) this.deathTimer <= 0.0 && (double) this.respawnCounter <= 0.0 && !this.reachedFinish)
            this.screenLeftBound = this.player.position.X - 96f;
          if ((double) this.screenLeftBound < (double) num2)
            this.screenLeftBound = num2;
        }
        if ((double) (this.generatorPosition.X * this.tileSize) <= (double) this.screenLeftBound + (double) this.screenWidth + (double) (16 * this.tileSize))
          this._GenerateMoreTrack();
        int num3 = (int) this.player.position.X / this.tileSize;
        if (this.respawnCounter <= 0)
        {
          if (num3 > this._lastTilePosition)
          {
            int num4 = num3 - this._lastTilePosition;
            this._lastTilePosition = num3;
            for (int index = 0; index < num4; ++index)
              this.score += 10;
          }
        }
        else if (this.respawnCounter > 0)
        {
          if ((double) this.deathTimer > 0.0)
            this.deathTimer -= time1;
          else if ((double) this.screenLeftBound <= (double) Math.Max(num2, this.player.position.X - 96f))
          {
            if (!this.player.enabled)
              Utility.CollectGarbage();
            this.player.enabled = true;
            this.respawnCounter -= num1;
          }
        }
        if (this._goalIndicator != null && this.distanceToTravel != -1 && (double) this.player.position.X >= (double) this._goalIndicator.position.X && this.distanceToTravel != -1 && (double) this.player.position.Y <= (double) this._goalIndicator.position.Y * (double) this.tileSize + 4.0 && !this.reachedFinish && (double) this.fadeDelta < 0.0)
        {
          Game1.playSound("reward");
          this.levelThemesFinishedThisRun.Add(this.currentTheme);
          if (this.gameMode == 2)
          {
            this.score += 5000;
            this.UpdateScoreState();
          }
          foreach (MineCart.Entity entity in this._entities)
          {
            switch (entity)
            {
              case MineCart.GoalIndicator _:
                (entity as MineCart.GoalIndicator).Activate();
                continue;
              case MineCart.Coin _:
              case MineCart.Fruit _:
                this.lastLevelWasPerfect = false;
                continue;
              default:
                continue;
            }
          }
          this.reachedFinish = true;
          this.fadeDelta = 1f;
        }
        if (this.score > this.currentHighScore)
          this.currentHighScore = this.score;
        if ((double) this.scoreUpdateTimer <= 0.0)
          this.UpdateScoreState();
        else
          this.scoreUpdateTimer -= time1;
        if (this.reachedFinish && Game1.random.NextDouble() < 0.25 && !this.gamePaused)
          this.createSparkShower();
        if (this.reachedFinish && (double) this.screenDarkness >= 1.0)
        {
          this.reachedFinish = false;
          if (this.gameMode != 3)
            this.currentTheme = this.infiniteModeLevels[(this.levelsBeat + 1) % 8];
          ++this.levelsBeat;
          this.setUpTheme(this.currentTheme);
          this.restartLevel();
        }
        float num5 = 3f;
        if (this.currentTheme == 9)
          num5 = 32f;
        if ((double) this.player.position.Y > (double) this.screenHeight + (double) num5)
          this.Die();
      }
      else if (this.gameState == MineCart.GameStates.FruitsSummary)
      {
        this.screenLeftBound = 0.0f;
        if (this.perfectText != null && this.perfectText.update(time))
          this.perfectText = (SparklingText) null;
      }
      if (this.gameState == MineCart.GameStates.Title)
        this.screenLeftBound += time1 * 100f;
      float num6 = (float) (((double) this.screenLeftBound - (double) screenLeftBound) / (double) this.tileSize);
      this.lakeSpeedAccumulator += (float) ((double) num1 * ((double) num6 / 4.0) % 96.0);
      this.backBGPosition += (float) num1 * (num6 / 5f);
      this.backBGPosition = (float) (((double) this.backBGPosition + 9600.0) % 96.0);
      this.midBGPosition += (float) num1 * (num6 / 4f);
      this.midBGPosition = (float) (((double) this.midBGPosition + 9600.0) % 96.0);
      this.waterFallPosition += (float) num1 * (float) ((double) num6 * 6.0 / 5.0);
      if ((double) this.waterFallPosition > (double) (this.screenWidth * 3 / 2))
      {
        this.waterFallPosition %= (float) (this.screenWidth * 3 / 2);
        this.waterfallWidth = Game1.random.Next(6);
      }
      for (int index = this.sparkShower.Count - 1; index >= 0; --index)
      {
        this.sparkShower[index].dy += (float) (0.104999996721745 * ((double) time1 / 0.0166666004806757));
        this.sparkShower[index].x += this.sparkShower[index].dx * (time1 / 0.0166666f);
        this.sparkShower[index].y += this.sparkShower[index].dy * (time1 / 0.0166666f);
        this.sparkShower[index].c.B = (byte) (0.0 + Math.Max(0.0, Math.Sin(this.totalTimeMS / (20.0 * Math.PI / (double) this.sparkShower[index].dx)) * (double) byte.MaxValue));
        if (this.reachedFinish)
        {
          this.sparkShower[index].c.R = (byte) (0.0 + Math.Max(0.0, Math.Sin((this.totalTimeMS + 50.0) / (20.0 * Math.PI / (double) this.sparkShower[index].dx)) * (double) byte.MaxValue));
          this.sparkShower[index].c.G = (byte) (0.0 + Math.Max(0.0, Math.Sin((this.totalTimeMS + 100.0) / (20.0 * Math.PI / (double) this.sparkShower[index].dx)) * (double) byte.MaxValue));
          if (this.sparkShower[index].c.R == (byte) 0)
            this.sparkShower[index].c.R = byte.MaxValue;
          if (this.sparkShower[index].c.G == (byte) 0)
            this.sparkShower[index].c.G = byte.MaxValue;
        }
        if ((double) this.sparkShower[index].y > (double) this.screenHeight)
          this.sparkShower.RemoveAt(index);
      }
      return false;
    }

    public void UpdateScoreState()
    {
      Game1.player.team.junimoKartStatus.UpdateState(this.score.ToString());
      this.scoreUpdateTimer = 1f;
    }

    public int GetValidCheckpointPosition(int x_pos)
    {
      int num1;
      for (num1 = 0; num1 < 16 && this.GetTrackForXPosition((float) (x_pos * this.tileSize)) == null; ++num1)
        --x_pos;
      for (; num1 < 16; ++num1)
      {
        if (this.GetTrackForXPosition((float) (x_pos * this.tileSize)) == null)
        {
          ++x_pos;
          break;
        }
        --x_pos;
      }
      if (this.GetTrackForXPosition((float) (x_pos * this.tileSize)) == null)
        return this.furthestGeneratedCheckpoint;
      int checkpointPosition = x_pos;
      int num2 = (int) ((double) this.GetTrackForXPosition((float) (x_pos * this.tileSize)).position.Y / (double) this.tileSize);
      ++x_pos;
      int num3 = 0;
      for (int index = 0; index < 16; ++index)
      {
        MineCart.Track trackForXposition = this.GetTrackForXPosition((float) (x_pos * this.tileSize));
        if (trackForXposition == null)
          return this.furthestGeneratedCheckpoint;
        if (Math.Abs((int) ((double) trackForXposition.position.Y / (double) this.tileSize) - num2) <= 1)
        {
          ++num3;
          if (num3 >= 3)
            return checkpointPosition;
        }
        else
        {
          num3 = 0;
          checkpointPosition = x_pos;
          num2 = (int) ((double) this.GetTrackForXPosition((float) (x_pos * this.tileSize)).position.Y / (double) this.tileSize);
        }
        ++x_pos;
      }
      return this.furthestGeneratedCheckpoint;
    }

    public virtual void CollectFruit(MineCart.CollectableFruits fruit_type)
    {
      this._collectedFruit.Add(fruit_type);
      if (this.gameMode == 3)
      {
        this.CollectCoin(10);
      }
      else
      {
        this.score += 1000;
        this.UpdateScoreState();
      }
    }

    public virtual void CollectCoin(int amount)
    {
      if (this.gameMode == 3)
      {
        this.coinCount += amount;
        if (this.coinCount < 100)
          return;
        Game1.playSound("yoba");
        int num = this.coinCount / 100;
        this.coinCount %= 100;
        this.livesLeft += num;
      }
      else
      {
        this.score += 30;
        this.UpdateScoreState();
      }
    }

    public void Die()
    {
      if (this.respawnCounter > 0 || (double) this.deathTimer > 0.0 || this.reachedFinish || !this.player.enabled)
        return;
      this.player.OnDie();
      this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(16, 96, 16, 16), this.player.position, (float) Game1.random.Next(-80, 81), (float) Game1.random.Next(-100, -49), life_time: 1f));
      this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(32, 96, 16, 16), this.player.position + new Vector2(0.0f, -this.player.characterExtraHeight), (float) Game1.random.Next(-80, 81), (float) Game1.random.Next(-150, -99), 0.1f, life_time: 1f, scale: 0.6666667f)).SetColor(Color.Lime);
      this.player.position.Y = -1000f;
      Game1.playSound("fishEscape");
      this.player.enabled = false;
      this.lastLevelWasPerfect = false;
      this.completelyPerfect = false;
      if (this.gameState == MineCart.GameStates.Cutscene)
        return;
      --this.livesLeft;
      if (this.gameMode != 3 || this.livesLeft < 0)
      {
        this.gameOver = true;
        this.fadeDelta = 1f;
        if (this.gameMode != 2)
          return;
        if (Game1.player.team.junimoKartScores.GetScores()[0].Value < this.score)
          Game1.multiplayer.globalChatInfoMessage("JunimoKartHighScore", Game1.player.Name);
        Game1.player.team.junimoKartScores.AddScore((string) (NetFieldBase<string, NetString>) Game1.player.name, this.score);
        if (Game1.player.team.specialOrders != null)
        {
          foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
          {
            if (specialOrder.onJKScoreAchieved != null)
              specialOrder.onJKScoreAchieved(Game1.player, this.score);
          }
        }
        this.RefreshHighScore();
      }
      else
      {
        this.player.position.X = this.checkpointPosition;
        for (int index = 0; index < 6; ++index)
        {
          MineCart.Track trackForXposition = this.GetTrackForXPosition((this.checkpointPosition / (float) this.tileSize + (float) index) * (float) this.tileSize);
          if (trackForXposition != null && trackForXposition.obstacle != null)
          {
            trackForXposition.obstacle.Destroy();
            trackForXposition.obstacle = (MineCart.Obstacle) null;
          }
        }
        this.player.SnapToFloor();
        this.deathTimer = 0.25f;
        this.respawnCounter = 1400;
      }
    }

    public void ReapEntities()
    {
      float num1 = this.checkpointPosition - 96f - (float) (4 * this.tileSize);
      int num2 = 0;
      foreach (int key in new List<int>((IEnumerable<int>) this._tracks.Keys))
      {
        if ((double) key < (double) num1 / (double) this.tileSize)
        {
          for (int index = 0; index < this._tracks[key].Count; ++index)
            this._entities.Remove((MineCart.Entity) this._tracks[key][index]);
          this._tracks.Remove(key);
          ++num2;
        }
      }
    }

    public void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public void releaseLeftClick(int x, int y)
    {
    }

    public void releaseRightClick(int x, int y)
    {
    }

    public void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public void receiveKeyPress(Keys k)
    {
      if (Game1.input.GetGamePadState().IsButtonDown(Buttons.Back) || k.Equals((object) Keys.Escape))
      {
        this.QuitGame();
      }
      else
      {
        if (!k.Equals((object) Keys.P) && !k.Equals((object) Keys.Enter) && (!Game1.options.gamepadControls || !Game1.input.GetGamePadState().IsButtonDown(Buttons.Start) || !this.CanPause()))
          return;
        this.gamePaused = !this.gamePaused;
        if (this.gamePaused)
          Game1.playSound("bigSelect");
        else
          Game1.playSound("bigDeSelect");
      }
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void ResetState()
    {
      this.gameOver = false;
      this.screenLeftBound = 0.0f;
      this.respawnCounter = 0;
      this.deathTimer = 0.0f;
      this._spawnedFruit = new HashSet<MineCart.CollectableFruits>();
      this.sparkShower.Clear();
      this._goalIndicator = (MineCart.GoalIndicator) null;
      this.checkpointPositions = new List<int>();
      this._tracks = new Dictionary<int, List<MineCart.Track>>();
      this._entities = new List<MineCart.Entity>();
      this.player = (MineCart.MineCartCharacter) this.AddEntity<MineCart.PlayerMineCartCharacter>(new MineCart.PlayerMineCartCharacter());
      this.player.position.X = 0.0f;
      this.player.position.Y = (float) (this.ytileOffset * this.tileSize);
      this.generatorPosition.X = 0;
      this.generatorPosition.Y = this.ytileOffset + 1;
      this._lastGenerator = (MineCart.BaseTrackGenerator) null;
      this._trackGenerator = (MineCart.BaseTrackGenerator) null;
      this._forcedNextGenerator = (MineCart.BaseTrackGenerator) null;
      this.trackBuilderCharacter = this.AddEntity<MineCart.MineCartCharacter>(new MineCart.MineCartCharacter());
      this.trackBuilderCharacter.visible = false;
      this.trackBuilderCharacter.enabled = false;
      this._lastTilePosition = 0;
      this.pauseBeforeTitleFadeOutTimer = 0.0f;
      this.lakeDecor.Clear();
      this.obstacles.Clear();
      this.reachedFinish = false;
    }

    public void QuitGame()
    {
      this.unload();
      Game1.playSound("bigDeSelect");
      Game1.currentMinigame = (IMinigame) null;
    }

    private void restartLevel(bool new_game = false)
    {
      if (new_game)
      {
        this.livesLeft = 3;
        this._collectedFruit.Clear();
        this.coinCount = 0;
        this.score = 0;
        this.levelsBeat = 0;
      }
      this.ResetState();
      if (this.levelsBeat > 0 && this._collectedFruit.Count > 0 || this.livesLeft < 3 && !new_game)
        this.ShowFruitsSummary();
      else
        this.ShowMap();
    }

    public void ShowFruitsSummary()
    {
      Game1.changeMusicTrack("none", music_context: Game1.MusicContext.MiniGame);
      if (!this.minecartLoop.IsPaused)
        this.minecartLoop.Pause();
      this.gameState = MineCart.GameStates.FruitsSummary;
      this.player.enabled = false;
      this.stateTimer = 0.75f;
    }

    public void ShowMap()
    {
      if (this.gameMode == 2)
      {
        this.ShowCutscene();
      }
      else
      {
        this.gameState = MineCart.GameStates.Map;
        this.mapTimer = 0.0f;
        this.screenDarkness = 1f;
        this.ResetState();
        this.player.enabled = false;
        Game1.changeMusicTrack("none", music_context: Game1.MusicContext.MiniGame);
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(256, 864, 16, 16), new Vector2(261f, 106f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, animation_interval: 0.15f, draw_depth: 0.2f)
        {
          ySinWaveMagnitude = (float) Game1.random.Next(1, 6)
        });
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(256, 864, 16, 16), new Vector2(276f, 117f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, animation_interval: 0.17f, draw_depth: 0.2f)
        {
          ySinWaveMagnitude = (float) Game1.random.Next(1, 6)
        });
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(256, 864, 16, 16), new Vector2(234f, 136f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, animation_interval: 0.19f, draw_depth: 0.2f)
        {
          ySinWaveMagnitude = (float) Game1.random.Next(1, 6)
        });
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(256, 864, 16, 16), new Vector2(264f, 131f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, animation_interval: 0.19f, draw_depth: 0.2f)
        {
          ySinWaveMagnitude = (float) Game1.random.Next(1, 6)
        });
        if (Game1.random.NextDouble() < 0.4)
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(256, 864, 16, 16), new Vector2(247f, 119f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, animation_interval: 0.19f, draw_depth: 0.2f)
          {
            ySinWaveMagnitude = (float) Game1.random.Next(1, 6)
          });
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(96, 864, 16, 16), new Vector2(327f, 186f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, animation_interval: 0.17f, draw_depth: 0.55f));
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(96, 864, 16, 16), new Vector2(362f, 190f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, animation_interval: 0.19f, draw_depth: 0.55f));
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(96, 864, 16, 16), new Vector2(299f, 197f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, animation_interval: 0.21f, draw_depth: 0.55f));
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(96, 864, 16, 16), new Vector2(375f, 212f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, animation_interval: 0.16f, draw_depth: 0.55f));
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(402, 660, 100, 72), new Vector2(205f, 184f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 2, animation_interval: 0.765f, draw_depth: 0.55f));
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(0, 736, 48, 50), new Vector2(280f, 66f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 2, animation_interval: 0.765f, draw_depth: 0.55f));
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(402, 638, 3, 21), new Vector2(234.66f, 66.66f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.55f));
        if (this.currentTheme == 0)
        {
          this.AddEntity<MineCart.CosmeticFallingBoulder>(new MineCart.CosmeticFallingBoulder(72f, new Color(130, 96, 79), delayBeforeAppear: 0.45f)).position = new Vector2((float) (40 + Game1.random.Next(40)), -16f);
          if (Game1.random.NextDouble() < 0.5)
            this.AddEntity<MineCart.CosmeticFallingBoulder>(new MineCart.CosmeticFallingBoulder(72f, new Color(130, 96, 79), 80f, 0.5f)).position = new Vector2((float) (80 + Game1.random.Next(40)), -16f);
          if (Game1.random.NextDouble() < 0.5)
            this.AddEntity<MineCart.CosmeticFallingBoulder>(new MineCart.CosmeticFallingBoulder(72f, new Color(130, 96, 79), 88f, 0.55f)).position = new Vector2((float) (120 + Game1.random.Next(40)), -16f);
        }
        else if (this.currentTheme == 1)
        {
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 604, 15, 12), new Vector2(119f, 162f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 0.8f, draw_depth: 0.55f)).SetDestroySound("boulderBreak");
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 604, 15, 12), new Vector2(49f, 166f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 1.2f, draw_depth: 0.55f)).SetDestroySound("boulderBreak");
          for (int index = 0; index < 4; ++index)
            this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(421, 607, 5, 5), new Vector2(119f, 162f), (float) Game1.random.Next(-30, 31), (float) Game1.random.Next(-50, -39), 0.25f, life_time: 0.75f, animation_interval: 1f, timeBeforeDisplay: 0.8f));
          for (int index = 0; index < 4; ++index)
            this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(421, 607, 5, 5), new Vector2(49f, 166f), (float) Game1.random.Next(-30, 31), (float) Game1.random.Next(-50, -39), 0.25f, life_time: 0.75f, animation_interval: 1f, timeBeforeDisplay: 1.2f));
        }
        else if (this.currentTheme == 3)
        {
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(455, 512, 58, 64), new Vector2(250f, 136f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 0.8f, draw_depth: 0.21f)).SetDestroySound("barrelBreak");
          for (int index = 0; index < 32; ++index)
            this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(51, 53, 9, 9), new Vector2(250f, 136f) + new Vector2((float) Game1.random.Next(-20, 31), (float) Game1.random.Next(-20, 21)), (float) Game1.random.Next(-30, 31), (float) Game1.random.Next(-70, -39), 0.25f, life_time: 0.75f, animation_interval: 1f, timeBeforeDisplay: ((float) (0.800000011920929 + 0.00999999977648258 * (double) index))));
        }
        else if (this.currentTheme == 2)
        {
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(416, 368, 24, 16), new Vector2(217f, 177f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.54f, holdLastFrame: true, timeBeforeDisplay: 0.8f));
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(416, 368, 1, 1), new Vector2(217f, 177f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 0.8f, draw_depth: 0.55f)).SetDestroySound("pullItemFromWater");
        }
        else if (this.currentTheme == 4)
        {
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 591, 12, 11), new Vector2(328f, 197f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.34f, timeBeforeDisplay: 2.5f)).SetStartSound("fireball");
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 591, 12, 11), new Vector2(336f, 197f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.35f, timeBeforeDisplay: 2.625f));
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 591, 12, 11), new Vector2(344f, 197f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.34f, timeBeforeDisplay: 2.75f)).SetStartSound("fireball");
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 591, 12, 11), new Vector2(344f, 189f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.35f, timeBeforeDisplay: 2.825f));
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 591, 12, 11), new Vector2(344f, 181f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.34f, timeBeforeDisplay: 3f)).SetStartSound("fireball");
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 591, 12, 11), new Vector2(344f, 173f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.35f, timeBeforeDisplay: 3.125f));
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 591, 12, 11), new Vector2(344f, 165f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.34f, timeBeforeDisplay: 3.25f)).SetStartSound("fireball");
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 591, 12, 11), new Vector2(352f, 165f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.35f, timeBeforeDisplay: 3.325f));
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 591, 12, 11), new Vector2(360f, 165f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.34f, timeBeforeDisplay: 3.5f)).SetStartSound("fireball");
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 591, 12, 11), new Vector2(360f, 157f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.35f, timeBeforeDisplay: 3.625f));
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 591, 12, 11), new Vector2(360f, 149f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 4, draw_depth: 0.34f, timeBeforeDisplay: 3.75f)).SetStartSound("fireball");
        }
        else if (this.currentTheme == 5)
        {
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(416, 384, 16, 16), new Vector2(213f, 34f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 5f, num_animation_frames: 6, draw_depth: 0.55f)).SetDestroySound("slimedead");
          for (int index = 0; index < 8; ++index)
            this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(427, 607, 6, 6), new Vector2((float) (205 + Game1.random.Next(3, 14)), (float) (26 + Game1.random.Next(6, 14))), (float) Game1.random.Next(-30, 31), (float) Game1.random.Next(-60, -39), 0.25f, life_time: 0.75f, animation_interval: 1f, timeBeforeDisplay: ((float) (5.0 + (double) index * 0.00499999988824129))));
        }
        if (this.currentTheme == 9)
        {
          for (int index = 0; index < 8; ++index)
            this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(368, 784, 16, 16), new Vector2((float) (274 + Game1.random.Next(-19, 20)), (float) (46 + Game1.random.Next(6, 14))), (float) Game1.random.Next(-4, 5), -16f, gravity_multiplier: 0.05f, life_time: 2f, num_animation_frames: 3, animation_interval: 0.33f, draw_depth: 0.35f, holdLastFrame: true, timeBeforeDisplay: ((float) (1.0 + (double) index * 0.100000001490116)))).SetStartSound("dirtyHit");
        }
        else if (this.currentTheme == 6)
        {
          for (int index = 0; index < 52; ++index)
            this.AddEntity<MineCart.CosmeticFallingBoulder>(new MineCart.CosmeticFallingBoulder((float) Game1.random.Next(72, 195), new Color(100, 66, 49), (float) (96 + Game1.random.Next(-10, 11)), (float) (0.649999976158142 + (double) index * 0.0500000007450581))).position = new Vector2((float) (5 + Game1.random.Next(360)), -16f);
        }
        if (!this.levelThemesFinishedThisRun.Contains(1))
        {
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 604, 15, 12), new Vector2(119f, 162f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, draw_depth: 0.55f));
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(401, 604, 15, 12), new Vector2(49f, 166f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, draw_depth: 0.55f));
        }
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(415, this.levelThemesFinishedThisRun.Contains(0) ? 630 : 650, 10, 9), new Vector2(88f, 87.66f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 5, draw_depth: 0.55f));
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(415, this.levelThemesFinishedThisRun.Contains(1) ? 630 : 650, 10, 9), new Vector2(105f, 183.66f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 5, draw_depth: 0.55f));
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(415, this.levelThemesFinishedThisRun.Contains(5) ? 630 : 640, 10, 9), new Vector2(169f, 119.66f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 5, draw_depth: 0.55f));
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(415, this.levelThemesFinishedThisRun.Contains(4) ? 630 : 650, 10, 9), new Vector2(328f, 199.66f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 5, draw_depth: 0.55f));
        this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(415, this.levelThemesFinishedThisRun.Contains(6) ? 630 : 650, 10, 9), new Vector2(361f, 72.66f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, num_animation_frames: 5, draw_depth: 0.55f));
        if (this.levelThemesFinishedThisRun.Contains(2))
          this.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(466, 642, 17, 17), new Vector2(216.66f, 200.66f), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 99f, animation_interval: 0.17f, draw_depth: 0.52f));
        this.fadeDelta = -1f;
        MineCart.MapJunimo mapJunimo = this.AddEntity<MineCart.MapJunimo>(new MineCart.MapJunimo());
        foreach (MineCart.LevelTransition levelTransition in this.LEVEL_TRANSITIONS)
        {
          if (levelTransition.startLevel == this.currentTheme && (levelTransition.shouldTakePath == null || levelTransition.shouldTakePath()))
          {
            mapJunimo.position = new Vector2(((float) levelTransition.startGridCoordinates.X + 0.5f) * (float) this.tileSize, ((float) levelTransition.startGridCoordinates.Y + 0.5f) * (float) this.tileSize);
            mapJunimo.moveString = levelTransition.pathString;
            this.currentTheme = levelTransition.destinationLevel;
            break;
          }
        }
      }
    }

    public void ShowCutscene()
    {
      this.gameState = MineCart.GameStates.Cutscene;
      this.screenDarkness = 1f;
      this.ResetState();
      this.player.enabled = false;
      this.setGameModeParameters();
      this.setUpTheme(this.currentTheme);
      this.cutsceneTick = 0;
      Game1.changeMusicTrack("none", music_context: Game1.MusicContext.MiniGame);
      for (int x = 0; x < this.screenWidth / this.tileSize + 4; ++x)
        this.AddTrack(x, 10).visible = false;
      this.player.SnapToFloor();
      if (this.gameMode != 2)
        return;
      this.EndCutscene();
    }

    public void PlayLevelMusic()
    {
      if (this.currentTheme == 0)
        Game1.changeMusicTrack("EarthMine", music_context: Game1.MusicContext.MiniGame);
      else if (this.currentTheme == 1)
        Game1.changeMusicTrack("FrostMine", music_context: Game1.MusicContext.MiniGame);
      else if (this.currentTheme == 2)
        Game1.changeMusicTrack("junimoKart_whaleMusic", music_context: Game1.MusicContext.MiniGame);
      else if (this.currentTheme == 4)
        Game1.changeMusicTrack("tribal", music_context: Game1.MusicContext.MiniGame);
      else if (this.currentTheme == 3)
        Game1.changeMusicTrack("junimoKart_ghostMusic", music_context: Game1.MusicContext.MiniGame);
      else if (this.currentTheme == 5)
        Game1.changeMusicTrack("junimoKart_slimeMusic", music_context: Game1.MusicContext.MiniGame);
      else if (this.currentTheme == 9)
        Game1.changeMusicTrack("junimoKart_mushroomMusic", music_context: Game1.MusicContext.MiniGame);
      else if (this.currentTheme == 6)
      {
        Game1.changeMusicTrack("nightTime", music_context: Game1.MusicContext.MiniGame);
      }
      else
      {
        if (this.currentTheme != 8)
          return;
        Game1.changeMusicTrack("Upper_Ambient", music_context: Game1.MusicContext.MiniGame);
      }
    }

    public void EndCutscene()
    {
      if (!this.minecartLoop.IsPaused)
        this.minecartLoop.Pause();
      this.gameState = MineCart.GameStates.Ingame;
      Utility.CollectGarbage();
      this.ResetState();
      this.setUpTheme(this.currentTheme);
      this.PlayLevelMusic();
      this.player.enabled = true;
      this.createBeginningOfLevel();
      this.player.position.X = (float) this.tileSize * 0.5f;
      this.player.SnapToFloor();
      this.checkpointPosition = this.player.position.X;
      this.furthestGeneratedCheckpoint = 0;
      this.lastLevelWasPerfect = true;
      this.secondsOnThisLevel = 0.0f;
      if (this.currentTheme == 2)
      {
        this.AddEntity<MineCart.Whale>(new MineCart.Whale());
        this.AddEntity<MineCart.PlayerBubbleSpawner>(new MineCart.PlayerBubbleSpawner());
      }
      if (this.currentTheme == 5)
        this.AddEntity<MineCart.HugeSlime>(new MineCart.HugeSlime()).position = new Vector2(0.0f, 0.0f);
      this.screenDarkness = 1f;
      this.fadeDelta = -1f;
      if (this.gameMode != 3 || this.currentTheme != 7)
        return;
      if (!Game1.player.hasOrWillReceiveMail("JunimoKart"))
        Game1.addMailForTomorrow("JunimoKart");
      Game1.multiplayer.globalChatInfoMessage("JunimoKart", Game1.player.Name);
      this.unload();
      Game1.globalFadeToClear((Game1.afterFadeFunction) (() => Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:MineCart.cs.12106"))), 0.015f);
      Game1.currentMinigame = (IMinigame) null;
      DelayedAction.playSoundAfterDelay("discoverMineral", 1000);
    }

    public void createSparkShower(Vector2 position)
    {
      int num = Game1.random.Next(3, 7);
      for (int index = 0; index < num; ++index)
        this.sparkShower.Add(new MineCart.Spark(position.X - 3f, position.Y, (float) Game1.random.Next(-200, 5) / 100f, (float) -Game1.random.Next(5, 150) / 100f));
    }

    public void createSparkShower()
    {
      int num = Game1.random.Next(3, 7);
      for (int index = 0; index < num; ++index)
        this.sparkShower.Add(new MineCart.Spark(this.player.drawnPosition.X - 3f, this.player.drawnPosition.Y, (float) Game1.random.Next(-200, 5) / 100f, (float) -Game1.random.Next(5, 150) / 100f));
    }

    public void createSparkShower(int number)
    {
      for (int index = 0; index < number; ++index)
        this.sparkShower.Add(new MineCart.Spark(this.player.drawnPosition.X - 3f, (float) ((double) this.player.position.Y * (double) this.tileSize + (double) this.tileSize - 4.0), (float) Game1.random.Next(-200, 5) / 100f, (float) -Game1.random.Next(5, 150) / 100f));
    }

    public void CreateLakeDecor()
    {
      for (int index = 0; index < 16; ++index)
        this.lakeDecor.Add(new MineCart.LakeDecor(this, this.currentTheme));
    }

    public void CreateBGDecor()
    {
      for (int forceXPosition = 0; forceXPosition < 16; ++forceXPosition)
        this.lakeDecor.Add(new MineCart.LakeDecor(this, this.currentTheme, true, forceXPosition));
    }

    public void createBeginningOfLevel()
    {
      this.CreateLakeDecor();
      for (int index = 0; index < 15; ++index)
      {
        this.AddTrack(this.generatorPosition.X, this.generatorPosition.Y);
        ++this.generatorPosition.X;
      }
    }

    public void setGameModeParameters()
    {
      switch (this.gameMode)
      {
        case 2:
          this.distanceToTravel = 150;
          break;
        case 3:
          this.distanceToTravel = 350;
          break;
      }
    }

    public void AddValidObstacle(MineCart.ObstacleTypes obstacle_type, Type type)
    {
      if (this._validObstacles == null)
        return;
      if (!this._validObstacles.ContainsKey(obstacle_type))
        this._validObstacles[obstacle_type] = new List<Type>();
      this._validObstacles[obstacle_type].Add(type);
    }

    public void setUpTheme(int whichTheme)
    {
      this._generatorRolls = new List<MineCart.GeneratorRoll>();
      this._validObstacles = new Dictionary<MineCart.ObstacleTypes, List<Type>>();
      float num1 = 0.0f;
      float num2 = 1f;
      if (this.gameState == MineCart.GameStates.Cutscene)
      {
        num1 = 0.0f;
        num2 = 1f;
      }
      else if (this.gameMode == 2)
      {
        int num3 = this.levelsBeat / this.infiniteModeLevels.Length;
        num1 = (float) num3 * 0.25f;
        num2 = (float) (1.0 + (double) num3 * 0.25);
      }
      this.midBGSource = new Rectangle(64, 0, 96, 162);
      this.backBGSource = new Rectangle(64, 162, 96, 111);
      this.lakeBGSource = new Rectangle(0, 80, 16, 97);
      this.backBGYOffset = this.tileSize * 2;
      this.midBGYOffset = 0;
      switch (whichTheme)
      {
        case 0:
          this.backBGTint = Color.DarkKhaki;
          this.midBGTint = Color.SandyBrown;
          this.caveTint = Color.SandyBrown;
          this.lakeTint = Color.MediumAquamarine;
          this.trackTint = Color.Beige;
          this.waterfallTint = Color.MediumAquamarine * 0.9f;
          this.trackShadowTint = new Color(60, 60, 60);
          this.player.velocity.X = 95f;
          NoiseGenerator.Amplitude = 2.0;
          NoiseGenerator.Frequency = 0.12;
          this.AddValidObstacle(MineCart.ObstacleTypes.Normal, typeof (MineCart.Roadblock));
          this.AddValidObstacle(MineCart.ObstacleTypes.Normal, typeof (MineCart.FallingBoulderSpawner));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.SmallGapGenerator(this).SetLength(1, 3).SetDepth(2, 2)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.25f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(2, 3).SetStaggerValues(-2, -1, 1, 2).SetNumberOfHops(2, 2).SetReleaseJumpChance(1f)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.3f, (MineCart.BaseTrackGenerator) new MineCart.SmallGapGenerator(this).SetLength(1, 1).SetDepth(-4, -2).AddPickupFunction<MineCart.SmallGapGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.SmallGapGenerator(this).SetLength(1, 4).SetDepth(-3, -3).AddPickupFunction<MineCart.SmallGapGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(1, 1).SetNumberOfHops(2, 2).SetReleaseJumpChance(1f).AddPickupFunction<MineCart.BunnyHopGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.5f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(2).SetStaggerChance(1f).SetStaggerValues(-3, -2, -1, 2).SetLength(2, 4).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Normal, -11, 0.3f + num1)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.015f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(2, 3).SetStaggerValues(-3, -4, 4, 3).SetNumberOfHops(1, 1).SetReleaseJumpChance(0.1f)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(1f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(1).SetStaggerChance(1f).SetStaggerValueRange(-1, 1).SetLength(3, 5).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Normal, -10, 0.3f + num1)));
          this.generatorPosition.Y = this.screenHeight / this.tileSize - 3;
          break;
        case 1:
          this.AddValidObstacle(MineCart.ObstacleTypes.Normal, typeof (MineCart.Roadblock));
          this.AddValidObstacle(MineCart.ObstacleTypes.Difficult, typeof (MineCart.Roadblock));
          MineCart.BaseTrackGenerator baseTrackGenerator = (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(2).SetStaggerChance(1f).SetStaggerValueRange(-1, 1).SetLength(4, 4).SetCheckpoint(true);
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.3f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(1, 1).SetNumberOfHops(2, 4).SetReleaseJumpChance(0.1f).SetStaggerValues(-2, -1).SetTrackType(MineCart.Track.TrackType.UpSlope), new Func<bool>(this._IsGeneratingOnLowerHalf), baseTrackGenerator));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.15f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(1, 1).SetNumberOfHops(2, 4).SetReleaseJumpChance(0.1f).SetStaggerValues(3, 2, 1).SetTrackType(MineCart.Track.TrackType.UpSlope), new Func<bool>(this._IsGeneratingOnUpperHalf), baseTrackGenerator));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.5f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(0).SetStaggerChance(1f).SetStaggerValues(1).SetLength(3, 5).AddPickupFunction<MineCart.StraightAwayGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.IceDownSlopesOnly)).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Normal, -12)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.3f, baseTrackGenerator));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(1f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(2).SetStaggerChance(1f).SetStaggerValueRange(-1, 1).SetLength(3, 6).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Difficult, -13, 0.5f + num1)));
          this.backBGTint = new Color(93, 242, (int) byte.MaxValue);
          this.midBGTint = Color.White;
          this.caveTint = new Color(230, 244, 254);
          this.lakeBGSource = new Rectangle(304, 0, 16, 0);
          this.lakeTint = new Color(147, 217, (int) byte.MaxValue);
          this.midBGSource = new Rectangle(320, 135, 96, 149);
          this.midBGYOffset = -13;
          this.waterfallTint = Color.LightCyan * 0.5f;
          this.trackTint = new Color(186, 240, (int) byte.MaxValue);
          this.player.velocity.X = 85f;
          NoiseGenerator.Amplitude = 2.8;
          NoiseGenerator.Frequency = 0.18;
          this.trackShadowTint = new Color(50, 145, 250);
          break;
        case 2:
          this.backBGTint = Color.White;
          this.midBGTint = Color.White;
          this.caveTint = Color.SlateGray;
          this.lakeTint = new Color(75, 104, 88);
          this.waterfallTint = Color.White * 0.0f;
          this.trackTint = new Color(100, 220, (int) byte.MaxValue);
          this.player.velocity.X = 85f;
          NoiseGenerator.Amplitude = 3.0;
          NoiseGenerator.Frequency = 0.15;
          this.trackShadowTint = new Color(32, 45, 180);
          this.midBGSource = new Rectangle(416, 0, 96, 69);
          this.backBGSource = new Rectangle(320, 0, 96, 135);
          this.backBGYOffset = 0;
          this.lakeBGSource = new Rectangle(304, 0, 16, 0);
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.SmallGapGenerator(this).SetLength(2, 5).SetDepth(-7, -3).AddPickupFunction<MineCart.SmallGapGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.SmallGapGenerator(this).SetLength(1, 3).SetDepth(100, 100)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(1f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(1).SetStaggerChance(1f).SetStaggerValues(2, -1, 0, 1, 2).SetLength(3, 5).SetCheckpoint(true)));
          this.CreateBGDecor();
          if (this.gameMode != 2)
          {
            this.distanceToTravel = 300;
            break;
          }
          break;
        case 3:
          this.backBGTint = new Color(60, 60, 60);
          this.midBGTint = new Color(60, 60, 60);
          this.caveTint = new Color(70, 70, 70);
          this.lakeTint = new Color(60, 70, 80);
          this.trackTint = Color.DimGray;
          this.waterfallTint = Color.Black * 0.0f;
          this.trackShadowTint = Color.Black;
          this.player.velocity.X = 120f;
          NoiseGenerator.Amplitude = 3.0;
          NoiseGenerator.Frequency = 0.2;
          this.AddValidObstacle(MineCart.ObstacleTypes.Normal, typeof (MineCart.Roadblock));
          this.AddValidObstacle(MineCart.ObstacleTypes.Difficult, typeof (MineCart.WillOWisp));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.25f, (MineCart.BaseTrackGenerator) new MineCart.SmallGapGenerator(this).SetLength(3, 5).SetDepth(-10, -6)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.SmallGapGenerator(this).SetLength(1, 3).SetDepth(3, 3)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.25f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(2, 3).SetStaggerValues(4, 3).SetNumberOfHops(1, 1).SetReleaseJumpChance(0.0f)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.25f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(2).SetStaggerChance(1f).SetStaggerValues(-1, 0, 0, -1).SetLength(7, 9).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Difficult, -10).AddPickupFunction<MineCart.StraightAwayGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.EveryOtherTile)).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Normal, -13, 0.75f + num1)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(1f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(2).SetStaggerChance(1f).SetStaggerValues(4, -1, 0, 1, -4).SetLength(2, 6).AddPickupFunction<MineCart.StraightAwayGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.EveryOtherTile))));
          this.distanceToTravel = this.gameMode == 2 ? (int) ((double) this.distanceToTravel * 1.5) : 450;
          this.CreateBGDecor();
          break;
        case 4:
          this.AddValidObstacle(MineCart.ObstacleTypes.Normal, typeof (MineCart.FallingBoulderSpawner));
          this.backBGTint = new Color((int) byte.MaxValue, 137, 82);
          this.midBGTint = new Color((int) byte.MaxValue, 82, 40);
          this.caveTint = Color.DarkRed;
          this.lakeTint = Color.Red;
          this.lakeBGSource = new Rectangle(304, 97, 16, 97);
          this.trackTint = new Color((int) byte.MaxValue, 160, 160);
          this.waterfallTint = Color.Red * 0.9f;
          this.trackShadowTint = Color.Orange;
          this.player.velocity.X = 120f;
          NoiseGenerator.Amplitude = 3.0;
          NoiseGenerator.Frequency = 0.18;
          this._generatorRolls.Add(new MineCart.GeneratorRoll(1f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(1, 1).SetNumberOfHops(3, 5).SetStaggerValues(-3, -1, 1, 3).SetReleaseJumpChance(0.33f).AddPickupFunction<MineCart.BunnyHopGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(1f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(0).SetStaggerChance(1f).SetStaggerValues(-1, 1).SetLength(5, 8).AddPickupFunction<MineCart.StraightAwayGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always)).SetCheckpoint(true).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Normal, -13, 0.5f + num1)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(1f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(0).SetStaggerChance(1f).SetStaggerValues(-1, 1).SetLength(5, 8).AddPickupFunction<MineCart.StraightAwayGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always)).SetCheckpoint(true).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Normal, -13, 0.5f + num1)));
          break;
        case 5:
          this.AddValidObstacle(MineCart.ObstacleTypes.Air, typeof (MineCart.FallingBoulderSpawner));
          this.AddValidObstacle(MineCart.ObstacleTypes.Normal, typeof (MineCart.Roadblock));
          this.backBGTint = new Color(180, 250, 180);
          this.midBGSource = new Rectangle(416, 69, 96, 162);
          this.midBGTint = Color.White;
          this.caveTint = new Color((int) byte.MaxValue, 200, 60);
          this.lakeTint = new Color(24, 151, 62);
          this.trackTint = Color.LightSlateGray;
          this.waterfallTint = new Color(0, (int) byte.MaxValue, 180) * 0.5f;
          this.trackShadowTint = new Color(0, 180, 50);
          this.player.velocity.X = 100f;
          this.slimeBossSpeed = this.player.velocity.X;
          NoiseGenerator.Amplitude = 3.1;
          NoiseGenerator.Frequency = 0.24;
          this.lakeBGSource = new Rectangle(304, 0, 16, 0);
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(2, 3).SetStaggerValues(10, 10).SetNumberOfHops(1, 1).SetReleaseJumpChance(0.1f)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.SmallGapGenerator(this).SetLength(2, 5).SetDepth(-7, -3).AddPickupFunction<MineCart.SmallGapGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.25f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(0).SetStaggerChance(1f).SetStaggerValueRange(-1, -1).SetLength(3, 5).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Air, -11, 0.75f + num1).AddPickupFunction<MineCart.SmallGapGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(1, 1).SetStaggerValues(1, -2).SetNumberOfHops(2, 2).SetReleaseJumpChance(0.25f).AddPickupFunction<MineCart.BunnyHopGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always)).SetTrackType(MineCart.Track.TrackType.SlimeUpSlope)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(1f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(1).SetStaggerChance(1f).SetStaggerValues(-1, -1, 0, 2, 2).SetLength(3, 5).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Normal, -10, 0.3f + num1)));
          break;
        case 6:
          this.backBGTint = Color.White;
          this.midBGTint = Color.White;
          this.caveTint = Color.Black;
          this.lakeTint = Color.Black;
          this.waterfallTint = Color.BlueViolet * 0.25f;
          this.trackTint = new Color(150, 70, 120);
          this.player.velocity.X = 110f;
          NoiseGenerator.Amplitude = 3.5;
          NoiseGenerator.Frequency = 0.35;
          this.trackShadowTint = Color.Black;
          this.midBGSource = new Rectangle(416, 231, 96, 53);
          this.backBGSource = new Rectangle(320, 284, 96, 116);
          this.backBGYOffset = 20;
          this.AddValidObstacle(MineCart.ObstacleTypes.Normal, typeof (MineCart.Roadblock));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.25f, (MineCart.BaseTrackGenerator) new MineCart.RapidHopsGenerator(this).SetLength(3, 5).SetYStep(-1).AddPickupFunction<MineCart.RapidHopsGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.25f, (MineCart.BaseTrackGenerator) new MineCart.RapidHopsGenerator(this).SetLength(3, 5).SetYStep(2).SetChaotic(true).AddPickupFunction<MineCart.RapidHopsGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.RapidHopsGenerator(this).SetLength(3, 5).SetYStep(-2)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.05f, (MineCart.BaseTrackGenerator) new MineCart.RapidHopsGenerator(this).SetLength(3, 5).SetYStep(3)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(2, 3).SetStaggerValues(4, 3).SetNumberOfHops(1, 1).SetReleaseJumpChance(0.0f)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(1, 1).SetNumberOfHops(3, 5).SetStaggerValues(-3, -1, 1, 3).SetReleaseJumpChance(0.33f).AddPickupFunction<MineCart.BunnyHopGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(1f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(1).SetStaggerChance(1f).SetStaggerValueRange(-1, 2).SetLength(3, 8).AddPickupFunction<MineCart.StraightAwayGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.EveryOtherTile)).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Normal, -10, 0.75f + num1)));
          this.generatorPosition.Y = this.screenHeight / this.tileSize - 2;
          this.CreateBGDecor();
          if (this.gameMode != 2)
          {
            this.distanceToTravel = 500;
            break;
          }
          break;
        case 7:
          this.backBGTint = Color.DarkKhaki;
          this.midBGTint = Color.SandyBrown;
          this.caveTint = Color.SandyBrown;
          this.lakeTint = Color.MediumAquamarine;
          this.trackTint = Color.Beige;
          this.waterfallTint = Color.MediumAquamarine * 0.9f;
          this.trackShadowTint = new Color(60, 60, 60);
          this.player.velocity.X = 95f;
          break;
        case 8:
          this.backBGTint = new Color(10, 30, 50);
          this.midBGTint = Color.Black;
          this.caveTint = Color.Black;
          this.lakeTint = new Color(0, 60, 150);
          this.trackTint = new Color(0, 90, 180);
          this.waterfallTint = Color.MediumAquamarine * 0.0f;
          this.trackShadowTint = new Color(0, 0, 60);
          this.player.velocity.X = 100f;
          this.generatorPosition.Y = this.screenHeight / this.tileSize - 4;
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.SmallGapGenerator(this).SetLength(1, 3).SetDepth(2, 2).AddPickupFunction<MineCart.SmallGapGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.25f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(2, 3).SetStaggerValues(-2, -1, 1, 2).SetNumberOfHops(2, 2).SetReleaseJumpChance(1f).AddPickupFunction<MineCart.BunnyHopGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.3f, (MineCart.BaseTrackGenerator) new MineCart.SmallGapGenerator(this).SetLength(1, 1).SetDepth(-4, -2).AddPickupFunction<MineCart.SmallGapGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.SmallGapGenerator(this).SetLength(1, 4).SetDepth(-3, -3).AddPickupFunction<MineCart.SmallGapGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(1, 1).SetNumberOfHops(2, 2).SetReleaseJumpChance(1f).AddPickupFunction<MineCart.BunnyHopGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.5f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(2).SetStaggerChance(1f).SetStaggerValues(-3, -2, -1, 2).SetLength(2, 4).AddPickupFunction<MineCart.StraightAwayGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.015f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(2, 3).SetStaggerValues(-3, -4, 4, 3).SetNumberOfHops(1, 1).SetReleaseJumpChance(0.1f).AddPickupFunction<MineCart.BunnyHopGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(1f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(1).SetStaggerChance(1f).SetStaggerValueRange(-1, 1).SetLength(3, 5).AddPickupFunction<MineCart.StraightAwayGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          if (this.gameMode != 2)
          {
            this.distanceToTravel = 200;
            break;
          }
          break;
        case 9:
          this.AddValidObstacle(MineCart.ObstacleTypes.Difficult, typeof (MineCart.NoxiousMushroom));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.MushroomBalanceTrackGenerator(this).SetHopSize(2, 2).SetReleaseJumpChance(1f).SetStaggerValues(0, -1, 3).SetTrackType(MineCart.Track.TrackType.Straight)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.15f, (MineCart.BaseTrackGenerator) new MineCart.MushroomBalanceTrackGenerator(this).SetHopSize(1, 1).SetReleaseJumpChance(1f).SetStaggerValues(-2, 4).SetTrackType(MineCart.Track.TrackType.Straight)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.2f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(1).SetStaggerChance(1f).SetStaggerValues(-1, 0, 1).SetLength(4, 4).SetCheckpoint(true)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.25f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(2, 3).SetStaggerValues(4, 3).SetNumberOfHops(1, 1).SetReleaseJumpChance(0.0f)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.1f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(2).SetStaggerChance(0.0f).SetLength(11, 11).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Difficult, 3).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Difficult, 7).SetCheckpoint(false)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.25f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(2).SetStaggerChance(0.0f).SetLength(7, 7).AddObstacle<MineCart.StraightAwayGenerator>(MineCart.ObstacleTypes.Difficult, 3).SetCheckpoint(false)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.2f, (MineCart.BaseTrackGenerator) new MineCart.MushroomBunnyHopGenerator(this).SetHopSize(1, 1).SetNumberOfHops(2, 3).SetStaggerValues(-3, -1, 2, 3).SetReleaseJumpChance(0.25f).AddPickupFunction<MineCart.MushroomBunnyHopGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.05f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetHopSize(1, 1).SetNumberOfHops(2, 3).SetStaggerValues(-3, -1, 2, 3).SetReleaseJumpChance(0.33f).AddPickupFunction<MineCart.BunnyHopGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.35f, (MineCart.BaseTrackGenerator) new MineCart.BunnyHopGenerator(this).SetTrackType(MineCart.Track.TrackType.MushroomMiddle).SetHopSize(1, 1).SetNumberOfHops(2, 3).SetStaggerValues(-3, -4, 4).SetReleaseJumpChance(0.33f).AddPickupFunction<MineCart.BunnyHopGenerator>(new Func<MineCart.Track, MineCart.BaseTrackGenerator, bool>(MineCart.BaseTrackGenerator.Always))));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(0.5f, (MineCart.BaseTrackGenerator) new MineCart.MushroomBalanceTrackGenerator(this).SetHopSize(1, 1).SetReleaseJumpChance(1f).SetStaggerValues(-2, 4).SetTrackType(MineCart.Track.TrackType.Straight)));
          this._generatorRolls.Add(new MineCart.GeneratorRoll(1f, (MineCart.BaseTrackGenerator) new MineCart.StraightAwayGenerator(this).SetMinimumDistanceBetweenStaggers(1).SetStaggerChance(1f).SetStaggerValues(2, -1, 0, 1, 2).SetLength(3, 5).SetCheckpoint(true)));
          this.CreateBGDecor();
          this.backBGTint = Color.White;
          this.backBGSource = new Rectangle(0, 789, 96, 111);
          this.midBGTint = Color.White;
          this.caveTint = Color.Purple;
          this.lakeBGSource = new Rectangle(304, 0, 16, 0);
          this.lakeTint = new Color(0, 8, 46);
          this.midBGSource = new Rectangle(416, 736, 96, 149);
          this.midBGYOffset = -13;
          this.waterfallTint = new Color(100, 0, 140) * 0.5f;
          this.trackTint = new Color(130, 50, 230);
          this.player.velocity.X = 120f;
          this.trackShadowTint = new Color(0, 225, 225);
          break;
      }
      this.player.velocity.X *= num2;
      this.trackBuilderCharacter.velocity = this.player.velocity;
      this.currentTheme = whichTheme;
    }

    public int KeepTileInBounds(int y)
    {
      if (y < this.topTile)
        return 4;
      return y > this.bottomTile ? this.bottomTile : y;
    }

    public bool IsTileInBounds(int y) => y >= this.topTile && y <= this.bottomTile;

    public T GetOverlap<T>(MineCart.ICollideable source) where T : MineCart.Entity
    {
      List<T> objList = new List<T>();
      Rectangle bounds1 = source.GetBounds();
      foreach (MineCart.Entity entity in this._entities)
      {
        if (entity.IsActive() && entity is MineCart.ICollideable && entity is T)
        {
          Rectangle bounds2 = (entity as MineCart.ICollideable).GetBounds();
          if (bounds1.Intersects(bounds2))
            return entity as T;
        }
      }
      return default (T);
    }

    public List<T> GetOverlaps<T>(MineCart.ICollideable source) where T : MineCart.Entity
    {
      List<T> overlaps = new List<T>();
      Rectangle bounds1 = source.GetBounds();
      foreach (MineCart.Entity entity in this._entities)
      {
        if (entity.IsActive() && entity is MineCart.ICollideable && entity is T)
        {
          Rectangle bounds2 = (entity as MineCart.ICollideable).GetBounds();
          if (bounds1.Intersects(bounds2))
            overlaps.Add(entity as T);
        }
      }
      return overlaps;
    }

    public MineCart.Pickup CreatePickup(Vector2 position, bool fruit_only = false)
    {
      if ((double) position.Y < (double) this.tileSize && !fruit_only)
        return (MineCart.Pickup) null;
      MineCart.Pickup pickup = (MineCart.Pickup) null;
      int fruit_type = 0;
      for (int index = 0; index < 3 && this._spawnedFruit.Contains((MineCart.CollectableFruits) index); ++index)
        ++fruit_type;
      if (fruit_type <= 2)
      {
        float num = 0.0f;
        if (fruit_type == 0)
          num = 0.15f * (float) this.distanceToTravel * (float) this.tileSize;
        else if (fruit_type == 1)
          num = 0.48f * (float) this.distanceToTravel * (float) this.tileSize;
        else if (fruit_type == 2)
          num = 0.81f * (float) this.distanceToTravel * (float) this.tileSize;
        if ((double) position.X >= (double) num)
        {
          this._spawnedFruit.Add((MineCart.CollectableFruits) fruit_type);
          pickup = this.AddEntity<MineCart.Pickup>((MineCart.Pickup) new MineCart.Fruit((MineCart.CollectableFruits) fruit_type));
        }
      }
      if (pickup == null && !fruit_only)
        pickup = this.AddEntity<MineCart.Pickup>((MineCart.Pickup) new MineCart.Coin());
      if (pickup != null)
        pickup.position = position;
      return pickup;
    }

    public void draw(SpriteBatch b)
    {
      this._shakeOffset = new Vector2(Utility.Lerp(-this.shakeMagnitude, this.shakeMagnitude, (float) Game1.random.NextDouble()), Utility.Lerp(-this.shakeMagnitude, this.shakeMagnitude, (float) Game1.random.NextDouble()));
      if (this.gamePaused)
        this._shakeOffset = Vector2.Zero;
      Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
      Game1.isUsingBackToFrontSorting = true;
      b.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
      Rectangle screen = Utility.ConstrainScissorRectToScreen(new Rectangle((int) this.upperLeft.X, (int) this.upperLeft.Y, (int) ((double) this.screenWidth * (double) this.pixelScale), (int) ((double) this.screenHeight * (double) this.pixelScale)));
      b.GraphicsDevice.ScissorRectangle = screen;
      if (this.gameState != MineCart.GameStates.Map)
      {
        if (this.gameState == MineCart.GameStates.FruitsSummary)
        {
          if (this.perfectText != null)
            this.perfectText.draw(b, this.TransformDraw(new Vector2(80f, 40f)));
        }
        else if (this.gameState != MineCart.GameStates.Cutscene)
        {
          for (int index = 0; index <= this.screenWidth / this.tileSize + 1; ++index)
            b.Draw(this.texture, this.TransformDraw(new Rectangle(index * this.tileSize - (int) this.lakeSpeedAccumulator % this.tileSize, this.tileSize * 9, this.tileSize, this.screenHeight - 96)), new Rectangle?(this.lakeBGSource), this.lakeTint, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
          for (int index = 0; index < this.lakeDecor.Count; ++index)
            this.lakeDecor[index].Draw(b);
          for (int index = 0; index <= this.screenWidth / this.backBGSource.Width + 2; ++index)
            b.Draw(this.texture, this.TransformDraw(new Vector2(-this.backBGPosition + (float) (index * this.backBGSource.Width), (float) this.backBGYOffset)), new Rectangle?(this.backBGSource), this.backBGTint, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.7f);
          for (int index = 0; index < this.screenWidth / this.midBGSource.Width + 2; ++index)
            b.Draw(this.texture, this.TransformDraw(new Vector2(-this.midBGPosition + (float) (index * this.midBGSource.Width), (float) (162 - this.midBGSource.Height + this.midBGYOffset))), new Rectangle?(this.midBGSource), this.midBGTint, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.6f);
        }
      }
      foreach (MineCart.Entity entity in this._entities)
      {
        if (entity.IsOnScreen())
          entity.Draw(b);
      }
      foreach (MineCart.Spark spark in this.sparkShower)
        b.Draw(Game1.staminaRect, this.TransformDraw(new Rectangle((int) spark.x, (int) spark.y, 1, 1)), new Rectangle?(), spark.c, 0.0f, Vector2.Zero, SpriteEffects.None, 0.3f);
      int num1;
      if (this.gameState == MineCart.GameStates.Title)
      {
        b.Draw(this.texture, this.TransformDraw(new Vector2((float) (this.screenWidth / 2 - 128), (float) (this.screenHeight / 2 - 35))), new Rectangle?(new Rectangle(256, 409, 256, 71)), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.25f);
        if (this.gameMode == 2)
        {
          Vector2 vector2_1 = new Vector2(125f, 0.0f);
          Vector2 dest = new Vector2((float) (this.screenWidth / 2) - vector2_1.X / 2f, 155f);
          for (int index = 0; index < 5 && index < this._currentHighScores.Count; ++index)
          {
            Color color1 = Color.White;
            if (index == 0)
              color1 = Utility.GetPrismaticColor();
            KeyValuePair<string, int> currentHighScore = this._currentHighScores[index];
            SpriteFont dialogueFont1 = Game1.dialogueFont;
            num1 = currentHighScore.Value;
            string text1 = num1.ToString() ?? "";
            int x = (int) dialogueFont1.MeasureString(text1).X / 4;
            SpriteBatch spriteBatch1 = b;
            SpriteFont dialogueFont2 = Game1.dialogueFont;
            num1 = index + 1;
            string text2 = "#" + num1.ToString();
            Vector2 position1 = this.TransformDraw(dest);
            Color color2 = color1;
            Vector2 zero1 = Vector2.Zero;
            double scale1 = (double) this.GetPixelScale() / 4.0;
            spriteBatch1.DrawString(dialogueFont2, text2, position1, color2, 0.0f, zero1, (float) scale1, SpriteEffects.None, 0.199f);
            b.DrawString(Game1.dialogueFont, currentHighScore.Key, this.TransformDraw(dest + new Vector2(16f, 0.0f)), color1, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.199f);
            SpriteBatch spriteBatch2 = b;
            SpriteFont dialogueFont3 = Game1.dialogueFont;
            num1 = currentHighScore.Value;
            string text3 = num1.ToString() ?? "";
            Vector2 position2 = this.TransformDraw(dest + vector2_1 - new Vector2((float) x, 0.0f));
            Color color3 = color1;
            Vector2 zero2 = Vector2.Zero;
            double scale2 = (double) this.GetPixelScale() / 4.0;
            spriteBatch2.DrawString(dialogueFont3, text3, position2, color3, 0.0f, zero2, (float) scale2, SpriteEffects.None, 0.199f);
            Vector2 vector2_2 = new Vector2(1f, 1f);
            SpriteBatch spriteBatch3 = b;
            SpriteFont dialogueFont4 = Game1.dialogueFont;
            num1 = index + 1;
            string text4 = "#" + num1.ToString();
            Vector2 position3 = this.TransformDraw(dest + vector2_2);
            Color black1 = Color.Black;
            Vector2 zero3 = Vector2.Zero;
            double scale3 = (double) this.GetPixelScale() / 4.0;
            spriteBatch3.DrawString(dialogueFont4, text4, position3, black1, 0.0f, zero3, (float) scale3, SpriteEffects.None, 0.1999f);
            b.DrawString(Game1.dialogueFont, currentHighScore.Key, this.TransformDraw(dest + new Vector2(16f, 0.0f) + vector2_2), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.1999f);
            SpriteBatch spriteBatch4 = b;
            SpriteFont dialogueFont5 = Game1.dialogueFont;
            num1 = currentHighScore.Value;
            string text5 = num1.ToString() ?? "";
            Vector2 position4 = this.TransformDraw(dest + vector2_1 - new Vector2((float) x, 0.0f) + vector2_2);
            Color black2 = Color.Black;
            Vector2 zero4 = Vector2.Zero;
            double scale4 = (double) this.GetPixelScale() / 4.0;
            spriteBatch4.DrawString(dialogueFont5, text5, position4, black2, 0.0f, zero4, (float) scale4, SpriteEffects.None, 0.1999f);
            dest.Y += 10f;
          }
        }
      }
      else if (this.gameState == MineCart.GameStates.Map)
      {
        b.Draw(this.texture, this.TransformDraw(new Vector2(0.0f, 0.0f)), new Rectangle?(new Rectangle(0, 512, 400, 224)), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.6f);
        if (!this.levelThemesFinishedThisRun.Contains(3))
          b.Draw(this.texture, this.TransformDraw(new Vector2(221f, 104f)), new Rectangle?(new Rectangle(455, 512, 57, 64)), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.21f);
        b.Draw(this.texture, this.TransformDraw(new Vector2(369f, 51f)), new Rectangle?(new Rectangle(480, 579, 31, 32)), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.21f);
        b.Draw(this.texture, this.TransformDraw(new Vector2(109f, 198f)), new Rectangle?(new Rectangle(420, 512, 25, 26)), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.21f);
        b.Draw(this.texture, this.TransformDraw(new Vector2(229f, 213f)), new Rectangle?(new Rectangle(425, 541, 9, 11)), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.21f);
      }
      else if (this.gameState != MineCart.GameStates.FruitsSummary)
      {
        if (this.gameState == MineCart.GameStates.Cutscene)
        {
          double num2 = (double) this.GetPixelScale() / 4.0;
          b.DrawString(Game1.dialogueFont, this.cutsceneText, this.TransformDraw(new Vector2((float) (this.screenWidth / 2 - (int) ((double) Game1.dialogueFont.MeasureString(this.cutsceneText).X / 2.0 / 4.0)), 32f)), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.199f);
        }
        else
        {
          for (int index1 = 0; index1 < this.waterfallWidth; index1 += 2)
          {
            for (int index2 = -2; index2 <= this.screenHeight / this.tileSize + 1; ++index2)
              b.Draw(this.texture, this.TransformDraw(new Vector2((float) (this.screenWidth + this.tileSize * index1) - this.waterFallPosition, (float) (index2 * this.tileSize + (int) (this._totalTime * 48.0 + (double) (this.tileSize * 100)) % this.tileSize))), new Rectangle?(new Rectangle(48, 32, 16, 16)), this.waterfallTint, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.2f);
          }
        }
      }
      if (!this.gamePaused && (this.gameState == MineCart.GameStates.Ingame || this.gameState == MineCart.GameStates.Cutscene || this.gameState == MineCart.GameStates.FruitsSummary || this.gameState == MineCart.GameStates.Map))
      {
        this._shakeOffset = Vector2.Zero;
        Vector2 dest = new Vector2(4f, 4f);
        if (this.gameMode == 2)
        {
          string str = Game1.content.LoadString("Strings\\StringsFromCSFiles:MineCart.cs.12115");
          b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.10444", (object) this.score), this.TransformDraw(dest), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.1f);
          b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingGame.cs.10444", (object) this.score), this.TransformDraw(dest + new Vector2(1f, 1f)), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.11f);
          dest.Y += 10f;
          b.DrawString(Game1.dialogueFont, str + this.currentHighScore.ToString(), this.TransformDraw(dest), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.1f);
          b.DrawString(Game1.dialogueFont, str + this.currentHighScore.ToString(), this.TransformDraw(dest + new Vector2(1f, 1f)), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.11f);
        }
        else
        {
          dest.X = 4f;
          for (int index = 0; index < this.livesLeft; ++index)
          {
            b.Draw(this.texture, this.TransformDraw(dest), new Rectangle?(new Rectangle(160, 32, 16, 16)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.07f);
            b.Draw(this.texture, this.TransformDraw(dest + new Vector2(1f, 1f)), new Rectangle?(new Rectangle(160, 32, 16, 16)), Color.Black, 0.0f, new Vector2(0.0f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.071f);
            dest.X += 18f;
            if ((double) dest.X > 90.0 && index < this.livesLeft - 1)
            {
              dest.X = 4f;
              dest.Y += 18f;
            }
          }
          dest.X = 4f;
          dest.X += 36f;
          for (int livesLeft = this.livesLeft; livesLeft < 3; ++livesLeft)
          {
            b.Draw(this.texture, this.TransformDraw(dest), new Rectangle?(new Rectangle(160, 48, 16, 16)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.07f);
            b.Draw(this.texture, this.TransformDraw(dest + new Vector2(1f, 1f)), new Rectangle?(new Rectangle(160, 48, 16, 16)), Color.Black, 0.0f, new Vector2(0.0f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.071f);
            dest.X -= 18f;
          }
        }
        dest.X = 4f;
        dest.Y += 18f;
        for (int index = 0; index < 3; ++index)
        {
          Vector2 zero = Vector2.Zero;
          if ((double) this.currentFruitCheckMagnitude > 0.0 && index == this.currentFruitCheckIndex - 1)
          {
            zero.X = Utility.Lerp(-this.currentFruitCheckMagnitude, this.currentFruitCheckMagnitude, (float) Game1.random.NextDouble());
            zero.Y = Utility.Lerp(-this.currentFruitCheckMagnitude, this.currentFruitCheckMagnitude, (float) Game1.random.NextDouble());
          }
          if (this._collectedFruit.Contains((MineCart.CollectableFruits) index))
          {
            b.Draw(this.texture, this.TransformDraw(dest + zero), new Rectangle?(new Rectangle(160 + index * 16, 0, 16, 16)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.07f);
            b.Draw(this.texture, this.TransformDraw(dest + new Vector2(1f, 1f) + zero), new Rectangle?(new Rectangle(160 + index * 16, 0, 16, 16)), Color.Black, 0.0f, new Vector2(0.0f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.075f);
          }
          else
          {
            b.Draw(this.texture, this.TransformDraw(dest + zero), new Rectangle?(new Rectangle(160 + index * 16, 16, 16, 16)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.07f);
            b.Draw(this.texture, this.TransformDraw(dest + zero + new Vector2(1f, 1f)), new Rectangle?(new Rectangle(160 + index * 16, 16, 16, 16)), Color.Black, 0.0f, new Vector2(0.0f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.075f);
          }
          dest.X += 18f;
        }
        if (this.gameMode == 3)
        {
          dest.X = 4f;
          dest.Y += 18f;
          b.Draw(this.texture, this.TransformDraw(dest), new Rectangle?(new Rectangle(0, 272, 9, 11)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.07f);
          b.Draw(this.texture, this.TransformDraw(dest + new Vector2(1f, 1f)), new Rectangle?(new Rectangle(0, 272, 9, 11)), Color.Black, 0.0f, new Vector2(0.0f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.08f);
          dest.X += 12f;
          b.DrawString(Game1.dialogueFont, this.coinCount.ToString("00"), this.TransformDraw(dest), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.01f);
          b.DrawString(Game1.dialogueFont, this.coinCount.ToString("00"), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-3f, -3f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
          b.DrawString(Game1.dialogueFont, this.coinCount.ToString("00"), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-2f, -2f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
          b.DrawString(Game1.dialogueFont, this.coinCount.ToString("00"), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-1f, -1f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
          b.DrawString(Game1.dialogueFont, this.coinCount.ToString("00"), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-3.5f, -3.5f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
          b.DrawString(Game1.dialogueFont, this.coinCount.ToString("00"), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-1.5f, -1.5f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
          b.DrawString(Game1.dialogueFont, this.coinCount.ToString("00"), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-2.5f, -2.5f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
        }
        if (Game1.IsMultiplayer)
        {
          string timeOfDayString = Game1.getTimeOfDayString(Game1.timeOfDay);
          dest = new Vector2((float) ((double) this.screenWidth - (double) Game1.dialogueFont.MeasureString(timeOfDayString).X / 4.0 - 4.0), 4f);
          Color white = Color.White;
          b.DrawString(Game1.dialogueFont, Game1.getTimeOfDayString(Game1.timeOfDay), this.TransformDraw(dest), white, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.01f);
          b.DrawString(Game1.dialogueFont, Game1.getTimeOfDayString(Game1.timeOfDay), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-3f, -3f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
          b.DrawString(Game1.dialogueFont, Game1.getTimeOfDayString(Game1.timeOfDay), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-2f, -2f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
          b.DrawString(Game1.dialogueFont, Game1.getTimeOfDayString(Game1.timeOfDay), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-1f, -1f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
          b.DrawString(Game1.dialogueFont, Game1.getTimeOfDayString(Game1.timeOfDay), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-3.5f, -3.5f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
          b.DrawString(Game1.dialogueFont, Game1.getTimeOfDayString(Game1.timeOfDay), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-1.5f, -1.5f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
          b.DrawString(Game1.dialogueFont, Game1.getTimeOfDayString(Game1.timeOfDay), this.TransformDraw(dest + new Vector2(1f, 1f)) + new Vector2(-2.5f, -2.5f), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.02f);
        }
        if (this.gameState == MineCart.GameStates.Ingame)
        {
          float num3 = (float) (this.screenWidth - 192) / 2f;
          float b1 = num3 + 192f;
          dest = new Vector2(num3, 4f);
          for (int index = 0; index < 12; ++index)
          {
            Rectangle rectangle = new Rectangle(192, 48, 16, 16);
            if (index == 0)
              rectangle = new Rectangle(176, 48, 16, 16);
            else if (index >= 11)
              rectangle = new Rectangle(207, 48, 16, 16);
            b.Draw(this.texture, this.TransformDraw(dest), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.15f);
            b.Draw(this.texture, this.TransformDraw(dest + new Vector2(1f, 1f)), new Rectangle?(rectangle), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.17f);
            dest.X += 16f;
          }
          b.Draw(this.texture, this.TransformDraw(dest), new Rectangle?(new Rectangle(176, 64, 16, 16)), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale(), SpriteEffects.None, 0.15f);
          dest.X += 8f;
          num1 = this.levelsBeat + 1;
          string text = num1.ToString() ?? "";
          dest.Y += 3f;
          b.DrawString(Game1.dialogueFont, text, this.TransformDraw(dest - new Vector2((float) ((double) Game1.dialogueFont.MeasureString(text).X / 2.0 / 4.0), 0.0f)), Color.Black, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.1f);
          ++dest.X;
          ++dest.Y;
          dest = new Vector2(num3, 4f);
          if (this.player != null && this.player.visible)
            dest.X = Utility.Lerp(num3, b1, Math.Min(this.player.position.X / (float) (this.distanceToTravel * this.tileSize), 1f));
          b.Draw(this.texture, this.TransformDraw(dest), new Rectangle?(new Rectangle(240, 48, 16, 16)), Color.White, 0.0f, new Vector2(8f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.12f);
          b.Draw(this.texture, this.TransformDraw(dest + new Vector2(1f, 1f)), new Rectangle?(new Rectangle(240, 48, 16, 16)), Color.Black, 0.0f, new Vector2(8f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.13f);
          if ((double) this.checkpointPosition > (double) this.tileSize * 0.5)
          {
            dest.X = Utility.Lerp(num3, b1, this.checkpointPosition / (float) (this.distanceToTravel * this.tileSize));
            b.Draw(this.texture, this.TransformDraw(dest), new Rectangle?(new Rectangle(224, 48, 16, 16)), Color.White, 0.0f, new Vector2(8f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.125f);
            b.Draw(this.texture, this.TransformDraw(dest + new Vector2(1f, 1f)), new Rectangle?(new Rectangle(224, 48, 16, 16)), Color.Black, 0.0f, new Vector2(8f, 0.0f), this.GetPixelScale(), SpriteEffects.None, 0.135f);
          }
        }
      }
      if (this.gameMode == 2 && Game1.IsMultiplayer && this.gameState != MineCart.GameStates.Title)
        Game1.player.team.junimoKartStatus.Draw(b, this.TransformDraw(new Vector2(4f, (float) (this.screenHeight - 4))), this.GetPixelScale(), 0.01f, vertical_origin: PlayerStatusList.VerticalAlignment.Bottom);
      if ((double) this.screenDarkness > 0.0)
        b.Draw(Game1.staminaRect, this.TransformDraw(new Rectangle(0, 0, this.screenWidth, this.screenHeight + this.tileSize)), new Rectangle?(), Color.Black * this.screenDarkness, 0.0f, Vector2.Zero, SpriteEffects.None, 0.145f);
      if (this.gamePaused)
      {
        b.Draw(Game1.staminaRect, this.TransformDraw(new Rectangle(0, 0, this.screenWidth, this.screenHeight + this.tileSize)), new Rectangle?(), Color.Black * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.145f);
        string text = Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10378");
        b.DrawString(Game1.dialogueFont, text, this.TransformDraw(new Vector2()
        {
          X = (float) (this.screenWidth / 2),
          Y = (float) (this.screenHeight / 4)
        } - new Vector2((float) ((double) Game1.dialogueFont.MeasureString(text).X / 2.0 / 4.0), 0.0f)), Color.White, 0.0f, Vector2.Zero, this.GetPixelScale() / 4f, SpriteEffects.None, 0.1f);
      }
      if (!Game1.options.hardwareCursor && !Game1.options.gamepadControls)
        b.Draw(Game1.mouseCursors, new Vector2((float) Game1.getMouseX(), (float) Game1.getMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float) (4.0 + (double) Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 0.0001f);
      b.End();
      Game1.isUsingBackToFrontSorting = false;
      b.GraphicsDevice.ScissorRectangle = scissorRectangle;
    }

    public float GetPixelScale() => this.pixelScale;

    public Rectangle TransformDraw(Rectangle dest)
    {
      dest.X = (int) Math.Round(((double) dest.X + (double) this._shakeOffset.X) * (double) this.pixelScale) + (int) this.upperLeft.X;
      dest.Y = (int) Math.Round(((double) dest.Y + (double) this._shakeOffset.Y) * (double) this.pixelScale) + (int) this.upperLeft.Y;
      dest.Width = (int) ((double) dest.Width * (double) this.pixelScale);
      dest.Height = (int) ((double) dest.Height * (double) this.pixelScale);
      return dest;
    }

    public static int Mod(int x, int m) => (x % m + m) % m;

    public Vector2 TransformDraw(Vector2 dest)
    {
      dest.X = (float) ((int) Math.Round(((double) dest.X + (double) this._shakeOffset.X) * (double) this.pixelScale) + (int) this.upperLeft.X);
      dest.Y = (float) ((int) Math.Round(((double) dest.Y + (double) this._shakeOffset.Y) * (double) this.pixelScale) + (int) this.upperLeft.Y);
      return dest;
    }

    public void changeScreenSize()
    {
      this.screenWidth = 400;
      this.screenHeight = 220;
      float num = 1f / Game1.options.zoomLevel;
      int width = Game1.game1.localMultiplayerWindow.Width;
      int height = Game1.game1.localMultiplayerWindow.Height;
      this.pixelScale = (float) Math.Min(5, (int) Math.Floor((double) Math.Min((float) (width / this.screenWidth) * num, (float) (height / this.screenHeight) * num)));
      this.upperLeft = new Vector2((float) (width / 2) * num, (float) (height / 2) * num);
      this.upperLeft.X -= (float) (this.screenWidth / 2) * this.pixelScale;
      this.upperLeft.Y -= (float) (this.screenHeight / 2) * this.pixelScale;
      this.tileSize = 16;
      this.ytileOffset = this.screenHeight / 2 / this.tileSize;
    }

    public void unload()
    {
      Game1.stopMusicTrack(Game1.MusicContext.MiniGame);
      Game1.player.team.junimoKartStatus.WithdrawState();
      Game1.player.faceDirection(0);
      if (this.minecartLoop == null || !this.minecartLoop.IsPlaying)
        return;
      this.minecartLoop.Stop(AudioStopOptions.Immediate);
    }

    public bool forceQuit()
    {
      this.unload();
      return true;
    }

    public void leftClickHeld(int x, int y)
    {
    }

    public void receiveEventPoke(int data) => throw new NotImplementedException();

    public string minigameId() => nameof (MineCart);

    public bool doMainGameUpdates() => false;

    [XmlType("MineCart.GameStates")]
    public enum GameStates
    {
      Title,
      Ingame,
      FruitsSummary,
      Map,
      Cutscene,
    }

    public class LevelTransition
    {
      public int startLevel;
      public int destinationLevel;
      public Point startGridCoordinates;
      public string pathString = "";
      public Func<bool> shouldTakePath;

      public LevelTransition(
        int start_level,
        int destination_level,
        int start_grid_x,
        int start_grid_y,
        string path_string,
        Func<bool> should_take_path = null)
      {
        this.startLevel = start_level;
        this.destinationLevel = destination_level;
        this.startGridCoordinates = new Point(start_grid_x, start_grid_y);
        this.pathString = path_string;
        this.shouldTakePath = should_take_path;
      }
    }

    public enum CollectableFruits
    {
      Cherry,
      Orange,
      Grape,
      MAX,
    }

    public enum ObstacleTypes
    {
      Normal,
      Air,
      Difficult,
    }

    public class GeneratorRoll
    {
      public float chance;
      public MineCart.BaseTrackGenerator generator;
      public Func<bool> additionalGenerationCondition;
      public MineCart.BaseTrackGenerator forcedNextGenerator;

      public GeneratorRoll(
        float generator_chance,
        MineCart.BaseTrackGenerator track_generator,
        Func<bool> additional_generation_condition = null,
        MineCart.BaseTrackGenerator forced_next_generator = null)
      {
        this.chance = generator_chance;
        this.generator = track_generator;
        this.forcedNextGenerator = forced_next_generator;
        this.additionalGenerationCondition = additional_generation_condition;
      }
    }

    public class MapJunimo : MineCart.Entity
    {
      public int direction = 2;
      public string moveString = "";
      public float moveSpeed = 60f;
      public float pixelsToMove;
      public MineCart.MapJunimo.MoveState moveState;
      public float nextBump;
      public float bumpHeight;
      private bool isOnWater;

      public void StartMoving() => this.moveState = MineCart.MapJunimo.MoveState.Moving;

      protected override void _Update(float time)
      {
        int num1 = this.direction;
        this.isOnWater = false;
        if ((double) this.position.X > 194.0 && (double) this.position.X < 251.0 && (double) this.position.Y > 165.0)
        {
          this.isOnWater = true;
          this._game.minecartLoop.Pause();
        }
        if (this.moveString.Length > 0)
        {
          if (this.moveString[0] == 'u')
            num1 = 0;
          else if (this.moveString[0] == 'd')
            num1 = 2;
          else if (this.moveString[0] == 'l')
            num1 = 3;
          else if (this.moveString[0] == 'r')
            num1 = 1;
        }
        if (this.moveState == MineCart.MapJunimo.MoveState.Idle && !this._game.minecartLoop.IsPaused)
          this._game.minecartLoop.Pause();
        if (this.moveState == MineCart.MapJunimo.MoveState.Moving)
        {
          this.nextBump -= time;
          this.bumpHeight = Utility.MoveTowards(this.bumpHeight, 0.0f, time * 5f);
          if ((double) this.nextBump <= 0.0)
          {
            this.nextBump = Utility.RandomFloat(0.1f, 0.3f);
            this.bumpHeight = -2f;
          }
          if (!this.isOnWater && this._game.minecartLoop.IsPaused)
            this._game.minecartLoop.Resume();
          if ((double) this.pixelsToMove <= 0.0)
          {
            if (num1 != this.direction)
            {
              this.direction = num1;
              if (!this.isOnWater)
              {
                ICue cue = Game1.soundBank.GetCue("parry");
                this._game.createSparkShower(this.position);
                cue.Play();
              }
              else
                Game1.playSound("waterSlosh");
            }
            if (this.moveString.Length > 0)
            {
              this.pixelsToMove = 16f;
              this.moveString = this.moveString.Substring(1);
            }
            else
            {
              this.moveState = MineCart.MapJunimo.MoveState.Finished;
              this.direction = 2;
              if ((double) this.position.X < 368.0)
              {
                if (!this.isOnWater)
                {
                  ICue cue = Game1.soundBank.GetCue("parry");
                  this._game.createSparkShower(this.position);
                  cue.Play();
                }
                else
                  Game1.playSound("waterSlosh");
              }
            }
          }
          if ((double) this.pixelsToMove > 0.0)
          {
            float num2 = Math.Min(this.pixelsToMove, this.moveSpeed * time);
            Vector2 zero = Vector2.Zero;
            if (this.direction == 1)
              zero.X = 1f;
            else if (this.direction == 3)
              zero.X = -1f;
            if (this.direction == 0)
              zero.Y = -1f;
            if (this.direction == 2)
              zero.Y = 1f;
            this.position = this.position + zero * num2;
            this.pixelsToMove -= num2;
          }
        }
        else
          this.bumpHeight = -2f;
        if (this.moveState == MineCart.MapJunimo.MoveState.Finished && !this._game.minecartLoop.IsPaused)
          this._game.minecartLoop.Pause();
        base._Update(time);
      }

      public override void _Draw(SpriteBatch b)
      {
        SpriteEffects effects = SpriteEffects.None;
        Rectangle rectangle = new Rectangle(400, 512, 16, 16);
        if (this.direction == 0)
          rectangle.Y = 544;
        else if (this.direction == 2)
        {
          rectangle.Y = 512;
        }
        else
        {
          rectangle.Y = 528;
          if (this.direction == 3)
            effects = SpriteEffects.FlipHorizontally;
        }
        if (this.isOnWater)
        {
          rectangle.Height -= 3;
          b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + new Vector2(0.0f, -1f) + new Vector2(0.0f, 1f) * this.bumpHeight), new Rectangle?(rectangle), Color.White, 0.0f, new Vector2(8f, 8f), this._game.GetPixelScale(), effects, 0.45f);
          b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + new Vector2(2f, 10f) + new Vector2(0.0f, 1f) * this.bumpHeight), new Rectangle?(new Rectangle(414, 624, 13, 5)), Color.White, 0.0f, new Vector2(8f, 8f), this._game.GetPixelScale(), effects, 0.44f);
        }
        else
          b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + new Vector2(0.0f, -1f) + new Vector2(0.0f, 1f) * this.bumpHeight), new Rectangle?(rectangle), Color.White, 0.0f, new Vector2(8f, 8f), this._game.GetPixelScale(), effects, 0.45f);
      }

      public enum MoveState
      {
        Idle,
        Moving,
        Finished,
      }
    }

    public class LakeDecor
    {
      public Point _position;
      public int spriteIndex;
      protected MineCart _game;
      public int _lastCycle = -1;
      public bool _bgDecor;
      private int _animationFrames = 1;

      public LakeDecor(MineCart game, int theme = -1, bool bgDecor = false, int forceXPosition = -1)
      {
        this._game = game;
        this._position = new Point(Game1.random.Next(0, this._game.screenWidth), Game1.random.Next(160, this._game.screenHeight));
        if (forceXPosition != -1)
          this._position.X = forceXPosition * (this._game.screenWidth / 16) + Game1.random.Next(0, this._game.screenWidth / 16);
        this._bgDecor = bgDecor;
        this.spriteIndex = Game1.random.Next(2);
        switch (theme)
        {
          case 1:
            this.spriteIndex += 3;
            break;
          case 2:
            this.spriteIndex = 2;
            break;
          case 4:
            this.spriteIndex = 14;
            this._animationFrames = 6;
            break;
          case 5:
            this.spriteIndex += 5;
            break;
          case 6:
            this.spriteIndex = 1;
            break;
          case 9:
            this.spriteIndex += 7;
            break;
        }
        if (!bgDecor)
          return;
        this.spriteIndex += 7;
        this._position.Y = Game1.random.Next(0, this._game.screenHeight / 3);
        if (theme == 2 && forceXPosition % 5 == 0)
        {
          ++this.spriteIndex;
          this._animationFrames = 4;
        }
        else
        {
          switch (theme)
          {
            case 3:
              this.spriteIndex = 24;
              this._animationFrames = 4;
              break;
            case 6:
              this.spriteIndex = 20;
              this._position.Y = Game1.random.Next(0, this._game.screenHeight / 5);
              this._animationFrames = 4;
              break;
            case 9:
              this.spriteIndex = 28;
              this._animationFrames = 4;
              break;
          }
        }
      }

      public void Draw(SpriteBatch b)
      {
        Vector2 vector2 = new Vector2();
        float num1 = 32f;
        float t = (float) (this._position.Y - 160) / (float) (this._game.screenHeight - 160);
        float num2 = Utility.Lerp(-0.4f, -0.75f, t);
        int num3 = (int) Math.Floor(((double) this._position.X + (double) this._game.screenLeftBound * (double) num2) / ((double) this._game.screenWidth + (double) num1 * 2.0));
        if (num3 != this._lastCycle)
        {
          this._lastCycle = num3;
          if (this.spriteIndex < 2)
          {
            this.spriteIndex = Game1.random.Next(2);
            if (this._game.currentTheme == 6)
              this.spriteIndex = 1;
          }
        }
        float y = (float) this._position.Y;
        if (this._bgDecor)
        {
          num2 = Utility.Lerp(-0.15f, -0.25f, (float) this._position.Y / (float) (this._game.screenHeight / 3));
          if (this._game.currentTheme == 3)
            y += (float) (int) (Math.Sin((double) Utility.Lerp(0.0f, 6.283185f, (float) ((this._game.totalTimeMS + (double) (this._position.X * 7) + (double) (this._position.Y * 2)) / 2.0 % 1000.0) / 1000f)) * 3.0);
        }
        vector2.X = (float) MineCart.Mod((int) ((double) this._position.X + (double) this._game.screenLeftBound * (double) num2), (int) ((double) this._game.screenWidth + (double) num1 * 2.0)) - num1;
        b.Draw(this._game.texture, this._game.TransformDraw(new Vector2(vector2.X, y)), new Rectangle?(new Rectangle(96 + this.spriteIndex % 14 * this._game.tileSize + (int) ((this._game.totalTimeMS + (double) (this._position.X * 10)) % 1000.0 / (double) (1000 / this._animationFrames)) % 14 * this._game.tileSize, 848 + this.spriteIndex / 14 * this._game.tileSize, 16, 16)), this.spriteIndex == 0 ? this._game.midBGTint : (this.spriteIndex == 1 ? this._game.lakeTint : Color.White), 0.0f, Vector2.Zero, this._game.GetPixelScale(), SpriteEffects.None, this._bgDecor ? 0.65f : (float) (0.800000011920929 + (double) t * (-1.0 / 1000.0)));
      }
    }

    public class StraightAwayGenerator : MineCart.BaseTrackGenerator
    {
      public int straightAwayLength = 10;
      public List<int> staggerPattern;
      public int minLength = 3;
      public int maxLength = 5;
      public float staggerChance = 0.25f;
      public int minimuimDistanceBetweenStaggers = 1;
      public int currentStaggerDistance;
      public bool generateCheckpoint = true;
      protected bool _generatedCheckpoint = true;

      public MineCart.StraightAwayGenerator SetMinimumDistanceBetweenStaggers(int min)
      {
        this.minimuimDistanceBetweenStaggers = min;
        return this;
      }

      public MineCart.StraightAwayGenerator SetLength(int min, int max)
      {
        this.minLength = min;
        this.maxLength = max;
        return this;
      }

      public MineCart.StraightAwayGenerator SetCheckpoint(bool checkpoint)
      {
        this.generateCheckpoint = checkpoint;
        return this;
      }

      public MineCart.StraightAwayGenerator SetStaggerChance(float chance)
      {
        this.staggerChance = chance;
        return this;
      }

      public MineCart.StraightAwayGenerator SetStaggerValues(params int[] args)
      {
        this.staggerPattern = new List<int>();
        for (int index = 0; index < args.Length; ++index)
          this.staggerPattern.Add(args[index]);
        return this;
      }

      public MineCart.StraightAwayGenerator SetStaggerValueRange(int min, int max)
      {
        this.staggerPattern = new List<int>();
        for (int index = min; index <= max; ++index)
          this.staggerPattern.Add(index);
        return this;
      }

      public StraightAwayGenerator(MineCart game)
        : base(game)
      {
      }

      public override void Initialize()
      {
        this.straightAwayLength = Game1.random.Next(this.minLength, this.maxLength + 1);
        this._generatedCheckpoint = false;
        if (this.straightAwayLength <= 3)
          this._generatedCheckpoint = true;
        base.Initialize();
      }

      protected override void _GenerateTrack()
      {
        if (this._game.generatorPosition.X >= this._game.distanceToTravel)
          return;
        for (int index = 0; index < this.straightAwayLength; ++index)
        {
          if (this._game.generatorPosition.X >= this._game.distanceToTravel)
            return;
          int y = this._game.generatorPosition.Y;
          if (this.currentStaggerDistance <= 0)
          {
            if (Game1.random.NextDouble() < (double) this.staggerChance)
              this._game.generatorPosition.Y += Utility.GetRandom<int>(this.staggerPattern);
            this.currentStaggerDistance = this.minimuimDistanceBetweenStaggers;
          }
          else
            --this.currentStaggerDistance;
          if (!this._game.IsTileInBounds(this._game.generatorPosition.Y))
          {
            this._game.generatorPosition.Y = y;
            this.straightAwayLength = 0;
            break;
          }
          this._game.generatorPosition.Y = this._game.KeepTileInBounds(this._game.generatorPosition.Y);
          MineCart.Track.TrackType track_type = MineCart.Track.TrackType.Straight;
          if (this._game.generatorPosition.Y < y)
            track_type = MineCart.Track.TrackType.UpSlope;
          else if (this._game.generatorPosition.Y > y)
            track_type = MineCart.Track.TrackType.DownSlope;
          if (track_type == MineCart.Track.TrackType.DownSlope && this._game.currentTheme == 1)
            track_type = MineCart.Track.TrackType.IceDownSlope;
          if (track_type == MineCart.Track.TrackType.UpSlope && this._game.currentTheme == 5)
            track_type = MineCart.Track.TrackType.SlimeUpSlope;
          this.AddPickupTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y, track_type);
          ++this._game.generatorPosition.X;
        }
        if (this._generatedTracks == null || this._generatedTracks.Count <= 0 || !this.generateCheckpoint || this._generatedCheckpoint)
          return;
        this._generatedCheckpoint = true;
        this._generatedTracks.OrderBy<MineCart.Track, float>((Func<MineCart.Track, float>) (o => o.position.X));
        this._game.AddCheckpoint((int) ((double) this._generatedTracks[0].position.X / (double) this._game.tileSize));
      }
    }

    public class SmallGapGenerator : MineCart.BaseTrackGenerator
    {
      public int minLength = 3;
      public int maxLength = 5;
      public int minDepth = 5;
      public int maxDepth = 5;

      public MineCart.SmallGapGenerator SetLength(int min, int max)
      {
        this.minLength = min;
        this.maxLength = max;
        return this;
      }

      public MineCart.SmallGapGenerator SetDepth(int min, int max)
      {
        this.minDepth = min;
        this.maxDepth = max;
        return this;
      }

      public SmallGapGenerator(MineCart game)
        : base(game)
      {
      }

      public override void Initialize() => base.Initialize();

      protected override void _GenerateTrack()
      {
        if (this._game.generatorPosition.X >= this._game.distanceToTravel)
          return;
        int num1 = Game1.random.Next(this.minDepth, this.maxDepth + 1);
        int num2 = Game1.random.Next(this.minLength, this.maxLength + 1);
        this.AddTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y);
        ++this._game.generatorPosition.X;
        this._game.generatorPosition.Y += num1;
        for (int index = 0; index < num2; ++index)
        {
          if (this._game.generatorPosition.X >= this._game.distanceToTravel)
          {
            this._game.generatorPosition.Y -= num1;
            return;
          }
          this.AddPickupTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y);
          ++this._game.generatorPosition.X;
        }
        this._game.generatorPosition.Y -= num1;
        if (this._game.generatorPosition.X >= this._game.distanceToTravel)
          return;
        this.AddTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y);
        ++this._game.generatorPosition.X;
      }
    }

    public class RapidHopsGenerator : MineCart.BaseTrackGenerator
    {
      public int minLength = 3;
      public int maxLength = 5;
      private int startY;
      public int yStep;
      public bool chaotic;

      public MineCart.RapidHopsGenerator SetLength(int min, int max)
      {
        this.minLength = min;
        this.maxLength = max;
        return this;
      }

      public MineCart.RapidHopsGenerator SetYStep(int yStep)
      {
        this.yStep = yStep;
        return this;
      }

      public MineCart.RapidHopsGenerator SetChaotic(bool chaotic)
      {
        this.chaotic = chaotic;
        return this;
      }

      public RapidHopsGenerator(MineCart game)
        : base(game)
      {
      }

      public override void Initialize() => base.Initialize();

      protected override void _GenerateTrack()
      {
        if (this._game.generatorPosition.X >= this._game.distanceToTravel)
          return;
        if (this.startY == 0)
          this.startY = this._game.generatorPosition.Y;
        int num = Game1.random.Next(this.minLength, this.maxLength + 1);
        this.AddTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y);
        ++this._game.generatorPosition.X;
        this._game.generatorPosition.Y += this.yStep;
        for (int index = 0; index < num; ++index)
        {
          if (this._game.generatorPosition.Y < 3 || this._game.generatorPosition.Y > this._game.screenHeight / this._game.tileSize - 2)
          {
            this._game.generatorPosition.Y = this._game.screenHeight / this._game.tileSize - 2;
            this.startY = this._game.generatorPosition.Y;
          }
          if (this._game.generatorPosition.X >= this._game.distanceToTravel)
          {
            this._game.generatorPosition.Y -= this.yStep;
            return;
          }
          this.AddPickupTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y);
          this._game.generatorPosition.X += Game1.random.Next(2, 4);
          if (Game1.random.NextDouble() < 0.33)
            this.AddTrack(this._game.generatorPosition.X - 1, Math.Min(this._game.screenHeight / this._game.tileSize - 2, this._game.generatorPosition.Y + Game1.random.Next(5)));
          if (this.chaotic)
            this._game.generatorPosition.Y = this.startY + Game1.random.Next(-Math.Abs(this.yStep), Math.Abs(this.yStep) + 1);
          else
            this._game.generatorPosition.Y += this.yStep;
        }
        if (this._game.generatorPosition.X >= this._game.distanceToTravel)
          return;
        this._game.generatorPosition.Y -= this.yStep;
        this.AddTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y);
        ++this._game.generatorPosition.X;
      }
    }

    public class NoxiousMushroom : MineCart.Obstacle
    {
      public float nextFire;
      public float firePeriod = 1.75f;
      protected MineCart.Track _track;
      public Rectangle[] frames = new Rectangle[3]
      {
        new Rectangle(288, 736, 16, 16),
        new Rectangle(288, 752, 16, 16),
        new Rectangle(288, 768, 16, 16)
      };
      public int currentFrame;
      public float frameDuration = 0.05f;
      public float frameTimer;

      public override Rectangle GetLocalBounds() => new Rectangle(-4, -12, 8, 12);

      public override void InitializeObstacle(MineCart.Track track)
      {
        this.nextFire = Utility.RandomFloat(0.0f, this.firePeriod);
        this._track = track;
        base.InitializeObstacle(track);
      }

      protected override void _Update(float time)
      {
        this.nextFire -= time;
        if ((double) this.nextFire <= 0.0)
        {
          if (this.IsOnScreen() && (double) this._game.deathTimer <= 0.0 && (double) this._game.respawnCounter <= 0.0)
          {
            MineCart.NoxiousGas noxiousGas = this._game.AddEntity<MineCart.NoxiousGas>(new MineCart.NoxiousGas());
            noxiousGas.position = this.position;
            noxiousGas.position.Y = (float) this.GetBounds().Top;
            noxiousGas.InitializeObstacle(this._track);
            Game1.playSound("sandyStep");
            this.currentFrame = 1;
            this.frameTimer = this.frameDuration;
          }
          this.nextFire = 1.5f;
        }
        if (this.currentFrame <= 0)
          return;
        this.frameTimer -= time;
        if ((double) this.frameTimer > 0.0)
          return;
        this.frameTimer = this.frameDuration;
        ++this.currentFrame;
        if (this.currentFrame < this.frames.Length)
          return;
        this.currentFrame = 0;
        this.frameTimer = 0.0f;
      }

      public override void _Draw(SpriteBatch b) => b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(this.frames[this.currentFrame]), Color.White, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale(), SpriteEffects.None, 0.45f);

      public override bool CanSpawnHere(MineCart.Track track) => track != null && track.trackType == MineCart.Track.TrackType.Straight;

      public override void OnPlayerReset() => base.OnPlayerReset();
    }

    public class MushroomSpring : MineCart.Obstacle
    {
      protected HashSet<MineCart.MineCartCharacter> _bouncedPlayers;
      public Rectangle[] frames = new Rectangle[3]
      {
        new Rectangle(400, 736, 16, 16),
        new Rectangle(400, 752, 16, 16),
        new Rectangle(400, 768, 16, 16)
      };
      public int currentFrame;
      public float frameDuration = 0.05f;
      public float frameTimer;

      public override Rectangle GetLocalBounds() => new Rectangle(-4, -12, 8, 12);

      public override void InitializeObstacle(MineCart.Track track)
      {
        base.InitializeObstacle(track);
        this._bouncedPlayers = new HashSet<MineCart.MineCartCharacter>();
      }

      protected override void _Update(float time)
      {
        if (this.currentFrame <= 0)
          return;
        this.frameTimer -= time;
        if ((double) this.frameTimer > 0.0)
          return;
        this.frameTimer = this.frameDuration;
        ++this.currentFrame;
        if (this.currentFrame < this.frames.Length)
          return;
        this.currentFrame = 0;
        this.frameTimer = 0.0f;
      }

      public override void _Draw(SpriteBatch b) => b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(this.frames[this.currentFrame]), Color.White, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale(), SpriteEffects.None, 0.45f);

      public override bool CanSpawnHere(MineCart.Track track) => track != null && track.trackType == MineCart.Track.TrackType.Straight;

      public override bool OnBounce(MineCart.MineCartCharacter player)
      {
        this.BouncePlayer(player);
        return true;
      }

      public override bool OnBump(MineCart.PlayerMineCartCharacter player)
      {
        this.BouncePlayer((MineCart.MineCartCharacter) player);
        return true;
      }

      public void BouncePlayer(MineCart.MineCartCharacter player)
      {
        if (this._bouncedPlayers.Contains(player))
          return;
        this._bouncedPlayers.Add(player);
        if (player is MineCart.PlayerMineCartCharacter)
        {
          this.currentFrame = 1;
          this.frameTimer = this.frameDuration;
          this.ShootDebris(Game1.random.Next(-10, -4), Game1.random.Next(-60, -19));
          this.ShootDebris(Game1.random.Next(5, 11), Game1.random.Next(-60, -19));
          this.ShootDebris(Game1.random.Next(-20, -9), Game1.random.Next(-40, 0));
          this.ShootDebris(Game1.random.Next(10, 21), Game1.random.Next(-40, 0));
          Game1.playSound("hitEnemy");
        }
        player.Bounce(0.15f);
      }

      public void ShootDebris(int x, int y) => this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(368, 784, 16, 16), Utility.PointToVector2(this.GetBounds().Center), (float) x, (float) y, 0.25f, 0.0f, 0.9f, num_animation_frames: 3, animation_interval: 0.3f));

      public override void OnPlayerReset()
      {
        this._bouncedPlayers.Clear();
        base.OnPlayerReset();
      }
    }

    public class MushroomBalanceTrackGenerator : MineCart.BaseTrackGenerator
    {
      protected int minHopSize = 1;
      protected int maxHopSize = 1;
      public int leadupRunway;
      protected float releaseJumpChance;
      protected List<int> staggerPattern;
      protected MineCart.Track.TrackType trackType;

      public MineCart.MushroomBalanceTrackGenerator SetTrackType(
        MineCart.Track.TrackType track_type)
      {
        this.trackType = track_type;
        return this;
      }

      public MineCart.MushroomBalanceTrackGenerator SetLeadupRunway(int leadup_runway)
      {
        this.leadupRunway = leadup_runway;
        return this;
      }

      public MineCart.MushroomBalanceTrackGenerator SetStaggerValues(params int[] args)
      {
        this.staggerPattern = new List<int>();
        for (int index = 0; index < args.Length; ++index)
          this.staggerPattern.Add(args[index]);
        return this;
      }

      public MineCart.MushroomBalanceTrackGenerator SetStaggerValueRange(int min, int max)
      {
        this.staggerPattern = new List<int>();
        for (int index = min; index <= max; ++index)
          this.staggerPattern.Add(index);
        return this;
      }

      public MineCart.MushroomBalanceTrackGenerator SetReleaseJumpChance(float chance)
      {
        this.releaseJumpChance = chance;
        return this;
      }

      public MineCart.MushroomBalanceTrackGenerator SetHopSize(int min, int max)
      {
        this.minHopSize = min;
        this.maxHopSize = max;
        return this;
      }

      public MushroomBalanceTrackGenerator(MineCart game)
        : base(game)
      {
        this.staggerPattern = new List<int>();
      }

      public override void Initialize() => base.Initialize();

      protected override void _GenerateTrack()
      {
        if (this._game.generatorPosition.X >= this._game.distanceToTravel)
          return;
        for (int index = 0; index < this.leadupRunway; ++index)
        {
          this._game.AddTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y);
          ++this._game.generatorPosition.X;
        }
        this._game.trackBuilderCharacter.enabled = true;
        List<MineCart.BalanceTrack> collection1 = new List<MineCart.BalanceTrack>();
        for (int index1 = 0; index1 < 4; ++index1)
        {
          if (index1 != 1 || Game1.random.NextDouble() >= 0.5)
          {
            this._game.trackBuilderCharacter.position.X = (float) ((double) this._game.generatorPosition.X - 1.0 + 0.5) * (float) this._game.tileSize;
            this._game.GetTrackForXPosition(this._game.trackBuilderCharacter.position.X);
            this._game.trackBuilderCharacter.position.Y = (float) (this._game.generatorPosition.Y * this._game.tileSize);
            this._game.trackBuilderCharacter.ForceGrounded();
            this._game.trackBuilderCharacter.Jump();
            this._game.trackBuilderCharacter.Update(0.03f);
            int y1 = this._game.generatorPosition.Y;
            if (index1 != 1)
            {
              if (index1 == 3 && Game1.random.NextDouble() < 0.5)
                y1 -= 4;
              else if (this.staggerPattern != null && this.staggerPattern.Count > 0)
                y1 += Utility.GetRandom<int>(this.staggerPattern);
            }
            int y2 = this._game.KeepTileInBounds(y1);
            bool flag = false;
            while (!flag)
            {
              if ((double) this._game.trackBuilderCharacter.position.Y < (double) (y2 * this._game.tileSize) && Math.Abs(Math.Round((double) this._game.trackBuilderCharacter.position.X / (double) this._game.tileSize) - (double) this._game.generatorPosition.X) > 0.0 && this._game.trackBuilderCharacter.IsJumping() && Game1.random.NextDouble() < (double) this.releaseJumpChance)
                this._game.trackBuilderCharacter.ReleaseJump();
              Vector2 position = this._game.trackBuilderCharacter.position;
              Vector2 velocity = this._game.trackBuilderCharacter.velocity;
              this._game.trackBuilderCharacter.Update(0.03f);
              if ((double) position.Y < (double) (y2 * this._game.tileSize) && (double) this._game.trackBuilderCharacter.position.Y >= (double) (y2 * this._game.tileSize))
                flag = true;
              if (this._game.trackBuilderCharacter.IsGrounded() || (double) this._game.trackBuilderCharacter.position.Y / (double) this._game.tileSize > (double) this._game.bottomTile)
              {
                this._game.trackBuilderCharacter.position = position;
                if (!this._game.IsTileInBounds(y2))
                  return;
                y2 = this._game.KeepTileInBounds((int) ((double) position.Y / (double) this._game.tileSize));
                break;
              }
            }
            this._game.generatorPosition.Y = y2;
            if (index1 == 0 || index1 == 2)
            {
              List<MineCart.BalanceTrack> collection2 = new List<MineCart.BalanceTrack>();
              this._game.generatorPosition.X = (int) ((double) this._game.trackBuilderCharacter.position.X / (double) this._game.tileSize);
              float num = 0.0f;
              if (index1 == 2 && collection1.Count > 0)
                num = collection1[0].position.Y - collection1[0].startY;
              MineCart.Track track1 = (MineCart.Track) new MineCart.BalanceTrack(MineCart.Track.TrackType.MushroomLeft, false);
              track1.position.X = (float) (this._game.generatorPosition.X * this._game.tileSize);
              track1.position.Y = this._game.trackBuilderCharacter.position.Y + num;
              (track1 as MineCart.BalanceTrack).startY = track1.position.Y;
              this.AddTrack(track1);
              collection2.Add(track1 as MineCart.BalanceTrack);
              ++this._game.generatorPosition.X;
              MineCart.Track track2 = (MineCart.Track) new MineCart.BalanceTrack(MineCart.Track.TrackType.MushroomMiddle, false);
              track2.position.X = (float) (this._game.generatorPosition.X * this._game.tileSize);
              track2.position.Y = this._game.trackBuilderCharacter.position.Y + num;
              (track2 as MineCart.BalanceTrack).startY = track2.position.Y;
              this.AddTrack(track2);
              collection2.Add(track2 as MineCart.BalanceTrack);
              ++this._game.generatorPosition.X;
              MineCart.Track track3 = (MineCart.Track) new MineCart.BalanceTrack(MineCart.Track.TrackType.MushroomRight, false);
              track3.position.X = (float) (this._game.generatorPosition.X * this._game.tileSize);
              track3.position.Y = this._game.trackBuilderCharacter.position.Y + num;
              (track3 as MineCart.BalanceTrack).startY = track3.position.Y;
              this.AddTrack(track3);
              collection2.Add(track3 as MineCart.BalanceTrack);
              ++this._game.generatorPosition.X;
              foreach (MineCart.BalanceTrack balanceTrack in collection2)
                balanceTrack.connectedTracks = new List<MineCart.BalanceTrack>((IEnumerable<MineCart.BalanceTrack>) collection2);
              if (index1 == 2)
              {
                foreach (MineCart.BalanceTrack balanceTrack in collection1)
                  balanceTrack.counterBalancedTracks = new List<MineCart.BalanceTrack>((IEnumerable<MineCart.BalanceTrack>) collection2);
                foreach (MineCart.BalanceTrack balanceTrack in collection2)
                  balanceTrack.counterBalancedTracks = new List<MineCart.BalanceTrack>((IEnumerable<MineCart.BalanceTrack>) collection1);
              }
              this._game.trackBuilderCharacter.SnapToFloor();
              while (this._game.trackBuilderCharacter.IsGrounded())
              {
                float x = this._game.trackBuilderCharacter.position.X;
                this._game.trackBuilderCharacter.Update(0.03f);
                if (!this._game.trackBuilderCharacter.IsGrounded())
                  this._game.trackBuilderCharacter.position.X = x;
                if (Game1.random.NextDouble() < 0.330000013113022)
                  break;
              }
              collection1.AddRange((IEnumerable<MineCart.BalanceTrack>) collection2);
            }
            else
            {
              int num = Game1.random.Next(this.minHopSize, this.maxHopSize + 1);
              for (int index2 = 0; index2 < num; ++index2)
              {
                this._game.generatorPosition.X = (int) ((double) this._game.trackBuilderCharacter.position.X / (double) this._game.tileSize) + index2;
                if (this._game.generatorPosition.X >= this._game.distanceToTravel)
                  return;
                this.AddPickupTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y, this.trackType);
              }
            }
          }
        }
        foreach (MineCart.BalanceTrack balanceTrack in collection1)
          balanceTrack.position.Y = balanceTrack.startY;
        ++this._game.generatorPosition.X;
      }
    }

    public class MushroomBunnyHopGenerator : MineCart.BaseTrackGenerator
    {
      protected int numberOfHops;
      protected int minHops = 1;
      protected int maxHops = 5;
      protected int minHopSize = 1;
      protected int maxHopSize = 1;
      public int leadupRunway;
      protected float releaseJumpChance;
      protected List<int> staggerPattern;
      protected MineCart.Track.TrackType trackType;

      public MineCart.MushroomBunnyHopGenerator SetTrackType(
        MineCart.Track.TrackType track_type)
      {
        this.trackType = track_type;
        return this;
      }

      public MineCart.MushroomBunnyHopGenerator SetLeadupRunway(int leadup_runway)
      {
        this.leadupRunway = leadup_runway;
        return this;
      }

      public MineCart.MushroomBunnyHopGenerator SetStaggerValues(params int[] args)
      {
        this.staggerPattern = new List<int>();
        for (int index = 0; index < args.Length; ++index)
          this.staggerPattern.Add(args[index]);
        return this;
      }

      public MineCart.MushroomBunnyHopGenerator SetStaggerValueRange(int min, int max)
      {
        this.staggerPattern = new List<int>();
        for (int index = min; index <= max; ++index)
          this.staggerPattern.Add(index);
        return this;
      }

      public MineCart.MushroomBunnyHopGenerator SetReleaseJumpChance(float chance)
      {
        this.releaseJumpChance = chance;
        return this;
      }

      public MineCart.MushroomBunnyHopGenerator SetHopSize(int min, int max)
      {
        this.minHopSize = min;
        this.maxHopSize = max;
        return this;
      }

      public MineCart.MushroomBunnyHopGenerator SetNumberOfHops(int min, int max)
      {
        this.minHops = min;
        this.maxHops = max;
        return this;
      }

      public MushroomBunnyHopGenerator(MineCart game)
        : base(game)
      {
        this.minHopSize = 1;
        this.maxHopSize = 1;
        this.staggerPattern = new List<int>();
      }

      public override void Initialize()
      {
        this.numberOfHops = Game1.random.Next(this.minHops, this.maxHops + 1);
        base.Initialize();
      }

      protected override void _GenerateTrack()
      {
        if (this._game.generatorPosition.X >= this._game.distanceToTravel)
          return;
        for (int index = 0; index < this.leadupRunway; ++index)
        {
          this._game.AddTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y);
          ++this._game.generatorPosition.X;
        }
        this._game.trackBuilderCharacter.enabled = true;
        MineCart.MushroomSpring mushroomSpring = (MineCart.MushroomSpring) null;
        for (int index1 = 0; index1 < this.numberOfHops; ++index1)
        {
          this._game.trackBuilderCharacter.position.X = (float) ((double) this._game.generatorPosition.X - 1.0 + 0.5) * (float) this._game.tileSize;
          this._game.trackBuilderCharacter.position.Y = (float) (this._game.generatorPosition.Y * this._game.tileSize);
          this._game.trackBuilderCharacter.ForceGrounded();
          this._game.trackBuilderCharacter.Jump();
          mushroomSpring?.BouncePlayer(this._game.trackBuilderCharacter);
          this._game.trackBuilderCharacter.Update(0.03f);
          int y1 = this._game.generatorPosition.Y;
          if (this.staggerPattern != null && this.staggerPattern.Count > 0)
            y1 += Utility.GetRandom<int>(this.staggerPattern);
          int y2 = this._game.KeepTileInBounds(y1);
          bool flag = false;
          while (!flag)
          {
            if ((double) this._game.trackBuilderCharacter.position.Y < (double) (y2 * this._game.tileSize) && Math.Abs(Math.Round((double) this._game.trackBuilderCharacter.position.X / (double) this._game.tileSize) - (double) this._game.generatorPosition.X) > 1.0 && this._game.trackBuilderCharacter.IsJumping() && Game1.random.NextDouble() < (double) this.releaseJumpChance)
              this._game.trackBuilderCharacter.ReleaseJump();
            Vector2 position = this._game.trackBuilderCharacter.position;
            double y3 = (double) this._game.trackBuilderCharacter.velocity.Y;
            this._game.trackBuilderCharacter.Update(0.03f);
            if (y3 < 0.0 && (double) this._game.trackBuilderCharacter.velocity.Y >= 0.0)
              this._game.CreatePickup(this._game.trackBuilderCharacter.position + new Vector2(0.0f, 8f));
            if ((double) position.Y < (double) (y2 * this._game.tileSize) && (double) this._game.trackBuilderCharacter.position.Y >= (double) (y2 * this._game.tileSize))
              flag = true;
            if (this._game.trackBuilderCharacter.IsGrounded() || (double) this._game.trackBuilderCharacter.position.Y / (double) this._game.tileSize > (double) this._game.bottomTile)
            {
              this._game.trackBuilderCharacter.position = position;
              if (!this._game.IsTileInBounds(y2))
                return;
              y2 = this._game.KeepTileInBounds((int) ((double) position.Y / (double) this._game.tileSize));
              break;
            }
          }
          this._game.generatorPosition.Y = y2;
          int num = Game1.random.Next(this.minHopSize, this.maxHopSize + 1);
          MineCart.Track.TrackType track_type = this.trackType;
          if (index1 >= this.numberOfHops - 1)
            track_type = MineCart.Track.TrackType.Straight;
          mushroomSpring = (MineCart.MushroomSpring) null;
          for (int index2 = 0; index2 < num; ++index2)
          {
            this._game.generatorPosition.X = (int) ((double) this._game.trackBuilderCharacter.position.X / (double) this._game.tileSize) + index2;
            if (this._game.generatorPosition.X >= this._game.distanceToTravel)
              return;
            if (track_type == MineCart.Track.TrackType.MushroomMiddle)
            {
              this.AddTrack(this._game.generatorPosition.X - 1, this._game.generatorPosition.Y, MineCart.Track.TrackType.MushroomLeft);
              this.AddTrack(this._game.generatorPosition.X + 1, this._game.generatorPosition.Y, MineCart.Track.TrackType.MushroomRight);
            }
            MineCart.Track track = this.AddTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y, track_type);
            if (index2 == num - 1 && index1 < this.numberOfHops - 1 && this._game.generatorPosition.Y > 4)
            {
              mushroomSpring = this._game.AddEntity<MineCart.MushroomSpring>(new MineCart.MushroomSpring());
              mushroomSpring.InitializeObstacle(track);
              mushroomSpring.position.X = track.position.X + (float) (this._game.tileSize / 2);
              mushroomSpring.position.Y = (float) track.GetYAtPoint(mushroomSpring.position.X);
            }
          }
        }
        ++this._game.generatorPosition.X;
      }
    }

    public class BunnyHopGenerator : MineCart.BaseTrackGenerator
    {
      protected int numberOfHops;
      protected int minHops = 1;
      protected int maxHops = 5;
      protected int minHopSize = 1;
      protected int maxHopSize = 1;
      public int leadupRunway;
      protected float releaseJumpChance;
      protected List<int> staggerPattern;
      protected MineCart.Track.TrackType trackType;

      public MineCart.BunnyHopGenerator SetTrackType(MineCart.Track.TrackType track_type)
      {
        this.trackType = track_type;
        return this;
      }

      public MineCart.BunnyHopGenerator SetLeadupRunway(int leadup_runway)
      {
        this.leadupRunway = leadup_runway;
        return this;
      }

      public MineCart.BunnyHopGenerator SetStaggerValues(params int[] args)
      {
        this.staggerPattern = new List<int>();
        for (int index = 0; index < args.Length; ++index)
          this.staggerPattern.Add(args[index]);
        return this;
      }

      public MineCart.BunnyHopGenerator SetStaggerValueRange(int min, int max)
      {
        this.staggerPattern = new List<int>();
        for (int index = min; index <= max; ++index)
          this.staggerPattern.Add(index);
        return this;
      }

      public MineCart.BunnyHopGenerator SetReleaseJumpChance(float chance)
      {
        this.releaseJumpChance = chance;
        return this;
      }

      public MineCart.BunnyHopGenerator SetHopSize(int min, int max)
      {
        this.minHopSize = min;
        this.maxHopSize = max;
        return this;
      }

      public MineCart.BunnyHopGenerator SetNumberOfHops(int min, int max)
      {
        this.minHops = min;
        this.maxHops = max;
        return this;
      }

      public BunnyHopGenerator(MineCart game)
        : base(game)
      {
        this.minHopSize = 1;
        this.maxHopSize = 1;
        this.staggerPattern = new List<int>();
      }

      public override void Initialize()
      {
        this.numberOfHops = Game1.random.Next(this.minHops, this.maxHops + 1);
        base.Initialize();
      }

      protected override void _GenerateTrack()
      {
        if (this._game.generatorPosition.X >= this._game.distanceToTravel)
          return;
        for (int index = 0; index < this.leadupRunway; ++index)
        {
          this._game.AddTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y);
          ++this._game.generatorPosition.X;
        }
        this._game.trackBuilderCharacter.enabled = true;
        for (int index1 = 0; index1 < this.numberOfHops; ++index1)
        {
          this._game.trackBuilderCharacter.position.X = (float) ((double) this._game.generatorPosition.X - 1.0 + 0.5) * (float) this._game.tileSize;
          this._game.trackBuilderCharacter.position.Y = (float) (this._game.generatorPosition.Y * this._game.tileSize);
          this._game.trackBuilderCharacter.ForceGrounded();
          this._game.trackBuilderCharacter.Jump();
          this._game.trackBuilderCharacter.Update(0.03f);
          int y1 = this._game.generatorPosition.Y;
          if (this.staggerPattern != null && this.staggerPattern.Count > 0)
            y1 += Utility.GetRandom<int>(this.staggerPattern);
          int y2 = this._game.KeepTileInBounds(y1);
          bool flag = false;
          while (!flag)
          {
            if ((double) this._game.trackBuilderCharacter.position.Y < (double) (y2 * this._game.tileSize) && Math.Abs(Math.Round((double) this._game.trackBuilderCharacter.position.X / (double) this._game.tileSize) - (double) this._game.generatorPosition.X) > 1.0 && this._game.trackBuilderCharacter.IsJumping() && Game1.random.NextDouble() < (double) this.releaseJumpChance)
              this._game.trackBuilderCharacter.ReleaseJump();
            Vector2 position = this._game.trackBuilderCharacter.position;
            double y3 = (double) this._game.trackBuilderCharacter.velocity.Y;
            this._game.trackBuilderCharacter.Update(0.03f);
            if (y3 < 0.0 && (double) this._game.trackBuilderCharacter.velocity.Y >= 0.0)
              this._game.CreatePickup(this._game.trackBuilderCharacter.position + new Vector2(0.0f, 8f));
            if ((double) position.Y < (double) (y2 * this._game.tileSize) && (double) this._game.trackBuilderCharacter.position.Y >= (double) (y2 * this._game.tileSize))
              flag = true;
            if (this._game.trackBuilderCharacter.IsGrounded() || (double) this._game.trackBuilderCharacter.position.Y / (double) this._game.tileSize > (double) this._game.bottomTile)
            {
              this._game.trackBuilderCharacter.position = position;
              if (!this._game.IsTileInBounds(y2))
                return;
              y2 = this._game.KeepTileInBounds((int) ((double) position.Y / (double) this._game.tileSize));
              break;
            }
          }
          this._game.generatorPosition.Y = y2;
          int num = Game1.random.Next(this.minHopSize, this.maxHopSize + 1);
          MineCart.Track.TrackType track_type = this.trackType;
          if (index1 >= this.numberOfHops - 1)
            track_type = MineCart.Track.TrackType.Straight;
          for (int index2 = 0; index2 < num; ++index2)
          {
            this._game.generatorPosition.X = (int) ((double) this._game.trackBuilderCharacter.position.X / (double) this._game.tileSize) + index2;
            if (this._game.generatorPosition.X >= this._game.distanceToTravel)
              return;
            if (track_type == MineCart.Track.TrackType.MushroomMiddle)
            {
              this.AddTrack(this._game.generatorPosition.X - 1, this._game.generatorPosition.Y, MineCart.Track.TrackType.MushroomLeft);
              this.AddTrack(this._game.generatorPosition.X + 1, this._game.generatorPosition.Y, MineCart.Track.TrackType.MushroomRight);
            }
            this.AddPickupTrack(this._game.generatorPosition.X, this._game.generatorPosition.Y, track_type);
          }
        }
        ++this._game.generatorPosition.X;
      }
    }

    public class BaseTrackGenerator
    {
      public const int OBSTACLE_NONE = -10;
      public const int OBSTACLE_MIDDLE = -10;
      public const int OBSTACLE_FRONT = -11;
      public const int OBSTACLE_BACK = -12;
      public const int OBSTACLE_RANDOM = -13;
      protected List<MineCart.Track> _generatedTracks;
      protected MineCart _game;
      protected int _obstaclePlacementPosition = -10;
      protected Dictionary<int, KeyValuePair<MineCart.ObstacleTypes, float>> _obstacleIndices = new Dictionary<int, KeyValuePair<MineCart.ObstacleTypes, float>>();
      protected Func<MineCart.Track, MineCart.BaseTrackGenerator, bool> _pickupFunction;

      public static bool FlatsOnly(MineCart.Track track, MineCart.BaseTrackGenerator generator) => track.trackType == MineCart.Track.TrackType.None;

      public static bool UpSlopesOnly(MineCart.Track track, MineCart.BaseTrackGenerator generator) => track.trackType == MineCart.Track.TrackType.UpSlope;

      public static bool DownSlopesOnly(MineCart.Track track, MineCart.BaseTrackGenerator generator) => track.trackType == MineCart.Track.TrackType.DownSlope;

      public static bool IceDownSlopesOnly(
        MineCart.Track track,
        MineCart.BaseTrackGenerator generator)
      {
        return track.trackType == MineCart.Track.TrackType.IceDownSlope;
      }

      public static bool Always(MineCart.Track track, MineCart.BaseTrackGenerator generator) => true;

      public static bool EveryOtherTile(MineCart.Track track, MineCart.BaseTrackGenerator generator) => (int) ((double) track.position.X / 16.0) % 2 == 0;

      public T AddObstacle<T>(
        MineCart.ObstacleTypes obstacle_type,
        int position,
        float obstacle_chance = 1f)
        where T : MineCart.BaseTrackGenerator
      {
        this._obstacleIndices.Add(position, new KeyValuePair<MineCart.ObstacleTypes, float>(obstacle_type, obstacle_chance));
        return this as T;
      }

      public T AddPickupFunction<T>(
        Func<MineCart.Track, MineCart.BaseTrackGenerator, bool> pickup_spawn_function)
        where T : MineCart.BaseTrackGenerator
      {
        this._pickupFunction += pickup_spawn_function;
        return this as T;
      }

      public BaseTrackGenerator(MineCart game) => this._game = game;

      public MineCart.Track AddTrack(int x, int y, MineCart.Track.TrackType track_type = MineCart.Track.TrackType.Straight)
      {
        MineCart.Track track = this._game.AddTrack(x, y, track_type);
        this._generatedTracks.Add(track);
        return track;
      }

      public MineCart.Track AddTrack(MineCart.Track track)
      {
        this._game.AddTrack(track);
        this._generatedTracks.Add(track);
        return track;
      }

      public MineCart.Track AddPickupTrack(
        int x,
        int y,
        MineCart.Track.TrackType track_type = MineCart.Track.TrackType.Straight)
      {
        MineCart.Track track = this.AddTrack(x, y, track_type);
        if (this._pickupFunction == null)
          return track;
        foreach (Func<MineCart.Track, MineCart.BaseTrackGenerator, bool> invocation in this._pickupFunction.GetInvocationList())
        {
          if (!invocation(track, this))
            return track;
        }
        MineCart.Pickup pickup1 = this._game.CreatePickup(track.position + new Vector2(8f, (float) -this._game.tileSize));
        if (pickup1 != null && (track.trackType == MineCart.Track.TrackType.DownSlope || track.trackType == MineCart.Track.TrackType.UpSlope || track.trackType == MineCart.Track.TrackType.IceDownSlope || track.trackType == MineCart.Track.TrackType.SlimeUpSlope))
        {
          MineCart.Pickup pickup2 = pickup1;
          pickup2.position = pickup2.position + new Vector2(0.0f, (float) -this._game.tileSize * 0.75f);
        }
        return track;
      }

      public virtual void Initialize() => this._generatedTracks = new List<MineCart.Track>();

      public void GenerateTrack()
      {
        this._GenerateTrack();
        this.PopulateObstacles();
      }

      public void PopulateObstacles()
      {
        if (this._game.generatorPosition.X >= this._game.distanceToTravel || this._generatedTracks.Count == 0)
          return;
        this._generatedTracks.OrderBy<MineCart.Track, float>((Func<MineCart.Track, float>) (o => o.position.X));
        if (this._obstacleIndices == null || this._obstacleIndices.Count == 0)
          return;
        foreach (int key1 in this._obstacleIndices.Keys)
        {
          double num1 = Game1.random.NextDouble();
          KeyValuePair<MineCart.ObstacleTypes, float> obstacleIndex = this._obstacleIndices[key1];
          double num2 = (double) obstacleIndex.Value;
          if (num1 <= num2)
          {
            int index;
            switch (key1)
            {
              case -13:
                index = Game1.random.Next(this._generatedTracks.Count);
                break;
              case -12:
                index = this._generatedTracks.Count - 1;
                break;
              case -11:
                index = 0;
                break;
              case -10:
                index = (this._generatedTracks.Count - 1) / 2;
                break;
              default:
                index = key1;
                break;
            }
            MineCart.Track generatedTrack = this._generatedTracks[index];
            if (generatedTrack != null && (int) ((double) generatedTrack.position.X / (double) this._game.tileSize) < this._game.distanceToTravel)
            {
              MineCart game = this._game;
              MineCart.Track track = generatedTrack;
              obstacleIndex = this._obstacleIndices[key1];
              int key2 = (int) obstacleIndex.Key;
              game.AddObstacle(track, (MineCart.ObstacleTypes) key2);
            }
          }
        }
      }

      protected virtual void _GenerateTrack() => ++this._game.generatorPosition.X;
    }

    public class Spark
    {
      public float x;
      public float y;
      public Color c;
      public float dx;
      public float dy;

      public Spark(float x, float y, float dx, float dy)
      {
        this.x = x;
        this.y = y;
        this.dx = dx;
        this.dy = dy;
        this.c = Color.Yellow;
      }
    }

    public class Entity
    {
      public Vector2 position;
      protected MineCart _game;
      public bool visible = true;
      public bool enabled = true;
      protected bool _destroyed;

      public Vector2 drawnPosition => this.position - new Vector2(this._game.screenLeftBound, 0.0f);

      public virtual void OnPlayerReset()
      {
      }

      public bool IsOnScreen() => (double) this.position.X >= (double) this._game.screenLeftBound - (double) (this._game.tileSize * 4) && (double) this.position.X <= (double) this._game.screenLeftBound + (double) this._game.screenWidth + (double) (this._game.tileSize * 4);

      public bool IsActive() => !this._destroyed && this.enabled;

      public void Initialize(MineCart game)
      {
        this._game = game;
        this._Initialize();
      }

      public void Destroy() => this._destroyed = true;

      protected virtual void _Initialize()
      {
      }

      public virtual bool ShouldReap() => this._destroyed;

      public void Draw(SpriteBatch b)
      {
        if (this._destroyed || !this.visible || !this.enabled)
          return;
        this._Draw(b);
      }

      public virtual void _Draw(SpriteBatch b)
      {
      }

      public void Update(float time)
      {
        if (this._destroyed || !this.enabled)
          return;
        this._Update(time);
      }

      protected virtual void _Update(float time)
      {
      }
    }

    public class BaseCharacter : MineCart.Entity
    {
      public Vector2 velocity;
    }

    public interface ICollideable
    {
      Rectangle GetLocalBounds();

      Rectangle GetBounds();
    }

    public class ObstacleSpawner : MineCart.Obstacle
    {
    }

    public class Bubble : MineCart.Obstacle
    {
      public Vector2 _normalizedVelocity;
      public float moveSpeed = 8f;
      protected float _age;
      protected int _currentFrame;
      protected float _timePerFrame = 0.5f;
      protected int[] _frames = new int[6]
      {
        0,
        1,
        2,
        3,
        3,
        2
      };
      protected int _repeatedFrameCount = 4;
      protected float _lifeTime = 3f;
      public Vector2 bubbleOffset = Vector2.Zero;

      public override void OnPlayerReset() => this.Destroy();

      public override Rectangle GetBounds()
      {
        Rectangle bounds = base.GetBounds();
        bounds.X += (int) this.bubbleOffset.X;
        bounds.Y += (int) this.bubbleOffset.Y;
        return base.GetBounds();
      }

      public Bubble(float angle, float speed)
      {
        this._normalizedVelocity.X = (float) Math.Cos((double) angle * 3.14159274101257 / 180.0);
        this._normalizedVelocity.Y = -(float) Math.Sin((double) angle * 3.14159274101257 / 180.0);
        this.moveSpeed = speed;
        this._age = 0.0f;
      }

      public override bool OnBump(MineCart.PlayerMineCartCharacter player)
      {
        this.Pop();
        return base.OnBump(player);
      }

      public override bool OnBounce(MineCart.MineCartCharacter player)
      {
        if (!(player is MineCart.PlayerMineCartCharacter))
          return false;
        player.Bounce();
        this.Pop();
        return true;
      }

      public void Pop(bool play_sound = true)
      {
        if (play_sound)
          Game1.playSound("dropItemInWater");
        this.Destroy();
        MineCart game = this._game;
        Rectangle source_rect = new Rectangle(32, 240, 16, 16);
        Rectangle bounds = this.GetBounds();
        double x = (double) bounds.Center.X;
        bounds = this.GetBounds();
        double y = (double) bounds.Center.Y;
        Vector2 spawn_position = new Vector2((float) x, (float) y);
        MineCart.MineDebris new_entity = new MineCart.MineDebris(source_rect, spawn_position, 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 0.4f, num_animation_frames: 2, animation_interval: 0.2f);
        game.AddEntity<MineCart.MineDebris>(new_entity);
      }

      protected override void _Update(float time)
      {
        this.position = this.position + this.moveSpeed * this._normalizedVelocity * time;
        this._age += time;
        this._currentFrame = (int) ((double) this._age / (double) this._timePerFrame);
        if (this._currentFrame >= this._frames.Length)
        {
          this._currentFrame -= this._frames.Length;
          this._currentFrame %= this._repeatedFrameCount;
          this._currentFrame += this._frames.Length - this._repeatedFrameCount;
        }
        this.bubbleOffset.X = (float) Math.Cos((double) this._age * 10.0) * 4f;
        this.bubbleOffset.Y = (float) Math.Sin((double) this._age * 10.0) * 4f;
        if ((double) this._age >= (double) this._lifeTime)
          this.Pop(false);
        base._Update(time);
      }

      public override void _Draw(SpriteBatch b) => b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + this.bubbleOffset), new Rectangle?(new Rectangle(this._frames[this._currentFrame] * 16, 256, 16, 16)), Color.White, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale(), SpriteEffects.None, 0.27f);
    }

    public class PlayerBubbleSpawner : MineCart.Entity
    {
      public int bubbleCount;
      public float timer;

      protected override void _Update(float time)
      {
        this.position = this._game.player.position;
        this.timer -= time;
        if ((double) this._game.player.velocity.Y > 0.0 && this.bubbleCount == 0)
        {
          this.bubbleCount = 1;
          this.timer = Utility.Lerp(0.05f, 0.25f, (float) Game1.random.NextDouble());
        }
        if ((double) this.timer <= 0.0 && this.bubbleCount <= 0)
        {
          this.bubbleCount = Game1.random.Next(1, 4);
          this.timer = Utility.Lerp(0.15f, 0.25f, (float) Game1.random.NextDouble());
        }
        else
        {
          if ((double) this.timer > 0.0)
            return;
          --this.bubbleCount;
          this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(0, 256, 16, 16), this.position + new Vector2((float) (-(double) this._game.player.characterExtraHeight - 16.0)) / 2f, -10f, 10f, gravity_multiplier: -1f, life_time: 1.5f, scale: 0.5f, num_animation_frames: 4, holdLastFrame: true));
          if (this.bubbleCount == 0)
            this.timer = Utility.Lerp(1f, 1.5f, (float) Game1.random.NextDouble());
          else
            this.timer = Utility.Lerp(0.15f, 0.25f, (float) Game1.random.NextDouble());
        }
      }
    }

    public class Whale : MineCart.Entity
    {
      protected MineCart.Whale.CurrentState _currentState;
      protected float _stateTimer;
      public float mouthCloseTime = 1f;
      protected float _nextFire;
      protected int _currentFrame;
      protected Vector2 _basePosition;

      public void SetState(MineCart.Whale.CurrentState new_state, float state_timer = 1f)
      {
        this._currentState = new_state;
        this._stateTimer = state_timer;
      }

      public override void OnPlayerReset()
      {
        this._currentState = MineCart.Whale.CurrentState.Idle;
        this._stateTimer = 2f;
      }

      protected override void _Update(float time)
      {
        base._Update(time);
        this._basePosition.Y = Utility.MoveTowards(this._basePosition.Y, this._game.player.position.Y + 32f, 48f * time);
        this.position.X = (float) ((double) this._game.screenLeftBound - 128.0 + (double) this._game.screenWidth + Math.Cos(this._game.totalTime * Math.PI / 2.29999995231628) * 24.0);
        this.position.Y = this._basePosition.Y + (float) Math.Sin(this._game.totalTime * Math.PI / 3.0) * 32f;
        if ((double) this.position.Y > (double) this._game.screenHeight)
          this.position.Y = (float) this._game.screenHeight;
        if ((double) this.position.Y < 120.0)
          this.position.Y = 120f;
        this._stateTimer -= time;
        if (this._currentState == MineCart.Whale.CurrentState.Idle)
        {
          this._currentFrame = 0;
          if ((double) this._stateTimer >= 0.0 || this._game.gameState == MineCart.GameStates.Cutscene)
            return;
          this._currentState = MineCart.Whale.CurrentState.OpenMouth;
          this._stateTimer = this.mouthCloseTime;
          Game1.playSound("croak");
        }
        else if (this._currentState == MineCart.Whale.CurrentState.OpenMouth)
        {
          this._currentFrame = (int) Utility.Lerp(3f, 0.0f, this._stateTimer / this.mouthCloseTime);
          if ((double) this._stateTimer < 0.0)
          {
            this._currentState = MineCart.Whale.CurrentState.FireBubbles;
            this._stateTimer = 4f;
          }
          this._nextFire = 0.0f;
        }
        else if (this._currentState == MineCart.Whale.CurrentState.FireBubbles)
        {
          this._currentFrame = 3;
          this._nextFire -= time;
          if ((double) this._nextFire <= 0.0)
          {
            Game1.playSound("dwop");
            this._nextFire = 0.3f;
            float speed = 32f;
            float b = 45f;
            if ((double) this._game.generatorPosition.X >= (double) this._game.distanceToTravel / 2.0)
            {
              speed = Utility.Lerp(32f, 64f, (float) Game1.random.NextDouble());
              b = 60f;
            }
            this._game.AddEntity<MineCart.Bubble>(new MineCart.Bubble(180f + Utility.Lerp(-b, b, (float) Game1.random.NextDouble()), speed)).position = this.position + new Vector2(48f, -40f);
            this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(0, 256, 16, 16), this.position + new Vector2(96f, -100f), -10f, 10f, gravity_multiplier: -1f, life_time: 1f, scale: 0.5f, num_animation_frames: 4, animation_interval: 0.25f));
          }
          if ((double) this._stateTimer >= 0.0)
            return;
          this._currentState = MineCart.Whale.CurrentState.CloseMouth;
          this._stateTimer = this.mouthCloseTime;
        }
        else
        {
          if (this._currentState != MineCart.Whale.CurrentState.CloseMouth)
            return;
          this._currentFrame = (int) Utility.Lerp(0.0f, 3f, this._stateTimer / this.mouthCloseTime);
          if ((double) this._stateTimer >= 0.0)
            return;
          this._currentState = MineCart.Whale.CurrentState.Idle;
          this._stateTimer = 2f;
        }
      }

      protected override void _Initialize()
      {
        this._currentState = MineCart.Whale.CurrentState.Idle;
        this._stateTimer = Utility.Lerp(1f, 2f, (float) Game1.random.NextDouble());
        this._basePosition.Y = (float) (this._game.screenHeight / 2 + 56);
        base._Initialize();
      }

      public override void _Draw(SpriteBatch b)
      {
        Point point = new Point();
        Point p = new Point();
        if (this._currentFrame > 0)
        {
          point.X = 85 * (this._currentFrame - 1) + 1;
          point.Y = 112;
          p.X = 3;
          p.Y = -3;
        }
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + new Vector2(85f, 0.0f)), new Rectangle?(new Rectangle(86, 288, 75, 112)), Color.White, 0.0f, new Vector2(0.0f, 112f), this._game.GetPixelScale(), SpriteEffects.None, 0.29f);
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + Utility.PointToVector2(p)), new Rectangle?(new Rectangle(point.X, 288 + point.Y, 85, 112)), Color.White, 0.0f, new Vector2(0.0f, 112f), this._game.GetPixelScale(), SpriteEffects.None, 0.28f);
      }

      public enum CurrentState
      {
        Idle,
        OpenMouth,
        FireBubbles,
        CloseMouth,
      }
    }

    public class EndingJunimo : MineCart.Entity
    {
      protected Color _color;
      protected Vector2 _velocity;
      private bool _special;

      public EndingJunimo(bool special = false) => this._special = special;

      protected override void _Initialize()
      {
        if (this._special || Game1.random.NextDouble() < 0.01)
        {
          switch (Game1.random.Next(8))
          {
            case 0:
              this._color = Color.Red;
              break;
            case 1:
              this._color = Color.Goldenrod;
              break;
            case 2:
              this._color = Color.Yellow;
              break;
            case 3:
              this._color = Color.Lime;
              break;
            case 4:
              this._color = new Color(0, (int) byte.MaxValue, 180);
              break;
            case 5:
              this._color = new Color(0, 100, (int) byte.MaxValue);
              break;
            case 6:
              this._color = Color.MediumPurple;
              break;
            case 7:
              this._color = Color.Salmon;
              break;
          }
          if (Game1.random.NextDouble() < 0.01)
            this._color = Color.White;
        }
        else
        {
          switch (Game1.random.Next(8))
          {
            case 0:
              this._color = Color.LimeGreen;
              break;
            case 1:
              this._color = Color.Orange;
              break;
            case 2:
              this._color = Color.LightGreen;
              break;
            case 3:
              this._color = Color.Tan;
              break;
            case 4:
              this._color = Color.GreenYellow;
              break;
            case 5:
              this._color = Color.LawnGreen;
              break;
            case 6:
              this._color = Color.PaleGreen;
              break;
            case 7:
              this._color = Color.Turquoise;
              break;
          }
        }
        this._velocity.X = Utility.RandomFloat(-10f, -40f);
        this._velocity.Y = Utility.RandomFloat(-20f, -60f);
      }

      protected override void _Update(float time)
      {
        this.position = this.position + time * this._velocity;
        this._velocity.Y += 210f * time;
        float y = this._game.GetTrackForXPosition(this.position.X).position.Y;
        if ((double) this.position.Y < (double) y)
          return;
        if (Game1.random.NextDouble() < 0.100000001490116)
          Game1.playSound("junimoMeep1");
        this.position.Y = y;
        this._velocity.Y = Utility.RandomFloat(-50f, -90f);
        if ((double) this.position.X < (double) this._game.player.position.X)
          this._velocity.X = Utility.RandomFloat(10f, 40f);
        if ((double) this.position.X <= (double) this._game.player.position.X)
          return;
        this._velocity.X = Utility.RandomFloat(10f, 40f) * -1f;
      }

      public override void _Draw(SpriteBatch b) => b.Draw(Game1.mouseCursors, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(294 + (int) (this._game.totalTimeMS % 400.0) / 100 * 16, 1432, 16, 16)), this._color, 0.0f, new Vector2(8f, 16f), (float) ((double) this._game.GetPixelScale() * 2.0 / 3.0), SpriteEffects.None, 0.25f);
    }

    public class FallingBoulderSpawner : MineCart.Obstacle
    {
      public float period = 2.33f;
      public float currentTime;
      protected MineCart.Track _track;

      public override Rectangle GetLocalBounds() => new Rectangle(0, 0, 0, 0);

      public override Rectangle GetBounds() => new Rectangle(0, 0, 0, 0);

      public override void InitializeObstacle(MineCart.Track track)
      {
        this._track = track;
        this.currentTime = (float) Game1.random.NextDouble() * this.period;
        this.position.Y = -32f;
      }

      protected override void _Update(float time)
      {
        base._Update(time);
        this.currentTime += time;
        if ((double) this.currentTime < (double) this.period)
          return;
        this.currentTime = 0.0f;
        MineCart.FallingBoulder fallingBoulder = this._game.AddEntity<MineCart.FallingBoulder>(new MineCart.FallingBoulder());
        fallingBoulder.position = this.position;
        fallingBoulder.InitializeObstacle(this._track);
      }
    }

    public class WillOWisp : MineCart.Obstacle
    {
      protected float _age;
      protected Vector2 offset;
      protected float tailUpdateTime;
      public float tailRotation;
      public float tailLength;
      public float scale = 1f;
      public float nextDebris = 0.1f;

      public override Rectangle GetBounds()
      {
        Rectangle bounds = base.GetBounds();
        bounds.X += (int) this.offset.X;
        bounds.Y += (int) this.offset.Y;
        return bounds;
      }

      public override Rectangle GetLocalBounds() => new Rectangle(-5, -5, 10, 10);

      protected override void _Update(float time)
      {
        this._age += time;
        Vector2 offset = this.offset;
        float num = 15f;
        this.offset.Y = (float) (Math.Sin((double) this._age * (double) num * 3.14159274101257 / 180.0) - 1.0) * 32f;
        this.offset.X = (float) Math.Cos((double) this._age * (double) num * 3.0 * 3.14159274101257 / 180.0) * 64f;
        this.offset.Y += (float) Math.Sin((double) this._age * (double) num * 6.0 * 3.14159274101257 / 180.0) * 16f;
        Vector2 vector2 = this.offset - offset;
        this.tailRotation = (float) Math.Atan2((double) vector2.Y, (double) vector2.X);
        this.tailLength = vector2.Length();
        this.scale = Utility.Lerp(0.5f, 0.6f, (float) Math.Sin((double) this._age * 200.0 * 3.14159274101257 / 180.0) + 0.5f);
        this.nextDebris -= time;
        if ((double) this.nextDebris > 0.0)
          return;
        this.nextDebris = 0.1f;
        MineCart game = this._game;
        Rectangle source_rect = new Rectangle(192, 96, 16, 16);
        Rectangle bounds = this.GetBounds();
        double x = (double) bounds.Center.X;
        bounds = this.GetBounds();
        double bottom = (double) bounds.Bottom;
        Vector2 spawn_position = new Vector2((float) x, (float) bottom) + new Vector2((float) Game1.random.Next(-4, 5), (float) Game1.random.Next(-4, 5));
        double dx = (double) Game1.random.Next(-30, 31);
        double dy = (double) Game1.random.Next(-30, -19);
        MineCart.MineDebris new_entity = new MineCart.MineDebris(source_rect, spawn_position, (float) dx, (float) dy, 0.25f, -0.15f, 1f, num_animation_frames: 4, animation_interval: 0.25f, draw_depth: 0.46f);
        game.AddEntity<MineCart.MineDebris>(new_entity).visible = this.visible;
      }

      public override bool OnBump(MineCart.PlayerMineCartCharacter player)
      {
        this.Destroy();
        Game1.playSound("ghost");
        for (int index = 0; index < 8; ++index)
        {
          MineCart game = this._game;
          Rectangle source_rect = new Rectangle(192, 96, 16, 16);
          Rectangle bounds = this.GetBounds();
          double x = (double) bounds.Center.X;
          bounds = this.GetBounds();
          double bottom = (double) bounds.Bottom;
          Vector2 spawn_position = new Vector2((float) x, (float) bottom) + new Vector2((float) Game1.random.Next(-4, 5), (float) Game1.random.Next(-4, 5));
          double dx = (double) Game1.random.Next(-50, 51);
          double dy = (double) Game1.random.Next(-50, 51);
          MineCart.MineDebris new_entity = new MineCart.MineDebris(source_rect, spawn_position, (float) dx, (float) dy, 0.25f, -0.15f, 1f, num_animation_frames: 4, animation_interval: 0.25f, draw_depth: 0.28f);
          game.AddEntity<MineCart.MineDebris>(new_entity);
        }
        return base.OnBump(player);
      }

      public override bool ShouldReap() => base.ShouldReap();

      public override void _Draw(SpriteBatch b)
      {
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + this.offset), new Rectangle?(new Rectangle(192, 80, 16, 16)), Color.White, (float) ((double) this._age * 200.0 * (Math.PI / 180.0)), new Vector2(8f, 8f), this._game.GetPixelScale() * this.scale, SpriteEffects.None, 0.27f);
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + this.offset), new Rectangle?(new Rectangle(160, 112, 32, 32)), Color.White, (float) ((double) this._age * 60.0 * (Math.PI / 180.0)), new Vector2(16f, 16f), this._game.GetPixelScale(), SpriteEffects.None, 0.29f);
        if ((double) this._age <= 0.25)
          return;
        Vector2 vector2 = new Vector2(this.tailLength, this.scale);
        if ((double) this.tailLength <= 0.5)
          return;
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + this.offset), new Rectangle?(new Rectangle(208 + (int) ((double) this._age / 0.100000001490116) % 3 * 16, 80, 16, 16)), Color.White, this.tailRotation, new Vector2(16f, 8f), vector2 * this._game.GetPixelScale(), SpriteEffects.None, 0.44f);
      }
    }

    public class CosmeticFallingBoulder : MineCart.FallingBoulder
    {
      private float yBreakPosition;
      private float delayBeforeAppear;
      private Color color;

      public CosmeticFallingBoulder(
        float yBreakPosition,
        Color color,
        float fallSpeed = 96f,
        float delayBeforeAppear = 0.0f)
      {
        this.yBreakPosition = yBreakPosition;
        this.color = color;
        this._fallSpeed = fallSpeed;
        this.delayBeforeAppear = delayBeforeAppear;
        if ((double) delayBeforeAppear <= 0.0)
          return;
        this.visible = false;
      }

      protected override void _Update(float time)
      {
        if ((double) this.delayBeforeAppear > 0.0)
        {
          this.delayBeforeAppear -= time;
          if ((double) this.delayBeforeAppear > 0.0)
            return;
          this.visible = true;
        }
        this._age += time;
        if ((double) this.position.Y >= (double) this.yBreakPosition)
        {
          this._currentFallSpeed = -30f;
          if (this.IsOnScreen())
            Game1.playSound("hammer");
          for (int index = 0; index < 3; ++index)
          {
            MineCart game = this._game;
            Rectangle source_rect = new Rectangle(16, 80, 16, 16);
            Rectangle bounds = this.GetBounds();
            double x = (double) bounds.Center.X;
            bounds = this.GetBounds();
            double bottom = (double) bounds.Bottom;
            Vector2 spawn_position = new Vector2((float) x, (float) bottom);
            double dx = (double) Game1.random.Next(-30, 31);
            double dy = (double) Game1.random.Next(-30, -19);
            MineCart.MineDebris new_entity = new MineCart.MineDebris(source_rect, spawn_position, (float) dx, (float) dy, 0.25f);
            game.AddEntity<MineCart.MineDebris>(new_entity).SetColor(this._game.caveTint);
          }
          this._destroyed = true;
        }
        if ((double) this._currentFallSpeed < (double) this._fallSpeed)
        {
          this._currentFallSpeed += 210f * time;
          if ((double) this._currentFallSpeed > (double) this._fallSpeed)
            this._currentFallSpeed = this._fallSpeed;
        }
        this.position.Y += time * this._currentFallSpeed;
      }

      public override void _Draw(SpriteBatch b)
      {
        SpriteEffects effects = SpriteEffects.None;
        if (Math.Floor((double) this._age / 0.5) % 2.0 == 0.0)
          effects = SpriteEffects.FlipHorizontally;
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(0, 32, 16, 16)), this.color, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale(), effects, 0.15f);
      }
    }

    public class NoxiousGas : MineCart.Obstacle
    {
      protected float _age;
      protected float _currentRiseSpeed;
      protected float _riseSpeed = -90f;

      public override void OnPlayerReset() => this.Destroy();

      public override void InitializeObstacle(MineCart.Track track) => base.InitializeObstacle(track);

      public override void _Draw(SpriteBatch b)
      {
        SpriteEffects effects = SpriteEffects.None;
        if (Math.Floor((double) this._age / 0.5) % 2.0 == 0.0)
          effects = SpriteEffects.FlipHorizontally;
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(368, 784, 16, 16)), Color.White, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale() * Utility.Clamp(this._age / 0.5f, 0.0f, 1f), effects, 0.44f);
      }

      protected override void _Update(float time)
      {
        this._age += time;
        if ((double) this._currentRiseSpeed > (double) this._riseSpeed)
        {
          this._currentRiseSpeed -= 40f * time;
          if ((double) this._currentRiseSpeed < (double) this._riseSpeed)
            this._currentRiseSpeed = this._riseSpeed;
        }
        this.position.Y += time * this._currentRiseSpeed;
      }

      public override bool OnBounce(MineCart.MineCartCharacter player) => false;

      public override bool OnBump(MineCart.PlayerMineCartCharacter player) => base.OnBump(player);

      public override bool ShouldReap() => (double) this.position.Y < -32.0 || base.ShouldReap();
    }

    public class FallingBoulder : MineCart.Obstacle
    {
      protected float _age;
      protected List<MineCart.Track> _tracks;
      protected float _currentFallSpeed;
      protected float _fallSpeed = 96f;
      protected bool _wasBouncedOn;

      public override void OnPlayerReset() => this.Destroy();

      public override void InitializeObstacle(MineCart.Track track)
      {
        base.InitializeObstacle(track);
        List<MineCart.Track> tracksForXposition = this._game.GetTracksForXPosition(this.position.X);
        if (tracksForXposition == null)
          return;
        this._tracks = new List<MineCart.Track>((IEnumerable<MineCart.Track>) tracksForXposition);
      }

      public override void _Draw(SpriteBatch b)
      {
        SpriteEffects effects = SpriteEffects.None;
        if (Math.Floor((double) this._age / 0.5) % 2.0 == 0.0)
          effects = SpriteEffects.FlipHorizontally;
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(0, 32, 16, 16)), this._game.caveTint, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale(), effects, 0.45f);
      }

      protected override void _Update(float time)
      {
        this._age += time;
        if (this._tracks != null && this._tracks.Count > 0)
        {
          if (this._tracks[0] == null)
            this._tracks.RemoveAt(0);
          else if ((double) this.position.Y >= (double) this._tracks[0].GetYAtPoint(this.position.X))
          {
            this._currentFallSpeed = -30f;
            this._tracks.RemoveAt(0);
            if (this.IsOnScreen())
              Game1.playSound("hammer");
            for (int index = 0; index < 3; ++index)
              this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(16, 80, 16, 16), new Vector2((float) this.GetBounds().Center.X, (float) this.GetBounds().Bottom), (float) Game1.random.Next(-30, 31), (float) Game1.random.Next(-30, -19), 0.25f)).SetColor(this._game.caveTint);
          }
        }
        if ((double) this._currentFallSpeed < (double) this._fallSpeed)
        {
          this._currentFallSpeed += 210f * time;
          if ((double) this._currentFallSpeed > (double) this._fallSpeed)
            this._currentFallSpeed = this._fallSpeed;
        }
        this.position.Y += time * this._currentFallSpeed;
      }

      public override bool OnBounce(MineCart.MineCartCharacter player)
      {
        if (!(player is MineCart.PlayerMineCartCharacter))
          return false;
        this._wasBouncedOn = true;
        player.Bounce();
        Game1.playSound("hammer");
        for (int index = 0; index < 3; ++index)
        {
          MineCart game = this._game;
          Rectangle source_rect = new Rectangle(16, 80, 16, 16);
          Rectangle bounds = this.GetBounds();
          double x = (double) bounds.Center.X;
          bounds = this.GetBounds();
          double top = (double) bounds.Top;
          Vector2 spawn_position = new Vector2((float) x, (float) top);
          double dx = (double) Game1.random.Next(-30, 31);
          double dy = (double) Game1.random.Next(-30, -19);
          MineCart.MineDebris new_entity = new MineCart.MineDebris(source_rect, spawn_position, (float) dx, (float) dy, 0.25f);
          game.AddEntity<MineCart.MineDebris>(new_entity).SetColor(this._game.caveTint);
        }
        return true;
      }

      public override bool OnBump(MineCart.PlayerMineCartCharacter player) => this._wasBouncedOn || base.OnBump(player);

      public override bool ShouldReap() => (double) this.position.Y > (double) (this._game.screenHeight + 32) || base.ShouldReap();
    }

    public class MineCartSlime : MineCart.Obstacle
    {
      public override Rectangle GetLocalBounds() => base.GetLocalBounds();

      public override void OnPlayerReset()
      {
      }

      public override void InitializeObstacle(MineCart.Track track) => base.InitializeObstacle(track);

      public override void _Draw(SpriteBatch b)
      {
        SpriteEffects effects = SpriteEffects.None;
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(0, 32, 16, 16)), this._game.caveTint, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale(), effects, 0.45f);
      }

      protected override void _Update(float time)
      {
      }

      public override bool ShouldReap() => false;
    }

    public class SlimeTrack : MineCart.Obstacle
    {
      public override Rectangle GetLocalBounds() => base.GetLocalBounds();

      public override void OnPlayerReset()
      {
      }

      public override void InitializeObstacle(MineCart.Track track) => base.InitializeObstacle(track);

      public override void _Draw(SpriteBatch b)
      {
        SpriteEffects effects = SpriteEffects.None;
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(0, 192, 32, 16)), Color.White, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale(), effects, 0.45f);
      }

      protected override void _Update(float time)
      {
      }

      public override bool ShouldReap() => false;
    }

    public class HugeSlime : MineCart.Obstacle
    {
      protected float _timeUntilHop = 30f;
      protected float _yVelocity;
      protected bool _grounded;
      protected float _maxFallSpeed = 300f;
      protected float _lastTrackY = 300f;
      public Vector2 spriteScale = new Vector2(1f, 1f);
      protected int _currentFrame;
      protected Vector2 _desiredScale = new Vector2(1f, 1f);
      protected float _scaleSpeed = 4f;
      protected float _jumpStrength = -200f;
      private bool _hasPeparedToJump;

      public override Rectangle GetLocalBounds() => new Rectangle(-40, -60, 80, 60);

      public override void OnPlayerReset() => this._game.slimeBossPosition = this._game.checkpointPosition + (float) this._game.slimeResetPosition;

      public override void InitializeObstacle(MineCart.Track track) => base.InitializeObstacle(track);

      protected override void _Initialize()
      {
        base._Initialize();
        this._game.slimeBossPosition = (float) this._game.slimeResetPosition;
        this._grounded = false;
      }

      public override void _Draw(SpriteBatch b)
      {
        SpriteEffects effects = SpriteEffects.None;
        Rectangle rectangle = new Rectangle(160, 176, 96, 80);
        if (this._currentFrame == 0)
          rectangle = new Rectangle(160, 176, 96, 80);
        else if (this._currentFrame == 1)
          rectangle = new Rectangle(160, 256, 96, 80);
        else if (this._currentFrame == 2)
          rectangle = new Rectangle(160, 336, 96, 64);
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(rectangle), Color.White, 0.0f, new Vector2((float) rectangle.Width * 0.5f, (float) rectangle.Height), this._game.GetPixelScale() * this.spriteScale, effects, 0.45f);
      }

      protected override void _Update(float time)
      {
        MineCart.Track trackForXposition = this._game.GetTrackForXPosition(this.position.X);
        float num = (float) (this._game.screenHeight + 32);
        if (trackForXposition != null)
        {
          this._lastTrackY = (float) trackForXposition.GetYAtPoint(this.position.X);
          num = this._lastTrackY;
        }
        this._game.slimeBossPosition += this._game.slimeBossSpeed * time;
        if (this._grounded)
        {
          this._timeUntilHop -= time;
          if ((double) this._timeUntilHop <= 0.0)
          {
            this._grounded = false;
            this.spriteScale = new Vector2(1.1f, 0.75f);
            this._desiredScale = new Vector2(1f, 1f);
            this._scaleSpeed = 1f;
            this._yVelocity = this._jumpStrength;
            Game1.playSound("dwoop");
            for (int index = 0; index < 8; ++index)
              this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(192, 112, 16, 16), new Vector2((float) this.GetBounds().Center.X, (float) this.GetBounds().Bottom) + new Vector2((float) Game1.random.Next(-32, 33), (float) Game1.random.Next(-32, 0)), (float) Game1.random.Next(-10, 11), (float) Game1.random.Next(-50, -29), 0.25f, 0.25f, 1f, num_animation_frames: 4, animation_interval: 0.25f, draw_depth: 0.46f));
          }
          else if ((double) this._timeUntilHop <= 0.25)
          {
            if (!this._hasPeparedToJump)
            {
              this.spriteScale = new Vector2(0.9f, 1.1f);
              this._desiredScale = new Vector2(1f, 1f);
              this._scaleSpeed = 1f;
              this._currentFrame = 2;
              this._hasPeparedToJump = true;
            }
          }
          else
          {
            this._desiredScale = new Vector2(1f, 1f);
            this._scaleSpeed = 4f;
          }
        }
        else
        {
          this._currentFrame = 1;
          if ((double) this.position.X > (double) this._game.slimeBossPosition)
            this.position.X = Utility.MoveTowards(this.position.X, this._game.slimeBossPosition, (float) ((double) this._game.slimeBossSpeed * (double) time * 8.0));
          else
            this.position.X = Utility.MoveTowards(this.position.X, this._game.slimeBossPosition, (float) ((double) this._game.slimeBossSpeed * (double) time * 2.0));
          this._yVelocity += 200f * time;
          this.position.Y += this._yVelocity * time;
          if ((double) this.position.Y > (double) this._lastTrackY && (double) this._yVelocity < 0.0)
            this._yVelocity = this._jumpStrength;
          if ((double) this._yVelocity < 0.0)
          {
            this._desiredScale = new Vector2(0.9f, 1.1f);
            this._scaleSpeed = 5f;
          }
          else if ((double) this._yVelocity > 0.0)
          {
            this._desiredScale = new Vector2(1f, 1f);
            this._scaleSpeed = 0.25f;
          }
          if ((double) this.position.Y > (double) num && (double) this._yVelocity > 0.0)
          {
            Game1.playSound("slimedead");
            Game1.playSound("breakingGlass");
            for (int index = 0; index < 8; ++index)
              this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(192, 112, 16, 16), new Vector2((float) this.GetBounds().Center.X, (float) this.GetBounds().Bottom) + new Vector2((float) Game1.random.Next(-32, 33), (float) Game1.random.Next(-32, 0)), (float) Game1.random.Next(-80, 81), (float) Game1.random.Next(-10, 1), 0.25f, 0.25f, 1f, num_animation_frames: 4, animation_interval: 0.25f, draw_depth: 0.46f));
            this._game.shakeMagnitude = 1.5f;
            this.position.Y = num;
            this._grounded = true;
            this._timeUntilHop = 0.5f;
            this._currentFrame = 2;
            this._hasPeparedToJump = false;
            this.spriteScale = new Vector2(1.1f, 0.75f);
          }
        }
        this.spriteScale.X = Utility.MoveTowards(this.spriteScale.X, this._desiredScale.X, this._scaleSpeed * time);
        this.spriteScale.Y = Utility.MoveTowards(this.spriteScale.Y, this._desiredScale.Y, this._scaleSpeed * time);
      }

      public override bool ShouldReap() => false;
    }

    public class Roadblock : MineCart.Obstacle
    {
      public override Rectangle GetLocalBounds() => new Rectangle(-4, -12, 8, 12);

      protected override void _Update(float time)
      {
      }

      public override void _Draw(SpriteBatch b) => b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(16, 0, 16, 16)), Color.White, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale(), SpriteEffects.None, 0.45f);

      public override bool CanSpawnHere(MineCart.Track track) => track != null && track.trackType == MineCart.Track.TrackType.Straight;

      public override bool OnBounce(MineCart.MineCartCharacter player)
      {
        if (!(player is MineCart.PlayerMineCartCharacter))
          return false;
        this.ShootDebris(Game1.random.Next(-10, -4), Game1.random.Next(-60, -19));
        this.ShootDebris(Game1.random.Next(5, 11), Game1.random.Next(-60, -19));
        this.ShootDebris(Game1.random.Next(-20, -9), Game1.random.Next(-40, 0));
        this.ShootDebris(Game1.random.Next(10, 21), Game1.random.Next(-40, 0));
        Game1.playSound("woodWhack");
        player.velocity.Y = 0.0f;
        player.velocity.Y = 0.0f;
        this.Destroy();
        return true;
      }

      public override bool OnBump(MineCart.PlayerMineCartCharacter player)
      {
        this.ShootDebris(Game1.random.Next(10, 41), Game1.random.Next(-40, 0));
        this.ShootDebris(Game1.random.Next(10, 41), Game1.random.Next(-40, 0));
        this.ShootDebris(Game1.random.Next(5, 31), Game1.random.Next(-60, -19));
        this.ShootDebris(Game1.random.Next(5, 31), Game1.random.Next(-60, -19));
        Game1.playSound("woodWhack");
        this.Destroy();
        return false;
      }

      public void ShootDebris(int x, int y) => this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(48, 48, 16, 16), Utility.PointToVector2(this.GetBounds().Center), (float) x, (float) y, 0.25f, life_time: 1f));
    }

    public class MineDebris : MineCart.Entity
    {
      protected Rectangle _sourceRect;
      protected float _dX;
      protected float _dY;
      protected float _age;
      protected float _lifeTime;
      protected float _gravityMultiplier;
      protected float _scale = 1f;
      protected Color _color = Color.White;
      protected int _numAnimationFrames;
      protected bool _holdLastFrame;
      protected float _animationInterval;
      protected int _currentAnimationFrame;
      protected float _animationTimer;
      public float ySinWaveMagnitude;
      public float flipRate;
      public float depth = 0.45f;
      private float timeBeforeDisplay;
      private string destroySound;
      private string startSound;

      public MineDebris(
        Rectangle source_rect,
        Vector2 spawn_position,
        float dx,
        float dy,
        float flip_rate = 0.0f,
        float gravity_multiplier = 1f,
        float life_time = 0.5f,
        float scale = 1f,
        int num_animation_frames = 1,
        float animation_interval = 0.1f,
        float draw_depth = 0.45f,
        bool holdLastFrame = false,
        float timeBeforeDisplay = 0.0f)
      {
        this.reset(source_rect, spawn_position, dx, dy, flip_rate, gravity_multiplier, life_time, scale, num_animation_frames, animation_interval, draw_depth, holdLastFrame, timeBeforeDisplay);
      }

      public void reset(
        Rectangle source_rect,
        Vector2 spawn_position,
        float dx,
        float dy,
        float flip_rate = 0.0f,
        float gravity_multiplier = 1f,
        float life_time = 0.5f,
        float scale = 1f,
        int num_animation_frames = 1,
        float animation_interval = 0.1f,
        float draw_depth = 0.45f,
        bool holdLastFrame = false,
        float timeBeforeDisplay = 0.0f)
      {
        this._sourceRect = source_rect;
        this._dX = dx;
        this._dY = dy;
        this._lifeTime = life_time;
        this.flipRate = flip_rate;
        this.position = spawn_position;
        this._gravityMultiplier = gravity_multiplier;
        this._scale = scale;
        this._numAnimationFrames = num_animation_frames;
        this._animationInterval = animation_interval;
        this.depth = draw_depth;
        this._holdLastFrame = holdLastFrame;
        this._currentAnimationFrame = 0;
        this.timeBeforeDisplay = timeBeforeDisplay;
        if ((double) timeBeforeDisplay <= 0.0)
          return;
        this.visible = false;
      }

      public void SetColor(Color color) => this._color = color;

      public void SetDestroySound(string sound) => this.destroySound = sound;

      public void SetStartSound(string sound) => this.startSound = sound;

      protected override void _Update(float time)
      {
        if ((double) this.timeBeforeDisplay > 0.0)
        {
          this.timeBeforeDisplay -= time;
          if ((double) this.timeBeforeDisplay > 0.0)
            return;
          this.visible = true;
          if (this.startSound != null)
            Game1.playSound(this.startSound);
        }
        this.position.X += this._dX * time;
        this.position.Y += this._dY * time;
        this._dY += 210f * time * this._gravityMultiplier;
        this._age += time;
        if ((double) this._age >= (double) this._lifeTime)
        {
          if (this.destroySound != null)
            Game1.playSound(this.destroySound);
          this.Destroy();
        }
        else
        {
          this._animationTimer += time;
          if ((double) this._animationTimer >= (double) this._animationInterval)
          {
            this._animationTimer = 0.0f;
            ++this._currentAnimationFrame;
            if (this._holdLastFrame && this._currentAnimationFrame >= this._numAnimationFrames - 1)
              this._currentAnimationFrame = this._numAnimationFrames - 1;
            else
              this._currentAnimationFrame %= this._numAnimationFrames;
          }
          base._Update(time);
        }
      }

      private Rectangle _GetSourceRect() => new Rectangle(this._sourceRect.X + this._currentAnimationFrame * this._sourceRect.Width, this._sourceRect.Y, this._sourceRect.Width, this._sourceRect.Height);

      public override void _Draw(SpriteBatch b)
      {
        SpriteEffects effects = SpriteEffects.None;
        if ((double) this.flipRate > 0.0 && Math.Floor((double) this._age / (double) this.flipRate) % 2.0 == 0.0)
          effects = SpriteEffects.FlipHorizontally;
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + new Vector2(0.0f, (float) Math.Sin(this._game.totalTime + (double) this.position.X) * this.ySinWaveMagnitude)), new Rectangle?(this._GetSourceRect()), this._color, 0.0f, new Vector2((float) this._sourceRect.Width / 2f, (float) this._sourceRect.Height / 2f), this._game.GetPixelScale() * this._scale, effects, this.depth);
      }
    }

    public class Obstacle : MineCart.Entity, MineCart.ICollideable
    {
      public virtual void InitializeObstacle(MineCart.Track track)
      {
      }

      public virtual bool OnBounce(MineCart.MineCartCharacter player) => false;

      public virtual bool OnBump(MineCart.PlayerMineCartCharacter player) => false;

      public virtual Rectangle GetLocalBounds() => new Rectangle(-4, -12, 8, 12);

      public virtual Rectangle GetBounds()
      {
        Rectangle localBounds = this.GetLocalBounds();
        localBounds.X += (int) this.position.X;
        localBounds.Y += (int) this.position.Y;
        return localBounds;
      }

      public override void _Draw(SpriteBatch b) => b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(16, 0, 16, 16)), Color.White, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale(), SpriteEffects.None, 0.45f);

      public virtual bool CanSpawnHere(MineCart.Track track) => true;
    }

    public class Fruit : MineCart.Pickup
    {
      protected MineCart.CollectableFruits _fruitType;

      public override Rectangle GetLocalBounds() => new Rectangle(-6, -6, 12, 12);

      public Fruit(MineCart.CollectableFruits fruit_type) => this._fruitType = fruit_type;

      public override void Collect(MineCart.PlayerMineCartCharacter player)
      {
        this._game.CollectFruit(this._fruitType);
        this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(0, 250, 5, 5), this.position, 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 0.6f, num_animation_frames: 6));
        for (int index = 0; index < 4; ++index)
        {
          float animation_interval = Utility.Lerp(0.1f, 0.2f, (float) Game1.random.NextDouble());
          this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(0, 250, 5, 5), this.position + new Vector2((float) Game1.random.Next(-8, 9), (float) Game1.random.Next(-8, 9)), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: (animation_interval * 6f), num_animation_frames: 6, animation_interval: animation_interval));
        }
        Game1.playSound("eat");
        this.Destroy();
      }

      public override void _Draw(SpriteBatch b) => b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(160 + 16 * (int) this._fruitType, 0, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), this._game.GetPixelScale(), SpriteEffects.None, 0.43f);
    }

    public class Coin : MineCart.Pickup
    {
      public float age;
      public float afterCollectionTimer;
      public bool collected;
      public float flashSpeed = 0.25f;
      public float flashDelay = 0.5f;
      public float collectYDelta;

      protected override void _Update(float time)
      {
        this.age += time;
        if ((double) this.age > (double) this.flashDelay + (double) this.flashSpeed * 3.0)
          this.age = 0.0f;
        if (this.collected)
        {
          this.afterCollectionTimer += time;
          if ((double) time > 0.0)
            this.position.Y -= (float) (3.0 - (double) this.afterCollectionTimer * 8.0 * (double) time);
          if ((double) this.afterCollectionTimer > 0.400000005960464)
            this.Destroy();
        }
        base._Update(time);
      }

      public override void _Draw(SpriteBatch b)
      {
        int num = this.collected ? 450 : 900;
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(9 * ((int) this._game.totalTimeMS % num / (num / 12)), 273, 9, 9)), Color.White * (float) (1.0 - (double) this.afterCollectionTimer / 0.400000005960464), 0.0f, new Vector2(4f, 4f), this._game.GetPixelScale(), SpriteEffects.None, 0.45f);
      }

      public override void Collect(MineCart.PlayerMineCartCharacter player)
      {
        if (this.collected)
          return;
        this._game.CollectCoin(1);
        Game1.playSound("junimoKart_coin");
        this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(0, 250, 5, 5), this.position, 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: 0.6f, num_animation_frames: 6));
        for (int index = 0; index < 4; ++index)
        {
          float animation_interval = Utility.Lerp(0.1f, 0.2f, (float) Game1.random.NextDouble());
          this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(0, 250, 5, 5), this.position + new Vector2((float) Game1.random.Next(-8, 9), (float) Game1.random.Next(-8, 9)), 0.0f, 0.0f, gravity_multiplier: 0.0f, life_time: (animation_interval * 6f), num_animation_frames: 6, animation_interval: animation_interval));
        }
        this.collectYDelta = -3f;
        this.collected = true;
      }
    }

    public class Pickup : MineCart.Entity, MineCart.ICollideable
    {
      public virtual Rectangle GetLocalBounds() => new Rectangle(-4, -4, 8, 8);

      public virtual Rectangle GetBounds()
      {
        Rectangle localBounds = this.GetLocalBounds();
        localBounds.X += (int) this.position.X;
        localBounds.Y += (int) this.position.Y;
        return localBounds;
      }

      public override void _Draw(SpriteBatch b) => b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(16, 16, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), this._game.GetPixelScale(), SpriteEffects.None, 0.45f);

      public virtual void Collect(MineCart.PlayerMineCartCharacter player)
      {
        Game1.playSound("Pickup_Coin15");
        this.Destroy();
      }
    }

    public class BalanceTrack : MineCart.Track
    {
      public List<MineCart.BalanceTrack> connectedTracks;
      public List<MineCart.BalanceTrack> counterBalancedTracks;
      public float startY;
      public float moveSpeed = 128f;

      public BalanceTrack(MineCart.Track.TrackType type, bool showSecondTile)
        : base(type, showSecondTile)
      {
        this.connectedTracks = new List<MineCart.BalanceTrack>();
        this.counterBalancedTracks = new List<MineCart.BalanceTrack>();
      }

      public override void OnPlayerReset() => this.position.Y = this.startY;

      public override void WhileCartGrounded(MineCart.MineCartCharacter character, float time)
      {
        foreach (MineCart.Entity connectedTrack in this.connectedTracks)
          connectedTrack.position.Y += this.moveSpeed * time;
        foreach (MineCart.Entity counterBalancedTrack in this.counterBalancedTracks)
          counterBalancedTrack.position.Y -= this.moveSpeed * time;
      }
    }

    public class Track : MineCart.Entity
    {
      public MineCart.Obstacle obstacle;
      private bool _showSecondTile;
      public MineCart.Track.TrackType trackType;

      public Track(MineCart.Track.TrackType type, bool showSecondTile)
      {
        this.trackType = type;
        this._showSecondTile = showSecondTile;
      }

      public virtual void WhileCartGrounded(MineCart.MineCartCharacter character, float time)
      {
      }

      public override void _Draw(SpriteBatch b)
      {
        if (this.trackType == MineCart.Track.TrackType.SlimeUpSlope)
        {
          b.Draw(this._game.texture, this._game.TransformDraw(new Vector2(this.drawnPosition.X, this.drawnPosition.Y - 32f)), new Rectangle?(new Rectangle(192, 144, 16, 32)), this._game.trackTint, 0.0f, Vector2.Zero, this._game.GetPixelScale(), SpriteEffects.None, (float) (0.5 + (double) this.drawnPosition.Y * 9.99999974737875E-06));
          b.Draw(this._game.texture, this._game.TransformDraw(new Vector2(this.drawnPosition.X, this.drawnPosition.Y - 32f)), new Rectangle?(new Rectangle(160 + (int) this.trackType * 16, 144, 16, 32)), Color.White, 0.0f, Vector2.Zero, this._game.GetPixelScale(), SpriteEffects.None, (float) (0.5 + (double) this.drawnPosition.Y * 9.99999974737875E-06 - 9.99999974737875E-05));
        }
        else if (this.trackType >= MineCart.Track.TrackType.MushroomLeft && this.trackType <= MineCart.Track.TrackType.MushroomRight)
        {
          if (this.GetType() == typeof (MineCart.Track))
            b.Draw(this._game.texture, this._game.TransformDraw(new Vector2(this.drawnPosition.X, this.drawnPosition.Y - 32f)), new Rectangle?(new Rectangle(304 + (int) (this.trackType - 6) * 16, 736, 16, 48)), Color.White, 0.0f, Vector2.Zero, this._game.GetPixelScale(), SpriteEffects.None, (float) (0.5 + (double) this.drawnPosition.Y * 9.99999974737875E-06));
          else
            b.Draw(this._game.texture, this._game.TransformDraw(new Vector2(this.drawnPosition.X, this.drawnPosition.Y - 32f)), new Rectangle?(new Rectangle(352 + (int) (this.trackType - 6) * 16, 736, 16, 48)), Color.White, 0.0f, Vector2.Zero, this._game.GetPixelScale(), SpriteEffects.None, (float) (0.5 + (double) this.drawnPosition.Y * 9.99999974737875E-06));
        }
        else if (this._game.currentTheme == 4 && (this.trackType == MineCart.Track.TrackType.UpSlope || this.trackType == MineCart.Track.TrackType.DownSlope))
          b.Draw(this._game.texture, this._game.TransformDraw(new Vector2(this.drawnPosition.X, this.drawnPosition.Y - 32f)), new Rectangle?(new Rectangle(256 + (int) (this.trackType - 2) * 16, 144, 16, 32)), this._game.trackTint, 0.0f, Vector2.Zero, this._game.GetPixelScale(), SpriteEffects.None, (float) (0.5 + (double) this.drawnPosition.Y * 9.99999974737875E-06));
        else
          b.Draw(this._game.texture, this._game.TransformDraw(new Vector2(this.drawnPosition.X, this.drawnPosition.Y - 32f)), new Rectangle?(new Rectangle(160 + (int) this.trackType * 16, 144, 16, 32)), this._game.trackTint, 0.0f, Vector2.Zero, this._game.GetPixelScale(), SpriteEffects.None, (float) (0.5 + (double) this.drawnPosition.Y * 9.99999974737875E-06));
        if (this.trackType == MineCart.Track.TrackType.MushroomLeft || this.trackType == MineCart.Track.TrackType.MushroomRight)
          return;
        float num = 0.0f;
        if (this.trackType == MineCart.Track.TrackType.MushroomMiddle)
        {
          for (float y = this.drawnPosition.Y; (double) y < (double) this._game.screenHeight; y += (float) (this._game.tileSize * 4))
          {
            b.Draw(this._game.texture, this._game.TransformDraw(new Vector2(this.drawnPosition.X, y + 16f)), new Rectangle?(new Rectangle(320, 784, 16, 64)), Color.White, 0.0f, Vector2.Zero, this._game.GetPixelScale(), SpriteEffects.None, (float) (0.5 + (double) this.drawnPosition.Y * 9.99999974737875E-06 + 0.00999999977648258));
            b.Draw(this._game.texture, this._game.TransformDraw(new Vector2(this.drawnPosition.X, y + 16f)), new Rectangle?(new Rectangle(368, 784, 16, 64)), this._game.trackShadowTint * num, 0.0f, Vector2.Zero, this._game.GetPixelScale(), SpriteEffects.None, (float) (0.5 + (double) this.drawnPosition.Y * 9.99999974737875E-06 + 0.00499999988824129));
            num += 0.1f;
          }
        }
        else
        {
          bool flag = this._showSecondTile;
          for (float y = this.drawnPosition.Y; (double) y < (double) this._game.screenHeight; y += (float) this._game.tileSize)
          {
            b.Draw(this._game.texture, this._game.TransformDraw(new Vector2(this.drawnPosition.X, y)), new Rectangle?(this._game.currentTheme == 4 ? new Rectangle(16 + (flag ? 1 : 0) * 16, 160, 16, 16) : new Rectangle(16 + (flag ? 1 : 0) * 16, 32, 16, 16)), this._game.trackTint, 0.0f, Vector2.Zero, this._game.GetPixelScale(), SpriteEffects.None, (float) (0.5 + (double) this.drawnPosition.Y * 9.99999974737875E-06 + 0.00999999977648258));
            b.Draw(this._game.texture, this._game.TransformDraw(new Vector2(this.drawnPosition.X, y)), new Rectangle?(this._game.currentTheme == 4 ? new Rectangle(16 + (flag ? 1 : 0) * 16, 160, 16, 16) : new Rectangle(16 + (flag ? 1 : 0) * 16, 32, 16, 16)), this._game.trackShadowTint * num, 0.0f, Vector2.Zero, this._game.GetPixelScale(), SpriteEffects.None, (float) (0.5 + (double) this.drawnPosition.Y * 9.99999974737875E-06 + 0.00499999988824129));
            num += 0.1f;
            flag = !flag;
          }
        }
      }

      public bool CanLandHere(Vector2 test_position)
      {
        int yatPoint = this.GetYAtPoint(test_position.X);
        return (double) test_position.Y >= (double) (yatPoint - 2) && (double) test_position.Y <= (double) (yatPoint + 8);
      }

      public int GetYAtPoint(float x)
      {
        int num = (int) ((double) x - (double) this.position.X);
        if (this.trackType == MineCart.Track.TrackType.UpSlope)
          return (int) ((double) this.position.Y - 2.0 - (double) num);
        if (this.trackType == MineCart.Track.TrackType.DownSlope)
          return (int) ((double) this.position.Y - 2.0 - 16.0 + (double) num);
        if (this.trackType == MineCart.Track.TrackType.IceDownSlope)
          return (int) ((double) this.position.Y - 2.0 - 16.0 + (double) num);
        return this.trackType == MineCart.Track.TrackType.SlimeUpSlope ? (int) ((double) this.position.Y - 2.0 - (double) num) : (int) ((double) this.position.Y - 2.0);
      }

      public enum TrackType
      {
        None = -1, // 0xFFFFFFFF
        Straight = 0,
        UpSlope = 2,
        DownSlope = 3,
        IceDownSlope = 4,
        SlimeUpSlope = 5,
        MushroomLeft = 6,
        MushroomMiddle = 7,
        MushroomRight = 8,
      }
    }

    public class PlayerMineCartCharacter : MineCart.MineCartCharacter, MineCart.ICollideable
    {
      public Rectangle GetLocalBounds() => new Rectangle(-4, -12, 8, 12);

      public virtual Rectangle GetBounds()
      {
        Rectangle localBounds = this.GetLocalBounds();
        localBounds.X += (int) this.position.X;
        localBounds.Y += (int) this.position.Y;
        return localBounds;
      }

      protected override void _Update(float time)
      {
        if (!this.IsActive())
          return;
        int num = (int) ((double) this.position.X / (double) this._game.tileSize);
        float y = this.velocity.Y;
        if (this._game.gameState != MineCart.GameStates.Cutscene && this._jumping && !this._game.isJumpPressed && !this._game.gamePaused)
          this.ReleaseJump();
        base._Update(time);
        if (this._grounded && this._game.respawnCounter <= 0)
        {
          if (this._game.minecartLoop.IsPaused && this._game.currentTheme != 7)
            this._game.minecartLoop.Resume();
          if (num != (int) ((double) this.position.X / (double) this._game.tileSize) && Game1.random.NextDouble() < 0.5)
            this.minecartBumpOffset = (float) -Game1.random.Next(1, 3);
        }
        else if (!this._grounded)
        {
          if (!this._game.minecartLoop.IsPaused)
            this._game.minecartLoop.Pause();
          this.minecartBumpOffset = 0.0f;
        }
        this.minecartBumpOffset = Utility.MoveTowards(this.minecartBumpOffset, 0.0f, time * 20f);
        foreach (MineCart.Pickup overlap in this._game.GetOverlaps<MineCart.Pickup>((MineCart.ICollideable) this))
          overlap.Collect(this);
        MineCart.Obstacle overlap1 = this._game.GetOverlap<MineCart.Obstacle>((MineCart.ICollideable) this);
        if (this._game.GetOverlap<MineCart.Obstacle>((MineCart.ICollideable) this) == null || ((double) this.velocity.Y > 0.0 || (double) y > 0.0 || (double) this.position.Y < (double) overlap1.position.Y - 1.0) && overlap1.OnBounce((MineCart.MineCartCharacter) this) || overlap1.OnBump(this))
          return;
        this._game.Die();
      }

      public override void OnJump()
      {
        if (Game1.soundBank == null)
          return;
        ICue cue = Game1.soundBank.GetCue("pickUpItem");
        cue.SetVariable("Pitch", 200);
        cue.Play();
      }

      public override void OnFall()
      {
        Game1.soundBank.GetCue("parry").Play();
        this._game.createSparkShower();
      }

      public override void OnLand()
      {
        if (this.currentTrackType == MineCart.Track.TrackType.SlimeUpSlope)
        {
          Game1.playSound("slimeHit");
        }
        else
        {
          if (this.currentTrackType >= MineCart.Track.TrackType.MushroomLeft && this.currentTrackType <= MineCart.Track.TrackType.MushroomRight)
          {
            Game1.playSound("slimeHit");
            bool flag = false;
            if (this.GetTrack().GetType() != typeof (MineCart.Track))
              flag = true;
            for (int index = 0; index < 3; ++index)
              this._game.AddEntity<MineCart.MineDebris>(new MineCart.MineDebris(new Rectangle(362 + (flag ? 5 : 0), 802, 5, 4), this.position, (float) Game1.random.Next(-30, 31), (float) Game1.random.Next(-50, -39), life_time: 0.75f, animation_interval: 1f, draw_depth: 0.15f));
            return;
          }
          Game1.soundBank.GetCue("parry").Play();
        }
        this._game.createSparkShower();
      }

      public override void OnTrackChange()
      {
        if (this._hasJustSnapped || !this._grounded)
          return;
        if (this.currentTrackType == MineCart.Track.TrackType.SlimeUpSlope)
        {
          Game1.playSound("slimeHit");
        }
        else
        {
          if (this.currentTrackType >= MineCart.Track.TrackType.MushroomLeft && this.currentTrackType <= MineCart.Track.TrackType.MushroomRight)
            return;
          Game1.soundBank.GetCue("parry").Play();
        }
        this._game.createSparkShower();
      }
    }

    public class CheckpointIndicator : MineCart.Entity
    {
      public const int CENTER_TO_POST_BASE_OFFSET = 5;
      public float rotation;
      protected bool _activated;
      public float swayRotation = 120f;
      public float swayTimer;

      protected override void _Update(float time)
      {
        if (!this._activated)
          return;
        this.swayTimer += time * 6.283185f;
        if ((double) this.swayTimer >= 2.0 * Math.PI)
        {
          this.swayTimer = 0.0f;
          this.swayRotation -= 20f;
          if ((double) this.swayRotation <= 30.0)
            this.swayRotation = 30f;
        }
        this.rotation = (float) Math.Sin((double) this.swayTimer) * this.swayRotation;
      }

      public void Activate()
      {
        if (this._activated)
          return;
        Game1.playSound("fireball");
        this._activated = true;
      }

      public override void _Draw(SpriteBatch b)
      {
        float rotation = (float) ((double) this.rotation * 3.14159274101257 / 180.0);
        Vector2 vector2 = new Vector2(0.0f, -12f);
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(16, 112, 16, 16)), this._game.trackTint, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale(), SpriteEffects.None, 0.31f);
        if (this._activated)
          b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + vector2), new Rectangle?(new Rectangle(48, 112, 16, 16)), Color.White, rotation, new Vector2(8f, 16f) + vector2, this._game.GetPixelScale(), SpriteEffects.None, 0.3f);
        else
          b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + vector2), new Rectangle?(new Rectangle(32, 112, 16, 16)), Color.White, rotation, new Vector2(8f, 16f) + vector2, this._game.GetPixelScale(), SpriteEffects.None, 0.3f);
      }
    }

    public class GoalIndicator : MineCart.Entity
    {
      public float rotation;
      protected bool _activated;

      public void Activate()
      {
        if (this._activated)
          return;
        this._activated = true;
      }

      protected override void _Update(float time)
      {
        if (!this._activated)
          return;
        this.rotation += (float) ((double) time * 360.0 / 0.25);
      }

      public override void _Draw(SpriteBatch b)
      {
        float rotation = (float) ((double) this.rotation * 3.14159274101257 / 180.0);
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition), new Rectangle?(new Rectangle(16, 128, 16, 16)), this._game.trackTint, 0.0f, new Vector2(8f, 16f), this._game.GetPixelScale(), SpriteEffects.None, 0.31f);
        Vector2 vector2 = new Vector2(0.0f, -8f);
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + vector2), new Rectangle?(new Rectangle(32, 128, 16, 16)), Color.White, rotation, new Vector2(8f, 16f) + vector2, this._game.GetPixelScale(), SpriteEffects.None, 0.3f);
      }
    }

    public class MineCartCharacter : MineCart.BaseCharacter
    {
      public float minecartBumpOffset;
      public float jumpStrength = 300f;
      public float maxFallSpeed = 150f;
      public float jumpGravity = 3400f;
      public float fallGravity = 3000f;
      public float jumpFloatDuration = 0.1f;
      public float gravity;
      protected float _jumpBuffer;
      protected float _jumpFloatAge;
      protected float _speedMultiplier = 1f;
      protected float _jumpMomentumThreshhold = -30f;
      public float jumpGracePeriod;
      public float respawnCounter;
      protected bool _grounded = true;
      protected bool _jumping;
      public bool jumpHeld;
      public float rotation;
      public Vector2 cartScale = Vector2.One;
      public MineCart.Track.TrackType currentTrackType = MineCart.Track.TrackType.None;
      public float characterExtraHeight;
      protected bool _hasJustSnapped;
      public float forcedJumpTime;

      public void QueueJump() => this._jumpBuffer = 0.25f;

      public virtual void OnDie()
      {
        this.cartScale = Vector2.One;
        this._speedMultiplier = 1f;
      }

      public void SnapToFloor()
      {
        List<MineCart.Track> tracksForXposition = this._game.GetTracksForXPosition(this.position.X);
        if (tracksForXposition == null)
          return;
        int index = 0;
        if (index >= tracksForXposition.Count)
          return;
        this.position.Y = (float) tracksForXposition[index].GetYAtPoint(this.position.X);
        this._grounded = true;
        this.gravity = 0.0f;
        this.velocity.Y = 0.0f;
        this.characterExtraHeight = 0.0f;
        this.minecartBumpOffset = 0.0f;
        this._hasJustSnapped = true;
      }

      public MineCart.Track GetTrack(Vector2 offset = default (Vector2))
      {
        int[] numArray = new int[3]{ 0, 4, -4 };
        foreach (int x in numArray)
        {
          Vector2 test_position = this.position + offset + new Vector2((float) x, 0.0f);
          List<MineCart.Track> tracksForXposition = this._game.GetTracksForXPosition(test_position.X);
          if (tracksForXposition != null)
          {
            for (int index = 0; index < tracksForXposition.Count; ++index)
            {
              if (tracksForXposition[index].CanLandHere(test_position))
                return tracksForXposition[index];
            }
          }
        }
        return (MineCart.Track) null;
      }

      protected override void _Update(float time)
      {
        if (this._game.respawnCounter > 0)
        {
          this.characterExtraHeight = 0.0f;
          this.rotation = 0.0f;
          this._jumpBuffer = 0.0f;
          this.jumpGracePeriod = 0.0f;
          this.gravity = 0.0f;
          this.velocity.Y = 0.0f;
          this.minecartBumpOffset = 0.0f;
          this.SnapToFloor();
        }
        else
        {
          base._Update(time);
          if ((double) this.jumpGracePeriod > 0.0)
            this.jumpGracePeriod -= time;
          if (this._grounded && (double) this._jumpBuffer > 0.0 && this._game.isJumpPressed)
          {
            this._jumpBuffer = 0.0f;
            this.Jump();
          }
          else if ((double) this._jumpBuffer > 0.0)
            this._jumpBuffer -= time;
          bool flag = false;
          MineCart.Track.TrackType currentTrackType = this.currentTrackType;
          MineCart.Track track1 = this.GetTrack();
          if (track1 != null && this._grounded)
            track1.WhileCartGrounded(this, time);
          int num = this._grounded ? 1 : 0;
          if ((double) this.velocity.Y >= 0.0 && track1 != null)
          {
            this.position.Y = (float) track1.GetYAtPoint(this.position.X);
            this.currentTrackType = track1.trackType;
            if (!this._grounded)
            {
              this.cartScale = new Vector2(1.5f, 0.5f);
              this.rotation = 0.0f;
              this.OnLand();
            }
            flag = true;
            this.velocity.Y = 0.0f;
            this._grounded = true;
          }
          else if (this._grounded && (double) this.velocity.Y >= 0.0)
          {
            MineCart.Track track2 = this.GetTrack(new Vector2(0.0f, 2f));
            if (track2 != null)
            {
              this.position.Y = (float) track2.GetYAtPoint(this.position.X);
              this.currentTrackType = track2.trackType;
              flag = true;
              this.velocity.Y = 0.0f;
              this._grounded = true;
            }
          }
          if (!flag)
          {
            if (this._grounded)
            {
              this.gravity = 0.0f;
              this.velocity.Y = this.GetMaxFallSpeed();
              if (!this.IsJumping())
              {
                this.OnFall();
                this.jumpGracePeriod = 0.2f;
              }
            }
            this.currentTrackType = MineCart.Track.TrackType.None;
            this._grounded = false;
          }
          float to = 0.0f;
          if (this.currentTrackType == MineCart.Track.TrackType.Straight)
            to = 0.0f;
          else if (this.currentTrackType == MineCart.Track.TrackType.UpSlope)
            to = -45f;
          else if (this.currentTrackType == MineCart.Track.TrackType.DownSlope)
            to = 30f;
          if (this.IsJumping())
          {
            this.rotation = Utility.MoveTowards(this.rotation, -45f, 300f * time);
            this.characterExtraHeight = 0.0f;
          }
          else if (!this._grounded)
          {
            this.rotation = Utility.MoveTowards(this.rotation, 0.0f, 100f * time);
            this.characterExtraHeight = Utility.MoveTowards(this.characterExtraHeight, 16f, 24f * time);
          }
          else
          {
            this.rotation = Utility.MoveTowards(this.rotation, to, 360f * time);
            this.characterExtraHeight = Utility.MoveTowards(this.characterExtraHeight, 0.0f, 128f * time);
          }
          this.cartScale.X = Utility.MoveTowards(this.cartScale.X, 1f, 4f * time);
          this.cartScale.Y = Utility.MoveTowards(this.cartScale.Y, 1f, 4f * time);
          if (num != 0 && currentTrackType != this.currentTrackType)
          {
            if ((double) this.rotation < 0.0 && (double) to > 0.0 || (double) this.rotation > 0.0 && (double) to < 0.0)
              this.rotation = 0.0f;
            this.OnTrackChange();
          }
          if ((double) this.forcedJumpTime > 0.0)
          {
            this.forcedJumpTime -= time;
            if (this._grounded)
              this.forcedJumpTime = 0.0f;
          }
          if (!this._grounded)
          {
            if (this._jumping)
            {
              this._jumpFloatAge += time;
              if ((double) this._jumpFloatAge < (double) this.jumpFloatDuration)
              {
                this.gravity = 0.0f;
                this.velocity.Y = Utility.Lerp(0.0f, -this.jumpStrength, this._jumpFloatAge / this.jumpFloatDuration);
              }
              else if ((double) this.velocity.Y <= (double) this._jumpMomentumThreshhold * 2.0)
              {
                this.gravity += time * this.jumpGravity;
              }
              else
              {
                this.velocity.Y = this._jumpMomentumThreshhold;
                this.ReleaseJump();
              }
            }
            else
              this.gravity += time * this.fallGravity;
            this.velocity.Y += time * this.gravity;
          }
          else
            this._jumping = false;
          if (this._game.currentTheme == 5)
            this._speedMultiplier = 1f;
          if (this.currentTrackType == MineCart.Track.TrackType.SlimeUpSlope)
            this._speedMultiplier = 0.5f;
          else if (this.currentTrackType == MineCart.Track.TrackType.IceDownSlope)
            this._speedMultiplier = Utility.MoveTowards(this._speedMultiplier, 3f, time * 2f);
          else if (this._grounded)
            this._speedMultiplier = Utility.MoveTowards(this._speedMultiplier, 1f, time * 6f);
          if (!(this is MineCart.PlayerMineCartCharacter))
            this._speedMultiplier = 1f;
          this.position.X += time * this.velocity.X * this._speedMultiplier;
          this.position.Y += time * this.velocity.Y;
          if ((double) this.velocity.Y > 0.0)
            this._jumping = false;
          if ((double) this.velocity.Y > (double) this.GetMaxFallSpeed())
            this.velocity.Y = this.GetMaxFallSpeed();
          if (!this._hasJustSnapped)
            return;
          this._hasJustSnapped = false;
        }
      }

      public float GetMaxFallSpeed() => this._game.currentTheme == 2 ? 75f : this.maxFallSpeed;

      public virtual void OnLand()
      {
      }

      public virtual void OnTrackChange()
      {
      }

      public virtual void OnFall()
      {
      }

      public virtual void OnJump()
      {
      }

      public void ReleaseJump()
      {
        if ((double) this.forcedJumpTime > 0.0 || !this._jumping || (double) this.velocity.Y >= 0.0)
          return;
        this._jumping = false;
        this.gravity = 0.0f;
        if ((double) this.velocity.Y >= (double) this._jumpMomentumThreshhold)
          return;
        this.velocity.Y = this._jumpMomentumThreshhold;
      }

      public bool IsJumping() => this._jumping;

      public bool IsGrounded() => this._grounded;

      public void Bounce(float forced_bounce_time = 0.0f)
      {
        this.forcedJumpTime = forced_bounce_time;
        this._jumping = true;
        this.gravity = 0.0f;
        this.cartScale = new Vector2(0.5f, 1.5f);
        this.velocity.Y = -this.jumpStrength;
        this._grounded = false;
      }

      public void Jump()
      {
        if (!this._grounded && (double) this.jumpGracePeriod <= 0.0)
          return;
        this._jumping = true;
        this.gravity = 0.0f;
        this._jumpFloatAge = 0.0f;
        this.cartScale = new Vector2(0.5f, 1.5f);
        this.OnJump();
        this.velocity.Y = -this.jumpStrength;
        this._grounded = false;
      }

      public void ForceGrounded()
      {
        this._grounded = true;
        this.gravity = 0.0f;
        this.velocity.Y = 0.0f;
      }

      public override void _Draw(SpriteBatch b)
      {
        if (this._game.respawnCounter / 200 % 2 != 0)
          return;
        float num = (float) ((double) this.rotation * 3.14159274101257 / 180.0);
        Vector2 vector2_1 = new Vector2((float) Math.Cos((double) num), -(float) Math.Sin((double) num));
        Vector2 vector2_2 = new Vector2((float) Math.Sin((double) num), -(float) Math.Cos((double) num));
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + vector2_2 * -this.minecartBumpOffset + vector2_2 * 4f), new Rectangle?(new Rectangle(0, 0, 16, 16)), Color.White, num, new Vector2(8f, 14f), this.cartScale * this._game.GetPixelScale(), SpriteEffects.None, 0.45f);
        b.Draw(this._game.texture, this._game.TransformDraw(this.drawnPosition + vector2_2 * -this.minecartBumpOffset + vector2_2 * 4f), new Rectangle?(new Rectangle(0, 16, 16, 16)), Color.White, num, new Vector2(8f, 14f), this.cartScale * this._game.GetPixelScale(), SpriteEffects.None, 0.4f);
        b.Draw(Game1.mouseCursors, this._game.TransformDraw(this.drawnPosition + vector2_1 * -2f + vector2_2 * -this.minecartBumpOffset + vector2_2 * 12f + new Vector2(0.0f, -this.characterExtraHeight)), new Rectangle?(new Rectangle(294 + (int) (this._game.totalTimeMS % 400.0) / 100 * 16, 1432, 16, 16)), Color.Lime, 0.0f, new Vector2(8f, 8f), (float) ((double) this._game.GetPixelScale() * 2.0 / 3.0), SpriteEffects.None, 0.425f);
      }
    }
  }
}
