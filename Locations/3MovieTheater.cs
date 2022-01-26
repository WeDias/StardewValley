// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.MovieTheater
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Events;
using StardewValley.GameData.Movies;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;

namespace StardewValley.Locations
{
  public class MovieTheater : GameLocation
  {
    protected bool _startedMovie;
    protected bool _isJojaTheater;
    protected static Dictionary<string, MovieData> _movieData;
    protected static List<MovieCharacterReaction> _genericReactions;
    protected static List<ConcessionTaste> _concessionTastes;
    protected readonly NetStringDictionary<int, NetInt> _spawnedMoviePatrons = new NetStringDictionary<int, NetInt>();
    protected readonly NetStringDictionary<int, NetInt> _purchasedConcessions = new NetStringDictionary<int, NetInt>();
    protected readonly NetStringDictionary<int, NetInt> _playerInvitedPatrons = new NetStringDictionary<int, NetInt>();
    protected readonly NetStringDictionary<bool, NetBool> _characterGroupLookup = new NetStringDictionary<bool, NetBool>();
    protected Dictionary<int, List<Point>> _hangoutPoints;
    protected Dictionary<int, List<Point>> _availableHangoutPoints;
    protected int _maxHangoutGroups;
    protected int _movieStartTime = -1;
    [XmlElement("dayFirstEntered")]
    public readonly NetInt dayFirstEntered = new NetInt(-1);
    protected static Dictionary<int, MovieConcession> _concessions;
    public const int LOVE_MOVIE_FRIENDSHIP = 200;
    public const int LIKE_MOVIE_FRIENDSHIP = 100;
    public const int DISLIKE_MOVIE_FRIENDSHIP = 0;
    public const int LOVE_CONCESSION_FRIENDSHIP = 50;
    public const int LIKE_CONCESSION_FRIENDSHIP = 25;
    public const int DISLIKE_CONCESSION_FRIENDSHIP = 0;
    public const int OPEN_TIME = 900;
    public const int CLOSE_TIME = 2100;
    public int nextRepathTime;
    public int repathTimeInterval = 1000;
    [XmlIgnore]
    protected Dictionary<string, KeyValuePair<Point, int>> _destinationPositions = new Dictionary<string, KeyValuePair<Point, int>>();
    [XmlIgnore]
    public PerchingBirds birds;
    protected int _exitX;
    protected int _exitY;
    private NetEvent1<MovieViewerLockEvent> movieViewerLockEvent = new NetEvent1<MovieViewerLockEvent>();
    private NetEvent1<StartMovieEvent> startMovieEvent = new NetEvent1<StartMovieEvent>();
    private NetEvent1Field<long, NetLong> requestStartMovieEvent = new NetEvent1Field<long, NetLong>();
    private NetEvent1Field<long, NetLong> endMovieEvent = new NetEvent1Field<long, NetLong>();
    protected List<Farmer> _viewingFarmers = new List<Farmer>();
    protected List<List<Character>> _viewingGroups = new List<List<Character>>();
    protected List<List<Character>> _playerGroups = new List<List<Character>>();
    protected List<List<Character>> _npcGroups = new List<List<Character>>();
    protected static bool _hasRequestedMovieStart = false;
    protected static int _playerHangoutGroup = -1;
    protected int _farmerCount;
    protected NetInt _currentState = new NetInt();
    public static string[][][][] possibleNPCGroups = new string[7][][][]
    {
      new string[3][][]
      {
        new string[1][]{ new string[1]{ "Lewis" } },
        new string[3][]
        {
          new string[3]{ "Jas", "Vincent", "Marnie" },
          new string[3]{ "Abigail", "Sebastian", "Sam" },
          new string[2]{ "Penny", "Maru" }
        },
        new string[1][]{ new string[2]{ "Lewis", "Marnie" } }
      },
      new string[3][][]
      {
        new string[3][]
        {
          new string[1]{ "Clint" },
          new string[2]{ "Demetrius", "Robin" },
          new string[1]{ "Lewis" }
        },
        new string[2][]
        {
          new string[2]{ "Caroline", "Jodi" },
          new string[3]{ "Abigail", "Sebastian", "Sam" }
        },
        new string[2][]
        {
          new string[1]{ "Lewis" },
          new string[3]{ "Abigail", "Sebastian", "Sam" }
        }
      },
      new string[3][][]
      {
        new string[2][]
        {
          new string[2]{ "Evelyn", "George" },
          new string[1]{ "Lewis" }
        },
        new string[2][]
        {
          new string[2]{ "Penny", "Pam" },
          new string[3]{ "Abigail", "Sebastian", "Sam" }
        },
        new string[2][]
        {
          new string[2]{ "Sandy", "Emily" },
          new string[1]{ "Elliot" }
        }
      },
      new string[3][][]
      {
        new string[3][]
        {
          new string[2]{ "Penny", "Pam" },
          new string[3]{ "Abigail", "Sebastian", "Sam" },
          new string[1]{ "Lewis" }
        },
        new string[2][]
        {
          new string[3]{ "Alex", "Haley", "Emily" },
          new string[3]{ "Abigail", "Sebastian", "Sam" }
        },
        new string[2][]
        {
          new string[2]{ "Pierre", "Caroline" },
          new string[3]{ "Shane", "Jas", "Marnie" }
        }
      },
      new string[3][][]
      {
        null,
        new string[3][]
        {
          new string[2]{ "Haley", "Emily" },
          new string[3]{ "Abigail", "Sebastian", "Sam" },
          new string[1]{ "Lewis" }
        },
        new string[2][]
        {
          new string[2]{ "Penny", "Pam" },
          new string[3]{ "Abigail", "Sebastian", "Sam" }
        }
      },
      new string[3][][]
      {
        new string[1][]{ new string[1]{ "Lewis" } },
        new string[2][]
        {
          new string[2]{ "Penny", "Pam" },
          new string[3]{ "Abigail", "Sebastian", "Sam" }
        },
        new string[2][]
        {
          new string[3]{ "Harvey", "Maru", "Penny" },
          new string[1]{ "Leah" }
        }
      },
      new string[3][][]
      {
        new string[3][]
        {
          new string[2]{ "Penny", "Pam" },
          new string[3]{ "George", "Evelyn", "Alex" },
          new string[1]{ "Lewis" }
        },
        new string[2][]
        {
          new string[2]{ "Gus", "Willy" },
          new string[2]{ "Maru", "Sebastian" }
        },
        new string[2][]
        {
          new string[2]{ "Penny", "Pam" },
          new string[2]{ "Sandy", "Emily" }
        }
      }
    };

    public MovieTheater()
    {
    }

