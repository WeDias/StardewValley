// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.WorldChangeEvent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;

namespace StardewValley.Events
{
  public class WorldChangeEvent : FarmEvent, INetObject<NetFields>
  {
    public const int identifier = 942066;
    public const int jojaGreenhouse = 0;
    public const int junimoGreenHouse = 1;
    public const int jojaBoiler = 2;
    public const int junimoBoiler = 3;
    public const int jojaBridge = 4;
    public const int junimoBridge = 5;
    public const int jojaBus = 6;
    public const int junimoBus = 7;
    public const int jojaBoulder = 8;
    public const int junimoBoulder = 9;
    public const int jojaMovieTheater = 10;
    public const int junimoMovieTheater = 11;
    public const int movieTheaterLightning = 12;
    public const int willyBoatRepair = 13;
    public const int treehouseBuild = 14;
    public readonly NetInt whichEvent = new NetInt();
    private int cutsceneLengthTimer;
    private int timerSinceFade;
    private int soundTimer;
    private int soundInterval = 99999;
    private GameLocation location;
    private string sound;
    private bool kill;
    private bool wasRaining;
    public GameLocation preEventLocation;

    public NetFields NetFields { get; } = new NetFields();

    public WorldChangeEvent() => this.NetFields.AddField((INetSerializable) this.whichEvent);

    public WorldChangeEvent(int which)
      : this()
    {
      this.whichEvent.Value = which;
    }

