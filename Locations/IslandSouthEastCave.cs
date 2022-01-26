// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandSouthEastCave
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Objects;
using System;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;

namespace StardewValley.Locations
{
  public class IslandSouthEastCave : IslandLocation
  {
    protected PerchingBirds _parrots;
    protected Texture2D _parrotTextures;
    public NetLongList drinksClaimed = new NetLongList();
    [XmlIgnore]
    public bool wasPirateCaveOnLoad;
    private float smokeTimer;

    public IslandSouthEastCave()
    {
    }

    public IslandSouthEastCave(string map, string name)
      : base(map, name)
    {
    }

    protected override void initNetFields()
    {
      this.NetFields.AddField((INetSerializable) this.drinksClaimed);
      base.initNetFields();
    }

    public override void updateMap()
    {
      if (IslandSouthEastCave.isPirateNight())
        this.mapPath.Value = "Maps\\IslandSouthEastCave_pirates";
      else
        this.mapPath.Value = "Maps\\IslandSouthEastCave";
      base.updateMap();
    }

    public override void MakeMapModifications(bool force = false)
    {
      base.MakeMapModifications(force);
      if (!IslandSouthEastCave.isPirateNight())
        return;
      this.setTileProperty(19, 9, "Buildings", "Action", "MessageSpeech Pirates1");
      this.setTileProperty(20, 9, "Buildings", "Action", "MessageSpeech Pirates2");
      this.setTileProperty(26, 17, "Buildings", "Action", "MessageSpeech Pirates3");
      this.setTileProperty(23, 8, "Buildings", "Action", "MessageSpeech Pirates4");
      this.setTileProperty(27, 5, "Buildings", "Action", "MessageSpeech Pirates5");
      this.setTileProperty(32, 6, "Buildings", "Action", "MessageSpeech Pirates6");
      this.setTileProperty(30, 8, "Buildings", "Action", "DartsGame");
      this.setTileProperty(33, 8, "Buildings", "Action", "Bartender");
    }

    protected override void resetLocalState()
    {
      this.wasPirateCaveOnLoad = IslandSouthEastCave.isPirateNight();
      base.resetLocalState();
      if (IslandSouthEastCave.isPirateNight())
      {
        this.addFlame(new Vector2(25.6f, 5.7f), 0.0f);
        this.addFlame(new Vector2(18f, 11f) + new Vector2(0.2f, -0.05f));
        this.addFlame(new Vector2(22f, 11f) + new Vector2(0.2f, -0.05f));
        this.addFlame(new Vector2(23f, 16f) + new Vector2(0.2f, -0.05f));
        this.addFlame(new Vector2(19f, 27f) + new Vector2(0.2f, -0.05f));
        this.addFlame(new Vector2(33f, 10f) + new Vector2(0.2f, -0.05f));
        this.addFlame(new Vector2(21f, 22f) + new Vector2(0.2f, -0.05f));
        this._parrotTextures = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\parrots");
        this._parrots = new PerchingBirds(this._parrotTextures, 3, 24, 24, new Vector2(12f, 19f), new Point[5]
        {
          new Point(12, 2),
          new Point(35, 6),
          new Point(25, 14),
          new Point(28, 1),
          new Point(27, 12)
        }, new Point[0]);
        this._parrots.peckDuration = 0;
        for (int index = 0; index < 3; ++index)
          this._parrots.AddBird(Game1.random.Next(0, 4));
        Game1.changeMusicTrack("PIRATE_THEME", true, Game1.MusicContext.SubLocation);
      }
      if (!(Game1.currentSeason == "winter"))
        return;
      this.addMoonlightJellies(40, new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame - 24917), new Microsoft.Xna.Framework.Rectangle(0, 0, 30, 15));
    }

