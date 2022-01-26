// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.WitchEvent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using System;

namespace StardewValley.Events
{
  public class WitchEvent : FarmEvent, INetObject<NetFields>
  {
    public const int identifier = 942069;
    private Vector2 witchPosition;
    private Building targetBuilding;
    private Farm f;
    private Random r;
    private int witchFrame;
    private int witchAnimationTimer;
    private int animationLoopsDone;
    private int timerSinceFade;
    private bool animateLeft;
    private bool terminate;
    public bool goldenWitch;

    public NetFields NetFields { get; } = new NetFields();

    public bool setUp()
    {
      this.f = Game1.getLocationFromName("Farm") as Farm;
      this.r = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
      foreach (Building building in this.f.buildings)
      {
        if (building is Coop && !building.buildingType.Equals((object) "Coop") && !(building.indoors.Value as AnimalHouse).isFull() && building.indoors.Value.objects.Count() < 50 && this.r.NextDouble() < 0.8)
        {
          this.targetBuilding = building;
          if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") && this.r.NextDouble() < 0.6)
            this.goldenWitch = true;
        }
      }
      if (this.targetBuilding == null)
      {
        foreach (Building building in this.f.buildings)
        {
          if (building.buildingType.Equals((object) "Slime Hutch") && building.indoors.Value.characters.Count > 0 && this.r.NextDouble() < 0.5 && building.indoors.Value.numberOfObjectsOfType(83, true) == 0)
            this.targetBuilding = building;
        }
      }
      if (this.targetBuilding == null)
        return true;
      Game1.currentLightSources.Add(new LightSource(4, this.witchPosition, 2f, Color.Black, 942069));
      Game1.currentLocation = (GameLocation) this.f;
      this.f.resetForPlayerEntry();
      Game1.fadeClear();
      Game1.nonWarpFade = true;
      Game1.timeOfDay = 2400;
      Game1.ambientLight = new Color(200, 190, 40);
      Game1.displayHUD = false;
      Game1.freezeControls = true;
      Game1.viewportFreeze = true;
      Game1.displayFarmer = false;
      Game1.viewport.X = Math.Max(0, Math.Min(this.f.map.DisplayWidth - Game1.viewport.Width, (int) (NetFieldBase<int, NetInt>) this.targetBuilding.tileX * 64 - Game1.viewport.Width / 2));
      Game1.viewport.Y = Math.Max(0, Math.Min(this.f.map.DisplayHeight - Game1.viewport.Height, ((int) (NetFieldBase<int, NetInt>) this.targetBuilding.tileY - 3) * 64 - Game1.viewport.Height / 2));
      this.witchPosition = new Vector2((float) (Game1.viewport.X + Game1.viewport.Width + 128), (float) ((int) (NetFieldBase<int, NetInt>) this.targetBuilding.tileY * 64 - 64));
      Game1.changeMusicTrack("nightTime");
      DelayedAction.playSoundAfterDelay(this.goldenWitch ? "yoba" : "cacklingWitch", 3200);
      return false;
    }

