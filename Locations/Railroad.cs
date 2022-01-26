// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Railroad
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class Railroad : GameLocation
  {
    public const int trainSoundDelay = 15000;
    [XmlIgnore]
    public readonly NetRef<Train> train = new NetRef<Train>();
    [XmlElement("hasTrainPassed")]
    private readonly NetBool hasTrainPassed = new NetBool(false);
    private int trainTime = -1;
    [XmlIgnore]
    public readonly NetInt trainTimer = new NetInt(0);
    public static ICue trainLoop;
    [XmlElement("witchStatueGone")]
    public readonly NetBool witchStatueGone = new NetBool(false);

    public Railroad()
    {
    }

    public Railroad(string map, string name)
      : base(map, name)
    {
    }

    public override void startEvent(StardewValley.Event evt)
    {
      if (evt != null && evt.id == 528052)
      {
        evt.eventPositionTileOffset.X -= 8f;
        evt.eventPositionTileOffset.Y -= 2f;
      }
      base.startEvent(evt);
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.train, (INetSerializable) this.hasTrainPassed, (INetSerializable) this.witchStatueGone, (INetSerializable) this.trainTimer);
      this.witchStatueGone.fieldChangeEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((field, oldValue, newValue) =>
      {
        if (!(!oldValue & newValue) || this.Map == null)
          return;
        DelayedAction.removeTileAfterDelay(54, 35, 2000, (GameLocation) this, "Buildings");
        DelayedAction.removeTileAfterDelay(54, 34, 2000, (GameLocation) this, "Front");
      });
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if ((bool) (NetFieldBase<bool, NetBool>) this.witchStatueGone || Game1.MasterPlayer.mailReceived.Contains("witchStatueGone"))
      {
        this.removeTile(54, 35, "Buildings");
        this.removeTile(54, 34, "Front");
      }
      if (!Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
        return;
      this.removeTile(24, 34, "Buildings");
      this.removeTile(25, 34, "Buildings");
      this.removeTile(24, 35, "Buildings");
      this.removeTile(25, 35, "Buildings");
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      if (Game1.getMusicTrackName().ToLower().Contains("ambient"))
        Game1.changeMusicTrack("none");
      if (Game1.IsWinter)
        return;
      AmbientLocationSounds.addSound(new Vector2(15f, 56f), 0);
    }

    public override void cleanupBeforePlayerExit()
    {
      base.cleanupBeforePlayerExit();
      if (Railroad.trainLoop != null)
        Railroad.trainLoop.Stop(AudioStopOptions.Immediate);
      Railroad.trainLoop = (ICue) null;
    }

    public override void checkForMusic(GameTime time)
    {
      if (Game1.timeOfDay < 1800 && !Game1.isRaining && !Game1.eventUp)
      {
        string currentSeason = Game1.currentSeason;
        if (!(currentSeason == "summer") && !(currentSeason == "fall") && !(currentSeason == "spring"))
          return;
        Game1.changeMusicTrack(Game1.currentSeason + "_day_ambient");
      }
      else
      {
        if (Game1.timeOfDay < 2000 || Game1.isRaining || Game1.eventUp)
          return;
        string currentSeason = Game1.currentSeason;
        if (!(currentSeason == "summer") && !(currentSeason == "fall") && !(currentSeason == "spring"))
          return;
        Game1.changeMusicTrack("spring_night_ambient");
      }
    }

    public override string checkForBuriedItem(
      int xLocation,
      int yLocation,
      bool explosion,
      bool detectOnly,
      Farmer who)
    {
      if (!who.secretNotesSeen.Contains(16) || xLocation != 12 || yLocation != 38 || who.mailReceived.Contains("SecretNote16_done"))
        return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
      who.mailReceived.Add("SecretNote16_done");
      Game1.createObjectDebris(166, xLocation, yLocation, who.UniqueMultiplayerID, (GameLocation) this);
      return "";
    }

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.map.GetLayer("Buildings").Tiles[tileLocation] == null || this.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex != 287)
        return base.checkAction(tileLocation, viewport, who);
      if (Game1.player.hasDarkTalisman)
      {
        Game1.player.freezePause = 7000;
        this.playSound("fireball");
        DelayedAction.playSoundAfterDelay("secret1", 2000);
        DelayedAction.removeTemporarySpriteAfterDelay((GameLocation) this, 9999f, 2000);
        this.witchStatueGone.Value = true;
        who.mailReceived.Add("witchStatueGone");
        for (int index = 0; index < 22; ++index)
          DelayedAction.playSoundAfterDelay("batFlap", 2220 + 240 * index);
        Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(576, 271, 28, 31), 60f, 3, 999, new Vector2(54f, 34f) * 64f + new Vector2(-2f, 1f) * 4f, false, false, 0.2176f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          xPeriodic = true,
          xPeriodicLoopTime = 8000f,
          xPeriodicRange = 384f,
          motion = new Vector2(-2f, 0.0f),
          acceleration = new Vector2(0.0f, -0.015f),
          pingPong = true,
          delayBeforeAnimationStart = 2000
        });
        Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 499, 10, 11), 50f, 7, 999, new Vector2(54f, 34f) * 64f + new Vector2(7f, 11f) * 4f, false, false, 0.2177f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          xPeriodic = true,
          xPeriodicLoopTime = 8000f,
          xPeriodicRange = 384f,
          motion = new Vector2(-2f, 0.0f),
          acceleration = new Vector2(0.0f, -0.015f),
          delayBeforeAnimationStart = 2000
        });
        Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 499, 10, 11), 35.715f, 7, 8, new Vector2(54f, 34f) * 64f + new Vector2(3f, 10f) * 4f, false, false, 0.2305f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          id = 9999f
        });
      }
      else
        Game1.drawObjectDialogue("???");
      return true;
    }

    public override void DayUpdate(int dayOfMonth)
    {
      base.DayUpdate(dayOfMonth);
      this.hasTrainPassed.Value = false;
      this.trainTime = -1;
      Random random = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed);
      if (random.NextDouble() >= 0.2 || !Game1.isLocationAccessible(nameof (Railroad)))
        return;
      this.trainTime = random.Next(900, 1800);
      this.trainTime -= this.trainTime % 10;
    }

    public override bool isCollidingPosition(
      Microsoft.Xna.Framework.Rectangle position,
      xTile.Dimensions.Rectangle viewport,
      bool isFarmer,
      int damagesFarmer,
      bool glider,
      Character character)
    {
      return !Game1.eventUp && this.train.Value != null && this.train.Value.getBoundingBox().Intersects(position) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
    }

    public void setTrainComing(int delay)
    {
      this.trainTimer.Value = delay;
      if (!Game1.IsMasterGame)
        return;
      this.PlayTrainApproach();
      Game1.multiplayer.sendServerToClientsMessage("trainApproach");
    }

    public void PlayTrainApproach()
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) Game1.currentLocation.isOutdoors || Game1.isFestival() || Game1.currentLocation.GetLocationContext() != GameLocation.LocationContext.Default)
        return;
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:Railroad_TrainComing"));
      if (Game1.soundBank == null)
        return;
      ICue cue = Game1.soundBank.GetCue("distantTrain");
      cue.SetVariable("Volume", 100f);
      cue.Play();
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
      if (Game1.player.secretNotesSeen.Contains(GameLocation.NECKLACE_SECRET_NOTE_INDEX) && !Game1.player.hasOrWillReceiveMail(GameLocation.CAROLINES_NECKLACE_MAIL))
      {
        Game1.player.mailForTomorrow.Add(GameLocation.CAROLINES_NECKLACE_MAIL + "%&NL&%");
        StardewValley.Object fish = new StardewValley.Object(GameLocation.CAROLINES_NECKLACE_ITEM, 1);
        Game1.player.addQuest(128);
        Game1.player.addQuest(129);
        return fish;
      }
      if (!who.mailReceived.Contains("gotSpaFishing"))
      {
        who.mailReceived.Add("gotSpaFishing");
        return (StardewValley.Object) new Furniture(2423, Vector2.Zero);
      }
      return Game1.random.NextDouble() < 0.08 ? (StardewValley.Object) new Furniture(2423, Vector2.Zero) : base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    }

    public override bool isTileFishable(int tileX, int tileY) => !Game1.currentSeason.Equals("winter") && base.isTileFishable(tileX, tileY);

    public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
    {
      base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
      if (this.train.Value != null && this.train.Value.Update(time, (GameLocation) this) && Game1.IsMasterGame)
        this.train.Value = (Train) null;
      if (!Game1.IsMasterGame)
        return;
      if (Game1.timeOfDay == this.trainTime - this.trainTime % 10 && (int) (NetFieldBase<int, NetInt>) this.trainTimer == 0 && !Game1.isFestival() && this.train.Value == null)
        this.setTrainComing(15000);
      if ((int) (NetFieldBase<int, NetInt>) this.trainTimer > 0)
      {
        this.trainTimer.Value -= time.ElapsedGameTime.Milliseconds;
        if ((int) (NetFieldBase<int, NetInt>) this.trainTimer <= 0)
        {
          this.train.Value = new Train();
          this.playSound("trainWhistle");
        }
        if ((int) (NetFieldBase<int, NetInt>) this.trainTimer < 3500 && Game1.currentLocation == this && Game1.soundBank != null && (Railroad.trainLoop == null || !Railroad.trainLoop.IsPlaying))
        {
          Railroad.trainLoop = Game1.soundBank.GetCue("trainLoop");
          Railroad.trainLoop.SetVariable("Volume", 0.0f);
          Railroad.trainLoop.Play();
        }
      }
      if (this.train.Value != null)
      {
        if (Game1.currentLocation == this && Game1.soundBank != null && (Railroad.trainLoop == null || !Railroad.trainLoop.IsPlaying))
        {
          Railroad.trainLoop = Game1.soundBank.GetCue("trainLoop");
          Railroad.trainLoop.SetVariable("Volume", 0.0f);
          Railroad.trainLoop.Play();
        }
        if (Railroad.trainLoop == null || (double) Railroad.trainLoop.GetVariable("Volume") >= 100.0)
          return;
        Railroad.trainLoop.SetVariable("Volume", Railroad.trainLoop.GetVariable("Volume") + 0.5f);
      }
      else if (Railroad.trainLoop != null && (int) (NetFieldBase<int, NetInt>) this.trainTimer <= 0)
      {
        Railroad.trainLoop.SetVariable("Volume", Railroad.trainLoop.GetVariable("Volume") - 0.15f);
        if ((double) Railroad.trainLoop.GetVariable("Volume") > 0.0)
          return;
        Railroad.trainLoop.Stop(AudioStopOptions.Immediate);
        Railroad.trainLoop = (ICue) null;
      }
      else
      {
        if ((int) (NetFieldBase<int, NetInt>) this.trainTimer <= 0 || Railroad.trainLoop == null)
          return;
        Railroad.trainLoop.SetVariable("Volume", Railroad.trainLoop.GetVariable("Volume") + 0.15f);
      }
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (this.train.Value == null || Game1.eventUp)
        return;
      this.train.Value.draw(b);
    }
  }
}