    public static bool isWearingPirateClothes(Farmer who)
    {
      if (who.hat.Value != null)
      {
        switch ((int) (NetFieldBase<int, NetInt>) who.hat.Value.which)
        {
          case 24:
          case 62:
          case 76:
            return true;
        }
      }
      return false;
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (action != null && who.IsLocalPlayer)
      {
        string str = action.Split(' ')[0];
        if (!(str == "Bartender"))
        {
          if (str == "DartsGame")
          {
            string question;
            switch (Game1.player.team.GetDroppedLimitedNutCount("Darts"))
            {
              case 0:
                question = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_0");
                break;
              case 1:
                question = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_1");
                break;
              case 2:
                question = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_2");
                break;
              default:
                question = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_3");
                break;
            }
            this.createQuestionDialogue(question, this.createYesNoResponses(), "DartsGame");
          }
        }
        else if (IslandSouthEastCave.isWearingPirateClothes(who))
        {
          if (this.drinksClaimed.Contains(Game1.player.UniqueMultiplayerID))
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:PirateBartender_PirateClothes_NoMore"));
          }
          else
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:PirateBartender_PirateClothes"));
            Game1.afterDialogues = (Game1.afterFadeFunction) (() => who.addItemByMenuIfNecessary((Item) new StardewValley.Object(459, 1), (ItemGrabMenu.behaviorOnItemSelect) ((x, y) => this.drinksClaimed.Add(Game1.player.UniqueMultiplayerID))));
          }
        }
        else
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:Pirates8"));
      }
      return base.performAction(action, who, tileLocation);
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
      return Game1.random.NextDouble() < 0.05 ? (StardewValley.Object) new Furniture(2332, Vector2.Zero) : base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
    }

    public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
    {
      switch (questionAndAnswer)
      {
        case "DartsGame_Yes":
          int droppedLimitedNutCount = Game1.player.team.GetDroppedLimitedNutCount("Darts");
          int dart_count = 20;
          switch (droppedLimitedNutCount)
          {
            case 1:
              dart_count = 15;
              break;
            case 2:
              dart_count = 10;
              break;
          }
          Game1.currentMinigame = (IMinigame) new Darts(dart_count);
          return true;
        case null:
          return false;
        default:
          return base.answerDialogueAction(questionAndAnswer, questionParams);
      }
    }

    public override void cleanupBeforePlayerExit()
    {
      this._parrots = (PerchingBirds) null;
      this._parrotTextures = (Texture2D) null;
      base.cleanupBeforePlayerExit();
    }

    private void addFlame(Vector2 tileLocation, float sort_offset_tiles = 2.25f) => this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), tileLocation * 64f, false, 0.0f, Color.White)
    {
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 4,
      light = true,
      lightRadius = 2f,
      scale = 4f,
      layerDepth = (float) (((double) tileLocation.Y + (double) sort_offset_tiles) * 64.0 / 10000.0)
    });

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      if (this._parrots != null)
        this._parrots.Draw(b);
      base.drawAboveAlwaysFrontLayer(b);
    }

    public override void DayUpdate(int dayOfMonth)
    {
      this.drinksClaimed.Clear();
      base.DayUpdate(dayOfMonth);
    }

    public override void SetBuriedNutLocations()
    {
      base.SetBuriedNutLocations();
      this.buriedNutPoints.Add(new Point(36, 26));
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      base.UpdateWhenCurrentLocation(time);
      if (!IslandSouthEastCave.isPirateNight())
        return;
      if (Game1.currentLocation == this && !this.wasPirateCaveOnLoad && Game1.locationRequest == null && Game1.activeClickableMenu == null && Game1.currentMinigame == null && Game1.CurrentEvent == null)
      {
        Game1.player.completelyStopAnimatingOrDoingAction();
        Game1.warpFarmer("IslandSouthEast", 29, 19, 1);
      }
      if (this._parrots != null)
        this._parrots.Update(time);
      this.smokeTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.smokeTimer > 0.0)
        return;
      Utility.addSmokePuff((GameLocation) this, new Vector2(25.6f, 5.7f) * 64f);
      Utility.addSmokePuff((GameLocation) this, new Vector2(34f, 7.2f) * 64f);
      this.smokeTimer = 1000f;
    }

    public static bool isPirateNight() => !Game1.IsRainingHere() && Game1.timeOfDay >= 2000 && Game1.dayOfMonth % 2 == 0;

    public override bool isTileOccupiedForPlacement(Vector2 tileLocation, StardewValley.Object toPlace = null) => base.isTileOccupiedForPlacement(tileLocation, toPlace);

    public override void seasonUpdate(string season, bool onLoad = false)
    {
    }

    public override void updateSeasonalTileSheets(Map map = null)
    {
    }

    public override void TransferDataFromSavedLocation(GameLocation l)
    {
      base.TransferDataFromSavedLocation(l);
      if (!(l is IslandSouthEastCave))
        return;
      IslandSouthEastCave islandSouthEastCave = l as IslandSouthEastCave;
      this.drinksClaimed.Clear();
      foreach (long num in (NetList<long, NetLong>) islandSouthEastCave.drinksClaimed)
        this.drinksClaimed.Add(num);
    }
  }
}
