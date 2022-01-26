// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.MineShaft
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley.Locations
{
  public class MineShaft : GameLocation
  {
    public const int mineFrostLevel = 40;
    public const int mineLavaLevel = 80;
    public const int upperArea = 0;
    public const int jungleArea = 10;
    public const int frostArea = 40;
    public const int lavaArea = 80;
    public const int desertArea = 121;
    public const int bottomOfMineLevel = 120;
    public const int quarryMineShaft = 77377;
    public const int numberOfLevelsPerArea = 40;
    public const int mineFeature_barrels = 0;
    public const int mineFeature_chests = 1;
    public const int mineFeature_coalCart = 2;
    public const int mineFeature_elevator = 3;
    public const double chanceForColoredGemstone = 0.008;
    public const double chanceForDiamond = 0.0005;
    public const double chanceForPrismaticShard = 0.0005;
    public const int monsterLimit = 30;
    public static SerializableDictionary<int, MineInfo> permanentMineChanges = new SerializableDictionary<int, MineInfo>();
    public static int numberOfCraftedStairsUsedThisRun;
    private Random mineRandom = new Random();
    private LocalizedContentManager mineLoader = Game1.content.CreateTemporary();
    private int timeUntilElevatorLightUp;
    [XmlIgnore]
    public int loadedMapNumber;
    private int fogTime;
    private NetBool isFogUp = new NetBool();
    public static int timeSinceLastMusic = 200000;
    private bool ladderHasSpawned;
    private bool ghostAdded;
    private bool loadedDarkArea;
    private bool isFallingDownShaft;
    private Vector2 fogPos;
    private readonly NetBool elevatorShouldDing = new NetBool();
    public readonly NetString mapImageSource = new NetString();
    private readonly NetInt netMineLevel = new NetInt();
    private readonly NetIntDelta netStonesLeftOnThisLevel = new NetIntDelta();
    private readonly NetVector2 netTileBeneathLadder = new NetVector2();
    private readonly NetVector2 netTileBeneathElevator = new NetVector2();
    private readonly NetPoint netElevatorLightSpot = new NetPoint();
    private readonly NetBool netIsSlimeArea = new NetBool();
    private readonly NetBool netIsMonsterArea = new NetBool();
    private readonly NetBool netIsTreasureRoom = new NetBool();
    private readonly NetBool netIsDinoArea = new NetBool();
    private readonly NetBool netIsQuarryArea = new NetBool();
    private readonly NetBool netAmbientFog = new NetBool();
    private readonly NetColor netLighting = new NetColor(Microsoft.Xna.Framework.Color.White);
    private readonly NetColor netFogColor = new NetColor();
    private readonly NetVector2Dictionary<bool, NetBool> createLadderAtEvent = new NetVector2Dictionary<bool, NetBool>();
    private readonly NetPointDictionary<bool, NetBool> createLadderDownEvent = new NetPointDictionary<bool, NetBool>();
    private float fogAlpha;
    [XmlIgnore]
    public static ICue bugLevelLoop;
    private readonly NetBool rainbowLights = new NetBool(false);
    private readonly NetBool isLightingDark = new NetBool(false);
    private LocalizedContentManager mapContent;
    public static List<MineShaft> activeMines = new List<MineShaft>();
    public static HashSet<int> mushroomLevelsGeneratedToday = new HashSet<int>();
    private int lastLevelsDownFallen;
    private Microsoft.Xna.Framework.Rectangle fogSource = new Microsoft.Xna.Framework.Rectangle(640, 0, 64, 64);
    private List<Vector2> brownSpots = new List<Vector2>();
    private int lifespan;

    public static int lowestLevelReached
    {
      get
      {
        if (Game1.netWorldState.Value.LowestMineLevelForOrder < 0)
          return Game1.netWorldState.Value.LowestMineLevel;
        return Game1.netWorldState.Value.LowestMineLevelForOrder == 120 ? Math.Max(Game1.netWorldState.Value.LowestMineLevelForOrder, Game1.netWorldState.Value.LowestMineLevelForOrder) : Game1.netWorldState.Value.LowestMineLevelForOrder;
      }
      set
      {
        if (Game1.netWorldState.Value.LowestMineLevelForOrder >= 0 && value <= 120)
          Game1.netWorldState.Value.LowestMineLevelForOrder = value;
        else
          Game1.netWorldState.Value.LowestMineLevel = value;
      }
    }

    public int mineLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.netMineLevel;
      set => this.netMineLevel.Value = value;
    }

    private int stonesLeftOnThisLevel
    {
      get => (int) (NetFieldBase<int, NetIntDelta>) this.netStonesLeftOnThisLevel;
      set => this.netStonesLeftOnThisLevel.Value = value;
    }

    private Vector2 tileBeneathLadder
    {
      get => (Vector2) (NetFieldBase<Vector2, NetVector2>) this.netTileBeneathLadder;
      set => this.netTileBeneathLadder.Value = value;
    }

    private Vector2 tileBeneathElevator
    {
      get => (Vector2) (NetFieldBase<Vector2, NetVector2>) this.netTileBeneathElevator;
      set => this.netTileBeneathElevator.Value = value;
    }

    private Point ElevatorLightSpot
    {
      get => (Point) (NetFieldBase<Point, NetPoint>) this.netElevatorLightSpot;
      set => this.netElevatorLightSpot.Value = value;
    }

    private bool isSlimeArea
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.netIsSlimeArea;
      set => this.netIsSlimeArea.Value = value;
    }

    private bool isDinoArea
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.netIsDinoArea;
      set => this.netIsDinoArea.Value = value;
    }

    private bool isMonsterArea
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.netIsMonsterArea;
      set => this.netIsMonsterArea.Value = value;
    }

    private bool isQuarryArea
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.netIsQuarryArea;
      set => this.netIsQuarryArea.Value = value;
    }

    private bool ambientFog
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.netAmbientFog;
      set => this.netAmbientFog.Value = value;
    }

    private Microsoft.Xna.Framework.Color lighting
    {
      get => (Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) this.netLighting;
      set => this.netLighting.Value = value;
    }

    private Microsoft.Xna.Framework.Color fogColor
    {
      get => (Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) this.netFogColor;
      set => this.netFogColor.Value = value;
    }

    public MineShaft()
    {
      this.name.Value = "UndergroundMine" + this.mineLevel.ToString();
      this.mapContent = Game1.game1.xTileContent.CreateTemporary();
    }

    public override bool CanPlaceThisFurnitureHere(Furniture furniture) => false;

    public MineShaft(int level)
      : this()
    {
      this.mineLevel = level;
      this.name.Value = "UndergroundMine" + level.ToString();
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.netMineLevel, (INetSerializable) this.netStonesLeftOnThisLevel, (INetSerializable) this.netTileBeneathLadder, (INetSerializable) this.netTileBeneathElevator, (INetSerializable) this.netElevatorLightSpot, (INetSerializable) this.netIsSlimeArea, (INetSerializable) this.netIsMonsterArea, (INetSerializable) this.netIsTreasureRoom, (INetSerializable) this.netIsDinoArea, (INetSerializable) this.netIsQuarryArea, (INetSerializable) this.netAmbientFog, (INetSerializable) this.netLighting, (INetSerializable) this.netFogColor, (INetSerializable) this.createLadderAtEvent, (INetSerializable) this.createLadderDownEvent, (INetSerializable) this.mapImageSource, (INetSerializable) this.rainbowLights, (INetSerializable) this.isLightingDark, (INetSerializable) this.elevatorShouldDing, (INetSerializable) this.isFogUp);
      this.isFogUp.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((field, oldValue, newValue) =>
      {
        if (!oldValue & newValue)
        {
          if (Game1.currentLocation == this)
            Game1.changeMusicTrack("none");
          if (!Game1.IsClient)
            return;
          this.fogTime = 35000;
        }
        else
        {
          if (newValue)
            return;
          this.fogTime = 0;
        }
      });
      this.createLadderAtEvent.OnValueAdded += (NetDictionary<Vector2, bool, NetBool, SerializableDictionary<Vector2, bool>, NetVector2Dictionary<bool, NetBool>>.ContentsChangeEvent) ((v, b) => this.doCreateLadderAt(v));
      this.createLadderDownEvent.OnValueAdded += (NetDictionary<Point, bool, NetBool, SerializableDictionary<Point, bool>, NetPointDictionary<bool, NetBool>>.ContentsChangeEvent) ((p, b) => this.doCreateLadderDown(p, b));
      this.mapImageSource.fieldChangeEvent += (NetFieldBase<string, NetString>.FieldChange) ((field, oldValue, newValue) =>
      {
        if (newValue == null || !(newValue != oldValue))
          return;
        this.Map.TileSheets[0].ImageSource = newValue;
        this.Map.LoadTileSheets(Game1.mapDisplayDevice);
      });
    }

    public override bool AllowMapModificationsInResetState() => true;

    protected override LocalizedContentManager getMapLoader() => this.mapContent;

    private void setElevatorLit()
    {
      this.setMapTileIndex(this.ElevatorLightSpot.X, this.ElevatorLightSpot.Y, 48, "Buildings");
      Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) this.ElevatorLightSpot.X, (float) this.ElevatorLightSpot.Y) * 64f, 2f, Microsoft.Xna.Framework.Color.Black, this.ElevatorLightSpot.X + this.ElevatorLightSpot.Y * 1000));
      this.elevatorShouldDing.Value = false;
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      int num1 = Game1.currentLocation == this ? 1 : 0;
      if ((Game1.isMusicContextActiveButNotPlaying() || Game1.getMusicTrackName().Contains("Ambient")) && Game1.random.NextDouble() < 0.00195)
        this.localSound("cavedrip");
      if (this.timeUntilElevatorLightUp > 0)
      {
        this.timeUntilElevatorLightUp -= time.ElapsedGameTime.Milliseconds;
        if (this.timeUntilElevatorLightUp <= 0)
        {
          this.localSound("crystal");
          this.setElevatorLit();
        }
      }
      if (num1 != 0)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) this.isFogUp && Game1.shouldTimePass())
        {
          if (Game1.soundBank != null && (MineShaft.bugLevelLoop == null || MineShaft.bugLevelLoop.IsStopped))
          {
            MineShaft.bugLevelLoop = Game1.soundBank.GetCue("bugLevelLoop");
            MineShaft.bugLevelLoop.Play();
          }
          if ((double) this.fogAlpha < 1.0)
          {
            if (Game1.shouldTimePass())
              this.fogAlpha += 0.01f;
            if (MineShaft.bugLevelLoop != null && Game1.soundBank != null)
            {
              MineShaft.bugLevelLoop.SetVariable("Volume", this.fogAlpha * 100f);
              MineShaft.bugLevelLoop.SetVariable("Frequency", this.fogAlpha * 25f);
            }
          }
          else if (MineShaft.bugLevelLoop != null && Game1.soundBank != null)
          {
            float num2 = (float) Math.Max(0.0, Math.Min(100.0, Math.Sin((double) this.fogTime / 10000.0 % (200.0 * Math.PI))));
            MineShaft.bugLevelLoop.SetVariable("Frequency", Math.Max(0.0f, Math.Min(100f, (float) ((double) this.fogAlpha * 25.0 + (double) num2 * 10.0))));
          }
        }
        else if ((double) this.fogAlpha > 0.0)
        {
          if (Game1.shouldTimePass())
            this.fogAlpha -= 0.01f;
          if (MineShaft.bugLevelLoop != null)
          {
            MineShaft.bugLevelLoop.SetVariable("Volume", this.fogAlpha * 100f);
            MineShaft.bugLevelLoop.SetVariable("Frequency", Math.Max(0.0f, MineShaft.bugLevelLoop.GetVariable("Frequency") - 0.01f));
            if ((double) this.fogAlpha <= 0.0)
            {
              MineShaft.bugLevelLoop.Stop(AudioStopOptions.Immediate);
              MineShaft.bugLevelLoop = (ICue) null;
            }
          }
        }
        else
        {
          int num3 = this.ambientFog ? 1 : 0;
        }
        if ((double) this.fogAlpha > 0.0 || this.ambientFog)
        {
          this.fogPos = Game1.updateFloatingObjectPositionForMovement(this.fogPos, new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y), Game1.previousViewportPosition, -1f);
          this.fogPos.X = (float) (((double) this.fogPos.X + 0.5) % 256.0);
          this.fogPos.Y = (float) (((double) this.fogPos.Y + 0.5) % 256.0);
        }
      }
      base.UpdateWhenCurrentLocation(time);
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      if (MineShaft.bugLevelLoop != null)
      {
        MineShaft.bugLevelLoop.Stop(AudioStopOptions.Immediate);
        MineShaft.bugLevelLoop = (ICue) null;
      }
      if (Game1.IsMultiplayer || this.mineLevel != 20)
        return;
      Game1.changeMusicTrack("none");
    }

    public override int getExtraMillisecondsPerInGameMinuteForThisLocation()
    {
      if (Game1.IsMultiplayer)
        return base.getExtraMillisecondsPerInGameMinuteForThisLocation();
      return this.getMineArea() != 121 ? 0 : 2000;
    }

    public Vector2 mineEntrancePosition(Farmer who) => !who.ridingMineElevator || this.tileBeneathElevator.Equals(Vector2.Zero) ? this.tileBeneathLadder : this.tileBeneathElevator;

    private void generateContents()
    {
      this.ladderHasSpawned = false;
      this.loadLevel(this.mineLevel);
      this.chooseLevelType();
      this.findLadder();
      this.populateLevel();
    }

    public void chooseLevelType()
    {
      this.fogTime = 0;
      if (MineShaft.bugLevelLoop != null)
      {
        MineShaft.bugLevelLoop.Stop(AudioStopOptions.Immediate);
        MineShaft.bugLevelLoop = (ICue) null;
      }
      this.ambientFog = false;
      this.rainbowLights.Value = false;
      this.isLightingDark.Value = false;
      Random random = new Random((int) Game1.stats.DaysPlayed * this.mineLevel + 4 * this.mineLevel + (int) Game1.uniqueIDForThisGame / 2);
      this.lighting = new Microsoft.Xna.Framework.Color(80, 80, 40);
      if (this.getMineArea() == 80)
        this.lighting = new Microsoft.Xna.Framework.Color(100, 100, 50);
      if (this.GetAdditionalDifficulty() > 0)
      {
        if (this.getMineArea() == 40)
        {
          this.lighting = new Microsoft.Xna.Framework.Color(230, 200, 90);
          this.ambientFog = true;
          this.fogColor = new Microsoft.Xna.Framework.Color(0, 80, (int) byte.MaxValue) * 0.55f;
          if (this.mineLevel < 50)
          {
            this.lighting = new Microsoft.Xna.Framework.Color(100, 80, 40);
            this.ambientFog = false;
          }
        }
      }
      else if (random.NextDouble() < 0.3 && this.mineLevel > 2)
      {
        this.isLightingDark.Value = true;
        this.lighting = new Microsoft.Xna.Framework.Color(120, 120, 40);
        if (random.NextDouble() < 0.3)
          this.lighting = new Microsoft.Xna.Framework.Color(150, 150, 60);
      }
      if (random.NextDouble() < 0.15 && this.mineLevel > 5 && this.mineLevel != 120)
      {
        this.isLightingDark.Value = true;
        switch (this.getMineArea())
        {
          case 0:
          case 10:
            this.lighting = new Microsoft.Xna.Framework.Color(110, 110, 70);
            break;
          case 40:
            this.lighting = Microsoft.Xna.Framework.Color.Black;
            if (this.GetAdditionalDifficulty() > 0)
            {
              this.lighting = new Microsoft.Xna.Framework.Color(237, 212, 185);
              break;
            }
            break;
          case 80:
            this.lighting = new Microsoft.Xna.Framework.Color(90, 130, 70);
            break;
        }
      }
      if (random.NextDouble() < 0.035 && this.getMineArea() == 80 && this.mineLevel % 5 != 0 && !MineShaft.mushroomLevelsGeneratedToday.Contains(this.mineLevel))
      {
        this.rainbowLights.Value = true;
        MineShaft.mushroomLevelsGeneratedToday.Add(this.mineLevel);
      }
      if (this.isDarkArea() && this.mineLevel < 120)
      {
        this.isLightingDark.Value = true;
        this.lighting = this.getMineArea() == 80 ? new Microsoft.Xna.Framework.Color(70, 100, 100) : new Microsoft.Xna.Framework.Color(150, 150, 120);
        if (this.getMineArea() == 0)
        {
          this.ambientFog = true;
          this.fogColor = Microsoft.Xna.Framework.Color.Black;
        }
      }
      if (this.mineLevel == 100)
        this.lighting = new Microsoft.Xna.Framework.Color(140, 140, 80);
      if (this.getMineArea() == 121)
      {
        this.lighting = new Microsoft.Xna.Framework.Color(110, 110, 40);
        if (random.NextDouble() < 0.05)
          this.lighting = random.NextDouble() < 0.5 ? new Microsoft.Xna.Framework.Color(30, 30, 0) : new Microsoft.Xna.Framework.Color(150, 150, 50);
      }
      if (this.getMineArea() != 77377)
        return;
      this.isLightingDark.Value = false;
      this.rainbowLights.Value = false;
      this.ambientFog = true;
      this.fogColor = Microsoft.Xna.Framework.Color.White * 0.4f;
      this.lighting = new Microsoft.Xna.Framework.Color(80, 80, 30);
    }

    private bool canAdd(int typeOfFeature, int numberSoFar)
    {
      if (MineShaft.permanentMineChanges.ContainsKey(this.mineLevel))
      {
        switch (typeOfFeature)
        {
          case 0:
            return MineShaft.permanentMineChanges[this.mineLevel].platformContainersLeft > numberSoFar;
          case 1:
            return MineShaft.permanentMineChanges[this.mineLevel].chestsLeft > numberSoFar;
          case 2:
            return MineShaft.permanentMineChanges[this.mineLevel].coalCartsLeft > numberSoFar;
          case 3:
            return MineShaft.permanentMineChanges[this.mineLevel].elevator == 0;
        }
      }
      return true;
    }

    public void updateMineLevelData(int feature, int amount = 1)
    {
      if (!MineShaft.permanentMineChanges.ContainsKey(this.mineLevel))
        MineShaft.permanentMineChanges.Add(this.mineLevel, new MineInfo());
      switch (feature)
      {
        case 0:
          MineShaft.permanentMineChanges[this.mineLevel].platformContainersLeft += amount;
          break;
        case 1:
          MineShaft.permanentMineChanges[this.mineLevel].chestsLeft += amount;
          break;
        case 2:
          MineShaft.permanentMineChanges[this.mineLevel].coalCartsLeft += amount;
          break;
        case 3:
          MineShaft.permanentMineChanges[this.mineLevel].elevator += amount;
          break;
      }
    }

    public void chestConsumed() => Game1.player.chestConsumedMineLevels[this.mineLevel] = true;

    public bool isLevelSlimeArea() => this.isSlimeArea;

    public void checkForMapAlterations(int x, int y)
    {
      Tile tile = this.map.GetLayer("Buildings").Tiles[x, y];
      if (tile == null || tile.TileIndex != 194 || this.canAdd(2, 0))
        return;
      this.setMapTileIndex(x, y, 195, "Buildings");
      this.setMapTileIndex(x, y - 1, 179, "Front");
    }

    public void findLadder()
    {
      int num = 0;
      this.tileBeneathElevator = Vector2.Zero;
      bool flag = this.mineLevel % 20 == 0;
      this.lightGlows.Clear();
      for (int index1 = 0; index1 < this.map.GetLayer("Buildings").LayerHeight; ++index1)
      {
        for (int index2 = 0; index2 < this.map.GetLayer("Buildings").LayerWidth; ++index2)
        {
          if (this.map.GetLayer("Buildings").Tiles[index2, index1] != null)
          {
            int tileIndex = this.map.GetLayer("Buildings").Tiles[index2, index1].TileIndex;
            switch (tileIndex)
            {
              case 112:
                this.tileBeneathElevator = new Vector2((float) index2, (float) (index1 + 1));
                ++num;
                break;
              case 115:
                this.tileBeneathLadder = new Vector2((float) index2, (float) (index1 + 1));
                this.sharedLights[index2 + index1 * 999] = new LightSource(4, new Vector2((float) index2, (float) (index1 - 2)) * 64f + new Vector2(32f, 0.0f), 0.25f, new Microsoft.Xna.Framework.Color(0, 20, 50), index2 + index1 * 999);
                this.sharedLights[index2 + index1 * 998] = new LightSource(4, new Vector2((float) index2, (float) (index1 - 1)) * 64f + new Vector2(32f, 0.0f), 0.5f, new Microsoft.Xna.Framework.Color(0, 20, 50), index2 + index1 * 998);
                this.sharedLights[index2 + index1 * 997] = new LightSource(4, new Vector2((float) index2, (float) index1) * 64f + new Vector2(32f, 0.0f), 0.75f, new Microsoft.Xna.Framework.Color(0, 20, 50), index2 + index1 * 997);
                this.sharedLights[index2 + index1 * 1000] = new LightSource(4, new Vector2((float) index2, (float) (index1 + 1)) * 64f + new Vector2(32f, 0.0f), 1f, new Microsoft.Xna.Framework.Color(0, 20, 50), index2 + index1 * 1000);
                ++num;
                break;
            }
            Microsoft.Xna.Framework.Color lighting = this.lighting;
            if (lighting.Equals(Microsoft.Xna.Framework.Color.White) && num == 2 && !flag)
              return;
            lighting = this.lighting;
            if (!lighting.Equals(Microsoft.Xna.Framework.Color.White) && (tileIndex == 97 || tileIndex == 113 || tileIndex == 65 || tileIndex == 66 || tileIndex == 81 || tileIndex == 82 || tileIndex == 48))
            {
              this.sharedLights[index2 + index1 * 1000] = new LightSource(4, new Vector2((float) index2, (float) index1) * 64f, 2.5f, new Microsoft.Xna.Framework.Color(0, 50, 100), index2 + index1 * 1000);
              if (tileIndex == 66)
                this.lightGlows.Add(new Vector2((float) index2, (float) index1) * 64f + new Vector2(0.0f, 64f));
              else if (tileIndex == 97 || tileIndex == 113)
                this.lightGlows.Add(new Vector2((float) index2, (float) index1) * 64f + new Vector2(32f, 32f));
            }
          }
          if (Game1.IsMasterGame && this.doesTileHaveProperty(index2, index1, "Water", "Back") != null && this.getMineArea() == 80 && Game1.random.NextDouble() < 0.1)
            this.sharedLights[index2 + index1 * 1000] = new LightSource(4, new Vector2((float) index2, (float) index1) * 64f, 2f, new Microsoft.Xna.Framework.Color(0, 220, 220), index2 + index1 * 1000);
        }
      }
      if (this.isFallingDownShaft)
      {
        Vector2 v = new Vector2();
        while (!this.isTileClearForMineObjects(v))
        {
          v.X = (float) Game1.random.Next(1, this.map.Layers[0].LayerWidth);
          v.Y = (float) Game1.random.Next(1, this.map.Layers[0].LayerHeight);
        }
        this.tileBeneathLadder = v;
        Game1.player.showFrame(5);
      }
      this.isFallingDownShaft = false;
    }

    public int EnemyCount => this.characters.OfType<Monster>().Count<Monster>();

    public override void performTenMinuteUpdate(int timeOfDay)
    {
      base.performTenMinuteUpdate(timeOfDay);
      if (this.mustKillAllMonstersToAdvance() && this.EnemyCount == 0)
      {
        Vector2 vector2 = new Vector2((float) (int) this.tileBeneathLadder.X, (float) (int) this.tileBeneathLadder.Y);
        if (this.getTileIndexAt(Utility.Vector2ToPoint(vector2), "Buildings") == -1)
        {
          this.createLadderAt(vector2, "newArtifact");
          if (this.mustKillAllMonstersToAdvance() && Game1.player.currentLocation == this)
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MineShaft.cs.9484"));
        }
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.isFogUp || this.map == null || this.mineLevel % 5 == 0 || Game1.random.NextDouble() >= 0.1 || this.AnyOnlineFarmerHasBuff(23))
        return;
      if (this.mineLevel > 10 && !this.mustKillAllMonstersToAdvance() && Game1.random.NextDouble() < 0.11 && this.getMineArea() != 77377)
      {
        this.isFogUp.Value = true;
        this.fogTime = 35000 + Game1.random.Next(-5, 6) * 1000;
        switch (this.getMineArea())
        {
          case 0:
          case 10:
            if (this.GetAdditionalDifficulty() > 0)
            {
              this.fogColor = this.isDarkArea() ? new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 150, 0) : Microsoft.Xna.Framework.Color.Cyan * 0.75f;
              break;
            }
            this.fogColor = this.isDarkArea() ? Microsoft.Xna.Framework.Color.Khaki : Microsoft.Xna.Framework.Color.Green * 0.75f;
            break;
          case 40:
            this.fogColor = Microsoft.Xna.Framework.Color.Blue * 0.75f;
            break;
          case 80:
            this.fogColor = Microsoft.Xna.Framework.Color.Red * 0.5f;
            break;
          case 121:
            this.fogColor = Microsoft.Xna.Framework.Color.BlueViolet * 1f;
            break;
        }
      }
      else
        this.spawnFlyingMonsterOffScreen();
    }

    public void spawnFlyingMonsterOffScreen()
    {
      Vector2 zero = Vector2.Zero;
      switch (Game1.random.Next(4))
      {
        case 0:
          zero.X = (float) Game1.random.Next(this.map.Layers[0].LayerWidth);
          break;
        case 1:
          zero.X = (float) (this.map.Layers[0].LayerWidth - 1);
          zero.Y = (float) Game1.random.Next(this.map.Layers[0].LayerHeight);
          break;
        case 2:
          zero.Y = (float) (this.map.Layers[0].LayerHeight - 1);
          zero.X = (float) Game1.random.Next(this.map.Layers[0].LayerWidth);
          break;
        case 3:
          zero.Y = (float) Game1.random.Next(this.map.Layers[0].LayerHeight);
          break;
      }
      if (Utility.isOnScreen(zero * 64f, 64))
        zero.X -= (float) (Game1.viewport.Width / 64);
      switch (this.getMineArea())
      {
        case 0:
          if (this.mineLevel <= 10 || !this.isDarkArea())
            break;
          NetCollection<NPC> characters1 = this.characters;
          Bat bat1 = new Bat(zero * 64f, this.mineLevel);
          bat1.focusedOnFarmers = true;
          Monster monster1 = this.BuffMonsterIfNecessary((Monster) bat1);
          characters1.Add((NPC) monster1);
          this.playSound("batScreech");
          break;
        case 10:
          if (this.GetAdditionalDifficulty() > 0)
          {
            NetCollection<NPC> characters2 = this.characters;
            BlueSquid blueSquid = new BlueSquid(zero * 64f);
            blueSquid.focusedOnFarmers = true;
            Monster monster2 = this.BuffMonsterIfNecessary((Monster) blueSquid);
            characters2.Add((NPC) monster2);
            break;
          }
          NetCollection<NPC> characters3 = this.characters;
          Fly fly = new Fly(zero * 64f);
          fly.focusedOnFarmers = true;
          Monster monster3 = this.BuffMonsterIfNecessary((Monster) fly);
          characters3.Add((NPC) monster3);
          break;
        case 40:
          NetCollection<NPC> characters4 = this.characters;
          Bat bat2 = new Bat(zero * 64f, this.mineLevel);
          bat2.focusedOnFarmers = true;
          Monster monster4 = this.BuffMonsterIfNecessary((Monster) bat2);
          characters4.Add((NPC) monster4);
          this.playSound("batScreech");
          break;
        case 80:
          NetCollection<NPC> characters5 = this.characters;
          Bat bat3 = new Bat(zero * 64f, this.mineLevel);
          bat3.focusedOnFarmers = true;
          Monster monster5 = this.BuffMonsterIfNecessary((Monster) bat3);
          characters5.Add((NPC) monster5);
          this.playSound("batScreech");
          break;
        case 121:
          if (this.mineLevel < 171 || Game1.random.NextDouble() < 0.5)
          {
            NetCollection<NPC> characters6 = this.characters;
            Serpent serpent1;
            if (this.GetAdditionalDifficulty() <= 0)
            {
              Serpent serpent2 = new Serpent(zero * 64f);
              serpent2.focusedOnFarmers = true;
              serpent1 = serpent2;
            }
            else
            {
              serpent1 = new Serpent(zero * 64f, "Royal Serpent");
              serpent1.focusedOnFarmers = true;
            }
            Monster monster6 = this.BuffMonsterIfNecessary((Monster) serpent1);
            characters6.Add((NPC) monster6);
            this.playSound("serpentDie");
            break;
          }
          NetCollection<NPC> characters7 = this.characters;
          Bat bat4 = new Bat(zero * 64f, this.mineLevel);
          bat4.focusedOnFarmers = true;
          Monster monster7 = this.BuffMonsterIfNecessary((Monster) bat4);
          characters7.Add((NPC) monster7);
          this.playSound("batScreech");
          break;
        case 77377:
          NetCollection<NPC> characters8 = this.characters;
          Bat bat5 = new Bat(zero * 64f, 77377);
          bat5.focusedOnFarmers = true;
          characters8.Add((NPC) bat5);
          this.playSound("rockGolemHit");
          break;
      }
    }

    public override void drawLightGlows(SpriteBatch b)
    {
      Microsoft.Xna.Framework.Color color;
      switch (this.getMineArea())
      {
        case 0:
          color = this.isDarkArea() ? Microsoft.Xna.Framework.Color.PaleGoldenrod * 0.5f : Microsoft.Xna.Framework.Color.PaleGoldenrod * 0.33f;
          break;
        case 40:
          color = Microsoft.Xna.Framework.Color.White * 0.65f;
          if (this.GetAdditionalDifficulty() > 0)
          {
            color = this.mineLevel % 40 >= 30 ? new Microsoft.Xna.Framework.Color(220, 240, (int) byte.MaxValue) * 0.8f : new Microsoft.Xna.Framework.Color(230, 225, 100) * 0.8f;
            break;
          }
          break;
        case 80:
          color = this.isDarkArea() ? Microsoft.Xna.Framework.Color.Pink * 0.4f : Microsoft.Xna.Framework.Color.Red * 0.33f;
          break;
        case 121:
          color = Microsoft.Xna.Framework.Color.White * 0.8f;
          if (this.isDinoArea)
          {
            color = Microsoft.Xna.Framework.Color.Orange * 0.5f;
            break;
          }
          break;
        default:
          color = Microsoft.Xna.Framework.Color.PaleGoldenrod * 0.33f;
          break;
      }
      foreach (Vector2 lightGlow in this.lightGlows)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) this.rainbowLights)
        {
          switch ((int) ((double) lightGlow.X / 64.0 + (double) lightGlow.Y / 64.0) % 4)
          {
            case 0:
              color = Microsoft.Xna.Framework.Color.Red * 0.5f;
              break;
            case 1:
              color = Microsoft.Xna.Framework.Color.Yellow * 0.5f;
              break;
            case 2:
              color = Microsoft.Xna.Framework.Color.Cyan * 0.33f;
              break;
            case 3:
              color = Microsoft.Xna.Framework.Color.Lime * 0.45f;
              break;
          }
        }
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, lightGlow), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(88, 1779, 30, 30)), color, 0.0f, new Vector2(15f, 15f), (float) (8.0 + 96.0 * Math.Sin((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) lightGlow.X * 777.0 + (double) lightGlow.Y * 9746.0) % 3140.0 / 1000.0) / 50.0), SpriteEffects.None, 1f);
      }
    }

    public Monster BuffMonsterIfNecessary(Monster monster)
    {
      if (monster != null && monster.GetBaseDifficultyLevel() < this.GetAdditionalDifficulty())
      {
        monster.BuffForAdditionalDifficulty(this.GetAdditionalDifficulty() - monster.GetBaseDifficultyLevel());
        if (monster is GreenSlime)
        {
          if (this.mineLevel < 40)
            (monster as GreenSlime).color.Value = new Microsoft.Xna.Framework.Color(Game1.random.Next(40, 70), Game1.random.Next(100, 190), (int) byte.MaxValue);
          else if (this.mineLevel < 80)
            (monster as GreenSlime).color.Value = new Microsoft.Xna.Framework.Color(0, 180, 120);
          else if (this.mineLevel < 120)
            (monster as GreenSlime).color.Value = new Microsoft.Xna.Framework.Color(Game1.random.Next(180, 250), 20, 120);
          else
            (monster as GreenSlime).color.Value = new Microsoft.Xna.Framework.Color(Game1.random.Next(120, 180), 20, (int) byte.MaxValue);
        }
        try
        {
          string str = (string) (NetFieldBase<string, NetString>) monster.Sprite.textureName + "_dangerous";
          if (Game1.content.Load<Texture2D>(str) != null)
            monster.Sprite.LoadTexture(str);
        }
        catch (Exception ex)
        {
        }
      }
      return monster;
    }

    public override StardewValley.Object getFish(
      float millisecondsAfterNibble,
      int bait,
      int waterDepth,
      Farmer who,
      double baitPotency,
      Vector2 bobberTile,
      string locationName = null)
    {
      int parentSheetIndex = -1;
      double num1 = 1.0 + 0.4 * (double) who.FishingLevel + (double) waterDepth * 0.1;
      if (who != null && who.CurrentTool is FishingRod && (who.CurrentTool as FishingRod).getBobberAttachmentIndex() == 856)
        num1 += 5.0;
      switch (this.getMineArea())
      {
        case 0:
        case 10:
          double num2 = num1 + (bait == 689 ? 3.0 : 0.0);
          if (Game1.random.NextDouble() < 0.02 + 0.01 * num2)
          {
            parentSheetIndex = 158;
            break;
          }
          break;
        case 40:
          double num3 = num1 + (bait == 682 ? 3.0 : 0.0);
          if (Game1.random.NextDouble() < 0.015 + 0.009 * num3)
          {
            parentSheetIndex = 161;
            break;
          }
          break;
        case 80:
          double num4 = num1 + (bait == 684 ? 3.0 : 0.0);
          if (Game1.random.NextDouble() < 0.01 + 0.008 * num4)
          {
            parentSheetIndex = 162;
            break;
          }
          break;
      }
      int quality = 0;
      if (Game1.random.NextDouble() < (double) who.FishingLevel / 10.0)
        quality = 1;
      if (Game1.random.NextDouble() < (double) who.FishingLevel / 50.0 + (double) who.LuckLevel / 100.0)
        quality = 2;
      if (parentSheetIndex != -1)
        return new StardewValley.Object(parentSheetIndex, 1, quality: quality);
      return this.getMineArea() == 80 ? new StardewValley.Object(Game1.random.Next(167, 173), 1) : base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "UndergroundMine");
    }

    private void adjustLevelChances(
      ref double stoneChance,
      ref double monsterChance,
      ref double itemChance,
      ref double gemStoneChance)
    {
      if (this.mineLevel == 1)
      {
        monsterChance = 0.0;
        itemChance = 0.0;
        gemStoneChance = 0.0;
      }
      else if (this.mineLevel % 5 == 0 && this.getMineArea() != 121)
      {
        itemChance = 0.0;
        gemStoneChance = 0.0;
        if (this.mineLevel % 10 == 0)
          monsterChance = 0.0;
      }
      if (this.mustKillAllMonstersToAdvance())
      {
        monsterChance = 0.025;
        itemChance = 0.001;
        stoneChance = 0.0;
        gemStoneChance = 0.0;
        if (this.isDinoArea)
          itemChance *= 4.0;
      }
      monsterChance += 0.02 * (double) this.GetAdditionalDifficulty();
      int num = this.AnyOnlineFarmerHasBuff(23) ? 1 : 0;
      bool flag = this.AnyOnlineFarmerHasBuff(24);
      if (num != 0 && this.getMineArea() != 121)
      {
        if (!flag)
          monsterChance = 0.0;
      }
      else if (flag)
        monsterChance *= 2.0;
      gemStoneChance /= 2.0;
      if (this.isQuarryArea || this.getMineArea() == 77377)
      {
        gemStoneChance = 0.001;
        itemChance = 0.0001;
        stoneChance *= 2.0;
        monsterChance = 0.02;
      }
      if (this.GetAdditionalDifficulty() <= 0 || this.getMineArea() != 40)
        return;
      monsterChance *= 0.660000026226044;
    }

    public bool AnyOnlineFarmerHasBuff(int which_buff)
    {
      if (which_buff == 23 && this.GetAdditionalDifficulty() > 0)
        return false;
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (onlineFarmer.hasBuff(which_buff))
          return true;
      }
      return false;
    }

    private void populateLevel()
    {
      this.objects.Clear();
      this.terrainFeatures.Clear();
      this.resourceClumps.Clear();
      this.debris.Clear();
      this.characters.Clear();
      this.ghostAdded = false;
      this.stonesLeftOnThisLevel = 0;
      double stoneChance = (double) this.mineRandom.Next(10, 30) / 100.0;
      double monsterChance = 0.002 + (double) this.mineRandom.Next(200) / 10000.0;
      double itemChance = 1.0 / 400.0;
      double gemStoneChance = 0.003;
      this.adjustLevelChances(ref stoneChance, ref monsterChance, ref itemChance, ref gemStoneChance);
      int numberSoFar = 0;
      bool flag1 = !MineShaft.permanentMineChanges.ContainsKey(this.mineLevel);
      if (this.mineLevel > 1 && this.mineLevel % 5 != 0 && (this.mineRandom.NextDouble() < 0.5 || this.isDinoArea))
      {
        int num = this.mineRandom.Next(5) + (int) (Game1.player.team.AverageDailyLuck(Game1.currentLocation) * 20.0);
        if (this.isDinoArea)
          num += this.map.Layers[0].LayerWidth * this.map.Layers[0].LayerHeight / 40;
        for (int index = 0; index < num; ++index)
        {
          Point point1;
          Point point2;
          if (this.mineRandom.NextDouble() < 0.33)
          {
            point1 = new Point(this.mineRandom.Next(this.map.GetLayer("Back").LayerWidth), 0);
            point2 = new Point(0, 1);
          }
          else if (this.mineRandom.NextDouble() < 0.5)
          {
            point1 = new Point(0, this.mineRandom.Next(this.map.GetLayer("Back").LayerHeight));
            point2 = new Point(1, 0);
          }
          else
          {
            point1 = new Point(this.map.GetLayer("Back").LayerWidth - 1, this.mineRandom.Next(this.map.GetLayer("Back").LayerHeight));
            point2 = new Point(-1, 0);
          }
          while (this.isTileOnMap(point1.X, point1.Y))
          {
            point1.X += point2.X;
            point1.Y += point2.Y;
            if (this.isTileClearForMineObjects(point1.X, point1.Y))
            {
              Vector2 vector2 = new Vector2((float) point1.X, (float) point1.Y);
              if (this.isDinoArea)
              {
                this.terrainFeatures.Add(vector2, (TerrainFeature) new CosmeticPlant(this.mineRandom.Next(3)));
                break;
              }
              if (!this.mustKillAllMonstersToAdvance())
              {
                this.objects.Add(vector2, (StardewValley.Object) new BreakableContainer(vector2, 118, this));
                break;
              }
              break;
            }
          }
        }
      }
      bool flag2 = false;
      if (this.mineLevel % 10 != 0 || this.getMineArea() == 121 && this.mineLevel != 220 && !this.netIsTreasureRoom.Value)
      {
        for (int index1 = 0; index1 < this.map.GetLayer("Back").LayerWidth; ++index1)
        {
          for (int index2 = 0; index2 < this.map.GetLayer("Back").LayerHeight; ++index2)
          {
            this.checkForMapAlterations(index1, index2);
            if (this.isTileClearForMineObjects(index1, index2))
            {
              if (this.mineRandom.NextDouble() <= stoneChance)
              {
                Vector2 vector2 = new Vector2((float) index1, (float) index2);
                if (!this.Objects.ContainsKey(vector2))
                {
                  if (this.getMineArea() == 40 && this.mineRandom.NextDouble() < 0.15)
                  {
                    int parentSheetIndex = this.mineRandom.Next(319, 322);
                    if (this.GetAdditionalDifficulty() > 0 && this.mineLevel % 40 < 30)
                      parentSheetIndex = this.mineRandom.Next(313, 316);
                    this.Objects.Add(vector2, new StardewValley.Object(vector2, parentSheetIndex, "Weeds", true, false, false, false)
                    {
                      Fragility = 2,
                      CanBeGrabbed = true
                    });
                  }
                  else if ((bool) (NetFieldBase<bool, NetBool>) this.rainbowLights && this.mineRandom.NextDouble() < 0.55)
                  {
                    if (this.mineRandom.NextDouble() < 0.25)
                    {
                      int parentSheetIndex = 404;
                      switch (this.mineRandom.Next(5))
                      {
                        case 0:
                          parentSheetIndex = 422;
                          break;
                        case 1:
                          parentSheetIndex = 420;
                          break;
                        case 2:
                          parentSheetIndex = 420;
                          break;
                        case 3:
                          parentSheetIndex = 420;
                          break;
                        case 4:
                          parentSheetIndex = 420;
                          break;
                      }
                      this.Objects.Add(vector2, new StardewValley.Object(parentSheetIndex, 1)
                      {
                        IsSpawnedObject = true
                      });
                    }
                  }
                  else
                  {
                    StardewValley.Object @object = this.chooseStoneType(0.001, 5E-05, gemStoneChance, vector2);
                    if (@object != null)
                    {
                      this.Objects.Add(vector2, @object);
                      ++this.stonesLeftOnThisLevel;
                    }
                  }
                }
              }
              else if (this.mineRandom.NextDouble() <= monsterChance && (double) this.getDistanceFromStart(index1, index2) > 5.0)
              {
                Monster monster = this.BuffMonsterIfNecessary(this.getMonsterForThisLevel(this.mineLevel, index1, index2));
                if (monster is GreenSlime && !flag2 && Game1.random.NextDouble() <= 0.012 + Game1.player.team.AverageDailyLuck() / 10.0 && Game1.player.team.SpecialOrderActive("Wizard2"))
                {
                  (monster as GreenSlime).makePrismatic();
                  flag2 = true;
                }
                if (monster is GreenSlime && this.GetAdditionalDifficulty() > 0 && this.mineRandom.NextDouble() < (double) Math.Min((float) this.GetAdditionalDifficulty() * 0.1f, 0.5f))
                {
                  if (this.mineRandom.NextDouble() < 0.00999999977648258)
                    (monster as GreenSlime).stackedSlimes.Value = 4;
                  else
                    (monster as GreenSlime).stackedSlimes.Value = 2;
                }
                if (monster is Leaper)
                {
                  float num = (float) (this.GetAdditionalDifficulty() + 1) * 0.3f;
                  if (this.mineRandom.NextDouble() < (double) num)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Leaper(Vector2.Zero)), index1 - 1, index2);
                  if (this.mineRandom.NextDouble() < (double) num)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Leaper(Vector2.Zero)), index1 + 1, index2);
                  if (this.mineRandom.NextDouble() < (double) num)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Leaper(Vector2.Zero)), index1, index2 - 1);
                  if (this.mineRandom.NextDouble() < (double) num)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Leaper(Vector2.Zero)), index1, index2 + 1);
                }
                if (monster is Grub)
                {
                  if (this.mineRandom.NextDouble() < 0.4)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Grub(Vector2.Zero)), index1 - 1, index2);
                  if (this.mineRandom.NextDouble() < 0.4)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Grub(Vector2.Zero)), index1 + 1, index2);
                  if (this.mineRandom.NextDouble() < 0.4)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Grub(Vector2.Zero)), index1, index2 - 1);
                  if (this.mineRandom.NextDouble() < 0.4)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Grub(Vector2.Zero)), index1, index2 + 1);
                }
                else if (monster is DustSpirit)
                {
                  if (this.mineRandom.NextDouble() < 0.6)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new DustSpirit(Vector2.Zero)), index1 - 1, index2);
                  if (this.mineRandom.NextDouble() < 0.6)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new DustSpirit(Vector2.Zero)), index1 + 1, index2);
                  if (this.mineRandom.NextDouble() < 0.6)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new DustSpirit(Vector2.Zero)), index1, index2 - 1);
                  if (this.mineRandom.NextDouble() < 0.6)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new DustSpirit(Vector2.Zero)), index1, index2 + 1);
                }
                if (this.mineRandom.NextDouble() < 0.00175)
                  monster.hasSpecialItem.Value = true;
                if (monster.GetBoundingBox().Width <= 64 || this.isTileClearForMineObjects(index1 + 1, index2))
                {
                  if (monster != null && monster is GreenSlime && (bool) (NetFieldBase<bool, NetBool>) (monster as GreenSlime).prismatic)
                  {
                    foreach (Character character in this.characters)
                    {
                      if (character is GreenSlime)
                      {
                        if ((bool) (NetFieldBase<bool, NetBool>) (character as GreenSlime).prismatic)
                          break;
                      }
                    }
                  }
                  this.characters.Add((NPC) monster);
                }
              }
              else if (this.mineRandom.NextDouble() <= itemChance)
                this.Objects.Add(new Vector2((float) index1, (float) index2), this.getRandomItemForThisLevel(this.mineLevel));
              else if (this.mineRandom.NextDouble() <= 0.005 && !this.isDarkArea() && !this.mustKillAllMonstersToAdvance() && (this.GetAdditionalDifficulty() <= 0 || this.getMineArea() == 40 && this.mineLevel % 40 < 30))
              {
                if (this.isTileClearForMineObjects(index1 + 1, index2) && this.isTileClearForMineObjects(index1, index2 + 1) && this.isTileClearForMineObjects(index1 + 1, index2 + 1))
                {
                  Vector2 tile = new Vector2((float) index1, (float) index2);
                  int parentSheetIndex = this.mineRandom.NextDouble() < 0.5 ? 752 : 754;
                  if (this.getMineArea() == 40)
                  {
                    if (this.GetAdditionalDifficulty() > 0)
                    {
                      parentSheetIndex = 600;
                      if (this.mineRandom.NextDouble() < 0.1)
                        parentSheetIndex = 602;
                    }
                    else
                      parentSheetIndex = this.mineRandom.NextDouble() < 0.5 ? 756 : 758;
                  }
                  this.resourceClumps.Add(new ResourceClump(parentSheetIndex, 2, 2, tile));
                }
              }
              else if (this.GetAdditionalDifficulty() > 0)
              {
                if (this.getMineArea() == 40 && this.mineLevel % 40 < 30 && this.mineRandom.NextDouble() < 0.01 && this.getTileIndexAt(index1, index2 - 1, "Buildings") != -1)
                  this.terrainFeatures.Add(new Vector2((float) index1, (float) index2), (TerrainFeature) new Tree(8, 5));
                else if (this.getMineArea() == 40 && this.mineLevel % 40 < 30 && this.mineRandom.NextDouble() < 0.1 && (this.getTileIndexAt(index1, index2 - 1, "Buildings") != -1 || this.getTileIndexAt(index1 - 1, index2, "Buildings") != -1 || this.getTileIndexAt(index1, index2 + 1, "Buildings") != -1 || this.getTileIndexAt(index1 + 1, index2, "Buildings") != -1 || this.terrainFeatures.ContainsKey(new Vector2((float) (index1 - 1), (float) index2)) || this.terrainFeatures.ContainsKey(new Vector2((float) (index1 + 1), (float) index2)) || this.terrainFeatures.ContainsKey(new Vector2((float) index1, (float) (index2 - 1))) || this.terrainFeatures.ContainsKey(new Vector2((float) index1, (float) (index2 + 1)))))
                  this.terrainFeatures.Add(new Vector2((float) index1, (float) index2), (TerrainFeature) new Grass(this.mineLevel >= 50 ? 6 : 5, this.mineLevel >= 50 ? 1 : this.mineRandom.Next(1, 5)));
                else if (this.getMineArea() == 80 && !this.isDarkArea() && this.mineRandom.NextDouble() < 0.1 && (this.getTileIndexAt(index1, index2 - 1, "Buildings") != -1 || this.getTileIndexAt(index1 - 1, index2, "Buildings") != -1 || this.getTileIndexAt(index1, index2 + 1, "Buildings") != -1 || this.getTileIndexAt(index1 + 1, index2, "Buildings") != -1 || this.terrainFeatures.ContainsKey(new Vector2((float) (index1 - 1), (float) index2)) || this.terrainFeatures.ContainsKey(new Vector2((float) (index1 + 1), (float) index2)) || this.terrainFeatures.ContainsKey(new Vector2((float) index1, (float) (index2 - 1))) || this.terrainFeatures.ContainsKey(new Vector2((float) index1, (float) (index2 + 1)))))
                  this.terrainFeatures.Add(new Vector2((float) index1, (float) index2), (TerrainFeature) new Grass(4, this.mineRandom.Next(1, 5)));
              }
            }
            else if (this.isContainerPlatform(index1, index2) && this.isTileLocationTotallyClearAndPlaceable(index1, index2) && this.mineRandom.NextDouble() < 0.4 && (flag1 || this.canAdd(0, numberSoFar)))
            {
              Vector2 vector2 = new Vector2((float) index1, (float) index2);
              this.objects.Add(vector2, (StardewValley.Object) new BreakableContainer(vector2, 118, this));
              ++numberSoFar;
              if (flag1)
                this.updateMineLevelData(0);
            }
            else if (this.mineRandom.NextDouble() <= monsterChance && this.isTileLocationTotallyClearAndPlaceable(index1, index2) && this.isTileOnClearAndSolidGround(index1, index2) && (double) this.getDistanceFromStart(index1, index2) > 5.0 && (!this.AnyOnlineFarmerHasBuff(23) || this.getMineArea() == 121))
            {
              Monster monster = this.BuffMonsterIfNecessary(this.getMonsterForThisLevel(this.mineLevel, index1, index2));
              if (this.mineRandom.NextDouble() < 0.01)
                monster.hasSpecialItem.Value = true;
              this.characters.Add((NPC) monster);
            }
          }
        }
        if (this.stonesLeftOnThisLevel > 35)
        {
          int num1 = this.stonesLeftOnThisLevel / 35;
          for (int index3 = 0; index3 < num1; ++index3)
          {
            Vector2 key1 = this.objects.Keys.ElementAt<Vector2>(this.mineRandom.Next(this.objects.Count()));
            if (this.objects[key1].name.Equals("Stone"))
            {
              int num2 = this.mineRandom.Next(3, 8);
              bool flag3 = this.mineRandom.NextDouble() < 0.1;
              for (int index4 = (int) key1.X - num2 / 2; (double) index4 < (double) key1.X + (double) (num2 / 2); ++index4)
              {
                for (int index5 = (int) key1.Y - num2 / 2; (double) index5 < (double) key1.Y + (double) (num2 / 2); ++index5)
                {
                  Vector2 key2 = new Vector2((float) index4, (float) index5);
                  if (this.objects.ContainsKey(key2) && this.objects[key2].name.Equals("Stone"))
                  {
                    this.objects.Remove(key2);
                    --this.stonesLeftOnThisLevel;
                    if ((double) this.getDistanceFromStart(index4, index5) > 5.0 && flag3 && this.mineRandom.NextDouble() < 0.12)
                      this.characters.Add((NPC) this.BuffMonsterIfNecessary(this.getMonsterForThisLevel(this.mineLevel, index4, index5)));
                  }
                }
              }
            }
          }
        }
        this.tryToAddAreaUniques();
        if (this.mineRandom.NextDouble() < 0.95 && !this.mustKillAllMonstersToAdvance() && this.mineLevel > 1 && this.mineLevel % 5 != 0 && this.shouldCreateLadderOnThisLevel())
        {
          Vector2 v = new Vector2((float) this.mineRandom.Next(this.map.GetLayer("Back").LayerWidth), (float) this.mineRandom.Next(this.map.GetLayer("Back").LayerHeight));
          if (this.isTileClearForMineObjects(v))
            this.createLadderDown((int) v.X, (int) v.Y);
        }
        if (this.mustKillAllMonstersToAdvance() && this.EnemyCount <= 1)
          this.characters.Add((NPC) new Bat(this.tileBeneathLadder * 64f + new Vector2(256f, 256f)));
      }
      if (this.mustKillAllMonstersToAdvance() && !this.isDinoArea || this.mineLevel % 5 == 0 || this.mineLevel <= 2 || this.mineLevel == 220 || this.netIsTreasureRoom.Value)
        return;
      this.tryToAddOreClumps();
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isLightingDark)
        return;
      this.tryToAddOldMinerPath();
    }

    public void placeAppropriateOreAt(Vector2 tile)
    {
      if (!this.isTileLocationTotallyClearAndPlaceable(tile))
        return;
      this.objects.Add(tile, this.getAppropriateOre(tile));
    }

    public StardewValley.Object getAppropriateOre(Vector2 tile)
    {
      StardewValley.Object appropriateOre = new StardewValley.Object(tile, 751, "Stone", true, false, false, false);
      appropriateOre.minutesUntilReady.Value = 3;
      switch (this.getMineArea())
      {
        case 0:
        case 10:
          if (this.GetAdditionalDifficulty() > 0)
          {
            appropriateOre.parentSheetIndex.Value = 849;
            appropriateOre.minutesUntilReady.Value = 6;
            break;
          }
          break;
        case 40:
          if (this.GetAdditionalDifficulty() > 0)
          {
            ColoredObject coloredObject = new ColoredObject(290, 1, new Microsoft.Xna.Framework.Color(150, 225, 160));
            coloredObject.MinutesUntilReady = 6;
            coloredObject.CanBeSetDown = true;
            coloredObject.name = "Stone";
            coloredObject.TileLocation = tile;
            coloredObject.ColorSameIndexAsParentSheetIndex = true;
            coloredObject.Flipped = this.mineRandom.NextDouble() < 0.5;
            appropriateOre = (StardewValley.Object) coloredObject;
            break;
          }
          if (this.mineRandom.NextDouble() < 0.8)
          {
            appropriateOre = new StardewValley.Object(tile, 290, "Stone", true, false, false, false);
            appropriateOre.minutesUntilReady.Value = 4;
            break;
          }
          break;
        case 80:
          if (this.mineRandom.NextDouble() < 0.8)
          {
            appropriateOre = new StardewValley.Object(tile, 764, "Stone", true, false, false, false);
            appropriateOre.minutesUntilReady.Value = 8;
            break;
          }
          break;
        case 121:
          appropriateOre = new StardewValley.Object(tile, 764, "Stone", true, false, false, false);
          appropriateOre.minutesUntilReady.Value = 8;
          if (this.mineRandom.NextDouble() < 0.02)
          {
            appropriateOre = new StardewValley.Object(tile, 765, "Stone", true, false, false, false);
            appropriateOre.minutesUntilReady.Value = 16;
            break;
          }
          break;
      }
      if (this.mineRandom.NextDouble() < 0.25 && this.getMineArea() != 40 && this.GetAdditionalDifficulty() <= 0)
      {
        appropriateOre = new StardewValley.Object(tile, this.mineRandom.NextDouble() < 0.5 ? 668 : 670, "Stone", true, false, false, false);
        appropriateOre.minutesUntilReady.Value = 2;
      }
      return appropriateOre;
    }

    public void tryToAddOreClumps()
    {
      if (this.mineRandom.NextDouble() >= 0.55 + Game1.player.team.AverageDailyLuck(Game1.currentLocation))
        return;
      Vector2 randomTile = this.getRandomTile();
      for (int index = 0; index < 1 || this.mineRandom.NextDouble() < 0.25 + Game1.player.team.AverageDailyLuck(Game1.currentLocation); ++index)
      {
        if (this.isTileLocationTotallyClearAndPlaceable(randomTile) && this.isTileOnClearAndSolidGround(randomTile) && this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "Diggable", "Back") == null)
        {
          StardewValley.Object appropriateOre = this.getAppropriateOre(randomTile);
          if ((int) (NetFieldBase<int, NetInt>) appropriateOre.parentSheetIndex == 670)
            appropriateOre.ParentSheetIndex = 668;
          Utility.recursiveObjectPlacement(appropriateOre, (int) randomTile.X, (int) randomTile.Y, 0.949999988079071, 0.300000011920929, (GameLocation) this, "Dirt", (int) (NetFieldBase<int, NetInt>) appropriateOre.parentSheetIndex == 668 ? 1 : 0, 0.0500000007450581, (int) (NetFieldBase<int, NetInt>) appropriateOre.parentSheetIndex == 668 ? 2 : 1);
        }
        randomTile = this.getRandomTile();
      }
    }

    public void tryToAddOldMinerPath()
    {
      Vector2 randomTile = this.getRandomTile();
      for (int index = 0; !this.isTileOnClearAndSolidGround(randomTile) && index < 8; ++index)
        randomTile = this.getRandomTile();
      if (!this.isTileOnClearAndSolidGround(randomTile))
        return;
      Stack<Point> path = PathFindController.findPath(Utility.Vector2ToPoint(this.tileBeneathLadder), Utility.Vector2ToPoint(randomTile), new PathFindController.isAtEnd(PathFindController.isAtEndPoint), (GameLocation) this, (Character) Game1.player, 500);
      if (path == null)
        return;
      while (path.Count > 0)
      {
        Point point = path.Pop();
        this.removeEverythingExceptCharactersFromThisTile(point.X, point.Y);
        if (path.Count > 0 && this.mineRandom.NextDouble() < 0.2)
        {
          Vector2 vector2 = Vector2.Zero;
          vector2 = path.Peek().X != point.X ? new Vector2((float) point.X, (float) (point.Y + (this.mineRandom.NextDouble() < 0.5 ? -1 : 1))) : new Vector2((float) (point.X + (this.mineRandom.NextDouble() < 0.5 ? -1 : 1)), (float) point.Y);
          if (!vector2.Equals(Vector2.Zero) && this.isTileLocationTotallyClearAndPlaceable(vector2) && this.isTileOnClearAndSolidGround(vector2))
          {
            if (this.mineRandom.NextDouble() < 0.5)
              new Torch(vector2, 1).placementAction((GameLocation) this, (int) vector2.X * 64, (int) vector2.Y * 64);
            else
              this.placeAppropriateOreAt(vector2);
          }
        }
      }
    }

    public void tryToAddAreaUniques()
    {
      if (this.getMineArea() != 10 && this.getMineArea() != 80 && (this.getMineArea() != 40 || this.mineRandom.NextDouble() >= 0.1) || this.isDarkArea() || this.mustKillAllMonstersToAdvance())
        return;
      int num = this.mineRandom.Next(7, 24);
      int parentSheetIndex = this.getMineArea() == 80 ? 316 : (this.getMineArea() == 40 ? 319 : 313);
      Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;
      int objectIndexAddRange = 2;
      if (this.GetAdditionalDifficulty() > 0)
      {
        if (this.getMineArea() == 10)
        {
          parentSheetIndex = 674;
          color = new Microsoft.Xna.Framework.Color(30, 120, (int) byte.MaxValue);
        }
        else if (this.getMineArea() == 40)
        {
          if (this.mineLevel % 40 >= 30)
          {
            parentSheetIndex = 319;
          }
          else
          {
            parentSheetIndex = 882;
            color = new Microsoft.Xna.Framework.Color(100, 180, 220);
          }
        }
        else if (this.getMineArea() == 80)
          return;
      }
      for (int index = 0; index < num; ++index)
      {
        Vector2 tileLocation = new Vector2((float) this.mineRandom.Next(this.map.GetLayer("Back").LayerWidth), (float) this.mineRandom.Next(this.map.GetLayer("Back").LayerHeight));
        if (color.Equals(Microsoft.Xna.Framework.Color.White))
        {
          Utility.recursiveObjectPlacement(new StardewValley.Object(tileLocation, parentSheetIndex, "Weeds", true, false, false, false)
          {
            Fragility = 2,
            CanBeGrabbed = true
          }, (int) tileLocation.X, (int) tileLocation.Y, 1.0, (double) this.mineRandom.Next(10, 40) / 100.0, (GameLocation) this, "Dirt", objectIndexAddRange, 0.29);
        }
        else
        {
          ColoredObject o = new ColoredObject(parentSheetIndex, 1, color);
          o.Fragility = 2;
          o.CanBeGrabbed = true;
          o.CanBeSetDown = true;
          o.Name = "Weeds";
          o.TileLocation = tileLocation;
          o.ColorSameIndexAsParentSheetIndex = true;
          Utility.recursiveObjectPlacement((StardewValley.Object) o, (int) tileLocation.X, (int) tileLocation.Y, 1.0, (double) this.mineRandom.Next(10, 40) / 100.0, (GameLocation) this, "Dirt", objectIndexAddRange, 0.29);
        }
      }
    }

    public void tryToAddMonster(Monster m, int tileX, int tileY)
    {
      if (!this.isTileClearForMineObjects(tileX, tileY) || this.isTileOccupied(new Vector2((float) tileX, (float) tileY), "", false))
        return;
      m.setTilePosition(tileX, tileY);
      this.characters.Add((NPC) m);
    }

    public bool isContainerPlatform(int x, int y) => this.map.GetLayer("Back").Tiles[x, y] != null && this.map.GetLayer("Back").Tiles[x, y].TileIndex == 257;

    public bool mustKillAllMonstersToAdvance() => this.isSlimeArea || this.isMonsterArea || this.isDinoArea;

    public void createLadderAt(Vector2 p, string sound = "hoeHit")
    {
      if (!this.shouldCreateLadderOnThisLevel())
        return;
      this.playSound(sound);
      this.createLadderAtEvent[p] = true;
    }

    public bool shouldCreateLadderOnThisLevel() => this.mineLevel != 77377 && this.mineLevel != 120;

    private void doCreateLadderAt(Vector2 p)
    {
      string str = Game1.currentLocation == this ? "sandyStep" : (string) null;
      this.updateMap();
      this.setMapTileIndex((int) p.X, (int) p.Y, 173, "Buildings");
      this.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f, Microsoft.Xna.Framework.Color.White * 0.5f)
      {
        interval = 80f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f - new Vector2(16f, 16f), Microsoft.Xna.Framework.Color.White * 0.5f)
      {
        delayBeforeAnimationStart = 150,
        interval = 80f,
        scale = 0.75f,
        startSound = str
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f + new Vector2(32f, 16f), Microsoft.Xna.Framework.Color.White * 0.5f)
      {
        delayBeforeAnimationStart = 300,
        interval = 80f,
        scale = 0.75f,
        startSound = str
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f - new Vector2(32f, -16f), Microsoft.Xna.Framework.Color.White * 0.5f)
      {
        delayBeforeAnimationStart = 450,
        interval = 80f,
        scale = 0.75f,
        startSound = str
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f - new Vector2(-16f, 16f), Microsoft.Xna.Framework.Color.White * 0.5f)
      {
        delayBeforeAnimationStart = 600,
        interval = 80f,
        scale = 0.75f,
        startSound = str
      });
      if (Game1.player.currentLocation != this)
        return;
      Game1.player.TemporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle((int) p.X * 64, (int) p.Y * 64, 64, 64));
    }

    public bool recursiveTryToCreateLadderDown(Vector2 centerTile, string sound = "hoeHit", int maxIterations = 16)
    {
      int num = 0;
      Queue<Vector2> vector2Queue = new Queue<Vector2>();
      vector2Queue.Enqueue(centerTile);
      List<Vector2> vector2List = new List<Vector2>();
      for (; num < maxIterations && vector2Queue.Count > 0; ++num)
      {
        Vector2 vector2 = vector2Queue.Dequeue();
        vector2List.Add(vector2);
        if (!this.isTileOccupied(vector2, "ignoreMe", false) && this.isTileOnClearAndSolidGround(vector2) && this.isTileOccupiedByFarmer(vector2) == null && this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Type", "Back") != null && this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Type", "Back").Equals("Stone"))
        {
          this.createLadderAt(vector2);
          return true;
        }
        foreach (Vector2 directionsTileVector in Utility.DirectionsTileVectors)
        {
          if (!vector2List.Contains(vector2 + directionsTileVector))
            vector2Queue.Enqueue(vector2 + directionsTileVector);
        }
      }
      return false;
    }

    public override void monsterDrop(Monster monster, int x, int y, Farmer who)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) monster.hasSpecialItem)
        Game1.createItemDebris(MineShaft.getSpecialItemForThisMineLevel(this.mineLevel, x / 64, y / 64), monster.Position, Game1.random.Next(4), monster.currentLocation);
      else if (this.mineLevel > 121 && who != null && who.getFriendshipHeartLevelForNPC("Krobus") >= 10 && (int) (NetFieldBase<int, NetInt>) who.houseUpgradeLevel >= 1 && !who.isMarried() && !who.isEngaged() && Game1.random.NextDouble() < 0.001)
        Game1.createItemDebris((Item) new StardewValley.Object(808, 1), monster.Position, Game1.random.Next(4), monster.currentLocation);
      else
        base.monsterDrop(monster, x, y, who);
      if ((this.mustKillAllMonstersToAdvance() || Game1.random.NextDouble() >= 0.15) && (!this.mustKillAllMonstersToAdvance() || this.EnemyCount > 1))
        return;
      Vector2 vector2 = new Vector2((float) x, (float) y) / 64f;
      vector2.X = (float) (int) vector2.X;
      vector2.Y = (float) (int) vector2.Y;
      monster.Name = "ignoreMe";
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) vector2.X * 64, (int) vector2.Y * 64, 64, 64);
      if (!this.isTileOccupied(vector2, "ignoreMe", false) && this.isTileOnClearAndSolidGround(vector2) && !Game1.player.GetBoundingBox().Intersects(rectangle) && this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Type", "Back") != null && this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Type", "Back").Equals("Stone"))
      {
        this.createLadderAt(vector2);
      }
      else
      {
        if (!this.mustKillAllMonstersToAdvance() || this.EnemyCount > 1)
          return;
        vector2 = new Vector2((float) (int) this.tileBeneathLadder.X, (float) (int) this.tileBeneathLadder.Y);
        this.createLadderAt(vector2, "newArtifact");
        if (!this.mustKillAllMonstersToAdvance() || Game1.player.currentLocation != this)
          return;
        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MineShaft.cs.9484"));
      }
    }

    public Item GetReplacementChestItem(int floor)
    {
      List<Item> list = (List<Item>) null;
      if (Game1.netWorldState.Value.ShuffleMineChests == Game1.MineChestType.Remixed)
      {
        list = new List<Item>();
        switch (floor)
        {
          case 10:
            list.Add((Item) new Boots(506));
            list.Add((Item) new Boots(507));
            list.Add((Item) new MeleeWeapon(12));
            list.Add((Item) new MeleeWeapon(17));
            list.Add((Item) new MeleeWeapon(22));
            list.Add((Item) new MeleeWeapon(31));
            break;
          case 20:
            list.Add((Item) new MeleeWeapon(11));
            list.Add((Item) new MeleeWeapon(24));
            list.Add((Item) new MeleeWeapon(20));
            list.Add((Item) new Ring(517));
            list.Add((Item) new Ring(519));
            break;
          case 50:
            list.Add((Item) new Boots(509));
            list.Add((Item) new Boots(510));
            list.Add((Item) new Boots(508));
            list.Add((Item) new MeleeWeapon(1));
            list.Add((Item) new MeleeWeapon(43));
            break;
          case 60:
            list.Add((Item) new MeleeWeapon(21));
            list.Add((Item) new MeleeWeapon(44));
            list.Add((Item) new MeleeWeapon(6));
            list.Add((Item) new MeleeWeapon(18));
            list.Add((Item) new MeleeWeapon(27));
            break;
          case 80:
            list.Add((Item) new Boots(512));
            list.Add((Item) new Boots(511));
            list.Add((Item) new MeleeWeapon(10));
            list.Add((Item) new MeleeWeapon(7));
            list.Add((Item) new MeleeWeapon(46));
            list.Add((Item) new MeleeWeapon(19));
            break;
          case 90:
            list.Add((Item) new MeleeWeapon(8));
            list.Add((Item) new MeleeWeapon(52));
            list.Add((Item) new MeleeWeapon(45));
            list.Add((Item) new MeleeWeapon(5));
            list.Add((Item) new MeleeWeapon(60));
            break;
          case 110:
            list.Add((Item) new Boots(514));
            list.Add((Item) new Boots(878));
            list.Add((Item) new MeleeWeapon(50));
            list.Add((Item) new MeleeWeapon(28));
            break;
        }
      }
      if (list == null || list.Count <= 0)
        return (Item) null;
      Random random = new Random((int) ((long) Game1.uniqueIDForThisGame * 512L) + floor);
      return Utility.GetRandom<Item>(list, random);
    }

    private void addLevelChests()
    {
      List<Item> items = new List<Item>();
      Vector2 vector2 = new Vector2(9f, 9f);
      Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;
      if (this.mineLevel < 121 && this.mineLevel % 20 == 0 && this.mineLevel % 40 != 0)
        vector2.Y += 4f;
      Item replacementChestItem = this.GetReplacementChestItem(this.mineLevel);
      bool flag = false;
      if (replacementChestItem != null)
      {
        items.Add(replacementChestItem);
      }
      else
      {
        switch (this.mineLevel)
        {
          case 5:
            Game1.player.completeQuest(14);
            if (!Game1.player.hasOrWillReceiveMail("guildQuest"))
            {
              Game1.addMailForTomorrow("guildQuest");
              break;
            }
            break;
          case 10:
            items.Add((Item) new Boots(506));
            break;
          case 20:
            items.Add((Item) new MeleeWeapon(11));
            break;
          case 40:
            Game1.player.completeQuest(17);
            items.Add((Item) new Slingshot());
            break;
          case 50:
            items.Add((Item) new Boots(509));
            break;
          case 60:
            items.Add((Item) new MeleeWeapon(21));
            break;
          case 70:
            items.Add((Item) new Slingshot(33));
            break;
          case 80:
            items.Add((Item) new Boots(512));
            break;
          case 90:
            items.Add((Item) new MeleeWeapon(8));
            break;
          case 100:
            items.Add((Item) new StardewValley.Object(434, 1));
            break;
          case 110:
            items.Add((Item) new Boots(514));
            break;
          case 120:
            Game1.player.completeQuest(18);
            Game1.getSteamAchievement("Achievement_TheBottom");
            if (!Game1.player.hasSkullKey)
              items.Add((Item) new SpecialItem(4));
            color = Microsoft.Xna.Framework.Color.Pink;
            break;
          case 220:
            if (Game1.player.secretNotesSeen.Contains(10) && !Game1.player.mailReceived.Contains("qiCave"))
            {
              Game1.eventUp = true;
              Game1.displayHUD = false;
              Game1.player.CanMove = false;
              Game1.player.showNotCarrying();
              this.currentEvent = new StardewValley.Event(Game1.content.LoadString(MineShaft.numberOfCraftedStairsUsedThisRun <= 10 ? "Data\\ExtraDialogue:SkullCavern_100_event_honorable" : "Data\\ExtraDialogue:SkullCavern_100_event"));
              this.currentEvent.exitLocation = new LocationRequest(this.Name, false, (GameLocation) this);
              Game1.player.chestConsumedMineLevels[this.mineLevel] = true;
              break;
            }
            flag = true;
            break;
        }
      }
      if (this.netIsTreasureRoom.Value | flag)
        items.Add(MineShaft.getTreasureRoomItem());
      if (items.Count <= 0 || Game1.player.chestConsumedMineLevels.ContainsKey(this.mineLevel))
        return;
      this.overlayObjects[vector2] = (StardewValley.Object) new Chest(0, items, vector2)
      {
        Tint = color
      };
    }

    public static Item getTreasureRoomItem()
    {
      switch (Game1.random.Next(26))
      {
        case 0:
          return (Item) new StardewValley.Object(288, 5);
        case 1:
          return (Item) new StardewValley.Object(287, 10);
        case 2:
          return (Item) new StardewValley.Object(802, 15);
        case 3:
          return (Item) new StardewValley.Object(773, Game1.random.Next(2, 5));
        case 4:
          return (Item) new StardewValley.Object(749, 5);
        case 5:
          return (Item) new StardewValley.Object(688, 5);
        case 6:
          return (Item) new StardewValley.Object(681, Game1.random.Next(1, 4));
        case 7:
          return (Item) new StardewValley.Object(Game1.random.Next(628, 634), 1);
        case 8:
          return (Item) new StardewValley.Object(645, Game1.random.Next(1, 3));
        case 9:
          return (Item) new StardewValley.Object(621, 4);
        case 10:
          return (Item) new StardewValley.Object(Game1.random.Next(472, 499), Game1.random.Next(1, 5) * 5);
        case 11:
          return (Item) new StardewValley.Object(286, 15);
        case 12:
          return (Item) new StardewValley.Object(437, 1);
        case 13:
          return (Item) new StardewValley.Object(439, 1);
        case 14:
          return (Item) new StardewValley.Object(349, Game1.random.Next(2, 5));
        case 15:
          return (Item) new StardewValley.Object(337, Game1.random.Next(2, 4));
        case 16:
          return (Item) new StardewValley.Object(Game1.random.Next(235, 245), 5);
        case 17:
          return (Item) new StardewValley.Object(74, 1);
        case 18:
          return (Item) new StardewValley.Object(Vector2.Zero, 21);
        case 19:
          return (Item) new StardewValley.Object(Vector2.Zero, 25);
        case 20:
          return (Item) new StardewValley.Object(Vector2.Zero, 165);
        case 21:
          return (Item) new StardewValley.Objects.Hat(37);
        case 22:
          return (Item) new StardewValley.Objects.Hat(38);
        case 23:
          return (Item) new StardewValley.Objects.Hat(65);
        case 24:
          return (Item) new StardewValley.Object(Vector2.Zero, 272);
        case 25:
          return (Item) new StardewValley.Objects.Hat(83);
        default:
          return (Item) new StardewValley.Object(288, 5);
      }
    }

    public static Item getSpecialItemForThisMineLevel(int level, int x, int y)
    {
      Random random = new Random(level + (int) Game1.stats.DaysPlayed + x + y * 10000);
      if (Game1.mine == null)
        return (Item) new StardewValley.Object(388, 1);
      if (Game1.mine.GetAdditionalDifficulty() > 0)
      {
        if (random.NextDouble() < 0.02)
          return (Item) new StardewValley.Object(Vector2.Zero, 272);
        switch (random.Next(7))
        {
          case 0:
            return (Item) new MeleeWeapon(61);
          case 1:
            return (Item) new StardewValley.Object(910, 1);
          case 2:
            return (Item) new StardewValley.Object(913, 1);
          case 3:
            return (Item) new StardewValley.Object(915, 1);
          case 4:
            return (Item) new Ring(527);
          case 5:
            return (Item) new StardewValley.Object(858, 1);
          case 6:
            Item treasureRoomItem = MineShaft.getTreasureRoomItem();
            treasureRoomItem.Stack = 1;
            return treasureRoomItem;
        }
      }
      if (level < 20)
      {
        switch (random.Next(6))
        {
          case 0:
            return (Item) new MeleeWeapon(16);
          case 1:
            return (Item) new MeleeWeapon(24);
          case 2:
            return (Item) new Boots(504);
          case 3:
            return (Item) new Boots(505);
          case 4:
            return (Item) new Ring(516);
          case 5:
            return (Item) new Ring(518);
        }
      }
      else if (level < 40)
      {
        switch (random.Next(7))
        {
          case 0:
            return (Item) new MeleeWeapon(22);
          case 1:
            return (Item) new MeleeWeapon(24);
          case 2:
            return (Item) new Boots(504);
          case 3:
            return (Item) new Boots(505);
          case 4:
            return (Item) new Ring(516);
          case 5:
            return (Item) new Ring(518);
          case 6:
            return (Item) new MeleeWeapon(15);
        }
      }
      else if (level < 60)
      {
        switch (random.Next(7))
        {
          case 0:
            return (Item) new MeleeWeapon(6);
          case 1:
            return (Item) new MeleeWeapon(26);
          case 2:
            return (Item) new MeleeWeapon(15);
          case 3:
            return (Item) new Boots(510);
          case 4:
            return (Item) new Ring(517);
          case 5:
            return (Item) new Ring(519);
          case 6:
            return (Item) new MeleeWeapon(27);
        }
      }
      else if (level < 80)
      {
        switch (random.Next(7))
        {
          case 0:
            return (Item) new MeleeWeapon(26);
          case 1:
            return (Item) new MeleeWeapon(27);
          case 2:
            return (Item) new Boots(508);
          case 3:
            return (Item) new Boots(510);
          case 4:
            return (Item) new Ring(517);
          case 5:
            return (Item) new Ring(519);
          case 6:
            return (Item) new MeleeWeapon(19);
        }
      }
      else if (level < 100)
      {
        switch (random.Next(7))
        {
          case 0:
            return (Item) new MeleeWeapon(48);
          case 1:
            return (Item) new MeleeWeapon(48);
          case 2:
            return (Item) new Boots(511);
          case 3:
            return (Item) new Boots(513);
          case 4:
            return (Item) new MeleeWeapon(18);
          case 5:
            return (Item) new MeleeWeapon(28);
          case 6:
            return (Item) new MeleeWeapon(52);
        }
      }
      else if (level < 120)
      {
        switch (random.Next(7))
        {
          case 0:
            return (Item) new MeleeWeapon(19);
          case 1:
            return (Item) new MeleeWeapon(50);
          case 2:
            return (Item) new Boots(511);
          case 3:
            return (Item) new Boots(513);
          case 4:
            return (Item) new MeleeWeapon(18);
          case 5:
            return (Item) new MeleeWeapon(46);
          case 6:
            return (Item) new Ring(887);
        }
      }
      else
      {
        switch (random.Next(12))
        {
          case 0:
            return (Item) new MeleeWeapon(45);
          case 1:
            return (Item) new MeleeWeapon(50);
          case 2:
            return (Item) new Boots(511);
          case 3:
            return (Item) new Boots(513);
          case 4:
            return (Item) new MeleeWeapon(18);
          case 5:
            return (Item) new MeleeWeapon(28);
          case 6:
            return (Item) new MeleeWeapon(52);
          case 7:
            return (Item) new StardewValley.Object(787, 1);
          case 8:
            return (Item) new Boots(878);
          case 9:
            return (Item) new StardewValley.Object(856, 1);
          case 10:
            return (Item) new Ring(859);
          case 11:
            return (Item) new Ring(887);
        }
      }
      return (Item) new StardewValley.Object(78, 1);
    }

    public override bool isTileOccupied(
      Vector2 tileLocation,
      string characterToIgnore = "",
      bool ignoreAllCharacters = false)
    {
      return this.tileBeneathLadder.Equals(tileLocation) || this.tileBeneathElevator != Vector2.Zero && this.tileBeneathElevator.Equals(tileLocation) || base.isTileOccupied(tileLocation, characterToIgnore, ignoreAllCharacters);
    }

    public bool isDarkArea() => (this.loadedDarkArea || this.mineLevel % 40 > 30) && this.getMineArea() != 40;

    public bool isTileClearForMineObjects(Vector2 v)
    {
      if (this.tileBeneathLadder.Equals(v) || this.tileBeneathElevator.Equals(v) || !this.isTileLocationTotallyClearAndPlaceable(v))
        return false;
      switch (this.doesTileHaveProperty((int) v.X, (int) v.Y, "Type", "Back"))
      {
        case "Stone":
          return this.isTileOnClearAndSolidGround(v) && !this.objects.ContainsKey(v);
        default:
          return false;
      }
    }

    public override string getFootstepSoundReplacement(string footstep) => this.GetAdditionalDifficulty() > 0 && this.getMineArea() == 40 && this.mineLevel % 40 < 30 && footstep == "stoneStep" ? "grassyStep" : base.getFootstepSoundReplacement(footstep);

    public bool isTileOnClearAndSolidGround(Vector2 v) => this.map.GetLayer("Back").Tiles[(int) v.X, (int) v.Y] != null && this.map.GetLayer("Front").Tiles[(int) v.X, (int) v.Y] == null && this.map.GetLayer("Buildings").Tiles[(int) v.X, (int) v.Y] == null && this.getTileIndexAt((int) v.X, (int) v.Y, "Back") != 77;

    public bool isTileOnClearAndSolidGround(int x, int y) => this.map.GetLayer("Back").Tiles[x, y] != null && this.map.GetLayer("Front").Tiles[x, y] == null && this.getTileIndexAt(x, y, "Back") != 77;

    public bool isTileClearForMineObjects(int x, int y) => this.isTileClearForMineObjects(new Vector2((float) x, (float) y));

    public void loadLevel(int level)
    {
      this.isMonsterArea = false;
      this.isSlimeArea = false;
      this.loadedDarkArea = false;
      this.isQuarryArea = false;
      this.isDinoArea = false;
      this.mineLoader.Unload();
      this.mineLoader.Dispose();
      this.mineLoader = Game1.content.CreateTemporary();
      int num1 = (level % 40 % 20 != 0 || level % 40 == 0 ? (level % 10 == 0 ? 10 : level) : 20) % 40;
      if (level == 120)
        num1 = 120;
      if (this.getMineArea(level) == 121)
      {
        MineShaft mineShaft = (MineShaft) null;
        foreach (MineShaft activeMine in MineShaft.activeMines)
        {
          if (activeMine != null && activeMine.mineLevel > 120 && activeMine.mineLevel < level && (mineShaft == null || activeMine.mineLevel > mineShaft.mineLevel))
            mineShaft = activeMine;
        }
        num1 = this.mineRandom.Next(40);
        while (mineShaft != null && num1 == mineShaft.loadedMapNumber)
          num1 = this.mineRandom.Next(40);
        while (num1 % 5 == 0)
          num1 = this.mineRandom.Next(40);
        if (level == 220)
          num1 = 10;
        else if (level >= 130)
        {
          double num2 = 0.01 + (Game1.player.team.AverageDailyLuck(Game1.currentLocation) / 10.0 + Game1.player.team.AverageLuckLevel(Game1.currentLocation) / 100.0);
          if (Game1.random.NextDouble() < num2)
          {
            this.netIsTreasureRoom.Value = true;
            num1 = 10;
          }
        }
      }
      else if (this.getMineArea() == 77377 && this.mineLevel == 77377)
        num1 = 77377;
      this.mapPath.Value = "Maps\\Mines\\" + num1.ToString();
      this.loadedMapNumber = num1;
      this.updateMap();
      Random random = new Random((int) Game1.stats.DaysPlayed + level * 100 + (int) Game1.uniqueIDForThisGame / 2);
      if ((!this.AnyOnlineFarmerHasBuff(23) || this.getMineArea() == 121) && random.NextDouble() < 0.044 && num1 % 5 != 0 && num1 % 40 > 5 && num1 % 40 < 30 && num1 % 40 != 19)
      {
        if (random.NextDouble() < 0.5)
          this.isMonsterArea = true;
        else
          this.isSlimeArea = true;
        if (this.getMineArea() == 121 && this.mineLevel > 126 && random.NextDouble() < 0.5)
        {
          this.isDinoArea = true;
          this.isSlimeArea = false;
          this.isMonsterArea = false;
        }
      }
      else if (this.mineLevel < 121 && random.NextDouble() < 0.044 && Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccCraftsRoom") && Game1.MasterPlayer.hasOrWillReceiveMail("VisitedQuarryMine") && num1 % 40 > 1 && num1 % 5 != 0)
      {
        this.isQuarryArea = true;
        if (random.NextDouble() < 0.25)
          this.isMonsterArea = true;
      }
      if (this.isQuarryArea || this.getMineArea(level) == 77377)
      {
        this.mapImageSource.Value = "Maps\\Mines\\mine_quarryshaft";
        int num3 = this.map.Layers[0].LayerWidth * this.map.Layers[0].LayerHeight / 100;
        this.isQuarryArea = true;
        this.isSlimeArea = false;
        this.isMonsterArea = false;
        this.isDinoArea = false;
        for (int index = 0; index < num3; ++index)
          this.brownSpots.Add(new Vector2((float) this.mineRandom.Next(0, this.map.Layers[0].LayerWidth), (float) this.mineRandom.Next(0, this.map.Layers[0].LayerHeight)));
      }
      else if (this.isDinoArea)
        this.mapImageSource.Value = "Maps\\Mines\\mine_dino";
      else if (this.isSlimeArea)
        this.mapImageSource.Value = "Maps\\Mines\\mine_slime";
      else if (this.getMineArea() == 0 || this.getMineArea() == 10 || this.getMineArea(level) != 0 && this.getMineArea(level) != 10)
      {
        if (this.getMineArea(level) == 40)
        {
          this.mapImageSource.Value = "Maps\\Mines\\mine_frost";
          if (level >= 70)
          {
            this.mapImageSource.Value += "_dark";
            this.loadedDarkArea = true;
          }
        }
        else if (this.getMineArea(level) == 80)
        {
          this.mapImageSource.Value = "Maps\\Mines\\mine_lava";
          if (level >= 110 && level != 120)
          {
            this.mapImageSource.Value += "_dark";
            this.loadedDarkArea = true;
          }
        }
        else if (this.getMineArea(level) == 121)
        {
          this.mapImageSource.Value = "Maps\\Mines\\mine_desert";
          if (num1 % 40 >= 30)
          {
            this.mapImageSource.Value += "_dark";
            this.loadedDarkArea = true;
          }
        }
      }
      if (this.GetAdditionalDifficulty() > 0)
      {
        string str1 = "Maps\\Mines\\mine";
        if (this.mapImageSource.Value != null)
          str1 = this.mapImageSource.Value;
        if (str1.EndsWith("_dark"))
          str1 = str1.Remove(str1.Length - "_dark".Length);
        string str2 = str1;
        if (level % 40 >= 30)
          this.loadedDarkArea = true;
        if (this.loadedDarkArea)
          str1 += "_dark";
        string str3 = str1 + "_dangerous";
        try
        {
          this.mapImageSource.Value = str3;
          Game1.temporaryContent.Load<Texture2D>(this.mapImageSource.Value);
        }
        catch (ContentLoadException ex1)
        {
          string str4 = str2 + "_dangerous";
          try
          {
            this.mapImageSource.Value = str4;
            Game1.temporaryContent.Load<Texture2D>(this.mapImageSource.Value);
          }
          catch (ContentLoadException ex2)
          {
            string str5 = str2;
            if (this.loadedDarkArea)
              str5 += "_dark";
            try
            {
              this.mapImageSource.Value = str5;
              Game1.temporaryContent.Load<Texture2D>(this.mapImageSource.Value);
            }
            catch (ContentLoadException ex3)
            {
              this.mapImageSource.Value = str2;
            }
          }
        }
      }
      this.ApplyDiggableTileFixes();
      if (this.isSideBranch())
        return;
      MineShaft.lowestLevelReached = Math.Max(MineShaft.lowestLevelReached, level);
      if (this.mineLevel % 5 != 0 || this.getMineArea() == 121)
        return;
      this.prepareElevator();
    }

    private void addBlueFlamesToChallengeShrine()
    {
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
      Game1.playSound("fireball");
    }

    public static void CheckForQiChallengeCompletion()
    {
      if (Game1.player.deepestMineLevel < 145 || !Game1.player.hasQuest(20) || Game1.player.hasOrWillReceiveMail("QiChallengeComplete"))
        return;
      Game1.player.completeQuest(20);
      Game1.addMailForTomorrow("QiChallengeComplete");
    }

    private void prepareElevator()
    {
      Point tile = Utility.findTile((GameLocation) this, 80, "Buildings");
      this.ElevatorLightSpot = tile;
      if (tile.X < 0)
        return;
      if (this.canAdd(3, 0))
      {
        this.elevatorShouldDing.Value = true;
        this.updateMineLevelData(3);
      }
      else
        this.setMapTileIndex(tile.X, tile.Y, 48, "Buildings");
    }

    public void enterMineShaft()
    {
      DelayedAction.playSoundAfterDelay("fallDown", 1200);
      DelayedAction.playSoundAfterDelay("clubSmash", 2200);
      Random random = new Random(this.mineLevel + (int) Game1.uniqueIDForThisGame + Game1.Date.TotalDays);
      int num = random.Next(3, 9);
      if (random.NextDouble() < 0.1)
        num = num * 2 - 1;
      if (this.mineLevel < 220 && this.mineLevel + num > 220)
        num = 220 - this.mineLevel;
      this.lastLevelsDownFallen = num;
      Game1.player.health = Math.Max(1, Game1.player.health - num * 3);
      this.isFallingDownShaft = true;
      Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.afterFall), 0.045f);
      Game1.player.CanMove = false;
      Game1.player.jump();
    }

    private void afterFall()
    {
      Game1.drawObjectDialogue(Game1.content.LoadString(this.lastLevelsDownFallen > 7 ? "Strings\\Locations:Mines_FallenFar" : "Strings\\Locations:Mines_Fallen", (object) this.lastLevelsDownFallen));
      Game1.messagePause = true;
      Game1.enterMine(this.mineLevel + this.lastLevelsDownFallen);
      Game1.fadeToBlackAlpha = 1f;
      Game1.player.faceDirection(2);
      Game1.player.showFrame(5);
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      Tile tile = this.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
      if (tile != null && who.IsLocalPlayer)
      {
        switch (tile.TileIndex)
        {
          case 112:
            if (this.mineLevel <= 120)
            {
              Game1.activeClickableMenu = (IClickableMenu) new MineElevatorMenu();
              return true;
            }
            break;
          case 115:
            this.createQuestionDialogue(" ", new Response[2]
            {
              new Response("Leave", Game1.content.LoadString("Strings\\Locations:Mines_LeaveMine")).SetHotKey(Keys.Y),
              new Response("Do", Game1.content.LoadString("Strings\\Locations:Mines_DoNothing")).SetHotKey(Keys.Escape)
            }, "ExitMine");
            return true;
          case 173:
            Game1.enterMine(this.mineLevel + 1);
            this.playSound("stairsdown");
            return true;
          case 174:
            Response[] answerChoices = new Response[2]
            {
              new Response("Jump", Game1.content.LoadString("Strings\\Locations:Mines_ShaftJumpIn")).SetHotKey(Keys.Y),
              new Response("Do", Game1.content.LoadString("Strings\\Locations:Mines_DoNothing")).SetHotKey(Keys.Escape)
            };
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Mines_Shaft"), answerChoices, "Shaft");
            return true;
          case 194:
            this.playSound("openBox");
            this.playSound("Ship");
            ++this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex;
            ++this.map.GetLayer("Front").Tiles[tileLocation.X, tileLocation.Y - 1].TileIndex;
            Game1.createRadialDebris((GameLocation) this, 382, tileLocation.X, tileLocation.Y, 6, false, item: true);
            this.updateMineLevelData(2, -1);
            return true;
          case 315:
          case 316:
          case 317:
            if (Game1.player.team.SpecialOrderRuleActive("MINE_HARD") || Game1.player.team.specialRulesRemovedToday.Contains("MINE_HARD"))
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ChallengeShrine_OnQiChallenge"));
              break;
            }
            if (Game1.player.team.toggleMineShrineOvernight.Value)
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ChallengeShrine_AlreadyActive"));
              break;
            }
            this.createQuestionDialogue(Game1.player.team.mineShrineActivated.Value ? Game1.content.LoadString("Strings\\Locations:ChallengeShrine_AlreadyHard") : Game1.content.LoadString("Strings\\Locations:ChallengeShrine_NotYetHard"), this.createYesNoResponses(), "ShrineOfChallenge");
            break;
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    public override string checkForBuriedItem(
      int xLocation,
      int yLocation,
      bool explosion,
      bool detectOnly,
      Farmer who)
    {
      if (this.isQuarryArea || Game1.random.NextDouble() >= 0.15)
        return "";
      int objectIndex = 330;
      if (Game1.random.NextDouble() < 0.07)
      {
        if (Game1.random.NextDouble() < 0.75)
        {
          switch (Game1.random.Next(5))
          {
            case 0:
              objectIndex = 96;
              break;
            case 1:
              objectIndex = who.hasOrWillReceiveMail("lostBookFound") ? ((int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.LostBooksFound < 21 ? 102 : 770) : 770;
              break;
            case 2:
              objectIndex = 110;
              break;
            case 3:
              objectIndex = 112;
              break;
            case 4:
              objectIndex = 585;
              break;
          }
        }
        else if (Game1.random.NextDouble() < 0.75)
        {
          switch (this.getMineArea())
          {
            case 0:
            case 10:
              objectIndex = Game1.random.NextDouble() < 0.5 ? 121 : 97;
              break;
            case 40:
              objectIndex = Game1.random.NextDouble() < 0.5 ? 122 : 336;
              break;
            case 80:
              objectIndex = 99;
              break;
          }
        }
        else
          objectIndex = Game1.random.NextDouble() < 0.5 ? 126 : (int) sbyte.MaxValue;
      }
      else if (Game1.random.NextDouble() < 0.19)
      {
        objectIndex = Game1.random.NextDouble() < 0.5 ? 390 : this.getOreIndexForLevel(this.mineLevel, Game1.random);
      }
      else
      {
        if (Game1.random.NextDouble() < 0.08)
        {
          Game1.createRadialDebris((GameLocation) this, 8, xLocation, yLocation, Game1.random.Next(1, 5), true);
          return "";
        }
        if (Game1.random.NextDouble() < 0.45)
          objectIndex = 330;
        else if (Game1.random.NextDouble() < 0.12)
        {
          if (Game1.random.NextDouble() < 0.25)
          {
            objectIndex = 749;
          }
          else
          {
            switch (this.getMineArea())
            {
              case 0:
              case 10:
                objectIndex = 535;
                break;
              case 40:
                objectIndex = 536;
                break;
              case 80:
                objectIndex = 537;
                break;
            }
          }
        }
        else
          objectIndex = 78;
      }
      Game1.createObjectDebris(objectIndex, xLocation, yLocation, who.UniqueMultiplayerID, (GameLocation) this);
      int num1 = who == null || who.CurrentTool == null || !(who.CurrentTool is Hoe) ? 0 : (who.CurrentTool.hasEnchantmentOfType<GenerousEnchantment>() ? 1 : 0);
      float num2 = 0.25f;
      if (num1 != 0 && Game1.random.NextDouble() < (double) num2)
        Game1.createObjectDebris(objectIndex, xLocation, yLocation, who.UniqueMultiplayerID, (GameLocation) this);
      return "";
    }

    public override bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character)
    {
      return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      base.drawAboveAlwaysFrontLayer(b);
      b.End();
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      foreach (NPC character in this.characters)
      {
        if (character is Monster)
          (character as Monster).drawAboveAllLayers(b);
      }
      b.End();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if ((double) this.fogAlpha > 0.0 || this.ambientFog)
      {
        Vector2 position = new Vector2();
        float num1 = (float) ((int) ((double) this.fogPos.X % 256.0) - 256);
        while (true)
        {
          double num2 = (double) num1;
          Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
          double width = (double) viewport.Width;
          if (num2 < width)
          {
            float num3 = (float) ((int) ((double) this.fogPos.Y % 256.0) - 256);
            while (true)
            {
              double num4 = (double) num3;
              viewport = Game1.graphics.GraphicsDevice.Viewport;
              double height = (double) viewport.Height;
              if (num4 < height)
              {
                position.X = (float) (int) num1;
                position.Y = (float) (int) num3;
                b.Draw(Game1.mouseCursors, position, new Microsoft.Xna.Framework.Rectangle?(this.fogSource), (double) this.fogAlpha > 0.0 ? this.fogColor * this.fogAlpha : this.fogColor, 0.0f, Vector2.Zero, 4.001f, SpriteEffects.None, 1f);
                num3 += 256f;
              }
              else
                break;
            }
            num1 += 256f;
          }
          else
            break;
        }
      }
      if (Game1.game1.takingMapScreenshot || this.isSideBranch())
        return;
      int color = this.getMineArea() == 0 || this.isDarkArea() && this.getMineArea() != 121 ? 4 : (this.getMineArea() == 10 ? 6 : (this.getMineArea() == 40 ? 7 : (this.getMineArea() == 80 ? 2 : 3)));
      string s = (this.mineLevel + (this.getMineArea() == 121 ? -120 : 0)).ToString() ?? "";
      Microsoft.Xna.Framework.Rectangle titleSafeArea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
      SpriteText.drawString(b, s, titleSafeArea.Left + 16, titleSafeArea.Top + 16, layerDepth: 1f, drawBGScroll: 2, color: color);
      int widthOfString = SpriteText.getWidthOfString(s);
      if (!this.mustKillAllMonstersToAdvance())
        return;
      b.Draw(Game1.mouseCursors, new Vector2((float) (titleSafeArea.Left + 16 + widthOfString + 16), (float) (titleSafeArea.Top + 16)) + new Vector2(4f, 6f) * 4f, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 324, 7, 10)), Microsoft.Xna.Framework.Color.White, 0.0f, new Vector2(3f, 5f), (float) (4.0 + (double) Game1.dialogueButtonScale / 25.0), SpriteEffects.None, 1f);
    }

    public override void checkForMusic(GameTime time)
    {
      if (Game1.player.freezePause > 0 || (bool) (NetFieldBase<bool, NetBool>) this.isFogUp || this.mineLevel == 120)
        return;
      string str = "";
      switch (this.getMineArea())
      {
        case 0:
        case 10:
        case 121:
        case 77377:
          str = "Upper";
          break;
        case 40:
          str = "Frost";
          break;
        case 80:
          str = "Lava";
          break;
      }
      string newTrackName = str + "_Ambient";
      if (this.GetAdditionalDifficulty() > 0 && this.getMineArea() == 40 && this.mineLevel < 70)
        newTrackName = "jungle_ambience";
      if (Game1.getMusicTrackName() == "none" || Game1.isMusicContextActiveButNotPlaying() || Game1.getMusicTrackName().EndsWith("_Ambient") && Game1.getMusicTrackName() != newTrackName)
        Game1.changeMusicTrack(newTrackName);
      MineShaft.timeSinceLastMusic = Math.Min(335000, MineShaft.timeSinceLastMusic + time.ElapsedGameTime.Milliseconds);
    }

    public string getMineSong()
    {
      if (this.mineLevel < 40)
        return "EarthMine";
      return this.mineLevel < 80 ? "FrostMine" : "LavaMine";
    }

    public int GetAdditionalDifficulty()
    {
      if (this.mineLevel == 77377)
        return 0;
      return this.mineLevel > 120 ? Game1.netWorldState.Value.SkullCavesDifficulty : Game1.netWorldState.Value.MinesDifficulty;
    }

    public bool isPlayingSongFromDifferentArea() => Game1.getMusicTrackName() != this.getMineSong() && Game1.getMusicTrackName().EndsWith("Mine");

    public void playMineSong()
    {
      string mineSong = this.getMineSong();
      if (!(Game1.getMusicTrackName() == "none") && !Game1.isMusicContextActiveButNotPlaying() && !Game1.getMusicTrackName().Contains("Ambient") || this.isDarkArea() || this.mineLevel == 77377)
        return;
      Game1.changeMusicTrack(mineSong);
      MineShaft.timeSinceLastMusic = 0;
    }

    protected override void resetLocalState()
    {
      this.addLevelChests();
      base.resetLocalState();
      if ((bool) (NetFieldBase<bool, NetBool>) this.elevatorShouldDing)
        this.timeUntilElevatorLightUp = 1500;
      else if (this.mineLevel % 5 == 0 && this.getMineArea() != 121)
        this.setElevatorLit();
      if (!this.isSideBranch(this.mineLevel))
      {
        Game1.player.deepestMineLevel = Math.Max(Game1.player.deepestMineLevel, this.mineLevel);
        if (Game1.player.team.specialOrders != null)
        {
          foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
          {
            if (specialOrder.onMineFloorReached != null)
              specialOrder.onMineFloorReached(Game1.player, this.mineLevel);
          }
        }
      }
      if (this.mineLevel == 77377)
        Game1.addMailForTomorrow("VisitedQuarryMine", true, true);
      MineShaft.CheckForQiChallengeCompletion();
      if (this.mineLevel == 120)
        ++Game1.player.timesReachedMineBottom;
      Vector2 vector2 = this.mineEntrancePosition(Game1.player);
      Game1.xLocationAfterWarp = (int) vector2.X;
      Game1.yLocationAfterWarp = (int) vector2.Y;
      if (Game1.IsClient)
        Game1.player.Position = new Vector2((float) (Game1.xLocationAfterWarp * 64), (float) (Game1.yLocationAfterWarp * 64 - (Game1.player.Sprite.getHeight() - 32) + 16));
      this.forceViewportPlayerFollow = true;
      if (this.mineLevel == 20 && !Game1.IsMultiplayer && Game1.isRaining && Game1.player.eventsSeen.Contains(901756) && !Game1.IsMultiplayer)
      {
        this.characters.Clear();
        NPC npc1 = new NPC(new AnimatedSprite("Characters\\Abigail", 0, 16, 32), new Vector2(896f, 644f), "SeedShop", 3, "AbigailMine", true, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\Abigail"));
        npc1.displayName = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions")["Abigail"].Split('/')[11];
        NPC npc2 = npc1;
        Random random = new Random((int) Game1.stats.DaysPlayed);
        if (!Game1.player.mailReceived.Contains("AbigailInMineFirst"))
        {
          npc2.setNewDialogue(Game1.content.LoadString("Strings\\Characters:AbigailInMineFirst"));
          npc2.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(0, 300),
            new FarmerSprite.AnimationFrame(1, 300),
            new FarmerSprite.AnimationFrame(2, 300),
            new FarmerSprite.AnimationFrame(3, 300)
          });
          Game1.player.mailReceived.Add("AbigailInMineFirst");
        }
        else if (random.NextDouble() < 0.15)
        {
          npc2.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(16, 500),
            new FarmerSprite.AnimationFrame(17, 500),
            new FarmerSprite.AnimationFrame(18, 500),
            new FarmerSprite.AnimationFrame(19, 500)
          });
          npc2.setNewDialogue(Game1.content.LoadString("Strings\\Characters:AbigailInMineFlute"));
          Game1.changeMusicTrack("AbigailFlute");
        }
        else
        {
          npc2.setNewDialogue(Game1.content.LoadString("Strings\\Characters:AbigailInMine" + random.Next(5).ToString()));
          npc2.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(0, 300),
            new FarmerSprite.AnimationFrame(1, 300),
            new FarmerSprite.AnimationFrame(2, 300),
            new FarmerSprite.AnimationFrame(3, 300)
          });
        }
        this.characters.Add(npc2);
      }
      if (this.mineLevel == 120 && this.GetAdditionalDifficulty() > 0 && !Game1.player.hasOrWillReceiveMail("reachedBottomOfHardMines"))
        Game1.addMailForTomorrow("reachedBottomOfHardMines", true, true);
      if (this.mineLevel == 120 && Game1.player.hasOrWillReceiveMail("reachedBottomOfHardMines"))
      {
        this.setMapTileIndex(9, 6, 315, "Buildings");
        this.setMapTileIndex(10, 6, 316, "Buildings");
        this.setMapTileIndex(11, 6, 317, "Buildings");
        this.setTileProperty(9, 6, "Buildings", "Action", "");
        this.setTileProperty(10, 6, "Buildings", "Action", "");
        this.setTileProperty(11, 6, "Buildings", "Action", "");
        this.setMapTileIndex(9, 5, 299, "Front");
        this.setMapTileIndex(10, 5, 300, "Front");
        this.setMapTileIndex(11, 5, 301, "Front");
        if (Game1.player.team.mineShrineActivated.Value && !Game1.player.team.toggleMineShrineOvernight.Value || !Game1.player.team.mineShrineActivated.Value && Game1.player.team.toggleMineShrineOvernight.Value)
          DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => this.addBlueFlamesToChallengeShrine()), 1000);
      }
      this.ApplyDiggableTileFixes();
      if (this.isMonsterArea || this.isSlimeArea)
      {
        Random random = new Random((int) Game1.stats.DaysPlayed);
        Game1.showGlobalMessage(Game1.content.LoadString(random.NextDouble() < 0.5 ? "Strings\\Locations:Mines_Infested" : "Strings\\Locations:Mines_Overrun"));
      }
      int num = this.mineLevel % 20 == 0 ? 1 : 0;
      bool flag = false;
      if (num != 0)
      {
        this.waterTiles = (WaterTiles) new bool[this.map.Layers[0].LayerWidth, this.map.Layers[0].LayerHeight];
        this.waterColor.Value = this.getMineArea() == 80 ? Microsoft.Xna.Framework.Color.Red * 0.8f : new Microsoft.Xna.Framework.Color(50, 100, 200) * 0.5f;
        for (int index1 = 0; index1 < this.map.GetLayer("Buildings").LayerHeight; ++index1)
        {
          for (int index2 = 0; index2 < this.map.GetLayer("Buildings").LayerWidth; ++index2)
          {
            string str = this.doesTileHaveProperty(index2, index1, "Water", "Back");
            if (str != null)
            {
              flag = true;
              if (str == "I")
                this.waterTiles.waterTiles[index2, index1] = new WaterTiles.WaterTileData(true, false);
              else
                this.waterTiles[index2, index1] = true;
              if (this.getMineArea() == 80 && Game1.random.NextDouble() < 0.1)
                this.sharedLights[index2 + index1 * 1000] = new LightSource(4, new Vector2((float) index2, (float) index1) * 64f, 2f, new Microsoft.Xna.Framework.Color(0, 220, 220), index2 + index1 * 1000);
            }
          }
        }
      }
      if (!flag)
        this.waterTiles = (WaterTiles) null;
      if (this.getMineArea(this.mineLevel) != this.getMineArea(this.mineLevel - 1) || this.mineLevel == 120 || this.isPlayingSongFromDifferentArea())
        Game1.changeMusicTrack("none");
      if (this.GetAdditionalDifficulty() > 0 && this.mineLevel == 70)
        Game1.changeMusicTrack("none");
      if (this.mineLevel == 77377 && Game1.player.mailReceived.Contains("gotGoldenScythe"))
      {
        this.setMapTileIndex(29, 4, 245, "Front");
        this.setMapTileIndex(30, 4, 246, "Front");
        this.setMapTileIndex(29, 5, 261, "Front");
        this.setMapTileIndex(30, 5, 262, "Front");
        this.setMapTileIndex(29, 6, 277, "Buildings");
        this.setMapTileIndex(30, 56, 278, "Buildings");
      }
      if (this.mineLevel <= 1 || this.mineLevel != 2 && (this.mineLevel % 5 == 0 || MineShaft.timeSinceLastMusic <= 150000 || Game1.random.NextDouble() >= 0.5))
        return;
      this.playMineSong();
    }

    public virtual void ApplyDiggableTileFixes()
    {
      if (this.map == null || this.GetAdditionalDifficulty() > 0 && this.getMineArea() != 40 && this.isDarkArea())
        return;
      if (!this.map.TileSheets[0].TileIndexProperties[165].ContainsKey("Diggable"))
        this.map.TileSheets[0].TileIndexProperties[165].Add("Diggable", new PropertyValue("true"));
      if (!this.map.TileSheets[0].TileIndexProperties[181].ContainsKey("Diggable"))
        this.map.TileSheets[0].TileIndexProperties[181].Add("Diggable", new PropertyValue("true"));
      if (this.map.TileSheets[0].TileIndexProperties[183].ContainsKey("Diggable"))
        return;
      this.map.TileSheets[0].TileIndexProperties[183].Add("Diggable", new PropertyValue("true"));
    }

    public void createLadderDown(int x, int y, bool forceShaft = false) => this.createLadderDownEvent[new Point(x, y)] = forceShaft || this.getMineArea() == 121 && !this.mustKillAllMonstersToAdvance() && this.mineRandom.NextDouble() < 0.2;

    private void doCreateLadderDown(Point point, bool shaft)
    {
      this.updateMap();
      int x = point.X;
      int y = point.Y;
      if (shaft)
      {
        this.map.GetLayer("Buildings").Tiles[x, y] = (Tile) new StaticTile(this.map.GetLayer("Buildings"), this.map.TileSheets[0], BlendMode.Alpha, 174);
      }
      else
      {
        this.ladderHasSpawned = true;
        this.map.GetLayer("Buildings").Tiles[x, y] = (Tile) new StaticTile(this.map.GetLayer("Buildings"), this.map.TileSheets[0], BlendMode.Alpha, 173);
      }
      if (Game1.player.currentLocation != this)
        return;
      Game1.player.TemporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle(x * 64, y * 64, 64, 64));
    }

    public void checkStoneForItems(int tileIndexOfStone, int x, int y, Farmer who)
    {
      if (who == null)
        who = Game1.player;
      double num1 = who.DailyLuck / 2.0 + (double) who.MiningLevel * 0.005 + (double) who.LuckLevel * 0.001;
      Random r = new Random(x * 1000 + y + this.mineLevel + (int) Game1.uniqueIDForThisGame / 2);
      r.NextDouble();
      double num2 = tileIndexOfStone == 40 || tileIndexOfStone == 42 ? 1.2 : 0.8;
      if (tileIndexOfStone != 34 && tileIndexOfStone != 36 && tileIndexOfStone != 50)
        ;
      --this.stonesLeftOnThisLevel;
      double num3 = 0.02 + 1.0 / (double) Math.Max(1, this.stonesLeftOnThisLevel) + (double) who.LuckLevel / 100.0 + Game1.player.DailyLuck / 5.0;
      if (this.EnemyCount == 0)
        num3 += 0.04;
      if (!this.ladderHasSpawned && !this.mustKillAllMonstersToAdvance() && (this.stonesLeftOnThisLevel == 0 || r.NextDouble() < num3) && this.shouldCreateLadderOnThisLevel())
        this.createLadderDown(x, y);
      if (this.breakStone(tileIndexOfStone, x, y, who, r))
        return;
      if (tileIndexOfStone == 44)
      {
        int num4 = r.Next(59, 70);
        int objectIndex = num4 + num4 % 2;
        if (who.timesReachedMineBottom == 0)
        {
          if (this.mineLevel < 40 && objectIndex != 66 && objectIndex != 68)
            objectIndex = r.NextDouble() < 0.5 ? 66 : 68;
          else if (this.mineLevel < 80 && (objectIndex == 64 || objectIndex == 60))
            objectIndex = r.NextDouble() < 0.5 ? (r.NextDouble() < 0.5 ? 66 : 70) : (r.NextDouble() < 0.5 ? 68 : 62);
        }
        Game1.createObjectDebris(objectIndex, x, y, (long) who.uniqueMultiplayerID, (GameLocation) this);
        ++Game1.stats.OtherPreciousGemsFound;
      }
      else
      {
        if (r.NextDouble() < 0.022 * (1.0 + num1) * (who.professions.Contains(22) ? 2.0 : 1.0))
        {
          int objectIndex = 535 + (this.getMineArea() == 40 ? 1 : (this.getMineArea() == 80 ? 2 : 0));
          if (this.getMineArea() == 121)
            objectIndex = 749;
          if (who.professions.Contains(19) && r.NextDouble() < 0.5)
            Game1.createObjectDebris(objectIndex, x, y, who.UniqueMultiplayerID, (GameLocation) this);
          Game1.createObjectDebris(objectIndex, x, y, who.UniqueMultiplayerID, (GameLocation) this);
          who.gainExperience(5, 20 * this.getMineArea());
        }
        if (this.mineLevel > 20 && r.NextDouble() < 0.005 * (1.0 + num1) * (who.professions.Contains(22) ? 2.0 : 1.0))
        {
          if (who.professions.Contains(19) && r.NextDouble() < 0.5)
            Game1.createObjectDebris(749, x, y, who.UniqueMultiplayerID, (GameLocation) this);
          Game1.createObjectDebris(749, x, y, who.UniqueMultiplayerID, (GameLocation) this);
          who.gainExperience(5, 40 * this.getMineArea());
        }
        if (r.NextDouble() < 0.05 * (1.0 + num1) * num2)
        {
          r.Next(1, 3);
          r.NextDouble();
          double num5 = 0.1 * (1.0 + num1);
          if (r.NextDouble() < 0.25 * (who.professions.Contains(21) ? 2.0 : 1.0))
          {
            Game1.createObjectDebris(382, x, y, who.UniqueMultiplayerID, (GameLocation) this);
            Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite(25, new Vector2((float) (64 * x), (float) (64 * y)), Microsoft.Xna.Framework.Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 80f, sourceRectHeight: 128));
          }
          Game1.createObjectDebris(this.getOreIndexForLevel(this.mineLevel, r), x, y, who.UniqueMultiplayerID, (GameLocation) this);
          who.gainExperience(3, 5);
        }
        else
        {
          if (r.NextDouble() >= 0.5)
            return;
          Game1.createDebris(14, x, y, 1, (GameLocation) this);
        }
      }
    }

    public int getOreIndexForLevel(int mineLevel, Random r)
    {
      if (this.getMineArea(mineLevel) == 77377)
        return 380;
      if (mineLevel < 40)
        return mineLevel >= 20 && r.NextDouble() < 0.1 ? 380 : 378;
      if (mineLevel < 80)
      {
        if (mineLevel >= 60 && r.NextDouble() < 0.1)
          return 384;
        return r.NextDouble() >= 0.75 ? 378 : 380;
      }
      if (mineLevel < 120)
      {
        if (r.NextDouble() < 0.75)
          return 384;
        return r.NextDouble() >= 0.75 ? 378 : 380;
      }
      if (r.NextDouble() < 0.01 + (double) (mineLevel - 120) / 2000.0)
        return 386;
      if (r.NextDouble() < 0.75)
        return 384;
      return r.NextDouble() >= 0.75 ? 378 : 380;
    }

    public bool shouldUseSnowTextureHoeDirt() => !this.isSlimeArea && (this.GetAdditionalDifficulty() > 0 && (this.mineLevel < 40 || this.mineLevel >= 70 && this.mineLevel < 80) || this.GetAdditionalDifficulty() <= 0 && this.getMineArea() == 40);

    public int getMineArea(int level = -1)
    {
      if (level == -1)
        level = this.mineLevel;
      if (this.isQuarryArea || level == 77377)
        return 77377;
      if (level >= 80 && level <= 120)
        return 80;
      if (level > 120)
        return 121;
      if (level >= 40)
        return 40;
      return level > 10 && this.mineLevel < 30 ? 10 : 0;
    }

    public bool isSideBranch(int level = -1)
    {
      if (level == -1)
        level = this.mineLevel;
      return level == 77377;
    }

    public byte getWallAt(int x, int y) => byte.MaxValue;

    public Microsoft.Xna.Framework.Color getLightingColor(GameTime time) => this.lighting;

    public StardewValley.Object getRandomItemForThisLevel(int level)
    {
      int parentSheetIndex = 80;
      if (this.mineRandom.NextDouble() < 0.05 && level > 80)
        parentSheetIndex = 422;
      else if (this.mineRandom.NextDouble() < 0.1 && level > 20 && this.getMineArea() != 40)
        parentSheetIndex = 420;
      else if (this.mineRandom.NextDouble() < 0.25 || this.GetAdditionalDifficulty() > 0)
      {
        switch (this.getMineArea())
        {
          case 0:
          case 10:
            if (this.GetAdditionalDifficulty() > 0 && !this.isDarkArea())
            {
              switch (this.mineRandom.Next(6))
              {
                case 0:
                case 6:
                  parentSheetIndex = 152;
                  break;
                case 1:
                  parentSheetIndex = 393;
                  break;
                case 2:
                  parentSheetIndex = 397;
                  break;
                case 3:
                  parentSheetIndex = 372;
                  break;
                case 4:
                  parentSheetIndex = 392;
                  break;
              }
              if (this.mineRandom.NextDouble() < 0.005)
              {
                parentSheetIndex = 797;
                break;
              }
              if (this.mineRandom.NextDouble() < 0.08)
              {
                parentSheetIndex = 394;
                break;
              }
              break;
            }
            parentSheetIndex = 86;
            break;
          case 40:
            if (this.GetAdditionalDifficulty() > 0 && this.mineLevel % 40 < 30)
            {
              switch (this.mineRandom.Next(4))
              {
                case 0:
                case 3:
                  parentSheetIndex = 259;
                  break;
                case 1:
                  parentSheetIndex = 404;
                  break;
                case 2:
                  parentSheetIndex = 420;
                  break;
              }
              if (this.mineRandom.NextDouble() < 0.08)
              {
                parentSheetIndex = 422;
                break;
              }
              break;
            }
            parentSheetIndex = 84;
            break;
          case 80:
            parentSheetIndex = 82;
            break;
          case 121:
            parentSheetIndex = this.mineRandom.NextDouble() < 0.3 ? 86 : (this.mineRandom.NextDouble() < 0.3 ? 84 : 82);
            break;
        }
      }
      else
        parentSheetIndex = 80;
      if (this.isDinoArea)
      {
        parentSheetIndex = 259;
        if (this.mineRandom.NextDouble() < 0.06)
          parentSheetIndex = 107;
      }
      return new StardewValley.Object(parentSheetIndex, 1)
      {
        IsSpawnedObject = true
      };
    }

    public bool shouldShowDarkHoeDirt() => this.getMineArea() != 121 || this.isDinoArea;

    public int getRandomGemRichStoneForThisLevel(int level)
    {
      int num1 = this.mineRandom.Next(59, 70);
      int num2 = num1 + num1 % 2;
      if (Game1.player.timesReachedMineBottom == 0)
      {
        if (level < 40 && num2 != 66 && num2 != 68)
          num2 = this.mineRandom.NextDouble() < 0.5 ? 66 : 68;
        else if (level < 80 && (num2 == 64 || num2 == 60))
          num2 = this.mineRandom.NextDouble() < 0.5 ? (this.mineRandom.NextDouble() < 0.5 ? 66 : 70) : (this.mineRandom.NextDouble() < 0.5 ? 68 : 62);
      }
      switch (num2)
      {
        case 60:
          return 12;
        case 62:
          return 14;
        case 64:
          return 4;
        case 66:
          return 8;
        case 68:
          return 10;
        case 70:
          return 6;
        default:
          return 40;
      }
    }

    public float getDistanceFromStart(int xTile, int yTile)
    {
      float val1 = Utility.distance((float) xTile, this.tileBeneathLadder.X, (float) yTile, this.tileBeneathLadder.Y);
      if (this.tileBeneathElevator != Vector2.Zero)
        val1 = Math.Min(val1, Utility.distance((float) xTile, this.tileBeneathElevator.X, (float) yTile, this.tileBeneathElevator.Y));
      return val1;
    }

    public Monster getMonsterForThisLevel(int level, int xTile, int yTile)
    {
      Vector2 vector2_1 = new Vector2((float) xTile, (float) yTile) * 64f;
      float distanceFromStart = this.getDistanceFromStart(xTile, yTile);
      if (this.isSlimeArea)
      {
        if (this.GetAdditionalDifficulty() > 0)
        {
          if (this.mineLevel < 20)
            return (Monster) new GreenSlime(vector2_1, this.mineLevel);
          if (this.mineLevel < 30)
            return (Monster) new BlueSquid(vector2_1);
          if (this.mineLevel < 40)
            return (Monster) new RockGolem(vector2_1, this);
          if (this.mineLevel < 50)
            return this.mineRandom.NextDouble() < 0.15 && (double) distanceFromStart >= 10.0 ? (Monster) new Fly(vector2_1) : (Monster) new Grub(vector2_1);
          if (this.mineLevel < 70)
            return (Monster) new Leaper(vector2_1);
        }
        else
          return this.mineRandom.NextDouble() < 0.2 ? (Monster) new BigSlime(vector2_1, this.getMineArea()) : (Monster) new GreenSlime(vector2_1, this.mineLevel);
      }
      else if (this.isDinoArea)
      {
        if (this.mineRandom.NextDouble() < 0.1)
          return (Monster) new Bat(vector2_1, 999);
        return this.mineRandom.NextDouble() < 0.1 ? (Monster) new Fly(vector2_1, true) : (Monster) new DinoMonster(vector2_1);
      }
      if (this.getMineArea() == 0 || this.getMineArea() == 10)
      {
        if (this.mineRandom.NextDouble() < 0.25 && !this.mustKillAllMonstersToAdvance())
          return (Monster) new Bug(vector2_1, this.mineRandom.Next(4), this);
        if (level < 15)
        {
          if (this.doesTileHaveProperty(xTile, yTile, "Diggable", "Back") != null)
            return (Monster) new Duggy(vector2_1);
          return this.mineRandom.NextDouble() < 0.15 ? (Monster) new RockCrab(vector2_1) : (Monster) new GreenSlime(vector2_1, level);
        }
        if (level <= 30)
        {
          if (this.doesTileHaveProperty(xTile, yTile, "Diggable", "Back") != null)
            return (Monster) new Duggy(vector2_1);
          if (this.mineRandom.NextDouble() < 0.15)
            return (Monster) new RockCrab(vector2_1);
          if (this.mineRandom.NextDouble() < 0.05 && (double) distanceFromStart > 10.0 && this.GetAdditionalDifficulty() <= 0)
            return (Monster) new Fly(vector2_1);
          if (this.mineRandom.NextDouble() < 0.45)
            return (Monster) new GreenSlime(vector2_1, level);
          if (this.GetAdditionalDifficulty() <= 0)
            return (Monster) new Grub(vector2_1);
          if ((double) distanceFromStart > 9.0)
            return (Monster) new BlueSquid(vector2_1);
          return this.mineRandom.NextDouble() < 0.01 ? (Monster) new RockGolem(vector2_1, this) : (Monster) new GreenSlime(vector2_1, level);
        }
        if (level <= 40)
        {
          if (this.mineRandom.NextDouble() < 0.1 && (double) distanceFromStart > 10.0)
            return (Monster) new Bat(vector2_1, level);
          return this.GetAdditionalDifficulty() > 0 && this.mineRandom.NextDouble() < 0.1 ? (Monster) new Ghost(vector2_1, "Carbon Ghost") : (Monster) new RockGolem(vector2_1, this);
        }
      }
      else if (this.getMineArea() == 40)
      {
        if (this.mineLevel >= 70 && (this.mineRandom.NextDouble() < 0.75 || this.GetAdditionalDifficulty() > 0))
          return this.mineRandom.NextDouble() < 0.75 || this.GetAdditionalDifficulty() <= 0 ? (Monster) new Skeleton(vector2_1, this.GetAdditionalDifficulty() > 0 && this.mineRandom.NextDouble() < 0.5) : (Monster) new Bat(vector2_1, 77377);
        if (this.mineRandom.NextDouble() < 0.3)
          return (Monster) new DustSpirit(vector2_1, this.mineRandom.NextDouble() < 0.8);
        if (this.mineRandom.NextDouble() < 0.3 && (double) distanceFromStart > 10.0)
          return (Monster) new Bat(vector2_1, this.mineLevel);
        if (!this.ghostAdded && this.mineLevel > 50 && this.mineRandom.NextDouble() < 0.3 && (double) distanceFromStart > 10.0)
        {
          this.ghostAdded = true;
          return this.GetAdditionalDifficulty() > 0 ? (Monster) new Ghost(vector2_1, "Putrid Ghost") : (Monster) new Ghost(vector2_1);
        }
        if (this.GetAdditionalDifficulty() > 0)
        {
          if (this.mineRandom.NextDouble() < 0.01)
          {
            RockCrab monsterForThisLevel = new RockCrab(vector2_1);
            monsterForThisLevel.makeStickBug();
            return (Monster) monsterForThisLevel;
          }
          if (this.mineLevel >= 50)
            return (Monster) new Leaper(vector2_1);
          return this.mineRandom.NextDouble() < 0.7 ? (Monster) new Grub(vector2_1) : (Monster) new GreenSlime(vector2_1, this.mineLevel);
        }
      }
      else if (this.getMineArea() == 80)
      {
        if (this.isDarkArea() && this.mineRandom.NextDouble() < 0.25)
          return (Monster) new Bat(vector2_1, this.mineLevel);
        if (this.mineRandom.NextDouble() < (this.GetAdditionalDifficulty() > 0 ? 0.05 : 0.15))
          return (Monster) new GreenSlime(vector2_1, this.getMineArea());
        if (this.mineRandom.NextDouble() < 0.15)
          return (Monster) new MetalHead(vector2_1, this.getMineArea());
        if (this.mineRandom.NextDouble() < 0.25)
          return (Monster) new ShadowBrute(vector2_1);
        if (this.GetAdditionalDifficulty() > 0 && this.mineRandom.NextDouble() < 0.25)
          return (Monster) new Shooter(vector2_1, "Shadow Sniper");
        if (this.mineRandom.NextDouble() < 0.25)
          return (Monster) new ShadowShaman(vector2_1);
        if (this.mineRandom.NextDouble() < 0.25)
          return (Monster) new RockCrab(vector2_1, "Lava Crab");
        if (this.mineRandom.NextDouble() < 0.2 && (double) distanceFromStart > 8.0 && this.mineLevel >= 90 && this.getTileIndexAt(xTile, yTile, "Back") != -1 && this.getTileIndexAt(xTile, yTile, "Front") == -1)
          return (Monster) new SquidKid(vector2_1);
      }
      else
      {
        if (this.getMineArea() == 121)
        {
          if (this.loadedDarkArea)
            return this.mineRandom.NextDouble() < 0.18 && (double) distanceFromStart > 8.0 ? (Monster) new Ghost(vector2_1, "Carbon Ghost") : (Monster) new Mummy(vector2_1);
          if (this.mineLevel % 20 == 0 && (double) distanceFromStart > 10.0)
            return (Monster) new Bat(vector2_1, this.mineLevel);
          if (this.mineLevel % 16 == 0 && !this.mustKillAllMonstersToAdvance())
            return (Monster) new Bug(vector2_1, this.mineRandom.Next(4), this);
          if (this.mineRandom.NextDouble() < 0.33 && (double) distanceFromStart > 10.0)
            return this.GetAdditionalDifficulty() <= 0 ? (Monster) new Serpent(vector2_1) : (Monster) new Serpent(vector2_1, "Royal Serpent");
          if (this.mineRandom.NextDouble() < 0.33 && (double) distanceFromStart > 10.0 && this.mineLevel >= 171)
            return (Monster) new Bat(vector2_1, this.mineLevel);
          if (this.mineLevel >= 126 && (double) distanceFromStart > 10.0 && this.mineRandom.NextDouble() < 0.04 && !this.mustKillAllMonstersToAdvance())
            return (Monster) new DinoMonster(vector2_1);
          if (this.mineRandom.NextDouble() < 0.33 && !this.mustKillAllMonstersToAdvance())
            return (Monster) new Bug(vector2_1, this.mineRandom.Next(4), this);
          if (this.mineRandom.NextDouble() < 0.25)
            return (Monster) new GreenSlime(vector2_1, level);
          if (this.mineLevel >= 146 && this.mineRandom.NextDouble() < 0.25)
            return (Monster) new RockCrab(vector2_1, "Iridium Crab");
          return this.GetAdditionalDifficulty() > 0 && this.mineRandom.NextDouble() < 0.2 && (double) distanceFromStart > 8.0 && this.getTileIndexAt(xTile, yTile, "Back") != -1 && this.getTileIndexAt(xTile, yTile, "Front") == -1 ? (Monster) new SquidKid(vector2_1) : (Monster) new BigSlime(vector2_1, this);
        }
        if (this.getMineArea() == 77377)
        {
          if (this.mineLevel == 77377 && yTile > 59 || this.mineLevel != 77377 && this.mineLevel % 2 == 0)
          {
            GreenSlime monsterForThisLevel = new GreenSlime(vector2_1, 77377);
            Vector2 vector2_2 = new Vector2((float) xTile, (float) yTile);
            bool flag = false;
            for (int index = 0; index < this.brownSpots.Count; ++index)
            {
              if ((double) Vector2.Distance(vector2_2, this.brownSpots[index]) < 4.0)
              {
                flag = true;
                break;
              }
            }
            if (flag)
            {
              int r = Game1.random.Next(120, 200);
              monsterForThisLevel.color.Value = new Microsoft.Xna.Framework.Color(r, r / 2, r / 4);
              while (Game1.random.NextDouble() < 0.33)
                monsterForThisLevel.objectsToDrop.Add(378);
              monsterForThisLevel.Health = (int) ((double) monsterForThisLevel.Health * 0.5);
              monsterForThisLevel.Speed += 2;
            }
            else
            {
              int num = Game1.random.Next(120, 200);
              monsterForThisLevel.color.Value = new Microsoft.Xna.Framework.Color(num, num, num);
              while (Game1.random.NextDouble() < 0.33)
                monsterForThisLevel.objectsToDrop.Add(380);
              monsterForThisLevel.Speed = 1;
            }
            return (Monster) monsterForThisLevel;
          }
          if (yTile < 51 || this.mineLevel != 77377)
            return (Monster) new Bat(vector2_1, 77377);
          Bat monsterForThisLevel1 = new Bat(vector2_1, 77377);
          monsterForThisLevel1.focusedOnFarmers = true;
          return (Monster) monsterForThisLevel1;
        }
      }
      return (Monster) new GreenSlime(vector2_1, level);
    }

    public Microsoft.Xna.Framework.Color getCrystalColorForThisLevel()
    {
      Random random = new Random(this.mineLevel + Game1.player.timesReachedMineBottom);
      if (random.NextDouble() < 0.04 && this.mineLevel < 80)
      {
        Microsoft.Xna.Framework.Color colorForThisLevel;
        for (colorForThisLevel = new Microsoft.Xna.Framework.Color(this.mineRandom.Next(256), this.mineRandom.Next(256), this.mineRandom.Next(256)); (int) colorForThisLevel.R + (int) colorForThisLevel.G + (int) colorForThisLevel.B < 500; colorForThisLevel.B = (byte) Math.Min((int) byte.MaxValue, (int) colorForThisLevel.B + 10))
        {
          colorForThisLevel.R = (byte) Math.Min((int) byte.MaxValue, (int) colorForThisLevel.R + 10);
          colorForThisLevel.G = (byte) Math.Min((int) byte.MaxValue, (int) colorForThisLevel.G + 10);
        }
        return colorForThisLevel;
      }
      if (random.NextDouble() < 0.07)
        return new Microsoft.Xna.Framework.Color((int) byte.MaxValue - this.mineRandom.Next(20), (int) byte.MaxValue - this.mineRandom.Next(20), (int) byte.MaxValue - this.mineRandom.Next(20));
      if (this.mineLevel < 40)
      {
        switch (this.mineRandom.Next(2))
        {
          case 0:
            return new Microsoft.Xna.Framework.Color(58, 145, 72);
          case 1:
            return new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue);
        }
      }
      else if (this.mineLevel < 80)
      {
        switch (this.mineRandom.Next(4))
        {
          case 0:
            return new Microsoft.Xna.Framework.Color(120, 0, 210);
          case 1:
            return new Microsoft.Xna.Framework.Color(0, 100, 170);
          case 2:
            return new Microsoft.Xna.Framework.Color(0, 220, (int) byte.MaxValue);
          case 3:
            return new Microsoft.Xna.Framework.Color(0, (int) byte.MaxValue, 220);
        }
      }
      else
      {
        switch (this.mineRandom.Next(2))
        {
          case 0:
            return new Microsoft.Xna.Framework.Color(200, 100, 0);
          case 1:
            return new Microsoft.Xna.Framework.Color(220, 60, 0);
        }
      }
      return Microsoft.Xna.Framework.Color.White;
    }

    private StardewValley.Object chooseStoneType(
      double chanceForPurpleStone,
      double chanceForMysticStone,
      double gemStoneChance,
      Vector2 tile)
    {
      Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;
      int num1 = 1;
      if (this.GetAdditionalDifficulty() > 0 && this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < (double) this.GetAdditionalDifficulty() * 0.001 + (double) this.mineLevel / 100000.0 + Game1.player.team.AverageDailyLuck() / 13.0 + Game1.player.team.AverageLuckLevel() * 0.000150000007124618)
        return new StardewValley.Object(tile, 95, "Stone", true, false, false, false)
        {
          MinutesUntilReady = 25
        };
      int parentSheetIndex1;
      if (this.getMineArea() == 0 || this.getMineArea() == 10)
      {
        parentSheetIndex1 = this.mineRandom.Next(31, 42);
        if (this.mineLevel % 40 < 30 && parentSheetIndex1 >= 33 && parentSheetIndex1 < 38)
          parentSheetIndex1 = this.mineRandom.NextDouble() < 0.5 ? 32 : 38;
        else if (this.mineLevel % 40 >= 30)
          parentSheetIndex1 = this.mineRandom.NextDouble() < 0.5 ? 34 : 36;
        if (this.GetAdditionalDifficulty() > 0)
        {
          parentSheetIndex1 = this.mineRandom.Next(33, 37);
          num1 = 5;
          if (Game1.random.NextDouble() < 0.33)
            parentSheetIndex1 = 846;
          else
            color = new Microsoft.Xna.Framework.Color(Game1.random.Next(60, 90), Game1.random.Next(150, 200), Game1.random.Next(190, 240));
          if (this.isDarkArea())
          {
            parentSheetIndex1 = this.mineRandom.Next(32, 39);
            int num2 = Game1.random.Next(130, 160);
            color = new Microsoft.Xna.Framework.Color(num2, num2, num2);
          }
          if (this.mineLevel != 1 && this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
            return new StardewValley.Object(tile, 849, "Stone", true, false, false, false)
            {
              MinutesUntilReady = 6
            };
          if (color.Equals(Microsoft.Xna.Framework.Color.White))
            return new StardewValley.Object(tile, parentSheetIndex1, "Stone", true, false, false, false)
            {
              MinutesUntilReady = num1
            };
        }
        else if (this.mineLevel != 1 && this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
          return new StardewValley.Object(tile, 751, "Stone", true, false, false, false)
          {
            MinutesUntilReady = 3
          };
      }
      else if (this.getMineArea() == 40)
      {
        parentSheetIndex1 = this.mineRandom.Next(47, 54);
        num1 = 3;
        if (this.GetAdditionalDifficulty() > 0 && this.mineLevel % 40 < 30)
        {
          parentSheetIndex1 = this.mineRandom.Next(39, 42);
          num1 = 5;
          color = new Microsoft.Xna.Framework.Color(170, (int) byte.MaxValue, 160);
          if (this.isDarkArea())
          {
            parentSheetIndex1 = this.mineRandom.Next(32, 39);
            int num3 = Game1.random.Next(130, 160);
            color = new Microsoft.Xna.Framework.Color(num3, num3, num3);
          }
          if (this.mineRandom.NextDouble() < 0.15)
          {
            ColoredObject coloredObject = new ColoredObject(294 + (this.mineRandom.NextDouble() < 0.5 ? 1 : 0), 1, new Microsoft.Xna.Framework.Color(170, 140, 155));
            coloredObject.MinutesUntilReady = 6;
            coloredObject.CanBeSetDown = true;
            coloredObject.name = "Twig";
            coloredObject.TileLocation = tile;
            coloredObject.ColorSameIndexAsParentSheetIndex = true;
            coloredObject.Flipped = this.mineRandom.NextDouble() < 0.5;
            return (StardewValley.Object) coloredObject;
          }
          if (this.mineLevel != 1 && this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
          {
            ColoredObject coloredObject = new ColoredObject(290, 1, new Microsoft.Xna.Framework.Color(150, 225, 160));
            coloredObject.MinutesUntilReady = 6;
            coloredObject.CanBeSetDown = true;
            coloredObject.name = "Stone";
            coloredObject.TileLocation = tile;
            coloredObject.ColorSameIndexAsParentSheetIndex = true;
            coloredObject.Flipped = this.mineRandom.NextDouble() < 0.5;
            return (StardewValley.Object) coloredObject;
          }
          if (color.Equals(Microsoft.Xna.Framework.Color.White))
            return new StardewValley.Object(tile, parentSheetIndex1, "Stone", true, false, false, false)
            {
              MinutesUntilReady = num1
            };
        }
        else if (this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
          return new StardewValley.Object(tile, 290, "Stone", true, false, false, false)
          {
            MinutesUntilReady = 4
          };
      }
      else if (this.getMineArea() == 80)
      {
        num1 = 4;
        parentSheetIndex1 = this.mineRandom.NextDouble() >= 0.3 || this.isDarkArea() ? (this.mineRandom.NextDouble() >= 0.3 ? (this.mineRandom.NextDouble() >= 0.5 ? 762 : 760) : this.mineRandom.Next(55, 58)) : (this.mineRandom.NextDouble() >= 0.5 ? 32 : 38);
        if (this.GetAdditionalDifficulty() > 0)
        {
          parentSheetIndex1 = this.mineRandom.NextDouble() >= 0.5 ? 32 : 38;
          num1 = 5;
          color = new Microsoft.Xna.Framework.Color(Game1.random.Next(140, 190), Game1.random.Next(90, 120), Game1.random.Next(210, (int) byte.MaxValue));
          if (this.isDarkArea())
          {
            parentSheetIndex1 = this.mineRandom.Next(32, 39);
            int num4 = Game1.random.Next(130, 160);
            color = new Microsoft.Xna.Framework.Color(num4, num4, num4);
          }
          if (this.mineLevel != 1 && this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
            return new StardewValley.Object(tile, 764, "Stone", true, false, false, false)
            {
              MinutesUntilReady = 7
            };
          if (color.Equals(Microsoft.Xna.Framework.Color.White))
            return new StardewValley.Object(tile, parentSheetIndex1, "Stone", true, false, false, false)
            {
              MinutesUntilReady = num1
            };
        }
        else if (this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
          return new StardewValley.Object(tile, 764, "Stone", true, false, false, false)
          {
            MinutesUntilReady = 8
          };
      }
      else
      {
        if (this.getMineArea() == 77377)
        {
          int num5 = 5;
          bool flag1 = false;
          foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(tile))
          {
            if (this.objects.ContainsKey(adjacentTileLocation))
            {
              flag1 = true;
              break;
            }
          }
          if (!flag1 && this.mineRandom.NextDouble() < 0.45)
            return (StardewValley.Object) null;
          bool flag2 = false;
          for (int index = 0; index < this.brownSpots.Count; ++index)
          {
            if ((double) Vector2.Distance(tile, this.brownSpots[index]) < 4.0)
            {
              flag2 = true;
              break;
            }
            if ((double) Vector2.Distance(tile, this.brownSpots[index]) < 6.0)
              return (StardewValley.Object) null;
          }
          int parentSheetIndex2;
          if (flag2)
          {
            parentSheetIndex2 = this.mineRandom.NextDouble() < 0.5 ? 32 : 38;
            if (this.mineRandom.NextDouble() < 0.01)
              return new StardewValley.Object(tile, 751, "Stone", true, false, false, false)
              {
                MinutesUntilReady = 3
              };
          }
          else
          {
            parentSheetIndex2 = this.mineRandom.NextDouble() < 0.5 ? 34 : 36;
            if (this.mineRandom.NextDouble() < 0.01)
              return new StardewValley.Object(tile, 290, "Stone", true, false, false, false)
              {
                MinutesUntilReady = 3
              };
          }
          return new StardewValley.Object(tile, parentSheetIndex2, "Stone", true, false, false, false)
          {
            MinutesUntilReady = num5
          };
        }
        num1 = 5;
        parentSheetIndex1 = this.mineRandom.NextDouble() >= 0.5 ? (this.mineRandom.NextDouble() >= 0.5 ? 42 : 40) : (this.mineRandom.NextDouble() >= 0.5 ? 32 : 38);
        int val2 = this.mineLevel - 120;
        double num6 = 0.02 + (double) val2 * 0.0005;
        if (this.mineLevel >= 130)
          num6 += 0.01 * ((double) (Math.Min(100, val2) - 10) / 10.0);
        double val1 = 0.0;
        if (this.mineLevel >= 130)
          val1 += 0.001 * ((double) (val2 - 10) / 10.0);
        double num7 = Math.Min(val1, 0.004);
        if (val2 > 100)
          num7 += (double) val2 / 1000000.0;
        if (!this.netIsTreasureRoom.Value && this.mineRandom.NextDouble() < num6)
        {
          double num8 = (double) Math.Min(100, val2) * (0.0003 + num7);
          double num9 = 0.01 + (double) (this.mineLevel - Math.Min(150, val2)) * 0.0005;
          double num10 = Math.Min(0.5, 0.1 + (double) (this.mineLevel - Math.Min(200, val2)) * 0.005);
          if (this.mineRandom.NextDouble() < num8)
            return new StardewValley.Object(tile, 765, "Stone", true, false, false, false)
            {
              MinutesUntilReady = 16
            };
          if (this.mineRandom.NextDouble() < num9)
            return new StardewValley.Object(tile, 764, "Stone", true, false, false, false)
            {
              MinutesUntilReady = 8
            };
          if (this.mineRandom.NextDouble() < num10)
            return new StardewValley.Object(tile, 290, "Stone", true, false, false, false)
            {
              MinutesUntilReady = 4
            };
          return new StardewValley.Object(tile, 751, "Stone", true, false, false, false)
          {
            MinutesUntilReady = 2
          };
        }
      }
      double num11 = Game1.player.team.AverageDailyLuck(Game1.currentLocation);
      double num12 = Game1.player.team.AverageSkillLevel(3, Game1.currentLocation);
      double num13 = num11 + num12 * 0.005;
      if (this.mineLevel > 50 && this.mineRandom.NextDouble() < 0.00025 + (double) this.mineLevel / 120000.0 + 0.0005 * num13 / 2.0)
      {
        parentSheetIndex1 = 2;
        num1 = 10;
      }
      else if (gemStoneChance != 0.0 && this.mineRandom.NextDouble() < gemStoneChance + gemStoneChance * num13 + (double) this.mineLevel / 24000.0)
        return new StardewValley.Object(tile, this.getRandomGemRichStoneForThisLevel(this.mineLevel), "Stone", true, false, false, false)
        {
          MinutesUntilReady = 5
        };
      if (this.mineRandom.NextDouble() < chanceForPurpleStone / 2.0 + chanceForPurpleStone * num12 * 0.008 + chanceForPurpleStone * (num11 / 2.0))
        parentSheetIndex1 = 44;
      if (this.mineLevel > 100 && this.mineRandom.NextDouble() < chanceForMysticStone + chanceForMysticStone * num12 * 0.008 + chanceForMysticStone * (num11 / 2.0))
        parentSheetIndex1 = 46;
      int parentSheetIndex3 = parentSheetIndex1 + parentSheetIndex1 % 2;
      if (this.mineRandom.NextDouble() < 0.1 && this.getMineArea() != 40)
      {
        if (!color.Equals(Microsoft.Xna.Framework.Color.White))
        {
          ColoredObject coloredObject = new ColoredObject(this.mineRandom.NextDouble() < 0.5 ? 668 : 670, 1, color);
          coloredObject.MinutesUntilReady = 2;
          coloredObject.CanBeSetDown = true;
          coloredObject.name = "Stone";
          coloredObject.TileLocation = tile;
          coloredObject.ColorSameIndexAsParentSheetIndex = true;
          coloredObject.Flipped = this.mineRandom.NextDouble() < 0.5;
          return (StardewValley.Object) coloredObject;
        }
        return new StardewValley.Object(tile, this.mineRandom.NextDouble() < 0.5 ? 668 : 670, "Stone", true, false, false, false)
        {
          MinutesUntilReady = 2,
          Flipped = this.mineRandom.NextDouble() < 0.5
        };
      }
      if (!color.Equals(Microsoft.Xna.Framework.Color.White))
      {
        ColoredObject coloredObject = new ColoredObject(parentSheetIndex3, 1, color);
        coloredObject.MinutesUntilReady = num1;
        coloredObject.CanBeSetDown = true;
        coloredObject.name = "Stone";
        coloredObject.TileLocation = tile;
        coloredObject.ColorSameIndexAsParentSheetIndex = true;
        coloredObject.Flipped = this.mineRandom.NextDouble() < 0.5;
        return (StardewValley.Object) coloredObject;
      }
      return new StardewValley.Object(tile, parentSheetIndex3, "Stone", true, false, false, false)
      {
        MinutesUntilReady = num1
      };
    }

    public static void OnLeftMines()
    {
      if (Game1.IsClient || Game1.IsMultiplayer)
        return;
      MineShaft.clearInactiveMines(false);
    }

    public static void clearActiveMines() => MineShaft.activeMines.RemoveAll((Predicate<MineShaft>) (mine =>
    {
      mine.mapContent.Dispose();
      return true;
    }));

    private static void clearInactiveMines(bool keepUntickedLevels = true)
    {
      int maxMineLevel = -1;
      int maxSkullLevel = -1;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.locationBeforeForcedEvent.Value != null && allFarmer.locationBeforeForcedEvent.Value.StartsWith("UndergroundMine"))
        {
          int int32 = Convert.ToInt32(allFarmer.locationBeforeForcedEvent.Value.Substring("UndergroundMine".Length));
          if (int32 > 120)
          {
            if (int32 < 77377)
              maxSkullLevel = Math.Max(maxSkullLevel, int32);
          }
          else
            maxMineLevel = Math.Max(maxMineLevel, int32);
        }
      }
      foreach (MineShaft activeMine in MineShaft.activeMines)
      {
        if (activeMine.farmers.Any())
        {
          if (activeMine.mineLevel > 120)
          {
            if (activeMine.mineLevel < 77377)
              maxSkullLevel = Math.Max(maxSkullLevel, activeMine.mineLevel);
          }
          else
            maxMineLevel = Math.Max(maxMineLevel, activeMine.mineLevel);
        }
      }
      MineShaft.activeMines.RemoveAll((Predicate<MineShaft>) (mine =>
      {
        if (mine.mineLevel == 77377)
          return false;
        if (mine.mineLevel > 120)
        {
          if (mine.mineLevel <= maxSkullLevel)
            return false;
        }
        else if (mine.mineLevel <= maxMineLevel)
          return false;
        if (mine.lifespan == 0 & keepUntickedLevels)
          return false;
        mine.mapContent.Dispose();
        return true;
      }));
    }

    public static void UpdateMines10Minutes(int timeOfDay)
    {
      MineShaft.clearInactiveMines();
      if (Game1.IsClient)
        return;
      foreach (MineShaft activeMine in MineShaft.activeMines)
      {
        if (activeMine.farmers.Any())
          activeMine.performTenMinuteUpdate(timeOfDay);
        ++activeMine.lifespan;
      }
    }

    protected override void updateCharacters(GameTime time)
    {
      if (!this.farmers.Any())
        return;
      base.updateCharacters(time);
    }

    public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
    {
      base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
      if (!Game1.shouldTimePass() || !(bool) (NetFieldBase<bool, NetBool>) this.isFogUp)
        return;
      int fogTime = this.fogTime;
      this.fogTime -= (int) time.ElapsedGameTime.TotalMilliseconds;
      if (!Game1.IsMasterGame)
        return;
      if (this.fogTime > 5000 && fogTime % 4000 < this.fogTime % 4000)
        this.spawnFlyingMonsterOffScreen();
      if (this.fogTime > 0)
        return;
      this.isFogUp.Value = false;
      if (this.isDarkArea())
      {
        this.netFogColor.Value = Microsoft.Xna.Framework.Color.Black;
      }
      else
      {
        if (this.GetAdditionalDifficulty() <= 0 || this.getMineArea() != 40 || this.isDarkArea())
          return;
        this.netFogColor.Value = new Microsoft.Xna.Framework.Color();
      }
    }

    public static void UpdateMines(GameTime time)
    {
      foreach (MineShaft activeMine in MineShaft.activeMines)
      {
        if (activeMine.farmers.Any())
          activeMine.UpdateWhenCurrentLocation(time);
        activeMine.updateEvenIfFarmerIsntHere(time, false);
      }
    }

    public static MineShaft GetMine(string name)
    {
      foreach (MineShaft activeMine in MineShaft.activeMines)
      {
        if (activeMine.Name.Equals(name))
          return activeMine;
      }
      MineShaft mine = new MineShaft(Convert.ToInt32(name.Substring("UndergroundMine".Length)));
      MineShaft.activeMines.Add(mine);
      mine.generateContents();
      return mine;
    }

    public static void ForEach(Action<MineShaft> action)
    {
      foreach (MineShaft activeMine in MineShaft.activeMines)
        action(activeMine);
    }
  }
}