    private void obliterateJojaMartDoor()
    {
      (Game1.getLocationFromName("Town") as Town).crackOpenAbandonedJojaMartDoor();
      for (int index = 0; index < 16; ++index)
        Game1.getLocationFromName("Town").temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(96f, 50f) * 64f + new Vector2((float) Game1.random.Next(-32, 64), 0.0f), false, 1f / 500f, Color.Gray)
        {
          alpha = 0.75f,
          motion = new Vector2(0.0f, -0.5f) + new Vector2((float) (Game1.random.Next(100) - 50) / 100f, (float) (Game1.random.Next(100) - 50) / 100f),
          interval = 99999f,
          layerDepth = (float) (0.949999988079071 + (double) index * (1.0 / 1000.0)),
          scale = 3f,
          scaleChange = 0.01f,
          rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
          delayBeforeAnimationStart = index * 25
        });
      Utility.addDirtPuffs(Game1.getLocationFromName("Town"), 95, 49, 2, 2);
      Game1.getLocationFromName("Town").temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(96f, 50f) * 64f + new Vector2((float) Game1.random.Next(-32, 64), 0.0f), false, 0.0f, Color.Gray)
      {
        alpha = 0.01f,
        interval = 99999f,
        layerDepth = 0.9f,
        light = true,
        lightRadius = 4f,
        lightcolor = new Color(1, 1, 1)
      });
    }

    public bool setUp()
    {
      this.preEventLocation = Game1.currentLocation;
      Game1.currentLightSources.Clear();
      this.location = (GameLocation) null;
      int num1 = 0;
      int num2 = 0;
      this.cutsceneLengthTimer = 8000;
      this.wasRaining = Game1.isRaining;
      Game1.isRaining = false;
      Game1.changeMusicTrack("nightTime");
      switch ((int) (NetFieldBase<int, NetInt>) this.whichEvent)
      {
        case 0:
          this.location = Game1.getLocationFromName("Farm");
          this.location.resetForPlayerEntry();
          num1 = 28;
          num2 = 13;
          if (Game1.whichFarm == 5)
          {
            num1 = 39;
            num2 = 32;
          }
          if (this.location != null && this.location is Farm)
          {
            foreach (Building building in (this.location as Farm).buildings)
            {
              if (building is GreenhouseBuilding)
              {
                num1 = (int) (NetFieldBase<int, NetInt>) building.tileX + 3;
                num2 = (int) (NetFieldBase<int, NetInt>) building.tileY + 3;
                break;
              }
            }
          }
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1349, 19, 28), 150f, 5, 999, new Vector2((float) ((num1 - 3) * 64 + 8), (float) ((num2 - 1) * 64 - 32)), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1377, 19, 28), 140f, 5, 999, new Vector2((float) ((num1 + 3) * 64 - 16), (float) ((num2 - 2) * 64)), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32), 1000f, 2, 999, new Vector2((float) (num1 * 64 + 8), (float) ((num2 - 4) * 64)), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.soundInterval = 560;
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f));
          this.sound = "axchop";
          break;
        case 1:
          this.location = Game1.getLocationFromName("Farm");
          this.location.resetForPlayerEntry();
          num1 = 28;
          num2 = 13;
          if (Game1.whichFarm == 5)
          {
            num1 = 39;
            num2 = 32;
          }
          if (this.location != null && this.location is Farm)
          {
            foreach (Building building in (this.location as Farm).buildings)
            {
              if (building is GreenhouseBuilding)
              {
                num1 = (int) (NetFieldBase<int, NetInt>) building.tileX + 3;
                num2 = (int) (NetFieldBase<int, NetInt>) building.tileY + 3;
                break;
              }
            }
          }
          Utility.addSprinklesToLocation(this.location, num1, num2 - 1, 7, 7, 15000, 150, Color.LightCyan);
          Utility.addStarsAndSpirals(this.location, num1, num2 - 1, 7, 7, 15000, 150, Color.White);
          Game1.player.activeDialogueEvents.Add("cc_Greenhouse", 3);
          this.sound = "junimoMeep1";
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f, Color.DarkGoldenrod));
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2((float) (num1 * 64), (float) ((num2 - 1) * 64 - 64)), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          this.soundInterval = 800;
          break;
        case 2:
          this.location = Game1.getLocationFromName("Town");
          this.location.resetForPlayerEntry();
          num1 = 105;
          num2 = 79;
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1377, 19, 28), 100f, 5, 999, new Vector2(6656f, 5024f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1406, 22, 26), 700f, 2, 999, new Vector2(6888f, 5014f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32), 1500f, 2, 999, new Vector2(6792f, 4864f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(335, 1410, 21, 21), 999f, 1, 9999, new Vector2(6912f, 5136f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          Game1.player.activeDialogueEvents.Add("cc_Minecart", 7);
          this.soundInterval = 500;
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f));
          this.sound = "clank";
          break;
        case 3:
          this.location = Game1.getLocationFromName("Town");
          this.location.resetForPlayerEntry();
          num1 = 105;
          num2 = 79;
          Utility.addSprinklesToLocation(this.location, num1 + 1, num2, 6, 4, 15000, 350, Color.LightCyan);
          Utility.addStarsAndSpirals(this.location, num1 + 1, num2, 6, 4, 15000, 350, Color.White);
          Game1.player.activeDialogueEvents.Add("cc_Minecart", 7);
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(6656f, 5056f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(6912f, 5056f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2300f,
            xPeriodicRange = 16f,
            color = Color.HotPink,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          this.sound = "junimoMeep1";
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f, Color.DarkGoldenrod));
          this.soundInterval = 800;
          break;
        case 4:
          this.location = Game1.getLocationFromName("Mountain");
          this.location.resetForPlayerEntry();
          num1 = 95;
          num2 = 27;
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(383, 1378, 28, 27), 400f, 2, 999, new Vector2(5504f, 1632f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            motion = new Vector2(0.5f, 0.0f)
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1406, 22, 26), 350f, 2, 999, new Vector2(6272f, 1632f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(358, 1415, 31, 20), 999f, 1, 9999, new Vector2(5888f, 1648f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(335, 1410, 21, 21), 999f, 1, 9999, new Vector2(6400f, 1648f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32), 1500f, 2, 999, new Vector2(5824f, 1584f), false, false)
          {
            scale = 4f,
            layerDepth = 0.8f
          });
          Game1.player.activeDialogueEvents.Add("cc_Bridge", 7);
          this.soundInterval = 700;
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f));
          this.sound = "axchop";
          break;
        case 5:
          this.location = Game1.getLocationFromName("Mountain");
          this.location.resetForPlayerEntry();
          num1 = 95;
          num2 = 27;
          Utility.addSprinklesToLocation(this.location, num1, num2, 7, 4, 15000, 150, Color.LightCyan);
          Utility.addStarsAndSpirals(this.location, num1 + 1, num2, 7, 4, 15000, 350, Color.White);
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(5824f, 1648f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(6336f, 1648f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2300f,
            xPeriodicRange = 16f,
            color = Color.Yellow,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          Game1.player.activeDialogueEvents.Add("cc_Bridge", 7);
          this.sound = "junimoMeep1";
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f, Color.DarkGoldenrod));
          this.soundInterval = 800;
          break;
        case 6:
          this.location = Game1.getLocationFromName("BusStop");
          this.location.resetForPlayerEntry();
          num1 = 14;
          num2 = 8;
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1349, 19, 28), 150f, 5, 999, new Vector2(1216f, 480f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1377, 19, 28), 140f, 5, 999, new Vector2(640f, 512f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32), 1500f, 2, 999, new Vector2(904f, 192f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          Game1.player.activeDialogueEvents.Add("cc_Bus", 7);
          this.soundInterval = 560;
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f));
          this.sound = "clank";
          break;
        case 7:
          this.location = Game1.getLocationFromName("BusStop");
          this.location.resetForPlayerEntry();
          num1 = 14;
          num2 = 8;
          Utility.addSprinklesToLocation(this.location, num1, num2, 9, 4, 10000, 200, Color.LightCyan, motionTowardCenter: true);
          Utility.addStarsAndSpirals(this.location, num1, num2, 9, 4, 15000, 150, Color.White);
          this.sound = "junimoMeep1";
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(640f, 640f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(768f, 640f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2300f,
            xPeriodicRange = 16f,
            color = Color.Pink,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(896f, 640f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2200f,
            xPeriodicRange = 16f,
            color = Color.Yellow,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(1024f, 640f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2100f,
            xPeriodicRange = 16f,
            color = Color.LightBlue,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          Game1.player.activeDialogueEvents.Add("cc_Bus", 7);
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f, Color.DarkGoldenrod));
          this.soundInterval = 500;
          break;
        case 8:
          this.location = Game1.getLocationFromName("Mountain");
          this.location.resetForPlayerEntry();
          num1 = 48;
          num2 = 5;
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1377, 19, 28), 100f, 5, 999, new Vector2(2880f, 288f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(387, 1340, 17, 37), 50f, 2, 99999, new Vector2(3040f, 160f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            yPeriodic = true,
            yPeriodicLoopTime = 100f,
            yPeriodicRange = 2f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(335, 1410, 21, 21), 999f, 1, 9999, new Vector2(2816f, 368f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32), 1500f, 2, 999, new Vector2(3200f, 368f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          Game1.player.activeDialogueEvents.Add("cc_Boulder", 7);
          this.soundInterval = 100;
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f));
          this.sound = "thudStep";
          break;
        case 9:
          this.location = Game1.getLocationFromName("Mountain");
          this.location.resetForPlayerEntry();
          Game1.player.activeDialogueEvents.Add("cc_Boulder", 7);
          num1 = 48;
          num2 = 5;
          Utility.addSprinklesToLocation(this.location, num1, num2, 4, 4, 15000, 350, Color.LightCyan);
          Utility.addStarsAndSpirals(this.location, num1 + 1, num2, 4, 4, 15000, 550, Color.White);
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(2880f, 368f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(3200f, 368f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2300f,
            xPeriodicRange = 16f,
            color = Color.Yellow,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          this.sound = "junimoMeep1";
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 1f));
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 1f, Color.DarkCyan));
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f, Color.DarkGoldenrod));
          this.soundInterval = 1000;
          break;
        case 10:
          this.location = Game1.getLocationFromName("Town");
          this.location.resetForPlayerEntry();
          num1 = 52;
          num2 = 18;
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1349, 19, 28), 150f, 5, 999, new Vector2(3760f, 1056f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(288, 1377, 19, 28), 140f, 5, 999, new Vector2(2948f, 1088f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(390, 1405, 18, 32), 1000f, 2, 999, new Vector2(3144f, 1280f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          Game1.player.activeDialogueEvents.Add("movieTheater", 3);
          this.soundInterval = 560;
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f));
          this.sound = "axchop";
          break;
        case 11:
          this.location = Game1.getLocationFromName("Town");
          this.location.resetForPlayerEntry();
          num1 = 95;
          num2 = 48;
          Utility.addSprinklesToLocation(this.location, num1, num2, 7, 7, 15000, 150, Color.LightCyan);
          Utility.addStarsAndSpirals(this.location, num1, num2, 7, 7, 15000, 150, Color.White);
          Game1.player.activeDialogueEvents.Add("movieTheater", 3);
          this.sound = "junimoMeep1";
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f, Color.DarkGoldenrod));
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(6080f, 2880f), false, false)
          {
            scale = 4f,
            layerDepth = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Color.DarkGoldenrod,
            lightRadius = 1f
          });
          this.soundInterval = 800;
          break;
        case 12:
          this.cutsceneLengthTimer += 3000;
          Game1.isRaining = true;
          Game1.changeMusicTrack("rain");
          this.location = Game1.getLocationFromName("Town");
          this.location.resetForPlayerEntry();
          num1 = 95;
          num2 = 48;
          if (Game1.IsMasterGame)
            Game1.addMailForTomorrow("abandonedJojaMartAccessible", true);
          Rectangle sourceRect = new Rectangle(644, 1078, 37, 57);
          Vector2 vector2 = new Vector2(96f, 50f) * 64f;
          for (Vector2 position = vector2 + new Vector2((float) (-sourceRect.Width * 4 / 2), (float) (-sourceRect.Height * 4)); (double) position.Y > (double) (-sourceRect.Height * 4); position.Y -= (float) (sourceRect.Height * 4))
            this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 9999f, 1, 999, position, false, Game1.random.NextDouble() < 0.5, (float) (((double) vector2.Y + 32.0) / 10000.0 + 1.0 / 1000.0), 0.025f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              light = true,
              lightRadius = 2f,
              delayBeforeAnimationStart = 6200,
              lightcolor = Color.Black
            });
          DelayedAction.playSoundAfterDelay("thunder_small", 6000);
          DelayedAction.playSoundAfterDelay("boulderBreak", 6300);
          DelayedAction.screenFlashAfterDelay(1f, 6000);
          DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.obliterateJojaMartDoor), 6050);
          break;
        case 13:
          this.location = Game1.getLocationFromName("BoatTunnel");
          this.location.resetForPlayerEntry();
          num1 = 7;
          num2 = 7;
          if (Game1.IsMasterGame)
            Game1.addMailForTomorrow("willyBoatFixed", true);
          Game1.mailbox.Add("willyHours");
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Willy", new Rectangle(0, 320, 16, 32), 120f, 3, 999, new Vector2(412f, 332f), false, false)
          {
            pingPong = true,
            scale = 4f,
            layerDepth = 1f
          });
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Robin", new Rectangle(0, 192, 16, 32), 140f, 4, 999, new Vector2(704f, 256f), false, false)
          {
            scale = 4f,
            layerDepth = 1f
          });
          this.soundInterval = 560;
          this.sound = "crafting";
          break;
        case 14:
          this.location = Game1.getLocationFromName("Mountain");
          this.location.resetForPlayerEntry();
          num1 = 16;
          num2 = 7;
          this.cutsceneLengthTimer = 12000;
          Game1.currentLightSources.Add(new LightSource(4, new Vector2((float) num1, (float) num2) * 64f, 4f, Color.DarkGoldenrod));
          this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(0, 0, 24, 24), new Vector2(14f, 4.5f) * 64f, false, 0.0f, Color.White)
          {
            id = 777f,
            scale = 4f,
            totalNumberOfLoops = 99999,
            interval = 9999f,
            animationLength = 1,
            layerDepth = 1f,
            drawAboveAlwaysFront = true
          });
          DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.ParrotSquawk), 2000);
          for (int index = 0; index < 16; ++index)
          {
            Rectangle r = new Rectangle(15, 5, 3, 3);
            TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(49 + 16 * Game1.random.Next(3), 229, 16, 6), Utility.getRandomPositionInThisRectangle(r, Game1.random) * 64f, Game1.random.NextDouble() < 0.5, 0.0f, Color.White)
            {
              motion = new Vector2((float) Game1.random.Next(-2, 3), -16f),
              acceleration = new Vector2(0.0f, 0.5f),
              rotationChange = (float) Game1.random.Next(-4, 5) * 0.05f,
              scale = 4f,
              animationLength = 1,
              totalNumberOfLoops = 1,
              interval = (float) (1000 + Game1.random.Next(500)),
              layerDepth = 1f,
              drawAboveAlwaysFront = true,
              yStopCoordinate = (r.Bottom + 1) * 64,
              delayBeforeAnimationStart = 4000 + index * 250
            };
            temporaryAnimatedSprite1.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSprite1.bounce);
            this.location.TemporarySprites.Add(temporaryAnimatedSprite1);
            TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(49 + 16 * Game1.random.Next(3), 229, 16, 6), Utility.getRandomPositionInThisRectangle(r, Game1.random) * 64f, Game1.random.NextDouble() < 0.5, 0.0f, Color.White)
            {
              motion = new Vector2((float) Game1.random.Next(-2, 3), -16f),
              acceleration = new Vector2(0.0f, 0.5f),
              rotationChange = (float) Game1.random.Next(-4, 5) * 0.05f,
              scale = 4f,
              animationLength = 1,
              totalNumberOfLoops = 1,
              interval = (float) (1000 + Game1.random.Next(500)),
              layerDepth = 1f,
              drawAboveAlwaysFront = true,
              delayBeforeAnimationStart = 4500 + index * 250,
              yStopCoordinate = (r.Bottom + 1) * 64
            };
            temporaryAnimatedSprite2.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSprite2.bounce);
            this.location.TemporarySprites.Add(temporaryAnimatedSprite2);
          }
          for (int index = 0; index < 20; ++index)
          {
            Vector2 position = new Vector2(Utility.RandomFloat(13f, 19f), 0.0f) * 64f;
            float num3 = 1024f - position.X;
            TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(48 + Game1.random.Next(2) * 72, Game1.random.Next(2) * 48, 24, 24), position, false, 0.0f, Color.White)
            {
              motion = new Vector2(num3 * 0.01f, 10f),
              acceleration = new Vector2(0.0f, -0.05f),
              id = 778f,
              scale = 4f,
              yStopCoordinate = 448,
              totalNumberOfLoops = 99999,
              interval = 50f,
              animationLength = 3,
              flipped = (double) num3 > 0.0,
              layerDepth = 1f,
              drawAboveAlwaysFront = true,
              delayBeforeAnimationStart = 3500 + index * 250,
              alpha = 0.0f,
              alphaFade = -0.1f
            };
            DelayedAction.playSoundAfterDelay("batFlap", 3500 + index * 250);
            temporaryAnimatedSprite.reachedStopCoordinateSprite = new Action<TemporaryAnimatedSprite>(this.ParrotBounce);
            this.location.temporarySprites.Add(temporaryAnimatedSprite);
          }
          DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.FinishTreehouse), 8000);
          DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.ParrotSquawk), 9000);
          DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.ParrotFlyAway), 11000);
          break;
      }
      this.soundTimer = this.soundInterval;
      Game1.currentLocation = this.location;
      Game1.fadeClear();
      Game1.nonWarpFade = true;
      Game1.timeOfDay = 2400;
      Game1.displayHUD = false;
      Game1.viewportFreeze = true;
      Game1.player.position.X = -999999f;
      Game1.viewport.X = Math.Max(0, Math.Min(this.location.map.DisplayWidth - Game1.viewport.Width, num1 * 64 - Game1.viewport.Width / 2));
      Game1.viewport.Y = Math.Max(0, Math.Min(this.location.map.DisplayHeight - Game1.viewport.Height, num2 * 64 - Game1.viewport.Height / 2));
      if (!this.location.IsOutdoors)
      {
        Game1.viewport.X = num1 * 64 - Game1.viewport.Width / 2;
        Game1.viewport.Y = num2 * 64 - Game1.viewport.Height / 2;
      }
      Game1.previousViewportPosition = new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
      if (Game1.debrisWeather != null && Game1.debrisWeather.Count > 0)
        Game1.randomizeDebrisWeatherPositions(Game1.debrisWeather);
      Game1.randomizeRainPositions();
      return false;
    }

    public virtual void ParrotFlyAway()
    {
      this.location.removeTemporarySpritesWithIDLocal(777f);
      this.location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(48, 0, 24, 24), new Vector2(14f, 4.5f) * 64f, false, 0.0f, Color.White)
      {
        id = 777f,
        scale = 4f,
        totalNumberOfLoops = 99999,
        layerDepth = 1f,
        drawAboveAlwaysFront = true,
        interval = 50f,
        animationLength = 3,
        motion = new Vector2(-2f, 0.0f),
        acceleration = new Vector2(0.0f, -0.1f)
      });
    }

    public virtual void ParrotSquawk()
    {
      TemporaryAnimatedSprite temporarySpriteById = this.location.getTemporarySpriteByID(777);
      temporarySpriteById.shakeIntensity = 1f;
      temporarySpriteById.sourceRectStartingPos.X = 24f;
      temporarySpriteById.sourceRect.X = 24;
      Game1.playSound("parrot");
      DelayedAction.functionAfterDelay(new DelayedAction.delayedBehavior(this.ParrotStopSquawk), 500);
    }

    public virtual void ParrotStopSquawk()
    {
      TemporaryAnimatedSprite temporarySpriteById = this.location.getTemporarySpriteByID(777);
      temporarySpriteById.shakeIntensity = 0.0f;
      temporarySpriteById.sourceRectStartingPos.X = 0.0f;
      temporarySpriteById.sourceRect.X = 0;
    }

    public virtual void FinishTreehouse()
    {
      Game1.flashAlpha = 1f;
      Game1.playSound("yoba");
      Game1.playSound("axchop");
      (this.location as Mountain).ApplyTreehouseIfNecessary();
      this.location.removeTemporarySpritesWithIDLocal(778f);
      for (int index = 0; index < 20; ++index)
      {
        Vector2 position = new Vector2(Utility.RandomFloat(13f, 19f), Utility.RandomFloat(4f, 7f)) * 64f;
        float num = 1024f - position.X;
        this.location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Rectangle(192, Game1.random.Next(2) * 48, 24, 24), position, false, 0.0f, Color.White)
        {
          motion = new Vector2(num * -0.01f, Utility.RandomFloat(-2f, 0.0f)),
          acceleration = new Vector2(0.0f, -0.05f),
          id = 778f,
          scale = 4f,
          totalNumberOfLoops = 99999,
          interval = 50f,
          animationLength = 3,
          flipped = (double) num > 0.0,
          layerDepth = 1f,
          drawAboveAlwaysFront = true
        });
      }
    }

    public void ParrotBounce(TemporaryAnimatedSprite sprite)
    {
      float num = 1024f - sprite.Position.X;
      sprite.motion.X = (float) Math.Sign(num) * Utility.RandomFloat(0.5f, 4f);
      sprite.motion.Y = Utility.RandomFloat(-15f, -10f);
      sprite.acceleration.Y = 0.5f;
      sprite.yStopCoordinate = 448;
      sprite.flipped = (double) num > 0.0;
      sprite.sourceRectStartingPos.X = (float) (48 + Game1.random.Next(2) * 72);
      if (Game1.random.NextDouble() < 0.0500000007450581)
        Game1.playSound("axe");
      else if (Game1.random.NextDouble() < 0.0500000007450581)
        Game1.playSound("crafting");
      else
        Game1.playSound("dirtyHit");
    }

    public bool tickUpdate(GameTime time)
    {
      Game1.UpdateGameClock(time);
      this.location.updateWater(time);
      this.cutsceneLengthTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.timerSinceFade > 0)
      {
        this.timerSinceFade -= time.ElapsedGameTime.Milliseconds;
        Game1.globalFade = true;
        Game1.fadeToBlackAlpha = 1f;
        return this.timerSinceFade <= 0;
      }
      if (this.cutsceneLengthTimer <= 0 && !Game1.globalFade)
        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.endEvent), 0.01f);
      this.soundTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.soundTimer <= 0 && this.sound != null)
      {
        Game1.playSound(this.sound);
        this.soundTimer = this.soundInterval;
      }
      return false;
    }

    public void endEvent()
    {
      if (this.preEventLocation != null)
      {
        Game1.currentLocation = this.preEventLocation;
        Game1.currentLocation.resetForPlayerEntry();
        this.preEventLocation = (GameLocation) null;
      }
      Game1.changeMusicTrack("none");
      this.timerSinceFade = 1500;
      Game1.isRaining = this.wasRaining;
      Game1.getFarm().temporarySprites.Clear();
    }

    public void draw(SpriteBatch b)
    {
    }

    public void makeChangesToLocation()
    {
    }

    public void drawAboveEverything(SpriteBatch b)
    {
    }
  }
}
