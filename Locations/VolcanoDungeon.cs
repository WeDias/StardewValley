// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.VolcanoDungeon
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewValley.Locations
{
  public class VolcanoDungeon : IslandLocation
  {
    public NetInt level = new NetInt();
    public NetEvent1Field<Point, NetPoint> coolLavaEvent = new NetEvent1Field<Point, NetPoint>();
    public static List<VolcanoDungeon> activeLevels = new List<VolcanoDungeon>();
    public NetVector2Dictionary<bool, NetBool> cooledLavaTiles = new NetVector2Dictionary<bool, NetBool>();
    public Dictionary<Vector2, Point> localCooledLavaTiles = new Dictionary<Vector2, Point>();
    public HashSet<Point> dirtTiles = new HashSet<Point>();
    public NetInt generationSeed = new NetInt();
    public NetInt layoutIndex = new NetInt();
    public Random generationRandom;
    private LocalizedContentManager mapContent;
    [XmlIgnore]
    public int mapWidth;
    [XmlIgnore]
    public int mapHeight;
    public const int WALL_HEIGHT = 4;
    public Layer backLayer;
    public Layer buildingsLayer;
    public Layer frontLayer;
    public Layer alwaysFrontLayer;
    [XmlIgnore]
    public Point? startPosition;
    [XmlIgnore]
    public Point? endPosition;
    public const int LAYOUT_WIDTH = 64;
    public const int LAYOUT_HEIGHT = 64;
    [XmlIgnore]
    public Texture2D mapBaseTilesheet;
    public static List<Microsoft.Xna.Framework.Rectangle> setPieceAreas = new List<Microsoft.Xna.Framework.Rectangle>();
    protected static Dictionary<int, Point> _blobIndexLookup = (Dictionary<int, Point>) null;
    protected static Dictionary<int, Point> _lavaBlobIndexLookup = (Dictionary<int, Point>) null;
    protected bool generated;
    protected static Point shortcutOutPosition = new Point(29, 34);
    [XmlIgnore]
    protected NetBool shortcutOutUnlocked = new NetBool(false);
    [XmlIgnore]
    protected NetBool bridgeUnlocked = new NetBool(false);
    public Microsoft.Xna.Framework.Color[] pixelMap;
    public int[] heightMap;
    public Dictionary<int, List<Point>> possibleSwitchPositions = new Dictionary<int, List<Point>>();
    public Dictionary<int, List<Point>> possibleGatePositions = new Dictionary<int, List<Point>>();
    public NetList<DwarfGate, NetRef<DwarfGate>> dwarfGates = new NetList<DwarfGate, NetRef<DwarfGate>>();
    [XmlIgnore]
    protected bool _sawFlameSprite;
    private int lavaSoundsPlayedThisTick;
    private float steamTimer = 6000f;

    public VolcanoDungeon()
    {
      this.mapContent = Game1.game1.xTileContent.CreateTemporary();
      this.mapPath.Value = "Maps\\Mines\\VolcanoTemplate";
    }

    public VolcanoDungeon(int level)
      : this()
    {
      this.level.Value = level;
      this.name.Value = nameof (VolcanoDungeon) + level.ToString();
    }

    public override bool BlocksDamageLOS(int x, int y) => !this.cooledLavaTiles.ContainsKey(new Vector2((float) x, (float) y)) && base.BlocksDamageLOS(x, y);

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.level, (INetSerializable) this.coolLavaEvent, this.cooledLavaTiles.NetFields, (INetSerializable) this.generationSeed, (INetSerializable) this.layoutIndex, (INetSerializable) this.dwarfGates, (INetSerializable) this.shortcutOutUnlocked, (INetSerializable) this.bridgeUnlocked);
      this.coolLavaEvent.onEvent += new AbstractNetEvent1<Point>.Event(this.OnCoolLavaEvent);
      this.bridgeUnlocked.InterpolationWait = false;
      this.bridgeUnlocked.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.UpdateBridge();
      });
      this.shortcutOutUnlocked.InterpolationWait = false;
      this.shortcutOutUnlocked.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((f, oldValue, newValue) =>
      {
        if (!newValue || this.mapPath.Value == null)
          return;
        this.UpdateShortcutOut();
      });
    }

    protected override LocalizedContentManager getMapLoader() => this.mapContent;

    public override bool CanPlaceThisFurnitureHere(Furniture furniture) => false;

    public virtual void OnCoolLavaEvent(Point point)
    {
      this.CoolLava(point.X, point.Y);
      this.UpdateLavaNeighbor(point.X, point.Y);
      this.UpdateLavaNeighbor(point.X - 1, point.Y);
      this.UpdateLavaNeighbor(point.X + 1, point.Y);
      this.UpdateLavaNeighbor(point.X, point.Y - 1);
      this.UpdateLavaNeighbor(point.X, point.Y + 1);
    }

    public virtual void CoolLava(int x, int y, bool playSound = true)
    {
      if (Game1.currentLocation == this)
      {
        for (int index = 0; index < 5; ++index)
          this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2((float) x, (float) y - 0.5f) * 64f + new Vector2((float) Game1.random.Next(64), (float) Game1.random.Next(64)), false, 0.007f, Microsoft.Xna.Framework.Color.White)
          {
            alpha = 0.75f,
            motion = new Vector2(0.0f, -1f),
            acceleration = new Vector2(1f / 500f, 0.0f),
            interval = 99999f,
            layerDepth = 1f,
            scale = 4f,
            scaleChange = 0.02f,
            rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
            delayBeforeAnimationStart = index * 35
          });
        if (playSound && this.lavaSoundsPlayedThisTick < 3)
        {
          DelayedAction.playSoundAfterDelay("steam", this.lavaSoundsPlayedThisTick * 300);
          ++this.lavaSoundsPlayedThisTick;
        }
      }
      if (this.cooledLavaTiles.ContainsKey(new Vector2((float) x, (float) y)))
        return;
      this.cooledLavaTiles[new Vector2((float) x, (float) y)] = true;
    }

    public virtual void UpdateLavaNeighbor(int x, int y)
    {
      if (!this.IsCooledLava(x, y))
        return;
      this.setTileProperty(x, y, "Buildings", "Passable", "T");
      int key = 0;
      if (this.IsCooledLava(x, y - 1))
        ++key;
      if (this.IsCooledLava(x, y + 1))
        key += 2;
      if (this.IsCooledLava(x - 1, y))
        key += 8;
      if (this.IsCooledLava(x + 1, y))
        key += 4;
      if (!this.GetBlobLookup().ContainsKey(key))
        return;
      this.localCooledLavaTiles[new Vector2((float) x, (float) y)] = this.GetBlobLookup()[key];
    }

    public virtual bool IsCooledLava(int x, int y) => x >= 0 && x < this.mapWidth && y >= 0 && y < this.mapHeight && this.cooledLavaTiles.ContainsKey(new Vector2((float) x, (float) y));

    public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
    {
      switch (questionAndAnswer)
      {
        case "LeaveVolcano_Yes":
          this.UseVolcanoShortcut();
          return true;
        case null:
          return false;
        default:
          return base.answerDialogueAction(questionAndAnswer, questionParams);
      }
    }

    public void UseVolcanoShortcut()
    {
      DelayedAction.playSoundAfterDelay("fallDown", 200);
      DelayedAction.playSoundAfterDelay("clubSmash", 900);
      Game1.player.CanMove = false;
      Game1.player.jump();
      Game1.warpFarmer("IslandNorth", 56, 17, 1);
    }

    public virtual void GenerateContents(bool use_level_level_as_layout = false)
    {
      this.generated = true;
      if (Game1.IsMasterGame)
      {
        this.generationSeed.Value = (int) Game1.stats.DaysPlayed * this.level.Value + this.level.Value * 5152 + (int) Game1.uniqueIDForThisGame / 2;
        if (this.level.Value == 0)
        {
          this.layoutIndex.Value = 0;
          this.bridgeUnlocked.Value = Game1.MasterPlayer.hasOrWillReceiveMail("Island_VolcanoBridge");
          this.parrotUpgradePerches.Clear();
          this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(27, 39), new Microsoft.Xna.Framework.Rectangle(28, 34, 5, 4), 5, (Action) (() =>
          {
            Game1.addMailForTomorrow("Island_VolcanoBridge", true, true);
            this.bridgeUnlocked.Value = true;
          }), (Func<bool>) (() => this.bridgeUnlocked.Value), "VolcanoBridge", "reachedCaldera, Island_Turtle"));
        }
        else if (this.level.Value == 5)
        {
          this.layoutIndex.Value = 31;
          this.waterColor.Value = Microsoft.Xna.Framework.Color.DeepSkyBlue * 0.6f;
          this.shortcutOutUnlocked.Value = Game1.MasterPlayer.hasOrWillReceiveMail("Island_VolcanoShortcutOut");
          this.parrotUpgradePerches.Clear();
          this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(VolcanoDungeon.shortcutOutPosition.X, VolcanoDungeon.shortcutOutPosition.Y), new Microsoft.Xna.Framework.Rectangle(VolcanoDungeon.shortcutOutPosition.X - 1, VolcanoDungeon.shortcutOutPosition.Y - 1, 3, 3), 5, (Action) (() =>
          {
            Game1.addMailForTomorrow("Island_VolcanoShortcutOut", true, true);
            this.shortcutOutUnlocked.Value = true;
          }), (Func<bool>) (() => this.shortcutOutUnlocked.Value), "VolcanoShortcutOut", "Island_Turtle"));
        }
        else if (this.level.Value == 9)
        {
          this.layoutIndex.Value = 30;
        }
        else
        {
          List<int> list = new List<int>();
          for (int index = 1; index < this.GetMaxRoomLayouts(); ++index)
            list.Add(index);
          Random random = new Random(this.generationSeed.Value);
          float num = (float) (1.0 + Game1.player.team.AverageLuckLevel() * 0.0350000001490116 + Game1.player.team.AverageDailyLuck() / 2.0);
          if ((int) (NetFieldBase<int, NetInt>) this.level > 1 && random.NextDouble() < 0.5 * (double) num)
          {
            bool flag = false;
            for (int index = 0; index < VolcanoDungeon.activeLevels.Count; ++index)
            {
              if (VolcanoDungeon.activeLevels[index].layoutIndex.Value >= 32)
              {
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              for (int index = 32; index < 38; ++index)
                list.Add(index);
            }
          }
          for (int index = 0; index < VolcanoDungeon.activeLevels.Count; ++index)
          {
            if (VolcanoDungeon.activeLevels[index].level.Value == this.level.Value - 1)
            {
              list.Remove(VolcanoDungeon.activeLevels[index].layoutIndex.Value);
              break;
            }
          }
          this.layoutIndex.Value = Utility.GetRandom<int>(list, random);
        }
      }
      this.GenerateLevel(use_level_level_as_layout);
      if (this.level.Value != 5)
        return;
      this.ApplyMapOverride("Mines\\Volcano_Well", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(25, 29, 6, 4)));
      for (int x = 27; x < 31; ++x)
      {
        for (int y = 29; y < 33; ++y)
          this.waterTiles[x, y] = true;
      }
      this.ApplyMapOverride("Mines\\Volcano_DwarfShop", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(34, 29, 5, 4)));
      this.setMapTileIndex(36, 30, 77, "Buildings");
      this.setMapTileIndex(36, 29, 61, "Front");
      this.setTileProperty(36, 30, "Buildings", "Action", "asedf");
      this.setMapTileIndex(35, 31, 78, "Back");
      this.setMapTileIndex(36, 31, 79, "Back");
      this.setMapTileIndex(37, 31, 62, "Back");
      if (!Game1.IsMasterGame)
        return;
      this.objects.Add(new Vector2(34f, 29f), (StardewValley.Object) new BreakableContainer(new Vector2(34f, 29f), true));
      this.objects.Add(new Vector2(26f, 32f), (StardewValley.Object) new BreakableContainer(new Vector2(26f, 32f), true));
      this.objects.Add(new Vector2(38f, 33f), (StardewValley.Object) new BreakableContainer(new Vector2(38f, 33f), true));
    }

    public bool isMushroomLevel() => (int) (NetFieldBase<int, NetInt>) this.layoutIndex >= 32 && (int) (NetFieldBase<int, NetInt>) this.layoutIndex <= 34;

    public bool isMonsterLevel() => (int) (NetFieldBase<int, NetInt>) this.layoutIndex >= 35 && (int) (NetFieldBase<int, NetInt>) this.layoutIndex <= 37;

    public override void checkForMusic(GameTime time)
    {
      if (!(Game1.getMusicTrackName() == "rain") && !(Game1.getMusicTrackName() == "none") && !(Game1.getMusicTrackName() == "IslandMusic") && !Game1.isMusicContextActiveButNotPlaying() && (!Game1.getMusicTrackName().EndsWith("_Ambient") || !(Game1.getMusicTrackName() != "Volcano_Ambient")))
        return;
      Game1.changeMusicTrack("Volcano_Ambient");
    }

    public virtual void UpdateShortcutOut()
    {
      if (this != Game1.currentLocation)
        return;
      if (this.shortcutOutUnlocked.Value)
      {
        this.setMapTileIndex(VolcanoDungeon.shortcutOutPosition.X, VolcanoDungeon.shortcutOutPosition.Y, 367, "Buildings");
        this.removeTile(VolcanoDungeon.shortcutOutPosition.X, VolcanoDungeon.shortcutOutPosition.Y - 1, "Front");
      }
      else
      {
        this.setMapTileIndex(VolcanoDungeon.shortcutOutPosition.X, VolcanoDungeon.shortcutOutPosition.Y, 399, "Buildings");
        this.setMapTileIndex(VolcanoDungeon.shortcutOutPosition.X, VolcanoDungeon.shortcutOutPosition.Y - 1, 383, "Front");
      }
    }

    public virtual void UpdateBridge()
    {
      if (this != Game1.currentLocation)
        return;
      if (Game1.MasterPlayer.hasOrWillReceiveMail("reachedCaldera"))
      {
        this.setMapTileIndex(27, 39, 399, "Buildings");
        this.setMapTileIndex(27, 38, 383, "Front");
      }
      if (!this.bridgeUnlocked.Value)
        return;
      for (int index1 = 28; index1 <= 32; ++index1)
      {
        for (int index2 = 34; index2 <= 37; ++index2)
        {
          int index3;
          switch (index1)
          {
            case 28:
              switch (index2)
              {
                case 34:
                  index3 = 189;
                  break;
                case 37:
                  index3 = 221;
                  break;
                default:
                  index3 = 205;
                  break;
              }
              break;
            case 32:
              switch (index2)
              {
                case 34:
                  index3 = 191;
                  break;
                case 37:
                  index3 = 223;
                  break;
                default:
                  index3 = 207;
                  break;
              }
              break;
            default:
              switch (index2)
              {
                case 34:
                  index3 = 190;
                  break;
                case 37:
                  index3 = 222;
                  break;
                default:
                  index3 = 206;
                  break;
              }
              break;
          }
          this.setMapTileIndex(index1, index2, index3, "Buildings");
          this.setTileProperty(index1, index2, "Buildings", "Passable", "T");
          this.removeTileProperty(index1, index2, "Back", "Water");
          NPC npc = this.isCharacterAtTile(new Vector2((float) index1, (float) index2));
          if (npc != null && npc is Monster)
            this.characters.Remove(npc);
          if (this.waterTiles != null && index1 != 28 && index1 != 32)
            this.waterTiles[index1, index2] = false;
          if (this.cooledLavaTiles.ContainsKey(new Vector2((float) index1, (float) index2)))
            this.cooledLavaTiles.Remove(new Vector2((float) index1, (float) index2));
        }
      }
    }

    protected override void resetLocalState()
    {
      if (!this.generated)
      {
        this.GenerateContents();
        this.generated = true;
      }
      foreach (Vector2 key in this.cooledLavaTiles.Keys)
        this.UpdateLavaNeighbor((int) key.X, (int) key.Y);
      if ((int) (NetFieldBase<int, NetInt>) this.level == 0)
        this.UpdateBridge();
      if (this.level.Value == 5)
        this.UpdateShortcutOut();
      base.resetLocalState();
      Game1.ambientLight = Microsoft.Xna.Framework.Color.White;
      int num = (int) ((double) Game1.player.Position.Y / 64.0);
      if (this.level.Value == 0 && Game1.player.previousLocationName == "Caldera")
        Game1.player.Position = new Vector2(44f, 50f) * 64f;
      else if (num == 0 && this.endPosition.HasValue)
      {
        if (this.endPosition.HasValue)
          Game1.player.Position = new Vector2((float) this.endPosition.Value.X, (float) this.endPosition.Value.Y) * 64f;
      }
      else if (num == 1 && this.startPosition.HasValue)
        Game1.player.Position = new Vector2((float) this.startPosition.Value.X, (float) this.startPosition.Value.Y) * 64f;
      this.mapBaseTilesheet = Game1.temporaryContent.Load<Texture2D>(this.map.TileSheets[0].ImageSource);
      LocationRequest locationRequest = Game1.locationRequest;
      foreach (DwarfGate dwarfGate in this.dwarfGates)
        dwarfGate.ResetLocalState();
      if ((int) (NetFieldBase<int, NetInt>) this.level == 5)
      {
        Game1.changeMusicTrack("Volcano_Ambient");
        AmbientLocationSounds.addSound(new Vector2(29f, 31f), 0);
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.level == 1 || (Game1.random.NextDouble() < 0.25 || (int) (NetFieldBase<int, NetInt>) this.level == 6) && (Game1.getMusicTrackName() == "none" || Game1.isMusicContextActiveButNotPlaying() || Game1.getMusicTrackName().EndsWith("_Ambient")))
        Game1.changeMusicTrack("VolcanoMines");
      if (this.level.Value != 0)
        return;
      if (Game1.player.hasOrWillReceiveMail("Saw_Flame_Sprite_Volcano"))
        this._sawFlameSprite = true;
      if (!this._sawFlameSprite)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Monsters\\Magma Sprite", new Microsoft.Xna.Framework.Rectangle(0, 32, 16, 16), new Vector2(30f, 38f) * 64f, false, 0.0f, Microsoft.Xna.Framework.Color.White)
        {
          id = 999f,
          scale = 4f,
          totalNumberOfLoops = 99999,
          interval = 70f,
          light = true,
          lightRadius = 1f,
          animationLength = 7,
          layerDepth = 1f,
          yPeriodic = true,
          yPeriodicRange = 12f,
          yPeriodicLoopTime = 1000f,
          xPeriodic = true,
          xPeriodicRange = 16f,
          xPeriodicLoopTime = 1800f
        });
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\shadow", new Microsoft.Xna.Framework.Rectangle(0, 0, 12, 7), new Vector2(30.2f, 39.4f) * 64f, false, 0.0f, Microsoft.Xna.Framework.Color.White)
        {
          id = 998f,
          scale = 4f,
          totalNumberOfLoops = 99999,
          interval = 1000f,
          animationLength = 1,
          layerDepth = 1f / 1000f,
          yPeriodic = true,
          yPeriodicRange = 1f,
          yPeriodicLoopTime = 1000f,
          xPeriodic = true,
          xPeriodicRange = 16f,
          xPeriodicLoopTime = 1800f
        });
      }
      this.ApplyMapOverride("Mines\\Volcano_Well", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(22, 43, 6, 4)));
      for (int x = 24; x < 28; ++x)
      {
        for (int y = 43; y < 47; ++y)
          this.waterTiles[x, y] = true;
      }
    }

    protected override void resetSharedState()
    {
      base.resetSharedState();
      if ((int) (NetFieldBase<int, NetInt>) this.level == 5)
        return;
      this.waterColor.Value = Microsoft.Xna.Framework.Color.White;
    }

    public override bool CanRefillWateringCanOnTile(int tileX, int tileY) => (int) (NetFieldBase<int, NetInt>) this.level == 5 && new Microsoft.Xna.Framework.Rectangle(27, 29, 4, 4).Contains(tileX, tileY) || (int) (NetFieldBase<int, NetInt>) this.level == 0 && tileX > 23 && tileX < 28 && tileY > 42 && tileY < 47;

    public virtual void GenerateLevel(bool use_level_level_as_layout = false)
    {
      this.generationRandom = new Random(this.generationSeed.Value);
      this.generationRandom.Next();
      this.mapPath.Value = "Maps\\Mines\\VolcanoTemplate";
      this.updateMap();
      this.loadedMapPath = (string) (NetFieldBase<string, NetString>) this.mapPath;
      Texture2D texture2D = Game1.temporaryContent.Load<Texture2D>("VolcanoLayouts\\Layouts");
      this.mapWidth = 64;
      this.mapHeight = 64;
      this.waterTiles = (WaterTiles) new bool[this.mapWidth, this.mapHeight];
      for (int index = 0; index < this.map.Layers.Count; ++index)
      {
        Layer layer = this.map.Layers[index];
        this.map.RemoveLayer(layer);
        this.map.InsertLayer(new Layer(layer.Id, this.map, new Size(this.mapWidth, this.mapHeight), layer.TileSize), index);
      }
      this.backLayer = this.map.GetLayer("Back");
      this.buildingsLayer = this.map.GetLayer("Buildings");
      this.frontLayer = this.map.GetLayer("Front");
      this.alwaysFrontLayer = this.map.GetLayer("AlwaysFront");
      this.map.TileSheets[0].TileIndexProperties[1].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[2].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[3].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[17].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[18].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[19].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[528].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[544].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[560].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[545].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[561].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[564].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[565].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[555].Add("Type", new PropertyValue("Stone"));
      this.map.TileSheets[0].TileIndexProperties[571].Add("Type", new PropertyValue("Stone"));
      this.pixelMap = new Microsoft.Xna.Framework.Color[this.mapWidth * this.mapHeight];
      this.heightMap = new int[this.mapWidth * this.mapHeight];
      int num1 = texture2D.Width / 64;
      int num2 = this.layoutIndex.Value;
      int source_x = num2 % num1 * 64;
      int source_y = num2 / num1 * 64;
      bool flip_x = this.generationRandom.Next(2) == 1;
      if (this.layoutIndex.Value == 0 || this.layoutIndex.Value == 31)
        flip_x = false;
      this.ApplyPixels("VolcanoLayouts\\Layouts", source_x, source_y, this.mapWidth, this.mapHeight, flip_x: flip_x);
      for (int x = 0; x < this.mapWidth; ++x)
      {
        for (int y = 0; y < this.mapHeight; ++y)
          this.PlaceGroundTile(x, y);
      }
      this.ApplyToColor(new Microsoft.Xna.Framework.Color(0, (int) byte.MaxValue, 0), (Action<int, int>) ((x, y) =>
      {
        if (!this.startPosition.HasValue)
          this.startPosition = new Point?(new Point(x, y));
        if (this.level.Value == 0)
          this.warps.Add(new Warp(x, y + 2, "IslandNorth", 40, 24, false));
        else
          this.warps.Add(new Warp(x, y + 2, nameof (VolcanoDungeon) + (this.level.Value - 1).ToString(), x - this.startPosition.Value.X, 0, false));
      }));
      this.ApplyToColor(new Microsoft.Xna.Framework.Color((int) byte.MaxValue, 0, 0), (Action<int, int>) ((x, y) =>
      {
        if (!this.endPosition.HasValue)
          this.endPosition = new Point?(new Point(x, y));
        if (this.level.Value == 9)
          this.warps.Add(new Warp(x, y - 2, "Caldera", 21, 39, false));
        else
          this.warps.Add(new Warp(x, y - 2, nameof (VolcanoDungeon) + (this.level.Value + 1).ToString(), x - this.endPosition.Value.X, 1, false));
      }));
      VolcanoDungeon.setPieceAreas.Clear();
      Microsoft.Xna.Framework.Color set_piece_color = new Microsoft.Xna.Framework.Color((int) byte.MaxValue, (int) byte.MaxValue, 0);
      this.ApplyToColor(set_piece_color, (Action<int, int>) ((x, y) =>
      {
        int num3 = 0;
        while (num3 < 32 && !(this.GetPixel(x + num3, y, Microsoft.Xna.Framework.Color.Black) != set_piece_color) && !(this.GetPixel(x, y + num3, Microsoft.Xna.Framework.Color.Black) != set_piece_color))
          ++num3;
        VolcanoDungeon.setPieceAreas.Add(new Microsoft.Xna.Framework.Rectangle(x, y, num3, num3));
        for (int index1 = 0; index1 < num3; ++index1)
        {
          for (int index2 = 0; index2 < num3; ++index2)
            this.SetPixelMap(x + index1, y + index2, Microsoft.Xna.Framework.Color.White);
        }
      }));
      this.possibleSwitchPositions = new Dictionary<int, List<Point>>();
      this.possibleGatePositions = new Dictionary<int, List<Point>>();
      this.ApplyToColor(new Microsoft.Xna.Framework.Color(128, 128, 128), (Action<int, int>) ((x, y) => this.AddPossibleSwitchLocation(0, x, y)));
      this.ApplySetPieces();
      this.GenerateWalls(Microsoft.Xna.Framework.Color.Black, 0, 4, random_wall_variants: 4, start_in_wall: true, on_insufficient_wall_height: ((Action<int, int>) ((x, y) => this.SetPixelMap(x, y, Microsoft.Xna.Framework.Color.Chartreuse))), use_corner_hack: true);
      this.GenerateWalls(Microsoft.Xna.Framework.Color.Chartreuse, 0, 13, 1);
      this.ApplyToColor(Microsoft.Xna.Framework.Color.Blue, (Action<int, int>) ((x, y) =>
      {
        this.waterTiles[x, y] = true;
        this.SetTile(this.backLayer, x, y, 4);
        this.setTileProperty(x, y, "Back", "Water", "T");
        if (this.generationRandom.NextDouble() >= 0.1)
          return;
        this.sharedLights[x + y * 1000] = new LightSource(4, new Vector2((float) x, (float) y) * 64f, 2f, new Microsoft.Xna.Framework.Color(0, 50, 50), x + y * 1000);
      }));
      this.GenerateBlobs(Microsoft.Xna.Framework.Color.Blue, 0, 16, is_lava_pool: true);
      if (this.startPosition.HasValue)
        this.CreateEntrance(new Point?(this.startPosition.Value));
      if (this.endPosition.HasValue)
        this.CreateExit(this.endPosition);
      if ((int) (NetFieldBase<int, NetInt>) this.level != 0)
        this.GenerateDirtTiles();
      if ((this.level.Value == 9 || this.generationRandom.NextDouble() < (this.isMonsterLevel() ? 1.0 : 0.2)) && this.possibleSwitchPositions.ContainsKey(0) && this.possibleSwitchPositions[0].Count > 0)
        this.AddPossibleGateLocation(0, this.endPosition.Value.X, this.endPosition.Value.Y);
      foreach (int key in this.possibleGatePositions.Keys)
      {
        if (this.possibleGatePositions[key].Count > 0 && this.possibleSwitchPositions.ContainsKey(key) && this.possibleSwitchPositions[key].Count > 0)
        {
          Point random = Utility.GetRandom<Point>(this.possibleGatePositions[key], this.generationRandom);
          this.CreateDwarfGate(key, random);
        }
      }
      if ((int) (NetFieldBase<int, NetInt>) this.level == 0)
      {
        this.CreateExit(new Point?(new Point(40, 48)), false);
        this.removeTile(40, 46, "Buildings");
        this.removeTile(40, 45, "Buildings");
        this.removeTile(40, 44, "Buildings");
        this.setMapTileIndex(40, 45, 266, "AlwaysFront");
        this.setMapTileIndex(40, 44, 76, "AlwaysFront");
        this.setMapTileIndex(39, 44, 76, "AlwaysFront");
        this.setMapTileIndex(41, 44, 76, "AlwaysFront");
        this.removeTile(40, 43, "Front");
        this.setMapTileIndex(40, 43, 70, "AlwaysFront");
        this.removeTile(39, 43, "Front");
        this.setMapTileIndex(39, 43, 69, "AlwaysFront");
        this.removeTile(41, 43, "Front");
        this.setMapTileIndex(41, 43, 69, "AlwaysFront");
        this.setMapTileIndex(39, 45, 265, "AlwaysFront");
        this.setMapTileIndex(41, 45, 267, "AlwaysFront");
        this.setMapTileIndex(40, 45, 60, "Back");
        this.setMapTileIndex(40, 46, 60, "Back");
        this.setMapTileIndex(40, 47, 60, "Back");
        this.setMapTileIndex(40, 48, 555, "Back");
        this.AddPossibleSwitchLocation(-1, 40, 51);
        this.CreateDwarfGate(-1, new Point(40, 48));
        this.setMapTileIndex(34, 30, 90, "Buildings");
        this.setMapTileIndex(34, 29, 148, "Buildings");
        this.setMapTileIndex(34, 31, 180, "Buildings");
        this.setMapTileIndex(34, 32, 196, "Buildings");
        this.CoolLava(34, 34, false);
        if (Game1.MasterPlayer.hasOrWillReceiveMail("volcanoShortcutUnlocked"))
        {
          foreach (DwarfGate dwarfGate in this.dwarfGates)
          {
            if (dwarfGate.gateIndex.Value == -1)
            {
              dwarfGate.opened.Value = true;
              dwarfGate.triggeredOpen = true;
              foreach (Point key in dwarfGate.switches.Keys)
                dwarfGate.switches[key] = true;
            }
          }
        }
        this.CreateExit(new Point?(new Point(44, 50)));
        this.warps.Add(new Warp(44, 48, "Caldera", 11, 36, false));
        this.CreateEntrance(new Point?(new Point(6, 48)));
        this.warps.Add(new Warp(6, 50, "IslandNorth", 12, 31, false));
      }
      if (Game1.IsMasterGame)
        this.GenerateEntities();
      this.pixelMap = (Microsoft.Xna.Framework.Color[]) null;
    }

    public virtual void GenerateDirtTiles()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.level == 5)
        return;
      for (int index1 = 0; index1 < 8; ++index1)
      {
        int num1 = this.generationRandom.Next(0, 64);
        int num2 = this.generationRandom.Next(0, 64);
        int num3 = this.generationRandom.Next(2, 8);
        int num4 = this.generationRandom.Next(1, 3);
        int num5 = this.generationRandom.Next(2) == 0 ? -1 : 1;
        int num6 = this.generationRandom.Next(2) == 0 ? -1 : 1;
        bool flag = this.generationRandom.Next(2) == 0;
        for (int index2 = 0; index2 < num3; ++index2)
        {
          for (int x = num1 - num4; x <= num1 + num4; ++x)
          {
            for (int y = num2 - num4; y <= num2 + num4; ++y)
            {
              if (!(this.GetPixel(x, y, Microsoft.Xna.Framework.Color.Black) != Microsoft.Xna.Framework.Color.White))
                this.dirtTiles.Add(new Point(x, y));
            }
          }
          if (flag)
            num6 += this.generationRandom.Next(2) == 0 ? -1 : 1;
          else
            num5 += this.generationRandom.Next(2) == 0 ? -1 : 1;
          num1 += num5;
          num2 += num6;
          num4 += this.generationRandom.Next(2) == 0 ? -1 : 1;
          if (num4 < 1)
            num4 = 1;
          if (num4 > 4)
            num4 = 4;
        }
      }
      for (int index = 0; index < 2; ++index)
        this.ErodeInvalidDirtTiles();
      HashSet<Point> pointSet = new HashSet<Point>();
      Point[] pointArray = new Point[8]
      {
        new Point(-1, -1),
        new Point(0, -1),
        new Point(1, -1),
        new Point(-1, 0),
        new Point(1, 0),
        new Point(-1, 1),
        new Point(0, 1),
        new Point(1, 1)
      };
      foreach (Point dirtTile in this.dirtTiles)
      {
        this.SetTile(this.backLayer, dirtTile.X, dirtTile.Y, VolcanoDungeon.GetTileIndex(9, 1));
        if (this.generationRandom.NextDouble() < 0.015)
          this.characters.Add((NPC) new Duggy(Utility.PointToVector2(dirtTile) * 64f, true));
        foreach (Point point1 in pointArray)
        {
          Point point2 = new Point(dirtTile.X + point1.X, dirtTile.Y + point1.Y);
          if (!this.dirtTiles.Contains(point2) && !pointSet.Contains(point2))
          {
            pointSet.Add(point2);
            Point? dirtNeighborTile = this.GetDirtNeighborTile(point2.X, point2.Y);
            if (dirtNeighborTile.HasValue)
              this.SetTile(this.backLayer, point2.X, point2.Y, VolcanoDungeon.GetTileIndex(8 + dirtNeighborTile.Value.X, dirtNeighborTile.Value.Y));
          }
        }
      }
    }

    public virtual void CreateEntrance(Point? position)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = 0; index2 <= 3; ++index2)
        {
          if (this.isTileOnMap(new Vector2((float) (position.Value.X + index1), (float) (position.Value.Y + index2))))
          {
            this.removeTile(position.Value.X + index1, position.Value.Y + index2, "Back");
            this.removeTile(position.Value.X + index1, position.Value.Y + index2, "Buildings");
            this.removeTile(position.Value.X + index1, position.Value.Y + index2, "Front");
          }
        }
      }
      if (this.getTileIndexAt(position.Value.X - 1, position.Value.Y - 1, "Front") != -1)
        this.SetTile(this.frontLayer, position.Value.X - 1, position.Value.Y - 1, VolcanoDungeon.GetTileIndex(13, 16));
      this.removeTile(position.Value.X, position.Value.Y - 1, "Front");
      this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y, VolcanoDungeon.GetTileIndex(13, 17));
      this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y + 1, VolcanoDungeon.GetTileIndex(13, 18));
      this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y + 2, VolcanoDungeon.GetTileIndex(13, 19));
      if (this.getTileIndexAt(position.Value.X + 1, position.Value.Y - 1, "Front") != -1)
        this.SetTile(this.frontLayer, position.Value.X + 1, position.Value.Y - 1, VolcanoDungeon.GetTileIndex(15, 16));
      this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y, VolcanoDungeon.GetTileIndex(15, 17));
      this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y + 1, VolcanoDungeon.GetTileIndex(15, 18));
      this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y + 2, VolcanoDungeon.GetTileIndex(15, 19));
      this.SetTile(this.backLayer, position.Value.X, position.Value.Y, VolcanoDungeon.GetTileIndex(14, 17));
      this.SetTile(this.backLayer, position.Value.X, position.Value.Y + 1, VolcanoDungeon.GetTileIndex(14, 18));
      this.SetTile(this.frontLayer, position.Value.X, position.Value.Y + 2, VolcanoDungeon.GetTileIndex(14, 19));
      this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y + 3, VolcanoDungeon.GetTileIndex(12, 4));
      this.SetTile(this.buildingsLayer, position.Value.X, position.Value.Y + 3, VolcanoDungeon.GetTileIndex(12, 4));
      this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y + 3, VolcanoDungeon.GetTileIndex(12, 4));
    }

    private void CreateExit(Point? position, bool draw_stairs = true)
    {
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -4; index2 <= 0; ++index2)
        {
          if (this.isTileOnMap(new Vector2((float) (position.Value.X + index1), (float) (position.Value.Y + index2))))
          {
            if (draw_stairs)
              this.removeTile(position.Value.X + index1, position.Value.Y + index2, "Back");
            this.removeTile(position.Value.X + index1, position.Value.Y + index2, "Buildings");
            this.removeTile(position.Value.X + index1, position.Value.Y + index2, "Front");
          }
        }
      }
      this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y, VolcanoDungeon.GetTileIndex(9, 19));
      this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y - 1, VolcanoDungeon.GetTileIndex(9, 18));
      this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y - 2, VolcanoDungeon.GetTileIndex(9, 17));
      this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y - 3, VolcanoDungeon.GetTileIndex(9, 16));
      this.SetTile(this.alwaysFrontLayer, position.Value.X - 1, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
      this.SetTile(this.alwaysFrontLayer, position.Value.X, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
      this.SetTile(this.alwaysFrontLayer, position.Value.X + 1, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
      this.SetTile(this.buildingsLayer, position.Value.X, position.Value.Y - 3, VolcanoDungeon.GetTileIndex(10, 16));
      this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y, VolcanoDungeon.GetTileIndex(11, 19));
      this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y - 1, VolcanoDungeon.GetTileIndex(11, 18));
      this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y - 2, VolcanoDungeon.GetTileIndex(11, 17));
      this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y - 3, VolcanoDungeon.GetTileIndex(11, 16));
      if (draw_stairs)
      {
        this.SetTile(this.backLayer, position.Value.X, position.Value.Y, VolcanoDungeon.GetTileIndex(12, 19));
        this.SetTile(this.backLayer, position.Value.X, position.Value.Y - 1, VolcanoDungeon.GetTileIndex(12, 18));
        this.SetTile(this.backLayer, position.Value.X, position.Value.Y - 2, VolcanoDungeon.GetTileIndex(12, 17));
        this.SetTile(this.backLayer, position.Value.X, position.Value.Y - 3, VolcanoDungeon.GetTileIndex(12, 16));
      }
      this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
      this.SetTile(this.buildingsLayer, position.Value.X, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
      this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
    }

    public virtual void ErodeInvalidDirtTiles()
    {
      Point[] pointArray = new Point[8]
      {
        new Point(-1, -1),
        new Point(0, -1),
        new Point(1, -1),
        new Point(-1, 0),
        new Point(1, 0),
        new Point(-1, 1),
        new Point(0, 1),
        new Point(1, 1)
      };
      Dictionary<Point, bool> dictionary = new Dictionary<Point, bool>();
      List<Point> pointList = new List<Point>();
      foreach (Point dirtTile in this.dirtTiles)
      {
        bool flag = false;
        foreach (Microsoft.Xna.Framework.Rectangle setPieceArea in VolcanoDungeon.setPieceAreas)
        {
          if (setPieceArea.Contains(dirtTile))
          {
            flag = true;
            break;
          }
        }
        if (!flag && this.getTileIndexAt(dirtTile, "Buildings") >= 0)
          flag = true;
        if (!flag)
        {
          foreach (Point point in pointArray)
          {
            Point key = new Point(dirtTile.X + point.X, dirtTile.Y + point.Y);
            if (dictionary.ContainsKey(key))
            {
              if (!dictionary[key])
              {
                flag = true;
                break;
              }
            }
            else if (!this.dirtTiles.Contains(key))
            {
              if (!this.GetDirtNeighborTile(key.X, key.Y).HasValue)
                flag = true;
              dictionary[key] = !flag;
              if (flag)
                break;
            }
          }
        }
        if (flag)
          pointList.Add(dirtTile);
      }
      foreach (Point point in pointList)
        this.dirtTiles.Remove(point);
    }

    public override void monsterDrop(Monster monster, int x, int y, Farmer who)
    {
      base.monsterDrop(monster, x, y, who);
      if (Game1.random.NextDouble() >= 0.05)
        return;
      Game1.player.team.RequestLimitedNutDrops("VolcanoMonsterDrop", (GameLocation) this, x, y, 5);
    }

    public Point? GetDirtNeighborTile(int tile_x, int tile_y)
    {
      if (this.GetPixel(tile_x, tile_y, Microsoft.Xna.Framework.Color.Black) != Microsoft.Xna.Framework.Color.White)
        return new Point?();
      if (this.getTileIndexAt(new Point(tile_x, tile_y), "Buildings") >= 0)
        return new Point?();
      if (this.dirtTiles.Contains(new Point(tile_x, tile_y - 1)) && this.dirtTiles.Contains(new Point(tile_x, tile_y + 1)))
        return new Point?();
      if (this.dirtTiles.Contains(new Point(tile_x - 1, tile_y)) && this.dirtTiles.Contains(new Point(tile_x + 1, tile_y)))
        return new Point?();
      if (this.dirtTiles.Contains(new Point(tile_x - 1, tile_y)) && !this.dirtTiles.Contains(new Point(tile_x + 1, tile_y)))
      {
        if (this.dirtTiles.Contains(new Point(tile_x, tile_y - 1)))
          return new Point?(new Point(3, 3));
        return this.dirtTiles.Contains(new Point(tile_x, tile_y + 1)) ? new Point?(new Point(3, 1)) : new Point?(new Point(2, 1));
      }
      if (this.dirtTiles.Contains(new Point(tile_x + 1, tile_y)) && !this.dirtTiles.Contains(new Point(tile_x - 1, tile_y)))
      {
        if (this.dirtTiles.Contains(new Point(tile_x, tile_y - 1)))
          return new Point?(new Point(3, 2));
        return this.dirtTiles.Contains(new Point(tile_x, tile_y + 1)) ? new Point?(new Point(3, 0)) : new Point?(new Point(0, 1));
      }
      if (this.dirtTiles.Contains(new Point(tile_x, tile_y - 1)) && !this.dirtTiles.Contains(new Point(tile_x, tile_y + 1)))
        return new Point?(new Point(1, 2));
      if (this.dirtTiles.Contains(new Point(tile_x, tile_y + 1)) && !this.dirtTiles.Contains(new Point(tile_x, tile_y - 1)))
        return new Point?(new Point(1, 0));
      if (this.dirtTiles.Contains(new Point(tile_x - 1, tile_y - 1)))
        return new Point?(new Point(2, 2));
      if (this.dirtTiles.Contains(new Point(tile_x + 1, tile_y - 1)))
        return new Point?(new Point(0, 2));
      if (this.dirtTiles.Contains(new Point(tile_x - 1, tile_y + 1)))
        return new Point?(new Point(0, 2));
      return this.dirtTiles.Contains(new Point(tile_x + 1, tile_y + 1)) ? new Point?(new Point(2, 2)) : new Point?();
    }

    public virtual void ErodeBadBorderedTiles()
    {
      List<Point> pointList = new List<Point>();
      foreach (Point dirtTile in this.dirtTiles)
      {
        bool flag1 = this.dirtTiles.Contains(new Point(dirtTile.X, dirtTile.Y - 1));
        bool flag2 = this.dirtTiles.Contains(new Point(dirtTile.X, dirtTile.Y + 1));
        bool flag3 = this.dirtTiles.Contains(new Point(dirtTile.X + 1, dirtTile.Y));
        bool flag4 = this.dirtTiles.Contains(new Point(dirtTile.X - 1, dirtTile.Y));
        if (!(flag1 & flag2 & flag3 & flag4))
        {
          if (flag1 & flag2 || flag1 && !flag2 || flag2 && !flag1)
            pointList.Add(dirtTile);
          else if (flag3 & flag4 || flag3 && !flag4 || flag4 && !flag3)
            pointList.Add(dirtTile);
        }
      }
      foreach (Point point in pointList)
        this.dirtTiles.Remove(point);
    }

    public HashSet<Point> GetDirtBorders()
    {
      HashSet<Point> borders = new HashSet<Point>();
      foreach (Point dirtTile in this.dirtTiles)
        this.AddBorders(dirtTile, borders);
      return borders;
    }

    public void AddBorders(Point point, HashSet<Point> borders)
    {
      Point[] pointArray = new Point[8]
      {
        new Point(-1, 0),
        new Point(-1, -1),
        new Point(0, -1),
        new Point(1, -1),
        new Point(1, 0),
        new Point(1, 1),
        new Point(0, 1),
        new Point(1, 1)
      };
      foreach (Point point1 in pointArray)
      {
        Point point2 = new Point(point.X + point1.X, point.Y + point1.Y);
        if (!this.dirtTiles.Contains(point2) && !borders.Contains(point2))
          borders.Add(point2);
      }
    }

    public virtual void CreateDwarfGate(int gate_index, Point tile_position)
    {
      this.SetTile(this.backLayer, tile_position.X, tile_position.Y + 1, VolcanoDungeon.GetTileIndex(3, 34));
      this.SetTile(this.buildingsLayer, tile_position.X - 1, tile_position.Y + 1, VolcanoDungeon.GetTileIndex(2, 34));
      this.SetTile(this.buildingsLayer, tile_position.X + 1, tile_position.Y + 1, VolcanoDungeon.GetTileIndex(4, 34));
      this.SetTile(this.buildingsLayer, tile_position.X - 1, tile_position.Y, VolcanoDungeon.GetTileIndex(2, 33));
      this.SetTile(this.buildingsLayer, tile_position.X + 1, tile_position.Y, VolcanoDungeon.GetTileIndex(4, 33));
      this.SetTile(this.frontLayer, tile_position.X - 1, tile_position.Y - 1, VolcanoDungeon.GetTileIndex(2, 32));
      this.SetTile(this.frontLayer, tile_position.X + 1, tile_position.Y - 1, VolcanoDungeon.GetTileIndex(4, 32));
      this.SetTile(this.alwaysFrontLayer, tile_position.X - 1, tile_position.Y - 1, VolcanoDungeon.GetTileIndex(2, 32));
      this.SetTile(this.alwaysFrontLayer, tile_position.X, tile_position.Y - 1, VolcanoDungeon.GetTileIndex(3, 32));
      this.SetTile(this.alwaysFrontLayer, tile_position.X + 1, tile_position.Y - 1, VolcanoDungeon.GetTileIndex(4, 32));
      if (gate_index == 0)
      {
        this.SetTile(this.alwaysFrontLayer, tile_position.X - 1, tile_position.Y - 2, VolcanoDungeon.GetTileIndex(0, 32));
        this.SetTile(this.alwaysFrontLayer, tile_position.X + 1, tile_position.Y - 2, VolcanoDungeon.GetTileIndex(0, 32));
      }
      else
      {
        this.SetTile(this.alwaysFrontLayer, tile_position.X - 1, tile_position.Y - 2, VolcanoDungeon.GetTileIndex(9, 25));
        this.SetTile(this.alwaysFrontLayer, tile_position.X + 1, tile_position.Y - 2, VolcanoDungeon.GetTileIndex(10, 25));
      }
      int seed = this.generationRandom.Next();
      if (!Game1.IsMasterGame)
        return;
      this.dwarfGates.Add(new DwarfGate(this, gate_index, tile_position.X, tile_position.Y, seed));
    }

    public virtual void AddPossibleSwitchLocation(int switch_index, int x, int y)
    {
      if (!this.possibleSwitchPositions.ContainsKey(switch_index))
        this.possibleSwitchPositions[switch_index] = new List<Point>();
      this.possibleSwitchPositions[switch_index].Add(new Point(x, y));
    }

    public virtual void AddPossibleGateLocation(int gate_index, int x, int y)
    {
      if (!this.possibleGatePositions.ContainsKey(gate_index))
        this.possibleGatePositions[gate_index] = new List<Point>();
      this.possibleGatePositions[gate_index].Add(new Point(x, y));
    }

    private void adjustLevelChances(
      ref double stoneChance,
      ref double monsterChance,
      ref double itemChance,
      ref double gemStoneChance)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.level == 0 || (int) (NetFieldBase<int, NetInt>) this.level == 5)
      {
        monsterChance = 0.0;
        itemChance = 0.0;
        gemStoneChance = 0.0;
        stoneChance = 0.0;
      }
      if (this.isMushroomLevel())
      {
        monsterChance = 0.025;
        itemChance *= 35.0;
        stoneChance = 0.0;
      }
      else if (this.isMonsterLevel())
      {
        stoneChance = 0.0;
        itemChance = 0.0;
        monsterChance *= 2.0;
      }
      bool flag1 = false;
      bool flag2 = false;
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (onlineFarmer.hasBuff(23))
          flag1 = true;
        if (onlineFarmer.hasBuff(24))
          flag2 = true;
        if (flag2 & flag1)
          break;
      }
      if (flag2)
        monsterChance *= 2.0;
      gemStoneChance /= 2.0;
    }

    public bool isTileClearForMineObjects(Vector2 v, bool ignoreRuins = false)
    {
      if ((double) Math.Abs((float) this.startPosition.Value.X - v.X) <= 2.0 && (double) Math.Abs((float) this.startPosition.Value.Y - v.Y) <= 2.0 || (double) Math.Abs((float) this.endPosition.Value.X - v.X) <= 2.0 && (double) Math.Abs((float) this.endPosition.Value.Y - v.Y) <= 2.0 || this.GetPixel((int) v.X, (int) v.Y, Microsoft.Xna.Framework.Color.Black) == new Microsoft.Xna.Framework.Color(128, 128, 128) || !this.isTileLocationTotallyClearAndPlaceable(v))
        return false;
      switch (this.doesTileHaveProperty((int) v.X, (int) v.Y, "Type", "Back"))
      {
        case "Stone":
          return this.isTileOnClearAndSolidGround(v) && !this.objects.ContainsKey(v) && (!ignoreRuins || this.map.GetLayer("Back").Tiles[(int) v.X, (int) v.Y] != null && this.map.GetLayer("Back").Tiles[(int) v.X, (int) v.Y].TileIndex < 384);
        default:
          return false;
      }
    }

    public bool isTileOnClearAndSolidGround(Vector2 v) => this.map.GetLayer("Back").Tiles[(int) v.X, (int) v.Y] != null && this.map.GetLayer("Front").Tiles[(int) v.X, (int) v.Y] == null && this.map.GetLayer("Buildings").Tiles[(int) v.X, (int) v.Y] == null;

    public virtual void GenerateEntities()
    {
      List<Point> spawn_points = new List<Point>();
      this.ApplyToColor(new Microsoft.Xna.Framework.Color(0, (int) byte.MaxValue, (int) byte.MaxValue), (Action<int, int>) ((x, y) => spawn_points.Add(new Point(x, y))));
      List<Point> spiker_spawn_points = new List<Point>();
      this.ApplyToColor(new Microsoft.Xna.Framework.Color(0, 128, (int) byte.MaxValue), (Action<int, int>) ((x, y) => spiker_spawn_points.Add(new Point(x, y))));
      double stoneChance = (double) this.generationRandom.Next(11, 18) / 150.0;
      double monsterChance = 0.0008 + (double) this.generationRandom.Next(70) / 10000.0;
      double itemChance = 0.001;
      double gemStoneChance = 0.003;
      this.adjustLevelChances(ref stoneChance, ref monsterChance, ref itemChance, ref gemStoneChance);
      if ((int) (NetFieldBase<int, NetInt>) this.level > 0 && (int) (NetFieldBase<int, NetInt>) this.level != 5 && (this.generationRandom.NextDouble() < 0.5 || this.isMushroomLevel()))
      {
        int num = this.generationRandom.Next(5) + (int) (Game1.player.team.AverageDailyLuck(Game1.currentLocation) * 20.0);
        if (this.isMushroomLevel())
          num += 50;
        for (int index = 0; index < num; ++index)
        {
          Point point1;
          Point point2;
          if (this.generationRandom.NextDouble() < 0.33)
          {
            point1 = new Point(this.generationRandom.Next(this.map.GetLayer("Back").LayerWidth), 0);
            point2 = new Point(0, 1);
          }
          else if (this.generationRandom.NextDouble() < 0.5)
          {
            point1 = new Point(0, this.generationRandom.Next(this.map.GetLayer("Back").LayerHeight));
            point2 = new Point(1, 0);
          }
          else
          {
            point1 = new Point(this.map.GetLayer("Back").LayerWidth - 1, this.generationRandom.Next(this.map.GetLayer("Back").LayerHeight));
            point2 = new Point(-1, 0);
          }
          while (this.isTileOnMap(point1.X, point1.Y))
          {
            point1.X += point2.X;
            point1.Y += point2.Y;
            if (this.isTileClearForMineObjects(new Vector2((float) point1.X, (float) point1.Y)))
            {
              Vector2 vector2 = new Vector2((float) point1.X, (float) point1.Y);
              if (this.isMushroomLevel())
              {
                this.terrainFeatures.Add(vector2, (TerrainFeature) new CosmeticPlant(6 + this.generationRandom.Next(3)));
                break;
              }
              this.objects.Add(vector2, (StardewValley.Object) new BreakableContainer(vector2, true));
              break;
            }
          }
        }
      }
      if ((int) (NetFieldBase<int, NetInt>) this.level != 5)
      {
        for (int x = 0; x < this.map.Layers[0].LayerWidth; ++x)
        {
          for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
          {
            Vector2 vector2 = new Vector2((float) x, (float) y);
            if (((double) Math.Abs((float) this.startPosition.Value.X - vector2.X) > 5.0 || (double) Math.Abs((float) this.startPosition.Value.Y - vector2.Y) > 5.0) && ((double) Math.Abs((float) this.endPosition.Value.X - vector2.X) > 5.0 || (double) Math.Abs((float) this.endPosition.Value.Y - vector2.Y) > 5.0))
            {
              if (this.isTileLocationTotallyClearAndPlaceable(vector2) && this.generationRandom.NextDouble() < monsterChance)
              {
                if (this.getTileIndexAt((int) vector2.X, (int) vector2.Y, "Back") == 25)
                {
                  if (!this.isMushroomLevel())
                    this.characters.Add((NPC) new Duggy(vector2 * 64f, true));
                }
                else if (this.isMushroomLevel())
                  this.characters.Add((NPC) new RockCrab(vector2 * 64f, "False Magma Cap"));
                else
                  this.characters.Add((NPC) new Bat(vector2 * 64f, (int) (NetFieldBase<int, NetInt>) this.level <= 5 || this.generationRandom.NextDouble() >= 0.5 ? -555 : -556));
              }
              else if (this.isTileClearForMineObjects(vector2, true))
              {
                double num = stoneChance;
                if (num > 0.0)
                {
                  foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(vector2))
                  {
                    if (this.objects.ContainsKey(adjacentTileLocation))
                      num += 0.1;
                  }
                }
                int stone1 = this.chooseStoneTypeIndexOnly(vector2);
                bool flag = stone1 >= 845 && stone1 <= 847;
                if (num > 0.0 && (!flag || this.generationRandom.NextDouble() < num))
                {
                  StardewValley.Object stone2 = this.createStone(stone1, vector2);
                  if (stone2 != null)
                    this.Objects.Add(vector2, stone2);
                }
                else if (this.generationRandom.NextDouble() < itemChance)
                  this.Objects.Add(vector2, new StardewValley.Object(851, 1)
                  {
                    IsSpawnedObject = true,
                    CanBeGrabbed = true
                  });
              }
            }
          }
        }
        while (this.generationRandom.NextDouble() < 0.2)
          this.tryToAddOreClumps();
      }
      for (int index1 = 0; index1 < 7 && spawn_points.Count != 0; ++index1)
      {
        int index2 = this.generationRandom.Next(0, spawn_points.Count);
        Point p1 = spawn_points[index2];
        if (this.isTileLocationTotallyClearAndPlaceable(p1.X, p1.Y))
        {
          Monster monster = (Monster) null;
          if (this.generationRandom.NextDouble() <= 0.25)
          {
            for (int index3 = 0; index3 < 20; ++index3)
            {
              Point p2 = p1;
              p2.X += this.generationRandom.Next(-10, 11);
              p2.Y += this.generationRandom.Next(-10, 11);
              bool flag = false;
              for (int index4 = -1; index4 <= 1; ++index4)
              {
                for (int index5 = -1; index5 <= 1; ++index5)
                {
                  if (!LavaLurk.IsLavaTile((GameLocation) this, p2.X + index4, p2.Y + index5))
                  {
                    flag = true;
                    break;
                  }
                }
              }
              if (!flag)
              {
                monster = (Monster) new LavaLurk(Utility.PointToVector2(p2) * 64f);
                break;
              }
            }
          }
          if (monster == null && this.generationRandom.NextDouble() <= 0.200000002980232)
            monster = (Monster) new HotHead(Utility.PointToVector2(p1) * 64f);
          if (monster == null)
          {
            monster = (Monster) new GreenSlime(Utility.PointToVector2(p1) * 64f, 0);
            (monster as GreenSlime).makeTigerSlime();
          }
          if (monster != null)
            this.characters.Add((NPC) monster);
        }
        spawn_points.RemoveAt(index2);
      }
      foreach (Point p in spiker_spawn_points)
      {
        if (this.isTileLocationTotallyClearAndPlaceable(p.X, p.Y))
        {
          int direction = 1;
          switch (this.getTileIndexAt(p, "Back"))
          {
            case 537:
            case 538:
              direction = 2;
              break;
            case 552:
            case 569:
              direction = 3;
              break;
            case 553:
            case 570:
              direction = 0;
              break;
          }
          this.characters.Add((NPC) new Spiker(new Vector2((float) p.X, (float) p.Y) * 64f, direction));
        }
      }
    }

    private StardewValley.Object createStone(int stone, Vector2 tile)
    {
      int parentSheetIndex = this.chooseStoneTypeIndexOnly(tile);
      int num = 1;
      switch (parentSheetIndex)
      {
        case 290:
          num = 8;
          break;
        case 751:
          num = 8;
          break;
        case 764:
          num = 8;
          break;
        case 765:
          num = 16;
          break;
        case 819:
          num = 8;
          break;
        case 843:
        case 844:
          num = 12;
          break;
        case 845:
        case 846:
        case 847:
          num = 6;
          break;
      }
      return new StardewValley.Object(tile, parentSheetIndex, "Stone", true, false, false, false)
      {
        MinutesUntilReady = num
      };
    }

    private int chooseStoneTypeIndexOnly(Vector2 tile)
    {
      int num1 = this.generationRandom.Next(845, 848);
      float num2 = (float) (1.0 + (double) (int) (NetFieldBase<int, NetInt>) this.level / 7.0);
      float num3 = 0.8f;
      float num4 = (float) (1.0 + Game1.player.team.AverageLuckLevel() * 0.0350000001490116 + Game1.player.team.AverageDailyLuck() / 2.0);
      double num5 = 0.008 * (double) num2 * (double) num3 * (double) num4;
      foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(tile))
      {
        if (this.objects.ContainsKey(adjacentTileLocation) && ((int) (NetFieldBase<int, NetInt>) this.objects[adjacentTileLocation].parentSheetIndex == 843 || (int) (NetFieldBase<int, NetInt>) this.objects[adjacentTileLocation].parentSheetIndex == 844))
          num5 += 0.15;
      }
      if (this.generationRandom.NextDouble() < num5)
      {
        num1 = this.generationRandom.Next(843, 845);
      }
      else
      {
        double num6 = 1.0 / 400.0 * (double) num2 * (double) num3 * (double) num4;
        foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(tile))
        {
          if (this.objects.ContainsKey(adjacentTileLocation) && (int) (NetFieldBase<int, NetInt>) this.objects[adjacentTileLocation].parentSheetIndex == 765)
            num6 += 0.1;
        }
        if (this.generationRandom.NextDouble() < num6)
        {
          num1 = 765;
        }
        else
        {
          double num7 = 0.01 * (double) num2 * (double) num3;
          foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(tile))
          {
            if (this.objects.ContainsKey(adjacentTileLocation) && (int) (NetFieldBase<int, NetInt>) this.objects[adjacentTileLocation].parentSheetIndex == 764)
              num7 += 0.2;
          }
          if (this.generationRandom.NextDouble() < num7)
          {
            num1 = 764;
          }
          else
          {
            double num8 = 0.015 * (double) num2 * (double) num3;
            foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(tile))
            {
              if (this.objects.ContainsKey(adjacentTileLocation) && (int) (NetFieldBase<int, NetInt>) this.objects[adjacentTileLocation].parentSheetIndex == 850)
                num8 += 0.25;
            }
            if (this.generationRandom.NextDouble() < num8)
            {
              num1 = 850;
            }
            else
            {
              double num9 = 0.018 * (double) num2 * (double) num3;
              foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(tile))
              {
                if (this.objects.ContainsKey(adjacentTileLocation) && (int) (NetFieldBase<int, NetInt>) this.objects[adjacentTileLocation].parentSheetIndex == 849)
                  num9 += 0.25;
              }
              if (this.generationRandom.NextDouble() < num9)
                num1 = 849;
            }
          }
        }
      }
      if (this.generationRandom.NextDouble() < 0.0005)
        num1 = 819;
      if (this.generationRandom.NextDouble() < 0.0007)
        num1 = 44;
      if ((int) (NetFieldBase<int, NetInt>) this.level > 2 && this.generationRandom.NextDouble() < 0.0002)
        num1 = 46;
      return num1;
    }

    public void tryToAddOreClumps()
    {
      if (this.generationRandom.NextDouble() >= 0.55 + Game1.player.team.AverageDailyLuck(Game1.currentLocation))
        return;
      Vector2 randomTile = this.getRandomTile();
      for (int index = 0; index < 1 || this.generationRandom.NextDouble() < 0.25 + Game1.player.team.AverageDailyLuck(Game1.currentLocation); ++index)
      {
        if (this.isTileLocationTotallyClearAndPlaceable(randomTile) && this.isTileOnClearAndSolidGround(randomTile) && this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "Diggable", "Back") == null)
        {
          StardewValley.Object o = new StardewValley.Object(randomTile, this.generationRandom.Next(843, 845), "Stone", true, false, false, false)
          {
            MinutesUntilReady = 12
          };
          if ((int) (NetFieldBase<int, NetInt>) o.parentSheetIndex == 670)
            o.ParentSheetIndex = 668;
          Utility.recursiveObjectPlacement(o, (int) randomTile.X, (int) randomTile.Y, 0.949999988079071, 0.300000011920929, (GameLocation) this, "Dirt", (int) (NetFieldBase<int, NetInt>) o.parentSheetIndex == 668 ? 1 : 0, 0.0500000007450581, (int) (NetFieldBase<int, NetInt>) o.parentSheetIndex == 668 ? 2 : 1);
        }
        randomTile = this.getRandomTile();
      }
    }

    public virtual void ApplySetPieces()
    {
      for (int index1 = 0; index1 < VolcanoDungeon.setPieceAreas.Count; ++index1)
      {
        Microsoft.Xna.Framework.Rectangle setPieceArea = VolcanoDungeon.setPieceAreas[index1];
        int num1 = 3;
        if (setPieceArea.Width >= 32)
          num1 = 32;
        else if (setPieceArea.Width >= 16)
          num1 = 16;
        else if (setPieceArea.Width >= 8)
          num1 = 8;
        else if (setPieceArea.Width >= 4)
          num1 = 4;
        Map override_map = Game1.game1.xTileContent.Load<Map>("Maps\\Mines\\Volcano_SetPieces_" + num1.ToString());
        int maxValue1 = override_map.Layers[0].LayerWidth / num1;
        int maxValue2 = override_map.Layers[0].LayerHeight / num1;
        int num2 = this.generationRandom.Next(0, maxValue1);
        int num3 = this.generationRandom.Next(0, maxValue2);
        this.ApplyMapOverride(override_map, "area_" + index1.ToString(), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(num2 * num1, num3 * num1, num1, num1)), new Microsoft.Xna.Framework.Rectangle?(setPieceArea));
        Layer layer = override_map.GetLayer("Paths");
        if (layer != null)
        {
          for (int index2 = 0; index2 < num1; ++index2)
          {
            for (int index3 = 0; index3 <= num1; ++index3)
            {
              int num4 = num2 * num1 + index2;
              int num5 = num3 * num1 + index3;
              int num6 = setPieceArea.Left + index2;
              int num7 = setPieceArea.Top + index3;
              if (layer.IsValidTileLocation(num4, num5))
              {
                Tile tile = layer.Tiles[num4, num5];
                int num8 = -1;
                if (tile != null)
                  num8 = tile.TileIndex;
                if (num8 >= VolcanoDungeon.GetTileIndex(10, 14) && num8 <= VolcanoDungeon.GetTileIndex(15, 14))
                {
                  int gate_index = num8 - VolcanoDungeon.GetTileIndex(10, 14);
                  if (gate_index > 0)
                    gate_index += index1 * 10;
                  double result = 1.0;
                  if (tile.Properties.ContainsKey("Chance") && !double.TryParse(tile.Properties["Chance"].ToString(), out result))
                    result = 1.0;
                  if (this.generationRandom.NextDouble() < result)
                    this.AddPossibleGateLocation(gate_index, num6, num7);
                }
                else if (num8 >= VolcanoDungeon.GetTileIndex(10, 15) && num8 <= VolcanoDungeon.GetTileIndex(15, 15))
                {
                  int switch_index = num8 - VolcanoDungeon.GetTileIndex(10, 15);
                  if (switch_index > 0)
                    switch_index += index1 * 10;
                  this.AddPossibleSwitchLocation(switch_index, num6, num7);
                }
                else if (num8 == VolcanoDungeon.GetTileIndex(10, 20))
                  this.SetPixelMap(num6, num7, new Microsoft.Xna.Framework.Color(0, (int) byte.MaxValue, (int) byte.MaxValue));
                else if (num8 == VolcanoDungeon.GetTileIndex(11, 20))
                  this.SetPixelMap(num6, num7, new Microsoft.Xna.Framework.Color(0, 0, (int) byte.MaxValue));
                else if (num8 == VolcanoDungeon.GetTileIndex(12, 20))
                  this.SpawnChest(num6, num7);
                else if (num8 == VolcanoDungeon.GetTileIndex(13, 20))
                  this.SetPixelMap(num6, num7, new Microsoft.Xna.Framework.Color(0, 0, 0));
                else if (num8 == VolcanoDungeon.GetTileIndex(14, 20) && this.generationRandom.NextDouble() < 0.5)
                {
                  if (Game1.IsMasterGame)
                    this.objects.Add(new Vector2((float) num6, (float) num7), (StardewValley.Object) new BreakableContainer(new Vector2((float) num6, (float) num7), true));
                }
                else if (num8 == VolcanoDungeon.GetTileIndex(15, 20) && this.generationRandom.NextDouble() < 0.5)
                {
                  if (Game1.IsMasterGame)
                    this.objects.Add(new Vector2((float) num6, (float) num7), new StardewValley.Object(852, 1)
                    {
                      IsSpawnedObject = true,
                      CanBeGrabbed = true
                    });
                }
                else if (num8 == VolcanoDungeon.GetTileIndex(10, 21))
                  this.SetPixelMap(num6, num7, new Microsoft.Xna.Framework.Color(0, 128, (int) byte.MaxValue));
              }
            }
          }
        }
      }
    }

    public virtual void SpawnChest(int tile_x, int tile_y)
    {
      Random chest_random = new Random(this.generationRandom.Next());
      float num = (float) (Game1.player.team.AverageLuckLevel() * 0.0350000001490116 + Game1.player.team.AverageDailyLuck() / 2.0);
      if (!Game1.IsMasterGame)
        return;
      Vector2 vector2 = new Vector2((float) tile_x, (float) tile_y);
      Chest o = new Chest(false, vector2);
      o.dropContents.Value = true;
      o.synchronized.Value = true;
      o.type.Value = "interactive";
      if (chest_random.NextDouble() < ((int) (NetFieldBase<int, NetInt>) this.level == 9 ? 0.5 + (double) num : 0.100000001490116 + (double) num))
      {
        o.SetBigCraftableSpriteIndex(227);
        this.PopulateChest(o.items, chest_random, 1);
      }
      else
      {
        o.SetBigCraftableSpriteIndex(223);
        this.PopulateChest(o.items, chest_random, 0);
      }
      this.setObject(vector2, (StardewValley.Object) o);
    }

    protected override bool breakStone(int indexOfStone, int x, int y, Farmer who, Random r)
    {
      if (indexOfStone >= 845 && indexOfStone <= 847 && Game1.random.NextDouble() < 0.005)
        Game1.createObjectDebris(827, x, y, (long) who.uniqueMultiplayerID, (GameLocation) this);
      if (r.NextDouble() < 0.03)
        Game1.player.team.RequestLimitedNutDrops("VolcanoMining", (GameLocation) this, x * 64, y * 64, 5);
      return base.breakStone(indexOfStone, x, y, who, r);
    }

    public virtual void PopulateChest(
      NetObjectList<Item> items,
      Random chest_random,
      int chest_type)
    {
      if (chest_type == 0)
      {
        int maxValue = 7;
        int num1 = chest_random.Next(maxValue);
        if (!Game1.netWorldState.Value.GoldenCoconutCracked.Value)
        {
          while (num1 == 1)
            num1 = chest_random.Next(maxValue);
        }
        if (Game1.random.NextDouble() <= 0.5 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        {
          int num2 = chest_random.Next(2, 6);
          for (int index = 0; index < num2; ++index)
            items.Add((Item) new StardewValley.Object(890, 1));
        }
        switch (num1)
        {
          case 0:
            for (int index = 0; index < 3; ++index)
              items.Add((Item) new StardewValley.Object(848, 1));
            break;
          case 1:
            items.Add((Item) new StardewValley.Object(791, 1));
            break;
          case 2:
            for (int index = 0; index < 8; ++index)
              items.Add((Item) new StardewValley.Object(831, 1));
            break;
          case 3:
            for (int index = 0; index < 5; ++index)
              items.Add((Item) new StardewValley.Object(833, 1));
            break;
          case 4:
            items.Add((Item) new Ring(861));
            break;
          case 5:
            items.Add((Item) new Ring(862));
            break;
          case 6:
            items.Add((Item) new MeleeWeapon(chest_random.Next(54, 57)));
            break;
        }
      }
      if (chest_type != 1)
        return;
      int maxValue1 = 9;
      int num3 = chest_random.Next(maxValue1);
      if (!Game1.netWorldState.Value.GoldenCoconutCracked.Value)
      {
        while (num3 == 3)
          num3 = chest_random.Next(maxValue1);
      }
      if (Game1.random.NextDouble() <= 1.0 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
      {
        int num4 = chest_random.Next(4, 6);
        for (int index = 0; index < num4; ++index)
          items.Add((Item) new StardewValley.Object(890, 1));
      }
      switch (num3)
      {
        case 0:
          for (int index = 0; index < 10; ++index)
            items.Add((Item) new StardewValley.Object(848, 1));
          break;
        case 1:
          items.Add((Item) new Boots(854));
          break;
        case 2:
          items.Add((Item) new Boots(855));
          break;
        case 3:
          for (int index = 0; index < 3; ++index)
            items.Add((Item) new StardewValley.Object(Vector2.Zero, 791, 1));
          break;
        case 4:
          items.Add((Item) new Ring(863));
          break;
        case 5:
          items.Add((Item) new Ring(860));
          break;
        case 6:
          items.Add((Item) new MeleeWeapon(chest_random.Next(57, 60)));
          break;
        case 7:
          items.Add((Item) new StardewValley.Objects.Hat(76));
          break;
        case 8:
          items.Add((Item) new StardewValley.Object(289, 1));
          break;
      }
    }

    public virtual void ApplyToColor(Microsoft.Xna.Framework.Color match, Action<int, int> action)
    {
      for (int x = 0; x < this.mapWidth; ++x)
      {
        for (int y = 0; y < this.mapHeight; ++y)
        {
          if (this.GetPixel(x, y, match) == match && action != null)
            action(x, y);
        }
      }
    }

    public override bool sinkDebris(Debris debris, Vector2 chunkTile, Vector2 chunkPosition) => !this.cooledLavaTiles.ContainsKey(chunkTile) && base.sinkDebris(debris, chunkTile, chunkPosition);

    public override bool performToolAction(Tool t, int tileX, int tileY)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.level != 5 && t is WateringCan && this.isTileOnMap(new Vector2((float) tileX, (float) tileY)) && this.waterTiles[tileX, tileY] && !this.cooledLavaTiles.ContainsKey(new Vector2((float) tileX, (float) tileY)))
        this.coolLavaEvent.Fire(new Point(tileX, tileY));
      return base.performToolAction(t, tileX, tileY);
    }

    public virtual void GenerateBlobs(
      Microsoft.Xna.Framework.Color match,
      int tile_x,
      int tile_y,
      bool fill_center = true,
      bool is_lava_pool = false)
    {
      for (int x = 0; x < this.mapWidth; ++x)
      {
        for (int y = 0; y < this.mapHeight; ++y)
        {
          if (this.GetPixel(x, y, match) == match)
          {
            int neighborValue = this.GetNeighborValue(x, y, match, is_lava_pool);
            if (fill_center || neighborValue != 15)
            {
              Dictionary<int, Point> dictionary = this.GetBlobLookup();
              if (is_lava_pool)
                dictionary = this.GetLavaBlobLookup();
              if (dictionary.ContainsKey(neighborValue))
              {
                Point point = dictionary[neighborValue];
                this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(tile_x + point.X, tile_y + point.Y));
              }
            }
          }
        }
      }
    }

    public Dictionary<int, Point> GetBlobLookup()
    {
      if (VolcanoDungeon._blobIndexLookup == null)
      {
        VolcanoDungeon._blobIndexLookup = new Dictionary<int, Point>();
        VolcanoDungeon._blobIndexLookup[0] = new Point(0, 0);
        VolcanoDungeon._blobIndexLookup[6] = new Point(1, 0);
        VolcanoDungeon._blobIndexLookup[14] = new Point(2, 0);
        VolcanoDungeon._blobIndexLookup[10] = new Point(3, 0);
        VolcanoDungeon._blobIndexLookup[7] = new Point(1, 1);
        VolcanoDungeon._blobIndexLookup[11] = new Point(3, 1);
        VolcanoDungeon._blobIndexLookup[5] = new Point(1, 2);
        VolcanoDungeon._blobIndexLookup[13] = new Point(2, 2);
        VolcanoDungeon._blobIndexLookup[9] = new Point(3, 2);
        VolcanoDungeon._blobIndexLookup[2] = new Point(0, 1);
        VolcanoDungeon._blobIndexLookup[3] = new Point(0, 2);
        VolcanoDungeon._blobIndexLookup[1] = new Point(0, 3);
        VolcanoDungeon._blobIndexLookup[4] = new Point(1, 3);
        VolcanoDungeon._blobIndexLookup[12] = new Point(2, 3);
        VolcanoDungeon._blobIndexLookup[8] = new Point(3, 3);
        VolcanoDungeon._blobIndexLookup[15] = new Point(2, 1);
      }
      return VolcanoDungeon._blobIndexLookup;
    }

    public override bool isCollidingPosition(
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
      return isFarmer && !glider && (position.Left < 0 || position.Right > this.map.DisplayWidth || position.Top < 0 || position.Bottom > this.map.DisplayHeight) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement);
    }

    public Dictionary<int, Point> GetLavaBlobLookup()
    {
      if (VolcanoDungeon._lavaBlobIndexLookup == null)
      {
        VolcanoDungeon._lavaBlobIndexLookup = new Dictionary<int, Point>((IDictionary<int, Point>) this.GetBlobLookup());
        VolcanoDungeon._lavaBlobIndexLookup[63] = new Point(2, 1);
        VolcanoDungeon._lavaBlobIndexLookup[47] = new Point(4, 3);
        VolcanoDungeon._lavaBlobIndexLookup[31] = new Point(4, 2);
        VolcanoDungeon._lavaBlobIndexLookup[15] = new Point(4, 1);
      }
      return VolcanoDungeon._lavaBlobIndexLookup;
    }

    public virtual void GenerateWalls(
      Microsoft.Xna.Framework.Color match,
      int source_x,
      int source_y,
      int wall_height = 4,
      int random_wall_variants = 1,
      bool start_in_wall = false,
      Action<int, int> on_insufficient_wall_height = null,
      bool use_corner_hack = false)
    {
      this.heightMap = new int[this.mapWidth * this.mapHeight];
      for (int index = 0; index < this.heightMap.Length; ++index)
        this.heightMap[index] = -1;
      for (int index = 0; index < 2; ++index)
      {
        for (int x = 0; x < this.mapWidth; ++x)
        {
          int num1 = -1;
          int num2 = 0;
          if (start_in_wall)
            num2 = wall_height;
          for (int y1 = 0; y1 <= this.mapHeight; ++y1)
          {
            if (this.GetPixel(x, y1, match) != match || y1 >= this.mapHeight)
            {
              int num3 = 0;
              int num4 = 0;
              if (random_wall_variants > 1 && this.generationRandom.NextDouble() < 0.5)
                num4 = this.generationRandom.Next(1, random_wall_variants);
              if (y1 >= this.mapHeight)
              {
                num3 = wall_height;
                num2 = wall_height;
              }
              for (int y2 = y1 - 1; y2 > num1; --y2)
              {
                if (num2 < wall_height)
                {
                  if (on_insufficient_wall_height != null)
                  {
                    on_insufficient_wall_height(x, y2);
                  }
                  else
                  {
                    this.SetPixelMap(x, y2, Microsoft.Xna.Framework.Color.White);
                    this.PlaceSingleWall(x, y2);
                  }
                  --num3;
                }
                else if (index == 0 && this.GetPixelClearance(x - 1, y2, wall_height, match) < wall_height && this.GetPixelClearance(x + 1, y2, wall_height, match) < wall_height)
                {
                  if (on_insufficient_wall_height != null)
                  {
                    on_insufficient_wall_height(x, y2);
                  }
                  else
                  {
                    this.SetPixelMap(x, y2, Microsoft.Xna.Framework.Color.White);
                    this.PlaceSingleWall(x, y2);
                  }
                  --num3;
                }
                else if (index == 1)
                {
                  this.heightMap[x + y2 * this.mapWidth] = num3 + 1;
                  if (num3 < wall_height || wall_height == 0)
                  {
                    if (wall_height > 0)
                      this.SetTile(this.buildingsLayer, x, y2, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants + num4, source_y + 1 + random_wall_variants + wall_height - num3 - 1));
                  }
                  else
                    this.SetTile(this.buildingsLayer, x, y2, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y));
                }
                if (num3 < wall_height)
                  ++num3;
              }
              num1 = y1;
              num2 = 0;
            }
            else
              ++num2;
          }
        }
      }
      List<Point> pointList = new List<Point>();
      for (int y = 0; y < this.mapHeight; ++y)
      {
        for (int x = 0; x < this.mapWidth; ++x)
        {
          int height1 = this.GetHeight(x, y, wall_height);
          int height2 = this.GetHeight(x - 1, y, wall_height);
          int height3 = this.GetHeight(x + 1, y, wall_height);
          int height4 = this.GetHeight(x, y - 1, wall_height);
          int num = this.generationRandom.Next(0, random_wall_variants);
          if (height3 < height1)
          {
            if (height3 == wall_height)
            {
              if (use_corner_hack)
              {
                pointList.Add(new Point(x, y));
                this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y));
              }
              else
                this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y + 1));
            }
            else
            {
              Layer layer = this.buildingsLayer;
              if (height3 >= 0)
              {
                this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants, source_y + 1 + random_wall_variants + wall_height - height3));
                layer = this.frontLayer;
              }
              if (height1 > wall_height)
                this.SetTile(layer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3 - 1, source_y + 1 + num));
              else
                this.SetTile(layer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 2 + num, source_y + 1 + random_wall_variants * 2 + 1 - height1 - 1));
              if (wall_height > 0 && y + 1 < this.mapHeight && height3 == -1 && this.GetHeight(x + 1, y + 1, wall_height) >= 0 && this.GetHeight(x, y + 1, wall_height) >= 0)
              {
                if (use_corner_hack)
                {
                  pointList.Add(new Point(x, y));
                  this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y));
                }
                else
                  this.SetTile(this.frontLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y + 2));
              }
            }
          }
          else if (height2 < height1)
          {
            if (height2 == wall_height)
            {
              if (use_corner_hack)
              {
                pointList.Add(new Point(x, y));
                this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y));
              }
              else
                this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3 + 1, source_y + 1));
            }
            else
            {
              Layer layer = this.buildingsLayer;
              if (height2 >= 0)
              {
                this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants, source_y + 1 + random_wall_variants + wall_height - height2));
                layer = this.frontLayer;
              }
              if (height1 > wall_height)
                this.SetTile(layer, x, y, VolcanoDungeon.GetTileIndex(source_x, source_y + 1 + num));
              else
                this.SetTile(layer, x, y, VolcanoDungeon.GetTileIndex(source_x + num, source_y + 1 + random_wall_variants * 2 + 1 - height1 - 1));
              if (wall_height > 0 && y + 1 < this.mapHeight && height2 == -1 && this.GetHeight(x - 1, y + 1, wall_height) >= 0 && this.GetHeight(x, y + 1, wall_height) >= 0)
              {
                if (use_corner_hack)
                {
                  pointList.Add(new Point(x, y));
                  this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y));
                }
                else
                  this.SetTile(this.frontLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3 + 1, source_y + 2));
              }
            }
          }
          if (height1 >= 0 && height4 == -1)
          {
            if (wall_height > 0)
            {
              if (height3 == -1)
                this.SetTile(this.frontLayer, x, y - 1, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 2 + num, source_y));
              else if (height2 == -1)
                this.SetTile(this.frontLayer, x, y - 1, VolcanoDungeon.GetTileIndex(source_x + num, source_y));
              else
                this.SetTile(this.frontLayer, x, y - 1, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants + num, source_y));
            }
            else if (height3 == -1)
              this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 2 + num, source_y));
            else if (height2 == -1)
              this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + num, source_y));
            else
              this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants + num, source_y));
          }
        }
      }
      if (use_corner_hack)
      {
        foreach (Point point in pointList)
        {
          if (this.GetHeight(point.X - 1, point.Y, wall_height) == -1)
            this.SetTile(this.frontLayer, point.X, point.Y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3 + 1, source_y + 2));
          else if (this.GetHeight(point.X + 1, point.Y, wall_height) == -1)
            this.SetTile(this.frontLayer, point.X, point.Y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y + 2));
          if (this.GetHeight(point.X - 1, point.Y, wall_height) == wall_height)
            this.SetTile(this.alwaysFrontLayer, point.X, point.Y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3 + 1, source_y + 1));
          else if (this.GetHeight(point.X + 1, point.Y, wall_height) == wall_height)
            this.SetTile(this.alwaysFrontLayer, point.X, point.Y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y + 1));
        }
      }
      this.heightMap = (int[]) null;
    }

    public int GetPixelClearance(int x, int y, int wall_height, Microsoft.Xna.Framework.Color match)
    {
      int num = 0;
      if (!(this.GetPixel(x, y, Microsoft.Xna.Framework.Color.White) == match))
        return 0;
      int pixelClearance = num + 1;
      for (int index = 1; index < wall_height && pixelClearance < wall_height; ++index)
      {
        if (y + index >= this.mapHeight)
          return wall_height;
        if (this.GetPixel(x, y + index, Microsoft.Xna.Framework.Color.White) == match)
          ++pixelClearance;
        else
          break;
      }
      for (int index = 1; index < wall_height && pixelClearance < wall_height; ++index)
      {
        if (y - index < 0)
          return wall_height;
        if (this.GetPixel(x, y - index, Microsoft.Xna.Framework.Color.White) == match)
          ++pixelClearance;
        else
          break;
      }
      return pixelClearance;
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      this.coolLavaEvent.Poll();
      this.lavaSoundsPlayedThisTick = 0;
      if ((int) (NetFieldBase<int, NetInt>) this.level == 0 && Game1.currentLocation == this)
      {
        this.steamTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
        if ((double) this.steamTimer < 0.0)
        {
          this.steamTimer = 5000f;
          Game1.playSound("cavedrip");
          this.temporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1), new Vector2(34.5f, 30.75f) * 64f, false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            texture = Game1.staminaRect,
            color = new Microsoft.Xna.Framework.Color(100, 150, (int) byte.MaxValue),
            alpha = 0.75f,
            motion = new Vector2(0.0f, 1f),
            acceleration = new Vector2(0.0f, 0.1f),
            interval = 99999f,
            layerDepth = 1f,
            scale = 8f,
            id = 89898f,
            yStopCoordinate = 2208,
            reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x =>
            {
              this.removeTemporarySpritesWithID(89898);
              Game1.playSound("steam");
              for (int index = 0; index < 4; ++index)
                this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(33.75f, 33.5f) * 64f + new Vector2((float) Game1.random.Next(64), (float) Game1.random.Next(64)), false, 0.007f, Microsoft.Xna.Framework.Color.White)
                {
                  alpha = 0.75f,
                  motion = new Vector2(0.0f, -1f),
                  acceleration = new Vector2(1f / 500f, 0.0f),
                  interval = 99999f,
                  layerDepth = 1f,
                  scale = 4f,
                  scaleChange = 0.02f,
                  rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0)
                });
            })
          });
        }
      }
      foreach (DwarfGate dwarfGate in this.dwarfGates)
        dwarfGate.UpdateWhenCurrentLocation(time, (GameLocation) this);
      if (this._sawFlameSprite || Utility.isThereAFarmerWithinDistance(new Vector2(30f, 38f), 3, (GameLocation) this) == null)
        return;
      Game1.addMailForTomorrow("Saw_Flame_Sprite_Volcano", true);
      TemporaryAnimatedSprite temporarySpriteById1 = this.getTemporarySpriteByID(999);
      if (temporarySpriteById1 != null)
      {
        temporarySpriteById1.yPeriodic = false;
        temporarySpriteById1.xPeriodic = false;
        temporarySpriteById1.sourceRect.Y = 0;
        temporarySpriteById1.sourceRectStartingPos.Y = 0.0f;
        temporarySpriteById1.motion = new Vector2(0.0f, -4f);
        temporarySpriteById1.acceleration = new Vector2(0.0f, -0.04f);
      }
      this.localSound("magma_sprite_spot");
      TemporaryAnimatedSprite temporarySpriteById2 = this.getTemporarySpriteByID(998);
      if (temporarySpriteById2 != null)
      {
        temporarySpriteById2.yPeriodic = false;
        temporarySpriteById2.xPeriodic = false;
        temporarySpriteById2.motion = new Vector2(0.0f, -4f);
        temporarySpriteById2.acceleration = new Vector2(0.0f, -0.04f);
      }
      this._sawFlameSprite = true;
    }

    public virtual void PlaceGroundTile(int x, int y)
    {
      if (this.generationRandom.NextDouble() < 0.300000011920929)
        this.SetTile(this.backLayer, x, y, VolcanoDungeon.GetTileIndex(1 + this.generationRandom.Next(0, 3), this.generationRandom.Next(0, 2)));
      else
        this.SetTile(this.backLayer, x, y, VolcanoDungeon.GetTileIndex(1, 0));
    }

    public override void drawFloorDecorations(SpriteBatch b)
    {
      base.drawFloorDecorations(b);
      Vector2 key = new Vector2();
      Point point = new Point();
      for (int index1 = Game1.viewport.Y / 64 - 1; index1 < (Game1.viewport.Y + Game1.viewport.Height) / 64 + 1; ++index1)
      {
        for (int index2 = Game1.viewport.X / 64 - 1; index2 < (Game1.viewport.X + Game1.viewport.Width) / 64 + 1; ++index2)
        {
          key.X = (float) index2;
          key.Y = (float) index1;
          if (this.localCooledLavaTiles.TryGetValue(key, out point))
          {
            point.X += 5;
            point.Y += 16;
            b.Draw(this.mapBaseTilesheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (index2 * 64), (float) (index1 * 64))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(point.X * 16, point.Y * 16, 16, 16)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.55f);
          }
        }
      }
    }

    public override void drawWaterTile(SpriteBatch b, int x, int y)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.level == 5)
        base.drawWaterTile(b, x, y);
      else if ((int) (NetFieldBase<int, NetInt>) this.level == 0 && x > 23 && x < 28 && y > 42 && y < 47)
      {
        this.drawWaterTile(b, x, y, Microsoft.Xna.Framework.Color.DeepSkyBlue * 0.8f);
      }
      else
      {
        int num1 = y == this.map.Layers[0].LayerHeight - 1 ? 1 : (!this.waterTiles[x, y + 1] ? 1 : 0);
        bool flag = y == 0 || !this.waterTiles[x, y - 1];
        int num2 = 0;
        int num3 = 320;
        b.Draw(this.mapBaseTilesheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - (!flag ? (int) this.waterPosition : 0)))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(num2 + this.waterAnimationIndex * 16, num3 + ((x + y) % 2 == 0 ? (this.waterTileFlip ? 32 : 0) : (this.waterTileFlip ? 0 : 32)) + (flag ? (int) this.waterPosition / 4 : 0), 16, 16 + (flag ? (int) -(double) this.waterPosition / 4 : 0))), (Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) this.waterColor, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.56f);
        if (num1 == 0)
          return;
        b.Draw(this.mapBaseTilesheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) ((y + 1) * 64 - (int) this.waterPosition))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(num2 + this.waterAnimationIndex * 16, num3 + ((x + (y + 1)) % 2 == 0 ? (this.waterTileFlip ? 32 : 0) : (this.waterTileFlip ? 0 : 32)), 16, 16 - (int) (16.0 - (double) this.waterPosition / 4.0) - 1)), (Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) this.waterColor, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.56f);
      }
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      foreach (DwarfGate dwarfGate in this.dwarfGates)
        dwarfGate.Draw(b);
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      base.drawAboveAlwaysFrontLayer(b);
      if (Game1.game1.takingMapScreenshot || (int) (NetFieldBase<int, NetInt>) this.level <= 0)
        return;
      int color = 2;
      string s = this.level?.ToString() ?? "";
      Microsoft.Xna.Framework.Rectangle titleSafeArea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
      SpriteText.drawString(b, s, titleSafeArea.Left + 16, titleSafeArea.Top + 16, layerDepth: 1f, drawBGScroll: 2, color: color);
    }

    public override void performTenMinuteUpdate(int timeOfDay)
    {
      base.performTenMinuteUpdate(timeOfDay);
      if (Game1.random.NextDouble() >= 0.1 || (int) (NetFieldBase<int, NetInt>) this.level <= 0 || (int) (NetFieldBase<int, NetInt>) this.level == 5)
        return;
      int num = 0;
      foreach (NPC character in this.characters)
      {
        if (character is Bat)
          ++num;
      }
      if (num >= this.farmers.Count * 4)
        return;
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
      this.playSound("magma_sprite_spot");
      NetCollection<NPC> characters = this.characters;
      Bat bat = new Bat(zero, (int) (NetFieldBase<int, NetInt>) this.level <= 5 || Game1.random.NextDouble() >= 0.5 ? -555 : -556);
      bat.focusedOnFarmers = true;
      characters.Add((NPC) bat);
    }

    public virtual void PlaceSingleWall(int x, int y)
    {
      int x1 = this.generationRandom.Next(0, 4);
      this.SetTile(this.frontLayer, x, y - 1, VolcanoDungeon.GetTileIndex(x1, 2));
      this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(x1, 3));
    }

    public virtual void ApplyPixels(
      string layout_texture_name,
      int source_x = 0,
      int source_y = 0,
      int width = 64,
      int height = 64,
      int x_offset = 0,
      int y_offset = 0,
      bool flip_x = false)
    {
      Texture2D texture2D = Game1.temporaryContent.Load<Texture2D>(layout_texture_name);
      Microsoft.Xna.Framework.Color[] pixels = new Microsoft.Xna.Framework.Color[width * height];
      Microsoft.Xna.Framework.Rectangle? rect = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(source_x, source_y, width, height));
      Microsoft.Xna.Framework.Color[] data = pixels;
      int elementCount = width * height;
      texture2D.GetData<Microsoft.Xna.Framework.Color>(0, rect, data, 0, elementCount);
      for (int x1 = 0; x1 < width; ++x1)
      {
        int x2 = x1 + x_offset;
        if (flip_x)
          x2 = x_offset + width - 1 - x1;
        if (x2 >= 0 && x2 < this.mapWidth)
        {
          for (int y1 = 0; y1 < height; ++y1)
          {
            int y2 = y1 + y_offset;
            if (y2 >= 0 && y2 < this.mapHeight)
            {
              Microsoft.Xna.Framework.Color pixelColor = this.GetPixelColor(width, height, pixels, x1, y1);
              this.SetPixelMap(x2, y2, pixelColor);
            }
          }
        }
      }
    }

    public int GetHeight(int x, int y, int max_height) => x < 0 || x >= this.mapWidth || y < 0 || y >= this.mapHeight ? max_height + 1 : this.heightMap[x + y * this.mapWidth];

    public Microsoft.Xna.Framework.Color GetPixel(int x, int y, Microsoft.Xna.Framework.Color out_of_bounds_color) => x < 0 || x >= this.mapWidth || y < 0 || y >= this.mapHeight ? out_of_bounds_color : this.pixelMap[x + y * this.mapWidth];

    public void SetPixelMap(int x, int y, Microsoft.Xna.Framework.Color color)
    {
      if (x < 0 || x >= this.mapWidth || y < 0 || y >= this.mapHeight)
        return;
      this.pixelMap[x + y * this.mapWidth] = color;
    }

    public int GetNeighborValue(int x, int y, Microsoft.Xna.Framework.Color matched_color, bool is_lava_pool = false)
    {
      int neighborValue = 0;
      if (this.GetPixel(x, y - 1, matched_color) == matched_color)
        ++neighborValue;
      if (this.GetPixel(x, y + 1, matched_color) == matched_color)
        neighborValue += 2;
      if (this.GetPixel(x + 1, y, matched_color) == matched_color)
        neighborValue += 4;
      if (this.GetPixel(x - 1, y, matched_color) == matched_color)
        neighborValue += 8;
      if (is_lava_pool && neighborValue == 15)
      {
        if (this.GetPixel(x - 1, y - 1, matched_color) == matched_color)
          neighborValue += 16;
        if (this.GetPixel(x + 1, y - 1, matched_color) == matched_color)
          neighborValue += 32;
      }
      return neighborValue;
    }

    public Microsoft.Xna.Framework.Color GetPixelColor(
      int width,
      int height,
      Microsoft.Xna.Framework.Color[] pixels,
      int x,
      int y)
    {
      if (x < 0 || x >= width || y < 0 || y >= height)
        return Microsoft.Xna.Framework.Color.Black;
      int index = x + y * width;
      return pixels[index];
    }

    public static int GetTileIndex(int x, int y) => x + y * 16;

    public void SetTile(Layer layer, int x, int y, int index)
    {
      if (x < 0 || x >= layer.LayerWidth || y < 0 || y >= layer.LayerHeight)
        return;
      Location location = new Location(x, y);
      layer.Tiles[location] = (Tile) new StaticTile(layer, this.map.TileSheets[0], BlendMode.Alpha, index);
    }

    public int GetMaxRoomLayouts() => 30;

    public static VolcanoDungeon GetLevel(string name, bool use_level_level_as_layout = false)
    {
      foreach (VolcanoDungeon activeLevel in VolcanoDungeon.activeLevels)
      {
        if (activeLevel.Name.Equals(name))
          return activeLevel;
      }
      VolcanoDungeon level = new VolcanoDungeon(Convert.ToInt32(name.Substring(nameof (VolcanoDungeon).Length)));
      VolcanoDungeon.activeLevels.Add(level);
      if (Game1.IsMasterGame)
        level.GenerateContents(use_level_level_as_layout);
      else
        level.reloadMap();
      return level;
    }

    public static void UpdateLevels(GameTime time)
    {
      foreach (VolcanoDungeon activeLevel in VolcanoDungeon.activeLevels)
      {
        if (activeLevel.farmers.Count<Farmer>() > 0)
          activeLevel.UpdateWhenCurrentLocation(time);
        activeLevel.updateEvenIfFarmerIsntHere(time, false);
      }
    }

    public static void UpdateLevels10Minutes(int timeOfDay)
    {
      VolcanoDungeon.ClearInactiveLevels();
      if (Game1.IsClient)
        return;
      foreach (VolcanoDungeon activeLevel in VolcanoDungeon.activeLevels)
      {
        if (activeLevel.farmers.Count<Farmer>() > 0)
          activeLevel.performTenMinuteUpdate(timeOfDay);
      }
    }

    private static void ClearInactiveLevels(bool keepUntickedLevels = true)
    {
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      if (Game1.locationRequest == null || Game1.locationRequest.Name.Contains("Volcano"))
        return;
      Game1.changeMusicTrack("none");
    }

    public static void ClearAllLevels() => VolcanoDungeon.activeLevels.RemoveAll((Predicate<VolcanoDungeon>) (level =>
    {
      level.CleanUp();
      level.mapContent.Dispose();
      return true;
    }));

    public virtual void CleanUp()
    {
      if (!Game1.IsMasterGame)
        return;
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

    public static void ForEach(Action<VolcanoDungeon> action)
    {
      foreach (VolcanoDungeon activeLevel in VolcanoDungeon.activeLevels)
        action(activeLevel);
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings") == 367)
      {
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Volcano_ShortcutOut"), this.createYesNoResponses(), "LeaveVolcano");
        return true;
      }
      if (this.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings") != 77)
        return base.checkAction(tileLocation, viewport, who);
      if (Game1.player.canUnderstandDwarves)
      {
        Random random = new Random((int) ((long) (Game1.stats.DaysPlayed + 898U) + (long) Game1.uniqueIDForThisGame));
        Dictionary<ISalable, int[]> dictionary = new Dictionary<ISalable, int[]>();
        dictionary.Add((ISalable) new Boots(853), new int[4]
        {
          0,
          int.MaxValue,
          848,
          100
        });
        Utility.AddStock(dictionary, (Item) new StardewValley.Object(Vector2.Zero, 286, int.MaxValue), 150);
        Utility.AddStock(dictionary, (Item) new StardewValley.Object(Vector2.Zero, 287, int.MaxValue), 300);
        Utility.AddStock(dictionary, (Item) new StardewValley.Object(Vector2.Zero, 288, int.MaxValue), 500);
        if (random.NextDouble() < 0.5)
          Utility.AddStock(dictionary, (Item) new StardewValley.Object(Vector2.Zero, 244, int.MaxValue), 600);
        else
          Utility.AddStock(dictionary, (Item) new StardewValley.Object(Vector2.Zero, 237, int.MaxValue), 600);
        if (random.NextDouble() < 0.25)
          Utility.AddStock(dictionary, (Item) new StardewValley.Objects.Hat(77), 5000);
        if (!Game1.player.craftingRecipes.ContainsKey("Warp Totem: Island"))
          Utility.AddStock(dictionary, (Item) new StardewValley.Object(886, 1, true), 5000);
        if (!Game1.player.cookingRecipes.ContainsKey("Ginger Ale"))
          Utility.AddStock(dictionary, (Item) new StardewValley.Object(903, 1, true), 500);
        Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(dictionary, who: "VolcanoShop", context: "VolcanoShop");
      }
      else
        Game1.player.doEmote(8);
      return true;
    }

    public override void performTouchAction(
      string full_action_string,
      Vector2 player_standing_position)
    {
      if (Game1.eventUp)
        return;
      if (full_action_string.Split(' ')[0] == "DwarfSwitch")
      {
        Point key = new Point((int) player_standing_position.X, (int) player_standing_position.Y);
        foreach (DwarfGate dwarfGate in this.dwarfGates)
        {
          if (dwarfGate.switches.ContainsKey(key) && !dwarfGate.switches[key])
            dwarfGate.pressEvent.Fire(key);
        }
      }
      base.performTouchAction(full_action_string, player_standing_position);
    }

    public enum TileNeighbors
    {
      N = 1,
      S = 2,
      E = 4,
      W = 8,
      NW = 16, // 0x00000010
      NE = 32, // 0x00000020
    }
  }
}