    public static void AddMoviePoster(GameLocation location, float x, float y, int month_offset = 0)
    {
      WorldDate date = new WorldDate(Game1.Date);
      date.TotalDays += 28 * month_offset;
      MovieData movieForDate = MovieTheater.GetMovieForDate(date);
      if (movieForDate == null)
        return;
      location.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Movies"),
        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, movieForDate.SheetIndex * 128, 13, 19),
        sourceRectStartingPos = new Vector2(0.0f, (float) (movieForDate.SheetIndex * 128)),
        animationLength = 1,
        totalNumberOfLoops = 9999,
        interval = 9999f,
        scale = 4f,
        position = new Vector2(x, y),
        layerDepth = 0.01f
      });
    }

    public MovieTheater(string map, string name)
      : base(map, name)
    {
      this._currentState.Set(0);
      MovieTheater.GetMovieData();
      this._InitializeMap();
      MovieTheater.GetMovieReactions();
    }

    public static List<MovieCharacterReaction> GetMovieReactions()
    {
      if (MovieTheater._genericReactions == null)
        MovieTheater._genericReactions = Game1.content.Load<List<MovieCharacterReaction>>("Data\\MoviesReactions");
      return MovieTheater._genericReactions;
    }

    public static string GetConcessionTasteForCharacter(
      Character character,
      MovieConcession concession)
    {
      if (MovieTheater._concessionTastes == null)
        MovieTheater._concessionTastes = Game1.content.Load<List<ConcessionTaste>>("Data\\ConcessionTastes");
      ConcessionTaste concessionTaste1 = (ConcessionTaste) null;
      foreach (ConcessionTaste concessionTaste2 in MovieTheater._concessionTastes)
      {
        if (concessionTaste2.Name == "*")
        {
          concessionTaste1 = concessionTaste2;
          break;
        }
      }
      foreach (ConcessionTaste concessionTaste3 in MovieTheater._concessionTastes)
      {
        if (concessionTaste3.Name == character.Name)
        {
          if (concessionTaste3.LovedTags.Contains(concession.Name))
            return "love";
          if (concessionTaste3.LikedTags.Contains(concession.Name))
            return "like";
          if (concessionTaste3.DislikedTags.Contains(concession.Name))
            return "dislike";
          if (concessionTaste1 != null)
          {
            if (concessionTaste1.LovedTags.Contains(concession.Name))
              return "love";
            if (concessionTaste1.LikedTags.Contains(concession.Name))
              return "like";
            if (concessionTaste1.DislikedTags.Contains(concession.Name))
              return "dislike";
          }
          if (concession.tags != null)
          {
            using (List<string>.Enumerator enumerator = concession.tags.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                string current = enumerator.Current;
                if (concessionTaste3.LovedTags.Contains(current))
                  return "love";
                if (concessionTaste3.LikedTags.Contains(current))
                  return "like";
                if (concessionTaste3.DislikedTags.Contains(current))
                  return "dislike";
                if (concessionTaste1 != null)
                {
                  if (concessionTaste1.LovedTags.Contains(current))
                    return "love";
                  if (concessionTaste1.LikedTags.Contains(current))
                    return "like";
                  if (concessionTaste1.DislikedTags.Contains(current))
                    return "dislike";
                }
              }
              break;
            }
          }
          else
            break;
        }
      }
      return "like";
    }

    public static IEnumerable<string> GetPatronNames()
    {
      if (!(Game1.getLocationFromName(nameof (MovieTheater)) is MovieTheater locationFromName))
        return (IEnumerable<string>) null;
      return locationFromName._spawnedMoviePatrons == null ? (IEnumerable<string>) null : (IEnumerable<string>) locationFromName._spawnedMoviePatrons.Keys;
    }

    protected void _InitializeMap()
    {
      this._hangoutPoints = new Dictionary<int, List<Point>>();
      this._maxHangoutGroups = 0;
      if (this.map.GetLayer("Paths") != null)
      {
        Layer layer = this.map.GetLayer("Paths");
        for (int x = 0; x < layer.LayerWidth; ++x)
        {
          for (int y = 0; y < layer.LayerHeight; ++y)
          {
            if (layer.Tiles[x, y] != null && layer.Tiles[x, y].TileIndex == 7)
            {
              int result = -1;
              if (this.map.GetLayer("Paths").Tiles[x, y].Properties.ContainsKey("group") && int.TryParse((string) this.map.GetLayer("Paths").Tiles[x, y].Properties["group"], out result))
              {
                if (!this._hangoutPoints.ContainsKey(result))
                  this._hangoutPoints[result] = new List<Point>();
                this._hangoutPoints[result].Add(new Point(x, y));
                this._maxHangoutGroups = Math.Max(this._maxHangoutGroups, result);
              }
            }
          }
        }
      }
      this.ResetTheater();
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this._spawnedMoviePatrons, (INetSerializable) this._purchasedConcessions, (INetSerializable) this._currentState, (INetSerializable) this.movieViewerLockEvent, (INetSerializable) this.requestStartMovieEvent, (INetSerializable) this.startMovieEvent, (INetSerializable) this.endMovieEvent, (INetSerializable) this._playerInvitedPatrons, (INetSerializable) this._characterGroupLookup, (INetSerializable) this.dayFirstEntered);
      this.movieViewerLockEvent.onEvent += new AbstractNetEvent1<MovieViewerLockEvent>.Event(this.OnMovieViewerLockEvent);
      this.requestStartMovieEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnRequestStartMovieEvent);
      this.startMovieEvent.onEvent += new AbstractNetEvent1<StartMovieEvent>.Event(this.OnStartMovieEvent);
    }

    public void OnStartMovieEvent(StartMovieEvent e)
    {
      if (e.uid != Game1.player.UniqueMultiplayerID)
        return;
      if (Game1.activeClickableMenu is ReadyCheckDialog)
        (Game1.activeClickableMenu as ReadyCheckDialog).closeDialog(Game1.player);
      StardewValley.Event viewing_event = new MovieTheaterScreeningEvent().getMovieEvent(MovieTheater.GetMovieForDate(Game1.Date).ID, e.playerGroups, e.npcGroups, this.GetConcessionsDictionary());
      Rumble.rumble(0.15f, 200f);
      Game1.player.completelyStopAnimatingOrDoingAction();
      this.playSoundAt("doorClose", Game1.player.getTileLocation());
      Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
      {
        Game1.changeMusicTrack("none");
        this.startEvent(viewing_event);
      }));
    }

    public void OnRequestStartMovieEvent(long uid)
    {
      if (!Game1.IsMasterGame)
        return;
      if (this._currentState.Value == 0)
      {
        if (Game1.player.team.movieMutex.IsLocked())
          Game1.player.team.movieMutex.ReleaseLock();
        Game1.player.team.movieMutex.RequestLock();
        this._playerGroups = new List<List<Character>>();
        this._npcGroups = new List<List<Character>>();
        List<Character> collection = new List<Character>();
        foreach (string patronName in MovieTheater.GetPatronNames())
        {
          Character characterFromName = (Character) Game1.getCharacterFromName(patronName);
          collection.Add(characterFromName);
        }
        foreach (Farmer viewingFarmer in this._viewingFarmers)
        {
          List<Character> characterList = new List<Character>();
          characterList.Add((Character) viewingFarmer);
          for (int index = 0; index < Game1.player.team.movieInvitations.Count; ++index)
          {
            MovieInvitation movieInvitation = Game1.player.team.movieInvitations[index];
            if (movieInvitation.farmer == viewingFarmer && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == viewingFarmer && collection.Contains((Character) movieInvitation.invitedNPC))
            {
              collection.Remove((Character) movieInvitation.invitedNPC);
              characterList.Add((Character) movieInvitation.invitedNPC);
            }
          }
          this._playerGroups.Add(characterList);
        }
        foreach (List<Character> playerGroup in this._playerGroups)
        {
          foreach (Character character in playerGroup)
          {
            if (character is NPC)
              (character as NPC).lastSeenMovieWeek.Set(Game1.Date.TotalWeeks);
          }
        }
        this._npcGroups.Add(new List<Character>((IEnumerable<Character>) collection));
        this._PopulateNPCOnlyGroups(this._playerGroups, this._npcGroups);
        this._viewingGroups = new List<List<Character>>();
        List<Character> characterList1 = new List<Character>();
        foreach (List<Character> playerGroup in this._playerGroups)
        {
          foreach (Character character in playerGroup)
            characterList1.Add(character);
        }
        this._viewingGroups.Add(characterList1);
        foreach (IEnumerable<Character> npcGroup in this._npcGroups)
          this._viewingGroups.Add(new List<Character>(npcGroup));
        this._currentState.Set(1);
      }
      this.startMovieEvent.Fire(new StartMovieEvent(uid, this._playerGroups, this._npcGroups));
    }

    public void OnMovieViewerLockEvent(MovieViewerLockEvent e)
    {
      this._viewingFarmers = new List<Farmer>();
      this._movieStartTime = e.movieStartTime;
      foreach (long uid in e.uids)
      {
        Farmer farmer = Game1.getFarmer(uid);
        if (farmer != null)
          this._viewingFarmers.Add(farmer);
      }
      if (this._viewingFarmers.Count > 0 && Game1.IsMultiplayer)
        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\UI:MovieStartRequest"));
      if (!Game1.player.team.movieMutex.IsLockHeld())
        return;
      this._ShowMovieStartReady();
    }

    public void _ShowMovieStartReady()
    {
      if (!Game1.IsMultiplayer)
      {
        this.requestStartMovieEvent.Fire(Game1.player.UniqueMultiplayerID);
      }
      else
      {
        Game1.player.team.SetLocalRequiredFarmers("start_movie", (IEnumerable<Farmer>) this._viewingFarmers);
        Game1.player.team.SetLocalReady("start_movie", true);
        Game1.dialogueUp = false;
        MovieTheater._hasRequestedMovieStart = true;
        Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog("start_movie", true, (ConfirmationDialog.behavior) (farmer =>
        {
          if (!MovieTheater._hasRequestedMovieStart)
            return;
          MovieTheater._hasRequestedMovieStart = false;
          this.requestStartMovieEvent.Fire(farmer.UniqueMultiplayerID);
        }), (ConfirmationDialog.behavior) (farmer =>
        {
          if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ReadyCheckDialog)
            (Game1.activeClickableMenu as ReadyCheckDialog).closeDialog(farmer);
          if (!Game1.player.team.movieMutex.IsLockHeld())
            return;
          Game1.player.team.movieMutex.ReleaseLock();
        }));
      }
    }

    public static Dictionary<string, MovieData> GetMovieData()
    {
      if (MovieTheater._movieData == null)
      {
        MovieTheater._movieData = Game1.content.Load<Dictionary<string, MovieData>>("Data\\Movies");
        foreach (KeyValuePair<string, MovieData> keyValuePair in MovieTheater._movieData)
          keyValuePair.Value.ID = keyValuePair.Key;
      }
      return MovieTheater._movieData;
    }

    public NPC GetMoviePatron(string name)
    {
      for (int index = 0; index < this.characters.Count; ++index)
      {
        if ((string) (NetFieldBase<string, NetString>) this.characters[index].name == name)
          return this.characters[index];
      }
      return (NPC) null;
    }

    protected NPC AddMoviePatronNPC(string name, int x, int y, int facingDirection)
    {
      if (this._spawnedMoviePatrons.ContainsKey(name))
        return this.GetMoviePatron(name);
      string str = name.Equals("Krobus") ? "Krobus_Trenchcoat" : NPC.getTextureNameForCharacter(name);
      string syncedPortraitPath = "Portraits\\" + NPC.getTextureNameForCharacter(name);
      int num = name.Contains("Dwarf") || name.Equals("Krobus") ? 96 : 128;
      NPC character = new NPC(new AnimatedSprite("Characters\\" + str, 0, 16, num / 4), new Vector2((float) (x * 64), (float) (y * 64)), this.Name, facingDirection, name, (Dictionary<int, int[]>) null, (Texture2D) null, true, syncedPortraitPath);
      character.eventActor = true;
      character.collidesWithOtherCharacters.Set(false);
      this.addCharacter(character);
      this._spawnedMoviePatrons.Add(name, 1);
      this.GetDialogueForCharacter(character);
      return character;
    }

    public void RemoveAllPatrons()
    {
      if (this._spawnedMoviePatrons == null)
        return;
      for (int index = 0; index < this.characters.Count; ++index)
      {
        if (this._spawnedMoviePatrons.ContainsKey(this.characters[index].Name))
        {
          this.characters.RemoveAt(index);
          --index;
        }
      }
      this._spawnedMoviePatrons.Clear();
    }

    protected override void resetSharedState()
    {
      base.resetSharedState();
      if (this._currentState.Value != 0)
        return;
      MovieData movieForDate = MovieTheater.GetMovieForDate(Game1.Date);
      Game1.multiplayer.globalChatInfoMessage("MovieStart", movieForDate.Title);
    }

    protected override void resetLocalState()
    {
      base.resetLocalState();
      this.birds = new PerchingBirds(Game1.birdsSpriteSheet, 2, 16, 16, new Vector2(8f, 14f), new Point[14]
      {
        new Point(19, 5),
        new Point(21, 4),
        new Point(16, 3),
        new Point(10, 13),
        new Point(2, 13),
        new Point(2, 6),
        new Point(9, 2),
        new Point(18, 12),
        new Point(21, 11),
        new Point(3, 11),
        new Point(4, 2),
        new Point(12, 12),
        new Point(11, 5),
        new Point(13, 13)
      }, new Point[6]
      {
        new Point(19, 5),
        new Point(21, 4),
        new Point(16, 3),
        new Point(9, 2),
        new Point(21, 11),
        new Point(4, 2)
      });
      if (!this._isJojaTheater && Game1.MasterPlayer.mailReceived.Contains("ccMovieTheaterJoja"))
        this._isJojaTheater = true;
      if (this.dayFirstEntered.Value == -1)
        this.dayFirstEntered.Value = Game1.Date.TotalDays;
      if (!this._isJojaTheater)
      {
        this.birds.roosting = this._currentState.Value == 2;
        for (int index = 0; index < Game1.random.Next(2, 5); ++index)
        {
          int bird_type = Game1.random.Next(0, 4);
          if (Game1.currentSeason == "fall")
            bird_type = 10;
          this.birds.AddBird(bird_type);
        }
        if (Game1.timeOfDay > 2100 && Game1.random.NextDouble() < 0.5)
          this.birds.AddBird(11);
      }
      MovieTheater.AddMoviePoster((GameLocation) this, 1104f, 292f);
      this.loadMap((string) (NetFieldBase<string, NetString>) this.mapPath, true);
      if (this._isJojaTheater)
      {
        this.Map.TileSheets[0].ImageSource = "Maps\\MovieTheaterJoja_TileSheet" + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? "" : "_international");
        this.Map.LoadTileSheets(Game1.mapDisplayDevice);
      }
      if (this._currentState.Value == 0)
      {
        this.addRandomNPCs();
      }
      else
      {
        if (this._currentState.Value != 2)
          return;
        Game1.changeMusicTrack("movieTheaterAfter");
        Game1.ambientLight = new Color(150, 170, 80);
        this.addSpecificRandomNPC(0);
      }
    }

    private void addRandomNPCs()
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + Game1.Date.TotalDays);
      this.critters = new List<Critter>();
      if (this.dayFirstEntered.Value == Game1.Date.TotalDays || random.NextDouble() < 0.25)
        this.addSpecificRandomNPC(0);
      if (!this._isJojaTheater && random.NextDouble() < 0.28)
      {
        this.addSpecificRandomNPC(4);
        this.addSpecificRandomNPC(11);
      }
      else if (this._isJojaTheater && random.NextDouble() < 0.33)
        this.addSpecificRandomNPC(13);
      if (random.NextDouble() < 0.1)
      {
        this.addSpecificRandomNPC(9);
        this.addSpecificRandomNPC(7);
      }
      if (Game1.currentSeason.Equals("fall") && random.NextDouble() < 0.5)
        this.addSpecificRandomNPC(1);
      if (Game1.currentSeason.Equals("spring") && random.NextDouble() < 0.5)
        this.addSpecificRandomNPC(3);
      if (random.NextDouble() < 0.25)
        this.addSpecificRandomNPC(2);
      if (random.NextDouble() < 0.25)
        this.addSpecificRandomNPC(6);
      if (random.NextDouble() < 0.25)
        this.addSpecificRandomNPC(8);
      if (random.NextDouble() < 0.2)
        this.addSpecificRandomNPC(10);
      if (random.NextDouble() < 0.2)
        this.addSpecificRandomNPC(12);
      if (random.NextDouble() < 0.2)
        this.addSpecificRandomNPC(5);
      if (this._isJojaTheater)
        return;
      if (random.NextDouble() < 0.75)
        this.addCritter((Critter) new Butterfly(new Vector2(13f, 7f)).setStayInbounds(true));
      if (random.NextDouble() < 0.75)
        this.addCritter((Critter) new Butterfly(new Vector2(4f, 8f)).setStayInbounds(true));
      if (random.NextDouble() >= 0.75)
        return;
      this.addCritter((Critter) new Butterfly(new Vector2(17f, 10f)).setStayInbounds(true));
    }

    private void addSpecificRandomNPC(int whichRandomNPC)
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + Game1.Date.TotalDays + whichRandomNPC);
      switch (whichRandomNPC)
      {
        case 0:
          this.setMapTile(2, 9, 215, "Buildings", "MessageSpeech MovieTheater_CraneMan" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setMapTile(2, 8, 199, "Front", (string) null);
          break;
        case 1:
          this.setMapTile(19, 7, 216, "Buildings", "MessageSpeech MovieTheater_Welwick" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setMapTile(19, 6, 200, "Front", (string) null);
          break;
        case 2:
          this.setAnimatedMapTile(21, 7, new int[4]
          {
            217,
            217,
            217,
            218
          }, 700L, "Buildings", "MessageSpeech MovieTheater_ShortsMan" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setAnimatedMapTile(21, 6, new int[4]
          {
            201,
            201,
            201,
            202
          }, 700L, "Front", (string) null);
          break;
        case 3:
          this.setMapTile(5, 9, 219, "Buildings", "MessageSpeech MovieTheater_Mother" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setMapTile(6, 9, 220, "Buildings", "MessageSpeech MovieTheater_Child" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setAnimatedMapTile(5, 8, new int[6]
          {
            203,
            203,
            203,
            204,
            204,
            204
          }, 1000L, "Front", (string) null);
          break;
        case 4:
          this.setMapTileIndex(20, 9, 222, "Front");
          this.setMapTileIndex(21, 9, 223, "Front");
          this.setMapTile(20, 10, 238, "Buildings", (string) null);
          this.setMapTile(21, 10, 239, "Buildings", (string) null);
          this.setMapTileIndex(20, 11, 254, "Buildings");
          this.setMapTileIndex(21, 11, (int) byte.MaxValue, "Buildings");
          break;
        case 5:
          this.setAnimatedMapTile(10, 7, new int[4]
          {
            251,
            251,
            251,
            252
          }, 900L, "Buildings", "MessageSpeech MovieTheater_Lupini" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setAnimatedMapTile(10, 6, new int[4]
          {
            235,
            235,
            235,
            236
          }, 900L, "Front", (string) null);
          break;
        case 6:
          this.setAnimatedMapTile(5, 7, new int[4]
          {
            249,
            249,
            249,
            250
          }, 600L, "Buildings", "MessageSpeech MovieTheater_ConcessionMan" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setAnimatedMapTile(5, 6, new int[4]
          {
            233,
            233,
            233,
            234
          }, 600L, "Front", (string) null);
          break;
        case 7:
          this.setMapTile(1, 12, 248, "Buildings", "MessageSpeech MovieTheater_PurpleHairLady");
          this.setMapTile(1, 11, 232, "Front", (string) null);
          break;
        case 8:
          this.setMapTile(3, 8, 247, "Buildings", "MessageSpeech MovieTheater_RedCapGuy" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setMapTile(3, 7, 231, "Front", (string) null);
          break;
        case 9:
          this.setMapTile(2, 11, 253, "Buildings", "MessageSpeech MovieTheater_Governor" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setMapTile(2, 10, 237, "Front", (string) null);
          break;
        case 10:
          this.setMapTile(9, 7, 221, "Buildings", "NPCSpeechMessageNoRadius Gunther MovieTheater_Gunther" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setMapTile(9, 6, 205, "Front", (string) null);
          break;
        case 11:
          this.setMapTile(19, 10, 208, "Buildings", "NPCSpeechMessageNoRadius Marlon MovieTheater_Marlon" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setMapTile(19, 9, 192, "Front", (string) null);
          break;
        case 12:
          this.setMapTile(12, 4, 209, "Buildings", "MessageSpeech MovieTheater_Marcello" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setMapTile(12, 3, 193, "Front", (string) null);
          break;
        case 13:
          this.setMapTile(17, 12, 241, "Buildings", "NPCSpeechMessageNoRadius Morris MovieTheater_Morris" + (random.NextDouble() < 0.5 ? "2" : ""));
          this.setMapTile(17, 11, 225, "Front", (string) null);
          break;
      }
    }

    public static MovieData GetMovieForDate(WorldDate date)
    {
      MovieTheater.GetMovieData();
      string str = (date.Season + "_movie_").ToString();
      long theaterBuildDate = (long) Game1.player.team.theaterBuildDate;
      long totalDays = (long) date.TotalDays;
      long num1 = totalDays / 112L - theaterBuildDate / 112L;
      if (totalDays / 28L % 4L < theaterBuildDate / 28L % 4L)
        --num1;
      int num2 = 0;
      if (MovieTheater._movieData.ContainsKey(str + num1.ToString()))
        return MovieTheater._movieData[str + num1.ToString()];
      foreach (MovieData movieData in MovieTheater._movieData.Values)
      {
        if (movieData != null && movieData.ID.StartsWith(str))
        {
          string[] strArray = movieData.ID.Split('_');
          if (strArray.Length >= 3)
          {
            int result = 0;
            if (int.TryParse(strArray[2], out result) && result > num2)
              num2 = result;
          }
        }
      }
      foreach (MovieData movieForDate in MovieTheater._movieData.Values)
      {
        if (movieForDate.ID == str + (num1 % (long) (num2 + 1)).ToString())
          return movieForDate;
      }
      return MovieTheater._movieData.Values.FirstOrDefault<MovieData>();
    }

    public override void DayUpdate(int dayOfMonth)
    {
      this.ResetTheater();
      this._ResetHangoutPoints();
      base.DayUpdate(dayOfMonth);
    }

    public override void UpdateWhenCurrentLocation(GameTime time)
    {
      if (this._farmerCount != this.farmers.Count)
      {
        this._farmerCount = this.farmers.Count;
        if (Game1.activeClickableMenu is ReadyCheckDialog)
        {
          (Game1.activeClickableMenu as ReadyCheckDialog).closeDialog(Game1.player);
          if (Game1.player.team.movieMutex.IsLockHeld())
            Game1.player.team.movieMutex.ReleaseLock();
        }
      }
      if (this.birds != null)
        this.birds.Update(time);
      base.UpdateWhenCurrentLocation(time);
    }

    public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
      if (this.birds != null)
        this.birds.Draw(b);
      base.drawAboveAlwaysFrontLayer(b);
    }

    public static bool Invite(Farmer farmer, NPC invited_npc)
    {
      if (farmer == null || invited_npc == null)
        return false;
      farmer.team.movieInvitations.Add(new MovieInvitation()
      {
        farmer = farmer,
        invitedNPC = invited_npc
      });
      return true;
    }

    public void ResetTheater()
    {
      MovieTheater._playerHangoutGroup = -1;
      this.RemoveAllPatrons();
      this._playerGroups.Clear();
      this._npcGroups.Clear();
      this._viewingGroups.Clear();
      this._viewingFarmers.Clear();
      this._purchasedConcessions.Clear();
      this._playerInvitedPatrons.Clear();
      this._characterGroupLookup.Clear();
      this._ResetHangoutPoints();
      Game1.player.team.movieMutex.ReleaseLock();
      this._currentState.Set(0);
    }

    public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
    {
      base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
      this.movieViewerLockEvent.Poll();
      this.requestStartMovieEvent.Poll();
      this.startMovieEvent.Poll();
      this.endMovieEvent.Poll();
      if (!Game1.IsMasterGame)
        return;
      for (int index = 0; index < this._viewingFarmers.Count; ++index)
      {
        Farmer viewingFarmer = this._viewingFarmers[index];
        if (!Game1.getOnlineFarmers().Contains(viewingFarmer))
        {
          this._viewingFarmers.RemoveAt(index);
          --index;
        }
        else if (this._currentState.Value == 2 && !this.farmers.Contains(viewingFarmer) && !this.HasFarmerWatchingBroadcastEventReturningHere() && viewingFarmer.currentLocation != null && viewingFarmer.currentLocation.Name != "Temp")
        {
          this._viewingFarmers.RemoveAt(index);
          --index;
        }
      }
      if (this._currentState.Value != 0 && this._viewingFarmers.Count == 0)
      {
        MovieData movieForDate = MovieTheater.GetMovieForDate(Game1.Date);
        Game1.multiplayer.globalChatInfoMessage("MovieEnd", movieForDate.Title);
        this.ResetTheater();
      }
      if (Game1.player.team.movieInvitations == null || this._playerInvitedPatrons.Count() >= 4)
        return;
      foreach (Farmer farmer in this.farmers)
      {
        for (int index = 0; index < Game1.player.team.movieInvitations.Count; ++index)
        {
          MovieInvitation movieInvitation = Game1.player.team.movieInvitations[index];
          if (!movieInvitation.fulfilled && !this._spawnedMoviePatrons.ContainsKey(movieInvitation.invitedNPC.displayName))
          {
            if (MovieTheater._playerHangoutGroup < 0)
              MovieTheater._playerHangoutGroup = Game1.random.Next(this._maxHangoutGroups);
            int playerHangoutGroup = MovieTheater._playerHangoutGroup;
            if (movieInvitation.farmer == farmer && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == farmer)
            {
              Point random = Utility.GetRandom<Point>(this._availableHangoutPoints[playerHangoutGroup]);
              NPC character = this.AddMoviePatronNPC((string) (NetFieldBase<string, NetString>) movieInvitation.invitedNPC.name, 14, 15, 0);
              this._playerInvitedPatrons.Add((string) (NetFieldBase<string, NetString>) character.name, 1);
              this._availableHangoutPoints[playerHangoutGroup].Remove(random);
              int result = 2;
              if (this.map.GetLayer("Paths").Tiles[random.X, random.Y].Properties != null && this.map.GetLayer("Paths").Tiles[random.X, random.Y].Properties.ContainsKey("direction"))
                int.TryParse((string) this.map.GetLayer("Paths").Tiles[random.X, random.Y].Properties["direction"], out result);
              this._destinationPositions[character.Name] = new KeyValuePair<Point, int>(random, result);
              this.PathCharacterToLocation(character, random, result);
              movieInvitation.fulfilled = true;
            }
          }
        }
      }
    }

    public static MovieCharacterReaction GetReactionsForCharacter(
      NPC character)
    {
      if (character == null)
        return (MovieCharacterReaction) null;
      foreach (MovieCharacterReaction movieReaction in MovieTheater.GetMovieReactions())
      {
        if (!(movieReaction.NPCName != character.Name))
          return movieReaction;
      }
      return (MovieCharacterReaction) null;
    }

    public override void checkForMusic(GameTime time)
    {
    }

    public static string GetResponseForMovie(NPC character)
    {
      string responseForMovie = "like";
      MovieData movieForDate = MovieTheater.GetMovieForDate(Game1.Date);
      if (movieForDate == null)
        return (string) null;
      if (movieForDate != null)
      {
        foreach (MovieCharacterReaction movieReaction in MovieTheater.GetMovieReactions())
        {
          if (!(movieReaction.NPCName != character.Name))
          {
            foreach (MovieReaction reaction in movieReaction.Reactions)
            {
              if (reaction.ShouldApplyToMovie(movieForDate, MovieTheater.GetPatronNames()) && reaction.Response != null && reaction.Response.Length > 0)
              {
                responseForMovie = reaction.Response;
                break;
              }
            }
          }
        }
      }
      return responseForMovie;
    }

    public Dialogue GetDialogueForCharacter(NPC character)
    {
      MovieData movieForDate = MovieTheater.GetMovieForDate(Game1.Date);
      if (movieForDate != null)
      {
        foreach (MovieCharacterReaction genericReaction in MovieTheater._genericReactions)
        {
          if (!(genericReaction.NPCName != character.Name))
          {
            using (List<MovieReaction>.Enumerator enumerator = genericReaction.Reactions.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                MovieReaction current = enumerator.Current;
                if (current.ShouldApplyToMovie(movieForDate, MovieTheater.GetPatronNames(), MovieTheater.GetResponseForMovie(character)) && current.Response != null && current.Response.Length > 0 && current.SpecialResponses != null)
                {
                  if (this._currentState.Value == 0 && current.SpecialResponses.BeforeMovie != null)
                    return new Dialogue(this.FormatString(current.SpecialResponses.BeforeMovie.Text), character);
                  if (this._currentState.Value == 1 && current.SpecialResponses.DuringMovie != null)
                    return new Dialogue(this.FormatString(current.SpecialResponses.DuringMovie.Text), character);
                  if (this._currentState.Value == 2)
                  {
                    if (current.SpecialResponses.AfterMovie != null)
                      return new Dialogue(this.FormatString(current.SpecialResponses.AfterMovie.Text), character);
                    break;
                  }
                  break;
                }
              }
              break;
            }
          }
        }
      }
      return (Dialogue) null;
    }

    public string FormatString(string text, params string[] args) => string.Format(text, (object) MovieTheater.GetMovieForDate(Game1.Date).Title, (object) Game1.player.displayName, (object) args);

    public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64);
      PropertyValue action = (PropertyValue) null;
      this.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size)?.Properties.TryGetValue("Action", out action);
      if (action != null)
        return this.performAction((string) action, who, tileLocation);
      foreach (NPC character in this.characters)
      {
        if (character != null && !character.IsMonster && (!who.isRidingHorse() || !(character is Horse)) && character.GetBoundingBox().Intersects(rectangle))
        {
          if (!character.isMoving())
          {
            if (this._playerInvitedPatrons.ContainsKey(character.Name))
            {
              character.faceTowardFarmerForPeriod(5000, 4, false, who);
              Dialogue dialogueForCharacter = this.GetDialogueForCharacter(character);
              if (dialogueForCharacter != null)
              {
                character.CurrentDialogue.Push(dialogueForCharacter);
                Game1.drawDialogue(character);
                character.grantConversationFriendship(Game1.player);
              }
            }
            else if (this._characterGroupLookup.ContainsKey(character.Name))
            {
              if (!this._characterGroupLookup[character.Name])
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_AfterMovieAlone", (object) character.Name));
              else
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_AfterMovie", (object) character.Name));
            }
          }
          return true;
        }
      }
      return base.checkAction(tileLocation, viewport, who);
    }

    protected void _PopulateNPCOnlyGroups(
      List<List<Character>> player_groups,
      List<List<Character>> groups)
    {
      HashSet<string> stringSet = new HashSet<string>();
      foreach (List<Character> playerGroup in player_groups)
      {
        foreach (Character character in playerGroup)
        {
          if (character is NPC)
            stringSet.Add((string) (NetFieldBase<string, NetString>) character.name);
        }
      }
      foreach (List<Character> group in groups)
      {
        foreach (Character character in group)
        {
          if (character is NPC)
            stringSet.Add((string) (NetFieldBase<string, NetString>) character.name);
        }
      }
      Random random = new Random((int) Game1.uniqueIDForThisGame + Game1.Date.TotalDays);
      int num = 0;
      for (int index = 0; index < 2; ++index)
      {
        if (random.NextDouble() < 0.75)
          ++num;
      }
      int index1 = 0;
      if (this._movieStartTime >= 1200)
        index1 = 1;
      if (this._movieStartTime >= 1800)
        index1 = 2;
      string[][] strArray = MovieTheater.possibleNPCGroups[(int) Game1.Date.DayOfWeek][index1];
      if (strArray == null)
        return;
      if (groups.Count > 0 && groups[0].Count == 0)
        groups.RemoveAt(0);
      for (int index2 = 0; index2 < num && groups.Count < 2; ++index2)
      {
        int index3 = random.Next(strArray.Length);
        bool flag1 = true;
        foreach (string str in strArray[index3])
        {
          bool flag2 = false;
          foreach (Farmer allFarmer in Game1.getAllFarmers())
          {
            if (allFarmer.friendshipData.ContainsKey(str))
            {
              flag2 = true;
              break;
            }
          }
          if (!flag2)
          {
            flag1 = false;
            break;
          }
          if (stringSet.Contains(str))
          {
            flag1 = false;
            break;
          }
          if (MovieTheater.GetResponseForMovie(Game1.getCharacterFromName(str)) == "dislike" || MovieTheater.GetResponseForMovie(Game1.getCharacterFromName(str)) == "reject")
          {
            flag1 = false;
            break;
          }
        }
        if (flag1)
        {
          List<Character> characterList = new List<Character>();
          foreach (string str in strArray[index3])
          {
            NPC npc = this.AddMoviePatronNPC(str, 1000, 1000, 2);
            characterList.Add((Character) npc);
            stringSet.Add(str);
            this._characterGroupLookup[str] = strArray[index3].Length > 1;
          }
          groups.Add(characterList);
        }
      }
    }

    public Dictionary<Character, MovieConcession> GetConcessionsDictionary()
    {
      Dictionary<Character, MovieConcession> concessionsDictionary = new Dictionary<Character, MovieConcession>();
      foreach (string key in this._purchasedConcessions.Keys)
      {
        Character characterFromName = (Character) Game1.getCharacterFromName(key);
        if (characterFromName != null && this.GetConcessions().ContainsKey(this._purchasedConcessions[key]))
          concessionsDictionary[characterFromName] = this.GetConcessions()[this._purchasedConcessions[key]];
      }
      return concessionsDictionary;
    }

    protected void _ResetHangoutPoints()
    {
      this._destinationPositions.Clear();
      this._availableHangoutPoints = new Dictionary<int, List<Point>>();
      foreach (int key in this._hangoutPoints.Keys)
        this._availableHangoutPoints[key] = new List<Point>((IEnumerable<Point>) this._hangoutPoints[key]);
    }

    public override void cleanupBeforePlayerExit()
    {
      if (!Game1.eventUp)
        Game1.changeMusicTrack("none");
      this.birds = (PerchingBirds) null;
      base.cleanupBeforePlayerExit();
    }

    public void RequestEndMovie(long uid)
    {
      if (!Game1.IsMasterGame)
        return;
      if (this._currentState.Value == 1)
      {
        this._currentState.Set(2);
        for (int index1 = 0; index1 < this._viewingGroups.Count; ++index1)
        {
          int index2 = Game1.random.Next(this._viewingGroups.Count);
          List<Character> viewingGroup = this._viewingGroups[index1];
          this._viewingGroups[index1] = this._viewingGroups[index2];
          this._viewingGroups[index2] = viewingGroup;
        }
        this._ResetHangoutPoints();
        int num = 0;
        for (int index3 = 0; index3 < this._viewingGroups.Count; ++index3)
        {
          for (int index4 = 0; index4 < this._viewingGroups[index3].Count; ++index4)
          {
            if (this._viewingGroups[index3][index4] is NPC)
            {
              NPC moviePatron = this.GetMoviePatron(this._viewingGroups[index3][index4].Name);
              if (moviePatron != null)
              {
                moviePatron.setTileLocation(new Vector2(14f, (float) (4.0 + (double) num * 1.0)));
                Point random = Utility.GetRandom<Point>(this._availableHangoutPoints[index3]);
                int result = 2;
                if (this.map.GetLayer("Paths").Tiles[random.X, random.Y].Properties.ContainsKey("direction"))
                  int.TryParse((string) this.map.GetLayer("Paths").Tiles[random.X, random.Y].Properties["direction"], out result);
                this._destinationPositions[moviePatron.Name] = new KeyValuePair<Point, int>(random, result);
                this.PathCharacterToLocation(moviePatron, random, result);
                this._availableHangoutPoints[index3].Remove(random);
                ++num;
              }
            }
          }
        }
      }
      Game1.getFarmer(uid).team.endMovieEvent.Fire(uid);
    }

    public void PathCharacterToLocation(NPC character, Point point, int direction)
    {
      if (character.currentLocation != this)
        return;
      character.temporaryController = new PathFindController((Character) character, (GameLocation) this, character.getTileLocationPoint(), direction)
      {
        pathToEndPoint = PathFindController.findPathForNPCSchedules(character.getTileLocationPoint(), point, (GameLocation) this, 30000)
      };
      character.followSchedule = true;
      character.ignoreScheduleToday = true;
    }

    public Dictionary<int, MovieConcession> GetConcessions()
    {
      if (MovieTheater._concessions == null)
      {
        MovieTheater._concessions = new Dictionary<int, MovieConcession>();
        List<ConcessionItemData> concessionItemDataList = Game1.content.Load<List<ConcessionItemData>>("Data\\Concessions");
        List<MovieConcession> movieConcessionList = new List<MovieConcession>();
        foreach (ConcessionItemData data in concessionItemDataList)
          MovieTheater._concessions[data.ID] = new MovieConcession(data);
      }
      return MovieTheater._concessions;
    }

    public bool OnPurchaseConcession(ISalable salable, Farmer who, int amount)
    {
      foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
      {
        if (movieInvitation.farmer == who && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player && this._spawnedMoviePatrons.ContainsKey(movieInvitation.invitedNPC.Name))
        {
          this._purchasedConcessions[movieInvitation.invitedNPC.Name] = (salable as MovieConcession).id;
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_ConcessionPurchased", (object) (salable as MovieConcession).DisplayName, (object) movieInvitation.invitedNPC.displayName));
          return true;
        }
      }
      return false;
    }

    public bool HasInvitedSomeone(Farmer who)
    {
      foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
      {
        if (movieInvitation.farmer == who && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player && this._spawnedMoviePatrons.ContainsKey(movieInvitation.invitedNPC.Name))
          return true;
      }
      return false;
    }

    public bool HasPurchasedConcession(Farmer who)
    {
      if (!this.HasInvitedSomeone(who))
        return false;
      foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
      {
        if (movieInvitation.farmer == who && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player)
        {
          foreach (string key in this._purchasedConcessions.Keys)
          {
            if (key == movieInvitation.invitedNPC.Name && this._spawnedMoviePatrons.ContainsKey(movieInvitation.invitedNPC.Name))
              return true;
          }
        }
      }
      return false;
    }

    public static Farmer GetFirstInvitedPlayer(NPC npc)
    {
      foreach (MovieInvitation movieInvitation in Game1.player.team.movieInvitations)
      {
        if (movieInvitation.invitedNPC.Name == npc.Name)
          return movieInvitation.farmer;
      }
      return (Farmer) null;
    }

    public override void performTouchAction(string fullActionString, Vector2 playerStandingPosition)
    {
      if (fullActionString.Split(' ')[0] == "Theater_Exit")
      {
        this._exitX = int.Parse(fullActionString.Split(' ')[1]) + Town.GetTheaterTileOffset().X;
        this._exitY = int.Parse(fullActionString.Split(' ')[2]) + Town.GetTheaterTileOffset().Y;
        if ((int) (NetFieldBase<int, NetInt>) Game1.player.lastSeenMovieWeek >= Game1.Date.TotalWeeks)
        {
          this._Leave();
        }
        else
        {
          Game1.player.position.Y -= (float) ((Game1.player.Speed + Game1.player.addedSpeed) * 2);
          Game1.player.Halt();
          Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_LeavePrompt"), Game1.currentLocation.createYesNoResponses(), "LeaveMovie");
        }
      }
      else
        base.performTouchAction(fullActionString, playerStandingPosition);
    }

    public List<MovieConcession> GetConcessionsForGuest(string npc_name)
    {
      List<MovieConcession> list1 = new List<MovieConcession>();
      List<MovieConcession> list2 = this.GetConcessions().Values.ToList<MovieConcession>();
      Random rng = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      Utility.Shuffle<MovieConcession>(rng, list2);
      NPC characterFromName = Game1.getCharacterFromName(npc_name);
      if (characterFromName == null)
        return list1;
      int num1 = 1;
      int num2 = 2;
      int num3 = 1;
      int num4 = 5;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        for (int index2 = 0; index2 < list2.Count; ++index2)
        {
          MovieConcession concession = list2[index2];
          if (MovieTheater.GetConcessionTasteForCharacter((Character) characterFromName, concession) == "love" && (!concession.Name.Equals("Stardrop Sorbet") || rng.NextDouble() < 0.33))
          {
            list1.Add(concession);
            list2.RemoveAt(index2);
            int num5 = index2 - 1;
            break;
          }
        }
      }
      for (int index3 = 0; index3 < num2; ++index3)
      {
        for (int index4 = 0; index4 < list2.Count; ++index4)
        {
          MovieConcession concession = list2[index4];
          if (MovieTheater.GetConcessionTasteForCharacter((Character) characterFromName, concession) == "like")
          {
            list1.Add(concession);
            list2.RemoveAt(index4);
            int num6 = index4 - 1;
            break;
          }
        }
      }
      for (int index5 = 0; index5 < num3; ++index5)
      {
        for (int index6 = 0; index6 < list2.Count; ++index6)
        {
          MovieConcession concession = list2[index6];
          if (MovieTheater.GetConcessionTasteForCharacter((Character) characterFromName, concession) == "dislike")
          {
            list1.Add(concession);
            list2.RemoveAt(index6);
            int num7 = index6 - 1;
            break;
          }
        }
      }
      for (int count = list1.Count; count < num4; ++count)
      {
        int index = 0;
        if (index < list2.Count)
        {
          MovieConcession movieConcession = list2[index];
          list1.Add(movieConcession);
          list2.RemoveAt(index);
          int num8 = index - 1;
        }
      }
      if (this._isJojaTheater && !list1.Exists((Predicate<MovieConcession>) (x => x.Name.Equals("JojaCorn"))))
      {
        MovieConcession movieConcession = list2.Find((Predicate<MovieConcession>) (x => x.Name.Equals("JojaCorn")));
        if (movieConcession != null)
          list1.Add(movieConcession);
      }
      Utility.Shuffle<MovieConcession>(rng, list1);
      return list1;
    }

    public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
    {
      switch (questionAndAnswer)
      {
        case "LeaveMovie_Yes":
          this._Leave();
          return true;
        case "Concession_Yes":
          string npc_name = "";
          foreach (MovieInvitation movieInvitation in Game1.player.team.movieInvitations)
          {
            if (movieInvitation.farmer == Game1.player && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player)
              npc_name = movieInvitation.invitedNPC.Name;
          }
          Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(((IEnumerable<ISalable>) this.GetConcessionsForGuest(npc_name)).ToList<ISalable>(), who: "Concessions", on_purchase: new Func<ISalable, Farmer, int, bool>(this.OnPurchaseConcession));
          return true;
        case null:
          return false;
        default:
          return base.answerDialogueAction(questionAndAnswer, questionParams);
      }
    }

    protected void _Leave()
    {
      Game1.player.completelyStopAnimatingOrDoingAction();
      Game1.warpFarmer("Town", this._exitX, this._exitY, 2);
    }

    public override bool performAction(string action, Farmer who, Location tileLocation)
    {
      if (action == "Concessions")
      {
        if (this._currentState.Value > 0)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_ConcessionAfterMovie"));
          return true;
        }
        if (!this.HasInvitedSomeone(who))
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_ConcessionAlone"));
          return true;
        }
        if (this.HasPurchasedConcession(who))
        {
          foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
          {
            if (movieInvitation.farmer == who && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player)
            {
              foreach (string key in this._purchasedConcessions.Keys)
              {
                if (key == movieInvitation.invitedNPC.Name)
                {
                  MovieConcession concessions = this.GetConcessionsDictionary()[(Character) Game1.getCharacterFromName(key)];
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_ConcessionPurchased", (object) concessions.DisplayName, (object) Game1.getCharacterFromName(key).displayName));
                  return true;
                }
              }
            }
          }
          return true;
        }
        Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_Concession"), Game1.currentLocation.createYesNoResponses(), "Concession");
      }
      else
      {
        if (action == "Theater_Doors")
        {
          if (this._currentState.Value > 0)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Theater_MovieEndReEntry"));
            return true;
          }
          if (Game1.player.team.movieMutex.IsLocked())
          {
            this._ShowMovieStartReady();
            return true;
          }
          Game1.player.team.movieMutex.RequestLock((Action) (() =>
          {
            List<Farmer> present_farmers = new List<Farmer>();
            foreach (Farmer farmer in this.farmers)
            {
              if (farmer.isActive() && farmer.currentLocation == this)
                present_farmers.Add(farmer);
            }
            this.movieViewerLockEvent.Fire(new MovieViewerLockEvent(present_farmers, Game1.timeOfDay));
          }));
          return true;
        }
        if (action == "CraneGame")
        {
          if (this.getTileIndexAt(2, 9, "Buildings") == -1)
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:MovieTheater_CranePlay", (object) 500), this.createYesNoResponses(), new GameLocation.afterQuestionBehavior(this.tryToStartCraneGame));
          else
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:MovieTheater_CraneOccupied"));
        }
      }
      return base.performAction(action, who, tileLocation);
    }

    private void tryToStartCraneGame(Farmer who, string whichAnswer)
    {
      if (!(whichAnswer.ToLower() == "yes"))
        return;
      if (Game1.player.Money >= 500)
      {
        Game1.player.Money -= 500;
        Game1.changeMusicTrack("none", music_context: Game1.MusicContext.MiniGame);
        Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => Game1.currentMinigame = (IMinigame) new CraneGame()), 0.008f);
      }
      else
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325"));
    }

    public static void ClearCachedLocalizedData()
    {
      MovieTheater._concessions = (Dictionary<int, MovieConcession>) null;
      MovieTheater._genericReactions = (List<MovieCharacterReaction>) null;
      MovieTheater._movieData = (Dictionary<string, MovieData>) null;
    }

    public enum MovieStates
    {
      Preshow,
      Show,
      PostShow,
    }
  }
}
