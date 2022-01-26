// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandFarmCave
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class IslandFarmCave : IslandLocation
  {
    [XmlIgnore]
    public NPC gourmand;
    [XmlElement("gourmandRequestsFulfilled")]
    public NetInt gourmandRequestsFulfilled = new NetInt();
    [XmlIgnore]
    public NetEvent0 requestGourmandCheckEvent = new NetEvent0();
    [XmlIgnore]
    public NetEvent1Field<string, NetString> gourmandResponseEvent = new NetEvent1Field<string, NetString>();
    [XmlIgnore]
    public bool triggeredGourmand;
    [XmlIgnore]
    public static int TOTAL_GOURMAND_REQUESTS = 3;
    [XmlIgnore]
    private NetMutex gourmandMutex = new NetMutex();
    private Texture2D smokeTexture;
    private float smokeTimer;

    public IslandFarmCave()
    {
    }

    public IslandFarmCave(string map, string name)
      : base(map, name)
    {
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.gourmandRequestsFulfilled, (INetSerializable) this.requestGourmandCheckEvent, (INetSerializable) this.gourmandResponseEvent, (INetSerializable) this.gourmandMutex.NetFields);
      this.requestGourmandCheckEvent.onEvent += new NetEvent0.Event(this.OnRequestGourmandCheck);
      this.gourmandResponseEvent.onEvent += new AbstractNetEvent1<string>.Event(this.OnGourmandResponse);
    }

    public virtual void OnRequestGourmandCheck()
    {
      if (!Game1.IsMasterGame)
        return;
      string str = "";
      IslandWest locationFromName = Game1.getLocationFromName("IslandWest") as IslandWest;
      foreach (Vector2 key in locationFromName.terrainFeatures.Keys)
      {
        TerrainFeature terrainFeature = locationFromName.terrainFeatures[key];
        if (terrainFeature is HoeDirt)
        {
          HoeDirt hoeDirt = terrainFeature as HoeDirt;
          if (hoeDirt.crop != null)
          {
            bool flag = (int) (NetFieldBase<int, NetInt>) hoeDirt.crop.currentPhase >= hoeDirt.crop.phaseDays.Count - 1 && (!(bool) (NetFieldBase<bool, NetBool>) hoeDirt.crop.fullyGrown || (int) (NetFieldBase<int, NetInt>) hoeDirt.crop.dayOfCurrentPhase <= 0);
            if (hoeDirt.crop.indexOfHarvest.Value == this.IndexForRequest(this.gourmandRequestsFulfilled.Value))
            {
              if (flag)
              {
                Point destination = new Point((int) key.X, (int) key.Y);
                Point thatFitsCharacter1 = this.FindNearbyUnoccupiedTileThatFitsCharacter((GameLocation) locationFromName, destination.X, destination.Y);
                Point thatFitsCharacter2 = this.FindNearbyUnoccupiedTileThatFitsCharacter((GameLocation) locationFromName, destination.X, destination.Y, 2, new Point?(thatFitsCharacter1));
                int relativeDirection = this.GetRelativeDirection(thatFitsCharacter1, destination);
                this.gourmandResponseEvent.Fire(key.X.ToString() + " " + key.Y.ToString() + " " + thatFitsCharacter1.X.ToString() + " " + thatFitsCharacter1.Y.ToString() + " " + relativeDirection.ToString() + " " + thatFitsCharacter2.X.ToString() + " " + thatFitsCharacter2.Y.ToString() + " 2");
                return;
              }
              str = "inProgress";
            }
          }
        }
      }
      this.gourmandResponseEvent.Fire(str);
    }

    public int GetRelativeDirection(Point source, Point destination)
    {
      Point point = new Point(destination.X - source.X, destination.Y - source.Y);
      return Math.Abs(point.Y) > Math.Abs(point.X) ? (point.Y < 0 ? 0 : 2) : (point.X < 0 ? 3 : 1);
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
      if (Game1.random.NextDouble() >= 0.1)
        return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
      Game1.createItemDebris((Item) new Hat(78), new Vector2((float) ((double) bobberTile.X * 64.0 + 32.0), bobberTile.Y * 64f), 0, (GameLocation) this, who.getStandingY());
      return (StardewValley.Object) null;
    }

    public Point FindNearbyUnoccupiedTileThatFitsCharacter(
      GameLocation location,
      int target_x,
      int target_y,
      int width = 1,
      Point? invalid_tile = null)
    {
      HashSet<Point> pointSet = new HashSet<Point>();
      List<Point> pointList = new List<Point>();
      pointList.Add(new Point(target_x, target_y));
      pointSet.Add(new Point(target_x, target_y));
      Point[] pointArray = new Point[4]
      {
        new Point(-1, 0),
        new Point(1, 0),
        new Point(0, -1),
        new Point(0, 1)
      };
      for (int index1 = 0; index1 < 500 && pointList.Count != 0; ++index1)
      {
        Point thatFitsCharacter = pointList[0];
        pointList.RemoveAt(0);
        foreach (Point point1 in pointArray)
        {
          Point point2 = new Point(thatFitsCharacter.X + point1.X, thatFitsCharacter.Y + point1.Y);
          if (!pointSet.Contains(point2))
            pointList.Add(point2);
        }
        if (!pointSet.Contains(thatFitsCharacter) && (!invalid_tile.HasValue || thatFitsCharacter.X != invalid_tile.Value.X || thatFitsCharacter.Y != invalid_tile.Value.Y))
        {
          pointSet.Add(thatFitsCharacter);
          bool flag = false;
          int num = 1;
          for (int index2 = 0; index2 < width; ++index2)
          {
            for (int index3 = 0; index3 < num; ++index3)
            {
              Point point = new Point(thatFitsCharacter.X + index2, thatFitsCharacter.Y + index3);
              new Microsoft.Xna.Framework.Rectangle(point.X * 64, point.Y * 64, 64, 64).Inflate(-4, -4);
              if (point.X == target_x && point.Y == target_y + 1)
              {
                flag = true;
                break;
              }
              if (invalid_tile.HasValue && invalid_tile.Value == point)
              {
                flag = true;
                break;
              }
              if (!location.isTileLocationOpenIgnoreFrontLayers(new Location(point.X, point.Y)))
              {
                flag = true;
                break;
              }
              if (location.isObjectAtTile(point.X, point.Y))
              {
                flag = true;
                break;
              }
              if (location.isTerrainFeatureAt(point.X, point.Y))
              {
                flag = true;
                break;
              }
              if (!flag)
              {
                Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(point.X * 64, point.Y * 64, 64, 64);
                foreach (ResourceClump resourceClump in location.resourceClumps)
                {
                  if (resourceClump.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) resourceClump.tile).Intersects(rectangle))
                  {
                    flag = true;
                    break;
                  }
                }
              }
            }
          }
          if (!flag)
            return thatFitsCharacter;
        }
      }
      return new Point(target_x, target_y);
    }

    public virtual void OnGourmandResponse(string response)
    {
      if (Game1.currentLocation != this)
        return;
      if (response == "")
      {
        if (this.triggeredGourmand)
        {
          Game1.player.freezePause = 0;
          this.ShowGourmandUnhappy();
        }
      }
      else if (response == "inProgress")
      {
        Game1.player.freezePause = 0;
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Gourmand_InProgress"));
      }
      else
      {
        string[] strArray = response.Split(' ');
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("none/-1000 -1000/");
        stringBuilder.Append("farmer " + strArray[2] + " " + strArray[3] + " " + strArray[4] + "/");
        stringBuilder.Append("changeLocation IslandWest/");
        stringBuilder.Append("viewport " + strArray[0] + " " + strArray[1] + "/");
        stringBuilder.Append("playMusic none/addTemporaryActor Gourmand 32 32 " + strArray[5] + " " + strArray[6] + " " + strArray[7] + " true character/positionOffset Gourmand 0 1/positionOffset farmer 0 1/animate Gourmand false true 500 2 3/");
        stringBuilder.Append("viewport " + strArray[0] + " " + strArray[1] + " true/");
        stringBuilder.Append("pause 3000/playSound croak/");
        foreach (string str in Game1.content.LoadString("Strings\\Locations:Gourmand_Request_" + this.gourmandRequestsFulfilled.Value.ToString() + "_Success").Split('|'))
          stringBuilder.Append("message \"" + str + "\"/pause 250/");
        stringBuilder.Append("pause 1000/end");
        StardewValley.Event evt = new StardewValley.Event(stringBuilder.ToString());
        if (this.triggeredGourmand)
          evt.onEventFinished += (Action) (() =>
          {
            if (Game1.locationRequest != null)
              Game1.locationRequest.OnWarp += (LocationRequest.Callback) (() => this.CompleteGourmandRequest());
            else
              this.CompleteGourmandRequest();
          });
        Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => Game1.currentLocation.startEvent(evt)));
        Game1.player.freezePause = 0;
      }
      this.triggeredGourmand = false;
    }

    public virtual void CompleteGourmandRequest()
    {
      if (!this.gourmandMutex.IsLockHeld())
        return;
      Game1.player.freezePause = 1250;
      DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
      {
        Game1.playSound("croak");
        this.gourmand.shake(1000);
        Game1.afterDialogues += new Game1.afterFadeFunction(this.GiveReward);
        if (this.gourmandRequestsFulfilled.Value < IslandFarmCave.TOTAL_GOURMAND_REQUESTS - 1)
          Game1.multipleDialogues(Game1.content.LoadString("Strings\\Locations:Gourmand_Reward").Split('|'));
        else
          Game1.multipleDialogues(Game1.content.LoadString("Strings\\Locations:Gourmand_LastReward").Split('|'));
      }), 1000);
    }

    public virtual void GiveReward()
    {
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(4.5f, 4f) * 64f, 3, (GameLocation) this);
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(4.5f, 4f) * 64f, 1, (GameLocation) this);
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(4.5f, 4f) * 64f, 1, (GameLocation) this);
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(4.5f, 4f) * 64f, 1, (GameLocation) this);
      Game1.createItemDebris((Item) new StardewValley.Object(73, 1), new Vector2(4.5f, 4f) * 64f, 1, (GameLocation) this);
      ++this.gourmandRequestsFulfilled.Value;
      Game1.player.team.MarkCollectedNut("IslandGourmand" + this.gourmandRequestsFulfilled.Value.ToString());
      this.gourmandMutex.ReleaseLock();
    }

    public void ShowGourmandUnhappy()
    {
      Game1.playSound("croak");
      this.gourmand.shake(1000);
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Gourmand_RequestFailed"));
      if (!this.gourmandMutex.IsLockHeld())
        return;
      this.gourmandMutex.ReleaseLock();
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.gourmand = new NPC(new AnimatedSprite("Characters\\Gourmand", 0, 32, 32), new Vector2(4f, 4f) * 64f, nameof (IslandFarmCave), 2, "Gourmand", false, (Dictionary<int, int[]>) null, Game1.content.Load<Texture2D>("Portraits\\SafariGuy"));
      this.smokeTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
      this.waterColor.Value = new Color(10, 250, 120);
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (this.gourmand != null && !Game1.eventUp)
        this.gourmand.draw(b);
      if ((int) (NetFieldBase<int, NetInt>) this.gourmandRequestsFulfilled >= IslandFarmCave.TOTAL_GOURMAND_REQUESTS)
        return;
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) this.gourmand.getStandingX(), (float) (this.gourmand.getStandingY() - 128 - 8) + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(114, 53, 6, 10)), Color.White, 0.0f, new Vector2(1f, 4f), 4f, SpriteEffects.None, 1f);
    }

    public override void DayUpdate(int dayOfMonth)
    {
      this.gourmandMutex.ReleaseLock();
      base.DayUpdate(dayOfMonth);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      if (this.gourmand != null)
      {
        this.gourmand.update(time, (GameLocation) this);
        if (time.TotalGameTime.TotalMilliseconds % 1000.0 < 500.0)
          this.gourmand.Sprite.CurrentFrame = 1;
        else
          this.gourmand.Sprite.CurrentFrame = 0;
      }
      this.requestGourmandCheckEvent.Poll();
      this.gourmandResponseEvent.Poll();
      this.smokeTimer -= (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.smokeTimer > 0.0 || this.smokeTexture == null)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.smokeTexture,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 180, 9, 11),
        sourceRectStartingPos = new Vector2(0.0f, 180f),
        layerDepth = 1f,
        interval = 250f,
        position = new Vector2(2f, 4f) * 64f + new Vector2(5f, 5f) * 4f,
        scale = 4f,
        scaleChange = 0.005f,
        alpha = 0.75f,
        alphaFade = 0.005f,
        motion = new Vector2(0.0f, -0.5f),
        acceleration = new Vector2((float) (Game1.random.NextDouble() - 0.5) / 100f, 0.0f),
        animationLength = 3,
        holdLastFrame = true
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.smokeTexture,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 180, 9, 11),
        sourceRectStartingPos = new Vector2(0.0f, 180f),
        layerDepth = 1f,
        interval = 250f,
        position = new Vector2(7f, 4f) * 64f + new Vector2(5f, 5f) * 4f,
        scale = 4f,
        scaleChange = 0.005f,
        alpha = 0.75f,
        alphaFade = 0.005f,
        motion = new Vector2(0.0f, -0.5f),
        acceleration = new Vector2((float) (Game1.random.NextDouble() - 0.5) / 100f, 0.0f),
        animationLength = 3,
        holdLastFrame = true
      });
      this.smokeTimer = 1250f;
    }

    public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
    {
      base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
      this.gourmandMutex.Update(Game1.getOnlineFarmers());
    }

    public override bool isTileOccupiedForPlacement(Vector2 tileLocation, StardewValley.Object toPlace = null) => base.isTileOccupiedForPlacement(tileLocation, toPlace);

    public override void seasonUpdate(string season, bool onLoad = false)
    {
    }

    public override void updateSeasonalTileSheets(Map map = null)
    {
    }

    public virtual void TalkToGourmand()
    {
      List<string> stringList = new List<string>();
      if (this.gourmandRequestsFulfilled.Value >= IslandFarmCave.TOTAL_GOURMAND_REQUESTS)
      {
        stringList.AddRange((IEnumerable<string>) Game1.content.LoadString("Strings\\Locations:Gourmand_Finished").Split('|'));
      }
      else
      {
        if (!Game1.player.hasOrWillReceiveMail("talkedToGourmand"))
        {
          Game1.addMailForTomorrow("talkedToGourmand", true);
          stringList.Add(Game1.content.LoadString("Strings\\Locations:Gourmand_Intro"));
        }
        Game1.playSound("croak");
        this.gourmand.shake(1000);
        stringList.Add(Game1.content.LoadString("Strings\\Locations:Gourmand_RequestIntro"));
        stringList.Add(Game1.content.LoadString("Strings\\Locations:Gourmand_Request_" + this.gourmandRequestsFulfilled.Value.ToString()));
        Response[] responses = this.createYesNoResponses();
        Game1.afterDialogues = (Game1.afterFadeFunction) (() =>
        {
          Game1.afterDialogues = (Game1.afterFadeFunction) null;
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Gourmand_RequestQuestion"), responses, "Gourmand");
        });
      }
      Game1.multipleDialogues(stringList.ToArray());
    }

    public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
    {
      switch (questionAndAnswer)
      {
        case "Gourmand_Yes":
          this.triggeredGourmand = true;
          Game1.player.freezePause = 3000;
          this.requestGourmandCheckEvent.Fire();
          return true;
        case "Gourmand_No":
          this.ShowGourmandUnhappy();
          return true;
        case null:
          return false;
        default:
          return base.answerDialogueAction(questionAndAnswer, questionParams);
      }
    }

    public int IndexForRequest(int request_number)
    {
      switch (request_number)
      {
        case 0:
          return 254;
        case 1:
          return 262;
        case 2:
          return 248;
        default:
          return -1;
      }
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (!(action == "Gourmand"))
        return base.performAction(action, who, tileLocation);
      this.gourmandMutex.RequestLock((Action) (() => this.TalkToGourmand()));
      return true;
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      base.TransferDataFromSavedLocation(l);
      if (!(l is IslandFarmCave))
        return;
      this.gourmandRequestsFulfilled.Value = (l as IslandFarmCave).gourmandRequestsFulfilled.Value;
    }
  }
}