    public bool tickUpdate(GameTime time)
    {
      if (this.terminate)
        return true;
      Game1.UpdateGameClock(time);
      this.f.UpdateWhenCurrentLocation(time);
      this.f.updateEvenIfFarmerIsntHere(time, false);
      Game1.UpdateOther(time);
      Utility.repositionLightSource(942069, this.witchPosition + new Vector2(32f, 32f));
      TimeSpan timeSpan;
      if (this.animationLoopsDone < 1)
      {
        int timerSinceFade = this.timerSinceFade;
        timeSpan = time.ElapsedGameTime;
        int milliseconds = timeSpan.Milliseconds;
        this.timerSinceFade = timerSinceFade + milliseconds;
      }
      if ((double) this.witchPosition.X > (double) ((int) (NetFieldBase<int, NetInt>) this.targetBuilding.tileX * 64 + 96))
      {
        if (this.timerSinceFade < 2000)
          return false;
        ref float local1 = ref this.witchPosition.X;
        double num1 = (double) local1;
        timeSpan = time.ElapsedGameTime;
        double num2 = (double) timeSpan.Milliseconds * 0.400000005960464;
        local1 = (float) (num1 - num2);
        ref float local2 = ref this.witchPosition.Y;
        double num3 = (double) local2;
        timeSpan = time.TotalGameTime;
        double num4 = Math.Cos((double) timeSpan.Milliseconds * Math.PI / 512.0) * 1.0;
        local2 = (float) (num3 + num4);
      }
      else if (this.animationLoopsDone < 4)
      {
        ref float local = ref this.witchPosition.Y;
        double num5 = (double) local;
        timeSpan = time.TotalGameTime;
        double num6 = Math.Cos((double) timeSpan.Milliseconds * Math.PI / 512.0) * 1.0;
        local = (float) (num5 + num6);
        int witchAnimationTimer = this.witchAnimationTimer;
        timeSpan = time.ElapsedGameTime;
        int milliseconds = timeSpan.Milliseconds;
        this.witchAnimationTimer = witchAnimationTimer + milliseconds;
        if (this.witchAnimationTimer > 2000)
        {
          this.witchAnimationTimer = 0;
          if (!this.animateLeft)
          {
            ++this.witchFrame;
            if (this.witchFrame == 1)
            {
              this.animateLeft = true;
              for (int index = 0; index < 75; ++index)
                this.f.temporarySprites.Add(new TemporaryAnimatedSprite(10, this.witchPosition + new Vector2(8f, 80f), this.goldenWitch ? (this.r.NextDouble() < 0.5 ? Color.Gold : new Color((int) byte.MaxValue, 150, 0)) : (this.r.NextDouble() < 0.5 ? Color.Lime : Color.DarkViolet))
                {
                  motion = new Vector2((float) this.r.Next(-100, 100) / 100f, 1.5f),
                  alphaFade = 0.015f,
                  delayBeforeAnimationStart = index * 30,
                  layerDepth = 1f
                });
              Game1.playSound(this.goldenWitch ? "discoverMineral" : "debuffSpell");
            }
          }
          else
          {
            --this.witchFrame;
            this.animationLoopsDone = 4;
            DelayedAction.playSoundAfterDelay(this.goldenWitch ? "yoba" : "cacklingWitch", 2500);
          }
        }
      }
      else
      {
        int witchAnimationTimer = this.witchAnimationTimer;
        timeSpan = time.ElapsedGameTime;
        int milliseconds1 = timeSpan.Milliseconds;
        this.witchAnimationTimer = witchAnimationTimer + milliseconds1;
        this.witchFrame = 0;
        if (this.witchAnimationTimer > 1000 && (double) this.witchPosition.X > -999999.0)
        {
          ref float local3 = ref this.witchPosition.Y;
          double num7 = (double) local3;
          timeSpan = time.TotalGameTime;
          double num8 = Math.Cos((double) timeSpan.Milliseconds * Math.PI / 256.0) * 2.0;
          local3 = (float) (num7 + num8);
          ref float local4 = ref this.witchPosition.X;
          double num9 = (double) local4;
          timeSpan = time.ElapsedGameTime;
          double num10 = (double) timeSpan.Milliseconds * 0.400000005960464;
          local4 = (float) (num9 - num10);
        }
        if ((double) this.witchPosition.X < (double) (Game1.viewport.X - 128) || float.IsNaN(this.witchPosition.X))
        {
          if (!Game1.fadeToBlack && (double) this.witchPosition.X != -999999.0)
          {
            Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.afterLastFade));
            Game1.changeMusicTrack("none");
            this.timerSinceFade = 0;
            this.witchPosition.X = -999999f;
          }
          int timerSinceFade = this.timerSinceFade;
          timeSpan = time.ElapsedGameTime;
          int milliseconds2 = timeSpan.Milliseconds;
          this.timerSinceFade = timerSinceFade + milliseconds2;
        }
      }
      return false;
    }

    public void afterLastFade()
    {
      this.terminate = true;
      Game1.globalFadeToClear();
    }

    public void draw(SpriteBatch b)
    {
      if (this.goldenWitch)
        b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, this.witchPosition), new Rectangle?(new Rectangle(215, 262 + this.witchFrame * 29, 34, 29)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9999999f);
      else
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.witchPosition), new Rectangle?(new Rectangle(277, 1886 + this.witchFrame * 29, 34, 29)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9999999f);
    }

    public void makeChangesToLocation()
    {
      if (!Game1.IsMasterGame)
        return;
      if (this.targetBuilding.buildingType.Equals((object) "Slime Hutch"))
      {
        foreach (NPC character in this.targetBuilding.indoors.Value.characters)
        {
          if (character is GreenSlime)
            (character as GreenSlime).color.Value = new Color(40 + this.r.Next(10), 40 + this.r.Next(10), 40 + this.r.Next(10));
        }
      }
      else
      {
        for (int index = 0; index < 200; ++index)
        {
          Vector2 vector2 = new Vector2((float) this.r.Next(2, this.targetBuilding.indoors.Value.Map.Layers[0].LayerWidth - 2), (float) this.r.Next(2, this.targetBuilding.indoors.Value.Map.Layers[0].LayerHeight - 2));
          if ((this.targetBuilding.indoors.Value.isTileLocationTotallyClearAndPlaceable(vector2) || this.targetBuilding.indoors.Value.terrainFeatures.ContainsKey(vector2) && this.targetBuilding.indoors.Value.terrainFeatures[vector2] is Flooring) && !this.targetBuilding.indoors.Value.objects.ContainsKey(vector2))
          {
            this.targetBuilding.indoors.Value.objects.Add(vector2, new StardewValley.Object(Vector2.Zero, this.goldenWitch ? 928 : 305, (string) null, false, true, false, true));
            break;
          }
        }
      }
    }

    public void drawAboveEverything(SpriteBatch b)
    {
    }
  }
}
